Public Class TileInstance

    Public Property Tile As Tile

    Public Property ScreenX As Integer
    Public Property ScreenY As Integer

    Public Property HomeX As Integer
    Public Property HomeY As Integer

    Public Property BoardX As Integer = -1
    Public Property BoardY As Integer = -1

    Public Property Confirmed As Boolean = False

    ' Для джокера.
    ' Если Tile.Letter = "*" и JokerLetter = "А",
    ' то на поле показываем А, но цена остаётся 0.
    Public Property JokerLetter As String = ""

    Public ReadOnly Property IsJoker As Boolean
        Get
            Return Tile IsNot Nothing AndAlso Tile.Letter = "*"c
        End Get
    End Property

    Public ReadOnly Property VisibleLetter As String
        Get
            If IsJoker AndAlso JokerLetter <> "" Then
                Return JokerLetter
            End If

            If Tile Is Nothing Then
                Return ""
            End If

            Return Tile.Letter.ToString()
        End Get
    End Property

End Class