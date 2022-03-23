Imports System.Data
Imports System.Data.SqlClient
Partial Class MIS_DashBord_Config
    Inherits System.Web.UI.Page
    Dim objDC As New DataClass()
    Dim SLADTColumn As String = ""
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
    Private Sub MIS_DashBord_Config_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            ddldbname.Enabled = False

            Dim dt As New DataTable()
            dt.Columns.AddRange(New DataColumn(4) {New DataColumn("DocumentType"), New DataColumn("WorkFlow"), New DataColumn("alias"), New DataColumn("orderid"), New DataColumn("Tid")})
            ViewState("Detail") = dt
            Me.BindGrid()
            Dim dt1 As New DataTable()
            dt1.Columns.AddRange(New DataColumn(3) {New DataColumn("DisplayName"), New DataColumn("DocumentType"), New DataColumn("Fields"), New DataColumn("Tid")})
            ViewState("gvrdetail") = dt1
            'Me.BindGridDetail()
            BindDashbordConfig()

        End If
    End Sub
    Private Sub PopulateAllowedRole()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select Rolename,Rolename from mmm_mst_role where eid=" & Session("EID") & "")
        If objDT.Rows.Count > 0 Then
            CheckBoxList1.DataSource = objDT
            CheckBoxList1.DataTextField = "Rolename"
            CheckBoxList1.DataValueField = "Rolename"
            CheckBoxList1.DataBind()
        Else
            CheckBoxList1.Items.Clear()

        End If
    End Sub
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            popAlert.Visible = False
            hdnTID.Value = 0
            PopulateAllowedRole()
        Catch ex As Exception
            Throw
        End Try
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        popAlert.Visible = False
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvDashboardData.DataKeys(row.RowIndex).Value)
        Dim c As String() = New String(50) {}
        btnSubmit.Text = "Update"
        ViewState("pid") = pid
        Dim objDT As New DataTable()
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                cmd.CommandText = ("select * from mmm_mst_misdb where Tid =" & pid)
                cmd.Connection = conn
                conn.Open()
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    While sdr.Read()
                        hdnTID.Value = pid
                        'txtDBID.Text = pid
                        txtDBName.SelectedItem.Text = sdr("DBName")
                        ddlcharttype.SelectedValue = ("ChartType")
                        PopulateAllowedRole()
                        c = sdr("Roles").ToString().Split(",")
                        Dim length2 As Integer = c.Length
                        For i As Integer = 0 To c.Length - 1
                            Dim cntry As String = c(i)
                            For j As Integer = 0 To CheckBoxList1.Items.Count - 1
                                If CheckBoxList1.Items(j).Value = c(i) Then
                                    CheckBoxList1.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next
                        For Each Item As ListItem In ddlisactive.Items
                            If Item.Value.ToString().Contains(sdr("Isactive")) Then
                                Item.Selected = True
                            Else
                                Item.Selected = False
                            End If
                        Next
                    End While
                End Using
                conn.Close()
            End Using
            updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Show()
        End Using
    End Sub
    Private Sub Populateworkflowstatus()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select StatusName  from  [MMM_MST_WORKFLOW_STATUS] where  Documenttype='" & ddldoctype.SelectedItem.Text.Trim() & "'  and eid=" & Session("EID") & "  order by StatusName")
        If objDT.Rows.Count > 0 Then
            chkbxFilterStatus.DataSource = objDT
            chkbxFilterStatus.DataTextField = "StatusName"
            chkbxFilterStatus.DataValueField = "StatusName"
            chkbxFilterStatus.DataBind()
        Else
            chkbxFilterStatus.Items.Clear()
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If txtDBName.SelectedValue = "Select" Then
            lblMsgEdit.Text = "Please Select DB Name"
            popAlert.Visible = True
            btnEdit_ModalPopupExtender.Show()
            Return
        End If
        If btnSubmit.Text = "Save" Then
            Dim objDT As New DataTable
            objDT = objDC.ExecuteQryDT("select * from mmm_mst_misdb where  DBName='" & txtDBName.SelectedItem.Text & "' and eid=" & Session("Eid") & "")
            If objDT.Rows.Count > 0 Then
                lblMsgEdit.Text = "Already Exist!!"
                popAlert.Visible = True
                Return
            End If
        End If
        Dim r As Integer
        Dim sb4 As StringBuilder = New StringBuilder()
        For r = 0 To CheckBoxList1.Items.Count - 1
            If CheckBoxList1.Items(r).Selected Then
                sb4.Append(CheckBoxList1.Items(r).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim AllowedRoll As String = ""
        If sb4.Length > 0 Then
            AllowedRoll = Left(sb4.ToString(), Len(sb4.ToString()) - 1)
        Else
            AllowedRoll = Nothing
        End If
        Try
            Dim objDC As New DataClass()
            Dim ht As New Hashtable
            ht.Add("@Tid", hdnTID.Value)
            ht.Add("@DBName", txtDBName.SelectedItem.Text)
            ht.Add("@Eid", Session("Eid"))
            ht.Add("@ChartType", ddlcharttype.SelectedValue)
            ht.Add("@Roles", AllowedRoll)
            ht.Add("@Isactive", ddlisactive.SelectedValue)
            ht.Add("@Dord", 0)
            objDC.ExecuteProDT("AddUpdateDBConfiguration", ht)
            Me.btnEdit_ModalPopupExtender.Hide()
            BindDashbordConfig()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub BindDashbordConfig()
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBDetail"
        cmd.Parameters.Add("@sValue", SqlDbType.VarChar).Value = txtValue.Text.Trim()
        cmd.Parameters.Add("@EID", SqlDbType.Int).Value = Session("Eid")
        cmd.Connection = con
        Try
            con.Open()
            gvDashboardData.EmptyDataText = "No Records Found"
            gvDashboardData.DataSource = cmd.ExecuteReader()
            gvDashboardData.DataBind()
        Catch ex As Exception
            Throw ex
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs)
        BindDashbordConfig()
    End Sub
    Protected Sub subchildbind(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objDC As New DataClass()
        Dim ht As New Hashtable
        ht.Add("@RefTid", hdnrefid.Value)
        ht.Add("@DocType", ddldoctype.SelectedItem.Text)
        ht.Add("@Type", ddltype.SelectedItem.Text)
        ht.Add("@catfield", ddlcatfield.SelectedValue.Split("-")(0))
        ht.Add("@valuefield", ddlvaluefield.SelectedValue)
        ht.Add("@Recdatefield", ddlrecvdatefield.SelectedValue)
        ht.Add("@Invdatefield", ddlinvdatefield.SelectedValue)
        objDC.ExecuteProDT("AddUpdateDBConfigurationChildItem", ht)
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        Div2.Visible = False
        Label2.Text = ""
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBchildDetail"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnrefid.Value
        cmd.Connection = con
        Try
            con.Open()
            gridchilditemdata.EmptyDataText = "No Records Found"
            gridchilditemdata.DataSource = cmd.ExecuteReader()
            gridchilditemdata.DataBind()
        Catch ex As Exception
            Throw ex
        Finally
            con.Close()
            con.Dispose()
        End Try
        addgridvalue.Text = "Add"
        updatePanel1.Update()
        Me.ModalPopupAddchilditem.Show()
        Me.ModalPopupAddchilditem.Show()
        cleartex()
    End Sub
    Private Sub cleartex()
        ddldoctype.SelectedItem.Text = "Select"
        ddlcatfield.SelectedItem.Text = ""
        ddlvaluefield.SelectedItem.Text = ""
        ddlinvdatefield.SelectedItem.Text = ""
        ddlrecvdatefield.SelectedItem.Text = ""
    End Sub

    Protected Sub child(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            trwf.Visible = False
            Dim btnDetailschild As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetailschild.NamingContainer, GridViewRow)
            Dim RefTid As Integer = Convert.ToString(Me.gvDashboardData.DataKeys(row.RowIndex).Value)
            Dim dropdownvalue As String = ""
            For i As Integer = 0 To Me.gvDashboardData.Rows.Count - 1
                dropdownvalue = Convert.ToString(Me.gvDashboardData.Rows(i).Cells(2).Text)
            Next
            Div1.Visible = False
            Label1.Text = ""
            ddldbname.Text = dropdownvalue.ToString()
            trtid.Visible = False
            hdnrefid.Value = RefTid
            BindDocumentType()
            'cleartex()
            Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(strConnString)
            Dim cmd As New SqlCommand()
            Div2.Visible = False
            Label2.Text = ""

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "uspGetDBchildDetail"
            cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnrefid.Value
            cmd.Connection = con
            Try
                con.Open()
                gridchilditemdata.EmptyDataText = "No Records Found"
                gridchilditemdata.DataSource = cmd.ExecuteReader()
                gridchilditemdata.DataBind()
            Catch ex As Exception
                Throw ex
            Finally
                con.Close()
                con.Dispose()
            End Try
        Catch ex As Exception
            Throw ex
        End Try
        updatePanel1.Update()
        Me.ModalPopupAddchilditem.Show()
    End Sub
    Protected Sub Detail(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Div3.Visible = False
            Dim btnDetailschild As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetailschild.NamingContainer, GridViewRow)
            Dim RefTid As Integer = Convert.ToString(Me.gvDashboardData.DataKeys(row.RowIndex).Value)
            hdndetail.Value = RefTid
            DocumentTypeBind()
            Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(strConnString)
            Dim cmd As New SqlCommand()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "uspGetDBDetailitemvalue"
            cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdndetail.Value
            cmd.Connection = con
            Try
                con.Open()
                detailgriditem.EmptyDataText = "No Records Found"
                detailgriditem.DataSource = cmd.ExecuteReader()
                detailgriditem.DataBind()
            Catch ex As Exception
                Throw ex
            Finally
                con.Close()
                con.Dispose()
            End Try
        Catch ex As Exception
            Throw
        End Try
        updtpnladddetailsfields.Update()
        Me.ModalPopupExtenderDetailfield.Show()
    End Sub
    Protected Sub BindDocumentType()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select distinct FormID, formname from mmm_mst_forms where formtype='DOCUMENT' and formsource='MENU DRIVEN' and isactive=1 and eid=" & Session("EID") & " order by formname ")
        If objDT.Rows.Count > 0 Then
            ddldoctype.DataSource = objDT
            ddldoctype.DataTextField = "formname"
            ddldoctype.DataValueField = "formname"
            ddldoctype.DataBind()
            ddldoctype.Items.Insert(0, "Select")

        Else
            ddldoctype.Items.Clear()
            ddldoctype.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub DocumentTypeBind()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select distinct FormID, formname from mmm_mst_forms where formtype='DOCUMENT' and formsource='MENU DRIVEN' and isactive=1 and eid=" & Session("EID") & " order by formname ")
        If objDT.Rows.Count > 0 Then
            documentTypeddl.DataSource = objDT
            documentTypeddl.DataTextField = "formname"
            documentTypeddl.DataValueField = "formname"
            documentTypeddl.DataBind()
            documentTypeddl.Items.Insert(0, "Select")
        Else
            documentTypeddl.Items.Clear()
            documentTypeddl.Items.Insert(0, "Select")
        End If
    End Sub
    Private Sub PopulateFilterField()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  fieldMapping+'-'+dropdown as fieldMapping,displayname from mmm_mst_fields where FieldType='Drop Down' and isactive=1 and DropDownType='MASTER VALUED' and eid=" & Session("EID") & " and DocumentType='" & ddldoctype.SelectedItem.Text.Trim() & "' order by displayName")
        If objDT.Rows.Count > 0 Then
            ddlcatfield.DataSource = objDT
            ddlcatfield.DataTextField = "displayName"
            ddlcatfield.DataValueField = "FieldMapping"
            ddlcatfield.DataBind()
            ddlcatfield.Items.Insert(0, "Select")
        Else
            ddlcatfield.Items.Clear()
            ddlcatfield.Items.Insert(0, "Select")
        End If
    End Sub
    Private Sub PopulateDetailField()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  fieldMapping,displayname from mmm_mst_fields where   isactive=1  and eid=" & Session("EID") & " and DocumentType='" & documentTypeddl.SelectedItem.Text.Trim() & "' order by displayName")
        If objDT.Rows.Count > 0 Then
            fieldsdropdown.DataSource = objDT
            fieldsdropdown.DataTextField = "displayName"
            fieldsdropdown.DataValueField = "FieldMapping"
            fieldsdropdown.DataBind()
            fieldsdropdown.Items.Insert(0, "Select")
        Else
            fieldsdropdown.Items.Clear()
            fieldsdropdown.Items.Insert(0, "Select")
        End If
    End Sub
    Private Sub Datefield()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  DisplayName,FieldMapping from mmm_mst_fields where datatype='Datetime' and fieldtype='Text Box' and Eid=" & Session("Eid") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayName")
        If objDT.Rows.Count > 0 Then
            ddlrecvdatefield.DataSource = objDT
            ddlrecvdatefield.DataTextField = "displayName"
            ddlrecvdatefield.DataValueField = "FieldMapping"
            ddlrecvdatefield.DataBind()
            ddlrecvdatefield.Items.Insert(0, "Select")
        Else
            ddlrecvdatefield.Items.Clear()
            ddlrecvdatefield.Items.Insert(0, "Select")
        End If
    End Sub
    Private Sub Datefield1()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  DisplayName,FieldMapping from mmm_mst_fields where datatype='Datetime' and fieldtype='Text Box' and Eid=" & Session("Eid") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' and DisplayName!='" & ddlrecvdatefield.SelectedItem.Text & "' order by displayName")
        If objDT.Rows.Count > 0 Then
            ddlinvdatefield.DataSource = objDT
            ddlinvdatefield.DataTextField = "displayName"
            ddlinvdatefield.DataValueField = "FieldMapping"
            ddlinvdatefield.DataBind()
            ddlinvdatefield.Items.Insert(0, "Select")
        Else
            ddlinvdatefield.Items.Clear()
            ddlinvdatefield.Items.Insert(0, "Select")
        End If
    End Sub

    Private Sub PopulateFilterField2()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select fieldMapping,displayname from mmm_mst_fields where isactive=1  and eid=" & Session("EID") & " and DocumentType='" & ddldoctype.SelectedItem.Text.Trim() & "'  and fieldtype in ('Text Box','Calculative Field','Child Item Total') and datatype='Numeric' order by displayName")
        If objDT.Rows.Count > 0 Then
            ddlvaluefield.DataSource = objDT
            ddlvaluefield.DataTextField = "displayName"
            ddlvaluefield.DataValueField = "FieldMapping"
            ddlvaluefield.DataBind()
            ddlvaluefield.Items.Insert(0, "Select")
        Else
            ddlvaluefield.Items.Clear()
            ddlvaluefield.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub BindGrid()
        Griddatediff.DataSource = DirectCast(ViewState("Detail"), DataTable)
        Griddatediff.DataBind()
    End Sub
    'Protected Sub BindGridDetail()
    '    gridviewdetails.DataSource = DirectCast(ViewState("gvrdetail"), DataTable)
    '    gridviewdetails.DataBind()
    'End Sub
    Private Sub detailfieldcleartext()
        ddlDisplayName.ClearSelection()
        documentTypeddl.ClearSelection()
        fieldsdropdown.SelectedItem.Text = ""

    End Sub
    Protected Sub InsertDetail(sender As Object, e As EventArgs)

        Dim objDC As New DataClass()
        Dim ht As New Hashtable
        ht.Add("@RefTid", hdndetail.Value)
        ht.Add("@DocType", documentTypeddl.SelectedItem.Text)
        ht.Add("@DisplayName", ddlDisplayName.SelectedItem.Text)
        ht.Add("@fields", fieldsdropdown.SelectedValue)
        objDC.ExecuteProDT("AddDBConfigurationSetailItem", ht)

        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBDetailitemvalue"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdndetail.Value
        cmd.Connection = con
        Try
            con.Open()
            detailgriditem.EmptyDataText = "No Records Found"
            detailgriditem.DataSource = cmd.ExecuteReader()
            detailgriditem.DataBind()
        Catch ex As Exception
            Throw ex
        Finally
            con.Close()
                con.Dispose()

            End Try
        detailfieldcleartext()
    End Sub
    Protected Sub Insert(sender As Object, e As EventArgs)
        Dim w As Integer
        Dim sb3 As StringBuilder = New StringBuilder()
        For w = 0 To chkbxFilterStatus.Items.Count - 1
            If chkbxFilterStatus.Items(w).Selected Then
                sb3.Append(chkbxFilterStatus.Items(w).Value & ",")
            End If
        Next
        If (Convert.ToString(hdnWorkFlow.Value) <> "") Then
            For w = 0 To chkbxFilterStatus.Items.Count - 1
                If chkbxFilterStatus.Items(w).Selected Then
                    If (("," & hdnWorkFlow.Value).Contains("," & (chkbxFilterStatus.Items(w).Value) & ",")) Then
                        Label1.Text = "Please Do not repeat WorkFlow values"
                        Div1.Visible = True
                        ModalPopupAddchilditem.Show()
                        Return
                    End If
                End If


            Next
        End If
        If (sb3.Length > 0) Then
            hdnWorkFlow.Value = hdnWorkFlow.Value & sb3.ToString()
        End If

        'Create the value to be inserted by removing the last comma in sb
        Dim Filterstatus As String = ""
        If sb3.Length > 0 Then
            Filterstatus = Left(sb3.ToString(), Len(sb3.ToString()) - 1)
        Else
            Filterstatus = Nothing
        End If

        Dim dt As DataTable = DirectCast(ViewState("Detail"), DataTable)
        dt.Rows.Add(ddldoctype.SelectedItem.Text.Trim(), Filterstatus, txtalias.Text.Trim(), txtorderid.Text.Trim())
        ViewState("Detail") = dt
        Me.BindGrid()

        Dim count As Integer = chkbxFilterStatus.Items.Count
        For i As Integer = 0 To count - 1

            If chkbxFilterStatus.Items(i).Selected = True Then
                chkbxFilterStatus.Items(i).Selected = False
            End If
        Next
        txtalias.Text = ""
        txtorderid.Text = ""

    End Sub

    Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateFilterField()
        Datefield()
        If (ddldbname.Text = "Open Invoice Ageing") Then
            trwf.Visible = True
            Populateworkflowstatus()

        End If
        If (ddldbname.Text = "SLA Performance") Then
            trwf.Visible = True
            Populateworkflowstatus()
        End If
    End Sub

    Protected Sub ddlcatfield_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateFilterField2()
    End Sub

    Protected Sub ddlrecvdatefield_SelectedIndexChanged(sender As Object, e As EventArgs)
        Datefield1()
    End Sub
    Protected Sub btnchild_Click(sender As Object, e As EventArgs)
        Dim StrColumn As String = ""
        Dim casequery1 As String = ""
        Dim datequery As String = ""
        Dim Incondition As String = ""
        Dim builder As New StringBuilder
        Dim build As String = ""
        Dim fdv As String = ""
        Dim groupby As String = ""
        Dim daterequired As String = ""
        Dim tblquery As String = ""
        Dim tbchitem As DataTable
        Dim pds As New DataSet
        Dim upd As New DataSet
        Dim newQuery As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim sdp As New DataSet
        Dim da As New SqlDataAdapter("", con)
        Dim da1 As New SqlDataAdapter("", con)
        Dim QueryNew As String = ""
        Dim predata As New DataSet
        Dim csv As String = ""
        Dim lstDocumenettype As New ArrayList
        Dim QueryType As String = ""
        Dim ddlvalue As String = ""
        Dim newQuery1 As String = ""
        For i As Integer = 0 To Me.gvDashboardData.Rows.Count - 1
            ddlvalue = Convert.ToString(Me.gvDashboardData.Rows(i).Cells(2).Text)
        Next
        'ddldbname.SelectedItem.Text = ddlvalue.ToString()
        'If ddldoctype.SelectedItem.Text = "Select" Then
        '    ddldoctype.Focus()
        '    Label1.Text = "Please Select DocumentType"
        '    Div1.Visible = True
        '    ModalPopupAddchilditem.Show()
        '    Return
        'End If
        'If ddlcatfield.SelectedItem.Text = "Select" Then
        '    Label1.Text = "Please Select Catagory Field"
        '    Div1.Visible = True
        '    ModalPopupAddchilditem.Show()
        '    Return
        'End If
        'If ddlvaluefield.SelectedItem.Text = "Select" Then
        '    Label1.Text = "Please Select Value Field"
        '    Div1.Visible = True
        '    ModalPopupAddchilditem.Show()
        '    Return
        'End If
        Try
            If (btnchild.Text = "Save" And ddlvalue.ToString() = "Expense Breakup") Then
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            casequery1 = casequery1 & "when max(documenttype)='" & ddoctype & "' then convert(numeric(10,2),sum(convert(numeric(10,2)," & valuefield & "))/1000000)"
                            groupby = groupby & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                        End If
                    Next
                End If
                Dim Query = "select top 5 category,sum(value)[value]from (select isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date and @Date group by case " & groupby & " end,documenttype) as t group by category order by [value] desc"
                Dim pit As String = ""
                pit = Query

                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()


            ElseIf (btnchild.Text = "Save" And ddlvalue.ToString() = "Supplier Spend Breakup" And ddltype.SelectedItem.Text = "Department") Then
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            casequery1 = casequery1 & "when max(documenttype)='" & ddoctype & "' then convert(numeric(10,2),sum(convert(numeric(10,2)," & valuefield & "))/1000000)"
                            groupby = groupby & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                        End If
                    Next
                End If
                Dim Query = "select top 5 category,sum(value)[value] from(select isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by  case " & groupby & " end,documenttype) as t group by category order by [value] desc"
                Dim pit As String = ""
                pit = Query

                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            ElseIf (btnchild.Text = "Save" And ddlvalue.ToString() = "Invoice LifeCycle") Then


                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""

                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                        datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            casequery1 = casequery1 & "when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                            If (ddoctype = "Invoice PO") Then
                                datequery = datequery & " when documenttype ='" & ddoctype & "' then '@PODate'"
                            ElseIf (ddoctype = "Invoice Non PO") Then
                                datequery = datequery & " when documenttype ='" & ddoctype & "' then '@NonPODate'"
                            End If
                            If (ddoctype = "Invoice PO") Then
                                groupby = groupby & "," & std & ",'@PODate'"
                            ElseIf (ddoctype = "Invoice Non PO") Then
                                groupby = groupby & "," & std & ",'@NonPODate'"
                            Else
                                Dim ViewName = "[@" & ddoctype.Replace(" ", "_") & "Date]"
                                groupby = groupby & "," & std & ",'" & ViewName & "'"
                            End If
                        End If
                    Next
                End If
                Dim query = "select top 5 t.category[category],isnull(convert(numeric(10,2),sum(case when Days<=15 then convert(numeric(10,2),Amount) end)/1000000),0)[0-15 Days]," _
                            & "isnull(convert(numeric(10,2),sum(case when Days>=16 and Days<=30 then convert(numeric(10,2),Amount)end)/1000000),0)[16-30 Days]," _
                            & "isnull(convert(numeric(10,2),sum(case when Days>=31 and Days<=45 then convert(numeric(10,2),Amount)end)/1000000),0)[31-45 Days]," _
                            & "isnull(convert(numeric(10,2),sum(case when Days>45 then convert(numeric(10,2),Amount)end)/1000000),0)[>45 Days]," _
                            & "isnull(convert(numeric(10,2),sum(case when Days<=15 then convert(numeric(10,2),Amount)end)/1000000),0) +" _
                            & "isnull(convert(numeric(10,2),sum(case when Days>=16 and Days<=30 then convert(numeric(10,2),Amount)end)/1000000),0) +" _
                            & "isnull(convert(numeric(10,2),sum(case when Days>=31 and Days<=45 then convert(numeric(10,2),Amount)end)/1000000),0) +" _
                            & "isnull(convert(numeric(10,2),sum(case when Days>45 then convert(numeric(10,2),Amount)end)/1000000),0)[Total] from(" _
                            & "select tid, case " & StrColumn & " end[category], case " & casequery1 & " end [Amount] ," _
                            & "datediff(dd,convert(date,case " & datequery & " end ,3)," _
                            & "(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))[Days]" _
                            & "from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & "and curstatus in ('archive') and @Date group by d.tid,documenttype " & groupby & " ) as t group by category order by [Total] desc"
                ''Dim Query = "select top 5 category,sum(value)[value] from(select isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by  case " & groupby & " end,documenttype) as t group by category order by [value] desc"
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            ElseIf (btnchild.Text = "Save" And ddlvalue.ToString() = "Invoice Distribution") Then


                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""

                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                        datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            casequery1 = casequery1 & "when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                            groupby = groupby & "," & std & ""

                        End If
                    Next
                End If
                Dim query = "select top 5 t.category[category],isnull(convert(numeric(10,2),sum(case when documenttype='invoice po' then convert(numeric(10,2),Amount)end)/1000000),0)[PO]," _
                            & "isnull(convert(numeric(10,2),sum(case when documenttype='Invoice non PO' then convert(numeric(10,2),Amount)end)/1000000),0)[NON PO]," _
                            & "isnull(convert(numeric(10,2),sum(case when documenttype='invoice po' then convert(numeric(10,2),Amount)end)/1000000),0)+" _
                            & "isnull(convert(numeric(10,2),sum(case when documenttype='Invoice non PO' then convert(numeric(10,2),Amount)end)/1000000),0)[sum] from (" _
                            & "select tid,case " & StrColumn & " end[category], case " & casequery1 & " end [Amount]," _
                            & "documenttype from mmm_mst_doc d where eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & "and curstatus not in ('rejected') and @Date group by d.tid,documenttype " & groupby & ") as t group by category order by [sum] desc"
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            ElseIf (btnchild.Text = "Save" And ddlvalue.ToString() = "Open Invoice Ageing") Then

                Dim vLoop As Integer = 0
                Do While (vLoop < Griddatediff.Rows.Count)
                    Dim gvrow As GridViewRow = Griddatediff.Rows(vLoop)
                    Dim DocumentType As String = gvrow.Cells(2).Text
                    Dim WorkFlow As String = gvrow.Cells(3).Text
                    Dim walias As String = gvrow.Cells(4).Text
                    Dim orderid As String = gvrow.Cells(5).Text
                    vLoop = (vLoop + 1)
                    con.Open()
                    Dim mss = New SqlCommand("Insert into mmm_mst_misdb_workflow(RefTid,DocumentType,WorkFlow,alias,orderid) values('" & hdnrefid.Value & "','" & DocumentType & "','" & WorkFlow & "','" & walias & "','" & orderid & "')", con)
                    mss.ExecuteNonQuery()
                    con.Close()
                Loop
                Dim workflowstatus As String = ""
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)

                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""

                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                        datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"

                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then

                            casequery1 = casequery1 & " when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                            If (ddoctype = "Invoice PO") Then
                                daterequired = daterequired & " when documenttype='" & ddoctype & "' then  @PODate"
                            ElseIf (ddoctype = "Invoice Non PO") Then
                                daterequired = daterequired & "  when documenttype='" & ddoctype & "' then @NonPODate"
                            Else
                                Dim ViewName = "[@" & ddoctype.Replace(" ", "_") & "Date]"
                                daterequired = daterequired & " when documenttype='" & ddoctype & "' then '" & ViewName & "'"
                            End If
                            If (ddoctype = "Invoice PO") Then
                                groupby = groupby & ",@PODate"
                            ElseIf (ddoctype = "Invoice Non PO") Then
                                groupby = groupby & ",@NonPODate"
                            Else
                                Dim ViewName = "[@" & ddoctype.Replace(" ", "_") & "Date]"
                                groupby = groupby & ",'" & ViewName & "'"
                            End If
                        End If
                    Next
                    newQuery1 = "select Tid,reftid,DocumentType,WorkFlow,alias,Orderid from mmm_mst_misdb_workflow where RefTid=" & hdnrefid.Value & ""
                    con = New SqlConnection(conStr)
                    da1 = New SqlDataAdapter(newQuery1, con)
                    Dim NameAlias As String = ""
                    da1.Fill(sdp)
                    If (sdp.Tables(0).Rows.Count > 0) Then
                        For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                            If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                workflowstatus = workflowstatus.Replace(",", "','")
                                NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                StrColumn = StrColumn & "when curstatus in ('" & workflowstatus & "') then '" & NameAlias & "'"
                            End If
                        Next
                    End If
                End If
                Dim query = "select t.category[category],convert(numeric(10,2),isnull(sum(case when Days<=5 then convert(numeric(10,2),Amount)end),0)/1000000)[0-5 Days]," _
                            & "convert(numeric(10,2),isnull(sum(case when Days>=6 and Days<=10 then convert(numeric(10,2),Amount)end),0)/1000000)[6-10 Days]," _
                            & "convert(numeric(10,2),isnull(sum(case when Days>11 and Days<=15 then convert(numeric(10,2),Amount)end),0)/1000000)[11-15 Days]," _
                            & "convert(numeric(10,2),isnull(sum(case when Days>15 then convert(numeric(10,2),Amount)end),0)/1000000)[>15 Days]," _
                            & "convert(numeric(10,2),isnull(sum(case when Days<=5 then convert(numeric(10,2),Amount)end),0)/1000000) +" _
                            & "convert(numeric(10,2),isnull(sum(case when Days>=6 and Days<=10 then convert(numeric(10,2),Amount)end),0)/1000000) +" _
                            & "convert(numeric(10,2),isnull(sum(case when Days>11 and Days<=15 then convert(numeric(10,2),Amount)end),0)/1000000) +" _
                            & "convert(numeric(10,2),isnull(sum(case when Days>15 then convert(numeric(10,2),Amount)end),0)/1000000)[TotalAmount] from (" _
                            & "select tid,case " & StrColumn & " end[category] ,case " & casequery1 & " end [Amount],datediff(dd,convert(date,case " & daterequired & " end ,3)," _
                            & "(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))[Days] from mmm_mst_doc d with(nolock) where Eid='" & Session("Eid") & "'" _
                            & "and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected','Archive')" _
                            & "group by curstatus, d.tid,documenttype " & groupby & ") as t group by category order by Totalamount desc"
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            ElseIf (btnchild.Text = "Save" And ddlvalue.ToString() = "SLA Performance") Then
                Dim vLoop As Integer = 0
                Do While (vLoop < Griddatediff.Rows.Count)
                    Dim gvrow As GridViewRow = Griddatediff.Rows(vLoop)
                    Dim DocumentType As String = gvrow.Cells(2).Text
                    Dim WorkFlow As String = gvrow.Cells(3).Text
                    Dim walias As String = gvrow.Cells(4).Text
                    Dim orderid As String = gvrow.Cells(5).Text
                    vLoop = (vLoop + 1)

                    con.Open()
                    Dim mss = New SqlCommand("Insert into mmm_mst_misdb_workflow(RefTid,DocumentType,WorkFlow,alias,orderid) values('" & hdnrefid.Value & "','" & DocumentType & "','" & WorkFlow & "','" & walias & "','" & orderid & "')", con)
                    mss.ExecuteNonQuery()
                    con.Close()
                Loop
                Dim workflowstatus As String = ""
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""

                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                        datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"

                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            casequery1 = casequery1 & "when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                        End If
                    Next
                    newQuery1 = "SELECT alias,STUFF (( SELECT ',' + workflow[workflow]FROM mmm_mst_misdb_workflow  where alias=w.alias FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')workflow from mmm_mst_misdb_workflow w where reftid=" & hdnrefid.Value & " group by alias"
                    con = New SqlConnection(conStr)
                    da1 = New SqlDataAdapter(newQuery1, con)
                    Dim NameAlias As String = ""
                    da1.Fill(sdp)
                    If (sdp.Tables(0).Rows.Count > 0) Then
                        For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                            If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                workflowstatus = workflowstatus.Replace(",", "','")
                                NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                StrColumn = StrColumn & "when category in ('" & workflowstatus & "') then '" & NameAlias & "'"
                            End If
                        Next
                    End If
                End If
                Dim query = "select  case " & StrColumn & " else category end[category],sum (convert(numeric(10,2),[Within SLA]))[Within SLA],sum (convert(numeric(10,2),[SLA Breached]))[SLA Breached]" _
                            & "from (select  t.category ,convert(numeric(10,2),isnull(sum(case when Type='Within SLA' then convert(numeric(10,2),Amount)end),0)/1000000)[Within SLA]," _
                            & "convert(numeric(10,2),isnull(sum(case when Type='SLA Breached' then convert(numeric(10,2),Amount)end),0)/1000000)[SLA Breached]" _
                            & "from (select distinct d.tid,case when aprstatus is not null and aprstatus not in ('RECALLED','REJECTED') then aprstatus when aprstatus is null then max(curstatus) end[category]," _
                            & "case " & casequery1 & "end [Amount],case when max(dl.ptat)>max(dl.atat) then 'SLA Breached' when max(dl.ptat)<=max(dl.atat) then 'Within SLA' end[Type]" _
                            & "from mmm_mst_doc d with(nolock) join mmm_doc_dtl dl on dl.docid=d.tid where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & "and curstatus not in ('rejected') group by d.tid,documenttype,aprstatus ) as t where category is not null  group by t.category) as d" _
                            & " group by case  " & StrColumn & " else category end"
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            Else
                If (btnchild.Text = "Update" And ddlvalue = "Expense Breakup") Then
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                        Next
                    End If
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            std = pds.Tables(0).Rows(t).Item("CatField")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                            QueryType = pds.Tables(0).Rows(t).Item("type")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                                casequery1 = casequery1 & "when max(documenttype)='" & ddoctype & "' then convert(numeric(10,2),sum(convert(numeric(10,2)," & valuefield & "))/1000000)"
                                groupby = groupby & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            End If
                        Next
                    End If
                    Dim Query = "select top 5 category,sum(value)[value]from (select isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date and @Date group by case " & groupby & " end,documenttype) as t group by category order by [value] desc"
                    Dim pit As String = ""
                    pit = Query
                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    BindDashbordConfig()
                    Me.ModalPopupAddchilditem.Hide()

                End If
                If (btnchild.Text = "Update" And ddlvalue.ToString() = "Supplier Spend Breakup" And ddltype.SelectedItem.Text = "Department") Then


                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                        Next
                    End If
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            std = pds.Tables(0).Rows(t).Item("CatField")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                            QueryType = pds.Tables(0).Rows(t).Item("type")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                                casequery1 = casequery1 & "when max(documenttype)='" & ddoctype & "' then convert(numeric(10,2),sum(convert(numeric(10,2)," & valuefield & "))/1000000)"
                                groupby = groupby & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            End If
                        Next
                    End If
                    Dim Query = "select top 5 category,sum(value)[value] from(select isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by  case " & groupby & " end,documenttype) as t group by category order by [value] desc"
                    Dim pit As String = ""
                    pit = Query

                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    Me.ModalPopupAddchilditem.Hide()
                    BindDashbordConfig()
                End If
                If (btnchild.Text = "Update" And ddlvalue.ToString() = "Invoice LifeCycle") Then


                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                        Next
                    End If
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""

                            std = pds.Tables(0).Rows(t).Item("CatField")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                            QueryType = pds.Tables(0).Rows(t).Item("type")
                            datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                            datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                                casequery1 = casequery1 & "when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                                If (ddoctype = "Invoice PO") Then
                                    datequery = datequery & " when documenttype ='" & ddoctype & "' then '@PODate'"
                                ElseIf (ddoctype = "Invoice Non PO") Then
                                    datequery = datequery & " when documenttype ='" & ddoctype & "' then '@NonPODate'"
                                End If
                                If (ddoctype = "Invoice PO") Then
                                    groupby = groupby & "," & std & ",'@PODate'"
                                ElseIf (ddoctype = "Invoice Non PO") Then
                                    groupby = groupby & "," & std & ",'@NonPODate'"
                                End If
                            End If
                        Next
                    End If
                    Dim query = "select top 5 t.category[category],isnull(convert(numeric(10,2),sum(case when Days<=15 then convert(numeric(10,2),Amount) end)/1000000),0)[0-15 Days]," _
                                & "isnull(convert(numeric(10,2),sum(case when Days>=16 and Days<=30 then convert(numeric(10,2),Amount)end)/1000000),0)[16-30 Days]," _
                                & "isnull(convert(numeric(10,2),sum(case when Days>=31 and Days<=45 then convert(numeric(10,2),Amount)end)/1000000),0)[31-45 Days]," _
                                & "isnull(convert(numeric(10,2),sum(case when Days>45 then convert(numeric(10,2),Amount)end)/1000000),0)[>45 Days]," _
                                & "isnull(convert(numeric(10,2),sum(case when Days<=15 then convert(numeric(10,2),Amount)end)/1000000),0) +" _
                                & "isnull(convert(numeric(10,2),sum(case when Days>=16 and Days<=30 then convert(numeric(10,2),Amount)end)/1000000),0) +" _
                                & "isnull(convert(numeric(10,2),sum(case when Days>=31 and Days<=45 then convert(numeric(10,2),Amount)end)/1000000),0) +" _
                                & "isnull(convert(numeric(10,2),sum(case when Days>45 then convert(numeric(10,2),Amount)end)/1000000),0)[Total] from(" _
                                & "select tid, case " & StrColumn & " end[category], case " & casequery1 & " end [Amount] ," _
                                & "datediff(dd,convert(date,case " & datequery & " end ,3)," _
                                & "(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))[Days]" _
                                & "from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                                & "and curstatus in ('archive') and @Date group by d.tid,documenttype " & groupby & " ) as t group by category order by [Total] desc"
                    ''Dim Query = "select top 5 category,sum(value)[value] from(select isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by  case " & groupby & " end,documenttype) as t group by category order by [value] desc"
                    Dim pit As String = ""
                    pit = query

                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    BindDashbordConfig()
                End If
                If (btnchild.Text = "Update" And ddlvalue.ToString() = "Invoice Distribution") Then


                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                        Next
                    End If
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""

                            std = pds.Tables(0).Rows(t).Item("CatField")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                            QueryType = pds.Tables(0).Rows(t).Item("type")
                            datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                            datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                                casequery1 = casequery1 & "when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                                groupby = groupby & "," & std & ""

                            End If
                        Next
                    End If
                    Dim query = "select top 5 t.category[category],isnull(convert(numeric(10,2),sum(case when documenttype='invoice po' then convert(numeric(10,2),Amount)end)/1000000),0)[PO]," _
                                & "isnull(convert(numeric(10,2),sum(case when documenttype='Invoice non PO' then convert(numeric(10,2),Amount)end)/1000000),0)[NON PO]," _
                                & "isnull(convert(numeric(10,2),sum(case when documenttype='invoice po' then convert(numeric(10,2),Amount)end)/1000000),0)+" _
                                & "isnull(convert(numeric(10,2),sum(case when documenttype='Invoice non PO' then convert(numeric(10,2),Amount)end)/1000000),0)[sum] from (" _
                                & "select tid,case " & StrColumn & " end[category], case " & casequery1 & " end [Amount]," _
                                & "documenttype from mmm_mst_doc d where eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                                & "and curstatus not in ('rejected') and @Date group by d.tid,documenttype " & groupby & ") as t group by category order by [sum] desc"
                    Dim pit As String = ""
                    pit = query
                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    Me.ModalPopupAddchilditem.Hide()
                    BindDashbordConfig()
                End If
                If (btnchild.Text = "Update" And ddlvalue = "Open Invoice Ageing") Then

                    Dim vLoop As Integer = 0
                    Do While (vLoop < Griddatediff.Rows.Count)
                        Dim gvrow As GridViewRow = Griddatediff.Rows(vLoop)
                        Dim DocumentType As String = gvrow.Cells(2).Text
                        Dim WorkFlow As String = gvrow.Cells(3).Text
                        Dim walias As String = gvrow.Cells(4).Text
                        Dim orderid As String = gvrow.Cells(5).Text
                        vLoop = (vLoop + 1)
                        con.Open()
                        Dim mss = New SqlCommand("Insert into mmm_mst_misdb_workflow(RefTid,DocumentType,WorkFlow,alias,orderid) values('" & hdnrefid.Value & "','" & DocumentType & "','" & WorkFlow & "','" & walias & "','" & orderid & "')", con)
                        mss.ExecuteNonQuery()
                        con.Close()
                    Loop
                    Dim workflowstatus As String = ""
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)

                    If (pds.Tables(0).Rows.Count > 0) Then
                        For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                        Next
                    End If
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""

                            std = pds.Tables(0).Rows(t).Item("CatField")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                            QueryType = pds.Tables(0).Rows(t).Item("type")
                            datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                            datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"

                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then

                                casequery1 = casequery1 & " when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                                If (ddoctype = "Invoice PO") Then
                                    daterequired = daterequired & " when documenttype='" & ddoctype & "' then  @PODate"
                                ElseIf (ddoctype = "Invoice Non PO") Then
                                    daterequired = daterequired & "  when documenttype='" & ddoctype & "' then @NonPODate"
                                Else
                                    Dim ViewName = "[@" & ddoctype.Replace(" ", "_") & "Date]"
                                    daterequired = daterequired & " when documenttype='" & ddoctype & "' then '" & ViewName & "'"
                                End If
                                If (ddoctype = "Invoice PO") Then
                                    groupby = groupby & ",@PODate"
                                ElseIf (ddoctype = "Invoice Non PO") Then
                                    groupby = groupby & ",@NonPODate"
                                Else
                                    Dim ViewName = "[@" & ddoctype.Replace(" ", "_") & "Date]"
                                    groupby = groupby & ",'" & ViewName & "'"
                                End If
                            End If
                        Next
                        newQuery1 = "select Tid,reftid,DocumentType,WorkFlow,alias,Orderid from mmm_mst_misdb_workflow where RefTid=" & hdnrefid.Value & ""
                        con = New SqlConnection(conStr)
                        da1 = New SqlDataAdapter(newQuery1, con)
                        Dim NameAlias As String = ""
                        da1.Fill(sdp)
                        If (sdp.Tables(0).Rows.Count > 0) Then
                            For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                                If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                    workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                    workflowstatus = workflowstatus.Replace(",", "','")
                                    NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                    StrColumn = StrColumn & "when curstatus in ('" & workflowstatus & "') then '" & NameAlias & "'"
                                End If
                            Next
                        End If
                    End If
                    Dim query = "select t.category[category],convert(numeric(10,2),isnull(sum(case when Days<=5 then convert(numeric(10,2),Amount)end),0)/1000000)[0-5 Days]," _
                                & "convert(numeric(10,2),isnull(sum(case when Days>=6 and Days<=10 then convert(numeric(10,2),Amount)end),0)/1000000)[6-10 Days]," _
                                & "convert(numeric(10,2),isnull(sum(case when Days>11 and Days<=15 then convert(numeric(10,2),Amount)end),0)/1000000)[11-15 Days]," _
                                & "convert(numeric(10,2),isnull(sum(case when Days>15 then convert(numeric(10,2),Amount)end),0)/1000000)[>15 Days]," _
                                & "convert(numeric(10,2),isnull(sum(case when Days<=5 then convert(numeric(10,2),Amount)end),0)/1000000) +" _
                                & "convert(numeric(10,2),isnull(sum(case when Days>=6 and Days<=10 then convert(numeric(10,2),Amount)end),0)/1000000) +" _
                                & "convert(numeric(10,2),isnull(sum(case when Days>11 and Days<=15 then convert(numeric(10,2),Amount)end),0)/1000000) +" _
                                & "convert(numeric(10,2),isnull(sum(case when Days>15 then convert(numeric(10,2),Amount)end),0)/1000000)[TotalAmount] from (" _
                                & "select tid,case " & StrColumn & " end[category] ,case " & casequery1 & " end [Amount],datediff(dd,convert(date,case " & daterequired & " end ,3)," _
                                & "(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))[Days] from mmm_mst_doc d with(nolock) where Eid='" & Session("Eid") & "'" _
                                & "and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected','Archive')" _
                                & "group by curstatus, d.tid,documenttype " & groupby & ") as t group by category order by Totalamount desc"
                    Dim pit As String = ""
                    pit = query
                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    Me.ModalPopupAddchilditem.Hide()
                    BindDashbordConfig()
                End If
            End If
            If (btnchild.Text = "Update" And ddlvalue = "SLA Performance") Then

                Dim vLoop As Integer = 0
                Do While (vLoop < Griddatediff.Rows.Count)
                    Dim gvrow As GridViewRow = Griddatediff.Rows(vLoop)
                    Dim DocumentType As String = gvrow.Cells(2).Text
                    Dim WorkFlow As String = gvrow.Cells(3).Text
                    Dim walias As String = gvrow.Cells(4).Text
                    Dim orderid As String = gvrow.Cells(5).Text
                    vLoop = (vLoop + 1)
                    con.Open()
                    Dim mss = New SqlCommand("Insert into mmm_mst_misdb_workflow(RefTid,DocumentType,WorkFlow,alias,orderid) values('" & hdnrefid.Value & "','" & DocumentType & "','" & WorkFlow & "','" & walias & "','" & orderid & "')", con)
                    mss.ExecuteNonQuery()
                    con.Close()
                Loop
                Dim workflowstatus As String = ""
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""

                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                        datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"

                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            casequery1 = casequery1 & "when documenttype='" & ddoctype & "' then max(" & valuefield & ")"
                        End If
                    Next
                    newQuery1 = "SELECT alias,STUFF (( SELECT ',' + workflow[workflow]FROM mmm_mst_misdb_workflow  where alias=w.alias FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')workflow from mmm_mst_misdb_workflow w where reftid=" & hdnrefid.Value & " group by alias"
                    con = New SqlConnection(conStr)
                    da1 = New SqlDataAdapter(newQuery1, con)
                    Dim NameAlias As String = ""
                    da1.Fill(sdp)
                    If (sdp.Tables(0).Rows.Count > 0) Then
                        For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                            If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                workflowstatus = workflowstatus.Replace(",", "','")
                                NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                StrColumn = StrColumn & "when category in ('" & workflowstatus & "') then '" & NameAlias & "'"
                            End If
                        Next
                    End If
                End If
                Dim query = "select  case " & StrColumn & " else category end[category],sum (convert(numeric(10,2),[Within SLA]))[Within SLA],sum (convert(numeric(10,2),[SLA Breached]))[SLA Breached]" _
                            & "from (select  t.category ,convert(numeric(10,2),isnull(sum(case when Type='Within SLA' then convert(numeric(10,2),Amount)end),0)/1000000)[Within SLA]," _
                            & "convert(numeric(10,2),isnull(sum(case when Type='SLA Breached' then convert(numeric(10,2),Amount)end),0)/1000000)[SLA Breached]" _
                            & "from (select distinct d.tid,case when aprstatus is not null and aprstatus not in ('RECALLED','REJECTED') then aprstatus when aprstatus is null then max(curstatus) end[category]," _
                            & "case " & casequery1 & "end [Amount],case when max(dl.ptat)>max(dl.atat) then 'SLA Breached' when max(dl.ptat)<=max(dl.atat) then 'Within SLA' end[Type]" _
                            & "from mmm_mst_doc d with(nolock) join mmm_doc_dtl dl on dl.docid=d.tid where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & "and curstatus not in ('rejected') group by d.tid,documenttype,aprstatus ) as t where category is not null  group by t.category) as d" _
                            & " group by case  " & StrColumn & " else category end"
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            End If
            Response.Redirect(Request.Url.AbsoluteUri)
            ' Me.ModalPopupAddchilditem.Hide()

            BindDashbordConfig()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Protected Sub EditshowchildItem_Click(sender As Object, e As ImageClickEventArgs)
        Try
            Div1.Visible = False
            Label1.Text = ""
            trtid.Visible = False
            Dim Docname As String = ""
            trwf.Visible = False
            addgridvalue.Text = "Update"
            Dim dropdownvalue As String = ""
            For i As Integer = 0 To Me.gvDashboardData.Rows.Count - 1
                dropdownvalue = Convert.ToString(Me.gvDashboardData.Rows(i).Cells(2).Text)
            Next
            ddldbname.Text = dropdownvalue.ToString()
            Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
            Docname = row.Cells(3).Text
            Dim ChildId As Integer = row.Cells(1).Text
            Dim pid As Integer = Convert.ToString(Me.gridchilditemdata.DataKeys(row.RowIndex).Value)

            Dim c As String() = New String(50) {}
            btnchild.Text = "Update"
            ViewState("pid") = pid
            hdnrefid.Value = pid
            Dim objDT As New DataTable()
            Dim ObDT As New DataTable()
            Using conn As SqlConnection = New SqlConnection()
                conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
                Using cmd As New SqlCommand()

                    conn.Open()
                    Dim cammandfillfrid = New SqlCommand("select Tid,DocumentType,WorkFlow,alias,orderid from mmm_mst_misdb_workflow where RefTid=" & pid & " and Documenttype='" & Docname & "'", conn)
                    Dim ad1 As New SqlDataAdapter()
                    ad1 = New SqlDataAdapter(cammandfillfrid)
                    ad1.Fill(ObDT)
                    Griddatediff.DataSource = ObDT
                    Griddatediff.DataBind()
                    conn.Close()

                    cmd.CommandText = ("select  Tid,RefTid,DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where Tid=" & ChildId & " and  DocumentType='" & Docname & "' and RefTid=" & pid)
                    cmd.Connection = conn
                    conn.Open()
                    Using sdr As SqlDataReader = cmd.ExecuteReader()
                        While sdr.Read()
                            txtchildTid.Text = sdr("Tid")
                            ddldoctype.ClearSelection()
                            BindDocumentType()
                            ddldoctype.SelectedItem.Text = sdr("DocumentType")
                            ddltype.ClearSelection()
                            For Each Item As ListItem In ddltype.Items
                                If Item.Value.ToString().Contains(sdr("Type")) Then
                                    Item.Selected = True
                                Else
                                    Item.Selected = False
                                End If
                            Next
                            ddldoctype_SelectedIndexChanged(Me, EventArgs.Empty)
                            Populateworkflowstatus()
                            ddlcatfield.ClearSelection()
                            PopulateFilterField()
                            For Each Item As ListItem In ddlcatfield.Items
                                If Item.Value.ToString().Contains(sdr("catField") & "-") Then
                                    Item.Selected = True
                                Else
                                    Item.Selected = False
                                End If
                            Next
                            ddlcatfield_SelectedIndexChanged(Me, EventArgs.Empty)
                            For Each Item1 As ListItem In ddlvaluefield.Items
                                If Item1.Value.ToString().Contains(sdr("ValueField")) Then
                                    Item1.Selected = True
                                Else
                                    Item1.Selected = False
                                End If
                            Next
                            Datefield()
                            ddlrecvdatefield.ClearSelection()
                            For Each Item2 As ListItem In ddlrecvdatefield.Items
                                If Item2.Value.ToString().Contains(sdr("RecdateField")) Then
                                    Item2.Selected = True
                                Else
                                    Item2.Selected = False
                                End If
                            Next
                            ddlrecvdatefield_SelectedIndexChanged(Me, EventArgs.Empty)
                            ddlinvdatefield.ClearSelection()
                            Datefield1()

                            For Each Item3 As ListItem In ddlinvdatefield.Items
                                If Item3.Value.ToString().Contains(sdr("InvDateField")) Then
                                    Item3.Selected = True
                                Else
                                    Item3.Selected = False
                                End If
                            Next
                        End While
                    End Using
                End Using
                Me.ModalPopupExtendershowchilditem.Hide()
                updatePanel1.Update()
                Me.ModalPopupAddchilditem.Show()
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Protected Sub imgDelete_Click(sender As Object, e As ImageClickEventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim sid As Integer = Convert.ToString(Me.Griddatediff.DataKeys(row.RowIndex).Value)
        ViewState("sid") = sid
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                Dim vLoop As Integer = 0
                Do While (vLoop < Griddatediff.Rows.Count)
                    Dim gvrow As GridViewRow = Griddatediff.Rows(vLoop)
                    Dim DocumentType As String = gvrow.Cells(2).Text
                    Dim WorkFlow As String = gvrow.Cells(3).Text
                    Dim walias As String = gvrow.Cells(4).Text
                    Dim orderid As String = gvrow.Cells(5).Text
                    vLoop = (vLoop + 1)
                    conn.Open()
                    Dim mycommand = New SqlCommand("Delete from  mmm_mst_misdb_workflow where Tid=" & sid & " and DocumentType='" & DocumentType & "' and WorkFlow='" & WorkFlow & "' and alias='" & walias & "' and orderid='" & orderid & "'", conn)
                    mycommand.ExecuteNonQuery()

                    conn.Close()
                    BindGrid()
                Loop
            End Using
        End Using
    End Sub

    Protected Sub documentTypeddl_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateDetailField()
    End Sub

    Protected Sub btnsavedetail_Click(sender As Object, e As EventArgs)
        Dim StrColumn As String = ""
        Dim casequery1 As String = ""
        Dim datequery As String = ""
        Dim Incondition As String = ""
        Dim builder As New StringBuilder
        Dim build As String = ""
        Dim fdv As String = ""
        Dim groupby As String = ""
        Dim daterequired As String = ""
        Dim tblquery As String = ""
        Dim tbchitem As DataTable
        Dim s As String = ""

        Dim upd As New DataSet
        Dim newQuery As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim sdp As New DataSet
        Dim da As New SqlDataAdapter("", con)
        Dim da1 As New SqlDataAdapter("", con)
        Dim QueryNew As String = ""
        Dim predata As New DataSet
        Dim csv As String = ""
        Dim lstDocumenettype As New ArrayList
        Dim QueryType As String = ""
        Dim ddlvalue As String = ""
        Dim newQuery1 As String = ""
        Dim dspname As String = ""
        Dim vLoop As Integer = 0
        Dim Loopquery As String = ""
        Dim DATACSD As New DataSet
        Dim std As String = ""

        Dim ddoctype As String = ""
        Dim Displayname1 As String = ""

        Loopquery = "select Distinct DisplayName from mmm_mst_misdb_details where refTid=" & hdndetail.Value & ""
        con = New SqlConnection(conStr)
        da = New SqlDataAdapter(Loopquery, con)
        da.Fill(DATACSD)
        If (DATACSD.Tables(0).Rows.Count > 0) Then
            For m As Integer = 0 To DATACSD.Tables(0).Rows.Count - 1
                Dim pds As New DataSet

                dspname = DATACSD.Tables(0).Rows(m).Item("DisplayName")
                newQuery = "select Tid,RefTid,DisplayName,DocumentType,fields from mmm_mst_misdb_details where RefTid=" & hdndetail.Value & " and DisplayName='" & dspname & "'"
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(i).Item("DocumentType")))
                    Next
                End If
                'StrColumn = StrColumn & " end '" & dspname & "'"
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        std = pds.Tables(0).Rows(t).Item("fields")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        Displayname1 = pds.Tables(0).Rows(t).Item("DisplayName")
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            If (predata.Tables(0).Rows(t).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            Else
                                StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then " & std & " "
                            End If
                        End If
                    Next
                    StrColumn = "case " & StrColumn & " end [" & dspname & "] , "
                    s = s & StrColumn
                    StrColumn = ""
                End If
            Next
        End If
        Dim query = "select  " & s & " " _
                    & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                    & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                    & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')"
        Dim pit As String = ""
        pit = query
        Dim objDC1 As New DataClass()
        Dim ht1 As New Hashtable
        ht1.Add("@Tid", hdndetail.Value)
        ht1.Add("@QueryType", Dropdowntype.SelectedItem.Text)
        ht1.Add("@Query", pit)
        objDC1.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht1)
        Me.ModalPopupAddchilditem.Hide()
        Response.Redirect(Request.Url.AbsoluteUri)
        BindDashbordConfig()
    End Sub

    Protected Sub showDetailItem(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            Dim showdetailitem As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(showdetailitem.NamingContainer, GridViewRow)
            Dim RefTid As Integer = Convert.ToString(Me.gvDashboardData.DataKeys(row.RowIndex).Value)

            Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(strConnString)
            Dim cmd As New SqlCommand()
            Div2.Visible = False
            Label2.Text = ""

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "uspGetDBDetailitemvalue"
            cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = RefTid
            cmd.Connection = con
            Try
                con.Open()
                detailgriditem.EmptyDataText = "No Records Found"
                detailgriditem.DataSource = cmd.ExecuteReader()
                detailgriditem.DataBind()
            Catch ex As Exception
                Throw ex
            Finally
                con.Close()
                con.Dispose()
            End Try
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Protected Sub EditshowdetailItem_Click(sender As Object, e As ImageClickEventArgs)
        Dim dropdownvalue As String = ""

        ddldbname.Text = dropdownvalue.ToString()
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        'Docname = row.Cells(3).Text
        dropdownvalue = row.Cells(4).Text
        Dim ChildId As Integer = row.Cells(1).Text
        Dim pid As Integer = Convert.ToString(Me.detailgriditem.DataKeys(row.RowIndex).Value)

        Dim c As String() = New String(50) {}
        btnsavedetail.Text = "Update"
        btnadddetail.Text = "Update"
        ViewState("pid") = pid
        hdnrefid.Value = pid
        Dim objDT As New DataTable()
        Dim ObDT As New DataTable()
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                cmd.CommandText = ("select  Tid,RefTid,DisplayName,DocumentType,Fields from mmm_mst_misdb_details where Tid=" & ChildId & " and  DocumentType='" & dropdownvalue & "' and RefTid=" & pid)
                cmd.Connection = conn
                conn.Open()
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    While sdr.Read()
                        ddlDisplayName.SelectedItem.Text = sdr("DisplayName")
                        DocumentTypeBind()
                        documentTypeddl.SelectedItem.Text = sdr("DocumentType")
                        documentTypeddl_SelectedIndexChanged(Me, EventArgs.Empty)
                        fieldsdropdown.ClearSelection()
                        PopulateDetailField()
                        For Each Item3 As ListItem In fieldsdropdown.Items
                            If Item3.Value.ToString().Contains(sdr("Fields")) Then
                                Item3.Selected = True
                            Else
                                Item3.Selected = False
                            End If
                        Next
                    End While
                End Using
            End Using
            updtpnladddetailsfields.Update()
            Me.ModalPopupExtenderDetailfield.Show()
        End Using
    End Sub
End Class
