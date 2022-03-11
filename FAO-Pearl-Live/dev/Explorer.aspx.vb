Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports AjaxControlToolkit

Partial Class Explorer
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'Load All dynamic Controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("SELECT FormDesc,formcaption,displayName,FieldType,DropDownType,dropdown,FieldMapping,LayoutType,isrequired,datatype,fieldID , Cal_fields,lookupvalue  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = 'FILE' order by displayOrder", con)
        Dim ds As New DataSet
        od.Fill(ds, "fields")
        Dim ob As New DynamicForm()
        ob.CreateControlsOnPanel(ds.Tables("fields"), pnlFields, updatePanelAddFile, btnConfirm, 0)
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
    Protected Sub UploadFileHit(ByVal sender As Object, ByVal e As System.EventArgs)
        pnlFields.Visible = True
        btnConfirm.Visible = True
        updatePanelAddFile.Update()
        Me.btnAddFile_ModalPopupExtender.Show()
    End Sub

    Private Sub SaveData()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter(" ", con)
        da.SelectCommand.CommandText = "SELECT * FROM MMM_MST_FIELDS WHERE EID=" & Session("EID").ToString() & " and documenttype='FILE' order by displayOrder"
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.Parameters.Clear()
        Dim dss As New DataSet
        da.Fill(dss, "fields")

        If dss.Tables("fields").Rows.Count > 0 Then
            Dim Updateqry As String = "UPDATE MMM_MST_DOC SET "
            For i = 0 To dss.Tables("fields").Rows.Count - 1
                If dss.Tables("fields").Rows(i).Item("isactive").ToString() = 1 Then
                    Select Case dss.Tables("fields").Rows(i).Item("FieldType").ToString().ToUpper()
                        Case "TEXT BOX"
                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & i.ToString()), TextBox)
                            Updateqry &= dss.Tables("fields").Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"
                            txtBox.Text = ""
                        Case "DROP DOWN"
                            Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & i.ToString()), DropDownList)
                            Updateqry &= dss.Tables("fields").Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.SelectedItem.Text & "',"
                        Case "TEXT AREA"
                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & i.ToString()), TextBox)
                            Updateqry &= dss.Tables("fields").Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"
                            txtBox.Text = ""
                    End Select
                End If
            Next

            da.SelectCommand.CommandText = Left(Updateqry, Len(Updateqry) - 1) & " WHERE SessionID='" & Session.SessionID.ToString() & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        End If
        pnlFields.Enabled = True
        con.Close()
        da.Dispose()
        con.Dispose()

        'ob.InsertAction(Val(Session("UID").ToString()), "FILE UPLOADED", AcFileName & " is Uploaded to " & Session("FNAME").ToString())
        showFiles()
        Me.btnAddFile_ModalPopupExtender.Hide()
    End Sub

    Protected Sub fileOnUploadComplete(sender As Object, fl As AjaxFileUploadEventArgs)
        Dim i As Double
        i = fl.FileSize / (1024 * 1024)

        'lblMsgAddFile.Text = tv.SelectedNode.Text

        Dim ar() As String = fl.FileName.Split("\")
        Dim DocURL As String = ar(ar.Length - 1)
        Dim DocImage As String = ""

        Dim ext As String = Right(DocURL, 4)
        If Left(ext, 1) = "." Then
            ext = Right(ext, 3)
        End If

        Dim AcFileName As String = Replace(DocURL, "." & ext, "")

        Select Case ext.ToUpper()
            Case "DOC", "DOCX"
                DocImage = "word.png"
            Case "JPG"
                DocImage = "jpeg.png"
            Case "PDF"
                DocImage = "adobe.png"
            Case "XLS", "XLSX"
                DocImage = "excel.png"
            Case Else
                DocImage = "nofileimage.png"
        End Select


        DocURL = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & "." & ext
        AFlu.SaveAs(Server.MapPath("DOCS\") & "\" & DocURL)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT ipaddress,userid,pwd FROM MMM_MST_ENTITY where EID=" & Session("EID"), con)


        If Session("ISLOCAL").ToString() <> "TRUE" Then
            Dim ds As New DataSet()
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count <> 1 Then
                da.Dispose()
                con.Dispose()
                Exit Sub
            End If

            Dim ipaddress As String = ds.Tables("data").Rows(0).Item("ipaddress").ToString() & "/"
            Dim UserID As String = ds.Tables("data").Rows(0).Item("userid").ToString()
            Dim pwd As String = ds.Tables("data").Rows(0).Item("pwd").ToString()

            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & DocURL
            ' your server name or localhost        
            Dim ftpfullpath As String = "ftp://" & ipaddress & DocURL
            Dim ftp As FtpWebRequest = DirectCast(FtpWebRequest.Create(ftpfullpath), FtpWebRequest)
            ftp.Credentials = New NetworkCredential(UserID, pwd)
            'userid and password for the ftp server to given  

            ftp.KeepAlive = True
            ftp.UseBinary = True
            ftp.Method = WebRequestMethods.Ftp.UploadFile
            Dim fs As FileStream = File.OpenRead(FPath)
            Dim buffer As Byte() = New Byte(fs.Length - 1) {}
            fs.Read(buffer, 0, buffer.Length)
            fs.Close()
            Dim ftpstream As Stream = ftp.GetRequestStream()
            ftpstream.Write(buffer, 0, buffer.Length)
            ftpstream.Close()
        End If

        da.SelectCommand.CommandText = "uspAddFile"
        da.SelectCommand.CommandType = CommandType.StoredProcedure


        Dim stid As Integer = Val(Session("STID"))
        Dim gid As Integer = Val(Session("GID"))

        da.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
        da.SelectCommand.Parameters.AddWithValue("gid", gid)
        da.SelectCommand.Parameters.AddWithValue("fname", AcFileName)
        If stid > 9999 Then
            da.SelectCommand.Parameters.AddWithValue("stid", stid)
        End If
        da.SelectCommand.Parameters.AddWithValue("docurl", DocURL)
        da.SelectCommand.Parameters.AddWithValue("docimage", DocImage)
        da.SelectCommand.Parameters.AddWithValue("oUID", Session("UID").ToString())
        da.SelectCommand.Parameters.AddWithValue("filesize", fl.FileSize)
        da.SelectCommand.Parameters.AddWithValue("SessionID", Session.SessionID.ToString())

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim fileID As Integer = da.SelectCommand.ExecuteScalar()

        If Session("FILEID") Is Nothing Then
            Session("FILEID") = fileID
        Else
            Session("FILEID") = Session("FILEID") & "," & fileID
        End If

        con.Close()
        da.Dispose()
        con.Dispose()
        Dim ob As New DMSUtil()
        ob.InsertAction(Val(Session("UID").ToString()), "FILE UPLOADED", AcFileName & " is Uploaded to " & Session("FNAME").ToString())
        'ob.CheckWorkFlow(fileID, Val(Session("EID").ToString()))
        'ob.SendMailOfChanges("FILEUPLOADED", fileID)
        ''sending Mail through Notification Manager
        ob.TemplateCalling(fileID, Session("EID"), "FILE", "CREATED")
    End Sub

    Protected Sub gridView_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        Dim dataTable As DataTable = TryCast(Session("filedata"), DataTable)
        If dataTable IsNot Nothing Then
            Dim dataView As New DataView(dataTable)
            dataView.Sort = e.SortExpression & " " & GetSortDirection(e.SortExpression)
            gvData.DataSource = dataView
            gvData.DataBind()
        End If
    End Sub

    Protected Sub gridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Dim dataTable As DataTable = TryCast(Session("filedata"), DataTable)
        If dataTable IsNot Nothing Then
            Dim dataView As New DataView(dataTable)
            gvData.PageIndex = e.NewPageIndex
            gvData.DataSource = dataView
            gvData.DataBind()
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

    Protected Sub ValidateData(ByVal sender As Object, ByVal e As System.EventArgs)
        'Check All Validations
        ' now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE Isactive=1 and  EID=" & Session("EID").ToString() & " and Documenttype='FILE' order by displayOrder", con)
        Dim ds As New DataSet
        od.Fill(ds, "fields")
        If ds.Tables("fields").Rows.Count > 0 Then

            Dim ob As New DynamicForm()
            Dim FinalQry As String = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET ", "", ds.Tables("fields"), pnlFields, 0)
            lblMsgAddFile.Text = "Please Enter "

            'If ValidationAttribute finished.. then lock all fields for editing

            If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
                'Some error occured
                lblMsgAddFile.Text = FinalQry
                updatePanelAddFile.Update()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "message", " $('#hideShow').css('display', 'block');", True)
            Else
                od.SelectCommand.CommandText = FinalQry & " WHERE SessionID='" & Session.SessionID.ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                od.SelectCommand.ExecuteNonQuery()
                Dim ob1 As New DMSUtil()
                Dim ARR() As String = Session("FILEID").ToString().Split(",")
                For i As Integer = 0 To ARR.Length - 1
                    ob1.CheckWorkFlow(Val(ARR(i)), Val(Session("EID").ToString()))
                Next
                Session("FILEID") = Nothing
                'save the data
                lblMsgAddFile.Text = ""
                od.Dispose()
                con.Dispose()
                btnAddFile_ModalPopupExtender.Hide()
                showFiles()
                'btnConfirm.Visible = False
                'btnActAddFile.Visible = True
                'AFlu.Visible = True
            End If
        End If
        Me.btnAddFile_ModalPopupExtender.Hide()
        showFiles()
        Me.updatePanelAddFile.Update()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindProject()
            lblFolderName.Text = "Selected Folder : None"
            btnCreateFolder.Visible = False
            btnDeleteFolder.Visible = False
            btnUploadFile.Visible = False
            btnRenameFolder.Visible = False
            UpdMsg.Update()
        End If

        If ViewState("ERROR") <> Nothing Then
            If ViewState("ERROR") = "FILEERROR" Then
                Me.btnAddFile_ModalPopupExtender.Show()
            End If
        End If
    End Sub

    Private Sub BindProject()
        tv.Nodes.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim od As New SqlDataAdapter("SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " order by grpName", con)
        If Session("USERROLE").ToString() = "USR" Then
            od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select grpid from MMM_REF_GRP_USR where uId = " & Session("UID").ToString() & ")  order by grpName"
        End If

        Dim ds As New DataSet
        od.Fill(ds, "boss")

        Dim masterNode As New TreeNode(Session("USERNAME").ToString())
        masterNode.Value = "MAIN"
        masterNode.ImageUrl = "images/mainnode.png"
        masterNode.SelectAction = TreeNodeSelectAction.Expand
        tv.Nodes.Add(masterNode)

        For Each rw As DataRow In ds.Tables("boss").Rows
            If rw.IsNull("stid") Then
                Dim n As New TreeNode()
                If Session("UID").ToString() = rw.Item("own").ToString() Then
                    n.Text = rw.Item("foldername") & "<b>-(OWNER)</b>"
                Else
                    n.Text = rw.Item("foldername")
                End If
                n.Value = rw.Item("tid")
                n.ImageUrl = "images/project.png"
                masterNode.ChildNodes.Add(n)
            End If
        Next
        tv.ExpandAll()
        updTV.Update()

    End Sub

    Protected Sub tv_SelectedNodeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tv.SelectedNodeChanged
        If tv.SelectedNode.Value.ToString() <> "MAIN" Then
            lblFolderName.Text = "Selected Folder : " & tv.SelectedNode.Text
            Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
            Dim gid As Integer = Val(arr(1))
            Session("STID") = tv.SelectedNode.Value
            Session("GID") = gid
            Session("FNAME") = tv.SelectedNode.Text
            'LoadWorkGroupTreechild()
            If Session("USERROLE").ToString() = "USR" Then
                If tv.FindNode("MAIN/" & gid.ToString()).Text.Contains("(OWNER)") Then
                    btnCreateFolder.Visible = True
                    btnDeleteFolder.Visible = True
                    btnRenameFolder.Visible = True
                Else
                    btnCreateFolder.Visible = False
                    btnDeleteFolder.Visible = False
                    btnRenameFolder.Visible = False
                End If
            Else
                btnCreateFolder.Visible = True
                btnDeleteFolder.Visible = True
                btnRenameFolder.Visible = True
            End If
            btnUploadFile.Visible = True
        Else
            lblFolderName.Text = "Selected Folder : None"
            btnCreateFolder.Visible = False
            btnDeleteFolder.Visible = False
            btnUploadFile.Visible = False
            btnRenameFolder.Visible = False
        End If
        UpdMsg.Update()
        UpdMsgTV.Update()
        If Not (tv.SelectedNode.ChildNodes.Count > 0) Then

            BindChilds(tv.SelectedNode, Convert.ToInt32(tv.SelectedNode.Value))

            tv.SelectedNode.Expand()

        End If
        showFiles()
    End Sub
    Private Function GetData(ByVal commandText As String) As SqlDataReader
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        Dim sqlcmd As New SqlCommand(commandText, con)
        Dim dr As SqlDataReader = sqlcmd.ExecuteReader()
        Return dr
    End Function
    Private Sub BindChilds(node As TreeNode, parentNodeID As Integer)
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
    Private Sub showFiles()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("uspSelectFiles", con)
        od.SelectCommand.CommandType = CommandType.StoredProcedure
        od.SelectCommand.Parameters.Clear()
        od.SelectCommand.Parameters.AddWithValue("stid", Session("STID"))
        od.SelectCommand.Parameters.AddWithValue("uid", Val(Session("UID").ToString()))
        Dim ds As New DataSet
        od.Fill(ds, "data")

        If ds.Tables("data").Rows.Count > 0 Then
            'enable File operation buttons

        Else
            'disable file operation buttons

        End If
        gvData.DataSource = ds.Tables("data")
        Session("filedata") = ds.Tables("data")
        gvData.DataBind()
        updPnlGrid.Update()
    End Sub

    Private Sub LoadWorkGroupTree()
        tv.Nodes.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " order by grpName", con)
        If Session("USERROLE").ToString() = "USR" Then
            od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select grpid from MMM_REF_GRP_USR where uId = " & Session("UID").ToString() & ")  order by grpName"
        End If
        'Dim od As New SqlDataAdapter("SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " UNION SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and grpName = 'DEFAULT' UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null and eid=" & Session("eid").ToString() & " AND foldername is not null UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " order by tid", con)

        'If Session("USERROLE").ToString() = "USR" Then
        '    od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select grpid from MMM_REF_GRP_USR where uId = " & Session("UID").ToString() & ") UNION SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and grpName = 'DEFAULT' UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null AND foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select grpid from MMM_REF_GRP_USR where uId = " & Session("UID").ToString() & ") UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select grpid from MMM_REF_GRP_USR where uId = " & Session("UID").ToString() & ") order by tid"
        'End If


        Dim ds As New DataSet
        od.Fill(ds, "boss")
        Dim dr As New DataRelation("bossrelation", ds.Tables("boss").Columns("tid"), ds.Tables("boss").Columns("stid"))
        'ViewState("td") = ds.Tables("bosss").Rows(0).Item("stid").ToString()
        ds.Relations.Add(dr)
        Dim masterNode As New TreeNode(Session("USERNAME").ToString())
        masterNode.Value = "MAIN"
        masterNode.ImageUrl = "images/mainnode.png"
        masterNode.SelectAction = TreeNodeSelectAction.Expand
        tv.Nodes.Add(masterNode)

        For Each rw As DataRow In ds.Tables("boss").Rows
            If rw.IsNull("stid") Then
                Dim n As New TreeNode()
                If Session("UID").ToString() = rw.Item("own").ToString() Then
                    n.Text = rw.Item("foldername") & "<b>-(OWNER)</b>"
                Else
                    n.Text = rw.Item("foldername")
                End If
                n.Value = rw.Item("tid")
                n.ImageUrl = "images/project.png"
                masterNode.ChildNodes.Add(n)
                LoadRecTree(rw, n)
            End If
        Next

        'Traverse tree and remove those nodes which are not authorized

        od.SelectCommand.CommandText = "SELECT fileid from MMM_MST_DENYRULE where UID=" & Session("UID").ToString() & " ORDER by tid"
        od.SelectCommand.CommandType = CommandType.Text
        od.Fill(ds, "auth")
        For i As Integer = 0 To ds.Tables("auth").Rows.Count - 1
            Dim fileid As Integer = Val(ds.Tables("auth").Rows(i).Item("fileid").ToString())

            Dim nb As New TreeNode
            nb = findNode(fileid, tv.Nodes)
            If Not nb Is Nothing Then
                nb.Parent.ChildNodes.Remove(nb)
                '                tv.Nodes.Remove(nb)
            End If
        Next
        con.Dispose()
        od.Dispose()
        ds.Dispose()
        ds = Nothing
        ' tv.ExpandAll()
        updTV.Update()
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

    Protected Sub AddFolderHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgAddFolder.Text = ""
        '         = System.Net.Dns.GetHostName() & " IP " & System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList(0).ToString()
        txtFolderName.Text = ""
        updatePanelAddFolder.Update()
        Me.btnAddFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub FolderRenameHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgRenameFolder.Text = ""
        txtFolderReName.Text = Session("FNAME").ToString()
        updatePanelRenameFolder.Update()
        Me.btnRenameFolder_ModalPopupExtender.Show()
    End Sub


    Protected Sub RenameFolder(ByVal sender As Object, ByVal e As System.EventArgs)
        If txtFolderReName.Text.Length < 3 Then
            lblMsgRenameFolder.Text = "please enter A valid Name for Folder"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("UPDATE MMM_MST_DOC SET foldername='" & txtFolderReName.Text & "' where tid=" & Session("STID").ToString(), con)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()

        Dim ob As New DMSUtil()
        ob.InsertAction(Val(Session("UID").ToString()), "FOLDER RENAME", Session("FNAME").ToString() & " is Renamed to " & txtFolderReName.Text)
        'Now rename selected folder to this text
        tv.SelectedNode.Text = txtFolderReName.Text
        Session("FNAME") = txtFolderReName.Text
        lblFolderName.Text = "Selected Folder : " & txtFolderReName.Text
        UpdMsg.Update()
        updTV.Update()
        btnRenameFolder_ModalPopupExtender.Hide()
    End Sub

    Protected Sub AddFolder(ByVal sender As Object, ByVal e As System.EventArgs)
        If txtFolderName.Text.Length < 3 Then
            lblMsgAddFolder.Text = "please enter A valid Name for Folder"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspAddFolder", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure

        Dim stid As Integer = tv.SelectedNode.Value
        Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
        Dim gid As Integer = Val(arr(1))

        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("gid", gid)
        oda.SelectCommand.Parameters.AddWithValue("foldername", txtFolderName.Text)

        If stid > 9999 Then
            oda.SelectCommand.Parameters.AddWithValue("stid", stid)
        End If

        oda.SelectCommand.Parameters.AddWithValue("oUID", Session("UID").ToString())

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()

        Dim ob As New DMSUtil()
        ob.InsertAction(Val(Session("UID").ToString()), "FOLDER CREATION", txtFolderName.Text & " is Created")

        ob.TemplateCalling(iSt, Session("EID"), "FOLDER", "CREATED")
        txtFolderName.Text = ""
        LoadWorkGroupTree()
        lblFolderName.Text = "Selected Folder : None"
        btnCreateFolder.Visible = False
        btnDeleteFolder.Visible = False
        btnUploadFile.Visible = False
        UpdMsg.Update()
        btnAddFolder_ModalPopupExtender.Hide()
    End Sub

    Protected Sub DelFolderHit(ByVal sender As Object, ByVal e As System.EventArgs)
        If Val(tv.SelectedNode.Value) < 10000 Then
            lblMsgDelFolder.Text = "You can not delete a Project"
            btnActDelFolder.Visible = False
        Else
            'check if folder can be deleted 
            lblMsgDelFolder.Text = "Are you Sure Want to Delete <b>" & tv.SelectedNode.Text & "</b> And all its subfolder and files"
            btnActDelFolder.Visible = True
        End If
        updatePanelDelFolder.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub DelFolder(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDelFolder", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", tv.SelectedNode.Value)

        Dim ob As New DMSUtil()
        ob.TemplateCalling(tv.SelectedNode.Value, Session("EID"), "FOLDER", "DELETED")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()

        ob.InsertAction(Val(Session("UID").ToString()), "FOLDER DELETED", tv.SelectedNode.Text & " is Deleted")
        Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
        Dim gid As Integer = Val(arr(1))
        LoadWorkGroupTree()
        lblFolderName.Text = "Selected Folder : None"
        btnCreateFolder.Visible = False
        btnDeleteFolder.Visible = False
        btnUploadFile.Visible = False
        UpdMsg.Update()
        btnDelFolder_ModalPopupExtender.Hide()
    End Sub


    'Protected Sub UploadFile(sender As Object, e As System.EventArgs)

    '    If Not AFlu.HasFile Then
    '        lblMsgAddFile.Text = "please Enter a valid File name"
    '        ViewState("ERROR") = "FILEERROR"
    '        Exit Sub
    '    End If

    '    Dim DocURL As String = flU.PostedFile.FileName
    '    Dim DocImage As String = ""

    '    Dim ext As String = Right(DocURL, 4)
    '    If Left(ext, 1) = "." Then
    '        ext = Right(ext, 3)
    '    End If

    '    Dim AcFileName As String = Replace(flU.FileName, "." & ext, "")

    '    Select Case ext.ToUpper()
    '        Case ".doc", ".docx"
    '            DocImage = "word.png"
    '        Case "JPG"
    '            DocImage = "jpeg.png"
    '        Case "PDF"
    '            DocImage = "adobe.png"
    '        Case "XLS"
    '            DocImage = "excel.png"
    '        Case Else
    '            DocImage = "nofileimage.png"
    '    End Select

    '    Dim i As Double
    '    i = flU.PostedFile.ContentLength / (1024 * 1024)
    '    If i > 10 Then
    '        lblMsgAddFile.Text = "File Size should not be greater then 10MB"
    '        Exit Sub
    '    End If

    '    DocURL = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & "." & ext
    '    flU.PostedFile.SaveAs(Server.MapPath("DOCS\") & "\" & DocURL)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT ipaddress,userid,pwd FROM MMM_MST_ENTITY where EID=" & Session("EID"), con)


    '    If Session("ISLOCAL").ToString() <> "TRUE" Then
    '        Dim ds As New DataSet()
    '        da.Fill(ds, "data")
    '        If ds.Tables("data").Rows.Count <> 1 Then
    '            da.Dispose()
    '            con.Dispose()
    '            Exit Sub
    '        End If

    '        Dim ipaddress As String = ds.Tables("data").Rows(0).Item("ipaddress").ToString() & "/"
    '        Dim UserID As String = ds.Tables("data").Rows(0).Item("userid").ToString()
    '        Dim pwd As String = ds.Tables("data").Rows(0).Item("pwd").ToString()

    '        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & DocURL
    '        ' your server name or localhost        
    '        Dim ftpfullpath As String = "ftp://" & ipaddress & DocURL
    '        Dim ftp As FtpWebRequest = DirectCast(FtpWebRequest.Create(ftpfullpath), FtpWebRequest)
    '        ftp.Credentials = New NetworkCredential(UserID, pwd)
    '        'userid and password for the ftp server to given  

    '        ftp.KeepAlive = True
    '        ftp.UseBinary = True
    '        ftp.Method = WebRequestMethods.Ftp.UploadFile
    '        Dim fs As FileStream = File.OpenRead(FPath)
    '        Dim buffer As Byte() = New Byte(fs.Length - 1) {}
    '        fs.Read(buffer, 0, buffer.Length)
    '        fs.Close()
    '        Dim ftpstream As Stream = ftp.GetRequestStream()
    '        ftpstream.Write(buffer, 0, buffer.Length)
    '        ftpstream.Close()
    '    End If

    '    da.SelectCommand.CommandText = "uspAddFile"
    '    da.SelectCommand.CommandType = CommandType.StoredProcedure

    '    Dim stid As Integer = tv.SelectedNode.Value
    '    Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
    '    Dim gid As Integer = Val(arr(1))

    '    da.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
    '    da.SelectCommand.Parameters.AddWithValue("gid", gid)
    '    da.SelectCommand.Parameters.AddWithValue("fname", AcFileName)
    '    If stid > 9999 Then
    '        da.SelectCommand.Parameters.AddWithValue("stid", stid)
    '    End If
    '    da.SelectCommand.Parameters.AddWithValue("docurl", DocURL)
    '    da.SelectCommand.Parameters.AddWithValue("docimage", DocImage)
    '    da.SelectCommand.Parameters.AddWithValue("oUID", Session("UID").ToString())
    '    da.SelectCommand.Parameters.AddWithValue("filesize", flU.PostedFile.ContentLength)

    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
    '    ViewState("ERROR") = Nothing
    '    da.SelectCommand.CommandText = "SELECT * FROM MMM_MST_FIELDS WHERE EID=" & Session("EID").ToString() & " order by displayOrder"
    '    da.SelectCommand.CommandType = CommandType.Text
    '    da.SelectCommand.Parameters.Clear()
    '    Dim dss As New DataSet
    '    da.Fill(dss, "fields")

    '    If dss.Tables("fields").Rows.Count > 0 Then
    '        Dim Updateqry As String = "UPDATE MMM_MST_DOC SET "
    '        For i = 0 To dss.Tables("fields").Rows.Count - 1
    '            If dss.Tables("fields").Rows(i).Item("isactive").ToString() = 1 Then
    '                Select Case dss.Tables("fields").Rows(i).Item("FieldType").ToString().ToUpper()
    '                    Case "TEXT BOX"
    '                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & i.ToString()), TextBox)
    '                        Updateqry &= dss.Tables("fields").Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"
    '                        txtBox.Text = ""
    '                    Case "DROP DOWN"
    '                        Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & i.ToString()), DropDownList)
    '                        Updateqry &= dss.Tables("fields").Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.SelectedItem.Text & "',"
    '                    Case "TEXT AREA"
    '                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & i.ToString()), TextBox)
    '                        Updateqry &= dss.Tables("fields").Rows(i).Item("Fieldmapping").ToString() & "='" & txtBox.Text & "',"
    '                        txtBox.Text = ""
    '                End Select
    '            End If
    '        Next
    '        da.SelectCommand.CommandText = Left(Updateqry, Len(Updateqry) - 1) & " WHERE TID=" & fileID
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        da.SelectCommand.ExecuteNonQuery()
    '    End If
    '    pnlFields.Enabled = True
    '    con.Close()
    '    da.Dispose()
    '    con.Dispose()
    '    Dim ob As New DMSUtil()
    '    ob.InsertAction(Val(Session("UID").ToString()), "FILE UPLOADED", AcFileName & " is Uploaded to " & tv.SelectedNode.Text)
    '    showFiles()
    '    btnAddFile_ModalPopupExtender.Hide()
    'End Sub

    Private Sub GetFileToUser(ByVal fullpath As String, ByVal fname As String)
        Try
            Dim ext = Path.GetExtension(fullpath)
            Dim type As String = ""

            Select Case ext
                Case ".htm", ".html"
                    type = "text/HTML"
                Case ".txt"
                    type = "text/plain"
                Case ".doc", ".rtf"
                    type = "Application/msword"
                Case ".csv", ".xls"
                    type = "Application/x-msexcel"
                Case ".pdf"
                    type = "Application/pdf"
                    'Case Else
                    '    type = "text/plain"
            End Select

            '            Response.Redirect(fullpath)
            Response.AddHeader("content-disposition", "attachment; filename=" & fname & ext)

            If type <> "" Then
                Response.ContentType = type
            End If

            Response.WriteFile(fullpath)
            Response.End()
        Catch ex As Exception
            Dim errMsg As String = (ex.Message)
            lblMsg.Text = errMsg
        End Try
    End Sub

    Private Sub CopyFileFromFTP(ByVal IPAddress As String, ByVal UserID As String, ByVal pwd As String, ByVal docURl As String, ByVal fullpath As String)

        Dim host As String = "ftp://" & IPAddress
        'Create a request
        Dim URI1 As String = host & docURl
        Dim downloadRequest As FtpWebRequest = DirectCast(WebRequest.Create(URI1), FtpWebRequest)
        downloadRequest.Credentials = New NetworkCredential(UserID, pwd)
        downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile
        Dim downloadResponse As FtpWebResponse = DirectCast(downloadRequest.GetResponse(), FtpWebResponse)

        Try
            Dim downloadStream As Stream = downloadResponse.GetResponseStream()
            If downloadStream IsNot Nothing Then
                Dim downloadReader As New StreamReader(downloadStream)
                Try
                    If downloadReader IsNot Nothing Then
                        Dim downloadWriter As New StreamWriter(fullpath)
                        downloadWriter.AutoFlush = True
                        downloadWriter.Write(downloadReader.ReadToEnd())
                        downloadWriter.Close()
                    End If
                Finally
                    If downloadReader IsNot Nothing Then
                        downloadReader.Close()
                    End If
                End Try
            End If
        Finally

            If downloadResponse IsNot Nothing Then
                downloadResponse.Close()
            End If
        End Try

        Response.AddHeader("content-disposition", "attachment; filename=" & docURl)

        Response.WriteFile(fullpath)
        Response.End()


        ''Use this for Accessing from FTP Server 
        'Dim reqFTP As FtpWebRequest
        'reqFTP = DirectCast(FtpWebRequest.Create(New Uri("ftp://" & IPAddress & docURl)), FtpWebRequest)
        'reqFTP.Method = WebRequestMethods.Ftp.DownloadFile
        'reqFTP.UsePassive = True
        'reqFTP.UseBinary = True
        'reqFTP.KeepAlive = False
        'reqFTP.Credentials = New NetworkCredential(UserID, pwd)
        'Dim resp As FtpWebResponse = DirectCast(reqFTP.GetResponse(), FtpWebResponse)
        'Dim ftpStream As Stream = resp.GetResponseStream()

        ' ''          // note: since you are writing directly to client, I removed the `file` stream in your original code since we don't need to store the file locally... or so I am assuming
        'Response.AddHeader("content-disposition", "attachment;filename=" + docURl)

        'Dim cl As Long = resp.ContentLength
        'Dim bufferSize As Integer = 2048
        'Dim readCount As Integer
        'Dim buffer As Byte() = New Byte(bufferSize - 1) {}
        'readCount = ftpStream.Read(buffer, 0, bufferSize)
        'While readCount > 0
        '    Response.OutputStream.Write(buffer, 0, readCount)
        '    readCount = ftpStream.Read(buffer, 0, bufferSize)
        'End While
        'ftpStream.Flush()
        'ftpStream.Close()
        'resp.Close()

        ''''filePath = The full path where the file is to be created. Here i put local system path.

    End Sub

    Protected Sub ShowHit(ByVal sender As Object, ByVal e As EventArgs)
        'get the file name and server credentials
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT fname,docurl,ipaddress,UserID,pwd  from MMM_MST_DOC left outer join MMM_MST_ENTITY on MMM_MST_DOC.EID=MMM_MST_ENTITY.EID where TID=" & pid, con)
        Dim ds As New DataSet()
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count <> 1 Then
            da.Dispose()
            con.Dispose()
            Exit Sub
        End If

        Dim ipaddress As String = ds.Tables("data").Rows(0).Item("ipaddress").ToString() & "/"
        Dim UserID As String = ds.Tables("data").Rows(0).Item("userid").ToString()
        Dim pwd As String = ds.Tables("data").Rows(0).Item("pwd").ToString()
        Dim docURl As String = ds.Tables("data").Rows(0).Item("docurl").ToString()
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\")
        Dim fname As String = ds.Tables("data").Rows(0).Item("fname").ToString()

        da.Dispose()
        con.Dispose()

        Dim fullpath = Path.GetFullPath(FPath & "\" & docURl)

        If File.Exists(fullpath) Then
            GetFileToUser(fullpath, fname)
        Else
            CopyFileFromFTP(ipaddress, UserID, pwd, docURl, fullpath)
        End If
        Dim ob As New DMSUtil()
        ob.InsertAction(Val(Session("UID").ToString()), "FILE VIEWED", row.Cells(0).ToString() & " is Viewed from " & tv.SelectedNode.Text)
    End Sub

    Protected Sub DelFileHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TID") = pid
        ViewState("FNAME") = row.Cells(2).Text
        lblMsgDelFile.Text = "Are you Sure Want to Delete <b>" & row.Cells(2).Text & "</b> And all its reference!"
        btnActDelFile.Visible = True
        updatePanelDelFile.Update()
        Me.btnDelFile_ModalPopupExtender.Show()
    End Sub

    Protected Sub MovFileHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim row As GridViewRow
        Dim i As Integer = 0
        Dim CheckBox As System.Web.UI.WebControls.CheckBox
        For Each row In gvData.Rows
            CheckBox = row.FindControl("Check")
            If CheckBox.Checked = True Then
                i = i + 1
            End If
        Next
        If i < 1 Then
            lblMsgMovFile.Text = "Please Select Files to Move"
            btnActMovFile.Visible = False
        Else
            lblMsgMovFile.Text = "Please Select Target Folder To Move All FIles"
            LoadWorkGroupTreeMove()
            btnActMovFile.Visible = True
        End If
        updatePanelMovFile.Update()
        Me.btnMovFile_ModalPopupExtender.Show()
    End Sub

    Private Sub LoadWorkGroupTreeMove()
        tvMove.Nodes.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim od As New SqlDataAdapter("SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null AND foldername is not null and eid=" & Session("eid").ToString() & " UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " order by tid", con)

        If Session("USERROLE").ToString() = "USR" Then
            od.SelectCommand.CommandText = "SELECT gid [tid],null [stid],grpName [FolderName],gwUID [own] FROM MMM_MST_GROUP where eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId = " & Session("UID").ToString() & ") UNION select tid,isnull(stid,gid) [stid],foldername,oUID [own] FROM MMM_MST_DOC where Stid is null AND foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId= " & Session("UID").ToString() & ") UNION SELECT tid,stid,foldername,oUID [own] FROM MMM_MST_DOC where Stid is not null and foldername is not null and eid=" & Session("eid").ToString() & " and gid in (select gid from MMM_MST_GROUP where gwuId= " & Session("UID").ToString() & ") order by tid"
        End If

        Dim ds As New DataSet
        od.Fill(ds, "boss")

        Dim dr As New DataRelation("bossrelation", ds.Tables("boss").Columns("tid"), ds.Tables("boss").Columns("stid"))
        ds.Relations.Add(dr)
        Dim masterNode As New TreeNode(Session("USERNAME").ToString())
        masterNode.Value = "MAIN"
        masterNode.ImageUrl = "images/mainnode.png"
        masterNode.SelectAction = TreeNodeSelectAction.Expand
        tvMove.Nodes.Add(masterNode)
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
        tvMove.ExpandAll()
    End Sub

    Protected Sub MovFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim row As GridViewRow
        Dim i As Integer = 0
        Dim CheckBox As System.Web.UI.WebControls.CheckBox
        Dim pids As String = ""
        Dim fNames As String = ""
        For Each row In gvData.Rows
            CheckBox = row.FindControl("Check")
            If CheckBox.Checked = True Then
                pids &= Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value) & ","
                fNames &= Me.gvData.Rows(row.RowIndex).Cells(2).Text & ","
                i = i + 1
            End If
        Next
        If i < 1 Then
            lblMsgMovFile.Text = "Please Select Files to Move"
            Exit Sub
        Else
            pids = Left(pids, Len(pids) - 1)
            fNames = Left(fNames, Len(fNames) - 1)
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("UPDATE MMM_MST_DOC SET stid= " & tvMove.SelectedNode.Value & " WHERE tid in (" & pids & ")", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        Dim ob As New DMSUtil()

        ob.InsertAction(Val(Session("UID").ToString()), "FILE MOVED", fNames & " Is Moved to " & tvMove.SelectedNode.Text)
        updatePanelMovFile.Update()
        Me.btnMovFile_ModalPopupExtender.Hide()
    End Sub

    Protected Sub RenameFileHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TID") = pid
        ViewState("FNAME") = row.Cells(2).Text
        ViewState("INDEX") = row.RowIndex
        lblMsgRenameFile.Text = ""
        txtFileReName.Text = row.Cells(2).Text
        updatePanelRenameFile.Update()
        Me.btnRenameFile_ModalPopupExtender.Show()
    End Sub

    Protected Sub RenameFile(ByVal sender As Object, ByVal e As System.EventArgs)
        If txtFileReName.Text.Length < 3 Then
            lblMsgRenameFile.Text = "please enter A valid file name"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("UPDATE MMM_MST_DOC SET fname='" & txtFileReName.Text & "' where tid=" & ViewState("TID").ToString(), con)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()

        Dim ob As New DMSUtil()
        ob.InsertAction(Val(Session("UID").ToString()), "FILE RENAME", ViewState("FNAME").ToString() & " is Renamed to " & txtFileReName.Text)
        'Now rename selected file to new filename
        gvData.Rows(Val(ViewState("INDEX").ToString)).Cells(1).Text = txtFileReName.Text
        Session("FNAME") = txtFolderReName.Text
        btnRenameFile_ModalPopupExtender.Hide()
    End Sub

    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("DELETE MMM_MST_DOC WHERE TID=" & ViewState("TID").ToString(), con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim ob As New DMSUtil()
        ob.TemplateCalling(ViewState("TID").ToString(), Session("EID"), "FILE", "DELETED")
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        ob.InsertAction(Val(Session("UID").ToString()), "FILE DELETED", ViewState("FNAME").ToString() & " is Deleted from " & tv.SelectedNode.Text)
        showFiles()
        btnDelFile_ModalPopupExtender.Hide()
    End Sub

    Protected Sub gvData_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        'Attache Postback trigger to Image view button

        Dim imgBut As ImageButton = CType(e.Row.FindControl("btnView"), ImageButton)
        Dim sc As ScriptManager = CType(Master.FindControl("scriptmanager1"), ScriptManager)
        sc.RegisterPostBackControl(imgBut)

        'Enable disable Command button
        If Session("USERROLE").ToString() = "USR" Then

            Dim arr() As String = tv.SelectedNode.ValuePath.Split("/")
            Dim gid As Integer = Val(arr(1))

            If tv.FindNode("MAIN/" & gid.ToString()).Text.Contains("(OWNER)") Then
                Exit Sub
            End If

            'Check if the user is uploader of the file
            If e.Row.Cells(1).Text <> "ME" Then
                Dim imgDel As ImageButton = CType(e.Row.FindControl("btnDelete"), ImageButton)
                imgDel.Visible = False

                If tv.SelectedNode.Text = "DEFAULT" Then
                    e.Row.Visible = False
                End If
            End If
            ' Dim imgBut As ImageButton = CType(e.Row.FindControl("btnView"), ImageButton)
            ' imgBut.Visible = False
            'End If
        End If
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