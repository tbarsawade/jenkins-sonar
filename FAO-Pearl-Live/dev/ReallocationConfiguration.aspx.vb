Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Globalization
Partial Class ReallocationConfiguration
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            getSearchResult()
            PopulateAllowedRoles()
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
    Private Sub binddldoctype()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select * from mmm_mst_forms where eid=" & Session("EID") & " and formsource='MENU DRIVEN' and formtype='document' and isworkflow=1 order by formname", con)
        Dim ds As New DataSet
        da.Fill(ds, "formname")
        ddldoctype.DataSource = ds.Tables("formname")
        ddldoctype.DataTextField = "Formname"
        ddldoctype.DataValueField = "Formid"
        ddldoctype.DataBind()
        ddldoctype.Items.Insert(0, New ListItem("Select"))
        con.Close()
    End Sub

    Private Sub bindroles()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select * from mmm_mst_role where eid=" & Session("EID") & " and roletype='post type' order by rolename", con)
        Dim ds As New DataSet
        da.Fill(ds, "role")
        ddlrole.DataSource = ds.Tables("role")
        ddlrole.DataTextField = "rolename"
        ddlrole.DataValueField = "Rolename"
        ddlrole.DataBind()
        ddlrole.Items.Insert(0, New ListItem("Select"))
        con.Close()
    End Sub


    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        ddldoctype.Items.Clear()
        ddlrole.Items.Clear()
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        lblMsgupdate.Text = ""
        txtRemarks.Text = ""
        binddldoctype()
        bindroles()
        chkRoles.Items.Clear()
        PopulateAllowedRoles()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgupdate.Text = ""
        lblMsgEdit.Text = ""
        binddldoctype()
        bindroles()
        Dim test1 As String = ddldoctype.Items.Count
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("TID") = Tid

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select * from mmm_mst_reallocation where eid=" & Session("EID") & " and tid=" & Tid & "", con)
        Dim ds As New DataSet
        da.Fill(ds, "Edit")

        If ds.Tables("Edit").Rows.Count > 0 Then
            Dim Chkvalues As String = ds.Tables("Edit").Rows(0)("AllowedRoles").ToString()
            'If (Chkvalues.Length > 0) Then

            '    For Each tech As String In Chkvalues.Split(","c)
            '        chkRoles.SelectedValue = tech
            '    Next
            'End If
            If Not String.IsNullOrEmpty(Chkvalues) Then
                Dim chkval As List(Of String) = Chkvalues.Split(New Char() {","c}).ToList()
                For Each li As ListItem In chkRoles.Items
                    li.Selected = chkval.Contains(li.Value)
                Next
            Else
                Dim chkval As List(Of String) = Chkvalues.Split(New Char() {","c}).ToList()
                For Each li As ListItem In chkRoles.Items
                    li.Selected = False
                Next
            End If
        End If
        If ds.Tables("Edit").Rows.Count > 0 Then
            ' bindfield()
            ddldoctype.SelectedIndex = ddldoctype.Items.IndexOf(ddldoctype.Items.FindByText(ds.Tables("Edit").Rows(0).Item("doctype").ToString()))
            da.SelectCommand.CommandText = "Select  * from MMM_MST_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by statusname"
            da.Fill(ds, "status")
            If ds.Tables("status").Rows.Count > 0 Then
                ddlstatus.DataSource = ds.Tables("status")
                ddlstatus.DataTextField = "StatusName"
                ddlstatus.DataValueField = "StatusName"
                ddlstatus.DataBind()
                ddlstatus.Items.Insert(0, New ListItem("Select"))
            End If
            da.SelectCommand.CommandText = "Select  displayname,fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname"
            da.Fill(ds, "field")
            If ds.Tables("field").Rows.Count > 0 Then
                ddlcfield.DataSource = ds.Tables("field")
                ddlcfield.DataTextField = "displayname"
                ddlcfield.DataValueField = "fieldmapping"
                ddlcfield.DataBind()
                ddlcfield.Items.Insert(0, New ListItem("Select"))
            End If
            ddlstatus.SelectedIndex = ddlstatus.Items.IndexOf(ddlstatus.Items.FindByText(ds.Tables("Edit").Rows(0).Item("status").ToString()))
            ddlrole.SelectedIndex = ddlrole.Items.IndexOf(ddlrole.Items.FindByText(ds.Tables("Edit").Rows(0).Item("role").ToString()))
            ddlcfield.SelectedIndex = ddlcfield.Items.IndexOf(ddlcfield.Items.FindByValue(ds.Tables("Edit").Rows(0).Item("cfield").ToString()))
            txtRemarks.Text = ds.Tables("Edit").Rows(0).Item("remarks").ToString()
        End If

        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)


        If ddldoctype.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Document Type!!"
            Exit Sub
        End If
        If ddlstatus.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Status!!"
            Exit Sub
        End If
        If ddlrole.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Role!!"
            Exit Sub
        End If
        If ddlcfield.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Circle Field!!"
            Exit Sub
        End If
        Dim values As String = ""
        For i As Integer = 0 To chkRoles.Items.Count - 1
            If chkRoles.Items(i).Selected Then
                values += chkRoles.Items(i).Value + ","
            End If
        Next
        values = values.TrimEnd(","c)
        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("USPinsert_Reallocation_031220", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@doctype", Trim(ddldoctype.SelectedItem.Text))
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@status", Trim(ddlstatus.SelectedItem.Text))
            oda.SelectCommand.Parameters.AddWithValue("@role", Trim(ddlrole.SelectedItem.Text))
            oda.SelectCommand.Parameters.AddWithValue("@cfield", Trim(ddlcfield.SelectedValue.ToString))
            oda.SelectCommand.Parameters.AddWithValue("@displayname", Trim(ddlcfield.SelectedItem.Text.ToString))
            oda.SelectCommand.Parameters.AddWithValue("@AllowedRole", values)
            oda.SelectCommand.Parameters.AddWithValue("@Remarks", Trim(txtRemarks.Text))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Me.btnEdit_ModalPopupExtender.Hide()
            lblMsgupdate.Text = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            con.Dispose()
            gvData.DataBind()
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("uspUpdate_Reallocation_031220", con)
            oda1.SelectCommand.CommandType = CommandType.StoredProcedure
            oda1.SelectCommand.Parameters.AddWithValue("@doctype", Trim(ddldoctype.SelectedItem.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@status", Trim(ddlstatus.SelectedItem.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@role", Trim(ddlrole.SelectedItem.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@cfield", Trim(ddlcfield.SelectedValue.ToString))
            oda1.SelectCommand.Parameters.AddWithValue("@displayname", Trim(ddlcfield.SelectedItem.Text.ToString))
            oda1.SelectCommand.Parameters.AddWithValue("@AllowedRole", values)
            oda1.SelectCommand.Parameters.AddWithValue("@Remarks", Trim(txtRemarks.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            lblMsgupdate.Text = oda1.SelectCommand.ExecuteNonQuery()
            updatePanelEdit.Update()
            btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda1.Dispose()
            con.Dispose()
        End If
        getSearchResult()
    End Sub

    'Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
    '    gvData.DataBind()
    '    If gvData.Rows.Count > 0 Then
    '        lblMsgupdate.Text = gvData.Rows.Count & " Records Found "
    '    Else
    '        lblMsgupdate.Text = " No record found "
    '    End If
    'End Sub

    Private Function getSearchResult() As DataTable
        gvData.DataBind()
    End Function


    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        lblMsgDelFolder.ForeColor = Drawing.Color.Red
        lblMsgDelFolder.Text = "Are You Sure Want to Delete this service"
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("TID") = Tid
        getSearchResult()
        Me.updatePanelEdit.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspdelete_Reallocation", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@Tid", ViewState("TID"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        lblMsgupdate.Text = oda.SelectCommand.ExecuteScalar()

        getSearchResult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        btnDelFolder_ModalPopupExtender.Hide()
    End Sub


    'Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldoctype.SelectedIndexChanged
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)

    '    Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "data")

    '    If ds.Tables("data").Rows.Count > 0 Then
    '        Dim addall As String = ""
    '        Dim fm As String = ""
    '        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
    '            Dim dn As String = UCase(ds.Tables("data").Rows(i).Item("displayname").ToString())
    '            Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
    '            Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
    '            Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
    '            Dim dty As String = ds.Tables("data").Rows(i).Item("documenttype").ToString()

    '            addall = addall & "{" & dn & "},"
    '            Dim d3 As String = ""
    '            Dim dw As String = ""
    '            If UCase(mv) = "MASTER VALUED" Then
    '                Dim ss As String() = DD.ToString().Split("-")
    '                d3 = "select substring((select ',{' + displayname + '}' FROM (select distinct  displayname from mmm_mst_fields f where eid=" & Session("EID") & " and DocumentType='" & ss(1).ToString() & "' ) AR FOR XML PATH('')),2,200000) As Displayname"
    '                da.SelectCommand.CommandText = d3
    '                da.Fill(ds, "mv")
    '                addall = addall & ds.Tables("mv").Rows(0).Item("displayname").ToString() & ","
    '            End If

    '        Next
    '        txtdisplaylist.Text = addall.ToString()

    '    End If


    'End Sub


    Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldoctype.SelectedIndexChanged
        bindstatus()
        bindfield()
    End Sub
    Private Sub bindfield()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select  displayname,fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "field")
        If ds.Tables("field").Rows.Count > 0 Then
            ddlcfield.DataSource = ds.Tables("field")
            ddlcfield.DataTextField = "displayname"
            ddlcfield.DataValueField = "fieldmapping"
            ddlcfield.DataBind()
            ddlcfield.Items.Insert(0, New ListItem("Select"))
        End If
    End Sub

    Private Sub bindstatus()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select  * from MMM_MST_workflow_status where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by statusname", con)
        Dim ds As New DataSet
        da.Fill(ds, "status")
        If ds.Tables("status").Rows.Count > 0 Then
            ddlstatus.DataSource = ds.Tables("status")
            ddlstatus.DataTextField = "StatusName"
            ddlstatus.DataValueField = "StatusName"
            ddlstatus.DataBind()
            ddlstatus.Items.Insert(0, New ListItem("Select"))
        End If
    End Sub

    Private Sub PopulateAllowedRoles()
        Using conn As New SqlConnection()
            conn.ConnectionString = ConfigurationManager _
                .ConnectionStrings("conStr").ConnectionString()
            Using cmd As New SqlCommand()
                cmd.CommandText = "Select roleid,rolename from mmm_mst_role where eid=" & Session("EID") & " order by rolename"
                cmd.Connection = conn
                conn.Open()
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    While sdr.Read()
                        Dim item As New ListItem()
                        item.Text = sdr("rolename").ToString()
                        item.Value = sdr("roleid").ToString()
                        chkRoles.Items.Add(item)
                    End While
                End Using
                conn.Close()
            End Using
        End Using
    End Sub

End Class

