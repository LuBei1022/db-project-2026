import hashlib
import json
import os
import re
import ssl
import tempfile
import urllib.error
import urllib.request
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from typing import Dict, List, Optional, Sequence, Set, Tuple
import unicodedata
import pdfplumber

# ========== 可选依赖：Flask ==========
try:
    from flask import Flask, jsonify, request
    from werkzeug.utils import secure_filename
except Exception:
    Flask = None
    jsonify = None
    request = None
    def secure_filename(filename):
        keep = []
        for char in filename or "":
            if char.isalnum() or char in {".", "-", "_"}:
                keep.append(char)
        return "".join(keep).strip("._") or "upload.pdf"

# ========== 可选依赖：PDF转图片（扫描件用） ==========
try:
    from pdf2image import convert_from_path
    from PIL import Image  # 显式检查 Pillow
    _SCANNED_SUPPORT_AVAILABLE = True
except Exception:
    convert_from_path = None
    Image = None
    _SCANNED_SUPPORT_AVAILABLE = False

# ========== 可选依赖：wordninja（分词用） ==========
try:
    import wordninja
except Exception:
    wordninja = None

# ===================== 配置常量 =====================
UPLOAD_FOLDER = os.getenv("LLM_PDF_UPLOAD_FOLDER", "uploads")
ALLOWED_EXTENSIONS = {"pdf"}
MAX_PDF_BYTES = int(os.getenv("PDF_PARSE_MAX_BYTES", str(50 * 1024 * 1024)))
HOST = os.getenv("PDF_PARSE_HOST", "127.0.0.1")
PORT = int(os.getenv("PDF_PARSE_PORT", "5050"))

MAX_TEXT_PAGES = int(os.getenv("LLM_PDF_TEXT_PAGES", "2"))
MAX_TEXT_CHARS = int(os.getenv("LLM_PDF_TEXT_CHARS", "26000"))
LLM_TIMEOUT_SECONDS = int(os.getenv("LLM_TIMEOUT_SECONDS", "60"))
LLM_TEMPERATURE = float(os.getenv("LLM_TEMPERATURE", "0"))
LLM_MAX_TOKENS = int(os.getenv("LLM_MAX_TOKENS", "2048"))
LLM_CACHE_ENABLED = os.getenv("LLM_PDF_CACHE", "1").strip().lower() not in {"0", "false", "no"}

PROMPT_VERSION = "pdf-metadata-combined-v1"
JOURNAL_SOURCE_TYPE = "期刊论文"
CONFERENCE_SOURCE_TYPE = "会议论文"
OTHER_SOURCE_TYPE = "其他"

# Poppler 路径（扫描件转图片用，Windows 必须配置环境变量）
POPPLER_PATH = os.getenv("POPPLER_PATH", None)

# 扫描件判定阈值（已放宽，减少误判）
SCAN_TEXT_RATIO_THRESHOLD = 0.15   # 有效字符占比低于此值认为是扫描件
SCAN_MIN_CHARS = 30                # 有效字符数低于此值认为是扫描件

DEFAULT_EXTRACTED = {
    "title": "Unknown Title",
    "authors": [],
    "institutions": [],
    "author_details": [],
    "author_affiliations": [],
    "keywords": [],
    "abstract": "",
    "journal": "",
    "conference": "",
    "doi": "",
    "publish_year": "",
    "publish_month": "",
    "publish_day": "",
    "publish_date_precision": "unknown",
    "source_type": OTHER_SOURCE_TYPE,
    "volume": "",
    "issue": "",
    "pages": "",
    "publisher": "",
    "page_count": "",
}

# ===================== pdf_parser 规则引擎核心代码 =====================
AFFILIATION_HINTS = (
    "university", "institute", "institution", "college", "school", "department",
    "faculty", "laboratory", "lab", "center", "centre", "academy", "hospital",
    "group", "research", "公司", "大学", "学院", "研究", "实验室", "中心", "医院",
)
FRONT_MATTER_NOISE_HINTS = (
    "open access version", "computer vision foundation", "accepted version",
    "published version of the proceedings", "provided by the",
    "except for this watermark", "authorized licensed use",
    "all rights reserved", "copyright",
)
AUTHOR_NAME_BAD_WORDS = {
    "agent", "based", "model", "models", "high", "institute", "language",
    "university", "stanford", "technology",
}
MAX_AUTHOR_COUNT = 30
ABSTRACT_MAX_LENGTH = 8000


def normalize_text(text: str) -> str:
    text = (text or "").replace("\x00", " ").replace("\ufffd", " ")
    text = text.replace("\ufb00", "ff").replace("\ufb01", "fi").replace("\ufb02", "fl")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    text = re.sub(r"\?{2,}", " ", text)
    text = "".join(
        " " if unicodedata.category(char) in {"Cc", "Cf", "Cs", "Co", "Cn"} else char
        for char in text
    )
    text = text.replace("\r", "\n").replace("\u00a0", " ")
    text = re.sub(r"[ \t]+", " ", text)
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def readable_char_count(text: str) -> int:
    return sum(1 for char in text if char.isalnum() or "\u4e00" <= char <= "\u9fff")


def is_readable_text(text: str, min_readable: int = 2, min_ratio: float = 0.35) -> bool:
    compact = re.sub(r"\s+", "", text or "")
    if not compact:
        return False
    readable = readable_char_count(compact)
    return readable >= min_readable and readable / float(len(compact)) >= min_ratio


def split_joined_english_token(token: str) -> str:
    token = re.sub(r"([a-z])([A-Z])", r"\1 \2", token)
    token = re.sub(r"([A-Z]{2,})([A-Z][a-z])", r"\1 \2", token)
    if wordninja is None:
        return token
    pieces = []
    for part in token.split():
        if len(part) >= 13 and re.fullmatch(r"[A-Za-z]+", part) and not re.fullmatch(r"[A-Z]{2,}", part):
            split_parts = wordninja.split(part)
            pieces.append(" ".join(split_parts) if len(split_parts) > 1 else part)
        else:
            pieces.append(part)
    return " ".join(pieces)


def restore_word_spaces(text: str) -> str:
    text = re.sub(
        r"[A-Za-z]{10,}",
        lambda m: split_joined_english_token(m.group(0)),
        text or "",
    )
    return re.sub(r"\s+", " ", text).strip()


def clean_paragraph(text: str) -> str:
    text = normalize_text(text)
    text = text.replace("-\n", "").replace("\n", " ")
    text = re.sub(r"([a-z])([A-Z])", r"\1 \2", text)
    text = re.sub(r"([A-Z]{2,})([A-Z][a-z])", r"\1 \2", text)
    text = re.sub(r"([,.;:!?])(?=[A-Za-z])", r"\1 ", text)
    text = re.sub(r"\s+", " ", text)
    return restore_word_spaces(text.strip(" ,;:-"))


def clean_extracted_field(text: str, max_length: Optional[int] = None) -> str:
    text = clean_paragraph(text)
    text = re.sub(r"\s+([,.;:!?])", r"\1", text).strip(" ,;:-")
    if max_length is not None and len(text) > max_length:
        text = text[:max_length].rsplit(" ", 1)[0].strip(" ,;:-") or text[:max_length].strip(" ,;:-")
    return text if is_readable_text(text) else ""


def clean_extracted_list(items: Sequence[str], max_items: int = 8) -> List[str]:
    cleaned, seen = [], set()
    for item in items:
        current = clean_extracted_field(item, 180)
        if not current or (len(current) < 3 and not re.search(r"[\u4e00-\u9fff]", current)):
            continue
        key = current.lower()
        if key in seen:
            continue
        seen.add(key)
        cleaned.append(current)
        if len(cleaned) >= max_items:
            break
    return cleaned


def normalize_line(line: str) -> str:
    line = normalize_text(line)
    line = re.sub(r"\s+", " ", line)
    return restore_word_spaces(line.strip(" ,;:-"))


def split_lines(text: str) -> List[str]:
    return [normalize_line(raw) for raw in (text or "").split("\n") if normalize_line(raw)]


def looks_like_author_name(token: str) -> bool:
    token = normalize_line(re.sub(r"\d+$", "", token or ""))
    if not token or len(token) < 2 or any(char.isdigit() for char in token):
        return False
    lower = token.lower()
    if any(hint in lower for hint in AFFILIATION_HINTS + FRONT_MATTER_NOISE_HINTS):
        return False
    if re.fullmatch(r"[\u4e00-\u9fff]{2,8}", token):
        return True
    return bool(
        re.fullmatch(
            r"(?:[A-Z][A-Za-z'`-]+|[A-Z]\.)(?:\s+(?:[A-Z][A-Za-z'`-]+|[A-Z]\.)){1,4}",
            token,
        )
    )


def extract_paper_info_rule_based(pdf_path: str) -> Optional[Dict]:
    """纯规则提取元数据（精简版核心逻辑）"""
    try:
        with pdfplumber.open(pdf_path) as pdf:
            if not pdf.pages:
                return None
            metadata = pdf.metadata or {}
            page_count = len(pdf.pages)

            full_text = ""
            first_page_lines = []
            for i, page in enumerate(pdf.pages[:3]):
                raw = page.extract_text(x_tolerance=2, y_tolerance=3) or ""
                if i == 0:
                    first_page_lines = split_lines(raw)
                full_text += raw + "\n"

            # 文本不可读，标记为可能是扫描件
            if not is_readable_text(
                full_text,
                min_readable=SCAN_MIN_CHARS,
                min_ratio=SCAN_TEXT_RATIO_THRESHOLD,
            ):
                return None

            # 标题提取
            title = "Unknown Title"
            for line in first_page_lines[:10]:
                lower = line.lower()
                if (
                    len(line) > 12
                    and not lower.startswith(("copyright", "open access", "doi", "arxiv"))
                    and not looks_like_author_name(line)
                ):
                    title = clean_extracted_field(line, 260)
                    break

            # 作者提取
            authors = []
            for line in first_page_lines[1:18]:
                if "," in line and len(line) < 220:
                    parts = [p.strip() for p in re.split(r"[,;]", line) if p.strip()]
                    candidates = [p for p in parts if looks_like_author_name(p)]
                    if len(candidates) >= 2:
                        authors = candidates
                        break

            # 摘要提取
            abstract = ""
            abs_match = re.search(
                r"(?i)abstract\s*[:：]?\s*(.+?)(?=\s{2,}keywords|\s{2,}introduction)",
                full_text,
                re.DOTALL,
            )
            if abs_match:
                abstract = clean_extracted_field(abs_match.group(1), ABSTRACT_MAX_LENGTH)

            # DOI 提取
            doi = ""
            doi_match = re.search(r"\b10\.\d{4,9}/[-._;()/:A-Z0-9]+\b", full_text, re.I)
            if doi_match:
                doi = doi_match.group(0).strip().lower()

            return {
                "title": title or "Unknown Title",
                "authors": clean_extracted_list(authors, MAX_AUTHOR_COUNT),
                "institutions": [],
                "author_details": [],
                "author_affiliations": [],
                "keywords": [],
                "abstract": abstract,
                "journal": "",
                "conference": "",
                "doi": doi,
                "publish_year": "",
                "publish_month": "",
                "publish_day": "",
                "publish_date_precision": "unknown",
                "source_type": OTHER_SOURCE_TYPE,
                "volume": "",
                "issue": "",
                "pages": "",
                "publisher": "",
                "page_count": str(page_count),
            }
    except Exception as exc:
        print(f"Rule-based parse error: {exc}")
        return None


# ===================== LLM / VLM 通用工具函数 =====================
def allowed_file(filename):
    return "." in filename and filename.rsplit(".", 1)[1].lower() in ALLOWED_EXTENSIONS


def hash_file(path):
    digest = hashlib.sha256()
    with open(path, "rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def build_cache_key(file_hash, mode="rule"):
    model = get_llm_model() if mode != "vlm" else get_vlm_model()
    return f"{PROMPT_VERSION}:{mode}:{model}:{file_hash}"


def cache_path():
    return os.path.join(UPLOAD_FOLDER, "llm_pdf_cache.json")


def load_cached_result(key):
    if not LLM_CACHE_ENABLED:
        return None
    path = cache_path()
    if not os.path.exists(path):
        return None
    try:
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)
        val = data.get(key)
        return val if isinstance(val, dict) else None
    except Exception:
        return None


def save_cached_result(key, info):
    os.makedirs(UPLOAD_FOLDER, exist_ok=True)
    path = cache_path()
    try:
        data = {}
        if os.path.exists(path):
            with open(path, "r", encoding="utf-8") as f:
                existing = json.load(f)
                if isinstance(existing, dict):
                    data = existing
        data[key] = info
        with open(path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
    except Exception as exc:
        print(f"Cache write warning: {exc}")


def clean_context_text(value):
    text = (value or "").replace("\x00", " ").replace("\u00a0", " ")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    lines = []
    for raw_line in text.replace("\r", "\n").split("\n"):
        line = re.sub(r"[ \t]+", " ", raw_line).strip()
        if line:
            lines.append(line)
    return "\n".join(lines)


def extract_pdf_context(pdf_path):
    page_texts, metadata, page_count = [], {}, 0
    with pdfplumber.open(pdf_path) as pdf:
        metadata = {
            str(k): clean_context_text(str(v))
            for k, v in (pdf.metadata or {}).items()
            if v
        }
        page_count = len(pdf.pages)
        for index, page in enumerate(pdf.pages[:max(1, MAX_TEXT_PAGES)]):
            raw = page.extract_text(x_tolerance=2, y_tolerance=3) or ""
            cleaned = clean_context_text(raw)
            if cleaned:
                page_texts.append(f"[Page {index + 1}]\n{cleaned}")
    text = "\n\n".join(page_texts).strip()
    if len(text) > MAX_TEXT_CHARS:
        text = text[:MAX_TEXT_CHARS]
    return {
        "metadata": metadata,
        "page_count": page_count,
        "text": text,
        "filename": os.path.basename(pdf_path),
    }


# ---- LLM 配置（文本模型） ----
def get_llm_model():
    return os.getenv("LLM_MODEL") or os.getenv("OPENAI_MODEL") or "gpt-4o-mini"


def get_llm_api_url():
    explicit = os.getenv("LLM_API_URL")
    if explicit:
        return explicit
    base = os.getenv("LLM_BASE_URL") or os.getenv("OPENAI_BASE_URL") or "https://api.openai.com/v1"
    return base.rstrip("/") + "/chat/completions"


def get_llm_api_key():
    return os.getenv("LLM_API_KEY") or os.getenv("OPENAI_API_KEY") or ""


# ---- VLM 配置（视觉模型，扫描件用） ----
def get_vlm_model():
    return os.getenv("VLM_MODEL") or "deepseek-vl-chat"


def get_vlm_api_url():
    explicit = os.getenv("VLM_API_URL")
    if explicit:
        return explicit
    base = os.getenv("VLM_BASE_URL") or "https://api.deepseek.com/v1"
    return base.rstrip("/") + "/chat/completions"


def get_vlm_api_key():
    return os.getenv("VLM_API_KEY") or get_llm_api_key()


def is_local_url(url):
    lower = (url or "").lower()
    return "://127.0.0.1" in lower or "://localhost" in lower


# SSL 上下文（全局复用）
_ssl_context = ssl.create_default_context()


def post_json(url, payload, api_key):
    body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
    headers = {
        "Content-Type": "application/json; charset=utf-8",
        "Accept": "application/json",
    }
    if api_key:
        headers["Authorization"] = "Bearer " + api_key

    req = urllib.request.Request(url, data=body, headers=headers, method="POST")
    with urllib.request.urlopen(req, timeout=LLM_TIMEOUT_SECONDS, context=_ssl_context) as resp:
        raw = resp.read().decode("utf-8", "replace")
    return json.loads(raw)


def extract_chat_content(response):
    choices = response.get("choices") if isinstance(response, dict) else None
    if not choices:
        raise RuntimeError("LLM response has no choices")
    message = choices[0].get("message") or {}
    content = message.get("content", "")
    if isinstance(content, list):
        pieces = []
        for item in content:
            if isinstance(item, dict):
                pieces.append(str(item.get("text") or item.get("content") or ""))
            else:
                pieces.append(str(item))
        return "\n".join(pieces)
    return str(content or "")


def load_model_json(content):
    text = (content or "").strip()
    if not text:
        raise RuntimeError("LLM response content is empty")
    try:
        return json.loads(text)
    except Exception:
        pass
    fenced = re.search(r"```(?:json)?\s*(\{.*?\})\s*```", text, re.I | re.DOTALL)
    if fenced:
        return json.loads(fenced.group(1))
    start, end = text.find("{"), text.rfind("}")
    if start >= 0 and end > start:
        return json.loads(text[start : end + 1])
    raise RuntimeError("LLM response is not JSON")


# ===================== 文本 LLM 解析 =====================
def build_prompt(context):
    schema = dict(DEFAULT_EXTRACTED)
    schema["page_count"] = str(context.get("page_count") or "")
    schema["author_details"] = [
        {
            "name": "Author Name",
            "name_cn": "",
            "name_en": "Author Name",
            "affiliations": ["Institution Name"],
            "affiliation_text": "Institution Name",
            "markers": ["1"],
            "mapping_status": "matched",
        }
    ]
    return (
        "Extract metadata from the following academic PDF text.\n"
        "Return only a valid JSON object. Do not add markdown.\n"
        "source_type must be one of: 期刊论文, 会议论文, 其他.\n"
        "For author_details, map each author to affiliations when possible.\n\n"
        f"JSON schema example:\n{json.dumps(schema, ensure_ascii=False, indent=2)}\n\n"
        f"PDF metadata:\n{json.dumps(context.get('metadata') or {}, ensure_ascii=False, indent=2)}\n\n"
        f"PDF text:\n{context.get('text') or ''}"
    )


def call_llm_for_metadata(context):
    api_url = get_llm_api_url()
    api_key = get_llm_api_key()
    if not api_key and not is_local_url(api_url):
        raise RuntimeError("Set LLM_API_KEY before starting service")

    payload = {
        "model": get_llm_model(),
        "messages": [
            {
                "role": "system",
                "content": "You extract academic paper metadata from PDF text. Return only valid JSON.",
            },
            {"role": "user", "content": build_prompt(context)},
        ],
        "temperature": LLM_TEMPERATURE,
        "max_tokens": LLM_MAX_TOKENS,
    }
    use_fmt = os.getenv("LLM_RESPONSE_FORMAT", "json_object").strip().lower()
    if use_fmt not in {"0", "false", "no", "none"}:
        payload["response_format"] = {"type": "json_object"}

    try:
        response = post_json(api_url, payload, api_key)
    except urllib.error.HTTPError as exc:
        if "response_format" not in payload or exc.code not in {400, 404, 422}:
            raise
        payload.pop("response_format", None)
        response = post_json(api_url, payload, api_key)

    return load_model_json(extract_chat_content(response))


# ===================== 视觉 VLM 解析（扫描件专用） =====================
def pdf_first_pages_to_base64(pdf_path, max_pages=2):
    if not _SCANNED_SUPPORT_AVAILABLE:
        raise RuntimeError(
            "Scanned PDF support unavailable. Install dependencies: "
            "pip install pdf2image pillow, and ensure poppler is in PATH."
        )

    kwargs = {}
    if POPPLER_PATH:
        kwargs["poppler_path"] = POPPLER_PATH

    images = convert_from_path(pdf_path, first_page=1, last_page=max_pages, **kwargs)
    import io
    import base64

    b64_list = []
    for img in images:
        buf = io.BytesIO()
        img.save(buf, format="JPEG", quality=85)
        b64 = base64.b64encode(buf.getvalue()).decode("utf-8")
        b64_list.append(b64)
    return b64_list


def call_vlm_for_metadata(pdf_path):
    api_url = get_vlm_api_url()
    api_key = get_vlm_api_key()
    if not api_key and not is_local_url(api_url):
        raise RuntimeError("Set VLM_API_KEY for scanned PDF parsing")

    images_b64 = pdf_first_pages_to_base64(pdf_path, max_pages=2)
    if not images_b64:
        raise RuntimeError("Failed to convert PDF to images")

    schema = dict(DEFAULT_EXTRACTED)
    schema["author_details"] = [
        {
            "name": "Author Name",
            "name_cn": "",
            "name_en": "Author Name",
            "affiliations": ["Institution Name"],
            "affiliation_text": "Institution Name",
            "markers": ["1"],
            "mapping_status": "matched",
        }
    ]

    content = [
        {
            "type": "text",
            "text": (
                "You are an academic paper metadata extractor. Analyze the first pages of this paper PDF (images). "
                "Extract title, authors, affiliations, abstract, keywords, journal/conference name, DOI, publication year. "
                "Return ONLY a valid JSON object matching this schema:\n"
                f"{json.dumps(schema, ensure_ascii=False, indent=2)}\n"
                "Use empty string / empty array for missing fields. Do not output markdown code blocks."
            ),
        }
    ]
    for b64 in images_b64:
        content.append({"type": "image_url", "image_url": {"url": f"data:image/jpeg;base64,{b64}"}})

    payload = {
        "model": get_vlm_model(),
        "messages": [{"role": "user", "content": content}],
        "temperature": LLM_TEMPERATURE,
        "max_tokens": LLM_MAX_TOKENS,
    }

    response = post_json(api_url, payload, api_key)
    return load_model_json(extract_chat_content(response))


# ===================== 归一化与结果整理 =====================
def clean_value(value):
    if value is None:
        return ""
    if isinstance(value, (dict, list)):
        value = json.dumps(value, ensure_ascii=False)
    text = str(value)
    text = text.replace("\x00", " ").replace("\ufffd", " ").replace("\u00a0", " ")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    text = re.sub(r"[ \t\r\n]+", " ", text)
    text = re.sub(r"\s+([,.;:!?])", r"\1", text)
    return text.strip(" ,;:-")


def limit_text(value, max_length):
    text = clean_value(value)
    if max_length and len(text) > max_length:
        return text[:max_length].strip(" ,;:-")
    return text


def contains_chinese(value):
    return any("\u4e00" <= char <= "\u9fff" for char in value or "")


def normalize_author_details_list(raw_list):
    """归一化 author_details 数组"""
    result = []
    if not isinstance(raw_list, list):
        return result
    for item in raw_list:
        if not isinstance(item, dict):
            continue
        name = clean_value(item.get("name") or item.get("name_cn") or item.get("name_en"))
        if not name:
            continue
        name_cn = clean_value(item.get("name_cn", ""))
        name_en = clean_value(item.get("name_en", ""))
        if not name_cn and contains_chinese(name):
            name_cn = name
        if not name_en and not contains_chinese(name):
            name_en = name

        aff_raw = item.get("affiliations") or item.get("affiliation") or item.get("institution") or []
        if isinstance(aff_raw, str):
            affiliations = [clean_value(a) for a in re.split(r"[;；,，]", aff_raw) if clean_value(a)]
        elif isinstance(aff_raw, list):
            affiliations = [clean_value(a) for a in aff_raw if clean_value(a)]
        else:
            affiliations = []

        markers = item.get("markers", [])
        if isinstance(markers, list):
            markers = [str(m) for m in markers if str(m).strip()]
        else:
            markers = [str(markers)] if str(markers).strip() else []

        status = clean_value(item.get("mapping_status", "")) or ("matched" if affiliations else "unmatched")

        result.append({
            "name": name,
            "name_cn": name_cn,
            "name_en": name_en,
            "affiliations": affiliations[:8],
            "affiliation_text": "；".join(affiliations[:8]),
            "markers": markers[:8],
            "mapping_status": status,
        })
    return result[:30]


def normalize_extracted(raw, context):
    """将 LLM/VLM 返回的原始 JSON 归一化为标准格式（完整保留所有字段）"""
    raw = raw if isinstance(raw, dict) else {}
    info = dict(DEFAULT_EXTRACTED)

    info["title"] = limit_text(clean_value(raw.get("title")), 260) or "Unknown Title"
    info["abstract"] = limit_text(clean_value(raw.get("abstract")), 8000)
    info["doi"] = clean_value(raw.get("doi") or raw.get("DOI"))
    info["journal"] = limit_text(clean_value(raw.get("journal") or raw.get("journal_name")), 260)
    info["conference"] = limit_text(clean_value(raw.get("conference") or raw.get("conference_name")), 260)
    info["publish_year"] = clean_value(raw.get("publish_year") or raw.get("year"))
    info["publish_month"] = clean_value(raw.get("publish_month") or raw.get("month"))
    info["publish_day"] = clean_value(raw.get("publish_day") or raw.get("day"))
    info["volume"] = limit_text(clean_value(raw.get("volume")), 60)
    info["issue"] = limit_text(clean_value(raw.get("issue")), 60)
    info["pages"] = limit_text(clean_value(raw.get("pages")), 80)
    info["publisher"] = limit_text(clean_value(raw.get("publisher")), 120)
    info["page_count"] = clean_value(raw.get("page_count")) or str(context.get("page_count") or "")

    # 作者列表
    authors_raw = raw.get("authors") or []
    if isinstance(authors_raw, list):
        info["authors"] = [clean_value(a) for a in authors_raw if clean_value(a)][:30]

    # 关键词
    keywords_raw = raw.get("keywords") or []
    if isinstance(keywords_raw, list):
        info["keywords"] = [clean_value(k) for k in keywords_raw if clean_value(k)][:10]
    elif isinstance(keywords_raw, str):
        info["keywords"] = [clean_value(k) for k in re.split(r"[,;，；]", keywords_raw) if clean_value(k)][:10]

    # 机构列表
    inst_raw = raw.get("institutions") or []
    if isinstance(inst_raw, list):
        info["institutions"] = [clean_value(i) for i in inst_raw if clean_value(i)][:12]

    # 作者详情（完整保留）
    author_details = normalize_author_details_list(
        raw.get("author_details") or raw.get("author_affiliations") or []
    )
    info["author_details"] = author_details
    info["author_affiliations"] = author_details

    # 若 authors 为空但 author_details 有数据，反向填充
    if not info["authors"] and author_details:
        info["authors"] = [d["name"] for d in author_details if d.get("name")][:30]

    # 若 institutions 为空但 author_details 有数据，反向填充
    if not info["institutions"] and author_details:
        seen = set()
        for d in author_details:
            for aff in d.get("affiliations", []):
                if aff and aff.lower() not in seen:
                    seen.add(aff.lower())
                    info["institutions"].append(aff)
        info["institutions"] = info["institutions"][:12]

    # 来源类型推断
    st = clean_value(raw.get("source_type", "")).lower()
    if "期刊" in st or "journal" in st:
        info["source_type"] = JOURNAL_SOURCE_TYPE
    elif "会议" in st or "conference" in st or "proceedings" in st:
        info["source_type"] = CONFERENCE_SOURCE_TYPE
    elif info["journal"]:
        info["source_type"] = JOURNAL_SOURCE_TYPE
    elif info["conference"]:
        info["source_type"] = CONFERENCE_SOURCE_TYPE
    else:
        info["source_type"] = OTHER_SOURCE_TYPE

    # 日期精度
    precision = clean_value(raw.get("publish_date_precision") or raw.get("precision", "")).lower()
    if precision in {"day", "month", "year", "unknown"}:
        info["publish_date_precision"] = precision
    else:
        if info["publish_day"] and info["publish_month"]:
            info["publish_date_precision"] = "day"
        elif info["publish_month"]:
            info["publish_date_precision"] = "month"
        elif info["publish_year"]:
            info["publish_date_precision"] = "year"
        else:
            info["publish_date_precision"] = "unknown"

    return info


# ===================== 主调度逻辑 =====================
def is_scanned_pdf(pdf_path):
    """判断 PDF 是否为扫描件（无可读文本）"""
    try:
        with pdfplumber.open(pdf_path) as pdf:
            text = ""
            for page in pdf.pages[:2]:
                text += page.extract_text() or ""
            return not is_readable_text(
                text,
                min_readable=SCAN_MIN_CHARS,
                min_ratio=SCAN_TEXT_RATIO_THRESHOLD,
            )
    except Exception:
        return True  # 解析失败保守认为是扫描件


def result_is_good(info):
    """判断规则解析结果是否足够好，无需调用 LLM"""
    if not info:
        return False
    title_ok = (
        info.get("title")
        and info["title"] != "Unknown Title"
        and len(info["title"]) > 10
    )
    authors_ok = len(info.get("authors", [])) >= 2
    return title_ok and authors_ok


def extract_paper_info_smart(pdf_path):
    """智能解析入口：规则 → 文本LLM → 视觉VLM 三级降级"""
    rule_result = None
    try:
        file_hash = hash_file(pdf_path)

        # 1. 尝试规则解析（最快，无成本）
        rule_cache_key = build_cache_key(file_hash, mode="rule")
        cached_rule = load_cached_result(rule_cache_key)
        if cached_rule:
            rule_result = cached_rule
        else:
            rule_result = extract_paper_info_rule_based(pdf_path)
            if rule_result and LLM_CACHE_ENABLED:
                save_cached_result(rule_cache_key, rule_result)

        if result_is_good(rule_result):
            return rule_result

        # 2. 判断是否扫描件
        scanned = is_scanned_pdf(pdf_path)

        if not scanned:
            # 非扫描件：调用文本 LLM 增强解析
            llm_cache_key = build_cache_key(file_hash, mode="llm")
            cached_llm = load_cached_result(llm_cache_key)
            if cached_llm:
                return cached_llm

            context = extract_pdf_context(pdf_path)
            if not context["text"]:
                return rule_result or None

            raw = call_llm_for_metadata(context)
            info = normalize_extracted(raw, context)

            if LLM_CACHE_ENABLED:
                save_cached_result(llm_cache_key, info)
            return info
        else:
            # 扫描件：调用视觉 VLM 解析
            vlm_cache_key = build_cache_key(file_hash, mode="vlm")
            cached_vlm = load_cached_result(vlm_cache_key)
            if cached_vlm:
                return cached_vlm

            raw = call_vlm_for_metadata(pdf_path)
            try:
                with pdfplumber.open(pdf_path) as pdf:
                    page_count = len(pdf.pages)
            except Exception:
                page_count = 0
            context = {"page_count": page_count}
            info = normalize_extracted(raw, context)

            if LLM_CACHE_ENABLED:
                save_cached_result(vlm_cache_key, info)
            return info

    except Exception as exc:
        print(f"Smart PDF parse error: {exc}")
        return rule_result


# ===== 兼容原 pdf_parser.py 的入口函数名 =====
extract_paper_info = extract_paper_info_smart


def parse_uploaded_pdf(filepath):
    info = extract_paper_info_smart(filepath)
    if not info:
        return {"error": "Failed to extract info"}, 500
    return {"paper_id": None, "extracted": info}, 200


# ===================== Web 服务层 =====================
def create_flask_app():
    app = Flask(__name__)
    app.config["UPLOAD_FOLDER"] = UPLOAD_FOLDER
    app.config["MAX_CONTENT_LENGTH"] = MAX_PDF_BYTES
    os.makedirs(UPLOAD_FOLDER, exist_ok=True)

    @app.route("/upload", methods=["POST"])
    def upload_paper():
        if "file" not in request.files:
            return jsonify({"error": "No file part"}), 400
        file = request.files["file"]
        if file.filename == "":
            return jsonify({"error": "No selected file"}), 400
        if not allowed_file(file.filename):
            return jsonify({"error": "File type not allowed"}), 400

        filename = secure_filename(file.filename)
        fd, filepath = tempfile.mkstemp(
            prefix="llm_pdf_", suffix="_" + filename, dir=app.config["UPLOAD_FOLDER"]
        )
        try:
            with os.fdopen(fd, "wb") as output:
                file.save(output)
            payload, status_code = parse_uploaded_pdf(filepath)
            return jsonify(payload), status_code
        finally:
            if os.path.exists(filepath):
                os.remove(filepath)

    @app.route("/health", methods=["GET"])
    def health():
        return jsonify(
            {
                "ok": True,
                "service": "combined-pdf-parser",
                "model": get_llm_model(),
                "vlm_model": get_vlm_model(),
                "scanned_support": _SCANNED_SUPPORT_AVAILABLE,
            }
        )

    return app


class PdfParseHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path.split("?", 1)[0] == "/health":
            self._write_json(
                {
                    "ok": True,
                    "service": "combined-pdf-parser",
                    "model": get_llm_model(),
                    "vlm_model": get_vlm_model(),
                    "scanned_support": _SCANNED_SUPPORT_AVAILABLE,
                },
                200,
            )
        else:
            self._write_json({"error": "Not found"}, 404)

    def do_POST(self):
        if self.path.split("?", 1)[0] != "/upload":
            self._write_json({"error": "Not found"}, 404)
            return
        content_type = self.headers.get("Content-Type", "")
        if "multipart/form-data" not in content_type:
            self._write_json({"error": "No file part"}, 400)
            return

        boundary = None
        for part in content_type.split(";"):
            part = part.strip()
            if part.startswith("boundary="):
                boundary = part.split("=", 1)[1].strip('"').encode("utf-8")
                break
        if not boundary:
            self._write_json({"error": "No file part"}, 400)
            return

        length = int(self.headers.get("Content-Length", "0") or "0")
        if length <= 0 or length > MAX_PDF_BYTES:
            self._write_json({"error": "PDF file is too large"}, 413)
            return

        body = self.rfile.read(length)
        filename, file_content = None, None
        marker = b"--" + boundary
        for part in body.split(marker):
            if b"filename=" not in part:
                continue
            header, _, content = part.partition(b"\r\n\r\n")
            if not content:
                continue
            content = content.rsplit(b"\r\n", 1)[0]
            disp = header.decode("utf-8", "ignore").split("\r\n", 1)[0]
            for hp in disp.split(";"):
                hp = hp.strip()
                if hp.startswith("filename="):
                    filename = hp.split("=", 1)[1].strip('"') or "upload.pdf"
                    break
            file_content = content
            break

        if not file_content:
            self._write_json({"error": "No selected file"}, 400)
            return
        if not allowed_file(filename or "upload.pdf"):
            self._write_json({"error": "File type not allowed"}, 400)
            return

        os.makedirs(UPLOAD_FOLDER, exist_ok=True)
        safe_name = secure_filename(filename or "upload.pdf")
        fd, filepath = tempfile.mkstemp(
            prefix="llm_pdf_", suffix="_" + safe_name, dir=UPLOAD_FOLDER
        )
        try:
            with os.fdopen(fd, "wb") as output:
                output.write(file_content)
            payload, status_code = parse_uploaded_pdf(filepath)
            self._write_json(payload, status_code)
        finally:
            if os.path.exists(filepath):
                os.remove(filepath)

    def log_message(self, format, *args):
        return

    def _write_json(self, payload, status_code):
        data = json.dumps(payload, ensure_ascii=False).encode("utf-8")
        self.send_response(status_code)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(data)))
        self.end_headers()
        self.wfile.write(data)


def main():
    os.makedirs(UPLOAD_FOLDER, exist_ok=True)
    if Flask is not None:
        app = create_flask_app()
        app.run(host=HOST, port=PORT)
        return
    server = ThreadingHTTPServer((HOST, PORT), PdfParseHandler)
    print(f"Combined PDF parse service listening on http://{HOST}:{PORT}")
    server.serve_forever()


if __name__ == "__main__":
    main()