
Imports System.Data
Imports System.Data.SqlClient

Partial Class usrFullScreenBPM
    Inherits System.Web.UI.MasterPage
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        If Session("UID") Is Nothing Then
            Response.Redirect("SessionOut.aspx")
        End If

        ' If ConfigurationManager.AppSettings("ImportantMessageToFlash") IsNot Nothing And Session("EID") IsNot Nothing Then
        '    If ConfigurationManager.AppSettings("ImportantMessageToFlash").ToString() = "ON" And
        '        ConfigurationManager.AppSettings("EIDNotToShow") IsNot Nothing Then
        '        If ConfigurationManager.AppSettings("EIDNotToShow").ToString().Contains(Session("EID")) = False Then
        '            lblDownTimeNotice.Visible = True
        '        End If
        '    End If
        'End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' lblLogo.Text = "<img width=""Auto"" class=""imglogo"" height=""50px"" src=""logo/" & Session("CLOGO") & """ alt=""" & Session("CODE") & """  />  "
            lblLogo.Text = "<img width=""Auto"" class=""imglogo mg0"" height=""45px"" src=""logo/" & Session("CLOGO") & """ alt=""" & Session("CODE") & """  />  "
            lblUserMsg.Text = " Welcome , " & Session("USERNAME") & "  "
            '' below if cond. new by sunil for not showing settings icon for 190 entity - tickeing
            If Not Session("EID") Is Nothing Then
                If Session("EID") = 190 Then
                    imgChangePwd.Visible = False
                End If
            End If
            Dim roles() As String = Session("Roles").ToString.Split(",")
            For i As Integer = 0 To roles.Length - 1
                ddlUserRole.Items.Add(roles(i).ToString())
            Next
            ddlUserRole.SelectedIndex = ddlUserRole.Items.IndexOf(ddlUserRole.Items.FindByText(Session("USERROLE").ToString))
            Call ChecknSet_SiteDownBannerText(Session("EID"))
        End If
        Call GetMenuData()
    End Sub

    Protected Sub ChecknSet_SiteDownBannerText(ByVal eid As Integer)
        Dim result As String = ""
        Dim objDC As New DataClass
        Dim ht As New Hashtable()
        ht.Add("@eid", eid)
        result = objDC.ExecuteProScaller("usp_GetSiteDownBannerText", ht)
        If Not String.IsNullOrEmpty(result) Then
            If result.Length > 5 Then
                'lblDownTimeNotice.InnerText = result
                lblDownTimeNotice.Visible = True
                mardowntime.InnerText = result
            End If
        Else
            'lblDownTimeNotice.InnerText = ""
            lblDownTimeNotice.Visible = False
            mardowntime.InnerText = ""
        End If
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
        Else
            Response.Redirect("Home.aspx")
        End If

    End Sub
End Class

