import pymysql


DB_CONFIG = {
    'host': '127.0.0.1',
    'user': 'root',
    'password': '12345678',
    'database': 'PaperPJ',
    'charset': 'utf8mb4'
}

def get_connection():
    return pymysql.connect(**DB_CONFIG)

def save_paper_to_db(paper_info, pdf_path):
    conn = get_connection()
    cursor = conn.cursor()
    try:
        # 插入 Paper 表
        sql_paper = "INSERT INTO Paper (title, abstract, pdf_path) VALUES (%s, %s, %s)"
        cursor.execute(sql_paper, (paper_info['title'], paper_info.get('abstract', ''), pdf_path))
        paper_id = cursor.lastrowid

        # 处理作者
        for idx, author_name in enumerate(paper_info.get('authors', [])):
            cursor.execute("SELECT id FROM Author WHERE name = %s", (author_name,))
            row = cursor.fetchone()
            if row:
                author_id = row[0]
            else:
                cursor.execute("INSERT INTO Author (name) VALUES (%s)", (author_name,))
                author_id = cursor.lastrowid
            cursor.execute(
                "INSERT IGNORE INTO PaperAuthor (paper_id, author_id, author_order) VALUES (%s, %s, %s)",
                (paper_id, author_id, idx+1)
            )

        conn.commit()
        return paper_id
    except Exception as e:
        conn.rollback()
        raise e
    finally:
        cursor.close()
        conn.close()