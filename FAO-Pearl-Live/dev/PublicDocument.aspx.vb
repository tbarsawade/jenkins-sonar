Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing

Partial Class PublicDocument
    Inherits System.Web.UI.Page


    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If Request.QueryString("public") = "1" Then
            Page.MasterPageFile = "PublicMaster.master"
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
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Session("ALLCHILD") = Nothing
        Session("FNS") = Nothing
        ' If Request.QueryString("SC") Is Nothing Then
        'Else
        If IsNothing(Request.QueryString("eid")) And IsNothing(Session("datetype") = Request.QueryString("date")) And IsNothing(Session("docid") = Request.QueryString("docid")) And IsNothing(Session("docref") = Request.QueryString("docref")) And IsNothing(Session("emailid") = Request.QueryString("emailid")) Then
        Else
            Session("EID") = Request.QueryString("eid")
            Session("pvdoctype") = Request.QueryString("pvdoctype")
            Session("datetype") = Request.QueryString("date")
            Session("docid") = Request.QueryString("docid")
            Session("docref") = Request.QueryString("docref")
            Session("emailid") = Request.QueryString("emailid")

            Dim scrname As String = Request.QueryString("pvdoctype").ToString()
            'Dim str1 As String = Request.UrlReferrer.ToString().Replace(" ", String.Empty) 'document balmiki commment
            'Dim str2 As String = Request.Url.ToString().Replace("+", String.Empty)
            'str2 = str2.Replace(" ", String.Empty)
            'If str1 <> str2 Then
            '    Session("dtNew") = Nothing
            'End If

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & scrname & "' order by displayOrder", con)
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            Page.Title = ds.Tables("data").Rows(0).Item("FormDesc").ToString()
            'Dim messageStrip As String = Session("HEADERSTRIP").ToString()  'document balmiki commment
            'If messageStrip.Length > 1 Then
            '    lblCaption.Text = "<div style=""width:100%;height:47px;padding-top:15px;padding-left:10px;font:bold 17px 'verdana';color:#fff;background-image:url(logo/" & messageStrip & ");background-repeat:no-repeat"">" & ds.Tables("data").Rows(0).Item("Formcaption").ToString() & "</div>"
            'Else
            '    lblCaption.Text = ds.Tables("data").Rows(0).Item("Formcaption").ToString()
            'End If
            Session("Document") = ds.Tables("data").Rows(0).Item("Documenttype").ToString()
            Dim ob As New DynamicForm()
            ob.CreateControlsOnPanel(ds.Tables("data"), pnlFields, UpdatePanel1, btnActEdit, 0, Session("DDL"))
            Dim ROW1() As DataRow = ds.Tables("data").Select("fieldtype='DROP DOWN' and (dropdowntype='MASTER VALUED' OR dropdowntype='CHILD VALUED' OR dropdowntype='SESSION VALUED')  and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    Dim id As String = Right(DDL.ID, DDL.ID.Length - 3)
                    DDL.AutoPostBack = True
                    'Change By V 24 Dec
                    If ds.Tables("data").Rows.Count > 0 Then
                        For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            If ds.Tables("data").Rows(j).Item("KC_LOGIC").ToString.Contains(id) = True Then
                                DDL.AutoPostBack = True
                                AddHandler DDL.TextChanged, AddressOf bindvalue2
                            End If
                        Next
                    End If
                    AddHandler DDL.TextChanged, AddressOf bindvalue
                Next
            End If
            Dim dtchild As DataTable = ds.Tables("data")
            If ds.Tables("data").Rows(0).Item("Iscalendar").ToString() = "1" Then
                Dim btncldr As Button = TryCast(pnlFields.FindControl("BTNCLNDR"), Button)
                AddHandler btncldr.Click, AddressOf ADDTASK
                Dim Grd1 As GridView = TryCast(pnlFields.FindControl("GRDCLNDR"), GridView)
                AddHandler Grd1.RowDataBound, AddressOf addTemplateField
                AddHandler Grd1.RowCommand, AddressOf DeleteTask
                AddHandler Grd1.RowDeleting, AddressOf DeletedTask
                Grd1.DataSource = Session("dtNew")
                Grd1.DataBind()
            End If
            Dim row() As DataRow = dtchild.Select("FieldType='CHILD ITEM'")
            If Session("ALLCHILD") Is Nothing Then
                Session("ALLCHILD") = row
            End If

            If row.Length > 0 Then
                Dim btn1 As New Button
                For i As Integer = 0 To row.Length - 1
                    '' removed frm here by sp 13-jan-14
                    'btn1 = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
                    'AddHandler btn1.Click, AddressOf ShowChildForm
                    Dim PRitem() As String = row(i).Item("dropdown").ToString().Split("-")
                    If PRitem.Length > 1 Then
                        Dim BTN2 As Button = pnlFields.FindControl("BTN" & PRitem(1).ToString & "-" & row(i).Item("FIELDID").ToString())
                        AddHandler BTN2.Click, AddressOf INSERTCHILDITEM
                    End If
                    
                    Session("FNS") = Session("FNS") & PRitem(0).ToString() & ":" & row(i).Item("Fieldid").ToString() & ":"

                    ' Dim array3Ds(,,) As String = New String(,,) {}



                    Dim ColHEAD() As String = {}
                    Dim DDLTXT() As String = {}
                    Dim DDLVAL() As String = {}

                    Session("COLHEAD") = ColHEAD
                    Session("DDLTXT") = DDLTXT
                    Session("DDLVAL") = DDLVAL

                    '' by sunil for def value 16-dec-13 - ends
                    Dim GRD As GridView = pnlFields.FindControl("GRD" & row(i).Item("Fieldid").ToString())
                    ''AddHandler GRD.RowDataBound, AddressOf totalrow  '' moved frm here to below
                    AddHandler GRD.RowCommand, AddressOf Delete
                    AddHandler GRD.RowDeleting, AddressOf Deleting
                    ' AddHandler GRD.RowDataBound, AddressOf addTemplateField


                    '' by sunil for def value insert for Aggrawal  16-dec-13 - starts
                    Dim strDF As String = "select * from mmm_mst_forms where formname='" & PRitem(0).ToString() & "' and formsource='DETAIL FORM' and EID=" & Session("EID").ToString() & " and isnull(HasDefaultValue,0)=1"
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("", con)
                    Dim dtIsdv As New DataTable
                    oda.SelectCommand.CommandText = strDF
                    oda.Fill(dtIsdv)
                    If dtIsdv.Rows.Count = 1 Then '' found hasdefvalue prop. true proceed to display button 
                        ' Dim btnIDV As New Button
                        ' btnIDV = pnlFields.FindControl("BTN_" & row(i).Item("FieldID").ToString())  '  "BTN" & "_" & ROWCHILD(i).Item("FIELDID").ToString()
                        AddHandler GRD.RowDataBound, AddressOf gvData_InlineEdit

                    Else
                        AddHandler GRD.RowDataBound, AddressOf totalrow
                        btn1 = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
                        AddHandler btn1.Click, AddressOf ShowChildForm
                    End If
                    ' oda.Dispose()
                    dtIsdv.Dispose()

                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.CommandText = "uspGetDetailITEM"
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
                    oda.SelectCommand.Parameters.AddWithValue("FN", PRitem(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
                    ds.Tables.Clear()
                    oda.Fill(ds, "ITEM")
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & row(i).Item("dropdown").ToString() & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
                    oda.Fill(ds, "TOTAL")

                    If Not IsPostBack Then
                        Session(PRitem(0)) = Nothing
                    End If

                    If Not Session(PRitem(0)) Is Nothing Then
                        ob.BINDITEMGRID1(Session(PRitem(0)), pnlFields, row(i).Item("fieldid"), UpdatePanel1, ds.Tables("TOTAL"))
                    Else

                    End If
                Next

                If Not Session("CHILD") Is Nothing Then
                    ob.CreateControlsOnPanel(Session("CHILD"), pnlFields1, updpnlchild, Button2, 0, Session("DDL"))
                    Dim ROW2() As DataRow = Session("CHILD").Select("fieldtype='DROP DOWN' and (dropdowntype='MASTER VALUED'  OR dropdowntype='CHILD VALUED')  and lookupvalue is not null")
                    If ROW2.Length > 0 Then
                        For i As Integer = 0 To ROW2.Length - 1
                            Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW2(i).Item("FieldID").ToString()), DropDownList)
                            DDL.AutoPostBack = True
                            AddHandler DDL.TextChanged, AddressOf bindvalue1
                        Next
                    End If
                    ChildFormddlRendering(row, 2)
                End If
            End If
            con.Close()
            oda.Dispose()
            con.Dispose()

        End If
       
        'End If
    End Sub

    '' my running page load
    '' my running 18-dec

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim pid As Integer = Session("docref")
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            'select E.EID,E.logo,D.TID,D.noneditable from MMM_MST_Entity E inner join  MMM_MST_DOC D on E.EID=D.EID where E.eid=42 and D.docref=111871 and D.noneditable=0
            oda.SelectCommand.CommandText = "select E.EID,E.logo,D.* from MMM_MST_Entity E left outer join  MMM_MST_DOC D on E.EID=D.EID where E.eid='" & Session("EID") & "' and D.docref=" & pid & ""
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "logo")
            If ds.Tables("logo").Rows(0).Item("noneditable") = 0 Then
                Response.Redirect("confermation.aspx?saved=1")
                Exit Sub
            Else
                'for expiry of link on date setting
                oda.SelectCommand.CommandText = "select * from MMM_MST_Template_extraFields where PVdoctype='" & Session("pvdoctype") & "' and eid=" & Session("EID") & " "
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "expiryDate")
                oda.SelectCommand.CommandText = "select " & ds.Tables("expiryDate").Rows(0).Item("linkexpiryDate").ToString() & " from MMM_MST_DOC where tid=" & ds.Tables("logo").Rows(0).Item("tid") & ""
                oda.SelectCommand.CommandType = CommandType.Text
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim datestr As String = oda.SelectCommand.ExecuteScalar() ' mm/dd/yy
                Dim datepart As String() = Split(datestr, "/")
                Dim dateofexpiry As Integer = Val(datepart(1) + Val(ds.Tables("expiryDate").Rows(0).Item("expireAfter").ToString()))
                Dim datenow As Date = System.DateTime.Today
                Dim datenowStr As String() = Split(datenow, "/")
                datepart(1) = dateofexpiry
                Dim claculateExpire As String = ""
                For i As Integer = 0 To datepart.Length - 1
                    claculateExpire &= datepart(i) & "/"
                Next
                datestr = Left(claculateExpire, claculateExpire.Length - 1)
                If IsDate(datestr) Then
                    If System.DateTime.Today > Convert.ToDateTime(datestr) Then
                        Response.Redirect("confermation.aspx?saved=2")
                        Exit Sub
                    End If
                End If
            End If

            'Dim date1 As Date = Convert.ToDateTime(datestr)
            'Dim datenow As Date = Date.Now
            'If datenow.Date < date1.Date Then
            'End If
            Dim clogo As String = ds.Tables("logo").Rows(0).Item("logo").ToString()
            lblCaption.Text = "<img width=""190px"" height=""50px"" src=""logo/" & clogo & """ alt=""" & Session("CODE") & """  />"
            oda.SelectCommand.CommandText = "select FF.*,FM.* from MMM_MST_FIELDS FF inner join MMM_MST_DOC M on FF.DocumentType =M.DocumentType INNER JOIN MMM_MST_FORMS FM ON FF.DOCUMENTTYPE=FM.FORMNAME  where FF.EID=" & Session("EID") & " and FM.EID=" & Session("EID") & "  AND M.docRef =" & pid & " and FF.isactive=1  order by displayOrder"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "fields")
            Dim ob As New DynamicForm
            oda.SelectCommand.CommandText = "select * from MMM_MST_DOC where docref=" & pid
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "docRefTid")
            Session("docRefTid") = ds.Tables("docRefTid").Rows(0).Item("TID")
            ob.FillControlsOnPanel(ds.Tables("fields"), pnlFields, "DOCUMENT", Session("docRefTid"))
            Session("FIELDS") = ds.Tables("fields")
            Session("ACTION") = "EDIT"
            Dim dtchild As DataTable = ds.Tables("fields")
            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As SqlConnection = New SqlConnection(conStr)
            Dim row() As DataRow = Session("ALLCHILD")
            If row.Length > 0 Then

                oda.SelectCommand.CommandText = "select * from MMM_MST_DOC where docref=" & pid & ""
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "childDocid")
                Session("childDocid") = ds.Tables("childDocid").Rows(0).Item("tid")
                Dim btn1 As New Button
                For i As Integer = 0 To row.Length - 1
                    ' btn1 = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
                    ' AddHandler btn1.Click, AddressOf ShowChildForm
                    Dim PRitem() As String = row(i).Item("dropdown").ToString().Split("-")

                    Dim strDF As String = "select * from mmm_mst_forms where formname='" & PRitem(0).ToString() & "' and formsource='DETAIL FORM' and EID=" & Session("EID").ToString() & " and isnull(HasDefaultValue,0)=1"
                    Dim dtIsdv As New DataTable
                    oda.SelectCommand.CommandText = strDF
                    oda.Fill(dtIsdv)
                    If dtIsdv.Rows.Count = 1 Then '' found hasdefvalue prop. true proceed to display button 
                        INSERT_DEFAULTVALUES(PRitem(0).ToString, row(i).Item("FieldID").ToString(), ds.Tables("childDocid").Rows(0).Item("tid"))  '  "BTN" & "_" & ROWCHILD(i).Item("FIELDID").ToString()
                    End If
                Next
            End If
            UpdatePanel1.Update()
            oda.Dispose()
            con.Dispose()
        End If
    End Sub

    '' pageload for new dynamic menu will be used later after testing
    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    If Not IsPostBack Then
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As SqlConnection = New SqlConnection(conStr)
    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT username,uid FROM MMM_MST_USER WHERE EID=" & Session("EID").ToString() & "", con)
    '        Dim ds As New DataSet()
    '        oda.Fill(ds, "USER")
    '        For i As Integer = 0 To ds.Tables("USER").Rows.Count - 1
    '            ddluser.Items.Add(ds.Tables("USER").Rows(i).Item("USERNAME").ToString())
    '            ddluser.Items(i).Value = ds.Tables("USER").Rows(i).Item("UID")
    '        Next
    '        ddluser.Items.Insert(0, "SELECT ONE")
    '        UpdPnlAddTask.Update()
    '        Call GetMenuandroles()
    '        '' above is for loading def values from master by sp
    '        Call ShowDefaultValuesinGrid()

    '        oda.Dispose()
    '        con.Dispose()
    '    End If
    'End Sub

    'Private Sub ShowDefaultValuesinGrid()

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim row() As DataRow = Session("ALLCHILD")
    '    If row.Length > 0 Then
    '        Dim btn1 As New Button
    '        For i As Integer = 0 To row.Length - 1
    '            ' btn1 = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
    '            ' AddHandler btn1.Click, AddressOf ShowChildForm
    '            Dim PRitem() As String = row(i).Item("dropdown").ToString().Split("-")

    '            Dim strDF As String = "select * from mmm_mst_forms where formname='" & PRitem(0).ToString() & "' and formsource='DETAIL FORM' and EID=" & Session("EID").ToString() & " and isnull(HasDefaultValue,0)=1"
    '            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '            Dim dtIsdv As New DataTable
    '            oda.SelectCommand.CommandText = strDF
    '            oda.Fill(dtIsdv)
    '            If dtIsdv.Rows.Count = 1 Then '' found hasdefvalue prop. true proceed to display button 
    '                ' INSERT_DEFAULTVALUES(PRitem(0).ToString, row(i).Item("FieldID").ToString())  '  "BTN" & "_" & ROWCHILD(i).Item("FIELDID").ToString()
    '                Dim GRD As GridView = pnlFields.FindControl("GRD" & row(i).Item("Fieldid").ToString())
    '                AddHandler GRD.RowDataBound, AddressOf totalrow
    '                AddHandler GRD.RowCommand, AddressOf Delete
    '                AddHandler GRD.RowDeleting, AddressOf Deleting
    '                oda.SelectCommand.Parameters.Clear()
    '                oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '                oda.SelectCommand.CommandText = "uspGetDetailITEMBYDOCID"
    '                oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '                oda.SelectCommand.Parameters.AddWithValue("DOCID", Session("docref"))
    '                oda.SelectCommand.Parameters.AddWithValue("FN", PRitem(0).ToString())
    '                oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
    '                ds.Tables.Clear()
    '                oda.Fill(ds, "ITEM")
    '                oda.SelectCommand.CommandType = CommandType.Text
    '                oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid) inner join MMM_MST_DOC D ON F1.DOCUMENTTYPE=D.DOCUMENTTYPE  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & row(i).Item("dropdown").ToString() & "' AND D.TID=" & pid & ""
    '                oda.Fill(ds, "TOTAL")
    '                ob.BINDITEMGRID(ds.Tables("ITEM"), pnlFields, btn1.ID, UpdatePanel1, ds.Tables("TOTAL"))


    '            End If
    '        Next
    '    End If
    '    ' oda.Dispose()
    '    ' dtIsdv.Dispose()
    'End Sub

    Public Sub INSERT_DEFAULTVALUES(ByVal FN As String, ByVal fldID As String, ByVal chidDocID As Integer)
        '' new created for inserting def. values from master table in document on 17-Dec-13 by sunil pareek
        'Dim btnDetails As Button = TryCast(SENDER, Button)
        Dim formname As String = FN
        Dim FormnameDVM As String = ""
        'formname = Right(formname, formname.Length - 8).Trim()
        Session("FN") = formname
        ViewState("FN_DVM") = formname & "_MASTER"
        FormnameDVM = formname & "_MASTER"
        'Dim ID() As String = btnDetails.ID.Split("_")
        Session("ID") = fldID.ToString  'ID(1).ToString
        ViewState("ID") = "BTN" & fldID.ToString  'ID(1).ToString

        Dim scrname As String = Request.QueryString("pvdoctype").ToString()
        Dim ob As New DynamicForm
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim DS As New DataSet

        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & formname & "' order by displayOrder", con)
        oda.Fill(DS, "CHILD")
        Session("CHILD") = DS.Tables("CHILD")
        Session("D" & formname) = DS.Tables("CHILD")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim DefValMastDoctype As String = ""
        Dim CdocType As String = ""
        DS.Tables.Clear()
        ' Dim ddl As DropDownList = TryCast(pnlFields.FindControl("fld" & ID(0).ToString()), DropDownList)
        ' Dim docid() As String = ddl.SelectedValue.ToString.Split("|")
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "uspGetDetailITEMBYDOCID"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("DOCID", chidDocID)
        oda.SelectCommand.Parameters.AddWithValue("FN", FN)
        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        DS.Tables.Clear()
        oda.Fill(DS, "ITEM")
        Session("ITEM") = DS.Tables("ITEM")
        oda.Fill(DS, "ITEM_VALS")
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        oda.Fill(DS, "TOTAL")

        '' WRITE here func that will add row by row values in grid ' write new function by sunil
        ADD_DV_ITEMSTOGRID(formname, DS.Tables("ITEM"), DS.Tables("ITEM_VALS"))

        '' arrays for holding master valued column data 
        'Dim array3Ds(,,) As String = New String(,,) {}

        Dim ColHead() As String = {}
        Dim DDLTxt() As String = {}
        Dim DDLval() As String = {}
        Dim aCnt As Integer = 0

        Dim dtC As DataTable = Session("D" & formname) 'DS.Tables("CHILD")

        For j = 0 To dtC.Rows.Count - 1
            If dtC.Rows(j).Item("fieldtype").ToString = "DROP DOWN" And dtC.Rows(j).Item("DROPDOWNTYPE") = "MASTER VALUED" Then
                For i As Integer = 0 To DS.Tables("item").Rows.Count - 1
                    For k As Integer = 0 To DS.Tables("item").Columns.Count - 1
                        If DS.Tables("item").Columns(k).ColumnName = dtC.Rows(j).Item("displayname").ToString Then  '' matched col. (mast. val.) 
                            ColHead(aCnt) = formname & "_" & dtC.Rows(j).Item("displayname").ToString
                            DDLTxt(aCnt) = DS.Tables("item").Rows(i).Item(DS.Tables("item").Columns(k).ColumnName).ToString
                            DDLval(aCnt) = DS.Tables("ITEM_VALS").Rows(i).Item(DS.Tables("item").Columns(k).ColumnName).ToString
                        End If
                    Next
                Next
            End If
        Next

        Session("COLHEAD") = ColHead
        Session("DDLTXT") = DDLTxt
        Session("DDLVAL") = DDLval

        dtC.Dispose()
        oda.Dispose()
        DS.Dispose()
        con.Dispose()
    End Sub

    Private Sub GetMenuandroles()
        Dim screen As String = Request.QueryString("SC").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        ' Dim scrname As String = Request.QueryString("SC").ToString()
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        'Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
        da.SelectCommand.CommandText = "select * from mmm_mst_menu where eid=" & Session("EID") & " and menuname='" & screen & "'"
        da.Fill(ds, "menu")
        Dim rol As String() = ds.Tables("menu").Rows(0).Item("Roles").ToString().Split(",")

        For j As Integer = 0 To rol.Length - 1
            Dim a As String() = rol(j).ToString().Split(":")
            If Session("USERROLE") = a(0).Remove(0, 1).ToString() Then
                ViewState("numval") = a(1).Remove(a(1).Length - 1)
            End If
        Next
        If ViewState("numval") = 1 Then
            btnActEdit.Visible = False
        ElseIf ViewState("numval") = 4 Then
            btnActEdit.Visible = False
        ElseIf ViewState("numval") = 5 Then
            btnActEdit.Visible = False
        ElseIf ViewState("numval") = 8 Then
            btnActEdit.Visible = False
        ElseIf ViewState("numval") = 9 Then
            btnActEdit.Visible = False
        ElseIf ViewState("numval") = 12 Then
            btnActEdit.Visible = False
        ElseIf ViewState("numval") = 13 Then
            btnActEdit.Visible = False
        End If


        con.Close()
        da.Dispose()
    End Sub

    Private Sub GetMenuData()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim scrname As String = Request.QueryString("SC").ToString()
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim cr As String = Session("CODE") & "_" & Session("USERROLE")
        da.SelectCommand.CommandText = "select tid,menuname,menutype, " & cr & " from mmm_mst_accessmenu where menutype='dynamic' and menuname='Document'"
        da.Fill(ds, "menu")
        For i As Integer = 0 To ds.Tables("menu").Rows.Count - 1

            Dim abc As String = ds.Tables("menu").Rows(i).Item(cr).ToString()
            Dim a1 As String() = abc.ToString().Split(",")
            For c As Integer = 0 To a1.Length - 1
                Dim b1 As String() = a1(c).ToString().Split("-")

                If b1(0).ToString = scrname Then
                    Dim ab As String() = b1(1).ToString().Split(":")
                    If ab(0).Length > 0 Then
                        ViewState("numval") = ab(1).ToString()
                    End If
                End If
            Next

        Next
        con.Close()
        da.Dispose()
    End Sub

    Protected Sub gvData_InlineEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Try
            Dim fn As String = ""
            'Dim dtFld1 As DataTable = ViewState(fn) ' Session(fn)
            Dim dtfld1 As DataTable '= ViewState(fn)

            Dim row As GridViewRow = e.Row

            Dim fns() As String = Session("FNS").ToString.Split(":")
            'Dim GV As GridView = 
            Dim GID As String = row.Parent.Parent.ID
            GID = Right(GID, GID.Length - 3)

            '' new added for hiding tid - on 27 dec 8.30 pm by sp
            e.Row.Cells(e.Row.Cells.Count - 1).Visible = False

            For I As Integer = 0 To fns.Length - 1
                If fns(I).ToString = GID.ToString Then
                    fn = fns(I - 1).ToString
                    Exit For
                End If
            Next
            dtfld1 = Session("D" & fn)

            If row.RowType = DataControlRowType.DataRow Then
                ' For j As Integer = 0 To row.Cells.Count - 1

                If row.Cells(0).Text.ToUpper <> "TOTAL" Then
                    For j As Integer = 0 To dtfld1.Rows.Count - 1
                        '  Dim strVal As String = CType(e.Row.DataItem, DataRowView)(dtfld1.Columns(j).ColumnName).ToString
                        Dim ftype As String = dtfld1.Rows(j).Item("fieldtype").ToString()
                        Dim ilEdit As Integer = dtfld1.Rows(j).Item("inlineEditing").ToString()
                        Dim FldID As String = dtfld1.Rows(j).Item("fieldid").ToString()
                        Dim Formula = Convert.ToString(dtfld1.Rows(j).Item("cal_text"))
                        Dim DisplayName = Convert.ToString(dtfld1.Rows(j).Item("DisplayName"))

                        If ilEdit = 1 Then
                            If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Then
                                Dim cb As New TextBox
                                'cb.ID = "fld" & j.ToString() & "_" & row.RowIndex
                                cb.ID = "fld" & FldID & row.RowIndex
                                Dim colValue As String = row.DataItem(j).ToString()
                                cb.Text = colValue
                                row.Cells(j).Controls.Add(cb)
                                'Code For calculative field
                                'Code End Here !!!!!!
                                If ftype = "Calculative Field" Then
                                    cb.Attributes.Add("READONLY", "READONLY")
                                    cb.Attributes.Add("COLOR", "GRAY")
                                    ' cb.ReadOnly = True
                                End If

                                If Val(colValue) = 0 Then
                                    If ftype.ToUpper() = "TEXT BOX" And dtfld1.Rows(j).Item("datatype").ToString().ToUpper() = "NUMERIC" Then
                                        cb.Text = "0"
                                    End If
                                End If

                                If Not Formula Is Nothing And Formula <> "" Then
                                    'Dim arrFor As String() = Formula.Split(",")
                                    Dim jScript = GenerateJQueryScript(dtfld1, GID, row.RowIndex, Formula, FldID)
                                    If jScript <> "" Then
                                        cb.Attributes.Add("onblur", jScript)
                                    End If
                                End If
                            ElseIf ftype.ToUpper() = "DROP DOWN" Then
                                Dim ddl As New DropDownList
                                ddl.ID = "fld" & FldID & row.RowIndex
                                ddl.CssClass = "txtBox"
                                Dim ddlText As String = dtfld1.Rows(j).Item("dropdown").ToString()
                                Dim dropdowntype As String = dtfld1.Rows(j).Item("dropdowntype").ToString()
                                Dim arr() As String
                                Dim arrMid() As String
                                If dtfld1.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then
                                    Dim cb As New DropDownList
                                    cb.ID = "fld" & FldID & row.RowIndex
                                    Dim ARR1() As String = dtfld1.Rows(j).Item("dropdown").ToString().Split(",")
                                    For K As Integer = 0 To ARR1.Length - 1
                                        cb.Items.Add(ARR1(K).ToString)
                                    Next
                                    row.Cells(j).Controls.Add(cb)
                                ElseIf dtfld1.Rows(j).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                                    '' code for getting master valued 
                                    Dim ob As New DynamicForm
                                    arr = ddlText.Split("-")
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
                                    Dim lookUpqry As String = ""
                                    Dim str As String = ""
                                    If arr(0).ToUpper() = "CHILD" Then
                                        str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                        str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    Else
                                        If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                            str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                        Else
                                            str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                        End If
                                    End If

                                    Dim xwhr As String = ""
                                    Dim tids As String = ""
                                    'Dim tidarr() As String

                                    ''FILTER THE DATA ACCORDING TO USER 
                                    tids = ob.UserDataFilter(dtfld1.Rows(j).Item("documenttype").ToString(), arr(1).ToString())

                                    If tids.Length >= 2 Then
                                        'tidarr = tids.Split("-")
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                    ElseIf tids = "0" Then
                                        pnlFields.Visible = False
                                        btnActEdit.Visible = False
                                        UpdatePanel1.Update()
                                        xwhr = ""
                                    End If
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                    Dim AutoFilter As String = dtfld1.Rows(j).Item("AutoFilter").ToString()
                                    If AutoFilter.Length > 0 Then
                                        If arr(0).ToUpper() = "CHILD" Then
                                            If AutoFilter.ToUpper = "DOCID" Then
                                                str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                                            Else
                                                str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                                            End If
                                        ElseIf arr(0).ToUpper() <> "STATIC" Then
                                            str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                            str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                        Else
                                            str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & Session("EID") & " "
                                            str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                        End If
                                    End If

                                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                    Dim con As SqlConnection = New SqlConnection(conStr)
                                    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                    Dim dss As New DataSet

                                    If str.Length > 0 Then
                                        oda.SelectCommand.CommandText = str
                                        oda.Fill(dss, "FV")
                                        Dim isAddJquery As Integer = 0
                                        ddl.Items.Add("Select")
                                        ddl.Items(0).Value = "0"
                                        For J1 As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                            ddl.Items.Add(dss.Tables("FV").Rows(J1).Item(0).ToString())
                                            Dim lookddlVal As String = dss.Tables("FV").Rows(J1).Item(1).ToString()
                                            ddl.Items(J1 + 1).Value = lookddlVal
                                        Next
                                        oda.Dispose()
                                        dss.Dispose()
                                        If isAddJquery = 1 Then
                                            Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                        End If
                                    End If
                                    con.Dispose()
                                    oda.Dispose()
                                    dss.Dispose()
                                    '' ends here for getting master valued 
                                End If

                            ElseIf ftype.ToUpper() = "FILE UPLOADER" Then
                                Dim txtBox As New FileUpload
                                txtBox.ID = "fld" & FldID & row.RowIndex
                                txtBox.CssClass = "txtBox"
                                row.Cells(j).Controls.Add(txtBox)
                                Dim pstback As New PostBackTrigger
                                pstback.ControlID = btnActEdit.ID
                                UpdatePanel1.Triggers.Add(pstback)
                            End If
                        Else '  not inline editng
                            ' If ftype.ToUpper() = "DROP DOWN" And dtfld1.Rows(j).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                            '' holding
                            'ColHEAD(0) = dtfld1.Rows(j).Item("displayname").ToString()

                            'End If
                        End If
                    Next
                End If
            Else

            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function GenerateJQueryScript(ByVal dt As DataTable, ByVal GridID As String, ByVal RowID As Integer, ByVal Formula As String, ByVal FieldID As Integer) As String
        Dim result = ""
        Try
            Dim CurRow As DataRow() = dt.Select("FieldID='" & FieldID & "'")
            Dim DisplayName = Convert.ToString(CurRow(0).Item("DisplayName"))
            Dim strFormula = ""
            'appending , in case when only one formula exists
            Formula = "," & Formula
            Dim FormulaField = ""
            'Formula = Formula.Remove("{", "").Remove("}", "")
            'Spliting all the formula with ,
            Dim arrFor As String() = Formula.Split(",")
            Dim liverFormula = From fldMapping In arrFor Where fldMapping <> "" And Not fldMapping Is Nothing Select fldMapping
            For Each Formula1 In liverFormula
                Dim arr As String() = Formula1.Split("=")
                FormulaField = arr(0).Replace("{", "").Replace("}", "")
                strFormula = arr(1)
                If arr(1).Contains("{" & DisplayName & "}") Then
                    Dim str As String() = arr(1).Split("+", "-", "*", "/", "%")
                    For Each str1 In str
                        str1 = str1.Replace("{", "").Replace("}", "")
                        Dim DR As DataRow() = dt.Select("DisplayName='" & str1 & "'")
                        If strFormula.Contains("{" & DR(0).Item("DisplayName") & "}") Then
                            strFormula = strFormula.Replace("{" & DR(0).Item("DisplayName") & "}", "parseFloat($('#ContentPlaceHolder1_GRD" & GridID & "_fld" & DR(0).Item("FieldID") & RowID & "_" & RowID & "').val())")
                        End If
                    Next
                    Dim DR1 As DataRow() = dt.Select("DisplayName='" & FormulaField & "'")
                    strFormula = "parseFloat($('#ContentPlaceHolder1_GRD" & GridID & "_fld" & DR1(0).Item("FieldID") & RowID & "_" & RowID & "').val(" & strFormula & "))"
                    If result = "" Then
                        result = strFormula
                    Else
                        result = result & ";" & strFormula
                    End If
                End If
            Next
        Catch ex As Exception
            Return ""
        End Try
        Return result
    End Function

    Public Sub bindvalue(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlFields, ddl)
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

    'Change By V 24 Dec
    Public Sub bindvalue2(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Session("DDL") = ddl
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlFields, ddl)
    End Sub

    Public Sub bind(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef ddl As DropDownList)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Dim DS As New DataSet
        oda.Fill(DS, "data")
        Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
        Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
        If LOOKUPVALUE.Length > 0 Then
            Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")
            If lookfld.Length > 0 Then
                For iLookFld As Integer = 0 To lookfld.Length - 1
                    Dim fldPair() As String = lookfld(iLookFld).Split("-")
                    If fldPair.Length > 1 Then
                        oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)
                        oda.Fill(DS, "FIELD")
                        Dim STR As String = ""
                        Dim DROPDOWN As String() = DS.Tables("FIELD").Rows(0).Item("DROPDOWN").ToString.Split("-")
                        Dim TABLENAME As String = ""
                        Dim TID As String = "TID"
                        If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                            TABLENAME = "MMM_MST_MASTER"
                        ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                            TABLENAME = "MMM_MST_DOC"
                        ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                            TABLENAME = "MMM_MST_USER"
                            TID = "UID"
                        Else
                            TABLENAME = DS.Tables("FIELD").Rows(0).Item("DBTABLENAME").ToString
                        End If
                        Dim SLVALUE As String() = ddl.SelectedValue.Split("|")
                        If DS.Tables("FIELD").Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then
                            Dim AUTOFILTER As String = DS.Tables("FIELD").Rows(0).Item("AUTOFILTER").ToString()
                            If TABLENAME <> "MMM_MST_USER" Then
                                STR = "select " & DROPDOWN(2).ToString() & ",convert(nvarchar(10),tid) [tid] from " & TABLENAME & " P WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & DROPDOWN(1).ToString() & "' AND " & AUTOFILTER & "=" & SLVALUE(0) & "  "
                            Else
                                STR = "select " & DROPDOWN(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid] from " & TABLENAME & " P WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & AUTOFILTER & "=" & SLVALUE(0) & " "
                            End If
                            oda.SelectCommand.CommandText = STR
                            oda.Fill(DS, "final")

                            Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                            For i As Integer = 0 To DS.Tables("final").Rows.Count - 1
                                ddlo.Items.Add(DS.Tables("final").Rows(i).Item(0).ToString())
                                ddlo.Items(i).Value = DS.Tables("final").Rows(i).Item("tID")
                            Next
                        Else
                            oda = New SqlDataAdapter("", con)
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.CommandText = "uspGetMasterValue"
                            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
                            oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                            oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                            oda.SelectCommand.Parameters.AddWithValue("TID", ddl.SelectedValue)
                            oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim value As String = oda.SelectCommand.ExecuteScalar()
                            Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), TextBox)
                            TXTBOX.Text = value
                        End If
                    End If
                Next
            End If
        End If
        con.Dispose()
        oda.Dispose()
    End Sub

    'Protected Sub ValidateData()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim screenname As String = Request.QueryString("SC").ToString()
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & Session("EID").ToString() & " and FormName = '" & screenname & "' order by displayOrder", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "fields")
    '    Dim ob As New DynamicForm
    '    Dim FinalQry As String
    '    Dim msgAN As String = ""
    '    Dim DocID As Integer = 0
    '    FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields, 0)
    '    If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
    '        lblTab.Text = FinalQry
    '    Else
    '        FinalQry = FinalQry & ";Select @@identity"
    '        'save the data
    '        lblTab.Text = ""
    '        da.SelectCommand.CommandText = FinalQry
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim fileid As Integer = da.SelectCommand.ExecuteScalar()
    '        DocID = fileid
    '        Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
    '        If row.Length > 0 Then
    '            da.SelectCommand.Parameters.Clear()
    '            da.SelectCommand.CommandText = "usp_GetAutoNoNew"
    '            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '            da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
    '            da.SelectCommand.Parameters.AddWithValue("docid", fileid)
    '            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
    '            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '            Dim an As String = da.SelectCommand.ExecuteScalar()
    '            msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
    '            da.SelectCommand.Parameters.Clear()
    '        End If


    '        ''commented by Ankit 
    '        'da.SelectCommand.CommandType = CommandType.Text
    '        'da.SelectCommand.CommandText = "UPDATE MMM_MST_DOC_ITEM SET DOCID =" & fileid & " WHERE SESSIONID='" & Session.SessionID & "' AND DOCID IS NULL "
    '        'If con.State <> ConnectionState.Open Then
    '        '    con.Open()
    '        'End If
    '        'da.SelectCommand.ExecuteNonQuery()

    '        ''Functionality to save child item with Docid rather than Sessionid
    '        Dim childitem() As DataRow = ds.Tables("fields").Select("Fieldtype='CHILD ITEM'")
    '        If childitem.Length > 0 Then
    '            For Each DR As DataRow In childitem
    '                SavingChildItem(DR.Item("DROPDOWN"), fileid)
    '            Next
    '        End If

    '        Dim ob1 As New DMSUtil()
    '        '''' disabled - used earlier b4 new rolematrix... by sunil
    '        'ob1.ApplyDynamicAuthMatrixNew(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()))

    '        ' by rajat bansal
    '        Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
    '        Dim viewdoc As String = screenname
    '        viewdoc = viewdoc.Replace(" ", "_")
    '        If CalculativeField.Length > 0 Then
    '            For Each CField As DataRow In CalculativeField
    '                Dim formulaeditorr As New formulaEditor
    '                Dim forvalue As String = String.Empty
    '                forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), DocID, "v" + Session("eid").ToString + viewdoc)
    '                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & DocID & ""
    '                da.SelectCommand.CommandText = upquery
    '                da.SelectCommand.CommandType = CommandType.Text
    '                da.SelectCommand.ExecuteNonQuery()
    '            Next
    '        End If

    '        '' insert default fiest movement of document - by sunil
    '        da.SelectCommand.CommandText = "InsertDefaultMovement"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.Parameters.AddWithValue("tid", fileid)
    '        da.SelectCommand.Parameters.AddWithValue("CUID", Val(Session("UID").ToString()))
    '        da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        da.SelectCommand.ExecuteNonQuery()

    '        '' here code to approve document  - by sunil
    '        Dim res As String
    '        res = ob1.GetNextUserFromRolematrix(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()))
    '        '' here code to approve document  - by sunil

    '        Dim sretMsgArr() As String = res.Split(":")

    '        If ds.Tables("fields").Rows(0).Item("iscalendar").ToString = "1" And Session("dtnew") <> Nothing Then
    '            ADDTASK(fileid, screenname)
    '        End If
    '        ob.CLEARDYNAMICFIELDS(pnlFields)

    '        '' code to send mail to first approver on document creation and to document owner also
    '        '' new added by sunil for mail sending 
    '        ob1.TemplateCalling(fileid, Session("EID"), screenname, "CREATED")

    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & fileid & " and eid='" & Session("EID") & "'"
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        If dt.Rows.Count = 1 Then
    '            If dt.Rows(0).Item(0).ToString = "1" Then
    '                ob1.TemplateCalling(fileid, Session("EID"), screenname, "APPROVE")
    '            End If
    '        End If

    '        ''INSERT INTO HISTORY 
    '        ob.History(Session("EID"), fileid, Session("UID"), screenname, "MMM_MST_DOC", "ADD")

    '        '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
    '        If sretMsgArr(0).ToUpper() = "NO SKIP" Then
    '            Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
    '            lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
    '        Else
    '            lblMsg.Text = "System Docid is " & fileid & " " & msgAN & ""
    '        End If
    '        Try
    '            Dim FormName As String = screenname
    '            Dim EID As Integer = 0
    '            EID = Convert.ToInt32(Session("EID"))
    '            If (DocID > 0) And (FormName <> "") Then
    '                Trigger.ExecuteTrigger(FormName, EID, DocID)
    '            End If
    '        Catch ex As Exception
    '        End Try
    '        '' by rajat bansal 
    '        'Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
    '        'Dim viewdoc As String = screenname
    '        'viewdoc = viewdoc.Replace(" ", "_")
    '        'If CalculativeField.Length > 0 Then
    '        '    For Each CField As DataRow In CalculativeField
    '        '        Dim formulaeditorr As New formulaEditor
    '        '        Dim forvalue As String = String.Empty
    '        '        forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), DocID, "v" + Session("eid").ToString + viewdoc)
    '        '        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & DocID & ""
    '        '        da.SelectCommand.CommandText = upquery
    '        '        da.SelectCommand.CommandType = CommandType.Text
    '        '        da.SelectCommand.ExecuteNonQuery()
    '        '    Next
    '        'End If
    '        updMsg.Update()
    '        btnForm_ModalPopupExtender.Show()
    '    End If
    '    da.Dispose()
    '    con.Dispose()
    'End Sub

    Protected Sub ValidateData()

        Dim docref As Integer = Request.QueryString("docref")
        Dim docid As Integer = Request.QueryString("docid")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim screenname As String = Request.QueryString("pvdoctype").ToString()
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & Session("EID").ToString() & " and FormName = '" & screenname & "' order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "fields")
        Dim ob As New DynamicForm
        Dim FinalQry As String
        Dim msgAN As String = ""
        Dim childvalidation As Integer = 0
        'Dim DocID As Integer = 0
        FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET adate=getdate(),", "", ds.Tables("fields"), pnlFields, Session("docRefTid"))
        'FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields, 0)
        If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
            lblTab.Text = FinalQry
        Else
            ' validating child item by balli 
            Dim validatechilditem() As DataRow = ds.Tables("fields").Select("Fieldtype='CHILD ITEM'")
            If validatechilditem.Length > 0 Then
                For Each DR As DataRow In validatechilditem
                    '' new added for saving differently if def. value feature is on
                    Dim strDF As String = "select * from mmm_mst_forms where formname='" & DR.Item("DROPDOWN") & "' and formsource='DETAIL FORM' and EID=" & Session("EID") & " and isnull(HasDefaultValue,0)=1"
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                    Dim dtIsdv As New DataTable
                    oda.SelectCommand.CommandText = strDF
                    oda.Fill(dtIsdv)
                    childvalidation = ValidatingChildItem_DV(DR.Item("DROPDOWN"))
                    If childvalidation = 1 Then
                        lblTab.Text = "Entered child value data should be greater than 0"
                        Exit Sub
                    End If
                    oda.Dispose()
                Next
            End If
            '' child item vakidation end here
            lblTab.Text = ""
            da.SelectCommand.CommandText = FinalQry & "where tid = " & Session("docRefTid")
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            Dim childitem() As DataRow = ds.Tables("fields").Select("Fieldtype='CHILD ITEM'")
            If childitem.Length > 0 Then
                For Each DR As DataRow In childitem
                    SavingChildItem_DV(DR.Item("DROPDOWN"), Session("childDocid"))
                    Try
                        Dim FormName As String = DR.Item("DROPDOWN")
                        Dim EID As Integer = 0
                        EID = Convert.ToInt32(Session("EID"))
                        If (Session("childDocid") > 0) And (FormName <> "") Then
                            'Last parameter is for child item
                            Trigger.ExecuteTrigger(screenname, EID, Session("docRefTid"), 1)
                        End If
                    Catch ex As Exception
                    End Try
                Next

            End If
            Dim ob1 As New DMSUtil()
            '''' disabled - used earlier b4 new rolematrix... by sunil
            'ob1.ApplyDynamicAuthMatrixNew(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()))

            ' by rajat bansal
            Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
            Dim viewdoc As String = screenname
            viewdoc = viewdoc.Replace(" ", "_")
            If CalculativeField.Length > 0 Then
                For Each CField As DataRow In CalculativeField
                    Dim formulaeditorr As New formulaEditor
                    Dim forvalue As String = String.Empty
                    forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), docid, "v" + Session("eid").ToString + viewdoc, Session("eid").ToString, 0)
                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & docid & ""
                    da.SelectCommand.CommandText = upquery
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.ExecuteNonQuery()
                Next
            End If

            '' insert default fiest movement of document - by sunil
            'da.SelectCommand.CommandText = "InsertDefaultMovement"
            'da.SelectCommand.CommandType = CommandType.StoredProcedure
            'da.SelectCommand.Parameters.Clear()
            'da.SelectCommand.Parameters.AddWithValue("tid", Session("childDocid"))
            'da.SelectCommand.Parameters.AddWithValue("CUID", Val(Session("UID").ToString()))
            'da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'da.SelectCommand.ExecuteNonQuery()

            '' here code to approve document  - by sunil
            'Dim res As String
            'res = ob1.GetNextUserFromRolematrix(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()))
            ' '' here code to approve document  - by sunil

            'Dim sretMsgArr() As String = res.Split(":")

            'If ds.Tables("fields").Rows(0).Item("iscalendar").ToString = "1" And Session("dtnew") <> Nothing Then
            '    ADDTASK(fileid, screenname)
            'End If
            ob.CLEARDYNAMICFIELDS(pnlFields)

            '' code to send mail to first approver on document creation and to document owner also
            '' new added by sunil for mail sending 
            '' temp disabled for testing
            ob1.TemplateCalling(Session("childDocid"), Session("EID"), screenname, "CREATED")
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & Session("childDocid") & " and eid='" & Session("EID") & "'"
            Dim dt As New DataTable
            da.Fill(dt)
            If dt.Rows.Count = 1 Then
                If dt.Rows(0).Item(0).ToString = "1" Then
                    ob1.TemplateCalling(Session("childDocid"), Session("EID"), screenname, "APPROVE")
                End If
            End If
            ''INSERT INTO HISTORY 
            ob.History(Session("EID"), Session("childDocid"), Session("UID"), screenname, "MMM_MST_DOC", "ADD")
            '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
            'If sretMsgArr(0).ToUpper() = "NO SKIP" Then
            '    Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
            '    lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
            'Else
            '    lblMsg.Text = "System Docid is " & fileid & " " & msgAN & ""
            'End If
            Try
                Dim FormName As String = screenname
                Dim EID As Integer = 0
                EID = Convert.ToInt32(Session("EID"))
                If (docid > 0) And (FormName <> "") Then
                    Trigger.ExecuteTrigger(FormName, EID, docid)
                End If
            Catch ex As Exception
            End Try
            '' by rajat bansal 
            'Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
            'Dim viewdoc As String = screenname
            'viewdoc = viewdoc.Replace(" ", "_")
            'If CalculativeField.Length > 0 Then
            '    For Each CField As DataRow In CalculativeField
            '        Dim formulaeditorr As New formulaEditor
            '        Dim forvalue As String = String.Empty
            '        forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), DocID, "v" + Session("eid").ToString + viewdoc)
            '        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & DocID & ""
            '        da.SelectCommand.CommandText = upquery
            '        da.SelectCommand.CommandType = CommandType.Text
            '        da.SelectCommand.ExecuteNonQuery()
            '    Next
            'End If
            lblMsg.Text = " Form is saved ,Please close the window"
            updMsg.Update()
            btnForm_ModalPopupExtender.Show()
        End If
        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub btnActEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        MP_Reconfirm.Show()
    End Sub

    Protected Sub ConfirmedData(ByVal sender As Object, ByVal e As EventArgs)
        ValidateData()
    End Sub
    Protected Sub CancelConfirmedData(ByVal sender As Object, ByVal e As EventArgs)
        MP_Reconfirm.Hide()
    End Sub

    Public Sub ShowChildForm(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim btnDetails As Button = TryCast(sender, Button)
        Dim formname As String = btnDetails.Text
        formname = Right(formname, formname.Length - 5).Trim()
        Session("FN") = formname
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
            '' new added by sunil on 26_dec 2.34 pm
            Session("D" & formname) = DS.Tables("CHILD")
            pnlFields1.Controls.Clear()
            ob.CreateControlsOnPanel(DS.Tables("CHILD"), pnlFields1, updpnlchild, Button2, 0, Session("DDL"))
            Dim ROW1() As DataRow = DS.Tables("CHILD").Select("fieldtype='DROP DOWN' and (dropdowntype='MASTER VALUED' OR dropdowntype='CHILD VALUED') and lookupvalue is not null")
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
        lblChildFormName.Visible = True
        lblChildFormName.Text = "ADD  " & Session("FN")
        updChildFormname.Update()
        updpnlchild.Update()
        Button2.Text = "Save"
        ModalPopupExtender1.Show()
    End Sub

    '' not in use as on 18-12-13
    Protected Sub ValidateChildData(ByVal actionType As String)
        'Check All Validations
        ' now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim screenname As String = Session("FN")
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
            lblTab1.Text = FinalQry
        Else
            If actionType <> "ADD" Then
                FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
            End If
            'save the data
            lblTab1.Text = ""
            da.SelectCommand.CommandText = FinalQry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            BINDGRID()
            Session("CHILD") = Nothing
            ModalPopupExtender1.Hide()
        End If
        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub EditItem(ByVal sender As Object, ByVal e As System.EventArgs)
        'If Button2.Text = "Save" Then
        '    ValidateChildData("ADD")
        'Else
        '    ValidateChildData("EDIT")
        'End If
        If Button2.Text = "Save" Then
            ADDITEMTOGRID(Session("FN"))
        Else
            SavingChildItemOnEdit(Session("FN"))
        End If
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
        ODA.SelectCommand.Parameters.AddWithValue("FN", Session("FN"))
        ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        DS.Tables.Clear()
        ODA.Fill(DS, "ITEM")
        Dim dt_item As DataTable = New DataTable()
        dt_item = DS.Tables("ITEM")
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & Session("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        ODA.Fill(DS, "TOTAL")
        ODA.Dispose()
        DS.Dispose()
        Dim OB As New DynamicForm
        OB.BINDITEMGRID(dt_item, pnlFields, ViewState("ID"), UpdatePanel1, DS.Tables("TOTAL"))
    End Sub
    '' you here on 17-dec-13 at 08.41 pm 
    Protected Sub ADD_DV_ITEMSTOGRID(ByVal FORMNAME As String, ByVal DtDV As DataTable, ByVal DtDV_Vals As DataTable)
        Dim dtFD As New DataTable
        Dim dtField As New DataTable
        Dim DTVALUE As New DataTable
        Dim errormsg As String = "Please Enter "
        dtField = ViewState(FORMNAME)
        ' Dim OB As New DynamicForm()

        If Session(FORMNAME) Is Nothing Then
            ' For Each dr As DataRow In dtField.Rows
            ' dtFD.Columns.Add(dr.Item("displayname"), GetType(String))
            ' DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
            ' Next
            dtFD = DtDV
            DTVALUE = DtDV_Vals
            'dtFD.Columns.Add("tid", GetType(String))
        Else
            dtFD = Session(FORMNAME)
            DTVALUE = Session(FORMNAME & "VAL")
        End If

        ''Check Form Level Validation
        'If dtField.Rows.Count > 0 Then
        '    Dim str As String = OB.validateForm(dtField.Rows(0).Item("Documenttype").ToString, Session("EID"), pnlFields, dtField, "ADD", 0)
        '    If str.Length > 5 Then
        '        str = "Please " & str
        '        Label3.Text = str
        '        Exit Sub
        '    End If
        'End If

        '' Remove the Total Row from Datatable
        ' If dtFD.Rows.Count > 1 Then
        ' dtFD.Rows.RemoveAt(dtFD.Rows.Count - 1)
        ' End If

        ' drnew.Item("tid") = FORMNAME & "-" & dtFD.Rows.Count & "-" & ViewState("ID")
        'For Each dr As DataRow In dtFD.Rows
        '    dr.Item("tid") = "" ' FORMNAME & "-" & dtFD.Rows.Count & "-" & ViewState("ID")
        'Next

        ' For Each dr As DataRow In DTVALUE.Rows
        ' dr.Item("tid") = FORMNAME & "-" & DTVALUE.Rows.Count & "-" & ViewState("ID")
        ' Next

        ' dtFD.Rows.Add(drnew)
        ' DTVALUE.Rows.Add(DRNEWVAL)
        Session(FORMNAME) = dtFD
        Session(FORMNAME & "VAL") = DTVALUE
        BINDGRID1(dtFD)
        ' ModalPopupExtender1.Hide()
    End Sub

    Public Sub INSERTCHILDITEM(ByVal SENDER As Object, ByVal E As System.EventArgs)
        Dim btnDetails As Button = TryCast(SENDER, Button)
        Dim formname As String = btnDetails.Text
        formname = Right(formname, formname.Length - 8).Trim()
        Session("FN") = formname
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
        ob.BINDITEMGRID(DS.Tables("ITEM"), pnlFields, "BTN" & ID(1).ToString(), UpdatePanel1, DS.Tables("TOTAL"))
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
        Dim CurIndex As Integer = rw.RowIndex
        If Pid.Length > 1 Then
            ViewState("ID") = Pid(2)
        End If
        If e.CommandName = "Remove" Then
            Dim dt As DataTable = Session(Pid(0))
            If dt.Rows.Count > 1 And dt.Rows.Count = 2 Then
                dt.Rows.RemoveAt(dt.Rows.Count - 1)
            End If
            Dim DTVAL As DataTable = Session(Pid(0) & "VAL")
            If dt.Rows.Count > 0 Then
                dt.Rows.RemoveAt(CurIndex)
                DTVAL.Rows.RemoveAt(CurIndex)
                Session(Pid(0)) = dt
                Session(Pid(0) & "VAL") = DTVAL

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & Session("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
                oda.Fill(DS, "TOTAL")

                'oda.SelectCommand.CommandText = "Delete from MMM_MST_DOC_ITEM where tid=" & Pid & ""
                'oda.SelectCommand.ExecuteScalar()
                oda.Dispose()
                con.Dispose()
                Dim OB As New DynamicForm
                OB.BINDITEMGRID1(dt, pnlFields, Session("ID"), UpdatePanel1, DS.Tables("TOTAL"))
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
            Session("FN") = DS.Tables("CHILD").Rows(0).Item("DOCUMENTTYPE").ToString
            Session("CHILD") = DS.Tables("CHILD")
            pnlFields1.Controls.Clear()
            ob.CreateControlsOnPanel(Session("CHILD"), pnlFields1, updpnlchild, Button2, 0)
            Dim ROW1() As DataRow = DS.Tables("CHILD").Select("fieldtype='DROP DOWN' and (dropdowntype='MASTER VALUED' or dropdowntype='CHILD VALUED' OR DROPDOWNTYPE='SESSION VALUED') and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue1
                Next
            End If
            ' FILLCONTROLONEDIT(DS.Tables("CHILD"), Session(Pid(0).ToString), Session(Pid(0) & "VAL"), pnlFields1, updpnlchild, CInt(Pid(1)))
            FILLCONTROLONEDIT(DS.Tables("CHILD"), Session(Pid(0).ToString), Session(Pid(0) & "VAL"), pnlFields1, updpnlchild, CurIndex)
            Button2.Text = "UPDATE"
            lblChildFormName.Text = "EDIT  " & Session("FN")
            updChildFormname.Update()
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
        ODA.SelectCommand.Parameters.AddWithValue("FN", Session("FN"))
        ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        DS.Tables.Clear()
        ODA.Fill(DS, "ITEM")
        Dim dt_item As DataTable = New DataTable()
        dt_item = DS.Tables("ITEM")
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & Session("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        ODA.Fill(DS, "TOTAL")
        ODA.Dispose()
        DS.Dispose()
        Dim OB As New DynamicForm
        OB.BINDITEMGRID(dt_item, pnlFields, ID, UpdatePanel1, DS.Tables("TOTAL"))
    End Sub

    Public Sub ADDTASK(ByVal sender As System.Object, ByVal e As System.EventArgs)
        AddTask_ModalPopUp.Show()
        ddluser.SelectedIndex = 0
        txtDue_Date.Text = ""
        txtRemarks.Text = ""
        UpdPnlAddTask.Update()
    End Sub

    Protected Sub BTNTask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTNTask.Click
        Dim DT_TASK As New DataTable
        If Session("dtNew") Is Nothing Then
            DT_TASK.Columns.Add("USERNAME", GetType(String))
            DT_TASK.Columns.Add("DUE_DATE", GetType(String))
            DT_TASK.Columns.Add("REMARKS", GetType(String))
            DT_TASK.Columns.Add("UID", GetType(Integer))
        Else
            DT_TASK = Session("dtNew")
        End If
        Dim DRNEW As DataRow = DT_TASK.NewRow()
        DRNEW.Item("USERNAME") = ddluser.SelectedItem.Text
        DRNEW.Item("DUE_DATE") = txtDue_Date.Text
        DRNEW.Item("REMARKS") = txtRemarks.Text
        DRNEW.Item("UID") = ddluser.SelectedValue
        DT_TASK.Rows.Add(DRNEW)
        Session("dtNew") = DT_TASK
        BINDGRIDTASK(DT_TASK)
        AddTask_ModalPopUp.Hide()
    End Sub

    '' above is used when child item's save button is clicked (adds row to grid) by sunil 
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

        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select displayName,FieldMapping,KC_LOGIC  from mmm_mst_fields  where FieldType='Formula Field' and documenttype='" & FORMNAME & "' "
        Dim dt As New DataTable()
        oda.Fill(dt)
        drnew.Item("tid") = FORMNAME & "-" & dtFD.Rows.Count & "-" & ViewState("ID")
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                '  dt.Rows(0)("Value") = ""
                Dim forchexec As String = String.Empty
                Dim formula As New formulaEditor()

                forchexec = formula.executeformulachielditem(dt.Rows(i).Item("KC_LOGIC"), drnew, dtFD)
                drnew.Item(dt.Rows(i).Item("displayName").ToString()) = forchexec
                DRNEWVAL.Item(dt.Rows(i).Item("displayName").ToString()) = forchexec
                ' forchexec = formula.executeformulachielditem(dt.Rows(i).Item("KC_LOGIC"), DRNEWVAL, DTVALUE)
                'DRNEWVAL.Item(dt.Rows(i).Item("displayName").ToString()) = forchexec
            Next
        End If
        con.Close()
        dt.Dispose()
        con.Dispose()
        oda.Dispose()


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

    '' above is used for binding grid (adding row to grid) when child item's save button is clicked
    Protected Sub BINDGRID1(ByVal DT As DataTable)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim scrname As String = Request.QueryString("pvdoctype").ToString()
        Dim DS As New DataSet
        'ODA.SelectCommand.Parameters.Clear()
        'ODA.SelectCommand.CommandText = "uspGetDetailITEM"
        'ODA.SelectCommand.CommandType = CommandType.StoredProcedure
        'ODA.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
        'ODA.SelectCommand.Parameters.AddWithValue("FN", Session("FN"))
        'ODA.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
        'DS.Tables.Clear()
        'ODA.Fill(DS, "ITEM")
        Dim dt_item As DataTable = New DataTable()
        dt_item = DT
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =F2.Fieldid  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & Session("FN") & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
        ODA.Fill(DS, "TOTAL")
        ODA.Dispose()
        DS.Dispose()
        Dim OB As New DynamicForm
        ' OB.BINDITEMGRID(dt_item, pnlFields, "BTN" & ViewState("ID"), UpdatePanel1, DS.Tables("TOTAL"))
        OB.BINDITEMGRID(dt_item, pnlFields, ViewState("ID"), UpdatePanel1, DS.Tables("TOTAL"))

    End Sub

    'Public Sub bindGrid2(ByVal DS As DataTable, ByRef PNLFIELDS As Panel, ByVal ID As String, ByRef UPD As UpdatePanel)
    '    Dim GID As String = ID
    '    Dim GV As GridView = CType(PNLFIELDS.FindControl("GRD" & GID.ToString()), GridView)
    '    Try
    '        If DS.Rows.Count > 0 Then
    '            GV.DataSource = DS
    '            GV.DataBind()
    '        End If

    '    Catch ex As Exception

    '    End Try
    '    UPD.Update()
    'End Sub

    Protected Sub BINDGRIDTASK(ByVal DT As DataTable)
        Dim GRD As GridView = TryCast(pnlFields.FindControl("GRDCLNDR"), GridView)
        GRD.DataSource = DT
        GRD.DataBind()
        GRD.Caption = "MANAGE TASK"
    End Sub

    Protected Sub DeleteTask(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim btnDelete As GridView = TryCast(sender, GridView)
        Dim ID As String = btnDelete.ID
        If e.CommandName = "Delete" Then
            Dim rw As GridViewRow = DirectCast(DirectCast(e.CommandSource, ImageButton).NamingContainer, GridViewRow)
            Dim Pid As String = btnDelete.DataKeys(rw.RowIndex).Value
            Dim dt As DataTable = Session("dtNew")
            dt.Rows(rw.RowIndex).Delete()
            Session("dtNew") = dt
            BINDGRIDTASK(dt)
        End If
    End Sub

    Protected Sub AddTask(ByVal docid As Integer, ByVal doctype As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "USP_ADDTASK"
        Dim DT As DataTable = Session("dtNew")
        For Each DR As DataRow In DT.Rows
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("DOCID", docid)
            oda.SelectCommand.Parameters.AddWithValue("DOCTYPE", doctype)
            oda.SelectCommand.Parameters.AddWithValue("UID", DR.Item("UID"))
            oda.SelectCommand.Parameters.AddWithValue("DD", DR.Item("DUE_DATE"))
            oda.SelectCommand.Parameters.AddWithValue("REMARKS", DR.Item("REMARKS"))
            oda.SelectCommand.ExecuteScalar()
        Next
        Session("dtNew") = Nothing
        BINDGRIDTASK(Session("dtNew"))
        oda.Dispose()
        con.Dispose()
    End Sub

    Public Sub addTemplateField(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim cnt As Integer = e.Row.Cells.Count - 1
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.Cells(0).Text.ToUpper() = "TOTAL" Then
                e.Row.Font.Bold = True
                e.Row.ForeColor = Drawing.Color.Black
            Else
                Dim img As ImageButton = New ImageButton()
                img.ID = e.Row.Cells(cnt).Text
                img.ImageUrl = "~/images/Cancel.gif"
                img.CommandName = "Delete"
                img.CommandArgument = e.Row.Cells(cnt).Text
                img.Height = Unit.Parse("16")
                img.Width = Unit.Parse("16")
                e.Row.Cells(cnt).Controls.Add(img)
            End If
        ElseIf e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(cnt).Text = " "
        End If

    End Sub

    Public Sub DeletedTask()
        'Dim GRD As GridView = TryCast(pnlFields.FindControl("GRDCLNDR"), GridView)
        'GRD.DataSource = Session("dtNew")
        'GRD.DataBind()
        'GRD.Caption = "MANAGE TASK"
    End Sub

    Public Sub parentchild(ByVal cid As Integer, ByVal Pid As Integer)
        Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & Pid), DropDownList)
        If DDL Is Nothing Then
            Exit Sub
        End If
        Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & cid), DropDownList)
        ddl1.Items.Clear()
        ddl1.Items.Insert(0, "Select One")
        ddl1.SelectedIndex = 0
        Dim li As ListItem = DDL.SelectedItem
        Dim tn As String = li.Text
        Dim vl As String = li.Value
        ddl1.Items.Add(tn)
        ddl1.Items(1).Value = vl
        updpnlchild.Update()
    End Sub

    Public Sub parentchild1(ByVal cid As Integer, ByVal Pid As Integer)
        'Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & Pid), DropDownList)
        'If DDL Is Nothing Then
        '    Exit Sub
        'End If
        'ddl1.Items.Clear()
        'Dim li As ListItem = DDL.SelectedItem
        'ddl1.Items.Add(li)
        Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & cid), DropDownList)
        ddl1.Enabled = False
        updpnlchild.Update()
    End Sub

    'Public Sub ChildFormddlRendering(ByVal ACTION As Integer)
    '    Dim c As Control = GetPostBackControl(Me.Page)
    '    '...
    '    'If c IsNot Nothing Then
    '    'End If

    '    If TypeOf c Is System.Web.UI.WebControls.Button Then
    '        Dim BTN As Button = TryCast(c, Button)
    '        If Left(BTN.Text.Trim, 3) = "ADD" Then
    '            Dim id As String = Right(c.ID, c.ID.Length - 3)
    '            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '            Dim con As New SqlConnection(conStr)
    '            Dim oda As SqlDataAdapter = New SqlDataAdapter("Select KC_LOGIC from mmm_mst_fields where fieldid=" & id & "", con)
    '            Dim STRKC As String = ""
    '            If con.State <> ConnectionState.Open Then
    '                con.Open()
    '            End If
    '            STRKC = oda.SelectCommand.ExecuteScalar().ToString()

    '            If STRKC <> "" Then
    '                Dim FLDS() As String = STRKC.Split("-")
    '                If ACTION = 1 Then
    '                    Dim DDL As DropDownList = TryCast(pnlFields.FindControl("fld" & FLDS(0)), DropDownList)
    '                    If DDL Is Nothing Then
    '                        Exit Sub
    '                    End If
    '                    Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
    '                    ddl1.Items.Clear()
    '                    ddl1.Items.Insert(0, "Select One")
    '                    ddl1.Enabled = True
    '                    ddl1.SelectedIndex = 0
    '                    Dim li As ListItem = DDL.SelectedItem
    '                    Dim tn As String = li.Text
    '                    Dim vl As String = li.Value
    '                    ddl1.Items.Add(tn)
    '                    ddl1.Items(1).Value = vl
    '                    updpnlchild.Update()
    '                Else
    '                    Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
    '                    ddl1.Enabled = False
    '                End If
    '            End If
    '        End If
    '    End If
    'End Sub

    Public Sub ChildFormddlRenderingOnCreation(ByVal ACTION As Integer)
        Dim c As Control = GetPostBackControl(Me.Page)
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
                        If DDL.SelectedIndex = Nothing Then
                            Exit Sub
                        End If
                        Dim ddl1 As DropDownList = TryCast(pnlFields1.FindControl("fld" & FLDS(1)), DropDownList)
                        'ddl1.Items.Clear()
                        'ddl1.Items.Insert(0, "SELECT")
                        'ddl1.Enabled = True
                        'ddl1.SelectedIndex = 0
                        'Dim li As ListItem = DDL.SelectedItem
                        'Dim tn As String = li.Text
                        'Dim vl As String = li.Value
                        'ddl1.Items.Add(tn)
                        'ddl1.Items(1).Value = vl

                        'Change By V 24 Dec
                        If IsNothing(ddl1) Then
                        Else
                            ddl1.Items.Clear()
                        End If

                        oda.SelectCommand.CommandText = "select isEditable,isactive from mmm_mst_fields where fieldid=" & FLDS(1) & " and eid=" & Session("EID") & ""
                        ' Dim edt As Integer = oda.SelectCommand.ExecuteScalar().ToString
                        Dim dt As New DataTable
                        oda.Fill(dt)
                        If dt.Rows(0).Item(0).ToString = "1" And dt.Rows(0).Item(1).ToString = "1" Then
                            ddl1.Items.Insert(0, "SELECT")
                            ddl1.Enabled = True
                            ddl1.SelectedIndex = 0
                            Dim li As ListItem = DDL.SelectedItem
                            Dim tn As String = li.Text
                            Dim vl As String = li.Value
                            ddl1.Items.Add(tn)
                            ddl1.Items(1).Value = vl
                        ElseIf dt.Rows(0).Item(0).ToString = "0" And dt.Rows(0).Item(1).ToString = "1" Then
                            Dim li As ListItem = DDL.SelectedItem
                            Dim tn As String = li.Text
                            Dim vl As String = li.Value
                            ddl1.Items.Add(tn)
                            ddl1.Items(0).Value = vl
                            ddl1.Enabled = False
                        Else
                        End If
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
                If FN.ToString = Session("ID").ToString.Trim Then
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
                            'ddl1.Enabled = False

                            'Change By V 27 Dec
                            If ddl1 Is Nothing Then
                            Else
                                ddl1.Enabled = False
                            End If
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

    'Protected Sub Page_PreLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreLoad
    '    If Not Session("CHILD") Is Nothing Then
    '        Dim ROW3() As DataRow = Session("CHILD").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and kc_logic is not null")
    '        If ROW3.Length > 0 Then
    '            For i As Integer = 0 To ROW3.Length - 1
    '                parentchild1(ROW3(i).Item("FIELDID"), CInt(ROW3(i).Item("KC_LOGIC").ToString()))
    '            Next
    '        End If
    '    End If
    'End Sub

    Public Sub Reset(ByVal sender As Object, ByVal e As EventArgs)
        'Dim scrname As String = Request.QueryString("SC").ToString()
        'Response.Redirect(Request.RawUrl)
    End Sub

    'Protected Sub btnimmm_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimmm.Click

    '    'ModalPopupExtender1.Show()

    '    modalpopupimport.Show()
    '    ModalPopupExtender1.Hide()
    'End Sub
    'Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
    '    Try

    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim scrname As String = ViewState("FN")
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

    '                            sb.Append("Insert Into MMM_MST_DOC_ITEM ( sessionid,documenttype,")

    '                            For k As Integer = 0 To c - 1
    '                                If UCase(sField(k)) <> UCase(Trim(ds.Tables("data").Rows(k).Item("displayname").ToString())) Then
    '                                    lblMsg.Text = "Not Found"
    '                                Else

    '                                    st = ds.Tables("data").Rows(k).Item("FieldMapping").ToString()
    '                                    sb.Append(st)
    '                                    If k = c - 1 Then
    '                                        sb.Append(") values ( ")
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
    '                        v &= "'" & Session.SessionID & "'"
    '                        v &= ","
    '                        v &= "'" & scrname & "'"
    '                        'sb.Append(Session("EID"))
    '                        'sb.Append(",")
    '                        'sb.Append("'" & scrname & "'")
    '                        'v &= "," ' sb.Append(",")
    '                        'v &= "'" & Session("UID") & "'" ' sb.Append(Session("UID"))
    '                        'v &= "," 'sb.Append(",")
    '                        'v &= "'" & DateAndTime.Now & "'" 'sb.Append("'" & DateAndTime.Now & "'")
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
    '                                    If vk = 1 And IsDate(Trim(sField(j))) = False Then
    '                                        c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err-Invalid Date " & sField(j) & ")" & "</td>" & " </tr> </table>"
    '                                        Continue While
    '                                    ElseIf vk = 0 And IsDate(Trim(sField(j))) = False Then
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
    '                            If dd.Length > 2 And dd.Contains("Master".ToUpper) Then
    '                                Dim b1 As String() = dd.ToString().Split("-")
    '                                ViewState("dd1") = b1(1).ToString()
    '                                da.SelectCommand.CommandText = "select count(tid) from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & ViewState("dd1") & "' and tid=" & sField(j) & ""
    '                                If con.State <> ConnectionState.Open Then
    '                                    con.Open()
    '                                End If
    '                                Dim cntt As Integer = da.SelectCommand.ExecuteScalar()
    '                                If cntt < "1" Then
    '                                    c1 &= "<table border=""1px"" style=""border-color:black;""> <tr> <td  width=""230px"" align=""Center"">" & icnt & " </td> <td  width=""230px"" align=""Center"">" & dn & "(Err- This tid  " & sField(j) & " does not exists)" & "</td>" & " </tr> </table>"
    '                                    Continue While
    '                                Else
    '                                    v &= "'"
    '                                    v &= Trim(sField(j))
    '                                End If
    '                            Else

    '                                v &= "'" 'sb.Append("'")
    '                                v &= Trim(sField(j))
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
    '                        con.Close()
    '                        adapter.Dispose()
    '                        sh.Clear()
    '                        BINDGRID()
    '                    End While

    '                    '.DataBind()

    '                    'gvexport.DataBind()
    '                    'updPnlGrid.UpdateMode = UpdatePanelUpdateMode.Conditional
    '                    'updPnlGrid.Update()
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
    '                End With
    '            Else
    '                lblMsg.Text = "File should be of CSV Format"
    '                Exit Sub
    '            End If
    '        Else
    '            lblMsg.Text = "Please select a File to Upload"
    '            Exit Sub
    '        End If



    '        '  Dim gvchild As New GridView
    '        'gvchild.DataSource=
    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured while importing data. Please try again"
    '    End Try

    'End Sub
    'Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable

    '    Dim returnTable As New DataTable()
    '    If columnX = "" Then
    '        columnX = table.Columns(0).ColumnName
    '    End If

    '    Dim columnXValues As New List(Of String)()

    '    For Each dr As DataRow In table.Rows
    '        Dim columnXTemp As String = dr(columnX).ToString()
    '        If Not columnXValues.Contains(columnXTemp) Then
    '            columnXValues.Add(columnXTemp)
    '            returnTable.Columns.Add(columnXTemp)
    '        End If
    '    Next
    '    If nullValue <> "" Then
    '        For Each dr As DataRow In returnTable.Rows
    '            For Each dc As DataColumn In returnTable.Columns
    '                If dr(dc.ColumnName).ToString() = "" Then
    '                    dr(dc.ColumnName) = nullValue
    '                End If
    '            Next
    '        Next
    '    End If
    '    Return returnTable

    'End Function
    'Protected Sub helpexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles helpexport.Click
    '    Try
    '        Response.Clear()
    '        Response.Buffer = True


    '        Response.Charset = ""
    '        Response.ContentType = "application/vnd.ms-excel"
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product
    '        Dim ds As New DataSet
    '        Dim scrname As String = ViewState("FN")
    '        Response.AddHeader("content-disposition", _
    '          "attachment;filename=" & scrname & ".xls")
    '        Dim da As New SqlDataAdapter("SELECT displayName[Display Name],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (MM/DD/YYYY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 ", con)
    '        Dim query As String = "SELECT displayName,case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (MM/DD/YYYY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 "
    '        Dim cmd As SqlCommand = New SqlCommand(query, con)
    '        con.Open()
    '        Dim dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
    '        Dim dt As DataTable = New DataTable()
    '        Dim dt2 As DataTable = New DataTable()
    '        Dim dt3 As DataTable = New DataTable()

    '        dt.Load(dr)
    '        da.Fill(ds, "data")
    '        dt3 = ds.Tables("data")
    '        dt2 = GetInversedDataTable(dt, "displayname", "")

    '        Dim gvex As New GridView()
    '        dt2.Rows.Add()

    '        gvex.AllowPaging = False
    '        gvex.DataSource = dt2
    '        gvex.DataBind()
    '        Dim gvexx As New GridView()
    '        gvexx.AllowPaging = False
    '        gvexx.DataSource = dt3

    '        gvexx.DataBind()
    '        Response.Clear()
    '        Response.Buffer = True
    '        Dim sw As New StringWriter()
    '        Dim hw As New HtmlTextWriter(sw)

    '        Dim tb As New Table()
    '        Dim tr1 As New TableRow()
    '        Dim cell1 As New TableCell()
    '        cell1.Controls.Add(gvex)
    '        tr1.Cells.Add(cell1)
    '        Dim cell3 As New TableCell()
    '        cell3.Controls.Add(gvexx)
    '        Dim cell2 As New TableCell()
    '        cell2.Text = "&nbsp;"

    '        Dim tr2 As New TableRow()
    '        tr2.Cells.Add(cell2)
    '        Dim tr3 As New TableRow()
    '        tr3.Cells.Add(cell3)
    '        tb.Rows.Add(tr1)
    '        tb.Rows.Add(tr2)
    '        tb.Rows.Add(tr3)

    '        tb.RenderControl(hw)

    '        'style to format numbers to string 
    '        Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
    '        Response.Write(style)
    '        Response.Output.Write(sw.ToString())
    '        Response.Flush()
    '        Response.[End]()

    '    Catch ex As Exception
    '        lblMsg.ForeColor = Drawing.Color.Red
    '        lblMsg.Text = "An error occured when Downloading data. Please try again"
    '    End Try
    'End Sub

    Protected Sub SavingChildItem_DV(ByVal formname As String, ByVal docid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtField As New DataTable
        Dim updquery As String = ""
        Dim ob As New DynamicForm()
        If Session(formname & "VAL") Is Nothing Then
            Exit Sub
        End If

        ' dtField = ViewState(formname)
        dtField = Session("D" & formname)
        Dim dtvalue As DataTable = Session(formname & "VAL")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'Session("id")
        Dim dt As New DataTable
        ODA.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("eid") & " and documenttype='" & Session("Document") & "' and dropdown='" & formname & "'"
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.Parameters.Clear()
        ODA.Fill(dt)

        Dim dtTotal As New DataTable
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID [MainFieldID],F2.FieldID [ChildFieldID],F2.displayName [CdisplayName] ,F1.dropdown [mDropDown],F1.displayName [MdisplayName],F1.FieldMapping [MFieldMapping]  FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & Session("Document") & "'"
        ODA.Fill(dtTotal)

        Dim isTotal As Boolean = False
        Dim cDispName As String = ""
        Dim Childtotal As Double = 0
        If dtTotal.Rows.Count = 1 Then
            isTotal = True
            cDispName = dtTotal.Rows(0).Item("cdisplayname").ToString
        End If

        'For Each dr As DataRow In DS1.Rows
        '    Dim txt As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("Fieldid").ToString()), TextBox)
        '    txt.Text = DS.Rows(rowcount - 1).Item(dr.Item("displayname")).ToString()
        'Next

        Dim GID As String = dt.Rows(0).Item("fieldId").ToString

        Dim dataitem As DataTable = Session("ITEM")

        ' GID = Right(GID, GID.Length - 3)
        'Dim itemDS As DataSet = Session("ITEM")
        Dim GV As GridView = CType(pnlFields.FindControl("GRD" & GID.ToString()), GridView)

        Dim cnt As Integer = 0
        For Each GR As GridViewRow In GV.Rows
            If GR.RowType = DataControlRowType.DataRow Then
                If GR.Cells(0).Text.ToUpper <> "TOTAL" Then
                    For i As Integer = 0 To dataitem.Rows.Count - 1
                        Dim str As String = "update MMM_MST_DOC_ITEM Set "
                        Dim STRFld As String = ""
                        Dim STRVal As String = ""
                        updquery = ""
                        Dim querry As String = ""
                        ' For j As Integer = 0 To GR.Cells.Count - 1
                        For j As Integer = 0 To dtField.Rows.Count - 1
                            STRFld &= dtField.Rows(j).Item("fieldmapping").ToString & ","
                            Dim colValue As String = ""
                            Dim FldID As String = ""
                            FldID = dtField.Rows(j).Item("fieldid").ToString()
                            If dtField.Rows(j).Item("inlineediting").ToString = "1" Then
                                '' new for getting child item total 
                                Dim ftype As String = dtField.Rows(j).Item("fieldtype").ToString()
                                Dim fldmapping As String = dtField.Rows(j).Item("fieldmapping").ToString()
                                If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Then
                                    ' Dim cb As TextBox = CType(GR.FindControl("fld" & j.ToString() & "_" & cnt), TextBox)
                                    'Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & cnt), TextBox)
                                    Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                    If cb IsNot Nothing Then
                                        colValue = cb.Text.ToString
                                        querry = fldmapping & "='" & colValue & "' " & ","
                                    Else
                                        colValue = "0"
                                    End If
                                    If Right(querry, 1) = "," Then
                                        querry = Left(querry, querry.Length - 1)
                                    End If
                                    str &= querry & " where TID=" & dataitem.Rows(GR.RowIndex).Item("TID")

                                    If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                        If colValue <> "" Then
                                            Childtotal = Childtotal + Convert.ToDouble(colValue)
                                        End If
                                    End If
                                ElseIf ftype.ToUpper() = "DROP DOWN" And dtField.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then
                                    'Dim cb As New DropDownList
                                    Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & cnt), DropDownList)
                                    If cb IsNot Nothing Then
                                        colValue = cb.SelectedItem.Text.ToString
                                    Else
                                        colValue = "0"
                                    End If
                                ElseIf ftype.ToUpper() = "DROP DOWN" And dtField.Rows(j).Item("dropdownTYPE") = "MASTER VALUED" Then
                                    Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & cnt), DropDownList)
                                    If cb IsNot Nothing Then
                                        colValue = cb.SelectedItem.Value.ToString  ' use value bcoz it's mastervalued - needs tid
                                    Else
                                        colValue = "0"
                                    End If
                                ElseIf ftype.ToUpper() = "FILE UPLOADER" Then
                                    '' here to code for getting file 
                                    Dim txtBox As FileUpload = CType(GR.FindControl("fld" & FldID & cnt), FileUpload)
                                    If txtBox.HasFile Then
                                        Dim FN As String = ""
                                        Dim ext As String = ""
                                        FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                                        ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))
                                        colValue = Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & "" & ext
                                        txtBox.SaveAs(Server.MapPath("DOCS/") & Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & ext)
                                    End If
                                Else
                                    colValue = GR.Cells(j).Text.ToString()
                                    If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                        If colValue <> "" Then
                                            Childtotal = Childtotal + Convert.ToDouble(colValue)
                                        End If
                                    End If

                                End If
                            Else  ' if no inline editing 
                                If dtField.Rows(j).Item("dropdownTYPE") = "MASTER VALUED" And dtField.Rows(j).Item("fieldtype") = "Drop Down" Then
                                    '' here code to get tid from session arrays by sp 13-jan-13
                                    Dim CH() As String = Session("COLHEAD")  ' {}
                                    Dim txt() As String = Session("DDLTXT") ' {}
                                    Dim Vals() As String = Session("DDLVAL") '{}
                                    Dim found As Boolean = False
                                    Dim searchHdr As String = formname & "_" & dtField.Rows(j).Item("displayname")
                                    For a As Integer = 0 To CH.Length - 1
                                        If CH(a).ToString = searchHdr And txt(a).ToString = GR.Cells(j).Text.ToString() Then ' if match found in array 
                                            colValue = Vals(a).ToString
                                            found = True
                                            Exit For
                                        End If
                                    Next
                                    If found = False Then
                                        colValue = dtvalue.Rows(cnt).Item(j).ToString()
                                    End If
                                    'ElseIf dtField.Rows(j).Item("fieldtype") = "Child Item Total" Then

                                Else
                                    colValue = GR.Cells(j).Text.ToString()
                                    If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                        If colValue <> "" Then
                                            Childtotal = Childtotal + Convert.ToDouble(colValue)
                                        End If
                                    End If
                                End If
                            End If
                            'STRVal &= "'" & colValue & "'" & ","
                            colValue = colValue
                        Next
                        ODA.SelectCommand.CommandText = str
                        ODA.SelectCommand.ExecuteNonQuery()
                        Exit For
                    Next
                    '''''
                    'If dtField.Rows(j).Item("KC_VALUE").ToString.Length > 5 And dtField.Rows(j).Item("KC_STATUS").ToString.Length = 0 Then
                    ' updquery &= ob.UPDATEKICKING(dtField.Rows(j).Item("KC_VALUE").ToString(), dtvalue.Rows(i).Item(j).ToString(), pnlFields1)
                    ' End If

                    'STRFld &= "DOCID,documenttype,isauth)"
                    'STRVal &= docid & "," & "'" & formname & "'" & ",1)"
                    'Str &= STRFld & "values(" & STRVal

                    'ODA.SelectCommand.CommandText = Str()
                    'ODA.SelectCommand.ExecuteNonQuery()
                    cnt += 1
                End If
            End If
        Next


        If isTotal Then
            Dim childTotalUpdstr As String = ""
            childTotalUpdstr = "UPDATE mmm_mst_doc set " & dtTotal.Rows(0).Item("mFieldMapping").ToString & "=" & Childtotal & " where tid=" & docid

            ODA.SelectCommand.CommandType = CommandType.Text
            ODA.SelectCommand.CommandText = childTotalUpdstr
            ODA.SelectCommand.ExecuteNonQuery()
        End If


        dt.Dispose()
        dtTotal.Dispose()
        dtField.Dispose()
        dtvalue.Dispose()

        Session(formname) = Nothing
        Session(formname & "VAL") = Nothing
        ODA.Dispose()
        con.Close()
        con.Dispose()
    End Sub

    Public Function getSafeString(ByVal strVar As String) As String
        Trim(strVar)
        strVar = Replace(strVar, "'", "")
        strVar = Replace(strVar, ";", "")
        strVar = Replace(strVar, "--", "")
        strVar = Replace(strVar, "%", "")
        strVar = Replace(strVar, "&", "")
        Return strVar
    End Function

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

        ' dtField = ViewState(formname)
        dtField = Session("D" & formname)
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
                If dtvalue.Columns(j).ColumnName.ToUpper <> "TID" Then
                    STRfLD &= dtField.Rows(j).Item("fieldmapping").ToString & ","
                    STRVAL &= "'" & dtvalue.Rows(i).Item(j).ToString() & "'" & ","
                    If dtField.Rows(j).Item("KC_VALUE").ToString.Length > 5 And dtField.Rows(j).Item("KC_STATUS").ToString.Length = 0 Then
                        updquery &= ob.UPDATEKICKING(dtField.Rows(j).Item("KC_VALUE").ToString(), dtvalue.Rows(i).Item(j).ToString(), pnlFields1)
                    End If
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

                    'Change By V 24 Dec
                    Dim edt As Integer = dtFields.Rows(j).Item("isEditable").ToString

                    Select Case dtFields.Rows(j).Item("FieldType").ToString().ToUpper()
                        Case "TEXT BOX"
                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dtFields.Rows(j).Item("FieldID").ToString()), TextBox)
                            'drnew.Item(dtFields.Rows(i).Item("displayname")) = txtBox.Text
                            txtBox.Text = dtval.Rows(i).Item(j).ToString()
                        Case "DROP DOWN"
                            Dim ddl As DropDownList = CType(pnlFields.FindControl("fld" & dtFields.Rows(j).Item("FieldID").ToString()), DropDownList)
                            'drnew.Item(dtFields.Rows(i).Item("displayname")) = txtBox.SelectedItem.Text
                            'drnewval.Item(dtFields.Rows(i).Item("displayname")) = txtBox.SelectedItem.Value
                            If edt = 0 Then
                                ddl.Enabled = False
                            End If
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
        ' dtField = ViewState(FORMNAME)
        dtField = Session("D" & FORMNAME)
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

        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select displayName,FieldMapping,KC_LOGIC  from mmm_mst_fields  where FieldType='Formula Field' and documenttype='" & FORMNAME & "' "
        Dim dt As New DataTable()
        oda.Fill(dt)
        drnew.Item("tid") = FORMNAME & "-" & dtFD.Rows.Count & "-" & ViewState("ID")
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                '  dt.Rows(0)("Value") = ""
                Dim forchexec As String = String.Empty
                Dim formula As New formulaEditor()

                forchexec = formula.executeformulachielditem(dt.Rows(i).Item("KC_LOGIC"), drnew, dtFD)
                drnew.Item(dt.Rows(i).Item("displayName").ToString()) = forchexec
                DRNEWVAL.Item(dt.Rows(i).Item("displayName").ToString()) = forchexec
                ' forchexec = formula.executeformulachielditem(dt.Rows(i).Item("KC_LOGIC"), DRNEWVAL, DTVALUE)
                'DRNEWVAL.Item(dt.Rows(i).Item("displayName").ToString()) = forchexec
            Next
        End If

        con.Close()
        dt.Dispose()
        con.Dispose()
        oda.Dispose()
        Dim OB As New DynamicForm()
        If dtField.Rows.Count > 0 Then
            Dim str As String = OB.validateForm(dtField.Rows(0).Item("Documenttype").ToString, Session("EID"), pnlFields, dtField, "ADD", 0)
            If str.Length > 5 Then
                str = "Please " & str
                Label3.Text = str
                Exit Sub
            End If
        End If
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
    '' old my proc b4 rajat's merge 
    'Protected Sub SavingChildItemOnEdit(ByVal FORMNAME As String)
    '    Dim dtFD As New DataTable
    '    Dim dtField As New DataTable
    '    Dim DTVALUE As New DataTable
    '    Dim errormsg As String = ""
    '    dtField = ViewState(FORMNAME)
    '    If Session(FORMNAME) Is Nothing Then
    '        For Each dr As DataRow In dtField.Rows
    '            dtFD.Columns.Add(dr.Item("displayname"), GetType(String))
    '            DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
    '        Next
    '        dtFD.Columns.Add("tid", GetType(String))
    '    Else
    '        dtFD = Session(FORMNAME)
    '        DTVALUE = Session(FORMNAME & "VAL")
    '        If dtFD.Rows.Count > 1 Then
    '            dtFD.Rows.RemoveAt(dtFD.Rows.Count - 1)
    '        End If
    '    End If
    '    Dim drnew As DataRow = dtFD.NewRow()
    '    Dim DRNEWVAL As DataRow = DTVALUE.NewRow()
    '    For Each dr As DataRow In dtField.Rows
    '        Dim dispName As String = dr.Item("displayname").ToString()
    '        Select Case dr.Item("FieldType").ToString().ToUpper()
    '            Case "TEXT BOX"
    '                Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
    '                drnew.Item(dr.Item("displayname")) = txtBox.Text
    '                DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
    '            Case "DROP DOWN"
    '                Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), DropDownList)
    '                drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
    '                DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
    '            Case "CALCULATIVE FIELD"
    '                Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
    '                drnew.Item(dr.Item("displayname")) = txtBox.Text
    '                DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
    '            Case "LOOKUP"
    '                Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
    '                drnew.Item(dr.Item("displayname")) = txtBox.Text
    '                DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
    '        End Select
    '    Next
    '    drnew.Item("tid") = FORMNAME & "-" & ViewState("Index") & "-" & ViewState("ID")
    '    'dtFD.Rows.Add(drnew)
    '    dtFD.Rows.RemoveAt(ViewState("Index"))
    '    dtFD.Rows.InsertAt(drnew, ViewState("Index"))
    '    'DTVALUE.Rows.Add(DRNEWVAL)
    '    DTVALUE.Rows.RemoveAt(ViewState("Index"))
    '    DTVALUE.Rows.InsertAt(DRNEWVAL, ViewState("Index"))
    '    Session(FORMNAME) = dtFD
    '    Session(FORMNAME & "VAL") = DTVALUE
    '    BINDGRID1(dtFD)
    '    ModalPopupExtender1.Hide()
    '    UpdatePanel1.Update()
    'End Sub
    Protected Sub btnOk_Click(sender As Object, e As System.EventArgs) Handles btnOk.Click
        Response.Redirect("confermation.aspx?saved=0")
    End Sub


    Protected Function ValidatingChildItem_DV(ByVal formname As String) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtField As New DataTable
        Dim updquery As String = ""
        Dim ob As New DynamicForm()
        If Session(formname & "VAL") Is Nothing Then
            Exit Function
        End If

        ' dtField = ViewState(formname)
        dtField = Session("D" & formname)
        Dim dtvalue As DataTable = Session(formname & "VAL")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'Session("id")
        Dim dt As New DataTable
        ODA.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("eid") & " and documenttype='" & Session("Document") & "' and dropdown='" & formname & "'"
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.Parameters.Clear()
        ODA.Fill(dt)

        Dim dtTotal As New DataTable
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID [MainFieldID],F2.FieldID [ChildFieldID],F2.displayName [CdisplayName] ,F1.dropdown [mDropDown],F1.displayName [MdisplayName],F1.FieldMapping [MFieldMapping]  FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType ='CHILD ITEM TOTAL' AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & Session("Document") & "'"
        ODA.Fill(dtTotal)

        Dim isTotal As Boolean = False
        Dim cDispName As String = ""
        Dim Childtotal As Double = 0
        If dtTotal.Rows.Count = 1 Then
            isTotal = True
            cDispName = dtTotal.Rows(0).Item("cdisplayname").ToString
        End If

        Dim GID As String = dt.Rows(0).Item("fieldId").ToString
        Dim GV As GridView = CType(pnlFields.FindControl("GRD" & GID.ToString()), GridView)
        Dim cnt As Integer = 0
        For Each GR As GridViewRow In GV.Rows
            If GR.RowType = DataControlRowType.DataRow Then
                If GR.Cells(0).Text.ToUpper <> "TOTAL" Then
                    'Dim str As String = "INSERT INTO MMM_MST_DOC_ITEM("
                    Dim STRFld As String = ""
                    Dim STRVal As String = ""
                    updquery = ""
                    For j As Integer = 0 To dtField.Rows.Count - 1
                        STRFld &= dtField.Rows(j).Item("fieldmapping").ToString & ","
                        Dim colValue As String = ""
                        Dim FldID As String = ""
                        FldID = dtField.Rows(j).Item("fieldid").ToString()
                        If dtField.Rows(j).Item("inlineediting").ToString = "1" Then
                            Dim ftype As String = dtField.Rows(j).Item("fieldtype").ToString()
                            If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Then
                                If dtField.Rows(j).Item("datatype").ToString().ToUpper = "NUMERIC" And dtField.Rows(j).Item("isrequired") = 1 Then
                                    Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                    If cb IsNot Nothing Then
                                        colValue = cb.Text
                                        If cb.Text = "0" Then
                                            'updquery = "Enterd Child Value data shoul be greater than 0"
                                            Return 1
                                            Exit Function
                                        Else
                                        End If
                                    Else
                                        colValue = "0"
                                    End If
                                    If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                        If colValue <> "" Then
                                            Childtotal = Childtotal + Convert.ToDouble(colValue)
                                        End If
                                    End If
                                End If
                            End If
                            'If ftype.ToUpper() = "FILE UPLOADER" Then
                            '    '' here to code for getting file 
                            '    'Dim txtBox As AjaxControlToolkit.AjaxFileUpload = CType(GR.FindControl("fld" & FldID & GR.RowIndex), AjaxControlToolkit.AjaxFileUpload)
                            '    'txtBox.AllowedFileTypes = dtField.Rows(j).Item("UploadAllowedTypes").ToString()
                            '    Dim txtBox As FileUpload = CType(GR.FindControl("fld" & FldID & cnt), FileUpload)
                            '    'If txtBox.HasFile Then
                            '    '    Dim FN As String = ""
                            '    '    Dim ext As String = ""
                            '    '    Dim flag As Integer = 0
                            '    '    FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                            '    '    ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))

                            '    '    If IsNothing(dtField.Rows(j).Item("UploadAllowedTypes")) = True Then
                            '    '    Else
                            '    '        Dim type As String() = Split(dtField.Rows(j).Item("UploadAllowedTypes").ToString(), ",")
                            '    '        For k As Integer = 0 To type.Length - 1
                            '    '            If type(k) = ext Then
                            '    '                flag = 0 ' if file type is match then passed and saved into DB
                            '    '                Exit For
                            '    '            Else
                            '    '                flag = 1 ' check for type of the not matched 
                            '    '            End If
                            '    '        Next
                            '    '        If flag = 0 Then
                            '    '        Else
                            '    '            Return 1
                            '    '            Exit Function
                            '    '        End If

                            '    '    End If
                            '    ''colValue = Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & "" & ext
                            '    ''txtBox.SaveAs(Server.MapPath("DOCS/") & Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & ext)
                            'End If
                            'End If
                        End If
                    Next
                    cnt += 1
                End If
            End If
        Next
        ODA.Dispose()
        con.Close()
        con.Dispose()
    End Function
End Class
