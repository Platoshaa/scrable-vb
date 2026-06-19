Imports System

Public Class TileBag

    Private rnd As New Random()

    Private Tiles As New List(Of Tile)

    Public Sub New()

        AddLetters("А", 8, 1)
        AddLetters("О", 10, 1)
        AddLetters("Е", 8, 1)
        AddLetters("И", 5, 1)
        AddLetters("Н", 5, 1)
        AddLetters("Р", 5, 1)
        AddLetters("С", 5, 1)
        AddLetters("Т", 5, 1)

        AddLetters("В", 4, 2)
        AddLetters("Д", 4, 2)
        AddLetters("К", 4, 2)
        AddLetters("Л", 4, 2)
        AddLetters("М", 3, 2)
        AddLetters("П", 4, 2)
        AddLetters("У", 3, 2)

        AddLetters("Б", 2, 3)
        AddLetters("Г", 2, 3)
        AddLetters("Ь", 2, 3)
        AddLetters("Я", 2, 3)

        AddLetters("Й", 1, 4)
        AddLetters("Ы", 2, 4)

        AddLetters("Ж", 1, 5)
        AddLetters("З", 1, 5)
        AddLetters("Х", 1, 5)
        AddLetters("Ц", 1, 5)
        AddLetters("Ч", 1, 5)

        AddLetters("Ш", 1, 8)
        AddLetters("Э", 1, 8)
        AddLetters("Ю", 1, 8)

        AddLetters("Ф", 1, 10)
        AddLetters("Щ", 1, 10)
        AddLetters("Ъ", 1, 10)

        AddLetters("*", 2, 0)

    End Sub

    Private Sub AddLetters(letter As String,
                           count As Integer,
                           value As Integer)

        For i = 1 To count

            Tiles.Add(
                New Tile(letter(0), value))

        Next

    End Sub

    Public Function Draw() As Tile

        If Tiles.Count = 0 Then
            Return Nothing
        End If

        Dim index = rnd.Next(Tiles.Count)

        Dim t = Tiles(index)

        Tiles.RemoveAt(index)

        Return t

    End Function

End Class