Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Collections

Partial Class MapSettings
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)
    Dim files As New DataTable

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        files = GetFiles("images/MapIcons/")
        If Not IsPostBack Then
            BindGrid()
            Bindddl()
            bindImage()
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
    Protected Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        Try
        divMsg.InnerHtml = ""
        divMsg.Visible = False
           
        Dim var = ValidateGroupName(txtgroup.Text.Trim())
        If var Then
            Dim chked As Boolean = False
            For i As Integer = 0 To GridView1.Rows.Count - 1
                Dim chk As CheckBox = DirectCast(GridView1.Rows(i).FindControl("chk1"), CheckBox)
                    If chk.Checked Then
                        chked = True

                        Dim chkInfo As CheckBox = DirectCast(GridView1.Rows(i).FindControl("CheckBox1"), CheckBox)
                        Dim chkList = DirectCast(GridView1.Rows(i).FindControl("chkList"), CheckBoxList)
                        Dim textBox = DirectCast(GridView1.Rows(i).FindControl("txtQuery"), TextBox)
                        Dim lbl As Label = DirectCast(GridView1.Rows(i).FindControl("lbl1"), Label)

                        Dim Query As String = ""
                        If chkInfo.Checked Then
                            Query = textBox.Text.Trim()
                            Query = Replace(Query, "'", "''")
                        Else
                            Dim fld = ""
                            For j As Integer = 0 To chkList.Items.Count - 1
                                If chkList.Items(j).Selected = True Then
                                    fld &= chkList.Items(j).Value.ToString() & IIf(j = chkList.Items.Count - 1, "", ",")
                                End If
                            Next
                            fld = fld.Substring(0, fld.Length - 1)
                            Dim updtQry = "Update MMM_MST_FIELDS set kc_Value='" & fld & "' where  Eid=" & Session("Eid") & " and Documenttype='" & lbl.Text & "' and FieldType='Geo Point'"
                            Dim dal As New BpmHelper()
                            dal.ExecDML(updtQry)

                        End If


                        Dim ddl As DropDownList = DirectCast(GridView1.Rows(i).FindControl("ddlIcon"), DropDownList)
                        Dim qry As String = "insert into mmm_mst_GroupMapSettings(Eid,GroupName,DocType,IconName, InfoQuery) "
                        qry &= "Values(" & Session("Eid") & ",'" & txtgroup.Text.Trim() & "', '" & lbl.Text & "',  '" & ddl.SelectedItem.Text & "', '" & Query & "')"
                        Dim cmd As New SqlCommand(qry, con)

                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim roAff = cmd.ExecuteNonQuery()
                        con.Close()
                    Else
                        Continue For
                    End If

            Next

            If Not chked Then
                Dim str As String = "<div class='red'>"
                    str &= "No row selected Please select atleast one roe in order to continue.<br/>"
                str &= "</div>"
                divMsg.Visible = True
                divMsg.InnerHtml = str
            Else
                Dim str As String = "<div class='green'>"
                str &= "Data saved successfully."
                str &= "</div>"
                divMsg.Visible = True
                divMsg.InnerHtml = str
                End If
            End If
            bindImage()
        Catch ex As Exception
            Dim str As String = "<div class='red'>"
            str &= "Server error. Please contact your system administrator."
            str &= "</div>"
            divMsg.Visible = True
            divMsg.InnerHtml = str
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function GetFiles(strPath As String) As DataTable
        Dim fpath As String = Server.MapPath(strPath)
        Dim txtFiles = Directory.GetFiles(fpath)
        Dim dt As New DataTable()
        dt.Columns.Add("FileName")
        dt.Columns.Add("Path")
        For Each filenm As String In txtFiles
            Dim dr = dt.NewRow()
            dr.Item("FileName") = Path.GetFileName(filenm)
            dr.Item("Path") = "images/MapIcons/" + Path.GetFileName(filenm)
            dt.Rows.Add(dr)
        Next
        Return dt
    End Function

    Private Sub BindGrid()
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "Select FormId[Tid], FormName from mmm_mst_forms where Eid=" & Session("Eid") & " and formName in(Select Distinct DocumentType from mmm_mst_fields where Eid=" & Session("Eid") & " and fieldtype='geo point')"
        Dim dt As New DataTable
        oda.Fill(dt)
        GridView1.DataSource = dt
        GridView1.DataBind()
    End Sub

    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ddl As DropDownList
            ddl = CType(e.Row.FindControl("ddlIcon"), DropDownList)
            ddl.DataSource = files
            ddl.DataTextField = "FileName"
            ddl.DataValueField = "Path"
            ddl.DataBind()
            For i As Integer = 0 To ddl.Items.Count - 1
                Dim item As ListItem = ddl.Items(i)
                item.Attributes("style") = "background: url('" + ddl.Items(i).Value + "');background-repeat:no-repeat;"
            Next

            Dim doc = CType(e.Row.FindControl("lbl1"), Label).Text
            Dim Qry = "Select DisplayName[Text], FieldMapping[Value] from mmm_mst_Fields where IsActive=1 and Documenttype='" & doc & "' and Eid=" & Session("Eid")
            Dim obj As New BpmHelper()
            Dim ds = obj.EcecDataSet(Qry)

            Dim ChkList = CType(e.Row.FindControl("chkList"), CheckBoxList)
            ChkList.DataSource = ds.obj
            ChkList.DataTextField = "Text"
            ChkList.DataValueField = "Value"
            ChkList.DataBind()

        End If
    End Sub

    Private Sub bindImage()
        For i As Integer = 0 To GridView1.Rows.Count - 1
            Dim ddl As DropDownList = DirectCast(GridView1.Rows(i).FindControl("ddlIcon"), DropDownList)
            For j As Integer = 0 To ddl.Items.Count - 1
                Dim item As ListItem = ddl.Items(j)
                item.Attributes("style") = "background: url('" + ddl.Items(j).Value + "');background-repeat:no-repeat;"
            Next
        Next
        For i As Integer = 0 To grdEdit.Rows.Count - 1
            Dim ddl As DropDownList = DirectCast(grdEdit.Rows(i).FindControl("ddlIcon"), DropDownList)
            For j As Integer = 0 To ddl.Items.Count - 1
                Dim item As ListItem = ddl.Items(j)
                item.Attributes("style") = "background: url('" + ddl.Items(j).Value + "');background-repeat:no-repeat;"
            Next
        Next
    End Sub

    Private Function ValidateGroupName(Group As String) As Boolean
        Try

            If Group = "" Then
                Dim str As String = "<div class='red'>"
                str &= "Group Name is required."
                str &= "</div>"
                divMsg.Visible = True
                divMsg.InnerHtml = str
                Return False
            End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "Select * from mmm_mst_GroupMapSettings where GroupName='" & Group & "' and Eid=" & Session("Eid") & ""
        Dim dt As New DataTable
        oda.Fill(dt)
        If dt.Rows.Count > 0 Then
            Dim str As String = "<div class='red'>"
            str &= "Group name already exists. It must be unique."
            str &= "</div>"
            divMsg.Visible = True
            divMsg.InnerHtml = str
            Return False
            Else
                For i As Integer = 0 To GridView1.Rows.Count - 1
                    Dim chkInfo As CheckBox = DirectCast(GridView1.Rows(i).FindControl("CheckBox1"), CheckBox)
                    Dim chkList = DirectCast(GridView1.Rows(i).FindControl("chkList"), CheckBoxList)
                    Dim textBox = DirectCast(GridView1.Rows(i).FindControl("txtQuery"), TextBox)
                    If chkInfo.Checked Then
                        If textBox.Text.Trim = "" Then
                            Dim str As String = "<div class='red'>"
                            str &= "Please fill query field in order to continue."
                            str &= "</div>"
                            divMsg.Visible = True
                            divMsg.InnerHtml = str
                            Return False
                        Else
                            divMsg.InnerHtml = ""
                            divMsg.Visible = False
                        End If
                    Else

                    End If
                Next

                divMsg.InnerHtml = ""
                divMsg.Visible = False
                Return True
            End If
        Catch ex As Exception
            Dim str As String = "<div class='red'>"
            str &= "Server error. Please contact your system administrator."
            str &= "</div>"
            divMsg.Visible = True
            divMsg.InnerHtml = str
            Return False
        Finally
            
        End Try
    End Function

    Public Sub Bindddl()
        Dim da As New SqlDataAdapter("Select Distinct GroupName from mmm_mst_GroupMapSettings where Eid=" & Session("Eid"), con)
        Dim dt As New DataTable
        da.Fill(dt)
        ddlGroup.DataValueField = "GroupName"
        ddlGroup.DataTextField = "GroupName"
        ddlGroup.DataSource = dt
        ddlGroup.DataBind()
        ddlGroup.Items.Insert(0, "Select")
    End Sub

    Protected Sub ddlGroup_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlGroup.SelectedIndexChanged
        Dim Qry = "with cte as (Select FormId, FormName from mmm_mst_forms where Eid=" & Session("Eid") & " and formName in(Select Distinct DocumentType from mmm_mst_fields where Eid=" & Session("Eid") & " and fieldtype='geo point'))"
        Qry = Qry & " Select isNull(g.Tid,0)[Tid],g.GroupName, FormName, isNull(g.IconName,'marker1.png')[IconName],"
        Qry = Qry & "Case  when g.Tid is null then 'False' else 'True' End as [Checked] "
        Qry = Qry & "from cte left join ( Select * from mmm_mst_GroupMapSettings where GroupName='" & ddlGroup.SelectedItem.Text & "' and Eid=" & Session("Eid") & ") g on cte.FormName=g.DocType"
        Dim da As New SqlDataAdapter(Qry, con)
        Dim dt As New DataTable
        da.Fill(dt)
        grdEdit.DataSource = dt
        grdEdit.DataBind()
        divGrid.Visible = True
        ScriptManager.RegisterClientScriptBlock(Me.Page, GetType(String), "calcHashFunction", "iterate();", True)

    End Sub

    Protected Sub grdEdit_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdEdit.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ddl As DropDownList
            ddl = CType(e.Row.FindControl("ddlIcon"), DropDownList)
            ddl.DataSource = files
            ddl.DataTextField = "FileName"
            ddl.DataValueField = "Path"
            ddl.DataBind()
            Dim icon As String = (DirectCast(e.Row.FindControl("lblicon"), Label)).Text
            ddl.Items.FindByText(icon).Selected = True
            For i As Integer = 0 To ddl.Items.Count - 1
                Dim item As ListItem = ddl.Items(i)
                item.Attributes("style") = "background: url('" + ddl.Items(i).Value + "');background-repeat:no-repeat;"
            Next

            Dim doc = CType(e.Row.FindControl("lbl1"), Label).Text
            Dim Qry = "Select DisplayName[Text], FieldMapping[Value] from mmm_mst_Fields where IsActive=1 and Documenttype='" & doc & "' and Eid=" & Session("Eid")
            Dim obj As New BpmHelper()
            Dim ds = obj.EcecDataSet(Qry)

            Dim ChkList = CType(e.Row.FindControl("chkList"), CheckBoxList)
            ChkList.DataSource = ds.obj
            ChkList.DataTextField = "Text"
            ChkList.DataValueField = "Value"
            ChkList.DataBind()

            Dim chkInfo As CheckBox = DirectCast(e.Row.FindControl("CheckBox1"), CheckBox)
            Dim textBox = DirectCast(e.Row.FindControl("txtQuery"), TextBox)

            Dim settingsQry = "Select InfoQuery from mmm_mst_Groupmapsettings  where GroupName='" & ddlGroup.SelectedItem.Text & "' and Eid=" & Session("Eid") & " and DocType='" & doc & "'"
            ds = obj.EcecDataSet(settingsQry)
            Dim str = ""
            If ds.obj.Tables(0).Rows.Count > 0 Then
                str = IIf(IsDBNull(ds.obj.Tables(0).Rows(0).Item("InfoQuery")), "", ds.obj.Tables(0).Rows(0).Item("InfoQuery").ToString)
            End If
            If Not str = "" Then
                textBox.Text = str
                textBox.Visible = True
                ChkList.Visible = False
                chkInfo.Checked = True
            Else
                textBox.Visible = False
                ChkList.Visible = True
                If ddlGroup.SelectedItem.Value = "Select" Then
                    For j As Integer = 0 To ChkList.Items.Count - 1
                        ChkList.Items(j).Selected = False
                    Next
                Else
                    str = GetInfoFldMappins(doc)
                    Dim arr = str.Split(",")
                    For i As Integer = 0 To arr.Length - 1
                        For j As Integer = 0 To ChkList.Items.Count - 1
                            If ChkList.Items(j).Value = arr(i) Then
                                ChkList.Items(j).Selected = True
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If
        End If

    End Sub

    Protected Sub btnSaveEdit_Click(sender As Object, e As EventArgs) Handles btnSaveEdit.Click
        DivMsg1.InnerHtml = ""
        DivMsg1.Visible = False
        Try
            Dim chked As Boolean = False
        For i As Integer = 0 To grdEdit.Rows.Count - 1
            Dim Tid = grdEdit.DataKeys(i).Value
                Dim chk As CheckBox = DirectCast(grdEdit.Rows(i).FindControl("chk1"), CheckBox)
                Dim chkInfo As CheckBox = DirectCast(grdEdit.Rows(i).FindControl("CheckBox1"), CheckBox)
                Dim chkList = DirectCast(grdEdit.Rows(i).FindControl("chkList"), CheckBoxList)
                Dim textBox = DirectCast(grdEdit.Rows(i).FindControl("txtQuery"), TextBox)
                Dim ddl As DropDownList = CType(grdEdit.Rows(i).FindControl("ddlIcon"), DropDownList)
                Dim doc As String = (DirectCast(grdEdit.Rows(i).FindControl("lbl1"), Label)).Text
                Dim qry As String = ""

                Dim strQry = ""
                If chkInfo.Checked Then
                    strQry = textBox.Text
                    strQry = Replace(strQry, "'", "''")
                Else
                    If chk.Checked Then
                        Dim fld = ""
                        For j As Integer = 0 To chkList.Items.Count - 1
                            If chkList.Items(j).Selected = True Then
                                fld &= chkList.Items(j).Value.ToString() & IIf(j = chkList.Items.Count - 1, "", ",")
                            End If
                        Next
                        If fld.Length > 0 Then
                            fld = fld.Substring(0, fld.Length - 1)
                            Dim updtQry = "Update MMM_MST_FIELDS set kc_Value='" & fld & "' where  Eid=" & Session("Eid") & " and Documenttype='" & doc & "' and FieldType='Geo Point'"
                            Dim dal As New BpmHelper()
                            dal.ExecDML(updtQry)
                        End If
                    Else
                        
                    End If
                End If

            If chk.Checked And Not Tid = 0 Then
                    qry = "update mmm_mst_Groupmapsettings set DocType='" & doc & "', IconName='" & ddl.SelectedItem.Text & "', InfoQuery='" & strQry & "'"
                    qry = qry & " where Tid=" & Tid & " and GroupName='" & ddlGroup.SelectedItem.Text & "' and Eid=" & Session("Eid")
            ElseIf chk.Checked And Tid = 0 Then
                    qry = "insert into mmm_mst_GroupMapSettings(Eid,GroupName,DocType,IconName, InfoQuery) "
                    qry &= " Values(" & Session("Eid") & ",'" & ddlGroup.SelectedItem.Text & "', '" & doc & "',  '" & ddl.SelectedItem.Text & "', '" & strQry & "')"
            ElseIf Not chk.Checked And Not Tid = 0 Then
                qry = "Delete from mmm_mst_GroupMapSettings where Tid=" & Tid & " and Eid=" & Session("Eid") & " and GroupName='" & ddlGroup.SelectedItem.Text & "'"
            Else
                Continue For
            End If
            Dim cmd As New SqlCommand(qry, con)
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim roAff = cmd.ExecuteNonQuery()
            con.Close()
                chked = True
            Next
            Dim str As String = "<div class='green'>"
            str &= "Data saved successfully."
            str &= "</div>"
            DivMsg1.Visible = True
            DivMsg1.InnerHtml = str

            ddlGroup_SelectedIndexChanged(Nothing, Nothing)

        Catch ex As Exception
            Dim str As String = "<div class='red'>"
            str &= "Server error. Please contact your system administrator."
            str &= "</div>"
            DivMsg1.Visible = True
            DivMsg1.InnerHtml = str
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        
    End Sub
   
    Protected Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs)
        Dim gvr As GridViewRow = DirectCast(DirectCast(sender, Control).Parent.Parent, GridViewRow)
        Dim index_row As Integer = gvr.RowIndex
        Dim chk = DirectCast(sender, CheckBox)
        Dim chkList = DirectCast(gvr.FindControl("chkList"), CheckBoxList)
        Dim textBox = DirectCast(gvr.FindControl("txtQuery"), TextBox)

        If chk.Checked Then
            textBox.Visible = True
            chkList.Visible = False
        Else
            textBox.Visible = False
            chkList.Visible = True
        End If

        bindImage()
    End Sub

    Protected Sub CheckBox1_CheckedChanged1(sender As Object, e As EventArgs)
        Dim gvr As GridViewRow = DirectCast(DirectCast(sender, Control).Parent.Parent, GridViewRow)
        Dim index_row As Integer = gvr.RowIndex
        Dim chk = DirectCast(sender, CheckBox)
        Dim chkList = DirectCast(gvr.FindControl("chkList"), CheckBoxList)
        Dim textBox = DirectCast(gvr.FindControl("txtQuery"), TextBox)

        If chk.Checked Then
            textBox.Visible = True
            chkList.Visible = False
        Else
            textBox.Visible = False
            chkList.Visible = True
        End If

        bindImage()
    End Sub

    Private Function GetInfoFldMappins(Doc As String) As String
        Dim Qry = "Select kc_Value from MMM_MST_FIELDS where Eid=" & Session("Eid") & " and Documenttype='" & Doc & "' and FieldType='Geo Point'"
        Dim obj As New BpmHelper()
        Dim ds = obj.EcecDataSet(Qry)
        Return ds.obj.Tables(0).Rows(0).Item("kc_Value").ToString
    End Function

End Class
