Imports System.Data
Imports System.Data.SqlClient
Imports System.Net


Partial Class FormulaDesigner
    Inherits System.Web.UI.Page


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select formname,formid from mmm_mst_forms where eid=" & Session("EID") & " order by formname", con)
            Dim ds As New DataSet

            Session("datat") = Nothing
            If Request.QueryString("Tid") Is Nothing Then
                Response.Redirect("FormulaDesignerConfiguration.aspx")
            End If
            Dim screen As String = Request.QueryString("TID").ToString()

            If screen <> "0" Then

                Dim dt As New DataTable()
                Dim dss As New DataSet()
                Dim dc As New DataColumn("Source Type")
                Dim dc1 As New DataColumn("Source Name")
                Dim dc4 As New DataColumn("Target Name")
                Dim dc2 As New DataColumn("Target Type")
                Dim dc3 As New DataColumn("Source Field")

                Dim dc5 As New DataColumn("Target Field")
                Dim dc6 As New DataColumn("Sorting Field")

                dt.Columns.Add(dc)
                dt.Columns.Add(dc1)
                dt.Columns.Add(dc2)
                dt.Columns.Add(dc4)
                dt.Columns.Add(dc3)

                dt.Columns.Add(dc5)
                dt.Columns.Add(dc6)
                dss.Tables.Add(dt)
                Session("datat") = dt
                ViewState("datat") = dt
                bindonedit(Session("EID"), screen)
            Else
                da.Fill(ds, "forms")

                If ds.Tables("forms").Rows.Count > 0 Then
                    ddldoctype.DataSource = ds.Tables("Forms")
                    ddldoctype.DataTextField = "FormName"
                    ddldoctype.DataValueField = "Formid"
                    ddldoctype.DataBind()
                    ddldoctype.Items.Insert(0, "SELECT")
                End If
                Dim dt As New DataTable()
                Dim dss As New DataSet()
                Dim dc As New DataColumn("Source Type")
                Dim dc1 As New DataColumn("Source Name")
                Dim dc4 As New DataColumn("Target Name")
                Dim dc2 As New DataColumn("Target Type")
                Dim dc3 As New DataColumn("Source Field")

                Dim dc5 As New DataColumn("Target Field")
                Dim dc6 As New DataColumn("Sorting Field")

                dt.Columns.Add(dc)
                dt.Columns.Add(dc1)
                dt.Columns.Add(dc2)
                dt.Columns.Add(dc4)
                dt.Columns.Add(dc3)

                dt.Columns.Add(dc5)
                dt.Columns.Add(dc6)
                dss.Tables.Add(dt)
                Session("datat") = dt
                ViewState("datat") = dt
            End If


            'bind existing formulas to dropdown

            da.SelectCommand.CommandText = "Select formulaname,condition from mmm_mst_formula where eid=" & Session("EID") & ""
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "eform")
            If ds.Tables("eform").Rows.Count > 0 Then
                For J As Integer = 0 To ds.Tables("eform").Rows.Count - 1
                    ddlformulaes.Items.Add(ds.Tables("eform").Rows(J).Item(0).ToString())
                    ddlformulaes.Items(J).Value = ds.Tables("eform").Rows(J).Item(1).ToString()
                    ddlformulaes.Attributes.Add("onchange", "javascript:setCaret('" & ds.Tables("eform").Rows(J).Item(1).ToString() & "')")
                Next
                'ddlformulaes.DataSource = ds.Tables("eform")
                'ddlformulaes.DataTextField = "formulaname"
                'ddlformulaes.DataValueField = "condition"
                'ddlformulaes.DataBind()
                ddlformulaes.Items.Insert(0, "Select")
                ddlformulaes.Visible = True


            End If


        End If
    End Sub


    Protected Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click

        lblMsg.Text = String.Empty
        If Request.QueryString("TID") Is Nothing Then
            Response.Redirect("FormulaDesignerConfiguration.aspx")
        End If
        Dim tid As String = Request.QueryString("TID").ToString()
        If tid <> "0" Then
            txtFormulaName.Enabled = False
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
        Dim con As New SqlConnection(conStr)



        Try
            If Trim(txtFormulaName.Text) = String.Empty Or Trim(txtFormulaName.Text).Length < 3 Then
                txtFormulaName.BorderWidth = 2
                txtFormulaName.BorderColor = Drawing.Color.Red
                txtFormulaName.BorderStyle = BorderStyle.Solid
                lblMsg.Text = "*Please write valid Formula Name!!!"
                Exit Sub
            Else
                'txtRuleName.BorderStyle = BorderStyle.None
                txtFormulaName.BorderColor = Drawing.Color.Empty


            End If

            If ddlformsource.SelectedItem.Text.ToUpper.Trim.ToString() = "SELECT" Then
                ddlformsource.BorderWidth = 2
                ddlformsource.BorderStyle = BorderStyle.Solid
                ddlformsource.BorderColor = Drawing.Color.Red
                lblMsg.Text = "*Please Select Valid Form Source!!!"
                Exit Sub
            Else
                ddlformsource.BorderColor = Drawing.Color.Empty

            End If

            If ddldoctype.SelectedItem.Text.ToUpper.Trim.ToString() = "SELECt" Then
                ddldoctype.BorderWidth = 2
                ddldoctype.BorderStyle = BorderStyle.Solid
                ddldoctype.BorderColor = Drawing.Color.Red
                lblMsg.Text = "*Please Select Valid Document Type!!!"
                Exit Sub
            Else
                ddldoctype.BorderColor = Drawing.Color.Empty
            End If

            If txtcondition.Text.Trim.ToString = String.Empty Then
                txtcondition.BorderWidth = 2
                txtcondition.BorderStyle = BorderStyle.Solid
                txtcondition.BorderColor = Drawing.Color.Red
                lblMsg.Text = "* Condition Can not be blank!!!"
                Exit Sub
            Else
                txtcondition.BorderColor = Drawing.Color.Empty
            End If






            Dim chk As Integer = 0
            If chkisactive.Checked = True Then
                chk = 1
            Else
                chk = 0
            End If
            Dim eid As Integer = Convert.ToInt32(Session("EID"))
            Dim uid As Integer = Convert.ToInt32(Session("UID"))

            Dim obj As New FormulaDesign()

            Dim res As String = obj.FormulaDesign(eid, Trim(txtFormulaName.Text).ToString(), Trim(chk), Trim(txtdescription.Text).ToString(), Trim(txtformulacategory.Text).ToString(), ddlformsource.SelectedItem.Text.Trim.ToString(), ddldoctype.SelectedItem.Text.Trim.ToString(), Trim(txtcondition.Text).ToString(), uid, tid.ToString())

            If res.Contains("CREATED SUCCESSFULLY") Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Formula Created Successfully!!');window.location='FormulaDesignerConfiguration.aspx';", True)
            ElseIf res = "UPDATED SUCCESSFULLY" Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Formula Updated Successfully!!');window.location='FormulaDesignerConfiguration.aspx';", True)
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('" & res.ToString() & "');", True)
                lblMsg.Text = res.ToString()
            End If
        Catch ex As Exception
            lblMsg.Text = ex.Message
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
        End Try
    End Sub


    'Protected Sub ddlfunctionCat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfunctionCat.SelectedIndexChanged
    '    Try
    '        btnfflds.Visible = True
    '        If UCase(ddlfunctionCat.SelectedItem.Text) = "DATE & TIME" Then

    '            ddlfunctionFields.Items.Clear()
    '            'ddlfunctionFields.Items.Insert(0, "SELECT")
    '            ddlfunctionFields.Items.Insert(0, New ListItem("Select", "0"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("DAY", "DAY[datetime]"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("DATE", "DATE[datetime]"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("MONTH", "MONTH[datetime]"))

    '            ddlfunctionFields.Items.Insert(0, New ListItem("PREVIOUS MONTH", "PREVIOUS MONTH[datetime]"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("YEAR", "YEAR[datetime]"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("DATE RANGE", "DATERANGE[datetime,datetime]"))


    '            ddlfunctionFields.DataBind()

    '        ElseIf UCase(ddlfunctionCat.SelectedItem.Text) = "TEXT" Then
    '            ddlfunctionFields.Items.Clear()
    '            ddlfunctionFields.Items.Insert(0, New ListItem("SELECT", "0"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("CONCATENATE", "CONCATENATE[fld1,fld2,text,spl charcter,fld 15,…….,n]"))

    '            ddlfunctionFields.DataBind()

    '        ElseIf UCase(ddlfunctionCat.SelectedItem.Text) = "LOGICAL" Then
    '            ddlfunctionFields.Items.Clear()


    '            ddlfunctionFields.Items.Insert(0, New ListItem("Select", "0"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("AND", "&&"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("NOT", "!="))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("OR", "||"))


    '            ddlfunctionFields.DataBind()
    '        ElseIf UCase(ddlfunctionCat.SelectedItem.Text) = "MATH" Then
    '            ddlfunctionFields.Items.Clear()

    '            ddlfunctionFields.Items.Insert(0, New ListItem("Select", "0"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("MAX", "MAX[comma separated values]"))
    '            ddlfunctionFields.Items.Insert(0, New ListItem("MIN", "MIN[comma separated values]"))
    '            ddlfunctionFields.DataBind()
    '        End If

    '    Catch ex As Exception

    '    End Try
    'End Sub

    

    Protected Sub ddlformsource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlformsource.SelectedIndexChanged
        lblMsg.Text = String.Empty
        If UCase(ddlformsource.SelectedItem.Text) = "MASTER" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='master' order by formname"
                da.Fill(ds, "master")

                If ds.Tables("master").Rows.Count > 0 Then
                    ddldoctype.DataSource = ds.Tables("master")
                    ddldoctype.DataTextField = "formname"
                    ddldoctype.DataValueField = "formid"
                    ddldoctype.DataBind()
                    ddldoctype.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try

        ElseIf UCase(ddlformsource.SelectedItem.Text) = "DOCUMENT" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='DOCUMENT' and formsource='menu driven' order by formname"
                da.Fill(ds, "document")

                If ds.Tables("document").Rows.Count > 0 Then
                    ddldoctype.DataSource = ds.Tables("document")
                    ddldoctype.DataTextField = "formname"
                    ddldoctype.DataValueField = "formid"
                    ddldoctype.DataBind()
                    ddldoctype.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
        ElseIf UCase(ddlformsource.SelectedItem.Text) = "ACTION DRIVEN" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='ACTION DRIVEN' order by formname"
                da.Fill(ds, "actiondriven")

                If ds.Tables("actiondriven").Rows.Count > 0 Then
                    ddldoctype.DataSource = ds.Tables("actiondriven")
                    ddldoctype.DataTextField = "formname"
                    ddldoctype.DataValueField = "formid"
                    ddldoctype.DataBind()
                    ddldoctype.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
        ElseIf UCase(ddlformsource.SelectedItem.Text) = "DETAIL FORM" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='DETAIL FORM ' order by formname"
                da.Fill(ds, "detailform")

                If ds.Tables("detailform").Rows.Count > 0 Then
                    ddldoctype.DataSource = ds.Tables("detailform")
                    ddldoctype.DataTextField = "formname"
                    ddldoctype.DataValueField = "formid"
                    ddldoctype.DataBind()
                    ddldoctype.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
        Else

            lblMsg.Text = "Please Select Form Source!!"
            ddldoctype.Items.Clear()
            Exit Sub
        End If
    End Sub


    Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldoctype.SelectedIndexChanged
        divrow.Visible = True
        lblMsg.Text = String.Empty
        'getfields()
        Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
        


    End Sub

    
    Private Sub AddChildItems(ByVal dropdown As String, ByVal parent As MenuItem)
        If dropdown.Contains("-") Then
            Dim str As String() = dropdown.ToString.Split("-")
            If str.Length > 1 Then
                If ViewState("cnt") < 2 Then


                    Dim ddtype As String = str(1).ToString
                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim con As New SqlConnection(conStr)
                    Dim da As New SqlDataAdapter("", con)
                    Dim ds As New DataSet
                    Try
                        da.SelectCommand.CommandText = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddtype.ToString().Trim() & "' order by displayname"
                        da.Fill(ds, "child")
                        If ds.Tables("child").Rows.Count > 0 Then

                            Dim view As New DataView(ds.Tables("child"))
                            For Each row As DataRowView In view
                                Dim childitem As New MenuItem(row("displayname").ToString(), row("fieldmapping").ToString(), row("dropdowntype").ToString, row("dropdown").ToString)
                                If UCase(row("dropdowntype")).ToString = "MASTER VALUED" Then
                                    parent.ChildItems.Add(childitem)
                                    If UCase(str(1)).ToString = "USER" Then
                                        ViewState("cnt") = ViewState("cnt") + 1
                                    End If
                                    AddChildItems(row("dropdown").ToString, childitem)
                                Else
                                    parent.ChildItems.Add(childitem)
                                    'AddChildItems(row("dropdown").ToString, childitem)
                                End If

                            Next
                        End If
                    Catch ex As Exception
                        lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
                    Finally
                        If Not con Is Nothing Then
                            con.Close()
                        End If
                        If Not da Is Nothing Then
                            da.Dispose()
                        End If

                    End Try

                End If
            End If
        End If


    End Sub

    Private Sub LoadWorkGroupTree(ByVal doctype As String)



        ViewState("cnt") = 0
        Dim conStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim cmd As SqlCommand = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim table As New DataTable()
            con = New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim sql As String = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & doctype.ToString().Trim() & "' order by displayname"
            cmd = New SqlCommand(sql, con)
            da = New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds, "table")

            Dim masterNode As New TreeNode(doctype.Trim.ToString())

            masterNode.Value = doctype.Trim.ToString()
            masterNode.ImageUrl = "images/Redp.png"
            masterNode.SelectAction = TreeNodeSelectAction.Expand
            'masterNode.NavigateUrl = "javascript:setCaret('" & masterNode.Text.ToString() & "')"
            masterNode.NavigateUrl = "javascript:setCaret('" & masterNode.Text.ToString() & "')"
            tv.Nodes.Add(masterNode)


            'tv.Attributes.Add("onclick", "return setCaret(" & masterNode.Text.ToString() & ");")
            Dim view As New DataView(ds.Tables("table"))
            For Each row As DataRowView In view
                ViewState("cnt") = 0
                If UCase(row.Item("dropdowntype").ToString) = "MASTER VALUED" Then
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "+"
                    n.ImageUrl = "images/redp.png"
                    n.NavigateUrl = "javascript:setCaret('" & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)
                    'tv.Attributes.Add("onclick", "return setCaret(" & n.Text.ToString() & ");")
                    LoadDocTree(row.Item("dropdown").ToString, n)
                Else
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "images/bluep.png"
                    n.NavigateUrl = "javascript:setCaret('" & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)
                    ' tv.Attributes.Add("onclick", "return setCaret(" & n.Text.ToString() & ");")
                End If

            Next

        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not cmd Is Nothing Then
                cmd.Dispose()
            End If

        End Try
    End Sub


    

    Private Sub LoadDocTree(ByVal dropdown As String, ByRef node As TreeNode)
        If dropdown.Contains("-") Then
            Dim str As String() = dropdown.ToString.Split("-")
            If str.Length > 1 Then
                If ViewState("cnt") < 2 Then


                    Dim ddtype As String = str(1).ToString
                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim con As New SqlConnection(conStr)
                    Dim da As New SqlDataAdapter("", con)
                    Dim ds As New DataSet
                    Try
                        da.SelectCommand.CommandText = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddtype.ToString().Trim() & "' order by displayname"
                        da.Fill(ds, "child")
                        If ds.Tables("child").Rows.Count > 0 Then

                            Dim view As New DataView(ds.Tables("child"))
                            For Each row As DataRowView In view
                                Dim n As New TreeNode()
                                If UCase(row("dropdowntype")).ToString = "MASTER VALUED" Then
                                    n.Text = row("displayname").ToString()
                                    n.Value = row("fieldmapping").ToString()
                                    n.ImageUrl = "images/redp.png"

                                    n.NavigateUrl = "javascript:setCaret('" & node.Text.ToString() & "." & n.Text.ToString() & "')"

                                    node.ChildNodes.Add(n)
                                    If UCase(str(1)).ToString = "USER" Then
                                        ViewState("cnt") = ViewState("cnt") + 1
                                    End If
                                    LoadDocTree(row.Item("dropdown").ToString, n)
                                Else
                                    n.Text = row("displayname").ToString()
                                    n.Value = row("fieldmapping").ToString()
                                    n.ImageUrl = "images/bluep.png"
                                    n.NavigateUrl = "javascript:setCaret('" & node.Text.ToString() & "." & n.Text.ToString() & "')"
                                    node.ChildNodes.Add(n)
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
                    Finally
                        If Not con Is Nothing Then
                            con.Close()
                        End If
                        If Not da Is Nothing Then
                            da.Dispose()
                        End If

                    End Try

                End If
            End If
        End If

    End Sub

    Protected Sub tv_TreeNodeCheckChanged(sender As Object, e As TreeNodeEventArgs) Handles tv.TreeNodeCheckChanged

    End Sub

    Protected Sub tv_TreeNodeExpanded(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.TreeNodeEventArgs) Handles tv.TreeNodeExpanded
        If e.Node.Parent Is Nothing Then
            Return
        End If
        Dim strNodeValue As String = e.Node.Value
        For Each node As TreeNode In e.Node.Parent.ChildNodes
            If node.Value <> strNodeValue Then
                node.Collapse()
            End If
        Next
    End Sub



    Protected Sub tv_SelectedNodeChanged(sender As Object, e As EventArgs) Handles tv.SelectedNodeChanged
        

    End Sub



    'Protected Sub btnfflds_Click(sender As Object, e As EventArgs) Handles btnfflds.Click
    '    Dim ds As String = String.Empty


    'End Sub

    Public Function bindonedit(ByVal eid As Integer, ByVal Tid As Integer) As String
        btnsave.Text = "Update"
        txtFormulaName.Enabled = False
        Dim Result As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim dt As New DataTable()
        Try
            da.SelectCommand.CommandText = "Select * from mmm_mst_Formula where eid=" & eid & " and Tid=" & Tid & " "
            da.Fill(ds, "Edit")
            If ds.Tables("Edit").Rows.Count > 0 Then


                txtFormulaName.Text = ds.Tables("Edit").Rows(0).Item("Formulaname").ToString()
                Dim cks As String = 0
                If ds.Tables("edit").Rows(0).Item("isactive").ToString = 1 Then
                    chkisactive.Checked = True
                Else
                    chkisactive.Checked = False
                End If

                If chkisactive.Checked = True Then
                    cks = 1
                Else
                    cks = 0
                End If

                txtdescription.Text = ds.Tables("edit").Rows(0).Item("formuladesc").ToString()
                ddlformsource.Items.Clear()

                ddlformsource.Items.Insert(0, "SELECT")
                ddlformsource.Items.Insert(1, "DOCUMENT")
                ddlformsource.Items.Insert(2, "MASTER")
                ddlformsource.Items.Insert(3, "DETAIL FORM")
                ddlformsource.Items.Insert(4, "ACTION DRIVEN")
                ddlformsource.DataBind()

                ddlformsource.SelectedIndex = ddlformsource.Items.IndexOf(ddlformsource.Items.FindByText(ds.Tables("edit").Rows(0).Item("formsource").ToString()))


                If UCase(ddlformsource.SelectedItem.Text) = "MASTER" Then
                    ddldoctype.Items.Clear()

                    da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='master' order by formname"
                    da.Fill(ds, "master")

                    If ds.Tables("master").Rows.Count > 0 Then
                        ddldoctype.DataSource = ds.Tables("master")
                        ddldoctype.DataTextField = "formname"
                        ddldoctype.DataValueField = "formid"
                        ddldoctype.DataBind()
                        ddldoctype.Items.Insert(0, "SELECT")
                        ddldoctype.SelectedIndex = ddldoctype.Items.IndexOf(ddldoctype.Items.FindByText(Trim(ds.Tables("edit").Rows(0).Item("documenttype")).ToString()))

                    End If

                ElseIf UCase(ddlformsource.SelectedItem.Text) = "DOCUMENT" Then

                    ddldoctype.Items.Clear()
                    da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='DOCUMENT' and formsource='menu driven' order by formname"
                    da.Fill(ds, "document")

                    If ds.Tables("document").Rows.Count > 0 Then
                        ddldoctype.DataSource = ds.Tables("document")
                        ddldoctype.DataTextField = "formname"
                        ddldoctype.DataValueField = "formid"
                        ddldoctype.DataBind()
                        ddldoctype.Items.Insert(0, "SELECT")
                        ddldoctype.SelectedIndex = ddldoctype.Items.IndexOf(ddldoctype.Items.FindByText(Trim(ds.Tables("edit").Rows(0).Item("documenttype")).ToString()))
                    End If

                ElseIf UCase(ddlformsource.SelectedItem.Text) = "ACTION DRIVEN" Then

                    ddldoctype.Items.Clear()

                    da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='ACTION DRIVEN' order by formname"
                    da.Fill(ds, "actiondriven")

                    If ds.Tables("actiondriven").Rows.Count > 0 Then
                        ddldoctype.DataSource = ds.Tables("actiondriven")
                        ddldoctype.DataTextField = "formname"
                        ddldoctype.DataValueField = "formid"
                        ddldoctype.DataBind()
                        ddldoctype.Items.Insert(0, "SELECT")
                        ddldoctype.SelectedIndex = ddldoctype.Items.IndexOf(ddldoctype.Items.FindByText(Trim(ds.Tables("edit").Rows(0).Item("documenttype")).ToString()))
                    End If

                ElseIf UCase(ddlformsource.SelectedItem.Text) = "DETAIL FORM" Then
                    ddldoctype.Items.Clear()

                    da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='DETAIL FORM ' order by formname"
                    da.Fill(ds, "detailform")

                    If ds.Tables("detailform").Rows.Count > 0 Then
                        ddldoctype.DataSource = ds.Tables("detailform")
                        ddldoctype.DataTextField = "formname"
                        ddldoctype.DataValueField = "formid"
                        ddldoctype.DataBind()
                        ddldoctype.Items.Insert(0, "SELECT")
                        ddldoctype.SelectedIndex = ddldoctype.Items.IndexOf(ddldoctype.Items.FindByText(Trim(ds.Tables("edit").Rows(0).Item("documenttype")).ToString()))
                    End If

                End If
                Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                txtcondition.Text = ds.Tables("edit").Rows(0).Item("condition").ToString()
                txtformulacategory.Text = ds.Tables("edit").Rows(0).Item("condition").ToString()
                divrow.Visible = True
            Else
                Response.Redirect("FormulaDesigner.aspx")
            End If
        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try




        Return Result

    End Function

    
    'Protected Sub ddlfunctionFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfunctionFields.SelectedIndexChanged
    '    If UCase(ddlfunctionFields.SelectedItem.Text) = "DAY" Then
    '        lblexampl.Text = "Format: DAY[DateTime], Returns day e.g. Day[dd/mm/yy] result Thursday "
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "DATE" Then
    '        lblexampl.Text = "Format: DATE[datetime], Returns date e.g. Date[dd/mm/yy] result dd "
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "MONTH" Then
    '        lblexampl.Text = "Format: MONTH[datetime], Returns month e.g. Month[dd/mm/yy] result month "
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "PREVIOUS MONTH" Then
    '        lblexampl.Text = "Format: PREVIOUS MONTH[datetime], Returns Previous Month e.g. Previous Month[4-Sep-14] result August or 8"
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "YEAR" Then
    '        lblexampl.Text = "Format: YEAR[datetime], Returns year e.g. Year[4-Sep-14] result 2014"
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "DATE RANGE" Then
    '        lblexampl.Text = "Format: DATERANGE[datetime,datetime], Returns date range e.g. DateRange[4-Sep-14,5-Sep-14]"
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "CONCATENATE" Then
    '        lblexampl.Text = "Format: CONCATENATE[fld1,fld2,text,spl charcter,fld 15,…….,n]"
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "AND" Then
    '        lblexampl.Text = "Format: AND or && "
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "NOT" Then
    '        lblexampl.Text = "Format: Not or != "
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "OR" Then
    '        lblexampl.Text = "Format: OR or ||"
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "MAX" Then
    '        lblexampl.Text = "Format: Max[fld1,fld2,...,n], Returns the max value e.g Max[30,12] gives 30"
    '    ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "MIN" Then
    '        lblexampl.Text = "Format: Min[fld1,fld2,....,n]"

    '    End If


    'End Sub

End Class
