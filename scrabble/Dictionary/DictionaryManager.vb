Imports System.IO
Imports System.Text

Public Class DictionaryManager

    Private Shared words As New HashSet(Of String)
    Private Shared aiWords As New HashSet(Of String)
    Private Shared aiWordsByLength As New Dictionary(Of Integer, List(Of String))
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



    End Sub


    Public Shared Function IsValidWord(
        word As String) As Boolean

        If word Is Nothing Then
            Return False
        End If

        Return words.Contains(
            word.Trim().ToUpper())

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

    End Sub
    Public Shared Function IsGoodAiWord(
    word As String
) As Boolean

        If word Is Nothing Then
            Return False
        End If

        word =
            word.Trim().
            ToUpper().
            Replace("Ё", "Е")

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
End Class