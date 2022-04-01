Imports System.Data
Imports System.Data.SqlClient
Imports System.Net

Partial Class pushNotification
    Inherits System.Web.UI.Page



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

        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "smaca")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
        mailClient.Send(Email)
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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("select RoleName from MMM_MST_Role where eid=" & Session("EID") & "", con)
            Dim dt As New DataTable
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlUserrole.DataSource = dt
                ddlUserrole.DataTextField = "RoleName"
                ddlUserrole.DataBind()
            End If
            da.Dispose()
            con.Close()
            con.Dispose()
        End If

    End Sub

    Protected Sub ddlUserrole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlUserrole.SelectedIndexChanged

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct RU.uid , u.Username from MMM_Ref_Role_User RU  inner join MMM_MST_User U on RU.uid=U.uid where RU.rolename='" & ddlUserrole.SelectedItem.Text & "' and U.eid=" & Session("EID") & " ", con)
        Dim dt As New DataTable
        dt.Clear()
        da.Fill(dt)
        chkUserName.Items.Clear()
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                chkUserName.Items.Add(dt.Rows(i).Item("username").ToString())
                chkUserName.Items(i).Value = dt.Rows(i).Item("uid").ToString()
            Next
        End If
        da.Dispose()
        con.Close()
        con.Dispose()
        ViewState("username") = dt
    End Sub
    Protected Sub sendMessage_Click(sender As Object, e As EventArgs) Handles sendMessage.Click
        Dim dt As DataTable = TryCast(ViewState("username"), DataTable)
        Dim chklist As New CheckBoxList
        chklist = CType(chkUserName.FindControl("chkUserName"), CheckBoxList)
        Dim b As String = String.Empty
        For Each chkitem As System.Web.UI.WebControls.ListItem In chklist.Items
            If chkitem.Selected = True Then
                b = b + chkitem.Value + ","
            End If
        Next
        b = b.TrimEnd(",", "")

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_googleclouid where userid in (" & b & ")", con)
        Dim dtt As New DataTable
        da.Fill(dtt)
        If dtt.Rows.Count > 0 Then
            Dim applicationID = "AIzaSyB6Bs0jCmHcpIcpjeLdEbIJ6HGPc_xm3Io"
            Dim SENDER_ID = "997065495011"
            Dim Msg As String = megBox.Text
            Dim obj As New GisMethods()
            Dim str As String = ""
            For i As Integer = 0 To dtt.Rows.Count - 1
                str = obj.sendNotification(dtt.Rows(i).Item("googleid").ToString(), Msg, applicationID, SENDER_ID)
            Next
        Else
            lblError.Visible = True
            lblError.Text = "user device is not registered !!!"
        End If
        con.Close()
        da.Dispose()

    End Sub

    'Public Function sendNotification(regId As String, Msg As String) As String
    '    Dim applicationID = "AIzaSyCqaT5MDZIX24NTvlXM8OWnW5lK1LQrIIo"
    '    Dim SENDER_ID = "947513119499"
    '    Dim tRequest As WebRequest
    '    tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send")
    '    tRequest.Method = "post"
    '    tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8"
    '    tRequest.Headers.Add(String.Format("Authorization: key={0}", applicationID))

    '    tRequest.Headers.Add(String.Format("Sender: id={0}", SENDER_ID))

    '    'Dim postData = "{ 'registration_id': [ '" + regId + "' ], 'data': {'message': '" + Msg + "'}}"
    '    Dim postData As String = (Convert.ToString("collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + Msg + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=") & regId) + ""

    '    Console.WriteLine(postData)

    '    Dim byteArray As [Byte]() = Encoding.UTF8.GetBytes(postData)
    '    tRequest.ContentLength = byteArray.Length

    '    Dim dataStream As Stream = tRequest.GetRequestStream()
    '    dataStream.Write(byteArray, 0, byteArray.Length)
    '    dataStream.Close()

    '    Dim tResponse As WebResponse = tRequest.GetResponse()

    '    dataStream = tResponse.GetResponseStream()

    '    Dim tReader As New StreamReader(dataStream)

    '    Dim sResponseFromServer As [String] = tReader.ReadToEnd()
    '    tReader.Close()
    '    dataStream.Close()
    '    tResponse.Close()
    '    Return sResponseFromServer
    'End Function


End Class


