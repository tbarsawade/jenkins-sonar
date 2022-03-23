
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Globalization

Partial Class RoleMaster
    Inherits System.Web.UI.Page
    Dim obDMS As New DMSUtil()

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
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='ROLE' order by DisName", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            getSearchResult()

            'belode function is for rights on role maser on based on menu
            Call GetMenuDataandroles()
            da.Dispose()
            con.Dispose()

        End If
    End Sub

    Private Sub GetMenuDataandroles()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        ' Dim scrname As String = Request.QueryString("SC").ToString()
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
        da.SelectCommand.CommandText = "select * from mmm_mst_menu where  pagelink='RoleMaster.aspx' and eid=" & Session("EID") & ""
        da.Fill(ds, "menu")
        If ds.Tables("menu").Rows.Count > 0 Then
            Dim rol As String() = ds.Tables("menu").Rows(0).Item("Roles").ToString().Split(",")
            For j As Integer = 0 To rol.Length - 1
                Dim a As String() = rol(j).ToString().Split(":")
                If Session("USERROLE") = a(0).Remove(0, 1).ToString() Then
                    ViewState("numval") = a(1).Remove(a(1).Length - 1)
                End If
            Next
            If ViewState("numval") = 1 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btne.Visible = False
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 2 Then
                gvData.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btne.Visible = False
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 3 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btne.Visible = False
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 4 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 5 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 6 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 7 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 8 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 9 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 10 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 11 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnEdit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 12 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
            ElseIf ViewState("numval") = 13 Then
                btnNew.Visible = False
                btnimport.Visible = False
                FileUpload.Visible = False
            ElseIf ViewState("numval") = 15 Then

            End If
        End If
        con.Close()
        da.Dispose()
    End Sub

    Public Function CreateRoleMasterLogForDelete(ByVal RoleId As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from mmm_mst_role where RoleId=" & RoleId & " and EID=" & Session("EID").ToString())
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                Logstring += " Role Name : " & dt.Rows(0).Item("RoleName") & ""
                Logstring += " deleted By Uid - " & Session("UID")
            End If
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function

    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim path As String = txtrole.Text
        path = path.Replace(" ", "")
        Dim RoleLog = CreateRoleMasterLogForDelete(ViewState("Roleid"))
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspdeleterole", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@rolename", Trim(path))
        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@roleid", ViewState("Roleid"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        lblMsgEdit.Text = oda.SelectCommand.ExecuteScalar()

        Dim objDMSUtil As New DMSUtil()
        objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Role Master", "Role Master Deleted : " & RoleLog, ViewState("Roleid"))
        getSearchResult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        btnDelFolder_ModalPopupExtender.Hide()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        lblMsgDelFolder.ForeColor = Drawing.Color.Red
        lblMsgDelFolder.Text = "Are You Sure Want to Delete"

        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Rid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("Roleid") = Rid
        txtrole.Text = row.Cells(1).Text
        txtdesc.Text = row.Cells(2).Text
        getSearchResult()
        Me.updatePanelEdit.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Rid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("Roleid") = Rid
        txtrole.Text = row.Cells(1).Text
        ViewState("editrole") = Trim(txtrole.Text)
        txtdesc.Text = row.Cells(2).Text

        ddlaccesstype.Items.Clear()
        ddlaccesstype.Items.Insert(0, New ListItem("Web", 0))
        ddlaccesstype.Items.Insert(1, New ListItem("Tab", 1))
        ddlaccesstype.Items.Insert(2, New ListItem("Both", 2))
        ddlaccesstype.DataBind()
        Dim abc As String = row.Cells(4).Text.ToString()
        Dim va As Integer = 0
        If abc = "Web" Then
            va = 0
        ElseIf abc = "Tab" Then
            va = 1
        ElseIf abc = "Both" Then
            va = 2
        End If
        ddlaccesstype.SelectedValue = va
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable()
        oda.SelectCommand.CommandText = "select FormName from mmm_mst_forms where eid=" & Session("EID") & " and isactive=1 and FormSource='Menu Driven' and FormType='Document'"
        oda.Fill(dt)
        If ddldefdoc.Items.Count > 0 Then
            ddldefdoc.Items.Clear()
        End If
        If dt.Rows.Count > 0 Then
            ddldefdoc.DataSource = dt
            ddldefdoc.DataTextField = "FormName"
            ddldefdoc.DataValueField = "FormName"
            ddldefdoc.DataBind()
        End If
        Dim RoleType As String = Convert.ToString(row.Cells(3).Text)
        If RoleType <> "" Then
            ddlrtype.SelectedValue = RoleType
        Else
            ddlrtype.SelectedIndex = 0
        End If

        Dim allowedRoles As String = Convert.ToString(row.Cells(7).Text)
        If allowedRoles = "0" Then
            chkVacation1.Checked = False
        ElseIf (allowedRoles = "1") Then
            chkVacation1.Checked = True
        Else
            chkVacation1.Checked = False
        End If

        ddldefdoc.Items.Insert("0", New ListItem("SELECT"))
        Dim abcd As String = Convert.ToString(row.Cells(5).Text)
        If abcd.Length > 1 Then
            If abcd <> "&nbsp;" Then
                ddldefdoc.SelectedValue = abcd
            Else
                ddldefdoc.SelectedIndex = 0
            End If

        Else
            ddldefdoc.SelectedIndex = 0
        End If

        Dim odaNew As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtNew As New DataTable()
        odaNew.SelectCommand.CommandText = "select FormName from mmm_mst_forms where eid=" & Session("EID") & " and isactive=1 and isroledef=1"
        odaNew.Fill(dtNew)
        If lstmappedroleassignment.Items.Count > 0 Then
            lstmappedroleassignment.Items.Clear()
        End If
        If dtNew.Rows.Count > 0 Then
            lstmappedroleassignment.DataSource = dtNew
            lstmappedroleassignment.DataTextField = "FormName"
            lstmappedroleassignment.DataValueField = "FormName"
            lstmappedroleassignment.DataBind()
        End If
        Dim mappedroleassignment As String = Convert.ToString(row.Cells(6).Text)
        If (lstmappedroleassignment.Items.Count > 0) And (mappedroleassignment.Split(",").Count > 0) Then
            Dim i2 As Integer
            For i2 = 0 To lstmappedroleassignment.Items.Count - 1
                Dim i As Integer
                For i = 0 To mappedroleassignment.Split(",").Count - 1
                    If lstmappedroleassignment.Items(i2).Text.ToString().Trim() = mappedroleassignment.Split(",")(i).ToString().Trim() Then
                        lstmappedroleassignment.Items(i2).Selected = True
                    End If
                Next
            Next
        End If

        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()

        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "setTimeout(function() {multiselectddl();},100);", True)
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        txtrole.Text = ""
        txtdesc.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable()
        oda.SelectCommand.CommandText = "select FormName from mmm_mst_forms where eid=" & Session("EID") & " and isactive=1 and FormSource='Menu Driven' and FormType='Document'"
        oda.Fill(dt)
        If ddldefdoc.Items.Count > 0 Then
            ddldefdoc.Items.Clear()
        End If
        If dt.Rows.Count > 0 Then
            ddldefdoc.DataSource = dt
            ddldefdoc.DataTextField = "FormName"
            ddldefdoc.DataValueField = "FormName"
            ddldefdoc.DataBind()
        End If
        ddldefdoc.Items.Insert("0", New ListItem("SELECT"))

        Dim odaNew As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtNew As New DataTable()
        odaNew.SelectCommand.CommandText = "select FormName from mmm_mst_forms where eid=" & Session("EID") & " and isactive=1 and isroledef=1"
        odaNew.Fill(dtNew)
        If lstmappedroleassignment.Items.Count > 0 Then
            lstmappedroleassignment.Items.Clear()
        End If
        If dtNew.Rows.Count > 0 Then
            lstmappedroleassignment.DataSource = dtNew
            lstmappedroleassignment.DataTextField = "FormName"
            lstmappedroleassignment.DataValueField = "FormName"
            lstmappedroleassignment.DataBind()
        End If

        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "setTimeout(function() {multiselectddl();},100);", True)
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        If Trim(txtrole.Text) = "" Or Trim(txtrole.Text).Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid Role"
            Exit Sub
        End If
        If Trim(txtdesc.Text) = "" Or Trim(txtdesc.Text).Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid Role "
            Exit Sub
        End If
        Dim path As String = txtrole.Text
        path = path.Replace(" ", "")
        lblMsgupdate.Text = ""


        Dim val As Integer = 0
        If ddlaccesstype.SelectedItem.Text = "Web" Then
            val = 0
        ElseIf ddlaccesstype.SelectedItem.Text = "Tab" Then
            val = 1
        Else
            val = 2
        End If

        Dim chkVacationrule As Integer

        If chkVacation1.Checked = True Then
            chkVacationrule = 1
        Else
            chkVacationrule = 0
        End If

        Dim MappedForm As String = ""
        Dim chkBoxListValue As New ArrayList()
        For Each li As ListItem In lstmappedroleassignment.Items
            If li.Selected = True Then
                chkBoxListValue.Add(li.Value)
            End If
        Next
        If chkBoxListValue.Count > 0 Then
            MappedForm = String.Join(",", chkBoxListValue.ToArray())
        Else
            MappedForm = "0"
        End If
        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspinsertrole", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@role", Trim(path))
            oda.SelectCommand.Parameters.AddWithValue("@roledescription", Trim(txtdesc.Text))
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@roletype", ddlrtype.SelectedItem.Text.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@accesstype", val)
            oda.SelectCommand.Parameters.AddWithValue("@MainHome_DefaultDcoument", IIf(ddldefdoc.SelectedIndex = 0, "", ddldefdoc.SelectedValue))
            oda.SelectCommand.Parameters.AddWithValue("@mappedroleassignment", IIf(ddlrtype.SelectedValue.ToUpper() = "POST TYPE", MappedForm, ""))
            oda.SelectCommand.Parameters.AddWithValue("@CreatedBy", Session("UID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@AllowedDelegate", chkVacationrule)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblMsgEdit.Text = oda.SelectCommand.ExecuteScalar()
            Me.btnEdit_ModalPopupExtender.Hide()
            txtrole.Text = ""
            txtdesc.Text = ""
            con.Close()
            oda.Dispose()
            con.Dispose()

            gvData.DataBind()

        Else

            Dim path1 As String = txtrole.Text
            path1 = path1.Replace(" ", "")
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspupdaterole", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@role", Trim(path1))
            oda.SelectCommand.Parameters.AddWithValue("@roledescription", Trim(txtdesc.Text))
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@rid", ViewState("Roleid"))
            oda.SelectCommand.Parameters.AddWithValue("@roletype", ddlrtype.SelectedItem.Text.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@accesstype", val)
            oda.SelectCommand.Parameters.AddWithValue("@MainHome_DefaultDcoument", IIf(ddldefdoc.SelectedIndex = 0, "", ddldefdoc.SelectedValue))
            oda.SelectCommand.Parameters.AddWithValue("@mappedroleassignment", IIf(ddlrtype.SelectedValue.ToUpper() = "POST TYPE", MappedForm, ""))
            oda.SelectCommand.Parameters.AddWithValue("@AllowedDelegate", chkVacationrule)

            Dim objDMSUtil As New DMSUtil()
            objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Role Master", "Role Master Updation : " & CreateRoleModificationLog(val, MappedForm, ViewState("Roleid")), ViewState("Roleid"))

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblMsgupdate.Text = oda.SelectCommand.ExecuteScalar()

            gvData.DataBind()
            updatePanelEdit.Update()
            btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda.Dispose()
            con.Dispose()
        End If
        getSearchResult()
    End Sub

    Public Function CreateRoleModificationLog(ByVal accesstype As String, ByVal MappedRole As String, ByVal RoleId As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from mmm_mst_role with(nolock) where RoleId=" & RoleId & " and EID=" & Session("EID").ToString())
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                If dt.Rows(0).Item("RoleName").ToString() <> txtrole.Text Then
                    Logstring += " Role Name : " & dt.Rows(0).Item("RoleName") & " Change " & txtrole.Text
                End If
                If dt.Rows(0).Item("RoleDescription").ToString() <> txtdesc.Text Then
                    Logstring += "|| Role Description : " & dt.Rows(0).Item("RoleDescription") & " Change " & txtdesc.Text
                End If
                If dt.Rows(0).Item("RoleType").ToString() <> ddlrtype.SelectedItem.Text.ToString() Then
                    Logstring += "|| Role Type : " & dt.Rows(0).Item("RoleType") & " Change " & ddlrtype.SelectedItem.Text.ToString()
                End If
                If dt.Rows(0).Item("mappedroleassignment").ToString() <> MappedRole Then
                    Logstring += "|| Mapped role assignment : " & dt.Rows(0).Item("mappedroleassignment") & " Change " & MappedRole
                End If
                If dt.Rows(0).Item("accesstype").ToString() <> accesstype Then
                    Dim AccessVal As String
                    If accesstype = "0" Then
                        AccessVal = "Web"
                    ElseIf accesstype = "1" Then
                        AccessVal = "Tab"
                    Else
                        AccessVal = "Both"
                    End If
                    Logstring += "|| Access Type : " & dt.Rows(0).Item("accesstype").ToString() & " Change " & AccessVal
                End If

            End If
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataBind()
        If gvData.Rows.Count > 0 Then
            lblMsgupdate.Text = gvData.Rows.Count & " Records Found "
        Else
            lblMsgupdate.Text = " No record found "
        End If
    End Sub
    Private Function getSearchResult() As DataTable
        gvData.DataBind()

    End Function

    Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            'Dim CID As String = Session("EID")
            If FileUpload.HasFile Then

                If Right(FileUpload.FileName, 4).ToUpper() = ".CSV" Then

                    Dim filename As String = Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(FileUpload.FileName, 4).ToUpper()
                    FileUpload.PostedFile.SaveAs(Server.MapPath(filename))
                    Dim sField As String()
                    Dim csvReader As Microsoft.VisualBasic.FileIO.TextFieldParser
                    csvReader = My.Computer.FileSystem.OpenTextFieldParser(Server.MapPath(filename), ",")
                    Dim icnt As Integer
                    Dim path As String = ""
                    Dim da As New SqlDataAdapter("uspimportrole", con)
                    Dim ds As New DataSet
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    With csvReader
                        .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
                        .Delimiters = New String() {","}
                        Dim UID As String = Session("UID")
                        While Not .EndOfData
                            Try
                                sField = .ReadFields()
                                If icnt < 1 Then
                                    If UCase(sField(0)) <> UCase("ROLE NAME") Then
                                        lblMsgupdate.ForeColor = Drawing.Color.Red
                                        lblMsgupdate.Text = "Role Name not found"
                                        Exit Sub
                                    End If
                                    If UCase(sField(1)) <> UCase("ROLE DESCRIPTION") Then
                                        lblMsgupdate.ForeColor = Drawing.Color.Red
                                        lblMsgupdate.Text = "Role Description not found"
                                        Exit Sub
                                    End If
                                    If UCase(sField(2)) <> UCase("ROLE TYPE") Then
                                        lblMsgupdate.ForeColor = Drawing.Color.Red
                                        lblMsgupdate.Text = "Role Description not found"
                                        Exit Sub
                                    End If
                                    If UCase(sField(3)) <> UCase("ACCESS TYPE") Then
                                        lblMsgupdate.ForeColor = Drawing.Color.Red
                                        lblMsgupdate.Text = "Access Type not found"
                                        Exit Sub
                                    End If
                                    path = sField(0).ToString()
                                    path = path.Replace(" ", "")
                                    icnt += 1
                                    Continue While
                                End If
                                icnt += 1

                                da.SelectCommand.CommandText = "uspimportrole"
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.Clear()

                                da.SelectCommand.Parameters.AddWithValue("@role", path)
                                da.SelectCommand.Parameters.AddWithValue("@roledescription", sField(1))
                                da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                                da.SelectCommand.Parameters.AddWithValue("@roletype", ddlrtype.SelectedItem.Text.ToString())
                                da.SelectCommand.Parameters.AddWithValue("@accesstype", ddlaccesstype.SelectedValue)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                da.SelectCommand.ExecuteNonQuery()

                                getSearchResult()
                                ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "HideLabel", String.Format("window.setTimeout('document.getElementById(""{0}"").style.display = ""none"";', 2000);", lblMsgupdate.ClientID), True)
                                da.Dispose()
                                con.Close()

                            Catch ex As Exception
                                lblMsgupdate.Text = ex.Message
                            End Try
                        End While
                        'lblMsgupdate.ForeColor = Drawing.Color.Red
                        'lblMsgupdate.Text = "Roles Inserted Successfully"

                    End With
                Else
                    lblMsgupdate.ForeColor = Drawing.Color.Red
                    lblMsgupdate.Text = "File should be of CSV Format"
                    Exit Sub
                End If
            Else
                lblMsgupdate.ForeColor = Drawing.Color.Red
                lblMsgupdate.Text = "Please select a File to Upload"
                Exit Sub
            End If
        Catch ex As Exception
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Text = "An error occured when importing data. Please try again"
        End Try
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        ' Verifies that the control is rendered
    End Sub
    Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexport.Click
        Try
            Response.ClearContent()
            gvData.AllowPaging = False
            gvData.FooterRow.Visible = False
            gvData.Columns(5).Visible = False
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("content-disposition", "attachment;filename=RoleMaster"".xls")
            'Prepare to export the DataGrid
            Dim oStringWriter As New System.IO.StringWriter
            Dim oHtmlTextWriter As New HtmlTextWriter(oStringWriter)

            'Use the DataGrid control to add the details
            gvData.RenderControl(oHtmlTextWriter)
            'Finish the Excel spreadsheet and send the response
            Response.Write(oStringWriter.ToString())
            ' gridDocs0.Dispose()
            Response.End()

        Catch ex As Exception
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Text = "An error occured when exporting data. Please try again"
        End Try
    End Sub
    Protected Sub gvData_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvData.SelectedIndexChanged

    End Sub
End Class

