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

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrDt").ConnectionString
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
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Try

                Dim ds As New DataSet

                da.SelectCommand.CommandText = "select tid,fld1 as [MOUNo] from mmm_mst_doc with(nolock) where DocumentType='lease mou' and EID=" & Session("EID") & "  and curstatus='ARCHIVE' and tid in (select   fld1 from mmm_mst_doc with(nolock) where DocumentType='MOU lease Document' and EID=" & Session("EID") & " and curstatus='ARCHIVE')"
                da.Fill(ds, "data")
                ddlDocType.DataSource = ds
                ddlDocType.DataTextField = "MOUNo"
                ddlDocType.DataValueField = "tid"
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
        Dim da As New SqlDataAdapter("select tid as DispText,fld50[DispVal] from mmm_mst_doc with(nolock) where DocumentType='MOU lease Document'  and fld1='" & ddlDocType.SelectedValue.ToString & "' and curstatus='ARCHIVE' and eid=" & Session("EID") & "", con)

        Try
            Dim ds As New DataSet
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlfields.DataSource = ds
                ddlfields.DataTextField = "DispVal"
                ddlfields.DataValueField = "DispText"
                ddlfields.DataBind()
                ddlfields.Items.Insert("0", "-Select-")
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")

                txtval.Attributes.Add("data-ty", "TEXT")
                txtval.Attributes.Add("IsSearch", "1")
                txtval.Attributes.Add("fld", "txtval")
                txtval.Visible = False
                pnl1.Visible = False
            End If

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

    Public Shared Function GetAllFields(EID As Integer) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrDt").ConnectionString
        Try
            '            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & " and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number');"
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
            grid = GenearateQuery1(eid, "MOU lease Document", ds, data)
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
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
                            StrColumn = StrColumn & ",case when isdate(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])=1 then convert(nvarchar,convert(date," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "])) else " & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] End AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                        Else
                            StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                        End If
                    End If
                Next
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
                Dim Query = "Select  distinct " & ViewName & ".[TID] As [DocID] ," & StrColumn.Substring(1, StrColumn.Length - 1) & "," & ViewName & ".[CurStatus] As [Current Status],convert(nvarchar,convert(Date," & ViewName & ".[adate],3)) As [Creation Date]" & " " & SLACol & " " & CUserandLSDADate & "   FROM " & ViewName & " With(nolock) " & StrJoinString

                da.SelectCommand.CommandText = "Select documenttype,dropdown,dropdowntype,datatype ,fieldtype ,FieldMapping,DisplayName from mmm_mst_fields with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and (documenttype='" & DocumentType.ToString & "' and displayname='Lease Doc No') or (documenttype in ( select dropdown from mmm_mst_fields with(nolock) where documenttype='" & DocumentType.ToString & "' and fieldtype='child item' and isSearch=1 and eid=" & HttpContext.Current.Session("EID") & ") and displayname='Lease Doc No' and eid=" & HttpContext.Current.Session("EID") & ")"
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

                    Query = Query & " WHERE  " & doctype & ".curstatus='ARCHIVE' and " & doctype & ".tid=" & data("ddlfields").ToString() & ""


                End If


                Query = "set dateformat dmy; " & Query & " order by " & ViewName & ".TID desc"

                HttpContext.Current.Session("Query") = Query
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
                    grid.Column.Clear()
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim SLADTColumn As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        da.SelectCommand.CommandText = "select Statusname from mmm_mst_workflow_status with(nolock) where eid=" & EID & " and documenttype='" & DocumentType & "' and showinsearch=1 order by dord "
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                SLADTColumn = SLADTColumn & ", (select convert(date,max(tdate),3) from mmm_doc_dtl with(nolock) where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID])[" & dt.Rows(i).Item(0).ToString & " Out Date]"
            Next
        End If
        Return SLADTColumn
    End Function
    Public Shared Function SLAUseQuery(EID As Integer, DocumentType As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim CRUserandLastDate As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        CRUserandLastDate = CRUserandLastDate & ", (select username from mmm_mst_user with(nolock) where uid=(select top 1 userid from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID] and TID=" & ViewName & ".[LastTID]))[Current User Name],(select replace(convert(varchar(50),convert(date,max(tdate),3),106),' ','-') from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID])[Last Action Date],(select username from mmm_mst_user with(nolock) where uid=(select top 1 ouid from mmm_mst_doc with(nolock) where tid=" & ViewName & ".[TID]))[Creator Name] "
        Return CRUserandLastDate
    End Function

    Public Function GenearateQueryExl(EID As Integer, DocumentType As String, ds As DataSet) As DataTable

        Dim con As New SqlConnection(conStr)
        Dim dt As New DataTable

        Try
            Dim Query As String = HttpContext.Current.Session("Query")
            Dim da As New SqlDataAdapter(Query, con)

            da.Fill(dt)

        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
        Return dt
    End Function
    Protected Sub btnViewInExcel_Click(sender As Object, e As EventArgs) Handles btnViewInExcel.Click

        Dim dt As New DataTable
        Dim ResultId As Integer = 0
        Dim gridview1 As New GridView
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim DtTable As New DataTable
        Dim FieldIdstr As String = String.Empty

        Response.ClearContent()
        Dim ds As New DataTable
        Dim ds1 As New DataSet
        ' Dim DocumentType As String
        ds1 = GetAllFields(Session("EID"))
        'DocumentType = ds1.Tables(0).Rows(0).Item("EventName").ToString
        'gridview1.DataSource = ViewState("grd")
        dt = GenearateQueryExl(Session("EID"), "MOU lease Document", ds1)

        If dt.Rows.Count > 0 Then
            Dim values As List(Of Integer) = New List(Of Integer)
            Dim result As List(Of Integer) = values.Distinct().ToList
            Dim rowMGAmt As DataRow() = dt.Select()
            If rowMGAmt.Length > 0 Then
                For Each CField As DataRow In rowMGAmt
                    values.Add(CField.Item("Docid").ToString)
                Next
            End If
            result = values.Distinct().ToList()

            Dim test As String = FieldIdstr
            Using con As New SqlConnection(constr)
                Using cmd As New SqlCommand("sp_insertingIntodraftDoc")
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd.Connection = con
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("@ResultID", SqlDbType.Int).Direction = ParameterDirection.Output
                    cmd.Parameters.Add(New SqlParameter("@eid", Session("EID")))
                    cmd.Parameters.Add(New SqlParameter("@Documenttype", "MOU lease Document"))
                    cmd.Parameters.Add(New SqlParameter("@FieldId", result(0)))                '
                    cmd.Parameters.Add(New SqlParameter("@DocumentID", result(0)))

                    cmd.ExecuteNonQuery()
                    ResultId = Convert.ToInt32(cmd.Parameters("@ResultID").Value)
                    If ResultId <> 0 Then
                        lblmsg.Text = "Document Saved as Draft is :" & ResultId & ""
                    Else
                        lblmsg.Text = "Document NOT Saved as Draft ..."

                    End If
                End Using
            End Using
        End If



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
