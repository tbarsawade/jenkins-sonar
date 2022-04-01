Imports System.Data
Imports System.Data.SqlClient
Partial Class MIS_DashBoard_Config
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
            If ddldbname.Text = "Invoice Distribution" Then
                lblinvtype.Visible = True
                ddlInvtype.Visible = True
                trvndrinvval.Visible = True
                InvType.Visible = False
                povaluefield()
                PopulateType()
            Else
                lblinvtype.Visible = False
                ddlInvtype.Visible = False
                trvndrinvval.Visible = False
                InvType.Visible = False
            End If
            
            'Dim dt As New DataTable()
            'dt.Columns.AddRange(New DataColumn(4) {New DataColumn("DocumentType"), New DataColumn("WorkFlow"), New DataColumn("alias"), New DataColumn("orderid"), New DataColumn("Tid")})
            'ViewState("Detail") = dt
            'Me.BindGrid()
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
    'Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)

    'End Sub
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
            Response.Redirect(Request.Url.AbsoluteUri)

            'Me.btnEdit_ModalPopupExtender.Hide()
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
        ht.Add("@InvType", ddlinvtypevendor.SelectedValue)
        ht.Add("@PovalueFields", ddlPofields.SelectedValue)
        ht.Add("@NonPovalueFields", ddlnonpodate.SelectedValue)
        ht.Add("@InvTypePOID", ddlpoType.SelectedValue)
        ht.Add("@InvTypeNonPOID", ddlnonpotype.SelectedValue)
        objDC.ExecuteProDT("AddUpdateDBConfigurationChildItem", ht)
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        Div2.Visible = False
        Label2.Text = ""
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBchildDetail"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnrefid.Value
        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
        cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = ddltype.SelectedItem.Text
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
        'ddldoctype.SelectedValue = "0"
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
            Dim row1 As GridViewRow = CType(CType(sender, ImageButton).Parent.Parent, GridViewRow)
            Dim dropdownvalue As String = ""
            dropdownvalue = row1.Cells(2).Text
            Div1.Visible = False
            Label1.Text = ""
            ddldbname.Text = dropdownvalue.ToString()
            trtid.Visible = False
            hdnrefid.Value = RefTid
            BindDocumentType()
            If ddldbname.Text = "Invoice Distribution" Then
                lblinvtype.Visible = True
                ddlInvtype.Visible = True
                trvndrinvval.Visible = True
                If ddlinvtypevendor.Text.ToUpper = "INVOICE TYPE" Then
                    InvType.Visible = True
                Else
                    InvType.Visible = False
                End If
            Else
                lblinvtype.Visible = False
                ddlInvtype.Visible = False
                trvndrinvval.Visible = False
                InvType.Visible = False
            End If

            'cleartex()
            Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(strConnString)
            Dim cmd As New SqlCommand()
            Div2.Visible = False
            Label2.Text = ""

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "uspGetDBchildDetail"
            cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnrefid.Value
            cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = ddltype.SelectedItem.Text
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
            fieldhdndisplayname.Value = row.Cells(2).Text
            DocumentTypeBind()
            Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(strConnString)
            Dim cmd As New SqlCommand()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "uspGetDBDetailitemvalue"
            cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdndetail.Value
            cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Dropdowntype.SelectedItem.Text
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
        objDT = objDC.ExecuteQryDT("select  fieldMapping+'-'+dropdown as fieldMapping,displayname from mmm_mst_fields where FieldType in ('Drop Down','AutoComplete')  and DropDownType='MASTER VALUED' and eid=" & Session("EID") & " and DocumentType='" & ddldoctype.SelectedItem.Text.Trim() & "' order by displayName")
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
        objDT = objDC.ExecuteQryDT("select  DisplayName,FieldMapping from mmm_mst_fields where datatype='Datetime' and fieldtype='Text Box' and Eid=" & Session("Eid") & " and documenttype='" & ddldoctype.SelectedItem.Text & "'  order by displayName")
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

    Private Sub povaluefield()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select fieldMapping,displayname from mmm_mst_fields where isactive=1  and eid=" & Session("EID") & " and DocumentType='" & ddldoctype.SelectedItem.Text.Trim() & "'  and fieldtype in ('Text Box','Calculative Field','Child Item Total') and datatype='Numeric' order by displayName")
        If objDT.Rows.Count > 0 Then
            ddlPofields.DataSource = objDT
            ddlPofields.DataTextField = "displayName"
            ddlPofields.DataValueField = "FieldMapping"
            ddlPofields.DataBind()
            ddlPofields.Items.Insert(0, "Select")
        Else
            ddlPofields.Items.Clear()
            ddlPofields.Items.Insert(0, "Select")
        End If
    End Sub

    Private Sub Nonpovaluefield()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select fieldMapping,displayname from mmm_mst_fields where isactive=1  and eid=" & Session("EID") & " and DocumentType='" & ddldoctype.SelectedItem.Text.Trim() & "'  and fieldtype in ('Text Box','Calculative Field','Child Item Total') and datatype='Numeric'  order by displayName")
        If objDT.Rows.Count > 0 Then
            ddlnonpodate.DataSource = objDT
            ddlnonpodate.DataTextField = "displayName"
            ddlnonpodate.DataValueField = "FieldMapping"
            ddlnonpodate.DataBind()
            ddlnonpodate.Items.Insert(0, "Select")
        Else
            ddlnonpodate.Items.Clear()
            ddlnonpodate.Items.Insert(0, "Select")
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim cmd As New SqlCommand()
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetMISDBWorkFlowDetail"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnrefid.Value
        cmd.Parameters.AddWithValue("@DocType", ddldoctype.SelectedItem.Text)
        cmd.Connection = con
        Try
            con.Open()
            Griddatediff.EmptyDataText = "No Records Found"
            Griddatediff.DataSource = cmd.ExecuteReader()
            Griddatediff.DataBind()
        Catch ex As Exception
            Throw ex
        Finally
            con.Close()
            con.Dispose()
        End Try


        'Griddatediff.DataSource = DirectCast(ViewState("Detail"), DataTable)
        'Griddatediff.DataBind()
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
        ht.Add("@Type", Dropdowntype.SelectedItem.Text)
        objDC.ExecuteProDT("AddDBConfigurationSetailItem", ht)

        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBDetailitemvalue"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdndetail.Value
        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
        cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Dropdowntype.SelectedItem.Text
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
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


        con.Open()
        Dim mss = New SqlCommand("Insert into mmm_mst_misdb_workflow(RefTid,DocumentType,WorkFlow,alias,orderid) values('" & hdnrefid.Value & "','" & ddldoctype.SelectedItem.Text & "','" & Filterstatus & "','" & txtalias.Text & "','" & txtorderid.Text & "')", con)
        mss.ExecuteNonQuery()
        con.Close()
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
    Protected Sub PopulateType()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select FieldMapping,DisplayName from mmm_mst_fields where documenttype like 'Vendor Invoice%' and eid=" & Session("Eid") & " and DropDownType='MASTER VALUED' and FieldType='Drop Down' and DBTableName='MMM_MST_MASTER' and isRequired=1 and isActive=1")
        If objDT.Rows.Count > 0 Then
            ddlinvtypevendor.DataSource = objDT
            ddlinvtypevendor.DataTextField = "displayName"
            ddlinvtypevendor.DataValueField = "FieldMapping"
            ddlinvtypevendor.DataBind()
            ddlinvtypevendor.Items.Insert(0, "Select")
        Else
            ddlinvtypevendor.Items.Clear()
            ddlinvtypevendor.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateFilterField()
        Datefield()
        If (ddldbname.Text = "Open Invoice Ageing") Then
            trwf.Visible = True
            Populateworkflowstatus()
            BindGrid()
        End If
        If (ddldbname.Text = "SLA Performance") Then
            trwf.Visible = True
            Populateworkflowstatus()
            BindGrid()
        End If
        If ddldbname.Text = "Invoice Distribution" Then
            lblinvtype.Visible = True
            ddlInvtype.Visible = True
            trvndrinvval.Visible = True
            'If ddlinvtypevendor.SelectedItem.Text.ToUpper = "INVOICE TYPE" Then
            '    InvType.Visible = True
            '    FillInvTypeID()
            'Else
            '    InvType.Visible = False
            'End If
            povaluefield()
            PopulateType()
        Else
            lblinvtype.Visible = False
            ddlInvtype.Visible = False
            trvndrinvval.Visible = False
            InvType.Visible = False
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
        Dim invtypes As New DataSet
        Dim csv As String = ""
        Dim lstDocumenettype As New ArrayList
        Dim QueryType As String = ""
        Dim ddlvalue As String = ""
        Dim newQuery1 As String = ""
        Dim QueryFilter As String = ""
        Dim QueryFilter1 As String = ""
        Dim filter As String = ""
        Dim Query12 As String = ""
        Dim TypeQuery As String = ""
        Dim diminvty As String = ""

        'For i As Integer = 0 To Me.gvDashboardData.Rows.Count - 1
        '    ddlvalue = Convert.ToString(Me.gvDashboardData.Rows(i).Cells(2).Text)
        'Next
        'ddldbname.Text = ddlvalue.ToString()
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
            If (btnchild.Text = "Save" And ddldbname.Text = "Expense Breakup") Then
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                Dim Query = "select top 5 category,sum(value)[value]from (select Tid, isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by case " & groupby & " end,documenttype,Tid) as t group by category order by [value] desc"
                Dim pit As String = ""
                pit = Query

                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()


            ElseIf (btnchild.Text = "Save" And ddldbname.Text = "Supplier Spend Breakup" And ddltype.SelectedItem.Text = "Department") Then
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                Dim Query = "select top 5 category,sum(value)[value] from(select Tid, isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by  case " & groupby & " end,documenttype,Tid) as t group by category order by [value] desc"
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
            ElseIf (btnchild.Text = "Save" And ddldbname.Text = "Invoice LifeCycle") Then
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)

                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""
                        std = pds.Tables(0).Rows(t).Item("CatField")
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                            casequery1 = casequery1 & "when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"

                            Dim ViewName = "@" & ddoctype.Replace(" ", "_") & "Date"
                            datequery = datequery & " when documenttype ='" & ddoctype & "' then " & ViewName & ""
                            groupby = groupby & "," & std & "," & ViewName & ""
                            QueryFilter1 = QueryFilter1 & "," & std & ""

                        End If
                    Next
                End If
                Dim query = "Select  category[category],Days[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount],case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end [Count]" _
                            & "  from (Select tid, Case " & StrColumn & " End[category], Case " & casequery1 & " End [Amount] , " _
                            & "  Case When datediff(dd,convert(Date,Case  " & datequery & " End ,3) ,(Select max(fdate) from mmm_doc_dtl With(nolock) where docid=d.tid ))<=15 Then '0-15 Days'" _
                            & "  when datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=16  and " _
                            & "  datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=30 then '16-30 Days'" _
                            & "  when datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=31  and " _
                            & "  datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=45 then '31-45 Days' " _
                            & "  when datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=46 then '45+ Days' end [Days] " _
                            & "  from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & "  and curstatus in ('archive') and @Date group by d.tid,documenttype " & groupby & "" _
                            & " union " _
                            & " select distinct '0'[Tid] , case " & StrColumn & " end[category], '0'[Amount] ,m.DaysType [days] from mmm_mst_doc d " _
                            & " cross join mmm_mst_misdbDaystype m where m.DashBoardname='invoice lifecycle' and eid=" & Session("Eid") & "" _
                            & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus in ('archive')" _
                            & " and @Date group by d.tid,documenttype " & QueryFilter1 & ",m.DaysType ) as t where t.category in (@category) and days is not null and category is not null and category<>'' group by category,days  "

                If (QueryType = "Department") Then
                    filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & "(select case " & StrColumn & " end[category],count(tid)[Count] " _
                             & "from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus in ('archive')" _
                             & "and @Date group by d.tid,documenttype " & QueryFilter1 & ")as d group by category order by [Count] desc)" _
                             & " as t  for xml path('')),1,1,'') as category"
                Else
                    filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & "(select case " & StrColumn & " end[category],count(tid)[Count] " _
                             & "from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus in ('archive')" _
                             & "and @Date group by d.tid,documenttype " & QueryFilter1 & ")as d group by category order by [Count] desc)" _
                             & " as t  for xml path('')),1,1,'') as category"
                End If
                Dim flt As String = ""
                flt = filter
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdnrefid.Value)
                ht1.Add("@QueryType", QueryType)
                ht1.Add("@Filter", flt)
                ht1.Add("@Query", pit)
                objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                Me.ModalPopupAddchilditem.Hide()
                BindDashbordConfig()
            ElseIf (btnchild.Text = "Save" And ddldbname.Text = "Invoice Distribution") Then
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField,InvType,PovalueFields,NonPovalueFields from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""

                        Dim Povaluefields As String = ""
                        Dim NonPovaluefields As String = ""
                        std = pds.Tables(0).Rows(t).Item("CatField")
                        ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                        valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                        QueryType = pds.Tables(0).Rows(t).Item("type")
                        datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                        datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                        If ddoctype.Contains("Vendor Invoice") Then
                            diminvty = pds.Tables(0).Rows(t).Item("InvType")
                            Povaluefields = pds.Tables(0).Rows(t).Item("PovalueFields")
                            NonPovaluefields = pds.Tables(0).Rows(t).Item("NonPovalueFields")
                        End If

                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
                        newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQuery, con)
                        da.Fill(predata)
                        If (predata.Tables(0).Rows.Count > 0) Then
                            StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                            If ddoctype.Contains("Vendor Invoice") Then
                                Query12 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & diminvty & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                                con = New SqlConnection(conStr)
                                da = New SqlDataAdapter(Query12, con)
                                da.Fill(invtypes)
                                If (invtypes.Tables(0).Rows.Count > 0) Then
                                    casequery1 = casequery1 & "when documenttype='" & ddoctype & "'  and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%Non%' then max(" & NonPovaluefields & ")"
                                    casequery1 = casequery1 & "when documenttype='" & ddoctype & "'  and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%Po%' then max(" & Povaluefields & ")"
                                    TypeQuery = TypeQuery & " when documenttype='" & ddoctype & "' and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%Non%' then 'Non PO'"
                                    TypeQuery = TypeQuery & " when documenttype='" & ddoctype & "' and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%PO' then 'PO'"
                                End If
                            Else
                                casequery1 = casequery1 & "when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"
                            End If
                            If ddoctype.Contains("Vendor Invoice") Then
                                groupby = groupby & "," & diminvty & ""
                                groupby = groupby & "," & std & ""
                            Else
                                groupby = groupby & "," & std & ""
                            End If
                        End If
                    Next
                End If
                Dim query = ""
                If invtypes.Tables.Count > 0 Then
                    If invtypes.Tables(0).Rows.Count > 0 Then
                        query = "Select category,Type[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount] , Case When convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 Then count(tid)-1 Else 0 End[Count]  " _
                            & "from (Select tid , Case " & StrColumn & " End [category], Case " & casequery1 & " End [Amount] ," _
                            & " Case When Documenttype Like '%non%' then 'Non PO' when Documenttype like '%PO%' then 'PO' " & TypeQuery & " end [Type] from mmm_mst_doc d " _
                            & " where eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                            & " and curstatus not in ('rejected') and  dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") not like '%hold' and @Date group by d.tid,documenttype " & groupby & " " _
                            & " Union " _
                            & "select distinct '0'[Tid],case " & StrColumn & "end[category],'0'[Amount] ,m.DaysType[days] from mmm_mst_doc d " _
                            & " cross join mmm_mst_misdbDaystype m   where m.DashBoardname='invoice Distribution' and eid='" & Session("Eid") & "'" _
                            & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected') and @Date" _
                            & " group by d.tid,documenttype " & groupby & ",m.DaysType ) as t where category in (@category) and type is not null and category is not null and category<>'' group by category,Type order by category"
                    End If
                Else
                    query = "Select category,Type[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount] , Case When convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 Then count(tid)-1 Else 0 End[Count]  " _
                        & "from (Select tid , Case " & StrColumn & " End [category], Case " & casequery1 & " End [Amount] ," _
                        & " Case When Documenttype Like '%non%' then 'Non PO' when Documenttype like '%PO%' then 'PO' " & TypeQuery & " end [Type] from mmm_mst_doc d " _
                        & " where eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                        & " and curstatus not in ('rejected') and @Date group by d.tid,documenttype " & groupby & " " _
                        & " Union " _
                        & "select distinct '0'[Tid],case " & StrColumn & "end[category],'0'[Amount] ,m.DaysType[days] from mmm_mst_doc d " _
                        & " cross join mmm_mst_misdbDaystype m   where m.DashBoardname='invoice Distribution' and eid='" & Session("Eid") & "'" _
                        & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected') and @Date" _
                        & " group by d.tid,documenttype " & groupby & ",m.DaysType ) as t where category in (@category) and type is not null and category is not null and category<>'' group by category,Type order by category"
                End If


                If (QueryType = "Department") Then
                        filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & "(select case " & StrColumn & " end [category],count(tid)[Count] " _
                             & " from mmm_mst_doc d where eid='" & Session("Eid") & "'  and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                             & " and curstatus not in ('rejected') and @Date group by documenttype " & groupby & ") as d group by category order by [Count] desc) as t " _
                             & " for xml path('') ),1,1,'') as category "
                    Else
                        filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & " (select case " & StrColumn & " end [category],count(tid)[Count] " _
                             & " from mmm_mst_doc d where eid='" & Session("Eid") & "'  and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                             & " and curstatus not in ('rejected') and @Date group by documenttype " & groupby & ") as d group by category order by [Count] desc) as t " _
                             & " for xml path('') ),1,1,'') as category "
                    End If
                    Dim flt As String = ""
                    flt = filter
                    Dim pit As String = ""
                    pit = query
                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    ht1.Add("@Filter", flt)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    Me.ModalPopupAddchilditem.Hide()
                    BindDashbordConfig()
                ElseIf (btnchild.Text = "Save" And ddldbname.Text = "Open Invoice Ageing") Then
                    Dim order As String = ""
                    Dim vLoop As Integer = 0

                    Dim workflowstatus As String = ""
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)


                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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

                                casequery1 = casequery1 & " when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"

                                Dim ViewName = "@" & ddoctype.Replace(" ", "_") & "Date"
                                daterequired = daterequired & " when documenttype='" & ddoctype & "' then  " & ViewName & ""
                                groupby = groupby & "," & ViewName & ""

                            End If
                        Next
                        newQuery1 = "select Tid,reftid,DocumentType,WorkFlow,alias,Orderid from mmm_mst_misdb_workflow where RefTid=" & hdnrefid.Value & ""
                        con = New SqlConnection(conStr)
                        da1 = New SqlDataAdapter(newQuery1, con)
                        Dim NameAlias As String = ""
                        Dim ordr As String = ""
                        da1.Fill(sdp)
                        If (sdp.Tables(0).Rows.Count > 0) Then
                            For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                                If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                    workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                    workflowstatus = workflowstatus.Replace(",", "','")
                                    NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                    ordr = sdp.Tables(0).Rows(c).Item("Orderid")
                                    StrColumn = StrColumn & "when curstatus in ('" & workflowstatus & "') then '" & NameAlias & "'"
                                    order = order & " when curstatus in ('" & workflowstatus & "') then  " & ordr & ""
                                End If
                            Next
                        End If
                    End If
                    Dim query = "select category,Days[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount],case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end[Count] ,max([order])[Order]" _
                            & " from (select tid,case " & StrColumn & " end [category] , case " & casequery1 & " end [Amount]," _
                            & " case when datediff(dd,convert(date,case  " & daterequired & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=5 then ' 0-5 Days'" _
                            & " when datediff(dd,convert(date,case  " & daterequired & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=6 " _
                            & " and datediff(dd,convert(date,case  " & daterequired & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=10 then ' 6-10 Days'" _
                            & " when datediff(dd,convert(date,case  " & daterequired & " end ,3), (select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=11 " _
                            & " and datediff(dd,convert(date,case  " & daterequired & " end ,3) , (select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=15 then '11-15 Days' " _
                            & " when datediff(dd,convert(date,case  " & daterequired & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=16 then '15+ Days' end [Days] ," _
                            & " case " & order & " end [order]" _
                            & " from mmm_mst_doc d with(nolock) where Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected','Archive')  and @Dept" _
                            & " group by curstatus,d.tid,documenttype " & groupby & "" _
                            & " union " _
                            & " select distinct '0', case " & StrColumn & " end[category] , '0'[Amount],m.DaysType[Days]," _
                            & " case " & order & " end [order] from mmm_mst_doc d with(nolock) cross join  mmm_mst_misdbDaystype m " _
                            & " where Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and " _
                            & " curstatus not in ('rejected','Archive') and m.DashBoardname='Open Invoice Ageing' and @Dept" _
                            & " group by curstatus,d.tid,documenttype,m.DaysType) as t where  days is not null and category is not null and category<>'' group by category,days order by max([category]),Days"

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
                ElseIf (btnchild.Text = "Save" And ddldbname.Text = "SLA Performance") Then
                    Dim order As String = ""

                    Dim workflowstatus As String = ""
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)

                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                                casequery1 = casequery1 & "when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"
                            End If
                        Next
                        newQuery1 = "SELECT orderid,alias,STUFF (( SELECT ',' + workflow[workflow]FROM mmm_mst_misdb_workflow  where alias=w.alias FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')workflow from mmm_mst_misdb_workflow w where reftid=" & hdnrefid.Value & " group by alias,orderid"
                        con = New SqlConnection(conStr)
                        da1 = New SqlDataAdapter(newQuery1, con)
                        Dim NameAlias As String = ""
                        Dim ordr As String = ""
                        da1.Fill(sdp)
                        If (sdp.Tables(0).Rows.Count > 0) Then
                            For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                                If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                    workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                    workflowstatus = workflowstatus.Replace(",", "','")
                                    NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                    ordr = sdp.Tables(0).Rows(c).Item("orderid")
                                    StrColumn = StrColumn & "when aprstatus in ('" & workflowstatus & "') then '" & NameAlias & "'"
                                    order = order & " when aprstatus in ('" & workflowstatus & "') then  " & ordr & ""
                                End If
                            Next
                        End If
                    End If
                    Dim query = " select  category,Type[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount],case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end[Count],max([order])[Order] " _
                            & " from (select distinct d.tid , case  " & StrColumn & " else aprstatus end category ," _
                            & " case " & casequery1 & " end [Amount] ,case when max(dl.ptat)>max(dl.atat) then 'SLA Breached' when max(dl.ptat)<=max(dl.atat) then 'Within SLA' end[Type] ," _
                            & " case " & order & " end [order] from mmm_mst_doc d with(nolock) join mmm_doc_dtl dl on dl.docid=d.tid " _
                            & " where Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected')  and aprstatus not in ('rejected','RECALLED') " _
                            & " and @Date and @Dept and @status and   aprstatus is not null group by d.tid,documenttype,aprstatus " _
                            & " union " _
                            & " select distinct 0[Tid], case " & StrColumn & " else aprstatus end category ,'0' [Amount] ,m.DaysType[Type] ," _
                            & " case " & order & " end [order] from mmm_mst_doc d with(nolock) join mmm_doc_dtl dl on dl.docid=d.tid cross join mmm_mst_misdbDaystype m " _
                            & " where m.DashBoardname='SLA Performance' and Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and " _
                            & " curstatus not in ('rejected')  and aprstatus not in ('rejected','RECALLED')" _
                            & " and @Date and @Dept and @status and   aprstatus is not null " _
                            & " group by d.tid,documenttype,aprstatus,m.DaysType) as t where  type is not null and category is not null and category<>'' group by category,Type order by [Order]"
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
                    If (btnchild.Text = "Update" And ddldbname.Text = "Expense Breakup") Then
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            std = pds.Tables(0).Rows(t).Item("CatField")
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                    Dim Query = "select top 5 category,sum(value)[value]from (select Tid, isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by case " & groupby & " end,documenttype,Tid) as t group by category order by [value] desc"
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
                If (btnchild.Text = "Update" And ddldbname.Text = "Supplier Spend Breakup" And ddltype.SelectedItem.Text = "Department") Then
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & ""
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)

                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                    Dim Query = "select top 5 category,sum(value)[value] from(select Tid, isnull(case " & StrColumn & " end,'Others') [category], case " & casequery1 & " end [value] from mmm_mst_doc where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus<>'rejected' and @Date  group by  case " & groupby & " end,documenttype,Tid) as t group by category order by [value] desc"
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
                If (btnchild.Text = "Update" And ddldbname.Text = "Invoice LifeCycle") Then
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                                casequery1 = casequery1 & "when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"
                                Dim ViewName = "@" & ddoctype.Replace(" ", "_") & "Date"
                                datequery = datequery & " when documenttype ='" & ddoctype & "' then " & ViewName & ""
                                groupby = groupby & "," & std & "," & ViewName & ""
                                QueryFilter1 = QueryFilter1 & "," & std & ""
                            End If
                        Next
                    End If
                    Dim query = "Select  category[category],Days[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount],case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end [Count]" _
                            & "  from (Select tid, Case " & StrColumn & " End[category], Case " & casequery1 & " End [Amount] , " _
                            & "  Case When datediff(dd,convert(Date,Case  " & datequery & " End ,3) ,(Select max(fdate) from mmm_doc_dtl With(nolock) where docid=d.tid ))<=15 Then '0-15 Days'" _
                            & "  when datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=16  and " _
                            & "  datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=30 then '16-30 Days'" _
                            & "  when datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=31  and " _
                            & "  datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=45 then '31-45 Days' " _
                            & "  when datediff(dd,convert(date,case  " & datequery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=46 then '45+ Days' end [Days] " _
                            & "  from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & "  and curstatus in ('archive') and @Date group by d.tid,documenttype " & groupby & "" _
                            & " union " _
                            & " select distinct '0'[Tid] , case " & StrColumn & " end[category], '0'[Amount] ,m.DaysType [days] from mmm_mst_doc d " _
                            & " cross join mmm_mst_misdbDaystype m where m.DashBoardname='invoice lifecycle' and eid=" & Session("Eid") & "" _
                            & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus in ('archive')" _
                            & " and @Date group by d.tid,documenttype " & QueryFilter1 & ",m.DaysType ) as t where t.category in (@category) and days is not null and category is not null and category<>'' group by category,days "

                    If (QueryType = "Department") Then
                        filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & "(select case " & StrColumn & " end[category],count(tid)[Count] " _
                             & "from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus in ('archive')" _
                             & "and @Date group by d.tid,documenttype " & QueryFilter1 & ")as d group by category order by [Count] desc)" _
                             & " as t  for xml path('')),1,1,'') as category"
                    Else
                        filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & "(select case " & StrColumn & " end[category],count(tid)[Count] " _
                             & "from mmm_mst_doc d where eid=" & Session("Eid") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus in ('archive')" _
                             & "and @Date group by d.tid,documenttype " & QueryFilter1 & ")as d group by category order by [Count] desc)" _
                             & " as t  for xml path('')),1,1,'') as category"
                    End If
                    Dim flt As String = ""
                    flt = filter
                    Dim pit As String = ""
                    pit = query
                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@Filter", flt)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    Me.ModalPopupAddchilditem.Hide()
                    BindDashbordConfig()
                End If
                If (btnchild.Text = "Update" And ddldbname.Text = "Invoice Distribution") Then
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField,InvType,PovalueFields,NonPovalueFields from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""

                            Dim Povaluefields As String = ""
                            Dim NonPovaluefields As String = ""
                            std = pds.Tables(0).Rows(t).Item("CatField")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            valuefield = pds.Tables(0).Rows(t).Item("Valuefield")
                            QueryType = pds.Tables(0).Rows(t).Item("type")
                            datefield = pds.Tables(0).Rows(t).Item("RecdateField")
                            datefield1 = pds.Tables(0).Rows(t).Item("InvDateField")
                            If ddoctype.Contains("Vendor Invoice") Then
                                diminvty = pds.Tables(0).Rows(t).Item("InvType")
                                Povaluefields = pds.Tables(0).Rows(t).Item("PovalueFields")
                                NonPovaluefields = pds.Tables(0).Rows(t).Item("NonPovalueFields")
                            End If


                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                                If ddoctype.Contains("Vendor Invoice") Then
                                    Query12 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & diminvty & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1 and DropDownType='MASTER VALUED'"
                                    con = New SqlConnection(conStr)
                                    da = New SqlDataAdapter(Query12, con)
                                    da.Fill(invtypes)
                                    If (invtypes.Tables(0).Rows.Count > 0) Then
                                        casequery1 = casequery1 & "when documenttype='" & ddoctype & "'  and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%Non%' then max(" & NonPovaluefields & ")"
                                        casequery1 = casequery1 & "when documenttype='" & ddoctype & "'  and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%Po%' then max(" & Povaluefields & ")"
                                        TypeQuery = TypeQuery & " when documenttype='" & ddoctype & "' and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%Non%' then 'Non PO'"
                                        TypeQuery = TypeQuery & " when documenttype='" & ddoctype & "' and dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") like '%PO' then 'PO'"

                                    End If
                                Else
                                    casequery1 = casequery1 & "when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"
                                End If
                                If ddoctype.Contains("Vendor Invoice") Then
                                    groupby = groupby & "," & diminvty & ""
                                    groupby = groupby & "," & std & ""

                                Else
                                    groupby = groupby & "," & std & ""
                                End If
                            End If
                        Next
                    End If
                    Dim query = ""
                    If invtypes.Tables(0).Rows.Count > 0 Then
                        query = "select category,Type[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount] , case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end[Count]  " _
                            & "from (select tid , case " & StrColumn & " end [category], case " & casequery1 & " end [Amount] ," _
                            & " case when Documenttype like '%non%' then 'Non PO' when Documenttype like '%PO%' then 'PO' " & TypeQuery & " end [Type] from mmm_mst_doc d " _
                            & " where eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                            & " and curstatus not in ('rejected') and  dms.udf_split('" & invtypes.Tables(0).Rows(0).Item("dropdown") & "'," & diminvty & ") not like '%hold' and @Date group by d.tid,documenttype " & groupby & " " _
                            & " Union " _
                            & "select distinct '0'[Tid],case " & StrColumn & "end[category],'0'[Amount] ,m.DaysType[days] from mmm_mst_doc d " _
                            & " cross join mmm_mst_misdbDaystype m   where m.DashBoardname='invoice Distribution' and eid='" & Session("Eid") & "'" _
                            & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected') and @Date" _
                            & " group by d.tid,documenttype " & groupby & ",m.DaysType ) as t where category in (@category) and type is not null and category is not null and category<>'' group by category,Type order by category"
                    Else
                        query = "select category,Type[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount] , case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end[Count]  " _
                            & "from (select tid , case " & StrColumn & " end [category], case " & casequery1 & " end [Amount] ," _
                            & " case when Documenttype like '%non%' then 'Non PO' when Documenttype like '%PO%' then 'PO' " & TypeQuery & " end [Type] from mmm_mst_doc d " _
                            & " where eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                            & " and curstatus not in ('rejected')  and @Date group by d.tid,documenttype " & groupby & " " _
                            & " Union " _
                            & "select distinct '0'[Tid],case " & StrColumn & "end[category],'0'[Amount] ,m.DaysType[days] from mmm_mst_doc d " _
                            & " cross join mmm_mst_misdbDaystype m   where m.DashBoardname='invoice Distribution' and eid='" & Session("Eid") & "'" _
                            & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected') and @Date" _
                            & " group by d.tid,documenttype " & groupby & ",m.DaysType ) as t where category in (@category) and type is not null and category is not null and category<>'' group by category,Type order by category"
                    End If



                    If (QueryType = "Department") Then
                        filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & "(select case " & StrColumn & " end [category],count(tid)[Count] " _
                             & " from mmm_mst_doc d where eid='" & Session("Eid") & "'  and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                             & " and curstatus not in ('rejected') and @Date group by documenttype " & groupby & ") as d group by category order by [Count] desc) as t " _
                             & " for xml path('') ),1,1,'') as category "
                    Else
                        filter = "select distinct stuff((select ','+ category from (select top 5 category,sum([Count])[Count] from " _
                             & " (select case " & StrColumn & " end [category],count(tid)[Count] " _
                             & " from mmm_mst_doc d where eid='" & Session("Eid") & "'  and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') " _
                             & " and curstatus not in ('rejected') and @Date group by documenttype " & groupby & ") as d group by category order by [Count] desc) as t " _
                             & " for xml path('') ),1,1,'') as category "
                    End If
                    Dim flt As String = ""
                    flt = filter
                    Dim pit As String = ""
                    pit = query
                    Dim objDC1 As New DataClass()
                    Dim ht1 As New Hashtable
                    ht1.Add("@Tid", hdnrefid.Value)
                    ht1.Add("@QueryType", QueryType)
                    ht1.Add("@Query", pit)
                    ht1.Add("@Filter", flt)
                    objDC1.ExecuteProDT("UpdateDBConfigurationQuery", ht1)
                    Me.ModalPopupAddchilditem.Hide()
                    BindDashbordConfig()

                End If
                If (btnchild.Text = "Update" And ddldbname.Text = "Open Invoice Ageing") Then

                    Dim order As String = ""

                    Dim workflowstatus As String = ""
                    newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            Dim std As String = ""
                            Dim ddoctype As String = ""
                            Dim valuefield As String = ""
                            Dim datefield As String = ""
                            Dim datefield1 As String = ""
                            lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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

                                casequery1 = casequery1 & " when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"

                                Dim ViewName = "@" & ddoctype.Replace(" ", "_") & "Date"
                                daterequired = daterequired & " when documenttype='" & ddoctype & "' then  " & ViewName & ""
                                groupby = groupby & "," & ViewName & ""

                            End If
                        Next
                        newQuery1 = "select Tid,reftid,DocumentType,WorkFlow,alias,Orderid from mmm_mst_misdb_workflow where RefTid=" & hdnrefid.Value & ""
                        con = New SqlConnection(conStr)
                        da1 = New SqlDataAdapter(newQuery1, con)
                        Dim NameAlias As String = ""
                        Dim ordr As String = ""
                        da1.Fill(sdp)
                        If (sdp.Tables(0).Rows.Count > 0) Then
                            For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                                If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                    workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                    workflowstatus = workflowstatus.Replace(",", "','")
                                    NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                    ordr = sdp.Tables(0).Rows(c).Item("Orderid")
                                    StrColumn = StrColumn & "when curstatus in ('" & workflowstatus & "') then '" & NameAlias & "'"
                                    order = order & " when curstatus in ('" & workflowstatus & "') then  " & ordr & ""
                                End If
                            Next
                        End If
                    End If
                    Dim query = "select category,Days[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount],case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end[Count] ,max([order])[Order]" _
                            & " from (select tid,case " & StrColumn & " end [category] , case " & casequery1 & " end [Amount]," _
                            & " case when datediff(dd,convert(date,case  " & daterequired & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=5 then ' 0-5 Days'" _
                            & " when datediff(dd,convert(date,case  " & daterequired & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=6 " _
                            & " and datediff(dd,convert(date,case  " & daterequired & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=10 then ' 6-10 Days'" _
                            & " when datediff(dd,convert(date,case  " & daterequired & " end ,3), (select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=11 " _
                            & " and datediff(dd,convert(date,case  " & daterequired & " end ,3) , (select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=15 then '11-15 Days' " _
                            & " when datediff(dd,convert(date,case  " & daterequired & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=16 then '15+ Days' end [Days] ," _
                            & " case " & order & " end [order]" _
                            & " from mmm_mst_doc d with(nolock) where Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected','Archive')  and @Dept" _
                            & " group by curstatus,d.tid,documenttype " & groupby & "" _
                            & " union " _
                            & " select distinct '0', case " & StrColumn & " end[category] , '0'[Amount],m.DaysType[Days]," _
                            & " case " & order & " end [order] from mmm_mst_doc d with(nolock) cross join  mmm_mst_misdbDaystype m " _
                            & " where Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and " _
                            & " curstatus not in ('rejected','Archive') and m.DashBoardname='Open Invoice Ageing' and @Dept" _
                            & " group by curstatus,d.tid,documenttype,m.DaysType) as t where days is not null and category is not null and category<>'' group by category,days order by max([category]),Days"

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
            If (btnchild.Text = "Update" And ddldbname.Text = "SLA Performance") Then
                Dim order As String = ""
                Dim workflowstatus As String = ""
                newQuery = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdnrefid.Value & " and Type='" & ddltype.SelectedItem.Text & "'"
                Dim Dtable As New DataTable
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(newQuery, con)
                da.Fill(pds)
                If (pds.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                        Dim std As String = ""
                        Dim ddoctype As String = ""
                        Dim valuefield As String = ""
                        Dim datefield As String = ""
                        Dim datefield1 As String = ""
                        lstDocumenettype.Add(Convert.ToString(pds.Tables(0).Rows(t).Item("DocumentType")))
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
                            casequery1 = casequery1 & "when documenttype='" & ddoctype & "' and max(" & valuefield & ")<>'' then max(" & valuefield & ")"
                        End If
                    Next
                    newQuery1 = "SELECT max(orderid)orderid,alias,STUFF (( SELECT ',' + workflow[workflow]FROM mmm_mst_misdb_workflow  where alias=w.alias FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')workflow from mmm_mst_misdb_workflow w where reftid=" & hdnrefid.Value & " group by alias"
                    'newQuery1 = "select Tid,reftid,DocumentType,WorkFlow,alias,Orderid from mmm_mst_misdb_workflow where RefTid=" & hdnrefid.Value & ""
                    con = New SqlConnection(conStr)
                    da1 = New SqlDataAdapter(newQuery1, con)
                    Dim NameAlias As String = ""
                    Dim ordr As String = ""
                    da1.Fill(sdp)
                    If (sdp.Tables(0).Rows.Count > 0) Then
                        For c As Integer = 0 To sdp.Tables(0).Rows.Count - 1
                            If Not IsDBNull(sdp.Tables(0).Rows(c).Item("WorkFlow")) Then
                                workflowstatus = sdp.Tables(0).Rows(c).Item("WorkFlow")
                                workflowstatus = workflowstatus.Replace(",", "','")
                                NameAlias = sdp.Tables(0).Rows(c).Item("alias")
                                ordr = sdp.Tables(0).Rows(c).Item("orderid")
                                StrColumn = StrColumn & "when aprstatus in ('" & workflowstatus & "') then '" & NameAlias & "'"
                                order = order & " when aprstatus in ('" & workflowstatus & "') then  " & ordr & ""
                            End If
                        Next
                    End If
                End If
                Dim query = " select  category,Type[DaysType],convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)[Amount],case when convert(numeric(10,4),sum(convert(numeric(10,2),Amount))/1000000)>0 then count(tid)-1 else 0 end[Count],max([order])[Order] " _
                            & " from (select distinct d.tid , case  " & StrColumn & " else aprstatus end category ," _
                            & " case " & casequery1 & " end [Amount] ,case when max(dl.ptat)>max(dl.atat) then 'SLA Breached' when max(dl.ptat)<=max(dl.atat) then 'Within SLA' end[Type] ," _
                            & " case " & order & " end [order] from mmm_mst_doc d with(nolock) join mmm_doc_dtl dl on dl.docid=d.tid " _
                            & " where Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and curstatus not in ('rejected')  and aprstatus not in ('rejected','RECALLED') " _
                            & " and @Date and @Dept and @status and   aprstatus is not null group by d.tid,documenttype,aprstatus " _
                            & " union " _
                            & " select distinct 0[Tid], case " & StrColumn & " else aprstatus end category ,'0' [Amount] ,m.DaysType[Type]," _
                            & " case " & order & " end [order] from mmm_mst_doc d with(nolock) join mmm_doc_dtl dl on dl.docid=d.tid cross join mmm_mst_misdbDaystype m " _
                            & " where m.DashBoardname='SLA Performance' and Eid='" & Session("Eid") & "' and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "') and " _
                            & " curstatus not in ('rejected')  and aprstatus not in ('rejected','RECALLED')" _
                            & " and @Date and @Dept and @status and   aprstatus is not null " _
                            & " group by d.tid,documenttype,aprstatus,m.DaysType) as t where type is not null and category is not null and category<>'' group by category,Type order by [Order]"

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
            'Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)

            'For i As Integer = 0 To Me.gvDashboardData.Rows.Count - 1
            '    dropdownvalue = Convert.ToString(Me.gvDashboardData.Rows(i).Cells(2).Text)
            'Next
            'ddldbname.Text = dropdownvalue.ToString()
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

                    cmd.CommandText = ("select  Tid,RefTid,DocumentType,Type,catField,ValueField,RecdateField,InvDateField,InvType,PovalueFields,NonPovalueFields from mmm_mst_misdb_dtl where Tid=" & ChildId & " and  DocumentType='" & Docname & "' and RefTid=" & pid)
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
                            'PopulateType()
                            'For Each Iteminvtype1 As ListItem In ddlinvtypevendor.Items
                            '    If Iteminvtype1.Value.ToString().Contains(sdr("InvType")) Then
                            '        Iteminvtype1.Selected = True
                            '    Else
                            '        Iteminvtype1.Selected = False
                            '    End If
                            'Next
                            If (ddldbname.Text = "Invoice Distribution") Then
                                povaluefield()
                                For Each ItemPo As ListItem In ddlPofields.Items
                                    If ItemPo.Value.ToString().Contains(sdr("PovalueFields")) Then
                                        ItemPo.Selected = True
                                    Else
                                        ItemPo.Selected = False
                                    End If
                                Next

                                PopulateType()
                                For Each Iteminvtype1 As ListItem In ddlinvtypevendor.Items
                                    If Iteminvtype1.Value.ToString().Contains(sdr("InvType")) Then
                                        Iteminvtype1.Selected = True
                                    Else
                                        Iteminvtype1.Selected = False
                                    End If
                                Next

                                Nonpovaluefield()
                                For Each ItemNonPo As ListItem In ddlnonpodate.Items
                                    If ItemNonPo.Value.ToString().Contains(sdr("NonPovalueFields")) Then
                                        ItemNonPo.Selected = True
                                    Else
                                        ItemNonPo.Selected = False
                                    End If
                                Next
                            End If

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
        Dim row1 As GridViewRow = CType(CType(sender, ImageButton).Parent.Parent, GridViewRow)
        'Dim pid As Integer = Convert.ToString(Me.detailgriditem.DataKeys(row.RowIndex).Value)
        Dim pid As Integer = row1.Cells(1).Text
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                conn.Open()
                Dim mycommand = New SqlCommand("Delete from  mmm_mst_misdb_workflow where Tid=" & pid & " ", conn)
                mycommand.ExecuteNonQuery()
                conn.Close()
                BindGrid()
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
        Dim AndQuery As String = ""

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
        Dim DNEWQUERY As String = ""
        Dim newdata As New DataSet
        Dim Dtldoctype As String = ""
        Dim ddoctype As String = ""
        Dim Displayname1 As String = ""
        Dim ANDDEPT As String = ""

        Dim DBDETAILNAME As String = fieldhdndisplayname.Value
        If (DBDETAILNAME = "Invoice LifeCycle") Then
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
                   
                    'StrColumn = StrColumn & " end '" & dspname & "'"
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            std = pds.Tables(0).Rows(t).Item("fields")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            Displayname1 = pds.Tables(0).Rows(t).Item("DisplayName")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            predata.Clear()
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(0).Item("dropdown") & "'," & std & ")"
                                ElseIf (predata.Tables(0).Rows(0).Item("FieldType").ToString.ToUpper = "LOOKUPDDL") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.Get_LookupddlValue(" & Session("Eid") & ",'" & ddoctype & "'," & std & ",'" & std & "')"
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
                DNEWQUERY = "select DocumentType,Type,CatField,ValueField from mmm_mst_misdb_dtl where RefTid=" & hdndetail.Value & ""
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(DNEWQUERY, con)
                da.Fill(newdata)
                If (newdata.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To newdata.Tables(0).Rows.Count - 1
                        Dtldoctype = newdata.Tables(0).Rows(t).Item("DocumentType")
                        Dim ViewName = "@" & Dtldoctype.Replace(" ", "_") & "Date"
                        AndQuery = AndQuery & " when  documenttype='" & Dtldoctype & "' then " & ViewName & ""
                        lstDocumenettype.Add(Convert.ToString(newdata.Tables(0).Rows(t).Item("DocumentType")))
                    Next
                End If
            End If
            Dim newQueryforall1 As String = ""
            Dim CDATA As New DataSet
            Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdndetail.Value & " and type='" & Dropdowntype.SelectedItem.Text & "'"
            Dim tabledata As New DataTable
            Dim SETDATA As New DataSet
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(dtlDeptquery, con)
            da.Fill(SETDATA)
            If (SETDATA.Tables(0).Rows.Count > 0) Then
                For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                    Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                    Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                    newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & Session("Eid") & " and isactive=1"
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQueryforall1, con)
                    CDATA.Clear()
                    da.Fill(CDATA)
                    If (CDATA.Tables(0).Rows.Count > 0) Then
                        If (CDATA.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                            ANDDEPT = ANDDEPT & "( dms.udf_split('" & CDATA.Tables(0).Rows(0).Item("dropdown") & "'," & CatogoryField & ")=@dept and documenttype='" & CDATA.Tables(0).Rows(0).Item("documenttype") & "')"
                            If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                                ANDDEPT = ANDDEPT & " or "
                            End If
                        End If
                    End If
                Next
            End If
            Dim query1 = "select distinct  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus='archive' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and datediff(dd,convert(date,case " & AndQuery & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=@Days1" _
                        & "  and datediff(dd,convert(date,case " & AndQuery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=@Days2 and @Date and (" & ANDDEPT & ") "
            Dim AllDetail = "select  " & s & " " _
                        & " Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and datediff(dd,convert(date,case " & AndQuery & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=@Days1" _
                        & "  and datediff(dd,convert(date,case " & AndQuery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=@Days2 " _
                        & " and @Date"
            Dim pit1 As String = ""
            Dim dtlall As String = ""
            pit1 = query1
            dtlall = AllDetail
            Dim objDC2 As New DataClass()
            Dim ht2 As New Hashtable
            ht2.Add("@Tid", hdndetail.Value)
            ht2.Add("@QueryType", Dropdowntype.SelectedItem.Text)
            ht2.Add("@Query", pit1)
            ht2.Add("@QueryDTLAll", dtlall)
            objDC2.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht2)
        ElseIf (DBDETAILNAME = "Open Invoice Ageing") Then
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
                   
                    'StrColumn = StrColumn & " end '" & dspname & "'"
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            std = pds.Tables(0).Rows(t).Item("fields")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            Displayname1 = pds.Tables(0).Rows(t).Item("DisplayName")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            predata.Clear()
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(0).Item("dropdown") & "'," & std & ")"
                                ElseIf (predata.Tables(0).Rows(0).Item("FieldType").ToString.ToUpper = "LOOKUPDDL") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.Get_LookupddlValue(" & Session("Eid") & ",'" & ddoctype & "'," & std & ",'" & std & "')"
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
                DNEWQUERY = "select DocumentType,Type,CatField,ValueField from mmm_mst_misdb_dtl where RefTid=" & hdndetail.Value & ""
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(DNEWQUERY, con)
                da.Fill(newdata)
                If (newdata.Tables(0).Rows.Count > 0) Then
                    For t As Integer = 0 To newdata.Tables(0).Rows.Count - 1
                        Dtldoctype = newdata.Tables(0).Rows(t).Item("DocumentType")
                        Dim ViewName = "@" & Dtldoctype.Replace(" ", "_") & "Date"
                        AndQuery = AndQuery & " when  documenttype='" & Dtldoctype & "' then " & ViewName & ""
                        lstDocumenettype.Add(Convert.ToString(newdata.Tables(0).Rows(t).Item("DocumentType")))
                    Next
                End If
            End If
            Dim query3 = "select distinct " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and  curstatus not in ('archive','rejected') and curstatus in (@status) and  @Dept and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and datediff(dd,convert(date,case " & AndQuery & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=@Days1" _
                        & "  and datediff(dd,convert(date,case " & AndQuery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=@Days2 "

            Dim Alldtlinvageging = "select  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and datediff(dd,convert(date,case " & AndQuery & " end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=@Days1" _
                        & "  and datediff(dd,convert(date,case " & AndQuery & " end ,3) ,(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=@Days2  "
            Dim pit3 As String = ""
            pit3 = query3
            Dim objDC3 As New DataClass()
            Dim ht3 As New Hashtable
            ht3.Add("@Tid", hdndetail.Value)
            ht3.Add("@QueryType", Dropdowntype.SelectedItem.Text)
            ht3.Add("@Query", pit3)
            ht3.Add("@QueryDTLAll", Alldtlinvageging)
            objDC3.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht3)
        ElseIf (DBDETAILNAME = "Invoice Distribution") Then
            Loopquery = "select Distinct DisplayName from mmm_mst_misdb_details where refTid=" & hdndetail.Value & ""
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(Loopquery, con)
            da.Fill(DATACSD)
            Dim dt As New DataTable
            Loopquery = "select Distinct DocumentType from mmm_mst_misdb_details where refTid=" & hdndetail.Value & ""
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(Loopquery, con)
            da.Fill(dt)
            If (dt.Rows.Count > 0) Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    lstDocumenettype.Add(Convert.ToString(dt.Rows(i).Item("DocumentType")))
                Next
            End If
            If (DATACSD.Tables(0).Rows.Count > 0) Then
                For m As Integer = 0 To DATACSD.Tables(0).Rows.Count - 1
                    Dim pds As New DataSet
                    dspname = DATACSD.Tables(0).Rows(m).Item("DisplayName")
                    newQuery = "select Tid,RefTid,DisplayName,DocumentType,fields from mmm_mst_misdb_details where RefTid=" & hdndetail.Value & " and DisplayName='" & dspname & "' and type='" & Dropdowntype.SelectedItem.Text & "'"
                    Dim Dtable As New DataTable
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(newQuery, con)
                    da.Fill(pds)
                    'StrColumn = StrColumn & " end '" & dspname & "'"
                    If (pds.Tables(0).Rows.Count > 0) Then
                        For t As Integer = 0 To pds.Tables(0).Rows.Count - 1
                            std = pds.Tables(0).Rows(t).Item("fields")
                            ddoctype = pds.Tables(0).Rows(t).Item("DocumentType")
                            Displayname1 = pds.Tables(0).Rows(t).Item("DisplayName")
                            newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype,DDLlookupValuesource from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & Session("Eid") & " and isactive=1"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQuery, con)
                            predata.Clear()
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(0).Item("dropdown") & "'," & std & ")"
                                ElseIf (predata.Tables(0).Rows(0).Item("FieldType").ToString.ToUpper = "LOOKUPDDL") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.Get_LookupddlValue(" & Session("Eid") & ",'" & ddoctype & "'," & std & ",'" & std & "')"
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
                Dim newQueryforall1 As String = ""
                Dim CDATA As New DataSet
                Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdndetail.Value & " and type='" & Dropdowntype.SelectedItem.Text & "'"
                Dim tabledata As New DataTable
                Dim SETDATA As New DataSet
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(dtlDeptquery, con)
                da.Fill(SETDATA)
                If (SETDATA.Tables(0).Rows.Count > 0) Then
                    'For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                    '    Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                    '    Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                    '    newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & Session("Eid") & " and isactive=1"
                    '    con = New SqlConnection(conStr)
                    '    da = New SqlDataAdapter(newQueryforall1, con)
                    '    da.Fill(CDATA)
                    '    If (CDATA.Tables(0).Rows.Count > 0) Then
                    '        If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                    '            ANDDEPT = ANDDEPT & " dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")=@dept "
                    '            If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                    '                ANDDEPT = ANDDEPT & " or "
                    '            End If
                    '        End If

                    '    End If
                    'Next
                    ANDDEPT = " @dept"
                End If
            End If

            Dim query = "select distinct  " & s & " " _
                    & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                    & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                    & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and @Date and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                    & " and (" & ANDDEPT & ") "

            Dim foralldetail = "select  " & s & " " _
                    & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                    & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                    & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                    & " and @Date"

            Dim pit As String = ""
            pit = query
            Dim objDC1 As New DataClass()
            Dim ht1 As New Hashtable
            ht1.Add("@Tid", hdndetail.Value)
            ht1.Add("@QueryType", Dropdowntype.SelectedItem.Text)
            ht1.Add("@Query", pit)
            ht1.Add("@QueryDTLAll", foralldetail)
            objDC1.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht1)
            ElseIf (DBDETAILNAME = "SLA Performance") Then

                Loopquery = "select Distinct DisplayName from mmm_mst_misdb_details where refTid=" & hdndetail.Value & ""
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(Loopquery, con)
                da.Fill(DATACSD)
                If (DATACSD.Tables(0).Rows.Count > 0) Then
                    For m As Integer = 0 To DATACSD.Tables(0).Rows.Count - 1
                        Dim pds As New DataSet

                        dspname = DATACSD.Tables(0).Rows(m).Item("DisplayName")
                        newQuery = "select Tid,RefTid,DisplayName,DocumentType,fields from mmm_mst_misdb_details where RefTid=" & hdndetail.Value & " and DisplayName='" & dspname & "' and type='" & Dropdowntype.SelectedItem.Text & "'"
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
                            predata.Clear()
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(0).Item("dropdown") & "'," & std & ")"
                                ElseIf (predata.Tables(0).Rows(0).Item("FieldType").ToString.ToUpper = "LOOKUPDDL") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.Get_LookupddlValue(" & Session("Eid") & ",'" & ddoctype & "'," & std & ",'" & std & "')"
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
            Dim query = "select distinct  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],d.tid[DocID] from mmm_mst_doc d" _
                        & " join mmm_doc_dtl dt on dt.docid=d.tid" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected'  and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and @SLA and @Date and @Dept and dt.aprstatus in (@status) and @curstatus"


                Dim foralldetail = "select  " & s & " " _
                            & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                            & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                            & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                            & " and @Date"
                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdndetail.Value)
                ht1.Add("@QueryType", Dropdowntype.SelectedItem.Text)
                ht1.Add("@Query", pit)
                ht1.Add("@QueryDTLAll", foralldetail)
                objDC1.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht1)
            ElseIf (DBDETAILNAME = "Supplier Spend Breakup") Then
                Loopquery = "select Distinct DisplayName from mmm_mst_misdb_details where refTid=" & hdndetail.Value & ""
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(Loopquery, con)
                da.Fill(DATACSD)
                If (DATACSD.Tables(0).Rows.Count > 0) Then
                    For m As Integer = 0 To DATACSD.Tables(0).Rows.Count - 1
                        Dim pds As New DataSet

                        dspname = DATACSD.Tables(0).Rows(m).Item("DisplayName")
                        newQuery = "select Tid,RefTid,DisplayName,DocumentType,fields from mmm_mst_misdb_details where RefTid=" & hdndetail.Value & " and DisplayName='" & dspname & "' and type='" & Dropdowntype.SelectedItem.Text & "'"
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
                            predata.Clear()
                                da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(0).Item("dropdown") & "'," & std & ")"
                                ElseIf (predata.Tables(0).Rows(0).Item("FieldType").ToString.ToUpper = "LOOKUPDDL") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.Get_LookupddlValue(" & Session("Eid") & ",'" & ddoctype & "'," & std & ",'" & std & "')"
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
                    Dim newQueryforall1 As String = ""
                    Dim CDATA As New DataSet
                    Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdndetail.Value & " and type='" & Dropdowntype.SelectedItem.Text & "'"
                    Dim tabledata As New DataTable
                    Dim SETDATA As New DataSet
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(dtlDeptquery, con)
                    da.Fill(SETDATA)
                    If (SETDATA.Tables(0).Rows.Count > 0) Then
                        For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                            Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                            Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                            newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & Session("Eid") & " and isactive=1"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(newQueryforall1, con)
                            da.Fill(CDATA)
                        If (CDATA.Tables(0).Rows.Count > 0) Then
                            If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                ANDDEPT = ANDDEPT & " (dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")=@vendor and documenttype='" & SETDATA.Tables(0).Rows(MI).Item("DocumentType") & "' )"
                                If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                                    ANDDEPT = ANDDEPT & " or "
                                End If
                            End If
                        End If
                        Next
                    End If
                End If

                Dim query = "select  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and @Date and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and (" & ANDDEPT & ")"

                Dim foralldetail = "select  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and @Date"

                Dim pit As String = ""
                pit = query
                Dim objDC1 As New DataClass()
                Dim ht1 As New Hashtable
                ht1.Add("@Tid", hdndetail.Value)
                ht1.Add("@QueryType", Dropdowntype.SelectedItem.Text)
                ht1.Add("@Query", pit)
                ht1.Add("@QueryDTLAll", foralldetail)
                objDC1.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht1)
        Else
            Loopquery = "select Distinct DisplayName from mmm_mst_misdb_details where refTid=" & hdndetail.Value & ""
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(Loopquery, con)
            da.Fill(DATACSD)
            If (DATACSD.Tables(0).Rows.Count > 0) Then
                For m As Integer = 0 To DATACSD.Tables(0).Rows.Count - 1
                    Dim pds As New DataSet

                    dspname = DATACSD.Tables(0).Rows(m).Item("DisplayName")
                    newQuery = "select Tid,RefTid,DisplayName,DocumentType,fields from mmm_mst_misdb_details where RefTid=" & hdndetail.Value & " and DisplayName='" & dspname & "' and type='" & Dropdowntype.SelectedItem.Text & "'"
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
                            predata.Clear()
                            da.Fill(predata)
                            If (predata.Tables(0).Rows.Count > 0) Then
                                If (predata.Tables(0).Rows(0).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(0).Item("dropdown") & "'," & std & ")"
                                ElseIf (predata.Tables(0).Rows(0).Item("FieldType").ToString.ToUpper = "LOOKUPDDL") Then
                                    StrColumn = StrColumn & " when documenttype='" & ddoctype & "' then dms.Get_LookupddlValue(" & Session("Eid") & ",'" & ddoctype & "'," & std & ",'" & std & "')"
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
                Dim newQueryforall1 As String = ""
                Dim CDATA As New DataSet
                Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=" & hdndetail.Value & " and type='" & Dropdowntype.SelectedItem.Text & "'"
                Dim tabledata As New DataTable
                Dim SETDATA As New DataSet
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(dtlDeptquery, con)
                da.Fill(SETDATA)
                If (SETDATA.Tables(0).Rows.Count > 0) Then
                    For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                        Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                        Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                        newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & Session("Eid") & " and isactive=1"
                        con = New SqlConnection(conStr)
                        da = New SqlDataAdapter(newQueryforall1, con)
                        da.Fill(CDATA)
                        If (CDATA.Tables(0).Rows.Count > 0) Then
                            If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                ANDDEPT = ANDDEPT & "( dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")=@dept  and documenttype='" & CDATA.Tables(0).Rows(MI).Item("documenttype") & "')"
                                If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                                    ANDDEPT = ANDDEPT & " or "
                                End If
                            End If

                        End If
                    Next
                End If
            End If
            Dim query = "select distinct  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and @Date and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and (" & ANDDEPT & ")"
            Dim foralldetail = "select  " & s & " " _
                        & "Curstatus[Current Status],(select replace(convert(varchar(50),convert(date,max(fdate),3),106),' ','-') " _
                        & " from mmm_doc_dtl with(nolock) where docid=d.tid)[Last Action Date],tid[DocID] from mmm_mst_doc d" _
                        & " where Eid=" & Session("Eid") & " and curstatus<>'rejected' and  documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')" _
                        & " and @Date"

            Dim pit As String = ""
            pit = query
            Dim objDC1 As New DataClass()
            Dim ht1 As New Hashtable
            ht1.Add("@Tid", hdndetail.Value)
            ht1.Add("@QueryType", Dropdowntype.SelectedItem.Text)
            ht1.Add("@Query", pit)
            ht1.Add("@QueryDTLAll", foralldetail)
            objDC1.ExecuteProDT("UpdateDBConfigurationLavelQuery", ht1)
            End If
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
            cmd.Parameters.Add("Eid", SqlDbType.Int).Value = Session("Eid")
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Dropdowntype.SelectedItem.Text
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
        Dim row1 As GridViewRow = CType(CType(sender, ImageButton).Parent.Parent, GridViewRow)
        'Dim pid As Integer = Convert.ToString(Me.detailgriditem.DataKeys(row.RowIndex).Value)
        Dim pid As Integer = row1.Cells(2).Text
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
                cmd.CommandText = ("select  a.Tid,a.RefTid,a.DisplayName,a.DocumentType,a.Fields from mmm_mst_misdb_details a where a.Tid=" & ChildId & " and  a.DocumentType='" & dropdownvalue & "' and a.RefTid=" & pid)
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
    Protected Sub imgDeletechild_Click(sender As Object, e As ImageClickEventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim sid As Integer = Convert.ToInt64(Me.gridchilditemdata.DataKeys(row.RowIndex).Value)
        Dim row1 As GridViewRow = CType(CType(sender, ImageButton).Parent.Parent, GridViewRow)
        'Dim pid As Integer = Convert.ToString(Me.detailgriditem.DataKeys(row.RowIndex).Value)
        Dim pid As Integer = row1.Cells(1).Text
        Dim tid1 As String = ""
        ViewState("sid") = sid
        hdnchilddetailReftid.Value = sid
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                conn.Open()
                Dim mycommand = New SqlCommand("Delete from  mmm_mst_misdb_dtl where  Tid=" & pid & " ", conn)
                mycommand.ExecuteNonQuery()
                conn.Close()
                Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim conm As New SqlConnection(strConnString)
                Dim cmd1 As New SqlCommand()
                Div2.Visible = False
                Label2.Text = ""
                cmd1.CommandType = CommandType.StoredProcedure
                cmd1.CommandText = "uspGetDBchildDetail"
                cmd1.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnchilddetailReftid.Value
                cmd1.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
                cmd1.Parameters.Add("@Type", SqlDbType.NVarChar).Value = ddltype.SelectedItem.Text
                cmd1.Connection = conm
                Try
                    conm.Open()
                    gridchilditemdata.EmptyDataText = "No Records Found"
                    gridchilditemdata.DataSource = cmd1.ExecuteReader()
                    gridchilditemdata.DataBind()
                Catch ex As Exception
                    Throw ex
                Finally
                    conm.Close()
                    conm.Dispose()
                End Try
            End Using
        End Using
    End Sub
    Protected Sub imgDetailItemDelete_Click(sender As Object, e As ImageClickEventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim showdetailitem As ImageButton = TryCast(sender, ImageButton)
        Dim row1 As GridViewRow = DirectCast(showdetailitem.NamingContainer, GridViewRow)
        Dim RefTid As Integer = Convert.ToString(Me.gvDashboardData.DataKeys(row1.RowIndex).Value)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim sid As Integer = Convert.ToInt64(Me.detailgriditem.DataKeys(row.RowIndex).Value)
        ViewState("sid") = sid
        'Dim pid As Integer = Convert.ToString(Me.detailgriditem.DataKeys(row.RowIndex).Value)
        Dim pid As Integer = row1.Cells(1).Text
        hdngridviewdetailReftid.Value = RefTid
        'hdnchilddetailReftid.Value = sid
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                conn.Open()
                Dim mycommand = New SqlCommand("Delete from  mmm_mst_misdb_details where  Tid='" & pid & "'", conn)
                mycommand.ExecuteNonQuery()
                conn.Close()
                Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim conm As New SqlConnection(strConnString)
                Dim cmd1 As New SqlCommand()
                Div2.Visible = False
                Label2.Text = ""
                cmd1.CommandType = CommandType.StoredProcedure
                cmd1.CommandText = "uspGetDBDetailitemvalue"
                cmd1.Parameters.Add("@sValue", SqlDbType.Int).Value = hdngridviewdetailReftid.Value
                cmd1.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
                cmd1.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Dropdowntype.SelectedItem.Text
                cmd1.Connection = conm
                Try
                    conm.Open()
                    detailgriditem.EmptyDataText = "No Records Found"
                    detailgriditem.DataSource = cmd1.ExecuteReader()
                    detailgriditem.DataBind()
                Catch ex As Exception
                    Throw ex
                Finally
                    conm.Close()
                    conm.Dispose()
                End Try
            End Using
        End Using
    End Sub
    Protected Sub ddltype_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        Div2.Visible = False
        Label2.Text = ""
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBchildDetail"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdnrefid.Value
        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
        cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = ddltype.SelectedItem.Text
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


    End Sub
    Protected Sub Dropdowntype_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()

        Div2.Visible = False
        Label2.Text = ""

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDBDetailitemvalue"
        cmd.Parameters.Add("@sValue", SqlDbType.Int).Value = hdndetail.Value
        cmd.Parameters.Add("Eid", SqlDbType.Int).Value = Session("Eid")
        cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Dropdowntype.SelectedItem.Text
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
    End Sub
    Protected Sub ddlPofields_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            Nonpovaluefield()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Protected Sub ddlinvtypevendor_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddldbname.Text = "Invoice Distribution" Then
            If ddlinvtypevendor.SelectedItem.Text.ToUpper = "INVOICE TYPE" Then
                InvType.Visible = True
                FillInvTypeID()
            Else
                InvType.Visible = False
            End If
        End If
    End Sub
    Private Sub FillInvTypeID()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select Tid,fld1 from mmm_mst_Master where eid=" & Session("EID") & " and DocumentType like 'INVOICE TYPE%' ")
        If objDT.Rows.Count > 0 Then
            ddlpoType.DataSource = objDT
            ddlpoType.DataTextField = "fld1"
            ddlpoType.DataValueField = "tid"
            ddlpoType.DataBind()
            ddlpoType.Items.Insert(0, "Select")

            ddlnonpotype.DataSource = objDT
            ddlnonpotype.DataTextField = "fld1"
            ddlnonpotype.DataValueField = "tid"
            ddlnonpotype.DataBind()
            ddlnonpotype.Items.Insert(0, "Select")
        Else
            ddlpoType.Items.Clear()
            ddlnonpotype.Items.Clear()
            ddlpoType.Items.Insert(0, "Select")
            ddlnonpotype.Items.Insert(0, "Select")
        End If
    End Sub


End Class
