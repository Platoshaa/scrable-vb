Public Class ScoreManager

    Public Shared Function CalculateWordsScore(
        tiles As List(Of TileInstance),
        board As Board
    ) As Integer

        Dim total As Integer = 0

        Dim words =
            WordBuilder.BuildWordTiles(tiles)

        For Each wordTiles As List(Of TileInstance) In words

            total += CalculateWordScore(
                wordTiles,
                board)

        Next

        Return total

    End Function


    Public Shared Function CalculateWordScore(
        wordTiles As List(Of TileInstance),
        board As Board
    ) As Integer

        Dim wordScore As Integer = 0
        Dim wordMultiplier As Integer = 1

        For Each t As TileInstance In wordTiles

            Dim letterScore As Integer =
                t.Tile.Value

            If Not t.Confirmed Then

                Dim cell =
                    board.GetCell(
                        t.BoardX,
                        t.BoardY)

                Select Case cell.Bonus

                    Case BonusType.DoubleLetter
                        letterScore *= 2

                    Case BonusType.TripleLetter
                        letterScore *= 3

                    Case BonusType.DoubleWord
                        wordMultiplier *= 2

                    Case BonusType.TripleWord
                        wordMultiplier *= 3

                    Case BonusType.Center
                        wordMultiplier *= 1

                End Select

            End If

            wordScore += letterScore

        Next

        Return wordScore * wordMultiplier

    End Function

End Class