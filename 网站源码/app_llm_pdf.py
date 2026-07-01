import hashlib
import json
import os
import re
import tempfile
import urllib.error
import urllib.request
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

import pdfplumber

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
PROMPT_VERSION = "pdf-metadata-v1"

JOURNAL_SOURCE_TYPE = "\u671f\u520a\u8bba\u6587"
CONFERENCE_SOURCE_TYPE = "\u4f1a\u8bae\u8bba\u6587"
OTHER_SOURCE_TYPE = "\u5176\u4ed6"

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


def allowed_file(filename):
    return "." in filename and filename.rsplit(".", 1)[1].lower() in ALLOWED_EXTENSIONS


def parse_uploaded_pdf(filepath):
    info = extract_paper_info_llm(filepath)
    if not info:
        return {"error": "Failed to extract info"}, 500
    return {"paper_id": None, "extracted": info}, 200


def extract_paper_info_llm(pdf_path):
    try:
        file_hash = hash_file(pdf_path)
        cache_key = build_cache_key(file_hash)
        cached = load_cached_result(cache_key)
        if cached:
            return cached

        context = extract_pdf_context(pdf_path)
        if not context["text"]:
            return None

        raw = call_llm_for_metadata(context)
        info = normalize_extracted(raw, context)
        if LLM_CACHE_ENABLED:
            save_cached_result(cache_key, info)
        return info
    except Exception as exc:
        print("LLM PDF parse error: {0}".format(exc))
        return None


def build_cache_key(file_hash):
    model = get_llm_model()
    return "{0}:{1}:{2}".format(PROMPT_VERSION, model, file_hash)


def cache_path():
    return os.path.join(UPLOAD_FOLDER, "llm_pdf_cache.json")


def load_cached_result(key):
    if not LLM_CACHE_ENABLED:
        return None
    path = cache_path()
    if not os.path.exists(path):
        return None
    try:
        with open(path, "r", encoding="utf-8") as handle:
            data = json.load(handle)
        value = data.get(key)
        return value if isinstance(value, dict) else None
    except Exception:
        return None


def save_cached_result(key, info):
    os.makedirs(UPLOAD_FOLDER, exist_ok=True)
    path = cache_path()
    try:
        data = {}
        if os.path.exists(path):
            with open(path, "r", encoding="utf-8") as handle:
                existing = json.load(handle)
                if isinstance(existing, dict):
                    data = existing
        data[key] = info
        with open(path, "w", encoding="utf-8") as handle:
            json.dump(data, handle, ensure_ascii=False, indent=2)
    except Exception as exc:
        print("LLM cache write warning: {0}".format(exc))


def hash_file(path):
    digest = hashlib.sha256()
    with open(path, "rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def extract_pdf_context(pdf_path):
    page_texts = []
    metadata = {}
    page_count = 0
    with pdfplumber.open(pdf_path) as pdf:
        metadata = {str(k): clean_context_text(str(v)) for k, v in (pdf.metadata or {}).items() if v}
        page_count = len(pdf.pages)
        for index, page in enumerate(pdf.pages[:max(1, MAX_TEXT_PAGES)]):
            raw_text = page.extract_text(x_tolerance=2, y_tolerance=3) or ""
            cleaned = clean_context_text(raw_text)
            if cleaned:
                page_texts.append("[Page {0}]\n{1}".format(index + 1, cleaned))

    text = "\n\n".join(page_texts).strip()
    if len(text) > MAX_TEXT_CHARS:
        text = text[:MAX_TEXT_CHARS]

    return {
        "metadata": metadata,
        "page_count": page_count,
        "text": text,
        "filename": os.path.basename(pdf_path),
    }


def clean_context_text(value):
    text = (value or "").replace("\x00", " ").replace("\u00a0", " ")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    lines = []
    for raw_line in text.replace("\r", "\n").split("\n"):
        line = re.sub(r"[ \t]+", " ", raw_line).strip()
        if line:
            lines.append(line)
    return "\n".join(lines)


def get_llm_model():
    return os.getenv("LLM_MODEL") or os.getenv("OPENAI_MODEL") or "gpt-4o-mini"


def get_llm_api_url():
    explicit = os.getenv("LLM_API_URL")
    if explicit:
        return explicit

    base_url = os.getenv("LLM_BASE_URL") or os.getenv("OPENAI_BASE_URL") or "https://api.openai.com/v1"
    return base_url.rstrip("/") + "/chat/completions"


def get_llm_api_key():
    return os.getenv("LLM_API_KEY") or os.getenv("OPENAI_API_KEY") or ""


def call_llm_for_metadata(context):
    api_url = get_llm_api_url()
    api_key = get_llm_api_key()
    if not api_key and not is_local_url(api_url):
        raise RuntimeError("Set LLM_API_KEY or OPENAI_API_KEY before starting app_llm_pdf.py")

    payload = {
        "model": get_llm_model(),
        "messages": [
            {
                "role": "system",
                "content": (
                    "You extract academic paper metadata from PDF text. "
                    "Return only a valid JSON object. Do not add markdown."
                ),
            },
            {
                "role": "user",
                "content": build_prompt(context),
            },
        ],
        "temperature": LLM_TEMPERATURE,
        "max_tokens": LLM_MAX_TOKENS,
    }

    use_response_format = os.getenv("LLM_RESPONSE_FORMAT", "json_object").strip().lower()
    if use_response_format not in {"0", "false", "no", "none"}:
        payload["response_format"] = {"type": "json_object"}

    try:
        response = post_json(api_url, payload, api_key)
    except urllib.error.HTTPError as exc:
        if "response_format" not in payload or exc.code not in {400, 404, 422}:
            raise
        payload.pop("response_format", None)
        response = post_json(api_url, payload, api_key)

    content = extract_chat_content(response)
    return load_model_json(content)


def is_local_url(url):
    lower = (url or "").lower()
    return "://127.0.0.1" in lower or "://localhost" in lower


def build_prompt(context):
    schema = {
        "title": "",
        "authors": ["Author Name"],
        "institutions": ["Institution Name"],
        "author_details": [
            {
                "name": "Author Name",
                "name_cn": "",
                "name_en": "Author Name",
                "affiliations": ["Institution Name"],
                "affiliation_text": "Institution Name",
                "markers": ["1"],
                "mapping_status": "matched",
            }
        ],
        "keywords": ["keyword"],
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
        "page_count": str(context.get("page_count") or ""),
    }
    return (
        "Extract metadata from the following academic PDF text.\n"
        "Use evidence from the PDF only. If a value is not visible, use an empty string or empty array.\n"
        "source_type must be one of: {journal}, {conference}, {other}.\n"
        "publish_date_precision must be one of: day, month, year, unknown.\n"
        "For author_details, map each author to affiliations when markers or layout make it possible.\n\n"
        "JSON schema example:\n{schema}\n\n"
        "PDF metadata:\n{metadata}\n\n"
        "PDF text:\n{text}"
    ).format(
        journal=JOURNAL_SOURCE_TYPE,
        conference=CONFERENCE_SOURCE_TYPE,
        other=OTHER_SOURCE_TYPE,
        schema=json.dumps(schema, ensure_ascii=False, indent=2),
        metadata=json.dumps(context.get("metadata") or {}, ensure_ascii=False, indent=2),
        text=context.get("text") or "",
    )


def post_json(url, payload, api_key):
    body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
    headers = {
        "Content-Type": "application/json; charset=utf-8",
        "Accept": "application/json",
    }
    if api_key:
        headers["Authorization"] = "Bearer " + api_key
    request = urllib.request.Request(url, data=body, headers=headers, method="POST")
    with urllib.request.urlopen(request, timeout=LLM_TIMEOUT_SECONDS) as response:
        raw = response.read().decode("utf-8", "replace")
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

    fenced = re.search(r"```(?:json)?\s*(\{.*?\})\s*```", text, re.IGNORECASE | re.DOTALL)
    if fenced:
        return json.loads(fenced.group(1))

    start = text.find("{")
    end = text.rfind("}")
    if start >= 0 and end > start:
        return json.loads(text[start:end + 1])

    raise RuntimeError("LLM response is not JSON")


def normalize_extracted(raw, context):
    raw = raw if isinstance(raw, dict) else {}
    info = dict(DEFAULT_EXTRACTED)

    info["title"] = limit_text(clean_value(raw.get("title")), 260) or "Unknown Title"
    info["authors"] = normalize_authors(raw.get("authors"))
    info["institutions"] = normalize_string_list(raw.get("institutions"), 12, 260)
    info["keywords"] = normalize_string_list(raw.get("keywords"), 10, 120, split_commas=True)
    info["abstract"] = limit_text(clean_value(raw.get("abstract")), 8000)
    info["journal"] = limit_text(clean_value(raw.get("journal") or raw.get("journal_name")), 260)
    info["conference"] = limit_text(clean_value(raw.get("conference") or raw.get("conference_name")), 260)
    info["doi"] = normalize_doi(clean_value(raw.get("doi") or raw.get("DOI")))

    year = clean_value(first_value(raw, "publish_year", "publication_year", "pub_year", "year"))
    month = clean_value(first_value(raw, "publish_month", "publication_month", "pub_month", "month"))
    day = clean_value(first_value(raw, "publish_day", "publication_day", "pub_day", "day"))
    precision = clean_value(first_value(raw, "publish_date_precision", "publication_date_precision", "date_precision", "precision"))
    if not year:
        date_text = clean_value(first_value(raw, "publish_date", "publication_date", "published_date", "date"))
        year, month, day, precision = parse_date_parts(date_text, year, month, day, precision)

    info["publish_year"] = normalize_year(year)
    info["publish_month"] = normalize_month(month)
    info["publish_day"] = normalize_day(day) if info["publish_month"] else ""
    info["publish_date_precision"] = normalize_precision(precision, info["publish_year"], info["publish_month"], info["publish_day"])

    info["volume"] = limit_text(clean_value(raw.get("volume")), 60)
    info["issue"] = limit_text(clean_value(raw.get("issue")), 60)
    info["pages"] = limit_text(clean_value(raw.get("pages")), 80)
    info["publisher"] = limit_text(clean_value(raw.get("publisher")), 120)
    info["page_count"] = clean_value(raw.get("page_count")) or str(context.get("page_count") or "")

    info["source_type"] = normalize_source_type(raw.get("source_type"), info["journal"], info["conference"])
    author_details = normalize_author_details(raw, info["authors"], info["institutions"])
    if not info["authors"]:
        info["authors"] = [item["name"] for item in author_details if item.get("name")]
    if not info["institutions"]:
        info["institutions"] = institutions_from_author_details(author_details)
    info["author_details"] = author_details
    info["author_affiliations"] = author_details

    return info


def first_value(source, *keys):
    for key in keys:
        if key in source and source.get(key) not in (None, ""):
            return source.get(key)
    return ""


def normalize_authors(value):
    values = []
    if isinstance(value, list):
        for item in value:
            if isinstance(item, dict):
                name = clean_value(item.get("name") or item.get("name_cn") or item.get("name_en"))
            else:
                name = clean_value(item)
            add_unique(values, name)
    else:
        for item in split_text_items(clean_value(value), split_commas=True):
            add_unique(values, item)
    return values[:30]


def normalize_string_list(value, max_items, max_length, split_commas=False):
    values = []
    if isinstance(value, list):
        for item in value:
            add_unique(values, limit_text(clean_value(item), max_length))
    else:
        for item in split_text_items(clean_value(value), split_commas=split_commas):
            add_unique(values, limit_text(item, max_length))
    return values[:max_items]


def split_text_items(value, split_commas=False):
    if not value:
        return []
    separators = r"[;\|\n\r\uff1b]+"
    if split_commas:
        separators = r"[,;\|\n\r\uff0c\uff1b]+"
    return [clean_value(item) for item in re.split(separators, value) if clean_value(item)]


def normalize_author_details(raw, authors, institutions):
    token = raw.get("author_details") or raw.get("author_affiliations")
    details = []
    if isinstance(token, list):
        for item in token:
            detail = normalize_author_detail(item, institutions)
            if detail and not any(existing["name"].lower() == detail["name"].lower() for existing in details):
                details.append(detail)

    if details:
        return details[:30]

    details = []
    for author in authors:
        affiliations = list(institutions) if len(institutions) == 1 else []
        details.append(create_author_detail(author, affiliations, [], "single_institution" if affiliations else "unmatched"))
    return details[:30]


def normalize_author_detail(item, institutions):
    if not isinstance(item, dict):
        name = clean_value(item)
        return create_author_detail(name, [], [], "unmatched") if name else None

    name = clean_value(item.get("name") or item.get("name_cn") or item.get("name_en"))
    if not name:
        return None
    affiliations = normalize_string_list(
        item.get("affiliations") or item.get("affiliation") or item.get("institution") or item.get("affiliation_text"),
        8,
        260,
    )
    if not affiliations and len(institutions) == 1:
        affiliations = list(institutions)
    markers = normalize_string_list(item.get("markers"), 8, 20)
    status = clean_value(item.get("mapping_status")) or ("matched" if affiliations else "unmatched")
    detail = create_author_detail(name, affiliations, markers, status)
    name_cn = clean_value(item.get("name_cn"))
    name_en = clean_value(item.get("name_en"))
    if name_cn:
        detail["name_cn"] = name_cn
    if name_en:
        detail["name_en"] = name_en
    return detail


def create_author_detail(name, affiliations, markers, mapping_status):
    clean_name = clean_value(name)
    name_cn = clean_name if contains_chinese(clean_name) else ""
    name_en = "" if name_cn else clean_name
    return {
        "name": clean_name,
        "name_cn": name_cn,
        "name_en": name_en,
        "affiliations": affiliations or [],
        "affiliation_text": "\uff1b".join(affiliations or []),
        "markers": markers or [],
        "mapping_status": mapping_status or "unmatched",
    }


def institutions_from_author_details(details):
    values = []
    for detail in details:
        for affiliation in detail.get("affiliations") or []:
            add_unique(values, affiliation)
    return values[:12]


def normalize_source_type(value, journal, conference):
    text = clean_value(value).lower()
    if "\u671f\u520a" in text or "journal" in text:
        return JOURNAL_SOURCE_TYPE
    if "\u4f1a\u8bae" in text or "conference" in text or "proceedings" in text:
        return CONFERENCE_SOURCE_TYPE
    if journal:
        return JOURNAL_SOURCE_TYPE
    if conference:
        return CONFERENCE_SOURCE_TYPE
    return OTHER_SOURCE_TYPE


def normalize_doi(value):
    text = clean_value(value)
    text = re.sub(r"^(?:https?://)?(?:dx\.)?doi\.org/", "", text, flags=re.IGNORECASE).strip()
    match = re.search(r"10\.\d{4,9}/[^\s\"<>]+", text, re.IGNORECASE)
    if match:
        return match.group(0).rstrip(".,;)")
    return text


def parse_date_parts(date_text, year, month, day, precision):
    text = clean_value(date_text)
    if not text:
        return year, month, day, precision
    match = re.search(r"\b((?:19|20)\d{2})[-/.年]\s*(\d{1,2})(?:[-/.月]\s*(\d{1,2}))?", text)
    if match:
        return match.group(1), match.group(2), match.group(3) or day, "day" if match.group(3) else "month"
    match = re.search(r"\b((?:19|20)\d{2})\b", text)
    if match:
        return match.group(1), month, day, "year"
    return year, month, day, precision


def normalize_year(value):
    match = re.search(r"(?:19|20)\d{2}", value or "")
    if not match:
        return ""
    number = int(match.group(0))
    return str(number) if 1900 <= number <= 2100 else ""


def normalize_month(value):
    match = re.search(r"\d{1,2}", value or "")
    if not match:
        return ""
    number = int(match.group(0))
    return str(number) if 1 <= number <= 12 else ""


def normalize_day(value):
    match = re.search(r"\d{1,2}", value or "")
    if not match:
        return ""
    number = int(match.group(0))
    return str(number) if 1 <= number <= 31 else ""


def normalize_precision(value, year, month, day):
    text = (value or "").strip().lower()
    if text in {"day", "month", "year", "unknown"}:
        return text
    if day:
        return "day"
    if month:
        return "month"
    return "year" if year else "unknown"


def clean_value(value):
    if value is None:
        return ""
    if isinstance(value, (dict, list)):
        value = json.dumps(value, ensure_ascii=False)
    text = str(value)
    text = text.replace("\x00", " ").replace("\ufffd", " ").replace("\u00a0", " ")
    text = re.sub(r"\(cid:\d+\)", " ", text)
    text = re.sub(r"\?{2,}", " ", text)
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


def add_unique(values, value):
    current = clean_value(value)
    if not current:
        return
    if not any(existing.lower() == current.lower() for existing in values):
        values.append(current)


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
        fd, filepath = tempfile.mkstemp(prefix="llm_pdf_", suffix="_" + filename, dir=app.config["UPLOAD_FOLDER"])
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
        return jsonify({"ok": True, "service": "llm-pdf-parser", "model": get_llm_model()})

    return app


class PdfParseHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path.split("?", 1)[0] == "/health":
            self.write_json({"ok": True, "service": "llm-pdf-parser", "model": get_llm_model()}, 200)
        else:
            self.write_json({"error": "Not found"}, 404)

    def do_POST(self):
        if self.path.split("?", 1)[0] != "/upload":
            self.write_json({"error": "Not found"}, 404)
            return

        content_type = self.headers.get("Content-Type", "")
        if "multipart/form-data" not in content_type:
            self.write_json({"error": "No file part"}, 400)
            return

        boundary = self.get_boundary(content_type)
        if not boundary:
            self.write_json({"error": "No file part"}, 400)
            return

        length = int(self.headers.get("Content-Length", "0") or "0")
        if length <= 0 or length > MAX_PDF_BYTES:
            self.write_json({"error": "PDF file is too large"}, 413)
            return

        body = self.rfile.read(length)
        filename, file_content = self.extract_file(body, boundary)
        if not file_content:
            self.write_json({"error": "No selected file"}, 400)
            return
        if not allowed_file(filename or "upload.pdf"):
            self.write_json({"error": "File type not allowed"}, 400)
            return

        os.makedirs(UPLOAD_FOLDER, exist_ok=True)
        safe_name = secure_filename(filename or "upload.pdf")
        fd, filepath = tempfile.mkstemp(prefix="llm_pdf_", suffix="_" + safe_name, dir=UPLOAD_FOLDER)
        try:
            with os.fdopen(fd, "wb") as output:
                output.write(file_content)
            payload, status_code = parse_uploaded_pdf(filepath)
            self.write_json(payload, status_code)
        finally:
            if os.path.exists(filepath):
                os.remove(filepath)

    def log_message(self, format, *args):
        return

    def get_boundary(self, content_type):
        for part in content_type.split(";"):
            part = part.strip()
            if part.startswith("boundary="):
                boundary = part.split("=", 1)[1].strip('"')
                return boundary.encode("utf-8")
        return None

    def extract_file(self, body, boundary):
        marker = b"--" + boundary
        for part in body.split(marker):
            if b"filename=" not in part:
                continue
            header, _, content = part.partition(b"\r\n\r\n")
            if not content:
                continue
            content = content.rsplit(b"\r\n", 1)[0]
            filename = "upload.pdf"
            disposition = header.decode("utf-8", "ignore").split("\r\n", 1)[0]
            for header_part in disposition.split(";"):
                header_part = header_part.strip()
                if header_part.startswith("filename="):
                    filename = header_part.split("=", 1)[1].strip('"') or filename
                    break
            return filename, content
        return None, None

    def write_json(self, payload, status_code):
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
    print("LLM PDF parse service listening on http://{0}:{1}".format(HOST, PORT))
    server.serve_forever()


if __name__ == "__main__":
    main()
