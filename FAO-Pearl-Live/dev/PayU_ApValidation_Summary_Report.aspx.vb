
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Newtonsoft.Json

Partial Class PayU_ApValidation_Summary_Report
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

    <System.Web.Services.WebMethod()>
    Public Shared Function BindCompanyCode(documentType As String) As kGridAmbitRptPayu
        Dim jsonData As String = ""
        Dim ret As New kGridAmbitRptPayu()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim dt As New DataTable
            strQuery1 = "SELECT distinct fld98 CompanyCode from MMM_MST_DOC WITH(NOLOCK) where eid =137 and documentTYpe ='Invoice PO' and isnull(fld98,'') <> ''"
            dt = DataLib.ExecuteTable(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(dt)
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    <WebMethod()>
    Public Shared Function GetData(sdate As String, edate As String, CompCode As String, Status As String) As DGrid
        Dim grid As New DGrid()
        If sdate = "null" Or edate = "null" Then
            grid.Message = "Please select date first..!"
            Return grid
            grid.Success = False
        End If
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.CommandText = "GetPayUApValidationSummaryReport"
            oda.SelectCommand.Parameters.AddWithValue("@CompCode", CompCode)
            oda.SelectCommand.Parameters.AddWithValue("@Status", Status)
            oda.SelectCommand.Parameters.AddWithValue("@From", sdate)
            oda.SelectCommand.Parameters.AddWithValue("@TO", edate)

            Dim ds As New DataSet()
            oda.SelectCommand.CommandTimeout = 900
            oda.Fill(ds, "data")
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
Public Class kGridAmbitRptPayu
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnAmbitRptPayu)
End Class
Public Class kColumnAmbitRptPayu
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