Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Text

Imports System.Diagnostics


Public Class Form1

    Private gameBoard As Board
    Private bag As TileBag
    Private players As New List(Of Player)

    Private currentPlayerIndex As Integer = 0
    Private currentPlayer As Player


    Private player1Name As String = ""
    Private player2Name As String = ""

    Private lblAiThinking As New Label()

    Private aiThinkingWatch As Stopwatch = Nothing
    Private aiThinkingShown As Boolean = False

    Private Const MaxCellSize As Integer = 60
    Private Const MinCellSize As Integer = 30

    Private CellSize As Integer = 60

    Private StartX As Integer = 20
    Private StartY As Integer = 20

    Private RightPanelX As Integer = 940
    Private RightPanelWidth As Integer = 300

    Private ActiveRackStartX As Integer = 980
    Private ActiveRackStartY As Integer = 230
    Private ActiveRackStepY As Integer = 70

    Private OpponentRackStartX As Integer = 1120
    Private OpponentRackStartY As Integer = 230
    Private OpponentRackStepY As Integer = 70
    Private RackColumns As Integer = 1
    Private RackRows As Integer = 7
    Private RackStepX As Integer = 0
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
    RackBalanceMode.Classic

    Private btnRackMode As New Button()
    Private btnNewGame As New Button()
    Private currentGameBestWord As String = ""
    Private currentGameBestWordScore As Integer = 0
    Private currentGameBestWordPlayer As String = ""
    Private player2IsComputer As Boolean = False
    Private currentGameBestMoveScore As Integer = 0
    Private currentGameBestMovePlayer As String = ""
    Private btnStats As New Button()

    Private Const ComputerMinWordLength As Integer = 3

    Private Const ComputerCandidateLimit As Integer = 5000
    Private Const ComputerMaxWordsInMove As Integer = 10
    Private Const ComputerSecondWordPool As Integer = 120
    Private Const ComputerMaxWordLength As Integer = 7

    Private Const ComputerMaxSinglePlacements As Integer = 700
    Private Const ComputerPairCombinationPool As Integer = 45
    Private Const ComputerTripleCombinationPool As Integer = 18
    Private Const ComputerMaxPlacesPerMove As Integer = 4
    Private Const ComputerThinkLimitMs As Integer = 7000
    Private Const ComputerUiPulseMs As Integer = 100
    Private isComputerThinking As Boolean = False
    Private computerSearchWatch As Stopwatch = Nothing
    Private computerNextUiPulseMs As Long = 0
    Private computerDebugMoves As New List(Of ComputerMove)
    Private Const ComputerDebugMoveLimit As Integer = 10
    Private Sub Form1_Load(sender As Object,
                           e As EventArgs) _
                           Handles MyBase.Load

        Me.DoubleBuffered = True
        Me.BackColor = FormBackColor

        Dim workArea As Rectangle =
    Screen.PrimaryScreen.WorkingArea

        Me.StartPosition = FormStartPosition.Manual

        Me.MinimumSize = New Size(820, 650)

        Me.Bounds =
    New Rectangle(
        workArea.Left,
        workArea.Top,
        Math.Min(1260, workArea.Width),
        Math.Min(920, workArea.Height))

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
        lblAiThinking.Text = "ИИ думает..."
        lblAiThinking.Visible = False
        lblAiThinking.TextAlign = ContentAlignment.MiddleCenter
        lblAiThinking.BorderStyle = BorderStyle.FixedSingle
        lblAiThinking.BackColor = Color.FromArgb(255, 245, 180)
        lblAiThinking.ForeColor = Color.Black
        lblAiThinking.Font = New Font("Arial", 10, FontStyle.Bold)

        Me.Controls.Add(lblAiThinking)
        RemoveDuplicateButtons()
        lblAiThinking.BringToFront()
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

        DictionaryManager.LoadDictionary(
    Path.Combine(
        Application.StartupPath,
        "russian.txt"))

        DictionaryManager.LoadAiDictionary(
    Path.Combine(
        Application.StartupPath,
        "ai_words.txt"))


        Dim firstLaunch As Boolean =
    Not HasLastGameSettings()

        LoadLastGameSettings()
        UpdateRackModeButton()

        If firstLaunch Then
            ShowNewGameDialog()
        End If

        StartNewGame()

        UpdateAdaptiveLayout()
        UpdateButtonsLayout()
        UpdateRackModeButton()

        Me.Invalidate()
        Me.Refresh()
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

        If isComputerThinking Then
            Exit Sub
        End If

        If currentPlayer IsNot Nothing AndAlso
       currentPlayer.IsComputer Then

            Exit Sub

        End If


        Dim t As TileInstance =
        FindDraggableTileAt(e.Location)

        If t Is Nothing Then
            Exit Sub
        End If


        dragTile = t

        dragOffsetX =
        e.X - t.ScreenX

        dragOffsetY =
        e.Y - t.ScreenY

    End Sub
    Private Function FindDraggableTileAt(
    mousePoint As Point
) As TileInstance

        Dim bestTile As TileInstance = Nothing
        Dim bestDistance As Integer = Integer.MaxValue

        For i = tileInstances.Count - 1 To 0 Step -1

            Dim t As TileInstance =
            tileInstances(i)

            If t Is Nothing Then
                Continue For
            End If

            ' Уже подтвержденные буквы двигать нельзя
            If t.Confirmed Then
                Continue For
            End If


            Dim isRackTile As Boolean =
            t.BoardX = -1 AndAlso
            t.BoardY = -1


            Dim drawSize As Integer

            If isRackTile Then

                If RackColumns = 2 Then
                    drawSize = 42
                Else
                    drawSize = Math.Min(CellSize, 60)
                End If

            Else

                drawSize = CellSize

            End If


            Dim centerX As Integer =
            t.ScreenX + drawSize \ 2

            Dim centerY As Integer =
            t.ScreenY + drawSize \ 2


            Dim dx As Integer =
            mousePoint.X - centerX

            Dim dy As Integer =
            mousePoint.Y - centerY

            Dim distance As Integer =
            dx * dx + dy * dy


            Dim radius As Integer

            If isRackTile Then

                If RackColumns = 2 Then
                    radius = 48
                Else
                    radius = 58
                End If

            Else

                radius = Math.Max(30, CellSize \ 2)

            End If


            If distance <= radius * radius AndAlso
           distance < bestDistance Then

                bestDistance = distance
                bestTile = t

            End If

        Next

        Return bestTile

    End Function

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
        If currentPlayer IsNot Nothing AndAlso
   currentPlayer.IsComputer Then

            Exit Sub

        End If
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


        Using dlg As New MoveConfirmDialog(
    currentPlayer.Name,
    words,
    wordScores,
    baseScore,
    bonusScore,
    totalScore)

            dlg.ShowDialog(Me)

            If dlg.BannedWord <> "" Then

                DictionaryManager.AddAiBadWord(
            dlg.BannedWord)

                MessageBox.Show(
            "Слово """ &
            dlg.BannedWord &
            """ добавлено в запрещённые." &
            Environment.NewLine &
            "Ход не засчитан.",
            "Слово запрещено",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

                Exit Sub

            End If

            If Not dlg.ConfirmMove Then
                Exit Sub
            End If

        End Using




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
        UpdateAdaptiveLayout()
        UpdatePlayersInfo()

        Me.Text =
        "Эрудит - " &
        currentPlayer.Name &
        " | Очки: " &
        currentPlayer.Score.ToString()

        Me.Invalidate()
        If currentPlayer.IsComputer AndAlso Not gameOver Then

            Me.BeginInvoke(
                New MethodInvoker(
                    AddressOf MakeComputerMove))

        End If

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

        If currentPlayer Is Nothing Then
            Exit Sub
        End If

        While currentPlayer.Rack.Count < 7

            Dim tile As Tile =
            bag.Draw()

            If tile Is Nothing Then
                Exit While
            End If

            currentPlayer.Rack.Add(tile)

        End While

        ApplyRackBalance(currentPlayer)

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
            190)

            g.DrawString(
            "Соперник:",
            normalFont,
            textBrush,
            OpponentRackStartX - 10,
            190)



            Dim rackFrameY As Integer =
    ActiveRackStartY - 20

            Dim rackTileHeight As Integer

            If RackColumns = 2 Then
                rackTileHeight = 42
            Else
                rackTileHeight = Math.Min(CellSize, 60)
            End If

            Dim rackFrameHeight As Integer =
    (RackRows - 1) * ActiveRackStepY + rackTileHeight + 28

            If RackColumns = 2 Then
                rackTileHeight = 42
            Else
                rackTileHeight = Math.Min(CellSize, 60)
            End If



            Using p As New Pen(Color.White)
                g.DrawRectangle(
        p,
        RightPanelX,
        rackFrameY,
        RightPanelWidth,
        rackFrameHeight)
            End Using
            Using p As New Pen(Color.White)

                g.DrawLine(
        p,
        RightPanelX + RightPanelWidth \ 2,
        rackFrameY,
        RightPanelX + RightPanelWidth \ 2,
        rackFrameY + rackFrameHeight)

            End Using
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

        If player Is Nothing Then
            Exit Sub
        End If

        For i = 0 To player.Rack.Count - 1

            Dim col As Integer =
            i Mod RackColumns

            Dim row As Integer =
            i \ RackColumns

            DrawSmallTile(
            g,
            player.Rack(i),
            startX + col * RackStepX,
            startY + row * OpponentRackStepY)

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

        If t Is Nothing OrElse t.Tile Is Nothing Then
            Exit Sub
        End If

        Dim letter As String =
        t.VisibleLetter

        If letter = "" Then
            Exit Sub
        End If

        Dim x As Integer =
        StartX + t.BoardX * CellSize

        Dim y As Integer =
        StartY + t.BoardY * CellSize


        Dim letterFontSize As Integer =
        CInt(CellSize * 0.55)

        If letterFontSize < 14 Then
            letterFontSize = 14
        End If

        If letterFontSize > 34 Then
            letterFontSize = 34
        End If


        Dim valueFontSize As Integer =
        CInt(CellSize * 0.15)

        If valueFontSize < 6 Then
            valueFontSize = 6
        End If

        If valueFontSize > 9 Then
            valueFontSize = 9
        End If


        Using letterFont As New Font(
        "Times New Roman",
        letterFontSize,
        FontStyle.Bold)

            Using valueFont As New Font(
            "Arial",
            valueFontSize,
            FontStyle.Bold)

                Using letterBrush As New SolidBrush(Color.Black)

                    Dim letterRect As New RectangleF(
                    x,
                    y + CellSize * 0.08F,
                    CellSize,
                    CellSize * 0.75F)

                    Using sf As New StringFormat()

                        sf.Alignment = StringAlignment.Center
                        sf.LineAlignment = StringAlignment.Center

                        g.DrawString(
                        letter,
                        letterFont,
                        letterBrush,
                        letterRect,
                        sf)

                    End Using


                    Dim valueText As String =
                    t.Tile.Value.ToString()

                    Dim valueSize As SizeF =
                    g.MeasureString(
                        valueText,
                        valueFont)

                    Dim valueX As Single =
                    x + CellSize - valueSize.Width - CellSize * 0.12F

                    Dim valueY As Single =
                    y + CellSize - valueSize.Height - CellSize * 0.08F

                    g.DrawString(
                    valueText,
                    valueFont,
                    letterBrush,
                    valueX,
                    valueY)

                End Using

            End Using

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

        If letter Is Nothing OrElse letter = "" Then
            Exit Sub
        End If


        Dim letterFontSize As Integer

        If RackColumns = 2 Then
            letterFontSize = 24
        Else
            letterFontSize = CInt(CellSize * 0.58)
        End If

        If letterFontSize < 20 Then
            letterFontSize = 20
        End If

        If letterFontSize > 34 Then
            letterFontSize = 34
        End If


        Dim valueFontSize As Integer

        If RackColumns = 2 Then
            valueFontSize = 7
        Else
            valueFontSize = CInt(CellSize * 0.15)
        End If

        If valueFontSize < 6 Then
            valueFontSize = 6
        End If

        If valueFontSize > 9 Then
            valueFontSize = 9
        End If


        Dim drawSize As Integer =
        Math.Min(CellSize, 60)

        If RackColumns = 2 Then
            drawSize = 42
        End If


        Using letterFont As New Font(
        "Times New Roman",
        letterFontSize,
        FontStyle.Bold)

            Using valueFont As New Font(
            "Arial",
            valueFontSize,
            FontStyle.Bold)

                Using shadowBrush As New SolidBrush(RackLetterShadowColor)
                    Using letterBrush As New SolidBrush(RackLetterColor)

                        Dim letterRect As New RectangleF(
                    x - 4,
                    y,
                    drawSize + 8,
                    drawSize * 0.82F)

                        Using sf As New StringFormat()

                            sf.Alignment = StringAlignment.Center
                            sf.LineAlignment = StringAlignment.Center

                            Dim shadowOffset As Integer = 1

                            If CellSize < 45 OrElse RackColumns = 2 Then
                                shadowOffset = 0
                            End If

                            If shadowOffset > 0 Then

                                Dim shadowRect As RectangleF =
        letterRect

                                shadowRect.Offset(
        shadowOffset,
        shadowOffset)

                                g.DrawString(
        letter,
        letterFont,
        shadowBrush,
        shadowRect,
        sf)

                            End If

                            g.DrawString(
    letter,
    letterFont,
    letterBrush,
    letterRect,
    sf)

                        End Using


                        Dim valueText As String =
                    value.ToString()

                        Dim valueSize As SizeF =
                    g.MeasureString(
                        valueText,
                        valueFont)

                        Dim valueX As Single =
                    x + drawSize - valueSize.Width - 4

                        Dim valueY As Single =
                    y + drawSize - valueSize.Height - 4

                        g.DrawString(
                    valueText,
                    valueFont,
                    shadowBrush,
                    valueX + 1,
                    valueY + 1)

                        g.DrawString(
                    valueText,
                    valueFont,
                    letterBrush,
                    valueX,
                    valueY)

                    End Using
                End Using

            End Using

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
    e As EventArgs
) Handles MyBase.Resize

        UpdateAdaptiveLayout()
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

        Select Case rackBalanceMode

            Case RackBalanceMode.Chance
                rackBalanceMode = RackBalanceMode.Classic

            Case RackBalanceMode.Classic
                rackBalanceMode = RackBalanceMode.Comfort

            Case RackBalanceMode.Comfort
                rackBalanceMode = RackBalanceMode.Chance

        End Select

        UpdateRackModeButtonText()

    End Sub

    Private Sub UpdateRackModeButtonText()

        Select Case rackBalanceMode

            Case RackBalanceMode.Chance
                btnRackMode.Text = "Режим: случай"

            Case RackBalanceMode.Classic
                btnRackMode.Text = "Режим: классика"

            Case RackBalanceMode.Comfort
                btnRackMode.Text = "Режим: комфорт"

        End Select

    End Sub
    Private Sub UpdateRackModeButton()

        Select Case rackBalanceMode

            Case RackBalanceMode.Chance

                btnRackMode.Text =
                "Режим: случай"

            Case RackBalanceMode.Classic

                btnRackMode.Text =
                "Режим: классика"

            Case RackBalanceMode.Comfort

                btnRackMode.Text =
                "Режим: комфорт"

        End Select

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

        If ShowNewGameDialog() Then
            StartNewGame()
        End If

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
        p1.IsComputer = False

        Dim p2 As New Player()
        p2.Name = player2Name
        p2.Score = 0
        p2.IsComputer = player2IsComputer

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

            For Each pl As Player In players
                ApplyRackBalance(p)
            Next

        Next

        currentPlayer =
            players(0)

        LoadCurrentPlayerRack()
        UpdateAdaptiveLayout()
        btnConfirmMove.Enabled = True
        btnCancelMove.Enabled = True
        btnNewGame.Enabled = True
        btnRackMode.Enabled = True



        UpdatePlayersInfo()

        Me.Text =
            "Эрудит - " &
            currentPlayer.Name

        SetupButtons()

        UpdateAdaptiveLayout()
        UpdateButtonsLayout()
        UpdateAiThinkingLabelLayout()

        btnRackMode.BringToFront()
        btnStats.BringToFront()
        btnNewGame.BringToFront()
        btnConfirmMove.BringToFront()
        btnCancelMove.BringToFront()

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


    Private Function FindFreeHorizontalSlot(
        length As Integer
    ) As Point

        For y = 0 To Board.Size - 1

            For x = 0 To Board.Size - length

                If IsHorizontalSlotFree(x, y, length) Then
                    Return New Point(x, y)
                End If

            Next

        Next

        Return New Point(-1, -1)

    End Function


    Private Function IsHorizontalSlotFree(
        startBoardX As Integer,
        boardY As Integer,
        length As Integer
    ) As Boolean

        If IsTileAt(startBoardX - 1, boardY) Then
            Return False
        End If

        If IsTileAt(startBoardX + length, boardY) Then
            Return False
        End If

        For i = 0 To length - 1

            Dim x As Integer =
                startBoardX + i

            If IsTileAt(x, boardY) Then
                Return False
            End If

            If IsTileAt(x, boardY - 1) Then
                Return False
            End If

            If IsTileAt(x, boardY + 1) Then
                Return False
            End If

        Next

        Return True

    End Function


    Private Function IsTileAt(
        boardX As Integer,
        boardY As Integer
    ) As Boolean

        If boardX < 0 OrElse
           boardY < 0 OrElse
           boardX >= Board.Size OrElse
           boardY >= Board.Size Then

            Return False

        End If

        For Each t As TileInstance In tileInstances

            If t.BoardX = boardX AndAlso
               t.BoardY = boardY Then

                Return True

            End If

        Next

        Return False

    End Function

    Private Class ComputerMove

        Public Property Placements As List(Of ComputerPlacement)
        Public Property Score As Integer
        Public Property WordsCount As Integer
        Public Property TilesUsedCount As Integer
        Public Property IsCrossMove As Boolean = False

        Public Sub New()
            Placements = New List(Of ComputerPlacement)
        End Sub

    End Class


    Private Class ComputerPlacement

        Public Property Word As String
        Public Property BoardX As Integer
        Public Property BoardY As Integer
        Public Property Horizontal As Boolean

        Public Property PlacedTiles As List(Of TileInstance)
        Public Property UsedRackTiles As List(Of Tile)

        Public Sub New()
            PlacedTiles = New List(Of TileInstance)
            UsedRackTiles = New List(Of Tile)
        End Sub

    End Class


    Private Sub MakeComputerMove()

        If gameOver Then Exit Sub
        If currentPlayer Is Nothing Then Exit Sub
        If Not currentPlayer.IsComputer Then Exit Sub


        Dim move As ComputerMove = Nothing

        BeginAiThinkingIndicator()
        BeginComputerSearchLimit()

        Try

            move =
            FindBestComputerMove()

        Finally

            EndComputerSearchLimit()
            EndAiThinkingIndicator()

        End Try



        If move Is Nothing Then

            ExchangeAllCurrentPlayerLetters()

            MessageBox.Show(
            currentPlayer.Name &
            " не нашёл хода и меняет буквы.",
            "Ход компьютера",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            NextPlayer()
            Exit Sub

        End If


        Dim placedTiles As List(Of TileInstance) =
        PlaceComputerMove(move)

        If placedTiles.Count = 0 Then

            ExchangeAllCurrentPlayerLetters()
            NextPlayer()
            Exit Sub

        End If


        Dim wordTileLists =
        WordBuilder.BuildWordTiles(tileInstances)

        Dim words As New List(Of String)
        Dim wordScores As New List(Of Integer)


        For Each wordTiles As List(Of TileInstance) In wordTileLists

            Dim w As String =
            GetWordText(wordTiles)

            If Not DictionaryManager.IsGoodAiWord(w) Then

                CancelComputerPlacedTiles(placedTiles)

                ExchangeAllCurrentPlayerLetters()
                NextPlayer()
                Exit Sub

            End If

            Dim score As Integer =
            ScoreManager.CalculateWordScore(
                wordTiles,
                gameBoard)

            words.Add(w)
            wordScores.Add(score)

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


        ' Окно хода ИИ: можно засчитать, отменить или запретить слово.
        Using dlg As New MoveConfirmDialog(
        currentPlayer.Name,
        words,
        wordScores,
        baseScore,
        bonusScore,
        totalScore)

            dlg.ShowDialog(Me)

            If dlg.BannedWord <> "" Then

                DictionaryManager.AddAiBadWord(
                dlg.BannedWord)

                MessageBox.Show(
                "Слово """ &
                dlg.BannedWord &
                """ добавлено в запрещённые." &
                Environment.NewLine &
                "ИИ попробует другой ход.",
                "Слово запрещено",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

                CancelComputerPlacedTiles(placedTiles)

                Me.Invalidate()

                Me.BeginInvoke(
                New MethodInvoker(
                    AddressOf MakeComputerMove))

                Exit Sub

            End If


            If Not dlg.ConfirmMove Then

                CancelComputerPlacedTiles(placedTiles)

                Me.Invalidate()

                Me.BeginInvoke(
                New MethodInvoker(
                    AddressOf MakeComputerMove))

                Exit Sub

            End If

        End Using


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
    Private Function FindBestComputerMove() As ComputerMove
        'computerDebugMoves.Clear()
        Dim singlePlacements As List(Of ComputerPlacement) =
        FindSingleComputerPlacements()

        If singlePlacements.Count = 0 Then
            Return Nothing
        End If


        Dim bestMove As ComputerMove = Nothing
        Dim singleMoves As New List(Of ComputerMove)


        For Each placement As ComputerPlacement In singlePlacements
            If ComputerSearchPulse() Then
                Return bestMove
            End If
            Dim singleMove As ComputerMove =
        BuildComputerMoveFromPlacements(
            New List(Of ComputerPlacement) From {
                placement
            })

            If singleMove IsNot Nothing Then

                singleMoves.Add(singleMove)
                'RememberComputerDebugMove(singleMove)
                If IsBetterComputerMove(singleMove, bestMove) Then
                    bestMove = singleMove
                End If

            End If


            Dim secondPlacement As ComputerPlacement =
        TryFindCrossPlacementForBase(placement)

            If secondPlacement IsNot Nothing Then

                Dim crossMove As ComputerMove =
            BuildComputerMoveFromPlacements(
                New List(Of ComputerPlacement) From {
                    placement,
                    secondPlacement
                })

                If crossMove IsNot Nothing Then

                    crossMove.IsCrossMove = True
                    'RememberComputerDebugMove(crossMove)
                    If IsBetterComputerMove(crossMove, bestMove) Then
                        bestMove = crossMove
                    End If

                End If

            End If

        Next


        singleMoves.Sort(
        Function(a As ComputerMove, b As ComputerMove)

            Return GetComputerMovePriority(b).
                CompareTo(GetComputerMovePriority(a))

        End Function)


        Dim pairPool As New List(Of ComputerPlacement)

        For i = 0 To Math.Min(
        ComputerPairCombinationPool,
        singleMoves.Count) - 1

            pairPool.Add(singleMoves(i).Placements(0))

        Next


        For i = 0 To pairPool.Count - 2
            If ComputerSearchPulse() Then
                Return bestMove
            End If
            For j = i + 1 To pairPool.Count - 1

                Dim move As ComputerMove =
                BuildComputerMoveFromPlacements(
                    New List(Of ComputerPlacement) From {
                        pairPool(i),
                        pairPool(j)
                    })

                If move IsNot Nothing Then
                    'RememberComputerDebugMove(move)
                    If IsBetterComputerMove(move, bestMove) Then
                        bestMove = move
                    End If

                End If

            Next

        Next


        If ComputerMaxPlacesPerMove >= 3 Then

            Dim triplePoolCount As Integer =
            Math.Min(
                ComputerTripleCombinationPool,
                pairPool.Count)

            For i = 0 To triplePoolCount - 3
                If ComputerSearchPulse() Then
                    Return bestMove
                End If
                For j = i + 1 To triplePoolCount - 2

                    For k = j + 1 To triplePoolCount - 1

                        Dim move As ComputerMove =
                        BuildComputerMoveFromPlacements(
                            New List(Of ComputerPlacement) From {
                                pairPool(i),
                                pairPool(j),
                                pairPool(k)
                            })

                        If move IsNot Nothing Then
                            'RememberComputerDebugMove(move)
                            If IsBetterComputerMove(move, bestMove) Then
                                bestMove = move
                            End If

                        End If

                    Next

                Next

            Next

        End If


        Return bestMove

    End Function
    'Private Function TryEvaluateComputerMove(
    '    word As String,
    '    startBoardX As Integer,
    '    startBoardY As Integer,
    '    horizontal As Boolean
    ') As ComputerMove

    '    If Not WordFitsOnBoard(
    '        word,
    '        startBoardX,
    '        startBoardY,
    '        horizontal) Then

    '        Return Nothing

    '    End If


    '    If HasTileBeforeOrAfterWord(
    '        word,
    '        startBoardX,
    '        startBoardY,
    '        horizontal) Then

    '        Return Nothing

    '    End If


    '    Dim usedRackTiles As New List(Of Tile)
    '    Dim placedTiles As New List(Of TileInstance)

    '    Dim touchesExistingTile As Boolean = False


    '    For i = 0 To word.Length - 1

    '        Dim boardX As Integer =
    '            startBoardX

    '        Dim boardY As Integer =
    '            startBoardY

    '        If horizontal Then
    '            boardX += i
    '        Else
    '            boardY += i
    '        End If


    '        Dim existingTile As TileInstance =
    '            GetTileAt(boardX, boardY)

    '        Dim neededLetter As Char =
    '            word(i)

    '        If existingTile IsNot Nothing Then

    '            If existingTile.VisibleLetter <> neededLetter.ToString() Then
    '                Return Nothing
    '            End If

    '            touchesExistingTile = True

    '        Else

    '            Dim rackTile As Tile =
    '                FindRackTileForLetter(
    '                    neededLetter,
    '                    usedRackTiles)

    '            Dim jokerLetter As String = ""

    '            If rackTile Is Nothing Then

    '                rackTile =
    '                    FindRackJoker(usedRackTiles)

    '                If rackTile IsNot Nothing Then
    '                    jokerLetter = neededLetter.ToString()
    '                End If

    '            End If

    '            If rackTile Is Nothing Then
    '                Return Nothing
    '            End If

    '            usedRackTiles.Add(rackTile)

    '            Dim screenX As Integer =
    '                StartX + boardX * CellSize

    '            Dim screenY As Integer =
    '                StartY + boardY * CellSize

    '            placedTiles.Add(
    '                New TileInstance With {
    '                    .Tile = rackTile,
    '                    .ScreenX = screenX,
    '                    .ScreenY = screenY,
    '                    .HomeX = screenX,
    '                    .HomeY = screenY,
    '                    .BoardX = boardX,
    '                    .BoardY = boardY,
    '                    .Confirmed = False,
    '                    .JokerLetter = jokerLetter
    '                })

    '        End If

    '    Next


    '    If Not touchesExistingTile Then
    '        Return Nothing
    '    End If

    '    If placedTiles.Count = 0 Then
    '        Return Nothing
    '    End If


    '    For Each t As TileInstance In placedTiles
    '        tileInstances.Add(t)
    '    Next


    '    Dim result As ComputerMove = Nothing

    '    Try

    '        If Not MoveValidator.ValidateMove(tileInstances) Then
    '            Return Nothing
    '        End If


    '        Dim wordTileLists =
    '            WordBuilder.BuildWordTiles(tileInstances)

    '        If wordTileLists.Count = 0 Then
    '            Return Nothing
    '        End If
    '        If wordTileLists.Count > ComputerMaxWordsInMove Then
    '            Return Nothing
    '        End If

    '        Dim totalScore As Integer = 0

    '        For Each wordTiles As List(Of TileInstance) In wordTileLists

    '            Dim builtWord As String =
    '                GetWordText(wordTiles)
    '            If builtWord.Length < ComputerMinWordLength Then
    '                Return Nothing
    '            End If
    '            If Not DictionaryManager.IsGoodAiWord(builtWord) Then
    '                Return Nothing
    '            End If

    '            totalScore +=
    '                ScoreManager.CalculateWordScore(
    '                    wordTiles,
    '                    gameBoard)

    '        Next


    '        If placedTiles.Count = 7 Then
    '            totalScore += AllTilesBonus
    '        End If


    '        result =
    '            New ComputerMove With {
    '                .Word = word,
    '                .BoardX = startBoardX,
    '                .BoardY = startBoardY,
    '                .Horizontal = horizontal,
    '                .Score = totalScore
    '            }

    '    Finally

    '        For Each t As TileInstance In placedTiles
    '            tileInstances.Remove(t)
    '        Next

    '    End Try

    '    Return result

    'End Function


    Private Function PlaceComputerMove(
    move As ComputerMove
) As List(Of TileInstance)

        Dim placed As New List(Of TileInstance)

        If move Is Nothing Then
            Return placed
        End If

        If move.Placements Is Nothing Then
            Return placed
        End If

        For Each placement As ComputerPlacement In move.Placements

            For Each t As TileInstance In placement.PlacedTiles

                tileInstances.Add(t)
                placed.Add(t)

            Next

        Next

        Me.Invalidate()

        Return placed

    End Function
    Private Function WordFitsOnBoard(
        word As String,
        startBoardX As Integer,
        startBoardY As Integer,
        horizontal As Boolean
    ) As Boolean

        If startBoardX < 0 OrElse
           startBoardY < 0 Then

            Return False

        End If

        If horizontal Then

            If startBoardX + word.Length > Board.Size Then
                Return False
            End If

            If startBoardY >= Board.Size Then
                Return False
            End If

        Else

            If startBoardY + word.Length > Board.Size Then
                Return False
            End If

            If startBoardX >= Board.Size Then
                Return False
            End If

        End If

        Return True

    End Function


    Private Function HasTileBeforeOrAfterWord(
        word As String,
        startBoardX As Integer,
        startBoardY As Integer,
        horizontal As Boolean
    ) As Boolean

        Dim beforeX As Integer =
            startBoardX

        Dim beforeY As Integer =
            startBoardY

        Dim afterX As Integer =
            startBoardX

        Dim afterY As Integer =
            startBoardY

        If horizontal Then

            beforeX -= 1
            afterX += word.Length

        Else

            beforeY -= 1
            afterY += word.Length

        End If

        If GetTileAt(beforeX, beforeY) IsNot Nothing Then
            Return True
        End If

        If GetTileAt(afterX, afterY) IsNot Nothing Then
            Return True
        End If

        Return False

    End Function


    Private Function GetTileAt(
        boardX As Integer,
        boardY As Integer
    ) As TileInstance

        If boardX < 0 OrElse
           boardY < 0 OrElse
           boardX >= Board.Size OrElse
           boardY >= Board.Size Then

            Return Nothing

        End If

        For Each t As TileInstance In tileInstances

            If t.BoardX = boardX AndAlso
               t.BoardY = boardY Then

                Return t

            End If

        Next

        Return Nothing

    End Function


    Private Function FindRackTileForLetter(
        letter As Char,
        usedTiles As List(Of Tile)
    ) As Tile

        For Each tile As Tile In currentPlayer.Rack

            If usedTiles.Contains(tile) Then
                Continue For
            End If

            If tile.Letter = letter Then
                Return tile
            End If

        Next

        Return Nothing

    End Function


    Private Function FindRackJoker(
        usedTiles As List(Of Tile)
    ) As Tile

        For Each tile As Tile In currentPlayer.Rack

            If usedTiles.Contains(tile) Then
                Continue For
            End If

            If tile.Letter = "*"c Then
                Return tile
            End If

        Next

        Return Nothing

    End Function


    Private Sub CancelComputerPlacedTiles(
        placedTiles As List(Of TileInstance))

        For Each t As TileInstance In placedTiles

            If tileInstances.Contains(t) Then
                tileInstances.Remove(t)
            End If

        Next

        Me.Invalidate()

    End Sub


    Private Function IsComputerAllowedWord(
        word As String
    ) As Boolean

        Dim alphabet As String =
            "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"

        For Each ch As Char In word

            If Not alphabet.Contains(ch.ToString()) Then
                Return False
            End If

        Next

        Return True

    End Function
    Private Function GetBoardAnchorLetters() As String

        Dim result As String = ""

        For Each t As TileInstance In tileInstances

            If t.Confirmed AndAlso
               t.BoardX <> -1 AndAlso
               t.BoardY <> -1 Then

                Dim letter As String =
                    t.VisibleLetter

                If letter <> "" AndAlso
                   letter <> "*" AndAlso
                   Not result.Contains(letter) Then

                    result &= letter

                End If

            End If

        Next

        Return result

    End Function
    Private Function FindSingleComputerPlacements() _
    As List(Of ComputerPlacement)

        Dim result As New List(Of ComputerPlacement)

        Dim candidateWords As List(Of String) =
            SimpleAi.BuildCandidateWords(
                currentPlayer.Rack,
                GetBoardAnchorLetters(),
                ComputerMaxWordLength,
                ComputerCandidateLimit)

        For Each word As String In candidateWords
            If ComputerSearchPulse() Then
                Return result
            End If
            If word.Length < ComputerMinWordLength Then
                Continue For
            End If

            If word.Length > ComputerMaxWordLength Then
                Continue For
            End If

            If Not IsComputerAllowedWord(word) Then
                Continue For
            End If


            For y = 0 To Board.Size - 1
                If ComputerSearchPulse() Then
                    Return result
                End If
                For x = 0 To Board.Size - 1
                    If ComputerSearchPulse() Then
                        Return result
                    End If
                    Dim horizontalPlacement As ComputerPlacement =
                        TryBuildComputerPlacement(
                            word,
                            x,
                            y,
                            True)

                    If horizontalPlacement IsNot Nothing Then

                        result.Add(horizontalPlacement)

                        If result.Count >= ComputerMaxSinglePlacements Then
                            Return result
                        End If

                    End If


                    Dim verticalPlacement As ComputerPlacement =
                        TryBuildComputerPlacement(
                            word,
                            x,
                            y,
                            False)

                    If verticalPlacement IsNot Nothing Then

                        result.Add(verticalPlacement)

                        If result.Count >= ComputerMaxSinglePlacements Then
                            Return result
                        End If

                    End If

                Next

            Next

        Next

        Return result

    End Function


    Private Function TryBuildComputerPlacement(
        word As String,
        startBoardX As Integer,
        startBoardY As Integer,
        horizontal As Boolean
    ) As ComputerPlacement

        If Not WordFitsOnBoard(
            word,
            startBoardX,
            startBoardY,
            horizontal) Then

            Return Nothing

        End If


        If HasTileBeforeOrAfterWord(
            word,
            startBoardX,
            startBoardY,
            horizontal) Then

            Return Nothing

        End If


        Dim placement As New ComputerPlacement()
        placement.Word = word
        placement.BoardX = startBoardX
        placement.BoardY = startBoardY
        placement.Horizontal = horizontal

        Dim touchesExistingTile As Boolean = False


        For i = 0 To word.Length - 1

            Dim boardX As Integer =
                startBoardX

            Dim boardY As Integer =
                startBoardY

            If horizontal Then
                boardX += i
            Else
                boardY += i
            End If


            Dim existingTile As TileInstance =
                GetTileAt(boardX, boardY)

            Dim neededLetter As Char =
                word(i)

            If existingTile IsNot Nothing Then

                If existingTile.VisibleLetter.ToUpper() <>
                   neededLetter.ToString() Then

                    Return Nothing

                End If

                touchesExistingTile = True

            Else

                Dim rackTile As Tile =
                    FindRackTileForLetter(
                        neededLetter,
                        placement.UsedRackTiles)

                Dim jokerLetter As String = ""

                If rackTile Is Nothing Then

                    rackTile =
                        FindRackJoker(
                            placement.UsedRackTiles)

                    If rackTile IsNot Nothing Then
                        jokerLetter = neededLetter.ToString()
                    End If

                End If

                If rackTile Is Nothing Then
                    Return Nothing
                End If

                placement.UsedRackTiles.Add(rackTile)

                Dim screenX As Integer =
                    StartX + boardX * CellSize

                Dim screenY As Integer =
                    StartY + boardY * CellSize

                placement.PlacedTiles.Add(
                    New TileInstance With {
                        .Tile = rackTile,
                        .ScreenX = screenX,
                        .ScreenY = screenY,
                        .HomeX = screenX,
                        .HomeY = screenY,
                        .BoardX = boardX,
                        .BoardY = boardY,
                        .Confirmed = False,
                        .JokerLetter = jokerLetter
                    })

            End If

        Next


        If Not touchesExistingTile Then
            Return Nothing
        End If

        If placement.PlacedTiles.Count = 0 Then
            Return Nothing
        End If

        Return placement

    End Function


    Private Function BuildComputerMoveFromPlacements(
    placements As List(Of ComputerPlacement)
) As ComputerMove

        If placements Is Nothing OrElse
       placements.Count = 0 Then

            Return Nothing

        End If


        For i = 0 To placements.Count - 2

            For j = i + 1 To placements.Count - 1

                If Not ArePlacementsCompatible(
                placements(i),
                placements(j)) Then

                    Return Nothing

                End If

            Next

        Next


        Dim newTiles As New List(Of TileInstance)

        For Each placement As ComputerPlacement In placements

            If placement Is Nothing Then
                Return Nothing
            End If

            For Each t As TileInstance In placement.PlacedTiles

                newTiles.Add(t)

            Next

        Next


        If newTiles.Count = 0 Then
            Return Nothing
        End If


        Dim testTiles As New List(Of TileInstance)

        For Each t As TileInstance In tileInstances
            testTiles.Add(t)
        Next

        For Each t As TileInstance In newTiles
            testTiles.Add(t)
        Next


        Dim wordTileLists As List(Of List(Of TileInstance)) =
        WordBuilder.BuildWordTiles(testTiles)

        If wordTileLists.Count = 0 Then
            Return Nothing
        End If


        Dim totalScore As Integer = 0

        For Each wordTiles As List(Of TileInstance) In wordTileLists

            Dim builtWord As String =
            GetWordText(wordTiles)

            If builtWord.Length < ComputerMinWordLength Then
                Return Nothing
            End If

            If Not DictionaryManager.IsGoodAiWord(builtWord) Then
                Return Nothing
            End If

            totalScore +=
            ScoreManager.CalculateWordScore(
                wordTiles,
                gameBoard)

        Next


        If newTiles.Count = 7 Then
            totalScore += AllTilesBonus
        End If


        Dim result As New ComputerMove()

        result.Score = totalScore
        result.WordsCount = wordTileLists.Count
        result.TilesUsedCount = newTiles.Count

        For Each placement As ComputerPlacement In placements
            result.Placements.Add(placement)
        Next

        Return result

    End Function
    Private Function ArePlacementsCompatible(
        a As ComputerPlacement,
        b As ComputerPlacement
    ) As Boolean

        For Each tileA As Tile In a.UsedRackTiles

            For Each tileB As Tile In b.UsedRackTiles

                If tileA Is tileB Then
                    Return False
                End If

            Next

        Next


        For Each placedA As TileInstance In a.PlacedTiles

            For Each placedB As TileInstance In b.PlacedTiles

                If placedA.BoardX = placedB.BoardX AndAlso
                   placedA.BoardY = placedB.BoardY Then

                    Return False

                End If

            Next

        Next

        Return True

    End Function


    Private Function IsBetterComputerMove(
        candidate As ComputerMove,
        currentBest As ComputerMove
    ) As Boolean

        If candidate Is Nothing Then
            Return False
        End If

        If currentBest Is Nothing Then
            Return True
        End If

        Return GetComputerMovePriority(candidate) >
            GetComputerMovePriority(currentBest)

    End Function


    Private Function GetComputerMovePriority(
    move As ComputerMove
) As Integer

        If move Is Nothing Then
            Return -1
        End If

        Dim priority As Integer =
        move.Score * 10

        priority += move.TilesUsedCount * 8

        If move.TilesUsedCount = 7 Then
            priority += 180
        End If

        If move.IsCrossMove Then
            priority += 0
        End If
        If move.WordsCount >= 3 Then
            priority += 60
        End If
        priority += Math.Max(0, move.WordsCount - 1) * 20

        'If Not move.IsCrossMove Then
        '    priority += Math.Max(0, move.Placements.Count - 1) * 10
        'End If

        Return priority

    End Function
    Private Sub UpdateAdaptiveLayout()

        Dim margin As Integer = 20
        Dim gap As Integer = 20
        Dim panelWidth As Integer = 280

        Dim clientW As Integer = Me.ClientSize.Width
        Dim clientH As Integer = Me.ClientSize.Height

        If clientW <= 0 OrElse clientH <= 0 Then
            Exit Sub
        End If


        Dim availableBoardWidth As Integer =
        clientW - margin - gap - panelWidth - margin

        Dim availableBoardHeight As Integer =
        clientH - margin - margin

        Dim sizeByWidth As Integer =
        availableBoardWidth \ Board.Size

        Dim sizeByHeight As Integer =
        availableBoardHeight \ Board.Size

        CellSize =
        Math.Min(
            MaxCellSize,
            Math.Min(
                sizeByWidth,
                sizeByHeight))

        If CellSize < MinCellSize Then
            CellSize = MinCellSize
        End If


        StartX = margin
        StartY = margin

        RightPanelX =
    StartX + Board.Size * CellSize + gap

        RightPanelWidth = panelWidth


        Dim rackFrameX As Integer =
    RightPanelX + 15

        Dim rackFrameWidth As Integer =
    RightPanelWidth - 30

        ActiveRackStartY =
    StartY + 185

        OpponentRackStartY =
    ActiveRackStartY

        Dim normalRackHeight As Integer =
    7 * Math.Max(CellSize + 8, 42)

        Dim boardBottom As Integer =
    StartY + Board.Size * CellSize

        Dim spaceForRackAndButtons As Integer =
    boardBottom - ActiveRackStartY - 160

        If spaceForRackAndButtons < normalRackHeight Then

            RackColumns = 2
            RackRows = 4
            RackStepX = 48

            ActiveRackStepY = 42
            OpponentRackStepY = 42

        Else

            RackColumns = 1
            RackRows = 7
            RackStepX = 0

            ActiveRackStepY =
        Math.Max(
            CellSize + 8,
            42)

            OpponentRackStepY =
        ActiveRackStepY

        End If

        ActiveRackStartX =
    rackFrameX + 25

        OpponentRackStartX =
    rackFrameX + rackFrameWidth \ 2 + 25
        Dim desiredWidth As Integer =
    RightPanelX + RightPanelWidth + margin

        If Me.WindowState = FormWindowState.Normal Then

            Me.ClientSize =
        New Size(
            desiredWidth,
            StartY + Board.Size * CellSize + margin)

        End If

        UpdateButtonsLayout()
        RepositionAllTiles()

        UpdateAiThinkingLabelLayout()
    End Sub
    Private Sub UpdateButtonsLayout()

        If btnConfirmMove Is Nothing Then
            Exit Sub
        End If

        Dim buttonX As Integer =
        RightPanelX + 15

        Dim buttonW As Integer =
        RightPanelWidth - 30

        Dim rackFrameY As Integer =
        ActiveRackStartY - 20

        Dim rackTileHeight As Integer

        If RackColumns = 2 Then
            rackTileHeight = 42
        Else
            rackTileHeight = Math.Min(CellSize, 60)
        End If

        Dim rackFrameHeight As Integer =
        (RackRows - 1) * ActiveRackStepY + rackTileHeight + 28

        Dim buttonY As Integer =
        rackFrameY + rackFrameHeight + 12


        btnRackMode.Location =
        New Point(buttonX, buttonY)

        btnRackMode.Size =
        New Size(buttonW, 26)


        btnStats.Location =
        New Point(buttonX, buttonY + 34)

        btnStats.Size =
        New Size(buttonW, 26)


        btnNewGame.Location =
        New Point(buttonX, buttonY + 68)

        btnNewGame.Size =
        New Size(buttonW, 26)


        btnConfirmMove.Location =
        New Point(buttonX, buttonY + 104)

        btnConfirmMove.Size =
        New Size(buttonW \ 2 - 5, 30)


        btnCancelMove.Location =
        New Point(buttonX + buttonW \ 2 + 5, buttonY + 104)

        btnCancelMove.Size =
        New Size(buttonW \ 2 - 5, 30)

    End Sub
    Private Sub RepositionAllTiles()

        If tileInstances Is Nothing Then
            Exit Sub
        End If

        For Each t As TileInstance In tileInstances

            If t.BoardX <> -1 AndAlso
           t.BoardY <> -1 Then

                t.ScreenX =
                StartX + t.BoardX * CellSize

                t.ScreenY =
                StartY + t.BoardY * CellSize

                t.HomeX = t.ScreenX
                t.HomeY = t.ScreenY

            End If

        Next

        If currentPlayer Is Nothing Then
            Exit Sub
        End If

        Dim rackIndex As Integer = 0

        For Each tile As Tile In currentPlayer.Rack

            Dim t As TileInstance =
            FindRackTileInstance(tile)

            If t IsNot Nothing Then

                Dim col As Integer =
                rackIndex Mod RackColumns

                Dim row As Integer =
                rackIndex \ RackColumns

                t.ScreenX =
                ActiveRackStartX + col * RackStepX

                t.ScreenY =
                ActiveRackStartY + row * ActiveRackStepY

                t.HomeX = t.ScreenX
                t.HomeY = t.ScreenY

            End If

            rackIndex += 1

        Next

    End Sub


    Private Function FindRackTileInstance(
        tile As Tile
    ) As TileInstance

        For Each t As TileInstance In tileInstances

            If t.Tile Is tile AndAlso
               Not t.Confirmed AndAlso
               t.BoardX = -1 AndAlso
               t.BoardY = -1 Then

                Return t

            End If

        Next

        Return Nothing

    End Function
    Private Function TryFindCrossPlacementForBase(
    basePlacement As ComputerPlacement
) As ComputerPlacement

        If basePlacement Is Nothing Then
            Return Nothing
        End If

        If basePlacement.PlacedTiles.Count = 0 Then
            Return Nothing
        End If


        AddPlacementTilesToBoard(basePlacement)

        Dim bestSecond As ComputerPlacement = Nothing
        Dim bestMove As ComputerMove = Nothing

        Try

            Dim remainingRack As List(Of Tile) =
                GetRemainingRackForAi(basePlacement.UsedRackTiles)

            Dim anchorLetters As String =
                GetBoardAnchorLetters() &
                GetPlacementLetters(basePlacement)

            Dim candidateWords As List(Of String) =
                SimpleAi.BuildCandidateWords(
                    remainingRack,
                    anchorLetters,
                    ComputerMaxWordLength,
                    ComputerSecondWordPool)


            For Each word As String In candidateWords

                If word.Length < ComputerMinWordLength Then
                    Continue For
                End If

                If word.Length > ComputerMaxWordLength Then
                    Continue For
                End If

                If Not DictionaryManager.IsGoodAiWord(word) Then
                    Continue For
                End If


                For Each crossTile As TileInstance In basePlacement.PlacedTiles

                    Dim crossLetter As String =
                        crossTile.VisibleLetter

                    If crossLetter = "" Then
                        Continue For
                    End If


                    For letterIndex = 0 To word.Length - 1

                        If word(letterIndex).ToString() <> crossLetter Then
                            Continue For
                        End If

                        Dim startX As Integer
                        Dim startY As Integer

                        If basePlacement.Horizontal Then

                            startX = crossTile.BoardX
                            startY = crossTile.BoardY - letterIndex

                        Else

                            startX = crossTile.BoardX - letterIndex
                            startY = crossTile.BoardY

                        End If


                        Dim secondPlacement As ComputerPlacement =
                            TryBuildComputerPlacementWithBlockedTiles(
                                word,
                                startX,
                                startY,
                                Not basePlacement.Horizontal,
                                basePlacement.UsedRackTiles)

                        If secondPlacement Is Nothing Then
                            Continue For
                        End If


                        RemovePlacementTilesFromBoard(basePlacement)

                        Dim testMove As ComputerMove =
                            BuildComputerMoveFromPlacements(
                                New List(Of ComputerPlacement) From {
                                    basePlacement,
                                    secondPlacement
                                })

                        AddPlacementTilesToBoard(basePlacement)


                        If testMove Is Nothing Then
                            Continue For
                        End If

                        If IsBetterComputerMove(testMove, bestMove) Then

                            bestMove = testMove
                            bestSecond = secondPlacement

                        End If

                    Next

                Next

            Next

        Finally

            RemovePlacementTilesFromBoard(basePlacement)

        End Try

        Return bestSecond

    End Function
    Private Function GetPlacementLetters(
    placement As ComputerPlacement
) As String

        Dim result As String = ""

        If placement Is Nothing Then
            Return result
        End If

        For Each t As TileInstance In placement.PlacedTiles

            Dim letter As String =
                t.VisibleLetter

            If letter <> "" AndAlso
               Not result.Contains(letter) Then

                result &= letter

            End If

        Next

        Return result

    End Function


    Private Function GetRemainingRackForAi(
        usedTiles As List(Of Tile)
    ) As List(Of Tile)

        Dim result As New List(Of Tile)

        For Each tile As Tile In currentPlayer.Rack

            If usedTiles IsNot Nothing AndAlso
               usedTiles.Contains(tile) Then

                Continue For

            End If

            result.Add(tile)

        Next

        Return result

    End Function


    Private Sub AddPlacementTilesToBoard(
        placement As ComputerPlacement)

        If placement Is Nothing Then
            Exit Sub
        End If

        For Each t As TileInstance In placement.PlacedTiles

            If Not tileInstances.Contains(t) Then
                tileInstances.Add(t)
            End If

        Next

    End Sub


    Private Sub RemovePlacementTilesFromBoard(
        placement As ComputerPlacement)

        If placement Is Nothing Then
            Exit Sub
        End If

        For Each t As TileInstance In placement.PlacedTiles

            While tileInstances.Contains(t)
                tileInstances.Remove(t)
            End While

        Next

    End Sub
    Private Function TryBuildComputerPlacementWithBlockedTiles(
    word As String,
    startBoardX As Integer,
    startBoardY As Integer,
    horizontal As Boolean,
    blockedTiles As List(Of Tile)
) As ComputerPlacement

        If Not WordFitsOnBoard(
            word,
            startBoardX,
            startBoardY,
            horizontal) Then

            Return Nothing

        End If

        If HasTileBeforeOrAfterWord(
            word,
            startBoardX,
            startBoardY,
            horizontal) Then

            Return Nothing

        End If


        Dim placement As New ComputerPlacement()

        placement.Word = word
        placement.BoardX = startBoardX
        placement.BoardY = startBoardY
        placement.Horizontal = horizontal

        Dim touchesAnyTile As Boolean = False


        For i = 0 To word.Length - 1

            Dim boardX As Integer =
                startBoardX

            Dim boardY As Integer =
                startBoardY

            If horizontal Then
                boardX += i
            Else
                boardY += i
            End If


            Dim existingTile As TileInstance =
                GetTileAt(boardX, boardY)

            Dim neededLetter As Char =
                word(i)

            If existingTile IsNot Nothing Then

                If existingTile.VisibleLetter.ToUpper() <>
                   neededLetter.ToString() Then

                    Return Nothing

                End If

                touchesAnyTile = True

            Else

                Dim rackTile As Tile =
                    FindRackTileForLetterBlocked(
                        neededLetter,
                        placement.UsedRackTiles,
                        blockedTiles)

                Dim jokerLetter As String = ""

                If rackTile Is Nothing Then

                    rackTile =
                        FindRackJokerBlocked(
                            placement.UsedRackTiles,
                            blockedTiles)

                    If rackTile IsNot Nothing Then
                        jokerLetter = neededLetter.ToString()
                    End If

                End If

                If rackTile Is Nothing Then
                    Return Nothing
                End If

                placement.UsedRackTiles.Add(rackTile)

                Dim screenX As Integer =
                    StartX + boardX * CellSize

                Dim screenY As Integer =
                    StartY + boardY * CellSize

                placement.PlacedTiles.Add(
                    New TileInstance With {
                        .Tile = rackTile,
                        .ScreenX = screenX,
                        .ScreenY = screenY,
                        .HomeX = screenX,
                        .HomeY = screenY,
                        .BoardX = boardX,
                        .BoardY = boardY,
                        .Confirmed = False,
                        .JokerLetter = jokerLetter
                    })

            End If

        Next


        If Not touchesAnyTile Then
            Return Nothing
        End If

        If placement.PlacedTiles.Count = 0 Then
            Return Nothing
        End If

        Return placement

    End Function
    Private Function FindRackTileForLetterBlocked(
    letter As Char,
    usedTiles As List(Of Tile),
    blockedTiles As List(Of Tile)
) As Tile

        For Each tile As Tile In currentPlayer.Rack

            If usedTiles IsNot Nothing AndAlso
               usedTiles.Contains(tile) Then

                Continue For

            End If

            If blockedTiles IsNot Nothing AndAlso
               blockedTiles.Contains(tile) Then

                Continue For

            End If

            If tile.Letter = letter Then
                Return tile
            End If

        Next

        Return Nothing

    End Function


    Private Function FindRackJokerBlocked(
        usedTiles As List(Of Tile),
        blockedTiles As List(Of Tile)
    ) As Tile

        For Each tile As Tile In currentPlayer.Rack

            If usedTiles IsNot Nothing AndAlso
               usedTiles.Contains(tile) Then

                Continue For

            End If

            If blockedTiles IsNot Nothing AndAlso
               blockedTiles.Contains(tile) Then

                Continue For

            End If

            If tile.Letter = "*"c Then
                Return tile
            End If

        Next

        Return Nothing

    End Function
    Private Sub UpdateAiThinkingLabelLayout()

        If lblAiThinking Is Nothing Then
            Exit Sub
        End If

        lblAiThinking.Location =
            New Point(
                RightPanelX + 15,
                StartY + 145)

        lblAiThinking.Size =
            New Size(
                RightPanelWidth - 30,
                28)

        lblAiThinking.BringToFront()

    End Sub
    Private Sub BeginAiThinkingIndicator()

        isComputerThinking = True

        aiThinkingWatch =
        Stopwatch.StartNew()

        aiThinkingShown = True

        SetHumanControlsEnabled(False)

        If lblAiThinking IsNot Nothing Then

            If currentPlayer IsNot Nothing Then
                lblAiThinking.Text =
                currentPlayer.Name & " думает..."
            Else
                lblAiThinking.Text =
                "ИИ думает..."
            End If

            UpdateAiThinkingLabelLayout()

            lblAiThinking.Visible = True
            lblAiThinking.BringToFront()

        End If

        Me.Cursor = Cursors.WaitCursor

        Me.Refresh()
        Application.DoEvents()

    End Sub


    Private Sub MaybeShowAiThinkingIndicator()

        If Not isComputerThinking Then
            Exit Sub
        End If

        If lblAiThinking Is Nothing Then
            Exit Sub
        End If

        If currentPlayer IsNot Nothing Then
            lblAiThinking.Text =
            currentPlayer.Name & " думает..."
        Else
            lblAiThinking.Text =
            "ИИ думает..."
        End If

        UpdateAiThinkingLabelLayout()

        If Not lblAiThinking.Visible Then
            lblAiThinking.Visible = True
        End If

        lblAiThinking.BringToFront()

    End Sub


    Private Sub EndAiThinkingIndicator()

        If aiThinkingWatch IsNot Nothing Then
            aiThinkingWatch.Stop()
        End If

        aiThinkingWatch = Nothing
        aiThinkingShown = False
        isComputerThinking = False

        If lblAiThinking IsNot Nothing Then
            lblAiThinking.Visible = False
        End If

        SetHumanControlsEnabled(True)

        Me.Cursor = Cursors.Default

        Me.Refresh()
        Application.DoEvents()

    End Sub
    Private Function GetTileHitRect(
    t As TileInstance
) As Rectangle

        If t Is Nothing Then
            Return Rectangle.Empty
        End If


        Dim isRackTile As Boolean =
        t.BoardX = -1 AndAlso
        t.BoardY = -1


        Dim drawSize As Integer

        If isRackTile Then

            If RackColumns = 2 Then
                drawSize = 42
            Else
                drawSize = Math.Min(CellSize, 60)
            End If

        Else

            drawSize = CellSize

        End If


        Dim leftPadding As Integer
        Dim rightPadding As Integer
        Dim topPadding As Integer
        Dim bottomPadding As Integer

        If isRackTile Then

            ' Рука игрока:
            ' зону специально двигаем вправо.
            If RackColumns = 2 Then

                leftPadding = 8
                rightPadding = 34
                topPadding = 18
                bottomPadding = 18

            Else

                leftPadding = 30
                rightPadding = 30
                topPadding = 18
                bottomPadding = 18

            End If

        Else

            ' Неподтверждённая буква на поле.
            leftPadding = 8
            rightPadding = 12
            topPadding = 8
            bottomPadding = 8

        End If


        Return New Rectangle(
        t.ScreenX - leftPadding,
        t.ScreenY - topPadding,
        drawSize + leftPadding + rightPadding,
        drawSize + topPadding + bottomPadding)

    End Function
    Private Sub RemoveDuplicateButtons()

        Dim keep As New List(Of Button) From {
            btnRackMode,
            btnStats,
            btnNewGame,
            btnConfirmMove,
            btnCancelMove
        }

        For i = Me.Controls.Count - 1 To 0 Step -1

            Dim b As Button =
                TryCast(Me.Controls(i), Button)

            If b Is Nothing Then
                Continue For
            End If

            If keep.Contains(b) Then
                Continue For
            End If

            If b.Text = "Режим: классический" OrElse
               b.Text.StartsWith("Режим:") OrElse
               b.Text = "Статистика" OrElse
               b.Text = "Новая игра" OrElse
               b.Text = "Готово" OrElse
               b.Text = "Отмена" Then

                Me.Controls.Remove(b)
                b.Dispose()

            End If

        Next

    End Sub
    Private Sub BeginComputerSearchLimit()

        computerSearchWatch =
            Stopwatch.StartNew()

        computerNextUiPulseMs = 0

    End Sub


    Private Sub EndComputerSearchLimit()

        If computerSearchWatch IsNot Nothing Then
            computerSearchWatch.Stop()
        End If

        computerSearchWatch = Nothing
        computerNextUiPulseMs = 0

    End Sub


    Private Function ComputerSearchPulse() As Boolean

        If computerSearchWatch Is Nothing Then
            Return False
        End If

        Dim elapsed As Long =
            computerSearchWatch.ElapsedMilliseconds

        If elapsed >= computerNextUiPulseMs Then

            MaybeShowAiThinkingIndicator()

            Application.DoEvents()

            computerNextUiPulseMs =
                elapsed + ComputerUiPulseMs

        End If

        Return elapsed >= ComputerThinkLimitMs

    End Function
    Private Sub SetHumanControlsEnabled(
    enabled As Boolean)

        If btnRackMode IsNot Nothing Then
            btnRackMode.Enabled = enabled
        End If

        If btnStats IsNot Nothing Then
            btnStats.Enabled = enabled
        End If

        If btnNewGame IsNot Nothing Then
            btnNewGame.Enabled = enabled
        End If

        If btnConfirmMove IsNot Nothing Then
            btnConfirmMove.Enabled = enabled
        End If

        If btnCancelMove IsNot Nothing Then
            btnCancelMove.Enabled = enabled
        End If

    End Sub
    'Private Sub RememberComputerDebugMove(
    'move As ComputerMove)

    '    If move Is Nothing Then
    '        Exit Sub
    '    End If

    '    computerDebugMoves.Add(move)

    '    computerDebugMoves.Sort(
    '        Function(a As ComputerMove, b As ComputerMove)

    '            Return GetComputerMovePriority(b).
    '                CompareTo(GetComputerMovePriority(a))

    '        End Function)

    '    While computerDebugMoves.Count > ComputerDebugMoveLimit
    '        computerDebugMoves.RemoveAt(computerDebugMoves.Count - 1)
    '    End While

    'End Sub


    'Private Function GetComputerMoveDebugText() As String

    '    If computerDebugMoves Is Nothing OrElse
    '   computerDebugMoves.Count = 0 Then

    '        Return ""

    '    End If

    '    Dim text As String = ""

    '    For i = 0 To computerDebugMoves.Count - 1

    '        Dim move As ComputerMove =
    '        computerDebugMoves(i)

    '        text &=
    '        (i + 1).ToString() &
    '        ". " &
    '        GetComputerMoveText(move) &
    '        " | очки: " &
    '        move.Score.ToString() &
    '        " | приоритет: " &
    '        GetComputerMovePriority(move).ToString() &
    '        " | букв: " &
    '        move.TilesUsedCount.ToString() &
    '        " | слов: " &
    '        move.WordsCount.ToString() &
    '        " | крест: " &
    '        move.IsCrossMove.ToString() &
    '        Environment.NewLine

    '    Next

    '    Return text

    'End Function
    Private Function GetComputerMoveText(
    move As ComputerMove
) As String

        If move Is Nothing OrElse
           move.Placements Is Nothing Then

            Return ""

        End If

        Dim words As New List(Of String)

        For Each placement As ComputerPlacement In move.Placements

            If placement IsNot Nothing AndAlso
               placement.Word IsNot Nothing AndAlso
               placement.Word <> "" Then

                words.Add(placement.Word)

            End If

        Next

        Return String.Join(", ", words)

    End Function
    Private Sub ApplyRackBalance(
    player As Player)

        If player Is Nothing Then
            Exit Sub
        End If

        If bag Is Nothing Then
            Exit Sub
        End If

        Select Case rackBalanceMode

            Case RackBalanceMode.Chance

                ' Полный случай. Ничего не исправляем.
                Exit Sub

            Case RackBalanceMode.Classic

                EnsureMaxHardLetters(player, 3)
                EnsureMinVowels(player, 1)
                EnsureMinConsonants(player, 1)
                EnsureMaxHardLetters(player, 3)

            Case RackBalanceMode.Comfort

                EnsureMaxHardLetters(player, 1)
                EnsureMinVowels(player, 2)
                EnsureMinConsonants(player, 3)
                EnsureMaxHardLetters(player, 1)

        End Select

    End Sub
    Private Function IsRussianVowel(
    letter As Char
) As Boolean

        Return "АЕЁИОУЫЭЮЯ".Contains(letter)

    End Function


    Private Function IsHardLetter(
        letter As Char
    ) As Boolean

        Return "ЖЗХЧЫЬФЦШЩЪЭЮ".Contains(letter)

    End Function


    Private Function IsConsonantLetter(
        letter As Char
    ) As Boolean

        If letter = "*"c Then
            Return False
        End If

        If IsRussianVowel(letter) Then
            Return False
        End If

        Return "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ".Contains(letter)

    End Function


    Private Function CountRackVowels(
        player As Player
    ) As Integer

        Dim count As Integer = 0

        For Each tile As Tile In player.Rack

            If tile IsNot Nothing AndAlso
               IsRussianVowel(tile.Letter) Then

                count += 1

            End If

        Next

        Return count

    End Function


    Private Function CountRackConsonants(
        player As Player
    ) As Integer

        Dim count As Integer = 0

        For Each tile As Tile In player.Rack

            If tile IsNot Nothing AndAlso
               IsConsonantLetter(tile.Letter) Then

                count += 1

            End If

        Next

        Return count

    End Function


    Private Function CountRackHardLetters(
        player As Player
    ) As Integer

        Dim count As Integer = 0

        For Each tile As Tile In player.Rack

            If tile IsNot Nothing AndAlso
               IsHardLetter(tile.Letter) Then

                count += 1

            End If

        Next

        Return count

    End Function
    Private Sub EnsureMinVowels(
    player As Player,
    minCount As Integer)

        While CountRackVowels(player) < minCount

            Dim newTile As Tile =
                bag.DrawFromLetters("АЕИОУЯ")

            If newTile Is Nothing Then

                newTile =
                    bag.DrawFromLetters("АЕЁИОУЫЭЮЯ")

            End If

            If newTile Is Nothing Then
                Exit Sub
            End If


            Dim replaceIndex As Integer =
                FindReplaceIndexForVowel(player)

            If replaceIndex = -1 Then

                bag.ReturnTile(newTile)
                Exit Sub

            End If


            Dim oldTile As Tile =
                player.Rack(replaceIndex)

            player.Rack.RemoveAt(replaceIndex)
            bag.ReturnTile(oldTile)
            player.Rack.Add(newTile)

        End While

    End Sub


    Private Sub EnsureMinConsonants(
        player As Player,
        minCount As Integer)

        While CountRackConsonants(player) < minCount

            Dim newTile As Tile =
                bag.DrawFromLetters("НПРСТВКЛМДГБЙ")

            If newTile Is Nothing Then

                newTile =
                    bag.DrawFromLetters("БВГДЖЗЙКЛМНПРСТФХЦЧШЩ")

            End If

            If newTile Is Nothing Then
                Exit Sub
            End If


            Dim replaceIndex As Integer =
                FindReplaceIndexForConsonant(player)

            If replaceIndex = -1 Then

                bag.ReturnTile(newTile)
                Exit Sub

            End If


            Dim oldTile As Tile =
                player.Rack(replaceIndex)

            player.Rack.RemoveAt(replaceIndex)
            bag.ReturnTile(oldTile)
            player.Rack.Add(newTile)

        End While

    End Sub


    Private Sub EnsureMaxHardLetters(
        player As Player,
        maxCount As Integer)

        While CountRackHardLetters(player) > maxCount

            Dim newTile As Tile =
                bag.DrawFromLetters("АЕИОУЯНПРСТВКЛМДГБЙ")

            If newTile Is Nothing Then
                Exit Sub
            End If


            Dim replaceIndex As Integer =
                FindHardLetterIndex(player)

            If replaceIndex = -1 Then

                bag.ReturnTile(newTile)
                Exit Sub

            End If


            Dim oldTile As Tile =
                player.Rack(replaceIndex)

            player.Rack.RemoveAt(replaceIndex)
            bag.ReturnTile(oldTile)
            player.Rack.Add(newTile)

        End While

    End Sub
    Private Function FindReplaceIndexForVowel(
    player As Player
) As Integer

        ' Сначала меняем сложную согласную
        For i = 0 To player.Rack.Count - 1

            Dim tile As Tile =
                player.Rack(i)

            If tile IsNot Nothing AndAlso
               tile.Letter <> "*"c AndAlso
               Not IsRussianVowel(tile.Letter) AndAlso
               IsHardLetter(tile.Letter) Then

                Return i

            End If

        Next


        ' Потом любую согласную
        For i = 0 To player.Rack.Count - 1

            Dim tile As Tile =
                player.Rack(i)

            If tile IsNot Nothing AndAlso
               tile.Letter <> "*"c AndAlso
               Not IsRussianVowel(tile.Letter) Then

                Return i

            End If

        Next

        Return -1

    End Function


    Private Function FindReplaceIndexForConsonant(
        player As Player
    ) As Integer

        ' Сначала меняем сложную букву
        For i = 0 To player.Rack.Count - 1

            Dim tile As Tile =
                player.Rack(i)

            If tile IsNot Nothing AndAlso
               tile.Letter <> "*"c AndAlso
               IsHardLetter(tile.Letter) Then

                Return i

            End If

        Next


        ' Потом лишнюю гласную
        For i = 0 To player.Rack.Count - 1

            Dim tile As Tile =
                player.Rack(i)

            If tile IsNot Nothing AndAlso
               tile.Letter <> "*"c AndAlso
               IsRussianVowel(tile.Letter) Then

                Return i

            End If

        Next

        Return -1

    End Function


    Private Function FindHardLetterIndex(
        player As Player
    ) As Integer

        For i = 0 To player.Rack.Count - 1

            Dim tile As Tile =
                player.Rack(i)

            If tile IsNot Nothing AndAlso
               tile.Letter <> "*"c AndAlso
               IsHardLetter(tile.Letter) Then

                Return i

            End If

        Next

        Return -1

    End Function
    Private Function GetSettingsFolderPath() As String

        Return Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            "EruditClone")

    End Function


    Private Function GetLastGameSettingsPath() As String

        Return Path.Combine(
            GetSettingsFolderPath(),
            "last_game.txt")

    End Function


    Private Sub LoadLastGameSettings()

        Dim filePath As String =
            GetLastGameSettingsPath()

        If Not File.Exists(filePath) Then
            Exit Sub
        End If

        Try

            For Each line As String In File.ReadAllLines(
                filePath,
                Encoding.UTF8)

                Dim eqIndex As Integer =
                    line.IndexOf("="c)

                If eqIndex <= 0 Then
                    Continue For
                End If

                Dim key As String =
                    line.Substring(0, eqIndex).Trim()

                Dim value As String =
                    line.Substring(eqIndex + 1).Trim()

                Select Case key

                    Case "Player1Name"

                        If value <> "" Then
                            player1Name = value
                        End If

                    Case "Player2Name"

                        If value <> "" Then
                            player2Name = value
                        End If

                    Case "Player2IsComputer"

                        Boolean.TryParse(
                            value,
                            player2IsComputer)

                    Case "TargetScore"

                        Dim parsedScore As Integer

                        If Integer.TryParse(value, parsedScore) Then

                            If parsedScore > 0 Then
                                targetScore = parsedScore
                            End If

                        End If

                    Case "RackBalanceMode"

                        If [Enum].IsDefined(
                            GetType(RackBalanceMode),
                            value) Then

                            rackBalanceMode =
                                CType(
                                    [Enum].Parse(
                                        GetType(RackBalanceMode),
                                        value),
                                    RackBalanceMode)

                        End If

                End Select

            Next

        Catch

            ' Если файл настроек битый — просто запускаемся с дефолтами.

        End Try

    End Sub


    Private Sub SaveLastGameSettings()

        Try

            Dim folderPath As String =
                GetSettingsFolderPath()

            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If

            Dim filePath As String =
                GetLastGameSettingsPath()

            Dim lines As New List(Of String)

            lines.Add("Player1Name=" & player1Name)
            lines.Add("Player2Name=" & player2Name)
            lines.Add("Player2IsComputer=" & player2IsComputer.ToString())
            lines.Add("TargetScore=" & targetScore.ToString())
            lines.Add("RackBalanceMode=" & rackBalanceMode.ToString())

            File.WriteAllLines(
                filePath,
                lines,
                Encoding.UTF8)

        Catch

            ' Настройки не критичны. Если не сохранилось — игру не ломаем.

        End Try

    End Sub
    Private Sub Form1_FormClosing(
    sender As Object,
    e As FormClosingEventArgs) _
    Handles Me.FormClosing

        SaveLastGameSettings()

    End Sub
    Private Function HasLastGameSettings() As Boolean

        Return File.Exists(
            GetLastGameSettingsPath())

    End Function
    Private Function ShowNewGameDialog() As Boolean

        Using dlg As New NewGameDialog(
            player1Name,
            player2Name,
            targetScore,
            player2IsComputer)

            If dlg.ShowDialog(Me) <> DialogResult.OK Then
                Return False
            End If

            player1Name = dlg.Player1Name
            player2Name = dlg.Player2Name
            targetScore = dlg.TargetScore
            player2IsComputer = dlg.Player2IsComputer

        End Using

        SaveLastGameSettings()

        Return True

    End Function
End Class