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

Partial Class ItemWiseBulkPayment
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

    End Sub
    <WebMethod()> _
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
       
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim qry As String = ""
            da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=803"
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = qry.Replace("@Frdate", sdate)
            qry = qry.Replace("@Todate", tdate)
            da.SelectCommand.CommandText = qry
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

    Protected Sub btnViewInExcel_Click(sender As Object, e As System.EventArgs) Handles btnViewInExcel.Click
        Dim grdExport As New GridView()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim qry As String = ""
        da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=803"
        da.Fill(ds, "qry")
        Dim d1 = txtd1.Text
        qry = ds.Tables("qry").Rows(0).Item(0).ToString()
        qry = qry.Replace("@Frdate", txtd1.Text.ToString)
        qry = qry.Replace("@Todate", txtd2.Text.ToString)
        da.SelectCommand.CommandText = qry
        da.Fill(ds, "data")
        grdExport.AllowPaging = False
        grdExport.AllowSorting = False
        grdExport.DataSource = ds.Tables("data")
        grdExport.DataBind()
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Item Wise Bulk Payment Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "Item Wise Bulk Payment Report" & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        grdExport.AllowPaging = False
        grdExport.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
End Class
