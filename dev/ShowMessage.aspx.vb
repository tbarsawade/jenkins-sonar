Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports AjaxControlToolkit
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Net
Imports System.Text
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Delegate

Imports OpenPop.Pop3
Imports OpenPop.Mime


Partial Class ShowMessage
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


    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim pop3Client As Pop3Client = CType(Session("Pop3Client"), Pop3Client)
            Dim messageNumber As Integer = Integer.Parse(Request.QueryString("MessageNumber"))
            Dim message As Message = pop3Client.GetMessage(messageNumber)
            Dim messagePart As MessagePart = message.MessagePart.MessageParts(0)
            lblFrom.Text = message.Headers.From.Address
            lblSubject.Text = message.Headers.Subject
            lblBody.Text = messagePart.BodyEncoding.GetString(messagePart.Body)
        End If
    End Sub


End Class
