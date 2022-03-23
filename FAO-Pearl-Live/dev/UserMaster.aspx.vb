Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Partial Class UserMaster
    Inherits System.Web.UI.Page
    Dim obDMS As New DMSUtil()

    Public msg As String = ""
    Public errmsg As String = ""
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

        Dim strPreviousPage As String = ""
        If Request.UrlReferrer <> Nothing Then
            strPreviousPage = Request.UrlReferrer.Segments(Request.UrlReferrer.Segments.Length - 1)
        End If
        If strPreviousPage = "" Then
            Response.Redirect("~/Invalidaction.aspx")
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1  and F.EID=" & Session("EID").ToString() & " and FormName = 'USER' order by displayOrder", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "fields")
        Dim ob As New DynamicForm

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
                oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & row(i).Item("dropdown").ToString() & "' AND F1.DOCUMENTTYPE='USER'"
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
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub

    Public Sub bindvalue(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlFields, ddl)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='USER' order by DisName", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            Dim od As SqlDataAdapter = New SqlDataAdapter("", con)
            If Session("userrole") = "SU" Then
                od.SelectCommand.CommandText = "select roleid,upper(rolename)rolename From MMM_MST_ROLE where EID=" & Session("EID").ToString() & " "
            Else
                od.SelectCommand.CommandText = "select roleid,upper(rolename)rolename From MMM_MST_ROLE where EID=" & Session("EID").ToString() & " AND rolename<>'SU'"
            End If
            od.SelectCommand.CommandType = CommandType.Text
            Dim dsa As New DataSet()
            od.Fill(dsa, "data")
            ddluserrole.DataSource = dsa.Tables("data")
            ddluserrole.DataTextField = "RoleName"
            ddluserrole.DataValueField = "RoleID"
            ddluserrole.DataBind()
            ddluserrole.Items.Insert(0, "Select")

            lblRecord.Text = "Total Records : " & CType(SqlData.[Select](DataSourceSelectArguments.Empty), DataView).Count

            '  GetMenuData()
            Call GetMenuDataandroles()
            od.Dispose()
            da.Dispose()
            con.Dispose()

        End If
    End Sub
    Private Sub GetMenuDataandroles()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        ' Dim scrname As String = Request.QueryString("SC").ToString()
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
        da.SelectCommand.CommandText = "select * from mmm_mst_menu where  pagelink='UserMaster.aspx' and eid=" & Session("EID") & ""
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
                btnNew.Visible = False
            ElseIf ViewState("numval") = 5 Then
                btnNew.Visible = False

            ElseIf ViewState("numval") = 6 Then

            ElseIf ViewState("numval") = 7 Then
            ElseIf ViewState("numval") = 8 Then
                btnNew.Visible = False
                For Each row As GridViewRow In gvData.Rows
                    Dim btne As ImageButton = TryCast(row.Cells(0).Controls(0).FindControl("btnedit"), ImageButton)
                    btne.Visible = False
                Next
            ElseIf ViewState("numval") = 9 Then
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
                btnNew.Visible = False
            ElseIf ViewState("numval") = 15 Then

            End If
        End If
        con.Close()
        da.Dispose()
    End Sub
    'Private Sub GetMenuData()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    ' Dim scrname As String = Request.QueryString("SC").ToString()
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet
    '    Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
    '    da.SelectCommand.CommandText = "select tid,menuname,menutype, " & cr & " from mmm_mst_accessmenu where menutype='static' and menuname='Users'"
    '    da.Fill(ds, "menu")
    '    For i As Integer = 0 To ds.Tables("menu").Rows.Count - 1

    '        Dim abc As String = ds.Tables("menu").Rows(i).Item(cr).ToString()
    '        If abc.Length > 0 Then
    '            ViewState("numval") = abc
    '        End If
    '    Next
    '    con.Close()
    '    da.Dispose()
    'End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        popAlert.Visible = False
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        ' No Value in Session just fill the Edit Form and Show two button
        btnActEdit.Text = "Update"

        'two methods.. either show data from Grid or Show data from Database.
        ViewState("pid") = pid
        txtProductName.Text = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(row.Cells(1).Text))
        txtProductDesc.Text = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(row.Cells(2).Text.Trim))

        Dim objDC As New DataClass()
        Dim RoleId As String = "0"
        Dim objDT = objDC.ExecuteQryDT("select RoleId from MMM_MST_ROLE where eid=" & Session("EID").ToString() & " and RoleName='" & row.Cells(5).Text.Trim & "'")
        If objDT IsNot Nothing And objDT.Rows.Count > 0 Then
            RoleId = objDT.Rows(0).Item(0).ToString()
        End If
        ddluserrole.SelectedValue = RoleId

        ''Fill Dynamic controls 
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT FF.*,F.* FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = 'USER' order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Dim ob As New DynamicForm
        ob.FillControlsOnPanel(ds.Tables("data"), pnlFields, "USER", pid)
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
        da.Dispose()
        ds.Dispose()
        con.Close()
    End Sub
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        popAlert.Visible = False
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"

        txtProductDesc.Text = ""
        txtProductName.Text = ""
        'txtUserId.Text = ""

        'Mon.Checked = False
        'Tus.Checked = False
        'Wed.Checked = False
        'Thus.Checked = False
        'Fri.Checked = False
        'Sat.Checked = False
        'Sun.Checked = False

        'ddlFrom.Items.Clear()
        'ddlTo.Items.Clear()

        'ddlFrom.Items.Insert(0, "00:00 ")
        'ddlFrom.Items.Insert(1, "00:30 ")
        'ddlFrom.Items.Insert(2, "01:00 ")
        'ddlFrom.Items.Insert(3, "01:30 ")
        'ddlFrom.Items.Insert(4, "02:00 ")
        'ddlFrom.Items.Insert(5, "02:30 ")
        'ddlFrom.Items.Insert(6, "03:00 ")
        'ddlFrom.Items.Insert(7, "03:30 ")
        'ddlFrom.Items.Insert(8, "04:00 ")
        'ddlFrom.Items.Insert(9, "04:30 ")
        'ddlFrom.Items.Insert(10, "05:00 ")
        'ddlFrom.Items.Insert(11, "05:30 ")
        'ddlFrom.Items.Insert(12, "06:00 ")
        'ddlFrom.Items.Insert(13, "06:30 ")
        'ddlFrom.Items.Insert(14, "07:00 ")
        'ddlFrom.Items.Insert(15, "07:30 ")
        'ddlFrom.Items.Insert(16, "08:00 ")
        'ddlFrom.Items.Insert(17, "08:30 ")
        'ddlFrom.Items.Insert(18, "09:00 ")
        'ddlFrom.Items.Insert(19, "09:30 ")
        'ddlFrom.Items.Insert(20, "10:00 ")
        'ddlFrom.Items.Insert(21, "10:30 ")
        'ddlFrom.Items.Insert(22, "11:00 ")
        'ddlFrom.Items.Insert(23, "11:30 ")
        'ddlFrom.Items.Insert(24, "12:00 ")
        'ddlFrom.Items.Insert(25, "12:30 ")
        'ddlFrom.Items.Insert(26, "13:00 ")
        'ddlFrom.Items.Insert(27, "13:30 ")
        'ddlFrom.Items.Insert(28, "14:00 ")
        'ddlFrom.Items.Insert(29, "14:30 ")
        'ddlFrom.Items.Insert(30, "15:00 ")
        'ddlFrom.Items.Insert(31, "15:30 ")
        'ddlFrom.Items.Insert(32, "16:00 ")
        'ddlFrom.Items.Insert(33, "16:30 ")
        'ddlFrom.Items.Insert(34, "17:00 ")
        'ddlFrom.Items.Insert(35, "17:30 ")
        'ddlFrom.Items.Insert(36, "18:00 ")
        'ddlFrom.Items.Insert(37, "18:30 ")
        'ddlFrom.Items.Insert(38, "19:00 ")
        'ddlFrom.Items.Insert(39, "19:30 ")
        'ddlFrom.Items.Insert(40, "20:00 ")
        'ddlFrom.Items.Insert(41, "20:30 ")
        'ddlFrom.Items.Insert(42, "21:00 ")
        'ddlFrom.Items.Insert(43, "21:30 ")
        'ddlFrom.Items.Insert(44, "22:00 ")
        'ddlFrom.Items.Insert(45, "22:30 ")
        'ddlFrom.Items.Insert(46, "23:00 ")
        'ddlFrom.Items.Insert(47, "23:30 ")

        'ddlTo.Items.Insert(0, "00:00 ")
        'ddlTo.Items.Insert(1, "00:30 ")
        'ddlTo.Items.Insert(2, "01:00 ")
        'ddlTo.Items.Insert(3, "01:30 ")
        'ddlTo.Items.Insert(4, "02:00 ")
        'ddlTo.Items.Insert(5, "02:30 ")
        'ddlTo.Items.Insert(6, "03:00 ")
        'ddlTo.Items.Insert(7, "03:30 ")
        'ddlTo.Items.Insert(8, "04:00 ")
        'ddlTo.Items.Insert(9, "04:30 ")
        'ddlTo.Items.Insert(10, "05:00 ")
        'ddlTo.Items.Insert(11, "05:30 ")
        'ddlTo.Items.Insert(12, "06:00 ")
        'ddlTo.Items.Insert(13, "06:30 ")
        'ddlTo.Items.Insert(14, "07:00 ")
        'ddlTo.Items.Insert(15, "07:30 ")
        'ddlTo.Items.Insert(16, "08:00 ")
        'ddlTo.Items.Insert(17, "08:30 ")
        'ddlTo.Items.Insert(18, "09:00 ")
        'ddlTo.Items.Insert(19, "09:30 ")
        'ddlTo.Items.Insert(20, "10:00 ")
        'ddlTo.Items.Insert(21, "10:30 ")
        'ddlTo.Items.Insert(22, "11:00 ")
        'ddlTo.Items.Insert(23, "11:30 ")
        'ddlTo.Items.Insert(24, "12:00 ")
        'ddlTo.Items.Insert(25, "12:30 ")
        'ddlTo.Items.Insert(26, "13:00 ")
        'ddlTo.Items.Insert(27, "13:30 ")
        'ddlTo.Items.Insert(28, "14:00 ")
        'ddlTo.Items.Insert(29, "14:30 ")
        'ddlTo.Items.Insert(30, "15:00 ")
        'ddlTo.Items.Insert(31, "15:30 ")
        'ddlTo.Items.Insert(32, "16:00 ")
        'ddlTo.Items.Insert(33, "16:30 ")
        'ddlTo.Items.Insert(34, "17:00 ")
        'ddlTo.Items.Insert(35, "17:30 ")
        'ddlTo.Items.Insert(36, "18:00 ")
        'ddlTo.Items.Insert(37, "18:30 ")
        'ddlTo.Items.Insert(38, "19:00 ")
        'ddlTo.Items.Insert(39, "19:30 ")
        'ddlTo.Items.Insert(40, "20:00 ")
        'ddlTo.Items.Insert(41, "20:30 ")
        'ddlTo.Items.Insert(42, "21:00 ")
        'ddlTo.Items.Insert(43, "21:30 ")
        'ddlTo.Items.Insert(44, "22:00 ")
        'ddlTo.Items.Insert(45, "22:30 ")
        'ddlTo.Items.Insert(46, "23:00 ")
        'ddlTo.Items.Insert(47, "23:30 ")
        'ddlFrom.DataBind()
        'ddlTo.DataBind()

        ''Clear Dynamic Fields
        Dim ob As New DynamicForm()
        ob.CLEARDYNAMICFIELDS(pnlFields)
        updatePanelEdit.Update()


        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1  and F.EID=" & Session("EID").ToString() & " and FormName = 'USER' order by displayOrder", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "fields")

        ob.CreateControlsOnPanel(ds.Tables("fields"), pnlFields, updatePanelEdit, btnActEdit, 0)
        Dim ROW1() As DataRow = ds.Tables("fields").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
        If ROW1.Length > 0 Then
            For i As Integer = 0 To ROW1.Length - 1
                Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                DDL.AutoPostBack = True
                AddHandler DDL.TextChanged, AddressOf bindvalue
            Next
        End If

        Me.btnEdit_ModalPopupExtender.Show()
        ds.Dispose()
        oda.Dispose()
        con.Close()

    End Sub

    'Protected Sub gridBind()

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetResultUsers", con)
    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '    oda.SelectCommand.Parameters.AddWithValue("username", Trim(txtProductName.Text))
    '    oda.SelectCommand.Parameters.AddWithValue("emailid", Trim(txtProductDesc.Text))
    '    oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())

    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
    '    con.Close()
    '    oda.Dispose()
    '    con.Dispose()

    'End Sub

    'Add By Manvendra Singh
    Private Sub doctype12()
        'Dim objDC As New DataClass()
        'Dim objDT As New DataTable
        'objDT = objDC.ExecuteQryDT("select substring (PageLink, 19, 20) as 'PageLink' from mmm_mst_menu where eid=" & Session("EID") & " and Roles like +'%{'+'" & ddluserrole.SelectedItem.Text & "'+':%' and pageLink like 'Documents.aspx%'" _
        '                            & " Union" _
        '                           & " select substring (PageLink, 19, 20) as 'PageLink' from mmm_mst_menu where eid=" & Session("EID") & " and Roles like +'%{'+'" & ddluserrole.SelectedItem.Text & "'+':%' and  pageLink like 'Master.aspx%'")
        'If objDT.Rows.Count > 0 Then
        '    chkDocumentType.DataSource = objDT
        '    chkDocumentType.DataTextField = "PageLink"
        '    chkDocumentType.DataValueField = "PageLink"
        '    chkDocumentType.DataBind()
        'Else
        '    chkDocumentType.Items.Clear()
        'End If
        ''Comcheck()
        'For j As Integer = 0 To chkDocumentType.Items.Count - 1
        '    chkDocumentType.Items(j).Selected = True
        'Next
        ''chkDocType.InputAttributes("onchange") = "r2('" + chkDocType.ClientID + "','" + chkDocumentType.ClientID + "')"

        ''da.SelectCommand.CommandText = "select FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
        ''da.Fill(ds, "doctype")
        ''da.Dispose()
        ''con.Dispose()
        ''For i As Integer = 0 To ds.Tables("doctype").Rows.Count - 1
        ''    chkDocumentType.Items.Add(ds.Tables("doctype").Rows(i).Item("Formcaption").ToString())
        ''    chkDocumentType.Items(i).Value = ds.Tables("doctype").Rows(i).Item("formname").ToString()
        ''Next
    End Sub
    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        'validation for null entry
        If Trim(txtProductName.Text).Length < 2 Then
            popAlert.Visible = True
            lblMsgEdit.Text = "Please Enter Valid User Name"
            btnEdit_ModalPopupExtender.Show()
            Exit Sub
        End If
        Dim ob As New User
        If ob.EmailAddressCheck(txtProductDesc.Text) = 0 Then
            popAlert.Visible = True
            lblMsgEdit.Text = "Please Enter Valid User EmailID "
            btnEdit_ModalPopupExtender.Show()
            Exit Sub
        End If

        '' disabled by sunil pareek on 
        'If ob.isAlphaNumericAndSpecial(txtProductName.Text) Then
        '    popAlert.Visible = True
        '    lblMsgEdit.Text = "Special character and Numeric are not allowed in User Name  "
        '    btnEdit_ModalPopupExtender.Show()
        '    Exit Sub
        'End If


        'If InStr(1, txtUserId.Text, "'") > 0 Then
        '    lblMsgEdit.Text = " User ID is not valid "
        '    Exit Sub
        'End If

        If ddluserrole.Text = "Select" Then
            popAlert.Visible = True
            lblMsgEdit.Text = "Please Select User Role"
            btnEdit_ModalPopupExtender.Show()
            Exit Sub
        End If

        'If validateemailid(Trim(txtProductDesc.Text)) = 1 Then
        '    lblMsgEdit.Text = "EmailID is already Present"
        '    Exit Sub
        'End If

        'Dim weekoff As String = ""
        'If Mon.Checked = True Then
        '    weekoff = String.Concat(weekoff, "1")
        'End If
        'If Tus.Checked = True Then
        '    weekoff = String.Concat(weekoff, "2")
        'End If
        'If Wed.Checked = True Then
        '    weekoff = String.Concat(weekoff, "3")
        'End If
        'If Thus.Checked = True Then
        '    weekoff = String.Concat(weekoff, "4")
        'End If
        'If Fri.Checked = True Then
        '    weekoff = String.Concat(weekoff, "5")
        'End If
        'If Sat.Checked = True Then
        '    weekoff = String.Concat(weekoff, "6")
        'End If
        'If Sun.Checked = True Then
        '    weekoff = String.Concat(weekoff, "7")
        'End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select loginfield from MMM_MST_Entity where eid=" & Session("EID") & ""
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet
        oda.Fill(ds, "data")
        Dim DocType As String = ""


        If btnActEdit.Text = "Save" Then
            If validateemailid(0, ds.Tables("DATA").Rows(0).Item("loginfield").ToString(), "ADD") Then
                popAlert.Visible = True
                lblMsgEdit.Text = msg
                updatePanelEdit.Update()
                btnEdit_ModalPopupExtender.Show()
                Exit Sub
            End If

            Dim dynamicqry As String = ""
            dynamicqry = ValidateData("ADD")
            If dynamicqry.Length >= 6 Then
                If Trim(Left(dynamicqry, 6)).ToUpper() = "PLEASE" Then
                    popAlert.Visible = True
                    lblMsgEdit.Text = dynamicqry
                    updatePanelEdit.Update()
                    btnEdit_ModalPopupExtender.Show()
                    Exit Sub
                End If
            End If
            '' commmendted as not required - handled in stored proc by sp
            'For Each li As ListItem In chkDocumentType.Items
            '    If li.Selected Then
            '        DocType &= li.Value & ","
            '    End If
            'Next

            'If DocType.Length < 3 Then
            '    popAlert.Visible = True
            '    lblMsgupdate.Text = "Please select some document type to apply rules"
            '    btnEdit_ModalPopupExtender.Show()
            '    Exit Sub
            'Else
            '    DocType = Left(DocType, Len(DocType) - 1)
            'End If
            '' commmendted as not required - handled in stored proc by sp

            Dim ob1 As New DynamicForm
            ''set LoginFields value in Userid 
            Dim LoginFieldQry As String = ""
            LoginFieldQry = "update mmm_mst_user set userid=" & ds.Tables("data").Rows(0).Item("loginfield").ToString & ""

            oda = New SqlDataAdapter("uspInsertUserNew1", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("username", Trim(txtProductName.Text))
            oda.SelectCommand.Parameters.AddWithValue("emailid", Trim(txtProductDesc.Text))
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("CreatedBy", Session("UID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("weeklyHoliday", "")
            oda.SelectCommand.Parameters.AddWithValue("WST", "")
            oda.SelectCommand.Parameters.AddWithValue("WET", "")

            'added default location Delhi 
            oda.SelectCommand.Parameters.AddWithValue("LocationID", 2072)
            oda.SelectCommand.Parameters.AddWithValue("userrole", ddluserrole.SelectedItem.Text)
            ''parameter for dynamicFields
            oda.SelectCommand.Parameters.AddWithValue("dynamicQry", dynamicqry)
            ''parameter for update Userid
            oda.SelectCommand.Parameters.AddWithValue("LoginFieldQry", LoginFieldQry)
            oda.SelectCommand.Parameters.AddWithValue("@DocType", "")
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()


            'ValidateData(iSt, ds.Tables("data").Rows(0).Item("loginfield").ToString(), "ADD")
            'If errmsg.Length > 6 Then
            '    lblMsgEdit.Text = errmsg
            '    updatePanelEdit.Update()
            '    btnEdit_ModalPopupExtender.Show()
            '    Exit Sub
            'End If

            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = "SELECT * FROM MMM_MST_FIELDS WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='USER' AND FIELDTYPE='CHILD ITEM'"
            oda.Fill(ds, "CHILDITEM")
            If ds.Tables("CHILDITEM").Rows.Count > 0 Then
                For Each DR As DataRow In ds.Tables("CHILDITEM").Rows
                    Dim FORMNAME As String() = DR.Item("DROPDOWN").ToString.Split("-")
                    SavingChildItem(FORMNAME(0).Trim(), iSt)
                Next
            End If

            con.Close()
            oda.Dispose()
            con.Dispose()
            If iSt <> 1 Then
                gvData.DataBind()
                ob1.History(Session("EID"), iSt, Session("UID"), "USER", "MMM_MST_USER", "ADD")
                Try
                    obDMS.notificationMail(iSt, Session("EID"), "USER", "USER CREATED")
                    txtProductDesc.Text = ""
                    txtProductName.Text = ""
                    msgUpdateDiv.Visible = True
                    lblMsgupdate.Text = "User has been created successfully "
                    gvData.DataBind()
                    updPnlGrid.Update()
                    'updatePanelEdit.Update()
                    btnEdit_ModalPopupExtender.Hide()
                Catch ex As Exception
                    popAlert.Visible = True
                    lblMsgEdit.Text = "User Created but unable to send mail"
                    txtProductDesc.Text = ""
                    txtProductName.Text = ""
                    updatePanelEdit.Update()
                End Try
            Else
                popAlert.Visible = True
                lblMsgEdit.Text = "This User Already Exist"
            End If
        Else
            'Edit/update Record
            'Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateUser", con) 'old procedure , adding userid in new 

            If validateemailid(ViewState("pid"), ds.Tables("DATA").Rows(0).Item("loginfield").ToString(), "UPDATE") Then
                popAlert.Visible = True
                lblMsgEdit.Text = msg
                updatePanelEdit.Update()
                btnEdit_ModalPopupExtender.Show()
                Exit Sub
            End If
            Dim OB1 As New DynamicForm
            OB1.History(Session("EID"), ViewState("pid"), Session("UID"), "USER", "MMM_MST_USER", "UPDATE")

            Dim dynamicqry As String = ""
            dynamicqry = ValidateData("UPDATE")
            If dynamicqry.Length >= 6 Then
                If Trim(Left(dynamicqry, 6)).ToUpper() = "PLEASE" Then
                    popAlert.Visible = True
                    lblMsgEdit.Text = dynamicqry
                    updatePanelEdit.Update()
                    btnEdit_ModalPopupExtender.Show()
                    Exit Sub
                End If
            End If
            Dim LogString As String = CreateUserModificationLog(Session("EID"), ViewState("pid"))
            Dim LoginFieldQry As String = ""
            LoginFieldQry = "update mmm_mst_user set userid=" & ds.Tables("data").Rows(0).Item("loginfield").ToString & ""

            oda = New SqlDataAdapter("uspUpdateUserNew1", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("username", Trim(txtProductName.Text))
            oda.SelectCommand.Parameters.AddWithValue("emailid", Trim(txtProductDesc.Text))
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("weeklyHoliday", "")
            oda.SelectCommand.Parameters.AddWithValue("WST", "")
            oda.SelectCommand.Parameters.AddWithValue("WET", "")
            oda.SelectCommand.Parameters.AddWithValue("userrole", ddluserrole.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("UID", ViewState("pid"))
            ''parameter for dynamicFields
            oda.SelectCommand.Parameters.AddWithValue("dynamicQry", dynamicqry)

            ''parameter for update Userid
            oda.SelectCommand.Parameters.AddWithValue("LoginFieldQry", LoginFieldQry)
            'oda.SelectCommand.Parameters.AddWithValue("@DocType", DocType)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim ist As Integer = oda.SelectCommand.ExecuteScalar()
            If ist <> 1 Then
                'ValidateData(ViewState("pid"), ds.Tables("data").Rows(0).Item("loginfield").ToString(), "UPDATE")
                If LogString <> "" Then
                    Dim objDMSUtil As New DMSUtil()
                    objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "USER", "USER Updation : " & LogString, ViewState("pid"))
                End If
                gvData.DataBind()
                msgUpdateDiv.Visible = True
                lblMsgupdate.Text = "User has been updated "
                updatePanelEdit.Update()
                btnEdit_ModalPopupExtender.Hide()
            End If
            con.Close()
            oda.Dispose()
            con.Dispose()
        End If
    End Sub

    Public Function CreateUserModificationLog(ByVal eid As Integer, ByVal docid As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from MMM_MST_USER with(nolock) where uid=" & docid & "")
            'Dim da As New SqlDataAdapter("select top 1 * from MMM_MST_HISTORY where docid='" & docid & "' and DOCUMENTTYPE='USER' and eid=" & eid & " order by adate desc", con)
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                If dt.Rows(0).Item("UserName") <> txtProductName.Text Then
                    Logstring += " UserName : " & dt.Rows(0).Item("UserName") & " Change " & txtProductName.Text
                End If
                If dt.Rows(0).Item("userrole") <> ddluserrole.SelectedItem.Text Then
                    Logstring += "|| UserRole : " & dt.Rows(0).Item("userrole") & " Change " & ddluserrole.SelectedItem.Text
                End If
                If dt.Rows(0).Item("emailId") <> txtProductDesc.Text Then
                    Logstring += "|| Email Id : " & dt.Rows(0).Item("emailId") & " Change " & txtProductDesc.Text
                End If
                ' for specific paytm Login in Id Log

                Logstring += CreateUserModificationLogforFieldMapping(dt.Rows(0).Item("fld1"), dt.Rows(0).Item("fld2"))

                ' for specific paytm Login in Id Log end here
            End If
            If Logstring <> "" Then
                Logstring += " By User Id : " & docid
            End If
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function

    Public Function CreateUserModificationLogforFieldMapping(ByVal fld1 As String, ByVal fld2 As String) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from MMM_MST_Fields where eid=" & Session("EID") & " and documentType='User'")
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    Dim dispName As String = dt.Rows(i).Item("displayname").ToString()
                    If dispName = "Login ID" Then
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dt.Rows(i).Item("FieldID").ToString()), TextBox)
                        If txtBox IsNot Nothing Then
                            If fld1 <> txtBox.Text Then
                                Logstring += " || Login Id " & fld1 & " Change  " & txtBox.Text
                            End If
                        End If
                    End If
                Next
            End If
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function




    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataBind()
        If gvData.Rows.Count > 0 Then
            msgUpdateDiv.Visible = True
            lblMsgupdate.Text = gvData.Rows.Count & " Records Found "
        Else
            msgUpdateDiv.Visible = True
            lblMsgupdate.Text = " No record found "
        End If

    End Sub
    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("ename") = pid

        If row.Cells(4).Text.ToUpper() = "ACTIVE" Then
            lblMsgLock.Text = "Are you Sure Want to <b>BLOCK</b> -- " & row.Cells(1).Text & " </br> Rights from Role Assignment, if any will also be revoked"

        Else
            lblMsgLock.Text = "Are you Sure Want to <b>ACTIVATE</b> -- " & row.Cells(1).Text
        End If
        Me.updatePanelLock.Update()
        Me.btnlock_ModalPopupExtender.Show()
    End Sub
    Protected Sub LockUser(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pid As Integer = Val(ViewState("ename").ToString())
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("usplockUnlock", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("uid", pid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()

        'This block added by Ajeet Kumar:: Dated:10th november 2014 
        Try
            Dim objDMSUtil As New DMSUtil
            If iSt = 0 Then
                objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "USER", "USER UNLOCKED", pid)
            Else
                objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "USER", "USER LOCKED", pid)
            End If
            '' below code for removing rights from Role assingment table on de-activation of user from user master
            '' by sunil pareek on 17-Apr-19
            If iSt = 1 Then
                oda.SelectCommand.CommandText = "OnLocing_RemoveRoleAssignment"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("uid", pid)
                oda.SelectCommand.Parameters.AddWithValue("eid", Convert.ToInt32(Session("EID")))
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim rSt As Integer = oda.SelectCommand.ExecuteNonQuery()
            End If
            '' below code for removing rights from Role assingment table on de-activation of user from user master

            Dim objRel As New Relation()
            Dim eid As Integer, CreatedDocType As String, DOCID As Integer, UID As Integer
            eid = Convert.ToUInt32(Session("EID"))
            UID = Convert.ToUInt32(Session("UID"))
            Dim objRes As New RelationResponse()
            CreatedDocType = "USER"
            DOCID = iSt
            objRes = objRel.ExtendRelation(eid, CreatedDocType, DOCID, UID, "", False)
            'In case of costom rule Creation
            If objRes.Success = True And objRes.ShowExtend = True Then
                'trans.Commit()
                Response.Redirect("ExtendRelation.aspx?DOCID=" & DOCID & "&SC=" & CreatedDocType, False)
            End If
        Catch ex As Exception
        End Try
        Dim obDMS As New DMSUtil()
        If iSt = 1 Then
            obDMS.notificationMail(pid, Session("EID"), "USER", "USER DE-ACTIVATED")
        ElseIf iSt = 0 Then
            obDMS.notificationMail(pid, Session("EID"), "USER", "USER ACTIVATED")
        End If
        con.Close()
        oda.Dispose()
        con.Dispose()
        gvData.DataBind()
        msgUpdateDiv.Visible = True
        lblMsgupdate.Text = "User status has been updated "
        updatePanelEdit.Update()
        updPnlGrid.Update()
        btnlock_ModalPopupExtender.Hide()
    End Sub



    Protected Function ValidateData(ByVal ACTION As String) As String
        'Check All Validations
        'now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*   FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = 'USER' order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "fields")
        Dim FinalQry As String = ""
        If ds.Tables("fields").Rows.Count > 0 Then
            Dim ob As New DynamicForm
            'pass query of updation and also type
            FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_USER SET ", "", ds.Tables("fields"), pnlFields, 0)
        End If
        da.Dispose()
        con.Dispose()
        Return FinalQry
    End Function

    Protected Function validateemailid(ByVal uid As Integer, ByVal FLD As String, ByVal action As String) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        da.SelectCommand.CommandText = "Select fieldid,displayname from MMM_MST_FIELDS WHERE EID=" & Session("EID") & " AND FIELDMAPPING='" & FLD & "' AND DOCUMENTTYPE='USER'"
        da.Fill(ds, "FIELD")
        If ds.Tables("FIELD").Rows.Count > 0 Then
            Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & ds.Tables("field").Rows(0).Item("fieldid").ToString()), TextBox)
            If action.ToUpper = "ADD" Then
                da.SelectCommand.CommandText = "select * from MMM_MST_USER where eid=" & Session("EID") & " and " & FLD & "='" & TXTBOX.Text & "'"
            ElseIf action.ToUpper = "UPDATE" Then
                da.SelectCommand.CommandText = "select * from MMM_MST_USER where eid=" & Session("EID") & " and " & FLD & "='" & TXTBOX.Text & "' and uid<>" & uid & ""
            End If
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "data")


            If ds.Tables("data").Rows.Count > 0 Then
                msg = ds.Tables("FIELD").Rows(0).Item("DISPLAYNAME").ToString() & " ALREADY EXISTS."
                updatePanelEdit.Update()
                'updPnlGrid.Update()
                Return 1
            Else
                Return 0
            End If
        Else
            If action.ToUpper = "ADD" Then
                da.SelectCommand.CommandText = "select * from MMM_MST_USER where eid=" & Session("EID") & " and emailid= '" & txtProductDesc.Text & "'"
            ElseIf action.ToUpper = "UPDATE" Then
                da.SelectCommand.CommandText = "select * from MMM_MST_USER where eid=" & Session("EID") & " and emailid= '" & txtProductDesc.Text & "' and uid<>" & uid & ""
            End If
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                msg = "Email Id Already Exist"
                Return 1
            Else
                Return 0
            End If
        End If
        con.Close()
        da.Dispose()
    End Function

    'Protected Sub ddlcountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlcountry.SelectedIndexChanged
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    da.SelectCommand.CommandText = "select distinct sid , locationstate  from MMM_MST_LOCATION where Country='" & ddlcountry.SelectedItem.Text & "' "
    '    Dim ds As New DataSet
    '    da.Fill(ds, "state")
    '    ddlstate.DataSource = ds.Tables("state")
    '    ddlstate.DataTextField = "locationstate"
    '    ddlstate.DataValueField = "sid"
    '    ddlstate.DataBind()
    '    ddlstate.Items.Insert(0, "Please Select")
    '    ds.Dispose()
    '    da.Dispose()
    '    con.Close()
    'End Sub

    'Protected Sub ddlstate_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlstate.SelectedIndexChanged
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    da.SelectCommand.CommandText = "select locid,locationname  from MMM_MST_LOCATION where Country='" & ddlcountry.SelectedItem.Text & "' and locationstate='" & ddlstate.SelectedItem.Text & "' "
    '    Dim ds As New DataSet
    '    da.Fill(ds, "state")
    '    ddlLocationName.DataSource = ds.Tables("state")
    '    ddlLocationName.DataTextField = "locationname"
    '    ddlLocationName.DataValueField = "locid"
    '    ddlLocationName.DataBind()
    '    ddlLocationName.Items.Insert(0, "Please Select")
    '    ds.Dispose()
    '    da.Dispose()
    '    con.Close()

    'End Sub

    ''''show Child Form as Pop Up
    Public Sub ShowChildForm(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim btnDetails As Button = TryCast(sender, Button)
        Dim formname As String = btnDetails.Text
        formname = Right(formname, formname.Length - 5).Trim()
        ViewState("FN") = formname
        Session("ID") = Right(btnDetails.ID, btnDetails.ID.Length - 3)
        Dim ob As New DynamicForm
        If ViewState("ID") <> btnDetails.ID Or ViewState("ID") Is Nothing Or Session("CHILD") Is Nothing Then
            ViewState("ID") = btnDetails.ID
            Dim scrname As String = "USER"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & formname & "' order by displayOrder", con)
            Dim DS As New DataSet
            oda.Fill(DS, "CHILD")
            Session("CHILD") = DS.Tables("CHILD")
            ViewState(formname) = DS.Tables("CHILD")
            pnlFields1.Controls.Clear()
            Label3.Text = ""
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

    ''' <summary>
    ''' SAVE CHILD ITEM
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>this function is used to save or update the child item</remarks>
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

    ''' <summary>
    ''' this function is used to create datatable and add record in that datatable child item wise and maintains the session variable for childitem
    ''' </summary>
    ''' <param name="formname"></param>
    ''' <remarks>By Ankit</remarks>
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

    ''' <summary>
    ''' this function is used display child items with Total Row in Grid before saving into database.
    ''' </summary>
    ''' <param name="DT"></param>
    ''' <remarks>By Ankit</remarks>
    Protected Sub BINDGRID1(ByVal DT As DataTable)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim scrname As String = "USER"
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
        Dim scrname As String = "USER"
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

    ''' <summary>
    ''' this function is used to save the child items in database with docid
    ''' </summary>
    ''' <param name="formname"></param>
    ''' <param name="docid"></param>
    ''' <remarks>By Ankit</remarks>
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

    ''' <summary>
    ''' this is used for filling controls on editing
    ''' </summary>
    ''' <param name="dtfields">this contains all fields of this documenttype</param>
    ''' <param name="dtflds">contains all fields value of current record which is displaying in Grid</param>
    ''' <param name="dtval" >contains all values of current which is saved in database</param>
    ''' <param name="index" >contains the index of current row</param>
    ''' <param name="pnlfield">Reference of Panel on which controls are rendered</param>
    ''' <param name="updpnl" >update panel on panel</param>
    ''' <remarks>by Ankit</remarks>
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

    ''' <summary>
    ''' this function is used for saving record in database while editing
    ''' </summary>
    ''' <param name="FORMNAME"> contains documenttype</param>
    ''' <remarks></remarks>
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

    Public Sub bindvalue1(ByVal sender As Object, ByVal e As EventArgs)
        Dim c As Control = GetPostBackControl(Me.Page)
        '...
        If c IsNot Nothing Then
        End If
        If TypeOf c Is System.Web.UI.WebControls.DropDownList Then
            Dim ddl As DropDownList = TryCast(c, DropDownList)
            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim ob As New DynamicForm()
            ob.bind(id, pnlFields1, ddl)
        End If
        updpnlchild.Update()
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

    Public Shared Function GetPostBackControl(page As Page) As Control
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

    Public Sub AssignRole(ByVal sender As Object, ByVal e As EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Role") = pid

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        ' for role name bind 

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim sql As String = ""
        'sql = "select * from MMM_MST_ROle where eid=" & Session("EID") & " and roletype='Pre Type' and roleName <> 'SU' "

        'If ddlRoleType.SelectedItem.Text = "Pre-Type" Then
        '    sql = "select * from MMM_MST_ROle where eid=" & Session("EID") & " and roletype='Pre Type' and roleName <> 'SU' "
        'ElseIf ddlRoleType.SelectedItem.Text = "Pre-Type" Then
        '    sql = "select * from MMM_MST_ROle where eid=" & Session("EID") & " and roletype='Post Type' and roleName <> 'SU' "
        'End If

        'oda.SelectCommand.CommandText = sql

        Dim ds As New DataSet()
        'oda.Fill(ds, "Role")
        ddlUserList.Items.Clear()
        ddlUserRoleName.Items.Clear()
        'ddlUserRoleName.DataSource = ds.Tables("Role")
        'ddlUserRoleName.DataTextField = "roleName"
        'ddlUserRoleName.DataValueField = "roleid"
        'ddlUserRoleName.DataBind()
        'ddlUserRoleName.Items.Insert(0, "Please Select")
        ' for username bind in dropdown
        oda.SelectCommand.CommandText = "select uid , username from MMM_MST_User where eid=" & Session("EID") & " and userrole<>'SU'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "User")
        ddlUserList.DataSource = ds.Tables("User")
        ddlUserList.DataTextField = "username"
        ddlUserList.DataValueField = "uid"
        ddlUserList.DataBind()
        ddlUserList.Items.Insert(0, "Please Select")
        ddlUserList.SelectedIndex = ddlUserList.Items.IndexOf(ddlUserList.Items.FindByValue(row.Cells(7).Text))
        ddlUserList.Enabled = False
        con.Dispose()
        oda.Dispose()
        UserRoledata(ViewState("Role"))
        lblMessRole.Text = ""
        Me.updPnlRole.Update()
        Me.btnRole_ModalPopupExtender.Show()
    End Sub
    Public Function CreateUserModificationPreRole(ByVal eid As Integer, ByVal Uid As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from MMM_ref_PreRole_user with(nolock) where uid=" & Uid & "")
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                If dt.Rows(0).Item("RoleName") <> ddlUserRoleName.SelectedItem.Text() Then
                    Logstring += " RoleName : " & dt.Rows(0).Item("RoleName") & " Change " & ddlUserRoleName.SelectedItem.Text() & " for UserName " & ddlUserList.SelectedItem.Value
                End If
            Else
                Logstring += "New Role : " & ddlUserRoleName.SelectedItem.Text() & " Created for " & ddlUserList.SelectedItem.Text
            End If
            Logstring += " By User Id : " & Uid
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function
    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click
        If btnActUserSave.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            '' Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertPreRoleAssignment", con)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertPre_PostRoleAssignment", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@uid", ddlUserList.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("@RoleName", ddlUserRoleName.SelectedItem.Text())
            oda.SelectCommand.Parameters.AddWithValue("@type", ddlRoleType.SelectedItem.Value())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim i As Integer = oda.SelectCommand.ExecuteScalar()
            If i > 0 Then
                Dim objDMSUtil As New DMSUtil()
                objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Additional Role Assignment", "Additional Role Assignment : " & CreateUserModificationPreRole(Session("EID"), ddlUserList.SelectedItem.Value), ViewState("pid"))
            End If
            con.Close()
            oda.Dispose()
            If i > 0 Then
                msgUpdateDiv.Visible = True
                lblMsgupdate.Text = " Role Assignment Created successfully "
            Else
                lblMsgupdate.Text = " Record in Role Assignment Already exists "
            End If
            UserRoledata(ViewState("Role"))
            lblMessRole.Text = "Role Created successfully"
            updPnlRole.Update()
            'Me.updPnlGrid.Update()
        End If
    End Sub
    Private Sub UserRoledata(ByVal roleid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("select tid, U.username,pU.Rolename from MMM_ref_PreRole_user PU left outer join MMM_MST_User U  on PU.uid=U.uid  where PU.eid=" & Session("EID") & "  and  PU.uid=" & roleid & " ", con)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select tid, U.username,pU.Rolename, 'Pre Type' [RoleType] from MMM_ref_PreRole_user PU left outer join MMM_MST_User U  on PU.uid=U.uid  where PU.eid=" & Session("EID") & "  and  PU.uid=" & roleid & " Union all " & " select tid, U.username,pU.Rolename, 'Post Type' [RoleType] from MMM_ref_Role_user PU left outer join MMM_MST_User U  on PU.uid=U.uid  where PU.eid=" & Session("EID") & "  and  PU.uid=" & roleid & "", con)
        Dim ds As New DataSet
        oda.Fill(ds, "roledata")
        gvDataRole.DataSource = ds.Tables("roledata")
        gvDataRole.DataBind()
        con.Close()
        oda.Dispose()
    End Sub
    Public Sub UserRoleDeleteHit(ByVal sender As Object, ByVal e As EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvDataRole.DataKeys(row.RowIndex).Value)
        Dim typ As String = Convert.ToString(Me.gvDataRole.Rows(row.RowIndex).Cells(2).Text())


        ViewState("DelRoleId") = pid
        ViewState("DelRoleType") = typ
        lblMessuserRole.Text = "Are you sure want to delete " & row.Cells(1).Text
        updDelRole.Update()
        MP_DelRole.Show()
    End Sub
    Public Function CreateUserModificationPreRoleDelete(ByVal Uid As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim str As String = ""
            If ViewState("DelRoleType") = "Pre Type" Then
                str = "select * from MMM_ref_PreRole_user where eid=" & Session("EID") & "  and  tid=" & ViewState("DelRoleId") & " "
            Else
                str = "select * from MMM_ref_Role_user where eid=" & Session("EID") & "  and  tid=" & ViewState("DelRoleId") & " "
            End If
            Dim dt As DataTable = dataClass.ExecuteQryDT(str)
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                Logstring += " " & dt.Rows(0).Item("RoleName") & " Role Delete for " & ddlUserList.SelectedItem.Text
            End If
            Logstring += " By User Id : " & Uid
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function
    Public Sub delUsrRole(ByVal sender As Object, ByVal e As EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim str As String = ""
        Dim log As String = CreateUserModificationPreRoleDelete(Session("UID"))
        If ViewState("DelRoleType") = "Pre Type" Then
            str = "delete from MMM_ref_PreRole_user where eid=" & Session("EID") & "  and  tid=" & ViewState("DelRoleId") & " "
        Else
            str = "delete from MMM_ref_Role_user where eid=" & Session("EID") & "  and  tid=" & ViewState("DelRoleId") & " "
        End If
        oda.SelectCommand.CommandText = str
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.Parameters.Clear()

        Dim i As Integer = oda.SelectCommand.ExecuteNonQuery()
        If i > 0 Then
            Dim objDMSUtil As New DMSUtil()
            objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Additional Role Assignment", "USER Additional Role Assignment Deleted : " & log, ViewState("pid"))
        End If
        con.Close()
        oda.Dispose()
        UserRoledata(ViewState("Role"))
        lblMessRole.Text = "Role deleted successfully"
        Me.updPnlRole.Update()
        MP_DelRole.Hide()
    End Sub

    Protected Sub btnexport_Click(sender As Object, e As ImageClickEventArgs) Handles btnexport.Click
        exportuser()
    End Sub

    Private Sub exportuser()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='User' order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Try
            If ds.Tables("data").Rows.Count > 0 Then
                Dim addall As String = ""
                Dim fm As String = ""
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    Dim dn As String = UCase(ds.Tables("data").Rows(i).Item("displayname").ToString())
                    Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                    Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                    Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                    Dim dty As String = ds.Tables("data").Rows(i).Item("documenttype").ToString()


                    addall = addall & dn
                    Dim d3 As String = ""
                    Dim dw As String = ""
                    If UCase(mv) = "MASTER VALUED" Or UCase(mv) = "SESSION VALUED" Then
                        Dim ss As String() = DD.ToString().Split("-")
                        If UCase(ss(1)).ToString <> "USER" Then
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & Session("EID") & " and convert(nvarchar,tid)=convert(nvarchar,replace(m." & fld & ",'Select','0')) and documenttype='" & ss(1).ToString & "')"
                        Else
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & Session("EID") & " and Uid=convert(nvarchar,replace(m." & fld & ",'Select','0')))"
                        End If
                        dw = dw & d3
                        fm = fm & dw & "[" & dn & "]" & ","
                    Else
                        fm = fm & fld & "[" & dn & "]" & ","
                    End If
                Next
                addall = addall.Remove(addall.Length - 1)


                fm = fm.Remove(fm.Length - 1)
                Dim ddd As String = ""

                ' backup 
                'da.SelectCommand.CommandText = "SELECT m.uid, m.Username[USERNAME],m.emailID[EMAILID],m.lastupdate[Last update],m.userid[Login ID],Dms.ReturnHolidays(m.weeklyHoliday) [WEEKLYHOLIDAY],m.workingStartTime[WORKINGSTARTTIME],m.workingEndTime[WORKINGENDTIME], case isauth when 0 then 'Blocked' when 1 then 'Active' when 100 then 'Inactive' END [STATUS],substring((select ',' + userrole FROM (select  userrole from mmm_mst_user u where eid=" & Session("eid") & " AND UID=M.UID union select rolename from MMM_Ref_Role_User ru where eid=" & Session("EID") & " AND UID=M.UID union select   rolename from MMM_ref_PreRole_user pu where eid=" & Session("eid") & " AND UID=M.UID) AR FOR XML PATH('')),2,200000) as USERROLE, " & fm & " FROM MMM_MST_User m left join MMM_MST_LOCATION L on m.locationId=L.LOCID  where m.Userrole <> 'SU' and m.EID=" & Session("EID") & " "
                da.SelectCommand.CommandText = "SELECT m.uid, m.Username[USERNAME],m.emailID[EMAILID],convert(varchar,m.lastupdate,103) as[Last update],m.userid[Login ID],convert(varchar,m.createdon,103)[Created On],case isauth when 0 then 'Blocked' when 1 then 'Active' when 100 then 'Inactive' END [STATUS],substring((select ',' + userrole FROM (select  userrole from mmm_mst_user u where eid=" & Session("eid") & " AND UID=M.UID union select rolename from MMM_Ref_Role_User ru where eid=" & Session("EID") & " AND UID=M.UID union select   rolename from MMM_ref_PreRole_user pu where eid=" & Session("eid") & " AND UID=M.UID) AR FOR XML PATH('')),2,200000) as USERROLE, " & fm & " ,(SELECT UserName from MMM_Mst_User where uid=m.CreatedBy)  as CreatedBy FROM MMM_MST_User m left join MMM_MST_LOCATION L on m.locationId=L.LOCID  where m.Userrole <> 'SU' and m.EID=" & Session("EID") & " "
                da.Fill(ds, "master")
                ' ViewState("table") = ds.Tables("master")

                'exporting table in excel
                Response.ClearContent()
                Dim gvexp As New GridView
                gvexp.AllowPaging = False

                gvexp.DataSource = ds.Tables("master")
                gvexp.DataBind()

                Response.ContentType = "application/vnd.ms-excel"

                Response.AddHeader("content-disposition", "attachment;filename=UserMaster.xls")

                Dim strwriter As New System.IO.StringWriter
                Dim HtmlTxtWriter As New HtmlTextWriter(strwriter)
                gvexp.RenderControl(HtmlTxtWriter)
                Response.Write(strwriter.ToString())
                Response.End()
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
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        ' Verifies that the control is rendered
    End Sub


    Protected Sub ddlRoleType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRoleType.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        ' for role name bind 

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim sql As String = ""

        If ddlRoleType.SelectedItem.Text = "Pre Type" Then
            sql = "select * from MMM_MST_ROle where eid=" & Session("EID") & " and roletype='Pre Type' and roleName <> 'SU' "
        ElseIf ddlRoleType.SelectedItem.Text = "Post Type" Then
            sql = "select * from MMM_MST_ROle where eid=" & Session("EID") & " and roletype='Post Type' and roleName <> 'SU' "
        End If


        oda.SelectCommand.CommandText = sql
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.Parameters.Clear()

        Dim ds As New DataSet()
        oda.Fill(ds, "Role")
        'ddlUserList.Items.Clear()
        ddlUserRoleName.Items.Clear()
        ddlUserRoleName.DataSource = ds.Tables("Role")
        ddlUserRoleName.DataTextField = "roleName"
        ddlUserRoleName.DataValueField = "roleid"
        ddlUserRoleName.DataBind()
        ddlUserRoleName.Items.Insert(0, "Please Select")

    End Sub



End Class

