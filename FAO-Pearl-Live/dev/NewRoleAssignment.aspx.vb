Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Partial Class NewRoleAssignment
    Inherits System.Web.UI.Page
    Dim obDMS As New DMSUtil()
    Dim objDC As New DataClass()
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Session("DATA") Is Nothing Then
            Dim ds As DataSet
            ds = CType(Session("DATA"), DataSet)
            If ds.Tables("Data").Rows.Count > 0 Then
                CreatePanelOnRoles(ds)
            End If
        End If
    End Sub
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
            Dim da As New SqlDataAdapter("select (rtrim(username) + '(' + UserID + ')'  )[username],uid from mmm_mst_user where eid=" & Session("EID") & " and userrole not in ('su') and isauth<>0  order by username", con)
            Dim ds As New DataSet
            da.Fill(ds, "user")
            ddluser.Items.Add("Select")
            For i As Integer = 0 To ds.Tables("user").Rows.Count - 1
                ddluser.Items.Add(ds.Tables("user").Rows(i).Item("username").ToString())
                ddluser.Items(i + 1).Value = ds.Tables("user").Rows(i).Item("uid").ToString()
            Next
            DocType.Visible = False
            PnlRights.Visible = False
            chkboxforup.Visible = False
            'da.SelectCommand.CommandText = "select roleid,rolename From MMM_MST_ROLE where EID=" & Session("EID").ToString() & " and roletype='Post Type' "
            Uncheck()
            BindRolesMasters()
        End If
        lblDtype.Visible = False
        ddddtype.Visible = False
    End Sub

    Private Sub doctype13()

        'chkDocType.InputAttributes("onchange") = "r2('" + chkDocType.ClientID + "','" + chkDocumentType.ClientID + "')"

        'da.SelectCommand.CommandText = "select FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
        'da.Fill(ds, "doctype")
        'da.Dispose()
        'con.Dispose()
        'For i As Integer = 0 To ds.Tables("doctype").Rows.Count - 1
        '    chkDocumentType.Items.Add(ds.Tables("doctype").Rows(i).Item("Formcaption").ToString())
        '    chkDocumentType.Items(i).Value = ds.Tables("doctype").Rows(i).Item("formname").ToString()
        'Next
    End Sub
    Private Sub doctype12()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select substring (PageLink, 19, 200) as 'PageLink' from mmm_mst_menu where eid=" & Session("EID") & " and Roles like +'%{'+'" & ddluserrole.SelectedValue & "'+':%' and pageLink like 'Documents.aspx%'" _
                                    & " Union" _
                                   & " select substring (PageLink, 17, 200) as 'PageLink' from mmm_mst_menu where eid=" & Session("EID") & " and Roles like +'%{'+'" & ddluserrole.SelectedValue & "'+':%' and  pageLink like 'Masters.aspx%'")
        If objDT.Rows.Count > 0 Then
            chkDocumentType.DataSource = objDT
            chkDocumentType.DataTextField = "PageLink"
            chkDocumentType.DataValueField = "PageLink"
            chkDocumentType.DataBind()
        Else
            chkDocumentType.Items.Clear()
        End If
        'Comcheck()
        For j As Integer = 0 To chkDocumentType.Items.Count - 1
            chkDocumentType.Items(j).Selected = True
        Next
        chkDocType.InputAttributes("onchange") = "r2('" + chkDocType.ClientID + "','" + chkDocumentType.ClientID + "')"

        'da.SelectCommand.CommandText = "select FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
        'da.Fill(ds, "doctype")
        'da.Dispose()
        'con.Dispose()
        'For i As Integer = 0 To ds.Tables("doctype").Rows.Count - 1
        '    chkDocumentType.Items.Add(ds.Tables("doctype").Rows(i).Item("Formcaption").ToString())
        '    chkDocumentType.Items(i).Value = ds.Tables("doctype").Rows(i).Item("formname").ToString()
        'Next
    End Sub
    Public Sub Comcheck()
        Using conn As SqlConnection = New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using cmd As New SqlCommand()
                conn.Open()
                cmd.CommandText = ("select * from mmm_ref_role_user  where Eid=" & Session("Eid") & " and UID = " & ddluser.SelectedItem.Value & " and rolename='" & ddluserrole.SelectedValue & "'")
                cmd.Connection = conn
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    While sdr.Read()
                        Dim c As String() = New String(50) {}
                        c = sdr("documenttype").ToString().Split(",")
                        Dim length2 As Integer = c.Length
                        For i As Integer = 0 To c.Length - 1
                            Dim cntry As String = c(i)
                            For j As Integer = 0 To chkDocumentType.Items.Count - 1
                                If chkDocumentType.Items(j).Value = c(i) Then
                                    chkDocumentType.Items(j).Selected = True
                                    Exit For
                                End If
                            Next
                        Next
                    End While
                End Using
                conn.Close()
            End Using
        End Using
    End Sub

    Public Sub Uncheck()

        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)
        'Dim da As New SqlDataAdapter("", con)
        'Dim ds As New DataSet
        'da.SelectCommand.CommandText = "select FormName,FormCaption FROM MMM_MST_FORMS Where FormSource='MENU DRIVEN' and  EID=" & Session("EID").ToString() & " Order by FormCaption"
        'da.Fill(ds, "doctype")
        'For i As Integer = 0 To ds.Tables("doctype").Rows.Count - 1
        '    chkDocumentType.Items(i).Selected = False
        'Next
        'da.Dispose()
        'con.Dispose()
    End Sub

    Public Sub BindDocType()
        If ddluser.SelectedItem.Text.ToUpper() = "SELECT" Then
            lblMsgupdate.Text = "Please select valid User"
            updMsg.Update()
            Exit Sub
        Else
            lblMsgupdate.Text = ""
        End If

        'If ddluserrole.SelectedItem.Text.ToUpper() = "SELECT" Then
        '    lblMsgupdate.Text = "Please select valid user role"
        '    updMsg.Update()
        '    Exit Sub
        'Else
        '    lblMsgupdate.Text = ""
        'End If

        ddlSeq.Items.Clear()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Users  
        Dim strQry As String = "Select tid,documenttype from mmm_ref_role_user  where eid=" & Session("EID").ToString() & " and rolename='" & ddluserrole.SelectedItem.Text & "' and UID = " & ddluser.SelectedItem.Value
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

        Dim da As New SqlDataAdapter("select * from MMM_MST_FORMS WHERE eid=" & Session("EID").ToString() & " and isRoleDef=1 and visibleinroleassignment=1 ", con)

        Dim ds As New DataSet
        da.Fill(ds, "data")
        Dim qry As String = String.Empty

        Session("DATA") = ds
        da.Dispose()
        con.Dispose()

    End Sub
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

                pnlFields.Controls.Add(New LiteralControl("<td style=""width:33%;text-align:left"">"))
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
    Private Sub FillPanelOnRoles(ByRef ds As DataSet)

        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)

        ' If ddlSeq.SelectedItem.Text = "NEW" Then
        'clear controls selection and changed button to new
        btnSave.Text = "Save New"
        chkCreateRight.Checked = False
        chkEditRight.Checked = False
        chkViewRight.Checked = False
        updMsg.Update()
        'Else
        'Fill panel and update Text
        btnSave.Text = "Update"
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill User Record  
        Dim strQry As String = ""
        strQry = "Select top 1 * from mmm_ref_role_user  where Eid=" & Session("Eid") & " and rolename='" & ddluserrole.SelectedValue & "' and UId=" & ddluser.SelectedValue & ""
        'strQry = "Select * from mmm_ref_role_user  where tid=" & ddlSeq.SelectedItem.Value
        Dim da As New SqlDataAdapter(strQry, con)
        Dim dt As New DataTable
        da.Fill(dt)
        Dim isCreate As Integer = 1
        Dim isEdit As Integer = 1
        Dim isView As Integer = 1

        If dt.Rows.Count > 0 Then
            isCreate = dt.Rows(0).Item("iscreate")
            isEdit = dt.Rows(0).Item("isedit")
            isView = dt.Rows(0).Item("isview")
        End If
        
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
            lblMsgupdate.Text = "Role Defination is not created! Please contact Admin!"
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

        'For i As Integer = 0 To ds1.Tables("doctype").Rows.Count - 1
        '    Dim ARR() As String = dt.Rows(0).Item("documenttype").ToString().Split(",")
        '    For ii As Integer = 0 To ARR.Length - 1
        '        If ARR(ii).ToString <> "" Or IsDBNull(ARR(ii).ToString) = False Then
        '            If ARR(ii).ToString = ds1.Tables("doctype").Rows(i).Item("formname").ToString Then
        '                chkDocumentType.Items(i).Selected = True
        '            End If
        '        End If
        '    Next
        'Next

        da.Dispose()
        con.Dispose()
        updControls.Update()
        'End If
    End Sub
    'Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
    '    Dim ds As New DataSet()
    '    ds = CType(Session("DATA"), DataSet)
    '    If ds.Tables("Data").Rows.Count > 0 Then
    '        FillPanelOnRoles(ds)
    '    End If
    'End Sub
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
                lblMsgupdate.Text = "There is no any rights on any Master/Document for selected Role in Menu Master!"
                updMsg.Update()
                Exit Sub
            Else
                DocType = Left(DocType, Len(DocType) - 1)
            End If

            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select * from mmm_ref_role_user  where tid=" & ddlSeq.SelectedItem.Value, con)
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

            'we have to update
            Dim ob As New DynamicForm
            ob.HistoryForRole(Session("EID"), ddluser.SelectedItem.Value, Session("UID"), DocType, "MMM_Ref_Role_User", "", "RoleAssignment", "New Role Assignment Page", "", ddluserrole.SelectedValue)

            Dim qry As String = "UPDATE mmm_ref_role_user  SET documenttype='" & DocType & "',iscreate=" & isCreate & ",isedit=" & isEdit & ",isview=" & isView & ","
            Dim qryValue As String = " Where Eid=" & Session("Eid") & " and rolename='" & ddluserrole.SelectedValue & "' and UID=" & ddluser.SelectedValue & ""
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

            'Dim objDMSUtil As New DMSUtil()

            'objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Role Assignment", "Role Assignment Updation", ddluser.SelectedItem.Value)
            lblMsgupdate.Text = "Record Updated"
            'End If
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
        'Comcheck()
        ddlseq1()
        doctype12()
        doctype13()
        DocType.Visible = True
        PnlRights.Visible = True
        'Uncheck()

    End Sub

    Private Sub Bindrol()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select rolename from mmm_ref_role_user  where EID=" & Session("EID").ToString() & " and UID='" & ddluser.SelectedValue & "'")
        If objDT.Rows.Count > 0 Then
            ddluserrole.DataSource = objDT
            ddluserrole.DataTextField = "rolename"
            ddluserrole.DataValueField = "rolename"
            ddluserrole.DataBind()
            ddluserrole.Items.Insert(0, "Select")
        Else
            ddluserrole.Items.Clear()
            ddluserrole.Items.Insert(0, "Select")
        End If



        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)
        ''Dim da As New SqlDataAdapter
        'Dim ds As New DataSet
        'Dim da As New SqlDataAdapter("select rolename from mmm_ref_role_user  where EID=" & Session("EID").ToString() & " and UID='" & ddluser.SelectedValue & "' ", con)
        'da.Fill(ds, "role")

        'ddluserrole.Items.Add("Select")
        'For i As Integer = 0 To ds.Tables("role").Rows.Count - 1
        '    ddluserrole.Items.Add(ds.Tables("role").Rows(i).Item("rolename").ToString())
        '    ddluserrole.Items(i + 1).Value = ds.Tables("role").Rows(i).Item("rolename").ToString()
        'Next
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddluser.SelectedIndexChanged

        DocType.Visible = False
        PnlRights.Visible = False
        Uncheck()
        Bindrol()
        BindDocType()


        btndelete.Visible = False
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
    Private Sub ddlseq1()
        Dim ds As New DataSet()
        Uncheck()
        ds = CType(Session("DATA"), DataSet)
        If ds.Tables("Data").Rows.Count > 0 Then
            FillPanelOnRoles(ds)
        End If
    End Sub
    Protected Sub ddlSeq_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSeq.SelectedIndexChanged
        'Dim ds As New DataSet()
        'Uncheck()
        'ds = CType(Session("DATA"), DataSet)
        'If ds.Tables("Data").Rows.Count > 0 Then
        '    FillPanelOnRoles(ds)
        'End If
    End Sub
    Protected Function BindGridToexport() As DataTable

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select r.tid, u.username,r.rolename,r.Documenttype,(case when iscreate = '0' then 'INACTIVE' when iscreate='1' then 'ACTIVE' end)[ISCreate],(case when isview ='0' then 'INACTIVE' when isview='1' then 'ACTIVE' end)[ISView],convert(nvarchar(500),r.fld1)[fld1],convert(nvarchar(500),r.fld4)[fld4] from mmm_ref_role_user  r inner join mmm_mst_user u on u.uid=r.uid where u.eid=" & Session("EID") & " and r.eid=" & Session("EID") & " and r.fld1 is not null and r.fld4 is not null order by r.uid", con)
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
                Dim query As String = "select row_number() over (order by u.username)[S.no.], u.username,r.rolename,r.Documenttype,(case when iscreate = '0' then 'INACTIVE' when iscreate='1' then 'ACTIVE' end)[ISCreate],(case when isview ='0' then 'INACTIVE' when isview='1' then 'ACTIVE' end)[ISView],'" & abc & "'[State], '" & xyz & "' [Circle] from mmm_ref_role_user  r inner join mmm_mst_user u on u.uid=r.uid where u.eid=" & Session("EID") & " and r.eid=" & Session("EID") & " and r.fld1 is not null and r.fld4 is not null and tid = " & tid & " order by r.uid"
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
        Dim da As New SqlDataAdapter("select docmapping,FormCaption from mmm_mst_forms with(nolock) where isroledef=1 and eid=" & Session("EID") & "", con)
        Dim ds As New DataSet
        Dim ds1 As New DataSet

        Dim qry As String = ""
        Dim StrColumn As String = ""
        da.Fill(ds, "data")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Dim fielmapping As String = ds.Tables("data").Rows(i).Item("docmapping").ToString
                Dim FormCaption As String = ds.Tables("data").Rows(i).Item("FormCaption").ToString
                StrColumn = StrColumn & "," & "isnull(STUFF((SELECT distinct ',' +CONVERT(varchar,fld1) from mmm_mst_Master where eid =" & Session("Eid") & "" _
                 & " and convert(varchar,tid) in (select * from InputString(u." & fielmapping & ")) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)') ,1,1,''),'') '" & FormCaption & "'"
            Next
            qry = "select distinct uid,(select UserName from mmm_mst_user MU where MU.UID=u.uid and eid=" & Session("Eid") & ") As 'UserName',(select UserID from mmm_mst_user TU where TU.UID=u.uid and eid=" & Session("Eid") & ") As 'UserId',rolename " & StrColumn & " from mmm_ref_role_user  u with(nolock) where eid=" & Session("Eid") & ""

            'qry = "select distinct uid,(select UserName from mmm_mst_user MU where MU.UID=u.uid and eid=" & Session("Eid") & ") As 'UserName',rolename,Documenttype  " & StrColumn & " from mmm_ref_role_user  u with(nolock) where eid=" & Session("Eid") & ""
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandTimeout = 300
            da.Fill(ds1)
            Dim GridView1 As New GridView()
            GridView1.DataSource = ds1
            GridView1.DataBind()
            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", "attachment;filename=RoleAssignment.xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            For T As Integer = 0 To GridView1.Rows.Count - 1
                'Apply text style to each Row 
                GridView1.Rows(T).Attributes.Add("class", "textmode")
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

        End If
    End Sub
    Protected Sub btndelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btndelete.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If ddlSeq.SelectedIndex <> 0 Then
            Dim da As New SqlDataAdapter("delete  from mmm_ref_role_user  where eid= " & Session("EID") & " and rolename= '" & ddluserrole.SelectedItem.Text & "' and uid= '" & ddluser.SelectedValue & "' and tid =" & ddlSeq.SelectedValue & "", con)
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
    'Protected Sub btnUpload_Click(sender As Object, e As ImageClickEventArgs)
    '    Dim DocType As String = ""
    '    'If CSVUploader.HasFile Then
    '    'If Right(CSVUploader.FileName, 4).ToUpper() = ".CSV" Then
    '    Dim filename As String = "RoleFile" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
    '    'CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
    '            Dim dtData As New DataTable
    '            'dtData = GetDataFromExcel(filename)
    '            ReadCSV(filename)
    '    ' End If
    '    'End If
    'End Sub
    Public Function GetDataFromExcel(ByVal strDataFilePath As String) As DataTable
        ' GetDataFromExcel123(strDataFilePath)
        Try
            Dim Seperator As String = ","
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'If Not DocumentType Is Nothing Then
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("select FieldSeperator from mmm_mst_forms where eid=" & Session("EID") & "", con)
                    Dim dtForm As New DataSet()
                    da.Fill(dtForm)
                    Select Case dtForm.Tables(0).Rows(0).Item(0).ToString().ToUpper().Trim()
                        'Case "COMMA"
                        '    'Case ""
                        '    Seperator = ","
                        Case "PIPE"
                            Seperator = "|"
                    End Select
                End Using
            End Using
            ' End If
            Dim sr As New StreamReader(Server.MapPath("~/Import/" & strDataFilePath))
            Dim FileName = Server.MapPath("~/Import/" & strDataFilePath)
            Dim fullFileStr As String = sr.ReadToEnd()
            fullFileStr = fullFileStr.Replace("'=", "=").Replace("'-", "-").Replace("'+", "+")
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(Seperator)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            Dim i As Integer = 0
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                    row.ItemArray = finalLine.Split(Seperator)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Function ReadCSV(ByVal path As String) As System.Data.DataTable
        Try
            Dim sr As New StreamReader(path)
            Dim fullFileStr As String = sr.ReadToEnd()
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(","c)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn())
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            For Each line As String In lines
                row = recs.NewRow()
                finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                row.ItemArray = finalLine.Split(","c)
                recs.Rows.Add(row)
            Next
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Protected Sub popUpClose()
        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)
        updPnlGrid.Update()
    End Sub
    Protected Sub btnimmm_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimmm.Click
        modalpopupimport.Show()
    End Sub
    Protected Sub helpexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles helpexport.Click
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("select docmapping,FormCaption from mmm_mst_forms with(nolock) where isroledef=1 and eid=" & Session("EID") & "", con)
            Dim ds As New DataSet
            Dim ds1 As New DataSet

            Dim qry As String = ""
            Dim StrColumn As String = ""
            da.Fill(ds, "data")
            If ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim fielmapping As String = ds.Tables("data").Rows(i).Item("docmapping").ToString
                    Dim FormCaption As String = ds.Tables("data").Rows(i).Item("FormCaption").ToString
                    StrColumn = StrColumn & "," & "isnull(STUFF((SELECT  distinct ',' +CONVERT(varchar,fld1) from mmm_mst_Master where eid =" & Session("Eid") & "" _
                     & " and convert(varchar,tid) in (select * from InputString(u." & fielmapping & ")) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)') ,1,1,''),'') '" & FormCaption & "'"
                Next
                qry = "select distinct top 1 EID,uid,(select UserName from mmm_mst_user MU where MU.UID=u.uid and eid=" & Session("Eid") & ") As 'UserName',(select UserID from mmm_mst_user TU where TU.UID=u.uid and eid=" & Session("Eid") & ") As 'UserId',rolename  " & StrColumn & " from mmm_ref_role_user  u with(nolock) where eid=" & Session("Eid") & ""
                da.SelectCommand.CommandText = qry
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandTimeout = 300
                da.Fill(ds1)
                Dim GridView1 As New GridView()
                GridView1.DataSource = ds1
                GridView1.DataBind()
                Response.Clear()
                Response.Buffer = True
                Response.AddHeader("content-disposition", "attachment;filename=RoleAssignment.xls")
                Response.Charset = ""
                Response.ContentType = "application/vnd.ms-excel"
                Dim sw As New StringWriter()
                Dim hw As New HtmlTextWriter(sw)
                For T As Integer = 0 To GridView1.Rows.Count - 1
                    'Apply text style to each Row 
                    GridView1.Rows(T).Attributes.Add("class", "textmode")
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
            End If
        Catch ex As Exception
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Text = "An error occured when Downloading data. Please try again"
        End Try
    End Sub
    Protected Sub btnimport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnimport.Click
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim icnt As Integer
            Dim ds As New DataSet
            lblMsg.Text = ""
            Dim da As New SqlDataAdapter("select docmapping,FormCaption from mmm_mst_forms with(nolock) where isroledef=1 and eid=" & Session("EID") & "", con)
            da.Fill(ds, "data")

            Dim DocType As String = ""



            Dim c As Integer = ds.Tables("data").Rows.Count
            Dim adapter As New SqlDataAdapter
            Dim sb As New System.Text.StringBuilder()
            Dim sh As New System.Text.StringBuilder()
            Dim errs As String = ""
            If impfile.HasFile Then
                ViewState("imprt_cnt") += 1
                If (Right(impfile.FileName, 4).ToUpper()) = ".CSV" Then
                    Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(impfile.FileName, 4).ToUpper()
                    impfile.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                    Dim ir As Integer = 0
                    Dim sField As String()
                    Dim csvReader As Microsoft.VisualBasic.FileIO.TextFieldParser
                    csvReader = My.Computer.FileSystem.OpenTextFieldParser(Server.MapPath("Import/") & filename, ",")
                    Dim st As String = ""
                    Dim ic As Integer = 0
                    Dim Updt As String = ""
                    Dim dfu As String = ""
                    Dim dfi As String = ""
                    Dim c1 As String = ""
                    Dim errmsg As String = ""
                    Dim v As String = ""
                    Dim ftype As String = ""
                    With csvReader
                        .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
                        .Delimiters = New String() {","}
                        While Not .EndOfData
                            sField = .ReadFields()
                            If icnt < 1 Then
                                sb.Append("Insert Into MMM_ref_role_user (eid,uid,Rolename,documenttype,")
                                If UCase(sField(0)) <> "EID" Then
                                    errmsg = errmsg & "EID " & "field is not matched"
                                ElseIf UCase(sField(1)) <> "UID" Then
                                    errmsg = errmsg & "UID " & "field is not matched"
                                ElseIf UCase(sField(2)) <> "USERNAME" Then
                                    errmsg = errmsg & "USERNAME " & "field is not matched"
                                ElseIf UCase(sField(3)) <> "USERID" Then
                                    errmsg = errmsg & "USERID " & "field is not matched"
                                    'ElseIf UCase(sField(4)) <> "DOCUMENTTYPE" Then
                                    '    errmsg = errmsg & "DOCUMENTTYPE " & "field is not matched"
                                End If
                                If errmsg.Length > 1 Then
                                    lblMsg.Text = errmsg
                                    Exit Sub
                                End If
                                For k As Integer = 0 To c - 1
                                    If UCase(sField(5 + k)) <> UCase(Trim(ds.Tables("data").Rows(k).Item("FormCaption").ToString())) Then
                                        errmsg = errmsg & Trim(ds.Tables("data").Rows(k).Item("FormCaption").ToString()) & "field is not matched "
                                        'Exit sub
                                    Else
                                        st = ds.Tables("data").Rows(k).Item("docmapping").ToString()
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
                            If errmsg.Length > 1 Then
                                lblMsg.Text = errmsg
                                Exit Sub
                            End If
                            If icnt > 0 Then
                                da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user  where eid=" & sField(0) & " and uid=" & sField(1) & " and rolename='" & sField(4) & "'"
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                Dim objDT As New DataTable
                                objDT = objDC.ExecuteQryDT("select substring (PageLink, 19, 200) as 'PageLink' from mmm_mst_menu where eid=" & Session("EID") & " and Roles like +'%{'+'" & sField(4) & "'+':%' and pageLink like 'Documents.aspx%'" _
                                    & " Union" _
                                   & " select substring (PageLink, 17, 200) as 'PageLink' from mmm_mst_menu where eid=" & Session("EID") & " and Roles like +'%{'+'" & sField(4) & "'+':%' and  pageLink like 'Masters.aspx%'")
                                If objDT.Rows.Count > 0 Then
                                    chkboxforup.DataSource = objDT
                                    chkboxforup.DataTextField = "PageLink"
                                    chkboxforup.DataValueField = "PageLink"
                                    chkboxforup.DataBind()
                                Else
                                    chkboxforup.Items.Clear()
                                End If
                                'Comcheck()
                                For j As Integer = 0 To chkboxforup.Items.Count - 1
                                    chkboxforup.Items(j).Selected = True
                                Next
                                For Each li As ListItem In chkboxforup.Items
                                    If li.Selected Then
                                        DocType &= li.Value & ","
                                    End If
                                Next
                                Dim cnttt As Integer = da.SelectCommand.ExecuteScalar()
                                For d As Integer = 0 To c - 1
                                    Dim dt As New DataTable
                                    c1 = Replace(sField(5 + d), ",", "','")
                                    da.SelectCommand.CommandText = "select isnull(STUFF((SELECT distinct ',' +CONVERT(varchar, tid) from mmm_mst_Master with(nolock)" &
                                                                    "where eid=" & Session("EID") & " and convert(varchar,tid) in (select tid from mmm_mst_master where fld1 in ('" & c1 & "') and eid=" & Session("EID") & ")" &
                                                                     "FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)') ,1,1,''),'')"
                                    da.Fill(dt)
                                    dfu = dfu & ds.Tables("data").Rows(d).Item("docmapping").ToString() & "=" & "'" & dt.Rows(0).Item(0).ToString & "',"
                                    dfi = dfi & "'" & dt.Rows(0).Item(0).ToString & "',"
                                Next
                                dfu = dfu.Substring(0, dfu.Length - 1)
                                dfi = dfi.Substring(0, dfi.Length - 1)
                                DocType = DocType.Substring(0, DocType.Length - 1)
                                If cnttt > 0 Then
                                    Updt = " update mmm_ref_role_user  set DocumentType='" & DocType & "'," & dfu & "  where eid=" & sField(0) & " and uid=" & sField(1) & " and Rolename='" & sField(4) & "'"
                                Else
                                    v = v & sField(0) & "," & sField(1) & ",'" & sField(4) & "','" & DocType & "'," & dfi
                                End If
                            End If
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            If Updt.Length > 0 Then
                                adapter.InsertCommand = New SqlCommand(Updt, con)
                                adapter.InsertCommand.CommandTimeout = 600
                            Else
                                Replace(sb.ToString(), "{", "")
                                Replace(sb.ToString(), "}", "")
                                sh.Append(sb)
                                sh.Append(v)
                                sh.Append(")")
                                adapter.InsertCommand = New SqlCommand(sh.ToString(), con)
                                adapter.InsertCommand.CommandTimeout = 600
                            End If
                            adapter.InsertCommand.ExecuteNonQuery()
                            sb.Clear()
                            v = ""
                            dfu = ""
                            dfi = ""
                            Updt = ""
                            errmsg = ""
                            adapter.Dispose()
                            sh.Clear()
                        End While
                        con.Close()
                        lblMsg.Text = "File processed Successfully."
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
End Class
