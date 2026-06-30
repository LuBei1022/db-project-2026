import re
import unicodedata
from typing import Dict, List, Optional, Sequence, Set, Tuple
import os
import warnings

import pdfplumber

try:
    import wordninja
except Exception:
    wordninja = None

# ---------------------- 新增导入（兼容处理）----------------------
try:
    from pdf2image import convert_from_path
    import pytesseract
    from PIL import Image
    import sympy
    from sympy.parsing.latex import parse_latex

    OCR_AVAILABLE = True
except ImportError as e:
    warnings.warn(f"OCR/数学符号处理依赖缺失：{e}，将降级为原有文本解析逻辑")
    OCR_AVAILABLE = False


# OCR
ENABLE_OCR = OCR_AVAILABLE
# 数学符号保留开关
ENABLE_MATH_SYMBOL_PRESERVE = True
# Tesseract路径
TESSERACT_CMD = None
if TESSERACT_CMD:
    pytesseract.pytesseract.tesseract_cmd = TESSERACT_CMD


AFFILIATION_HINTS = (
    "university", "institute", "institution", "college", "school", "department",
    "faculty", "laboratory", "lab", "center", "centre", "academy", "hospital",
    "group",
    "research", "公司", "大学", "学院", "研究", "实验室", "中心", "医院",
)

JOURNAL_HINTS = (
    "journal", "journalof", "transactions", "letters", "magazine", "review",
    "期刊", "学报",
)

CONFERENCE_HINTS = (
    "conference", "symposium", "workshop", "proceedings", "meeting",
    "cvpr", "iccv", "eccv", "siggraph", "neurips", "nips", "iclr", "icml",
    "aaai", "ijcai", "chi", "uist", "iros", "icra", "会议", "论坛",
)

FRONT_MATTER_NOISE_HINTS = (
    "open access version",
    "computer vision foundation",
    "accepted version",
    "published version of the proceedings",
    "provided by the",
    "except for this watermark",
    "authorized licensed use",
    "all rights reserved",
    "copyright",
)

AUTHOR_NAME_BAD_WORDS = {
    "agent", "aggregation", "articulated", "asset", "based", "clip", "conv",
    "decoupling", "direct", "effective", "fidelity", "generation", "group",
    "high", "institute", "language", "memory", "model", "models", "monocular",
    "offset", "recovery", "relief", "scalable", "shanghai", "stanford",
    "support", "table", "technology", "ulip", "uni", "university", "universityof",
    "usa",
}

STOP_SECTION_HINTS = (
    "abstract", "摘要", "keywords", "keyword", "index terms", "introduction",
    "1.", "i.", "doi", "arxiv",
)

KNOWN_CONFERENCE_LABELS = {
    "cvpr": "CVPR",
    "iccv": "ICCV",
    "eccv": "ECCV",
    "siggraph": "SIGGRAPH",
    "neurips": "NeurIPS",
    "nips": "NeurIPS",
    "iclr": "ICLR",
    "icml": "ICML",
    "aaai": "AAAI",
    "ijcai": "IJCAI",
    "chi": "CHI",
    "uist": "UIST",
    "iros": "IROS",
    "icra": "ICRA",
}

MAX_AUTHOR_COUNT = 30

#新增数学符号。
MATH_SYMBOLS = {
    # 基础数学符号
    "+", "-", "×", "÷", "=", "≠", "≈", "≤", "≥", "<", ">", "∞", "π", "∑", "∏",
    "∫", "∬", "∭", "∇", "Δ", "∂", "ℕ", "ℤ", "ℚ", "ℝ", "ℂ", "⊥", "∥", "∠", "°",
    # 逻辑符号
    "∧", "∨", "¬", "⇒", "⇔", "∀", "∃",
    # 希腊字母
    "α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι", "κ", "λ", "μ", "ν", "ξ", "ο",
    "π", "ρ", "σ", "τ", "υ", "φ", "χ", "ψ", "ω",
    "Α", "Β", "Γ", "Δ", "Ε", "Ζ", "Η", "Θ", "Ι", "Κ", "Λ", "Μ", "Ν", "Ξ", "Ο",
    "Π", "Ρ", "Σ", "Τ", "Υ", "Φ", "Χ", "Ψ", "Ω",
    # 其他常见符号
    "%", "‰", "√", "∛", "∜", "!", "∩", "∪", "∈", "∉", "⊂", "⊃", "⊆", "⊇"
}



def normalize_text(text: str) -> str:
    text = (text or "").replace("\x00", " ").replace("\ufffd", " ")
    text = text.replace("\ufb00", "ff").replace("\ufb01", "fi").replace("\ufb02", "fl")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    text = re.sub(r"\?{2,}", " ", text)
    # 优化：保留数学符号，仅过滤真正的坏字符
    text = "".join(" " if is_bad_pdf_char(char) and char not in MATH_SYMBOLS else char for char in text)
    text = text.replace("\r", "\n").replace("\u00a0", " ")
    text = re.sub(r"[ \t]+", " ", text)
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def is_bad_pdf_char(char: str) -> bool:
    if char in "\n\t" or char in MATH_SYMBOLS:  # 排除数学符号
        return False
    category = unicodedata.category(char)
    return category in {"Cc", "Cf", "Cs", "Co", "Cn"} or char in {"■", "□", "◊", "◆", "◇", "❖"}


def readable_char_count(text: str) -> int:
    # 优化：数学符号计入可读字符
    return sum(
        1 for char in text
        if char.isalnum() or "\u4e00" <= char <= "\u9fff" or char in MATH_SYMBOLS
    )



def is_scanned_pdf(page) -> bool:
    """检测页面是否为扫描件（无文本字符，只有图像）"""
    try:
        # 检查是否有可提取的字符
        chars = page.chars or []
        if len(chars) > 50:  # 有足够文本字符，不是扫描件
            return False

        # 检查页面是否有图像
        images = page.images or []
        if len(images) == 0:
            # 极端情况：字符少但也无图像，降级为非扫描件
            return False

        # 图像占比超过页面50%，判定为扫描件
        page_area = page.width * page.height
        image_area = sum(img["width"] * img["height"] for img in images)
        return image_area / page_area > 0.5
    except Exception:
        return False


def ocr_page_to_text(page, pdf_path: str, page_num: int) -> str:
    """对扫描件页面进行OCR识别"""
    if not ENABLE_OCR:
        return extract_page_text(page)

    try:
        # 将PDF页面转为图像
        images = convert_from_path(
            pdf_path,
            first_page=page_num + 1,  # pdf2image页码从1开始
            last_page=page_num + 1,
            poppler_path=os.getenv("POPPLER_PATH"),  # 兼容Windows
            grayscale=True,  # 提高OCR准确率
            dpi=300  # 高分辨率扫描
        )
        if not images:
            return extract_page_text(page)

        # OCR识别（支持中英+数学符号）
        custom_config = r'--oem 3 --psm 6 -c preserve_interword_spaces=1'
        ocr_text = pytesseract.image_to_string(images[0], config=custom_config, lang='chi_sim+eng')

        # 归一化OCR结果
        return normalize_text(ocr_text)
    except Exception as e:
        warnings.warn(f"OCR处理失败：{e}，降级为原有文本解析")
        return extract_page_text(page)


# ---------------------- 新增：数学符号解析辅助函数 ----------------------
def parse_math_symbols(text: str) -> List[str]:
    """提取文本中的数学符号并标准化"""
    if not ENABLE_MATH_SYMBOL_PRESERVE:
        return []

    # 匹配连续数学符号/公式片段
    math_pattern = re.compile(
        r'[' + re.escape(''.join(MATH_SYMBOLS)) + r']+[a-zA-Z0-9]*[' + re.escape(''.join(MATH_SYMBOLS)) + r']*')
    math_fragments = math_pattern.findall(text)

    # 标准化数学符号（如将×转为*，便于后续处理）
    normalized_math = []
    for frag in math_fragments:
        normalized = frag.replace("×", "*").replace("÷", "/")
        try:
            # 尝试用sympy解析验证公式合法性
            parse_latex(normalized)
            normalized_math.append(normalized)
        except Exception:
            normalized_math.append(frag)

    return normalized_math


# ---------------------- 原有核心函数重构：兼容扫描件 ----------------------
def extract_page_text(page) -> str:
    try:
        chars = page.chars or []
    except Exception:
        chars = []

    if not chars:
        return normalize_text(page.extract_text(x_tolerance=2, y_tolerance=3) or "")

    rows: List[List[dict]] = []
    current_row: List[dict] = []
    current_top: Optional[float] = None
    for char in sorted(chars,
                       key=lambda item: (round(float(item.get("top", 0) or 0), 1), float(item.get("x0", 0) or 0))):
        top = float(char.get("top", 0) or 0)
        if current_top is None or abs(top - current_top) <= 3:
            current_row.append(char)
            current_top = top if current_top is None else min(current_top, top)
        else:
            rows.append(current_row)
            current_row = [char]
            current_top = top
    if current_row:
        rows.append(current_row)

    line_texts: List[str] = []
    for row in rows:
        row = sorted(row, key=lambda item: float(item.get("x0", 0) or 0))
        pieces: List[str] = []
        previous_x1: Optional[float] = None
        previous_char = ""
        for char in row:
            value = char.get("text", "")
            if not value:
                continue
            x0 = float(char.get("x0", 0) or 0)
            x1 = float(char.get("x1", 0) or 0)
            width = max(0.1, x1 - x0)
            if previous_x1 is not None:
                gap = x0 - previous_x1
                if gap > max(1.2, width * 0.55):
                    pieces.append(" ")
                elif previous_char and previous_char.isalnum() and value.isalnum() and gap > max(0.6, width * 0.18):
                    pieces.append(" ")
            pieces.append(value)
            previous_x1 = x1
            previous_char = value
        line = normalize_line("".join(pieces))
        if line:
            line_texts.append(line)
    return "\n".join(line_texts)


def extract_page_text_reading_order(page, pdf_path: str = "", page_num: int = 0) -> str:
    """
    重构：支持扫描件OCR，保留原有双列检测逻辑
    :param page: pdfplumber页面对象
    :param pdf_path: PDF文件路径（OCR需要）
    :param page_num: 页码（从0开始，OCR需要）
    :return: 解析后的文本
    """
    # 第一步：检测是否为扫描件，若是则走OCR
    if is_scanned_pdf(page):
        return ocr_page_to_text(page, pdf_path, page_num)

    # 原有双列检测逻辑
    if not looks_like_two_column_page(page):
        return extract_page_text(page)

    try:
        width = float(page.width)
        height = float(page.height)
        left = page.crop((0, 0, width * 0.505, height))
        right = page.crop((width * 0.495, 0, width, height))
        parts = [extract_page_text(left), extract_page_text(right)]
        return "\n".join(part for part in parts if part)
    except Exception:
        return extract_page_text(page)


# ---------------------- 原有函数保持不变（以下为原有代码）----------------------
def is_readable_text(text: str, min_readable: int = 2, min_ratio: float = 0.35) -> bool:
    compact = re.sub(r"\s+", "", text or "")
    if not compact:
        return False
    readable = readable_char_count(compact)
    return readable >= min_readable and readable / float(len(compact)) >= min_ratio


def split_joined_english_token(token: str) -> str:
    token = re.sub(r"(IEEE)([A-Z])", r"\1 \2", token)
    token = re.sub(r"(ACM)([A-Z])", r"\1 \2", token)
    token = re.sub(r"(JOURNAL)(OF)", r"\1 \2", token)
    token = re.sub(r"([a-z])([A-Z])", r"\1 \2", token)
    token = re.sub(r"([A-Z]{2,})([A-Z][a-z])", r"\1 \2", token)
    if wordninja is None:
        return token
    pieces: List[str] = []
    for part in token.split():
        if len(part) >= 13 and re.fullmatch(r"[A-Za-z]+", part) and not re.fullmatch(r"[A-Z]{2,}", part):
            split_parts = wordninja.split(part)
            pieces.append(" ".join(split_parts) if len(split_parts) > 1 else part)
        else:
            pieces.append(part)
    return " ".join(pieces)


def restore_word_spaces(text: str) -> str:
    text = re.sub(r"[A-Za-z]{10,}", lambda match: split_joined_english_token(match.group(0)), text or "")
    return re.sub(r"\s+", " ", text).strip()


def clean_paragraph(text: str) -> str:
    text = normalize_text(text)
    text = text.replace("-\n", "")
    text = text.replace("\n", " ")
    text = re.sub(r"([a-z])([A-Z])", r"\1 \2", text)
    text = re.sub(r"([A-Z]{2,})([A-Z][a-z])", r"\1 \2", text)
    text = re.sub(r"([A-Za-z])(\()", r"\1 \2", text)
    text = re.sub(r"(\))([A-Za-z])", r"\1 \2", text)
    text = re.sub(r"([,.;:!?])(?=[A-Za-z])", r"\1 ", text)
    text = re.sub(r"\s+", " ", text)
    text = re.sub(r"\b([A-Z]{2,})\s+([A-Z])s\b", r"\1\2s", text)
    return restore_word_spaces(text.strip(" ,;:-"))


def clean_extracted_field(text: str, max_length: Optional[int] = None) -> str:
    text = clean_paragraph(text)
    text = re.sub(r"\s+([,.;:!?])", r"\1", text)
    text = re.sub(r"([,;:]){2,}", r"\1", text)
    text = text.strip(" ,;:-")
    if max_length is not None and len(text) > max_length:
        text = text[:max_length].rsplit(" ", 1)[0].strip(" ,;:-") or text[:max_length].strip(" ,;:-")
    return text if is_readable_text(text) else ""


def clean_short_bibliographic_value(text: str, max_length: int = 60) -> str:
    text = clean_paragraph(text)
    text = re.sub(r"\s+([,.;:!?])", r"\1", text).strip(" ,;:-")
    if max_length and len(text) > max_length:
        text = text[:max_length].strip(" ,;:-")
    if re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9._/\- ]*", text or ""):
        return text
    return text if is_readable_text(text) else ""


def clean_extracted_list(items: Sequence[str], max_items: int = 8) -> List[str]:
    cleaned: List[str] = []
    seen: Set[str] = set()
    for item in items:
        current = clean_extracted_field(item, 180)
        if not current:
            continue
        if len(current) < 3 and not re.search(r"[\u4e00-\u9fff]", current):
            continue
        key = current.lower()
        if key in seen:
            continue
        seen.add(key)
        cleaned.append(current)
        if len(cleaned) >= max_items:
            break
    return cleaned


def is_margin_noise_line(line: str) -> bool:
    text = normalize_line(line)
    if not text:
        return True
    if re.fullmatch(r"[\dA-Za-z.\]\[]", text):
        return True
    if re.fullmatch(r"\d+\s+Figure\s+\d+:.+", text, re.IGNORECASE):
        return False
    return False


def normalize_line(line: str) -> str:
    line = normalize_text(line)
    line = re.sub(r"\s+", " ", line)
    return restore_word_spaces(line.strip(" ,;:-"))


def split_lines(text: str) -> List[str]:
    lines: List[str] = []
    for raw in (text or "").split("\n"):
        line = normalize_line(raw)
        if line:
            lines.append(line)
    return lines


def looks_like_two_column_page(page) -> bool:
    try:
        chars = page.chars or []
        width = float(page.width)
        height = float(page.height)
    except Exception:
        return False
    if not chars or width <= 0 or height <= 0:
        return False

    try:
        words = page.extract_words(x_tolerance=2, y_tolerance=3) or []
    except Exception:
        words = []
    if words:
        rows: List[List[dict]] = []
        current_row: List[dict] = []
        current_top: Optional[float] = None
        body_words = [word for word in words if float(word.get("top", 0) or 0) > height * 0.18]
        for word in sorted(body_words,
                           key=lambda item: (round(float(item.get("top", 0) or 0), 1), float(item.get("x0", 0) or 0))):
            top = float(word.get("top", 0) or 0)
            if current_top is None or abs(top - current_top) <= 4:
                current_row.append(word)
                current_top = top if current_top is None else min(current_top, top)
            else:
                rows.append(current_row)
                current_row = [word]
                current_top = top
        if current_row:
            rows.append(current_row)

        split_rows = 0
        useful_rows = 0
        for row in rows:
            if len(row) < 4:
                continue
            row = sorted(row, key=lambda item: float(item.get("x0", 0) or 0))
            left_words = [word for word in row if float(word.get("x1", 0) or 0) < width * 0.48]
            right_words = [word for word in row if float(word.get("x0", 0) or 0) > width * 0.52]
            if not left_words or not right_words:
                continue
            useful_rows += 1
            left_end = max(float(word.get("x1", 0) or 0) for word in left_words)
            right_start = min(float(word.get("x0", 0) or 0) for word in right_words)
            if right_start - left_end > width * 0.08:
                split_rows += 1
        if useful_rows:
            return split_rows >= 8 and split_rows / float(useful_rows) >= 0.45

    body_chars = [char for char in chars if float(char.get("top", 0) or 0) > height * 0.18]
    if len(body_chars) < 200:
        return False
    left = sum(1 for char in body_chars if float(char.get("x0", 0) or 0) < width * 0.46)
    middle = sum(1 for char in body_chars if width * 0.46 <= float(char.get("x0", 0) or 0) <= width * 0.54)
    right = sum(1 for char in body_chars if float(char.get("x0", 0) or 0) > width * 0.54)
    return left > 80 and right > 80 and middle < min(left, right) * 0.25


def has_standalone_abstract_heading(lines: Sequence[str]) -> bool:
    for index, line in enumerate(lines[:70]):
        lower = line.lower().strip(" .:-")
        if lower in {"abstract", "摘要"}:
            return True
        if lower.startswith("abstract") and len(line) <= 16:
            return True
        if "abstract" in lower and index > 5:
            return False
    return False


def word_set(text: str) -> Set[str]:
    return {word.lower() for word in re.findall(r"[A-Za-z]{3,}", text or "")}


def title_overlap_ratio(line: str, title_words: Set[str]) -> float:
    line_words = word_set(line)
    if not line_words or not title_words:
        return 0.0
    return len(line_words & title_words) / float(len(line_words))


def is_reliable_author_candidate(name: str, title_words: Optional[Set[str]] = None) -> bool:
    candidate = normalize_line(name)
    if not looks_like_author_name(candidate):
        return False
    words = word_set(candidate)
    if words & AUTHOR_NAME_BAD_WORDS:
        return False
    if title_words and words and len(words & title_words) / float(len(words)) >= 0.75:
        return False
    return True


def filter_reliable_author_data(
        authors: Sequence[str],
        author_details: Sequence[Dict],
        title: str,
) -> Tuple[List[str], List[Dict]]:
    title_words = word_set(title)
    reliable_authors: List[str] = []
    seen: Set[str] = set()
    for author in authors:
        cleaned = clean_extracted_field(author, 180)
        if not is_reliable_author_candidate(cleaned, title_words):
            continue
        key = cleaned.lower()
        if key not in seen:
            seen.add(key)
            reliable_authors.append(cleaned)

    reliable_keys = {author.lower() for author in reliable_authors}
    reliable_details: List[Dict] = []
    for detail in author_details or []:
        name = clean_extracted_field(str(detail.get("name", "")), 180)
        if name.lower() not in reliable_keys:
            continue
        copied = dict(detail)
        copied["name"] = name
        reliable_details.append(copied)

    if len(reliable_authors) < 2:
        return [], []
    return reliable_authors, reliable_details


def looks_like_heading(line: str) -> bool:
    lower = (line or "").lower()
    return any(hint in lower for hint in STOP_SECTION_HINTS)


def contains_affiliation_hint(line: str) -> bool:
    lower = (line or "").lower()
    for hint in AFFILIATION_HINTS:
        if re.search(r"[\u4e00-\u9fff]", hint):
            if hint in line:
                return True
        elif re.search(r"\b" + re.escape(hint) + r"\b", lower):
            return True
    return False


def looks_like_front_matter_noise(line: str) -> bool:
    lower = (line or "").lower()
    return any(hint in lower for hint in FRONT_MATTER_NOISE_HINTS)


def looks_like_journal_header(line: str) -> bool:
    lower = (line or "").lower()
    compact = lower.replace(" ", "")
    if any(hint in compact for hint in JOURNAL_HINTS):
        return True
    return bool(re.search(r"\bvol\.?\s*\d+|\bno\.?\s*\d+|\bpp\.?\s*\d+", lower))


def looks_like_email_or_url(line: str) -> bool:
    lower = (line or "").lower()
    return "@" in lower or lower.startswith("http") or "www." in lower


def looks_like_year_or_id(line: str) -> bool:
    return bool(re.fullmatch(r"[\W_]*\d{4,}[\W_]*", line or ""))


def looks_like_author_name(token: str) -> bool:
    token = normalize_line(re.sub(r"\d+$", "", token or ""))
    if not token or len(token) < 2:
        return False
    if any(char.isdigit() for char in token):
        return False
    lower = token.lower()
    if contains_affiliation_hint(token) or any(
            hint in lower for hint in JOURNAL_HINTS + CONFERENCE_HINTS + FRONT_MATTER_NOISE_HINTS):
        return False
    if re.fullmatch(r"[\u4e00-\u9fff]{2,8}", token):
        return True
    return bool(re.fullmatch(r"(?:[A-Z][A-Za-z'`-]+|[A-Z]\.)(?:\s+(?:[A-Z][A-Za-z'`-]+|[A-Z]\.)){1,4}", token))


def split_many_titlecase_authors(line: str) -> List[str]:
    lower = (line or "").lower()
    if contains_affiliation_hint(line) or any(
            hint in lower for hint in JOURNAL_HINTS + CONFERENCE_HINTS + FRONT_MATTER_NOISE_HINTS):
        return []
    working = re.sub(r"\b(and)\b", " ", line, flags=re.IGNORECASE)
    working = re.sub(r"[,;]", " ", working)
    tokens = re.findall(r"[A-Z][a-z]+(?:-[A-Z][a-z]+)?|[A-Z]\.", working)
    if len(tokens) < 4:
        return []

    names: List[str] = []
    index = 0
    while index + 1 < len(tokens):
        candidate = tokens[index] + " " + tokens[index + 1]
        if looks_like_author_name(candidate):
            names.append(candidate)
            index += 2
        else:
            index += 1

    unique: List[str] = []
    seen: Set[str] = set()
    for name in names:
        key = name.lower()
        if key not in seen:
            seen.add(key)
            unique.append(name)
    return unique[:10]


def split_candidate_authors(line: str) -> List[str]:
    working = normalize_line(line)
    working = re.sub(r"[*†‡§¶#0-9]+", " ", working)
    working = re.sub(r"\s+(and|&)\s+", ", ", working, flags=re.IGNORECASE)
    if looks_like_front_matter_noise(working) or looks_like_journal_header(working):
        return []

    if "," not in working and ";" not in working:
        many = split_many_titlecase_authors(working)
        if len(many) >= 2:
            return many

    parts = [normalize_line(part) for part in re.split(r",|;", working) if normalize_line(part)]
    authors: List[str] = []
    for part in parts:
        title_case_tokens = re.findall(r"[A-Z][A-Za-z'`-]+|[A-Z]\.", part)
        if "," not in working and len(title_case_tokens) > 3:
            continue
        if looks_like_author_name(part):
            authors.append(part)
    if authors:
        title_case_tokens = re.findall(r"[A-Z][A-Za-z'`-]+|[A-Z]\.", working)
        if len(authors) < 2 and len(title_case_tokens) >= 4:
            many = split_many_titlecase_authors(working)
            if many:
                return clean_extracted_list(many + authors, MAX_AUTHOR_COUNT)
        return authors

    many = split_many_titlecase_authors(working)
    if many:
        return many

    tokens = re.findall(r"[A-Z][A-Za-z'`-]+|[A-Z]\.", working)
    fallback: List[str] = []
    index = 0
    while index < len(tokens):
        if index + 1 < len(tokens):
            candidate = tokens[index] + " " + tokens[index + 1]
            if looks_like_author_name(candidate):
                fallback.append(candidate)
                index += 2
                continue
        index += 1
    return clean_extracted_list(fallback, MAX_AUTHOR_COUNT)


def looks_like_author_line(line: str) -> bool:
    parsed = split_candidate_authors(line)
    if not parsed:
        return False
    lower = (line or "").lower()
    if len(parsed) >= 2 and len(line) < 160 and not contains_affiliation_hint(line):
        return True
    if "," in line or " and " in lower or "&" in line:
        return True
    if re.search(r"[A-Za-z][*†‡§¶#]?\d", line):
        return True
    return False


def extract_title_from_lines(lines: Sequence[str]) -> Tuple[str, int]:
    title_parts: List[str] = []
    start_index = -1
    for index, line in enumerate(lines[:18]):
        lower = line.lower()
        if looks_like_front_matter_noise(line) or looks_like_journal_header(line):
            continue
        if looks_like_heading(line) or looks_like_email_or_url(line) or lower.startswith("figure"):
            if title_parts:
                break
            continue
        if title_parts and looks_like_author_line(line):
            break
        if title_parts and contains_affiliation_hint(line):
            break
        if len(line) < 8:
            continue
        if start_index < 0:
            start_index = index
        title_parts.append(line)
        if len(title_parts) >= 4:
            break

    title = clean_extracted_field(" ".join(title_parts), 260)
    if title and len(title) >= 12:
        return title, max(0, start_index + len(title_parts))
    return "", 0


def extract_title(first_page, lines: Sequence[str]) -> Tuple[str, int]:
    title, next_index = extract_title_from_lines(lines)
    if title:
        return title, next_index

    try:
        words = first_page.extract_words(extra_attrs=["size"])
    except Exception:
        words = []
    if words:
        max_font_size = max(float(word.get("size", 0) or 0) for word in words)
        large_words = [word for word in words if float(word.get("size", 0) or 0) >= max_font_size - 0.2]
        large_words.sort(key=lambda item: (round(float(item.get("top", 0) or 0), 1), float(item.get("x0", 0) or 0)))
        candidate = normalize_line(" ".join(word.get("text", "").strip() for word in large_words if word.get("text")))
        if len(candidate) >= 12 and not looks_like_journal_header(candidate):
            return candidate, 0

    for index, line in enumerate(lines[:8]):
        if looks_like_heading(line) or looks_like_email_or_url(line) or looks_like_front_matter_noise(
                line) or looks_like_journal_header(line):
            continue
        if len(line) >= 12:
            return line, index + 1
    return "Unknown Title", 0


def extract_authors(lines: Sequence[str], title: str, start_index: int) -> List[str]:
    title_words = word_set(title)
    candidates: List[str] = []
    for line in lines[max(0, start_index):18]:
        lower = line.lower()
        if "abstract" in lower or "摘要" in lower:
            break
        if looks_like_front_matter_noise(line) or looks_like_journal_header(line):
            continue
        if title_overlap_ratio(line, title_words) > 0.65 or looks_like_email_or_url(line) or looks_like_year_or_id(
                line):
            continue
        if contains_affiliation_hint(line):
            if candidates:
                break
            continue
        parsed = split_candidate_authors(line)
        if parsed and looks_like_author_line(line):
            candidates.extend(parsed)
            continue
        if candidates:
            break
    return clean_extracted_list(candidates, MAX_AUTHOR_COUNT)


def extract_institutions(lines: Sequence[str], authors: Sequence[str], title: str) -> List[str]:
    title_words = word_set(title)
    author_keys = {author.lower() for author in authors}
    institutions: List[str] = []
    for line in lines[:22]:
        lower = line.lower()
        if "abstract" in lower or "摘要" in lower:
            break
        if looks_like_front_matter_noise(line) or looks_like_email_or_url(line) or looks_like_journal_header(line):
            continue
        if title_overlap_ratio(line, title_words) > 0.5 or lower in author_keys:
            continue
        if not contains_affiliation_hint(line):
            continue
        for cleaned in split_affiliation_line(line):
            if cleaned:
                institutions.append(cleaned)
    cleaned = clean_extracted_list(institutions, 5)
    if cleaned:
        return cleaned
    return extract_institutions_from_email(lines)


def contains_chinese(text: str) -> bool:
    return bool(re.search(r"[\u4e00-\u9fff]", text or ""))


def split_author_name_fields(name: str) -> Tuple[str, str]:
    cleaned = clean_extracted_field(name, 180)
    if contains_chinese(cleaned):
        return cleaned, ""
    return "", cleaned


def normalize_marker_text(text: str) -> str:
    replacements = {
        "\u00b9": "1", "\u00b2": "2", "\u00b3": "3",
        "\u2070": "0", "\u2074": "4", "\u2075": "5", "\u2076": "6",
        "\u2077": "7", "\u2078": "8", "\u2079": "9",
        "\u2080": "0", "\u2081": "1", "\u2082": "2", "\u2083": "3",
        "\u2084": "4", "\u2085": "5", "\u2086": "6", "\u2087": "7",
        "\u2088": "8", "\u2089": "9",
        "\u2020": "*", "\u2021": "*", "\u00a7": "*", "\u00b6": "*",
        "\u2709": "*", "\u2217": "*", "#": "*",
    }
    for source, target in replacements.items():
        text = (text or "").replace(source, target)
    return text


def split_author_markers(marker_text: str) -> List[str]:
    text = normalize_marker_text(marker_text or "")
    markers = re.findall(r"\d+|\*", text)
    result: List[str] = []
    for marker in markers:
        if marker not in result:
            result.append(marker)
    return result


def collect_author_source_lines(lines: Sequence[str], title: str, start_index: int) -> List[str]:
    title_words = word_set(title)
    source_lines: List[str] = []
    for line in lines[max(0, start_index):18]:
        lower = line.lower()
        if "abstract" in lower or "摘要" in lower:
            break
        if looks_like_front_matter_noise(line) or looks_like_journal_header(line):
            continue
        if title_overlap_ratio(line, title_words) > 0.65 or looks_like_email_or_url(line) or looks_like_year_or_id(
                line):
            continue
        if contains_affiliation_hint(line):
            if source_lines:
                break
            continue
        parsed = split_candidate_authors(line)
        if parsed and looks_like_author_line(line):
            source_lines.append(line)
            continue
        if source_lines:
            break
    return source_lines


def extract_author_markers(author: str, source_lines: Sequence[str]) -> List[str]:
    markers: List[str] = []
    if not author:
        return markers

    author_pattern = re.escape(normalize_marker_text(author))
    for line in source_lines:
        working = normalize_marker_text(line)
        match = re.search(author_pattern + r"\s*([0-9*,\s\[\]\(\).-]{0,16})", working, re.IGNORECASE)
        if not match:
            continue
        for marker in split_author_markers(match.group(1)):
            if marker not in markers:
                markers.append(marker)
    return markers


def extract_affiliation_marker_map(lines: Sequence[str]) -> Dict[str, str]:
    marker_map: Dict[str, str] = {}
    for line in lines[:24]:
        lower = line.lower()
        if "abstract" in lower or "摘要" in lower:
            break
        if looks_like_front_matter_noise(line) or looks_like_email_or_url(line) or looks_like_journal_header(line):
            continue
        if not contains_affiliation_hint(line):
            continue

        working = normalize_marker_text(normalize_line(line))
        working = re.sub(r"(?<!\d)(\d+)\s*(?=[A-Z\u4e00-\u9fff])", r"; \1 ", working)
        working = re.sub(r"(?<!\*)\*\s*(?=[A-Z\u4e00-\u9fff])", r"; * ", working)
        for part in [normalize_line(item) for item in working.split(";") if normalize_line(item)]:
            match = re.match(r"^(\d+|\*)\s*(.+)$", part)
            if not match:
                continue
            marker = match.group(1)
            affiliation = clean_extracted_field(match.group(2), 260)
            if affiliation and contains_affiliation_hint(affiliation) and marker not in marker_map:
                marker_map[marker] = affiliation
    return marker_map


def dedupe_list(items: Sequence[str]) -> List[str]:
    result: List[str] = []
    seen: Set[str] = set()
    for item in items:
        cleaned = clean_extracted_field(item, 260)
        if not cleaned:
            continue
        key = cleaned.lower()
        if key in seen:
            continue
        seen.add(key)
        result.append(cleaned)
    return result


def build_author_details(authors: Sequence[str], institutions: Sequence[str], lines: Sequence[str], title: str,
                         start_index: int) -> List[Dict]:
    cleaned_authors = clean_extracted_list(authors, MAX_AUTHOR_COUNT)
    cleaned_institutions = clean_extracted_list(institutions, 8)
    source_lines = collect_author_source_lines(lines, title, start_index)
    affiliation_marker_map = extract_affiliation_marker_map(lines)
    details: List[Dict] = []

    for author in cleaned_authors:
        markers = extract_author_markers(author, source_lines)
        affiliations = dedupe_list(
            affiliation_marker_map[marker] for marker in markers if marker in affiliation_marker_map)
        mapping_status = "matched" if affiliations else "unmatched"

        if not affiliations and len(cleaned_institutions) == 1:
            affiliations = cleaned_institutions[:]
            mapping_status = "single_institution"

        name_cn, name_en = split_author_name_fields(author)
        details.append({
            "name": author,
            "name_cn": name_cn,
            "name_en": name_en,
            "affiliations": affiliations,
            "affiliation_text": "; ".join(affiliations),
            "markers": markers,
            "mapping_status": mapping_status,
        })

    return details


def extract_institutions_from_email(lines: Sequence[str]) -> List[str]:
    ignored_domains = {
        "gmail", "googlemail", "hotmail", "outlook", "qq", "163", "126", "icloud",
        "yahoo", "foxmail", "protonmail",
    }
    candidates: List[str] = []
    for line in lines[:24]:
        for match in re.findall(r"[\w.+-]+@([A-Za-z0-9.-]+\.[A-Za-z]{2,})", line or ""):
            parts = [part for part in match.lower().split(".") if part]
            if not parts:
                continue
            root = parts[-2] if len(parts) >= 2 else parts[0]
            if root in ignored_domains:
                continue
            if any(char.isdigit() for char in root):
                label = root[:1].upper() + root[1:]
            else:
                label = root.upper() if len(root) <= 4 else root.replace("-", " ").title()
            candidates.append(label)
    return clean_extracted_list(candidates, 3)


def email_domain_label(domain: str) -> str:
    ignored_domains = {
        "gmail", "googlemail", "hotmail", "outlook", "qq", "163", "126", "icloud",
        "yahoo", "foxmail", "protonmail",
    }
    parts = [part for part in (domain or "").lower().split(".") if part]
    if not parts:
        return ""
    root = parts[-2] if len(parts) >= 2 else parts[0]
    if root in ignored_domains:
        return ""
    return root.upper() if len(root) <= 4 else root.replace("-", " ").title()


def extract_email_author_details(lines: Sequence[str], authors: Sequence[str]) -> Tuple[List[str], List[Dict]]:
    cleaned_authors = clean_extracted_list(authors, MAX_AUTHOR_COUNT)
    if not cleaned_authors:
        return [], []

    labels: List[str] = []
    for line in lines[:24]:
        for domain in re.findall(r"[\w.+-]+@([A-Za-z0-9.-]+\.[A-Za-z]{2,})", line or ""):
            label = email_domain_label(domain)
            if label and label not in labels:
                labels.append(label)

    if len(labels) != len(cleaned_authors):
        return [], []

    details: List[Dict] = []
    for author, affiliation in zip(cleaned_authors, labels):
        name_cn, name_en = split_author_name_fields(author)
        details.append({
            "name": author,
            "name_cn": name_cn,
            "name_en": name_en,
            "affiliations": [affiliation],
            "affiliation_text": affiliation,
            "markers": [],
            "mapping_status": "email_matched",
        })
    return labels, details


def split_affiliation_line(line: str) -> List[str]:
    working = normalize_line(line)
    working = re.sub(r"[*†‡§¶#]+", " ", working)
    working = re.sub(r"(?<!\d)(\d+)\s*(?=[A-Z\u4e00-\u9fff])", r"; \1 ", working)
    parts = [normalize_line(part) for part in working.split(";") if normalize_line(part)]
    results: List[str] = []
    for part in parts:
        part = re.sub(r"^\d+\s*", "", part).strip(" ,;")
        if not part or not contains_affiliation_hint(part):
            continue
        results.append(part)
    return results


def word_center(word: Dict) -> float:
    return (float(word.get("x0", 0) or 0) + float(word.get("x1", 0) or 0)) / 2.0


def word_top(word: Dict) -> float:
    return float(word.get("top", 0) or 0)


def word_size(word: Dict) -> float:
    return float(word.get("size", 0) or 0)


def clean_layout_value(text: str, max_length: int = 260) -> str:
    return clean_extracted_field(text, max_length)


def repair_affiliation_spacing(text: str) -> str:
    text = clean_layout_value(text, 260)
    if not text:
        return ""
    text = re.sub(r"\b([A-Z][A-Za-z]+)of\b", r"\1 of", text)
    text = re.sub(r"\b([A-Z][A-Za-z]+)and\b", r"\1 and", text)
    text = re.sub(r"\b([A-Z][A-Za-z]+)at\b", r"\1 at", text)
    text = re.sub(r"\b([A-Z][A-Za-z]+)for\b", r"\1 for", text)
    text = re.sub(r"([A-Za-z])(\d{5,})\b", r"\1 \2", text)
    text = re.sub(r"\.\s+([A-Z]{2,})\b", r".\1", text)
    text = text.replace("Zeal and", "Zealand").replace("Engl and", "England").replace("Netherl ands", "Netherlands")
    text = re.sub(r"\s+", " ", text)
    return text.strip(" ,;:-")


def group_layout_rows(words: Sequence[Dict], tolerance: float = 3.0) -> List[List[Dict]]:
    rows: List[List[Dict]] = []
    current: List[Dict] = []
    current_top: Optional[float] = None
    for word in sorted(words, key=lambda item: (round(word_top(item), 1), float(item.get("x0", 0) or 0))):
        top = word_top(word)
        if current_top is None or abs(top - current_top) <= tolerance:
            current.append(word)
            current_top = top if current_top is None else min(current_top, top)
        else:
            rows.append(sorted(current, key=lambda item: float(item.get("x0", 0) or 0)))
            current = [word]
            current_top = top
    if current:
        rows.append(sorted(current, key=lambda item: float(item.get("x0", 0) or 0)))
    return rows


def find_abstract_top(words: Sequence[Dict]) -> float:
    tops: List[float] = []
    for word in words:
        text = clean_layout_value(str(word.get("text", "")), 80).lower()
        if text in {"abstract", "摘要"}:
            tops.append(word_top(word))
    return min(tops) if tops else 340.0


def is_layout_symbol(text: str) -> bool:
    return not re.search(r"[A-Za-z\u4e00-\u9fff]", text or "")


def is_location_line(text: str) -> bool:
    value = repair_affiliation_spacing(text)
    if not value or contains_affiliation_hint(value):
        return False
    lower = value.lower()
    if looks_like_email_or_url(value):
        return True
    countries = ("usa", "u.s.a", "china", "illinois", "california", "korea", "japan", "uk", "united states")
    if "," in value and any(country in lower for country in countries):
        return True
    if lower in {
        "stanford", "wuhan", "new haven", "changsha", "mountain view", "chicago",
        "illinois", "usa", "china", "california"
    }:
        return True
    return False


def affiliation_needs_continuation(text: str) -> bool:
    lower = (text or "").strip().lower()
    return lower.endswith((" and", " of", " at", " for", " in", "&"))


def looks_like_org_line(text: str) -> bool:
    value = repair_affiliation_spacing(text)
    if not value or looks_like_email_or_url(value) or is_location_line(value):
        return False
    lower = value.lower()
    if contains_affiliation_hint(value):
        return True
    if re.search(r"\b[A-Z][A-Za-z0-9.-]*\.AI\b", value):
        return True
    if lower.endswith(".ai") or " lab" in lower or "labs" in lower:
        return True
    return False


def build_affiliations_from_column_lines(lines: Sequence[str]) -> List[str]:
    affiliations: List[str] = []
    pending = ""
    for raw in lines:
        line = repair_affiliation_spacing(raw)
        if not line or looks_like_email_or_url(line):
            continue
        if is_location_line(line) and not affiliation_needs_continuation(pending):
            continue

        if pending:
            pending = repair_affiliation_spacing(pending + " " + line)
            if affiliation_needs_continuation(pending):
                continue
            if pending and pending not in affiliations:
                affiliations.append(pending)
            pending = ""
            continue

        if looks_like_org_line(line):
            pending = line
            if affiliation_needs_continuation(pending):
                continue
            if pending not in affiliations:
                affiliations.append(pending)
            pending = ""

    if pending and pending not in affiliations:
        affiliations.append(pending)
    return affiliations


def clean_affiliation_list(items: Sequence[str], max_items: int = 12) -> List[str]:
    cleaned: List[str] = []
    seen: Set[str] = set()
    for item in items:
        current = repair_affiliation_spacing(item)
        if not current:
            continue
        key = current.lower()
        if key in seen:
            continue
        seen.add(key)
        cleaned.append(current)
        if len(cleaned) >= max_items:
            break
    return cleaned


def marker_token_parts(text: str) -> List[str]:
    cleaned = normalize_marker_text(normalize_text(text or ""))
    cleaned = re.sub(r"\(cid:\d+\)", " ", cleaned, flags=re.IGNORECASE)
    return split_author_markers(cleaned)


def is_small_marker_word(word: Dict) -> bool:
    text = normalize_marker_text(normalize_text(str(word.get("text", ""))))
    compact = re.sub(r"\s+", "", text)
    if not compact:
        return False
    return len(compact) <= 2 and re.fullmatch(r"[\d*]+", compact) is not None
