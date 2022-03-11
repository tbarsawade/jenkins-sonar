Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security

Partial Class GPSvsBilledKMReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindCircleList()
            ddlYear.Items.Add(DateTime.Now.Year - 1)
            ddlYear.Items.Add(DateTime.Now.Year)
            ddlYear.Items.Add(DateTime.Now.Year + 1)
            ddlYear.SelectedIndex = 1

        End If
    End Sub
    'Add Theme Code
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

    Public Sub BindCircleList()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        If Session("USERROLE") = "SU" Then
            oda.SelectCommand.CommandText = "select distinct tid, [Circle Name] from v32Circle_Office where eid=" & Session("EID") & "order by [Circle Name] "
        Else
            oda.SelectCommand.CommandText = "select distinct tid, [Circle Name] from [v32Circle_Office]  where tid in (select * from  split((select fld4 from  mmm_ref_role_user where  eid=" & Session("EID") & " and uid=" & Session("UID") & "),',')) order by [Circle Name] "
        End If

        ds.Clear()
        oda.SelectCommand.CommandTimeout = 3000
        oda.Fill(ds, "Circle")
        For i As Integer = 0 To ds.Tables("Circle").Rows.Count - 1
            CheckListCircle.Items.Add(ds.Tables("Circle").Rows(i).Item(1).ToString())
            CheckListCircle.DataValueField = ds.Tables("Circle").Rows(i).Item(0).ToString()
        Next
        con.Dispose()
        oda.Dispose()
        ds.Dispose()
    End Sub
    Public Sub bindSearchData()

        If CheckListCircle.Items.Count = 0 Then
            lblNotAuthorised.Visible = True
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim CircleList As String = String.Empty
            For i As Integer = 0 To CheckListCircle.Items.Count - 1
                If CheckListCircle.Items(i).Selected = True Then
                    CircleList = CircleList & CheckListCircle.Items(i).Value & ","
                End If
            Next
            If CheckListCircle.SelectedIndex = -1 Then
                For i As Integer = 0 To CheckListCircle.Items.Count - 1
                    CircleList = CircleList & CheckListCircle.Items(i).Value & ","
                Next
            End If
            Dim CircleName As String = CircleList.Trim(",".ToString())
            Dim oda As SqlDataAdapter = New SqlDataAdapter("GPSandBilledKMReport", con)
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            Dim a As String = ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue & ""
            oda.SelectCommand.Parameters.AddWithValue("@smonth", ddlMonth.SelectedValue)
            oda.SelectCommand.Parameters.AddWithValue("@syear", ddlYear.SelectedItem.Text)
            'oda.SelectCommand.Parameters.AddWithValue("@CircleName", CircleName)
            oda.SelectCommand.Parameters.AddWithValue("@CircleName", CircleName)
            Dim ds As New DataSet()
            oda.SelectCommand.CommandTimeout = 1000000
            oda.Fill(ds, "data")
            Dim usrqry As String = String.Empty

            grdgpsbilleddata.DataSource = ds.Tables("data")
            ViewState("Data") = ds.Tables("data")
            ViewState("DataPdf") = ds

            ViewState("Pdf") = ds.Tables("data")
            grdgpsbilleddata.DataBind()

            grdgpsbilleddata.Caption = ""
            con.Close()
            oda.Dispose()
        End If
        ' Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetTripBetween", con)
    End Sub
    Protected Sub btsSearch_Click(sender As Object, e As System.EventArgs) Handles btsSearch.Click
        bindSearchData()
    End Sub
    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1
    Protected Sub grdgpsbilleddata_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grdgpsbilleddata.PageIndexChanging
        Try
            grdgpsbilleddata.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1
            'grdgpsbilleddata.DataBind()
            bindSearchData()
            'updPnlSearch.Update()

        Catch ex As Exception
        End Try
    End Sub

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click


        grdgpsbilleddata.Rows(0).Cells(3).Visible = False
        grdgpsbilleddata.Rows(0).Cells(0).Text = ""
        grdgpsbilleddata.ShowHeader = True
        grdgpsbilleddata.RowHeaderColumn = True
        grdgpsbilleddata.AllowPaging = False
        grdgpsbilleddata.DataSource = ViewState("Data")
        grdgpsbilleddata.DataBind()
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>GPS vs Billed KM Report</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=GPSvsBilledKMReport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To grdgpsbilleddata.Rows.Count - 1
            'Apply text style to each Row 

            grdgpsbilleddata.Rows(i).Attributes.Add("class", "textmode")
        Next
        grdgpsbilleddata.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub

    Protected Sub ToPdf(ByVal newDataSet As DataSet)

        Dim PDFData As New System.IO.MemoryStream()
        Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
        Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
        newDocument.Open()
        '  newDataSet.Tables("data").Remove("tid")
        For Page As Integer = 0 To newDataSet.Tables.Count - 1
            Dim totalColumns As Integer = newDataSet.Tables(Page).Columns.Count
            Dim newPdfTable As New iTextSharp.text.pdf.PdfPTable(totalColumns)
            newPdfTable.DefaultCell.Padding = 4
            newPdfTable.WidthPercentage = 100
            newPdfTable.DefaultCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT
            newPdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE
            newPdfTable.HeaderRows = 1
            newPdfTable.DefaultCell.BorderWidth = 1
            newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
            newPdfTable.DefaultCell.BackgroundColor = New iTextSharp.text.BaseColor(255, 255, 255)
            For i As Integer = 0 To totalColumns - 1
                newPdfTable.AddCell(New Phrase(newDataSet.Tables(Page).Columns(i).ColumnName, FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
            Next

            For Each record As DataRow In newDataSet.Tables(Page).Rows
                For i As Integer = 0 To totalColumns - 1
                    newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
                    newPdfTable.AddCell(New Phrase(record(i).ToString, FontFactory.GetFont("Tahoma", 9, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80))))
                Next
            Next

            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase("GPS vs Billed KM Report ", FontFactory.GetFont("Tahoma", 15, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(0, 80, 0))))

            newDocument.Add(New Phrase(Environment.NewLine))

            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(newPdfTable)
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase("Created By: " & Session("USERNAME"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase("Printed Date: " & DateTime.Now.ToString(), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            '  newDocument.Add(New Phrase("Company Address: " & ViewState("pagefooter"), FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
            If Page < newDataSet.Tables.Count Then
                newDocument.NewPage()
            End If
        Next
        newDocument.Close()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
        Response.AppendHeader("Content-Type", "application/pdf")
        Response.AppendHeader("Content-Disposition", "attachment; filename=GPSvsBilledKMReport")
        Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
        Response.OutputStream.Flush()
        Response.OutputStream.Close()

    End Sub
    Protected Sub btnPdfExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnPdfExport.Click
        If ViewState("Pdf").Rows.Count > 0 Then
            grdgpsbilleddata.DataBind()
            ToPdf(ViewState("DataPdf"))
        End If
    End Sub


End Class

