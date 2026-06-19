Imports System.Drawing
Imports System.Linq
Public Class Form1

    Private gameBoard As Board
    Private bag As TileBag
    Private players As New List(Of Player)

    Private currentPlayerIndex As Integer = 0

    Private currentPlayer As Player
    Private selectedTileIndex As Integer = -1


    Private Const CellSize As Integer = 32
    Private Const StartX As Integer = 20
    Private Const StartY As Integer = 20
    Private ReadOnly BoardColor As Color =
    Color.FromArgb(60, 130, 60)

    Private ReadOnly CenterColor As Color =
    Color.Gold
    Private dragManager As New DragManager()
    Private dragTile As TileInstance = Nothing
    Private dragOffsetX As Integer
    Private dragOffsetY As Integer
    Private tileInstances As New List(Of TileInstance)
    Private Sub Form1_Load(sender As Object,
                           e As EventArgs) _
                           Handles MyBase.Load

        Me.DoubleBuffered = True
        DictionaryManager.LoadDictionary(
    "russian.txt")
        gameBoard = New Board()

        bag = New TileBag()

        Dim p1 As New Player
        p1.Name = "Игрок 1"

        Dim p2 As New Player
        p2.Name = "Игрок 2"

        players.Add(p1)
        players.Add(p2)

        For Each p As Player In players

            For i = 1 To 7

                p.Rack.Add(
            bag.Draw())

            Next

        Next

        currentPlayer = players(0)
        Dim rackX As Integer = 32

        For Each t In currentPlayer.Rack

            tileInstances.Add(
    New TileInstance With {
        .Tile = t,
        .ScreenX = rackX,
        .ScreenY = 540,
        .HomeX = rackX,
        .HomeY = 540
    })

            rackX += 60

        Next
        Me.Width = 800
        Me.Height = 700
        UpdatePlayersInfo()


    End Sub

    Private Sub Form1_Paint(sender As Object,
                            e As PaintEventArgs) _
                            Handles Me.Paint

        Dim g = e.Graphics

        ' Поле
        For x = 0 To 14

            For y = 0 To 14

                Dim cell =
                    gameBoard.GetCell(x, y)

                Dim px =
                    StartX + x * CellSize

                Dim py =
                    StartY + y * CellSize

                Dim brush As Brush =
    New SolidBrush(BoardColor)

                If x = 7 AndAlso y = 7 Then
                    brush = New SolidBrush(CenterColor)
                End If

                g.FillRectangle(
                    brush,
                    px,
                    py,
                    CellSize,
                    CellSize)

                g.DrawRectangle(
                    Pens.Black,
                    px,
                    py,
                    CellSize,
                    CellSize)
                If x = 7 AndAlso y = 7 Then

                    g.DrawString(
        "★",
        New Font(
            "Arial",
            14,
            FontStyle.Bold),
        Brushes.Black,
        px + 5,
        py + 5)

                End If
                If Not cell.IsEmpty Then

                    g.DrawString(
                        cell.Tile.Letter.ToString(),
                        New Font(
                            "Arial",
                            14,
                            FontStyle.Bold),
                        Brushes.Yellow,
                        px + 8,
                        py + 6)

                End If

            Next

        Next

        ' Подставка игрока
        ' Все буквы (и на подставке и на поле)

        For Each t In tileInstances

            g.FillRectangle(
        Brushes.Beige,
        t.ScreenX,
        t.ScreenY,
        32,
        32)

            g.DrawRectangle(
        Pens.Black,
        t.ScreenX,
        t.ScreenY,
        32,
        32)

            g.DrawString(
        t.Tile.Letter.ToString(),
        New Font(
            "Arial",
            18,
            FontStyle.Bold),
        Brushes.Black,
        t.ScreenX,
        t.ScreenY)

            g.DrawString(
        t.Tile.Value.ToString(),
        New Font(
            "Arial",
            8,
            FontStyle.Regular),
        Brushes.Black,
        t.ScreenX + 20,
        t.ScreenY + 20)


        Next

    End Sub
    Private Sub Form1_MouseDown(
    sender As Object,
    e As MouseEventArgs) _
    Handles Me.MouseDown

        For i = tileInstances.Count - 1 To 0 Step -1

            Dim t = tileInstances(i)

            Dim r As New Rectangle(
            t.ScreenX,
            t.ScreenY,
            50,
            50)

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

        lblPlayers.Text = text

    End Sub
    Private Sub Form1_MouseUp(
    sender As Object,
    e As MouseEventArgs) _
    Handles Me.MouseUp

        If dragTile Is Nothing Then Exit Sub

        Dim boardX =
        (dragTile.ScreenX - StartX + 16) \ CellSize

        Dim boardY =
        (dragTile.ScreenY - StartY + 16) \ CellSize

        If boardX >= 0 AndAlso
       boardX <= 14 AndAlso
       boardY >= 0 AndAlso
       boardY <= 14 Then
            Dim occupied As Boolean = False

            For Each t In tileInstances

                If t IsNot dragTile Then

                    If t.BoardX = boardX AndAlso
           t.BoardY = boardY Then

                        occupied = True

                        Exit For

                    End If

                End If

            Next

            If occupied Then

                dragTile.ScreenX = dragTile.HomeX
                dragTile.ScreenY = dragTile.HomeY

                dragTile.BoardX = -1
                dragTile.BoardY = -1

                dragTile = Nothing

                Me.Invalidate()

                Exit Sub

            End If
            dragTile.BoardX = boardX
            dragTile.BoardY = boardY

            dragTile.ScreenX =
            StartX + boardX * CellSize

            dragTile.ScreenY =
            StartY + boardY * CellSize

        Else

            dragTile.ScreenX = dragTile.HomeX
            dragTile.ScreenY = dragTile.HomeY

            dragTile.BoardX = -1
            dragTile.BoardY = -1

        End If

        dragTile = Nothing

        Me.Invalidate()

    End Sub
    Private Sub btnCancelMove_Click(
    sender As Object,
    e As EventArgs) _
    Handles btnCancelMove.Click

        For Each t In tileInstances

            If Not t.Confirmed Then

                t.BoardX = -1
                t.BoardY = -1

                t.ScreenX = t.HomeX
                t.ScreenY = t.HomeY

            End If

        Next

        Me.Invalidate()

    End Sub
    Private Sub Button1_Click(
    sender As Object,
    e As EventArgs) _
    Handles btnConfirmMove.Click
        If tileInstances.All(
    Function(t)
        Return Not t.Confirmed
    End Function) Then

            If Not MoveValidator.
        FirstMoveUsesCenter(
            tileInstances) Then

                MessageBox.Show(
            "Первый ход должен проходить через центр")

                Exit Sub

            End If

        End If
        If Not MoveValidator.ValidateMove(
        tileInstances) Then

            MessageBox.Show(
            "Буквы должны быть одной линией")

            Exit Sub

        End If

        Dim word =
        WordBuilder.BuildWord(
            tileInstances)

        If Not DictionaryManager.
        IsValidWord(word) Then

            MessageBox.Show(
            "Слова нет в словаре: " &
            word)

            Exit Sub

        End If
        Dim score =
ScoreManager.CalculateScore(
    tileInstances,
    gameBoard)

        For Each t In tileInstances

            If Not t.Confirmed AndAlso
       t.BoardX <> -1 Then

                gameBoard.GetCell(
            t.BoardX,
            t.BoardY).Tile = t.Tile

                t.Confirmed = True

            End If

        Next

        currentPlayer.Score += score
        UpdatePlayersInfo()
        lblScore.Text =
"Очки: " &
currentPlayer.Score.ToString()

        MessageBox.Show(
    "Принято: " & word)

        NextPlayer()
    End Sub

    Private Sub NextPlayer()

        ' Удаляем использованные буквы
        Dim removeTiles As New List(Of Tile)

        For Each t As TileInstance In tileInstances

            If t.Confirmed AndAlso
           t.BoardX <> -1 AndAlso
           t.HomeY = 540 Then

                removeTiles.Add(t.Tile)

            End If

        Next

        For Each tile As Tile In removeTiles

            currentPlayer.Rack.Remove(tile)

        Next


        ' Добираем новые буквы текущему игроку
        While currentPlayer.Rack.Count < 7

            Dim newTile As Tile =
            bag.Draw()

            If newTile Is Nothing Then Exit While

            currentPlayer.Rack.Add(newTile)

        End While


        ' Сохраняем все буквы поля
        Dim boardTiles As New List(Of TileInstance)

        For Each t As TileInstance In tileInstances

            If t.BoardX <> -1 Then

                boardTiles.Add(
                New TileInstance With {
                    .Tile = t.Tile,
                    .ScreenX = t.ScreenX,
                    .ScreenY = t.ScreenY,
                    .BoardX = t.BoardX,
                    .BoardY = t.BoardY,
                    .Confirmed = True
                })

            End If

        Next


        ' Переключаем игрока
        currentPlayerIndex += 1

        If currentPlayerIndex >= players.Count Then
            currentPlayerIndex = 0
        End If

        currentPlayer =
        players(currentPlayerIndex)


        ' Полностью очищаем экран
        tileInstances.Clear()


        ' Возвращаем буквы поля
        For Each t In boardTiles

            tileInstances.Add(t)

        Next


        ' Добавляем буквы нового игрока
        Dim rackX As Integer = 40

        For Each tile As Tile In currentPlayer.Rack

            tileInstances.Add(
            New TileInstance With {
                .Tile = tile,
                .ScreenX = rackX,
                .ScreenY = 540,
                .HomeX = rackX,
                .HomeY = 540,
                .BoardX = -1,
                .BoardY = -1,
                .Confirmed = False
            })

            rackX += 60

        Next


        UpdatePlayersInfo()

        Me.Text =
        "Эрудит - " &
        currentPlayer.Name &
        " | Очки: " &
        currentPlayer.Score

        Me.Invalidate()

    End Sub
End Class