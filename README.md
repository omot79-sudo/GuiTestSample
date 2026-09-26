# GuiTestSample

ビルド済みEXE（C# / VB.NET、.NET Framework 4.6.2）を外から操作し、キャプチャを取る
GUI自動テスト（MSTest + FlaUI）のサンプルです。

## 構成

```
GuiTestSample/
├─ GuiTestSample.sln
├─ run-gui-tests.ps1                  … ビルド→テスト→結果出力を一括実行
├─ src/SampleApp/                     … テスト対象 C#版（WinForms / .NET Framework 4.6.2）
├─ src/SampleAppVB/                   … テスト対象 VB版（WinForms / .NET Framework 4.6.2）
└─ tests/SampleApp.GuiTests/          … GUIテスト（.NET Framework 4.6.2。テスト対象はプロジェクト参照しない）
    ├─ gui.runsettings                … テスト対象EXEのパス・キャプチャ保存先
    ├─ AssemblyInfo.cs                … 並列実行の無効化
    ├─ Infrastructure/GuiTestBase.cs  … EXE起動/終了、キャプチャ保存（共通処理）
    ├─ Pages/MainWindowPage.cs        … Page Object（部品の探し方と操作）
    └─ CalculatorTests.cs             … テストケース本体（C#版・VB版の両方に同じケースを実行）
```

## 実行方法

### Visual Studio
1. `GuiTestSample.sln` を開いて「ソリューションのリビルド」
2. テストエクスプローラーで「すべて実行」
   （`gui.runsettings` はプロジェクト設定で自動的に読み込まれます）

テストエクスプローラーには `CSharp版_計算画面` と `VB版_計算画面` の2クラスが表示され、
同じテストケースがそれぞれのEXEに対して実行されます。

### コマンドライン
```powershell
.\run-gui-tests.ps1
```

実行中は画面が自動操作されるので、マウス・キーボードに触らないでください。

## 出力

- キャプチャ: `Evidence\<実行日時>\<テストクラス名>\<テスト名>[_<ケース番号>]\01_入力後.png` など
- テスト結果: `TestResults\gui-test-result.trx`（キャプチャも添付されます）
- 失敗したテストは「失敗時の画面」を自動で保存します

## テスト対象を差し替えるには

`gui.runsettings` の `TargetExeCS` / `TargetExeVB` を書き換えます（絶対パス可）。
Page Object（`MainWindowPage.cs`）の AutomationId を対象画面に合わせれば、実際の製品EXEもテストできます。

AutomationId は「Accessibility Insights for Windows」や Windows SDK 付属の `Inspect.exe`、
`FlaUInspect` で画面を覗いて確認します。WinForms ではコントロールの Name がそのまま AutomationId になります。

## 単体テストと分けて回す

```powershell
dotnet test --filter "TestCategory!=GUI"   # 単体テストだけ
dotnet test --filter "TestCategory=GUI"    # GUIテストだけ
```
"# GuiTestSample" 
