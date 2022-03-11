Imports System.Data
Imports System.Data.SqlClient
Partial Class mobile_DocumentHistroy
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Session("EID") Is Nothing Then
            If Not IsPostBack Then
                BindDashBoard()
                bindFilterDDl()
                bindDDlSyDocIDHistory()
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
    End Sub

    Public Sub BindDashBoard()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        'Dim od As New SqlDataAdapter("Select top 20 M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
        Dim od As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
        od.Fill(ds, "action")
        ViewState("action") = ds.Tables("action")
        gvAction.DataSource = ds.Tables("action")
        gvAction.DataBind()
        ViewState("DropDownFilt") = ds.Tables("action")

        'Coding below is for GVMYUPLOAD GridView
        'total no of pages / divided by page size
        ViewState("cpnu") = currentPageNumberu.ToString()
        ViewState("cnt") = CInt(Math.Ceiling(ds.Tables("action").Rows.Count / gvAction.PageSize))
        lbltotalpages.Text = Trim("Displaying page no. " & currentPageNumber & " of total page no(s) " & ViewState("cnt"))
        ViewState("cpn") = currentPageNumber.ToString()
        If ds.Tables("action").Rows.Count = "0" Then
            lbltotalpages.Visible = False
        Else
            lbltotalpages.Visible = True
        End If
        If currentPageNumber = 1 Then
            lnkprev.Visible = False
            If ViewState("cnt") > 0 Then
                lnknext.Enabled = True
            Else
                lnknext.Enabled = False
            End If
        Else
            lnkprev.Visible = True
            If currentPageNumber = Convert.ToInt32(ds.Tables("action").Rows.Count) Then
                lnknext.Enabled = False
            Else
                lnknext.Enabled = True
            End If
        End If
        If currentPageNumber = ViewState("cnt") Or ViewState("cnt") = 0 Then
            lnknext.Visible = False
        Else
            lnknext.Visible = True
        End If
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
    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvAction.PageIndexChanging
        Try
            gvAction.PageIndex = e.NewPageIndex
            currentPageNumber = e.NewPageIndex + 1
            BindDashBoard()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvAction_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvAction.Sorting
        Dim sortExpression As String = e.SortExpression
        If (GridViewSortDirection = SortDirection.Ascending) Then
            GridViewSortDirection = SortDirection.Descending
            SortGridViewa(sortExpression, DESCENDING)
        Else
            GridViewSortDirection = SortDirection.Ascending
            SortGridViewa(sortExpression, ASCENDING)
        End If
    End Sub

    Private Sub SortGridViewa(ByVal sortExpression As String, ByVal direction As String)
        'You can cache the DataTable for improving performance
        Dim dt As DataTable = CType(ViewState("action"), DataTable)
        dt.DefaultView.Sort = sortExpression + direction
        Dim dt1 As DataTable = dt.DefaultView.ToTable()
        gvAction.DataSource = ViewState("action")
        bindFilteGrid()
        gvAction.DataBind()
        'ViewState("data") = dt1
    End Sub

    'Custom paging on GridView functionality

    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1

    'Private Const PAGE_SIZE As Integer = 10

    Protected Sub ChangePage(sender As Object, e As CommandEventArgs)
        Select Case e.CommandName
            Case "Previous"
                currentPageNumber = Int32.Parse(ViewState("cpn")) - 1
                If gvAction.PageIndex > 0 Then
                    gvAction.PageIndex = gvAction.PageIndex - 1
                    bindFilteGrid()
                    BindDashBoard()
                End If

                Exit Select
            Case "Next"
                currentPageNumber = Int32.Parse(ViewState("cpn")) + 1
                gvAction.PageIndex = gvAction.PageIndex + 1
                bindFilteGrid()
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
    Private Sub bindFilterDDl()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From]  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "
        'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        Dim ds As New DataTable
        oda.Fill(ds)
        oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "

        'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        Dim ds1 As New DataTable
        oda.Fill(ds1)
        For i As Integer = 0 To ds.Columns.Count - 1
            ddlGrdHdr.Items.Add(ds.Columns.Item(i).ToString)
            ddlGrdHdr.Items(i).Value = ds1.Columns.Item(i).ToString
        Next

        'ddlGrdHdr.DataValueField = ds
    End Sub
    Protected Sub ddlGrdHdr_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlGrdHdr.SelectedIndexChanged
        BindDashBoard()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlGrdHdr.SelectedItem.Text = "System Doc. ID" Then
            ' oda.SelectCommand.CommandText = "Select distinct  M." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  "
        ElseIf ddlGrdHdr.SelectedItem.Text = "Created By" Then
            'oda.SelectCommand.CommandText = "Select distinct  u." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
            oda.SelectCommand.CommandText = "Select distinct  u." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "
        ElseIf ddlGrdHdr.SelectedItem.Text = "To" Then
            'oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
            oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  "
        ElseIf ddlGrdHdr.SelectedItem.Text = "Pending From" Then
            'oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & "  "
            oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  "
        ElseIf ddlGrdHdr.SelectedItem.Text = "Applied On" Then
            'oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & "  "
            oda.SelectCommand.CommandText = "Select distinct convert(nvarchar,adate,23) from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  "

        Else
            'oda.SelectCommand.CommandText = "Select distinct  " & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
            oda.SelectCommand.CommandText = "Select distinct  " & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  "
        End If
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlGrdVal.Items.Clear()
        For j As Integer = 0 To ds.Rows.Count - 1
            ddlGrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
        Next

    End Sub
    Private Sub bindFilteGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlGrdHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and M." & ddlGrdHdr.SelectedValue & "=" & ddlGrdVal.SelectedItem.Text & " "
        ElseIf ddlGrdHdr.SelectedItem.Text = "Pending From" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and datediff(day,fdate,getdate()) = '" & ddlGrdVal.SelectedItem.Text & "' "
        ElseIf ddlGrdHdr.SelectedItem.Text = "Applied On" Then
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and convert(nvarchar," & ddlGrdHdr.SelectedValue & ",23) = '" & ddlGrdVal.SelectedItem.Text & "' "
        Else
            oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and " & ddlGrdHdr.SelectedValue & "='" & ddlGrdVal.SelectedItem.Text & "' "
        End If
        Dim ds As New DataSet
        oda.Fill(ds, "action")
        gvAction.DataSource = ds.Tables("action")
        gvAction.DataBind()
        ViewState("count") = CInt(Math.Ceiling(ds.Tables("action").Rows.Count / gvAction.PageSize))
        lbltotalpages.Text = Trim("Displaying page no. " & currentPageNumber & " of total page no(s) " & ViewState("count"))
        'lbltotalpages.Text = ""
    End Sub
    Protected Sub ddlGrdVal_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlGrdVal.SelectedIndexChanged
        bindFilteGrid()
    End Sub

    'Search filter for Need to act
   
    'Search filter for My Request
    Private Sub bindDDlSyDocIDHistory()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        If ddlGrdHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  "
        End If
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlGrdVal.Items.Clear()
        For j As Integer = 0 To ds.Rows.Count - 1
            ddlGrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
        Next
    End Sub
End Class
