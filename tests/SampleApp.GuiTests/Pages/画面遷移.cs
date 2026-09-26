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
