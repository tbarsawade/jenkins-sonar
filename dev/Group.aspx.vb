Imports System.Data
Imports System.Data.SqlClient

Partial Class Group
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='GROUP' order by DisName", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next

            da.SelectCommand.CommandText = "SELECT Username,uid from MMM_MST_USER where userrole='USR' and eid=" & Session("EID").ToString() & "  order by UserName"
            da.Fill(ds, "unit")
            ddlOwner.Items.Clear()
            ddlOwner.Items.Add("Please Select")
            For i As Integer = 0 To ds.Tables("unit").Rows.Count - 1
                ddlOwner.Items.Add(ds.Tables("unit").Rows(i).Item(0))
                ddlOwner.Items(i + 1).Value = ds.Tables("unit").Rows(i).Item(1).ToString()
            Next
            con.Dispose()
            da.Dispose()
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
        txtLocation.Text = row.Cells(2).Text
        ddlOwner.SelectedIndex = ddlOwner.Items.IndexOf(ddlOwner.Items.FindByText(row.Cells(3).Text))
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        txtName.Text = ""
        txtLocation.Text = ""
        ddlOwner.SelectedIndex = ddlOwner.Items.IndexOf(ddlOwner.Items.FindByText("Please Select"))
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Group? " & row.Cells(2).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        'validation for null entry

        If txtName.Text.Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid Group Name"
            Exit Sub
        End If

        If ddlOwner.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Please Select Group Owner for this Group"
            Exit Sub
        End If

        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertGroup", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("grpname", txtName.Text)
            oda.SelectCommand.Parameters.AddWithValue("grpDesc", txtLocation.Text)
            oda.SelectCommand.Parameters.AddWithValue("gwuid", ddlOwner.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            con.Dispose()
            If iSt = 0 Then
                txtName.Text = ""
                lblRecord.Text = "Project has been created successfully"
                updatePanelEdit.Update()
                gvData.DataBind()
                btnEdit_ModalPopupExtender.Hide()
            Else
                lblMsgEdit.Text = "This Group Already Exist"
                updatePanelEdit.Update()
            End If
        Else

            'Edit Record
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateGroup", con)
            Dim pid As Integer = Val(ViewState("pid").ToString())
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("grpname", txtName.Text)
            oda.SelectCommand.Parameters.AddWithValue("grpDesc", txtLocation.Text)
            oda.SelectCommand.Parameters.AddWithValue("gwuid", ddlOwner.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("gid", pid)
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
 
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            con.Dispose()
            If iSt = 0 Then
                txtName.Text = ""
                lblRecord.Text = "Project has been updated successfully"
                updatePanelEdit.Update()
                gvData.DataBind()
                btnEdit_ModalPopupExtender.Hide()
            Else
                lblMsgEdit.Text = "This Group Already Exist"
                updatePanelEdit.Update()
            End If
        End If
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pid As String = ViewState("pid").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteGroup", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("gid", pid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Then
            gvData.DataBind()
            lblRecord.Text = "Project has been deleted successfully"
            btnDelete_ModalPopupExtender.Hide()
        Else
            lblMsgDelete.Text = "This Group can Not be Deleted"
            updatePanelDelete.Update()
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataBind()
    End Sub
End Class
