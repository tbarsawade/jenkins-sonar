Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration

Partial Class AccessControl
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadWorkGroupTree()
            lblFolderName.Text = "Selected Folder : None"

            ' tv.Attributes.Add("tv_SelectedNodeChanged", "fixhead()")
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
    Protected Sub tv_SelectedNodeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tv.SelectedNodeChanged
        If tv.SelectedNode.Value.ToString() <> "MAIN" Then
            lblFolderName.Text = "Selected Folder : " & tv.SelectedNode.Text
            binddata()
        Else
            lblFolderName.Text = "Selected Folder : None"
        End If
        If Not (tv.SelectedNode.ChildNodes.Count > 0) Then

            BindChilds(tv.SelectedNode, Convert.ToInt32(tv.SelectedNode.Value))

            tv.SelectedNode.Expand()

        End If
        '        showFiles()
    End Sub
    Private Function GetData(ByVal commandText As String) As SqlDataReader
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        Dim sqlcmd As New SqlCommand(commandText, con)
        Dim dr As SqlDataReader = sqlcmd.ExecuteReader()
        Return dr
    End Function
    Private Sub BindChilds(ByVal node As TreeNode, ByVal parentNodeID As Integer)
        Dim str As String = "select tid,isnull(stid,gid) [stid],foldername,oUID [own]   from MMM_MST_DOC where eid=" & Session("eid").ToString() & " and foldername is not null and stid is null and gid=" & parentNodeID & " union  SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " and stid=" & parentNodeID & "  order by foldername"
        'Dim str As String = "select tid,isnull(stid,gid) [stid],foldername,oUID [own]   from MMM_MST_DOC where eid=" & Session("eid").ToString() & " and foldername is not null and gid=" & parentNodeID & " union  SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where stid in (Select tid from mmm_mst_doc where folderName is not null  and EID=" & Session("eid").ToString() & " ) and foldername is not null and eid=" & Session("eid").ToString() & " and gid=" & parentNodeID & "  order by foldername"
        Dim reader As SqlDataReader = GetData(str)
        While reader.Read()
            Dim childNode As New TreeNode(reader(2).ToString(), reader(0).ToString())
            node.ChildNodes.Add(childNode)
            childNode.ImageUrl = "images/folder.png"
            'BindChilds(node, parentNodeID)
        End While
        reader.Close()
    End Sub
    Private Sub LoadWorkGroupTree()
        tv.Nodes.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)



        Dim od As New SqlDataAdapter("SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " order by grpName", con)
        If Session("USERROLE").ToString() = "USR" Then
            'od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select grpid from MMM_REF_GRP_USR where uId = " & Session("UID").ToString() & ")  order by grpName"
            od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId = " & Session("UID").ToString() & ") UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null AND foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId= " & Session("UID").ToString() & ") UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId= " & Session("UID").ToString() & ") order by tid"
        End If
        'Dim od As New SqlDataAdapter("SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null AND foldername is not null and eid=" & Session("eid").ToString() & " UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " order by tid", con)

        'If Session("USERROLE").ToString() = "USR" Then
        '    od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId = " & Session("UID").ToString() & ") UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null AND foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId= " & Session("UID").ToString() & ") UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId= " & Session("UID").ToString() & ") order by tid"
        'End If

        Dim ds As New DataSet
        od.Fill(ds, "boss")

        Dim dr As New DataRelation("bossrelation", ds.Tables("boss").Columns("tid"), ds.Tables("boss").Columns("stid"))
        ds.Relations.Add(dr)
        Dim masterNode As New TreeNode(Session("USERNAME").ToString())
        masterNode.Value = "MAIN"
        masterNode.ImageUrl = "images/mainnode.png"
        masterNode.SelectAction = TreeNodeSelectAction.Expand
        tv.Nodes.Add(masterNode)
        con.Dispose()
        od.Dispose()

        For Each rw As DataRow In ds.Tables("boss").Rows
            If rw.IsNull("stid") Then
                Dim n As New TreeNode()
                If Session("UID").ToString() = rw.Item("own").ToString() Then
                    n.Text = rw.Item("foldername")
                Else
                    n.Text = rw.Item("foldername")
                End If
                n.Value = rw.Item("tid")
                n.ImageUrl = "images/project.png"

                masterNode.ChildNodes.Add(n)
                LoadRecTree(rw, n)
            End If
        Next

        ds.Dispose()
        ds = Nothing
        tv.ExpandAll()
        updTV.Update()
    End Sub

    Private Sub LoadRecTree(ByVal row As DataRow, ByRef node As TreeNode)
        For Each rw As DataRow In row.GetChildRows("bossrelation")
            Dim n As New TreeNode()
            n.Text = rw("foldername").ToString()
            n.Value = rw("tid").ToString()
            n.ImageUrl = "images/folder.png"
            node.ChildNodes.Add(n)
            LoadRecTree(rw, n)
        Next
    End Sub

    Private Function FindRootNode(treeNode As TreeNode) As String
        Dim strRet As String = "0"

        While treeNode.Parent.Value <> "MAIN"
            strRet = strRet & "," & treeNode.Value.ToString()
            treeNode = treeNode.Parent
        End While
        Return strRet

    End Function

    Private Sub binddata()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Users  
        lblMsgS.Text = ""
        Dim sqlQRY As String = ""

        Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
        Dim gid As Integer = Val(arr(1))

        Dim da As New SqlDataAdapter("select UserName,uid from MMM_MST_USER where userrole='USR' and uid in (Select uid from MMM_REF_GRP_USR where grpId=" & gid & ") and EID=" & Session("EID").ToString() & " and UID not in (select UID from MMM_MST_DENYRULE where fileid in (" & FindRootNode(tv.SelectedNode) & "))", con)
        Dim ds As New DataSet
        da.Fill(ds, "users")
        For i As Integer = 0 To ds.Tables("users").Rows.Count - 1
            sqlQRY &= " ' ' as [" & ds.Tables("users").Rows(i).Item(0).ToString() & "],"
        Next
        sqlQRY = "Select tID, '<img src=""images/' + isnull(docimage,'folder.png') + '""/> '  + ISNULL(foldername,fname) [FileName],Case oUID when " & Session("UID").ToString() & " then 'ME' else UserName end [UserName], " & Left(sqlQRY, Len(sqlQRY) - 1) & " FROM MMM_MST_DOC LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where isnull(Stid,gID) = " & tv.SelectedNode.Value.ToString() & "  AND MMM_MST_DOC.EID=" & Session("EID").ToString()
        da.SelectCommand.CommandText = sqlQRY
        da.Fill(ds, "data")
        If ds.Tables.Count > 0 Then
            gvData.Columns.Clear()
            For i As Integer = 1 To ds.Tables("data").Columns.Count - 1
                Dim abc As New BoundField
                abc.DataField = ds.Tables("data").Columns(i).ColumnName()
                abc.HeaderText = ds.Tables("data").Columns(i).ColumnName()
                abc.HtmlEncode = False
                gvData.Columns.Add(abc)
            Next
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()

            MarkAccessRights()

            If ds.Tables("data").Rows.Count > 0 Then
                btnSave.Visible = True
            Else
                btnSave.Visible = False
            End If
        Else
            btnSave.Visible = False

        End If
        updGrid.Update()
        da.Dispose()
        con.Dispose()
    End Sub

    Private Sub MarkAccessRights()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim stid As Integer = Val(tv.SelectedNode.Value.ToString())

        Dim da As New SqlDataAdapter("SELECT fileid,stid,username FROM MMM_MST_DENYRULE LEFT OUTER JOIN MMM_MST_USER on MMM_MST_DENYRULE.UID = MMM_MST_USER.UID  where MMM_MST_DENYRULE.stid=" & stid & " AND MMM_MST_DENYRULE.EID=" & Session("EID").ToString(), con)
        Dim ds As New DataSet
        da.Fill(ds, "data")

        For Each row In gvData.Rows
            Dim FileID As String = Val(Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value))
            For i As Integer = 2 To row.Cells.Count - 1
                Dim userName As String = gvData.HeaderRow.Cells(i).Text
                Dim rows As DataRow() = ds.Tables("data").Select("fileID = " & FileID & " and username='" & userName & "'")
                If rows.Count = 1 Then
                    Dim txtname As String = "chk" & i.ToString() & row.RowIndex
                    Dim cb As New CheckBox
                    cb = CType(row.FindControl(txtname), CheckBox)
                    cb.Checked = False
                End If
            Next
        Next
    End Sub

    Protected Sub gvData_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowCreated
        Dim row As GridViewRow = e.Row
        If row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        For i As Integer = 2 To row.Cells.Count - 1
            Dim cb As New CheckBox
            cb.ID = "chk" & i.ToString() & row.RowIndex
            '            ln.Width = 50
            cb.Checked = True
            row.Cells(i).Controls.Add(cb)
        Next
    End Sub

    Protected Sub gvData_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowDataBound
        Dim row As GridViewRow = e.Row
        If row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        For i As Integer = 2 To row.Cells.Count - 1
            Dim cb As New CheckBox
            cb.ID = "chk" & i.ToString() & row.RowIndex
            '            ln.Width = 50
            cb.Checked = True
            row.Cells(i).Controls.Add(cb)
        Next

        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(0).CssClass = "locked"
        End If


    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim EID As Integer = Val(Session("EID").ToString())
        Dim da As New SqlDataAdapter("DELETE MMM_MST_DENYRULE WHERE stid=" & tv.SelectedNode.Value() & " and EID=" & EID, con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteNonQuery()
        Dim row As GridViewRow
        For Each row In gvData.Rows
            Dim FileID As Integer = Val(Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value))
            'check if denyrule exist in the system
            For i As Integer = 2 To gvData.Columns.Count - 1
                Dim userName As String = gvData.Columns(i).HeaderText
                Dim txtname As String = "chk" & i.ToString() & row.RowIndex
                If CType(row.FindControl(txtname), CheckBox).Checked = False Then
                    da.SelectCommand.CommandText = "uspUpdatePermision"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.Parameters.AddWithValue("userName", userName)
                    da.SelectCommand.Parameters.AddWithValue("fileid", FileID)
                    da.SelectCommand.Parameters.AddWithValue("stid", tv.SelectedNode.Value.ToString())
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
        lblMsgS.Text = "Users are Assigned Rights"
    End Sub

    Private Sub resetAll()
        binddata()
    End Sub

    Protected Sub tv_TreeNodeExpanded(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.TreeNodeEventArgs) Handles tv.TreeNodeExpanded
        If e.Node.Parent Is Nothing Then
            Return
        End If
        Dim strNodeValue As String = e.Node.Value
        For Each node As TreeNode In e.Node.Parent.ChildNodes
            If node.Value <> strNodeValue Then
                node.Collapse()
            End If
        Next
    End Sub
End Class
