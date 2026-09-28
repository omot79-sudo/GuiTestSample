# GuiTestSample 一式（社内環境への移行用）

このファイル1つに、GUI 自動テストのリポジトリ（GuiTestSample）の **全ファイルの中身** と、**復元の手順** が入っています。
社内環境でこのファイルから、リポジトリをそのまま作り直せます。

| 項目 | 内容 |
|---|---|
| 元のリポジトリ | omot79-sudo/GuiTestSample（ブランチ `claude/beautiful-einstein-261ida`、コミット `8a10f4e`） |
| 作成日 | 2026/09/28 |
| ファイル数 | 43 |

---

## 0. このファイルの使い方（人向け）

1. 社内の Windows PC に空のフォルダ（例: `C:\work\GuiTestSample`）を作り、このファイルを置く。
2. そのフォルダで Claude Code を起動し、次のように頼む。
   > `社内移行用_GuiTestSample一式.md` の「1. Claude への復元手順」に従って、ファイルを復元してください。
3. 復元が終わったら、Claude に次のように頼む。
   > `CLAUDE.md` を読んで、Step 1 から始めてください。

Claude Code を使わずに手作業で復元する場合も、「1. Claude への復元手順」と同じことをします。

---

## 1. Claude への復元手順

あなた（社内環境の Claude）へ。このファイルの内容からリポジトリを復元してください。

1. **「4. ファイルの中身」にある全ファイルを作る。**
   - 見出しのパス（このファイルがあるフォルダからの相対パス）に作る。フォルダがなければ作る。
   - コードブロック（5つのバッククォートで囲まれた部分）の中身を、**1文字も変えずに** 書く。整形・コメント追加・修正はしない。
   - コードブロックの中身の最初の行から最後の行までが、ファイルの中身。最後の行の後に改行を1つ入れて終わる。
   - 改行コードは LF のままでよい（次の手順のスクリプトで整える）。
   - このファイル自身（`社内移行用_GuiTestSample一式.md`）は、復元するファイルに含めない。
2. **「2. 仕上げと確認のスクリプト」を `restore-check.ps1` として保存し、実行する。**
   スクリプトは、BOM（文字コードの印）と改行コードを元のリポジトリと同じにそろえてから、全ファイルが元と完全に一致するかをハッシュ値で確認する。
   ```powershell
   # Windows PowerShell 5.1 は BOM なしの日本語スクリプトを読めないため、先に BOM を付けてから実行する
   $p = 'restore-check.ps1'; [IO.File]::WriteAllText($p, [IO.File]::ReadAllText($p), (New-Object Text.UTF8Encoding($true)))
   powershell -ExecutionPolicy Bypass -File .\restore-check.ps1
   ```
3. **「不一致」「ない」と表示されたファイルがあれば**、そのファイルを「4. ファイルの中身」と見比べて書き直し、2 をもう一度実行する。全件「一致」になるまで繰り返す。
4. 全件一致したら、`restore-check.ps1` を削除し、このファイルはリポジトリの外へ移す（リポジトリには含めない）。
5. Git が使える場合は、リポジトリとして記録する。
   ```powershell
   git init
   git add -A
   git commit -m "GuiTestSample を復元（元のコミット 8a10f4e）"
   ```
6. 復元できたことをユーザーに報告し、`CLAUDE.md` を読んで Step 1 から作業を始める。

---

## 2. 仕上げと確認のスクリプト

このファイルがあるフォルダ（＝リポジトリの一番上）で実行します。

`````powershell
# restore-check.ps1
# 復元したファイルの BOM と改行コードを元のリポジトリと同じにそろえ、SHA256 で元と一致するか確認する
$ErrorActionPreference = "Stop"
$files = @(
    @{ Path = '.gitignore'; Bom = $false; Sha256 = '5900cf5d91c19d776c888b2512409be463b629872697176c4cd3fbd4bbde54f2' },
    @{ Path = 'CLAUDE.md'; Bom = $true; Sha256 = '5fc09b06f9c0fa559f03806682313673d03667869cb2449a0a1e290ea7b8cf2b' },
    @{ Path = 'GuiTestSample.sln'; Bom = $true; Sha256 = 'f6ea3c6f40e527719fe1a818ee2c336dab539249b30b32355455e5a31224d663' },
    @{ Path = 'README.md'; Bom = $true; Sha256 = 'e0a36df62c4c1160bdd7896c470f212cf901979076938b62812f035508eb520a' },
    @{ Path = 'docs/自動テスト方針.md'; Bom = $true; Sha256 = '860c4f916da6c7af81e69f700619aa694399c8de7f6f8ef1363e7a1a0491908c' },
    @{ Path = 'run-gui-tests.ps1'; Bom = $true; Sha256 = '5d317a57fc956376dd5460a5224bc79581817016bd750658d68a8d76936acb51' },
    @{ Path = 'src/SampleApp/CustomerForm.cs'; Bom = $true; Sha256 = '1c2aaf71b632de77730ca437de66f985c759453cf706be02463e2d9c48c334cb' },
    @{ Path = 'src/SampleApp/MainForm.cs'; Bom = $true; Sha256 = '9104cddbf24fa3d9ffd446d6dfc05826d6295662c8dd60afe7d580fa4b108194' },
    @{ Path = 'src/SampleApp/Program.cs'; Bom = $true; Sha256 = '81464e2c50248fdbd7cacf42bba793bb3dc2c59a5b6402cc496dc9b9619e119d' },
    @{ Path = 'src/SampleApp/SampleApp.csproj'; Bom = $true; Sha256 = '63653658ccf14a486d5473c7215e12340c1b8b3c3beff6c1962ed95092362555' },
    @{ Path = 'src/SampleAppVB/CustomerForm.vb'; Bom = $true; Sha256 = 'f05675d97ee28b9ea87cf5ecd5714bd5ba2487f24a5eac6f2c0e0df5d8c73de1' },
    @{ Path = 'src/SampleAppVB/MainForm.vb'; Bom = $true; Sha256 = '5ef006880f9b9c48dc6572c1e8d7142f170ec261ebb4409030e154f592f2d124' },
    @{ Path = 'src/SampleAppVB/Program.vb'; Bom = $true; Sha256 = '53e3f0a6ce8d5aa94fabddac9c8915558ca061b31504dd8248ccd99dc5ec8938' },
    @{ Path = 'src/SampleAppVB/SampleAppVB.vbproj'; Bom = $true; Sha256 = '350526f9e26d06171f624d8cfb610fcc3d3e300bbdfd9075e446b4e26094efcf' },
    @{ Path = 'tests/GuiTestKit/Automation/ControlAccess.cs'; Bom = $true; Sha256 = 'bc693262abd7afb76be8e24b9acc290ded6459208dc747af4b9bc9fa3eb33494' },
    @{ Path = 'tests/GuiTestKit/Automation/ICustomControlHandler.cs'; Bom = $true; Sha256 = '741be2fa9451bbea8b5ae6a4e52c32fa3df5c5a3b27eb80ae3d0e4a1e46ab20c' },
    @{ Path = 'tests/GuiTestKit/Automation/MessageBoxPage.cs'; Bom = $true; Sha256 = '50c332aa0dd7cfc1dcac65e09357f014b58afc6e981f06bf6a577f177b2dc720' },
    @{ Path = 'tests/GuiTestKit/Automation/NativeMethods.cs'; Bom = $true; Sha256 = '1371d92f19db6dac26999cc6612246c3154fb69ff9bc7d4d29235e0fc8a75fe4' },
    @{ Path = 'tests/GuiTestKit/Automation/ScreenPageBase.cs'; Bom = $true; Sha256 = 'd0b9efd77ad983652905e9e2569dcffe25fca92e109c85265a04f6b7f04ae19c' },
    @{ Path = 'tests/GuiTestKit/Automation/ScreenValues.cs'; Bom = $true; Sha256 = '640620eb1ee5b87ff8dfce4967bb1ef94020415714bbd11292559b5190b38eb9' },
    @{ Path = 'tests/GuiTestKit/Automation/SpreadGrid.cs'; Bom = $true; Sha256 = 'cf7a11bb7f4c37b8bd096c2ec1062dd555f5afdc90cb302305d0815fb201739d' },
    @{ Path = 'tests/GuiTestKit/Comparison/CompareReportWriter.cs'; Bom = $true; Sha256 = '292fb9527e63e1268862aaff64d8bff608beedd255626018f3cff0f2f6a22319' },
    @{ Path = 'tests/GuiTestKit/Comparison/EvidenceComparer.cs'; Bom = $true; Sha256 = 'd374d4ca6b43c51471be7e77941ce3b0d085db296de451d7815cbece905e5edd' },
    @{ Path = 'tests/GuiTestKit/Comparison/ImageComparer.cs'; Bom = $true; Sha256 = 'daf0583a2ddfc3a83d4ea5cb9719551230f3adbe74c67248f75e8641da69e74b' },
    @{ Path = 'tests/GuiTestKit/Evidence/JsonFile.cs'; Bom = $false; Sha256 = 'afd471ae690b8481de60bbd67b46453ca56dd02aeed760faee07cba008b3b19c' },
    @{ Path = 'tests/GuiTestKit/Evidence/Records.cs'; Bom = $false; Sha256 = 'ac03ec25f097a11eff21675f207a4d1df9ec75411a888a4b4ffc21d9343856a9' },
    @{ Path = 'tests/GuiTestKit/GuiTestKit.csproj'; Bom = $true; Sha256 = '2fde50253ceae0a87220fb18fad4ecc0d1fd1d0b99c23951ef6b7ab6f3aa0165' },
    @{ Path = 'tests/GuiTestKit/Reporting/Html.cs'; Bom = $true; Sha256 = '7dc0b80c679bbe5d6d60d103c388d63f5f53a6084e9b091401b843c47a2b72b4' },
    @{ Path = 'tests/GuiTestKit/Reporting/RunReportWriter.cs'; Bom = $true; Sha256 = '3ccdefcea6d64ff3e76537126f28bbf49dbf6aa43afce34054ed83f5b035813f' },
    @{ Path = 'tests/GuiTestKit/Testing/GuiTestBase.cs'; Bom = $true; Sha256 = '7990d187eb942eef2a2e57b9b30712bcf234c05ec561f84b8ca94234522456a3' },
    @{ Path = 'tests/GuiTestKit/Testing/TestSession.cs'; Bom = $true; Sha256 = '5c5a1d24b3101058bab3a5ea474c196b8a96f95353fb4ce8cb48ea4cd90e3f73' },
    @{ Path = 'tests/SampleApp.GuiTests/AssemblyInfo.cs'; Bom = $true; Sha256 = '3b79c9916dd2de9d083f3763a218f2f0beac96b3af1451063543b6567798fc6d' },
    @{ Path = 'tests/SampleApp.GuiTests/Pages/メイン画面Page.cs'; Bom = $true; Sha256 = 'd58f997543e3871e867bae35f515fd4e6b61a7fee5203d04e5321eaec23b81d9' },
    @{ Path = 'tests/SampleApp.GuiTests/Pages/画面遷移.cs'; Bom = $true; Sha256 = 'fe321574927c779ad56307be35e02c6abab02794f1acaa01f8a03c976fbc6f31' },
    @{ Path = 'tests/SampleApp.GuiTests/Pages/顧客登録Page.cs'; Bom = $true; Sha256 = 'e03fd34e0d4d0a50c7d9a684f317c14411d4eaf381838764615f103240ce4761' },
    @{ Path = 'tests/SampleApp.GuiTests/SampleApp.GuiTests.csproj'; Bom = $true; Sha256 = '7973575d77d87477f35ce10893ab8e5933f81dd1956c5a0a7943861c4e8eeb2e' },
    @{ Path = 'tests/SampleApp.GuiTests/SampleAppTestBase.cs'; Bom = $true; Sha256 = '2587f11981d89e5bc15fa7a3c1bf8ee140761acd18ec0cba406af1610e73bb31' },
    @{ Path = 'tests/SampleApp.GuiTests/gui.runsettings'; Bom = $true; Sha256 = '864f070b259bf51e03b1b0697574551685a041a817e262e468afb8e734b1271a' },
    @{ Path = 'tests/SampleApp.GuiTests/計算画面Tests.cs'; Bom = $true; Sha256 = '71f59a96002d31e231cb1e3a1b1534bb889f2997bd81ed6ea54d579c9bf7fef0' },
    @{ Path = 'tests/SampleApp.GuiTests/顧客登録画面Tests.cs'; Bom = $true; Sha256 = '566be2df97c6fb082a02a73e86f7008494568b01336fceb247f2ab397e5b5fea' },
    @{ Path = 'tools/GuiTestTool/DumpCommand.cs'; Bom = $true; Sha256 = 'c46c4264479613f8a7b0a2ab08aa451e1db87dd3606f56c826645e3a0010170f' },
    @{ Path = 'tools/GuiTestTool/GuiTestTool.csproj'; Bom = $true; Sha256 = '0857714d4b7c2aa319044b30f169eedec489206cd5b461d712d91d80befed602' },
    @{ Path = 'tools/GuiTestTool/Program.cs'; Bom = $true; Sha256 = '935da20e1d51c6836a5bcf15657066a1703b9d28c08b3c1bceb4b634fac75477' }
)

$utf8 = New-Object System.Text.UTF8Encoding($false)
$sha = [System.Security.Cryptography.SHA256]::Create()
$ng = 0
foreach ($f in $files) {
    $full = Join-Path (Get-Location) $f.Path
    if (-not (Test-Path -LiteralPath $full)) {
        Write-Host ("ない    : " + $f.Path) -ForegroundColor Red
        $ng++
        continue
    }
    # BOM を外し、改行を LF にそろえてから、必要なファイルだけ BOM を付け直す
    $text = [System.IO.File]::ReadAllText($full, $utf8).TrimStart([char]0xFEFF).Replace("`r`n", "`n")
    if (-not $text.EndsWith("`n")) { $text += "`n" }
    $bytes = $utf8.GetBytes($text)
    if ($f.Bom) { $bytes = [byte[]](0xEF, 0xBB, 0xBF) + $bytes }
    [System.IO.File]::WriteAllBytes($full, $bytes)

    $hash = -join ($sha.ComputeHash($bytes) | ForEach-Object { $_.ToString("x2") })
    if ($hash -eq $f.Sha256) {
        Write-Host ("一致    : " + $f.Path)
    } else {
        Write-Host ("不一致  : " + $f.Path) -ForegroundColor Red
        $ng++
    }
}

Write-Host ""
if ($ng -eq 0) {
    Write-Host ("全 " + $files.Count + " ファイルが元のリポジトリと一致しました。") -ForegroundColor Green
} else {
    Write-Host ("一致しないファイルが " + $ng + " 件あります。") -ForegroundColor Red
    exit 1
}
`````

---

## 3. ファイル一覧

| # | パス | サイズ(バイト) | BOM |
|---|---|---|---|
| 1 | `.gitignore` | 1,308 | なし |
| 2 | `CLAUDE.md` | 12,305 | あり |
| 3 | `GuiTestSample.sln` | 2,949 | あり |
| 4 | `README.md` | 11,665 | あり |
| 5 | `docs/自動テスト方針.md` | 20,904 | あり |
| 6 | `run-gui-tests.ps1` | 4,265 | あり |
| 7 | `src/SampleApp/CustomerForm.cs` | 6,607 | あり |
| 8 | `src/SampleApp/MainForm.cs` | 3,113 | あり |
| 9 | `src/SampleApp/Program.cs` | 348 | あり |
| 10 | `src/SampleApp/SampleApp.csproj` | 657 | あり |
| 11 | `src/SampleAppVB/CustomerForm.vb` | 6,037 | あり |
| 12 | `src/SampleAppVB/MainForm.vb` | 2,856 | あり |
| 13 | `src/SampleAppVB/Program.vb` | 261 | あり |
| 14 | `src/SampleAppVB/SampleAppVB.vbproj` | 719 | あり |
| 15 | `tests/GuiTestKit/Automation/ControlAccess.cs` | 13,428 | あり |
| 16 | `tests/GuiTestKit/Automation/ICustomControlHandler.cs` | 1,174 | あり |
| 17 | `tests/GuiTestKit/Automation/MessageBoxPage.cs` | 1,873 | あり |
| 18 | `tests/GuiTestKit/Automation/NativeMethods.cs` | 4,503 | あり |
| 19 | `tests/GuiTestKit/Automation/ScreenPageBase.cs` | 15,074 | あり |
| 20 | `tests/GuiTestKit/Automation/ScreenValues.cs` | 3,459 | あり |
| 21 | `tests/GuiTestKit/Automation/SpreadGrid.cs` | 4,933 | あり |
| 22 | `tests/GuiTestKit/Comparison/CompareReportWriter.cs` | 8,958 | あり |
| 23 | `tests/GuiTestKit/Comparison/EvidenceComparer.cs` | 8,909 | あり |
| 24 | `tests/GuiTestKit/Comparison/ImageComparer.cs` | 8,224 | あり |
| 25 | `tests/GuiTestKit/Evidence/JsonFile.cs` | 1,193 | なし |
| 26 | `tests/GuiTestKit/Evidence/Records.cs` | 5,134 | なし |
| 27 | `tests/GuiTestKit/GuiTestKit.csproj` | 940 | あり |
| 28 | `tests/GuiTestKit/Reporting/Html.cs` | 3,800 | あり |
| 29 | `tests/GuiTestKit/Reporting/RunReportWriter.cs` | 8,124 | あり |
| 30 | `tests/GuiTestKit/Testing/GuiTestBase.cs` | 11,204 | あり |
| 31 | `tests/GuiTestKit/Testing/TestSession.cs` | 9,314 | あり |
| 32 | `tests/SampleApp.GuiTests/AssemblyInfo.cs` | 645 | あり |
| 33 | `tests/SampleApp.GuiTests/Pages/メイン画面Page.cs` | 1,173 | あり |
| 34 | `tests/SampleApp.GuiTests/Pages/画面遷移.cs` | 1,395 | あり |
| 35 | `tests/SampleApp.GuiTests/Pages/顧客登録Page.cs` | 1,851 | あり |
| 36 | `tests/SampleApp.GuiTests/SampleApp.GuiTests.csproj` | 1,354 | あり |
| 37 | `tests/SampleApp.GuiTests/SampleAppTestBase.cs` | 862 | あり |
| 38 | `tests/SampleApp.GuiTests/gui.runsettings` | 1,513 | あり |
| 39 | `tests/SampleApp.GuiTests/計算画面Tests.cs` | 2,899 | あり |
| 40 | `tests/SampleApp.GuiTests/顧客登録画面Tests.cs` | 4,839 | あり |
| 41 | `tools/GuiTestTool/DumpCommand.cs` | 10,518 | あり |
| 42 | `tools/GuiTestTool/GuiTestTool.csproj` | 891 | あり |
| 43 | `tools/GuiTestTool/Program.cs` | 5,680 | あり |

---

## 4. ファイルの中身

### 1. `.gitignore`

`````text
## ================================================
## GuiTestSample 用 .gitignore（Visual Studio / .NET）
## ================================================

# ---- Visual Studio のユーザー設定・キャッシュ ----
.vs/
*.suo
*.user
*.userosscache
*.sln.docstates
*.rsuser

# ---- ビルド成果物 ----
[Bb]in/
[Oo]bj/
[Dd]ebug/
[Rr]elease/
x64/
x86/
[Aa][Rr][Mm]64/
[Ll]og/
[Ll]ogs/

# ---- NuGet ----
*.nupkg
*.snupkg
**/[Pp]ackages/*
!**/[Pp]ackages/build/
project.lock.json
project.fragment.lock.json
*.nuget.props
*.nuget.targets

# ---- テスト結果 ----
TestResults/
[Tt]est[Rr]esult*/
*.trx
*.coverage
*.coveragexml

# ---- GUIテストのキャプチャ（エビデンス） ----
# 実行のたびに大量に増えるため管理対象外にする。
# 残したい場合は、別の保管場所にコピーすること。
# 先頭の / でリポジトリ直下だけに限定する（tests/GuiTestKit/Evidence/ はソースなので除外しない）
/Evidence/

# ---- 各種ツールのキャッシュ ----
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user
.idea/
*.sln.iml
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json

# ---- Windows / その他 ----
Thumbs.db
ehthumbs.db
Desktop.ini
$RECYCLE.BIN/
*.log
*.tmp
*.bak
~$*
`````

### 2. `CLAUDE.md`

`````markdown
# CLAUDE.md — 会社環境の Claude への指示

このリポジトリは、コードファインダー（VB.NET → C# リニューアル）の GUI 自動テストの基盤とサンプルです。
**まず `docs/自動テスト方針.md` を読んでください。** 方針はユーザーと合意済みです。変えるときは必ずユーザーに相談してください。

## 前提

- このコードは Linux のクラウド環境で作られました。**ビルドと、比較ツール・HTML レポートの動作までしか確認できていません。**
  実際に画面を操作するテストは、Windows で一度も実行されていません。
- 会社環境（Windows）で最初にやることは、「サンプルで動くことの確認」と「動かない箇所の修正」です。
- ユーザーはテスト自動化の検証中です。報告は日本語で、専門用語を避けて簡潔にしてください。

## 作業の順番

### Step 1. 環境の確認

1. `dotnet --info` で .NET SDK があるか確認する（なければ Visual Studio 2022 の有無を確認）。
2. `dotnet build GuiTestSample.sln` を実行する。
   - NuGet の取得に失敗したら、社内フィード・プロキシ設定・オフラインパッケージのどれを使うかユーザーに確認する。勝手に設定を変えない。

### Step 2. サンプルで動作確認（最優先）

1. ユーザーに「実行中はマウス・キーボードに触らないでください」と伝えてから、`.\run-gui-tests.ps1 -Target Both` を実行する。
2. 全件合格し、`Evidence\実行日時\report.html` と `Evidence\Compare_*\compare.html` が出ること、比較結果が「一致」であることを確認する。
3. 失敗したら原因を調べて `tests/GuiTestKit/` を直す。特に次の点は未検証なので、重点的に確認する。
   - コンボボックスの選択（`ControlAccess.Write` の ComboBox。WinForms の DropDownList で選択と SelectedIndexChanged が起きるか）
   - ラジオボタンの ON（SelectionItem パターンが使えるか。使えなければクリック）
   - メッセージボックスの検出（`ScreenPageBase.FindMessageBox`）、別画面の検出（`FindWindow`）
   - 閉じるボタンで画面が閉じたことの判定（`WaitUntilClosed`）
   - カーソル点滅の停止と復元（`GuiTestBase.DisableCaretBlink`。テスト後に元の点滅に戻っているか）
   - 高DPI（拡大率125%等）でキャプチャの位置がずれないか
   - VB断面と C#断面の比較で、日時欄（`lblTimestamp`）以外に差異が出ないか。出たらノイズの原因を調べる
4. `GuiTestTool dump` を試す。サンプル（VB版）を起動して顧客登録画面を開き、次を実行する。
   ```powershell
   .\tools\GuiTestTool\bin\Debug\net462\GuiTestTool.exe dump --process SampleAppVB --window CustomerForm --class 顧客登録Page --out $env:TEMP\顧客登録Page.cs
   ```
   既存の `tests/SampleApp.GuiTests/Pages/顧客登録Page.cs` と同じような基本値・項目名が出るか確認する。
5. 直した内容と、確認できたこと・できなかったことをユーザーに報告する。
   その後、`docs/自動テスト方針.md` の「10. 未検証・未実装の一覧」を更新する。

### Step 3. 実製品（コードファインダー）への適用の準備

ユーザーの了承を得てから進めてください。

1. 製品用のテストプロジェクトを作る。`tests/SampleApp.GuiTests/` を手本にし、共通基盤の `tests/GuiTestKit/` はそのまま参照する。
   - `gui.runsettings`: `TargetExeVB` / `TargetExeCS` に実製品の EXE パス、`ResetCommand` に DB 初期化コマンドを設定する。
   - 製品のテスト基底クラス: ログインなど、全テスト共通の前処理を書く（`SampleAppTestBase` を参照）。
2. 実製品の画面を `GuiTestTool dump` で吸い出し、次を確認してユーザーに報告する。
   - 項目ID（Name）が取れているか
   - 同じ項目IDが重複していないか。重複していれば `"親ID/子ID"` や `"ID#2"` で指定する
   - SPREAD・ActiveReports・その他の市販部品やユーザーコントロールが、どの種類として見えるか
3. **SPREAD**: `tests/GuiTestKit/Automation/SpreadGrid.cs` はキー操作とクリップボードで作ったひな形です。
   実機で読み書きを確認し、動かなければ直す。UI Automation でセルが取れるなら、そちらに切り替えてよい。
   製品で使っている SPREAD のバージョンをユーザーに確認すること。
4. **ActiveReports・出力ファイル・DB**: 比較の仕組みがまだありません。
   出力ファイル（PDF/Excel/CSV 等）や DB の更新結果を VB/C# で比較する方式を提案し、ユーザーの合意を得てから実装する。

5. **テストデータ部品を作る**（方針は `docs/自動テスト方針.md` の「7. テストデータの方針」）。
   先にユーザーへ次を確認し、設計案を見せて合意を得てから実装する。
   - プロセス起動モードの引数の形（ファイルの渡し方、VB版・C#版で同じか）、終了コードの意味、取り込みのログの出力先
   - Eファイル・Fファイルの列の定義（列の並びと名前）、文字コード・区切り文字・改行コード、行を特定するキー項目
   - 患者データの形式と取り込み方
   - DB 初期化の方法と、1回にかかる時間
   作るもの:
   - 基本ファイル＋差分（項目の変更・行の追加・行の削除）からファイルを作る部品。行はキー項目、列は列名で指定する。
     差分なしで書き出すと元のファイルと1バイトも違わないことを、単体テストで確認する。
   - プロセス起動モードで取り込みを実行する部品（Target に応じて VB / C# の EXE を使う。終了待ち・終了コード確認・時間切れ）。
     EXE のパスと引数は runsettings に持たせる。
   - 作ったファイルと差分内容・取り込み結果を、エビデンスとレポートに残す仕組み。
   - 手本として、実績点検画面のテストを数ケース作り、ユーザーに見てもらう。

6. **テストの範囲を確定させる**（`docs/自動テスト方針.md` の「4.6 テストの範囲と網羅基準」は提案段階）。
   - VB のソースを調べ、点検ルールなどのロジックが DLL に分かれているか（画面を通さない VB/C# 比較ができるか）を報告する。
   - 画面一覧を作り、ユーザーにランク（A/B/C）を決めてもらう。
   - 合意できたら 4.6 の「提案」表記を外し、Step 4 のテストケースの洗い出しに反映する。

### Step 4. 画面ごとのテストを作る（250画面、繰り返し）

1画面ずつ、次の手順で進めてください。

1. **VB のソースを読む**（`.Designer.vb` で Name と画面構成、イベントハンドラで入力チェック・有効/無効の切り替え・画面遷移・メッセージ）。
2. **テストケースを洗い出す**。例:
   - 基本値での正常登録・更新・検索
   - 入力チェック（必須、桁数、文字種、範囲、相関チェック）ごとのエラーメッセージ
   - 境界値（最小・最大・その外側）
   - 区分などで項目の有効/無効や表示が変わるケース
   - 画面遷移（開く・閉じる・戻る）、確認メッセージの「はい/いいえ」
   - 帳票・ファイル出力・DB 更新
3. **基本値を用意する**。VB 画面に正しい値を入力し、`GuiTestTool dump` で Page クラスのひな形を作る。
4. **Page クラスを仕上げる**。比較対象外の項目（日時・採番・ユーザー名など）と、画面特有の操作を書き足す。
5. **テストを書く**（書き方は下記「テストコードのルール」）。
6. **VB断面で実行し、全件合格させる**（`.\run-gui-tests.ps1 -Target VB -Filter "FullyQualifiedName~画面名Tests"`）。
7. **C#断面で実行し、比較する**（`-Target CS -Compare`）。不合格や差異は、下記「C#断面で失敗したとき」に従う。
8. テストケースの一覧と、VB/C# の結果をユーザーに報告する。
   網羅状況（例: メッセージ 23 件中 23 件、画面遷移 5 経路中 5 経路）も合わせて報告する。

## テストコードのルール

- テスト名・画面クラス名・操作名は日本語。テスト名は「〜の場合は〜になる」のように、期待する結果が分かる形にする。
- テストクラスは画面ごとに1つ（`○○画面Tests`）。`[TestCategory("GUI")]` を付ける。
- 入力は `EnterBaseValues(差分)` を基本にする。差分には、そのケースで変える項目だけを書く。
- 対象画面までの遷移は `Pages/画面遷移.cs` にまとめ、テストクラスの `[TestInitialize]` から呼ぶ。
  各テストケースには遷移を書かない（`顧客登録画面Tests` が手本）。遷移の途中ではキャプチャを撮らない。
- ケースごとのデータは「基本ファイル＋差分」で作る。ケースごとにファイル一式を丸ごと用意しない。
  DB はテストごとに初期状態へ戻し、ケースの違いはファイルの取り込みで作る（直接 DB を書き換えない）。
- 確認は `Assert` を直接使わず、`Check` / `CheckEventually` を使う（レポートに期待値と実際の値が出るため）。
- キャプチャ（`Snap`）は、少なくとも「入力後」「ボタン押下後の結果（メッセージや遷移先）」で撮る。
  VB/C# の比較はキャプチャの順番で対応付けるので、条件分岐で撮る・撮らないを変えない。
- ダイアログや別画面が開くボタンは、`Press` ではなく `Click` / `ClickAndWaitMessageBox` / `OpenScreen` を使う。
- `Thread.Sleep` は使わない。待つときは `CheckEventually` / `WaitUntil` を使う。
- データ駆動のテストでは、`CaseId` をキャプチャより前に設定する。
- 共通基盤（`tests/GuiTestKit/`）を直したときは、サンプルのテストが VB・CS とも合格することを確認する。

## C#断面で失敗したとき

- C#断面の不合格や VB との差異は、**移行の欠陥候補**です。
  **C# に合わせてテストの期待値を変えたり、比較対象外を増やしたりしないでください。**
- 原因を調べ、「どの画面の、どの操作で、VB と C# で何が違うか」をユーザーに報告する。
  C# のコードを直すかどうかは、ユーザーに確認してから進める。
- 例外: 実行のたびに変わる値（日時・採番など）が原因の差異は、比較対象外に追加してよい。追加したことは報告する。

## やってはいけないこと

- 画面キャプチャ・エビデンス・製品のソースコードを、外部のサービスへ送らない（社内規定の確認が済んでいないため）。
- `Evidence/`・`TestResults/` をコミットしない（`.gitignore` で除外済み）。
- テストを通すためにテストを削除・スキップしない。
- 方針（`docs/自動テスト方針.md`）と違うやり方にするときは、先にユーザーに相談する。

## よく使うコマンド

```powershell
dotnet build GuiTestSample.sln
.\run-gui-tests.ps1 -Target VB                  # VB断面
.\run-gui-tests.ps1 -Target CS -Compare         # C#断面＋直近のVB断面と比較
.\run-gui-tests.ps1 -Target Both                # VB → C# → 比較
.\tools\GuiTestTool\bin\Debug\net462\GuiTestTool.exe compare <VBのフォルダ> <CSのフォルダ>
.\tools\GuiTestTool\bin\Debug\net462\GuiTestTool.exe report  <Evidence\実行日時>
.\tools\GuiTestTool\bin\Debug\net462\GuiTestTool.exe dump --process <プロセス名> --window <画面ID> --class <クラス名>
```
`````

### 3. `GuiTestSample.sln`

`````text

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SampleApp", "src\SampleApp\SampleApp.csproj", "{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E51}"
EndProject
Project("{778DAE3C-4631-46EA-AA77-85C1314464D9}") = "SampleAppVB", "src\SampleAppVB\SampleAppVB.vbproj", "{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E53}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SampleApp.GuiTests", "tests\SampleApp.GuiTests\SampleApp.GuiTests.csproj", "{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E52}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "GuiTestKit", "tests\GuiTestKit\GuiTestKit.csproj", "{1BCA12FA-3979-4A33-A080-654AA1B03BED}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "GuiTestTool", "tools\GuiTestTool\GuiTestTool.csproj", "{E735C6C7-B337-485E-8967-C480D3686A4F}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E51}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E51}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E51}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E51}.Release|Any CPU.Build.0 = Release|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E53}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E53}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E53}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E53}.Release|Any CPU.Build.0 = Release|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E52}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E52}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E52}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{6F1B2C3A-1A2B-4C5D-8E9F-0A1B2C3D4E52}.Release|Any CPU.Build.0 = Release|Any CPU
		{1BCA12FA-3979-4A33-A080-654AA1B03BED}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{1BCA12FA-3979-4A33-A080-654AA1B03BED}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{1BCA12FA-3979-4A33-A080-654AA1B03BED}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{1BCA12FA-3979-4A33-A080-654AA1B03BED}.Release|Any CPU.Build.0 = Release|Any CPU
		{E735C6C7-B337-485E-8967-C480D3686A4F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{E735C6C7-B337-485E-8967-C480D3686A4F}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{E735C6C7-B337-485E-8967-C480D3686A4F}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{E735C6C7-B337-485E-8967-C480D3686A4F}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
`````

### 4. `README.md`

`````markdown
# GuiTestSample

ビルド済みEXE（VB.NET / C#、WinForms、.NET Framework 4.6.2）を外から操作し、
キャプチャと画面の全項目の値をエビデンスとして残す GUI 自動テスト（MSTest + FlaUI）のサンプルです。

VB → C# 移行で「同じ動きをすること」を確認し、移行後はそのまま回帰テストとして使うことを想定しています。

- 決定した方針: [docs/自動テスト方針.md](docs/自動テスト方針.md)
- 会社環境の Claude への指示: [CLAUDE.md](CLAUDE.md)

## 運用の流れ

```
① Claude が VB断面の画面を見てテストを作る
② VB断面にテストを実行（キャプチャ・項目値を記録）  … 全件合格＝テストが正しいことの確認
③ C#断面に同じテストを実行                        … 不合格＝移行の欠陥候補 → 修正
④ VB断面とC#断面のキャプチャ・項目値を自動比較     … compare.html
⑤ 人が差異のあるものをレビュー                    … report.html / compare.html
⑥ リニューアル後は C#断面だけ実行（回帰テスト）
```

## 構成

```
GuiTestSample/
├─ run-gui-tests.ps1                  … ビルド → テスト → レポート → 比較 を一括実行
├─ src/SampleApp/                     … テスト対象 C#版（計算画面・顧客登録画面）
├─ src/SampleAppVB/                   … テスト対象 VB版（C#版と同じ部品名・同じ動き）
├─ tests/GuiTestKit/                  … 共通基盤（製品に依存しない。そのまま流用する）
│   ├─ Automation/ScreenPageBase.cs   … 全画面共通の Page 基底クラス（入力・読み取り・ボタン・画面遷移）
│   ├─ Automation/ScreenValues.cs     … 基本値・差分の入れ物
│   ├─ Automation/ControlAccess.cs    … 部品の種類ごとの読み書き、全項目の吸い出し
│   ├─ Automation/MessageBoxPage.cs   … 標準メッセージボックス（全画面共通）
│   ├─ Automation/SpreadGrid.cs       … Spread 用のひな形【要検証】
│   ├─ Testing/GuiTestBase.cs         … テストの基底クラス（起動・終了・初期化・確認・キャプチャ）
│   ├─ Testing/TestSession.cs         … キャプチャ（人が見る用・比較用・項目値）と記録
│   ├─ Reporting/RunReportWriter.cs   … report.html（レビュー用）
│   └─ Comparison/                    … VB/C# の比較（画像・項目値）と compare.html
├─ tests/SampleApp.GuiTests/          … 製品ごとのテスト（Page クラスとテストケースだけ）
│   ├─ gui.runsettings                … Target(VB/CS)・EXEのパス・保存先・データ初期化コマンド
│   ├─ SampleAppTestBase.cs           … 製品共通の前処理（メイン画面の用意など）
│   ├─ Pages/メイン画面Page.cs
│   ├─ Pages/顧客登録Page.cs
│   ├─ 計算画面Tests.cs
│   └─ 顧客登録画面Tests.cs
└─ tools/GuiTestTool/                 … compare（比較）/ report（レポート再作成）/ dump（画面の吸い出し）
```

## 実行方法

### コマンドライン（推奨）

```powershell
.\run-gui-tests.ps1 -Target VB            # VB断面をテスト
.\run-gui-tests.ps1 -Target CS -Compare   # C#断面をテストし、直近のVB断面の結果と比較
.\run-gui-tests.ps1 -Target Both          # VB断面 →（全件合格なら）C#断面 → 比較
.\run-gui-tests.ps1 -Target VB -Filter "FullyQualifiedName~顧客登録画面Tests"   # 一部の画面だけ
```

実行中は画面が自動操作されるので、マウス・キーボードに触らないでください。

### Visual Studio

1. `GuiTestSample.sln` を開いて「ソリューションのリビルド」
2. `gui.runsettings` の `Target` を `VB` か `CS` にする
3. テストエクスプローラーで実行

## 出力

```
Evidence\
├─ 20260926_100000\                     … 1回の実行
│   ├─ report.html                      … レビュー用レポート（手順・入力値・確認結果・キャプチャ）
│   └─ VB\顧客登録画面Tests\入力エラーの場合は…_C01\
│       ├─ 01_入力後.png                … 人が見る用（タイトルバー込み・親画面も含む）
│       ├─ compare\01_入力後.png        … 比較用（前面の画面の中身だけ）
│       └─ record.json                  … 操作・確認・全項目の値の記録
└─ Compare_20260926_110500\
    ├─ compare.html                     … VB｜C#｜差分画像 を並べた比較結果（既定で差異のあるものだけ表示）
    └─ diff\*.png                       … 差分画像（赤＝差異、青い斜線＝比較対象外）
```

- 失敗したテストは「失敗時の画面」とデスクトップ全体を自動で保存します。
- テスト結果（trx）は `TestResults\` に出ます。

## テストの書き方

部品の変数は定義しません。項目ID（AutomationId ＝ WinForms の Name）で指定します。

### 画面ごとの Page クラス（中身は薄い）

```csharp
public class 顧客登録Page : ScreenPageBase
{
    public const string WindowId = "CustomerForm";   // フォームの Name
    public 顧客登録Page(Window window) : base(window) { }

    // 全項目を正しく入力した状態（GuiTestTool dump で画面から吸い出して作る）
    public override ScreenValues BaseValues { get; } = new ScreenValues
    {
        { "txtCode", "000001", "顧客コード" },     // 項目ID, 値, 項目名（レポート表示用）
        { "cmbKubun", "法人", "区分" },
        { "chkActive", ScreenValues.ON, "取引中" },
        // … 数百項目
    };

    // 実行するたびに変わる項目（日時・採番など）は比較対象外にする
    public override IEnumerable<string> CompareExcludedIds { get { return new[] { "lblTimestamp" }; } }

    // その画面特有の操作だけ書く
    public MessageBoxPage 登録() { return ClickAndWaitMessageBox("btnRegister"); }
}
```

### テストケース（これが設計になる）

```csharp
[TestMethod]
public void 区分を個人にすると法人番号が入力不可になり法人番号なしで登録できる()
{
    var page = メイン画面.顧客登録を開く();

    page.EnterBaseValues(new ScreenValues      // 基本値から変える項目だけ書く
    {
        { "cmbKubun", "個人" },
        { "txtCorpNo", "" },
    });
    Snap("入力後");                              // キャプチャ＋全項目の値を記録
    Check("法人番号が入力可能か", false, page.IsEnabled("txtCorpNo"));

    var message = page.登録();
    Check("メッセージ", "顧客 000001 を登録しました。", message.Message);
    message.ClickOk();
}
```

| 基底クラスの主な機能 | |
|---|---|
| `EnterBaseValues(差分)` | 基本値を全部入力し、差分の項目だけ変える（レポートには差分だけ出る） |
| `Set` / `Type` / `Get` / `IsEnabled` | 1項目の入力（値の設定 / 打鍵）・読み取り・有効かどうか |
| `Press` / `Click` | ボタン押下（Invoke / マウス。ダイアログや画面が開くボタンは Click） |
| `ClickAndWaitMessageBox` / `OpenScreen` | メッセージボックス・別画面を開いて待つ |
| `Snap` / `Check` / `CheckEventually` | キャプチャ・確認（結果はレポートに出る） |

値の書き方: テキスト・コンボは表示文字列、チェック・ラジオは `ON` / `OFF`、`""` は空にする、`null` は触らない。
無効な項目は、すでにその値なら飛ばします（区分で無効になる項目など）。
同じ項目IDが複数ある画面は `"親ID/子ID"` や `"ID#2"` で指定できます。

## 新しい画面を追加する手順

1. 対象アプリ（VB版）を起動し、画面を開いて、基本値にしたい値を手で入力する
2. 画面から項目を吸い出して Page クラスのひな形を作る
   ```powershell
   .\tools\GuiTestTool\bin\Debug\net462\GuiTestTool.exe dump --process SampleAppVB --window CustomerForm --class 顧客登録Page --namespace SampleApp.GuiTests.Pages --out tests\SampleApp.GuiTests\Pages\顧客登録Page.cs
   ```
   項目ID・項目名（左隣のラベルから推定）・現在の値が基本値として出力されます。
3. 比較対象外の項目と、画面特有の操作（ボタン等）を書き足す
4. テストケースのクラスを作る

## VB/C# 比較の仕組み

- **項目値**: キャプチャのたびに画面の全項目（値・有効/無効・位置）を記録し、項目IDで突き合わせる。
- **画像**: 前面の画面の中身だけ（タイトルバー・枠なし）をピクセル単位で比較する。
  タイトルの「(VB)」「(C#)」の違いは比較に入りません。
- **比較対象外**: `CompareExcludedIds` の項目は、値も画像上の範囲も比較しない。
- **ノイズ対策**: 撮る前に対象を最前面に出し、マウスを画面の外へ移動し、カーソルの点滅を止める
  （フォーカスは動かしません。フォーカス移動で入力チェック等が走る画面があるため）。
- テストは「画面クラス\テスト名[_ケース番号]」、キャプチャは撮った順番で対応付ける。
- `--tolerance 8` のように色の許容差を指定できる（既定は 0 ＝ 完全一致）。

## テストデータの初期化

`gui.runsettings` の `ResetCommand` に書いたコマンドを、テスト1件ごと・アプリ起動前に実行します。

```xml
<Parameter name="ResetCommand" value="sqlcmd -S .\SQLEXPRESS -d CodeFinder -i ..\..\..\..\..\TestData\reset.sql -b" />
```

## 市販部品

- **Spread**: `CustomControls.Handlers` に `SpreadHandler` を登録すると、比較・レポートではシート全体を TSV で記録します。
  セルの入力は Page クラスで `new SpreadGrid(Find("spdMeisai")).SetCell(行, 列, 値)`。
  キー操作とクリップボードで実装したひな形のため、**実機の Spread で要検証**です。
- **ActiveReports**: プレビュー画面は画像なので値を読めません。PDF/Excel 等への出力ファイルを比較するか、
  プレビューのキャプチャで比較します（今後追加）。

## 会社環境で動かすときの注意

- **NuGet**: FlaUI・MSTest を nuget.org から取得します。つながらない場合は社内フィードかオフラインのパッケージを用意してください。
- **実行する PC**: ログインしてデスクトップが表示されている状態が必要です（画面ロック・RDP の最小化で失敗します）。
  専用の PC/VM と自動ログオンを推奨します。
- **解像度・DPI（拡大率）**: VB断面と C#断面は同じ PC・同じ設定で実行してください（違うとキャプチャが一致しません）。
- **パスの長さ**: テスト名が長いと Windows のパス長制限（260文字）に当たることがあります。`EvidenceDir` は浅い場所に。
- **カーソルの点滅**: テスト中は Windows 全体のカーソル点滅を止め、テスト後に戻します。

## 単体テストと分けて回す

```powershell
dotnet test --filter "TestCategory!=GUI"   # 単体テストだけ
dotnet test --filter "TestCategory=GUI"    # GUIテストだけ
```
`````

### 5. `docs/自動テスト方針.md`

`````markdown
# コードファインダー リニューアル GUI自動テスト 方針

作成日: 2026/09/26　／　状態: 検証中（Windows 実機での動作確認前）

---

## 1. 背景と目的

| 項目 | 内容 |
|---|---|
| 案件 | コードファインダーのリニューアル（VB.NET → C#、.NET Framework 4.6.2） |
| 現状の課題 | 入出力仕様書をリバースして IT テストを設計し、人手で打鍵する方式では、工数の増加・品質の悪化が顕在化している |
| 目的 | GUI テストを自動化し、**工数の抑制**と**品質の向上**を図る |
| 最終目標 | このリポジトリのコードを会社環境へ持ち込み、会社環境でテストを実行できる状態にする |

## 2. 対象製品の前提

| 項目 | 内容 |
|---|---|
| 移行元 / 移行先 | VB.NET（WinForms）/ C#（WinForms）、どちらも .NET Framework 4.6.2 |
| 画面数 | 約250画面。テキストボックスが数百ある画面もある |
| 市販部品 | **SPREAD**（表）、**ActiveReports**（帳票） |
| 画面以外の出力 | あり（帳票・ファイル・DB） |
| DB | SQL Server |
| データの取り込み | プロセス起動モード（EXE に引数を渡して起動すると取り込みが走る）。Eファイル・Fファイルの単体ファイルが対象 |
| テストデータ | DB を初期状態に戻すことができる |

## 3. 運用の流れ（決定事項）

```
① Claude が VB断面の画面を見てテストを作る
② VB断面にテストを実行する（キャプチャ・全項目の値を記録）
     → 全件合格 ＝「テストが正しい」ことの確認。不合格ならテスト側を直す
③ C#断面に同じテストを実行する
     → 不合格 ＝ 移行の欠陥候補。C#側を直す（テストの期待値は変えない）
④ VB断面と C#断面のキャプチャ・項目値をツールで自動比較する（compare.html）
⑤ 人は「差異あり」と判定されたものを中心にレビューする（report.html / compare.html）
⑥ リニューアル完了後は、C#断面だけに実行する回帰テストとして使い続ける（VB との比較はしない）
```

- テストコードは VB・C# で共通。対象は設定（`gui.runsettings` の `Target` = `VB` / `CS`）で切り替える。
- VB断面と C#断面は別々に実行する（同じ PC・同じ画面設定で）。

## 4. テスト設計の方針（決定事項）

### 4.1 テストコードを「設計」とする

- テストケースは **Claude が作るテストコードそのものを正（設計）** とし、Excel のテスト設計書との二重管理はしない。
- 人が読めるように、テスト名・画面名・操作名は日本語で書く。
- 人向けの資料（手順・入力値・確認結果・キャプチャ）は、**実行結果から HTML レポートとして自動で作る**。
  これが「テスト仕様書 兼 結果報告書」になる。
- 客先・品質保証部門から Excel の成果物を求められた場合は、実行結果（record.json）から出力する仕組みを追加する。

### 4.2 入力データは「基本値 ＋ 差分」

- Excel 駆動（1列に1テキストボックス）は、数百項目の画面で保守できないため**採用しない**。
- 画面ごとに「全項目を正しく入力した状態」を **基本値** として1つ定義し、
  テストケースには **そのケースで変える項目だけ** を書く。
- 基本値は人が書かない。ツール（`GuiTestTool dump`）で VB 画面から吸い出して作る。
- レポートには「基本値を入力」と「基本値から変えた項目」だけが出るので、レビューしやすい。

### 4.3 画面クラス（Page）の構成

- 全画面共通の基底クラス（`ScreenPageBase`）に、入力・読み取り・ボタン・画面遷移・メッセージボックス操作をまとめる。
- 画面ごとのクラスに書くのは次の3つだけ。
  1. 基本値
  2. 比較対象外の項目（日時・採番など、実行のたびに変わるもの）
  3. その画面特有の操作（登録ボタン、別画面を開く、など）
- **部品の変数は定義しない**。部品は項目ID（AutomationId ＝ WinForms の Name）の文字列で指定する。
- 部品の種類（テキスト・コンボ・チェック・ラジオ）に応じた扱い分けは基底クラスが自動で行う。

### 4.4 画面遷移

- 複雑な遷移をしないとたどり着けない画面は、遷移の手順を **`画面遷移` クラスに1か所だけ** 書く。
- テストクラスの前処理（`[TestInitialize]`）でその画面まで進め、各テストケースには遷移を書かない。
- 途中の画面が変わったときは `画面遷移` だけ直す。
- テストケースごとに「起動 → 遷移」は毎回実行する（ケース同士が影響し合わないように）。
  実行時間が問題になったら、アプリを閉じずに使い回す方式を別途検討する。

### 4.5 移行チームへのルール（必須）

> **C# へ移行するとき、フォームとコントロールの Name を VB版と同じにすること。**

テストは Name（＝AutomationId）で部品を特定するため、Name が変わるとテストが動かない。
やむを得ず変える場合は、新旧の対応表を作って共有すること。

### 4.6 テストの範囲と網羅基準（提案：ユーザーの合意待ち）

> この節は提案段階。会社環境で画面一覧・VB のソース構成を見たうえで、ユーザーと確定させる。

**単体テストと画面テストの切り分け**

| | 単体テスト | 画面テスト（GUI 自動テスト） |
|---|---|---|
| 確認すること | 計算・判定・変換のロジックそのもの | 入力 → 処理 → 表示・出力が画面でつながっているか |
| 例（実績点検） | 点検ルールの判定、点数・日数の計算、日付の扱い、E/F ファイルの解析 | 取り込み → 点検 → 結果の表示、エラー時のメッセージ、帳票出力の起動 |
| ケース数 | 多い（組み合わせ・境界値をすべて） | 少なめ（代表値） |

- 目安: 「同じ画面操作で入力値だけ変えて何十通りも試したい」ものは単体テストに回す。
- ロジックがクラスライブラリ（DLL）に分かれていれば、C# のテストから VB版の DLL も呼べる。
  同じ入力を VB版・C#版の両方に渡して結果を突き合わせるテスト（画面を通さない VB/C# 比較）を作れる。
  組み合わせの多い点検ルールは、この方法を優先する。ロジックがフォームのコードにある場合は画面テストで代表値を確認する。

**画面テストの網羅基準**

コードのカバレッジ（全行・全分岐）は画面テストの目標にしない。**画面仕様の網羅** を基準にする。
一覧は VB のソースから機械的に作り、網羅状況を数えて報告する。

| 観点 | 基準 | 一覧の作り方 |
|---|---|---|
| 入力項目 | 項目ごとに正常1件＋入力チェックの種類ごとに異常1件 | `.Designer.vb` の項目と入力チェック処理 |
| メッセージ | VB のソースにあるメッセージを全件1回ずつ表示させる | `MessageBox.Show` を検索 |
| ボタン・操作 | 全ボタンを1回以上押す | `.Designer.vb` とクリック処理 |
| 画面遷移 | 開く・閉じる・戻るの全経路を1回ずつ | `Show` / `ShowDialog` を検索 |
| 状態の変化 | 区分などで有効/無効・表示が変わるパターンを各1回 | `Enabled` / `Visible` の切り替え処理 |
| 出力 | 帳票・ファイル・DB 更新の種類ごとに1回 | 出力処理 |

**移行で壊れやすい箇所（テストデータに必ず含める）**

- 空欄・未入力（VB の `Nothing` と空文字の扱いの違い）
- 小数の丸め（VB の `CInt` は .5 を偶数側へ丸める。2.5 → 2）
- 割り算（VB の `\` と `/` の使い分け）
- 文字列の比較・切り出し（大文字小文字・全角半角、`Mid` / `Left` の範囲外）
- 日付（書式、月末・うるう年、和暦）

**画面のランク分け（深さを変える）**

| ランク | 対象の例 | 確認の深さ |
|---|---|---|
| A | 取り込み・点検・出力など業務の中心 | 網羅基準をすべて＋移行で壊れやすい箇所 |
| B | 通常の入力・更新画面 | 入力項目・メッセージ・遷移を網羅 |
| C | 参照だけのマスタ画面など | 開く・表示する・閉じる |

ランクはユーザーが画面一覧を見て決める。

## 5. エビデンス（キャプチャ）の方針（決定事項）

キャプチャ1回で、次の3つを保存する。

| 種類 | 撮る範囲 | 用途 |
|---|---|---|
| 人が見る用 | 前面の画面＋親画面（タイトルバー込み） | レビュー（どの画面か分かるように） |
| 比較用 | **前面の画面の中身部分だけ**（タイトルバー・枠なし）。ダイアログが出ていればダイアログだけ | VB/C# の画像比較 |
| 全項目の値 | 前面の画面の全項目（値・有効/無効・位置） | VB/C# の値比較、レポートでの確認 |

- テストが失敗したときは、失敗時の画面とデスクトップ全体を自動で保存する。
- レビュー用の出力は **HTML**（`Evidence\実行日時\report.html`）。
- エビデンスは量が多いため Git の管理対象外（`.gitignore` の `Evidence/`）。残す場合は別の保管場所へコピーする。

## 6. VB/C# 比較の方針（決定事項）

- **項目値の比較を主**、**画像の比較を補助** とする。
  - 項目値: 項目IDで突き合わせ、値・有効/無効・部品の種類・位置の違いを出す。
  - 画像: 比較用画像をピクセル単位で比べ、差異を赤く塗った差分画像を作る。
- 比較対象外の項目（`CompareExcludedIds`）は、値も画像上の範囲も比較しない。
- 結果は `compare.html` に「VB｜C#｜差分画像」を横に並べて出す。既定で差異のあるものだけ表示する。
- **人は差異のあるものだけを見る**運用にして、全件を目で比べる工数をなくす。
- Claude による画像レビュー（外部 API へ画面キャプチャを送る）は、社内のセキュリティ規定を確認するまで**行わない**。

### ノイズ対策

| ノイズ | 対策 |
|---|---|
| タイトルの「(VB)」「(C#)」の違い、フォーカスによる枠の色 | 比較用画像はタイトルバーを含めない |
| マウスを重ねたときのボタンの色 | 撮る前にマウスを画面の外へ移動する |
| カーソルの点滅 | テスト中は Windows のカーソル点滅を止める（テスト後に元へ戻す） |
| ほかのウィンドウの写り込み | 撮る前に対象の画面を最前面に出す |
| 日時・採番・ユーザー名 | 比較対象外に指定する |
| 画面サイズの違い | 差異として報告する（移行の欠陥の可能性があるため） |

> 当初は「撮る前にフォーカスを入力欄以外へ移す」予定だったが、**フォーカスは動かさない**方針に変更した。
> 業務画面ではフォーカス移動だけで入力チェック等のイベントが走り、画面の状態が変わってしまうため。

## 7. テストデータの方針（決定事項）

### 7.1 基本の考え方

入力値と同じく **「基本データ ＋ ケースごとの差分」** で用意する。

| データ | 用意の仕方 |
|---|---|
| DB（SQL Server） | テスト1件ごとに **決まった初期状態へ戻す**。ケースごとの違いは DB に直接書かず、ファイルの取り込みで作る |
| 取り込むファイル（Eファイル・Fファイル・患者データ等） | **基本ファイルを1組だけ用意** し、ケースごとに **変える箇所だけ** をテストコードに書く。テストのたびに基本ファイル＋差分から作り直す |

ケースごとにファイル一式を丸ごと持つことはしない（列が多く、基本が変わったときに全ケースを直すことになるため）。

### 7.2 テスト1件の流れ

```
① DB を初期状態に戻す            … ResetCommand（sqlcmd 等）
② 取り込むファイルを作る          … 基本ファイルをコピー → そのケースの差分を反映
③ 取り込みを実行する              … プロセス起動モード（EXE に引数を渡して起動）。終了を待ち、終了コードを確認
④ 画面を起動し、対象画面まで遷移   … 画面遷移クラス（4.4）
⑤ 操作・確認・キャプチャ
```

- ①〜③は **テストの前提準備** としてテストクラスの `[TestInitialize]`、またはテストケースの先頭で行う。
- ③の取り込みは、**VB断面なら VB の EXE、C#断面なら C# の EXE** で実行する（取り込み処理も移行の対象なので、両方で同じ結果になることを確認する）。
  EXE のパスと引数の形は runsettings に持たせる。
- 取り込みに失敗した（終了コードが 0 以外・時間切れ）場合は、そのテストを不合格にする。

### 7.3 基本ファイルの置き場所と作り方

```
TestData└─ 実績点検\                 … 画面（または業務）ごと
    ├─ 基本\E.txt            … 正しい内容の Eファイル（架空の患者で作る）
    ├─ 基本\F.txt            … 正しい内容の Fファイル
    └─ 基本\患者.txt          … 患者データ（形式は製品に合わせる）
```

- 基本ファイルは「点検でエラーが1件も出ない、正しいデータ」にする。ケースはそこから崩して作る。
- 差分が大きくなりすぎる場合（入院と外来で内容がまったく違う、など）は、基本を複数持ってよい（`基本_入院\`、`基本_外来\`）。
- 基本ファイルは Git で管理する。

### 7.4 差分の書き方

テストコードに次の3種類だけを書く。

| 操作 | 例 |
|---|---|
| 項目を変える | Eファイルの「患者ID=P0001 の行」の「退院年月日」を 20260931 にする |
| 行を足す | Fファイルに「P0001 の行をコピーして、行為明細番号と点数を変えた行」を足す |
| 行を消す | Eファイルから「患者ID=P0002 の行」を消す |

- 行は **行番号ではなく、キー項目（患者ID・データ識別番号・入退院年月日など）で指定** する。基本ファイルに行を足しても、既存のケースが壊れないようにするため。
- 列は **列名で指定** する。Eファイル・Fファイルの列の定義（列の並びと名前）は、共通部品に1回だけ書く。
- 書き出すときは、**文字コード・区切り文字・改行コードなど、変えた項目以外は元のファイルと1バイトも変えない**。
  （差分なしで書き出すと元のファイルと完全一致する、ことを部品のテストで確認する）

イメージ（実装は会社環境で行う）:

```csharp
[TestMethod]
public void 退院年月日が不正な場合は点検エラーになる()
{
    var data = テストデータ.基本("実績点検");
    data.E.変更(キー: "P0001", 列: "退院年月日", 値: "20260931");
    取り込み(data);                    // ファイル作成 → プロセス起動モードで取り込み → エビデンスに保存

    var page = メイン画面.実績点検画面へ();
    page.点検();
    Snap("点検結果");
    Check("点検結果", "退院年月日エラー", page.結果(1));
}
```

### 7.5 エビデンスとレポート

- ケースごとに作ったファイル（E・F・患者データ）は、そのテストのエビデンスフォルダに保存する。**何を取り込ませたかを後から確認できるようにする**。
- レポートには、基本ファイルからの差分を「Eファイル P0001 の 退院年月日: 20260915 → 20260931」の形で出す。
- 取り込みの実行結果（終了コード・かかった時間・取り込み処理が出すログがあればそのパス）もレポートに出す。

### 7.6 DB の初期化

- 初期化は runsettings の `ResetCommand` で行う（例: `sqlcmd -S <サーバー> -d <DB名> -i TestData\reset.sql -b`）。
- テストの件数が多いので、**初期化にかかる時間** に注意する。遅い場合は次のどれかを検討する。
  - 必要なテーブルだけを消して入れ直す SQL にする
  - SQL Server のデータベーススナップショットから戻す
- VB断面と C#断面で **同じ DB・同じ初期状態** を使う。

### 7.7 VB/C# の比較をぶらさないための注意

- テストで使うデータ（DB の初期状態・基本ファイル・差分）は VB断面・C#断面で完全に同じにする。
- 処理結果が **その日の日付** に左右される機能（年齢計算、期限切れ判定など）がある場合、VB と C# を別の日に実行すると結果がずれる。
  該当する画面が分かったら、日付を固定する方法（データ側の日付を実行日から相対で作る、など）を個別に決める。

### 7.8 出力（帳票・ファイル）

- 出力のあるケースでは、出力ファイルをエビデンスに保存し、VB/C# で比較する。
- 比較の細かい方法（日時など毎回変わる箇所の除外、PDF の扱いなど）は、画面ごとにケースバイケースで決める。

## 8. 市販部品の方針

| 部品 | 方針 | 状態 |
|---|---|---|
| SPREAD | UI Automation に頼らず、キー操作でセル入力・クリップボード経由で読み取る。比較ではシート全体を TSV で記録する | ひな形あり（**実機で要検証**） |
| ActiveReports | プレビューは画像なので値は読めない。PDF・Excel 等への出力ファイルを比較する。補助としてプレビュー画像も比較する | **未実装** |
| 画面以外の出力（ファイル・DB） | 出力ファイルの内容・DB の更新結果を比較対象にする | **未実装** |

## 9. 実行環境の要件

| 項目 | 内容 |
|---|---|
| OS | Windows（ログインしてデスクトップが表示された状態）。画面ロック・RDP の最小化で失敗する |
| 推奨 | テスト専用の PC/VM ＋ 自動ログオン |
| 画面設定 | VB断面と C#断面は **同じ PC・同じ解像度・同じ拡大率(DPI)** で実行する |
| 開発環境 | Visual Studio 2022 または .NET SDK（`dotnet build` / `dotnet test` が使えること） |
| NuGet | FlaUI.UIA3 4.0.0、MSTest 3.6.4 等を取得できること（社内フィード or オフラインパッケージ） |
| 並列実行 | しない（画面・マウスを奪い合うため） |
| パスの長さ | Windows の 260文字制限に注意。エビデンスの保存先は浅い場所にする |

## 10. 未検証・未実装の一覧

作成環境（Linux）ではビルドと比較ツール・レポートの動作までしか確認できていない。

| # | 項目 | 状態 |
|---|---|---|
| 1 | サンプルで `run-gui-tests.ps1 -Target Both` が全件合格すること | 未検証 |
| 2 | WinForms のコンボボックス選択・ラジオボタン ON・メッセージボックス検出 | 未検証 |
| 3 | カーソル点滅の停止・復元 | 未検証 |
| 4 | `GuiTestTool dump` による Page クラスのひな形生成 | 未検証 |
| 5 | 高DPI（125%等）環境でのキャプチャ位置 | 未検証 |
| 6 | SPREAD の読み書き | 未検証 |
| 7 | ActiveReports・出力ファイル・DB の比較 | 未実装 |
| 8 | 実製品（コードファインダー）への適用 | 未着手 |
| 9 | テストデータ部品（基本ファイル＋差分、プロセス起動モードでの取り込み） | 未実装（方針のみ。7章） |

## 11. 用語

| 用語 | 意味 |
|---|---|
| VB断面 / C#断面 | 移行前（VB.NET版）/ 移行後（C#版）の EXE |
| 項目ID | 部品の AutomationId。WinForms ではコントロールの Name |
| 基本値 | 画面の全項目を正しく入力した状態の値 |
| 比較対象外 | 実行のたびに変わるため、VB/C# の比較から外す項目 |
| エビデンス | キャプチャ・項目値・操作と確認の記録（`Evidence\` 配下） |
`````

### 6. `run-gui-tests.ps1`

`````powershell
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
`````

### 7. `src/SampleApp/CustomerForm.cs`

`````csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SampleApp
{
    /// <summary>
    /// テスト対象の登録画面（C#版）。入力項目が多い業務画面の縮小版。
    /// テキストボックス・コンボボックス・チェックボックス・ラジオボタン・表示日時（比較対象外にする項目）を含む。
    /// ※ Name がそのまま AutomationId になるので、VB版と同じ Name にしておくこと。
    /// </summary>
    public class CustomerForm : Form
    {
        private readonly TextBox txtCode = new TextBox { Name = "txtCode", Location = new Point(120, 20), Width = 100 };
        private readonly TextBox txtName = new TextBox { Name = "txtName", Location = new Point(120, 50), Width = 220 };
        private readonly TextBox txtKana = new TextBox { Name = "txtKana", Location = new Point(120, 80), Width = 220 };
        private readonly ComboBox cmbKubun = new ComboBox { Name = "cmbKubun", Location = new Point(120, 110), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtCorpNo = new TextBox { Name = "txtCorpNo", Location = new Point(120, 140), Width = 150 };
        private readonly TextBox txtTel = new TextBox { Name = "txtTel", Location = new Point(120, 170), Width = 150 };
        private readonly TextBox txtCreditLimit = new TextBox { Name = "txtCreditLimit", Location = new Point(120, 200), Width = 150, TextAlign = HorizontalAlignment.Right };
        private readonly CheckBox chkActive = new CheckBox { Name = "chkActive", Text = "取引中", Location = new Point(120, 230), AutoSize = true };
        private readonly GroupBox grpPay = new GroupBox { Name = "grpPay", Text = "支払方法", Location = new Point(20, 260), Size = new Size(320, 50) };
        private readonly RadioButton rdoPayCash = new RadioButton { Name = "rdoPayCash", Text = "現金", Location = new Point(20, 20), AutoSize = true };
        private readonly RadioButton rdoPayTransfer = new RadioButton { Name = "rdoPayTransfer", Text = "振込", Location = new Point(120, 20), AutoSize = true };
        private readonly Label lblStatus = new Label { Name = "lblStatus", Text = "", Location = new Point(20, 325), AutoSize = true };
        private readonly Label lblTimestamp = new Label { Name = "lblTimestamp", Location = new Point(20, 350), AutoSize = true };
        private readonly Button btnRegister = new Button { Name = "btnRegister", Text = "登録", Location = new Point(180, 345), Width = 75 };
        private readonly Button btnClose = new Button { Name = "btnClose", Text = "閉じる", Location = new Point(265, 345), Width = 75 };

        public CustomerForm()
        {
            Name = "CustomerForm";
            Text = "顧客登録（C#）";
            ClientSize = new Size(360, 385);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            cmbKubun.Items.AddRange(new object[] { "法人", "個人" });
            cmbKubun.SelectedIndex = 0;
            chkActive.Checked = true;
            rdoPayTransfer.Checked = true;
            grpPay.Controls.AddRange(new Control[] { rdoPayCash, rdoPayTransfer });

            // 実行するたびに変わる値（テストでは比較対象外に指定する例）
            lblTimestamp.Text = "表示日時: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            cmbKubun.SelectedIndexChanged += CmbKubun_SelectedIndexChanged;
            btnRegister.Click += BtnRegister_Click;
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                NewLabel("顧客コード", 23), txtCode,
                NewLabel("顧客名", 53), txtName,
                NewLabel("顧客名カナ", 83), txtKana,
                NewLabel("区分", 113), cmbKubun,
                NewLabel("法人番号", 143), txtCorpNo,
                NewLabel("電話番号", 173), txtTel,
                NewLabel("与信限度額", 203), txtCreditLimit,
                chkActive, grpPay, lblStatus, lblTimestamp, btnRegister, btnClose,
            });
        }

        private static Label NewLabel(string text, int y)
        {
            return new Label { Text = text, Location = new Point(20, y), AutoSize = true };
        }

        private void CmbKubun_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 個人のときは法人番号を入力できない
            var isCorp = (string)cmbKubun.SelectedItem == "法人";
            txtCorpNo.Enabled = isCorp;
            if (!isCorp) txtCorpNo.Clear();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            long creditLimit;
            string error = null;
            Control errorControl = null;

            if (!IsDigits(txtCode.Text, 6))
            {
                error = "顧客コードは6桁の数字で入力してください。";
                errorControl = txtCode;
            }
            else if (txtName.Text.Trim().Length == 0)
            {
                error = "顧客名を入力してください。";
                errorControl = txtName;
            }
            else if (txtCorpNo.Enabled && !IsDigits(txtCorpNo.Text, 13))
            {
                error = "法人番号は13桁の数字で入力してください。";
                errorControl = txtCorpNo;
            }
            else if (!long.TryParse(txtCreditLimit.Text, out creditLimit) || creditLimit < 0 || creditLimit > 99999999)
            {
                error = "与信限度額は0～99999999の数値で入力してください。";
                errorControl = txtCreditLimit;
            }

            if (error != null)
            {
                MessageBox.Show(this, error, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                errorControl.Focus();
                return;
            }

            MessageBox.Show(this, "顧客 " + txtCode.Text + " を登録しました。", "登録完了",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblStatus.Text = "登録済み: " + txtCode.Text;
        }

        private static bool IsDigits(string text, int length)
        {
            if (text.Length != length) return false;
            foreach (var c in text)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }
    }
}
`````

### 8. `src/SampleApp/MainForm.cs`

`````csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SampleApp
{
    /// <summary>
    /// テスト対象の小さな画面（C#版）。2つの数値を足し算して結果を表示する。
    /// ※ WinForms ではコントロールの Name プロパティが UI Automation の AutomationId になる。
    ///    テストはこの Name（txtA, btnCalc など）で部品を特定するので、変更しないこと。
    /// </summary>
    public class MainForm : Form
    {
        private readonly TextBox txtA = new TextBox { Name = "txtA", Location = new Point(90, 20), Width = 170 };
        private readonly TextBox txtB = new TextBox { Name = "txtB", Location = new Point(90, 55), Width = 170 };
        private readonly Button btnCalc = new Button { Name = "btnCalc", Text = "計算", Location = new Point(90, 95), Width = 80 };
        private readonly Button btnClear = new Button { Name = "btnClear", Text = "クリア", Location = new Point(180, 95), Width = 80 };
        private readonly Label lblResult = new Label { Name = "lblResult", Text = "結果: -", Location = new Point(20, 140), AutoSize = true };
        private readonly Button btnCustomer = new Button { Name = "btnCustomer", Text = "顧客登録...", Location = new Point(90, 175), Width = 170 };

        public MainForm()
        {
            Name = "MainForm";
            Text = "計算サンプル（C#）";
            ClientSize = new Size(290, 215);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var lblA = new Label { Text = "数値A", Location = new Point(20, 23), AutoSize = true };
            var lblB = new Label { Text = "数値B", Location = new Point(20, 58), AutoSize = true };

            btnCalc.Click += BtnCalc_Click;
            btnClear.Click += BtnClear_Click;
            btnCustomer.Click += BtnCustomer_Click;

            Controls.AddRange(new Control[] { lblA, txtA, lblB, txtB, btnCalc, btnClear, lblResult, btnCustomer });
            AcceptButton = btnCalc;
        }

        private void BtnCalc_Click(object sender, EventArgs e)
        {
            long a, b;
            if (!long.TryParse(txtA.Text, out a) || !long.TryParse(txtB.Text, out b))
            {
                MessageBox.Show(this, "数値を入力してください。", "入力エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblResult.Text = "結果: " + (a + b);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtA.Clear();
            txtB.Clear();
            lblResult.Text = "結果: -";
            txtA.Focus();
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            // 別画面をモーダルで開く（業務アプリで多い画面遷移の例）
            using (var form = new CustomerForm())
            {
                form.ShowDialog(this);
            }
        }
    }
}
`````

### 9. `src/SampleApp/Program.cs`

`````csharp
using System;
using System.Windows.Forms;

namespace SampleApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
`````

### 10. `src/SampleApp/SampleApp.csproj`

`````xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net462</TargetFramework>
    <RootNamespace>SampleApp</RootNamespace>
    <AssemblyName>SampleApp</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="System.Drawing" />
    <Reference Include="System.Windows.Forms" />
  </ItemGroup>

  <ItemGroup>
    <!-- .NET Framework 4.6.2 の参照アセンブリ（Targeting Pack 未インストールでもビルドできるように） -->
    <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
  </ItemGroup>

</Project>
`````

### 11. `src/SampleAppVB/CustomerForm.vb`

`````vbnet
Imports System
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' テスト対象の登録画面（VB版）。C#版と同じ部品名(Name)・同じ動きにしてある。
''' </summary>
Public Class CustomerForm
    Inherits Form

    Private ReadOnly txtCode As New TextBox With {.Name = "txtCode", .Location = New Point(120, 20), .Width = 100}
    Private ReadOnly txtName As New TextBox With {.Name = "txtName", .Location = New Point(120, 50), .Width = 220}
    Private ReadOnly txtKana As New TextBox With {.Name = "txtKana", .Location = New Point(120, 80), .Width = 220}
    Private ReadOnly cmbKubun As New ComboBox With {.Name = "cmbKubun", .Location = New Point(120, 110), .Width = 100, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly txtCorpNo As New TextBox With {.Name = "txtCorpNo", .Location = New Point(120, 140), .Width = 150}
    Private ReadOnly txtTel As New TextBox With {.Name = "txtTel", .Location = New Point(120, 170), .Width = 150}
    Private ReadOnly txtCreditLimit As New TextBox With {.Name = "txtCreditLimit", .Location = New Point(120, 200), .Width = 150, .TextAlign = HorizontalAlignment.Right}
    Private ReadOnly chkActive As New CheckBox With {.Name = "chkActive", .Text = "取引中", .Location = New Point(120, 230), .AutoSize = True}
    Private ReadOnly grpPay As New GroupBox With {.Name = "grpPay", .Text = "支払方法", .Location = New Point(20, 260), .Size = New Size(320, 50)}
    Private ReadOnly rdoPayCash As New RadioButton With {.Name = "rdoPayCash", .Text = "現金", .Location = New Point(20, 20), .AutoSize = True}
    Private ReadOnly rdoPayTransfer As New RadioButton With {.Name = "rdoPayTransfer", .Text = "振込", .Location = New Point(120, 20), .AutoSize = True}
    Private ReadOnly lblStatus As New Label With {.Name = "lblStatus", .Text = "", .Location = New Point(20, 325), .AutoSize = True}
    Private ReadOnly lblTimestamp As New Label With {.Name = "lblTimestamp", .Location = New Point(20, 350), .AutoSize = True}
    Private ReadOnly btnRegister As New Button With {.Name = "btnRegister", .Text = "登録", .Location = New Point(180, 345), .Width = 75}
    Private ReadOnly btnClose As New Button With {.Name = "btnClose", .Text = "閉じる", .Location = New Point(265, 345), .Width = 75}

    Public Sub New()
        Me.Name = "CustomerForm"
        Me.Text = "顧客登録（VB）"
        Me.ClientSize = New Size(360, 385)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterParent

        cmbKubun.Items.AddRange(New Object() {"法人", "個人"})
        cmbKubun.SelectedIndex = 0
        chkActive.Checked = True
        rdoPayTransfer.Checked = True
        grpPay.Controls.AddRange(New Control() {rdoPayCash, rdoPayTransfer})

        ' 実行するたびに変わる値（テストでは比較対象外に指定する例）
        lblTimestamp.Text = "表示日時: " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

        AddHandler cmbKubun.SelectedIndexChanged, AddressOf CmbKubun_SelectedIndexChanged
        AddHandler btnRegister.Click, AddressOf BtnRegister_Click
        AddHandler btnClose.Click, Sub(s, e) Me.Close()

        Me.Controls.AddRange(New Control() {
            NewLabel("顧客コード", 23), txtCode,
            NewLabel("顧客名", 53), txtName,
            NewLabel("顧客名カナ", 83), txtKana,
            NewLabel("区分", 113), cmbKubun,
            NewLabel("法人番号", 143), txtCorpNo,
            NewLabel("電話番号", 173), txtTel,
            NewLabel("与信限度額", 203), txtCreditLimit,
            chkActive, grpPay, lblStatus, lblTimestamp, btnRegister, btnClose})
    End Sub

    Private Shared Function NewLabel(text As String, y As Integer) As Label
        Return New Label With {.Text = text, .Location = New Point(20, y), .AutoSize = True}
    End Function

    Private Sub CmbKubun_SelectedIndexChanged(sender As Object, e As EventArgs)
        ' 個人のときは法人番号を入力できない
        Dim isCorp As Boolean = CStr(cmbKubun.SelectedItem) = "法人"
        txtCorpNo.Enabled = isCorp
        If Not isCorp Then txtCorpNo.Clear()
    End Sub

    Private Sub BtnRegister_Click(sender As Object, e As EventArgs)
        Dim creditLimit As Long
        Dim errorMessage As String = Nothing
        Dim errorControl As Control = Nothing

        If Not IsDigits(txtCode.Text, 6) Then
            errorMessage = "顧客コードは6桁の数字で入力してください。"
            errorControl = txtCode
        ElseIf txtName.Text.Trim().Length = 0 Then
            errorMessage = "顧客名を入力してください。"
            errorControl = txtName
        ElseIf txtCorpNo.Enabled AndAlso Not IsDigits(txtCorpNo.Text, 13) Then
            errorMessage = "法人番号は13桁の数字で入力してください。"
            errorControl = txtCorpNo
        ElseIf Not Long.TryParse(txtCreditLimit.Text, creditLimit) OrElse creditLimit < 0 OrElse creditLimit > 99999999 Then
            errorMessage = "与信限度額は0～99999999の数値で入力してください。"
            errorControl = txtCreditLimit
        End If

        If errorMessage IsNot Nothing Then
            MessageBox.Show(Me, errorMessage, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            errorControl.Focus()
            Return
        End If

        MessageBox.Show(Me, "顧客 " & txtCode.Text & " を登録しました。", "登録完了",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
        lblStatus.Text = "登録済み: " & txtCode.Text
    End Sub

    Private Shared Function IsDigits(text As String, length As Integer) As Boolean
        If text.Length <> length Then Return False
        For Each c As Char In text
            If c < "0"c OrElse c > "9"c Then Return False
        Next
        Return True
    End Function

End Class
`````

### 12. `src/SampleAppVB/MainForm.vb`

`````vbnet
Imports System
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' テスト対象の小さな画面（VB版）。C#版と同じ部品名(Name)にしてあるので、
''' 同じテストコード（Page Object）でそのまま操作できる。
''' </summary>
Public Class MainForm
    Inherits Form

    Private ReadOnly txtA As New TextBox With {.Name = "txtA", .Location = New Point(90, 20), .Width = 170}
    Private ReadOnly txtB As New TextBox With {.Name = "txtB", .Location = New Point(90, 55), .Width = 170}
    Private ReadOnly btnCalc As New Button With {.Name = "btnCalc", .Text = "計算", .Location = New Point(90, 95), .Width = 80}
    Private ReadOnly btnClear As New Button With {.Name = "btnClear", .Text = "クリア", .Location = New Point(180, 95), .Width = 80}
    Private ReadOnly lblResult As New Label With {.Name = "lblResult", .Text = "結果: -", .Location = New Point(20, 140), .AutoSize = True}
    Private ReadOnly btnCustomer As New Button With {.Name = "btnCustomer", .Text = "顧客登録...", .Location = New Point(90, 175), .Width = 170}

    Public Sub New()
        Me.Name = "MainForm"
        Me.Text = "計算サンプル（VB）"
        Me.ClientSize = New Size(290, 215)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen

        Dim lblA As New Label With {.Text = "数値A", .Location = New Point(20, 23), .AutoSize = True}
        Dim lblB As New Label With {.Text = "数値B", .Location = New Point(20, 58), .AutoSize = True}

        AddHandler btnCalc.Click, AddressOf BtnCalc_Click
        AddHandler btnClear.Click, AddressOf BtnClear_Click
        AddHandler btnCustomer.Click, AddressOf BtnCustomer_Click

        Me.Controls.AddRange(New Control() {lblA, txtA, lblB, txtB, btnCalc, btnClear, lblResult, btnCustomer})
        Me.AcceptButton = btnCalc
    End Sub

    Private Sub BtnCalc_Click(sender As Object, e As EventArgs)
        Dim a As Long
        Dim b As Long
        If Not Long.TryParse(txtA.Text, a) OrElse Not Long.TryParse(txtB.Text, b) Then
            MessageBox.Show(Me, "数値を入力してください。", "入力エラー",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        lblResult.Text = "結果: " & (a + b).ToString()
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        txtA.Clear()
        txtB.Clear()
        lblResult.Text = "結果: -"
        txtA.Focus()
    End Sub

    Private Sub BtnCustomer_Click(sender As Object, e As EventArgs)
        ' 別画面をモーダルで開く（業務アプリで多い画面遷移の例）
        Using form As New CustomerForm()
            form.ShowDialog(Me)
        End Using
    End Sub

End Class
`````

### 13. `src/SampleAppVB/Program.vb`

`````vbnet
Imports System
Imports System.Windows.Forms

Module Program

    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New MainForm())
    End Sub

End Module
`````

### 14. `src/SampleAppVB/SampleAppVB.vbproj`

`````xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net462</TargetFramework>
    <RootNamespace>SampleAppVB</RootNamespace>
    <AssemblyName>SampleAppVB</AssemblyName>
    <StartupObject>SampleAppVB.Program</StartupObject>
    <MyType>Empty</MyType>
    <OptionExplicit>On</OptionExplicit>
    <OptionStrict>On</OptionStrict>
    <OptionInfer>On</OptionInfer>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="System.Drawing" />
    <Reference Include="System.Windows.Forms" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
  </ItemGroup>

</Project>
`````

### 15. `tests/GuiTestKit/Automation/ControlAccess.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using GuiTestKit.Evidence;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 部品の種類ごとの値の読み書きと、画面の全項目の吸い出し。
    /// ScreenPageBase・キャプチャ・吸い出しツール（GuiTestTool dump）で共通に使う。
    /// </summary>
    public static class ControlAccess
    {
        /// <summary>中を辿らずに、それ自体を1項目として扱う部品。</summary>
        private static readonly HashSet<ControlType> LeafTypes = new HashSet<ControlType>
        {
            ControlType.Edit, ControlType.ComboBox, ControlType.CheckBox, ControlType.RadioButton,
            ControlType.Text, ControlType.Button, ControlType.SplitButton, ControlType.Hyperlink,
            ControlType.Spinner, ControlType.Slider, ControlType.ProgressBar,
            ControlType.Table, ControlType.DataGrid, ControlType.List, ControlType.Tree, ControlType.Document,
        };

        /// <summary>値を入力できる部品（基本値の対象）。</summary>
        public static bool IsInputType(string type)
        {
            return type == "Edit" || type == "ComboBox" || type == "CheckBox" || type == "RadioButton" || type == "Spinner";
        }

        // ------------------------------------------------------------------
        // 読み取り
        // ------------------------------------------------------------------

        public static string Read(AutomationElement element, IEnumerable<ICustomControlHandler> handlers)
        {
            var handler = FindHandler(element, handlers);
            if (handler != null) return handler.Read(element);

            try
            {
                switch (element.ControlType)
                {
                    case ControlType.Edit:
                        if (element.Properties.IsPassword.ValueOrDefault) return "(パスワード)";
                        return ReadValuePattern(element) ?? element.Name;

                    case ControlType.ComboBox:
                        var combo = element.AsComboBox();
                        var value = ReadValuePattern(element);
                        if (!string.IsNullOrEmpty(value)) return value;
                        var selected = combo.SelectedItem;
                        return selected == null ? string.Empty : selected.Text;

                    case ControlType.CheckBox:
                        var state = element.AsCheckBox().ToggleState;
                        return state == ToggleState.On ? ScreenValues.ON : state == ToggleState.Off ? ScreenValues.OFF : "不定";

                    case ControlType.RadioButton:
                        return element.AsRadioButton().IsChecked ? ScreenValues.ON : ScreenValues.OFF;

                    case ControlType.Text:
                    case ControlType.Button:
                    case ControlType.SplitButton:
                    case ControlType.Hyperlink:
                        return element.Name;

                    default:
                        var generic = ReadValuePattern(element);
                        if (generic != null) return generic;
                        var range = element.Patterns.RangeValue.PatternOrDefault;
                        if (range != null) return range.Value.ValueOrDefault.ToString();
                        return "(未対応: " + element.ControlType + ")";
                }
            }
            catch (Exception ex)
            {
                return "(取得エラー: " + ex.GetType().Name + ")";
            }
        }

        private static string ReadValuePattern(AutomationElement element)
        {
            var pattern = element.Patterns.Value.PatternOrDefault;
            return pattern == null ? null : pattern.Value.ValueOrDefault;
        }

        // ------------------------------------------------------------------
        // 書き込み
        // ------------------------------------------------------------------

        public static void Write(AutomationElement element, string value, InputMode mode, IEnumerable<ICustomControlHandler> handlers)
        {
            var handler = FindHandler(element, handlers);
            if (handler != null)
            {
                handler.Write(element, value);
                return;
            }

            switch (element.ControlType)
            {
                case ControlType.Edit:
                    if (mode == InputMode.Keyboard) TypeByKeyboard(element, value);
                    else element.AsTextBox().Text = value;
                    break;

                case ControlType.ComboBox:
                    var combo = element.AsComboBox();
                    if (combo.IsEditable)
                    {
                        combo.EditableText = value;
                    }
                    else
                    {
                        var item = combo.Select(value);
                        combo.Collapse();
                        if (item == null) throw new InvalidOperationException("コンボボックスに「" + value + "」がありません。");
                    }
                    break;

                case ControlType.CheckBox:
                    element.AsCheckBox().IsChecked = ToBool(value);
                    break;

                case ControlType.RadioButton:
                    // OFF はグループ内の別のラジオボタンを ON にすることで実現するため、何もしない
                    if (!ToBool(value)) break;
                    var radio = element.AsRadioButton();
                    if (radio.Patterns.SelectionItem.IsSupported) radio.IsChecked = true;
                    else radio.Click();
                    break;

                default:
                    var pattern = element.Patterns.Value.PatternOrDefault;
                    if (pattern == null)
                    {
                        throw new NotSupportedException(element.ControlType + " には値を入力できません。ICustomControlHandler で対応してください。");
                    }
                    pattern.SetValue(value);
                    break;
            }
        }

        /// <summary>実際にキーを打鍵して入力する（キー入力・フォーカス移動のイベントで動く画面用）。</summary>
        private static void TypeByKeyboard(AutomationElement element, string value)
        {
            element.Focus();
            // 単一行の TextBox は Ctrl+A で全選択できないことがあるので Home → Shift+End で選択して消す
            Keyboard.Type(VirtualKeyShort.HOME);
            Keyboard.TypeSimultaneously(VirtualKeyShort.SHIFT, VirtualKeyShort.END);
            Keyboard.Type(VirtualKeyShort.DELETE);
            if (value.Length > 0) Keyboard.Type(value);
            Wait.UntilInputIsProcessed();
        }

        private static bool ToBool(string value)
        {
            if (value == ScreenValues.ON) return true;
            if (value == ScreenValues.OFF) return false;
            throw new ArgumentException("チェックボックス・ラジオボタンの値は ON / OFF で指定してください: " + value);
        }

        private static ICustomControlHandler FindHandler(AutomationElement element, IEnumerable<ICustomControlHandler> handlers)
        {
            return handlers == null ? null : handlers.FirstOrDefault(h => h.CanHandle(element));
        }

        // ------------------------------------------------------------------
        // 全項目の吸い出し
        // ------------------------------------------------------------------

        /// <summary>
        /// 画面（ウィンドウ）内の、AutomationId を持つ全項目の状態を読み取る。
        /// 位置は origin（比較用画像の左上）からの相対座標で記録する。
        /// 同じ AutomationId が複数あるときは2つ目以降を "ID#2", "ID#3" … とする。
        /// </summary>
        public static List<ControlSnapshot> ReadAll(AutomationElement root, IEnumerable<ICustomControlHandler> handlers, Point origin)
        {
            var handlerList = handlers == null ? new List<ICustomControlHandler>() : handlers.ToList();
            var found = new List<Found>();
            var texts = new List<Found>();
            Walk(root, null, handlerList, found, texts);

            var counts = new Dictionary<string, int>();
            var result = new List<ControlSnapshot>();
            foreach (var f in found)
            {
                int count;
                counts.TryGetValue(f.Id, out count);
                counts[f.Id] = ++count;

                var type = f.Element.ControlType.ToString();
                result.Add(new ControlSnapshot
                {
                    Id = count == 1 ? f.Id : f.Id + "#" + count,
                    Type = FindHandler(f.Element, handlerList) != null ? "Custom" : type,
                    Label = GuessLabel(f, texts),
                    Value = Read(f.Element, handlerList),
                    Enabled = SafeIsEnabled(f.Element),
                    X = f.Bounds.X - origin.X,
                    Y = f.Bounds.Y - origin.Y,
                    Width = f.Bounds.Width,
                    Height = f.Bounds.Height,
                });
            }
            return result;
        }

        private class Found
        {
            public AutomationElement Element;
            public string Id;
            public string Group;
            public Rectangle Bounds;
        }

        private static void Walk(AutomationElement parent, string group, List<ICustomControlHandler> handlers, List<Found> found, List<Found> texts)
        {
            AutomationElement[] children;
            try { children = parent.FindAllChildren(); }
            catch (Exception) { return; }

            foreach (var child in children)
            {
                ControlType type;
                string id;
                try
                {
                    type = child.ControlType;
                    id = child.Properties.AutomationId.ValueOrDefault;
                }
                catch (Exception)
                {
                    continue;
                }

                // 子画面(ダイアログ)は別画面として扱う
                if (type == ControlType.Window) continue;

                var isHandled = handlers.Any(h => h.CanHandle(child));
                var hasId = !string.IsNullOrEmpty(id);
                var item = new Found { Element = child, Id = id, Group = group, Bounds = SafeBounds(child) };

                if (type == ControlType.Text) texts.Add(item);

                if (isHandled || LeafTypes.Contains(type))
                {
                    if (hasId || isHandled) found.Add(item);
                    continue;
                }

                // GroupBox 等の入れ物：中の部品の項目名に枠の名前を付けるため、名前を引き継ぐ
                var childGroup = type == ControlType.Group && !string.IsNullOrEmpty(child.Name) ? child.Name : group;
                Walk(child, childGroup, handlers, found, texts);
            }
        }

        /// <summary>項目名の推定：チェックボックス等は自分の表示名、入力欄は左隣のラベル。</summary>
        private static string GuessLabel(Found target, List<Found> texts)
        {
            var type = target.Element.ControlType;
            string label = null;
            if (type == ControlType.CheckBox || type == ControlType.RadioButton || type == ControlType.Button)
            {
                label = target.Element.Name;
            }
            else if (type != ControlType.Text)
            {
                var b = target.Bounds;
                var centerY = b.Top + b.Height / 2;
                var nearest = texts
                    .Where(t => t.Bounds.Right <= b.Left + 4 && b.Left - t.Bounds.Right < 300
                             && t.Bounds.Top - 4 <= centerY && centerY <= t.Bounds.Bottom + 4)
                    .OrderBy(t => b.Left - t.Bounds.Right)
                    .FirstOrDefault();
                if (nearest != null) label = nearest.Element.Name;
            }

            if (string.IsNullOrEmpty(label)) return null;
            return target.Group == null ? label : target.Group + "/" + label;
        }

        private static Rectangle SafeBounds(AutomationElement element)
        {
            try { return element.BoundingRectangle; }
            catch (Exception) { return Rectangle.Empty; }
        }

        private static bool SafeIsEnabled(AutomationElement element)
        {
            try { return element.IsEnabled; }
            catch (Exception) { return false; }
        }
    }

    /// <summary>テキストボックスへの入力方法。</summary>
    public enum InputMode
    {
        /// <summary>値を直接設定する（速い。IMEやキー入力イベントに左右されない）。</summary>
        SetValue,

        /// <summary>実際にキーを打鍵する（KeyPress・Validating 等のイベントで動く画面用）。</summary>
        Keyboard,
    }
}
`````

### 16. `tests/GuiTestKit/Automation/ICustomControlHandler.cs`

`````csharp
using System.Collections.Generic;
using FlaUI.Core.AutomationElements;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 標準の部品として扱えないもの（Spread などの市販部品）の値の読み書きを差し替える口。
    /// CustomControls.Handlers に登録すると、Set / Get / 全項目の吸い出し がこちらを使う。
    /// </summary>
    public interface ICustomControlHandler
    {
        /// <summary>この部品を担当するかどうか（AutomationId の接頭辞などで判定する）。</summary>
        bool CanHandle(AutomationElement element);

        /// <summary>比較・レポート用に値を文字列で返す（表なら TSV など）。</summary>
        string Read(AutomationElement element);

        void Write(AutomationElement element, string value);
    }

    /// <summary>全画面共通で使う差し替え部品の登録先。製品のテスト基底クラスの静的コンストラクタ等で登録する。</summary>
    public static class CustomControls
    {
        public static readonly List<ICustomControlHandler> Handlers = new List<ICustomControlHandler>();
    }
}
`````

### 17. `tests/GuiTestKit/Automation/MessageBoxPage.cs`

`````csharp
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// Windows 標準のメッセージボックス（MessageBox.Show で出るダイアログ）。
    /// 全画面で共通なので、画面ごとに作る必要はない。
    /// </summary>
    public class MessageBoxPage : ScreenPageBase
    {
        /// <summary>標準ダイアログのメッセージ文の固定ID。</summary>
        private const string MessageTextId = "65535";

        public MessageBoxPage(Window window) : base(window)
        {
        }

        public string Title { get { return Window.Title; } }

        public string Message
        {
            get
            {
                var text = Window.FindFirstDescendant(cf => cf.ByAutomationId(MessageTextId));
                return text == null ? string.Empty : text.Name;
            }
        }

        /// <summary>OK ボタンを押して閉じる。</summary>
        public void ClickOk()
        {
            ClickButton("OK");
        }

        /// <summary>
        /// 表示名でボタンを押す。「はい(Y)」のようなアクセスキー付きも「はい」で指定できる。
        /// </summary>
        public void ClickButton(string caption)
        {
            var button = Window.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
                .FirstOrDefault(b => b.Name == caption || b.Name.StartsWith(caption + "("));
            if (button == null) throw new AssertFailedException("メッセージボックスにボタン「" + caption + "」がありません。");

            Log("メッセージボックス「" + Title + "」の [" + button.Name + "] を押す");
            button.AsButton().Invoke();
        }
    }
}
`````

### 18. `tests/GuiTestKit/Automation/NativeMethods.cs`

`````csharp
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GuiTestKit.Automation
{
    /// <summary>キャプチャ範囲の計算などで使う Win32 API。</summary>
    internal static class NativeMethods
    {
        private const uint GW_OWNER = 4;
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
            public Rectangle ToRectangle() { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X, Y;
        }

        [DllImport("user32.dll")] private static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr hWnd, uint cmd);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
        [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hWnd, out RECT rect);
        [DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
        [DllImport("user32.dll")] private static extern uint GetCaretBlinkTime();
        [DllImport("user32.dll")] private static extern bool SetCaretBlinkTime(uint milliseconds);
        [DllImport("user32.dll")] private static extern int GetSystemMetrics(int index);
        [DllImport("dwmapi.dll")] private static extern int DwmGetWindowAttribute(IntPtr hWnd, int attribute, out RECT value, int size);

        /// <summary>
        /// 高DPI(125%等)の環境でも、座標とキャプチャのピクセルがずれないようにする。
        /// テストプロセスの最初に1回呼ぶ。
        /// </summary>
        public static void EnableDpiAwareness()
        {
            try { SetProcessDPIAware(); } catch (EntryPointNotFoundException) { }
        }

        /// <summary>前面(アクティブ)ウィンドウが指定プロセスのものなら、そのハンドルを返す。</summary>
        public static IntPtr GetForegroundWindowOf(int processId)
        {
            var hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero) return IntPtr.Zero;
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);
            return pid == processId ? hWnd : IntPtr.Zero;
        }

        public static IntPtr GetOwner(IntPtr hWnd)
        {
            return GetWindow(hWnd, GW_OWNER);
        }

        /// <summary>ウィンドウの見た目どおりの枠（Windows 10 以降の透明な枠を含まない）。</summary>
        public static Rectangle GetVisibleWindowBounds(IntPtr hWnd)
        {
            RECT rect;
            if (DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RECT))) == 0)
            {
                return rect.ToRectangle();
            }
            GetWindowRect(hWnd, out rect);
            return rect.ToRectangle();
        }

        /// <summary>タイトルバーと枠を除いた中身部分の、画面上の位置とサイズ。</summary>
        public static Rectangle GetClientBoundsOnScreen(IntPtr hWnd)
        {
            RECT client;
            GetClientRect(hWnd, out client);
            var origin = new POINT();
            ClientToScreen(hWnd, ref origin);
            return new Rectangle(origin.X, origin.Y, client.Right - client.Left, client.Bottom - client.Top);
        }

        /// <summary>全モニタを合わせた範囲。</summary>
        public static Rectangle GetVirtualScreen()
        {
            return new Rectangle(
                GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN),
                GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN));
        }

        public static uint GetCaretBlink() { return GetCaretBlinkTime(); }

        public static void SetCaretBlink(uint milliseconds) { SetCaretBlinkTime(milliseconds); }
    }
}
`````

### 19. `tests/GuiTestKit/Automation/ScreenPageBase.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using GuiTestKit.Evidence;
using GuiTestKit.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 全画面共通の Page 基底クラス。
    /// 部品は変数として定義せず、項目ID（AutomationId = WinForms の Name）の文字列で指定する。
    /// 画面ごとのクラスには「基本値」「比較対象外の項目」「その画面特有の操作」だけを書く。
    /// </summary>
    public abstract class ScreenPageBase
    {
        /// <summary>部品や画面が出てくるまで待つ最大時間。</summary>
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        private Dictionary<string, AutomationElement> _index;

        protected ScreenPageBase(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");
            Window = window;
            var session = TestSession.Current;
            if (session != null) session.RegisterPage(this);
        }

        public Window Window { get; private set; }

        /// <summary>この画面の全項目の標準的な入力値。GuiTestTool dump で画面から吸い出して作る。</summary>
        public virtual ScreenValues BaseValues { get { return new ScreenValues(); } }

        /// <summary>実行するたびに変わる項目（日時・採番など）。VB/C# の比較で値も画像も比較しない。</summary>
        public virtual IEnumerable<string> CompareExcludedIds { get { return Enumerable.Empty<string>(); } }

        /// <summary>テキストボックスへの入力方法。キー入力イベントで動く画面は Keyboard にする。</summary>
        protected virtual InputMode TextInputMode { get { return InputMode.SetValue; } }

        /// <summary>この画面で使う、市販部品などの読み書きの差し替え。既定は全画面共通の登録内容。</summary>
        public virtual IEnumerable<ICustomControlHandler> CustomHandlers { get { return CustomControls.Handlers; } }

        // ------------------------------------------------------------------
        // 値の入力・取得
        // ------------------------------------------------------------------

        /// <summary>
        /// 基本値を全部入力する。changes を渡すと、その項目だけ基本値から変えて入力する。
        /// レポートには「基本値を入力」と「基本値から変えた項目」だけが出る。
        /// </summary>
        public void EnterBaseValues(ScreenValues changes = null)
        {
            var values = BaseValues.With(changes);
            Log("基本値を入力（" + values.Count + "項目）");
            if (changes != null)
            {
                foreach (var c in changes)
                {
                    var baseValue = BaseValues[c.Id];
                    Log("　変更: " + Describe(c.Id) + " = " + Show(c.Value) + (BaseValues.Contains(c.Id) ? "（基本値: " + Show(baseValue) + "）" : ""));
                }
            }
            foreach (var v in values) SetCore(v.Id, v.Value);
        }

        /// <summary>複数の項目を、書いた順に入力する（基本値を使わない場合）。</summary>
        public void SetValues(ScreenValues values)
        {
            foreach (var v in values)
            {
                Log("入力: " + Describe(v.Id) + " = " + Show(v.Value));
                SetCore(v.Id, v.Value);
            }
        }

        /// <summary>1項目を入力する。テキスト・コンボは表示文字列、チェック・ラジオは ON/OFF。</summary>
        public void Set(string id, string value)
        {
            Log("入力: " + Describe(id) + " = " + Show(value));
            SetCore(id, value);
        }

        /// <summary>1項目を実際にキーを打鍵して入力する。</summary>
        public void Type(string id, string value)
        {
            Log("打鍵入力: " + Describe(id) + " = " + Show(value));
            ControlAccess.Write(Find(id), value, InputMode.Keyboard, CustomHandlers);
        }

        public string Get(string id)
        {
            return ControlAccess.Read(Find(id), CustomHandlers);
        }

        public bool IsEnabled(string id)
        {
            return Find(id).IsEnabled;
        }

        /// <summary>画面の全項目の状態（値・有効/無効）を読み取る。</summary>
        public List<ControlSnapshot> ReadAll()
        {
            return ControlAccess.ReadAll(Window, CustomHandlers, Window.BoundingRectangle.Location);
        }

        private void SetCore(string id, string value)
        {
            if (value == null) return;   // null は「触らない」

            var element = Find(id);
            if (!element.IsEnabled)
            {
                // 無効な項目：すでにその値なら何もしない（区分によって無効になる項目など）
                var current = ControlAccess.Read(element, CustomHandlers);
                if (current == value || (value.Length == 0 && string.IsNullOrEmpty(current))) return;
                throw new AssertFailedException(Describe(id) + " は無効のため「" + value + "」を入力できません（現在値: " + current + "）。入力順か基本値を見直してください。");
            }

            try
            {
                ControlAccess.Write(element, value, TextInputMode, CustomHandlers);
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(Describe(id) + " に「" + value + "」を入力できませんでした: " + ex.Message, ex);
            }
        }

        // ------------------------------------------------------------------
        // ボタン操作・画面遷移
        // ------------------------------------------------------------------

        /// <summary>
        /// ボタンを押す（UI Automation の Invoke。マウスは動かない）。
        /// 押すとダイアログや別画面が開くボタンには Click を使うこと（Invoke だと戻ってこない場合がある）。
        /// </summary>
        public void Press(string id)
        {
            Log("押下: " + Describe(id));
            Find(id).AsButton().Invoke();
            Wait.UntilInputIsProcessed();
        }

        /// <summary>マウスでクリックする。</summary>
        public void Click(string id)
        {
            Log("クリック: " + Describe(id));
            ClickCore(id);
        }

        /// <summary>ボタンをクリックし、表示されたメッセージボックスを返す。</summary>
        public MessageBoxPage ClickAndWaitMessageBox(string id)
        {
            Click(id);
            return WaitForMessageBox();
        }

        /// <summary>ボタンをクリックして別画面を開き、その画面の Page を返す。</summary>
        protected T OpenScreen<T>(string buttonId, string windowId, Func<Window, T> createPage) where T : ScreenPageBase
        {
            Click(buttonId);
            return createPage(WaitForWindow(windowId));
        }

        /// <summary>この画面から開いたメッセージボックス（標準ダイアログ）を待って返す。</summary>
        public MessageBoxPage WaitForMessageBox(TimeSpan? timeout = null)
        {
            var dialog = Retry.WhileNull(() => FindMessageBox(), timeout ?? DefaultTimeout).Result;
            if (dialog == null) throw new AssertFailedException("メッセージボックスが表示されませんでした。");
            return new MessageBoxPage(dialog);
        }

        /// <summary>画面（フォーム）を AutomationId で探して待つ。WinForms ではフォームの Name。</summary>
        protected Window WaitForWindow(string windowId, TimeSpan? timeout = null)
        {
            var window = Retry.WhileNull(() => FindWindow(windowId), timeout ?? DefaultTimeout).Result;
            if (window == null) throw new AssertFailedException("画面が表示されませんでした: " + windowId);
            window.SetForeground();
            return window;
        }

        /// <summary>画面を閉じる（タイトルバーの×と同じ）。</summary>
        public void CloseWindow()
        {
            Log("画面を閉じる: " + Window.Title);
            Window.Close();
        }

        /// <summary>この画面が閉じるまで待つ。</summary>
        public bool WaitUntilClosed(TimeSpan? timeout = null)
        {
            return Retry.WhileTrue(() => IsOpen(), timeout ?? DefaultTimeout).Result;
        }

        /// <summary>条件が true になるまで待つ（固定の Sleep を使わないため）。</summary>
        public bool WaitUntil(Func<bool> condition, TimeSpan? timeout = null)
        {
            return Retry.WhileFalse(condition, timeout ?? DefaultTimeout).Result;
        }

        // ------------------------------------------------------------------
        // 部品の検索
        // ------------------------------------------------------------------

        /// <summary>
        /// 項目IDで部品を探す（出てくるまで待つ）。
        /// "親ID/子ID" で入れ物の中を指定、"ID#2" で同じIDの2つ目を指定できる。
        /// </summary>
        public AutomationElement Find(string id)
        {
            var element = Retry.WhileNull(() => FindOrNull(id), DefaultTimeout).Result;
            if (element == null) throw new AssertFailedException("部品が見つかりません: " + id + "（画面: " + Window.Title + "）");
            return element;
        }

        /// <summary>項目IDで部品を探す（待たない。無ければ null）。</summary>
        public AutomationElement FindOrNull(string id)
        {
            AutomationElement cached;
            if (_index != null && _index.TryGetValue(id, out cached) && IsAlive(cached)) return cached;

            // 画面全体を1回だけ走査して索引を作る（項目が数百ある画面でも速く探せるように）
            _index = BuildIndex();
            if (_index.TryGetValue(id, out cached)) return cached;

            return id.Contains("/") ? FindByPath(id) : null;
        }

        private Dictionary<string, AutomationElement> BuildIndex()
        {
            var index = new Dictionary<string, AutomationElement>();
            var counts = new Dictionary<string, int>();
            foreach (var e in Window.FindAllDescendants())
            {
                string id;
                try { id = e.Properties.AutomationId.ValueOrDefault; }
                catch (ElementNotAvailableException) { continue; }
                if (string.IsNullOrEmpty(id)) continue;

                int count;
                counts.TryGetValue(id, out count);
                counts[id] = ++count;
                index[count == 1 ? id : id + "#" + count] = e;
            }
            return index;
        }

        private AutomationElement FindByPath(string path)
        {
            AutomationElement scope = Window;
            foreach (var part in path.Split('/'))
            {
                var id = part;
                var nth = 1;
                var hash = part.LastIndexOf('#');
                if (hash > 0 && int.TryParse(part.Substring(hash + 1), out nth)) id = part.Substring(0, hash);
                else nth = 1;

                var matches = scope.FindAllDescendants(cf => cf.ByAutomationId(id));
                if (matches.Length < nth) return null;
                scope = matches[nth - 1];
            }
            return scope;
        }

        private static bool IsAlive(AutomationElement element)
        {
            try
            {
                // 閉じた画面の部品は、プロパティを読むと例外になる
                var unused = element.BoundingRectangle;
                return true;
            }
            catch (ElementNotAvailableException) { return false; }
            catch (System.Runtime.InteropServices.COMException) { return false; }
        }

        private bool IsOpen()
        {
            try { return Window.IsAvailable && !Window.IsOffscreen; }
            catch (Exception) { return false; }
        }

        private void ClickCore(string id)
        {
            var element = Find(id);
            Window.SetForeground();
            element.Click();
            Wait.UntilInputIsProcessed();
        }

        private Window FindMessageBox()
        {
            var modal = Window.ModalWindows.FirstOrDefault();
            if (modal != null) return modal;

            // 所有関係が取れない場合：同じプロセスの標準ダイアログ(#32770)を探す
            var session = TestSession.Current;
            if (session == null) return null;
            return session.App.GetAllTopLevelWindows(session.Automation)
                .FirstOrDefault(w => w.ClassName == "#32770");
        }

        private Window FindWindow(string windowId)
        {
            var cf = Window.ConditionFactory;
            var condition = cf.ByControlType(ControlType.Window).And(cf.ByAutomationId(windowId));

            var child = Window.FindFirstChild(condition);
            if (child != null) return child.AsWindow();

            var session = TestSession.Current;
            if (session == null) return null;
            foreach (var top in session.App.GetAllTopLevelWindows(session.Automation))
            {
                if (top.AutomationId == windowId) return top;
                var nested = top.FindFirstChild(condition);
                if (nested != null) return nested.AsWindow();
            }
            return null;
        }

        // ------------------------------------------------------------------
        // レポート用の記録
        // ------------------------------------------------------------------

        /// <summary>操作をエビデンス（レポート）に記録する。</summary>
        protected void Log(string text)
        {
            var session = TestSession.Current;
            if (session != null) session.LogOperation(text);
        }

        /// <summary>レポート用の項目表示：「顧客コード(txtCode)」。項目名は基本値の定義から取る。</summary>
        protected string Describe(string id)
        {
            var label = BaseValues.LabelOf(id);
            return label == null ? id : label + "(" + id + ")";
        }

        private static string Show(string value)
        {
            if (value == null) return "（変更しない）";
            return value.Length == 0 ? "（空）" : "「" + value + "」";
        }
    }
}
`````

### 20. `tests/GuiTestKit/Automation/ScreenValues.cs`

`````csharp
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 画面の項目ID → 値 の一覧。入力する順番を保つ（区分を変えると他の項目が無効になる、等の画面があるため）。
    /// コレクション初期化子で書ける:
    /// <code>
    /// new ScreenValues
    /// {
    ///     { "txtCode", "000001", "顧客コード" },   // 項目ID, 値, 項目名（項目名は省略可。レポートの表示に使う）
    ///     { "chkActive", ScreenValues.ON },
    /// }
    /// </code>
    /// 値の書き方: テキスト・コンボボックスは表示文字列、チェックボックス・ラジオボタンは "ON"/"OFF"、
    /// 空文字 "" は「空にする」、null は「触らない」。
    /// </summary>
    public class ScreenValues : IEnumerable<ScreenValue>
    {
        public const string ON = "ON";
        public const string OFF = "OFF";

        private readonly List<ScreenValue> _items = new List<ScreenValue>();

        public void Add(string id, string value)
        {
            Add(id, value, null);
        }

        public void Add(string id, string value, string label)
        {
            var index = _items.FindIndex(x => x.Id == id);
            var item = new ScreenValue(id, value, label);
            if (index >= 0) _items[index] = item; else _items.Add(item);
        }

        public int Count { get { return _items.Count; } }

        public bool Contains(string id)
        {
            return _items.Any(x => x.Id == id);
        }

        public string this[string id]
        {
            get
            {
                var item = _items.FirstOrDefault(x => x.Id == id);
                return item == null ? null : item.Value;
            }
        }

        public string LabelOf(string id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            return item == null ? null : item.Label;
        }

        /// <summary>
        /// この一覧（基本値）に changes を上書きした新しい一覧を返す。
        /// 並び順は基本値の順。基本値にない項目は末尾に追加する。
        /// </summary>
        public ScreenValues With(ScreenValues changes)
        {
            var result = new ScreenValues();
            foreach (var item in _items)
            {
                var changed = changes != null && changes.Contains(item.Id);
                result.Add(item.Id, changed ? changes[item.Id] : item.Value, item.Label ?? (changed ? changes.LabelOf(item.Id) : null));
            }
            if (changes != null)
            {
                foreach (var item in changes.Where(x => !Contains(x.Id)))
                {
                    result.Add(item.Id, item.Value, item.Label);
                }
            }
            return result;
        }

        public IEnumerator<ScreenValue> GetEnumerator() { return _items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    public class ScreenValue
    {
        public ScreenValue(string id, string value, string label)
        {
            Id = id;
            Value = value;
            Label = label;
        }

        public string Id { get; private set; }
        public string Value { get; private set; }
        public string Label { get; private set; }
    }
}
`````

### 21. `tests/GuiTestKit/Automation/SpreadGrid.cs`

`````csharp
using System;
using System.Threading;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

namespace GuiTestKit.Automation
{
    // ------------------------------------------------------------------
    // 【要検証】SPREAD for Windows Forms 用のひな形。
    // Spread は UI Automation でセルを読み書きできるかがバージョンによって異なるため、
    // UIA に頼らず「キー操作でセル移動・入力」「クリップボードで読み取り」で実装している。
    // 実機の Spread で動作を確認し、必要に応じて調整すること。
    // ------------------------------------------------------------------

    /// <summary>
    /// Spread を1項目として扱うハンドラ。全項目の吸い出し・VB/C#比較では、シート全体を TSV（タブ区切り）で記録する。
    /// 登録例: CustomControls.Handlers.Add(new SpreadHandler(id => id.StartsWith("spd")));
    /// </summary>
    public class SpreadHandler : ICustomControlHandler
    {
        private readonly Func<string, bool> _isSpreadId;

        /// <param name="isSpreadId">AutomationId で Spread かどうかを判定する（命名規約 "spd" 始まり等）。</param>
        public SpreadHandler(Func<string, bool> isSpreadId)
        {
            _isSpreadId = isSpreadId;
        }

        public bool CanHandle(AutomationElement element)
        {
            var id = element.Properties.AutomationId.ValueOrDefault;
            return !string.IsNullOrEmpty(id) && _isSpreadId(id);
        }

        public string Read(AutomationElement element)
        {
            return new SpreadGrid(element).ReadAllAsTsv();
        }

        public void Write(AutomationElement element, string value)
        {
            throw new NotSupportedException("Spread はセル単位で入力してください（SpreadGrid.SetCell）。");
        }
    }

    /// <summary>Spread のセル操作。Page クラスから new SpreadGrid(Find("spdMeisai")) のように使う。</summary>
    public class SpreadGrid
    {
        private readonly AutomationElement _element;

        public SpreadGrid(AutomationElement element)
        {
            _element = element;
        }

        /// <summary>セルに値を入力する（行・列は 0 始まり）。</summary>
        public void SetCell(int row, int column, string value)
        {
            MoveTo(row, column);
            Keyboard.Type(VirtualKeyShort.F2);          // 編集開始（設定によっては不要）
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            Keyboard.Type(VirtualKeyShort.DELETE);
            if (value.Length > 0) Keyboard.Type(value);
            Keyboard.Type(VirtualKeyShort.RETURN);      // 確定
            Wait.UntilInputIsProcessed();
        }

        /// <summary>セルの値を読む（クリップボード経由）。</summary>
        public string GetCell(int row, int column)
        {
            MoveTo(row, column);
            return CopyToText();
        }

        /// <summary>シート全体を TSV で読む（Ctrl+A → Ctrl+C）。</summary>
        public string ReadAllAsTsv()
        {
            _element.Focus();
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            return CopyToText();
        }

        private void MoveTo(int row, int column)
        {
            _element.Click();
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.HOME);
            for (var r = 0; r < row; r++) Keyboard.Type(VirtualKeyShort.DOWN);
            for (var c = 0; c < column; c++) Keyboard.Type(VirtualKeyShort.RIGHT);
            Wait.UntilInputIsProcessed();
        }

        private static string CopyToText()
        {
            RunSta(Clipboard.Clear);
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_C);
            Wait.UntilInputIsProcessed();

            string text = null;
            for (var i = 0; i < 20 && string.IsNullOrEmpty(text); i++)
            {
                Thread.Sleep(50);
                RunSta(() => text = Clipboard.GetText());
            }
            return (text ?? string.Empty).TrimEnd('\r', '\n');
        }

        /// <summary>クリップボードは STA スレッドからしか使えないため、専用スレッドで実行する。</summary>
        private static void RunSta(Action action)
        {
            Exception error = null;
            var thread = new Thread(() =>
            {
                try { action(); }
                catch (Exception ex) { error = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null) throw error;
        }
    }
}
`````

### 22. `tests/GuiTestKit/Comparison/CompareReportWriter.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuiTestKit.Evidence;
using GuiTestKit.Reporting;

namespace GuiTestKit.Comparison
{
    /// <summary>
    /// VB断面・C#断面の比較結果を compare.html に出す。
    /// 既定では差異のあるものだけ表示し、キャプチャを「VB｜C#｜差分画像」の順で横に並べる。
    /// </summary>
    public static class CompareReportWriter
    {
        public static string Write(string outDir, string dirA, string dirB, CompareOptions options, List<TestComparison> results)
        {
            var nameA = SideName(dirA, results.Select(r => r.A));
            var nameB = SideName(dirB, results.Select(r => r.B));

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html lang=\"ja\"><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>キャプチャ比較 " + Html.E(nameA) + " / " + Html.E(nameB) + "</title>");
            sb.AppendLine("<style>" + Html.Style + "</style></head><body>");
            sb.AppendLine("<h1>キャプチャ・項目値の比較（" + Html.E(nameA) + " / " + Html.E(nameB) + "）</h1>");
            sb.AppendLine("<div class=\"meta\">");
            sb.AppendLine(Html.E(nameA) + ": " + Html.E(Path.GetFullPath(dirA)) + "<br>");
            sb.AppendLine(Html.E(nameB) + ": " + Html.E(Path.GetFullPath(dirB)) + "<br>");
            sb.AppendLine("色の許容差: " + options.Tolerance + "　／　作成: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.AppendLine("</div>");

            // ---- 集計 ----
            sb.AppendLine("<table><tr><th>テスト数</th><th>一致</th><th>差異あり</th><th>片方のみ</th><th>キャプチャ数</th><th>差異のあるキャプチャ</th></tr>");
            sb.AppendFormat("<tr><td class=\"num\">{0}</td><td class=\"num\">{1}</td><td class=\"num\">{2}</td><td class=\"num\">{3}</td><td class=\"num\">{4}</td><td class=\"num\">{5}</td></tr></table>\n",
                results.Count,
                results.Count(r => r.Status == CompareStatus.Same),
                results.Count(r => r.Status == CompareStatus.Different),
                results.Count(r => r.Status == CompareStatus.OnlyOneSide),
                results.Sum(r => r.Steps.Count),
                results.Sum(r => r.Steps.Count(s => s.Status != CompareStatus.Same)));

            sb.AppendLine("<div class=\"meta\">差分画像の色: <span style=\"color:#cf222e\">■ 赤＝差異</span>　<span style=\"color:#f90\">■ オレンジ＝サイズ違いではみ出した部分</span>　<span style=\"color:#9db4d0\">■ 青い斜線＝比較対象外</span>　薄いグレー＝一致</div>");
            sb.AppendLine("<div class=\"toolbar\"><label><input type=\"checkbox\" id=\"filter\" checked onchange=\"applyFilter(this)\"> 差異のあるものだけ表示</label></div>");

            // ---- 一覧 ----
            sb.AppendLine("<h2>テスト一覧</h2>");
            sb.AppendFormat("<table><tr><th>No</th><th>テスト</th><th>判定</th><th>{0} 結果</th><th>{1} 結果</th><th>差異のあるキャプチャ</th></tr>\n", Html.E(nameA), Html.E(nameB));
            for (var i = 0; i < results.Count; i++)
            {
                var r = results[i];
                sb.AppendFormat("<tr data-status=\"{0}\"><td class=\"num\">{1}</td><td><a href=\"#c{1}\">{2}</a></td><td>{3}</td><td>{4}</td><td>{5}</td><td class=\"num\">{6} / {7}</td></tr>\n",
                    FilterStatus(r.Status), i + 1, Html.E(r.Key), Badge(r.Status),
                    Html.E(r.A == null ? "（なし）" : r.A.Outcome), Html.E(r.B == null ? "（なし）" : r.B.Outcome),
                    r.Steps.Count(s => s.Status != CompareStatus.Same), r.Steps.Count);
            }
            sb.AppendLine("</table>");

            // ---- 詳細 ----
            sb.AppendLine("<h2>詳細</h2>");
            for (var i = 0; i < results.Count; i++)
            {
                AppendTest(sb, outDir, results[i], i + 1, nameA, nameB);
            }

            sb.AppendLine(Html.FilterScript);
            sb.AppendLine("</body></html>");

            var path = Path.Combine(outDir, "compare.html");
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
            return path;
        }

        private static void AppendTest(StringBuilder sb, string outDir, TestComparison r, int no, string nameA, string nameB)
        {
            sb.AppendFormat("<div class=\"test\" id=\"c{0}\" data-status=\"{1}\">\n", no, FilterStatus(r.Status));
            sb.AppendFormat("<h3>{0}. {1}　{2}</h3>\n", no, Html.E(r.Key), Badge(r.Status));
            if (r.A == null || r.B == null)
            {
                sb.AppendFormat("<div>{0} にしかありません。</div></div>\n", Html.E(r.A == null ? nameB : nameA));
                return;
            }
            foreach (var note in r.Notes) sb.AppendLine("<div class=\"badge warn\">" + Html.E(note) + "</div>");

            foreach (var s in r.Steps)
            {
                sb.AppendFormat("<div data-status=\"{0}\">\n", FilterStatus(s.Status));
                var label = s.A != null ? s.A.Text : s.B.Text;
                sb.AppendFormat("<div class=\"caption\" style=\"margin-top:16px\">{0:00}. {1}　{2}</div>\n", s.No, Html.E(label), Badge(s.Status));

                if (s.A == null || s.B == null)
                {
                    sb.AppendFormat("<div>このキャプチャは {0} にしかありません。</div></div>\n", Html.E(s.A == null ? nameB : nameA));
                    continue;
                }
                foreach (var note in s.Notes) sb.AppendLine("<div class=\"badge warn\">" + Html.E(note) + "</div>");

                var img = s.Image;
                var sizeText = img.SizeMatches
                    ? string.Format("{0}×{1}", img.SizeA.Width, img.SizeA.Height)
                    : string.Format("<span class=\"badge ng\">サイズ違い {0}×{1} / {2}×{3}</span>", img.SizeA.Width, img.SizeA.Height, img.SizeB.Width, img.SizeB.Height);
                sb.AppendFormat("<div class=\"meta\">画像: {0}　差異ピクセル: {1:N0}（{2:P3}）</div>\n", sizeText, img.DiffPixels, img.DiffRatio);

                sb.AppendLine("<div class=\"pair\">");
                AppendImage(sb, outDir, nameA, r.A, s.A);
                AppendImage(sb, outDir, nameB, r.B, s.B);
                var diffUrl = Html.RelativeUrl(outDir, s.DiffImage);
                sb.AppendFormat("<div><div class=\"meta\">差分</div><a href=\"{0}\" target=\"_blank\"><img class=\"shot\" src=\"{0}\" loading=\"lazy\"></a></div>\n", diffUrl);
                sb.AppendLine("</div>");

                if (s.Values.Count > 0)
                {
                    sb.AppendFormat("<table><tr><th>項目ID</th><th>項目名</th><th>差異</th><th>{0}</th><th>{1}</th></tr>\n", Html.E(nameA), Html.E(nameB));
                    foreach (var v in s.Values)
                    {
                        sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td class=\"diffcell\">{3}</td><td class=\"diffcell\">{4}</td></tr>\n",
                            Html.E(v.Id), Html.E(v.Label), Html.E(v.What), Html.E(v.A), Html.E(v.B));
                    }
                    sb.AppendLine("</table>");
                }
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
        }

        private static void AppendImage(StringBuilder sb, string outDir, string name, TestRecord record, StepRecord step)
        {
            var compare = Html.RelativeUrl(outDir, Path.Combine(record.Folder, step.CompareImage));
            var evidence = Html.RelativeUrl(outDir, Path.Combine(record.Folder, step.EvidenceImage));
            sb.AppendFormat("<div><div class=\"meta\">{0}（<a href=\"{1}\" target=\"_blank\">タイトルバー込み</a>）</div><a href=\"{2}\" target=\"_blank\"><img class=\"shot\" src=\"{2}\" loading=\"lazy\"></a></div>\n",
                Html.E(name), evidence, compare);
        }

        /// <summary>表示名: 記録の Target（VB / CS）。取れなければフォルダ名。</summary>
        private static string SideName(string dir, IEnumerable<TestRecord> records)
        {
            var target = records.Where(r => r != null).Select(r => r.Target).FirstOrDefault();
            return target ?? Path.GetFileName(Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar));
        }

        private static string FilterStatus(string status)
        {
            return status == CompareStatus.Same ? "ok" : "ng";
        }

        private static string Badge(string status)
        {
            var css = status == CompareStatus.Same ? "ok" : status == CompareStatus.Different ? "ng" : "warn";
            return "<span class=\"badge " + css + "\">" + Html.E(status) + "</span>";
        }
    }
}
`````

### 23. `tests/GuiTestKit/Comparison/EvidenceComparer.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GuiTestKit.Evidence;
using GuiTestKit.Reporting;

namespace GuiTestKit.Comparison
{
    /// <summary>比較の判定。</summary>
    public static class CompareStatus
    {
        public const string Same = "一致";
        public const string Different = "差異あり";
        public const string OnlyOneSide = "片方のみ";
    }

    public class CompareOptions
    {
        /// <summary>RGB各色の差がこの値以下なら同じとみなす（0 = 完全一致）。</summary>
        public int Tolerance { get; set; }
    }

    /// <summary>テスト1件の比較結果。</summary>
    public class TestComparison
    {
        /// <summary>シナリオ\テスト名[_ケース番号]（VB・C#でテストを対応付けるキー）。</summary>
        public string Key { get; set; }
        public TestRecord A { get; set; }
        public TestRecord B { get; set; }
        public List<StepComparison> Steps { get; set; }
        public List<string> Notes { get; set; }

        public string Status
        {
            get
            {
                if (A == null || B == null) return CompareStatus.OnlyOneSide;
                if (Notes.Count > 0 || Steps.Any(s => s.Status != CompareStatus.Same)) return CompareStatus.Different;
                return CompareStatus.Same;
            }
        }

        public TestComparison()
        {
            Steps = new List<StepComparison>();
            Notes = new List<string>();
        }
    }

    /// <summary>キャプチャ1枚分の比較結果。</summary>
    public class StepComparison
    {
        public int No { get; set; }
        public StepRecord A { get; set; }
        public StepRecord B { get; set; }
        public ImageDiffResult Image { get; set; }
        public string DiffImage { get; set; }
        public List<ValueDifference> Values { get; set; }
        public List<string> Notes { get; set; }

        public string Status
        {
            get
            {
                if (A == null || B == null) return CompareStatus.OnlyOneSide;
                var imageDiff = Image != null && (Image.DiffPixels > 0 || !Image.SizeMatches);
                return imageDiff || Values.Count > 0 || Notes.Count > 0 ? CompareStatus.Different : CompareStatus.Same;
            }
        }

        public StepComparison()
        {
            Values = new List<ValueDifference>();
            Notes = new List<string>();
        }
    }

    /// <summary>項目値の差異。</summary>
    public class ValueDifference
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string What { get; set; }
        public string A { get; set; }
        public string B { get; set; }
    }

    /// <summary>
    /// 2つのエビデンスフォルダ（例: VB断面の実行結果 と C#断面の実行結果）を突き合わせる。
    /// テストは「シナリオ\テスト名」、キャプチャは撮った順番で対応付ける。
    /// </summary>
    public static class EvidenceComparer
    {
        /// <param name="dirA">比較元（例: Evidence\20260926_100000\VB）</param>
        /// <param name="dirB">比較先（例: Evidence\20260926_110000\CS）</param>
        /// <param name="outDir">差分画像と compare.html の出力先</param>
        public static List<TestComparison> Compare(string dirA, string dirB, string outDir, CompareOptions options)
        {
            var recordsA = RunReportWriter.LoadRecords(dirA).ToDictionary(r => KeyOf(dirA, r));
            var recordsB = RunReportWriter.LoadRecords(dirB).ToDictionary(r => KeyOf(dirB, r));
            var diffDir = Path.Combine(outDir, "diff");
            Directory.CreateDirectory(diffDir);

            var results = new List<TestComparison>();
            var keys = recordsA.Keys.Concat(recordsB.Keys.Where(k => !recordsA.ContainsKey(k))).ToList();
            for (var t = 0; t < keys.Count; t++)
            {
                TestRecord a, b;
                recordsA.TryGetValue(keys[t], out a);
                recordsB.TryGetValue(keys[t], out b);
                var test = new TestComparison { Key = keys[t], A = a, B = b };
                results.Add(test);
                if (a == null || b == null) continue;

                if (a.Outcome != b.Outcome) test.Notes.Add("テスト結果が違います（" + a.Outcome + " / " + b.Outcome + "）");

                var capturesA = a.Steps.Where(s => s.Kind == StepKind.Capture).ToList();
                var capturesB = b.Steps.Where(s => s.Kind == StepKind.Capture).ToList();
                for (var i = 0; i < Math.Max(capturesA.Count, capturesB.Count); i++)
                {
                    var step = new StepComparison
                    {
                        No = i + 1,
                        A = i < capturesA.Count ? capturesA[i] : null,
                        B = i < capturesB.Count ? capturesB[i] : null,
                    };
                    test.Steps.Add(step);
                    if (step.A == null || step.B == null) continue;

                    CompareStep(step, a, b, Path.Combine(diffDir, string.Format("{0:0000}_{1:00}.png", t + 1, i + 1)), options);
                }
            }
            return results;
        }

        private static void CompareStep(StepComparison step, TestRecord a, TestRecord b, string diffPath, CompareOptions options)
        {
            if (step.A.Text != step.B.Text)
            {
                step.Notes.Add("キャプチャの名前が違います（" + step.A.Text + " / " + step.B.Text + "）。手順がずれている可能性があります。");
            }

            // 比較対象外: 両方の指定を合わせる
            var maskAreas = (step.A.Masks ?? new List<MaskArea>()).Concat(step.B.Masks ?? new List<MaskArea>()).ToList();
            var maskIds = new HashSet<string>(maskAreas.Select(m => m.Id));
            var maskRects = maskAreas.Select(m => new Rectangle(m.X, m.Y, m.Width, m.Height)).ToList();

            step.Image = ImageComparer.Compare(
                Path.Combine(a.Folder, step.A.CompareImage), Path.Combine(b.Folder, step.B.CompareImage),
                maskRects, options.Tolerance, diffPath);
            step.DiffImage = diffPath;

            step.Values = CompareValues(step.A.Controls, step.B.Controls, maskIds);
        }

        public static List<ValueDifference> CompareValues(List<ControlSnapshot> a, List<ControlSnapshot> b, ICollection<string> excludedIds)
        {
            var diffs = new List<ValueDifference>();
            var mapA = (a ?? new List<ControlSnapshot>()).ToDictionary(c => c.Id);
            var mapB = (b ?? new List<ControlSnapshot>()).ToDictionary(c => c.Id);
            var ids = mapA.Keys.Concat(mapB.Keys.Where(k => !mapA.ContainsKey(k)));

            foreach (var id in ids)
            {
                if (excludedIds.Contains(id)) continue;
                ControlSnapshot ca, cb;
                mapA.TryGetValue(id, out ca);
                mapB.TryGetValue(id, out cb);
                var label = (ca ?? cb).Label;

                if (ca == null || cb == null)
                {
                    diffs.Add(new ValueDifference { Id = id, Label = label, What = "項目の有無", A = ca == null ? "（なし）" : "あり", B = cb == null ? "（なし）" : "あり" });
                    continue;
                }
                if (ca.Value != cb.Value) diffs.Add(new ValueDifference { Id = id, Label = label, What = "値", A = ca.Value, B = cb.Value });
                if (ca.Enabled != cb.Enabled) diffs.Add(new ValueDifference { Id = id, Label = label, What = "有効/無効", A = ca.Enabled ? "有効" : "無効", B = cb.Enabled ? "有効" : "無効" });
                if (ca.Type != cb.Type) diffs.Add(new ValueDifference { Id = id, Label = label, What = "部品の種類", A = ca.Type, B = cb.Type });
                if (ca.X != cb.X || ca.Y != cb.Y || ca.Width != cb.Width || ca.Height != cb.Height)
                {
                    diffs.Add(new ValueDifference { Id = id, Label = label, What = "位置・サイズ", A = Bounds(ca), B = Bounds(cb) });
                }
            }
            return diffs;
        }

        private static string Bounds(ControlSnapshot c)
        {
            return string.Format("({0},{1}) {2}×{3}", c.X, c.Y, c.Width, c.Height);
        }

        private static string KeyOf(string dir, TestRecord record)
        {
            var full = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var folder = Path.GetFullPath(record.Folder);
            return folder.StartsWith(full, StringComparison.OrdinalIgnoreCase) ? folder.Substring(full.Length) : folder;
        }
    }
}
`````

### 24. `tests/GuiTestKit/Comparison/ImageComparer.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GuiTestKit.Comparison
{
    /// <summary>画像比較の結果。</summary>
    public class ImageDiffResult
    {
        public Size SizeA { get; set; }
        public Size SizeB { get; set; }
        public bool SizeMatches { get { return SizeA == SizeB; } }

        /// <summary>差異のあったピクセル数（サイズ違いではみ出した部分を含む）。</summary>
        public int DiffPixels { get; set; }

        /// <summary>比較したピクセル数（比較対象外の領域を除く）。</summary>
        public int ComparedPixels { get; set; }

        /// <summary>差異のあった範囲（差異がなければ Empty）。</summary>
        public Rectangle DiffBounds { get; set; }

        public double DiffRatio { get { return ComparedPixels == 0 ? 0 : (double)DiffPixels / ComparedPixels; } }
    }

    /// <summary>
    /// 2枚の画像をピクセル単位で比較し、差異を赤く塗った差分画像を作る。
    /// 差分画像の色: 赤 = 差異、オレンジ = サイズ違いではみ出した部分、青い斜線 = 比較対象外、薄いグレー = 一致。
    /// </summary>
    public static class ImageComparer
    {
        private const int Red = unchecked((int)0xFFFF0000);
        private const int Orange = unchecked((int)0xFFFF9900);
        private const int MaskDark = unchecked((int)0xFF9DB4D0);
        private const int MaskLight = unchecked((int)0xFFDCE5F0);

        /// <param name="tolerance">RGB各色の差がこの値以下なら同じとみなす（0 = 完全一致）。</param>
        /// <param name="diffImagePath">差分画像の保存先（null なら保存しない）。</param>
        public static ImageDiffResult Compare(string pathA, string pathB, IList<Rectangle> masks, int tolerance, string diffImagePath)
        {
            int widthA, heightA, widthB, heightB;
            var a = LoadPixels(pathA, out widthA, out heightA);
            var b = LoadPixels(pathB, out widthB, out heightB);

            var width = Math.Max(widthA, widthB);
            var height = Math.Max(heightA, heightB);
            var mask = BuildMask(width, height, masks);
            var output = new int[width * height];

            int diff = 0, compared = 0;
            int left = int.MaxValue, top = int.MaxValue, right = -1, bottom = -1;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y * width + x;
                    if (mask[i])
                    {
                        output[i] = ((x + y) / 4) % 2 == 0 ? MaskDark : MaskLight;
                        continue;
                    }

                    compared++;
                    var inA = x < widthA && y < heightA;
                    var inB = x < widthB && y < heightB;
                    bool different;
                    if (!inA || !inB)
                    {
                        output[i] = Orange;
                        different = true;
                    }
                    else
                    {
                        var ca = a[y * widthA + x];
                        var cb = b[y * widthB + x];
                        different = MaxChannelDiff(ca, cb) > tolerance;
                        output[i] = different ? Red : Faded(cb);
                    }

                    if (different)
                    {
                        diff++;
                        if (x < left) left = x;
                        if (y < top) top = y;
                        if (x > right) right = x;
                        if (y > bottom) bottom = y;
                    }
                }
            }

            var result = new ImageDiffResult
            {
                SizeA = new Size(widthA, heightA),
                SizeB = new Size(widthB, heightB),
                DiffPixels = diff,
                ComparedPixels = compared,
                DiffBounds = diff == 0 ? Rectangle.Empty : Rectangle.FromLTRB(left, top, right + 1, bottom + 1),
            };

            if (diffImagePath != null) SaveDiffImage(diffImagePath, output, width, height, result.DiffBounds);
            return result;
        }

        private static int[] LoadPixels(string path, out int width, out int height)
        {
            // new Bitmap(path) はファイルをロックし続けるので、読み込んだらすぐ閉じる
            using (var original = new Bitmap(path))
            using (var bitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap)) g.DrawImage(original, 0, 0, original.Width, original.Height);
                width = bitmap.Width;
                height = bitmap.Height;
                var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    var pixels = new int[width * height];
                    for (var y = 0; y < height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * width, width);
                    }
                    return pixels;
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
        }

        private static bool[] BuildMask(int width, int height, IList<Rectangle> masks)
        {
            var mask = new bool[width * height];
            if (masks == null) return mask;
            foreach (var m in masks)
            {
                var r = Rectangle.Intersect(m, new Rectangle(0, 0, width, height));
                for (var y = r.Top; y < r.Bottom; y++)
                {
                    for (var x = r.Left; x < r.Right; x++) mask[y * width + x] = true;
                }
            }
            return mask;
        }

        private static int MaxChannelDiff(int a, int b)
        {
            var dr = Math.Abs(((a >> 16) & 0xFF) - ((b >> 16) & 0xFF));
            var dg = Math.Abs(((a >> 8) & 0xFF) - ((b >> 8) & 0xFF));
            var db = Math.Abs((a & 0xFF) - (b & 0xFF));
            return Math.Max(dr, Math.Max(dg, db));
        }

        /// <summary>一致した部分は、差異の赤が目立つように薄いグレーにする。</summary>
        private static int Faded(int color)
        {
            var gray = (((color >> 16) & 0xFF) * 3 + ((color >> 8) & 0xFF) * 6 + (color & 0xFF)) / 10;
            var light = 255 - (255 - gray) / 3;
            return unchecked((int)0xFF000000) | (light << 16) | (light << 8) | light;
        }

        private static void SaveDiffImage(string path, int[] pixels, int width, int height, Rectangle diffBounds)
        {
            using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    for (var y = 0; y < height; y++)
                    {
                        Marshal.Copy(pixels, y * width, IntPtr.Add(data.Scan0, y * data.Stride), width);
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                if (!diffBounds.IsEmpty)
                {
                    // 小さな差異も見落とさないよう、差異の範囲を枠で囲む
                    using (var g = Graphics.FromImage(bitmap))
                    using (var pen = new Pen(Color.Red, 2))
                    {
                        var frame = Rectangle.Inflate(diffBounds, 3, 3);
                        frame.Intersect(new Rectangle(0, 0, width - 1, height - 1));
                        g.DrawRectangle(pen, frame);
                    }
                }
                bitmap.Save(path, ImageFormat.Png);
            }
        }
    }
}
`````

### 25. `tests/GuiTestKit/Evidence/JsonFile.cs`

`````csharp
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GuiTestKit.Evidence
{
    /// <summary>
    /// record.json の読み書き。.NET Framework 標準の DataContractJsonSerializer を使い、
    /// 追加のライブラリなしで動くようにしている。
    /// </summary>
    public static class JsonFile
    {
        private static readonly DataContractJsonSerializerSettings Settings =
            new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };

        public static void Write<T>(string path, T value)
        {
            using (var stream = File.Create(path))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, new UTF8Encoding(false), false, true, "  "))
            {
                new DataContractJsonSerializer(typeof(T), Settings).WriteObject(writer, value);
                writer.Flush();
            }
        }

        public static T Read<T>(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return (T)new DataContractJsonSerializer(typeof(T), Settings).ReadObject(stream);
            }
        }
    }
}
`````

### 26. `tests/GuiTestKit/Evidence/Records.cs`

`````csharp
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuiTestKit.Evidence
{
    // ------------------------------------------------------------------
    // エビデンスの記録内容。テスト1件ごとに record.json として保存し、
    // HTMLレポートとVB/C#比較ツールの両方がこれを読む。
    // ------------------------------------------------------------------

    /// <summary>テスト1件分の記録。</summary>
    [DataContract]
    public class TestRecord
    {
        [DataMember(Order = 1)] public string Target { get; set; }
        [DataMember(Order = 2)] public string Scenario { get; set; }
        [DataMember(Order = 3)] public string TestName { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)] public string CaseId { get; set; }
        [DataMember(Order = 5)] public string Outcome { get; set; }
        [DataMember(Order = 6)] public string StartedAt { get; set; }
        [DataMember(Order = 7)] public string FinishedAt { get; set; }
        [DataMember(Order = 8)] public List<StepRecord> Steps { get; set; }

        /// <summary>record.json があるフォルダ（読み込み時に設定。保存はしない）。</summary>
        public string Folder { get; set; }

        public string DisplayName
        {
            get { return CaseId == null ? TestName : TestName + " [" + CaseId + "]"; }
        }

        public TestRecord()
        {
            Steps = new List<StepRecord>();
        }
    }

    /// <summary>記録の種類。テスト中に起きた順に並べる。</summary>
    public static class StepKind
    {
        public const string Operation = "操作";
        public const string Check = "確認";
        public const string Capture = "キャプチャ";
    }

    /// <summary>テスト中の1件の出来事（操作・確認・キャプチャ）。</summary>
    [DataContract]
    public class StepRecord
    {
        [DataMember(Order = 1)] public string Kind { get; set; }
        [DataMember(Order = 2)] public string Text { get; set; }

        // ---- 確認（Kind = 確認） ----
        [DataMember(Order = 10, EmitDefaultValue = false)] public string Expected { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)] public string Actual { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)] public bool? Passed { get; set; }

        // ---- キャプチャ（Kind = キャプチャ） ----
        [DataMember(Order = 20, EmitDefaultValue = false)] public int CaptureNo { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)] public string WindowId { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)] public string WindowTitle { get; set; }

        /// <summary>人が見る用（タイトルバー込み、親画面も含む）。record.json からの相対パス。</summary>
        [DataMember(Order = 23, EmitDefaultValue = false)] public string EvidenceImage { get; set; }

        /// <summary>比較用（前面ウィンドウの中身部分だけ）。record.json からの相対パス。</summary>
        [DataMember(Order = 24, EmitDefaultValue = false)] public string CompareImage { get; set; }

        /// <summary>失敗時のみ：デスクトップ全体。record.json からの相対パス。</summary>
        [DataMember(Order = 25, EmitDefaultValue = false)] public string DesktopImage { get; set; }

        /// <summary>撮影時点の画面の全項目の値。</summary>
        [DataMember(Order = 26, EmitDefaultValue = false)] public List<ControlSnapshot> Controls { get; set; }

        /// <summary>比較対象外の項目（値の比較からも、画像の比較からも外す）。</summary>
        [DataMember(Order = 27, EmitDefaultValue = false)] public List<MaskArea> Masks { get; set; }
    }

    /// <summary>画面の1項目の状態。</summary>
    [DataContract]
    public class ControlSnapshot
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string Type { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)] public string Label { get; set; }
        [DataMember(Order = 4)] public string Value { get; set; }
        [DataMember(Order = 5)] public bool Enabled { get; set; }

        /// <summary>比較用画像の左上を (0,0) とした位置。</summary>
        [DataMember(Order = 6)] public int X { get; set; }
        [DataMember(Order = 7)] public int Y { get; set; }
        [DataMember(Order = 8)] public int Width { get; set; }
        [DataMember(Order = 9)] public int Height { get; set; }
    }

    /// <summary>比較対象外の項目と、比較用画像上の位置。</summary>
    [DataContract]
    public class MaskArea
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public int X { get; set; }
        [DataMember(Order = 3)] public int Y { get; set; }
        [DataMember(Order = 4)] public int Width { get; set; }
        [DataMember(Order = 5)] public int Height { get; set; }
    }
}
`````

### 27. `tests/GuiTestKit/GuiTestKit.csproj`

`````xml
<Project Sdk="Microsoft.NET.Sdk">

  <!--
    GUIテストの共通基盤（製品に依存しない部分）。
    製品ごとのテストプロジェクト（Page クラス・テストケース）と、ツール（GuiTestTool）から参照する。
  -->
  <PropertyGroup>
    <TargetFramework>net462</TargetFramework>
    <RootNamespace>GuiTestKit</RootNamespace>
    <AssemblyName>GuiTestKit</AssemblyName>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="System.Drawing" />
    <Reference Include="System.Runtime.Serialization" />
    <Reference Include="System.Windows.Forms" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="FlaUI.UIA3" Version="4.0.0" />
    <PackageReference Include="MSTest.TestFramework" Version="3.6.4" />
    <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
  </ItemGroup>

</Project>
`````

### 28. `tests/GuiTestKit/Reporting/Html.cs`

`````csharp
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace GuiTestKit.Reporting
{
    /// <summary>HTMLレポート共通の部品（エスケープ・相対リンク・スタイル）。</summary>
    internal static class Html
    {
        public static string E(string text)
        {
            return WebUtility.HtmlEncode(text ?? string.Empty);
        }

        /// <summary>fromDir から見た target への相対URL（日本語・記号はエンコード）。</summary>
        public static string RelativeUrl(string fromDir, string target)
        {
            var from = new Uri(Path.GetFullPath(fromDir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            var to = new Uri(Path.GetFullPath(target));
            if (from.Scheme != to.Scheme || !string.Equals(from.Host, to.Host, StringComparison.OrdinalIgnoreCase)
                || Path.GetPathRoot(from.LocalPath) != Path.GetPathRoot(to.LocalPath))
            {
                return to.AbsoluteUri;
            }
            var relative = Uri.UnescapeDataString(from.MakeRelativeUri(to).ToString());
            return string.Join("/", relative.Split('/').Select(s => s == ".." ? s : Uri.EscapeDataString(s)));
        }

        public const string Style = @"
:root { --fg:#1f2328; --muted:#656d76; --line:#d0d7de; --bg:#ffffff; --sub:#f6f8fa;
        --ok:#1a7f37; --okbg:#dafbe1; --ng:#cf222e; --ngbg:#ffebe9; --warn:#9a6700; --warnbg:#fff8c5; }
* { box-sizing: border-box; }
body { margin: 0; padding: 16px 24px 48px; font-family: 'Yu Gothic UI','Meiryo UI',Meiryo,sans-serif;
       font-size: 14px; color: var(--fg); background: var(--bg); }
h1 { font-size: 20px; margin: 0 0 4px; }
h2 { font-size: 16px; margin: 32px 0 8px; padding-bottom: 4px; border-bottom: 2px solid var(--line); }
h3 { font-size: 14px; margin: 16px 0 6px; }
.meta { color: var(--muted); margin-bottom: 16px; }
table { border-collapse: collapse; margin: 8px 0; }
th, td { border: 1px solid var(--line); padding: 4px 8px; text-align: left; vertical-align: top; }
th { background: var(--sub); font-weight: 600; white-space: nowrap; }
td.num { text-align: right; }
.badge { display: inline-block; padding: 1px 8px; border-radius: 10px; font-size: 12px; font-weight: 600; }
.ok { color: var(--ok); background: var(--okbg); }
.ng { color: var(--ng); background: var(--ngbg); }
.warn { color: var(--warn); background: var(--warnbg); }
.toolbar { position: sticky; top: 0; background: var(--bg); padding: 8px 0; border-bottom: 1px solid var(--line); z-index: 1; }
.test { border: 1px solid var(--line); border-radius: 6px; padding: 8px 16px 16px; margin: 16px 0; }
.test > h3 { margin-top: 8px; }
ol.steps { padding-left: 20px; }
ol.steps li { margin: 4px 0; }
li.op { color: var(--fg); }
li.check { list-style: none; margin-left: -20px; }
li.capture { list-style: none; margin: 12px 0 12px -20px; }
.caption { font-weight: 600; margin-bottom: 4px; }
img.shot { max-width: 100%; border: 1px solid var(--line); cursor: zoom-in; }
.pair { display: flex; gap: 12px; flex-wrap: wrap; align-items: flex-start; }
.pair > div { flex: 1 1 300px; min-width: 0; }
details { margin-top: 4px; }
summary { cursor: pointer; color: var(--muted); }
.diffcell { background: var(--ngbg); }
.masked { color: var(--muted); }
";

        /// <summary>チェックボックスで「問題のあるものだけ表示」を切り替えるスクリプト。</summary>
        public const string FilterScript = @"
<script>
function applyFilter(cb){document.querySelectorAll('[data-status]').forEach(function(el){
 el.style.display=(cb.checked&&el.getAttribute('data-status')==='ok')?'none':'';});}
window.addEventListener('load',function(){var cb=document.getElementById('filter');if(cb)applyFilter(cb);});
</script>";
    }
}
`````

### 29. `tests/GuiTestKit/Reporting/RunReportWriter.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuiTestKit.Evidence;

namespace GuiTestKit.Reporting
{
    /// <summary>
    /// 1回の実行分のエビデンス（record.json 群）から、人がレビューするための report.html を作る。
    /// テスト名・操作手順・入力値・確認結果・キャプチャを時系列で並べる。
    /// </summary>
    public static class RunReportWriter
    {
        /// <param name="runDir">Evidence\実行日時 のフォルダ</param>
        /// <returns>作成した report.html のパス</returns>
        public static string Write(string runDir)
        {
            var records = LoadRecords(runDir);
            var path = Path.Combine(runDir, "report.html");
            File.WriteAllText(path, Build(runDir, records), new UTF8Encoding(true));
            return path;
        }

        public static List<TestRecord> LoadRecords(string dir)
        {
            var records = new List<TestRecord>();
            foreach (var file in Directory.GetFiles(dir, "record.json", SearchOption.AllDirectories))
            {
                var record = JsonFile.Read<TestRecord>(file);
                record.Folder = Path.GetDirectoryName(file);
                records.Add(record);
            }
            return records
                .OrderBy(r => r.Target).ThenBy(r => r.Scenario).ThenBy(r => r.StartedAt)
                .ToList();
        }

        private static string Build(string runDir, List<TestRecord> records)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html lang=\"ja\"><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>GUIテスト結果 " + Html.E(Path.GetFileName(runDir)) + "</title>");
            sb.AppendLine("<style>" + Html.Style + "</style></head><body>");
            sb.AppendLine("<h1>GUIテスト結果</h1>");
            sb.AppendLine("<div class=\"meta\">実行: " + Html.E(Path.GetFileName(runDir)) + "　／　レポート作成: "
                + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "</div>");

            // ---- 集計 ----
            sb.AppendLine("<table><tr><th>対象</th><th>テスト数</th><th>合格</th><th>不合格</th></tr>");
            foreach (var g in records.GroupBy(r => r.Target))
            {
                var ng = g.Count(r => r.Outcome != "合格");
                sb.AppendFormat("<tr><td>{0}</td><td class=\"num\">{1}</td><td class=\"num\">{2}</td><td class=\"num\">{3}</td></tr>\n",
                    Html.E(g.Key), g.Count(), g.Count() - ng, ng == 0 ? "0" : "<span class=\"badge ng\">" + ng + "</span>");
            }
            sb.AppendLine("</table>");

            sb.AppendLine("<div class=\"toolbar\"><label><input type=\"checkbox\" id=\"filter\" onchange=\"applyFilter(this)\"> 不合格のみ表示</label></div>");

            // ---- 一覧 ----
            sb.AppendLine("<h2>テスト一覧</h2>");
            sb.AppendLine("<table><tr><th>No</th><th>対象</th><th>画面（テストクラス）</th><th>テスト</th><th>結果</th><th>キャプチャ</th></tr>");
            for (var i = 0; i < records.Count; i++)
            {
                var r = records[i];
                sb.AppendFormat("<tr data-status=\"{0}\"><td class=\"num\">{1}</td><td>{2}</td><td>{3}</td><td><a href=\"#t{1}\">{4}</a></td><td>{5}</td><td class=\"num\">{6}</td></tr>\n",
                    Status(r), i + 1, Html.E(r.Target), Html.E(r.Scenario), Html.E(r.DisplayName), Badge(r.Outcome),
                    r.Steps.Count(s => s.Kind == StepKind.Capture));
            }
            sb.AppendLine("</table>");

            // ---- 詳細 ----
            sb.AppendLine("<h2>テスト詳細</h2>");
            for (var i = 0; i < records.Count; i++)
            {
                AppendTest(sb, runDir, records[i], i + 1);
            }

            sb.AppendLine(Html.FilterScript);
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static void AppendTest(StringBuilder sb, string runDir, TestRecord r, int no)
        {
            sb.AppendFormat("<div class=\"test\" id=\"t{0}\" data-status=\"{1}\">\n", no, Status(r));
            sb.AppendFormat("<h3>{0}. {1}　{2}</h3>\n", no, Html.E(r.DisplayName), Badge(r.Outcome));
            sb.AppendFormat("<div class=\"meta\">対象: {0}　／　画面: {1}　／　{2} ～ {3}</div>\n",
                Html.E(r.Target), Html.E(r.Scenario), Html.E(r.StartedAt), Html.E(r.FinishedAt));

            sb.AppendLine("<ol class=\"steps\">");
            foreach (var s in r.Steps)
            {
                if (s.Kind == StepKind.Operation)
                {
                    sb.AppendLine("<li class=\"op\">" + Html.E(s.Text) + "</li>");
                }
                else if (s.Kind == StepKind.Check)
                {
                    var ok = s.Passed == true;
                    sb.AppendFormat("<li class=\"check\"><span class=\"badge {0}\">{1}</span> 確認: {2}　期待値「{3}」　実際「{4}」</li>\n",
                        ok ? "ok" : "ng", ok ? "OK" : "NG", Html.E(s.Text), Html.E(s.Expected), Html.E(s.Actual));
                }
                else if (s.Kind == StepKind.Capture)
                {
                    AppendCapture(sb, runDir, r, s);
                }
            }
            sb.AppendLine("</ol>");

            if (r.Outcome != "合格" && !r.Steps.Any(s => s.Passed == false))
            {
                sb.AppendLine("<div class=\"badge warn\">確認以外の箇所でエラーになっています。詳細はテスト結果(.trx)のエラーメッセージを参照してください。</div>");
            }
            sb.AppendLine("</div>");
        }

        private static void AppendCapture(StringBuilder sb, string runDir, TestRecord r, StepRecord s)
        {
            var image = Html.RelativeUrl(runDir, Path.Combine(r.Folder, s.EvidenceImage));
            sb.AppendLine("<li class=\"capture\">");
            sb.AppendFormat("<div class=\"caption\">{0:00}. {1}　<span class=\"meta\">（{2}）</span></div>\n",
                s.CaptureNo, Html.E(s.Text), Html.E(s.WindowTitle));
            sb.AppendFormat("<a href=\"{0}\" target=\"_blank\"><img class=\"shot\" src=\"{0}\" loading=\"lazy\" alt=\"{1}\"></a>\n", image, Html.E(s.Text));
            if (s.DesktopImage != null)
            {
                var desktop = Html.RelativeUrl(runDir, Path.Combine(r.Folder, s.DesktopImage));
                sb.AppendFormat("<div><a href=\"{0}\" target=\"_blank\">デスクトップ全体のキャプチャ</a></div>\n", desktop);
            }
            if (s.Controls != null && s.Controls.Count > 0)
            {
                var masked = new HashSet<string>((s.Masks ?? new List<MaskArea>()).Select(m => m.Id));
                sb.AppendFormat("<details><summary>画面の項目値（{0}項目）</summary>\n", s.Controls.Count);
                sb.AppendLine("<table><tr><th>項目ID</th><th>項目名</th><th>種類</th><th>値</th><th>状態</th></tr>");
                foreach (var c in s.Controls)
                {
                    sb.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>\n",
                        masked.Contains(c.Id) ? " class=\"masked\"" : "", Html.E(c.Id), Html.E(c.Label), Html.E(c.Type),
                        Html.E(c.Value).Replace("\n", "<br>") + (masked.Contains(c.Id) ? "（比較対象外）" : ""), c.Enabled ? "有効" : "無効");
                }
                sb.AppendLine("</table></details>");
            }
            sb.AppendLine("</li>");
        }

        private static string Status(TestRecord r)
        {
            return r.Outcome == "合格" ? "ok" : "ng";
        }

        private static string Badge(string outcome)
        {
            if (outcome == "合格") return "<span class=\"badge ok\">合格</span>";
            return "<span class=\"badge ng\">" + Html.E(outcome ?? "未完了") + "</span>";
        }
    }
}
`````

### 30. `tests/GuiTestKit/Testing/GuiTestBase.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using GuiTestKit.Automation;
using GuiTestKit.Evidence;
using GuiTestKit.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiTestKit.Testing
{
    /// <summary>
    /// GUIテストの共通基底クラス。
    /// ・テストごとに対象EXEを起動・終了する（対象は runsettings の Target = VB / CS で切り替え）
    /// ・テストデータ初期化コマンドの実行
    /// ・操作／確認／キャプチャをエビデンスとして記録し、HTMLレポートを出す
    /// </summary>
    public abstract class GuiTestBase
    {
        // テスト実行1回分で共通のフォルダ名（例: Evidence\20260926_171500\...）
        private static readonly string RunStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        private static string _runDir;
        private static uint? _originalCaretBlink;

        static GuiTestBase()
        {
            NativeMethods.EnableDpiAwareness();
        }

        public TestContext TestContext { get; set; }

        protected Application App { get; private set; }
        protected UIA3Automation Automation { get; private set; }
        protected Window MainWindow { get; private set; }
        protected TestSession Session { get; private set; }

        /// <summary>テスト対象（runsettings の Target。例: VB / CS）。</summary>
        protected string Target { get; private set; }

        /// <summary>DataRow のケース番号など。エビデンスのフォルダ名に付く。キャプチャより前に設定する。</summary>
        protected string CaseId
        {
            get { return Session.Record.CaseId; }
            set { Session.SetCaseId(value); }
        }

        [TestInitialize]
        public void StartApplication()
        {
            Target = GetParameter("Target");
            var exePath = ResolvePath(GetParameter("TargetExe" + Target));
            Assert.IsTrue(File.Exists(exePath),
                "テスト対象の EXE が見つかりません: " + exePath + Environment.NewLine + "先にビルドするか、runsettings のパスを確認してください。");

            _runDir = _runDir ?? Path.Combine(ResolvePath(GetParameter("EvidenceDir")), RunStamp);
            var resetMessage = RunResetCommand();

            // 業務アプリは設定ファイルを作業フォルダから読むことが多いので、EXE のフォルダで起動する
            App = Application.Launch(new ProcessStartInfo(exePath) { WorkingDirectory = Path.GetDirectoryName(exePath) });
            Automation = new UIA3Automation();
            MainWindow = App.GetMainWindow(Automation, TimeSpan.FromSeconds(30));
            Assert.IsNotNull(MainWindow, "メイン画面が表示されませんでした。");
            MainWindow.SetForeground();

            DisableCaretBlink();

            Session = new TestSession(App, Automation, MainWindow, _runDir, new TestRecord
            {
                Target = Target,
                Scenario = GetType().Name,
                TestName = TestContext.TestName,
                StartedAt = Now(),
            });
            TestSession.Current = Session;
            if (resetMessage != null) Session.LogOperation(resetMessage);
        }

        [TestCleanup]
        public void StopApplication()
        {
            try
            {
                if (Session != null)
                {
                    var passed = TestContext.CurrentTestOutcome == UnitTestOutcome.Passed;
                    // 失敗したテストは、終了直前の画面とデスクトップ全体を必ず残す
                    if (!passed)
                    {
                        try { Session.Snap("失敗時の画面", includeDesktop: true); }
                        catch (Exception ex) { Session.LogOperation("失敗時の画面を撮れませんでした: " + ex.Message); }
                    }
                    Session.Record.Outcome = passed ? "合格" : "不合格";
                    Session.Record.FinishedAt = Now();
                    Session.Save();
                    TestContext.AddResultFile(Path.Combine(Session.TestDir, "record.json"));
                }
            }
            finally
            {
                TestSession.Current = null;
                RestoreCaretBlink();
                CloseApplication();
            }
        }

        /// <summary>
        /// 実行した全テストのHTMLレポートを出す。
        /// テストプロジェクトの [AssemblyCleanup] から呼ぶ（MSTest の仕様で、基盤側には置けないため）。
        /// </summary>
        public static void WriteRunReport()
        {
            if (_runDir != null && Directory.Exists(_runDir)) RunReportWriter.Write(_runDir);
        }

        // ------------------------------------------------------------------
        // テストケースから使う操作
        // ------------------------------------------------------------------

        /// <summary>前面の画面をキャプチャする（人が見る用・比較用・全項目の値）。</summary>
        protected void Snap(string label)
        {
            Session.Snap(label);
        }

        /// <summary>レポートにメモを残す。</summary>
        protected void Log(string text)
        {
            Session.LogOperation(text);
        }

        /// <summary>期待値と一致するか確認する（結果はレポートにも出る）。</summary>
        protected void Check(string item, string expected, string actual)
        {
            var passed = expected == actual;
            Session.LogCheck(item, expected, actual, passed);
            if (!passed) Assert.Fail("[" + item + "] 期待値: " + expected + " / 実際: " + actual);
        }

        protected void Check(string item, bool expected, bool actual)
        {
            Check(item, ToText(expected), ToText(actual));
        }

        /// <summary>画面の表示が変わるのを待ってから確認する（計算結果の反映待ちなど）。</summary>
        protected void CheckEventually(string item, string expected, Func<string> actual, TimeSpan? timeout = null)
        {
            string last = null;
            Retry.WhileFalse(() => (last = actual()) == expected, timeout ?? ScreenPageBase.DefaultTimeout);
            Check(item, expected, last);
        }

        // ------------------------------------------------------------------
        // 内部処理
        // ------------------------------------------------------------------

        /// <summary>runsettings の ResetCommand（テストデータ初期化）をテストごとに実行する。</summary>
        private string RunResetCommand()
        {
            var command = GetParameter("ResetCommand", required: false);
            if (string.IsNullOrWhiteSpace(command)) return null;

            var psi = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using (var process = Process.Start(psi))
            {
                var output = process.StandardOutput.ReadToEndAsync();
                var error = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(5 * 60 * 1000))
                {
                    process.Kill();
                    Assert.Fail("テストデータ初期化コマンドが5分で終わりませんでした: " + command);
                }
                if (process.ExitCode != 0)
                {
                    Assert.Fail("テストデータ初期化コマンドが失敗しました（終了コード " + process.ExitCode + "）: " + command
                        + Environment.NewLine + output.Result + error.Result);
                }
            }
            return "テストデータを初期化: " + command;
        }

        private void CloseApplication()
        {
            if (App != null)
            {
                try
                {
                    // まず普通に閉じる（アプリの後始末を走らせる）。閉じなければ強制終了
                    App.Close(false);
                    using (var process = Process.GetProcessById(App.ProcessId))
                    {
                        if (!process.WaitForExit(3000)) process.Kill();
                    }
                }
                catch (ArgumentException)
                {
                    // すでに終了している
                }
                catch (InvalidOperationException)
                {
                    // すでに終了している
                }
                App.Dispose();
                App = null;
            }
            if (Automation != null)
            {
                Automation.Dispose();
                Automation = null;
            }
        }

        /// <summary>
        /// カーソル(キャレット)の点滅を止める（常に表示）。撮るタイミングで写ったり写らなかったりするのを防ぐ。
        /// Windows 全体の設定なので、テスト後に元に戻す。
        /// </summary>
        private static void DisableCaretBlink()
        {
            if (_originalCaretBlink == null) _originalCaretBlink = NativeMethods.GetCaretBlink();
            NativeMethods.SetCaretBlink(uint.MaxValue);
        }

        private static void RestoreCaretBlink()
        {
            if (_originalCaretBlink != null) NativeMethods.SetCaretBlink(_originalCaretBlink.Value);
        }

        private string GetParameter(string name, bool required = true)
        {
            object raw = null;
            try { raw = TestContext.Properties[name]; }
            catch (KeyNotFoundException) { }

            var value = raw as string;
            if (required && string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    "runsettings のパラメータ '" + name + "' がありません。gui.runsettings を指定して実行してください。");
            }
            return value;
        }

        /// <summary>相対パスはテストの出力フォルダ（bin\Debug\net462）基準の絶対パスにする。</summary>
        private static string ResolvePath(string value)
        {
            return Path.IsPathRooted(value)
                ? value
                : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value));
        }

        private static string ToText(bool value)
        {
            return value ? "はい" : "いいえ";
        }

        private static string Now()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
`````

### 31. `tests/GuiTestKit/Testing/TestSession.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using GuiTestKit.Automation;
using GuiTestKit.Evidence;

namespace GuiTestKit.Testing
{
    /// <summary>
    /// 実行中のテスト1件分の状態（起動中のアプリ、エビデンスの記録）。
    /// GUIテストは並列実行しないので、実行中のテストは常に1件（Current）。
    /// Page クラスは Current を通して操作ログを記録する。
    /// </summary>
    public sealed class TestSession
    {
        /// <summary>マウス移動後、ボタンのハイライト等が消えるのを待つ時間。</summary>
        public static int CaptureSettleMilliseconds = 200;

        private readonly List<ScreenPageBase> _pages = new List<ScreenPageBase>();
        private readonly string _runDir;
        private int _captureNo;
        private string _testDir;

        internal TestSession(Application app, AutomationBase automation, Window mainWindow, string runDir, TestRecord record)
        {
            App = app;
            Automation = automation;
            MainWindow = mainWindow;
            Record = record;
            _runDir = runDir;
        }

        public static TestSession Current { get; internal set; }

        public Application App { get; private set; }
        public AutomationBase Automation { get; private set; }
        public Window MainWindow { get; private set; }
        public TestRecord Record { get; private set; }

        /// <summary>このテストのエビデンスフォルダ: 実行日時\対象\シナリオ\テスト名[_ケース番号]</summary>
        public string TestDir
        {
            get
            {
                if (_testDir == null)
                {
                    var folder = Record.CaseId == null ? Record.TestName : Record.TestName + "_" + Record.CaseId;
                    _testDir = Path.Combine(_runDir, SafeName(Record.Target), SafeName(Record.Scenario), SafeName(folder));
                }
                return _testDir;
            }
        }

        internal void SetCaseId(string caseId)
        {
            if (_testDir != null) throw new InvalidOperationException("CaseId はキャプチャを撮る前に設定してください。");
            Record.CaseId = caseId;
        }

        internal void RegisterPage(ScreenPageBase page)
        {
            _pages.Add(page);
        }

        // ------------------------------------------------------------------
        // 記録
        // ------------------------------------------------------------------

        public void LogOperation(string text)
        {
            Record.Steps.Add(new StepRecord { Kind = StepKind.Operation, Text = text });
        }

        public void LogCheck(string item, string expected, string actual, bool passed)
        {
            Record.Steps.Add(new StepRecord
            {
                Kind = StepKind.Check,
                Text = item,
                Expected = expected,
                Actual = actual,
                Passed = passed,
            });
        }

        /// <summary>
        /// 前面の画面をキャプチャする。1回で次の3つを保存する。
        /// ・人が見る用: 前面の画面＋その親画面（タイトルバー込み）
        /// ・比較用　　: 前面の画面の中身部分だけ（タイトルバー・枠なし）
        /// ・画面の全項目の値（record.json に記録）
        /// </summary>
        public StepRecord Snap(string label, bool includeDesktop = false)
        {
            var front = FindFrontWindow();
            var hWnd = front.Properties.NativeWindowHandle.Value;

            var evidenceBounds = NativeMethods.GetVisibleWindowBounds(hWnd);
            for (var owner = NativeMethods.GetOwner(hWnd); owner != IntPtr.Zero; owner = NativeMethods.GetOwner(owner))
            {
                evidenceBounds = Rectangle.Union(evidenceBounds, NativeMethods.GetVisibleWindowBounds(owner));
            }

            PrepareForCapture(front, evidenceBounds);
            var clientBounds = NativeMethods.GetClientBoundsOnScreen(hWnd);

            _captureNo++;
            var fileName = string.Format("{0:00}_{1}.png", _captureNo, SafeName(label));
            Directory.CreateDirectory(Path.Combine(TestDir, "compare"));

            var step = new StepRecord
            {
                Kind = StepKind.Capture,
                Text = label,
                CaptureNo = _captureNo,
                WindowId = front.Properties.AutomationId.ValueOrDefault,
                WindowTitle = front.Title,
                EvidenceImage = fileName,
                CompareImage = "compare/" + fileName,
            };
            Capture.Rectangle(evidenceBounds).ToFile(Path.Combine(TestDir, fileName));
            Capture.Rectangle(clientBounds).ToFile(Path.Combine(TestDir, "compare", fileName));

            if (includeDesktop)
            {
                step.DesktopImage = string.Format("{0:00}_{1}_デスクトップ.png", _captureNo, SafeName(label));
                Capture.Rectangle(NativeMethods.GetVirtualScreen()).ToFile(Path.Combine(TestDir, step.DesktopImage));
            }

            var page = _pages.LastOrDefault(p => SameWindow(p.Window, hWnd));
            var handlers = page != null ? page.CustomHandlers : CustomControls.Handlers;
            step.Controls = ControlAccess.ReadAll(front, handlers, clientBounds.Location);
            if (page != null) step.Masks = ResolveMasks(page, clientBounds.Location);

            Record.Steps.Add(step);
            Save();
            return step;
        }

        public void Save()
        {
            Directory.CreateDirectory(TestDir);
            JsonFile.Write(Path.Combine(TestDir, "record.json"), Record);
        }

        // ------------------------------------------------------------------
        // キャプチャの準備
        // ------------------------------------------------------------------

        /// <summary>
        /// 前面の画面を決める。基本は「アクティブなウィンドウ」。
        /// 別アプリが前面にある場合は、メイン画面から開いているモーダル画面を辿った一番上。
        /// </summary>
        private Window FindFrontWindow()
        {
            var hWnd = NativeMethods.GetForegroundWindowOf(App.ProcessId);
            if (hWnd != IntPtr.Zero)
            {
                var element = Automation.FromHandle(hWnd);
                if (element.ControlType == ControlType.Window) return element.AsWindow();
            }

            var window = MainWindow;
            while (true)
            {
                var modal = window.ModalWindows.FirstOrDefault();
                if (modal == null) return window;
                window = modal;
            }
        }

        /// <summary>
        /// 比較のノイズを減らす: 対象を最前面に出し、マウスを画面の外へ逃がす。
        /// （フォーカスは動かさない。フォーカス移動で入力チェック等のイベントが走る画面があるため）
        /// </summary>
        private static void PrepareForCapture(Window front, Rectangle bounds)
        {
            try { front.SetForeground(); } catch (Exception) { }

            var screen = NativeMethods.GetVirtualScreen();
            var right = new Point(bounds.Right + 40, bounds.Top + 10);
            var left = new Point(bounds.Left - 40, bounds.Top + 10);
            Mouse.Position = screen.Contains(right) ? right : screen.Contains(left) ? left : new Point(screen.Left + 1, screen.Top + 1);

            Wait.UntilInputIsProcessed();
            Thread.Sleep(CaptureSettleMilliseconds);
        }

        private static List<MaskArea> ResolveMasks(ScreenPageBase page, Point origin)
        {
            var masks = new List<MaskArea>();
            foreach (var id in page.CompareExcludedIds)
            {
                var element = page.FindOrNull(id);
                var bounds = element == null ? Rectangle.Empty : element.BoundingRectangle;
                masks.Add(new MaskArea
                {
                    Id = id,
                    X = bounds.X - origin.X,
                    Y = bounds.Y - origin.Y,
                    Width = bounds.Width,
                    Height = bounds.Height,
                });
            }
            return masks;
        }

        private static bool SameWindow(Window window, IntPtr hWnd)
        {
            try { return window.Properties.NativeWindowHandle.ValueOrDefault == hWnd; }
            catch (Exception) { return false; }
        }

        internal static string SafeName(string name)
        {
            var chars = name.ToCharArray();
            var invalid = Path.GetInvalidFileNameChars();
            for (var i = 0; i < chars.Length; i++)
            {
                if (Array.IndexOf(invalid, chars[i]) >= 0 || chars[i] == '#' || chars[i] == '%') chars[i] = '_';
            }
            return new string(chars);
        }
    }
}
`````

### 32. `tests/SampleApp.GuiTests/AssemblyInfo.cs`

`````csharp
using GuiTestKit.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// GUIテストは画面(フォーカス・マウス)を奪い合うため、並列実行しない
[assembly: DoNotParallelize]

namespace SampleApp.GuiTests
{
    /// <summary>テスト実行全体の前後処理。</summary>
    [TestClass]
    public class TestRunHooks
    {
        /// <summary>全テストの実行後に、エビデンスのHTMLレポート（Evidence\実行日時\report.html）を出す。</summary>
        [AssemblyCleanup]
        public static void WriteReport()
        {
            GuiTestBase.WriteRunReport();
        }
    }
}
`````

### 33. `tests/SampleApp.GuiTests/Pages/メイン画面Page.cs`

`````csharp
using FlaUI.Core.AutomationElements;
using GuiTestKit.Automation;

namespace SampleApp.GuiTests.Pages
{
    /// <summary>メイン画面（計算画面）。</summary>
    public class メイン画面Page : ScreenPageBase
    {
        public const string WindowId = "MainForm";

        public メイン画面Page(Window window) : base(window)
        {
        }

        public override ScreenValues BaseValues { get; } = new ScreenValues
        {
            { "txtA", "", "数値A" },
            { "txtB", "", "数値B" },
        };

        public string 結果 { get { return Get("lblResult"); } }

        public void 計算() { Press("btnCalc"); }

        /// <summary>エラーになる入力で計算ボタンを押す（メッセージボックスが開くのでマウスでクリックする）。</summary>
        public MessageBoxPage 計算_エラー表示() { return ClickAndWaitMessageBox("btnCalc"); }

        public void クリア() { Press("btnClear"); }

        public 顧客登録Page 顧客登録を開く()
        {
            return OpenScreen("btnCustomer", 顧客登録Page.WindowId, w => new 顧客登録Page(w));
        }
    }
}
`````

### 34. `tests/SampleApp.GuiTests/Pages/画面遷移.cs`

`````csharp
using GuiTestKit.Testing;

namespace SampleApp.GuiTests.Pages
{
    /// <summary>
    /// 画面遷移のルート集。対象の画面にたどり着くまでの手順は、ここにだけ書く。
    /// テストクラスの [TestInitialize] から呼び、各テストケースには遷移を書かない。
    /// 途中の画面が変わったときは、ここだけ直せばよい。
    ///
    /// 多段の遷移の例（実製品）:
    /// <code>
    /// public static 実績点検Page 実績点検画面へ(this メイン画面Page main, string 対象年月)
    /// {
    ///     Log("前提: 実績点検画面まで遷移（対象年月 " + 対象年月 + "）");
    ///     var menu = main.業務メニューを開く();
    ///     var 条件 = menu.点検条件を開く();
    ///     条件.Set("txtTaishoYm", 対象年月);
    ///     return 条件.実績点検を開く();
    /// }
    /// </code>
    /// </summary>
    public static class 画面遷移
    {
        public static 顧客登録Page 顧客登録画面へ(this メイン画面Page main)
        {
            Log("前提: 顧客登録画面まで遷移");
            return main.顧客登録を開く();
        }

        private static void Log(string text)
        {
            var session = TestSession.Current;
            if (session != null) session.LogOperation(text);
        }
    }
}
`````

### 35. `tests/SampleApp.GuiTests/Pages/顧客登録Page.cs`

`````csharp
using System.Collections.Generic;
using FlaUI.Core.AutomationElements;
using GuiTestKit.Automation;

namespace SampleApp.GuiTests.Pages
{
    /// <summary>
    /// 顧客登録画面。
    /// 基本値は GuiTestTool dump で画面から吸い出したひな形をもとにしている。
    /// </summary>
    public class 顧客登録Page : ScreenPageBase
    {
        public const string WindowId = "CustomerForm";

        public 顧客登録Page(Window window) : base(window)
        {
        }

        /// <summary>全項目を正しく入力した状態。テストケースではここからの差分だけを書く。</summary>
        public override ScreenValues BaseValues { get; } = new ScreenValues
        {
            { "txtCode", "000001", "顧客コード" },
            { "txtName", "テスト商事株式会社", "顧客名" },
            { "txtKana", "テストショウジ", "顧客名カナ" },
            { "cmbKubun", "法人", "区分" },
            { "txtCorpNo", "1234567890123", "法人番号" },
            { "txtTel", "03-1234-5678", "電話番号" },
            { "txtCreditLimit", "1000000", "与信限度額" },
            { "chkActive", ScreenValues.ON, "取引中" },
            { "rdoPayTransfer", ScreenValues.ON, "支払方法/振込" },
        };

        /// <summary>表示日時は実行するたびに変わるので、VB/C# の比較対象から外す。</summary>
        public override IEnumerable<string> CompareExcludedIds
        {
            get { return new[] { "lblTimestamp" }; }
        }

        public string 状態表示 { get { return Get("lblStatus"); } }

        public MessageBoxPage 登録() { return ClickAndWaitMessageBox("btnRegister"); }

        public void 閉じる()
        {
            Click("btnClose");
            WaitUntilClosed();
        }
    }
}
`````

### 36. `tests/SampleApp.GuiTests/SampleApp.GuiTests.csproj`

`````xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <!-- 既存の単体テストと同じ .NET Framework 4.6.2 で作る -->
    <TargetFramework>net462</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <!-- Visual Studio のテストエクスプローラーでも自動でこの設定ファイルを使う -->
    <RunSettingsFilePath>$(MSBuildProjectDirectory)\gui.runsettings</RunSettingsFilePath>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.6.4" />
    <PackageReference Include="MSTest.TestFramework" Version="3.6.4" />
    <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <!-- GUIテストの共通基盤（FlaUI・キャプチャ・レポートなど）。製品が変わってもそのまま使う -->
    <ProjectReference Include="..\GuiTestKit\GuiTestKit.csproj" />
  </ItemGroup>

  <!--
    ポイント: SampleApp / SampleAppVB へのプロジェクト参照は「しない」。
    ビルド済みの EXE を別プロセスとして起動し、外から操作する。
    テスト対象の EXE パスは gui.runsettings で指定する。
  -->

</Project>
`````

### 37. `tests/SampleApp.GuiTests/SampleAppTestBase.cs`

`````csharp
using GuiTestKit.Testing;
using SampleApp.GuiTests.Pages;

namespace SampleApp.GuiTests
{
    /// <summary>
    /// この製品のテストの基底クラス。起動直後の画面（メイン画面）を用意する。
    /// 製品共通の前処理（ログイン等）もここに書く。
    /// </summary>
    public abstract class SampleAppTestBase : GuiTestBase
    {
        private メイン画面Page _mainPage;

        static SampleAppTestBase()
        {
            // Spread などの市販部品を使う場合はここで登録する（全画面共通）。例:
            // CustomControls.Handlers.Add(new SpreadHandler(id => id.StartsWith("spd")));
        }

        protected メイン画面Page メイン画面
        {
            get { return _mainPage ?? (_mainPage = new メイン画面Page(MainWindow)); }
        }
    }
}
`````

### 38. `tests/SampleApp.GuiTests/gui.runsettings`

`````xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <TestRunParameters>
    <!--
      テスト対象: VB（移行前）/ CS（移行後）。
      run-gui-tests.ps1 -Target VB|CS で実行するとここを上書きする。
      Visual Studio のテストエクスプローラーで実行するときは、この値を書き換える。
    -->
    <Parameter name="Target" value="VB" />

    <!--
      Target ごとの EXE。名前は "TargetExe" + Target。
      相対パスはテストの出力フォルダ (tests\SampleApp.GuiTests\bin\Debug\net462\) からの相対。
      絶対パスも可。例: C:\Program Files\製品名\Product.exe
    -->
    <Parameter name="TargetExeVB" value="..\..\..\..\..\src\SampleAppVB\bin\Debug\net462\SampleAppVB.exe" />
    <Parameter name="TargetExeCS" value="..\..\..\..\..\src\SampleApp\bin\Debug\net462\SampleApp.exe" />

    <!-- キャプチャ(エビデンス)の保存先。ソリューション直下の Evidence フォルダ -->
    <Parameter name="EvidenceDir" value="..\..\..\..\..\Evidence" />

    <!--
      テストデータ初期化コマンド（任意）。テスト1件ごと、アプリ起動前に cmd.exe /c で実行する。
      終了コードが 0 以外ならテストは失敗になる。作業フォルダはテストの出力フォルダ。
      例: sqlcmd -S .\SQLEXPRESS -d CodeFinder -i ..\..\..\..\..\TestData\reset.sql -b
    -->
    <Parameter name="ResetCommand" value="" />
  </TestRunParameters>
</RunSettings>
`````

### 39. `tests/SampleApp.GuiTests/計算画面Tests.cs`

`````csharp
using GuiTestKit.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleApp.GuiTests
{
    /// <summary>
    /// 計算画面（メイン画面）のテスト。
    /// VB版・C#版のどちらに対して実行するかは runsettings の Target で切り替える（テストコードは共通）。
    /// </summary>
    [TestClass]
    [TestCategory("GUI")]
    public class 計算画面Tests : SampleAppTestBase
    {
        [TestMethod]
        public void 正常値を入力して計算すると合計が表示される()
        {
            var page = メイン画面;
            page.EnterBaseValues(new ScreenValues { { "txtA", "12" }, { "txtB", "30" } });
            Snap("入力後");

            page.計算();

            CheckEventually("結果", "結果: 42", () => page.結果);
            Snap("計算後");
        }

        // データ駆動：同じ手順を入力値・期待値だけ変えて回す
        [DataTestMethod]
        [DataRow("C01", "1", "2", "結果: 3")]
        [DataRow("C02", "-5", "5", "結果: 0")]
        [DataRow("C03", "999999", "1", "結果: 1000000")]
        public void 境界値パターンの計算(string caseId, string a, string b, string expected)
        {
            CaseId = caseId;
            var page = メイン画面;

            page.EnterBaseValues(new ScreenValues { { "txtA", a }, { "txtB", b } });
            page.計算();

            CheckEventually("結果", expected, () => page.結果);
            Snap("計算後");
        }

        [TestMethod]
        public void 数値以外を入力するとエラーダイアログが表示される()
        {
            var page = メイン画面;
            page.EnterBaseValues(new ScreenValues { { "txtA", "abc" }, { "txtB", "1" } });

            var dialog = page.計算_エラー表示();
            Snap("エラーダイアログ");

            Check("ダイアログのタイトル", "入力エラー", dialog.Title);
            Check("メッセージ", "数値を入力してください。", dialog.Message);

            dialog.ClickOk();
            Check("結果（エラー時は変わらないこと）", "結果: -", page.結果);
        }

        [TestMethod]
        public void クリアすると入力と結果が初期状態に戻る()
        {
            var page = メイン画面;
            page.Type("txtA", "7");   // こちらは実際に打鍵して入力する例
            page.Set("txtB", "8");
            page.計算();
            CheckEventually("結果", "結果: 15", () => page.結果);
            Snap("クリア前");

            page.クリア();

            CheckEventually("結果", "結果: -", () => page.結果);
            Check("数値A", "", page.Get("txtA"));
            Check("数値B", "", page.Get("txtB"));
            Snap("クリア後");
        }
    }
}
`````

### 40. `tests/SampleApp.GuiTests/顧客登録画面Tests.cs`

`````csharp
using GuiTestKit.Automation;
using SampleApp.GuiTests.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleApp.GuiTests
{
    /// <summary>
    /// 顧客登録画面のテスト。
    /// 入力は「基本値（全項目を正しく入力した状態）＋ そのケースで変える項目だけ」で書く。
    /// 画面までの遷移は [TestInitialize] で済ませ、各テストケースには書かない。
    /// </summary>
    [TestClass]
    [TestCategory("GUI")]
    public class 顧客登録画面Tests : SampleAppTestBase
    {
        private 顧客登録Page page;

        /// <summary>各テストの前に実行される（アプリの起動は基底クラスで済んでいる）。</summary>
        [TestInitialize]
        public void 顧客登録画面まで進む()
        {
            page = メイン画面.顧客登録画面へ();
        }

        [TestMethod]
        public void 基本値で登録すると登録完了メッセージが表示される()
        {
            Snap("初期表示");

            page.EnterBaseValues();
            Snap("入力後");

            var message = page.登録();
            Snap("登録完了メッセージ");
            Check("メッセージのタイトル", "登録完了", message.Title);
            Check("メッセージ", "顧客 000001 を登録しました。", message.Message);

            message.ClickOk();
            Check("状態表示", "登録済み: 000001", page.状態表示);
            Snap("登録後");
        }

        [DataTestMethod]
        [DataRow("C01", "txtCode", "12345", "顧客コードは6桁の数字で入力してください。")]
        [DataRow("C02", "txtCode", "12345A", "顧客コードは6桁の数字で入力してください。")]
        [DataRow("C03", "txtName", "", "顧客名を入力してください。")]
        [DataRow("C04", "txtName", "　", "顧客名を入力してください。")]
        [DataRow("C05", "txtCorpNo", "123456789012", "法人番号は13桁の数字で入力してください。")]
        [DataRow("C06", "txtCreditLimit", "100000000", "与信限度額は0～99999999の数値で入力してください。")]
        [DataRow("C07", "txtCreditLimit", "-1", "与信限度額は0～99999999の数値で入力してください。")]
        [DataRow("C08", "txtCreditLimit", "", "与信限度額は0～99999999の数値で入力してください。")]
        public void 入力エラーの場合はメッセージが表示され登録されない(string caseId, string id, string value, string expectedMessage)
        {
            CaseId = caseId;
            page.EnterBaseValues(new ScreenValues { { id, value } });
            Snap("入力後");

            var message = page.登録();
            Snap("エラーメッセージ");
            Check("メッセージのタイトル", "入力エラー", message.Title);
            Check("メッセージ", expectedMessage, message.Message);

            message.ClickOk();
            Check("状態表示（登録されないこと）", "", page.状態表示);
        }

        [DataTestMethod]
        [DataRow("C01", "0", "000000")]
        [DataRow("C02", "99999999", "999999")]
        public void 境界値で登録できる(string caseId, string creditLimit, string code)
        {
            CaseId = caseId;
            page.EnterBaseValues(new ScreenValues
            {
                { "txtCode", code },
                { "txtCreditLimit", creditLimit },
            });
            var message = page.登録();
            Snap("登録完了メッセージ");
            Check("メッセージ", "顧客 " + code + " を登録しました。", message.Message);
            message.ClickOk();
        }

        [TestMethod]
        public void 区分を個人にすると法人番号が入力不可になり法人番号なしで登録できる()
        {
            // 区分を先に変えると法人番号は無効・空になる。空のまま（基本値を上書き）にする
            page.EnterBaseValues(new ScreenValues
            {
                { "cmbKubun", "個人" },
                { "txtCorpNo", "" },
            });
            Snap("入力後");
            Check("法人番号が入力可能か", false, page.IsEnabled("txtCorpNo"));

            var message = page.登録();
            Check("メッセージ", "顧客 000001 を登録しました。", message.Message);
            message.ClickOk();
        }

        [TestMethod]
        public void 閉じるボタンで画面が閉じる()
        {
            Snap("閉じる前");

            page.閉じる();
            Snap("閉じた後");
            Check("計算画面に戻っていること", "結果: -", メイン画面.結果);
        }
    }
}
`````

### 41. `tools/GuiTestTool/DumpCommand.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using GuiTestKit.Automation;
using GuiTestKit.Evidence;

namespace GuiTestTool
{
    /// <summary>
    /// 起動中の画面から全項目を吸い出し、Page クラスのひな形を作る。
    /// 250画面分の「項目ID・項目名・基本値」を人が書かずに済ませるためのもの。
    /// </summary>
    internal static class DumpCommand
    {
        public static int Run(Options options)
        {
            var processArg = options.Get("process");
            if (processArg == null) throw new ToolException("--process でプロセス名か PID を指定してください。");

            var process = FindProcess(processArg);
            using (var automation = new UIA3Automation())
            using (var app = Application.Attach(process))
            {
                var window = SelectWindow(app, automation, options);
                if (window == null) return 2;

                Console.WriteLine("対象画面: {0}（画面ID: {1}）", window.Title, window.AutomationId);
                var controls = ControlAccess.ReadAll(window, CustomControls.Handlers, window.BoundingRectangle.Location);
                PrintTable(controls);

                var className = options.Get("class") ?? SafeIdentifier((string.IsNullOrEmpty(window.AutomationId) ? "画面" : window.AutomationId) + "Page");
                var ns = options.Get("namespace") ?? "Product.GuiTests.Pages";
                var outPath = options.Get("out") ?? className + ".cs";
                var code = Generate(className, ns, window, process.ProcessName, controls);
                File.WriteAllText(outPath, code, new UTF8Encoding(true));
                Console.WriteLine();
                Console.WriteLine("Page クラスのひな形: " + Path.GetFullPath(outPath));
            }
            return 0;
        }

        private static Process FindProcess(string arg)
        {
            int pid;
            if (int.TryParse(arg, out pid)) return Process.GetProcessById(pid);

            var name = arg.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? arg.Substring(0, arg.Length - 4) : arg;
            var processes = Process.GetProcessesByName(name);
            if (processes.Length == 0) throw new ToolException("プロセスが見つかりません: " + arg + "（対象アプリを起動してから実行してください）");
            if (processes.Length > 1) throw new ToolException("同じ名前のプロセスが複数あります。--process に PID を指定してください: "
                + string.Join(", ", processes.Select(p => p.Id)));
            return processes[0];
        }

        private static Window SelectWindow(Application app, UIA3Automation automation, Options options)
        {
            var delay = options.Get("delay");
            if (delay != null)
            {
                Console.WriteLine("{0} 秒以内に、吸い出したい画面をクリックして前面にしてください…", delay);
                Thread.Sleep(int.Parse(delay) * 1000);
                var hWnd = NativeForeground.Get();
                var fg = automation.FromHandle(hWnd);
                if (fg.Properties.ProcessId.ValueOrDefault != app.ProcessId) throw new ToolException("前面の画面が対象アプリのものではありません。");
                return fg.AsWindow();
            }

            var windows = AllWindows(app, automation);
            var key = options.Get("window");
            if (key != null)
            {
                var match = windows.FirstOrDefault(w => w.AutomationId == key || w.Title == key);
                if (match == null) throw new ToolException("画面が見つかりません: " + key + Environment.NewLine + ListWindows(windows));
                return match;
            }
            if (windows.Count == 1) return windows[0];

            Console.Error.WriteLine("画面が複数開いています。--window で画面ID（またはタイトル）を指定してください。");
            Console.Error.WriteLine(ListWindows(windows));
            return null;
        }

        /// <summary>トップレベルの画面と、そこから開いている子画面（モーダル画面など）すべて。</summary>
        private static List<Window> AllWindows(Application app, UIA3Automation automation)
        {
            var result = new List<Window>();
            var queue = new Queue<AutomationElement>(app.GetAllTopLevelWindows(automation));
            while (queue.Count > 0)
            {
                var w = queue.Dequeue();
                result.Add(w.AsWindow());
                foreach (var child in w.FindAllChildren(cf => cf.ByControlType(ControlType.Window))) queue.Enqueue(child);
            }
            return result;
        }

        private static string ListWindows(IEnumerable<Window> windows)
        {
            return string.Join(Environment.NewLine, windows.Select(w => "  画面ID: " + w.AutomationId + "　タイトル: " + w.Title));
        }

        private static void PrintTable(List<ControlSnapshot> controls)
        {
            Console.WriteLine();
            Console.WriteLine("{0,-24} {1,-12} {2,-20} {3}", "項目ID", "種類", "項目名", "値");
            foreach (var c in controls)
            {
                Console.WriteLine("{0,-24} {1,-12} {2,-20} {3}{4}", c.Id, c.Type, c.Label, OneLine(c.Value), c.Enabled ? "" : "（無効）");
            }
        }

        // ------------------------------------------------------------------
        // コード生成
        // ------------------------------------------------------------------

        private static string Generate(string className, string ns, Window window, string processName, List<ControlSnapshot> controls)
        {
            var inputs = controls.Where(c => ControlAccess.IsInputType(c.Type) || c.Type == "Custom")
                                 .Where(c => c.Type != "RadioButton" || c.Value == ScreenValues.ON)
                                 .ToList();
            var others = controls.Except(inputs).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FlaUI.Core.AutomationElements;");
            sb.AppendLine("using GuiTestKit.Automation;");
            sb.AppendLine();
            sb.AppendLine("namespace " + ns);
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// " + Escape(window.Title) + "");
            sb.AppendFormat("    /// GuiTestTool dump で自動生成（{0:yyyy/MM/dd HH:mm}、{1}）。基本値は吸い出した時点の画面の値。{2}", DateTime.Now, processName, Environment.NewLine);
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class " + className + " : ScreenPageBase");
            sb.AppendLine("    {");
            sb.AppendLine("        public const string WindowId = \"" + Escape(window.AutomationId) + "\";");
            sb.AppendLine();
            sb.AppendLine("        public " + className + "(Window window) : base(window)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>全項目を正しく入力した状態。テストケースではここからの差分だけを書く。</summary>");
            sb.AppendLine("        public override ScreenValues BaseValues { get; } = new ScreenValues");
            sb.AppendLine("        {");
            foreach (var c in inputs)
            {
                var comment = new List<string>();
                if (c.Type == "Custom") comment.Add("市販部品: 入力はセル単位で行う");
                if (!c.Enabled) comment.Add("吸い出し時は無効");
                var value = c.Type == "Custom" ? "null" : "\"" + Escape(c.Value) + "\"";
                sb.AppendFormat("            {{ \"{0}\", {1}, \"{2}\" }},{3}{4}",
                    Escape(c.Id), value, Escape(c.Label ?? ""), comment.Count > 0 ? "   // " + string.Join("、", comment) : "", Environment.NewLine);
            }
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>実行するたびに変わる項目（日時・採番など）。VB/C# の比較で値も画像も比較しない。</summary>");
            sb.AppendLine("        public override IEnumerable<string> CompareExcludedIds");
            sb.AppendLine("        {");
            sb.AppendLine("            get { return new string[] { }; }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- 表示項目・ボタン（参考）。必要な操作だけメソッドにする。例:");
            sb.AppendLine("        //   public MessageBoxPage 登録() { return ClickAndWaitMessageBox(\"btnRegister\"); }");
            foreach (var c in others)
            {
                sb.AppendFormat("        // {0,-20} {1,-10} {2}{3}", c.Id, c.Type, OneLine(c.Value), Environment.NewLine);
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string Escape(string text)
        {
            return (text ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static string OneLine(string text)
        {
            var line = (text ?? string.Empty).Replace("\r", "").Replace("\n", " ");
            return line.Length > 60 ? line.Substring(0, 60) + "…" : line;
        }

        private static string SafeIdentifier(string name)
        {
            var sb = new StringBuilder();
            foreach (var ch in name) sb.Append(char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_');
            if (sb.Length == 0 || char.IsDigit(sb[0])) sb.Insert(0, '_');
            return sb.ToString();
        }
    }

    internal static class NativeForeground
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr Get();
    }
}
`````

### 42. `tools/GuiTestTool/GuiTestTool.csproj`

`````xml
<Project Sdk="Microsoft.NET.Sdk">

  <!--
    GUIテスト用のコマンドラインツール。
      compare : VB断面とC#断面のエビデンスを比較して compare.html を出す
      report  : エビデンスから report.html を作り直す
      dump    : 起動中の画面から項目を吸い出し、Page クラスのひな形（基本値つき）を作る
  -->
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net462</TargetFramework>
    <RootNamespace>GuiTestTool</RootNamespace>
    <AssemblyName>GuiTestTool</AssemblyName>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\tests\GuiTestKit\GuiTestKit.csproj" />
  </ItemGroup>

</Project>
`````

### 43. `tools/GuiTestTool/Program.cs`

`````csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuiTestKit.Comparison;
using GuiTestKit.Reporting;

namespace GuiTestTool
{
    internal static class Program
    {
        private const string Usage = @"GuiTestTool — GUIテスト用ツール

使い方:
  GuiTestTool compare <比較元フォルダ> <比較先フォルダ> [--out <出力フォルダ>] [--tolerance <0-255>]
      2回分のエビデンスを突き合わせ、差分画像と compare.html を出す。
      例) GuiTestTool compare Evidence\20260926_100000\VB Evidence\20260926_110000\CS
      終了コード: 0 = すべて一致, 1 = 差異あり, 2 = エラー

  GuiTestTool report <実行フォルダ>
      エビデンス（record.json）から report.html を作り直す。
      例) GuiTestTool report Evidence\20260926_100000

  GuiTestTool dump --process <プロセス名|PID> [--window <画面ID|タイトル>] [--delay <秒>]
                   [--class <クラス名>] [--namespace <名前空間>] [--out <出力ファイル>]
      起動中の画面から全項目を吸い出し、基本値つきの Page クラスのひな形を作る。
      画面に基本値を手で入力してから実行すると、その値が基本値になる。
      --delay を付けると、その秒数のあいだに前面にした画面を対象にする。
      例) GuiTestTool dump --process SampleAppVB --window CustomerForm --class 顧客登録Page
";

        private static int Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            try
            {
                if (args.Length == 0) return ShowUsage();
                var options = ParseOptions(args.Skip(1).ToArray());
                switch (args[0].ToLowerInvariant())
                {
                    case "compare": return Compare(options);
                    case "report": return Report(options);
                    case "dump": return DumpCommand.Run(options);
                    default: return ShowUsage();
                }
            }
            catch (ToolException ex)
            {
                Console.Error.WriteLine("エラー: " + ex.Message);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("エラー: " + ex);
                return 2;
            }
        }

        private static int Compare(Options options)
        {
            if (options.Positional.Count != 2) throw new ToolException("比較元フォルダと比較先フォルダを指定してください。");
            var dirA = RequireDirectory(options.Positional[0]);
            var dirB = RequireDirectory(options.Positional[1]);

            // 既定の出力先: Evidence\Compare_日時（比較先フォルダの2つ上 = Evidence）
            var outDir = options.Get("out") ?? Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(Path.GetFullPath(dirB).TrimEnd(Path.DirectorySeparatorChar))),
                "Compare_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(outDir);

            var compareOptions = new CompareOptions { Tolerance = int.Parse(options.Get("tolerance") ?? "0") };
            var results = EvidenceComparer.Compare(dirA, dirB, outDir, compareOptions);
            var report = CompareReportWriter.Write(outDir, dirA, dirB, compareOptions, results);

            var different = results.Count(r => r.Status != CompareStatus.Same);
            Console.WriteLine("テスト {0} 件: 一致 {1} 件 / 差異あり・片方のみ {2} 件", results.Count, results.Count - different, different);
            Console.WriteLine("比較結果: " + report);
            return different == 0 ? 0 : 1;
        }

        private static int Report(Options options)
        {
            if (options.Positional.Count != 1) throw new ToolException("実行フォルダ（Evidence\\実行日時）を指定してください。");
            var path = RunReportWriter.Write(RequireDirectory(options.Positional[0]));
            Console.WriteLine("レポート: " + path);
            return 0;
        }

        private static string RequireDirectory(string path)
        {
            if (!Directory.Exists(path)) throw new ToolException("フォルダがありません: " + path);
            return path;
        }

        private static int ShowUsage()
        {
            Console.WriteLine(Usage);
            return 2;
        }

        private static Options ParseOptions(string[] args)
        {
            var options = new Options();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    if (i + 1 >= args.Length) throw new ToolException(args[i] + " の値がありません。");
                    options.Named[args[i].Substring(2).ToLowerInvariant()] = args[++i];
                }
                else
                {
                    options.Positional.Add(args[i]);
                }
            }
            return options;
        }
    }

    internal class Options
    {
        public readonly List<string> Positional = new List<string>();
        public readonly Dictionary<string, string> Named = new Dictionary<string, string>();

        public string Get(string name)
        {
            string value;
            return Named.TryGetValue(name, out value) ? value : null;
        }
    }

    internal class ToolException : Exception
    {
        public ToolException(string message) : base(message)
        {
        }
    }
}
`````
