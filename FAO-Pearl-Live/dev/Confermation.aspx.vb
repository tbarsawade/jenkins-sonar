Imports System.Data.SqlClient
Imports System.Data

Partial Class confermation
    Inherits System.Web.UI.Page
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
    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim confirmation As String = Request.QueryString("saved").ToString()
            If confirmation = "1" Then
                lblMess.Text = "You have Already Submitted the required data ! "
            End If
            If confirmation = "2" Then
                lblMess.Text = "The link is expired  ! "
            End If
        End If
        'Dim sURL As String = Request.Url.GetLeftPart(UriPartial.Authority).ToUpper()
        ''Dim sURL As String = "www.myndsaas.com".ToUpper()
        ''Dim sURL As String = "https://hfcl.myndsaas.com".ToUpper()
        'Dim resend As Integer = 0
        'If Left(sURL, 5).ToUpper() <> "HTTPS" Then
        '    resend = 1
        'End If
        'sURL = Replace(sURL, "HTTP://", "")
        'sURL = Replace(sURL, "HTTPS://", "")
        'sURL = Replace(sURL, "WWW.", "")
        'sURL = Replace(sURL, ".MYNDSAAS.COM", "")
        'sURL = Replace(sURL, "MYNDSAAS.COM", "")
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim od As SqlDataAdapter = New SqlDataAdapter("Select EID,Code,name,logo from MMM_MST_ENTITY where Code='" & sURL & "' ", con)
        'od.SelectCommand.CommandType = CommandType.Text
        'Dim ds As New DataSet()
        'od.Fill(ds, "code")
        'If ds.Tables("code").Rows.Count = 1 Then
        '    lblMsg.Text = "Your session has expired and you have been logged off from your account. Thanks for using " & ds.Tables("code").Rows(0).Item("Name").ToString() & " System."
        '    lblLogo.Text = "<img src=""logo/" & ds.Tables("code").Rows(0).Item("logo").ToString() & """ alt=""" & ds.Tables("code").Rows(0).Item("Name").ToString() & """  />"
        '    'lbllogin.Text = ds.Tables("code").Rows(0).Item("Code").ToString()
        '    ViewState("company") = ds.Tables("code").Rows(0).Item("Code").ToString()
        'End If
        'con.Close()
        'ds.Dispose()
        'od.Dispose()
    End Sub


    'Protected Sub btndefaultpage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btndefaultpage.Click
    '    'Dim page As String = "HTTPS://" & ViewState("company") & ".myndsaas.com"
    '    'Response.Redirect(page)
    'End Sub
End Class
