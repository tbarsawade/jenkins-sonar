Imports System.Data
Imports System.IO
Imports System.Net
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports System.Xml

Partial Class _Default
    Inherits System.Web.UI.Page
    Public Shared ReCaptcha_Key As String = "6Lf-LY0UAAAAAH_LcClWa7H3UfXdJ6rFJfXZmU_P"
    Public Shared ReCaptcha_Secret As String = "6Lf-LY0UAAAAAMtmsySIi3gq_RtYKP1RtNGGfvj3"
    Public Shared logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
    'Public Shared a As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblYear.InnerHtml = DateAndTime.Now.Year.ToString()
        'a = Session("CODE")
        If (Not Page.IsPostBack) And Session("CODE") Is Nothing Then
            If Request.Cookies("ASP.NET_SessionId") IsNot Nothing Then
                Session.Clear()
                Response.Cookies("ASP.NET_SessionId").Value = String.Empty
                Response.Cookies("ASP.NET_SessionId").Expires = DateTime.Now.AddMonths(-20)
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function ForgotPassword(ByVal userid As String, ByVal customercode As String) As String
        Dim name As New Dictionary(Of String, String)()
        Dim objUser As New User
        Dim obj As New _Default()
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
        'entityname = "Pearl"
        Dim objDT As New DataTable()
        Dim objDC As New DataClass()
        Dim obj As New _Default()
        Dim strLoginID As String = username.ToString().Trim().ToLower()
        Dim strPWD As String = password.ToString().Trim()
        Dim strEntity As String = ""
        If obj.ViewState("EID") Is Nothing Then
            strEntity = Trim(entityname)
        Else
            strEntity = Trim(obj.ViewState("EID"))
        End If
        strEntity.ToLower()
        Dim name As New Dictionary(Of String, String)()
        If HttpContext.Current.Session("CODE") IsNot Nothing And HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value IsNot Nothing Then
            If HttpContext.Current.Session("CODE").ToString.ToUpper() = strEntity.ToUpper() Then
                name.Add("99", "Another User is already logged In, kindly close the current session or use another browser")
                Dim myJsonString1 As String = (New JavaScriptSerializer()).Serialize(name)
                Return myJsonString1
                Exit Function
            End If
        End If

        strLoginID = strLoginID.Replace("'", "")
        ' strLoginID = strLoginID.Replace("-", "")
        strPWD = strPWD.Replace("'", "")
        'strPWD = strPWD.Replace("-", "")
        strEntity = strEntity.Replace("'", "")
        strEntity = strEntity.Replace("-", "")
        objDT = objDC.ExecuteQryDT("Select EID,Code,ISNULL(EnableMailOTP,0) EnableMailOTP, ISNULL(MOTPExpiry,10) MOTPExpiry,ISNULL(MOTP_EXcludeRoles,'') MOTP_EXcludeRoles from MMM_MST_ENTITY where Code='" & strEntity & "' ")
        Dim TempEid As Integer = 0
        If objDT.Rows.Count > 0 Then
            TempEid = objDT.Rows(0).Item("EID")
        End If
        'Dim name As New Dictionary(Of String, String)()
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
                ' build OTP logic===============================================
                Dim Is_EnableMailOTP As Boolean = Convert.ToBoolean(objDT.Rows(0)("EnableMailOTP"))
                If (Is_EnableMailOTP) Then
                    Dim OTP_ExcludeRoles = objDT.Rows(0)("MOTP_EXcludeRoles")
                    Dim arrExcludeRoles As String() = OTP_ExcludeRoles.split(","c)
                    If (OTP_ExcludeRoles <> "") Then
                        For i As Integer = 0 To arrExcludeRoles.Length - 1
                            If (arrExcludeRoles(i) = oUser.UserRole) Then
                                Is_EnableMailOTP = False
                                Exit For
                            End If
                        Next
                    Else
                        Is_EnableMailOTP = True
                    End If
                End If

                If Not Is_EnableMailOTP Then
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
                    HttpContext.Current.Session("Docversion") = oUser.Docdetailversion
                    'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                    ' objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,isauth=1 where userid='" & username & "' and eid=" & HttpContext.Current.Session("EID") & "")
                    objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where userid='" & username & "' and eid=" & HttpContext.Current.Session("EID") & "")
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
                            logger.Error("Exception in LogUserLogin: " & ex.InnerException.Message)
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
                Else
                    'Send OTP By Mail=========================================

                    Dim saAllowedCharacters As String() = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0"}
                    Dim sRandomOTP As String = GenerateRandomOTP(6, saAllowedCharacters)
                    Dim oMailSend As New MailUtill(oUser.EID)

                    Dim ret = oMailSend.SendMail(oUser.emailID, "Verification OTP", "" & sRandomOTP & " is your login otp. Do not share it with anyone.")
                    ret = "Mail Sent"
                    If (ret = "Mail Sent") Then
                        objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET MOTPCode='" & sRandomOTP & "',MOTPsenttime='" & DateTime.Now & "' where uid='" & oUser.UID & "' and eid=" & oUser.EID & "")
                        name.Add("1", "OTP SENT")
                    Else
                        name.Add("1", "OTP NOT SENT")
                    End If
                    '===============================================================

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
        'sURL = "https://noon.myndsaas.com".ToUpper()
        'sURL = "https://hris.myndsaas.com".ToUpper
        sURL = "myndsaas.com".ToUpper()
        'sURL = "https://hfcl.myndsaas.com".ToUpper()
        'sURL = "https://pearlhelpdesk.myndsaas.com".ToUpper()
        'sURL = "https://Chargebee.myndsaas.com".ToUpper()
        'sURL = "https://testchargebee.myndsaas.com".ToUpper()
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
        logger.Debug("Request SURL: " & sURL)

        objDT = objDC.ExecuteQryDT("Select EID,Code,name,logo,isnull(headerimage,'login.jpg') [headerimage],isnull(headerstrip,'myndstrip.jpg') [headerstrip] from MMM_MST_ENTITY where Code='" & sURL & "' ")
        Dim issequelEID As Integer = 0
        Dim name As New Dictionary(Of String, String)()


        logger.Debug("Selected Rows Count: " & objDT.Rows.Count)
        If objDT.Rows.Count = 1 Then
            Dim eidstr As String = ",32,56,57,58,60,66,67,79,85,89,95,"
            If eidstr.Contains("," & objDT.Rows(0).Item("EID").ToString() & ",") Then
                issequelEID = 1
                HttpContext.Current.Response.Redirect("http://" & sURL & ".bpm.sequelone.com")
            End If
            name.Add("1", objDT.Rows(0).Item("Name").ToString())
            '    name.Add("1", "Welcome To " & objDT.Rows(0).Item("Name").ToString())
            name.Add("2", objDT.Rows(0).Item("logo").ToString())
            name.Add("3", objDT.Rows(0).Item("headerimage").ToString())
            HttpContext.Current.Session("HEADERSTRIP") = objDT.Rows(0).Item("headerstrip").ToString()

            logger.Debug("Selected EID: " & objDT.Rows(0).Item("EID"))
            'write code by Mayank
            'If entity id is 190 then move to SSO page start here 
            If (Convert.ToInt32(objDT.Rows(0).Item("EID")) = 190) Then
                Dim xmlDoc As XmlDocument = New XmlDocument()
                xmlDoc.Load(HttpContext.Current.Server.MapPath("/SSOXML/Qwikcilver_MetaData.xml"))
                xmlDoc.DocumentElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"))
                xmlDoc.DocumentElement.SetAttribute("ID", "_" & Guid.NewGuid.ToString())
                Dim SamlXML As String = EncodeSamlAuthnRequest(xmlDoc.InnerXml)
                Dim server As String = Convert.ToString("https://accounts.google.com/o/")
                Dim relativePath As String = server & Convert.ToString("saml2/idp?idpid=C00yykks4&SAMLRequest=") & SamlXML
                'Dim serverUri As Uri = New Uri(server)
                'Dim relativeUri As Uri = New Uri(relativePath, UriKind.Relative)
                'Dim fullUri As Uri = New Uri(serverUri, relativeUri)
                name.Add("SSOURL", relativePath.ToString())
            ElseIf (Convert.ToInt32(objDT.Rows(0).Item("EID")) = 189) Then
                Dim xmlDoc As XmlDocument = New XmlDocument()
                xmlDoc.Load(HttpContext.Current.Server.MapPath("/SSOXML/Chargebee_MetaData.xml"))
                xmlDoc.DocumentElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"))
                xmlDoc.DocumentElement.SetAttribute("ID", "_" & Guid.NewGuid.ToString())
                Dim SamlXML As String = EncodeSamlAuthnRequest(xmlDoc.InnerXml)
                Dim server As String = Convert.ToString("https://accounts.google.com/o/")
                Dim relativePath As String = server & Convert.ToString("saml2/idp?idpid=C00vuvgqi&SAMLRequest=") & SamlXML
                'Dim serverUri As Uri = New Uri(server)
                'Dim relativeUri As Uri = New Uri(relativePath, UriKind.Relative)
                'Dim fullUri As Uri = New Uri(serverUri, relativeUri)
                name.Add("SSOURL", relativePath.ToString())
            ElseIf (Convert.ToInt32(objDT.Rows(0).Item("EID")) = 102) Then
                Dim xmlDoc As XmlDocument = New XmlDocument()
                xmlDoc.Load(HttpContext.Current.Server.MapPath("/SSOXML/TestChargebee_MetaData.xml"))
                xmlDoc.DocumentElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"))
                xmlDoc.DocumentElement.SetAttribute("ID", "_" & Guid.NewGuid.ToString())
                Dim SamlXML As String = EncodeSamlAuthnRequest(xmlDoc.InnerXml)
                Dim server As String = Convert.ToString("https://accounts.google.com/o/")
                Dim relativePath As String = server & Convert.ToString("saml2/idp?idpid=C01ejxwa8&SAMLRequest=") & SamlXML
                'Dim serverUri As Uri = New Uri(server)
                'Dim relativeUri As Uri = New Uri(relativePath, UriKind.Relative)
                'Dim fullUri As Uri = New Uri(serverUri, relativeUri)
                name.Add("SSOURL", relativePath.ToString())
            ElseIf (Convert.ToInt32(objDT.Rows(0).Item("EID")) = 165) Then
                Dim xmlDoc As XmlDocument = New XmlDocument()
                xmlDoc.Load(HttpContext.Current.Server.MapPath("/SSOXML/Noon_MetaData.xml"))
                xmlDoc.DocumentElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"))
                xmlDoc.DocumentElement.SetAttribute("ID", "_" & Guid.NewGuid.ToString())
                Dim SamlXML As String = EncodeSamlAuthnRequest(xmlDoc.InnerXml)
                Dim server As String = Convert.ToString("https://noon.onelogin.com/trust/")
                Dim relativePath As String = server & Convert.ToString("saml2/http-post/sso/2ecea2b3-6f3f-4c10-b953-b9c285661619/?SAMLRequest=") & SamlXML
                'Dim serverUri As Uri = New Uri(server)
                'Dim relativeUri As Uri = New Uri(relativePath, UriKind.Relative)
                'Dim fullUri As Uri = New Uri(serverUri, relativeUri)
                name.Add("SSOURL", relativePath.ToString())
            End If

            'HttpContext.Current.Response.Redirect(fullUri.ToString())
            'If entity id is 190 then move to SSO page End here

        Else
            'If (sURL.ToUpper() <> "PEARLHELPDESK") Then

            'End If
            name.Add("1", "Partner Engagement and Relationship Tool!")
            name.Add("2", "myndlogo.png")
            'name.Add("2", "logo.gif")
            name.Add("3", "invoiceNew.png")
            HttpContext.Current.Session("HEADERSTRIP") = "myndstrip.jpg"
        End If


        If sURL.Length < 2 Then
            'disable text box
            name.Add("EntityName", "VISIBLE")

            'txtEntityName.Visible = True
            'lblEntity.Visible = True
            'txtFEntityName.Visible = True
            'lblEntityName.Visible = True
            'lblMsgAddFolder.Visible = True
            'lbltopmsg.Text = "Please submit your user ID and Customer Code here under. You will recieve an eMail to set up a new password."
            HttpContext.Current.Session("EID") = Nothing
        Else
            'sURL = "pearl"
            ' enable Text Box
            If (sURL.ToUpper() <> "PEARLHELPDESK") Then
                name.Add("EntityName", "INVISIBLE")
                name.Add("EntityCode", sURL)
                Dim obj As New _Default()
                obj.ViewState("EID") = sURL
            Else
                name.Add("EntityName", "VISIBLE")
            End If


            'txtEntityName.Visible = False
            'lblEntity.Visible = False
            'txtFEntityName.Visible = False
            'lblEntityName.Visible = False

            'lblMsgAddFolder.Text = sURL
        End If

        Dim myJsonString As String = (New JavaScriptSerializer()).Serialize(name)
        logger.Debug("GetRequest Response: " & myJsonString)
        Return myJsonString
    End Function
    Public Shared Function EncodeSamlAuthnRequest(ByVal authnRequest As String) As String
        Dim bytes = System.Text.Encoding.UTF8.GetBytes(authnRequest)

        Using output = New System.IO.MemoryStream()

            Using zip = New System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress)
                zip.Write(bytes, 0, bytes.Length)
            End Using

            Dim base64 = Convert.ToBase64String(output.ToArray())
            Return HttpUtility.UrlEncode(base64)
        End Using
    End Function

    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function ValidateOTP(ByVal username As String, ByVal password As String, ByVal entityname As String, ByVal OTP As String) As String
        'entityname = "Pearl"
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        Dim obj As New _Default()
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
        objDT = objDC.ExecuteQryDT("Select EID,Code,ISNULL(EnableMailOTP,0) EnableMailOTP, ISNULL(MOTPExpiry,10) MOTPExpiry,ISNULL(MOTP_EXcludeRoles,'') MOTP_EXcludeRoles from MMM_MST_ENTITY where Code='" & strEntity & "' ")
        Dim TempEid As Integer = objDT.Rows(0).Item("EID")

        Dim name As New Dictionary(Of String, String)()
        If HttpContext.Current.Session("CODE") IsNot Nothing And HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value IsNot Nothing Then
            If HttpContext.Current.Session("CODE").ToString.ToUpper() = strEntity.ToUpper() Then
                name.Add("99", "Another User is already logged In, kindly close the current session or use another browser")
                Dim myJsonString1 As String = (New JavaScriptSerializer()).Serialize(name)
                Return myJsonString1
                Exit Function
            End If
        End If

        Dim oUser As New User
        Select Case oUser.validateUserOTP(strLoginID, strPWD, strEntity, OTP)
            Case 111
                name.Add("111", "The OTP has expired. Please re-send the verification code to try again.")
            Case 100
                'user doesn't exist
                name.Add("100", "Sorry, your User ID is not configured in the system.")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Sorry, your User ID is not configured in the system."

            Case 0
                'user doesn't exist
                name.Add("0", "Sorry, you have entered an invalid OTP. Please try again.")
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
                HttpContext.Current.Session("Docversion") = oUser.Docdetailversion

                'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where userid='" & username & "' and eid=" & HttpContext.Current.Session("EID") & "")

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
                    'Dim cs As String = HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString
                    'objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where userid='" & username & "' and eid=" & HttpContext.Current.Session("EID") & "")
            Case 2
                name.Add("2", "Your password has expired. Please regenerate it!")
                'txtUserID.Text = ""
                'txtPwd.Text = ""
                'lblMsg.Text = "Your password has expired. Please regenerate it."
        End Select
        Dim myJsonString As String = (New JavaScriptSerializer()).Serialize(name)
        Return myJsonString
    End Function

    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function ResendOTP(ByVal username As String, ByVal password As String, ByVal entityname As String) As String
        'entityname = "Pearl"
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        Dim obj As New _Default()
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
        objDT = objDC.ExecuteQryDT("Select EID,Code,ISNULL(EnableMailOTP,0) EnableMailOTP, ISNULL(MOTPExpiry,10) MOTPExpiry,ISNULL(MOTP_EXcludeRoles,'') MOTP_EXcludeRoles from MMM_MST_ENTITY where Code='" & strEntity & "' ")
        Dim TempEid As Integer = objDT.Rows(0).Item("EID")

        Dim name As New Dictionary(Of String, String)()

        If HttpContext.Current.Session("CODE") IsNot Nothing And HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value IsNot Nothing Then
            If HttpContext.Current.Session("CODE").ToString.ToUpper() = strEntity.ToUpper() Then
                name.Add("99", "Another User is already logged In, kindly close the current session or use another browser")
                Dim myJsonString1 As String = (New JavaScriptSerializer()).Serialize(name)
                Return myJsonString1
                Exit Function
            End If
        End If

        Dim objuserDT As New DataTable()
        objuserDT = objDC.ExecuteQryDT("SELECT E.EID, U.UID,emailID,U.userrole FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & strLoginID & "' and E.CODE='" & strEntity & "'")
        Try
            Dim i As Integer
            i = objuserDT.Rows.Count

            Select Case i
                Case 0
                    name.Add("0", "User Not Valid")
                Case 1
                    ' build OTP logic===============================================
                    Dim Is_EnableMailOTP As Boolean = Convert.ToBoolean(objDT.Rows(0)("EnableMailOTP"))
                    Dim OTP_ExcludeRoles = objDT.Rows(0)("MOTP_EXcludeRoles")
                    Dim arrExcludeRoles As String() = OTP_ExcludeRoles.split(","c)
                    Dim IsRolesSkip As Boolean = False
                    If (OTP_ExcludeRoles <> "") Then
                        For k As Integer = 0 To arrExcludeRoles.Length - 1
                            If (arrExcludeRoles(k) = objuserDT.Rows(0)("userrole")) Then
                                IsRolesSkip = True
                                Exit For
                            End If
                        Next
                    End If

                    'Send OTP By Mail=========================================
                    If Not IsRolesSkip Then
                        Dim saAllowedCharacters As String() = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0"}
                        Dim sRandomOTP As String = GenerateRandomOTP(6, saAllowedCharacters)
                        Dim oMailSend As New MailUtill(objuserDT.Rows(0)("EID"))

                        Dim ret = oMailSend.SendMail(objuserDT.Rows(0)("emailID"), "Verification OTP", "" & sRandomOTP & " is your login otp. Do not share it with anyone.")
                        ret = "Mail Sent"
                        If (ret = "Mail Sent") Then
                            objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET MOTPCode='" & sRandomOTP & "',MOTPsenttime='" & DateTime.Now & "' where uid='" & objuserDT.Rows(0)("UID") & "' and eid=" & objuserDT.Rows(0)("EID") & "")
                            name.Add("1", "OTP SENT")
                        Else
                            name.Add("1", "OTP NOT SENT")
                        End If
                    Else
                        name.Add("1", "You not authorize")
                    End If

                    '===============================================================
            End Select
            Dim myJsonString As String = (New JavaScriptSerializer()).Serialize(name)
            Return myJsonString
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Function

    Private Shared Function GenerateRandomOTP(ByVal iOTPLength As Integer, ByVal saAllowedCharacters As String()) As String
        Dim sOTP As String = String.Empty
        Dim sTempChars As String = String.Empty
        Dim rand As Random = New Random()
        For i As Integer = 0 To iOTPLength - 1
            Dim p As Integer = rand.[Next](0, saAllowedCharacters.Length)
            sTempChars = saAllowedCharacters(rand.[Next](0, saAllowedCharacters.Length))
            sOTP += sTempChars
        Next
        Return sOTP
    End Function

End Class
