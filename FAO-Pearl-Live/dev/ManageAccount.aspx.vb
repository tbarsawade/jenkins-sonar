Imports System.Data
Imports System.Data.SqlClient

Partial Class ManageAccount
    Inherits System.Web.UI.Page
    Private Const s As String = "ABCDEFGHIJKLMNOPQRST"
    Dim r As New Random
    Dim sb As New StringBuilder
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
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='MANAGE' order by DisName", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
        End If
    End Sub
    'Added By Komal on 17Feb2014
    Public Function APIString(size As Integer) As String
        ' Dim uid As String = Session("UID")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim abc As SqlDataAdapter = New SqlDataAdapter("select top 1 uid from mmm_mst_user where eid =" & Session("EID") & " and userrole='SU'", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim uid As String = abc.SelectCommand.ExecuteScalar()
        Dim uidchar As String = 6
        Dim k As Integer
        Dim builder As New StringBuilder()
        ' Dim ch As Char
        For i As Integer = 0 To 10
            Dim idx As Integer = r.Next(0, 19)
            sb.Append(s.Substring(idx, 1))
        Next
        k = uid.Length
        k = uidchar - k
        For l As Integer = 1 To k
            sb.Append("0")
        Next
        sb.Append(uid)
        'Dim len = sb.Length
        'sb.Append("32")
        'For i As Integer = 12 To 17
        '    Dim idx As Integer = r.Next(0, 19)
        '    sb.Append(s.Substring(idx, 1))
        'Next
        For i As Integer = 18 To 20
            Dim idx As Integer = r.Next(0, 19)
            sb.Append(s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function
    Protected Sub GenerateKey(ByVal sender As Object, ByVal e As System.EventArgs)
        APIString(20)
        If sb.Length > 0 Then
            lblAppKeyName.Visible = True
            lblApiKey.Visible = True
            lblApiKey.Text = sb.ToString
        End If
    End Sub
    Protected Sub AddTicket(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim objDC As New DataClass
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select * from mmm_mst_forms  with (nolock) where eid=" & pid & " and formname='Ticket'")
        ViewState("pid") = pid
        If objDT.Rows.Count > 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('You have already configured account for helpdesk');", True)
            lblMsgTicket.Text = ""
            btnSave.Visible = False
            Exit Sub
        End If
        btnSave.Visible = True

        Me.UpdatePanelTicket.Update()
        Me.btnTicket_ModalPopupExtender.Show()
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""

        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        ' No Value in Session just fill the Edit Form and Show two button
        btnActEdit.Text = "Update"

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT MMM_MST_ENTITY.eid,Code,Name,IPAddress,MMM_MST_USER.userid,MMM_MST_ENTITY.pwd [EPWD],Username,emailID,MMM_MST_USER.pwd,MMM_MST_ENTITY.APIKey FROM MMM_MST_ENTITY LEFT OUTER JOIN MMM_MST_USER On MMM_MST_ENTITY.EID = MMM_MST_USER.EID WHERE MMM_MST_ENTITY.EID=" & pid, con)
        Dim dt As New DataTable()
        oda.Fill(dt)

        oda.Dispose()
        con.Dispose()


        If dt.Rows.Count > 0 Then
            ViewState("pid") = pid
            txtAccCode.Text = dt.Rows(0).Item("code").ToString()
            txtAccName.Text = dt.Rows(0).Item("Name").ToString()
            txtServerIP.Text = dt.Rows(0).Item("IPAddress").ToString()
            txtServeruserID.Text = dt.Rows(0).Item("UserID").ToString()
            txtServerPWD.Text = dt.Rows(0).Item("EPWD").ToString()
            txtSUName.Text = dt.Rows(0).Item("Username").ToString()
            txtSUID.Text = dt.Rows(0).Item("emailid").ToString()
            txtSUPwd.Text = dt.Rows(0).Item("pwd").ToString()
            'Added By Komal on 18Feb2014
            If dt.Rows(0).Item("APIKey").ToString() = "" Then
                btnGenApiKey.Visible = True
            Else
                btnGenApiKey.Visible = False
                lblAppKeyName.Visible = True
                lblApiKey.Visible = True
                lblApiKey.Text = dt.Rows(0).Item("APIKey").ToString()
            End If
            'End
        End If
        'two methods.. either show data from Grid or Show data from Database.

        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        'validation for null entry

        If Trim(txtAccName.Text).Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid Account Name"
            Exit Sub
        End If

        If Trim(txtAccCode.Text).Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid Account Code"
            Exit Sub
        End If
        'Commnted By Komal on 18Feb2014
        'If Trim(txtServerIP.Text).Length < 5 Then
        '    lblMsgEdit.Text = "Please Enter Valid server IP Address"
        '    Exit Sub
        'End If

        'Edit Record
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateAccount", con)
        Dim pid As Integer = Val(ViewState("pid").ToString())
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("AcCode", txtAccCode.Text)
        oda.SelectCommand.Parameters.AddWithValue("AcName", txtAccName.Text)
        oda.SelectCommand.Parameters.AddWithValue("IPAddress", txtServerIP.Text)
        oda.SelectCommand.Parameters.AddWithValue("ServerUserID", txtServeruserID.Text)
        oda.SelectCommand.Parameters.AddWithValue("ServerPWD", txtServerPWD.Text)
        oda.SelectCommand.Parameters.AddWithValue("UserName", txtSUName.Text)
        oda.SelectCommand.Parameters.AddWithValue("emailID", txtSUID.Text)
        oda.SelectCommand.Parameters.AddWithValue("pwd", txtSUPwd.Text)
        oda.SelectCommand.Parameters.AddWithValue("eid", pid)
        oda.SelectCommand.Parameters.AddWithValue("APIKey", lblApiKey.Text)                      'Added By Komal on 18Feb2014
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Then
            txtAccCode.Text = ""
            txtAccName.Text = ""
            updatePanelEdit.Update()
            gvData.DataBind()
            btnEdit_ModalPopupExtender.Hide()
        Else
            lblMsgEdit.Text = "This Sales Depo Already Exist"
            updatePanelEdit.Update()
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataBind()
    End Sub

    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("ename") = pid

        If row.Cells(6).Text.ToUpper() = "ACTIVE" Then
            lblMsgLock.Text = "Are you Sure Want to <b>BLOCK</b> ::: " & row.Cells(2).Text
        Else
            lblMsgLock.Text = "Are you Sure Want to <b>ACTIVATE</b> ::: " & row.Cells(2).Text
        End If
        Me.updatePanelLock.Update()
        Me.btnlock_ModalPopupExtender.Show()
    End Sub

    Protected Sub LockUser(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pid As Integer = Val(ViewState("ename").ToString())
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("usplockUnlockEntity", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", pid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        gvData.DataBind()
        btnlock_ModalPopupExtender.Hide()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim objDC As New DataClass
        Dim ht As New Hashtable
        If ValidateHelpDesk() Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim tran As SqlTransaction = Nothing
            Dim con As SqlConnection = New SqlConnection(conStr)
            Try
                con.Open()
                tran = con.BeginTransaction()
                ht.Add("@EID", ViewState("pid"))
                ht.Add("@mdmailid", txtUserEmailID.Text)
                ht.Add("@mdpwd", txtPassword.Text)
                ht.Add("@mdport", txtPortNumber.Text)
                ht.Add("@mdisssl", chkSSL.Checked)
                ht.Add("@hostname", txtHostName.Text)
                ht.Add("@DocumentType", "Ticket")
                ht.Add("@IsAllowCreateUser", chkIsAllowCreateUser.Checked)
                ht.Add("@IsActive", chkIsActive.Checked)
                ht.Add("@RolematrixfromOrganization", chkRoleMatrix.Checked)
                ht.Add("@USERROLE", "USER")
                ht.Add("@AGENTROLE", "AGENT")
                ht.Add("@ADMINROLE", "ADMIN")
                objDC.TranExecuteProDT("sp_insert_mmm_hdmail_schdule", ht, con, tran)

                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                ht.Add("@SourceFormName", "Organizations")
                objDC.TranExecuteProDT("InsertSpecificFormWithFields", ht, con, tran)
                UpdateFieldsBasedOnDocument(objDC.TranExecuteQryDT("select Eid,FormName from MMM_Mst_Forms Where Eid=112 and formname='Organizations'", con, tran), ViewState("pid"), con, tran)
                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                ht.Add("@SourceFormName", "Attachments")
                objDC.TranExecuteProDT("InsertSpecificFormWithFields", ht, con, tran)
                UpdateFieldsBasedOnDocument(objDC.TranExecuteQryDT("select Eid,FormName from MMM_Mst_Forms Where Eid=112 and formname='Attachments'", con, tran), ViewState("pid"), con, tran)

                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                ht.Add("@SourceFormName", "Groups")
                objDC.TranExecuteProDT("InsertSpecificFormWithFields", ht, con, tran)
                UpdateFieldsBasedOnDocument(objDC.TranExecuteQryDT("select Eid,FormName from MMM_Mst_Forms Where Eid=112 and formname='Groups'", con, tran), ViewState("pid"), con, tran)
                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                ht.Add("@SourceFormName", "USER")
                objDC.TranExecuteProDT("InsertSpecificFormWithFields", ht, con, tran)
                UpdateFieldsBasedOnDocument(objDC.TranExecuteQryDT("select Eid,FormName from MMM_Mst_Forms Where Eid=112 and formname='USER'", con, tran), ViewState("pid"), con, tran)
                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                ht.Add("@SourceFormName", "Department")
                objDC.TranExecuteProDT("InsertSpecificFormWithFields", ht, con, tran)
                UpdateFieldsBasedOnDocument(objDC.TranExecuteQryDT("select Eid,FormName from MMM_Mst_Forms Where Eid=112 and formname='Department'", con, tran), ViewState("pid"), con, tran)
                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                ht.Add("@SourceFormName", "Ticket")
                objDC.TranExecuteProDT("InsertSpecificFormWithFields", ht, con, tran)
                UpdateFieldsBasedOnDocument(objDC.TranExecuteQryDT("select Eid,FormName from MMM_Mst_Forms Where Eid=112 and formname='Ticket'", con, tran), ViewState("pid"), con, tran)

                ' Update triggers
                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                objDC.TranExecuteProDT("InsertTriggers", ht, con, tran)
                objDC.TranExecuteQryDT("Update MMM_MST_Triggers set TriggerText=replace(TriggerText,'=112','=" & ViewState("pid") & "') Where eid='" & ViewState("pid") & "' ", con, tran)

                'Update Template
                ht.Clear()
                ht.Add("@Eidtarget", ViewState("pid"))
                ht.Add("@Eidsource", "112")
                objDC.TranExecuteProDT("InsertTEMPLATE", ht, con, tran)

                'Updated Master data & User Ceration
                Dim IsExistUser As Integer = 0
                IsExistUser = objDC.TranExecuteQryScaller("select count(*) from mmm_mst_user where emailid in ('mayank.garg@myndsol.com','sunil.pareek@myndsol.com') and eid=" & ViewState("pid") & "", con:=con, tran:=tran)
                If IsExistUser = 0 Then
                    objDC.TranExecuteQryDT("insert into mmm_mst_user (username,emailid,pwd,userrole,isauth,eid,passtry,modifydate,locationid,skey,userid,lastupdate) values('AGENT HELPDESK','mayank.garg@myndsol.com','gQykWZlVfWLFGZozFB2TYg==','AGENT',1," & ViewState("pid") & ",0,getdate(),2072,0,'Agent Support',getdate()),('ADMIN HELPDESK','sunil.pareek@myndsol.com','gQykWZlVfWLFGZozFB2TYg==','ADMIN',1," & ViewState("pid") & ",0,getdate(),2072,0,'Admin Support',getdate())", con, tran)
                    'Insert Group Master
                    objDC.TranExecuteQryDT("declare @UID int,@AUID int select @UID=uid from mmm_mst_user where eid=" & ViewState("pid") & " and userrole='Admin' select @AUID=uid from mmm_mst_user where eid=" & ViewState("pid") & " and userrole='AGENT' insert into mmm_mst_master (eid,documenttype,createdby,updateddate,fld1,fld2,isauth,source,lastupdate,adate,tallyisactive) values(" & ViewState("pid") & ",'Groups',@UID,getdate(),'GmailSupport',@UID,1,'Web',getdate(),getdate(),0), (" & ViewState("pid") & ",'Groups',@UID,getdate(),'YahooSupport',@AUID,1,'Web',getdate(),getdate(),0)", con, tran)
                    'Insert Organizations Master
                    objDC.TranExecuteQryDT("declare @UID int,@AUID int ,@GmailSupport nvarchar(200),@YahooSupport nvarchar(200) select @UID=uid from mmm_mst_user where eid=" & ViewState("pid") & " and userrole='Admin' select @AUID=uid from mmm_mst_user where eid=" & ViewState("pid") & " and userrole='AGENT' select @GmailSupport= tid from mmm_mst_master where documenttype='GRoups' and eid=" & ViewState("pid") & " order by tid select @YahooSupport= tid from mmm_mst_master where documenttype='GRoups' and eid=" & ViewState("pid") & " order by tid desc insert into mmm_mst_master (eid,documenttype,createdby,updateddate,fld1,fld2,fld3,fld4,isauth,source,lastupdate,adate,tallyisactive) values(" & ViewState("pid") & ",'Organizations',@UID,getdate(),'GmailORG','myndsol.com,gmail.com',@GmailSupport,@UID,1,'Web',getdate(),getdate(),0), (" & ViewState("pid") & ",'Organizations',@UID,getdate(),'YahooORG','yahoo.com',@YahooSupport,@AUID,1,'Web',getdate(),getdate(),0)", con, tran)

                    'Insert for Report 
                    ht.Clear()
                    ht.Add("@Eidtarget", ViewState("pid"))
                    ht.Add("@Eidsource", "112")
                    objDC.TranExecuteProDT("InsertREPORT", ht, con, tran)
                    Dim dt As New DataTable
                    dt = objDC.TranExecuteQryDT("select * from mmm_mst_report where eid=" & ViewState("pid"), con, tran)
                    For i As Integer = 0 To dt.Rows.Count - 1
                        If Convert.ToString(dt.Rows(i)("TicketUser")) <> String.Empty Then
                            If IsNumeric(dt.Rows(i)("TicketUser")) Then
                                Dim dttemp As New DataTable
                                dttemp = objDC.TranExecuteQryDT("select  uid from mmm_mst_user where userrole in (select userrole from mmm_mst_user where uid=" & dt.Rows(i)("TicketUser") & ") and eid=" & ViewState("pid") & "", con, tran)
                                If dttemp.Rows.Count > 0 Then
                                    objDC.TranExecuteQryDT("update mmm_mst_report set qryField= Replace(qryField,'userid=''" & dt.Rows(i)("TicketUser") & "''','userid=''" & dttemp.Rows(0)(0) & "'''),TicketUser='" & dttemp.Rows(0)(0) & "'  where reportid= " & dt.Rows(i)("Reportid") & " and eid= " & ViewState("pid"), con, tran)
                                End If

                            End If
                        End If
                    Next
                    'Insert for Role
                    ht.Clear()
                    ht.Add("@Eidtarget", ViewState("pid"))
                    ht.Add("@Eidsource", "112")
                    objDC.TranExecuteProDT("InsertRole", ht, con, tran)

                    'Insert for Role

                    'Insert for menu
                    'ht.Clear()
                    'ht.Add("@Eidtarget", ViewState("pid"))
                    'ht.Add("@Eidsource", "112")
                    'objDC.TranExecuteProDT("InsertMenu", ht, con, tran)

                    'Dim mv As New DataTable
                    'mv = objDC.TranExecuteQryDT("select * from mmm_mst_menu Where Eid='" & ViewState("pid") & "'", con, tran)
                    'Dim val As String = ""
                    'Dim flagg As Boolean = False
                    'For j As Integer = 0 To mv.Rows.Count - 1
                    '    Dim inr As New DataTable
                    '    inr = objDC.TranExecuteQryDT("select mid from mmm_mst_menu Where Eid='" & ViewState("pid") & "' and oldmid='" & mv.Rows(j).Item("oldmid").ToString() & "'", con, tran)
                    '    If inr.Rows.Count > 0 Then
                    '        Val = inr.Rows(0).Item("mid").ToString()
                    '    End If
                    '    objDC.TranExecuteQryDT("Update mmm_mst_menu set Pmenu='" & val & "'  where eid='" & ViewState("pid") & "' and pmenu='" & mv.Rows(j).Item("oldmid").ToString() & "'", con, tran)
                    'Next


                    objDC.TranExecuteQryDT("insert into mmm_mst_menu (Eid,MenuName,Pmenu,PageLink,Dord,Image,Roles,IsMobile,MTYPE,lastupdate,iscalendar) values(" & ViewState("pid") & ",'New',0,'MENU',6,'','{SU:15}',0,null,getdate(),0) declare @Pmenu int select @Pmenu= mid from mmm_mst_menu where eid=" & ViewState("pid") & " and menuname='New' insert into mmm_mst_menu (Eid,MenuName,Pmenu,PageLink,Dord,Image,Roles,IsMobile,MTYPE,lastupdate,iscalendar) values(" & ViewState("pid") & ",'Ticket',@Pmenu,'Documents.Aspx?SC=Ticket',0,'','{SU:15}',1,'DYNAMIC',getdate(),0) select @Pmenu= mid from mmm_mst_menu where eid=" & ViewState("pid") & " and menuname='Master' insert into mmm_mst_menu (Eid,MenuName,Pmenu,PageLink,Dord,Image,Roles,IsMobile,MTYPE,lastupdate,iscalendar) values(" & ViewState("pid") & ",'Department',@Pmenu,'Masters.ASPX?SC=Department',3,'','{SU:15}',1,'DYNAMIC',getdate(),0), (" & ViewState("pid") & ",'Groups',@Pmenu,'Masters.ASPX?SC=Groups',4,'','{SU:15}',1,'DYNAMIC',getdate(),0),(" & ViewState("pid") & ",'Organizations',@Pmenu,'Masters.ASPX?SC=Organizations',5,'','{SU:15}',1,'DYNAMIC',getdate(),0)", con, tran)
                    Dim dttempdata As New DataTable
                    dttempdata = objDC.TranExecuteQryDT("select * from mmm_mst_menu where eid=112 and Roles like '%,%'", con, tran)
                    For Each dr As DataRow In dttempdata.Rows
                        objDC.TranExecuteQryDT(" if exists(select count(*) from mmm_mst_menu where eid=" & ViewState("pid") & " and MenuName='" & dr("MenuName") & "') update mmm_mst_menu set roles=isnull(roles,'')+','+'" & dr("Roles") & "' where eid=" & ViewState("pid") & " and MenuName='" & dr("MenuName") & "' else update mmm_mst_menu set roles='" & dr("Roles") & "' where eid=" & ViewState("pid") & " and MenuName='" & dr("MenuName") & "' ", con, tran)
                    Next
                    'Insert for menu
                    'Set Default login Page
                    'objDC.TranExecuteQryDT("update mmm_mst_entity set defaultpage='thome.aspx',ctheme='Orange' where eid=" & ViewState("pid"), con, tran)
                    'Set Default login Page
                    tran.Commit()
                    ShowMessage("Ticket account has been successfully activated.")
                Else
                    tran.Rollback()
                    ShowMessage(" Default EmailIDs already configured with this account (mayank.garg@myndsol.com and sunil.pareek@myndsol.com)")
                End If

                Me.btnTicket_ModalPopupExtender.Hide()
            Catch ex As Exception
                If Not tran Is Nothing Then
                    tran.Rollback()
                    ShowMessage(" There were some error in details")
                End If
            Finally
                con.Close()
            End Try
        End If

    End Sub
    Protected Sub ShowMessage(ByVal msg As String)
        ScriptManager.RegisterStartupScript(Page, Page.GetType, "MyScript", "alert('" & msg.ToString() & "');", True)
    End Sub

    Public Function UpdateFieldsBasedOnDocument(ByVal dt As DataTable, ByVal res As String, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim result As String = ""
        Try
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim dt11 As New DataTable
                Dim docname As String = dt.Rows(i).Item("FormName").ToString().Trim()
                Dim objDC As New DataClass
                dt11 = objDC.TranExecuteQryDT("Select * from MMM_Mst_Fields Where Eid='" & res & "' and Documenttype='" & dt.Rows(i).Item("FormName").ToString().Trim() & "'", con, tran)
                For j As Integer = 0 To dt11.Rows.Count - 1
                    Dim documenttype As String = dt11.Rows(j).Item("Documenttype")
                    Dim oldfieldid As String = IIf(Not IsDBNull(dt11.Rows(j).Item("Oldtid")), dt11.Rows(j).Item("Oldtid"), "")
                    Dim newfieldid As String = dt11.Rows(j).Item("Fieldid")
                    Dim dt1 As New DataTable
                    dt1 = objDC.TranExecuteQryDT("Select * from MMM_Mst_Fields Where Eid='" & res & "' and Documenttype='" & dt.Rows(i).Item("FormName").ToString().Trim() & "'  and oldtid <> '" & dt11.Rows(j).Item("Oldtid").ToString().Trim() & "'", con, tran)
                    For k As Integer = 0 To dt1.Rows.Count - 1
                        Dim changelukupvalue As String = String.Empty
                        Dim changelukupvaluenew As String = String.Empty
                        Dim changeddllukupvalue As String = String.Empty
                        Dim changedropdownvalue As String = String.Empty
                        Dim changekclogicvalue As String = String.Empty
                        Dim changedependentONvalue As String = String.Empty
                        Dim changeinitialFiltervalue As String = String.Empty
                        Dim lookupvalue As String = dt1.Rows(k).Item("lookupvalue").ToString().Trim()
                        Dim ddllookupvalue As String = dt1.Rows(k).Item("ddllookupvalue").ToString().Trim()
                        Dim dropdown As String = dt1.Rows(k).Item("dropdown").ToString().Trim()
                        Dim KC_LOGIC As String = dt1.Rows(k).Item("KC_LOGIC").ToString().Trim()

                        Dim KC_VALUE As String = dt1.Rows(k).Item("KC_VALUE").ToString().Trim()

                        Dim dependentON As String = dt1.Rows(k).Item("dependentON").ToString().Trim()
                        Dim initialFilter As String = dt1.Rows(k).Item("initialFilter").ToString().Trim()
                        Dim cal_fields As String = dt1.Rows(k).Item("cal_fields").ToString().Trim()
                        Dim flag As Boolean = False


                        '''''''''''''''''''''''' For LukUp'''''''''''''''''''''''
                        If (lookupvalue <> "" And lookupvalue.Length > 1) Then
                            lookupvalue = lookupvalue.Substring(0, lookupvalue.Length - 1)
                            Dim splitlukup As String() = Split(lookupvalue, ",")
                            Dim modifylukupvalue As String = ""
                            Dim modifylukupvaluenew As String = ""
                            For l As Integer = 0 To splitlukup.Length - 1
                                Dim A As String() = Split(splitlukup(l), "-")
                                If A.Length > 1 Then
                                    Dim mainvalue As String = A(0)
                                    If mainvalue.Trim() = oldfieldid.Trim() Then
                                        modifylukupvalue &= newfieldid.Trim() & "-" & A(1) & ","
                                        flag = True
                                    Else
                                        modifylukupvalue &= mainvalue.Trim() & "-" & A(1) & ","
                                    End If
                                End If
                            Next
                            changelukupvalue = modifylukupvalue.Trim()
                            changelukupvaluenew = modifylukupvaluenew.Trim()
                        End If
                        '''''''''''''''''''''''' For DDlLukUp'''''''''''''''''''''''''''
                        If (ddllookupvalue <> "" And ddllookupvalue.Length > 1) Then
                            Dim modifyddllukupvalue As String = ""
                            ddllookupvalue = ddllookupvalue.Substring(0, ddllookupvalue.Length - 1)
                            Dim splitddllukup As String() = Split(ddllookupvalue, ",")
                            For n As Integer = 0 To splitddllukup.Length - 1
                                Dim B As String() = Split(splitddllukup(n), "-")
                                If B.Length > 1 Then
                                    Dim mainvalueddllukup As String = B(0)
                                    If mainvalueddllukup.Trim() = oldfieldid.Trim() Then
                                        modifyddllukupvalue &= newfieldid.Trim() & "-" & B(1) & ","
                                        flag = True
                                    Else
                                        modifyddllukupvalue &= mainvalueddllukup.Trim() & "-" & B(1) & ","
                                    End If
                                End If
                            Next
                            changeddllukupvalue = modifyddllukupvalue.Trim()
                        End If
                        ''''''''''''''''''''' For DropDown''''''''''''''''''''''''''''''''''''
                        If (dropdown <> "" And dropdown.Length > 1) Then
                            If dropdown.Trim() = oldfieldid.Trim() Then
                                changedropdownvalue = newfieldid.Trim()
                                flag = True
                            Else
                                changedropdownvalue = dropdown.Trim()
                            End If
                        End If
                        ''''''''''''''''''''' For KC_LOgic''''''''''''''''''''''''''''''''''''
                        If (KC_LOGIC <> "" And KC_LOGIC.Length > 1) Then
                            Dim modifykclogicvalue As String = ""
                            Dim splitkclogic As String() = Split(KC_LOGIC, "-")
                            For p As Integer = 0 To splitkclogic.Length - 1
                                If splitkclogic.Length > 1 Then
                                    Dim mainkclogicvalue As String = splitkclogic(p)
                                    If mainkclogicvalue.Trim() = oldfieldid.Trim() Then
                                        modifykclogicvalue &= newfieldid.Trim() & "-"
                                        flag = True
                                    Else
                                        modifykclogicvalue &= mainkclogicvalue.Trim() & "-"
                                    End If
                                    changekclogicvalue = modifykclogicvalue.Trim()
                                End If
                            Next
                            If changekclogicvalue <> "" Then
                                changekclogicvalue = changekclogicvalue.Substring(0, changekclogicvalue.Length - 1)
                            End If
                        End If
                        ''''''''''''''''''''' For dependentON''''''''''''''''''''''''''''''''''''
                        If (dependentON <> "" And dependentON.Length > 1) Then
                            dependentON = dependentON.Substring(0, dependentON.Length - 1)
                            Dim modifydependentONvalue As String = ""
                            Dim splitdependentON As String() = Split(dependentON, ",")
                            For r As Integer = 0 To splitdependentON.Length - 1
                                If splitdependentON(r).Length > 0 Then
                                    Dim maindependentONvalue As String = splitdependentON(r)
                                    If maindependentONvalue.Trim() = oldfieldid.Trim() Then
                                        modifydependentONvalue &= newfieldid.Trim() & ","
                                        flag = True
                                    Else
                                        modifydependentONvalue &= maindependentONvalue.Trim() & ","
                                    End If
                                End If
                            Next
                            changedependentONvalue = modifydependentONvalue.Trim()
                        End If
                        ''''''''''''''''''''' For initialFilter''''''''''''''''''''''''''''''''''''
                        If (initialFilter <> "" And initialFilter.Length > 1) Then
                            initialFilter = initialFilter.Substring(0, initialFilter.Length - 1)
                            Dim modifyinitialFiltervalue As String = ""
                            Dim splitinitialFilter As String() = Split(initialFilter, ":")
                            For r As Integer = 0 To splitinitialFilter.Length - 1
                                If splitinitialFilter.Length > 1 Then
                                    Dim maininitialFiltervalue As String = splitinitialFilter(r)
                                    If maininitialFiltervalue.Trim() = oldfieldid.Trim() Then
                                        modifyinitialFiltervalue &= newfieldid.Trim() & ":"
                                        flag = True
                                    Else
                                        modifyinitialFiltervalue &= maininitialFiltervalue.Trim() & ":"
                                    End If
                                    changeinitialFiltervalue = modifyinitialFiltervalue.Trim()
                                End If
                            Next
                            If changeinitialFiltervalue <> "" Then
                                changeinitialFiltervalue = changeinitialFiltervalue.Substring(0, changeinitialFiltervalue.Length - 1)
                            End If

                        End If

                        ''''''''''''''''''''''''''Update Query'''''''''''''''''''''''''''''''''' 
                        If flag = True Then
                            objDC.TranExecuteQryDT("Update MMM_Mst_Fields set lookupvalue='" & changelukupvalue & "',ddllookupvalue='" & changeddllukupvalue & "',dropdown='" & changedropdownvalue & "',KC_LOGIC='" & changekclogicvalue & "',dependentON='" & changedependentONvalue & "',initialFilter='" & changeinitialFiltervalue & "'  where eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'", con, tran)
                        End If
                        If (cal_fields <> "") Then
                            objDC.TranExecuteQryDT("update MMM_Mst_Fields set cal_fields=replace(cal_fields,'fld" & oldfieldid & "'')','fld" & newfieldid & "'')') where eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'", con, tran)
                        End If
                        ' End If
                    Next
                    dt1.Dispose()
                Next
            Next
        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
        End Try
        Return result
    End Function
    Public Function ValidateHelpDesk() As Boolean
        Dim Result As Boolean = True
        Dim sb As New StringBuilder
        If txtUserEmailID.Text = String.Empty Then

            sb.Append("Please Fill User Email ID " & Environment.NewLine)
        End If
        If txtPassword.Text = String.Empty Then

            sb.Append("Please Fill User Email Password " & Environment.NewLine)
        End If
        If txtPortNumber.Text = String.Empty Then

            sb.Append("Please Fill User Email Port Number " & Environment.NewLine)
        End If
        If txtHostName.Text = String.Empty Then

            sb.Append("Please Fill User Email Host Name " & Environment.NewLine)
        End If
        If chkSSL.Checked = False Then

            sb.Append("Please check SSL " & Environment.NewLine)
        End If
        If sb.Length > 1 Then
            Result = False
            lblMsgTicket.Text = sb.ToString()
        Else
            lblMsgTicket.Text = String.Empty
        End If
        Return Result
    End Function
End Class
