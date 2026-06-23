Public Class WordBuilder

    Public Shared Function BuildWords(
        tiles As List(Of TileInstance)
    ) As List(Of String)

        Dim result As New List(Of String)

        Dim wordTileLists =
            BuildWordTiles(tiles)

        For Each wordTiles As List(Of TileInstance) In wordTileLists

            Dim word As String = ""

            For Each t As TileInstance In wordTiles
                word &= t.VisibleLetter
            Next

            If word.Length > 1 Then
                result.Add(word)
            End If

        Next

        Return result

    End Function


    Public Shared Function BuildWordTiles(
        tiles As List(Of TileInstance)
    ) As List(Of List(Of TileInstance))

        Dim result As New List(Of List(Of TileInstance))

        For Each t As TileInstance In tiles

            If Not t.Confirmed AndAlso
               t.BoardX <> -1 AndAlso
               t.BoardY <> -1 Then

                Dim horizontalWord =
                    BuildLine(tiles, t.BoardX, t.BoardY, True)

                If horizontalWord.Count > 1 Then
                    AddWordIfNew(result, horizontalWord)
                End If

                Dim verticalWord =
                    BuildLine(tiles, t.BoardX, t.BoardY, False)

                If verticalWord.Count > 1 Then
                    AddWordIfNew(result, verticalWord)
                End If

            End If

        Next

        Return result

    End Function


    Private Shared Function BuildLine(
        tiles As List(Of TileInstance),
        startX As Integer,
        startY As Integer,
        horizontal As Boolean
    ) As List(Of TileInstance)

        Dim result As New List(Of TileInstance)

        Dim x As Integer = startX
        Dim y As Integer = startY

        If horizontal Then

            While TileAt(tiles, x - 1, y) IsNot Nothing
                x -= 1
            End While

            While TileAt(tiles, x, y) IsNot Nothing

                result.Add(
                    TileAt(tiles, x, y))

                x += 1

            End While

        Else

            While TileAt(tiles, x, y - 1) IsNot Nothing
                y -= 1
            End While

            While TileAt(tiles, x, y) IsNot Nothing

                result.Add(
                    TileAt(tiles, x, y))

                y += 1

            End While

        End If

        Return result

    End Function


    Private Shared Function TileAt(
        tiles As List(Of TileInstance),
        x As Integer,
        y As Integer
    ) As TileInstance

        For Each t As TileInstance In tiles

            If t.BoardX = x AndAlso
               t.BoardY = y Then

                Return t

            End If

        Next

        Return Nothing

    End Function


    Private Shared Sub AddWordIfNew(
        words As List(Of List(Of TileInstance)),
        candidate As List(Of TileInstance)
    )

        Dim candidateKey =
            GetWordKey(candidate)

        For Each existing As List(Of TileInstance) In words

            If GetWordKey(existing) = candidateKey Then
                Exit Sub
            End If

        Next

        words.Add(candidate)

    End Sub


    Private Shared Function GetWordKey(
        wordTiles As List(Of TileInstance)
    ) As String

        Dim key As String = ""

        For Each t As TileInstance In wordTiles

            key &= t.BoardX.ToString()
            key &= ","
            key &= t.BoardY.ToString()
            key &= ";"

        Next

        Return key

    End Function

End Class