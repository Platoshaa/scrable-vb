Imports System.Linq


Public Class WordBuilder

    Public Shared Function BuildWord(
        tiles As List(Of TileInstance)
        ) As String

        Dim moveTiles =
            tiles.Where(
                Function(t)
                    Return Not t.Confirmed AndAlso
                           t.BoardX <> -1
                End Function).ToList()

        If moveTiles.Count = 0 Then
            Return ""
        End If


        Dim horizontal As Boolean

        If moveTiles.Count > 1 Then

            horizontal =
                moveTiles.All(
                    Function(t)
                        Return t.BoardY =
                               moveTiles(0).BoardY
                    End Function)

        Else

            ' Для одной буквы смотрим соседей
            Dim t = moveTiles(0)

            Dim hasLeftRight =
                tiles.Any(
                    Function(x)
                        Return x.BoardY = t.BoardY AndAlso
                        (x.BoardX = t.BoardX - 1 OrElse
                         x.BoardX = t.BoardX + 1)
                    End Function)

            horizontal = hasLeftRight

        End If


        Dim result As String = ""

        If horizontal Then

            Dim y = moveTiles(0).BoardY
            Dim x = moveTiles(0).BoardX

            ' Идем влево
            While tiles.Any(
                Function(t)
                    Return t.BoardX = x - 1 AndAlso
                           t.BoardY = y
                End Function)

                x -= 1

            End While

            ' Идем вправо
            While True

                Dim tile =
                    tiles.FirstOrDefault(
                        Function(t)
                            Return t.BoardX = x AndAlso
                                   t.BoardY = y
                        End Function)

                If tile Is Nothing Then Exit While

                result &= tile.Tile.Letter

                x += 1

            End While

        Else

            Dim x = moveTiles(0).BoardX
            Dim y = moveTiles(0).BoardY

            ' Идем вверх
            While tiles.Any(
                Function(t)
                    Return t.BoardX = x AndAlso
                           t.BoardY = y - 1
                End Function)

                y -= 1

            End While

            ' Идем вниз
            While True

                Dim tile =
                    tiles.FirstOrDefault(
                        Function(t)
                            Return t.BoardX = x AndAlso
                                   t.BoardY = y
                        End Function)

                If tile Is Nothing Then Exit While

                result &= tile.Tile.Letter

                y += 1

            End While

        End If

        Return result

    End Function

End Class