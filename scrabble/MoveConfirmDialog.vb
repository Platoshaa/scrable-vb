Imports System.Drawing
Imports System.Windows.Forms

Public Class MoveConfirmDialog
    Inherits Form

    Private lblInfo As New Label()
    Private lstWords As New ListBox()
    Private btnConfirm As New Button()
    Private btnCancel As New Button()
    Private btnBan As New Button()

    Public Property ConfirmMove As Boolean = False
    Public Property BannedWord As String = ""

    Public Sub New(
        playerName As String,
        words As List(Of String),
        wordScores As List(Of Integer),
        baseScore As Integer,
        bonusScore As Integer,
        totalScore As Integer)

        Me.Text = "Подтверждение хода"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ClientSize = New Size(430, 310)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False

        lblInfo.Location = New Point(12, 12)
        lblInfo.Size = New Size(400, 100)

        lblInfo.Text =
            playerName &
            " составил слова." &
            Environment.NewLine &
            Environment.NewLine &
            "Очки за слова: " &
            baseScore.ToString()

        If bonusScore > 0 Then

            lblInfo.Text &=
                Environment.NewLine &
                "Бонус за все 7 букв: +" &
                bonusScore.ToString()

        End If

        lblInfo.Text &=
            Environment.NewLine &
            "Итого за ход: " &
            totalScore.ToString() &
            Environment.NewLine &
            Environment.NewLine &
            "Завершить ход?"

        lstWords.Location = New Point(12, 118)
        lstWords.Size = New Size(400, 110)

        If words IsNot Nothing Then

            For i = 0 To words.Count - 1

                Dim scoreText As String = ""

                If wordScores IsNot Nothing AndAlso
                   i < wordScores.Count Then

                    scoreText =
                        " — " &
                        wordScores(i).ToString() &
                        " очков"

                End If

                lstWords.Items.Add(
                    words(i) & scoreText)

            Next

        End If

        If lstWords.Items.Count > 0 Then
            lstWords.SelectedIndex = 0
        End If

        btnBan.Text = "Запретить слово"
        btnBan.Location = New Point(12, 250)
        btnBan.Size = New Size(145, 30)

        AddHandler btnBan.Click,
            AddressOf btnBan_Click

        btnConfirm.Text = "Засчитать"
        btnConfirm.Location = New Point(205, 250)
        btnConfirm.Size = New Size(95, 30)

        AddHandler btnConfirm.Click,
            AddressOf btnConfirm_Click

        btnCancel.Text = "Отмена"
        btnCancel.Location = New Point(315, 250)
        btnCancel.Size = New Size(95, 30)

        AddHandler btnCancel.Click,
            AddressOf btnCancel_Click

        Me.Controls.Add(lblInfo)
        Me.Controls.Add(lstWords)
        Me.Controls.Add(btnBan)
        Me.Controls.Add(btnConfirm)
        Me.Controls.Add(btnCancel)

    End Sub


    Private Sub btnConfirm_Click(
        sender As Object,
        e As EventArgs)

        ConfirmMove = True
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub


    Private Sub btnCancel_Click(
        sender As Object,
        e As EventArgs)

        ConfirmMove = False
        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub


    Private Sub btnBan_Click(
        sender As Object,
        e As EventArgs)

        If lstWords.SelectedItem Is Nothing Then

            MessageBox.Show(
                "Выберите слово, которое нужно запретить.",
                "Запрет слова",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        Dim itemText As String =
            lstWords.SelectedItem.ToString()

        Dim parts() As String =
            itemText.Split("—"c)

        BannedWord =
            parts(0).Trim().ToUpper()

        ConfirmMove = False
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class