Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters

Partial Class SendMailNew
    Inherits System.Web.UI.Page


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
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Dim dt As New DataTable
            DDLCC.Items.Clear()
            'ddltemplate.Items.Clear()
            Try
                'Using cmd As New SqlCommand()
                '    'cmd.CommandText = "Select userid,uid from mmm_mst_user where eid=" & Session("EID") & " and isauth<>1 "
                '    cmd.CommandText = ""
                '    cmd.Connection = con
                '    con.Open()
                '    Using sdr As SqlDataReader = cmd.ExecuteReader()
                '        While sdr.Read()
                '            Dim item As New ListItem()
                '            item.Text = sdr("userid").ToString()
                '            item.Value = sdr("uId").ToString()
                '            DDLCC.Items.Add(item)
                '        End While
                '    End Using
                '    con.Close()
                'End Using


                'da.SelectCommand.CommandText = "Select top 1 * from mmm_mst_template where (eid=" & Session("EID") & " or eid=0) and eventname='user' and subevent='USER CREATED' order by tid desc "
                'da.Fill(ds, "Template")
                'If ds.Tables("Template").Rows.Count > 0 Then
                '    ddltemplate.DataSource = ds.Tables("template")
                '    ddltemplate.DataTextField = "Template_name"
                '    ddltemplate.DataValueField = "tid"
                '    ddltemplate.DataBind()
                'End If
                'ddltemplate.Items.Insert(0, "Select")
                'Session("TabTemp") = ds.Tables("Template")


            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If


            End Try
        End If

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

        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
        mailClient.Send(Email)
    End Sub

    Protected Sub getallrecords(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim userlist As String = String.Empty
        Dim userlistname As String = String.Empty
        Dim cnt As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim fn As String = ""
        Dim errorMsg As String = " Mail Not Sent To "
        Dim mailNotSent As Integer = 0
        Dim MSG As String = ""
        'Dim subj As String = txtSubject.Text.ToString()
        Dim subj As String
        Dim var As String = ""
        Try


            For i As Integer = 0 To DDLCC.Items.Count - 1
                If DDLCC.Items(i).Selected = True Then
                    userlist = userlist & DDLCC.Items(i).Value & ","
                    userlistname = userlistname & DDLCC.Items(i).Text & ","

                    cnt = cnt + 1
                End If
            Next
            userlist = userlist.Remove(userlist.Length - 1)
            userlistname = userlistname.Remove(userlistname.Length - 1)

            Dim UidList As String() = userlist.ToString.Split(",")
            Dim UidName As String() = userlistname.ToString.Split(",")

            Dim str As String = ""
            If Not Session("MailQ") Is Nothing Then
                str = Session("MailQ").ToString()
            End If

            Dim mcnt As Integer
            For i As Integer = 0 To UidList.Length - 1
                da.SelectCommand.CommandText = "USpgetallEmailbyUIDsS"
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@userlist", UidList(i))
                da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim emailid As String = da.SelectCommand.ExecuteScalar()

                Dim dt As New DataTable

                da.SelectCommand.CommandText = str & "where eid=" & Session("EID") & " and uid in (" & UidList(i) & ")"
                da.SelectCommand.CommandType = CommandType.Text
                da.Fill(dt)


                da.SelectCommand.CommandText = "update mmm_mst_user set isauth=100 where eid=" & Session("EID") & " and uid in (" & UidList(i) & ")"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteScalar()

                ' Dim dt As DataTable = DirectCast(Session("MailQ"), DataTable)
                If dt.Rows.Count > 0 Then

                    For k As Integer = 0 To dt.Rows.Count - 1
                        For Each dc As DataColumn In dt.Columns
                            fn = "{" & dc.ColumnName.ToString() & "}"
                            MSG = MSG.Replace(fn, dt.Rows(k).Item(dc.ColumnName).ToString())
                            subj = subj.Replace(fn, dt.Rows(k).Item(dc.ColumnName).ToString())
                        Next
                    Next


                End If

                ' End If
                '  
                Dim obj As New MailUtill(EID:=Session("EID"))
                var = var & emailid.ToString() & ","
                mcnt += 1
                obj.SendMail(emailid, subj.ToString(), HttpUtility.HtmlDecode(MSG.ToString()), "", , "")

                dt.Clear()
                dt.Dispose()
                fn = ""
                'MSG = txtBody.Text.ToString()
                'subj = txtSubject.Text.ToString()
            Next

            lblMsg1.Text = "Mail Sent Successfully (" & mcnt & ") to & userlistname.ToString() & """
            btnDelete_ModalPopupExtender.Hide()
            'txtBcc.Text = ""
            'txtBody.Text = ""
            'txtSubject.Text = ""
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Mail Sent Successfully to " & userlistname.ToString() & "');window.location='SendMailNew.aspx';", True)

        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            'lblMsg1.Text = mailNotSent & ":::" & var
        End Try

    End Sub


    'Protected Sub getallrecords(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim userlist As String = String.Empty
    '    Dim userlistname As String = String.Empty
    '    Dim cnt As Integer = 0
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet
    '    Dim fn As String = ""
    '    Dim errorMsg As String = " Mail Not Sent To "
    '    Dim mailNotSent As Integer = 0
    '    Dim MSG As String = txtBody.Text.ToString()
    '    Dim subj As String = txtSubject.Text.ToString()
    '    Dim var As String = ""
    '    Try


    '        For i As Integer = 0 To DDLCC.Items.Count - 1
    '            If DDLCC.Items(i).Selected = True Then
    '                userlist = userlist & DDLCC.Items(i).Value & ","
    '                userlistname = userlistname & DDLCC.Items(i).Text & ","

    '                cnt = cnt + 1
    '            End If
    '        Next
    '        userlist = userlist.Remove(userlist.Length - 1)
    '        userlistname = userlistname.Remove(userlistname.Length - 1)
    '        da.SelectCommand.CommandText = "USpgetallEmailbyUIDsS"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.AddWithValue("@userlist", userlist)
    '        da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim emailid As String = da.SelectCommand.ExecuteScalar()
    '        If Not Session("MailQ") Is Nothing Then
    '            Dim str As String = Session("MailQ").ToString()
    '            da.SelectCommand.CommandText = str & "where eid=" & Session("EID") & " and uid in (" & userlist & ")"
    '            da.SelectCommand.CommandType = CommandType.Text
    '            Dim dt As New DataTable
    '            da.Fill(dt)

    '            da.SelectCommand.CommandText = "update mmm_mst_user set isauth=100 where eid=" & Session("EID") & " and uid in (" & userlist & ")"
    '            If con.State <> ConnectionState.Open Then
    '                con.Open()
    '            End If
    '            da.SelectCommand.ExecuteScalar()

    '            ' Dim dt As DataTable = DirectCast(Session("MailQ"), DataTable)
    '            If dt.Rows.Count > 0 Then
    '                For Each dr As DataRow In dt.Rows

    '                    For Each dc As DataColumn In dt.Columns
    '                        fn = "{" & dc.ColumnName.ToString() & "}"
    '                        MSG = MSG.Replace(fn, dr.Item(dc.ColumnName).ToString())

    '                        subj = subj.Replace(fn, dr.Item(dc.ColumnName).ToString())
    '                    Next
    '                Next
    '            End If
    '        End If
    '        Dim elist As String() = emailid.ToString.Split(",")

    '        Dim obj As New MailUtill(eid:=Session("EID"))
    '        For i As Integer = 0 To elist.Length - 1
    '            var = var & elist(i).ToString()

    '            obj.SendMail(elist(i).ToString(), subj.ToString(), HttpUtility.HtmlDecode(MSG.ToString()), "", "", txtBcc.Text.ToString())

    '        Next

    '        lblMsg1.Text = "Mail Sent Successfully to " & userlistname.ToString() & ""
    '        btnDelete_ModalPopupExtender.Hide()
    '        txtBcc.Text = ""
    '        txtBody.Text = ""
    '        txtSubject.Text = ""

    '        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Mail Sent Successfully to " & userlistname.ToString() & "');window.location='SendMailNew.aspx';", True)

    '    Catch ex As Exception

    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        'lblMsg1.Text = mailNotSent & ":::" & var
    '    End Try

    'End Sub

    Protected Sub ALErtHit(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        'If ddltemplate.SelectedItem.Text = "Select" Then
        '    lblMsg1.Text = "* Please select a valid Template"
        '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Please select a valid Template');", True)
        '    Exit Sub
        'End If

        'If txtSubject.Text.Length < 5 Then

        lblMsg1.Text = "* Please enter a valid subject"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Please enter a valid subject');", True)
            Exit Sub
        'End If
        Dim userlist As String = ""
        Dim cnt As Integer = 0
        For i As Integer = 0 To DDLCC.Items.Count - 1
            If DDLCC.Items(i).Selected = True Then
                ' userlist = userlist & DDLCC.Items(i).Value & ","
                cnt = cnt + 1
            End If
        Next
        If cnt < 1 Then
            lblMsg1.Text = "* Please select User to send mail!!"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Please select User to send mail!!');", True)
            Exit Sub
        End If


        lblMsgDelete.Text = "Are you sure want to send mail to the selected Users?"
        'txtBody.Text = HttpUtility.HtmlDecode(Session("bodytext")).ToString()
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub
    'Protected Sub ddltemplate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddltemplate.SelectedIndexChanged
    '    If Not Session("TabTemp") Is Nothing Then
    '        Dim dt As DataTable = DirectCast(Session("TabTemp"), DataTable)
    '        txtBcc.Text = dt.Rows(0).Item("BCC").ToString()
    '        txtSubject.Text = dt.Rows(0).Item("Subject").ToString
    '        txtBody.Text = HttpUtility.HtmlDecode(dt.Rows(0).Item("MSGBODY").ToString.Trim())
    '        Session("bodytext") = HttpUtility.HtmlDecode(dt.Rows(0).Item("MSGBODY").ToString.Trim())
    '        Session("MailQ") = dt.Rows(0).Item("qry").ToString()

    '    End If
    'End Sub
    <System.Web.Services.WebMethod()>
    Public Shared Function GetTemplate(documentType As String) As kGridReallocationSendMail
        Dim jsonData As String = ""

        Dim ret As New kGridReallocationSendMail()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim strQuery1 As String = ""
        Try

            da.SelectCommand.CommandText = "Select top 1 * from mmm_mst_template where (eid=" & HttpContext.Current.Session("EID") & " or eid=0) and eventname='user' and subevent='USER CREATED' order by tid desc"
            da.Fill(ds, "Template")
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            ret.Data = jsonData
            HttpContext.Current.Session("TabTemp") = ds.Tables("Template")
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function Getdata()
        Dim jsonData As String = ""
        Dim ds As New DataTable()
        Dim UID As Integer = 0
        Dim URole As String = ""
        Dim qry As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dt As New DataTable
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        qry = "Select  UserName ,Emailid ,UserID ,userRole,UID  from mmm_mst_user  where eid=" & HttpContext.Current.Session("EID") & " and isauth=100"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = qry
        oda.Fill(ds)
        con.Close()
        dt.Dispose()
        Dim lstColumns As New List(Of String)
        Dim serializerSettings As New JsonSerializerSettings()
        Dim json_serializer As New JavaScriptSerializer()
        serializerSettings.Converters.Add(New DataTableConverter())
        jsonData = JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.None, serializerSettings)
        Return jsonData
    End Function
    'ClsNextUserSendMail As List(Of ClsNextUserSendMail) , subj As String, bcc As String, Mailqry As String


    <System.Web.Services.WebMethod()>
    Public Shared Function sendnewmail(ClsNextUser As List(Of ClsNextUserSendMail), MSG As String, subj As String, bcc As String, Mailqry As String) As String



        Dim userlist As String = String.Empty
        Dim userlistname As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim dt1 As New DataTable
        Dim fn As String = ""
        Dim errorMsg As String = " Mail Not Sent To "
        Dim mailNotSent As Integer = 0
        Dim dmsbodytext As String = String.Empty
        Dim dmssubject As String = String.Empty

        'Dim MSG As String = txtBody.Text.ToString()
        'Dim subj As String = txtSubject.Text.ToString()
        Dim var As String = ""
        Try

            'txtBody.Text = HttpUtility.HtmlDecode(dt.Rows(0).Item("MSGBODY").ToString.Trim())

            HttpContext.Current.Session("MailQ") = Mailqry
            Dim str As String = ""
            If Not HttpContext.Current.Session("MailQ") Is Nothing Then
                str = HttpContext.Current.Session("MailQ").ToString()
            End If
            Dim mcnt As Integer
            For i As Integer = 0 To ClsNextUser.Count - 1
                da.SelectCommand.CommandText = "USpgetallEmailbyUIDsS"
                da.SelectCommand.Parameters.Clear()

                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@userlist", ClsNextUser(i).UID)
                da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("EID"))
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim emailid As String = da.SelectCommand.ExecuteScalar()
                Dim dt As New DataTable
                dmsbodytext = HttpUtility.HtmlDecode(MSG)
                dmssubject = subj

                da.SelectCommand.CommandText = str & "where eid=" & HttpContext.Current.Session("EID") & " and uid in (" & ClsNextUser(i).UID & ")"
                da.SelectCommand.CommandType = CommandType.Text
                da.Fill(dt)


                da.SelectCommand.CommandText = "update mmm_mst_user set isauth=100 where eid=" & HttpContext.Current.Session("EID") & " and uid in (" & ClsNextUser(i).UID & ")"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteScalar()

                ' Dim dt As DataTable = DirectCast(Session("MailQ"), DataTable)
                If dt.Rows.Count > 0 Then

                    For k As Integer = 0 To dt.Rows.Count - 1
                        For Each dc As DataColumn In dt.Columns
                            fn = "{" & dc.ColumnName.ToString() & "}"
                            dmsbodytext = dmsbodytext.Replace(fn, dt.Rows(k).Item(dc.ColumnName).ToString())
                            dmssubject = dmssubject.Replace(fn, dt.Rows(k).Item(dc.ColumnName).ToString())
                        Next
                    Next
                End If

                ' End If
                '  
                Dim obj As New MailUtill(EID:=HttpContext.Current.Session("EID"))
                var = var & emailid.ToString() & ","
                mcnt += 1
                'emailid
                'manvendra.singh@myndsol.com
                obj.SendMail(emailid, dmssubject.ToString(), HttpUtility.HtmlDecode(dmsbodytext.ToString()), "", , bcc)

                dt.Clear()
                dt.Dispose()
                fn = ""
                'MSG = MSG
                'subj = subj

            Next

        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
            con.Dispose()
        End Try

        Return "Mail Sent Successfully"
    End Function
End Class
Public Class ClsNextUserSendMail
    Public Property UserName As String
    Public Property Emailid As String
    Public Property UserID As String
    Public Property userRole As String
    Public Property UID As Integer


End Class
Public Class kGridReallocationSendMail
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnReallocationSendMail)
End Class
Public Class kColumnReallocationSendMail
    Public Sub New()

    End Sub
    Public Sub New(staticfield As [String], statictitle As [String], statictype As String, staticFormat As String)
        field = staticfield
        title = statictitle
        type = statictype
        format = staticFormat
        filterable = True
        If (statictype = "number") Then
            filterable = ""
        End If
        'width = staticwidth
    End Sub

    Public field As String = ""
    Public title As String = ""
    Public width As Integer = 200
    Public format As String = ""
    Public filterable As String = ""
    'Public locked As Boolean = True
    'Public locked As Boolean = True
    Public type As String = ""
    Public FieldID As String = ""

End Class
