Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Partial Class WSInward
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
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("Select * from mmm_mst_forms where eid=" & Session("EID") & "and formsource ='MENU DRIVEN' order by Formname ", con)
            Dim ds As New DataSet
            da.Fill(ds, "formname")
            If ds.Tables("formname").Rows.Count > 0 Then


                da.SelectCommand.CommandText = "select apikey from mmm_mst_entity where eid=" & Session("EId") & ""
                'da.Fill(ds, "key")
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                ViewState("key") = da.SelectCommand.ExecuteScalar()
                ViewState("keyedit") = da.SelectCommand.ExecuteScalar()

                ddldtype.DataSource = ds.Tables("formname")
                ddldtype.DataTextField = "Formname"
                ddldtype.DataValueField = "Formid"
                ddldtype.DataBind()
                ddldtype.Items.Insert(0, New ListItem("Select"))

                DropDownList1.DataSource = ds.Tables("formname")
                DropDownList1.DataTextField = "Formname"
                DropDownList1.DataValueField = "Formid"
                DropDownList1.DataBind()
                DropDownList1.Items.Insert(0, New ListItem("Select"))


                da.SelectCommand.CommandText = "Select * from mmm_mst_forms where eid=" & Session("EID") & "and formsource ='ACTION DRIVEN' order by Formname "
                da.Fill(ds, "ACTION")
                If ds.Tables("ACTION").Rows.Count > 0 Then
                    ddlactdr.DataSource = ds.Tables("ACTION")
                    ddlactdr.DataTextField = "Formname"
                    ddlactdr.DataValueField = "Formid"
                    ddlactdr.DataBind()
                    ddlactdr.Items.Insert(0, New ListItem("Select"))

                End If

                da.SelectCommand.CommandText = "Select formname,formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='Detail Form'"
                da.SelectCommand.CommandType = CommandType.Text
                da.Fill(ds, "child")
                If ds.Tables("child").Rows.Count > 0 Then
                    ddlchild.DataSource = ds.Tables("child")
                    ddlchild.DataTextField = "Formname"
                    ddlchild.DataValueField = "Formid"
                    ddlchild.DataBind()
                    ddlchild.Items.Insert(0, New ListItem("Select"))
                End If



                con.Close()
            End If
            lblexample.Text = ""
            lblformat.Text = ""
        End If
    End Sub
    Protected Sub ddldtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldtype.SelectedIndexChanged

        getformat()

    End Sub



    Private Sub getformat()
        lblexample.Text = ""
        lblformat.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        If ddldtype.SelectedItem.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please Select Document Type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If
        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldtype.SelectedItem.Text & "' order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        If ds.Tables("data").Rows.Count < 1 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('There are no fields in Selected Document type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If
        da.SelectCommand.CommandText = "select * from mmm_mst_forms where eid=" & Session("EID") & " and formname ='" & ddldtype.SelectedItem.Text & "' "
        da.Fill(ds, "form")
        Dim dtype As String = ds.Tables("form").Rows(0).Item("Formtype").ToString

      
        Try
            ' below code is written to display the format
            If ds.Tables("data").Rows.Count > 0 Then
                Dim addall As String = ""
                Dim fm As String = ""
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1

                    Dim dot As String = "::<"
                    Dim dn As String = ds.Tables("data").Rows(i).Item("displayname").ToString()
                    Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                    Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                    Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                    Dim gr As String = ">|"
                    'Key$$PRMSNBPHOOP002012BJK~DOCTYPE$$Static Survey Form~Data$$
                    addall = addall & dn & dot & dn & gr
                    Dim d3 As String = ""
                    Dim dw As String = ""
                    If UCase(mv) = "MASTER VALUED" Or UCase(mv) = "SESSION VALUED" Then
                        Dim ss As String() = DD.ToString().Split("-")
                        If UCase(ss(1)).ToString <> "USER" Then
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & Session("EID") & " and tid=m." & fld & ")"
                        Else
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & Session("EID") & " and uid=m." & fld & ")"
                        End If
                        dw = dw & d3
                        fm = fm & dw & "[" & dn & "]" & ","
                    Else
                        fm = fm & fld & "[" & dn & "]" & ","
                    End If
                Next

              
                'addall = "IsAuth::<IsAuth>|" & addall


                addall = addall.Remove(addall.Length - 1)
                If ddldtype.SelectedItem.Text = "USER" Then
                    addall = "Key$$" & ViewState("key") & " ~DOCTYPE$$" & ddldtype.SelectedItem.Text & "~Data$$" & addall & "|USERNAME::<USERNAME>|EMAILID::<EMAILID>|USERROLE::<USERROLE>"
                Else

                    addall = "Key$$" & ViewState("key") & " ~DOCTYPE$$" & ddldtype.SelectedItem.Text & "~Data$$" & addall
                End If

                Dim htmlcode As String = HttpUtility.HtmlEncode(addall)
                lblformat.Text = htmlcode
                
                ' fm = "isauth[IsAuth]," & fm
                fm = fm.Remove(fm.Length - 1)
                Dim ddd As String = ""
                If UCase(dtype) = "MASTER" Then
                    If ddldtype.SelectedItem.Text = "USER" Then
                        da.SelectCommand.CommandText = "SELECT top 1  " & fm & " ,USERNAME,EMAILID,USERROLE FROM MMM_MST_USER m WHERE EID=" & Session("EID") & " AND USERROLE<>'SU' "
                    Else
                        da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_MASTER m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & ddldtype.SelectedItem.Text & "'"
                    End If

                    da.Fill(ds, "master")
                    If ds.Tables("master").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("master").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("master").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("master").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next

                        lblexample.Text = ddd.ToString
                    End If

                ElseIf UCase(dtype) = "DOCUMENT" Then
                    da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_DOC m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & ddldtype.SelectedItem.Text & "' "
                    da.Fill(ds, "document")
                    If ds.Tables("document").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("document").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("document").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next
                        lblexample.Text = ddd.ToString()
                    End If

                End If

                lble.Visible = True
                lblexample.Visible = True
                lblf.Visible = True
                lblformat.Visible = True
            End If
        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

    End Sub

    '' below given code is used for edit wsinward functinality

    'Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
    '    txtuk.Text = String.Empty
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim Tid As Integer = Convert.ToString(Me.gvedit.DataKeys(row.RowIndex).Value)
    '    btnActEdit.Text = "Update"
    '    ViewState("TID") = Tid

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("Select * from mmm_mst_forms where eid=" & Session("EID") & " and formsource <>'action driven' and formid=" & Tid & "", con)
    '    Dim ds As New DataSet
    '    da.Fill(ds, "Edit")

    '    If ds.Tables("Edit").Rows.Count > 0 Then
    '        lblfn.Text = ds.Tables("Edit").Rows(0).Item("FormName").ToString()
    '        bindchk()
    '        If Convert.ToString(ds.Tables("Edit").Rows(0).Item("UniqueKeys")).Length > 0 Then
    '            Dim ukey As String = Convert.ToString(ds.Tables("Edit").Rows(0).Item("UniqueKeys")).ToString()
    '            If ukey.Contains(",") Then
    '                Dim str As String() = Convert.ToString(ds.Tables("Edit").Rows(0).Item("UniqueKeys")).Split(",")
    '                Dim a As String = ""
    '                For i As Integer = 0 To str.Length - 1
    '                    a = a & "'" & str(i).ToString & "',"
    '                Next
    '                a = a.Remove(a.Length - 1)
    '                ' da.SelectCommand.CommandText = "Select displayname,fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & lblfn.Text & "' and fieldmapping in (" & a.ToString() & ")"
    '                da.SelectCommand.CommandText = "SELECT SUBSTRING((SELECT ',' + s.fieldmapping FROM mmm_mst_fields s where eid=" & Session("EID") & " and documenttype='" & lblfn.Text & "' and fieldmapping in (" & a.ToString() & ") ORDER BY displayName FOR XML PATH('')),2,200000) AS fieldmapping,SUBSTRING((SELECT ',' + s.displayname FROM mmm_mst_fields s where eid=" & Session("EID") & " and documenttype='" & lblfn.Text & "' and fieldmapping in (" & a.ToString() & ") ORDER BY displayName FOR XML PATH('')),2,200000) AS displayname"
    '                da.Fill(ds, "val")
    '                If ds.Tables("val").Rows.Count > 0 Then
    '                    txtuk.Text = ds.Tables("val").Rows(0).Item("displayname").ToString()
    '                    Dim stt As String() = Convert.ToString(txtuk.Text).Split(",")
    '                    If stt.Length > 0 Then
    '                        'Dim StrList As List(Of [String]) = New List(Of String)()
    '                        For i As Integer = 0 To chkflds.Items.Count - 1
    '                            For j As Integer = 0 To stt.Length - 1
    '                                If stt(j).ToString = chkflds.Items(i).Text.ToString() Then
    '                                    chkflds.Items(i).Selected = True
    '                                Else
    '                                    chkflds.Items(i).Selected = False
    '                                End If
    '                            Next
    '                        Next
    '                    End If
    '                End If

    '            End If
    '        End If


    '    End If
    '    ' Me.updatePanelEdit.Update()
    '    Me.btnEdit_ModalPopupExtender.Show()
    'End Sub

    'Protected Sub bindchk()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet
    '    Try
    '        da.SelectCommand.CommandText = "Select displayname,fieldmapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & Trim(lblfn.Text) & "'"
    '        da.Fill(ds, "data")
    '        chkflds.DataSource = ds.Tables("data")
    '        chkflds.DataTextField = "displayname"
    '        chkflds.DataValueField = "fieldmapping"
    '        chkflds.DataBind()

    '    Catch ex As Exception
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If

    '    End Try



    'End Sub

    'Protected Sub chkflds_SelectedIndexChanged(sender As Object, e As EventArgs) Handles chkflds.SelectedIndexChanged
    '    Dim YrStrListtext As List(Of [String]) = New List(Of String)()
    '    Dim YrStrListVal As List(Of [String]) = New List(Of String)()

    '    For Each item As ListItem In chkflds.Items
    '        If item.Selected Then
    '            YrStrListVal.Add(item.Value)
    '            YrStrListtext.Add(item.Text)

    '        Else
    '        End If
    '    Next
    '    Dim YrStrval As [String] = [String].Join(",", YrStrListVal.ToArray())
    '    Dim YrStrtext As [String] = [String].Join(",", YrStrListtext.ToArray())

    '    txtuk.Text = YrStrtext.ToString()
    '    ViewState("uval") = YrStrval.ToString()


    'End Sub


    'Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
    '    If txtuk.Text = "" Then
    '        lblMsgEdit.Text = "* Please select a Key"
    '        Exit Sub
    '    End If

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("USPupdate_uniquekey", con)

    '    Try
    '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '        oda.SelectCommand.Parameters.AddWithValue("@doctype", Trim(lblfn.Text))
    '        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
    '        oda.SelectCommand.Parameters.AddWithValue("@ukeys", Trim(ViewState("uval")))
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        '
    '        lblss.Text = oda.SelectCommand.ExecuteScalar()

    '        Me.btnEdit_ModalPopupExtender.Hide()
    '        gvedit.DataBind()
    '    Catch ex As Exception
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not oda Is Nothing Then
    '            oda.Dispose()
    '        End If

    '    End Try
    'End Sub

    Private Sub getformatedit()

        Label6.Text = ""
        Label8.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        If DropDownList1.SelectedItem.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please Select Document Type!!!! ');", True)
            Exit Sub
        End If
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        da.SelectCommand.CommandText = "select * from mmm_mst_forms where eid=" & Session("EID") & " and formname ='" & DropDownList1.SelectedItem.Text.Trim() & "' "
        da.Fill(ds, "form")
        Dim dtype As String = ds.Tables("form").Rows(0).Item("Formtype").ToString
        Dim ab As String() = ds.Tables("form").Rows(0).Item("UniqueKeys").ToString().Split(",")
        Dim x As String = ""
        Dim ax As String = ""
        If ab.Length > 0 Then
            For j As Integer = 0 To ab.Length - 1
                ax = ax & "'" & ab(j).ToString() & "',"
            Next
            ax = ax.Remove(ax.Length - 1)
            x = "Union Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & DropDownList1.SelectedItem.Text & "' and fieldmapping in (" & ax.ToString() & ")"
        End If
        da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & DropDownList1.SelectedItem.Text.Trim() & "' and enableedit=1 " & x.ToString() & ""
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count < 1 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please enableedit for fields or there are no fields in selected Document type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If



        Try
            ' below code is written to display the format
            If ds.Tables("data").Rows.Count > 0 Then
                Dim addall As String = ""
                Dim fm As String = ""
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    Dim dot As String = "::<"
                    Dim dn As String = ds.Tables("data").Rows(i).Item("displayname").ToString()
                    Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                    Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                    Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                    Dim gr As String = ">|"
                    'Key$$PRMSNBPHOOP002012BJK~DOCTYPE$$Static Survey Form~Data$$
                    addall = addall & dn & dot & dn & gr
                    Dim d3 As String = ""
                    Dim dw As String = ""
                    If UCase(mv) = "MASTER VALUED" Or UCase(mv) = "SESSION VALUED" Then
                        Dim ss As String() = DD.ToString().Split("-")
                        If UCase(ss(1)).ToString <> "USER" Then
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & Session("EID") & " and tid=m." & fld & ")"
                        Else
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & Session("EID") & " and uid=m." & fld & ")"
                        End If
                        dw = dw & d3
                        fm = fm & dw & "[" & dn & "]" & ","
                    Else
                        fm = fm & fld & "[" & dn & "]" & ","
                    End If
                Next

                addall = "ISAUTH::<ISAUTH>|" & addall

                addall = addall.Remove(addall.Length - 1)
                If ddldtype.SelectedItem.Text = "USER" Then
                    addall = "Key$$" & ViewState("key") & " ~DOCTYPE$$" & DropDownList1.SelectedItem.Text & "~Data$$" & addall & "|USERNAME::<USERNAME>|EMAILID::<EMAILID>|USERROLE::<USERROLE>"
                Else
                    addall = "Key$$" & ViewState("key") & " ~DOCTYPE$$" & DropDownList1.SelectedItem.Text & "~Data$$" & addall
                End If

                Dim htmlcode As String = HttpUtility.HtmlEncode(addall)
                Label6.Text = htmlcode

                fm = "isauth[IsAuth]," & fm
                fm = fm.Remove(fm.Length - 1)
                Dim ddd As String = ""
                If UCase(dtype) = "MASTER" Then
                    If ddldtype.SelectedItem.Text = "USER" Then
                        da.SelectCommand.CommandText = "SELECT top 1  " & fm & " ,USERNAME,EMAILID,USERROLE FROM MMM_MST_USER m WHERE EID=" & Session("EID") & " AND USERROLE<>'SU' "
                    Else
                        da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_MASTER m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & DropDownList1.SelectedItem.Text & "'"
                    End If

                    da.Fill(ds, "master")
                    If ds.Tables("master").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("master").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("master").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("master").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next

                        Label8.Text = ddd.ToString()
                    End If

                ElseIf UCase(dtype) = "DOCUMENT" Then
                    da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_DOC m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & DropDownList1.SelectedItem.Text & "' "
                    da.Fill(ds, "document")
                    If ds.Tables("document").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("document").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("document").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next
                        Label8.Text = ddd.ToString()
                    End If



                End If

                Label5.Visible = True
                Label6.Visible = True
                Label7.Visible = True
                Label8.Visible = True
            End If
        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

    End Sub

    

    Private Sub getformataction()
        lblexample.Text = ""
        lblformat.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        If ddlactdr.SelectedItem.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please Select Document Type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If
        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlactdr.SelectedItem.Text & "'  order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        If ds.Tables("data").Rows.Count < 1 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('There are no fields in Selected Document type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If
        da.SelectCommand.CommandText = "select * from mmm_mst_forms where eid=" & Session("EID") & " and formname ='" & ddlactdr.SelectedItem.Text & "' "
        da.Fill(ds, "form")
        Dim dtype As String = ds.Tables("form").Rows(0).Item("Formtype").ToString
        Dim Fsource As String = ds.Tables("form").Rows(0).Item("Formsource").ToString()
        Dim Fevent As String = ds.Tables("form").Rows(0).Item("eventname").ToString()
        Dim Fstatus As String = ds.Tables("form").Rows(0).Item("curstatus").ToString()


        Try
            ' below code is written to display the format
            If ds.Tables("data").Rows.Count > 0 Then
                Dim addall As String = ""
                Dim fm As String = ""
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1

                    Dim dot As String = "::<"
                    Dim dn As String = ds.Tables("data").Rows(i).Item("displayname").ToString()
                    Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                    Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                    Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                    Dim gr As String = ">|"
                    'Key$$PRMSNBPHOOP002012BJK~DOCTYPE$$Static Survey Form~Data$$
                    addall = addall & dn & dot & dn & gr
                    Dim d3 As String = ""
                    Dim dw As String = ""
                    If UCase(mv) = "MASTER VALUED" Or UCase(mv) = "SESSION VALUED" Then
                        Dim ss As String() = DD.ToString().Split("-")
                        If UCase(ss(1)).ToString <> "USER" Then
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & Session("EID") & " and tid=m." & fld & ")"
                        Else
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & Session("EID") & " and uid=m." & fld & ")"
                        End If
                        dw = dw & d3
                        fm = fm & dw & "[" & dn & "]" & ","
                    Else
                        fm = fm & fld & "[" & dn & "]" & ","
                    End If
                Next

                addall = addall

                addall = addall.Remove(addall.Length - 1)
                If ddldtype.SelectedItem.Text = "USER" Then
                    addall = "Key$$" & ViewState("key") & " ~DOCTYPE$$" & ddlactdr.SelectedItem.Text & "~DOCID$$<DOCID>" & "~Data$$" & addall & "|USERNAME::<USERNAME>|EMAILID::<EMAILID>|USERROLE::<USERROLE>"
                Else

                    addall = "Key$$" & ViewState("key") & " ~DOCTYPE$$" & ddlactdr.SelectedItem.Text & "~DOCID$$<DOCID>" & "~Data$$" & addall
                End If

                Dim htmlcode As String = HttpUtility.HtmlEncode(addall)
                lblacrf.Text = htmlcode
                
                fm = "tid[DOCID]," & fm
                fm = fm.Remove(fm.Length - 1)
                Dim ddd As String = ""
                If UCase(dtype) = "MASTER" Then
                    If ddldtype.SelectedItem.Text = "USER" Then
                        da.SelectCommand.CommandText = "SELECT top 1  " & fm & " ,USERNAME,EMAILID,USERROLE FROM MMM_MST_USER m WHERE EID=" & Session("EID") & " AND USERROLE<>'SU' "
                    Else
                        da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_MASTER m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & ddlactdr.SelectedItem.Text & "'"
                    End If

                    da.Fill(ds, "master")
                    If ds.Tables("master").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("master").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("master").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("master").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next

                        lblcte.Text = ddd.ToString()
                    End If

                ElseIf UCase(dtype) = "DOCUMENT" Then
                    da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_DOC m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & Fevent.ToString() & "' and curstatus='" & Fstatus.ToString() & "'"
                    da.Fill(ds, "document")
                    If ds.Tables("document").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("document").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("document").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next
                        lblcte.Text = ddd.ToString()
                    End If

                End If

                Label13.Visible = True
                lblacrf.Visible = True
                Label15.Visible = True
                lblcte.Visible = True
            End If
        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

    End Sub
    Private Sub getformatchild()
        lblchildexample.Text = ""
        lblchildFormat.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        If ddlchild.SelectedItem.Text = "Select" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please Select Document Type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If
        Dim da As New SqlDataAdapter("Select * from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlchild.SelectedItem.Text & "'  order by displayname", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        If ds.Tables("data").Rows.Count < 1 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('There are no fields in Selected Document type!!!! ');window.location='WSInward.aspx';", True)
            Exit Sub
        End If
        da.SelectCommand.CommandText = "select * from mmm_mst_forms where eid=" & Session("EID") & " and formname ='" & ddlchild.SelectedItem.Text & "' "
        da.Fill(ds, "form")
        Dim dtype As String = ds.Tables("form").Rows(0).Item("Formtype").ToString
        Dim Fsource As String = ds.Tables("form").Rows(0).Item("Formsource").ToString()
        Dim Fevent As String = ds.Tables("form").Rows(0).Item("eventname").ToString()
        Dim Fstatus As String = ds.Tables("form").Rows(0).Item("curstatus").ToString()


        Try
            ' below code is written to display the format
            If ds.Tables("data").Rows.Count > 0 Then
                Dim addall As String = ""
                Dim formname As String = ddlchild.SelectedItem.Text.ToString().Trim()
                Dim fm As String = ""
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1

                    Dim line As String = "()"
                    Dim dot As String = "<>"
                    Dim dn As String = ds.Tables("data").Rows(i).Item("displayname").ToString()
                    Dim fld As String = ds.Tables("data").Rows(i).Item("fieldmapping").ToString()
                    Dim mv As String = ds.Tables("data").Rows(i).Item("dropdowntype").ToString()
                    Dim DD As String = ds.Tables("data").Rows(i).Item("dropdown").ToString()
                    Dim gr As String = "()"
                    'Key$$PRMSNBPHOOP002012BJK~DOCTYPE$$Static Survey Form~Data$$
                    addall = addall & line & dn & dot & "{" & dn & "}"
                    Dim d3 As String = ""
                    Dim dw As String = ""
                    If UCase(mv) = "MASTER VALUED" Or UCase(mv) = "SESSION VALUED" Then
                        Dim ss As String() = DD.ToString().Split("-")
                        If UCase(ss(1)).ToString <> "USER" Then
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & Session("EID") & " and tid=m." & fld & ")"
                        Else
                            d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & Session("EID") & " and uid=m." & fld & ")"
                        End If
                        dw = dw & d3
                        fm = fm & dw & "[" & dn & "]" & ","
                    Else
                        fm = fm & fld & "[" & dn & "]" & ","
                    End If
                    addall = addall
                Next

                addall = formname & "{}" & addall & "{}"

                'addall = addall.Remove(addall.Length - 2)
                If ddldtype.SelectedItem.Text = "USER" Then
                    addall = addall & "|USERNAME::<USERNAME>|EMAILID::<EMAILID>|USERROLE::<USERROLE>"
                Else
                    addall = addall
                End If

                Dim htmlcode As String = HttpUtility.HtmlEncode(addall)
                lblchildFormat.Text = htmlcode

                fm = "tid[DOCID]," & fm
                fm = fm.Remove(fm.Length - 1)
                Dim ddd As String = ""
                If UCase(dtype) = "MASTER" Then
                    If ddldtype.SelectedItem.Text = "USER" Then
                        da.SelectCommand.CommandText = "SELECT top 1  " & fm & " ,USERNAME,EMAILID,USERROLE FROM MMM_MST_USER m WHERE EID=" & Session("EID") & " AND USERROLE<>'SU' "
                    Else
                        da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_MASTER m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & ddlchild.SelectedItem.Text & "'"
                    End If

                    da.Fill(ds, "master")
                    If ds.Tables("master").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("master").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("master").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("master").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next

                        lblchildexample.Text = ddd.ToString()
                    End If

                ElseIf UCase(dtype) = "DOCUMENT" Then
                    da.SelectCommand.CommandText = "SELECT top 1 " & fm & " FROM MMM_MST_DOC m WHERE EID=" & Session("EID") & " AND DOCUMENTTYPE='" & ddlchild.SelectedItem.Text & "' "
                    da.Fill(ds, "document")
                    If ds.Tables("document").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("document").Columns.Count - 1
                            Dim a As String = "<" & ds.Tables("document").Columns(i).ColumnName.ToString() & ">"
                            addall = addall.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                            ddd = HttpUtility.HtmlEncode(addall).ToString()
                        Next
                        lblchildexample.Text = ddd.ToString()
                    End If

                End If

              
            End If
            lblchildform.Text = "{} Denotes Row Seprator, () Denotes FieldSeprator"
            Label18.Visible = True
            lblchildFormat.Visible = True
            Label20.Visible = True
            lblchildexample.Visible = True
        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList1.SelectedIndexChanged
        getformatedit()

    End Sub

    Protected Sub ddlactdr_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlactdr.SelectedIndexChanged
        getformataction()
    End Sub

    Protected Sub ddlchild_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlchild.SelectedIndexChanged
        getformatchild()
    End Sub
End Class
