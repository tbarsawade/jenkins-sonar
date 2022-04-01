Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
Imports System.Threading
Imports System.IO
Imports System.Net.Mail
Imports System.Net
Imports System.Windows.Forms

Partial Class Multipleprerole
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname FROM MMM_MST_SEARCH where tableName='MMM_ref_PreRole_user'", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            da.Dispose()
            con.Dispose()
            ds.Dispose()
            bindgrid()
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
    Protected Sub btnadd_Click(sender As Object, e As EventArgs)
        If ddlusr.SelectedItem.Text = "--Select--" Then
            lblMsgEdit.Text = "Please Enter User Name"
            Exit Sub
        End If
        ''Check for Subject Line
        If ddlrole.SelectedItem.Text = "--Select--" Then
            lblMsgEdit.Text = "Please Enter Role Name"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        oda.SelectCommand.CommandText = "Select * from MMM_ref_PreRole_user where eid ='" & Val(Session("EID").ToString()) & "' and uid='" & ddlusr.SelectedValue.Trim() & "' and rolename= '" & ddlrole.SelectedItem.Text.Trim() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(dt)

        If dt.Rows.Count > 0 Then
            lblMsgEdit.Visible = True
            lblMsgEdit.Text = "This Role is already assigned to User!"
            Me.btnEdit_ModalPopupExtender.Show()
            updPnlGrid.Update()
            con.Dispose()
            dt.Dispose()
            oda.Dispose()
            Exit Sub
        End If

        dt.Clear()
        oda.SelectCommand.CommandText = "Select * from MMM_ref_Role_user where eid ='" & Val(Session("EID").ToString()) & "' and uid='" & ddlusr.SelectedValue.Trim() & "' and rolename= '" & ddlrole.SelectedItem.Text.Trim() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(dt)

        If dt.Rows.Count > 0 Then
            lblMsgEdit.Visible = True
            lblMsgEdit.Text = "This Role is already assigned to User in Role assignment!"
            Me.btnEdit_ModalPopupExtender.Show()
            updPnlGrid.Update()
            con.Dispose()
            dt.Dispose()
            oda.Dispose()
            Exit Sub
        End If

        dt.Clear()
        oda.SelectCommand.CommandText = "Select * from MMM_mst_user where eid ='" & Val(Session("EID").ToString()) & "' and uid='" & ddlusr.SelectedValue.Trim() & "' and userrole= '" & ddlrole.SelectedItem.Text.Trim() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(dt)

        If dt.Rows.Count > 0 Then
            lblMsgEdit.Visible = True
            lblMsgEdit.Text = "This Role is already assigned to User as a default role!"
            Me.btnEdit_ModalPopupExtender.Show()
            updPnlGrid.Update()
            con.Dispose()
            dt.Dispose()
            oda.Dispose()
            Exit Sub
        End If

        oda.SelectCommand.CommandText = "InsertMultiprerole"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.Parameters.AddWithValue("eid", Val(Session("EID").ToString()))
        oda.SelectCommand.Parameters.AddWithValue("uid", ddlusr.SelectedValue.Trim())
        '   oda.SelectCommand.Parameters.AddWithValue("username", ddlusr.SelectedItem.Text.Trim())
        oda.SelectCommand.Parameters.AddWithValue("rolename", ddlrole.SelectedItem.Text.Trim())
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteScalar()
        ddlusr.SelectedIndex = 0
        ddlrole.SelectedIndex = 0
        lblMsgEdit.Text = ""
        Me.btnEdit_ModalPopupExtender.Hide()
        updPnlGrid.Update()


        con.Dispose()
        dt.Dispose()
        oda.Dispose()
        bindgrid()
    End Sub

    Protected Sub DeleteHitUser(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Delete from MMM_ref_PreRole_user where tid=" & tid & " and eid ='" & Val(Session("EID").ToString()) & "'", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        oda.Dispose()
        con.Dispose()
        bindgrid()
    End Sub

    Public Sub bindgrid()
        lblmsg1.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Select tid, u.userid,u.username,mu.rolename,dms.ufn_rtnUserRolesAll(mu.eid,mu.uid) [CurrentRoles]  from MMM_ref_PreRole_user MU inner join mmm_mst_user U on mu.uid=u.uid and mu.eid=u.eid where mu.Eid=" & Val(Session("EID").ToString()) & "", con)
        Dim dt As New DataTable
        oda.Fill(dt)
        gvData.DataSource = dt
        gvData.DataBind()
        con.Dispose()
        dt.Dispose()
        oda.Dispose()
    End Sub
     
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Filldrpdwn() 
        lblMsgEdit.Text = ""
        ddlusr.SelectedIndex = 0
        ddlrole.SelectedIndex = 0
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs) Handles btnSearch.Click 
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter

        If ddlField.SelectedValue = "USERNAME" Then
            oda = New SqlDataAdapter("Select tid, u.userid,u.username,mu.rolename,dms.ufn_rtnUserRolesAll(mu.eid,mu.uid) [Current Roles]  from MMM_ref_PreRole_user MU inner join mmm_mst_user U on mu.uid=u.uid and mu.eid=u.eid where u.username like '" & txtValue.Text & "%' and mu.EID='" & Session("eid") & "'", con)
        ElseIf ddlField.SelectedValue = "ROLENAME" Then
            oda = New SqlDataAdapter("Select tid, u.userid,u.username,mu.rolename,dms.ufn_rtnUserRolesAll(mu.eid,mu.uid) [Current Roles]  from MMM_ref_PreRole_user MU inner join mmm_mst_user U on mu.uid=u.uid and mu.eid=u.eid where u.userid like '" & txtValue.Text & "%' and mu.EID='" & Session("eid") & "'", con)
        ElseIf ddlField.SelectedValue = "USERID" Then
            oda = New SqlDataAdapter("Select tid, u.userid,u.username,mu.rolename,dms.ufn_rtnUserRolesAll(mu.eid,mu.uid) [Current Roles]  from MMM_ref_PreRole_user MU inner join mmm_mst_user U on mu.uid=u.uid and mu.eid=u.eid where mu.roleName like '" & txtValue.Text & "%' and mu.EID='" & Session("eid") & "'", con)
        End If

        Dim ds As New DataSet()
        oda.Fill(ds, "data")

        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        If gvData.Rows.Count = 0 Then
            lblMsg1.Visible = True
            lblMsg1.Text = "No Record Found"
        Else
            lblMsg1.Visible = False
        End If
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub

    Protected Sub Filldrpdwn()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim EID As Integer = Session("EID")
        Dim da As New SqlDataAdapter("Select uid, USERID  + ' (' +  username + ')' [USERNAME],EID from MMM_MST_USER where EID=" & Session("EID") & " and userrole <>'SU'", con)
        Dim dt As New DataTable
        da.Fill(dt)
        ddlusr.DataSource = dt
        ddlusr.DataTextField = "username"
        ddlusr.DataValueField = "uid"
        ddlusr.DataBind()
        ddlusr.Items.Insert(0, "--Select--")

        da.SelectCommand.CommandText = "Select RoleId,RoleName from mmm_mst_role where Roletype='Pre Type' and rolename <>'SU' AND EID=" & Session("EID") & ""
        Dim dt1 As New DataTable
        da.Fill(dt1)
        ddlrole.DataSource = dt1
        ddlrole.DataTextField = "RoleName"
        ddlrole.DataValueField = "RoleId"
        ddlrole.DataBind()
        ddlrole.Items.Insert(0, "--Select--")
        con.Dispose()
        dt.Dispose()
        dt1.Dispose()
        da.Dispose()
    End Sub

    Protected Sub ddlusr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusr.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim EID As Integer = Session("EID")
        Dim da As New SqlDataAdapter("Select dms.ufn_rtnUserRolesAll(eid,uid) [CurrentRoles] from MMM_MST_USER where EID=" & Session("EID") & " and uid='" & ddlusr.SelectedValue.ToString & "'", con)
        Dim dt As New DataTable
        da.Fill(dt)
        lblcurrRoles.text = dt.Rows(0).Item(0).ToString

        con.Close()
        con.Dispose()
        da.Dispose()
        dt.Dispose()
    End Sub
End Class
