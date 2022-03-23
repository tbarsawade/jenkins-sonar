Imports System.Data
Imports System.Data.SqlClient

Partial Class AssignUser
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            binddata()
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
    Private Sub binddata()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Users  

        lblMsgS.Text = ""

        'Dim Orderdate As Date = txtFDate.Text

        Dim sqlQRY As String = ""
        Dim da As New SqlDataAdapter("SELECT grpName FROM MMM_MST_GROUP where EID=" & Session("EID").ToString(), con)
        Dim ds As New DataSet
        da.Fill(ds, "grp")

        For i As Integer = 0 To ds.Tables("grp").Rows.Count - 1
            sqlQRY &= " ' ' as [" & ds.Tables("grp").Rows(i).Item(0).ToString() & "],"
        Next

        sqlQRY = "Select UID,UserName as [User Name], " & Left(sqlQRY, Len(sqlQRY) - 1) & " FROM MMM_MST_USER WHERE USERROLE='USR' AND EID=" & Session("EID").ToString()
        da.SelectCommand.CommandText = sqlQRY
        da.Fill(ds, "data")
        If ds.Tables.Count > 0 Then
            gvData.Columns.Clear()
            For i As Integer = 1 To ds.Tables("data").Columns.Count - 1
                Dim abc As New BoundField
                abc.DataField = ds.Tables("data").Columns(i).ColumnName()
                abc.HeaderText = ds.Tables("data").Columns(i).ColumnName()
                gvData.Columns.Add(abc)
            Next
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            updGrid.Update()

        Else
            lblMsg.Text = "No Record Found"
            updGrid.Update()
        End If
        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub gvData_RowCreated(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowCreated
        Dim row As GridViewRow = e.Row
        If row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        For i As Integer = 1 To row.Cells.Count - 1
            Dim cb As New CheckBox
            cb.ID = "chk" & i.ToString() & row.RowIndex
            '            ln.Width = 50
            row.Cells(i).Controls.Add(cb)
        Next
    End Sub

    Protected Sub gvData_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowDataBound
        Dim row As GridViewRow = e.Row
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT UID,grpID,grpname FROM MMM_REF_GRP_USR LEFT OUTER JOIN MMM_MST_GROUP ON  MMM_MST_GROUP.gid = MMM_REF_GRP_USR.grpId  where MMM_REF_GRP_USR.EID=" & Session("EID").ToString() & " Order By UID", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        If row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        For i As Integer = 1 To row.Cells.Count - 1
            Dim cb As New CheckBox
            cb.ID = "chk" & i.ToString() & row.RowIndex
            '            ln.Width = 50
            Dim UID As String = row.DataItem(0).ToString()
            Dim grpName As String = gvData.HeaderRow.Cells(i).Text
            Dim rows As DataRow() = ds.Tables("data").Select("UID = " & UID & " and grpname='" & grpName & "'")

            If rows.Count = 1 Then
                cb.Checked = True
            End If
            row.Cells(i).Controls.Add(cb)
        Next
    End Sub

    'Protected Sub usefullLinks(ByVal sender As Object, ByVal e As System.EventArgs)

    '    'Me.link_ModelPopControl.Show()


    'End Sub


    'Protected Sub UpdateRecord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click

    'End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim EID As Integer = Val(Session("EID").ToString())
        Dim da As New SqlDataAdapter("DELETE MMM_REF_GRP_USR WHERE EID=" & EID, con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteNonQuery()
        Dim row As GridViewRow
        'Now insert in Order table first and get Order ID 
        For Each row In gvData.Rows
            Dim UID As Integer = Val(Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value))
            'check if any order is need to be placed
            For i As Integer = 1 To gvData.Columns.Count - 1
                Dim grpName As String = gvData.Columns(i).HeaderText
                Dim txtname As String = "chk" & i.ToString() & row.RowIndex
                '               Dim txtvalue As Integer = Val(CType(row.FindControl(txtname), CheckBox).Checked.ToString())
                If CType(row.FindControl(txtname), CheckBox).Checked = True Then
                    da.SelectCommand.CommandText = "uspInsertMapping"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.Parameters.AddWithValue("grpName", grpName)
                    da.SelectCommand.Parameters.AddWithValue("uid", UID)
                    da.SelectCommand.Parameters.AddWithValue("eid", EID)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.SelectCommand.ExecuteNonQuery()
                End If
            Next
        Next
        con.Close()
        da.Dispose()
        con.Dispose()
        lblMsgS.Text = "Users are Assigned Groups"
    End Sub

    Private Sub resetAll()
        binddata()
    End Sub
End Class
