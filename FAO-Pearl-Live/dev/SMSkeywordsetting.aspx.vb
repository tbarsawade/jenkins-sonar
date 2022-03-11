Imports System.Data
Imports System.Data.SqlClient
Partial Class SMSkeywordsetting
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BinddataGrid()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("select DISTINCT FORMNAME,formid,formtype FROM MMM_MST_FORMS WHERE FORMSOURCE='MENU DRIVEN' AND ( EID=" & Session("EID") & " OR EID=0)", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddldtype.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddldtype.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            ddldtype.Items.Insert(0, "SELECT")
            ds.Reset()
            da.SelectCommand.CommandText = " select* from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='user' "
            da.Fill(ds, "User")
            For i As Integer = 0 To ds.Tables("User").Rows.Count - 1
                ddlumobno.Items.Add(ds.Tables("User").Rows(i).Item("Displayname"))
                ddlumobno.Items(i).Value = ds.Tables("User").Rows(i).Item("Fieldmapping")
            Next
            ddlumobno.Items.Insert(0, "SELECT")
            da.SelectCommand.CommandText = " select* from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='vehicle' "
            da.Fill(ds, "Driver")
            For i As Integer = 0 To ds.Tables("Driver").Rows.Count - 1
                ddldmobno.Items.Add(ds.Tables("Driver").Rows(i).Item("Displayname"))
                ddldmobno.Items(i).Value = ds.Tables("Driver").Rows(i).Item("Fieldmapping")
            Next
            ddldmobno.Items.Insert(0, "SELECT")

            If ddlstype.SelectedItem.Text = "Authentication" Then
                ddlprc.Visible = False
                lblctype.Visible = False
                lblptype.Visible = False
                ddlopr.Visible = False
            ElseIf ddlstype.SelectedItem.Text = "Existence" Then
                ddlprc.Visible = False
                lblctype.Visible = False
                lblptype.Visible = False
                ddlopr.Visible = False
            ElseIf ddlstype.SelectedItem.Text = "Processing" Then
                ddlprc.Visible = True
                lblctype.Visible = True
                lblptype.Visible = True
                ddlopr.Visible = True
            End If
            If ddlKtype.SelectedItem.Text.ToUpper = "SELECT TYPE" Then
                pnltst.Visible = False
                ddltst.Visible = False
                pnldmobno.Visible = False
                ddldmobno.Visible = False
                pnlddays.Visible = False
                txtddays.Visible = False
            ElseIf ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
                pnltst.Visible = True
                ddltst.Visible = True
                pnldmobno.Visible = True
                ddldmobno.Visible = True
                pnlddays.Visible = True
                txtddays.Visible = True
            Else
                pnltst.Visible = False
                ddltst.Visible = False
                pnldmobno.Visible = False
                ddldmobno.Visible = False
                pnlddays.Visible = False
                txtddays.Visible = False
            End If
            con.Dispose()
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
    Public Sub BinddataGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT tid,keywordname,kwdesc,paracount,mErrAuth,mErrExist,mErrProcess,ktype,mSuccess,ttype,usermobfield,case isActive when 0 then 'InActive' else 'Active' end [isActive],helpingmsg,mErrPara FROM MMM_mst_smskeywords WHERE EID = " & Session("EID").ToString() & " order by keywordname ", con)
        da.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        da.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        updPnlGrid.Update()
        da.Dispose()
        con.Dispose()
    End Sub
    Public Sub BindchildGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT tid,settingtype,documenttype,case when len(fieldmapping)>5 then fieldmapping else (select displayname from mmm_mst_fields where eid=" & Session("EID").ToString() & " and documenttype=s.documenttype and fieldmapping=s.fieldmapping) end [Field],fieldmapping,tablename,keyword,isnull(errmsg,'')[errmsg],isnull(paravalue,'')[paravalue],ProcType,Ctype FROM mmm_sms_settings s WHERE EID = " & Session("EID").ToString() & " and keyword='" & ViewState("kw") & "' order by settingtype ", con)
        da.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        da.Fill(ds, "data")
        gvField.DataSource = ds.Tables("data")
        gvField.DataBind()
        updPnlGrid.Update()
        updatePanelEdit.Update()
        da.Dispose()
        con.Dispose()
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        da.SelectCommand.CommandText = "select usermobfield,DriverMobfld from mmm_mst_smskeywords where tid=" & tid & ""
        da.Fill(dt)
        Dim ufld As String = dt.Rows(0).Item("usermobfield").ToString
        Dim dfld As String = dt.Rows(0).Item("DriverMobfld").ToString
        btnsavekw.Text = "Update"
        txtkeyword.Text = row.Cells(1).Text
        txtkwdesc.Text = row.Cells(2).Text
        txtparacnt.Text = row.Cells(3).Text
        txthmsg.Text = row.Cells(4).Text
        ddlKtype.SelectedIndex = ddlKtype.Items.IndexOf(ddlKtype.Items.FindByText(row.Cells(5).Text))
        ddltst.SelectedIndex = ddltst.Items.IndexOf(ddltst.Items.FindByText(row.Cells(6).Text))
        ddlumobno.SelectedIndex = ddlumobno.Items.IndexOf(ddlumobno.Items.FindByValue(ufld))
        ddldmobno.SelectedIndex = ddldmobno.Items.IndexOf(ddldmobno.Items.FindByValue(dfld))
        Me.UpdatePanel1.Update()
        If ddlKtype.SelectedItem.Text.ToUpper = "SELECT TYPE" Then
            pnltst.Visible = False
            ddltst.Visible = False
            pnldmobno.Visible = False
            ddldmobno.Visible = False
            pnlddays.Visible = False
            txtddays.Visible = False
        ElseIf ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
            pnltst.Visible = True
            ddltst.Visible = True
            pnldmobno.Visible = True
            ddldmobno.Visible = True
            pnlddays.Visible = True
            txtddays.Visible = True
        Else
            pnltst.Visible = False
            ddltst.Visible = False
            pnldmobno.Visible = False
            ddldmobno.Visible = False
            pnlddays.Visible = False
            txtddays.Visible = False
        End If
        con.Dispose()
        Me.btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        lblForm.Text = ""
        txtkeyword.Text = ""
        txtkwdesc.Text = ""
        txthmsg.Text = ""
        txtparacnt.Text = ""
        ddlKtype.SelectedIndex = -1
        ddltst.Visible = False
        btnsavekw.Text = "Save"
        updatePanelEdit.Update()
        UpdatePanel1.Update()
        If ddlKtype.SelectedItem.Text.ToUpper = "SELECT TYPE" Then
            pnltst.Visible = False
            ddltst.Visible = False
            pnldmobno.Visible = False
            ddldmobno.Visible = False
            pnlddays.Visible = False
            txtddays.Visible = False
        ElseIf ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
            pnltst.Visible = True
            ddltst.Visible = True
            pnldmobno.Visible = True
            ddldmobno.Visible = True
            pnlddays.Visible = True
            txtddays.Visible = True
        Else
            pnltst.Visible = False
            ddltst.Visible = False
            pnldmobno.Visible = False
            ddldmobno.Visible = False
            pnlddays.Visible = False
            txtddays.Visible = False
        End If
        Me.btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsavekw.Click
        If Trim(txtkeyword.Text) = "" Or Trim(txtkeyword.Text).Length < 2 Then
            lblForm.Text = "Please Enter Valid KeyWord"
            Exit Sub
        End If
        If Trim(txtkwdesc.Text) = "" Or Trim(txtkwdesc.Text).Length < 2 Then
            lblForm.Text = "Please Enter Valid Description "
            Exit Sub
        End If
        If Trim(txtparacnt.Text) = "" Or Trim(txtparacnt.Text).Length < 0 Then
            lblMsgEdit.Text = "Please Enter Parameter Count"
            Exit Sub
        End If
        If ddlKtype.SelectedItem.Text.ToUpper = "SELECT TYPE" Then
            lblMsgEdit.Text = "Please Select Keyword Type "
            Exit Sub
        End If

        If ddlumobno.SelectedItem.Text.ToUpper = "SELECT" Then
            lblMsgEdit.Text = "Please Select User Mobile Field."
            Exit Sub
        End If
        If ddldmobno.SelectedItem.Text.ToUpper = "SELECT" And ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
            lblMsgEdit.Text = "Please Select Driver Mobile Field."
            Exit Sub
        End If
        If ddlKtype.SelectedItem.Text.ToUpper = "STATIC" And (txtddays.Text = "" Or IsNumeric(txtddays.Text) = False) Then
            lblMsgEdit.Text = "Please Enter valid days"
            Exit Sub
        End If
        '    If Trim(txtErrProcess.Text) = "" Or Trim(txtErrProcess.Text).Length < 2 Then
        '        lblMsgEdit.Text = "Please Enter Error Message"
        '        Exit Sub
        '    End If
        '    If Trim(txtsuccess.Text) = "" Or Trim(txtsuccess.Text).Length < 2 Then
        '        lblMsgEdit.Text = "Please Enter success Message "
        '        Exit Sub
        '    End If
        '    If Trim(txtErrpara.Text) = "" Or Trim(txtErrpara.Text).Length < 2 Then
        '        lblMsgEdit.Text = "Please Enter Error Message"
        '        Exit Sub
        '    End If
        Dim chkval As Integer
        If chkactive.Checked = True Then
            chkval = 1
        Else
            chkval = 0
        End If

        'lblform.Text = ""
        If btnsavekw.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspsmskeywordinsert", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@keyword", Trim(txtkeyword.Text))
            oda.SelectCommand.Parameters.AddWithValue("@keyworddescription", Trim(txtkwdesc.Text))
            oda.SelectCommand.Parameters.AddWithValue("@helpmsg", Trim(txthmsg.Text))
            oda.SelectCommand.Parameters.AddWithValue("@Ktype", ddlKtype.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("@paracount", txtparacnt.Text)
            oda.SelectCommand.Parameters.AddWithValue("@isactive", chkval)
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@umfield", ddlumobno.SelectedValue)
            oda.SelectCommand.Parameters.AddWithValue("@dmfield", ddldmobno.SelectedValue)
            If ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
                oda.SelectCommand.Parameters.AddWithValue("@ttype", ddltst.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@Ddays", txtddays.Text)
            End If
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            Me.btnForm_ModalPopupExtender.Hide()
            lblerr.Text = "Created Successfully"
            BinddataGrid()
            txtkeyword.Text = ""
            txtkwdesc.Text = ""
            con.Close()
            oda.Dispose()
            con.Dispose()
            ddldtype.Items.Clear()
            ddlfn.Items.Clear()
        ElseIf btnsavekw.Text = "Update" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspsmskeywordinsert", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@keyword", Trim(txtkeyword.Text))
            oda.SelectCommand.Parameters.AddWithValue("@keyworddescription", Trim(txtkwdesc.Text))
            oda.SelectCommand.Parameters.AddWithValue("@helpmsg", Trim(txthmsg.Text))
            oda.SelectCommand.Parameters.AddWithValue("@Ktype", ddlKtype.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("@paracount", txtparacnt.Text)
            oda.SelectCommand.Parameters.AddWithValue("@isactive", chkval)
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@umfield", ddlumobno.SelectedValue)
            oda.SelectCommand.Parameters.AddWithValue("@dmfield", ddldmobno.SelectedValue)
            If ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
                oda.SelectCommand.Parameters.AddWithValue("@ttype", ddltst.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@Ddays", txtddays.Text)
            End If
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            lblerr.Text = "Updated Successfully"
            btnForm_ModalPopupExtender.Hide()
            BinddataGrid()
            con.Close()
            oda.Dispose()
            con.Dispose()
            ddldtype.Items.Clear()
            ddlfn.Items.Clear()
        End If
        'getSearchResult()
    End Sub
    Protected Sub PreviewHit(ByVal sender As Object, ByVal e As System.EventArgs)
    End Sub
    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "delete MMM_mst_smskeywords where tid=" & tid & " and eid=" & Session("eid") & "  "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        'oda.SelectCommand.CommandType = CommandType.Text
        'oda.SelectCommand.CommandType = CommandType.Text
        BinddataGrid()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub
    Protected Sub AddFields(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TID") = tid
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select DISTINCT FORMNAME,formid,formtype FROM MMM_MST_FORMS WHERE FORMSOURCE='MENU DRIVEN' AND ( EID=" & Session("EID") & " OR EID=0)"
        Dim ds As New DataSet
        da.Fill(ds, "data")
        ddldtype.Items.Clear()
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddldtype.Items.Add(ds.Tables("data").Rows(i).Item(0))
            ddldtype.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
        Next
        ddldtype.Items.Insert(0, "SELECT")

        da.SelectCommand.CommandText = "select paracount,ktype from mmm_mst_smskeywords where tid=" & tid & ""
        ds.Reset()
        da.Fill(ds, "Ktype")
        Dim pcnt As Integer
        Dim ktype As String = ""
        If ds.Tables("Ktype").Rows.Count > 0 Then
            pcnt = ds.Tables("Ktype").Rows(0).Item(0).ToString
            ktype = ds.Tables("Ktype").Rows(0).Item(1).ToString
        End If

        If ktype.ToUpper = "STATIC" Then
            Exit Sub
        End If

        ddlparavalue.Items.Clear()

        ddlparavalue.Items.Insert(0, "Select")
        ddlparavalue.Items.Insert(1, "MobileNo")
        For i As Integer = 0 To pcnt - 1
            ddlparavalue.Items.Add(i)
            ddlparavalue.Items(i + 2).Value = i + 2
        Next
        ViewState("kw") = row.Cells(1).Text
        btnsavefield.Text = "Save"
        BindchildGrid()
        ddlprc.Visible = False
        lblctype.Visible = False
        lblptype.Visible = False
        ddlopr.Visible = False
        ddlfn.Items.Clear()
        updatePanelEdit.Update()
        ddlstype.SelectedIndex = -1
        ddldtype.SelectedIndex = -1
        ddlopr.SelectedIndex = -1
        ddlprc.SelectedIndex = -1
        ddlparavalue.SelectedIndex = -1
        con.Dispose()
        btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub EditHitField(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvField.DataKeys(row.RowIndex).Value)
        Dim da As New SqlDataAdapter("", con)
        ViewState("TID") = tid
        da.SelectCommand.CommandText = "select settingtype from mmm_sms_settings where tid=" & tid & ""
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim Stype As String = da.SelectCommand.ExecuteScalar()
        If Stype = "Authentication" Then
            ddlprc.Visible = False
            lblctype.Visible = False
            lblptype.Visible = False
            ddlopr.Visible = False
            ddlstype.SelectedIndex = ddlstype.Items.IndexOf(ddlstype.Items.FindByText(row.Cells(1).Text))
            ddldtype.SelectedIndex = ddldtype.Items.IndexOf(ddldtype.Items.FindByText(row.Cells(2).Text))
            da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ddldtype.SelectedItem.Text & "' AND EID= " & Session("EID") & ""
            Dim ds As New DataSet
            da.Fill(ds, "data")

            ddlfn.Items.Clear()
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlfn.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlfn.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            ddlfn.SelectedIndex = ddlfn.Items.IndexOf(ddlfn.Items.FindByText(row.Cells(3).Text))
            ddlparavalue.SelectedIndex = ddlparavalue.Items.IndexOf(ddlparavalue.Items.FindByText(row.Cells(7).Text))
        ElseIf Stype = "Existence" Then
            ddlprc.Visible = False
            lblctype.Visible = False
            lblptype.Visible = False
            ddlopr.Visible = False
            ddlstype.SelectedIndex = ddlstype.Items.IndexOf(ddlstype.Items.FindByText(row.Cells(1).Text))
            ddldtype.SelectedIndex = ddldtype.Items.IndexOf(ddldtype.Items.FindByText(row.Cells(2).Text))
            da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ddldtype.SelectedItem.Text & "' AND EID= " & Session("EID") & ""
            Dim ds As New DataSet
            da.Fill(ds, "data")

            ddlfn.Items.Clear()
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlfn.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlfn.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            ddlfn.SelectedIndex = ddlfn.Items.IndexOf(ddlfn.Items.FindByText(row.Cells(3).Text))
            ddlparavalue.SelectedIndex = ddlparavalue.Items.IndexOf(ddlparavalue.Items.FindByText(row.Cells(7).Text))
        ElseIf Stype = "Processing" Or Stype = "Where" Then
            ddlprc.Visible = True
            lblctype.Visible = True
            lblptype.Visible = True
            ddlopr.Visible = True
            ddlstype.SelectedIndex = ddlstype.Items.IndexOf(ddlstype.Items.FindByText(row.Cells(1).Text))
            ddldtype.SelectedIndex = ddldtype.Items.IndexOf(ddldtype.Items.FindByText(row.Cells(2).Text))
            da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ddldtype.SelectedItem.Text & "' AND EID= " & Session("EID") & ""
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlfn.Items.Clear()
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlfn.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlfn.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            ddlfn.SelectedIndex = ddlfn.Items.IndexOf(ddlfn.Items.FindByText(row.Cells(3).Text))
            ddlparavalue.SelectedIndex = ddlparavalue.Items.IndexOf(ddlparavalue.Items.FindByText(row.Cells(7).Text))
            ddlprc.SelectedIndex = ddlprc.Items.IndexOf(ddlprc.Items.FindByText(row.Cells(8).Text))
            ddlopr.SelectedIndex = ddlopr.Items.IndexOf(ddlopr.Items.FindByText(row.Cells(9).Text))
        End If
        btnsavefield.Text = "Update"
        ViewState("TIID") = tid
        con.Dispose()
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub ddlstype_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlstype.SelectedIndexChanged
        If ddlstype.SelectedItem.Text = "Authentication" Then
            ddlprc.Visible = False
            lblctype.Visible = False
            lblptype.Visible = False
            ddlopr.Visible = False
        ElseIf ddlstype.SelectedItem.Text = "Existence" Then
            ddlprc.Visible = False
            lblctype.Visible = False
            lblptype.Visible = False
            ddlopr.Visible = False
        ElseIf ddlstype.SelectedItem.Text = "Processing" Or ddlstype.SelectedItem.Text = "Where" Then
            ddlprc.Visible = True
            lblctype.Visible = True
            lblptype.Visible = True
            ddlopr.Visible = True
        End If
    End Sub
    Protected Sub ddldtype_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddldtype.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ddldtype.SelectedItem.Text & "' AND EID= " & Session("EID") & ""
        Dim ds As New DataSet
        da.Fill(ds, "data")
        da.SelectCommand.CommandText = "select 'User Name'[Dispname],'UserName'[Fmapping] union select 'Email ID'[Dispname],'EmailID'[Fmapping]union select 'User Role'[Dispname],'UserRole'[Fmapping] union select 'User ID'[Dispname],'UserID'[Fmapping]"
        da.Fill(ds, "USER")
        ddlfn.Items.Clear()
        If ddldtype.SelectedItem.Text.ToUpper = "USER" Then
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlfn.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlfn.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            For j As Integer = ds.Tables("data").Rows.Count To ds.Tables("data").Rows.Count + ds.Tables("USER").Rows.Count - 1
                ddlfn.Items.Add(ds.Tables("USER").Rows(j - ds.Tables("data").Rows.Count).Item(0))
                ddlfn.Items(j).Value = ds.Tables("USER").Rows(j - ds.Tables("data").Rows.Count).Item(1)
            Next
        Else
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlfn.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlfn.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
        End If
        con.Dispose()
    End Sub
    Protected Sub btnsavefield_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsavefield.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspsmssettinginsert", con)
        'oda.SelectCommand.CommandType = CommandType.StoredProcedure
        Dim str As String = ""
        Dim abc As String = ""
        oda.SelectCommand.CommandText = "select formtype from mmm_mst_forms where eid=" & Session("eid") & " and formid=" & ddldtype.SelectedValue & ""
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        abc = oda.SelectCommand.ExecuteScalar()
        If abc.ToUpper() = "MASTER" And ddldtype.SelectedItem.Text.ToUpper = "USER" Then
            str = "MMM_MST_USER"
        ElseIf abc.ToUpper() = "MASTER" Then
            str = "MMM_MST_MASTER"
        Else
            str = "MMM_MST_DOC"
        End If
        If btnsavefield.Text = "Save" Then
            If ddlstype.SelectedItem.Text = "Authentication" Then
                If ddldtype.SelectedItem.Text = "SELECT" Then
                    lblerr.Text = "Please select Document type"
                    Exit Sub
                End If
                oda.SelectCommand.CommandText = "uspsmssettinginsert"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@kw", ViewState("kw"))
                oda.SelectCommand.Parameters.AddWithValue("@settype", ddlstype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@doctype", ddldtype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@fldname", ddlfn.SelectedValue)
                oda.SelectCommand.Parameters.AddWithValue("@table", str)
                If ddlparavalue.SelectedItem.Text.ToUpper = "SELECT" Then
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", txtRole.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Static")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", ddlparavalue.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Dynamic")
                End If
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("eid").ToString())
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                lblerr.Text = "Fields Added Successfully"
                btnEdit_ModalPopupExtender.Show()
                BinddataGrid()
                con.Close()
                oda.Dispose()
                con.Dispose()
            ElseIf ddlstype.SelectedItem.Text = "Existence" Then
                If ddldtype.SelectedItem.Text = "SELECT" Then
                    lblerr.Text = "Please select Document type"
                    Exit Sub
                End If
                oda.SelectCommand.CommandText = "uspsmssettinginsert"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@kw", ViewState("kw"))
                oda.SelectCommand.Parameters.AddWithValue("@settype", ddlstype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@doctype", ddldtype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@fldname", ddlfn.SelectedValue)
                oda.SelectCommand.Parameters.AddWithValue("@table", str)
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                If ddlparavalue.SelectedItem.Text.ToUpper = "SELECT" Then
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", txtRole.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Static")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", ddlparavalue.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Dynamic")
                End If
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                lblerr.Text = "Fields Added Successfully"
                btnEdit_ModalPopupExtender.Show()
                con.Close()
                oda.Dispose()
                con.Dispose()
            ElseIf ddlstype.SelectedItem.Text = "Processing" Or ddlstype.SelectedItem.Text = "Where" Then
                oda.SelectCommand.CommandText = "uspsmssettinginsert"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@kw", ViewState("kw"))
                oda.SelectCommand.Parameters.AddWithValue("@settype", ddlstype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@doctype", ddldtype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@fldname", ddlfn.SelectedValue)
                oda.SelectCommand.Parameters.AddWithValue("@table", str)
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                oda.SelectCommand.Parameters.AddWithValue("@ctype", ddlopr.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@PType", ddlprc.SelectedItem.Text)
                If ddlparavalue.SelectedItem.Text.ToUpper = "SELECT" Then
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", txtRole.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Static")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", ddlparavalue.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Dynamic")
                End If
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                lblerr.Text = "Field Added Successfully"
                btnEdit_ModalPopupExtender.Show()
                con.Close()
                oda.Dispose()
                con.Dispose()
            End If
        ElseIf btnsavefield.Text = "Update" Then
            If ddlstype.SelectedItem.Text = "Authentication" Then
                If ddldtype.SelectedItem.Text = "SELECT" Then
                    lblerr.Text = "Please select Document type"
                    Exit Sub
                End If
                oda.SelectCommand.CommandText = "uspsmssettingUpdate"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@kw", ViewState("kw"))
                oda.SelectCommand.Parameters.AddWithValue("@settype", ddlstype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@doctype", ddldtype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@fldname", ddlfn.SelectedValue)
                oda.SelectCommand.Parameters.AddWithValue("@table", str)
                If ddlparavalue.SelectedItem.Text.ToUpper = "SELECT" Then
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", txtRole.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Static")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", ddlparavalue.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Dynamic")
                End If
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("eid").ToString())
                oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                lblerr.Text = "Fields Updated Successfully"
                btnEdit_ModalPopupExtender.Show()
                BinddataGrid()
                con.Close()
                oda.Dispose()
                con.Dispose()
            ElseIf ddlstype.SelectedItem.Text = "Existence" Then
                If ddldtype.SelectedItem.Text = "SELECT" Then
                    lblerr.Text = "Please select Document type"
                    Exit Sub
                End If
                oda.SelectCommand.CommandText = "uspsmssettingUpdate"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@kw", ViewState("kw"))
                oda.SelectCommand.Parameters.AddWithValue("@settype", ddlstype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@doctype", ddldtype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@fldname", ddlfn.SelectedValue)
                oda.SelectCommand.Parameters.AddWithValue("@table", str)
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                If ddlparavalue.SelectedItem.Text.ToUpper = "SELECT" Then
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", txtRole.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Static")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", ddlparavalue.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Dynamic")
                End If
                oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                lblerr.Text = "Fields Updated Successfully"
                btnEdit_ModalPopupExtender.Show()
                con.Close()
                oda.Dispose()
                con.Dispose()
            ElseIf ddlstype.SelectedItem.Text = "Processing" Or ddlstype.SelectedItem.Text = "Where" Then
                oda.SelectCommand.CommandText = "uspsmssettingUpdate"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@kw", ViewState("kw"))
                oda.SelectCommand.Parameters.AddWithValue("@settype", ddlstype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@doctype", ddldtype.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@fldname", ddlfn.SelectedValue)
                oda.SelectCommand.Parameters.AddWithValue("@table", str)
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                oda.SelectCommand.Parameters.AddWithValue("@ctype", ddlopr.SelectedItem.Text)
                oda.SelectCommand.Parameters.AddWithValue("@PType", ddlprc.SelectedItem.Text)
                If ddlparavalue.SelectedItem.Text.ToUpper = "SELECT" Then
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", txtRole.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Static")
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@paraval", ddlparavalue.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@paratype", "Dynamic")
                End If
                oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                lblerr.Text = "Field Updated Successfully"
                btnEdit_ModalPopupExtender.Show()
                con.Close()
                oda.Dispose()
                con.Dispose()
            End If
        End If
        BindchildGrid()
    End Sub
    Protected Sub Del(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvField.DataKeys(row.RowIndex).Value)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "delete mmm_sms_settings where tid=" & tid & " and eid=" & Session("eid") & "  "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        'oda.SelectCommand.CommandType = CommandType.Text
        'oda.SelectCommand.CommandType = CommandType.Text
        BindchildGrid()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub

    Protected Sub ddlKtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlKtype.SelectedIndexChanged
        If ddlKtype.SelectedItem.Text.ToUpper = "SELECT TYPE" Then
            pnltst.Visible = False
            ddltst.Visible = False
            pnldmobno.Visible = False
            ddldmobno.Visible = False
            pnlddays.Visible = False
            txtddays.Visible = False
        ElseIf ddlKtype.SelectedItem.Text.ToUpper = "STATIC" Then
            pnltst.Visible = True
            ddltst.Visible = True
            pnldmobno.Visible = True
            ddldmobno.Visible = True
            pnlddays.Visible = True
            txtddays.Visible = True
        Else
            pnltst.Visible = False
            ddltst.Visible = False
            pnldmobno.Visible = False
            ddldmobno.Visible = False
            pnlddays.Visible = False
            txtddays.Visible = False
        End If
    End Sub
End Class
