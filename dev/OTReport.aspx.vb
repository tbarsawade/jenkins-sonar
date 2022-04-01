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
Imports Ionic.Zip
Imports Microsoft.Office.Interop
Imports System.Web.Hosting

Partial Class OTReport
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
        End If

    End Sub
    <WebMethod()> _
    Public Shared Function GetData(sdate As String, edate As String, Bunit As String) As DGrid
        Dim grid As New DGrid()
        If Bunit = "null" Then
            grid.Message = "Please select business unit...!"
            Return grid
            grid.Success = False
        End If
        Dim str() As String = Bunit.Split(",")
        Dim s As String = ""
        For i As Integer = 0 To str.Length - 1
            s &= "'" & str(i) & "',"
        Next
        Bunit = s.Trim().Remove(s.Length - 1)
        Dim jsonData As String = ""

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim qry As String = ""
            If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=960"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=960"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            'qry &= " and convert(date,d.attdate)>='" & sdate & "' and convert(date,d.attdate)<='" & edate & "'"
            qry = Replace(qry, "@Frdate", sdate)
            qry = Replace(qry, "@Todate", edate)
            qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@BI", Bunit)
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
        Return grid

    End Function
    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetExport(sdate As String, edate As String, Bunit As String) As String
        Dim str() As String = Bunit.Split(",")
        Dim s As String = ""
        For i As Integer = 0 To str.Length - 1
            s &= "'" & str(i) & "',"
        Next
        Bunit = s.Trim().Remove(s.Length - 1)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim qry As String
        Try
            If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=960"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=960"
            End If
            da.SelectCommand.CommandTimeout = 300
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@Frdate", sdate)
            qry = Replace(qry, "@Todate", edate)
            qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@BI", Bunit)
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandTimeout = 900
            da.Fill(dt)
            Dim fname As String = "OTReport.CSV"
            Dim FPath As String = HostingEnvironment.MapPath("~\Mailattach\")
            FPath = FPath & fname
            ' Dim FPath As String = "D:\Websites\APP1\Myndsaas\Mailattach\"
            Dim sw As StreamWriter = New StreamWriter(FPath, False)
            sw.Flush()
            'First we will write the headers.
            Dim iColCount As Integer = dt.Columns.Count
            For i As Integer = 0 To iColCount - 1
                sw.Write(dt.Columns(i))
                If (i < iColCount - 1) Then
                    sw.Write(",")
                End If
            Next
            sw.Write(sw.NewLine)
            ' Now write all the rows.
            Dim dr As DataRow
            For Each dr In dt.Rows
                For i As Integer = 0 To iColCount - 1
                    If Not Convert.IsDBNull(dr(i)) Then
                        sw.Write(dr(i).ToString)
                    End If
                    If (i < iColCount - 1) Then
                        sw.Write(",")
                    End If
                Next
                sw.Write(sw.NewLine)
            Next
            sw.Close()
            Return "/Mailattach/" & fname
        Catch ex As Exception
            da.SelectCommand.CommandText = "INSERT_ERRORLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & " Error column number:")
            da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "DCMREPORT")
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

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetBusinessUnit() As String
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "'"
        If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
            Qry = "select tid[TID],fld3[BU] from  mmm_mst_master with(nolock) where tid in (select * from  InputString((select fld1 from mmm_ref_role_user with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and UID='" & HttpContext.Current.Session("UID") & "'))) and EID=" & HttpContext.Current.Session("EID") & " and DocumentType='HRSPOC Master'"
        Else
            Qry = "select tid[TID],fld3[BU] from  mmm_mst_master with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='HRSPOC Master'"
        End If

        Dim ds As New DataSet()
        Using con As New SqlConnection(conStr)
            Using da As New SqlDataAdapter(Qry, con)
                da.Fill(ds)
            End Using
        End Using
        Dim serializerSettings As New JsonSerializerSettings()
        Dim json_serializer As New JavaScriptSerializer()
        serializerSettings.Converters.Add(New DataTableConverter())
        ret = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
        Return ret
    End Function

End Class
