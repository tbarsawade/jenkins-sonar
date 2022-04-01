Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Globalization
Partial Class ReallocationRights
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            binddldoctype()
            xrow.Visible = False
        End If
    End Sub
    Protected Sub page_init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Session("getresultdata") Is Nothing Then
            Dim dt As DataTable = CType(Session("getresultdata"), DataTable)
            bindgrid(dt)
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
    Private Sub binddldoctype()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select distinct doctype from mmm_mst_Reallocation where eid=" & Session("EID") & " and role='" & Session("USERROLE") & "' ", con)
        '        Dim da As New SqlDataAdapter("Select * from mmm_mst_Reallocation where eid=" & Session("EID") & " and role='" & Session("USERROLE") & "'", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")
        ddldoctype.DataSource = ds.Tables("data")
        ddldoctype.DataTextField = "doctype"
        ddldoctype.DataValueField = "doctype"
        ddldoctype.DataBind()
        ddldoctype.Items.Insert(0, New ListItem("Select"))
        con.Close()
    End Sub
    Protected Sub getresult()
        ddltu.Items.Clear()
        ddlcu.Items.Clear()
        xrow.Visible = True
        btnsave.Visible = True
        gvData.Visible = True
        trdocid.Visible = True
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select cfield,remarks from mmm_mst_Reallocation where eid=" & Session("EID") & " and doctype='" & ddldoctype.SelectedItem.Text & "' and status='" & ddlstatus.SelectedItem.Text & "'", con)
        Dim dss As New DataSet
        da.Fill(dss, "cr")
        If dss.Tables("cr").Rows.Count > 0 Then
            Dim objdc As New DataClass()
            da.SelectCommand.CommandText = "Select dropdown from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim sse As String() = da.SelectCommand.ExecuteScalar().ToString.Split("-")

            Dim ff As String = String.Empty
            If sse.Length > 1 Then
                da.SelectCommand.CommandText = "Select docmapping from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & sse(1).ToString & "'"
                ff = da.SelectCommand.ExecuteScalar()
                ViewState("ff") = ff
            Else
                Dim value As String = objdc.ExecuteQryScaller("Select DDLlookupValueSource from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'")
                ff = objdc.ExecuteQryScaller("Select docmapping from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & value & "'")
                ViewState("ff") = ff
            End If
            Dim fld As String = dss.Tables("cr").Rows(0).Item("cfield").ToString()
            Dim remarks As String = dss.Tables("cr").Rows(0).Item("remarks").ToString()
            ViewState("fld") = fld.ToString()
            ViewState("remarks") = remarks.ToString()
            da.SelectCommand.CommandTimeout = 700
            da.SelectCommand.CommandText = "SELECT  DocID,[Current User],[LastTID],[ptat],[ordering],[fdate] FROM (select d.tid[DocID],(select distinct username from mmm_mst_user where eid=" & Session("EID") & " and uid= dd.userid )[Current User],dd.userid,d.lasttid[Lasttid],dd.ptat[ptat],dd.ordering[ordering],dd.fdate from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & Session("EID") & " and d.documenttype='" & ddldoctype.SelectedItem.Text & "' and d.curstatus='" & ddlstatus.SelectedItem.Text & "' and dd.userid is not null and dd.userid not in (217,363)  and " & fld & " in (select * from  InputString((select " & ff.ToString() & " from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename='" & Session("USERROLE") & "'))))A  where [current user] is not null and [Current user]<>'' "

            ' Dim da As New SqlDataAdapter("select d.tid[DocID],(select username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid))[Current User],* from mmm_mst_doc d where d.eid=" & Session("EID") & " and d.documenttype='VRF FIXED_POOL' and d.curstatus='approved' ", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")

            Session("ddlselecteddoc") = ddldoctype.SelectedItem.Text.ToString()
            Session("ddlselectedstatus") = ddlstatus.SelectedItem.Text
            Session("getresultdata") = ds.Tables("data")

            bindgrid(ds.Tables("data"))
            Call bindcu(ff.ToString())
        End If
    End Sub
    Protected Sub bindgrid(ByVal dt As DataTable)
        gvData.Caption = "No. of Records: " & dt.Rows.Count
        gvData.DataSource = dt
        gvData.DataBind()
        If ddltu.SelectedIndex <> -1 AndAlso ddltu.SelectedIndex <> 0 AndAlso ddlcu.SelectedIndex <> -1 AndAlso ddlcu.SelectedIndex <> 0 Then
            For Each row As GridViewRow In gvData.Rows
                Dim ddl As DropDownList = DirectCast(row.FindControl("ddluser"), DropDownList)
                '    ddl.SelectedValue = ddltu.SelectedValue.ToString()
                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(ddltu.SelectedItem.Text.ToString()))
            Next
        End If
    End Sub
    Protected Sub bindcu(ByVal st As String)
        If ddlcu.Items.Count = 0 Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("select distinct (select distinct username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid and userid is not null and userid not in (217,363) ))[Current User],dd.userid[UID]  from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & Session("EID") & " and d.documenttype='" & ddldoctype.SelectedItem.Text & "' and d.curstatus='" & ddlstatus.SelectedItem.Text & "' and " & ViewState("fld") & " in (Select * from inputstring((select " & st & " from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename='" & Session("USERROLE") & "'))) ", con)
            'Dim da As New SqlDataAdapter("select distinct (select distinct username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid and userid is not null and userid not in (217,363) ))[Current User],dd.userid[UID]  from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & Session("EID") & " and d.documenttype='" & ddldoctype.SelectedItem.Text & "' and d.curstatus='" & ddlstatus.SelectedItem.Text & "' and " & ViewState("fld") & " in (select fld4 from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & ") ", con)
            da.SelectCommand.CommandTimeout = 600
            Dim ds As New DataSet
            da.Fill(ds, "dt")
            ddlcu.DataSource = ds.Tables("dt")
            ddlcu.DataTextField = "Current User"
            ddlcu.DataValueField = "UID"
            ddlcu.DataBind()
            ddlcu.Items.Insert(0, New ListItem("Select"))
            xrow.Visible = True
        End If
        'If ddltu.Items.Contains(New ListItem("Select")) = False Then
        '    ddltu.Items.Insert(0, New ListItem("Select"))
        'End If
    End Sub
    Protected Sub SearchDocID(Optional ByVal docid As String = "0")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select cfield,remarks from mmm_mst_Reallocation where eid=" & Session("EID") & " and doctype='" & ddldoctype.SelectedItem.Text & "' and status='" & ddlstatus.SelectedItem.Text & "'", con)
        Dim dss As New DataSet
        da.Fill(dss, "cr")

        If dss.Tables("cr").Rows.Count > 0 Then
            Dim objdc As New DataClass()
            da.SelectCommand.CommandText = "Select dropdown from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim sse As String() = da.SelectCommand.ExecuteScalar().ToString.Split("-")

            Dim ff As String = String.Empty
            If sse.Length > 1 Then
                da.SelectCommand.CommandText = "Select docmapping from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & sse(1).ToString & "'"
                ff = da.SelectCommand.ExecuteScalar()

                ViewState("ff") = ff
            Else
                Dim value As String = objdc.ExecuteQryScaller("Select DDLlookupValueSource from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'")
                ff = objdc.ExecuteQryScaller("Select docmapping from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & value & "'")
                ViewState("ff") = ff
            End If

            Dim fld As String = dss.Tables("cr").Rows(0).Item("cfield").ToString()
            Dim remarks As String = dss.Tables("cr").Rows(0).Item("remarks").ToString()
            ViewState("fld") = fld.ToString()
            ViewState("remarks") = remarks.ToString()
            da.SelectCommand.CommandTimeout = 700
            Dim query As String = "SELECT  DocID,[Current User],[LastTID],[ptat],[ordering],[fdate] FROM (select d.tid[DocID],(select distinct username from mmm_mst_user where eid=" & Session("EID") & " and uid= dd.userid )[Current User],dd.userid,d.lasttid[Lasttid],dd.ptat[ptat],dd.ordering[ordering],dd.fdate from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & Session("EID") & " and d.documenttype='" & ddldoctype.SelectedItem.Text & "' and d.curstatus='" & ddlstatus.SelectedItem.Text & "' and dd.userid is not null and dd.userid not in (217,363)  and " & fld & " in (select * from  InputString((select " & ff.ToString() & " from mmm_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename='" & Session("USERROLE") & "'))))A  where [current user] is not null and [Current user]<>'' "
            If docid <> "" Then
                query &= "  and  DocID in (" & docid & ") "
            End If
            If ddlcu.SelectedIndex <> -1 And ddlcu.SelectedIndex <> 0 Then
                query &= "  and [Current User]= '" & ddlcu.SelectedItem.Text.ToString() & "'"
            End If
            da.SelectCommand.CommandText = query

            ' Dim da As New SqlDataAdapter("select d.tid[DocID],(select username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid))[Current User],* from mmm_mst_doc d where d.eid=" & Session("EID") & " and d.documenttype='VRF FIXED_POOL' and d.curstatus='approved' ", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")

            Session("ddlselecteddoc") = ddldoctype.SelectedItem.Text.ToString()
            Session("ddlselectedstatus") = ddlstatus.SelectedItem.Text
            Session("getresultdata") = ds.Tables("data")

            bindgrid(ds.Tables("data"))
            Call bindcu(ff.ToString())

        End If
    End Sub
    Protected Sub getresultByDocid()
        'Dim datatable As DataTable
        'If Not Session("getfilterresult") Is Nothing Then
        '    datatable = CType(Session("getfilterresult"), DataTable)
        'Else
        '    datatable = CType(Session("getresultdata"), DataTable)
        'End If
        'Dim DBDT As DataView = datatable.DefaultView
        'Dim var As String() = Split(txtDocid.Text, ",")
        'Dim docStr As String = ""
        'For i As Integer = 0 To var.Length - 1
        '    docStr &= docStr & "'" & var(i).Trim & "',"
        'Next
        'docStr = Left(docStr, docStr.Length - 1)
        'DBDT.RowFilter = "DOCID in (" & docStr & ")"
        'Dim datafields As DataTable = DBDT.Table.DefaultView.ToTable()

        'bindgrid(datafields)

        'ddltu.Items.Clear()
        xrow.Visible = True
        btnsave.Visible = True
        gvData.Visible = True
        trdocid.Visible = True
        Dim dt As DataTable
        If Session("getfilterresult") Is Nothing Then
            dt = Session("getresultdata")
        Else
            dt = Session("getresultdata")
        End If
        Dim results As DataTable
        If ddlcu.SelectedIndex <> -1 Then
            If ddlcu.SelectedItem.Text <> "Select" Then
                Try
                    results = dt.[Select]("[Current User] ='" & ddlcu.SelectedItem.Text.ToString() & "' and DocID in (" & txtDocid.Text & ")").CopyToDataTable()
                    gvData.DataSource = results
                    gvData.DataBind()
                    Session("getfilterresult") = results
                    gvData.Caption = "No. of Records: " & results.Rows.Count
                    If ddltu.SelectedItem.Text <> "Select" AndAlso ddlcu.SelectedItem.Text <> "Select" Then
                        For Each row As GridViewRow In gvData.Rows
                            Dim ddl As DropDownList = DirectCast(row.FindControl("ddluser"), DropDownList)
                            '    ddl.SelectedValue = ddltu.SelectedValue.ToString()
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(ddltu.SelectedItem.Text.ToString()))
                        Next
                    End If
                Catch ex As Exception
                    SearchDocID(txtDocid.Text)
                End Try
            Else
                SearchDocID(txtDocid.Text)
            End If
        Else
            SearchDocID(txtDocid.Text)
        End If

        'Uncomment if not working 11-Nov-2016
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)
        'Dim da As New SqlDataAdapter("Select cfield,remarks from mmm_mst_Reallocation where eid=" & Session("EID") & " and doctype='" & ddldoctype.SelectedItem.Text & "' and status='" & ddlstatus.SelectedItem.Text & "'", con)
        'Dim dss As New DataSet
        'da.Fill(dss, "cr")

        'If dss.Tables("cr").Rows.Count > 0 Then


        '    da.SelectCommand.CommandText = "Select dropdown from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddldoctype.SelectedItem.Text & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'"
        '    If con.State <> ConnectionState.Open Then
        '        con.Open()
        '    End If
        '    Dim sse As String() = da.SelectCommand.ExecuteScalar().ToString.Split("-")

        '    Dim ff As String = String.Empty
        '    If sse.Length > 1 Then
        '        da.SelectCommand.CommandText = "Select docmapping from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & sse(1).ToString & "'"
        '        ff = da.SelectCommand.ExecuteScalar()

        '        ViewState("ff") = ff
        '    End If

        '    Dim fld As String = dss.Tables("cr").Rows(0).Item("cfield").ToString()
        '    Dim remarks As String = dss.Tables("cr").Rows(0).Item("remarks").ToString()
        '    ViewState("fld") = fld.ToString()
        '    ViewState("remarks") = remarks.ToString()
        '    da.SelectCommand.CommandTimeout = 7000
        '    If txtDocid.Text.Length > 1 Then
        '        da.SelectCommand.CommandText = "SELECT  DocID,[Current User],[LastTID],[ptat],[ordering],[fdate] FROM (select d.tid[DocID],(select distinct username from mmm_mst_user where eid=" & Session("EID") & " and uid= dd.userid )[Current User],dd.userid,d.lasttid[Lasttid],dd.ptat[ptat],dd.ordering[ordering],dd.fdate from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & Session("EID") & " and d.documenttype='" & ddldoctype.SelectedItem.Text & "' and d.curstatus='" & ddlstatus.SelectedItem.Text & "' and dd.userid is not null and dd.userid not in (217,363)  and " & fld & " in (select * from  InputString((select " & ff.ToString() & " from mmm_ref_role_user where eid=" & Session("EID") & " and rolename='" & Session("USERROLE") & "' and uid=" & Session("UID") & "))))A  where [current user] is not null and [Current user]<>'' and docid in (" & Trim(txtDocid.Text) & ") "

        '        ' Dim da As New SqlDataAdapter("select d.tid[DocID],(select username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid))[Current User],* from mmm_mst_doc d where d.eid=" & Session("EID") & " and d.documenttype='VRF FIXED_POOL' and d.curstatus='approved' ", con)
        '        Dim ds As New DataSet
        '        da.Fill(ds, "data")

        '        Session("ddlselecteddoc") = ddldoctype.SelectedItem.Text.ToString()
        '        Session("ddlselectedstatus") = ddlstatus.SelectedItem.Text
        '        Session("getresultdata") = ds.Tables("data")

        '        bindgrid(ds.Tables("data"))
        '        Call bindcu(ff.ToString())
        '    End If
        '    da.Dispose()
        '    con.Close()
        '    con.Dispose()

        'End If

    End Sub
    Protected Sub ddldoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldoctype.SelectedIndexChanged
        bindstatus()
    End Sub
    Private Sub bindstatus()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select  * from mmm_mst_reallocation where eid=" & Session("EID") & " and doctype='" & ddldoctype.SelectedItem.Text & "'  and role='" & Session("USERROLE") & "'   order by status", con)
        '        Dim da As New SqlDataAdapter("Select  * from mmm_mst_reallocation where eid=" & Session("EID") & " and doctype='" & ddldoctype.SelectedItem.Text & "' and role='" & Session("USERROLE") & "' order by status", con)
        Dim ds As New DataSet
        da.Fill(ds, "status")
        If ds.Tables("status").Rows.Count > 0 Then
            ddlstatus.DataSource = ds.Tables("status")
            ddlstatus.DataTextField = "Status"
            ddlstatus.DataValueField = "Status"
            ddlstatus.DataBind()
            ddlstatus.Items.Insert(0, New ListItem("Select"))
        End If

        trdocid.Visible = True
    End Sub
    Protected Sub gvData_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvData.RowDataBound
        Dim row As GridViewRow = e.Row
        Try

            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim ddl As DropDownList = CType(e.Row.FindControl("ddluser"), DropDownList)
                Dim chk As CheckBox = DirectCast(row.FindControl("chk"), CheckBox)
                Dim tid As String = row.Cells(1).Text.ToString()
                If row.Cells(2).Text.ToString() = "" Then
                    chk.Visible = False
                End If
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As New SqlConnection(conStr)
                Dim da As New SqlDataAdapter("USP_get_targetusers", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.CommandTimeout = 500
                da.SelectCommand.Parameters.AddWithValue("@TID", tid)
                da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(Session("ddlselecteddoc")))
                da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(Session("ddlselectedstatus")))
                Dim ds As New DataSet
                da.Fill(ds, "data")
                ddl.DataSource = ds.Tables("data")
                ddl.DataTextField = "username"
                ddl.DataValueField = "uid"
                ddl.DataBind()
                ddl.Items.Insert(0, New ListItem("Select"))

                'da.SelectCommand.Parameters.Clear()
                'da.selectcommand.commandtext = "USP_get_targetusersdropdown"
                'da.selectcommand.commandtype = commandtype.storedprocedure
                'da.selectcommand.commandtimeout = 5000
                'da.SelectCommand.Parameters.AddWithValue("@TID", tid)
                'da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                'da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(Session("ddlselecteddoc")))
                'da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(Session("ddlselectedstatus")))
                'da.fill(ds, "users")

                'Session("targetuser") = ds.Tables("users")
                'ddltu.DataSource = Session("targetuser")
                'ddltu.DataTextField = "username"
                'ddltu.DataValueField = "uid"
                'ddltu.DataBind()



                row.Cells(3).Controls.Add(ddl)

            End If

        Catch ex As Exception

        End Try

    End Sub
    Protected Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click
        lblMsgupdate.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim cnt As Integer = 0
        Dim dt As New DataTable
        dt = Session("getresultdata")
        Dim i As Integer = 0
        For Each row As GridViewRow In gvData.Rows
            Dim chk As CheckBox = DirectCast(row.FindControl("chk"), CheckBox)
            If chk.Checked = True Then
                cnt = cnt + 1
                Dim ddl As DropDownList = DirectCast(row.FindControl("ddluser"), DropDownList)
                Dim tid As Integer = row.Cells(1).Text.ToString()
                If ddl.SelectedItem.Text = "Select" Then
                    Continue For
                Else
                    Dim uid As Integer = ddl.SelectedValue.ToString
                    Dim da As New SqlDataAdapter("update mmm_doc_dtl set tdate=getdate(),atat=datediff(day,'" & dt.Rows(i).Item("fdate").ToString() & "' ,getdate() ),remarks='" & ViewState("remarks") & "',aprstatus='" & ddlstatus.SelectedItem.Text & "' where tid=(select lasttid from mmm_mst_doc where eid=" & Session("EID") & " and tid=" & tid & ")", con)

                    'Dim da As New SqlDataAdapter("update mmm_doc_dtl set userid=" & uid & ",fdate=getdate(),atat=datediff(day,fdate ,tdate ),remarks='" & ViewState("remarks") & "' where tid=(select lasttid from mmm_mst_doc where eid=" & Session("EID") & " and tid=" & tid & ")", con)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.SelectCommand.ExecuteNonQuery()
                    da.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,tdate,ptat,atat ,aprstatus,remarks,pathID,Ordering,DocNature)values(" & uid & "," & tid & ", getdate(),null," & dt.Rows(i).Item("ptat").ToString() & ",null,null,null,0," & dt.Rows(i).Item("ordering").ToString() & ",'CREATE' );UPdate mmm_mst_doc set lasttid=(select scope_identity()) where tid=" & tid & ""
                    da.SelectCommand.ExecuteNonQuery()
                End If
            End If
            i = i + 1
        Next
        getresult()
        getresulfilter()
        gvData.Visible = False
        'bindgrid(Session("getresultdata"))
        'bindgrid(Session("getfilterresult"))
        Session("getresultdata") = Nothing
        Session("getfilterresult") = Nothing
        ddltu.SelectedIndex = ddltu.Items.IndexOf(ddltu.Items.FindByText("Select"))
        btnsave.Visible = False
        xrow.Visible = False
        con.Close()
        con.Dispose()
        If cnt > 0 Then
            Session("getresultdata") = Nothing
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Reallocation is Done!!!!!');window.location='ReallocationRights.aspx';", True)
            'criptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Reallocation is Done!!!!!');", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Please select at least one User to Reallocate!!!!!');", True)
        End If
    End Sub
    Protected Sub getresulfilter()
        lblMsgupdate.Text = ""
        xrow.Visible = True
        btnsave.Visible = True
        gvData.Visible = True
        trdocid.Visible = True
        Dim dt As DataTable
        If Session("getfilterresult") Is Nothing Then
            dt = Session("getresultdata")
        Else
            dt = Session("getresultdata")
        End If
        Dim results As DataTable
        If ddlcu.SelectedItem.Text <> "Select" Then
            Try
                results = dt.[Select]("[Current User] ='" & ddlcu.SelectedItem.Text.ToString() & "'").CopyToDataTable()
                gvData.DataSource = results
                gvData.DataBind()
                Session("getfilterresult") = results
                gvData.Caption = "No. of Records: " & results.Rows.Count
                If ddltu.SelectedItem.Text <> "Select" AndAlso ddlcu.SelectedItem.Text <> "Select" Then
                    For Each row As GridViewRow In gvData.Rows
                        Dim ddl As DropDownList = DirectCast(row.FindControl("ddluser"), DropDownList)
                        '    ddl.SelectedValue = ddltu.SelectedValue.ToString()
                        ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(ddltu.SelectedItem.Text.ToString()))
                    Next
                End If
            Catch ex As Exception
                SearchDocID(txtDocid.Text)
            End Try

        Else

        End If
        ' updPnlGrid.Update()
    End Sub
    Protected Sub ddlcu_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlcu.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim dt As New DataTable
        dt = Session("getresultdata")
        Dim tid As Integer

        If dt.Rows.Count > 0 Then
            tid = dt.Rows(dt.Rows.Count() - 1)("DocID").ToString()
            da.SelectCommand.Parameters.Clear()
            'da.SelectCommand.CommandText = "USP_get_targetusersdropdown"
            'da.SelectCommand.CommandType = CommandType.StoredProcedure
            'da.SelectCommand.CommandTimeout = 5000
            'da.SelectCommand.Parameters.AddWithValue("@TID", tid)
            'da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
            'da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(Session("ddlselecteddoc")))
            'da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(Session("ddlselectedstatus")))
            'da.Fill(ds, "users")
            da.SelectCommand.CommandText = "USP_get_targetusers"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandTimeout = 5000
            da.SelectCommand.Parameters.AddWithValue("@TID", tid)
            da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
            da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(Session("ddlselecteddoc")))
            da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(Session("ddlselectedstatus")))
            da.Fill(ds, "data")
            ddltu.DataSource = ds.Tables("data")
            ddltu.DataTextField = "username"
            ddltu.DataValueField = "uid"
            ddltu.DataBind()
            ddltu.Items.Insert(0, New ListItem("Select"))
            getresulfilter()
        End If
        'For i As Integer = 0 To dt.Rows.Count - 1
        '    tid = dt.Rows(i).Item("DocID").ToString()
        '    da.SelectCommand.Parameters.Clear()
        '    da.SelectCommand.CommandText = "USP_get_targetusersdropdown"
        '    da.SelectCommand.CommandType = CommandType.StoredProcedure
        '    da.SelectCommand.CommandTimeout = 5000
        '    da.SelectCommand.Parameters.AddWithValue("@TID", tid)
        '    da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
        '    da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(Session("ddlselecteddoc")))
        '    da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(Session("ddlselectedstatus")))


        'Next
    End Sub
End Class

