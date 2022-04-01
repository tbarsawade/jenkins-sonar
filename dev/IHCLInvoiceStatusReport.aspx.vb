Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Partial Class IHCLInvoiceStatusReport
    Inherits System.Web.UI.Page
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
            'pnl1.Visible = False
            ' txtval.Visible = False
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Try
                'Dim doc As String = ""
                'da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & ""
               
                'Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                Dim ds As New DataSet
                Dim dept1 As String = ""
                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                    da.SelectCommand.CommandText = "select TID,fld2[Dept] from mmm_mst_master where eid=" & Session("EID") & " and documenttype='Department master' order by fld2"
                Else
                    da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & Session("EID") & "  and UID=" & HttpContext.Current.Session("UID") & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    dept1 = da.SelectCommand.ExecuteScalar().ToString
                    Session("SubDept") = dept1
                    da.SelectCommand.CommandText = "select distinct fld25[TID],dms.udf_split('MASTER-Department Master-fld2',fld25)[Dept] from mmm_mst_master where eid=" & Session("EID") & " and documenttype='sub department' and tid in (" & dept1 & ")  order by Dept"
                End If
                '' 
                da.Fill(ds, "data")
                'ddlDocType.DataSource = ds
                'ddlDocType.DataTextField = "ReportName"
                'ddlDocType.DataValueField = "ReportDescription"
                'ddlDocType.DataBind()
                'ddlDocType.Items.Insert(0, "--Please Select--")
                lstdept.DataSource = ds.Tables("data")
                lstdept.DataTextField = "Dept"
                lstdept.DataValueField = "TID"
                lstdept.DataBind()
                da.SelectCommand.CommandText = "select TID,fld2 from mmm_mst_master where eid=" & Session("EID") & " and documenttype='Vendor Master' order by fld2 "
                da.Fill(ds, "data1")
                ddlvendor.DataSource = ds.Tables("data1")
                ddlvendor.DataTextField = "fld2"
                ddlvendor.DataValueField = "TID"
                ddlvendor.DataBind()
                ddlvendor.Items.Insert(0, "All")

                Dim selectedValues As String = String.Empty
                For Each li As ListItem In lstdept.Items
                    li.Selected = True
                    selectedValues = selectedValues & li.Value & ","
                Next

                If selectedValues.ToString <> "" Then
                    selectedValues = selectedValues.ToString.Remove(selectedValues.ToString.LastIndexOf(","), 1)
                End If
                Session("Dept") = selectedValues

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
        Else

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
    'Protected Sub lstdept_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstdept.SelectedIndexChanged
    '    Dim selectedValues As String = String.Empty
    '    For Each li As ListItem In lstdept.Items
    '        If li.Selected = True Then
    '            If selectedValues.ToString = "" Then
    '                selectedValues = selectedValues & li.Value & ","
    '            Else
    '                selectedValues = selectedValues & li.Value & ","
    '            End If
    '        End If
    '    Next
    '    If selectedValues.ToString <> "" Then
    '        'selectedValues = selectedValues.Remove(selectedValues.Length - 1, 1)
    '        'selectedValues = selectedValues.ToString.Remove(selectedValues.ToString.LastIndexOf("'"), 1)
    '        selectedValues = selectedValues.ToString.Remove(selectedValues.ToString.LastIndexOf(","), 1)
    '    End If
    '    Session("Val") = selectedValues
    'End Sub
    Protected Shared Function GenearateQuery(sdate As String, edate As String, vendor As String, status As String, dept As String) As DGrid
        Dim ret As String = ""
        Dim grid As New DGrid()
        Dim ds As New DataSet()
        Dim dt As New DataTable

        'Dim str As String = ""
        ' Geiing all the field of Entity becouse all the field might be required
        Dim eid As Integer = HttpContext.Current.Session("eid")
        ' ds = GetAllFields(eid)
        If dept <> "" Then
            dept = dept.ToString.Remove(dept.ToString.LastIndexOf(","), 1)
        End If
        HttpContext.Current.Session("Sdate") = sdate
        HttpContext.Current.Session("Edate") = edate
        HttpContext.Current.Session("Status") = status
        HttpContext.Current.Session("Dept") = dept
        HttpContext.Current.Session("Vendor") = vendor

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & eid & "  and Reportid=1694 "
        Else
            da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & eid & "  and Reportid=1694 "
        End If

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
        Dim Dept1 As String = ""
        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & eid & "  and UID=" & HttpContext.Current.Session("UID") & " "
            Dept1 = da.SelectCommand.ExecuteScalar().ToString
            If Dept1 = "" Then
                Exit Function
            End If
        End If

        If HttpContext.Current.Session("Dept").ToString <> "" Then
            str = Replace(str, "@Dept1", " fld65 in  (" & dept & ")")
            str = Replace(str, "@Dept2", " fld3 in  (" & dept & ")")
            str = Replace(str, "@Dept3", " fld21 in  (" & dept & ")")
            str = Replace(str, "@Dept4", " fld19 in  (" & dept & ")")
        End If
        If sdate.ToString <> "" Then
            str = Replace(str, "@IRDate1", "convert(varchar(50),convert(date,fld18,3),120)>='" & sdate.ToString() & "' and convert(varchar(50),convert(date,fld18,3),120)<='" & edate.ToString() & "'")
            str = Replace(str, "@IRDate2", "convert(varchar(50),convert(date,fld14,3),120)>='" & sdate.ToString() & "' and convert(varchar(50),convert(date,fld14,3),120)<='" & edate.ToString() & "'")
            str = Replace(str, "@IRDate3", "convert(varchar(50),convert(date,fld4,3),120)>='" & sdate.ToString() & "' and convert(varchar(50),convert(date,fld4,3),120)<='" & edate.ToString() & "'")
            str = Replace(str, "@IRDate4", "convert(varchar(50),convert(date,fld24,3),120)>='" & sdate.ToString() & "' and convert(varchar(50),convert(date,fld24,3),120)<='" & edate.ToString() & "'")
        End If
        If status.ToString = "0" Then
            str = Replace(str, "@status", " curstatus=curstatus")
        ElseIf status.ToString = "1" Then
            str = Replace(str, "@status", " curstatus='Archive'")
        Else
            str = Replace(str, "@status", " curstatus not in ('Archive')")
        End If
        If vendor.ToString = "0" Then
            str = Replace(str, "@vendor1", " fld5=fld5")
            str = Replace(str, "@vendor2", " fld5=fld5")
            str = Replace(str, "@vendor3", " fld5=fld5")
            str = Replace(str, "@vendor4", " fld3=fld3")
        ElseIf vendor.ToString = "All" Then
            str = Replace(str, "@vendor1", " fld5=fld5")
            str = Replace(str, "@vendor2", " fld5=fld5")
            str = Replace(str, "@vendor3", " fld5=fld5")
            str = Replace(str, "@vendor4", " fld3=fld3")
        Else
            str = Replace(str, "@vendor1", " fld5='" & vendor & "'")
            str = Replace(str, "@vendor2", " fld5='" & vendor & "'")
            str = Replace(str, "@vendor3", " fld5='" & vendor & "'")
            str = Replace(str, "@vendor4", " fld3='" & vendor & "'")
        End If

        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            str = Replace(str, "@rolePO", " fld79 in (" & Dept1 & ")")
            str = Replace(str, "@roleNonPO", " fld2 in (" & Dept1 & ")")
            str = Replace(str, "@roleHold", " fld18 in (" & Dept1 & ")")
            str = Replace(str, "@roleVInvoice", " fld20 in (" & Dept1 & ")")
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

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
    End Sub
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
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>Invoice Status Report</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=InvoiceStatusReport.xls")
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
    Public Function GenearateQueryExl(eid As Integer) As DataTable
        Try
            Dim ret As String = ""
            Dim grid As New DGrid()
            Dim ds As New DataSet()
            Dim dt As New DataTable

            'Dim str As String = ""
            ' Geiing all the field of Entity becouse all the field might be required
            ' ds = GetAllFields(eid)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)

            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & eid & "  and Reportid=1694 "
            Else
                da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where eid=" & eid & "  and Reportid=1694 "
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

            If HttpContext.Current.Session("Dept").ToString <> "" Then
                str = Replace(str, "@Dept1", " fld65 in  (" & HttpContext.Current.Session("Dept").ToString() & ")")
                str = Replace(str, "@Dept2", " fld3 in  (" & HttpContext.Current.Session("Dept").ToString() & ")")
                str = Replace(str, "@Dept3", " fld21 in  (" & HttpContext.Current.Session("Dept").ToString() & ")")
                str = Replace(str, "@Dept4", " fld19 in  (" & HttpContext.Current.Session("Dept").ToString() & ")")
            End If
            If txtsdate.ToString <> "" Then
                str = Replace(str, "@IRDate1", "convert(varchar(50),convert(date,fld18,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld18,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@IRDate2", "convert(varchar(50),convert(date,fld14,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld14,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@IRDate3", "convert(varchar(50),convert(date,fld4,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld4,3),120)<='" & txtedate.Text.ToString() & "'")
                str = Replace(str, "@IRDate4", "convert(varchar(50),convert(date,fld24,3),120)>='" & txtsdate.Text.ToString() & "' and convert(varchar(50),convert(date,fld24,3),120)<='" & txtedate.Text.ToString() & "'")
            End If
            If ddlstatus.SelectedValue.ToString = "0" Then
                str = Replace(str, "@status", " curstatus=curstatus")
            ElseIf ddlstatus.SelectedValue.ToString = "1" Then
                str = Replace(str, "@status", " curstatus='Archive'")
            Else
                str = Replace(str, "@status", " curstatus not in ('Archive')")
            End If

            If Session("Vendor") = "All" Then
                str = Replace(str, "@vendor1", " fld5=fld5")
                str = Replace(str, "@vendor2", " fld5=fld5")
                str = Replace(str, "@vendor3", " fld5=fld5")
                str = Replace(str, "@vendor4", " fld3=fld3")
            ElseIf Session("Vendor") = "0" Then
                str = Replace(str, "@vendor1", " fld5=fld5")
                str = Replace(str, "@vendor2", " fld5=fld5")
                str = Replace(str, "@vendor3", " fld5=fld5")
                str = Replace(str, "@vendor4", " fld3=fld3")
            Else
                str = Replace(str, "@vendor1", " fld5='" & Session("Vendor") & "'")
                str = Replace(str, "@vendor2", " fld5='" & Session("Vendor") & "'")
                str = Replace(str, "@vendor3", " fld5='" & Session("Vendor") & "'")
                str = Replace(str, "@vendor4", " fld3='" & Session("Vendor") & "'")
            End If

            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
            Else
                str = Replace(str, "@rolePO", " fld79 in (" & Dept & ")")
                str = Replace(str, "@roleNonPO", " fld2 in (" & Dept & ")")
                str = Replace(str, "@roleHold", " fld18 in (" & Dept & ")")
                str = Replace(str, "@roleVInvoice", " fld20 in (" & Dept & ")")
            End If


            da.SelectCommand.CommandText = str
            da.SelectCommand.CommandTimeout = 1200
            da.SelectCommand.ExecuteNonQuery()
            ''da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(dt)
            Return dt
        Catch ex As Exception

        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetData(sdate As String, edate As String, vendor As String, status As String, dept As String) As DGrid
        Dim d As New DGrid()
        Try
            d = GenearateQuery(sdate, edate, vendor, status, dept)
        Catch ex As Exception
            ex.ToString()
        End Try
        Return d
    End Function
    <WebMethod()> _
    Public Shared Function GetVendors() As List(Of ListItem)
        Try
            Dim query As String = ""
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                query = "select fld5[TID],dms.udf_split('MASTER-Vendor Master-fld2',fld5)[Vendor] from mmm_mst_doc d where documenttype='Invoice PO' and curstatus not in ('rejected') and eid=152 and fld65 in  (" & HttpContext.Current.Session("Dept").ToString() & ") and convert(varchar(50),convert(date,fld18,3),120)>='" & HttpContext.Current.Session("Sdate").ToString() & "' and convert(varchar(50),convert(date,fld18,3),120)<='" & HttpContext.Current.Session("Edate").ToString() & "'"
                Query &= " union select fld5,dms.udf_split('MASTER-Vendor Master-fld2',fld5) from mmm_mst_doc d where documenttype='Invoice non PO' and curstatus not in ('rejected') and eid=152 and fld3 in  (" & HttpContext.Current.Session("Dept").ToString() & ") and convert(varchar(50),convert(date,fld14,3),120)>='" & HttpContext.Current.Session("Sdate").ToString() & "' and convert(varchar(50),convert(date,fld14,3),120)<='" & HttpContext.Current.Session("Edate").ToString() & "'  "
                Query &= "Union select fld5,dms.udf_split('MASTER-Vendor Master-fld2',fld5) from mmm_mst_doc d where documenttype='Invoice on hold' and curstatus not in ('rejected') and eid=152 and fld21 in  (" & HttpContext.Current.Session("Dept").ToString() & ") and convert(varchar(50),convert(date,fld4,3),120)>='" & HttpContext.Current.Session("Sdate").ToString() & "' and convert(varchar(50),convert(date,fld4,3),120)<='" & HttpContext.Current.Session("Edate").ToString() & "' order by Vendor  "
            Else
                query = "select fld5[TID],dms.udf_split('MASTER-Vendor Master-fld2',fld5)[Vendor] from mmm_mst_doc d where documenttype='Invoice PO' and curstatus not in ('rejected') and eid=152 and fld65 in  (" & HttpContext.Current.Session("Dept").ToString() & ") and convert(varchar(50),convert(date,fld18,3),120)>='" & HttpContext.Current.Session("Sdate").ToString() & "' and convert(varchar(50),convert(date,fld18,3),120)<='" & HttpContext.Current.Session("Edate").ToString() & "' and fld79 in (" & HttpContext.Current.Session("SubDept").ToString & ") "
                query &= " union select fld5,dms.udf_split('MASTER-Vendor Master-fld2',fld5) from mmm_mst_doc d where documenttype='Invoice non PO' and curstatus not in ('rejected') and eid=152 and fld3 in  (" & HttpContext.Current.Session("Dept").ToString() & ") and convert(varchar(50),convert(date,fld14,3),120)>='" & HttpContext.Current.Session("Sdate").ToString() & "' and convert(varchar(50),convert(date,fld14,3),120)<='" & HttpContext.Current.Session("Edate").ToString() & "' and fld2 in (" & HttpContext.Current.Session("SubDept").ToString & ") "
                query &= "Union select fld5,dms.udf_split('MASTER-Vendor Master-fld2',fld5) from mmm_mst_doc d where documenttype='Invoice on hold' and curstatus not in ('rejected') and eid=152 and fld21 in  (" & HttpContext.Current.Session("Dept").ToString() & ") and convert(varchar(50),convert(date,fld4,3),120)>='" & HttpContext.Current.Session("Sdate").ToString() & "' and convert(varchar(50),convert(date,fld4,3),120)<='" & HttpContext.Current.Session("Edate").ToString() & "' and fld18 in (" & HttpContext.Current.Session("SubDept").ToString & ") order by Vendor  "
            End If

            Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using con As New SqlConnection(constr)
                Using cmd As New SqlCommand(Query)
                    Dim customers As New List(Of ListItem)()
                    cmd.CommandType = CommandType.Text
                    cmd.Connection = con
                    con.Open()
                    Using sdr As SqlDataReader = cmd.ExecuteReader()
                        While sdr.Read()
                            customers.Add(New ListItem() With { _
                              .Value = sdr("TID").ToString(), _
                              .Text = sdr("Vendor").ToString() _
                            })
                        End While
                    End Using
                    con.Close()
                    Return customers
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Function
End Class
