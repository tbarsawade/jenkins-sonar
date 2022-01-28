Imports System.Data.SqlClient

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT  * from [user]", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        con.Dispose()
        oda.Dispose()



    End Sub
End Class