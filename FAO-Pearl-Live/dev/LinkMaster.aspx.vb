Imports System.Data
Imports System.Data.SqlClient



Partial Class FieldMaster
    Inherits System.Web.UI.Page

    Public Sub BinddataGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("SELECT * FROM MMM_MST_SU_LINKS where eid=" & Session("EID") & " ", con)
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

    Protected Sub gvData_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvData.PageIndexChanging
        Dim currenpage As Short = 1
        Try
            gvData.PageIndex = e.NewPageIndex
            currenpage = e.NewPageIndex + 1
            BinddataGrid()
            updPnlGrid.Update()
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
        txtFName.Text = row.Cells(0).Text
        txtUrl.Text = row.Cells(1).Text
        txtDisplayOrder.Text = row.Cells(2).Text
        txtScriptToRun.Text = (row.Cells(3).Text).ToString()
        fuHomepageDoc.Visible = False
        ddlSso.Visible = False
        ddlTargettype.SelectedIndex = 0
        ddlSso.SelectedIndex = 0
        
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        txtFName.Text = ""
        txtDisplayOrder.Text = ""
        txtScriptToRun.Text = ""
        txtUrl.Text = ""
        txtUrl.Visible = True
        fuHomepageDoc.Visible = False
        ddlSso.Visible = False
        ddlTargettype.SelectedIndex = 0
        ddlSso.SelectedIndex = 0
        ddlLinkType.SelectedIndex = 0
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Field? " & row.Cells(0).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        'validation for null entry
        If txtFName.Text.Length < 2 Then
            lblMsgEdit.Text = "Please Enter Valid Field Name"
            Exit Sub
        End If
        If ddlTargettype.SelectedItem.Text = "Single Sign On" Then
            If ddlSso.SelectedItem.Text = "PLease Select" Then
                lblMsgEdit.Text = "Please Enter Single Sign On"
                Exit Sub
            End If
        End If
        If ddlLinkType.SelectedItem.Text = "PLease Select" Then
            lblMsgEdit.Text = "Please Enter Link Type"
            Exit Sub
        End If
        Dim file As String = ""
        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertLinks", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Val(Session("EID").ToString()))
            oda.SelectCommand.Parameters.AddWithValue("displayname", txtFName.Text)
            oda.SelectCommand.Parameters.AddWithValue("displayorder", txtDisplayOrder.Text)
            oda.SelectCommand.Parameters.AddWithValue("Script", txtScriptToRun.Text)

            If ddlTargettype.SelectedItem.Text = "File" Then
                If fuHomepageDoc.HasFile Then
                    file = fuHomepageDoc.PostedFile.FileName
                    Dim doc As String() = Split(file, ".")
                    If UCase(doc(1)) = "DOC" Or UCase(doc(1)) = "PDF" Or UCase(doc(1)) = "XLS" Or UCase(doc(1)) = "DOCX" Or UCase(doc(1)) = "XLSX" Then
                    Else
                        lblMsg.Text = "Upload File may be incorrect formet"
                        Exit Sub
                    End If

                    Dim file1 As String
                    file1 = Server.MapPath("DOCS\" & Session("EID") & "") & "\" & file
                    file = "DOCS\" & Session("EID") & "\" & file
                    fuHomepageDoc.PostedFile.SaveAs(file1)
                    oda.SelectCommand.Parameters.AddWithValue("url", file)
                End If
            End If
            If ddlTargettype.SelectedItem.Text = "URL" Then
                oda.SelectCommand.Parameters.AddWithValue("url", txtUrl.Text)
            End If
            If ddlTargettype.SelectedItem.Text = "Single Sign On" Then
                oda.SelectCommand.Parameters.AddWithValue("url", ddlSso.SelectedItem.Text().ToString())
            End If
            oda.SelectCommand.Parameters.AddWithValue("linktype", ddlLinkType.SelectedItem.Text().ToString())


            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteScalar()
            lblRecord.Text = "Link has been created successfully"
            updatePanelEdit.Update()
            BinddataGrid()
            btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda.Dispose()
            con.Dispose()
        Else
            'Edit Record
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateLink", con)
            Dim pid As Integer = Val(ViewState("pid").ToString())
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("FieldID", pid)
            oda.SelectCommand.Parameters.AddWithValue("EID", Val(Session("EID").ToString()))
            oda.SelectCommand.Parameters.AddWithValue("displayname", txtFName.Text)
            oda.SelectCommand.Parameters.AddWithValue("displayOrder", txtDisplayOrder.Text)
            oda.SelectCommand.Parameters.AddWithValue("ScriptTorun", txtScriptToRun.Text)
            If ddlTargettype.SelectedItem.Text = "File" Then
                If fuHomepageDoc.HasFile Then
                    file = fuHomepageDoc.PostedFile.FileName
                    Dim doc As String() = Split(file, ".")
                    If UCase(doc(1)) = "DOC" Or UCase(doc(1)) = "PDF" Or UCase(doc(1)) = "XLS" Or UCase(doc(1)) = "DOCX" Or UCase(doc(1)) = "XLSX" Then
                    Else
                        lblMsg.Text = "Upload File may be incorrect formet"
                        Exit Sub
                    End If

                    Dim file1 As String
                    file1 = Server.MapPath("DOCS\" & Session("EID") & "") & "\" & file
                    file = "DOCS\" & Session("EID") & "\" & file
                    fuHomepageDoc.PostedFile.SaveAs(file1)
                    oda.SelectCommand.Parameters.AddWithValue("url", file)
                End If
            End If
            If ddlTargettype.SelectedItem.Text = "URL" Then
                oda.SelectCommand.Parameters.AddWithValue("url", txtUrl.Text)
            End If
            If ddlTargettype.SelectedItem.Text = "Single Sign On" Then
                oda.SelectCommand.Parameters.AddWithValue("url", ddlSso.SelectedItem.Text().ToString())
            End If
            oda.SelectCommand.Parameters.AddWithValue("linktype", ddlLinkType.SelectedItem.Text().ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            lblMsgEdit.Text = "Field Updated"
            lblRecord.Text = "Link has been updated successfully"
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
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteLink", con)
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
            lblRecord.Text = "Link has been deleted successfully"
            btnDelete_ModalPopupExtender.Hide()
        Else
            lblMsgDelete.Text = "This Field can Not be Deleted"
            updatePanelDelete.Update()
        End If
    End Sub

    
  
    Protected Sub ddlTargettype_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTargettype.SelectedIndexChanged
        If ddlTargettype.SelectedItem.Text = "Target Type" Then
            lblMsgEdit.Text = "PLease select Target Type"
            Exit Sub
        End If
        If ddlTargettype.SelectedItem.Text = "URL" Then
            txtUrl.Visible = True
            fuHomepageDoc.Visible = False
            ddlSso.Visible = False
        End If
        If ddlTargettype.SelectedItem.Text = "File" Then
            txtUrl.Visible = False
            fuHomepageDoc.Visible = True
            ddlSso.Visible = False
        End If
        If ddlTargettype.SelectedItem.Text = "Single Sign On" Then
            txtUrl.Visible = False
            fuHomepageDoc.Visible = False
            ddlSso.Visible = True
        End If
        updatePanelEdit.Update()
    End Sub
End Class
