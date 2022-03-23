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

Partial Class UserDetailRpt
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim grid As New DGrid()
            Dim sdate As String = ""
            'GetData(sdate)
        End If
    End Sub
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetData(sdate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim eid As Integer = HttpContext.Current.Session("eid")
        Try
            'da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & eid & "  and Reportid=1688 "
            Dim str As String = "select UserName[User Name],EmailID[Email ID],UserID,UserRole[User Role],substring((select ',' + userrole FROM (select  userrole from mmm_mst_user u where eid=" & HttpContext.Current.Session("EID") & " AND UID=M.UID union select rolename from MMM_Ref_Role_User ru where eid=" & HttpContext.Current.Session("EID") & " AND UID=M.UID union select   rolename from MMM_ref_PreRole_user pu where eid=" & HttpContext.Current.Session("EID") & " AND UID=M.UID) AR FOR XML PATH('')),2,200000) as [Additional Role],case when isauth=1 then 'Active' when isauth=100 then 'Inactive' else 'Other/Locked' end [Status] from mmm_mst_user M where eid=" & HttpContext.Current.Session("EID") & " and userrole<>'SU'"
            Dim dt As New DataTable
            da.SelectCommand.CommandText = str
            da.SelectCommand.CommandTimeout = 300
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            ''da.SelectCommand.ExecuteNonQuery()
            da.Fill(dt)
            Dim strError = ""
            grid = DynamicGrid.GridData(dt, strError)
            If dt.Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0

        Finally
            con.Close()
            da.Dispose()
            con.Dispose()
        End Try
        Return grid
    End Function
End Class
