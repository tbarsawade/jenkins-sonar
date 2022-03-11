Imports System.Data
Imports System.Data.SqlClient
Partial Class mobile_MyRequest
    Inherits System.Web.UI.Page
    Protected dsHolidays As DataSet

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Session("EID") Is Nothing Then
            If Not IsPostBack Then
                BindDashBoard()
                bindFilteMyReqddl()
                bindDDlSyDocIDMyReq()
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
        
    End Sub

    Protected Sub SSOLink(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ob As New MainUtility()
        Dim timestamp As Integer = ob.DateTimeToEpoch(DateTime.UtcNow)
        Dim name As String = Session("USERNAME")
        Dim email As String = Session("EMAIL")
        Dim ext_id As String = ""

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("Select * from MMM_SSO_ZENDESK WHERE EID=" & Session("EID").ToString(), con)
        Dim dt As New DataTable
        od.Fill(dt)
        od.Dispose()
        con.Dispose()


        If dt.Rows.Count = 1 Then
            Dim org As String = dt.Rows(0).Item("orgName").ToString()
            Dim token As String = dt.Rows(0).Item("zKey").ToString()
            Dim link As String = dt.Rows(0).Item("zendesklink").ToString()
            Dim hash As String = ob.Md5(name & email & ext_id & org & token & timestamp)
            Dim retUrl As String = link & "name=" & name & "&email=" & email & "&external_id=" & ext_id & "&organization=" & org & "&timestamp=" & timestamp & "&hash=" & hash
            ScriptManager.RegisterStartupScript(Page, GetType(Page), "OpenWindow", "window.open('" & retUrl & "');", True)
            'Response.Redirect(retUrl)
        End If

    End Sub

    Public Sub BindDashBoard()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        'Dim od As New SqlDataAdapter("Select top 20 M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
        Dim od As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        'counting rows
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'counting rows
        'od.SelectCommand.CommandText = "Select count(*) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "
        'od.SelectCommand.CommandText = "Select top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,aprstatus [curstatus] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus ='UPLOADED' order by M.Tid desc"
        od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        od.Fill(ds, "upload")
        ViewState("upload") = ds.Tables("upload")
        ''counting rows
        gvMyUpload.DataSource = ds.Tables("upload")
        gvMyUpload.DataBind()

        'Coding below is for GVPENDING GridView

        'total no of pages / divided by page size
        'Current page number



        'Coding below is for GVMYUPLOAD GridView

        'total no of pages / divided by page size
        ViewState("cntu") = CInt(Math.Ceiling(ds.Tables("upload").Rows.Count / gvMyUpload.PageSize))
        lbltotup.Text = Trim("Displaying page no. " & currentPageNumberu & " of total page no(s) " & ViewState("cntu"))
        'Current page number
        If ds.Tables("upload").Rows.Count = 0 Then
            lbltotup.Visible = False
        Else
            lbltotup.Visible = True
        End If
        ViewState("cpnu") = currentPageNumberu.ToString()
        If currentPageNumberu = 1 Then
            lnkPrevup.Visible = False
            If ViewState("cntu") > 0 Then
                lnknextup.Enabled = True
            Else
                lnknextup.Enabled = False
            End If
        Else
            lnkPrevup.Visible = True
            If currentPageNumberu = Convert.ToInt32(ds.Tables("upload").Rows.Count) Then
                lnknextup.Enabled = False
            Else
                lnknextup.Enabled = True
            End If
        End If
        If currentPageNumberu = ViewState("cntu") Or ViewState("cntu") = "0" Then
            lnknextup.Visible = False
        Else
            lnknextup.Visible = True
        End If
        'coding below is for GVACTION Gridview'
        'total no of pages / divided by page size
        con.Close()
        od.Dispose()

    End Sub


    Protected Sub RefreshPanel(ByVal sender As Object, ByVal e As EventArgs)
        BindDashBoard()
    End Sub

    Private Const ASCENDING As String = " ASC"
    Private Const DESCENDING As String = " DESC"

    Public Property GridViewSortDirection As SortDirection
        Get
            If Val(ViewState("sortDirection")) = Val(DBNull.Value.ToString) Then
                ViewState("sortDirection") = SortDirection.Ascending

            End If
            Return CType(ViewState("sortDirection"), SortDirection)

        End Get
        Set(ByVal value As SortDirection)
            ViewState("sortDirection") = value
        End Set
    End Property

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvMyUpload.PageIndexChanging
        Try
            gvMyUpload.PageIndex = e.NewPageIndex
            currentPageNumberu = e.NewPageIndex + 1
            BindDashBoard()
            'bindMyReqGrid()
        Catch ex As Exception
        End Try
    End Sub

   

    
   

    Protected Sub gvMyUpload_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvMyUpload.SelectedIndexChanged

    End Sub

    Protected Sub gvMyUpload_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvMyUpload.Sorting
        Dim sortExpression As String = e.SortExpression
        If (GridViewSortDirection = SortDirection.Ascending) Then
            GridViewSortDirection = SortDirection.Descending
            SortGridViewu(sortExpression, DESCENDING)
        Else
            GridViewSortDirection = SortDirection.Ascending
            SortGridViewu(sortExpression, ASCENDING)
        End If
    End Sub

    Private Sub SortGridViewu(ByVal sortExpression As String, ByVal direction As String)
        'You can cache the DataTable for improving performance
        Dim dt As DataTable = CType(ViewState("upload"), DataTable)

        dt.DefaultView.Sort = sortExpression + direction
        Dim dt1 As DataTable = dt.DefaultView.ToTable()
        gvMyUpload.DataSource = ViewState("upload")
        gvMyUpload.DataBind()
        'ViewState("data") = dt1
    End Sub


    'Custom paging on GridView functionality

    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1

    'Private Const PAGE_SIZE As Integer = 10
    Protected Sub ChangePageU(ByVal sender As Object, ByVal e As CommandEventArgs)

        Select Case e.CommandName
            Case "Previous"
                currentPageNumberu = Int32.Parse(ViewState("cpnu")) - 1
                If gvMyUpload.PageIndex > 0 Then
                    gvMyUpload.PageIndex = gvMyUpload.PageIndex - 1
                    bindMyReqGrid()
                    BindDashBoard()
                End If

                Exit Select
            Case "Next"

                currentPageNumberu = Int32.Parse(ViewState("cpnu")) + 1
                gvMyUpload.PageIndex = gvMyUpload.PageIndex + 1
                bindMyReqGrid()
                BindDashBoard()
                Exit Select
        End Select

        BindDashBoard()
    End Sub
    Private Sub bindFilteMyReqddl()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From]   from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        Dim ds As New DataTable
        oda.Fill(ds)
        oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,ap.username,datediff(day,fdate,getdate())[fdate]  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "

        'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        Dim ds1 As New DataTable
        oda.Fill(ds1)
        For i As Integer = 0 To ds.Columns.Count - 1
            ddlMyReqHdr.Items.Add(ds.Columns.Item(i).ToString)
            ddlMyReqHdr.Items(i).Value = ds1.Columns.Item(i).ToString
        Next
    End Sub
    Protected Sub ddlMyReqHdr_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlMyReqHdr.SelectedIndexChanged
        BindDashBoard()
        'ddlGrdVal.Items.Insert(0, "--Select Value-- ")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlMyReqHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlMyReqHdr.SelectedValue & "  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        ElseIf ddlMyReqHdr.SelectedItem.Text = "Created By" Then
            oda.SelectCommand.CommandText = "Select distinct  u." & ddlMyReqHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "

        ElseIf ddlMyReqHdr.SelectedItem.Text = "To" Then

            oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        ElseIf ddlMyReqHdr.SelectedItem.Text = "Pending From" Then

            oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        ElseIf ddlMyReqHdr.SelectedItem.Text = "Applied On" Then

            oda.SelectCommand.CommandText = "Select distinct convert(nvarchar,adate,23) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "

        Else

            oda.SelectCommand.CommandText = "Select distinct  " & ddlMyReqHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        End If
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlMyReqVal.Items.Clear()
        For j As Integer = 0 To ds.Rows.Count - 1
            ddlMyReqVal.Items.Add(ds.Rows(j).Item(0).ToString)
        Next
    End Sub
    Private Sub bindMyReqGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlMyReqHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and M." & ddlMyReqHdr.SelectedValue & "=" & ddlMyReqVal.SelectedItem.Text & " "
        ElseIf ddlMyReqHdr.SelectedItem.Text = "Pending From" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and datediff(day,fdate,getdate()) = '" & ddlMyReqVal.SelectedItem.Text & "' "

        ElseIf ddlMyReqHdr.SelectedItem.Text = "Applied On" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and  convert(nvarchar," & ddlMyReqHdr.SelectedValue & ",23) = '" & ddlMyReqVal.SelectedItem.Text & "' "


        ElseIf ddlMyReqHdr.SelectedItem.Text = "To" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and AP." & Left(ddlMyReqHdr.SelectedValue, ddlMyReqHdr.SelectedValue.Length - 1) & "='" & ddlMyReqVal.SelectedItem.Text & "' "

        Else
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and " & ddlMyReqHdr.SelectedValue & "='" & ddlMyReqVal.SelectedItem.Text & "' "
        End If

        Dim ds As New DataSet
        oda.Fill(ds, "upload")
        gvMyUpload.DataSource = ds.Tables("upload")
        gvMyUpload.DataBind()

        'ViewState("cntUpload") = CInt(Math.Ceiling(ds.Tables("upload").Rows.Count / gvMyUpload.PageSize))
        'lbltotalpages.Text = Trim("Displaying page no. " & currentPageNumber & " of total page no(s) " & ViewState("cntUpload"))
        'lbltotalpages.Text = ""
    End Sub
    Protected Sub ddlMyReqVal_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlMyReqVal.SelectedIndexChanged
        bindMyReqGrid()
    End Sub
    Private Sub bindDDlSyDocIDMyReq()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlMyReqHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlMyReqHdr.SelectedValue & "  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        End If
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlMyReqVal.Items.Clear()
        For j As Integer = 0 To ds.Rows.Count - 1
            ddlMyReqVal.Items.Add(ds.Rows(j).Item(0).ToString)
        Next
    End Sub
End Class
