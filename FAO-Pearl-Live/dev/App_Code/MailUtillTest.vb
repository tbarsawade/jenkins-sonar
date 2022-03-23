Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System
Imports System.Text
Imports System.Net.Security
Imports System.IO.Stream
Imports System.Web.Hosting

Public Class MailUtilltest
    Dim EID As Integer
    Dim UserName As String
    Dim Password As String
    Dim SMTP As String
    Dim PORT As String
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Sub New(EID As Integer)   'constructor
        Me.EID = EID
        Dim ds As New DataSet()
        ds = Me.GetSMTP()
        Me.UserName = Convert.ToString(ds.Tables(0).Rows(0).Item("UserName")).Trim()
        Me.Password = Convert.ToString(ds.Tables(0).Rows(0).Item("Password")).Trim()
        Me.SMTP = Convert.ToString(ds.Tables(0).Rows(0).Item("Smtp")).Trim()
        Me.PORT = Convert.ToString(ds.Tables(0).Rows(0).Item("port")).Trim()
    End Sub

    Private Function GetSMTP() As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetSMTP", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", Me.EID)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function


    Public Function SendMail1(ToMail As String, Subject As String, MailBody As String, Optional CC As String = "", Optional BCC As String = "", Optional ByVal DocType As String = "", Optional ByVal EID As Integer = 0, Optional ByVal UpCommingFrom As String = "", Optional ByVal attachementListString As String = "", Optional ByVal tid As Integer = 0, Optional ByVal listOfDisplayNameForAttachments As String = "") As String
        Dim ret = "fail"
        Dim arrToArray As Array
        Dim arrCC As Array
        Dim arrBCC As Array
        ' Dim arrAtch As Array
        Dim splitter As Char() = {","} 'When multiple recepient seperated by ';'

        Try
            If Left(ToMail, 1) = "{" Then
                ret = "Error"
                ' Exit Function
            End If
            ''new for hd mail sending
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim query As String = "select * from mmm_hdmail_schdule where eid=" & EID & " and documenttype='" & DocType & "' and IsSendMailFromDefaultEMailID=0"
            If UpCommingFrom <> "" Then
                query = query & " and mdmailid='" & UpCommingFrom & "'"
            End If
            da.SelectCommand.CommandText = query
            da.SelectCommand.CommandType = CommandType.Text
            Dim dtSch As New DataTable
            da.Fill(dtSch)

            arrToArray = ToMail.Split(splitter)
            arrCC = CC.Split(splitter)
            arrBCC = BCC.Split(splitter)
            ' arrAtch = Attachments.Split(splitter)
            Dim message As New MailMessage()

            message.Subject = Subject
            message.Body = MailBody
            message.IsBodyHtml = True
            'message.SubjectEncoding = System.Text.Encoding.UTF8
            'message.BodyEncoding = System.Text.Encoding.UTF8
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
            '' adding attachments 
            If attachementListString <> "" Then
                Try
                    Dim docspath As String = HostingEnvironment.MapPath("~/Docs/")
                    Dim attachmentNames As String() = attachementListString.Split(New Char() {","c})
                    Dim attachmentName As String
                    Dim index As Integer = 1
                    Dim ListOfDisplayNames As String() = listOfDisplayNameForAttachments.Split(New Char() {","c})
                    For Each attachmentName In attachmentNames
                        If attachmentName <> "" Then
                            If File.Exists(docspath + attachmentName) Then
                                Dim attachmentToBeAdded As New Attachment(docspath + attachmentName)
                                Dim extension As String = Path.GetExtension(attachmentName)
                                'attachmentToBeAdded.ContentDisposition.FileName = EID.ToString() + "_" + tid.ToString() + "_" + index.ToString() + extension
                                attachmentToBeAdded.ContentDisposition.FileName = ListOfDisplayNames.GetValue(index - 1) + extension
                                message.Attachments.Add(attachmentToBeAdded)
                                index = index + 1
                            End If
                        End If
                    Next
                Catch ex As Exception
                End Try
            End If

            If dtSch.Rows.Count >= 1 Then
                message.From = New MailAddress(Convert.ToString(dtSch.Rows(0).Item("mdmailid")))
                '' add cred. and send mail
                Dim nCred As New NetworkCredential()
                nCred.UserName = Convert.ToString(dtSch.Rows(0).Item("mdmailid")) 'Me.UserName
                nCred.Password = Convert.ToString(dtSch.Rows(0).Item("mdpwd"))    'Password
                Dim client As New SmtpClient(Convert.ToString(dtSch.Rows(0).Item("hostname")))    'Me.SMTP
                client.Port = Me.PORT
                If Me.SMTP.ToLower = "smtp.gmail.com" Then
                    client.DeliveryMethod = SmtpDeliveryMethod.Network
                    client.EnableSsl = True
                End If
                client.UseDefaultCredentials = True
                client.Credentials = nCred
                client.Send(message)
                ret = "Mail Sent"
            Else  ' case of not from hd mail
                Try
                    message.From = New MailAddress(Me.UserName)
                    'Adding SMTP details
                    Dim nCred As New NetworkCredential()
                    nCred.UserName = Me.UserName
                    nCred.Password = Password
                    Dim client As New SmtpClient(Me.SMTP)
                    client.Port = Me.PORT
                    If Me.SMTP.ToLower = "smtp.gmail.com" Then
                        client.DeliveryMethod = SmtpDeliveryMethod.Network
                        client.EnableSsl = True
                    End If
                    client.UseDefaultCredentials = True
                    client.Credentials = nCred
                    client.Send(message)
                    ret = "Mail Sent"
                Catch ex As Exception
                    Return ex.InnerException.Message
                End Try
            End If
        Catch ex As Exception
            Return ex.InnerException.Message
        End Try
        Return ret
        ' --------------------------------------- starts here TLS PORT code
    End Function

    Public Function SendMail(ToMail As String, Subject As String, MailBody As String, Optional CC As String = "", Optional Attachments As String = "", Optional BCC As String = "") As String
        Dim ret = "fail"
        Dim arrToArray As Array
        Dim arrCC As Array
        Dim arrBCC As Array
        Dim arrAtch As Array
        Dim splitter As Char() = {","} 'When multiple recepient seperated by ';'
        If Me.SMTP.ToLower = "mail.myndsol.commmm" Then
            ' ret = SendMail(ToMail, Subject, MailBody, 465, CC, Attachments, BCC)
        Else
            Try
                arrToArray = ToMail.Split(splitter)
                arrCC = CC.Split(splitter)
                arrBCC = BCC.Split(splitter)
                arrAtch = Attachments.Split(splitter)
                Dim message As New MailMessage()
                message.From = New MailAddress(Me.UserName)
                message.Subject = Subject
                message.Body = MailBody
                message.IsBodyHtml = True
                'message.SubjectEncoding = System.Text.Encoding.UTF8
                'message.BodyEncoding = System.Text.Encoding.UTF8
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

                'Adding BCC Mail 
                Try
                    For Each s As String In arrAtch
                        If s.Trim() <> "" Then
                            message.Attachments.Add(New Attachment(s))
                        End If
                    Next
                Catch ex As Exception
                End Try
                'Adding SMTP details
                Dim nCred As New NetworkCredential()
                nCred.UserName = Me.UserName
                nCred.Password = Password
                Dim client As New SmtpClient(Me.SMTP)
                client.Port = Me.PORT
                If Me.SMTP.ToLower = "smtp.gmail.com" Then
                    client.DeliveryMethod = SmtpDeliveryMethod.Network
                    client.EnableSsl = True
                End If
                client.UseDefaultCredentials = True
                client.Credentials = nCred
                client.Send(message)
                ret = "Mail Sent"
            Catch ex As Exception
                Return ex.InnerException.Message
            End Try

        End If
        Return ret
    End Function


    '' backup by sp on 17-jan adding extra optional paramet..   for protocol upgrade.
    'Public Function SendMail(ToMail As String, Subject As String, MailBody As String, Optional CC As String = "", Optional Attachments As String = "", Optional BCC As String = "") As String
    '    Dim ret = "fail"
    '    Dim arrToArray As Array
    '    Dim arrCC As Array
    '    Dim arrBCC As Array
    '    Dim arrAtch As Array
    '    Dim splitter As Char() = {","} 'When multiple recepient seperated by ';'
    '    If Me.SMTP.ToLower = "mail.myndsol.commmm" Then
    '        ret = SendMail(ToMail, Subject, MailBody, 465, CC, Attachments, BCC)
    '    Else
    '        Try
    '            arrToArray = ToMail.Split(splitter)
    '            arrCC = CC.Split(splitter)
    '            arrBCC = BCC.Split(splitter)
    '            arrAtch = Attachments.Split(splitter)
    '            Dim message As New MailMessage()
    '            message.From = New MailAddress(Me.UserName)
    '            message.Subject = Subject
    '            message.Body = MailBody
    '            message.IsBodyHtml = True
    '            'message.SubjectEncoding = System.Text.Encoding.UTF8
    '            'message.BodyEncoding = System.Text.Encoding.UTF8
    '            'Adding To Mail 
    '            Try
    '                For Each s As String In arrToArray
    '                    If s.Trim() <> "" Then
    '                        message.To.Add(New MailAddress(s))
    '                    End If
    '                Next
    '            Catch ex As Exception
    '            End Try

    '            'Adding CC Mail 
    '            Try
    '                For Each s As String In arrCC
    '                    If s.Trim() <> "" Then
    '                        message.CC.Add(New MailAddress(s))
    '                    End If
    '                Next
    '            Catch ex As Exception
    '            End Try

    '            'Adding BCC Mail 
    '            Try
    '                For Each s As String In arrBCC
    '                    If s.Trim() <> "" Then
    '                        message.Bcc.Add(New MailAddress(s))
    '                    End If
    '                Next
    '            Catch ex As Exception
    '            End Try

    '            'Adding BCC Mail 
    '            Try
    '                For Each s As String In arrAtch
    '                    If s.Trim() <> "" Then
    '                        message.Attachments.Add(New Attachment(s))
    '                    End If
    '                Next
    '            Catch ex As Exception
    '            End Try
    '            'Adding SMTP details
    '            Dim nCred As New NetworkCredential()
    '            nCred.UserName = Me.UserName
    '            nCred.Password = Password
    '            Dim client As New SmtpClient(Me.SMTP)
    '            client.Port = 587
    '            If Me.SMTP.ToLower = "smtp.gmail.com" Then
    '                client.DeliveryMethod = SmtpDeliveryMethod.Network
    '                client.EnableSsl = True
    '            End If
    '            client.UseDefaultCredentials = True
    '            client.Credentials = nCred
    '            client.Send(message)
    '            ret = "Mail Sent"
    '        Catch ex As Exception
    '            Return ex.InnerException.Message
    '        End Try

    '    End If
    '    Return ret
    'End Function


    '' disabled as this is not being used after upgradation
    'Public Function SendMail(ToMail As String, Subject As String, MailBody As String, port As Integer, Optional CC As String = "", Optional Attachments As String = "", Optional BCC As String = "") As String
    '    Dim ret = "fail"
    '    Dim arrToArray As Array
    '    Dim arrCC As Array
    '    Dim arrBCC As Array
    '    Dim arrAtch As Array
    '    Dim splitter As Char() = {","} 'When multiple recepient seperated by ';'
    '    Try
    '        arrToArray = ToMail.Split(splitter)
    '        arrCC = CC.Split(splitter)
    '        arrBCC = BCC.Split(splitter)
    '        arrAtch = Attachments.Split(splitter)
    '        Dim message As CDO.Message = New CDO.Message()
    '        Dim configuration As CDO.IConfiguration = message.Configuration
    '        Dim fields As ADODB.Fields = configuration.Fields
    '        Dim field As ADODB.Field = fields("http://schemas.microsoft.com/cdo/configuration/smtpserver")
    '        field.Value = Me.SMTP
    '        field = fields("http://schemas.microsoft.com/cdo/configuration/smtpserverport")
    '        field.Value = port
    '        field = fields("http://schemas.microsoft.com/cdo/configuration/sendusing")
    '        field.Value = CDO.CdoSendUsing.cdoSendUsingPort
    '        field = fields("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate")
    '        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic
    '        field = fields("http://schemas.microsoft.com/cdo/configuration/sendusername")
    '        field.Value = Me.UserName
    '        field = fields("http://schemas.microsoft.com/cdo/configuration/sendpassword")
    '        field.Value = Me.Password
    '        field = fields("http://schemas.microsoft.com/cdo/configuration/smtpusessl")
    '        field.Value = "true"
    '        fields.Update()

    '        message.From = Me.UserName

    '        'Adding To Mail 
    '        Try
    '            For Each s As String In arrToArray
    '                If s.Trim() <> "" Then
    '                    message.To = message.To & ";" & s
    '                End If
    '            Next
    '        Catch ex As Exception
    '        End Try
    '        'Adding CC Mail 
    '        Try
    '            For Each s As String In arrCC
    '                If s.Trim() <> "" Then
    '                    'message.CC = s
    '                    message.CC = message.CC & ";" & s
    '                End If
    '            Next
    '        Catch ex As Exception
    '        End Try
    '        'Adding BCC Mail 
    '        Try
    '            For Each s As String In arrBCC
    '                If s.Trim() <> "" Then
    '                    'message.BCC = s
    '                    message.BCC = message.BCC & ";" & s
    '                End If
    '            Next
    '        Catch ex As Exception
    '        End Try

    '        'Adding BCC Mail 
    '        Try
    '            For Each s As String In arrAtch
    '                If s.Trim() <> "" Then
    '                    'message.Attachments(s)
    '                    message.AddAttachment(s)
    '                End If
    '            Next
    '        Catch ex As Exception
    '        End Try
    '        message.Subject = Subject
    '        message.HTMLBody = MailBody
    '        message.Send()
    '        ret = "Mail Sent"
    '    Catch ex As Exception
    '        Return ex.InnerException.Message
    '    End Try
    '    Return ret
    'End Function

    Public Sub New(EID As Integer, UserName As String, Password As String)   'constructor
        Me.EID = EID

        Dim ds As New DataSet()
        'ds = Me.GetSMTP()
        Me.UserName = UserName
        Me.Password = Password
        Me.SMTP = "mail.myndsol.com"
    End Sub

    Public Class Apperrors
        Dim EID As Integer
        Dim UID As Integer
        Dim MessHeading As String
        Dim erroMess As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

        Public Sub New(EID As Integer, UID As Integer, MessHeading As String, erroMess As String)
            Try
                Using con = New SqlConnection(conStr)
                    Dim qry As String = "insert into mmm_mst_apperror (eid,UID,MessHeading,erroMess,LogTime) values (" & EID & "," & UID & ",'" & MessHeading & "','" & erroMess & "',getdate() )"
                    Using cmd = New SqlCommand(qry, con)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim result As Integer
                        result = Convert.ToInt32(cmd.ExecuteNonQuery())
                        con.Dispose()
                        cmd.Dispose()
                    End Using
                End Using
            Catch ex As Exception
                Throw
            Finally

            End Try


        End Sub

    End Class



End Class
