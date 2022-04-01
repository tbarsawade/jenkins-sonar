Imports Microsoft.VisualBasic

Public Class MailSend

    Dim mailFrom As String = ""
    Dim mailPwd As String = ""
    Dim mailHost As String = ""

    Public Sub New(ByRef CallMailFrom As String, ByRef CallMailPwd As String, ByRef CallMailHost As String)
        mailFrom = CallMailFrom
        mailPwd = CallMailPwd
        mailHost = CallMailHost
    End Sub
    Public Function SendMail(ByRef Mto As String, ByRef MSubject As String, ByRef MBody As String, ByRef cc As String, ByRef bcc As String) As Boolean
        Dim bool As Boolean = False
        Try
            If Left(Mto, 1) = "{" Then
                Exit Try
            End If
            Dim Email As New System.Net.Mail.MailMessage(mailFrom, Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
            End If

            If bcc <> "" Then
                Email.Bcc.Add(bcc)
            End If
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential(mailFrom, mailPwd)
            mailClient.Host = mailHost
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                bool = False
                Exit Try
            End Try
        Catch ex As Exception
            bool = False
            Exit Try
        End Try
        Return bool
    End Function

  
End Class
