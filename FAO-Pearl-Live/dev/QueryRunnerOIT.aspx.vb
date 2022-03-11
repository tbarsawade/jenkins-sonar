Imports System.Data
Imports System.Data.SqlClient

Partial Class QueryRunnerOIT
    Inherits System.Web.UI.Page

    'Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
    '    Try
    '        If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
    '            Page.Theme = Convert.ToString(Session("CTheme"))
    '        Else
    '            Page.Theme = "Default"
    '        End If
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Protected Sub btnQuery_Click(sender As Object, e As System.EventArgs) Handles btnQuery.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("OITconStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter(txtQry.Text, con)
        Dim ds As New DataSet
        Try
            da.SelectCommand.CommandTimeout = 180
            da.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                da.Dispose()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
    End Sub

End Class
