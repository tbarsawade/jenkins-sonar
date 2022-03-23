
Imports System.Data

Partial Class FtpFileTransferSetting
    Inherits System.Web.UI.Page
    Dim objDC As New DataClass()
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

    Private Sub FtpFileTransferSetting_Load(sender As Object, e As EventArgs) Handles Me.Load
        'ddlField.Items.Add()

    End Sub

    Protected Sub BindUserName()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select uid,username from mmm_mst_user where eid=" & Session("EID") & " order by username ")
        If objDT.Rows.Count > 0 Then
            ddlUserList.DataSource = objDT
            ddlUserList.DataTextField = "UserName"
            ddlUserList.DataValueField = "UID"
            ddlUserList.DataBind()
            ddlUserList.Items.Insert(0, "Select")
        Else
            ddlUserList.Items.Clear()
            ddlUserList.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub BindDocumentType()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select distinct formname from mmm_mst_forms where formtype='DOCUMENT' and formsource='MENU DRIVEN' and isactive=1 and eid=" & Session("EID") & " order by formname ")
        If objDT.Rows.Count > 0 Then
            ddlDocumenttype.DataSource = objDT
            ddlDocumenttype.DataTextField = "formname"
            ddlDocumenttype.DataValueField = "formname"
            ddlDocumenttype.DataBind()
            ddlDocumenttype.Items.Insert(0, "Select")
        Else
            ddlDocumenttype.Items.Clear()
            ddlDocumenttype.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            popAlert.Visible = False
            hdnFtpID.Value = 0
            BindUserName()
            BindDocumentType()
            If ddlFupMapping.Items.Count() >= 0 Then
                ddlFupMapping.Items.Clear()
                ddlFupMapping.Items.Insert(0, "Select")
            End If
            If ddlLocation.Items.Count() >= 0 Then
                ddlLocation.Items.Clear()
                ddlLocation.Items.Insert(0, "Select")
            End If
            If ddlLocationMaster.Items.Count() >= 0 Then
                ddlLocationMaster.Items.Clear()
                ddlLocationMaster.Items.Insert(0, "Select")
            End If

        Catch ex As Exception
            Throw
        End Try

        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        popAlert.Visible = False
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        ' No Value in Session just fill the Edit Form and Show two button
        btnSubmit.Text = "Update"
        ViewState("pid") = pid
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("select * from mmm_mst_ftpFileTransfer where ftpid=" & pid)
        txtFtpID.Text = pid
        hdnFtpID.Value = pid
        BindUserName()
        BindDocumentType()
        If objDT.Rows.Count > 0 Then
            ddlUserList.SelectedValue = objDT.Rows(0)("UID")
            ddlDocumenttype.SelectedValue = objDT.Rows(0)("DocType")
            ddlDocumenttype_SelectedIndexChanged(Me, EventArgs.Empty)
            ddlFupMapping.SelectedValue = objDT.Rows(0)("FUP_FieldMapping")
            For Each Item As ListItem In ddlLocation.Items
                If Item.Value.ToString().Contains(objDT.Rows(0)("LOC_FieldMapping") & "-") Then
                    Item.Selected = True
                Else
                    Item.Selected = False
                End If
            Next
            ddlLocation_SelectedIndexChanged(Me, EventArgs.Empty)
            ddlLocationMaster.SelectedValue = objDT.Rows(0)("LOCID")
            ddlBarCode.SelectedValue = objDT.Rows(0)("BARCODE")
            ddlReadMode.SelectedValue = objDT.Rows(0)("READMODE").ToString().Trim().ToUpper
            ddlReadMode_SelectedIndexChanged(Me, New EventArgs())
            If ddlReadMode.SelectedIndex = 2 Then
                txtHostName.Text = Convert.ToString(objDT.Rows(0)("hostName"))
                txtFTPUserName.Text = Convert.ToString(objDT.Rows(0)("username"))
                txtFTPPassword.Text = Convert.ToString(objDT.Rows(0)("password"))
                txtPort.Text = Convert.ToString(objDT.Rows(0)("port"))
                txtPrefix.Text = Convert.ToString(objDT.Rows(0)("postfix"))
            Else
                txtHostName.Text = ""
                txtFTPUserName.Text = ""
                txtFTPPassword.Text = ""
                txtPort.Text = ""
                txtPrefix.Text = ""
            End If

            isDublicityChecked.Checked = objDT.Rows(0)("isDuplicityChecked")
        End If

        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()

    End Sub


    Protected Sub ddlDocumenttype_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlFupMapping.Items.Count() > 0 Then
            ddlFupMapping.Items.Clear()
            ddlFupMapping.Items.Insert(0, "Select")
        End If
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("Select displayname, fieldMapping from mmm_mst_fields where fieldType='File Uploader' and isactive=1 and eid=" & Session("EID") & " and DocumentType='" & ddlDocumenttype.SelectedItem.Text.Trim() & "'")
            If objDT.Rows.Count > 0 Then
            ddlFupMapping.DataSource = objDT
            ddlFupMapping.DataTextField = "displayname"
            ddlFupMapping.DataValueField = "fieldMapping"
            ddlFupMapping.DataBind()
            ddlFupMapping.Items.Insert(0, "Select")
        Else
            ddlFupMapping.Items.Clear()
            ddlFupMapping.Items.Insert(0, "Select")
        End If

        If ddlLocation.Items.Count() > 0 Then
            ddlLocation.Items.Clear()
            ddlLocation.Items.Insert(0, "Select")
        End If
        Dim objDTLocation As New DataTable
        '' prev
        'objDTLocation = objDC.ExecuteQryDT("Select  displayname, fieldMapping+'-'+dropdown as fieldMapping from mmm_mst_fields where documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and eid=" & Session("EID") & " and ('-'+dropdown+'-' like '%-Location-%' or '-'+dropdown+'-' like '%-Location Master-%')")
        'changed 
        objDTLocation = objDC.ExecuteQryDT("Select  displayname, fieldMapping+'-'+dropdown as fieldMapping from mmm_mst_fields where documenttype='" & ddlDocumenttype.SelectedItem.Text.Trim() & "' and eid=" & Session("EID") & " and fieldtype='Drop Down' and dropdown like 'Master-%' and DropDownType = 'MASTER VALUED'")
        If objDTLocation.Rows.Count > 0 Then
            ddlLocation.DataSource = objDTLocation
            ddlLocation.DataTextField = "displayname"
            ddlLocation.DataValueField = "fieldMapping"
            ddlLocation.DataBind()
            ddlLocation.Items.Insert(0, "Select")
        Else
            ddlLocation.Items.Clear()
            ddlLocation.Items.Insert(0, "Select")
        End If
        Dim objDTBar As New DataTable()
        objDTBar = objDC.ExecuteQryDT("select displayname,fieldMapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlDocumenttype.SelectedItem.Text & "'  order by displayName")
        If objDTBar.Rows.Count > 0 Then
            ddlBarCode.DataSource = objDTBar
            ddlBarCode.DataTextField = "displayname"
            ddlBarCode.DataValueField = "fieldMapping"
            ddlBarCode.DataBind()
            ddlBarCode.Items.Insert(0, "Select")
        End If
    End Sub
    Protected Sub ddlLocation_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlLocation.SelectedIndex <> 0 Then
            Dim textMaster As String() = ddlLocation.SelectedValue.Split("-")
            Dim objDT As New DataTable()
            objDT = objDC.ExecuteQryDT("select " & textMaster(3) & " as text,tid from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & textMaster(2) & "' and isauth=1 ")
            If objDT.Rows.Count() > 0 Then
                ddlLocationMaster.DataSource = objDT
                ddlLocationMaster.DataTextField = "text"
                ddlLocationMaster.DataValueField = "tid"
                ddlLocationMaster.DataBind()
                ddlLocationMaster.Items.Insert(0, "Select")
            End If
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If ddlDocumenttype.SelectedIndex = 0 Then
            lblMsgEdit.Text = "Please enter document type"
            popAlert.Visible = True
            btnEdit_ModalPopupExtender.Show()
        End If
        Try
            Dim objDC As New DataClass()
            Dim ht As New Hashtable
            ht.Add("@ftpID", hdnFtpID.Value)
            ht.Add("@EID", Session("EID"))
            ht.Add("@UID", ddlUserList.SelectedValue)
            ht.Add("@GID", "0")
            ht.Add("@Documenttype", ddlDocumenttype.SelectedItem.Text.Trim())
            ht.Add("@FUP_FieldMapping", ddlFupMapping.SelectedValue.Trim())
            ht.Add("@Loc_FieldMapping", ddlLocation.SelectedValue.Split("-")(0))
            ht.Add("@LocID", ddlLocationMaster.SelectedValue)
            ht.Add("@ReadMode", ddlReadMode.SelectedValue)
            ht.Add("@HostName", txtHostName.Text)
            ht.Add("@UserName", txtFTPUserName.Text)
            ht.Add("@Password", txtFTPPassword.Text)
            ht.Add("@Port", txtPort.Text)
            ht.Add("@Prefix", txtPrefix.Text)
            ht.Add("@BarCode", ddlBarCode.SelectedValue)
            ht.Add("@CreatedBy", Session("UID"))
            ht.Add("@isDuplicityChecked", IIf(isDublicityChecked.Checked, True, False))
            objDC.ExecuteProDT("AddUpdateFtpFileTransfer", ht)
            Me.btnEdit_ModalPopupExtender.Hide()
            Response.Redirect(Request.Url.AbsoluteUri)
        Catch ex As Exception

        End Try

    End Sub
    Protected Sub ddlReadMode_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlReadMode.SelectedIndex = 1 Then
            FTPMode.Visible = False
            FTPMode2.Visible = False
            FTPMode3.Visible = False
        Else
            FTPMode.Visible = True
            FTPMode2.Visible = True
            FTPMode3.Visible = True
        End If
    End Sub
End Class
