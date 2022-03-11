<%@ Application Language="VB" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.Data.SqlClient"%>

<script runat="server">
    Public Shared logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
    Protected Sub Application_PreSendRequestHeaders()
        ' Response.Headers.Remove("Server");
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-AspNetMvc-Version")
        Response.AddHeader("x-frame-options", "DENY")
        Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, pre-check=0,post-check=0")
        ' HTTP 1.1.
        Response.AppendHeader("Pragma", "no-cache")
        ' HTTP 1.0.
        Response.AppendHeader("Expires", "0")
        ' Proxies.
    End Sub

    ' new added n 09-may-17 for Security issue 

    Sub Application_BeginRequest()
        Dim URL = Context.Request.Url.ToString().ToUpper().Trim()
        If Not URL.Contains(".SVC/") Then
            If Not (Context.Request.IsSecureConnection) Then
                '''' below commented when app migrated to LB environment on 22-Jan-22 
                ''Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"))
            End If
        End If
    End Sub


    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
        'RegisterRoute(RouteTable.Routes)
    End Sub

    ' BELOW event will check if existing session and forcefully kill previous session if any by Vishal feb-19
    ' making commented/disable temporarily on request of Operation - IHCL 
    Private Sub Application_AcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
        If System.Web.HttpContext.Current.Session IsNot Nothing Then
            If Session("UID") IsNot Nothing Then
                If Session("USERROLE").ToString.ToUpper <> "SU" Then
                    Dim objDC As New DataClass()
                    Dim ssval As String = ""
                    Dim eid As Int32 = Session("EID")
                    ssval = objDC.ExecuteQryScaller("select sessionvalue from mmm_mst_user where eid=" & Session("EID") & " and uid=" & Session("UID") & "")
                    If ssval.ToString.Length > 1 And Session("TicketSSO") <> 1 Then
                        Dim cs As String = Request.Cookies("ASP.NET_SessionId").Value.ToString()
                        If ssval = cs Then
                        Else
                            HttpContext.Current.Response.Write("<hr/>Your ID is used for another login hence you are logged out.")
                            Session.Clear()
                            Session("UID") = Nothing
                            Response.Redirect("SignedOut.aspx")
                        End If
                    End If
                End If
            ElseIf Session("UID") Is Nothing Then
                ' Response.Redirect("SessionOut.aspx")
            End If
        End If
    End Sub


    'Private Sub Application_AcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
    '    If System.Web.HttpContext.Current.Session IsNot Nothing Then
    '        If Session("UID") IsNot Nothing Then
    '            If Session("USERROLE").ToString.ToUpper <> "SU" Then
    '                Dim objDC As New DataClass()
    '                Dim ssval As String = ""
    '                Dim eid As Int32 = Session("EID")
    '                ssval = objDC.ExecuteQryScaller("select sessionvalue from mmm_mst_user where eid=" & Session("EID") & " and uid=" & Session("UID") & "")
    '                Dim cs As String = Request.Cookies("ASP.NET_SessionId").Value.ToString()
    '                If ssval = cs Then
    '                Else
    '                    'HttpContext.Current.Response.Write("<hr/>Your ID is used for another login hence you are logged out.")
    '                    Session.Clear()
    '                    Session("UID") = Nothing
    '                    Response.Redirect("SignedOut.aspx")
    '                End If
    '            End If
    '        End If
    '    End If
    'End Sub


    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)


        ' Code that runs when an unhandled error occurs

        '' below liens disabled by sunil 21-sep for testing in devmynd for actual error
        Dim anError As System.Exception = Server.GetLastError()
        If anError.InnerException Is Nothing Then
            Server.ClearError()
            Exit Sub
        End If
        Dim TEst As String = sender.ToString()
        'Dim mm As New MyndImport
        'mm.Inserterrormessage(1, anError.InnerException.Source, anError.InnerException.Message)


        Dim mBody As String = "Hi Support Team (Live Server), <br/><br/> <b>Error Source : </b>" & anError.InnerException.Source & " <br/> <b>Error Message is </b><br/>" & anError.InnerException.Message & " <br/><b>Error Stack Trace is</b><br/> " & anError.StackTrace
        mBody = mBody & "<br/><br/> User ID " & Session("UID") & " Account Code : " & Session("EID")
        Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "swhelpdesk@myndsol.com", "UserID:" & Session("UID") & " Account Code:" & Session("EID"), mBody)
        Dim mailClient As New System.Net.Mail.SmtpClient()
        'Email.CC.Add("manish@myndsol.com")
        Email.IsBodyHtml = True
        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
        mailClient.Host = "mail1.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        mailClient.Send(Email)


        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString.ToString()
            Dim con As New SqlConnection(ConStr)
            Dim oda As New SqlDataAdapter("InsertErrorMess", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
            oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
            oda.SelectCommand.Parameters.AddWithValue("Messheading", anError.InnerException.Message)
            oda.SelectCommand.Parameters.AddWithValue("ErrorMess", mBody)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            con.Close()
            oda.Dispose()
        Catch ex As Exception
        End Try



        Server.ClearError()
        Response.Redirect("ErrorOccur.aspx")
    End Sub


    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        logger.Debug("Session_End: " & e.ToString())
        Response.Redirect("SessionOut.aspx")
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub


</script>