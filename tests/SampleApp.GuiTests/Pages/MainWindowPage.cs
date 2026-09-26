using System;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleApp.GuiTests.Pages
{
    /// <summary>
    /// Page Object：メイン画面の「部品の探し方」と「操作」をここに集める。
    /// 画面が変わったときはこのクラスだけ直せば、テストケース側は直さずに済む。
    /// C#版・VB版とも部品名が同じなので、このクラス1つで両方を操作できる。
    /// </summary>
    public class MainWindowPage
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public MainWindowPage(Window window)
        {
            Window = window;
        }

        public Window Window { get; private set; }

        // ---- 部品（AutomationId で特定。WinForms では Name プロパティの値） ----
        public TextBox TxtA { get { return Find("txtA").AsTextBox(); } }
        public TextBox TxtB { get { return Find("txtB").AsTextBox(); } }
        public Button BtnCalc { get { return Find("btnCalc").AsButton(); } }
        public Button BtnClear { get { return Find("btnClear").AsButton(); } }
        public Label LblResult { get { return Find("lblResult").AsLabel(); } }

        // ---- 操作 ----

        /// <summary>値を直接設定する（IMEの状態に左右されない。通常はこちら）。</summary>
        public void InputA(string value) { TxtA.Text = value; }
        public void InputB(string value) { TxtB.Text = value; }

        /// <summary>実際にキーを打鍵して入力する（キー入力イベントを確認したいとき用）。</summary>
        public void TypeA(string value) { TxtA.Enter(value); }

        /// <summary>計算ボタン押下（UIAのInvokeで押す。マウスは動かない）。</summary>
        public void ClickCalc() { BtnCalc.Invoke(); }

        /// <summary>
        /// 計算ボタンをマウスでクリックする。
        /// ボタンからモーダルダイアログ(MessageBox)が開く場合、Invoke だと
        /// ダイアログが閉じるまで呼び出しが戻らないことがあるため、こちらを使う。
        /// </summary>
        public void ClickCalcByMouse()
        {
            Window.Focus();
            BtnCalc.Click();
        }

        public void ClickClear() { BtnClear.Invoke(); }

        public string ResultText { get { return LblResult.Text; } }

        /// <summary>結果欄が期待値になるまで待つ（固定Sleepを使わない）。</summary>
        public bool WaitForResult(string expected)
        {
            return Retry.WhileFalse(() => ResultText == expected, Timeout).Result;
        }

        /// <summary>モーダルダイアログ（MessageBox）が出るまで待って返す。</summary>
        public Window WaitForDialog()
        {
            var dialog = Retry.WhileNull(() => Window.ModalWindows.FirstOrDefault(), Timeout).Result;
            if (dialog == null) throw new AssertFailedException("ダイアログが表示されませんでした。");
            return dialog;
        }

        private AutomationElement Find(string automationId)
        {
            var element = Retry.WhileNull(
                () => Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
                Timeout).Result;
            if (element == null) throw new AssertFailedException("部品が見つかりません: " + automationId);
            return element;
        }
    }

    /// <summary>標準の MessageBox を扱うヘルパー（Win32標準ダイアログの固定ID）。</summary>
    public static class MessageBoxExtensions
    {
        public static string GetMessage(this Window dialog)
        {
            var text = dialog.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            return text == null ? string.Empty : text.Name;
        }

        public static void ClickOk(this Window dialog)
        {
            dialog.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton().Invoke();
        }
    }
}
