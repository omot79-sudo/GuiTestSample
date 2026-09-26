using GuiTestKit.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleApp.GuiTests
{
    /// <summary>
    /// 顧客登録画面のテスト。
    /// 入力は「基本値（全項目を正しく入力した状態）＋ そのケースで変える項目だけ」で書く。
    /// </summary>
    [TestClass]
    [TestCategory("GUI")]
    public class 顧客登録画面Tests : SampleAppTestBase
    {
        [TestMethod]
        public void 基本値で登録すると登録完了メッセージが表示される()
        {
            var page = メイン画面.顧客登録を開く();
            Snap("初期表示");

            page.EnterBaseValues();
            Snap("入力後");

            var message = page.登録();
            Snap("登録完了メッセージ");
            Check("メッセージのタイトル", "登録完了", message.Title);
            Check("メッセージ", "顧客 000001 を登録しました。", message.Message);

            message.ClickOk();
            Check("状態表示", "登録済み: 000001", page.状態表示);
            Snap("登録後");
        }

        [DataTestMethod]
        [DataRow("C01", "txtCode", "12345", "顧客コードは6桁の数字で入力してください。")]
        [DataRow("C02", "txtCode", "12345A", "顧客コードは6桁の数字で入力してください。")]
        [DataRow("C03", "txtName", "", "顧客名を入力してください。")]
        [DataRow("C04", "txtName", "　", "顧客名を入力してください。")]
        [DataRow("C05", "txtCorpNo", "123456789012", "法人番号は13桁の数字で入力してください。")]
        [DataRow("C06", "txtCreditLimit", "100000000", "与信限度額は0～99999999の数値で入力してください。")]
        [DataRow("C07", "txtCreditLimit", "-1", "与信限度額は0～99999999の数値で入力してください。")]
        [DataRow("C08", "txtCreditLimit", "", "与信限度額は0～99999999の数値で入力してください。")]
        public void 入力エラーの場合はメッセージが表示され登録されない(string caseId, string id, string value, string expectedMessage)
        {
            CaseId = caseId;
            var page = メイン画面.顧客登録を開く();

            page.EnterBaseValues(new ScreenValues { { id, value } });
            Snap("入力後");

            var message = page.登録();
            Snap("エラーメッセージ");
            Check("メッセージのタイトル", "入力エラー", message.Title);
            Check("メッセージ", expectedMessage, message.Message);

            message.ClickOk();
            Check("状態表示（登録されないこと）", "", page.状態表示);
        }

        [DataTestMethod]
        [DataRow("C01", "0", "000000")]
        [DataRow("C02", "99999999", "999999")]
        public void 境界値で登録できる(string caseId, string creditLimit, string code)
        {
            CaseId = caseId;
            var page = メイン画面.顧客登録を開く();

            page.EnterBaseValues(new ScreenValues
            {
                { "txtCode", code },
                { "txtCreditLimit", creditLimit },
            });
            var message = page.登録();
            Snap("登録完了メッセージ");
            Check("メッセージ", "顧客 " + code + " を登録しました。", message.Message);
            message.ClickOk();
        }

        [TestMethod]
        public void 区分を個人にすると法人番号が入力不可になり法人番号なしで登録できる()
        {
            var page = メイン画面.顧客登録を開く();

            // 区分を先に変えると法人番号は無効・空になる。空のまま（基本値を上書き）にする
            page.EnterBaseValues(new ScreenValues
            {
                { "cmbKubun", "個人" },
                { "txtCorpNo", "" },
            });
            Snap("入力後");
            Check("法人番号が入力可能か", false, page.IsEnabled("txtCorpNo"));

            var message = page.登録();
            Check("メッセージ", "顧客 000001 を登録しました。", message.Message);
            message.ClickOk();
        }

        [TestMethod]
        public void 閉じるボタンで画面が閉じる()
        {
            var page = メイン画面.顧客登録を開く();
            Snap("閉じる前");

            page.閉じる();
            Snap("閉じた後");
            Check("計算画面に戻っていること", "結果: -", メイン画面.結果);
        }
    }
}
