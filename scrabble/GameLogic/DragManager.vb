Public Class DragManager

    Public Property DraggedTile As TileInstance

    Public Property OffsetX As Integer

    Public Property OffsetY As Integer

    Public ReadOnly Property IsDragging As Boolean

        Get
            Return DraggedTile IsNot Nothing
        End Get

    End Property

End Class