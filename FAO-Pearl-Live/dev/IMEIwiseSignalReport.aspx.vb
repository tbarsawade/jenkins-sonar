Imports System.Data
Imports System.Data.SqlClient
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Partial Class TripReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub
    Protected Sub btsSearch_Click(sender As Object, e As System.EventArgs) Handles btsSearch.Click
        show()
    End Sub
    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1
    Public Sub show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            If txtsdate.Text = "" Then
                lblmsg.Text = "Please select Date."
                Exit Sub
            End If
            da.SelectCommand.CommandText = "select distinct imieno[IMEI] from mmm_mst_gpsdata g inner join mmm_mst_master m on m.fld13=g.imieno where convert(date,ctime)='" & txtsdate.Text & "' and m.documenttype='GPS'"
            da.SelectCommand.CommandTimeout = 180
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 1 Then
                GPSReport.Controls.Clear()
                GPSReport.DataSource = ds.Tables("data")
                GPSReport.DataBind()
                lblmsg.Text = ds.Tables("data").Rows.Count & " Records Found..."
            End If
            ViewState("xlexport") = ds.Tables("data")
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
    Protected Sub GPSReport_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GPSReport.PageIndexChanged
        show()
    End Sub
    Protected Sub GPSReport_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GPSReport.PageIndexChanging
        GPSReport.PageIndex = e.NewPageIndex
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click
        GPSReport.AllowPaging = False
        GPSReport.DataSource = ViewState("xlexport")
        GPSReport.DataBind()
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>GPS Half Hourly Report </h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=GPS Half Hourly Report.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        'For i As Integer = 0 To grdTripData.Rows.Count - 1
        '    'Apply text style to each Row 

        '    grdTripData.Rows(i).Attributes.Add("class", "textmode")
        'Next
        GPSReport.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
End Class

