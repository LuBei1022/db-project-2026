"""
RAG 微服务（Flask）。独立运行，默认端口 5051。
网站(ASP.NET)端只需通过 HTTP 调这里的接口即可接入“智能问答”功能。

接口一览：
  GET  /health                       健康检查
  GET  /rag/search_paper?title=xxx   按标题搜论文（供问答页选论文，返回 Literature.id）
  POST /rag/ask                      智能问答：传 {paper_id, question} → 返回 {answer, sources}
  GET  /rag/search?q=xxx&paper_id=   纯语义检索（只返回相关原文段落，不生成回答）

说明：原 MySQL 原型里的 /upload、/search/author 等接口已移除，
那些是早期原型代码，与真网站的 SQL Server 表结构不匹配。
真正的论文上传/检索由 ASP.NET 网站负责，本服务只专注 RAG。
"""
from flask import Flask, request, jsonify
import os

import rag_utils
import db_utils

app = Flask(__name__)


@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok", "service": "rag"}), 200


@app.route("/rag/search_paper", methods=["GET"])
def search_paper():
    """按标题模糊搜索论文，供前端问答页选定一篇论文。"""
    title = request.args.get("title", "")
    if not title.strip():
        return jsonify({"error": "缺少标题关键词 (title)"}), 400
    try:
        papers = db_utils.search_literature_by_title(title)
        return jsonify({"keyword": title, "papers": papers}), 200
    except Exception as e:
        return jsonify({"error": f"查询论文失败: {e}"}), 500


@app.route("/rag/ask", methods=["POST"])
def rag_ask():
    """
    智能问答主接口。
    请求体 (JSON 或表单)：
      - question : 用户问题（必填）
      - paper_id : 论文 id（可选；传了就只在该篇内问答，不传则全局问答）
    """
    data = request.get_json(silent=True) or request.form
    question = (data.get("question") or "").strip()
    paper_id = data.get("paper_id")

    if not question:
        return jsonify({"error": "缺少问题内容 (question)"}), 400

    # paper_id 容错转换
    try:
        paper_id = int(paper_id) if paper_id not in (None, "", "null") else None
    except (TypeError, ValueError):
        return jsonify({"error": "paper_id 必须是整数"}), 400

    try:
        result = rag_utils.answer_question(question, paper_id=paper_id)
        paper_title = None
        if paper_id:
            lit = db_utils.get_literature_by_id(paper_id)
            paper_title = lit["title"] if lit else None
        return jsonify({
            "question": question,
            "paper_id": paper_id,
            "paper_title": paper_title,
            "answer": result["answer"],
            "sources": result["sources"],
        }), 200
    except Exception as e:
        return jsonify({"error": f"问答失败: {e}"}), 500


@app.route("/rag/index_paper", methods=["POST"])
def rag_index_paper():
    """
    重建单篇论文索引。
    网站上传或后台导入成功后可以异步调用；失败只影响 RAG，不应阻断上传流程。
    """
    data = request.get_json(silent=True) or request.form
    paper_id = data.get("paper_id")
    try:
        paper_id = int(paper_id)
    except (TypeError, ValueError):
        return jsonify({"error": "paper_id 必须是整数"}), 400

    lit = db_utils.get_literature_by_id(paper_id)
    if not lit:
        return jsonify({"error": f"未找到论文 ID {paper_id}"}), 404

    try:
        from backfill_index import get_paper_text

        text, source = get_paper_text(paper_id, lit)
        if not text:
            return jsonify({"error": "该论文既无 PDF 全文也无可用元数据，无法建索引"}), 422
        rag_utils.delete_paper_from_vector_db(paper_id)
        ok = rag_utils.add_paper_to_vector_db(paper_id, text)
        if not ok:
            return jsonify({"error": "写入向量库失败"}), 500
        return jsonify({"status": "ok", "paper_id": paper_id, "source": source}), 200
    except Exception as e:
        return jsonify({"error": f"重建索引失败: {e}"}), 500


@app.route("/rag/search", methods=["GET"])
def rag_search():
    """纯语义检索：只返回相关原文段落，不调用大模型生成。"""
    query = request.args.get("q", "")
    paper_id = request.args.get("paper_id", type=int)
    if not query.strip():
        return jsonify({"error": "缺少查询内容 (q)"}), 400
    try:
        results = rag_utils.search_similar_texts(query=query, paper_id=paper_id)
        return jsonify({
            "query": query,
            "target_paper_id": paper_id if paper_id else "全局搜索",
            "results": results,
        }), 200
    except Exception as e:
        return jsonify({"error": f"检索失败: {e}"}), 500


if __name__ == "__main__":
    app.run(debug=True, host="0.0.0.0", port=int(os.getenv("RAG_PORT", "5051")))
