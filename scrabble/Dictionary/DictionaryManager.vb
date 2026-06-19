Imports System.IO
Imports System.Text

Public Class DictionaryManager

    Private Shared words As New HashSet(Of String)

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

        MessageBox.Show(
            "Словарь загружен: " &
            words.Count &
            " слов")

    End Sub


    Public Shared Function IsValidWord(
        word As String) As Boolean

        If word Is Nothing Then
            Return False
        End If

        Return words.Contains(
            word.Trim().ToUpper())

    End Function

End Class