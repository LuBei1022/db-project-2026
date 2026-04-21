import os
from flask import Flask, request, jsonify
from werkzeug.utils import secure_filename
from pdf_parser import extract_paper_info
from db_utils import save_paper_to_db, DB_CONFIG
import pymysql
import csv
from io import StringIO, TextIOWrapper
from pdf_parser import extract_paper_info, extract_full_text_smart 
from db_utils import save_paper_to_db, DB_CONFIG
from rag_utils import add_paper_to_vector_db 

app = Flask(__name__)
UPLOAD_FOLDER = 'uploads'
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER
os.makedirs(UPLOAD_FOLDER, exist_ok=True)
ALLOWED_EXTENSIONS = {'pdf'}

def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

@app.route('/upload', methods=['POST'])
def upload_paper():
    if 'file' not in request.files:
        return jsonify({'error': 'No file part'}), 400
    file = request.files['file']
    if file.filename == '':
        return jsonify({'error': 'No selected file'}), 400
    if not allowed_file(file.filename):
        return jsonify({'error': 'File type not allowed'}), 400

    filename = secure_filename(file.filename)
    filepath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    file.save(filepath)
    
    try:
        info = extract_paper_info(filepath)
        if not info:
            return jsonify({'error': 'Failed to extract metadata'}), 500
        paper_id = save_paper_to_db(info, filepath)
        
        print(f"开始为论文 ID {paper_id} 构建全文向量索引...")
        # 1. 提取全文 (遇到图片会自动触发 OCR)
        full_text = extract_full_text_smart(filepath)
        
        is_success = add_paper_to_vector_db(paper_id, full_text)
        
        if is_success:
            return jsonify({
                'paper_id': paper_id, 
                'extracted': info,
                'msg': '文件上传成功，结构化数据与全文向量库均已更新！'
            }), 200
        else:
            return jsonify({'error': '全文向量化失败，但元数据已存入数据库'}), 500

    except Exception as e:
        return jsonify({'error': str(e)}), 500


@app.route('/search/author', methods=['GET'])
def search_author():
    name = request.args.get('name', '')
    if not name:
        return jsonify({'error': 'Missing author name'}), 400
    conn = pymysql.connect(**DB_CONFIG)
    cursor = conn.cursor(pymysql.cursors.DictCursor)
    sql = """
        SELECT DISTINCT p.id, p.title, p.abstract
        FROM Paper p
        JOIN PaperAuthor pa ON p.id = pa.paper_id
        JOIN Author a ON pa.author_id = a.id
        WHERE a.name LIKE %s
    """
    cursor.execute(sql, (f'%{name}%',))
    papers = cursor.fetchall()
    cursor.close()
    conn.close()
    return jsonify(papers), 200

@app.route('/search/keyword', methods=['GET'])
def search_keyword():
    kw = request.args.get('kw', '')
    if not kw:
        return jsonify({'error': 'Missing keyword'}), 400
    conn = pymysql.connect(**DB_CONFIG)
    cursor = conn.cursor(pymysql.cursors.DictCursor)
    sql = """
        SELECT DISTINCT p.id, p.title, p.abstract
        FROM Paper p
        JOIN PaperKeyword pk ON p.id = pk.paper_id
        JOIN Keyword k ON pk.keyword_id = k.id
        WHERE k.word LIKE %s
    """
    cursor.execute(sql, (f'%{kw}%',))
    papers = cursor.fetchall()
    cursor.close()
    conn.close()
    return jsonify(papers), 200

@app.route('/export/csv', methods=['GET'])
def export_csv():
    conn = pymysql.connect(**DB_CONFIG)
    cursor = conn.cursor(pymysql.cursors.DictCursor)
    cursor.execute("SELECT id, title, abstract, pdf_path FROM Paper")
    papers = cursor.fetchall()
    cursor.close()
    conn.close()
    output = StringIO()
    writer = csv.DictWriter(output, fieldnames=['id', 'title', 'abstract', 'pdf_path'])
    writer.writeheader()
    writer.writerows(papers)
    return jsonify({'csv_data': output.getvalue()})

@app.route('/import/csv', methods=['POST'])
def import_csv():
    if 'file' not in request.files:
        return jsonify({'error': 'No file'}), 400
    file = request.files['file']
    if file.filename == '':
        return jsonify({'error': 'No file selected'}), 400
    stream = TextIOWrapper(file.stream, encoding='utf-8')
    reader = csv.DictReader(stream)
    conn = pymysql.connect(**DB_CONFIG)
    cursor = conn.cursor()
    inserted = 0
    for row in reader:
        try:
            cursor.execute(
                "INSERT INTO Paper (title, abstract, pdf_path) VALUES (%s, %s, %s)",
                (row.get('title'), row.get('abstract', ''), row.get('pdf_path', ''))
            )
            inserted += 1
        except:
            continue
    conn.commit()
    cursor.close()
    conn.close()
    return jsonify({'inserted': inserted}), 200

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5050)