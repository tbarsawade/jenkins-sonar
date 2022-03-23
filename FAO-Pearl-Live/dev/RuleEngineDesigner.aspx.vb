Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Globalization
Imports System.Net
Partial Class RuleEngineDesigner
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        'If Not Session("sd") Is Nothing Then
        '    Dim ab As String() = Session("sd").ToString.Split(",")
        '    For i As Integer = 0 To ab.Length - 1
        '        CreateTreeView(ab(i).ToString())[
        '    Next
        'End If

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


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select formname,formid from mmm_mst_forms where eid=" & Session("EID") & " order by formname", con)
            Dim ds As New DataSet

            Session("datat") = Nothing
            If Request.QueryString("RuleID") Is Nothing Then
                Response.Redirect("RuleEngineConfiguration.aspx")
            End If
            Dim screen As String = Request.QueryString("RuleID").ToString()
            tv.Attributes.Add("onclick", "return insertAtCursorTree(" & txtcondition.ClientID & ");")

            tvsource.Attributes.Add("onclick", "return insertAtCursorTreeSource(" & txtcondition.ClientID & ");")
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
                ' ViewState("datat") = dt
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
                '  ViewState("datat") = dt
            End If




        End If
    End Sub


    Protected Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click

        lblMsg.Text = String.Empty
        If Request.QueryString("RuleID") Is Nothing Then
            Response.Redirect("RuleEngineConfiguration.aspx")
        End If
        Dim rid As String = Request.QueryString("RuleID").ToString()
        If rid <> "0" Then
            txtRuleName.Enabled = False
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
        Dim con As New SqlConnection(conStr)

        Try
            Dim TargetField As String = ""
            Dim selectedValue As String = ""
            Dim Controlfield As String = ddlcontrolfield.SelectedValue.ToString()
            If Not Session("valControl") Is Nothing Then
                'TargetField = Session("valControl").ToString()
                If ddltypeofrun.SelectedItem.Text.ToUpper = "CONTROL" Then
                    For Each item As ListItem In chktargetfields.Items
                        If item.Selected Then
                            selectedValue = selectedValue & item.Value & ","
                        End If
                    Next
                End If
                If Not selectedValue.ToUpper = "" Then
                    selectedValue = selectedValue.Substring(0, selectedValue.Length - 1)
                End If
                TargetField = selectedValue
            Else

            End If


            If Trim(txtRuleName.Text) = String.Empty Or Trim(txtRuleName.Text).Length < 3 Then
                txtRuleName.BorderWidth = 2
                txtRuleName.BorderColor = Drawing.Color.Red
                txtRuleName.BorderStyle = BorderStyle.Solid
                lblMsg.Text = "*Please write valid Rule Name!!!"
                Exit Sub
            Else
                'txtRuleName.BorderStyle = BorderStyle.None
                txtRuleName.BorderColor = Drawing.Color.Empty


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

            If ddldoctype.SelectedItem.Text.ToUpper.Trim.ToString() = "SELECT" Then
                ddldoctype.BorderWidth = 2
                ddldoctype.BorderStyle = BorderStyle.Solid
                ddldoctype.BorderColor = Drawing.Color.Red
                lblMsg.Text = "*Please Select Valid Document Type!!!"
                Exit Sub
            Else
                ddldoctype.BorderColor = Drawing.Color.Empty
            End If

            If ddldocnature.SelectedItem.Text.ToUpper.Trim.ToString() = "SELECT" Then
                ddldocnature.BorderWidth = 2
                ddldocnature.BorderStyle = BorderStyle.Solid
                ddldocnature.BorderColor = Drawing.Color.Red
                lblMsg.Text = "*Please Select Valid Document Nature!!!"
                Exit Sub
            Else
                ddldocnature.BorderColor = Drawing.Color.Empty
            End If

            If ddltypeofrun.SelectedItem.Text.ToUpper.Trim.ToString() = "SELECT" Then
                ddltypeofrun.BorderWidth = 2
                ddltypeofrun.BorderStyle = BorderStyle.Solid
                ddltypeofrun.BorderColor = Drawing.Color.Red
                lblMsg.Text = "*Please Select Valid Document Nature!!!"
                Exit Sub
            Else
                ddltypeofrun.BorderColor = Drawing.Color.Empty
            End If

            'If ddlactiontype.SelectedItem.Text.ToUpper.Trim.ToString() = "SELECT" Then
            '    ddlactiontype.BorderWidth = 2
            '    ddlactiontype.BorderStyle = BorderStyle.Solid
            '    ddlactiontype.BorderColor = Drawing.Color.Red
            '    lblMsg.Text = "*Please Select Valid Valid Action Type!!!"
            '    Exit Sub
            'Else
            '    ddlactiontype.BorderColor = Drawing.Color.Empty
            'End If


            If txtcondition.Text.Trim.ToString = String.Empty Then
                txtcondition.BorderWidth = 2
                txtcondition.BorderStyle = BorderStyle.Solid
                txtcondition.BorderColor = Drawing.Color.Red
                lblMsg.Text = "* Condition Can not be blank!!!"
                Exit Sub
            Else
                txtcondition.BorderColor = Drawing.Color.Empty
            End If


            'If ddlsuccessaction.SelectedItem.Text = "SELECT" Then
            '    ddlsuccessaction.BorderWidth = 2
            '    ddlsuccessaction.BorderStyle = BorderStyle.Solid
            '    ddlsuccessaction.BorderColor = Drawing.Color.Red
            '    lblMsg.Text = "* Please select success action!!!"
            '    Exit Sub
            'Else
            '    ddlsuccessaction.BorderColor = Drawing.Color.Empty
            'End If

            'If ddlfaction.SelectedItem.Text = "SELECT" Then
            '    ddlfaction.BorderWidth = 2
            '    ddlfaction.BorderStyle = BorderStyle.Solid
            '    ddlfaction.BorderColor = Drawing.Color.Red
            '    lblMsg.Text = "* Please select Failure Action!!!"
            '    Exit Sub
            'Else
            '    ddlfaction.BorderColor = Drawing.Color.Empty
            'End If

            Dim sf As String = ""
            If ddlsf.SelectedItem.Text = "SELECT" Then
                sf = "NONE"
            Else
                sf = ddlsf.SelectedItem.Text.ToString()
            End If
            Dim ff As String = ""
            If ddlfflds.SelectedItem.Text = "SELECT" Then
                ff = "NONE"
            Else
                ff = ddlfflds.SelectedItem.Text.ToString()
            End If

            ' Dim chk As Integer = 0
            'If chkisactive.Checked = True Then
            '    chk = 1
            'Else
            '    chk = 0
            'End If
            Dim eid As Integer = Convert.ToInt32(Session("EID"))
            Dim uid As Integer = Convert.ToInt32(Session("UID"))
            Dim red As New RuleEngineDesign()

            Dim res As String = red.RuleEngine(eid, Trim(txtRuleName.Text).ToString(), Trim(txtdescription.Text).ToString(), ddlformsource.SelectedItem.Text.Trim.ToString(), ddldoctype.SelectedItem.Text.Trim.ToString(), ddldocnature.SelectedItem.Text.Trim.ToString(), ddltypeofrun.SelectedItem.Text.Trim.ToString(), Trim(txtcondition.Text).ToString(), ddlsuccessaction.SelectedItem.Text.Trim.ToString(), txtsuccessmsg.Text.Trim.ToString(), ddlfaction.SelectedItem.Text.ToString(), txterrormessage.Text.Trim.ToString(), uid, sf.ToString(), ff.ToString(), rid.ToString(), Controlfield.ToString(), TargetField.ToString())

            If res.Contains("CREATED SUCCESSFULLY") Then
                Dim a As String() = res.ToString.Split("_")
                Dim da As New SqlDataAdapter("", con)
                Dim DT As New DataTable()
                DT = CType(Session("datat"), DataTable)
                If Not DT Is Nothing Then

                    For i As Integer = 0 To DT.Rows.Count - 1
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "USP_insertRuleRelation"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@ruleid", a(1).ToString())
                        da.SelectCommand.Parameters.AddWithValue("@sourcetype", DT.Rows(i).Item("Source Type").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@sourcename", DT.Rows(i).Item("Source Name").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@s_relationidentifierfiled", DT.Rows(i).Item("Source Field").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@targettype", DT.Rows(i).Item("Target Type").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@targetname", DT.Rows(i).Item("Target Name").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@t_relationidentifierfield", DT.Rows(i).Item("Target Field").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@sortingfield", DT.Rows(i).Item("sorting field").ToString())
                        da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))

                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                    Next
                End If


                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Rule Created Successfully!!');window.location='RuleEngineConfiguration.aspx';", True)
            ElseIf res = "UPDATED SUCCESSFULLY" Then
                Dim da As New SqlDataAdapter("", con)
                Dim DT As New DataTable()
                DT = CType(Session("datat"), DataTable)
                If Not DT Is Nothing Then


                    For Each gvrow As GridViewRow In gvmap.Rows
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "USP_insertRuleRelation"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                        da.SelectCommand.Parameters.AddWithValue("@ruleid", rid)
                        da.SelectCommand.Parameters.AddWithValue("@sourcetype", gvrow.Cells(2).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@sourcename", gvrow.Cells(3).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@s_relationidentifierfiled", gvrow.Cells(6).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@targettype", gvrow.Cells(4).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@targetname", gvrow.Cells(5).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@t_relationidentifierfield", gvrow.Cells(7).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@sortingfield", gvrow.Cells(8).Text.ToString())
                        da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))

                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                    Next
                End If
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Rule Updated Successfully!!');window.location='RuleEngineConfiguration.aspx';", True)

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


    Protected Sub ddlfunctionCat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfunctionCat.SelectedIndexChanged
        Try
            btnfflds.Visible = True
            If UCase(ddlfunctionCat.SelectedItem.Text) = "DATE & TIME" Then

                ddlfunctionFields.Items.Clear()
                'ddlfunctionFields.Items.Insert(0, "SELECT")
                ddlfunctionFields.Items.Insert(0, New ListItem("Select", "0"))
                ddlfunctionFields.Items.Insert(0, New ListItem("DAY", "DAY[datetime]"))
                ddlfunctionFields.Items.Insert(0, New ListItem("DATE", "DATE[datetime]"))
                ddlfunctionFields.Items.Insert(0, New ListItem("MONTH", "MONTH[datetime]"))

                ddlfunctionFields.Items.Insert(0, New ListItem("PREVIOUS MONTH", "PREVIOUS MONTH[datetime]"))
                ddlfunctionFields.Items.Insert(0, New ListItem("YEAR", "YEAR[datetime]"))
                ddlfunctionFields.Items.Insert(0, New ListItem("DATE RANGE", "DATERANGE[datetime,datetime]"))


                ddlfunctionFields.DataBind()

            ElseIf UCase(ddlfunctionCat.SelectedItem.Text) = "TEXT" Then
                ddlfunctionFields.Items.Clear()
                ddlfunctionFields.Items.Insert(0, New ListItem("SELECT", "0"))
                ddlfunctionFields.Items.Insert(0, New ListItem("CONCATENATE", "CONCATENATE[fld1,fld2,text,spl charcter,fld 15,…….,n]"))

                ddlfunctionFields.DataBind()

            ElseIf UCase(ddlfunctionCat.SelectedItem.Text) = "LOGICAL" Then
                ddlfunctionFields.Items.Clear()


                ddlfunctionFields.Items.Insert(0, New ListItem("Select", "0"))
                ddlfunctionFields.Items.Insert(0, New ListItem("AND", "&&"))
                ddlfunctionFields.Items.Insert(0, New ListItem("NOT", "!="))
                ddlfunctionFields.Items.Insert(0, New ListItem("OR", "||"))


                ddlfunctionFields.DataBind()
            ElseIf UCase(ddlfunctionCat.SelectedItem.Text) = "MATH" Then
                ddlfunctionFields.Items.Clear()

                ddlfunctionFields.Items.Insert(0, New ListItem("Select", "0"))
                ddlfunctionFields.Items.Insert(0, New ListItem("MAX", "MAX[comma separated values]"))
                ddlfunctionFields.Items.Insert(0, New ListItem("MIN", "MIN[comma separated values]"))
                ddlfunctionFields.DataBind()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddltypeofrun_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddltypeofrun.SelectedIndexChanged

        divrow2.Visible = True
        divrows.Visible = True


        If UCase(ddltypeofrun.SelectedItem.Text) = "FORM LOAD" Then

            Dim masterNode As New TreeNode(ddldoctype.SelectedItem.Text.Trim.ToString())

            masterNode.Value = ddldoctype.SelectedItem.Text.Trim.ToString()
            masterNode.ImageUrl = "images/Redp.png"
            masterNode.SelectAction = TreeNodeSelectAction.Expand
            tv.Nodes.Add(masterNode)
            tv.Nodes.Clear()
            'Dim list As New List(Of String)
            Dim treeNode As New TreeNode("UID")
            treeNode.ImageUrl = "images/bluep.png"
            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("USERNAME")
            treeNode.ImageUrl = "images/bluep.png"
            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("USERROLE")
            treeNode.ImageUrl = "images/bluep.png"
            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("CODE")
            treeNode.ImageUrl = "images/bluep.png"
            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("USERIMAGE")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("CLOGO")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("EID")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("ISLOCAL")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("IPADDRESS")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("MACADDRESS")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("INTIME")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("EMAIL")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("LID")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("HEADERSTRIP")
            treeNode.ImageUrl = "images/bluep.png"

            tv.Nodes.Add(treeNode)
            treeNode = New TreeNode("ROLES")
            tv.Nodes.Add(treeNode)


            treeNode.ImageUrl = "images/bluep.png"

            tv.DataBind()







            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As New SqlConnection(conStr)
            'Dim da As New SqlDataAdapter("", con)
            'Dim ds As New DataSet
            'Try

            '    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & Session("EID") & " and dropdowntype='SESSION VALUED' and documenttype='" & ddldoctype.SelectedItem.Text.Trim.ToString() & "'"
            '    da.Fill(ds, "flds")
            '    If ds.Tables("flds").Rows.Count > 0 Then
            '        ddlsuccessaction.Items.Clear()
            '        ddlsuccessaction.DataSource = ds.Tables("flds")
            '        ddlsuccessaction.DataTextField = "displayname"
            '        ddlsuccessaction.DataValueField = "fieldmapping"
            '        ddlsuccessaction.DataBind()


            '    End If
            '    ddlsuccessaction.Items.Insert(0, "SELECT")
            'Catch ex As Exception
            '    lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            'Finally
            '    If Not con Is Nothing Then
            '        con.Close()
            '    End If
            '    If Not da Is Nothing Then
            '        da.Dispose()
            '    End If

            'End Try
            'ddlsuccessaction.Items.Clear()
            'ddlsuccessaction.Items.Insert(0, "SELECT")
            'ddlsuccessaction.Items.Insert(1, "ADD")
            'ddlsuccessaction.Items.Insert(2, "SUBTRACT")
            'ddlsuccessaction.Items.Insert(3, "MULTIPLY")
            'ddlsuccessaction.Items.Insert(4, "DIVIDE")
            'ddlsuccessaction.Items.Insert(5, "REPLACE")
            'ddlsuccessaction.Items.Insert(6, "SAVE WITH ALERT")
            'ddlsuccessaction.Items.Insert(7, "ADDITION OF WORKFLOW")
            'ddlsuccessaction.Items.Insert(8, "SAVE")
            'ddlsuccessaction.Items.Insert(9, "DENY")
            'ddlsuccessaction.Items.Insert(10, "CHANGE DROPDOWN COLOR")

            'ddlsuccessaction.DataBind()

        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "SUBMIT" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
            ddlsuccessaction.Items.Clear()

            ddlsuccessaction.Items.Insert(0, "SELECT")
            ddlsuccessaction.Items.Insert(1, "SAVE")
            ddlsuccessaction.Items.Insert(2, "SAVE WITH ALERT")
            ddlsuccessaction.Items.Insert(3, "DENY")
            ddlsuccessaction.Items.Insert(4, "WORKFLOW")
            ddlsuccessaction.Items.Insert(5, "MANDATORY")
            ddlsuccessaction.Items.Insert(6, "NON MANDATORY")
            ddlsuccessaction.Items.Insert(7, "SEND EMAIL")

            ddlsuccessaction.DataBind()

            ddlfaction.Items.Insert(0, "SELECT")
            ddlfaction.Items.Insert(1, "SAVE")
            ddlfaction.Items.Insert(2, "SAVE WITH ALERT")
            ddlfaction.Items.Insert(3, "DENY")
            ddlfaction.Items.Insert(4, "WORKFLOW")
            ddlfaction.Items.Insert(5, "MANDATORY")
            ddlfaction.Items.Insert(6, "NON MANDATORY")
            ddlfaction.Items.Insert(7, "SEND EMAIL")

            ddlfaction.DataBind()
            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString())

        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "CONTROL" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString())
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            ddlsuccessaction.Items.Insert(1, "ENABLE")
            ddlsuccessaction.Items.Insert(2, "DISABLE")
            ddlsuccessaction.Items.Insert(3, "VISIBLE")
            ddlsuccessaction.Items.Insert(4, "INVISIBLE")
            ddlsuccessaction.Items.Insert(5, "CHANGE DROPDOWN COLOR")
            ddlsuccessaction.Items.Insert(6, "NO ACTION")
            ddlsuccessaction.DataBind()

            ddlfaction.Items.Clear()
            ddlfaction.Items.Insert(0, "SELECT")
            ddlfaction.Items.Insert(1, "ENABLE")
            ddlfaction.Items.Insert(2, "DISABLE")
            ddlfaction.Items.Insert(3, "VISIBLE")
            ddlfaction.Items.Insert(4, "INVISIBLE")
            ddlfaction.Items.Insert(5, "CHANGE DROPDOWN COLOR")
            ddlfaction.Items.Insert(6, "ALERT")
            ddlfaction.DataBind()

            ' Below Code is to enable disable the control Field & Target Control Field
            lblcontrolfield.Visible = True
            ddlcontrolfield.Visible = True

            btnopentargetcontrol.Visible = True
            txtboxtargetcontrol.Visible = True

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim dt As DataTable = New DataTable()
            Try
                da.SelectCommand.CommandText = "Select fieldid,displayname from mmm_mst_fields where documenttype='" & ddldoctype.SelectedItem.Text.Trim() & "' and isActive=1 order by displayname"
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    ddlcontrolfield.DataSource = dt
                    ddlcontrolfield.DataTextField = "displayname"
                    ddlcontrolfield.DataValueField = "Fieldid"
                    ddlcontrolfield.DataBind()
                End If
            Catch ex As Exception

            End Try




        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "APPROVE" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString())
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            ddlsuccessaction.Items.Insert(1, "SAVE")
            ddlsuccessaction.Items.Insert(2, "SAVE WITH ALERT")
            ddlsuccessaction.Items.Insert(3, "DENY")
            ddlsuccessaction.Items.Insert(4, "WORKFLOW")
            ddlsuccessaction.Items.Insert(5, "MANDATORY")
            ddlsuccessaction.Items.Insert(6, "NON MANDATORY")
            ddlsuccessaction.Items.Insert(7, "SEND EMAIL")
            ddlsuccessaction.DataBind()

            ddlfaction.Items.Clear()
            ddlfaction.Items.Insert(0, "SELECT")
            ddlfaction.Items.Insert(1, "SAVE")
            ddlfaction.Items.Insert(2, "SAVE WITH ALERT")
            ddlfaction.Items.Insert(3, "DENY")
            ddlfaction.Items.Insert(4, "WORKFLOW")
            ddlfaction.Items.Insert(5, "MANDATORY")
            ddlfaction.Items.Insert(6, "NON MANDATORY")
            ddlfaction.Items.Insert(7, "SEND EMAIL")
            ddlfaction.DataBind()

            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False



        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "REJECT" Then
            '  Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            ddlsuccessaction.Items.Insert(1, "DENY")
            ddlsuccessaction.Items.Insert(2, "SEND EMAIL")
            ddlsuccessaction.DataBind()

            ddlfaction.Items.Clear()
            ddlfaction.Items.Insert(0, "SELECT")
            ddlfaction.Items.Insert(1, "DENY")
            ddlfaction.Items.Insert(2, "SEND EMAIL")
            ddlfaction.DataBind()
            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False
        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "CRM(HOLD)" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False
        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "DRAFT" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False
        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "ACTIVE" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False
        ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "INACTIVE" Then
            ' Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
            ddlsuccessaction.Items.Clear()
            ddlsuccessaction.Items.Insert(0, "SELECT")
            btnopentargetcontrol.Visible = False
            txtboxtargetcontrol.Visible = False

        End If



    End Sub

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
        If UCase(ddldoctype.SelectedItem.Text) = "SELECT" Then
            lblMsg.Text = "Please select valid document type"
            Exit Sub
        End If
        Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try

            da.SelectCommand.CommandText = "select upper(displayname)[formname],upper(fieldmapping)[fieldmapping],fieldid from mmm_mst_fields where eid=" & Session("Eid") & " and documenttype='" & Trim(ddldoctype.SelectedItem.Text) & "' order by displayname"
            da.Fill(ds, "fields")
            If ds.Tables("fields").Rows.Count > 0 Then
                ddlsdf.DataSource = ds.Tables("fields")
                ddlsdf.DataTextField = "formname"
                ddlsdf.DataValueField = "fieldmapping"
                ddlsdf.DataBind()
                ddlsdf.Items.Insert(0, "SELECT")

                'binding checkboxlist target control

                chktargetfields.DataSource = ds.Tables("fields")
                chktargetfields.DataTextField = "Formname"
                chktargetfields.DataValueField = "fieldid"
                chktargetfields.DataBind()
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


    End Sub

    'Private Sub getfields()
    '    menufields.Items.Clear()
    '    Dim conStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = Nothing
    '    Dim cmd As SqlCommand = Nothing
    '    Dim da As SqlDataAdapter = Nothing
    '    Try
    '        Dim table As New DataTable()
    '        con = New SqlConnection(conStr)
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim sql As String = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text.Trim() & "' order by displayname"
    '        cmd = New SqlCommand(sql, con)
    '        da = New SqlDataAdapter(cmd)
    '        da.Fill(table)

    '        'End If

    '        Dim view As New DataView(table)
    '        'view.RowFilter = "Pmenu =0 "
    '        For Each row As DataRowView In view
    '            ViewState("cnt") = 0
    '            Dim menuItem As New MenuItem(row("displayname").ToString(), row("fieldmapping").ToString(), row("dropdowntype").ToString, row("dropdown").ToString)
    '            If UCase(row("dropdowntype")).ToString = "MASTER VALUED" Then
    '                menufields.Items.Add(menuItem)
    '                AddChildItems(row("dropdown").ToString, menuItem)

    '            Else
    '                menufields.Items.Add(menuItem)
    '                ' AddChildItems(table, menuItem)
    '            End If

    '        Next
    '    Catch ex As Exception
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not cmd Is Nothing Then
    '            cmd.Dispose()
    '        End If

    '    End Try

    'End Sub
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
            tv.Nodes.Add(masterNode)

            Dim view As New DataView(ds.Tables("table"))
            For Each row As DataRowView In view
                ViewState("cnt") = 0
                If UCase(row.Item("dropdowntype").ToString) = "MASTER VALUED" Then
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "+"
                    n.ImageUrl = "images/redp.png"
                    n.NavigateUrl = "javascript:setCaret('" & "Form." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)
                    LoadDocTree(row.Item("dropdown").ToString, n)
                Else
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "images/bluep.png"
                    n.NavigateUrl = "javascript:setCaret('" & "Form." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)

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


    'Private Sub LoadWorksourceTree()
    '    'Dim tvv As New TreeView()
    '    'Dim ab As String = ddlsourcedoc.SelectedItem.Text
    '    'ab = ab.Replace(" ", "_")
    '    'tvv.ID = "tv_" & ab
    '    'tvv.Nodes.Clear()
    '    ViewState("cnt") = 0
    '    Dim conStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = Nothing
    '    Dim cmd As SqlCommand = Nothing
    '    Dim da As SqlDataAdapter = Nothing
    '    Try
    '        Dim table As New DataTable()
    '        con = New SqlConnection(conStr)
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim sql As String = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddlsourcedoc.SelectedItem.Text.Trim() & "' order by displayname"
    '        cmd = New SqlCommand(sql, con)
    '        da = New SqlDataAdapter(cmd)
    '        Dim ds As New DataSet
    '        da.Fill(ds, "table")

    '        If ds.Tables("table").Rows.Count > 0 Then
    '            ddltf.DataSource = ds.Tables("table")
    '            ddltf.DataTextField = "displayname"
    '            ddltf.DataValueField = "fieldid"
    '            ddltf.DataBind()
    '        End If
    '        'End If

    '        Dim masterNode As New TreeNode(ddlsourcedoc.SelectedItem.Text.Trim.ToString())


    '        masterNode.Value = ddlsourcedoc.SelectedItem.Text.Trim.ToString()
    '        masterNode.ImageUrl = "images/Redp.png"
    '        masterNode.SelectAction = TreeNodeSelectAction.Expand
    '        tvsource.Nodes.Add(masterNode)

    '        Dim view As New DataView(ds.Tables("table"))
    '        For Each row As DataRowView In view
    '            ViewState("cnt") = 0
    '            If UCase(row.Item("dropdowntype").ToString) = "MASTER VALUED" Then
    '                Dim n As New TreeNode()
    '                n.Text = row.Item("displayname").ToString()
    '                n.Value = row.Item("fieldmapping")
    '                n.ImageUrl = "+"
    '                n.ImageUrl = "images/redp.png"
    '                masterNode.ChildNodes.Add(n)
    '                ' LoadDocTree(row.Item("dropdown").ToString, n)
    '            Else
    '                Dim n As New TreeNode()
    '                n.Text = row.Item("displayname").ToString()
    '                n.Value = row.Item("fieldmapping")
    '                n.ImageUrl = "images/bluep.png"
    '                masterNode.ChildNodes.Add(n)

    '            End If

    '        Next
    '    Catch ex As Exception
    '        lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not cmd Is Nothing Then
    '            cmd.Dispose()
    '        End If

    '    End Try
    'End Sub

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
                                    'If Not UCase(dropdown.ToString()) = UCase(row.Item("dropdown")).ToString() Then
                                    '    LoadDocTree(row.Item("dropdown").ToString, n)
                                    'End If

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
        'Dim ds As String = String.Empty
        'ds = tv.SelectedNode.Parent.Text & "." & tv.SelectedNode.Text
        'txtcondition.Text = txtcondition.Text & " " & ds

        ''Dim node As TreeNode = Me.tv.SelectedNode
        ''tv.SelectedNodeStyle.BackColor = Drawing.Color.Yellow
        ''Dim pathStr As [String] = node.Text
        ''Dim separator As String = "."

        ''tv.PathSeparator = Convert.ToChar(separator)

        ''While node.Parent IsNot Nothing
        ''    pathStr = node.Parent.Text + Me.tv.PathSeparator + pathStr
        ''    node = node.Parent
        ''End While
        ''pathStr = "{" & pathStr & "}"
        ''txtcondition.Text = txtcondition.Text.Trim.ToString() & " " & pathStr

    End Sub

    Protected Sub tvsource_SelectedNodeChanged(sender As Object, e As EventArgs) Handles tv.SelectedNodeChanged
        Dim node As TreeNode = Me.tvsource.SelectedNode
        tvsource.SelectedNodeStyle.BackColor = Drawing.Color.Yellow

    End Sub


    Protected Sub btnfflds_Click(sender As Object, e As EventArgs) Handles btnfflds.Click
        ''Dim ds As String = String.Empty

        ''If ddlfunctionFields.SelectedItem.Text = "DAY" Then
        ''    ds = "DAY(datetime)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "DATE" Then
        ''    ds = "DATE(datetime)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "MONTH" Then
        ''    ds = "MONTH(datetime)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "PREVIOUS MONTH" Then
        ''    ds = "PREVIOUS MONTH(datetime)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "YEAR" Then
        ''    ds = "YEAR(datetime)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "DATE RANGE" Then
        ''    ds = "DATERANGE((datetime),(datetime))"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "CONCATENATE" Then
        ''    ds = "CONCATENATE(fld1,fld2,text,spl charcter,fld 15,…….,n)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "MAX" Then
        ''    ds = "MAX(comma separated values)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "MIN" Then
        ''    ds = "MIN(comma separated values)"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "AND" Then
        ''    ds = "&&"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "NOT" Then
        ''    ds = "!="
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "OR" Then
        ''    ds = "||"
        ''ElseIf ddlfunctionFields.SelectedItem.Text = "SELECT" Then
        ''    ds = ""
        ''End If

        ' ''ds = Trim(ddlfunctionFields.SelectedItem.Text).ToString()

        ''txtcondition.Text = Trim(txtcondition.Text) & " " & ds
    End Sub

    Public Function bindonedit(ByVal eid As Integer, ByVal Ruleid As Integer) As String
        btnsave.Text = "Update"
        txtRuleName.Enabled = False
        Dim Result As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim dt As New DataTable
        Try
            da.SelectCommand.CommandText = "Select * from mmm_mst_rules where eid=" & eid & " and ruleid=" & Ruleid & " "
            da.Fill(ds, "Edit")
            If ds.Tables("Edit").Rows.Count > 0 Then



                'filling rulename
                txtRuleName.Text = ds.Tables("Edit").Rows(0).Item("Rulename").ToString()
                'Dim cks As String = 0
                'filling checkbox isactive
                'If ds.Tables("edit").Rows(0).Item("isactive").ToString = 1 Then
                '    chkisactive.Checked = True
                'Else
                '    chkisactive.Checked = False
                'End If

                'If chkisactive.Checked = True Then
                '    cks = 1
                'Else
                '    cks = 0
                'End If
                'Filling Rule Description
                txtdescription.Text = ds.Tables("edit").Rows(0).Item("ruledesc").ToString()

                'filling form source

                ddlformsource.Items.Clear()

                ddlformsource.Items.Insert(0, "SELECT")
                ddlformsource.Items.Insert(1, "DOCUMENT")
                ddlformsource.Items.Insert(2, "MASTER")
                ddlformsource.Items.Insert(3, "DETAIL FORM")
                ddlformsource.Items.Insert(4, "ACTION DRIVEN")
                ddlformsource.DataBind()

                ddlformsource.SelectedIndex = ddlformsource.Items.IndexOf(ddlformsource.Items.FindByText(ds.Tables("edit").Rows(0).Item("formsource").ToString()))


                da.SelectCommand.CommandText = "select sourcetype[Source Type],Sourcename[Source Name],targettype[Target Type],targetname[Target Name],s_relationidentifierfield[Source Field],T_RelationIdentifierField [Target Field],sortingfield[Sorting Field] from mmm_mst_rulerelation where eid=" & Session("EID") & " and ruleid=" & Ruleid & ""
                da.Fill(ds, "fe")
                da.Fill(dt)
                If ds.Tables("fe").Rows.Count > 0 Then
                    gvmap.DataSource = ds.Tables("fe")
                    gvmap.DataBind()
                    Session("datat") = dt
                    For i As Integer = 0 To ds.Tables("fe").Rows.Count - 1
                        LoadWorkGroupTreeSource(ds.Tables("fe").Rows(i).Item("Target Name").ToString)
                    Next
                End If



                'filling document type

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





                'filling docnature
                ddldocnature.Items.Clear()

                ddldocnature.Items.Insert(0, "SELECT")
                ddldocnature.Items.Insert(1, "CREATED")
                ddldocnature.Items.Insert(2, "AMENDMENT")
                ddldocnature.Items.Insert(3, "CANCEL")
                ddldocnature.DataBind()

                ddldocnature.SelectedIndex = ddldocnature.Items.IndexOf(ddldocnature.Items.FindByText(ds.Tables("edit").Rows(0).Item("docnature").ToString()))

                'filling when to run

                ddltypeofrun.Items.Clear()

                ddltypeofrun.Items.Insert(0, "SELECT")
                ddltypeofrun.Items.Insert(1, "FORM LOAD")
                ddltypeofrun.Items.Insert(2, "SUBMIT")
                ddltypeofrun.Items.Insert(3, "CONTROL")
                ddltypeofrun.Items.Insert(4, "APPROVE")
                ddltypeofrun.Items.Insert(5, "REJECT")
                ddltypeofrun.Items.Insert(6, "CRM(HOLD)")
                ddltypeofrun.Items.Insert(7, "DRAFT")
                ddltypeofrun.Items.Insert(8, "ACTIVE")
                ddltypeofrun.Items.Insert(9, "INACTIVE")
                ddltypeofrun.DataBind()

                ddltypeofrun.SelectedIndex = ddltypeofrun.Items.IndexOf(ddltypeofrun.Items.FindByText(ds.Tables("edit").Rows(0).Item("whentorun").ToString()))

                'filling SUccess action type

                divrow2.Visible = True
                divrows.Visible = True

                'If Not Session("datat") Is Nothing Then
                '    Dim dsd As DataSet = CType(Session("datat"), DataSet)
                '    For i As Integer = 0 To dsd.Tables("fe").Rows.Count - 1
                '        LoadWorkGroupTree(dsd.Tables("fe").Rows(i).Item("Target Name").ToString)
                '    Next
                'End If
                If UCase(ddltypeofrun.SelectedItem.Text) = "FORM LOAD" Then


                    tv.Nodes.Clear()
                    'Dim list As New List(Of String)
                    Dim treeNode As New TreeNode("UID")
                    treeNode.ImageUrl = "images/bluep.png"
                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("USERNAME")
                    treeNode.ImageUrl = "images/bluep.png"
                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("USERROLE")
                    treeNode.ImageUrl = "images/bluep.png"
                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("CODE")
                    treeNode.ImageUrl = "images/bluep.png"
                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("USERIMAGE")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("CLOGO")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("EID")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("ISLOCAL")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("IPADDRESS")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("MACADDRESS")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("INTIME")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("EMAIL")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("LID")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("HEADERSTRIP")
                    treeNode.ImageUrl = "images/bluep.png"

                    tv.Nodes.Add(treeNode)
                    treeNode = New TreeNode("ROLES")
                    tv.Nodes.Add(treeNode)


                    treeNode.ImageUrl = "images/bluep.png"

                    tv.DataBind()


                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "SUBMIT" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()

                    ddlsuccessaction.Items.Insert(0, "SELECT")
                    ddlsuccessaction.Items.Insert(1, "SAVE")
                    ddlsuccessaction.Items.Insert(2, "SAVE WITH ALERT")
                    ddlsuccessaction.Items.Insert(3, "DENY")
                    ddlsuccessaction.Items.Insert(4, "WORKFLOW")
                    ddlsuccessaction.Items.Insert(5, "MANDATORY")
                    ddlsuccessaction.Items.Insert(6, "NON MANDATORY")
                    ddlsuccessaction.Items.Insert(7, "SEND EMAIL")

                    ddlsuccessaction.DataBind()

                    ddlfaction.Items.Insert(0, "SELECT")
                    ddlfaction.Items.Insert(1, "SAVE")
                    ddlfaction.Items.Insert(2, "SAVE WITH ALERT")
                    ddlfaction.Items.Insert(3, "DENY")
                    ddlfaction.Items.Insert(4, "WORKFLOW")
                    ddlfaction.Items.Insert(5, "MANDATORY")
                    ddlfaction.Items.Insert(6, "NON MANDATORY")
                    ddlfaction.Items.Insert(7, "SEND EMAIL")

                    ddlfaction.DataBind()



                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "CONTROL" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")
                    ddlsuccessaction.Items.Insert(1, "ENABLE")
                    ddlsuccessaction.Items.Insert(2, "DISABLE")
                    ddlsuccessaction.Items.Insert(3, "VISIBLE")
                    ddlsuccessaction.Items.Insert(4, "INVISIBLE")
                    ddlsuccessaction.Items.Insert(5, "CHANGE DROPDOWN COLOR")
                    ddlsuccessaction.Items.Insert(6, "NO ACTION")
                    ddlsuccessaction.DataBind()

                    ddlfaction.Items.Clear()
                    ddlfaction.Items.Insert(0, "SELECT")
                    ddlfaction.Items.Insert(1, "ENABLE")
                    ddlfaction.Items.Insert(2, "DISABLE")
                    ddlfaction.Items.Insert(3, "VISIBLE")
                    ddlfaction.Items.Insert(4, "INVISIBLE")
                    ddlfaction.Items.Insert(5, "CHANGE DROPDOWN COLOR")
                    ddlfaction.Items.Insert(6, "ALERT")
                    ddlfaction.DataBind()

                    lblcontrolfield.Visible = True
                    ddlcontrolfield.Visible = True




                    chktargetfields.Items.Clear()
                    da.SelectCommand.CommandText = "Select fieldid ,displayname from mmm_mst_Fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text.ToString() & "'"
                    da.Fill(ds, "chklistc")
                    If ds.Tables("chklistc").Rows.Count > 0 Then
                        chktargetfields.DataSource = ds.Tables("chklistc")
                        chktargetfields.DataTextField = "displayname"
                        chktargetfields.DataValueField = "fieldid"
                        chktargetfields.DataBind()

                        'binding control dropdown
                        ddlcontrolfield.DataSource = ds.Tables("chklistc")
                        ddlcontrolfield.DataTextField = "displayname"
                        ddlcontrolfield.DataValueField = "fieldid"
                        ddlcontrolfield.DataBind()
                        ddlcontrolfield.SelectedValue = ds.Tables("edit").Rows(0).Item("controlfield").ToString()
                    End If
                    Dim str As String() = ds.Tables("edit").Rows(0).Item("TargetControlfield").ToString().Split(",")
                    Dim val As String = ""
                    For i As Integer = 0 To str.Length - 1
                        For j As Integer = 0 To chktargetfields.Items.Count - 1
                            If str(i).ToString = chktargetfields.Items(j).Value Then
                                chktargetfields.Items(j).Selected = True
                                'val = val & chktargetfields.Items(j).Text.ToString()
                                val = val & chktargetfields.Items(j).Text.ToString() & ","
                            Else
                                ' chktargetfields.Items(j).Selected = False
                            End If
                        Next
                    Next
                    If Not String.IsNullOrEmpty(val) Then
                        val = val.Substring(0, val.Length - 1)
                    End If
                    txtboxtargetcontrol.Visible = True
                    btnopentargetcontrol.Visible = True
                    txtboxtargetcontrol.Text = val.ToString()
                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "APPROVE" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")
                    ddlsuccessaction.Items.Insert(1, "SAVE")
                    ddlsuccessaction.Items.Insert(2, "SAVE WITH ALERT")
                    ddlsuccessaction.Items.Insert(3, "DENY")
                    ddlsuccessaction.Items.Insert(4, "WORKFLOW")
                    ddlsuccessaction.Items.Insert(5, "MANDATORY")
                    ddlsuccessaction.Items.Insert(6, "NON MANDATORY")
                    ddlsuccessaction.Items.Insert(7, "SEND EMAIL")
                    ddlsuccessaction.DataBind()

                    ddlfaction.Items.Clear()
                    ddlfaction.Items.Insert(0, "SELECT")
                    ddlfaction.Items.Insert(1, "SAVE")
                    ddlfaction.Items.Insert(2, "SAVE WITH ALERT")
                    ddlfaction.Items.Insert(3, "DENY")
                    ddlfaction.Items.Insert(4, "WORKFLOW")
                    ddlfaction.Items.Insert(5, "MANDATORY")
                    ddlfaction.Items.Insert(6, "NON MANDATORY")
                    ddlfaction.Items.Insert(7, "SEND EMAIL")
                    ddlfaction.DataBind()





                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "REJECT" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")
                    ddlsuccessaction.Items.Insert(1, "DENY")
                    ddlsuccessaction.Items.Insert(2, "SEND EMAIL")
                    ddlsuccessaction.DataBind()

                    ddlfaction.Items.Clear()
                    ddlfaction.Items.Insert(0, "SELECT")
                    ddlfaction.Items.Insert(1, "DENY")
                    ddlfaction.Items.Insert(2, "SEND EMAIL")
                    ddlfaction.DataBind()

                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "CRM(HOLD)" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")

                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "DRAFT" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")
                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "ACTIVE" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")
                ElseIf UCase(ddltypeofrun.SelectedItem.Text) = "INACTIVE" Then
                    Call LoadWorkGroupTree(ddldoctype.SelectedItem.Text.ToString.Trim())
                    ddlsuccessaction.Items.Clear()
                    ddlsuccessaction.Items.Insert(0, "SELECT")

                End If


                ddlsuccessaction.SelectedIndex = ddlsuccessaction.Items.IndexOf(ddlsuccessaction.Items.FindByText(ds.Tables("edit").Rows(0).Item("succactiontype").ToString()))
                ddlfaction.SelectedIndex = ddlfaction.Items.IndexOf(ddlfaction.Items.FindByText(ds.Tables("edit").Rows(0).Item("failactiontype").ToString()))

                'bind successactionfiels

                ddlsf.Items.Insert(0, "SELECT")
                If UCase(ddlsuccessaction.SelectedItem.Text).ToString = "WORKFLOW" Then
                    ddlsf.Visible = True

                    ddlsf.Items.Clear()
                    ddlsf.Items.Insert(0, "SELECT")
                    ddlsf.Items.Insert(1, "DEFAULT")
                    ddlsf.Items.Insert(2, "MANUAL")
                    ddlsf.DataBind()

                ElseIf UCase(ddlsuccessaction.SelectedItem.Text).ToString = "MANDATORY" Or UCase(ddlsuccessaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
                    ddlsf.Visible = True

                    'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    'Dim con As New SqlConnection(conStr)
                    'Dim da As New SqlDataAdapter("", con)
                    'Dim ds As New DataSet
                    Try

                        da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname"
                        da.Fill(ds, "fld")
                        If ds.Tables("fld").Rows.Count > 0 Then
                            ddlsf.DataSource = ds.Tables("fld")
                            ddlsf.DataTextField = "displayname"
                            ddlsf.DataValueField = "fieldmapping"
                            ddlsf.DataBind()

                            ddlsortingfields.DataSource = ds.Tables("fld")
                            ddlsortingfields.DataTextField = "displayname"
                            ddlsortingfields.DataValueField = "fieldmapping"
                            ddlsortingfields.DataBind()
                        End If


                    Catch ex As Exception
                        lblMsg.Text = "SERVER ERROR: PLEASE TRY AGAIN AFTER SOME TIME!"
                    Finally
                        If Not con Is Nothing Then
                            con.Close()
                        End If
                        If Not da Is Nothing Then
                            'da.Dispose()
                        End If
                    End Try

                    ' ElseIf UCase(ddlsuccessaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
                    'ddlsf.Visible = True

                    'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    'Dim con As New SqlConnection(conStr)
                    'Dim da As New SqlDataAdapter("", con)
                    'Dim ds As New DataSet



                Else

                    ddlsf.Visible = False

                End If

                ddlsf.SelectedIndex = ddlsf.Items.IndexOf(ddlsf.Items.FindByText(ds.Tables("edit").Rows(0).Item("succactionfield").ToString()))
                '
                ddlfflds.Items.Insert(0, "SELECT")
                If UCase(ddlfaction.SelectedItem.Text).ToString = "WORKFLOW" Then
                    ddlfflds.Visible = True

                    ddlfflds.Items.Clear()
                    ddlfflds.Items.Insert(0, "SELECT")
                    ddlfflds.Items.Insert(1, "DEFAULT")
                    ddlfflds.Items.Insert(2, "MANUAL")
                    ddlfflds.DataBind()

                ElseIf UCase(ddlfaction.SelectedItem.Text).ToString = "MANDATORY" Or UCase(ddlfaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
                    ddlfflds.Visible = True

                    'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    'Dim con As New SqlConnection(conStr)
                    'Dim da As New SqlDataAdapter("", con)
                    'Dim ds As New DataSet
                    Try
                        da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname"
                        da.Fill(ds, "fld")
                        If ds.Tables("fld").Rows.Count > 0 Then
                            ddlfflds.DataSource = ds.Tables("fld")
                            ddlfflds.DataTextField = "displayname"
                            ddlfflds.DataValueField = "fieldmapping"
                            ddlfflds.DataBind()
                        End If


                    Catch ex As Exception
                        lblMsg.Text = "SERVER ERROR: PLEASE TRY AGAIN AFTER SOME TIME!"
                    Finally
                        If Not con Is Nothing Then
                            con.Close()
                        End If
                        If Not da Is Nothing Then
                            'da.Dispose()
                        End If
                    End Try



                    ' ElseIf UCase(ddlsuccessaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
                    'ddlsf.Visible = True

                    'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    'Dim con As New SqlConnection(conStr)
                    'Dim da As New SqlDataAdapter("", con)
                    'Dim ds As New DataSet



                Else

                    ddlfflds.Visible = False

                End If
                ddlfflds.SelectedIndex = ddlfflds.Items.IndexOf(ddlfflds.Items.FindByText(ds.Tables("edit").Rows(0).Item("failactionfield").ToString()))



                'bind treeview
                ' LoadWorkGroupTree(ddldoctype.SelectedItem.Text.Trim.ToString)

                'fill condition textbox

                txtcondition.Text = ds.Tables("edit").Rows(0).Item("condition").ToString()


                'filling success textbox
                txtsuccessmsg.Text = ds.Tables("edit").Rows(0).Item("succmsg").ToString()
                'filling error msg textbox

                txterrormessage.Text = ds.Tables("edit").Rows(0).Item("errormsg").ToString()

                'enabling rows
                divrow.Visible = True
                divrow2.Visible = True

            Else
                Response.Redirect("RuleEngineDesigner.aspx")
            End If
            da.SelectCommand.CommandText = "select upper(displayname)[formname],upper(fieldmapping)[fieldmapping] from mmm_mst_fields where eid=" & Session("Eid") & " and documenttype='" & Trim(ddldoctype.SelectedItem.Text) & "' order by displayname"
            da.Fill(ds, "fields")
            If ds.Tables("fields").Rows.Count > 0 Then
                ddlsdf.DataSource = ds.Tables("fields")
                ddlsdf.DataTextField = "formname"
                ddlsdf.DataValueField = "fieldmapping"
                ddlsdf.DataBind()
                ddlsdf.Items.Insert(0, "SELECT")
                ddlsdf.SelectedIndex = ddlsdf.Items.IndexOf(ddlsdf.Items.FindByText(ds.Tables("fields").Rows(0).Item("formname").ToString()))
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




    Protected Sub ddlsuccessaction_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsuccessaction.SelectedIndexChanged
        ddlsf.Items.Insert(0, "SELECT")
        If UCase(ddlsuccessaction.SelectedItem.Text).ToString = "WORKFLOW" Then
            ddlsf.Visible = True

            ddlsf.Items.Clear()
            ddlsf.Items.Insert(0, "SELECT")
            ddlsf.Items.Insert(1, "DEFAULT")
            ddlsf.Items.Insert(2, "MANUAL")
            ddlsf.DataBind()

        ElseIf UCase(ddlsuccessaction.SelectedItem.Text).ToString = "MANDATORY" Or UCase(ddlsuccessaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
            ddlsf.Visible = True

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Try

                da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname"
                da.Fill(ds, "fld")
                If ds.Tables("fld").Rows.Count > 0 Then
                    ddlsf.DataSource = ds.Tables("fld")
                    ddlsf.DataTextField = "displayname"
                    ddlsf.DataValueField = "fieldmapping"
                    ddlsf.DataBind()
                End If


            Catch ex As Exception
                lblMsg.Text = "SERVER ERROR: PLEASE TRY AGAIN AFTER SOME TIME!"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try

            ' ElseIf UCase(ddlsuccessaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
            'ddlsf.Visible = True

            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As New SqlConnection(conStr)
            'Dim da As New SqlDataAdapter("", con)
            'Dim ds As New DataSet



        Else

            ddlsf.Visible = False

        End If


    End Sub

    Protected Sub ddlfaction_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfaction.SelectedIndexChanged
        ddlfflds.Items.Insert(0, "SELECT")
        If UCase(ddlfaction.SelectedItem.Text).ToString = "WORKFLOW" Then
            ddlfflds.Visible = True

            ddlfflds.Items.Clear()
            ddlfflds.Items.Insert(0, "SELECT")
            ddlfflds.Items.Insert(1, "DEFAULT")
            ddlfflds.Items.Insert(2, "MANUAL")
            ddlfflds.DataBind()

        ElseIf UCase(ddlfaction.SelectedItem.Text).ToString = "MANDATORY" Or UCase(ddlfaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
            ddlfflds.Visible = True

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Try

                da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname"
                da.Fill(ds, "fld")
                If ds.Tables("fld").Rows.Count > 0 Then
                    ddlfflds.DataSource = ds.Tables("fld")
                    ddlfflds.DataTextField = "displayname"
                    ddlfflds.DataValueField = "fieldmapping"
                    ddlfflds.DataBind()
                End If


            Catch ex As Exception
                lblMsg.Text = "SERVER ERROR: PLEASE TRY AGAIN AFTER SOME TIME!"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try

            ' ElseIf UCase(ddlsuccessaction.SelectedItem.Text).ToString = "NON MANDATORY" Then
            'ddlsf.Visible = True

            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As New SqlConnection(conStr)
            'Dim da As New SqlDataAdapter("", con)
            'Dim ds As New DataSet



        Else

            ddlfflds.Visible = False

        End If
    End Sub

    Protected Sub ddlsourcetype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsourcetype.SelectedIndexChanged
        lblMsg.Text = String.Empty
        If UCase(ddlsourcetype.SelectedItem.Text) = "MASTER" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='master' order by formname"
                da.Fill(ds, "master")

                If ds.Tables("master").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("master")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
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

        ElseIf UCase(ddlsourcetype.SelectedItem.Text) = "DOCUMENT" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='DOCUMENT' and formsource='menu driven' order by formname"
                da.Fill(ds, "document")

                If ds.Tables("document").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("document")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
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
        ElseIf UCase(ddlsourcetype.SelectedItem.Text) = "ACTION DRIVEN" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='ACTION DRIVEN' order by formname"
                da.Fill(ds, "actiondriven")

                If ds.Tables("actiondriven").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("actiondriven")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
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
        ElseIf UCase(ddlsourcetype.SelectedItem.Text) = "DETAIL FORM" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='DETAIL FORM ' order by formname"
                da.Fill(ds, "detailform")

                If ds.Tables("detailform").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("detailform")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
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

            lblMsg.Text = "Please Select Source Type!!"
            ddlsourcedoc.Items.Clear()
            Exit Sub
        End If
    End Sub

    Protected Sub ddlsourcedoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsourcedoc.SelectedIndexChanged
        bindfields()
        'LoadWorkGroupTree(ddlsourcedoc.SelectedItem.Text.ToString())
    End Sub

    'Protected Sub btnnext_Click(sender As Object, e As EventArgs) Handles btnnext.Click



    '    'lbldi.Text = "Common Field From " & ddldoctype.SelectedItem.Text & " is : " & tv.SelectedNode.Text.ToString & " and from " & ddlsourcedoc.SelectedItem.Text & " is : " & tvsource.SelectedNode.Text & " "
    'End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)

        Dim Ruleid As Integer = Convert.ToString(Me.gvmap.DataKeys(row.RowIndex).Value)

        ViewState("id") = Ruleid
        Me.updPnlGrid.Update()
        'Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(sender As Object, e As EventArgs)
        Me.btnEdit_ModalPopupExtender.Hide()
        Dim ds As DataTable
        ds = DirectCast(Session("datat"), DataTable)
        Dim dr As DataRow = ds.NewRow()
        dr(0) = ddlformsource.SelectedItem.Text.Trim()
        dr(1) = ddldoctype.SelectedItem.Text.Trim()
        dr(2) = ddlsourcetype.SelectedItem.Text.Trim()
        dr(3) = ddlsourcedoc.SelectedItem.Text.Trim()
        dr(4) = ddlsdf.SelectedItem.Text.Trim()
        dr(5) = ddltf.SelectedItem.Text.Trim()
        dr(6) = ddlsortingfields.SelectedItem.Text.Trim()


        ds.Rows.Add(dr)

        Session("datat") = ds
        bindgrid()

        'CreateTreeView(ddlsourcedoc.SelectedItem.Text.ToString())
        LoadWorkGroupTreeSource(ddlsourcedoc.SelectedItem.Text.ToString())
        updatePanelEdit.Update()
        Me.updPnlGrid.Update()
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        lblsd.Text = ddldoctype.SelectedItem.Text.Trim
        ddlsourcedoc.Items.Clear()
        ddlsortingfields.Items.Clear()
        ddltf.Items.Clear()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub bindgrid()

        Dim ds As DataTable = DirectCast(Session("datat"), DataTable)

        gvmap.DataSource = ds
        gvmap.DataBind()
        'updgrid.Update()
    End Sub

    Private Sub bindfields()

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
            Dim sql As String = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddlsourcedoc.SelectedItem.Text.ToString().Trim() & "' order by displayname"
            cmd = New SqlCommand(sql, con)
            da = New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds, "table")

            If ds.Tables("table").Rows.Count > 0 Then
                ddltf.DataSource = ds.Tables("table")
                ddltf.DataTextField = "displayname"
                ddltf.DataValueField = "fieldid"
                ddltf.DataBind()

                ddlsortingfields.DataSource = ds.Tables("table")
                ddlsortingfields.DataTextField = "displayname"
                ddlsortingfields.DataValueField = "fieldid"
                ddlsortingfields.DataBind()

            End If

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

    Protected Sub OnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim item As String = e.Row.Cells(3).Text
            For Each button As ImageButton In e.Row.Cells(0).Controls.OfType(Of ImageButton)()
                If button.CommandName = "Delete" Then
                    button.Attributes("onclick") = "if(!confirm('Do you want to delete " + item + "?')){ return false; };"
                End If
            Next
        End If
    End Sub

    Protected Sub OnRowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = Convert.ToInt32(e.RowIndex)

        Dim dt As DataTable = TryCast(Session("datat"), DataTable)
        dt.Rows.RemoveAt(index)

        Session("datat") = dt
        bindgrid()
        updPnlGrid.Update()

    End Sub

    Protected Sub ddlfunctionFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfunctionFields.SelectedIndexChanged
        If UCase(ddlfunctionFields.SelectedItem.Text) = "DAY" Then
            lblexampl.Text = "Format: DAY[DateTime], Returns day e.g. Day[dd/mm/yy] result Thursday "
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "DATE" Then
            lblexampl.Text = "Format: DATE[datetime], Returns date e.g. Date[dd/mm/yy] result dd "
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "MONTH" Then
            lblexampl.Text = "Format: MONTH[datetime], Returns month e.g. Month[dd/mm/yy] result month "
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "PREVIOUS MONTH" Then
            lblexampl.Text = "Format: PREVIOUS MONTH[datetime], Returns Previous Month e.g. Previous Month[4-Sep-14] result August or 8"
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "YEAR" Then
            lblexampl.Text = "Format: YEAR[datetime], Returns year e.g. Year[4-Sep-14] result 2014"
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "DATE RANGE" Then
            lblexampl.Text = "Format: DATERANGE[datetime,datetime], Returns date range e.g. DateRange[4-Sep-14,5-Sep-14]"
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "CONCATENATE" Then
            lblexampl.Text = "Format: CONCATENATE[fld1,fld2,text,spl charcter,fld 15,…….,n]"
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "AND" Then
            lblexampl.Text = "Format: AND or && "
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "NOT" Then
            lblexampl.Text = "Format: Not or != "
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "OR" Then
            lblexampl.Text = "Format: OR or ||"
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "MAX" Then
            lblexampl.Text = "Format: Max[fld1,fld2,...,n], Returns the max value e.g Max[30,12] gives 30"
        ElseIf UCase(ddlfunctionFields.SelectedItem.Text) = "MIN" Then
            lblexampl.Text = "Format: Min[fld1,fld2,....,n]"
        End If



    End Sub

    Protected Sub btnsavetargetfields_Click(sender As Object, e As EventArgs) Handles btnsavetargetfields.Click
        modalpopuptagetfields.Hide()

        updPnlGrid.Update()
        Dim str As String = ""
        Dim val As String = ""
        For i As Integer = 0 To chktargetfields.Items.Count - 1
            If chktargetfields.Items(i).Selected = True Then
                str = str & chktargetfields.Items(i).Text.ToString() & ","
                val = val & chktargetfields.Items(i).Value.ToString() & ","
            End If
        Next
        If str.Length > 0 Then
            str = str.Remove(str.Length - 1)
            val = val.Remove(val.Length - 1)
        End If




        txtboxtargetcontrol.Visible = True

        txtboxtargetcontrol.Text = str.ToString()

        If Not Session("valControl") Is Nothing Then
            Session("valControl") = Session("valControl") & "," & val
        Else
            Session("valControl") = val
        End If


    End Sub

    ' binding source Treeview

    Private Sub LoadWorkGroupTreeSource(ByVal doctype As String)
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
            tvsource.Nodes.Add(masterNode)

            Dim view As New DataView(ds.Tables("table"))
            For Each row As DataRowView In view
                ViewState("cnt") = 0
                If UCase(row.Item("dropdowntype").ToString) = "MASTER VALUED" Then
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "+"
                    n.ImageUrl = "images/redp.png"
                    n.NavigateUrl = "javascript:setCaret('" & "Ds." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)
                    LoadDocTree(row.Item("dropdown").ToString, n)
                Else
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "images/bluep.png"
                    n.NavigateUrl = "javascript:setCaret('" & "DS." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)

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


End Class
