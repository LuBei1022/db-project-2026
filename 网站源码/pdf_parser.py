import re
import pdfplumber
import pytesseract
from pdf2image import convert_from_path
from typing import List, Dict, Optional


def extract_text_from_page(pdf_path: str, page_num: int = 0) -> tuple[str, list]:
    """提取文本，如果发现是扫描件则自动降级使用 OCR"""
    text = ""
    words = []

    # 尝试原生文本层提取
    try:
        with pdfplumber.open(pdf_path) as pdf:
            if len(pdf.pages) > page_num:
                page = pdf.pages[page_num]
                text = page.extract_text() or ""
                words = page.extract_words(extra_attrs=["size"])
    except Exception as e:
        print(f"pdfplumber 读取异常: {e}")

    # 扫描件判定逻辑：如果提取出来的文本长度小于 50 个字符，认为是扫描件
    if len(text.strip()) < 50:
        print(f"[{pdf_path}] 检测到扫描件或纯图片，启动 OCR ")
        try:
            # 将该页转为高清图片 (DPI=300 保证 OCR 精度)
            images = convert_from_path(pdf_path, first_page=page_num + 1, last_page=page_num + 1, dpi=300)
            if images:
                # 使用英文和简体中文混合识别
                text = pytesseract.image_to_string(images[0], lang='eng+chi_sim')
                # OCR 无法轻易获取每个字的坐标和大小，因此 words 列表置空
                words = []
        except Exception as e:
            print(f"OCR 解析失败: {e}\n请确保已正确安装 poppler 和 tesseract系统依赖。")

    return text, words


def extract_paper_info(pdf_path: str) -> Optional[Dict]:
    text, words = extract_text_from_page(pdf_path, page_num=0)

    if not text:
        return None

    # 智能提取标题
    title = "Unknown Title"
    if words:
        # 如果有文本层数据，基于最大字体提取
        max_font_size = max(w['size'] for w in words)
        title_words = [w['text'] for w in words if w['size'] == max_font_size]
        title = ' '.join(title_words).strip()
        if len(title) < 5:
            title = text.split('\n')[0].strip()
    else:
        # 如果是 OCR 结果，取前两行作为有效标题的近似
        lines = [line.strip() for line in text.split('\n') if line.strip()]
        if lines:
            title = lines[0] if len(lines[0]) > 10 else " ".join(lines[:2])

    # 作者提取模式 (同原文件)
    authors = []
    author_patterns = [
        r'([A-Z][a-z]\.?\s+[A-Z][a-z]+(?:\s*,\s*[A-Z][a-z]\.?\s+[A-Z][a-z]+)*)'
    ]
    for pattern in author_patterns:
        match = re.search(pattern, text)
        if match:
            authors = [a.strip() for a in match.group(1).split(',')]
            break

    # 关键字提取
    keywords = []
    kw_match = re.search(r'(?:Keywords|Index Terms|Key words)[\s:]*(.+)', text, re.IGNORECASE)
    if kw_match:
        kw_text = kw_match.group(1)
        keywords = [k.strip() for k in re.split(r'[,;；，]', kw_text) if k.strip()][:5]

    # 会议/期刊名提取
    conference = None
    conf_patterns = [
        r'Proceedings of (.*?)(?:\n|\.)',
        r'Published in (.*?)(?:\n|\.)',
        r'Presented at (.*?)(?:\n|\.)',
        r'International (?:Journal|Conference) on (.*?)(?:\n|\.)',
        r'[A-Z]{2,}\s\d{4}',
    ]
    for pattern in conf_patterns:
        conf_match = re.search(pattern, text, re.IGNORECASE)
        if conf_match:
            conference = conf_match.group(1).strip() if conf_match.lastindex else conf_match.group(0).strip()
            if conference:
                break

    # 摘要提取 (针对公式和断行进行容错优化)
    abstract = ""
    abstract_match = re.search(
        r'(?:Abstract|摘要)[\s\n]*([\s\S]*?)(?:\n\s*\n|\.\s*\n|1\.\s*Introduction|\n(?:Keywords|Index Terms|参考文献|References))',
        text, re.IGNORECASE)
    if abstract_match:
        # 将断行替换为空格，去除多余空白
        abstract = re.sub(r'\s+', ' ', abstract_match.group(1).strip())

    if not abstract:
        lines = [line.strip() for line in text.split('\n') if line.strip()]
        if len(lines) > 5:
            abstract = " ".join(lines[1:4])

    return {
        "title": title[:500],
        "authors": authors,
        "abstract": abstract,
        "keywords": keywords,
        "conference": conference
    }