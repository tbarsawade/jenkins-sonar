Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Partial Class mobile_MasterNew
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Session("EID") Is Nothing Then
            If Request.QueryString("SC") Is Nothing Then
                Response.Redirect("~/mobile/MainHome.aspx")
            Else
                Dim screen As String = Request.QueryString("SC").ToString()
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As SqlConnection = New SqlConnection(conStr)
                Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & screen & "' order by displayOrder", con)
                Dim ds As New DataSet()
                oda.Fill(ds, "fields")
                Dim ob As New DynamicForm
                ob.CLEARDYNAMICFIELDS(pnlFields)
                ob.CreateControlsOnPanel(ds.Tables("fields"), pnlFields, updatePanelEdit, btnActEdit, 0)
                Dim ROW1() As DataRow = ds.Tables("fields").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
                If ROW1.Length > 0 Then
                    For i As Integer = 0 To ROW1.Length - 1
                        Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                        DDL.AutoPostBack = True
                        AddHandler DDL.TextChanged, AddressOf bindvalue
                    Next
                End If

                ''Child Item 
                Dim dtchild As DataTable = ds.Tables("fields")
                Dim row() As DataRow = dtchild.Select("FieldType='CHILD ITEM'")
                If row.Length > 0 Then
                    For i As Integer = 0 To row.Length - 1
                        Dim btn1 As Button = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
                        AddHandler btn1.Click, AddressOf ShowChildForm
                        Dim PRitem() As String = row(i).Item("dropdown").ToString().Split("-")
                        If PRitem.Length > 1 Then
                            Dim BTN2 As Button = pnlFields.FindControl("BTN" & PRitem(1).ToString & "-" & row(i).Item("FIELDID").ToString())
                            AddHandler BTN2.Click, AddressOf INSERTCHILDITEM
                        End If
                        Dim GRD As GridView = pnlFields.FindControl("GRD" & row(i).Item("Fieldid").ToString())
                        AddHandler GRD.RowDataBound, AddressOf totalrow
                        AddHandler GRD.RowCommand, AddressOf Delete
                        AddHandler GRD.RowDeleting, AddressOf Deleting

                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        oda.SelectCommand.CommandText = "uspGetDetailITEM"
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        oda.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
                        oda.SelectCommand.Parameters.AddWithValue("FN", PRitem(0).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
                        ds.Tables.Clear()
                        oda.Fill(ds, "ITEM")
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & row(i).Item("dropdown").ToString() & "' AND F1.DOCUMENTTYPE='" & screen & "'"
                        oda.Fill(ds, "TOTAL")
                        If Not IsPostBack Then
                            Session(PRitem(0)) = Nothing
                        End If

                        If Not Session(PRitem(0)) Is Nothing Then
                            ob.BINDITEMGRID1(Session(PRitem(0)), pnlFields, row(i).Item("fieldid"), updatePanelEdit, ds.Tables("TOTAL"))
                        Else
                            'ob.BINDITEMGRID(ds.Tables("ITEM"), pnlFields, btn1.ID, UpdatePanel1, ds.Tables("TOTAL"))
                        End If
                    Next
                    If Not Session("CHILD") Is Nothing Then
                        ob.CreateControlsOnPanel(Session("CHILD"), pnlFields1, updpnlchild, Button2, 0)
                        Dim ROW2() As DataRow = Session("CHILD").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
                        If ROW2.Length > 0 Then
                            For i As Integer = 0 To ROW2.Length - 1
                                Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW2(i).Item("FieldID").ToString()), DropDownList)
                                DDL.AutoPostBack = True
                                AddHandler DDL.TextChanged, AddressOf bindvalue1
                            Next
                        End If
                        ChildFormddlRendering(row, 2)
                    End If
                    con.Close()
                    oda.Dispose()
                    con.Dispose()
                End If
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
        
    End Sub
    Public Shared Function GetPostBackControl(ByVal page As Page) As Control
        Dim control As Control = Nothing

        Dim ctrlname As String = page.Request.Params.[Get]("__EVENTTARGET")
        If ctrlname IsNot Nothing AndAlso ctrlname <> String.Empty Then
            control = page.FindControl(ctrlname)
        Else
            For Each ctl As String In page.Request.Form
                Dim c As Control = page.FindControl(ctl)
                If TypeOf c Is System.Web.UI.WebControls.Button Then
                    control = c
                    Exit For
                End If
            Next
        End If
        Return control
    End Function
    Public Sub ChildFormddlRendering(ByVal row As DataRow(), ByVal ACTION As Integer)
        Dim c As Control = GetPostBackControl(Me.Page)
        '...
        'If c IsNot Nothing Then
        'End If
        If row.Length > 0 Then
            'If TypeOf c Is System.Web.UI.WebControls.Button Then
            '    Dim BTN As Button = TryCast(c, Button)
            '    If Left(BTN.Text.Trim, 3) = "ADD" Then
            '        Dim id As String = Right(c.ID, c.ID.Length - 3)
            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As New SqlConnection(conStr)
            'Dim oda As SqlDataAdapter = New SqlDataAdapter("Select KC_LOGIC from mmm_mst_fields where fieldid=" & ID & "", con)

            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'STRKC = oda.SelectCommand.ExecuteScalar().ToString()

            Dim STRKC As String = ""

            For i As Integer = 0 To row.Length - 1
                Dim FN As String = row(i).Item("FIELDID").ToString
                If FN = Session("ID").ToString.Trim Then
                    STRKC = row(i).Item("KC_Logic").ToString
                    If STRKC <> "" Then
                        Dim FLDS() As String = STRKC.Split("-")
                        If ACTION = 1 Then
                            Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & FLDS(0)), DropDownList)
                            If DDL Is Nothing Then
                                Exit Sub
                            End If
                            Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
                            ddl1.Items.Clear()
                            ddl1.Items.Insert(0, "Select One")
                            ddl1.Enabled = True
                            ddl1.SelectedIndex = 0
                            Dim li As ListItem = DDL.SelectedItem
                            Dim tn As String = li.Text
                            Dim vl As String = li.Value
                            ddl1.Items.Add(tn)
                            ddl1.Items(1).Value = vl
                            updpnlchild.Update()
                        Else
                            Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
                            ddl1.Enabled = False
                        End If
                    End If
                End If
            Next
        End If
    End Sub
    Public Sub bindvalue1(ByVal sender As Object, ByVal e As EventArgs)
        Dim c As Control = GetPostBackControl(Me.Page)
        '...
        If c IsNot Nothing Then
        End If

        Dim ddl As DropDownList = TryCast(c, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlFields1, ddl)

    End Sub
    Public Sub INSERTCHILDITEM(ByVal SENDER As Object, ByVal E As System.EventArgs)
        Dim btnDetails As Button = TryCast(SENDER, Button)
        Dim formname As String = btnDetails.Text
        formname = Right(formname, formname.Length - 8).Trim()
        ViewState("FN") = formname
        Dim ID() As String = btnDetails.ID.Split("-")
        ID(0) = Right(ID(0), ID(0).Length - 3).Trim()
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim ob As New DynamicForm
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim DS As New DataSet
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim ddl As DropDownList = TryCast(pnlFields.FindControl("fld" & ID(0).ToString()), DropDownList)
        Dim docid() As String = ddl.SelectedValue.ToString.Split("|")
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "USP_COPY_CHILDITEM"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
        oda.SelectCommand.Parameters.AddWithValue("FN", formname)
        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("DOCID", docid(0).ToString())
        oda.SelectCommand.ExecuteScalar()


        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "uspGetDetailITEM"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
        oda.SelectCommand.Parameters.AddWithValue("FN", formname)
        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        ds.Tables.Clear()
        oda.Fill(ds, "ITEM")
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        oda.Fill(ds, "TOTAL")
        ob.BINDITEMGRID(DS.Tables("ITEM"), pnlFields, "BTN" & ID(1).ToString(), updatePanelEdit, DS.Tables("TOTAL"))
        oda.Dispose()
        DS.Dispose()
        con.Dispose()
    End Sub
    Protected Sub EditItem(ByVal sender As Object, ByVal e As System.EventArgs)
    End Sub
    Public Sub totalrow(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim cnt As Integer = e.Row.Cells.Count - 1
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.Cells(0).Text.ToUpper() = "TOTAL" Then
                e.Row.Font.Bold = True
                e.Row.ForeColor = Drawing.Color.Black
            Else
                Dim img As ImageButton = New ImageButton()
                img.ID = e.Row.Cells(cnt).Text
                img.ImageUrl = "~/images/Cancel.gif"
                img.CommandName = "Remove"
                img.CommandArgument = e.Row.Cells(cnt).Text
                img.Height = Unit.Parse("16")
                img.Width = Unit.Parse("16")
                e.Row.Cells(cnt).Controls.Add(img)
                e.Row.Cells(cnt).Controls.Add(New LiteralControl("&nbsp;"))
                Dim btnEdit As ImageButton = New ImageButton()
                btnEdit.ID = e.Row.Cells(cnt).Text & "Child"
                btnEdit.CommandName = "Editchild"
                btnEdit.ImageUrl = "~/images/Edit.gif"
                btnEdit.CommandArgument = e.Row.Cells(cnt).Text
                btnEdit.Height = Unit.Parse("16")
                btnEdit.Width = Unit.Parse("16")
                e.Row.Cells(cnt).Controls.Add(btnEdit)
            End If
        ElseIf e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(cnt).Text = " "
        End If

    End Sub

    Public Sub Delete(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim btnDelete As GridView = TryCast(sender, GridView)
        Dim ID As String = btnDelete.ID
        ID = Right(ID, ID.Length - 3)
        Session("ID") = ID
        ID = "BTN" & ID
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim DS As New DataSet
        Dim rw As GridViewRow = DirectCast(DirectCast(e.CommandSource, ImageButton).NamingContainer, GridViewRow)
        'Dim Pid As Integer = rw.RowIndex
        Dim Pid As String() = btnDelete.DataKeys(rw.RowIndex).Value.ToString.Split("-")
        If Pid.Length > 1 Then
            ViewState("ID") = Pid(2)
        End If
        If e.CommandName = "Remove" Then
            Dim dt As DataTable = Session(Pid(0))
            If dt.Rows.Count > 1 Then
                dt.Rows.RemoveAt(dt.Rows.Count - 1)
            End If
            Dim DTVAL As DataTable = Session(Pid(0) & "VAL")
            If dt.Rows.Count > 0 Then
                dt.Rows.RemoveAt(Pid(1))
                DTVAL.Rows.RemoveAt(Pid(1))
                Session(Pid(0)) = dt
                Session(Pid(0) & "VAL") = DTVAL

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & ViewState("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
                oda.Fill(DS, "TOTAL")

                'oda.SelectCommand.CommandText = "Delete from MMM_MST_DOC_ITEM where tid=" & Pid & ""
                'oda.SelectCommand.ExecuteScalar()
                oda.Dispose()
                con.Dispose()
                Dim OB As New DynamicForm
                OB.BINDITEMGRID1(dt, pnlFields, Session("ID"), updatePanelEdit, DS.Tables("TOTAL"))
            Else
                Session(Session(Pid(0))) = Nothing
                Session(Session(Pid(0) & "VAL")) = Nothing
            End If

            'BINDGRID1(dt)
            'BINDGRIDAFTERDELETION(ID)
        ElseIf e.CommandName.ToUpper = "EDITCHILD" Then
            Dim ob As New DynamicForm
            'ob._CallerPage = 1
            oda = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FORMNAME='" & Pid(0).ToString() & "' order by displayOrder", con)
            oda.Fill(DS, "CHILD")
            ViewState("FN") = DS.Tables("CHILD").Rows(0).Item("DOCUMENTTYPE").ToString
            Session("CHILD") = DS.Tables("CHILD")
            pnlFields1.Controls.Clear()
            ob.CreateControlsOnPanel(Session("CHILD"), pnlFields1, updpnlchild, Button2, 0)
            Dim ROW1() As DataRow = DS.Tables("CHILD").Select("fieldtype='DROP DOWN' and (dropdowntype='MASTER VALUED' OR DROPDOWNTYPE='SESSION VALUED') and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue1
                Next
            End If
            FILLCONTROLONEDIT(DS.Tables("CHILD"), Session(Pid(0).ToString), Session(Pid(0) & "VAL"), pnlFields1, updpnlchild, CInt(Pid(1)))
            Button2.Text = "UPDATE"
            updpnlchild.Update()
            ModalPopupExtender1.Show()
        End If
    End Sub
    Protected Sub FILLCONTROLONEDIT(ByVal dtFields As DataTable, ByVal dtflds As DataTable, ByVal dtval As DataTable, ByRef pnlfield As Panel, ByRef updpnl As UpdatePanel, ByVal index As Integer)
        Dim drnewval As DataRow = dtval.NewRow()
        ViewState("Index") = index
        For i As Integer = 0 To dtval.Rows.Count - 1
            If i = index Then
                For j As Integer = 0 To dtFields.Rows.Count - 1
                    Dim dispName As String = dtFields.Rows(i).Item("displayname").ToString()
                    Select Case dtFields.Rows(j).Item("FieldType").ToString().ToUpper()
                        Case "TEXT BOX"
                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dtFields.Rows(j).Item("FieldID").ToString()), TextBox)
                            'drnew.Item(dtFields.Rows(i).Item("displayname")) = txtBox.Text
                            txtBox.Text = dtval.Rows(i).Item(j).ToString()
                        Case "DROP DOWN"
                            Dim ddl As DropDownList = CType(pnlFields.FindControl("fld" & dtFields.Rows(j).Item("FieldID").ToString()), DropDownList)
                            'drnew.Item(dtFields.Rows(i).Item("displayname")) = txtBox.SelectedItem.Text
                            'drnewval.Item(dtFields.Rows(i).Item("displayname")) = txtBox.SelectedItem.Value
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(dtval.Rows(i).Item(j).ToString()))
                        Case "CALCULATIVE FIELD"
                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dtFields.Rows(j).Item("FieldID").ToString()), TextBox)
                            'drnew.Item(dtFields.Rows(i).Item("displayname")) = txtBox.Text
                            txtBox.Text = dtval.Rows(i).Item(j).ToString()
                        Case "LOOKUP"
                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dtFields.Rows(j).Item("FieldID").ToString()), TextBox)
                            'drnew.Item(dtFields.Rows(i).Item("displayname")) = txtBox.Text
                            txtBox.Text = dtval.Rows(i).Item(j).ToString()
                    End Select
                Next
            End If
        Next

    End Sub

    Public Sub Deleting(ByVal Sender As Object, ByVal e As GridViewDeleteEventArgs)
        Dim btnDelete As GridView = TryCast(Sender, GridView)
        btnDelete.DataBind()
    End Sub
    Public Sub ShowChildForm(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim btnDetails As Button = TryCast(sender, Button)
        Dim formname As String = btnDetails.Text
        formname = Right(formname, formname.Length - 5).Trim()
        ViewState("FN") = formname
        Session("ID") = Right(btnDetails.ID, btnDetails.ID.Length - 3)
        Dim ob As New DynamicForm
        If ViewState("ID") <> btnDetails.ID Or ViewState("ID") Is Nothing Or Session("CHILD") Is Nothing Then
            ViewState("ID") = btnDetails.ID
            Dim scrname As String = Request.QueryString("SC").ToString()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & formname & "' order by displayOrder", con)
            Dim DS As New DataSet
            oda.Fill(DS, "CHILD")
            Session("CHILD") = DS.Tables("CHILD")
            ViewState(formname) = DS.Tables("CHILD")
            pnlFields1.Controls.Clear()
            ob.CreateControlsOnPanel(DS.Tables("CHILD"), pnlFields1, updpnlchild, Button2, 0)
            Dim ROW1() As DataRow = DS.Tables("CHILD").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue1
                Next
            End If
            oda.Dispose()
            DS.Dispose()
            con.Dispose()
        End If
        ob.CLEARDYNAMICFIELDS(pnlFields1)
        ChildFormddlRenderingOnCreation(1)
        updpnlchild.Update()
        Button2.Text = "Save"
        ModalPopupExtender1.Show()
    End Sub
    Public Sub ChildFormddlRenderingOnCreation(ByVal ACTION As Integer)
        Dim c As Control = GetPostBackControl(Me.Page)
        '...
        'If c IsNot Nothing Then
        'End If
        If TypeOf c Is System.Web.UI.WebControls.Button Then

            Dim BTN As Button = TryCast(c, Button)
            If Left(BTN.Text.Trim, 3) = "ADD" Then
                Dim id As String = Right(c.ID, c.ID.Length - 3)
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As New SqlConnection(conStr)
                Dim oda As SqlDataAdapter = New SqlDataAdapter("Select KC_LOGIC from mmm_mst_fields where fieldid=" & id & "", con)
                Dim STRKC As String = ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                STRKC = oda.SelectCommand.ExecuteScalar().ToString()

                If STRKC <> "" Then
                    Dim FLDS() As String = STRKC.Split("-")
                    If ACTION = 1 Then
                        Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & FLDS(0)), DropDownList)
                        If DDL Is Nothing Then
                            Exit Sub
                        End If
                        Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
                        ddl1.Items.Clear()
                        ddl1.Items.Insert(0, "Select One")
                        ddl1.Enabled = True
                        ddl1.SelectedIndex = 0
                        Dim li As ListItem = DDL.SelectedItem
                        Dim tn As String = li.Text
                        Dim vl As String = li.Value
                        ddl1.Items.Add(tn)
                        ddl1.Items(1).Value = vl
                        updpnlchild.Update()
                    Else
                        Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
                        ddl1.Enabled = False
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub bindvalue(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlFields, ddl)
    End Sub
    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        If btnActEdit.Text = "Save" Then
            ValidateData("ADD")
        Else
            ValidateData("EDIT")
        End If
    End Sub
    Protected Sub ValidateData(ByVal actionType As String)
        'Check All Validations
        ' now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim screenname As String = Request.QueryString("SC").ToString()
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & screenname & "' order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "fields")
        Dim ob As New DynamicForm
        Dim FinalQry As String
        If actionType = "ADD" Then
            If ds.Tables("fields").Rows(0).Item("layouttype") = "CUSTOM" Then
                FinalQry = ob.ValidateAndGenrateQueryForCustom("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields) & ""
            Else
                FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields, 0) & ""
            End If

        Else
            'pass query of updation and also type
            FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_MASTER SET updateddate=getdate(),", "", ds.Tables("fields"), pnlFields, ViewState("tid"))
        End If

        If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
            lblTab.Text = FinalQry
        Else
            If actionType <> "ADD" Then
                FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
            Else
                FinalQry = FinalQry & ";select @@identity"
            End If
            'save the data
            lblTab.Text = ""
            da.SelectCommand.CommandText = FinalQry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim fileid As Integer = da.SelectCommand.ExecuteScalar()

            If actionType <> "ADD" Then
                fileid = ViewState("tid")
            ElseIf actionType = "ADD" Then
                Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
                If row.Length > 0 Then
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
                    da.SelectCommand.Parameters.AddWithValue("docid", fileid)
                    da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
                    da.SelectCommand.Parameters.AddWithValue("FormType", "Master")
                    Dim an As String = da.SelectCommand.ExecuteScalar()
                    'msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
                    da.SelectCommand.Parameters.Clear()
                End If
            End If
            'INSERT INTO HISTORY 
            ob.History(Session("EID"), fileid, Session("UID"), screenname, "MMM_MST_MASTER", actionType)
            ob.CLEARDYNAMICFIELDS(pnlFields)
            Try
                lblTab.Text = "Master updated successfully."
                Dim scrname As String = ""
                If Request.QueryString.HasKeys() Then
                    If Request.QueryString("SC") <> Nothing Then
                        scrname = Request.QueryString("SC").ToString()
                        Response.Redirect("~/mobile/Masters.aspx?SC=" & scrname & "")
                    End If
                End If
            Catch ex As Exception

            End Try
        End If
        da.Dispose()
        con.Dispose()
    End Sub
End Class
