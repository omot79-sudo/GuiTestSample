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
