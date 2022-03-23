Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports iTextSharp.text.pdf
Imports System.Web.Services
Imports System.Web.Hosting
Imports iTextSharp.text
Imports System.IO.Compression
Imports Ionic.Zip


Partial Class BulkDownloader
    Inherits System.Web.UI.Page
    Dim count As Integer = 0
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
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("doctype") Is Nothing Then
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FormDesc,formcaption,displayName,FieldType,DropDownType,dropdown,FieldMapping,LayoutType,isrequired,datatype,fieldid,cal_fields,autofilter  FROM MMM_MST_FIELDS FF with(nolock) left outer JOIN MMM_MST_FORMS F with(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isSearch=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & Session("doctype") & "'  order by displayOrder", con)
            Try
                Dim ds As New DataSet()
                oda.Fill(ds, "fields")
                Dim ob As New DynamicForm()
                ob.CreateControlsOnAdvanceSearch(ds.Tables("fields"), pnlFields)
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    oda.Dispose()
                    con.Dispose()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If
            End Try
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("select distinct formname,formcaption from mmm_mst_forms with(nolock) where eid='" & Session("EID") & "' and formtype='document'  and formsource='MENU DRIVEN' ", con)
            Try
                Dim ds As New DataSet
                da.Fill(ds, "data")
                ddlDocType.DataSource = ds
                ddlDocType.DataTextField = "formcaption"
                ddlDocType.DataValueField = "formname"
                ddlDocType.DataBind()
                ddlDocType.Items.Insert(0, "--Please Select--")
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    da.Dispose()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
        End If
    End Sub
    Public Sub BindGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'Dim oda As New SqlCommand
        Dim tbtype As String = ""
        Dim table As String = ""
        Dim doc As String = ""
        Dim ob As New DynamicForm
        oda.SelectCommand.CommandType = CommandType.Text
        Try
            oda.SelectCommand.CommandText = "select formtype from mmm_mst_forms with(nolock) where eid=" & Session("EID") & " and formname='" & ddlDocType.SelectedValue.ToString & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            tbtype = oda.SelectCommand.ExecuteScalar()

            If tbtype.ToString.ToUpper = "MASTER" Then
                table = "MMM_MST_MASTER"
            Else
                table = "MMM_MST_DOC"
            End If
            doc = ob.UserDataFilter_PreRole(ddlDocType.SelectedValue.ToString(), table)

            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            'oda.SelectCommand.CommandText = "uspGetSearchDocTypeSUIDNew"
            'oda.SelectCommand.CommandText = "uspGetSearchDocTypeSUIDNewWithPreRole"
            oda.SelectCommand.CommandText = "uspGetSearchDocTypeSUIDWithPreRole"
            oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
            oda.SelectCommand.Parameters.AddWithValue("@documentType", ddlDocType.SelectedValue.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@val", txtSearch.Text)
            oda.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))
            If IsNothing(Session("SUBUID")) Then
                oda.SelectCommand.Parameters.AddWithValue("@suid", "0")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@suid", Session("SUBUID"))
            End If
            If doc = "" Then
                oda.SelectCommand.Parameters.AddWithValue("@docid", "0")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@docid", doc.ToString)
            End If
            oda.SelectCommand.Parameters.AddWithValue("@urole", Session("USERROLE"))
            Dim ds As New DataSet
            oda.SelectCommand.CommandTimeout = 120
            oda.Fill(ds, "pending")
            ViewState("pending") = ds.Tables("pending")
            'counting rows

            ' gvPending.DataSourcp[e = oda.ExecuteNonQuery()

            'ViewState("cntp") = CInt(Math.Ceiling(ds.Tables("pending").Rows.Count / gvPending.PageSize))
            'lbltotpending.Text = Trim("Displaying page no. " & currentPageNumberp & " of total page no(s) " & ViewState("cntp"))
            'Current page number
            If ds.Tables("pending").Rows.Count > 0 Then
                gvPending.DataSource = ds.Tables("pending")
                gvPending.DataBind()
                lblmsg1.Text = ""
                gvPending.Visible = True
            Else
                gvPending.Controls.Clear()
                lblmsg1.Text = "Data not found.."
            End If
            'ViewState("cntp") = CInt(Math.Ceiling(ds.Tables("pending").Rows.Count / gvPending.PageSize))
            'lbltotpending.Text = Trim("Displaying page no. " & currentPageNumberp & " of total page no(s) " & ViewState("cntp"))
            'Current page number
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                oda.Dispose()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        BindGrid()
    End Sub
    Protected Sub gvPending_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPending.PageIndexChanging
        Try
            gvPending.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1
            AdvSearch()
            BindGrid()
        Catch ex As Exception
        End Try

    End Sub
    Private Const ASCENDING As String = " ASC"
    Private Const DESCENDING As String = " DESC"
    Public Property GridViewSortDirection As SortDirection
        Get
            If Val(ViewState("sortDirection")) = Val(DBNull.Value.ToString) Then
                ViewState("sortDirection") = SortDirection.Ascending
            End If
            Return CType(ViewState("sortDirection"), SortDirection)
        End Get
        Set(ByVal value As SortDirection)
            ViewState("sortDirection") = value
        End Set
    End Property
    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1
    Protected Sub ChangePagep(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "Previous"
                currentPageNumberp = Int32.Parse(ViewState("cpnp")) - 1
                If gvPending.PageIndex > 0 Then
                    gvPending.PageIndex = gvPending.PageIndex - 1
                End If
                Exit Select
            Case "Next"
                currentPageNumberp = Int32.Parse(ViewState("cpnp")) + 1
                gvPending.PageIndex = gvPending.PageIndex + 1
                Exit Select
        End Select
    End Sub
    Private Sub SortGridView(ByVal sortExpression As String, ByVal direction As String)
        'You can cache the DataTable for improving performance
        Dim dt As DataTable = CType(ViewState("pending"), DataTable)
        dt.DefaultView.Sort = sortExpression + direction
        Dim dt1 As DataTable = dt.DefaultView.ToTable()
        gvPending.DataSource = ViewState("pending")
        gvPending.DataBind()

        'BindGrid()
        'AdvSearch()
        'ViewState("data") = dt1
    End Sub

    Protected Sub gvPending_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvPending.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Or e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(1).Visible = False
        End If
    End Sub
    Protected Sub gvPending_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvPending.Sorting
        Dim sortExpression As String = e.SortExpression
        If (GridViewSortDirection = SortDirection.Ascending) Then
            GridViewSortDirection = SortDirection.Descending
            SortGridView(sortExpression, DESCENDING)
        Else
            GridViewSortDirection = SortDirection.Ascending
            SortGridView(sortExpression, ASCENDING)
        End If
    End Sub
    Protected Sub btnAdvanceSearch_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnAdvanceSearch.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If ddlDocType.SelectedItem.Text = "--Please Select--" Then
            lblmsg1.Text = "Please select any Document Type. "
            Exit Sub
        End If
        Dim ob As New DynamicForm()
        ob.CLEARDYNAMICFIELDS(pnlFields)
        btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub btnDynamicSearch_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnDynamicSearch.Click
        AdvSearch()
    End Sub
    Protected Sub AdvSearch()
        Dim datatype As String = ""
        gvPending.Visible = False
        Dim str As String = ""
        Dim strtn As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select *  from mmm_mst_fields with(nolock) where isSearch=1 and eid='" & Session("EID") & "' and documenttype='" & ddlDocType.SelectedValue.ToString() & "'  ", con)
        Dim ds1 As New DataSet
        Dim ds As New DataTable
        Try
            da.Fill(ds)
            Dim ob As New DynamicForm()
            strtn = ob.getsearchresult(ds, pnlFields)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Try
                'Dim oda As New SqlCommand
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                'oda.SelectCommand.CommandText = "uspGetAdvSearchDocTypeSUIDNew"
                oda.SelectCommand.CommandText = "uspGetAdvSearchDocTypeSUIDPreRole"
                da.SelectCommand.CommandTimeout = 600
                oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                oda.SelectCommand.Parameters.AddWithValue("@documentType", ddlDocType.SelectedValue.ToString())
                oda.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))
                If IsNothing(Session("SUBUID")) Then
                    oda.SelectCommand.Parameters.AddWithValue("@suid", "0")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@suid", Session("SUBUID"))
                End If
                oda.SelectCommand.Parameters.AddWithValue("@urole", Session("USERROLE"))
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                str = oda.SelectCommand.ExecuteScalar()
                If strtn = "" Then
                    str = str
                Else
                    strtn = strtn.Remove(strtn.Length - 4)
                    str = str & " and " & strtn
                End If

                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.Fill(ds1, "data")
                ViewState("pending") = ds1.Tables("data")
                If ds1.Tables("data").Rows.Count > 0 Then
                    gvPending.DataSource = ds1.Tables("Data")
                    gvPending.DataBind()
                    gvPending.Visible = True
                    lblmsg1.Text = ""
                Else
                    lblmsg1.Text = "Data not found.."
                    gvPending.Controls.Clear()
                End If
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    oda.Dispose()
                    con.Dispose()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If
            End Try
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
            ds.Dispose()
        End Try

    End Sub
    Protected Sub ddlDocType_TextChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.TextChanged
        Session("doctype") = ddlDocType.SelectedValue.ToString()
        txtSearch.Text = ""
        BindGrid()
    End Sub


    Protected Sub ChkDocid()
        Dim str As String = String.Empty
        Dim count As Integer = 0
        For Each gvrow As GridViewRow In gvPending.Rows
            Dim chk As CheckBox = DirectCast(gvrow.FindControl("ChkDocid"), CheckBox)
            If chk.Checked = True Then
                count = count + 1
                ViewState("count") = count
                str = gvrow.Cells(1).Text
                Dim TemplateID As Integer = 92
                Print(TemplateID, str)
            End If
        Next
    End Sub
    Protected Sub CopyAttachments()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim filenamestr As String = ""

        Try
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select fieldmapping from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and Fieldtype='File Uploader' and  eid=" & Session("EID") & ""
            da.Fill(ds, "data")
            Dim sourcepath As String = HostingEnvironment.MapPath("~/Docs/" & Session("EID") & "/")
            Dim DestPath As String = HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text)
            Dim filename As String = ""
            Dim filedname As String = ""
            If ds.Tables("data").Rows.Count > 0 Then
                For i = 0 To ds.Tables("data").Rows.Count - 1
                    Dim str As String = String.Empty
                    For Each gvrow As GridViewRow In gvPending.Rows
                        Dim chk As CheckBox = DirectCast(gvrow.FindControl("ChkDocid"), CheckBox)
                        If chk.Checked = True Then
                            str = gvrow.Cells(1).Text
                            filedname = ds.Tables("data").Rows(i).Item("fieldmapping").ToString
                            da.SelectCommand.CommandText = "select " & filedname & " from mmm_mst_doc with(nolock) where tid='" & str & "' and  eid=" & Session("EID") & ""
                            da.Fill(ds, "file")
                            If ds.Tables("file").Rows.Count > 0 Then
                                filename = ds.Tables("file").Rows(0).Item(0).ToString()

                                If filename.ToUpper <> "NULL" And filename <> "" Then
                                    filename = Replace(filename, Session("EID") & "/", "")
                                    da.SelectCommand.CommandText = "select distinct dropdowntype,fieldmapping,dropdown from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and displayname='" & ddlFieldName.SelectedItem.Text & "' and eid=" & Session("EID") & ""
                                    da.SelectCommand.CommandTimeout = 600
                                    da.Fill(ds, "formtype")
                                    Dim formtype As String = ds.Tables("formtype").Rows(0).Item("dropdowntype").ToString
                                    Dim fieldmapping As String = ds.Tables("formtype").Rows(0).Item("fieldmapping").ToString
                                    count = count + 1
                                    If formtype.ToUpper = "MASTER VALUED" Then
                                        da.SelectCommand.CommandText = "select dms.udf_split('" & ds.Tables("formtype").Rows(0).Item("dropdown").ToString & "'," & fieldmapping & ")[Name] from mmm_mst_doc with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and tid=" & str & " and eid=" & Session("EID") & ""
                                    Else
                                        da.SelectCommand.CommandText = "select  " & fieldmapping & "[Name] from mmm_mst_doc with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and tid=" & str & " and eid=" & Session("EID") & ""
                                    End If
                                    da.SelectCommand.CommandTimeout = 600
                                    da.Fill(ds, "filename")
                                    If ds.Tables("filename").Rows.Count = 1 Then
                                        filenamestr = txtFileName.Text & ds.Tables("filename").Rows(0).Item("Name").ToString & "_" & str & "_" & Now.Millisecond & count
                                    Else
                                        If txtFileName.Text <> "" Then
                                            filenamestr = txtFileName.Text & ds.Tables("filename").Rows(0).Item("Name").ToString & "_" & str & "_" & Now.Millisecond & count
                                        Else
                                            filenamestr = str & "_" & Now.Millisecond & count
                                        End If
                                    End If

                                    Dim file = New FileInfo(sourcepath & filename)
                                    file.CopyTo(Path.Combine(DestPath, filenamestr & "_" & str & "_" & filename), True)
                                Else
                                    Continue For
                                End If

                            Else
                                Continue For
                            End If

                        End If
                    Next
                Next
            End If
            da.SelectCommand.CommandText = "select distinct dropdown from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and fieldtype='Child Item' and eid=" & Session("EID") & " "
            da.SelectCommand.CommandTimeout = 600
            da.Fill(ds, "childdoc")
            If ds.Tables("childdoc").Rows.Count > 0 Then
                For i = 0 To ds.Tables("childdoc").Rows.Count - 1
                    da.SelectCommand.CommandText = "select fieldmapping from mmm_mst_fields with(nolock) where documenttype='" & ds.Tables("childdoc").Rows(i).Item("dropdown") & "' and Fieldtype='File Uploader' and  eid=" & Session("EID") & ""
                    da.SelectCommand.CommandTimeout = 600
                    da.Fill(ds, "childFieldmap")
                    For j = 0 To ds.Tables("childFieldmap").Rows.Count - 1
                        Dim str As String = String.Empty
                        For Each gvrow As GridViewRow In gvPending.Rows
                            Dim chk As CheckBox = DirectCast(gvrow.FindControl("ChkDocid"), CheckBox)
                            If chk.Checked = True Then
                                str = gvrow.Cells(1).Text
                                filedname = ds.Tables("childFieldmap").Rows(j).Item("fieldmapping").ToString
                                da.SelectCommand.CommandText = "select " & filedname & "[fieldname] from mmm_mst_doc_item with(nolock) where docid='" & str & "'"
                                da.Fill(ds, "file")
                                For k As Integer = 0 To ds.Tables("file").Rows.Count - 1
                                    filename = ds.Tables("file").Rows(k).Item("fieldname").ToString()
                                    filename = Replace(filename, Session("EID") & "/", "")
                                    If filename <> "" Or filename <> "NULL" Then
                                        Dim file = New FileInfo(sourcepath & filename)
                                        file.CopyTo(Path.Combine(DestPath, txtFileName.Text & "_" & str & "_" & filename), True)
                                    Else
                                        Continue For
                                    End If
                                Next

                            End If
                        Next
                    Next
                Next
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
            ds.Dispose()
        End Try

    End Sub
    Protected Sub CopyCoverSheet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim str As String = String.Empty
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "select * from mmm_print_template with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and isprint='1' and  eid=" & Session("EID") & ""
        da.SelectCommand.CommandTimeout = 600
        da.Fill(ds, "data")
        Try
            If ds.Tables("data").Rows.Count = 1 Then
                Dim count As Integer = 0
                For Each gvrow As GridViewRow In gvPending.Rows
                    Dim chk As CheckBox = DirectCast(gvrow.FindControl("ChkDocid"), CheckBox)
                    If chk.Checked = True Then
                        count = count + 1
                        ViewState("count") = count
                        str = gvrow.Cells(1).Text
                        Dim TemplateID As Integer = ds.Tables("data").Rows(0).Item("tid")
                        PrintCoverSheet(TemplateID, str)
                    End If
                Next
            End If
           
        Catch ex As Exception

        End Try
    End Sub
    Sub Zip()
        Dim path As String = HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text)
        Dim filenames As String() = Directory.GetFiles(path)
        Using zip As New ZipFile()
            zip.AddFiles(filenames, "files")
            '  HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text)
            zip.Save(HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
        End Using
    End Sub
    Public Sub PrintCoverSheet(ByVal TemplateID As Integer, ByVal DocID As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "select * from MMM_Print_Template with(nolock) where tid='" & TemplateID & "' and draft='original' and isprint='1'"
        da.SelectCommand.CommandTimeout = 600
        Dim ds As New DataSet
        da.Fill(ds, "data1")
        da.SelectCommand.CommandType = CommandType.Text
        If ds.Tables("data1").Rows.Count <> 1 Then
            Exit Sub
        End If
        Dim body As String = ds.Tables("data1").Rows(0).Item("body").ToString()
        Dim MainQry As String = ds.Tables("data1").Rows(0).Item("qry").ToString()
        Dim childQry As String = ds.Tables("data1").Rows(0).Item("SQL_CHILDITEM").ToString()
        Dim DocType As String = ds.Tables("data1").Rows(0).Item("Documenttype").ToString()
        Dim moveqry As String = ds.Tables("data1").Rows(0).Item("SQL_MOV_DTL").ToString()
        ' Start  signature image
        Try
            Dim EID As String = ds.Tables("data1").Rows(0).Item("EID").ToString()
            Dim fldmap As String = ""
            da.SelectCommand.CommandText = MainQry.Replace("@tid", DocID)
            da.Fill(ds, "main")
            For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
                body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
            Next
            da.SelectCommand.CommandText = childQry.Replace("@tid", DocID)
            da.Fill(ds, "child")
            ds.Dispose()
            Dim strChildItem As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
            Dim prevVal As String = ""
            For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
                If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
                    prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                    ds.Tables("child").Rows(i).Item(0) = ""
                Else
                    prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                End If
            Next

            For i As Integer = 0 To ds.Tables("child").Rows.Count
                strChildItem &= "<tr>"
                For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
                    strChildItem &= "<td text-align:left>"
                    If i = 0 Then
                        strChildItem &= ds.Tables("child").Columns(j).ColumnName
                    Else
                        strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                    End If
                    strChildItem &= "</td>"
                Next
                strChildItem &= "</tr>"
            Next
            strChildItem &= "</table></div>"
            body = body.Replace("[child item]", strChildItem)

            If body.Contains("[movdtl]") Then
                Dim hub As String = ds.Tables("main").Rows(0).Item("Hub Name").ToString()
                da.SelectCommand.CommandText = moveqry.Replace("@hub", hub)
                da.Fill(ds, "movdtl")
                Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                Dim preMovvVal As String = ""
                For i As Integer = 0 To ds.Tables("movdtl").Rows.Count
                    stmov &= "<tr>"
                    For j As Integer = 0 To ds.Tables("movdtl").Columns.Count - 1
                        stmov &= "<td text-align:left>"
                        If i = 0 Then
                            stmov &= ds.Tables("movdtl").Columns(j).ColumnName
                        Else
                            stmov &= ds.Tables("movdtl").Rows(i - 1).Item(j).ToString()
                        End If
                        stmov &= "</td>"
                    Next
                    stmov &= "</tr>"
                Next
                stmov &= "</table></div>"
                body = body.Replace("[movdtl]", stmov)
            End If

            WritePdf(body, DocID)
            ' Return filename
        Catch ex As Exception
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub
    Public Sub Print(ByVal TemplateID As Integer, ByVal DocID As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        TemplateID = 92
        Dim da As New SqlDataAdapter("select * from MMM_Print_Template with(nolock) where tid=" & TemplateID, con)
        Dim ds As New DataSet
        Try
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count <> 1 Then
                da.Dispose()
                con.Dispose()
                Exit Sub
            End If
            Dim body As String = ds.Tables("data").Rows(0).Item("body").ToString()
            If body.Contains("[Main]") Then
                Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetDetailGridByDocid_Print", con)
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("@pid", DocID)
                oda.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
                oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                oda.Fill(ds, "Main")
                If ds.Tables("Main").Rows.Count > 0 Then
                    Dim stmov As String = "<div border=""2"" border-radius=""25"" border-color=""#FF8330""><table width=""80%"" height=""400"" border=""0.5"" text-align=""center"" align=""center""   >"
                    For i As Integer = 0 To ds.Tables("Main").Rows.Count
                        For j As Integer = 0 To ds.Tables("Main").Columns.Count - 1
                            If i = 0 Then
                                stmov &= "<tr>"
                                stmov &= "<td text-align:left>"
                                stmov &= ds.Tables("Main").Columns(j).ColumnName
                                stmov &= "</td>"
                                stmov &= "<td text-align:left>"
                                stmov &= ds.Tables("Main").Rows(i).Item(j).ToString()
                                stmov &= "</td>"
                                stmov &= "</tr>"
                            End If
                        Next
                    Next
                    stmov &= "</table></div>"
                    body = body.Replace("[Main]", stmov)
                    body = body.Replace("[DocumentType]", ddlDocType.SelectedValue.ToString())
                End If
            End If

            If body.Contains("[Child]") Then
                da.SelectCommand.CommandText = "select distinct dropdown from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and fieldtype='Child Item' and eid=" & Session("EID") & " "
                da.SelectCommand.CommandTimeout = 600
                da.Fill(ds, "childdoctype")
                Dim childdocttype As String = ""
                Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                For k As Integer = 0 To ds.Tables("childdoctype").Rows.Count - 1
                    childdocttype = ds.Tables("childdoctype").Rows(k).Item("dropdown").ToString
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetDetailITEMDetail_Print", con)
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("DOCID", DocID)
                    oda.SelectCommand.Parameters.AddWithValue("FN", childdocttype)
                    oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
                    oda.Fill(ds, "Child")
                    If ds.Tables("Child").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("Child").Rows.Count
                            stmov &= "<tr>"
                            For j As Integer = 0 To ds.Tables("Child").Columns.Count - 1
                                stmov &= "<td text-align:left>"
                                If i = 0 Then
                                    stmov &= ds.Tables("Child").Columns(j).ColumnName
                                Else
                                    stmov &= ds.Tables("Child").Rows(i - 1).Item(j).ToString()
                                End If
                                stmov &= "</td>"
                            Next
                            stmov &= "</tr>"
                        Next
                        stmov &= "</table></div><br/><br/>"
                    End If
                Next
                body = body.Replace("[Child]", stmov)
                body = body.Replace("[ChildDoctype]", childdocttype)
                body = body.Replace("[Child]", "")
            End If

            If body.Contains("[MovementDtl]") Then
                Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetMoveDetailwithRole_Print", con)
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("DOCID", DocID)
                oda.Fill(ds, "MovementDtl")
                If ds.Tables("MovementDtl").Rows.Count > 0 Then
                    Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    For i As Integer = 0 To ds.Tables("MovementDtl").Rows.Count
                        stmov &= "<tr>"
                        For j As Integer = 0 To ds.Tables("MovementDtl").Columns.Count - 1
                            stmov &= "<td text-align:left>"
                            If i = 0 Then
                                stmov &= ds.Tables("MovementDtl").Columns(j).ColumnName
                            Else
                                stmov &= ds.Tables("MovementDtl").Rows(i - 1).Item(j).ToString()
                            End If
                            stmov &= "</td>"
                        Next
                        stmov &= "</tr>"
                    Next
                    stmov &= "</table></div>"
                    body = body.Replace("[MovementDtl]", stmov)
                    body = body.Replace("[MovementsDetails]", "Movement Details")
                End If
            End If
            If body.Contains("[Smovdtl]") Then
                Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectFuturePath_Print", con)
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("DOCID", DocID)
                oda.Fill(ds, "Smovdtl")
                If ds.Tables("Smovdtl").Rows.Count > 0 Then
                    Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    For i As Integer = 0 To ds.Tables("Smovdtl").Rows.Count
                        stmov &= "<tr>"
                        For j As Integer = 0 To ds.Tables("Smovdtl").Columns.Count - 1
                            stmov &= "<td text-align:left>"
                            If i = 0 Then
                                stmov &= ds.Tables("Smovdtl").Columns(j).ColumnName
                            Else
                                stmov &= ds.Tables("Smovdtl").Rows(i - 1).Item(j).ToString()
                            End If
                            stmov &= "</td>"
                        Next
                        stmov &= "</tr>"
                    Next
                    stmov &= "</table></div>"
                    body = body.Replace("[Smovdtl]", stmov)
                End If
                body = body.Replace("[Smovdtl]", "")
                body = body.Replace("[FutureMovementsDetails]", "Future Movement Details")
            End If

            body = body.Replace("[DocumentType]", "")
            body = body.Replace("[ChildDoctype]", "")
            body = body.Replace("[MovementsDetails]", "")
            body = body.Replace("[FutureMovementsDetails]", "")
            body = body.Replace("[DocID]", "System Doc ID - " & DocID)
            WritePdf(body, DocID)
        Catch ex As Exception
        Finally

            con.Close()
            con.Dispose()
            da.Dispose()
            ds.Dispose()
        End Try
    End Sub
    Private Sub WritePdf(ByVal mBody As String, ByVal filename As String)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            ds.Clear()
            Dim filenamestr As String = ""
        da.SelectCommand.CommandText = "select distinct dropdowntype,fieldmapping,dropdown from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and displayname='" & ddlFieldName.SelectedItem.Text & "' and eid=" & Session("EID") & ""
        da.SelectCommand.CommandTimeout = 600
        da.Fill(ds, "formtype")
            Dim formtype As String = ds.Tables("formtype").Rows(0).Item("dropdowntype").ToString
            Dim fieldmapping As String = ds.Tables("formtype").Rows(0).Item("fieldmapping").ToString
        count = count + 1
        Try

            If formtype.ToUpper = "MASTER VALUED" Then
                da.SelectCommand.CommandText = "select dms.udf_split('" & ds.Tables("formtype").Rows(0).Item("dropdown").ToString & "'," & fieldmapping & ")[Name] from mmm_mst_doc with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and tid=" & filename & " and eid=" & Session("EID") & ""
            Else
                da.SelectCommand.CommandText = "select  " & fieldmapping & "[Name] from mmm_mst_doc with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString() & "' and tid=" & filename & " and eid=" & Session("EID") & ""
            End If
            da.SelectCommand.CommandTimeout = 600
            da.Fill(ds, "filename")
            If ds.Tables("filename").Rows.Count = 1 Then
                filenamestr = txtFileName.Text & ds.Tables("filename").Rows(0).Item("Name").ToString & "_" & filename & "_" & Now.Millisecond & count
            Else
                If txtFileName.Text <> "" Then
                    filenamestr = txtFileName.Text & ds.Tables("filename").Rows(0).Item("Name").ToString & "_" & filename & "_" & Now.Millisecond & count
                Else
                    filenamestr = filename & "_" & Now.Millisecond & count
                End If
            End If

            Dim _strRepeater As New StringBuilder(mBody)
            Dim _ObjHtm As New Html32TextWriter(New System.IO.StringWriter(_strRepeater))
            Dim _str As String = _strRepeater.ToString()
            Dim document As New iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 15, 15, 15, 15)
            PdfWriter.GetInstance(document, New FileStream(HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & filenamestr & ".pdf"), FileMode.Create))
            document.Open()
            Dim htmlarraylist As List(Of IElement) = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(New StringReader(_str), Nothing)
            For k As Integer = 0 To htmlarraylist.Count - 1
                document.Add(DirectCast(htmlarraylist(k), IElement))
            Next
            document.Close()
        Catch generatedExceptionName As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
    End Sub
    Protected Sub AddFolderHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim count As Integer = 0
        For Each gvrow As GridViewRow In gvPending.Rows
            Dim chk As CheckBox = DirectCast(gvrow.FindControl("ChkDocid"), CheckBox)
            If chk.Checked = True Then
                count = count + 1
            End If
        Next
        If count < 1 Then
            lblmsg1.Text = "Please Select Document's for Download"
            Exit Sub
        End If
        If count > 10 Then
            lblmsg1.Text = "You can't select more than 10 documnet's"
            Exit Sub
        End If
        BindFieldDDL()
        lblMsgAddFolder.Text = ""
        txtFolderName.Text = ""
        Me.btnAddFolder_ModalPopupExtender.Show()
        updatePanelAddFolder.Update()
    End Sub

    Public Sub BindFieldDDL()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim filename As String = ""
        Try
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select fieldmapping,displayname from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString & "' and datatype='Text' and  eid='" & Session("EID") & "' and fieldtype in ('text box','drop down','Lookup')"
            da.Fill(ds, "fieldtype")
            ddlFieldName.DataSource = ds.Tables("fieldtype")
            ddlFieldName.DataTextField = "displayname"
            ddlFieldName.DataValueField = "fieldmapping"
            ddlFieldName.DataBind()
            ddlFieldName.Items.Insert(0, "--Please Select--")
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            ds.Dispose()
        End Try
    End Sub
    Protected Sub btnDownload_Click(sender As Object, e As System.EventArgs) Handles btnDownload.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select emailid,username from MMM_mst_user with(nolock) where uid=" & Session("UID") & " and eid='" & Session("EID") & "' ", con)
        Dim ds As New DataSet
        da.Fill(ds, "email")
        da.SelectCommand.CommandTimeout = 300
        Dim Mailto As String = ds.Tables("email").Rows(0).Item("emailid").ToString
        Dim Username As String = ds.Tables("email").Rows(0).Item("username").ToString
        If Len(txtFolderName.Text) >= 1 Then

            If Directory.Exists(HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text)) Then
                lblMsgAddFolder.BackColor = Drawing.Color.Crimson
                lblMsgAddFolder.Text = "Folder Name Already Exist"
                Exit Sub
            End If

            If Directory.Exists(HostingEnvironment.MapPath("~/Templates/")) Then
                Dim datestr As String = Now.Date & Now.Month & Now.Year & Now.Millisecond
                Directory.CreateDirectory(HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text))
            End If
            Dim obj As New MailUtill(eid:=Session("EID"))

            If chkPDF.Checked = True And chkAttachment.Checked = True And chkCoverSheet.Checked = True Then
                ChkDocid()
                CopyAttachments()
                CopyCoverSheet()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"
            ElseIf chkPDF.Checked = True And chkAttachment.Checked = True Then
                ChkDocid()
                CopyAttachments()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"
            ElseIf chkPDF.Checked = True And chkCoverSheet.Checked = True Then
                ChkDocid()
                CopyCoverSheet()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"
            ElseIf chkAttachment.Checked = True And chkCoverSheet.Checked = True Then
                CopyAttachments()
                CopyCoverSheet()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"
            ElseIf chkPDF.Checked = True Then
                ChkDocid()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"

            ElseIf chkAttachment.Checked = True Then
                CopyAttachments()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"
            ElseIf chkCoverSheet.Checked = True Then
                CopyCoverSheet()
                Zip()
                obj.SendMail(ToMail:=Mailto, Subject:="Backup of Documents", MailBody:="Dear " & Username & "<br/><br/>  Please Find attached backup of documents.<br/><br/>Regards,<br/> IT Support Team", Attachments:=HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text & "/" & txtFolderName.Text & ".zip"))
                lblMsgAddFolder.BackColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.BorderColor = Drawing.Color.DarkGreen
                lblMsgAddFolder.Text = "Files Sent successfully"
            Else
                lblMsgAddFolder.BackColor = Drawing.Color.Crimson
                lblMsgAddFolder.Text = "Please select file types for download"
            End If
        Else
            lblMsgAddFolder.BackColor = Drawing.Color.Crimson
            lblMsgAddFolder.Text = "Please enter valid folder name"
            btnAddFolder_ModalPopupExtender.Show()
        End If
        con.Close()
        con.Dispose()
        ds.Dispose()

    End Sub


End Class
