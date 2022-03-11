Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
'Imports Tasks
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Delegate


Partial Class Mailtest
    Inherits System.Web.UI.Page

    'Add Theme Code
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try

    End Sub

    Protected Sub btnShowGrid_Click1(sender As Object, e As EventArgs) Handles btnShowGrid.Click
        Try
            Dim eid = 66
            Dim obj As New MailUtill(eid:=eid)
            lblMsg.Text = "start"

            Dim ret As String = obj.SendMail("sunil.pareek@myndsol.com", "SECURITAS MAIL TESTING", "Hello! testing for mail from securitas domain", "")
            If ret = "Mail Sent" Then
                lblMsg.Text = "Mail sent successfully from rediff"
            Else
                lblMsg.Text = "Error in sending - " & ret
            End If

        Catch ex As Exception
            lblMsg.Text = "Error in mail sending - " & Convert.ToSingle(ex.InnerException.Message)
        End Try



    End Sub

   
End Class
