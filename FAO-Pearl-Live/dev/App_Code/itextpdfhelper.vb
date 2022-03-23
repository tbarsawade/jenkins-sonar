Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports Microsoft.VisualBasic

Public Class itextpdfhelper
    Inherits PdfPageEventHelper
    'Public Overrides Sub OnStartPage(ByVal writer As iTextSharp.text.pdf.PdfWriter, ByVal document As iTextSharp.text.Document)
    '    Dim ch As New Chunk("Page " & writer.PageNumber)
    '    document.Add(ch)
    'End Sub
    Public Overrides Sub OnEndPage(ByVal writer As iTextSharp.text.pdf.PdfWriter, ByVal document As iTextSharp.text.Document)
        Dim times As Font = New Font(Font.FontFamily.TIMES_ROMAN, 8)
        'Dim ch As New Chunk("Page " & writer.PageNumber, times)
        Dim tbl As New PdfPTable(1)
        tbl.DefaultCell.Border = Rectangle.NO_BORDER
        Dim cell As New PdfPCell(New Phrase("Page " & writer.PageNumber, times))
        cell.Border = Rectangle.NO_BORDER
        cell.HorizontalAlignment = 2
        'cell.PaddingLeft = 500.0F
        tbl.AddCell(cell)
        tbl.HorizontalAlignment = Element.ALIGN_RIGHT
        document.Add(tbl)
    End Sub
End Class
