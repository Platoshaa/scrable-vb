Public Class BoardWordBuilder

    Public Shared Function BuildHorizontalWord(
        board As Board,
        x As Integer,
        y As Integer) As String

        Dim startX As Integer = x

        While startX > 0 AndAlso
              Not board.GetCell(startX - 1, y).IsEmpty

            startX -= 1

        End While

        Dim word As String = ""

        While startX < 15 AndAlso
              Not board.GetCell(startX, y).IsEmpty

            word &= board.GetCell(
                startX,
                y).Tile.Letter

            startX += 1

        End While

        Return word

    End Function

End Class