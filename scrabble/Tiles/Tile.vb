Public Class Tile
    Public Property Letter As Char
    Public Property Value As Integer

    Public Sub New(letter As Char, value As Integer)
        Me.Letter = letter
        Me.Value = value
    End Sub
End Class