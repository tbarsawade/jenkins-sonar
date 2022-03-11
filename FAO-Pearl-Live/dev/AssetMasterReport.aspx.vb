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
Imports iTextSharp.text.html.simpleparser
Imports iTextSharp.text

Partial Class AssetMasterReport
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
            'txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            'txtd2.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub
    <WebMethod()> _
    Public Shared Function GetData(Bunit As String) As DGrid
        Dim grid As New DGrid()
        If Bunit = "null" Then
            grid.Message = "Please select location...!"
            Return grid
            grid.Success = False
        End If
        Dim str() As String = Bunit.Split(",")
        Dim s As String = ""
        For i As Integer = 0 To str.Length - 1
            s &= "'" & str(i) & "',"
        Next
        Bunit = s.Trim().Remove(s.Length - 1)
        Dim jsonData As String = ""
        HttpContext.Current.Session("BU") = Bunit
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim qry As String = ""
            If System.Web.HttpContext.Current.Session("USERROLE") = "SU" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=1089"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=1089"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            'qry &= " and convert(date,d.attdate)>='" & sdate & "' and convert(date,d.attdate)<='" & edate & "'"
            qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@BI", Bunit)
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
        Return grid

    End Function
    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetBusinessUnit() As String
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "'"
        If System.Web.HttpContext.Current.Session("USERROLE") <> "SU" Then
            Qry = "select tid[TID],fld1[BU] from  mmm_mst_master with(nolock) where tid in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and UID='" & HttpContext.Current.Session("UID") & "'))) and EID=" & HttpContext.Current.Session("EID") & " and DocumentType='Location Master New'"
        Else
            Qry = "select tid[TID],fld1[BU] from  mmm_mst_master with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='Location Master New'"
        End If

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
    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        ' Verifies that the control is rendered 
    End Sub
    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetExport(Bunit As String) As String
        Dim str() As String = Bunit.Split(",")
        Dim s As String = ""
        For i As Integer = 0 To str.Length - 1
            s &= "'" & str(i) & "',"
        Next
        Bunit = s.Trim().Remove(s.Length - 1)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim qry As String
        Try
            If System.Web.HttpContext.Current.Session("USERROLE") = "SU" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=1089"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=1089"
            End If
            da.SelectCommand.CommandTimeout = 300
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@BI", Bunit)
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandTimeout = 900
            da.Fill(dt)
            Using sw As New StringWriter()
                Using hw As New HtmlTextWriter(sw)
                    'To Export all pages
                    Dim GridView1 As New GridView
                    GridView1.AllowPaging = False
                    GridView1.DataSource = dt
                    GridView1.DataBind()
                    GridView1.RenderControl(hw)
                    Dim sr As New StringReader(sw.ToString())
                    Dim pdfDoc As New Document(PageSize.A2, 10.0F, 10.0F, 10.0F, 0.0F)
                    Dim htmlparser As New HTMLWorker(pdfDoc)
                    PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream)
                    pdfDoc.Open()
                    htmlparser.Parse(sr)
                    pdfDoc.Close()
                    HttpContext.Current.Response.ContentType = "application/pdf"
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=AssetMasterReport.pdf")
                    HttpContext.Current.Response.Write(pdfDoc)
                    HttpContext.Current.Response.[End]()
                    '  Return "/Mailattach/" & "AssetMasterReport.pdf"
                End Using
            End Using
        Catch ex As Exception
            da.SelectCommand.CommandText = "INSERT_ERRORLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & " Error column number:")
            da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FAAUDIT")
            da.SelectCommand.Parameters.AddWithValue("@EID", 109)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Function

    Protected Sub btnPDF_Click(sender As Object, e As System.EventArgs) Handles btnPDF.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim qry As String
        Try
            If System.Web.HttpContext.Current.Session("USERROLE") = "SU" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=1089"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=1089"
            End If
            da.SelectCommand.CommandTimeout = 300
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            qry = Replace(qry, "@UID", HttpContext.Current.Session("UID"))
            qry = Replace(qry, "@BI", HttpContext.Current.Session("BU"))
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandTimeout = 900
            ds.Tables.Clear()
            da.Fill(ds, "data")
            ToPdf(ds)
            'Using sw As New StringWriter()
            '    Using hw As New HtmlTextWriter(sw)
            '        'To Export all pages
            '        Dim GridView1 As New GridView
            '        GridView1.AllowPaging = False
            '        GridView1.DataSource = dt
            '        GridView1.DataBind()

            '        GridView1.RenderControl(hw)
            '        Dim sr As New StringReader(sw.ToString())
            '        Dim pdfDoc As New Document(PageSize.A2, 10.0F, 10.0F, 10.0F, 0.0F)

            '        Dim htmlparser As New HTMLWorker(pdfDoc)
            '        PdfWriter.GetInstance(pdfDoc, Response.OutputStream)
            '        pdfDoc.Open()
            '        htmlparser.Parse(sr)
            '        pdfDoc.Close()

            '        Response.ContentType = "application/pdf"
            '        Response.AddHeader("content-disposition", "attachment;filename=AssetMasterReport.pdf")
            '        Response.Write(pdfDoc)
            '        Response.[End]()
            '        '  Return "/Mailattach/" & "AssetMasterReport.pdf"
            '    End Using
            'End Using
        Catch ex As Exception
            da.SelectCommand.CommandText = "INSERT_ERRORLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & " Error column number:")
            da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FAAUDIT")
            da.SelectCommand.Parameters.AddWithValue("@EID", 109)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub

    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Try
            Dim PDFData As New System.IO.MemoryStream()
            Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
            Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
            newDocument.Open()
            For Page As Integer = 0 To newDataSet.Tables.Count - 1
                Dim totalColumns As Integer = newDataSet.Tables(Page).Columns.Count
                Dim newPdfTable As New iTextSharp.text.pdf.PdfPTable(totalColumns)
                newPdfTable.DefaultCell.Padding = 4
                newPdfTable.WidthPercentage = 100
                newPdfTable.DefaultCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE
                newPdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE
                newPdfTable.HeaderRows = 1
                newPdfTable.DefaultCell.BorderWidth = 1
                newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
                newPdfTable.DefaultCell.BackgroundColor = New iTextSharp.text.BaseColor(255, 255, 255)

                For i As Integer = 0 To totalColumns - 1
                    newPdfTable.AddCell(New Phrase(newDataSet.Tables(Page).Columns(i).ColumnName, FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
                Next

                For Each record As DataRow In newDataSet.Tables(Page).Rows
                    For i As Integer = 0 To totalColumns - 1
                        newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
                        newPdfTable.AddCell(New Phrase(record(i).ToString, FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80))))
                    Next
                Next
                ' Dim gif As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(MapPath("logo") & "\" & Session("CLOGO"))
                'newDocument.Add(gif)
                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(New Phrase("Asset Master Report", FontFactory.GetFont("Tahoma", 12, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(newPdfTable)

                If Page < newDataSet.Tables.Count Then
                    newDocument.NewPage()
                End If
            Next
            newDocument.Close()
            Response.ContentType = "application/pdf"
            Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
            Response.AppendHeader("Content-Type", "application/pdf")
            Response.AppendHeader("Content-Disposition", "attachment; filename=Asset Master Report.pdf")
            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
            Response.OutputStream.Flush()
            Response.OutputStream.Close()
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Sub

End Class
