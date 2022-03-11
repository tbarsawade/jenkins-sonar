﻿Imports System.Collections.Generic
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

Partial Class InvoicePOReport_194
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
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlClient.SqlConnection(conStr)
            Dim cmd As New SqlClient.SqlCommand("select ReportName,ReportID from MMM_MST_REPORT where eid=194 order by reportid desc", con)

            Try
                con.Open()
                Dim sda As New SqlDataAdapter(cmd)
                Dim ds As New DataSet()
                sda.Fill(ds)
                ddl_reportName.DataSource = ds
                ddl_reportName.DataTextField = "ReportName"
                ddl_reportName.DataValueField = "ReportID"
                ddl_reportName.DataBind()
                con.Close()
            Catch ex As Exception
                Dim ExMsg As String = ex.Message
            End Try

            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd2.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub
    <WebMethod()>
    Public Shared Function GetData(sdate As String, edate As String, reportID As String) As DGrid
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
            If System.Web.HttpContext.Current.Session("USERROLE") = "SU" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid='" + reportID + "'"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid='" + reportID + "'"
            End If
            Dim unused = da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@startDate", sdate)
            qry = Replace(qry, "@endDate", edate)
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
