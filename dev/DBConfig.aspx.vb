Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class DBConfig
    Inherits System.Web.UI.Page




    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Try

                getresult()
                BindRoles()
            Catch ex As Exception

            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
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
    'Protected Sub btnsetdefaultpage_Click(sender As Object, e As EventArgs) Handles btnsetdefaultpage.Click

    '    Dim page As String = String.Empty
    '    If ddldefaultpage.SelectedValue = 0 Then
    '        page = "MainHome.aspx"
    '    ElseIf ddldefaultpage.SelectedValue = 1 Then
    '        page = "Dashboard.aspx"
    '    End If

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet

    '    Using Sqlcon As New SqlConnection(conStr)
    '        Dim strSelQuery As String = "Update mmm_mst_entity set defaultpage='" & page.Trim.ToString() & "' FROM mmm_mst_entity where eid=" & Session("EID") & " "
    '        Using cmd As New SqlCommand()
    '            Sqlcon.Open()
    '            cmd.Connection = Sqlcon
    '            cmd.CommandType = CommandType.Text
    '            cmd.CommandText = strSelQuery
    '            cmd.ExecuteNonQuery()
    '        End Using
    '    End Using
    '    lblmsg.Text = ddldefaultpage.SelectedItem.Text.ToString() & " has been set as your Default Page!!!"

    '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* '" & ddldefaultpage.SelectedItem.Text.ToString() & "' has been set as your Default Page!!!');", True)


    'End Sub


    Protected Sub btnsetdefaultpage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnsetdefaultpage.Click

        Dim page As String = String.Empty
        If ddldefaultpage.SelectedValue = 0 Then
            page = "MainHome.aspx"
        ElseIf ddldefaultpage.SelectedValue = 1 Then
            page = "Dashboard.aspx"
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Using Sqlcon As New SqlConnection(conStr)
            Dim strSelQuery As String = "USPSetDefaultPageInsert"
            Using cmd As New SqlCommand()
                Sqlcon.Open()
                cmd.Connection = Sqlcon
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@eid", Session("EID"))
                cmd.Parameters.AddWithValue("@page", page.ToString())
                cmd.CommandText = strSelQuery
                cmd.ExecuteNonQuery()
            End Using
        End Using
        lblmsg.Text = ddldefaultpage.SelectedItem.Text.ToString() & " has been set as your Default Page!!!"

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* '" & ddldefaultpage.SelectedItem.Text.ToString() & "' has been set as your Default Page!!!');", True)


    End Sub


    Protected Sub gvdata_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim lnk As LinkButton = DirectCast(e.Row.FindControl("img_status"), LinkButton)

            If e.Row.Cells(5).Text = "Active" Then

                lnk.Text = "Active"

            Else

                lnk.Text = "InActive"
                e.Row.BackColor = Drawing.Color.LightYellow

            End If
            Dim a As String = ""
            If lnk.Text = "Active" Then
                a = "InActive"
            Else
                a = "Active"
            End If

            lnk.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to " & a.ToString() & " this record : " + DataBinder.Eval(e.Row.DataItem, "DBNAME") + "')")

        End If
        e.Row.Cells(5).Visible = False
    End Sub

    Protected Sub gvData_SelectedIndexChanging(sender As Object, e As GridViewSelectEventArgs)
        Dim tid As String = gvData.DataKeys(e.NewSelectedIndex).Value.ToString()
        Dim status As String = gvData.Rows(e.NewSelectedIndex).Cells(5).Text


        Dim i As Integer = 0
        If status = "Active" Then
            status = "Inactive"
            i = 0
        Else
            status = "Active"
            i = 1
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim cmd As New SqlCommand("update mmm_mst_dashboard set status=" & i & " where tID=" + tid, con)

        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
        getresult()
    End Sub

    Private Sub getresult()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Using Sqlcon As New SqlConnection(conStr)
            Dim strSelQuery As String = "select tid, dbname ,dbtype,widgettype,widgetnature,case when status=1 then 'Active' else 'InActive' end [Status],Roles,dorder from mmm_mst_dashboard where eid=" & Session("EID") & " order by dorder "
            Using cmd As New SqlCommand()
                Sqlcon.Open()
                cmd.Connection = Sqlcon
                cmd.CommandType = CommandType.Text
                cmd.CommandText = strSelQuery
                da = New SqlDataAdapter(cmd)
                ds = New DataSet()
                da.Fill(ds)
                gvData.DataSource = ds
                gvData.DataBind()
            End Using
        End Using

    End Sub

    Private Sub BindRoles()
        Using conn As New SqlConnection()
            conn.ConnectionString = ConfigurationManager _
                .ConnectionStrings("constr").ConnectionString()
            Using cmd As New SqlCommand()
                cmd.CommandText = "select roleid,rolename from mmm_mst_role where eid=" & Session("EID") & " order by rolename"
                cmd.Connection = conn
                conn.Open()
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    While sdr.Read()
                        Dim item As New ListItem()
                        item.Text = sdr("Rolename").ToString()
                        item.Value = sdr("RoleID").ToString()
                        chkroles.Items.Add(item)
                    End While
                End Using
                conn.Close()
            End Using
        End Using
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        reset()
        btnActEdit.Text = "Save"

        row1.Visible = False

        row3.Visible = False
        lblwidth.Visible = False
        txtwidth.Visible = False

        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
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
            da.SelectCommand.CommandText = "select * from mmm_mst_dashboard where eid=" & Session("EID") & " and tid=" & Tid & ""
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                txtname.Text = ds.Tables("data").Rows(0).Item("dbname").ToString()
                'ddldbtype.SelectedItem.Value = ds.Tables("data").Rows(0).Item("dbtype").ToString()

                ddldbtype.SelectedIndex = ddldbtype.Items.IndexOf(ddldbtype.Items.FindByValue(ds.Tables("data").Rows(0).Item("dbtype").ToString()))

                Dim ste As String = ds.Tables("data").Rows(0).Item("displayon").ToString()
                If ste = "Mobile" Then
                    rdbtn.SelectedIndex = 0
                Else
                    rdbtn.SelectedIndex = 1

                End If

                ddlwtype.SelectedIndex = ddlwtype.Items.IndexOf(ddlwtype.Items.FindByValue(ds.Tables("data").Rows(0).Item("widgettype").ToString()))
                If ddlwtype.SelectedItem.Text = "New" Then
                    row1.Visible = True
                    row3.Visible = True
                Else
                    row1.Visible = False

                    row3.Visible = False
                   
                End If
                If ddldbtype.SelectedItem.Text = "Home" Then
                    lblwidth.Visible = False
                    txtwidth.Visible = False
                    lblcellpos.Visible = False
                    txtcellposition.Visible = False
                Else
                    lblwidth.Visible = True
                    txtwidth.Visible = True
                    lblcellpos.Visible = True
                    txtcellposition.Visible = True
                End If
                txtroot.Text = ds.Tables("data").Rows(0).Item("rootquery").ToString()
                txtfirst.Text = ds.Tables("data").Rows(0).Item("firstlevelquery").ToString()
                txtsecond.Text = ds.Tables("data").Rows(0).Item("secondlevelquery").ToString()

                Dim str As String = ds.Tables("data").Rows(0).Item("roles").ToString()
                str = str.Replace("{", "")
                str = str.Replace("}", "")

                Dim ab As String() = str.Split(",")
                For Each li As ListItem In chkroles.Items
                    For j As Integer = 0 To ab.Length - 1
                        If li.Text = ab(j).ToString() Then
                            li.Selected = True
                        End If
                    Next
                Next

                txtwidth.Text = ds.Tables("data").Rows(0).Item("dbwidth").ToString()

                ddlwnature.SelectedIndex = ddlwnature.Items.IndexOf(ddlwnature.Items.FindByValue(ds.Tables("data").Rows(0).Item("widgetnature").ToString()))
                If ddlwnature.SelectedItem.Text = "Grid" Then
                    lblcellpos.Visible = True
                    txtcellposition.Visible = True
                Else
                    lblcellpos.Visible = False
                    txtcellposition.Visible = False
                End If
                txtcellposition.Text = ds.Tables("data").Rows(0).Item("cellposition").ToString()



            End If
            updatePanelEdit.Update()
            lblmsg.Text = ""
        Catch ex As Exception

        End Try





        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgDelFolder.Text = "Are You Sure Want to Delete this Configuration"
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TID") = Tid
        Me.updatePanelEdit.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspdeletedashboard", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@Tid", ViewState("TID"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        lblmsg.Text = oda.SelectCommand.ExecuteScalar()
        getresult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        updatePanelDelFolder.Update()
        btnDelFolder_ModalPopupExtender.Hide()

    End Sub

    Protected Sub editrecord()
        If Trim(txtname.Text) = "" Then
            lblMsgEdit.Text = "* Name Cannot be Blank!!"
            Exit Sub
        End If
        If Trim(txtname.Text).Length < 3 Then
            lblMsgEdit.Text = "* Please write a valid name!!"
            Exit Sub
        End If

        If ddldbtype.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Valid DashBoard Type!!"
            Exit Sub
        End If
        Dim str As String = String.Empty
        Dim dnat As String = String.Empty
        If ddlwtype.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Valid Widget Type!!"
            Exit Sub
        ElseIf ddlwtype.SelectedItem.Text = "New" Then
            If Trim(txtroot.Text) = "" Then
                lblMsgEdit.Text = "* Query cannot be blank!!"

                Exit Sub
            End If
            If Trim(txtroot.Text).Length < 20 Then
                lblMsgEdit.Text = "* Query cannot be less than 20 characters!!"

                Exit Sub
            End If
            'If chkroles.SelectedIndex = -1 Then
            '    lblMsgEdit.Text = "* Please Select a Valid Role!!!!"
            '    Exit Sub
            'End If

            'If ddlwnature.SelectedItem.Text = "Grid" Then
            '    If Trim(txtcellposition.Text) = "" Then
            '        lblMsgEdit.Text = "Cellpostion Cannot Be Blank!!!"
            '        Exit Sub
            '    End If


            'End If
            dnat = ddlwnature.SelectedItem.Text.ToString()
           
        End If

        Dim cnt As Integer = 0

        For Each li As ListItem In chkroles.Items
            If li.Selected Then
                cnt = cnt + 1
                str = str & "{" & li.Text.ToString() & "},"
            End If
        Next

        str = str.Remove(str.Length - 1)
        If cnt < 1 Then
            lblMsgEdit.Text = "* Please Select a Role!!"

            Exit Sub
        End If

        If UCase(Trim(ddldbtype.SelectedItem.Text)) = "DASHBOARD" Then
            If txtwidth.Text.Length < 1 Then
                lblMsgEdit.Text = "* Please enter Width!!"
                Exit Sub
            End If
        End If



        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspinsertDashboard", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@Eid", Session("EID"))
            oda.SelectCommand.Parameters.AddWithValue("@DBName", Trim(txtname.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@DBType", Trim(ddldbtype.SelectedValue.ToString()))
            oda.SelectCommand.Parameters.AddWithValue("@WidgetType", Trim(ddlwtype.SelectedItem.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@DBWidth", Trim(txtwidth.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@WidgetNature", Trim(dnat).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@RootQuery", Trim(txtroot.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@FirstLevelQuery", Trim(txtfirst.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@SecondLevelQuery", Trim(txtsecond.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@Roles", str.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@CellPosition", Trim(txtcellposition.Text).ToString())
            oda.SelectCommand.Parameters.AddWithValue("@Createdon", DateTime.Now)
            oda.SelectCommand.Parameters.AddWithValue("@Createdby", Session("UID"))
            oda.SelectCommand.Parameters.AddWithValue("@displayon", rdbtn.SelectedItem.Text.Trim().ToString())

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblmsg.Text = oda.SelectCommand.ExecuteScalar()
            Me.btnEdit_ModalPopupExtender.Hide()
            lblmsg.Text = "Created Successfully"

            con.Close()
            oda.Dispose()
            con.Dispose()
            gvData.DataBind()

        Else


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("uspupdateDashboard", con)
            oda1.SelectCommand.CommandType = CommandType.StoredProcedure
            oda1.SelectCommand.CommandType = CommandType.StoredProcedure
            oda1.SelectCommand.Parameters.AddWithValue("@Eid", Session("EID"))
            oda1.SelectCommand.Parameters.AddWithValue("@DBName", Trim(txtname.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@DBType", Trim(ddldbtype.SelectedValue.ToString()))
            oda1.SelectCommand.Parameters.AddWithValue("@WidgetType", Trim(ddlwtype.SelectedItem.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@DBWidth", Trim(txtwidth.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@WidgetNature", Trim(dnat).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@RootQuery", Trim(txtroot.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@FirstLevelQuery", Trim(txtfirst.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@SecondLevelQuery", Trim(txtsecond.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@Roles", str.ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@CellPosition", Trim(txtcellposition.Text).ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@Createdon", DateTime.Now)
            oda1.SelectCommand.Parameters.AddWithValue("@Createdby", Session("UID"))
            oda1.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
            oda1.SelectCommand.Parameters.AddWithValue("@displayon", rdbtn.SelectedItem.Text.Trim().ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda1.SelectCommand.ExecuteNonQuery()
            lblmsg.Text = "Updated Successfully"
            ClientScript.RegisterStartupScript(Me.GetType(), "AlertScript", "alert('Updated Successfully');", True)
            updatePanelEdit.Update()
            btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda1.Dispose()
            con.Dispose()
        End If
        getresult()






    End Sub

    Public Sub reset()
        txtroot.Text = String.Empty
        txtfirst.Text = String.Empty
        txtsecond.Text = String.Empty
        txtname.Text = String.Empty
        ddldbtype.SelectedIndex = 0
        ddlwtype.SelectedIndex = 0
        txtwidth.Text = String.Empty
        txtcellposition.Text = String.Empty

    End Sub



    Protected Sub ddlwtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlwtype.SelectedIndexChanged
        If ddlwtype.SelectedItem.Text = "New" Then
            row1.Visible = True
            row3.Visible = True
         
        Else
            row1.Visible = False
            ddlwnature.Items.Clear()
            row3.Visible = False
         
        End If

    End Sub

    'Protected Sub ddlwnature_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlwnature.SelectedIndexChanged
    '    If ddlwnature.SelectedItem.Text = "Grid" Then
    '        lblcellpos.Visible = True
    '        txtcellposition.Visible = True
    '    Else
    '        lblcellpos.Visible = False
    '        txtcellposition.Visible = False
    '    End If
    'End Sub



    Protected Sub ddldbtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldbtype.SelectedIndexChanged
        If ddldbtype.SelectedItem.Text = "Home" Then
            lblwidth.Visible = False
            txtwidth.Visible = False
            lblcellpos.Visible = False
            txtcellposition.Visible = False
        Else
            lblwidth.Visible = True
            txtwidth.Visible = True
            lblcellpos.Visible = True
            txtcellposition.Visible = True
        End If
    End Sub

    'Protected Sub ChangePreference(sender As Object, e As EventArgs)
    '    Dim commandArgument As String = TryCast(sender, LinkButton).CommandArgument

    '    Dim rowIndex As Integer = TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex
    '    Dim preference As Integer = Convert.ToInt32(gvData.Rows(rowIndex).Cells(7).Text)
    '    Dim tid As Integer = Convert.ToInt32(gvData.DataKeys(rowIndex).Value)
    '    preference = If(commandArgument = "up", preference - 1, preference + 1)
    '    Me.UpdatePreference(tId, preference)

    '    rowIndex = If(commandArgument = "up", rowIndex - 1, rowIndex + 1)
    '    tId = Convert.ToInt32(gvData.Rows(rowIndex).Cells(7).Text)
    '    preference = Convert.ToInt32(gvData.DataKeys(rowIndex).Value)
    '    preference = If(commandArgument = "up", preference + 1, preference - 1)
    '    Me.UpdatePreference(tId, preference)

    '    Response.Redirect(Request.Url.AbsoluteUri)
    'End Sub
    'Private Sub UpdatePreference(tId As Integer, preference As Integer)
    '    Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
    '    Using con As New SqlConnection(constr)
    '        Using cmd As New SqlCommand("UPDATE mmm_mst_dashboard SET dorder= @Preference WHERE tId = @tId")
    '            Using sda As New SqlDataAdapter()
    '                cmd.CommandType = CommandType.Text
    '                cmd.Parameters.AddWithValue("@tId", tId)
    '                cmd.Parameters.AddWithValue("@Preference", preference)
    '                cmd.Connection = con
    '                con.Open()
    '                cmd.ExecuteNonQuery()
    '                con.Close()
    '            End Using
    '        End Using
    '    End Using
    'End Sub

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
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDashboardPositionUpdate", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        oda.SelectCommand.Parameters.AddWithValue("ntid", ntid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        getresult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        updatePanelEdit.Update()
        lblmsg.Text = ""
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
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDashboardPositionUpdate", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tid)
        oda.SelectCommand.Parameters.AddWithValue("ntid", ntid)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        getresult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        updatePanelEdit.Update()
        lblmsg.Text = ""
    End Sub
End Class
