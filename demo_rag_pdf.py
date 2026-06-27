"""
真·全文 RAG 演示：直接扫描网站的 PDF 目录，对【真实论文】做全文向量化 + 智能问答。
不需要 SQL Server、不需要建数据库——纯验证你负责的 RAG 部分在真实数据上的效果。

PDF 来源：网站源码/Web/A_UpLoad/upload_file/（按日期分文件夹）

用法：
    python demo_rag_pdf.py            # 默认只索引前 5 篇（快速试跑）
    python demo_rag_pdf.py --all      # 索引全部 PDF（较慢，几分钟）
    python demo_rag_pdf.py --limit 10 # 索引前 10 篇
"""
import os
import sys
import glob

# 用独立向量目录，跟将来对接真库的 chroma_db、以及摘要 demo 都分开
os.environ.setdefault("CHROMA_PERSIST_DIR", "./chroma_demo_pdf")

import config            # noqa: E402
import rag_utils         # noqa: E402
from pdf_parser import extract_full_text_smart  # noqa: E402

PDF_DIR = os.path.join("网站源码", "Web", "A_UpLoad", "upload_file")


def parse_limit():
    if "--all" in sys.argv:
        return None
    if "--limit" in sys.argv:
        try:
            return int(sys.argv[sys.argv.index("--limit") + 1])
        except (ValueError, IndexError):
            pass
    return 5  # 默认快速试跑


def build_index(limit):
    pdfs = sorted(glob.glob(os.path.join(PDF_DIR, "**", "*.pdf"), recursive=True))
    if not pdfs:
        print(f"[失败] 在 {PDF_DIR} 下没找到 PDF。请确认在项目根目录下运行。")
        sys.exit(1)
    if limit:
        pdfs = pdfs[:limit]

    print(f"准备索引 {len(pdfs)} 篇 PDF（首次会下载 m3e 模型，请耐心）...\n")
    id_to_name = {}
    for i, path in enumerate(pdfs, start=1):
        paper_id = i  # 演示用顺序号当作 paper_id（真集成时会换成 Literature.id）
        name = os.path.basename(path)
        id_to_name[paper_id] = name
        print(f"[{i}/{len(pdfs)}] 提取全文: {name}")
        text = extract_full_text_smart(path)
        if not text.strip():
            print("    (提取为空，跳过)")
            continue
        rag_utils.delete_paper_from_vector_db(paper_id)
        rag_utils.add_paper_to_vector_db(paper_id, text)
    print("\n索引建立完成。\n")
    return id_to_name


def print_papers(id_to_name):
    print("已索引论文（编号 = paper_id）：")
    for pid, name in id_to_name.items():
        print(f"  [{pid}] {name}")
    print()


def ask(question, paper_id=None):
    print("-" * 56)
    scope = f"论文 {paper_id}" if paper_id else "全部论文(全局)"
    print(f"提问范围: {scope}\n问题: {question}")
    result = rag_utils.answer_question(question, paper_id=paper_id)
    print(f"\n回答:\n{result['answer']}")
    src = [s["paper_id"] for s in result["sources"]]
    print(f"\n（依据 {len(src)} 段检索内容，来自论文编号 {src}）")
    print("-" * 56 + "\n")


def main():
    if not config.LLM_API_KEY:
        print("[提示] 没读到 LLM_API_KEY，请先在 .env 里配置。")
        return

    id_to_name = build_index(parse_limit())
    print_papers(id_to_name)

    # 自动跑一个全局示例问答
    ask("这些论文里有没有关于机器人或计算机视觉的研究？分别讲了什么？")

    print("进入交互问答（指定论文编号可只问那一篇；直接回车=全局；输入 q 退出）")
    while True:
        try:
            pid = input("论文编号(可留空) > ").strip()
            q = input("你的问题 > ").strip()
        except (EOFError, KeyboardInterrupt):
            print("\n再见。")
            break
        if q.lower() == "q":
            print("再见。")
            break
        if not q:
            continue
        ask(q, paper_id=int(pid) if pid.isdigit() else None)


if __name__ == "__main__":
    main()
