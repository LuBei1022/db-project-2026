import pymysql
import webbrowser
from neo4j import GraphDatabase


MYSQL_CONFIG = {
    "host": "localhost",
    "user": "root",          # 真实ip
    "password": "123456",      
    "database": "PaperPJ",   
    "charset": "utf8mb4"
}

# Neo4j 配置
NEO4J_URI = "bolt://localhost:7687"
NEO4J_AUTH = ("neo4j", "12345678")  # 你的 Neo4j 账号密码

# 1. 连接 MySQL
def connect_mysql():
    return pymysql.connect(**MYSQL_CONFIG)

# 2. 连接 Neo4j
def connect_neo4j():
    driver = GraphDatabase.driver(NEO4J_URI, auth=NEO4J_AUTH)
    return driver

# 3. 从 MySQL 读取数据 → 写入 Neo4j
def build_kg_from_mysql():
    mysql_conn = connect_mysql()
    neo4j_driver = connect_neo4j()
    cursor = mysql_conn.cursor(pymysql.cursors.DictCursor)

    with neo4j_driver.session() as session:
        # 清空旧图谱
        session.run("MATCH (n) DETACH DELETE n")
        print("已清空 Neo4j 原有数据")

        # 一、创建实体节点

        # 1. 会议/期刊 ConferenceJournal
        cursor.execute("SELECT * FROM ConferenceJournal")
        for item in cursor.fetchall():
            session.run("""
                CREATE (cj:ConferenceJournal {
                    id: $id,
                    name: $name,
                    type: $type,
                    year: $year,
                    location: $location
                })
            """, item)

        # 2. 论文 Paper
        cursor.execute("SELECT * FROM Paper")
        for item in cursor.fetchall():
            session.run("""
                CREATE (p:Paper {
                    id: $id,
                    title: $title,
                    abstract: $abstract,
                    publish_date: $publish_date,
                    pdf_path: $pdf_path
                })
            """, item)

        # 3. 作者 Author
        cursor.execute("SELECT * FROM Author")
        for item in cursor.fetchall():
            session.run("""
                CREATE (a:Author {
                    id: $id,
                    name: $name,
                    email: $email,
                    affiliation: $affiliation
                })
            """, item)

        # 4. 单位 Institution
        cursor.execute("SELECT * FROM Institution")
        for item in cursor.fetchall():
            session.run("""
                CREATE (i:Institution {
                    id: $id,
                    name: $name,
                    address: $address
                })
            """, item)

        # 5. 关键字 Keyword
        cursor.execute("SELECT * FROM Keyword")
        for item in cursor.fetchall():
            session.run("""
                CREATE (k:Keyword {
                    id: $id,
                    word: $word
                })
            """, item)

        # 二、创建关系

        # 1. 论文 -发表于-> 会议/期刊
        cursor.execute("""
            SELECT p.id AS paper_id, cj.id AS cj_id
            FROM Paper p
            JOIN ConferenceJournal cj ON p.conf_journal_id = cj.id
        """)
        for rel in cursor.fetchall():
            session.run("""
                MATCH (p:Paper {id: $paper_id})
                MATCH (cj:ConferenceJournal {id: $cj_id})
                CREATE (p)-[:发表于]->(cj)
            """, rel)

        # 2. 作者 -撰写-> 论文（多对多）
        cursor.execute("SELECT * FROM PaperAuthor")
        for rel in cursor.fetchall():
            session.run("""
                MATCH (a:Author {id: $author_id})
                MATCH (p:Paper {id: $paper_id})
                CREATE (a)-[:撰写 {order: $author_order}]->(p)
            """, rel)

        # 3. 论文 -包含关键字-> 关键字
        cursor.execute("SELECT * FROM PaperKeyword")
        for rel in cursor.fetchall():
            session.run("""
                MATCH (p:Paper {id: $paper_id})
                MATCH (k:Keyword {id: $keyword_id})
                CREATE (p)-[:包含关键字]->(k)
            """, rel)

        # 4. 作者 -隶属于-> 单位
        cursor.execute("SELECT * FROM AuthorInstitution")
        for rel in cursor.fetchall():
            session.run("""
                MATCH (a:Author {id: $author_id})
                MATCH (i:Institution {id: $institution_id})
                CREATE (a)-[:隶属于]->(i)
            """, rel)

       

    # 关闭连接
    cursor.close()
    mysql_conn.close()
    neo4j_driver.close()

if __name__ == "__main__":
    build_kg_from_mysql()
    driver.close()
    # 自动打开 Neo4j 浏览器，并自动填入查询语句
    neo4j_url = "http://localhost:7474/browser/"
    query = "MATCH (n)-[r]->(m) RETURN n, r, m"
    webbrowser.open(f"{neo4j_url}?query={query}")