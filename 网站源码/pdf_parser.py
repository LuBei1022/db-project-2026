import re
import unicodedata
from typing import Dict, List, Optional, Sequence, Set, Tuple

import pdfplumber

try:
    import wordninja
except Exception:
    wordninja = None


AFFILIATION_HINTS = (
    "university", "institute", "institution", "college", "school", "department",
    "faculty", "laboratory", "lab", "center", "centre", "academy", "hospital",
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


def normalize_text(text: str) -> str:
    text = (text or "").replace("\x00", " ").replace("\ufffd", " ")
    text = text.replace("\ufb00", "ff").replace("\ufb01", "fi").replace("\ufb02", "fl")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    text = re.sub(r"\?{2,}", " ", text)
    text = "".join(" " if is_bad_pdf_char(char) else char for char in text)
    text = text.replace("\r", "\n").replace("\u00a0", " ")
    text = re.sub(r"[ \t]+", " ", text)
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def is_bad_pdf_char(char: str) -> bool:
    if char in "\n\t":
        return False
    category = unicodedata.category(char)
    return category in {"Cc", "Cf", "Cs", "Co", "Cn"} or char in {"■", "□", "◊", "◆", "◇", "❖"}


def readable_char_count(text: str) -> int:
    return sum(1 for char in text if char.isalnum() or "\u4e00" <= char <= "\u9fff")


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
    for char in sorted(chars, key=lambda item: (round(float(item.get("top", 0) or 0), 1), float(item.get("x0", 0) or 0))):
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
        for word in sorted(body_words, key=lambda item: (round(float(item.get("top", 0) or 0), 1), float(item.get("x0", 0) or 0))):
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


def extract_page_text_reading_order(page) -> str:
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
    if contains_affiliation_hint(token) or any(hint in lower for hint in JOURNAL_HINTS + CONFERENCE_HINTS + FRONT_MATTER_NOISE_HINTS):
        return False
    if re.fullmatch(r"[\u4e00-\u9fff]{2,8}", token):
        return True
    return bool(re.fullmatch(r"(?:[A-Z][A-Za-z'`-]+|[A-Z]\.)(?:\s+(?:[A-Z][A-Za-z'`-]+|[A-Z]\.)){1,4}", token))


def split_many_titlecase_authors(line: str) -> List[str]:
    lower = (line or "").lower()
    if contains_affiliation_hint(line) or any(hint in lower for hint in JOURNAL_HINTS + CONFERENCE_HINTS + FRONT_MATTER_NOISE_HINTS):
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
                return clean_extracted_list(many + authors, 10)
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
    return clean_extracted_list(fallback, 10)


def looks_like_author_line(line: str) -> bool:
    parsed = split_candidate_authors(line)
    if not parsed:
        return False
    lower = (line or "").lower()
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
        if looks_like_heading(line) or looks_like_email_or_url(line) or looks_like_front_matter_noise(line) or looks_like_journal_header(line):
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
        if title_overlap_ratio(line, title_words) > 0.65 or looks_like_email_or_url(line) or looks_like_year_or_id(line):
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
    return clean_extracted_list(candidates, 10)


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


def extract_keywords(text: str) -> List[str]:
    patterns = [
        r"(?:keywords?|index terms)\s*[:：\-]?\s*(.+)",
        r"(?:关键词|關鍵詞)\s*[:：\-]?\s*(.+)",
    ]
    for pattern in patterns:
        match = re.search(pattern, text, re.IGNORECASE)
        if not match:
            continue
        keyword_line = clean_paragraph(match.group(1))
        keyword_line = re.split(r"\s{2,}|\babstract\b|摘要|\bintroduction\b", keyword_line, maxsplit=1, flags=re.IGNORECASE)[0]
        keywords = [normalize_line(item) for item in re.split(r"[,;，；]", keyword_line) if normalize_line(item)]
        return clean_extracted_list(keywords, 10)
    return []


def extract_doi(text: str, metadata: Dict) -> str:
    candidates = [str(value) for value in (metadata or {}).values() if value]
    candidates.append(text or "")
    combined = "\n".join(candidates)
    compact = normalize_text(combined)
    compact = re.sub(r"\s*([./:_;()/-])\s*", r"\1", compact)
    compact = re.sub(r"\bdoi\s*[:：]\s*", "", compact, flags=re.IGNORECASE)
    match = re.search(r"\b10\.\d{4,9}/[-._;()/:A-Z0-9]+\b", compact, re.IGNORECASE)
    if match:
        return match.group(0).strip().rstrip(".,;)]:").lower()
    arxiv_match = re.search(r"\barxiv\s*[:：]?\s*(\d{4}\.\d{4,5})(?:v\d+)?\b", combined, re.IGNORECASE)
    if arxiv_match:
        return "10.48550/arxiv." + arxiv_match.group(1)
    return ""


def extract_vertical_arxiv_doi(lines: Sequence[str]) -> str:
    chars: List[str] = []
    for line in lines[:90]:
        value = normalize_line(line)
        if re.fullmatch(r"[A-Za-z0-9.\[\]:-]", value or ""):
            chars.append(value)
            continue
        leading = re.match(r"^([A-Za-z0-9.\[\]:-])\s+.+", value or "")
        if leading:
            chars.append(leading.group(1))
    if len(chars) < 10:
        return ""

    candidates = ["".join(chars), "".join(reversed(chars))]
    for candidate in candidates:
        compact = candidate.replace("：", ":")
        match = re.search(r"arxiv:?(\d{4}\.\d{4,5})(?:v\d+)?", compact, re.IGNORECASE)
        if match:
            return "10.48550/arxiv." + match.group(1)
    return ""


def extract_publish_year(text: str, lines: Sequence[str], metadata: Dict, doi: str = "") -> str:
    arxiv_match = re.search(r"arxiv\.(\d{2})(\d{2})\.\d+", doi or "", re.IGNORECASE)
    if arxiv_match:
        year = int(arxiv_match.group(1))
        return str(2000 + year if year < 90 else 1900 + year)

    source = "\n".join([str(value) for value in (metadata or {}).values() if value] + list(lines[:24]) + [text[:1200]])
    years = []
    for year in re.findall(r"\b(?:19|20)\d{2}\b", source):
        value = int(year)
        if 1990 <= value <= 2100:
            years.append(year)
    if not years:
        return ""
    counts: Dict[str, int] = {}
    for year in years:
        counts[year] = counts.get(year, 0) + 1
    return sorted(counts.items(), key=lambda item: (-item[1], -int(item[0])))[0][0]


def normalize_venue_name(candidate: str) -> str:
    candidate = normalize_line(candidate)
    candidate = candidate.replace("JOURNALOF", "JOURNAL OF ")
    candidate = re.sub(r"\bpreprint\s*version\b.*$", "", candidate, flags=re.IGNORECASE).strip(" .,-")
    candidate = re.sub(r"\bpreprintversion\b.*$", "", candidate, flags=re.IGNORECASE).strip(" .,-")
    candidate = re.sub(r"\bvol\.?\s*\d+.*$", "", candidate, flags=re.IGNORECASE).strip(" .,-")
    candidate = re.sub(r"\bno\.?\s*\d+.*$", "", candidate, flags=re.IGNORECASE).strip(" .,-")
    candidate = re.sub(r"\b(?:19|20)\d{2}\s*\d*$", "", candidate).strip(" .,-")
    candidate = restore_word_spaces(candidate)
    if candidate.isupper():
        candidate = candidate.title().replace("Ieee", "IEEE").replace("Acm", "ACM")
    return clean_extracted_field(candidate, 220)


def is_reasonable_venue(candidate: str) -> bool:
    candidate = normalize_line(candidate)
    if len(candidate) < 3 or len(candidate) > 180:
        return False
    lower = candidate.lower()
    if looks_like_front_matter_noise(candidate) or lower.startswith("require"):
        return False
    return True


def extract_conference_from_noise(line: str) -> str:
    lower = (line or "").lower()
    for key, label in KNOWN_CONFERENCE_LABELS.items():
        if re.search(r"\b" + re.escape(key) + r"\b", lower):
            return label
        if key == "cvpr" and "cvprpaper" in lower:
            return label
    return ""


def contains_conference_hint(line: str) -> bool:
    lower = (line or "").lower()
    for generic in ("conference", "symposium", "workshop", "proceedings", "meeting"):
        if re.search(r"\b" + re.escape(generic) + r"\b", lower):
            return True
    return bool(extract_conference_from_noise(line))


def extract_venue(lines: Sequence[str], metadata: Dict) -> Tuple[str, str]:
    metadata_text = " ".join(str(value) for value in (metadata or {}).values() if value)
    for candidate in [metadata_text] + list(lines[:24]):
        if not candidate:
            continue
        lower = candidate.lower()
        if looks_like_front_matter_noise(candidate):
            conference = extract_conference_from_noise(candidate)
            if conference:
                return "", conference
            continue
        if looks_like_journal_header(candidate):
            journal = normalize_venue_name(candidate)
            if is_reasonable_venue(journal):
                return journal, ""
        explicit = re.search(r"(?:published in|accepted at|presented at|proceedings of(?: the)?)\s+(.+)", candidate, re.IGNORECASE)
        if explicit:
            conference = normalize_venue_name(explicit.group(1))
            if is_reasonable_venue(conference):
                return "", conference
        conference = extract_conference_from_noise(candidate)
        if conference:
            return "", conference
        if contains_conference_hint(candidate):
            value = normalize_venue_name(candidate)
            if is_reasonable_venue(value):
                return "", value
    return "", ""


def extract_abstract(text: str, lines: Sequence[str], title: str, authors: Sequence[str]) -> str:
    patterns = [
        r"(?:abstract)\s*[:：—\-]?\s*(.*?)(?=\n\s*(?:keywords?|index terms|1\.|i\.|introduction)\b)",
        r"(?:摘要)\s*[:：—\-]?\s*(.*?)(?=\n\s*(?:关键词|關鍵詞|1\.|引言))",
    ]
    for pattern in patterns:
        match = re.search(pattern, text, re.IGNORECASE | re.DOTALL)
        if match:
            abstract = clean_abstract_block(match.group(1))
            if len(abstract) >= 40:
                return abstract

    title_words = word_set(title)
    author_keys = {author.lower() for author in authors}
    parts: List[str] = []
    collecting = False
    for line in lines:
        lower = line.lower()
        if not collecting and ("abstract" in lower or "摘要" in lower):
            collecting = True
            remainder = re.split(r"abstract|摘要", line, maxsplit=1, flags=re.IGNORECASE)[-1]
            remainder = normalize_line(re.sub(r"^[:：—\-\s]+", "", remainder))
            if remainder:
                parts.append(remainder)
            continue
        if collecting:
            if is_margin_noise_line(line):
                continue
            if re.match(r"^\d+\s+Figure\s+\d+:", line, re.IGNORECASE):
                continue
            if line.lower().startswith("figure"):
                continue
            if (looks_like_heading(line) and parts) or lower.startswith("figure"):
                break
            if title_overlap_ratio(line, title_words) > 0.6 or lower in author_keys:
                continue
            cleaned_line = clean_abstract_line(line)
            if cleaned_line:
                parts.append(cleaned_line)
            if len(clean_paragraph(" ".join(parts))) >= 1200:
                break
    return clean_abstract_text(" ".join(parts))


def extract_front_matter_abstract(lines: Sequence[str], title: str, authors: Sequence[str]) -> str:
    title_words = word_set(title)
    author_keys = {author.lower() for author in authors}
    parts: List[str] = []
    collecting = False

    for line in lines[:90]:
        lower = line.lower()
        if "abstract" in lower or "摘要" in lower:
            continue
        if re.match(r"^(?:\d+\.\s*)?(?:i\.?\s*)?introduction\b", lower):
            break
        if lower.startswith("code can be found") or lower.startswith("code is at"):
            break
        if "keywords" in lower or "index terms" in lower:
            break
        if is_margin_noise_line(line) or looks_like_front_matter_noise(line) or looks_like_journal_header(line):
            continue
        if looks_like_email_or_url(line) or contains_affiliation_hint(line):
            continue
        if title_overlap_ratio(line, title_words) > 0.45 or lower in author_keys:
            continue
        parsed_authors = split_candidate_authors(line)
        if not collecting and len(parsed_authors) >= 2 and "," in line and len(line) < 160:
            continue

        cleaned_line = clean_abstract_line(line)
        if not cleaned_line:
            continue
        if not collecting and len(cleaned_line) < 55:
            continue
        collecting = True
        parts.append(cleaned_line)
        if len(clean_paragraph(" ".join(parts))) >= 1200:
            break

    abstract = clean_abstract_text(" ".join(parts))
    return abstract if len(abstract) >= 120 else ""


def clean_abstract_line(line: str) -> str:
    text = normalize_line(line)
    text = re.sub(r"^\d+\s+(?=[A-Z])", "", text)
    text = re.sub(r"^[arxivcsCVROgGuU]\s+(?=[A-Za-z])", "", text)
    text = re.sub(r"^A\s+(?=real-world\b)", "", text)
    return text


def repair_common_pdf_word_splits(text: str) -> str:
    replacements = {
        "fabric ability": "fabricability",
        "sup ports": "supports",
        "ef ficiency": "efficiency",
        "Compre hensive": "Comprehensive",
        "compre hensive": "comprehensive",
        "conver sations": "conversations",
        "conver sation": "conversation",
        "prod uct": "product",
        "prod ucts": "products",
        "multi session": "multi-session",
    }
    for source, target in replacements.items():
        text = re.sub(re.escape(source), target, text, flags=re.IGNORECASE if source.islower() else 0)
    return text


def clean_abstract_block(text: str) -> str:
    lines = split_lines(text)
    if len(lines) <= 1:
        return clean_abstract_text(text)

    parts: List[str] = []
    for line in lines:
        if is_margin_noise_line(line):
            continue
        if re.match(r"^\d+\s+Figure\s+\d+:", line, re.IGNORECASE):
            continue
        cleaned_line = clean_abstract_line(line)
        if cleaned_line:
            parts.append(cleaned_line)
    return clean_abstract_text(" ".join(parts))


def clean_abstract_text(text: str) -> str:
    text = clean_extracted_field(text, 2000)
    if not text:
        return ""
    # Remove isolated margin/page tokens that often leak into two-column abstracts.
    text = re.sub(r"^(?:[\[\]\d]\s+)+", "", text)
    text = re.sub(r"(?<=\s)[\[\]\d](?=\s)", " ", text)
    text = re.sub(r"(?<=\s)[vViIxXrRgGuU](?=\s+(?:[A-Za-z(]))", " ", text)
    text = re.sub(r"\s+", " ", text)
    return repair_common_pdf_word_splits(text).strip(" ,;:-")


def extract_bibliographic_info(lines: Sequence[str], metadata: Dict, page_count: int, doi: str) -> Dict[str, str]:
    source = "\n".join(list(lines[:24]) + [str(value) for value in (metadata or {}).values() if value])
    volume = ""
    issue = ""
    publisher = ""

    volume_match = re.search(r"\bvol(?:ume)?\.?\s+([A-Za-z0-9\-]+)", source, re.IGNORECASE)
    if volume_match:
        volume = volume_match.group(1)

    issue_match = re.search(r"\bno\.?\s+([A-Za-z0-9\-]+)", source, re.IGNORECASE)
    if issue_match:
        issue = issue_match.group(1)

    if looks_like_journal_header(source) and "ieee" in source.lower():
        publisher = "IEEE"
    elif doi and "10.48550/arxiv" in doi.lower():
        publisher = "arXiv"

    pages = "1-" + str(page_count) if page_count and page_count > 1 else ("1" if page_count == 1 else "")

    return {
        "volume": clean_short_bibliographic_value(volume, 60),
        "issue": clean_short_bibliographic_value(issue, 60),
        "pages": pages,
        "publisher": publisher if publisher == "arXiv" else clean_extracted_field(publisher, 120),
        "page_count": str(page_count or ""),
    }


def get_pdf_text(pdf, max_pages: int = 3) -> Tuple[str, List[str]]:
    texts: List[str] = []
    first_page_lines: List[str] = []
    for index, page in enumerate(pdf.pages[:max_pages]):
        if index == 0:
            raw_text = extract_page_text(page)
            first_page_lines = split_lines(raw_text)
            page_text = raw_text if has_standalone_abstract_heading(first_page_lines) else extract_page_text_reading_order(page)
        else:
            page_text = extract_page_text_reading_order(page)
        if page_text:
            texts.append(page_text)
    return "\n".join(texts), first_page_lines


def extract_paper_info(pdf_path: str) -> Optional[Dict]:
    try:
        with pdfplumber.open(pdf_path) as pdf:
            if not pdf.pages:
                return None

            metadata = pdf.metadata or {}
            full_text, first_page_lines = get_pdf_text(pdf, 3)
            if not full_text or not first_page_lines:
                return None

            title, title_end_index = extract_title(pdf.pages[0], first_page_lines)
            authors = extract_authors(first_page_lines, title, title_end_index)
            institutions = extract_institutions(first_page_lines, authors, title)
            keywords = extract_keywords(full_text)
            abstract = extract_abstract(full_text, split_lines(full_text), title, authors)
            if not abstract:
                abstract = extract_front_matter_abstract(first_page_lines, title, authors)
            journal, conference = extract_venue(first_page_lines, metadata)
            doi = extract_doi(full_text, metadata) or extract_vertical_arxiv_doi(first_page_lines)
            publish_year = extract_publish_year(full_text, first_page_lines, metadata, doi)
            source_type = "期刊论文" if journal else ("会议论文" if conference else "其他")
            bibliographic = extract_bibliographic_info(first_page_lines, metadata, len(pdf.pages), doi)

            return {
                "title": clean_extracted_field(title, 260) or "Unknown Title",
                "authors": clean_extracted_list(authors, 10),
                "institutions": clean_extracted_list(institutions, 5),
                "keywords": clean_extracted_list(keywords, 10),
                "abstract": clean_extracted_field(abstract, 2000),
                "journal": clean_extracted_field(journal, 260),
                "conference": clean_extracted_field(conference, 260),
                "doi": doi,
                "publish_year": publish_year,
                "source_type": source_type,
                "volume": bibliographic["volume"],
                "issue": bibliographic["issue"],
                "pages": bibliographic["pages"],
                "publisher": bibliographic["publisher"],
                "page_count": bibliographic["page_count"],
            }
    except Exception as exc:
        print("PDF parse error: {0}".format(exc))
        return None
