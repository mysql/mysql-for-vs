Imports System.ComponentModel
'<WizardGeneratedCode>Namespace_UserCode</WizardGeneratedCode>

Namespace $safeprojectname$

    Public Class Form1

        '<WizardGeneratedCode>Private Variables Frontend</WizardGeneratedCode>

        Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
            '<WizardGeneratedCode>Form_Load</WizardGeneratedCode>
        End Sub

        Private Sub ToolStripButton1_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton1.Click
            If Not Me.Validate() Then
              Return
            End If
            '<WizardGeneratedCode>Save Event</WizardGeneratedCode>
        End Sub

        Private Sub Form1_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
            e.Cancel = False
        End Sub

        '<WizardGeneratedCode>Validation Events</WizardGeneratedCode>

        Private Sub bindingNavigatorAddNewItem_Click(sender As System.Object, e As System.EventArgs)
          '<WizardGeneratedCode>Add Event</WizardGeneratedCode>
        End Sub

    End Class

End Namespace