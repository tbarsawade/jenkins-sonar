Imports System.Data.SqlClient
Imports System.Data
Imports iTextSharp.text
Imports System.IO

Partial Class smslogreport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)

            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            oda.SelectCommand.CommandText = "select Distinct UserName,fld28,uid from mmm_mst_user where fld28 in (select sendornumber from mmm_mst_smslog)  order by Username"
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            UsrVeh.Items.Clear()
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                UsrVeh.Items.Add(ds.Tables("data").Rows(i).Item(0).ToString() & "-" & ds.Tables("data").Rows(i).Item(1).ToString())
                UsrVeh.Items(i).Value = ds.Tables("data").Rows(i).Item("uid").ToString()
            Next
            con.Dispose()
            oda.Dispose()

            ds.Dispose()
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
    Protected Sub checkuncheckr(sender As Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then

            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = True
            Next
            UpdatePanel1.Update()
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = False
            Next
            UpdatePanel1.Update()
        End If


    End Sub
    Protected Sub gvReport_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GVReport.PageIndexChanged

        Show()

    End Sub
    Protected Sub gvReport_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport.PageIndexChanging
        GVReport.PageIndex = e.NewPageIndex
    End Sub
    Protected Sub btnshow_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnshow.Click
        Show()
    End Sub

    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Dim PDFData As New System.IO.MemoryStream()
        Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
        Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
        newDocument.Open()
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
            Dim gif As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(MapPath("logo") & "\" & Session("CLOGO"))
            newDocument.Add(gif)
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase("Report Name: " & "SMS Report", FontFactory.GetFont("Tahoma", 12, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(newPdfTable)
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            'newDocument.Add(New Phrase("Created By: " & Session("USERNAME"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
            'newDocument.Add(New Phrase(Environment.NewLine))
            'newDocument.Add(New Phrase("Printed Date: " & DateTime.Now.ToString(), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
            'newDocument.Add(New Phrase(Environment.NewLine))
            'newDocument.Add(New Phrase(Environment.NewLine))
            'newDocument.Add(New Phrase("Company Address: " & ViewState("pagefooter"), FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
            If Page < newDataSet.Tables.Count Then
                newDocument.NewPage()
            End If
        Next
        newDocument.Close()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
        Response.AppendHeader("Content-Type", "application/pdf")
        Response.AppendHeader("Content-Disposition", "attachment; filename=smsreport.pdf")
        Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
        Response.OutputStream.Flush()
        Response.OutputStream.Close()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim uids As String = ""
        For i As Integer = 0 To UsrVeh.Items.Count - 1
            If UsrVeh.Items(i).Selected = True Then
                uids = uids & UsrVeh.Items(i).Value & ","
            End If
        Next
        uids = uids.Remove(uids.Length - 1)
        oda.SelectCommand.CommandText = "select sendornumber[M Number],Keyword[Keyword],Msgtext[Message Text],Replymsg[Reply Message],Currdate[Date],user1.username from mmm_mst_smslog sms INNER JOIN mmm_mst_user user1 ON  sms.sendornumber like '%' + user1.fld28  +'%'  where  sms.currdate>='" & txtsdate.Text & " " & TxtStime.Text & "' and sms.currdate <='" & txtedate.Text & " " & txtetime.Text & "'  and user1.uid    in (" + uids + ")  order by user1.username,sms.currdate"
        Dim ds As New DataSet()
        oda.Fill(ds, "smsdata")
        ViewState("xlexport") = ds.Tables("smsdata")
        Session("Datar") = ds
        GVReport.DataSource = ds.Tables("smsdata")
        GVReport.DataBind()
        con.Dispose()
        oda.Dispose()
        ds.Dispose()
    End Sub
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        GVReport.AllowPaging = False
        GVReport.DataBind()
        GVReport.DataSource = ViewState("xlexport")

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "smsreport" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=smsreport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GVReport.Rows.Count - 1
            'Apply text style to each Row 
            GVReport.Rows(i).Attributes.Add("class", "textmode")
        Next
        GVReport.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub
    Protected Sub btnExportPDF_Click(sender As Object, e As ImageClickEventArgs) Handles btnExportPDF.Click
        Show()
        Dim ds As DataSet
        ds = CType(Session("Datar"), DataSet)

        If ds.Tables("smsdata").Rows.Count > 0 Then
            ToPdf(ds)
        End If
    End Sub
End Class
