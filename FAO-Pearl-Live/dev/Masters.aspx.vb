Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports AjaxControlToolkit
Imports System.Web.Services

Partial Class Masters
    Inherits System.Web.UI.Page

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
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Request.QueryString("SC") Is Nothing Then
            Response.Redirect("MainHome.aspx")
        Else
            Dim screen As String = Request.QueryString("SC").ToString()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & screen & "' order by displayOrder", con)

            Dim ds As New DataSet()
            oda.Fill(ds, "fields")

            'bind grid using role assignment
            bindgridbyrole()
            'end


            Dim dtF As New DataTable
            oda.SelectCommand.CommandText = "select formDesc from mmm_mst_forms where EID=" & Session("EID").ToString() & " and FormName='" & screen & "'"
            oda.Fill(dtF)
            Dim Formdesc As String = screen
            If dtF.Rows.Count > 0 Then
                Formdesc = dtF.Rows(0).Item(0).ToString
            End If
            dtF.Dispose()
            Dim messageStrip As String = ""
            Try
                If (Not Session("HEADERSTRIP") Is Nothing) Then
                    messageStrip = Session("HEADERSTRIP").ToString()
                End If
            Catch ex As Exception
            End Try

            If messageStrip.Length > 1 Then
                lblCaption.Text = "<div style=""width:100%;height:47px;padding-top:15px;padding-left:10px;font:bold 17px 'verdana';color:#fff;background-image:url(logo/" & messageStrip & ");background-repeat:no-repeat"">" & Formdesc & "</div>"
            Else
                lblCaption.Text = Formdesc '  ds.Tables("fields").Rows(0).Item("FormDesc").ToString()
            End If
            lblHeaderPopUp.Text = Formdesc 'ds.Tables("fields").Rows(0).Item("FormDesc".ToString())

            Dim ob As New DynamicForm
            ob.CLEARDYNAMICFIELDS(pnlFields)
            ob.CreateControlsOnPanel(ds.Tables("fields"), pnlFields, updatePanelEdit, btnActEdit, 0)
            'Dim ROW1() As DataRow = ds.Tables("fields").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
            Dim ROW1() As DataRow = ds.Tables("fields").Select("fieldtype='DROP DOWN'   and (lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or ddllookupvalueSource is not null or HasRule='1')")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue
                Next
            End If

            ''Child Item 
            'Dim ROWMenu() As DataRow = ds.Tables("fields").Select("(fieldtype='Parent Field' or fieldtype='Self Reference') and isactive='1'")
            'If ROWMenu.Length > 0 Then
            '    For i As Integer = 0 To ROWMenu.Length - 1
            '        Dim menu As Menu = TryCast(pnlFields.FindControl("fld" & ROWMenu(i).Item("FieldID").ToString()), Menu)
            '        AddHandler menu.MenuItemClick, AddressOf MenuChanged
            '    Next
            'End If

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
                    'Dim ROW2() As DataRow = Session("CHILD").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
                    Dim ROW2() As DataRow = Session("CHILD").Select("fieldtype='DROP DOWN'   and (lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or HasRule='1')")
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
    End Sub

    'Public Sub MenuChanged(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim Menu As Menu = TryCast(sender, Menu)
    '    Dim id As String = Right(Menu.ID, Menu.ID.Length - 3)
    '    Dim id1 As Integer = CInt(id)
    '    Dim txtbox As TextBox = TryCast(pnlFields1.FindControl("fld" & id1.ToString() & "RES"), TextBox)
    '    txtbox.Text = Menu.SelectedItem.Text
    'End Sub


    Public Sub bindvalue(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlFields, ddl)
        ob.bindlookupddl(id, pnlFields, ddl)
        ob.bindMultiLookUP(id, pnlFields, ddl)
        ob.bindddlMultiLookUP(id, pnlFields, ddl)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strPreviousPage As String = ""
        If Request.UrlReferrer <> Nothing Then
            strPreviousPage = Request.UrlReferrer.Segments(Request.UrlReferrer.Segments.Length - 1)
        End If
        If strPreviousPage = "" Then
            Response.Redirect("~/Invalidaction.aspx")
        End If

        If Not IsPostBack Then
            ViewState("pageheader") = lblHeaderPopUp.Text.Replace("Add New ", "").Replace("Edit", "")
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim scrname As String = Request.QueryString("SC").ToString()

            lblRecord.Text = ""

            Dim da As New SqlDataAdapter("SELECT displayName,FieldMapping,fieldtype FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 order by displayOrder", con)
            Try
                Dim ds As New DataSet
                'lblCaption.Text = scrname
                da.Fill(ds, "data")
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                    ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
                Next
                lblMsg.Text = ""
                da.SelectCommand.CommandText = "Select count(*) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname.ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim cnt As Integer = da.SelectCommand.ExecuteScalar()

                'Check Map Show button visible code
                Dim dtm As New DataTable
                da.SelectCommand.CommandText = "select fieldtype FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' and fieldtype='Geo Point'"
                da.Fill(dtm)
                If dtm.Rows.Count > 0 Then
                    btnchangeView.Visible = True
                Else
                    btnchangeView.Visible = False
                End If
                lblRecord.Text = "Displaying 0 of total " & cnt & " Records."
                ViewState("recordscnt") = cnt
                '' updated for restricting count to other roles on 20_jan
                If UCase(Session("USERROLE")) = "SU" Then
                    lblRecord.Text = "Displaying 0 of total " & cnt & " Records."
                    ViewState("recordscnt") = cnt
                Else
                    da.SelectCommand.CommandText = "Select tid " & Session("MasterQRY") & " "
                    da.Fill(ds, "cnt")
                    If ds.Tables("cnt").Rows.Count > 0 Then
                        lblRecord.Text = "Displaying 0 of total " & ds.Tables("cnt").Rows.Count & " Records."
                        ViewState("recordscnt") = ds.Tables("cnt").Rows.Count
                    End If
                End If
                Call GetMenuandroles()
                'Calculate Button by Komal on 18March2014
                Dim Formula() As DataRow = ds.Tables("data").Select("fieldtype='Formula Field'")
                If Formula.Length > 0 Then
                    Button1.Visible = True
                Else
                    Button1.Visible = False
                End If
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
            con.Dispose()
            da.Dispose()
        End If
    End Sub

    Private Sub GetMenuandroles()
        Dim screen As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        ' Dim scrname As String = Request.QueryString("SC").ToString()
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        'Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
        da.SelectCommand.CommandText = "select * from mmm_mst_menu where eid=" & Session("EID") & " and pagelink='Masters.aspx?SC=" & screen & "'"
        da.Fill(ds, "menu")
        If ds.Tables("menu").Rows.Count > 0 Then
            Dim rol As String() = ds.Tables("menu").Rows(0).Item("Roles").ToString().Split(",")

            For j As Integer = 0 To rol.Length - 1
                Dim a As String() = rol(j).ToString().Split(":")
                If Session("USERROLE") = a(0).Remove(0, 1).ToString() Then
                    ViewState("numval") = a(1).Remove(a(1).Length - 1)
                End If
            Next
            If ViewState("numval") = 1 Then
                btnimmm.Visible = False
                btnNew.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btne.Visible = False
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 2 Then
                gvData.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btne.Visible = False
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 3 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btne.Visible = False
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 4 Then
                btnimmm.Visible = False
                btnNew.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 5 Then
                btnimmm.Visible = False
                btnNew.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 6 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 7 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btnl As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnDtl"), ImageButton)
                    btnl.Visible = False
                Next
            ElseIf ViewState("numval") = 8 Then
                btnimmm.Visible = False
                btnNew.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 9 Then
                btnimmm.Visible = False
                btnNew.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 10 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 11 Then
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 12 Then
                btnimmm.Visible = False
                btnNew.Visible = False
            ElseIf ViewState("numval") = 13 Then
                btnimmm.Visible = False
                btnNew.Visible = False
            ElseIf ViewState("numval") = 14 Then

            ElseIf ViewState("numval") = 15 Then

            End If
        End If
        con.Close()
        da.Dispose()
    End Sub
    'Private Sub GetMenuData()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim scrname As String = Request.QueryString("SC").ToString()
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet
    '    Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
    '    da.SelectCommand.CommandText = "select tid,menuname,menutype, " & cr & " from mmm_mst_accessmenu where menutype='dynamic' and menuname='Master'"
    '    da.Fill(ds, "menu")
    '    For i As Integer = 0 To ds.Tables("menu").Rows.Count - 1

    '        Dim abc As String = ds.Tables("menu").Rows(i).Item(cr).ToString()
    '        Dim a1 As String() = abc.ToString().Split(",")
    '        For c As Integer = 0 To a1.Length - 1
    '            Dim b1 As String() = a1(c).ToString().Split("-")

    '            If b1(0).ToString = scrname Then
    '                Dim ab As String() = b1(1).ToString().Split(":")
    '                If ab(0).Length > 0 Then
    '                    ViewState("numval") = ab(1).ToString()
    '                End If
    '            End If
    '        Next

    '    Next
    '    con.Close()
    '    da.Dispose()
    'End Sub

    'Public Sub ShowChildForm(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim btnDetails As Button = TryCast(sender, Button)
    '    Dim formname As String = btnDetails.Text
    '    formname = Right(formname, formname.Length - 5)
    '    ViewState("FN") = formname
    '    Session("ID") = Right(btnDetails.ID, btnDetails.ID.Length - 3)
    '    Dim ob As New DynamicForm
    '    If ViewState("ID") <> btnDetails.ID Or ViewState("ID") Is Nothing Or Session("CHILD") Is Nothing Then
    '        ViewState("ID") = btnDetails.ID
    '        Dim scrname As String = Request.QueryString("SC").ToString()
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & formname & "' order by displayOrder", con)
    '        Dim DS As New DataSet
    '        oda.Fill(DS, "CHILD")
    '        Session("CHILD") = DS.Tables("CHILD")
    '        pnlFields1.Controls.Clear()
    '        ob.CreateControlsOnPanel(DS.Tables("CHILD"), pnlFields1, updpnlchild, Button2, 0)
    '        Dim ROW1() As DataRow = DS.Tables("CHILD").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
    '        If ROW1.Length > 0 Then
    '            For i As Integer = 0 To ROW1.Length - 1
    '                Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
    '                DDL.AutoPostBack = True
    '                AddHandler DDL.TextChanged, AddressOf bindvalue1
    '            Next
    '        End If
    '        'Dim ROW2() As DataRow = DS.Tables("CHILD").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and kc_logic is not null")
    '        'If ROW2.Length > 0 Then
    '        '    For i As Integer = 0 To ROW2.Length - 1
    '        '        'parentchild(ROW2(i).Item("FIELDID"), CInt(ROW2(i).Item("KC_LOGIC").ToString()))
    '        '    Next
    '        'End If
    '        ChildFormddlRenderingOnCreation(1)
    '        oda.Dispose()
    '        DS.Dispose()
    '        con.Dispose()
    '    End If
    '    'ob.CLEARDYNAMICFIELDS(pnlFields1)
    '    updpnlchild.Update()
    '    ModalPopupExtender1.Show()
    'End Sub

    Protected Sub ValidateChildData(ByVal actionType As String)
        'Check All Validations
        ' now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim screenname As String = ViewState("FN")
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and ff.fieldtype<>'Child Item' and F.EID=" & Session("EID").ToString() & " and FormName = '" & screenname & "' order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "fields")

        Dim ob As New DynamicForm
        Dim FinalQry As String
        If actionType = "ADD" Then
            If ds.Tables("fields").Rows(0).Item("layouttype") = "CUSTOM" Then
                FinalQry = ob.ValidateAndGenrateQueryForCustom("ADD", "INSERT INTO MMM_MST_DOC_ITEM(SESSIONID,documenttype,", "VALUES (" & Session.SessionID & ",'" & screenname & "',", ds.Tables("fields"), pnlFields1)
            Else
                FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_DOC_ITEM(SESSIONID,documenttype,", "VALUES ('" & Session.SessionID & "','" & screenname & "',", ds.Tables("fields"), pnlFields1, 0)
            End If

        Else
            'pass query of updation and also type
            FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC_ITEM SET updateddate=getdate(),", "", ds.Tables("fields"), pnlFields1, ViewState("tid"))
        End If

        If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
            lblTab.Text = FinalQry
        Else
            If actionType <> "ADD" Then
                FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
            End If
            'save the data
            lblTab.Text = ""
            da.SelectCommand.CommandText = FinalQry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim DOCID As Integer = da.SelectCommand.ExecuteNonQuery()
            BINDGRID()
            Session("CHILD") = Nothing
            ModalPopupExtender1.Hide()
        End If
        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub btnchangeView_Click(sender As Object, e As EventArgs) Handles btnchangeView.Click
        Dim screen As String = Request.QueryString("SC").ToString()
        Session("DocumentName") = screen
        Response.Redirect("VB.aspx?DocName=" + screen)

        'Response.Redirect("Default2.aspx")
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
        DS.Tables.Clear()
        oda.Fill(DS, "ITEM")
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        oda.Fill(DS, "TOTAL")
        ob.BINDITEMGRID(DS.Tables("ITEM"), pnlFields, "BTN" & ID(1).ToString(), updatePanelEdit, DS.Tables("TOTAL"))
        oda.Dispose()
        DS.Dispose()
        con.Dispose()
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

    Public Sub Deleting(ByVal Sender As Object, ByVal e As GridViewDeleteEventArgs)
        Dim btnDelete As GridView = TryCast(Sender, GridView)
        btnDelete.DataBind()
    End Sub

    Protected Sub BINDGRIDAFTERDELETION(ByVal ID As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim DS As New DataSet
        ODA.SelectCommand.Parameters.Clear()
        ODA.SelectCommand.CommandText = "uspGetDetailITEM"
        ODA.SelectCommand.CommandType = CommandType.StoredProcedure
        ODA.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
        ODA.SelectCommand.Parameters.AddWithValue("FN", ViewState("FN"))
        ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        DS.Tables.Clear()
        ODA.Fill(DS, "ITEM")
        Dim dt_item As DataTable = New DataTable()
        dt_item = DS.Tables("ITEM")
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & ViewState("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        ODA.Fill(DS, "TOTAL")
        ODA.Dispose()
        DS.Dispose()
        Dim OB As New DynamicForm
        OB.BINDITEMGRID(dt_item, pnlFields, ID, updatePanelEdit, DS.Tables("TOTAL"))
    End Sub

    Protected Sub BINDGRID()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim DS As New DataSet
        ODA.SelectCommand.Parameters.Clear()
        ODA.SelectCommand.CommandText = "uspGetDetailITEM"
        ODA.SelectCommand.CommandType = CommandType.StoredProcedure
        ODA.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
        ODA.SelectCommand.Parameters.AddWithValue("FN", ViewState("FN"))
        ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        DS.Tables.Clear()
        ODA.Fill(DS, "ITEM")
        Dim dt_item As DataTable = New DataTable()
        dt_item = DS.Tables("ITEM")
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & ViewState("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        ODA.Fill(DS, "TOTAL")
        ODA.Dispose()
        DS.Dispose()
        Dim OB As New DynamicForm
        OB.BINDITEMGRID(dt_item, pnlFields, ViewState("ID"), updatePanelEdit, DS.Tables("TOTAL"))
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

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("tid") = pid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select FF.* from MMM_MST_FIELDS FF inner join MMM_MST_MASTER M on FF.DocumentType =M.DocumentType   where FF.EID=" & Session("EID") & " AND M.tid =" & pid & " and FF.isactive=1  order by displayOrder", con)
        Dim ds As New DataSet()
        Session("EDITonEDIT") = "EDITonEDIT"
        oda.Fill(ds, "fields")
        Dim ob As New DynamicForm
        ob.FillControlsOnPanel(ds.Tables("fields"), pnlFields, "MASTER", pid)
        btnActEdit.Text = "Update"
        lblHeaderPopUp.Text = "Edit " & Replace(lblHeaderPopUp.Text.Replace("Edit ", ""), "Add New ", "")
        updPanalHeader.Update()
        updatePanelEdit.Update()
        btnEdit_ModalPopupExtender.Show()
        lblTab.Text = ""
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblTab.Text = ""
        btnActEdit.Text = "Save"
        lblHeaderPopUp.Text = "Add New " & Replace(lblHeaderPopUp.Text.Replace("Edit ", ""), "Add New ", "")
        Session("EDITonEDIT") = ""
        Dim ob As New DynamicForm()
        ' ob.CLEARDYNAMICFIELDS(pnlFields)
        updPanalHeader.Update()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        'Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        'Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        'ViewState("pid") = pid
        'lblMsgDelete.Text = "Are you Sure Want to delete this Group? " & row.Cells(2).Text
        'Me.updatePanelDelete.Update()
        'Me.btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        'validation for null entry
        If btnActEdit.Text = "Save" Then
            ValidateData("ADD")
        Else
            ValidateData("EDIT")
        End If
    End Sub

    'Protected Sub ValidateData(ByVal actionType As String)
    '    'Check All Validations
    '    ' now validation of created controls
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim screenname As String = Request.QueryString("SC").ToString()
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & screenname & "' order by displayOrder", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "fields")
    '    Dim ob As New DynamicForm
    '    Dim FinalQry As String
    '    If actionType = "ADD" Then
    '        If ds.Tables("fields").Rows(0).Item("layouttype") = "CUSTOM" Then
    '            FinalQry = ob.ValidateAndGenrateQueryForCustom("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields) & ""
    '        Else
    '            FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields, 0) & ""
    '        End If

    '    Else
    '        'pass query of updation and also type
    '        FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_MASTER SET updateddate=getdate(),", "", ds.Tables("fields"), pnlFields, ViewState("tid"))
    '    End If

    '    If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
    '        lblTab.Text = FinalQry
    '    Else
    '        If actionType <> "ADD" Then
    '            FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
    '        Else
    '            FinalQry = FinalQry & ";select @@identity"
    '        End If
    '        'save the data
    '        lblTab.Text = ""
    '        da.SelectCommand.CommandText = FinalQry
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim fileid As Integer = da.SelectCommand.ExecuteScalar()

    '        If actionType <> "ADD" Then
    '            fileid = ViewState("tid")
    '        ElseIf actionType = "ADD" Then
    '            Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
    '            If row.Length > 0 Then
    '                da.SelectCommand.Parameters.Clear()
    '                da.SelectCommand.CommandText = "usp_GetAutoNoNew"
    '                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
    '                da.SelectCommand.Parameters.AddWithValue("docid", fileid)
    '                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
    '                da.SelectCommand.Parameters.AddWithValue("FormType", "Master")
    '                Dim an As String = da.SelectCommand.ExecuteScalar()
    '                'msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
    '                da.SelectCommand.Parameters.Clear()
    '            End If
    '        End If




    '        'Added By Komal for Formula

    '        Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
    '        Dim viewdoc As String = screenname
    '        viewdoc = viewdoc.Replace(" ", "_")
    '        If CalculativeField.Length > 0 Then
    '            For Each CField As DataRow In CalculativeField
    '                Dim formulaeditorr As New formulaEditor
    '                Dim forvalue As String = String.Empty
    '                forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), fileid, "v" + Session("eid").ToString + viewdoc, Session("EID"), 0)
    '                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & fileid & ""
    '                da.SelectCommand.CommandText = upquery
    '                da.SelectCommand.CommandType = CommandType.Text
    '                da.SelectCommand.ExecuteNonQuery()
    '            Next
    '        End If


    '        'INSERT INTO HISTORY 
    '        ob.History(Session("EID"), fileid, Session("UID"), screenname, "MMM_MST_MASTER", actionType)
    '        gvData.DataBind()
    '        ob.CLEARDYNAMICFIELDS(pnlFields)
    '        btnEdit_ModalPopupExtender.Hide()
    '    End If
    '    da.Dispose()
    '    con.Dispose()
    '    updPnlGrid.Update()
    'End Sub

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
        Dim trans As SqlTransaction = Nothing
        Dim updateorcreate = "Create"
        Dim Message = "Master created successfully."
        Try
            Dim dv As DataView = ds.Tables("fields").DefaultView
            dv.RowFilter = "IsActive=1"
            Dim theFields As DataTable = dv.ToTable
            Dim lstData As New List(Of UserData)
            'Creating collection for rule engine execution
            Dim obj As New DynamicForm()
            lstData = obj.CreateCollection(pnlFields, theFields)
            'Setting it to session for getting it's value for child Item validation
            Session("pColl") = lstData
            If actionType = "ADD" Then
                If ds.Tables("fields").Rows(0).Item("layouttype") = "CUSTOM" Then
                    FinalQry = ob.ValidateAndGenrateQueryForCustom("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields) & ""
                    updateorcreate = "Create"
                    Message = "Master created successfully."
                Else
                    FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields, 0) & ""
                    updateorcreate = "Create"
                    Message = "Master updated successfully."
                End If
            Else
                'pass query of updation and also type
                updateorcreate = "Update"
                Message = "Master Updated successfully."
                FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_MASTER SET updateddate=getdate(),", "", ds.Tables("fields"), pnlFields, ViewState("tid"))
            End If

            If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
                lblTab.Text = FinalQry
                btnEdit_ModalPopupExtender.Show()
            Else
                'Code For Rule Engine By Ajeet Kumar Dated :30-july-2014

                ''Creating object of rule response
                'Dim ObjRet As New RuleResponse()
                ''Initialising rule Object
                'Dim ObjRule As New RuleEngin(Session("EID"), screenname, "CREATED", "SUBMIT")
                ''Uncomment
                'ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False)
                'If ObjRet.Success = False Then
                '    lblTab.Text = ObjRet.Message
                '    Exit Sub
                'End If

                'Creating object of rule response
                Dim ObjRet As New RuleResponse()
                'Initialising rule Object
                Dim ObjRule As New RuleEngin(Session("EID"), screenname, "CREATED", "SUBMIT")
                'Uncomment
                Dim dsrule As DataSet = ObjRule.GetRules()
                Dim dtrule As New DataTable
                dtrule = dsrule.Tables(1)

                Dim errorlist As New ArrayList
                For Each dr As DataRow In dsrule.Tables(0).Rows
                    ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False, screenname, dr, dtrule)
                    If ObjRet.Success = False Then
                        errorlist.Add(Convert.ToString(ObjRet.ErrorMessage))
                    End If
                Next
                If errorlist.Count > 0 Then
                    lblTab.Text = String.Join("<br/>", errorlist.ToArray())
                    lblTab.ForeColor = Color.Red
                    Exit Sub
                End If


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
                trans = con.BeginTransaction()
                da.SelectCommand.Transaction = trans
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
                'Added By Komal for Formula
                Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
                Dim viewdoc As String = screenname
                viewdoc = viewdoc.Replace(" ", "_")
                If CalculativeField.Length > 0 Then
                    For Each CField As DataRow In CalculativeField
                        Dim formulaeditorr As New formulaEditor
                        Dim forvalue As String = String.Empty
                        forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), fileid, "v" + Session("eid").ToString + viewdoc, Session("EID"), 0, con, trans)
                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & fileid & ""
                        da.SelectCommand.CommandText = upquery
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.ExecuteNonQuery()
                    Next
                End If
                'INSERT INTO HISTORY 
                If screenname.ToUpper() = "USER" Then
                    ob.HistoryT(Session("EID"), fileid, Session("UID"), screenname, "MMM_MST_USER", actionType, con, trans)
                Else
                    ob.HistoryT(Session("EID"), fileid, Session("UID"), screenname, "MMM_MST_MASTER", actionType, con, trans)
                End If

                Trigger.ExecuteTriggerT(screenname, Session("EID"), fileid, con, trans, 0, TriggerNature:=updateorcreate, FormType:="MASTER")
                'This block added by Ajeet Kumar:: Dated:10th november 2014 
                Dim eid As Integer, CreatedDocType As String, DOCID As Integer, UID As Integer
                Try
                    Dim objRel As New Relation()
                    If actionType = "ADD" Then

                        eid = Convert.ToUInt32(Session("EID"))
                        UID = Convert.ToUInt32(Session("UID"))
                        Dim objRes As New RelationResponse()
                        CreatedDocType = screenname
                        DOCID = fileid
                        objRes = objRel.ExtendRelation(eid, CreatedDocType, DOCID, UID, "", False)
                        'In case of costom rule Creation

                        da.SelectCommand.CommandText = "usp_AutoEntryInRoleAssignment"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@ScreenName", screenname)
                        da.SelectCommand.Parameters.AddWithValue("@DocMappedId", fileid)
                        da.SelectCommand.ExecuteNonQuery()

                        da.SelectCommand.Parameters.Clear()
                        If objRes.Success = True And objRes.ShowExtend = True Then
                            Response.Redirect("ExtendRelation.aspx?DOCID=" & DOCID & "&SC=" & CreatedDocType, False)
                        End If
                    End If
                Catch ex As Exception
                End Try

                trans.Commit()

                'This Code block is added by Ajeet On:23/07/2015 For AutoDocument creation
                Try
                    Dim objSh As New ScheduleDocument(Session("EID"), screenname, "Save")
                    Dim res = objSh.Execute(DOCID)
                Catch ex As Exception

                End Try
                'Update site csv 

                Dim objCsv As New UpdateSiteCsv()
                objCsv.UpdateCsv(fileid, Convert.ToInt32(Session("Eid")), screenname)

                'Update site csv End

                gvData.DataBind()
                ob.CLEARDYNAMICFIELDS(pnlFields)
                btnEdit_ModalPopupExtender.Hide()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('" & Message & "!!');", True)
            End If
            updPnlGrid.Update()
        Catch ex As Exception
            If Not trans Is Nothing Then
                trans.Rollback()
            End If
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Error occured at server!!');", True)
        Finally
            If Not con Is Nothing Then
                con.Dispose()
                da.Dispose()
            End If
            If Not trans Is Nothing Then
                trans.Dispose()
            End If
        End Try

    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim pid As String = ViewState("pid").ToString()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteMasterVal", con)
        'oda.SelectCommand.CommandType = CommandType.StoredProcedure
        'oda.SelectCommand.Parameters.AddWithValue("gid", pid)
        'If con.State <> ConnectionState.Open Then
        '    con.Open()
        'End If
        'Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        'con.Close()
        'oda.Dispose()
        'con.Dispose()
        'If iSt = 1 Then
        '    gvData.DataBind()
        '    lblRecord.Text = "Data has been deleted successfully"
        '    updatePanelDelete.Update()
        '    btnDelete_ModalPopupExtender.Hide()

        'Else
        '    lblMsgDelete.Text = "This Data can Not be Deleted"
        '    updatePanelDelete.Update()
        'End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        lblMsg.Text = ""

        'If Trim(txtValue.Text).Length < 1 Then
        '    lblMsg.Text = "* Please enter value to search!!!"
        '    Exit Sub
        'End If

        gvData.DataBind()


    End Sub
    'Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
    '    Try

    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim scrname As String = Request.QueryString("SC").ToString()
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '        Dim icnt As Integer

    '        Dim i As Integer = Session("EID")
    '        'Dim sql As String = "insert into mmm_mst_fields () values ()"
    '        Dim ds As New DataSet
    '        'Dim cmdInsert As New SqlCommand(sql, con)
    '        'cmdInsert.ExecuteNonQuery()


    '        'Dim row As New DataRow
    '        Dim da As New SqlDataAdapter("select * from MMM_MST_FIELDS where EID=" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 ", con)
    '        da.Fill(ds, "data")
    '        Dim c As Integer = ds.Tables("data").Rows.Count
    '        Dim adapter As New SqlDataAdapter
    '        Dim sb As New System.Text.StringBuilder()
    '        Dim sh As New System.Text.StringBuilder()

    '        Dim errs As String = ""
    '        If impfile.HasFile Then
    '            ViewState("imprt_cnt") += 1
    '            If Right(impfile.FileName, 4).ToUpper() = ".CSV" Then
    '                Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(impfile.FileName, 4).ToUpper()
    '                impfile.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
    '                Dim ir As Integer = 0
    '                Dim sField As String()
    '                Dim csvReader As Microsoft.VisualBasic.FileIO.TextFieldParser
    '                csvReader = My.Computer.FileSystem.OpenTextFieldParser(Server.MapPath("Import/") & filename, ",")
    '                'bno = "5" & ViewState("imprt_cnt") & (Now.Hour * 24) + (Now.Minute * 60)
    '                Dim st As String = ""
    '                Dim ic As Integer = 0
    '                Dim vk As String = ""
    '                Dim isuni As String = ""
    '                Dim ddty As String = ""
    '                Dim c1 As String = ""
    '                Dim mv As String = ""
    '                Dim dn As String = ""
    '                Dim dd As String = ""
    '                Dim minl As Integer = 0
    '                Dim maxl As Integer = 0
    '                Dim fm As String = ""
    '                With csvReader
    '                    .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
    '                    .Delimiters = New String() {","}
    '                    While Not .EndOfData
    '                        sField = .ReadFields()

    '                        If icnt < 1 Then

    '                            sb.Append("Insert Into MMM_MST_MASTER (eid, documenttype,createdby,updateddate,")
    '                            For k As Integer = 0 To c - 1
    '                                If UCase(sField(k)) <> UCase(Trim(ds.Tables("data").Rows(k).Item("displayname").ToString())) Then
    '                                    lblMsg.Text = "Not Found"
    '                                Else

    '                                    st = ds.Tables("data").Rows(k).Item("FieldMapping").ToString()
    '                                    sb.Append(st)
    '                                    If k = c - 1 Then
    '                                        sb.Append(") values (")
    '                                        Exit For
    '                                    Else
    '                                        sb.Append(", ")
    '                                    End If
    '                                End If
    '                            Next
    '                            icnt += 1
    '                            Continue While
    '                        End If

    '                        icnt += 1
    '                        Dim v As String = ""
    '                        v &= Session("EID")
    '                        v &= ","
    '                        v &= "'" & scrname & "'"
    '                        'sb.Append(Session("EID"))
    '                        'sb.Append(",")
    '                        'sb.Append("'" & scrname & "'")
    '                        v &= "," ' sb.Append(",")
    '                        v &= "'" & Session("UID") & "'" ' sb.Append(Session("UID"))
    '                        v &= "," 'sb.Append(",")
    '                        v &= "'" & DateAndTime.Now & "'" 'sb.Append("'" & DateAndTime.Now & "'")
    '                        v &= "," ' sb.Append(",")


    '                        For j As Integer = 0 To c - 1
    '                            mv = ds.Tables("data").Rows(j).Item("dropdowntype").ToString()
    '                            dn = ds.Tables("data").Rows(j).Item("displayname").ToString()
    '                            dd = ds.Tables("data").Rows(j).Item("dropdown").ToString()
    '                            vk = ds.Tables("data").Rows(j).Item("isrequired").ToString()
    '                            ddty = ds.Tables("data").Rows(j).Item("datatype").ToString()
    '                            isuni = ds.Tables("data").Rows(j).Item("isunique").ToString()
    '                            minl = ds.Tables("data").Rows(j).Item("minlen").ToString()
    '                            maxl = ds.Tables("data").Rows(j).Item("maxlen").ToString()
    '                            fm = ds.Tables("data").Rows(j).Item("FieldMapping").ToString()
    '                            If UCase(mv) = UCase("Master Valued") Then
    '                                If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- Please enter TID of corresponding Master instead of Text  )" & " </tr> </table>"
    '                                    Continue While
    '                                ElseIf vk = 1 And Trim(sField(j)) = "" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & " </tr> </table>"
    '                                    Continue While
    '                                End If
    '                            Else
    '                                If vk = 1 And Trim(sField(j)) = "" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                ElseIf ddty = "Numeric" Then
    '                                    If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values! you entered " & sField(j) & ")" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And IsNumeric(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values! you entered " & sField(j) & ")" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf minl <> Trim(0) And maxl <> Trim(0) Then
    '                                        If Trim(sField(j).Length) < minl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        ElseIf Trim(sField(j).Length) > maxl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If
    '                                        If isuni = 1 Then
    '                                            da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
    '                                            Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
    '                                            If cnttt >= 1 Then
    '                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                                Continue While
    '                                            End If
    '                                        End If
    '                                    End If
    '                                ElseIf ddty = "Datetime" Then
    '                                    If vk = 1 And Vaildatedate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And Vaildatedate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf isuni = 1 Then
    '                                        da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
    '                                        Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
    '                                        If cnttt >= 1 Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If
    '                                    End If
    '                                ElseIf ddty = "Text" Then
    '                                    If minl <> Trim(0) And maxl <> Trim(0) Then
    '                                        If Trim(sField(j).Length) < minl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        ElseIf Trim(sField(j).Length) > maxl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If
    '                                    End If
    '                                    If isuni = 1 Then

    '                                        da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
    '                                        If con.State <> ConnectionState.Open Then
    '                                            con.Open()
    '                                        End If
    '                                        Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
    '                                        If cnttt >= 1 Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If

    '                                    End If
    '                                End If

    '                            End If
    '                            If dd.Contains("-") Then
    '                                If sField(j) <> "" Then

    '                                    Dim b1 As String() = dd.ToString().Split("-")

    '                                    If b1(0).Contains("STATIC") Then
    '                                        da.SelectCommand.CommandText = "select count(uid) from mmm_mst_user where eid=" & Session("EID") & " and uid=" & sField(j) & ""
    '                                    Else
    '                                        ViewState("dd1") = b1(1).ToString()
    '                                        da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & ViewState("dd1") & "' and tid=" & sField(j) & ""
    '                                    End If

    '                                    If con.State <> ConnectionState.Open Then
    '                                        con.Open()
    '                                    End If
    '                                    Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
    '                                    If cntt < "1" Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- This tid  " & sField(j) & " does not exists)" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    Else
    '                                        If sField(j).Contains("'") Then
    '                                            sField(j) = Replace(sField(j), "'", "''")
    '                                        End If
    '                                        v &= "'"
    '                                        v &= Trim(sField(j))
    '                                    End If
    '                                Else
    '                                    v &= "'"
    '                                End If

    '                            Else
    '                                If sField(j).Contains("'") Then
    '                                    sField(j) = Replace(sField(j), "'", "''")
    '                                End If
    '                                v &= "'" 'sb.Append("'")
    '                                If ddty = "Datetime" Then
    '                                    v &= getdate(Trim(sField(j)))
    '                                Else
    '                                    v &= Trim(sField(j))
    '                                End If

    '                            End If
    '                            ' sb.Append(sField(j))
    '                            v &= "'" 'sb.Append("'")
    '                            If j = c - 1 Then
    '                                v &= ")"   ' sb.Append(")")
    '                                Exit For
    '                            Else
    '                                v &= "," 'sb.Append(", ")
    '                            End If
    '                        Next
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Replace(sb.ToString(), "{", "")
    '                        Replace(sb.ToString(), "}", "")
    '                        sh.Append(sb)
    '                        sh.Append(v)
    '                        adapter.InsertCommand = New SqlCommand(sh.ToString(), con)
    '                        adapter.InsertCommand.ExecuteNonQuery()
    '                        ic += 1

    '                        adapter.Dispose()
    '                        sh.Clear()

    '                    End While
    '                    gvData.DataBind()
    '                    gvexport.DataBind()
    '                    updPnlGrid.UpdateMode = UpdatePanelUpdateMode.Conditional
    '                    updPnlGrid.Update()
    '                    modalstatus.Show()
    '                    lblstat.Text = "Out of <font color=""Green"">" & icnt - 1 & "</font>, <font color=""Green""> " & ic & " </font> Successfully Imported  "
    '                    ViewState("c1") = c1
    '                    If ViewState("c1") = "" Then
    '                        lblstatus1.Text = ""
    '                    Else
    '                        Label2.Text = "Data which are not uploaded due to Errors are given below: "
    '                        lblstatus1.Text = "" & c1 & " "
    '                        lblstatus1.ForeColor = Color.Red
    '                    End If
    '                    con.Close()
    '                End With
    '            Else
    '                lblMsg.Text = "File should be of CSV Format"
    '                Exit Sub
    '            End If
    '        Else
    '            lblMsg.Text = "Please select a File to Upload"
    '            Exit Sub
    '        End If

    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured while importing data. Please try again"
    '    End Try
    'End Sub


    '' dt 19_aug for checkbox list control...
    'Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
    '    Try

    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim scrname As String = Request.QueryString("SC").ToString()
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '        Dim icnt As Integer

    '        Dim i As Integer = Session("EID")
    '        'Dim sql As String = "insert into mmm_mst_fields () values ()"
    '        Dim ds As New DataSet
    '        'Dim cmdInsert As New SqlCommand(sql, con)
    '        'cmdInsert.ExecuteNonQuery()



    '        'Dim row As New DataRow
    '        Dim da As New SqlDataAdapter("select * from MMM_MST_FIELDS where EID=" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 ", con)
    '        da.Fill(ds, "data")
    '        Dim c As Integer = ds.Tables("data").Rows.Count
    '        Dim adapter As New SqlDataAdapter
    '        Dim sb As New System.Text.StringBuilder()
    '        Dim sh As New System.Text.StringBuilder()



    '        'below code added on 24 June, by Vinay for checking unikey keys functionalities on multiple columns



    '        'da.SelectCommand.CommandText = "SELECT  items FROM [dbo].[Split] ((select UniqueKeys from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & scrname.ToString() & "'), ',') "
    '        da.SelectCommand.CommandText = "select UniqueKeys from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & scrname.ToString() & "'"
    '        da.Fill(ds, "UK")

    '        'The End

    '        Dim errs As String = ""
    '        If impfile.HasFile Then
    '            ViewState("imprt_cnt") += 1
    '            If Right(impfile.FileName, 4).ToUpper() = ".CSV" Then
    '                Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(impfile.FileName, 4).ToUpper()
    '                impfile.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
    '                Dim ir As Integer = 0
    '                Dim sField As String()
    '                Dim csvReader As Microsoft.VisualBasic.FileIO.TextFieldParser
    '                csvReader = My.Computer.FileSystem.OpenTextFieldParser(Server.MapPath("Import/") & filename, ",")
    '                'bno = "5" & ViewState("imprt_cnt") & (Now.Hour * 24) + (Now.Minute * 60)
    '                Dim st As String = ""
    '                Dim ic As Integer = 0
    '                Dim vk As String = ""
    '                Dim isuni As String = ""
    '                Dim ddty As String = ""
    '                Dim c1 As String = ""
    '                Dim mv As String = ""
    '                Dim dn As String = ""
    '                Dim dd As String = ""
    '                Dim minl As Integer = 0
    '                Dim maxl As Integer = 0
    '                Dim fm As String = ""
    '                With csvReader
    '                    .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
    '                    .Delimiters = New String() {","}
    '                    While Not .EndOfData
    '                        sField = .ReadFields()

    '                        If icnt < 1 Then

    '                            sb.Append("Insert Into MMM_MST_MASTER (eid, documenttype,createdby,updateddate,")
    '                            For k As Integer = 0 To c - 1
    '                                If UCase(sField(k)) <> UCase(Trim(ds.Tables("data").Rows(k).Item("displayname").ToString())) Then
    '                                    lblMsg.Text = "Not Found"
    '                                Else
    '                                    st = ds.Tables("data").Rows(k).Item("FieldMapping").ToString()
    '                                    sb.Append(st)
    '                                    If k = c - 1 Then
    '                                        sb.Append(") values (")
    '                                        Exit For
    '                                    Else
    '                                        sb.Append(", ")
    '                                    End If
    '                                End If
    '                            Next
    '                            icnt += 1
    '                            Continue While
    '                        End If

    '                        icnt += 1
    '                        Dim v As String = ""
    '                        v &= Session("EID")
    '                        v &= ","
    '                        v &= "'" & scrname & "'"
    '                        'sb.Append(Session("EID"))
    '                        'sb.Append(",")
    '                        'sb.Append("'" & scrname & "'")
    '                        v &= "," ' sb.Append(",")
    '                        v &= "'" & Session("UID") & "'" ' sb.Append(Session("UID"))
    '                        v &= "," 'sb.Append(",")
    '                        v &= "'" & DateAndTime.Now & "'" 'sb.Append("'" & DateAndTime.Now & "'")
    '                        v &= "," ' sb.Append(",")

    '                        Dim dds As String = String.Empty
    '                        Dim saz() As String
    '                        Dim ge As Integer = 0
    '                        For j As Integer = 0 To c - 1
    '                            mv = ds.Tables("data").Rows(j).Item("dropdowntype").ToString()
    '                            dn = ds.Tables("data").Rows(j).Item("displayname").ToString()
    '                            dd = ds.Tables("data").Rows(j).Item("dropdown").ToString()
    '                            vk = ds.Tables("data").Rows(j).Item("isrequired").ToString()
    '                            ddty = ds.Tables("data").Rows(j).Item("datatype").ToString()
    '                            isuni = ds.Tables("data").Rows(j).Item("isunique").ToString()
    '                            minl = ds.Tables("data").Rows(j).Item("minlen").ToString()
    '                            maxl = ds.Tables("data").Rows(j).Item("maxlen").ToString()
    '                            fm = ds.Tables("data").Rows(j).Item("FieldMapping").ToString()

    '                            'code added by vinay on 24th june verifying for columns which are unique keys
    '                            If ds.Tables("UK").Rows.Count > 0 Then
    '                                saz = ds.Tables("UK").Rows(0).Item("uniquekeys").ToString().Split(",")

    '                                For l As Integer = 0 To saz.Length - 1
    '                                    If fm = saz(l).ToString() Then
    '                                        dds = dds & saz(l).ToString() & "='" & sField(j).ToString() & "' and "
    '                                        ge = ge + 1
    '                                    End If
    '                                Next

    '                            End If
    '                            'end of the code

    '                            If UCase(mv) = UCase("Master Valued") Then
    '                                If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- Please enter TID of corresponding Master instead of Text  )" & " </tr> </table>"
    '                                    Continue While
    '                                ElseIf vk = 1 And Trim(sField(j)) = "" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & " </tr> </table>"
    '                                    Continue While
    '                                End If
    '                            Else
    '                                If vk = 1 And Trim(sField(j)) = "" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                ElseIf ddty = "Numeric" Then
    '                                    If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values! you entered " & sField(j) & ")" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And IsNumeric(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values! you entered " & sField(j) & ")" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf minl <> Trim(0) And maxl <> Trim(0) Then
    '                                        If Trim(sField(j).Length) < minl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        ElseIf Trim(sField(j).Length) > maxl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If
    '                                        If isuni = 1 Then
    '                                            da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
    '                                            Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
    '                                            If cnttt >= 1 Then
    '                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                                Continue While
    '                                            End If
    '                                        End If
    '                                    End If
    '                                ElseIf ddty = "Datetime" Then
    '                                    If vk = 1 And Vaildatedate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And Vaildatedate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf isuni = 1 Then
    '                                        da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
    '                                        Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
    '                                        If cnttt >= 1 Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If
    '                                    End If
    '                                ElseIf ddty = "Text" Then
    '                                    If minl <> Trim(0) And maxl <> Trim(0) Then
    '                                        If Trim(sField(j).Length) < minl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        ElseIf Trim(sField(j).Length) > maxl Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If
    '                                    End If
    '                                    If isuni = 1 Then

    '                                        da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
    '                                        If con.State <> ConnectionState.Open Then
    '                                            con.Open()
    '                                        End If
    '                                        Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
    '                                        If cnttt >= 1 Then
    '                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                            Continue While
    '                                        End If

    '                                    End If
    '                                End If

    '                            End If

    '                            'code on 25th june by vinay
    '                            If ge = ds.Tables("UK").Rows(0).Item("uniquekeys").ToString().Split(",").Length Then
    '                                '  dds = dds.Remove(dds.Length - 4)
    '                                ge = 0
    '                                da.SelectCommand.CommandText = "select count(*) from mmm_mst_master where eid=" & Session("EID") & " and " & dds.ToString() & " documenttype='" & scrname.ToString() & "'"
    '                                If con.State <> ConnectionState.Open Then
    '                                    con.Open()
    '                                End If
    '                                Dim cntdt As Integer = da.SelectCommand.ExecuteScalar()

    '                                If cntdt > 0 Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- Duplicacy on Unique key  " & sField(j) & " not allowed)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                End If
    '                            End If

    '                            'The End

    '                            If dd.Contains("-") Then
    '                                If sField(j) <> "" Then

    '                                    Dim b1 As String() = dd.ToString().Split("-")

    '                                    If b1(0).Contains("STATIC") Then
    '                                        da.SelectCommand.CommandText = "select count(uid) from mmm_mst_user where eid=" & Session("EID") & " and uid=" & sField(j) & ""
    '                                    Else
    '                                        ViewState("dd1") = b1(1).ToString()
    '                                        da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & ViewState("dd1") & "' and tid=" & sField(j) & ""
    '                                    End If

    '                                    If con.State <> ConnectionState.Open Then
    '                                        con.Open()
    '                                    End If
    '                                    Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
    '                                    If cntt < "1" Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- This tid  " & sField(j) & " does not exists)" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    Else
    '                                        If sField(j).Contains("'") Then
    '                                            sField(j) = Replace(sField(j), "'", "''")
    '                                        End If
    '                                        v &= "'"
    '                                        v &= Trim(sField(j))
    '                                    End If
    '                                Else
    '                                    v &= "'"
    '                                End If

    '                            Else
    '                                If sField(j).Contains("'") Then
    '                                    sField(j) = Replace(sField(j), "'", "''")
    '                                End If
    '                                v &= "'" 'sb.Append("'")
    '                                If ddty = "Datetime" Then
    '                                    v &= getdate(Trim(sField(j)))
    '                                Else
    '                                    v &= Trim(sField(j))
    '                                End If

    '                            End If
    '                            ' sb.Append(sField(j))
    '                            v &= "'" 'sb.Append("'")
    '                            If j = c - 1 Then
    '                                v &= ")"   ' sb.Append(")")
    '                                Exit For
    '                            Else
    '                                v &= "," 'sb.Append(", ")
    '                            End If
    '                        Next
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Replace(sb.ToString(), "{", "")
    '                        Replace(sb.ToString(), "}", "")
    '                        sh.Append(sb)
    '                        sh.Append(v)
    '                        adapter.InsertCommand = New SqlCommand(sh.ToString(), con)
    '                        adapter.InsertCommand.ExecuteNonQuery()
    '                        ic += 1

    '                        adapter.Dispose()
    '                        sh.Clear()

    '                    End While
    '                    gvData.DataBind()
    '                    gvexport.DataBind()
    '                    updPnlGrid.UpdateMode = UpdatePanelUpdateMode.Conditional
    '                    updPnlGrid.Update()
    '                    modalstatus.Show()
    '                    lblstat.Text = "Out of <font color=""Green"">" & icnt - 1 & "</font>, <font color=""Green""> " & ic & " </font> Successfully Imported  "
    '                    ViewState("c1") = c1
    '                    If ViewState("c1") = "" Then
    '                        lblstatus1.Text = ""
    '                    Else
    '                        Label2.Text = "Data which are not uploaded due to Errors are given below: "
    '                        lblstatus1.Text = "" & c1 & " "
    '                        lblstatus1.ForeColor = Color.Red
    '                    End If
    '                    con.Close()
    '                End With
    '            Else
    '                lblMsg.Text = "File should be of CSV Format"
    '                Exit Sub
    '            End If
    '        Else
    '            lblMsg.Text = "Please select a File to Upload"
    '            Exit Sub
    '        End If

    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured while importing data. Please try again"
    '    End Try
    'End Sub

    Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
        Try

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim scrname As String = Request.QueryString("SC").ToString()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim icnt As Integer

            Dim i As Integer = Session("EID")
            'Dim sql As String = "insert into mmm_mst_fields () values ()"
            Dim ds As New DataSet
            'Dim cmdInsert As New SqlCommand(sql, con)
            'cmdInsert.ExecuteNonQuery()



            'Dim row As New DataRow
            Dim da As New SqlDataAdapter("select * from MMM_MST_FIELDS where EID=" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 ", con)
            da.Fill(ds, "data")
            Dim c As Integer = ds.Tables("data").Rows.Count
            Dim adapter As New SqlDataAdapter
            Dim sb As New System.Text.StringBuilder()
            Dim sh As New System.Text.StringBuilder()



            'below code added on 24 June, by Vinay for checking unikey keys functionalities on multiple columns



            'da.SelectCommand.CommandText = "SELECT  items FROM [dbo].[Split] ((select UniqueKeys from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & scrname.ToString() & "'), ',') "
            da.SelectCommand.CommandText = "select UniqueKeys from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & scrname.ToString() & "'"
            da.Fill(ds, "UK")

            'The End

            Dim errs As String = ""
            If impfile.HasFile Then
                ViewState("imprt_cnt") += 1
                If Right(impfile.FileName, 4).ToUpper() = ".CSV" Then
                    Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(impfile.FileName, 4).ToUpper()
                    impfile.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                    Dim ir As Integer = 0
                    Dim sField As String()
                    Dim csvReader As Microsoft.VisualBasic.FileIO.TextFieldParser
                    csvReader = My.Computer.FileSystem.OpenTextFieldParser(Server.MapPath("Import/") & filename, ",")
                    'bno = "5" & ViewState("imprt_cnt") & (Now.Hour * 24) + (Now.Minute * 60)
                    Dim st As String = ""
                    Dim ic As Integer = 0
                    Dim vk As String = ""
                    Dim isuni As String = ""
                    Dim ddty As String = ""
                    Dim c1 As String = ""
                    Dim mv As String = ""
                    Dim dn As String = ""
                    Dim dd As String = ""
                    Dim minl As Integer = 0
                    Dim maxl As Integer = 0
                    Dim fm As String = ""
                    Dim ftype As String = ""
                    With csvReader
                        .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
                        .Delimiters = New String() {","}
                        While Not .EndOfData
                            sField = .ReadFields()

                            If icnt < 1 Then

                                sb.Append("Insert Into MMM_MST_MASTER (eid, documenttype,createdby,updateddate,")
                                For k As Integer = 0 To c - 1
                                    If UCase(sField(k)) <> UCase(Trim(ds.Tables("data").Rows(k).Item("displayname").ToString())) Then
                                        lblMsg.Text = "Not Found"
                                    Else
                                        st = ds.Tables("data").Rows(k).Item("FieldMapping").ToString()
                                        sb.Append(st)
                                        If k = c - 1 Then
                                            sb.Append(") values (")
                                            Exit For
                                        Else
                                            sb.Append(", ")
                                        End If
                                    End If
                                Next
                                icnt += 1
                                Continue While
                            End If

                            icnt += 1
                            Dim v As String = ""
                            v &= Session("EID")
                            v &= ","
                            v &= "'" & scrname & "'"
                            'sb.Append(Session("EID"))
                            'sb.Append(",")
                            'sb.Append("'" & scrname & "'")
                            v &= "," ' sb.Append(",")
                            v &= "'" & Session("UID") & "'" ' sb.Append(Session("UID"))
                            v &= "," 'sb.Append(",")
                            v &= "'" & DateAndTime.Now & "'" 'sb.Append("'" & DateAndTime.Now & "'")
                            v &= "," ' sb.Append(",")

                            Dim dds As String = String.Empty
                            Dim saz() As String
                            Dim ge As Integer = 0
                            For j As Integer = 0 To c - 1
                                mv = ds.Tables("data").Rows(j).Item("dropdowntype").ToString()
                                dn = ds.Tables("data").Rows(j).Item("displayname").ToString()
                                dd = ds.Tables("data").Rows(j).Item("dropdown").ToString()
                                vk = ds.Tables("data").Rows(j).Item("isrequired").ToString()
                                ddty = ds.Tables("data").Rows(j).Item("datatype").ToString()
                                isuni = ds.Tables("data").Rows(j).Item("isunique").ToString()
                                minl = ds.Tables("data").Rows(j).Item("minlen").ToString()
                                maxl = ds.Tables("data").Rows(j).Item("maxlen").ToString()
                                fm = ds.Tables("data").Rows(j).Item("FieldMapping").ToString()
                                ftype = ds.Tables("data").Rows(j).Item("Fieldtype").ToString()
                                'code added by vinay on 24th june verifying for columns which are unique keys
                                If ds.Tables("UK").Rows.Count > 0 Then
                                    saz = ds.Tables("UK").Rows(0).Item("uniquekeys").ToString().Split(",")

                                    For l As Integer = 0 To saz.Length - 1
                                        If fm = saz(l).ToString() Then
                                            dds = dds & saz(l).ToString() & "='" & sField(j).ToString() & "' and "
                                            ge = ge + 1
                                        End If
                                    Next

                                End If
                                'end of the code

                                If UCase(mv) = UCase("Master Valued") Then
                                    If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- Please enter TID of corresponding Master instead of Text  )" & " </tr> </table>"
                                        Continue While
                                    ElseIf vk = 1 And Trim(sField(j)) = "" Then
                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & " </tr> </table>"
                                        Continue While
                                    End If
                                Else
                                    If vk = 1 And Trim(sField(j)) = "" Then
                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err empty column)" & "</td>" & " </tr> </table>"
                                        Continue While
                                    ElseIf ddty = "Numeric" Then
                                        If vk = 1 And IsNumeric(Trim(sField(j))) = False Then
                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values! you entered " & sField(j) & ")" & " </tr> </table>"
                                            Continue While
                                        ElseIf vk = 0 And IsNumeric(Trim(sField(j))) = False Then
                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-only numeric values! you entered " & sField(j) & ")" & " </tr> </table>"
                                            Continue While
                                        ElseIf minl <> Trim(0) And maxl <> Trim(0) Then
                                            If Trim(sField(j).Length) < minl Then
                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                Continue While
                                            ElseIf Trim(sField(j).Length) > maxl Then
                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                Continue While
                                            End If
                                            If isuni = 1 Then
                                                da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
                                                da.SelectCommand.CommandTimeout = 5000
                                                Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
                                                If cnttt >= 1 Then
                                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                    Continue While
                                                End If
                                            End If
                                        End If
                                    ElseIf ddty = "Datetime" Then
                                        If vk = 1 And Vaildatedate(Trim(sField(j))) = False Then
                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                            Continue While
                                        ElseIf vk = 0 And Vaildatedate(Trim(sField(j))) = False Then
                                            c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                            Continue While
                                        ElseIf isuni = 1 Then
                                            da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
                                            da.SelectCommand.CommandTimeout = 5000
                                            Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
                                            If cnttt >= 1 Then
                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                Continue While
                                            End If
                                        End If
                                    ElseIf ddty = "Text" Then
                                        If minl <> Trim(0) And maxl <> Trim(0) Then
                                            If Trim(sField(j).Length) < minl Then
                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                Continue While
                                            ElseIf Trim(sField(j).Length) > maxl Then
                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Plz check character Length " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                Continue While
                                            End If
                                        End If
                                        If isuni = 1 Then

                                            da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname & "' and " & fm & "='" & sField(j) & "' "
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            da.SelectCommand.CommandTimeout = 5000
                                            Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
                                            If cnttt >= 1 Then
                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Duplicate Entry. " & sField(j) & ")" & "</td>" & " </tr> </table>"
                                                Continue While
                                            End If

                                        End If
                                    End If

                                End If

                                'code on 25th june by vinay
                                If ge = ds.Tables("UK").Rows(0).Item("uniquekeys").ToString().Split(",").Length Then
                                    '  dds = dds.Remove(dds.Length - 4)
                                    ge = 0
                                    da.SelectCommand.CommandText = "select count(*) from mmm_mst_master where eid=" & Session("EID") & " and " & dds.ToString() & " documenttype='" & scrname.ToString() & "'"
                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If
                                    da.SelectCommand.CommandTimeout = 5000
                                    Dim cntdt As Integer = da.SelectCommand.ExecuteScalar()

                                    If cntdt > 0 Then
                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- Duplicacy on Unique key  " & sField(j) & " not allowed)" & "</td>" & " </tr> </table>"
                                        Continue While
                                    End If
                                End If

                                'The End

                                If dd.Contains("-") Then
                                    If sField(j) <> "" Then

                                        Dim b1 As String() = dd.ToString().Split("-")

                                        If b1(0).Contains("STATIC") Then
                                            da.SelectCommand.CommandText = "select count(uid) from mmm_mst_user where eid=" & Session("EID") & " and uid=" & sField(j) & ""
                                        Else
                                            ViewState("dd1") = b1(1).ToString()
                                            If UCase(mv) = UCase("Master Valued") Then
                                                If UCase(ftype).ToString() = "CHECKBOX LIST" Then

                                                    Dim asz As String() = sField(j).ToString.Split(",")
                                                    For n As Integer = 0 To asz.Length - 1
                                                        If asz(n).Length > 1 Then
                                                            da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & ViewState("dd1") & "' and tid=" & asz(n) & ""
                                                            If con.State <> ConnectionState.Open Then
                                                                con.Open()
                                                            End If
                                                            da.SelectCommand.CommandTimeout = 5000
                                                            Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
                                                            If cntt < "1" Then
                                                                c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- This tid  " & sField(j) & " does not exists)" & "</td>" & " </tr> </table>"
                                                                Continue While
                                                            End If
                                                        End If

                                                    Next
                                                End If

                                                If sField(j).Contains("'") Then
                                                    sField(j) = Replace(sField(j), "'", "''")
                                                End If
                                                v &= "'"
                                                v &= Trim(sField(j))
                                            Else
                                                da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & ViewState("dd1") & "' and tid=" & sField(j) & ""
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                da.SelectCommand.CommandTimeout = 5000
                                                Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
                                                If cntt < "1" Then
                                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- This tid  " & sField(j) & " does not exists)" & "</td>" & " </tr> </table>"
                                                    Continue While
                                                Else
                                                    If sField(j).Contains("'") Then
                                                        sField(j) = Replace(sField(j), "'", "''")
                                                    End If
                                                    v &= "'"
                                                    v &= Trim(sField(j))
                                                End If
                                            End If



                                            'code added for multiple tids

                                        End If


                                    Else
                                        v &= "'"
                                    End If

                                Else
                                    If sField(j).Contains("'") Then
                                        sField(j) = Replace(sField(j), "'", "''")
                                    End If
                                    v &= "'" 'sb.Append("'")
                                    If ddty = "Datetime" Then
                                        v &= getdate(Trim(sField(j)))
                                    Else
                                        v &= Trim(sField(j))
                                    End If

                                End If
                                ' sb.Append(sField(j))
                                v &= "'" 'sb.Append("'")
                                If j = c - 1 Then
                                    v &= ")"   ' sb.Append(")")
                                    Exit For
                                Else
                                    v &= "," 'sb.Append(", ")
                                End If
                            Next
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Replace(sb.ToString(), "{", "")
                            Replace(sb.ToString(), "}", "")
                            sh.Append(sb)
                            sh.Append(v)
                            adapter.InsertCommand = New SqlCommand(sh.ToString(), con)
                            adapter.InsertCommand.CommandTimeout = 5000
                            adapter.InsertCommand.ExecuteNonQuery()
                            ic += 1

                            adapter.Dispose()
                            sh.Clear()

                        End While

                        gvData.DataBind()
                        gvexport.DataBind()
                        updPnlGrid.UpdateMode = UpdatePanelUpdateMode.Conditional
                        updPnlGrid.Update()
                        modalstatus.Show()
                        lblstat.Text = "Out of <font color=""Green"">" & icnt - 1 & "</font>, <font color=""Green""> " & ic & " </font> Successfully Imported  "
                        ViewState("c1") = c1
                        If ViewState("c1") = "" Then
                            lblstatus1.Text = ""
                        Else
                            Label2.Text = "Data which are not uploaded due to Errors are given below: "
                            lblstatus1.Text = "" & c1 & " "
                            lblstatus1.ForeColor = Color.Red
                        End If
                        con.Close()
                    End With
                Else
                    lblMsg.Text = "File should be of CSV Format"
                    Exit Sub
                End If
            Else
                lblMsg.Text = "Please select a File to Upload"
                Exit Sub
            End If

        Catch ex As Exception
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Text = "An error occured while importing data. Please try again"
        End Try
    End Sub

    Public Shared Function Vaildatedate(ByVal dbt As String) As Boolean

        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        Try

            If dtArr.Length = 3 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

    End Function
    Public Shared Function getdate(ByVal dbt As String) As String
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.Length = 3 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            Dim dt As String = String.Empty
            Dim ab As String = dd & "/" & mm & "/" & yy
            Try
                dt = (ab.ToString())
                Return dt
            Catch ex As Exception
                Return ""
            End Try
        Else
            Return ""
        End If
    End Function

    'Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    '    ' Verifies that the control is rendered
    'End Sub

    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Lid") = pid
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("Select isauth from MMM_MST_MASTER where eid=" & Session("EID") & " and tid=" & ViewState("Lid") & "", con)
        'oda.SelectCommand.CommandType = CommandType.Text
        'oda.SelectCommand.Parameters.AddWithValue("tid", ViewState("Lid"))
        'Dim ds As New DataSet()
        'oda.Fill(ds, "auth")
        If row.Cells(1).Text.ToUpper().Trim() = "ACTIVE" Then
            lblLock.Text = "<b>Please click the option hereunder to confirm if you wish to lock the """ & ViewState("pageheader").ToString() & """ record - """ & row.Cells(3).Text & " ""</b>"
            btnLockupdate.Text = "Lock"
        Else
            lblLock.Text = "<b>Please click the option hereunder to confirm if you wish to Unlock the """ & ViewState("pageheader").ToString() & """ record - """ & row.Cells(3).Text & " ""</b>"
            btnLockupdate.Text = "Unlock"
        End If
        'con.Close()
        'con.Dispose()
        'oda.Dispose()
        Me.updLock.Update()
        Me.ModalPopup_Lock.Show()
    End Sub

    Protected Sub LockRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspLockMaster", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("tid", ViewState("Lid"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Or iSt = 1 Then
            gvData.DataBind()
            lblRecord.Text = "Updated  successfully"


            'Update CSV
            Dim objcsv As New UpdateSiteCsv()
            objcsv.UpdateCsv(Convert.ToInt32(ViewState("Lid")), Convert.ToInt32(Session("Eid")), Request.QueryString("SC"))
            'Update CSV End

            ModalPopup_Lock.Hide()
        Else
            lblLock.Text = "Not updated"
        End If
        updLock.Update()
        updPnlGrid.Update()
    End Sub

    Protected Sub popUpClose()
        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)
        updPnlGrid.Update()
    End Sub

    Protected Sub gvData_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvData.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Or e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(0).Visible = False
        End If

    End Sub

    Protected Sub btnimmm_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimmm.Click
        modalpopupimport.Show()
    End Sub

    Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable

        Dim returnTable As New DataTable()
        If columnX = "" Then
            columnX = table.Columns(0).ColumnName
        End If

        Dim columnXValues As New List(Of String)()

        For Each dr As DataRow In table.Rows
            Dim columnXTemp As String = dr(columnX).ToString()
            If Not columnXValues.Contains(columnXTemp) Then
                columnXValues.Add(columnXTemp)
                returnTable.Columns.Add(columnXTemp)
            End If
        Next
        If nullValue <> "" Then
            For Each dr As DataRow In returnTable.Rows
                For Each dc As DataColumn In returnTable.Columns
                    If dr(dc.ColumnName).ToString() = "" Then
                        dr(dc.ColumnName) = nullValue
                    End If
                Next
            Next
        End If
        Return returnTable

    End Function

    Protected Sub helpexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles helpexport.Click
        Try
            Response.Clear()
            Response.Buffer = True


            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product
            Dim ds As New DataSet
            Dim scrname As String = Request.QueryString("SC").ToString()
            Response.AddHeader("content-disposition",
              "attachment;filename=" & scrname & ".xls")
            Dim da As New SqlDataAdapter("SELECT displayName[Display Name],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 ", con)
            Dim query As String = "SELECT displayName,case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 "
            Dim cmd As SqlCommand = New SqlCommand(query, con)
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            Dim dt As DataTable = New DataTable()
            Dim dt2 As DataTable = New DataTable()
            Dim dt3 As DataTable = New DataTable()

            dt.Load(dr)
            da.Fill(ds, "data")
            dt3 = ds.Tables("data")
            dt2 = GetInversedDataTable(dt, "displayname", "")

            Dim gvex As New GridView()
            dt2.Rows.Add()

            gvex.AllowPaging = False
            gvex.DataSource = dt2
            gvex.DataBind()
            Dim gvexx As New GridView()
            gvexx.AllowPaging = False
            gvexx.DataSource = dt3

            gvexx.DataBind()
            Response.Clear()
            Response.Buffer = True
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            Dim tb As New Table()
            Dim tr1 As New TableRow()
            Dim cell1 As New TableCell()
            cell1.Controls.Add(gvex)
            tr1.Cells.Add(cell1)
            Dim cell3 As New TableCell()
            cell3.Controls.Add(gvexx)
            Dim cell2 As New TableCell()
            cell2.Text = "&nbsp;"

            Dim tr2 As New TableRow()
            tr2.Cells.Add(cell2)
            Dim tr3 As New TableRow()
            tr3.Cells.Add(cell3)
            tb.Rows.Add(tr1)
            tb.Rows.Add(tr2)
            tb.Rows.Add(tr3)

            tb.RenderControl(hw)

            'style to format numbers to string 
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()

        Catch ex As Exception
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Text = "An error occured when Downloading data. Please try again"
        End Try
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

    Protected Sub EditItem(ByVal sender As Object, ByVal e As System.EventArgs)
        'If Button2.Text = "Save" Then
        '    ValidateChildData("ADD")
        'Else
        '    ValidateChildData("EDIT")
        'End If
        If Button2.Text = "Save" Then
            ADDITEMTOGRID(ViewState("FN"))
        Else
            SavingChildItemOnEdit(ViewState("FN"))
        End If
    End Sub

    Protected Sub ADDITEMTOGRID(ByVal FORMNAME As String)
        Dim dtFD As New DataTable
        Dim dtField As New DataTable
        Dim DTVALUE As New DataTable
        Dim errormsg As String = "Please Enter "
        dtField = ViewState(FORMNAME)
        Dim OB As New DynamicForm()
        '' OB.ADDITEMTOGRID(dtField, FORMNAME, pnlFields1)
        If Session(FORMNAME) Is Nothing Then
            For Each dr As DataRow In dtField.Rows
                dtFD.Columns.Add(dr.Item("displayname"), GetType(String))
                DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
            Next
            dtFD.Columns.Add("tid", GetType(String))
        Else
            dtFD = Session(FORMNAME)
            DTVALUE = Session(FORMNAME & "VAL")
        End If
        Dim drnew As DataRow = dtFD.NewRow()
        Dim DRNEWVAL As DataRow = DTVALUE.NewRow()
        For Each dr As DataRow In dtField.Rows
            Dim dispName As String = dr.Item("displayname").ToString()
            Select Case dr.Item("FieldType").ToString().ToUpper()
                Case "TEXT BOX"
                    Dim txtBox As TextBox = CType(pnlFields1.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.Text.Trim.Length < 1 Then
                        errormsg &= dispName & ","

                    End If
                    If dr.Item("datatype") = "Datetime" Then
                        Dim str1 As String() = Split(txtBox.Text, "/")
                        If str1.Length = 3 Then
                            Dim strDate1 As String = str1(1) & "/" & str1(0) & "/" & str1(2)
                            txtBox.Text = strDate1

                        Else
                            errormsg &= "Date is not in correct format at " & dispName & ","
                            Continue For
                        End If
                        If Not IsDate(txtBox.Text) Then
                            errormsg &= "Date is not in correct format at " & dispName & ","
                            Continue For
                        Else
                            txtBox.Text = Format(Convert.ToDateTime(txtBox.Text.ToString), "dd/MM/yy")
                            Dim str As String() = Split(txtBox.Text, "/")
                            Dim strDate As String = str(0).PadLeft(2, "0") & "/" & str(1).PadLeft(2, "0") & "/" & str(2).PadLeft(2, "0")
                            txtBox.Text = strDate
                        End If
                    End If
                    If txtBox.Text.Length < CInt(dr.Item("minlen").ToString) And txtBox.Text.Length > 0 And dr.Item("datatype").ToString.ToUpper <> "DATETIME" Then
                        errormsg &= "Minimum  " & dr.Item("minlen").ToString() & " character in " & dispName & ","
                        Continue For
                    End If
                    If txtBox.Text.Length > CInt(dr.Item("maxlen").ToString) And txtBox.Text.Length > 0 And dr.Item("datatype").ToString.ToUpper <> "DATETIME" Then
                        errormsg &= "Maximum  " & dr.Item("maxlen").ToString() & " character in " & dispName & ","
                        Continue For
                    End If
                    If dr.Item("isunique").ToString() = "1" Then
                        If OB.checkduplicate("ADD", 0, dr.Item("DBTABLENAME").ToString, dr.Item("Fieldmapping").ToString(), txtBox.Text, dr.Item("DOCUMENTTYPE").ToString) Then
                            errormsg &= "unique " & dispName & " ,"
                            Exit For
                        End If
                    End If
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text

                Case "DROP DOWN"
                    Dim txtBox As DropDownList = CType(pnlFields1.FindControl("fld" & dr.Item("FieldID").ToString()), DropDownList)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.ToUpper = "SELECT" Then
                        errormsg &= dispName & ","
                        Continue For
                    End If
                    If UCase(dr.Item("dropdowntype").ToString()) = "FIX VALUED" Then
                        drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                        DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
                    Else
                        'Dim fldpair() As String = txtBox.SelectedValue.ToString().Split("|")
                        If dr.Item("lookupvalue").ToString().Length > 2 Then
                            Dim fldpair() As String = txtBox.SelectedValue.ToString().Split("|")
                            'dataField &= "'" & fldpair(0).ToString() & "',"
                            drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                            DRNEWVAL.Item(dr.Item("displayname")) = fldpair(0).ToString()
                        Else
                            'dataField &= "'" & txtBox.SelectedValue.ToString() & "',"
                            drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                            DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
                        End If
                    End If
                Case "CALCULATIVE FIELD"
                    Dim txtBox As TextBox = CType(pnlFields1.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                        errormsg &= dispName & ","
                        Continue For
                    End If
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
                Case "LOOKUP"
                    Dim txtBox As TextBox = CType(pnlFields1.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                        errormsg &= dispName & ","
                        Continue For
                    End If
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
                Case "CHECKBOX LIST"
                    Dim txtBox As CheckBoxList = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), CheckBoxList)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                        errormsg &= dispName & ","
                        Continue For
                    End If
                    Dim livalue As String = ""
                    Dim litext As String = ""
                    If UCase(dr.Item("dropdowntype").ToString()) = "FIX VALUED" Then
                        For Each li As ListItem In txtBox.Items
                            If li.Selected Then
                                livalue &= li.Text & ","
                            End If
                        Next
                        livalue = Left(livalue, livalue.Length - 1)
                        'dataField &= "'" & livalue & "',"
                        drnew.Item(dr.Item("displayname")) = livalue
                        DRNEWVAL.Item(dr.Item("displayname")) = livalue
                    Else
                        For Each li As ListItem In txtBox.Items
                            If li.Selected Then
                                livalue &= li.Value & ","
                                litext &= li.Text & ","
                            End If
                        Next
                        livalue = Left(livalue, livalue.Length - 1)
                        drnew.Item(dr.Item("displayname")) = litext
                        DRNEWVAL.Item(dr.Item("displayname")) = livalue
                    End If

                Case "LIST BOX"
                    Dim txtBox As ListBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), ListBox)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                        errormsg &= dispName & ","
                        Continue For
                    End If
                    Dim livalue As String = ""
                    Dim litext As String = ""
                    If UCase(dr.Item("dropdowntype").ToString()) = "FIX VALUED" Then
                        For Each li As ListItem In txtBox.Items
                            If li.Selected Then
                                livalue &= li.Text & ","
                            End If
                        Next
                        livalue = Left(livalue, livalue.Length - 1)
                        drnew.Item(dr.Item("displayname")) = livalue
                        DRNEWVAL.Item(dr.Item("displayname")) = livalue
                    Else
                        For Each li As ListItem In txtBox.Items
                            If li.Selected Then
                                livalue &= li.Value & ","
                                litext &= li.Text & ","
                            End If
                        Next
                        livalue = Left(livalue, livalue.Length - 1)
                        drnew.Item(dr.Item("displayname")) = litext
                        DRNEWVAL.Item(dr.Item("displayname")) = livalue
                    End If
                Case "TEXT AREA"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    If dr.Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                        errormsg &= dispName & ","
                        Continue For
                    End If
                    drnew.Item(dr.Item("displayname")) = OB.getSafeString(txtBox.Text)
                    DRNEWVAL.Item(dr.Item("displayname")) = OB.getSafeString(txtBox.Text)
                    'qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                    'dataField &= "'" & getSafeString(txtBox.Text) & "',"

                Case "FILE UPLOADER"
                    Dim txtBox As FileUpload = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), FileUpload)
                    If dr.Item("isrequired").ToString() = 1 Then
                        If txtBox.HasFile Then
                            Dim FN As String = ""
                            Dim ext As String = ""
                            FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                            ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))
                            'qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            'dataField &= "'" & HttpContext.Current.Session("EID").ToString() & "/" & OB.getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & "" & ext & "',"
                            drnew.Item(dr.Item("displayname")) = txtBox.FileName
                            DRNEWVAL.Item(dr.Item("displayname")) = Session("EID").ToString() & "/" & OB.getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & "" & ext & "'"
                            txtBox.SaveAs(HttpContext.Current.Server.MapPath("DOCS/") & HttpContext.Current.Session("EID").ToString() & "/" & OB.getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & ext)
                        Else
                            errormsg &= dispName & ","
                            Continue For
                        End If
                    Else
                        If txtBox.HasFile Then
                            Dim FN As String = ""
                            Dim ext As String = ""
                            FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                            ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))
                            drnew.Item(dr.Item("displayname")) = txtBox.FileName
                            DRNEWVAL.Item(dr.Item("displayname")) = Session("EID").ToString() & "/" & OB.getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & "" & ext & "'"
                            txtBox.SaveAs(HttpContext.Current.Server.MapPath("DOCS/") & HttpContext.Current.Session("EID").ToString() & "/" & OB.getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & ext)
                        Else
                        End If
                    End If

                Case "CHILD ITEM TOTAL"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text

                    'Case "SELF REFERENCE"
                    '    Dim txtBox As Menu = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), Menu)

                    '    If txtBox.SelectedValue = "0" Then
                    '    Else
                    '        drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                    '        DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
                    '    End If


                    'Case "PARENT FIELD"
                    '    Dim txtBox As Menu = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), Menu)

                    '    If txtBox.SelectedValue = "0" Then
                    '    Else
                    '        drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                    '        DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
                    '    End If
            End Select
        Next

        ''Check Field level Properties
        If errormsg.Length > 15 Then
            Label3.Text = errormsg
            Exit Sub
        End If

        ''Check Form Level Validation
        If dtField.Rows.Count > 0 Then
            Dim str As String = OB.validateForm(dtField.Rows(0).Item("Documenttype").ToString, Session("EID"), pnlFields, dtField, "ADD", 0)
            If str.Length > 5 Then
                str = "Please " & str
                Label3.Text = str
                Exit Sub
            End If
        End If

        '' Remove the Total Row from Datatable
        If dtFD.Rows.Count > 1 Then
            dtFD.Rows.RemoveAt(dtFD.Rows.Count - 1)
        End If
        drnew.Item("tid") = FORMNAME & "-" & dtFD.Rows.Count & "-" & ViewState("ID")
        dtFD.Rows.Add(drnew)
        DTVALUE.Rows.Add(DRNEWVAL)
        Session(FORMNAME) = dtFD
        Session(FORMNAME & "VAL") = DTVALUE
        BINDGRID1(dtFD)
        ModalPopupExtender1.Hide()
    End Sub

    Protected Sub BINDGRID1(ByVal DT As DataTable)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim DS As New DataSet
        'ODA.SelectCommand.Parameters.Clear()
        'ODA.SelectCommand.CommandText = "uspGetDetailITEM"
        'ODA.SelectCommand.CommandType = CommandType.StoredProcedure
        'ODA.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
        'ODA.SelectCommand.Parameters.AddWithValue("FN", ViewState("FN"))
        'ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        'DS.Tables.Clear()
        'ODA.Fill(DS, "ITEM")
        Dim dt_item As DataTable = New DataTable()
        dt_item = DT
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & ViewState("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        ODA.Fill(DS, "TOTAL")
        ODA.Dispose()
        DS.Dispose()
        Dim OB As New DynamicForm
        OB.BINDITEMGRID(dt_item, pnlFields, ViewState("ID"), updatePanelEdit, DS.Tables("TOTAL"))
    End Sub

    Protected Sub SavingChildItem(ByVal formname As String, ByVal docid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtField As New DataTable
        Dim updquery As String = ""
        Dim ob As New DynamicForm()
        If Session(formname & "VAL") Is Nothing Then
            Exit Sub
        End If
        dtField = ViewState(formname)
        Dim dtvalue As DataTable = Session(formname & "VAL")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        For i As Integer = 0 To dtvalue.Rows.Count - 1
            Dim str As String = "INSERT INTO MMM_MST_DOC_ITEM("
            Dim STRfLD As String = ""
            Dim STRVAL As String = ""
            updquery = ""
            For j As Integer = 0 To dtvalue.Columns.Count - 1
                STRfLD &= dtField.Rows(j).Item("fieldmapping").ToString & ","
                STRVAL &= "'" & dtvalue.Rows(i).Item(j).ToString() & "'" & ","
                If dtField.Rows(j).Item("KC_VALUE").ToString.Length > 5 And dtField.Rows(j).Item("KC_STATUS").ToString.Length = 0 Then
                    updquery &= ob.UPDATEKICKING(dtField.Rows(j).Item("KC_VALUE").ToString(), dtvalue.Rows(i).Item(j).ToString(), pnlFields1)
                End If
            Next
            STRfLD &= "DOCID,documenttype,isauth)"
            STRVAL &= docid & "," & "'" & formname & "'" & ",1)"
            str &= STRfLD & "values(" & STRVAL
            ODA.SelectCommand.CommandText = str
            ODA.SelectCommand.ExecuteNonQuery()

            '' Hit Kicking Field
            If updquery.Trim.Length > 5 Then
                ODA.SelectCommand.CommandText = updquery
                ODA.SelectCommand.ExecuteNonQuery()
            End If
        Next
        Session(formname) = Nothing
        Session(formname & "VAL") = Nothing
        ODA.Dispose()
        con.Close()
        con.Dispose()
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

    Protected Sub SavingChildItemOnEdit(ByVal FORMNAME As String)
        Dim dtFD As New DataTable
        Dim dtField As New DataTable
        Dim DTVALUE As New DataTable
        Dim errormsg As String = ""
        dtField = ViewState(FORMNAME)
        If Session(FORMNAME) Is Nothing Then
            For Each dr As DataRow In dtField.Rows
                dtFD.Columns.Add(dr.Item("displayname"), GetType(String))
                DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
            Next
            dtFD.Columns.Add("tid", GetType(String))
        Else
            dtFD = Session(FORMNAME)
            DTVALUE = Session(FORMNAME & "VAL")
            If dtFD.Rows.Count > 1 Then
                dtFD.Rows.RemoveAt(dtFD.Rows.Count - 1)
            End If
        End If
        Dim drnew As DataRow = dtFD.NewRow()
        Dim DRNEWVAL As DataRow = DTVALUE.NewRow()
        For Each dr As DataRow In dtField.Rows
            Dim dispName As String = dr.Item("displayname").ToString()
            Select Case dr.Item("FieldType").ToString().ToUpper()
                Case "TEXT BOX"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
                Case "DROP DOWN"
                    Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), DropDownList)
                    drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
                Case "CALCULATIVE FIELD"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
                Case "LOOKUP"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
            End Select
        Next
        drnew.Item("tid") = FORMNAME & "-" & ViewState("Index") & "-" & ViewState("ID")
        'dtFD.Rows.Add(drnew)
        dtFD.Rows.RemoveAt(ViewState("Index"))
        dtFD.Rows.InsertAt(drnew, ViewState("Index"))
        'DTVALUE.Rows.Add(DRNEWVAL)
        DTVALUE.Rows.RemoveAt(ViewState("Index"))
        DTVALUE.Rows.InsertAt(DRNEWVAL, ViewState("Index"))
        Session(FORMNAME) = dtFD
        Session(FORMNAME & "VAL") = DTVALUE
        BINDGRID1(dtFD)
        ModalPopupExtender1.Hide()
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
    Protected Sub GridExp()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim DS As New DataSet
        ODA.SelectCommand.Parameters.Clear()
        ODA.SelectCommand.CommandTimeout = 180
        ODA.SelectCommand.CommandText = "uspGetGridexport"
        ODA.SelectCommand.CommandType = CommandType.StoredProcedure
        ODA.SelectCommand.Parameters.AddWithValue("sField", ddlField.SelectedValue)
        ODA.SelectCommand.Parameters.AddWithValue("sValue", txtValue.Text)
        ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        ODA.SelectCommand.Parameters.AddWithValue("documentType", scrname)
        ODA.SelectCommand.Parameters.AddWithValue("MASTERQRY", Session("MASTERQRY"))
        DS.Tables.Clear()
        ODA.Fill(DS, "ITEM")
        ViewState("grd") = DS.Tables("ITEM")
        DS.Dispose()
    End Sub

    'Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexport.Click
    '    Try
    '        GridExp()
    '        Dim screen As String = Request.QueryString("SC").ToString()
    '        Response.ClearContent()

    '        'gvexport.Columns(0).Visible = False
    '        Dim gridview1 As New GridView
    '        gridview1.AllowPaging = False
    '        gridview1.DataSource = ViewState("grd")
    '        gridview1.DataBind()

    '        Response.ContentType = "application/vnd.ms-excel"

    '        Response.AddHeader("content-disposition", "attachment;filename=" & screen & ".xls")

    '        'Prepare to export the DataGrid
    '        Dim oStringWriter As New System.IO.StringWriter
    '        Dim oHtmlTextWriter As New HtmlTextWriter(oStringWriter)

    '        'Use the DataGrid control to add the details
    '        gridview1.RenderControl(oHtmlTextWriter)
    '        'Finish the Excel spreadsheet and send the response
    '        Response.Write(oStringWriter.ToString())
    '        ' gridDocs0.Dispose()
    '        Response.End()

    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured when exporting data. Please try again"
    '    End Try
    'End Sub

    Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexport.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataTable
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Try
            GridExp()
            Dim screen As String = Request.QueryString("SC").ToString()
            Response.ClearContent()
            Dim gridview1 As New GridView
            gridview1.AllowPaging = False
            Dim dt As New DataTable
            dt = ViewState("grd")
            If dt.Rows.Count = 0 Then
                Dim dr1 As DataRow
                dr1 = dt.NewRow()
                dt.Rows.Add(dr1)
                lblMsg.Text = "There were no record(s) against this master"
                Exit Sub
            End If
            gridview1.DataSource = dt
            gridview1.DataBind()
            Response.Clear()
            Response.Buffer = True
            '  Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>VEHICLE LOG BOOK (Electronic)</h3></div> <br/>")
            Response.AddHeader("content-disposition", "attachment;filename=" & screen & ".xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            Dim datatype As String
            For i = 0 To gridview1.HeaderRow.Cells.Count - 1
                da.SelectCommand.CommandText = "Select datatype from mmm_mst_fields where eid=" & Session("EID") & " and displayname='" & gridview1.HeaderRow.Cells(i).Text.ToString() & "' and documenttype='" & screen & "'"
                ds.Clear()
                da.Fill(ds)
                If ds.Rows.Count > 0 Then
                    datatype = ds.Rows(0).Item(0).ToString
                    If datatype.ToUpper.ToString = "TEXT" Or datatype.ToUpper.ToString = "DATETIME" Then
                        For j = 0 To gridview1.Rows.Count - 1
                            gridview1.Rows(j).Cells(i).Attributes.Add("class", "textmode")
                        Next
                    End If
                End If
            Next
            gridview1.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
            Response.Write(style)
            Response.Output.Write(HttpUtility.HtmlDecode(sw.ToString()))
            Response.Flush()
            Response.End()
        Catch ex As Exception
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Text = "An error occured when exporting data. Please try again"
        Finally
            ds.Dispose()
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub


    Protected Sub SqlData_Selected(sender As Object, e As SqlDataSourceStatusEventArgs) Handles SqlData.Selected
        lblRecord.Text = ""
        lblMsg.Text = ""
        Try
            lblRecord.Text = "Displaying " & e.AffectedRows.ToString() & " of total " & ViewState("recordscnt") & " Records."

            If e.AffectedRows < 1 Then
                lblMsg.Text = "No records Found!!!"
                Exit Sub
            End If

        Catch ex As Exception

        Finally


        End Try
    End Sub


    Protected Sub SqlData_Selecting(sender As Object, e As SqlDataSourceSelectingEventArgs) Handles SqlData.Selecting
        e.Command.CommandTimeout = 300
        If Not IsPostBack Then
            e.Cancel = True
        End If
    End Sub

    Protected Sub CalculateFormulaP(sender As Object, e As EventArgs)
        Dim viewdoc As String = Request.QueryString("SC").ToString()
        CalculateFormulaParent(viewdoc)
    End Sub

    Protected Sub CalculateFormulaParent(Documenttype As String)
        Dim Formula As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim screenname As String = Request.QueryString("SC").ToString()
        Dim ob As New DynamicForm
        Dim viewdoc As String = screenname
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & Session("EID").ToString() & " and FormName = '" & Documenttype & "' and FF.isActive=1 order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "fields")


        Dim onlyFiltered As DataView = ds.Tables(0).DefaultView
        onlyFiltered.RowFilter = "Fieldtype='Formula Field'"
        Dim FormulaFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()           ' Formula Fields Datatable
        Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
        viewdoc = viewdoc.Replace(" ", "_")

        If CalculativeField.Length > 0 Then
            For i As Integer = 0 To FormulaFlds.Rows.Count - 1
                Dim formulaeditorr As New formulaEditor
                Dim forvalue As String = String.Empty
                Formula = ob.FormulaCalculation(FormulaFlds.Rows(i).Item("KC_LOGIC"), FormulaFlds, ds.Tables("fields"), pnlFields)      'Get Values in formula Fileds from Control
                forvalue = formulaeditorr.CalculateFormulaonFly(Formula)
                'ob.FillFormulaControlsOnPanel(forvalue, FormulaFlds, pnlFields)
                Dim txtBox As New TextBox
                txtBox = CType(pnlFields.FindControl("fld" & FormulaFlds.Rows(i).Item("FieldID").ToString()), TextBox)
                txtBox.Text = forvalue
                'Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & 32 & ""
                'da.SelectCommand.CommandText = upquery
                'da.SelectCommand.CommandType = CommandType.Text
                'da.SelectCommand.ExecuteNonQuery()
            Next
            con.Close()
        End If
    End Sub

    Public Sub bindgridbyrole()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim scrname As String = Request.QueryString("SC").ToString()
            da.SelectCommand.CommandText = "select formname,docmapping from mmm_mst_forms where eid=" & Session("EID") & " and isroledef=1 order by formname "
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "docmapping")
            Dim docmappingfld As String = String.Empty
            Dim form As String = String.Empty
            Dim fld As String = String.Empty
            Dim mainqry As String = " from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname.ToString() & "'  "
            Dim qry As String = String.Empty
            Dim ischeck As String = "select docmapping from mmm_mst_Forms where eid=" & Session("EID") & " and formname='" & scrname.ToString() & "'"
            da.SelectCommand.CommandText = ischeck
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim issc As String = da.SelectCommand.ExecuteScalar().ToString()

            If ds.Tables("docmapping").Rows.Count > 0 Then
                If Not issc Is Nothing And issc <> "" Then
                    Dim ss As String = "  and tid in (select * from inputstring(coalesce((select top 1  isnull(convert(nvarchar(max)," & issc & "),0) from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename='" & Session("USERROLE") & "'),'0')))"
                    Session("MasterQRY") = mainqry & ss
                Else
                    For i As Integer = 0 To ds.Tables("docmapping").Rows.Count - 1
                        docmappingfld = ds.Tables("docmapping").Rows(i).Item("docmapping").ToString().Trim()
                        form = ds.Tables("docmapping").Rows(i).Item("formname").ToString().Trim()
                        da.SelectCommand.CommandText = "Select top 1 fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & scrname.ToString() & "' and dropdown like '%Master-" & form.ToString() & "-%' order by displayname"
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        fld = da.SelectCommand.ExecuteScalar()
                        If Not fld Is Nothing Then
                            qry = qry & " " & fld & " in (select * from inputstring(coalesce((select top 1  isnull(convert(nvarchar(max)," & docmappingfld & "),0) from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename='" & Session("USERROLE") & "'),'0'))) and"
                        End If
                    Next
                    If qry.Length > 10 Then
                        qry = qry.Remove(qry.Length - 3)
                    End If

                    If qry.Length > 10 Then
                        mainqry = mainqry & " and   "
                    End If
                    Session("MasterQRY") = mainqry & qry
                End If
            Else
                Session("MasterQRY") = "from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname.ToString() & "' and 1=2 "
            End If

            If UCase(Session("USERROLE")) = "SU" Then
                Session("MasterQRY") = " from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & scrname.ToString() & "'  "
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                da.Dispose()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try



    End Sub

#Region "auto complete list" ' by balli 1 aprail march
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetCompletionList(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim arr() As String
        arr = contextKey.Split("-")
        Dim TID As String = "TID"
        Dim TABLENAME As String = ""
        If UCase(arr(0).ToString()) = "MASTER" Then
            TABLENAME = "MMM_MST_MASTER"
        ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
            TABLENAME = "MMM_MST_DOC"
        ElseIf UCase(arr(0).ToString()) = "CHILD" Then
            TABLENAME = "MMM_MST_DOC_ITEM"
        ElseIf UCase(arr(0).ToString) = "STATIC" Then
            If arr(1).ToString.ToUpper = "USER" Then
                TABLENAME = "MMM_MST_USER"
                TID = "UID"
            ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                TABLENAME = "MMM_MST_LOCATION"
                TID = "LOCID"
            End If
        End If
        HttpContext.Current.Session("tableTID") = TABLENAME & "||" & TID  ' this is initializing because we need this at textbox changed event  in dynamic class
        Dim panelfields As Panel = HttpContext.Current.Session("pnlFields")
        Dim updpanel As UpdatePanel = HttpContext.Current.Session("updatepanel1")
        'autofilExtender.ContextKey = ds.Rows(i).Item("dropdown").ToString() & "-" & ds.Rows(i).Item("FieldID").ToString() & "-" & ds.Rows(i).Item("dropdowntype").ToString() & "-" & ds.Rows(i).Item("autofilter").ToString() & "-" & ds.Rows(i).Item("InitialFilter").ToString()
        'context key contain  : dropdown-fieldid-dropdowntype-autofilter-InitialFilter' adding the fieldid in context key because later on we need id of field in web method 

        ''''for blank the lookup control on selection 


        'Dim sqlda As New SqlDataAdapter("select * from MMM_MST_Fields where eid=" & HttpContext.Current.Session("EID") & " and dropdown='" & arr(3) & "'", con)
        'Dim dtt As New DataTable
        'sqlda.Fill(dtt)
        'If dtt.Rows.Count > 0 Then
        '    For j As Integer = 0 To dtt.Rows.Count - 1
        '        If DynamicForm.GetControl(panelfields, "fld" & dtt.Rows(j).Item("fieldid").ToString) Then
        '            Dim txtbox As TextBox = CType(panelfields.FindControl("fld" & dtt.Rows(j).Item("fieldid").ToString), TextBox)
        '            Dim num As String = txtbox.Text
        '            txtbox.Text = String.Empty
        '        End If
        '    Next
        'End If
        'sqlda.Dispose()
        ''' end here to lookup check

        Dim lookUpqry As String = ""
        Dim str As String = ""
        If arr(0).ToUpper() = "CHILD" Then
            str = "select top 50  " & arr(2).ToString() & " As [name]," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
        ElseIf arr(0).ToUpper() <> "STATIC" Then
            str = "select top 50  " & arr(2).ToString() & " As [name]," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
        Else
            If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                str = "select DISTINCT " & arr(2).ToString() & " As [name],SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
            Else
                str = "select " & arr(2).ToString() & " As [name]," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
            End If
        End If

        Dim xwhr As String = ""
        Dim tids As String = ""
        Dim Rtids As String = ""   ' for prerole data filter
        'Dim tidarr() As String

        ''FILTER THE DATA ACCORDING TO USER 
        Dim ob As New DynamicForm
        tids = ob.UserDataFilter(contextKey.Split("-").ElementAt(1).ToString(), arr(1).ToString())
        Rtids = ob.UserDataFilter_PreRole(arr(1).ToString(), TABLENAME)  '' new by sunil for pre role data filter 22-feb

        If tids.Length >= 2 Then
            xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
            'ElseIf tids = "0" Then
            '    pnlFields.Visible = False
            '    btnActEdit.Visible = False
            '    UpdatePanel1.Update()
            '    xwhr = ""
        End If

        If Rtids.Length >= 2 Then
            If xwhr.ToString = "" Then
                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & Rtids & ")"
            Else
                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & "," & Rtids & ")"
            End If
        End If

        str = str & "  AND M.isauth=1 " & xwhr
        Dim AutoFilter As String = arr(5) 'ds.Rows(i).Item("AutoFilter").ToString()
        Dim InitFilterArr As String() = arr(6).Split(":") 'ds.Rows(i).Item("InitialFilter").ToString().Split(":")

        If AutoFilter.Length > 0 Then
            Dim filteriD As String = arr(3)
            Dim mval As String = ""
            Dim filterMasVal As String = ""
            Dim ODA As New SqlDataAdapter("", con)
            ' ODA.SelectCommand.CommandText = "select top 1 * from MMM_MSt_Fields where eid='" & HttpContext.Current.Session("EID") & "' and fieldtype in ('Drop Down','AutoComplete') and lookupvalue like '%" & filteriD & "-S%'"
            'With(nolock) added by Himank on 29th sep 2015
            ODA.SelectCommand.CommandText = "select top 1 * from MMM_MSt_Fields  WITH(NOLOCK)  where eid='" & HttpContext.Current.Session("EID") & "' and fieldtype in ('Drop Down','AutoComplete') and lookupvalue like '%" & filteriD & "-S%'"
            Dim dt As New DataSet()
            ODA.Fill(dt, "filtrId")
            If dt.Tables("filtrId").Rows.Count > 0 Then
                Dim fieldtype As String = dt.Tables("filtrId").Rows(0).Item("fieldtype").ToString
                If fieldtype.ToUpper = "DROP DOWN" Then
                    If dt.Tables("filtrId").Rows(0).Item("DropDownType").ToString = "SESSION VALUED" Then
                        ' 16 march balli  
                        'str = "select " & arr(2).ToString() & " As [name]," & TID & "[tid]" & lookUpqry & " from MMM_MST_USER M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        'Dim ddl As DropDownList = CType(panelfields.FindControl("fld" & dt.Tables("filtrId").Rows(0).Item("fieldid")), DropDownList)
                        'mval = ddl.SelectedItem.Value
                        'filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & mval & "' "
                    Else
                        Dim ddl As DropDownList = CType(panelfields.FindControl("fld" & dt.Tables("filtrId").Rows(0).Item("fieldid")), DropDownList)
                        mval = ddl.SelectedItem.Value
                        filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & mval & "' "
                    End If
                Else
                    Dim ddl As HiddenField = CType(panelfields.FindControl("HDN" & dt.Tables("filtrId").Rows(0).Item("fieldid")), HiddenField)  ' for hidden field
                    'Dim ddl As TextBox = CType(panelfields.FindControl("fld" & dt.Tables("filtrId").Rows(0).Item("fieldid")), TextBox)
                    'Dim MId As String() = Split(ddl.Text, "-")
                    'mval = Replace(MId(1), "[", "")
                    'mval = mval.Replace("]", "")
                    ' filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & mval & "' "
                    filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & Val(ddl.Value) & "' "
                End If
            End If
            ' for binding autocomplete with the selection of dropdo
            If arr(0).ToUpper() = "CHILD" Then
                If AutoFilter.ToUpper = "DOCID" Then
                    str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                Else
                    str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                End If
            ElseIf arr(0).ToUpper() <> "STATIC" Then
                'With(nolock) added by Himank on 29th sep 2015
                str = "select top 50 " & arr(2).ToString() & " As [name],convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK) WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                str = str & "  AND M.isauth=1 " & xwhr
                If filterMasVal.Length > 1 Then
                    str = str & filterMasVal
                End If
            Else
                'With(nolock) added by Himank on 29th sep 2015
                str = "select top 50 " & arr(2).ToString() & " As [name],convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)  WHERE EID=" & HttpContext.Current.Session("EID") & " "
                str = str & "  AND M.isauth=1 " & xwhr
                If filterMasVal.Length > 1 Then
                    str = str & filterMasVal
                End If
            End If
        ElseIf InitFilterArr.Length > 1 Then
            Dim daa As New SqlDataAdapter("select * from MMM_MST_Fields where fieldid=" & InitFilterArr(0) & "", con)
            Dim dss As New DataSet
            daa.Fill(dss, "data")
            Dim row() As DataRow = dss.Tables("data").Select("fieldid=" & InitFilterArr(0).ToString())
            If arr(0).ToUpper() = "DOCUMENT" Or arr(0).ToUpper() = "MASTER" Then
                If row.Length > 0 Then
                    'With(nolock) added by Himank on 29th sep 2015
                    str = " Select top 50 " & arr(2).ToString() & "  As [name], convert(nvarchar(10),tid) [TID] FROM " & TABLENAME & " M  WITH(NOLOCK)  where EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                    str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                    str = str & "  AND M.isauth=1 " & xwhr
                End If
            ElseIf arr(0).ToUpper() = "STATIC" Then
                If row.Length > 0 Then
                    'With(nolock) added by Himank on 29th sep 2015
                    str = " Select top 50 " & arr(2).ToString() & "As [name],convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " M  WITH(NOLOCK)  where EID=" & HttpContext.Current.Session("EID") & " "
                    str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                    str = str & "  AND M.isauth=1 " & xwhr
                End If
            End If
            dss.Dispose()
            daa.Dispose()
        End If
        ' Dim da As New SqlDataAdapter("select top 50 " & contextKey.Split("-").ElementAt(2) & " As [name] , tid from " & TABLENAME & "  where EID ='" & HttpContext.Current.Session("EID") & "' and documenttype ='" & contextKey.Split("-").ElementAt(1).ToString() & "' and " & contextKey.Split("-").ElementAt(2).ToString() & " like '" & prefixText & "%' ", con)
        ' for hidden field value check
        'Dim pnlHdnfld As HiddenField = CType(panelfields.FindControl("HDN" & arr(3)), HiddenField)


        ' Dim da As New SqlDataAdapter(str & " and " & contextKey.Split("-").ElementAt(2).ToString() & " like '%" & prefixText & "%'  order by " & arr(2).ToString(), con)
        Dim da As New SqlDataAdapter(str & " and " & contextKey.Split("-").ElementAt(2).ToString() & " like '%" & prefixText & "%'  order by LEN(" & arr(2).ToString() & ") ", con)
        Dim ds As New DataSet()
        da.Fill(ds, "data")
        Dim items As New List(Of String)
        Dim items1 As String = String.Empty
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            'items.Add(ds.Tables("data").Rows(i).Item("name").ToString() & "-[" & ds.Tables("data").Rows(i).Item("tid").ToString() & "]")
            items1 = AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(ds.Tables("data").Rows(i).Item("name").ToString(), ds.Tables("data").Rows(i).Item("tid").ToString())
            items.Add(items1)
        Next
        HttpContext.Current.Session("fieldid") = contextKey.Split("-").ElementAt(3)  ' this session id we need dynamic form for calculation the other field
        ds.Dispose()
        da.Dispose()

        con.Dispose()
        con = Nothing
        Return items.ToArray()
    End Function
#End Region
    Protected Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles gvData.RowCreated

        Dim row As GridViewRow = e.Row
        ' Intitialize TableCell list
        Dim columns As New List(Of TableCell)()
        For Each column As DataControlField In gvData.Columns
            'Get the first Cell /Column
            Dim cell As TableCell = row.Cells(0)
            ' Then Remove it after
            row.Cells.Remove(cell)
            'And Add it to the List Collections
            columns.Add(cell)
        Next

        ' Add cells
        row.Cells.AddRange(columns.ToArray())
    End Sub



    Protected Sub txtValue_TextChanged(sender As Object, e As EventArgs) Handles txtValue.TextChanged
        Dim strSearch As String = Replace(txtValue.Text, "'", "")
        Session("Value") = strSearch
    End Sub
End Class
