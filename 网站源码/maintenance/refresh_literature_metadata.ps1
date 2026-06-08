param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot),
    [string]$ConnectionString = "Data Source=(local)\SQLEXPRESS;Initial Catalog=manage_db;User ID=sa;Password=123456;Encrypt=False;TrustServerCertificate=True;",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$PythonExe = if (Test-Path "E:\tools\python.exe") { "E:\tools\python.exe" } else { "python" }
$UploadRoot = Join-Path $Root "Web\A_UpLoad\upload_file"
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

function Invoke-Query($sql) {
    $conn = New-Connection
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 180
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        return ,$table
    }
    finally {
        $conn.Close()
    }
}

function Invoke-NonQuery($sql) {
    if ($DryRun) {
        Write-Host "[DRYRUN SQL] $sql"
        return 0
    }
    $conn = New-Connection
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 180
        return $cmd.ExecuteNonQuery()
    }
    finally {
        $conn.Close()
    }
}

function Add-Parameter($cmd, [string]$name, $value) {
    $param = $cmd.Parameters.AddWithValue($name, $(if ($null -eq $value) { [DBNull]::Value } else { $value }))
    if ($null -eq $value) {
        $param.Value = [DBNull]::Value
    }
    return $param
}

function Invoke-ParameterizedNonQuery([string]$sql, [hashtable]$params) {
    if ($DryRun) {
        Write-Host "[DRYRUN PARAM SQL] $sql"
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

function Invoke-ParameterizedScalar([string]$sql, [hashtable]$params) {
    $conn = New-Connection
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 180
        foreach ($key in $params.Keys) {
            [void](Add-Parameter $cmd $key $params[$key])
        }
        $value = $cmd.ExecuteScalar()
        if ($value -eq [DBNull]::Value) { return $null }
        return $value
    }
    finally {
        $conn.Close()
    }
}

function Normalize-Text($value) {
    if ($null -eq $value) { return "" }
    $text = [System.Web.HttpUtility]::HtmlDecode([string]$value)
    $text = $text -replace "\\underline\{([^{}]*)\}", '$1'
    $text = $text -replace "\\textit\{([^{}]*)\}", '$1'
    $text = $text -replace "\\[a-zA-Z]+\*?(?:\[[^\]]+\])?\{([^{}]*)\}", '$1'
    $text = $text -replace "\\[a-zA-Z]+\*?", " "
    $text = $text -replace "[{}]", ""
    $xi = [string]([char]0x03be)
    $text = $text.Replace($xi + $xi + "_or_" + $xi + $xi, "or")
    $text = $text.Replace($xi + $xi + "_and_" + $xi + $xi, "and")
    $text = $text -replace "\(cid:\d+\)", " "
    $text = $text -replace "\s+", " "
    return $text.Trim(" ", ",", ";", ":", "-")
}

function Truncate-Text([string]$value, [int]$max) {
    $text = Normalize-Text $value
    if ($text.Length -le $max) { return $text }
    return $text.Substring(0, $max).Trim()
}

function Is-WeakTitle([string]$title) {
    $text = Normalize-Text $title
    if ([string]::IsNullOrWhiteSpace($text)) { return $true }
    if ($text -match "\.pdf$") { return $true }
    if ($text.Contains(([string]([char]0x03be)) + ([string]([char]0x03be) + "_")) -or $text -match "&nbsp;") { return $true }
    if ($text.Length -lt 20) { return $true }
    if ($text -match "^(JOURNAL OF LATEX CLASS FILES|JOURNALOF LATEXCLASS FILES)") { return $true }
    return $false
}

function Clean-Keyword([string]$value) {
    $text = Normalize-Text $value
    $text = $text -replace "^[\u2014\u2013\-]+\s*", ""
    $text = $text -replace "\.$", ""
    $text = $text.Trim()
    if ($text.Length -lt 2) { return "" }
    return $text
}

function Add-Unique([System.Collections.Generic.List[string]]$list, [string]$value) {
    $text = Clean-Keyword $value
    if ([string]::IsNullOrWhiteSpace($text)) { return }
    foreach ($item in $list) {
        if ($item.Equals($text, [System.StringComparison]::OrdinalIgnoreCase)) { return }
    }
    $list.Add($text)
}

function Extract-ArxivId($row, $parsed) {
    $sources = @(
        $row["doi"], $row["title"], $row["file_name"], $row["file_path"],
        $(if ($parsed) { $parsed.doi } else { "" }),
        $(if ($parsed) { $parsed.title } else { "" })
    )
    foreach ($source in $sources) {
        $text = Normalize-Text $source
        if ($text -match "(?i)arxiv[./:]?(\d{4}\.\d{4,5})") { return $Matches[1] }
        if ($text -match "\b(\d{4}\.\d{4,5})v?\d*\b") { return $Matches[1] }
    }
    $title = Normalize-Text $(if ($parsed -and $parsed.title) { $parsed.title } else { $row["title"] })
    if ($title -match "3DTopia-XL") { return "2409.12957" }
    return ""
}

function Convert-ArxivAuthor([string]$name) {
    $text = Normalize-Text $name
    if ($text -match "^([^,]+),\s*(.+)$") {
        return (($Matches[2] + " " + $Matches[1]) -replace "\s+", " ").Trim()
    }
    return $text
}

function Get-ArxivMetadata([string]$id) {
    if ([string]::IsNullOrWhiteSpace($id)) { return $null }
    $url = "https://arxiv.org/abs/$id"
    try {
        $response = Invoke-WebRequest -UseBasicParsing -Uri $url -Headers @{ "User-Agent" = "AcademicPortalMetadataRescan/1.0" } -TimeoutSec 30
        $html = $response.Content
        $title = ""
        $titleMatch = [regex]::Match($html, '<meta name="citation_title" content="(.*?)"', "Singleline")
        if ($titleMatch.Success) {
            $title = [System.Web.HttpUtility]::HtmlDecode($titleMatch.Groups[1].Value)
        }
        else {
            $h1 = [regex]::Match($html, '<h1 class="title mathjax">\s*<span[^>]*>Title:</span>\s*(.*?)\s*</h1>', "Singleline")
            if ($h1.Success) {
                $title = [regex]::Replace($h1.Groups[1].Value, "<.*?>", "")
                $title = [System.Web.HttpUtility]::HtmlDecode($title)
            }
        }

        $authors = New-Object System.Collections.Generic.List[string]
        foreach ($match in [regex]::Matches($html, '<meta name="citation_author" content="(.*?)"', "Singleline")) {
            Add-Unique $authors (Convert-ArxivAuthor $match.Groups[1].Value)
        }

        $published = ""
        $dateMatch = [regex]::Match($html, '<meta name="citation_date" content="(.*?)"', "Singleline")
        if ($dateMatch.Success) { $published = [System.Web.HttpUtility]::HtmlDecode($dateMatch.Groups[1].Value) }

        $abstract = ""
        $abstractMatch = [regex]::Match($html, '<blockquote class="abstract mathjax">\s*<span[^>]*>Abstract:</span>\s*(.*?)\s*</blockquote>', "Singleline")
        if ($abstractMatch.Success) {
            $abstract = [regex]::Replace($abstractMatch.Groups[1].Value, "<.*?>", " ")
            $abstract = [System.Web.HttpUtility]::HtmlDecode($abstract)
            $abstract = Normalize-Text $abstract
        }

        return [pscustomobject]@{
            id = $id
            title = Normalize-Text $title
            authors = @($authors)
            abstract = $abstract
            year = $(if ($published -match "^(\d{4})") { [int]$Matches[1] } else { $null })
            doi = "10.48550/arxiv.$id"
            url = $url
        }
    }
    catch {
        Write-Warning "arXiv metadata fetch failed for ${id}: $($_.Exception.Message)"
        return $null
    }
}

function Parse-Pdf([string]$path) {
    if (!(Test-Path $path)) { return $null }
    $script = "import json,sys; from pdf_parser import extract_paper_info; data=extract_paper_info(sys.argv[1]) or {}; print(json.dumps(data, ensure_ascii=False))"
    $env:PYTHONIOENCODING = "utf-8"
    $env:PYTHONUTF8 = "1"
    $json = & $PythonExe -c $script $path
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($json)) { return $null }
    return ($json | ConvertFrom-Json)
}

function Clean-Institutions($items) {
    $values = New-Object System.Collections.Generic.List[string]
    foreach ($item in @($items)) {
        $text = Normalize-Text $item
        if ([string]::IsNullOrWhiteSpace($text)) { continue }
        $text = $text -replace "^[A-Z][A-Z\-\s]+,\s*", ""
        $text = $text -replace "^YUXIN CHEN,\s*", ""
        $text = $text -replace "^WENBO HU,\s*", ""
        $text = $text -replace "^MENG-HAO GUO,\s*", ""
        $text = $text -replace "\s+", " "
        if ($text -match "(?i)abstract|keywords|introduction|preprint") { continue }
        Add-Unique $values $text
    }
    if ($values.Count -eq 0) { return "" }
    return (($values | Select-Object -First 6) -join (U "\uFF1B"))
}

function Get-DerivedTerms([string]$title, [string]$abstract, [string]$venue, $parsedKeywords) {
    $keywords = New-Object System.Collections.Generic.List[string]
    $tags = New-Object System.Collections.Generic.List[string]
    foreach ($kw in @($parsedKeywords)) { Add-Unique $keywords $kw }
    $text = ((Normalize-Text $title) + " " + (Normalize-Text $abstract) + " " + (Normalize-Text $venue)).ToLowerInvariant()

    $rules = @(
        @{ P="\b(large language models?|llms?|language models?)\b"; K=@("Large Language Models","LLM"); T=@((U "\u5927\u8BED\u8A00\u6A21\u578B"),(U "\u4EBA\u5DE5\u667A\u80FD")) },
        @{ P="\b(agentic|ai agents?|agents?|multi-agent)\b"; K=@("AI Agents","Agentic AI"); T=@((U "\u667A\u80FD\u4F53"),(U "\u4EBA\u5DE5\u667A\u80FD")) },
        @{ P="\b(memory|long-term|short-term|long-context|retrieval)\b"; K=@("Memory","Long-Context","Retrieval"); T=@((U "\u8BB0\u5FC6\u673A\u5236"),(U "\u957F\u4E0A\u4E0B\u6587")) },
        @{ P="\brag\b|retrieval augmented"; K=@("RAG","Retrieval-Augmented Generation"); T=@("RAG") },
        @{ P="\b3d\b|three-dimensional|asset generation|3d generation"; K=@("3D Generation","3D Asset Generation"); T=@((U "3D\u751F\u6210"),"AI4Science") },
        @{ P="\b(articulated|joint|urdf)\b"; K=@("Articulated Assets","Articulated 3D Objects"); T=@((U "\u53EF\u52A8\u5173\u8282\u8D44\u4EA7"),(U "\u673A\u5668\u4EBA")) },
        @{ P="\b(diffusion|primitive)\b"; K=@("Diffusion Models","Primitive Diffusion"); T=@((U "\u6269\u6563\u6A21\u578B"),(U "\u56FE\u5143\u5EFA\u6A21")) },
        @{ P="image-to|pixel-aligned|from images|image features"; K=@("Image-to-3D","Pixel-Aligned Generation"); T=@((U "\u56FE\u50CF\u52303D"),(U "\u8BA1\u7B97\u673A\u89C6\u89C9")) },
        @{ P="text-to-3d|prompts?|printable|3d print|support structures|additive manufacturing"; K=@("Text-to-3D","3D Printing","Additive Manufacturing"); T=@((U "\u6587\u672C\u52303D"),(U "3D\u6253\u5370")) },
        @{ P="\b(relief|depth|normal|monocular)\b"; K=@("Depth Estimation","Normal Estimation","Relief Recovery"); T=@((U "\u4E09\u7EF4\u91CD\u5EFA"),(U "\u8BA1\u7B97\u673A\u89C6\u89C9")) },
        @{ P="\b(clip|ulip|uni3d|evaluation)\b"; K=@("Evaluation Metrics","CLIP","ULIP"); T=@((U "\u8BC4\u4F30\u6307\u6807"),(U "\u591A\u6A21\u6001")) },
        @{ P="\b(dataset|benchmark)\b"; K=@("Dataset","Benchmark"); T=@((U "\u6570\u636E\u96C6")) },
        @{ P="\b(cvpr|computer vision)\b"; K=@("Computer Vision"); T=@((U "\u8BA1\u7B97\u673A\u89C6\u89C9")) },
        @{ P="reinforcement learning|\brl\b"; K=@("Reinforcement Learning"); T=@((U "\u5F3A\u5316\u5B66\u4E60")) }
    )

    foreach ($rule in $rules) {
        if ($text -match $rule.P) {
            foreach ($kw in $rule.K) { Add-Unique $keywords $kw }
            foreach ($tag in $rule.T) { Add-Unique $tags $tag }
        }
    }

    if ($tags.Count -eq 0 -and $keywords.Count -gt 0) { Add-Unique $tags (U "\u4EBA\u5DE5\u667A\u80FD") }
    return [pscustomobject]@{
        keywords = @(($keywords | Select-Object -First 10))
        tags = @(($tags | Select-Object -First 6))
    }
}

function Ensure-Tag([string]$name) {
    $tagName = Normalize-Text $name
    if ([string]::IsNullOrWhiteSpace($tagName)) { return 0 }
    $id = Invoke-ParameterizedScalar "SELECT TOP 1 id FROM dbo.LiteratureTag WHERE name=@name AND status<>-1" @{ "@name" = $tagName }
    if ($id) { return [int]$id }
    [void](Invoke-ParameterizedNonQuery "INSERT INTO dbo.LiteratureTag(name,orderid,status,addtime) VALUES(@name,0,1,GETDATE())" @{ "@name" = $tagName })
    return [int](Invoke-ParameterizedScalar "SELECT TOP 1 id FROM dbo.LiteratureTag WHERE name=@name AND status<>-1 ORDER BY id DESC" @{ "@name" = $tagName })
}

function Ensure-Author([string]$name, [string]$institution) {
    $authorName = Normalize-Text $name
    if ([string]::IsNullOrWhiteSpace($authorName)) { return 0 }
    $id = Invoke-ParameterizedScalar "SELECT TOP 1 id FROM dbo.Author WHERE name_cn=@name AND status<>-1" @{ "@name" = $authorName }
    if ($id) { return [int]$id }
    [void](Invoke-ParameterizedNonQuery "INSERT INTO dbo.Author(name_cn,name_en,institution,status,addtime) VALUES(@name,@name_en,@institution,1,GETDATE())" @{
        "@name" = $authorName
        "@name_en" = $authorName
        "@institution" = $(if ([string]::IsNullOrWhiteSpace($institution)) { $null } else { Truncate-Text $institution 300 })
    })
    return [int](Invoke-ParameterizedScalar "SELECT TOP 1 id FROM dbo.Author WHERE name_cn=@name AND status<>-1 ORDER BY id DESC" @{ "@name" = $authorName })
}

function Set-AuthorMap([int]$literatureId, $authors, [string]$institution) {
    if (!$authors -or @($authors).Count -eq 0) { return 0 }
    [void](Invoke-ParameterizedNonQuery "DELETE FROM dbo.LiteratureAuthorMap WHERE literature_id=@id" @{ "@id" = $literatureId })
    $order = 1
    foreach ($author in @($authors | Select-Object -First 20)) {
        $authorId = Ensure-Author $author $institution
        if ($authorId -le 0) { continue }
        [void](Invoke-ParameterizedNonQuery "IF NOT EXISTS(SELECT 1 FROM dbo.LiteratureAuthorMap WHERE literature_id=@lit AND author_id=@author) INSERT INTO dbo.LiteratureAuthorMap(literature_id,author_id,author_order,is_corresponding,addtime) VALUES(@lit,@author,@ord,0,GETDATE())" @{
            "@lit" = $literatureId
            "@author" = $authorId
            "@ord" = $order
        })
        $order++
    }
    return ($order - 1)
}

function Add-Tags([int]$literatureId, $tags) {
    $count = 0
    [void](Invoke-ParameterizedNonQuery "DELETE FROM dbo.LiteratureTagMap WHERE literature_id=@lit" @{ "@lit" = $literatureId })
    foreach ($tag in @($tags)) {
        $tagId = Ensure-Tag $tag
        if ($tagId -le 0) { continue }
        $changed = Invoke-ParameterizedNonQuery "IF NOT EXISTS(SELECT 1 FROM dbo.LiteratureTagMap WHERE literature_id=@lit AND tag_id=@tag) INSERT INTO dbo.LiteratureTagMap(literature_id,tag_id,addtime) VALUES(@lit,@tag,GETDATE())" @{
            "@lit" = $literatureId
            "@tag" = $tagId
        }
        if ($changed -gt 0) { $count++ }
    }
    return $count
}

function Ensure-Category([string]$code, [string]$name, [string]$nameEn, [int]$orderId, $parentId) {
    $id = Invoke-ParameterizedScalar "SELECT TOP 1 id FROM dbo.LiteratureCategory WHERE status<>-1 AND (code=@code OR name=@name) ORDER BY id" @{
        "@code" = $code
        "@name" = $name
    }
    if ($id) {
        [void](Invoke-ParameterizedNonQuery "UPDATE dbo.LiteratureCategory SET parent_id=@parent_id,name=@name,name_en=@name_en,code=@code,orderid=@orderid,status=1,updatetime=GETDATE() WHERE id=@id" @{
            "@id" = [int]$id
            "@parent_id" = $parentId
            "@name" = $name
            "@name_en" = $nameEn
            "@code" = $code
            "@orderid" = $orderId
        })
        return [int]$id
    }

    [void](Invoke-ParameterizedNonQuery "INSERT INTO dbo.LiteratureCategory(parent_id,name,name_en,code,orderid,status,addtime,updatetime) VALUES(@parent_id,@name,@name_en,@code,@orderid,1,GETDATE(),GETDATE())" @{
        "@parent_id" = $parentId
        "@name" = $name
        "@name_en" = $nameEn
        "@code" = $code
        "@orderid" = $orderId
    })
    return [int](Invoke-ParameterizedScalar "SELECT TOP 1 id FROM dbo.LiteratureCategory WHERE status<>-1 AND code=@code ORDER BY id DESC" @{ "@code" = $code })
}

function Initialize-SubjectCategories {
    [void](Invoke-ParameterizedNonQuery "UPDATE dbo.LiteratureCategory SET status=-1,parent_id=NULL,updatetime=GETDATE() WHERE id IN (1,2); UPDATE dbo.LiteratureCategory SET parent_id=NULL,updatetime=GETDATE() WHERE parent_id IN (1,2)" @{})
    $naturalId = $null

    $ids = @{}
    $ids["threeD"] = Ensure-Category "3d-generation-graphics" (U "\u4E09\u7EF4\u751F\u6210\u4E0E\u56FE\u5F62\u5B66") "3D Generation and Graphics" 30 $naturalId
    $ids["agents"] = Ensure-Category "ai-agents-memory" (U "\u667A\u80FD\u4F53\u4E0E\u8BB0\u5FC6\u7CFB\u7EDF") "AI Agents and Memory Systems" 40 $naturalId
    $ids["retrieval"] = Ensure-Category "information-retrieval-knowledge" (U "\u4FE1\u606F\u68C0\u7D22\u4E0E\u77E5\u8BC6\u7BA1\u7406") "Information Retrieval and Knowledge Management" 50 $naturalId
    $ids["vision"] = Ensure-Category "computer-vision" (U "\u8BA1\u7B97\u673A\u89C6\u89C9") "Computer Vision" 60 $naturalId
    $ids["robotics"] = Ensure-Category "robotics-intelligent-manufacturing" (U "\u673A\u5668\u4EBA\u4E0E\u667A\u80FD\u5236\u9020") "Robotics and Intelligent Manufacturing" 70 $naturalId
    $ids["aiML"] = Ensure-Category "ai-machine-learning" (U "\u4EBA\u5DE5\u667A\u80FD\u4E0E\u673A\u5668\u5B66\u4E60") "Artificial Intelligence and Machine Learning" 80 $naturalId
    $ids["nlp"] = Ensure-Category "natural-language-processing" (U "\u81EA\u7136\u8BED\u8A00\u5904\u7406") "Natural Language Processing" 90 $naturalId
    $ids["multimodal"] = Ensure-Category "multimodal-learning" (U "\u591A\u6A21\u6001\u5B66\u4E60") "Multimodal Learning" 100 $naturalId
    $ids["dataStats"] = Ensure-Category "data-science-statistics" (U "\u6570\u636E\u79D1\u5B66\u4E0E\u7EDF\u8BA1") "Data Science and Statistics" 110 $naturalId
    $ids["software"] = Ensure-Category "software-engineering-systems" (U "\u8F6F\u4EF6\u5DE5\u7A0B\u4E0E\u7CFB\u7EDF") "Software Engineering and Systems" 120 $naturalId
    $ids["hci"] = Ensure-Category "hci-visualization" (U "\u4EBA\u673A\u4EA4\u4E92\u4E0E\u53EF\u89C6\u5316") "Human-Computer Interaction and Visualization" 130 $naturalId
    $ids["electronics"] = Ensure-Category "electronics-communications" (U "\u7535\u5B50\u4FE1\u606F\u4E0E\u901A\u4FE1") "Electronics, Information and Communications" 140 $naturalId
    $ids["security"] = Ensure-Category "cybersecurity-privacy" (U "\u7F51\u7EDC\u5B89\u5168\u4E0E\u9690\u79C1") "Cybersecurity and Privacy" 150 $naturalId
    $ids["math"] = Ensure-Category "mathematics-optimization" (U "\u6570\u5B66\u4E0E\u4F18\u5316") "Mathematics and Optimization" 160 $naturalId
    $ids["physics"] = Ensure-Category "physics-astronomy" (U "\u7269\u7406\u4E0E\u5929\u6587\u5B66") "Physics and Astronomy" 170 $naturalId
    $ids["chemMaterials"] = Ensure-Category "chemistry-materials" (U "\u5316\u5B66\u4E0E\u6750\u6599\u79D1\u5B66") "Chemistry and Materials Science" 180 $naturalId
    $ids["lifeBio"] = Ensure-Category "life-sciences-bioinformatics" (U "\u751F\u547D\u79D1\u5B66\u4E0E\u751F\u7269\u4FE1\u606F") "Life Sciences and Bioinformatics" 190 $naturalId
    $ids["medical"] = Ensure-Category "medical-imaging-health-informatics" (U "\u533B\u5B66\u5F71\u50CF\u4E0E\u5065\u5EB7\u4FE1\u606F") "Medical Imaging and Health Informatics" 200 $naturalId
    $ids["earthEnergy"] = Ensure-Category "earth-environment-energy" (U "\u5730\u7403\u73AF\u5883\u4E0E\u80FD\u6E90") "Earth, Environment and Energy" 210 $naturalId
    $ids["architectureDT"] = Ensure-Category "architecture-digital-twins" (U "\u5EFA\u7B51\u4E0E\u6570\u5B57\u5B6A\u751F") "Architecture and Digital Twins" 220 $naturalId

    $humanId = $null
    $ids["education"] = Ensure-Category "education-learning-sciences" (U "\u6559\u80B2\u4E0E\u5B66\u4E60\u79D1\u5B66") "Education and Learning Sciences" 300 $humanId
    $ids["management"] = Ensure-Category "economics-management-innovation" (U "\u7ECF\u6D4E\u7BA1\u7406\u4E0E\u521B\u65B0") "Economics, Management and Innovation" 310 $humanId
    $ids["lawEthics"] = Ensure-Category "law-policy-ethics" (U "\u6CD5\u5B66\u653F\u7B56\u4E0E\u4F26\u7406") "Law, Policy and Ethics" 320 $humanId
    $ids["languageMedia"] = Ensure-Category "language-literature-communication" (U "\u8BED\u8A00\u6587\u5B66\u4E0E\u4F20\u64AD") "Language, Literature and Communication" 330 $humanId
    $ids["artsDesign"] = Ensure-Category "art-design-digital-media" (U "\u827A\u672F\u8BBE\u8BA1\u4E0E\u6570\u5B57\u5A92\u4F53") "Art, Design and Digital Media" 340 $humanId
    $ids["psychCognitive"] = Ensure-Category "psychology-cognitive-science" (U "\u5FC3\u7406\u5B66\u4E0E\u8BA4\u77E5\u79D1\u5B66") "Psychology and Cognitive Science" 350 $humanId
    $ids["digitalHumanities"] = Ensure-Category "digital-humanities-cultural-heritage" (U "\u6570\u5B57\u4EBA\u6587\u4E0E\u6587\u5316\u9057\u4EA7") "Digital Humanities and Cultural Heritage" 360 $humanId
    return $ids
}

function Get-SubjectCategoryId([string]$title, [string]$abstract, [string]$keywords, $currentCategoryId) {
    if (!$script:SubjectCategoryIds) {
        $script:SubjectCategoryIds = Initialize-SubjectCategories
    }
    $normalizedTitle = Normalize-Text $title
    $text = $normalizedTitle + " " + (Normalize-Text $keywords) + " " + (Normalize-Text $abstract)

    if ($text -match "(?i)From Prompts to Printable Models|Additive Manufacturing|3D Printing|Printable Models|Robotics And Automation") {
        return $script:SubjectCategoryIds["robotics"]
    }
    if ($text -match "(?i)3DTopia|Pixal3D|Articraft|3D Generation|3D Asset|Text-to-3D|Image-to-3D|Primitive Diffusion|Articulated 3D") {
        return $script:SubjectCategoryIds["threeD"]
    }
    if ($text -match "(?i)MonoRelief|Monocular|Depth Estimation|Normal Estimation|Relief Recovery|Computer Vision") {
        return $script:SubjectCategoryIds["vision"]
    }
    if ($normalizedTitle -match "(?i)Beyond RAG|Retrieval by Decoupling") {
        return $script:SubjectCategoryIds["retrieval"]
    }
    if ($text -match "(?i)Mem0|MIRIX|MemAgent|Agentic Memory|AI Agents|Agentic AI|Long-Context|Large Language Models|LLM|Memory") {
        return $script:SubjectCategoryIds["agents"]
    }
    if ($text -match "(?i)Retrieval-Augmented|RAG") {
        return $script:SubjectCategoryIds["retrieval"]
    }
    if ($text -match "(?i)\b(NLP|natural language|language model|text generation|machine translation|information extraction|named entity|sentiment analysis|question answering)\b") {
        return $script:SubjectCategoryIds["nlp"]
    }
    if ($text -match "(?i)multimodal|multi-modal|cross-modal|vision-language|visual language|CLIP|audio-visual|text-image") {
        return $script:SubjectCategoryIds["multimodal"]
    }
    if ($text -match "(?i)machine learning|deep learning|neural network|representation learning|supervised learning|unsupervised learning|self-supervised|classification|prediction") {
        return $script:SubjectCategoryIds["aiML"]
    }
    if ($text -match "(?i)data science|statistics|statistical|bayesian|causal inference|data mining|analytics|forecasting|time series|regression") {
        return $script:SubjectCategoryIds["dataStats"]
    }
    if ($text -match "(?i)software engineering|program analysis|database|distributed system|cloud computing|operating system|compiler|microservice|devops") {
        return $script:SubjectCategoryIds["software"]
    }
    if ($text -match "(?i)human-computer interaction|\bHCI\b|user experience|\bUX\b|visualization|interface|interaction design") {
        return $script:SubjectCategoryIds["hci"]
    }
    if ($text -match "(?i)communication|wireless|signal processing|semiconductor|circuit|sensor|internet of things|\bIoT\b|edge computing|network protocol") {
        return $script:SubjectCategoryIds["electronics"]
    }
    if ($text -match "(?i)security|privacy|cryptography|encryption|adversarial|attack|defense|malware|vulnerability|blockchain") {
        return $script:SubjectCategoryIds["security"]
    }
    if ($text -match "(?i)mathematics|optimization|optimal|theorem|proof|algebra|geometry|topology|numerical method|operations research") {
        return $script:SubjectCategoryIds["math"]
    }
    if ($text -match "(?i)physics|astronomy|quantum|particle|cosmology|optics|mechanics|thermodynamics") {
        return $script:SubjectCategoryIds["physics"]
    }
    if ($text -match "(?i)chemistry|chemical|material|polymer|catalyst|battery|nanomaterial|molecule|crystal") {
        return $script:SubjectCategoryIds["chemMaterials"]
    }
    if ($text -match "(?i)biology|bioinformatics|genomics|protein|cell|gene|biomedical|neuroscience|ecology") {
        return $script:SubjectCategoryIds["lifeBio"]
    }
    if ($text -match "(?i)medical|medicine|clinical|health|radiology|diagnosis|patient|disease|MRI|CT|ultrasound|pathology") {
        return $script:SubjectCategoryIds["medical"]
    }
    if ($text -match "(?i)environment|climate|energy|geology|earth|carbon|sustainability|renewable|ocean|atmosphere") {
        return $script:SubjectCategoryIds["earthEnergy"]
    }
    if ($text -match "(?i)architecture|building information modeling|\bBIM\b|digital twin|urban|construction|city model") {
        return $script:SubjectCategoryIds["architectureDT"]
    }
    if ($text -match "(?i)education|learning science|teaching|student|curriculum|pedagogy|MOOC|e-learning") {
        return $script:SubjectCategoryIds["education"]
    }
    if ($text -match "(?i)economics|management|business|innovation|finance|marketing|organization|supply chain|strategy") {
        return $script:SubjectCategoryIds["management"]
    }
    if ($text -match "(?i)law|policy|ethics|governance|regulation|compliance|copyright|fairness|responsible ai") {
        return $script:SubjectCategoryIds["lawEthics"]
    }
    if ($text -match "(?i)linguistics|literature|communication|journalism|media studies|translation studies|discourse") {
        return $script:SubjectCategoryIds["languageMedia"]
    }
    if ($text -match "(?i)art|design|digital media|creative|aesthetics|animation|game studies|museum") {
        return $script:SubjectCategoryIds["artsDesign"]
    }
    if ($text -match "(?i)psychology|cognitive|behavior|behaviour|emotion|perception|attention|human factors") {
        return $script:SubjectCategoryIds["psychCognitive"]
    }
    if ($text -match "(?i)digital humanities|cultural heritage|archive|philology|history|archaeology") {
        return $script:SubjectCategoryIds["digitalHumanities"]
    }
    return [int]$currentCategoryId
}

Write-Host "Creating backup tables with stamp $Stamp ..."
Invoke-NonQuery "SELECT * INTO dbo.LiteratureMetadataRescanBackup_$Stamp FROM dbo.Literature WHERE status IN (0,1); SELECT m.* INTO dbo.LiteratureAuthorMapRescanBackup_$Stamp FROM dbo.LiteratureAuthorMap m INNER JOIN dbo.Literature l ON l.id=m.literature_id WHERE l.status IN (0,1); SELECT m.* INTO dbo.LiteratureTagMapRescanBackup_$Stamp FROM dbo.LiteratureTagMap m INNER JOIN dbo.Literature l ON l.id=m.literature_id WHERE l.status IN (0,1);"

$rows = Invoke-Query @"
SELECT l.*,
       f.file_name,
       f.file_path
FROM dbo.Literature l
OUTER APPLY (
    SELECT TOP 1 file_name,file_path
    FROM dbo.LiteratureFile
    WHERE literature_id=l.id AND status=1 AND (LOWER(file_name) LIKE '%.pdf' OR LOWER(file_path) LIKE '%.pdf')
    ORDER BY orderid,id
) f
WHERE l.status IN (0,1)
ORDER BY l.id
"@

$items = New-Object System.Collections.Generic.List[object]
$arxivIds = New-Object System.Collections.Generic.HashSet[string]
foreach ($row in $rows.Rows) {
    $pdfPath = ""
    if (![string]::IsNullOrWhiteSpace([string]$row["file_path"])) {
        $pdfPath = Join-Path $UploadRoot ([string]$row["file_path"])
    }
    $parsed = $null
    if (![string]::IsNullOrWhiteSpace($pdfPath) -and (Test-Path $pdfPath)) {
        Write-Host "Parsing #$($row["id"]) $pdfPath"
        $parsed = Parse-Pdf $pdfPath
    }
    $arxivId = Extract-ArxivId $row $parsed
    if (![string]::IsNullOrWhiteSpace($arxivId)) { [void]$arxivIds.Add($arxivId) }
    $items.Add([pscustomobject]@{ row=$row; parsed=$parsed; arxivId=$arxivId; pdfPath=$pdfPath })
}

$arxivMap = @{}
foreach ($id in $arxivIds) {
    Write-Host "Fetching arXiv metadata $id"
    $meta = Get-ArxivMetadata $id
    if ($meta) { $arxivMap[$id] = $meta }
    Start-Sleep -Milliseconds 700
}

$report = New-Object System.Collections.Generic.List[object]
foreach ($item in $items) {
    $row = $item.row
    $parsed = $item.parsed
    $litId = [int]$row["id"]
    $arxiv = if ($item.arxivId -and $arxivMap.ContainsKey($item.arxivId)) { $arxivMap[$item.arxivId] } else { $null }

    $parsedTitle = if ($parsed) { Normalize-Text $parsed.title } else { "" }
    $currentTitle = Normalize-Text $row["title"]
    $title = if ($arxiv -and $arxiv.title) { $arxiv.title } elseif ((Is-WeakTitle $currentTitle) -or $parsedTitle.Length -gt ($currentTitle.Length + 8)) { $parsedTitle } else { $currentTitle }
    if ([string]::IsNullOrWhiteSpace($title)) { $title = $currentTitle }

    $abstract = ""
    if ($arxiv -and $arxiv.abstract -and $arxiv.abstract.Length -gt 120) {
        $abstract = $arxiv.abstract
    }
    elseif ($parsed -and $parsed.abstract -and (Normalize-Text $parsed.abstract).Length -gt 80) {
        $abstract = Normalize-Text $parsed.abstract
    }
    else {
        $abstract = Normalize-Text $row["abstract_text"]
    }

    $authors = @()
    if ($arxiv -and $arxiv.authors.Count -gt 0) {
        $authors = @($arxiv.authors)
    }
    elseif ($parsed -and $parsed.authors) {
        $authors = @($parsed.authors | ForEach-Object { Normalize-Text $_ } | Where-Object { $_ })
    }

    $institution = ""
    if ($parsed -and $parsed.institutions) {
        $institution = Clean-Institutions $parsed.institutions
    }
    if ([string]::IsNullOrWhiteSpace($institution)) {
        $institution = Normalize-Text $row["institution"]
    }

    $doi = if ($arxiv) { $arxiv.doi } elseif ($parsed -and $parsed.doi) { Normalize-Text $parsed.doi } else { Normalize-Text $row["doi"] }
    if ($doi -eq "000001") { $doi = "" }
    $publisher = if ($parsed -and $parsed.publisher) { Normalize-Text $parsed.publisher } elseif ($arxiv) { "arXiv" } else { Normalize-Text $row["publisher"] }
    $sourceDb = if ($arxiv) { "arXiv" } else { Normalize-Text $row["source_db"] }
    $externalUrl = if ($arxiv) { $arxiv.url } else { Normalize-Text $row["external_url"] }

    $journal = if ($parsed -and $parsed.journal) { Normalize-Text $parsed.journal } else { Normalize-Text $row["journal_name"] }
    if ($journal -match "(?i)journal of latex class files|journalof latexclass files") { $journal = "" }
    $conference = if ($parsed -and $parsed.conference) { Normalize-Text $parsed.conference } else { Normalize-Text $row["conference_name"] }
    if ($conference -match "(?i)final published version|ieee xplore|preprint version") { $conference = "" }
    if ($title -match "3DTopia-XL") {
        $conference = "CVPR"
        $publisher = "IEEE/CVF"
    }
    if (![string]::IsNullOrWhiteSpace($journal)) { $conference = "" }

    $sourceType = U "\u5176\u4ED6"
    if (![string]::IsNullOrWhiteSpace($journal)) { $sourceType = U "\u671F\u520A\u8BBA\u6587" }
    elseif (![string]::IsNullOrWhiteSpace($conference)) { $sourceType = U "\u4F1A\u8BAE\u8BBA\u6587" }
    elseif (![string]::IsNullOrWhiteSpace((Normalize-Text $row["source_type"])) -and -not $arxiv) { $sourceType = Normalize-Text $row["source_type"] }

    $year = $null
    if ($title -match "3DTopia-XL") { $year = 2025 }
    elseif ($arxiv -and $arxiv.year) { $year = [int]$arxiv.year }
    elseif ($parsed -and $parsed.publish_year) { $year = [int]$parsed.publish_year }
    elseif ($row["publish_year"] -ne [DBNull]::Value) { $year = [int]$row["publish_year"] }

    $pages = if ($title -match "3DTopia-XL") { "26576-26586" } elseif ($parsed -and $parsed.pages) { Normalize-Text $parsed.pages } else { Normalize-Text $row["pages"] }
    $volume = if ($parsed -and $parsed.volume) { Normalize-Text $parsed.volume } else { Normalize-Text $row["volume"] }
    $issue = if ($parsed -and $parsed.issue) { Normalize-Text $parsed.issue } else { Normalize-Text $row["issue"] }

    if ($litId -eq 1 -and !$parsed -and !$arxiv) {
        $doi = ""
        $journal = ""
        $conference = ""
        $publisher = ""
        $pages = ""
        $sourceType = U "\u5176\u4ED6"
        $categoryId = 0
    }

    $terms = Get-DerivedTerms $title $abstract ($journal + " " + $conference) $(if ($parsed) { $parsed.keywords } else { @() })
    $keywords = ($terms.keywords -join (U "\uFF0C"))
    $categoryId = Get-SubjectCategoryId $title $abstract $keywords $row["category_id"]
    if ($litId -eq 1 -and !$parsed -and !$arxiv) {
        $categoryId = 0
        $keywords = ""
        $terms = [pscustomobject]@{ keywords=@(); tags=@() }
    }

    [void](Invoke-ParameterizedNonQuery @"
UPDATE dbo.Literature
SET title=@title,
    doi=@doi,
    keywords=@keywords,
    abstract_text=@abstract,
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
    institution=@institution,
    external_url=CASE WHEN LTRIM(RTRIM(ISNULL(external_url,N'')))=N'' THEN @external_url ELSE external_url END,
    source_db=CASE WHEN LTRIM(RTRIM(ISNULL(source_db,N'')))=N'' THEN @source_db ELSE source_db END,
    updatetime=GETDATE()
WHERE id=@id
"@ @{
        "@id" = $litId
        "@title" = Truncate-Text $title 500
        "@doi" = Truncate-Text $doi 100
        "@keywords" = Truncate-Text $keywords 500
        "@abstract" = $abstract
        "@source_type" = Truncate-Text $sourceType 50
        "@language" = $(if ($title -match "[\u4e00-\u9fff]") { U "\u4E2D\u6587" } else { U "\u82F1\u6587" })
        "@publish_year" = $year
        "@journal_name" = Truncate-Text $journal 300
        "@conference_name" = Truncate-Text $conference 300
        "@publisher" = Truncate-Text $publisher 300
        "@volume" = Truncate-Text $volume 50
        "@issue" = Truncate-Text $issue 50
        "@pages" = Truncate-Text $pages 100
        "@category_id" = $categoryId
        "@institution" = Truncate-Text $institution 500
        "@external_url" = Truncate-Text $externalUrl 500
        "@source_db" = Truncate-Text $sourceDb 200
    })

    $authorCount = Set-AuthorMap $litId $authors $institution
    $tagCount = Add-Tags $litId $terms.tags
    $report.Add([pscustomobject]@{
        id = $litId
        title = $title
        arxiv = $item.arxivId
        author_count = $authorCount
        added_tag_count = $tagCount
        keywords = $keywords
        source_type = $sourceType
        year = $year
        pages = $pages
    })
}

$reportPath = Join-Path $Root "tools\rescan_literature_metadata_$Stamp.json"
$report | ConvertTo-Json -Depth 5 | Set-Content -Path $reportPath -Encoding UTF8
Write-Host "Done. Report: $reportPath"
Write-Host "Backup tables: LiteratureMetadataRescanBackup_$Stamp, LiteratureAuthorMapRescanBackup_$Stamp, LiteratureTagMapRescanBackup_$Stamp"
