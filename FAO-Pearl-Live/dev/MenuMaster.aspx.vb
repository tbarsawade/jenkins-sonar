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
Partial Class MM
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Call bindgrid()
            Call GetMenuData()
            Dim da As New SqlDataAdapter("select * from mmm_mst_menu where eid=" & Session("EID") & " order by menuname", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")

            ddlpar.DataSource = ds.Tables("data")
            ddlpar.DataTextField = "Menuname"
            ddlpar.DataValueField = "Mid"
            ddlpar.DataBind()
            ddlpar.Items.Insert(0, New ListItem("Select"))

            ddlchngp.DataSource = ds.Tables("data")
            ddlchngp.DataTextField = "Menuname"
            ddlchngp.DataValueField = "Mid"
            ddlchngp.DataBind()
            ddlchngp.Items.Insert(0, New ListItem("Select"))

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

    Private Shared Sub Addchildtree(ByVal table As DataTable, ByVal menuItem As MenuItem)
        Dim viewItem As New DataView(table)
        viewItem.RowFilter = "PMENU = " + menuItem.Value
        For Each childView As DataRowView In viewItem
            Dim childItem As New MenuItem(childView("MENUNAME").ToString(), childView("MID").ToString(), "", "")
            menuItem.ChildItems.Add(childItem)
            Addchildtree(table, childItem)
        Next
    End Sub

    Private Sub GetMenuData()
        Dim table As New DataTable()
        Dim strCon As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim conn As New SqlConnection(strCon)

        '   Dim strColN As String
        If conn.State <> ConnectionState.Open Then
            conn.Open()
        End If

        Dim sql As String = ""
        sql = "select MID,MENUNAME,PAGELINK,pmenu,image,roles,dord from mmm_mst_menu where  EID=" & Session("EID") & "  order by dord "

        Dim cmd As New SqlCommand(sql, conn)
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(table)
        Dim view As New DataView(table)
        view.RowFilter = "PMENU =0"
        For Each row As DataRowView In view
            Dim menuItem As New MenuItem(row("MENUNAME").ToString(), row("MID").ToString(), "Images/" & row("IMAGE") & "", "")
            MenuPreview.Items.Add(menuItem)
            AddChildItems(table, menuItem)
        Next
    End Sub

    Private Shared Sub AddChildItems(ByVal table As DataTable, ByVal menuItem As MenuItem)
        Dim viewItem As New DataView(table)
        viewItem.RowFilter = "PMENU = " + menuItem.Value
        For Each childView As DataRowView In viewItem
            Dim childItem As New MenuItem(childView("MENUNAME").ToString(), childView("MID").ToString(), "Images/" & childView("IMAGE") & "", "")
            menuItem.ChildItems.Add(childItem)
            AddChildItems(table, childItem)
        Next
    End Sub




    Public Sub bindgrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim da As New SqlDataAdapter("SELECT rolename FROM MMM_MST_role where eid=" & Session("eid") & " ", con)
        Dim ds As New DataSet
        da.Fill(ds, "role")
        Gvrole.DataSource = ds.Tables("role")
        Gvrole.DataBind()
        con.Close()
        da.Dispose()
    End Sub

    Public Sub cleargrid()
        For Each di As GridViewRow In Gvrole.Rows
            Dim chkv As CheckBox = DirectCast(di.FindControl("chbview"), CheckBox)
            Dim chkc As CheckBox = DirectCast(di.FindControl("chbcreate"), CheckBox)
            Dim chke As CheckBox = DirectCast(di.FindControl("chbedit"), CheckBox)
            Dim chkd As CheckBox = DirectCast(di.FindControl("chbdelete"), CheckBox)
            chkc.Checked = False
            chkv.Checked = False
            chke.Checked = False
            chkd.Checked = False
        Next
    End Sub

    Private Sub GETROLES()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim abc As String = ""
        For Each di As GridViewRow In Gvrole.Rows
            Dim num As Integer = 0
            Dim role As String = di.Cells(0).Text.ToString()
            Dim chkv As CheckBox = DirectCast(di.FindControl("chbview"), CheckBox)
            Dim chkc As CheckBox = DirectCast(di.FindControl("chbcreate"), CheckBox)
            Dim chke As CheckBox = DirectCast(di.FindControl("chbedit"), CheckBox)
            Dim chkd As CheckBox = DirectCast(di.FindControl("chbdelete"), CheckBox)

            If chkv.Checked Then
                num += 1
            End If
            If chkc.Checked Then
                num += 2
            End If
            If chke.Checked Then
                num += 4
            End If
            If chkd.Checked Then
                num += 8
            End If
            If num <> 0 Then
                role = "{" & role & ":" & num & "},"
                abc &= role

                ViewState("Roles") = abc.Remove(abc.Length - 1)
            End If
            If ViewState("Roles") Is Nothing Then
                ViewState("Roles") = 0
            End If
        Next

    End Sub

    Private Sub UploadIcon()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            If uploadimage.HasFile Then
                Dim filename As String = Path.GetFileName(uploadimage.FileName)
                ViewState("FILENAME") = filename
                Dim file As HttpPostedFile = uploadimage.PostedFile
                Dim fileExtension As String = System.IO.Path.GetExtension(filename)
                Dim fileMimeType As String = uploadimage.PostedFile.ContentType
                Dim fileLengthInKB As Integer = uploadimage.PostedFile.ContentLength / 20
                Dim getpath As String = [String].Format("~/Images/{0}", "m" & filename)
                imgicon.ImageUrl = getpath
                Dim matchExtension As String() = {".png"}
                Dim matchMimeType As String() = {"image/x-png", "image/png"}
                Dim img As System.Drawing.Image = System.Drawing.Image.FromStream(uploadimage.PostedFile.InputStream)
                Dim imgHeight As Single = img.PhysicalDimension.Height
                Dim imgWidth As Single = img.PhysicalDimension.Width
                Dim input As String = Request.Url.AbsoluteUri
                Dim output As String = input.Substring(input.IndexOf("="c) + 1)
                Dim stream As Stream = uploadimage.PostedFile.InputStream
                Dim sourceImage As New System.Drawing.Bitmap(stream)
                Dim maxImageWidth As Integer = 16
                Dim newImageHeight As Integer = 16
                Dim resizedImage As New System.Drawing.Bitmap(maxImageWidth, newImageHeight)
                Dim gr As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(resizedImage)
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                gr.DrawImage(sourceImage, 0, 0, maxImageWidth, newImageHeight)

                If matchExtension.Contains(fileExtension) AndAlso matchMimeType.Contains(fileMimeType) Then
                    If fileLengthInKB <= 20480 Then
                        If imgHeight > 80 Or imgWidth > 80 Then
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('This image is too large.max height is 80 pixels & max width is 80 pixels.');window.location='MenuMaster.aspx';", True)

                        End If
                        'saving this image for icon display in menu
                        resizedImage.Save(Server.MapPath((Convert.ToString("Images/") & filename)))
                        'saving this image as the original size for mobile
                        uploadimage.PostedFile.SaveAs(Server.MapPath("Images/") & "m" & filename)
                    Else
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Image size can not be exceed 20kb');window.location='MenuMaster.aspx';", True)

                    End If
                Else

                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* File type must be .png');window.location='MenuMaster.aspx';", True)
                End If
            Else
                ViewState("FILENAME") = ""
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* " & ex.Message & "' );window.location='MenuMaster.aspx';", True)


        End Try
    End Sub


    Private Sub saverecords()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Call UploadIcon()
        Call GETROLES()
        If txtchild.Text = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu name can not be blank!!');", True)
            Exit Sub
        End If
        lblmenu.Text = "New"
        'If lblmenu.Text = txtchild.Text Then
        '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Parent menu and Menu Name can not be same, So you can't save!!');window.location='MenuMaster.aspx';", True)
        'End If
        'da.SelectCommand.CommandText = "SELECT COUNT(*) FROM MMM_MST_MENU WHERE EID=" & Session("EID") & " AND MENUNAME='" & txtchild.Text & "'"
        'If con.State <> ConnectionState.Open Then
        '    con.Open()
        'End If

        Dim ismobile As Integer = 0
        If chkparent.Checked = True Then
            ismobile = 1
        Else
            ismobile = 0
        End If


        'Dim AB As Integer = da.SelectCommand.ExecuteScalar()
        'If AB > 0 Then
        '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu name already exists!!');", True)
        '    Exit Sub
        'End If
        da.SelectCommand.CommandText = "USPBINDMENUinsert"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
        da.SelectCommand.Parameters.AddWithValue("@PMENU", lblmenu.Text)
        da.SelectCommand.Parameters.AddWithValue("@MENUNAME", txtchild.Text)
        da.SelectCommand.Parameters.AddWithValue("@PLTYPE", "New")
        da.SelectCommand.Parameters.AddWithValue("@PAGELINK", "MENU")
        da.SelectCommand.Parameters.AddWithValue("@ICONAME", ViewState("FILENAME"))
        da.SelectCommand.Parameters.AddWithValue("@ROLES", ViewState("Roles"))
        da.SelectCommand.Parameters.AddWithValue("@DORD", txtdord.Text)
        da.SelectCommand.Parameters.AddWithValue("@Ismobile", ismobile)
        da.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim MID As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu saved successfully.');window.location='MenuMaster.aspx';", True)
        SendMailMenuLog(MID, "SAVE")
        con.Close()
        da.Dispose()




    End Sub

    Protected Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click
        saverecords()

        If Not Session("M_Menu") Is Nothing Then
            Session("M_Menu") = Nothing
        End If
        UpdatePanel1.Update()
    End Sub

    Protected Sub ddlplt1_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        lblmsg.Text = ""

        If ddlplt1.SelectedItem.Text = "Master" Then
            da.SelectCommand.CommandText = "Select formname,formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Master' and formsource='menu driven' order by formname "
            da.Fill(ds, "Master")
            If ds.Tables("Master").Rows.Count > 0 Then
                ddlpl1.DataSource = ds.Tables("Master")
                ddlpl1.DataTextField = "formname"
                ddlpl1.DataValueField = "formname"
                ddlpl1.DataBind()
            Else
                lblmsg.Text = "No Page Found"
                ddlpl1.Items.Clear()
                Exit Sub
            End If
        ElseIf ddlplt1.SelectedItem.Text = "Document" Then
            da.SelectCommand.CommandText = "Select formname,formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Document' and formsource='menu driven' order by formname"
            da.Fill(ds, "Document")
            If ds.Tables("Document").Rows.Count > 0 Then
                ddlpl1.DataSource = ds.Tables("Document")
                ddlpl1.DataTextField = "formname"
                ddlpl1.DataValueField = "formname"
                ddlpl1.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl1.Items.Clear()
                Exit Sub
            End If
        ElseIf ddlplt1.SelectedItem.Text = "Report" Then
            da.SelectCommand.CommandText = "Select reportid,reportname from mmm_mst_report where eid=" & Session("EID") & " order by reportname"
            da.Fill(ds, "Report")
            If ds.Tables("Report").Rows.Count > 0 Then
                ddlpl1.DataSource = ds.Tables("Report")
                ddlpl1.DataTextField = "reportname"
                ddlpl1.DataValueField = "reportname"
                ddlpl1.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl1.Items.Clear()
                Exit Sub
            End If
        ElseIf ddlplt1.SelectedItem.Text = "Static" Then
            'da.SelectCommand.CommandText = "Select mid,Menuname,pagelink from mmm_mst_menu where eid=" & Session("EID") & " and pagelink<>'menu' and MTYPE='STATIC' order by menuname"
            da.SelectCommand.CommandText = "select pagelink,pagedesc from mmm_mst_static_pages WHERE isActive=1 ORDER BY PAGEDESC desc"
            da.Fill(ds, "Static")
            If ds.Tables("Static").Rows.Count > 0 Then
                ddlpl1.DataSource = ds.Tables("Static")
                ddlpl1.DataTextField = "pagedesc"
                ddlpl1.DataValueField = "pagelink"
                ddlpl1.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl1.Items.Clear()
                Exit Sub
            End If
        ElseIf ddlplt1.SelectedItem.Text = "Calendar" Then
            da.SelectCommand.CommandText = "Select formid,formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype in ('Document','master') and formsource='menu driven' and iscalendar=1 order by formname"
            da.Fill(ds, "Calender")
            If ds.Tables("Calender").Rows.Count > 0 Then
                ddlpl1.DataSource = ds.Tables("Calender")
                ddlpl1.DataTextField = "formname"
                ddlpl1.DataValueField = "formname"
                ddlpl1.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl1.Items.Clear()
                Exit Sub
            End If

        ElseIf ddlplt1.SelectedItem.Text = "Master Calendar" Then
            da.SelectCommand.CommandText = "Select tid,name from mmm_mst_calendar  where eid=" & Session("EID") & " order by name"
            da.Fill(ds, "masterCalender")
            If ds.Tables("masterCalender").Rows.Count > 0 Then
                ddlpl1.DataSource = ds.Tables("masterCalender")
                ddlpl1.DataTextField = "name"
                ddlpl1.DataValueField = "name"
                ddlpl1.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl1.Items.Clear()
                Exit Sub
            End If
        End If


    End Sub

    Protected Sub btnsavesubmenu_Click(sender As Object, e As EventArgs) Handles btnsavesubmenu.Click
        savesubmenu()
        If Not Session("M_Menu") Is Nothing Then
            Session("M_Menu") = Nothing
        End If
        UpdatePanel1.Update()
    End Sub

    Private Sub savesubmenu()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If ddlpar.SelectedItem.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please Select Parent Menu');", True)
            Exit Sub
        End If
        If ddlplt1.SelectedItem.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please Select Pagelink Type');", True)
            Exit Sub
        End If
        If txtchild1.Text = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu Name Cannot be blank!!!!!');", True)
            Exit Sub
        End If
        'da.SelectCommand.CommandText = "SELECT COUNT(*) FROM MMM_MST_MENU WHERE EID=" & Session("EID") & " AND MENUNAME='" & txtchild1.Text & "'"
        'If con.State <> ConnectionState.Open Then
        '    con.Open()
        'End If
        'Dim AB As Integer = da.SelectCommand.ExecuteScalar()
        'If AB > 0 Then
        '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Submenu name already exists!!');", True)
        '    Exit Sub
        'End If
        If ddlpl1.Items.Count = 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('You Cannot create this menu!!!!!');", True)
            Exit Sub
        End If

        Dim ismobile As Integer = 0
        If chkmobile.Checked = True Then
            ismobile = 1
        Else
            ismobile = 0
        End If

        Call UploadIconsubmenu()
        Call GETROLES()

        lblmenu.Text = ddlpar.SelectedItem.Text
        da.SelectCommand.CommandText = "USPBINDMENUinsert"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
        da.SelectCommand.Parameters.AddWithValue("@PMENU", ddlpar.SelectedItem.Text)
        da.SelectCommand.Parameters.AddWithValue("@MENUNAME", txtchild1.Text)
        da.SelectCommand.Parameters.AddWithValue("@PLTYPE", ddlplt1.SelectedItem.Text)
        da.SelectCommand.Parameters.AddWithValue("@PAGELINK", ddlpl1.SelectedItem.Text.Trim())
        da.SelectCommand.Parameters.AddWithValue("@ICONAME", ViewState("FILENAME"))
        da.SelectCommand.Parameters.AddWithValue("@ROLES", ViewState("Roles"))
        da.SelectCommand.Parameters.AddWithValue("@DORD", txtdord1.Text)
        da.SelectCommand.Parameters.AddWithValue("@ismobile", ismobile)
        da.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim MID As String = Convert.ToString(da.SelectCommand.ExecuteScalar())

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Sub Menu saved successfully.');window.location='MenuMaster.aspx';", True)
        SendMailMenuLog(Convert.ToInt32(MID), "SAVE")
        con.Close()
        da.Dispose()
    End Sub

    Protected Sub MenuPreview_MenuItemClick(sender As Object, e As MenuEventArgs) Handles MenuPreview.MenuItemClick
        If MenuPreview.SelectedItem Is Nothing Then
            Return
        End If
        cleargrid()
        btndelete.Visible = True
        lblchngp.Visible = True
        ddlchngp.Visible = True
        ViewState("as") = MenuPreview.SelectedItem.Text
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select * from mmm_mst_menu where eid= " & Session("EID") & " and mid= '" & e.Item.Value & "'", con)
        Dim ds As New DataSet
        da.Fill(ds, "edit")
        ViewState("MID") = ds.Tables("edit").Rows(0).Item("MID").ToString()
        txtchild2.Text = ds.Tables("edit").Rows(0).Item("Menuname").ToString()
        txtdord2.Text = ds.Tables("edit").Rows(0).Item("dord").ToString()
        Dim ismobile As Integer = ds.Tables("edit").Rows(0).Item("Ismobile").ToString()

        'Dim isCal As Integer = ds.Tables("edit").Rows(0).Item("Iscalendar").ToString()

        Dim imgg As String = ds.Tables("edit").Rows(0).Item("Image").ToString()
        Dim getpath As String = [String].Format("~/Images/{0}", imgg)
        img3.ImageUrl = getpath
        ViewState("FILENAME") = imgg
        Dim ab2 As String = ds.Tables("edit").Rows(0).Item("pmenu").ToString()

        ViewState("ab2") = ab2
        ViewState("new") = "New"
        Dim plink As String = ds.Tables("edit").Rows(0).Item("Pagelink").ToString()
        Dim mnt As String = ""
        If ismobile = 0 Then
            CheckBox1.Checked = False
        Else
            CheckBox1.Checked = True
        End If

        If ab2 = 0 Then
            lbl3.Text = "New"
            'lblmenu.Text = ViewState("as")
            ddlpt2.Visible = False
            ddlpl2.Visible = False
            Label3.Visible = False
            Label4.Visible = False
        Else
            ddlpt2.Visible = True
            ddlpl2.Visible = True
            Label3.Visible = True
            Label4.Visible = True
            da.SelectCommand.CommandText = "select menuname from mmm_mst_menu where eid=" & Session("EID") & " and mid=" & ab2 & ""
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            mnt = da.SelectCommand.ExecuteScalar()
            lbl3.Text = mnt.ToString()
            ViewState("as") = lbl3.Text
            con.Close()
        End If

        If UCase(plink).Contains("MASTERS.ASPX?SC=") Then

            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Master"

            da.SelectCommand.CommandText = "Select * from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Master' order by FormName"
            da.Fill(ds, "Master")
            ddlpl2.DataSource = ds.Tables("Master")
            ddlpl2.DataTextField = "formname"
            ddlpl2.DataValueField = "formname"
            ddlpl2.DataBind()
            ddlpl2.SelectedIndex = ddlpl2.Items.IndexOf(ddlpl2.Items.FindByText(plink.Substring(16)))

        ElseIf UCase(plink).Contains("NEWMASTER.ASPX?SC=") Then
            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Kendo Master"
            da.SelectCommand.CommandText = "Select * from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Master' order by FormName"
            da.Fill(ds, "Master")
            ddlpl2.DataSource = ds.Tables("Master")
            ddlpl2.DataTextField = "formname"
            ddlpl2.DataValueField = "formname"
            ddlpl2.DataBind()
            ddlpl2.SelectedIndex = ddlpl2.Items.IndexOf(ddlpl2.Items.FindByText(plink.Substring(18)))


        ElseIf UCase(plink).Contains("DOCUMENTS.ASPX?SC=") Then

            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Document"
            da.SelectCommand.CommandText = "Select * from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Document' and formsource='menu Driven' order by FormName"
            da.Fill(ds, "Document")
            ddlpl2.DataSource = ds.Tables("Document")
            ddlpl2.DataTextField = "formname"
            ddlpl2.DataValueField = "formname"
            ddlpl2.DataBind()
            Try
                ddlpl2.SelectedIndex = ddlpl2.Items.IndexOf(ddlpl2.Items.FindByText(plink.Substring(18)))
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Some one change Document Name !!!');window.location='MenuMaster.aspx';", True)
            End Try





        ElseIf UCase(plink).Contains("REPORTMASTER.ASPX?SC=") Then
            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Report"
            da.SelectCommand.CommandText = "Select * from mmm_mst_report where eid=" & Session("EID") & " "
            da.Fill(ds, "Report")
            ddlpl2.DataSource = ds.Tables("Report")
            ddlpl2.DataTextField = "reportname"
            ddlpl2.DataValueField = "reportname"
            ddlpl2.DataBind()
            ddlpl2.SelectedIndex = ddlpl2.Items.IndexOf(ddlpl2.Items.FindByText(plink.Substring(21)))

        ElseIf UCase(plink).Contains("DOCEVENTS.ASPX?SC=") Then
            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Calendar"
            da.SelectCommand.CommandText = "Select * from mmm_mst_forms where eid=" & Session("EID") & " and formsource='menu driven' and iscalendar=1 order by formname "
            da.Fill(ds, "Calendar")
            ddlpl2.DataSource = ds.Tables("Calendar")
            ddlpl2.DataTextField = "formname"
            ddlpl2.DataValueField = "formid"
            ddlpl2.DataBind()
            ddlpl2.SelectedIndex = ddlpl2.Items.IndexOf(ddlpl2.Items.FindByText(plink.Substring(18)))
        ElseIf UCase(plink).Contains("MASTEREVENTS.ASPX?SC=") Then
            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Master Calendar"
            da.SelectCommand.CommandText = "Select name,tid from mmm_mst_calendar where eid=" & Session("EID") & " order by name "
            da.Fill(ds, "masterCalendar")
            ddlpl2.DataSource = ds.Tables("masterCalendar")
            ddlpl2.DataTextField = "name"
            ddlpl2.DataValueField = "tid"
            ddlpl2.DataBind()
            ddlpl2.SelectedIndex = ddlpl2.Items.IndexOf(ddlpl2.Items.FindByText(plink.Substring(21)))
        Else

            ddlpt2.Items.Clear()
            ddlpt2.Items.Insert(0, "Select")
            ddlpt2.Items.Insert(1, "Static")
            ddlpt2.Items.Insert(2, "Master")
            ddlpt2.Items.Insert(3, "Document")
            ddlpt2.Items.Insert(4, "Report")
            ddlpt2.Items.Insert(5, "Calendar")
            ddlpt2.Items.Insert(6, "Master Calendar")
            ddlpt2.Items.Insert(7, "Kendo Master")
            ddlpt2.SelectedItem.Text = "Static"
            'da.SelectCommand.CommandText = "Select mid,Menuname,pagelink from mmm_mst_menu where eid=" & Session("EID") & " and pagelink<>'menu' "
            da.SelectCommand.CommandText = "select upper(pagelink) as pagelink,upper(pagedesc) as pagedesc from mmm_mst_static_pages where isactive=1"
            da.Fill(ds, "static")
            ddlpl2.DataSource = ds.Tables("static")
            ddlpl2.DataTextField = "pagedesc"
            ddlpl2.DataValueField = "pagelink"
            ddlpl2.DataBind()
            'da.SelectCommand.CommandText = "select menuname from mmm_mst_menu where eid=" & Session("EID") & " and pagelink ='" & plink.ToString() & "'"
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'Dim ms As String = da.SelectCommand.ExecuteScalar()
            If plink <> "MENU" Then
                If plink <> "" Then
                    Try
                        ddlpl2.SelectedValue = plink.ToUpper
                    Catch ex As Exception
                        ddlpl2.SelectedIndex = 0
                    End Try
                End If
            End If
            con.Close()
        End If

        ' this qry is for filling grid as per given rights

        Dim str As String() = ds.Tables("edit").Rows(0).Item("roles").ToString().Split(",")

        For i As Integer = 0 To str.Length - 1
            Dim ab As String() = str(i).ToString().Split(":")
            If ab.Length > 1 Then
                Dim val As Integer = ab(1).Remove(ab(1).Length - 1)
                For Each gvr As GridViewRow In Gvrole.Rows
                    If gvr.Cells(0).Text = ab(0).Remove(0, 1).ToString() Then
                        Dim chkv As CheckBox = DirectCast(gvr.FindControl("chbview"), CheckBox)
                        Dim chkc As CheckBox = DirectCast(gvr.FindControl("chbcreate"), CheckBox)
                        Dim chke As CheckBox = DirectCast(gvr.FindControl("chbedit"), CheckBox)
                        Dim chkd As CheckBox = DirectCast(gvr.FindControl("chbdelete"), CheckBox)
                        If val = 1 Then
                            chkv.Checked = True
                        End If
                        If val = 2 Then
                            chkc.Checked = True
                        End If
                        If val = 4 Then
                            chke.Checked = True
                        End If
                        If val = 8 Then
                            chkd.Checked = True
                        End If
                        If val = 3 Then
                            chkc.Checked = True
                            chkv.Checked = True
                        End If
                        If val = 5 Then
                            chkv.Checked = True
                            chke.Checked = True
                        End If
                        If val = 9 Then
                            chkv.Checked = True
                            chkd.Checked = True
                        End If
                        If val = 6 Then
                            chkc.Checked = True
                            chke.Checked = True
                        End If
                        If val = 10 Then
                            chkc.Checked = True
                            chkd.Checked = True
                        End If
                        If val = 7 Then
                            chkc.Checked = True
                            chkv.Checked = True
                            chke.Checked = True
                        End If
                        If val = 12 Then
                            chke.Checked = True
                            chkd.Checked = True
                        End If
                        If val = 11 Then
                            chkc.Checked = True
                            chkv.Checked = True
                            chkd.Checked = True
                        End If
                        If val = 13 Then
                            chkd.Checked = True
                            chke.Checked = True
                            chkv.Checked = True
                        End If
                        If val = 14 Then
                            chkd.Checked = True
                            chke.Checked = True
                            chkc.Checked = True
                        End If
                        If val = 15 Then
                            chkd.Checked = True
                            chke.Checked = True
                            chkv.Checked = True
                            chkc.Checked = True
                        End If
                    End If
                Next

            End If
        Next
    End Sub

    Private Sub updaterecords()
        If MenuPreview.SelectedItem Is Nothing Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Select Menu to Update');window.location='MenuMaster.aspx';", True)
            Return
        End If
        If txtchild2.Text = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu Name Cannot be blank!!!!!');", True)
            Exit Sub
        End If
        If lbl3.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Parent Name Cannot be Select!!!!!');", True)
            Exit Sub
        End If
        If ddlpl2.Items.Count = 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('You Cannot update this menu!!!!!');", True)
            Exit Sub
        End If

        Dim ismobile As Integer = 0
        If CheckBox1.Checked = True Then
            ismobile = 1
        Else
            ismobile = 0
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Call UploadIconedit()
        Call GETROLES()
        ' lbl3.Text = MenuPreview.SelectedItem.Text
        da.SelectCommand.CommandText = "select * from mmm_mst_menu where eid=" & Session("EID") & " and menuname='" & MenuPreview.SelectedItem.Text & "'"
        da.Fill(ds, "menu")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        If UCase(txtchild2.Text) = UCase(lbl3.Text) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Parent menu and menu name cannot be same.');", True)
            Exit Sub
        End If
        'If lbl3.Text = txtchild2.Text Then
        '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Parent menu and Menu Name can not be same, so you can't update!!');window.location='MenuMaster.aspx';", True)
        'End If
        'If UCase(MenuPreview.SelectedItem.Text) <> UCase(txtchild2.Text) Then
        '    da.SelectCommand.CommandText = "SELECT COUNT(*) FROM MMM_MST_MENU WHERE EID=" & Session("EID") & " AND MENUNAME='" & txtchild2.Text & "'"
        '    If con.State <> ConnectionState.Open Then
        '        con.Open()
        '    End If
        '    Dim AB As Integer = da.SelectCommand.ExecuteScalar()
        '    If AB > 0 Then
        '        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu name already exists!!');", True)
        '        Exit Sub
        '    End If
        'End If
        Dim MenuLog As String = ""
        da.SelectCommand.CommandText = "USPBINDMENUupdate"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        If ViewState("ab2") = 0 Then
            da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
            da.SelectCommand.Parameters.AddWithValue("@PMENU", ViewState("new"))
            da.SelectCommand.Parameters.AddWithValue("@MENUNAME", txtchild2.Text)
            da.SelectCommand.Parameters.AddWithValue("@PLTYPE", "New")
            da.SelectCommand.Parameters.AddWithValue("@PAGELINK", "MENU")
            da.SelectCommand.Parameters.AddWithValue("@ICONAME", ViewState("FILENAME"))
            da.SelectCommand.Parameters.AddWithValue("@ROLES", ViewState("Roles"))
            da.SelectCommand.Parameters.AddWithValue("@DORD", txtdord2.Text)
            da.SelectCommand.Parameters.AddWithValue("@mid", ViewState("MID"))
            da.SelectCommand.Parameters.AddWithValue("@ismobile", ismobile)
            da.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))
        Else
            MenuLog = CreateMenuMasterLog(ViewState("MID"))
            da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
            da.SelectCommand.Parameters.AddWithValue("@PMENU", lbl3.Text)
            da.SelectCommand.Parameters.AddWithValue("@MENUNAME", txtchild2.Text)
            da.SelectCommand.Parameters.AddWithValue("@PLTYPE", ddlpt2.SelectedItem.Text)
            da.SelectCommand.Parameters.AddWithValue("@PAGELINK", ddlpl2.SelectedValue)
            da.SelectCommand.Parameters.AddWithValue("@ICONAME", ViewState("FILENAME"))
            da.SelectCommand.Parameters.AddWithValue("@ROLES", ViewState("Roles"))
            da.SelectCommand.Parameters.AddWithValue("@DORD", txtdord2.Text)
            da.SelectCommand.Parameters.AddWithValue("@mid", ViewState("MID"))
            da.SelectCommand.Parameters.AddWithValue("@ismobile", ismobile)
            da.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))
        End If

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim rowAffrected As Integer = 0
        rowAffrected = da.SelectCommand.ExecuteNonQuery()
        If rowAffrected > 0 Then
            Dim objDMSUtil As New DMSUtil()
            objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Menu Master", "Menu Master Updation : " & MenuLog, ViewState("MID"))
        End If
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu Updated successfully.');window.location='MenuMaster.aspx';", True)
        SendMailMenuLog(Convert.ToInt32(ViewState("MID")), "UPDATE")

        con.Close()
        da.Dispose()

    End Sub

    Public Function CreateMenuMasterLog( ByVal Mid As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from mmm_mst_menu where mid=" & Mid & " and EID=" & Session("EID").ToString())
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                Logstring += " Menu Name : " & dt.Rows(0).Item("MenuName") & ""
                If dt.Rows(0).Item("PageLink").ToString() <> ddlpl2.SelectedValue Then
                    Logstring += "|| Page Link : " & dt.Rows(0).Item("PageLink") & " Change " & ddlpl2.SelectedValue
                End If
                If dt.Rows(0).Item("Roles").ToString() <> ViewState("Roles") Then
                    Logstring += "|| Role assign : " & dt.Rows(0).Item("Roles") & " Change " & ViewState("Roles")
                End If
                Logstring += " Changed By Uid - " & Session("UID")
            End If
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function

    Private Sub SendMailMenuLog(MID As Integer, ACTION As String)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ds As New DataSet
        Using con = New SqlConnection(conStr)
            Using da = New SqlDataAdapter("DMS.GetMenuLogDetail", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                da.SelectCommand.Parameters.AddWithValue("@mid", MID)
                da.Fill(ds, "data")
            End Using
        End Using
        If (ds.Tables("data").Rows.Count > 0) Then
            Dim mSub As String = "Menu Log Details"
            Dim bBody As New StringBuilder()
            bBody.Append("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head>    <title>Welcome To MyeDMS</title></head><body style=""margin: 0;font: 12px/0.7500em  Times New Roman;color: #555;text-align: left;""><div style=""text-align:left;width:100%;"">Dear Sir/ Madam,<br />Please find below the summary of changes in Menu Master:<br /><br />")
            bBody.Append("<table border=""2"" width=""60%""><tbody><tr><th style=""white-space:nowrap"">Menu Name</th><th style=""white-space:nowrap"">Updated On</th><th style=""white-space:nowrap"">Action By</th><th style=""white-space:nowrap"">Role</th><th>Action</th>")
            If (ACTION = "UPDATE") Then
                bBody.Append("<th>Change</th>")
            End If
            bBody.Append("<th>Access</th></tr>")
            For Each row As DataRow In ds.Tables("data").Rows
                bBody.Append("<tr><td>" & Convert.ToString(row("MenuName")) & "</td><td>" & Convert.ToString(row("UpdatedOn")) & "</td><td>" & Convert.ToString(row("ActionBy")) & "</td><td>" & Convert.ToString(row("Role")) & "</td><td>" & Convert.ToString(row("Action")) & "</td>")
                If (ACTION = "UPDATE") Then
                    bBody.Append("<td>" & Convert.ToString(row("Change")) & "</td>")
                End If
                bBody.Append("<td>" & Convert.ToString(row("Access")) & "</td></tr>")
            Next row

            bBody.Append("</tbody></table><br /><b>Regards,</b><br />This is System generated mail for Menu Access change</div></body></html>")

            Try
                Dim objM As New MailUtill(EID:=Session("EID"))
                objM.SendMail(ToMail:=Convert.ToString(Session("MenuLogTo")), Subject:=mSub, MailBody:=bBody.ToString(), CC:=Convert.ToString(Session("MenuLogCC")), Attachments:="", BCC:="")
            Catch ex As Exception
            End Try

        End If

    End Sub


    Protected Sub btnupdate_Click(sender As Object, e As EventArgs) Handles btnupdate.Click
        updaterecords()
        If Not Session("M_Menu") Is Nothing Then
            Session("M_Menu") = Nothing
        End If
        UpdatePanel1.Update()
    End Sub

    Public Function CreateMenuMasterLogForDelete(ByVal Mid As Integer) As String
        Dim Logstring As String = ""
        Try
            Dim dataClass As New DataClass()
            Dim dt As DataTable = dataClass.ExecuteQryDT("select * from mmm_mst_menu where mid=" & Mid & " and EID=" & Session("EID").ToString())
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                Logstring += " Menu Name : " & dt.Rows(0).Item("MenuName") & ""
                Logstring += "|| Page Link : " & dt.Rows(0).Item("PageLink")
                Logstring += "|| Role assign : " & dt.Rows(0).Item("Roles")
                Logstring += " deleted By Uid - " & Session("UID")
            End If
            Return Logstring.ToString()
        Catch ex As Exception
        End Try
    End Function

    Protected Sub btndelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btndelete.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim MenuLog = CreateMenuMasterLogForDelete(ViewState("MID"))
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.CommandText = "DeleteRecordWithChild"
        da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
        da.SelectCommand.Parameters.AddWithValue("@MID", ViewState("MID"))
        Dim rowAffrected As Integer = da.SelectCommand.ExecuteNonQuery()
        If rowAffrected > 0 Then
            Dim objDMSUtil As New DMSUtil()
            objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "Menu Master", "Menu Master Deleted : " & MenuLog, ViewState("MID"))
        End If
        SendMailMenuLog(Convert.ToInt32(ViewState("MID")), "DELETE")
        con.Close()
        da.Dispose()
        If Not Session("M_Menu") Is Nothing Then
            Session("M_Menu") = Nothing
        End If
        UpdatePanel1.Update()
        cleargrid()
        GetMenuData()
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Menu Deleted successfully!!!');window.location='MenuMaster.aspx';", True)


    End Sub

    Protected Sub ddlpt2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlpt2.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        If ddlpt2.SelectedItem.Text = "Master" Then
            da.SelectCommand.CommandText = "Select formid,formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Master' and formsource='menu driven' "
            da.Fill(ds, "Master")

            If ds.Tables("Master").Rows.Count > 0 Then
                ddlpl2.DataSource = ds.Tables("Master")
                ddlpl2.DataTextField = "formname"
                ddlpl2.DataValueField = "formname"
                ddlpl2.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl2.Items.Clear()
                Exit Sub
            End If
        ElseIf ddlpt2.SelectedItem.Text = "Document" Then
            da.SelectCommand.CommandText = "Select formid,formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype='Document' and formsource='menu driven' "
            da.Fill(ds, "Document")

            If ds.Tables("Document").Rows.Count > 0 Then
                ddlpl2.DataSource = ds.Tables("Document")
                ddlpl2.DataTextField = "formname"
                ddlpl2.DataValueField = "formname"
                ddlpl2.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl2.Items.Clear()
                Exit Sub
            End If

        ElseIf ddlpt2.SelectedItem.Text = "Report" Then
            da.SelectCommand.CommandText = "Select reportid,reportname from mmm_mst_report where eid=" & Session("EID") & " "
            da.Fill(ds, "Report")

            If ds.Tables("Report").Rows.Count > 0 Then
                ddlpl2.DataSource = ds.Tables("Report")
                ddlpl2.DataTextField = "ReportName"
                ddlpl2.DataValueField = "ReportName"
                ddlpl2.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl2.Items.Clear()
                Exit Sub
            End If


        ElseIf ddlpt2.SelectedItem.Text = "Static" Then
            'da.SelectCommand.CommandText = "Select mid,Menuname,pagelink from mmm_mst_menu where eid=" & Session("EID") & " and pagelink<>'menu' and MTYPE='STATIC' order by menuname"
            da.SelectCommand.CommandText = "select pagelink,pagedesc from mmm_mst_static_pages WHERE isActive=1 ORDER BY PAGEDESC"
            da.Fill(ds, "Static")
            If ds.Tables("Static").Rows.Count > 0 Then
                ddlpl2.DataTextField = "pagedesc"
                ddlpl2.DataValueField = "pagelink"
                ddlpl2.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl2.Items.Clear()
                Exit Sub
            End If

        ElseIf ddlpt2.SelectedItem.Text = "Calendar" Then
            da.SelectCommand.CommandText = "Select formid,formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype in ('Document','master') and formsource='menu driven' and iscalendar=1 order by formname"
            da.Fill(ds, "Calender")
            If ds.Tables("Calender").Rows.Count > 0 Then
                ddlpl2.DataSource = ds.Tables("Calender")
                ddlpl2.DataTextField = "formname"
                ddlpl2.DataValueField = "formname"
                ddlpl2.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl2.Items.Clear()
                Exit Sub
            End If

        ElseIf ddlpt2.SelectedItem.Text = "Master Calendar" Then
            da.SelectCommand.CommandText = "Select tid,name from mmm_mst_calendar  where eid=" & Session("EID") & " order by name"
            da.Fill(ds, "masterCalender")
            If ds.Tables("masterCalender").Rows.Count > 0 Then
                ddlpl2.DataSource = ds.Tables("masterCalender")
                ddlpl2.DataTextField = "name"
                ddlpl2.DataValueField = "name"
                ddlpl2.DataBind()
            Else
                lblmsg.Text = "* No Page Found for this Page Link Type!!"
                ddlpl2.Items.Clear()
                Exit Sub
            End If
        End If
    End Sub

    Private Sub UploadIconedit()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            If fp3.HasFile Then
                Dim filename As String = Path.GetFileName(fp3.FileName)
                ViewState("FILENAME") = filename
                Dim file As HttpPostedFile = fp3.PostedFile
                Dim fileExtension As String = System.IO.Path.GetExtension(filename)
                Dim fileMimeType As String = fp3.PostedFile.ContentType
                Dim fileLengthInKB As Integer = fp3.PostedFile.ContentLength / 20
                Dim getpath As String = [String].Format("~/Images/{0}", "m" & filename)
                img3.ImageUrl = getpath
                Dim matchExtension As String() = {".png"}
                Dim matchMimeType As String() = {"image/x-png", "image/png"}
                Dim img As System.Drawing.Image = System.Drawing.Image.FromStream(fp3.PostedFile.InputStream)
                Dim imgHeight As Single = img.PhysicalDimension.Height
                Dim imgWidth As Single = img.PhysicalDimension.Width
                Dim input As String = Request.Url.AbsoluteUri
                Dim output As String = input.Substring(input.IndexOf("="c) + 1)
                Dim stream As Stream = fp3.PostedFile.InputStream
                Dim sourceImage As New System.Drawing.Bitmap(stream)
                Dim maxImageWidth As Integer = 16
                Dim newImageHeight As Integer = 16
                Dim resizedImage As New System.Drawing.Bitmap(maxImageWidth, newImageHeight)
                Dim gr As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(resizedImage)
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                gr.DrawImage(sourceImage, 0, 0, maxImageWidth, newImageHeight)

                If matchExtension.Contains(fileExtension) AndAlso matchMimeType.Contains(fileMimeType) Then
                    If fileLengthInKB <= 20480 Then
                        If imgHeight > 80 Or imgWidth > 80 Then
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('This image is too large.max height is 80 pixels & max width is 80 pixels.!!!');window.location='MenuMaster.aspx';", True)
                        End If
                        'saving this image for icon display in menu
                        resizedImage.Save(Server.MapPath((Convert.ToString("Images/") & filename)))
                        'saving this image as the original size for mobile
                        fp3.PostedFile.SaveAs(Server.MapPath("Images/") & "m" & filename)
                    Else
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Image size can not be exceed 20kb!!!');window.location='MenuMaster.aspx';", True)

                    End If
                Else
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* File type must be .png!!!');window.location='MenuMaster.aspx';", True)

                End If
            Else
                ViewState("FILENAME") = ""
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* " & ex.Message & "!!!');window.location='MenuMaster.aspx';", True)
            'lblmsg.Text = "* " & ex.Message

        End Try

    End Sub

    Private Sub UploadIconsubmenu()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            If fp2.HasFile Then
                Dim filename As String = Path.GetFileName(fp2.FileName)
                ViewState("FILENAME") = filename
                Dim file As HttpPostedFile = fp2.PostedFile
                Dim fileExtension As String = System.IO.Path.GetExtension(filename)
                Dim fileMimeType As String = fp2.PostedFile.ContentType
                Dim fileLengthInKB As Integer = fp2.PostedFile.ContentLength / 20
                Dim getpath As String = [String].Format("~/Images/{0}", "m" & filename)
                img2.ImageUrl = getpath
                Dim matchExtension As String() = {".png"}
                Dim matchMimeType As String() = {"image/x-png", "image/png"}
                Dim img As System.Drawing.Image = System.Drawing.Image.FromStream(fp2.PostedFile.InputStream)
                Dim imgHeight As Single = img.PhysicalDimension.Height
                Dim imgWidth As Single = img.PhysicalDimension.Width
                Dim input As String = Request.Url.AbsoluteUri
                Dim output As String = input.Substring(input.IndexOf("="c) + 1)
                Dim stream As Stream = fp2.PostedFile.InputStream
                Dim sourceImage As New System.Drawing.Bitmap(stream)
                Dim maxImageWidth As Integer = 16
                Dim newImageHeight As Integer = 16
                Dim resizedImage As New System.Drawing.Bitmap(maxImageWidth, newImageHeight)
                Dim gr As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(resizedImage)
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                gr.DrawImage(sourceImage, 0, 0, maxImageWidth, newImageHeight)

                If matchExtension.Contains(fileExtension) AndAlso matchMimeType.Contains(fileMimeType) Then
                    If fileLengthInKB <= 20480 Then
                        If imgHeight > 80 Or imgWidth > 80 Then
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('This image is too large.max height is 80 pixels & max width is 80 pixels.!!!');window.location='MenuMaster.aspx';", True)
                        End If
                        'saving this image for icon display in menu
                        resizedImage.Save(Server.MapPath((Convert.ToString("Images/") & filename)))
                        'saving this image as the original size for mobile
                        fp3.PostedFile.SaveAs(Server.MapPath("Images/") & "m" & filename)
                    Else
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* Image size can not be exceed 20kb!!!');window.location='MenuMaster.aspx';", True)
                    End If
                Else
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* File type must be .png!!!');window.location='MenuMaster.aspx';", True)
                End If
            Else
                ViewState("FILENAME") = ""
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('* " & ex.Message & "!!!');window.location='MenuMaster.aspx';", True)

        End Try
    End Sub

    Protected Sub ddlchngp_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlchngp.SelectedIndexChanged
        lbl3.Text = ddlchngp.SelectedItem.Text
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select mid from mmm_mst_menu where eid=" & Session("EID") & " and menuname='" & ddlchngp.SelectedItem.Text & "'", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        ViewState("new") = da.SelectCommand.ExecuteScalar()
        con.Close()
        da.Dispose()
    End Sub

    Protected Sub ddlpl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlpl1.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select formcaption from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & ddlpl1.SelectedItem.Text & "'", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        txtchild1.Text = da.SelectCommand.ExecuteScalar()
        con.Close()
        da.Dispose()

    End Sub

    Protected Sub ddlpl2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlpl2.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select formcaption from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & ddlpl2.SelectedItem.Text & "'", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        txtchild2.Text = da.SelectCommand.ExecuteScalar()
        con.Close()
        da.Dispose()
    End Sub
End Class
