Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography


Public Class User
    Public UID As Integer
    Public UserName As String
    Public pwd As String
    Public emailID As String
    Public UserRole As String
    Public EmpStatus As Integer
    Public EntityStatus As Integer
    Public domainname As String
    Public UserImage As String
    Public clogo As String
    Public footer As String
    Public EID As String
    Public islocal As String
    Public ipAddress As String
    Public macAddress As String
    Public locationID As Integer
    Public offSet As Double
    Public strCode As String
    Public roles As String
    Public HeaderStrip As String
    Public ExtID As String
    Public CTHEME As String
    Public Docdetailversion As String
    'Encription variables not used in code 
    'Protected enc As System.Text.UTF8Encoding
    'Protected encryptor As ICryptoTransform
    'Protected decryptor As ICryptoTransform

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    ''' <summary>
    ''' this class is written by Manish Kumar
    ''' Date of Creation : 29-01-2007
    ''' Last modified Date : 29-01-2007
    ''' </summary>
    ''' <remarks>
    ''' This class should not be modified by any one without my written permission 
    ''' other wise he will be responsible for not running the application
    ''' </remarks>

#Region "Constuctor"
    Public Sub New()
        
    End Sub
#End Region

#Region "Method"

    Public Function CheckUserIsExist(ByVal emailID As String) As Boolean
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select count(emailid) from MMM_MST_USER where emailid='" & emailID & "'", con)
        Try
            Dim ds As New DataTable()
            oda.Fill(ds)
            con.Close()
            oda.Dispose()
            con.Dispose()
            If Val(ds.Rows(0).Item(0).ToString()) = 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function


    Public Function InsertNewAccount(ByVal AppType As String, ByVal AcCode As String, ByVal AcName As String, ByVal IPAddress As String, ByVal ServerUserID As String, ByVal ServerPWD As String, ByVal UserName As String, ByVal emailID As String, ByVal pwd As String, ByVal localfile As Integer, ByVal sDefFolder As String) As Integer

        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertNewAccount", con)
        Try
            Dim obj As New User
            Dim sKey As Integer
            Dim Generator As System.Random = New System.Random()
            sKey = Generator.Next(10000, 99999)
            Dim strPwd As String = obj.EncryptTripleDES(pwd, sKey)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("AppType", AppType)
            oda.SelectCommand.Parameters.AddWithValue("AcCode", AcCode)
            oda.SelectCommand.Parameters.AddWithValue("AcName", AcName)
            oda.SelectCommand.Parameters.AddWithValue("IPAddress", IPAddress)
            oda.SelectCommand.Parameters.AddWithValue("ServerUserID", ServerUserID)
            oda.SelectCommand.Parameters.AddWithValue("ServerPWD", ServerPWD)
            oda.SelectCommand.Parameters.AddWithValue("UserName", UserName)
            oda.SelectCommand.Parameters.AddWithValue("emailID", emailID)
            oda.SelectCommand.Parameters.AddWithValue("pwd", strPwd)
            oda.SelectCommand.Parameters.AddWithValue("localfilesystem", localfile)
            oda.SelectCommand.Parameters.AddWithValue("sDefFolder", sDefFolder)
            oda.SelectCommand.Parameters.AddWithValue("sKey", sKey)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            If iSt = 5 Then
                Dim mSub As String = "Welcome to eDMS"
                Dim bBody As New StringBuilder()
                bBody.Append("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head>    <title>Welcome To MyeDMS</title></head><body style=""margin: 0; padding: 10px; font: 11px/1.6em  Verdana, Tahoma, Arial,sans-serif; background-color:#fff;color: #555;text-align: center;""><table cellspacing=""0px"" cellpadding=""0px"" width=""100%""><tr><td style=""width:20%"">&nbsp;</td><td style=""width:60%;text-align:left ""><div style=""background-color:#ddd;text-align:left;width:100%;padding:10px""><br />Dear <b>")
                bBody.Append(UserName)

                bBody.Append("</b>,<br /> <br />Welcome to <b>eDMS</b>. Your Account has been confirmed.<br /> <br />Thank you for choosing <b>eDMS</b>. We are committed to serving your needs and are always here to assist you. <br /> <br />Use the following credentials:<br /> <br />Email Address:")
                bBody.Append(emailID & "<br />Password:" & pwd & "<br /> <br />")
                bBody.Append("<br /><br />please activate your account by clicking on following link.<br />")
                bBody.Append("<a href=""http://myedms.myndsolution.com/activate.aspx?UID=JSHUDYHG675GHJSDUY8976KHJSUHDG&UIID=MOJHUKSHGYDTEGJSJAHSGDVJKJJHGDU&UIIID=0QWERASDFZXCVYTHGNB" & iSt.ToString() & """>http://myedms.myndsolution.com/activate.aspx?UID=JSHUDYHG675GHJSDUY8976KHJSUHDG&UIID=MOJHUKSHGYDTEGJSJAHSGDVJKJJHGDU&UIIID=0QWERASDFZXCVYTHGNB" & iSt.ToString() & "</a>")
                bBody.Append("<br /><br />Contact us at manish@myndsol.com if you require assistance with setting your password.<br /><br /><br />Regards,<br />eDMS Team<br />manish@myndsol.com<br />www.myndsol.com</div></td><td style=""width:20%"">&nbsp;</td></tr></table></body></html>")
                Try

                    ' sendMail(emailID, mSub, bBody.ToString())
                    Dim objM As New MailUtill(eid:=EID)
                    objM.SendMail(ToMail:=emailID, Subject:=mSub, MailBody:=bBody.ToString(), CC:="", BCC:="")
                Catch ex As Exception
                    Return iSt
                End Try
            End If
            Return iSt
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try

    End Function

    Public Function MyeDmsPasswordRecover(ByVal un As String, ByVal cCode As String) As String
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspforgetPass", con)
        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("cCode", cCode)
            oda.SelectCommand.Parameters.AddWithValue("userid", un)

            Dim ds As New DataSet
            oda.Fill(ds, "data")
            Dim str As String = ""
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim ist As Integer = oda.SelectCommand.ExecuteScalar()
            If ist = 0 Then
                str = "UserID NOT PRESENT"
            Else
                If ds.Tables("data").Rows(0).Item("isauth") = 0 Then
                    str = "ID LOCKED BY SU"
                Else
                    Dim uid As Integer = ds.Tables("data").Rows(0).Item("uid")
                    Dim ob As DMSUtil = New DMSUtil()
                    ob.notificationMail(uid, ds.Tables("data").Rows(0).Item("EID"), "USER", "FORGET PASSWORD")
                    str = "Successfully"
                End If
            End If
            Return str
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Function

    Private Sub SendChangeMailToGroup()
        Dim mSub As String = "Notification : Change in Your MyeDMS Folder"
        Dim bBody As New StringBuilder()
        bBody.Append("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head>    <title>Welcome To MyeDMS</title></head><body style=""margin: 0; padding: 10px;	font: 11px/1.6em  Verdana, Tahoma, Arial,sans-serif; background-color:#fff;color: #555;text-align: center;""><table cellspacing=""0px"" cellpadding=""0px"" width=""100%""><tr><td style=""width:20%"">&nbsp;</td><td style=""width:60%;text-align:left ""><div style=""background-color:#ddd;text-align:left;width:100%;padding:10px""><br />Dear <b>")
        bBody.Append(UserName)

        bBody.Append("</b>,<br /> <br />Welcome to <b>eDMS</b>. Your Account has been confirmed.<br /> <br />Thank you for choosing <b>eDMS</b>. We are committed to serving your needs and are always here to assist you. <br /> <br />Use the following credentials:<br /> <br />Email Address:")
        bBody.Append("<br /><br />please activate your account by clicking on following link.<br />")
        bBody.Append("<br /><br />Contact us at manish@myndsol.com if you require assistance with setting your password.<br /><br /><br />Regards,<br />eDMS Team<br />manish@myndsol.com<br />www.myndsol.com</div></td><td style=""width:20%"">&nbsp;</td></tr></table></body></html>")
        Try

            Dim Email As New System.Net.Mail.MailMessage("no-reply@mayndsol.com", emailID, mSub, bBody.ToString())
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "smaca")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            mailClient.Send(Email)
        Catch ex As Exception

        End Try
    End Sub

    'Public Sub getValueByEmail(ByVal UN As String, ByVal entity As Integer)
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT uid,username,U.pwd [pwd],U.eid [EID],userrole,U.isauth [EmpStatus],E.isauth [entityStatus],Imagefile,isnull(logo,'mynd.gif') [logo],footer,domainname,localfilesystem,locationID,T.indVal FROM MMM_MST_USER U LEFT OUTER JOIN MMM_MST_ENTITY E on E.eid=U.EID left outer join MMM_MST_LOCATION L on U.locationID=L.locID left outer join MMM_MST_TIMEZONE T on L.ZoneID=T.ZoneID where U.userid='" & UN & "' and U.EID=" & entity & "", con)
    '    Dim ds As New DataSet
    '    oda.Fill(ds, "user")
    '    If ds.Tables("user").Rows.Count = 1 Then
    '        'User Is Found now Initilized all values
    '        emailID = UN
    '        UID = ds.Tables("user").Rows(0).Item("UID")
    '        UserName = ds.Tables("user").Rows(0).Item("Username")
    '        UserRole = ds.Tables("user").Rows(0).Item("userrole").ToString()
    '        EmpStatus = Val(ds.Tables("user").Rows(0).Item("empstatus").ToString())
    '        EntityStatus = Val(ds.Tables("user").Rows(0).Item("entitystatus").ToString())
    '        domainname = ds.Tables("user").Rows(0).Item("Domainname").ToString()
    '        clogo = ds.Tables("user").Rows(0).Item("logo").ToString()
    '        UserImage = ds.Tables("user").Rows(0).Item("Imagefile").ToString()
    '        footer = ds.Tables("user").Rows(0).Item("footer").ToString()
    '        EID = ds.Tables("user").Rows(0).Item("EID").ToString()
    '        islocal = ds.Tables("user").Rows(0).Item("localfilesystem").ToString()
    '        locationID = Val(ds.Tables("user").Rows(0).Item("locationID").ToString())
    '        ipAddress = GetIPAddress()
    '        macAddress = getMacAddress()
    '        offSet = ds.Tables("user").Rows(0).Item("indVal").ToString()
    '    End If
    '    con.Close()
    '    oda.Dispose()
    '    con.Dispose()
    'End Sub

    Public Sub getValueByEmail(ByVal UN As String, ByVal entity As Integer)
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT extid, uid,username,emailid,U.pwd [pwd],U.eid [EID],userrole,U.isauth [EmpStatus],E.isauth [entityStatus],E.Code,E.Headerstrip ,Imagefile,isnull(logo,'mynd.gif') [logo],footer,domainname,localfilesystem,locationID,T.indVal,e.ctheme,e.DocDeatilVersion FROM MMM_MST_USER U LEFT OUTER JOIN MMM_MST_ENTITY E on E.eid=U.EID left outer join MMM_MST_LOCATION L on U.locationID=L.locID left outer join MMM_MST_TIMEZONE T on L.ZoneID=T.ZoneID where U.userid='" & UN & "' and U.EID=" & entity & "", con)
        Dim ds As New DataSet
        Try
            oda.Fill(ds, "user")
            If ds.Tables("user").Rows.Count = 1 Then
                'User Is Found now Initilized all values
                emailID = UN
                UID = ds.Tables("user").Rows(0).Item("UID")
                emailID = ds.Tables("user").Rows(0).Item("emailid").ToString()
                UserName = ds.Tables("user").Rows(0).Item("Username")
                UserRole = UCase(ds.Tables("user").Rows(0).Item("userrole").ToString())
                EmpStatus = Val(ds.Tables("user").Rows(0).Item("empstatus").ToString())
                EntityStatus = Val(ds.Tables("user").Rows(0).Item("entitystatus").ToString())
                domainname = ds.Tables("user").Rows(0).Item("Domainname").ToString()
                clogo = ds.Tables("user").Rows(0).Item("logo").ToString()
                UserImage = ds.Tables("user").Rows(0).Item("Imagefile").ToString()
                footer = ds.Tables("user").Rows(0).Item("footer").ToString()
                EID = ds.Tables("user").Rows(0).Item("EID").ToString()
                islocal = ds.Tables("user").Rows(0).Item("localfilesystem").ToString()
                locationID = Val(ds.Tables("user").Rows(0).Item("locationID").ToString())
                ipAddress = GetIPAddress()
                macAddress = getMacAddress()
                offSet = ds.Tables("user").Rows(0).Item("indVal").ToString()
                strCode = ds.Tables("user").Rows(0).Item("Code").ToString()
                HeaderStrip = ds.Tables("user").Rows(0).Item("Headerstrip").ToString()
                ExtID = ds.Tables("user").Rows(0).Item("extid").ToString()
                CTHEME = ds.Tables("user").Rows(0).Item("ctheme").ToString()
                Docdetailversion = ds.Tables("user").Rows(0).Item("DocDeatilVersion").ToString()
                ''fetch role from role definition
                Dim str As String = ""
                str &= "select distinct rolename [userrole] from MMM_REF_ROLE_USER where eid=" & EID & " AND UID=" & UID & " union select rolename from MMM_ref_PreRole_user where eid=" & EID & " AND UID=" & UID & ""
                oda.SelectCommand.CommandText = str
                oda.Fill(ds, "Roles")

                For Each dr As DataRow In ds.Tables("Roles").Rows
                    If dr.Item("userrole").ToString.ToUpper <> UserRole.ToUpper() Then
                        roles &= UCase(dr.Item("userrole").ToString()) & ","
                    End If
                Next
                roles &= UserRole
            End If
            
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
#Region "UserLoingLog"

    Private Function GetUserMacAddress() As String
        Dim now As DateTime = DateTime.Now
        Dim str As String = ""
        Try
            For Each nic As System.Net.NetworkInformation.NetworkInterface In System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                str += "," + nic.GetPhysicalAddress().ToString()
            Next

            '        str = Right(str, 12)
            Return str
        Catch ex As Exception
            Return "Client Deneid MAc Retireval Request"
        End Try
    End Function

    Private Function GetUserIPAddress() As String
        Dim IP As String = If(HttpContext.Current.Request.Params("HTTP_CLIENT_IP"), HttpContext.Current.Request.UserHostAddress)
        Return IP
    End Function

    Public Function LogUserLogin(EID As Integer, UID As Integer) As String
        Dim ret = "fail"
        Dim IPAddress As String
        Dim MacAddress As String
        Try
            IPAddress = GetUserIPAddress()
            MacAddress = GetUserMacAddress()
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("logUserLogin", con)
                    con.Open()
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@UID", UID)
                    da.SelectCommand.Parameters.AddWithValue("@IPAddress", IPAddress)
                    da.SelectCommand.Parameters.AddWithValue("@MacAddress", MacAddress)
                    Dim res As Integer = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
                    If res > 0 Then
                        ret = "Success"
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return "fail"
        End Try
        Return ret
    End Function

    Public Function UpdateUserLoginLog(EID As Integer, UID As Integer) As String
        Dim ret = "fail"
        Dim IPAddress As String
        Dim MacAddress As String
        Try
            IPAddress = GetUserIPAddress()
            MacAddress = GetUserMacAddress()
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("UpdateUserLoginLog", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    con.Open()
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@UID", UID)
                    Dim res As Integer = 0 ' Convert.ToInt32(da.SelectCommand.ExecuteScalar())
                    If res > 0 Then
                        ret = "Success"
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return "fail"
        End Try
        Return ret
    End Function
#End Region

    Private Function getMacAddress() As String
        Dim str As String = ""
        Try
            For Each nic As System.Net.NetworkInformation.NetworkInterface In System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                str &= "," & nic.GetPhysicalAddress().ToString()
            Next
            Return str

            '        str = Right(str, 12)
        Catch ex As Exception
            Return "Client Deneid MAc Retireval Request"
        End Try
    End Function

    Private Function GetIPAddress() As String
        Dim strHostName As String
        Dim strIPAddress As String
        strHostName = System.Net.Dns.GetHostName()
        strIPAddress = System.Net.Dns.GetHostByName(strHostName).AddressList(0).ToString()
        Return strHostName & "( " & strIPAddress & ")"
    End Function

    Public Sub SSOValidateUser(ByVal emailid As String, ByVal entity As Integer)
        getValueByEmail(emailid, entity)
    End Sub

    Public Function validateUser(ByVal UN As String, ByVal pwd As String, ByVal ecode As String) As Integer
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim sqlq As String
        sqlq = "SELECT E.Ctheme, u.pwd [pwd],u.isauth [uisauth],u.sKey,minPassAttempt,passtry,passExpDays,passExpMsgDays,autoUnlockHour,datediff(hour,ModifyDate,getdate()) [hourElapsed],locationID,E.EID,E.MenuLogTo,E.MenuLogCC FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & UN & "' and E.CODE='" & ecode & "'"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
        Try
            Dim ds As New DataSet
            oda.Fill(ds, "user")
            Dim i As Integer
            i = ds.Tables("user").Rows.Count

            Select Case i
                Case 0
                    Return 0
                Case 1
                    Dim enitity As Integer = Val(ds.Tables("user").Rows(0).Item("EID").ToString())
                    Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
                    Dim sPwd As String = ""
                    If sKey <> "" And Convert.ToString(ds.Tables("user").Rows(0).Item("pwd")) <> "" Then
                        sPwd = DecryptTripleDES(Convert.ToString(ds.Tables("user").Rows(0).Item("pwd")), sKey)
                    End If
                    HttpContext.Current.Session("CTheme") = Convert.ToString(ds.Tables("user").Rows(0).Item("CTHEME").ToString())
                    'Dim sPwd As String = ds.Tables("user").Rows(0).Item("pwd").ToString()
                    HttpContext.Current.Session("MenuLogTo") = Convert.ToString(ds.Tables("user").Rows(0).Item("MenuLogTo").ToString())
                    HttpContext.Current.Session("MenuLogCC") = Convert.ToString(ds.Tables("user").Rows(0).Item("MenuLogCC").ToString())


                    If sPwd = pwd Then
                        'If Val(ds.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(ds.Tables("user").Rows(0).Item("passtry").ToString()) Then
                        '    'password retry reached to limit set isauth to lock mode and exit sub
                        '    Return 4
                        '    Exit Function
                        'End If

                        Select Case Val(ds.Tables("user").Rows(0).Item("uisauth").ToString())
                            Case 100
                                Return 100

                            Case 0
                                Return 7


                            Case 1
                                If Val(ds.Tables("user").Rows(0).Item("passExpMsgDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                                    'Send him a message to change the password
                                    HttpContext.Current.Session("MESSAGE") = "Please change your password, Your password will be expired soon"
                                End If

                                If Val(ds.Tables("user").Rows(0).Item("passExpDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                                    Return 10
                                    'Lock the password and send message password expired
                                Else
                                    getValueByEmail(UN, enitity)
                                    Return 1
                                End If

                            Case 3
                                If Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) >= Val(ds.Tables("user").Rows(0).Item("autoUnlockHour").ToString()) Then
                                    'Unlock and let him login - change the isauth to 1
                                    getValueByEmail(UN, enitity)
                                    Return 1
                                Else
                                    Return 5
                                End If
                            Case 2
                                Return 6
                            Case Else
                                Return 2
                        End Select

                    Else
                        If Val(ds.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(ds.Tables("user").Rows(0).Item("passtry").ToString()) Then
                            'password retry reached to limit set isauth to lock mode and exit sub
                            Return 4
                            Exit Function
                        End If
                        Return 3
                    End If
                Case Else
                    Return 0
            End Select
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Function

    Public Function validateUserOTP(ByVal UN As String, ByVal pwd As String, ByVal ecode As String, ByVal OTP As String) As Integer
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim sqlq As String
        sqlq = "SELECT E.Ctheme,MOTPsenttime,ISNULL(MOTPExpiry,10) MOTPExpiry,u.pwd [pwd],u.isauth [uisauth],u.sKey,minPassAttempt,passtry,passExpDays,passExpMsgDays,autoUnlockHour,datediff(hour,ModifyDate,getdate()) [hourElapsed],locationID,E.EID,E.MenuLogTo,E.MenuLogCC FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & UN & "' and MOTPCode ='" & OTP & "' and E.CODE='" & ecode & "'"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
        Try
            Dim ds As New DataSet
            oda.Fill(ds, "user")
            Dim i As Integer
            i = ds.Tables("user").Rows.Count

            Select Case i
                Case 0
                    Return 0
                Case 1
                    Dim date1 As DateTime = Convert.ToDateTime(ds.Tables("user").Rows(0)("MOTPsenttime"))
                    Dim date2 As DateTime = DateTime.Now
                    Dim ts As TimeSpan = date2.Subtract(date1)
                    If (ts.TotalMinutes > Convert.ToInt32(ds.Tables("user").Rows(0)("MOTPExpiry"))) Then
                        Return 111
                    End If
                    Dim enitity As Integer = Val(ds.Tables("user").Rows(0).Item("EID").ToString())
                    Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
                    Dim sPwd As String = ""
                    If sKey <> "" And Convert.ToString(ds.Tables("user").Rows(0).Item("pwd")) <> "" Then
                        sPwd = DecryptTripleDES(Convert.ToString(ds.Tables("user").Rows(0).Item("pwd")), sKey)
                    End If
                    HttpContext.Current.Session("CTheme") = Convert.ToString(ds.Tables("user").Rows(0).Item("CTHEME").ToString())
                    'Dim sPwd As String = ds.Tables("user").Rows(0).Item("pwd").ToString()
                    HttpContext.Current.Session("MenuLogTo") = Convert.ToString(ds.Tables("user").Rows(0).Item("MenuLogTo").ToString())
                    HttpContext.Current.Session("MenuLogCC") = Convert.ToString(ds.Tables("user").Rows(0).Item("MenuLogCC").ToString())


                    If sPwd = pwd Then
                        'If Val(ds.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(ds.Tables("user").Rows(0).Item("passtry").ToString()) Then
                        '    'password retry reached to limit set isauth to lock mode and exit sub
                        '    Return 4
                        '    Exit Function
                        'End If

                        Select Case Val(ds.Tables("user").Rows(0).Item("uisauth").ToString())
                            Case 100
                                Return 100

                            Case 0
                                Return 7


                            Case 1
                                If Val(ds.Tables("user").Rows(0).Item("passExpMsgDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                                    'Send him a message to change the password
                                    HttpContext.Current.Session("MESSAGE") = "Please change your password, Your password will be expired soon"
                                End If

                                If Val(ds.Tables("user").Rows(0).Item("passExpDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                                    Return 10
                                    'Lock the password and send message password expired
                                Else
                                    getValueByEmail(UN, enitity)
                                    Return 1
                                End If

                            Case 3
                                If Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) >= Val(ds.Tables("user").Rows(0).Item("autoUnlockHour").ToString()) Then
                                    'Unlock and let him login - change the isauth to 1
                                    getValueByEmail(UN, enitity)
                                    Return 1
                                Else
                                    Return 5
                                End If
                            Case 2
                                Return 6
                            Case Else
                                Return 2
                        End Select

                    Else
                        If Val(ds.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(ds.Tables("user").Rows(0).Item("passtry").ToString()) Then
                            'password retry reached to limit set isauth to lock mode and exit sub
                            Return 4
                            Exit Function
                        End If
                        Return 3
                    End If
                Case Else
                    Return 0
            End Select
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Function


    '''' - bkup 26-apr-17
    'Public Function validateUser(ByVal UN As String, ByVal pwd As String, ByVal ecode As String) As Integer
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim sqlq As String
    '    sqlq = "SELECT E.Ctheme, u.pwd [pwd],u.isauth [uisauth],u.sKey,minPassAttempt,passtry,passExpDays,passExpMsgDays,autoUnlockHour,datediff(hour,ModifyDate,getdate()) [hourElapsed],locationID,E.EID FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & UN & "' and E.CODE='" & ecode & "'"
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
    '    Try
    '        Dim ds As New DataSet
    '        oda.Fill(ds, "user")
    '        Dim i As Integer
    '        i = ds.Tables("user").Rows.Count

    '        Select Case i
    '            Case 0
    '                Return 0
    '            Case 1
    '                Dim enitity As Integer = Val(ds.Tables("user").Rows(0).Item("EID").ToString())
    '                Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
    '                Dim sPwd As String = DecryptTripleDES(ds.Tables("user").Rows(0).Item("pwd"), sKey)
    '                HttpContext.Current.Session("CTheme") = Convert.ToString(ds.Tables("user").Rows(0).Item("CTHEME").ToString())
    '                'Dim sPwd As String = ds.Tables("user").Rows(0).Item("pwd").ToString()

    '                If sPwd = pwd Then
    '                    If Val(ds.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(ds.Tables("user").Rows(0).Item("passtry").ToString()) Then
    '                        'password retry reached to limit set isauth to lock mode and exit sub
    '                        Return 4
    '                        Exit Function
    '                    End If

    '                    Select Case Val(ds.Tables("user").Rows(0).Item("uisauth").ToString())
    '                        Case 100
    '                            Return 100

    '                        Case 0
    '                            Return 7


    '                        Case 1
    '                            If Val(ds.Tables("user").Rows(0).Item("passExpMsgDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
    '                                'Send him a message to change the password
    '                                HttpContext.Current.Session("MESSAGE") = "Please change your password, Your password will be expired soon"
    '                            End If

    '                            If Val(ds.Tables("user").Rows(0).Item("passExpDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
    '                                Return 10
    '                                'Lock the password and send message password expired
    '                            Else
    '                                getValueByEmail(UN, enitity)
    '                                Return 1
    '                            End If

    '                        Case 3
    '                            If Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) >= Val(ds.Tables("user").Rows(0).Item("autoUnlockHour").ToString()) Then
    '                                'Unlock and let him login - change the isauth to 1
    '                                getValueByEmail(UN, enitity)
    '                                Return 1
    '                            Else
    '                                Return 5
    '                            End If
    '                        Case 2
    '                            Return 6
    '                        Case Else
    '                            Return 2
    '                    End Select

    '                Else
    '                    Return 3
    '                End If
    '            Case Else
    '                Return 0
    '        End Select
    '    Catch ex As Exception
    '        Throw
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try
    'End Function

    Public Function RecoverPassword(ByVal UN As String, ByVal SQ As String, ByVal sp As String) As String
        Dim sRes As String = ""
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim sqlq As String
        sqlq = "SELECT * FROM TDMS_MST_USERS where userid='" & UN & "' and seqq='" & SQ & "' and seqa='" & sp & "'"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
        Try
            Dim ds As New DataSet
            oda.Fill(ds, "user")
            Dim i As Integer
            i = ds.Tables("user").Rows.Count
            If i = 1 Then
                'user is authenticated now geenrate a random number fetch his email address 
                'genrate random number
                Dim Uname As String = ds.Tables("user").Rows(0).Item("userName")
                Dim em As String = ds.Tables("user").Rows(0).Item("emailid")
                Dim rnd As Random = New Random
                Dim pwd As String = rnd.Next(10000, 99999)
                Dim strPwd As String = pwd
                Dim sKey As String = rnd.Next(10000, 99999)
                oda.SelectCommand.CommandText = "update TDMS_MST_USERS set pwd='" & pwd & "',skey='" & sKey & "',isAuth=0 where userid='" & UN & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()


                Dim mSub, mMsg As String
                mSub = "Recovered Password"
                mMsg = " <p style=""color: Steelblue""> Dear <b> " & Uname & "</b></p>"
                mMsg += "<p> Your password is <b> """ & strPwd & """.</b></p>"

                mMsg += "<br>"
                mMsg += "<br>"

                mMsg += "You must change you password on first login."

                mMsg += "<br>"

                mMsg += "<br>" + "<font color=steelblue>Regards"
                mMsg += "<br>" + "<b>Mynd Solution Pvt Ltd</font></b>"
                ' sendMail(em, mSub, mMsg)
                Dim objM As New MailUtill(eid:=EID)
                objM.SendMail(ToMail:=emailID, Subject:=mSub, MailBody:=mMsg, CC:="", BCC:="")
                Return "Dear " & Uname & ", You Password sent to your e-mail ID"
            Else
                Return ("Invalid Input")
            End If
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try

    End Function

    Public Function ActivateUser(ByVal UID As Integer) As String
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Select isauth from MMM_MST_USER where uid=" & UID, con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim rettype As String = oda.SelectCommand.ExecuteScalar()

            If rettype = "100" Or rettype = "3" Or rettype = "2" Then
                oda.SelectCommand.CommandText = "UPDATE MMM_MST_USER SET isauth=1 where uid=" & UID
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            End If


            ' If rettype = "100" Then
            Return "A"
            '  Else
            '  Return "AA"
            '  End If
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function


    'Private Sub sendMail(ByVal Mto As String, ByVal MSubject As String, ByVal MBody As String)
    '    Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
    '    Dim mailClient As New System.Net.Mail.SmtpClient()
    '    Email.IsBodyHtml = True
    '    Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "smaca")
    '    mailClient.Host = "mail.myndsol.com"
    '    mailClient.UseDefaultCredentials = False
    '    mailClient.Credentials = basicAuthenticationInfo
    '    'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
    '    mailClient.Send(Email)
    'End Sub

    Public Function isAlphaNumeric(ByVal strToCheck As String) As [Boolean]
        Dim isnumeric As Boolean = False

        For Each c As Char In strToCheck
            If Char.IsNumber(c) Then
                isnumeric = True
            End If
        Next
        Return isnumeric
    End Function

    Public Function isAlphaNumericAndCapital(ByVal strToCheck As String) As [Boolean]
        Dim isnumeric As Boolean = False
        Dim isOneUper As Boolean = False

        For Each c As Char In strToCheck
            If Char.IsNumber(c) Then
                isnumeric = True
            End If

            If Char.IsUpper(c) Then
                isOneUper = True
            End If
        Next
        Return isnumeric And isOneUper
    End Function

    Public Function isAlphaNumericAndSpecial(ByVal strToCheck As String) As [Boolean]
        Dim isnumeric As Boolean = False
        Dim isOneSpecial As Boolean = False

        For Each c As Char In strToCheck
            If Char.IsNumber(c) Then
                isnumeric = True
            End If
            '  If System.Text.RegularExpressions.Regex.IsMatch(c, "^[a-zA-Z0-9\x20]+$") Then

            'If Char.IsSymbol(c) Then
            '    isOneSpecial = True
            'End If
        Next
        Dim str = "!, -, #, $, % ,& , (,), *, +, .,/,:,;, <, =,>,?, @,[,],^,_,`,{,|,},~,"
        Dim arr As String() = str.Split(",")
        For s As Integer = 0 To arr.Length
            Dim s1 = arr(s).Trim()
            If strToCheck.Contains(s1) Then
                isOneSpecial = True
                Exit For
            End If
        Next
        Return isnumeric And isOneSpecial
    End Function

    '' prev 
    'Public Function isAlphaNumericAndSpecial(ByVal strToCheck As String) As [Boolean]
    '    Dim isnumeric As Boolean = False
    '    Dim isOneSpecial As Boolean = False

    '    For Each c As Char In strToCheck
    '        If Char.IsNumber(c) Then
    '            isnumeric = True
    '        End If
    '        '  If System.Text.RegularExpressions.Regex.IsMatch(c, "^[a-zA-Z0-9\x20]+$") Then

    '        If Char.IsSymbol(c) Then
    '            isOneSpecial = True
    '        End If
    '    Next
    '    Return isnumeric And isOneSpecial
    'End Function

    Public Function ChangePassword(ByVal iUID As Integer, ByVal cpwd As String, ByVal npwd As String) As String
        Dim sRes As String = ""
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim sqlq As String
        sqlq = "Select minchar,maxchar,passtype,isnull(PassHisChkCount,3) as PassHisChkCount from MMM_MST_ENTITY where EID=" & HttpContext.Current.Session("EID").ToString()
        Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
        Try
            Dim ds As New DataSet
            oda.Fill(ds, "setting")

            Dim PassHisChkCnt As Integer = Val(ds.Tables("setting").Rows(0).Item("PassHisChkCount").ToString())
            If PassHisChkCnt < 3 Then PassHisChkCnt = 3

            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandText = "select top " & ds.Tables("setting").Rows(0).Item("PassHisChkCount").ToString() & " * from mmm_mst_password_history where Uid=" & iUID & " and eid=" & HttpContext.Current.Session("EID") & "  order by entrydate desc"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dtR As New DataTable
            oda.Fill(dtR)

            'If dtR.Rows.Count = PassHisChkCnt Then
            For c As Integer = 0 To dtR.Rows.Count - 1
                If npwd = Convert.ToString(dtR.Rows(c).Item("pwdused")) Then
                    Return ("Password must be different from last " & ds.Tables("setting").Rows(0).Item("PassHisChkCount").ToString() & " used Passwords!")
                End If
            Next
            ' End If

            'prev
            'If dtR.Rows(0).Item(0) <> 0 Then
            '    Return ("Password is already used by you earlier, hence choose new password!")
            'End If

            If npwd.Length() < Val(ds.Tables("setting").Rows(0).Item("minchar").ToString()) Then
                Return ("Password must be " & ds.Tables("setting").Rows(0).Item("minchar").ToString() & " Character Long")
            End If

            If npwd.Length() > Val(ds.Tables("setting").Rows(0).Item("maxchar").ToString()) Then
                Return ("Password must not be greater than " & ds.Tables("setting").Rows(0).Item("maxchar").ToString() & " Character Long")
            End If

            Select Case ds.Tables("setting").Rows(0).Item("passtype").ToString()
                Case "NORMAL"
                    ' do nothing
                Case "ALPHA NUMERIC"

                    If Not isAlphaNumeric(npwd) Then
                        Return ("Password must be alphanumeric ")
                    End If
                    'must be alphanumeric

                Case "ALPHA NUMERIC WITH CAPS LETTER"
                    'must be alphanumeric with one Capital letter
                    If Not isAlphaNumericAndCapital(npwd) Then
                        Return ("Password must be alphanumeric and contains atleast one capital letter ")
                    End If
                Case "ALPHA NUMERIC WITH SPECIAL CHARACTER"
                    If Not isAlphaNumericAndSpecial(npwd) Then
                        Return ("Password must be alphanumeric and must contain one special character ")
                    End If
            End Select
            sqlq = "SELECT * FROM MMM_MST_USER where uid=" & iUID
            oda.SelectCommand.CommandText = sqlq
            oda.Fill(ds, "user")

            Dim i As Integer
            i = ds.Tables("user").Rows.Count
            If i = 1 Then

                Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
                Dim sPwd As String = DecryptTripleDES(ds.Tables("user").Rows(0).Item("pwd"), sKey)

                Dim newPwd As String = npwd
                If sPwd = npwd Then
                    Return ("Current and New password cannot be Same")
                ElseIf sPwd = cpwd Then
                    Dim ssKey As Integer
                    Dim Generator As System.Random = New System.Random()
                    sKey = Generator.Next(10000, 99999)

                    Dim strPwd As String = EncryptTripleDES(npwd, ssKey)
                    'Password matched. now changed the password
                    oda.SelectCommand.CommandText = "UPDATE MMM_MST_USER SET pwd='" & strPwd & "',modifydate=getdate(),sKey='" & ssKey & "' where uid=" & iUID
                    oda.SelectCommand.Parameters.Clear()
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()

                    ' ##### for inserting new password in history ##### by sp - 25-apr-17
                    oda.SelectCommand.CommandText = "insert into mmm_mst_password_history (eid,uid,pwdused) values (" & HttpContext.Current.Session("EID") & "," & iUID & ",'" & npwd & "')"
                    oda.SelectCommand.Parameters.Clear()
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    ' ##### for inserting new password in history ##### by sp

                    Return ("Password changed successfully")
                Else
                    'password(doesn) 't matched
                    Return ("Password Doesn't matched")
                End If
            Else
                Return ("User Not Found")
            End If
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Function

    Public Sub LockUser(ByVal userid As String)
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("update TDMS_MST_USERS set isauth=2,lastlogindate=getdate() where userid='" & userid & "'", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        con.Dispose()
    End Sub

    Public Function EncryptTripleDES(ByVal sIn As String, ByVal sKey As String) As String
        Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
        Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

        ' scramble the key
        sKey = ScrambleKey(sKey)
        ' Compute the MD5 hash.
        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
        ' Set the cipher mode.
        DES.Mode = System.Security.Cryptography.CipherMode.ECB
        ' Create the encryptor.
        Dim DESEncrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateEncryptor()
        ' Get a byte array of the string.
        Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn)
        ' Transform and return the string.
        Return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function

    Public Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
        Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
        Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

        ' scramble the key
        sKey = ScrambleKey(sKey)
        ' Compute the MD5 hash.
        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
        ' Set the cipher mode.
        DES.Mode = System.Security.Cryptography.CipherMode.ECB
        ' Create the decryptor.
        Dim DESDecrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateDecryptor()
        Dim Buffer As Byte() = Convert.FromBase64String(sOut)
        ' Transform and return the string.
        Return System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function

    Private Function ScrambleKey(ByVal v_strKey As String) As String
        Dim sbKey As New System.Text.StringBuilder
        Dim intPtr As Integer
        For intPtr = 1 To v_strKey.Length
            Dim intIn As Integer = v_strKey.Length - intPtr + 1
            sbKey.Append(Mid(v_strKey, intIn, 1))
        Next
        Dim strKey As String = sbKey.ToString
        Return sbKey.ToString
    End Function

    Public Function EmailAddressCheck(ByVal emailAddress As String) As Boolean

        Dim pattern As String = "^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"
        Dim emailAddressMatch As Match = Regex.Match(emailAddress, pattern)
        If emailAddressMatch.Success Then
            EmailAddressCheck = True
        Else
            EmailAddressCheck = False
        End If
    End Function

    ''Function To Validate Page according to User (Coded By Ravi Sharma)
    Public Function validatePage(ByVal code As String, ByVal userrole As String, ByVal pageurl As String) As Boolean
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim str As String = code & "_" & userrole
        Dim dataadp As New SqlDataAdapter("", con)
        Try

            dataadp.SelectCommand.CommandType = CommandType.Text
            dataadp.SelectCommand.CommandText = "select pagename,menuname,menutype  from mmm_mst_accessmenu where  pagename='" & pageurl & "' And " & str & " <> '0'"
            Dim ds As New DataSet()
            dataadp.Fill(ds, "user")
            If ds.Tables("user").Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            dataadp.Dispose()
        End Try
    End Function

#End Region

End Class

