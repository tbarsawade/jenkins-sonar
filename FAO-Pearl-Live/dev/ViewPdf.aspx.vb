Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.IO.FileInfo


Partial Class ViewPdf
    Inherits System.Web.UI.Page

    'prev b4 auth. removed.... by sp on 15 mya 14
    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
    '    If Not IsPostBack Then
    '        Try
    '            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString.ToString()
    '            Dim con As SqlConnection = New SqlConnection(conStr)
    '            Dim ds As New DataSet
    '            If IsNothing(Session("EID")) = True Then
    '                Exit Sub
    '            Else
    '                If Request.QueryString.HasKeys Then
    '                    Dim url As String = Request.QueryString("Mod")
    '                    Dim docid As Integer = Request.QueryString("docid")
    '                    Dim docDtlAuth As String = Request.QueryString("EidPage")
    '                    If IsNothing(docDtlAuth) = False Then
    '                        Dim eidNpdf As String() = Split(docDtlAuth, "/")
    '                        If eidNpdf(0) = Session("EID").ToString Then
    '                            Dim oda As New SqlDataAdapter("uspGetMultiAuthOnDoc", con)
    '                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            oda.SelectCommand.Parameters.AddWithValue("Uid", Session("UID"))
    '                            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
    '                            oda.Fill(ds, "authUser")
    '                            Dim uids As String = ""
    '                            If ds.Tables("authUser").Rows.Count > 0 Then
    '                                For i As Integer = 0 To ds.Tables("authUser").Rows.Count - 1
    '                                    uids &= ds.Tables("authUser").Rows(i).Item("uid") & ","
    '                                Next
    '                            End If
    '                            uids = uids & Session("UID").ToString()
    '                            oda.SelectCommand.CommandText = "select * from MMM_doc_dtl where docid=" & docid & " and userid in (" & uids & ")"
    '                            oda.SelectCommand.CommandType = CommandType.Text
    '                            oda.Fill(ds, "val")
    '                            If ds.Tables("val").Rows.Count > 0 Or Session("USERROLE") = "SU" Then
    '                                Try
    '                                    'Dim fullpath As String = "http://localhost:12586/Backup/DOCS/" & docDtlAuth
    '                                    Dim fullpath As String = Server.MapPath("~/DOCS/" & docDtlAuth)
    '                                    Dim ext = Path.GetExtension(fullpath)
    '                                    Dim type As String = ""
    '                                    Select Case ext
    '                                        Case ".htm", ".html"
    '                                            type = "text/HTML"
    '                                        Case ".txt"
    '                                            type = "text/plain"
    '                                        Case ".doc", ".rtf"
    '                                            type = "Application/msword"
    '                                        Case ".csv", ".xls"
    '                                            type = "Application/x-msexcel"
    '                                        Case ".pdf"
    '                                            type = "Application/pdf"
    '                                            'Case Else
    '                                            '    type = "text/plain"
    '                                    End Select
    '                                    '            Response.Redirect(fullpath)
    '                                    Response.AddHeader("content-disposition", "attachment; filename=" & eidNpdf(1))
    '                                    If type <> "" Then
    '                                        Response.ContentType = type
    '                                    End If
    '                                    Response.WriteFile(fullpath)
    '                                    Response.End()
    '                                Catch ex As Exception
    '                                    lblMsg.Text = "Error while downloading file. - msg : " '& ex.InnerException.ToString
    '                                End Try
    '                                Exit Sub
    '                            End If
    '                        End If
    '                    Else
    '                    End If
    '                    Session("Draft") = "DRAFT"
    '                    Dim da As New SqlDataAdapter("select * from MMM_MST_doc_draft where eid=" & Session("EID") & " and tid=" & docid & " ", con)
    '                    da.Fill(ds, "data")
    '                    If ds.Tables("data").Rows.Count > 0 Then 'GenerateMailPdf(en & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en)
    '                        Dim ob As New DMSUtil
    '                        Dim pdffile As String = ob.GenerateMailPdf("Static Survey Form_42_print_64", docid, ds.Tables("data").Rows(0).Item("documenttype"))
    '                        'Page.ClientScript.RegisterStartupScript(GetType(),"OpenPDFScript", "window.open(~/MailAttach/" & pdffile & ".pdf\");",True)
    '                        'Response.Redirect("~/MailAttach/" & pdffile & ".pdf")
    '                        Dim path As String = "/MailAttach/" & pdffile & ".pdf"
    '                        Response.Redirect("~" & path, True)
    '                    End If
    '                    con.Close()
    '                    da.Dispose()
    '                    con.Dispose()
    '                End If
    '            End If
    '        Catch ex As Exception
    '            lblMsg.Text = "Error while starting - msg : "
    '        End Try
    '    End If

    'End Sub
    'Add Theme Code

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim strPreviousPage As String = ""
        If Request.UrlReferrer <> Nothing Then
            strPreviousPage = Request.UrlReferrer.Segments(Request.UrlReferrer.Segments.Length - 1)
        End If
        If strPreviousPage = "" Then
            Response.Redirect("~/Invalidaction.aspx")
        End If
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

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString.ToString()
                Dim con As SqlConnection = New SqlConnection(conStr)
                Dim ds As New DataSet
                If IsNothing(Session("EID")) = True Then
                    Exit Sub
                Else
                    If Request.QueryString.HasKeys Then
                        Dim url As String = Request.QueryString("Mod")
                        Dim docid As Integer = Request.QueryString("docid")
                        Dim docDtlAuth As String = Request.QueryString("EidPage")
                        If IsNothing(docDtlAuth) = False Then
                            Dim eidNpdf As String() = Split(docDtlAuth, "/")
                            If eidNpdf(0) = Session("EID").ToString Then
                                'Dim oda As New SqlDataAdapter("uspGetMultiAuthOnDoc", con)
                                'oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                'oda.SelectCommand.Parameters.AddWithValue("Uid", Session("UID"))
                                'oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
                                'oda.Fill(ds, "authUser")
                                'Dim uids As String = ""
                                'If ds.Tables("authUser").Rows.Count > 0 Then
                                '    For i As Integer = 0 To ds.Tables("authUser").Rows.Count - 1
                                '        uids &= ds.Tables("authUser").Rows(i).Item("uid") & ","
                                '    Next
                                'End If
                                'uids = uids & Session("UID").ToString()
                                'oda.SelectCommand.CommandText = "select * from MMM_doc_dtl where docid=" & docid & " and userid in (" & uids & ")"
                                'oda.SelectCommand.CommandType = CommandType.Text
                                'oda.Fill(ds, "val")
                                ' If ds.Tables("val").Rows.Count > 0 Or Session("USERROLE") = "SU" Then
                                Try
                                    'Dim fullpath As String = "http://localhost:12586/Backup/DOCS/" & docDtlAuth
                                    Dim fullpath As String = Server.MapPath("~/DOCS/" & docDtlAuth)
                                    Dim ext = Path.GetExtension(fullpath)
                                    Dim fileName As System.IO.FileInfo = New System.IO.FileInfo(fullpath)

                                    Dim type As String = ""

                                    If ext <> "" Then
                                        Response.Clear()
                                        Response.ContentType = ReturnExtension(fileName.Extension.ToLower())
                                        type = Response.ContentType
                                    End If
                                    If type <> "application/outlook" Then
                                        Dim User = New WebClient()
                                        Dim FileBuffer As Byte() = User.DownloadData(fullpath)
                                        If Not IsNothing(FileBuffer) Then
                                            Response.AddHeader("content-length", FileBuffer.Length.ToString())
                                            Response.BinaryWrite(FileBuffer)
                                            Response.Flush()
                                            Response.End()
                                        End If
                                    Else
                                        Response.AddHeader("content-disposition", "attachment; filename=" & eidNpdf(1))
                                        Response.WriteFile(fullpath)
                                        Response.Flush()
                                        Response.End()
                                    End If
                                Catch ex As Exception
                                    lblMsg.Text = "Error while downloading file. - msg : " '& ex.InnerException.ToString
                                End Try
                                Exit Sub
                                ' End If
                            End If
                        Else
                        End If
                        Session("Draft") = "DRAFT"
                        Dim da As New SqlDataAdapter("select * from MMM_MST_doc_draft where eid=" & Session("EID") & " and tid=" & docid & " ", con)
                        da.Fill(ds, "data")
                        If ds.Tables("data").Rows.Count > 0 Then 'GenerateMailPdf(en & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en)
                            Dim ob As New DMSUtil
                            Dim pdffile As String = ob.GenerateMailPdf("Static Survey Form_42_print_64", docid, ds.Tables("data").Rows(0).Item("documenttype"))
                            'Page.ClientScript.RegisterStartupScript(GetType(),"OpenPDFScript", "window.open(~/MailAttach/" & pdffile & ".pdf\");",True)
                            'Response.Redirect("~/MailAttach/" & pdffile & ".pdf")
                            Dim path As String = "/MailAttach/" & pdffile & ".pdf"
                            Response.Redirect("~" & path, True)
                            Response.Flush()
                            Response.End()
                        End If
                        con.Close()
                        da.Dispose()
                        con.Dispose()
                    End If
                End If
            Catch ex As Exception
                lblMsg.Text = "Error while starting - msg : "
            End Try
        End If

    End Sub

    Public Function ReturnExtension(ByVal fileExtentsion As String) As String
        Select Case fileExtentsion.Trim().ToLower()
            Case ".zip"
                Return "application/x-zip-compressed"
            Case ".rar", ".RAR"
                Return "application/x-rar-compressed"
            Case ".html", ".htm"
                Return "text/HTML"
            Case ".txt", ".TXT"
                Return "text/plain"
            Case ".doc", ".rtf", ".DOC", ".RTF"
                Return "application/msword"
            Case ".docx", ".DOCS"
                Return "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Case ".tiff"
                Return "image/tiff"
            Case ".tif"
                Return "image/tiff"
            Case ".asf"
                Return "video/x-ms-asf"
            Case ".avi"
                Return "video/avi"
            Case ".csv", ".xls", ".XLS"
                Return "application/x-msexcel"
            Case ".xlsx", ".XLSX"
                Return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case ".gif"
                Return "image/gif"
            Case ".jpg"
                Return "image/jpeg"
            Case ".jpeg", ".jpg", ".JPEG", ".JPG"
                Return "image/jpeg"
            Case ".png", ".PNG"
                Return "image/png"
            Case ".bmp"
                Return "image/bmp"
            Case ".wav"
                Return "audio/wav"
            Case ".mp3"
                Return "audio/mpeg3"
            Case "mp4"
                Return "video/mp4"
            Case ".mpg"
                Return "video/mpeg"
            Case ".mpeg"
                Return "video/mpeg"
            Case ".rtf"
                Return "application/rtf"
            Case ".asp"
                Return "text/asp"
            Case ".pdf", ".PDF"
                Return "application/pdf"
            Case ".fdf"
                Return "application/vnd.fdf"
            Case ".ppt"
                Return "application/mspowerpoint"
            Case ".pps"
                Return "application/vnd.ms-powerpoint"
            Case ".pptx"
                Return "application/vnd.openxmlformats-officedocument.presentationml.presentation"
            Case ".ppsx"
                Return "application/vnd.openxmlformats-officedocument.presentationml.slideshow"
            Case ".dwg"
                Return "image/vnd.dwg"
            Case ".msg", ".MSG"
                Return "application/outlook"
            Case ".xml"
                Return "application/xml"
            Case ".sdxl"
                Return "application/xml"
            Case ".xdp"
                Return "application/vnd.adobe.xdp+xml"
            Case Else
                Return "application/octet-stream"
        End Select
    End Function

End Class
