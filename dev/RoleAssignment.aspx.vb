Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Partial Class UserMaster
    Inherits System.Web.UI.Page
    Dim obDMS As New DMSUtil()
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Session("DATA") Is Nothing Then
            Dim ds As DataSet
            ds = CType(Session("DATA"), DataSet)
            If ds.Tables("Data").Rows.Count > 0 Then
                CreatePanelOnRoles(ds)
            End If
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Users  
            Dim da As New SqlDataAdapter("select (rtrim(username) + '(' + UserID + ')-' + userrole )[username],uid from mmm_mst_user where eid=" & Session("EID") & " and userrole not in ('su')  order by username", con)
            Dim ds As New DataSet
            da.Fill(ds, "user")
            ddluser.Items.Add("Select")
            For i As Integer = 0 To ds.Tables("user").Rows.Count - 1
                ddluser.Items.Add(ds.Tables("user").Rows(i).Item("username").ToString())
                ddluser.Items(i + 1).Value = ds.Tables("user").Rows(i).Item("uid").ToString()
            Next

            da.SelectCommand.CommandText = "select roleid,rolename From MMM_MST_ROLE where EID=" & Session("EID").ToString() & " and roletype='Post Type'"
            da.Fill(ds, "role")

            ddluserrole.Items.Add("Select")
            For i As Integer = 0 To ds.Tables("role").Rows.Count - 1
                ddluserrole.Items.Add(ds.Tables("role").Rows(i).Item("rolename").ToString())
                ddluserrole.Items(i + 1).Value = ds.Tables("role").Rows(i).Item("roleid").ToString()
            Next

            da.SelectCommand.CommandText = "select FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
            da.Fill(ds, "doctype")

            da.Dispose()
            con.Dispose()

            For i As Integer = 0 To ds.Tables("doctype").Rows.Count - 1
                chkDocumentType.Items.Add(ds.Tables("doctype").Rows(i).Item("Formcaption").ToString())
                chkDocumentType.Items(i).Value = ds.Tables("doctype").Rows(i).Item("formname").ToString()
            Next

            chkDocType.InputAttributes("onchange") = "r2('" + chkDocType.ClientID + "','" + chkDocumentType.ClientID + "')"
            Uncheck()
            BindRolesMasters()
        End If
    End Sub
    Public Sub Uncheck()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        da.SelectCommand.CommandText = "select FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
        da.Fill(ds, "doctype")
        For i As Integer = 0 To ds.Tables("doctype").Rows.Count - 1
            chkDocumentType.Items(i).Selected = False
        Next
        da.Dispose()
        con.Dispose()
    End Sub
    Public Sub BindDocType()
        If ddluser.SelectedItem.Text.ToUpper() = "SELECT" Then
            lblMsgupdate.Text = "Please select valid User"
            updMsg.Update()
            Exit Sub
        Else
            lblMsgupdate.Text = ""
        End If

        If ddluserrole.SelectedItem.Text.ToUpper() = "SELECT" Then
            lblMsgupdate.Text = "Please select valid user role"
            updMsg.Update()
            Exit Sub
        Else
            lblMsgupdate.Text = ""
        End If

        ddlSeq.Items.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Users  
        Dim strQry As String = "Select tid,documenttype from MMM_REF_ROLE_USER where eid=" & Session("EID").ToString() & " and rolename='" & ddluserrole.SelectedItem.Text & "' and UID = " & ddluser.SelectedItem.Value
        Dim da As New SqlDataAdapter(strQry, con)
        Dim dt As New DataTable
        da.Fill(dt)

        ddlSeq.Items.Add("NEW")
        For i As Integer = 0 To dt.Rows.Count - 1
            ddlSeq.Items.Add(dt.Rows(i).Item("documenttype").ToString())
            ddlSeq.Items(i + 1).Value = dt.Rows(i).Item("tid").ToString()
        Next

        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)

        updMsg.Update()
        da.Dispose()
        con.Dispose()
    End Sub
    Public Sub BindRolesMasters()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        '        Dim da As New SqlDataAdapter("select distinct dropdown from MMM_MST_FIELDS WHERE eid=" & Session("EID").ToString() & " and substring(replace(dropdown,substring(dropdown,0,charindex('-',dropdown)+1),''),0,charindex('-',replace(dropdown,substring(dropdown,0,charindex('-',dropdown)+1),''))) in (select formname from MMM_MST_FORMS WHERE eid=" & Session("EID").ToString() & " and isRoleDef=1) order by dropdown", con)
        Dim da As New SqlDataAdapter("select * from MMM_MST_FORMS WHERE eid=" & Session("EID").ToString() & " and isRoleDef=1 and visibleinroleassignment=1 ", con)

        Dim ds As New DataSet
        da.Fill(ds, "data")
        Dim qry As String = String.Empty
        'For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
        '    Dim dType As String = ds.Tables("data").Rows(i).Item("FORMTYPE").ToString()
        '    Dim tbName As String = String.Empty
        '    Select Case dType
        '        Case "MASTER"
        '            tbName = "MMM_MST_MASTER"
        '        Case "DOCUMENT"
        '            tbName = "MMM_MST_DOC"
        '    End Select
        '    qry &= "Select distinct documenttype,fld1,tid from " & tbName & " WHERE EID=" & Session("EID").ToString() & " and documenttype='" & ds.Tables("data").Rows(i).Item("FORMName").ToString() & "' UNION "
        'Next
        'qry = Left(qry, Len(qry) - 6)
        'da.SelectCommand.CommandText = qry
        'da.Fill(ds, "maindata")
        Session("DATA") = ds
        da.Dispose()
        con.Dispose()
        ' CreatePanelOnRoles(ds)
    End Sub

    ''onCheckedChanged function by Rajat Bansal   ...commented by bally on 2 april 15
    'Private Sub onCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim dab As New SqlDataAdapter("", con)
    '    Dim dt As New DataTable
    '    Dim ds As DataSet
    '    Dim chlistr As New CheckBoxList
    '    Dim b As String = String.Empty
    '    ds = CType(Session("DATA"), DataSet)
    '    For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
    '        Dim dispName As String = ds.Tables("data").Rows(i).Item("FORMNAME").ToString()
    '        chlistr = CType(pnlFields.FindControl("chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()), CheckBoxList)
    '        For Each chkitem As System.Web.UI.WebControls.ListItem In chlistr.Items
    '            If chkitem.Selected = True And dispName = "State" Then
    '                b = b + chkitem.Value + ","
    '                'dt = dtb
    '            End If
    '        Next
    '    Next
    '    If b.Length > 0 Then
    '        b = b.Remove(b.Length - 1)
    '        dab.SelectCommand.CommandText = ViewState("bb") + "and  fld10 in (" + b + ") order by fld1"
    '        dab.SelectCommand.CommandType = CommandType.Text
    '        dab.Fill(dt)
    '        chlistr = CType(pnlFields.FindControl("chklist" & ViewState("Cidb")), CheckBoxList)
    '        chlistr.Items.Clear()
    '        For J As Integer = 0 To dt.Rows.Count - 1
    '            chlistr.Items.Add(" " & dt.Rows(J).Item("fld1"))
    '            chlistr.Items(J).Value = dt.Rows(J).Item("tid")
    '            chlistr.Height = 200
    '        Next
    '    Else
    '        dab.SelectCommand.CommandText = ViewState("bb")
    '        dab.SelectCommand.CommandType = CommandType.Text
    '        dab.Fill(dt)
    '        chlistr = CType(pnlFields.FindControl("chklist" & ViewState("Cidb")), CheckBoxList)
    '        chlistr.Items.Clear()
    '        For J As Integer = 0 To dt.Rows.Count - 1
    '            chlistr.Items.Add(" " & dt.Rows(J).Item("fld1"))
    '            chlistr.Items(J).Value = dt.Rows(J).Item("tid")
    '            chlistr.Height = 200
    '        Next

    '    End If
    '    dab.Dispose()
    '    ds.Dispose()
    '    con.Dispose()
    'End Sub


    Private Sub onCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dab As New SqlDataAdapter("", con)

        Dim ds As DataSet
        Dim flag As Integer = 0
        Dim chlistr As New CheckBoxList

        ds = CType(Session("DATA"), DataSet)
        Dim docstr As String = ""
        Dim flg As Integer = 0
        'ViewState("lstfilter") = lstfilter
        'ViewState("lstqry") = lstqry
        Dim doct As String = ""

        Dim lst As List(Of String) = TryCast(ViewState("lstfilter"), List(Of String))
        Dim lstqry As List(Of String) = TryCast(ViewState("lstqry"), List(Of String))
        Try
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1 ' loop for which isroledef on 
                Dim frmname As String = ds.Tables("data").Rows(i).Item("formname").ToString()
                If IsNothing(ViewState("dispName")) Then
                    For k As Integer = 0 To lst.Count - 1 ' loop for list state-vendor, state-city,state-circle office,city-vendor,city-circle office  etc...
                        Dim lstarr As String() = Split(lst(k), "||")
                        Dim lstname As String = lstarr(0)
                        If lstarr(0) = ds.Tables("data").Rows(i).Item("formname").ToString() Then ' if state=state then it will excute  
                            'Dim var As String = lstarr(0)

                            Dim ckid As String = ""
                            For M As Integer = 0 To lstqry.Count - 1
                                Dim arr As String() = Split(lstqry(M), "||")
                                If arr(1) = frmname Then
                                    ckid = arr(2)  'here is the id of checkbox
                                End If
                            Next
                            For j As Integer = 0 To lstqry.Count - 1
                                Dim lstqryarr As String() = Split(lstqry(j), "||") ' saperating qry which are stored in at the time creation
                                Dim lstqryarr1 As String = lstqryarr(1)
                                Dim lstarr1 As String = lstarr(1)
                                If lstarr(1).ToUpper = lstqryarr(1).ToUpper Then
                                    ' finding the id of checkbox from querry 
                                    Dim b As String = String.Empty
                                    ' checking check boxex is checked or not 
                                    chlistr = CType(pnlFields.FindControl("chklist" & ckid.ToString()), CheckBoxList)
                                    For Each chkitem As System.Web.UI.WebControls.ListItem In chlistr.Items
                                        If chkitem.Selected = True Then ' checking the check boxes  
                                            b = b + chkitem.Value + ","
                                        End If
                                    Next
                                    b = b.TrimEnd(",", "")
                                    If b.Length > 1 Then
                                        Dim dt As New DataTable
                                        'dab.SelectCommand.CommandText = fltdoc(0) + "and  " & arr(2) & " in (" + b + ") order by " & arr(2) & ""
                                        dab.SelectCommand.CommandText = lstqryarr(0) + "and  " & lstarr(2) & " in (" + b + ")"
                                        dab.SelectCommand.CommandType = CommandType.Text
                                        dab.Fill(dt)
                                        'docstr &= arr(1) & ","  ' here stre  the document name  which is excuted for excuting only one time 
                                        ViewState("dispName") &= frmname & ","
                                        chlistr = CType(pnlFields.FindControl("chklist" & lstqryarr(2)), CheckBoxList)
                                        chlistr.Items.Clear()
                                        For M As Integer = 0 To dt.Rows.Count - 1
                                            chlistr.Items.Add(" " & dt.Rows(M).Item(lstqryarr(3)))
                                            chlistr.Items(M).Value = dt.Rows(M).Item("tid")
                                            chlistr.Height = 200
                                        Next
                                        dt.Clear()
                                        dt.Dispose()
                                        Exit For
                                    End If
                                End If
                            Next

                        End If
                    Next
                Else
                    If ViewState("dispName").ToString.Contains(frmname) Then
                        'Dim array As String() = Split(ViewState("dispName"), ",")
                    Else
                        For k As Integer = 0 To lst.Count - 1 ' loop for list state-vendor, state-city,state-circle office,city-vendor,city-circle office  etc...
                            Dim lstarr As String() = Split(lst(k), "||")
                            Dim lstname As String = lstarr(0)
                            If lstarr(0) = ds.Tables("data").Rows(i).Item("formname").ToString() Then ' if state=state then it will excute  
                                'Dim var As String = lstarr(0)

                                Dim ckid As String = ""
                                For M As Integer = 0 To lstqry.Count - 1
                                    Dim arr As String() = Split(lstqry(M), "||")
                                    If arr(1) = frmname Then
                                        ckid = arr(2)  'here is the id of checkbox
                                    End If
                                Next
                                For j As Integer = 0 To lstqry.Count - 1
                                    Dim lstqryarr As String() = Split(lstqry(j), "||") ' saperating qry which are stored in at the time creation
                                    Dim lstqryarr1 As String = lstqryarr(1)
                                    Dim lstarr1 As String = lstarr(1)
                                    If lstarr(1).ToUpper = lstqryarr(1).ToUpper Then
                                        ' finding the id of checkbox from querry 
                                        Dim b As String = String.Empty
                                        ' checking check boxex is checked or not 
                                        chlistr = CType(pnlFields.FindControl("chklist" & ckid.ToString()), CheckBoxList)
                                        For Each chkitem As System.Web.UI.WebControls.ListItem In chlistr.Items
                                            If chkitem.Selected = True Then ' checking the check boxes  
                                                b = b + chkitem.Value + ","
                                            End If
                                        Next
                                        b = b.TrimEnd(",", "")
                                        If b.Length > 1 Then
                                            Dim dt As New DataTable
                                            'dab.SelectCommand.CommandText = fltdoc(0) + "and  " & arr(2) & " in (" + b + ") order by " & arr(2) & ""
                                            dab.SelectCommand.CommandText = lstqryarr(0) + "and  " & lstarr(2) & " in (" + b + ")"
                                            dab.SelectCommand.CommandType = CommandType.Text
                                            dab.Fill(dt)
                                            'docstr &= arr(1) & ","  ' here stre  the document name  which is excuted for excuting only one time 
                                            ViewState("dispName") &= frmname & ","
                                            chlistr = CType(pnlFields.FindControl("chklist" & lstqryarr(2)), CheckBoxList)
                                            chlistr.Items.Clear()
                                            For M As Integer = 0 To dt.Rows.Count - 1
                                                chlistr.Items.Add(" " & dt.Rows(M).Item(lstqryarr(3)))
                                                chlistr.Items(M).Value = dt.Rows(M).Item("tid")
                                                chlistr.Height = 200
                                            Next
                                            dt.Clear()
                                            dt.Dispose()
                                            Exit For
                                        End If
                                    End If
                                Next

                            End If
                        Next



                    End If
                End If

            Next

        Catch ex As Exception

        Finally
            dab.Dispose()
            ds.Dispose()
            con.Dispose()
        End Try
        dab.Dispose()
        ds.Dispose()
        con.Dispose()
    End Sub


    ''' COMMENTTED ON 4 APRIL
    'Private Sub onCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim dab As New SqlDataAdapter("", con)

    '    Dim ds As DataSet
    '    Dim flag As Integer = 0
    '    Dim chlistr As New CheckBoxList
    '    Dim b As String = String.Empty
    '    ds = CType(Session("DATA"), DataSet)
    '    Dim docstr As String = ""
    '    Dim flg As Integer = 0
    '    'ViewState("lstfilter") = lstfilter
    '    'ViewState("lstqry") = lstqry
    '    Dim doct As String = ""
    '    Dim lst As List(Of String) = TryCast(ViewState("lstfilter"), List(Of String))
    '    Dim lstqry As List(Of String) = TryCast(ViewState("lstqry"), List(Of String))
    '    Try
    '        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1 ' loop for which isroledef on 
    '            Dim dispName As String = ds.Tables("data").Rows(i).Item("FORMNAME").ToString()
    '            chlistr = CType(pnlFields.FindControl("chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()), CheckBoxList)
    '            'If ViewState("dispName") <> dispName Then
    '            For k As Integer = 0 To lst.Count - 1

    '                Dim arr As String() = Split(lst(k), "||") ' if state=state then it will excute  
    '                If arr(0).ToString = dispName Then
    '                    If docstr.Contains(arr(1)) = True Then 'checking for one time loop only if it exute then do not bind again for same doctype. 
    '                        Exit For
    '                    End If

    '                    'If ViewState("dispName").Contains(arr(1)) = True Then 'checking for one time loop only if it exute then do not bind again for same doctype. 
    '                    '    Exit For
    '                    'End If
    '                    For Each chkitem As System.Web.UI.WebControls.ListItem In chlistr.Items
    '                        If chkitem.Selected = True Then ' checking the check boxes  
    '                            b = b + chkitem.Value + ","
    '                        End If
    '                    Next
    '                    If b.Length > 0 Then  ' if any field is checked then it goes inside the function
    '                        b = b.TrimEnd(",", "")

    '                        For j As Integer = 0 To lstqry.Count - 1
    '                            Dim fltdoc As String() = Split(lstqry(j), "||")

    '                            If arr(1).ToUpper = fltdoc(1).ToUpper Then
    '                                Dim dt As New DataTable
    '                                'dab.SelectCommand.CommandText = fltdoc(0) + "and  " & arr(2) & " in (" + b + ") order by " & arr(2) & ""
    '                                dab.SelectCommand.CommandText = fltdoc(0) + "and  " & arr(2) & " in (" + b + ")"
    '                                dab.SelectCommand.CommandType = CommandType.Text
    '                                dab.Fill(dt)
    '                                docstr &= arr(1) & ","  ' here stre  the document name  which is excuted for excuting only one time 
    '                                ViewState("dispName") &= dispName & ","
    '                                chlistr = CType(pnlFields.FindControl("chklist" & fltdoc(2)), CheckBoxList)
    '                                chlistr.Items.Clear()
    '                                For M As Integer = 0 To dt.Rows.Count - 1
    '                                    chlistr.Items.Add(" " & dt.Rows(M).Item(fltdoc(3)))
    '                                    chlistr.Items(M).Value = dt.Rows(M).Item("tid")
    '                                    chlistr.Height = 200
    '                                Next
    '                                dt.Clear()
    '                                dt.Dispose()
    '                                Exit For
    '                            End If
    '                        Next

    '                    Else
    '                        ' here is code for not checked any value
    '                        'For j As Integer = 0 To lstqry.Count - 1
    '                        '    Dim fltdoc As String() = Split(lstqry(j), "||")
    '                        '    If arr(1).ToUpper = fltdoc(1).ToUpper Then
    '                        '        Dim dt As New DataTable
    '                        '        'dab.SelectCommand.CommandText = fltdoc(0) + "and  " & arr(2) & " in (" + b + ") order by " & arr(2) & ""
    '                        '        dab.SelectCommand.CommandText = fltdoc(0)
    '                        '        dab.SelectCommand.CommandType = CommandType.Text
    '                        '        dab.Fill(dt)
    '                        '        docstr &= arr(1) & ","  ' here stre  the document name  which is excuted for excuting only one time 
    '                        '        chlistr = CType(pnlFields.FindControl("chklist" & fltdoc(2)), CheckBoxList)
    '                        '        chlistr.Items.Clear()
    '                        '        For M As Integer = 0 To dt.Rows.Count - 1
    '                        '            chlistr.Items.Add(" " & dt.Rows(M).Item(fltdoc(3)))
    '                        '            chlistr.Items(M).Value = dt.Rows(M).Item("tid")
    '                        '            chlistr.Height = 200
    '                        '        Next
    '                        '        dt.Clear()
    '                        '        dt.Dispose()
    '                        '    End If
    '                        'Next
    '                    End If
    '                End If

    '            Next
    '            'End If
    '        Next

    '    Catch ex As Exception

    '    Finally
    '        dab.Dispose()
    '        ds.Dispose()
    '        con.Dispose()
    '    End Try
    '    dab.Dispose()
    '    ds.Dispose()
    '    con.Dispose()
    'End Sub

    Private Sub CreatePanelOnRoles(ByRef ds As DataSet)
        pnlFields.Controls.Add(New LiteralControl("<div><table width=""100%"" cellspacing=""2px"" border=""1"" cellpadding=""0px"">"))
        Dim lblWidth As Integer = 130
        Dim controlWdth As Integer = 220
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim lstqry As New List(Of String)
        Dim lstfilter As New List(Of String)
        'For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
        Try

            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim flg As Integer = 0
                Dim dispName As String = ds.Tables("data").Rows(i).Item("FORMNAME").ToString()

                Dim fldmap As String = ""
                da.SelectCommand.CommandText = "Select fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & dispName.ToString() & "' and showinroleassignment=1"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                fldmap = da.SelectCommand.ExecuteScalar() ' only one should be fieldmapping 
                If i Mod 2 = 0 Then
                    pnlFields.Controls.Add(New LiteralControl("<tr>"))
                End If

                pnlFields.Controls.Add(New LiteralControl("<td style=""width:50%;text-align:left"">"))
                pnlFields.Controls.Add(New LiteralControl("<div class=""form"" style=""overflow-Y:scroll;width:90%;height:200px""><h2>"))
                Dim FormID As String = ds.Tables("data").Rows(i).Item("FormID").ToString()
                Dim chklist As New CheckBoxList
                chklist.ID = "chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                chklist.CssClass = "txtbox"

                ''''''''''
                Dim chkAll As New CheckBox
                chkAll.ID = "chkAll" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                chkAll.CssClass = "txtbox"
                chkAll.InputAttributes("onchange") = "r('" + chkAll.ClientID + "','" + chklist.ClientID + "')"
                pnlFields.Controls.Add(chkAll)
                pnlFields.Controls.Add(New LiteralControl(" " & dispName & "</h2>"))
                For k As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    Dim formname As String = ds.Tables("data").Rows(k).Item("FORMNAME").ToString()
                    'checking the documenttype of where it is contained or not  in dropdown feilds  '
                    da.SelectCommand.CommandText = "select dropdown,fieldmapping from MMM_MSt_Fields where eid='" & Session("EID") & "' and documenttype='" & formname.ToString() & "' and dropdown like ('%" & dispName & "%')  "

                    da.Fill(ds, "filter")
                    If ds.Tables("filter").Rows.Count > 0 Then
                        For j As Integer = 0 To ds.Tables("filter").Rows.Count - 1
                            Dim arr As String() = Split(ds.Tables("filter").Rows(j).Item(0).ToString, "-")
                            If dispName = arr(1) Then ' checking the next filtering 
                                chklist.AutoPostBack = True
                                AddHandler chklist.SelectedIndexChanged, AddressOf onCheckedChanged
                                chkAll.AutoPostBack = True
                                ViewState("filterDoc") = formname.ToString
                                lstfilter.Add(dispName & "||" & formname & "||" & ds.Tables("filter").Rows(j).Item("fieldmapping"))
                                Exit For
                                'flg = 0
                            End If
                        Next
                    End If
                    ds.Tables("Filter").Clear()
                    ds.Tables("filter").Dispose()
                Next
                Dim dType As String = ds.Tables("data").Rows(i).Item("FORMTYPE").ToString()
                Dim tbName As String = String.Empty
                Dim qry As String = String.Empty

                Select Case dType
                    Case "MASTER"
                        tbName = "MMM_MST_MASTER"
                    Case "DOCUMENT"
                        tbName = "MMM_MST_DOC"
                End Select
                If lstfilter.Count > 0 Then
                    Dim arr As String() = Split(lstfilter(0), "-")
                    If dispName.Trim().ToUpper = "VENDOR" And Session("EID").ToString() = "46" Then
                        qry = "Select distinct top 2000  documenttype," & fldmap & ",tid from " & tbName & " WHERE EID=" & Session("EID").ToString() & " and documenttype='" & dispName & "' "
                    Else
                        qry = "Select distinct documenttype," & fldmap & ",tid from " & tbName & " WHERE EID=" & Session("EID").ToString() & " and documenttype='" & dispName & "' "
                    End If

                    If i > 0 And dt.Rows.Count > 0 Then
                        dt.Rows.Clear()
                    End If
                    da.SelectCommand.CommandText = qry
                    lstqry.Add(qry & "||" & dispName & "||" & ds.Tables("data").Rows(i).Item("FORMID").ToString() & "||" & fldmap)
                    da.Fill(dt)
                    For J As Integer = 0 To dt.Rows.Count - 1
                        chklist.Items.Add(" " & dt.Rows(J).Item(fldmap))
                        chklist.Items(J).Value = dt.Rows(J).Item("tid")
                        chklist.Height = 200
                    Next
                Else
                    If IsNothing(fldmap) Then
                        fldmap = "fld1"  ' by default making field mapping as fld1 , if showinroleassignment is not 1 
                    End If
                    If dispName.Trim().ToUpper = "VENDOR" And Session("EID").ToString() = "46" Then
                        qry = "Select distinct top 2000  documenttype," & fldmap & ",tid from " & tbName & " WHERE EID=" & Session("EID").ToString() & " and documenttype='" & dispName & "' "
                    Else
                        qry = "Select distinct documenttype," & fldmap & ",tid from " & tbName & " WHERE EID=" & Session("EID").ToString() & " and documenttype='" & dispName & "' "
                    End If

                    If i > 0 And dt.Rows.Count > 0 Then
                        dt.Rows.Clear()
                    End If
                    da.SelectCommand.CommandText = qry
                    da.Fill(dt)
                    lstqry.Add(qry & "||" & dispName & "||" & ds.Tables("data").Rows(i).Item("FORMID").ToString() & "||" & fldmap)
                    For J As Integer = 0 To dt.Rows.Count - 1
                        chklist.Items.Add(" " & dt.Rows(J).Item(fldmap))
                        chklist.Items(J).Value = dt.Rows(J).Item("tid")
                        chklist.Height = 200
                    Next
                End If
                chklist.Attributes.Item("OnChange") = "RMH('" + chklist.ClientID + "','" + chkAll.ClientID + "')"
                pnlFields.Controls.Add(chklist)
                pnlFields.Controls.Add(New LiteralControl("</div>"))
                pnlFields.Controls.Add(New LiteralControl("</td>"))
                If i Mod 2 = 1 Then
                    pnlFields.Controls.Add(New LiteralControl("</tr>"))
                End If
            Next
        Catch ex As Exception

        Finally
            da.Dispose()
            con.Dispose()
        End Try

        ViewState("lstfilter") = lstfilter
        ViewState("lstqry") = lstqry
        pnlFields.Controls.Add(New LiteralControl("</table></div>"))
        da.Dispose()
        con.Dispose()
        updControls.Update()
    End Sub

    'Private Sub CreatePanelOnRoles(ByRef ds As DataSet)
    '    pnlFields.Controls.Add(New LiteralControl("<div><table width=""100%"" cellspacing=""2px"" border=""1"" cellpadding=""0px"">"))
    '    Dim lblWidth As Integer = 130
    '    Dim controlWdth As Integer = 220
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim dt As New DataTable

    '    'For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
    '    For i As Integer = 0 To ds.Tables("data").Rows.Count - 1

    '        Dim dispName As String = ds.Tables("data").Rows(i).Item("FORMNAME").ToString()
    '        'Dim lbl As New Label
    '        'lbl.ID = "lbl" & ds.Tables("data").Rows(i).Item("FormID").ToString()
    '        'lbl.Text = dispName
    '        'lbl.Font.Bold = True

    '        If i Mod 2 = 0 Then
    '            pnlFields.Controls.Add(New LiteralControl("<tr>"))
    '        End If

    '        pnlFields.Controls.Add(New LiteralControl("<td style=""width:50%;text-align:left"">"))
    '        pnlFields.Controls.Add(New LiteralControl("<div class=""form"" style=""overflow-Y:scroll;width:90%;height:200px""><h2>"))
    '        Dim FormID As String = ds.Tables("data").Rows(i).Item("FormID").ToString()
    '        Dim chklist As New CheckBoxList
    '        chklist.ID = "chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()
    '        chklist.CssClass = "txtbox"



    '        ''''''''''
    '        Dim chkAll As New CheckBox
    '        chkAll.ID = "chkAll" & ds.Tables("data").Rows(i).Item("FormID").ToString()
    '        chkAll.CssClass = "txtbox"
    '        chkAll.InputAttributes("onchange") = "r('" + chkAll.ClientID + "','" + chklist.ClientID + "')"
    '        pnlFields.Controls.Add(chkAll)
    '        pnlFields.Controls.Add(New LiteralControl(" " & dispName & "</h2>"))
    '        'code By Rajat Bansal
    '        If dispName = "State" Then
    '            chklist.AutoPostBack = True
    '            AddHandler chklist.SelectedIndexChanged, AddressOf onCheckedChanged
    '            chkAll.AutoPostBack = True
    '        End If
    '        Dim dType As String = ds.Tables("data").Rows(i).Item("FORMTYPE").ToString()
    '        Dim tbName As String = String.Empty

    '        Dim qry As String = String.Empty
    '        Select Case dType
    '            Case "MASTER"
    '                tbName = "MMM_MST_MASTER"
    '            Case "DOCUMENT"
    '                tbName = "MMM_MST_DOC"
    '        End Select


    '        Dim str As String = String.Empty
    '        da.SelectCommand.CommandText = "Select fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & dispName.ToString() & "' and showinroleassignment=1"
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        str = da.SelectCommand.ExecuteScalar() ' only one should be fieldmapping 

    '        If str Is Nothing Then
    '            str = "fld1"
    '        End If
    '        qry = "Select distinct documenttype," & str & ",tid from " & tbName & " WHERE EID=" & Session("EID").ToString() & " and documenttype='" & ds.Tables("data").Rows(i).Item("FORMName").ToString() & "' "

    '        If i > 0 And dt.Rows.Count > 0 Then
    '            dt.Rows.Clear()
    '        End If
    '        da.SelectCommand.CommandText = qry
    '        da.Fill(dt)
    '        ' ViewState("bb") = qry
    '        For J As Integer = 0 To dt.Rows.Count - 1
    '            chklist.Items.Add(" " & dt.Rows(J).Item(str))
    '            chklist.Items(J).Value = dt.Rows(J).Item("tid")
    '            chklist.Height = 200
    '        Next

    '        If ds.Tables("data").Rows(i).Item("FORMNAME").ToString() = "City" Then
    '            ViewState("Cidb") = ds.Tables("data").Rows(i).Item("FormID").ToString()
    '            ViewState("bb") = qry
    '        End If
    '        'Rajat

    '        pnlFields.Controls.Add(chklist)
    '        pnlFields.Controls.Add(New LiteralControl("</div>"))
    '        pnlFields.Controls.Add(New LiteralControl("</td>"))
    '        If i Mod 2 = 1 Then
    '            pnlFields.Controls.Add(New LiteralControl("</tr>"))
    '        End If
    '    Next
    '    pnlFields.Controls.Add(New LiteralControl("</table></div>"))
    '    da.Dispose()
    '    con.Dispose()
    '    updControls.Update()
    'End Sub

    Private Sub FillPanelOnRoles(ByRef ds As DataSet)

        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)

        If ddlSeq.SelectedItem.Text = "NEW" Then
            'clear controls selection and changed button to new
            btnSave.Text = "Save New"
            chkCreateRight.Checked = False
            chkEditRight.Checked = False
            chkViewRight.Checked = False
            updMsg.Update()
        Else
            'Fill panel and update Text
            btnSave.Text = "Update"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill User Record  
            Dim strQry As String = ""
            strQry = "Select * from MMM_REF_ROLE_USER where tid=" & ddlSeq.SelectedItem.Value
            Dim da As New SqlDataAdapter(strQry, con)
            Dim dt As New DataTable
            da.Fill(dt)
            Dim isCreate As Integer = dt.Rows(0).Item("iscreate")
            Dim isEdit As Integer = dt.Rows(0).Item("isedit")
            Dim isView As Integer = dt.Rows(0).Item("isview")
            If isCreate = 1 Then
                chkCreateRight.Checked = True
            Else
                chkCreateRight.Checked = False
            End If
            If isEdit = 1 Then
                chkEditRight.Checked = True
            Else
                chkEditRight.Checked = False
            End If
            If isView = 1 Then
                chkViewRight.Checked = True
            Else
                chkViewRight.Checked = False
            End If
            da.SelectCommand.CommandText = "select formid,FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
            Dim ds1 As New DataSet
            da.Fill(ds1, "doctype")
            If dt.Rows.Count = 0 Then
                lblMsgupdate.Text = "No role definetion found, please save to create"
                updMsg.Update()
                Exit Sub
            End If

            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim ControlID As String = "chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                Dim CHKControlID As String = "chkAll" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                Dim chkAll As New CheckBox
                chkAll = CType(pnlFields.FindControl(CHKControlID), CheckBox)
                Dim fldMapping As String = ds.Tables("data").Rows(i).Item("DocMapping").ToString()
                Dim chklist As New CheckBoxList
                chklist = CType(pnlFields.FindControl(ControlID), CheckBoxList)
                If IsDBNull(dt.Rows(0).Item(fldMapping).ToString()) Or dt.Rows(0).Item(fldMapping).ToString() = "" Then
                    Continue For
                End If
                Dim ARR() As String = dt.Rows(0).Item(fldMapping).ToString().Split(",")

                Dim namer As String = ds.Tables("data").Rows(i).Item("FORMNAME").ToString()

                'For Each chkitem As System.Web.UI.WebControls.ListItem In chklist.Items

                ''Next

                ' commentted on 8 april 15 
                'Dim dtr As New DataTable
                'If namer = "City" Then
                '    'b = b.Remove(b.Length - 1)
                '    da.SelectCommand.CommandText = ViewState("bb") + "and  tid in( " + dt.Rows(0).Item(fldMapping).ToString() + ") order by fld1"
                '    da.SelectCommand.CommandType = CommandType.Text
                '    da.Fill(dtr)
                '    chklist = CType(pnlFields.FindControl("chklist" & ViewState("Cidb")), CheckBoxList)
                '    chklist.Items.Clear()
                '    For J As Integer = 0 To dtr.Rows.Count - 1
                '        chklist.Items.Add(" " & dtr.Rows(J).Item("fld1"))
                '        chklist.Items(J).Value = dtr.Rows(J).Item("tid")
                '        chklist.Height = 200
                '    Next
                'End If
                'dtr.Dispose()
                If chklist.Items.Count = ARR.Length Then
                    chkAll.Checked = True
                Else
                    chkAll.Checked = False
                End If
                For ii As Integer = 0 To ARR.Length - 1
                    If ARR(ii).ToString <> "" Or IsDBNull(ARR(ii).ToString) = False Then
                        If ARR(ii).ToString() = "*" Then
                            chkAll.Checked = True
                            For Each li As ListItem In chklist.Items
                                li.Selected = True
                            Next
                        Else
                            chklist.Items.FindByValue(ARR(ii).ToString()).Selected = True
                        End If

                    End If
                Next
            Next

            For i As Integer = 0 To ds1.Tables("doctype").Rows.Count - 1
                Dim ARR() As String = dt.Rows(0).Item("documenttype").ToString().Split(",")
                For ii As Integer = 0 To ARR.Length - 1
                    If ARR(ii).ToString <> "" Or IsDBNull(ARR(ii).ToString) = False Then
                        If ARR(ii).ToString = ds1.Tables("doctype").Rows(i).Item("formname").ToString Then
                            chkDocumentType.Items(i).Selected = True
                        End If
                    End If
                Next
            Next

            da.Dispose()
            con.Dispose()
            updControls.Update()
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        Dim ds As New DataSet()
        ds = CType(Session("DATA"), DataSet)
        If ds.Tables("Data").Rows.Count > 0 Then
            FillPanelOnRoles(ds)
        End If
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' Validation
        Dim ds As New DataSet()
        ds = CType(Session("DATA"), DataSet)
        If ds.Tables("Data").Rows.Count > 0 Then
            'there are some master association
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'fill User Record  
            Dim DocType As String = ""

            For Each li As ListItem In chkDocumentType.Items
                If li.Selected Then
                    DocType &= li.Value & ","
                End If
            Next

            If DocType.Length < 3 Then
                lblMsgupdate.Text = "Please select some document type to apply rules"
                updMsg.Update()
                Exit Sub
            Else
                DocType = Left(DocType, Len(DocType) - 1)
            End If

            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select * from MMM_REF_ROLE_USER where tid=" & ddlSeq.SelectedItem.Value, con)
            Dim isCreate As Integer
            Dim isEdit As Integer
            Dim isView As Integer
            If chkCreateRight.Checked = True Then
                isCreate = 1
            Else
                isCreate = 0
            End If
            If chkEditRight.Checked = True Then
                isEdit = 1
            Else
                isEdit = 0
            End If
            If chkViewRight.Checked = True Then
                isView = 1
            Else
                isView = 0
            End If

            If ddlSeq.SelectedItem.Text.ToUpper() = "NEW" Then
                Dim qry As String = "INSERT INTO MMM_REF_ROLE_USER(EID,UID,rolename,documenttype,iscreate,isedit,isview"
                Dim qryValue As String = "VALUES (" & Session("EID").ToString() & "," & ddluser.SelectedItem.Value & ",'" & ddluserrole.SelectedItem.Text & "','" & DocType & "'," & isCreate & "," & isEdit & "," & isView & ","
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    Dim ControlID As String = "chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                    Dim chkcontrolid As String = "chkAll" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                    Dim fldMapping As String = ds.Tables("data").Rows(i).Item("DocMapping").ToString()
                    Dim chk As New CheckBox
                    chk = CType(pnlFields.FindControl(chkcontrolid), CheckBox)
                    Dim chklist As New CheckBoxList
                    chklist = CType(pnlFields.FindControl(ControlID), CheckBoxList)
                    Dim selectedValues As List(Of String) = chklist.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
                    If chk.Checked = True Then
                        qry &= "," & fldMapping
                        qryValue &= "'" & String.Join(",", selectedValues.ToArray) & "',"
                    Else
                        Dim livalue As String = ""
                        'For Each li As ListItem In chklist.Items
                        '    If li.Selected Then
                        '        livalue &= li.Value & ","
                        '    End If
                        'Next
                        livalue = "'" & String.Join(",", selectedValues.ToArray) & "'"
                        If livalue.Length > 4 Then
                            'livalue = Left(livalue, livalue.Length - 1)
                            qry &= "," & fldMapping
                            qryValue &= livalue & ","
                        End If
                    End If

                Next

                If IsMoreRecord() Then
                    Dim arrlist As New List(Of DataCollection)
                    arrlist = AddMoreRecord()
                    For Each order As DataCollection In arrlist
                        qry &= "," & order.Text
                        qryValue &= "'" & order.Value & "',"
                    Next
                End If


                qry = qry & ") "
                qryValue = Left(qryValue, Len(qryValue) - 1) & ")"
                'qryValue = qryValue & ")"
                da.SelectCommand.CommandText = qry & qryValue
                da.SelectCommand.CommandType = CommandType.Text
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()

                Dim ob As New DynamicForm
                ob.HistoryForRole(Session("EID"), ddluser.SelectedItem.Value, Session("UID"), DocType, "MMM_Ref_Role_User", "Add", "RoleAssignment", "Old Role Assignment Page")

                lblMsgupdate.Text = "Record Created."
            Else
                'we have to update
                Dim ob As New DynamicForm
                ob.HistoryForRole(Session("EID"), ddluser.SelectedItem.Value, Session("UID"), DocType, "MMM_Ref_Role_User", "UPDATE", "RoleAssignment", "Old Role Assignment Page", ddlSeq.SelectedItem.Value)

                Dim qry As String = "UPDATE MMM_REF_ROLE_USER SET documenttype='" & DocType & "',iscreate=" & isCreate & ",isedit=" & isEdit & ",isview=" & isView & ","
                Dim qryValue As String = " Where Tid=" & ddlSeq.SelectedItem.Value
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    Dim ControlID As String = "chklist" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                    Dim chkcontrolid As String = "chkAll" & ds.Tables("data").Rows(i).Item("FormID").ToString()
                    Dim fldMapping As String = ds.Tables("data").Rows(i).Item("DocMapping").ToString()
                    Dim chk As New CheckBox
                    chk = CType(pnlFields.FindControl(chkcontrolid), CheckBox)
                    Dim chklist As New CheckBoxList
                    chklist = CType(pnlFields.FindControl(ControlID), CheckBoxList)
                    Dim selectedValues As List(Of String) = chklist.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
                    If chk.Checked = True Then
                        qry &= fldMapping & "='" & String.Join(",", selectedValues.ToArray) & "',"
                    Else
                        Dim livalue As String = ""
                        'For Each li As ListItem In chklist.Items
                        '    If li.Selected Then
                        '        livalue &= li.Value & ","
                        '    End If
                        'Next
                        livalue = String.Join(",", selectedValues.ToArray)
                        If livalue.Length > 4 Then
                            'livalue = Left(livalue, livalue.Length - 1)
                            qry &= fldMapping & "='" & livalue & "',"
                        End If

                    End If
                Next
                qry = Left(qry, Len(qry) - 1) & qryValue
                da.SelectCommand.CommandText = qry
                da.SelectCommand.CommandType = CommandType.Text
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()

                lblMsgupdate.Text = "Record Updated"
            End If
            updMsg.Update()
            con.Dispose()
            da.Dispose()
        End If
    End Sub

    Protected Function IsMoreRecord() As Boolean
        Dim Bool As Boolean = False
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select count(*) as docmapping from mmm_mst_forms where isroledef=1 and  visibleinroleassignment=0 and eid=" & Session("EID")
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        If Convert.ToInt32(da.SelectCommand.ExecuteScalar()) <> 0 Then
            Bool = True
        End If
        Return Bool
    End Function


    Public Function AddMoreRecord() As List(Of DataCollection)
        Dim cuslist1 As List(Of DataCollection) = New List(Of DataCollection)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select docmapping from mmm_mst_forms where isroledef=1 and  visibleinroleassignment=0 and eid=" & Session("EID")
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Dim dr As SqlDataReader = da.SelectCommand.ExecuteReader()
        Dim arrlist As New ArrayList
        While (dr.Read())
            arrlist.Add(dr(0))
        End While
        dr.Close()
        For Each li As String In arrlist
            Dim dc As New DataCollection
            Dim arrvalue As New ArrayList
            da.SelectCommand.CommandText = "select tid from mmm_mst_master where documenttype in (select formname from mmm_mst_forms where docmapping='" & Convert.ToString(li) & "' and eid=" & Session("EID") & ") and eid=" & Session("EID") & ""
            Dim dt As New DataTable
            da.Fill(dt)
            For Each drr As DataRow In dt.Rows
                arrvalue.Add(Convert.ToString(drr(0)))
            Next
            dc.Text = li
            dc.Value = String.Join(",", arrvalue.ToArray)
            cuslist1.Add(dc)
        Next
        Return cuslist1
    End Function

    Public Class DataCollection
        Public Property Text As String
        Public Property Value As String
    End Class



    Protected Sub ddluserrole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddluserrole.SelectedIndexChanged
        BindDocType()
        Uncheck()

    End Sub

    Protected Sub ddluser_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddluser.SelectedIndexChanged
        BindDocType()
        Uncheck()
        btndelete.Visible = True
        If ddluserrole.SelectedItem.Text.ToUpper <> "SELECT" Then
            If ddlSeq.SelectedItem.Text = "NEW" Then
                'clear controls selection and changed button to new
                btnSave.Text = "Save New"
                chkCreateRight.Checked = False
                chkEditRight.Checked = False
                chkViewRight.Checked = False
                updMsg.Update()
            End If
        End If
    End Sub

    Protected Sub ddlSeq_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSeq.SelectedIndexChanged
        Dim ds As New DataSet()
        Uncheck()
        ds = CType(Session("DATA"), DataSet)
        If ds.Tables("Data").Rows.Count > 0 Then
            FillPanelOnRoles(ds)
        End If
    End Sub

    Protected Function BindGridToexport() As DataTable

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select r.tid, u.username,r.rolename,r.Documenttype,(case when iscreate = '0' then 'INACTIVE' when iscreate='1' then 'ACTIVE' end)[ISCreate],(case when isview ='0' then 'INACTIVE' when isview='1' then 'ACTIVE' end)[ISView],convert(nvarchar(500),r.fld1)[fld1],convert(nvarchar(500),r.fld4)[fld4] from MMM_Ref_Role_User r inner join mmm_mst_user u on u.uid=r.uid where u.eid=" & Session("EID") & " and r.eid=" & Session("EID") & " and r.fld1 is not null and r.fld4 is not null order by r.uid", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Dim st As String = ""
        Dim cir As String = ""
        Dim tid As Integer = 0
        Dim abc As String = ""
        Dim xyz As String = ""
        Dim dt As DataTable = New DataTable()
        If ds.Tables("data").Rows.Count > 0 Then
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                st = ds.Tables("data").Rows(i).Item("fld1").ToString()
                cir = ds.Tables("data").Rows(i).Item("fld4").ToString()
                tid = ds.Tables("data").Rows(i).Item("tid").ToString()
                abc = ""
                xyz = ""
                If st <> "" Then
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "USPgetState"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@tid", tid)
                    da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                    abc = da.SelectCommand.ExecuteScalar().ToString()

                End If
                If cir <> "" Then
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "USPgetCircle"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@tid", tid)
                    da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                    xyz = da.SelectCommand.ExecuteScalar().ToString()
                End If
                Dim query As String = "select row_number() over (order by u.username)[S.no.], u.username,r.rolename,r.Documenttype,(case when iscreate = '0' then 'INACTIVE' when iscreate='1' then 'ACTIVE' end)[ISCreate],(case when isview ='0' then 'INACTIVE' when isview='1' then 'ACTIVE' end)[ISView],'" & abc & "'[State], '" & xyz & "' [Circle] from MMM_Ref_Role_User r inner join mmm_mst_user u on u.uid=r.uid where u.eid=" & Session("EID") & " and r.eid=" & Session("EID") & " and r.fld1 is not null and r.fld4 is not null and tid = " & tid & " order by r.uid"
                Dim daa As New SqlDataAdapter(query, con)
                daa.Fill(dt)
            Next
            con.Close()

        End If
        ViewState("table") = dt
        Return dt

    End Function

    Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexport.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select docmapping from mmm_mst_forms with(nolock) where isroledef=1 and eid=" & Session("EID") & "", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        Dim fielmapping As String = ds.Tables("data").Rows(0).Item(0).ToString
        Dim qry As String = ";WITH cte AS(SELECT A.[UID],A.[Rolename],Split.a.value('.', 'VARCHAR(100)') AS Hub FROM (SELECT [UID],[Rolename], CAST ('<M>' + REPLACE([" & fielmapping & "], ',', '</M><M>') + '</M>' AS XML)[Hub] "
        qry &= "FROM mmm_ref_role_user with(nolock)  where eid=" & Session("EID") & " and  " & fielmapping & " is not null and " & fielmapping & "<>'')  AS A CROSS APPLY Hub.nodes ('/M') AS Split(a)) "
        qry &= "select distinct cte.[UID],u.username[UserName],cte.[Rolename],(select fld1 from mmm_mst_master with(nolock)  where tid=cte.[Hub])[Hub Name],((select fld1 from mmm_mst_master with(nolock)  where tid=cte.[Hub]) +'-'+ cte.[Rolename])[Hub/Role] from cte with(nolock)  inner join "
        qry &= "mmm_mst_user u with(nolock) on u.uid=cte.[UID] where u.eid=" & Session("EID") & ""
        da.SelectCommand.CommandText = qry
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandTimeout = 300
        da.Fill(ds, "Roledata")
        Dim GridView1 As New GridView()
        GridView1.DataSource = ds.Tables("Roledata")
        GridView1.DataBind()
        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=RoleAssignment.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GridView1.Rows.Count - 1
            'Apply text style to each Row 
            GridView1.Rows(i).Attributes.Add("class", "textmode")
        Next
        GridView1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
        da.Dispose()
        con.Close()
        con.Dispose()
    End Sub


    'Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexport.Click
    '    BindGridToexport()
    '    'Create a dummy GridView 
    '    Dim GridView1 As New GridView()
    '    GridView1.DataSource = ViewState("table")
    '    GridView1.DataBind()
    '    Response.Clear()
    '    Response.Buffer = True
    '    Response.AddHeader("content-disposition", "attachment;filename=RoleAssignment.xls")
    '    Response.Charset = ""
    '    Response.ContentType = "application/vnd.ms-excel"
    '    Dim sw As New StringWriter()
    '    Dim hw As New HtmlTextWriter(sw)
    '    For i As Integer = 0 To GridView1.Rows.Count - 1
    '        'Apply text style to each Row 
    '        GridView1.Rows(i).Attributes.Add("class", "textmode")
    '    Next
    '    GridView1.RenderControl(hw)
    '    'style to format numbers to string 
    '    Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
    '    Response.Write(style)
    '    Response.Output.Write(sw.ToString())
    '    Response.Flush()
    '    Response.End()

    'End Sub


    Protected Sub btndelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btndelete.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ob As New DynamicForm
        ob.HistoryForRole(Session("EID"), ddluser.SelectedItem.Value, Session("UID"), ddlSeq.SelectedItem.Text, "MMM_Ref_Role_User", "Delete", "RoleAssignment", "Old Role Assignment")

        If ddlSeq.SelectedIndex <> 0 Then
            Dim da As New SqlDataAdapter("delete  from MMM_Ref_Role_User where eid= " & Session("EID") & " and rolename= '" & ddluserrole.SelectedItem.Text & "' and uid= '" & ddluser.SelectedValue & "' and tid =" & ddlSeq.SelectedValue & "", con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteScalar()
            lblMsgupdate.Text = "Deleted Successfully"
            Response.Redirect("roleassignment.aspx")
            da.Dispose()
            con.Close()
        Else
            lblMsgupdate.Text = "Please Select Doc Type"
        End If


    End Sub

End Class


