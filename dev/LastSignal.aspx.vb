Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Web.Services
Imports Ionic.Zip
Imports System.Web.Hosting
Partial Class LastSignal
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Public Sub BindGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            Dim STR As String = txtImei.Text.ToString
            Dim str1 As String() = STR.Split(",")
            STR = ""
            For i As Integer = 0 To str1.Length - 1
                STR = STR & "'" & str1(i) & "',"
            Next
            STR = STR.TrimEnd(",")
            oda.SelectCommand.CommandText = "select  imieno[IMEI No.],CONVERT(VARCHAR(20),ctime, 113)[Last C-Time],CONVERT(VARCHAR(20),recordtime, 113)[Last R-Time] from mmm_mst_gpsdata g with(nolock) where imieno in (" & Trim(STR) & ")  group by IMIENO,recordtime,ctime having recordtime=(select max(recordtime) from mmm_mst_gpsdata with(nolock) where imieno=g.imieno)"
            oda.SelectCommand.CommandTimeout = 600
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                GVReport.DataSource = ds
                GVReport.DataBind()
            Else
                GVReport.DataBind()
            End If

        Catch ex As Exception
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
   

    Protected Sub btnShow_Click(sender As Object, e As System.EventArgs) Handles btnShow.Click

        BindGrid()

    End Sub
End Class
