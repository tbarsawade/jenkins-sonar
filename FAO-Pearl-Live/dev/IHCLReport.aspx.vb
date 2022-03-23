Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports iTextSharp.text.pdf
Imports System.Web.Hosting
Partial Class IHCLReport
    Inherits System.Web.UI.Page
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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd2.Text = Now.ToString("yyyy-MM-dd")
            BindReportDdl()
            BindOperatingUnit()
        End If
    End Sub
    Sub BindReportDdl()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim strQuery1 As String = ""
        Try
            Dim dt As New DataTable
            strQuery1 = "Select distinct Reportid,ReportName from MMM_MST_REPORT where eid=" & HttpContext.Current.Session("EID") & " and IsActive=1 ORDER BY ReportName"
            dt = DataLib.ExecuteTable(conStr, CommandType.Text, strQuery1)
            ddlReportName.Items.Clear()
            ddlReportName.DataSource = dt
            ddlReportName.DataTextField = "ReportName"
            ddlReportName.DataValueField = "Reportid"
            ddlReportName.DataBind()
            ddlReportName.Items.Insert(0, "--Select--")
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Sub BindOperatingUnit()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim strQuery1 As String = ""
        Try
            Dim dt As New DataTable
            strQuery1 = "select tid,fld1[Category] from mmm_mst_master with(nolock) where eid=205 and documenttype='Operating Unit' order by fld1"
            dt = DataLib.ExecuteTable(conStr, CommandType.Text, strQuery1)
            ddlHotelCategory.Items.Clear()
            ddlHotelCategory.DataSource = dt
            ddlHotelCategory.DataTextField = "Category"
            ddlHotelCategory.DataValueField = "tid"
            ddlHotelCategory.DataBind()
            ddlHotelCategory.Items.Insert(0, "--Select--")
        Catch ex As Exception
            Throw
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetReportName(documentType As String) As kGridAmbitRptIHCL
        Dim jsonData As String = ""
        Dim ret As New kGridAmbitRptIHCL()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim dt As New DataTable
            strQuery1 = "Select distinct Reportid,ReportName from MMM_MST_REPORT where eid=" & HttpContext.Current.Session("EID") & " and IsActive=1 ORDER BY ReportName"
            dt = DataLib.ExecuteTable(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(dt)
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetOperatingUnit() As kGridAmbitRptIHCL
        Dim jsonData As String = ""
        Dim ret As New kGridAmbitRptIHCL()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim dt As New DataTable
            strQuery1 = "select tid,fld1[Category] from mmm_mst_master with(nolock) where eid=205 and documenttype='Operating Unit' order by fld1"
            dt = DataLib.ExecuteTable(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(dt)
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    Private Sub exportReport()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim qry As String = ""
        If (HttpContext.Current.Session("Roles").ToString.ToUpper = "SU") Then
            da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid='" + ddlReportName.SelectedItem.Value + "'"
        Else
            da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid='" + ddlReportName.SelectedItem.Value + "'"
        End If
        da.Fill(ds, "qry")
        qry = ds.Tables("qry").Rows(0).Item(0).ToString()
        qry = Replace(qry, "@Frdate", txtd1.Text)
        qry = Replace(qry, "@Todate", txtd2.Text)
        qry = Replace(qry, "@uid", HttpContext.Current.Session("UID"))
        qry = Replace(qry, "@role", HttpContext.Current.Session("USERROLE"))
        qry = Replace(qry, "@catrgory", ddlHotelCategory.SelectedItem.Value)
        qry = Replace(qry, "@vendor", txtVendorName.Text)
        da.SelectCommand.CommandText = qry
        da.SelectCommand.CommandTimeout = 900
        da.Fill(ds, "data")
        Try
            If ds.Tables("data").Rows.Count > 0 Then
                Response.ClearContent()
                Dim gvexp As New GridView
                gvexp.AllowPaging = False
                gvexp.DataSource = ds.Tables("data")
                gvexp.DataBind()
                Response.ContentType = "application/vnd.ms-excel"
                Response.AddHeader("content-disposition", "attachment;filename=Report.xls")
                Dim strwriter As New System.IO.StringWriter
                Dim HtmlTxtWriter As New HtmlTextWriter(strwriter)
                gvexp.RenderControl(HtmlTxtWriter)
                Response.Write(strwriter.ToString())
                Response.End()
            End If
        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
    End Sub

    <WebMethod()>
    Public Shared Function GetData(sdate As String, edate As String, Reportid As Integer, catrgory As String, vendorName As String) As DGrid
        Dim grid As New DGrid()
        If sdate = "null" Or edate = "null" Then
            grid.Message = "Please select date first..!"
            Return grid
            grid.Success = False
        End If
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim qry As String = ""
            If (HttpContext.Current.Session("Roles").ToString.ToUpper = "SU") Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid='" + Reportid.ToString() + "'"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid='" + Reportid.ToString() + "'"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@Frdate", sdate)
            qry = Replace(qry, "@Todate", edate)
            qry = Replace(qry, "@uid", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@role", HttpContext.Current.Session("USERROLE"))
            qry = Replace(qry, "@catrgory", catrgory)
            qry = Replace(qry, "@vendor", vendorName)
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandTimeout = 900
            da.Fill(ds, "data")
            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables("data"), strError)
            If ds.Tables("data").Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0
        End Try
        con.Close()
        con.Dispose()
        Return grid
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetExport(sdate As String, edate As String, Reportid As Integer, catrgory As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim qry As String
        Try
            If (HttpContext.Current.Session("Roles").ToString.ToUpper = "SU") Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid='" + Reportid.ToString() + "'"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid='" + Reportid.ToString() + "'"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@Frdate", sdate)
            qry = Replace(qry, "@Todate", edate)
            qry = Replace(qry, "@uid", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@role", HttpContext.Current.Session("Roles"))
            qry = Replace(qry, "@catrgory", catrgory)
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandTimeout = 900
            da.Fill(dt)

            'If dt.Rows.Count > 0 Then
            '    HttpContext.Current.Response.ClearContent()
            '    Dim gvexp As New GridView
            '    gvexp.AllowPaging = False
            '    gvexp.DataSource = dt
            '    gvexp.DataBind()
            '    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"
            '    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Report.xls")
            '    Dim strwriter As New System.IO.StringWriter
            '    Dim HtmlTxtWriter As New HtmlTextWriter(strwriter)
            '    gvexp.RenderControl(HtmlTxtWriter)
            '    HttpContext.Current.Response.Write(strwriter.ToString())
            '    HttpContext.Current.Response.End()
            'End If


            Dim fname As String = "Report_" + DateTime.Now.Ticks.ToString + ".xls"
            Dim FPath As String = HostingEnvironment.MapPath("~\Mailattach\")
            FPath = FPath & fname
            Dim gvexp As New GridView
            gvexp.AllowPaging = False
            gvexp.DataSource = dt
            gvexp.DataBind()

            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fname + "")
            ' Dim strwriter As New System.IO.StringWriter
            Dim strwriter As StreamWriter = New StreamWriter(FPath, False)
            Dim HtmlTxtWriter As New HtmlTextWriter(strwriter)
            gvexp.RenderControl(HtmlTxtWriter)
            HttpContext.Current.Response.Write(strwriter.ToString())
            '  HttpContext.Current.Response.End()

            'Dim sw As StreamWriter = New StreamWriter(FPath, False)
            'sw.Flush()
            ''First we will write the headers.
            'Dim iColCount As Integer = dt.Columns.Count
            'For i As Integer = 0 To iColCount - 1
            '    sw.Write(dt.Columns(i))
            '    If (i < iColCount - 1) Then
            '        sw.Write("|")
            '    End If
            'Next
            'sw.Write(sw.NewLine)
            '' Now write all the rows.
            'Dim dr As DataRow
            'For Each dr In dt.Rows
            '    For i As Integer = 0 To iColCount - 1
            '        If Not Convert.IsDBNull(dr(i)) Then
            '            sw.Write(dr(i).ToString)
            '        End If
            '        If (i < iColCount - 1) Then
            '            sw.Write("|")
            '        End If
            '    Next
            '    sw.Write(sw.NewLine)
            'Next
            'sw.Close()
            Return "/Mailattach/" & fname
        Catch ex As Exception
            da.SelectCommand.CommandText = "INSERT_ERRORLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & " Error column number:")
            da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "IHCL Report")
            da.SelectCommand.Parameters.AddWithValue("@EID", 128)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Function
    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        exportReport()
    End Sub
End Class


Public Class kGridAmbitRptIHCL
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnAmbitRptIHCL)
End Class
Public Class kColumnAmbitRptIHCL
    Public Sub New()

    End Sub
    Public Sub New(staticfield As [String], statictitle As [String], statictype As String, staticFormat As String)
        field = staticfield
        title = statictitle
        type = statictype
        format = staticFormat
        filterable = True
        If (statictype = "number") Then
            filterable = ""
        End If
        'width = staticwidth
    End Sub

    Public field As String = ""
    Public title As String = ""
    Public width As Integer = 200
    Public format As String = ""
    Public filterable As String = ""
    'Public locked As Boolean = True
    'Public locked As Boolean = True
    Public type As String = ""
    Public FieldID As String = ""

End Class