Public Class ScoreManager

    Public Shared Function CalculateScore(
        tiles As List(Of TileInstance),
        board As Board) As Integer

        Dim total As Integer = 0

        Dim wordMultiplier As Integer = 1

        For Each t In tiles

            If t.BoardX = -1 Then Continue For

            If t.Confirmed Then Continue For

            Dim value =
                t.Tile.Value

            Dim cell =
                board.GetCell(
                    t.BoardX,
                    t.BoardY)

            Select Case cell.Bonus

                Case BonusType.DoubleLetter
                    value *= 2

                Case BonusType.TripleLetter
                    value *= 3

                Case BonusType.DoubleWord
                    wordMultiplier *= 2

                Case BonusType.TripleWord
                    wordMultiplier *= 3

            End Select

            total += value

        Next

        Return total * wordMultiplier

    End Function

End Class