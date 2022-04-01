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
            da.SelectCommand.CommandText = "select '23:30 to 00:00'[Time],count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & "  23:30' and recordtime <'" & txtsdate.Text & "  23:59'union select '23:00 to 23:30'[Time],count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 23:00' and recordtime <'" & txtsdate.Text & " 23:30' union select '22:30 to 23:00'[Time],count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 22:30' and recordtime <'" & txtsdate.Text & " 23:00' union select '22:00 to 22:30'[Time],count(*) as[Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 22:00' and recordtime <'" & txtsdate.Text & " 22:30' union select '21:30 to 22:00'[Time],count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 21:30' and recordtime <'" & txtsdate.Text & " 22:00' union select '21:00 to 21:30'[Time],count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 21:00' and recordtime <'" & txtsdate.Text & " 21:30' union select '20:30 to 21:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 20:30' and recordtime <'" & txtsdate.Text & " 21:00' union select '20:00 to 20:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 20:00' and recordtime <'" & txtsdate.Text & " 20:30' union select '19:30 to 20:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 19:30' and recordtime <'" & txtsdate.Text & " 20:00' union select '19:00 to 19:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 19:00' and recordtime <'" & txtsdate.Text & " 19:30' union select '18:30 to 19:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 18:30' and recordtime <'" & txtsdate.Text & " 19:00' union select '18:00 to 18:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 18:00' and recordtime <'" & txtsdate.Text & " 18:30' union select '17:30 to 18:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 17:30' and recordtime <'" & txtsdate.Text & " 18:00' union select '17:00 to 17:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 17:00' and recordtime <'" & txtsdate.Text & " 17:30' union select '16:30 to 17:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 16:30' and recordtime <'" & txtsdate.Text & " 17:00' union select '16:00 to 16:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 16:00' and recordtime <'" & txtsdate.Text & " 16:30' union select '15:30 to 16:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 15:30' and recordtime <'" & txtsdate.Text & " 16:00' union select '15:00 to 15:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 15:00' and recordtime <'" & txtsdate.Text & " 15:30' union select '14:30 to 15:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 14:30' and recordtime <'" & txtsdate.Text & " 15:00' union select '14:00 to 14:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 14:00' and recordtime <'" & txtsdate.Text & " 14:30' union select '13:30 to 14:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 13:30' and recordtime <'" & txtsdate.Text & " 14:00' union select '13:00 to 13:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 13:00' and recordtime <'" & txtsdate.Text & " 13:30' union select '12:30 to 13:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 12:30' and recordtime <'" & txtsdate.Text & " 13:00' union select '12:00 to 12:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 12:00' and recordtime <'" & txtsdate.Text & " 12:30' union select '11:30 to 12:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 11:30' and recordtime <'" & txtsdate.Text & " 12:00' union select '11:00 to 11:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 11:00' and recordtime <'" & txtsdate.Text & " 11:30' union select '10:30 to 11:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 10:30' and recordtime <'" & txtsdate.Text & " 11:00' union select '10:00 to 10:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 10:00' and recordtime <'" & txtsdate.Text & " 10:30' union select '09:30 to 10:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 09:30' and recordtime <'" & txtsdate.Text & " 10:00' union select '09:00 to 09:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 09:00' and recordtime <'" & txtsdate.Text & " 09:30' union select '08:30 to 09:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 08:30' and recordtime <'" & txtsdate.Text & " 09:00' union select '08:00 to 08:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 08:00' and recordtime <'" & txtsdate.Text & " 08:30' union select '07:30 to 08:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 07:30' and recordtime <'" & txtsdate.Text & " 08:00' union select '07:00 to 07:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 07:00' and recordtime <'" & txtsdate.Text & " 07:30' union select '06:30 to 07:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 06:30' and recordtime <'" & txtsdate.Text & " 07:00' union select '06:00 to 06:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 06:00' and recordtime <'" & txtsdate.Text & " 06:30' union select '05:30 to 06:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 05:30' and recordtime <'" & txtsdate.Text & " 06:00' union select '05:00 to 05:30',count(*)  as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 05:00' and recordtime <'" & txtsdate.Text & " 05:30' union select '04:30 to 05:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 04:30' and recordtime <'" & txtsdate.Text & " 05:00' union select '04:00 to 04:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 04:00' and recordtime <'" & txtsdate.Text & " 04:30' union select '03:30 to 04:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 03:30' and recordtime <'" & txtsdate.Text & " 04:00' union select '03:00 to 03:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 03:00' and recordtime <'" & txtsdate.Text & " 03:30' union select '02:30 to 03:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 02:30' and recordtime <'" & txtsdate.Text & " 03:00' union select '02:00 to 02:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 02:00' and recordtime <'" & txtsdate.Text & " 02:30' union select '01:30 to 02:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 01:30' and recordtime <'" & txtsdate.Text & " 02:00' union select '01:00 to 01:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 01:00' and recordtime <'" & txtsdate.Text & " 01:30' union select '00:30 to 01:00',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI] from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 01:00' and recordtime <'" & txtsdate.Text & " 01:30' union select '00:00 to 00:30',count(*) as [Data Count],count(distinct IMIENO)[Total IMEI]  from MMM_MST_GPSDATA where recordtime >'" & txtsdate.Text & " 00:01' and recordtime <'" & txtsdate.Text & " 00:30' "
            da.SelectCommand.CommandTimeout = 2000
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 1 Then
                GPSReport.Controls.Clear()
                GPSReport.DataSource = ds.Tables("data")
                GPSReport.DataBind()
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

