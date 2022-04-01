Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports System.Data.OleDb
Imports System.Xml
Imports System.Net
Imports System.IO.FileInfo


Partial Class TecumScanViewer
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                'BindDropDown(ddlSource.SelectedItem.Text.Trim())
            End If
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try


        Dim strPreviousPage As String = ""
        If Request.UrlReferrer <> Nothing Then
            strPreviousPage = Request.UrlReferrer.Segments(Request.UrlReferrer.Segments.Length - 1)
        End If
        If strPreviousPage = "" Then
            Response.Redirect("~/Invalidaction.aspx")
        End If
    End Sub


    Protected Sub OpenWindow(sender As Object, e As EventArgs)

        If txtbarcode.Text = "" Then
            lblMessage.Text = "Enter Barcode first, please!"
            Exit Sub
        End If



        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim dtData As New DataTable
        Dim EID As Integer = 0
        Dim Query = "select * FROM MMM_MST_doc where EID=7 and documentType='vendor invoice' and fld17='" & txtbarcode.Text & "'"
        Dim ds As New DataSet()
        'Code For populating non editable field with current date
        Using con = New SqlConnection(conStr)
            Using da = New SqlDataAdapter(Query, con)
                da.Fill(ds)
            End Using
        End Using
        Dim str As String = ""
        If ds.Tables(0).Rows.Count > 0 Then
            Dim Result As String = Convert.ToString(ds.Tables(0).Rows(0).Item("fld15"))
            Dim PId As Integer = ds.Tables(0).Rows(0).Item("tid")
            str = "viewpdf.aspx?EidPage=" & Result
        End If
        'Dim url As String = "viewpdf.aspx"
        'Dim s As String = "window.open('" & str + "', '_blank', 'width=800,height=900,left=100,top=100,resizable=yes');"
        Dim s As String = "window.open('" & str + "', '_blank', '');"
        ClientScript.RegisterStartupScript(Me.GetType(), "script", s, True)

        ds.Dispose()
    End Sub


    'Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
    '    If txtbarcode.Text = "" Then
    '        lblMessage.Text = "Enter Barcode first, please!"
    '        Exit Sub
    '    End If



    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim dtData As New DataTable
    '    Dim EID As Integer = 0
    '    Dim Query = "select * FROM MMM_MST_doc where EID=7 and documentType='vendor invoice' and fld17='" & txtbarcode.Text & "'"
    '    Dim ds As New DataSet()
    '    'Code For populating non editable field with current date
    '    Using con = New SqlConnection(conStr)
    '        Using da = New SqlDataAdapter(Query, con)
    '            da.Fill(ds)
    '        End Using
    '    End Using
    '    If ds.Tables(0).Rows.Count > 0 Then
    '        Dim Result As String = ds.Tables(0).Rows(0).Item("fld15")
    '        Dim PId As Integer = ds.Tables(0).Rows(0).Item("tid")
    '        Dim str As String

    '        str = "viewpdf.aspx?" & PId & "&eidpage=" & ds.Tables("data").Rows(0).Item("fld15")
    '        '            Response.Write("<script language='javascript> window.open('" & str & "','_blank');</script>")
    '        ''ScriptManager.RegisterStartupScript(this, typeof(Page), "alert", script, false);
    '        '           Response.End()



    '        ''Dim url As String = Request.QueryString("Mod")
    '        'Dim docid As Integer = PId
    '        'Dim docDtlAuth As String = Result
    '        'If IsNothing(docDtlAuth) = False Then
    '        '    Dim eidNpdf As String() = Split(docDtlAuth, "/")
    '        '    If eidNpdf(0) = "7" Then

    '        '        Try
    '        '            'Dim fullpath As String = "http://localhost:12586/Backup/DOCS/" & docDtlAuth
    '        '            Dim fullpath As String = Server.MapPath("~/DOCS/" & docDtlAuth)
    '        '            Dim ext = Path.GetExtension(fullpath)
    '        '            Dim type As String = ""
    '        '            Select Case ext
    '        '                Case ".htm", ".html"
    '        '                    type = "text/HTML"
    '        '                Case ".txt", ".TXT"
    '        '                    type = "text/plain"
    '        '                Case ".doc", ".rtf", ".DOC", ".RTF"
    '        '                    type = "Application/msword"
    '        '                Case ".docx", ".DOCS"
    '        '                    type = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    '        '                Case ".csv", ".xls", ".XLS"
    '        '                    type = "application/vnd.ms-excel"
    '        '                Case ".xlsx", ".XLSX"
    '        '                    type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    '        '                Case ".pdf", ".PDF"
    '        '                    type = "Application/pdf"
    '        '                Case ".gif", ".GIF"
    '        '                    type = "image/gif"
    '        '                Case ".jpeg", ".jpg", ".JPEG", ".JPG"
    '        '                    type = "image/jpeg"
    '        '                Case ".png", ".PNG"
    '        '                    type = "image/png"
    '        '                Case ".tiff"
    '        '                    type = "image/tiff"
    '        '                Case ".tiff"
    '        '                    type = "image/tiff"
    '        '                Case ".zip", ".ZIP"
    '        '                    type = "application/x-zip-compressed"
    '        '                    'Case Else
    '        '                    '    type = "text/plain"
    '        '            End Select
    '        '            '            Response.Redirect(fullpath)

    '        '            ' removed for erro 27_may sp  Response.AddHeader("content-disposition", "attachment; filename=" & """ & eidNpdf(1) & """)
    '        '            If type <> "" Then
    '        '                Response.ContentType = type
    '        '            End If
    '        '            '  If type = "Application/pdf" Then
    '        '            Dim User = New WebClient()
    '        '            Dim FileBuffer As Byte() = User.DownloadData(fullpath)
    '        '            If Not IsNothing(FileBuffer) Then
    '        '                Response.AddHeader("content-length", FileBuffer.Length.ToString())
    '        '                Response.BinaryWrite(FileBuffer)
    '        '            End If
    '        '            '  Else
    '        '            ' Response.AddHeader("content-disposition", "attachment; filename=" & eidNpdf(1))
    '        '            '  Response.WriteFile(fullpath)
    '        '            '  Response.End()
    '        '            '  End If
    '        '        Catch ex As Exception
    '        '            lblMessage.Text = "Error while downloading file. - msg : " '& ex.InnerException.ToString
    '        '        End Try
    '        '        Exit Sub
    '        '        ' End If
    '        '    End If
    '        'Else
    '        'End If



    '    End If

    'End Sub

End Class
