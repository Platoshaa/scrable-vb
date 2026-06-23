Public Class MoveValidator

    Public Shared Function ValidateMove(
        tiles As List(Of TileInstance)
    ) As Boolean

        Dim newTiles As New List(Of TileInstance)

        For Each t As TileInstance In tiles

            If Not t.Confirmed AndAlso
               t.BoardX <> -1 AndAlso
               t.BoardY <> -1 Then

                newTiles.Add(t)

            End If

        Next

        If newTiles.Count = 0 Then
            Return False
        End If

        Dim words =
            WordBuilder.BuildWordTiles(tiles)

        If words.Count = 0 Then
            Return False
        End If

        ' Каждая новая буква должна входить хотя бы в одно слово
        For Each newTile As TileInstance In newTiles

            Dim used As Boolean = False

            For Each wordTiles As List(Of TileInstance) In words

                For Each wt As TileInstance In wordTiles

                    If wt Is newTile Then
                        used = True
                        Exit For
                    End If

                Next

                If used Then Exit For

            Next

            If Not used Then
                Return False
            End If

        Next

        Return True

    End Function


    Public Shared Function FirstMoveUsesCenter(
        tiles As List(Of TileInstance)
    ) As Boolean

        For Each t As TileInstance In tiles

            If Not t.Confirmed AndAlso
               t.BoardX = 7 AndAlso
               t.BoardY = 7 Then

                Return True

            End If

        Next

        Return False

    End Function

End Class