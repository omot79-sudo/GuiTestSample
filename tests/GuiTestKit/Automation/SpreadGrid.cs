using System;
using System.Threading;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

namespace GuiTestKit.Automation
{
    // ------------------------------------------------------------------
    // 【要検証】SPREAD for Windows Forms 用のひな形。
    // Spread は UI Automation でセルを読み書きできるかがバージョンによって異なるため、
    // UIA に頼らず「キー操作でセル移動・入力」「クリップボードで読み取り」で実装している。
    // 実機の Spread で動作を確認し、必要に応じて調整すること。
    // ------------------------------------------------------------------

    /// <summary>
    /// Spread を1項目として扱うハンドラ。全項目の吸い出し・VB/C#比較では、シート全体を TSV（タブ区切り）で記録する。
    /// 登録例: CustomControls.Handlers.Add(new SpreadHandler(id => id.StartsWith("spd")));
    /// </summary>
    public class SpreadHandler : ICustomControlHandler
    {
        private readonly Func<string, bool> _isSpreadId;

        /// <param name="isSpreadId">AutomationId で Spread かどうかを判定する（命名規約 "spd" 始まり等）。</param>
        public SpreadHandler(Func<string, bool> isSpreadId)
        {
            _isSpreadId = isSpreadId;
        }

        public bool CanHandle(AutomationElement element)
        {
            var id = element.Properties.AutomationId.ValueOrDefault;
            return !string.IsNullOrEmpty(id) && _isSpreadId(id);
        }

        public string Read(AutomationElement element)
        {
            return new SpreadGrid(element).ReadAllAsTsv();
        }

        public void Write(AutomationElement element, string value)
        {
            throw new NotSupportedException("Spread はセル単位で入力してください（SpreadGrid.SetCell）。");
        }
    }

    /// <summary>Spread のセル操作。Page クラスから new SpreadGrid(Find("spdMeisai")) のように使う。</summary>
    public class SpreadGrid
    {
        private readonly AutomationElement _element;

        public SpreadGrid(AutomationElement element)
        {
            _element = element;
        }

        /// <summary>セルに値を入力する（行・列は 0 始まり）。</summary>
        public void SetCell(int row, int column, string value)
        {
            MoveTo(row, column);
            Keyboard.Type(VirtualKeyShort.F2);          // 編集開始（設定によっては不要）
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            Keyboard.Type(VirtualKeyShort.DELETE);
            if (value.Length > 0) Keyboard.Type(value);
            Keyboard.Type(VirtualKeyShort.RETURN);      // 確定
            Wait.UntilInputIsProcessed();
        }

        /// <summary>セルの値を読む（クリップボード経由）。</summary>
        public string GetCell(int row, int column)
        {
            MoveTo(row, column);
            return CopyToText();
        }

        /// <summary>シート全体を TSV で読む（Ctrl+A → Ctrl+C）。</summary>
        public string ReadAllAsTsv()
        {
            _element.Focus();
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            return CopyToText();
        }

        private void MoveTo(int row, int column)
        {
            _element.Click();
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.HOME);
            for (var r = 0; r < row; r++) Keyboard.Type(VirtualKeyShort.DOWN);
            for (var c = 0; c < column; c++) Keyboard.Type(VirtualKeyShort.RIGHT);
            Wait.UntilInputIsProcessed();
        }

        private static string CopyToText()
        {
            RunSta(Clipboard.Clear);
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_C);
            Wait.UntilInputIsProcessed();

            string text = null;
            for (var i = 0; i < 20 && string.IsNullOrEmpty(text); i++)
            {
                Thread.Sleep(50);
                RunSta(() => text = Clipboard.GetText());
            }
            return (text ?? string.Empty).TrimEnd('\r', '\n');
        }

        /// <summary>クリップボードは STA スレッドからしか使えないため、専用スレッドで実行する。</summary>
        private static void RunSta(Action action)
        {
            Exception error = null;
            var thread = new Thread(() =>
            {
                try { action(); }
                catch (Exception ex) { error = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null) throw error;
        }
    }
}
