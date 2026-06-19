Public Class TurnManager

    Public Shared Function GetNewTiles(
        tiles As List(Of TileInstance)
        ) As List(Of TileInstance)

        Return tiles.
            Where(
                Function(t)
                    Return Not t.Confirmed AndAlso
                           t.BoardX <> -1
                End Function).
            ToList()

    End Function

End Class