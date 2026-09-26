# ビルド → GUIテスト実行 → 結果(trx)とキャプチャを出力
# 使い方: PowerShell でソリューションのフォルダに移動して  .\run-gui-tests.ps1
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

dotnet build .\GuiTestSample.sln -c Debug
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet test .\tests\SampleApp.GuiTests\SampleApp.GuiTests.csproj `
    --no-build `
    --settings .\tests\SampleApp.GuiTests\gui.runsettings `
    --filter "TestCategory=GUI" `
    --logger "trx;LogFileName=gui-test-result.trx" `
    --results-directory .\TestResults

Write-Host ""
Write-Host "キャプチャ: $PSScriptRoot\Evidence"
Write-Host "テスト結果: $PSScriptRoot\TestResults\gui-test-result.trx"
