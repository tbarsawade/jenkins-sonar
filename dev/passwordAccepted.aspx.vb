Imports System.Data.SqlClient
Imports System.Data

Partial Class passwordAccepted
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Dim strcode As String = Request.QueryString("urlcode").ToString()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim obj As New User
            strcode = strcode.Replace(" ", "+")
            Dim DecEID As String = obj.DecryptTripleDES(strcode, "12345")

            Dim od As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_MST_ENTITY where EID='" & DecEID & "' ", con)
            od.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            od.Fill(ds, "code")
            If ds.Tables("code").Rows.Count = 1 Then
                lblMsg.Text = "Your change password request for log on to your account with " & ds.Tables("code").Rows(0).Item("Name").ToString() & " System has been successfully accepted. Please click on link hereunder to log into the system"
                lblLogo.Text = "<img src=""logo/" & ds.Tables("code").Rows(0).Item("logo").ToString() & """ alt=""" & ds.Tables("code").Rows(0).Item("Name").ToString() & """  />"
                ViewState("company") = ds.Tables("code").Rows(0).Item("Code").ToString()

            End If

            con.Close()
            ds.Dispose()
            od.Dispose()
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
    Protected Sub btndefaultpage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btndefaultpage.Click
        Dim page As String = "HTTPS://" & ViewState("company") & ".myndsaas.com"
        Response.Redirect(page)
    End Sub
End Class
