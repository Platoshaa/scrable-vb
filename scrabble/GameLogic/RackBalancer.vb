Public Class RackBalancer

    Private Shared ReadOnly Vowels As String =
        "АЕИОУЫЭЮЯ"

    Private Shared ReadOnly Consonants As String =
        "БВГДЖЗЙКЛМНПРСТФХЦЧШЩ"

    Private Shared ReadOnly HeavyLetters As String =
        "ЖЗХЧШЩФЦЪЭЮ"

    Private Shared ReadOnly SignLetters As String =
        "ЬЪ"

    Private Shared ReadOnly CommonLetters As String =
        "АНОЕТИСРКЛМПВ"


    Public Shared Sub Balance(
        player As Player,
        bag As TileBag,
        mode As RackBalanceMode)

        If player Is Nothing Then Exit Sub
        If bag Is Nothing Then Exit Sub

        Select Case mode


            Case RackBalanceMode.Comfort
                BalanceComfort(player, bag)

                'Case RackBalanceMode.VeryComfort
                '    BalanceVeryComfort(player, bag)

        End Select

    End Sub


    Private Shared Sub BalanceComfort(
        player As Player,
        bag As TileBag)

        ' Минимум 1 гласная
        If EffectiveVowelCount(player) < 1 Then

            ReplaceOneTile(
                player,
                bag,
                Vowels,
                Vowels,
                "")

        End If


        ' Минимум 1 согласная
        If EffectiveConsonantCount(player) < 1 Then

            ReplaceOneTile(
                player,
                bag,
                Consonants,
                Consonants,
                "")

        End If

    End Sub


    Private Shared Sub BalanceVeryComfort(
        player As Player,
        bag As TileBag)

        ' Не больше одного Ь / Ъ
        While CountLetters(player, SignLetters) > 1

            If Not ReplaceOneTile(
                player,
                bag,
                GetNeededLetters(player, 2, 3),
                "",
                SignLetters) Then

                Exit While

            End If

        End While


        ' Не больше двух тяжёлых букв
        While CountLetters(player, HeavyLetters) > 2

            If Not ReplaceOneTile(
                player,
                bag,
                GetNeededLetters(player, 2, 3),
                "",
                HeavyLetters) Then

                Exit While

            End If

        End While


        ' Минимум 2 гласные.
        ' ВАЖНО: сохраняем уже имеющиеся гласные,
        ' а заменяем что-то другое.
        While EffectiveVowelCount(player) < 2

            If Not ReplaceOneTile(
                player,
                bag,
                Vowels,
                Vowels,
                "") Then

                Exit While

            End If

        End While


        ' Минимум 3 согласные.
        ' ВАЖНО: сохраняем уже имеющиеся согласные.
        While EffectiveConsonantCount(player) < 3

            If Not ReplaceOneTile(
                player,
                bag,
                Consonants,
                Consonants,
                "") Then

                Exit While

            End If

        End While

    End Sub


    Private Shared Function GetNeededLetters(
        player As Player,
        minVowels As Integer,
        minConsonants As Integer
    ) As String

        If EffectiveVowelCount(player) < minVowels Then
            Return Vowels
        End If

        If EffectiveConsonantCount(player) < minConsonants Then
            Return Consonants
        End If

        Return CommonLetters

    End Function


    Private Shared Function EffectiveVowelCount(
        player As Player
    ) As Integer

        Return CountLetters(player, Vowels) +
            CountJokers(player)

    End Function


    Private Shared Function EffectiveConsonantCount(
        player As Player
    ) As Integer

        Return CountLetters(player, Consonants) +
            CountJokers(player)

    End Function


    Private Shared Function CountJokers(
        player As Player
    ) As Integer

        Dim count As Integer = 0

        For Each tile As Tile In player.Rack

            If tile.Letter = "*"c Then
                count += 1
            End If

        Next

        Return count

    End Function


    Private Shared Function CountLetters(
        player As Player,
        letters As String
    ) As Integer

        Dim count As Integer = 0

        For Each tile As Tile In player.Rack

            If letters.Contains(
                tile.Letter.ToString()) Then

                count += 1

            End If

        Next

        Return count

    End Function


    Private Shared Function ReplaceOneTile(
        player As Player,
        bag As TileBag,
        desiredLetters As String,
        preserveLetters As String,
        preferredReplaceLetters As String
    ) As Boolean

        Dim newTile As Tile =
            bag.DrawFromLetters(desiredLetters)

        If newTile Is Nothing Then
            Return False
        End If

        Dim replaceIndex As Integer =
            FindReplaceIndex(
                player,
                preserveLetters,
                preferredReplaceLetters)

        If replaceIndex = -1 Then

            bag.ReturnTile(newTile)
            Return False

        End If

        Dim oldTile As Tile =
            player.Rack(replaceIndex)

        player.Rack(replaceIndex) = newTile

        bag.ReturnTile(oldTile)

        Return True

    End Function


    Private Shared Function FindReplaceIndex(
        player As Player,
        preserveLetters As String,
        preferredReplaceLetters As String
    ) As Integer

        ' Сначала пытаемся заменить конкретно плохие буквы:
        ' тяжёлые, Ь, Ъ и т.д.
        If preferredReplaceLetters <> "" Then

            For i = 0 To player.Rack.Count - 1

                Dim letter As String =
                    player.Rack(i).Letter.ToString()

                If letter <> "*" AndAlso
                   preferredReplaceLetters.Contains(letter) AndAlso
                   Not preserveLetters.Contains(letter) Then

                    Return i

                End If

            Next

        End If


        ' Потом меняем любую букву, которую не нужно сохранять
        For i = 0 To player.Rack.Count - 1

            Dim letter As String =
                player.Rack(i).Letter.ToString()

            If letter <> "*" AndAlso
               Not preserveLetters.Contains(letter) Then

                Return i

            End If

        Next


        ' Крайний случай — меняем любую не-джокер букву
        For i = 0 To player.Rack.Count - 1

            If player.Rack(i).Letter <> "*"c Then
                Return i
            End If

        Next

        Return -1

    End Function

End Class