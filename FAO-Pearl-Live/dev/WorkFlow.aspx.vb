Imports System.Data
Imports System.Data.SqlClient

Partial Class WorkFlow
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='WORKFLOW' order by DisName", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next

            da.SelectCommand.CommandText = "SELECT Username,uid from MMM_MST_USER where userrole='USR' and (eid=" & Session("EID").ToString() & " or eid=0) and username<>'ARCHIVE' order by UserName"
            da.Fill(ds, "unit")
            ddlOwner.Items.Clear()
            ddlOwner.Items.Add("Please Select")
            For i As Integer = 0 To ds.Tables("unit").Rows.Count - 1
                ddlOwner.Items.Add(ds.Tables("unit").Rows(i).Item(0))
                ddlOwner.Items(i + 1).Value = ds.Tables("unit").Rows(i).Item(1).ToString()
            Next
            '' new added for doc type drop down filling
            da.SelectCommand.CommandText = "SELECT Formname,Formid from MMM_MST_FORMS where eid=" & Session("EID").ToString() & " and formtype='DOCUMENT' and formsource='MENU DRIVEN' and isactive=1 order by FormName"
            da.Fill(ds, "dtype")
            ddlDocumentType.Items.Clear()
            ddlDocumentType.Items.Add("Please Select")
            For i As Integer = 0 To ds.Tables("dtype").Rows.Count - 1
                ddlDocumentType.Items.Add(ds.Tables("dtype").Rows(i).Item(0))
                ddlDocumentType.Items(i + 1).Value = ds.Tables("dtype").Rows(i).Item(1).ToString()
            Next

            da.SelectCommand.CommandText = "SELECT tid,eid,dord,StatusName from MMM_MST_WORKFLOW_status where Eid=" & Session("EID").ToString() & "  order by dord"
            da.Fill(ds, "WFS")
            ddlWFStatus.Items.Clear()
            ddlWFStatus.Items.Add("Please Select")
            For i As Integer = 0 To ds.Tables("WFS").Rows.Count - 1
                ddlWFStatus.Items.Add(ds.Tables("WFS").Rows(i).Item("StatusName"))
                ddlWFStatus.Items(i + 1).Value = ds.Tables("WFS").Rows(i).Item("tid").ToString()
            Next

            da.Dispose()
            ds.Dispose()
            con.Dispose()
        End If
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
  Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        ' No Value in Session just fill the Edit Form and Show two button
        btnActEdit.Text = "Update"

        'two methods.. either show data from Grid or Show data from Database.
        ViewState("pid") = pid
        txtName.Text = row.Cells(1).Text

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

        oda.SelectCommand.CommandText = "select documenttype from MMM_MST_WORKFLOW where wfid=" & pid
        oda.SelectCommand.CommandType = CommandType.Text
        Dim dtDc As New DataTable
        oda.Fill(dtDc)
        Dim DocType As String
        If dtDc.Rows.Count <> 0 Then
            DocType = dtDc.Rows(0).Item("documenttype").ToString
        Else
            DocType = "FILE"
        End If

        oda.SelectCommand.CommandText = "select displayname, fieldMapping  from MMM_MST_FIELDS where eid=" & Session("EID").ToString & " AND documenttype='" & DocType & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        Dim dt As New DataTable
        Dim ds As New DataSet
        oda.Fill(ds, "data")
        Dim OrigVal() As String = Split(row.Cells(2).Text, " ")
        Dim ConvValue() As String = Split(row.Cells(2).Text, " ")

        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            For j As Integer = 0 To OrigVal.Count - 1
                If ds.Tables("data").Rows(i).Item("fieldMapping") <> OrigVal(j) Then
                    j = j + 1
                Else
                    ConvValue(j) = Replace(OrigVal(j), ds.Tables("data").Rows(i).Item("fieldMapping").ToString, ds.Tables("data").Rows(i).Item("displayname").ToString)

                    txtLogic.Text = String.Join(" ", ConvValue)
                    txtLogic.Text = Replace(txtLogic.Text, "&#39;", "'")
                    ' &#39; this is coming when we are adding single quote . we are removing 
                End If

            Next

        Next


        '' here on yesterday (19th feb) correct replace logic of fld1 with fld 11 etc...
        'Dim fields As String
        'Dim displayName As String
        'For i As Integer = 0 To dt.Tables("data").Rows.Count - 1
        '    fields = dt.Tables("data").Rows(i).Item("fieldMapping").ToString()
        '    displayName = dt.Tables("data").Rows(i).Item("displayname").ToString()
        'Next

        'For i As Integer = 0 To dt.Rows.Count - 1

        '    OrigVal = Replace(OrigVal, dt.Rows(i).Item("fieldMapping").ToString, dt.Rows(i).Item("displayname").ToString)

        'Next

        'ConvValue = Trim(ConvValue)

        'txtLogic.Text = ConvValue


        ddlDocumentType.SelectedIndex = ddlDocumentType.Items.IndexOf(ddlDocumentType.Items.FindByText(DocType))
        ddlOwner.SelectedIndex = ddlOwner.Items.IndexOf(ddlOwner.Items.FindByText(row.Cells(3).Text))

        oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid=" & pid & " order by flowpos"
        Dim dr As New DataSet()
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(dr, "data")
        gvUsers.DataSource = dr.Tables("data")
        gvUsers.DataBind()
        dt.Dispose()
        con.Close()
        oda.Dispose()
        con.Dispose()
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub



    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        txtName.Text = ""
        txtLogic.Text = ""
        ddlOwner.SelectedIndex = ddlOwner.Items.IndexOf(ddlOwner.Items.FindByText("Please Select"))

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid=0 and sessionid ='" & Session.SessionID & "' order by flowpos"
        Dim ds As New DataSet()
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvUsers.DataSource = ds.Tables("data")
        gvUsers.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this WorkFlow? " & row.Cells(1).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub


    Protected Sub DeleteHitUser(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)
        ViewState("Deltid") = tid
        lblMsgDeleteUser.Text = "Are you Sure Want to delete User - " & row.Cells(1).Text
        Me.updatePanelDeleteUser.Update()
        Me.btnDeleteUser_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteRecordUser(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        'Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        'Dim pid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)

        Dim tid As Integer = ViewState("Deltid")

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteUserInWF", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()

        If btnActEdit.Text = "Save" Then
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid=0 and sessionid ='" & Session.SessionID & "' order by flowpos"
        Else
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid =" & ViewState("pid") & " order by flowpos"
        End If

        Dim ds As New DataSet()
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvUsers.DataSource = ds.Tables("data")
        gvUsers.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
        btnDeleteUser_ModalPopupExtender.Hide()
        updatePanelEdit.Update()

    End Sub

    Protected Sub PositionUp(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        If row.RowIndex = 0 Then
            Exit Sub
        End If

        Dim tid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)
        Dim ntid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex - 1).Value)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspWFPositionUpdate", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        oda.SelectCommand.Parameters.AddWithValue("ntid", ntid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        If btnActEdit.Text = "Save" Then
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid=0 and sessionid ='" & Session.SessionID & "' order by flowpos"
        Else
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid =" & ViewState("pid") & " order by flowpos"
        End If

        Dim ds As New DataSet()
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvUsers.DataSource = ds.Tables("data")
        gvUsers.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
        updatePanelEdit.Update()
    End Sub
    Protected Sub PositionDown(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        If row.RowIndex >= gvUsers.Rows.Count - 1 Then
            Exit Sub
        End If

        Dim tid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)
        Dim ntid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex + 1).Value)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspWFPositionUpdate", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        oda.SelectCommand.Parameters.AddWithValue("ntid", ntid)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        oda.SelectCommand.ExecuteNonQuery()
        If btnActEdit.Text = "Save" Then
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid=0 and sessionid ='" & Session.SessionID & "' order by flowpos"
        Else
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid =" & ViewState("pid") & " order by flowpos"
        End If

        Dim ds As New DataSet()
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvUsers.DataSource = ds.Tables("data")
        gvUsers.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
        updatePanelEdit.Update()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        'validation for null entry

        If txtName.Text.Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid WorkFlow Name"
            Exit Sub
        End If

        If Len(txtLogic.Text) < 10 Then
            lblMsgEdit.Text = "Please select Valid Condition (Logic)"
            Exit Sub
        End If

        If ddlType.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Please Select Valid Field"
            Exit Sub
        End If

        If gvUsers.Rows.Count < 1 Then
            lblMsgEdit.Text = "Please Add User in the WorkFlow"
            Exit Sub
        End If
        Dim logicval As String = ""


        For i As Integer = 0 To ddlType.Items.Count - 1
            txtLogic.Text = Replace(txtLogic.Text, ddlType.Items(i).Text, ddlType.Items(i).Value)
        Next
        logicval = txtLogic.Text
        logicval = Trim(logicval)
        If Right(logicval, 3) = "AND" Then
            logicval = Left(logicval, Len(logicval) - 3)
        End If
        If Right(logicval, 2) = "OR" Then
            logicval = Left(logicval, Len(logicval) - 2)
        End If

        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertWF", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("wfname", txtName.Text)

            'If ddlType.SelectedItem.Text = "FILENAME CONTAINS" Then
            '    logicval = " fname like '%" & txtLogic.Text & "%' "
            'ElseIf ddlType.SelectedItem.Text = "FOLDERNAME CONTAINS" Then
            '    logicval = " foldername like '%" & txtLogic.Text & "%' "
            'Else
            '    Dim fldname As String = ""
            '    ' Dim pos As Integer = InStr(ddlType.SelectedItem.Text, "(")
            '    ' Dim pos2 As Integer = InStr(ddlType.SelectedItem.Text, ")")
            '    ' fldname = ddlType.SelectedItem.Text.Substring(pos, (pos2 - pos) - 1)
            '    fldname = ddlType.SelectedItem.Value
            '    logicval = " " & fldname & " like '%" & txtLogic.Text & "%' "
            'End If
            oda.SelectCommand.Parameters.AddWithValue("wflogic", logicval)
            oda.SelectCommand.Parameters.AddWithValue("sessionid", Session.SessionID)
            oda.SelectCommand.Parameters.AddWithValue("documenttype", ddlDocumentType.SelectedItem.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            con.Dispose()
            If iSt = 0 Then
                txtName.Text = ""
                updatePanelEdit.Update()
                gvData.DataBind()
                btnEdit_ModalPopupExtender.Hide()
            Else
                lblMsgEdit.Text = "This Workflow Already Exist"
                updatePanelEdit.Update()
            End If
        Else
            'Edit Record
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateWF", con)
            Dim pid As Integer = Val(ViewState("pid").ToString())
            oda.SelectCommand.CommandType = CommandType.StoredProcedure

            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("wfname", txtName.Text)

            'If ddlType.SelectedItem.Text = "FILENAME CONTAINS" Then
            '    logicval = " fname like '%" & txtLogic.Text & "%' "
            'ElseIf ddlType.SelectedItem.Text = "FOLDERNAME CONTAINS" Then
            '    logicval = " foldername like '%" & txtLogic.Text & "%' "
            'Else
            '    Dim fldname As String = ""
            '    ' Dim pos As Integer = InStr(ddlType.SelectedItem.Text, "(")
            '    ' Dim pos2 As Integer = InStr(ddlType.SelectedItem.Text, ")")
            '    ' fldname = ddlType.SelectedItem.Text.Substring(pos, (pos2 - pos) - 1)
            '    fldname = ddlType.SelectedItem.Value
            '    logicval = " " & fldname & " like '%" & txtLogic.Text & "%' "
            'End If            
            oda.SelectCommand.Parameters.AddWithValue("wflogic", logicval)
            oda.SelectCommand.Parameters.AddWithValue("wfid", pid)
            oda.SelectCommand.Parameters.AddWithValue("documenttype", ddlDocumentType.SelectedItem.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            con.Dispose()
            If iSt = 0 Then
                txtName.Text = ""
                updatePanelEdit.Update()
                gvData.DataBind()
                btnEdit_ModalPopupExtender.Hide()
            Else
                lblMsgEdit.Text = "This Workflow Already Exist"
                updatePanelEdit.Update()
            End If
        End If
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pid As String = ViewState("pid").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteWF", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("wfid", pid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Then
            gvData.DataBind()
            btnDelete_ModalPopupExtender.Hide()
        Else
            lblMsgDelete.Text = "This WorkFlow can Not be Deleted"
            updatePanelDelete.Update()
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataBind()
    End Sub

    Protected Sub AdduserInWorkFlow(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click
        'validation for null entry

        If ddlOwner.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Please Select Valid Approver To Add"
            Exit Sub
        End If

        If ddlWFStatus.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Please Select Valid Status To Add"
            Exit Sub
        End If

        If Val(txtSLA.Text) < 1 Then
            lblMsgEdit.Text = "Please Enter Valid SLA"
            Exit Sub
        End If

        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertUserInWF", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("wfid", 0)
            oda.SelectCommand.Parameters.AddWithValue("userid", ddlOwner.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("WFStatus", ddlWFStatus.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("sla", Val(txtSLA.Text))
            oda.SelectCommand.Parameters.AddWithValue("sessionid", Session.SessionID)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid=0 and sessionid ='" & Session.SessionID & "' order by flowpos"
            Dim ds As New DataSet()
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data")
            gvUsers.DataSource = ds.Tables("data")
            gvUsers.DataBind()
            con.Close()
            oda.Dispose()
            con.Dispose()
            updatePanelEdit.Update()
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim wfid As Integer = Val(ViewState("pid").ToString())
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertUserInWF", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("wfid", wfid)
            oda.SelectCommand.Parameters.AddWithValue("userid", ddlOwner.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("WFStatus", ddlWFStatus.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("sla", Val(txtSLA.Text))
            oda.SelectCommand.Parameters.AddWithValue("sessionid", Session.SessionID)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            oda.SelectCommand.CommandText = "Select tid,username,sla,WFstatus from MMM_WORKFLOW_USER W LEFT outer join MMM_MST_USER U on U.uid = W.userid where wfid =" & wfid & " order by flowpos"
            Dim ds As New DataSet()
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data")
            gvUsers.DataSource = ds.Tables("data")
            gvUsers.DataBind()
            con.Close()
            oda.Dispose()
            con.Dispose()
            updatePanelEdit.Update()
        End If
    End Sub

    Protected Sub ddlDocumentType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDocumentType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select displayname  [FieldName], fieldMapping  from MMM_MST_FIELDS where eid=" & Session("EID").ToString & " AND documenttype='" & ddlDocumentType.SelectedItem.Text & "'", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim dt As New DataTable
        oda.Fill(dt)
        ddlType.Items.Clear()
        ddlType.Items.Add("SELECT")
        ddlType.Items(0).Value = "0"
        ddlType.Items.Add("FILENAME")
        ddlType.Items(1).Value = "fname"
        ddlType.Items.Add("FOLDERNAME")
        ddlType.Items(2).Value = "foldername"

        For i As Integer = 0 To dt.Rows.Count - 1
            ddlType.Items.Add(dt.Rows(i).Item(0))
            ddlType.Items(i + 3).Value = dt.Rows(i).Item(1).ToString()
        Next

        dt.Dispose()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub

    Protected Sub btnAddCond_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddCond.Click
        Dim CurDtType As String = ""

        If txtCondValue.Text = "" Then
            lblMsgEdit.Text = "Please type valid value!"
            txtCondValue.Focus()
            Exit Sub
        End If

        If ddlType.SelectedItem.Text = "SELECT" Then
            lblMsgEdit.Text = "Please select Field Name for Condition!"
            ddlType.Focus()
            Exit Sub
        End If

        If ddlType.SelectedItem.Text = "FILENAME" Then
            txtLogic.Text &= "fname" & " " & ddlOperator.SelectedItem.Text & " " & txtCondValue.Text & " " & ddlAndOR.SelectedItem.Text & " "
        ElseIf ddlType.SelectedItem.Text = "FOLDERNAME" Then
            txtLogic.Text &= "foldername" & " " & ddlOperator.SelectedItem.Text & " " & txtCondValue.Text & " " & ddlAndOR.SelectedItem.Text & " "
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("select displayname, fieldMapping, datatype  from MMM_MST_FIELDS where eid=" & Session("EID").ToString & " AND displayname='" & ddlType.SelectedItem.Text & "' and fieldMapping='" & ddlType.SelectedItem.Value & "'", con)
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataTable
            oda.Fill(dt)

            If dt.Rows(0).Item("datatype").ToString().ToUpper = "TEXT" Then
                CurDtType = "TEXT"
                'ElseIf dt.Rows(0).Item("datatype").ToString().ToUpper = "NUMERIC" Then
                'ElseIf dt.Rows(0).Item("datatype").ToString().ToUpper = "DATETIME" Then
            End If

            dt.Dispose()
            con.Close()
            oda.Dispose()
            con.Dispose()
            If CurDtType = "TEXT" Then
                If ddlOperator.SelectedItem.Text = "Like" Then
                    txtLogic.Text &= ddlType.SelectedItem.Text & " " & ddlOperator.SelectedItem.Text & " '%" & txtCondValue.Text & "%' " & ddlAndOR.SelectedItem.Text & " "
                Else                 ' If ddlOperator.SelectedItem.Text = "=" Then
                    txtLogic.Text &= ddlType.SelectedItem.Text & " " & ddlOperator.SelectedItem.Text & " '" & txtCondValue.Text & "' " & ddlAndOR.SelectedItem.Text & " "
                End If
            Else
                txtLogic.Text &= ddlType.SelectedItem.Text & " " & ddlOperator.SelectedItem.Text & " " & txtCondValue.Text & " " & ddlAndOR.SelectedItem.Text & " "
            End If


        End If


    End Sub

    Protected Sub ddlType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlType.SelectedIndexChanged
        ddlOperator.Items.Clear()
        If ddlType.SelectedItem.Text = "SELECT" Then
            Exit Sub
        End If
        If ddlType.SelectedItem.Text = "FILENAME" Or ddlType.SelectedItem.Text = "FOLDERNAME" Then
            ddlOperator.Items.Add("Like")
            ddlOperator.Items.Add("=")
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("select displayname, fieldMapping, datatype  from MMM_MST_FIELDS where eid=" & Session("EID").ToString & " AND displayname='" & ddlType.SelectedItem.Text & "' and fieldMapping='" & ddlType.SelectedItem.Value & "'", con)
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataTable
            oda.Fill(dt)

            If dt.Rows(0).Item("datatype").ToString().ToUpper = "TEXT" Then
                ddlOperator.Items.Add("Like")
                ddlOperator.Items.Add("=")
            ElseIf dt.Rows(0).Item("datatype").ToString().ToUpper = "NUMERIC" Then
                ddlOperator.Items.Add("=")
                ddlOperator.Items.Add(">")
                ddlOperator.Items.Add("<")
                ddlOperator.Items.Add(">=")
                ddlOperator.Items.Add("<=")
            ElseIf dt.Rows(0).Item("datatype").ToString().ToUpper = "DATETIME" Then
                ddlOperator.Items.Add("=")
                ddlOperator.Items.Add(">")
                ddlOperator.Items.Add("<")
                ddlOperator.Items.Add(">=")
                ddlOperator.Items.Add("<=")
            End If

            dt.Dispose()
            con.Close()
            oda.Dispose()
            con.Dispose()
        End If
    End Sub
End Class
