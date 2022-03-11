Imports System
Imports System.Data
Imports System.Data.SqlClient

Partial Class Calendar
    Inherits System.Web.UI.Page
    Protected dsHolidays As DataSet

    Protected Sub FillHolidayDataset()
        Dim firstDate As New DateTime(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month, 1)
        Dim lastDate As DateTime = GetFirstDayOfNextMonth()
        dsHolidays = GetCurrentMonthData(firstDate, lastDate)
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
    Protected Function GetFirstDayOfNextMonth() As DateTime
        Dim monthNumber, yearNumber As Integer
        If Calendar1.VisibleDate.Month = 12 Then
            monthNumber = 1
            yearNumber = Calendar1.VisibleDate.Year + 1
        Else
            monthNumber = Calendar1.VisibleDate.Month + 1
            yearNumber = Calendar1.VisibleDate.Year
        End If
        Dim lastDate As New DateTime(yearNumber, monthNumber, 1)
        Return lastDate
    End Function

    Protected Sub Calendar1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Calendar1.SelectionChanged
        showTsklist(Calendar1.SelectedDate)
    End Sub

    Protected Sub Calendar1_VisibleMonthChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles Calendar1.VisibleMonthChanged
        FillHolidayDataset()
    End Sub

    Function GetCurrentMonthData(ByVal firstDate As DateTime, ByVal lastDate As DateTime) As DataSet
        Dim dsMonth As New DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_HOLIDAYS WHERE HoliDate >= @firstDate AND HoliDate < @lastDate and locid=" & Session("LID").ToString(), con)
        oda.SelectCommand.Parameters.Add(New SqlParameter("@firstDate", firstDate))
        oda.SelectCommand.Parameters.Add(New SqlParameter("@lastDate", lastDate))
        oda.Fill(dsMonth, "holidays")

        'For weekly Off
        oda.SelectCommand.CommandText = "select weeklyHoliday from mmm_mst_user where uid=" & Session("UID").ToString()
        oda.SelectCommand.Parameters.Clear()
        oda.Fill(dsMonth, "woff")

        oda.SelectCommand.CommandText = "getCurrentAppTask"
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@uid", Session("UID").ToString())
        oda.SelectCommand.Parameters.AddWithValue("@firstDate", firstDate)
        oda.SelectCommand.Parameters.AddWithValue("@lastDate", lastDate)
        oda.Fill(dsMonth, "aprTask")
        oda.Dispose()
        con.Dispose()
        Return dsMonth
    End Function

    Protected Sub Calendar1_DayRender(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DayRenderEventArgs) Handles Calendar1.DayRender
        If Not dsHolidays Is Nothing Then
            If e.Day.Date >= New DateTime(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month, 1) And e.Day.Date <= New DateTime(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month, DateTime.DaysInMonth(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month)) Then
                e.Day.IsSelectable = True
                'For weekly off
                Dim woff As String = dsHolidays.Tables("woff").Rows(0).Item("weeklyHoliday").ToString
                If InStr(woff, Weekday(e.Day.Date, Microsoft.VisualBasic.FirstDayOfWeek.Monday)) > 0 Then
                    e.Cell.BackColor = Drawing.Color.LightGray
                    e.Cell.ToolTip = "Weekly Holiday"
                End If

                Dim nextDate As DateTime
                For Each dr As DataRow In dsHolidays.Tables("holidays").Rows
                    nextDate = CType(dr("HoliDate"), DateTime)
                    If nextDate = e.Day.Date Then
                        e.Cell.BackColor = System.Drawing.Color.LemonChiffon
                        e.Cell.ToolTip = dr.Item("holidayname").ToString()
                    End If
                Next


                For Each dr As DataRow In dsHolidays.Tables("aprTask").Rows
                    If Val(dr("dt").ToString()) = e.Day.Date.Day Then
                        e.Cell.BackColor = System.Drawing.Color.LightGoldenrodYellow
                        e.Cell.ToolTip = dr.Item("curstatus").ToString()

                        Dim lt As New LiteralControl()
                        lt.Text = "<br /> " & dr("curstatus") & "(" & dr("cnt") & ")"
                        e.Cell.Controls.Add(lt)

                        'Dim ln As New LinkButton()
                        'ln.Text = "<br />"
                        ' AddHandler ln.Click, AddressOf ShowDayDetail
                        ' e.Cell.Controls.Add(ln)
                    End If
                Next
            Else
                e.Day.IsSelectable = False
                e.Cell.BackColor = Drawing.Color.LightCyan
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Calendar1.VisibleDate = DateTime.Today
            showTsklist(Today.Date)
        End If
        FillHolidayDataset()
    End Sub

    Public Sub showTsklist(ByVal dt1 As DateTime)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim dt As DateTime = dt1.ToString("yyyy-MM-dd hh:mm:ss")
        Dim qry As String = "select docid,remarks [TaskName],case status WHEN 1 then 'PENDING' END [STATUS]  from MMM_MST_CALENDAR where UID=@UID AND   CAST(FLOOR(CAST(DUE_DATE  AS float)) AS DATETIME) =CAST(FLOOR(CAST(@firstDate AS float)) AS DATETIME)"
        qry &= " Union all "
        qry &= "select docid,isnull(D.fld1,D.fld10) [TaskName],D.curstatus [STATUS]  from MMM_DOC_DTL M inner join MMM_MST_DOC D on D.tid=M.docid   where cast(floor(cast(fdate as float)) as datetime) = cast(floor(cast(@firstDate as float)) AS datetime) and M.userid=@uid and M.aprstatus is null "
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("select docid,remarks [TaskName],case status WHEN 1 then 'PENDING' END [STATUS]  from MMM_MST_CALENDAR where UID=@UID AND   CAST(FLOOR(CAST(DUE_DATE  AS float)) AS DATETIME) =CAST(FLOOR(CAST(@firstDate AS float)) AS DATETIME)", con)
        Dim oda As SqlDataAdapter = New SqlDataAdapter(qry, con)
        oda.SelectCommand.Parameters.Add(New SqlParameter("@firstDate", dt))
        oda.SelectCommand.Parameters.Add(New SqlParameter("@uid", Session("UID")))
        Dim ds As New DataSet
        oda.Fill(ds)
        grdTasklist.DataSource = ds
        grdTasklist.DataBind()
        If dt1 = Now.Date Then
            lbtoday.Text = "<Today TaskList>"
        Else
            lbtoday.Text = "<" & Calendar1.SelectedDate.ToString("dd-MMM-yyyy") & ">" & " Task List"
        End If
        UpdGrid.Update()



    End Sub
End Class
