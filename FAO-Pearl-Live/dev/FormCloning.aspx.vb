Imports System
Imports System.Data
Imports System.Data.SqlClient

Partial Class FormCloning
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim scrptMngr = DirectCast(Master.FindControl("ScriptManager1"), ScriptManager)
        'scrptMngr.AsyncPostBackTimeout = "3000"

        If Not IsPostBack Then
            reset()
        End If
        ' reset()
    End Sub

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

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim tran As SqlTransaction = Nothing
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try

            If ddlFormType.SelectedItem.Text.ToUpper = "SELECT" Then
                lblMsg.Text = "Please Form Type first!"
                Exit Sub
            End If
            If ddlOldForm.SelectedItem.Text.ToUpper = "SELECT" Then
                lblMsg.Text = "Please select Form Name first!"
                Exit Sub
            End If
            If Trim(txtNewForm.Text) = "" Then
                lblMsg.Text = "Please Type Form Name!"
                Exit Sub
            End If
            If (ddlFormType.SelectedValue = "Document" And Trim(txtNewFormSuffix.Text) = "") Then
                lblMsg.Text = "Please specify a suffix for Action/Child forms."
                Exit Sub
            End If


            Dim EID As String = ""
            If Not IsNothing(Session("EID")) Then
                EID = Session("EID")
            End If


            ' con.Open()
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Dim dtChk As New DataTable
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            tran = con.BeginTransaction()
            'Image1.Visible = True
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.CommandText = "select * from MMM_MST_forms with (nolock) Where formname='" & txtNewForm.Text & "' and eid=" & EID
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.Transaction = tran
            oda.SelectCommand.Parameters.Clear()
            oda.Fill(dtChk)
            If dtChk.Rows.Count > 0 Then
                lblMsg.Text = "Form Name already exists in Entity!"
                Exit Sub
            End If
            Dim OldForm As String = ddlOldForm.SelectedValue.ToString()
            Dim newform As String = txtNewForm.Text

            'For Form (document/Master) start (Pallavi)
            CloneForm(EID, ddlOldForm.SelectedValue.ToString(), txtNewForm.Text, con, tran, "", OldForm)
            'For Form (document/Master) end

            'For child items in Document Start (pallavi)
            Dim dtchild As New DataTable
            oda.SelectCommand.CommandText = "select * from mmm_mst_fields with (nolock) where eid =" & EID & " and FieldType ='child item'  and Documenttype ='" & newform & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.Transaction = tran
            oda.SelectCommand.Parameters.Clear()
            oda.Fill(dtchild)

            For i As Integer = 0 To dtchild.Rows.Count - 1
                Dim old_chld_docNm = dtchild.Rows(i).Item("dropdown").ToString()
                Dim New_clld_docNm = old_chld_docNm & "_" & txtNewFormSuffix.Text
                oda.SelectCommand.CommandText = "Update mmm_mst_fields set DropDown='" & New_clld_docNm & "'  where eid=" & EID & " and FieldID=" & dtchild.Rows(i).Item("FieldID").ToString() & " and DocumentType = '" & newform & "'"
                oda.SelectCommand.Transaction = tran
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                CloneForm(EID, old_chld_docNm, old_chld_docNm & "_" & txtNewFormSuffix.Text, con, tran)
                Change_Kc_LogicChildInMain(EID, New_clld_docNm, newform, tran, con)
            Next
            dtchild.Dispose()
            'For child items in Document end (pallavi)
            Dim dtAction As New DataTable
            oda.SelectCommand.CommandText = "Select * from mmm_mst_forms with (nolock) where eid =" & EID & " and EventName ='" & OldForm & "'  and FormSource='ACTION DRIVEN'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.Transaction = tran
            oda.SelectCommand.Parameters.Clear()
            oda.Fill(dtAction)

            For i As Integer = 0 To dtAction.Rows.Count - 1
                Dim old_Action_docNm = dtAction.Rows(i).Item("FormName").ToString()
                Dim New_Action_docNm = old_Action_docNm & "_" & txtNewFormSuffix.Text
                CloneForm(EID, old_Action_docNm, New_Action_docNm, con, tran, newform, OldForm)

            Next
            tran.Commit()
            reset()
            lblmsgg.Text = "New Form created through Cloning Successfully!"






        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
            If Not con Is Nothing Then
                con.Close()
            End If
            lblMsg.Text = "Error found - " & ex.Message.ToString()
            ' Image1.Visible = False
        Finally
            If (con.State = ConnectionState.Open) Then
                con.Close()
            End If
        End Try

    End Sub
    Public Sub Change_Kc_LogicChildInMain(EID As String, ChildName As String, ParentName As String, ByVal tran As SqlTransaction, ByVal con As SqlConnection)

        Dim dt As New DataTable
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select FieldId,Oldtid,Documenttype,displayname from mmm_mst_fields with (nolock) where Documenttype ='" & ChildName & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.Transaction = tran
        oda.Fill(dt)

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim dt2 As New DataTable
            Dim NewFieldid As String = dt.Rows(i).Item("FieldId").ToString()
            Dim oltid As String = dt.Rows(i).Item("Oldtid").ToString()
            oda.SelectCommand.CommandText = "select FieldId,Oldtid,dropdown,Kc_Logic from mmm_mst_fields   with (nolock) where eid =" & EID & " and Documenttype ='" & ParentName & "' and (dropdown like '%" & dt.Rows(i).Item("Oldtid").ToString() & "%' or KC_Logic like '%" & dt.Rows(i).Item("Oldtid").ToString() & "%')"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.Transaction = tran
            oda.Fill(dt2)
            For k As Integer = 0 To dt2.Rows.Count - 1
                Dim changeDropdown As String = dt2.Rows(k).Item("dropdown").ToString()
                If (dt2.Rows(k).Item("dropdown").ToString().Contains(oltid)) Then
                    changeDropdown = Replace(dt2.Rows(k).Item("dropdown").ToString(), oltid.Trim(), NewFieldid.Trim())
                End If
                Dim changeKc_Logic As String = dt2.Rows(k).Item("Kc_Logic").ToString()
                If (dt2.Rows(k).Item("Kc_Logic").ToString().Contains(oltid)) Then
                    changeKc_Logic = Replace(dt2.Rows(k).Item("Kc_Logic").ToString(), "-" + oltid.Trim(), "-" + NewFieldid.Trim())
                End If
                oda.SelectCommand.CommandText = "Update mmm_mst_fields set dropdown='" & changeDropdown & "' ,Kc_Logic='" & changeKc_Logic & "' where eid='" & EID & "' and Fieldid=" & dt2.Rows(k).Item("FieldId") & ""
                oda.SelectCommand.Transaction = tran
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Next

        Next


    End Sub


    Public Sub CloneForm(EID As String, OldForm As String, NewForm As String, ByVal con As SqlConnection, ByVal tran As SqlTransaction, Optional ByVal EventName As String = "", Optional ByRef ParentDoc As String = "")
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim tran As SqlTransaction = Nothing
        'Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)


        'con.Open()
        'Try
        'Dim OldForm As String = ddlOldForm.SelectedValue.ToString()
        'Dim NewForm As String = txtNewForm.ToString()

        oda.SelectCommand.CommandText = "InsertForm_FormCloning"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Transaction = tran
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.Parameters.AddWithValue("Eid", EID)
        oda.SelectCommand.Parameters.AddWithValue("OLDFORM", OldForm)
        oda.SelectCommand.Parameters.AddWithValue("NEWFORM", NewForm)
        oda.SelectCommand.Parameters.AddWithValue("EventName", EventName)
        oda.SelectCommand.ExecuteNonQuery()

        Dim odaa As SqlDataAdapter = New SqlDataAdapter("", con)
        '' '''''''''''''''''''''''''''''''''''' (Insert Trigger and workflow and authmatrix,Menu,Prerole_datafilter,Role,TEMPLATE,REPORT,reportscheduler,print_template)'''''''''''''''''''''''''''''''''''''''
        '  odaa.SelectCommand.CommandText = "select Eid from MMM_MST_Triggers Where Eid='" & Eidsource & "'"
        '  odaa.SelectCommand.CommandType = CommandType.Text
        '  odaa.SelectCommand.Transaction = tran
        '  Dim dttri As New DataTable
        '  odaa.Fill(dttri)
        odaa.SelectCommand.CommandText = "InsertTriggers_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.Parameters.AddWithValue("@BaseDocType", ParentDoc)
        If (EventName <> "") Then
            odaa.SelectCommand.Parameters.AddWithValue("BaseDocTypeReplace", EventName)
        Else
            odaa.SelectCommand.Parameters.AddWithValue("BaseDocTypeReplace", NewForm)
        End If
        odaa.SelectCommand.ExecuteNonQuery()

        'odaa.SelectCommand.CommandText = "Update MMM_MST_Triggers set TriggerText=replace(TriggerText,'=" & Eidsource & "','=" & res & "') Where eid='" & res & "' "
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Parameters.Clear()
        'If con.State <> ConnectionState.Open Then
        '    con.Open()
        'End If
        'odaa.SelectCommand.ExecuteNonQuery()

        'Dim Eidsource As String = "49"
        'Dim res As String = "59"
        'odaa.SelectCommand.CommandText = "select Eid from MMM_MST_Workflow_Status with (nolock) Where Eid='" & EID & "' and documenttype='" & OldForm & "'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtwrk As New DataTable
        'odaa.Fill(dtwrk)
        odaa.SelectCommand.CommandText = "InsertWorkflowStatus_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()

        ''''''''qry starts for ExportMapping

        'odaa.SelectCommand.CommandText = "select Eid from mmm_mst_exportmapping with (nolock) Where Eid='" & EID & "' and sDocType='" & OldForm & "' "
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtexpmapp As New DataTable
        'odaa.Fill(dtexpmapp)
        odaa.SelectCommand.CommandText = "InsertExportMapping_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()



        ''End of Code

        'odaa.SelectCommand.CommandText = "select Eid from MMM_MST_RULES with (nolock) Where Eid='" & EID & "' and Documenttype='" & OldForm & "'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtRULES As New DataTable
        'odaa.Fill(dtRULES)
        odaa.SelectCommand.CommandText = "InsertRULES_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        odaa.SelectCommand.ExecuteNonQuery()

        ''odaa.SelectCommand.CommandText = "select Eid from MMM_MST_RuleRelation Where Eid=" & EID & " and (SourceName ='" & OldForm & "' or TargetName ='" & OldForm & "')"
        ''odaa.SelectCommand.CommandType = CommandType.Text
        ''odaa.SelectCommand.Transaction = tran
        ''Dim dtruEng As New DataTable
        ''odaa.Fill(dtruEng)
        odaa.SelectCommand.CommandText = "InsertRuleRelation_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()

        'odaa.SelectCommand.CommandText = "select Eid from MMM_MST_Relation Where Eid='" & Eidsource & "'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtres As New DataTable
        'odaa.Fill(dtres)
        'odaa.SelectCommand.CommandText = "InsertRelation"
        'odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        'odaa.SelectCommand.Transaction = tran
        'odaa.SelectCommand.Parameters.Clear()
        'odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
        'odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
        ' odaa.SelectCommand.ExecuteNonQuery()

        'odaa.SelectCommand.CommandText = "select Eid from MMM_MST_AuthMetrix with (nolock) Where Eid='" & EID & "' and doctype='" & OldForm & "'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtauth As New DataTable
        'odaa.Fill(dtauth)
        odaa.SelectCommand.CommandText = "InsertAuthMetrix_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()

        'odaa.SelectCommand.CommandText = "select Eid from mmm_mst_menu with (nolock) Where Eid='" & EID & "' and pagelink like '%SC=" & OldForm & "%'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtmenu As New DataTable
        'odaa.Fill(dtmenu)
        'odaa.SelectCommand.CommandText = "InsertMenu_FormCloning"
        'odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        'odaa.SelectCommand.Transaction = tran
        'odaa.SelectCommand.Parameters.Clear()
        'odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        'odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        'odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        'odaa.SelectCommand.ExecuteNonQuery()

        'odaa.SelectCommand.CommandText = "select Eid from MMM_PreRole_dataFilter with (nolock) Where Eid='" & EID & "' and Documenttype ='" & OldForm & "'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dtpre As New DataTable
        'odaa.Fill(dtpre)
        odaa.SelectCommand.CommandText = "InsertPreRoledataFilteraccount_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()

        'odaa.SelectCommand.CommandText = "select Eid from MMM_MST_TEMPLATE with (nolock) Where Eid='" & EID & "' and Eventname='" & OldForm & "'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dttemp As New DataTable
        'odaa.Fill(dttemp)
        odaa.SelectCommand.CommandText = "InsertTEMPLATE_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()

        '''''''''''''''''''''''''''''''''''FOR MENU''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        odaa.SelectCommand.CommandText = "select * from mmm_mst_menu with (nolock) Where Eid='" & EID & "' and pagelink like '%SC=" & NewForm & "%'"
        odaa.SelectCommand.CommandType = CommandType.Text

        odaa.SelectCommand.Transaction = tran
        Dim mv As New DataTable
        odaa.Fill(mv)
        Dim val As String = ""
        Dim flagg As Boolean = False
        For j As Integer = 0 To mv.Rows.Count - 1
            odaa.SelectCommand.CommandText = "select mid from mmm_mst_menu with (nolock) Where Eid='" & EID & "' and oldmid='" & mv.Rows(j).Item("oldmid").ToString() & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim inr As New DataTable
            odaa.Fill(inr)
            If inr.Rows.Count > 0 Then
                val = inr.Rows(0).Item("mid").ToString()
            End If
            odaa.SelectCommand.CommandText = "Update mmm_mst_menu set Pmenu='" & val & "'  where eid='" & EID & "' and pmenu='" & mv.Rows(j).Item("oldmid").ToString() & "'"
            odaa.SelectCommand.Transaction = tran
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            odaa.SelectCommand.ExecuteNonQuery()
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''''''''''''''''''''''''''''''''''FOR Rules Relation ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        odaa.SelectCommand.CommandText = "select RuleID,oldid from MMM_MST_RULES Where Eid=" & EID & " and Documenttype='" & NewForm & "'"
        odaa.SelectCommand.CommandType = CommandType.Text
        odaa.SelectCommand.Transaction = tran
        Dim mvs As New DataTable
        odaa.Fill(mvs)
        For j As Integer = 0 To mvs.Rows.Count - 1
            odaa.SelectCommand.CommandText = "select tid,RuleID from MMM_MST_RuleRelation with (nolock) Where Eid=" & EID & " and (SourceName='" & NewForm & "' or TargetName ='" & NewForm & "') and RuleID=" & mvs.Rows(j).Item("oldid").ToString() & ""
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim inr As New DataTable
            odaa.Fill(inr)
            If inr.Rows.Count > 0 Then
                odaa.SelectCommand.CommandText = "Update MMM_MST_RuleRelation set RuleID='" & mvs.Rows(j).Item("RuleID").ToString() & "'  where eid=" & EID & " and tid='" & inr.Rows(0).Item("tid").ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                odaa.SelectCommand.Transaction = tran
                odaa.SelectCommand.ExecuteNonQuery()
            End If
        Next
        ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


        'odaa.SelectCommand.CommandText = "select Eid,FormName from MMM_Mst_Forms with (nolock) Where Eid='" & EID & "' and FormName='" & OldForm & "'"
        '' odaa.SelectCommand.CommandText = "select Eid,FormName from MMM_Mst_Forms Where Eid='32'"
        'odaa.SelectCommand.CommandType = CommandType.Text
        'odaa.SelectCommand.Transaction = tran
        'Dim dt As New DataTable
        'odaa.Fill(dt)
        'For w As Integer = 0 To dt.Rows.Count - 1
        odaa.SelectCommand.CommandText = "AccountInsertFields_FormCloning"
        odaa.SelectCommand.CommandType = CommandType.StoredProcedure
        odaa.SelectCommand.Transaction = tran
        odaa.SelectCommand.Parameters.Clear()
        odaa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odaa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        odaa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        odaa.SelectCommand.ExecuteNonQuery()
        'Next
        '   For i As Integer = 0 To dt.Rows.Count - 1
        Dim dt11 As New DataTable
        Dim docname As String = OldForm
        Dim oda1 As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_Mst_Fields with (nolock) Where Eid='" & EID & "' and Documenttype='" & NewForm & "'", con)
        oda1.SelectCommand.Transaction = tran
        ' Dim oda1 As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_Mst_Fields Where Eid='146' and Documenttype ='" & docname & "'", con)
        oda1.Fill(dt11)
        For j As Integer = 0 To dt11.Rows.Count - 1
            'If (dt11.Rows(j).Item("displayname").ToString() = "Test MultiLookupDDl Field") Then
            '    Dim field = dt11.Rows(j).Item("displayname").ToString()
            'End If

            Dim documenttype As String = dt11.Rows(j).Item("Documenttype")
            Dim oldfieldid As String = dt11.Rows(j).Item("Oldtid")
            Dim newfieldid As String = dt11.Rows(j).Item("Fieldid")
            oda1.SelectCommand.CommandText = "Select * from MMM_Mst_Fields with (nolock) Where Eid='" & EID & "' and Documenttype='" & NewForm & "'  and oldtid <> '" & dt11.Rows(j).Item("Oldtid").ToString().Trim() & "'"
            oda1.SelectCommand.CommandType = CommandType.Text
            oda1.SelectCommand.Transaction = tran
            Dim dt1 As New DataTable
            oda1.Fill(dt1)

            For k As Integer = 0 To dt1.Rows.Count - 1
                Dim changelukupvalue As String = String.Empty
                Dim changelukupvaluenew As String = String.Empty
                Dim changeddllukupvalue As String = String.Empty
                Dim changeddlmultilukupvalue As String = String.Empty
                Dim changedropdownvalue As String = String.Empty
                Dim changekclogicvalue As String = String.Empty
                Dim changedependentONvalue As String = String.Empty
                Dim changeinitialFiltervalue As String = String.Empty
                Dim lookupvalue As String = dt1.Rows(k).Item("lookupvalue").ToString().Trim()
                Dim ddllookupvalue As String = dt1.Rows(k).Item("ddllookupvalue").ToString().Trim()
                'If (dt11.Rows(j).Item("displayname").ToString() = "Test MultiLookupDDl Field") Then
                '    If (dt1.Rows(k).Item("displayname").ToString() = "Expense Category" Or dt1.Rows(k).Item("displayname").ToString() = "User Value") Then
                '        Dim cc = dt1.Rows(k).Item("displayname").ToString()
                '    End If
                'End If
                Dim ddlMultilookupvalue As String = dt1.Rows(k).Item("ddlMultilookupval").ToString().Trim()
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
                '16808-fld10,
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
                'Petty Cash Voucher HUB-fld11-14037,
                '''''''''''''''''''''''' For DDlmultiLukUp'''''''''''''''''''''''''''
                If (ddlMultilookupvalue <> "" And ddlMultilookupvalue.Length > 1) Then
                    Dim modifyddlmultilukupvalue As String = ""
                    If (ddllookupvalue.EndsWith(",")) Then
                        ddlMultilookupvalue = ddlMultilookupvalue.Substring(0, ddlMultilookupvalue.Length - 1)
                    Else
                        ddlMultilookupvalue = ddlMultilookupvalue.Substring(0, ddlMultilookupvalue.Length)
                    End If
                    'ddlMultilookupvalue = ddlMultilookupvalue.Substring(0, ddlMultilookupvalue.Length - 1)
                    Dim splitddlmultilukup As String() = Split(ddlMultilookupvalue.Trim(), ",")
                    For n As Integer = 0 To splitddlmultilukup.Length - 1
                        Dim B As String() = Split(splitddlmultilukup(n), "-")
                        If B.Length > 2 Then
                            Dim mainvalueddlmultilukup As String = B(2)
                            If mainvalueddlmultilukup.Trim() = oldfieldid.Trim() Then
                                modifyddlmultilukupvalue &= txtNewForm.Text & "-" & B(1) & "-" & newfieldid.Trim() & ","
                                flag = True
                            Else
                                modifyddlmultilukupvalue &= ddlMultilookupvalue
                            End If
                        End If
                    Next
                    changeddlmultilukupvalue = modifyddlmultilukupvalue.Trim()
                End If

                ''''''''''''''''''''' For DropDown''''''''''''''''''''''''''''''''''''
                If (dropdown <> "" And dropdown.Length > 1) Then
                    If dropdown.Trim().Contains(oldfieldid.Trim()) Then
                        changedropdownvalue = Replace(dropdown, oldfieldid.Trim(), newfieldid.Trim())
                        flag = True
                    Else
                        changedropdownvalue = dropdown.Trim()
                    End If
                End If
                ''''''''''''''''''''' For KC_LOgic''''''''''''''''''''''''''''''''''''
                '18740-9635
                If (KC_LOGIC <> "" And KC_LOGIC.Length > 1) Then
                    Dim modifykclogicvalue As String = KC_LOGIC

                    If (KC_LOGIC.Trim().Contains(oldfieldid)) Then
                        modifykclogicvalue = Replace(modifykclogicvalue, oldfieldid + "-", newfieldid + "-")
                        modifykclogicvalue = Replace(modifykclogicvalue, "-" + oldfieldid, "-" + newfieldid)
                        flag = True
                    End If
                    changekclogicvalue = modifykclogicvalue.Trim()

                    'Dim splitkclogic As String() = Split(KC_LOGIC, "-")
                    'For p As Integer = 0 To splitkclogic.Length - 1
                    '    If splitkclogic.Length > 1 Then
                    '        Dim mainkclogicvalue As String = splitkclogic(p)
                    '        If mainkclogicvalue.Trim() = oldfieldid.Trim() Then
                    '            modifykclogicvalue &= newfieldid.Trim() & "-"
                    '            flag = True
                    '        Else
                    '            modifykclogicvalue &= mainkclogicvalue.Trim() & "-"
                    '        End If
                    '        changekclogicvalue = modifykclogicvalue.Trim()
                    '    End If
                    'Next

                    If changekclogicvalue <> "" Then
                        If (changekclogicvalue.EndsWith(",")) Then
                            changekclogicvalue = changekclogicvalue.Substring(0, changekclogicvalue.Length - 1)
                        Else
                            changekclogicvalue = changekclogicvalue.Trim()
                        End If

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
                    oda1.SelectCommand.CommandText = "Update MMM_Mst_Fields set lookupvalue='" & changelukupvalue & "',ddllookupvalue='" & changeddllukupvalue &
                        "',dropdown='" & changedropdownvalue & "',KC_LOGIC='" & changekclogicvalue &
                        "',dependentON='" & changedependentONvalue & "',initialFilter='" & changeinitialFiltervalue & "'," &
                         "  ddlmultilookupval = '" & changeddlmultilukupvalue & "' where eid='" & EID & "' and Documenttype='" &
                                                NewForm & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                    ' oda1.SelectCommand.CommandText = "Update MMM_Mst_Fields set tempcount=tempcount+1,templukupval='" & changelukupvaluenew & "',lookupvalue='" & changelukupvalue & "'  where Eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                    oda1.SelectCommand.CommandType = CommandType.Text
                    oda1.SelectCommand.Transaction = tran
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda1.SelectCommand.ExecuteNonQuery()
                End If
                If (cal_fields <> "") Then
                    ' oda1.SelectCommand.CommandText = "update MMM_Mst_Fields set cal_fields=replace(cal_fields,'fld" & oldfieldid & "'')','fld" & newfieldid & "'')') where Eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                    oda1.SelectCommand.CommandText = "update MMM_Mst_Fields set cal_fields=replace(cal_fields,'fld" & oldfieldid & "'')','fld" & newfieldid & "'')') where eid='" & EID & "' and Documenttype='" & NewForm & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                    oda1.SelectCommand.CommandType = CommandType.Text
                    oda1.SelectCommand.Transaction = tran
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda1.SelectCommand.ExecuteNonQuery()
                End If
                ' End If
            Next
            dt1.Dispose()
        Next

        ' Next


        Dim daa As SqlDataAdapter = New SqlDataAdapter("", con)
        'daa.SelectCommand.CommandType = CommandType.Text
        'daa.SelectCommand.Transaction = tran
        'Dim dtf As New DataTable
        'daa.Fill(dtf)
        daa.SelectCommand.CommandText = "InsertFORMVALIDATION_FormCloning"
        daa.SelectCommand.CommandType = CommandType.StoredProcedure
        daa.SelectCommand.Transaction = tran
        daa.SelectCommand.Parameters.Clear()
        daa.SelectCommand.Parameters.AddWithValue("Eid", EID)
        daa.SelectCommand.Parameters.AddWithValue("Oldform", OldForm)
        daa.SelectCommand.Parameters.AddWithValue("Newform", NewForm)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        daa.SelectCommand.ExecuteNonQuery()
        'dtf.Dispose()

        ' Dim res As String = "165"
        Dim da As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_MST_FORMVALIDATION with (nolock) Where Eid='" & EID & "' and DocType ='" & NewForm & "'", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.Transaction = tran
        Dim dv As New DataTable
        da.Fill(dv)
        For j As Integer = 0 To dv.Rows.Count - 1
            Dim documenttype As String = dv.Rows(j).Item("DocTYPE")
            Dim fldID As String = dv.Rows(j).Item("fldID")
            Dim Oprator As String = dv.Rows(j).Item("Operator")
            Dim Value As String = dv.Rows(j).Item("Value")
            Dim changefldIDvalue As String = ""
            Dim changeOpratorvalue As String = ""
            Dim changevalue As String = ""
            Dim flagV As Boolean = False

            If (fldID <> "") Or (Oprator <> "") Then
                If (fldID <> "") Then
                    Dim valuefldid As String() = Split(fldID, "fld")
                    Dim a1 As String = valuefldid(1)
                    da.SelectCommand.CommandText = "Select fieldid from MMM_Mst_Fields with (nolock) Where Eid='" & EID & "' and Documenttype ='" & NewForm & "' and oldtid='" & a1 & "'"
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Transaction = tran
                    Dim dte As New DataTable
                    da.Fill(dte)
                    If dte.Rows.Count > 0 Then
                        changefldIDvalue = "fld" & dte.Rows(0).Item("fieldid").ToString()
                        flagV = True
                    Else
                        changefldIDvalue = fldID
                    End If
                    dte.Dispose()
                End If
                If (Oprator <> "") Then
                    Dim dto As New DataTable
                    If Oprator.Contains("fld") Then
                        Dim valueOprator As String() = Split(Oprator, "fld")
                        Dim ba1 As String = valueOprator(1)
                        da.SelectCommand.CommandText = "Select fieldid from MMM_Mst_Fields with (nolock) Where Eid='" & EID & "' and Documenttype ='" & NewForm & "'  and oldtid='" & ba1 & "'"
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Transaction = tran
                        da.Fill(dto)
                        If dto.Rows.Count > 0 Then
                            changeOpratorvalue = "fld" & dto.Rows(0).Item("fieldid").ToString()
                            flagV = True
                        End If
                    Else
                        changeOpratorvalue = Oprator
                    End If
                    dto.Dispose()
                End If
            End If
            If (Value <> "") Then
                Dim valueOprator As String = Value & ":"
                Dim ba1 As String() = Split(valueOprator, ":")
                'valueOprator = valueOprator.Substring(0, valueOprator.Length - 1)
                For er As Integer = 0 To ba1.Length - 1
                    If ba1(er).ToString() <> "" Then
                        Dim dev As String = ba1(er) & ":"
                        Dim ert As String() = Split(dev, "=")
                        For g As Integer = 0 To ert.Length - 1
                            If ert(g).Contains(":") Then
                                If ert(g).Contains("fld") Then
                                    Dim y As String = ert(g).Replace("fld", "")
                                    Dim q As String = y.Replace(":", "")
                                    da.SelectCommand.CommandText = "Select fieldid from MMM_Mst_Fields with (nolock) Where Eid='" & EID & "' and Documenttype ='" & NewForm & "'  and oldtid='" & q & "'"
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Transaction = tran
                                    Dim dtv As New DataTable
                                    da.Fill(dtv)
                                    If dtv.Rows.Count > 0 Then
                                        Dim oldfld As String = "fld" & q & ":"
                                        Dim newfld As String = "fld" & dtv.Rows(0).Item("fieldid").ToString() & ":"
                                        valueOprator = valueOprator.Replace(oldfld, newfld)
                                    End If
                                End If
                            End If
                        Next
                    End If
                Next
                valueOprator = valueOprator.Substring(0, valueOprator.Length - 1)
                da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set Value='" & valueOprator & "'  where eid='" & EID & "' and doctype ='" & NewForm & "'  and TID='" & dv.Rows(j).Item("TID") & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()
            End If


            ''''''''''''''''''''''''''Update Query'''''''''''''''''''''''''''''''''' 
            If flagV = True Then
                da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set fldID='" & changefldIDvalue & "',Operator='" & changeOpratorvalue & "' where eid='" & EID & "' and doctype ='" & NewForm & "'  and TID='" & dv.Rows(j).Item("TID") & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()
            End If
            'If (Value <> "") Then
            '    da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set Value=replace(Value + ':','fld' +'" & oldfieldidp & "' +':','fld' +'" & newfieldidp & "' +':')  where eid='" & res & "' and TID='" & dt1.Rows(j).Item("TID") & "'"
            '    If con.State <> ConnectionState.Open Then
            '        con.Open()
            '    End If
            '    da.SelectCommand.ExecuteNonQuery()
            '    da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set Value=left(value,len(value)-1)  where eid='" & res & "' and TID='" & dt1.Rows(j).Item("TID") & "'"
            '    If con.State <> ConnectionState.Open Then
            '        con.Open()
            '    End If
            '    da.SelectCommand.ExecuteNonQuery()
            'End If
        Next
        da.Dispose()

        ''''''''''''''''''''''''''''''''''''''''''''Create view''''''''''''''''''''''''''''''''''''''
        Dim odv As SqlDataAdapter = New SqlDataAdapter("", con)
        odv.SelectCommand.CommandText = "CreateNewView_FormCloning"
        odv.SelectCommand.CommandType = CommandType.StoredProcedure
        odv.SelectCommand.Transaction = tran
        odv.SelectCommand.Parameters.Clear()
        odv.SelectCommand.Parameters.AddWithValue("Eid", EID)
        odv.SelectCommand.Parameters.AddWithValue("@FormNm", NewForm)
        odv.SelectCommand.ExecuteNonQuery()
        ' ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' reset()
        'lblmsgg.Visible = True
        'lblmsgg.Text = "New Account created through Cloning Successfully!"
        'Commiting transaction 
        '  tran.Commit()

        'Catch ex As Exception
        '    If Not tran Is Nothing Then
        '        tran.Rollback()
        '    End If
        '    If Not con Is Nothing Then
        '        con.Close()
        '    End If
        '    lblMsg.Text = "Error found - " & ex.Message.ToString()
        '    ' Image1.Visible = False


        'Finally

        'End Try
    End Sub

    Public Sub reset()
        txtNewForm.Text = ""
        ddlFormType.SelectedIndex = 0
        ddlOldForm.SelectedIndex = 0
        txtNewFormSuffix.Text = ""
    End Sub


    Protected Sub ddlFormType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFormType.SelectedIndexChanged
        Try


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Dim Qstr As String = ""

            Dim EID As String = ""
            If Not IsNothing(Session("EID")) Then
                EID = Session("EID")
            End If

            If ddlFormType.SelectedItem.Text.ToUpper = "DOCUMENTS" Then
                Qstr = "Select FORMNAME, FORMCAPTION + '(' + FORMNAME + ')' [formCaption] from MMM_MST_FORMS with (nolock) WHERE FORMTYPE='DOCUMENT' AND FORMSOURCE='MENU DRIVEN' AND EID=" & EID & " order by FORMNAME"
            ElseIf ddlFormType.SelectedItem.Text.ToUpper = "MASTERS" Then
                Qstr = "Select FORMNAME, FORMCAPTION + '(' + FORMNAME + ')'  [formCaption] from MMM_MST_FORMS with (nolock) WHERE FORMTYPE='MASTER' AND FORMSOURCE='MENU DRIVEN' AND EID=" & EID & " order by FORMNAME"
            ElseIf ddlFormType.SelectedItem.Text.ToUpper = "ACTION FORMS" Then
                Qstr = "Select FORMNAME, FORMCAPTION + '(' + FORMNAME + ')'  [formCaption] from MMM_MST_FORMS with (nolock) WHERE FORMTYPE='DOCUMENT' AND FORMSOURCE='DETAIL FORM' AND EID=" & EID & " order by FORMNAME"
            ElseIf ddlFormType.SelectedItem.Text.ToUpper = "DETAIL FORMS" Then
                Qstr = "Select FORMNAME, FORMCAPTION + '(' + FORMNAME + ')' [formCaption] from MMM_MST_FORMS with (nolock) WHERE FORMTYPE='DOCUMENT' AND FORMSOURCE='ACTION DRIVEN'  AND EID=" & EID & " order by FORMNAME"
            End If
            oda.SelectCommand.CommandText = Qstr
            oda.Fill(ds, "forms")
            ddlOldForm.DataSource = ds.Tables("forms")
            ddlOldForm.DataTextField = "formCaption"
            ddlOldForm.DataValueField = "formname"
            ddlOldForm.DataBind()
            ddlOldForm.Items.Insert(0, "SELECT")
            oda.Dispose()
            ds.Dispose()
            con.Close()
            con.Dispose()
        Catch ex As Exception

        End Try
    End Sub


End Class
