"""
大模型调用封装（DeepSeek，OpenAI 兼容接口）。
RAG 里的 "G"（Generation，生成）就在这里。
"""
import config

# openai 库用 1.x 新版写法；未安装时给出清晰提示
try:
    from openai import OpenAI
    _OPENAI_AVAILABLE = True
except Exception:
    _OPENAI_AVAILABLE = False

_client = None


def _get_client():
    """惰性创建客户端，避免没配 key 时一导入就报错。"""
    global _client
    if not _OPENAI_AVAILABLE:
        raise RuntimeError("未安装 openai 库，请先执行：pip install openai")
    if not config.LLM_API_KEY:
        raise RuntimeError("未配置 LLM_API_KEY，请在 .env 里填入你的 DeepSeek API Key")
    if _client is None:
        _client = OpenAI(
            api_key=config.LLM_API_KEY,
            base_url=config.LLM_BASE_URL,
            timeout=config.LLM_TIMEOUT,
        )
    return _client


SYSTEM_PROMPT = (
    "你是一个严谨的学术文献助手。请只依据【参考资料】里的内容回答用户问题，"
    "用中文作答，条理清晰。如果参考资料里找不到答案，就直说“根据现有资料无法回答”，"
    "不要编造。对于论文结构、主要内容、贡献总结类问题，可以根据标题、摘要、引言、方法、实验、"
    "结果和结论等参考资料线索进行归纳；资料不足时说明不足。回答末尾不需要重复列出参考资料。"
)


def generate_answer(question: str, context_chunks: list, temperature: float = 0.2, is_overview: bool = False) -> str:
    """
    把检索到的原文段落(context_chunks)和用户问题(question)一起交给大模型，生成回答。

    参数:
    - question: 用户的问题
    - context_chunks: 字符串列表，每一项是一段检索到的论文原文
    返回:
    - 大模型生成的回答字符串
    """
    if not context_chunks:
        return "根据现有资料无法回答：没有检索到与问题相关的论文内容。"

    # 把召回的多段原文编号拼成参考资料
    context_text = "\n\n".join(
        f"[资料{i + 1}]\n{chunk}" for i, chunk in enumerate(context_chunks)
    )

    task_hint = ""
    if is_overview:
        task_hint = (
            "用户当前问题偏向论文结构/概览。请优先从研究问题、核心方法、实验验证、"
            "主要结论四个角度组织回答；如果参考资料包含章节标题，也可以据此梳理。"
        )

    user_prompt = (
        f"【参考资料】\n{context_text}\n\n"
        f"【用户问题】\n{question}\n\n"
        f"{task_hint}\n请依据上述参考资料回答。"
    )

    client = _get_client()
    resp = client.chat.completions.create(
        model=config.LLM_MODEL,
        messages=[
            {"role": "system", "content": SYSTEM_PROMPT},
            {"role": "user", "content": user_prompt},
        ],
        temperature=temperature,
        stream=False,
    )
    return resp.choices[0].message.content.strip()
