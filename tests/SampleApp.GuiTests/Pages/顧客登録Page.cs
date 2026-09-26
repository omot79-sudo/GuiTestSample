using System.Collections.Generic;
using FlaUI.Core.AutomationElements;
using GuiTestKit.Automation;

namespace SampleApp.GuiTests.Pages
{
    /// <summary>
    /// 顧客登録画面。
    /// 基本値は GuiTestTool dump で画面から吸い出したひな形をもとにしている。
    /// </summary>
    public class 顧客登録Page : ScreenPageBase
    {
        public const string WindowId = "CustomerForm";

        public 顧客登録Page(Window window) : base(window)
        {
        }

        /// <summary>全項目を正しく入力した状態。テストケースではここからの差分だけを書く。</summary>
        public override ScreenValues BaseValues { get; } = new ScreenValues
        {
            { "txtCode", "000001", "顧客コード" },
            { "txtName", "テスト商事株式会社", "顧客名" },
            { "txtKana", "テストショウジ", "顧客名カナ" },
            { "cmbKubun", "法人", "区分" },
            { "txtCorpNo", "1234567890123", "法人番号" },
            { "txtTel", "03-1234-5678", "電話番号" },
            { "txtCreditLimit", "1000000", "与信限度額" },
            { "chkActive", ScreenValues.ON, "取引中" },
            { "rdoPayTransfer", ScreenValues.ON, "支払方法/振込" },
        };

        /// <summary>表示日時は実行するたびに変わるので、VB/C# の比較対象から外す。</summary>
        public override IEnumerable<string> CompareExcludedIds
        {
            get { return new[] { "lblTimestamp" }; }
        }

        public string 状態表示 { get { return Get("lblStatus"); } }

        public MessageBoxPage 登録() { return ClickAndWaitMessageBox("btnRegister"); }

        public void 閉じる()
        {
            Click("btnClose");
            WaitUntilClosed();
        }
    }
}
