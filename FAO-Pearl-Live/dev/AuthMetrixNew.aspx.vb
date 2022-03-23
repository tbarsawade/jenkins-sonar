Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing

Partial Class authMetrixNew
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("doctype") Is Nothing Then
        Else
            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As SqlConnection = New SqlConnection(conStr)
            'Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FormDesc,formcaption,displayName,FieldType,DropDownType,dropdown,FieldMapping,LayoutType,isrequired,datatype,fieldid,cal_fields,autofilter  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & Session("doctype") & "' and FF.isworkFlow=1 order by displayOrder", con)
            'Dim ds As New DataSet()
            'oda.Fill(ds, "fields")
            'Dim ob As New DynamicForm()
            'ob.CreateControlsOnAuthMetrix(ds.Tables("fields"), pnlFields)
            'oda.Dispose()
            'ds.Dispose()
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
            'fill Product  
            Dim da As New SqlDataAdapter("select FormID,FormName  from MMM_MST_FORMS where EID='" & Session("EID").ToString() & "' and FormSource ='MENU DRIVEN'  and FormType='document'", con)
            Dim ds As New DataSet()
            ddlDocumentType.Items.Insert(0, "Please Select")
            da.Fill(ds, "docType")
            For i As Integer = 0 To ds.Tables("docType").Rows.Count - 1
                ddlDocumentType.Items.Add(ds.Tables("doctype").Rows(i).Item("formname"))
                ddlDocumentType.Items(i + 1).Value = ds.Tables("doctype").Rows(i).Item("formID")
            Next
            da.SelectCommand.CommandText = "select * from mmm_mst_role where eid='" & Session("EID") & "'"  '"and roletype='Pre Type' "
            ddlSelectRole.Items.Clear()
            ddlSelectRole.Items.Insert(0, "Please Select")
            ddlSelectRole.Items.Insert(1, "#SELF")
            ddlSelectRole.Items.Insert(2, "#SUPERVISOR")
            ddlSelectRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlSelectRole.Items.Insert(4, "#USER")

            ddlRole.Items.Insert(0, "Please Select")
            ddlRole.Items.Insert(1, "#SELF")
            ddlRole.Items.Insert(2, "#SUPERVISOR")
            ddlRole.Items.Insert(3, "#LAST SUPERVISOR")
            ddlRole.Items.Insert(4, "#USER")

            da.Fill(ds, "role")
            For i As Integer = 0 To ds.Tables("role").Rows.Count - 1
                ddlSelectRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlSelectRole.Items(i + 5).Value = ds.Tables("role").Rows(i).Item("roleid")
                ddlRole.Items.Add(ds.Tables("role").Rows(i).Item("RoleName"))
                ddlRole.Items(i + 5).Value = ds.Tables("role").Rows(i).Item("roleID")
            Next
            da.Dispose()
            con.Dispose()
        End If
    End Sub

    Protected Sub bindgrid()
        Dim str As String = ""
        If UCase(ddlSelectRole.SelectedItem.Text) <> "PLEASE SELECT" Then
            str = " and rolename='" & ddlSelectRole.SelectedItem.Text & "'"
        End If

        If UCase(ddlStatus.SelectedItem.Text) <> "PLEASE SELECT" Then
            str &= " and aprStatus= '" & ddlStatus.SelectedItem.Text & "'"
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetRoleMatrixDoc", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("documenttype", ddlDocumentType.SelectedItem.Text)
        oda.SelectCommand.Parameters.AddWithValue("sField", str)


        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds
        gvData.DataBind()
        If ds.Tables("data").Rows.Count > 0 Then
            lblRecord.Text = ""
        Else
            lblRecord.Text = "No record has been found"
        End If
        oda.Dispose()
        con.Close()
        con.Dispose()
        '        updPnlGrid.Update()
    End Sub

    Protected Sub ADD(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FormDesc,formcaption,displayName,FieldType,DropDownType,dropdown,FieldMapping,LayoutType,isrequired,datatype,fieldid,cal_fields,autofilter  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & ddlDocumentType.SelectedItem.Text & "' and FF.isworkflow=1 order by displayOrder", con)
        'Dim ds As New DataSet()
        'oda.Fill(ds, "fields")
        btnActEdit.Text = "Save"
        ddlFieldName.Items.Clear()
        ddlFieldName.Visible = False
        lblfldname.visible = False
        ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByText("Please Select"))
        ddlstatus1.SelectedIndex = ddlstatus1.Items.IndexOf(ddlstatus1.Items.FindByText("Please Select"))
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
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_RoleMatrix where EID=" & Session("EID") & " AND tid =" & pid & " order by ordering", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "fields")
        If ds.Tables("fields").Rows.Count > 0 Then
            ' Dim ob As New DynamicForm
            ' ob.FillControlsOnAuthMatrix(ds.Tables("fields"), pnlFields, pnluser, "MASTER", pid)
            oda.SelectCommand.CommandText = "Select * from MMM_MST_RoleMatrix WHERE TID=" & pid
            oda.Fill(ds, "data")
            ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByText(ds.Tables("data").Rows(0).Item("rolename").ToString()))
            If ds.Tables("data").Rows(0).Item("rolename").ToString() = "#USER" Or ds.Tables("data").Rows(0).Item("rolename").ToString() = "#SUPERVISOR" Or ds.Tables("data").Rows(0).Item("rolename").ToString() = "#LAST SUPERVISOR" Then
                Call ShowHideFieldDdl(ds.Tables("data").Rows(0).Item("rolename").ToString())
                'ddlFieldName.Visible = True
                'lblfldName.Visible = True
                ddlFieldName.SelectedIndex = ddlFieldName.Items.IndexOf(ddlFieldName.Items.FindByValue(ds.Tables("data").Rows(0).Item("fieldname").ToString()))
            Else
                'ddlFieldName.Visible = False
                'lblfldName.Visible = False
            End If
            ddlstatus1.SelectedIndex = ddlstatus1.Items.IndexOf(ddlstatus1.Items.FindByText(ds.Tables("data").Rows(0).Item("aprstatus").ToString()))
            txtsla.Text = ds.Tables("data").Rows(0).Item("sla").ToString()
            txtOrdering.Text = ds.Tables("data").Rows(0).Item("ordering").ToString()
        Else
            
        End If
        oda.Dispose()
        con.Dispose()
        btnActEdit.Text = "Update"
        updatePanelEdit.Update()
        btnEdit_ModalPopupExtender.Show()
        lblTab.Text = ""
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("DELETE FROM MMM_MST_rolematrix WHERE TID=" & ViewState("Did").ToString(), con)
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
        If UCase(ddlRole.SelectedItem.Text) = "PLEASE SELECT" Then
            lblTab.Text = "Please select Role Name"
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
        ' Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE Isactive=1 and  EID=" & Session("EID").ToString() & " and Documenttype='" & screenname & "' and isworkflow=1 order by displayOrder", con)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select * from mmm_mst_rolematrix where EID=" & Session("EID").ToString() & " and Doctype='" & screenname & "' and ordering=" & txtOrdering.Text, con)
        Dim dtchk As New DataTable
        da.Fill(dtchk)
        If dtchk.Rows.Count > 0 Then
            lblTab.Text = "Ordering value already exists"
            dtchk.Dispose()
            da.Dispose()
            con.Dispose()
            Exit Sub
        End If

        Dim fldName As String = ""

        If ddlRole.SelectedItem.Text = "#SELF" Then
            fldName = "OUID"
        ElseIf ddlRole.SelectedItem.Text = "#USER" Or ddlRole.SelectedItem.Text = "#SUPERVISOR" Or ddlRole.SelectedItem.Text = "#LAST SUPERVISOR" Then
            fldName = ddlFieldName.SelectedItem.Value
        End If

        Dim ob As New DynamicForm
        Dim FinalQry As String
        If actionType = "ADD" Then
            FinalQry = "INSERT INTO MMM_MST_ROLEMATRIX(EID,Doctype,rolename,aprstatus,sla,fieldname,ordering) VALUES (" & Session("EID").ToString() & ",'" & Session("doctype").ToString() & "','" & ddlRole.SelectedItem.Text & "','" & ddlstatus1.SelectedItem.Text & "'," & txtsla.Text & ",'" & fldName & "'," & txtOrdering.Text & ")"
        Else
            'pass query of updation and also type
            FinalQry = "UPDATE MMM_MST_ROLEMATRIX SET rolename='" & ddlRole.SelectedItem.Text & "',aprstatus='" & ddlstatus1.SelectedItem.Text & "',sla=" & txtsla.Text & ",fieldname='" & fldName & "', ordering=" & txtOrdering.Text & ""
        End If

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
            ' da.SelectCommand.CommandText = "UPDATE MMM_MST_ROLEMATRIX SET ORDERING=" & tid & " WHERE TID =" & tid & ""
            ' da.SelectCommand.ExecuteNonQuery()
            ' ob.CLEARDYNAMICFIELDS(pnlFields)
            btnActEdit.Text = ""
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

    'Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    '    ' Verifies that the control is rendered
    'End Sub

    'Protected Sub btnimpAM_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimpAM.Click
    '    modalpopupimport.Show()
    'End Sub

    'Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable
    '    Dim returnTable As New DataTable()
    '    If columnX = "" Then
    '        columnX = table.Columns(0).ColumnName
    '    End If

    '    Dim columnXValues As New List(Of String)()

    '    For Each dr As DataRow In table.Rows
    '        Dim columnXTemp As String = dr(columnX).ToString()
    '        If Not columnXValues.Contains(columnXTemp) Then
    '            columnXValues.Add(columnXTemp)
    '            returnTable.Columns.Add(columnXTemp)
    '        End If
    '    Next
    '    If nullValue <> "" Then
    '        For Each dr As DataRow In returnTable.Rows
    '            For Each dc As DataColumn In returnTable.Columns
    '                If dr(dc.ColumnName).ToString() = "" Then
    '                    dr(dc.ColumnName) = nullValue
    '                End If
    '            Next
    '        Next
    '    End If
    '    Return returnTable
    'End Function

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

    Protected Sub ddlstatus1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlstatus1.SelectedIndexChanged
        txtOrdering.Text = ddlstatus1.SelectedItem.Value
    End Sub

    Protected Sub ShowHideFieldDdl(ByVal SeleRole As String)
        If SeleRole = "#USER" Or SeleRole = "#SUPERVISOR" Or SeleRole = "#LAST SUPERVISOR" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim TBLName As String
            If SeleRole = "#USER" Then
                TBLName = Session("doctype")
            Else
                TBLName = "#USER"
            End If

            Dim oda As SqlDataAdapter = New SqlDataAdapter("select fieldId,displayname,fieldMapping from mmm_mst_fields where eid=" & Session("eid") & " and documenttype='" & TBLName & "'", con)
            oda.SelectCommand.CommandType = CommandType.Text
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim DT As New DataTable
            oda.Fill(DT)
            lblfldName.Visible = True
            ddlFieldName.Visible = True
            ddlFieldName.Items.Clear()
            ddlFieldName.Items.Insert(0, "Please Select")
            If DT.Rows.Count <> 0 Then
                For i As Integer = 0 To DT.Rows.Count - 1
                    ddlFieldName.Items.Add(DT.Rows(i).Item("displayname"))
                    ddlFieldName.Items(i + 1).Value = DT.Rows(i).Item("fieldmapping")
                Next
            End If
            oda.Dispose()
            con.Dispose()
            DT.Dispose()
        Else
            lblfldName.Visible = False
            ddlFieldName.Visible = False
        End If
    End Sub

    Protected Sub ddlRole_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRole.SelectedIndexChanged
        Call ShowHideFieldDdl(ddlRole.SelectedItem.Text)
    End Sub
End Class
