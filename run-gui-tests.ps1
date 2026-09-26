# ビルド → GUIテスト実行 → レポート(HTML)・キャプチャを出力
#
# 使い方（PowerShell でソリューションのフォルダに移動して実行）:
#   .\run-gui-tests.ps1 -Target VB            … VB断面をテスト
#   .\run-gui-tests.ps1 -Target CS            … C#断面をテスト
#   .\run-gui-tests.ps1 -Target Both          … VB断面 → (全件合格なら) C#断面 → 比較 を続けて実行
#   .\run-gui-tests.ps1 -Target CS -Compare   … C#断面をテストし、直近のVB断面の結果と比較
#   .\run-gui-tests.ps1 -Target VB -Filter "FullyQualifiedName~顧客登録画面Tests"   … 一部の画面だけ
param(
    [ValidateSet("VB", "CS", "Both")]
    [string]$Target = "VB",
    [switch]$Compare,
    [string]$Filter = "TestCategory=GUI"
)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$settings = ".\tests\SampleApp.GuiTests\gui.runsettings"
$evidence = Join-Path $PSScriptRoot "Evidence"
$tool     = ".\tools\GuiTestTool\bin\Debug\net462\GuiTestTool.exe"

dotnet build .\GuiTestSample.sln -c Debug
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# 指定した対象(VB/CS)でテストを実行し、エビデンスのフォルダ(Evidence\実行日時\対象)を返す
function Invoke-GuiTest([string]$t) {
    # gui.runsettings の Target だけ書き換えた設定ファイルを作って使う
    New-Item -ItemType Directory -Force .\TestResults | Out-Null
    $xml = [xml](Get-Content $settings -Encoding UTF8)
    $xml.RunSettings.TestRunParameters.Parameter | Where-Object { $_.name -eq "Target" } | ForEach-Object { $_.value = $t }
    $runSettings = Join-Path $PSScriptRoot "TestResults\gui.$t.runsettings"
    $xml.Save($runSettings)

    $started = Get-Date
    Write-Host ""
    Write-Host "===== $t 断面のテストを実行します（マウス・キーボードに触らないでください） =====" -ForegroundColor Cyan
    dotnet test .\tests\SampleApp.GuiTests\SampleApp.GuiTests.csproj `
        --no-build `
        --settings $runSettings `
        --filter $Filter `
        --logger "trx;LogFileName=gui-test-$t.trx" `
        --results-directory .\TestResults | Out-Host   # 出力を戻り値に混ぜない
    $passed = ($LASTEXITCODE -eq 0)

    $run = Get-ChildItem $evidence -Directory -ErrorAction SilentlyContinue |
        Where-Object { $_.LastWriteTime -ge $started.AddSeconds(-5) -and (Test-Path (Join-Path $_.FullName $t)) } |
        Sort-Object Name -Descending | Select-Object -First 1
    if ($run) {
        Write-Host "レポート: $(Join-Path $run.FullName 'report.html')"
        return @{ Passed = $passed; Dir = (Join-Path $run.FullName $t) }
    }
    return @{ Passed = $passed; Dir = $null }
}

# 直近の実行結果のうち、指定した対象(VB/CS)のエビデンスフォルダ
function Get-LatestEvidence([string]$t) {
    $run = Get-ChildItem $evidence -Directory -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -notlike "Compare_*" -and (Test-Path (Join-Path $_.FullName $t)) } |
        Sort-Object Name -Descending | Select-Object -First 1
    if ($run) { return (Join-Path $run.FullName $t) }
    return $null
}

function Invoke-Compare([string]$vbDir, [string]$csDir) {
    if (-not $vbDir -or -not $csDir) {
        Write-Host "比較するエビデンスが見つかりません（VB: $vbDir / CS: $csDir）" -ForegroundColor Yellow
        return
    }
    Write-Host ""
    Write-Host "===== VB断面とC#断面を比較します =====" -ForegroundColor Cyan
    & $tool compare $vbDir $csDir | Out-Host
}

if ($Target -eq "Both") {
    $vb = Invoke-GuiTest "VB"
    if (-not $vb.Passed) {
        Write-Host ""
        Write-Host "VB断面で不合格のテストがあります。テスト側の誤りの可能性があるため、C#断面は実行しません。" -ForegroundColor Yellow
        exit 1
    }
    $cs = Invoke-GuiTest "CS"
    Invoke-Compare $vb.Dir $cs.Dir
    if (-not $cs.Passed) { exit 1 }
}
else {
    $result = Invoke-GuiTest $Target
    if ($Compare) {
        if ($Target -eq "CS") { Invoke-Compare (Get-LatestEvidence "VB") $result.Dir }
        else { Invoke-Compare $result.Dir (Get-LatestEvidence "CS") }
    }
    if (-not $result.Passed) { exit 1 }
}
