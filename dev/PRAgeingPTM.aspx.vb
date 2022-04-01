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

Partial Class PRAgeingPTM
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
            BindVendor()
            BindDepartment()
            BindCreatedBy()
        End If
    End Sub
    Public Sub BindVendor()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            da.SelectCommand.CommandText = "select fld2 + ' (' + fld1 + ')'[Vendor],tid from mmm_mst_master with(nolock) where eid=180 and isauth=1  and  documenttype='Vendor Master' order by fld2"
            da.Fill(ds, "qry")
            ddlVendor.DataSource = ds.Tables(0)
            ddlVendor.DataTextField = "Vendor"
            ddlVendor.DataValueField = "tid"
            ddlVendor.DataBind()
            ddlVendor.Items.Insert(0, New ListItem("--Select--", "0"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Public Sub BindDepartment()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            da.SelectCommand.CommandText = "select fld1[Department],tid from mmm_mst_master with(nolock) where eid=180 and documenttype='department' and isauth=1  order by fld1"
            da.Fill(ds, "qry")
            ddlDepartment.DataSource = ds.Tables(0)
            ddlDepartment.DataTextField = "Department"
            ddlDepartment.DataValueField = "tid"
            ddlDepartment.DataBind()
            ddlDepartment.Items.Insert(0, New ListItem("--Select--", "0"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Public Sub BindCreatedBy()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            da.SelectCommand.CommandText = "select UID,UserName from mmm_mst_user with(nolock) where eid=180 and isauth=1  order by UserName"
            da.Fill(ds, "qry")
            ddlCreatedBy.DataSource = ds.Tables(0)
            ddlCreatedBy.DataTextField = "UserName"
            ddlCreatedBy.DataValueField = "UID"
            ddlCreatedBy.DataBind()
            ddlCreatedBy.Items.Insert(0, New ListItem("--Select--", "0"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    <WebMethod()>
    Public Shared Function GetData(sdate As String, edate As String, CreatedBy As String, Department As String, Status As String, Entity As String, Vendor As String) As DGrid
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
            da.SelectCommand.CommandText = "PRAgeingReport_PTM"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandTimeout = 5000
            da.SelectCommand.Parameters.AddWithValue("@Frdate", sdate)
            da.SelectCommand.Parameters.AddWithValue("@Todate", edate)
            da.SelectCommand.Parameters.AddWithValue("@CreatedBy", Trim(CreatedBy))
            da.SelectCommand.Parameters.AddWithValue("@Department", Department)
            da.SelectCommand.Parameters.AddWithValue("@Status", Trim(Status))
            da.SelectCommand.Parameters.AddWithValue("@Entity", Trim(Entity))
            da.SelectCommand.Parameters.AddWithValue("@Vendor", Vendor)
            da.Fill(ds, "data")
            'If System.Web.HttpContext.Current.Session("USERROLE") = "SU" Then
            '    da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=2113"
            'Else
            '    da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=2113"
            'End If
            'da.Fill(ds, "qry")
            'qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            'qry = Replace(qry, "@Frdate", sdate)
            'qry = Replace(qry, "@Todate", edate)
            'da.SelectCommand.CommandText = qry
            'da.SelectCommand.CommandTimeout = 900
            'da.Fill(ds, "data")
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
