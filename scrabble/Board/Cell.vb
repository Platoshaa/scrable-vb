Public Class Cell

    Public Property Tile As Tile

    Public Property Bonus As BonusType =
        BonusType.None

    Public ReadOnly Property IsEmpty As Boolean

        Get
            Return Tile Is Nothing
        End Get

    End Property

End Class