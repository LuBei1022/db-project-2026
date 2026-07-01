(function (window, document) {
    "use strict";

    var fragmentEndings = {
        ge: true,
        geo: true,
        intro: true,
        mani: true,
        conti: true,
        integra: true,
        comple: true,
        complemen: true,
        condi: true,
        gener: true,
        catego: true,
        applica: true,
        simula: true,
        optimi: true,
        fabrica: true,
        molecu: true,
        geome: true,
        direc: true,
        environ: true,
        archi: true,
        mathe: true,
        compu: true,
        automa: true,
        distri: true,
        classi: true,
        repre: true,
        experi: true,
        imple: true,
        evalua: true,
        physi: true,
        physico: true
    };

    function trimLine(value) {
        return String(value || "").replace(/[ \t]+/g, " ").trim();
    }

    function getTrailingWord(value) {
        var match = String(value || "").match(/([A-Za-z]{2,18})-?$/);
        return match ? match[1] : "";
    }

    function startsWithLowerWord(value) {
        return /^[a-z]{2,18}\b/.test(String(value || ""));
    }

    function shouldJoinWithoutSpace(previous, next) {
        if (!previous || !next || !startsWithLowerWord(next)) return false;

        if (/[A-Za-z]-$/.test(previous)) {
            return true;
        }

        var trailing = getTrailingWord(previous).toLowerCase();
        return !!fragmentEndings[trailing];
    }

    function appendLine(previous, next) {
        if (!previous) return next;
        if (!shouldJoinWithoutSpace(previous, next)) {
            return previous + " " + next;
        }

        var trailing = getTrailingWord(previous).toLowerCase();
        if (/[A-Za-z]-$/.test(previous) && fragmentEndings[trailing]) {
            return previous.replace(/-$/, "") + next;
        }
        return previous + next;
    }

    function normalizeAcademicText(value) {
        var text = String(value || "");
        if (!text) return "";

        text = text.replace(/\r\n?/g, "\n").replace(/\u00a0/g, " ");

        var paragraphs = text.split(/\n{2,}/);
        var cleaned = [];
        for (var i = 0; i < paragraphs.length; i++) {
            var lines = paragraphs[i].split("\n");
            var paragraph = "";
            for (var j = 0; j < lines.length; j++) {
                var line = trimLine(lines[j]);
                if (!line) continue;
                paragraph = appendLine(paragraph, line);
            }
            paragraph = paragraph.replace(/[ \t]{2,}/g, " ").trim();
            if (paragraph) cleaned.push(paragraph);
        }

        return cleaned.join("\n\n");
    }

    function isAbstractTextarea(el) {
        if (!el || String(el.tagName || "").toLowerCase() !== "textarea") return false;
        var id = String(el.id || "").toLowerCase();
        if (id.indexOf("abstract_text") >= 0) return true;
        if (el.getAttribute("data-batch-field") === "abstract_text") return true;
        if (el.getAttribute("data-pdf-field") === "abstract_text") return true;
        return false;
    }

    function getClipboardText(event) {
        var original = event || window.event;
        var clipboard = original.clipboardData || window.clipboardData;
        if (!clipboard) return "";
        return clipboard.getData("text") || clipboard.getData("Text") || "";
    }

    function insertText(el, text) {
        var value = el.value || "";
        var start = typeof el.selectionStart === "number" ? el.selectionStart : value.length;
        var end = typeof el.selectionEnd === "number" ? el.selectionEnd : start;
        el.value = value.substring(0, start) + text + value.substring(end);
        var cursor = start + text.length;
        if (el.setSelectionRange) {
            el.setSelectionRange(cursor, cursor);
        }
        if (typeof Event === "function") {
            el.dispatchEvent(new Event("input", { bubbles: true }));
            el.dispatchEvent(new Event("change", { bubbles: true }));
        }
    }

    function onPaste(event) {
        var target = event.target || event.srcElement;
        if (!isAbstractTextarea(target)) return;

        var pasted = getClipboardText(event);
        if (!pasted || pasted.indexOf("\n") < 0) return;

        var cleaned = normalizeAcademicText(pasted);
        if (!cleaned || cleaned === pasted) return;

        if (event.preventDefault) event.preventDefault();
        event.returnValue = false;
        insertText(target, cleaned);
    }

    function install() {
        if (document.__literatureTextNormalizerInstalled) return;
        document.__literatureTextNormalizerInstalled = true;
        if (document.addEventListener) {
            document.addEventListener("paste", onPaste, true);
        } else if (document.attachEvent) {
            document.attachEvent("onpaste", onPaste);
        }
    }

    window.LiteratureTextNormalizer = {
        normalizeAbstract: normalizeAcademicText,
        install: install
    };

    if (document.readyState === "loading") {
        if (document.addEventListener) {
            document.addEventListener("DOMContentLoaded", install);
        } else {
            window.attachEvent && window.attachEvent("onload", install);
        }
    } else {
        install();
    }
})(window, document);
