Imports System.Drawing
Imports System.Windows.Forms

Public Class NewGameDialog
    Inherits Form

    Private txtPlayer1 As TextBox
    Private txtPlayer2 As TextBox
    Private numTargetScore As NumericUpDown
    Private chkPlayer2Computer As CheckBox

    Private btnOk As Button
    Private btnCancel As Button

    Public Property Player1Name As String
    Public Property Player2Name As String
    Public Property TargetScore As Integer
    Public Property Player2IsComputer As Boolean


    Public Sub New(
        currentPlayer1Name As String,
        currentPlayer2Name As String,
        currentTargetScore As Integer,
        currentPlayer2IsComputer As Boolean)

        Me.Text = "Новая игра"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ClientSize = New Size(350, 225)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.FromArgb(0, 70, 0)
        Me.ForeColor = Color.White

        CreateControls(
            currentPlayer1Name,
            currentPlayer2Name,
            currentTargetScore,
            currentPlayer2IsComputer)

    End Sub


    Private Sub CreateControls(
        currentPlayer1Name As String,
        currentPlayer2Name As String,
        currentTargetScore As Integer,
        currentPlayer2IsComputer As Boolean)

        Dim lblTitle As New Label()
        lblTitle.Text = "Параметры новой игры"
        lblTitle.Font = New Font("Arial", 11, FontStyle.Bold)
        lblTitle.ForeColor = Color.White
        lblTitle.Location = New Point(20, 15)
        lblTitle.Size = New Size(300, 24)
        Me.Controls.Add(lblTitle)


        Dim lblPlayer1 As New Label()
        lblPlayer1.Text = "Игрок 1:"
        lblPlayer1.ForeColor = Color.White
        lblPlayer1.Location = New Point(20, 55)
        lblPlayer1.Size = New Size(90, 22)
        Me.Controls.Add(lblPlayer1)

        txtPlayer1 = New TextBox()
        txtPlayer1.Location = New Point(130, 52)
        txtPlayer1.Size = New Size(180, 22)
        txtPlayer1.Text = currentPlayer1Name
        Me.Controls.Add(txtPlayer1)


        Dim lblPlayer2 As New Label()
        lblPlayer2.Text = "Игрок 2:"
        lblPlayer2.ForeColor = Color.White
        lblPlayer2.Location = New Point(20, 85)
        lblPlayer2.Size = New Size(90, 22)
        Me.Controls.Add(lblPlayer2)

        txtPlayer2 = New TextBox()
        txtPlayer2.Location = New Point(130, 82)
        txtPlayer2.Size = New Size(180, 22)
        txtPlayer2.Text = currentPlayer2Name
        Me.Controls.Add(txtPlayer2)


        chkPlayer2Computer = New CheckBox()
        chkPlayer2Computer.Text = "Игрок 2 — компьютер"
        chkPlayer2Computer.ForeColor = Color.White
        chkPlayer2Computer.BackColor = Color.FromArgb(0, 70, 0)
        chkPlayer2Computer.Location = New Point(130, 110)
        chkPlayer2Computer.Size = New Size(180, 22)
        chkPlayer2Computer.Checked = currentPlayer2IsComputer
        Me.Controls.Add(chkPlayer2Computer)


        Dim lblTarget As New Label()
        lblTarget.Text = "Цель очков:"
        lblTarget.ForeColor = Color.White
        lblTarget.Location = New Point(20, 145)
        lblTarget.Size = New Size(90, 22)
        Me.Controls.Add(lblTarget)

        numTargetScore = New NumericUpDown()
        numTargetScore.Location = New Point(130, 142)
        numTargetScore.Size = New Size(90, 22)
        numTargetScore.Minimum = 20
        numTargetScore.Maximum = 1000
        numTargetScore.Increment = 10

        If currentTargetScore < 20 Then
            currentTargetScore = 150
        End If

        If currentTargetScore > 1000 Then
            currentTargetScore = 1000
        End If

        numTargetScore.Value = currentTargetScore
        Me.Controls.Add(numTargetScore)


        btnOk = New Button()
        btnOk.Text = "Начать"
        btnOk.Location = New Point(110, 185)
        btnOk.Size = New Size(100, 28)
        StyleButton(btnOk)
        AddHandler btnOk.Click, AddressOf btnOk_Click
        Me.Controls.Add(btnOk)


        btnCancel = New Button()
        btnCancel.Text = "Отмена"
        btnCancel.Location = New Point(220, 185)
        btnCancel.Size = New Size(100, 28)
        StyleButton(btnCancel)
        btnCancel.DialogResult = DialogResult.Cancel
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnOk
        Me.CancelButton = btnCancel

    End Sub


    Private Sub StyleButton(btn As Button)

        btn.BackColor = Color.FromArgb(0, 90, 0)
        btn.ForeColor = Color.White
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderColor = Color.White
        btn.Font = New Font("Arial", 9, FontStyle.Bold)

    End Sub


    Private Sub btnOk_Click(
        sender As Object,
        e As EventArgs)

        Dim p1 As String =
            txtPlayer1.Text.Trim()

        Dim p2 As String =
            txtPlayer2.Text.Trim()

        If p1 = "" Then

            MessageBox.Show(
                "Введите имя первого игрока.",
                "Новая игра",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            txtPlayer1.Focus()
            Exit Sub

        End If

        If p2 = "" Then

            MessageBox.Show(
                "Введите имя второго игрока.",
                "Новая игра",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            txtPlayer2.Focus()
            Exit Sub

        End If

        Player1Name = p1
        Player2Name = p2
        TargetScore = CInt(numTargetScore.Value)
        Player2IsComputer = chkPlayer2Computer.Checked

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class