using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp.GuiTests.Infrastructure;
using SampleApp.GuiTests.Pages;

namespace SampleApp.GuiTests
{
    // ------------------------------------------------------------------
    // 実行されるテストクラス：対象EXEごとに1クラス。
    // 中身のテストケースは下の CalculatorScenarios を継承して共通化している。
    // ------------------------------------------------------------------

    [TestClass]
    [TestCategory("GUI")]
    public class CSharp版_計算画面 : CalculatorScenarios
    {
        protected override string TargetExeParameter { get { return "TargetExeCS"; } }
    }

    [TestClass]
    [TestCategory("GUI")]
    public class VB版_計算画面 : CalculatorScenarios
    {
        protected override string TargetExeParameter { get { return "TargetExeVB"; } }
    }

    /// <summary>
    /// 計算画面のテストケース（C#版・VB版で共通）。
    /// テストケース＝メソッド。手順は Page Object の操作を並べるだけにして読みやすく保つ。
    /// </summary>
    public abstract class CalculatorScenarios : GuiTestBase
    {
        [TestMethod]
        public void 正常値を入力して計算すると合計が表示される()
        {
            Page.InputA("12");
            Page.InputB("30");
            Snap("入力後");

            Page.ClickCalc();

            Assert.IsTrue(Page.WaitForResult("結果: 42"), "実際の表示: " + Page.ResultText);
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

            Page.InputA(a);
            Page.InputB(b);
            Page.ClickCalc();

            Assert.IsTrue(Page.WaitForResult(expected), "[" + caseId + "] 実際の表示: " + Page.ResultText);
            Snap("計算後");
        }

        [TestMethod]
        public void 数値以外を入力するとエラーダイアログが表示される()
        {
            Page.InputA("abc");
            Page.InputB("1");

            // MessageBox が開くボタンはマウスクリックで押す（Invoke だと戻ってこない場合がある）
            Page.ClickCalcByMouse();

            var dialog = Page.WaitForDialog();
            Snap("エラーダイアログ", dialog);

            Assert.AreEqual("入力エラー", dialog.Title);
            StringAssert.Contains(dialog.GetMessage(), "数値を入力してください");

            dialog.ClickOk();
            Assert.AreEqual("結果: -", Page.ResultText, "エラー時は結果が変わらないこと");
        }

        [TestMethod]
        public void クリアすると入力と結果が初期状態に戻る()
        {
            Page.TypeA("7");   // こちらは実際に打鍵して入力する例
            Page.InputB("8");
            Page.ClickCalc();
            Assert.IsTrue(Page.WaitForResult("結果: 15"));
            Snap("クリア前");

            Page.ClickClear();

            Assert.IsTrue(Page.WaitForResult("結果: -"));
            Assert.AreEqual(string.Empty, Page.TxtA.Text);
            Assert.AreEqual(string.Empty, Page.TxtB.Text);
            Snap("クリア後");
        }
    }
}
