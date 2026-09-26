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

## テストコードのルール

- テスト名・画面クラス名・操作名は日本語。テスト名は「〜の場合は〜になる」のように、期待する結果が分かる形にする。
- テストクラスは画面ごとに1つ（`○○画面Tests`）。`[TestCategory("GUI")]` を付ける。
- 入力は `EnterBaseValues(差分)` を基本にする。差分には、そのケースで変える項目だけを書く。
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
