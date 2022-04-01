Imports System.Data
Imports System.Data.SqlClient
Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim UserID As String = ""
        Dim Password As String = ""
        Dim ECode As String = ""
        Try
            If Not IsPostBack Then

                If Request.QueryString.HasKeys() Then
                    If Request.QueryString("uid") <> Nothing Then
                        UserID = Request.QueryString("uid")
                    End If
                    If Request.QueryString("pwd") <> Nothing Then
                        Password = Request.QueryString("pwd")
                    End If
                    If Request.QueryString("eid") <> Nothing Then
                        ECode = Request.QueryString("eid")
                    End If

                    If UserID <> "" And Password <> "" And ECode <> "" Then
                        validateUser(UserID, Password, ECode)
                    End If
                End If
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As System.EventArgs) Handles btnLogin.Click
        Dim UserID As String
        Dim Password As String
        Dim ECode As String
        Dim strMsg As String
        If Page.IsValid Then
            Try
                UserID = txtName.Text.Trim()
                Password = txtPwd.Text.Trim()
                ECode = txtEntity.Text.Trim()
                UserID = UserID.Replace("'", "")
                UserID = UserID.Replace("-", "")
                Password = Password.Replace("'", "")
                Password = Password.Replace("-", "")
                ECode = ECode.Replace("'", "")
                ECode = ECode.Replace("-", "")
                validateUser(UserID, Password, ECode)
            Catch ex As Exception
                strMsg = "Sorry! Your request can not be submitted at the moment.Error occured at server."
                lblMessage.Text = "<b>" & strMsg & "</b>"
            End Try
            
        End If
    End Sub

    Protected Sub validateUser(strLoginID As String, strPWD As String, strEntity As String)
        Dim ret As Integer = 0
        Dim strMsg As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim od As SqlDataAdapter = Nothing
        Try
            od = New SqlDataAdapter("Select EID,Code from MMM_MST_ENTITY where Code='" & strEntity & "' ", con)
            con = New SqlConnection(conStr)
            od.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            Dim oUser As New User
            oUser.validateUser(strLoginID, strPWD, strEntity)
            Select Case oUser.validateUser(strLoginID, strPWD, strEntity)
                Case 100
                    'user doesn't exist
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Sorry, your User ID is not configured in the system."

                Case 0
                    'user doesn't exist
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Sorry, you have entered an invalid User ID or Password. Please try again."


                Case 7
                    'user Blocked by Super user exist
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Sorry, your User ID has been blocked."

                Case 3
                    'wrong password but valid userID
                    Dim sqlq As String
                    sqlq = "UPDATE MMM_MST_USER SET passtry=isnull(passtry,0) +1 where emailid='" & strLoginID & "'"
                    Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                    'Dim ds As New DataSet
                    oda.Fill(ds, "user")
                    con.Dispose()
                    oda.Dispose()

                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Sorry, you have entered an invalid User ID or Password. Please try again."

                Case 4
                    'Password retry reached lock him now

                    Dim sqlq As String
                    sqlq = "UPDATE MMM_MST_USER SET passtry=0,Modifydate=getdate(),isauth=3 where emailid='" & strLoginID & "'"
                    Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                    'Dim ds As New DataSet
                    oda.Fill(ds, "user")
                    con.Dispose()
                    oda.Dispose()
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "You have exceeded allowed maximum login attempts and your account is blocked temporarily."
                Case 10
                    'Password retry reached lock him now
                    'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                    Dim sqlq As String
                    sqlq = "UPDATE MMM_MST_USER SET passtry=0,Modifydate=getdate(),isauth=2 where emailid='" & strLoginID & "'"
                    Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                    'Dim ds As New DataSet
                    oda.Fill(ds, "user")
                    con.Dispose()
                    oda.Dispose()
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Your password has expired. Please regenerate it"

                Case 5
                    'Password is Locked
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "You have exceeded allowed maximum login attempts and your account is blocked temporarily."

                Case 6
                    'Password is Locked
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Your password has expired. Please regenerate it"

                Case 1
                    Session("UID") = oUser.UID
                    Session("USERNAME") = oUser.UserName
                    Session("USERROLE") = oUser.UserRole
                    Session("EMAIL") = oUser.emailID
                    Session("USERIMAGE") = oUser.UserImage
                    Session("CLOGO") = oUser.clogo
                    Session("EID") = oUser.EID
                    Session("IPADDRESS") = oUser.ipAddress
                    Session("MACADDRESS") = oUser.macAddress
                    Session("HEADERSTRIP") = oUser.HeaderStrip
                    If oUser.islocal = 0 Then
                        Session("ISLOCAL") = "TRUE"
                    Else
                        Session("ISLOCAL") = "TRUE"
                    End If
                    Session("INTIME") = Now
                    Session("LID") = oUser.locationID
                    Session("OFFSET") = oUser.offSet
                    Session("CODE") = oUser.strCode
                    Session("Roles") = oUser.roles
                    'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                    Dim sqlq As String
                    sqlq = "UPDATE MMM_MST_USER SET passtry=0,isauth=1 where emailid='" & strLoginID & "'"
                    Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                    'Dim ds As New DataSet
                    oda.Fill(ds, "user")
                    con.Dispose()
                    oda.Dispose()
                    Response.RedirectPermanent("~/mobile/MainHome.aspx")
                Case 2
                    txtName.Text = ""
                    txtPwd.Text = ""
                    strMsg = "Your password has expired. Please regenerate it."
            End Select
            lblMessage.Text = "<b>" & strMsg & "</b>"
        Catch ex As Exception
            strMsg = "Sorry! Your request can not be submitted at the moment.Error occured at server."
        Finally
            con.Close()
            con.Dispose()
            od.Dispose()
        End Try


    End Sub
End Class
