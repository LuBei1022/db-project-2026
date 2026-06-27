"""
最小测试脚本：只验证能不能连上 SQL Server（真网站的 manage_db_final 库）。
不调大模型、不碰向量库。

用法：
    python test_db.py
"""
import sys

import config
import db_utils


def main():
    print("=" * 50)
    print("SQL Server 连通性测试")
    print("=" * 50)
    print(f"服务器 : {config.SQLSERVER_HOST}:{config.SQLSERVER_PORT}")
    print(f"数据库 : {config.SQLSERVER_DB}")
    print(f"用户   : {config.SQLSERVER_USER}")

    # 1) 能不能连上
    print("\n正在连接 ...")
    try:
        conn = db_utils.get_connection()
    except Exception as e:
        print(f"[失败] 连不上: {e}")
        print("\n常见原因：")
        print("  - 没装 pymssql：pip install pymssql")
        print("  - SQL Server 没启动，或库不在你本机（可能在同学电脑上）")
        print("  - .env 里的 SQLSERVER_HOST / 用户 / 密码 不对")
        sys.exit(1)
    print("连接成功 ✓")

    # 2) 数一下有多少篇已审核论文
    try:
        cursor = conn.cursor()
        cursor.execute("SELECT COUNT(*) FROM Literature WHERE status = 1")
        total = cursor.fetchone()[0]
        print(f"\n已审核论文数量(Literature, status=1): {total}")
    except Exception as e:
        print(f"[失败] 连上了但查 Literature 表出错: {e}")
        conn.close()
        sys.exit(1)
    finally:
        conn.close()

    # 3) 取前 3 篇标题看看
    try:
        papers = db_utils.search_literature_by_title("", limit=3)  # 空关键词返回[]
        # 空关键词被挡掉了，这里换成直接取样
        conn = db_utils.get_connection()
        cur = conn.cursor(as_dict=True)
        cur.execute("SELECT TOP 3 id, title FROM Literature WHERE status = 1 ORDER BY id DESC")
        for r in cur.fetchall():
            print(f"  - [{r['id']}] {r['title']}")
        conn.close()
    except Exception as e:
        print(f"(取样标题时出错，可忽略: {e})")

    print("\n[成功] 数据库通了，可以执行 python backfill_index.py 建索引了 ✓")


if __name__ == "__main__":
    main()
