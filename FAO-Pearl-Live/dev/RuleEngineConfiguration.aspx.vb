Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Globalization
Partial Class RuleEngineConfiguration
    Inherits System.Web.UI.Page




    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then


            ' bind dropdownlist to search in the grid

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As New SqlDataAdapter("", con)
            oda = New SqlDataAdapter("select disname,colname FROM MMM_MST_SEARCH where tableName='Rules'", con)
            Dim ods As New DataSet
            oda.Fill(ods, "Rules")
            For i As Integer = 0 To ods.Tables("Rules").Rows.Count - 1
                ddlField.Items.Add(ods.Tables("Rules").Rows(i).Item(0))
                ddlField.Items(i).Value = ods.Tables("Rules").Rows(i).Item(1)
            Next


             getSearchResult()
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
        Try
            lblMsg.Text = String.Empty
            updPnlGrid.Update()
            Response.Redirect("RuleEngineDesigner.aspx?RuleID=0")

        Catch ex As Exception
        End Try

    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Ruleid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Ruleid") = Ruleid
        Response.Redirect("RuleEngineDesigner.aspx?RuleID=" & Ruleid & "")


    End Sub

    Protected Sub gvData_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvData.PageIndexChanging
        gvData.PageIndex = e.NewPageIndex
        getSearchResult()
    End Sub

    Private Function getSearchResult() As DataTable
        gvData.DataBind()
        ''Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        ''Dim con As New SqlConnection(conStr)

        ''Dim da As New SqlDataAdapter("", con)
        ''Dim ds As New DataSet
        ''Try
        ''    da.SelectCommand.CommandText = "Select ruleid,eid,rulename,case when isactive=1 then 'Active' else 'In Active' end [Status] ,ruledesc,formsource,documenttype,docnature,whentorun,condition,succactiontype,succactionfield,succmsg,failactiontype,failactionfield,errormsg,controlfield,targetcontrolfield from mmm_mst_rules where eid=" & Session("EID") & ""
        ''    da.Fill(ds, "data")

        ''    If ds.Tables("data").Rows.Count > 0 Then

        ''        gvData.DataSource = ds.Tables("data")
        ''        gvData.DataBind()

        ''    End If
        ''Catch ex As Exception
        ''End Try



    End Function
    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Ruleid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Ruleid") = Ruleid
        Me.updPnlGrid.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub
    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ruleid As Integer = ViewState("Ruleid")
        Try
            da.SelectCommand.CommandText = "Delete from mmm_mst_rules where eid=" & Session("EID") & " and ruleid=" & ruleid & " "

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteScalar()
            Me.btnDelFolder_ModalPopupExtender.Hide()
            'getSearchResult()

            lblMsg.Text = "Deleted Successfully!!!!"
            getSearchResult()
            Me.updPnlGrid.Update()
        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AGAIN AFTER SOMETIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try

        
        


    End Sub
    
    'Protected Sub gvData_RowDataBound(sender As Object, e As GridViewRowEventArgs)
    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        Dim lnk As LinkButton = DirectCast(e.Row.FindControl("img_status"), LinkButton)
    '        If e.Row.Cells(6).Text = "Active" Then
    '            lnk.Text = "Active"
    '        Else
    '            lnk.Text = "InActive"
    '            e.Row.BackColor = Drawing.Color.LightYellow
    '        End If
    '        Dim a As String = ""
    '        If lnk.Text = "Active" Then
    '            a = "InActive"
    '        Else
    '            a = "Active"
    '        End If
    '        lnk.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to " & a.ToString() & " this record : " + DataBinder.Eval(e.Row.DataItem, "RUlename") + "');")
    '    End If
    '    e.Row.Cells(6).Visible = False
    'End Sub

    'Protected Sub gvData_SelectedIndexChanging(sender As Object, e As GridViewSelectEventArgs)
    '    Dim ruleid As String = gvData.DataKeys(e.NewSelectedIndex).Value.ToString()
    '    Dim status As String = gvData.Rows(e.NewSelectedIndex).Cells(6).Text
    '    Dim i As Integer = 0
    '    If status = "Active" Then
    '        status = "Inactive"
    '        i = 0
    '    Else
    '        status = "Active"
    '        i = 1
    '    End If
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim cmd As New SqlCommand("update mmm_mst_rules set isactive=" & i & " where ruleid=" + ruleid, con)
    '    con.Open()
    '    cmd.ExecuteNonQuery()
    '    con.Close()
    '    getSearchResult()
    'End Sub

    Protected Sub Active_Clicked(sender As Object, e As EventArgs)
        Dim btnDetails As LinkButton = TryCast(sender, LinkButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim hasrule As Integer = 0
       
        If DirectCast(sender, LinkButton).Text = "In Active" Then
        
            DirectCast(sender, LinkButton).Text = "Active"
        Else
            DirectCast(sender, LinkButton).Text = "In Active"
        End If
        Dim stat As Integer = 0
        If DirectCast(sender, LinkButton).Text = "Active" Then
            stat = 1
            hasrule = 1
        Else
            stat = 0
            hasrule = 0
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim cmd As New SqlCommand("update mmm_mst_rules set isactive=" & stat & " where ruleid=" & Tid & " ", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        If UCase(Trim(row.Cells(5).Text)) = "CONTROL" Then
            Dim da As New SqlDataAdapter("Select controlfield from mmm_mst_rules where ruleid=" & Tid & "", con)
            Dim fieldid As Integer = Val(da.SelectCommand.ExecuteScalar())
            da.SelectCommand.CommandText = "Update mmm_mst_Fields set hasrule=" & hasrule & " where eid=" & Session("EID") & " and fieldid=" & fieldid & " "
            da.SelectCommand.ExecuteNonQuery()
        End If
        cmd.ExecuteNonQuery()
        con.Close()
        getSearchResult()
        updPnlGrid.Update()
    End Sub
    

    Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataBind()
        'updPnlGrid.Update()
    End Sub
End Class
