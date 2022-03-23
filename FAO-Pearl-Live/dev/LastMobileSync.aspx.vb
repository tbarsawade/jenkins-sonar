Imports System.Data.SqlClient
Imports System.Data

Partial Class LastMobileSync
    Inherits System.Web.UI.Page

    Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim cons As New SqlConnection(conStrs)
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindMobileSync()
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

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Clear()
        BindInformation()
        btnlogin.Text = "Save"
        UpdatePanel1.Update()
        btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub Clear()
        txtNoofdays.Text = ""
        If ddlDocType.Items.Count > 0 Then
            ddlDocType.SelectedIndex = 0
        End If
        If ddlRoleName.Items.Count > 0 Then
            ddlRoleName.SelectedIndex = 0
        End If
        If ddlBasedCon.Items.Count > 0 Then
            ddlBasedCon.SelectedIndex = 0
        End If
        chkisActive.Checked = False
        lblMsg.Text = ""
        btnlogin.Text = "Save"
    End Sub
    Protected Sub BindInformation()
        'Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim cons As New SqlConnection(conStrs)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", cons)
        da.SelectCommand.CommandText = "select RoleID,RoleDescription from mmm_mst_role where eid=" & Session("EID") & " ;select FormName from mmm_mst_forms where eid=" & Session("EID") & " and formType='DOCUMENT' and FORMSOURCE='MENU DRIVEN' and isactive=1"
        Dim ds As New DataSet()
        da.Fill(ds, "VAL")
        If ds.Tables("VAL").Rows.Count > 0 Then
            ddlRoleName.DataSource = ds.Tables("VAL")
            ddlRoleName.DataTextField = "RoleDescription"
            ddlRoleName.DataValueField = "RoleID"
            ddlRoleName.DataBind()
            ddlRoleName.Items.Insert("0", New ListItem("Select"))
            ddlRoleName.SelectedIndex = 0
        End If
        If ds.Tables("VAL1").Rows.Count > 0 Then
            ddlDocType.DataSource = ds.Tables("VAL1")
            ddlDocType.DataTextField = "FormName"
            ddlDocType.DataValueField = "FormName"
            ddlDocType.DataBind()
            ddlDocType.Items.Insert("0", New ListItem("Select"))
            ddlDocType.SelectedIndex = 0
        End If

    End Sub
    Protected Sub btnlogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnlogin.Click
        If ddlDocType.SelectedIndex = 0 Then
            lblForm.Text = "Please Select Doucmenttype"
            ddlDocType.Focus()
            Exit Sub
        ElseIf ddlRoleName.SelectedIndex = 0 Then
            lblForm.Text = "Please Select RoleName"
            ddlRoleName.Focus()
            Exit Sub
        ElseIf txtNoofdays.Text = String.Empty Then
            lblForm.Text = "Please Enter No of Days"
            txtNoofdays.Focus()
            Exit Sub
        ElseIf ddlBasedCon.SelectedIndex = 0 Then
            lblForm.Text = "Please Select Based Condition"
            ddlBasedCon.Focus()
            Exit Sub
        End If

        If btnlogin.Text.ToUpper = "SAVE" Then
            'Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim cons As New SqlConnection(conStrs)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", cons)
            Try
                da.SelectCommand.CommandText = "insert into mmm_mst_lastMobileSync(Documenttype,EID,NoOfDays,LastUpdate,IsActive,RoleId,Field) values('" & ddlDocType.SelectedValue.Trim() & "'," & Session("EID") & "," & txtNoofdays.Text & ",getdate(),'" & chkisActive.Checked & "'," & ddlRoleName.SelectedValue & ",'" & ddlBasedCon.SelectedValue.Trim() & "')"
                If cons.State = ConnectionState.Closed Then
                    cons.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()
                cons.Close()
            Catch ex As Exception
            End Try
        ElseIf btnlogin.Text.ToUpper = "UPDATE" Then
            Dim da As SqlDataAdapter = New SqlDataAdapter("", cons)

            Try
                da.SelectCommand.CommandText = "update mmm_mst_lastMobileSync set Documenttype ='" & ddlDocType.SelectedValue.Trim() & "',EID = " & Session("EID") & ",NoOfDays =" & txtNoofdays.Text & ",LastUpdate=getdate(),IsActive ='" & chkisActive.Checked & "',RoleId=" & ddlRoleName.SelectedValue & ",Field='" & ddlBasedCon.SelectedValue & "' where syncid=" & ViewState("SyncID")
                If cons.State = ConnectionState.Closed Then
                    cons.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()
                cons.Close()
            Catch ex As Exception

            End Try
        End If
        BindMobileSync()
        updPnlGrid.Update()
        btnForm_ModalPopupExtender.Hide()
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        BindInformation()
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Formid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", cons)
        da.SelectCommand.CommandText = "select * from mmm_mst_lastMobileSync where syncid=" & Formid
        Dim dt As New DataTable()
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            ddlDocType.SelectedValue = dt.Rows(0)("Documenttype")
            ddlDocType_SelectedIndexChanged(ddlDocType, New EventArgs())
            ddlBasedCon.SelectedValue = dt.Rows(0)("Field")
            ddlRoleName.SelectedValue = dt.Rows(0)("RoleID")
            If dt.Rows(0)("IsActive") = "True" Then
                chkisActive.Checked = True
            Else
                chkisActive.Checked = False
            End If
        End If
        txtNoofdays.Text = dt.Rows(0)("NoOfDays")
        btnlogin.Text = "Update"
        ViewState("SyncID") = Formid
        UpdatePanel1.Update()
        btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub BindMobileSync()
        'Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim cons As New SqlConnection(conStrs)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", cons)
        If cons.State = ConnectionState.Closed Then
            cons.Open()
        End If
        da.SelectCommand.CommandText = "select SyncID, Documenttype,noofdays,Rolename,case isactive when 'True' then 'Yes' else 'No'  end as [isactive] from  mmm_mst_lastMobileSync as a inner join mmm_mst_role as b on a.roleid=b.roleid and a.eid=b.eid where a.EID=" & Session("EID")
        Dim dt As New DataTable()
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            gvData.DataSource = dt
            gvData.DataBind()
        End If
        cons.Close()
    End Sub

    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", cons)
        If cons.State = ConnectionState.Closed Then
            cons.Open()
        End If
        da.SelectCommand.CommandText = "select displayname,fieldMapping from mmm_mst_fields where documenttype='" & ddlDocType.SelectedItem.Text.Trim() & "' and eid=" & Session("EID") & " and datatype='Datetime' union all select name as displayname,name as fieldMapping from sys.columns where object_id=object_id('mmm_mst_doc') and name not like'fld%' and system_type_id=61"
        Dim dt As New DataTable()
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            ddlBasedCon.DataSource = dt
            ddlBasedCon.DataTextField = "displayname"
            ddlBasedCon.DataValueField = "fieldMapping"
            ddlBasedCon.DataBind()
        Else
            If ddlBasedCon.Items.Count > 0 Then
                ddlBasedCon.Items.Clear()
            End If
        End If
        ddlBasedCon.Items.Insert("0", New ListItem("Select"))

    End Sub
End Class
