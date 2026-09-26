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
