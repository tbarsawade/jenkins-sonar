Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Partial Class TestForm




    Inherits System.Web.UI.Page


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim val As String = "123"
        'If IsNumeric(val) Then
        '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        '    Dim tran As SqlTransaction = Nothing
        '    Dim con As SqlConnection = New SqlConnection(conStr)
        '    Try
        '        con.Open()
        '        tran = con.BeginTransaction()
        '        Dim objDC As New DataClass
        '        Dim dt As New DataTable
        '        Dim ht As New Hashtable
        '        ht.Add("@data", "server testing")
        '        objDC.TranExecuteProDT("test123", ht, con, tran)
        '        ht.Clear()
        '        ht.Add("@data", "Again server testing")
        '        objDC.TranExecuteProDT("test123", ht, con, tran)
        '        dt = objDC.TranExecuteQryDT("insert into test1 values('testing')", con, tran)
        '        dt = objDC.TranExecuteQryDT("select * from test1", con, tran)
        '        'dt = objDC.TranExecuteQryDT("insert into test1 values('testing',7)", con, tran)
        '        tran.Commit()
        '    Catch ex As Exception
        '        If Not tran Is Nothing Then
        '            tran.Rollback()
        '        End If
        '    Finally
        '        con.Close()
        '    End Try



        'End If
        'lvl.Attributes.Add("Style", " color: #565656;    display: block;    float: left;    height: 38px;    padding-bottom: 5px;    text-align: left;")
        '' txttest.Attributes.Add("style", "background: #fff none repeat scroll 0 0;    border: 2px solid #e96125;    border-radius: 4px;    color: #7d7d7d;    float: left;    font-size: 18px;    height: 38px;    margin-bottom: 8px;    padding-bottom: 5px;    padding-left: 10px;")
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '138/636836852135957399__2112019344889_34280.pdf
        'Dim SVal As String = txttest.Text
        'SVal = Replace(Replace(SVal, Chr(13), ""), Chr(10), "")
        'txtCorrect.Text = SVal
        Dim MAILTO As String = txtEmail.Text ' "sunil3778@gmail.com"
        ' Dim MAILTO As String = "sunil.pareek@myndsol.com"
        Dim MAILID As String = "test"
        Dim subject As String = "Test SSL/TLS mail from server"
        Dim MSG As String = ""
        Dim cc As String = ""
        Dim Bcc As String = ""
        Dim eid As Integer = Session("EID")
        Dim obj As New MailUtilltest(eid:=eid)

        Dim MailBody As String = ""

        MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
        MailBody &= "<p>Following PRAs are pending for your approval, please take suitable action at your end</p> <br>"

        'MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
        'MailBody &= "<TD>PRA Type</TD>"
        'MailBody &= "<TD>BPM ID</TD>"
        'MailBody &= "<TD>PRA Date</TD>"
        'MailBody &= "<TD>Customer name</TD>"
        'MailBody &= "<TD>Payment type</TD>"
        'MailBody &= "<TD>Total Payment Amount</TD>"
        'MailBody &= "<TD>Invoice Total</TD>"
        'MailBody &= "<TD>Received Date</TD>"
        'MailBody &= "<TD>Pending Hours</TD>"
        'MailBody &= "</Tr>"


        'MailBody &= "</Table>"

        MailBody &= "<font face=""arial, helvetica, sans-serif"" size=""6""> <p style=""color: Maroon"" > with Port 587 to GMAIL/Other than myndsol domain</p> </font> "

        MailBody &= "<font face=""arial, helvetica, sans-serif"" size=""6""> <p style=""color: Maroon"" > Entity - " & Session("EID") & " </p> </font> "

        MailBody &= "<p> Click  <a href=""https://myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"


        MailBody &= "<p style=""color: Maroon"">Regards</p>"
        MailBody &= "<p style=""color: Maroon"">Integration Team</p>"



        ' obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MailBody, CC:=cc, BCC:=Bcc)
        Try
            obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MailBody, CC:=cc, BCC:=Bcc)
            Label1.Text = "Mail Successful"
        Catch ex As Exception
            Label1.Text = "Erro in Mail sending" ' = ex.InnerException.ToString()  ' "Error in Mail"
        End Try


        ' sendMail1(MAILTO, cc, Bcc, subject, MailBody)


    End Sub





    'Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
    '    'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
    '    Try
    '        If Left(Mto, 1) = "{" Then
    '            Exit Sub
    '        End If
    '        Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", Mto, MSubject, MBody)
    '        Dim mailClient As New System.Net.Mail.SmtpClient()
    '        Email.IsBodyHtml = True
    '        If cc <> "" Then
    '            Email.CC.Add(cc)
    '        End If
    '        'vc79aK123AJ&$kL0
    '        If bcc <> "" Then
    '            Email.Bcc.Add(bcc)
    '        End If
    '        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
    '        mailClient.Host = "mail.myndsol.com"
    '        mailClient.UseDefaultCredentials = False
    '        mailClient.Credentials = basicAuthenticationInfo
    '        'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
    '        Try
    '            mailClient.Send(Email)
    '        Catch ex As Exception
    '            Exit Sub
    '        End Try
    '    Catch ex As Exception
    '        Exit Sub
    '    End Try
    'End Sub

    'Protected Sub btnValidate_Click(sender As Object, e As EventArgs) Handles btnValidate.Click
    '    'Dim EncodedResponse As String = Request.Form("g-Recaptcha-Response")
    '    'Dim IsCaptchaValid As Boolean = IIf(ReCaptchaClass.Validate(EncodedResponse) = "True", True, False)

    '    'If IsCaptchaValid Then
    '    '    'Valid Request
    '    'End If
    'End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '138/636836852135957399__2112019344889_34280.pdf
        'Dim SVal As String = txttest.Text
        'SVal = Replace(Replace(SVal, Chr(13), ""), Chr(10), "")
        'txtCorrect.Text = SVal
        Dim MAILTO As String = txtEmail.Text ' "sunil3778@gmail.com"
        ' Dim MAILTO As String = "sunil.pareek@myndsol.com"
        Dim MAILID As String = "test"
        Dim subject As String = "Test SSL/TLS mail from server"
        Dim MSG As String = ""
        Dim cc As String = ""
        Dim Bcc As String = ""
        Dim eid As Integer = Session("EID")
        Dim obj As New MailUtilltest(eid:=eid)

        Dim MailBody As String = ""

        MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
        MailBody &= "<p>Following PRAs are pending for your approval, please take suitable action at your end</p> <br>"

        

        'MailBody &= "</Table>"

        MailBody &= "<font face=""arial, helvetica, sans-serif"" size=""6""> <p style=""color: Maroon"" > with Port 587 to GMAIL/Other than myndsol domain</p> </font> "

        MailBody &= "<font face=""arial, helvetica, sans-serif"" size=""6""> <p style=""color: Maroon"" > Entity - " & Session("EID") & " </p> </font> "

        MailBody &= "<p> Click  <a href=""https://myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"


        MailBody &= "<p style=""color: Maroon"">Regards</p>"
        MailBody &= "<p style=""color: Maroon"">Integration Team</p>"

        If txtAtchment.Text = "" Then
            Label1.Text = "Enter Attachement Path"
            Exit Sub
        End If

        ' obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MailBody, CC:=cc, BCC:=Bcc)
        Try
            obj.SendMail1(ToMail:=MAILTO, Subject:=subject, MailBody:=MailBody, CC:=cc, BCC:=Bcc, DocType:="Invoice PO", attachementListString:=txtAtchment.Text, listOfDisplayNameForAttachments:="Invoice Attachment")
            Label1.Text = "Attachement Mail Successful"
        Catch ex As Exception
            Label1.Text = "Erro in Mail sending" ' = ex.InnerException.ToString()  ' "Error in Mail"
        End Try


        ' sendMail1(MAILTO, cc, Bcc, subject, MailBody)
    End Sub
End Class
