Public Class Board

    Public Const Size As Integer = 15

    Private _cells(14, 14) As Cell

    Public Sub New()

        For x = 0 To 14
            For y = 0 To 14
                _cells(x, y) = New Cell()
            Next
        Next

        InitializeBonuses()

    End Sub


    Public Function GetCell(
        x As Integer,
        y As Integer
    ) As Cell

        Return _cells(x, y)

    End Function


    Private Sub InitializeBonuses()

        ' Центр
        SetBonus(BonusType.Center,
                 New Point(7, 7))

        ' Розовые клетки — слово x3
        SetBonus(BonusType.TripleWord,
                 New Point(0, 0),
                 New Point(7, 0),
                 New Point(14, 0),
                 New Point(0, 7),
                 New Point(14, 7),
                 New Point(0, 14),
                 New Point(7, 14),
                 New Point(14, 14))

        ' Синие клетки — буква x3
        SetBonus(BonusType.TripleLetter,
                 New Point(1, 1),
                 New Point(2, 2),
                 New Point(3, 3),
                 New Point(4, 4),
                 New Point(13, 1),
                 New Point(12, 2),
                 New Point(11, 3),
                 New Point(10, 4),
                 New Point(4, 10),
                 New Point(3, 11),
                 New Point(2, 12),
                 New Point(1, 13),
                 New Point(10, 10),
                 New Point(11, 11),
                 New Point(12, 12),
                 New Point(13, 13))

        ' Жёлтые клетки — слово x2
        SetBonus(BonusType.DoubleWord,
                 New Point(5, 1),
                 New Point(9, 1),
                 New Point(1, 5),
                 New Point(5, 5),
                 New Point(9, 5),
                 New Point(13, 5),
                 New Point(1, 9),
                 New Point(5, 9),
                 New Point(9, 9),
                 New Point(13, 9),
                 New Point(5, 13),
                 New Point(9, 13))

        ' Ярко-зелёные клетки — буква x2
        SetBonus(BonusType.DoubleLetter,
                 New Point(3, 0),
                 New Point(11, 0),
                 New Point(6, 2),
                 New Point(8, 2),
                 New Point(0, 3),
                 New Point(7, 3),
                 New Point(14, 3),
                 New Point(2, 6),
                 New Point(6, 6),
                 New Point(8, 6),
                 New Point(12, 6),
                 New Point(3, 7),
                 New Point(11, 7),
                 New Point(2, 8),
                 New Point(6, 8),
                 New Point(8, 8),
                 New Point(12, 8),
                 New Point(0, 11),
                 New Point(7, 11),
                 New Point(14, 11),
                 New Point(6, 12),
                 New Point(8, 12),
                 New Point(3, 14),
                 New Point(11, 14))

    End Sub


    Private Sub SetBonus(
        bonus As BonusType,
        ParamArray cells() As Point
    )

        For Each p As Point In cells
            _cells(p.X, p.Y).Bonus = bonus
        Next

    End Sub

End Class