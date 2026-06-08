param(
    [string]$ConnectionString = "Data Source=(local)\SQLEXPRESS;Initial Catalog=manage_db;User ID=sa;Password=123456;Encrypt=False;TrustServerCertificate=True;",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$Stamp = Get-Date -Format "yyyyMMddHHmmss"

Add-Type -AssemblyName System.Data
Add-Type -AssemblyName System.Web

function U([string]$value) {
    return [System.Text.RegularExpressions.Regex]::Unescape($value)
}

function New-Connection {
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $conn.Open()
    return $conn
}

function Add-Parameter($cmd, [string]$name, $value) {
    $param = $cmd.Parameters.AddWithValue($name, $(if ($null -eq $value) { [DBNull]::Value } else { $value }))
    if ($null -eq $value) {
        $param.Value = [DBNull]::Value
    }
    return $param
}

function Invoke-Query([string]$sql, [hashtable]$params = @{}) {
    $conn = New-Connection
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 180
        foreach ($key in $params.Keys) {
            [void](Add-Parameter $cmd $key $params[$key])
        }
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        return ,$table
    }
    finally {
        $conn.Close()
    }
}

function Invoke-Scalar([string]$sql, [hashtable]$params = @{}) {
    $conn = New-Connection
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 180
        foreach ($key in $params.Keys) {
            [void](Add-Parameter $cmd $key $params[$key])
        }
        $value = $cmd.ExecuteScalar()
        if ($null -eq $value -or $value -eq [DBNull]::Value) { return $null }
        return $value
    }
    finally {
        $conn.Close()
    }
}

function Invoke-NonQuery([string]$sql, [hashtable]$params = @{}) {
    if ($DryRun) {
        Write-Host "[DRYRUN SQL] $sql"
        return 0
    }
    $conn = New-Connection
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 180
        foreach ($key in $params.Keys) {
            [void](Add-Parameter $cmd $key $params[$key])
        }
        return $cmd.ExecuteNonQuery()
    }
    finally {
        $conn.Close()
    }
}

function Normalize-Text($value) {
    if ($null -eq $value -or $value -eq [DBNull]::Value) { return "" }
    $text = [System.Web.HttpUtility]::HtmlDecode([string]$value)
    $xi = [string]([char]0x03be)
    $text = $text.Replace($xi + $xi + "_or_" + $xi + $xi, "or")
    $text = $text.Replace($xi + $xi + "_and_" + $xi + $xi, "and")
    $text = $text.Replace($xi + $xi + "_lt_" + $xi + $xi, "lt")
    $text = $text.Replace($xi + $xi + "_gt_" + $xi + $xi, "gt")
    $text = $text -replace "\\underline\{([^{}]*)\}", '$1'
    $text = $text -replace "\\textit\{([^{}]*)\}", '$1'
    $text = $text -replace "\\[a-zA-Z]+\*?(?:\[[^\]]+\])?\{([^{}]*)\}", '$1'
    $text = $text -replace "\\[a-zA-Z]+\*?", " "
    $text = $text -replace "[{}]", ""
    $text = $text -replace "\(cid:\d+\)", " "
    $text = $text -replace "\s+", " "
    return $text.Trim(" ", ",", ";", ":", "-")
}

function Needs-Cleanup($value) {
    if ($null -eq $value -or $value -eq [DBNull]::Value) { return $false }
    $text = [string]$value
    $xi = [string]([char]0x03be)
    return $text.Contains("&nbsp;") -or $text.Contains($xi + $xi + "_") -or $text.Contains("\textit") -or $text.Contains("\underline")
}

function Sql-SetNullableInt($value) {
    if ($null -eq $value -or $value -eq [DBNull]::Value) { return $null }
    return [int]$value
}

function Ensure-Category([string]$code, [string]$name, [string]$nameEn, [int]$orderId, $parentId) {
    $id = Invoke-Scalar "SELECT TOP 1 id FROM dbo.LiteratureCategory WHERE status<>-1 AND (code=@code OR name=@name) ORDER BY id" @{
        "@code" = $code
        "@name" = $name
    }
    if ($null -eq $id) {
        [void](Invoke-NonQuery "INSERT INTO dbo.LiteratureCategory(parent_id,name,name_en,code,orderid,status,addtime,updatetime) VALUES(@parent_id,@name,@name_en,@code,@orderid,1,GETDATE(),GETDATE())" @{
            "@parent_id" = $parentId
            "@name" = $name
            "@name_en" = $nameEn
            "@code" = $code
            "@orderid" = $orderId
        })
        $id = Invoke-Scalar "SELECT TOP 1 id FROM dbo.LiteratureCategory WHERE status<>-1 AND code=@code ORDER BY id DESC" @{ "@code" = $code }
    }
    else {
        [void](Invoke-NonQuery "UPDATE dbo.LiteratureCategory SET parent_id=@parent_id,name=@name,name_en=@name_en,code=@code,orderid=@orderid,status=1,updatetime=GETDATE() WHERE id=@id" @{
            "@id" = [int]$id
            "@parent_id" = $parentId
            "@name" = $name
            "@name_en" = $nameEn
            "@code" = $code
            "@orderid" = $orderId
        })
    }
    return [int]$id
}

function Get-DuplicateMasterId($remark) {
    $text = Normalize-Text $remark
    $match = [System.Text.RegularExpressions.Regex]::Match($text, "ID:(\d+)")
    if ($match.Success) { return [int]$match.Groups[1].Value }
    $match = [System.Text.RegularExpressions.Regex]::Match($text, "\u6587\u732eID:(\d+)")
    if ($match.Success) { return [int]$match.Groups[1].Value }
    return 0
}

function Get-CategoryForLiterature($row, $categoryIds) {
    $title = Normalize-Text $row["title"]
    $keywords = Normalize-Text $row["keywords"]
    $abstract = Normalize-Text $row["abstract_text"]
    $journal = Normalize-Text $row["journal_name"]
    $conference = Normalize-Text $row["conference_name"]
    $text = ($title + " " + $keywords + " " + $abstract + " " + $journal + " " + $conference)

    if ($text -match "(?i)From Prompts to Printable Models|Additive Manufacturing|3D Printing|Printable Models|Robotics And Automation") {
        return $categoryIds["robotics"]
    }
    if ($text -match "(?i)3DTopia|Pixal3D|Articraft|3D Generation|3D Asset|Text-to-3D|Image-to-3D|Primitive Diffusion|Articulated 3D") {
        return $categoryIds["threeD"]
    }
    if ($text -match "(?i)MonoRelief|Monocular|Depth Estimation|Normal Estimation|Relief Recovery|Computer Vision") {
        return $categoryIds["vision"]
    }
    if ($title -match "(?i)Beyond RAG|Retrieval by Decoupling") {
        return $categoryIds["retrieval"]
    }
    if ($text -match "(?i)Mem0|MIRIX|MemAgent|Agentic Memory|AI Agents|Agentic AI|Long-Context|Large Language Models|LLM|Memory") {
        return $categoryIds["agents"]
    }
    if ($text -match "(?i)Retrieval-Augmented|RAG") {
        return $categoryIds["retrieval"]
    }
    if ($text -match "(?i)\b(NLP|natural language|language model|text generation|machine translation|information extraction|named entity|sentiment analysis|question answering)\b") {
        return $categoryIds["nlp"]
    }
    if ($text -match "(?i)multimodal|multi-modal|cross-modal|vision-language|visual language|CLIP|audio-visual|text-image") {
        return $categoryIds["multimodal"]
    }
    if ($text -match "(?i)machine learning|deep learning|neural network|representation learning|supervised learning|unsupervised learning|self-supervised|classification|prediction") {
        return $categoryIds["aiML"]
    }
    if ($text -match "(?i)data science|statistics|statistical|bayesian|causal inference|data mining|analytics|forecasting|time series|regression") {
        return $categoryIds["dataStats"]
    }
    if ($text -match "(?i)software engineering|program analysis|database|distributed system|cloud computing|operating system|compiler|microservice|devops") {
        return $categoryIds["software"]
    }
    if ($text -match "(?i)human-computer interaction|\bHCI\b|user experience|\bUX\b|visualization|interface|interaction design") {
        return $categoryIds["hci"]
    }
    if ($text -match "(?i)communication|wireless|signal processing|semiconductor|circuit|sensor|internet of things|\bIoT\b|edge computing|network protocol") {
        return $categoryIds["electronics"]
    }
    if ($text -match "(?i)security|privacy|cryptography|encryption|adversarial|attack|defense|malware|vulnerability|blockchain") {
        return $categoryIds["security"]
    }
    if ($text -match "(?i)mathematics|optimization|optimal|theorem|proof|algebra|geometry|topology|numerical method|operations research") {
        return $categoryIds["math"]
    }
    if ($text -match "(?i)physics|astronomy|quantum|particle|cosmology|optics|mechanics|thermodynamics") {
        return $categoryIds["physics"]
    }
    if ($text -match "(?i)chemistry|chemical|material|polymer|catalyst|battery|nanomaterial|molecule|crystal") {
        return $categoryIds["chemMaterials"]
    }
    if ($text -match "(?i)biology|bioinformatics|genomics|protein|cell|gene|biomedical|neuroscience|ecology") {
        return $categoryIds["lifeBio"]
    }
    if ($text -match "(?i)medical|medicine|clinical|health|radiology|diagnosis|patient|disease|MRI|CT|ultrasound|pathology") {
        return $categoryIds["medical"]
    }
    if ($text -match "(?i)environment|climate|energy|geology|earth|carbon|sustainability|renewable|ocean|atmosphere") {
        return $categoryIds["earthEnergy"]
    }
    if ($text -match "(?i)architecture|building information modeling|\bBIM\b|digital twin|urban|construction|city model") {
        return $categoryIds["architectureDT"]
    }
    if ($text -match "(?i)education|learning science|teaching|student|curriculum|pedagogy|MOOC|e-learning") {
        return $categoryIds["education"]
    }
    if ($text -match "(?i)economics|management|business|innovation|finance|marketing|organization|supply chain|strategy") {
        return $categoryIds["management"]
    }
    if ($text -match "(?i)law|policy|ethics|governance|regulation|compliance|copyright|fairness|responsible ai") {
        return $categoryIds["lawEthics"]
    }
    if ($text -match "(?i)linguistics|literature|communication|journalism|media studies|translation studies|discourse") {
        return $categoryIds["languageMedia"]
    }
    if ($text -match "(?i)art|design|digital media|creative|aesthetics|animation|game studies|museum") {
        return $categoryIds["artsDesign"]
    }
    if ($text -match "(?i)psychology|cognitive|behavior|behaviour|emotion|perception|attention|human factors") {
        return $categoryIds["psychCognitive"]
    }
    if ($text -match "(?i)digital humanities|cultural heritage|archive|philology|history|archaeology") {
        return $categoryIds["digitalHumanities"]
    }
    return [int]$row["category_id"]
}

function Update-LiteratureTextIfNeeded($row) {
    $updates = @{}
    foreach ($field in @("title", "keywords", "abstract_text", "institution", "journal_name", "conference_name", "publisher")) {
        if (Needs-Cleanup $row[$field]) {
            $updates[$field] = Normalize-Text $row[$field]
        }
    }
    if ($updates.Count -le 0) { return $false }

    $setParts = @()
    $params = @{ "@id" = [int]$row["id"] }
    foreach ($key in $updates.Keys) {
        $paramName = "@" + $key
        $setParts += "$key=$paramName"
        $params[$paramName] = $updates[$key]
    }
    $sql = "UPDATE dbo.Literature SET " + ($setParts -join ",") + ",updatetime=GETDATE() WHERE id=@id"
    [void](Invoke-NonQuery $sql $params)
    return $true
}

function Sync-DuplicateMetadata($row) {
    $masterId = Get-DuplicateMasterId $row["remark"]
    if ($masterId -le 0) { return $false }

    $masterRows = Invoke-Query "SELECT TOP 1 * FROM dbo.Literature WHERE id=@id AND status<>-1" @{ "@id" = $masterId }
    if ($masterRows.Rows.Count -le 0) { return $false }

    $m = $masterRows.Rows[0]
    [void](Invoke-NonQuery @"
UPDATE dbo.Literature
SET title=@title,
    subtitle=@subtitle,
    institution=@institution,
    doi=@doi,
    keywords=@keywords,
    abstract_text=@abstract_text,
    source_type=@source_type,
    language=@language,
    publish_year=@publish_year,
    journal_name=@journal_name,
    conference_name=@conference_name,
    publisher=@publisher,
    volume=@volume,
    issue=@issue,
    pages=@pages,
    category_id=@category_id,
    cover_pic=@cover_pic,
    external_url=@external_url,
    source_db=@source_db,
    download_points=@download_points,
    updatetime=GETDATE()
WHERE id=@id
"@ @{
        "@id" = [int]$row["id"]
        "@title" = Normalize-Text $m["title"]
        "@subtitle" = Normalize-Text $m["subtitle"]
        "@institution" = Normalize-Text $m["institution"]
        "@doi" = Normalize-Text $m["doi"]
        "@keywords" = Normalize-Text $m["keywords"]
        "@abstract_text" = Normalize-Text $m["abstract_text"]
        "@source_type" = Normalize-Text $m["source_type"]
        "@language" = Normalize-Text $m["language"]
        "@publish_year" = Sql-SetNullableInt $m["publish_year"]
        "@journal_name" = Normalize-Text $m["journal_name"]
        "@conference_name" = Normalize-Text $m["conference_name"]
        "@publisher" = Normalize-Text $m["publisher"]
        "@volume" = Normalize-Text $m["volume"]
        "@issue" = Normalize-Text $m["issue"]
        "@pages" = Normalize-Text $m["pages"]
        "@category_id" = [int]$m["category_id"]
        "@cover_pic" = Normalize-Text $m["cover_pic"]
        "@external_url" = Normalize-Text $m["external_url"]
        "@source_db" = Normalize-Text $m["source_db"]
        "@download_points" = Sql-SetNullableInt $m["download_points"]
    })
    return $true
}

if (-not $DryRun) {
    Invoke-NonQuery "SELECT * INTO dbo.LiteratureCategoryBackup_$Stamp FROM dbo.LiteratureCategory; SELECT id,title,keywords,category_id,status,remark,updatetime INTO dbo.LiteratureCategoryAssignmentBackup_$Stamp FROM dbo.Literature WHERE status IN (0,1,3,4);"
}

$uncategorizedName = "Uncategorized"
[void](Invoke-NonQuery "UPDATE dbo.LiteratureCategory SET name=@name,name_en=@name_en,code=@code,orderid=999,status=0,parent_id=NULL,updatetime=GETDATE() WHERE id=0" @{
    "@name" = $uncategorizedName
    "@name_en" = "Uncategorized"
    "@code" = "uncategorized"
})

[void](Invoke-NonQuery "UPDATE dbo.LiteratureCategory SET name=@name,name_en=@name_en,code=@code,orderid=900,status=-1,parent_id=NULL,updatetime=GETDATE() WHERE id=1" @{
    "@name" = U("\u4eba\u6587\u793e\u79d1")
    "@name_en" = "Humanities and Social Sciences"
    "@code" = "humanities-social-sciences"
})

[void](Invoke-NonQuery "UPDATE dbo.LiteratureCategory SET name=@name,name_en=@name_en,code=@code,orderid=910,status=-1,parent_id=NULL,updatetime=GETDATE() WHERE id=2" @{
    "@name" = U("\u81ea\u7136\u79d1\u5b66")
    "@name_en" = "Natural Sciences"
    "@code" = "natural-sciences"
})

[void](Invoke-NonQuery "UPDATE dbo.LiteratureCategory SET parent_id=NULL,updatetime=GETDATE() WHERE parent_id IN (1,2)")

$naturalId = $null
$categoryIds = @{}
$categoryIds["threeD"] = Ensure-Category "3d-generation-graphics" (U("\u4e09\u7ef4\u751f\u6210\u4e0e\u56fe\u5f62\u5b66")) "3D Generation and Graphics" 30 $naturalId
$categoryIds["agents"] = Ensure-Category "ai-agents-memory" (U("\u667a\u80fd\u4f53\u4e0e\u8bb0\u5fc6\u7cfb\u7edf")) "AI Agents and Memory Systems" 40 $naturalId
$categoryIds["retrieval"] = Ensure-Category "information-retrieval-knowledge" (U("\u4fe1\u606f\u68c0\u7d22\u4e0e\u77e5\u8bc6\u7ba1\u7406")) "Information Retrieval and Knowledge Management" 50 $naturalId
$categoryIds["vision"] = Ensure-Category "computer-vision" (U("\u8ba1\u7b97\u673a\u89c6\u89c9")) "Computer Vision" 60 $naturalId
$categoryIds["robotics"] = Ensure-Category "robotics-intelligent-manufacturing" (U("\u673a\u5668\u4eba\u4e0e\u667a\u80fd\u5236\u9020")) "Robotics and Intelligent Manufacturing" 70 $naturalId
$categoryIds["aiML"] = Ensure-Category "ai-machine-learning" (U("\u4eba\u5de5\u667a\u80fd\u4e0e\u673a\u5668\u5b66\u4e60")) "Artificial Intelligence and Machine Learning" 80 $naturalId
$categoryIds["nlp"] = Ensure-Category "natural-language-processing" (U("\u81ea\u7136\u8bed\u8a00\u5904\u7406")) "Natural Language Processing" 90 $naturalId
$categoryIds["multimodal"] = Ensure-Category "multimodal-learning" (U("\u591a\u6a21\u6001\u5b66\u4e60")) "Multimodal Learning" 100 $naturalId
$categoryIds["dataStats"] = Ensure-Category "data-science-statistics" (U("\u6570\u636e\u79d1\u5b66\u4e0e\u7edf\u8ba1")) "Data Science and Statistics" 110 $naturalId
$categoryIds["software"] = Ensure-Category "software-engineering-systems" (U("\u8f6f\u4ef6\u5de5\u7a0b\u4e0e\u7cfb\u7edf")) "Software Engineering and Systems" 120 $naturalId
$categoryIds["hci"] = Ensure-Category "hci-visualization" (U("\u4eba\u673a\u4ea4\u4e92\u4e0e\u53ef\u89c6\u5316")) "Human-Computer Interaction and Visualization" 130 $naturalId
$categoryIds["electronics"] = Ensure-Category "electronics-communications" (U("\u7535\u5b50\u4fe1\u606f\u4e0e\u901a\u4fe1")) "Electronics, Information and Communications" 140 $naturalId
$categoryIds["security"] = Ensure-Category "cybersecurity-privacy" (U("\u7f51\u7edc\u5b89\u5168\u4e0e\u9690\u79c1")) "Cybersecurity and Privacy" 150 $naturalId
$categoryIds["math"] = Ensure-Category "mathematics-optimization" (U("\u6570\u5b66\u4e0e\u4f18\u5316")) "Mathematics and Optimization" 160 $naturalId
$categoryIds["physics"] = Ensure-Category "physics-astronomy" (U("\u7269\u7406\u4e0e\u5929\u6587\u5b66")) "Physics and Astronomy" 170 $naturalId
$categoryIds["chemMaterials"] = Ensure-Category "chemistry-materials" (U("\u5316\u5b66\u4e0e\u6750\u6599\u79d1\u5b66")) "Chemistry and Materials Science" 180 $naturalId
$categoryIds["lifeBio"] = Ensure-Category "life-sciences-bioinformatics" (U("\u751f\u547d\u79d1\u5b66\u4e0e\u751f\u7269\u4fe1\u606f")) "Life Sciences and Bioinformatics" 190 $naturalId
$categoryIds["medical"] = Ensure-Category "medical-imaging-health-informatics" (U("\u533b\u5b66\u5f71\u50cf\u4e0e\u5065\u5eb7\u4fe1\u606f")) "Medical Imaging and Health Informatics" 200 $naturalId
$categoryIds["earthEnergy"] = Ensure-Category "earth-environment-energy" (U("\u5730\u7403\u73af\u5883\u4e0e\u80fd\u6e90")) "Earth, Environment and Energy" 210 $naturalId
$categoryIds["architectureDT"] = Ensure-Category "architecture-digital-twins" (U("\u5efa\u7b51\u4e0e\u6570\u5b57\u5b6a\u751f")) "Architecture and Digital Twins" 220 $naturalId

$humanId = $null
$categoryIds["education"] = Ensure-Category "education-learning-sciences" (U("\u6559\u80b2\u4e0e\u5b66\u4e60\u79d1\u5b66")) "Education and Learning Sciences" 300 $humanId
$categoryIds["management"] = Ensure-Category "economics-management-innovation" (U("\u7ecf\u6d4e\u7ba1\u7406\u4e0e\u521b\u65b0")) "Economics, Management and Innovation" 310 $humanId
$categoryIds["lawEthics"] = Ensure-Category "law-policy-ethics" (U("\u6cd5\u5b66\u653f\u7b56\u4e0e\u4f26\u7406")) "Law, Policy and Ethics" 320 $humanId
$categoryIds["languageMedia"] = Ensure-Category "language-literature-communication" (U("\u8bed\u8a00\u6587\u5b66\u4e0e\u4f20\u64ad")) "Language, Literature and Communication" 330 $humanId
$categoryIds["artsDesign"] = Ensure-Category "art-design-digital-media" (U("\u827a\u672f\u8bbe\u8ba1\u4e0e\u6570\u5b57\u5a92\u4f53")) "Art, Design and Digital Media" 340 $humanId
$categoryIds["psychCognitive"] = Ensure-Category "psychology-cognitive-science" (U("\u5fc3\u7406\u5b66\u4e0e\u8ba4\u77e5\u79d1\u5b66")) "Psychology and Cognitive Science" 350 $humanId
$categoryIds["digitalHumanities"] = Ensure-Category "digital-humanities-cultural-heritage" (U("\u6570\u5b57\u4eba\u6587\u4e0e\u6587\u5316\u9057\u4ea7")) "Digital Humanities and Cultural Heritage" 360 $humanId

$syncedDuplicates = 0
$cleanedRows = 0
$changedRows = @()

$rows = Invoke-Query "SELECT * FROM dbo.Literature WHERE status IN (0,1,3,4) ORDER BY id"
foreach ($row in $rows.Rows) {
    if ([int]$row["status"] -eq 3) {
        if (Sync-DuplicateMetadata $row) {
            $syncedDuplicates++
        }
    }
}

$rows = Invoke-Query "SELECT * FROM dbo.Literature WHERE status IN (0,1,3,4) ORDER BY id"
foreach ($row in $rows.Rows) {
    if (Update-LiteratureTextIfNeeded $row) {
        $cleanedRows++
    }
}

$rows = Invoke-Query "SELECT * FROM dbo.Literature WHERE status IN (0,1,3,4) ORDER BY id"
foreach ($row in $rows.Rows) {
    $oldCategoryId = [int]$row["category_id"]
    $newCategoryId = Get-CategoryForLiterature $row $categoryIds
    if ($newCategoryId -ne $oldCategoryId) {
        [void](Invoke-NonQuery "UPDATE dbo.Literature SET category_id=@category_id,updatetime=GETDATE() WHERE id=@id" @{
            "@category_id" = $newCategoryId
            "@id" = [int]$row["id"]
        })
    }
    $changedRows += [pscustomobject]@{
        id = [int]$row["id"]
        status = [int]$row["status"]
        title = Normalize-Text $row["title"]
        old_category_id = $oldCategoryId
        new_category_id = $newCategoryId
    }
}

$summaryRows = Invoke-Query @"
SELECT c.id,c.name,c.name_en,c.code,c.orderid,c.status,COUNT(l.id) AS literature_count
FROM dbo.LiteratureCategory c
LEFT JOIN dbo.Literature l ON l.category_id=c.id AND l.status IN (0,1,3,4)
WHERE c.status<>-1
GROUP BY c.id,c.name,c.name_en,c.code,c.orderid,c.status
ORDER BY c.orderid,c.id
"@

$report = [pscustomobject]@{
    stamp = $Stamp
    dry_run = [bool]$DryRun
    synced_duplicate_count = $syncedDuplicates
    cleaned_literature_count = $cleanedRows
    category_ids = $categoryIds
    assignments = $changedRows
    category_summary = @($summaryRows.Rows | ForEach-Object {
        [pscustomobject]@{
            id = [int]$_["id"]
            name = [string]$_["name"]
            code = [string]$_["code"]
            status = [int]$_["status"]
            literature_count = [int]$_["literature_count"]
        }
    })
}

$reportPath = Join-Path $PSScriptRoot ("reclassify_literature_categories_" + $Stamp + ".json")
$report | ConvertTo-Json -Depth 6 | Set-Content -Path $reportPath -Encoding UTF8
Write-Host "Report: $reportPath"
