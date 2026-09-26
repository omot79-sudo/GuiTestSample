Imports System
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' テスト対象の小さな画面（VB版）。C#版と同じ部品名(Name)にしてあるので、
''' 同じテストコード（Page Object）でそのまま操作できる。
''' </summary>
Public Class MainForm
    Inherits Form

    Private ReadOnly txtA As New TextBox With {.Name = "txtA", .Location = New Point(90, 20), .Width = 170}
    Private ReadOnly txtB As New TextBox With {.Name = "txtB", .Location = New Point(90, 55), .Width = 170}
    Private ReadOnly btnCalc As New Button With {.Name = "btnCalc", .Text = "計算", .Location = New Point(90, 95), .Width = 80}
    Private ReadOnly btnClear As New Button With {.Name = "btnClear", .Text = "クリア", .Location = New Point(180, 95), .Width = 80}
    Private ReadOnly lblResult As New Label With {.Name = "lblResult", .Text = "結果: -", .Location = New Point(20, 140), .AutoSize = True}

    Public Sub New()
        Me.Name = "MainForm"
        Me.Text = "計算サンプル（VB）"
        Me.ClientSize = New Size(290, 180)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen

        Dim lblA As New Label With {.Text = "数値A", .Location = New Point(20, 23), .AutoSize = True}
        Dim lblB As New Label With {.Text = "数値B", .Location = New Point(20, 58), .AutoSize = True}

        AddHandler btnCalc.Click, AddressOf BtnCalc_Click
        AddHandler btnClear.Click, AddressOf BtnClear_Click

        Me.Controls.AddRange(New Control() {lblA, txtA, lblB, txtB, btnCalc, btnClear, lblResult})
        Me.AcceptButton = btnCalc
    End Sub

    Private Sub BtnCalc_Click(sender As Object, e As EventArgs)
        Dim a As Long
        Dim b As Long
        If Not Long.TryParse(txtA.Text, a) OrElse Not Long.TryParse(txtB.Text, b) Then
            MessageBox.Show(Me, "数値を入力してください。", "入力エラー",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        lblResult.Text = "結果: " & (a + b).ToString()
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        txtA.Clear()
        txtB.Clear()
        lblResult.Text = "結果: -"
        txtA.Focus()
    End Sub

End Class
