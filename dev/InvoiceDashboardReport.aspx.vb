Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Partial Class InvoiceDashboardReport
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
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Session("ReportName") = ""
            Try
                'Dim doc As String = ""
                'da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & ""
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                Dim ds As New DataSet
                '' 
                pnl1.Visible = False
                da.SelectCommand.CommandText = "select ReportName,ReportDescription from mmm_mst_report where eid=" & Session("EID") & " and reportid in (1689,1691,1692,1693) "
                da.Fill(ds, "data")
                ddlDocType.DataSource = ds
                ddlDocType.DataTextField = "ReportName"
                ddlDocType.DataValueField = "ReportDescription"
                ddlDocType.DataBind()
                ddlDocType.Items.Insert(0, "--Please Select--")

                Dim dept1 As String = ""
                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                    da.SelectCommand.CommandText = "select TID,fld2[Dept] from mmm_mst_master where eid=" & Session("EID") & " and documenttype='Department master' order by fld2"
                    da.Fill(ds, "data1")
                Else
                    da.SelectCommand.CommandText = "select fld1 from mmm_ref_role_user where eid=" & Session("EID") & "  and UID=" & HttpContext.Current.Session("UID") & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    dept1 = da.SelectCommand.ExecuteScalar().ToString
                    Session("SubDept") = dept1
                    'da.SelectCommand.CommandText = "select distinct fld25[TID],dms.udf_split('MASTER-Department Master-fld2',fld25)[Dept] from mmm_mst_master where eid=" & Session("EID") & " and documenttype='sub department' and tid in (" & dept1 & ")  order by Dept"
                End If
                lstdept.DataSource = ds.Tables("data1")
                lstdept.DataTextField = "Dept"
                lstdept.DataValueField = "TID"
                lstdept.DataBind()

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
    'Private Sub SortGridView(ByVal sortExpression As String, ByVal direction As String)
    '    'You can cache the DataTable for improving performance
    '    Dim dt As DataTable = CType(ViewState("pending"), DataTable)
    '    dt.DefaultView.Sort = sortExpression + direction
    '    Dim dt1 As DataTable = dt.DefaultView.ToTable()
    '    '   gvPending.DataSource = ViewState("pending")
    '    '  gvPending.DataBind()
    '    'BindGrid()
    '    'AdvSearch()
    '    'ViewState("data") = dt1
    'End Sub
    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim str As String = ""
        Dim da As New SqlDataAdapter("select actualfilter from mmm_mst_report where eid=" & Session("EID") & " and reportname='" & ddlDocType.SelectedItem.Text & "' ", con)
        ''''union select 'Invoice Number','Invoice Number',0 as DocDtlDisplayOrder union select 'Invoice Date','Invoice Date', 0 as DocDtlDisplayOrder union select 'Vendor Name','Vendor Name',0 as DocDtlDisplayOrder 
        Try
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'str = da.SelectCommand.ExecuteScalar()
            'If str = "Department" Then
            '    da.SelectCommand.CommandText = "select tid[TID],fld2[Disp] from mmm_mst_master where documenttype='Department master' and eid=" & Session("EID") & " "
            'Else
            '    da.SelectCommand.CommandText = "select distinct aprstatus[TID],aprstatus[Disp] from mmm_mst_authmetrix where eid=" & Session("EID") & " and doctype in ('Invoice PO','Invoice Non PO','Invoice On Hold') union select 'Uploaded','Uploaded' union select 'Archive','Archive'"
            'End If
            'Dim ds As New DataSet
            'da.Fill(ds, "data")
            'lstBoxTest.DataSource = ds
            'lstBoxTest.DataTextField = "Disp"
            'lstBoxTest.DataValueField = "TID"
            'lstBoxTest.DataBind()
            ''   gvPending.Controls.Clear()
            'Session("DCType") = ddlDocType.SelectedItem.Text
            'Session("Filter") = str
            'For Each li As ListItem In lstBoxTest.Items
            '    li.Selected = True
            'Next
            'GetData(j)
            If ddlDocType.SelectedItem.Text.ToString() = "Open Invoice Ageing Report from Last Action Status Wise" Or ddlDocType.SelectedItem.Text.ToString = "Open Invoice Ageing Report from Receipt Date Status Wise" Then
                pnl1.Visible = True
            Else
                pnl1.Visible = False
            End If
            Session("ReportName") = ddlDocType.SelectedItem.ToString
            'GetData(ddlcurrency.SelectedValue.ToString, ddlstatus.SelectedValue.ToString, Session("Dept"))
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
    
    'Public Shared Function GetAllFields(EID As Integer) As DataSet
    '    Dim ds As New DataSet()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Try
    '        '            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & " and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number');"
    '        Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName,F.Datatype  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & " and  issearch=1 and (f.fieldtype='auto number' or f.isactive in (1,0) or f.fieldtype='new auto number') order by f.DocDtlDisplayOrder;"
    '        Using con = New SqlConnection(conStr)
    '            Using da = New SqlDataAdapter(Query, con)
    '                da.Fill(ds)
    '            End Using
    '        End Using
    '    Catch ex As Exception
    '        Throw
    '    End Try
    '    Return ds
    'End Function
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
        'EID = 999

        'Dim str As String = ""
        ' Geiing all the field of Entity becouse all the field might be required
        ' ds = GetAllFields(eid)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("select qryfield from mmm_mst_report where eid=" & eid & "  and Reportname='" & HttpContext.Current.Session("ReportName").ToString & "' ", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        'HttpContext.Current.Session("Dept") = dept
        'Dim util As New InvoiceDashboardReport
        'Dim selectedValues As String = util.list()

        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
        Dim repval As String = ""
        If HttpContext.Current.Session("Dept") <> "" Then
            str = Replace(str, "@dept1", " fld65 in  (" & HttpContext.Current.Session("Dept") & ")")
            str = Replace(str, "@dept2", " fld3 in  (" & HttpContext.Current.Session("Dept") & ")")
            str = Replace(str, "@dept3", " fld21 in  (" & HttpContext.Current.Session("Dept") & ")")
            str = Replace(str, "@dept4", " fld19 in  (" & HttpContext.Current.Session("Dept") & ")")
        Else
            str = Replace(str, "@dept1", " fld65=fld65")
            str = Replace(str, "@dept2", " fld3 =fld3")
            str = Replace(str, "@dept3", " fld21 =fld21")
            str = Replace(str, "@dept4", " fld19 =fld19")
        End If
        If ddlstatus.SelectedValue.ToString = "0" Then
            str = Replace(str, "@status", " curstatus=curstatus")
        ElseIf ddlstatus.SelectedValue.ToString = "1" Then
            str = Replace(str, "@status", " curstatus='Archive'")
        Else
            str = Replace(str, "@status", " curstatus not in ('Archive')")
        End If
        If ddlcurrency.SelectedValue.ToString = "0" Then
            str = Replace(str, "@curr1", " fld64=fld64")
            str = Replace(str, "@curr2", " fld12=fld12")
            str = Replace(str, "@curr3", " fld20=fld20")
            str = Replace(str, "@curr4", " fld88=fld88")
        ElseIf ddlcurrency.SelectedValue.ToString.ToString = "1" Then
            str = Replace(str, "@curr1", " fld64='INR'")
            str = Replace(str, "@curr2", " fld12='INR'")
            str = Replace(str, "@curr3", " fld20='INR'")
            str = Replace(str, "@curr4", " fld88='INR'")
        Else
            str = Replace(str, "@curr1", " fld64<>'INR'")
            str = Replace(str, "@curr2", " fld12<>'INR'")
            str = Replace(str, "@curr3", " fld20<>'INR'")
            str = Replace(str, "@curr4", " fld88<>'INR'")
        End If

        'str = Replace(str, "@filter", "tid=tid")
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
        'ds1 = GetAllFields(Session("EID"))
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

        For i = 0 To gridview1.HeaderRow.Cells.Count - 1
            If gridview1.HeaderRow.Cells(i).Text.Contains("CurrentStatus").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Current Status"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("Count1To7Days").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Count 1-7 Days"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("InvoiceAmount1To7Days").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Invoice Amount 1-7 Days(lacs)"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("Count8To14Days").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Count 8-14 Days"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("InvoiceAmount8To14Days").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Invoice Amount 8-14 Days(lacs)"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("Count15Days").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Count 15+ Days"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("InvoiceAmount15Days").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Invoice Amount 15+ Days(lacs)"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("TotalBarCodeCount").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Total Bar Code Count"
            ElseIf gridview1.HeaderRow.Cells(i).Text.Contains("TotalInvoiceAmount").ToString Then
                gridview1.HeaderRow.Cells(i).Text = "Total Invoice Amount(lacs)"
            End If
        Next
        gridview1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
    Public Shared Function GenearateQuery(currency As String, status As String, dept As String) As DGrid
        Dim ret As String = ""
        Dim grid As New DGrid()
        Dim ds As New DataSet()
        Dim dt As New DataTable

        'Dim str As String = ""
        ' Geiing all the field of Entity becouse all the field might be required
        Dim eid As Integer = HttpContext.Current.Session("eid")  '999
        ' ds = GetAllFields(eid)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("select qryfield from mmm_mst_report where eid=" & eid & "  and Reportname='" & HttpContext.Current.Session("ReportName").ToString & "' ", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        HttpContext.Current.Session("Dept") = dept
        'Dim util As New InvoiceDashboardReport
        'Dim selectedValues As String = util.list()

        Dim str As String = da.SelectCommand.ExecuteScalar().ToString
        Dim repval As String = ""
        If dept <> "" Then
            dept = dept.ToString.Remove(dept.ToString.LastIndexOf(","), 1)
            HttpContext.Current.Session("Dept") = dept
            str = Replace(str, "@dept1", " fld65 in  (" & dept & ")")
            str = Replace(str, "@dept2", " fld3 in  (" & dept & ")")
            str = Replace(str, "@dept3", " fld21 in  (" & dept & ")")
            str = Replace(str, "@dept4", " fld20 in  (" & dept & ")")
        Else
            str = Replace(str, "@dept1", " fld65=fld65")
            str = Replace(str, "@dept2", " fld3 =fld3")
            str = Replace(str, "@dept3", " fld21 =fld21")
            str = Replace(str, "@dept4", " fld20 =fld20")
        End If
        If status.ToString = "0" Then
            str = Replace(str, "@status", " curstatus=curstatus")
        ElseIf status.ToString = "1" Then
            str = Replace(str, "@status", " curstatus='Archive'")
        Else
            str = Replace(str, "@status", " curstatus not in ('Archive')")
        End If
        If currency.ToString = "0" Then
            str = Replace(str, "@curr1", " fld64=fld64")
            str = Replace(str, "@curr2", " fld12=fld12")
            str = Replace(str, "@curr3", " fld20=fld20")
            str = Replace(str, "@curr4", " fld88=fld88")
        ElseIf currency.ToString = "1" Then
            str = Replace(str, "@curr1", " fld64='INR'")
            str = Replace(str, "@curr2", " fld12='INR'")
            str = Replace(str, "@curr3", " fld20='INR'")
            str = Replace(str, "@curr4", " fld88='INR'")
        Else
            str = Replace(str, "@curr1", " fld64<>'INR'")
            str = Replace(str, "@curr2", " fld12<>'INR'")
            str = Replace(str, "@curr3", " fld20<>'INR'")
            str = Replace(str, "@curr4", " fld88<>'INR'")
        End If

        'str = Replace(str, "@filter", "tid=tid")
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
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetData(currency As String, status As String, dept As String) As DGrid
        Dim d As New DGrid()
        Try
            'If HttpContext.Current.Session("DCType") <> "0" Then
            d = GenearateQuery(currency, status, dept)
            'End If
        Catch ex As Exception
            ex.ToString()
        End Try
        Return d
    End Function

End Class

