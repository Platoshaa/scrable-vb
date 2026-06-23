<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.btnConfirmMove = New System.Windows.Forms.Button()
        Me.btnCancelMove = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnConfirmMove
        '
        Me.btnConfirmMove.Location = New System.Drawing.Point(520, 540)
        Me.btnConfirmMove.Name = "btnConfirmMove"
        Me.btnConfirmMove.Size = New System.Drawing.Size(100, 35)
        Me.btnConfirmMove.TabIndex = 0
        Me.btnConfirmMove.Text = "Готово"
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
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1255, 657)
        Me.Controls.Add(Me.btnCancelMove)
        Me.Controls.Add(Me.btnConfirmMove)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnConfirmMove As Button
    Friend WithEvents btnCancelMove As Button
End Class
