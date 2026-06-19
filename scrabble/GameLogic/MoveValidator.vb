Public Class MoveValidator

    Public Shared Function ValidateMove(
    tiles As List(Of TileInstance)
    ) As Boolean

        Dim moveTiles =
        tiles.Where(
            Function(t)
                Return Not t.Confirmed AndAlso
                       t.BoardX <> -1
            End Function).ToList()

        If moveTiles.Count <= 1 Then
            Return True
        End If


        Dim sameRow =
        moveTiles.All(
            Function(t)
                Return t.BoardY =
                       moveTiles(0).BoardY
            End Function)

        Dim sameColumn =
        moveTiles.All(
            Function(t)
                Return t.BoardX =
                       moveTiles(0).BoardX
            End Function)

        If Not sameRow AndAlso
       Not sameColumn Then

            Return False

        End If


        ' Проверка отсутствия разрывов
        If sameRow Then

            Dim y = moveTiles(0).BoardY

            Dim minX =
            moveTiles.Min(
                Function(t) t.BoardX)

            Dim maxX =
            moveTiles.Max(
                Function(t) t.BoardX)

            For x = minX To maxX

                If Not tiles.Any(
                Function(t)
                    Return t.BoardX = x AndAlso
                           t.BoardY = y
                End Function) Then

                    Return False

                End If

            Next

        Else

            Dim x = moveTiles(0).BoardX

            Dim minY =
            moveTiles.Min(
                Function(t) t.BoardY)

            Dim maxY =
            moveTiles.Max(
                Function(t) t.BoardY)

            For y = minY To maxY

                If Not tiles.Any(
                Function(t)
                    Return t.BoardX = x AndAlso
                           t.BoardY = y
                End Function) Then

                    Return False

                End If

            Next

        End If

        Return True

    End Function
    Public Shared Function FirstMoveUsesCenter(
    tiles As List(Of TileInstance)
    ) As Boolean

        For Each t In tiles

            If Not t.Confirmed Then

                If t.BoardX = 7 AndAlso
                   t.BoardY = 7 Then

                    Return True

                End If

            End If

        Next

        Return False

    End Function
End Class