Imports System.Data
Imports System.Data.SqlClient

Partial Class Events
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Request.QueryString("SC") Is Nothing Then
            Else
                Dim scrname As String = Request.QueryString("SC").ToString()
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As SqlConnection = New SqlConnection(conStr)
                Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT * FROM MMM_MST_FORMS where formname='" & scrname & "' and EID=" & Session("EID").ToString(), con)
                Dim ds As New DataSet()
                oda.Fill(ds, "data")
                Page.Title = ds.Tables("data").Rows(0).Item("FormDesc").ToString()
                lblCaption.Text = ds.Tables("data").Rows(0).Item("Formcaption").ToString()
                con.Close()
                oda.Dispose()
                con.Dispose()
            End If
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
End Class
