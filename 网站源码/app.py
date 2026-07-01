import json
import os
import tempfile
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

from pdf_parser import extract_paper_info

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


UPLOAD_FOLDER = "uploads"
ALLOWED_EXTENSIONS = {"pdf"}
MAX_PDF_BYTES = int(os.getenv("PDF_PARSE_MAX_BYTES", str(50 * 1024 * 1024)))
HOST = os.getenv("PDF_PARSE_HOST", "127.0.0.1")
PORT = int(os.getenv("PDF_PARSE_PORT", "5050"))


def allowed_file(filename):
    return "." in filename and filename.rsplit(".", 1)[1].lower() in ALLOWED_EXTENSIONS


def parse_uploaded_pdf(filepath):
    info = extract_paper_info(filepath)
    if not info:
        return {"error": "Failed to extract info"}, 500
    return {"paper_id": None, "extracted": info}, 200


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
        fd, filepath = tempfile.mkstemp(prefix="pdf_", suffix="_" + filename, dir=app.config["UPLOAD_FOLDER"])
        try:
            with os.fdopen(fd, "wb") as output:
                file.save(output)
            payload, status_code = parse_uploaded_pdf(filepath)
            return jsonify(payload), status_code
        finally:
            if os.path.exists(filepath):
                os.remove(filepath)

    return app


class PdfParseHandler(BaseHTTPRequestHandler):
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
        fd, filepath = tempfile.mkstemp(prefix="pdf_", suffix="_" + safe_name, dir=UPLOAD_FOLDER)
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
    print(f"PDF parse service listening on http://{HOST}:{PORT}")
    server.serve_forever()


if __name__ == "__main__":
    main()
