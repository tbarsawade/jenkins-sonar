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
Partial Class IHCLTATADB
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

    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetReportName(documentType As String) As kGridIHCLRpt
        Dim jsonData As String = ""
        Dim ret As New kGridIHCLRpt()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim dt As New DataTable
            strQuery1 = "Select distinct Tid,fld1 as 'ClusterName' from MMM_MST_MASTER where eid=" & HttpContext.Current.Session("EID") & " and DocumentType='Cluster' and IsAuth=1 ORDER BY fld1"
            dt = DataLib.ExecuteTable(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(dt)
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    <WebMethod()>
    Public Shared Function GetData(cluster As String, valueof As String, sdate As String, edate As String, paymenttype As String) As DGrid
        Dim grid As New DGrid()
        If sdate = "null" Or edate = "null" Then
            grid.Message = "Please select date first..!"
            Return grid
            grid.Success = False
        End If
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ds As New DataSet()
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim qry As String = ""
            Dim cmd As New SqlCommand()
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "IHCLTATDASHBOARDREPORT"
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@cluster", cluster)
            da.SelectCommand.Parameters.AddWithValue("@PaymentType", paymenttype)
            da.SelectCommand.Parameters.AddWithValue("@Fromdate", sdate)
            da.SelectCommand.Parameters.AddWithValue("@Todate", edate)
            da.SelectCommand.Parameters.AddWithValue("@value", valueof)
            da.SelectCommand.Parameters.AddWithValue("@Eid", HttpContext.Current.Session("EID"))
            ds.Tables.Clear()
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
Public Class kGridIHCLRpt
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnIHCLRpt)
End Class
Public Class kColumnIHCLRpt
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
