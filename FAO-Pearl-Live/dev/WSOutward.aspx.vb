Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Globalization
Partial Class WSOutward
    Inherits System.Web.UI.Page


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

            ' binddldoctype()
            getSearchResult()

        End If
    End Sub
    Private Sub binddldoctype()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("Select * from mmm_mst_forms where eid=" & Session("EID") & "and formsource='MENU DRIVEN' and enablews=1 ", con)
        Dim ds As New DataSet
        da.Fill(ds, "formname")

        ddldoctype.DataSource = ds.Tables("formname")
        ddldoctype.DataTextField = "Formname"
        ddldoctype.DataValueField = "Formid"
        ddldoctype.DataBind()
        ddldoctype.Items.Insert(0, New ListItem("Select"))
        con.Close()
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        ddldoctype.Items.Clear()
        ddltype.Items.Clear()
        lblMsgEdit.Text = ""
        txtdisplaylist.Text = ""
        btnActEdit.Text = "Save"
        txtParameterList.Text = ""
        txtParameterSeprator.Text = ""
        txtRemarks.Text = ""
        txtURI.Text = ""
        binddldoctype()
        ddltype.Items.Insert(0, "Select")
        ddltype.Items.Insert(1, "Get")
        ddltype.Items.Insert(2, "Post")
        ddltype.DataBind()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        '  ddldoctype.Items.Clear()
        ddltype.Items.Clear()
        lblMsgEdit.Text = ""
        ddltype.Items.Insert(0, "Select")
        ddltype.Items.Insert(1, "Get")
        ddltype.Items.Insert(2, "Post")
        ddltype.DataBind()
        binddldoctype()
        Dim test1 As String = ddldoctype.Items.Count
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("TID") = Tid
        txtdatef.Text = row.Cells(6).Text
        txtParameterList.Text = row.Cells(7).Text
        txtParameterSeprator.Text = row.Cells(5).Text
        txtURI.Text = row.Cells(4).Text
        txtRemarks.Text = row.Cells(8).Text

        ddldoctype.SelectedIndex = ddldoctype.Items.IndexOf(ddldoctype.Items.FindByText(row.Cells(3).Text))
        ddltype.SelectedIndex = ddltype.Items.IndexOf(ddltype.Items.FindByText(row.Cells(2).Text))
        ddlwstype.SelectedIndex = ddlwstype.Items.IndexOf(ddlwstype.Items.FindByText(row.Cells(1).Text))

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count > 0 Then
            Dim addall As String = ""
            Dim fm As String = ""
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim dn As String = UCase(ds.Tables("data").Rows(i).Item("displayname").ToString())
                Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                Dim dty As String = ds.Tables("data").Rows(i).Item("documenttype").ToString()


                addall = addall & "{" & dn & "}," & vbLf
                'Dim d3 As String = ""
                'Dim dw As String = ""
                'If UCase(mv) = "MASTER VALUED" Then
                '    Dim ss As String() = DD.ToString().Split("-")
                '    d3 = "select substring((select ',{' + displayname + '}' FROM (select distinct  displayname from mmm_mst_fields f where eid=" & Session("EID") & " and DocumentType='" & ss(1).ToString() & "' ) AR FOR XML PATH('')),2,200000) As Displayname"
                '    da.SelectCommand.CommandText = d3
                '    da.Fill(ds, "mv")
                '    addall = addall & ds.Tables("mv").Rows(0).Item("displayname").ToString() & ","
                'End If

            Next
            txtdisplaylist.Text = addall.ToString()
        End If



        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        If ddltype.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Type!!"
            Exit Sub
        End If

        If ddldoctype.SelectedItem.Text = "Select" Then
            lblMsgEdit.Text = "* Please Select Document Type!!"
            Exit Sub
        End If

        If Trim(txtParameterList.Text) = "" Then
            lblMsgEdit.Text = "* Parameter List can not be blank!!"
            Exit Sub

        End If

        If Trim(txtParameterSeprator.Text) = "" Then
            lblMsgEdit.Text = "* Parameter Seperator can not be blank!!"
            Exit Sub
        End If
        If Trim(txtURI.Text) = "" Then
            lblMsgEdit.Text = "* URI can not be blank!!"
            Exit Sub
        End If

        If btnActEdit.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspinsertWSOutward", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@wstype", Trim(ddlwstype.SelectedItem.Text))
            oda.SelectCommand.Parameters.AddWithValue("@type", Trim(ddltype.SelectedItem.Text))
            oda.SelectCommand.Parameters.AddWithValue("@doctype", Trim(ddldoctype.SelectedItem.Text))
            oda.SelectCommand.Parameters.AddWithValue("@URI", Trim(txtURI.Text))
            oda.SelectCommand.Parameters.AddWithValue("@Paralist", Trim(txtParameterList.Text))
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ParaSep", Trim(txtParameterSeprator.Text))
            oda.SelectCommand.Parameters.AddWithValue("@Remarks", Trim(txtRemarks.Text))
            oda.SelectCommand.Parameters.AddWithValue("@dateformat", Trim(txtdatef.Text))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblMsgEdit.Text = oda.SelectCommand.ExecuteScalar()
            Me.btnEdit_ModalPopupExtender.Hide()
            lblMsgupdate.Text = "Created Successfully"

            con.Close()
            oda.Dispose()
            con.Dispose()
            gvData.DataBind()

        Else


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("uspUpdateWSOutward", con)
            oda1.SelectCommand.CommandType = CommandType.StoredProcedure
            oda1.SelectCommand.Parameters.AddWithValue("@wstype", Trim(ddlwstype.SelectedItem.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@type", Trim(ddltype.SelectedItem.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@doctype", Trim(ddldoctype.SelectedItem.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@URI", Trim(txtURI.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@Paralist", Trim(txtParameterList.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            oda1.SelectCommand.Parameters.AddWithValue("@ParaSep", Trim(txtParameterSeprator.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@Remarks", Trim(txtRemarks.Text))
            oda1.SelectCommand.Parameters.AddWithValue("@tid", ViewState("TID"))
            oda1.SelectCommand.Parameters.AddWithValue("@dateformat", txtdatef.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda1.SelectCommand.ExecuteNonQuery()
            lblMsgupdate.Text = "Updated Successfully"

            updatePanelEdit.Update()
            btnEdit_ModalPopupExtender.Hide()
            con.Close()
            oda1.Dispose()
            con.Dispose()
        End If
        getSearchResult()
    End Sub

    'Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
    '    gvData.DataBind()
    '    If gvData.Rows.Count > 0 Then
    '        lblMsgupdate.Text = gvData.Rows.Count & " Records Found "
    '    Else
    '        lblMsgupdate.Text = " No record found "
    '    End If
    'End Sub

    Private Function getSearchResult() As DataTable
        gvData.DataBind()
    End Function


   

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        lblMsgDelFolder.ForeColor = Drawing.Color.Red
        lblMsgDelFolder.Text = "Are You Sure Want to Delete this service"
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActEdit.Text = "Update"
        ViewState("TID") = Tid
        getSearchResult()
        Me.updatePanelEdit.Update()
        Me.btnDelFolder_ModalPopupExtender.Show()
    End Sub

    Protected Sub DelFile(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspdeleteWSOutward", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@Tid", ViewState("TID"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        lblMsgEdit.Text = oda.SelectCommand.ExecuteScalar()

        getSearchResult()
        con.Close()
        oda.Dispose()
        con.Dispose()
        btnDelFolder_ModalPopupExtender.Hide()
    End Sub

    
    'Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldoctype.SelectedIndexChanged
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)

    '    Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "data")

    '    If ds.Tables("data").Rows.Count > 0 Then
    '        Dim addall As String = ""
    '        Dim fm As String = ""
    '        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
    '            Dim dn As String = UCase(ds.Tables("data").Rows(i).Item("displayname").ToString())
    '            Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
    '            Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
    '            Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
    '            Dim dty As String = ds.Tables("data").Rows(i).Item("documenttype").ToString()

    '            addall = addall & "{" & dn & "},"
    '            Dim d3 As String = ""
    '            Dim dw As String = ""
    '            If UCase(mv) = "MASTER VALUED" Then
    '                Dim ss As String() = DD.ToString().Split("-")
    '                d3 = "select substring((select ',{' + displayname + '}' FROM (select distinct  displayname from mmm_mst_fields f where eid=" & Session("EID") & " and DocumentType='" & ss(1).ToString() & "' ) AR FOR XML PATH('')),2,200000) As Displayname"
    '                da.SelectCommand.CommandText = d3
    '                da.Fill(ds, "mv")
    '                addall = addall & ds.Tables("mv").Rows(0).Item("displayname").ToString() & ","
    '            End If

    '        Next
    '        txtdisplaylist.Text = addall.ToString()

    '    End If


    'End Sub


    Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldoctype.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count > 0 Then
            Dim addall As String = ""
            Dim fm As String = ""
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim dn As String = UCase(ds.Tables("data").Rows(i).Item("displayname").ToString())
                Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                Dim dty As String = ds.Tables("data").Rows(i).Item("documenttype").ToString()

                addall = addall & "{" & dn & "}," & vbLf
                'Dim d3 As String = ""
                'Dim dw As String = ""
                'If UCase(mv) = "MASTER VALUED" Then
                '    Dim ss As String() = DD.ToString().Split("-")
                '    d3 = "select substring((select ',{' + displayname + '}' FROM (select distinct  displayname from mmm_mst_fields f where eid=" & Session("EID") & " and DocumentType='" & ss(1).ToString() & "' ) AR FOR XML PATH('')),2,200000) As Displayname"
                '    da.SelectCommand.CommandText = d3
                '    da.Fill(ds, "mv")
                '    addall = addall & ds.Tables("mv").Rows(0).Item("displayname").ToString() & ","
                'End If

            Next
            txtdisplaylist.Text = addall.ToString()

        End If


    End Sub

    
End Class
