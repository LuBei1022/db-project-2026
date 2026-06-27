"""
最小测试脚本：只验证 DeepSeek 能不能调通（不连数据库、不连向量库）。
第一次用 API 时跑这个，确认 key 和网络没问题。

用法：
    python test_llm.py
"""
import sys

import config
import llm_utils


def main():
    print("=" * 50)
    print("DeepSeek API 连通性测试")
    print("=" * 50)

    # 1) 检查配置
    print(f"模型     : {config.LLM_MODEL}")
    print(f"接口地址 : {config.LLM_BASE_URL}")
    if not config.LLM_API_KEY:
        print("\n[失败] 没读到 API Key。")
        print("       请确认同目录下有 .env 文件，且里面填了 LLM_API_KEY=你的key")
        sys.exit(1)
    masked = config.LLM_API_KEY[:6] + "..." + config.LLM_API_KEY[-4:]
    print(f"API Key  : {masked}  (已读到)")

    # 2) 模拟一次 RAG 问答：给一段假的"论文原文"，问一个问题
    fake_context = [
        "本文提出了一种基于检索增强生成(RAG)的文献问答方法，"
        "通过将论文全文向量化存入向量数据库，实现语义级检索与智能问答，"
        "在实验中显著提升了科研人员的文献阅读效率。"
    ]
    question = "这篇论文提出了什么方法？解决了什么问题？"

    print("\n正在调用 DeepSeek ...（第一次可能要几秒）\n")
    try:
        answer = llm_utils.generate_answer(question, fake_context)
    except Exception as e:
        print(f"[失败] 调用出错: {e}")
        print("\n常见原因：")
        print("  - key 填错或没充值")
        print("  - 没装 openai 库：pip install openai")
        print("  - 网络/代理问题")
        sys.exit(1)

    print("-" * 50)
    print("问题:", question)
    print("回答:", answer)
    print("-" * 50)
    print("\n[成功] DeepSeek 调通了，RAG 的生成这步没问题 ✓")


if __name__ == "__main__":
    main()
