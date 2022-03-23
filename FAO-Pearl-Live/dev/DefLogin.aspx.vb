
Imports System.Data
Imports System.Web.Script.Serialization

Partial Class _DefLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Page.IsPostBack) Then
            If Request.Cookies("ASP.NET_SessionId") IsNot Nothing Then
                Response.Cookies("ASP.NET_SessionId").Value = String.Empty
                Response.Cookies("ASP.NET_SessionId").Expires = DateTime.Now.AddMonths(-20)
            End If
        End If
    End Sub


    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function ForgotPassword(ByVal userid As String, ByVal customercode As String) As String
        Dim name As New Dictionary(Of String, String)()
        Dim objUser As New User
        Dim obj As New _DefLogin()
        Dim RESULT As String = ""
        If obj.ViewState("EID") Is Nothing Then
            If customercode = "" Then
                name.Add("ERROR", "Kindly enter your valid Customer Code")
            Else
                RESULT = objUser.MyeDmsPasswordRecover(userid, customercode)
            End If
        Else
            RESULT = objUser.MyeDmsPasswordRecover(userid, obj.ViewState("EID").ToString)
        End If
        If RESULT = "Successfully" Then
            name.Add("SUCCESS", userid)
        ElseIf RESULT = "ID LOCKED BY SU" Then
            name.Add("ERROR", "Your User ID is locked, contact your Super User")
        Else
            name.Add("ERROR", "User ID does not exists, Please check.")
        End If
        Dim myJsonString As String = (New JavaScriptSerializer()).Serialize(name)
        Return myJsonString
    End Function


    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function ValidateUser(ByVal username As String, ByVal password As String, ByVal entityname As String) As String
        Dim objDT As New DataTable()
        Dim objDC As New DataClass()
        Dim obj As New _DefLogin()
        Dim strLoginID As String = username.ToString().Trim().ToLower()
        Dim strPWD As String = password.ToString().Trim()
        Dim strEntity As String = ""
        If obj.ViewState("EID") Is Nothing Then
            strEntity = Trim(entityname)
        Else
            strEntity = Trim(obj.ViewState("EID"))
        End If
        strEntity.ToLower()

        strLoginID = strLoginID.Replace("'", "")
        ' strLoginID = strLoginID.Replace("-", "")
        strPWD = strPWD.Replace("'", "")
        'strPWD = strPWD.Replace("-", "")
        strEntity = strEntity.Replace("'", "")
        strEntity = strEntity.Replace("-", "")
        objDT = objDC.ExecuteQryDT("Select EID,Code from MMM_MST_ENTITY where Code='" & strEntity & "' ")
        Dim TempEid As Integer = 0
        If objDT.Rows.Count > 0 Then
            TempEid = objDT.Rows(0).Item("EID")
        End If
        Dim name As New Dictionary(Of String, String)()
        Dim oUser As New User
        Select Case oUser.validateUser(strLoginID, strPWD, strEntity)
            Case 100
                'user doesn't exist
                name.Add("100", "Sorry, your User ID is not configured in the system.")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Sorry, your User ID is not configured in the system."

            Case 0
                'user doesn't exist
                name.Add("0", "Sorry, you have entered an invalid User ID or Password. Please try again.")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Sorry, you have entered an invalid User ID or Password. Please try again."


            Case 7
                'user Blocked by Super user exist
                name.Add("7", "Sorry, your User ID has been blocked.")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Sorry, your User ID has been blocked."

            Case 3
                'wrong password but valid userID
                name.Add("3", "Sorry, you have entered an invalid User ID or Password. Please try again.")
                objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=isnull(passtry,0) +1 where USERID='" & username & "' and eid=" & TempEid)
                'Dim sqlq As String
                'sqlq = "UPDATE MMM_MST_USER SET passtry=isnull(passtry,0) +1 where USERID='" & Request.Form(txtUserID.UniqueID) & "' and eid=" & TempEid
                'Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                ''Dim ds As New DataSet
                'oda.Fill(ds, "user")
                'con.Dispose()
                'oda.Dispose()

                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Sorry, you have entered an invalid User ID or Password. Please try again."

            Case 4
                'Password retry reached lock him now
                'passtry=0,
                name.Add("4", "You have exceeded allowed maximum login attempts and your account is blocked temporarily.")
                objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET Modifydate=getdate(),isauth=3 where USERID='" & username & "' and eid=" & TempEid)
                'Dim sqlq As String
                'sqlq = "UPDATE MMM_MST_USER SET Modifydate=getdate(),isauth=3 where USERID='" & Request.Form(txtUserID.UniqueID) & "' and eid=" & TempEid
                'Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                ''Dim ds As New DataSet
                'oda.Fill(ds, "user")
                'con.Dispose()
                'oda.Dispose()
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "You have exceeded allowed maximum login attempts and your account is blocked temporarily."
            Case 10
                'Password retry reached lock him now
                'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                name.Add("10", "Your password has expired. Please regenerate it!")
                objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,Modifydate=getdate(),isauth=2 where USERID='" & username & "' and eid=" & TempEid)

                'Dim sqlq As String
                'sqlq = "UPDATE MMM_MST_USER SET passtry=0,Modifydate=getdate(),isauth=2 where USERID='" & Request.Form(txtUserID.UniqueID) & "' and eid=" & TempEid
                'Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                ''Dim ds As New DataSet
                'oda.Fill(ds, "user")
                'con.Dispose()
                'oda.Dispose()
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Your password has expired. Please regenerate it!"

            Case 5
                'Password is Locked
                name.Add("5", "You have exceeded allowed maximum login attempts and your account is blocked temporarily!")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "You have exceeded allowed maximum login attempts and your account is blocked temporarily!"

            Case 6
                'Password is Locked
                name.Add("6", "Your password has expired. Please regenerate it!")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Your password has expired. Please regenerate it!"
            Case 1
                HttpContext.Current.Session("UID") = oUser.UID
                HttpContext.Current.Session("USERNAME") = oUser.UserName
                HttpContext.Current.Session("USERROLE") = oUser.UserRole
                HttpContext.Current.Session("EMAIL") = oUser.emailID
                HttpContext.Current.Session("USERIMAGE") = oUser.UserImage
                HttpContext.Current.Session("CLOGO") = oUser.clogo
                HttpContext.Current.Session("EID") = oUser.EID
                HttpContext.Current.Session("IPADDRESS") = oUser.ipAddress
                HttpContext.Current.Session("MACADDRESS") = oUser.macAddress
                HttpContext.Current.Session("HEADERSTRIP") = obj.ViewState("HEADERSTRIP")
                HttpContext.Current.Session("EXTID") = oUser.ExtID
                If oUser.islocal = 0 Then
                    HttpContext.Current.Session("ISLOCAL") = "TRUE"
                Else
                    HttpContext.Current.Session("ISLOCAL") = "TRUE"
                End If
                HttpContext.Current.Session("INTIME") = Now
                HttpContext.Current.Session("LID") = oUser.locationID
                HttpContext.Current.Session("OFFSET") = oUser.offSet
                HttpContext.Current.Session("CODE") = oUser.strCode
                HttpContext.Current.Session("Roles") = oUser.roles
                HttpContext.Current.Session("Ctheme") = oUser.CTHEME

                'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,isauth=1 where userid='" & username & "' and eid=" & HttpContext.Current.Session("EID") & "")

                'Dim sqlq As String
                'sqlq = "UPDATE MMM_MST_USER SET passtry=0,isauth=1 where userid='" & Request.Form(txtUserID.UniqueID) & "' and eid=" & Session("EID") & ""
                'Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'oda.SelectCommand.ExecuteNonQuery()
                Dim objDTDefault As New DataTable()
                objDTDefault = objDC.ExecuteQryDT("select defaultpage from MMM_MST_ENTITY where eid=" & HttpContext.Current.Session("EID") & "")
                'sqlq = "select defaultpage from MMM_MST_ENTITY where eid=" & Session("EID") & ""
                'oda.SelectCommand.CommandText = sqlq
                'oda.Fill(ds, "user")
                If objDTDefault.Rows.Count = 1 Then
                    Try
                        Dim res = oUser.LogUserLogin(HttpContext.Current.Session("EID"), HttpContext.Current.Session("UID"))
                    Catch ex As Exception
                    End Try
                    Dim defaultpage As String = objDTDefault.Rows(0).Item("defaultpage").ToString()
                    If HttpContext.Current.Session("EID") = 54 Then
                        name.Add("1", "http://industowers.myndsaas.com/GMapHome.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("http://industowers.myndsaas.com/GMapHome.aspx")
                    ElseIf HttpContext.Current.Session("EID") = 58 Then
                        name.Add("1", "http://traqueur.myndsaas.com/TqMapHome.aspx")
                        HttpContext.Current.Response.RedirectPermanent("http://traqueur.myndsaas.com/TqMapHome.aspx")
                        'Response.RedirectPermanent("~/" & defaultpage & "")
                    ElseIf HttpContext.Current.Session("EID") = 60 Then
                        name.Add("1", "http://reliance.myndsaas.com/GMapHome.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("http://reliance.myndsaas.com/GMapHome.aspx")
                    ElseIf HttpContext.Current.Session("EID") = 71 Then
                        name.Add("1", "http://sales.myndsaas.com/TqMapHome.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("http://sales.myndsaas.com/TqMapHome.aspx")
                    ElseIf HttpContext.Current.Session("EID") = 72 Then
                        name.Add("1", "http://Telematics.myndsaas.com/NMapHome.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("http://Telematics.myndsaas.com/NMapHome.aspx")
                    ElseIf HttpContext.Current.Session("EID") = 49 And (HttpContext.Current.Session("UID") = 6595 Or HttpContext.Current.Session("UID") = 5900) Then
                        name.Add("1", "http://flipkart.myndsaas.com/Home.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("http://flipkart.myndsaas.com/Home.aspx")
                    End If
                    Dim objW As New Widget()
                    Dim EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
                    Dim DBName = "DashBoard"
                    Dim Roles = Convert.ToString(HttpContext.Current.Session("Roles"))
                    Dim dsD As New DataSet()
                    dsD = objW.GetWidgets(EID, DBName, Roles, 0)
                    If dsD.Tables(0).Rows.Count > 0 Then
                        name.Add("1", "dashboard1.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("~/" & "dashboard1.aspx" & "")
                    Else
                        name.Add("1", "" & defaultpage & "")
                        'HttpContext.Current.Response.RedirectPermanent("~/" & defaultpage & "")
                    End If
                Else
                    name.Add("1", "Please SET the default page for login  or contact to the super user")
                    'lblMsg.Text = "Please SET the default page for login  or contact to the super user "
                End If
                'con.Dispose()
                'oda.Dispose()
            Case 2
                name.Add("2", "Your password has expired. Please regenerate it!")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Your password has expired. Please regenerate it."
        End Select
        Dim myJsonString As String = (New JavaScriptSerializer()).Serialize(name)
        Return myJsonString
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetRequest(ByVal url As String) As String
        Dim sURL As String = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).ToUpper()
        'Comment
        sURL = "myndsaas.com".ToUpper()
        'Uncomment
        'lblYear.Text = "2013-" & DateAndTime.Now.Year.ToString()
        Dim resend As Integer = 0
        'Uncomment
        'If Left(sURL, 5).ToUpper() <> "HTTPS" Then
        '    resend = 1
        'End If
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        sURL = Replace(sURL, "HTTP://", "")
        sURL = Replace(sURL, "HTTPS://", "")
        sURL = Replace(sURL, "WWW.", "")
        sURL = Replace(sURL, ".MYNDSAAS.COM", "")
        sURL = Replace(sURL, "MYNDSAAS.COM", "")
        objDT = objDC.ExecuteQryDT("Select EID,Code,name,logo,isnull(headerimage,'login.jpg') [headerimage],isnull(headerstrip,'myndstrip.jpg') [headerstrip] from MMM_MST_ENTITY where Code='" & sURL & "' ")
        Dim issequelEID As Integer = 0
        Dim name As New Dictionary(Of String, String)()



        If objDT.Rows.Count = 1 Then
            Dim eidstr As String = ",32,56,57,58,60,66,67,79,85,89,95,"
            If eidstr.Contains("," & objDT.Rows(0).Item("EID").ToString() & ",") Then
                issequelEID = 1
                HttpContext.Current.Response.Redirect("http://" & sURL & ".bpm.sequelone.com")
            End If
            name.Add("1", "Welcome To " & objDT.Rows(0).Item("Name").ToString())
            name.Add("2", objDT.Rows(0).Item("logo").ToString())
            name.Add("3", objDT.Rows(0).Item("headerimage").ToString())
            HttpContext.Current.Session("HEADERSTRIP") = objDT.Rows(0).Item("headerstrip").ToString()
        Else
            name.Add("1", "Welcome To Mynd SaaS: Business Process Management System")
            name.Add("2", "myndlogo.png")
            'name.Add("2", "logo.gif")
            name.Add("3", "login.png")
            HttpContext.Current.Session("HEADERSTRIP") = "myndstrip.jpg"
        End If


        If sURL.Length < 2 Then
            'disable text box
            name.Add("EntityName", "VISIBLE")
            'name.Add("EntityName", "VISIBLE")  ' igagEntity
            'txtEntityName.Visible = True
            'lblEntity.Visible = True
            'txtFEntityName.Visible = True
            'lblEntityName.Visible = True
            'lblMsgAddFolder.Visible = True
            'lbltopmsg.Text = "Please submit your user ID and Customer Code here under. You will recieve an eMail to set up a new password."
            HttpContext.Current.Session("EID") = Nothing
        Else
            ' enable Text Box
            name.Add("EntityName", "INVISIBLE")
            name.Add("EntityCode", sURL)
            'txtEntityName.Visible = False
            'lblEntity.Visible = False
            'txtFEntityName.Visible = False
            'lblEntityName.Visible = False
            Dim obj As New _DefLogin()
            obj.ViewState("EID") = sURL
            'lblMsgAddFolder.Text = sURL
        End If

        Dim myJsonString As String = (New JavaScriptSerializer()).Serialize(name)
        Return myJsonString
    End Function

End Class
