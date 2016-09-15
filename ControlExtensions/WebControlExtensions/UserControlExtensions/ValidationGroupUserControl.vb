Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace WebControlExtensions.UserControlExtensions
    Public Class UserControlWithValidationGroup
        Inherits UserControl

        Public Property ValidationGroup() As String
            Get
                Return CStr(ViewState("ValidationGroup"))
            End Get
            Set(ByVal value As String)
                SetValidationGroupOnChildren(Me, value)
                ViewState("ValidationGroup") = value
            End Set
        End Property

        Private Sub SetValidationGroupOnChildren(ByVal parent As Control, ByVal validationGroup As String)
            For Each ctrl As Control In parent.Controls
                If TypeOf ctrl Is BaseValidator Then
                    CType(ctrl, BaseValidator).ValidationGroup = validationGroup
                ElseIf TypeOf ctrl Is IButtonControl Then
                    CType(ctrl, IButtonControl).ValidationGroup = validationGroup
                ElseIf ctrl.HasControls() And ctrl.Visible = True Then
                    SetValidationGroupOnChildren(ctrl, validationGroup)
                End If
            Next
        End Sub
    End Class
End Namespace

