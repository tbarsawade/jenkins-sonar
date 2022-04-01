Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Partial Class MobileAssureEnhancement
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("doctype") Is Nothing Then
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter(" Select Distinct [Audit cycle] as Auditcycle from  [DMS].V199asPerFAR_MobileASSURE where [Audit cycle] <>'' order by  [Audit cycle]  ", con)
            Try
                Dim ds As New DataSet
                da.Fill(ds, "data")
                ddlDocType.DataSource = ds
                ddlDocType.DataTextField = "Auditcycle"
                ddlDocType.DataValueField = "Auditcycle"
                ddlDocType.DataBind()
                ddlDocType.Items.Insert(0, "--Please Select--")
            Catch ex As Exception
                Console.Write("Error info:" & ex.Message)
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

    'Protected Sub ddlDocType_TextChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.TextChanged
    '    Session("doctype") = ddlDocType.SelectedValue.ToString()
    '    'txtSearch.Text = ""
    '    'BindGrid()
    'End Sub

    Public Sub BindGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim tbtype As String = ""
        Dim table As String = ""
        Dim doc As String = ""
        Dim ob As New DynamicForm
        oda.SelectCommand.CommandType = CommandType.Text

        Try
            'oda.SelectCommand.CommandText = "select formtype from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & ddlDocType.SelectedValue.ToString & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.CommandText = "Usp_MobileAutoReconciliation"
            oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
            If ddlDocType.SelectedValue.ToString() = "--Please Select--" Then
                oda.SelectCommand.Parameters.AddWithValue("@documentType", "")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@documentType", ddlDocType.SelectedValue.ToString())
            End If
            oda.SelectCommand.Parameters.AddWithValue("@val", txtSearch.Text)
            oda.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))

            'If IsNothing(Session("SUBUID")) Then
            '    oda.SelectCommand.Parameters.AddWithValue("@suid", "0")
            'Else
            '    oda.SelectCommand.Parameters.AddWithValue("@suid", Session("SUBUID"))
            'End If
            'If doc = "" Then
            '    oda.SelectCommand.Parameters.AddWithValue("@docid", "0")
            'Else
            '    oda.SelectCommand.Parameters.AddWithValue("@docid", doc.ToString)
            'End If
            'oda.SelectCommand.Parameters.AddWithValue("@urole", Session("USERROLE"))

            Dim ds As New DataSet
            oda.SelectCommand.CommandTimeout = 900
            oda.Fill(ds, "pending")
            ViewState("pending") = ds.Tables("pending")



            If ds.Tables("pending").Rows.Count > 0 Then
                gvPending.DataSource = ds.Tables("pending")
                gvPending.DataBind()
                lblmsg.Text = ""
                gvPending.Visible = True
                ViewState("xlexport") = ds.Tables("pending")
            Else
                gvPending.Controls.Clear()
                lblmsg.Text = "Data not found.."
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
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        BindGrid()
    End Sub
    Protected Sub gvPending_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPending.PageIndexChanging
        Try
            gvPending.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1

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

    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Dim GVReport As New GridView
        GVReport.AllowPaging = False
        GVReport.DataSource = ViewState("xlexport")
        GVReport.DataBind()
        Response.Clear()
        Response.Buffer = True
        If ddlDocType.SelectedItem.Text = "--Please Select--" Then
            Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>All Audit Cycle</h3></div> <br/>")
            Response.AddHeader("content-disposition", "attachment;filename=All Audit Cycle.xls")
        Else
            Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & ddlDocType.SelectedItem.Text & "</h3></div> <br/>")
            Response.AddHeader("content-disposition", "attachment;filename=" & ddlDocType.SelectedItem.Text & ".xls")
        End If
        'Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & ddlDocType.SelectedItem.Text & "</h3></div> <br/>")
        'Response.AddHeader("content-disposition", "attachment;filename=" & ddlDocType.SelectedItem.Text & ".xls")
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



End Class
