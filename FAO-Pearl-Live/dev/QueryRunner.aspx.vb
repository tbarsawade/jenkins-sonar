Imports System.Data
Imports System.Data.SqlClient

Partial Class QueryRunner
    Inherits System.Web.UI.Page

    Protected Sub btnQuery_Click(sender As Object, e As System.EventArgs) Handles btnQuery.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
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
