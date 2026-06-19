<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnConfirmMove = New System.Windows.Forms.Button()
        Me.btnCancelMove = New System.Windows.Forms.Button()
        Me.lblScore = New System.Windows.Forms.Label()
        Me.lblPlayers = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnConfirmMove
        '
        Me.btnConfirmMove.Location = New System.Drawing.Point(520, 540)
        Me.btnConfirmMove.Name = "btnConfirmMove"
        Me.btnConfirmMove.Size = New System.Drawing.Size(100, 35)
        Me.btnConfirmMove.TabIndex = 0
        Me.btnConfirmMove.Text = "Ход"
        Me.btnConfirmMove.UseVisualStyleBackColor = True
        '
        'btnCancelMove
        '
        Me.btnCancelMove.Location = New System.Drawing.Point(520, 585)
        Me.btnCancelMove.Name = "btnCancelMove"
        Me.btnCancelMove.Size = New System.Drawing.Size(100, 35)
        Me.btnCancelMove.TabIndex = 1
        Me.btnCancelMove.Text = "Отмена"
        Me.btnCancelMove.UseVisualStyleBackColor = True
        '
        'lblScore
        '
        Me.lblScore.AutoSize = True
        Me.lblScore.Location = New System.Drawing.Point(520, 20)
        Me.lblScore.Name = "lblScore"
        Me.lblScore.Size = New System.Drawing.Size(44, 13)
        Me.lblScore.TabIndex = 2
        Me.lblScore.Text = "Очки: 0"
        '
        'lblPlayers
        '
        Me.lblPlayers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblPlayers.Location = New System.Drawing.Point(550, 20)
        Me.lblPlayers.Name = "lblPlayers"
        Me.lblPlayers.Size = New System.Drawing.Size(300, 60)
        Me.lblPlayers.TabIndex = 3
        Me.lblPlayers.Text = "Label1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1255, 657)
        Me.Controls.Add(Me.lblPlayers)
        Me.Controls.Add(Me.lblScore)
        Me.Controls.Add(Me.btnCancelMove)
        Me.Controls.Add(Me.btnConfirmMove)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnConfirmMove As Button
    Friend WithEvents btnCancelMove As Button
    Friend WithEvents lblScore As Label
    Friend WithEvents lblPlayers As Label
End Class
