Imports System.Net
Imports System.IO

Partial Class SendGreeting
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub
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

    Protected Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Dim EmailIDs() As String
        Dim msgSent As String = ""
        EmailIDs = txtMailIDs.Text.Split(",")
        For i As Integer = 0 To EmailIDs.Length - 1
            Dim strComp() As String = EmailIDs(i).Split(":")
            Try
                SendMailGreetings(strComp(1), strComp(0))
                msgSent = msgSent & strComp(1) & ","
            Catch ex As Exception
            End Try
        Next
        lblMsg.Text = "Greeting Sent to " & msgSent
    End Sub


    Private Sub SendMailGreetings(mailto As String, uname As String)
        ' Code that runs when an unhandled error occurs
        '' below liens disabled by sunil 21-sep for testing in devmynd for actual error

        Dim mBody As String = uname & "<br/><br/><div style=""text-align:center""> "
        'Dim mBody As String = "<div style=""text-align:center""> "
        mBody = mBody & "<img src=""http://www.myndsaas.com/greetings/diwali greeting(mynd).gif"" alt=""Happy Diwali"" /><br/>"
        mBody = mBody & "<a href=""http://www.myndsaas.com/greetings/Diwalione.html"">Click Here To View Greeting</a>"
        mBody = mBody & "</div>"
        Dim sendorID As String = "no-reply@myndsol.com"
        Dim password As String = "Dn#Ms@538Ti"
        If txtSendorID.Text.Length > 1 Then
            sendorID = txtSendorID.Text
        End If


        Dim Email As New System.Net.Mail.MailMessage(sendorID, mailto, txtSubject.Text, mBody)
        Dim mailClient As New System.Net.Mail.SmtpClient()
        Email.Bcc.Add(txtBCC.Text)
        Email.IsBodyHtml = True

        If txtSendorID.Text.Length > 1 Then
            password = txtPWD.Text
        Else
            sendorID = "no-reply@myndsol.com"
        End If

        Dim basicAuthenticationInfo As New System.Net.NetworkCredential(sendorID, password)
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        Try
            mailClient.Send(Email)
        Catch ex As Exception
        End Try
    End Sub


End Class
