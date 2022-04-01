Imports System.Data
Imports System.Data.SqlClient

Partial Class ReportBuilder
    Inherits System.Web.UI.Page


    Dim stradd As String = ""
    Dim actualval As String = ""
    Dim strOr As String = ""

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
    Public Sub BinddataGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT reportid,reportName,reportDescription,MainEntity,SubEntity,pageheader,pagefooter FROM MMM_MST_REPORT WHERE EID = " & Session("EID").ToString(), con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        updatePanelEdit.Update()
        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BinddataGrid()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("SELECT Formid,formName from MMM_MST_FORMS where formtype='" & ddlMainEntity.Text & "' and EID='" & Session("EID") & "'", con)
            Dim ds As New DataSet

            da.Fill(ds, "data")
            ddlSubEntity.Items.Clear()
            ddlSubEntity.Items.Add("")
            'dlSubEntity.Items(0).Value = 0
            For I As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlSubEntity.Items.Add(ds.Tables("data").Rows(I).Item("formName").ToString())
            Next
            da.Dispose()
            con.Dispose()
        End If
    End Sub

    Protected Sub btnsavereport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsavereport.Click
        If txtReportName.Text.Length < 2 Then
            lblForm.Text = "Please Enter Valid Report Name"
            Exit Sub
        End If

        If txtDesc.Text.Length < 4 Then
            lblForm.Text = "Please Enter Valid Description"
            Exit Sub
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)


        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If btnsavereport.Text = "Save And Create Report" Then
            lblReportSave.Visible = False
            oda.SelectCommand.CommandText = "uspAddReport"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("reportname", txtReportName.Text)

            oda.SelectCommand.Parameters.AddWithValue("reportdesc", txtDesc.Text)
            oda.SelectCommand.Parameters.AddWithValue("mainentity", ddlMainEntity.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("subentity", ddlSubEntity.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("pageheader", txtPageHeader.Text)
            oda.SelectCommand.Parameters.AddWithValue("pagefooter", txtPageFooter.Text)
            oda.SelectCommand.Parameters.AddWithValue("isqryfield", 1)
            oda.SelectCommand.Parameters.AddWithValue("qryfield", ViewState("qry"))
            oda.SelectCommand.Parameters.AddWithValue("Reporttype", ddlrptType.SelectedItem.Value())
            'oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("pid").ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            oda.SelectCommand.ExecuteNonQuery()
            lblReportSave.Visible = True

        End If

        If btnsavereport.Text = "Update" Then

            oda.SelectCommand.CommandText = "uspEditReport"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("reportname", txtReportName.Text)
            oda.SelectCommand.Parameters.AddWithValue("descr", txtDesc.Text)
            oda.SelectCommand.Parameters.AddWithValue("mainentity", ddlMainEntity.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("subentity", ddlSubEntity.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("pageheader", txtPageHeader.Text)
            oda.SelectCommand.Parameters.AddWithValue("pagefooter", txtPageFooter.Text)
            oda.SelectCommand.Parameters.AddWithValue("reportid", ViewState("pid").ToString())
            oda.SelectCommand.Parameters.AddWithValue("Reporttype", ddlrptType.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("qryfield", ViewState("qry"))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        End If
        oda.SelectCommand.CommandText = "select reportid,reportname,reportDescription,mainentity,subentity,pageheader,pagefooter From MMM_MST_REPORT   where EID='" & Session("EID").ToString() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()

        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Hide()
        con.Close()
        con.Dispose()
        oda.Dispose()
        btnForm_ModalPopupExtender.Hide()

    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        txtReportName.Text = row.Cells(1).Text
        txtDesc.Text = row.Cells(2).Text


        'ddlMainEntity.SelectedItem.Text = row.Cells(3).Text
        'ddlSubEntity.SelectedItem.Text = row.Cells(4).Text
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT reportid,subentity,mainentity,isqryfield from MMM_MST_REPORT where reportid='" & ViewState("pid").ToString() & "'", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        ViewState("qryfld") = ds.Tables("data").Rows(0).Item("isqryfield").ToString()
        If ViewState("qryfld") = "1" Then
            ' rbQuerydrvn.Checked = True
            ddlrptType.Enabled = False
            ddlMainEntity.Enabled = False
            ddlSubEntity.Enabled = False
        End If
        If ds.Tables("data").Rows.Count > 0 Then
            ddlMainEntity.SelectedIndex = ddlMainEntity.Items.IndexOf(ddlMainEntity.Items.FindByText(ds.Tables("data").Rows(0).Item("mainentity").ToString()))
            ddlSubEntity.SelectedIndex = ddlSubEntity.Items.IndexOf(ddlSubEntity.Items.FindByText(ds.Tables("data").Rows(0).Item("subentity").ToString()))
        End If
        'ddlSubEntity.SelectedIndex = ddlSubEntity.Items.IndexOf(ddlSubEntity.Items.FindByText(ds.Tables("data").Rows(0).Item("subentity").ToString()))
        txtPageHeader.Text = row.Cells(5).Text
        txtPageFooter.Text = row.Cells(6).Text
        con.Close()
        da.Dispose()
        btnsavereport.Text = "Update"
        Me.updatePanelEdit.Update()
        btnForm_ModalPopupExtender.Show()
    End Sub

    'Protected Sub EditReportHit(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim btnRptDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnRptDetails.NamingContainer, GridViewRow)
    '    Dim tid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)
    '    ViewState("tid") = tid
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("select rd.FieldName,displayName,rd.FieldType,IsGroupBy ,IsOrderBy,AggFunction from MMM_REPORT_DTL rd inner join MMM_MST_REPORT rm on rm.Reportid=rd.ReportID inner join MMM_MST_FIELDS mf on mf.FieldMapping=rd.FieldMapping inner join MMM_MST_REPORT rm1 on rm1.SubEntity=mf.DocumentType  where tID=" & ViewState("tid") & " and mf.eid=" & Session("EID") & "", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "data")
    '    Dim gp As Integer
    '    Dim ord As Integer
    '    Dim str As String = ddlFieldType.SelectedItem.Text.ToString()
    '    If ds.Tables("data").Rows.Count > 0 Then
    '        txtFName.Text = ds.Tables("data").Rows(0).Item("fieldname").ToString()
    '        ddlFieldType.SelectedIndex = ddlFieldType.Items.IndexOf(ddlFieldType.Items.FindByText(ds.Tables("data").Rows(0).Item("FieldType").ToString()))
    '        ddlFieldValue.SelectedIndex = ddlFieldValue.Items.IndexOf(ddlFieldValue.Items.FindByText(ds.Tables("data").Rows(0).Item("displayName").ToString()))
    '        txtAggFunction.Text = ds.Tables("data").Rows(0).Item("AggFunction").ToString()
    '        gp = ds.Tables("data").Rows(0).Item("IsGroupBy").ToString()
    '        If gp = 1 Then
    '            chkM.Checked = True
    '        Else
    '            chkM.Checked = False
    '        End If
    '        ord = ds.Tables("data").Rows(0).Item("IsOrderBy").ToString()
    '        If ord = 1 Then
    '            chkActive.Checked = True
    '        Else
    '            chkActive.Checked = False
    '        End If
    '    End If
    '    Dim CF_Values As String = ""
    '    Dim cal_fields As String = ""
    '    If str = "CALCULATIVE FIELD" Then
    '        CF_Values = txtFormula.Text
    '        Dim dt As DataTable = CType(ViewState("FieldName"), DataTable)
    '        For Each dr As DataRow In dt.Rows
    '            Dim str1 As String = dr.Item("Displayname").ToString()
    '            If CF_Values.Contains(str1) Then
    '                CF_Values = CF_Values.Replace(str1, dr.Item("Fieldmapping"))
    '                cal_fields = Trim(CF_Values).ToString() & ","
    '            End If
    '        Next
    '        If cal_fields.Length > 0 Then
    '            cal_fields = cal_fields.Substring(0, cal_fields.Length - 1)
    '        End If
    '    Else
    '        cal_fields = ddlFieldValue.SelectedValue
    '    End If
    '    btnActSave.Text = "Update"
    '    Me.updatePanelEdit.Update()

    'End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)

        txtReportName.Text = ""
        txtDesc.Text = ""
        txtPageHeader.Text = ""
        txtPageFooter.Text = ""
        btnsavereport.Text = "Save And Create Report"
        btnForm_ModalPopupExtender.Show()
    End Sub

    Protected Sub AddFields(ByVal sender As Object, ByVal e As System.EventArgs)

        btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Report ? " & row.Cells(1).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
        BinddataGrid()
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteReport", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("pid").ToString))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        Dim ds As New DataSet()

        oda.SelectCommand.CommandText = "select reportid,reportname,reportdescription,mainentity,subentity,pageheader,pagefooter From MMM_MST_REPORT   where reportid='" & ViewState("pid").ToString() & " ' and eid='" & Session("EID") & "' "
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
        Me.updatePanelEdit.Update()
        Me.btnDelete_ModalPopupExtender.Hide()
        BinddataGrid()
    End Sub

    Protected Sub EditField(ByVal sender As Object, ByVal e As System.EventArgs)


    End Sub

    'Public Sub BindUpGrid()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("SELECT tid,reportid,fieldname,fieldtype,isorderby,isgroupby,aggfunction FROM MMM_REPORT_DTL WHERE reportid = " & ViewState("pid").ToString() & " order by dorder ", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "data")
    '    gvUsers.DataSource = ds.Tables("data")
    '    gvUsers.DataBind()
    '    updatePanelEdit.Update()
    '    da.Dispose()
    '    con.Dispose()

    'End Sub

    'Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim str As String = ddlFieldType.SelectedItem.Text.ToString()

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateReport ", con)
    '    Dim CF_Values As String = ""
    '    Dim cal_fields As String = ""


    '    If str = "CALCULATIVE FIELD" Then
    '        CF_Values = txtFormula.Text
    '        Dim dt As DataTable = CType(ViewState("FieldName"), DataTable)
    '        For Each dr As DataRow In dt.Rows
    '            Dim str1 As String = dr.Item("Displayname").ToString()
    '            If CF_Values.Contains(str1) Then
    '                CF_Values = CF_Values.Replace(str1, dr.Item("Fieldmapping"))
    '                cal_fields = Trim(CF_Values).ToString() & ","
    '            End If
    '        Next
    '        If cal_fields.Length > 0 Then
    '            cal_fields = cal_fields.Substring(0, cal_fields.Length - 1)
    '        End If
    '    Else
    '        cal_fields = ddlFieldValue.SelectedValue
    '    End If

    '    If btnActSave.Text = "Save" Then
    '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '        oda.SelectCommand.Parameters.AddWithValue("reportid", Val(ViewState("pid").ToString))
    '        oda.SelectCommand.Parameters.AddWithValue("fieldname", txtFName.Text)
    '        oda.SelectCommand.Parameters.AddWithValue("fieldtype", ddlFieldType.SelectedItem.Text)
    '        oda.SelectCommand.Parameters.AddWithValue("fieldMapvalue", cal_fields)
    '        oda.SelectCommand.Parameters.AddWithValue("aggfunction", txtAggFunction.Text)
    '        If chkM.Checked Then
    '            oda.SelectCommand.Parameters.AddWithValue("isgroupby", 1)
    '        Else
    '            oda.SelectCommand.Parameters.AddWithValue("isgroupby", 0)
    '        End If

    '        If chkActive.Checked Then
    '            oda.SelectCommand.Parameters.AddWithValue("isorderby", 1)
    '        Else
    '            oda.SelectCommand.Parameters.AddWithValue("isorderby", 0)
    '        End If
    '        'oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("pid").ToString())
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '    Else
    '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '        oda.SelectCommand.CommandText = "uspEditReportDetailFields"
    '        oda.SelectCommand.Parameters.Clear()
    '        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("tid").ToString))
    '        oda.SelectCommand.Parameters.AddWithValue("fldname", txtFName.Text)
    '        oda.SelectCommand.Parameters.AddWithValue("fldtype", ddlFieldType.SelectedItem.Text)
    '        oda.SelectCommand.Parameters.AddWithValue("fldMapping", ddlFieldValue.SelectedValue.ToString())
    '        oda.SelectCommand.Parameters.AddWithValue("aggfunction", txtAggFunction.Text)
    '        'oda.SelectCommand.Parameters.AddWithValue("aggfunction", txtAggFunction.Text)
    '        If chkM.Checked Then
    '            oda.SelectCommand.Parameters.AddWithValue("grpby", 1)
    '        Else
    '            oda.SelectCommand.Parameters.AddWithValue("grpby", 0)
    '        End If

    '        If chkActive.Checked Then
    '            oda.SelectCommand.Parameters.AddWithValue("orderby", 1)
    '        Else
    '            oda.SelectCommand.Parameters.AddWithValue("orderby", 0)
    '        End If
    '        'oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("pid").ToString())
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '    End If
    '    oda.SelectCommand.ExecuteNonQuery()
    '    oda.SelectCommand.CommandText = "SELECT tid,reportid,fieldname,fieldtype,isorderby,isgroupby,aggfunction FROM MMM_REPORT_DTL WHERE reportid='" & ViewState("pid").ToString() & "' "
    '    oda.SelectCommand.CommandType = CommandType.Text
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "data")
    '    gvUsers.DataSource = ds.Tables("data")
    '    gvUsers.DataBind()

    '    Me.updatePanelEdit.Update()

    '    con.Close()
    '    con.Dispose()
    '    oda.Dispose()

    '    BindUpGrid()
    'End Sub

    Protected Sub AddFilter(ByVal sender As Object, ByVal e As System.EventArgs)
        'ModalPopupExtender_filter.Show()
    End Sub

    Protected Sub UpdateScreen(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where eid=" & Session("EID") & " and reportid=" & pid & ""
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        txtquery.Text = da.SelectCommand.ExecuteScalar()
        con.Close()
        da.Dispose()
        updatePanelEdit.Update()
        btnEdit_ModalPopupExtender.Show()

    End Sub

   

    Protected Sub ddlMainEntity_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMainEntity.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT Formid,formName from MMM_MST_FORMS where formtype='" & ddlMainEntity.Text & "' and EID='" & Session("EID") & "' order by formname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        ddlSubEntity.Items.Clear()
        ddlSubEntity.Items.Add("")
        'dlSubEntity.Items(0).Value = 0
        For I As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddlSubEntity.Items.Add(ds.Tables("data").Rows(I).Item("formName").ToString())
        Next


        'ddlMainEntity.DataSource = ds
        'ddlMainEntity.DataTextField = "FormName"
        'ddlMainEntity.DataValueField = "formid"
        'ddlMainEntity.DataBind()
        da.Dispose()
        con.Dispose()
    End Sub

    'Protected Sub DeleteHitUser(ByVal sender As Object, ByVal e As System.EventArgs)

    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim tid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)
    '    ViewState("tid") = tid
    '    btnShowPopupDelete.Text = "Delete"
    '    lblconf.Text = "Are you Sure Want to delete this Field ? " & row.Cells(1).Text
    '    Me.updateDeleteRptFiles.Update()
    '    Me.btnDeleteReport.Show()
    'End Sub

    'Protected Sub DeleteFields(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteRptField", con)
    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '    oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("tid").ToString))
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    oda.SelectCommand.ExecuteNonQuery()
    '    Dim ds As New DataSet()

    '    oda.SelectCommand.CommandType = CommandType.Text

    '    oda.SelectCommand.CommandText = "select tid,reportid,fieldname,fieldmapping,dorder,isgroupby,isorderby,AggFunction From MMM_REPORT_dtl   where reportid='" & ViewState("pid").ToString() & " '  "
    '    'oda.SelectCommand.CommandType = CommandType.Text
    '    'oda.SelectCommand.CommandType = CommandType.Text
    '    oda.Fill(ds, "data")
    '    gvUsers.DataSource = ds.Tables("data")
    '    gvUsers.DataBind()
    '    con.Close()
    '    oda.Dispose()
    '    con.Dispose()
    '    Me.updatePanelEdit.Update()
    '    Me.btnDelete_ModalPopupExtender.Hide()
    '    Me.btnDeleteReport.Hide()
    '    BindUpGrid()
    'End Sub

    Public Sub FilterSHowPopUp(ByVal sender As Object, ByVal e As System.EventArgs)
        '  ModalPopupExtender_filter.Show()
    End Sub



    Protected Sub btnSaveQry_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '" Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        ' Dim con As SqlConnection = New SqlConnection(conStr)
        ' Dim oda As SqlDataAdapter = New SqlDataAdapter("uspAddQry ", con)

        ' oda.SelectCommand.CommandText = "uspAddQry"
        ' oda.SelectCommand.CommandType = CommandType.StoredProcedure
        ' oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
        ' oda.SelectCommand.Parameters.AddWithValue("reportid", ViewState("pid"))


        ' oda.SelectCommand.Parameters.AddWithValue("qry", txtQry.Text)

        ' If con.State <> ConnectionState.Open Then
        '     con.Open()
        ' End If

        ' oda.SelectCommand.ExecuteNonQuery()
        ' lblqry.Text = "Query Saved Successfully"
        ' txtQry.Text = ""
    End Sub

    Protected Sub ddlSubEntity_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSubEntity.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspgetalldetailitemnew", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("fn", ddlSubEntity.SelectedItem.Text)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        ViewState("qry") = oda.SelectCommand.ExecuteScalar()
    End Sub

    Protected Sub btnActSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActSave.Click
        Try

        
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspupdatereportqry", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
            oda.SelectCommand.Parameters.AddWithValue("tid", ViewState("pid"))
            oda.SelectCommand.Parameters.AddWithValue("qryField", txtquery.Text)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        btnEdit_ModalPopupExtender.Hide()
        Catch ex As Exception
            lblMsgEdit.Text = ex.Message
        End Try
    End Sub
End Class
