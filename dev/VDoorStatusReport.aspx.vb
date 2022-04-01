Imports System.Data.SqlClient
Imports System.Data
Imports System.IO
Partial Class VDoorStatusReport
    Inherits System.Web.UI.Page
    Protected Sub btnshow_Click(sender As Object, e As System.EventArgs) Handles btnshow.Click
        Show()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select  IMIENO,ctime,igstatus,speed,tripon from  mmm_mst_gpsdata with (nolock) where imieno='" & txtIMEI.Text.ToString & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtfrom.Text.ToString & "'  and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtTo.Text.ToString & "' order by DATEADD(minute,DATEDIFF(minute,0,ctime),0)"
            Dim ds As New DataSet
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "data")
            Dim tbl As New DataTable
            tbl.Columns.Add("IMEINO", GetType(String))
            tbl.Columns.Add("FromDate", GetType(String))
            tbl.Columns.Add("ToDate", GetType(String))
            tbl.Columns.Add("Duration(HH:MM)", GetType(String))
            tbl.Columns.Add("Status", GetType(String))
            lblmsg.Text = ""
            GVReport1.Controls.Clear()
            If CDate(Left(txtfrom.Text, 10)) > CDate(txtTo.Text) Then
                lblmsg.Text = "Start Date should not be greater than from End Date."
                Exit Sub
            End If

            If CDate(txtfrom.Text) < CDate("2014-08-01") Then
                lblmsg.Text = "Data for this report is available from 2nd August 2014 Onward."
                Exit Sub
            End If
            Dim FrDate As String = ""
            Dim ToDate As String = ""
            Dim dist As Decimal = 0
            Dim rw As DataRow
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                If ds.Tables("data").Rows(i).Item("tripon").ToString() = "0" Then
                    If FrDate.ToString = "" Then
                        FrDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    End If
                    ToDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    If i = ds.Tables("data").Rows.Count - 1 Then
                        rw = tbl.NewRow
                        rw(0) = ds.Tables("data").Rows(i).Item("IMIENO").ToString
                        rw(1) = FrDate.ToString()
                        rw(2) = ToDate.ToString()
                        Dim mint As Integer = DateDiff(DateInterval.Minute, CDate(FrDate), CDate(ToDate)).ToString
                        Dim hours As Integer = mint \ 60
                        Dim minutes As Integer = mint - (hours * 60)
                        Dim timeElapsed As String = CType(hours, String) & ":" & CType(minutes, String)
                        rw(3) = timeElapsed.ToString
                        'rw(3) = DateDiff(DateInterval.Minute, CDate(FrDate), CDate(ToDate)).ToString
                        rw(4) = "Door Open"
                        tbl.Rows.Add(rw)
                    End If
                Else
                    ToDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    If i = ds.Tables("data").Rows.Count - 1 And FrDate.ToString = "" Then
                    Else
                        If FrDate <> "" Then
                            rw = tbl.NewRow
                            rw(0) = ds.Tables("data").Rows(i).Item("IMIENO").ToString
                            rw(1) = FrDate.ToString()
                            rw(2) = ToDate.ToString()
                            Dim mint As Integer = DateDiff(DateInterval.Minute, CDate(FrDate), CDate(ToDate)).ToString
                            Dim hours As Integer = mint \ 60
                            Dim minutes As Integer = mint - (hours * 60)
                            Dim timeElapsed As String = CType(hours, String) & ":" & CType(minutes, String)
                            rw(3) = timeElapsed.ToString
                            'rw(3) = DateDiff(DateInterval.Minute, CDate(FrDate), CDate(ToDate)).ToString
                            rw(4) = "Door Open"
                            tbl.Rows.Add(rw)
                            FrDate = ""
                        End If
                    End If
                End If
                'Insert in data table and change status of the devstatus filed to  new one
                'now initilize all values
            Next
            GVReport1.DataSource = tbl
            GVReport1.DataBind()
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Sub
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
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        GVReport1.AllowPaging = False
        GVReport1.DataBind()
        GVReport1.DataSource = ViewState("xlexport")

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Vehicle Status Interval Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=VehicleSIReport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GVReport1.Rows.Count - 1
            'Apply text style to each Row 
            GVReport1.Rows(i).Attributes.Add("class", "textmode")
        Next
        GVReport1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
    End Sub
    Protected Sub OnMap(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim OmMap As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(OmMap.NamingContainer, GridViewRow)
        Response.Redirect("ShowMap.aspx?IMIE=" & txtIMEI.Text & "&Start=" + row.Cells(3).ToString() + "&End=" + row.Cells(4).ToString())
    End Sub
    Protected Sub GVReport_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GVReport1.RowDataBound

    End Sub
End Class
