Public Class SimpleAi

    Private Const MinWordLength As Integer = 3

    Private Shared ReadOnly JokerSubstituteLetters As String =
        "АЕИОУНТРСКЛМПВ"


    Public Shared Function BuildCandidateWords(
    rack As List(Of Tile),
    anchorLetters As String,
    maxLength As Integer,
    limit As Integer
) As List(Of String)

        Dim result As New List(Of String)

        If rack Is Nothing Then
            Return result
        End If


        Dim rackCounts As New Dictionary(Of Char, Integer)
        Dim jokerCount As Integer = 0

        For Each tile As Tile In rack

            If tile Is Nothing Then
                Continue For
            End If

            If tile.Letter = "*"c Then

                jokerCount += 1

            Else

                If Not rackCounts.ContainsKey(tile.Letter) Then
                    rackCounts(tile.Letter) = 0
                End If

                rackCounts(tile.Letter) += 1

            End If

        Next


        Dim anchors As New HashSet(Of Char)

        If anchorLetters IsNot Nothing Then

            For Each ch As Char In anchorLetters.ToUpper().Replace("Ё", "Е")

                If ch >= "А"c AndAlso ch <= "Я"c Then
                    anchors.Add(ch)
                End If

            Next

        End If


        Dim scanLimit As Integer =
        Math.Max(limit * 3, limit)

        For length As Integer = maxLength To 2 Step -1

            Dim words As List(Of String) =
            DictionaryManager.GetAiWordsOfLength(length)

            For Each word As String In words

                If word.Length > maxLength Then
                    Continue For
                End If

                If anchors.Count > 0 AndAlso
               Not WordContainsAnyAnchor(word, anchors) Then

                    Continue For

                End If

                If CanBuildWordThroughAnchor(
                word,
                rackCounts,
                jokerCount,
                anchors) Then

                    result.Add(word)

                    If result.Count >= scanLimit Then
                        Exit For
                    End If

                End If

            Next

            If result.Count >= scanLimit Then
                Exit For
            End If

        Next


        result.Sort(
        Function(a As String, b As String)

            Return GetCandidateWordPriority(b).
                CompareTo(GetCandidateWordPriority(a))

        End Function)


        If result.Count > limit Then
            result = result.GetRange(0, limit)
        End If

        Return result

    End Function
    Private Shared Sub GenerateForAnchor(
        rackLetters As List(Of Char),
        jokerCount As Integer,
        anchor As Char,
        maxLength As Integer,
        limit As Integer,
        resultSet As HashSet(Of String))

        Dim letters As New List(Of Char)

        For Each ch As Char In rackLetters
            letters.Add(ch)
        Next

        letters.Add(anchor)

        GenerateWords(
            "",
            letters,
            New List(Of Boolean),
            anchor,
            maxLength,
            limit,
            resultSet)


        ' Если есть джокер, пробуем подставить одну частую букву.
        ' Этого достаточно для быстрого простого ИИ.
        If jokerCount > 0 Then

            For Each jokerLetter As Char In JokerSubstituteLetters

                Dim lettersWithJoker As New List(Of Char)

                For Each ch As Char In rackLetters
                    lettersWithJoker.Add(ch)
                Next

                lettersWithJoker.Add(anchor)
                lettersWithJoker.Add(jokerLetter)

                GenerateWords(
                    "",
                    lettersWithJoker,
                    New List(Of Boolean),
                    anchor,
                    maxLength,
                    limit,
                    resultSet)

                If resultSet.Count >= limit Then
                    Exit For
                End If

            Next

        End If

    End Sub


    Private Shared Sub GenerateWords(
        prefix As String,
        letters As List(Of Char),
        used As List(Of Boolean),
        anchor As Char,
        maxLength As Integer,
        limit As Integer,
        resultSet As HashSet(Of String))

        If resultSet.Count >= limit Then
            Exit Sub
        End If

        If used.Count = 0 Then

            For i = 0 To letters.Count - 1
                used.Add(False)
            Next

        End If


        If prefix.Length >= MinWordLength Then

            If prefix.Contains(anchor.ToString()) Then

                If DictionaryManager.IsGoodAiWord(prefix) Then
                    resultSet.Add(prefix)
                End If

            End If

        End If


        If prefix.Length >= maxLength Then
            Exit Sub
        End If


        For i = 0 To letters.Count - 1

            If used(i) Then
                Continue For
            End If

            used(i) = True

            GenerateWords(
                prefix & letters(i),
                letters,
                used,
                anchor,
                maxLength,
                limit,
                resultSet)

            used(i) = False

            If resultSet.Count >= limit Then
                Exit For
            End If

        Next

    End Sub


    Private Shared Function EstimateWordScore(
        word As String
    ) As Integer

        Dim score As Integer = 0

        For Each ch As Char In word

            Select Case ch

                Case "А"c, "О"c, "Е"c, "И"c, "Н"c
                    score += 1

                Case "Р"c, "С"c, "Т"c,
                     "В"c, "Д"c, "К"c,
                     "Л"c, "М"c, "П"c
                    score += 2

                Case "У"c, "Я"c, "Г"c
                    score += 3

                Case "Й"c
                    score += 4

                Case "Ж"c, "З"c, "Х"c,
                     "Ч"c, "Ы"c, "Ь"c
                    score += 5

                Case "Ф"c, "Ц"c, "Ш"c,
                     "Щ"c, "Ъ"c, "Э"c, "Ю"c
                    score += 10

                Case Else
                    score += 1

            End Select

        Next

        Return score

    End Function
    Private Shared Function WordContainsAnyAnchor(
    word As String,
    anchors As HashSet(Of Char)
) As Boolean

        For Each ch As Char In word

            If anchors.Contains(ch) Then
                Return True
            End If

        Next

        Return False

    End Function


    Private Shared Function CanBuildWordThroughAnchor(
        word As String,
        rackCounts As Dictionary(Of Char, Integer),
        jokerCount As Integer,
        anchors As HashSet(Of Char)
    ) As Boolean

        If anchors Is Nothing OrElse anchors.Count = 0 Then
            Return CanBuildWordFromRackOnly(word, rackCounts, jokerCount)
        End If


        For anchorIndex = 0 To word.Length - 1

            Dim anchorLetter As Char =
                word(anchorIndex)

            If Not anchors.Contains(anchorLetter) Then
                Continue For
            End If


            Dim counts As New Dictionary(Of Char, Integer)(rackCounts)
            Dim jokersLeft As Integer = jokerCount
            Dim ok As Boolean = True

            For i = 0 To word.Length - 1

                If i = anchorIndex Then
                    Continue For
                End If

                Dim ch As Char =
                    word(i)

                If counts.ContainsKey(ch) AndAlso counts(ch) > 0 Then

                    counts(ch) -= 1

                ElseIf jokersLeft > 0 Then

                    jokersLeft -= 1

                Else

                    ok = False
                    Exit For

                End If

            Next

            If ok Then
                Return True
            End If

        Next

        Return False

    End Function


    Private Shared Function CanBuildWordFromRackOnly(
        word As String,
        rackCounts As Dictionary(Of Char, Integer),
        jokerCount As Integer
    ) As Boolean

        Dim counts As New Dictionary(Of Char, Integer)(rackCounts)
        Dim jokersLeft As Integer = jokerCount

        For Each ch As Char In word

            If counts.ContainsKey(ch) AndAlso counts(ch) > 0 Then

                counts(ch) -= 1

            ElseIf jokersLeft > 0 Then

                jokersLeft -= 1

            Else

                Return False

            End If

        Next

        Return True

    End Function


    Private Shared Function GetCandidateWordPriority(
        word As String
    ) As Integer

        If word Is Nothing Then
            Return 0
        End If

        Return word.Length * 100 + GetRawWordScore(word)

    End Function


    Private Shared Function GetRawWordScore(
        word As String
    ) As Integer

        Dim result As Integer = 0

        For Each ch As Char In word
            result += GetRussianLetterValue(ch)
        Next

        Return result

    End Function


    Private Shared Function GetRussianLetterValue(
        ch As Char
    ) As Integer

        Select Case ch

            Case "А"c, "О"c, "Е"c, "И"c, "Н"c
                Return 1

            Case "Р"c, "С"c, "Т"c, "В"c, "Д"c, "К"c, "Л"c, "М"c, "П"c
                Return 2

            Case "У"c, "Я"c, "Г"c
                Return 3

            Case "Й"c
                Return 4

            Case "Ж"c, "З"c, "Х"c, "Ч"c, "Ы"c, "Ь"c
                Return 5

            Case "Ф"c, "Ц"c, "Ш"c, "Щ"c, "Ъ"c, "Э"c, "Ю"c
                Return 10

        End Select

        Return 1

    End Function
End Class