Imports System.Data
Imports System.Data.SqlClient
Partial Class mobile_DashBoard
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Session("EID") Is Nothing Then
            If Not Page.IsPostBack Then
                BindDashBoard()
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
    End Sub

    Public Sub BindDashBoard()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim od As SqlDataAdapter = Nothing
        Try
            con = New SqlConnection(conStr)
            od = New SqlDataAdapter("Select distinct  M.tid,documenttype,curstatus,U.username,adate,AP.Username [apusername],datediff(day,fdate,getdate())[fdate]  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
            Dim ds As New DataSet
            od.Fill(ds, "pending")
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblNeedToAct.Text = ds.Tables("pending").Rows.Count
            od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
            od.Fill(ds, "action")
            lblHistroy.Text = ds.Tables("action").Rows.Count
            od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
            od.Fill(ds, "upload")
            ViewState("upload") = ds.Tables("upload")
            lblRequest.Text = ds.Tables("upload").Rows.Count
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            od.Dispose()
        End Try
    End Sub
End Class
