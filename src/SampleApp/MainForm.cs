using System;
using System.Drawing;
using System.Windows.Forms;

namespace SampleApp
{
    /// <summary>
    /// テスト対象の小さな画面（C#版）。2つの数値を足し算して結果を表示する。
    /// ※ WinForms ではコントロールの Name プロパティが UI Automation の AutomationId になる。
    ///    テストはこの Name（txtA, btnCalc など）で部品を特定するので、変更しないこと。
    /// </summary>
    public class MainForm : Form
    {
        private readonly TextBox txtA = new TextBox { Name = "txtA", Location = new Point(90, 20), Width = 170 };
        private readonly TextBox txtB = new TextBox { Name = "txtB", Location = new Point(90, 55), Width = 170 };
        private readonly Button btnCalc = new Button { Name = "btnCalc", Text = "計算", Location = new Point(90, 95), Width = 80 };
        private readonly Button btnClear = new Button { Name = "btnClear", Text = "クリア", Location = new Point(180, 95), Width = 80 };
        private readonly Label lblResult = new Label { Name = "lblResult", Text = "結果: -", Location = new Point(20, 140), AutoSize = true };

        public MainForm()
        {
            Name = "MainForm";
            Text = "計算サンプル（C#）";
            ClientSize = new Size(290, 180);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var lblA = new Label { Text = "数値A", Location = new Point(20, 23), AutoSize = true };
            var lblB = new Label { Text = "数値B", Location = new Point(20, 58), AutoSize = true };

            btnCalc.Click += BtnCalc_Click;
            btnClear.Click += BtnClear_Click;

            Controls.AddRange(new Control[] { lblA, txtA, lblB, txtB, btnCalc, btnClear, lblResult });
            AcceptButton = btnCalc;
        }

        private void BtnCalc_Click(object sender, EventArgs e)
        {
            long a, b;
            if (!long.TryParse(txtA.Text, out a) || !long.TryParse(txtB.Text, out b))
            {
                MessageBox.Show(this, "数値を入力してください。", "入力エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblResult.Text = "結果: " + (a + b);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtA.Clear();
            txtB.Clear();
            lblResult.Text = "結果: -";
            txtA.Focus();
        }
    }
}
