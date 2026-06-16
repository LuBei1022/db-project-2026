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
        if title_overlap_ratio(line, title_words) > 0.65 or looks_like_email_or_url(line) or looks_like_year_or_id(line):
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


def build_author_details(authors: Sequence[str], institutions: Sequence[str], lines: Sequence[str], title: str, start_index: int) -> List[Dict]:
    cleaned_authors = clean_extracted_list(authors, MAX_AUTHOR_COUNT)
    cleaned_institutions = clean_extracted_list(institutions, 8)
    source_lines = collect_author_source_lines(lines, title, start_index)
    affiliation_marker_map = extract_affiliation_marker_map(lines)
    details: List[Dict] = []

    for author in cleaned_authors:
        markers = extract_author_markers(author, source_lines)
        affiliations = dedupe_list(affiliation_marker_map[marker] for marker in markers if marker in affiliation_marker_map)
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
    if not re.fullmatch(r"[\d,.*\[\]\(\)-]+", compact):
        return False
    return word_size(word) <= 9.8


def clean_marker_author_name(text: str) -> str:
    working = normalize_marker_text(str(text or ""))
    working = re.sub(r"\(cid:\d+\)", " ", working, flags=re.IGNORECASE)
    working = re.sub(r"[\d,.*\[\]\(\)-]+$", "", working).strip()
    return clean_layout_value(restore_word_spaces(working), 180)


def attached_markers_from_author_word(text: str) -> List[str]:
    working = normalize_marker_text(str(text or ""))
    match = re.search(r"([\d,.*]+)$", working)
    if not match:
        return []
    return split_author_markers(match.group(1))


def is_marker_author_word(word: Dict) -> bool:
    size = word_size(word)
    if size < 10.5 or size > 13.8:
        return False
    name = clean_marker_author_name(str(word.get("text", "")))
    if not name or contains_affiliation_hint(name) or looks_like_email_or_url(name):
        return False
    if looks_like_front_matter_noise(name) or looks_like_journal_header(name):
        return False
    return looks_like_author_name(name)


def row_text_from_words(row: Sequence[Dict]) -> str:
    return repair_affiliation_spacing(" ".join(str(word.get("text", "")) for word in row if word.get("text")))


def parse_marker_affiliation_row(row: Sequence[Dict], marker_map: Dict[str, str]) -> None:
    current_markers: List[str] = []
    current_words: List[str] = []

    def flush() -> None:
        if not current_markers or not current_words:
            return
        affiliation = repair_affiliation_spacing(" ".join(current_words))
        if not affiliation or not contains_affiliation_hint(affiliation):
            return
        for marker in current_markers:
            if marker != "*" and marker not in marker_map:
                marker_map[marker] = affiliation

    for word in sorted(row, key=lambda item: float(item.get("x0", 0) or 0)):
        if is_small_marker_word(word):
            flush()
            current_markers = [marker for marker in marker_token_parts(str(word.get("text", ""))) if marker != "*"]
            current_words = []
            continue
        if current_markers:
            current_words.append(str(word.get("text", "")))

    flush()


def parse_marker_author_row(row: Sequence[Dict]) -> List[Dict]:
    entries: List[Dict] = []
    words = sorted(row, key=lambda item: float(item.get("x0", 0) or 0))
    index = 0
    while index < len(words):
        word = words[index]
        if not is_marker_author_word(word):
            index += 1
            continue

        name = clean_marker_author_name(str(word.get("text", "")))
        markers = attached_markers_from_author_word(str(word.get("text", "")))
        next_index = index + 1
        while next_index < len(words):
            next_word = words[next_index]
            if is_marker_author_word(next_word):
                break
            if is_small_marker_word(next_word):
                for marker in marker_token_parts(str(next_word.get("text", ""))):
                    if marker not in markers:
                        markers.append(marker)
            next_index += 1

        entries.append({"name": name, "markers": markers})
        index = max(index + 1, next_index)

    return entries


def extract_marker_author_details(page) -> Tuple[List[str], List[str], List[Dict]]:
    try:
        words = page.extract_words(extra_attrs=["size", "fontname"])
    except Exception:
        return [], [], []
    if not words:
        return [], [], []

    abstract_top = find_abstract_top(words)
    rows = group_layout_rows([word for word in words if word_top(word) < abstract_top - 8], 3.0)
    marker_map: Dict[str, str] = {}
    author_entries: List[Dict] = []

    for row in rows:
        text = row_text_from_words(row)
        lower = text.lower()
        if not text or looks_like_front_matter_noise(text) or looks_like_journal_header(text) or looks_like_email_or_url(text):
            continue
        if lower.startswith("figure"):
            continue

        has_small_marker = any(is_small_marker_word(word) for word in row)
        if has_small_marker and contains_affiliation_hint(text):
            parse_marker_affiliation_row(row, marker_map)
            continue

        row_entries = parse_marker_author_row(row)
        if row_entries and (has_small_marker or len(row_entries) >= 2):
            author_entries.extend(row_entries)

    if len(author_entries) < 2 or not marker_map:
        return [], [], []

    authors: List[str] = []
    details: List[Dict] = []
    seen_authors: Set[str] = set()
    for entry in author_entries:
        name = clean_layout_value(str(entry.get("name", "")), 180)
        if not name:
            continue
        key = name.lower()
        if key in seen_authors:
            continue
        seen_authors.add(key)
        markers = [marker for marker in entry.get("markers", []) if marker]
        affiliations = dedupe_list(marker_map[marker] for marker in markers if marker in marker_map)
        name_cn, name_en = split_author_name_fields(name)
        authors.append(name)
        details.append({
            "name": name,
            "name_cn": name_cn,
            "name_en": name_en,
            "affiliations": affiliations,
            "affiliation_text": "; ".join(affiliations),
            "markers": markers,
            "mapping_status": "marker_matched" if affiliations else "unmatched",
        })
        if len(authors) >= MAX_AUTHOR_COUNT:
            break

    institutions = clean_affiliation_list(list(marker_map.values()), 12)
    if len(authors) < 2 or not institutions:
        return [], [], []
    return authors, institutions, details


def normalize_author_identity(text: str) -> str:
    return re.sub(r"[^a-z\u4e00-\u9fff]+", "", clean_extracted_field(text, 180).lower())


def split_footnote_author_names(text: str) -> List[str]:
    working = re.sub(r"\s+and\s+", ",", text or "", flags=re.IGNORECASE)
    return clean_extracted_list([part.strip() for part in working.split(",")], MAX_AUTHOR_COUNT)


def raw_row_text_from_words(row: Sequence[Dict]) -> str:
    return normalize_text(" ".join(str(word.get("text", "")) for word in row if word.get("text"))).strip()


def join_footnote_continuation(current: str, line: str) -> str:
    current = (current or "").strip()
    line = (line or "").strip()
    if not current:
        return line
    if current.endswith("-"):
        return current[:-1] + line
    return current + " " + line


def clean_footnote_affiliation(text: str) -> str:
    working = re.sub(r"\bE\s*-\s*mail\s*:.*$", "", text or "", flags=re.IGNORECASE)
    working = re.sub(r"\bE-?mail\s*:.*$", "", working, flags=re.IGNORECASE)
    working = re.sub(r"\bEmail\s*:.*$", "", working, flags=re.IGNORECASE)
    working = re.sub(r"\bE\s*-\s*$", "", working, flags=re.IGNORECASE)
    working = re.sub(r"\s+", " ", working).strip(" ,;:-")
    return repair_affiliation_spacing(working)


def extract_left_column_footnote_lines(page) -> List[str]:
    try:
        words = page.extract_words(extra_attrs=["size", "fontname"])
    except Exception:
        return []
    if not words:
        return []

    page_width = float(getattr(page, "width", 0) or 0)
    page_height = float(getattr(page, "height", 0) or 0)
    if page_width <= 0 or page_height <= 0:
        return []

    left_words = [
        word for word in words
        if float(word.get("x0", 0) or 0) < page_width * 0.50
        and word_top(word) > page_height * 0.42
    ]
    rows = group_layout_rows(left_words, 3.0)
    return [raw_row_text_from_words(row) for row in rows if raw_row_text_from_words(row)]


def extract_footnote_author_details(page, authors: Sequence[str]) -> Tuple[List[str], List[Dict]]:
    cleaned_authors = clean_extracted_list(authors, MAX_AUTHOR_COUNT)
    if not cleaned_authors:
        return [], []

    author_by_key = {normalize_author_identity(author): author for author in cleaned_authors}
    lines = extract_left_column_footnote_lines(page)
    if not lines:
        return [], []

    affiliation_by_author: Dict[str, List[str]] = {}
    index = 0
    while index < len(lines):
        line = lines[index]
        lower = line.lower()
        if " is with " not in lower and " are with " not in lower and not (re.search(r"\bare\b", lower) and index + 1 < len(lines) and lines[index + 1].lower().startswith("with ")):
            index += 1
            continue

        statement = line
        next_index = index + 1
        if " are with " not in lower and " is with " not in lower and next_index < len(lines) and lines[next_index].lower().startswith("with "):
            statement = join_footnote_continuation(statement, lines[next_index])
            next_index += 1

        while next_index < len(lines):
            next_line = lines[next_index]
            next_lower = next_line.lower()
            if re.search(r"\b(is|are)\s+with\b", next_lower):
                break
            if re.search(r"\bemail\s*:|\be\s*-\s*mail\s*:|@", next_lower):
                statement = join_footnote_continuation(statement, next_line)
                next_index += 1
                break
            statement = join_footnote_continuation(statement, next_line)
            if "." in next_line and contains_affiliation_hint(statement):
                break
            next_index += 1

        match = re.match(r"^(.+?)\s+(is|are)\s+with\s+(.+)$", statement, flags=re.IGNORECASE)
        if not match:
            index = max(index + 1, next_index)
            continue

        affiliation = clean_footnote_affiliation(match.group(3))
        if not affiliation or not contains_affiliation_hint(affiliation):
            index = max(index + 1, next_index)
            continue

        for name in split_footnote_author_names(match.group(1)):
            key = normalize_author_identity(name)
            if key not in author_by_key:
                continue
            affiliation_by_author.setdefault(author_by_key[key], [])
            if affiliation not in affiliation_by_author[author_by_key[key]]:
                affiliation_by_author[author_by_key[key]].append(affiliation)

        index = max(index + 1, next_index)

    if not affiliation_by_author:
        return [], []

    details: List[Dict] = []
    all_institutions: List[str] = []
    for author in cleaned_authors:
        affiliations = affiliation_by_author.get(author, [])
        for affiliation in affiliations:
            if affiliation not in all_institutions:
                all_institutions.append(affiliation)
        name_cn, name_en = split_author_name_fields(author)
        details.append({
            "name": author,
            "name_cn": name_cn,
            "name_en": name_en,
            "affiliations": affiliations,
            "affiliation_text": "; ".join(affiliations),
            "markers": [],
            "mapping_status": "footnote_matched" if affiliations else "unmatched",
        })

    return clean_affiliation_list(all_institutions, 12), details


def normalize_inline_author_name(text: str) -> str:
    working = normalize_marker_text(text or "")
    working = re.sub(r"[*†‡§¶#]+", " ", working)
    working = re.sub(r"\s+", " ", working).strip(" ,;:-")
    if not working:
        return ""

    letters = re.sub(r"[^A-Za-z]", "", working)
    if letters and letters.upper() == letters:
        pieces: List[str] = []
        for token in working.split():
            hyphen_parts = []
            for part in token.split("-"):
                hyphen_parts.append(part[:1].upper() + part[1:].lower() if part else part)
            pieces.append("-".join(hyphen_parts))
        working = " ".join(pieces)
    return clean_extracted_field(working, 180)


def clean_inline_affiliation(text: str) -> str:
    working = normalize_marker_text(text or "")
    working = working.replace("BNRist", "BN Rist").replace("Bnrist", "BN Rist")
    working = re.sub(r"\s+", " ", working).strip(" ,;:-")
    return repair_affiliation_spacing(working)


def split_inline_author_affiliation_line(line: str) -> Tuple[str, str]:
    working = normalize_line(line)
    if not working or "," not in working:
        return "", ""

    match = re.match(r"^([A-Z][A-Z'`-]+(?:\s+[A-Z][A-Z'`-]+){1,4})\s*,\s*(.+)$", working)
    if not match:
        return "", ""

    author = normalize_inline_author_name(match.group(1))
    affiliation = clean_inline_affiliation(match.group(2))
    if not author or not looks_like_author_name(author):
        return "", ""
    if not affiliation or not contains_affiliation_hint(affiliation):
        return "", ""
    return author, affiliation


def looks_like_inline_author_only_line(line: str) -> bool:
    author = normalize_inline_author_name(line)
    if not author or not looks_like_author_name(author):
        return False
    working = normalize_marker_text(line or "")
    return bool(re.fullmatch(r"[A-Z][A-Z'`-]+(?:\s+[A-Z][A-Z'`-]+){1,4}\s*[*†‡§¶#]?", working.strip()))


def extract_inline_author_affiliation_details(lines: Sequence[str], title: str, start_index: int) -> Tuple[List[str], List[str], List[Dict]]:
    authors: List[str] = []
    institutions: List[str] = []
    details: List[Dict] = []
    title_words = word_set(title)
    index = max(0, start_index)
    end = min(len(lines), index + 18)

    while index < end:
        line = normalize_line(lines[index])
        lower = line.lower()
        if not line or looks_like_front_matter_noise(line) or looks_like_journal_header(line):
            index += 1
            continue
        if lower.startswith("fig.") or lower.startswith("figure") or "authors’contact" in lower or "authors'contact" in lower:
            break
        if title_overlap_ratio(line, title_words) > 0.65:
            index += 1
            continue

        author, affiliation = split_inline_author_affiliation_line(line)
        if not author and looks_like_inline_author_only_line(line) and index + 1 < len(lines):
            next_line = normalize_line(lines[index + 1])
            next_affiliation = clean_inline_affiliation(next_line)
            if next_affiliation and contains_affiliation_hint(next_affiliation):
                author = normalize_inline_author_name(line)
                affiliation = next_affiliation
                index += 1

        if author and affiliation:
            if author not in authors:
                authors.append(author)
                name_cn, name_en = split_author_name_fields(author)
                details.append({
                    "name": author,
                    "name_cn": name_cn,
                    "name_en": name_en,
                    "affiliations": [affiliation],
                    "affiliation_text": affiliation,
                    "markers": [],
                    "mapping_status": "inline_matched",
                })
            if affiliation not in institutions:
                institutions.append(affiliation)
        elif authors and contains_affiliation_hint(line):
            # Once explicit author-affiliation rows started, an unmatched affiliation line
            # likely belongs to the preceding author-only line and should not become a global fallback.
            pass

        index += 1

    if len(authors) < 2 or not institutions:
        return [], [], []
    return authors, clean_affiliation_list(institutions, 12), details


def extract_layout_author_rows(page) -> List[Dict]:
    try:
        words = page.extract_words(extra_attrs=["size", "fontname"])
    except Exception:
        return []
    if not words:
        return []

    abstract_top = find_abstract_top(words)
    rows = group_layout_rows([word for word in words if word_top(word) < abstract_top - 8])
    author_rows: List[Dict] = []

    for row in rows:
        author_words = []
        for word in row:
            size = word_size(word)
            if size < 11.0 or size > 13.3:
                continue
            raw = str(word.get("text", ""))
            cleaned = clean_layout_value(raw, 180)
            if not cleaned or is_layout_symbol(cleaned) or contains_affiliation_hint(cleaned):
                continue
            if looks_like_author_name(cleaned):
                author_words.append({
                    "name": cleaned,
                    "x": word_center(word),
                    "top": word_top(word)
                })

        if not author_words:
            row_text = " ".join(str(word.get("text", "")) for word in row if 11.0 <= word_size(word) <= 13.3)
            parsed = split_candidate_authors(row_text)
            if len(parsed) >= 2:
                row_min_x = min(float(word.get("x0", 0) or 0) for word in row)
                row_max_x = max(float(word.get("x1", 0) or 0) for word in row)
                step = (row_max_x - row_min_x) / max(1, len(parsed))
                author_words = [
                    {
                        "name": parsed[index],
                        "x": row_min_x + step * (index + 0.5),
                        "top": min(word_top(word) for word in row)
                    }
                    for index in range(len(parsed))
                ]

        if author_words:
            top = min(item["top"] for item in author_words)
            if top < abstract_top - 8:
                author_rows.append({
                    "top": top,
                    "authors": author_words
                })

    cleaned_rows: List[Dict] = []
    seen_tops: Set[int] = set()
    for row in sorted(author_rows, key=lambda item: item["top"]):
        top_key = int(round(row["top"]))
        if top_key in seen_tops:
            continue
        seen_tops.add(top_key)
        cleaned_rows.append(row)
    return cleaned_rows


def collect_column_lines(words: Sequence[Dict], start_top: float, end_top: float, author_x: float, neighbor_distance: float) -> List[str]:
    assigned: List[Dict] = []
    max_distance = max(72.0, min(128.0, neighbor_distance * 0.52 if neighbor_distance > 0 else 96.0))
    for word in words:
        top = word_top(word)
        if top <= start_top + 5 or top >= end_top - 4:
            continue
        if word_size(word) > 11.5:
            continue
        text = str(word.get("text", ""))
        if not text or is_layout_symbol(text):
            continue
        if abs(word_center(word) - author_x) <= max_distance:
            assigned.append(word)

    rows = group_layout_rows(assigned, 3.0)
    lines: List[str] = []
    for row in rows:
        pieces = [repair_affiliation_spacing(str(word.get("text", ""))) for word in row]
        line = repair_affiliation_spacing(" ".join(piece for piece in pieces if piece))
        if line:
            lines.append(line)
    return lines


def extract_layout_author_details(page) -> Tuple[List[str], List[str], List[Dict]]:
    try:
        words = page.extract_words(extra_attrs=["size", "fontname"])
    except Exception:
        return [], [], []
    if not words:
        return [], [], []

    author_rows = extract_layout_author_rows(page)
    if not author_rows:
        return [], [], []

    abstract_top = find_abstract_top(words)
    authors: List[str] = []
    institutions: List[str] = []
    details: List[Dict] = []

    for row_index, row in enumerate(author_rows):
        row_authors = sorted(row["authors"], key=lambda item: item["x"])
        next_top = author_rows[row_index + 1]["top"] if row_index + 1 < len(author_rows) else abstract_top
        xs = [item["x"] for item in row_authors]
        for index, author in enumerate(row_authors):
            left_gap = author["x"] - xs[index - 1] if index > 0 else 0
            right_gap = xs[index + 1] - author["x"] if index + 1 < len(xs) else 0
            neighbor_distance = min([gap for gap in (left_gap, right_gap) if gap > 0] or [0])
            column_lines = collect_column_lines(words, row["top"], next_top, author["x"], neighbor_distance)
            affiliations = build_affiliations_from_column_lines(column_lines)

            name = clean_layout_value(author["name"], 180)
            if not name or name in authors:
                continue
            name_cn, name_en = split_author_name_fields(name)
            authors.append(name)
            for affiliation in affiliations:
                if affiliation not in institutions:
                    institutions.append(affiliation)
            details.append({
                "name": name,
                "name_cn": name_cn,
                "name_en": name_en,
                "affiliations": affiliations,
                "affiliation_text": "; ".join(affiliations),
                "markers": [],
                "mapping_status": "layout_matched" if affiliations else "unmatched",
            })

    if len(authors) < 2:
        return [], [], []
    return authors, institutions, details


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


MONTH_NAME_LOOKUP = {
    "jan": 1, "january": 1,
    "feb": 2, "february": 2,
    "mar": 3, "march": 3,
    "apr": 4, "april": 4,
    "may": 5,
    "jun": 6, "june": 6,
    "jul": 7, "july": 7,
    "aug": 8, "august": 8,
    "sep": 9, "sept": 9, "september": 9,
    "oct": 10, "october": 10,
    "nov": 11, "november": 11,
    "dec": 12, "december": 12,
}


def valid_publish_date(year: int, month: int = 0, day: int = 0) -> bool:
    if year < 1990 or year > 2100:
        return False
    if month and (month < 1 or month > 12):
        return False
    if day:
        if month < 1 or month > 12:
            return False
        max_day = 31
        if month in (4, 6, 9, 11):
            max_day = 30
        elif month == 2:
            max_day = 29 if (year % 400 == 0 or (year % 4 == 0 and year % 100 != 0)) else 28
        if day < 1 or day > max_day:
            return False
    return True


def extract_publish_date(text: str, lines: Sequence[str], metadata: Dict, doi: str = "") -> Dict[str, str]:
    arxiv_match = re.search(r"arxiv\.(\d{2})(\d{2})\.\d+", doi or "", re.IGNORECASE)
    if arxiv_match:
        year = int(arxiv_match.group(1))
        month = int(arxiv_match.group(2))
        full_year = 2000 + year if year < 90 else 1900 + year
        if valid_publish_date(full_year, month):
            return {"year": str(full_year), "month": str(month), "day": "", "precision": "month"}
        return {"year": str(full_year), "month": "", "day": "", "precision": "year"}

    source = "\n".join([str(value) for value in (metadata or {}).values() if value] + list(lines[:24]) + [text[:1200]])
    candidates: List[Tuple[int, int, int, int]] = []

    month_name_pattern = r"(Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:t(?:ember)?)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)"
    for match in re.finditer(month_name_pattern + r"\s+(\d{1,2},\s*)?((?:19|20)\d{2})", source, re.IGNORECASE):
        month = MONTH_NAME_LOOKUP.get(match.group(1).lower()[:3], 0)
        day_text = (match.group(2) or "").strip(" ,")
        day = int(day_text) if day_text.isdigit() else 0
        year = int(match.group(3))
        if valid_publish_date(year, month, day):
            candidates.append((year, month, day, 90 if day else 85))

    for match in re.finditer(r"\b((?:19|20)\d{2})\s+" + month_name_pattern + r"(?:\s+(\d{1,2}))?\b", source, re.IGNORECASE):
        year = int(match.group(1))
        month = MONTH_NAME_LOOKUP.get(match.group(2).lower()[:3], 0)
        day = int(match.group(3)) if match.group(3) and match.group(3).isdigit() else 0
        if valid_publish_date(year, month, day):
            candidates.append((year, month, day, 84 if day else 80))

    for match in re.finditer(r"\b((?:19|20)\d{2})[-/.](\d{1,2})(?:[-/.](\d{1,2}))?\b", source):
        year = int(match.group(1))
        month = int(match.group(2))
        day = int(match.group(3)) if match.group(3) else 0
        if valid_publish_date(year, month, day):
            candidates.append((year, month, day, 82 if day else 78))

    if candidates:
        year, month, day, _score = sorted(candidates, key=lambda item: (-item[3], -item[0], -item[1], -item[2]))[0]
        return {
            "year": str(year),
            "month": str(month) if month else "",
            "day": str(day) if day else "",
            "precision": "day" if day else ("month" if month else "year"),
        }

    years = []
    for year in re.findall(r"\b(?:19|20)\d{2}\b", source):
        value = int(year)
        if 1990 <= value <= 2100:
            years.append(year)
    if not years:
        return {"year": "", "month": "", "day": "", "precision": "unknown"}
    counts: Dict[str, int] = {}
    for year in years:
        counts[year] = counts.get(year, 0) + 1
    year = sorted(counts.items(), key=lambda item: (-item[1], -int(item[0])))[0][0]
    return {"year": year, "month": "", "day": "", "precision": "year"}


def extract_publish_year(text: str, lines: Sequence[str], metadata: Dict, doi: str = "") -> str:
    return extract_publish_date(text, lines, metadata, doi).get("year", "")


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
            marker_authors, marker_institutions, marker_author_details = extract_marker_author_details(pdf.pages[0])
            inline_authors, inline_institutions, inline_author_details = extract_inline_author_affiliation_details(first_page_lines, title, title_end_index)
            layout_authors, layout_institutions, layout_author_details = extract_layout_author_details(pdf.pages[0])
            authors = marker_authors or inline_authors or layout_authors or extract_authors(first_page_lines, title, title_end_index)
            footnote_institutions, footnote_author_details = extract_footnote_author_details(pdf.pages[0], authors)
            email_institutions, email_author_details = extract_email_author_details(first_page_lines, authors)
            institutions = marker_institutions or inline_institutions or layout_institutions or extract_institutions(first_page_lines, authors, title)
            cleaned_authors = clean_extracted_list(authors, MAX_AUTHOR_COUNT)
            if footnote_author_details:
                institutions = footnote_institutions or institutions
                cleaned_institutions = clean_affiliation_list(institutions, 12)
                author_details = footnote_author_details
            elif email_author_details:
                institutions = email_institutions or institutions
                cleaned_institutions = clean_affiliation_list(institutions, 12)
                author_details = email_author_details
            else:
                cleaned_institutions = clean_affiliation_list(institutions, 12) if (marker_author_details or inline_author_details or layout_author_details) else clean_extracted_list(institutions, 12)
                author_details = marker_author_details or inline_author_details or layout_author_details or build_author_details(cleaned_authors, cleaned_institutions, first_page_lines, title, title_end_index)
            cleaned_authors, author_details = filter_reliable_author_data(cleaned_authors, author_details, title)
            if author_details:
                detail_institutions: List[str] = []
                for detail in author_details:
                    for affiliation in detail.get("affiliations", []) or []:
                        if affiliation and affiliation not in detail_institutions:
                            detail_institutions.append(affiliation)
                if detail_institutions:
                    cleaned_institutions = clean_affiliation_list(detail_institutions, 12)
            keywords = extract_keywords(full_text)
            abstract = extract_abstract(full_text, split_lines(full_text), title, authors)
            if not abstract:
                abstract = extract_front_matter_abstract(first_page_lines, title, authors)
            journal, conference = extract_venue(first_page_lines, metadata)
            doi = extract_doi(full_text, metadata) or extract_vertical_arxiv_doi(first_page_lines)
            publish_date = extract_publish_date(full_text, first_page_lines, metadata, doi)
            publish_year = publish_date.get("year", "")
            source_type = "期刊论文" if journal else ("会议论文" if conference else "其他")
            bibliographic = extract_bibliographic_info(first_page_lines, metadata, len(pdf.pages), doi)

            return {
                "title": clean_extracted_field(title, 260) or "Unknown Title",
                "authors": cleaned_authors,
                "institutions": cleaned_institutions,
                "author_details": author_details,
                "author_affiliations": author_details,
                "keywords": clean_extracted_list(keywords, 10),
                "abstract": clean_extracted_field(abstract, 2000),
                "journal": clean_extracted_field(journal, 260),
                "conference": clean_extracted_field(conference, 260),
                "doi": doi,
                "publish_year": publish_year,
                "publish_month": publish_date.get("month", ""),
                "publish_day": publish_date.get("day", ""),
                "publish_date_precision": publish_date.get("precision", "unknown"),
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
