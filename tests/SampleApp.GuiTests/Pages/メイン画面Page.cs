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
