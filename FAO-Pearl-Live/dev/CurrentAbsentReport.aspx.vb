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

Partial Class CurrentAbsentReport
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
    Public Shared Function GetData(sdate As String, edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim qry As String = ""
            If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=963"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=963"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@Frdate", sdate)
            qry = Replace(qry, "@Todate", edate)
            qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandTimeout = 600
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
    Public Sub Exportdata()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim qry As String = ""
        If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
            da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=963"
        Else
            da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=963"
        End If
        qry = Replace(qry, "@Frdate", txtd1.Text)
        qry = Replace(qry, "@Todate", txtd2.Text)
        qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
        da.Fill(ds, "qry")
        qry = ds.Tables("qry").Rows(0).Item(0).ToString()
        da.SelectCommand.CommandText = qry
        da.Fill(ds, "data")
        Try
            Dim gvReport As New GridView
            gvReport.AllowPaging = False
            gvReport.AllowSorting = False
            gvReport.DataSource = ds.Tables("data")
            gvReport.DataBind()
            Response.Clear()
            Response.Buffer = True
            Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Attendance Report" & "</h3></div> <br/>")
            Response.AddHeader("content-disposition", "attachment;filename=" & "Attendance Report" & ".xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            For i = 0 To gvReport.HeaderRow.Cells.Count - 1
                For j = 0 To gvReport.Rows.Count - 1
                    gvReport.Rows(j).Cells(i).Attributes.Add("class", "textmode")
                Next
            Next
            gvReport.AllowPaging = False
            gvReport.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()

        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
        End Try
    End Sub
    Public Sub ExportToCSV()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim qry As String
        If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
            da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=963"
        Else
            da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=963"
        End If
        da.Fill(ds, "qry")
        qry = ds.Tables("qry").Rows(0).Item(0).ToString()
        qry = Replace(qry, "@Frdate", txtd1.Text)
        qry = Replace(qry, "@Todate", txtd2.Text)
        qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
        da.SelectCommand.CommandText = qry
        da.SelectCommand.CommandTimeout = 900
        da.Fill(ds, "data")

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", _
                "attachment;filename=AbsentReport.csv")
        Response.Charset = ""
        Response.ContentType = "application/text"

        Dim sb As New StringBuilder()
        For k As Integer = 0 To ds.Tables("data").Columns.Count - 1
            'add separator
            sb.Append(ds.Tables("data").Columns(k).ColumnName + ","c)
        Next
        'append new line
        sb.Append(vbCr & vbLf)
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            For k As Integer = 0 To ds.Tables("data").Columns.Count - 1
                'add separator
                sb.Append(ds.Tables("data").Rows(i)(k).ToString().Replace(",", ";") + ","c)
            Next
            'append new line
            sb.Append(vbCr & vbLf)
        Next
        Response.Output.Write(sb.ToString())
        Response.Flush()
        Response.End()
    End Sub


    Protected Sub btnExp_Click(sender As Object, e As System.EventArgs) Handles btnExp.Click
        ExportToCSV()
    End Sub
End Class
