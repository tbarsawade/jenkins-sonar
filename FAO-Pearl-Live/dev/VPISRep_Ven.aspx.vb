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

Partial Class VPISRep_Ven
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
        If Not Page.IsPostBack Then
            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd2.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub
    <WebMethod()> _
      <Script.Services.ScriptMethod()> _
    Public Shared Function GetPGtype() As String

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "select 'All' [tid] ,'All' [fld1] union all    select 'In-Process' [tid] ,'In-Process' [fld1] union all  select 'Processed' [tid] ,'Processed' [fld1] union all select 'Rejected' [tid] ,'Rejected' [fld1]  "

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
    <WebMethod()> _
     <Script.Services.ScriptMethod()> _
    Public Shared Function GetData(sdate As String, tdate As String, PoNum As String, Vname As String, PrGrp As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim qry As String = ""
        Dim UID As Integer = 0
        UID = Convert.ToInt32(HttpContext.Current.Session("UID").ToString())
        da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=967"
        da.Fill(ds, "qry")
        qry = ds.Tables("qry").Rows(0).Item(0).ToString()
        qry = qry.Replace("@Frdate", sdate)
        qry = qry.Replace("@Todate", tdate)
        qry = qry.Replace("@UID", UID)
        qry = qry.Replace("@PoNum", PoNum)
        qry = qry.Replace("@Vname", Vname)

        If PrGrp = "null" Or PrGrp = "All" Then
            qry = qry.Replace("and curstatus in (@CurStatus)", "")
        End If

        If PrGrp = "Processed" Then
            qry = qry.Replace("@CurStatus", "Archive")
        End If

        If PrGrp = "In-Process" Then
            qry = qry.Replace("@CurStatus", "'UPLOADED','DISPATCH','Physical','Approver 1','Approver 2','Approver 3','Approver 4','Approver 5','GR Service Entry','SAP ID Update','SAP ID Posted','QC'")
        End If

        If PrGrp = "Rejected" Then
            qry = qry.Replace("@CurStatus", "'REJECTED'")
        End If


        da.SelectCommand.CommandText = qry
        da.SelectCommand.CommandTimeout = 600
        da.Fill(ds, "data")

        Dim strError = ""
        grid = DynamicGrid.GridData(ds.Tables("data"), strError)
        Try
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
End Class
    

