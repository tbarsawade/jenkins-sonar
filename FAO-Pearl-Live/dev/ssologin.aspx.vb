Imports System.Data
Imports System.Data.SqlClient

Partial Class ssologin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        pnlsso.Visible = False
        If Request.QueryString("emailid") Is Nothing Then
            'Response.Redirect("default.aspx")
        Else
            Dim emailID As String = Request.QueryString("emailid").ToString().Replace(" ", "+")
            Dim decryptdata = DecryptTripleDES(emailID, "0")
            If Not decryptdata.Contains("|") Then
                ' here we have to pass UserID in the parameter SSOValidator because we change login code . 
                emailID = Request.QueryString("emailid").ToString()
                Dim EID As Integer = Val(Request.QueryString("EID").ToString())
                Dim retURl As String = Request.QueryString("returl").ToString()

                Dim oUser As New User
                oUser.SSOValidateUser(emailID, EID)

                If oUser.UID > 0 Then
                    Session("UID") = oUser.UID
                    Session("USERNAME") = oUser.UserName
                    Session("CTHEME") = oUser.CTHEME
                    Session("USERROLE") = oUser.UserRole
                    'Session("Roles") = oUser.UserRole
                    Session("USERIMAGE") = oUser.UserImage
                    Session("CLOGO") = oUser.clogo
                    Session("EID") = oUser.EID
                    Session("IPADDRESS") = oUser.ipAddress
                    Session("MACADDRESS") = oUser.macAddress
                    Session("RETURL") = retURl

                    Session("USERROLE") = oUser.UserRole
                    Session("EMAIL") = oUser.emailID
                    Session("HEADERSTRIP") = ViewState("HEADERSTRIP")
                    Session("EXTID") = oUser.ExtID

                    Session("INTIME") = Now
                    Session("LID") = oUser.locationID
                    Session("OFFSET") = oUser.offSet
                    Session("CODE") = oUser.strCode
                    Session("Roles") = oUser.roles

                    If oUser.islocal = 0 Then
                        Session("ISLOCAL") = "TRUE"
                    Else
                        Session("ISLOCAL") = "TRUE"
                    End If
                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim str As String = ""
                    Str = "UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where UID=" & Session("UID") & " and eid=" & HttpContext.Current.Session("EID") & ""
                    Using con = New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(Str, con)
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            da.SelectCommand.ExecuteNonQuery()
                            con.Dispose()
                        End Using
                    End Using
                    Response.Redirect("~/Explorer.aspx")
                Else
                    Response.Redirect("~/Default.aspx")
                End If
            Else
                RedirectionForTicket(decryptdata.ToString())
            End If

        End If

        If Request.QueryString("Token") Is Nothing Then
            'Response.Redirect("Default.aspx")
        Else
            Dim objdataclass As New DataClass
            Dim dt As New DataTable
            Dim ouser As New User

            dt = objdataclass.GetSSNLoginEntry(Request.QueryString("Token"))

            If (dt.Rows.Count > 0) Then

                Session("UID") = Convert.ToString(dt.Rows(0)("uid"))
                Session("USERNAME") = Convert.ToString(dt.Rows(0)("UserName"))
                Session("USERROLE") = Convert.ToString(dt.Rows(0)("Userrole"))
                Session("CODE") = Convert.ToString(dt.Rows(0)("Code"))
                Session("USERIMAGE") = Convert.ToString(dt.Rows(0)("Imagefile"))
                Session("CLOGO") = Convert.ToString(dt.Rows(0)("logo"))
                Session("EID") = Convert.ToString(dt.Rows(0)("eid"))
                Session("ISLOCAL") = Convert.ToString(dt.Rows(0)("localfilesystem"))
                Session("INTIME") = Now
                Session("EMAIL") = Convert.ToString(dt.Rows(0)("emailID"))
                Session("LID") = Convert.ToString(dt.Rows(0)("locationID"))
                Session("HEADERSTRIP") = Convert.ToString(dt.Rows(0)("headerstrip"))
                Session("ROLES") = Convert.ToString(dt.Rows(0)("Userrole"))
                objdataclass.updateSSNLoginEntry(Request.QueryString("Token"))

                'Session("USERROLE") = ouser.UserRole
                'Session("EMAIL") = ouser.emailID
                'Session("HEADERSTRIP") = ViewState("HEADERSTRIP")
                ' Session("EXTID") = ouser.ExtID

                'Session("INTIME") = Now
               
                'Session("CODE") = ouser.strCode

                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim str As String = ""
                str &= "select distinct rolename [userrole] from MMM_REF_ROLE_USER where eid=" & Session("EID") & " AND UID=" & Session("UID") & " union select rolename from MMM_ref_PreRole_user where eid=" & Session("EID") & " AND UID=" & Session("UID") & ""
                Dim ds As New DataSet()
                Using con = New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(str, con)
                        da.Fill(ds)
                    End Using
                End Using
                Dim Roles = ""
                For Each dr As DataRow In ds.Tables(0).Rows
                    If dr.Item("userrole").ToString.ToUpper <> Session("USERROLE").ToUpper() Then
                        Roles &= UCase(dr.Item("userrole").ToString()) & ","
                    End If
                Next
                Roles &= Session("USERROLE").ToUpper()
                Session("LID") = ouser.locationID
                Session("OFFSET") = ouser.offSet
                Session("Roles") = Roles
                'Dim objDC As New DataClass()
                str = "UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where UID=" & Session("UID") & " and eid=" & HttpContext.Current.Session("EID") & ""
                Using con = New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(str, con)
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                        con.Dispose()
                    End Using
                End Using
                Response.Redirect("Home.aspx")

            Else
                ' lblMsg.Text = "Your login request failed , Please check your token."
                pnlsso.Visible = True
            End If


        End If



    End Sub

    Public Function RedirectionForTicket(ByVal obj As String) As String
        Dim emailAPI As String() = obj.Split("|")
        Dim objDataClass As New DataClass()
        Dim emailID As String = ""
        Dim EID As Integer = objDataClass.GetEIDBasedOnAPIKEY(emailAPI(0))
        Dim retURl As String = ""
        If EID <> 0 Then
            emailID = emailAPI(1)
            Dim oUser As New User
            oUser.SSOValidateUser(emailID, EID)
            If oUser.UID > 0 Then
                Session("UID") = oUser.UID
                Session("USERNAME") = oUser.UserName
                Session("USERROLE") = oUser.UserRole
                Session("Roles") = oUser.UserRole
                Session("USERIMAGE") = oUser.UserImage
                Session("CLOGO") = oUser.clogo
                Session("EID") = oUser.EID
                Session("IPADDRESS") = oUser.ipAddress
                Session("MACADDRESS") = oUser.macAddress
                Session("RETURL") = retURl
                Session("ISLOCAL") = "TRUE"
                Session("CTHEME") = oUser.CTHEME
                Session("INTIME") = DateTime.Now
                Session("TicketSSO") = 1
                'If oUser.islocal = 0 Then
                '    Session("ISLOCAL") = "TRUE"
                'Else
                '    Session("ISLOCAL") = "TRUE"
                'End If
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim str As String = ""
                str = "UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where UID=" & Session("UID") & " and eid=" & HttpContext.Current.Session("EID") & ""
                Using con = New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(str, con)
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                        con.Dispose()
                    End Using
                End Using
                Response.Redirect("~/THome.aspx")
            Else
                Dim objScaller As DataTable = objDataClass.ExecuteQryDT("select isallowcreateuser  from mmm_hdmail_schdule where isactive=1 and mdmailid is not null  and mdpwd is not null and mdport is not null and mdisssl is not null and hostname is not null and eid=" & EID & "")
                If objScaller.Rows(0)(0) = True Then
                    Dim dt As New DataTable
                    dt = objDataClass.ExecuteQryDT("insert into mmm_mst_user (username,userid,emailid,userrole,isauth,eid,passtry,modifydate,locationid) values ('" & emailAPI(2) & "','" & emailAPI(1) & "','" & emailAPI(1) & "','END_USER',100," & EID & ",0,getdate(),2072);select scope_identity()")
                    oUser.SSOValidateUser(emailID, EID)
                    Dim obDMS As New DMSUtil()
                    obDMS.notificationMail(oUser.UID, oUser.EID, "USER", "USER CREATED")
                    If oUser.UID > 0 Then
                        Session("UID") = oUser.UID
                        Session("USERNAME") = oUser.UserName
                        Session("USERROLE") = oUser.UserRole
                        Session("Roles") = oUser.UserRole
                        Session("USERIMAGE") = oUser.UserImage
                        Session("CLOGO") = oUser.clogo
                        Session("EID") = oUser.EID
                        Session("IPADDRESS") = oUser.ipAddress
                        Session("MACADDRESS") = oUser.macAddress
                        Session("RETURL") = retURl
                        Session("ISLOCAL") = "TRUE"
                        Session("CTHEME") = oUser.CTHEME
                        Session("INTIME") = DateTime.Now
                        Session("TicketSSO") = 1
                        'If oUser.islocal = 0 Then
                        '    Session("ISLOCAL") = "TRUE"
                        'Else
                        '    Session("ISLOCAL") = "TRUE"
                        'End If
                        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                        Dim str As String = ""
                        str = "UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where UID=" & Session("UID") & " and eid=" & HttpContext.Current.Session("EID") & ""
                        Using con = New SqlConnection(conStr)
                            Using da As New SqlDataAdapter(str, con)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                da.SelectCommand.ExecuteNonQuery()
                                con.Dispose()
                            End Using
                        End Using
                        Response.Redirect("~/THome.aspx")
                    End If
                Else
                    Response.Redirect("~/Default.aspx")
                End If
            End If
        End If

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
End Class
