Imports System.Data
Imports System.Data.SqlClient

Partial Class Workflowstatus
    Inherits System.Web.UI.Page
    Dim dord As Integer

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
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='WORKFLOW_STATUS' order by DisName", con)
            Dim dtLoc As New DataTable
            da.Fill(dtLoc)
            For i As Integer = 0 To dtLoc.Rows.Count - 1
                ddlLocationName.Items.Add(dtLoc.Rows(i).Item(0))
                ddlLocationName.Items(i).Value = dtLoc.Rows(i).Item(1)
            Next

            bindGridLoc()
            da.Dispose()
            Dim oda As SqlDataAdapter = New SqlDataAdapter("select FormID,FormName from MMM_MST_FORMS where FormType='DOCUMENT' and EID=" & Session("eid") & " order by formname", con)
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet
            oda.Fill(ds, "data")
            ddlDoctype.DataSource = ds
            ddlDoctype.DataTextField = "FormName"
            ddlDoctype.DataValueField = "FormID"
            ddlDoctype.DataBind()
            ddlDoctype.Items.Insert(0, "Select")

            bindstatus()
            'txtConfigstatus.Enabled = False

            dtLoc.Dispose()
            con.Close()
        End If
    End Sub

    Public Sub bindGridLoc()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter(" select * from (select TID,EID,Dord,isauth,StatusName,Documenttype,Approve,Reject,Reconsider,RejectStatus,Amendment,Recall,Cancel,case IsRcallable when 'True' then 'Yes' else 'No' end as [IsRcallable],RoleName From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " union all select TID,EID,Dord, '0' [isauth] ,StatusName,Documenttype,'' [Approve],'' [Reject],'' [Reconsider],'' [RejectStatus],Amendment,Recall,Cancel,case IsRcallable when 'True' then 'Yes' else 'No' end as [IsRcallable],RoleName  from MMM_MST_WORKFLOW_STATUS_CONFIG  where EID=" & Session("EID") & " ) a  order by Documenttype,Dord", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub

    Public Sub bindstatus()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select FormID,FormName from MMM_MST_FORMS where FormType='DOCUMENT' and EID=" & Session("eid") & " order by formname", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet
        oda.Fill(ds, "data")
        ddlConfigstatus.Items.Clear()
        ddlConfigstatus.DataSource = ds
        ddlConfigstatus.DataTextField = "FormName"
        ddlConfigstatus.DataValueField = "FormID"
        ddlConfigstatus.DataBind()
        ddlConfigstatus.Items.Insert(0, "Select")
        updConfigStatus.Update()
        con.Close()
        oda.Dispose()
        ds.Dispose()
    End Sub

    Public Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        chkActive.Checked = False
        txtDord.Text = ""
        txtStatusName.Text = ""
        txtAppCap.Text = ""
        txtRejCap.Text = ""
        txtRecCap.Text = ""
        txtAmendment.Text = ""
        txtRecall.Text = ""
        txtCancel.Text = ""
        txtAllowSplt.Text = ""
        txtDocEdit.Text = ""
        chkManByOthrRole.Checked = False
        chkAllowSkp.Checked = False
        chkallowprint.Checked = False
        ddlRoleName.Items.Clear()
        ddlDoctype.SelectedIndex = ddlDoctype.Items.IndexOf(ddlDoctype.Items.FindByText("Select"))
        btnActUserSave.Text = "Save"
        '   lblMsgEdit.Text = "Display order should between 0-100 "
        lblMsgEdit.Text = ""
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Public Sub ConfigureStatus(ByVal sender As Object, ByVal e As System.EventArgs)
        txtConfDord.Text = ""
        txtConfAmendment.Text = ""
        txtConfRecall.Text = ""
        txtConfCancel.Text = ""
        lblConfigStatus.Text = ""
        txtConfAllSplt.Text = ""
        txtConfDocEdit.Text = ""
        chkAllowPrint_Conf.Checked = False
        bindstatus()
        chkConfManByOtherRole.Checked = False
        chkCofAllowSkp.Checked = False
        ddlConfRoleName.Items.Clear()
        MP_ConfigStatus.Show()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        chkActive.Checked = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        btnActUserSave.Text = "Update"
        ViewState("pid") = pid
        ddlDoctype.SelectedIndex = ddlDoctype.Items.IndexOf(ddlDoctype.Items.FindByText(row.Cells(1).Text))

        '' added by sunil for validation of default status on 26 sep 
        If UCase(row.Cells(2).Text) = "ARCHIVE" And Trim(row.Cells(1).Text) = "&nbsp;" Then
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

        If UCase(row.Cells(2).Text) = "ARCHIVE" And Trim(row.Cells(1).Text) <> "" Then
            lblRecord.Text = "To edit this First Status, please click on Configure button at top right corner"
            Exit Sub
        End If

        '' added by sunil for validation of default status on 26 sep  - ends 

        'lblMsgDelete.Text = "This (Archive) is system generated field, can't Update"
        ''btnActDelete.Text = "Cancel"
        'btnActDelete.Visible = False
        'Me.updatePanelDelete.Update()
        'Me.btnDelete_ModalPopupExtender.Show()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and tid=" & ViewState("pid").ToString() & "", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            If (ds.Tables("data").Rows(0).Item("isauth").ToString()) = 0 Then
                chkActive.Checked = False
            Else
                chkActive.Checked = True
            End If
            If (ds.Tables("data").Rows(0).Item("isAllowSkip").ToString()) = 0 Then
                chkAllowSkp.Checked = False
            Else
                chkAllowSkp.Checked = True
            End If

            If IsDBNull(ds.Tables("data").Rows(0).Item("ShowINsearch")) = False Then
                If ds.Tables("data").Rows(0).Item("ShowINsearch").ToString() = 1 Then
                    chkShowinSearch.Checked = True
                Else
                    chkShowinSearch.Checked = False
                End If
            Else
                chkShowinSearch.Checked = False
            End If

            'check condition with IsRecallable 
            If IsDBNull(ds.Tables("data").Rows(0).Item("IsRcallable")) Then
                chkIsRecallable.Checked = False
            Else
                If ds.Tables("data").Rows(0).Item("IsRcallable") = 0 Then
                    chkIsRecallable.Checked = False
                Else
                    chkIsRecallable.Checked = True
                End If
            End If
            'check condition with IsRecallable 



            If ds.Tables("data").Rows(0).Item("ManagebyotherRole") = 0 Then
                chkManByOthrRole.Checked = False
                ddlRoleName.Enabled = False

            Else
                chkManByOthrRole.Checked = True
                oda.SelectCommand.CommandText = "select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & ""
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "rolename")
                If ds.Tables("rolename").Rows.Count > 0 Then
                    ddlRoleName.DataSource = ds.Tables("rolename")
                    ddlRoleName.DataTextField = "RoleName"
                    ddlRoleName.DataValueField = "RoleID"
                    ddlRoleName.DataBind()
                    ddlRoleName.Items.Insert(0, "Please Select")
                    ddlRoleName.Enabled = True
                    ddlRoleName.SelectedIndex = ddlRoleName.Items.IndexOf(ddlRoleName.Items.FindByText(ds.Tables("data").Rows(0).Item("Rolename")))
                Else
                    lblManByother.Text = "There is No Role in Role Master"
                End If

            End If

            'If ds.Tables("data").Rows(0).Item("aprovaltype").ToString() = 0 Then
            '    chkapproval.Checked = False
            '    ddlDoctype.Enabled = False
            'Else
            '    chkapproval.Checked = True
            '    oda.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlDoctype.SelectedItem.Text & "' and datatype='Datetime'"
            '    oda.SelectCommand.CommandType = CommandType.Text
            '    oda.Fill(ds, "value")
            '    If ds.Tables("value").Rows.Count > 0 Then
            '        ddledate.DataSource = ds.Tables("value")
            '        ddledate.DataTextField = "displayname"
            '        ddledate.DataValueField = "fieldmapping"
            '        ddledate.DataBind()
            '        ddledate.Items.Insert(0, "Please Select")
            '        ddledate.Enabled = True
            '        ddledate.SelectedIndex = ddledate.Items.IndexOf(ddledate.Items.FindByValue(ds.Tables("data").Rows(0).Item("expirydate")))
            '    Else
            '        lblManByother.Text = "There is No expiry date in '" & ddlDoctype.SelectedItem.Text & "' "
            '    End If

            'End If

            txtStatusName.Text = ds.Tables("data").Rows(0).Item("statusname").ToString()
            txtDord.Text = ds.Tables("data").Rows(0).Item("dord").ToString()
            txtAmendment.Text = ds.Tables("data").Rows(0).Item("amendment").ToString()
            txtRecall.Text = ds.Tables("data").Rows(0).Item("recall").ToString()
            txtCancel.Text = ds.Tables("data").Rows(0).Item("cancel").ToString()
            txtAppCap.Text = ds.Tables("data").Rows(0).Item("Approve").ToString()
            txtRejCap.Text = ds.Tables("data").Rows(0).Item("Reject").ToString()
            txtRecCap.Text = ds.Tables("data").Rows(0).Item("Reconsider").ToString()
            txtAllowSplt.Text = ds.Tables("data").Rows(0).Item("split").ToString()
            txtDocEdit.Text = ds.Tables("data").Rows(0).Item("Edit").ToString()
            txtcopy.Text = ds.Tables("data").Rows(0).Item("copy").ToString()

            '   txtahours.Text = ds.Tables("data").Rows(0).Item("afterhours").ToString()
            Dim IsAllowonDashBoard As String = ""
            IsAllowonDashBoard = Convert.ToString(ds.Tables("data").Rows(0).Item("AllowOnDashboard"))
            If IsAllowonDashBoard = "1" Then
                chkIsAllowAction.Checked = True
            Else
                chkIsAllowAction.Checked = False
            End If
            updatePanelEdit.Update()
            'updtabpnlupdate.Update()
            oda.Dispose()
            con.Close()
            Me.btnEdit_ModalPopupExtender.Show()
            ' End If
        End If
    End Sub

    Protected Sub editRejHit(ByVal sender As Object, ByVal e As System.EventArgs)

        lblRejectStatus.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActUserSave.Text = "Update"
        ViewState("tid") = pid

        '' added by sunil for validation of default status on 26 sep 
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
        '' added by sunil for validation of default status on 26 sep  - ends 

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandText = "select statusname,tid From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and tid=" & ViewState("tid") & ""
        'Dim ds As New DataSet()
        'oda.Fill(ds, "statusname")

        oda.SelectCommand.CommandText = "select statusname,tid From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and Documenttype='" & row.Cells(1).Text & "' and statusname not in('" & row.Cells(2).Text & "') and dord <='" & row.Cells(3).Text & "' "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")

        ddlRejStatus.DataSource = ds
        ddlRejStatus.DataTextField = "statusname"
        ddlRejStatus.DataValueField = "tid"
        ddlRejStatus.DataBind()
        ddlRejStatus.Items.Insert(0, "DEFAULT")
        ddlRejStatus.Items.Insert(1, "UPLOADED")
        ddlRejStatus.SelectedIndex = ddlRejStatus.Items.IndexOf(ddlRejStatus.Items.FindByText(row.Cells(8).Text))

        'dord = Val(row.Cells(3).Text)
        txtRejDocumenttype.Text = row.Cells(1).Text
        txtRejStatusname.Text = row.Cells(2).Text

        updatePanel_rejectstatus.Update()
        Me.ModalPopupExtender_RejectStatus.Show()


    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteWorkflowStatus", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("Did").ToString))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
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

        '' added by sunil for validation of default status on 26 sep 
        If UCase(row.Cells(2).Text) = "ARCHIVE" And row.Cells(1).Text = "&nbsp;" Then
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
        '' added by sunil for validation of default status on 26 sep  - ends 

        If row.Cells(2).Text = "0" Or row.Cells(2).Text = "100" Then
            lblMsgDelete.Text = "This is SYSTEM generated field . You can't delete"
            btnActDelete.Visible = False
            Me.updatePanelDelete.Update()
            Me.btnDelete_ModalPopupExtender.Show()
            Exit Sub
        Else
            lblMsgDelete.Text = "Are you Sure Want to delete this Record? " & row.Cells(1).Text
            btnActDelete.Visible = True
            Me.updatePanelDelete.Update()
            Me.btnDelete_ModalPopupExtender.Show()
        End If

    End Sub

    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click
        If UCase(ddlDoctype.SelectedItem.Text) = "SELECT" Then
            lblMsgEdit.Text = "Select Document type"
            Exit Sub
        End If
        If txtStatusName.Text.Length < 2 Then
            lblMsgEdit.Text = "Status Name should be valid"
            Exit Sub
        End If
        If Not IsNumeric(txtDord.Text) Then
            lblMsgEdit.Text = "Display order should be integer only "
            Exit Sub
        End If
        Dim dord As Integer = Int32.Parse(txtDord.Text).ToString
        If dord < 1 Or dord > 99 Then
            lblMsgEdit.Text = "Display order must be > 0 and < 100"
            Exit Sub
        End If
        If chkManByOthrRole.Checked = True Then
            If ddlRoleName.SelectedItem.Text = "Please Select" Then
                lblMsgEdit.Text = "PLease Select Role Name"
                Exit Sub
            End If
        End If

        If chkapproval.Checked = True Then
            If ddledate.SelectedItem.Text = "Please Select" Then
                lblMsgEdit.Text = "Please Select Expiry Date"
                Exit Sub
            End If
        End If

        If txtAppCap.Text.Length > 15 Then
            lblMsgEdit.Text = "Approve caption must be less than 15 character"
            Exit Sub
        End If
        If txtRejCap.Text.Length > 15 Then
            lblMsgEdit.Text = "Rejection caption must be less than 15 character"
            Exit Sub
        End If
        If txtRecCap.Text.Length > 15 Then
            lblMsgEdit.Text = "Reconsider caption must be less than 15 character"
            Exit Sub
        End If

        If btnActUserSave.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertworkflowstatus", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@Doctype", ddlDoctype.SelectedItem.Text())
            oda.SelectCommand.Parameters.AddWithValue("workflowstatus", txtStatusName.Text)
            oda.SelectCommand.Parameters.AddWithValue("Dord", txtDord.Text)
            If chkActive.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("isauth", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isauth", 0)
            End If
            If Trim(txtAppCap.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("approve", Trim(txtAppCap.Text))
            End If
            If Trim(txtRejCap.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("Reject", Trim(txtRejCap.Text))
            End If
            If Trim(txtRecCap.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("Reconsider", Trim(txtRecCap.Text))
            End If
            If Trim(txtAmendment.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("amendent", Trim(txtAmendment.Text))
            End If
            If Trim(txtRecall.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("Recall", Trim(txtRecall.Text))
            End If
            If Trim(txtCancel.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("cancel", Trim(txtCancel.Text))
            End If
            If chkManByOthrRole.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 1)
                oda.SelectCommand.Parameters.AddWithValue("RoleName", ddlRoleName.SelectedItem.Text)
            Else
                oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 0)
            End If
            If chkAllowSkp.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("isallow", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isallow", 0)
            End If
            If chkallowprint.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("allowprint", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("allowprint", 0)
            End If
            If chkShowinSearch.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("ShowINsearch", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("ShowINsearch", 0)
            End If

            If Trim(txtAllowSplt.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("split", Trim(txtAllowSplt.Text))
            End If
            If Trim(txtDocEdit.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("docEdit", Trim(txtDocEdit.Text))
            End If
            If Trim(txtcopy.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("copy", Trim(txtcopy.Text))
            End If
            If chkapproval.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("@apprtype", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("@apprtype", 0)
            End If
            oda.SelectCommand.Parameters.AddWithValue("@expirydate", ddledate.SelectedValue)
            If Trim(txtahours.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("@afterhours", Trim(txtahours.Text))
            End If
            If chkIsAllowAction.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("@AllowOnDashboard", "1")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@AllowOnDashboard", "0")
            End If

            If chkIsRecallable.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("@IsRcallable", "True")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@IsRcallable", "False")
            End If
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            lblRecord.Visible = True
            lblRecord.Text = " Workflow Status Created successfully "
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()

            con.Close()
            oda.Dispose()
        ElseIf btnActUserSave.Text = "Update" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateWorkflowStatus", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("TID", ViewState("pid"))
            oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("workflowstatus", txtStatusName.Text)
            oda.SelectCommand.Parameters.AddWithValue("docType", ddlDoctype.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("Dord", txtDord.Text)
            If chkActive.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("isauth", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isauth", 0)
            End If
            If Trim(txtAppCap.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("approvecap", Trim(txtAppCap.Text))
            End If
            If Trim(txtRejCap.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("Rejectcap", Trim(txtRejCap.Text))
            End If
            If Trim(txtRecCap.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("Reconsidercap", Trim(txtRecCap.Text))
            End If
            If Trim(txtAmendment.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("amendment", Trim(txtAmendment.Text))
            End If
            If Trim(txtRecall.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("Recall", Trim(txtRecall.Text))
            End If
            If Trim(txtCancel.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("cancel", Trim(txtCancel.Text))
            End If
            If chkManByOthrRole.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 1)
                oda.SelectCommand.Parameters.AddWithValue("RoleName", ddlRoleName.SelectedItem.Text)
            Else
                oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 0)
            End If
            If chkAllowSkp.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("isallow", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isallow", 0)
            End If

            If chkallowprint.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("allowprint", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("allowprint", 0)
            End If
            If chkShowinSearch.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("ShowINsearch", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("ShowINsearch", 0)
            End If
            If Trim(txtAllowSplt.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("split", Trim(txtAllowSplt.Text))
            End If

            If Trim(txtDocEdit.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("docEdit", Trim(txtDocEdit.Text))
            End If
            If Trim(txtcopy.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("copy", Trim(txtcopy.Text))
            End If
            If chkapproval.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("@apprtype", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("@apprtype", 0)
            End If
            oda.SelectCommand.Parameters.AddWithValue("@expirydate", ddledate.SelectedValue)
            If Trim(txtahours.Text) <> "" Then
                oda.SelectCommand.Parameters.AddWithValue("@afterhours", Trim(txtahours.Text))
            End If
            If chkIsAllowAction.Checked = True Then
                oda.SelectCommand.Parameters.AddWithValue("@AllowOnDashboard", "1")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@AllowOnDashboard", "0")
            End If
            If chkIsRecallable.Checked Then
                oda.SelectCommand.Parameters.AddWithValue("@IsRcallable", "True")
            Else
                oda.SelectCommand.Parameters.AddWithValue("@IsRcallable", "false")
            End If
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            lblRecord.Visible = True
            lblRecord.Text = " Workflow Status Updated successfully"
            Me.btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda.Dispose()
        ElseIf btnActUserSave.Text = "Cancel" Then
            Me.btnEdit_ModalPopupExtender.Hide()
        End If
    End Sub

    Protected Sub Search(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_WORKFLOW_STATUS where " & ddlLocationName.SelectedItem.Value & " like '" & txtValue.Text & "%' and EID='" & Session("EID") & "' ", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        If gvData.Rows.Count = 0 Then
            lblRecord.Visible = True
            lblRecord.Text = "No Record Found"
        Else
            lblRecord.Visible = False
        End If
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub

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

    Protected Sub btnConfigStatusSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfigStatusSave.Click

        If ddlConfigstatus.SelectedItem.Text = "Select" Then
            lblConfigStatus.Text = "PLease select Document Type"
            updConfigStatus.Update()
            Exit Sub
        End If
        If ddlstatusNam.SelectedItem.Text = "Select" Then
            ddlstatusNam.Text = "PLease select Document Type"
            updConfigStatus.Update()
            Exit Sub
        End If
        If Not IsNumeric(txtConfDord.Text) Then
            lblConfigStatus.Text = "Display order should be numeric"
            updConfigStatus.Update()
            Exit Sub
        End If
        If Len(Trim(txtConfAmendment.Text)) > 0 And Len(Trim(txtConfAmendment.Text)) < 2 Then
            lblConfigStatus.Text = "Please enter minimum 2 characters in Amendment"
        End If
        If Len(Trim(txtConfRecall.Text)) > 0 And Len(Trim(txtConfRecall.Text)) < 2 Then
            lblConfigStatus.Text = "Please enter minimum 2 characters in Recall"
        End If
        If Len(Trim(txtConfCancel.Text)) > 0 And Len(Trim(txtConfCancel.Text)) < 2 Then
            lblConfigStatus.Text = "Please enter minimum 2 characters in Cancel"
        End If
        If chkConfManByOtherRole.Checked = True Then
            If ddlConfRoleName.SelectedItem.Text = "Please Select" Then
                lblConfigStatus.Text = "PLease Select Role Name"
                Exit Sub
            End If
        End If



        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("uspWorkflowStausConfig", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("doctype", ddlConfigstatus.SelectedItem.Text)
        oda.SelectCommand.Parameters.AddWithValue("statusName", ddlstatusNam.SelectedItem.Text)
        oda.SelectCommand.Parameters.AddWithValue("dispOrd", txtConfDord.Text)
        If Len(Trim(txtConfAmendment.Text)) > 0 Then
            oda.SelectCommand.Parameters.AddWithValue("amendment", txtConfAmendment.Text)
        End If
        If Len(Trim(txtConfRecall.Text)) > 0 Then
            oda.SelectCommand.Parameters.AddWithValue("Recall", txtConfRecall.Text)
        End If
        If Len(Trim(txtConfCancel.Text)) > 0 Then
            oda.SelectCommand.Parameters.AddWithValue("cancel", txtConfCancel.Text)
        End If
        If chkConfManByOtherRole.Checked = True Then
            oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 1)
            oda.SelectCommand.Parameters.AddWithValue("RoleName", ddlConfRoleName.SelectedItem.Text)
        Else
            oda.SelectCommand.Parameters.AddWithValue("ManageByOtherRole", 0)
        End If
        If chkCofAllowSkp.Checked = True Then
            oda.SelectCommand.Parameters.AddWithValue("isallow", 1)
        Else
            oda.SelectCommand.Parameters.AddWithValue("isallow", 0)
        End If
        If chkAllowPrint_Conf.Checked = True Then
            oda.SelectCommand.Parameters.AddWithValue("allowprint", 1)
        Else
            oda.SelectCommand.Parameters.AddWithValue("allowprint", 0)
        End If
        If chkIsAuth.Checked = True Then
            oda.SelectCommand.Parameters.AddWithValue("isAuth", 1)
        Else
            oda.SelectCommand.Parameters.AddWithValue("isAuth", 0)
        End If

        If Len(Trim(txtConfAllSplt.Text)) > 0 Then
            oda.SelectCommand.Parameters.AddWithValue("split", txtConfAllSplt.Text)
        End If
        If Len(Trim(txtConfDocEdit.Text)) > 0 Then
            oda.SelectCommand.Parameters.AddWithValue("docEdit", txtConfDocEdit.Text)
        End If




        'If chkConfSplt.Checked = True Then
        '    oda.SelectCommand.Parameters.AddWithValue("split", 1)
        'Else
        '    oda.SelectCommand.Parameters.AddWithValue("split", 0)
        'End If
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteScalar()
        bindGridLoc()
        lblRecord.Text = "Configure default/first status has been created"
        oda.Dispose()
        con.Close()
        MP_ConfigStatus.Hide()
    End Sub

    Protected Sub chkManByOthrRole_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkManByOthrRole.CheckedChanged
        If chkManByOthrRole.Checked = True Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As New SqlDataAdapter("select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & "", con)
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                ddlRoleName.DataSource = ds.Tables("data")
                ddlRoleName.DataTextField = "RoleName"
                ddlRoleName.DataValueField = "RoleID"
                ddlRoleName.DataBind()
                ddlRoleName.Items.Insert(0, "Please Select")
                ddlRoleName.Enabled = True
            Else
                lblManByother.Text = "There is No Role in Role Master"
            End If
            con.Close()
            oda.Dispose()
        Else
            ddlRoleName.Enabled = False

        End If
        updatePanelEdit.Update()

    End Sub

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

    Protected Sub ddlstatusNam_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlstatusNam.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("select * from MMM_MST_WORKFLOW_STATUS_CONFIG where EID=" & Session("EID") & " and Documenttype='" & ddlConfigstatus.SelectedItem.Text & "' and statusname='" & ddlstatusNam.SelectedItem.Text & "' ", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            txtConfAmendment.Text = ds.Tables("data").Rows(0).Item("Amendment").ToString()
            txtConfRecall.Text = ds.Tables("data").Rows(0).Item("Recall").ToString()
            txtConfCancel.Text = ds.Tables("data").Rows(0).Item("Cancel").ToString()
            txtConfDord.Text = ds.Tables("data").Rows(0).Item("Dord").ToString()
            chkConfManByOtherRole.Checked = ds.Tables("data").Rows(0).Item("managebyotherrole")
            txtConfAllSplt.Text = ds.Tables("data").Rows(0).Item("split").ToString()
            txtConfDocEdit.Text = ds.Tables("data").Rows(0).Item("Edit").ToString()
            If ds.Tables("data").Rows(0).Item("isallowskip") = 0 Then
                chkCofAllowSkp.Checked = False
            Else
                chkCofAllowSkp.Checked = True
            End If
            If ds.Tables("data").Rows(0).Item("isauth") = 0 Then
                chkIsAuth.Checked = False
            Else
                chkIsAuth.Checked = True
            End If
            If ds.Tables("data").Rows(0).Item("allowprint") = 0 Then
                chkAllowPrint_Conf.Checked = False
            Else
                chkAllowPrint_Conf.Checked = True
            End If

            If ds.Tables("data").Rows(0).Item("ManagebyotherRole") = 0 Then
                chkConfManByOtherRole.Checked = False
            Else
                chkConfManByOtherRole.Checked = True
                oda.SelectCommand.CommandText = "select RoleID,RoleName from MMM_MST_ROLE where EID=" & Session("EID") & ""
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "rolename")
                If ds.Tables("rolename").Rows.Count > 0 Then
                    ddlConfRoleName.DataSource = ds.Tables("rolename")
                    ddlConfRoleName.DataTextField = "RoleName"
                    ddlConfRoleName.DataValueField = "RoleID"
                    ddlConfRoleName.DataBind()
                    ddlConfRoleName.Items.Insert(0, "Please Select")
                    ddlConfRoleName.Enabled = True
                    ddlConfRoleName.SelectedIndex = ddlConfRoleName.Items.IndexOf(ddlConfRoleName.Items.FindByText(ds.Tables("data").Rows(0).Item("Rolename")))
                Else
                    lblManByother.Text = "There is No Role in Role Master"
                End If
            End If
        Else
            txtConfAmendment.Text = ""
            txtConfRecall.Text = ""
            txtConfCancel.Text = ""
            txtConfDord.Text = ""
            txtConfAllSplt.Text = ""
            chkConfManByOtherRole.Checked = False
            ddlConfRoleName.Items.Clear()
            chkCofAllowSkp.Checked = False
            chkIsAuth.Checked = False
        End If

        updConfigStatus.Update()
        oda.Dispose()
        con.Close()
    End Sub

    Protected Sub ddlConfigstatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlConfigstatus.SelectedIndexChanged
        txtConfAmendment.Text = ""
        txtConfRecall.Text = ""
        txtConfCancel.Text = ""
        txtConfDord.Text = ""
        ddlstatusNam.SelectedIndex = 0
        chkConfManByOtherRole.Checked = False
        ddlConfRoleName.Items.Clear()
        chkCofAllowSkp.Checked = False
        chkIsAuth.Checked = False
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


    'Protected Sub chkapproval_CheckedChanged(sender As Object, e As EventArgs) Handles chkapproval.CheckedChanged
    '    If chkapproval.Checked = True Then
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ViewState("adoctype") & "' and datatype='Datetime'", con)
    '        Dim ds As New DataSet
    '        da.Fill(ds, "data")
    '        If ds.Tables("data").Rows.Count > 0 Then
    '            ddledate.DataSource = ds.Tables("data")
    '            ddledate.DataTextField = "Displayname"
    '            ddledate.DataValueField = "Fieldmapping"
    '            ddledate.DataBind()
    '            ddledate.Items.Insert(0, "Please Select")
    '            ddledate.Enabled = True
    '        Else
    '            lblamsg.Text = "There is No expiry date in '" & ViewState("adoctype") & "' "
    '        End If
    '    Else
    '        ddledate.Enabled = False
    '    End If

    'End Sub



    ''auto approval code starts from here

    Protected Sub autohit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        btnsaveauto.Text = "Update"
        ViewState("pid") = pid

        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * From MMM_MST_WOrkflow_status where EID=" & Session("EID") & " and tid=" & pid & "", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        Try

            If ds.Tables("data").Rows.Count > 0 Then
                ddladoctype.Text = ds.Tables("data").Rows(0).Item("DocumentType").ToString().Trim()
                ViewState("adoctype") = ds.Tables("data").Rows(0).Item("DocumentType").ToString().Trim()

                'Dim chk As Integer = Val(ds.Tables("data").Rows(0).Item("isactive"))

                'If chk = 0 Then
                '    chkiact.Checked = False
                'Else
                '    chkiact.Checked = True
                'End If


                oda.SelectCommand.CommandText = "select distinct statusname,dord from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ds.Tables("data").Rows(0).Item("DocumentType").ToString().Trim() & "' order by dord"
                oda.Fill(ds, "status")
                If ds.Tables("status").Rows.Count > 0 Then
                    ddlaStatus.DataSource = ds.Tables("status")
                    ddlaStatus.DataTextField = "statusname"
                    ddlaStatus.DataValueField = "dord"
                    ddlaStatus.DataBind()
                    ddlaStatus.Items.Insert(0, "Select")
                End If

                If Not ds.Tables("data").Rows(0).Item("autostatus").ToString().Trim() Is Nothing Then
                    ddlaStatus.SelectedIndex = ddlaStatus.Items.IndexOf(ddlaStatus.Items.FindByText(ds.Tables("data").Rows(0).Item("autostatus")))
                End If
                If Not ds.Tables("data").Rows(0).Item("autoaction").ToString().Trim() Is Nothing Then
                    ddlaaction.SelectedIndex = ddlaaction.Items.IndexOf(ddlaaction.Items.FindByText(ds.Tables("data").Rows(0).Item("autoaction")))
                End If
                If Not ds.Tables("data").Rows(0).Item("autonextstatus").ToString().Trim() Is Nothing Then
                    ddlavalu.Items.Clear()
                    If ddlaaction.SelectedItem.Text = "Approve" Then
                        Dim da As New SqlDataAdapter("select distinct statusname,dord from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ViewState("adoctype") & "' and dord > '" & Convert.ToInt32(ddlaStatus.SelectedValue) & "' union select 'Archived','' ", con)
                        da.Fill(ds, "statuss")
                        If ds.Tables("statuss").Rows.Count > 0 Then
                            ddlavalu.DataSource = ds.Tables("statuss")
                            ddlavalu.DataTextField = "statusname"
                            ddlavalu.DataValueField = "dord"
                            ddlavalu.DataBind()
                            ddlavalu.Items.Insert(0, "Select")
                        End If
                    Else
                        ddlavalu.Items.Insert(0, "Select")
                        ddlavalu.Items.Insert(0, "Default")
                    End If
                    ddlavalu.SelectedIndex = ddlavalu.Items.IndexOf(ddlavalu.Items.FindByText(ds.Tables("data").Rows(0).Item("autonextstatus")))
                End If

                oda.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ViewState("adoctype") & "' and datatype='Datetime'"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "value")
                If ds.Tables("value").Rows.Count > 0 Then
                    ddledate.DataSource = ds.Tables("value")
                    ddledate.DataTextField = "displayname"
                    ddledate.DataValueField = "fieldmapping"
                    ddledate.DataBind()
                    ddledate.Items.Insert(0, "Select")
                    'ddledate.Enabled = True

                Else
                    ddledate.Items.Insert(0, "Select")
                    lblamsg.Text = "There is No expiry date in '" & ViewState("adoctype") & "' "
                End If




                If Not ds.Tables("data").Rows(0).Item("expirydate").ToString().Trim() Is Nothing Then
                    ddledate.SelectedIndex = ddledate.Items.IndexOf(ddledate.Items.FindByValue(ds.Tables("data").Rows(0).Item("expirydate")))
                End If
                If Not ds.Tables("data").Rows(0).Item("afterhours").ToString().Trim() Is Nothing Then
                    txtahours.Text = ds.Tables("data").Rows(0).Item("afterhours").ToString()
                End If
                oda.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ViewState("adoctype") & "' and datatype='Datetime'"

                oda.Fill(ds, "fld")
                If ds.Tables("fld").Rows.Count > 0 Then
                    ddledate.DataSource = ds.Tables("fld")
                    ddledate.DataTextField = "Displayname"
                    ddledate.DataValueField = "Fieldmapping"
                    ddledate.DataBind()
                    ddledate.Items.Insert(0, "Please Select")
                    ddledate.Enabled = True
                Else
                    lblamsg.Text = "There is No expiry date in '" & ViewState("adoctype") & "' "
                End If

                updapp.Update()

                oda.Dispose()
                con.Close()
                Me.modalpopupapp.Show()

            End If
        Catch ex As Exception
            lblamsg.Text = "Please try after some time!!!!"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try

    End Sub




    Protected Sub ddlaaction_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlaaction.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        ddlavalu.Items.Clear()
        If ddlaaction.SelectedItem.Text = "Approve" Then
            Dim da As New SqlDataAdapter("select distinct statusname,dord from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ViewState("adoctype") & "' and dord > '" & Convert.ToInt32(ddlaStatus.SelectedValue) & "' order by dord", con)
            Dim ds As New DataSet()
            da.Fill(ds, "status")
            If ds.Tables("status").Rows.Count > 0 Then
                ddlavalu.DataSource = ds.Tables("status")
                ddlavalu.DataTextField = "statusname"
                ddlavalu.DataValueField = "dord"
                ddlavalu.DataBind()
            End If
        Else
            ddlavalu.Items.Insert(0, "Default")
        End If
    End Sub

    Protected Sub btnsaveauto_Click(sender As Object, e As EventArgs) Handles btnsaveauto.Click

        Dim edate As String = ""
        Dim ahours As String = ""
        Dim astatus As String = ""
        Dim aaction As String = ""
        Dim nextstatus As String = ""

        If ddledate.SelectedValue = "" Then
            edate = ""
        Else
            edate = ddledate.SelectedValue
        End If
        If txtahours.Text.Trim = "" Then
            ahours = ""
        Else
            ahours = txtahours.Text.Trim()
        End If
        If ddlaStatus.SelectedItem.Text = "Select" Then
            astatus = ""
        Else
            astatus = ddlaStatus.SelectedItem.Text.Trim()
        End If
        If ddlaaction.SelectedItem.Text = "Select" Then
            aaction = ""
        Else
            aaction = ddlaaction.SelectedItem.Text
        End If
        If ddlavalu.SelectedItem.Text.Trim = "Select" Then
            nextstatus = ""
        Else
            nextstatus = ddlavalu.SelectedItem.Text.Trim()
        End If

        Dim chk As Integer = 0
        If chkiact.Checked = True Then
            chk = 1
        Else
            chk = 0
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("proc_updateautopprovalWS", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("pid"))

        oda.SelectCommand.Parameters.AddWithValue("@expirydate", edate.ToString())
        If Trim(txtahours.Text) <> "" Then
            oda.SelectCommand.Parameters.AddWithValue("@afterhours", ahours.ToString())
        End If

        oda.SelectCommand.Parameters.AddWithValue("@astatus", astatus.ToString())
        oda.SelectCommand.Parameters.AddWithValue("@aaction", aaction.ToString())
        oda.SelectCommand.Parameters.AddWithValue("@nextstatus", nextstatus.ToString())
        oda.SelectCommand.Parameters.AddWithValue("@isactive", Val(chk))


        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        oda.SelectCommand.CommandText = "select * From MMM_MST_WORKFLOW_STATUS where EID='" & Session("EID").ToString() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        lblRecord.Visible = True
        lblRecord.Text = "Auto Approval Setting Created Successfully!!! "
        Me.updapp.Update()
        Me.modalpopupapp.Hide()
        bindGridLoc()
        con.Close()
        oda.Dispose()

    End Sub
End Class
