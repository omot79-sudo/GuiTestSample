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
