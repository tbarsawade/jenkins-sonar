
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing

Partial Class GPSDATA
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As New SqlDataAdapter("select distinct IMIENO from MMM_MST_GPSDATA where devtype='PHONE'", con)
            Dim ds As New DataSet()
            oda.Fill(ds)
            ddlImieno.DataSource = ds
            ddlImieno.DataTextField = "IMIENO"
            ddlImieno.DataBind()
            con.Close()
            ds.Dispose()
            oda.Dispose()
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
    Protected Sub ddlImieno_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlImieno.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim oda As New SqlDataAdapter("select top " & txtCount.Text & " * from MMM_MST_GPSDATA where imieno='" & ddlImieno.SelectedItem.Text & "' and devtype='PHONE' order by recordtime desc", con)
        Dim ds As New DataSet()
        oda.Fill(ds)
        gvData.DataSource = ds
        gvData.DataBind()
        con.Close()
        ds.Dispose()
        oda.Dispose()

    End Sub
End Class
