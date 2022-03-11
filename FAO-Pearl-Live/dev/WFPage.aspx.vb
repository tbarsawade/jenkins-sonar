Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net

Partial Class WFPage
    Inherits System.Web.UI.Page
    'Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    '    If Not Session("ACTION") Is Nothing Then
    '        Dim dt As DataTable
    '        dt = CType(Session("FIELDS"), DataTable)
    '        If dt.Rows.Count > 0 Then
    '            If Session("ACTION").ToString() = "APPROVAL" Then
    '                lblHeaderPopUp.Text = dt.Rows(0).Item("Formcaption").ToString()
    '                Dim ob As New DynamicForm()
    '                ob.CreateControlsOnPanel(dt, pnlFields, updatePanelApprove, btnApprove)
    '            Else
    '                lblHeaderPopUp.Text = dt.Rows(0).Item("Formcaption").ToString()
    '                Dim ob As New DynamicForm()
    '                ob.CreateControlsOnPanel(dt, pnlFieldsRej, updatePanelReject, btnReject)
    '            End If
    '        End If
    '    End If

    'End Sub
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
            Dim docid As String = CStr(Request.QueryString("docid"))
            If docid <> "" Then
                BindGridbyDocid(docid)
            End If
            LoadWorkGroupTree()
            UpdMsg.Update()
        End If
    End Sub

    Private Sub LoadWorkGroupTree()
        tv.Nodes.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim STR As String = "Select UserName,documenttype, M.userid,isnull(aprstatus,'PENDING APPROVALS') [aprstatus],COUNT(docid) [cnt],D.curstatus  from MMM_MST_DOC D LEFT OUTER JOIN MMM_DOC_DTL M on M.tid=D.lasttid LEFT OUTER JOIN MMM_MST_USER U on U.Uid = M.userid "
        Dim od As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        If Session("USERROLE").ToString() = "USR" Then
            STR &= " where isWorkFlow=1 and M.userid =" & Session("UID") & " and D.eid=" & Session("EID") & " group by UserName,M.userid,documenttype,aprstatus,curstatus "
        Else
            STR &= " where isWorkFlow=1 and M.userid<>217 and D.eid=" & Session("EID") & "  group by UserName,M.userid,documenttype,aprstatus,curstatus"
        End If
        od.SelectCommand.CommandText = STR
        od.Fill(ds, "data")
        Dim masterNode As New TreeNode("Work Flow")
        masterNode.Value = "MAIN"
        masterNode.ImageUrl = "images/mainnode.png"
        masterNode.SelectAction = TreeNodeSelectAction.Expand
        tv.Nodes.Add(masterNode)
        Dim uid As Integer = 0
        For Each rw As DataRow In ds.Tables("data").Rows
            If uid <> rw.Item("userid") Then
                uid = rw.Item("userid")
                Dim n As New TreeNode()
                n.Text = rw.Item("UserName").ToString()
                n.Value = rw.Item("Userid")
                n.ImageUrl = "images/newuser.png"
                masterNode.ChildNodes.Add(n)
                LoadDocTree(ds.Tables("data"), n, uid)
            End If
        Next
        lblFolderName.Text = "Selected STATUS : None"
        updTV.Update()
    End Sub



    Private Sub LoadDocTree(ByVal dt As DataTable, ByRef node As TreeNode, ByVal uid As Integer)
        Dim doctype As String = ""
        Dim dtrow As DataRow() = dt.Select("userid=" & uid & "")
        For Each rw As DataRow In dtrow
            Dim n As New TreeNode()
            If doctype <> rw("documenttype").ToString() Then
                doctype = rw("documenttype").ToString()
                n.Text = rw("documenttype").ToString()
                n.Value = rw("documenttype").ToString()
                n.ImageUrl = "images/folder.png"
                node.ChildNodes.Add(n)
                LoadPENDINGDocTree(dtrow, n)
            End If
        Next
    End Sub



    Private Sub LoadPENDINGDocTree(ByVal dtrow As DataRow(), ByRef node As TreeNode)
        Dim doctype As String = ""
        For Each rw As DataRow In dtrow
            Dim n As New TreeNode()
            If doctype <> rw("aprstatus").ToString() Then
                doctype = rw("aprstatus").ToString()
                n.Text = rw("aprstatus").ToString()
                n.Value = rw("aprstatus").ToString()
                n.ImageUrl = "images/master.png"
                node.ChildNodes.Add(n)
                LoadAPStatusTree(dtrow, n, doctype)
            End If
        Next
    End Sub


    Private Sub LoadAPStatusTree(ByVal dtrow As DataRow(), ByRef node As TreeNode, ByVal aprstatus As String)
        Dim status As String = ""
        For Each rw As DataRow In dtrow
            If status <> rw.Item("curstatus").ToString() And aprstatus = rw.Item("aprstatus").ToString() Then
                status = rw.Item("curstatus").ToString()
                Dim n As New TreeNode()
                n.Text = rw("curstatus").ToString() & " ( " & rw("cnt").ToString() & " )"
                n.Value = rw("curstatus").ToString()
                n.ImageUrl = "images/process.png"
                node.ChildNodes.Add(n)
            End If
        Next
    End Sub


    Private Sub LoadStatusTree(ByVal dt As DataTable, ByRef node As TreeNode, ByVal userid As Integer)
        Dim status As String = ""
        Dim dtdoc As DataTable = dt.DefaultView.ToTable(True, "userid")

        'Dim dtrow As DataRow() = dt.Select("userid=" & userid & " and aprstatus='" & dtdoc.Rows(0).Item("userid") & "'")
        Dim dtrow As DataRow() = dt.Select("userid=" & userid & "")
        For Each rw As DataRow In dtrow
            If status <> rw.Item("aprstatus").ToString() Then
                status = rw.Item("aprstatus").ToString()
                Dim n As New TreeNode()
                n.Text = rw("aprstatus").ToString() & " ( " & rw("cnt").ToString() & " )"
                n.Value = rw("aprstatus").ToString()
                n.ImageUrl = "images/process.png"
                node.ChildNodes.Add(n)
            End If
        Next
    End Sub

    Public Function findNode(ByVal value As String, ByVal nodes As TreeNodeCollection) As TreeNode
        If String.IsNullOrEmpty(value) Then
            Return Nothing
        End If
        For Each n As TreeNode In nodes
            If n.Value = value Then
                Return n
            Else
                ' search n's child nodes
                Dim nChild As TreeNode = findNode(value, n.ChildNodes)
                If nChild IsNot Nothing Then
                    Return nChild
                End If
            End If
        Next
        Return Nothing
    End Function

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

    Protected Sub gridView_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        Dim dataTable As DataTable = TryCast(ViewState("filedata"), DataTable)
        If dataTable IsNot Nothing Then
            Dim dataView As New DataView(dataTable)
            dataView.Sort = e.SortExpression & " " & GetSortDirection(e.SortExpression)
            gvData.DataSource = dataView
            gvData.DataBind()
            updPnlGrid.Update()
        End If
    End Sub

    Protected Sub gridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Dim dataTable As DataTable = TryCast(ViewState("filedata"), DataTable)
        If dataTable IsNot Nothing Then
            Dim dataView As New DataView(dataTable)
            gvData.PageIndex = e.NewPageIndex
            gvData.DataSource = dataView
            gvData.DataBind()
            updPnlGrid.Update()

        End If
    End Sub

    Private Function GetSortDirection(ByVal column As String) As String
        ' By default, set the sort direction to ascending.
        Dim sortDirection = "ASC"
        ' Retrieve the last column that was sorted.
        Dim sortExpression = TryCast(ViewState("SortExpression"), String)
        If sortExpression IsNot Nothing Then
            ' Check if the same column is being sorted.
            ' Otherwise, the default value can be returned.
            If sortExpression = column Then
                Dim lastDirection = TryCast(ViewState("SortDirection"), String)
                If lastDirection IsNot Nothing _
                  AndAlso lastDirection = "ASC" Then
                    sortDirection = "DESC"
                End If
            End If
        End If

        ' Save new values in ViewState.
        ViewState("SortDirection") = sortDirection
        ViewState("SortExpression") = column

        Return sortDirection

    End Function

    Public Function ConvertSortDirection(ByVal sdirection As SortDirection) As String
        Dim sortD As String = ""
        Select Case sdirection
            Case SortDirection.Ascending
                sortD = "ASC"
            Case SortDirection.Descending
                sortD = "DESC"
        End Select
        Return sortD
    End Function


    Private Sub BindDataGrid()
        If Not tv.SelectedNode Is Nothing Then
            If UCase(tv.SelectedNode.Parent.Value) = "PENDING APPROVALS" Then
                lblFolderName.Text = "Selected STATUS : " & tv.SelectedNode.Text
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As SqlConnection = New SqlConnection(conStr)
                Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
                Dim gid As Integer = Val(arr(1))
                Session("STID") = tv.SelectedNode.Value
                Dim od As New SqlDataAdapter("uspGetGridByDocType", con)
                od.SelectCommand.CommandType = CommandType.StoredProcedure
                od.SelectCommand.Parameters.AddWithValue("cStatus", tv.SelectedNode.Value)
                od.SelectCommand.Parameters.AddWithValue("uid", gid)
                od.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
                od.SelectCommand.Parameters.AddWithValue("documentType", tv.SelectedNode.Parent.Parent.Value)
                Dim ds As New DataSet
                od.Fill(ds, "data")
                gvData.DataSource() = ds.Tables("data")
                gvData.DataBind()
                ViewState("filedata") = ds.Tables("data")
                updPnlGrid.Update()
            End If
        Else
            lblFolderName.Text = "Selected STATUS : None"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim od As New SqlDataAdapter("uspGetGridByDocType", con)
            od.SelectCommand.CommandType = CommandType.StoredProcedure
            od.SelectCommand.Parameters.AddWithValue("cStatus", "")
            od.SelectCommand.Parameters.AddWithValue("uid", 0)
            od.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
            od.SelectCommand.Parameters.AddWithValue("documentType", "")
            Dim ds As New DataSet
            od.Fill(ds, "data")
            gvData.DataSource() = ds.Tables("data")
            gvData.DataBind()
            ViewState("filedata") = ds.Tables("data")
            updPnlGrid.Update()
        End If
        UpdMsg.Update()
    End Sub




    Protected Sub tv_SelectedNodeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tv.SelectedNodeChanged
        BindDataGrid()
    End Sub

    Protected Sub ShowHit(ByVal sender As Object, ByVal e As EventArgs)
        'get the file name and server credentials
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT fname,docurl  from MMM_MST_DOC where TID=" & pid, con)
        Dim ds As New DataSet()
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count <> 1 Then
            da.Dispose()
            con.Dispose()
            Exit Sub
        End If

        Dim docURl As String = ds.Tables("data").Rows(0).Item("docurl").ToString()

        da.Dispose()
        con.Dispose()

        Dim sb As StringBuilder = New StringBuilder("")
        Dim strRoot As String
        strRoot = Request.Url.GetLeftPart(UriPartial.Authority)
        sb.Append("window.open('" & strRoot & "/DOCS/" & docURl & "');")
        ScriptManager.RegisterClientScriptBlock(Me.updPnlGrid, Me.updPnlGrid.GetType(), "NewClientScript", sb.ToString(), True)
    End Sub

    Protected Sub ShowDetailAction(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ' No Value in Session just fill the Edit Form and Show two button
        'two methods.. either show data from Grid or Show data from Database.
        ViewState("pid") = pid
        BindDocDetailGrid(pid)
        BindDocumentDetail(pid)
        BindFutureMovementDetail(pid)
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Public Sub BindDocDetailGrid(ByVal pid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim fldQry As String = ""
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "uspGetDetailGridByDocid"
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.Parameters.AddWithValue("@pid", pid)
        oda.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
        oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))

        'For i As Integer = 0 To ds.Tables("fields").Rows.Count - 1
        '    fldQry = fldQry & "," & ds.Tables("fields").Rows(i).Item("fieldmapping").ToString() & " as [" & ds.Tables("fields").Rows(i).Item("displayname").ToString() & "]"
        'Next

        'Dim fnlQuery As String = "SELECT tid [Document ID],'<b>' + FName + '</b>' [File Name],Case oUID when " & Session("UID").ToString() & " then 'ME' else UserName end [UserName],adate [Upload Date],left(convert(nvarchar(20),filesize / (1024*1024)),5) + ' MB' [filesize]"
        'fnlQuery &= fldQry & ",curstatus [Current Status] FROM MMM_MST_DOC LEFT OUTER JOIN MMM_MST_USER ON uid=oUID WHERE tid=" & pid
        'oda.SelectCommand.CommandText = fnlQuery
        oda.Fill(ds, "data")

        Dim tblData As New StringBuilder()
        tblData.Append("<table cellspacing=""5px"" cellpadding=""5px"" width=""100%"" border=""3px"">")

        If ds.Tables("data").Rows.Count = 1 Then
            Dim cnt As Integer = ds.Tables("data").Columns.Count
            For i As Integer = 0 To ds.Tables("data").Columns.Count - 1
                If i Mod 2 = 0 Then
                    tblData.Append("<tr>")
                End If

                tblData.Append("<td align=""left"">" & ds.Tables("data").Columns(i).ColumnName & "</td>")
                If ds.Tables("data").Rows(0).Item(i).ToString().Contains(Session("EID").ToString() & "/") Then
                    tblData.Append("<td align=""left""><input type=""button"" value=""View Attachment"" onclick=""Javascript: return window.open('DOCS/" & ds.Tables("data").Rows(0).Item(i).ToString() & "', 'CustomPopUp', 'width=600, height=600, menubar=no, resizable=yes');"" /></td>")
                Else
                    tblData.Append("<td align=""left""><b>" & ds.Tables("data").Rows(0).Item(i).ToString() & "</td>")
                End If


                If i Mod 2 = 1 Then
                    tblData.Append("</tr>")
                End If
            Next
            lblDetail.Text = tblData.ToString() & "</table>"
        End If
        'dvData.DataSource = ds.Tables("data")
        'dvData.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub

    Public Sub BindDocumentDetail(ByVal pid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select tid,docid,fdate,tdate,ptat,atat, UserName [UserName],remarks from MMM_DOC_DTL D LEFT OUTER JOIN MMM_MST_USER U on D.userid=U.uid  where docid=" & pid & " order by tid"
        Dim ds As New DataSet()
        oda.SelectCommand.CommandType = CommandType.Text

        oda.Fill(ds, "data")
        gvMovDetail.DataSource = ds.Tables("data")
        gvMovDetail.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub

    'Protected Sub ShowApprove(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
    '    ' No Value in Session just fill the Edit Form and Show two button
    '    'two methods.. either show data from Grid or Show data from Database.
    '    ViewState("pid") = pid
    '    lblTab.Text = ""

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen", con)
    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '    oda.SelectCommand.Parameters.AddWithValue("docid", pid)
    '    oda.SelectCommand.Parameters.AddWithValue("subevent", "APPROVAL")
    '    Dim ds As New DataSet
    '    oda.Fill(ds, "fields")
    '    Session("FIELDS") = ds.Tables("fields")
    '    Session("ACTION") = "APPROVAL"
    '    If ds.Tables("fields").Rows.Count > 0 Then
    '        lblHeaderPopUp.Text = ds.Tables("fields").Rows(0).Item("Formcaption").ToString()
    '        Dim ob As New DynamicForm()
    '        pnlFields.Controls.Clear()
    '        ob.CreateControlsOnPanel(ds.Tables("fields"), pnlFields, updatePanelApprove, btnApprove)
    '    End If
    '    con.Close()
    '    oda.Dispose()
    '    con.Dispose()
    '    Me.updatePanelApprove.Update()
    '    Me.btnApprove_ModalPopupExtender.Show()
    'End Sub


    'Protected Sub ShowReject(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
    '    ' No Value in Session just fill the Edit Form and Show two button
    '    'two methods.. either show data from Grid or Show data from Database.
    '    ViewState("pid") = pid
    '    lblTabRej.Text = ""
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen", con)
    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '    oda.SelectCommand.Parameters.AddWithValue("docid", pid)
    '    oda.SelectCommand.Parameters.AddWithValue("subevent", "REJECTION")
    '    Dim dt As New DataTable
    '    oda.Fill(dt)
    '    Session("FIELDS") = dt
    '    Session("ACTION") = "REJECTION"
    '    If dt.Rows.Count > 0 Then
    '        lblHeaderPopUp.Text = dt.Rows(0).Item("Formcaption").ToString()
    '        Dim ob As New DynamicForm()
    '        pnlFields.Controls.Clear()
    '        ob.CreateControlsOnPanel(dt, pnlFieldsRej, updatePanelReject, btnReject)
    '    End If
    '    con.Close()
    '    oda.Dispose()
    '    con.Dispose()
    '    Me.updatePanelReject.Update()
    '    Me.btnReject_ModalPopupExtender.Show()
    'End Sub



    'Protected Sub editBtnApprove(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim dt As DataTable
    '    dt = CType(Session("FIELDS"), DataTable)
    '    Dim Updateqry As String = "UPDATE MMM_MST_DOC SET "
    '    Dim ob As New DynamicForm
    '    Dim retMsg As String = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET ", "", dt, pnlFields)
    '    If Trim(Left(retMsg, 6)).ToUpper() = "PLEASE" Then
    '        lblTab.Text = retMsg
    '    Else
    '        retMsg = retMsg & " WHERE tid=" & ViewState("pid")
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As SqlConnection = New SqlConnection(conStr)
    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("ApproveWorkFlow", con)
    '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '        oda.SelectCommand.Parameters.AddWithValue("TID", Val(ViewState("pid").ToString()))
    '        oda.SelectCommand.Parameters.AddWithValue("qry", retMsg)
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
    '        con.Close()
    '        oda.Dispose()
    '        con.Dispose()
    '        Session("ACTION") = Nothing
    '        Session("FIELDS") = Nothing

    '        If tv.SelectedNode Is Nothing Then
    '        Else
    '            Dim tvValuePath = tv.SelectedNode.ValuePath()
    '            LoadWorkGroupTree()
    '            Dim node As TreeNode = tv.FindNode(tvValuePath)
    '            If node Is Nothing Then
    '            Else
    '                node.Selected = True
    '                node.Parent.Expand()
    '            End If
    '        End If
    '        BindDataGrid()
    '        btnApprove_ModalPopupExtender.Hide()
    '    End If
    'End Sub

    'Protected Function ValidateData() As String
    '    'Check All Validations
    '    Dim dt As DataTable
    '    dt = CType(Session("FIELDS"), DataTable)
    '    Dim Updateqry As String = "UPDATE MMM_MST_DOC SET "
    '    lblTab.Text = "Please Enter "
    '    If dt.Rows.Count > 0 Then
    '        For i As Integer = 0 To dt.Rows.Count - 1
    '            If dt.Rows(i).Item("isrequired").ToString() = 1 Then
    '                Dim dispName As String = dt.Rows(i).Item("displayname").ToString()
    '                Select Case dt.Rows(i).Item("FieldType").ToString().ToUpper()
    '                    Case "TEXT BOX"
    '                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & i.ToString()), TextBox)
    '                        Updateqry &= dt.Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"

    '                        If dt.Rows(i).Item("isactive").ToString() And txtBox.Text.Length < 1 Then
    '                            lblTab.Text &= dispName & ", "
    '                        End If

    '                    Case "DROP DOWN"
    '                        Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & i.ToString()), DropDownList)
    '                        Updateqry &= dt.Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.SelectedItem.Value & "',"

    '                        If dt.Rows(i).Item("isactive").ToString() And txtBox.Text.Length < 1 Then
    '                            lblTab.Text &= dispName & ", "
    '                        End If

    '                    Case "TEXT AREA"
    '                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & i.ToString()), TextBox)
    '                        Updateqry &= dt.Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"
    '                        If dt.Rows(i).Item("isactive").ToString() And txtBox.Text.Length < 1 Then
    '                            lblTab.Text &= dispName & ", "
    '                        End If
    '                End Select
    '            End If
    '        Next
    '    End If


    '    If lblTab.Text.Length > 15 Then
    '        'Some error occured
    '        lblTab.Text = Left(lblTab.Text, lblTab.Text.Length - 1)
    '        Updateqry = "ERROR"
    '    Else
    '        'save the data
    '        Updateqry = Left(Updateqry, Len(Updateqry) - 1) & " WHERE TID=" & Val(ViewState("pid").ToString())
    '        lblTab.Text = ""
    '    End If
    '    updatePanelEdit.Update()
    '    Return Updateqry
    'End Function

    'Protected Function ValidateDataRejection() As String
    '    'Check All Validations
    '    Dim dt As DataTable
    '    dt = CType(Session("FIELDS"), DataTable)
    '    Dim Updateqry As String = "UPDATE MMM_MST_DOC SET "
    '    lblTabRej.Text = "Please Enter "
    '    If dt.Rows.Count > 0 Then
    '        For i As Integer = 0 To dt.Rows.Count - 1
    '            If dt.Rows(i).Item("isactive").ToString() = 1 Then
    '                Dim dispName As String = dt.Rows(i).Item("displayname").ToString()
    '                Select Case dt.Rows(i).Item("FieldType").ToString().ToUpper()
    '                    Case "TEXT BOX"
    '                        Dim txtBox As TextBox = CType(pnlFieldsRej.FindControl("rfld" & i.ToString()), TextBox)
    '                        Updateqry &= dt.Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"

    '                        If dt.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
    '                            lblTabRej.Text &= dispName & ", "
    '                        End If

    '                    Case "DROP DOWN"
    '                        Dim txtBox As DropDownList = CType(pnlFieldsRej.FindControl("rfld" & i.ToString()), DropDownList)
    '                        Updateqry &= dt.Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.SelectedItem.Value & "',"

    '                        If dt.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
    '                            lblTabRej.Text &= dispName & ", "
    '                        End If

    '                    Case "TEXT AREA"
    '                        Dim txtBox As TextBox = CType(pnlFieldsRej.FindControl("rfld" & i.ToString()), TextBox)
    '                        Updateqry &= dt.Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"
    '                        If dt.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
    '                            lblTabRej.Text &= dispName & ", "
    '                        End If
    '                End Select
    '            End If
    '        Next
    '    End If


    '    If lblTabRej.Text.Length > 15 Then
    '        'Some error occured
    '        lblTabRej.Text = Left(lblTabRej.Text, lblTabRej.Text.Length - 1)
    '        Updateqry = "ERROR"
    '    Else
    '        'save the data
    '        Updateqry = Left(Updateqry, Len(Updateqry) - 1) & " WHERE TID=" & Val(ViewState("pid").ToString())
    '        lblTabRej.Text = ""
    '    End If
    '    updatePanelReject.Update()
    '    Return Updateqry
    'End Function

    'Protected Sub editBtnReject(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim dt As DataTable
    '    dt = CType(Session("FIELDS"), DataTable)
    '    Dim ob As New DynamicForm
    '    Dim retMsg As String = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET ", "", dt, pnlFieldsRej)
    '    If Trim(Left(retMsg, 6)).ToUpper() = "PLEASE" Then
    '        lblTabRej.Text = retMsg
    '    Else
    '        retMsg = retMsg & " WHERE tid=" & ViewState("pid")
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As SqlConnection = New SqlConnection(conStr)
    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("RejectWorkFlow", con)
    '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '        oda.SelectCommand.Parameters.AddWithValue("TID", Val(ViewState("pid").ToString()))
    '        oda.SelectCommand.Parameters.AddWithValue("remarks", "REJECTED")
    '        oda.SelectCommand.Parameters.AddWithValue("qry", retMsg)

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
    '        con.Close()
    '        oda.Dispose()
    '        con.Dispose()
    '        Session("ACTION") = Nothing
    '        Session("FIELDS") = Nothing

    '        If tv.SelectedNode Is Nothing Then
    '        Else
    '            Dim tvValuePath = tv.SelectedNode.ValuePath()
    '            LoadWorkGroupTree()
    '            Dim node As TreeNode = tv.FindNode(tvValuePath)
    '            If node Is Nothing Then
    '            Else
    '                node.Selected = True
    '                node.Parent.Expand()
    '            End If
    '        End If
    '        BindDataGrid()
    '        btnReject_ModalPopupExtender.Hide()
    '    End If
    'End Sub

    Protected Sub BindGridbyDocid(ByVal docid As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("uspGetGridByDocID_OR_Fname", con)
        od.SelectCommand.CommandType = CommandType.StoredProcedure
        od.SelectCommand.Parameters.AddWithValue("docid", Val(docid))
        od.SelectCommand.Parameters.AddWithValue("Fname", docid)
        od.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
        od.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
        Dim ds As New DataSet
        od.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            gvData.DataSource() = ds.Tables("data")
            gvData.DataBind()
        Else
            lblMsg.Text = "No Record Found !"
            gvData.DataSource() = ds.Tables("data")
            gvData.DataBind()
        End If
        updPnlGrid.Update()
    End Sub

    Protected Sub BindGridbyDocidorFname(ByVal docid As String, ByVal Fname As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("uspGetGridByDocID_OR_Fname", con)
        If UCase(Session("USERROLE")) = "USR" Then
            od = New SqlDataAdapter("uspGetGridByDocID_OR_Fname", con)
        Else
            od = New SqlDataAdapter("uspGetGridByDocID", con)
        End If

        od.SelectCommand.CommandType = CommandType.StoredProcedure
        od.SelectCommand.Parameters.AddWithValue("docid", Val(docid))
        od.SelectCommand.Parameters.AddWithValue("Fname", Fname)
        od.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
        od.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
        Dim ds As New DataSet
        od.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            gvData.DataSource() = ds.Tables("data")
            gvData.DataBind()
        Else
            lblMsg.Text = "No Record Found !"
            gvData.DataSource() = ds.Tables("data")
            gvData.DataBind()
        End If
        updPnlGrid.Update()
    End Sub

    Protected Sub SearchbyDocid()
        lblMsg.Text = ""
        If txtDocid.Text = "" Then
            lblMsg.Text = "Please Enter Doc ID !"
            txtDocid.Focus()
            Exit Sub
        End If
        BindGridbyDocidorFname(txtDocid.Text, txtDocid.Text)
    End Sub

    Public Sub BindFutureMovementDetail(ByVal pid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectFuturePath", con)
        ' oda.SelectCommand.CommandText = "select dm.tid, u.username, dm.docid,dm.uid,dm.isauth, dm.sla from MMM_MOVPATH_DOC DM Inner join MMM_MST_USER U on u.uid=dm.UID where dm.DocID=" & pid & " order by dm.tid"
        Dim ds As New DataSet()
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("DOCID", Val(pid))
        oda.Fill(ds, "data")
        gvFutureMov.DataSource = ds.Tables("data")
        gvFutureMov.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub
End Class

