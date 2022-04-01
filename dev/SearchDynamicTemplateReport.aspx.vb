Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
Partial Class SearchDynamicTemplateReport
    Inherits System.Web.UI.Page
    Shared FormType As String = ""
    Shared doc1 As String = ""
    Shared ReportName As String = ""
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
        'ReportName = txtreportName.Text
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            pnl1.Visible = False
            txtsdate.Attributes.Add("autocomplete", "off")
            txtedate.Attributes.Add("autocomplete", "off")
            Dim yr As String = Now.Year.ToString
            Dim mm As String = Now.Month.ToString.PadLeft(2, "0")
            Dim dd As String = Now.Day.ToString.PadLeft(2, "0")

            Try
                Dim ds As New DataSet
                If Session("USERROLE") = "SU" Then
                    da.SelectCommand.CommandText = "select A.Temp_Name+'-'+B.FormName+'-'+A.FormType TempName,A.Temp_Description,A.FormType from mmm_rep_template_config A " &
                                                    "join mmm_mst_forms B on (A.documenttype=B.formname)" &
                                                    "where A.Temp_Description is not null and B.eid='" & Session("EID") & "' and  B.formsource='MENU DRIVEN' and A.EID='" & Session("EID") & "'"
                    da.Fill(ds, "data")
                    ddlDocType.DataSource = ds
                    ddlDocType.DataTextField = "Temp_Description"
                    ddlDocType.DataValueField = "TempName"
                    ddlDocType.DataBind()

                Else
                    'da.SelectCommand.CommandText = "select A.Temp_Name+'-'+replace(B.pagelink,'Documents.Aspx?SC=','')[DocName],A.Temp_Description from mmm_rep_template_config A " &
                    '                                "join mmm_mst_menu B on(A.documenttype=B.menuname)" &
                    '                                "where roles like '%{" & Session("USERROLE") & ":%' and B.eid=" & Session("EID") & " and pagelink like 'documents%' and A.Temp_Description is not null"


                    da.SelectCommand.CommandText = "select A.Temp_Name+'-'+A.Documenttype+'-'+A.FormType DocName,A.Temp_Description,A.FormType from mmm_rep_template_config A where " &
                                                    " A.Temp_Description Is Not null And A.Eid='" & Session("EID") & "' and " & "+" & "','" & " + a.AllowedRoles +" & "','" & "+''" & " like '%," & Session("USERROLE") & ",%'"

                    da.Fill(ds, "data")
                    ddlDocType.DataSource = ds
                    ddlDocType.DataTextField = "Temp_Description"
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
        doc1 = ddlDocType.SelectedValue.Split("-")(1)
        ReportName = ddlDocType.SelectedValue.Split("-")(0)
        'ReportName = txtreportName.Text
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
    Public Shared Function SLAInDateQuery(EID As Integer, DocumentType As String, ps As DataSet) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim fts As New DataSet
        Dim SLADTColumn As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        Dim Pre As String = ""

        If Not IsDBNull(ps.Tables(0).Rows(0).Item("StagesInDate")) Then
            Pre = ps.Tables(0).Rows(0).Item("StagesInDate")
            Pre = Pre.Replace(",", "','")
            da.SelectCommand.CommandText = "Select Statusname from mmm_mst_workflow_status with(nolock) where eid=" & EID & " And documenttype='" & DocumentType & "' and Statusname in ('" & Pre & "') order by dord "
        End If
        If da.SelectCommand.CommandText.ToString.Length > 0 Then
            da.Fill(dt)
        End If

        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                SLADTColumn = SLADTColumn & ",(select convert(varchar(23),max(fdate),120) from mmm_doc_dtl with(nolock) where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID])[" & dt.Rows(i).Item(0).ToString & " In Date]"
            Next
        End If
        Return SLADTColumn
    End Function
    Public Shared Function SLAOutDateQuery(EID As Integer, DocumentType As String, ps As DataSet) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim fts As New DataSet
        Dim SLADTColumn As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
        Dim Pre As String = ""
        If Not IsDBNull(ps.Tables(0).Rows(0).Item("Computedfield")) Then
            Pre = ps.Tables(0).Rows(0).Item("Computedfield")
            Pre = Pre.Replace(",", "','")
            da.SelectCommand.CommandText = "Select Statusname from mmm_mst_workflow_status with(nolock) where eid=" & EID & " And documenttype='" & DocumentType & "' and Statusname in ('" & Pre & "') order by dord "
        End If
        If da.SelectCommand.CommandText.ToString.Length > 0 Then
            da.Fill(dt)
        End If
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                SLADTColumn = SLADTColumn & ",(select convert(varchar(23),max(tdate),120) from mmm_doc_dtl with(nolock) where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID])[" & dt.Rows(i).Item(0).ToString & " Out Date]"
            Next
        End If
        Return SLADTColumn
    End Function
    Public Shared Function SLAUseQuery(EID As Integer, DocumentType As String, ps As DataSet) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim SLAUNColumn As String = ""
        Dim SLADTCol As String = ""
        Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"

        Dim Pre As String = ""
        If Not IsDBNull(ps.Tables(0).Rows(0).Item("Computedfield")) Then
            Pre = ps.Tables(0).Rows(0).Item("Computedfield")
            Pre = Pre.Replace(",", "','")
            da.SelectCommand.CommandText = "select Statusname from mmm_mst_workflow_status with(nolock) where eid=" & EID & " and documenttype='" & DocumentType & "' and Statusname in ('" & Pre & "') order by dord "
            'Else
            '   da.SelectCommand.CommandText = "select Statusname from mmm_mst_workflow_status where eid=" & EID & " and documenttype='" & DocumentType & "'  order by dord "
        End If
        If da.SelectCommand.CommandText.ToString.Length > 0 Then
            da.Fill(dt)
        End If
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                'Case when [V144Invoice_PO].curstatus='GRN / SRN'
                'aprstatus='" & dt.Rows(i).Item(0).ToString & "'
                SLAUNColumn = SLAUNColumn & ", Case when " & ViewName & ".curstatus='" & dt.Rows(i).Item(0).ToString & "' then (select top 1 u.username from mmm_doc_dtl dt with(nolock) inner join mmm_mst_user u with(nolock) on u.uid=dt.userid   where  DOCID=" & ViewName & ".[TID] and dt.tid=" & ViewName & ".[LastTID]) else (select top 1 u.username from mmm_doc_dtl dt with(nolock) inner join mmm_mst_user u with(nolock) on u.uid=dt.userid where aprstatus='" & dt.Rows(i).Item(0).ToString & "' and DOCID=" & ViewName & ".[TID]) end [" & dt.Rows(i).Item(0).ToString & " UserName]"
            Next
        End If
        If SLAUNColumn.Length > 1 Then
            SLADTCol = SLADTCol & SLAInDateQuery(EID, DocumentType, ps)
            SLADTCol = SLADTCol & SLAOutDateQuery(EID, DocumentType, ps)
            SLADTCol = SLADTCol & SLAUNColumn
        End If
        Return SLADTCol
    End Function

    Public Shared Function StaticFields(EID As Integer, DocumentType As String, ps As DataSet) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim Sfld As String = ""
        If ps.Tables(0).Rows(0).Item("FormType").ToString.ToUpper = "MASTER" Then
        Else
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            If ps.Tables(0).Rows(0).Item("LastWFS").ToString = "1" Then
                Sfld = Sfld & ", (select aprstatus from mmm_doc_dtl with(nolock) where tid in (select max(tid) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID] and aprstatus is not null)) [Last Work Flow Stage] "
            End If
            If ps.Tables(0).Rows(0).Item("LastUN").ToString = "1" Then
                Sfld = Sfld & ", (select username from mmm_mst_user with(nolock) where uid= (select top 1 userid from mmm_doc_dtl with(nolock) where tid in (select max(tid) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID] and aprstatus is not null))) [Last Approver Name] "
            End If
            If ps.Tables(0).Rows(0).Item("LastAD").ToString = "1" Then
                Sfld = Sfld & ", (select replace(convert(varchar(50),convert(date,max(tdate),3),106),' ','-') from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID])[Last Action Date]"
            End If
            If ps.Tables(0).Rows(0).Item("CurrentUser").ToString = "1" Then
                Sfld = Sfld & ", (select username from mmm_mst_user with(nolock) where uid=(select top 1 userid from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID] and TID=" & ViewName & ".[LastTID]))[Current User Name]"
            End If
            If ps.Tables(0).Rows(0).Item("CreationDateFlag").ToString = "1" Then
                Sfld = Sfld & ", convert(nvarchar, " & ViewName & ".ADATE) [Creation Date]"
            End If
        End If
        'Sfld = Sfld & ", (select username from mmm_mst_user where uid=(select top 1 userid from mmm_doc_dtl where docid=" & ViewName & ".[TID] and TID=" & ViewName & ".[LastTID]))[Current User Name],(select replace(convert(varchar(50),convert(date,max(tdate),3),106),' ','-') from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".[TID])[Last Action Date],(select username from mmm_mst_user where uid=(select top 1 ouid from mmm_mst_doc where tid=" & ViewName & ".[TID]))[Creator Name] "
        Return Sfld
    End Function
    Protected Shared Function GenearateQuery(data As Dictionary(Of String, Object), IsActionForm As Boolean) As DGrid
        Dim ret As String = ""
        Dim grid As New DGrid()
        Dim ds As New DataSet()
        Dim ps As New DataSet()
        ' Geiing all the field of Entity becouse all the field might be required
        Dim eid As Integer = HttpContext.Current.Session("eid")
        Dim DocumentType1 As String = doc1
        ps = GetTableData(DocumentType1)
        ds = GetAllFields(eid)
        'Dim BaseView1 As DataView
        'Dim BaseTable1 As DataTable
        Dim BaseView As DataView
        Dim BaseTable As DataTable
        Dim DocumentType As String = ""
        If ds.Tables(0).Rows.Count > 0 Then
            BaseView = ds.Tables(0).DefaultView
            'BaseView.RowFilter = "DocumentType='" & ds.Tables("flds").Rows & "'"

            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            BaseTable = BaseView.Table.DefaultView.ToTable()
            If IsActionForm = True Then
                DocumentType = ds.Tables(0).Rows(0).Item("EventName").ToString
                BaseView.RowFilter = "DocumentType='" & data("ddlDocType").ToString().Split("-")(1) & "'"
                BaseTable = BaseView.Table.DefaultView.ToTable()
            End If
            grid = GenearateQuery1(eid, data("ddlDocType").ToString().Split("-")(1), ds, ps, data)
            'Now find all object relation 
        End If
        Return grid
    End Function
    Public Shared Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, ps As DataSet, data As Dictionary(Of String, Object)) As DGrid
        Dim ret As String = ""
        Dim View As DataView
        Dim View1 As DataView
        Dim DCS As New DataSet
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim childStrJoinString As String = ""
        Dim cDoc As String = ""
        Dim DOCNew As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim pds As New DataSet
        Dim srch As String = ""
        Dim ct As Integer = 0
        Dim ViewNamech = ""
        Dim fld As String = ""
        Dim grid As New DGrid()
        Dim newQuery As String = ""
        Dim newQuery12 As String = ""

        Dim Diff As String = ""
        Dim ChildAddQuery As String = ""
        Dim QueryNew As String = ""
        Dim strError As String = ""
        Dim sdp As New DataSet
        Dim ut As String = ""
        Dim ChildVIEWNAME As String = ""
        Dim NewdataChild As New DataSet
        Dim NewJoinColumn As New DataSet
        Dim chview As String = ""
        Dim listofdoc As New ArrayList

        Dim strig As String = ""

        Dim SchemaString As String = DocumentType
        Try

            View = ps.Tables(0).DefaultView
            VIEW1 = ds.Tables(0).DefaultView
            VIEW1.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            Dim flTable As String = ViewName

            Dim chldview = "" & ps.Tables(0).Rows(0).Item("ChldDoctype").ToString.Replace(" ", "_") & ""
            Dim strdocfields() As String = ps.Tables(0).Rows(0).Item("Docfields").Split(",")
            Dim chlddtype() As String = chldview.Split(",")


            If ps.Tables(0).Rows(0).Item("ChldDoctype").ToString.Length > 0 Then
                For t As Integer = 0 To chlddtype.Length - 1
                    flTable = flTable & " left Outer join  V" & EID & "" & chlddtype(t).ToString & " on V" & EID & "" & chlddtype(t).ToString & ".Docid=" & ViewName & ".TID"
                Next
            End If
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                For j As Integer = 0 To strdocfields.Length - 1
                    Dim dtfield As String
                    dtfield = strdocfields(j).ToString
                    'New Query For Report
                    Dim Dtable As New DataTable
                    newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,DisplayName,Datatype,EID from mmm_mst_fields with(nolock) where FieldID=" & dtfield & " "
                    Dim predata As New DataSet
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    da.Fill(predata)
                    If (predata.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To predata.Tables(0).Rows.Count - 1
                            'Dtable.Columns.Add(pds.Tables(0).Rows(t).Item("displayName"))
                            'Dtable.AcceptChanges()
                            If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED" And predata.Tables(0).Rows(0).Item("FieldType") = "Drop Down") Then
                                StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("DisplayName") & "'"
                            ElseIf (predata.Tables(0).Rows(0).Item("FieldType") = "LookupDDL") Then
                                'StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & ViewName & ".[" & predata.Tables(0).Rows(0).Item("displayname").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("displayName") & "'"
                                StrColumn = StrColumn & ", dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "]" & ",'" & predata.Tables(0).Rows(i).Item("FieldMapping").ToString &
                                 "') AS [" & predata.Tables(0).Rows(0).Item("DisplayName") & "]"
                            ElseIf (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED" And predata.Tables(0).Rows(0).Item("FieldType") = "AutoComplete") Then
                                StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("DisplayName") & "'"
                            ElseIf (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE").ToString.ToUpper = "SESSION VALUED") Then
                                StrColumn = StrColumn & "," & "(select username from mmm_mst_user with(nolock) where uid=" & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "]) AS [" & predata.Tables(0).Rows(0).Item("DisplayName") & "]"
                            ElseIf pds.Tables(0).Rows(i).Item("Datatype").ToString.ToUpper = "DATETIME" Then
                                StrColumn = StrColumn & ",case when isdate(" & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "])=1 then convert(nvarchar,convert(date," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "])) else " & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "] End AS [" & predata.Tables(0).Rows(0).Item("DisplayName") & "]"
                            Else
                                StrColumn = StrColumn & "," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "] "
                            End If
                        Next
                    End If
                Next
            Next
            Dim SLACol As String = ""
            SLACol = SLAUseQuery(EID, DocumentType, ps)

            Dim sfls As String = ""
            sfls = StaticFields(EID, DocumentType, ps)

            Dim DiffTable As New DataTable
            Dim Differencequery As String = ""
            Dim DiffStatusquery As String = ""
            Dim Addcoulmn As String = ""
            Dim AddDatecolumn As String = ""
            Diff = "select First_Field,Second_Field,DisplayName,Type,DocumentType,TypeSecond_Field from mmm_rep_template_diff with(nolock) where DocumentType='" & ps.Tables(0).Rows(0).Item("DocumentType") & "' and RefTid='" & ps.Tables(0).Rows(0).Item("Tid") & "'"
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(Diff, con)
            da.Fill(DCS)
            If (DCS.Tables(0).Rows.Count > 0) Then
                For Ciff As Integer = 0 To DCS.Tables(0).Rows.Count - 1
                    If (DCS.Tables(0).Rows(Ciff).Item("Type") = "F") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "F") Then
                        Dim iDate As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDate As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        Differencequery = Differencequery & "," & " case when " & ViewName & ".[" & iDate & "] is null then '' when " & ViewName & ".[" & pDate & "] is null then '' else datediff(day ,convert(varchar(30) ,isnull(" & ViewName & ".[" & iDate & "],0),102) ,Convert(varchar(30) ,isnull(" & ViewName & ".[" & pDate & "],0),102)) end  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    ElseIf (DCS.Tables(0).Rows(Ciff).Item("Type") = "S") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "S") Then
                        Dim iDatestatus1 As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDatestatus2 As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        DiffStatusquery = DiffStatusquery & "," & "  datediff(day,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & iDatestatus1 & "') ,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & pDatestatus2 & "'))  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    ElseIf (DCS.Tables(0).Rows(Ciff).Item("Type") = "F") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "S") Then
                        Dim iDatestatus1 As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDatestatus2 As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        DiffStatusquery = DiffStatusquery & "," & " case when " & ViewName & ".[" & iDatestatus1 & "] is null then '' else  datediff(day,convert(date," & ViewName & ".[" & iDatestatus1 & "]) ,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & pDatestatus2 & "'))  end  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    ElseIf (DCS.Tables(0).Rows(Ciff).Item("Type") = "S") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "F") Then
                        Dim iDatestatus1 As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDatestatus2 As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        DiffStatusquery = DiffStatusquery & "," & " case when " & ViewName & ".[" & pDatestatus2 & "] is null then '' else  datediff(day ,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & iDatestatus1 & "'),convert(date," & ViewName & ".[" & pDatestatus2 & "]))  end  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    End If
                Next
                AddDatecolumn = AddDatecolumn & Differencequery
                Addcoulmn = Addcoulmn & DiffStatusquery
                StrColumn = StrColumn & AddDatecolumn & Addcoulmn
            End If
            Dim Query = ""
            'Dim Query = "Select distinct  " & ViewName & ".[TID] As [DocID] , " & StrColumn.Substring(1, StrColumn.Length - 1) & ", " & ViewName & ".[CurStatus] As [Current Status], Convert(nvarchar, " & ViewName & ".[adate], 3) As [Creation Date]" & " FROM " & ViewName & " With(nolock) " & StrJoinString
            If SLACol = "" Then
                If ps.Tables(0).Rows(0).Item("FormType").ToString.ToUpper = "MASTER" Then
                    If ps.Tables(0).Rows(0).Item("Documenttype").ToString.ToUpper = "USER" Then
                        Query = "Select  distinct " & ViewName & ".UserName," & ViewName & ".EmailID," & ViewName & ".UserRole," & ViewName & ".UserID," & StrColumn.Substring(1, StrColumn.Length - 1) & " " & sfls & " , " & ViewName & ".[UID] As [UID] FROM " & flTable '& ViewName & " With(nolock) " & StrJoinString & childStrJoinString
                    Else
                        Query = "Select  distinct " & StrColumn.Substring(1, StrColumn.Length - 1) & " " & sfls & " , " & ViewName & ".[TID] As [DocID] FROM " & flTable '& ViewName & " With(nolock) " & StrJoinString & childStrJoinString
                    End If
                Else
                    Query = "Select  distinct " & StrColumn.Substring(1, StrColumn.Length - 1) & "," & ViewName & ".[CurStatus]  As [Current Status] " & sfls & " , " & ViewName & ".[TID] As [DocID] FROM " & flTable '& ViewName & " With(nolock) " & StrJoinString & childStrJoinString
                End If
            Else
                Query = "Select   distinct " & StrColumn.Substring(1, StrColumn.Length - 1) & SLACol & "," & ViewName & ".[CurStatus]  As [Current Status] " & sfls & " , " & ViewName & ".[TID] As [DocID] FROM " & flTable '& ViewName & " With(nolock) " & StrJoinString & childStrJoinString
            End If
            da.SelectCommand.CommandText = "Select documenttype,dropdown,dropdowntype,datatype ,fieldtype ,FieldMapping,DisplayName from mmm_mst_fields with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and (documenttype='" & DocumentType.ToString & "') or (documenttype in ( select dropdown from mmm_mst_fields with(nolock) where documenttype='" & DocumentType.ToString & "' and fieldtype='child item' and isSearch=1 and eid=" & HttpContext.Current.Session("EID") & ") and fieldid in (" & ps.Tables(0).Rows(0).Item("Docfields") & ") and  displayname='" & data("ddlfields").ToString() & "' and eid=" & HttpContext.Current.Session("EID") & ")"
            Dim dt1 As New DataTable
            da.Fill(dt1)
            View = pds.Tables(0).DefaultView
            tblRe = View.Table.DefaultView.ToTable()
            Dim YES = tblRe.Rows(0).Item("Dropdown").ToString().Split("-")
            'Dim FieldMalling = YES(2)
            'Dim VAL = YES(1)
            Dim doctype As String
            Dim chrdoc() As String
            Dim rchdoc As String = ""
            Dim mstval As String = ""
            Dim Cserch As String = ""
            Dim IsUser As Boolean = False
            If pds.Tables(0).Rows.Count > 0 Then
                doctype = pds.Tables(0).Rows(0).Item(2).ToString
                mstval = pds.Tables(0).Rows(0).Item(3).ToString
                If mstval.ToUpper = "MASTER VALUED" Then
                    chrdoc = pds.Tables(0).Rows(0).Item(4).ToString.Split("-")
                    If Not chrdoc(1).ToString().ToUpper = "USER" Then
                        rchdoc = "[V" & EID & chrdoc(1).Replace(" ", "_") & "]"
                        'Query = Query & " where  DMS.udf_split('" & dt1.Rows(0).Item("Dropdown").ToString() & "'," & ViewName & ".[" & dt1.Rows(0).Item("displayname").ToString() & "]) "

                        If chrdoc(2).ToString().Contains("fld") Then
                            da.SelectCommand.CommandText = "select DisplayName from mmm_mst_fields with(nolock) where documenttype='" & chrdoc(1).ToString() & "' and fieldmapping='" & chrdoc(2).ToString() & "' and eid=" & HttpContext.Current.Session("EID") & ""
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
                        doctype = "[V" & EID & doctype.Replace(" ", "_") & "]"
                    End If
                ElseIf mstval.ToUpper = "SESSION VALUED" Then
                    IsUser = True

                Else
                    doctype = ViewName
                End If
            Else
                doctype = ViewName
            End If

            If pds.Tables(0).Rows.Count > 0 Then
                If pds.Tables(0).Rows(0).Item(7).ToString.ToUpper <> "DATETIME" Then
                    If srch = "" Then
                        If tblRe.Rows(0)("fieldtype").ToString.ToUpper = ("Lookupddl").ToUpper Then
                            Query = Query & " WHERE  dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & dt1.Rows(0).Item("DisplayName") & "]" & ",'" & dt1.Rows(0).Item("FieldMapping").ToString &
                            "')  like '%" & data("txtval").ToString() & "%' "
                            ct = ct + 1
                        Else
                            Query = " set dateformat dmy;" & Query & " where  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                            If (ps.Tables(0).Rows(0).Item("Filterfield2").ToString <> "0") Then
                                Dim csv As String = ""
                                csv = ps.Tables(0).Rows(0).Item("FilterfieldData2").ToString
                                csv = csv.Replace(",", "','")
                                Query = Query & " and " & ViewName & ".[" & ps.Tables(0).Rows(0).Item("Filterfield2") & "] in ('" & Convert.ToString(csv) & "')"
                            End If

                            If (ps.Tables(0).Rows(0).Item("Filterfield1").ToString.ToUpper <> "SELECT") Then
                                Dim fdv As String = ""
                                fdv = ps.Tables(0).Rows(0).Item("FilterfieldsData1").ToString
                                fdv = fdv.Replace(",", "','")
                                Query = Query & " and " & ViewName & ".[" & ps.Tables(0).Rows(0).Item("Filterfield1") & "] in ('" & Convert.ToString(fdv) & "')"
                            End If
                            Dim filterstatus As String = ""
                            ct = ct + 1
                        End If
                    Else
                        Query = Query & " WHERE " & srch & " like '%" & data("txtval").ToString() & "%'"
                        ct = ct + 1
                    End If
                ElseIf data("ddlfields").ToString() <> "-Select-" Then
                    Query = " set dateformat dmy;" & Query & " WHERE  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) >= '" & data("frdate").ToString() & "' and  convert(date," & doctype & ".[" & data("ddlfields").ToString() & "],3) <='" & data("todate").ToString() & "'"
                    ct = ct + 1
                End If
            Else
                If data("ddlfields").ToString() = "adate" Then
                    ct = ct + 1
                ElseIf data("txtval").ToString() <> "" Then
                    Query = Query & " WHERE " & doctype & ".[" & data("ddlfields").ToString() & "] like '%" & data("txtval").ToString() & "%' "
                    ct = ct + 1
                End If
            End If
            Dim AllowedRoles As String = ""
            AllowedRoles = ps.Tables(0).Rows(0).Item("AllowedRoles").ToString
            If AllowedRoles.ToString.ToString.Length > 0 Then
                AllowedRoles = AllowedRoles.Replace(",", "','")
            End If

            'da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user with(nolock) where uid=" & HttpContext.Current.Session("UID") & " and rolename in ('" & AllowedRoles & "')"
            da.SelectCommand.CommandText = "select count(*) from mmm_mst_role with(nolock) where eid=" & HttpContext.Current.Session("EID") & "  and rolename in ('" & AllowedRoles & "')"
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
                    ' If HttpContext.Current.Session("EID").ToString <> "49" And HttpContext.Current.Session("EID").ToString <> "62" And Convert.ToInt64(HttpContext.Current.Session("EID")) < 137 Then
                    If Convert.ToInt64(HttpContext.Current.Session("EID")) < 46 Then
                        da.SelectCommand.CommandText = "uspGetUserRightIDnew"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                        da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                        Query = Query & " or " & ViewName & ".ouid in (" & str & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                    Else
                        da.SelectCommand.CommandText = "uspGetRightOnDoc"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                        da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                        da.SelectCommand.Parameters.AddWithValue("@docType", DocumentType.ToString)
                        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                        If ct > 0 Then
                            'Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                            Query = Query & " " & str & " and (" & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & " )) "
                        Else
                            str = Replace(str, (Left(str, 6)), " Where ")
                            'Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                            Query = Query & " " & str & " and (" & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & " )) "
                        End If
                    End If
                Else
                    ' Query = Query & " or (" & ViewName & ".ouid in (" & HttpContext.Current.Session("UID") & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & "))"
                    Query = Query & " and " & ViewName & ".ouid in (" & HttpContext.Current.Session("UID") & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ") or " & ViewName & ".TID in ( select distinct " & ViewName & ".tid from " & ViewName & "  join mmm_doc_dtl dt with(nolock) on dt.docid=" & ViewName & ".tid  where userid=" & HttpContext.Current.Session("UID") & ") "
                End If
            End If
            Dim DiffTable1 As New DataSet
            Dim Differencequery1 As String = ""
            Dim DiffStatusquery1 As String = ""
            Dim Addcoulmn1 As String = ""
            Dim AddDatecolumn1 As String = ""

            Dim pre As String = ""
            If ps.Tables(0).Rows(0).Item("FilterStatus").ToString.Length > 6 Then
                pre = ps.Tables(0).Rows(0).Item("FilterStatus").ToString
                pre = pre.Replace(",", "','")
                pre = " and " & ViewName & ".curstatus in ('" & pre & "')"
            End If

            Dim Fixval As String = ""
            If ps.Tables(0).Rows(0).Item("FilterField3").ToString <> "Select" And ps.Tables(0).Rows(0).Item("FilterField3").ToString <> "" Then
                Fixval = ps.Tables(0).Rows(0).Item("FilterFieldData3").ToString
                Fixval = Fixval.Replace(",", "','")
                Fixval = " and " & ViewName & ".[" & ps.Tables(0).Rows(0).Item("FilterField3").ToString & "] in ('" & Fixval & "')"
            End If
            If ps.Tables(0).Rows(0).Item("Documenttype").ToString.ToUpper = "USER" Then
                Query = "set dateformat dmy; " & Query & " " & pre & " " & Fixval & " order by " & ViewName & ".UID desc"
            Else
                Query = "set dateformat dmy; " & Query & " " & pre & " " & Fixval & " order by " & ViewName & ".TID desc"
            End If

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
                grid.Message = "Data Not Found!"
                grid.Success = False
                grid.Count = 0
                'Return grid
                grid.Column.Clear()
            End If
        Catch ex As Exception
            grid = DynamicGrid.GridData(New DataTable(), "Error occured at server please contact your system administrator.")
        Finally
            con.Close()
            con.Dispose()
        End Try
        Return grid
    End Function
    Public Shared Function GetTableData(DocumentType1 As String) As DataSet
        Dim ps As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim Query = "SET NOCOUNT ON;select f.Temp_Name,f.Tid,f.DocumentType,f.Docfields,f.Computedfield,f.SortBy,f.filterfield1,f.filterfieldsData1,f.filterfield2,f.FilterfieldData2," &
                        "f.FilterStatus,f.AllowedRoles,f.ChldDoctype,f.StagesInDate,f.LastWFS,f.LastUN,f.lastAD,f.CurrentUser,f.CreationDateFlag,f.FormType,f.filterfield3,f.filterfielddata3 from mmm_rep_template_config f JOIN MMM_MST_FORMS " &
                        "FF ON (FF.FormName=F.DocumentType) where Temp_Name='" & ReportName & "' and DocumentType='" & DocumentType1 & "' and ff.EID='" & HttpContext.Current.Session("eid") & "'"
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ps)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ps
    End Function
    Public Shared Function GetAllFields(EID As Integer) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE," & _
                                    "F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype " & _
                                     "FROM MMM_MST_FIELDS F with(nolock) INNER JOIN MMM_MST_FORMS FF with(nolock) ON FF.FormName=F.DocumentType " & _
                                   " WHERE F.EID= " & EID & " AND FF.EID= " & EID & "  " & _
                                    "and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number') order by f.DocDtlDisplayOrder;"
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

    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim doctype As String = ddlDocType.SelectedValue.Split("-")(1)
        FormType = ddlDocType.SelectedValue.Split("-")(2)
        Dim dt As New DataTable
        da.SelectCommand.CommandText = " select top 1 DateFilterFields from mmm_rep_template_config with(nolock) where eid=" & Session("EID") & " and Temp_Description='" & ddlDocType.SelectedItem.ToString & "'"
        da.Fill(dt)
        Dim Dtfilter As String = ""
        If dt.Rows.Count > 0 Then
            Dtfilter = dt.Rows(0).Item(0).ToString.Replace(",", "','")
        End If
        If FormType.ToUpper = "MASTER" Then
            If doc1.ToUpper = "USER" Then
                da.SelectCommand.CommandText = "select 'lastupdate'[DispText],'Creation Date' [DispVal]"
            Else
                da.SelectCommand.CommandText = "select 'UpdatedDate'[DispText],'Creation Date' [DispVal] union select DisplayName,Displayname from mmm_mst_fields with(nolock) where eid=" & Session("EID") & " and documenttype='" & doctype & "' and fieldmapping  in ('" & Dtfilter & "')"
            End If
        Else
            da.SelectCommand.CommandText = "select 'adate'[DispText],'Creation Date' [DispVal] union select DisplayName,Displayname from mmm_mst_fields with(nolock) where eid=" & Session("EID") & " and documenttype='" & doctype & "' and fieldmapping  in ('" & Dtfilter & "')"
        End If
        Try
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlfields.DataSource = ds
            ddlfields.DataTextField = "DispVal"
            ddlfields.DataValueField = "DispText"
            ddlfields.DataBind()
            ddlfields.Items.Insert("0", "-Select-")
            txtsdate.Text = ""
            txtedate.Text = ""
            'ReportName = txtreportName.Text
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select datatype,fieldmapping,FieldID from mmm_mst_fields with(nolock) where eid='" & Session("EID") & "' and (documenttype='" & ddlDocType.SelectedValue.Split("-")(1) & "' and displayname='" & ddlfields.SelectedItem.Text.ToString & "') ", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Try
            txtsdate.Text = ""
            txtedate.Text = ""
            ' ReportName = txtreportName.Text
            '   gvPending.Controls.Clear()
            If ddlfields.SelectedValue.ToString = "-Select-" Then
                pnl1.Visible = False
            End If
            If ddlfields.SelectedValue.ToString = "adate" Or ddlfields.SelectedValue.ToString = "UpdatedDate" Or ddlfields.SelectedValue.ToString = "lastupdate" Then
                pnl1.Visible = True
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
                pnl1.Visible = False
                ' txtval.Visible = False
            ElseIf ddlfields.SelectedValue.ToString = "tid" Then
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")
                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")
                pnl1.Visible = False
            End If
            Dim datatype As String = ds.Tables("data").Rows(0).Item("datatype").ToString()
            If datatype = "Datetime" Then
                pnl1.Visible = True
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

            Else
                ddlDocType.Attributes.Add("IsSearch", "1")
                ddlDocType.Attributes.Add("data-ty", "DDL")
                ddlDocType.Attributes.Add("fld", "ddlDocType")

                ddlfields.Attributes.Add("IsSearch", "1")
                ddlfields.Attributes.Add("data-ty", "DDL")
                ddlfields.Attributes.Add("fld", "ddlfields")
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
    Public Function GenearateQueryExl(EID As Integer, DocumentType As String, ps As DataSet, ds As DataSet) As DataTable
        Dim ret As String = ""
        Dim View As DataView
        Dim View1 As DataView
        Dim DCS As New DataSet
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim childStrJoinString As String = ""
        Dim cDoc As String = ""
        Dim DOCNew As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim pds As New DataSet
        Dim srch As String = ""
        Dim ct As Integer = 0
        Dim ViewNamech = ""
        Dim fld As String = ""
        Dim grid As New DGrid()
        Dim newQuery As String = ""
        Dim newQuery12 As String = ""
        Dim Diff As String = ""
        Dim ChildAddQuery As String = ""
        Dim QueryNew As String = ""
        Dim strError As String = ""
        Dim sdp As New DataSet
        Dim ut As String = ""
        Dim ChildVIEWNAME As String = ""
        Dim NewdataChild As New DataSet
        Dim NewJoinColumn As New DataSet
        Dim chview As String = ""
        Dim listofdoc As New ArrayList

        Dim strig As String = ""

        Dim SchemaString As String = DocumentType
        Try

            View = ps.Tables(0).DefaultView
            View1 = ds.Tables(0).DefaultView
            View1.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            Dim chldview = "" & ps.Tables(0).Rows(0).Item("ChldDoctype").ToString.Replace(" ", "_") & ""

            Dim strdocfields() As String = ps.Tables(0).Rows(0).Item("Docfields").Split(",")
            Dim chlddtype() As String = chldview.Split(",")
            Dim flTable As String = ViewName

            If ps.Tables(0).Rows(0).Item("ChldDoctype").ToString.Length > 0 Then
                For t As Integer = 0 To chlddtype.Length - 1
                    flTable = flTable & " left Outer join  V" & EID & "" & chlddtype(t).ToString & " on V" & EID & "" & chlddtype(t).ToString & ".Docid=" & ViewName & ".TID"
                Next
            End If

            Dim ddlDocType = ""

            For i As Integer = 0 To tbl.Rows.Count - 1
                For j As Integer = 0 To strdocfields.Length - 1
                    Dim dtfield As String
                    dtfield = strdocfields(j).ToString
                    'New Query For Report
                    Dim Dtable As New DataTable
                    newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype,EID from mmm_mst_fields with(nolock) where FieldID=" & dtfield & " "
                    Dim predata As New DataSet
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    da.Fill(predata)
                    If (predata.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To predata.Tables(0).Rows.Count - 1
                            'Dtable.Columns.Add(pds.Tables(0).Rows(t).Item("displayName"))
                            'Dtable.AcceptChanges()
                            If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED" And predata.Tables(0).Rows(0).Item("FieldType") = "Drop Down") Then
                                StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayname").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("displayName") & "'"
                            ElseIf (predata.Tables(0).Rows(0).Item("FieldType") = "LookupDDL") Then
                                'StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & ViewName & ".[" & predata.Tables(0).Rows(0).Item("displayname").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("displayName") & "'"
                                StrColumn = StrColumn & ", dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName") & "]" & ",'" & predata.Tables(0).Rows(i).Item("FieldMapping").ToString &
                                 "') AS [" & predata.Tables(0).Rows(0).Item("DisplayName") & "]"
                            ElseIf (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED" And predata.Tables(0).Rows(0).Item("FieldType") = "AutoComplete") Then
                                StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("DisplayName").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("DisplayName") & "'"
                            ElseIf (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED" And predata.Tables(0).Rows(0).Item("FieldType") = "Auto Number") Then
                                StrColumn = StrColumn & "," & "" & " DMS.udf_split('" & predata.Tables(0).Rows(0).Item("Dropdown").ToString() & "'," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayname").ToString() & "]) As '" & predata.Tables(0).Rows(0).Item("displayName") & "'"
                            ElseIf (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE").ToString.ToUpper = "SESSION VALUED") Then
                                StrColumn = StrColumn & "," & "(select username from mmm_mst_user with(nolock) where uid=" & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayname").ToString() & "]) AS [" & predata.Tables(0).Rows(0).Item("displayname").ToString() & "]"
                            ElseIf pds.Tables(0).Rows(i).Item("Datatype").ToString.ToUpper = "DATETIME" Then
                                StrColumn = StrColumn & ",case when isdate(" & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayName") & "])=1 then convert(nvarchar,convert(date," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayName") & "])) else " & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayName") & "] End AS [" & predata.Tables(0).Rows(0).Item("displayName") & "]"
                            Else
                                StrColumn = StrColumn & "," & "V" & predata.Tables(0).Rows(0).Item("EID") & Replace(predata.Tables(0).Rows(0).Item("documenttype"), " ", "_") & ".[" & predata.Tables(0).Rows(0).Item("displayName") & "] "
                            End If
                        Next
                    End If
                Next
            Next
            Dim SLACol As String = ""
            SLACol = SLAUseQuery(EID, DocumentType, ps)
            Dim sfls As String = ""
            sfls = StaticFields(EID, DocumentType, ps)

            Dim DiffTable As New DataTable
            Dim Differencequery As String = ""
            Dim DiffStatusquery As String = ""
            Dim Addcoulmn As String = ""
            Dim AddDatecolumn As String = ""
            Diff = "select First_Field,Second_Field,DisplayName,Type,DocumentType,TypeSecond_Field from mmm_rep_template_diff with(nolock) where DocumentType='" & ps.Tables(0).Rows(0).Item("DocumentType") & "' and RefTid='" & ps.Tables(0).Rows(0).Item("Tid") & "'"
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(Diff, con)
            da.Fill(DCS)
            If (DCS.Tables(0).Rows.Count > 0) Then
                For Ciff As Integer = 0 To DCS.Tables(0).Rows.Count - 1
                    If (DCS.Tables(0).Rows(Ciff).Item("Type") = "F") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "F") Then
                        Dim iDate As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDate As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        Differencequery = Differencequery & "," & " case when " & ViewName & ".[" & iDate & "] is null then '' when " & ViewName & ".[" & pDate & "] is null then '' else datediff(day ,convert(varchar(30) ,isnull(" & ViewName & ".[" & iDate & "],0),102) ,Convert(varchar(30) ,isnull(" & ViewName & ".[" & pDate & "],0),102)) end  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    ElseIf (DCS.Tables(0).Rows(Ciff).Item("Type") = "S") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "S") Then
                        Dim iDatestatus1 As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDatestatus2 As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        DiffStatusquery = DiffStatusquery & "," & "  datediff(day,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & iDatestatus1 & "') ,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & pDatestatus2 & "'))  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    ElseIf (DCS.Tables(0).Rows(Ciff).Item("Type") = "F") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "S") Then
                        Dim iDatestatus1 As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDatestatus2 As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        DiffStatusquery = DiffStatusquery & "," & " case when " & ViewName & ".[" & iDatestatus1 & "] is null then '' else  datediff(day,convert(date," & ViewName & ".[" & iDatestatus1 & "]) ,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & pDatestatus2 & "'))  end  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    ElseIf (DCS.Tables(0).Rows(Ciff).Item("Type") = "S") And (DCS.Tables(0).Rows(Ciff).Item("TypeSecond_Field") = "F") Then
                        Dim iDatestatus1 As String = DCS.Tables(0).Rows(Ciff).Item("First_Field")
                        Dim pDatestatus2 As String = DCS.Tables(0).Rows(Ciff).Item("Second_Field")
                        DiffStatusquery = DiffStatusquery & "," & " case when " & ViewName & ".[" & pDatestatus2 & "] is null then '' else  datediff(day ,(select max(tdate) from mmm_doc_dtl with(nolock) where docid=" & ViewName & ".tid and aprstatus='" & iDatestatus1 & "'),convert(date," & ViewName & ".[" & pDatestatus2 & "]))  end  as '" & DCS.Tables(0).Rows(Ciff).Item("DisplayName") & "'"
                    End If
                Next
                AddDatecolumn = AddDatecolumn & Differencequery
                Addcoulmn = Addcoulmn & DiffStatusquery
                StrColumn = StrColumn & AddDatecolumn & Addcoulmn
            End If
            Dim Query = ""
            'Dim Query = "Select distinct  " & ViewName & ".[TID] As [DocID] , " & StrColumn.Substring(1, StrColumn.Length - 1) & ", " & ViewName & ".[CurStatus] As [Current Status], Convert(nvarchar, " & ViewName & ".[adate], 3) As [Creation Date]" & " FROM " & ViewName & " With(nolock) " & StrJoinString
            If SLACol = "" Then
                Query = "Select  Distinct" & StrColumn.Substring(1, StrColumn.Length - 1) & sfls & ViewName & ".[CurStatus]  As [Current Status],convert(nvarchar,convert(varchar," & ViewName & ".[adate],120)) As [Creation Date]" & " , " & ViewName & ".[TID] As [DocID] FROM " & flTable '& ViewName & " With(nolock) " & StrJoinString & childStrJoinString
            Else
                Query = "Select  Distinct" & StrColumn.Substring(1, StrColumn.Length - 1) & sfls & SLACol & "," & ViewName & ".[CurStatus]  As [Current Status],convert(nvarchar,convert(varchar," & ViewName & ".[adate],120)) As [Creation Date]" & " , " & ViewName & ".[TID] As [DocID] FROM " & flTable '& ViewName & " With(nolock) " & StrJoinString & childStrJoinString
            End If

            
            da.SelectCommand.CommandText = "Select documenttype,dropdown,dropdowntype,datatype ,fieldtype ,FieldMapping,DisplayName from mmm_mst_fields with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and (documenttype='" & DocumentType.ToString & "') or (documenttype in ( select dropdown from mmm_mst_fields with(nolock) where documenttype='" & DocumentType.ToString & "' and fieldtype='child item' and isSearch=1 and eid=" & HttpContext.Current.Session("EID") & ") and fieldid in (" & ps.Tables(0).Rows(0).Item("Docfields") & ") and  displayname='" & ddlfields.SelectedValue.ToString() & "' and eid=" & HttpContext.Current.Session("EID") & ")"
            Dim dt1 As New DataTable
            da.Fill(dt1)
            View = pds.Tables(0).DefaultView
            tblRe = View.Table.DefaultView.ToTable()
            Dim YES = tblRe.Rows(0).Item("Dropdown").ToString().Split("-")
            'Dim FieldMalling = YES(2)
            'Dim VAL = YES(1)
            Dim doctype As String
            Dim chrdoc() As String
            Dim rchdoc As String = ""
            Dim mstval As String = ""
            Dim Cserch As String = ""
            Dim IsUser As Boolean = False
            If pds.Tables(0).Rows.Count > 0 Then
                doctype = pds.Tables(0).Rows(0).Item(2).ToString
                mstval = pds.Tables(0).Rows(0).Item(3).ToString
                If mstval.ToUpper = "MASTER VALUED" Then
                    chrdoc = pds.Tables(0).Rows(0).Item(4).ToString.Split("-")
                    If Not chrdoc(1).ToString().ToUpper = "USER" Then
                        rchdoc = "[V" & EID & chrdoc(1).Replace(" ", "_") & "]"
                        'Query = Query & " where  DMS.udf_split('" & dt1.Rows(0).Item("Dropdown").ToString() & "'," & ViewName & ".[" & dt1.Rows(0).Item("displayname").ToString() & "]) "

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
                        doctype = "[V" & EID & doctype.Replace(" ", "_") & "]"
                    End If
                ElseIf mstval.ToUpper = "SESSION VALUED" Then
                    IsUser = True

                Else
                    doctype = ViewName
                End If
            Else
                doctype = ViewName
            End If

            If pds.Tables(0).Rows.Count > 0 Then
                If pds.Tables(0).Rows(0).Item(7).ToString.ToUpper <> "DATETIME" Then
                    If srch = "" Then
                        If tblRe.Rows(0)("fieldtype").ToString.ToUpper = ("Lookupddl").ToUpper Then
                            Query = Query & " WHERE  dms.Get_LookupddlValue(" & HttpContext.Current.Session("EID") & ",'" & DocumentType & "', " & ViewName & ".[" & dt1.Rows(0).Item("DisplayName") & "]" & ",'" & dt1.Rows(0).Item("FieldMapping").ToString &
                            "')  like '%" & txtsdate.Text.ToString() & "%' "
                            ct = ct + 1
                        Else
                            Query = " set dateformat dmy;" & Query & " where  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString() & "],3) >= '" & txtsdate.Text.ToString & "' and  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString() & "],3) <='" & txtedate.Text.ToString & "'"
                            If (ps.Tables(0).Rows(0).Item("Filterfield2") <> "0") Then
                                Dim csv As String = ""
                                csv = ps.Tables(0).Rows(0).Item("FilterfieldData2")
                                csv = csv.Replace(",", "','")
                                Query = Query & " and " & ViewName & ".[" & ps.Tables(0).Rows(0).Item("Filterfield2") & "] in ('" & Convert.ToString(csv) & "')"
                            End If

                            If (ps.Tables(0).Rows(0).Item("Filterfield1").ToString.ToUpper <> "SELECT") Then
                                Dim fdv As String = ""
                                fdv = ps.Tables(0).Rows(0).Item("FilterfieldsData1")
                                fdv = fdv.Replace(",", "','")
                                Query = Query & " and " & ViewName & ".[" & ps.Tables(0).Rows(0).Item("Filterfield1") & "] in ('" & Convert.ToString(fdv) & "')"
                            End If
                            Dim filterstatus As String = ""
                            ct = ct + 1
                        End If
                    Else
                        Query = Query & " WHERE " & srch & " like '%" & txtsdate.Text.ToString() & "%'"
                        ct = ct + 1
                    End If
                ElseIf ddlfields.SelectedValue.ToString() <> "-Select-" Then
                    Query = " set dateformat dmy;" & Query & " WHERE  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString() & "],3) >= '" & txtsdate.Text.ToString & "' and  convert(date," & doctype & ".[" & ddlfields.SelectedValue.ToString() & "],3) <='" & txtedate.Text.ToString() & "'"
                    ct = ct + 1
                End If
            Else
                If ddlfields.SelectedValue.ToString() = "adate" Then
                    ct = ct + 1
                ElseIf ddlfields.SelectedValue.ToString() <> "" Then
                    Query = Query & " WHERE " & doctype & ".[" & ddlfields.SelectedValue.ToString() & "] like '%" & txtsdate.Text & "%' "
                    ct = ct + 1
                End If
            End If
            Dim AllowedRoles As String = ""
            AllowedRoles = IsDBNull(ps.Tables(0).Rows(0).Item("AllowedRoles")).ToString
            If AllowedRoles.ToString.ToString.Length > 0 Then
                AllowedRoles = AllowedRoles.Replace(",", "','")
            End If

            'da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user with(nolock) where uid=" & HttpContext.Current.Session("UID") & " and rolename in ('" & AllowedRoles & "')"
            da.SelectCommand.CommandText = "select count(*) from mmm_mst_role with(nolock) where eid=" & HttpContext.Current.Session("EID") & "  and rolename in ('" & AllowedRoles & "')"
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
                    If HttpContext.Current.Session("EID").ToString <> "49" And HttpContext.Current.Session("EID").ToString <> "62" And HttpContext.Current.Session("EID").ToString <> "137" And HttpContext.Current.Session("EID").ToString <> "138" And HttpContext.Current.Session("EID").ToString <> "46" Then
                        da.SelectCommand.CommandText = "uspGetUserRightIDnew"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                        da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                        Query = Query & " or " & ViewName & ".ouid in (" & str & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                    Else
                        da.SelectCommand.CommandText = "uspGetRightOnDoc"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@UID", HttpContext.Current.Session("UID"))
                        da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@rolename", HttpContext.Current.Session("USERROLE"))
                        da.SelectCommand.Parameters.AddWithValue("@docType", DocumentType.ToString)
                        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
                        If ct > 0 Then
                            Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                        Else
                            str = Replace(str, (Left(str, 6)), " Where ")
                            Query = Query & " " & str & " or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                        End If
                    End If
                Else
                    Query = Query & " or " & ViewName & ".ouid in (" & HttpContext.Current.Session("UID") & ") or " & ViewName & ".ouid in(" & HttpContext.Current.Session("SUBUID") & ") or " & ViewName & ".TID in ( " & doc & ")"
                End If
            End If
            Dim DiffTable1 As New DataSet
            Dim Differencequery1 As String = ""
            Dim DiffStatusquery1 As String = ""
            Dim Addcoulmn1 As String = ""
            Dim AddDatecolumn1 As String = ""

            Dim pre As String = ""
            If ps.Tables(0).Rows(0).Item("FilterStatus").ToString.Length > 6 Then
                pre = ps.Tables(0).Rows(0).Item("FilterStatus")
                pre = pre.Replace(",", "','")
                pre = " and " & ViewName & ".curstatus in ('" & pre & "')"
            End If
            Dim Fixval As String = ""
            If ps.Tables(0).Rows(0).Item("FilterField3").ToString <> "Select" And ps.Tables(0).Rows(0).Item("FilterField3").ToString <> "" Then
                Fixval = ps.Tables(0).Rows(0).Item("FilterFieldData3").ToString
                Fixval = Fixval.Replace(",", "','")
                Fixval = " and " & ViewName & ".[" & ps.Tables(0).Rows(0).Item("FilterField3").ToString & "] in ('" & Fixval & "')"
            End If

            If ps.Tables(0).Rows(0).Item("Documenttype").ToString.ToUpper = "USER" Then
                Query = "set dateformat dmy; " & Query & " " & pre & " " & Fixval & " order by " & ViewName & ".UID desc"
            Else
                Query = "set dateformat dmy; " & Query & " " & pre & " " & Fixval & " order by " & ViewName & ".TID desc"
            End If

            'Query = "set dateformat dmy; " & Query & " " & pre & " order by " & ViewName & ".TID desc"
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

        Catch ex As Exception
            ex.ToString()
        Finally
            con.Close()
            con.Dispose()
        End Try
        Return dt
    End Function
    Protected Sub btnViewInExcel_Click(sender As Object, e As ImageClickEventArgs)
        Response.ClearContent()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataTable
        Dim ds1 As New DataSet
        Dim ds2 As New DataSet
        Dim dt As New DataTable
        Dim gridview1 As New GridView
        ' Dim DocumentType As String
        'ds1 = GetAllFields(Session("EID"))
        ds1 = GetTableData(ddlDocType.SelectedValue.Split("-")(1))
        ds2 = GetAllFields(Session("EID"))
        'DocumentType = ds1.Tables(0).Rows(0).Item("EventName").ToString
        'gridview1.DataSource = ViewState("grd")
        dt = GenearateQueryExl(Session("EID"), ddlDocType.SelectedValue.ToString.Split("-")(1), ds1, ds2)
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
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
End Class
