Imports System.Drawing
Imports System.Linq

Public Class Form1

    Private gameBoard As Board
    Private bag As TileBag
    Private players As New List(Of Player)

    Private currentPlayerIndex As Integer = 0
    Private currentPlayer As Player


    Private Const PlayerRackStartX As Integer = 545
    Private Const Player1RackY As Integer = 120
    Private Const Player2RackY As Integer = 260
    Private player1Name As String = "Мама"
    Private player2Name As String = "Папа"
    Private Const CellSize As Integer = 60
    Private Const StartX As Integer = 20
    Private Const StartY As Integer = 20

    Private Const RightPanelX As Integer = 940
    Private Const RightPanelWidth As Integer = 300

    Private Const ActiveRackStartX As Integer = 980
    Private Const ActiveRackStartY As Integer = 230
    Private Const ActiveRackStepY As Integer = 70

    Private Const OpponentRackStartX As Integer = 1120
    Private Const OpponentRackStartY As Integer = 230
    Private Const OpponentRackStepY As Integer = 70

    Private dragTile As TileInstance = Nothing
    Private dragOffsetX As Integer
    Private dragOffsetY As Integer
    Private ReadOnly RackLetterColor As Color =
    Color.FromArgb(235, 120, 210)

    Private ReadOnly RackLetterShadowColor As Color =
    Color.FromArgb(0, 45, 0)
    Private tileInstances As New List(Of TileInstance)
    Private Const AllTilesBonus As Integer = 15
    Private targetScore As Integer = 400
    Private gameOver As Boolean = False
    Private ReadOnly FormBackColor As Color =
        Color.FromArgb(0, 55, 0)

    Private ReadOnly BoardColor As Color =
        Color.FromArgb(55, 145, 55)

    Private ReadOnly BoardAltColor As Color =
        Color.FromArgb(35, 120, 45)

    Private ReadOnly BonusBlueColor As Color =
        Color.FromArgb(30, 60, 180)

    Private ReadOnly BonusPinkColor As Color =
        Color.FromArgb(255, 40, 150)

    Private ReadOnly BonusYellowColor As Color =
        Color.FromArgb(210, 170, 20)
    Private ReadOnly BonusGreenColor As Color =
    Color.FromArgb(0, 190, 0)

    Private ReadOnly CenterColor As Color =
        Color.FromArgb(255, 220, 40)

    Private ReadOnly TileColor As Color =
        Color.FromArgb(245, 238, 200)

    Private rackBalanceMode As RackBalanceMode =
    RackBalanceMode.Comfort

    Private btnRackMode As New Button()
    Private btnNewGame As New Button()
    Private currentGameBestWord As String = ""
    Private currentGameBestWordScore As Integer = 0
    Private currentGameBestWordPlayer As String = ""

    Private currentGameBestMoveScore As Integer = 0
    Private currentGameBestMovePlayer As String = ""

    Private btnStats As New Button()
    Private Sub Form1_Load(sender As Object,
                           e As EventArgs) _
                           Handles MyBase.Load

        Me.DoubleBuffered = True
        Me.BackColor = FormBackColor

        Dim workArea As Rectangle =
    Screen.PrimaryScreen.WorkingArea

        Me.StartPosition = FormStartPosition.Manual

        Me.Bounds = New Rectangle(
    workArea.Left,
    workArea.Top,
    Math.Min(1260, workArea.Width),
    Math.Min(1040, workArea.Height))

        Me.MinimumSize = New Size(1260, 1000)

        Me.DoubleBuffered = True
        Me.AutoScroll = False
       Me.Controls.Add(btnRackMode)
Me.Controls.Add(btnNewGame)
Me.Controls.Add(btnStats)
        AddHandler btnRackMode.Click,
    AddressOf btnRackMode_Click

        AddHandler btnStats.Click,
    AddressOf btnStats_Click

        AddHandler btnNewGame.Click,
    AddressOf btnNewGame_Click
        SetupButtons()

        Me.DoubleBuffered = True

        btnConfirmMove.Text = "Готово"
        btnCancelMove.Text = "Отмена"

        btnConfirmMove.Size = New Size(110, 34)
        btnCancelMove.Size = New Size(110, 34)

        btnConfirmMove.BackColor = Color.FromArgb(0, 90, 0)
        btnConfirmMove.ForeColor = Color.White
        btnConfirmMove.Font = New Font("Arial", 9, FontStyle.Bold)

        btnCancelMove.BackColor = Color.FromArgb(0, 90, 0)
        btnCancelMove.ForeColor = Color.White
        btnCancelMove.Font = New Font("Arial", 9, FontStyle.Bold)
        Me.Text = "Эрудит"
        Me.Width = 820
        Me.Height = 700

        DictionaryManager.LoadDictionary("russian.txt")
        StartNewGame()

    End Sub


    Private Sub Form1_Paint(sender As Object,
                            e As PaintEventArgs) _
                            Handles Me.Paint

        Dim g As Graphics = e.Graphics

        ' Поле
        For x = 0 To 14

            For y = 0 To 14

                Dim px As Integer =
                    StartX + x * CellSize

                Dim py As Integer =
                    StartY + y * CellSize

                Dim cellColor As Color =
                    GetBoardCellColor(x, y)

                Using cellBrush As New SolidBrush(cellColor)

                    g.FillRectangle(
                        cellBrush,
                        px,
                        py,
                        CellSize,
                        CellSize)

                End Using

                g.DrawRectangle(
                    Pens.Black,
                    px,
                    py,
                    CellSize,
                    CellSize)



            Next

        Next
        DrawRightPanel(g)

        ' Все фишки: и на поле, и на подставке
        For Each t As TileInstance In tileInstances

            If t.Confirmed AndAlso
       t.BoardX <> -1 AndAlso
       t.BoardY <> -1 Then

                DrawConfirmedBoardLetter(g, t)

            Else

                DrawRackTile(g, t)

            End If

        Next


    End Sub


    Private Sub Form1_MouseDown(
        sender As Object,
        e As MouseEventArgs) _
        Handles Me.MouseDown

        For i = tileInstances.Count - 1 To 0 Step -1

            Dim t As TileInstance = tileInstances(i)

            ' Уже подтвержденные буквы на поле двигать нельзя
            If t.Confirmed Then
                Continue For
            End If

            Dim r As New Rectangle(
                t.ScreenX,
                t.ScreenY,
                32,
                32)

            If r.Contains(e.Location) Then

                dragTile = t

                dragOffsetX =
                    e.X - t.ScreenX

                dragOffsetY =
                    e.Y - t.ScreenY

                Exit For

            End If

        Next

    End Sub


    Private Sub Form1_MouseMove(
        sender As Object,
        e As MouseEventArgs) _
        Handles Me.MouseMove

        If dragTile Is Nothing Then Exit Sub

        dragTile.ScreenX =
            e.X - dragOffsetX

        dragTile.ScreenY =
            e.Y - dragOffsetY

        Me.Invalidate()

    End Sub


    Private Sub Form1_MouseUp(
    sender As Object,
    e As MouseEventArgs) _
    Handles MyBase.MouseUp

        If dragTile Is Nothing Then
            Exit Sub
        End If

        Dim t As TileInstance =
        dragTile

        dragTile = Nothing

        Dim centerX As Integer =
        t.ScreenX + CellSize \ 2

        Dim centerY As Integer =
        t.ScreenY + CellSize \ 2

        Dim boardPixelWidth As Integer =
        Board.Size * CellSize

        Dim insideBoard As Boolean =
        centerX >= StartX AndAlso
        centerY >= StartY AndAlso
        centerX < StartX + boardPixelWidth AndAlso
        centerY < StartY + boardPixelWidth

        If Not insideBoard Then

            ReturnTileToHome(t)
            Me.Invalidate()
            Exit Sub

        End If

        Dim boardX As Integer =
        (centerX - StartX) \ CellSize

        Dim boardY As Integer =
        (centerY - StartY) \ CellSize

        Dim occupied As Boolean = False

        For Each other As TileInstance In tileInstances

            If other IsNot t AndAlso
           other.BoardX = boardX AndAlso
           other.BoardY = boardY Then

                occupied = True
                Exit For

            End If

        Next

        If occupied Then

            ReturnTileToHome(t)
            Me.Invalidate()
            Exit Sub

        End If

        t.BoardX = boardX
        t.BoardY = boardY

        t.ScreenX =
        StartX + boardX * CellSize

        t.ScreenY =
        StartY + boardY * CellSize

        If Not EnsureJokerLetterSelected(t) Then

            ReturnTileToHome(t)
            Me.Invalidate()
            Exit Sub

        End If

        Me.Invalidate()

    End Sub
    Private Sub btnCancelMove_Click(
    sender As Object,
    e As EventArgs) _
    Handles btnCancelMove.Click

        For Each t As TileInstance In tileInstances

            If Not t.Confirmed Then

                t.ScreenX = t.HomeX
                t.ScreenY = t.HomeY
                t.BoardX = -1
                t.BoardY = -1
                t.JokerLetter = ""

            End If

        Next

        Me.Invalidate()

    End Sub


    Private Sub Button1_Click(
    sender As Object,
    e As EventArgs) _
    Handles btnConfirmMove.Click
        If gameOver Then
            Exit Sub
        End If
        Dim placedTiles =
    GetCurrentMoveTiles()

        For Each t As TileInstance In placedTiles

            If t.IsJoker AndAlso t.JokerLetter = "" Then

                If Not EnsureJokerLetterSelected(t) Then
                    Exit Sub
                End If

            End If

        Next

        If placedTiles.Count = 0 Then

            OfferExchangeAllLetters()

            Exit Sub

        End If


        If Not MoveValidator.ValidateMove(
        tileInstances) Then

            MessageBox.Show(
            "Ход составлен неправильно")

            Exit Sub

        End If


        Dim wordTileLists =
        WordBuilder.BuildWordTiles(tileInstances)

        If wordTileLists.Count = 0 Then

            MessageBox.Show(
            "Ход не образует слов")

            Exit Sub

        End If


        Dim words As New List(Of String)
        Dim wordScores As New List(Of Integer)

        For Each wordTiles As List(Of TileInstance) In wordTileLists

            Dim word As String =
            GetWordText(wordTiles)

            If Not DictionaryManager.IsValidWord(word) Then

                MessageBox.Show(
                "Слова нет в словаре: " & word)

                Exit Sub

            End If

            Dim wordScore As Integer =
            ScoreManager.CalculateWordScore(
                wordTiles,
                gameBoard)

            words.Add(word)
            wordScores.Add(wordScore)

        Next


        Dim baseScore As Integer = 0

        For Each s As Integer In wordScores
            baseScore += s
        Next


        Dim bonusScore As Integer = 0

        If placedTiles.Count = 7 Then
            bonusScore = AllTilesBonus
        End If


        Dim totalScore As Integer =
        baseScore + bonusScore


        Dim confirmMessage As String =
        "Составленные слова:" &
        Environment.NewLine &
        Environment.NewLine

        For i = 0 To words.Count - 1

            confirmMessage &= words(i) &
            " — " &
            wordScores(i).ToString() &
            " очков" &
            Environment.NewLine

        Next

        confirmMessage &= Environment.NewLine &
        "Очки за слова: " &
        baseScore.ToString()

        If bonusScore > 0 Then

            confirmMessage &= Environment.NewLine &
            "Бонус за все 7 букв: +" &
            bonusScore.ToString()

        End If

        confirmMessage &= Environment.NewLine &
        "Итого за ход: " &
        totalScore.ToString() &
        Environment.NewLine &
        Environment.NewLine &
        "Завершить ход?"


        Dim answer =
        MessageBox.Show(
            confirmMessage,
            "Подтверждение хода",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If answer <> DialogResult.Yes Then

            ' Ход не засчитываем.
            ' Буквы остаются на поле, игрок может подумать дальше.
            Exit Sub

        End If


        For Each t As TileInstance In placedTiles

            gameBoard.GetCell(
            t.BoardX,
            t.BoardY).Tile = t.Tile

            t.Confirmed = True

            t.HomeX = t.ScreenX
            t.HomeY = t.ScreenY

            currentPlayer.Rack.Remove(t.Tile)

        Next


        RefillCurrentPlayerRack()
        UpdateCurrentGameRecords(
    words,
    wordScores,
    totalScore)

        currentPlayer.Score += totalScore

        UpdatePlayersInfo()



        If CheckGameOver() Then
            Exit Sub
        End If

        NextPlayer()

    End Sub
    Private Sub NextPlayer()

        Dim boardTiles As New List(Of TileInstance)


        For Each t As TileInstance In tileInstances

            If t.Confirmed AndAlso
           t.BoardX <> -1 AndAlso
           t.BoardY <> -1 Then

                boardTiles.Add(
    New TileInstance With {
        .Tile = t.Tile,
        .ScreenX = t.ScreenX,
        .ScreenY = t.ScreenY,
        .HomeX = t.ScreenX,
        .HomeY = t.ScreenY,
        .BoardX = t.BoardX,
        .BoardY = t.BoardY,
        .Confirmed = True,
        .JokerLetter = t.JokerLetter
    })

            End If

        Next


        currentPlayerIndex += 1

        If currentPlayerIndex >= players.Count Then
            currentPlayerIndex = 0
        End If

        currentPlayer =
        players(currentPlayerIndex)


        tileInstances.Clear()


        For Each t As TileInstance In boardTiles
            tileInstances.Add(t)
        Next


        LoadCurrentPlayerRack()

        UpdatePlayersInfo()

        Me.Text =
        "Эрудит - " &
        currentPlayer.Name &
        " | Очки: " &
        currentPlayer.Score.ToString()

        Me.Invalidate()

    End Sub
    Private Sub LoadCurrentPlayerRack()

        Dim rackX As Integer = ActiveRackStartX
        Dim rackY As Integer = ActiveRackStartY

        For Each tile As Tile In currentPlayer.Rack

            tileInstances.Add(
            New TileInstance With {
                .Tile = tile,
                .ScreenX = rackX,
                .ScreenY = rackY,
                .HomeX = rackX,
                .HomeY = rackY,
                .BoardX = -1,
                .BoardY = -1,
                .Confirmed = False
            })

            rackY += ActiveRackStepY

        Next

    End Sub

    Private Sub UpdatePlayersInfo()

        Dim text As String = ""

        For Each p As Player In players

            If p Is currentPlayer Then
                text &= "> "
            Else
                text &= "  "
            End If

            text &= p.Name &
                ": " &
                p.Score &
                " очков"

            text &= Environment.NewLine

        Next



    End Sub


    Private Function GetBoardCellColor(
    x As Integer,
    y As Integer
) As Color

        Dim bonus =
        gameBoard.GetCell(x, y).Bonus

        Select Case bonus

            Case BonusType.TripleWord
                Return BonusPinkColor

            Case BonusType.TripleLetter
                Return BonusBlueColor

            Case BonusType.DoubleWord
                Return BonusYellowColor

            Case BonusType.DoubleLetter
                Return BonusGreenColor

            Case BonusType.Center
                Return BoardColor

        End Select

        If (x + y) Mod 2 = 0 Then
            Return BoardColor
        End If

        Return BoardAltColor

    End Function
    Private Function IsCell(
    x As Integer,
    y As Integer,
    ParamArray cells() As Point
) As Boolean

        For Each p As Point In cells

            If p.X = x AndAlso p.Y = y Then
                Return True
            End If

        Next

        Return False

    End Function
    Private Function GetCurrentMoveTiles() As List(Of TileInstance)

        Dim result As New List(Of TileInstance)

        For Each t As TileInstance In tileInstances

            If Not t.Confirmed AndAlso
               t.BoardX <> -1 AndAlso
               t.BoardY <> -1 Then

                result.Add(t)

            End If

        Next

        Return result

    End Function


    Private Function BoardHasConfirmedTiles() As Boolean

        For Each t As TileInstance In tileInstances

            If t.Confirmed AndAlso
               t.BoardX <> -1 AndAlso
               t.BoardY <> -1 Then

                Return True

            End If

        Next

        Return False

    End Function


    Private Sub OfferExchangeAllLetters()

        Dim answer =
            MessageBox.Show(
                "Вы не составили слово." &
                Environment.NewLine &
                "Поменять все буквы и пропустить ход?",
                "Обмен букв",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

        If answer <> DialogResult.Yes Then
            Exit Sub
        End If

        ExchangeAllCurrentPlayerLetters()

        MessageBox.Show(
            "Буквы заменены. Ход переходит следующему игроку.")

        NextPlayer()

    End Sub


    Private Sub ExchangeAllCurrentPlayerLetters()

        Dim oldTiles As New List(Of Tile)

        For Each tile As Tile In currentPlayer.Rack
            oldTiles.Add(tile)
        Next


        If bag.Count < oldTiles.Count Then

            MessageBox.Show(
                "В мешке недостаточно букв для обмена.")

            Exit Sub

        End If


        currentPlayer.Rack.Clear()


        For i = 1 To oldTiles.Count

            Dim newTile As Tile =
                bag.Draw()

            If newTile IsNot Nothing Then
                currentPlayer.Rack.Add(newTile)
            End If

        Next


        For Each tile As Tile In oldTiles
            bag.ReturnTile(tile)
        Next

    End Sub


    Private Sub RefillCurrentPlayerRack()

        While currentPlayer.Rack.Count < 7

            Dim newTile As Tile =
                bag.Draw()

            If newTile Is Nothing Then
                Exit While
            End If

            currentPlayer.Rack.Add(newTile)

        End While
        EnsureBalancedRack(currentPlayer)
    End Sub
    Private Sub CreateStartTile()

        Dim startLetters As String =
        "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"

        Dim startTile As Tile =
        bag.DrawFromLetters(startLetters)

        If startTile Is Nothing Then
            Exit Sub
        End If

        Dim centerX As Integer = 7
        Dim centerY As Integer = 7

        Dim screenX As Integer =
        StartX + centerX * CellSize

        Dim screenY As Integer =
        StartY + centerY * CellSize

        gameBoard.GetCell(
        centerX,
        centerY).Tile = startTile

        tileInstances.Add(
        New TileInstance With {
            .Tile = startTile,
            .ScreenX = screenX,
            .ScreenY = screenY,
            .HomeX = screenX,
            .HomeY = screenY,
            .BoardX = centerX,
            .BoardY = centerY,
            .Confirmed = True,
            .JokerLetter = ""
        })

    End Sub
    Private Function GetWordText(
    wordTiles As List(Of TileInstance)
) As String

        Dim word As String = ""

        For Each t As TileInstance In wordTiles
            word &= t.VisibleLetter
        Next

        Return word

    End Function
    Private Sub DrawRightPanel(g As Graphics)

        Using panelBrush As New SolidBrush(FormBackColor)
            g.FillRectangle(
            panelBrush,
            RightPanelX,
            0,
            Me.ClientSize.Width - RightPanelX,
            Me.ClientSize.Height)
        End Using

        Using textBrush As New SolidBrush(Color.White)

            Dim titleFont As New Font(
            "Arial",
            11,
            FontStyle.Bold)

            Dim normalFont As New Font(
            "Arial",
            9,
            FontStyle.Regular)

            g.DrawString(
            "Ходит:",
            normalFont,
            textBrush,
            RightPanelX + 20,
            20)

            g.DrawString(
            currentPlayer.Name,
            titleFont,
            textBrush,
            RightPanelX + 20,
            42)

            g.DrawString(
            players(0).Name & ": " & players(0).Score.ToString(),
            normalFont,
            textBrush,
            RightPanelX + 20,
            80)

            g.DrawString(
            players(1).Name & ": " & players(1).Score.ToString(),
            normalFont,
            textBrush,
            RightPanelX + 20,
            102)

            g.DrawString(
            "В мешке: " & bag.Count.ToString(),
            normalFont,
            textBrush,
            RightPanelX + 20,
            130)
            g.DrawString(
    "Цель: " & targetScore.ToString(),
    normalFont,
    textBrush,
    RightPanelX + 20,
    152)
            ' Заголовки колонок
            g.DrawString(
            "Ваши:",
            normalFont,
            textBrush,
            ActiveRackStartX,
            200)

            g.DrawString(
            "Соперник:",
            normalFont,
            textBrush,
            OpponentRackStartX - 10,
            200)

            ' Общая рамка
            g.DrawRectangle(
            Pens.LightGray,
            RightPanelX + 15,
            225,
            260,
            540)

            ' Вертикальный разделитель
            g.DrawLine(
            Pens.LightGray,
            RightPanelX + 145,
            225,
            RightPanelX + 145,
            750)

            Dim otherPlayer As Player = Nothing

            For Each p As Player In players

                If p IsNot currentPlayer Then
                    otherPlayer = p
                    Exit For
                End If

            Next

            If otherPlayer IsNot Nothing Then

                DrawStaticRackVertical(
                g,
                otherPlayer,
                OpponentRackStartX,
                OpponentRackStartY)

            End If

        End Using

    End Sub

    Private Sub DrawStaticRackVertical(
    g As Graphics,
    player As Player,
    startX As Integer,
    startY As Integer)

        Dim y As Integer = startY

        For Each tile As Tile In player.Rack

            DrawSmallTile(
            g,
            tile,
            startX,
            y)

            y += OpponentRackStepY

        Next

    End Sub

    Private Sub DrawSmallTile(
    g As Graphics,
    tile As Tile,
    x As Integer,
    y As Integer)

        DrawPinkLetterTile(
        g,
        tile.Letter.ToString(),
        tile.Value,
        x,
        y)

    End Sub
    Private Sub DrawConfirmedBoardLetter(
    g As Graphics,
    t As TileInstance)

        Dim letterFont As New Font(
        "Times New Roman",
        30,
        FontStyle.Bold)

        Dim scoreFont As New Font(
        "Arial",
        8,
        FontStyle.Regular)

        Using letterBrush As New SolidBrush(Color.Black)

            g.DrawString(
            t.VisibleLetter,
            letterFont,
            letterBrush,
            t.ScreenX + 8,
            t.ScreenY + 4)

            g.DrawString(
            t.Tile.Value.ToString(),
            scoreFont,
            letterBrush,
            t.ScreenX + 42,
            t.ScreenY + 42)

        End Using

    End Sub

    Private Sub DrawRackTile(
    g As Graphics,
    t As TileInstance)

        DrawPinkLetterTile(
        g,
        t.VisibleLetter,
        t.Tile.Value,
        t.ScreenX,
        t.ScreenY)

    End Sub
    Private Sub DrawPinkLetterTile(
    g As Graphics,
    letter As String,
    value As Integer,
    x As Integer,
    y As Integer)

        Dim letterFont As New Font(
        "Times New Roman",
        31,
        FontStyle.Bold)

        Dim scoreFont As New Font(
        "Arial",
        8,
        FontStyle.Regular)

        Using shadowBrush As New SolidBrush(RackLetterShadowColor)

            g.DrawString(
            letter,
            letterFont,
            shadowBrush,
            x + 2,
            y + 2)

            g.DrawString(
            value.ToString(),
            scoreFont,
            shadowBrush,
            x + 38,
            y + 40)

        End Using

        Using letterBrush As New SolidBrush(RackLetterColor)

            g.DrawString(
            letter,
            letterFont,
            letterBrush,
            x,
            y)

            g.DrawString(
            value.ToString(),
            scoreFont,
            letterBrush,
            x + 36,
            y + 38)

        End Using

    End Sub
    Private Function CheckGameOver() As Boolean

        If currentPlayer.Score < targetScore Then
            Return False
        End If

        gameOver = True
        StatsManager.RecordGame(
    players(0).Name,
    players(0).Score,
    players(1).Name,
    players(1).Score,
    currentPlayer.Name,
    currentGameBestWord,
    currentGameBestWordScore,
    currentGameBestWordPlayer,
    currentGameBestMoveScore,
    currentGameBestMovePlayer)
        MessageBox.Show(
    currentPlayer.Name &
    " победил!" &
    Environment.NewLine &
    "Счёт: " &
    currentPlayer.Score.ToString() &
    " очков" &
    Environment.NewLine &
    "Цель партии: " &
    targetScore.ToString(),
    "Конец игры",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information)

        btnConfirmMove.Enabled = False
        btnCancelMove.Enabled = False

        Me.Text =
        "Эрудит - победил " &
        currentPlayer.Name

        Me.Invalidate()

        Return True

    End Function
    Private Sub SetupButtons()

        If btnConfirmMove Is Nothing OrElse
       btnCancelMove Is Nothing Then

            Exit Sub

        End If

        btnConfirmMove.Text = "Готово"
        btnCancelMove.Text = "Отмена"
        btnNewGame.Text = "Новая игра"
        btnStats.Text = "Статистика"

        btnRackMode.Size = New Size(240, 34)
        btnStats.Size = New Size(240, 34)
        btnNewGame.Size = New Size(240, 34)

        btnConfirmMove.Size = New Size(115, 36)
        btnCancelMove.Size = New Size(115, 36)

        Dim buttonY As Integer =
        Me.ClientSize.Height - 58

        btnRackMode.Location =
        New Point(
            RightPanelX + 25,
            buttonY - 132)

        btnStats.Location =
        New Point(
            RightPanelX + 25,
            buttonY - 88)

        btnNewGame.Location =
        New Point(
            RightPanelX + 25,
            buttonY - 44)

        btnConfirmMove.Location =
        New Point(
            RightPanelX + 25,
            buttonY)

        btnCancelMove.Location =
        New Point(
            RightPanelX + 150,
            buttonY)

        StyleGameButton(btnRackMode)
        StyleGameButton(btnStats)
        StyleGameButton(btnNewGame)
        StyleGameButton(btnConfirmMove)
        StyleGameButton(btnCancelMove)

        UpdateRackModeButton()

    End Sub
    Private Sub Form1_Resize(
    sender As Object,
    e As EventArgs) _
    Handles MyBase.Resize

        SetupButtons()
        Me.Invalidate()

    End Sub
    Private Function EnsureJokerLetterSelected(
    t As TileInstance
) As Boolean

        If t Is Nothing Then
            Return True
        End If

        If Not t.IsJoker Then
            Return True
        End If

        If t.JokerLetter <> "" Then
            Return True
        End If

        Dim selectedLetter As String =
            AskJokerLetter()

        If selectedLetter = "" Then
            Return False
        End If

        t.JokerLetter = selectedLetter

        Return True

    End Function


    Private Function AskJokerLetter() As String

        Dim alphabet As String =
            "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"

        Do

            Dim input As String =
                InputBox(
                    "Введите букву для джокера:" &
                    Environment.NewLine &
                    alphabet,
                    "Джокер").Trim().ToUpper()

            If input = "" Then
                Return ""
            End If

            If input = "Ё" Then
                input = "Е"
            End If

            Dim letter As String =
                input.Substring(0, 1)

            If alphabet.Contains(letter) Then
                Return letter
            End If

            MessageBox.Show(
                "Нужно ввести одну русскую букву.",
                "Джокер",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

        Loop

    End Function


    Private Sub ReturnTileToHome(
        t As TileInstance)

        If t Is Nothing Then
            Exit Sub
        End If

        t.ScreenX = t.HomeX
        t.ScreenY = t.HomeY
        t.BoardX = -1
        t.BoardY = -1

        If Not t.Confirmed Then
            t.JokerLetter = ""
        End If

    End Sub
    Private Sub StyleGameButton(
    btn As Button)

        btn.BackColor =
            Color.FromArgb(0, 90, 0)

        btn.ForeColor =
            Color.White

        btn.Font =
            New Font(
                "Arial",
                9,
                FontStyle.Bold)

        btn.FlatStyle =
            FlatStyle.Flat

        btn.FlatAppearance.BorderColor =
            Color.White

    End Sub
    Private Sub btnRackMode_Click(
    sender As Object,
    e As EventArgs)

        If rackBalanceMode = RackBalanceMode.Comfort Then

            rackBalanceMode =
                RackBalanceMode.VeryComfort

        Else

            rackBalanceMode =
                RackBalanceMode.Comfort

        End If

        UpdateRackModeButton()

        Me.Invalidate()

    End Sub


    Private Sub UpdateRackModeButton()

        If rackBalanceMode = RackBalanceMode.Comfort Then

            btnRackMode.Text =
                "Режим: классический"

        Else

            btnRackMode.Text =
                "Режим: комфорт"

        End If

    End Sub
    Private Sub EnsureBalancedRack(
    player As Player)

        RackBalancer.Balance(
            player,
            bag,
            rackBalanceMode)

    End Sub
    Private Sub btnNewGame_Click(
    sender As Object,
    e As EventArgs)

        Using dlg As New NewGameDialog(
        player1Name,
        player2Name,
        targetScore)

            If dlg.ShowDialog(Me) <> DialogResult.OK Then
                Exit Sub
            End If

            player1Name = dlg.Player1Name
            player2Name = dlg.Player2Name
            targetScore = dlg.TargetScore

        End Using

        StartNewGame()

    End Sub
    Private Sub StartNewGame()

        gameOver = False
        currentGameBestWord = ""
        currentGameBestWordScore = 0
        currentGameBestWordPlayer = ""

        currentGameBestMoveScore = 0
        currentGameBestMovePlayer = ""
        gameBoard = New Board()
        bag = New TileBag()

        tileInstances.Clear()

        players.Clear()
        currentPlayerIndex = 0

        CreateStartTile()

        Dim p1 As New Player()
        p1.Name = player1Name
        p1.Score = 0

        Dim p2 As New Player()
        p2.Name = player2Name
        p2.Score = 0

        players.Add(p1)
        players.Add(p2)

        For Each p As Player In players

            p.Rack.Clear()

            For i = 1 To 7

                Dim tile As Tile =
                    bag.Draw()

                If tile IsNot Nothing Then
                    p.Rack.Add(tile)
                End If

            Next

            EnsureBalancedRack(p)

        Next

        currentPlayer =
            players(0)

        LoadCurrentPlayerRack()

        btnConfirmMove.Enabled = True
        btnCancelMove.Enabled = True
        btnNewGame.Enabled = True
        btnRackMode.Enabled = True



        UpdatePlayersInfo()

        Me.Text =
            "Эрудит - " &
            currentPlayer.Name

        SetupButtons()

        Me.Invalidate()

    End Sub
    Private Function AskTargetScore() As Boolean

        Dim input As String =
            InputBox(
                "До скольки очков играем?" &
                Environment.NewLine &
                "Например: 100, 150, 200",
                "Цель партии",
                targetScore.ToString())

        If input.Trim() = "" Then
            Return False
        End If

        Dim value As Integer

        If Not Integer.TryParse(input.Trim(), value) Then

            MessageBox.Show(
                "Нужно ввести число.",
                "Цель партии",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            Return False

        End If

        If value < 20 OrElse value > 1000 Then

            MessageBox.Show(
                "Поставь нормальную цель: от 20 до 1000 очков.",
                "Цель партии",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            Return False

        End If

        targetScore = value

        Return True

    End Function
    Private Function AskPlayerNames() As Boolean

        Dim name1 As String =
            InputBox(
                "Имя первого игрока:",
                "Игроки",
                player1Name).Trim()

        If name1 = "" Then
            Return False
        End If


        Dim name2 As String =
            InputBox(
                "Имя второго игрока:",
                "Игроки",
                player2Name).Trim()

        If name2 = "" Then
            Return False
        End If


        player1Name = name1
        player2Name = name2

        Return True

    End Function
    Private Sub btnStats_Click(
    sender As Object,
    e As EventArgs)

        MessageBox.Show(
            StatsManager.GetStatsText(),
            "Статистика",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

    End Sub
    Private Sub UpdateCurrentGameRecords(
    words As List(Of String),
    wordScores As List(Of Integer),
    totalMoveScore As Integer)

        For i = 0 To words.Count - 1

            If wordScores(i) > currentGameBestWordScore Then

                currentGameBestWord =
                    words(i)

                currentGameBestWordScore =
                    wordScores(i)

                currentGameBestWordPlayer =
                    currentPlayer.Name

            End If

        Next


        If totalMoveScore > currentGameBestMoveScore Then

            currentGameBestMoveScore =
                totalMoveScore

            currentGameBestMovePlayer =
                currentPlayer.Name

        End If

    End Sub
End Class