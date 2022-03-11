Imports System.Data
Imports System.Data.SqlClient
Partial Class SMSkeywordsetting
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As New SqlConnection(conStr)
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BinddataGrid()
            Dim da As New SqlDataAdapter("select DISTINCT FORMNAME,formid FROM MMM_MST_FORMS WHERE FORMSOURCE='MENU DRIVEN' AND ( EID=" & Session("EID") & " OR EID=0)", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
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
        Dim da As New SqlDataAdapter("SELECT tid,tname,ttype,ttext FROM mmm_mst_smstemplate", con)
        da.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        da.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
        Else
            gvData.Controls.Clear()
        End If
        updPnlGrid.Update()
        da.Dispose()
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        ViewState("tid") = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnsave.Text = "Update"
        txttname.Text = row.Cells(1).Text
        ddlttype.SelectedIndex = ddlttype.Items.IndexOf(ddlttype.Items.FindByText(row.Cells(2).Text))
        txttText.Text = row.Cells(3).Text
        Me.UpdatePanel1.Update()
        Me.btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        btnsave.Text = "Save"
        txttname.Text = ""
        txttText.Text = ""
        ddlttype.SelectedIndex = -1
        UpdatePanel1.Update()
        Me.btnForm_ModalPopupExtender.Show()
    End Sub
    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsave.Click
        
        If Trim(txttname.Text) = "" Or Trim(txttname.Text).Length < 2 Then
            lblmsg.Text = "Please Enter Template Name "
            Exit Sub
        End If
        If Trim(txttText.Text) = "" Or Trim(txttText.Text).Length < 2 Then
            lblmsg.Text = "Please Enter Template Text"
            Exit Sub
        End If
        'lblMsgupdate.Text = ""
        If btnsave.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspinsertsmstemplate", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@ttype", ddlttype.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("@tname", Trim(txttname.Text))
            oda.SelectCommand.Parameters.AddWithValue("@ttext", Trim(txttText.Text))

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Me.btnForm_ModalPopupExtender.Hide()
            lblerr.Text = oda.SelectCommand.ExecuteScalar
            BinddataGrid()
            txttname.Text = ""
            txttText.Text = ""
            con.Close()
            oda.Dispose()
            con.Dispose()

        ElseIf btnsave.Text = "Update" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspupdatesmstemplate", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@ttype", ddlttype.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("@tname", Trim(txttname.Text))
            oda.SelectCommand.Parameters.AddWithValue("@ttext", Trim(txttText.Text))
            oda.SelectCommand.Parameters.AddWithValue("@tid", ViewState("tid"))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            lblerr.Text = oda.SelectCommand.ExecuteScalar()
            btnForm_ModalPopupExtender.Hide()
            BinddataGrid()
            con.Close()
            oda.Dispose()
            con.Dispose()
        End If
        'getSearchResult()
    End Sub
    Protected Sub PreviewHit(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "delete MMM_mst_smstemplate where tid=" & tid & "   "
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
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        lblmsg.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("kw") = row.Cells(1).Text
    End Sub
    Protected Sub Del(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "delete mmm_mst_smstemplate where tid=" & tid & " "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        'oda.SelectCommand.CommandType = CommandType.Text
        'oda.SelectCommand.CommandType = CommandType.Text
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub
End Class
