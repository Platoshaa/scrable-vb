Imports System

Public Class TileBag

    Private rnd As New Random()
    Private Tiles As New List(Of Tile)


    Public Sub New()

        AddLetters("А", 10, 1)
        AddLetters("О", 10, 1)
        AddLetters("Е", 9, 1)
        AddLetters("И", 8, 1)
        AddLetters("Н", 8, 1)
        AddLetters("Р", 6, 2)
        AddLetters("С", 6, 2)
        AddLetters("Т", 5, 2)

        AddLetters("В", 5, 2)
        AddLetters("Д", 5, 2)
        AddLetters("К", 6, 2)
        AddLetters("Л", 4, 2)
        AddLetters("М", 5, 2)
        AddLetters("П", 6, 2)

        AddLetters("У", 3, 3)
        AddLetters("Я", 2, 3)
        AddLetters("Г", 3, 3)

        AddLetters("Й", 4, 4)

        AddLetters("Ж", 2, 5)
        AddLetters("З", 2, 5)
        AddLetters("Х", 2, 5)
        AddLetters("Ч", 2, 5)
        AddLetters("Ы", 2, 5)
        AddLetters("Ь", 2, 5)

        AddLetters("Ф", 1, 10)
        AddLetters("Ц", 1, 10)
        AddLetters("Ш", 1, 10)
        AddLetters("Щ", 1, 10)
        AddLetters("Ъ", 1, 10)
        AddLetters("Э", 1, 10)
        AddLetters("Ю", 1, 10)

        ' Джокер пока не добавляю.
        ' Его лучше включить только когда сделаем выбор буквы.
        AddLetters("*", 2, 0)

    End Sub


    Public ReadOnly Property Count As Integer
        Get
            Return Tiles.Count
        End Get
    End Property


    Private Sub AddLetters(
        letter As String,
        count As Integer,
        value As Integer)

        For i = 1 To count
            Tiles.Add(
                New Tile(
                    letter(0),
                    value))
        Next

    End Sub


    Public Function Draw() As Tile

        If Tiles.Count = 0 Then
            Return Nothing
        End If

        Dim index =
            rnd.Next(Tiles.Count)

        Dim t =
            Tiles(index)

        Tiles.RemoveAt(index)

        Return t

    End Function


    Public Sub ReturnTile(tile As Tile)

        If tile Is Nothing Then
            Exit Sub
        End If

        Tiles.Add(tile)

    End Sub
    Public Function DrawFromLetters(
    allowedLetters As String
) As Tile

        Dim candidates As New List(Of Integer)

        For i = 0 To Tiles.Count - 1

            Dim letter As String =
                Tiles(i).Letter.ToString()

            If allowedLetters.Contains(letter) Then
                candidates.Add(i)
            End If

        Next

        If candidates.Count = 0 Then
            Return Nothing
        End If

        Dim chosenCandidateIndex As Integer =
            rnd.Next(candidates.Count)

        Dim tileIndex As Integer =
            candidates(chosenCandidateIndex)

        Dim result As Tile =
            Tiles(tileIndex)

        Tiles.RemoveAt(tileIndex)

        Return result

    End Function
End Class