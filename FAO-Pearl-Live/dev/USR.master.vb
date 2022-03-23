Imports System.Data
Imports System.Data.SqlClient

Partial Class USR_USR
    Inherits System.Web.UI.MasterPage


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not IsPostBack Then
            'For SALES ADMIN TEST

            'Session("UID") = 2
            'Session("USERERIMAGE") = "2.jpg"
            'Session("CLOGO") = "mynd.gif"
            'Session("USERNAME") = "Manish Kumar"
            'Session("USERROLE") = "ADMIN"

            ''for SU 
            'Session("UID") = 2012    '443 '393 for HFCL - 234 for 16,28 -1926-ecab, 1353 esfa
            'Session("USERNAME") = "Vinay Kumar"
            'Session("USERROLE") = "SU"
            'Session("CODE") = "agarwalpackers"
            'Session("USERIMAGE") = "2.jpg"
            'Session("CLOGO") = "hfcl.png"
            'Session("EID") = 42
            'Session("ISLOCAL") = "TRUE"
            'Session("IPADDRESS") = "Vinay"
            'Session("MACADDRESS") = "Vinay"
            'Session("INTIME") = Now
            'Session("EMAIL") = "vinay.kumar@myndsol.com"
            'Session("LID") = "25"
            'Session("HEADERSTRIP") = "hfclstrip.jpg"
            'Session("ROLES") = "SU"

            ' '' ''for Project Owner
            'Session("UID") = 28
            'Session("USERNAME") = "TechmSeh Approvar"
            'Session("USERROLE") = "SU"
            'Session("USERIMAGE") = "noimage.png"
            'Session("CLOGO") = "mynd.gif"
            'Session("EID") = 4
            'Session("ISLOCAL") = "TRUE"
            'Session("IPADDRESS") = "Manish"
            'Session("MACADDRESS") = "Manish"
            'Session("INTIME") = Now
            'Session("LID") = "25"

            ''for Users
            'Session("UID") = 35
            'Session("USERNAME") = "Balmiki"
            'Session("USERROLE") = "USR"
            'Session("USERIMAGE") = "noimage.png"
            'Session("CLOGO") = "mynd.gif"
            'Session("EID") = 2
            'Session("ISLOCAL") = "TRUE"
            'Session("IPADDRESS") = "Manish"
            'Session("MACADDRESS") = "Manish"

            ''for USRU
            'Session("UID") = 13
            'Session("USERNAME") = "Manish Kumar"
            'Session("USERROLE") = "USR"
            'Session("USERIMAGE") = "2.jpg"
            'Session("CLOGO") = "mynd.gif"
            'Session("EID") = 2
            'Session("IPADDRESS") = "Manish"
            'Session("MACADDRESS") = "Manish"
        End If

        If Session("UID") Is Nothing Then
            Response.Redirect("SessionOut.aspx")
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblLogo.Text = "<img width=""Auto"" height=""50px"" src=""logo/" & Session("CLOGO") & """ alt=""" & Session("CODE") & """  />"
            lblUserMsg.Text = "<b>" & Session("USERNAME") & " </b> "

            Dim roles() As String = Session("Roles").ToString.Split(",")
            For i As Integer = 0 To roles.Length - 1
                ddlUserRole.Items.Add(roles(i).ToString())
            Next
            ddlUserRole.SelectedIndex = ddlUserRole.Items.IndexOf(ddlUserRole.Items.FindByText(Session("USERROLE").ToString))

            ''ddlUserRole.Items.Add(Session("USERROLE").ToString())

            'Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim dac As New SqlDataAdapter("", con)
            'Dim dt As New DataTable
            'Dim STR As String = "select userrole [userrole] from MMM_MST_USER where EID=" & Session("eid") & " AND UID=" & Session("UID").ToString() & ""
            'STR &= " UNION "
            'STR &= "select rolename [userrole] from MMM_REF_ROLE_USER where eid=" & Session("eid") & " AND UID=" & Session("UID").ToString() & " "
            'dac.SelectCommand.CommandText = STR
            'dac.Fill(dt)
            'For i As Integer = 0 To dt.Rows.Count - 1
            '    'If Session("USERROLE").ToString().ToUpper() <> dt.Rows(i).Item("rolename").ToString().ToUpper() Then
            '    ddlUserRole.Items.Add(dt.Rows(i).Item("userrole").ToString())
            '    'End If
            'Next

            'ddlUserRole.SelectedIndex = ddlUserRole.Items.IndexOf(ddlUserRole.Items.FindByText(Session("USERROLE").ToString))

            ' strColN = "SU"
            ' imglogo.ImageUrl = "~/logo/" & Session("CLOGO")
            'If Session("UID") Is Nothing Then
            '    Response.RedirectPermanent("Default.aspx?msg=NO_SESSION_FOUND")
            'End If
            'If Session("USERROLE").ToString() = "ADMIN" Then
            '    GetMenu(Session("USERROLE").ToString(), 1)
            'Else
            '    GetMenu(Session("USERROLE").ToString(), Val(Session("EID").ToString()))
            'End If
            ' commendted for gps menu feature
            ' PopulateMenu()
            ' new added 
            'Session("Page") = menuBar.SelectedItem
            'lbllink.Text = menuBar.SelectedValue


        End If
        Call GetMenuData()
    End Sub


    ''CODING BY VINAY

    Private Sub GetMenuData()
        Dim strCon As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim conn As SqlConnection = Nothing
        Dim cmd As SqlCommand = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim table As New DataTable()
            If Not Session("M_Menu") Is Nothing Then
                table = CType(Session("M_Menu"), DataTable)
            Else
                conn = New SqlConnection(strCon)
                If conn.State <> ConnectionState.Open Then
                    conn.Open()
                End If
                Dim sql As String = "select MID,MENUNAME,PAGELINK,pmenu,image,roles,dord from mmm_mst_menu where ROLES <> '0' and roles like '" & "%{" & Session("USERROLE") & "%" & "' AND EID=" & Session("EID") & "  order by dord "
                cmd = New SqlCommand(sql, conn)
                da = New SqlDataAdapter(cmd)
                da.Fill(table)
                Session("M_Menu") = table
            End If

            Dim view As New DataView(table)
            view.RowFilter = "Pmenu =0 "
            For Each row As DataRowView In view
                Dim menuItem As New MenuItem(row("MenuName").ToString(), row("MID").ToString(), "Images/" & row("Image") & "", row("Pagelink").ToString)
                If UCase(row("Pagelink")).ToString = "MENU" Then
                    menuItem.NavigateUrl = row("Pagelink").ToString().Replace("MENU", "#")
                Else
                    menuItem.NavigateUrl = row("Pagelink").ToString()
                End If
                menuBar.Items.Add(menuItem)
                AddChildItems(table, menuItem)
            Next
        Catch ex As Exception
        Finally
            If Not conn Is Nothing Then
                conn.Close()
                conn.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not cmd Is Nothing Then
                cmd.Dispose()
            End If

        End Try

    End Sub

    '' bakcup on 16_sep_14 
    'Private Sub AddChildItems(ByVal table As DataTable, ByVal menuItem As MenuItem)
    '    Dim viewItem As New DataView(table)
    '    viewItem.RowFilter = "Pmenu= " + menuItem.Value + ""
    '    For Each childView As DataRowView In viewItem
    '        Dim abc As String() = childView("roles").ToString().Split(",")
    '        For i As Integer = 0 To abc.Length - 1
    '            Dim a As String() = abc(i).ToString().Split(":")
    '            If UCase(Session("USERROLE")) = UCase(a(0).Remove(0, 1).ToString()) Then
    '                Dim childItem As New MenuItem(childView("MenuName").ToString, childView("MID").ToString, "Images/" & childView("Image") & "", childView("Pagelink").ToString)
    '                If UCase(childView("Pagelink")).ToString = "MENU" Then
    '                    childItem.NavigateUrl = UCase(childView("Pagelink")).ToString().Replace("MENU", "#")
    '                Else
    '                    childItem.NavigateUrl = childView("Pagelink").ToString()
    '                End If

    '                Dim result As String = checkview(childView("roles").ToString())
    '                If UCase(result) <> "EXIT" Then
    '                    menuItem.ChildItems.Add(childItem)
    '                    AddChildItems(table, childItem)
    '                End If

    '            End If
    '        Next
    '    Next
    'End Sub

    Private Sub AddChildItems(ByVal table As DataTable, ByVal menuItem As MenuItem)
        Dim viewItem As New DataView(table)
        viewItem.RowFilter = "Pmenu= " + menuItem.Value + ""
        For Each childView As DataRowView In viewItem
            Dim abc As String() = childView("roles").ToString().Split(",")
            For i As Integer = 0 To abc.Length - 1
                Dim a As String() = abc(i).ToString().Split(":")
                If UCase(Session("USERROLE")) = UCase(a(0).Remove(0, 1).ToString()) Then
                    Dim childItem As New MenuItem(childView("MenuName").ToString, childView("MID").ToString, "Images/" & childView("Image") & "", childView("Pagelink").ToString)
                    If UCase(childView("Pagelink")).ToString = "MENU" Then
                        childItem.NavigateUrl = UCase(childView("Pagelink")).ToString().Replace("MENU", "#")
                    Else
                        childItem.NavigateUrl = childView("Pagelink").ToString()
                    End If


                    Dim result As String = checkview(childView("roles").ToString())
                    If UCase(result) = "EXIT" And UCase(menuItem.Text.ToString()) = "DOCUMENT" Then
                        'If Not UCase(childView("Pagelink")).ToString.Contains("DOCUMENTS.ASPX?SC=") Then
                        Continue For
                    End If
                    menuItem.ChildItems.Add(childItem)
                    AddChildItems(table, childItem)
                End If
            Next
        Next
    End Sub

    '' new func added 09_sep_14 for hiding menu item if only view right is given

    Public Function checkview(ByVal roles As String) As String
        Dim result As String = String.Empty
        Dim str As String() = roles.ToString().Split(",")

        For i As Integer = 0 To str.Length - 1
            Dim aas As String() = str(i).ToString.Split(":")
            Dim a As String = aas(1).Remove(aas(1).Length - 1, 1)
            Dim s As String = aas(0).Replace("{", "")
            If a = "1" And UCase(Session("Userrole")) = UCase(s.ToString()) Then
                result = "EXIT"
            End If
        Next
        Return result
    End Function


    '' these are previous functions b4 adding gps menu feature added 
    'Sub PopulateMenu()
    '    Dim dst As DataSet = GetMenuData()
    '    For Each masterRow As DataRow In dst.Tables("top").Rows()
    '        Dim masterItem As New MenuItem(masterRow("MenuName").ToString(), masterRow("MenuName").ToString(), "Images/" & masterRow("ImageName") & "", masterRow("PageName").ToString)
    '        menuBar.Items.Add(masterItem)

    '        For Each childRow As DataRow In masterRow.GetChildRows("Children")
    '            If childRow.Item("menutype").ToString = "dynamic" Then
    '                Dim a1 As String() = childRow.Item(ViewState("rl")).ToString().Split(",")

    '                For i As Integer = 0 To a1.Length - 1

    '                    If a1(i).Length > 1 Then
    '                        Dim b1 As String() = a1(i).ToString().Split("-")
    '                        Dim childItem As New MenuItem(b1(1).ToString, b1(1).ToString, "Images/" & childRow("ImageName") & "", childRow("PageName").ToString & "?SC=" & b1(0).ToString & "")
    '                        masterItem.ChildItems.Add(childItem)
    '                    End If
    '                Next
    '            Else
    '                Dim childItem As New MenuItem(childRow("MenuName").ToString, childRow("MenuName").ToString, "Images/" & childRow("ImageName") & "", childRow("PageName").ToString)
    '                masterItem.ChildItems.Add(childItem)
    '            End If

    '        Next
    '        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        'Dim con As SqlCon1nection = New SqlConnection(conStr)
    '        'con.Open()
    '        'Dim oda As New SqlDataAdapter("SELECT * FROM MMM_MST_FORMS where FormSource = 'MENU DRIVEN' and EID=" & Session("eid") & " Order by FormName", con)
    '        'Dim ds As New DataSet
    '        'oda.Fill(ds, "menu")

    '        'oda.SelectCommand.CommandText = "SELECT ReportID,ReportName FROM MMM_MST_REPORT where EID=" & Session("eid") & " Order by DisplayOrder"
    '        'oda.Fill(ds, "report")
    '        'For i As Integer = 0 To ds.Tables("menu").Rows.Count - 1
    '        '    If ds.Tables("menu").Rows(i).Item("formType").ToString() = "MASTER" Then
    '        '        masterItem.ChildItems.Add(New MenuItem(" " & ds.Tables("menu").Rows(i).Item("formName"), ds.Tables("menu").Rows(i).Item("formName"), "Images/explorer.png", "Masters.aspx?SC=" & ds.Tables("menu").Rows(i).Item("formName")))
    '        '    End If
    '        'Next
    '        'con.Close()
    '        'oda.Dispose()
    '    Next

    '    'Dim mnuLogin As New MenuItem(" User Name :" & Session("USERNAME").ToString() & " ", "UserName")
    '    'mnuLogin.ChildItems.Add(New MenuItem(" Update Profile", "Passwordchange", "Images/lock.png", "profile.aspx"))
    '    'mnuLogin.ChildItems.Add(New MenuItem(" Sign Out", "SignOut", "Images/icon_exit.png", "SignOut.aspx"))
    '    'menuBar.Items.Add(mnuLogin)
    'End Sub
    'Function GetMenuData() As DataSet...
    '    Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
    '    Dim strColN As String
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    Dim dac As New SqlDataAdapter("select code from MMM_MST_Entity where eid=" & Session("eid") & "", con)
    '    Dim cid As String = dac.SelectCommand.ExecuteScalar()
    '    ' strColN = "SU"
    '    strColN = cid & "_" & Session("UserRole")
    '    ViewState("rl") = strColN
    '    Dim dst As New DataSet()
    '    Dim da As New SqlDataAdapter("SELECT * FROM mmm_mst_accessmenu where  Pmenu is null and [" & strColN & "]='1' order by dord", con)
    '    da.Fill(dst, "top")
    '    da.SelectCommand.CommandText = "SELECT * FROM mmm_mst_accessmenu where pagename is not null and pmenu is not null and [" & strColN & "] <> '0' order by dord"
    '    da.Fill(dst, "ver")
    '    If dst.Tables("top").Rows.Count > 0 And dst.Tables("ver").Rows.Count > 0 Then
    '        dst.Relations.Add("Children", dst.Tables("top").Columns("MenuName"), dst.Tables("ver").Columns("Pmenu"), False)
    '        Return dst
    '    End If
    '    con.Close()
    '    da.Dispose()
    'End Function


    Protected Sub ddlUserRole_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlUserRole.SelectedIndexChanged
        Dim usrRole As String = ddlUserRole.SelectedItem.Value
        Session("USERROLE") = UCase(usrRole)
        If Not Session("M_Menu") Is Nothing Then
            Session("M_Menu") = Nothing
        End If
        If Session("EID") = 54 Then
            Response.Redirect("http://industowers.myndsaas.com/GMapHome.aspx")
        ElseIf Session("EID") = 61 Then
            Response.Redirect("http://Here.myndsaas.com/VehicleRout.aspx")
        ElseIf Session("EID") = 53 Then
            Response.Redirect("dashboard.aspx")
        ElseIf Session("EID") = 46 Then
            Response.Redirect("Home.aspx")
        Else
            Response.Redirect("MainHome.aspx")
        End If

    End Sub


End Class
