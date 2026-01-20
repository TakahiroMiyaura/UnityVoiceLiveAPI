# ============================================================
# CopyPrecompiledRefs.ps1
# .asmdefのprecompiledReferencesからDLLをコピー
# ============================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$AsmdefPath,           # .asmdefファイルのパス

    [Parameter(Mandatory=$true)]
    [string]$SourceDir,            # DLLの取得元フォルダ

    [Parameter(Mandatory=$true)]
    [string]$DestinationDir        # コピー先フォルダ
)

# JSONファイルを読み込み
$json = Get-Content -Path $AsmdefPath -Raw | ConvertFrom-Json

# precompiledReferencesを取得
$dlls = $json.precompiledReferences

if (-not $dlls -or $dlls.Count -eq 0) {
    Write-Warning "precompiledReferences が見つかりません"
    exit 1
}

Write-Host "=== DLLコピー開始 ===" -ForegroundColor Cyan
Write-Host "取得元: $SourceDir"
Write-Host "コピー先: $DestinationDir"
Write-Host "対象DLL数: $($dlls.Count)"
Write-Host ""

# コピー先フォルダがなければ作成
if (-not (Test-Path $DestinationDir)) {
    New-Item -ItemType Directory -Path $DestinationDir -Force | Out-Null
    Write-Host "コピー先フォルダを作成しました" -ForegroundColor Yellow
}

# 各DLLをコピー
$successCount = 0
$failCount = 0

foreach ($dll in $dlls) {
    $sourcePath = Join-Path $SourceDir $dll
    $destPath = Join-Path $DestinationDir $dll

    if (Test-Path $sourcePath) {
        Copy-Item -Path $sourcePath -Destination $destPath -Force
        Write-Host "[OK] $dll" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "[NG] $dll - ファイルが見つかりません" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "=== 完了 ===" -ForegroundColor Cyan
Write-Host "成功: $successCount / 失敗: $failCount"