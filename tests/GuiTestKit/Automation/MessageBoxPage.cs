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
