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
Partial Class HubandBeneficiarySOA
    Inherits System.Web.UI.Page
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))

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

    <WebMethod()>
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Try
            Dim eid As Integer = HttpContext.Current.Session("eid")
            ' ds = GetAllFields(eid)

            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & eid & "  and Reportid=1997 "
            Else
                da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & eid & "  and Reportid=1997 "
            End If
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
            Dim Dept As String = ""
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            Else
                da.SelectCommand.CommandText = "select fld3 from mmm_ref_role_user where eid=" & eid & "  and UID=" & HttpContext.Current.Session("UID") & " "
                Dept = da.SelectCommand.ExecuteScalar().ToString
                If Dept = "" Then
                    Exit Function
                End If
            End If

            str = Replace(str, "@fdate", "'" & sdate & "'")
            str = Replace(str, "@tdate", "'" & tdate & "'")
            str = Replace(str, "@role", "" & Dept & "")


            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            Else
                ' str = Replace(str, "@role", " d.fld20 in (" & Dept & ")")
            End If

            da.SelectCommand.CommandText = str
            da.SelectCommand.CommandTimeout = 1200
            da.SelectCommand.ExecuteNonQuery()
            ''da.SelectCommand.ExecuteNonQuery()
            da.Fill(dt)
            Dim DocumentType As String = ""
            Dim strError As String = ""
            If dt.Rows.Count > 0 Then
                grid = DynamicGrid.GridData(dt, strError)
            Else
                strError = "No data found"
                grid.Column.Clear()
            End If
        Catch ex As Exception

        End Try
        Return grid
    End Function
End Class
