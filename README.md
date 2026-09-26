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
