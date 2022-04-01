Imports System.Data
Imports System.Data.SqlClient
Partial Class mobile_Pending
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Session("EID") Is Nothing Then
            If Not IsPostBack Then
                BindDashBoard()
                bindFilteNeedActddl()
                bindDDlSyDocIDNeedact()
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
    End Sub


    Public Sub BindDashBoard()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        'Dim od As New SqlDataAdapter("Select top 20 M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
        Dim od As New SqlDataAdapter("Select distinct  M.tid,documenttype,curstatus,U.username,adate,AP.Username [apusername],datediff(day,fdate,getdate())[fdate]  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
        Dim ds As New DataSet
        od.Fill(ds, "pending")
        ViewState("pending") = ds.Tables("pending")
        'counting rows
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'od.SelectCommand.CommandText = "Select distinct  top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus [curstatus] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
        od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
        od.Fill(ds, "action")
        ViewState("action") = ds.Tables("action")
        'counting rows
        'od.SelectCommand.CommandText = "Select count(*) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "

        'od.SelectCommand.CommandText = "Select top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,aprstatus [curstatus] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus ='UPLOADED' order by M.Tid desc"
        od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        od.Fill(ds, "upload")
        ViewState("upload") = ds.Tables("upload")

        ''counting rows

        gvPending.DataSource = ds.Tables("pending")
        gvPending.DataBind()
        ViewState("DropDownFilt") = ds.Tables("action")

        'Coding below is for GVPENDING GridView

        'total no of pages / divided by page size
        ViewState("cntp") = CInt(Math.Ceiling(ds.Tables("pending").Rows.Count / gvPending.PageSize))
        lbltotpending.Text = Trim("Displaying page no. " & currentPageNumberp & " of total page no(s) " & ViewState("cntp"))
        'Current page number
        If ds.Tables("pending").Rows.Count = "0" Then
            lbltotpending.Visible = False
        Else
            lbltotpending.Visible = True
        End If
        ViewState("cpnp") = currentPageNumberp.ToString()
        If currentPageNumberp = 1 Then
            lnkprevpending.Visible = False
            If ViewState("cntu") > 0 Then
                lnknextpending.Enabled = True
            Else
                lnknextpending.Enabled = False
            End If
        Else
            lnkprevpending.Visible = True
            If currentPageNumberu = Convert.ToInt32(ds.Tables("pending").Rows.Count) Then
                lnknextpending.Enabled = False
            Else
                lnknextpending.Enabled = True
            End If
        End If
        If currentPageNumberp = ViewState("cntp") Or ViewState("cntp") = "0" Then
            lnknextpending.Visible = False
        Else
            lnknextpending.Visible = True
        End If



        'Coding below is for GVMYUPLOAD GridView

        'total no of pages / divided by page size
        ViewState("cpnu") = currentPageNumberu.ToString()


        'coding below is for GVACTION Gridview'



        'total no of pages / divided by page size


        'lblcnt.Text = "Displaying records ( " &  & ") "

        ViewState("cpn") = currentPageNumber.ToString()
        
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

    
    Protected Sub gvPending_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvPending.PageIndexChanged

    End Sub
    Protected Sub GridView3_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPending.PageIndexChanging
        Try
            gvPending.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1
            bindNeedActGrid()
            'BindDashBoard()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvPending_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvPending.Sorting
        Dim sortExpression As String = e.SortExpression

        If (GridViewSortDirection = SortDirection.Ascending) Then

            GridViewSortDirection = SortDirection.Descending

            SortGridView(sortExpression, DESCENDING)

        Else

            GridViewSortDirection = SortDirection.Ascending
            SortGridView(sortExpression, ASCENDING)


        End If
    End Sub
    Private Sub SortGridView(ByVal sortExpression As String, ByVal direction As String)
        'You can cache the DataTable for improving performance
        Dim dt As DataTable = CType(ViewState("pending"), DataTable)

        dt.DefaultView.Sort = sortExpression + direction
        Dim dt1 As DataTable = dt.DefaultView.ToTable()
        gvPending.DataSource = ViewState("pending")
        gvPending.DataBind()
        'ViewState("data") = dt1
    End Sub

   



   
    

   


    



    'Custom paging on GridView functionality

    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1

    'Private Const PAGE_SIZE As Integer = 10



    Protected Sub ChangePagep(ByVal sender As Object, ByVal e As CommandEventArgs)

        Select Case e.CommandName
            Case "Previous"
                currentPageNumberp = Int32.Parse(ViewState("cpnp")) - 1
                If gvPending.PageIndex > 0 Then
                    gvPending.PageIndex = gvPending.PageIndex - 1
                    bindNeedActGrid()
                    BindDashBoard()
                End If
                If gvPending.PageIndex > 0 Then
                    gvPending.PageIndex = gvPending.PageIndex - 1
                    bindNeedActGrid()
                    BindDashBoard()
                End If
                Exit Select
            Case "Next"
                currentPageNumberp = Int32.Parse(ViewState("cpnp")) + 1
                gvPending.PageIndex = gvPending.PageIndex + 1
                bindNeedActGrid()
                BindDashBoard()
                Exit Select
        End Select

        BindDashBoard()
    End Sub

    'Private Function CalculateTotalPages(ByVal totalRows As Double) As Integer
    '    Dim totalPages As Integer = CInt(Math.Ceiling(totalRows / PAGE_SIZE))
    '    Return totalPages
    'End Function

    'Search filter for history

    'Search filter for Need to act
    Private Sub bindFilteNeedActddl()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [Current User] ,datediff(day,fdate,getdate())[Pending From]   from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        Dim ds As New DataTable
        oda.Fill(ds)
        oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[Current User],datediff(day,fdate,getdate())[fdate]  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""

        'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        Dim ds1 As New DataTable
        oda.Fill(ds1)
        ddlPendinggrdHdr.Items.Clear()
        For i As Integer = 0 To ds.Columns.Count - 1
            ddlPendinggrdHdr.Items.Add(ds.Columns.Item(i).ToString)
            ddlPendinggrdHdr.Items(i).Value = ds1.Columns.Item(i).ToString
        Next
    End Sub
    Protected Sub ddlPendinggrdHdr_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlPendinggrdHdr.SelectedIndexChanged
        BindDashBoard()
        'ddlGrdVal.Items.Insert(0, "--Select Value-- ")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlPendinggrdHdr.SelectedValue & "  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Created By" Then
            oda.SelectCommand.CommandText = "Select distinct  u." & ddlPendinggrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""

        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Current User" Then

            oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Pending From" Then

            oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Applied On" Then

            oda.SelectCommand.CommandText = "Select distinct convert(nvarchar,adate,23) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""

        Else

            oda.SelectCommand.CommandText = "Select distinct  " & ddlPendinggrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        End If
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlPendinggrdVal.Items.Clear()
        For j As Integer = 0 To ds.Rows.Count - 1
            ddlPendinggrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
        Next
    End Sub
    Private Sub bindNeedActGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M." & ddlPendinggrdHdr.SelectedValue & "=" & ddlPendinggrdVal.SelectedItem.Text & " "
        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Pending From" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and datediff(day,fdate,getdate()) = '" & ddlPendinggrdVal.SelectedItem.Text & "' "


        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Applied On" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and convert(nvarchar," & ddlPendinggrdHdr.SelectedValue & ",23) = '" & ddlPendinggrdVal.SelectedItem.Text & "' "

        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Current User" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and u." & ddlPendinggrdHdr.SelectedValue & "=" & ddlPendinggrdVal.SelectedItem.Text & "' "


        Else
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  and " & ddlPendinggrdHdr.SelectedValue & "='" & ddlPendinggrdVal.SelectedItem.Text & "' "
        End If

        Dim ds As New DataSet
        oda.Fill(ds, "pending")
        gvPending.DataSource = ds.Tables("pending")
        gvPending.DataBind()
        ViewState("count") = CInt(Math.Ceiling(ds.Tables("pending").Rows.Count / gvPending.PageSize))
        'lbltotalpages.Text = ""
    End Sub
    Protected Sub ddlPendinggrdVal_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlPendinggrdVal.SelectedIndexChanged
        bindNeedActGrid()
    End Sub



    'Search filter for My Request
    Private Sub bindDDlSyDocIDNeedact()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlPendinggrdHdr.SelectedValue & "  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        End If
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlPendinggrdVal.Items.Clear()
        For j As Integer = 0 To ds.Rows.Count - 1
            ddlPendinggrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
        Next
        con.Close()
        con.Dispose()
    End Sub
End Class
