Public Class ConnectivityValidator

    Public Shared Function HasConnection(
        board As Board,
        moveTiles As List(Of TileInstance)
    ) As Boolean

        For Each t In moveTiles

            Dim x = t.BoardX
            Dim y = t.BoardY

            If HasConfirmedTile(board, x - 1, y) Then Return True
            If HasConfirmedTile(board, x + 1, y) Then Return True
            If HasConfirmedTile(board, x, y - 1) Then Return True
            If HasConfirmedTile(board, x, y + 1) Then Return True

        Next

        Return False

    End Function

    Private Shared Function HasConfirmedTile(
        board As Board,
        x As Integer,
        y As Integer
    ) As Boolean

        If x < 0 OrElse x > 14 Then Return False
        If y < 0 OrElse y > 14 Then Return False

        Return Not board.GetCell(x, y).IsEmpty

    End Function

End Class