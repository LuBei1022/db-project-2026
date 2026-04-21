import re
import pdfplumber
from typing import List, Dict, Optional
import pytesseract


def extract_paper_info(pdf_path: str) -> Optional[Dict]:

    try:
        with pdfplumber.open(pdf_path) as pdf:
            if len(pdf.pages) == 0:
                return None
            first_page = pdf.pages[0]
            text = first_page.extract_text()
            if not text:
                return None
    except Exception as e:
        print(f"PDF读取错误: {e}")
        return None

    # 智能提取标题 (基于最大字体) ---
    words = first_page.extract_words(extra_attrs=["size"])
    if not words:
        title = text.split('\n')[0].strip()
    else:
        max_font_size = max(w['size'] for w in words)
        title_words = [w['text'] for w in words if w['size'] == max_font_size]
        title = ' '.join(title_words).strip()
        # 如果提取出的标题过短或不合理，用第一行作为后备
        if len(title) < 5:
            title = text.split('\n')[0].strip()
    if not title:
        title = "Unknown Title"

    # 增强作者提取 (更全面的正则) ---
    # 查找位于标题和摘要之间的文本块，并匹配更复杂的作者格式
    authors = []
    # 中英文作者模式
    author_patterns = [
        r'([A-Z][a-z]\.?\s+[A-Z][a-z]+(?:\s+[A-Z][a-z]+)?)',  # J. Smith
        r'([A-Z][a-z]+\s+[A-Z][a-z]+)',  # John Smith
        r'([A-Z][a-z]+\s+[A-Z]\.\s+[A-Z][a-z]+)',  # John A. Smith
        r'([A-Z][a-z]+\s+[A-Z][a-z]+(?:,[A-Z]\.[A-Z][a-z]+)?)',  # Smith, J.A.
        r'([\u4e00-\u9fff]{2,4})',  # 中文姓名
    ]

    # 在标题后的文本中寻找作者
    lines_after_title = text.split('\n')
    for line in lines_after_title:
        if any(kw in line for kw in ['Abstract', '摘要', 'Introduction', '1.']):
            break
        for pattern in author_patterns:
            found = re.findall(pattern, line)
            if found:
                authors.extend([a.strip() for a in found if len(a.strip()) > 2])
                break
    authors = list(dict.fromkeys(authors))[:5]

#单位提取
    institutions = []
    inst_patterns = [
        r'([\w\s]+University[\w\s,]*)',
        r'([\w\s]+Institute[\w\s,]*)',
        r'([\w\s]+College[\w\s,]*)',
        r'([\w\s]+Lab[\w\s,]*)',
        r'([\w\s]+School of[\w\s,]*)',
        r'([\u4e00-\u9fff]+大学[\u4e00-\u9fff]*)',  # 中文大学
        r'([\u4e00-\u9fff]+学院[\u4e00-\u9fff]*)',  # 中文学院
    ]
    for pattern in inst_patterns:
        for match in re.finditer(pattern, text, re.IGNORECASE):
            inst = match.group(1).strip()
            if inst and len(inst) > 5 and inst not in institutions:
                institutions.append(inst)
    institutions = institutions[:3]

    #关键词提取 ---
    keywords = []
    kw_match = re.search(r'(?:Keywords|关键词)[:：]?\s*(.+)', text, re.IGNORECASE)
    if kw_match:
        kw_text = kw_match.group(1)
        keywords = [k.strip() for k in re.split(r'[,;；，]', kw_text) if k.strip()][:5]

    #增强会议/期刊名提取 ---
    conference = None
    conf_patterns = [
        r'Proceedings of (.*?)(?:\n|\.)',
        r'Published in (.*?)(?:\n|\.)',
        r'Presented at (.*?)(?:\n|\.)',
        r'International (?:Journal|Conference) on (.*?)(?:\n|\.)',
        r'[A-Z]{2,}\s\d{4}',  # 会议简称+年份，如 ICML 2024
    ]
    for pattern in conf_patterns:
        conf_match = re.search(pattern, text, re.IGNORECASE)
        if conf_match:
            conference = conf_match.group(1).strip() if conf_match.lastindex else conf_match.group(0).strip()
            if conference:
                break

    #摘要提取 (支持中英文 Abstract/摘要) ---
    abstract = ""
    abstract_match = re.search(r'(?:Abstract|摘要)[\s\n]*(.*?)(?:\n\s*\n|\.\s*\n|\n(?:Keywords|参考文献|References))',
                               text, re.IGNORECASE | re.DOTALL)
    if abstract_match:
        abstract = abstract_match.group(1).strip().replace('\n', ' ')
    if not abstract and len(lines_after_title) > 1:
        # 后备：取一个较长的段落作为摘要
        for line in lines_after_title[1:10]:
            if len(line) > 100:
                abstract = line.strip()
                break

    return {
        'title': title,
        'authors': authors,
        'institutions': institutions,
        'keywords': keywords,
        'abstract': abstract,
        'conference': conference
    }



def extract_full_text_smart(pdf_path: str) -> str:
    #RAG专用 智能全文提取：优先光速直读，遇扫描版自动回退至 OCR 识别。

    full_text = ""
    try:
        with pdfplumber.open(pdf_path) as pdf:
            if len(pdf.pages) == 0:
                return ""
            
            # 遍历 PDF 的每一页
            for i, page in enumerate(pdf.pages):
                #优先尝试直接提取底层文本 (速度极快)
                text = page.extract_text()
                
                # 如果提取出的文本极少（少于50字），我们判定这大概率是一页扫描件或纯图片
                if not text or len(text.strip()) < 50:
                    print(f"正在使用 OCR 深度扫描第 {i+1} 页...")
                    
                    # 将 PDF 当前页渲染为高分辨率图片 (300 dpi 保证 OCR 认得清)
                    img = page.to_image(resolution=300).original
                    
                    # 调用 Tesseract 识别图片中的文字 (支持中文简体和英文)
                    text = pytesseract.image_to_string(img, lang='chi_sim+eng')
                
                if text:
                    full_text += text + "\n\n"
                    
        return full_text
    except Exception as e:
        print(f"RAG智能提取失败，文件路径: {pdf_path}, 错误: {e}")
        return ""

