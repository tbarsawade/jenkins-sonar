Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Web.Services
Imports Ionic.Zip
Imports Microsoft.Office.Interop
Imports System.Web.Hosting
Partial Class NewReportMaster
    Inherits System.Web.UI.Page
    Dim stradd As String = ""
    Dim actualval As String = ""
    Dim strOr As String = ""
    Public Shared Mydatatable As New DataTable()
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
        If Request.QueryString("SC") Is Nothing Then
        Else
            Dim con As SqlConnection = Nothing
            Dim oda As SqlDataAdapter = Nothing
            Try
                Dim scrname As String = Request.QueryString("SC").ToString()
                Dim str1 As String = "" 'Request.UrlReferrer.ToString().Replace(" ", String.Empty)
                Dim str2 As String = Request.Url.ToString().Replace("+", String.Empty)
                str2 = str2.Replace(" ", String.Empty)
                If str1 <> str2 Then
                    Session("dtNew") = Nothing
                End If
                pnlchart.Visible = False
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                con = New SqlConnection(conStr)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim fldm As String
                'Commented By Komal on 13Nov2013 
                'Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & Session("eid") & " and reportname= '" & scrname & "'", con)
                'Added by Komal on 13Nov2013
                Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity,IsViewOn  from MMM_MST_REPORT where eid=" & Session("eid") & " and reportname= '" & scrname & "'", con)
                Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString              ' ToString Appended By Komal on 15Nov2013
                Dim dt As New DataTable
                abc.Fill(dt)
                ViewState("IsViewOn") = dt.Rows(0)("IsViewOn")    'Added By Komal on 15Nov2013

                hdnView.Value = dt.Rows(0)("IsViewOn")

                If dt.Rows(0)("IsViewOn") = 1 Then
                    fldm = Viewfldmapping()
                Else
                    fldm = fldmapping()
                End If
                'End
                'Dim da2 As New SqlDataAdapter("SELECT ISQRYFIELD FROM MMM_MST_REPORT WHERE  EID='" & Session("EID") & "' AND reportid= " & ViewState("rid") & " ", con)
                oda = New SqlDataAdapter("", con)
                oda.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where uid=" & Session("UID") & " "
                Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                If dt.Rows(0)("IsViewOn") = 1 Then
                    If cnt > 0 Then
                        oda.SelectCommand.CommandText = "SELECT distinct FF.*,U.UID,U.DOCUMENTTYPE [REF_DOCTYPE],U.ROLENAME  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID left outer join mmm_ref_role_user u on u.uid=" & Session("UID") & " and u.rolename='" & Session("Userrole") & "' where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and DisplayName in (" & fldm & ") order by datatype "
                    Else
                        If ((Session("USERROLE") = "FCAGGN") Or (Session("USERROLE") = "SU") Or (Session("USERROLE") = "BNK") Or (Session("USERROLE") = "CADMIN")) Then
                            oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and DisplayName in (" & fldm & ") order by datatype "
                        ElseIf ((Session("USERROLE") <> "SU") Or (Session("USERROLE") <> "CADMIN")) Then
                            oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ") order by datatype "
                        Else
                            Exit Sub
                        End If
                    End If
                Else
                    If cnt > 0 Then
                        oda.SelectCommand.CommandText = "SELECT distinct FF.*,U.UID,U.DOCUMENTTYPE [REF_DOCTYPE],U.ROLENAME  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID left outer join mmm_ref_role_user u on u.uid=" & Session("UID") & " and u.rolename='" & Session("Userrole") & "' where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ")  order by datatype "
                    Else
                        If ((Session("USERROLE") = "FCAGGN") Or (Session("USERROLE") = "SU") Or (Session("USERROLE") = "BNK") Or (Session("USERROLE") = "CADMIN")) Then
                            oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ") order by datatype "
                        ElseIf ((Session("USERROLE") <> "SU") Or (Session("USERROLE") <> "CADMIN")) Then
                            oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ") order by datatype "
                        Else
                            Exit Sub
                        End If
                    End If
                End If
                'Commented By Komal on 18Nov2013
                'If cnt > 0 Then
                '    oda.SelectCommand.CommandText = "SELECT distinct FF.*,U.UID,U.DOCUMENTTYPE [REF_DOCTYPE],U.ROLENAME  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID left outer join mmm_ref_role_user u on u.uid=" & Session("UID") & " and u.rolename='" & Session("Userrole") & "' where FF.isactive=1 and u.isview=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ") and u.documenttype like '%" & xy & "%' order by datatype desc"
                'Else
                '    If ((Session("USERROLE") = "FCAGGN") Or (Session("USERROLE") = "SU") Or (Session("USERROLE") = "BNK") Or (Session("USERROLE") = "CADMIN")) Then
                '        'Commented By Komal on 14Nov2013
                '        'oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ") order by datatype desc"
                '        'Added By Komal on 12Nov2013
                '        If dt.Rows(0)("IsViewOn") = 1 Then
                '            oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and DisplayName in (" & fldm & ") order by datatype desc"
                '        Else
                '            oda.SelectCommand.CommandText = "SELECT distinct FF.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and ff.documenttype = '" & xy & "' and fieldmapping in (" & fldm & ") order by datatype desc"
                '        End If
                '        'End
                '    Else
                '        Exit Sub
                '    End If
                'End If
                Dim ds As New DataSet()
                oda.Fill(ds, "data")
                If ds.Tables("data").Rows.Count < 1 Then
                    'btnActEdit.Visible = False
                Else
                    'btnActEdit.Visible = True
                End If
                Dim ob As New DynamicForm()
                'ob.CreateControlsOnAdvanceSearch(ds.Tables("data"), pnlFields)
                'ob.CreateControlsOnReport(ds.Tables("data"), pnlFields)
                ob.CreateControlsOnReport(ds.Tables("data"), pnlFields, scrname)
                If ds.Tables("data").Rows.Count = 0 Then
                    ' pnlFields.Visible = False
                    pnlFields.Visible = True
                    pngv.Height = "300"
                Else
                    pnlFields.Visible = True
                    pngv.Height = "300"
                End If
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If
            End Try
        End If
    End Sub
    Public Shared Function fldmapping(Optional ByVal SC As String = Nothing) As String
        Dim scrname As String = ""
        If SC = Nothing Then
            scrname = HttpContext.Current.Request.QueryString("SC").ToString()
        Else
            scrname = SC
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & HttpContext.Current.Session("eid") & " and reportname= '" & scrname & "'", con)
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            If HttpContext.Current.Session("USERROLE") = "SU" Then
                abc.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and eid=" & HttpContext.Current.Session("EID") & " and reportname= '" & scrname & "'"
            Else
                abc.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where subentity ='" & xy & "' and eid=" & HttpContext.Current.Session("EID") & " and reportname= '" & scrname & "'"
            End If
            Dim ss As String = abc.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()
            Dim strArray As String() = ss.Split(" "c)
            For Each item As String In strArray
                If item.Contains("@") Then
                    strArray = item.Split("@"c)
                    If Trim(strArray(1)).Contains(")") Then
                        strArray(1) = Microsoft.VisualBasic.Left(strArray(1), 5)
                        '  strArray(1) = Left(strArray(1), strArray(1).Length - 1)
                        strArray(1) = strArray(1).Replace(")", "")
                        strArray(1) = strArray(1).Replace(",", "")

                    End If
                    sb.Append("'" & strArray(1).ToString() & "'")
                    sb.Append(",")
                End If
            Next

            Dim az As String = sb.ToString()

            If az.Length > 2 Then
                az = az.Remove(az.Length - 1)
            Else
                az = "''"
            End If
            Return az
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function
    ' Public Function Get Report using Views 
    Public Shared Function Viewfldmapping(Optional ByVal SC As String = Nothing) As String
        Dim scrname As String = ""
        If SC = Nothing Then
            scrname = HttpContext.Current.Request.QueryString("SC").ToString()
        Else
            scrname = SC
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & HttpContext.Current.Session("eid") & " and reportname= '" & scrname & "'", con)
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            If HttpContext.Current.Session("USERROLE") = "SU" Then
                abc.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and eid=" & HttpContext.Current.Session("EID") & " and reportname= '" & scrname & "'"
            Else
                abc.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where subentity ='" & xy & "' and eid=" & HttpContext.Current.Session("EID") & " and reportname= '" & scrname & "'"
            End If

            Dim ss As String = abc.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            Dim strArray As String() = ss.Split("="c)
            For Each item As String In strArray
                If item.Contains("@") Then
                    'Dim index As Int16 = item.IndexOf("@") + 1
                    Dim start As Int16 = item.IndexOf("@")
                    Dim finish As Int16 = item.IndexOf("]")
                    item = item.Substring(start, finish - start)
                    strArray = item.Split("@"c)
                    strArray(1) = Microsoft.VisualBasic.Left(strArray(1), strArray(1).Length)
                    '  strArray(1) = Left(strArray(1), strArray(1).Length - 1)
                    strArray(1) = strArray(1).Replace(")", "")
                    strArray(1) = strArray(1).Replace("]", "")
                    strArray(1) = strArray(1).Replace("[", "")
                    strArray(1) = strArray(1).Replace(",", "")
                    sb.Append("'" & strArray(1).ToString() & "'")
                    sb.Append(",")
                End If
            Next
            Dim az As String = sb.ToString()

            If az.Length > 2 Then
                az = az.Remove(az.Length - 1)
            Else
                az = "''"
            End If
            Return az
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            abc.Dispose()
        End Try
    End Function
    Public Sub bindvalue(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim ddl As DropDownList = TryCast(sender, DropDownList)
            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim ob As New DynamicForm()
            ob.bind(id, pnlFields, ddl)
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Request.QueryString("SC") Is Nothing Then
            Else
                Dim scrname As String = Request.QueryString("SC").ToString()
                ViewState("ReportName") = scrname
                Dim da As SqlDataAdapter = Nothing
                Dim da1 As SqlDataAdapter = Nothing
                Dim con As SqlConnection = Nothing
                Try
                    lblMsg.Text = scrname
                    ViewState("RPTname") = scrname
                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    con = New SqlConnection(conStr)
                    'fill Product  
                    'show()
                    If Request.QueryString("SC") = "Pending Invoice Against VRF Fixed Pool" Then
                        dvmsg.InnerHtml = "Please click on search button for processing your request & report will be sent on your registered e-mail ID shortly !"
                    Else
                        dvmsg.InnerHtml = ""
                    End If
                    da = New SqlDataAdapter("uspGetReportDtl", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.Clear()
                    'da.Dispose()
                    'ds.Dispose()
                    ' Dim con As New SqlConnection(conStr)
                    con.Open()
                    pnlchart.Visible = False
                    Dim da3 As New SqlDataAdapter("select distinct mainentity,subentity,reportid,actualfilter,ismapicon  from MMM_MST_REPORT where EID=" & Session("EID").ToString() & " and reportName ='" & ViewState("RPTname") & "' ", con)
                    Dim dt3 As New DataTable()
                    da3.Fill(dt3)
                    ViewState("me") = dt3.Rows(0).Item("mainentity").ToString()
                    ViewState("se") = dt3.Rows(0).Item("subentity").ToString()
                    ViewState("rid") = dt3.Rows(0).Item("reportid").ToString()
                    ViewState("Actualfilter") = dt3.Rows(0).Item("actualfilter").ToString()
                    If dt3.Rows(0).Item("isMapIcon").ToString = "1" Then
                        btnchangeView.Visible = True
                        Session("Map") = "1"
                    Else
                        btnchangeView.Visible = False
                        Session("Map") = "0"
                    End If
                    Dim da2 As New SqlDataAdapter("select distinct  displayName,FieldMapping  from MMM_MST_FIELDS f inner join MMM_MST_REPORT r on r.SubEntity =f.DocumentType  where DocumentType='" & ViewState("se") & "' and datatype='numeric' and f.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "'", con)
                    Dim dt As New DataTable()
                    da2.Fill(dt)
                    ddly.DataSource = dt
                    'ViewState("x") = dt.Rows(0).Item("fieldmapping").ToString()
                    ddly.DataTextField = "displayname"
                    ddly.DataValueField = "fieldmapping"
                    ddly.DataBind()
                    ddly.Items.Insert(0, "Select")

                    da1 = New SqlDataAdapter("select distinct  displayName,FieldMapping  from MMM_MST_FIELDS f inner join MMM_MST_REPORT r on r.SubEntity =f.DocumentType  where DocumentType='" & ViewState("se") & "' and datatype<>'numeric' and f.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "'", con)
                    Dim dt1 As New DataTable()
                    da1.Fill(dt1)
                    ddlx.DataSource = dt1
                    ' ViewState("y") = dt1.Rows(0).Item("fieldmapping").ToString()
                    ddlx.DataTextField = "displayname"
                    ddlx.DataValueField = "fieldmapping"
                    ddlx.DataBind()
                    ddlx.Items.Insert(0, "Select")
                    lblCaption.Text = "Please Click on Seach button to view report."
                    lblCaption.ForeColor = Color.Red
                Catch ex As Exception
                Finally
                    If Not da Is Nothing Then
                        da.Dispose()
                    End If
                    If Not da1 Is Nothing Then
                        da1.Dispose()
                    End If
                    If Not con Is Nothing Then
                        con.Close()
                        con.Dispose()
                    End If
                End Try
            End If
            btnchart.Visible = False
        End If
    End Sub
    Protected Sub btnActEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        If Request.QueryString("SC") = "Pending Invoice Against VRF Fixed Pool" Then
            ' dvmsg.InnerHtml = "We are processing your request & report will be sent on your registered e-mail ID shortly !"
            Dim str As String = ShowVrfReport()
            If str = "True" Then
                dvmsg.InnerHtml = "Report has been sent to your registered e-mail ID."
            End If
        Else
            show()
        End If
    End Sub
    Protected Sub show()
        If ViewState("IsViewOn") = 1 Then                     ' Added By Komal on 15Nov213
            ShowViewsOnReport()
        Else
            showViewOffReport()
        End If
    End Sub
    'Show Report from View
    Protected Sub ShowViewsOnReport()
        gvReport.Visible = True
        pnlchart.Visible = False
        pngv.Visible = True

        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select qryfieldrole from mmm_mst_report where reportname ='" & scrname & "' and eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
            Dim fldms As String
            ' If ViewState("IsViewOn") = 1 Then
            fldms = Viewfldmapping()
            'Else
            '    fldms = fldmapping()
            'End If

            Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & Session("eid") & " and reportname= '" & scrname & "'", con)
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            'If ViewState("IsViewOn") = 1 Then
            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and displayname in (" & fldms & ")"
            'Else
            '    da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and FieldMapping in (" & fldms & ")"
            'End If
            da.Fill(ds, "flds")
            Dim ss As String = ""
            If Session("USERROLE") = "SU" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            Else
                da.SelectCommand.CommandText = "select qryfieldrole  from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            End If

            da.SelectCommand.CommandTimeout = 600
            ss = da.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            'Filter by Dynamic controls
            For Each row As DataRow In ds.Tables("flds").Rows
                Dim pqr As String = row.Item("fieldtype").ToString()
                Dim dtyp As String = row.Item("datatype").ToString()
                Dim fmap As String = row.Item("fieldmapping").ToString()
                Dim DsplyName As String = row.Item("DisplayName").ToString()
                Dim var As String = ""
                Dim var2 As String = ""
                Dim chklist As String = ""
                var = row.Item("fieldid").ToString()
                var2 = row.Item("fieldid").ToString()
                If dtyp.ToUpper() = "DATETIME" Then
                    var = "Frfld" & var
                    var2 = "Tofld" & var2
                ElseIf dtyp.ToUpper = "NUMERIC" Then
                    var = "fldR1" & var
                    var2 = "fldR2" & var2
                End If
                If pqr = "Text Box" Then
                    Dim txtobj As New System.Web.UI.WebControls.TextBox
                    Dim txtobj2 As New System.Web.UI.WebControls.TextBox
                    txtobj = TryCast(pnlFields.FindControl(var), System.Web.UI.WebControls.TextBox)
                    txtobj2 = TryCast(pnlFields.FindControl(var2), System.Web.UI.WebControls.TextBox)
                    Dim ct As String = "[" & "@" & DsplyName & "]"
                    Dim ct2 As String = "[" & "$" & DsplyName & "]"
                    If IsNothing(txtobj2) Then
                        If txtobj.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ' ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False Then
                                    ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                                    ' ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%" & Trim(txtobj.Text) & "%'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False Then
                                    'If ViewState("IsViewOn") = 1 Then

                                    ss = Replace(ss, ct, Replace(ct, "@", ""))
                                    ss = Replace(ss, ct, Replace(ct, "$", ""))
                                    'Else
                                    '    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    '    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                    'End If
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, "like '%%'")
                            End If
                        End If
                    Else
                        If txtobj.Text <> "" Or txtobj2.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then

                                If IsDate(getdate(txtobj.Text)) <> False And IsDate(getdate(txtobj2.Text)) <> False Then
                                    'If ViewState("IsViewOn") = 1 Then
                                    ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                                    ss = Replace(ss, ct2, "'" & txtobj2.Text & "'")
                                    'Else
                                    '    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    '    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                    'End If
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%%'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                'If ViewState("IsViewOn") = 1 Then
                                ss = Replace(ss, ct, Replace(ct, "@", ""))
                                ss = Replace(ss, ct2, Replace(ct2, "$", ""))
                                'Else
                                '    ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                '    ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                                'End If
                            Else
                                ss = Replace(ss, ct, Replace(ct, "@", ""))
                                ss = Replace(ss, ct2, Replace(ct2, "@", ""))
                            End If
                        End If
                    End If
                ElseIf pqr = "Drop Down" Then
                    Dim chkobj As New CheckBoxList
                    chkobj = TryCast(pnlFields.FindControl("chklist" & var), CheckBoxList)
                    Dim ct As String = "[" & "@" & DsplyName & "]"
                    For i As Integer = 0 To chkobj.Items.Count - 1
                        If chkobj.Items(i).Selected = True Then
                            chklist = chklist & chkobj.Items(i).Value & ","
                        End If
                    Next
                    If chklist.ToString = "" Then
                    Else
                        chklist = Left(chklist, chklist.Length - 1)
                    End If
                    If chklist.ToString <> "" Then
                        If dtyp.ToUpper() = "NUMERIC" Then
                            ss = Replace(ss, ct, chklist)
                        Else
                            If chkobj.SelectedItem.Text.ToUpper() = "SELECT" Then
                                ct = "=" & ct
                                ss = Replace(ss, ct, "LIKE '%%'")
                            Else
                                ss = Replace(ss, ct, chklist)
                            End If
                        End If
                    Else
                        ss = Replace(ss, ct, "EXFP." & DsplyName)
                    End If
                End If
            Next
            If xy.ToUpper = "HUB MASTER" Then
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frfldtxtdate"), System.Web.UI.WebControls.TextBox)
                If txtobjdate.Text <> "" Then
                    ss = Replace(ss, "@adate", "Convert(date,'" & txtobjdate.Text & "')")
                Else
                    ss = Replace(ss, "@adate", "Convert(date,getdate())")
                End If
            Else
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                Dim txtobjdate1 As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frflddate"), System.Web.UI.WebControls.TextBox)
                txtobjdate1 = TryCast(pnlFields.FindControl("Toflddate"), System.Web.UI.WebControls.TextBox)
                If IsNothing(txtobjdate) Then
                Else
                    If txtobjdate.Text <> "" Then
                        ss = Replace(ss, "@adate", "Convert(date,'" & txtobjdate.Text & "')")
                    Else
                        ss = Replace(ss, "@adate", "convert(date,d.adate)")
                    End If
                    If txtobjdate1.Text <> "" Then
                        ss = Replace(ss, "$adate", "Convert(date,'" & txtobjdate1.Text & "')")
                    Else
                        ss = Replace(ss, "$adate", "convert(date,d.adate)")
                    End If
                End If
            End If
            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & Session("EID") & " and uid=" & Session("UID") & " and isview=1"
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab <> "0" Then
                ss = Replace(ss, "[@uid]", Session("UID"))
                ss = Replace(ss, "[@rolename]", "'" & Session("USERROLE") & "'")
                da.SelectCommand.CommandText = ss
                da.SelectCommand.CommandTimeout = 900
                da.Fill(ds, "ss")
                ViewState("xlexport") = ds.Tables("ss")
                Mydatatable = ds.Tables("ss")
                If ds.Tables("ss").Rows.Count > 0 Then
                    gvReport.DataSource = ds.Tables("ss")
                    gvReport.DataBind()
                    lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                    btnchart.Visible = True
                Else
                    btnchart.Visible = False
                    lblCaption.Text = "No Records Found.."
                    gvReport.Controls.Clear()
                    gvReport.CaptionAlign = TableCaptionAlign.Left
                End If
                btnchart.Visible = False
                Exit Sub
            End If
            If ((Session("USERROLE") <> "FCAGGN") And (Session("USERROLE") <> "SU") And (Session("USERROLE") <> "BNK") And (Session("USERROLE") <> "CADMIN") And (Session("USERROLE") <> "FCANHQ")) And (ab = 0) Then
                If Session("EID") = "42" Then
                Else
                    lblCaption.Text = " Please Contact your Admin for authorization "
                    lblCaption.ForeColor = Color.Red
                    Exit Sub
                End If
            End If
            da.SelectCommand.CommandText = ss
            da.SelectCommand.CommandTimeout = 900
            da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(ds, "ss")

            ViewState("xlexport") = ds.Tables("ss")
            Mydatatable = ds.Tables("ss")
            If ds.Tables("ss").Rows.Count > 0 Then
                gvReport.DataSource = ds.Tables("ss")
                gvReport.DataBind()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                'gvReport.CaptionAlign = TableCaptionAlign.Left
                lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                btnchart.Visible = True
            Else
                btnchart.Visible = False
                lblCaption.Text = "No Records Found.."
                gvReport.Controls.Clear()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                gvReport.CaptionAlign = TableCaptionAlign.Left
            End If

        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
        Session("MyDatatable") = Mydatatable

    End Sub
    'Show Report from real Table
    Protected Sub showViewOffReport()
        gvReport.Visible = True
        pnlchart.Visible = False
        pngv.Visible = True
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select qryfieldrole from mmm_mst_report where reportname ='" & scrname & "' and eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString

            Dim fldms As String = fldmapping()
            Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & Session("eid") & " and reportname= '" & scrname & "'", con)
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and fieldmapping in (" & fldms & ") order by FieldMapping desc"
            da.Fill(ds, "flds")
            Dim ss As String = ""
            If Session("USERROLE") = "SU" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            Else
                da.SelectCommand.CommandText = "select qryfieldrole  from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            End If

            da.SelectCommand.CommandTimeout = 600
            ss = da.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            'Filter by Dynamic controls
            For Each row As DataRow In ds.Tables("flds").Rows
                Dim pqr As String = row.Item("fieldtype").ToString()
                Dim dtyp As String = row.Item("datatype").ToString()
                Dim fmap As String = row.Item("fieldmapping").ToString()
                Dim var As String = ""
                Dim var2 As String = ""
                Dim chklist As String = ""
                var = row.Item("fieldid").ToString()
                var2 = row.Item("fieldid").ToString()
                If dtyp.ToUpper() = "DATETIME" Then
                    var = "Frfld" & var
                    var2 = "Tofld" & var2
                ElseIf dtyp.ToUpper = "NUMERIC" Then
                    var = "fldR1" & var
                    var2 = "fldR2" & var2
                ElseIf pqr = "Text Box" And dtyp.ToUpper = "TEXT" Then
                    var = "fld" & var
                End If

                If pqr = "Text Box" Then
                    Dim txtobj As New System.Web.UI.WebControls.TextBox
                    Dim txtobj2 As New System.Web.UI.WebControls.TextBox
                    txtobj = TryCast(pnlFields.FindControl(var), System.Web.UI.WebControls.TextBox)
                    txtobj2 = TryCast(pnlFields.FindControl(var2), System.Web.UI.WebControls.TextBox)
                    Dim ct As String = "@" & fmap
                    Dim ct2 As String = "$" & fmap
                    If IsNothing(txtobj2) Then
                        If txtobj.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ' ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False Then
                                    'ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                                    ' ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%" & Trim(txtobj.Text) & "%'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                'ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                ss = Replace(ss, ct, "d." & fmap)
                                ' ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                            ElseIf dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, "d." & fmap)
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%%'")
                                'ss = Replace(ss, ct, "d." & fmap)
                                'ss = Replace(ss, ct2, "d." & fmap)
                            End If

                        End If
                    Else
                        If txtobj.Text <> "" Or txtobj2.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False And IsDate(getdate(txtobj2.Text)) <> False Then
                                    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%" & Trim(txtobj.Text) & "%'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                            Else
                                ss = Replace(ss, ct, "d." & fmap)
                                ss = Replace(ss, ct2, "d." & fmap)
                            End If

                        End If
                    End If

                ElseIf pqr = "Drop Down" Then
                    Dim chkobj As New CheckBoxList
                    chkobj = TryCast(pnlFields.FindControl("chklist" & var), CheckBoxList)
                    Dim ct As String = "@" & fmap

                    For i As Integer = 0 To chkobj.Items.Count - 1
                        If chkobj.Items(i).Selected = True Then
                            chklist = chklist & "'" & chkobj.Items(i).Value & "',"
                        End If
                    Next
                    If chklist.ToString = "" Then
                    Else
                        chklist = Left(chklist, chklist.Length - 1)
                    End If

                    If chklist.ToString <> "" Then
                        If dtyp.ToUpper() = "NUMERIC" Then
                            ss = Replace(ss, ct, chklist)
                        Else
                            If chkobj.SelectedItem.Text.ToUpper() = "SELECT" Then
                                ct = "=" & ct
                                ss = Replace(ss, ct, "LIKE '%%'")
                            Else
                                ss = Replace(ss, ct, chklist)
                            End If
                        End If
                    Else
                        ss = Replace(ss, ct, "d." & fmap)
                    End If
                End If
            Next

            If xy.ToUpper = "HUB MASTER" Then
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frfldtxtdate"), System.Web.UI.WebControls.TextBox)
                If txtobjdate.Text <> "" Then
                    ss = Replace(ss, "@adate", "Convert(date,'" & txtobjdate.Text & "')")
                Else
                    ss = Replace(ss, "@adate", "Convert(date,getdate())")
                End If
            Else
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                Dim txtobjdate1 As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frflddate"), System.Web.UI.WebControls.TextBox)
                txtobjdate1 = TryCast(pnlFields.FindControl("Toflddate"), System.Web.UI.WebControls.TextBox)
                If IsNothing(txtobjdate) Then
                Else
                    If txtobjdate.Text <> "" Then
                        ss = Replace(ss, "@adate", "Convert(date,'" & txtobjdate.Text & "')")
                    Else
                        ss = Replace(ss, "@adate", "convert(date,d.adate)")
                    End If
                    If txtobjdate1.Text <> "" Then
                        ss = Replace(ss, "$adate", "Convert(date,'" & txtobjdate1.Text & "')")
                    Else
                        ss = Replace(ss, "$adate", "convert(date,d.adate)")
                    End If
                End If
            End If

            'For JV REPORT 
            If scrname.ToUpper = "JV REPORT" Or scrname.ToUpper = "APPROVED PETTY CASH VOUCHER HUB REPORT" Or scrname.ToUpper = "REJECTED PETTY CASH VOUCHER HUB REPORT" Then
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                Dim txtobjdate1 As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frfldtxtdate"), System.Web.UI.WebControls.TextBox)
                txtobjdate1 = TryCast(pnlFields.FindControl("FrfldtxtTodate"), System.Web.UI.WebControls.TextBox)
                If txtobjdate.Text <> "" And txtobjdate1.Text <> "" Then
                    ss = Replace(ss, "@Frtdate", "Convert(date,'" & txtobjdate.Text & "')")
                    ss = Replace(ss, "@Totdate", "Convert(date,'" & txtobjdate1.Text & "')")
                    ss = Replace(ss, "@adate", "convert(date,d.adate)")
                    ss = Replace(ss, "$adate", "convert(date,d.adate)")
                Else
                    ss = Replace(ss, "@Frtdate", "Convert(date,dd.tdate)")
                    ss = Replace(ss, "@Totdate", "Convert(date,dd.tdate)")
                    ss = Replace(ss, "@adate", "convert(date,d.adate)")
                    ss = Replace(ss, "$adate", "convert(date,d.adate)")
                End If
            ElseIf scrname.ToUpper = "PERIOD WISE BALANCES" Then
                Dim txtobjdate = TryCast(pnlFields.FindControl("Frfldtxtdate"), System.Web.UI.WebControls.TextBox)
                If txtobjdate.Text <> "" Then
                    Dim strArr = txtobjdate.Text.Split("-")
                    Dim mnth As String = Convert.ToString(strArr(0))
                    Dim yr As String = Convert.ToString(strArr(1))
                    ss = Replace(ss, "@mnth", mnth)
                    ss = Replace(ss, "@yr", yr)
                Else
                    ss = Replace(ss, "@mnth", 0)
                    ss = Replace(ss, "@yr", 0)
                End If
            End If

            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & Session("EID") & " and uid=" & Session("UID") & " and isview=1"
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab <> "0" Then
                ss = Replace(ss, "@uid", Session("UID"))
                ss = Replace(ss, "@role", "'" & Session("USERROLE") & "'")
                da.SelectCommand.CommandText = ss
                da.SelectCommand.CommandTimeout = 600
                da.Fill(ds, "ss")
                ViewState("xlexport") = ds.Tables("ss")
                Mydatatable = ds.Tables("ss")
                If ds.Tables("ss").Rows.Count > 0 Then
                    gvReport.DataSource = ds.Tables("ss")
                    gvReport.DataBind()
                    lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                    btnchart.Visible = True
                Else
                    btnchart.Visible = False
                    lblCaption.Text = "No Records Found.."
                    gvReport.Controls.Clear()
                    gvReport.CaptionAlign = TableCaptionAlign.Left
                End If
                con.Dispose()
                da.Dispose()
                btnchart.Visible = False
                Exit Sub
            End If
            If ((Session("USERROLE") <> "FCAGGN") And (Session("USERROLE") <> "SU") And (Session("USERROLE") <> "BNK") And (Session("USERROLE") <> "CADMIN") And (Session("USERROLE") <> "FCANHQ")) And (ab = 0) Then
                If Session("EID") = "42" Then
                Else
                    If ab <> 0 Then
                        lblCaption.Text = " Please Contact your Admin for authorization "
                        lblCaption.ForeColor = Color.Red
                        Exit Sub
                    End If
                End If
            End If
            da.SelectCommand.CommandText = ss
            da.SelectCommand.CommandTimeout = 900
            'da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(ds, "ss")

            ViewState("xlexport") = ds.Tables("ss")
            Mydatatable = ds.Tables("ss")
            If ds.Tables("ss").Rows.Count > 0 Then
                gvReport.DataSource = ds.Tables("ss")
                gvReport.DataBind()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                'gvReport.CaptionAlign = TableCaptionAlign.Left
                lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                btnchart.Visible = True
            Else
                btnchart.Visible = False
                lblCaption.Text = "No Records Found.."
                gvReport.Controls.Clear()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                gvReport.CaptionAlign = TableCaptionAlign.Left
            End If
        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
        End Try
        Session("MyDatatable") = Mydatatable
    End Sub
    'Date convertsion Code
    Shared Function getdate(ByVal dbt As String) As Date
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(1)
            mm = dtArr(0)
            yy = dtArr(2)
            Dim dt As Date
            Try
                dt = mm & "/" & dd & "/" & yy
                Return dt
            Catch ex As Exception
                Return Now.Date
            End Try
        Else
            Return Now.Date
        End If
    End Function
    'Filteration on Button click
    Protected Sub clicktoshow()
        gvReport.Visible = True
        pnlchart.Visible = False
        pngv.Visible = True

        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select qryfieldrole from mmm_mst_report where reportname ='" & scrname & "' and eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString

            Dim fldms As String = fldmapping()
            Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & Session("eid") & " and reportname= '" & scrname & "'", con)
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            'Commented By Komal on 14Nov2013
            'da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and fieldmapping in (" & fldms & ")"
            'Added By Komal on 14Nov2013
            If ViewState("IsViewOn") = 1 Then
                da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and displayname in (" & fldms & ")"
            Else
                da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and FieldMapping in (" & fldms & ")"
            End If
            'End
            da.Fill(ds, "flds")
            Dim ss As String = ""

            da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            ss = da.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            'Filter by Dynamic controls
            For Each row As DataRow In ds.Tables("flds").Rows
                Dim pqr As String = row.Item("fieldtype").ToString()
                Dim dtyp As String = row.Item("datatype").ToString()
                Dim fmap As String = row.Item("fieldmapping").ToString()
                Dim var As String = ""
                Dim var2 As String = ""
                Dim chklist As String = ""
                var = row.Item("fieldid").ToString()
                var2 = row.Item("fieldid").ToString()
                If dtyp.ToUpper() = "DATETIME" Then
                    var = "Frfld" & var
                    var2 = "Tofld" & var2
                ElseIf dtyp.ToUpper = "NUMERIC" Then
                    var = "fldR1" & var
                    var2 = "fldR2" & var2
                End If

                If pqr = "Text Box" Then
                    Dim txtobj As New System.Web.UI.WebControls.TextBox
                    Dim txtobj2 As New System.Web.UI.WebControls.TextBox
                    txtobj = TryCast(pnlFields.FindControl(var), System.Web.UI.WebControls.TextBox)
                    txtobj2 = TryCast(pnlFields.FindControl(var2), System.Web.UI.WebControls.TextBox)
                    Dim ct As String = "@" & fmap
                    Dim ct2 As String = "$" & fmap
                    If IsNothing(txtobj2) Then
                        If txtobj.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ' ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False Then
                                    'ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                                    ' ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                'ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                ss = Replace(ss, ct, "d." & fmap)
                                ' ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                            Else
                                ss = Replace(ss, ct, "d." & fmap)
                                'ss = Replace(ss, ct2, "d." & fmap)
                            End If

                        End If
                    Else
                        If txtobj.Text <> "" Or txtobj2.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False And IsDate(getdate(txtobj2.Text)) <> False Then
                                    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                            Else
                                ss = Replace(ss, ct, "d." & fmap)
                                ss = Replace(ss, ct2, "d." & fmap)
                            End If

                        End If
                    End If
                ElseIf pqr = "Drop Down" Then
                    Dim chkobj As New CheckBoxList
                    chkobj = TryCast(pnlFields.FindControl("chklist" & var), CheckBoxList)
                    Dim ct As String = "@" & fmap

                    For i As Integer = 0 To chkobj.Items.Count - 1
                        If chkobj.Items(i).Selected = True Then
                            chklist = chklist & chkobj.Items(i).Value & ","
                        End If
                    Next
                    If chklist.ToString = "" Then
                    Else
                        chklist = Left(chklist, chklist.Length - 1)
                    End If

                    If chklist.ToString <> "" Then
                        If dtyp.ToUpper() = "NUMERIC" Then
                            ss = Replace(ss, ct, chklist)
                        Else
                            If chkobj.SelectedItem.Text.ToUpper() = "SELECT" Then
                                ct = "=" & ct
                                ss = Replace(ss, ct, "LIKE '%%'")
                            Else
                                ss = Replace(ss, ct, chklist)
                            End If
                        End If
                    Else
                        ss = Replace(ss, ct, "d." & fmap)
                    End If

                End If
            Next
            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & Session("EID") & " and uid=" & Session("UID") & " and isview=1"
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab <> "0" Then
                da.SelectCommand.CommandText = "select qryfieldrole from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
                ss = da.SelectCommand.ExecuteScalar().ToString
                ''ss = da.SelectCommand.ExecuteScalar()
                ss = Replace(ss, "@uid", Session("UID"))
                ss = Replace(ss, "@role", "'" & Session("USERROLE") & "'")
                da.SelectCommand.CommandText = ss
                da.Fill(ds, "ss")
                ViewState("xlexport") = ds.Tables("ss")
                Mydatatable = ds.Tables("ss")
                If ds.Tables("ss").Rows.Count > 0 Then
                    gvReport.DataSource = ds.Tables("ss")
                    gvReport.DataBind()
                    'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                    'gvReport.CaptionAlign = TableCaptionAlign.Left
                    'lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                    btnchart.Visible = True
                Else
                    btnchart.Visible = False
                    lblCaption.Text = "No Records Found.."
                    gvReport.Controls.Clear()
                    ' gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                    gvReport.CaptionAlign = TableCaptionAlign.Left
                End If
                con.Dispose()
                da.Dispose()
                btnchart.Visible = False
                Exit Sub
            End If

            da.SelectCommand.CommandText = ss
            da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(ds, "ss")

            ViewState("xlexport") = ds.Tables("ss")
            Mydatatable = ds.Tables("ss")
            If ds.Tables("ss").Rows.Count > 0 Then
                gvReport.DataSource = ds.Tables("ss")
                gvReport.DataBind()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                'gvReport.CaptionAlign = TableCaptionAlign.Left
                lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                btnchart.Visible = True
            Else
                btnchart.Visible = False
                lblCaption.Text = "No Records Found.."
                gvReport.Controls.Clear()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                gvReport.CaptionAlign = TableCaptionAlign.Left
            End If
        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
        End Try
        Session("MyDatatable") = Mydatatable
    End Sub
    Protected Sub gvReport_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvReport.PageIndexChanged
        show()
    End Sub
    Protected Sub gvReport_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvReport.PageIndexChanging
        gvReport.PageIndex = e.NewPageIndex
    End Sub

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnExport.Click
        'Start Prashant30_12
        If gvReport.Rows.Count = 0 Then
            Exit Sub
        Else
            Pdf()
            Exit Sub
        End If
        'end Prashant30_12

        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        gvReport.AllowPaging = False
        gvReport.AllowSorting = False
        Dim da As New SqlDataAdapter("select qryfield from mmm_mst_report where reportname ='" & scrname & "' and eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim ss As String = da.SelectCommand.ExecuteScalar().ToString
            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & Session("EID") & " and uid=" & Session("UID") & ""
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab = "0" Then
                lblCaption.Text = "No Records Found.."
                Exit Sub
            End If
            ss = Replace(ss, "@uid", Session("UID"))
            ss = Replace(ss, "@role", "'" & Session("USERROLE") & "'")
            da.SelectCommand.CommandText = ss
            da.SelectCommand.ExecuteNonQuery()
            da.Fill(ds, "ss")

            ViewState("xlexport") = ds.Tables("ss")
            Mydatatable = ds.Tables("ss")
            If ds.Tables("ss").Rows.Count > 0 Then
                gvReport.DataSource = ds.Tables("ss")
                gvReport.DataBind()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                'gvReport.CaptionAlign = TableCaptionAlign.Left
                lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
            Else
                lblCaption.Text = "No Records Found.."
                gvReport.Controls.Clear()
                gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                gvReport.CaptionAlign = TableCaptionAlign.Left
            End If

            If ds.Tables("ss").Rows.Count > 0 Then
                bindreport()
                ToPdf(ds)
            Else
                lblMsg.Text = "No Record Found"
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
    End Sub
    'Export to PDF Report
    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Try
            Dim PDFData As New System.IO.MemoryStream()
            Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
            Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
            newDocument.Open()
            gvReport.AllowPaging = False
            gvReport.AllowSorting = False
            For Page As Integer = 0 To newDataSet.Tables.Count - 1
                Dim totalColumns As Integer = newDataSet.Tables(Page).Columns.Count
                Dim newPdfTable As New iTextSharp.text.pdf.PdfPTable(totalColumns)
                newPdfTable.DefaultCell.Padding = 4
                newPdfTable.WidthPercentage = 100
                newPdfTable.DefaultCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT
                newPdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE
                newPdfTable.HeaderRows = 1
                newPdfTable.DefaultCell.BorderWidth = 1
                newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
                newPdfTable.DefaultCell.BackgroundColor = New iTextSharp.text.BaseColor(255, 255, 255)

                For i As Integer = 0 To totalColumns - 1
                    newPdfTable.AddCell(New Phrase(newDataSet.Tables(Page).Columns(i).ColumnName, FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
                Next

                For Each record As DataRow In newDataSet.Tables(Page).Rows
                    For i As Integer = 0 To totalColumns - 1
                        newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
                        newPdfTable.AddCell(New Phrase(record(i).ToString, FontFactory.GetFont("Tahoma", 9, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80))))
                    Next
                Next
                Dim gif As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(MapPath("logo") & "\" & Session("CLOGO"))
                newDocument.Add(gif)
                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(New Phrase("Report Name: " & ViewState("pageheader"), FontFactory.GetFont("Tahoma", 12, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))

                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(newPdfTable)
                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(New Phrase(Environment.NewLine))

                newDocument.Add(New Phrase("Created By: " & Session("USERNAME"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(New Phrase("Printed Date: " & DateTime.Now.ToString(), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))

                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(New Phrase(Environment.NewLine))
                newDocument.Add(New Phrase("Company Address: " & ViewState("pagefooter"), FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))

                If Page < newDataSet.Tables.Count Then
                    newDocument.NewPage()
                End If
            Next
            newDocument.Close()
            Response.ContentType = "application/pdf"
            Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
            Response.AppendHeader("Content-Type", "application/pdf")
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & Request.QueryString("SC").ToString())
            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
            Response.OutputStream.Flush()
            Response.OutputStream.Close()
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Sub
    Protected Sub bindreport()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("usp_selectReport", con)
        Try
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
            da.SelectCommand.Parameters.AddWithValue("rpname", ViewState("RPTname"))
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ViewState("ReportName") = ds.Tables("data").Rows(0).Item("reportname").ToString()
            ViewState("pageheader") = ds.Tables("data").Rows(0).Item("pageheader").ToString()
            ViewState("pagefooter") = ds.Tables("data").Rows(0).Item("pagefooter").ToString()
        Catch ex As Exception
            Throw
        Finally
            con.Dispose()
        End Try
    End Sub
    'Export to Excel Report
    Protected Sub btnexcel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexcel.Click
        Try
            Dim scrname As String = Request.QueryString("SC").ToString()
            Dim ob As New MainUtility
            Response.Clear()
            gvReport.AllowPaging = False
            gvReport.AllowSorting = False
            Response.ContentType = "Application/x-msexcel"
            Response.AddHeader("content-disposition", "attachment;filename=" & scrname & ".csv")
            Response.Write(ob.ToCSV(Export()))
            Response.[End]()
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
    End Sub
    Private Function Export() As DataTable
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        gvReport.AllowPaging = False
        gvReport.AllowSorting = False
        Dim da As New SqlDataAdapter("select qryfield from mmm_mst_report where reportname ='" & scrname & "' and eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataTable
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim ss As String = da.SelectCommand.ExecuteScalar().ToString


            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & Session("EID") & " and uid=" & Session("UID") & ""
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab = "0" Then
                lblCaption.Text = "No Records Found.."
                Exit Function
            End If
            ss = Replace(ss, "@uid", Session("UID"))
            ss = Replace(ss, "@role", "'" & Session("USERROLE") & "'")
            da.SelectCommand.CommandText = ss
            da.SelectCommand.ExecuteNonQuery()
            da.Fill(ds)

            ViewState("xlexport") = ds
            If ds.Rows.Count > 0 Then
                gvReport.DataSource = ds
                gvReport.DataBind()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                'gvReport.CaptionAlign = TableCaptionAlign.Left
                lblCaption.Text = ds.Rows.Count & " Records Found..."
            Else
                lblCaption.Text = "No Records Found.."
                gvReport.Controls.Clear()
                gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Rows.Count & " Records Found...</td></tr></table>"
                gvReport.CaptionAlign = TableCaptionAlign.Left
            End If

            Return ds
        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
        End Try
    End Function
    Protected Sub Excelexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Excelexport.Click

        If (IsNothing(Session("MyDatatable"))) Then
            Exit Sub
        End If

        Dim dt As DataTable = DirectCast(Session("MyDatatable"), DataTable)
        Dim grid As New GridView()
        grid.DataSource = dt

        If dt.Rows.Count = 0 Then
            Exit Sub
        End If

        Try
            grid.AllowPaging = False
            grid.AllowSorting = False
            'grid.DataSource = ViewState("xlexport")
            grid.DataBind()
            '  GridView1.DataBind()
            Response.Clear()
            Response.Buffer = True
            Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & ViewState("ReportName") & "</h3></div> <br/>")
            Response.AddHeader("content-disposition", "attachment;filename=" & ViewState("ReportName") & ".xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            If IsNothing(ViewState("xlexport")) Then
            Else
                For i = 0 To grid.HeaderRow.Cells.Count - 1
                    If grid.HeaderRow.Cells(i).Text.ToUpper.Contains("AMOUNT") Or grid.HeaderRow.Cells(i).Text.ToUpper.Contains("TOTAL") Or grid.HeaderRow.Cells(i).Text.ToUpper.Contains("BALANCE") Or grid.HeaderRow.Cells(i).Text.ToUpper.Contains("IN PROCESS") Or grid.HeaderRow.Cells(i).Text.ToUpper.Contains("DEBIT") Or grid.HeaderRow.Cells(i).Text.ToUpper.Contains("CREDIT") Or grid.HeaderRow.Cells(i).Text.ToUpper.Contains("BUDGET") Then
                    Else
                        For j = 0 To grid.Rows.Count - 1
                            grid.Rows(j).Cells(i).Attributes.Add("class", "textmode")
                        Next
                    End If
                Next
            End If

            'For i As Integer = 0 To gvReport.Rows.Count - 1
            '    'Apply text style to each Row 
            '    gvReport.Rows(i).Attributes.Add("class", "textmode")
            'Next
            grid.AllowPaging = False
            grid.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        Catch ex As Exception

        Finally
        End Try
    End Sub
    Protected Sub btnchart_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnchart.Click
        Try
            pnlchart.Visible = True
            gvReport.Visible = False
            pngv.Visible = False
            pnlFields.Visible = False
        Catch ex As Exception
            lblMsg.Text = ex.Message
        End Try
    End Sub
    Protected Sub disply()
        Dim con As SqlConnection = Nothing
        Dim da1 As SqlDataAdapter = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            If ddlx.SelectedItem.Text = "Select" Then
                lblMsg.Text = "Please Select X "
                Exit Sub
            ElseIf ddly.SelectedItem.Text = "Select" Then
                lblMsg.Text = "Please Select Y"
                Exit Sub
            ElseIf ddlct.SelectedItem.Text = "Select" Then
                lblMsg.Text = "Please Select Chart Type"
                Exit Sub
            Else
                lblMsg.Text = ""
                pnlchart.Visible = True
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                con = New SqlConnection(conStr)
                con.Open()
                Dim ct As Integer = 0
                If ddlct.SelectedValue = 1 Then
                    ct = DataVisualization.Charting.SeriesChartType.Column

                ElseIf ddlct.SelectedValue = 2 Then
                    ct = DataVisualization.Charting.SeriesChartType.Pie
                    ch.Legends("Default").Enabled = True

                    ch.Series("Series1").IsValueShownAsLabel = True
                ElseIf ddlct.SelectedValue = 3 Then
                    ct = DataVisualization.Charting.SeriesChartType.Line

                ElseIf ddlct.SelectedValue = 4 Then
                    ct = DataVisualization.Charting.SeriesChartType.Area

                ElseIf ddlct.SelectedValue = 5 Then
                    ct = DataVisualization.Charting.SeriesChartType.Pyramid
                    ch.Series("Series1").IsValueShownAsLabel = True

                    ch.Legends("Default").Enabled = True
                ElseIf ddlct.SelectedValue = 6 Then
                    ct = DataVisualization.Charting.SeriesChartType.Radar

                    ch.Series("Series1").IsValueShownAsLabel = True
                ElseIf ddlct.SelectedValue = 7 Then
                    ct = DataVisualization.Charting.SeriesChartType.Bubble
                ElseIf ddlct.SelectedValue = 8 Then
                    ct = DataVisualization.Charting.SeriesChartType.BoxPlot
                ElseIf ddlct.SelectedValue = 9 Then
                    ct = DataVisualization.Charting.SeriesChartType.Candlestick
                ElseIf ddlct.SelectedValue = 10 Then
                    ct = DataVisualization.Charting.SeriesChartType.ErrorBar
                ElseIf ddlct.SelectedValue = 11 Then
                    ct = DataVisualization.Charting.SeriesChartType.Funnel
                ElseIf ddlct.SelectedValue = 12 Then
                    ct = DataVisualization.Charting.SeriesChartType.Kagi
                ElseIf ddlct.SelectedValue = 13 Then
                    ct = DataVisualization.Charting.SeriesChartType.Point
                ElseIf ddlct.SelectedValue = 14 Then
                    ct = DataVisualization.Charting.SeriesChartType.Polar
                ElseIf ddlct.SelectedValue = 15 Then
                    ct = DataVisualization.Charting.SeriesChartType.RangeColumn
                ElseIf ddlct.SelectedValue = 16 Then
                    ct = DataVisualization.Charting.SeriesChartType.ThreeLineBreak
                ElseIf ddlct.SelectedValue = 17 Then
                    ct = DataVisualization.Charting.SeriesChartType.Spline
                End If

                Dim scrname As String = Request.QueryString("SC").ToString()

                da1 = New SqlDataAdapter("select distinct  displayName,FieldMapping,dropdowntype,dropdown,mainentity  from MMM_MST_FIELDS f inner join MMM_MST_REPORT r on r.SubEntity =f.DocumentType  where DocumentType='" & ViewState("se") & "' and datatype<>'numeric' and f.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "'", con)
                Dim dt1 As New DataTable()
                da1.Fill(dt1)
                ddlx.DataSource = dt1
                Dim cmd As New SqlCommand
                Dim ssa As String = ""
                If ViewState("me") = "MASTER" Then
                    For i As Integer = 0 To dt1.Rows.Count - 1
                        If dt1.Rows(i).Item("DropDownType").ToString().ToUpper() = "MASTER VALUED" Then
                            Dim abc As String = dt1.Rows(i).Item("DropDown").ToString()
                            Dim xyz As String() = abc.ToString().Split("-")
                            ssa = "select  " & xyz(2) & " from mmm_mst_master where eid= " & Session("EID") & " and documenttype= '" & xyz(1) & "' and tid=m." & ddlx.SelectedValue & ""
                            cmd.CommandText = "select distinct (" & ssa & ")[" & ddlx.SelectedValue & "]," & ddly.SelectedValue & " from MMM_MST_REPORT R inner join mmm_mst_master m on m.DocumentType=r.SubEntity where m.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "' "
                        Else
                            cmd.CommandText = "select distinct " & ddlx.SelectedValue & "," & ddly.SelectedValue & " from MMM_MST_REPORT R inner join mmm_mst_master m on m.DocumentType=r.SubEntity where m.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "' "
                        End If
                    Next
                Else
                    For i As Integer = 0 To dt1.Rows.Count - 1
                        If dt1.Rows(i).Item("DropDownType").ToString().ToUpper() = "MASTER VALUED" Then
                            Dim abc As String = dt1.Rows(i).Item("DropDown").ToString()
                            Dim xyz As String() = abc.ToString().Split("-")
                            ssa = "select  " & xyz(2) & " from mmm_mst_master where eid= " & Session("EID") & " and documenttype= '" & xyz(1) & "' and tid=m." & ddlx.SelectedValue & ""
                            cmd.CommandText = "select distinct (" & ssa & ")[" & ddlx.SelectedValue & "]," & ddly.SelectedValue & " from MMM_MST_REPORT R inner join mmm_mst_master m on m.DocumentType=r.SubEntity where m.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "' "
                        Else
                            cmd.CommandText = "select distinct " & ddlx.SelectedValue & "," & ddly.SelectedValue & " from MMM_MST_REPORT R inner join mmm_mst_Doc m on m.DocumentType=r.SubEntity where m.EID=" & Session("EID").ToString() & " and r.reportName ='" & ViewState("RPTname") & "' "
                        End If
                    Next
                End If

                cmd.Connection = con
                da = New SqlDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                ch.DataSource = dt
                ch.Series("Series1").XValueMember = ddlx.SelectedValue
                ch.Series("Series1").YValueMembers = ddly.SelectedValue
                ' ch.Series("Series1").YValueMembers = ddlx.SelectedItem.Text
                'ct = ddlct.SelectedItem.Text
                ch.Series("Series1").ChartType = ct
                ch.ChartAreas("ChartArea1").AxisX.Title = ddlx.SelectedItem.Text
                ch.ChartAreas("ChartArea1").AxisY.Title = ddly.SelectedItem.Text
                ch.ChartAreas("ChartArea1").AxisX.Interval = 1
                'ch.Series("Series1").ChartType = DataVisualization.Charting.SeriesChartType.Column
                ch.DataBind()

            End If

        Catch ex As Exception
            lblMsg.Text = ex.Message
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not da1 Is Nothing Then
                da1.Dispose()
            End If
        End Try
    End Sub

    Protected Sub btns_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btns.Click
        disply()
    End Sub
    Protected Sub btnexpo_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexpo.Click
        Try
            Response.ContentType = "image/png"
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename=" & ddlct.SelectedItem.Text & ".png"))
            disply()
            ch.SaveImage(Response.OutputStream, ChartImageFormat.Png)
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvReport_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvReport.DataBound
        Try
            Dim scrname As String = Request.QueryString("SC").ToString()
            If scrname = "Business wise analyses of Vehicle Hire" Then
                gvReport.ShowFooter = True

                Dim row As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal)
                Dim cel As New TableHeaderCell()
                cel.Text = ""
                cel.ColumnSpan = 2
                row.Controls.Add(cel)


                cel = New TableHeaderCell()
                cel.Text = "For the month"
                cel.ColumnSpan = 3
                row.Controls.Add(cel)

                cel = New TableHeaderCell()
                cel.Text = "YTD (period)"
                cel.ColumnSpan = 3
                row.Controls.Add(cel)


                Dim row1 As New GridViewRow(1, 1, DataControlRowType.Header, DataControlRowState.Normal)
                Dim cell As New TableHeaderCell()
                cell.Text = ""
                cell.ColumnSpan = 2
                row1.Controls.Add(cell)


                cell = New TableHeaderCell()
                cell.Text = "Incoices / Bills of Vendors"
                cell.ColumnSpan = 2
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "% of Claims"
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "Incoices / Bills of Vendors"
                cell.ColumnSpan = 2
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "% of Claims"
                row1.Controls.Add(cell)

                row.BackColor = ColorTranslator.FromHtml("#D4CFD1")
                row1.BackColor = ColorTranslator.FromHtml("#D4CFD1")
                gvReport.HeaderRow.Parent.Controls.AddAt(0, row)
                gvReport.HeaderRow.Parent.Controls.AddAt(1, row1)

            ElseIf scrname = "Circle wise analyses of Vehicle Hire (consolidated)" Then
                gvReport.ShowFooter = True
                Dim row As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal)
                Dim cel As New TableHeaderCell()
                cel.Text = ""
                cel.ColumnSpan = 2
                row.Controls.Add(cel)
                cel = New TableHeaderCell()
                cel.Text = "For the month"
                cel.ColumnSpan = 3
                row.Controls.Add(cel)

                cel = New TableHeaderCell()
                cel.Text = "YTD (period)"
                cel.ColumnSpan = 3
                row.Controls.Add(cel)

                Dim row1 As New GridViewRow(1, 1, DataControlRowType.Header, DataControlRowState.Normal)
                Dim cell As New TableHeaderCell()
                cell.Text = ""
                cell.ColumnSpan = 2
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "Incoices / Bills of Vendors"
                cell.ColumnSpan = 2
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "% of Claims"
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "Incoices / Bills of Vendors"
                cell.ColumnSpan = 2
                row1.Controls.Add(cell)

                cell = New TableHeaderCell()
                cell.Text = "% of Claims"
                row1.Controls.Add(cell)

                row.BackColor = ColorTranslator.FromHtml("#D4CFD1")
                row1.BackColor = ColorTranslator.FromHtml("#D4CFD1")
                gvReport.HeaderRow.Parent.Controls.AddAt(0, row)
                gvReport.HeaderRow.Parent.Controls.AddAt(1, row1)
            End If
        Catch ex As Exception

        Finally
        End Try
    End Sub
    Dim tot As Decimal
    Protected Sub gvReport_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvReport.RowDataBound
        Try
            Dim scrname As String = Request.QueryString("SC").ToString()
            If e.Row.RowType = DataControlRowType.DataRow Then
                If e.Row.Cells(1).Text = "Total" Then
                    e.Row.BackColor = ColorTranslator.FromHtml("#D4CFD1")
                    e.Row.Font.Bold = True
                End If
            End If
            If scrname = "Invoice Status Tracker Report" Then
                If e.Row.RowType = DataControlRowType.DataRow Then
                    e.Row.Cells(12).HorizontalAlign = HorizontalAlign.Right
                End If
            End If
            If scrname = "Pending Invoice Tracker Report" Then
                If e.Row.RowType = DataControlRowType.DataRow Then
                    e.Row.Cells(7).HorizontalAlign = HorizontalAlign.Right
                End If
            End If
        Catch ex As Exception

        Finally
        End Try
    End Sub
    <WebMethod>
    Public Shared Function ConvertDataTabletoString() As String
        Try
            Dim Sb As New StringBuilder()
            Dim Data As DataTable = Mydatatable
            Dim str As String = ""
            Dim geopoint As String = ""
            Dim geofence As String = ""
            Dim mtch As String = ""
            If Data.Rows.Count > 0 Then
                For i As Integer = 0 To Data.Rows.Count - 1
                    If Data.Rows(i).Item("GeoFence").ToString.Trim <> "" And IsDBNull(Data.Rows(i).Item("GeoFence").ToString) = False Then
                        Dim arr = Data.Rows(i).Item("GeoFence").ToString.Split(",")
                        Dim var1 = ""
                        If mtch <> Data.Rows(i).Item("GeoFence").ToString.Trim Then
                            For a = 0 To arr.Count - 1
                                If a < arr.Count - 1 Then
                                    If var1 = "" Then
                                        var1 = arr(a + 1) & " " & arr(a)
                                    Else
                                        var1 = var1 & "," & arr(a + 1) & " " & arr(a)
                                    End If
                                    a = a + 1
                                End If
                            Next
                            mtch = Data.Rows(i).Item("GeoFence").ToString.Trim
                            If geofence.Contains(var1) = False Then
                                geofence = geofence & "#" & var1
                            End If
                        End If
                        'Sb.Append("#").Append(var1)
                    End If
                    If Data.Rows(i).Item("GeoPoint").ToString.Trim <> "" And Data.Rows(i).Item("GeoPoint").ToString.Contains("Error") = False Then
                        geopoint = geopoint & "['Product Name: " & Data.Rows(i).Item("Product Name").ToString & "<br>Advisor Code: " & Data.Rows(i).Item("Advisor Code").ToString & "'," & Data.Rows(i).Item("GeoPoint").ToString & ",'images/human.png'],"
                    End If
                Next
                'geopoint =  geopoint 
                If geopoint <> "" Then
                    Sb.Append(geopoint).Append("|").Append(geofence)
                End If
            End If
            str = Sb.ToString()
            'Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "jhdgh", "bindMap()")
            Return str
        Catch ex As Exception

        Finally
        End Try
    End Function
    Protected Sub btnViewInExcel_Click(sender As Object, e As EventArgs) Handles btnViewInExcel.Click
        Try
            If Request.QueryString("SC") = "Pending Invoice Against VRF Fixed Pool" Then
                Dim str As String = ShowVrfReport()
                If str = "True" Then
                    dvmsg.InnerHtml = "Report has been sent to your registered e-mail ID."
                End If
            Else
                Dim grdExport As New GridView()
                show()
                gvReport.AllowPaging = False
                gvReport.AllowSorting = False
                gvReport.DataSource = ViewState("xlexport")
                gvReport.DataBind()
                Response.Clear()
                Response.Buffer = True
                Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & ViewState("ReportName") & "</h3></div> <br/>")
                Response.AddHeader("content-disposition", "attachment;filename=" & ViewState("ReportName") & ".xls")
                Response.Charset = ""
                Response.ContentType = "application/vnd.ms-excel"
                Dim sw As New StringWriter()
                Dim hw As New HtmlTextWriter(sw)
                If IsNothing(ViewState("xlexport")) Then
                Else
                    For i = 0 To gvReport.HeaderRow.Cells.Count - 1
                        If gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("AMOUNT") Or gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("TOTAL") Or gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("BALANCE") Or gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("IN PROCESS") Or gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("DEBIT") Or gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("CREDIT") Or gvReport.HeaderRow.Cells(i).Text.ToUpper.Contains("BUDGET") Then
                        Else
                            For j = 0 To gvReport.Rows.Count - 1
                                gvReport.Rows(j).Cells(i).Attributes.Add("class", "textmode")
                            Next
                        End If
                    Next
                End If

                'For i As Integer = 0 To gvReport.Rows.Count - 1
                '    'Apply text style to each Row 
                '    gvReport.Rows(i).Attributes.Add("class", "textmode")
                'Next
                gvReport.AllowPaging = False
                gvReport.RenderControl(hw)
                'style to format numbers to string 
                Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
                Response.Write(style)
                Response.Output.Write(sw.ToString())
                Response.Flush()
                Response.End()
                gvReport.DataSource = Nothing
                gvReport.DataBind()
            End If
        Catch ex As Exception

        Finally
        End Try
    End Sub
    Function CreateCSV(ByVal dt As DataTable, ByVal path As String) As String
        'Create CSV File here
        Try
            Dim fname As String = path & Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & ".CSV"
            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
            If File.Exists(path & fname) Then
                File.Delete(path & fname)
            End If
            Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
            sw.Flush()
            'First we will write the headers.
            Dim iColCount As Integer = dt.Columns.Count
            For i As Integer = 0 To iColCount - 1
                sw.Write(dt.Columns(i))
                If (i < iColCount - 1) Then
                    sw.Write(",")
                End If
            Next
            sw.Write(sw.NewLine)
            ' Now write all the rows.
            Dim dr As DataRow
            For Each dr In dt.Rows
                For i As Integer = 0 To iColCount - 1
                    If Not Convert.IsDBNull(dr(i)) Then
                        sw.Write(dr(i).ToString)
                    End If
                    If (i < iColCount - 1) Then
                        sw.Write(",")
                    End If
                Next
                sw.Write(sw.NewLine)
            Next
            sw.Close()
            Return fname
        Catch ex As Exception
            Return ""
            Throw
        Finally
        End Try
    End Function
    Function ShowVrfReport() As String
        Dim fm As String = fldmapping()
        'ScriptManager.RegisterStartupScript(Me, [GetType](), "showalert", "alert('Report sent to your Email.');", True)
        Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim qry As String = "Select FieldID,displayName, FieldType, FieldMapping,datatype from mmm_mst_fields where eid=32 and"
        qry &= " Documenttype='VRF Fixed_Pool' and fieldMapping in (" & fm & ")"
        Dim da As New SqlDataAdapter(qry, con)
        Dim dt As New DataTable
        Try
            da.Fill(dt)
            Dim txtstrt As New System.Web.UI.WebControls.TextBox
            Dim txtend As New System.Web.UI.WebControls.TextBox
            Dim fldmappingVrf As String = ""
            Dim txtobjdate As New System.Web.UI.WebControls.TextBox
            Dim txtobjdate1 As New System.Web.UI.WebControls.TextBox
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim ftype As String = dt.Rows(i).Item("FieldType")
                Dim dtype As String = dt.Rows(i).Item("Datatype")
                If dt.Rows(i).Item("DisplayName") = "REQUISITION NO." Then
                    fldmappingVrf = dt.Rows(i).Item("FieldMapping")
                End If
                txtobjdate = TryCast(pnlFields.FindControl("Frflddate"), System.Web.UI.WebControls.TextBox)
                txtobjdate1 = TryCast(pnlFields.FindControl("Toflddate"), System.Web.UI.WebControls.TextBox)
            Next
            If txtstrt IsNot Nothing Then
                qry = "Select qryField from MMM_MST_REPORT where ReportName='" & Request.QueryString("SC") & "' and Eid=32"
                da.SelectCommand.CommandText = qry
                Dim dtq As New DataTable
                da.Fill(dtq)
                Dim str As String = dtq.Rows(0).Item(0).ToString
                Dim dtVrf As New DataTable
                Dim dtData As New DataTable
                If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Then
                    da.SelectCommand.CommandText = "Select distinct v.fld1[VRF] from mmm_mst_doc v with (nolock) where v.eid=32 and v.documenttype='VRF Fixed_Pool' and convert(date,v.fld17,3)>=convert(date,'" & txtobjdate.Text & "',3) and convert(date,v.fld19,3)<=convert(date,'" & txtobjdate1.Text & "',3) and v.curstatus in ('allotted','surrender','archive')"
                Else
                    da.SelectCommand.CommandText = "Select distinct v.fld1[VRF] from mmm_mst_doc v with (nolock) where v.eid=32 and v.documenttype='VRF Fixed_Pool' and convert(date,v.fld17,3)>=convert(date,'" & txtobjdate.Text & "',3) and convert(date,v.fld19,3)<=convert(date,'" & txtobjdate1.Text & "',3) and v.fld16 in (select * from inputstring((select fld4 from mmm_ref_role_user with(nolock) where uid =" & Session("UID") & " and rolename='" & Session("USERROLE") & "'))) and v.curstatus in ('allotted','surrender','archive')"
                End If
                da.Fill(dtVrf)
                dtData.Clear()
                If dtVrf.Rows.Count > 0 Then
                    For i As Integer = 0 To dtVrf.Rows.Count - 1
                        Dim strNew = str
                        strNew = strNew.Replace("@" & fldmappingVrf, "'" & dtVrf.Rows(i).Item("VRF").ToString() & "'")
                        strNew = strNew.Replace("@uid", Session("UID"))
                        strNew = strNew.Replace("@role", Session("USERROLE"))
                        da.SelectCommand.CommandText = strNew
                        'da.SelectCommand.CommandTimeout = 120
                        da.Fill(dtData)
                    Next
                End If
                Dim filename = CreateCSV(dtData, "VRF BillPending Report")
                Dim fpath As String = HostingEnvironment.MapPath("~/MailAttach/")
                da.SelectCommand.CommandText = "select emailID from mmm_mst_user with (nolock)  where eid=" & Session("EID") & " and uid=" & Session("UID")
                Dim dtmail As New DataTable
                da.Fill(dtmail)
                Dim obj As New MailUtill(eid:=32)
                'da.SelectCommand.CommandText = "select emailid from mmm_mst_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " "
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                ' Dim em As String = "Vishal.kumar@myndsol.com"
                'Dim em As String = da.SelectCommand.ExecuteScalar.ToString()
                'obj.SendMail(ToMail:="Vishal.kumar@myndsol.com", Subject:=mailsub, MailBody:=msg, CC:="sachin.madaan@myndsol.com", Attachments:=FPath + fname, BCC:=Bcc)
                obj.SendMail(ToMail:=dtmail.Rows(0).Item(0).ToString, Subject:="Pending Invoice Against VRF Fixed Pool", MailBody:="<Div>Dear Sir,</Div><BR><BR><Div>Please find attached Pending Invoice Against VRF Fixed Pool.</Div><BR><BR><div>Regards</div><div>R4G Vehicle Management System Team</div></div>", CC:="", Attachments:=fpath + filename, BCC:="")
                'File.Delete(fpath)
                'lblMail.Text = "Report has been sent to your EmailID."
            End If
            Return "True"
        Catch ex As Exception
            Return "False"
            Throw
        Finally
            con.Close()
            da.Dispose()
        End Try
    End Function
    Private Sub Pdf()
        gvReport.Visible = True
        pnlchart.Visible = False
        pngv.Visible = True

        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select qryfieldrole from mmm_mst_report where reportname ='" & scrname & "' and eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString

            Dim fldms As String = fldmapping()
            Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & Session("eid") & " and reportname= '" & scrname & "'", con)
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and fieldmapping in (" & fldms & ") order by FieldMapping desc"
            da.Fill(ds, "flds")
            Dim ss As String = ""
            If Session("USERROLE") = "SU" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            Else
                da.SelectCommand.CommandText = "select qryfieldrole  from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & Session("EID") & ""
            End If

            da.SelectCommand.CommandTimeout = 600
            ss = da.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            'Filter by Dynamic controls
            For Each row As DataRow In ds.Tables("flds").Rows
                Dim pqr As String = row.Item("fieldtype").ToString()
                Dim dtyp As String = row.Item("datatype").ToString()
                Dim fmap As String = row.Item("fieldmapping").ToString()
                Dim var As String = ""
                Dim var2 As String = ""
                Dim chklist As String = ""
                var = row.Item("fieldid").ToString()
                var2 = row.Item("fieldid").ToString()
                If dtyp.ToUpper() = "DATETIME" Then
                    var = "Frfld" & var
                    var2 = "Tofld" & var2
                ElseIf dtyp.ToUpper = "NUMERIC" Then
                    var = "fldR1" & var
                    var2 = "fldR2" & var2
                ElseIf pqr = "Text Box" And dtyp.ToUpper = "TEXT" Then
                    var = "fld" & var
                End If

                If pqr = "Text Box" Then
                    Dim txtobj As New System.Web.UI.WebControls.TextBox
                    Dim txtobj2 As New System.Web.UI.WebControls.TextBox
                    txtobj = TryCast(pnlFields.FindControl(var), System.Web.UI.WebControls.TextBox)
                    txtobj2 = TryCast(pnlFields.FindControl(var2), System.Web.UI.WebControls.TextBox)
                    Dim ct As String = "@" & fmap
                    Dim ct2 As String = "$" & fmap
                    If IsNothing(txtobj2) Then
                        If txtobj.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ' ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False Then
                                    'ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ss = Replace(ss, ct, "'" & txtobj.Text & "'")
                                    ' ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%" & Trim(txtobj.Text) & "%'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                'ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                ss = Replace(ss, ct, "d." & fmap)
                                ' ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                            ElseIf dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, "d." & fmap)
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%%'")
                                'ss = Replace(ss, ct, "d." & fmap)
                                'ss = Replace(ss, ct2, "d." & fmap)
                            End If

                        End If
                    Else
                        If txtobj.Text <> "" Or txtobj2.Text <> "" Then
                            If dtyp.ToUpper() = "NUMERIC" Then
                                ss = Replace(ss, ct, txtobj.Text)
                                ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf dtyp.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(txtobj.Text)) <> False And IsDate(getdate(txtobj2.Text)) <> False Then
                                    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    lblCaption.Text = "  * Please Select a valid date"
                                    lblCaption.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            Else
                                ct = "=" & ct
                                ss = Replace(ss, ct, " like '%" & Trim(txtobj.Text) & "%'")
                            End If
                        Else
                            If dtyp.ToUpper() = "DATETIME" Then
                                ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                            Else
                                ss = Replace(ss, ct, "d." & fmap)
                                ss = Replace(ss, ct2, "d." & fmap)
                            End If

                        End If
                    End If

                ElseIf pqr = "Drop Down" Then
                    Dim chkobj As New CheckBoxList
                    chkobj = TryCast(pnlFields.FindControl("chklist" & var), CheckBoxList)
                    Dim ct As String = "@" & fmap

                    For i As Integer = 0 To chkobj.Items.Count - 1
                        If chkobj.Items(i).Selected = True Then
                            chklist = chklist & "'" & chkobj.Items(i).Value & "',"
                        End If
                    Next
                    If chklist.ToString = "" Then
                    Else
                        chklist = Left(chklist, chklist.Length - 1)
                    End If

                    If chklist.ToString <> "" Then
                        If dtyp.ToUpper() = "NUMERIC" Then
                            ss = Replace(ss, ct, chklist)
                        Else
                            If chkobj.SelectedItem.Text.ToUpper() = "SELECT" Then
                                ct = "=" & ct
                                ss = Replace(ss, ct, "LIKE '%%'")
                            Else
                                ss = Replace(ss, ct, chklist)
                            End If
                        End If
                    Else
                        ss = Replace(ss, ct, "d." & fmap)
                    End If
                End If
            Next

            If xy.ToUpper = "HUB MASTER" Then
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frfldtxtdate"), System.Web.UI.WebControls.TextBox)
                If txtobjdate.Text <> "" Then
                    ss = Replace(ss, "@adate", "Convert(date,'" & txtobjdate.Text & "')")
                Else
                    ss = Replace(ss, "@adate", "Convert(date,getdate())")
                End If
            Else
                Dim txtobjdate As New System.Web.UI.WebControls.TextBox
                Dim txtobjdate1 As New System.Web.UI.WebControls.TextBox
                txtobjdate = TryCast(pnlFields.FindControl("Frflddate"), System.Web.UI.WebControls.TextBox)
                txtobjdate1 = TryCast(pnlFields.FindControl("Toflddate"), System.Web.UI.WebControls.TextBox)
                If IsNothing(txtobjdate) Then
                Else
                    If txtobjdate.Text <> "" Then
                        ss = Replace(ss, "@adate", "Convert(date,'" & txtobjdate.Text & "')")
                    Else
                        ss = Replace(ss, "@adate", "convert(date,d.adate)")
                    End If
                    If txtobjdate1.Text <> "" Then
                        ss = Replace(ss, "$adate", "Convert(date,'" & txtobjdate1.Text & "')")
                    Else
                        ss = Replace(ss, "$adate", "convert(date,d.adate)")
                    End If
                End If
            End If

            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & Session("EID") & " and uid=" & Session("UID") & " and isview=1"
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab <> "0" Then
                ss = Replace(ss, "@uid", Session("UID"))
                ss = Replace(ss, "@role", "'" & Session("USERROLE") & "'")
                da.SelectCommand.CommandText = ss
                da.SelectCommand.CommandTimeout = 600
                da.Fill(ds, "ss")
                ViewState("xlexport") = ds.Tables("ss")
                Mydatatable = ds.Tables("ss")
                ds.Tables.Remove("flds")
                If ds.Tables("ss").Rows.Count > 0 Then
                    bindreport()
                    ToPdf(ds)
                Else
                    lblMsg.Text = "No Record Found"
                End If

                If ds.Tables("ss").Rows.Count > 0 Then
                    gvReport.DataSource = ds.Tables("ss")
                    gvReport.DataBind()
                    lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                    btnchart.Visible = True
                Else
                    btnchart.Visible = False
                    lblCaption.Text = "No Records Found.."
                    gvReport.Controls.Clear()
                    gvReport.CaptionAlign = TableCaptionAlign.Left
                End If
                con.Dispose()
                da.Dispose()
                btnchart.Visible = False
                Exit Sub
            End If
            If ((Session("USERROLE") <> "FCAGGN") And (Session("USERROLE") <> "SU") And (Session("USERROLE") <> "BNK") And (Session("USERROLE") <> "CADMIN") And (Session("USERROLE") <> "FCANHQ")) And (ab = 0) Then
                If Session("EID") = "42" Then
                Else
                    If ab <> 0 Then
                        lblCaption.Text = " Please Contact your Admin for authorization "
                        lblCaption.ForeColor = Color.Red
                        Exit Sub
                    End If
                End If
            End If
            da.SelectCommand.CommandText = ss
            da.SelectCommand.CommandTimeout = 600
            'da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(ds, "ss")

            ViewState("xlexport") = ds.Tables("ss")
            Mydatatable = ds.Tables("ss")
            ds.Tables.Remove("flds")
            If ds.Tables("ss").Rows.Count > 0 Then
                bindreport()
                ToPdf(ds)
            Else
                lblMsg.Text = "No Record Found"
            End If


            If ds.Tables("ss").Rows.Count > 0 Then
                ' gvReport.DataSource = ds.Tables("ss")
                'gvReport.DataBind()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;""> &nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                'gvReport.CaptionAlign = TableCaptionAlign.Left
                lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                btnchart.Visible = True
            Else
                btnchart.Visible = False
                lblCaption.Text = "No Records Found.."
                gvReport.Controls.Clear()
                'gvReport.Caption = "<table><tr><td style=""color:Navy;"">&nbsp;&nbsp;" & ds.Tables("ss").Rows.Count & " Records Found...</td></tr></table>"
                gvReport.CaptionAlign = TableCaptionAlign.Left
            End If

        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
        End Try
        Session("MyDatatable") = Mydatatable

    End Sub


    <WebMethod()> _
          <Script.Services.ScriptMethod()> _
    Public Shared Function GetData(json As Dictionary(Of String, Object)) As DGrid
        Try
            If json("IsView").ToString = "0" Then
                Return showViewOffReport1(json)
            Else
                Return ShowViewsOnReport1(json)
            End If
        Catch ex As Exception

        End Try
    End Function


    Protected Shared Function showViewOffReport1(data As Dictionary(Of String, Object)) As DGrid

        Dim scrname As String = HttpUtility.UrlDecode(data("SC").ToString)
        'scrname = Replace(scrname, "%20", " ")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim grid As New DGrid()
        Dim strError As String = ""

        Dim da As New SqlDataAdapter("select qryfieldrole from mmm_mst_report where reportname ='" & scrname & "' and eid=" & HttpContext.Current.Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString

            Dim fldms As String = fldmapping(scrname)
            Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & HttpContext.Current.Session("eid") & " and reportname= '" & scrname & "'", con)
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='" & xy & "' and fieldmapping in (" & fldms & ") order by FieldMapping desc"
            da.Fill(ds, "flds")
            Dim ReportQuery As String = ""
            If HttpContext.Current.Session("USERROLE") = "SU" Or HttpContext.Current.Session("USERROLE") = "BNK" Or HttpContext.Current.Session("USERROLE") = "FCAGGN" Or HttpContext.Current.Session("USERROLE") = "CADMIN" Or HttpContext.Current.Session("USERROLE") = "FCANHQ" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & HttpContext.Current.Session("EID") & ""
            Else
                da.SelectCommand.CommandText = "select qryfieldrole  from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & HttpContext.Current.Session("EID") & ""
            End If

            da.SelectCommand.CommandTimeout = 600
            ReportQuery = da.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            'Filter by Dynamic controls
            For Each row As DataRow In ds.Tables("flds").Rows
                Dim fldType As String = row.Item("fieldtype").ToString()
                Dim datatype As String = row.Item("datatype").ToString()
                Dim fldmapping As String = row.Item("fieldmapping").ToString()
                Dim FieldID As String = row.Item("FieldID").ToString()

                Dim Key1 As String = ""
                Dim Key2 As String = ""

                If datatype.ToUpper() = "DATETIME" Then
                    Key1 = "fr_" & fldmapping & "_" & FieldID
                    Key2 = "to_" & fldmapping & "_" & FieldID
                ElseIf datatype.ToUpper = "NUMERIC" Then
                    Key1 = "Frfld" & fldmapping & "_" & FieldID
                    Key2 = "Tofld" & fldmapping & "_" & FieldID
                ElseIf fldType = "Text Box" And datatype.ToUpper = "TEXT" Then
                    Key1 = fldmapping & "_" & FieldID
                End If

                If fldType = "Text Box" Then
                    Dim ct As String = "@" & fldmapping
                    Dim ct2 As String = "$" & fldmapping

                    If Not data.ContainsKey(Key2) Then
                        If Not data.ContainsKey(Key1) Then
                            If datatype.ToUpper() = "NUMERIC" Then
                                ReportQuery = Replace(ReportQuery, ct, data(Key1).ToString)
                                ' ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf datatype.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(data(Key1).ToString)) <> False Then
                                    'ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    ReportQuery = Replace(ReportQuery, ct, "'" & data(Key1).ToString & "'")
                                    ' ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    strError &= "  * Please Select a valid date"
                                End If
                            Else
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, " like '%" & Trim(data(Key1).ToString) & "%'")
                            End If
                        Else
                            If datatype.ToUpper() = "DATETIME" Then
                                ReportQuery = Replace(ReportQuery, ct, "d." & fldmapping)
                            ElseIf datatype.ToUpper() = "NUMERIC" Then
                                ReportQuery = Replace(ReportQuery, ct, "d." & fldmapping)
                            Else
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, " like '%%'")
                            End If

                        End If
                    Else
                        If data.ContainsKey(Key1) Or data.ContainsKey(Key2) Then
                            If datatype.ToUpper() = "NUMERIC" Then
                                ReportQuery = Replace(ReportQuery, ct, data.ContainsKey(Key1))
                                ReportQuery = Replace(ReportQuery, ct2, data.ContainsKey(Key2))
                            ElseIf datatype.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(data.ContainsKey(Key1))) <> False And IsDate(getdate(data.ContainsKey(Key2).ToString)) <> False Then

                                    If data(Key1).ToString = "" Then
                                        ReportQuery = Replace(ReportQuery, ct, "d." & Replace(ct, "@", "") & "")
                                    End If
                                    If data(Key2).ToString = "" Then
                                        ReportQuery = Replace(ReportQuery, ct2, "d." & Replace(ct2, "$", "") & "")
                                    End If
                                    ReportQuery = Replace(ReportQuery, ct, "Convert(datetime,'" & data(Key1).ToString & "',3)")
                                    ReportQuery = Replace(ReportQuery, ct2, "Convert(datetime,'" & data(Key2).ToString & "',3)")
                                Else
                                    strError &= "  * Please Select a valid date"
                                End If
                            Else
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, " like '%" & Trim(data(Key1).ToString) & "%'")
                            End If
                        Else
                            If datatype.ToUpper() = "DATETIME" Then
                                ReportQuery = Replace(ReportQuery, ct, "convert(datetime,d." & fldmapping & ",3)")
                                ReportQuery = Replace(ReportQuery, ct2, "convert(datetime,d." & fldmapping & ",3)")
                            Else
                                ReportQuery = Replace(ReportQuery, ct, "d." & fldmapping)
                                ReportQuery = Replace(ReportQuery, ct2, "d." & fldmapping)
                            End If

                        End If
                    End If
                ElseIf fldType = "Drop Down" Then
                    'Dim chkobj As New CheckBoxList
                    ' chkobj = TryCast(pnlFields.FindControl("chklist" & var), CheckBoxList)
                    Dim ct As String = "@" & fldmapping
                    Key1 = fldmapping & "_" & FieldID
                    Dim arrData = DirectCast(data(Key1), Array)
                    Dim chklist = ""
                    For i As Integer = 0 To arrData.Length - 1
                        chklist = chklist & "'" & arrData(i) & "',"
                    Next
                    If chklist.ToString = "" Then
                    Else
                        chklist = Left(chklist, chklist.Length - 1)
                    End If

                    If chklist.ToString <> "" Then
                        If datatype.ToUpper() = "NUMERIC" Then
                            ReportQuery = Replace(ReportQuery, ct, chklist)
                        Else
                            'If chkobj.SelectedItem.Text.ToUpper() = "SELECT" Then
                            '    ct = "=" & ct
                            '    ss = Replace(ss, ct, "LIKE '%%'")
                            'Else
                            ReportQuery = Replace(ReportQuery, ct, chklist)
                            ' End If
                        End If
                    Else
                        ReportQuery = Replace(ReportQuery, ct, "d." & fldmapping)
                    End If
                End If
            Next

            If xy.ToUpper = "HUB MASTER" Then

                Dim key1 = "Frfldtxtdate"

                If data(key1).ToString <> "" Then
                    ReportQuery = Replace(ReportQuery, "@adate", "Convert(date,'" & data(key1).ToString & "')")
                Else
                    ReportQuery = Replace(ReportQuery, "@adate", "Convert(date,getdate())")
                End If
            Else

                Dim key1 = "Frflddate"
                Dim key2 = "Toflddate"

                If Not data.ContainsKey(key1) Then

                Else
                    If data(key1).ToString.Trim <> "" Then
                        ReportQuery = Replace(ReportQuery, "@adate", "Convert(date,'" & data(key1).ToString & "')")
                    Else
                        ReportQuery = Replace(ReportQuery, "@adate", "convert(date,d.adate)")
                    End If
                    If data(key2).ToString.Trim <> "" Then
                        ReportQuery = Replace(ReportQuery, "$adate", "Convert(date,'" & data(key2).ToString.Trim & "')")
                    Else
                        ReportQuery = Replace(ReportQuery, "$adate", "convert(date,d.adate)")
                    End If
                End If
            End If

            'For JV REPORT 
            If scrname.ToUpper = "JV REPORT" Or scrname.ToUpper = "APPROVED PETTY CASH VOUCHER HUB REPORT" Or scrname.ToUpper = "REJECTED PETTY CASH VOUCHER HUB REPORT" Then

                Dim key1 = "Frfldtxtdate"
                Dim key2 = "FrfldtxtTodate"

                If data(key1).ToString.Trim <> "" And data(key2).ToString <> "" Then
                    ReportQuery = Replace(ReportQuery, "@Frtdate", "Convert(date,'" & data(key1).ToString.Trim & "')")
                    ReportQuery = Replace(ReportQuery, "@Totdate", "Convert(date,'" & data(key2).ToString.Trim & "')")
                    ReportQuery = Replace(ReportQuery, "@adate", "convert(date,d.adate)")
                    ReportQuery = Replace(ReportQuery, "$adate", "convert(date,d.adate)")
                Else
                    ReportQuery = Replace(ReportQuery, "@Frtdate", "Convert(date,dd.tdate)")
                    ReportQuery = Replace(ReportQuery, "@Totdate", "Convert(date,dd.tdate)")
                    ReportQuery = Replace(ReportQuery, "@adate", "convert(date,d.adate)")
                    ReportQuery = Replace(ReportQuery, "$adate", "convert(date,d.adate)")
                End If
            ElseIf scrname.ToUpper = "PERIOD WISE BALANCES" Then

                Dim key1 = "Frfldtxtdate"
                Dim key2 = "FrfldtxtTodate"

                If data(key1).ToString.Trim <> "" Then
                    Dim strArr = data(key1).ToString.Trim
                    Dim mnth As String = Convert.ToString(strArr(0))
                    Dim yr As String = Convert.ToString(strArr(1))
                    ReportQuery = Replace(ReportQuery, "@mnth", mnth)
                    ReportQuery = Replace(ReportQuery, "@yr", yr)
                Else
                    ReportQuery = Replace(ReportQuery, "@mnth", 0)
                    ReportQuery = Replace(ReportQuery, "@yr", 0)
                End If
            End If

            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and isview=1"
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab <> "0" Then
                ReportQuery = Replace(ReportQuery, "@uid", HttpContext.Current.Session("UID"))
                ReportQuery = Replace(ReportQuery, "@role", "'" & HttpContext.Current.Session("USERROLE") & "'")
                da.SelectCommand.CommandText = ReportQuery
                da.SelectCommand.CommandTimeout = 600
                da.Fill(ds, "ss")
                ' ViewState("xlexport") = ds.Tables("ss")
                Mydatatable = ds.Tables("ss")
                If ds.Tables("ss").Rows.Count > 0 Then
                    'gvReport.DataSource = ds.Tables("ss")
                    'gvReport.DataBind()
                    'lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                    'btnchart.Visible = True
                Else
                    'btnchart.Visible = False
                    'lblCaption.Text = "No Records Found.."
                    'gvReport.Controls.Clear()
                    'gvReport.CaptionAlign = TableCaptionAlign.Left
                End If
                'con.Dispose()
                'da.Dispose()
                ' btnchart.Visible = False

            End If
            If ((HttpContext.Current.Session("USERROLE") <> "FCAGGN") And (HttpContext.Current.Session("USERROLE") <> "SU") And (HttpContext.Current.Session("USERROLE") <> "BNK") And (HttpContext.Current.Session("USERROLE") <> "CADMIN") And (HttpContext.Current.Session("USERROLE") <> "FCANHQ")) And (ab = 0) Then
                If HttpContext.Current.Session("EID") = "42" Then
                Else
                    If ab <> 0 Then
                        strError = " Please Contact your Admin for authorization "

                    End If
                End If
            End If
            da.SelectCommand.CommandText = ReportQuery
            da.SelectCommand.CommandTimeout = 900
            'da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(ds, "ss")

            Mydatatable = ds.Tables("ss")

            If ds.Tables("ss").Rows.Count > 0 Then

            Else
                strError = "No data found"
            End If

            grid = DynamicGrid.GridData(ds.Tables("ss"), strError)

            grid.Chart = GetChartData(ds.Tables("ss"))

        Catch ex As Exception
            grid = DynamicGrid.GridData(New DataTable(), "Error occured at server please contact your system administrator.")
        Finally
            con.Close()
            da.Dispose()
        End Try
        HttpContext.Current.Session("MyDatatable") = Mydatatable
        Return grid
    End Function

    Protected Shared Function ShowViewsOnReport1(data As Dictionary(Of String, Object)) As DGrid

        Dim grid As New DGrid()
        Dim scrname As String = HttpUtility.UrlDecode(data("SC").ToString)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim strErr As String = ""
        Dim da As New SqlDataAdapter("select qryfieldrole from mmm_mst_report where reportname ='" & scrname & "' and eid=" & HttpContext.Current.Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim str As String = da.SelectCommand.ExecuteScalar().ToString
            Dim fldms As String
            ' If ViewState("IsViewOn") = 1 Then
            fldms = Viewfldmapping(scrname)
            'Else
            '    fldms = fldmapping()
            'End If

            Dim abc As SqlDataAdapter = New SqlDataAdapter("select subentity from MMM_MST_REPORT where eid=" & HttpContext.Current.Session("eid") & " and reportname= '" & scrname & "'", con)
            Dim xy As String = abc.SelectCommand.ExecuteScalar().ToString
            'If ViewState("IsViewOn") = 1 Then
            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='" & xy & "' and displayname in (" & fldms & ")"
            'Else
            '    da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & xy & "' and FieldMapping in (" & fldms & ")"
            'End If
            da.Fill(ds, "flds")
            Dim ReportQuery As String = ""
            If HttpContext.Current.Session("USERROLE") = "SU" Or HttpContext.Current.Session("USERROLE") = "BNK" Or HttpContext.Current.Session("USERROLE") = "FCAGGN" Or HttpContext.Current.Session("USERROLE") = "CADMIN" Or HttpContext.Current.Session("USERROLE") = "FCANHQ" Then
                da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & HttpContext.Current.Session("EID") & ""
            Else
                da.SelectCommand.CommandText = "select qryfieldrole  from mmm_mst_report where subentity ='" & xy & "' and reportname= '" & scrname & "' and eid=" & HttpContext.Current.Session("EID") & ""
            End If

            da.SelectCommand.CommandTimeout = 600
            ReportQuery = da.SelectCommand.ExecuteScalar().ToString
            Dim sb As New StringBuilder()

            'Filter by Dynamic controls
            For Each row As DataRow In ds.Tables("flds").Rows
                Dim fldType As String = row.Item("fieldtype").ToString()
                Dim datatype As String = row.Item("datatype").ToString()
                Dim fldmapping As String = row.Item("fieldmapping").ToString()
                Dim FieldID As String = row.Item("FieldID").ToString()
                Dim DsplyName As String = row.Item("DisplayName").ToString()

                Dim Key1 As String = ""
                Dim Key2 As String = ""

                If datatype.ToUpper() = "DATETIME" Then
                    Key1 = "fr_" & fldmapping & "_" & FieldID
                    Key2 = "to_" & fldmapping & "_" & FieldID
                ElseIf datatype.ToUpper = "NUMERIC" Then
                    Key1 = "Frfld" & fldmapping & "_" & FieldID
                    Key2 = "Tofld" & fldmapping & "_" & FieldID
                ElseIf datatype = "Text Box" And datatype.ToUpper = "TEXT" Then
                    Key1 = fldmapping & "_" & FieldID
                End If


                If fldType = "Text Box" Then

                    Dim ct As String = "[" & "@" & DsplyName & "]"
                    Dim ct2 As String = "[" & "$" & DsplyName & "]"
                    If Not data.ContainsKey(Key2) Then
                        If data(Key1) <> "" Then
                            If datatype.ToUpper() = "NUMERIC" Then
                                ReportQuery = Replace(ReportQuery, ct, data(Key1))
                                ' ss = Replace(ss, ct2, txtobj2.Text)
                            ElseIf datatype.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(data(Key1))) <> False Then
                                    ReportQuery = Replace(ReportQuery, ct, "'" & data(Key1) & "'")
                                    ' ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                Else
                                    strErr = "  * Please Select a valid date"
                                End If
                            Else
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, " like '%" & Trim(data(Key1)) & "%'")
                            End If
                        Else
                            If datatype.ToUpper() = "DATETIME" Then
                                If IsDate(getdate(data(Key1))) <> False Then
                                    'If ViewState("IsViewOn") = 1 Then

                                    ReportQuery = Replace(ReportQuery, ct, Replace(ct, "@", ""))
                                    ReportQuery = Replace(ReportQuery, ct, Replace(ct, "$", ""))
                                    'Else
                                    '    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    '    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                    'End If
                                Else
                                    strErr = "  * Please Select a valid date"

                                End If
                            Else
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, "like '%%'")
                            End If
                        End If
                    Else
                        If data(Key1) <> "" Or data(Key2) <> "" Then
                            If datatype.ToUpper() = "NUMERIC" Then
                                ReportQuery = Replace(ReportQuery, ct, data(Key1))
                                ReportQuery = Replace(ReportQuery, ct2, data(Key2))
                            ElseIf datatype.ToUpper() = "DATETIME" Then

                                If IsDate(getdate(data(Key1))) <> False And IsDate(getdate(data(Key2))) <> False Then
                                    'If ViewState("IsViewOn") = 1 Then
                                    ReportQuery = Replace(ReportQuery, ct, "'" & data(Key1) & "'")
                                    ReportQuery = Replace(ReportQuery, ct2, "'" & data(Key2) & "'")
                                    'Else
                                    '    ss = Replace(ss, ct, "Convert(datetime,'" & txtobj.Text & "',3)")
                                    '    ss = Replace(ss, ct2, "Convert(datetime,'" & txtobj2.Text & "',3)")
                                    'End If
                                Else
                                    strErr = "  * Please Select a valid date"

                                End If
                            Else
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, " like '%%'")
                            End If
                        Else
                            If datatype.ToUpper() = "DATETIME" Then
                                'If ViewState("IsViewOn") = 1 Then
                                ReportQuery = Replace(ReportQuery, ct, Replace(ct, "@", ""))
                                ReportQuery = Replace(ReportQuery, ct2, Replace(ct2, "$", ""))
                                'Else
                                '    ss = Replace(ss, ct, "convert(datetime,d." & fmap & ",3)")
                                '    ss = Replace(ss, ct2, "convert(datetime,d." & fmap & ",3)")
                                'End If
                            Else
                                ReportQuery = Replace(ReportQuery, ct, Replace(ct, "@", ""))
                                ReportQuery = Replace(ReportQuery, ct2, Replace(ct2, "@", ""))
                            End If
                        End If
                    End If
                ElseIf fldType = "Drop Down" Then
                    Key1 = fldmapping & "_" & FieldID
                    Dim arrData = DirectCast(data(Key1), Array)
                    Dim chklist = ""
                    For i As Integer = 0 To arrData.Length - 1
                        chklist = chklist & "'" & arrData(i) & "',"
                    Next
                    Dim ct As String = "[" & "@" & DsplyName & "]"

                    If chklist.ToString = "" Then
                    Else
                        chklist = Left(chklist, chklist.Length - 1)
                    End If
                    If chklist.ToString <> "" Then
                        If datatype.ToUpper() = "NUMERIC" Then
                            ReportQuery = Replace(ReportQuery, ct, chklist)
                        Else
                            If chklist.ToUpper() = "SELECT" Then
                                ct = "=" & ct
                                ReportQuery = Replace(ReportQuery, ct, "LIKE '%%'")
                            Else
                                ReportQuery = Replace(ReportQuery, ct, chklist)
                            End If
                        End If
                    Else
                        ReportQuery = Replace(ReportQuery, ct, "EXFP." & DsplyName)
                    End If
                End If
            Next
            If xy.ToUpper = "HUB MASTER" Then

                Dim key1 = "Frfldtxtdate"

                If data(key1).ToString.Trim <> "" Then
                    ReportQuery = Replace(ReportQuery, "@adate", "Convert(date,'" & data(key1).ToString.Trim & "')")
                Else
                    ReportQuery = Replace(ReportQuery, "@adate", "Convert(date,getdate())")
                End If
            Else

                Dim key1 = "Frflddate"
                Dim key2 = "Toflddate"

                If Not data.ContainsKey(key1) Then
                Else
                    If data(key1).ToString.Trim <> "" Then
                        ReportQuery = Replace(ReportQuery, "@adate", "Convert(date,'" & data(key1).ToString.Trim & "')")
                    Else
                        ReportQuery = Replace(ReportQuery, "@adate", "convert(date,d.adate)")
                    End If
                    If data(key2).ToString.Trim <> "" Then
                        ReportQuery = Replace(ReportQuery, "$adate", "Convert(date,'" & data(key2).ToString.Trim & "')")
                    Else
                        ReportQuery = Replace(ReportQuery, "$adate", "convert(date,d.adate)")
                    End If
                End If
            End If
            da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where eid= " & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and isview=1"
            Dim ab As Integer = da.SelectCommand.ExecuteScalar()
            If ab <> "0" Then
                ReportQuery = Replace(ReportQuery, "[@uid]", HttpContext.Current.Session("UID"))
                ReportQuery = Replace(ReportQuery, "[@rolename]", "'" & HttpContext.Current.Session("USERROLE") & "'")
                da.SelectCommand.CommandText = ReportQuery
                da.SelectCommand.CommandTimeout = 900
                da.Fill(ds, "ss")
                'ViewState("xlexport") = ds.Tables("ss")
                Mydatatable = ds.Tables("ss")
                If ds.Tables("ss").Rows.Count > 0 Then
                    'gvReport.DataSource = ds.Tables("ss")
                    'gvReport.DataBind()
                    'lblCaption.Text = ds.Tables("ss").Rows.Count & " Records Found..."
                    'btnchart.Visible = True
                Else
                    'btnchart.Visible = False
                    'lblCaption.Text = "No Records Found.."
                    'gvReport.Controls.Clear()
                    'gvReport.CaptionAlign = TableCaptionAlign.Left
                End If
                'btnchart.Visible = False
                'Exit Function
            End If
            If ((HttpContext.Current.Session("USERROLE") <> "FCAGGN") And (HttpContext.Current.Session("USERROLE") <> "SU") And (HttpContext.Current.Session("USERROLE") <> "BNK") And (HttpContext.Current.Session("USERROLE") <> "CADMIN") And (HttpContext.Current.Session("USERROLE") <> "FCANHQ")) And (ab = 0) Then
                If HttpContext.Current.Session("EID") = "42" Then
                Else
                    strErr = " Please Contact your Admin for authorization "

                End If
            End If
            da.SelectCommand.CommandText = ReportQuery
            da.SelectCommand.CommandTimeout = 900
            da.SelectCommand.ExecuteNonQuery()
            ds.Clear()
            da.Fill(ds, "ss")

            ' ViewState("xlexport") = ds.Tables("ss")
            Mydatatable = ds.Tables("ss")

            grid = DynamicGrid.GridData(ds.Tables("ss"), strErr)

        Catch ex As Exception
            grid = DynamicGrid.GridData(New DataTable(), "Error occured at server please contact your system administrator.")
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
        HttpContext.Current.Session("MyDatatable") = Mydatatable
        Return grid
    End Function


    Public Shared Function GetChartData(dt As DataTable) As String
        Dim strData As New StringBuilder()
        Try
            If dt.Rows.Count = 0 Then
                Return strData.ToString()
            End If
            Dim dv As New DataView()
            dv = dt.DefaultView()


            Dim dtCustomer = dv.ToTable(True, "Customer")
            For i As Integer = 0 To dtCustomer.Rows.Count - 1
                Dim customer As String = Convert.ToString(dtCustomer.Rows(i).Item(0)).ToUpper()
                Dim per As Integer = 0
                Dim Notper As Integer = 0
                For j As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(j).Item("Customer").ToString().ToUpper() = customer Then
                        If dt.Rows(j).Item("Action").ToString().ToUpper() = "PERFORMED" Then
                            per = per + 1
                        ElseIf dt.Rows(j).Item("Action").ToString().ToUpper() = "NOT PERFORMED" Then
                            Notper = Notper + 1
                        End If
                    End If
                Next
                strData.Append(customer & "," & per & "," & Notper & "|")
            Next

            strData.Append("==")

            Dim dtClient = dv.ToTable(True, "Client")
            For i As Integer = 0 To dtClient.Rows.Count - 1
                Dim client As String = Convert.ToString(dtClient.Rows(i).Item(0)).ToUpper()
                Dim per As Integer = 0
                Dim Notper As Integer = 0
                For j As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(j).Item("Client").ToString().ToUpper() = client Then
                        If dt.Rows(j).Item("Action").ToString().ToUpper() = "PERFORMED" Then
                            per = per + 1
                        ElseIf dt.Rows(j).Item("Action").ToString().ToUpper() = "NOT PERFORMED" Then
                            Notper = Notper + 1
                        End If
                    End If
                Next
                strData.Append(client & "," & per & "," & Notper & "|")
            Next

            strData.Append("==")

            Dim dtSites = dv.ToTable(True, "Sites")
            For i As Integer = 0 To dtSites.Rows.Count - 1
                Dim site As String = Convert.ToString(dtSites.Rows(i).Item(0)).ToUpper()
                Dim per As Integer = 0
                Dim Notper As Integer = 0
                For j As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(j).Item("Sites").ToString().ToUpper() = site Then
                        If dt.Rows(j).Item("Action").ToString().ToUpper() = "PERFORMED" Then
                            per = per + 1
                        ElseIf dt.Rows(j).Item("Action").ToString().ToUpper() = "NOT PERFORMED" Then
                            Notper = Notper + 1
                        End If
                    End If
                Next
                strData.Append(site & "," & per & "," & Notper & "|")
            Next
            strData.Append("==")

            Dim dtAct = dv.ToTable(True, "Act_Category")
            For i As Integer = 0 To dtAct.Rows.Count - 1
                Dim Act As String = Convert.ToString(dtAct.Rows(i).Item(0)).ToUpper()
                Dim per As Integer = 0
                Dim Notper As Integer = 0
                For j As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(j).Item("Act_Category").ToString().ToUpper() = Act Then
                        If dt.Rows(j).Item("Action").ToString().ToUpper() = "PERFORMED" Then
                            per = per + 1
                        ElseIf dt.Rows(j).Item("Action").ToString().ToUpper() = "NOT PERFORMED" Then
                            Notper = Notper + 1
                        End If
                    End If
                Next
                strData.Append(Act & "," & per & "," & Notper & "|")
            Next

            strData.Append("==")

            Dim dtContractor = dv.ToTable(True, "Contractor_Name")
            For i As Integer = 0 To dtContractor.Rows.Count - 1
                Dim Contractor As String = Convert.ToString(dtContractor.Rows(i).Item(0)).ToUpper()
                Dim per As Integer = 0
                Dim Notper As Integer = 0
                For j As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(j).Item("Contractor_Name").ToString().ToUpper() = Contractor Then
                        If dt.Rows(j).Item("Action").ToString().ToUpper() = "PERFORMED" Then
                            per = per + 1
                        ElseIf dt.Rows(j).Item("Action").ToString().ToUpper() = "NOT PERFORMED" Then
                            Notper = Notper + 1
                        End If
                    End If
                Next
                strData.Append(Contractor & "," & per & "," & Notper & "|")
            Next


        Catch ex As Exception

        End Try
        Return strData.ToString()
    End Function

End Class
