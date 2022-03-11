Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Partial Class UniversalSearch
    Inherits System.Web.UI.Page
    'Add Theme Code

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrSRC").ConnectionString
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
            ddlUserList.Visible = False
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
                If Session("USERROLE") = "SU" Then
                    da.SelectCommand.CommandText = "select distinct formname,formcaption from mmm_mst_forms with(nolock) where eid='" & Session("EID") & "' and formtype='document'  and formsource='MENU DRIVEN' "
                    da.Fill(ds, "data")
                    ddlDocType.DataSource = ds
                    ddlDocType.DataTextField = "formcaption"
                    ddlDocType.DataValueField = "formname"
                    ddlDocType.DataBind()
                Else
                    da.SelectCommand.CommandText = "select menuname,replace(pagelink,'Documents.Aspx?SC=','')[DocName] from mmm_mst_menu with(nolock) where roles like '%{" & Session("USERROLE") & ":%' and eid=" & Session("EID") & " and pagelink like 'documents%'"
                    da.Fill(ds, "data")
                    ddlDocType.DataSource = ds
                    ddlDocType.DataTextField = "menuname"
                    ddlDocType.DataValueField = "DocName"
                    ddlDocType.DataBind()
                End If
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
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select dispText,DispVal,DocDtlDisplayOrder from (select distinct displayname[DispText],displayname[DispVal],DocDtlDisplayOrder from mmm_mst_fields with(nolock) where eid=" & Session("EID") & " and documenttype='" & ddlDocType.SelectedValue.ToString & "' and ((isSearch=1 and isactive=1) or fieldtype in ('New Auto number','Auto number') ) and fieldtype<>'Child item'  union select 'curstatus','Current Status', 0 as DocDtlDisplayOrder union select 'adate','Creation Date', 0 as DocDtlDisplayOrder union select 'userid','Current User', 0 as DocDtlDisplayOrder union select 'lastupdate','Last Action Date', 0 as DocDtlDisplayOrder union select 'tid','Document ID', 0 as DocDtlDisplayOrder union select distinct displayname[DispText],displayname[DispVal], DocDtlDisplayOrder from mmm_mst_fields with(nolock) where eid=" & Session("EID") & " and documenttype in ( select dropdown from mmm_mst_fields where documenttype='" & ddlDocType.SelectedValue.ToString & "' and fieldtype='child item' ) and isactive=1 and isSearch=1 ) as t order by DispVal", con)

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
            ddlUserList.Visible = False
            pnl1.Visible = False
            '   gvPending.Controls.Clear()
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

        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select datatype,fieldmapping,FieldID from mmm_mst_fields with(nolock) where eid='" & Session("EID") & "' and (documenttype='" & ddlDocType.SelectedValue.ToString & "' and displayname='" & ddlfields.SelectedValue.ToString & "') or (documenttype in ( select dropdown from mmm_mst_fields with(nolock) where documenttype='" & ddlDocType.SelectedValue.ToString & "' and fieldtype='child item' and eid=" & Session("EID") & ") and displayname='" & ddlfields.SelectedValue.ToString & "' and eid=" & Session("EID") & ")  ", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Try
            txtval.Text = ""
            txtsdate.Text = ""
            txtedate.Text = ""
            ddlUserList.Visible = False
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
            ElseIf ddlfields.SelectedValue.ToString = "userid" Then
                txtval.Visible = False
                ddlUserList.Visible = True
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")
                Dim ada As New SqlDataAdapter("select UserName,uid from MMM_Mst_User with(nolock) where isAuth=1 and eid=" & Session("EID") & " order by UserName asc", con)
                Dim dataset As New DataSet
                ada.Fill(dataset, "data")
                ddlUserList.DataSource = dataset
                ddlUserList.DataTextField = "UserName"
                ddlUserList.DataValueField = "uid"
                ddlUserList.DataBind()
                ddlUserList.Items.Insert("0", "-Select-")
                ddlUserList.Attributes.Add("isDocdtl", "1")
                pnl1.Visible = False

            ElseIf ddlfields.SelectedValue.ToString = "lastupdate" Then
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

            ElseIf ddlfields.SelectedValue.ToString = "tid" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            End If

            Dim datatype As String = ds.Tables("data").Rows(0).Item("datatype").ToString()
            If datatype = "Datetime" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")

                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtedate.Attributes.Add("IsSearch", "1")
                txtsdate.Attributes.Add("IsSearch", "1")
                txtsdate.Attributes.Add("data-ty", "DATETIME")
                txtedate.Attributes.Add("data-ty", "DATETIME")
                txtsdate.Attributes.Add("fld", "frdate")
                txtedate.Attributes.Add("fld", "todate")
                pnl1.Visible = True
                txtval.Visible = False
            Else
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")

                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("data-ty", "NUMERIC")
                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = True
                pnl1.Visible = False
            End If
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrSRC").ConnectionString
        Try
            '            Dim Query = "Set NOCOUNT On;Select F.FieldID, f.FieldType, f.FieldMapping, f.FieldID, f.DropDown, f.lookupvalue, f.DROPDOWNTYPE, f.DisplayName, f.DocumentType, FF.FormSource, FF.EventName, f.Datatype  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " And FF.EID= " & EID & " And  issearch=1 And (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number');"
                Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype  FROM MMM_MST_FIELDS F with(nolock) INNER JOIN MMM_MST_FORMS FF with(nolock) ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & " and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number') order by f.DocDtlDisplayOrder;"
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
        ' Geiing all the field of Entity becouse all the field might be required
        Dim eid As Integer = HttpContext.Current.Session("eid")
        ds = GetAllFields(eid)
        Dim BaseView As DataView
        Dim BaseTable As DataTable
        Dim DocumentType As String = ""
        If ds.Tables(0).Rows.Count > 0 Then
            BaseView = ds.Tables(0).DefaultView
            ' BaseView.RowFilter = "DocumentType='" & ds.Tables("flds").Rows & "'"

            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            BaseTable = BaseView.Table.DefaultView.ToTable()
            If IsActionForm = True Then
                DocumentType = ds.Tables(0).Rows(0).Item("EventName").ToString
                BaseView.RowFilter = "DocumentType='" & data("ddlDocType").ToString() & "'"
                BaseTable = BaseView.Table.DefaultView.ToTable()
            End If
            grid = GenearateQuery1(eid, data("ddlDocType").ToString(), ds, data)
            'Now find all object relation 
        End If

        Return grid
    End Function
    Public Shared Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, data As Dictionary(Of String, Object)) As DGrid
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim tbCh As DataTable
        Dim tbchitem As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim cDoc As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrSRC").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim srch As String = ""
        Dim ct As Integer = 0
        Dim ViewNamech = ""
        Dim fld As String = ""
        Dim grid As New DGrid()
        Dim strError As String = ""
        Dim SchemaString As String = DocumentType
        Try
            If ds.Tables(0).Rows.Count > 0 Then
                View = ds.Tables(0).DefaultView
                View.RowFilter = "DocumentType='" & DocumentType & "'"
                'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
                tbl = View.Table.DefaultView.ToTable()
                Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
                Dim ddlDocType = ""
                For i As Integer = 0 To tbl.Rows.Count - 1
                    If (tbl.Rows(i).Item("DROPDOWNTYPE").ToString.ToUpper = "SESSION VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                        StrColumn = StrColumn & "," & "(select username from mmm_mst_user with(nolock) where convert(varchar,uid)=" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "]) AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                    ElseIf tbl.Rows(i).Item("DROPDOWNTYPE").ToString.ToUpper = "MASTER VALUED" And tbl.Rows(i).Item("FieldType").ToString = "AutoComplete" Then
                        StrColumn = StrColumn & ",DMS.udf_split('" & tbl.Rows(i).Item("Dropdown").ToString & "'," & ViewName & ".  [" & tbl.Rows(i).Item("DisplayName") & "]) AS [" & tbl.Rows(i).Item("DisplayName") & "] "
                    ElseIf Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                        If tbl.Rows(i).Item("Fieldtype").ToString = "Child Item" Then
                            cDoc = tbl.Rows(i).Item("Dropdown").ToString
                            If cDoc <> "" Then
                                ViewNamech = "[V" & EID & cDoc.Replace(" ", "_") & "]"
                                View.RowFilter = "DocumentType='" & cDoc & "'"
                                tbCh = View.Table.DefaultView.ToTable()
                                'Change due to DOC detail confliction with Session("Child") name
                                HttpContext.Current.Session("SearchChild") = tbCh
                                For r As Integer = 0 To tbCh.Rows.Count - 1
                                    If Not (tbCh.Rows(r).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbCh.Rows(r).Item("FieldType") = "Drop Down") Then
                                        If tbCh.Rows(r).Item("Datatype").ToString.ToUpper = "DATETIME" Then
                                            StrColumn = StrColumn & "," & ViewNamech & ".[" & tbCh.Rows(r).Item("DisplayName") & "] AS [" & tbCh.Rows(r).Item("DisplayName") & "]"

                                        ElseIf tbCh.Rows(r).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbCh.Rows(r).Item("FieldType") = "AutoComplete" Then
                                            StrColumn = StrColumn & ",DMS.udf_split('" & tbCh.Rows(r).Item("Dropdown").ToString & "'," & ViewNamech & ".  [" & tbCh.Rows(r).Item("DisplayName") & "]) AS [" & tbCh.Rows(r).Item("DisplayName") & "] "

                                        Else
                                            StrColumn = StrColumn & "," & ViewNamech & ".[" & tbCh.Rows(r).Item("DisplayName") & "] AS [" & tbCh.Rows(r).Item("DisplayName") & "]"
                                        End If
                                    End If
                                Next
                                StrJoinString = StrJoinString & " left outer join " & ViewNamech & " with(nolock) on " & ViewNamech & ".docid = " & ViewName & ".TID"
                                View.RowFilter = "DocumentType='" & cDoc & "' AND FieldType ='DROP DOWN' AND DropDownType='MASTER VALUED'"
                                tbchitem = View.Table.DefaultView.ToTable()
                                For k As Integer = 0 To tbchitem.Rows.Count - 1
                                    Dim arrddl = tbchitem.Rows(k).Item("Dropdown").ToString().Split("-")
                                    ddlDocType = arrddl(1)
                                    SchemaString = SchemaString & "." & ddlDocType
                                    Dim ddlview = "[v" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
                                    Dim joincolumn = "tid"
                                    Dim DisPlayName = tbchitem.Rows(k).Item("DisplayName").ToString().Trim
                                    If ddlDocType.Trim.ToUpper = "USER" Then
                                        joincolumn = "UID"
                                        ddlview = "MMM_MST_USER with(nolock) "
                                    End If
                                    If StrJoinString.Contains(" left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = ") Then
                                    Else
                                        StrJoinString = StrJoinString & " left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = " & ViewNamech & ".[" & DisPlayName & "]"
                                    End If
                                Next
                                GenearateQuery3(EID, StrColumn, cDoc, ds)
                            End If
                            cDoc = ""
                            Continue For
                        ElseIf tbl.Rows(i).Item("Fieldtype").ToString = "LookupDDL" Then
                            StrColumn = StrColumn & ", dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "]" & ",'" & tbl.Rows(i).Item("FieldMapping").ToString &
                                "') AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                            Continue For
                        End If
                        If tbl.Rows(i).Item("Datatype").ToString.ToUpper = "DATETIME" Then
                            'StrColumn = StrColumn & ",case when isdate(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])=1 then convert(nvarchar,convert(date," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])) else " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] End AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                            StrColumn = StrColumn & ",case when isdate(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])=1 then convert(varchar,CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] as date),103) else " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] End AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                        Else
                            StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                        End If
                    End If
                Next

                If data("ddlfields") = "userid" Or data("ddlfields") = "lastupdate" Then
                    StrJoinString = StrJoinString & " left outer join [MMM_Doc_Dtl] with(nolock) on [MMM_Doc_Dtl].Tid = " & ViewName & ".LastTid"
                End If

                View.RowFilter = "DocumentType='" & DocumentType & "' AND (FieldType ='DROP DOWN' or FieldType ='AUTOCOMPLETE' ) AND DropDownType='MASTER VALUED'"
                tblRe = View.Table.DefaultView.ToTable()
                For j As Integer = 0 To tblRe.Rows.Count - 1
                    Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
                    ddlDocType = arrddl(1)
                    fld = Left(arrddl(1), 3)
                    SchemaString = SchemaString & "." & ddlDocType
                    Dim ddlview = "[v" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
                    Dim joincolumn = "tid"
                    Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
                    If ddlDocType.Trim.ToUpper = "USER" Then
                        joincolumn = "UID"
                        If fld.ToUpper = "FLD" Then
                            'comment for payu 
                            'Else
                            '    ddlview = " MMM_MST_USER "
                        End If
                    End If
                    'Remove join condition for User because there may be more than one user comming from mmm_mst_user 'Mayank Garg 20/12/2107'
                    If Not ddlDocType.Trim.ToUpper = "USER" Then
                        If StrJoinString.Contains(" left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = ") Then
                        Else
                            StrJoinString = StrJoinString & " left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = " & ViewName & ".[" & DisPlayName & "]"
                        End If
                    End If
                    'Remove join condition for User because there may be more than one user comming from mmm_mst_user 'Mayank Garg 20/12/2107'
                    If Not ddlDocType.Trim.ToUpper = "USER" Then
                        GenearateQuery2(HttpContext.Current.Session("EID"), StrColumn, StrJoinString, SchemaString, ddlDocType, ds, arrddl(2), displayName:=DisPlayName, MainDocumentType:=DocumentType)
                    Else
                        StrColumn = StrColumn & ",DMS.udf_split('" & tblRe.Rows(j).Item("Dropdown").ToString() & "'," & ViewName & ".[" & DisPlayName & "])" & " AS [" & DisPlayName & "]"
                    End If


                    If data("ddlDocType").ToString() = tblRe.Rows(j).Item("DisplayName").ToString().Trim Then
                        srch = GenearateQuery4(EID, ddlDocType, ds, arrddl(2))
                    End If
                Next


                Dim SLACol As String = ""
                Dim CUserandLSDADate As String = ""

                SLACol = SLAUseQuery(EID, DocumentType)
                CUserandLSDADate = CurrentUserandLastActionDate(EID, DocumentType)
                'Dim Query = "Select distinct  " & ViewName & ".[TID] As [DocID] , " & StrColumn.Substring(1, StrColumn.Length - 1) & ", " & ViewName & ".[CurStatus] As [Current Status], Convert(nvarchar, " & ViewName & ".[adate], 3) As [Creation Date]" & " FROM " & ViewName & " With(nolock) " & StrJoinString
                Dim Query = "Select  " & ViewName & ".[TID] As [DocID] ," & StrColumn.Substring(1, StrColumn.Length - 1) & "," & ViewName & ".[CurStatus] As [Current Status],convert(varchar,CAST(" & ViewName & ".[adate] as date),103) As [Creation Date]" & " " & SLACol & " " & CUserandLSDADate & "   FROM " & ViewName & " With(nolock) " & StrJoinString

                da.SelectCommand.CommandText = "Select documenttype,dropdown,dropdowntype,datatype ,fieldtype ,FieldMapping,DisplayName from mmm_mst_fields with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and (documenttype='" & DocumentType.ToString & "' and displayname='" & data("ddlfields").ToString() & "') or (documenttype in ( select dropdown from mmm_mst_fields with(nolock) where documenttype='" & DocumentType.ToString & "' and fieldtype='child item' and isSearch=1 and eid=" & HttpContext.Current.Session("EID") & ") and displayname='" & data("ddlfields").ToString() & "' and eid=" & HttpContext.Current.Session("EID") & ") order by fieldid"
                Dim dt1 As New DataTable
                da.Fill(dt1)
                Dim doctype As String
                Dim chrdoc() As String
                Dim rchdoc As String = ""
                Dim mstval As String = ""
                Dim Cserch As String = ""
                Dim IsUser As Boolean = False
                If dt1.Rows.Count > 0 Then
                    doctype = dt1.Rows(0).Item(0).ToString
                    mstval = dt1.Rows(0).Item(2).ToString
                    If mstval.ToUpper = "MASTER VALUED" Then
                        chrdoc = dt1.Rows(0).Item(1).ToString.Split("-")
                        If Not chrdoc(1).ToString().ToUpper = "USER" Then
                            rchdoc = "[V" & EID & chrdoc(1).Replace(" ", "_") & "]"
                            If chrdoc(2).ToString().Contains("fld") Then
                                da.SelectCommand.CommandText = "select displayname from mmm_mst_fields with(nolock) where documenttype='" & chrdoc(1).ToString() & "' and fieldmapping='" & chrdoc(2).ToString() & "' and eid=" & HttpContext.Current.Session("EID") & ""
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                Cserch = da.SelectCommand.ExecuteScalar()
                            Else
                                Cserch = chrdoc(2).ToString()
                            End If
                            doctype = "[V" & EID & doctype.Replace(" ", "_") & "]"
                        Else
                            IsUser = True
                        End If
                    ElseIf mstval.ToUpper = "SESSION VALUED" Then
                        IsUser = True
                    Else
                        doctype = ViewName
                    End If
                Else
                    doctype = ViewName
                End If
                '' dms.Get_LookupddlValue(51,'Employee Claims', V51Employee_Claims.[Grade],'fld15') like '%ex%'
                Dim newfilterstr As String = ""
                If dt1.Rows.Count > 0 Then
                    If dt1.Rows(0).Item(3).ToString.ToUpper <> "DATETIME" Then
                        If srch = "" Then
                            If dt1.Rows(0)("fieldtype").ToString.ToUpper = ("Lookupddl").ToUpper Then
                                Query = Query & " WHERE  dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & dt1.Rows(0).Item("DisplayName") & "]" & ",'" & dt1.Rows(0).Item("FieldMapping").ToString &
                                "')  like '%" & data("txtval").ToString() & "%' "
                                newfilterstr = " dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & dt1.Rows(0).Item("DisplayName") & "]" & ",'" & dt1.Rows(0).Item("FieldMapping").ToString &
                                "')  like '%" & data("txtval").ToString() & "%' "
                                ct = ct + 1

                            ElseIf rchdoc.ToString = "" Then
                                If Not IsUser Then
                                    Query = Query & " WHERE " & doctype & ".[" & data("ddlfields").ToString() & "] like '%" & data("txtval").ToString() & "%' "
                                    newfilterstr = "" & doctype & ".[" & data("ddlfields").ToString() & "] like '%" & data("txtval").ToString() & "%' "
                                Else
                                    Query = Query & " where  DMS.udf_split('" & dt1.Rows(0).Item("Dropdown").ToString() & "'," & ViewName & ".[" & dt1.Rows(0).Item("displayname").ToString() & "]) Like '%" & data("txtval").ToString() & "%' "
                                    newfilterstr = " DMS.udf_split('" & dt1.Rows(0).Item("Dropdown").ToString() & "'," & ViewName & ".[" & dt1.Rows(0).Item("displayname").ToString() & "]) Like '%" & data("txtval").ToString() & "%' "
                                End If
                                ct = ct + 1

                            Else
                                Query = Query & " WHERE " & rchdoc & ".[" & Cserch & "] like '%" & data("txtval").ToString() & "%' "
                                newfilterstr = "" & rchdoc & ".[" & Cserch & "] like '%" & data("txtval").ToString() & "%' "
                                ct = ct + 1
                            End If
                        Else
                            Query = Query & " WHERE " & srch & " like '%" & data("txtval").ToString() & "%'"
                            newfilterstr = "" & srch & " like '%" & data("txtval").ToString() & "%'"
                            ct = ct + 1
                        End If
                    ElseIf data("ddlfields").ToString() <> "-Select-" Then
                        Query = " set dateformat dmy;" & Query & " WHERE  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                        newfilterstr = "convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                        ct = ct + 1
                    End If
                Else
                    If data("ddlfields").ToString() = "adate" Then
                        Query = " set dateformat dmy;" & Query & " WHERE  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                        newfilterstr = " convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                        ct = ct + 1
                    ElseIf data("ddlfields") = "lastupdate" Then
                        Query = " set dateformat dmy;" & Query & " WHERE  convert(date,[MMM_Doc_Dtl].[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date,[MMM_Doc_Dtl].[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                    ElseIf data("ddlfields") = "userid" Then
                        Query = Query & " WHERE [MMM_Doc_Dtl].[" & data("ddlfields").ToString() & "] like '%" & data("ddlUserList").ToString() & "%' "
                    ElseIf data("txtval").ToString() <> "" Then
                        Query = Query & " WHERE " & doctype & ".[" & data("ddlfields").ToString() & "] like '%" & data("txtval").ToString() & "%' "
                        newfilterstr = "" & doctype & ".[" & data("ddlfields").ToString() & "] like '%" & data("txtval").ToString() & "%' "
                        ct = ct + 1
                    End If
                End If

                da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user with(nolock) where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'"

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                Dim doc As String = "0"
                Dim ob As New DynamicForm

                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "FCAGGN" Then

                Else
                    doc = ob.UserDataFilter_PreRole(DocumentType.ToString(), "MMM_MST_DOC")
                    If doc = "" Then
                        doc = "0"
                    End If
                    If HttpContext.Current.Session("SUBUID") = "" Then
                        HttpContext.Current.Session("SUBUID") = "0"
                    End If
                    If cnt > 0 Then
                        da.SelectCommand.Parameters.Clear()
                        'If HttpContext.Current.Session("EID").ToString <> "49" And HttpContext.Current.Session("EID").ToString <> "62" And HttpContext.Current.Session("EID").ToString <> "137" And HttpContext.Current.Session("EID").ToString <> "138" And HttpContext.Current.Session("EID").ToString <> "46" And HttpContext.Current.Session("EID").ToString <> "152" And HttpContext.Current.Session("EID").ToString <> "158" And HttpContext.Current.Session("EID").ToString <> "144" And HttpContext.Current.Session("EID").ToString <> "173" And HttpContext.Current.Session("EID").ToString <> "174" And HttpContext.Current.Session("EID").ToString <> "177" Then
                        If Convert.ToInt64(HttpContext.Current.Session("EID")) < 46 Then
                            da.SelectCommand.CommandText = "uspGetUserRightIDnew"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                            da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                            da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                            Query = Query & " and " & ViewName & ".ouid in (" & str & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                        Else
                            da.SelectCommand.CommandText = "uspGetRightOnDoc"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                            da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                            da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                            da.SelectCommand.Parameters.AddWithValue("@docType", DocumentType.ToString)
                            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                            If str = "" Then
                                str = " and " & ViewName & " .TID in (0)"
                            End If
                            If ct > 0 Then
                                'If HttpContext.Current.Session("EID") = 62 Then
                                '    Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                                'Else
                                Query = Query & " " & str & " or (" & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & " )) and " & newfilterstr & ""
                                'End If
                            Else
                                'If HttpContext.Current.Session("EID") = 62 Then
                                'str = Replace(str, (Left(str, 6)), " Where ")
                                'Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") "
                                'Else
                                str = Replace(str, (Left(str, 6)), " Where ")
                                Query = Query & " " & str & " or (" & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & " )) and " & newfilterstr & ""
                                'End If
                            End If
                        End If
                    Else
                        Query = Query & " and " & ViewName & ".ouid in (" & HttpContext.Current.Session("UID") & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & ") and " & newfilterstr & " "
                    End If
                End If
                Query = "set dateformat dmy; " & Query & " order by " & ViewName & ".TID desc"
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = Query
                da.SelectCommand.CommandTimeout = 1200
                dt.Clear()
                da.Fill(dt)
                HttpContext.Current.Session("grd") = dt
                '  ViewState("grd") = dt
                If dt.Rows.Count > 0 Then
                    grid = DynamicGrid.GridData(dt, strError)
                Else
                    strError = "No data found"
                    grid = DynamicGrid.GridData(dt, strError)

                End If

                'If dt.Rows.Count > 0 Then
                '    gvPending.DataSource = dt
                '    gvPending.DataBind()
                '    lblmsg.Text = ""
                'Else
                '    lblmsg.Text = "Data Not Found..."
                '    gvPending.Controls.Clear()
                'End If
            End If
        Catch ex As Exception
            grid = DynamicGrid.GridData(New DataTable(), "Error occured at server please contact your system administrator.")
        Finally
            con.Dispose()
        End Try
        Return grid
    End Function
    Public Shared Function GenearateQuery2(EID As Integer, ByRef StrColumn As String, StrJoinString As String, SchemaString As String, DocumentType As String, ds As DataSet, fld As String, ByVal displayName As String, ByVal MainDocumentType As String) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim dispname As String = ""
        'StrColumn = ""
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            Dim ddlDocType = ""

            For i As Integer = 0 To tbl.Rows.Count - 1
                If fld.ToUpper.Contains("FLD") Then
                    If fld.ToUpper = tbl.Rows(i).Item("Fieldmapping").ToString.ToUpper Then
                        'If (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                        '    Dim arrddl = tbl.Rows(i).Item("Dropdown").ToString().Split("-")
                        '    ddlDocType = arrddl(1)
                        '    Dim FieldMalling = arrddl(2)
                        '    Dim DR As DataRow() = ds.Tables(0).Select("FieldMapping='" & FieldMalling & "' AND DocumentType='" & arrddl(1) & "'")
                        '    If DR.Count > 0 Then
                        '        Dim DisplayName = DR(0).Item("DisplayName")
                        '        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & tbl.Rows(i).Item("DisplayName") & "]"
                        '        StrColumn = StrColumn & "," & str1
                        '    End If
                        'Else

                        'End If
                        'If (tbl.Rows(i).Item("datatype").ToString.ToUpper) = "DATETIME" Then
                        '    dispname = tbl.Rows(i).Item("displayname").ToString
                        '    StrColumn = StrColumn & ",convert(date," & ViewName & ".[" & dispname & "]) AS [" & dispname & "]"
                        'Else
                        dispname = tbl.Rows(i).Item("displayname").ToString()
                        'Change condition if master comes more than one but tid is different for 
                        If StrColumn.ToString().Contains(ViewName & ".[" & dispname & "]") Then
                            StrColumn = StrColumn & ", " & "(select  " & ViewName & ".[" & dispname & "] from " & ViewName & " where " & ViewName & ".tid in([V" & EID & MainDocumentType.Replace(" ", "_") & "]" & ".[" & displayName & "]))  AS [" & displayName & "]"
                        Else
                            StrColumn = StrColumn & ", " & ViewName & ".[" & dispname & "] AS [" & displayName & "]"
                        End If


                        Exit For
                        'End If
                    End If
                Else
                    StrColumn = StrColumn & ", " & ViewName & ".[" & fld & "] AS [" & displayName & "]"
                    Exit For
                End If
            Next
        End If
        Return StrColumn
    End Function
    Public Shared Function GenearateQuery3(EID As Integer, ByRef StrColumn As String, DocumentType As String, ds As DataSet) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim dispname As String = ""
        'StrColumn = ""
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                'If fld.ToUpper = tbl.Rows(i).Item("Fieldmapping").ToString.ToUpper Then
                If (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    Dim arrddl = tbl.Rows(i).Item("Dropdown").ToString().Split("-")
                    ddlDocType = arrddl(1)
                    Dim FieldMalling = arrddl(2)
                    Dim DR As DataRow() = ds.Tables(0).Select("FieldMapping='" & FieldMalling & "' AND DocumentType='" & arrddl(1) & "'")
                    If DR.Count > 0 Then
                        Dim DisplayName = DR(0).Item("DisplayName")
                        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & tbl.Rows(i).Item("DisplayName") & "]"
                        StrColumn = StrColumn & "," & str1
                    End If
                Else
                    'End If
                    ' dispname = tbl.Rows(i).Item("displayname").ToString
                    'StrColumn = StrColumn & "," & ViewName & ".[" & dispname & "] AS [" & dispname & "]"
                End If
            Next
        End If
        Return StrColumn
    End Function
    Public Shared Function GenearateQuery4(EID As Integer, DocumentType As String, ds As DataSet, fld As String) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim dispname As String = ""
        Dim srch As String = ""
        'StrColumn = ""
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                If fld.ToUpper = tbl.Rows(i).Item("Fieldmapping").ToString.ToUpper Then
                    'If (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    '    Dim arrddl = tbl.Rows(i).Item("Dropdown").ToString().Split("-")
                    '    ddlDocType = arrddl(1)
                    '    Dim FieldMalling = arrddl(2)
                    '    Dim DR As DataRow() = ds.Tables(0).Select("FieldMapping='" & FieldMalling & "' AND DocumentType='" & arrddl(1) & "'")
                    '    If DR.Count > 0 Then
                    '        Dim DisplayName = DR(0).Item("DisplayName")
                    '        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & tbl.Rows(i).Item("DisplayName") & "]"
                    '        StrColumn = StrColumn & "," & str1
                    '    End If
                    'Else

                    'End If
                    'If (tbl.Rows(i).Item("datatype").ToString.ToUpper) = "DATETIME" Then
                    '    dispname = tbl.Rows(i).Item("displayname").ToString
                    '    srch = "Convert(date," & ViewName & ".[" & dispname & "])"
                    'Else
                    dispname = tbl.Rows(i).Item("displayname").ToString
                    srch = ViewName & ".[" & dispname & "]"
                    'End If

                End If
            Next
        End If
        Return srch
    End Function
    Public Shared Function SLAOutDateQuery(EID As Integer, DocumentType As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrSRC").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim SLADTColumn As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        da.SelectCommand.CommandText = "select Statusname from mmm_mst_workflow_status with(nolock) where eid=" & EID & " and documenttype='" & DocumentType & "' and showinsearch=1 order by dord "
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                'SLADTColumn = SLADTColumn & ", (select convert(nvarchar,convert(date,max(tdate),3)) from mmm_doc_dtl with(nolock) where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID])[" & dt.Rows(i).Item(0).ToString & " Out Date]"
                SLADTColumn = SLADTColumn & ", (select convert(varchar, max(tdate), 3) from mmm_doc_dtl with(nolock) where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID])[" & dt.Rows(i).Item(0).ToString & " Out Date]"
            Next
        End If
        Return SLADTColumn
    End Function
    Public Shared Function SLAUseQuery(EID As Integer, DocumentType As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrSRC").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim SLAUNColumn As String = ""
        Dim SLADTCol As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        da.SelectCommand.CommandText = "select Statusname from mmm_mst_workflow_status with(nolock) where eid=" & EID & " and documenttype='" & DocumentType & "' and showinsearch=1 order by dord "
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                SLAUNColumn = SLAUNColumn & ", (select top 1 u.username from mmm_doc_dtl dt with(nolock) inner join mmm_mst_user u with(nolock) on u.uid=dt.userid   where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID])[" & dt.Rows(i).Item(0).ToString & " UserName]"
            Next
        End If
        If SLAUNColumn.Length > 1 Then
            SLADTCol = SLAOutDateQuery(EID, DocumentType)
            SLADTCol = SLADTCol & SLAUNColumn
        End If
        Return SLADTCol
    End Function

    Public Shared Function CurrentUserandLastActionDate(EID As Integer, DocumentType As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrSRC").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim CRUserandLastDate As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        CRUserandLastDate = CRUserandLastDate & ", (select username from mmm_mst_user with(nolock) where uid=(select top 1 userid from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID] and TID=" & ViewName & ".[LastTID]))[Current User Name],(select replace(convert(varchar,CAST(max(tdate) as date),103),' ','-') from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID])[Last Action Date],(select username from mmm_mst_user with(nolock) where uid=(select top 1 ouid from mmm_mst_doc with(nolock) where tid=" & ViewName & ".[TID]))[Creator Name] "
        Return CRUserandLastDate
    End Function
    'Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs) Handles btnSearch.Click
    '    If ddlDocType.SelectedValue.ToString <> "0" Then
    '        GenearateQuery(Session("EID"), ddlDocType.SelectedValue.ToString, False)
    '    End If
    'End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
    End Sub
    Public Function GenearateQueryExl(EID As Integer, DocumentType As String, ds As DataSet) As DataTable
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim tbCh As DataTable
        Dim tbchitem As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim cDoc As String = ""
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim srch As String = ""
        Dim ct As Integer = 0
        Dim ViewNamech = ""
        Dim fld As String = ""
        Dim SchemaString As String = DocumentType
        Try
            If ds.Tables(0).Rows.Count > 0 Then
                View = ds.Tables(0).DefaultView
                View.RowFilter = "DocumentType='" & DocumentType & "'"
                'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
                tbl = View.Table.DefaultView.ToTable()
                Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
                Dim ddlDocType = ""
                For i As Integer = 0 To tbl.Rows.Count - 1
                    If (tbl.Rows(i).Item("DROPDOWNTYPE").ToString.ToUpper = "SESSION VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                        StrColumn = StrColumn & "," & "(select username from mmm_mst_user with(nolock) where convert(varchar,uid)=" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "]) AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                    ElseIf tbl.Rows(i).Item("DROPDOWNTYPE").ToString.ToUpper = "MASTER VALUED" And tbl.Rows(i).Item("FieldType").ToString = "AutoComplete" Then
                        StrColumn = StrColumn & ",DMS.udf_split('" & tbl.Rows(i).Item("Dropdown").ToString & "'," & ViewName & ".  [" & tbl.Rows(i).Item("DisplayName") & "]) AS [" & tbl.Rows(i).Item("DisplayName") & "] "
                    ElseIf Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                        If tbl.Rows(i).Item("Fieldtype").ToString = "Child Item" Then
                            cDoc = tbl.Rows(i).Item("Dropdown").ToString
                            If cDoc <> "" Then
                                ViewNamech = "[V" & EID & cDoc.Replace(" ", "_") & "]"
                                View.RowFilter = "DocumentType='" & cDoc & "'"
                                tbCh = View.Table.DefaultView.ToTable()
                                'Change due to DOC detail confliction with Session("Child") name
                                HttpContext.Current.Session("SearchChild") = tbCh
                                For r As Integer = 0 To tbCh.Rows.Count - 1
                                    If Not (tbCh.Rows(r).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbCh.Rows(r).Item("FieldType") = "Drop Down") Then
                                        If tbCh.Rows(r).Item("Datatype").ToString.ToUpper = "DATETIME" Then
                                            StrColumn = StrColumn & "," & ViewNamech & ".[" & tbCh.Rows(r).Item("DisplayName") & "] AS [" & tbCh.Rows(r).Item("DisplayName") & "]"
                                        ElseIf tbCh.Rows(r).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbCh.Rows(r).Item("FieldType") = "AutoComplete" Then
                                            StrColumn = StrColumn & ",DMS.udf_split('" & tbCh.Rows(r).Item("Dropdown").ToString & "'," & ViewNamech & ".  [" & tbCh.Rows(r).Item("DisplayName") & "]) AS [" & tbCh.Rows(r).Item("DisplayName") & "] "
                                        Else
                                            StrColumn = StrColumn & "," & ViewNamech & ".[" & tbCh.Rows(r).Item("DisplayName") & "] AS [" & tbCh.Rows(r).Item("DisplayName") & "]"
                                        End If
                                    End If
                                Next
                                StrJoinString = StrJoinString & " left outer join " & ViewNamech & " with(nolock) on " & ViewNamech & ".docid = " & ViewName & ".TID"
                                View.RowFilter = "DocumentType='" & cDoc & "' AND FieldType ='DROP DOWN' AND DropDownType='MASTER VALUED'"
                                tbchitem = View.Table.DefaultView.ToTable()
                                For k As Integer = 0 To tbchitem.Rows.Count - 1
                                    Dim arrddl = tbchitem.Rows(k).Item("Dropdown").ToString().Split("-")
                                    ddlDocType = arrddl(1)
                                    SchemaString = SchemaString & "." & ddlDocType
                                    Dim ddlview = "[v" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
                                    Dim joincolumn = "tid"
                                    Dim DisPlayName = tbchitem.Rows(k).Item("DisplayName").ToString().Trim
                                    If ddlDocType.Trim.ToUpper = "USER" Then
                                        joincolumn = "UID"
                                        ddlview = "MMM_MST_USER "
                                    End If
                                    If StrJoinString.Contains(" left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = ") Then
                                    Else
                                        StrJoinString = StrJoinString & " left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = " & ViewNamech & ".[" & DisPlayName & "]"
                                    End If
                                Next
                                GenearateQuery3(EID, StrColumn, cDoc, ds)
                            End If
                            cDoc = ""
                            Continue For
                        ElseIf tbl.Rows(i).Item("Fieldtype").ToString = "LookupDDL" Then
                            StrColumn = StrColumn & ", dms.Get_LookupddlValue(" & Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "]" & ",'" & tbl.Rows(i).Item("FieldMapping").ToString &
                                "') AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                            Continue For
                        End If
                        If (tbl.Rows(i).Item("datatype").ToString.ToUpper) = "DATETIME" Then
                            'StrColumn = StrColumn & ",case when isdate(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])=1 then convert(nvarchar,convert(date," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])) else " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] End AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                            StrColumn = StrColumn & ",case when isdate(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])=1 then convert(varchar,CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] as date),103) else " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] End AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                        Else
                            StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                        End If

                    End If
                Next
                View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='DROP DOWN' AND DropDownType='MASTER VALUED'"
                tblRe = View.Table.DefaultView.ToTable()
                For j As Integer = 0 To tblRe.Rows.Count - 1
                    Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
                    ddlDocType = arrddl(1)
                    fld = Left(arrddl(1), 3)
                    SchemaString = SchemaString & "." & ddlDocType
                    Dim ddlview = "[v" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
                    Dim joincolumn = "tid"
                    Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
                    If ddlDocType.Trim.ToUpper = "USER" Then
                        joincolumn = "UID"
                        If fld.ToUpper = "FLD" Then
                        Else
                            ddlview = " MMM_MST_USER "
                        End If
                    End If
                    If StrJoinString.Contains(" left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = ") Then
                    Else
                        StrJoinString = StrJoinString & " left outer join " & ddlview & " with(nolock) on convert(varchar," & ddlview & "." & joincolumn & ") = " & ViewName & ".[" & DisPlayName & "]"
                    End If
                    If Not ddlDocType.Trim.ToUpper = "USER" Then
                        GenearateQuery2(EID, StrColumn, StrJoinString, SchemaString, ddlDocType, ds, arrddl(2), displayName:=DisPlayName, MainDocumentType:=DocumentType)
                    Else
                        StrColumn = StrColumn & ",DMS.udf_split('" & tblRe.Rows(j).Item("Dropdown").ToString() & "'," & ViewName & ".[" & DisPlayName & "])" & " AS [" & DisPlayName & "]"
                    End If

                    If ddlfields.SelectedItem.Text = tblRe.Rows(j).Item("DisplayName").ToString().Trim Then
                        srch = GenearateQuery4(EID, ddlDocType, ds, arrddl(2))
                    End If
                Next

                Dim SLACol As String = ""
                Dim CUserandLSDADate As String = ""

                SLACol = SLAUseQuery(EID, DocumentType)
                CUserandLSDADate = CurrentUserandLastActionDate(EID, DocumentType)
                'Dim Query = "Select distinct  " & ViewName & ".[TID] As [DocID] , " & StrColumn.Substring(1, StrColumn.Length - 1) & ", " & ViewName & ".[CurStatus] As [Current Status], Convert(nvarchar, " & ViewName & ".[adate], 3) As [Creation Date]" & " FROM " & ViewName & " With(nolock) " & StrJoinString
                Dim Query = "Select  " & ViewName & ".[TID] As [DocID] ," & StrColumn.Substring(1, StrColumn.Length - 1) & "," & ViewName & ".[CurStatus] As [Current Status],convert(varchar," & ViewName & ".[adate],3) As [Creation Date]" & " " & SLACol & " " & CUserandLSDADate & "   FROM " & ViewName & " With(nolock) " & StrJoinString

                'prev    da.SelectCommand.CommandText = "select documenttype,dropdown,dropdowntype,datatype ,fieldtype ,FieldMapping,DisplayName from mmm_mst_fields where eid='" & Session("EID") & "' and (documenttype='" & DocumentType.ToString & "' and displayname='" & ddlfields.SelectedValue.ToString & "') or (documenttype=( select dropdown from mmm_mst_fields where documenttype='" & DocumentType.ToString & "' and fieldtype='child item' and isSearch=1 and eid=" & Session("EID") & ") and displayname='" & ddlfields.SelectedValue.ToString & "')"
                da.SelectCommand.CommandText = "select documenttype,dropdown,dropdowntype,datatype ,fieldtype ,FieldMapping,DisplayName from mmm_mst_fields with(nolock) where eid='" & Session("EID") & "' and (documenttype='" & DocumentType.ToString & "' and displayname='" & ddlfields.SelectedValue.ToString & "') or (documenttype in ( select dropdown from mmm_mst_fields with(nolock) where documenttype='" & DocumentType.ToString & "' and fieldtype='child item' and isSearch=1 and eid=" & Session("EID") & ") and displayname='" & ddlfields.SelectedValue.ToString & "')"
                Dim dt1 As New DataTable
                da.Fill(dt1)
                Dim doctype As String
                Dim chrdoc() As String
                Dim rchdoc As String = ""
                Dim mstval As String = ""
                If dt1.Rows.Count > 0 Then
                    doctype = dt1.Rows(0).Item(0).ToString
                    mstval = dt1.Rows(0).Item(2).ToString
                    If mstval.ToUpper = "MASTER VALUED" Then
                        chrdoc = dt1.Rows(0).Item(1).ToString.Split("-")
                        rchdoc = "[V" & EID & chrdoc(1).Replace(" ", "_") & "]"
                    End If
                    doctype = "[V" & EID & doctype.Replace(" ", "_") & "]"
                Else
                    doctype = ViewName
                End If
                '' dms.Get_LookupddlValue(51,'Employee Claims', V51Employee_Claims.[Grade],'fld15') like '%ex%'
                Dim newfilterstr As String = ""
                If dt1.Rows.Count > 0 Then
                    If dt1.Rows(0).Item(3).ToString.ToUpper <> "DATETIME" Then
                        If srch = "" Then
                            If dt1.Rows(0)("fieldtype").ToString.ToUpper = ("Lookupddl").ToUpper Then
                                Query = Query & " WHERE  dms.Get_LookupddlValue(" & Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & dt1.Rows(0).Item("DisplayName") & "]" & ",'" & dt1.Rows(0).Item("FieldMapping").ToString &
                                "')  like '%" & txtval.Text & "%' "
                                newfilterstr = " dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & dt1.Rows(0).Item("DisplayName") & "]" & ",'" & dt1.Rows(0).Item("FieldMapping").ToString &
                               "')  like '%" & txtval.Text.ToString() & "%' "
                                ct = ct + 1

                            ElseIf rchdoc.ToString = "" Then
                                Query = Query & " WHERE " & doctype & ".[" & ddlfields.SelectedValue.ToString & "] like '%" & txtval.Text & "%' "
                                newfilterstr = "" & doctype & ".[" & ddlfields.SelectedValue.ToString & "] like '%" & txtval.Text.ToString() & "%' "
                                ct = ct + 1

                            Else
                                Query = Query & " WHERE " & rchdoc & ".[" & ddlfields.SelectedValue.ToString & "] like '%" & txtval.Text & "%' "
                                newfilterstr = "" & rchdoc & ".[" & ddlfields.SelectedValue.ToString & "] like '%" & txtval.Text & "%' "
                                ct = ct + 1
                            End If
                        Else
                            Query = Query & " WHERE " & srch & " like '%" & txtval.Text & "%'"
                            newfilterstr = "" & srch & " like '%" & txtval.Text & "%'"
                            ct = ct + 1
                        End If
                    ElseIf ddlfields.SelectedItem.Text <> "-Select-" Then
                        Query = " set dateformat dmy;" & Query & " WHERE  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) >= '" & txtsdate.Text & "' and  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) <='" & txtedate.Text & "'"
                        newfilterstr = " convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) >= '" & txtsdate.Text & "' and  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) <='" & txtedate.Text & "'"
                        ct = ct + 1
                    End If
                Else
                    If ddlfields.SelectedItem.Text.ToUpper = "CREATION DATE" Then
                        Query = " set dateformat dmy;" & Query & " WHERE  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) >= '" & txtsdate.Text & "' and  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) <='" & txtedate.Text & "'"
                        newfilterstr = " convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) >= '" & txtsdate.Text & "' and  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString & "]) <='" & txtedate.Text & "'"
                        ct = ct + 1
                    ElseIf txtval.Text <> "" Then
                        Query = Query & " WHERE " & doctype & ".[" & ddlfields.SelectedValue.ToString & "] like '%" & txtval.Text & "%' "
                        newfilterstr = "" & doctype & ".[" & ddlfields.SelectedValue.ToString & "] like '%" & txtval.Text & "%' "
                        ct = ct + 1
                    End If
                End If

                da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user with(nolock) where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'"

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                Dim doc As String = "0"
                Dim ob As New DynamicForm

                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "FCAGGN" Then

                Else
                    doc = ob.UserDataFilter_PreRole(DocumentType.ToString(), "MMM_MST_DOC")
                    If doc = "" Then
                        doc = "0"
                    End If
                    If HttpContext.Current.Session("SUBUID") = "" Then
                        HttpContext.Current.Session("SUBUID") = "0"
                    End If
                    If cnt > 0 Then
                        da.SelectCommand.Parameters.Clear()
                        'If HttpContext.Current.Session("EID").ToString <> "49" And HttpContext.Current.Session("EID").ToString <> "62" And HttpContext.Current.Session("EID").ToString <> "137" And HttpContext.Current.Session("EID").ToString <> "138" And HttpContext.Current.Session("EID").ToString <> "46" And HttpContext.Current.Session("EID").ToString <> "152" And HttpContext.Current.Session("EID").ToString <> "158" And HttpContext.Current.Session("EID").ToString <> "144" Then
                        If Convert.ToInt64(HttpContext.Current.Session("EID")) < 46 Then
                            da.SelectCommand.CommandText = "uspGetUserRightIDnew"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                            da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                            da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                            Query = Query & " and " & ViewName & ".ouid in (" & str & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                        Else
                            da.SelectCommand.CommandText = "uspGetRightOnDoc"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                            da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                            da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                            da.SelectCommand.Parameters.AddWithValue("@docType", DocumentType.ToString)
                            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                            If str = "" Then
                                str = " and " & ViewName & " .TID in (0)"
                            End If
                            If ct > 0 Then
                                'If HttpContext.Current.Session("EID") = 62 Then
                                '    Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                                'Else
                                Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & " ) and " & newfilterstr & ""
                                'End If
                            Else
                                'If HttpContext.Current.Session("EID") = 62 Then
                                'str = Replace(str, (Left(str, 6)), " Where ")
                                'Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") "
                                'Else
                                str = Replace(str, (Left(str, 6)), " Where ")
                                Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & " ) and " & newfilterstr & ""
                                'End If
                            End If
                        End If
                    Else
                        Query = Query & " and " & ViewName & ".ouid in (" & HttpContext.Current.Session("UID") & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & ") and " & newfilterstr & " "
                    End If
                End If
                Query = "set dateformat dmy; " & Query & " order by " & ViewName & ".TID desc"
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = Query
                da.SelectCommand.CommandTimeout = 1200
                dt.Clear()
                da.Fill(dt)

                ViewState("grd") = dt
                If dt.Rows.Count > 0 Then
                    'gvPending.DataSource = dt
                    'gvPending.DataBind()
                    lblmsg.Text = ""
                Else
                    lblmsg.Text = "Data Not Found..."
                    ' gvPending.Controls.Clear()
                End If
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
        Return dt
    End Function
    Protected Sub btnViewInExcel_Click(sender As Object, e As EventArgs) Handles btnViewInExcel.Click
        Response.ClearContent()
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
        dt = GenearateQueryExl(Session("EID"), ddlDocType.SelectedValue.ToString, ds1)
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
        Dim datatype As String
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
        Response.Output.Write(HttpUtility.HtmlDecode(sw.ToString().Replace("&nbsp;", "")))
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
