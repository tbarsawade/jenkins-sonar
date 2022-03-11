Imports System.Data
Imports System.Data.SqlClient

Partial Class PreRoleDataFilter
    Inherits System.Web.UI.Page
    Dim dord As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            bindGridLoc()
        End If
    End Sub

    Public Sub bindGridLoc()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select DF.TID, DF.Rolename , DF.documenttype,MF.displayName from MMM_PreRole_dataFilter DF left outer join MMM_MST_Fields MF on DF.Documenttype=MF.Documenttype  and DF.fldmapping=MF.fieldMapping where DF.eid=" & Session("EID") & "", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        ds.Dispose()
        oda.Dispose()
        con.Close()
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
    'Public Sub bindstatus()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select FormID,FormName from MMM_MST_FORMS where FormType='DOCUMENT' and EID=" & Session("eid") & " order by formname", con)
    '    oda.SelectCommand.CommandType = CommandType.Text
    '    Dim ds As New DataSet
    '    oda.Fill(ds, "data")
    '    ddlConfigstatus.Items.Clear()
    '    ddlConfigstatus.DataSource = ds
    '    ddlConfigstatus.DataTextField = "FormName"
    '    ddlConfigstatus.DataValueField = "FormID"
    '    ddlConfigstatus.DataBind()
    '    ddlConfigstatus.Items.Insert(0, "Select")
    '    updConfigStatus.Update()
    '    con.Close()
    '    oda.Dispose()
    '    ds.Dispose()
    'End Sub


    Public Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        ' for role name bind 
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_ROle where eid=" & Session("EID") & " and roletype='Pre Type' and roleName <> 'SU' ", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "Role")
        ddlDocType.Items.Clear()
        ddlUserRoleName.Items.Clear()
        ddlUserRoleName.DataSource = ds.Tables("Role")
        ddlUserRoleName.DataTextField = "roleName"
        ddlUserRoleName.DataValueField = "roleid"
        ddlUserRoleName.DataBind()
        ddlUserRoleName.Items.Insert(0, "Please Select")
        ' for username bind in dropdown
        oda.SelectCommand.CommandText = "select * from MMM_MST_FORMS where eid=" & Session("EID") & " and formsource='MENU DRIVEN'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "DOC")
        ddlDocType.DataSource = ds.Tables("DOC")
        ddlDocType.DataTextField = "Formname"
        ddlDocType.DataBind()
        ddlDocType.Items.Insert(0, "Please Select")
        con.Dispose()
        oda.Dispose()
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click

        If ddlUserRoleName.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "User Role is not valid"
            updatePanelEdit.Update()
            Exit Sub
        End If
        If ddlDocType.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Doc type is not valid"
            updatePanelEdit.Update()
            Exit Sub
        End If

        If btnActUserSave.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertPreRoledataFilter", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@RoleName", ddlUserRoleName.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("@Documenttype", ddlDocType.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("@fldMapping", ddlDocFldMapping.SelectedItem.Value())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim i As Integer = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            If i = 0 Then
                bindGridLoc()
                lblRecord.Text = "Role Data Filter Created successfully "

            Else
                lblMsgEdit.Text = "Role Data Filter not created "
            End If
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()
        End If
    End Sub

    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlDocType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_Fields where eid=" & Session("EID") & " and documenttype='" & ddlDocType.SelectedItem.Text & "' ", con)
        Dim ds As New DataSet()
        ddlDocFldMapping.Items.Clear()
        oda.Fill(ds, "Fields")
        ddlDocFldMapping.DataSource = ds.Tables("Fields")
        ddlDocFldMapping.DataTextField = "displayname"
        ddlDocFldMapping.DataValueField = "fieldmapping"
        ddlDocFldMapping.DataBind()
        ddlDocFldMapping.Items.Insert(0, "Please Select")
        con.Close()
        oda.Dispose()
        updatePanelEdit.Update()

    End Sub


    'Public Sub ConfigureStatus(ByVal sender As Object, ByVal e As System.EventArgs)
    '    txtConfDord.Text = ""
    '    txtConfAmendment.Text = ""
    '    txtConfRecall.Text = ""
    '    txtConfCancel.Text = ""
    '    lblConfigStatus.Text = ""
    '    txtConfAllSplt.Text = ""
    '    txtConfDocEdit.Text = ""
    '    bindstatus()
    '    chkConfManByOtherRole.Checked = False
    '    chkCofAllowSkp.Checked = False
    '    ddlConfRoleName.Items.Clear()
    '    MP_ConfigStatus.Show()
    'End Sub

    'Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
    '    chkActive.Checked = False
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

    '    btnActUserSave.Text = "Update"
    '    ViewState("pid") = pid
    '    ddlDoctype.SelectedIndex = ddlDoctype.Items.IndexOf(ddlDoctype.Items.FindByText(row.Cells(1).Text))

    '    '' added by sunil for validation of default status on 26 sep 
    '    If UCase(row.Cells(2).Text) = "ARCHIVE" And Trim(row.Cells(1).Text) = "&nbsp;" Then
    '        lblRecord.Text = "This (Archive) is system generated field, can't Update"
    '        Exit Sub
    '    End If

    '    If row.Cells(2).Text = "UPLOADED" And Trim(row.Cells(1).Text) = "&nbsp;" Then
    '        lblRecord.Text = "This (Uploaded) is system generated field, can't Update"
    '        Exit Sub
    '    End If

    '    If row.Cells(2).Text = "UPLOADED" And Trim(row.Cells(1).Text) <> "" Then
    '        lblRecord.Text = "To edit this First Status, please click on Configure button at top right corner"
    '        Exit Sub
    '    End If

    '    If UCase(row.Cells(2).Text) = "ARCHIVE" And Trim(row.Cells(1).Text) <> "" Then
    '        lblRecord.Text = "To edit this First Status, please click on Configure button at top right corner"
    '        Exit Sub
    '    End If

    '    '' added by sunil for validation of default status on 26 sep  - ends 

    '    'lblMsgDelete.Text = "This (Archive) is system generated field, can't Update"
    '    ''btnActDelete.Text = "Cancel"
    '    'btnActDelete.Visible = False
    '    'Me.updatePanelDelete.Update()
    '    'Me.btnDelete_ModalPopupExtender.Show()

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select * From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and tid=" & ViewState("pid").ToString() & "", con)
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")
    '    If ds.Tables("data").Rows.Count > 0 Then
    '        If (ds.Tables("data").Rows(0).Item("isauth").ToString()) = 0 Then
    '            chkActive.Checked = False
    '        Else
    '            chkActive.Checked = True
    '        End If
    '        If (ds.Tables("data").Rows(0).Item("isAllowSkip").ToString()) = 0 Then
    '            chkAllowSkp.Checked = False
    '        Else
    '            chkAllowSkp.Checked = True
    '        End If

    '        If ds.Tables("data").Rows(0).Item("ManagebyotherRole") = 0 Then
    '            chkManByOthrRole.Checked = False
    '        Else
    '            chkManByOthrRole.Checked = True
    '            oda.SelectCommand.CommandText = "select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & ""
    '            oda.SelectCommand.CommandType = CommandType.Text
    '            oda.Fill(ds, "rolename")
    '            If ds.Tables("rolename").Rows.Count > 0 Then
    '                ddlRoleName.DataSource = ds.Tables("rolename")
    '                ddlRoleName.DataTextField = "RoleName"
    '                ddlRoleName.DataValueField = "RoleID"
    '                ddlRoleName.DataBind()
    '                ddlRoleName.Items.Insert(0, "Please Select")
    '                ddlRoleName.Enabled = True
    '                ddlRoleName.SelectedIndex = ddlRoleName.Items.IndexOf(ddlRoleName.Items.FindByText(ds.Tables("data").Rows(0).Item("Rolename")))
    '            Else
    '                lblManByother.Text = "There is No Role in Role Master"
    '            End If

    '        End If
    '        txtStatusName.Text = ds.Tables("data").Rows(0).Item("statusname").ToString()
    '        txtDord.Text = ds.Tables("data").Rows(0).Item("dord").ToString()
    '        txtAmendment.Text = ds.Tables("data").Rows(0).Item("amendment").ToString()
    '        txtRecall.Text = ds.Tables("data").Rows(0).Item("recall").ToString()
    '        txtCancel.Text = ds.Tables("data").Rows(0).Item("cancel").ToString()
    '        txtAppCap.Text = ds.Tables("data").Rows(0).Item("Approve").ToString()
    '        txtRejCap.Text = ds.Tables("data").Rows(0).Item("Reject").ToString()
    '        txtRecCap.Text = ds.Tables("data").Rows(0).Item("Reconsider").ToString()
    '        txtAllowSplt.Text = ds.Tables("data").Rows(0).Item("split").ToString()
    '        txtDocEdit.Text = ds.Tables("data").Rows(0).Item("Edit").ToString()
    '        updatePanelEdit.Update()
    '        'updtabpnlupdate.Update()
    '        oda.Dispose()
    '        con.Close()
    '        Me.btnEdit_ModalPopupExtender.Show()
    '        ' End If
    '    End If
    'End Sub

    'Protected Sub editRejHit(ByVal sender As Object, ByVal e As System.EventArgs)

    '    lblRejectStatus.Text = ""
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
    '    btnActUserSave.Text = "Update"
    '    ViewState("tid") = pid

    '    '' added by sunil for validation of default status on 26 sep 
    '    If UCase(row.Cells(2).Text) = "ARCHIVE" Then
    '        lblRecord.Text = "This (Archive) is system generated field, can't Update"
    '        Exit Sub
    '    End If

    '    If row.Cells(2).Text = "UPLOADED" And Trim(row.Cells(1).Text) = "&nbsp;" Then
    '        lblRecord.Text = "This (Uploaded) is system generated field, can't Update"
    '        Exit Sub
    '    End If

    '    If row.Cells(2).Text = "UPLOADED" And Trim(row.Cells(1).Text) <> "" Then
    '        lblRecord.Text = "To edit this First Status, please click on Configure button at top right corner"
    '        Exit Sub
    '    End If
    '    '' added by sunil for validation of default status on 26 sep  - ends 

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)

    '    Dim oda As New SqlDataAdapter("", con)
    '    'oda.SelectCommand.CommandText = "select statusname,tid From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and tid=" & ViewState("tid") & ""
    '    'Dim ds As New DataSet()
    '    'oda.Fill(ds, "statusname")

    '    oda.SelectCommand.CommandText = "select statusname,tid From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and Documenttype='" & row.Cells(1).Text & "' and statusname not in('" & row.Cells(2).Text & "') and dord <='" & row.Cells(3).Text & "' "
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")

    '    ddlRejStatus.DataSource = ds
    '    ddlRejStatus.DataTextField = "statusname"
    '    ddlRejStatus.DataValueField = "tid"
    '    ddlRejStatus.DataBind()
    '    ddlRejStatus.Items.Insert(0, "DEFAULT")
    '    ddlRejStatus.Items.Insert(1, "UPLOADED")
    '    ddlRejStatus.SelectedIndex = ddlRejStatus.Items.IndexOf(ddlRejStatus.Items.FindByText(row.Cells(8).Text))

    '    'dord = Val(row.Cells(3).Text)
    '    txtRejDocumenttype.Text = row.Cells(1).Text
    '    txtRejStatusname.Text = row.Cells(2).Text

    '    updatePanel_rejectstatus.Update()
    '    Me.ModalPopupExtender_RejectStatus.Show()


    'End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("delete From MMM_PreRole_dataFilter where eid=" & Session("EID") & " and tid=" & ViewState("Did") & "", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()
        bindGridLoc()
        lblRecord.Visible = True
        lblRecord.Text = " Deleted Successfully"
        updatePanelEdit.Update()
        btnDelete_ModalPopupExtender.Hide()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblRecord.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Record? " & row.Cells(1).Text
        btnActDelete.Visible = True
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub



    'Protected Sub Search(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_WORKFLOW_STATUS where " & ddlLocationName.SelectedItem.Value & " like '" & txtValue.Text & "%' and EID='" & Session("EID") & "' ", con)
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")
    '    gvData.DataSource = ds.Tables("data")
    '    gvData.DataBind()
    '    If gvData.Rows.Count = 0 Then
    '        lblRecord.Visible = True
    '        lblRecord.Text = "No Record Found"
    '    Else
    '        lblRecord.Visible = False
    '    End If
    '    ds.Dispose()
    '    oda.Dispose()
    '    con.Close()
    'End Sub

    'Protected Sub TabMovement_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabMovement.Load
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select * From MMM_MST_WOrkflow_status where EID='" & Session("EID") & "' ", con)
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")
    '    gvRejdata.DataSource = ds.Tables("data")
    '    gvRejdata.DataBind()
    '    ds.Dispose()
    '    oda.Dispose()
    '    con.Close()
    'End Sub

    'Protected Sub TabDetail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabDetail.Load
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select * From MMM_MST_WOrkflow_status where EID='" & Session("EID") & "'", con)
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")
    '    gvData.DataSource = ds.Tables("data")
    '    gvData.DataBind()
    '    ds.Dispose()
    '    oda.Dispose()
    '    con.Close()
    '    gvData.Visible = True
    'End Sub

    Protected Sub RefreshPanel(ByVal sender As Object, ByVal e As EventArgs)
        bindGridLoc()
    End Sub

    Protected Sub btnRejStatus_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRejStatus.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "' and tid=" & (ViewState("tid")) & " "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows(0).Item("dord") < ViewState("dord") Then
            lblRejectStatus.Text = "Reject status not valid"
            Exit Sub
        End If
        Dim od As New SqlDataAdapter("uspRejStatus", con)
        od.SelectCommand.CommandType = CommandType.StoredProcedure
        od.SelectCommand.Parameters.AddWithValue("pid", Val(ViewState("tid").ToString))
        od.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        If ddlRejStatus.SelectedItem.Text = "Please Select" Then
            od.SelectCommand.Parameters.AddWithValue("rejectStatus", "NULL")
        Else
            od.SelectCommand.Parameters.AddWithValue("rejectStatus", ddlRejStatus.SelectedItem.Text)
        End If


        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        od.SelectCommand.ExecuteNonQuery()
        gridbind()
        lblRecord.Visible = True
        lblRecord.Text = " Workflow Rejected Status Added successfully "
        Me.updatePanelEdit.Update()
        ModalPopupExtender_RejectStatus.Hide()
        con.Dispose()
        oda.Dispose()
        con.Close()
        od.Dispose()
    End Sub

    Protected Sub gridbind()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As New SqlDataAdapter("", con)

        oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        con.Dispose()
        oda.Dispose()
        con.Close()

    End Sub

    Protected Sub ddlRejStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRejStatus.SelectedIndexChanged

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select dord From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "' and statusname='" & ddlRejStatus.SelectedItem.Text & "' "
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        ViewState("dord") = ds.Tables("data").Rows(0).Item("dord")
        con.Dispose()
        oda.Dispose()
        con.Close()


    End Sub

    'Protected Sub btnConfigStatusSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfigStatusSave.Click

    '    If ddlConfigstatus.SelectedItem.Text = "Select" Then
    '        lblConfigStatus.Text = "PLease select Document Type"
    '        updConfigStatus.Update()
    '        Exit Sub
    '    End If
    '    If ddlstatusNam.SelectedItem.Text = "Select" Then
    '        ddlstatusNam.Text = "PLease select Document Type"
    '        updConfigStatus.Update()
    '        Exit Sub
    '    End If
    '    If Not IsNumeric(txtConfDord.Text) Then
    '        lblConfigStatus.Text = "Display order should be numeric"
    '        updConfigStatus.Update()
    '        Exit Sub
    '    End If
    '    If Len(Trim(txtConfAmendment.Text)) > 0 And Len(Trim(txtConfAmendment.Text)) < 2 Then
    '        lblConfigStatus.Text = "Please enter minimum 2 characters in Amendment"
    '    End If
    '    If Len(Trim(txtConfRecall.Text)) > 0 And Len(Trim(txtConfRecall.Text)) < 2 Then
    '        lblConfigStatus.Text = "Please enter minimum 2 characters in Recall"
    '    End If
    '    If Len(Trim(txtConfCancel.Text)) > 0 And Len(Trim(txtConfCancel.Text)) < 2 Then
    '        lblConfigStatus.Text = "Please enter minimum 2 characters in Cancel"
    '    End If
    '    If chkConfManByOtherRole.Checked = True Then
    '        If ddlConfRoleName.SelectedItem.Text = "Please Select" Then
    '            lblConfigStatus.Text = "PLease Select Role Name"
    '            Exit Sub
    '        End If
    '    End If



    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("uspWorkflowStausConfig", con)
    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '    oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
    '    oda.SelectCommand.Parameters.AddWithValue("doctype", ddlConfigstatus.SelectedItem.Text)
    '    oda.SelectCommand.Parameters.AddWithValue("statusName", ddlstatusNam.SelectedItem.Text)
    '    oda.SelectCommand.Parameters.AddWithValue("dispOrd", txtConfDord.Text)
    '    If Len(Trim(txtConfAmendment.Text)) > 0 Then
    '        oda.SelectCommand.Parameters.AddWithValue("amendment", txtConfAmendment.Text)
    '    End If
    '    If Len(Trim(txtConfRecall.Text)) > 0 Then
    '        oda.SelectCommand.Parameters.AddWithValue("Recall", txtConfRecall.Text)
    '    End If
    '    If Len(Trim(txtConfCancel.Text)) > 0 Then
    '        oda.SelectCommand.Parameters.AddWithValue("cancel", txtConfCancel.Text)
    '    End If
    '    If chkConfManByOtherRole.Checked = True Then
    '        oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 1)
    '        oda.SelectCommand.Parameters.AddWithValue("RoleName", ddlConfRoleName.SelectedItem.Text)
    '    Else
    '        oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 0)
    '    End If
    '    If chkCofAllowSkp.Checked = True Then
    '        oda.SelectCommand.Parameters.AddWithValue("isallow", 1)
    '    Else
    '        oda.SelectCommand.Parameters.AddWithValue("isallow", 0)
    '    End If
    '    If Len(Trim(txtConfAllSplt.Text)) > 0 Then
    '        oda.SelectCommand.Parameters.AddWithValue("split", txtConfAllSplt.Text)
    '    End If
    '    If Len(Trim(txtConfDocEdit.Text)) > 0 Then
    '        oda.SelectCommand.Parameters.AddWithValue("docEdit", txtConfDocEdit.Text)
    '    End If




    '    'If chkConfSplt.Checked = True Then
    '    '    oda.SelectCommand.Parameters.AddWithValue("split", 1)
    '    'Else
    '    '    oda.SelectCommand.Parameters.AddWithValue("split", 0)
    '    'End If
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    oda.SelectCommand.ExecuteScalar()
    '    bindGridLoc()
    '    lblRecord.Text = "Configure default/first status has been created"
    '    oda.Dispose()
    '    con.Close()
    '    MP_ConfigStatus.Hide()
    'End Sub

    'Protected Sub chkManByOthrRole_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkManByOthrRole.CheckedChanged
    '    If chkManByOthrRole.Checked = True Then
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As SqlConnection = New SqlConnection(conStr)
    '        Dim oda As New SqlDataAdapter("select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & "", con)
    '        oda.SelectCommand.CommandType = CommandType.Text
    '        Dim ds As New DataSet()
    '        oda.Fill(ds, "data")
    '        If ds.Tables("data").Rows.Count > 0 Then
    '            ddlRoleName.DataSource = ds.Tables("data")
    '            ddlRoleName.DataTextField = "RoleName"
    '            ddlRoleName.DataValueField = "RoleID"
    '            ddlRoleName.DataBind()
    '            ddlRoleName.Items.Insert(0, "Please Select")
    '            ddlRoleName.Enabled = True
    '        Else
    '            lblManByother.Text = "There is No Role in Role Master"
    '        End If
    '        con.Close()
    '        oda.Dispose()
    '    Else
    '        ddlRoleName.Enabled = False

    '    End If
    '    updatePanelEdit.Update()

    'End Sub

    Protected Sub chkConfManByOtherRole_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkConfManByOtherRole.CheckedChanged
        If chkConfManByOtherRole.Checked = True Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As New SqlDataAdapter("select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & "", con)
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                ddlConfRoleName.DataSource = ds.Tables("data")
                ddlConfRoleName.DataTextField = "RoleName"
                ddlConfRoleName.DataValueField = "RoleID"
                ddlConfRoleName.DataBind()
                ddlConfRoleName.Items.Insert(0, "Please Select")
                ddlConfRoleName.Enabled = True
            Else
                lblConfManByother.Text = "There is No Role in Role Master"
            End If
            oda.Dispose()
            con.Close()
        Else
            ddlConfRoleName.Enabled = False
        End If
        updConfigStatus.Update()
    End Sub

    'Protected Sub ddlstatusNam_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlstatusNam.SelectedIndexChanged
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("select * from MMM_MST_WORKFLOW_STATUS_CONFIG where EID=" & Session("EID") & " and Documenttype='" & ddlConfigstatus.SelectedItem.Text & "' and statusname='" & ddlstatusNam.SelectedItem.Text & "' ", con)
    '    oda.SelectCommand.CommandType = CommandType.Text
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")
    '    If ds.Tables("data").Rows.Count > 0 Then
    '        txtConfAmendment.Text = ds.Tables("data").Rows(0).Item("Amendment").ToString()
    '        txtConfRecall.Text = ds.Tables("data").Rows(0).Item("Recall").ToString()
    '        txtConfCancel.Text = ds.Tables("data").Rows(0).Item("Cancel").ToString()
    '        txtConfDord.Text = ds.Tables("data").Rows(0).Item("Dord").ToString()
    '        chkConfManByOtherRole.Checked = ds.Tables("data").Rows(0).Item("managebyotherrole")
    '        txtConfAllSplt.Text = ds.Tables("data").Rows(0).Item("split").ToString()
    '        txtConfDocEdit.Text = ds.Tables("data").Rows(0).Item("Edit").ToString()
    '        If ds.Tables("data").Rows(0).Item("isallowskip") = 0 Then
    '            chkCofAllowSkp.Checked = False
    '        Else
    '            chkCofAllowSkp.Checked = True
    '        End If
    '        'If ds.Tables("data").Rows(0).Item("splitfeature") = 0 Then
    '        '    chkConfSplt.Checked = False
    '        'Else
    '        '    chkConfSplt.Checked = True
    '        'End If

    '        If ds.Tables("data").Rows(0).Item("ManagebyotherRole") = 0 Then
    '            chkConfManByOtherRole.Checked = False
    '        Else
    '            chkConfManByOtherRole.Checked = True
    '            oda.SelectCommand.CommandText = "select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & ""
    '            oda.SelectCommand.CommandType = CommandType.Text
    '            oda.Fill(ds, "rolename")
    '            If ds.Tables("rolename").Rows.Count > 0 Then
    '                ddlConfRoleName.DataSource = ds.Tables("rolename")
    '                ddlConfRoleName.DataTextField = "RoleName"
    '                ddlConfRoleName.DataValueField = "RoleID"
    '                ddlConfRoleName.DataBind()
    '                ddlConfRoleName.Items.Insert(0, "Please Select")
    '                ddlConfRoleName.Enabled = True
    '                ddlConfRoleName.SelectedIndex = ddlConfRoleName.Items.IndexOf(ddlConfRoleName.Items.FindByText(ds.Tables("data").Rows(0).Item("Rolename")))
    '            Else
    '                lblManByother.Text = "There is No Role in Role Master"
    '            End If
    '        End If
    '    Else
    '        txtConfAmendment.Text = ""
    '        txtConfRecall.Text = ""
    '        txtConfCancel.Text = ""
    '        txtConfDord.Text = ""
    '        txtConfAllSplt.Text = ""
    '        chkConfManByOtherRole.Checked = False
    '        ddlConfRoleName.Items.Clear()
    '        chkCofAllowSkp.Checked = False
    '    End If

    '    updConfigStatus.Update()
    '    oda.Dispose()
    '    con.Close()
    'End Sub

    Protected Sub ddlConfigstatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlConfigstatus.SelectedIndexChanged
        txtConfAmendment.Text = ""
        txtConfRecall.Text = ""
        txtConfCancel.Text = ""
        txtConfDord.Text = ""
        ddlstatusNam.SelectedIndex = 0
        chkConfManByOtherRole.Checked = False
        ddlConfRoleName.Items.Clear()
        chkCofAllowSkp.Checked = False
        updConfigStatus.Update()
    End Sub
    Protected Sub editDocStatusHit(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim btndetail As ImageButton = TryCast(sender, ImageButton)
        'Dim row As GridViewRow = 
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActUserSave.Text = "Update"
        ViewState("tid") = pid


        If UCase(row.Cells(2).Text) = "ARCHIVE" Then
            lblRecord.Text = "This (Archive) is system generated field, can't Update"
            Exit Sub
        End If

        If row.Cells(2).Text = "UPLOADED" And Trim(row.Cells(1).Text) = "&nbsp;" Then
            lblRecord.Text = "This (Uploaded) is system generated field, can't Update"
            Exit Sub
        End If
        If row.Cells(2).Text = "UPLOADED" And Trim(row.Cells(1).Text) <> "" Then
            lblRecord.Text = "To edit this First Status, please click on Configure button at top right corner"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandText = "select statusname,tid From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and Documenttype='" & row.Cells(1).Text & "' and statusname not in('" & row.Cells(2).Text & "') and dord <='" & row.Cells(3).Text & "' "
        oda.SelectCommand.CommandText = "select statusname,tid From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and Documenttype='" & row.Cells(1).Text & "' and statusname not in('" & row.Cells(2).Text & "') "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        ddlCurstatus.DataSource = ds
        ddlCurstatus.DataTextField = "statusname"
        ddlCurstatus.DataValueField = "tid"
        ddlCurstatus.DataBind()
        ddlCurstatus.Items.Insert(0, "DEFAULT")
        ddlCurstatus.Items.Insert(1, "UPLOADED")
        '  ddlCurstatus.SelectedIndex = ddlRejStatus.Items.IndexOf(ddlRejStatus.Items.FindByText(row.Cells(8).Text))
        ddlNewStatus.DataSource = ds
        ddlNewStatus.DataTextField = "statusname"
        ddlNewStatus.DataValueField = "tid"
        ddlNewStatus.DataBind()
        ddlNewStatus.Items.Insert(0, "DEFAULT")
        ddlNewStatus.Items.Insert(1, "UPLOADED")
        oda.SelectCommand.CommandText = "select * from MMM_MST_WOrkflow_status where eid=" & Session("EID") & " and tid=" & ViewState("tid") & ""
        oda.Fill(ds, "status")
        ddlCurstatus.SelectedIndex = ddlCurstatus.Items.IndexOf(ddlCurstatus.Items.FindByText(ds.Tables("status").Rows(0).Item("curdocstatus").ToString))
        ddlNewStatus.SelectedIndex = ddlNewStatus.Items.IndexOf(ddlNewStatus.Items.FindByText(ds.Tables("status").Rows(0).Item("newdocstatus").ToString))
        txtCurDocType.Text = row.Cells(1).Text
        txtCurStatus.Text = row.Cells(2).Text
        con.Close()
        ds.Dispose()
        updCurStatus.Update()
        Me.MP_CurNewStatus.Show()
    End Sub
    Protected Sub btnCurNewStatus_Click(sender As Object, e As System.EventArgs) Handles btnCurNewStatus.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "' and tid=" & (ViewState("tid")) & " "
        'Dim ds As New DataSet()
        'oda.Fill(ds, "data")
        'If ds.Tables("data").Rows(0).Item("dord") < ViewState("dord") Then
        '    lblRejectStatus.Text = "Reject status not valid"
        '    Exit Sub
        'End If
        Dim od As New SqlDataAdapter("uspcurNewStatus", con)
        od.SelectCommand.CommandType = CommandType.StoredProcedure
        od.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("tid").ToString))
        od.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        If ddlCurstatus.SelectedItem.Text = "DEFAULT" Then
        Else
            od.SelectCommand.Parameters.AddWithValue("curDocstatus", ddlCurstatus.SelectedItem.Text)
        End If
        If ddlNewStatus.SelectedItem.Text = "DEFAULT" Then
        Else
            od.SelectCommand.Parameters.AddWithValue("newDocstatus", ddlNewStatus.SelectedItem.Text)
        End If

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        od.SelectCommand.ExecuteNonQuery()
        gridbind()
        lblRecord.Visible = True
        lblRecord.Text = " Workflow Current/New Doc Status Added successfully "
        Me.updatePanelEdit.Update()
        MP_CurNewStatus.Hide()
        con.Dispose()
        oda.Dispose()
        con.Close()
        od.Dispose()
    End Sub


End Class
