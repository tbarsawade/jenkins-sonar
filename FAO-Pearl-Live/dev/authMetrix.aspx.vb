Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing

Partial Class authMetrix
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("doctype") Is Nothing Then
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FormDesc,formcaption,displayName,FieldType,DropDownType,dropdown,FieldMapping,LayoutType,isrequired,datatype,fieldid,cal_fields,autofilter  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & Session("doctype") & "' and FF.isworkFlow=1 order by displayOrder", con)
            Dim ds As New DataSet()
            oda.Fill(ds, "fields")
            Dim ob As New DynamicForm()
            ob.CreateControlsOnAuthMetrix(ds.Tables("fields"), pnlFields)
            oda.Dispose()
            ds.Dispose()
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim i As Integer
            'fill Product  
            Dim da As New SqlDataAdapter("select FormID,FormName  from MMM_MST_FORMS where EID='" & Session("EID").ToString() & "' and FormSource ='MENU DRIVEN'  and FormType='document'", con)
            Dim ds As New DataSet()
            ddlDocumentType.Items.Insert(0, "Please Select")
            da.Fill(ds, "docType")
            For i = 0 To ds.Tables("docType").Rows.Count - 1
                ddlDocumentType.Items.Add(ds.Tables("doctype").Rows(i).Item("formname"))
                ddlDocumentType.Items(i + 1).Value = ds.Tables("doctype").Rows(i).Item("formID")
            Next
            da.SelectCommand.CommandText = "select * from mmm_mst_role where eid='" & Session("EID") & "'"  '"and roletype='Pre Type' "
            da.Fill(ds, "role")

            ddlSelectrole.Items.Clear()

            ddlSelectRole.Items.Insert(0, "Please Select")
            ddlSelectRole.Items.Insert(1, "#SELF")
            ddlSelectRole.Items.Insert(2, "#SUPERVISOR")
            ddlSelectRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlSelectRole.Items.Insert(4, "#USER")
            ddlSelectRole.Items.Insert(5, "#CURRENTUSER")

            ' ddlRole.Items.Insert(0, "Please Select")
            ' ddlRole.Items.Insert(1, "#SELF")
            ' ddlRole.Items.Insert(2, "#SUPERVISOR")
            ' ddlRole.Items.Insert(3, "#LAST SUPERVISOR")
            ' ddlRole.Items.Insert(4, "#USER")

            For i = 0 To ds.Tables("role").Rows.Count - 1
                ddlSelectRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlSelectRole.Items(i + 6).Value = "ROLE" 'ds.Tables("role").Rows(i).Item("roleid")
                '    ddlRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                '    ddlRole.Items(i + 5).Value = "ROLE" ' ds.Tables("role").Rows(i).Item("roleID")
            Next

            '' for adding users as was in old format
            da.SelectCommand.CommandText = "select uid, username from MMM_MST_USER where eid='" & Session("EID") & "' order by username "
            da.Fill(ds, "user")
            For k As Integer = 0 To ds.Tables("user").Rows.Count - 1
                If ds.Tables("user").Rows(k).Item("username").ToString <> "" Then
                    ddlSelectRole.Items.Add(ds.Tables("user").Rows(k).Item("username").ToString)
                    ddlSelectRole.Items(k + 1).Value = ds.Tables("user").Rows(k).Item("uid")
                End If
            Next

            da.Dispose()
            con.Dispose()
        End If
    End Sub

    Protected Sub bindgrid()
        Dim str As String = ""

        If UCase(ddlSeleDocNature.SelectedItem.Text) <> "SELECT" Then
            str &= " and docnature= '" & ddlSeleDocNature.SelectedItem.Text & "'"
        End If

        If ddlSelectType.SelectedItem.Text = "ROLE" Then
            If UCase(ddlSelectRole.SelectedItem.Text) <> "PLEASE SELECT" Then
                str = " and ROLENAME='" & ddlSelectRole.SelectedItem.Text & "'"
            End If
        ElseIf ddlSelectType.SelectedItem.Text = "USER" Then
            If UCase(ddlSelectRole.SelectedItem.Text) <> "PLEASE SELECT" Then
                str = " and d.uid=" & ddlSelectRole.SelectedValue & ""
            End If
        End If

        If UCase(ddlStatus.SelectedItem.Text) <> "PLEASE SELECT" Then
            str &= " and aprStatus= '" & ddlStatus.SelectedItem.Text & "'"
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetAuthMetrixDoc", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("documenttype", ddlDocumentType.SelectedItem.Text)
        oda.SelectCommand.Parameters.AddWithValue("sField", str)
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())

        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds
        gvData.DataBind()
        If ds.Tables("data").Rows.Count > 0 Then
            lblRecord.Text = ""
            gvData.Columns(0).Visible = False
            gvData.HeaderRow.Cells(2).Visible = False
            For Each gvr As GridViewRow In gvData.Rows
                gvr.Cells(2).Visible = False
            Next
        Else
            lblRecord.Text = "No record has been found"
        End If
        oda.Dispose()
        con.Close()
        con.Dispose()
        '        updPnlGrid.Update()
    End Sub

    Protected Sub ADD(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlType.SelectedIndex = ddlType.Items.IndexOf(ddlType.Items.FindByText("SELECT"))
        ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByText("Please Select"))
        ddlstatus1.SelectedIndex = ddlstatus1.Items.IndexOf(ddlstatus1.Items.FindByText("Please Select"))
        ddlDocNature.SelectedIndex = ddlDocNature.Items.IndexOf(ddlDocNature.Items.FindByText("SELECT"))

        ddlFieldName.Items.Clear()
        txtLevel.Text = ""
        txtOrdering.Text = ""
        txtsla.Text = ""

        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)
        btnActEdit.Text = "Save"
        updatePanelEdit.Update()
        btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("tid") = pid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dtM As New DataTable

        oda.SelectCommand.CommandText = "Select * from MMM_MST_AuthMetrix WHERE TID=" & pid
        oda.Fill(dtM)

        '' you here for resume

        ddlDocNature.SelectedIndex = ddlDocNature.Items.IndexOf(ddlDocNature.Items.FindByText(dtM.Rows(0).Item("docnature").ToString))

        If dtM.Rows(0).Item("type").ToString = "ROLE" Then
            ddlType.SelectedIndex = ddlType.Items.IndexOf(ddlType.Items.FindByText("ROLE"))
        ElseIf dtM.Rows(0).Item("type").ToString = "USER" Then
            ddlType.SelectedIndex = ddlType.Items.IndexOf(ddlType.Items.FindByText("USER"))
        ElseIf dtM.Rows(0).Item("type").ToString = "NEWROLE" Then
            ddlType.SelectedIndex = ddlType.Items.IndexOf(ddlType.Items.FindByText("NEWROLE"))
            ViewState("NewRole") = 1
        End If

        Call FillRolenUsers(dtM.Rows(0).Item("type").ToString)

        If (dtM.Rows(0).Item("type").ToString = "ROLE") Or (dtM.Rows(0).Item("type").ToString = "NEWROLE") Then
            ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByText(dtM.Rows(0).Item("rolename").ToString()))
            If dtM.Rows(0).Item("rolename").ToString() = "#USER" Or dtM.Rows(0).Item("rolename").ToString() = "#SUPERVISOR" Or dtM.Rows(0).Item("rolename").ToString() = "#LAST SUPERVISOR" Then
                Call ShowHideFieldDdl(dtM.Rows(0).Item("rolename").ToString())
                ddlFieldName.SelectedIndex = ddlFieldName.Items.IndexOf(ddlFieldName.Items.FindByValue(dtM.Rows(0).Item("fieldname").ToString()))
                ddlFieldName.Enabled = True
                lblfldName.Enabled = True
                If dtM.Rows(0).Item("rolename").ToString() = "#LAST SUPERVISOR" Then
                    txtLevel.Enabled = True
                    lblLevel.Enabled = True
                Else
                    txtLevel.Enabled = False
                    lblLevel.Enabled = False
                End If
            Else
                ddlFieldName.Enabled = False
                lblfldName.Enabled = False
                txtLevel.Enabled = False
                lblLevel.Enabled = False
            End If
        ElseIf dtM.Rows(0).Item("type").ToString = "USER" Then
            ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByValue(dtM.Rows(0).Item("rolename").ToString()))
            ddlFieldName.Enabled = False
            lblfldName.Enabled = False
            txtLevel.Enabled = False
            lblLevel.Enabled = False
        End If


        ddlstatus1.SelectedIndex = ddlstatus1.Items.IndexOf(ddlstatus1.Items.FindByText(dtM.Rows(0).Item("aprstatus").ToString()))
        txtsla.Text = dtM.Rows(0).Item("sla").ToString()
        txtOrdering.Text = dtM.Rows(0).Item("ordering").ToString()
        Dim ob As New DynamicForm
        If dtM.Rows(0).Item("type").ToString = "USER" Then
            oda.SelectCommand.CommandText = "select FF.* from MMM_MST_FIELDS FF inner join MMM_MST_AUTHMETRIX M on FF.DocumentType =M.DocType   where FF.EID=" & Session("EID") & " AND M.tid =" & pid & " and FF.isactive=1 and FF.isworkflow=1 order by displayOrder"
            oda.Fill(ds, "fields")
            If ds.Tables("fields").Rows.Count > 0 Then
                ob.FillControlsOnAuthMatrix(ds.Tables("fields"), pnlFields, pnluser, "MASTER", pid)
            End If
        ElseIf dtM.Rows(0).Item("type").ToString = "NEWROLE" Then
            oda.SelectCommand.CommandText = "select FF.* from MMM_MST_FIELDS FF inner join MMM_MST_AUTHMETRIX M on FF.DocumentType =M.DocType   where FF.EID=" & Session("EID") & " AND M.tid =" & pid & " and FF.isactive=1 and FF.isworkflow=1 order by displayOrder"
            oda.Fill(ds, "fields")
            Dim field As String = ""
            If ds.Tables("fields").Rows.Count > 0 Then
                For k As Integer = 0 To ds.Tables("fields").Rows.Count - 1
                    field = field & ds.Tables("fields").Rows(k).Item("fieldmapping").ToString() & "='',"
                Next
                ViewState("fieldmapping") = Left(field, field.Length - 1)
                ob.FillControlsOnAuthMatrix(ds.Tables("fields"), pnlFields, pnluser, "MASTER", pid)
            End If
            ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByText(dtM.Rows(0).Item("rolename").ToString()))
        End If

        oda.Dispose()
        con.Dispose()
        btnActEdit.Text = "Update"
        updatePanelEdit.Update()
        btnEdit_ModalPopupExtender.Show()
        lblTab.Text = ""
    End Sub






    '' removed because taken to dynamicform class... by sunil pareek on 27-Aug-13
    'Public Sub FillControlsOnAuthMatrix(ByVal ds As DataTable, ByRef pnlFields As Panel, ByRef pnlUser As Panel, ByVal type As String, ByVal pid As Integer)
    '    If ds.Rows.Count > 0 Then
    '        Dim strcol As String = ""
    '        Dim strqry As String = ""
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As SqlConnection = New SqlConnection(conStr)
    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '        Dim dss As New DataSet
    '        For Each rw As DataRow In ds.Rows
    '            strcol &= rw.Item("fieldmapping").ToString & ","
    '        Next
    '        strcol = strcol.Substring(0, strcol.Length - 1)
    '        If UCase(type) = "MASTER" Then
    '            strqry = "Select  " & strcol & ",uid,doctype,aprstatus,sla from MMM_MST_AuthMetrix WHERE TID=" & pid & ""
    '        End If

    '        oda.SelectCommand.CommandText = strqry
    '        oda.Fill(dss, "data")
    '        Dim ddluser As New DropDownList
    '        ddluser = CType(pnlUser.FindControl("ddlrole"), DropDownList)
    '        ddluser.SelectedIndex = ddluser.Items.IndexOf(ddluser.Items.FindByValue(dss.Tables("data").Rows(0).Item("uid").ToString()))
    '        Dim ddlStatus As DropDownList
    '        ddlStatus = CType(pnlUser.FindControl("ddlStatus1"), DropDownList)
    '        ddlStatus.SelectedIndex = ddlStatus.Items.IndexOf(ddlStatus.Items.FindByText(dss.Tables("data").Rows(0).Item("aprstatus").ToString()))
    '        Dim txtsla As New TextBox
    '        txtsla = CType(pnlUser.FindControl("txtsla"), TextBox)
    '        txtsla.Text = dss.Tables("data").Rows(0).Item("sla").ToString()
    '        txtOrdering = CType(pnlUser.FindControl("txtOrdering"), TextBox)
    '        txtOrdering.Text = dss.Tables("data").Rows(0).Item("ordering").ToString()
    '        For i As Integer = 0 To ds.Rows.Count - 1
    '            Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
    '                Case "TEXT BOX"
    '                    Dim txtBox As New TextBox
    '                    txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
    '                    txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
    '                    If ds.Rows(i).Item("isEditable").ToString() = "0" Then
    '                        txtBox.Enabled = False
    '                    End If

    '                Case "DROP DOWN"
    '                    Dim ddl As New ListBox
    '                    ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
    '                    Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
    '                    If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    Else
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    End If
    '                Case "CHECKBOX LIST"

    '                    Dim chklist As New CheckBoxList
    '                    chklist = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), CheckBoxList)
    '                    Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
    '                    If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
    '                        For ii As Integer = 0 To ARR.Length - 1
    '                            'ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(ARR(ii).ToString()))
    '                            chklist.Items.FindByText(ARR(ii).ToString()).Selected = True
    '                        Next
    '                    Else
    '                        For ii As Integer = 0 To ARR.Length - 1
    '                            chklist.Items.FindByValue(ARR(ii).ToString()).Selected = True
    '                        Next
    '                    End If
    '                Case "LIST BOX"
    '                    Dim ddl As New ListBox
    '                    ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
    '                    Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
    '                    If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    Else
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    End If
    '                Case "TEXT AREA"
    '                    Dim txtBox As New TextBox
    '                    txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
    '                    txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
    '            End Select
    '        Next
    '        oda.Dispose()
    '        dss.Dispose()
    '    End If
    '    ds.Dispose()
    'End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("DELETE FROM MMM_MST_AUTHMETRIX WHERE TID=" & ViewState("Did").ToString(), con)
        oda.SelectCommand.CommandType = CommandType.Text
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        oda.Dispose()
        con.Close()
        con.Dispose()
        bindgrid()
        btnDelete_ModalPopupExtender.Hide()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblRecord.Visible = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Auth Matrix Record? " & row.Cells(1).Text
        btnActDelete.Visible = True
        updatePanelDelete.Update()
        btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        If ddlDocumentType.SelectedItem.Text = "Please Select" Then
            lblRecord.Text = "Please select 'Document type' "
            lblRecord.Visible = True
            Exit Sub
        End If
        bindgrid()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click

        If UCase(ddlDocNature.SelectedItem.Text) = "SELECT" Then
            lblTab.Text = "Please select Document Nature"
            Exit Sub
        End If

        If UCase(ddlRole.SelectedItem.Text) = "PLEASE SELECT" Then
            lblTab.Text = "Please select Role/user Name"
            Exit Sub
        End If

        If ddlRole.SelectedItem.Text = "#USER" Or ddlRole.SelectedItem.Text = "#SUPERVISOR" Or ddlRole.SelectedItem.Text = "#LAST SUPERVISOR" Then
            If UCase(ddlFieldName.SelectedItem.Text) = "PLEASE SELECT" Then
                lblTab.Text = "Please select field name"
                Exit Sub
            End If
        End If

        If UCase(ddlstatus1.SelectedItem.Text) = "PLEASE SELECT" Then
            lblTab.Text = "Please select Status Name"
            Exit Sub
        End If


        If IsNumeric(txtOrdering.Text) = False Then
            lblTab.Text = "Only numeric values allowed in ordering"
            Exit Sub
        End If

        If Val(txtOrdering.Text) = 0 Then
            lblTab.Text = "Ordering value should be >= 1"
            Exit Sub
        End If

        If IsNumeric(txtsla.Text) = False Then
            lblTab.Text = "Only numeric values allowed in SLA"
            Exit Sub
        End If


        If UCase(btnActEdit.Text) = "SAVE" Then
            ValidateData("ADD")
        ElseIf UCase(btnActEdit.Text) = "UPDATE" Then
            ValidateData("UPDATE")
        End If
        bindgrid()
    End Sub

    Protected Sub ValidateData(ByVal actionType As String)
        'Check All Validations
        ' now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim screenname As String = Session("doctype").ToString()
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        If actionType = "UPDATE" Then
            da.SelectCommand.CommandText = "select * from mmm_mst_authmetrix where EID=" & Session("EID").ToString() & " and Doctype='" & screenname & "' and docnature='" & ddlDocNature.SelectedItem.Text & "' and aprstatus='" & ddlstatus1.SelectedItem.Text & "' and ordering=" & txtOrdering.Text & " and tid<>" & ViewState("tid")
        ElseIf actionType = "ADD" Then
            da.SelectCommand.CommandText = "select * from mmm_mst_authmetrix where EID=" & Session("EID").ToString() & " and Doctype='" & screenname & "' and docnature='" & ddlDocNature.SelectedItem.Text & "' and aprstatus='" & ddlstatus1.SelectedItem.Text & "' and ordering=" & txtOrdering.Text
        End If

        Dim dtchk As New DataTable
        da.Fill(dtchk)
        If dtchk.Rows.Count > 0 Then
            lblTab.Text = "Duplicate Status Name and Ordering value in current Doc Type- " & screenname
            dtchk.Dispose()
            da.Dispose()
            con.Dispose()
            Exit Sub
        End If

        Dim fldName As String = ""
        Dim level As Integer = 0

        If ddlRole.SelectedItem.Text = "#SELF" Then
            fldName = "OUID"
        ElseIf ddlRole.SelectedItem.Text = "#USER" Or ddlRole.SelectedItem.Text = "#SUPERVISOR" Or ddlRole.SelectedItem.Text = "#LAST SUPERVISOR" Then
            fldName = ddlFieldName.SelectedItem.Value
        End If

        If ddlRole.SelectedItem.Text = "#LAST SUPERVISOR" Then
            level = Val(txtLevel.Text)
        End If

        Dim FinalQry As String = ""
        Dim ob As New DynamicForm

        If ddlType.SelectedItem.Text = "USER" Then
            da.SelectCommand.CommandText = "SELECT * FROM MMM_MST_FIELDS WHERE Isactive=1 and  EID=" & Session("EID").ToString() & " and Documenttype='" & screenname & "' and isworkflow=1 order by displayOrder"
            Dim ds As New DataSet
            da.Fill(ds, "fields")

            If actionType = "ADD" Then
                FinalQry = ob.ValidateAndGenrateQueryForAUTHMATRIX("ADD", "INSERT INTO MMM_MST_AuthMetrix(EID,Doctype,uid,aprstatus,sla,ordering,type,rolename,fieldname,docNature,", "VALUES (" & Session("EID").ToString() & ",'" & Session("doctype").ToString() & "'," & ddlRole.SelectedValue & ",'" & ddlstatus1.SelectedItem.Text & "'," & txtsla.Text & "," & txtOrdering.Text & ",'" & ddlType.SelectedItem.Text & "','" & ddlRole.SelectedItem.Value & "','" & fldName & "','" & ddlDocNature.SelectedItem.Value & "',", ds.Tables("fields"), pnlFields)
            Else
                FinalQry = ob.ValidateAndGenrateQueryForAUTHMATRIX("UPDATE", "UPDATE MMM_MST_AuthMetrix SET uid=" & ddlRole.SelectedValue & ",aprstatus='" & ddlstatus1.SelectedItem.Text & "',sla=" & txtsla.Text & ",Ordering=" & txtOrdering.Text & ", type='" & ddlType.SelectedItem.Text & "', rolename='" & ddlRole.SelectedItem.Value & "', docnature='" & ddlDocNature.SelectedItem.Value & "',", "", ds.Tables("fields"), pnlFields)
            End If
        ElseIf ddlType.SelectedItem.Text = "ROLE" Then
            If actionType = "ADD" Then
                FinalQry = "INSERT INTO MMM_MST_AuthMetrix(EID,Doctype,rolename,type,aprstatus,sla,fieldname,ordering,sLevel,docNature) VALUES (" & Session("EID").ToString() & ",'" & Session("doctype").ToString() & "','" & ddlRole.SelectedItem.Text & "','" & ddlType.SelectedItem.Text & "','" & ddlstatus1.SelectedItem.Text & "'," & txtsla.Text & ",'" & fldName & "'," & txtOrdering.Text & "," & level & ",'" & ddlDocNature.SelectedItem.Text & "')"
            Else
                ' added by balli 
                If ViewState("NewRole") = 1 Then
                    FinalQry = "UPDATE MMM_MST_AuthMetrix SET rolename='" & ddlRole.SelectedItem.Text & "',uid=null,type='" & ddlType.SelectedItem.Text & "',aprstatus='" & ddlstatus1.SelectedItem.Text & "',sla=" & txtsla.Text & ",fieldname='" & fldName & "', ordering=" & txtOrdering.Text & ", sLevel=" & level & ", docnature='" & ddlDocNature.SelectedItem.Text & "' , " & ViewState("fieldmapping") & " "
                Else
                    FinalQry = "UPDATE MMM_MST_AuthMetrix SET rolename='" & ddlRole.SelectedItem.Text & "',uid=null,type='" & ddlType.SelectedItem.Text & "',aprstatus='" & ddlstatus1.SelectedItem.Text & "',sla=" & txtsla.Text & ",fieldname='" & fldName & "', ordering=" & txtOrdering.Text & ", sLevel=" & level & ", docnature='" & ddlDocNature.SelectedItem.Text & "'"
                End If

            End If
        ElseIf ddlType.SelectedItem.Text = "NEWROLE" Then
            da.SelectCommand.CommandText = "SELECT * FROM MMM_MST_FIELDS WHERE Isactive=1 and  EID=" & Session("EID").ToString() & " and Documenttype='" & screenname & "' and isworkflow=1 order by displayOrder"
            Dim ds As New DataSet
            da.Fill(ds, "fields")
            If actionType = "ADD" Then
                FinalQry = ob.ValidateAndGenrateQueryForAUTHMATRIX("ADD", "INSERT INTO MMM_MST_AuthMetrix(EID,sLevel,Doctype,aprstatus,sla,ordering,type,rolename,fieldname,docNature,", "VALUES (" & Session("EID").ToString() & "," & level & ",'" & Session("doctype").ToString() & "','" & ddlstatus1.SelectedItem.Text & "'," & txtsla.Text & "," & txtOrdering.Text & ",'" & ddlType.SelectedItem.Text & "','" & ddlRole.SelectedItem.Text & "','" & fldName & "','" & ddlDocNature.SelectedItem.Text & "',", ds.Tables("fields"), pnlFields)
            Else
                FinalQry = ob.ValidateAndGenrateQueryForAUTHMATRIX("UPDATE", "UPDATE MMM_MST_AuthMetrix SET aprstatus='" & ddlstatus1.SelectedItem.Text & "',sla=" & txtsla.Text & ",Ordering=" & txtOrdering.Text & ", type='" & ddlType.SelectedItem.Text & "',uid=null, rolename='" & ddlRole.SelectedItem.Text & "', docnature='" & ddlDocNature.SelectedItem.Value & "',sLevel=" & level & ",", "", ds.Tables("fields"), pnlFields)
            End If
        End If

        ViewState("fieldmapping") = Nothing
        ViewState("NewRole") = Nothing

        If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
            lblTab.Text = FinalQry
        Else
            If actionType <> "ADD" Then
                FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
            End If
            'save the data
            lblTab.Text = ""
            FinalQry = FinalQry & ""
            da.SelectCommand.CommandText = FinalQry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim tid As Integer = da.SelectCommand.ExecuteScalar()
            'da.SelectCommand.CommandText = "UPDATE MMM_MST_AUTHMETRIX SET ORDERING=" & tid & " WHERE TID =" & tid & ""
            'da.SelectCommand.ExecuteNonQuery()
            ob.CLEARDYNAMICFIELDS(pnlFields)
            gvData.DataBind()
            btnEdit_ModalPopupExtender.Hide()
        End If
        da.Dispose()
        con.Dispose()
    End Sub




    Protected Sub ddlDocumentType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDocumentType.SelectedIndexChanged
        If UCase(ddlDocumentType.SelectedItem.Text) <> "PLEASE SELECT" Then
            Session("doctype") = ddlDocumentType.SelectedItem.Text
            btnAdd.Enabled = True
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Status  
            Dim da As New SqlDataAdapter("select tid,StatusName,dord from MMM_MST_WORKFLOW_STATUS where eid='" & Session("EID") & "' and (documenttype is null or documenttype='" & ddlDocumentType.SelectedItem.Text & "') ", con)
            Dim ds As New DataSet()
            ddlStatus.Items.Clear()
            ddlstatus1.Items.Clear()
            ddlStatus.Items.Insert(0, "Please Select")
            ddlstatus1.Items.Insert(0, "Please Select")
            da.Fill(ds, "status")
            For i As Integer = 0 To ds.Tables("status").Rows.Count - 1
                ddlStatus.Items.Add(ds.Tables("status").Rows(i).Item("StatusName"))
                ddlStatus.Items(i + 1).Value = ds.Tables("status").Rows(i).Item("dord")
                ddlstatus1.Items.Add(ds.Tables("status").Rows(i).Item("StatusName"))
                ddlstatus1.Items(i + 1).Value = ds.Tables("status").Rows(i).Item("dord")
            Next
            bindgrid()
        Else
            Session("doctype") = Nothing
            btnAdd.Enabled = False
        End If
    End Sub

    Protected Sub PositionUp(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        If row.RowIndex = 0 Then
            Exit Sub
        End If
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim ntid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex - 1).Value)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("PositionUpdateAuthmatrix", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        oda.SelectCommand.Parameters.AddWithValue("ntid", ntid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()
        bindgrid()
    End Sub

    Protected Sub PositionDown(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        If row.RowIndex >= gvData.Rows.Count - 1 Then
            Exit Sub
        End If

        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim ntid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex + 1).Value)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("PositionUpdateAuthmatrix", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        oda.SelectCommand.Parameters.AddWithValue("ntid", ntid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()
        bindgrid()
    End Sub


    'Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
    '    Try
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim scrname As String = ddlDocumentType.SelectedItem.Text   'Request.QueryString("SC").ToString()
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '        Dim icnt As Integer

    '        Dim i As Integer = Session("EID")
    '        Dim ds As New DataSet

    '        'Dim row As New DataRow
    '        Dim da As New SqlDataAdapter("select * from MMM_MST_FIELDS where EID=" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND isworkflow=1 ", con)
    '        da.Fill(ds, "data")
    '        Dim colCount As Integer = ds.Tables("data").Rows.Count
    '        Dim adapter As New SqlDataAdapter
    '        Dim sb As New System.Text.StringBuilder()
    '        Dim sh As New System.Text.StringBuilder()

    '        Dim errs As String = ""
    '        If impfile.HasFile Then
    '            ViewState("imprt_cnt") += 1
    '            If Right(impfile.FileName, 4).ToUpper() = ".CSV" Then
    '                Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(impfile.FileName, 4).ToUpper()
    '                impfile.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
    '                Dim ir As Integer = 0
    '                Dim sField As String()
    '                Dim csvReader As Microsoft.VisualBasic.FileIO.TextFieldParser
    '                csvReader = My.Computer.FileSystem.OpenTextFieldParser(Server.MapPath("Import/") & filename, ",")
    '                Dim st As String = ""
    '                Dim ic As Integer = 0
    '                Dim vk As String = ""
    '                Dim ddty As String = ""
    '                Dim c1 As String = ""
    '                Dim mv As String = ""
    '                Dim dn As String = ""
    '                Dim dd As String = ""
    '                Dim sFieldTop As String()
    '                With csvReader
    '                    .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
    '                    .Delimiters = New String() {","}
    '                    While Not .EndOfData
    '                        sField = .ReadFields()

    '                        If icnt < 1 Then
    '                            If UCase(sField(0)) <> "APRSTATUS" Then
    '                                lblMsg.Text = "APRSTATUS not found in file."
    '                                Exit Sub
    '                            End If
    '                            If UCase(sField(1)) <> "DISPLAY ORDER" Then
    '                                lblMsg.Text = "DISPLAY ORDER not found in file."
    '                                Exit Sub
    '                            End If
    '                            If UCase(sField(2)) <> "SLA" Then
    '                                lblMsg.Text = "SLA not found in file."
    '                                Exit Sub
    '                            End If
    '                            If UCase(sField(3)) <> "UID" Then
    '                                lblMsg.Text = "UID not found in file."
    '                                Exit Sub
    '                            End If

    '                            sFieldTop = sField

    '                            Dim FldNames(sField.Length) As String
    '                            FldNames(0) = "APRSTATUS"
    '                            FldNames(1) = "DISPLAY ORDER"
    '                            FldNames(2) = "SLA"
    '                            FldNames(3) = "UID"

    '                            sb.Append("Insert Into MMM_MST_AuthMetrix (eid,doctype,aprstatus,ordering,sla,uid,")
    '                            For k As Integer = 4 To sField.Length - 1

    '                                FldNames(k) = sField(k).ToString

    '                                da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and DocumentType ='" & scrname & "' and displayname='" & sField(k) & "' and isworkflow=1"
    '                                If con.State <> ConnectionState.Open Then
    '                                    con.Open()
    '                                End If
    '                                Dim dtF As New DataTable
    '                                da.Fill(dtF)

    '                                If dtF.Rows.Count = 0 Then
    '                                    lblMsg.Text = "'" & sField(k) & "' Not Found or workflow flag not set"
    '                                    Exit Sub
    '                                Else
    '                                    st = dtF.Rows(0).Item("FieldMapping").ToString()
    '                                    sb.Append(st)
    '                                    If k = sField.Length - 1 Then
    '                                        sb.Append(") values (")
    '                                        Exit For
    '                                    Else
    '                                        sb.Append(", ")
    '                                    End If
    '                                End If
    '                            Next
    '                            icnt += 1
    '                            Continue While
    '                        End If
    '                        ' {Insert Into MMM_MST_AuthMetrix (eid,doctype,aprstatus,ordering,sla,uidBudget Description, State)
    '                        '                          values (32,'Modify Budget','Created',1,1,1054,1054,'4203','West Bengal')}
    '                        icnt += 1
    '                        Dim v As String = ""

    '                        v &= Session("EID")
    '                        v &= ","
    '                        v &= "'" & scrname & "'"

    '                        '' for checking existance of aprstatus / workflow status
    '                        If Trim(sField(0)) = "" Then
    '                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Blank AprStatus not allowed " & sField(0) & ")" & " </tr> </table>"
    '                            Continue While
    '                        End If

    '                        da.SelectCommand.CommandText = "select count(tid) from MMM_MST_WORKFLOW_STATUS where eid=" & Session("EID") & " and statusname='" & sField(0) & "' and documenttype='" & scrname & "'"
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Dim scnt As Integer = da.SelectCommand.ExecuteScalar()
    '                        If scnt < 1 Then
    '                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- " & sField(0) & " not exists)" & "</td>" & " </tr> </table>"
    '                            Continue While
    '                        Else
    '                            v &= ",'" & Trim(sField(0)) & "'"
    '                        End If


    '                        If Trim(sField(1)) = "" Or IsNumeric(Trim(sField(1))) = False Then
    '                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid display order " & sField(1) & ")" & " </tr> </table>"
    '                            Continue While
    '                        Else
    '                            v &= "," & Trim(sField(1))
    '                        End If


    '                        If Trim(sField(2)) = "" Or IsNumeric(Trim(sField(2))) = False Then
    '                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid SLA " & sField(2) & ")" & " </tr> </table>"
    '                            Continue While
    '                        Else
    '                            v &= "," & Trim(sField(2))
    '                        End If

    '                        '' for checking duplicacy of user ID's 
    '                        If IsNumeric(Trim(sField(3))) = False Or Trim(sField(3)) = "" Then
    '                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid UID is entered  " & sField(3) & ")" & " </tr> </table>"
    '                            Continue While
    '                        End If

    '                        da.SelectCommand.CommandText = "select count(uid) from mmm_mst_user where eid=" & Session("EID") & " and uid=" & sField(3) & ""
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Dim ucnt As Integer = da.SelectCommand.ExecuteScalar()
    '                        If ucnt < 1 Then
    '                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- " & sField(3) & " not exists)" & "</td>" & " </tr> </table>"
    '                            Continue While
    '                        Else
    '                            v &= ","
    '                            v &= Trim(sField(3))
    '                        End If

    '                        v &= ","

    '                        For j As Integer = 4 To sField.Length - 1
    '                            da.SelectCommand.CommandText = "select top 1 * from mmm_mst_fields where eid=" & Session("EID") & " and DocumentType ='" & scrname & "' and displayname='" & sFieldTop(j) & "' and isworkflow=1"
    '                            If con.State <> ConnectionState.Open Then
    '                                con.Open()
    '                            End If

    '                            Dim dtR As New DataTable
    '                            da.Fill(dtR)

    '                            mv = dtR.Rows(0).Item("dropdowntype").ToString()
    '                            dn = dtR.Rows(0).Item("displayname").ToString()
    '                            dd = dtR.Rows(0).Item("dropdown").ToString()
    '                            vk = dtR.Rows(0).Item("isrequired").ToString()
    '                            ddty = dtR.Rows(0).Item("datatype").ToString()

    '                            '' you are here on last night at 10:05 pm

    '                            If UCase(mv) = UCase("Master Valued") Or UCase(mv) = UCase("Session Valued") Then
    '                                If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values should be entered  " & sField(j) & ")" & " </tr> </table>"
    '                                    Continue While
    '                                ElseIf vk = 1 And Trim(sField(j)) = "" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Empty column)" & " </tr> </table>"
    '                                    Continue While
    '                                End If
    '                            Else
    '                                If vk = 1 And Trim(sField(j)) = "" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                ElseIf ddty = "Numeric" Then
    '                                    If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values should be entered " & sField(j) & ")" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And IsNumeric(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values should be entered " & sField(j) & ")" & " </tr> </table>"
    '                                        Continue While
    '                                    End If
    '                                ElseIf ddty = "Datetime" Then
    '                                    If vk = 1 And IsDate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And IsDate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    End If
    '                                End If
    '                            End If
    '                            If UCase(mv) = UCase("Master Valued") Then
    '                                Dim b1 As String() = dd.ToString().Split("-")
    '                                ViewState("dd1") = b1(1).ToString()
    '                                da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & ViewState("dd1") & "' and tid=" & sField(j) & ""
    '                                If con.State <> ConnectionState.Open Then
    '                                    con.Open()
    '                                End If
    '                                Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
    '                                If cntt < 1 Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- " & sField(j) & " not exists)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                Else
    '                                    v &= "'"
    '                                    v &= Trim(sField(j))
    '                                End If
    '                            ElseIf UCase(mv) = UCase("Session Valued") Then
    '                                Dim b1 As String() = dd.ToString().Split("-")
    '                                ViewState("dd1") = b1(1).ToString()
    '                                da.SelectCommand.CommandText = "select count(uid) from mmm_mst_user where eid=" & Session("EID") & " and uid=" & sField(j) & ""
    '                                If con.State <> ConnectionState.Open Then
    '                                    con.Open()
    '                                End If
    '                                Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
    '                                If cntt < 1 Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- " & sField(j) & " not exists)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                Else
    '                                    v &= "'"
    '                                    v &= Trim(sField(j))
    '                                End If
    '                            Else
    '                                v &= "'" 'sb.Append("'")
    '                                v &= Trim(sField(j))
    '                            End If

    '                            v &= "'"
    '                            If j = sField.Length - 1 Then
    '                                v &= ")"   ' sb.Append(")")
    '                                Exit For
    '                            Else
    '                                v &= "," 'sb.Append(", ")
    '                            End If
    '                            dtR.Clear()
    '                            dtR.Dispose()
    '                        Next
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Replace(sb.ToString(), "{", "")
    '                        Replace(sb.ToString(), "}", "")
    '                        sh.Append(sb)
    '                        sh.Append(v)
    '                        adapter.InsertCommand = New SqlCommand(sh.ToString(), con)
    '                        adapter.InsertCommand.ExecuteNonQuery()
    '                        ic += 1
    '                        con.Close()
    '                        adapter.Dispose()
    '                        sh.Clear()

    '                    End While
    '                    gvData.DataBind()
    '                    ' gvexport.DataBind()
    '                    updPnlGrid.UpdateMode = UpdatePanelUpdateMode.Conditional
    '                    updPnlGrid.Update()
    '                    modalstatus.Show()
    '                    lblstat.Text = "Out of <font color=""Green"">" & icnt - 1 & "</font>, <font color=""Green""> " & ic & " </font> Successfully Imported  "
    '                    ViewState("c1") = c1
    '                    If ViewState("c1") = "" Then
    '                        lblstatus1.Text = ""
    '                    Else
    '                        lblstatus.Text = "Data which are not uploaded due to Errors are given below: "
    '                        lblstatus1.Text = "" & c1 & " "
    '                        lblstatus1.ForeColor = Color.Red
    '                    End If
    '                End With
    '            Else
    '                lblMsg.Text = "File should be of CSV Format"
    '                Exit Sub
    '            End If
    '        Else
    '            lblMsg.Text = "Please select a File to Upload"
    '            Exit Sub
    '        End If

    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured while importing data. Please try again"
    '    End Try
    'End Sub

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        ' Verifies that the control is rendered
    End Sub

    'Protected Sub btnimpAM_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimpAM.Click
    '    'modalpopupimport.Show()
    'End Sub

    Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable
        Dim returnTable As New DataTable()
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

    'Protected Sub helpexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles helpexport.Click
    '    Try
    '        Response.Clear()
    '        Response.Buffer = True


    '        Response.Charset = ""
    '        Response.ContentType = "application/vnd.ms-excel"
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product
    '        Dim ds As New DataSet
    '        Dim scrname As String = ddlDocumentType.SelectedItem.Text   ' Request.QueryString("SC").ToString()
    '        Response.AddHeader("content-disposition", "attachment;filename=" & scrname & ".xls")

    '        Dim da As New SqlDataAdapter("select  'aprstatus' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Text' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all select  'DISPLAY ORDER' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Numeric' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all select  'SLA' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Numeric' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all select  'UID' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Numeric' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all SELECT  displayName [DisplayName],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields], case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (MM/DD/YYYY)' end [Data Type], case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length]  FROM MMM_MST_FIELDS  where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 and isworkflow=1 ", con)
    '        Dim query As String = "select  'aprstatus' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Text' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all select  'DISPLAY ORDER' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Numeric' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all select  'SLA' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Numeric' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all select  'UID' [DISPLAYNAME], 'Yes' [Mandatory Fields] , 'Numeric' [Data Type], 0 [Minimum Length], 0 [Maximum Length] union all SELECT  displayName [DisplayName],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields], case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (MM/DD/YYYY)' end [Data Type], case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length]  FROM MMM_MST_FIELDS  where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1  and isworkflow=1"
    '        Dim cmd As SqlCommand = New SqlCommand(query, con)
    '        con.Open()
    '        Dim dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
    '        Dim dt As DataTable = New DataTable()
    '        Dim dt2 As DataTable = New DataTable()
    '        Dim dt3 As DataTable = New DataTable()

    '        dt.Load(dr)
    '        da.Fill(ds, "data")
    '        dt3 = ds.Tables("data")
    '        dt2 = GetInversedDataTable(dt, "displayname", "")

    '        Dim gvex As New GridView()
    '        dt2.Rows.Add()

    '        gvex.AllowPaging = False
    '        gvex.DataSource = dt2
    '        gvex.DataBind()
    '        Dim gvexx As New GridView()
    '        gvexx.AllowPaging = False
    '        gvexx.DataSource = dt3

    '        gvexx.DataBind()
    '        Response.Clear()
    '        Response.Buffer = True
    '        Dim sw As New StringWriter()
    '        Dim hw As New HtmlTextWriter(sw)

    '        Dim tb As New Table()
    '        Dim tr1 As New TableRow()
    '        Dim cell1 As New TableCell()
    '        cell1.Controls.Add(gvex)
    '        tr1.Cells.Add(cell1)
    '        Dim cell3 As New TableCell()
    '        cell3.Controls.Add(gvexx)
    '        Dim cell2 As New TableCell()
    '        cell2.Text = "&nbsp;"

    '        Dim tr2 As New TableRow()
    '        tr2.Cells.Add(cell2)
    '        Dim tr3 As New TableRow()
    '        tr3.Cells.Add(cell3)
    '        tb.Rows.Add(tr1)
    '        tb.Rows.Add(tr2)
    '        tb.Rows.Add(tr3)

    '        tb.RenderControl(hw)

    '        'style to format numbers to string 
    '        Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
    '        Response.Write(style)
    '        Response.Output.Write(sw.ToString())
    '        Response.Flush()
    '        Response.[End]()

    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured when Downloading data. Please try again"
    '    End Try
    'End Sub

    Protected Sub ShowHideFieldDdl(ByVal SeleRole As String)
        If SeleRole = "#USER" Or SeleRole = "#SUPERVISOR" Or SeleRole = "#LAST SUPERVISOR" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim TBLName As String
            If SeleRole = "#USER" Then
                TBLName = Session("doctype")
            Else
                TBLName = "USER"
            End If

            Dim oda As SqlDataAdapter = New SqlDataAdapter("select fieldId,displayname,fieldMapping from mmm_mst_fields where eid=" & Session("eid") & " and documenttype='" & TBLName & "'", con)
            oda.SelectCommand.CommandType = CommandType.Text
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim DT As New DataTable
            oda.Fill(DT)
            lblfldName.Enabled = True
            ddlFieldName.Enabled = True
            ddlFieldName.Items.Clear()
            ddlFieldName.Items.Insert(0, "Please Select")
            If DT.Rows.Count <> 0 Then
                For i As Integer = 0 To DT.Rows.Count - 1
                    ddlFieldName.Items.Add(DT.Rows(i).Item("displayname"))
                    ddlFieldName.Items(i + 1).Value = DT.Rows(i).Item("fieldmapping")
                Next
            End If
            lblfldName.Enabled = True
            ddlFieldName.Enabled = True

            If SeleRole = "#LAST SUPERVISOR" Then
                lblLevel.Enabled = True
                txtLevel.Enabled = True
            Else
                lblLevel.Enabled = False
                txtLevel.Enabled = False
            End If

            oda.Dispose()
            con.Dispose()
            DT.Dispose()
        Else
            lblfldName.Enabled = False
            ddlFieldName.Enabled = False
            txtLevel.Enabled = False
        End If
    End Sub

    Protected Sub ddlrole_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRole.SelectedIndexChanged
        Call ShowHideFieldDdl(ddlRole.SelectedItem.Text)
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlType.SelectedIndexChanged
        Call FillRolenUsers(ddlType.SelectedItem.Text)
    End Sub

    Protected Sub FillRolenUsers(ByVal Typ As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet()

        If Typ = "ROLE" Then
            da.SelectCommand.CommandText = "select * from mmm_mst_role where eid='" & Session("EID") & "'"  '"and roletype='Pre Type' "
            da.Fill(ds, "role")

            ddlRole.Items.Clear()
            ddlRole.Items.Insert(0, "Please Select")
            ddlRole.Items.Insert(1, "#SELF")
            ddlRole.Items.Insert(2, "#SUPERVISOR")
            ddlRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlRole.Items.Insert(4, "#USER")
            ddlRole.Items.Insert(5, "#CURRENTUSER")

            For i = 0 To ds.Tables("role").Rows.Count - 1
                ddlRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlRole.Items(i + 6).Value = ds.Tables("role").Rows(i).Item("roleID")
            Next
            pnlFields.Visible = False

        ElseIf Typ = "USER" Then
            '' for adding users as was in old format
            da.SelectCommand.CommandText = "select uid, username from MMM_MST_USER where eid='" & Session("EID") & "' and isAuth=1 order by username"
            da.Fill(ds, "user")
            ddlRole.Items.Clear()
            ddlRole.Items.Insert(0, "Please Select")
            For k As Integer = 0 To ds.Tables("user").Rows.Count - 1
                ddlRole.Items.Add(ds.Tables("user").Rows(k).Item("username"))
                ddlRole.Items(k + 1).Value = ds.Tables("user").Rows(k).Item("uid")
            Next
            pnlFields.Visible = True

        ElseIf Typ = "NEWROLE" Then
            da.SelectCommand.CommandText = "select * from mmm_mst_role where eid='" & Session("EID") & "'"  '"and roletype='Pre Type' "
            da.Fill(ds, "role")

            ddlRole.Items.Clear()
            ddlRole.Items.Insert(0, "Please Select")
            ddlRole.Items.Insert(1, "#SELF")
            ddlRole.Items.Insert(2, "#SUPERVISOR")
            ddlRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlRole.Items.Insert(4, "#USER")
            ddlRole.Items.Insert(5, "#CURRENTUSER")

            For i = 0 To ds.Tables("role").Rows.Count - 1
                ddlRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlRole.Items(i + 6).Value = ds.Tables("role").Rows(i).Item("roleID")
            Next
            pnlFields.Visible = True
        End If

        da.Dispose()
        ds.Dispose()
        con.Dispose()
    End Sub

    Protected Sub ddlSelectType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSelectType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet()

        If ddlSelectType.SelectedItem.Text = "ROLE" Then
            da.SelectCommand.CommandText = "select * from mmm_mst_role where eid='" & Session("EID") & "'"  '"and roletype='Pre Type' "
            da.Fill(ds, "role")

            ddlSelectRole.Items.Clear()
            ddlSelectRole.Items.Insert(0, "Please Select")
            ddlSelectRole.Items.Insert(1, "#SELF")
            ddlSelectRole.Items.Insert(2, "#SUPERVISOR")
            ddlSelectRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlSelectRole.Items.Insert(4, "#USER")
            ddlSelectRole.Items.Insert(5, "#CURRENTUSER")

            For i = 0 To ds.Tables("role").Rows.Count - 1
                ddlSelectRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlSelectRole.Items(i + 6).Value = ds.Tables("role").Rows(i).Item("roleID")
            Next

        ElseIf ddlSelectType.SelectedItem.Text = "USER" Then
            '' for adding users as was in old format
            da.SelectCommand.CommandText = "select uid, username from MMM_MST_USER where eid='" & Session("EID") & "' "
            da.Fill(ds, "user")
            ddlSelectRole.Items.Clear()
            ddlSelectRole.Items.Insert(0, "Please Select")
            For k As Integer = 0 To ds.Tables("user").Rows.Count - 1
                ddlSelectRole.Items.Add(ds.Tables("user").Rows(k).Item("username"))
                ddlSelectRole.Items(k + 1).Value = ds.Tables("user").Rows(k).Item("uid")
            Next
        ElseIf ddlSelectType.SelectedItem.Text = "NEWROLE" Then
            da.SelectCommand.CommandText = "select * from mmm_mst_role where eid='" & Session("EID") & "'"  '"and roletype='Pre Type' "
            da.Fill(ds, "role")

            ddlSelectRole.Items.Clear()
            ddlSelectRole.Items.Insert(0, "Please Select")
            ddlSelectRole.Items.Insert(1, "#SELF")
            ddlSelectRole.Items.Insert(2, "#SUPERVISOR")
            ddlSelectRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlSelectRole.Items.Insert(4, "#USER")
            ddlSelectRole.Items.Insert(5, "#CURRENTUSER")

            For i = 0 To ds.Tables("role").Rows.Count - 1
                ddlSelectRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlSelectRole.Items(i + 6).Value = ds.Tables("role").Rows(i).Item("roleID")
            Next

        End If


        da.Dispose()
        ds.Dispose()
        con.Dispose()
    End Sub

    Protected Sub ddlstatus1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlstatus1.SelectedIndexChanged
        txtOrdering.Text = ddlstatus1.SelectedItem.Value
    End Sub
End Class
