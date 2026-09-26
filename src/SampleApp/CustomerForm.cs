using System;
using System.Drawing;
using System.Windows.Forms;

namespace SampleApp
{
    /// <summary>
    /// テスト対象の登録画面（C#版）。入力項目が多い業務画面の縮小版。
    /// テキストボックス・コンボボックス・チェックボックス・ラジオボタン・表示日時（比較対象外にする項目）を含む。
    /// ※ Name がそのまま AutomationId になるので、VB版と同じ Name にしておくこと。
    /// </summary>
    public class CustomerForm : Form
    {
        private readonly TextBox txtCode = new TextBox { Name = "txtCode", Location = new Point(120, 20), Width = 100 };
        private readonly TextBox txtName = new TextBox { Name = "txtName", Location = new Point(120, 50), Width = 220 };
        private readonly TextBox txtKana = new TextBox { Name = "txtKana", Location = new Point(120, 80), Width = 220 };
        private readonly ComboBox cmbKubun = new ComboBox { Name = "cmbKubun", Location = new Point(120, 110), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtCorpNo = new TextBox { Name = "txtCorpNo", Location = new Point(120, 140), Width = 150 };
        private readonly TextBox txtTel = new TextBox { Name = "txtTel", Location = new Point(120, 170), Width = 150 };
        private readonly TextBox txtCreditLimit = new TextBox { Name = "txtCreditLimit", Location = new Point(120, 200), Width = 150, TextAlign = HorizontalAlignment.Right };
        private readonly CheckBox chkActive = new CheckBox { Name = "chkActive", Text = "取引中", Location = new Point(120, 230), AutoSize = true };
        private readonly GroupBox grpPay = new GroupBox { Name = "grpPay", Text = "支払方法", Location = new Point(20, 260), Size = new Size(320, 50) };
        private readonly RadioButton rdoPayCash = new RadioButton { Name = "rdoPayCash", Text = "現金", Location = new Point(20, 20), AutoSize = true };
        private readonly RadioButton rdoPayTransfer = new RadioButton { Name = "rdoPayTransfer", Text = "振込", Location = new Point(120, 20), AutoSize = true };
        private readonly Label lblStatus = new Label { Name = "lblStatus", Text = "", Location = new Point(20, 325), AutoSize = true };
        private readonly Label lblTimestamp = new Label { Name = "lblTimestamp", Location = new Point(20, 350), AutoSize = true };
        private readonly Button btnRegister = new Button { Name = "btnRegister", Text = "登録", Location = new Point(180, 345), Width = 75 };
        private readonly Button btnClose = new Button { Name = "btnClose", Text = "閉じる", Location = new Point(265, 345), Width = 75 };

        public CustomerForm()
        {
            Name = "CustomerForm";
            Text = "顧客登録（C#）";
            ClientSize = new Size(360, 385);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            cmbKubun.Items.AddRange(new object[] { "法人", "個人" });
            cmbKubun.SelectedIndex = 0;
            chkActive.Checked = true;
            rdoPayTransfer.Checked = true;
            grpPay.Controls.AddRange(new Control[] { rdoPayCash, rdoPayTransfer });

            // 実行するたびに変わる値（テストでは比較対象外に指定する例）
            lblTimestamp.Text = "表示日時: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            cmbKubun.SelectedIndexChanged += CmbKubun_SelectedIndexChanged;
            btnRegister.Click += BtnRegister_Click;
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                NewLabel("顧客コード", 23), txtCode,
                NewLabel("顧客名", 53), txtName,
                NewLabel("顧客名カナ", 83), txtKana,
                NewLabel("区分", 113), cmbKubun,
                NewLabel("法人番号", 143), txtCorpNo,
                NewLabel("電話番号", 173), txtTel,
                NewLabel("与信限度額", 203), txtCreditLimit,
                chkActive, grpPay, lblStatus, lblTimestamp, btnRegister, btnClose,
            });
        }

        private static Label NewLabel(string text, int y)
        {
            return new Label { Text = text, Location = new Point(20, y), AutoSize = true };
        }

        private void CmbKubun_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 個人のときは法人番号を入力できない
            var isCorp = (string)cmbKubun.SelectedItem == "法人";
            txtCorpNo.Enabled = isCorp;
            if (!isCorp) txtCorpNo.Clear();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            long creditLimit;
            string error = null;
            Control errorControl = null;

            if (!IsDigits(txtCode.Text, 6))
            {
                error = "顧客コードは6桁の数字で入力してください。";
                errorControl = txtCode;
            }
            else if (txtName.Text.Trim().Length == 0)
            {
                error = "顧客名を入力してください。";
                errorControl = txtName;
            }
            else if (txtCorpNo.Enabled && !IsDigits(txtCorpNo.Text, 13))
            {
                error = "法人番号は13桁の数字で入力してください。";
                errorControl = txtCorpNo;
            }
            else if (!long.TryParse(txtCreditLimit.Text, out creditLimit) || creditLimit < 0 || creditLimit > 99999999)
            {
                error = "与信限度額は0～99999999の数値で入力してください。";
                errorControl = txtCreditLimit;
            }

            if (error != null)
            {
                MessageBox.Show(this, error, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                errorControl.Focus();
                return;
            }

            MessageBox.Show(this, "顧客 " + txtCode.Text + " を登録しました。", "登録完了",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblStatus.Text = "登録済み: " + txtCode.Text;
        }

        private static bool IsDigits(string text, int length)
        {
            if (text.Length != length) return false;
            foreach (var c in text)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }
    }
}
