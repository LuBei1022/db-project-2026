"""
回填脚本：把 SQL Server 里已有的论文一次性灌进向量库，建立 RAG 索引。

逻辑：
  对每篇已审核论文(Literature)：
    1) 尝试找到它的 PDF → 提全文 → 建索引（最理想）
    2) 找不到 PDF/提取失败 → 降级用 标题+摘要+关键词 建索引（保证问答能用）
  每段文本绑定的 paper_id = Literature.id，与网站一致。

用法：
    python backfill_index.py            # 索引所有论文
    python backfill_index.py --rebuild  # 先删旧向量再重建（重复运行时用）

注意：本机没有 PDF 文件也能跑——会自动走降级方案（基于摘要问答）。
等拿到 PDF 存放目录后，在 .env 里设置 PDF_BASE_DIR 再 --rebuild 一次即可升级到全文问答。
"""
import os
import sys

import config
import db_utils
import rag_utils
from pdf_parser import extract_full_text_smart


def resolve_pdf_path(raw_path: str):
    """
    把数据库里存的 file_path 解析成本机能打开的真实路径。
    兼容三种情况：绝对路径 / 相对 PDF_BASE_DIR 的路径 / 带前导斜杠的站内路径。
    找不到返回 None。
    """
    if not raw_path:
        return None
    raw_path = raw_path.strip()

    # 1) 本身就是绝对且存在的路径
    if os.path.isabs(raw_path) and os.path.exists(raw_path):
        return raw_path

    # 2) 拼到 PDF_BASE_DIR 下
    if config.PDF_BASE_DIR:
        candidate = os.path.join(config.PDF_BASE_DIR, raw_path.lstrip("/\\"))
        if os.path.exists(candidate):
            return candidate

    # 3) 相对当前工作目录
    if os.path.exists(raw_path):
        return os.path.abspath(raw_path)

    return None


def get_paper_text(paper_id: int, lit_row: dict):
    """
    取一篇论文用于建索引的文本。
    返回 (text, source)，source 为 'pdf' 或 'metadata' 或 None。
    """
    # 优先 PDF 全文
    try:
        raw_path = db_utils.get_pdf_path_for_literature(paper_id)
    except Exception as e:
        print(f"  [警告] 查询 PDF 路径失败: {e}")
        raw_path = None

    if raw_path:
        real_path = resolve_pdf_path(raw_path)
        if real_path:
            text = extract_full_text_smart(real_path)
            if text and text.strip():
                return text, "pdf"
            print(f"  [提示] PDF 提取为空，降级用元数据: {real_path}")
        else:
            print(f"  [提示] 找不到 PDF 文件({raw_path})，降级用元数据")

    # 降级：标题 + 摘要 + 关键词
    meta_text = db_utils.build_metadata_text(lit_row)
    if meta_text:
        return meta_text, "metadata"
    return None, None


def main():
    rebuild = "--rebuild" in sys.argv

    print("正在从 SQL Server 读取论文列表...")
    papers = db_utils.get_all_literature_for_index()
    print(f"共 {len(papers)} 篇已审核论文。\n")

    stat = {"pdf": 0, "metadata": 0, "skipped": 0}

    for lit in papers:
        pid = lit["id"]
        title = (lit.get("title") or "")[:40]
        print(f"[论文 {pid}] {title}")

        if rebuild:
            rag_utils.delete_paper_from_vector_db(pid)

        text, source = get_paper_text(pid, lit)
        if not text:
            print("  [跳过] 既无 PDF 也无摘要，无法建索引。")
            stat["skipped"] += 1
            continue

        ok = rag_utils.add_paper_to_vector_db(pid, text)
        if ok:
            stat[source] += 1
        else:
            stat["skipped"] += 1

    print("\n===== 回填完成 =====")
    print(f"  全文(PDF)索引: {stat['pdf']} 篇")
    print(f"  降级(摘要)索引: {stat['metadata']} 篇")
    print(f"  跳过: {stat['skipped']} 篇")


if __name__ == "__main__":
    main()
