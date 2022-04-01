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

Partial Class MenuControl
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            bindgrid()
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
    Public Sub bindgrid()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'fill Product  
        Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
        Dim ds1 As New DataSet
        da1.Fill(ds1, "role")

        Dim sb As String = ""
        For Each rw As DataRow In ds1.Tables("role").Rows
            sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
        Next
        If sb <> "" Then
            sb = "," & sb.Substring(("0"), sb.Length - 1)
        End If
        Dim daa As New SqlDataAdapter("select isgpsactivated from mmm_mst_entity where  eid=" & Session("eid") & "", con)
        Dim isg As Integer = daa.SelectCommand.ExecuteScalar
        If isg = 0 Then
            Dim da As New SqlDataAdapter("SELECT Menuname,menutype " & sb & " FROM MMM_MST_Accessmenu where setting <> 'ISGPS'", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
        Else
            Dim da As New SqlDataAdapter("SELECT Menuname,menutype " & sb & " FROM MMM_MST_Accessmenu", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
        End If
        'Dim da As New SqlDataAdapter("SELECT Menuname,menutype " & sb & " FROM MMM_MST_Accessmenu", con)
        'Dim ds As New DataSet
        gvData.DataBind()

        con.Dispose()
        'da.Dispose()
    End Sub


    '' prev proc b4 gps menu added 
    'Public Sub bindgrid()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    'fill Product  
    '    Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
    '    Dim ds1 As New DataSet
    '    da1.Fill(ds1, "role")

    '    Dim sb As String = ""
    '    For Each rw As DataRow In ds1.Tables("role").Rows
    '        sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
    '    Next
    '    If sb <> "" Then
    '        sb = "," & sb.Substring(("0"), sb.Length - 1)
    '    End If
    '    Dim da As New SqlDataAdapter("SELECT Menuname,menutype " & sb & " FROM MMM_MST_Accessmenu", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "data")
    '    gvData.DataSource = ds.Tables("data")

    '    gvData.DataBind()

    '    con.Dispose()
    '    da.Dispose()
    'End Sub

    'Protected Sub gvData_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvData.RowCommand

    '    If e.CommandName = "Master" Then
    '        ViewState("menu") = "Master"
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
    '        Dim ds1 As New DataSet
    '        da1.Fill(ds1, "role")
    '        Dim sb As String = ""

    '        For Each rw As DataRow In ds1.Tables("role").Rows
    '            sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
    '        Next
    '        sb = sb.Substring(0, sb.Length - 1)
    '        ViewState("sb") = sb
    '        Dim da As New SqlDataAdapter("select f.FormName, " & sb & "  from mmm_mst_forms f inner join mmm_mst_accessmenu a on f.FormType=a.Pmenu where f.EID=" & Session("eid") & " and a.Menutype='dynamic' and f.formtype='Master'", con)
    '        Dim ds As New DataSet
    '        da.Fill(ds, "formname")
    '        GridView1.DataSource = ds.Tables("formname")
    '        GridView1.DataBind()


    '    ElseIf e.CommandName = "Document" Then
    '        ViewState("menu") = "Document"
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
    '        Dim ds1 As New DataSet
    '        da1.Fill(ds1, "role")

    '        Dim sb As String = ""
    '        For Each rw As DataRow In ds1.Tables("role").Rows
    '            sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
    '        Next
    '        sb = sb.Substring(0, sb.Length - 1)
    '        ViewState("sb") = sb
    '        Dim da As New SqlDataAdapter("select f.FormName, " & sb & "  from mmm_mst_forms f inner join mmm_mst_accessmenu a on f.FormType=a.Pmenu where f.EID=" & Session("eid") & " and a.Menutype='dynamic' and f.formtype='Document' and f.formsource='MENU DRIVEN'", con)
    '        Dim ds As New DataSet
    '        da.Fill(ds, "formname")
    '        GridView1.DataSource = ds.Tables("formname")
    '        GridView1.DataBind()
    '    ElseIf e.CommandName = "Reports" Then
    '        ViewState("menu") = "Reports"
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(conStr)
    '        'fill Product  
    '        Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
    '        Dim ds1 As New DataSet
    '        da1.Fill(ds1, "role")

    '        Dim sb As String = ""
    '        For Each rw As DataRow In ds1.Tables("role").Rows
    '            sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
    '        Next
    '        sb = sb.Substring(0, sb.Length - 1)
    '        ViewState("sb") = sb
    '        Dim da As New SqlDataAdapter("select reportname, " & sb & "  from mmm_mst_report,mmm_mst_accessmenu where EID=" & Session("eid") & " and Menutype='dynamic' and menuname='Reports'", con)
    '        Dim ds As New DataSet
    '        da.Fill(ds, "formname")
    '        GridView1.DataSource = ds.Tables("formname")
    '        GridView1.DataBind()
    '    End If
    'End Sub

    Protected Sub gvData_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowCreated
        Dim row As GridViewRow = e.Row

        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 2 To row.Cells.Count - 1

                Dim cbv As New CheckBox
                cbv.ID = "chkv" & i.ToString() & row.RowIndex
                cbv.Checked = True
                row.Cells(i).Controls.Add(cbv)

                Dim cbc As New CheckBox
                cbc.ID = "chkc" & i.ToString() & row.RowIndex
                cbc.Checked = True
                row.Cells(i).Controls.Add(cbc)

                Dim cbe As New CheckBox
                cbe.ID = "chke" & i.ToString() & row.RowIndex
                cbe.Checked = True
                row.Cells(i).Controls.Add(cbe)

                Dim cbl As New CheckBox
                cbl.ID = "chkl" & i.ToString() & row.RowIndex
                cbl.Checked = True
                row.Cells(i).Controls.Add(cbl)

                If e.Row.Cells(2).Text = "Dynamic" Then
                    Dim btn As New Button
                    btn = TryCast(row.FindControl("btndm"), Button)
                    btn.Visible = True
                End If
            Next
        End If
    End Sub
    Protected Sub gvData_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowDataBound
        Dim row As GridViewRow = e.Row
        e.Row.Cells(2).Visible = False
        e.Row.HorizontalAlign = HorizontalAlign.Left
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If e.Row.RowType = DataControlRowType.DataRow Then

            'Mind that i used i=1 not 0 because the width of cells(0) has already been set

            For i As Integer = 3 To row.Cells.Count - 1

                Dim MenuName As String = gvData.HeaderRow.Cells(i).Text
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If

                If e.Row.Cells(2).Text = "static" Then
                    'Dim cb As New CheckBox
                    'cb.ID = "chk" & i.ToString() & row.RowIndex
                    Dim cbv As New CheckBox
                    cbv.ID = "chkv" & i.ToString() & row.RowIndex
                    cbv.ToolTip = "IsView"

                    Dim cbc As New CheckBox
                    cbc.ID = "chkc" & i.ToString() & row.RowIndex
                    cbc.ToolTip = "IsCreate"

                    Dim cbe As New CheckBox
                    cbe.ID = "chke" & i.ToString() & row.RowIndex
                    cbe.ToolTip = "IsEdit"

                    Dim cbl As New CheckBox
                    cbl.ID = "chkl" & i.ToString() & row.RowIndex
                    cbl.ToolTip = "IsLock"

                    Dim da As New SqlDataAdapter("SELECT " & MenuName & " FROM MMM_MST_Accessmenu where menuname = '" & e.Row.Cells(1).Text & "' AND MENUTYPE='STATIC'   ", con)
                    Dim str As String = da.SelectCommand.ExecuteScalar.ToString()
                    If str.Length > 0 Then
                        If str = "1" Then
                            cbv.Checked = True
                        ElseIf str = "2" Then
                            cbc.Checked = True
                        ElseIf str = "3" Then
                            cbv.Checked = True
                            cbc.Checked = True
                        ElseIf str = "4" Then
                            cbe.Checked = True
                        ElseIf str = "5" Then
                            cbv.Checked = True
                            cbe.Checked = True
                        ElseIf str = "6" Then
                            cbc.Checked = True
                            cbe.Checked = True
                        ElseIf str = "7" Then
                            cbv.Checked = True
                            cbc.Checked = True
                            cbe.Checked = True
                        ElseIf str = "8" Then
                            cbl.Checked = True
                        ElseIf str = "9" Then
                            cbv.Checked = True
                            cbl.Checked = True
                        ElseIf str = "10" Then
                            cbc.Checked = True
                            cbl.Checked = True
                        ElseIf str = "12" Then
                            cbe.Checked = True
                            cbl.Checked = True
                        ElseIf str = "15" Then
                            cbv.Checked = True
                            cbc.Checked = True
                            cbe.Checked = True
                            cbl.Checked = True
                        End If
                    End If

                    con.Close()
                    da.Dispose()
                    row.Cells(i).Controls.Add(cbv)
                    row.Cells(i).Controls.Add(cbc)
                    row.Cells(i).Controls.Add(cbe)
                    row.Cells(i).Controls.Add(cbl)
                    Dim btn As New Button
                    btn = TryCast(row.FindControl("btndm"), Button)
                    btn.Visible = False
                ElseIf e.Row.Cells(2).Text = "dynamic" Then
                    Dim da As New SqlDataAdapter("SELECT " & MenuName & " FROM MMM_MST_Accessmenu where menuname = '" & e.Row.Cells(1).Text & "' AND MENUTYPE='DYNAMIC'   ", con)
                    Dim str As String = da.SelectCommand.ExecuteScalar.ToString()
                    Dim btn As New Button
                    btn = TryCast(row.FindControl("btndm"), Button)
                    btn.Visible = True
                    row.Cells(i).Text = str
                End If
            Next
            For i As Integer = 0 To e.Row.Cells.Count - 1
                ViewState("OrigData") = e.Row.Cells(i).Text
                If e.Row.Cells(i).Text.Length >= 30 Then
                    e.Row.Cells(i).Text = e.Row.Cells(i).Text.Substring(0, 30) + "..."
                    e.Row.Cells(i).ToolTip = ViewState("OrigData").ToString()
                    e.Row.Cells(i).Wrap = True
                End If
            Next
        End If
        con.Dispose()
    End Sub
    Public Sub showpopup()
        btnEdit_ModalPopupExtender.Show()
    End Sub



    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim row As GridViewRow
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim da As New SqlDataAdapter("", con)
        Dim cmd As New SqlCommand("", con)
        For Each row In gvData.Rows
            If row.RowType = DataControlRowType.DataRow Then

                If row.Cells(2).Text = "static" Then
                    For i As Integer = 3 To row.Cells.Count - 1
                        Dim int As Integer = 0
                        Dim MenuName As String = gvData.HeaderRow.Cells(i).Text
                        
                        Dim cbv As New CheckBox
                        cbv.ID = "chkv" & i.ToString() & row.RowIndex
                        Dim cbc As New CheckBox
                        cbc.ID = "chkc" & i.ToString() & row.RowIndex
                        Dim cbe As New CheckBox
                        cbe.ID = "chke" & i.ToString() & row.RowIndex
                        Dim cbl As New CheckBox
                        cbl.ID = "chkl" & i.ToString() & row.RowIndex

                        Dim txtnamev As String = "chkv" & i.ToString() & row.RowIndex
                        Dim txtnamec As String = "chkc" & i.ToString() & row.RowIndex
                        Dim txtnamee As String = "chke" & i.ToString() & row.RowIndex
                        Dim txtnamel As String = "chkl" & i.ToString() & row.RowIndex
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        If CType(row.FindControl(txtnamev), CheckBox).Checked = True Then
                            int += 1
                        End If
                        If CType(row.FindControl(txtnamec), CheckBox).Checked = True Then
                            int += 2
                        End If
                        If CType(row.FindControl(txtnamee), CheckBox).Checked = True Then
                            int += 4
                        End If
                        If CType(row.FindControl(txtnamel), CheckBox).Checked = True Then
                            int += 8
                        End If

                        cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = " & int & "  where menuname = '" & row.Cells(1).Text & "'  and menutype= 'static'"
                        cmd.ExecuteNonQuery()

                    Next
                ElseIf row.Cells(2).Text = "dynamic" Then

                    For i As Integer = 3 To row.Cells.Count - 1
                        Dim az As String = ""
                        az = row.Cells(i).Text
                        If az = "&nbsp;" Then
                            az = az.Replace("&nbsp;", " ")
                        End If
                        Dim MenuName As String = gvData.HeaderRow.Cells(i).Text
                        Dim daa As New SqlDataAdapter("select " & MenuName & " from mmm_mst_accessmenu where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'", con)
                        Dim ae As String = daa.SelectCommand.ExecuteScalar()
                        If ae = "&nbsp;" Then
                            ae = az.Replace("&nbsp;", " ")
                        End If
                        'Dim cb As New CheckBox
                        'cb = TryCast(row.FindControl("chk" & i.ToString() & row.RowIndex), CheckBox)
                        'cb.ID = "chk" & i.ToString() & row.RowIndex
                        '            ln.Width = 50

                        'fill Product  
                        Dim cbv As New CheckBox
                        cbv.ID = "chkv" & i.ToString() & row.RowIndex
                        Dim cbc As New CheckBox
                        cbc.ID = "chkc" & i.ToString() & row.RowIndex
                        Dim cbe As New CheckBox
                        cbe.ID = "chke" & i.ToString() & row.RowIndex
                        Dim cbl As New CheckBox
                        cbl.ID = "chkl" & i.ToString() & row.RowIndex

                        Dim txtnamev As String = "chkv" & i.ToString() & row.RowIndex
                        Dim txtnamec As String = "chkc" & i.ToString() & row.RowIndex
                        Dim txtnamee As String = "chke" & i.ToString() & row.RowIndex
                        Dim txtnamel As String = "chkl" & i.ToString() & row.RowIndex
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If

                        '  Dim txtname As String = "chk" & i.ToString() & row.RowIndex
                        If CType(row.FindControl(txtnamev), CheckBox).Checked = True Then
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        Else
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        End If
                        If CType(row.FindControl(txtnamec), CheckBox).Checked = True Then
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        Else
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        End If
                        If CType(row.FindControl(txtnamee), CheckBox).Checked = True Then
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        Else
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        End If
                        If CType(row.FindControl(txtnamel), CheckBox).Checked = True Then
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        Else
                            cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & ae & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        End If
                        'If CType(row.FindControl(txtname), CheckBox).Checked = False Then
                        '    cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & az & "'  where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        'Else
                        '    cmd.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = '" & az & "'   where menuname = '" & row.Cells(1).Text & "' and menutype= 'dynamic'"
                        'End If
                        'da.SelectCommand.CommandText = "SELECT " & MenuName & " FROM MMM_MST_Accessmenu where menuname = '" & row.Cells(1).Text & "'   "
                        cmd.ExecuteNonQuery()
                    Next
                End If
            End If

        Next
        con.Close()
        da.Dispose()
        bindgrid()
        updatePanelEdit.Update()
    End Sub

    Protected Sub GridView1_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowCreated
        Dim row As GridViewRow = e.Row

        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To row.Cells.Count - 1
                Dim cbv As New CheckBox
                cbv.ID = "chkv" & i.ToString() & row.RowIndex
                ' cbv.Text = "V"
                cbv.Checked = True
                row.Cells(i).Controls.Add(cbv)


                Dim cbc As New CheckBox
                cbc.ID = "chkc" & i.ToString() & row.RowIndex
                'cbc.Text = "C"
                cbc.Checked = True
                row.Cells(i).Controls.Add(cbc)

                Dim cbe As New CheckBox
                cbe.ID = "chke" & i.ToString() & row.RowIndex
                'cbe.Text = "E"
                cbe.Checked = True
                row.Cells(i).Controls.Add(cbe)

                Dim cbl As New CheckBox
                cbl.ID = "chkl" & i.ToString() & row.RowIndex
                ' cbl.Text = "L"
                cbl.Checked = True
                row.Cells(i).Controls.Add(cbl)

            Next
        End If

    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim row As GridViewRow = e.Row

        e.Row.HorizontalAlign = HorizontalAlign.Center
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To row.Cells.Count - 1
                Dim p As String = GridView1.HeaderRow.Cells(i).Text
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As New SqlConnection(conStr)
                'If e.Row.Cells(1).Text = "Dynamic" Then

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim cbv As New CheckBox
                cbv.ID = "chkv" & i.ToString() & row.RowIndex
                cbv.ToolTip = "IsView"
                '  cbv.Text = "V"
                Dim cbc As New CheckBox
                cbc.ID = "chkc" & i.ToString() & row.RowIndex
                cbc.ToolTip = "IsCreate"
                ' cbc.Text = "C"
                Dim cbe As New CheckBox
                cbe.ID = "chke" & i.ToString() & row.RowIndex
                cbe.ToolTip = "IsEdit"
                'cbe.Text = "E"
                Dim cbl As New CheckBox
                cbl.ID = "chkl" & i.ToString() & row.RowIndex
                cbl.ToolTip = "IsLock"
                'cbl.Text = "L"
                ''Dim MenuName As String = gvData.HeaderRow.Cells(i).Text
                ''Dim da As New SqlDataAdapter("SELECT " & MenuName & " FROM MMM_MST_Accessmenu where menuname = '" & ViewState("menu") & "'   ", con)
                ''Dim str As String = da.SelectCommand.ExecuteScalar


                ''da.Dispose()
                Dim daa As New SqlDataAdapter("select " & p & " from mmm_mst_accessmenu where menuname = '" & ViewState("menu") & "' and menutype='dynamic'", con)
                Dim asd As String = daa.SelectCommand.ExecuteScalar.ToString()


                Dim words As String() = asd.Split(New Char() {","c})
                Dim word As String
                For Each word In words
                    Dim ab As String() = word.Split(New Char() {":"c})
                    If ab.Length > 1 Then
                        If ab(0) = e.Row.Cells(0).Text Then
                            If ab(1) = 1 Then
                                cbv.Checked = True
                            ElseIf ab(1) = 2 Then
                                cbc.Checked = True
                            ElseIf ab(1) = 3 Then
                                cbv.Checked = True
                                cbc.Checked = True
                            ElseIf ab(1) = 4 Then
                                cbe.Checked = True
                            ElseIf ab(1) = 5 Then
                                cbv.Checked = True
                                cbe.Checked = True
                            ElseIf ab(1) = 6 Then
                                cbc.Checked = True
                                cbe.Checked = True
                            ElseIf ab(1) = 7 Then
                                cbv.Checked = True
                                cbc.Checked = True
                                cbe.Checked = True
                            ElseIf ab(1) = 8 Then
                                cbl.Checked = True
                            ElseIf ab(1) = 9 Then
                                cbv.Checked = True
                                cbl.Checked = True
                            ElseIf ab(1) = 10 Then
                                cbc.Checked = True
                                cbl.Checked = True
                            ElseIf ab(1) = 12 Then
                                cbe.Checked = True
                                cbl.Checked = True
                            ElseIf ab(1) = 15 Then
                                cbv.Checked = True
                                cbc.Checked = True
                                cbe.Checked = True
                                cbl.Checked = True
                            End If
                        End If
                    End If
                Next

                row.Cells(i).Controls.Add(cbv)
                row.Cells(i).Controls.Add(cbc)
                row.Cells(i).Controls.Add(cbe)
                row.Cells(i).Controls.Add(cbl)
                con.Close()
                daa.Dispose()
            Next
        End If


    End Sub

    Protected Sub btnActEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim da As New SqlDataAdapter("", con)

        For i As Integer = 1 To GridView1.HeaderRow.Cells.Count - 1
            Dim st As String = ""
            Dim ab As String = ""
            ab = GridView1.HeaderRow.Cells(i).Text
            For Each row In GridView1.Rows
                Dim int As Integer = 0
                Dim acs As String = row.cells(0).text
                If row.RowType = DataControlRowType.DataRow Then
                    Dim cbv As New CheckBox
                    cbv.ID = "chkv" & i.ToString() & row.RowIndex
                    Dim cbc As New CheckBox
                    cbc.ID = "chkc" & i.ToString() & row.RowIndex
                    Dim cbe As New CheckBox
                    cbe.ID = "chke" & i.ToString() & row.RowIndex
                    Dim cbl As New CheckBox
                    cbl.ID = "chkl" & i.ToString() & row.RowIndex

                    Dim txtnamev As String = "chkv" & i.ToString() & row.RowIndex
                    Dim txtnamec As String = "chkc" & i.ToString() & row.RowIndex
                    Dim txtnamee As String = "chke" & i.ToString() & row.RowIndex
                    Dim txtnamel As String = "chkl" & i.ToString() & row.RowIndex

                    ' st &= row.Cells(0).Text & ":"
                    If CType(row.FindControl(txtnamev), CheckBox).Checked = True Then
                        Int += 1
                        st &= row.Cells(0).Text & ":"
                    End If
                    If CType(row.FindControl(txtnamec), CheckBox).Checked = True Then
                        Int += 2
                        If st.Contains(row.Cells(0).Text & ":").ToString() Then
                        Else
                            st &= row.Cells(0).Text & ":"
                        End If
                    End If
                    If CType(row.FindControl(txtnamee), CheckBox).Checked = True Then
                        Int += 4
                        If st.Contains(row.Cells(0).Text & ":").ToString() Then
                            '    st &= int
                        Else
                            st &= row.Cells(0).Text & ":"
                        End If

                    End If
                    If CType(row.FindControl(txtnamel), CheckBox).Checked = True Then
                        Int += 8
                        If st.Contains(row.Cells(0).Text & ":").ToString() Then
                            '    st &= int
                        Else
                            st &= row.Cells(0).Text & ":"
                        End If

                    End If


                End If
                If st.Contains(row.Cells(0).Text & ":").ToString() Then
                    st &= int & ","
                End If
            Next
            If st.Count > 0 Then
                st = st.Substring(0, st.Length - 1)
            End If

            da.SelectCommand.CommandText = "update  MMM_MST_Accessmenu set  " & ab & " = '" & st & "'  where menuname = '" & ViewState("menu") & "' and menutype='dynamic'"
            da.SelectCommand.ExecuteScalar()

        Next
        con.Close()
        da.Dispose()
        bindgrid()
        updGrid.Update()
        btnEdit_ModalPopupExtender.Hide()




        'For i As Integer = 1 To row.Cells.Count - 1
        '    Dim MenuName As String = gvData.HeaderRow.Cells(0).Text
        '    Dim cb As New CheckBox
        '    cb.ID = "chk1" & i.ToString() & row.RowIndex
        '    '            ln.Width = 50

        '    'fill Product  
        '    If con.State <> ConnectionState.Open Then
        '        con.Open()
        '    End If
        '    Dim txtname As String = "chk" & i.ToString() & row.RowIndex

        '    'If CType(row.FindControl(txtname), CheckBox).Checked = False Then
        '    '    da.SelectCommand.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = 0  where menuname = '" & row.Cells(1).Text & "'"
        '    'Else
        '    '    Dim str As String = ""
        '    '    str &= MenuName
        '    '    da.SelectCommand.CommandText = "update  MMM_MST_Accessmenu set  " & MenuName & " = " & str & "  where menuname = '" & row.Cells(1).Text & "'"
        '    'End If
        '    'da.SelectCommand.CommandText = "SELECT " & MenuName & " FROM MMM_MST_Accessmenu where menuname = '" & row.Cells(1).Text & "'   "
        '    da.SelectCommand.ExecuteScalar()





    End Sub
    
    Protected Sub showpopup(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        Dim btnDetails As Button = TryCast(sender, Button)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        If row.Cells(1).Text = "Master" Then
            ViewState("menu") = "Master"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
            Dim ds1 As New DataSet
            da1.Fill(ds1, "role")
            Dim sb As String = ""

            For Each rw As DataRow In ds1.Tables("role").Rows
                sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
            Next
            sb = sb.Substring(0, sb.Length - 1)
            ViewState("sb") = sb


            Dim da As New SqlDataAdapter("select f.FormName + '-' + f.formcaption[MenuName], " & sb & "  from mmm_mst_forms f inner join mmm_mst_accessmenu a on f.FormType=a.Pmenu where f.EID=" & Session("eid") & " and a.Menutype='dynamic' and f.formtype='Master'", con)
            Dim ds As New DataSet
            da.Fill(ds, "formname")
            ViewState("Mas") = ds.Tables("formname")
            GridView1.DataSource = ds.Tables("formname")
            GridView1.DataBind()

        ElseIf row.Cells(1).Text = "Document" Then
            ViewState("menu") = "Document"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
            Dim ds1 As New DataSet
            da1.Fill(ds1, "role")

            Dim sb As String = ""
            For Each rw As DataRow In ds1.Tables("role").Rows
                sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
            Next
            sb = sb.Substring(0, sb.Length - 1)
            ViewState("sb") = sb
            Dim da As New SqlDataAdapter("select f.FormName + '-' + f.formcaption[MenuName], " & sb & "  from mmm_mst_forms f inner join mmm_mst_accessmenu a on f.FormType=a.Pmenu where f.EID=" & Session("eid") & " and a.Menutype='dynamic' and f.formtype='Document' and f.formsource='MENU DRIVEN'", con)
            Dim ds As New DataSet
            da.Fill(ds, "formname")
            ViewState("Doc") = ds.Tables("formname")
            GridView1.DataSource = ds.Tables("formname")

            GridView1.DataBind()

        ElseIf row.Cells(1).Text = "Reports" Then
            ViewState("menu") = "Reports"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da1 As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & "", con)
            Dim ds1 As New DataSet
            da1.Fill(ds1, "role")

            Dim sb As String = ""
            For Each rw As DataRow In ds1.Tables("role").Rows
                sb &= Session("CODE") & "_" & rw.Item("rolename").ToString() & ","
            Next
            sb = sb.Substring(0, sb.Length - 1)
            ViewState("sb") = sb
            Dim da As New SqlDataAdapter("select reportname + '-' + reportname[MenuName], " & sb & "  from mmm_mst_report,mmm_mst_accessmenu where EID=" & Session("eid") & " and Menutype='dynamic' and menuname='Reports'", con)
            Dim ds As New DataSet
            da.Fill(ds, "formname")
            ViewState("Rep") = ds.Tables("formname")
            GridView1.DataSource = ds.Tables("formname")

            GridView1.DataBind()


        End If

        showpopup()
        updatePanelEdit.Update()
        bindgrid()


    End Sub
End Class
