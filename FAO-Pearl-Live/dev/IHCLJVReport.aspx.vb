Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Partial Class IHCLJVReport
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
            Try
                'Dim doc As String = ""
                'da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & ""
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                Dim ds As New DataSet
                '' 
                da.SelectCommand.CommandText = "select Reportid,ReportName from mmm_mst_report where eid=" & Session("EID") & " and reportid in (1687) order by reportid "
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
    Private Const ASCENDING As String = " ASC"
    Private Const DESCENDING As String = " DESC"
    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.SelectedIndexChanged
        Session("Report") = ddlDocType.SelectedValue.ToString
    End Sub
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

    Protected Shared Function GenearateQuery(stdate As String, edate As String) As DGrid
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

        'If HttpContext.Current.Session("DDlField") = "adate" Then
        'If data("frdate").ToString() <> "" Then
        str = Replace(str, "@filter", "((select convert(date,max(fdate)) from mmm_doc_dtl where docid=d.tid)>='" & stdate.ToString & "' and (select convert(date,max(fdate)) from mmm_doc_dtl where docid=d.tid)<='" & edate.ToString & "')")
        'End If
        'End If
        str = Replace(str, "@filter", " tid=tid")
        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CADMIN" Then
        Else
            str = Replace(str, "@role", " d.fld20 in (" & Dept & ")")
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
    <WebMethod()>
   <Script.Services.ScriptMethod()>
    Public Shared Function GetData(stdate As String, edate As String) As DGrid
        Dim d As New DGrid()
        Try
            'If HttpContext.Current.Session("DCType") <> "0" Then
            d = GenearateQuery(stdate, edate)
            'End If
        Catch ex As Exception
            ex.ToString()
        End Try
        Return d
    End Function
End Class
