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

Partial Class PayUPRToInvRptNew
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
            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd2.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub
    '<WebMethod()>
    'Public Shared Function GetData(sdate As String, tdate As String) As DGrid
    '    Dim jsonData As String = ""
    '    Dim grid As New DGrid()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim eid As Integer = HttpContext.Current.Session("eid")

    '    Try
    '        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
    '            da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & eid & "  and Reportid=1998 "
    '        Else
    '            da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & eid & "  and Reportid=1998 "
    '        End If

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If



    '        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
    '        Dim Dept1 As String = ""
    '        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
    '        Else
    '            da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & eid & "  and UID=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' "
    '            Dept1 = da.SelectCommand.ExecuteScalar().ToString
    '            If Dept1 = "" Then
    '                Exit Function
    '            End If
    '            str = Replace(str, "@role", " d.fld2 in (" & Dept1 & ")")
    '        End If

    '        str = Replace(str, "@date1", " convert(date,'" & sdate.ToString & "')")
    '        str = Replace(str, "@date2", " convert(date,'" & tdate.ToString & "')")

    '        Dim dt As New DataTable
    '        da.SelectCommand.CommandText = str
    '        da.SelectCommand.CommandTimeout = 1200
    '        da.SelectCommand.ExecuteNonQuery()
    '        ''da.SelectCommand.ExecuteNonQuery()
    '        da.Fill(dt)
    '        Dim strError = ""
    '        grid = DynamicGrid.GridData(dt, strError)
    '        If dt.Rows.Count = 0 Then
    '            grid.Message = "No data found...!"
    '            grid.Success = False
    '        End If
    '    Catch Ex As Exception
    '        grid.Success = False
    '        grid.Message = "No data found...!"
    '        grid.Count = 0

    '    Finally
    '        con.Close()
    '        da.Dispose()
    '        con.Dispose()
    '    End Try
    '    Return grid
    'End Function

    <WebMethod()>
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        Dim eid As Integer = HttpContext.Current.Session("eid")

        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.CommandText = "Payu_PRtoPOtoINV_Report"
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("date1", sdate.ToString)
            oda.SelectCommand.Parameters.AddWithValue("date2", tdate.ToString)
           
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim dt As New DataTable
            oda.SelectCommand.CommandTimeout = 1200
            'oda.SelectCommand.ExecuteNonQuery()
            ''da.SelectCommand.ExecuteNonQuery()
            oda.Fill(dt)

            If dt.Columns.Contains("ErrorMessage") = True Then
                grid.Message = "Error in processing report, please contact system admin"
                grid.Success = False
                Return grid
            End If


            'Dim str As String = da.SelectCommand.ExecuteScalar().ToString
            'Dim Dept1 As String = ""
            'If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            'Else
            '    da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & eid & "  and UID=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' "
            '    Dept1 = da.SelectCommand.ExecuteScalar().ToString
            '    If Dept1 = "" Then
            '        Exit Function
            '    End If
            '    str = Replace(str, "@role", " d.fld2 in (" & Dept1 & ")")
            'End If


            Dim strError = ""
            grid = DynamicGrid.GridData(dt, strError)
            If dt.Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "Please contact system admin."
            grid.Count = 0

        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        Return grid
    End Function
End Class
