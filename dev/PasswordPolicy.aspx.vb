Imports System.Data
Imports System.Data.SqlClient

Partial Class PasswordPolicy
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT minchar,maxchar,passtype,passExpDays,passExpMsgDays,autoUnlockHour,minPassAttempt,isgpsactivated FROM MMM_MST_ENTITY WHERE EID=" & Session("EID").ToString(), con)
            Dim ds As New DataSet
            oda.Fill(ds, "data")
            txtMinCHar.Text = ds.Tables("data").Rows(0).Item("minchar").ToString()
            txtMaxChar.Text = ds.Tables("data").Rows(0).Item("maxchar").ToString()
            ddlPasswordType.Text = ds.Tables("data").Rows(0).Item("passtype").ToString()
            txtPassExpDays.Text = ds.Tables("data").Rows(0).Item("passExpDays").ToString()
            txtPassExpMsgDays.Text = ds.Tables("data").Rows(0).Item("passExpMsgDays").ToString()
            txtAutoUnlockHour.Text = ds.Tables("data").Rows(0).Item("autoUnlockHour").ToString()
            txtPasswordAttempt.Text = ds.Tables("data").Rows(0).Item("minPassAttempt").ToString()
            If ds.Tables("data").Rows(0).Item("isgpsactivated") = 1 Then
                chkActivate.Checked = True
            Else
                chkActivate.Checked = False
            End If
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
    Protected Sub btnlogin_Click(sender As Object, e As System.EventArgs) Handles btnlogin.Click
        If Val(txtMinCHar.Text) < 0 Then
            lblMsg.Text = "Please Enter Valid minimum Character"
            Exit Sub
        End If

        If Val(txtMaxChar.Text) < 0 Then
            lblMsg.Text = "Please Enter maximum value"
            Exit Sub
        End If

        'If Val(txtPassExpDays) < 0 Then
        '    lblMsg.Text = "Please Enter expiry days "
        '    Exit Sub
        'End If

        'If Val(txtPassExpMsgDays) < 0 Then
        '    lblMsg.Text = "Please Enter expiry message "
        '    Exit Sub
        'End If

        If Val(txtAutoUnlockHour.Text) < 0 Then
            lblMsg.Text = "Please Enter minimum Unlocking hours "
            Exit Sub
        End If

        If Val(txtPasswordAttempt.Text) < 0 Then
            lblMsg.Text = "Please Enter numbers of wrong attempts before account get blocked"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("UPDATE MMM_MST_ENTITY SET minchar=" & txtMinCHar.Text & ",maxchar=" & txtMaxChar.Text & ",passtype='" & ddlPasswordType.SelectedItem.Text & "',passExpDays=" & txtPassExpDays.Text & ",passExpMsgDays=" & txtPassExpMsgDays.Text & ",autoUnlockHour=" & txtAutoUnlockHour.Text & ",minPassAttempt=" & txtPasswordAttempt.Text & " Where EID=" & Session("EID").ToString(), con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()
        lblMsg.Text = "Password Policy Updated successfully "
    End Sub

    Protected Sub btnradioothers_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnradioothers.CheckedChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select fieldid,displayname,fieldmapping from MMM_MST_FIELDS where eid=" & Session("EID") & " and Documenttype='USER'"
        Dim ds As New DataSet
        da.Fill(ds, "account")

        ddlothers.DataSource = ds
        ddlothers.DataTextField = "displayname"
        ddlothers.DataValueField = "fieldmapping"
        ddlothers.DataBind()
        con.Dispose()
        da.Dispose()

    End Sub

    Protected Sub btnUname_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUname.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        If btnradioemail.Checked = True Then
            da.SelectCommand.CommandText = "update MMM_MST_USER set userid=emailid where eid=" & Session("EID") & ""
            da.SelectCommand.CommandType = CommandType.Text
        Else
            btnradioothers.Checked = True

            da.SelectCommand.CommandText = "update MMM_MST_USER set userid= " & ddlothers.SelectedItem.Value().ToString() & " where eid=" & Session("EID") & ";update MMM_MST_ENTITY set LoginField='" & ddlothers.SelectedItem.Value().ToString() & "' where eid=" & Session("EID")
            da.SelectCommand.CommandType = CommandType.Text
        End If
        da.SelectCommand.CommandType = CommandType.Text
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteNonQuery()

        con.Close()
        da.Dispose()
        lblMessacc.Text = "Updated Successfully"

    End Sub

    Protected Sub btnGpsActivated_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGpsActivated.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        If chkActivate.checked = True Then
            da.SelectCommand.CommandText = "update MMM_MST_ENTITY set isgpsactivated=1 where eid=" & Session("EID") & ""
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            lblGpsAct.Text = "GPS is Activated"
        Else
            da.SelectCommand.CommandText = "update MMM_MST_ENTITY set isgpsactivated=0 where eid=" & Session("EID") & ""
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            lblGpsAct.Text = "GPS is Deactivated"
        End If
        con.Close()
        da.Dispose()
    End Sub
End Class
