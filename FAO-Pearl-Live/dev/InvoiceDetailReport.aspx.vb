Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Partial Class InvoiceDetailReport
    Inherits System.Web.UI.Page
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pnl1.Visible = False
            txtval.Visible = False
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Try
                'Dim doc As String = ""
                'da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & ""
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                Dim ds As New DataSet
                '' 
                Session("Report") = ""
                da.SelectCommand.CommandText = "select Reportid,ReportName from mmm_mst_report where eid=" & Session("EID") & " and reportid in (1695,1696,1697) order by reportid "
                da.Fill(ds, "data")
                ddlDocType.DataSource = ds
                ddlDocType.DataTextField = "ReportName"
                ddlDocType.DataValueField = "Reportid"
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
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = ""
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataSet
            oda.Fill(dt)
            ' gvPending.DataSource = dt
            ' gvPending.DataBind()
            ViewState("grd") = dt
        Catch ex As Exception
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    'Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexport.Click
    '    Response.ClearContent()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataTable
    '    Dim gridview1 As New GridView
    '    gridview1.DataSource = ViewState("grd")
    '    gridview1.DataBind()
    '    Response.ContentType = "application/vnd.ms-excel"
    '    Response.Clear()
    '    Response.Buffer = True
    '    Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & ddlDocType.SelectedItem.Text.ToString & "</h3></div> <br/>")
    '    Response.AddHeader("content-disposition", "attachment;filename=" & ddlDocType.SelectedItem.Text.ToString & ".xls")
    '    Response.Charset = ""
    '    Response.ContentType = "application/vnd.ms-excel"
    '    Dim sw As New StringWriter()
    '    Dim hw As New HtmlTextWriter(sw)
    '    'For i = 0 To gridview1.Rows.Count - 1
    '    '    gridview1.Rows(i).Attributes.Add("class", "textmode")
    '    'Next
    '    Dim datatb As DataTable = ViewState("Child")
    '    Dim datatype As String
    '    Dim chdtype As String = ""
    '    If IsNothing(datatb) = False Then
    '        If datatb.Rows.Count > 0 Then
    '            chdtype = datatb.Rows(0).Item("Documenttype").ToString
    '        End If
    '    End If
    '    For i = 1 To gridview1.HeaderRow.Cells.Count - 1
    '        da.SelectCommand.CommandText = "Select datatype from mmm_mst_fields where eid=" & Session("EID") & " and displayname='" & gridview1.HeaderRow.Cells(i).Text.ToString() & "' and documenttype in ('" & ddlDocType.SelectedValue.ToString & "','" & chdtype & "')"
    '        ds.Clear()
    '        da.Fill(ds)
    '        If ds.Rows.Count > 0 Then
    '            datatype = ds.Rows(0).Item(0).ToString
    '            If datatype.ToUpper.ToString = "TEXT" Or datatype.ToUpper.ToString = "DATETIME" Then
    '                For j = 0 To gridview1.Rows.Count - 1
    '                    gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
    '                Next
    '            End If
    '        ElseIf gridview1.HeaderRow.Cells(i).Text.ToUpper = "CREATION DATE" Then
    '            For j = 0 To gridview1.Rows.Count - 1
    '                gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
    '            Next
    '        End If
    '    Next

    '    'For i = 0 To gridview1.HeaderRow.Cells.Count - 1
    '    '    If gridview1.HeaderRow.Cells(i).Text.ToUpper.Contains("DATE").ToString Then
    '    '        For j = 0 To gridview1.Rows.Count - 1
    '    '            gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
    '    '        Next
    '    '    End If
    '    '    If gridview1.HeaderRow.Cells(i).Text.ToUpper.Contains("PERIOD").ToString Then
    '    '        For j = 0 To gridview1.Rows.Count - 1
    '    '            gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
    '    '        Next
    '    '    End If
    '    'Next
    '    gridview1.RenderControl(hw)
    '    'style to format numbers to string 
    '    Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
    '    Response.Write(style)
    '    Response.Output.Write(sw.ToString())
    '    Response.Flush()
    '    Response.End()
    'End Sub
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
        'Select Case e.CommandName
        '    Case "Previous"
        '        currentPageNumberp = Int32.Parse(ViewState("cpnp")) - 1
        '        If gvPending.PageIndex > 0 Then
        '            gvPending.PageIndex = gvPending.PageIndex - 1
        '        End If
        '        Exit Select
        '    Case "Next"
        '        currentPageNumberp = Int32.Parse(ViewState("cpnp")) + 1
        '        gvPending.PageIndex = gvPending.PageIndex + 1
        '        Exit Select
        'End Select
    End Sub
    Private Sub SortGridView(ByVal sortExpression As String, ByVal direction As String)
        'You can cache the DataTable for improving performance
        Dim dt As DataTable = CType(ViewState("pending"), DataTable)
        dt.DefaultView.Sort = sortExpression + direction
        Dim dt1 As DataTable = dt.DefaultView.ToTable()
        '   gvPending.DataSource = ViewState("pending")
        '  gvPending.DataBind()
        'BindGrid()
        'AdvSearch()
        'ViewState("data") = dt1
    End Sub
    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select distinct displayname[DispText],displayname[DispVal],DocDtlDisplayOrder from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlDocType.SelectedValue.ToString & "' and ((isSearch=1 and isactive=1) or fieldtype in ('New Auto number','Auto number') ) and fieldtype<>'Child item' Union select 'curstatus','Current Status', 0 as DocDtlDisplayOrder union select 'adate','Creation Date', 0 as DocDtlDisplayOrder union select 'Invoice Number','Invoice Number',0 as DocDtlDisplayOrder union select 'Invoice Date','Invoice Date', 0 as DocDtlDisplayOrder union select 'Vendor Name','Vendor Name',0 as DocDtlDisplayOrder union select 'Document Type','Document Type',0 as DocDtlDisplayOrder union select 'Sub Department','Sub Department',0 as DocDtlDisplayOrder union select 'Department','Department',0 as DocDtlDisplayOrder union select 'Bar Code','Bar Code',0 as DocDtlDisplayOrder union select 'Invoice Receipt Date','Invoice Receipt Date',0 as DocDtlDisplayOrder union select 'Currency','Currency',0 as DocDtlDisplayOrder ", con)
        ''''union select 'Invoice Number','Invoice Number',0 as DocDtlDisplayOrder union select 'Invoice Date','Invoice Date', 0 as DocDtlDisplayOrder union select 'Vendor Name','Vendor Name',0 as DocDtlDisplayOrder 
        Try
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlfields.DataSource = ds
            ddlfields.DataTextField = "DispVal"
            ddlfields.DataValueField = "DispText"
            ddlfields.DataBind()
            ddlfields.Items.Insert("0", "-Select-")
            txtval.Text = ""
            txtsdate.Text = ""
            txtedate.Text = ""
            txtval.Visible = False
            pnl1.Visible = False
            '   gvPending.Controls.Clear()
            Session("Report") = ddlDocType.SelectedValue.ToString
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
    End Sub
    Protected Sub ddlfields_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlfields.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select datatype,fieldmapping,FieldID from mmm_mst_fields where eid='" & Session("EID") & "' and (documenttype='" & ddlDocType.SelectedValue.ToString & "' and displayname='" & ddlfields.SelectedValue.ToString & "') or (documenttype in ( select dropdown from mmm_mst_fields where documenttype='" & ddlDocType.SelectedValue.ToString & "' and fieldtype='child item' and eid=" & Session("EID") & ") and displayname='" & ddlfields.SelectedValue.ToString & "' and eid=" & Session("EID") & ")  ", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Try
            txtval.Text = ""
            txtsdate.Text = ""
            txtedate.Text = ""
            '   gvPending.Controls.Clear()
            If ddlfields.SelectedValue.ToString = "-Select-" Then
                txtval.Visible = False
                pnl1.Visible = False
            End If
            If ddlfields.SelectedValue.ToString = "adate" Then
                pnl1.Visible = True
                txtval.Visible = False
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtsdate.Attributes.Add("IsSearch", "1")
                txtedate.Attributes.Add("IsSearch", "1")
                txtsdate.Attributes.Add("data-ty", "DATETIME")
                txtedate.Attributes.Add("data-ty", "DATETIME")
                txtsdate.Attributes.Add("fld", "frdate")
                txtedate.Attributes.Add("fld", "todate")
            ElseIf ddlfields.SelectedValue.ToString = "Invoice Date" Then
                pnl1.Visible = True
                txtval.Visible = False
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtsdate.Attributes.Add("IsSearch", "1")
                txtedate.Attributes.Add("IsSearch", "1")
                txtsdate.Attributes.Add("data-ty", "DATETIME")
                txtedate.Attributes.Add("data-ty", "DATETIME")
                txtsdate.Attributes.Add("fld", "frdate")
                txtedate.Attributes.Add("fld", "todate")
            ElseIf ddlfields.SelectedValue.ToString = "curstatus" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
                ' txtval.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Vendor Name" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Document Type" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Sub Department" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Department" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
                'ElseIf ddlfields.SelectedValue.ToString = "tid" Then
                '    ddlDocType.Attributes.Add("IsSearch", "1")
                '    ddlDocType.Attributes.Add("data-ty", "DDL")
                '    ddlDocType.Attributes.Add("fld", "ddlDocType")
                '    ddlfields.Attributes.Add("IsSearch", "1")
                '    ddlfields.Attributes.Add("data-ty", "DDL")
                '    ddlfields.Attributes.Add("fld", "ddlfields")

                '    txtval.Attributes.Add("data-ty", "NUMERIC")
                '    txtval.Attributes.Add("IsSearch", "1")
                '    txtval.Attributes.Add("fld", "txtval")
                '    txtval.Visible = True
                '    pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Invoice Number" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Bar Code" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Currency" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "Invoice Receipt Date" Then
                pnl1.Visible = True
                txtval.Visible = False
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtsdate.Attributes.Add("IsSearch", "1")
                txtedate.Attributes.Add("IsSearch", "1")
                txtsdate.Attributes.Add("data-ty", "DATETIME")
                txtedate.Attributes.Add("data-ty", "DATETIME")
                txtsdate.Attributes.Add("fld", "frdate")
                txtedate.Attributes.Add("fld", "todate")
            End If

            Session("DDlField") = ddlfields.SelectedItem.Value.ToString()

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
    End Sub
    Public Shared Function GetAllFields(EID As Integer) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            '            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & " and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number');"
            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & " and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number') order by f.DocDtlDisplayOrder;"
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Protected Shared Function GenearateQuery(data As Dictionary(Of String, Object), IsActionForm As Boolean) As DGrid
        Dim ret As String = ""
        Dim grid As New DGrid()
        Dim ds As New DataSet()
        Dim dt As New DataTable

        'Dim str As String = ""
        ' Geiing all the field of Entity becouse all the field might be required
        Dim eid As Integer = HttpContext.Current.Session("eid")
        ' ds = GetAllFields(eid)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & eid & "  and Reportid=" & HttpContext.Current.Session("Report") & " "
        Else
            da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & eid & "  and Reportid=" & HttpContext.Current.Session("Report") & " "
        End If

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
        Dim Dept As String = ""
        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & eid & "  and UID=" & HttpContext.Current.Session("UID") & " "
            Dept = da.SelectCommand.ExecuteScalar().ToString
            If Dept = "" Then
                Exit Function
            End If
        End If

        If HttpContext.Current.Session("DDlField") = "adate" Then
            If data("frdate").ToString() <> "" Then
                str = Replace(str, "@filter1", "convert(date,adate)>='" & data("frdate").ToString() & "' and convert(date,adate)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter2", "convert(date,adate)>='" & data("frdate").ToString() & "' and convert(date,adate)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter3", "convert(date,adate)>='" & data("frdate").ToString() & "' and convert(date,adate)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter4", "convert(date,adate)>='" & data("frdate").ToString() & "' and convert(date,adate)<='" & data("todate").ToString() & "'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "curstatus" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " curstatus like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " curstatus like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " curstatus like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " curstatus like '%" & data("txtval").ToString() & "%'")
            End If
        End If

        'If HttpContext.Current.Session("DDlField") = "tid" Then
        '    If data("txtval").ToString() <> "" Then
        '        str = Replace(str, "@filter1", " curstatus like '%" & data("txtval").ToString() & "%'")
        '        str = Replace(str, "@filter2", " curstatus like '%" & data("txtval").ToString() & "%'")
        '        str = Replace(str, "@filter3", " curstatus like '%" & data("txtval").ToString() & "%'")
        '    End If
        'End If
        If HttpContext.Current.Session("DDlField") = "Invoice Number" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " fld17 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " fld13 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " fld3 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " fld23 like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Invoice Date" Then
            If data("frdate").ToString() <> "" Then
                str = Replace(str, "@filter1", "convert(varchar(50),convert(date,fld18,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld18,3),120)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter2", "convert(varchar(50),convert(date,fld14,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld14,3),120)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter3", "convert(varchar(50),convert(date,fld4,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld4,3),120)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter4", "convert(varchar(50),convert(date,fld24,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld24,3),120)<='" & data("todate").ToString() & "'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Vendor Name" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " dms.udf_split('MASTER-Vendor Master-fld2',fld5) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " dms.udf_split('MASTER-Vendor Master-fld2',fld5) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " dms.udf_split('MASTER-Vendor Master-fld2',fld5) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " fld3 like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Document Type" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " Documenttype like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " Documenttype like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " Documenttype like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " Documenttype like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Sub Department" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " dms.udf_split('MASTER-Sub Department-fld1',fld79) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " dms.udf_split('MASTER-Sub Department-fld1',fld2) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " dms.udf_split('MASTER-Sub Department-fld1',fld79) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " dms.udf_split('MASTER-Sub Department-fld1',fld20) like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Department" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " dms.udf_split('MASTER-Department Master-fld2',fld65) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " dms.udf_split('MASTER-Department Master-fld2',fld3) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " dms.udf_split('MASTER-Department Master-fld2',fld65) like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " dms.udf_split('MASTER-Department Master-fld2',fld19) like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Currency" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " fld64 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " fld12 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " fld20 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " fld88 like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Bar Code" Then
            If data("txtval").ToString() <> "" Then
                str = Replace(str, "@filter1", " fld72 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter2", " fld61 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter3", " fld1 like '%" & data("txtval").ToString() & "%'")
                str = Replace(str, "@filter4", " fld32 like '%" & data("txtval").ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Invoice Receipt Date" Then
            If data("frdate").ToString() <> "" Then
                str = Replace(str, "@filter1", "convert(varchar(50),convert(date,fld85,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld85,3),120)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter2", "convert(varchar(50),convert(date,fld87,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld87,3),120)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter3", "convert(varchar(50),convert(date,fld2,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld2,3),120)<='" & data("todate").ToString() & "'")
                str = Replace(str, "@filter4", "convert(varchar(50),convert(date,fld22,3),120)>='" & data("frdate").ToString() & "' and convert(varchar(50),convert(date,fld22,3),120)<='" & data("todate").ToString() & "'")
            End If
        End If
        str = Replace(str, "@filter1", " tid=tid")
        str = Replace(str, "@filter2", " tid=tid")
        str = Replace(str, "@filter3", " tid=tid")
        str = Replace(str, "@filter4", " tid=tid")

        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            str = Replace(str, "@rolePO", " fld79 in (" & Dept & ",'',0)")
            str = Replace(str, "@roleNonPO", " fld2 in (" & Dept & ",'',0)")
            str = Replace(str, "@roleVInvoice", " fld20 in (" & Dept & ",'',0)")
        End If

        da.SelectCommand.CommandText = str
        da.SelectCommand.CommandTimeout = 1200
        da.SelectCommand.ExecuteNonQuery()
        ''da.SelectCommand.ExecuteNonQuery()
        ds.Clear()
        da.Fill(dt)
        Dim DocumentType As String = ""
        Dim strError As String = ""
        If dt.Rows.Count > 0 Then
            grid = DynamicGrid.GridData(dt, strError)
        Else
            strError = "No data found"
            grid.Column.Clear()
        End If

        Return grid
    End Function

    'Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs) Handles btnSearch.Click
    '    If ddlDocType.SelectedValue.ToString <> "0" Then
    '        GenearateQuery(Session("EID"), ddlDocType.SelectedValue.ToString, False)
    '    End If
    'End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
    End Sub
    Public Function GenearateQueryExl(EID As Integer) As DataTable
        Dim ret As String = ""
        Dim grid As New DGrid()
        Dim ds As New DataSet()
        Dim dt As New DataTable
        'Dim str As String = ""
        ' Geiing all the field of Entity becouse all the field might be required
        'Dim eid As Integer = HttpContext.Current.Session("eid")
        ' ds = GetAllFields(eid)
        ' ds = GetAllFields(eid)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & EID & "  and Reportid=" & HttpContext.Current.Session("Report") & " "
        Else
            da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & EID & "  and Reportid=" & HttpContext.Current.Session("Report") & " "
        End If

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
        Dim Dept As String = ""
        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & EID & "  and UID=" & HttpContext.Current.Session("UID") & " "
            Dept = da.SelectCommand.ExecuteScalar().ToString
            If Dept = "" Then
                Exit Function
            End If
        End If

        If HttpContext.Current.Session("DDlField") = "adate" Then
            If txtsdate.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", "convert(date,adate)>='" & txtsdate.Text.ToString() & "' and convert(date,adate)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter2", "convert(date,adate)>='" & txtsdate.Text.ToString() & "' and convert(date,adate)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter3", "convert(date,adate)>='" & txtsdate.Text.ToString() & "' and convert(date,adate)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter4", "convert(date,adate)>='" & txtsdate.Text.ToString() & "' and convert(date,adate)<='" & txtedate.Text.ToString() & "'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "curstatus" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " curstatus like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " curstatus like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " curstatus like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " curstatus like '%" & txtval.Text.ToString() & "%'")
            End If
        End If

        'If HttpContext.Current.Session("DDlField") = "tid" Then
        '    If data("txtval").ToString() <> "" Then
        '        str = Replace(str, "@filter1", " curstatus like '%" & data("txtval").ToString() & "%'")
        '        str = Replace(str, "@filter2", " curstatus like '%" & data("txtval").ToString() & "%'")
        '        str = Replace(str, "@filter3", " curstatus like '%" & data("txtval").ToString() & "%'")
        '    End If
        'End If
        If HttpContext.Current.Session("DDlField") = "Invoice Number" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " fld17 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " fld13 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " fld3 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " fld23 like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Invoice Date" Then
            If txtsdate.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", "convert(varchar(50),convert(date,fld18,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld18,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter2", "convert(varchar(50),convert(date,fld14,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld14,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter3", "convert(varchar(50),convert(date,fld4,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld4,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter4", "convert(varchar(50),convert(date,fld24,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld24,3),120)<='" & txtedate.Text.ToString() & "'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Vendor Name" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " dms.udf_split('MASTER-Vendor Master-fld2',fld5) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " dms.udf_split('MASTER-Vendor Master-fld2',fld5) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " dms.udf_split('MASTER-Vendor Master-fld2',fld5) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " fld3 like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Document Type" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " Documenttype like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " Documenttype like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " Documenttype like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " Documenttype like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Sub Department" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " dms.udf_split('MASTER-Sub Department-fld1',fld79) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " dms.udf_split('MASTER-Sub Department-fld1',fld2) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " dms.udf_split('MASTER-Sub Department-fld1',fld79) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " dms.udf_split('MASTER-Sub Department-fld1',fld20) like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Department" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " dms.udf_split('MASTER-Department Master-fld2',fld65) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " dms.udf_split('MASTER-Department Master-fld2',fld3) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " dms.udf_split('MASTER-Department Master-fld2',fld65) like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " dms.udf_split('MASTER-Department Master-fld2',fld19) like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Currency" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " fld64 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " fld12 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " fld20 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " fld88 like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Bar Code" Then
            If txtval.Text.ToString() <> "" Then
                str = Replace(str, "@filter1", " fld72 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter2", " fld61 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter3", " fld1 like '%" & txtval.Text.ToString() & "%'")
                str = Replace(str, "@filter4", " fld32 like '%" & txtval.Text.ToString() & "%'")
            End If
        End If
        If HttpContext.Current.Session("DDlField") = "Invoice Receipt Date" Then
            If txtsdate.Text <> "" Then
                str = Replace(str, "@filter1", "convert(varchar(50),convert(date,fld85,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld85,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter2", "convert(varchar(50),convert(date,fld87,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld87,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter3", "convert(varchar(50),convert(date,fld2,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld2,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@filter4", "convert(varchar(50),convert(date,fld22,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld22,3),120)<='" & txtedate.Text.ToString() & "'")
            End If
        End If
        str = Replace(str, "@filter1", " tid=tid")
        str = Replace(str, "@filter2", " tid=tid")
        str = Replace(str, "@filter3", " tid=tid")
        str = Replace(str, "@filter4", " tid=tid")

        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            str = Replace(str, "@rolePO", " fld79 in (" & Dept & ",'',0)")
            str = Replace(str, "@roleNonPO", " fld2 in (" & Dept & ",'',0)")
            str = Replace(str, "@roleVInvoice", " fld20 in (" & Dept & ",'',0)")
        End If

        da.SelectCommand.CommandText = str
        da.SelectCommand.CommandTimeout = 1200
        da.SelectCommand.ExecuteNonQuery()
        ''da.SelectCommand.ExecuteNonQuery()
        ds.Clear()
        da.Fill(dt)
        Return dt
    End Function
    Protected Sub btnViewInExcel_Click(sender As Object, e As EventArgs) Handles btnViewInExcel.Click
        Response.ClearContent()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataTable
        Dim ds1 As New DataSet
        Dim dt As New DataTable
        Dim gridview1 As New GridView
        ' Dim DocumentType As String
        ds1 = GetAllFields(Session("EID"))
        'DocumentType = ds1.Tables(0).Rows(0).Item("EventName").ToString
        'gridview1.DataSource = ViewState("grd")
        dt = GenearateQueryExl(Session("EID"))
        gridview1.DataSource = dt
        gridview1.DataBind()
        Response.ContentType = "application/vnd.ms-excel"
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & ddlDocType.SelectedItem.Text.ToString & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & ddlDocType.SelectedItem.Text.ToString & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        'For i = 0 To gridview1.Rows.Count - 1
        '    gridview1.Rows(i).Attributes.Add("class", "textmode")
        'Next
        Dim datatb As DataTable = ViewState("Child")
        'Dim datatype As String
        Dim chdtype As String = ""
        If IsNothing(datatb) = False Then
            If datatb.Rows.Count > 0 Then
                chdtype = datatb.Rows(0).Item("Documenttype").ToString
            End If
        End If
        'For i = 1 To gridview1.HeaderRow.Cells.Count - 1
        '    da.SelectCommand.CommandText = "Select datatype from mmm_mst_fields where eid=" & Session("EID") & " and displayname='" & gridview1.HeaderRow.Cells(i).Text.ToString() & "' and documenttype in ('" & ddlDocType.SelectedValue.ToString & "','" & chdtype & "')"
        '    ds.Clear()
        '    da.Fill(ds)
        'If ds.Rows.Count > 0 Then
        '    datatype = ds.Rows(0).Item(0).ToString
        '    If datatype.ToUpper.ToString = "TEXT" Or datatype.ToUpper.ToString = "DATETIME" Then
        '        For j = 0 To gridview1.Rows.Count - 1
        '            gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
        '        Next
        '    End If
        'ElseIf gridview1.HeaderRow.Cells(i).Text.ToUpper = "CREATION DATE" Then
        '    For j = 0 To gridview1.Rows.Count - 1
        '        gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
        '    Next
        'End If
        ' Next

        'For i = 0 To gridview1.HeaderRow.Cells.Count - 1
        '    If gridview1.HeaderRow.Cells(i).Text.ToUpper.Contains("DATE").ToString Then
        '        For j = 0 To gridview1.Rows.Count - 1
        '            gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
        '        Next
        '    End If
        '    If gridview1.HeaderRow.Cells(i).Text.ToUpper.Contains("PERIOD").ToString Then
        '        For j = 0 To gridview1.Rows.Count - 1
        '            gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
        '        Next
        '    End If
        'Next
        gridview1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetData(json As Dictionary(Of String, Object)) As DGrid
        Dim d As New DGrid()
        Try
            If json("ddlDocType").ToString <> "0" Then
                d = GenearateQuery(json, False)
            End If
        Catch ex As Exception
            ex.ToString()
        End Try
        Return d
    End Function
End Class
