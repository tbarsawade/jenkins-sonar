Imports System.Data
Imports System.Data.SqlClient

Partial Class FieldMaster
    Inherits System.Web.UI.Page

    Public Sub BinddataGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("SELECT FieldID,displayName,FieldType,displayOrder,dropdown,case isRequired when 1 then 'Mandatory' else 'Nullable' END [isrequired],case isActive when 0 then 'InActive' else 'Active' end [isActive] FROM MMM_MST_FIELDS WHERE EID = " & Session("EID").ToString() & " Order By Displayorder", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        con.Dispose()
        da.Dispose()
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
            BinddataGrid()
        End If
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
        txtFName.Text = row.Cells(1).Text
        ddlType.SelectedIndex = ddlType.Items.IndexOf(ddlType.Items.FindByText(row.Cells(2).Text))
        txtDDLValues.Text = row.Cells(4).Text
        If row.Cells(5).Text = "Mandatory" Then
            chkM.Checked = True
        Else
            chkM.Checked = False
        End If

        If row.Cells(6).Text = "Active" Then
            chkActive.Checked = True
        Else
            chkActive.Checked = False
        End If
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        txtFName.Text = ""
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Field? " & row.Cells(2).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        'validation for null entry
        If txtFName.Text.Length < 2 Then
            lblMsg.Text = "Please Enter Valid Field Name"
            Exit Sub
        End If

        If btnActEdit.Text = "Save" Then

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertField", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Val(Session("EID").ToString()))
            oda.SelectCommand.Parameters.AddWithValue("displayname", txtFName.Text)
            oda.SelectCommand.Parameters.AddWithValue("fieldtype", ddlType.SelectedItem.Text)

            If chkM.Checked Then
                oda.SelectCommand.Parameters.AddWithValue("isRequired", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isRequired", 0)
            End If

            If chkActive.Checked Then
                oda.SelectCommand.Parameters.AddWithValue("isActive", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isActive", 0)
            End If
            oda.SelectCommand.Parameters.AddWithValue("dropdown", txtDDLValues.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim ist As Integer = oda.SelectCommand.ExecuteScalar()
            If ist = 0 Then
                txtFName.Text = ""
                lblMsgEdit.Text = "Field Created"
                updatePanelEdit.Update()
                BinddataGrid()
                btnEdit_ModalPopupExtender.Hide()
            Else
                lblMsgEdit.Text = "Maximum Allowed columns reached"
                updatePanelEdit.Update()
            End If

            con.Close()
            oda.Dispose()
            con.Dispose()

        Else
            'Edit Record
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateField", con)
            Dim pid As Integer = Val(ViewState("pid").ToString())
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("FieldID", pid)
            oda.SelectCommand.Parameters.AddWithValue("displayname", txtFName.Text)
            oda.SelectCommand.Parameters.AddWithValue("fieldtype", ddlType.SelectedItem.Text)

            If chkM.Checked Then
                oda.SelectCommand.Parameters.AddWithValue("isRequired", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isRequired", 0)
            End If

            If chkActive.Checked Then
                oda.SelectCommand.Parameters.AddWithValue("isActive", 1)
            Else
                oda.SelectCommand.Parameters.AddWithValue("isActive", 0)
            End If
            oda.SelectCommand.Parameters.AddWithValue("dropdown", txtDDLValues.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            lblMsgEdit.Text = "Field Updated"
            updatePanelEdit.Update()
            BinddataGrid()
            btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda.Dispose()
            con.Dispose()
        End If
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pid As String = ViewState("pid").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteField", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("Fieldid", pid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Then
            BinddataGrid()
            btnDelete_ModalPopupExtender.Hide()
        Else
            lblMsgDelete.Text = "This Field can Not be Deleted"
            updatePanelDelete.Update()
        End If
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlType.SelectedIndexChanged
        If ddlType.SelectedItem.Text.ToUpper() = "DROP DOWN" Then
            txtDDLValues.Visible = True
        Else
            txtDDLValues.Visible = False
        End If
    End Sub
End Class
