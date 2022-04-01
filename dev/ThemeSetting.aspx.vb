Imports System.Data.SqlClient
Imports System.Data
Imports System.IO


Partial Class ThemeSetting
    Inherits System.Web.UI.Page

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
            BindTheme()
        End If
    End Sub

    Protected Sub BindTheme()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            If (con.State = ConnectionState.Closed) Then
                con.Open()
            End If
            Dim oda As SqlDataAdapter = New SqlDataAdapter("select CTheme  from MMM_MST_ENTITY  where EID=" + Session("EID") + "", con)
            Dim ds As New DataTable()
            oda.Fill(ds)
            If (con.State = ConnectionState.Open) Then
                con.Dispose()
            End If
            Dim ThemeDt As New DataTable()
            ThemeDt.Columns.Add("ThemeName")
            ThemeDt.Columns.Add("IsCT")
            Dim dr As DataRow = ThemeDt.NewRow()
            If (ds.Rows.Count > 0) Then
                Dim paths As String = Server.MapPath("~/App_Themes")
                Dim fileArray() As String = Directory.GetDirectories(paths)
                For i As Integer = 0 To fileArray.Length - 1
                    dr("ThemeName") = Path.GetFileName(Convert.ToString(fileArray(i)))
                    If (dr("ThemeName").ToString = ds.Rows(0)(0).ToString) Then
                        dr("IsCT") = "Yes"
                    Else
                        dr("IsCT") = "No"
                    End If
                    ThemeDt.Rows.Add(dr)
                    dr = ThemeDt.NewRow()
                Next
                grdTheme.DataSource = ThemeDt
                grdTheme.DataBind()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub imgApply_click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
        Try
            Me.btnEdit_ModalPopupExtender.Hide()
            Dim btnApply As ImageButton = CType(sender, ImageButton)
            Dim gvr As GridViewRow = CType(btnApply.NamingContainer, GridViewRow)
            Dim lblThemeName As Label = CType(gvr.FindControl("lblThemeName"), Label)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            If (con.State = ConnectionState.Closed) Then
                con.Open()
            End If
            Dim oda As SqlDataAdapter = New SqlDataAdapter("Update MMM_MST_ENTITY set CTheme='" + lblThemeName.Text.Trim() + "'where EID=" + Session("EID") + "", con)
            Dim ds As New DataTable()
            oda.Fill(ds)
            If (con.State = ConnectionState.Open) Then
                con.Dispose()
            End If
            BindTheme()

        Catch ex As Exception

        End Try
    End Sub



    Protected Sub imgPreview_click(ByVal sender As Object, ByVal e As ImageClickEventArgs)


        imgLogin.Src = ""
        lblLogin.Text = ""
        imgControl.Src = ""
        lblControl.Text = ""
        imgHome.Src = ""
        lblHome.Text = ""
        Dim btnPreview As ImageButton = CType(sender, ImageButton)
        Dim gvr As GridViewRow = CType(btnPreview.NamingContainer, GridViewRow)
        Dim lblThemeName As Label = CType(gvr.FindControl("lblThemeName"), Label)
        lblHeaderPopUp.Text = lblThemeName.Text + " Preview Images"
        Dim paths As String = Server.MapPath("~/Images/" + lblThemeName.Text.Trim())
        If (Directory.Exists(paths)) Then
            Dim fileArray() As String = Directory.GetFiles(paths)
            For i As Integer = 0 To fileArray.Length - 1
                Dim FileName As String = Path.GetFileName(Convert.ToString(fileArray(i)))
                If (imgLogin.Src = "") Then
                    imgLogin.Src = "~/Images/" + lblThemeName.Text.Trim() + "/" + FileName
                    imgLogin.Attributes.Add("data-zoom-image", "Images/" + lblThemeName.Text.Trim() + "/" + FileName)

                    lblLogin.Text = FileName
                ElseIf (imgHome.Src = "") Then
                    imgHome.Src = "~/Images/" + lblThemeName.Text.Trim() + "/" + FileName
                    lblHome.Text = FileName
                ElseIf (imgControl.Src = "") Then
                    imgControl.Src = "~/Images/" + lblThemeName.Text.Trim() + "/" + FileName
                    lblControl.Text = FileName
                End If
            Next
            'If (fileArray.Length > 0) Then

            'End If
        End If
        updtheme.Update()
        updateimage.Update()
        updPanalHeader.Update()
        Me.btnEdit_ModalPopupExtender.Show()
        'Dim message As String = "Message from server side"
        'ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "ShowPopup('" + message + "');", True)

    End Sub
    'Protected Sub btnCloseEdit_click(ByVal sender As Object, ByVal e As ImageClickEventArgs)

    '    pnlPopupEdit.Attributes.Add("display", "none")
    '    btnEdit_ModalPopupExtender.Hide()
    'End Sub

End Class
