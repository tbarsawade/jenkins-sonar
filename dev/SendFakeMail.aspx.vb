Imports System.Net
Imports System.IO

Partial Class SendGreeting
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub


    Protected Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Dim EmailIDs As String
        Dim msgSent As String = ""
        EmailIDs = txtMailIDs.Text
        SendMailGreetings()
        lblMsg.Text = "Greeting Sent to " & msgSent
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
    Private Sub SendMailGreetings()
        ' Code that runs when an unhandled error occurs
        '' below liens disabled by sunil 21-sep for testing in devmynd for actual error

        Dim mBody As String = "Dear Rajesh Mohanty" & "<br/><br/>"


        mBody = mBody & "Shalini Verma was employed with us from 22-Jan-2013 to 08-July-2014 with the monthly payout of 23,500, She left our organization without serving notice period since according to her she needed to go to her hometown for some personal reason. Our Organization asked her to reimburse one month salary and her training cost of 30,000 but she denied and left without serving notice period."
        mBody = mBody & "<br /> <br /> Thanks and regards<br />Priyanka Sirohi - Assistant Manager <br/>+91-9873358431<br /> HR Department<br />VCare Pvt Ltd"

        mBody = mBody & "<br /><br /><br /><br />Sheduled message [HR Request No 1372: Shalini Verma] Queue No 219 <br /><br />"

        Dim sendorID As String = "no-reply@vcarecorporation.com"
        
        Dim Email As New System.Net.Mail.MailMessage(sendorID, "rajesh.mohanty@convergys.com", "HR Request No 1372: Shalini Verma", mBody)

        'Dim Email As New System.Net.Mail.MailMessage(sendorID, "shalini.9794@gmail.com", "HR Request No 219: Shalini Verma", mBody)
        Dim mailClient As New System.Net.Mail.SmtpClient()
        Email.CC.Add("meenakshi.chitkara@convergys.com")
        Email.Bcc.Add("manish@myndsol.com")

        Email.IsBodyHtml = True


        Dim basicAuthenticationInfo As New System.Net.NetworkCredential(sendorID, "12345")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        '' mailClient.Credentials = basicAuthenticationInfo
        Try
            mailClient.Send(Email)
        Catch ex As Exception
        End Try
    End Sub


    'Private Sub SendMailGreetingsTest()
    '    ' Code that runs when an unhandled error occurs
    '    '' below liens disabled by sunil 21-sep for testing in devmynd for actual error

    '    Dim mBody As String = "Rajesh Mohanty" & "<br/><br/><div style=""text-align:center""> "
    '    'Dim mBody As String = "<div style=""text-align:center""> "
    '    mBody = mBody & "<img src=""http://www.myndsaas.com/greetings/diwali greeting(mynd).jpg"" alt=""Happy Diwali"" /><br/>"
    '    mBody = mBody & "<a href=""http://www.myndsaas.com/greetings/Diwalione.html"">Click Here To View Greeting</a>"
    '    mBody = mBody & "</div>"
    '    Dim sendorID As String = "no-reply@myndsol.com"
    '    Dim password As String = "Dn#Ms@538Ti"
    '    If txtSendorID.Text.Length > 1 Then
    '        sendorID = txtSendorID.Text
    '    End If


    '    Dim Email As New System.Net.Mail.MailMessage(sendorID, mailto, txtSubject.Text, mBody)
    '    Dim mailClient As New System.Net.Mail.SmtpClient()
    '    Email.Bcc.Add(txtBCC.Text)
    '    Email.IsBodyHtml = True

    '    If txtSendorID.Text.Length > 1 Then
    '        password = txtPWD.Text
    '    Else
    '        sendorID = "no-reply@myndsol.com"
    '    End If

    '    Dim basicAuthenticationInfo As New System.Net.NetworkCredential(sendorID, password)
    '    mailClient.Host = "mail.myndsol.com"
    '    mailClient.UseDefaultCredentials = False
    '    mailClient.Credentials = basicAuthenticationInfo
    '    Try
    '        mailClient.Send(Email)
    '    Catch ex As Exception
    '    End Try
    'End Sub


End Class
