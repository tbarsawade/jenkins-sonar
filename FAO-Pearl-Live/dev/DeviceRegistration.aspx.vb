
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing

Partial Class DeviceRegistration
    Inherits System.Web.UI.Page


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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Try
                'da.SelectCommand.CommandText = "select disname,colname from MMM_MST_SEARCH where tablename='DEVICEREGISTRATION' order by DisName"
                'da.Fill(ds, "data")
                'For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                '    ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                '    ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
                'Next

                da.SelectCommand.CommandText = "Select username,uid from mmm_mst_user where eid=" & Session("EID") & " order by username"
                da.Fill(ds, "user")
                If ds.Tables("user").Rows.Count > 0 Then
                    ddluser.DataSource = ds.Tables("user")
                    ddluser.DataTextField = "username"
                    ddluser.DataValueField = "uid"
                    ddluser.DataBind()
                    Dim li As New ListItem("--Select User--", 0)
                    ddluser.Items.Insert(0, li)


                End If
                getSearchResult()
            Catch ex As Exception


            Finally
                If Not da Is Nothing Then
                    da.Dispose()
                End If
                If Not con Is Nothing Then
                    con.Close()
                End If
            End Try


        End If
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        txtimi.Text = ""
        ddluser.SelectedValue = "0"
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim path As String = txtimi.Text
        If ddluser.SelectedValue = "0" Then
            lblMsgEdit.Text = "* Please select a User"
            Exit Sub
        End If

        '  lblMsgupdate.Text = ""
        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)

            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspinsertmobdeviceinfo", con)
            Try


                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@imiei", Trim(path))
                oda.SelectCommand.Parameters.AddWithValue("@user", Trim(ddluser.SelectedValue))
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                oda.SelectCommand.Parameters.AddWithValue("@UID", Session("UID").ToString())

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                lblMsgupdate.Text = oda.SelectCommand.ExecuteScalar()
                If lblMsgupdate.Text = "Device Registered Successfully" Then
                    Me.btnEdit_ModalPopupExtender.Hide()
                    getSearchResult()
                Else
                    lblMsgEdit.Text = "Device Is Already Registered!!!"
                End If




            Catch ex As Exception

            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If

            End Try

        Else

            Dim path1 As String = txtimi.Text
            path1 = path1.Replace(" ", "")
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspupdatemobdeviceinfo", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@imiei", Trim(path))
            oda.SelectCommand.Parameters.AddWithValue("@user", Trim(ddluser.SelectedValue))
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
            oda.SelectCommand.Parameters.AddWithValue("@UID", Session("UID").ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblMsgEdit.Text = oda.SelectCommand.ExecuteScalar()
            If lblMsgEdit.Text = "Updated Successfully" Then
                getSearchResult()
                Me.btnEdit_ModalPopupExtender.Hide()
                lblMsgupdate.Text = "Updated Successfully"
            Else
                lblMsgEdit.Text = "Already Exists!!!"
            End If

        End If



    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        'gvData.DataBind()
        getSearchResult()
        If gvData.Rows.Count > 0 Then
            lblRecord.Text = gvData.Rows.Count & " Records Found "

        Else
            lblRecord.Text = " No record found !! "
        End If
    End Sub

    Private Function getSearchResult() As DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        Dim ds As New DataSet
        Try
            'Dim val As String = txtValue.Text.Trim
            'If val <> "" Then
            '    val = " and m.iminumber like '%" & val & "%' "
            'End If
            'Dim us As String = UCase(ddluserlist.SelectedItem.Text)
            'If us <> "SELECT ALL" Then
            '    val = val & " and m.userid like '%" & ddluserlist.SelectedValue & "%'"
            'End If
            If (ddluserlist.SelectedItem.Text = "IMEI NUMBER") Then
                da.SelectCommand.CommandText = "Select m.tid, m.iminumber,u.username[User],u.userid,case when m.isactive =0 then 'InActive' else 'Active' end [Status] from mmm_mst_mobdeviceinfo m left outer join (select uid,eid,username,userid from mmm_mst_user where eid=" & Session("EID") & ") u on u.uid=m.userid where m.eid=" & Session("EID") & " and  m.iminumber like'" + txtValue.Text + "%'"
            ElseIf (ddluserlist.SelectedItem.Text = "USERID") Then
                da.SelectCommand.CommandText = "Select m.tid, m.iminumber,u.username[User],u.userid,case when m.isactive =0 then 'InActive' else 'Active' end [Status] from mmm_mst_mobdeviceinfo m left outer join (select uid,eid,username,userid from mmm_mst_user where eid=" & Session("EID") & ") u on u.uid=m.userid where m.eid=" & Session("EID") & " and  u.userid like'" + txtValue.Text + "%'"
            ElseIf (ddluserlist.SelectedItem.Text = "USERNAME") Then
                da.SelectCommand.CommandText = "Select m.tid, m.iminumber,u.username[User],u.userid,case when m.isactive =0 then 'InActive' else 'Active' end [Status] from mmm_mst_mobdeviceinfo m left outer join (select uid,eid,username,userid from mmm_mst_user where eid=" & Session("EID") & ") u on u.uid=m.userid where m.eid=" & Session("EID") & " and  u.username like'" + txtValue.Text + "%'"

            End If

            'da.SelectCommand.CommandText = "Select m.tid, m.iminumber,(select username from mmm_mst_user where eid=" & Session("EID") & " and uid=m.userid)[User],,case when m.isactive =0 then 'InActive' else 'Active' end [Status] from mmm_mst_mobdeviceinfo m where eid=" & Session("EID") & "  " & val.ToString() & ""
            'da.SelectCommand.CommandText = "Select m.tid, m.iminumber,u.username[User],u.userid,case when m.isactive =0 then 'InActive' else 'Active' end [Status] from mmm_mst_mobdeviceinfo m left outer join (select uid,eid,username,userid from mmm_mst_user where eid=" & Session("EID") & ") u on u.uid=m.userid where m.eid=" & Session("EID") & "  " & val.ToString() & ""
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                gvData.DataSource = ds.Tables("data")
                gvData.DataBind()
            Else
                gvData.DataSource = Nothing
                gvData.DataBind()

            End If
            updPnlGrid.Update()
        Catch ex As Exception
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
            End If

        End Try

    End Function
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("TID") = Tid

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Try
            da.SelectCommand.CommandText = "Select * from mmm_mst_mobdeviceinfo where eid=" & Session("EID") & " and tid=" & Tid & ""
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                txtimi.Text = ds.Tables("data").Rows(0).Item("iminumber").ToString

                'da.SelectCommand.CommandText = "select * from mmm_mst_user where eid=" & Session("EID") & " order by username"
                'da.Fill(ds, "user")
                'If ds.Tables("user").Rows.Count > 0 Then
                '    ddluser.DataSource = ds.Tables("user")
                '    ddluser.DataTextField = "Username"
                '    ddluser.DataValueField = "UID"
                '    ddluser.DataBind()
                '    ddluser.Items.Insert(0, "Select")

                ddluser.SelectedIndex = ddluser.Items.IndexOf(ddluser.Items.FindByValue(ds.Tables("data").Rows(0).Item("userid").ToString()))
                'End If
                Me.updatePanelEdit.Update()
                Me.btnEdit_ModalPopupExtender.Show()
            End If


        Catch ex As Exception
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
            End If



        End Try

    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        lblMsgDelFolder.ForeColor = Drawing.Color.Red
        lblMsgDelFolder.Text = "Are You Sure Want to Delete"

        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("tid") = tid

        getSearchResult()
        Me.updatePanelEdit.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim path As String = txtimi.Text
        path = path.Replace(" ", "")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspdeletemobdeviceinfo", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("tid"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        lblMsgEdit.Text = oda.SelectCommand.ExecuteScalar()

        getSearchResult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        btnDelFolder_ModalPopupExtender.Hide()
    End Sub
    Protected Sub LockRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspLockmobdeviceinfo", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("tid", ViewState("tid"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Or iSt = 1 Then
            gvData.DataBind()
            lblRecord.Text = "Updated  successfully"
            ModalPopup_Lock.Hide()
        Else
            lblLock.Text = "Not updated"
        End If
        getSearchResult()

        updLock.Update()
        'updPnlGrid.Update()
    End Sub

    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("tid") = tid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Select isactive from MMM_MST_mobdeviceinfo where eid=" & Session("EID") & " and tid=" & ViewState("tid") & "", con)
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.Parameters.AddWithValue("tid", ViewState("tid"))
        Dim ds As New DataSet()
        oda.Fill(ds, "auth")
        Dim img As ImageButton = DirectCast(row.FindControl("ImageButton1"), ImageButton)
        If ds.Tables("auth").Rows(0).Item("isactive") = 1 Then
            lblLock.Text = "<b>Please click the option hereunder to confirm if you wish to Inactive  </b>"
            btnLockupdate.Text = "InActive"
            img.ImageUrl = "~/Images/unlock.png"
        Else
            lblLock.Text = "<b>Please click the option hereunder to confirm if you wish to Active </b>"
            btnLockupdate.Text = "Active"
            img.ImageUrl = "~/Images/lock.png"
        End If


        Me.updLock.Update()
        Me.ModalPopup_Lock.Show()
        getSearchResult()
    End Sub

  

    'Protected Sub gvData_SelectedIndexChanging(sender As Object, e As GridViewSelectEventArgs) Handles gvData.SelectedIndexChanging
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim id As String = gvData.DataKeys(e.NewSelectedIndex).Value.ToString()
    '    Dim status As String = gvData.Rows(e.NewSelectedIndex).Cells(3).Text
    '    Dim i As Integer = 0
    '    If status = "Active" Then
    '        i = 1
    '    Else
    '        i = 0
    '    End If

    '    Try
    '        If Not con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim cmd As New SqlCommand("update MMM_MST_mobdeviceinfo set isactive=" & i & " where ID=" + id, con)
    '        cmd.ExecuteNonQuery()

    '    Catch ex As Exception
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '        End If

    '    End Try

    'End Sub
End Class
