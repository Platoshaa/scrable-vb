Imports System.IO
Imports System.Xml.Linq

Public Class StatsManager

    Private Shared ReadOnly StatsFilePath As String =
        Path.Combine(
            Application.StartupPath,
            "stats.xml")


    Public Shared Sub RecordGame(
        player1Name As String,
        player1Score As Integer,
        player2Name As String,
        player2Score As Integer,
        winnerName As String,
        bestWord As String,
        bestWordScore As Integer,
        bestWordPlayer As String,
        bestMoveScore As Integer,
        bestMovePlayer As String)

        Dim doc As XDocument =
            LoadStatsDocument()

        Dim root As XElement =
            doc.Root

        Dim totalGames As Integer =
            GetIntValue(
                root.Element("TotalGames"),
                0)

        root.Element("TotalGames").Value =
            (totalGames + 1).ToString()


        UpdatePlayerStats(
            root,
            player1Name,
            player1Score,
            player1Name = winnerName)

        UpdatePlayerStats(
            root,
            player2Name,
            player2Score,
            player2Name = winnerName)


        Dim bestWordElement As XElement =
            root.Element("BestWord")

        Dim oldBestWordScore As Integer =
            GetIntAttribute(
                bestWordElement,
                "Score",
                0)

        If bestWordScore > oldBestWordScore Then

            bestWordElement.SetAttributeValue(
                "Text",
                bestWord)

            bestWordElement.SetAttributeValue(
                "Score",
                bestWordScore.ToString())

            bestWordElement.SetAttributeValue(
                "Player",
                bestWordPlayer)

        End If


        Dim bestMoveElement As XElement =
            root.Element("BestMove")

        Dim oldBestMoveScore As Integer =
            GetIntAttribute(
                bestMoveElement,
                "Score",
                0)

        If bestMoveScore > oldBestMoveScore Then

            bestMoveElement.SetAttributeValue(
                "Score",
                bestMoveScore.ToString())

            bestMoveElement.SetAttributeValue(
                "Player",
                bestMovePlayer)

        End If


        doc.Save(StatsFilePath)

    End Sub


    Public Shared Function GetStatsText() As String

        Dim doc As XDocument =
            LoadStatsDocument()

        Dim root As XElement =
            doc.Root

        Dim text As String = ""

        text &= "Общая статистика" &
            Environment.NewLine &
            Environment.NewLine

        text &= "Всего партий: " &
            root.Element("TotalGames").Value &
            Environment.NewLine &
            Environment.NewLine


        Dim bestWord As XElement =
            root.Element("BestWord")

        text &= "Лучшее слово: "

        If GetIntAttribute(bestWord, "Score", 0) > 0 Then

            text &= bestWord.Attribute("Text").Value &
                " — " &
                bestWord.Attribute("Score").Value &
                " очков" &
                " (" &
                bestWord.Attribute("Player").Value &
                ")"

        Else

            text &= "-"

        End If

        text &= Environment.NewLine


        Dim bestMove As XElement =
            root.Element("BestMove")

        text &= "Самый дорогой ход: "

        If GetIntAttribute(bestMove, "Score", 0) > 0 Then

            text &= bestMove.Attribute("Score").Value &
                " очков" &
                " (" &
                bestMove.Attribute("Player").Value &
                ")"

        Else

            text &= "-"

        End If

        text &= Environment.NewLine &
            Environment.NewLine


        text &= "Игроки:" &
            Environment.NewLine

        Dim playersElement As XElement =
            root.Element("Players")

        For Each playerElement As XElement In playersElement.Elements("Player")

            Dim name As String =
                playerElement.Attribute("Name").Value

            Dim games As Integer =
                GetIntAttribute(
                    playerElement,
                    "Games",
                    0)

            Dim wins As Integer =
                GetIntAttribute(
                    playerElement,
                    "Wins",
                    0)

            Dim totalScore As Integer =
                GetIntAttribute(
                    playerElement,
                    "TotalScore",
                    0)

            Dim bestScore As Integer =
                GetIntAttribute(
                    playerElement,
                    "BestScore",
                    0)

            Dim averageScore As Integer = 0

            If games > 0 Then
                averageScore = totalScore \ games
            End If

            text &= Environment.NewLine &
                name &
                Environment.NewLine &
                "  Игры: " &
                games.ToString() &
                Environment.NewLine &
                "  Победы: " &
                wins.ToString() &
                Environment.NewLine &
                "  Средний счёт: " &
                averageScore.ToString() &
                Environment.NewLine &
                "  Лучший счёт: " &
                bestScore.ToString() &
                Environment.NewLine

        Next

        Return text

    End Function


    Private Shared Function LoadStatsDocument() As XDocument

        If Not File.Exists(StatsFilePath) Then

            Dim newDoc As New XDocument(
                New XElement(
                    "Stats",
                    New XElement("TotalGames", "0"),
                    New XElement(
                        "BestWord",
                        New XAttribute("Text", ""),
                        New XAttribute("Score", "0"),
                        New XAttribute("Player", "")),
                    New XElement(
                        "BestMove",
                        New XAttribute("Score", "0"),
                        New XAttribute("Player", "")),
                    New XElement("Players")))

            newDoc.Save(StatsFilePath)

            Return newDoc

        End If

        Return XDocument.Load(StatsFilePath)

    End Function


    Private Shared Sub UpdatePlayerStats(
        root As XElement,
        playerName As String,
        score As Integer,
        won As Boolean)

        Dim playersElement As XElement =
            root.Element("Players")

        Dim playerElement As XElement = Nothing

        For Each p As XElement In playersElement.Elements("Player")

            If p.Attribute("Name").Value = playerName Then

                playerElement = p
                Exit For

            End If

        Next


        If playerElement Is Nothing Then

            playerElement =
                New XElement(
                    "Player",
                    New XAttribute("Name", playerName),
                    New XAttribute("Games", "0"),
                    New XAttribute("Wins", "0"),
                    New XAttribute("TotalScore", "0"),
                    New XAttribute("BestScore", "0"))

            playersElement.Add(playerElement)

        End If


        Dim games As Integer =
            GetIntAttribute(
                playerElement,
                "Games",
                0)

        Dim wins As Integer =
            GetIntAttribute(
                playerElement,
                "Wins",
                0)

        Dim totalScore As Integer =
            GetIntAttribute(
                playerElement,
                "TotalScore",
                0)

        Dim bestScore As Integer =
            GetIntAttribute(
                playerElement,
                "BestScore",
                0)


        games += 1
        totalScore += score

        If won Then
            wins += 1
        End If

        If score > bestScore Then
            bestScore = score
        End If


        playerElement.SetAttributeValue(
            "Games",
            games.ToString())

        playerElement.SetAttributeValue(
            "Wins",
            wins.ToString())

        playerElement.SetAttributeValue(
            "TotalScore",
            totalScore.ToString())

        playerElement.SetAttributeValue(
            "BestScore",
            bestScore.ToString())

    End Sub


    Private Shared Function GetIntValue(
        element As XElement,
        defaultValue As Integer
    ) As Integer

        If element Is Nothing Then
            Return defaultValue
        End If

        Dim value As Integer

        If Integer.TryParse(
            element.Value,
            value) Then

            Return value

        End If

        Return defaultValue

    End Function


    Private Shared Function GetIntAttribute(
        element As XElement,
        attributeName As String,
        defaultValue As Integer
    ) As Integer

        If element Is Nothing Then
            Return defaultValue
        End If

        Dim attr As XAttribute =
            element.Attribute(attributeName)

        If attr Is Nothing Then
            Return defaultValue
        End If

        Dim value As Integer

        If Integer.TryParse(
            attr.Value,
            value) Then

            Return value

        End If

        Return defaultValue

    End Function

End Class