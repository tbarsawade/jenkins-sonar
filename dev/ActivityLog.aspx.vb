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

Partial Class ActivityLog
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
        If HttpContext.Current.Session("EID") = "180" Then
            btnShowLastLogtime.Visible = True
        Else
            btnShowLastLogtime.Visible = False
        End If
    End Sub

    <WebMethod()>
    Public Shared Function GetLog(sdate As String, tdate As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable
            Dim qry As String = ""

            qry &= "With cte As ( "
            qry &= " Select Case When s.actiontakenscreen='User' then (select username + '-' +  convert(varchar(10), uid)  from mmm_mst_user with(nolock) where uid=s.targetuid and eid=" & HttpContext.Current.Session("EID") & ") WHEN s.actiontakenscreen='Menu Master' then 'Menu Master'WHEN s.actiontakenscreen='Role Master' then 'Role Master'WHEN s.actiontakenscreen='Additional Role Assignment' then 'Additional Role Assignment'WHEN s.actiontakenscreen='Role Assignment' then 'Role Assignment'else '' END [Action Taken On], "
            qry &= "(select username  + '-' +  convert(varchar(10), uid)  from mmm_mst_user with(nolock) where uid=s.createby and eid=" & HttpContext.Current.Session("EID") & ")[Action Taken By], "
            qry &= "convert(varchar,s.modifydate)[Modified Date],s.actiontakenscreen[Action Taken Screen], "
            qry &= "s.actionremarks[Action Remarks] from mmm_suactivity_log s with(nolock) where s.eid=" & HttpContext.Current.Session("EID") & " and  convert(date,s.modifydate)>='" & sdate & "' and convert(date,s.modifydate)<='" & tdate & "'"
            qry &= " union all "
            qry &= "Select username + '-' +  convert(varchar(10), uid)  [Action Taken On], (select username  + '-' +  convert(varchar(10), uid)  from mmm_mst_user with(nolock) where uid=s.createdby and eid=" & HttpContext.Current.Session("EID") & ")[Action Taken By],"
            qry &= "Convert(varchar, s.createdon)[Modified Date],  'USER' [Action Taken Screen], 'USER Ceation' [Action Remarks] "
            qry &= " From mmm_mst_user s with(nolock) where s.eid=" & HttpContext.Current.Session("EID") & " and s.createdon is not null and  Convert(Date, s.createdon)>='" & sdate & "' and convert(date,s.createdon)<='" & tdate & "' "
            qry &= ") select [Action Taken On],	[Action Taken By],	[Modified Date],	[Action Taken Screen],	[Action Remarks] from cte order by [Modified Date] desc"

            oda.SelectCommand.CommandText = qry
            Dim ds As New DataSet()

            Try
                Try
                    oda.SelectCommand.CommandTimeout = 300
                    oda.Fill(ds, "data")
                    If ds.Tables("data").Rows.Count = 0 Then
                        grid.Success = False
                        grid.Message = "No data found."
                    Else
                        grid = DynamicGrid.GridData(ds.Tables("data"), strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter short date range."
                End Try
            Catch ex As Exception
                grid.Success = False
                grid.Message = "No data found."
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function


    <WebMethod()>
    Public Shared Function GetLastLoginReport() As DGrid
        Dim grid As New DGrid()
        Dim strError = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable
            Dim qry As String = ""


            qry &= "select l.uid [UID],u.username [USER_NAME] ,convert(varchar,max(logintime)) [Last_Login] from MMM_MST_USERLOGINLOG L inner join mmm_mst_user u on u.uid=l.uid where l.eid=" & HttpContext.Current.Session("EID")
            qry &= "  And u.userrole Not In ('vendor','temp vendor','supplier','tempvendor') group by l.uid,u.username order by Last_login desc"

            oda.SelectCommand.CommandText = qry
            Dim ds As New DataSet()

            Try
                Try
                    oda.SelectCommand.CommandTimeout = 200
                    oda.Fill(ds, "data")
                    If ds.Tables("data").Rows.Count = 0 Then
                        grid.Success = False
                        grid.Message = "No data found."
                    Else
                        grid = DynamicGrid.GridData(ds.Tables("data"), strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "There is an Effor while fatching data, Please contact Admin"
                End Try
            Catch ex As Exception
                grid.Success = False
                grid.Message = "There is an Effor while fatching data, Please contact Admin"
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function


End Class
