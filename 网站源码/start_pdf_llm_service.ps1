param(
    [string]$PythonExe = "E:\tools\python.exe",
    [int]$Port = 5050,
    [string]$LlmBaseUrl = "https://ws-59uxaofc3x230c1h.cn-beijing.maas.aliyuncs.com/compatible-mode/v1",
    [string]$LlmModel = "qwen-plus-latest",
    [switch]$StopExisting
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

if ($StopExisting) {
    $listeners = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
    foreach ($listener in $listeners) {
        $processId = $listener.OwningProcess
        if ($processId -and $processId -ne $PID) {
            Stop-Process -Id $processId -Force
        }
    }
}

if (-not (Test-Path -LiteralPath $PythonExe)) {
    throw "Python executable not found: $PythonExe"
}

if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot "app_llm_pdf.py"))) {
    throw "app_llm_pdf.py not found in $ProjectRoot"
}

$env:PDF_PARSE_HOST = "127.0.0.1"
$env:PDF_PARSE_PORT = [string]$Port
$env:PYTHONIOENCODING = "utf-8"
$env:PYTHONUTF8 = "1"
$env:LLM_BASE_URL = $LlmBaseUrl
$env:LLM_MODEL = $LlmModel
if (-not $env:LLM_API_KEY) {
    $env:LLM_API_KEY = [Environment]::GetEnvironmentVariable("LLM_API_KEY", "User")
}
if (-not $env:LLM_API_KEY) {
    Write-Warning "LLM_API_KEY is not set. Set it once with: [Environment]::SetEnvironmentVariable('LLM_API_KEY','your-key','User')"
}

Start-Process -FilePath $PythonExe -ArgumentList "app_llm_pdf.py" -WorkingDirectory $ProjectRoot -WindowStyle Hidden
Write-Host "LLM PDF service started on http://127.0.0.1:$Port"
Write-Host "Health check: http://127.0.0.1:$Port/health"
