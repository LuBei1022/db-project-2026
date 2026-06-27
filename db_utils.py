"""
数据库访问层 —— 对接真网站的 SQL Server（manage_db_final）。
论文主表是 Literature，PDF 文件信息在 LiteratureFile。

注意：RAG 向量库里每段文本绑定的 paper_id 就是这里的 Literature.id，
和网站保持一致（PPT 第26页要求“检索结果自动关联 SQL Server 论文 ID”）。
"""
import config

try:
    import pymssql
    _PYMSSQL_AVAILABLE = True
except Exception:
    _PYMSSQL_AVAILABLE = False


def get_connection():
    """建立到 SQL Server 的连接。"""
    if not _PYMSSQL_AVAILABLE:
        raise RuntimeError("未安装 pymssql，请先执行：pip install pymssql")
    return pymssql.connect(
        server=config.SQLSERVER_HOST,
        port=config.SQLSERVER_PORT,
        user=config.SQLSERVER_USER,
        password=config.SQLSERVER_PASSWORD,
        database=config.SQLSERVER_DB,
        charset="utf8",
    )


def search_literature_by_title(title_keyword: str, limit: int = 20):
    """
    按标题模糊搜索论文，供“智能问答”页面选论文用。
    只返回已通过审核(status=1)的论文。
    """
    if not title_keyword or not title_keyword.strip():
        return []
    conn = get_connection()
    try:
        cursor = conn.cursor(as_dict=True)
        sql = """
            SELECT TOP (%(limit)s) id, title, publish_year, journal_name, abstract_text
            FROM Literature
            WHERE status = 1 AND title LIKE %(kw)s
            ORDER BY is_top DESC, id DESC
        """
        cursor.execute(sql, {"limit": limit, "kw": f"%{title_keyword.strip()}%"})
        rows = cursor.fetchall()
        # 摘要太长就截断，避免列表页传输过多数据
        for r in rows:
            ab = r.get("abstract_text") or ""
            r["abstract_text"] = (ab[:200] + "...") if len(ab) > 200 else ab
        return rows
    finally:
        conn.close()


def get_literature_by_id(paper_id: int):
    """按 id 取单篇论文的基础信息。"""
    conn = get_connection()
    try:
        cursor = conn.cursor(as_dict=True)
        cursor.execute(
            """
            SELECT id, title, abstract_text, keywords, publish_year,
                   journal_name, conference_name
            FROM Literature
            WHERE id = %(id)s
            """,
            {"id": paper_id},
        )
        return cursor.fetchone()
    finally:
        conn.close()


def get_pdf_path_for_literature(paper_id: int):
    """
    取某篇论文的 PDF 文件路径（LiteratureFile 表里 file_type 为 pdf 的那条）。
    没有就返回 None。
    """
    conn = get_connection()
    try:
        cursor = conn.cursor(as_dict=True)
        cursor.execute(
            """
            SELECT TOP 1 file_path
            FROM LiteratureFile
            WHERE literature_id = %(id)s
              AND status = 1
              AND (file_type LIKE '%%pdf%%' OR file_name LIKE '%%.pdf')
            ORDER BY orderid ASC, id ASC
            """,
            {"id": paper_id},
        )
        row = cursor.fetchone()
        return row["file_path"] if row else None
    finally:
        conn.close()


def get_all_literature_for_index():
    """
    回填索引用：取所有已审核论文的 id / 标题 / 摘要 / 关键词。
    PDF 路径在回填脚本里单独按需查询。
    """
    conn = get_connection()
    try:
        cursor = conn.cursor(as_dict=True)
        cursor.execute(
            """
            SELECT id, title, abstract_text, keywords
            FROM Literature
            WHERE status = 1
            ORDER BY id ASC
            """
        )
        return cursor.fetchall()
    finally:
        conn.close()


def build_metadata_text(lit_row: dict) -> str:
    """
    当某篇论文没有 PDF 全文时的降级方案：
    用 标题 + 关键词 + 摘要 拼成一段可被向量化、可被问答的文本。
    """
    parts = []
    if lit_row.get("title"):
        parts.append(f"标题：{lit_row['title']}")
    if lit_row.get("keywords"):
        parts.append(f"关键词：{lit_row['keywords']}")
    if lit_row.get("abstract_text"):
        parts.append(f"摘要：{lit_row['abstract_text']}")
    return "\n".join(parts).strip()
