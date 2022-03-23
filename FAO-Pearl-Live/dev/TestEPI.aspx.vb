Imports System.Data

Partial Class TestEPI
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        OnloadAgentTemplate()
        OnloadAdminTemplate()
    End Sub

    Protected Sub OnloadAdminTemplate()
        Dim objDC As New DataClass()
        Dim ht As New Hashtable()
        ht.Add("@documenttype", "Ticket document")
        ht.Add("@Eid", 113)
        Dim objDTUser As New DataTable()
        objDTUser = objDC.ExecuteProDT("sp_GetAdminEPITAT", ht)
        ht.Clear()
        ht.Add("@documenttype", "Ticket document")
        ht.Add("@Eid", 113)
        Dim objDTAll As New DataTable()
        objDTAll = objDC.ExecuteProDT("sp_GetCountAdminEPITAT", ht)
        AdminTemplate(objDTUser, objDTAll)

    End Sub
    Protected Sub AdminTemplate(Optional ByVal objDT As DataTable = Nothing, Optional ByVal objDTTotal As DataTable = Nothing)
        Dim mailTemplate As New StringBuilder()
        Dim objDC As New DataClass()
        Dim superAdminDT As DataTable = objDC.ExecuteQryDT("select username,emailid from mmm_mst_user where eid=113 and userrole='ADMIN' and isAuth=1")
        If Not IsNothing(objDT) Then
            Dim subject As String = "Ticket(s) SLA Expiration Alert "
            If objDT.Rows.Count > 0 Then
                mailTemplate.Append("Dear " & IIf(superAdminDT.Rows(0)(0) = "", "Super Admin", superAdminDT.Rows(0)(0)) & ",<div><br></div><div>Please find below are the tickets which had expired. </div><div><br></div>")
                mailTemplate.Append("<table border=""1"" ><tr style=""background-color: #0000ff; color:white;""><td>Category</td><td>Count</td></tr>")
                For Each dr As DataRow In objDTTotal.Rows
                    mailTemplate.Append("<tr><td>" & dr("Cateogory") & "</td><td>" & dr("Count") & "</td></tr>")
                Next
                mailTemplate.Append("</table>")
            End If
            mailTemplate.Append("<div><br></div><div>Details of the expired SLA tickets :-</div><div><br></div>")
            mailTemplate.Append("<table border=""1"" ><tr style=""background-color: #0000ff; color:white;""><td>DOCID</td><td>Category</td><td>Requestor</td></tr>")
            Dim arr As New ArrayList
            For Each dr As DataRow In objDT.Rows
                arr.Add(dr("EMAILID"))
                mailTemplate.Append("<tr><td>" & dr("DOCID") & "</td><td>" & dr("Cateogory") & "</td><td>" & dr("Requestor") & "</td></tr>")
            Next
            mailTemplate.Append("</table><div><br></div><div>Thanks &amp; Regards</div><div>Ask HR Team</div>")
            sendMail1(superAdminDT.Rows(0)(1), String.Join(",", arr.ToArray().Distinct()), "mayank.garg@myndsol.com,swhelpdesk@myndsol.com", subject, mailTemplate.ToString())
        End If
    End Sub
    Protected Sub OnloadAgentTemplate()
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("select uid,emailid from mmm_mst_user where eid=113 and userrole='AGENT'")
        For Each dr As DataRow In objDT.Rows
            Dim ht As New Hashtable()
            ht.Add("@documenttype", "Ticket document")
            ht.Add("@Eid", 113)
            ht.Add("@UserID", dr(0))
            Dim objDTUser As New DataTable()
            objDTUser = objDC.ExecuteProDT("sp_GetActualEPITAT", ht)
            AgentTemplate(objDTUser, dr(1))
        Next
    End Sub
    Protected Sub AgentTemplate(Optional ByVal objDT As DataTable = Nothing, Optional ByRef mailTO As String = "")
        Dim mailTemplate As New StringBuilder()

        If Not IsNothing(objDT) Then
            Dim subject As String = "SLA for Ticket(s) resolution is pending for today "
            If objDT.Rows.Count > 0 Then
                mailTemplate.Append("Dear " & objDT.Rows(0)("USERNAME") & ",<div><br></div><div>Please find below are the tickets pending for revolution by EOD.</div><div><br></div>")
                mailTemplate.Append("<table border=""1"" ><tr style=""background-color: #0000ff; color:white;""><td>DOCID</td><td>Requestor</td></tr>")
                For Each dr As DataRow In objDT.Rows
                    mailTemplate.Append("<tr><td>" & dr("DOCID") & "</td><td>" & dr("Requestor") & "</td></tr>")
                Next
                mailTemplate.Append("</table><div><br></div><div>Thanks &amp; Regards</div><div>Ask HR Team</div>")
                sendMail1(mailTO, "", "mayank.garg@myndsol.com,swhelpdesk@myndsol.com", subject, mailTemplate.ToString())
            End If
        End If
    End Sub
    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
        Try
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
        If Left(Mto, 1) = "{" Then
            Exit Sub
        End If
        Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
        Dim mailClient As New System.Net.Mail.SmtpClient()
        Email.IsBodyHtml = True
        If cc <> "" Then
            Email.CC.Add(cc)
        End If

        If bcc <> "" Then
            Email.Bcc.Add(bcc)
        End If

            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            mailClient.Send(Email)
        Catch ex As Exception

        End Try
    End Sub
End Class
