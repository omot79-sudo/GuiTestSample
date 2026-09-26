Imports System
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' テスト対象の登録画面（VB版）。C#版と同じ部品名(Name)・同じ動きにしてある。
''' </summary>
Public Class CustomerForm
    Inherits Form

    Private ReadOnly txtCode As New TextBox With {.Name = "txtCode", .Location = New Point(120, 20), .Width = 100}
    Private ReadOnly txtName As New TextBox With {.Name = "txtName", .Location = New Point(120, 50), .Width = 220}
    Private ReadOnly txtKana As New TextBox With {.Name = "txtKana", .Location = New Point(120, 80), .Width = 220}
    Private ReadOnly cmbKubun As New ComboBox With {.Name = "cmbKubun", .Location = New Point(120, 110), .Width = 100, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly txtCorpNo As New TextBox With {.Name = "txtCorpNo", .Location = New Point(120, 140), .Width = 150}
    Private ReadOnly txtTel As New TextBox With {.Name = "txtTel", .Location = New Point(120, 170), .Width = 150}
    Private ReadOnly txtCreditLimit As New TextBox With {.Name = "txtCreditLimit", .Location = New Point(120, 200), .Width = 150, .TextAlign = HorizontalAlignment.Right}
    Private ReadOnly chkActive As New CheckBox With {.Name = "chkActive", .Text = "取引中", .Location = New Point(120, 230), .AutoSize = True}
    Private ReadOnly grpPay As New GroupBox With {.Name = "grpPay", .Text = "支払方法", .Location = New Point(20, 260), .Size = New Size(320, 50)}
    Private ReadOnly rdoPayCash As New RadioButton With {.Name = "rdoPayCash", .Text = "現金", .Location = New Point(20, 20), .AutoSize = True}
    Private ReadOnly rdoPayTransfer As New RadioButton With {.Name = "rdoPayTransfer", .Text = "振込", .Location = New Point(120, 20), .AutoSize = True}
    Private ReadOnly lblStatus As New Label With {.Name = "lblStatus", .Text = "", .Location = New Point(20, 325), .AutoSize = True}
    Private ReadOnly lblTimestamp As New Label With {.Name = "lblTimestamp", .Location = New Point(20, 350), .AutoSize = True}
    Private ReadOnly btnRegister As New Button With {.Name = "btnRegister", .Text = "登録", .Location = New Point(180, 345), .Width = 75}
    Private ReadOnly btnClose As New Button With {.Name = "btnClose", .Text = "閉じる", .Location = New Point(265, 345), .Width = 75}

    Public Sub New()
        Me.Name = "CustomerForm"
        Me.Text = "顧客登録（VB）"
        Me.ClientSize = New Size(360, 385)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterParent

        cmbKubun.Items.AddRange(New Object() {"法人", "個人"})
        cmbKubun.SelectedIndex = 0
        chkActive.Checked = True
        rdoPayTransfer.Checked = True
        grpPay.Controls.AddRange(New Control() {rdoPayCash, rdoPayTransfer})

        ' 実行するたびに変わる値（テストでは比較対象外に指定する例）
        lblTimestamp.Text = "表示日時: " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

        AddHandler cmbKubun.SelectedIndexChanged, AddressOf CmbKubun_SelectedIndexChanged
        AddHandler btnRegister.Click, AddressOf BtnRegister_Click
        AddHandler btnClose.Click, Sub(s, e) Me.Close()

        Me.Controls.AddRange(New Control() {
            NewLabel("顧客コード", 23), txtCode,
            NewLabel("顧客名", 53), txtName,
            NewLabel("顧客名カナ", 83), txtKana,
            NewLabel("区分", 113), cmbKubun,
            NewLabel("法人番号", 143), txtCorpNo,
            NewLabel("電話番号", 173), txtTel,
            NewLabel("与信限度額", 203), txtCreditLimit,
            chkActive, grpPay, lblStatus, lblTimestamp, btnRegister, btnClose})
    End Sub

    Private Shared Function NewLabel(text As String, y As Integer) As Label
        Return New Label With {.Text = text, .Location = New Point(20, y), .AutoSize = True}
    End Function

    Private Sub CmbKubun_SelectedIndexChanged(sender As Object, e As EventArgs)
        ' 個人のときは法人番号を入力できない
        Dim isCorp As Boolean = CStr(cmbKubun.SelectedItem) = "法人"
        txtCorpNo.Enabled = isCorp
        If Not isCorp Then txtCorpNo.Clear()
    End Sub

    Private Sub BtnRegister_Click(sender As Object, e As EventArgs)
        Dim creditLimit As Long
        Dim errorMessage As String = Nothing
        Dim errorControl As Control = Nothing

        If Not IsDigits(txtCode.Text, 6) Then
            errorMessage = "顧客コードは6桁の数字で入力してください。"
            errorControl = txtCode
        ElseIf txtName.Text.Trim().Length = 0 Then
            errorMessage = "顧客名を入力してください。"
            errorControl = txtName
        ElseIf txtCorpNo.Enabled AndAlso Not IsDigits(txtCorpNo.Text, 13) Then
            errorMessage = "法人番号は13桁の数字で入力してください。"
            errorControl = txtCorpNo
        ElseIf Not Long.TryParse(txtCreditLimit.Text, creditLimit) OrElse creditLimit < 0 OrElse creditLimit > 99999999 Then
            errorMessage = "与信限度額は0～99999999の数値で入力してください。"
            errorControl = txtCreditLimit
        End If

        If errorMessage IsNot Nothing Then
            MessageBox.Show(Me, errorMessage, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            errorControl.Focus()
            Return
        End If

        MessageBox.Show(Me, "顧客 " & txtCode.Text & " を登録しました。", "登録完了",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
        lblStatus.Text = "登録済み: " & txtCode.Text
    End Sub

    Private Shared Function IsDigits(text As String, length As Integer) As Boolean
        If text.Length <> length Then Return False
        For Each c As Char In text
            If c < "0"c OrElse c > "9"c Then Return False
        Next
        Return True
    End Function

End Class
