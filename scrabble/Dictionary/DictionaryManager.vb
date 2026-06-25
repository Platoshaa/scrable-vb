Imports System.IO
Imports System.Text

Public Class DictionaryManager

    Private Shared words As New HashSet(Of String)
    Private Shared aiWords As New HashSet(Of String)
    Private Shared aiWordsByLength As New Dictionary(Of Integer, List(Of String))
    Private Shared aiBadWords As New HashSet(Of String)
    Public Shared Sub LoadDictionary(
        filePath As String)

        words.Clear()

        If Not File.Exists(filePath) Then

            MessageBox.Show(
                "Файл словаря не найден: " &
                filePath)

            Exit Sub

        End If

        ' windows-1251 для русского словаря
        Dim enc As Encoding =
            Encoding.GetEncoding(1251)

        For Each line As String In File.ReadLines(
            filePath,
            enc)

            Dim word As String =
                line.Trim().
                ToUpper()

            If word <> "" Then

                words.Add(word)

            End If

        Next

        LoadAiBadWords(
    Path.Combine(
        Application.StartupPath,
        "ai_bad_words.txt"))

    End Sub


    Public Shared Function IsValidWord(
    word As String) As Boolean

        word =
        NormalizeAiWord(word)

        If word = "" Then
            Return False
        End If

        If aiBadWords.Contains(word) Then
            Return False
        End If

        Return words.Contains(word)

    End Function
    Public Shared Function GetAllWords() As List(Of String)

        Dim result As New List(Of String)

        For Each w As String In words

            If w.Length >= 2 AndAlso
               w.Length <= 7 Then

                result.Add(w)

            End If

        Next

        Return result

    End Function
    Public Shared Sub LoadAiDictionary(
    filePath As String)

        aiWords.Clear()
        aiWordsByLength.Clear()
        If Not File.Exists(filePath) Then

            MessageBox.Show(
                "Файл словаря ИИ не найден: " &
                filePath)

            Exit Sub

        End If

        Dim enc As Encoding =
            Encoding.GetEncoding(1251)

        For Each line As String In File.ReadLines(
            filePath,
            enc)

            Dim word As String =
                line.Trim().
                ToUpper().
                Replace("Ё", "Е")

            If word <> "" Then

                If Not aiWords.Contains(word) Then

                    aiWords.Add(word)

                    If Not aiWordsByLength.ContainsKey(word.Length) Then
                        aiWordsByLength(word.Length) = New List(Of String)
                    End If

                    aiWordsByLength(word.Length).Add(word)

                End If

            End If
        Next
        LoadAiBadWords(
    Path.Combine(
        Application.StartupPath,
        "ai_bad_words.txt"))
    End Sub
    Public Shared Function IsGoodAiWord(
    word As String
) As Boolean

        word =
        NormalizeAiWord(word)

        If word = "" Then
            Return False
        End If

        If aiBadWords.Contains(word) Then
            Return False
        End If

        Return aiWords.Contains(word)

    End Function
    Public Shared Function GetAiWordsOfLength(
    length As Integer
) As List(Of String)

        If aiWordsByLength.ContainsKey(length) Then
            Return aiWordsByLength(length)
        End If

        Return New List(Of String)

    End Function
    Public Shared Sub LoadAiBadWords(
    filePath As String)

        aiBadWords.Clear()

        If Not File.Exists(filePath) Then
            Exit Sub
        End If

        Dim enc As Encoding =
        Encoding.GetEncoding(1251)

        For Each line As String In File.ReadLines(
        filePath,
        enc)

            Dim word As String =
            NormalizeAiWord(line)

            If word <> "" Then
                aiBadWords.Add(word)
            End If

        Next

    End Sub
    Private Shared Function NormalizeAiWord(
    word As String
) As String

        If word Is Nothing Then
            Return ""
        End If

        Return word.Trim().
            ToUpper().
            Replace("Ё", "Е")

    End Function
    Public Shared Sub AddAiBadWord(
    word As String)

        word =
            NormalizeAiWord(word)

        If word = "" Then
            Exit Sub
        End If

        If aiBadWords.Contains(word) Then
            Exit Sub
        End If

        aiBadWords.Add(word)

        Dim filePath As String =
            Path.Combine(
                Application.StartupPath,
                "ai_bad_words.txt")

        Dim enc As Encoding =
            Encoding.GetEncoding(1251)

        File.AppendAllText(
            filePath,
            word & Environment.NewLine,
            enc)

    End Sub
End Class