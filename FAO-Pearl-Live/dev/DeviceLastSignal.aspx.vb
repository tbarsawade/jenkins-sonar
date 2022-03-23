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
Partial Class DeviceLastSignal
    Inherits System.Web.UI.Page
 
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
      
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
    Public Sub BindGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            oda.SelectCommand.CommandText = "select qryfield from mmm_mst_report where reportid=342 and eid=54"
            oda.Fill(ds, "data")
            Dim qry As String = ds.Tables("data").Rows(0).Item("qryfield").ToString
            oda.SelectCommand.CommandText = qry
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataSet
            oda.SelectCommand.CommandTimeout = 600
            oda.Fill(dt)
            ViewState("grd") = dt
        Catch ex As Exception
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    'Export to Excel Report

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
    End Sub

    Protected Sub btnViewInExcel_Click(sender As Object, e As System.EventArgs) Handles btnViewInExcel.Click
        Response.ClearContent()
        BindGrid()
        Dim gridview1 As New GridView
        gridview1.DataSource = ViewState("grd")
        gridview1.DataBind()
        Response.ContentType = "application/vnd.ms-excel"
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Last Signal Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "Last Signal Report" & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i = 0 To gridview1.Rows.Count - 1
            gridview1.Rows(i).Attributes.Add("class", "textmode")
        Next
        gridview1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
End Class
