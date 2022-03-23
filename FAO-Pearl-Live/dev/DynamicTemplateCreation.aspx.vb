Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class DynamicTemplateCreation
    Inherits System.Web.UI.Page
    Dim objDC As New DataClass()
    Dim SLADTColumn As String = ""
    Dim ar1 As ArrayList = New ArrayList()
    Dim ar2 As ArrayList = New ArrayList()
    Dim RTID As Integer
    Dim pid As Integer
    Shared doctypechild As String = ""
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

    Protected Sub BindGrid()
        Dim objDT As New DataTable
        Dim DIFFRTI As Integer
        If (btndatediffhdn.Value <> "") Then
            DIFFRTI = btndatediffhdn.Value
        Else
            objDT = objDC.ExecuteQryDT("SELECT isnull(MAX(Tid),0) + 1 as TID FROM mmm_rep_template_config where eid=" & Session("Eid") & "")
            If objDT.Rows.Count > 0 Then

                DIFFRTI = objDT.Rows(0).Item("Tid")
            End If
        End If
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGettemplateDiffrenceNew"
        cmd.Parameters.AddWithValue("@RefTid", SqlDbType.Int).Value = DIFFRTI
        cmd.Parameters.AddWithValue("@Eid", SqlDbType.Int).Value = Session("Eid")
        'cmd.Parameters.AddWithValue("@Type", "F")
        cmd.Parameters.AddWithValue("@documenttype", ddlDocumenttype.SelectedItem.Text.Trim())
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
    End Sub
    Protected Sub Insert(sender As Object, e As EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim PI As Integer
        Dim Tb As String = ""
        Dim myCounter As Integer
        Dim objDT As New DataTable
        If (btndatediffhdn.Value <> "") Then
            RTID = btndatediffhdn.Value
        Else
            objDT = objDC.ExecuteQryDT("SELECT isnull(MAX(Tid),0) + 1 as TID FROM mmm_rep_template_config where eid=" & Session("Eid") & "")
            If objDT.Rows.Count > 0 Then
                RTID = objDT.Rows(0).Item("Tid")
            End If
        End If
        Dim fType As String = ""
        Dim SType As String = ""
        If ddldatetypefield1.SelectedItem.Text.ToUpper.Contains("OUT") Then
            fType = "S"
        Else
            fType = "F"
        End If
        If ddldatetypefield2.SelectedItem.Text.ToUpper.Contains("OUT") Then
            SType = "S"
        Else
            SType = "F"
        End If
        If txtdisplayname.Text = "" Then
            txtdisplayname.Text = "DisplayName"
        End If
        con.Open()
        Dim mss = New SqlCommand("Insert into mmm_rep_template_Diff(RefTid,Eid,Type,First_Field,Second_Field,DisplayName,DocumentType,TypeSecond_Field) values(" & RTID & ",'" & Session("Eid") & "', '" & fType & "', '" & Replace(ddldatetypefield1.SelectedItem.Text, "Out Date", "") & "','" & Replace(ddldatetypefield2.SelectedItem.Text, "Out Date", "") & "','" & txtdisplayname.Text & "','" & ddlDocumenttype.SelectedItem.Text & "','" & SType & "')", con)
        mss.ExecuteNonQuery()
        con.Close()
        Me.BindGrid()
    End Sub

    Private Sub DynamicTemplateCreation_Load(sender As Object, e As EventArgs) Handles Me.Load
        'ddlField.Items.Add()
        If Not Me.IsPostBack Then
            BindDocumentType()
            Dim dt As New DataTable()
            'Dim dt1 As New DataTable()
            'dt.Columns.AddRange(New DataColumn(4) {New DataColumn("DocumentType"), New DataColumn("First_Field"), New DataColumn("Second_Field"), New DataColumn("DisplayName"), New DataColumn("Tid")})
            'ViewState("Detail") = dt
            'ViewState("Item") = dt
            'Me.BindGrid()
            'Me.BindGridstatusdiff()
        End If
    End Sub
    Protected Sub BindDocumentType()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select distinct convert(varchar,FormID)+'-'+formType[FormID], formname,formType from mmm_mst_forms where formsource='MENU DRIVEN' and isactive=1 and eid=" & Session("EID") & " order by formtype,formname --formtype='DOCUMENT' and ")
        If objDT.Rows.Count > 0 Then
            If (ddlDocumenttype.Items.Count() > 0) Then
                ddlDocumenttype.Items.Clear()
            End If
            ddlDocumenttype.DataSource = objDT
            ddlDocumenttype.DataTextField = "formname"
            ddlDocumenttype.DataValueField = "FormID"
            ddlDocumenttype.DataBind()
            ddlDocumenttype.Items.Insert(0, New ListItem("Select", "0"))
        Else
            ddlDocumenttype.Items.Clear()
            ddlDocumenttype.Items.Insert(0, New ListItem("Select", "0"))
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
    Private Sub PopulateDatefieldFilter()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select 'CreationDate'[displayname],'adate'[fieldmapping] union select displayName,fieldmapping from mmm_mst_fields where datatype='Datetime'  and eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'")
        If objDT.Rows.Count > 0 Then
            chkdatefltr.DataSource = objDT
            chkdatefltr.DataTextField = "displayname"
            chkdatefltr.DataValueField = "fieldmapping"
            chkdatefltr.DataBind()
        Else
            chkdatefltr.Items.Clear()
        End If
    End Sub
    Private Sub PopulateFilterField()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  fieldMapping+'-'+dropdown as fieldMapping,displayname from mmm_mst_fields where FieldType='Drop Down' and isactive=1 and DropDownType='MASTER VALUED' and eid=" & Session("EID") & " and DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and DBTableName in ('MMM_MST_MASTER','MMM_MST_DOC') order by displayName")
        If objDT.Rows.Count > 0 Then
            If (ddlFilterfeild1.Items.Count() > 0) Then
                ddlFilterfeild1.Items.Clear()
            End If
            ddlFilterfeild1.DataSource = objDT
            ddlFilterfeild1.DataTextField = "displayName"
            ddlFilterfeild1.DataValueField = "fieldMapping"
            ddlFilterfeild1.DataBind()
            ddlFilterfeild1.Items.Insert(0, New ListItem("Select", "0"))

        Else
            ddlFilterfeild1.Items.Clear()
            ddlFilterfeild1.Items.Insert(0, New ListItem("Select", "0"))
        End If
    End Sub
    Private Sub PopulateFilterField2()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  fieldMapping+'-'+dropdown as fieldMapping,displayname from mmm_mst_fields where FieldType='Drop Down' and isactive=1 and DropDownType='MASTER VALUED' and eid=" & Session("EID") & " and DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and displayname !='" & ddlFilterfeild1.SelectedItem.Text & "' and DBTableName in('MMM_MST_MASTER','MMM_MST_DOC') order by displayName")
        If objDT.Rows.Count > 0 Then
            If (ddlfilterfield2.Items.Count() > 0) Then
                ddlfilterfield2.Items.Clear()
            End If
            ddlfilterfield2.DataSource = objDT
            ddlfilterfield2.DataTextField = "displayName"
            ddlfilterfield2.DataValueField = "fieldMapping"
            ddlfilterfield2.DataBind()
            ddlfilterfield2.Items.Insert(0, New ListItem("Select", "0"))

        Else
            ddlfilterfield2.Items.Clear()
            ddlfilterfield2.Items.Insert(0, New ListItem("Select", "0"))
        End If
    End Sub
    Private Sub PopulateFilterfixval()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select  fieldMapping+'-'+dropdown as fieldMapping,displayname from mmm_mst_fields where FieldType='Drop Down' and DropDownType='Fix VALUED' and eid=" & Session("EID") & " and DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'  order by displayName")
        If objDT.Rows.Count > 0 Then
            If (ddlfixval.Items.Count() > 0) Then
                ddlfixval.Items.Clear()
            End If
            ddlfixval.DataSource = objDT
            ddlfixval.DataTextField = "displayName"
            ddlfixval.DataValueField = "fieldMapping"
            ddlfixval.DataBind()
            ddlfixval.Items.Insert(0, New ListItem("Select", "0"))
        Else
            ddlfixval.Items.Clear()
            ddlfixval.Items.Insert(0, New ListItem("Select", "0"))
        End If
    End Sub
    Private Sub Populateworkflowstatus()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select StatusName  from  [MMM_MST_WORKFLOW_STATUS] where  Documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'  and eid=" & Session("EID") & " union select 'Uploaded'[StatusName] union select 'Rejected'[StatusName] union select 'Archive'[StatusName]  order by StatusName")
        If objDT.Rows.Count > 0 Then
            chkbxFilterStatus.DataSource = objDT
            chkbxFilterStatus.DataTextField = "StatusName"
            chkbxFilterStatus.DataValueField = "StatusName"
            chkbxFilterStatus.DataBind()


        Else
            chkbxFilterStatus.Items.Clear()

        End If
    End Sub
    Private Sub PopulateComputedfeild()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select Statusname,Statusname +' '+'Out Date' as 'Status' from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' order by dord")
        If objDT.Rows.Count > 0 Then
            chkbxcomputedfeilds.DataSource = objDT
            chkbxcomputedfeilds.DataTextField = "Status"
            chkbxcomputedfeilds.DataValueField = "Statusname"
            chkbxcomputedfeilds.DataBind()
        Else
            chkbxcomputedfeilds.Items.Clear()
        End If
    End Sub
    Private Sub PopulatestatusInDate()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select Statusname,Statusname +' '+'In Date' as 'Status' from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' order by dord")
        If objDT.Rows.Count > 0 Then
            chkSID.DataSource = objDT
            chkSID.DataTextField = "Status"
            chkSID.DataValueField = "Statusname"
            chkSID.DataBind()
        Else
            chkSID.Items.Clear()
        End If
    End Sub
    Private Sub PopulateChildItem()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select dropdown[displayName],FieldMapping from MMM_MST_FIELDS where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and fieldtype='Child Item' order by dropdown")
        If objDT.Rows.Count > 0 Then
            ddlchilditem.DataSource = objDT
            ddlchilditem.DataTextField = "displayName"
            ddlchilditem.DataValueField = "FieldMapping"
            ddlchilditem.DataBind()
            ddlchilditem.Items.Insert(0, New ListItem("Select", "0"))
        Else
            ddlchilditem.Items.Clear()
            ddlchilditem.Items.Insert(0, New ListItem("Select", "0"))
        End If
    End Sub
    Private Sub PopulateSortBy()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select displayName,FieldMapping from MMM_MST_FIELDS where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and isActive=1 order by displayName")
        If objDT.Rows.Count > 0 Then
            If (ddlSortBy.Items.Count() > 0) Then
                ddlSortBy.Items.Clear()
            End If
            ddlSortBy.DataSource = objDT
            ddlSortBy.DataTextField = "displayName"
            ddlSortBy.DataValueField = "FieldMapping"
            ddlSortBy.DataBind()
            ddlSortBy.Items.Insert(0, New ListItem("Select", "0"))

        Else
            ddlSortBy.Items.Clear()
            ddlSortBy.Items.Insert(0, New ListItem("Select", "0"))
        End If
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            popAlert.Visible = False
            hdnTID.Value = 0
            BindDocumentType()
            PopulateAllowedRole()
            Populateworkflowstatus()
        Catch ex As Exception
            Throw
        End Try
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub SearchValue(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim objDC As New DataClass()
            Dim ht As New Hashtable

        Catch
        End Try
    End Sub

    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)
        'SLAOutDateQuery(Session("eid"), txtValue.Text)
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        popAlert.Visible = False
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvrptData.DataKeys(row.RowIndex).Value)
        Dim s As String() = New String(49) {}
        Dim p As String() = New String(50) {}
        Dim c As String() = New String(50) {}
        Dim M As String() = New String(50) {}
        Dim Z As String() = New String(50) {}
        Dim stg As String() = New String(50) {}
        Dim X As String() = New String(50) {}
        Dim chld As String() = New String(50) {}

        btnSubmit.Text = "Update"
        ViewState("pid") = pid
        btndatediffhdn.Value = pid
        Dim objDT As New DataTable()
        Dim DTOb As New DataTable()
        Dim CDDT As New DataTable()
        Dim ObDT As New DataTable()

        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                conn.Open()
                Dim fillgridcoomand = New SqlCommand("SELECT Tid, DocumentType,First_field,Second_field,DisplayName,DocumentType,Type,TypeSecond_field FROM mmm_rep_template_Diff where RefTid=" & pid & "", conn)
                Dim ad As New SqlDataAdapter()
                ad = New SqlDataAdapter(fillgridcoomand)
                ad.Fill(DTOb)
                If DTOb.Rows.Count > 0 Then
                    Griddatediff.DataSource = DTOb
                    Griddatediff.DataBind()
                End If
                conn.Close()
                conn.Open()
                cmd.CommandText = ("select * from MMM_REP_TEMPLATE_CONFIG where Tid =" & pid)
                cmd.Connection = conn
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    While sdr.Read()
                        txtTempID.Text = pid
                        hdnTID.Value = pid
                        txtReptName.Text = sdr("Temp_Name")
                        txtdescription.Text = sdr("Temp_Description")
                        ddlDocumenttype.SelectedItem.Text = sdr("DocumentType")
                        ddlDocumenttype_SelectedIndexChanged(Me, EventArgs.Empty)

                        PopulateFilterfixval()
                        ddlfixval.SelectedItem.Text = sdr("FilterField3").ToString
                        ddlfixval_SelectedIndexChanged(Me, EventArgs.Empty)

                        PopulateChildItem()

                        'lstDocfield.Text = sdr("Docfields")
                        s = sdr("Docfields").ToString().Split(",")

                        'Dim length As Integer = s.Length
                        For i As Integer = 0 To s.Length - 1
                            Dim cntry As String = s(i)
                            For j As Integer = 0 To Listbox1.Items.Count - 1
                                If Listbox1.Items(j).Value = s(i) Then
                                    Listbox1.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next

                        Dim ar() As String
                        ar = sdr("Docfields").ToString.Split(",")
                        Dim Lb As New ListBox
                        Listbox2.Items.Clear()

                        For g As Integer = 0 To ar.Length - 1
                            Dim PT As New DataTable
                            Dim Li As New ListItem
                            PT = objDC.ExecuteQryDT("select FieldID,displayname from mmm_mst_fields where FieldID in (" & ar(g).ToString & ")   and eid=" & Session("EID") & "")
                            Li.Text = PT.Rows(0).Item("displayname")
                            Li.Value = PT.Rows(0).Item("FieldID")
                            Listbox2.Items.Add(Li)
                        Next
                        If Not IsDBNull(sdr("ChldDoctype")) Then
                            ddlchilditem.SelectedItem.Text = sdr("ChldDoctype")
                            ddlchilditem_SelectedIndexChanged(Me, EventArgs.Empty)
                        End If


                        'ddlFilterfeild1.Items.Insert(0, "Select")

                        If (sdr("Filterfield1") = "0") Then
                            ddlFilterfeild1.SelectedValue = sdr("Filterfield1")
                        Else
                            For Each Item As ListItem In ddlFilterfeild1.Items
                                If Item.Text.ToString().Contains(sdr("Filterfield1")) Then
                                    Item.Selected = True
                                Else
                                    Item.Selected = False
                                End If
                            Next
                        End If
                        If sdr("SortBy") = "0" Then
                            ddlSortBy.SelectedValue = sdr("SortBy")
                        Else
                            For Each Item As ListItem In ddlSortBy.Items
                                If Item.Value.ToString().Contains(sdr("SortBy")) Then
                                    Item.Selected = True
                                Else
                                    Item.Selected = False
                                End If
                            Next
                        End If
                        ddlFilterfeild1_SelectedIndexChanged(Me, EventArgs.Empty)
                        PopulateFilterField2()
                        If (ddlFilterfeild1.SelectedValue = "0") Then
                            ddlFilterfeild1.ClearSelection()
                        Else
                            p = sdr("FilterfieldsData1").ToString().Split(",")
                            ' Dim length1 As Integer = p.Length
                            For i As Integer = 0 To p.Length - 1
                                Dim cntry As String = p(i)
                                For j As Integer = 0 To ckbxfilterfield.Items.Count - 1
                                    If ckbxfilterfield.Items(j).Value = p(i) Then
                                        ckbxfilterfield.Items(j).Selected = True
                                        Exit For
                                    End If
                                Next
                            Next
                        End If
                        If (sdr("Filterfield2") = "0") Then
                            ddlfilterfield2.SelectedValue = sdr("Filterfield2")
                        Else
                            For Each ItemP As ListItem In ddlfilterfield2.Items
                                If ItemP.Text.ToString().Contains(sdr("Filterfield2")) Then
                                    ItemP.Selected = True
                                Else
                                    ItemP.Selected = False
                                End If
                            Next
                        End If
                        If (ddlfilterfield2.SelectedValue = "0") Then
                            ddlfilterfield2.ClearSelection()
                        Else
                            ddlfilterfield2_SelectedIndexChanged(Me, EventArgs.Empty)
                            M = sdr("FilterfieldData2").ToString().Split(",")
                            'Dim length4 As Integer = M.Length
                            For i As Integer = 0 To M.Length - 1
                                Dim cntry As String = M(i)
                                For j As Integer = 0 To chbxfilterfielddata2.Items.Count - 1
                                    If chbxfilterfielddata2.Items(j).Value = M(i) Then
                                        chbxfilterfielddata2.Items(j).Selected = True
                                        Exit For
                                    End If
                                Next
                            Next
                        End If
                        Populateworkflowstatus()
                        X = sdr("FilterStatus").ToString().Split(",")
                        'Dim length32 As Integer = X.Length
                        For i As Integer = 0 To X.Length - 1
                            ' Dim cntry As String = X(i)
                            For j As Integer = 0 To chkbxFilterStatus.Items.Count - 1
                                If chkbxFilterStatus.Items(j).Value = X(i) Then
                                    chkbxFilterStatus.Items(j).Selected = True
                                End If
                            Next
                        Next
                        'chkbxFilterStatus.SelectedValue = sdr("FilterStatus").ToString()
                        PopulateAllowedRole()
                        c = sdr("AllowedRoles").ToString().Split(",")
                        'Dim length2 As Integer = c.Length
                        For i As Integer = 0 To c.Length - 1
                            ' Dim cntry As String = c(i)
                            For j As Integer = 0 To CheckBoxList1.Items.Count - 1
                                If CheckBoxList1.Items(j).Value = c(i) Then
                                    CheckBoxList1.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next
                        PopulateComputedfeild()
                        Z = sdr("Computedfield").ToString().Split(",")
                        'Dim length10 As Integer = Z.Length
                        For i As Integer = 0 To Z.Length - 1
                            'Dim cntry As String = Z(i)
                            For j As Integer = 0 To chkbxcomputedfeilds.Items.Count - 1
                                If chkbxcomputedfeilds.Items(j).Value = Z(i) Then
                                    chkbxcomputedfeilds.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next

                        PopulatestatusInDate()
                        stg = sdr("StagesInDate").ToString().Split(",")
                        'Dim length11 As Integer = stg.Length
                        For i As Integer = 0 To stg.Length - 1
                            'Dim cntry As String = stg(i)
                            For j As Integer = 0 To chkSID.Items.Count - 1
                                If chkSID.Items(j).Value = stg(i) Then
                                    chkSID.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next

                        PopulateDatefieldFilter()
                        stg = sdr("DateFilterFields").ToString().Split(",")
                        ' Dim length12 As Integer = stg.Length
                        For i As Integer = 0 To stg.Length - 1
                            'Dim cntry As String = stg(i)
                            For j As Integer = 0 To chkdatefltr.Items.Count - 1
                                If chkdatefltr.Items(j).Value = stg(i) Then
                                    chkdatefltr.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next


                        stg = sdr("FilterFielddata3").ToString().Split(",")
                        'Dim length13 As Integer = stg.Length
                        For i As Integer = 0 To stg.Length - 1
                            'Dim cntry As String = stg(i)
                            For j As Integer = 0 To chkfixval.Items.Count - 1
                                If chkfixval.Items(j).Value = stg(i) Then
                                    chkfixval.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next


                        If sdr("LastWFS").ToString = "1" Then
                            chkLWFS.Checked = True
                        Else
                            chkLWFS.Checked = False
                        End If
                        If sdr("LastUN").ToString = "1" Then
                            chkLUN.Checked = True
                        Else
                            chkLUN.Checked = False
                        End If
                        If sdr("LastAD").ToString = "1" Then
                            chkLAD.Checked = True
                        Else
                            chkLAD.Checked = False
                        End If
                        If sdr("CurrentUser").ToString = "1" Then
                            chkCUN.Checked = True
                        Else
                            chkCUN.Checked = False
                        End If
                        If sdr("CreationDateFlag").ToString = "1" Then
                            chkCD.Checked = True
                        Else
                            chkCD.Checked = False
                        End If
                    End While
                    Dim dataTable As DataTable = New DataTable()

                End Using
                conn.Close()
            End Using
            updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Show()
        End Using
    End Sub
    Protected Sub ddlDocumenttype_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateFilterField()
        PopulateFilterField2()
        PopulateFilterfixval()
        PopulateDatefieldFilter()
        Populateworkflowstatus()
        PopulateDateDiffBy1()
        'Populatesattusdiff()
        PopulateChildItem()
        Me.BindGrid()
        'Me.BindGridstatusdiff()
        Dim objDT As New DataTable
        If ddlDocumenttype.SelectedItem.Text.ToUpper <> "SELECT" And btnSubmit.Text.ToUpper = "SAVE" Then
            Dim Type() As String = ddlDocumenttype.SelectedValue.ToString.Split("-")
            ViewState("FormType") = Type(1)
        End If
        objDT = objDC.ExecuteQryDT("select FieldID,displayname from mmm_mst_fields where  DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'  and eid=" & Session("EID") & " and FieldType !='Child Item' order by displayname")
        If objDT.Rows.Count > 0 Then
            Listbox1.DataSource = objDT
            Listbox1.DataTextField = "displayname"
            Listbox1.DataValueField = "FieldID"
            Listbox1.DataBind()
        Else
            Listbox1.Items.Clear()
            'ddlFilterfeild1.Items.Insert(0, "Select")
        End If
        ddlchildlist.Items.Clear()
        PopulateComputedfeild()
        PopulatestatusInDate()
        PopulateSortBy()
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If ddlDocumenttype.SelectedItem.Text = "Select" Then
            Dim sMsg As String = "Please select document type!!"
            ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
            Return
            lblMsgEdit.Text = "Please select document type"
            popAlert.Visible = True
            btnEdit_ModalPopupExtender.Show()
            Return
        End If
        If txtdescription.Text = "" Then
            Dim sMsg As String = "Please Enter Report Description!!"
            ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
            Return
        End If
        If btnSubmit.Text = "Save" Then
            Dim objDT As New DataTable
            objDT = objDC.ExecuteQryDT("select * from mmm_rep_template_config where  DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'  and Temp_Name='" & txtReptName.Text & "'")
            If objDT.Rows.Count > 0 Then
                Dim sMsg As String = "Already Exist!!!!"
                ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
                Return
                lblMsgEdit.Text = "Already Exist!!"
                popAlert.Visible = True
                'btnEdit_ModalPopupExtender.Show()
                Return
            End If
        End If
        Dim i As Integer
        Dim sb As StringBuilder = New StringBuilder()
        For i = 0 To Listbox2.Items.Count - 1
            sb.Append(Listbox2.Items(i).Value & ",")
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim DocType As String = ""
        DocType = Left(sb.ToString(), Len(sb.ToString()) - 1)
        Dim p As Integer
        Dim sb1 As StringBuilder = New StringBuilder()
        For p = 0 To ckbxfilterfield.Items.Count - 1
            If ckbxfilterfield.Items(p).Selected Then
                sb1.Append(ckbxfilterfield.Items(p).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim Filter1 As String = ""
        If sb1.Length > 0 Then
            Filter1 = Left(sb1.ToString(), Len(sb1.ToString()) - 1)
        Else
            Filter1 = Nothing
        End If

        Dim q As Integer
        Dim sb2 As StringBuilder = New StringBuilder()
        For q = 0 To chbxfilterfielddata2.Items.Count - 1
            If chbxfilterfielddata2.Items(q).Selected Then
                sb2.Append(chbxfilterfielddata2.Items(q).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim Filter2 As String = ""
        If sb2.Length > 0 Then
            Filter2 = Left(sb2.ToString(), Len(sb2.ToString()) - 1)
        Else
            Filter2 = Nothing
        End If

        Dim w As Integer
        Dim sb3 As StringBuilder = New StringBuilder()
        For w = 0 To chkbxFilterStatus.Items.Count - 1
            If chkbxFilterStatus.Items(w).Selected Then
                sb3.Append(chkbxFilterStatus.Items(w).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim Filterstatus As String = ""
        If sb3.Length > 0 Then
            Filterstatus = Left(sb3.ToString(), Len(sb3.ToString()) - 1)
        Else
            Filterstatus = Nothing
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
            Dim sMsg As String = "Please select at least one role!!"
            ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
            Return
        End If
        Dim schld As StringBuilder = New StringBuilder
        Dim objDTchld As New DataTable
        Dim DOCNewchld As String = ""
        objDTchld = objDC.ExecuteQryDT("select dropdown,FieldMapping from MMM_MST_FIELDS where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and fieldtype='Child Item' order by dropdown")
        If objDTchld.Rows.Count > 0 Then
            For chld As Integer = 0 To objDTchld.Rows.Count - 1
                DOCNewchld = objDTchld.Rows(chld).Item("dropdown").ToString
                schld.Append(DOCNewchld & ",")
            Next
        End If
        Dim PROCHLD As String = ""
        If schld.Length > 0 Then
            PROCHLD = Left(schld.ToString(), Len(schld.ToString()) - 1)
        Else
            PROCHLD = Nothing
        End If

        Dim t As Integer
        Dim sb5 As StringBuilder = New StringBuilder()

        ''''Stages Out date
        For t = 0 To chkbxcomputedfeilds.Items.Count - 1
            If chkbxcomputedfeilds.Items(t).Selected Then
                sb5.Append(chkbxcomputedfeilds.Items(t).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim ComputedFields As String = ""
        If sb5.Length > 0 Then
            ComputedFields = Left(sb5.ToString(), Len(sb5.ToString()) - 1)
        Else
            ComputedFields = Nothing
        End If

        ''''Stages In Date
        sb5.Clear()
        For t = 0 To chkSID.Items.Count - 1
            If chkSID.Items(t).Selected Then
                sb5.Append(chkSID.Items(t).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim StagesInDate As String = ""
        If sb5.Length > 0 Then
            StagesInDate = Left(sb5.ToString(), Len(sb5.ToString()) - 1)
        Else
            StagesInDate = Nothing
        End If

        '''' Date filter fields
        sb5.Clear()
        For t = 0 To chkdatefltr.Items.Count - 1
            If chkdatefltr.Items(t).Selected Then
                sb5.Append(chkdatefltr.Items(t).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim Filterdate As String = ""
        If sb5.Length > 0 Then
            Filterdate = Left(sb5.ToString(), Len(sb5.ToString()) - 1)
        Else
            Filterdate = "adate"
        End If

        '''' Fix Valued Filter
        sb5.Clear()
        For t = 0 To chkfixval.Items.Count - 1
            If chkfixval.Items(t).Selected Then
                sb5.Append(chkfixval.Items(t).Value & ",")
            End If
        Next
        'Create the value to be inserted by removing the last comma in sb
        Dim FixValued As String = ""
        If sb5.Length > 0 Then
            FixValued = Left(sb5.ToString(), Len(sb5.ToString()) - 1)
        Else
            FixValued = Nothing
        End If

        Dim LastWFS = 0
        Dim LastUN = 0
        Dim LastAD = 0
        Dim CurrentUser = 0
        Dim CreationDateFlag = 0

        If chkLWFS.Checked = True Then
            LastWFS = 1
        End If
        If chkLUN.Checked = True Then
            LastUN = 1
        End If
        If chkLAD.Checked = True Then
            LastAD = 1
        End If
        If chkCD.Checked = True Then
            CurrentUser = 1
        End If
        If chkCUN.Checked = True Then
            CreationDateFlag = 1
        End If

        Try
            Dim objDC As New DataClass()
            Dim ht As New Hashtable
            ht.Add("@Tid", hdnTID.Value)
            ht.Add("@Temp_Name", txtReptName.Text)
            ht.Add("@Documenttype", ddlDocumenttype.SelectedItem.Text.Trim())
            ht.Add("@Docfields", DocType)
            ht.Add("@Computedfield", ComputedFields)
            ht.Add("@SortBy", ddlSortBy.SelectedValue)
            ht.Add("@Filterfield1", ddlFilterfeild1.SelectedItem.Text)
            ht.Add("@FilterfieldsData1", Filter1)
            If ddlfilterfield2.SelectedItem.Text = "Select" Then
                ht.Add("@Filterfield2", ddlfilterfield2.SelectedValue)
            Else
                ht.Add("@Filterfield2", ddlfilterfield2.SelectedItem.Text.Trim)
            End If
            ht.Add("@FilterfieldData2", Filter2)
            ht.Add("@FilterStatus", Filterstatus)
            ht.Add("@AllowedRoles", AllowedRoll)
            ht.Add("@Temp_Description", txtdescription.Text)
            ht.Add("@Eid", Session("Eid"))
            ht.Add("@ChldDoctype", PROCHLD)
            ht.Add("@StagesInDate", StagesInDate)
            ht.Add("@LastWFS", LastWFS)
            ht.Add("@LastUN", LastUN)
            ht.Add("@LastAD", LastAD)
            ht.Add("@CurrentUser", CurrentUser)
            ht.Add("@CreationDateFlag", CreationDateFlag)
            If btnSubmit.Text.ToUpper = "SAVE" Then
                ht.Add("@FormType", ViewState("FormType"))
            Else
            End If
            ht.Add("@FilterField3", ddlfixval.SelectedItem.Text)
            ht.Add("@FilterFieldData3", FixValued)
            ht.Add("@DateFilterField", Filterdate)
            'ht.Add("@CurrentUser", CurrentUser)

            objDC.ExecuteProDT("AddUpdateDynamictemplateCreationNew", ht)
            'Dim objDc As New DataClass()           
            If btnSubmit.Text = "Save" Then
                Dim sMsg As String = "Report has been saved successfully!!"
                ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
            Else
                Dim sMsg As String = "Report has been updated successfully!!"
                ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
                'popAlert.Visible = True
                'Dim sMsg As String = "Updated Successfully!!!"
                'ScriptManager.RegisterStartupScript(Page, Page.GetType, Guid.NewGuid().ToString(), "alert('" & sMsg & "')", True)
                ''lblMsgEdit.Text = "Updated Successfully !!"
                'Response.Redirect(Request.Url.AbsoluteUri)
            End If
            Me.btnEdit_ModalPopupExtender.Hide()
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub ddlFilterfeild1_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim objDT As New DataTable
        If (ddlFilterfeild1.SelectedItem.Text <> "Select") Then
            Dim textMaster As String() = ddlFilterfeild1.SelectedValue.Split("-")
            objDT = objDC.ExecuteQryDT("select  " & textMaster(3) & " as text,tid from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & textMaster(2) & "' and isauth=1")
            If objDT.Rows.Count > 0 Then
                ckbxfilterfield.DataSource = objDT
                ckbxfilterfield.DataTextField = "text"
                ckbxfilterfield.DataValueField = "tid"
                ckbxfilterfield.DataBind()
            End If
        Else
            ckbxfilterfield.Items.Clear()
            ' ddlFilterfeild1.Items.Insert(0, "Select")
        End If
        PopulateFilterField2()
    End Sub
    Protected Sub ddlfilterfield2_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim objDT As New DataTable
        If ddlfilterfield2.SelectedItem.Text = "Select" Then
        Else
            Dim textMaster As String() = ddlfilterfield2.SelectedValue.Split("-")
            objDT = objDC.ExecuteQryDT("select  " & textMaster(3) & " as text,tid from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & textMaster(2) & "' and isauth=1")
            If objDT.Rows.Count > 0 Then
                chbxfilterfielddata2.DataSource = objDT
                chbxfilterfielddata2.DataTextField = "text"
                chbxfilterfielddata2.DataValueField = "tid"
                chbxfilterfielddata2.DataBind()
            Else
                chbxfilterfielddata2.Items.Clear()
                'ddlFilterfeild1.Items.Insert(0, "Select")
            End If
        End If
    End Sub
    Protected Sub ddlfixval_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim fixval As String = ""
        Dim objDT As New DataTable
        If ddlfixval.SelectedItem.Text = "Select" Or ddlfixval.SelectedItem.Text = "" Then
        Else
            Dim txtval() As String
            If btnSubmit.Text.ToUpper = "SAVE" Then
                txtval = ddlfixval.SelectedValue.ToString.Split("-")
            Else
                objDT = objDC.ExecuteQryDT("select  fieldMapping+'-'+dropdown as fieldMapping from mmm_mst_fields where FieldType='Drop Down' and DropDownType='Fix VALUED' and eid=" & Session("EID") & " and DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and displayname='" & ddlfixval.SelectedItem.Text & "'  order by displayName")
                If objDT.Rows.Count > 0 Then
                    fixval = objDT.Rows(0).Item("fieldMapping")
                End If
                txtval = fixval.ToString.Split("-")
            End If
            objDT = objDC.ExecuteQryDT("select  * from dms.inputstring1('" & txtval(1) & "')")
            If objDT.Rows.Count > 0 Then
                chkfixval.DataSource = objDT
                chkfixval.DataTextField = "number"
                chkfixval.DataValueField = "number"
                chkfixval.DataBind()
            Else
                chkfixval.Items.Clear()
                'ddlFilterfeild1.Items.Insert(0, "Select")
            End If
        End If



    End Sub

    Protected Sub right_Click(sender As Object, e As EventArgs)
        If Listbox1.SelectedIndex >= 0 Then
            For i As Integer = 0 To Listbox1.Items.Count - 1
                If Listbox1.Items(i).Selected Then
                    If Not ar1.Contains(Listbox1.Items(i)) Then
                        ar1.Add(Listbox1.Items(i))
                    End If
                End If
            Next
            For i As Integer = 0 To ar1.Count - 1
                If Not Listbox2.Items.Contains((CType(ar1(i), ListItem))) Then
                    Listbox2.Items.Add((CType(ar1(i), ListItem)))
                End If
                Listbox1.Items.Remove((CType(ar1(i), ListItem)))
            Next
            Listbox2.SelectedIndex = -1
        Else
        End If
    End Sub
    Protected Sub leftClick_Click(sender As Object, e As EventArgs)
        If Listbox2.SelectedIndex >= 0 Then
            For i As Integer = 0 To Listbox2.Items.Count - 1
                If Listbox2.Items(i).Selected Then
                    If Not ar2.Contains(Listbox2.Items(i)) Then
                        ar2.Add(Listbox2.Items(i))
                    End If
                End If
            Next
            For i As Integer = 0 To ar2.Count - 1
                If Not Listbox1.Items.Contains((CType(ar2(i), ListItem))) Then
                    Listbox1.Items.Add((CType(ar2(i), ListItem)))
                End If
                Listbox2.Items.Remove((CType(ar2(i), ListItem)))
            Next
            Listbox1.SelectedIndex = -1
        Else
        End If
    End Sub
    Protected Sub Up_Click(sender As Object, e As EventArgs)
        Dim item As Object = Listbox2.SelectedItem
        If Not item Is Nothing Then
            Dim index As Integer = Listbox2.Items.IndexOf(item)
            If index > 0 Then
                Listbox2.Items.RemoveAt(index)
                index -= 1
                Listbox2.Items.Insert(index, item)
                Listbox2.SelectedIndex = index
            End If
        End If
    End Sub
    Protected Sub downArrow_Click(sender As Object, e As EventArgs)
        Dim item As Object = Listbox2.SelectedItem
        If Not item Is Nothing Then
            Dim index As Integer = Listbox2.Items.IndexOf(item)
            If index < Listbox2.Items.Count - 1 Then
                Listbox2.Items.RemoveAt(index)
                index += 1
                Listbox2.Items.Insert(index, item)
                Listbox2.SelectedIndex = index
            End If
        End If
    End Sub
    Private Sub PopulateDateDiffBy1()
        Dim objDT As New DataTable
        'objDT = objDC.ExecuteQryDT("select displayName,FieldMapping from mmm_mst_fields where datatype='Datetime' and eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'  order by displayName")
        objDT = objDC.ExecuteQryDT("select displayName,FieldMapping from(select displayName,displayname[FieldMapping] from mmm_mst_fields where datatype='Datetime'  and eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' union select Statusname +' '+'Out Date',Statusname  as 'Status' from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "') as t  ")
        If objDT.Rows.Count > 0 Then
            If (ddldatetypefield1.Items.Count() > 0) Then
                ddldatetypefield1.Items.Clear()
            End If
            ddldatetypefield1.DataSource = objDT
            ddldatetypefield1.DataTextField = "displayName"
            ddldatetypefield1.DataValueField = "FieldMapping"
            ddldatetypefield1.DataBind()
            ddldatetypefield1.Items.Insert(0, "Select")
        Else
            ddldatetypefield1.Items.Clear()
            ddldatetypefield1.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub ddldatetypefield1_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim objDT As New DataTable
        ' objDT = objDC.ExecuteQryDT("select displayName,FieldMapping from mmm_mst_fields where datatype='Datetime' and eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and displayName !='" & ddldatetypefield1.SelectedItem.Text & "'  order by displayName")
        objDT = objDC.ExecuteQryDT("select displayName,FieldMapping from(select displayName,displayname[FieldMapping] from mmm_mst_fields where datatype='Datetime'  and eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' union select Statusname +' '+'Out Date',Statusname from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' ) as t where FieldMapping !='" & ddldatetypefield1.SelectedValue.ToString & "' ")
        If objDT.Rows.Count > 0 Then
            If (ddldatetypefield2.Items.Count() > 0) Then
                ddldatetypefield2.Items.Clear()
            End If
            ddldatetypefield2.DataSource = objDT
            ddldatetypefield2.DataTextField = "displayName"
            ddldatetypefield2.DataValueField = "FieldMapping"
            ddldatetypefield2.DataBind()
            ddldatetypefield2.Items.Insert(0, "Select")
        Else
            ddldatetypefield2.Items.Clear()
            ddldatetypefield2.Items.Insert(0, "Select")
        End If
    End Sub
    'Private Sub Populatesattusdiff()
    '    Dim objDT As New DataTable
    '    objDT = objDC.ExecuteQryDT("select Statusname,Statusname +' '+'Out Date' as 'Status' from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' order by dord")
    '    If objDT.Rows.Count > 0 Then
    '        If (ddlstatusdiff1.Items.Count() > 0) Then
    '            ddlstatusdiff1.Items.Clear()
    '        End If
    '        ddlstatusdiff1.DataSource = objDT
    '        ddlstatusdiff1.DataTextField = "Status"
    '        ddlstatusdiff1.DataValueField = "Statusname"
    '        ddlstatusdiff1.DataBind()
    '        ddlstatusdiff1.Items.Insert(0, "Select")
    '    Else
    '        ddlstatusdiff1.Items.Clear()
    '        ddlstatusdiff1.Items.Insert(0, "Select")
    '    End If
    'End Sub
    'Protected Sub ddlstatusdiff1_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim objDT As New DataTable
    '    objDT = objDC.ExecuteQryDT("select Statusname,Statusname +' '+'Out Date' as 'Status' from mmm_mst_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and Statusname !='" & ddlstatusdiff1.SelectedValue & "' order by dord")
    '    If objDT.Rows.Count > 0 Then
    '        If (ddlstatusdiff2.Items.Count() > 0) Then
    '            ddlstatusdiff2.Items.Clear()
    '        End If
    '        ddlstatusdiff2.DataSource = objDT
    '        ddlstatusdiff2.DataTextField = "Status"
    '        ddlstatusdiff2.DataValueField = "Statusname"
    '        ddlstatusdiff2.DataBind()
    '        ddlstatusdiff2.Items.Insert(0, "Select")
    '    Else
    '        ddlstatusdiff2.Items.Clear()
    '        ddlstatusdiff2.Items.Insert(0, "Select")
    '    End If
    'End Sub
    'Protected Sub BindGridstatusdiff()
    '    Dim objDT As New DataTable
    '    Dim DIFFRTI As Integer
    '    If btndatediffhdn.Value <> "" Then
    '        DIFFRTI = btndatediffhdn.Value
    '    Else
    '        objDT = objDC.ExecuteQryDT("SELECT isnull(MAX(Tid),0) + 1 as TID FROM mmm_rep_template_config where eid=" & Session("Eid") & "")
    '        If objDT.Rows.Count > 0 Then
    '            DIFFRTI = objDT.Rows(0).Item("Tid")
    '        End If
    '    End If
    '    Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(strConnString)
    '    Dim cmd As New SqlCommand()
    '    cmd.CommandType = CommandType.StoredProcedure
    '    cmd.CommandText = "uspGettemplateDiffrence"
    '    cmd.Parameters.AddWithValue("@RefTid", SqlDbType.Int).Value = DIFFRTI
    '    cmd.Parameters.AddWithValue("@Eid", SqlDbType.Int).Value = Session("Eid")
    '    cmd.Parameters.AddWithValue("@Type", "S")
    '    cmd.Parameters.AddWithValue("@documenttype", ddlDocumenttype.SelectedItem.Text.Trim())
    '    cmd.Connection = con
    '    Try
    '        con.Open()
    '        gvrstatusdiff.EmptyDataText = "No Records Found"
    '        gvrstatusdiff.DataSource = cmd.ExecuteReader()
    '        gvrstatusdiff.DataBind()
    '    Catch ex As Exception
    '        Throw ex
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '    End Try
    'End Sub
    'Protected Sub InsertStatus(sender As Object, e As EventArgs)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim PI As Integer
    '    Dim Tb As String = ""
    '    Dim myCounter As Integer
    '    Dim objDT As New DataTable
    '    If btndatediffhdn.Value <> "" Then
    '        RTID = btndatediffhdn.Value
    '    Else
    '        objDT = objDC.ExecuteQryDT("SELECT isnull(MAX(Tid),0) + 1 as TID FROM mmm_rep_template_config where eid=" & Session("Eid") & "")
    '        If objDT.Rows.Count > 0 Then
    '            RTID = objDT.Rows(0).Item("Tid")
    '        End If
    '    End If
    '    con.Open()
    '    Dim mss = New SqlCommand("Insert into mmm_rep_template_Diff(RefTid,Eid,Type,First_Field,Second_Field,DisplayName,DocumentType) values('" & RTID & "','" & Session("Eid") & "','S','" & ddlstatusdiff1.SelectedValue & "','" & ddlstatusdiff2.SelectedValue & "','" & txtdisplaynamestatusdiff.Text.Trim() & "','" & ddlDocumenttype.SelectedItem.Text & "')", con)
    '    mss.ExecuteNonQuery()
    '    con.Close()
    '    Me.BindGridstatusdiff()
    'End Sub
    'Protected Sub Deleteimg_Click(sender As Object, e As ImageClickEventArgs)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim sid As Integer = Convert.ToString(Me.gvrstatusdiff.DataKeys(row.RowIndex).Value)
    '    ViewState("sid") = sid
    '    Using conn As SqlConnection = New SqlConnection()
    '        conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
    '        Using cmd As New SqlCommand()
    '            Dim vLoop As Integer = 0
    '            Do While (vLoop < gvrstatusdiff.Rows.Count)
    '                Dim gvrow As GridViewRow = gvrstatusdiff.Rows(vLoop)
    '                Dim firstcoltext As String = gvrow.Cells(3).Text
    '                Dim secondcoltext As String = gvrow.Cells(4).Text
    '                Dim dpname As String = gvrow.Cells(5).Text
    '                Dim typedoc As String = gvrow.Cells(2).Text
    '                vLoop = (vLoop + 1)
    '                conn.Open()
    '                Dim mycommand = New SqlCommand("Delete from  mmm_rep_template_Diff where Tid=" & sid & " and first_field='" & firstcoltext & "' and second_field='" & secondcoltext & "' and Documenttype='" & typedoc & "' and displayname='" & dpname & "'", conn)
    '                mycommand.ExecuteNonQuery()
    '                conn.Close()
    '                BindGridstatusdiff()
    '            Loop
    '        End Using
    '    End Using
    'End Sub
    Protected Sub imgDelete_Click(sender As Object, e As ImageClickEventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim sid As Integer = Convert.ToInt64(Me.Griddatediff.DataKeys(row.RowIndex).Value)
        ViewState("sid") = sid
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                Dim vLoop As Integer = 0
                Do While (vLoop < Griddatediff.Rows.Count)
                    Dim gvrow As GridViewRow = Griddatediff.Rows(vLoop)
                    Dim firstcoltext As String = gvrow.Cells(3).Text
                    Dim secondcoltext As String = gvrow.Cells(4).Text
                    Dim dpname As String = gvrow.Cells(5).Text
                    Dim typedoc As String = gvrow.Cells(2).Text
                    vLoop = (vLoop + 1)
                    conn.Open()
                    Dim mycommand = New SqlCommand("Delete from  mmm_rep_template_Diff where  Eid='" & Session("Eid") & "' and Tid=" & sid & " and first_field='" & firstcoltext & "' and second_field='" & secondcoltext & "' and Documenttype='" & typedoc & "' and displayname='" & dpname & "'", conn)
                    mycommand.ExecuteNonQuery()
                    conn.Close()
                    BindGrid()
                Loop
            End Using
        End Using
    End Sub
    Protected Sub firstbindgrid()
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(strConnString)
        Dim cmd As New SqlCommand()
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "uspGetDynamicformDetail"
        cmd.Parameters.Add("@sValue", SqlDbType.NVarChar).Value = txtValue.Text
        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = Session("Eid")
        cmd.Connection = con
        Dim objDT As New DataTable
        Try
            con.Open()
            objDT.Load(cmd.ExecuteReader())
            'gvrptData.EmptyDataText = "No Records Found"
            gvrptData.DataSource = objDT
            gvrptData.DataBind()
        Catch ex As Exception
            Throw ex
        Finally
            con.Close()
            con.Dispose()

        End Try
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs)
        firstbindgrid()
    End Sub
    Protected Sub gvrptData_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvrptData.PageIndex = e.NewPageIndex
        firstbindgrid()
    End Sub
    Protected Sub ddlchilditem_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim objDT As New DataTable
        Try
            objDT = objDC.ExecuteQryDT("select displayName,FieldID from MMM_MST_FIELDS where eid=" & Session("EID") & " and documenttype='" & ddlchilditem.SelectedItem.Text.Trim() & "'  order by displayName")
            If objDT.Rows.Count > 0 Then
                ddlchildlist.DataSource = objDT
                ddlchildlist.DataTextField = "displayName"
                ddlchildlist.DataValueField = "FieldID"
                ddlchildlist.DataBind()
            Else
                ddlchildlist.Items.Clear()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Protected Sub btnrightchild_Click(sender As Object, e As EventArgs)
        If Listbox2.SelectedIndex >= 0 Then
            For i As Integer = 0 To Listbox2.Items.Count - 1
                If Listbox2.Items(i).Selected Then
                    If Not ar2.Contains(Listbox2.Items(i)) Then
                        ar2.Add(Listbox2.Items(i))
                    End If
                End If
            Next
            For i As Integer = 0 To ar2.Count - 1
                If Not ddlchildlist.Items.Contains((CType(ar2(i), ListItem))) Then
                    ddlchildlist.Items.Add((CType(ar2(i), ListItem)))
                End If
                Listbox2.Items.Remove((CType(ar2(i), ListItem)))
            Next
            ddlchildlist.SelectedIndex = -1
        Else
        End If
    End Sub
    Protected Sub btnleftchild_Click(sender As Object, e As EventArgs)
        If ddlchildlist.SelectedIndex >= 0 Then
            For i As Integer = 0 To ddlchildlist.Items.Count - 1
                If ddlchildlist.Items(i).Selected Then
                    If Not ar1.Contains(ddlchildlist.Items(i)) Then
                        ar1.Add(ddlchildlist.Items(i))
                    End If
                End If
            Next
            For i As Integer = 0 To ar1.Count - 1
                If Not Listbox2.Items.Contains((CType(ar1(i), ListItem))) Then
                    Listbox2.Items.Add((CType(ar1(i), ListItem)))
                End If
                ddlchildlist.Items.Remove((CType(ar1(i), ListItem)))
            Next
            Listbox2.SelectedIndex = -1
        Else
        End If
    End Sub
End Class
