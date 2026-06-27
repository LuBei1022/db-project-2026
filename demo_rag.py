"""
离线演示脚本：不连 SQL Server、不需要 PDF，用内置样例论文把整条
RAG 链路（向量检索 + DeepSeek 生成回答）跑通。

适合在数据库还没搭好时，单独验证/演示你负责的 RAG 部分。

用法：
    python demo_rag.py
（第一次会下载 m3e 嵌入模型，约几百 MB，请耐心等）
"""
import os

# 用独立的向量目录，避免污染将来对接真库的 chroma_db
os.environ.setdefault("CHROMA_PERSIST_DIR", "./chroma_demo")

import config           # noqa: E402  (要在设置完环境变量后再导入)
import rag_utils        # noqa: E402

# ===== 内置样例论文（模拟数据库里的 Literature 记录）=====
# paper_id 就当作 Literature.id 用
SAMPLE_PAPERS = [
    {
        "id": 1001,
        "title": "基于RAG的学术文献智能问答系统",
        "text": (
            "标题：基于RAG的学术文献智能问答系统\n"
            "摘要：本文提出一种结合检索增强生成(RAG)的文献问答方法。系统先将论文全文"
            "切分成小段并用嵌入模型向量化，存入向量数据库；用户提问时，先用语义检索"
            "找到最相关的若干段原文，再交给大语言模型生成回答。实验表明，该方法相比"
            "传统关键词检索，能更准确地回答开放式问题，显著提升科研人员的文献阅读效率。"
            "本文的主要贡献包括：提出了面向中文文献的切块策略；设计了带出处引用的回答格式。"
        ),
    },
    {
        "id": 1002,
        "title": "知识图谱在科研合作网络分析中的应用",
        "text": (
            "标题：知识图谱在科研合作网络分析中的应用\n"
            "摘要：本文基于Neo4j构建学术知识图谱，将文献、作者、机构、关键词建模为节点，"
            "将合作、发表、隶属关系建模为边。通过图查询与可视化，揭示科研合作网络的结构特征，"
            "支持对高产作者、核心机构和研究热点的深度挖掘。结果显示，图结构相比关系型表格"
            "更适合表达多跳的学术关联关系。"
        ),
    },
    {
        "id": 1003,
        "title": "Transformer架构综述",
        "text": (
            "标题：Transformer架构综述\n"
            "摘要：Transformer是一种完全基于自注意力机制(self-attention)的神经网络架构，"
            "摒弃了循环和卷积结构。其核心是多头注意力机制，能并行处理序列并捕捉长距离依赖。"
            "本文综述了Transformer在自然语言处理中的发展，包括BERT、GPT等代表性模型，"
            "并讨论了其在计算复杂度上的挑战与改进方向。"
        ),
    },
]


def build_index():
    print("正在为样例论文建立向量索引（首次会下载嵌入模型，请稍候）...\n")
    for p in SAMPLE_PAPERS:
        rag_utils.delete_paper_from_vector_db(p["id"])   # 先清旧的，避免重复
        rag_utils.add_paper_to_vector_db(p["id"], p["text"])
    print("\n索引建立完成。\n")


def print_papers():
    print("可提问的样例论文：")
    for p in SAMPLE_PAPERS:
        print(f"  [{p['id']}] {p['title']}")
    print()


def ask(question, paper_id=None):
    print("-" * 56)
    scope = f"论文 {paper_id}" if paper_id else "全部论文(全局)"
    print(f"提问范围: {scope}")
    print(f"问题: {question}")
    result = rag_utils.answer_question(question, paper_id=paper_id)
    print(f"\n回答:\n{result['answer']}")
    print(f"\n（依据 {len(result['sources'])} 段检索内容，分别来自论文 "
          f"{[s['paper_id'] for s in result['sources']]}）")
    print("-" * 56 + "\n")


def main():
    if not config.LLM_API_KEY:
        print("[提示] 没读到 LLM_API_KEY，请先在 .env 里配置（你之前 test_llm 已经通过，应该没问题）")
        return

    build_index()
    print_papers()

    # 先自动跑一个示例问答，确认链路通
    ask("这个RAG系统是怎么工作的？主要贡献有哪些？", paper_id=1001)

    # 再进入交互模式
    print("进入交互问答（直接回车用全局搜索；输入 q 退出）")
    while True:
        try:
            pid = input("指定论文ID(可留空) > ").strip()
            q = input("你的问题 > ").strip()
        except (EOFError, KeyboardInterrupt):
            print("\n再见。")
            break
        if q.lower() == "q":
            print("再见。")
            break
        if not q:
            continue
        paper_id = int(pid) if pid.isdigit() else None
        ask(q, paper_id=paper_id)


if __name__ == "__main__":
    main()
