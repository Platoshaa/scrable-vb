Public Class Board

    Public Const Size As Integer = 15

    Private _cells(14, 14) As Cell

    Public Sub New()

        For x = 0 To 14

            For y = 0 To 14

                _cells(x, y) =
                New Cell()

            Next

        Next

        InitializeBonuses()

    End Sub

    Public Function GetCell(x As Integer, y As Integer) As Cell
        Return _cells(x, y)
    End Function
    Private Sub InitializeBonuses()

        _cells(7, 7).Bonus =
            BonusType.Center

        _cells(0, 0).Bonus =
            BonusType.TripleWord

        _cells(0, 14).Bonus =
            BonusType.TripleWord

        _cells(14, 0).Bonus =
            BonusType.TripleWord

        _cells(14, 14).Bonus =
            BonusType.TripleWord

        _cells(3, 3).Bonus =
            BonusType.DoubleWord

        _cells(11, 11).Bonus =
            BonusType.DoubleWord

        _cells(3, 11).Bonus =
            BonusType.DoubleWord

        _cells(11, 3).Bonus =
            BonusType.DoubleWord

    End Sub
End Class