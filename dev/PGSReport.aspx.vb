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
Partial Class PGSReport
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
            txtInvoiceFrom.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtInvoiceTo.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetReportName(documentType As String) As kGridPGSRpt
        Dim jsonData As String = ""
        Dim ret As New kGridPGSRpt()
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

    <WebMethod()>
    Public Shared Function GetData(sdate As String, edate As String, InvFrom As String, InvTo As String, Reportid As Integer) As DGrid
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
            da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid='" + Reportid.ToString() + "'"
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()

            Dim PaymentDateqry As String = ""
            Dim InvoiceDateqry As String = ""
            If Reportid = 2136 Then
                If sdate <> "" And edate <> "" Then
                    PaymentDateqry = " and (cast(CONVERT(CHAR(19), CONVERT(DATETIME, d.fld61, 3), 120) as date) >= '" & sdate & "' and cast(CONVERT(CHAR(19), CONVERT(DATETIME, d.fld61, 3), 120) as date) <='" & edate & "') "
                End If
                If InvFrom <> "" And InvTo <> "" Then
                    InvoiceDateqry += " and (cast(CONVERT(CHAR(19), CONVERT(DATETIME, d.fld20, 3), 120) as date) >= '" & InvFrom & "' and cast(CONVERT(CHAR(19), CONVERT(DATETIME, d.fld20, 3), 120) as date) <='" & InvTo & "') "
                End If
                qry = Replace(qry, "@PaymentDate", PaymentDateqry)
                qry = Replace(qry, "@InvoiceDate", InvoiceDateqry)
            Else
                qry = Replace(qry, "@Frdate", sdate)
                qry = Replace(qry, "@Todate", edate)
            End If

            'qry = Replace(qry, "@InvFromdate", InvFrom)
            'qry = Replace(qry, "@InvTodate", InvTo)

            'qry = Replace(qry, "@Vendor", VendorCode)
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
End Class


Public Class kGridPGSRpt
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnPGSRpt)
End Class
Public Class kColumnPGSRpt
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
