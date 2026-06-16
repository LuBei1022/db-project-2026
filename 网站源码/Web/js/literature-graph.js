(function () {
    var REFRESH_INTERVAL = 5000;

    function onReady(callback) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", callback);
        } else {
            callback();
        }
    }

    function initWidget(container) {
        if (!container || container.getAttribute("data-graph-ready") === "1") {
            return;
        }
        container.setAttribute("data-graph-ready", "1");

        var apiUrl = container.getAttribute("data-api") || "/Inc/LiteratureGraph.ashx";
        var canvas = container.querySelector(".literature-graph-canvas");
        var status = container.querySelector(".literature-graph-status");
        var empty = container.querySelector(".literature-graph-empty");
        var detail = container.querySelector(".literature-graph-panel");
        var network = null;
        var nodeDataSet = null;
        var edgeDataSet = null;
        var timer = null;

        function setStatus(text) {
            if (status) {
                status.textContent = text || "";
            }
        }

        function setEmpty(active, text) {
            if (!empty) {
                return;
            }
            empty.textContent = text || "";
            empty.className = active ? "literature-graph-empty active" : "literature-graph-empty";
        }

        function setDetail(node) {
            if (!detail) {
                return;
            }

            while (detail.firstChild) {
                detail.removeChild(detail.firstChild);
            }

            var title = document.createElement("h5");
            title.textContent = node ? node.name : "节点详情";
            detail.appendChild(title);

            if (!node) {
                var tip = document.createElement("p");
                tip.textContent = "点击图谱中的文献、作者、分类或出版物节点查看详情。";
                detail.appendChild(tip);
                return;
            }

            var props = node.properties || {};
            var list = document.createElement("div");
            list.className = "literature-graph-detail";
            detail.appendChild(list);

            Object.keys(props).forEach(function (key) {
                if (props[key] === null || props[key] === undefined || props[key] === "") {
                    return;
                }

                var row = document.createElement("div");
                row.className = "literature-graph-detail-row";
                var label = document.createElement("strong");
                var value = document.createElement("span");
                label.textContent = key;
                value.textContent = props[key];
                row.appendChild(label);
                row.appendChild(value);
                list.appendChild(row);
            });
        }

        function normalizeNode(node) {
            var colorMap = {
                Literature: { background: "#e8f2ff", border: "#0066cc" },
                Author: { background: "#ecfdf5", border: "#10b981" },
                Institution: { background: "#f0fdfa", border: "#14b8a6" },
                Venue: { background: "#fff7ed", border: "#f97316" },
                Category: { background: "#f5f3ff", border: "#8b5cf6" }
            };
            var color = colorMap[node.label] || { background: "#f3f4f6", border: "#9ca3af" };
            return {
                id: node.id,
                label: node.name,
                group: node.label,
                title: node.name,
                properties: node.properties || {},
                name: node.name,
                color: color,
                font: { color: "#111827", size: 13, face: "Microsoft YaHei, Arial" },
                borderWidth: node.label === "Literature" ? 2 : 1,
                shape: node.label === "Literature" ? "box" : "ellipse",
                margin: node.label === "Literature" ? 12 : 8
            };
        }

        function normalizeEdge(edge) {
            return {
                id: edge.id,
                from: edge.from,
                to: edge.to,
                label: edge.label || "",
                arrows: edge.arrows || "to",
                color: { color: "#cbd5e1", highlight: "#0066cc" },
                font: { color: "#64748b", size: 11, align: "middle" },
                smooth: { type: "dynamic" }
            };
        }

        function syncDataSet(dataSet, items) {
            var incoming = {};
            for (var i = 0; i < items.length; i++) {
                incoming[items[i].id] = true;
            }

            var currentIds = dataSet.getIds();
            var removeIds = [];
            for (var j = 0; j < currentIds.length; j++) {
                if (!incoming[currentIds[j]]) {
                    removeIds.push(currentIds[j]);
                }
            }

            dataSet.update(items);
            if (removeIds.length) {
                dataSet.remove(removeIds);
            }
        }

        function captureView() {
            if (!network) {
                return null;
            }

            return {
                position: network.getViewPosition(),
                scale: network.getScale()
            };
        }

        function restoreView(view) {
            if (!network || !view) {
                return;
            }

            network.moveTo({
                position: view.position,
                scale: view.scale,
                animation: false
            });
        }

        function render(data) {
            if (!window.vis || !canvas) {
                setEmpty(true, "图谱组件加载失败，请检查 vis-network 资源。");
                return;
            }

            var nodes = (data.nodes || []).map(normalizeNode);
            var edges = (data.edges || []).map(normalizeEdge);
            var previousView = captureView();
            if (!nodes.length) {
                setEmpty(true, "暂无可展示的文献关系数据。");
            } else {
                setEmpty(false);
            }

            if (!nodeDataSet) {
                nodeDataSet = new window.vis.DataSet(nodes);
            } else {
                syncDataSet(nodeDataSet, nodes);
            }

            if (!edgeDataSet) {
                edgeDataSet = new window.vis.DataSet(edges);
            } else {
                syncDataSet(edgeDataSet, edges);
            }

            var graphData = {
                nodes: nodeDataSet,
                edges: edgeDataSet
            };

            var options = {
                autoResize: true,
                layout: { improvedLayout: true },
                physics: {
                    enabled: true,
                    stabilization: { iterations: 120 },
                    barnesHut: {
                        gravitationalConstant: -3200,
                        centralGravity: 0.25,
                        springLength: 135,
                        springConstant: 0.045,
                        damping: 0.16
                    }
                },
                interaction: {
                    hover: true,
                    tooltipDelay: 120,
                    navigationButtons: false,
                    keyboard: false
                },
                edges: {
                    width: 1.2,
                    selectionWidth: 2
                }
            };

            if (network) {
                restoreView(previousView);
                window.setTimeout(function () {
                    restoreView(previousView);
                }, 60);
            } else {
                network = new window.vis.Network(canvas, graphData, options);
                network.on("click", function (params) {
                    if (!params.nodes || !params.nodes.length) {
                        setDetail(null);
                        return;
                    }
                    var selectedNode = graphData.nodes.get(params.nodes[0]);
                    setDetail(selectedNode);
                });
            }

            setStatus("节点 " + nodes.length + " 个，关系 " + edges.length + " 条\n更新时间 " + (data.update_time || ""));
        }

        function loadGraph() {
            setStatus("正在读取文献关系...");
            fetch(apiUrl, { method: "GET", credentials: "same-origin" })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("HTTP " + response.status);
                    }
                    return response.json();
                })
                .then(function (data) {
                    if (!data || data.code !== 200) {
                        throw new Error((data && data.msg) || "图谱数据异常");
                    }
                    render(data);
                })
                .catch(function (error) {
                    setStatus("读取失败：" + error.message);
                    setEmpty(true, "文献关系图谱暂时无法加载。");
                });
        }

        function bindTools() {
            var fitBtn = container.querySelector("[data-graph-action='fit']");
            var zoomInBtn = container.querySelector("[data-graph-action='zoom-in']");
            var zoomOutBtn = container.querySelector("[data-graph-action='zoom-out']");

            if (fitBtn) {
                fitBtn.onclick = function () {
                    if (network) {
                        network.fit({ animation: true });
                    }
                };
            }
            if (zoomInBtn) {
                zoomInBtn.onclick = function () {
                    if (network) {
                        network.moveTo({ scale: network.getScale() * 1.2, animation: true });
                    }
                };
            }
            if (zoomOutBtn) {
                zoomOutBtn.onclick = function () {
                    if (network) {
                        network.moveTo({ scale: network.getScale() * 0.82, animation: true });
                    }
                };
            }
        }

        bindTools();
        setDetail(null);
        loadGraph();
        timer = window.setInterval(loadGraph, REFRESH_INTERVAL);

        window.addEventListener("beforeunload", function () {
            if (timer) {
                window.clearInterval(timer);
            }
            if (network) {
                network.destroy();
                network = null;
            }
        });

        if (window.location.search.indexOf("graph=1") >= 0) {
            window.setTimeout(function () {
                container.scrollIntoView({ behavior: "smooth", block: "start" });
            }, 350);
        }
    }

    onReady(function () {
        var widgets = document.querySelectorAll(".literature-graph-widget");
        for (var i = 0; i < widgets.length; i++) {
            initWidget(widgets[i]);
        }
    });
})();
