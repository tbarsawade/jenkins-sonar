Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Web.Hosting
Imports System.Net.Mail

Public Class CRMBAL

    Dim strCon As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Function SetMessage(EID As Integer, DOCID As Integer, SMSText As String, MobNumber As String) As String
        Dim ret = ""
        Try
            Using con = New SqlConnection(strCon)
                Using da = New SqlDataAdapter("SetMessage", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@DOCID", DOCID)
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@SMS_Text", SMSText)
                    con.Open()
                    da.SelectCommand.ExecuteNonQuery()
                    ret = "success"
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Public Function GetConversation(EID As Integer, DOCID As Integer, CRMID As Integer) As DataSet
        Dim DS As New DataSet()
        Try
            Using con = New SqlConnection(strCon)
                Using da = New SqlDataAdapter("GetCRMConversation", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@DOCID", DOCID)
                    da.SelectCommand.Parameters.AddWithValue("@CRMID", CRMID)
                    con.Open()
                    da.Fill(DS)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return DS
    End Function

    Public Function SetMail(EID As Integer, DOCID As Integer, MailText As String, Fmail As String, Password As String, SMTP As String, Tomail As String, Subject As String, CC As String, BCC As String) As String
        Dim ret = ""
        Try
            Using con = New SqlConnection(strCon)
                Using da = New SqlDataAdapter("SendMail", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@DOCID", DOCID)
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@MailBody", MailText)
                    da.SelectCommand.Parameters.AddWithValue("@S_MailID", Fmail)
                    da.SelectCommand.Parameters.AddWithValue("@T_MailID", Tomail)
                    con.Open()
                    da.SelectCommand.ExecuteNonQuery()
                    ret = "success"
                End Using
            End Using
            'Code For sending mail
            Try
                If Fmail.Trim.ToLower = "no-reply@myndsol.com" Then
                    SMTP = "mail.myndsol.com"
                    Password = "smaca"
                End If
                Dim str = SendMail(Fmail, Password, SMTP, Tomail, CC, BCC, MailText, Subject)
            Catch ex As Exception

            End Try
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Private Function sendMail(FEmail As String, Password As String, SMTP As String, ToMail As String, CC As String, BCC As String, MailBody As String, Subject As String) As String
        Dim ret = ""
        'Array arrToArray, arrCC;
        Dim arrToArray As Array
        Dim arrCC As Array
        Dim arrBCC As Array
        Dim splitter As Char() = {","} 'When multiple recepient seperated by ';'
        Try
            arrToArray = ToMail.Split(splitter)
            arrCC = CC.Split(splitter)
            arrBCC = CC.Split(splitter)
            Dim message As New MailMessage()
            message.From = New MailAddress(FEmail)
            message.Subject = Subject
            message.Body = MailBody
            message.IsBodyHtml = True
            'Adding To Mail 
            Try
                For Each s As String In arrToArray
                    If s.Trim() <> "" Then
                        message.To.Add(New MailAddress(s))
                    End If
                Next
            Catch ex As Exception
            End Try
            
            'Adding CC Mail 
            Try
                For Each s As String In arrCC
                    If s.Trim() <> "" Then
                        message.CC.Add(New MailAddress(s))
                    End If
                Next
            Catch ex As Exception
            End Try
            
            'Adding BCC Mail 
            Try
                For Each s As String In arrBCC
                    If s.Trim() <> "" Then
                        message.Bcc.Add(New MailAddress(s))
                    End If
                Next
            Catch ex As Exception
            End Try
            'Adding SMTP details
            Dim nCred As New NetworkCredential()
            Dim client As New SmtpClient(SMTP)
            nCred.UserName = FEmail
            nCred.Password = Password
            client.UseDefaultCredentials = True
            client.Credentials = nCred
            client.Send(message)
            ret = "Mail Sent"
        Catch ex As Exception
            Return "fail"
        End Try
        Return ret
    End Function


End Class
