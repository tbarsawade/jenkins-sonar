Imports System.Data
Imports System.Data.SqlClient

Partial Class SendMail
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim STR As String = ""
        Dim MAILTO As String = ""
        Dim MAILID As String = ""
        Dim subject As String = ""
        Dim MSG As String = ""
        Dim cc As String = ""
        Dim Bcc As String = ""
        Dim ds As New DataSet()
        Dim da As SqlDataAdapter = New SqlDataAdapter("select T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry from MMM_MST_TEMPLATE T  WHERE tid=47", con)
        da.Fill(DS, "TEMP")
        MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
        MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
        '       cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
        '      Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()
        STR = ds.Tables("TEMP").Rows(0).Item("qry").ToString()
        STR &= " WHERE EID=32 and uid in (" & txtUserID.Text & ")"

        'STR &= " WHERE EID=32 and userid='80633'"
        da.SelectCommand.CommandText = STR
        da.Fill(ds, "qry")

        Dim errorMsg As String = " Mail Not Sent To "
        Dim mailNotSent As Integer = 0
        Dim i As Integer = 0
        If ds.Tables("TEMP").Rows.Count <> 0 Then
            Dim fn As String = ""
            For Each dr As DataRow In ds.Tables("qry").Rows
                i = i + 1
                MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()

                For Each dc As DataColumn In ds.Tables("qry").Columns
                    fn = "{" & dc.ColumnName.ToString() & "}"
                    MSG = MSG.Replace(fn, dr.Item(dc.ColumnName).ToString())
                    subject = subject.Replace(fn, dr.Item(dc.ColumnName).ToString())
                    MAILTO = dr.Item("emailID").ToString()
                Next
                Bcc = txtCCList.Text
                Try
                    sendMail1(MAILTO, cc, Bcc, subject, MSG)
                Catch ex As Exception
                    mailNotSent += 1
                    errorMsg &= " , " & MAILTO
                End Try
            Next
            lblError.Text = errorMsg
        End If
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
    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
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

        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "$up90rt#534")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
        mailClient.Send(Email)
    End Sub


End Class
