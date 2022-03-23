Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports AjaxControlToolkit
Partial Class BulkApproval
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                BindDocumentType()
                'Hidding 
                If Not Session("AppData") Is Nothing Then
                    Session("AppData") = Nothing
                    gvData.DataSource = Nothing
                    gvData.DataBind()
                    If ddlDocumentType.Items.Count() = 2 Then
                        ddlDocumentType.SelectedIndex = 1
                        ddlDocumentType_SelectedIndexChanged(ddlDocumentType, New EventArgs())
                        'Up1.Update()
                    End If
                    'pnlData.Visible = False
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
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
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Session("AppData") Is Nothing Then
            Try
                CreateDynamicControls()
                Dim ds As New DataSet
                ds = DirectCast(Session("AppData"), DataSet)
                bindGrid(ds)
            Catch ex As Exception

            End Try

        End If
    End Sub

    Protected Sub BindDocumentType()

        Dim ds As New DataSet()
        Dim li As New ListItem("--Select Document--", 0)
        Try
            Dim EID As Integer, UID As Integer
            EID = Convert.ToInt32(Session("EID"))
            UID = Convert.ToInt32(Session("UID"))
            Dim obj As New BulkApprovalBAL()
            ds = obj.GetBulkApprovalDocType(EID, UID)
            If ds.Tables(0).Rows.Count > 0 Then
                ddlDocumentType.DataSource = ds
                ddlDocumentType.DataValueField = "DocumentType"
                ddlDocumentType.DataTextField = "DocumentType"
                ddlDocumentType.DataBind()

            End If
            ddlDocumentType.Items.Insert(0, li)
            If ddlDocumentType.Items.Count() = 2 Then
                ddlDocumentType.AutoPostBack = True
                ddlDocumentType.SelectedIndex = 1
                ddlDocumentType_SelectedIndexChanged(ddlDocumentType, New EventArgs())
                Up1.Update()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub GetFormField(EID As Integer, DocumentType As String, ActionFormName As String)
        Dim li As New ListItem("--Select--", 0)
        Try
            Dim obj As New BulkApprovalBAL()
            Dim ds As New DataSet()

            ds = obj.GetFormField(EID, DocumentType, ActionFormName)
            If ds.Tables(0).Rows.Count > 0 Then
                ddlFieldName.DataSource = ds
                ddlFieldName.DataTextField = "DisplayName"
                ddlFieldName.DataValueField = "FieldMapping"
                ddlFieldName.DataBind()
            End If
            ddlFieldName.Items.Insert(0, li)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlWF_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlWF.SelectedIndexChanged
        If ddlWF.SelectedItem.Text.ToUpper.Contains("SELECT") = False Then
            ScriptManager.RegisterClientScriptBlock(TryCast(btnSubmit, Control), Me.GetType(), "alert", "ValidateForm()", True)
            'ClientScript.RegisterStartupScript(Me.GetType(), "client click", "ValidateForm()", True)
            btnSubmit_Click(btnSubmit, New EventArgs())
        End If
    End Sub
    Protected Sub ddlDocumentType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentType.SelectedIndexChanged
        Dim li As New ListItem("--Select--", 0)
        Dim ds As New DataSet()
        Try
            'hiding the binded grid 
            gvData.DataSource = Nothing
            gvData.DataBind()
            'gvReport.DataSource = Nothing
            'gvReport.DataBind()
            'pnlData.Visible = False
            Dim EID As Integer, UID As Integer
            UID = Convert.ToInt32(Session("UID"))
            Dim Documenttype = ""
            EID = Convert.ToInt32(Session("EID"))
            Documenttype = ddlDocumentType.SelectedValue.ToString()
            ddlWF.Items.Clear()
            ddlWF.DataBind()
            Dim obj As New BulkApprovalBAL()
            If (Documenttype <> "0") Then
                ds = obj.GetBulkApprovalWF(EID, Documenttype, UID)
                If ds.Tables(0).Rows.Count > 0 Then
                    ddlWF.DataSource = ds
                    ddlWF.DataValueField = "Curstatus"
                    ddlWF.DataTextField = "Curstatus"
                    ddlWF.DataBind()
                    ddlWF.Items.Insert(0, li)
                    If ddlWF.Items.Count() = 2 Then
                        ddlWF.SelectedIndex = 1
                        ddlWF_SelectedIndexChanged(ddlWF, New EventArgs())
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim ds As New DataSet()
        lblMessage.Text = ""
        Label2.Text = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim obj As New BulkApprovalBAL()
        Dim dsFields As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'pnlData.Visible = False
        Try
            gvData.DataSource = Nothing
            gvData.DataBind()
            'gvReport.DataSource = Nothing
            'gvReport.DataBind()
            Dim EID As Integer
            Dim Documenttype = "", WFStatus = ""
            EID = Convert.ToInt32(Session("EID"))
            Dim UID = Convert.ToInt32(Session("UID"))
            Documenttype = ddlDocumentType.SelectedValue.ToString()
            Dim ActionForm = ""
            WFStatus = ddlWF.SelectedItem.Text
            If (Documenttype <> "0" And WFStatus <> "0") Then
                ds = obj.GetActionFormNameBA(EID, Documenttype, WFStatus)
                Dim strDocType = "'" & Documenttype & "'"
                If (ds.Tables(0).Rows.Count > 0) Then
                    strDocType = strDocType & ",'" & ds.Tables(0).Rows(0).Item("FormName") & "'"
                    ActionForm = ds.Tables(0).Rows(0).Item("FormName")
                End If
                Dim strQuery = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where  F.EID=" & EID & " AND  FF.DocumentType in (" & strDocType & ") and Invisible=0  and showinbulkapproval=1 order by Documenttype, FF.DisplayOrder"
                con = New SqlConnection(conStr)
                Dim cmd As New SqlCommand()
                cmd.CommandText = strQuery
                cmd.CommandType = CommandType.Text
                cmd.CommandTimeout = 180
                cmd.Connection = con
                da = New SqlDataAdapter(cmd)
                da.Fill(dsFields)
                Dim onlyFiltered As DataView = dsFields.Tables(0).DefaultView
                onlyFiltered.RowFilter = "DocumentType='" & Documenttype & "'"
                Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

                'Filtering name of Action Form
                Dim theFlds1 As New DataTable()
                If (ds.Tables(0).Rows.Count > 0) Then
                    onlyFiltered.RowFilter = "DocumentType='" & ds.Tables(0).Rows(0).Item("FormName") & "' AND invisible='0' "
                    theFlds1 = onlyFiltered.Table.DefaultView.ToTable()
                    'theFlds1 = theFlds1.Select("1=1", "displayorder")
                    Session("tblActionField") = theFlds1
                    Session("ActionField") = theFlds1
                Else
                    Session("tblActionField") = Nothing
                End If

                Dim Sb As New StringBuilder()

                Dim StrViewName = "[" & "V" & EID & Documenttype.Replace(" ", "_") & "] D"

                If theFlds1.Rows.Count > 0 Then
                    onlyFiltered.RowFilter = "DocumentType='" & ds.Tables(0).Rows(0).Item("FormName") & "' AND  IsEditable=1 AND invisible='0'"
                    theFlds1 = onlyFiltered.Table.DefaultView.ToTable()
                    Sb.Append(GenerateQuery(theFlds1, Sb))
                    onlyFiltered.RowFilter = "DocumentType='" & ds.Tables(0).Rows(0).Item("FormName") & "' AND IsEditable=0 AND invisible='0'"
                    theFlds1 = onlyFiltered.Table.DefaultView.ToTable()
                    Sb.Append(GenerateQuery(theFlds1, Sb))
                End If

                If theFlds.Rows.Count > 0 Then
                    Sb.Append(GenerateQuery(theFlds, Sb))
                End If
                Dim StrQuery1 = Sb.ToString.Substring(1, Sb.ToString.Length - 1).Replace("<", "").Replace(">", "")
                Dim strCountREcord = Sb.ToString.Substring(1, Sb.ToString.Length - 1).Replace("<", "").Replace(">", "")
                strCountREcord = "SELECT count(D.tid) As 'DOCID'FROM [V32Expense_Claim_Fixed_Pool] D INNER JOIN MMM_DOC_DTL DTL ON D.LastTID=DTL.tid WHERE D.EID=" & EID & " AND DTL.userid=" & UID & " and D.DocumentType='" & Documenttype & "' AND D.Curstatus='" & WFStatus & "'"
                Dim dscount As New DataSet()
                Dim dacount As New SqlDataAdapter(strCountREcord, con)
                dacount.Fill(dscount)
                Dim strcount As String = dscount.Tables(0).Rows(0)("DOCID").ToString()

                StrQuery1 = "SELECT ''AS 'Check ALL',D.tid As 'DOCID', " & StrQuery1 & "FROM " & StrViewName

                StrQuery1 = StrQuery1 & " INNER JOIN MMM_DOC_DTL DTL ON D.LastTID=DTL.tid WHERE D.EID=" & EID & " AND DTL.userid=" & UID & " and D.DocumentType='" & Documenttype & "' AND D.Curstatus='" & WFStatus & "'"
                'StrQuery1 = StrQuery1 & " INNER JOIN MMM_DOC_DTL DTL ON D.LastTID=DTL.tid WHERE D.EID=" & EID & " AND DTL.userid=" & UID & " and D.DocumentType='" & Documenttype & "' AND D.Curstatus='" & WFStatus & "' order by D.tid desc"
                ViewState("Query") = StrQuery1
                StrQuery1 = StrQuery1 & "order by Priority,D.tid desc"

                Dim dsData As New DataSet()
                cmd.CommandText = StrQuery1
                cmd.CommandType = CommandType.Text
                cmd.CommandTimeout = 1080
                cmd.Connection = con
                da = New SqlDataAdapter(cmd)
                da.Fill(dsData)
                bindGrid(dsData)
                Session("AppData1") = dsData

                Label2.Text = "Total Records : " & dsData.Tables(0).Rows.Count
                'if action field show file uploader
                'Dim tbl As New DataTable()=CType(Session("tblActionField"),DataTable)
                Dim tbl As DataTable
                pnlUploader.Visible = False
                If (Not Session("tblActionField") Is Nothing) Then
                    tbl = DirectCast(Session("tblActionField"), DataTable)
                    If tbl.Rows.Count > 0 Then
                        pnlUploader.Visible = True
                    Else
                        pnlUploader.Visible = False
                    End If
                End If
                'Binding field dropdown 
                Try
                    GetFormField(EID, Documenttype, ActionForm)
                Catch ex As Exception
                End Try
            End If
        Catch ex As Exception
            lblMessage.Text = "Sorry!!! Your request can not be completed at the moment.Error occured at server."
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        'Creating Dynamic Controls
        Try
            CreateDynamicControls()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub CreateDynamicControls()
        Try
            If (Not Session("ActionField") Is Nothing) Then
                Dim dt As New DataTable()
                dt = CType(Session("ActionField"), DataTable)
                Dim onlyFiltered As DataView = dt.DefaultView
                onlyFiltered.RowFilter = "IsEditable=1 AND invisible='0'"
                Dim dtField As DataTable
                dtField = onlyFiltered.ToTable
                Dim ob As New BulkApprovalBAL()
                Session("tblActionFieldCopy") = dtField
                pnlFields.Controls.Clear()
                If (dtField.Rows.Count > 0) Then
                    ob.CreateControlsOnPanel(dtField, pnlFields, UpDynamicFields, btnCopy)
                    Dim ROW1() As DataRow = dtField.Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
                    If ROW1.Length > 0 Then
                        For i As Integer = 0 To ROW1.Length - 1
                            Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                            DDL.AutoPostBack = True
                            AddHandler DDL.TextChanged, AddressOf bindvalue1
                        Next
                    End If
                    If (",L1_Approver,L2_Approver,L3_Approver,L4_Approver,GRN,L5_Approver").ToString().ToUpper.Contains("," & Session("USERROLE").ToString().ToUpper & ",") Then
                        pnlfieldTop.Visible = False
                    Else
                        pnlfieldTop.Visible = True
                    End If
                Else
                    pnlfieldTop.Visible = False
                    Session("ActionField") = Nothing
                End If
            End If
        Catch ex As Exception
            Throw New Exception("Error details:Bulk Approval:CreateDynamicControls" & ex.Message)
        End Try
    End Sub

    Public Shared Function GetPostBackControl(ByVal page As Page) As Control
        Dim control As Control = Nothing

        Dim ctrlname As String = page.Request.Params.[Get]("__EVENTTARGET")
        If ctrlname IsNot Nothing AndAlso ctrlname <> String.Empty Then
            control = page.FindControl(ctrlname)
        Else
            For Each ctl As String In page.Request.Form
                Dim c As Control = page.FindControl(ctl)
                If TypeOf c Is System.Web.UI.WebControls.Button Then
                    control = c
                    Exit For
                ElseIf TypeOf c Is System.Web.UI.WebControls.ImageButton Then
                    control = c
                    Exit For
                End If
            Next
        End If
        Return control
    End Function

    Public Sub bindvalue1(ByVal sender As Object, ByVal e As EventArgs)
        Dim c As Control = GetPostBackControl(Me.Page)
        '...
        If c IsNot Nothing Then
        End If
        If TypeOf c Is System.Web.UI.WebControls.DropDownList Then
            Dim ddl As DropDownList = TryCast(c, DropDownList)
            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim ob As New DynamicForm()
            ob.bind(id, pnlFields, ddl)
        End If
    End Sub

    Protected Sub bindGrid(dsData As DataSet)
        Try

            If dsData.Tables(0).Rows.Count > 0 Then
                gvData.DataSource = dsData.Tables(0)
                gvData.DataBind()
                Label2.Text = ""
                lblDataCount.Text = dsData.Tables(0).Rows.Count
                lblDataCount.Visible = False

                'pnlData.Visible = True
                Session("AppData") = dsData
                pnlUploader.Visible = True
                divgvData.Visible = True
                dvControl.Visible = True
                pnlgvData.Visible = True
                Up1.Update()
            Else
                gvData.DataSource = Nothing
                gvData.DataBind()
                Session("AppData") = Nothing
                'pnlData.Visible = False
                lblMessage.Text = "No record found."
                pnlUploader.Visible = False
                divgvData.Visible = False
                pnlgvData.Visible = False
                dvControl.Visible = False
                Up1.Update()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function GenerateQuery(theFlds1 As DataTable, Sb1 As StringBuilder) As String
        Dim da As SqlDataAdapter = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim Sb As New StringBuilder()
        'Sb.Append(Sb1.ToString())
        Try
            Dim EID = Session("EID")

            If theFlds1.Rows.Count > 0 Then
                For i As Integer = 0 To theFlds1.Rows.Count - 1
                    If (theFlds1.Rows(i).Item("FieldType") = "Drop Down" Or theFlds1.Rows(i).Item("FieldType") = "AutoComplete") And theFlds1.Rows(i).Item("DropDownType") = "MASTER VALUED" Then
                        Dim arr = theFlds1.Rows(i).Item("DropDown").ToString().Split("-")
                        Dim str = ""
                        Dim str1 = ""
                        If arr(1).ToUpper = "USER" Then
                            str1 = "(SELECT isnull([" & arr(2) & "],'') from MMM_MST_USER s WHERE CAST(s.[UID] AS VARCHAR)=CAST(D.[" & theFlds1.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & theFlds1.Rows(i).Item("DisplayName") & "]"
                        Else
                            str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & EID & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(str, con)
                            Dim ds1 As New DataSet()
                            da.Fill(ds1)
                            str1 = "(SELECT isnull([" & ds1.Tables(0).Rows(0).Item("DisplayName") & "],'')  from [V" & EID & arr(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(D.[" & theFlds1.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & theFlds1.Rows(i).Item("DisplayName") & "]"
                        End If
                        If Sb1.ToString().Contains("<" & str1 & ">") Then
                        Else
                            Sb.Append(",").Append("<" & str1 & ">")
                        End If
                    ElseIf theFlds1.Rows(i).Item("DropDownType").ToUpper = "SESSION VALUED" Then
                        Sb.Append(",").Append("(SELECT username FROM mmm_mst_user WITH(nolock) WHERE  CONVERT(VARCHAR, uid) = D.[" & theFlds1.Rows(i).Item("DisplayName") & "])AS[" & theFlds1.Rows(i).Item("DisplayName") & "]")
                    Else
                        If Sb1.ToString().Contains("<isnull(D.[" & theFlds1.Rows(i).Item("DisplayName") & "],'')'" & theFlds1.Rows(i).Item("DisplayName") & "'>") Then
                        Else
                            Sb.Append(",").Append("<isnull(D.[" & theFlds1.Rows(i).Item("DisplayName") & "],'')'" & theFlds1.Rows(i).Item("DisplayName") & "'>")
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return Sb.ToString()
    End Function

    Protected Sub gvData_RowDataBind(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Try
            Dim row As GridViewRow = e.Row
            Dim dtAction As New DataTable()
            If Not Session("tblActionField") Is Nothing Then
                dtAction = DirectCast(Session("tblActionField"), DataTable)
            End If
            If row.RowType = DataControlRowType.Header Then
                Session("HeaderRow") = row
                Dim chk As New CheckBox()
                chk.Checked = False
                chk.Text = "Check All"
                chk.Attributes.Add("onclick", "javascript:checkAll();")
                row.Cells(0).Controls.Add(chk)
            End If

            If row.RowType = DataControlRowType.DataRow Then
                Dim chk As New CheckBox()
                chk.Checked = False
                chk.ID = "chkBox" & row.RowIndex
                row.Cells(0).Controls.Add(chk)
                For i As Integer = 0 To row.Cells.Count - 1
                    Dim row1 = DirectCast(Session("HeaderRow"), GridViewRow)
                    Dim DisplayName = row1.Cells(i).Text.Trim()
                    Dim Text = row.Cells(i).Text
                    If String.IsNullOrEmpty(Text) Or String.IsNullOrWhiteSpace(Text) Or Text = "&nbsp;" Then
                        Text = String.Empty
                    End If
                    Dim onlyFiltered As DataView = dtAction.DefaultView
                    onlyFiltered.RowFilter = "DisplayName='" & DisplayName & "'" & ""
                    Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                    If theFlds.Rows.Count > 0 Then
                        row1.Cells(i).BackColor = Drawing.Color.Yellow
                        If theFlds.Rows(0).Item("IsEditable") = "1" Then
                            Dim txtBox As New TextBox()
                            txtBox.ID = "fld" & theFlds.Rows(0).Item("FieldID") & row.RowIndex
                            txtBox.Text = Text
                            txtBox.CssClass = "txtBox"
                            txtBox.BorderStyle = BorderStyle.Groove

                            'txtBox.Width = 70
                            'txtBox.Height = 20
                            row.Cells(i).Controls.Add(txtBox)
                            If theFlds.Rows(0).Item("DataType").ToString().ToUpper() = "DATETIME" Then
                                Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                                CLNDR.Controls.Clear()
                                CLNDR.ID = "CLNDR" & theFlds.Rows(0).Item("FieldID") & row.RowIndex
                                CLNDR.Format = "dd/MM/yy"
                                CLNDR.TargetControlID = txtBox.ID
                                txtBox.Enabled = True
                                row.Cells(i).Controls.Add(CLNDR)
                            End If

                        End If
                    End If
                Next

            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click

        Dim ds As New DataSet
        ds = DirectCast(Session("AppData"), DataSet)
        Dim gvreport1 As New GridView()
        gvreport1.AllowPaging = False
        gvreport1.AutoGenerateColumns = True

        gvreport1.DataSource = ds
        gvreport1.DataBind()
        'gvReport.AllowPaging = False
        'gvReport.DataBind()
        'gvData.DataSource = ViewState("xlexport")
        Response.Clear()
        Response.Buffer = True
        'Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "GPSDataReport" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "Report" & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To gvreport1.Rows.Count - 1
            'Apply text style to each Row 
            gvreport1.Rows(i).Attributes.Add("class", "textmode")
        Next
        gvreport1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()


    End Sub

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub

    Protected Sub btnApprove_Click(sender As Object, e As EventArgs) Handles btnApprove.Click
        Dim HeaderRow As GridViewRow
        Dim dtAction As New DataTable()

        Dim KeyValuePair As New StringBuilder()
        Dim Message As New StringBuilder()
        Dim DocType = ""
        Dim EID = Convert.ToInt32(Session("EID"))
        Dim DocID As Integer = 0
        If Not Session("tblActionField") Is Nothing Then
            dtAction = DirectCast(Session("tblActionField"), DataTable)
        End If
        Dim StrMessage = ""
        Dim ErrMsgLog = ""
        Dim RowNumber As Integer = 0
        Dim IsChecked As Boolean = False
        Dim MainDoctype = ddlDocumentType.SelectedItem.Text
        Try
            HeaderRow = gvData.HeaderRow
            If gvData.Rows.Count > 0 Then
                'loop through all rows of grid
                For Each row As GridViewRow In gvData.Rows
                    'Navigating each column of grid
                    RowNumber = RowNumber + 1
                    Dim DisPlayName = ""
                    IsChecked = False
                    KeyValuePair = New StringBuilder()
                    If row.RowType = DataControlRowType.DataRow Then

                        Dim chkBox As CheckBox
                        chkBox = CType(row.FindControl("chkBox" & row.RowIndex), CheckBox)
                        If Not (chkBox Is Nothing) And chkBox.Checked = True Then
                            DocID = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
                            ''Added By Mayank
                            'Dim objCustomRule As New CustomRuleEngine(EID, dtAction.Rows(0)("DocumentType"))
                            'Dim resultRuleResponse As New List(Of RuleResponse)
                            'resultRuleResponse = objCustomRule.InvisibleDocIDForDocDetail(DocID)
                            'Dim arrayList As New ArrayList()
                            'For Each _ruleResponse As RuleResponse In resultRuleResponse
                            '    If _ruleResponse.HasRule Then
                            '        If _ruleResponse.SuccActionType = "VISIBLE" Or _ruleResponse.FailActionType = "VISIBLE" Then
                            '            'arrayList.Add(" in (" & _ruleResponse.TargetField & ") ")
                            '        ElseIf _ruleResponse.SuccActionType = "INVISIBLE" Or _ruleResponse.FailActionType = "INVISIBLE" Then
                            '            arrayList.Add(_ruleResponse.TargetField)
                            '        End If
                            '    End If
                            'Next
                            ''Added By Mayank
                            IsChecked = True
                            If dtAction.Rows.Count > 0 Then
                                For i As Integer = 0 To row.Cells.Count - 1
                                    DisPlayName = HeaderRow.Cells(i).Text
                                    Dim onlyFiltered As DataView = dtAction.DefaultView
                                    onlyFiltered.RowFilter = "DisplayName='" & DisPlayName & "' "
                                    Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                    If theFlds.Rows.Count > 0 Then
                                        Dim txtBox As TextBox
                                        DocType = theFlds.Rows(0).Item("DocumentType")
                                        txtBox = CType(row.FindControl("fld" & theFlds.Rows(0).Item("FieldID") & row.RowIndex), TextBox)
                                        Dim Data = row.Cells(i).Text
                                        If Not txtBox Is Nothing Then
                                            If row.Cells(i).Text.Trim = "&nbsp;" Then
                                                row.Cells(i).Text = ""
                                            End If
                                            KeyValuePair.Append("|").Append(DisPlayName).Append("::").Append(txtBox.Text)
                                        Else
                                            If row.Cells(i).Text.Trim = "&nbsp;" Then
                                                row.Cells(i).Text = ""
                                            End If
                                            KeyValuePair.Append("|").Append(DisPlayName).Append("::").Append(row.Cells(i).Text.Trim)

                                            End If
                                        End If
                                Next
                            End If
                        End If
                        Dim lineItem As New LineitemWrap()
                        Dim Flag = False
                        Dim result = ""
                        If IsChecked = True Then
                            If KeyValuePair.ToString <> "" Then
                                StrMessage = CommanUtil.ValidateParameterByDocumentType(Session("EID"), DocType, Session("UID"), KeyValuePair.ToString(), lineItem, Flag, True, True)
                                'Save into history table
                                If Flag = True Then
                                    result = Approve(DocID, lineItem, DocType, MainDoctype)
                                Else
                                    result = StrMessage  'Message.Append(RowNumber).Append("|").Append(DocID).Append(resu)
                                End If
                            Else
                                result = Approve(DocID, lineItem, DocType, MainDoctype)
                            End If
                            Message.Append(RowNumber).Append("|").Append(DocID).Append("|").Append(result).Append("$$")
                        End If
                    End If
                Next
                Dim Message1 As String = ""
                Message1 = Message.ToString()
                gvData.DataSource = Nothing
                gvData.DataBind()
                'gvReport.DataSource = Nothing
                'gvReport.DataBind()
                Session("AppData") = Nothing
                Session("AppData1") = Nothing
                Session("tblActionField") = Nothing
                'pnlData.Visible = False
                ShowResult(Message1)
                divgvData.Visible = False
                dvControl.Visible = False
                Up1.Update()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Function Approve(DocID As Integer, lineItem As LineitemWrap, DocType As String, MainDocType As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret = ""
        Dim strfld = "", strFldVal = ""
        Dim UpQuery = ""
        Dim insertSql = ""
        Dim HasUpField = False
        Dim con = New SqlConnection(conStr)
        Dim trans As SqlTransaction = Nothing
        Dim objBA As New BulkApprovalBAL()
        Dim WFStatus = ddlWF.SelectedItem.Text
        Try
            Dim ds As DataSet = DirectCast(Session("AppData1"), DataSet)
            Dim dt As DataTable = ds.Tables(0)
            Dim onlyFiltered As DataView = dt.DefaultView
            onlyFiltered.RowFilter = "DOCID= " & DocID
            Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

            If theFlds.Rows.Count > 0 Then
                If (objBA.IsValidMovementRequest(Session("EID"), DocID, Session("UID"), WFStatus)) Then
                    Dim HisReport = CommanUtil.SetHistory(Session("EID"), DocID, "MMM_MST_DOC", Session("UID"), "Bulk Approval", DocType)
                    Dim UpdateQuery As New StringBuilder()
                    UpdateQuery.Append("UPDATE MMM_MST_DOC SET ")
                    If Not lineItem.DataItem Is Nothing Then
                        For Each item As UserData In lineItem.DataItem
                            HasUpField = True
                            UpdateQuery.Append(item.FieldMapping).Append("=").Append("'" & item.Values & "'").Append(",")
                        Next
                    End If

                    Dim DocUpQuery = ""
                    If HasUpField = True Then
                        UpQuery = UpdateQuery.ToString().Trim.Substring(0, UpdateQuery.ToString().Trim().Length - 1)
                        UpQuery = UpQuery.ToString() & " WHERE EID=" & Session("EID") & " AND tid=" & DocID
                        DocUpQuery = UpQuery
                        UpQuery = insertSql & ";" & UpQuery
                    Else
                        UpQuery = ""
                    End If

                    'Executing Update Query
                    con.Open()
                    trans = con.BeginTransaction()
                    If DocUpQuery.Length > 10 Then
                        Dim da = New SqlDataAdapter(DocUpQuery, con)
                        da.SelectCommand.Transaction = trans
                        Dim Uresult = da.SelectCommand.ExecuteNonQuery()
                    End If


                    Dim ob1 As New DMSUtil()
                    Dim res As String
                    res = "Not Uploaded:Fail" 'ob1.GetNextUserFromRolematrix(DocID, Session("EID"), Val(Session("UID").ToString()), UpQuery, Val(Session("UID").ToString()))

                    res = ob1.GetNextUserFromRolematrixT(DocID, Session("EID"), Val(Session("UID").ToString()), UpQuery, Val(Session("UID").ToString()), con, trans)
                    Dim sretMsgArr() As String = res.Split(":")
                    Dim IsValidMovement = True
                    If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                        ret = "Next Approvar/User not found, please contact Admin"
                        IsValidMovement = False
                        trans.Rollback()
                    End If

                    If res = "User Not Authorised" Then
                        ret = "You are not authorised to Approve this document"
                        IsValidMovement = False
                        trans.Rollback()
                    End If
                    If res = "Can not Approve, Reached to ARCHIVE" Then
                        ret = "Can not Approve, Reached to ARCHIVE"
                        IsValidMovement = False
                        trans.Rollback()
                    End If

                    If res = "Current and Next Status cannot be same" Then
                        ret = "Current and Next Status cannot be same"
                        IsValidMovement = False
                        trans.Rollback()
                    End If

                    If IsValidMovement = True Then
                        If sretMsgArr(0) = "ARCHIVE" Then
                            Dim Op As New Exportdata()
                            Op.PushdataT(DocID, sretMsgArr(1), Session("EID"), con, trans)
                        End If
                        Try
                            Trigger.ExecuteTriggerT(DocType, Session("EID"), DocID, TriggerNature:="Create", con:=con, tran:=trans)
                        Catch ex As Exception
                            Throw
                        End Try
                        trans.Commit()
                        ret = "Document has been approved successfully."
                        ob1.TemplateCalling(DocID, Session("EID").ToString(), "", "APPROVE")

                    End If
                Else
                    ret = "Document has already been approved."
                End If
            Else
                ret = "Invalid document."
            End If
        Catch ex As Exception
            Dim Msg = ex.Message
            trans.Rollback()
            Return "Error occured at server."
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not trans Is Nothing Then
                trans.Dispose()
            End If
        End Try
        Return ret
    End Function

    'Protected Function ShowResult(strError As String) As String
    '    Dim str = ""
    '    Dim SB As New StringBuilder()
    '    SB.Append("<table cellpadding='2' width='100%' border='2px'><tr style='background-color:#000000; color:white;'><td>Row No.</td><td>DocID</td><td>Result</td></tr>")
    '    Dim arr As String() = Split(strError, "$$")
    '    For i As Integer = 0 To arr.Count - 1
    '        If arr(i) <> "" Then
    '            Dim arr1 As String() = Split(arr(i), "|")
    '            SB.Append("<tr><td>" & arr1(0) & "</td><td>" & arr1(1) & "</td><td style='color:red;' >" & arr1(2) & " at " & arr1(4) & "</td></tr>")
    '        End If
    '    Next
    '    SB.Append("</table>")
    '    str = SB.ToString()
    '    lblMessage.Text = str
    '    Return str
    'End Function
    Protected Function ShowResult(strError As String) As String
        Dim str = ""
        Dim SB As New StringBuilder()
        SB.Append("<table cellpadding='2' width='100%' border='2px'><tr style='background-color:#000000; color:white;'><td>Row No.</td><td>DocID</td><td>Result</td></tr>")
        Dim arr As String() = Split(strError, "$$")
        For i As Integer = 0 To arr.Count - 1
            If arr(i) <> "" Then
                Dim arr1 As String() = Split(arr(i), "|")
                If arr1.Length > 3 Then
                    SB.Append("<tr><td>" & arr1(0) & "</td><td>" & arr1(1) & "</td><td style='color:red;' >" & arr1(2) & " at " & arr1(4) & "</td></tr>")
                Else
                    SB.Append("<tr><td>" & arr1(0) & "</td><td>" & arr1(1) & "</td><td style='color:green;' >" & arr1(2) & "</td></tr>")
                End If

            End If
        Next
        SB.Append("</table>")
        str = SB.ToString()
        lblMessage.Text = str
        Return str
    End Function

    Protected Sub btnSample_Click(sender As Object, e As EventArgs) Handles btnSample.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = Nothing
        Try
            Response.Clear()
            Response.Buffer = True


            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"

            Dim Documenttype = "", WFStatus = "", EID = "0"
            EID = Convert.ToInt32(Session("EID"))
            Dim UID = Convert.ToInt32(Session("UID"))
            Documenttype = ddlDocumentType.SelectedValue.ToString()
            WFStatus = ddlWF.SelectedItem.Text
            Dim obj As New BulkApprovalBAL()
            Dim ds1 As New DataSet()
            Dim strDocType = ""
            If (Documenttype <> "0" And WFStatus <> "0") Then
                ds1 = obj.GetActionFormNameBA(EID, Documenttype, WFStatus)
                If (ds1.Tables(0).Rows.Count > 0) Then
                    strDocType = ds1.Tables(0).Rows(0).Item("FormName")
                End If
            End If

            'fill Product
            Dim ds As New DataSet
            Dim scrname As String = strDocType
            scrname = scrname.Replace(" ", "_")
            Response.AddHeader("content-disposition", "attachment;filename=" & scrname & ".xls")
            da = New SqlDataAdapter("SELECT  displayName[Display Name],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (MM/DD/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & strDocType & "'  AND IsEditable=1 and invisible='0' order by displayorder", con)
            Dim query As String = "SELECT displayName,case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (MM/DD/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & strDocType & "'  AND IsEditable=1 and invisible='0' order by displayorder"
            Dim cmd As SqlCommand = New SqlCommand(query, con)
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            Dim dt As DataTable = New DataTable()
            Dim dt2 As DataTable = New DataTable()
            Dim dt3 As DataTable = New DataTable()

            dt.Load(dr)
            da.Fill(ds, "data")
            dt3 = ds.Tables("data")
            dt2 = GetInversedDataTable(dt, "displayname", "")

            Dim gvex As New GridView()
            dt2.Rows.Add()

            gvex.AllowPaging = False
            gvex.DataSource = dt2
            gvex.DataBind()
            Dim gvexx As New GridView()
            gvexx.AllowPaging = False
            gvexx.DataSource = dt3

            gvexx.DataBind()
            Response.Clear()
            Response.Buffer = True
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            Dim tb As New Table()
            Dim tr1 As New TableRow()
            Dim cell1 As New TableCell()
            cell1.Controls.Add(gvex)
            tr1.Cells.Add(cell1)
            Dim cell3 As New TableCell()
            cell3.Controls.Add(gvexx)
            Dim cell2 As New TableCell()
            cell2.Text = "&nbsp;"

            Dim tr2 As New TableRow()
            tr2.Cells.Add(cell2)
            Dim tr3 As New TableRow()
            tr3.Cells.Add(cell3)
            tb.Rows.Add(tr1)
            tb.Rows.Add(tr2)
            tb.Rows.Add(tr3)
            tb.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
    End Sub

    Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable

        Dim returnTable As New DataTable()
        returnTable.Columns.Add("DocID")
        If columnX = "" Then
            columnX = table.Columns(0).ColumnName
        End If

        Dim columnXValues As New List(Of String)()

        For Each dr As DataRow In table.Rows
            Dim columnXTemp As String = dr(columnX).ToString()
            If Not columnXValues.Contains(columnXTemp) Then
                columnXValues.Add(columnXTemp)
                returnTable.Columns.Add(columnXTemp)
            End If
        Next
        If nullValue <> "" Then
            For Each dr As DataRow In returnTable.Rows
                For Each dc As DataColumn In returnTable.Columns
                    If dr(dc.ColumnName).ToString() = "" Then
                        dr(dc.ColumnName) = nullValue
                    End If
                Next
            Next
        End If
        Return returnTable

    End Function

    Public Function GetDataFromExcel(ByVal strDataFilePath As String) As DataTable

        Try
            Dim sr As New StreamReader(Server.MapPath("~/" & strDataFilePath))
            Dim fullFileStr As String = sr.ReadToEnd()
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(","c)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            Dim i As Integer = 0
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                    row.ItemArray = finalLine.Split(","c)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            'DataColumn Col   = datatable.Columns.Add("Column Name", System.Type.GetType("System.Boolean"));
            Dim col As DataColumn = recs.Columns.Add("Check All", System.Type.GetType("System.String"))
            col.SetOrdinal(0)
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim filename As String = ""
        Dim dt As New DataTable()
        'Session("AppData") = Nothing
        gvData.DataSource = Nothing
        gvData.DataBind()
        lblDataCount.Text = "0"
        Try
            If FlUploader.HasFile() Then
                filename = "BulkAppr" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(FlUploader.FileName, 4).ToUpper()
                FlUploader.PostedFile.SaveAs(Server.MapPath("~/Import/" & filename))
                filename = "Import/" & filename
                dt = GetDataFromExcel(filename)
                If dt.Rows.Count > 0 Then
                    FillDocData(dt)
                    gvData.DataSource = dt
                    gvData.DataBind()
                    lblDataCount.Text = dt.Rows.Count
                    Dim ds As New DataSet()
                    ds.Tables.Add(dt)
                    Label2.Text = "Total Records : " & dt.Rows.Count
                    Session("AppData") = ds
                    'gvReport.DataSource = dt
                    'gvReport.DataBind()
                End If
            End If
        Catch ex As Exception
        Finally
            If File.Exists(Server.MapPath("~/Import/" & filename)) Then
                File.Delete(Server.MapPath("~/Import/" & filename))
            End If
        End Try
    End Sub

    Protected Sub gv_OnRowCreated(sender As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Alternate Then
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CAFF70';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';")
            Else
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CAFF70';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';")
            End If
        End If
    End Sub

    Public Sub FillDocData(ByRef dt As DataTable)
        Try
            Dim ds As New DataSet
            ds = DirectCast(Session("AppData"), DataSet)
            Dim dtA = CType(Session("tblActionField"), DataTable)
            If dt.Rows.Count > 0 Then
                'loop through all column of Excell file
                For Each column As DataColumn In ds.Tables(0).Columns
                    'if column do not exists add that column to the concern field
                    If Not ContainColumn(column.ColumnName.Trim, dt) Then
                        dt.Columns.Add(column.ColumnName)
                    End If
                Next
                'loop for filling all data of the concern document
                For i As Integer = 0 To dt.Rows.Count - 1
                    Try
                        Dim DOCID = dt.Rows(i).Item("DOCID").ToString()
                        Dim onlyFiltered As DataView = ds.Tables(0).DefaultView
                        onlyFiltered.RowFilter = "DOCID=" & DOCID & ""
                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

                        If theFlds.Rows.Count > 0 Then
                            For Each column As DataColumn In dt.Columns
                                Dim DisplayName = column.ColumnName.Trim()
                                Dim onlyFiltered1 As DataView = dtA.DefaultView
                                onlyFiltered1.RowFilter = "displayname='" & DisplayName & "' and iseditable=1"
                                Dim theFlds1 As DataTable = onlyFiltered1.Table.DefaultView.ToTable()
                                If Not theFlds1.Rows.Count > 0 And DisplayName.ToUpper <> "CHECK ALL" Then
                                    dt.Rows(i).Item(column.ColumnName) = theFlds.Rows(0).Item(column.ColumnName)
                                End If
                            Next
                        End If

                    Catch ex As Exception

                    End Try
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub
    'Session("AppData")

    Private Function ContainColumn(columnName As String, table As DataTable) As Boolean
        Dim ret As Boolean = False
        Dim columns As DataColumnCollection = table.Columns
        If (columns.Contains(columnName)) Then
            ret = True
        End If
        Return ret
    End Function

    Protected Sub ddlFieldName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFieldName.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim Documenttype = ""
            Dim FieldMapping As String = ""
            Documenttype = ddlDocumentType.SelectedValue.ToString()
            FieldMapping = ddlFieldName.SelectedValue
            Dim EID = Session("EID").ToString
            Dim UID = Session("UID")
            Dim WFStatus = ddlWF.SelectedItem.Text
            Dim StrViewName = "[" & "V" & EID & Documenttype.Replace(" ", "_") & "] D"
            'Getting field derfnition from database
            If FieldMapping <> "0" Then
                ddlFormValue.DataSource = Nothing
                ddlFormValue.DataBind()
                Dim li As New ListItem("--Select--", 0)
                ddlFormValue.Items.Insert(0, li)
                Dim Query = "select * from MMM_MST_FIELDS WHERE EID= " & Session("EID") & " AND DocumentType='" & Documenttype & "' AND fieldMapping='" & FieldMapping & "'"
                Dim ds As New DataSet()
                Using con As New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(Query, con)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        da.Fill(ds)
                    End Using
                End Using
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim FieldType = "", DropDownType = ""
                    FieldType = Convert.ToString(ds.Tables(0).Rows(0).Item("FieldType")).Trim()
                    DropDownType = Convert.ToString(ds.Tables(0).Rows(0).Item("DropDownType")).Trim()
                    Dim StrQuery1 = ""
                    StrQuery1 = "SELECT Distinct [" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'Text',[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value' FROM " & StrViewName
                    If FieldType = "Drop Down" And (DropDownType = "MASTER VALUED" Or DropDownType = "SESSION VALUED") Then
                        Dim arr = ds.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                        If arr(1).ToUpper = "USER" Then
                            StrQuery1 = "(SELECT isnull([userName],'') from MMM_MST_USER s WHERE s.[UID]=D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "])'Text',[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'Value'"
                        Else
                            Dim Str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & EID & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
                            Dim ds1 As New DataSet()
                            Using con As New SqlConnection(conStr)
                                Using da As New SqlDataAdapter(Str, con)
                                    If con.State = ConnectionState.Closed Then
                                        con.Open()
                                    End If
                                    da.Fill(ds1)
                                End Using
                            End Using
                            StrQuery1 = "(SELECT isnull([" & ds1.Tables(0).Rows(0).Item("DisplayName") & "],'')  from [V" & EID & arr(1).Replace(" ", "_") & "] s WHERE s.tid=D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "])'Text',[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value'"
                        End If
                        StrQuery1 = "SELECT distinct  " & StrQuery1 & " FROM " & StrViewName
                    End If
                    StrQuery1 = StrQuery1 & " INNER JOIN MMM_DOC_DTL DTL ON D.LastTID=DTL.tid WHERE D.EID=" & EID & " AND DTL.userid=" & UID & " and D.DocumentType='" & Documenttype & "' AND D.Curstatus='" & WFStatus & "'"
                    'Filling data to the concern dropdown
                    ds = New DataSet()
                    Using con As New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(StrQuery1, con)
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            da.Fill(ds)
                        End Using
                    End Using
                    If ds.Tables(0).Rows.Count > 0 Then
                        ddlFormValue.DataSource = ds
                        ddlFormValue.DataTextField = "Text"
                        ddlFormValue.DataValueField = "value"
                        ddlFormValue.DataBind()
                        ddlFormValue.Items.Insert(0, li)
                    End If
                End If
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim FieldName = "", FieldValue = ""
            FieldName = ddlFieldName.SelectedItem.Text
            FieldValue = ddlFormValue.SelectedItem.Value
            Dim Cond = ""
            Cond = "AND D.[" & FieldName & "] = '" & FieldValue & "'"
            Dim Query = ViewState("Query").ToString()
            Query = Query & Cond
            Query = Query & "order by D.tid desc"
            Dim ds As New DataSet()
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Query, con)
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
            Session("AppData") = ds
            bindGrid(ds)
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub gvPending_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvData.RowDataBound
        'If e.Row.RowType = DataControlRowType.Header Then
        '    e.Row.Cells(6).Visible = False
        'End If
        If e.Row.RowType = DataControlRowType.DataRow Then
            'e.Row.Cells(7).Visible = False
            'e.Row.Attributes.Add("onclick", "javascript:OpenWindow('DocDetail.aspx?DOCID=" & gvData.DataKeys(e.Row.RowIndex).Value & "')")
            'e.Row.Attributes("style") = "cursor:pointer"
            e.Row.Cells(1).Attributes.Add("onclick", "javascript:OpenWindow('DocDetail.aspx?DOCID=" & gvData.DataKeys(e.Row.RowIndex).Value & "')")
            e.Row.Cells(1).Attributes("style") = "cursor:pointer"
        End If
    End Sub

    Protected Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        Dim HeaderRow As GridViewRow
        Dim dtAction As New DataTable()

        Dim DocType = ""
        Dim EID = Convert.ToInt32(Session("EID"))
        Dim DocID As Integer = 0
        If Not Session("tblActionFieldCopy") Is Nothing Then
            dtAction = DirectCast(Session("tblActionFieldCopy"), DataTable)
        End If
        Dim RowNumber As Integer = 0
        Dim IsChecked As Boolean = False
        Dim MainDoctype = ddlDocumentType.SelectedItem.Text
        Try
            HeaderRow = gvData.HeaderRow
            If gvData.Rows.Count > 0 Then
                'loop through all rows of grid
                For Each row As GridViewRow In gvData.Rows
                    'Navigating each column of grid
                    RowNumber = RowNumber + 1
                    Dim DisPlayName = ""
                    IsChecked = False
                    If row.RowType = DataControlRowType.DataRow Then
                        Dim chkBox As CheckBox
                        chkBox = CType(row.FindControl("chkBox" & row.RowIndex), CheckBox)
                        If Not (chkBox Is Nothing) And chkBox.Checked = True Then
                            DocID = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
                            IsChecked = True
                            If dtAction.Rows.Count > 0 Then
                                For i As Integer = 0 To row.Cells.Count - 1
                                    DisPlayName = HeaderRow.Cells(i).Text
                                    Dim onlyFiltered As DataView = dtAction.DefaultView
                                    onlyFiltered.RowFilter = "DisplayName='" & DisPlayName & "' "
                                    Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

                                    If theFlds.Rows.Count > 0 Then
                                        Dim txtBox As TextBox
                                        DocType = theFlds.Rows(0).Item("DocumentType")
                                        Dim FIeldID As String = theFlds.Rows(0).Item("FieldID")
                                        Dim FieldType As String = theFlds.Rows(0).Item("FieldType").ToString.Trim.ToUpper
                                        Dim cValue As String = ""
                                        Select Case FieldType
                                            Case "TEXT BOX"
                                                Dim txtBox1 As TextBox = CType(pnlFields.FindControl("fld" & FIeldID.ToString()), TextBox)
                                                cValue = txtBox1.Text.Trim
                                            Case "DROP DOWN"
                                                Dim DDL As DropDownList = CType(pnlFields.FindControl("fld" & FIeldID.ToString()), DropDownList)
                                                cValue = DDL.SelectedItem.Text
                                            Case "TEXT AREA"
                                                Dim txtBox1 As TextBox = CType(pnlFields.FindControl("fld" & FIeldID), TextBox)
                                                cValue = txtBox1.Text.Trim
                                            Case "AUTOCOMPLETE"
                                                Dim txtBox1 As TextBox = CType(pnlFields.FindControl("fld" & FIeldID), TextBox)
                                                cValue = txtBox1.Text.Trim
                                            Case "FILE UPLOADER"
                                                Dim lbl As Label = CType(pnlFields.FindControl("lblf" & FIeldID), Label)
                                                If Not IsNothing(lbl) Then
                                                    cValue = lbl.Text
                                                End If
                                            Case "LOOKUP"
                                                Dim txtBox1 As TextBox = CType(pnlFields.FindControl("fld" & FIeldID), TextBox)
                                                cValue = txtBox1.Text.Trim
                                            Case "CALCULATIVE FIELD"
                                                Dim txtBox1 As TextBox = CType(pnlFields.FindControl("fld" & FIeldID), TextBox)
                                                cValue = txtBox1.Text.Trim
                                            Case "FORMULA FIELD"
                                                Dim txtBox1 As TextBox = CType(pnlFields.FindControl("fld" & FIeldID), TextBox)
                                                cValue = txtBox1.Text.Trim
                                        End Select
                                        txtBox = CType(row.FindControl("fld" & theFlds.Rows(0).Item("FieldID") & row.RowIndex), TextBox)
                                        Dim Data = row.Cells(i).Text
                                        If Not txtBox Is Nothing Then
                                            txtBox.Text = cValue
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
