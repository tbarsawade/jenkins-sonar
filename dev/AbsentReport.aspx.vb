Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Partial Class AbsentReport
    Inherits System.Web.UI.Page
    'Private FPath As String = "D:\VHDJDE\MailAttach\"
    Private FPath As String = Server.MapPath("~/MailAttach/")
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
    Private Function CreateCSV(ByVal dt As DataTable, ByVal path As String) As String
        ' Dim fname As String = "F014Z1_" & DateTime.Now.ToString("yyyy-MM-dd") & ".CSV"
        Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & ".CSV"
        'Dim fname As String = "F014Z1_" & DateTime.Now.ToString("yyyy-MM-dd") & ".CSV"
        If File.Exists(path & fname) Then
            File.Delete(path & fname)
        End If
        Dim sw As StreamWriter = New StreamWriter(path & fname, False)
        sw.Flush()
        'First we will write the headers.
        Dim iColCount As Integer = dt.Columns.Count
        For i As Integer = 0 To iColCount - 1
            sw.Write(dt.Columns(i))
            If (i < iColCount - 1) Then
                sw.Write(",")
            End If
        Next
        sw.Write(sw.NewLine)
        ' Now write all the rows.
        Dim dr As DataRow
        For Each dr In dt.Rows
            For i As Integer = 0 To iColCount - 1
                If Not Convert.IsDBNull(dr(i)) Then
                    sw.Write(dr(i).ToString)
                End If
                If (i < iColCount - 1) Then
                    sw.Write(",")
                End If
            Next
            sw.Write(sw.NewLine)
        Next
        sw.Close()
        Return fname
    End Function
    Sub FerringFarmaAbsentReportConsolidated()
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt11 As New DataTable
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dt2 As New DataTable
        Dim lv As New DataTable
        Dim fndt As New DataTable
        Dim mnt As Integer
        Dim currmntName As String
        Dim prmntName As String
        mnt = Microsoft.VisualBasic.DateAndTime.Month(Now.Date)
        If mnt = 1 Then
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(12)
        Else
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt - 1)
        End If
        currmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt)
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where eid=57 and iscommon<>'1' and tid=292 order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                ' If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "EMAIL" Then
                    Dim eid As String = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                    Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                    Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                    Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                    Dim fdate As String = ds.Tables("rpt").Rows(d).Item("fromdate").ToString()
                    Dim tdate As String = ds.Tables("rpt").Rows(d).Item("todate").ToString()
                    oda.SelectCommand.CommandText = "select " & fdate & "[fdate]," & tdate & "[tdate]"
                    oda.Fill(ds, "date")
                    Dim Nfdate As String = ds.Tables("date").Rows(0).Item("fdate").ToString
                    Dim Ntdate As String = ds.Tables("date").Rows(0).Item("tdate").ToString
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "select distinct fld1 from mmm_mst_master with(nolock) where eid=57 and documenttype='Employee Master' and isauth=1 and fld15<>'Alumni' and fld1 not in ('FPPL304','FPPL214')"
                    oda.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetAtt_mismatchnew"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("EID", 57)
                            oda.SelectCommand.Parameters.AddWithValue("fdate", Nfdate)
                            oda.SelectCommand.Parameters.AddWithValue("tdate", Ntdate)
                            oda.Fill(dt1)
                        Next
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    'oda.SelectCommand.CommandText = "set dateformat dmy;select distinct d.fld1,d.fld11,d.fld12 from mmm_mst_doc d with(nolock) inner join mmm_mst_master m on m.fld1=d.fld1 where d.eid=57 and d.documenttype='Leave Application' and m.documenttype='Employee Master' and m.isauth=1 and m.fld15<>'Alumni' and convert(date,d.fld11)>=convert(date," & fdate & ") and convert(date,d.fld12)<=convert(date," & tdate & ") and d.fld14 in ('approved')"
                    oda.SelectCommand.CommandText = "set dateformat dmy;select distinct d.fld1,d.fld11,d.fld12 from mmm_mst_doc d with(nolock) inner join mmm_mst_master m on m.fld1=d.fld1 where d.eid=57 and d.documenttype='Leave Application' and m.documenttype='Employee Master' and m.isauth=1 and m.fld15<>'Alumni' and ((convert(date," & fdate & ")<=convert(date,d.fld11) and  convert(date," & tdate & ")>=convert(date,d.fld12)) or (convert(date," & tdate & ")>=convert(date,d.fld11)) and convert(date,d.fld12)>=convert(date," & fdate & ")) and d.fld14 in ('approved') and m.fld1 not in ('FPPL304','FPPL214')"
                    oda.Fill(dt2)
                    If dt2.Rows.Count > 0 Then
                        For i As Integer = 0 To dt2.Rows.Count - 1
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetEmpLeaveDtl"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt2.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("dt1", dt2.Rows(i).Item(1).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("dt2", dt2.Rows(i).Item(2).ToString)
                            oda.Fill(lv)
                        Next
                    End If
                    Dim rows_to_remove As New List(Of DataRow)()
                    For Each row1 As DataRow In dt1.Rows
                        For Each row2 As DataRow In lv.Rows
                            If (row1.Item(0).ToString() = row2.Item(0).ToString()) And (row1.Item(2).ToString() = row2.Item(2).ToString()) And (row1.Item(3).ToString() = row2.Item(3).ToString()) Then
                                rows_to_remove.Add(row1)
                            End If
                        Next
                    Next
                    For Each row As DataRow In rows_to_remove
                        dt1.Rows.Remove(row)
                        dt1.AcceptChanges()
                    Next
                    Dim fname As String = ""
                    fname = CreateCSV(dt1, FPath)
                    mailsub = mailsub.Replace("{Previous Month}", prmntName)
                    mailsub = mailsub.Replace("{Current Month}", currmntName)
                    msg = msg.Replace("{Previous Month}", prmntName)
                    msg = msg.Replace("{Current Month}", currmntName)
                    Dim obj As New MailUtill(eid:=eid)
                    obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, Attachments:=FPath + fname, BCC:=Bcc)
                    ' obj.SendMail(ToMail:="prashant.singh@myndsol.com", Subject:=mailsub, MailBody:=msg, CC:="prashant.singh@myndsol.com", Attachments:=FPath + fname, BCC:="vishal.kumar@myndsol.com")
                End If
                ' End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "UPDATE  mmm_mst_reportscheduler SET LASTSCHEDULEDDATE=GETDATE() WHERE TID=" & ds.Tables("rpt").Rows(d).Item("tid") & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Next
            lblMsg.Text = "Mail sent successfully."
            lblMsg.ForeColor = Drawing.Color.Green
        Catch ex As Exception
            lblMsg.Text = "Exception occured at server. Please contact your system administrator !"
            lblMsg.ForeColor = Drawing.Color.Red
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "Ferring")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 57)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Sub FerringFarmaAbsentReport()
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt11 As New DataTable
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dt2 As New DataTable
        Dim lv As New DataTable
        Dim fndt As New DataTable
        Dim mnt As Integer
        Dim currmntName As String
        Dim prmntName As String
        mnt = Microsoft.VisualBasic.DateAndTime.Month(Now.Date)
        If mnt = 1 Then
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(12)
        Else
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt - 1)
        End If
        currmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt)
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where eid=57 and iscommon<>'1' and tid=284 order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                'If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "EMAIL" Then
                    Dim eid As String = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                    Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                    Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                    Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                    'Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                    Dim fdate As String = ds.Tables("rpt").Rows(d).Item("fromdate").ToString()
                    Dim tdate As String = ds.Tables("rpt").Rows(d).Item("todate").ToString()
                    oda.SelectCommand.CommandText = "select " & fdate & "[fdate]," & tdate & "[tdate]"
                    oda.Fill(ds, "date")
                    Dim Nfdate As String = ds.Tables("date").Rows(0).Item("fdate").ToString
                    Dim Ntdate As String = ds.Tables("date").Rows(0).Item("tdate").ToString
                    oda.SelectCommand.CommandText = "select distinct  fld1,fld19,fld23 from mmm_mst_master with(nolock) where eid=57 and documenttype='Employee Master' and isauth=1 and fld15<>'Alumni' and fld1 not in ('FPPL304','FPPL214')"
                    oda.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            dt2.Clear()
                            lv.Clear()
                            dt1.Clear()
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetAtt_mismatchnew"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("EID", 57)
                            oda.SelectCommand.Parameters.AddWithValue("fdate", Nfdate)
                            oda.SelectCommand.Parameters.AddWithValue("tdate", Ntdate)
                            oda.Fill(dt1)
                            If dt1.Rows.Count > 0 Then
                            Else
                                Continue For
                            End If
                            oda.SelectCommand.CommandType = CommandType.Text
                            'oda.SelectCommand.CommandText = "set dateformat dmy;select distinct fld1,fld11,fld12 from mmm_mst_doc with(nolock) where eid=57 and documenttype='Leave Application' and convert(date,fld11)>=convert(date," & fdate & ") and convert(date,fld12)<=convert(date," & tdate & ") and fld1 ='" & dt.Rows(i).Item(0).ToString & "' and fld14 in ('approved')"
                            oda.SelectCommand.CommandText = "set dateformat dmy;select distinct fld1,fld11,fld12 from mmm_mst_doc with(nolock) where eid=57 and documenttype='Leave Application' and ((convert(date,'" & Nfdate & "')<=convert(date,fld11) and  convert(date,'" & Ntdate & "')>=convert(date,fld12)) or (convert(date,'" & Ntdate & "')>=convert(date,fld11)) and convert(date,fld12)>=convert(date,'" & Nfdate & "')) and fld1 ='" & dt.Rows(i).Item(0).ToString & "' and fld14 in ('approved')"
                            oda.Fill(dt2)
                            If dt2.Rows.Count > 0 Then
                                For j As Integer = 0 To dt2.Rows.Count - 1
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.CommandText = "uspgetEmpLeaveDtl"
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("empcode", dt2.Rows(j).Item(0).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("dt1", dt2.Rows(j).Item(1).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("dt2", dt2.Rows(j).Item(2).ToString)
                                    oda.Fill(lv)
                                Next
                            End If
                            Dim rows_to_remove As New List(Of DataRow)()
                            If lv.Rows.Count > 0 Then
                                For Each row1 As DataRow In dt1.Rows
                                    For Each row2 As DataRow In lv.Rows
                                        If (row1.Item(0).ToString() = row2.Item(0).ToString()) And (row1.Item(2).ToString() = row2.Item(2).ToString()) And (row1.Item(3).ToString() = row2.Item(3).ToString()) Then
                                            rows_to_remove.Add(row1)
                                        End If
                                    Next
                                Next
                                For Each row As DataRow In rows_to_remove
                                    dt1.Rows.Remove(row)
                                    dt1.AcceptChanges()
                                Next
                            End If
                            If dt1.Rows.Count < 1 Then
                                Continue For
                            End If
                            Dim fname As String = ""
                            ' fname = CreateCSV(dt1, FPath)
                            Dim MailTable As New StringBuilder()
                            MailTable.Append("<table border=""1"" width=""100%"">")
                            MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")

                            For l As Integer = 0 To dt1.Columns.Count - 1
                                MailTable.Append("<td >" & dt1.Columns(l).ColumnName & "</td>")
                            Next

                            For k As Integer = 0 To dt1.Rows.Count - 1 ' binding the tr tab in table
                                MailTable.Append("</tr><tr>") ' for row records
                                For t As Integer = 0 To dt1.Columns.Count - 1
                                    MailTable.Append("<td>" & dt1.Rows(k).Item(t).ToString() & " </td>")
                                Next
                                MailTable.Append("</tr>")
                            Next
                            MailTable.Append("</table>")

                            Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                            mailsub = mailsub.Replace("{Previous Month}", prmntName)
                            mailsub = mailsub.Replace("{Current Month}", currmntName)
                            Dim strmsgBdy As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                            If strmsgBdy.Contains("@body") Then
                                strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                msg = strmsgBdy
                            Else
                                msg = MailTable.ToString()
                            End If
                            msg = strmsgBdy
                            msg = msg.Replace("{Employee Name}", dt1.Rows(0).Item(1).ToString)
                            msg = msg.Replace("{Previous Month}", prmntName)
                            msg = msg.Replace("{Current Month}", currmntName)
                            Dim obj As New MailUtill(eid:=eid)
                            obj.SendMail(ToMail:=dt.Rows(i).Item(1).ToString, Subject:=mailsub, MailBody:=msg, CC:=dt.Rows(i).Item(2).ToString, Attachments:="", BCC:=Bcc)
                            'obj.SendMail(ToMail:="vishal.kumar@myndsol.com", Subject:=mailsub, MailBody:=msg, CC:="vishal.kumar@myndsol.com", Attachments:="", BCC:="vishal.kumar@myndsol.com")
                        Next
                    End If
                End If
                'End If
            Next
            lblMsg.Text = "Mail sent successfully."
            lblMsg.ForeColor = Drawing.Color.Green
        Catch ex As Exception
            lblMsg.Text = "Exception occured at server. Please contact your system administrator !"
            lblMsg.ForeColor = Drawing.Color.Red
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "Ferring")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 57)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        lblMsg.Text = ""
        FerringFarmaAbsentReport()
    End Sub
    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        lblMsg.Text = ""
        FerringFarmaAbsentReportConsolidated()
    End Sub

    Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ferringAbsentConsTestReport()
    End Sub
    Sub ferringAbsentConsTestReport()
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt11 As New DataTable
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dt2 As New DataTable
        Dim lv As New DataTable
        Dim fndt As New DataTable
        Dim mnt As Integer
        Dim currmntName As String
        Dim prmntName As String
        mnt = Microsoft.VisualBasic.DateAndTime.Month(Now.Date)
        If mnt = 1 Then
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(12)
        Else
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt - 1)
        End If
        currmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt)
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where eid=57 and iscommon<>'1' and tid=292 order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                ' If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "EMAIL" Then
                    Dim eid As String = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                    Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                    Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                    Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                    Dim fdate As String = ds.Tables("rpt").Rows(d).Item("fromdate").ToString()
                    Dim tdate As String = ds.Tables("rpt").Rows(d).Item("todate").ToString()
                    oda.SelectCommand.CommandText = "select " & fdate & "[fdate]," & tdate & "[tdate]"
                    oda.Fill(ds, "date")
                    Dim Nfdate As String = ds.Tables("date").Rows(0).Item("fdate").ToString
                    Dim Ntdate As String = ds.Tables("date").Rows(0).Item("tdate").ToString
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "select distinct fld1 from mmm_mst_master with(nolock) where eid=57 and documenttype='Employee Master' and isauth=1 and fld15<>'Alumni' and fld1 not in ('FPPL304','FPPL214')"
                    oda.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetAtt_mismatchnew"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("EID", 57)
                            oda.SelectCommand.Parameters.AddWithValue("fdate", Nfdate)
                            oda.SelectCommand.Parameters.AddWithValue("tdate", Ntdate)
                            oda.Fill(dt1)
                        Next
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    'oda.SelectCommand.CommandText = "set dateformat dmy;select distinct d.fld1,d.fld11,d.fld12 from mmm_mst_doc d with(nolock) inner join mmm_mst_master m on m.fld1=d.fld1 where d.eid=57 and d.documenttype='Leave Application' and m.documenttype='Employee Master' and m.isauth=1 and m.fld15<>'Alumni' and convert(date,d.fld11)>=convert(date," & fdate & ") and convert(date,d.fld12)<=convert(date," & tdate & ") and d.fld14 in ('approved')"
                    oda.SelectCommand.CommandText = "set dateformat dmy;select distinct d.fld1,d.fld11,d.fld12 from mmm_mst_doc d with(nolock) inner join mmm_mst_master m on m.fld1=d.fld1 where d.eid=57 and d.documenttype='Leave Application' and m.documenttype='Employee Master' and m.isauth=1 and m.fld15<>'Alumni' and ((convert(date," & fdate & ")<=convert(date,d.fld11) and  convert(date," & tdate & ")>=convert(date,d.fld12)) or (convert(date," & tdate & ")>=convert(date,d.fld11)) and convert(date,d.fld12)>=convert(date," & fdate & ")) and d.fld14 in ('approved') and m.fld1 not in ('FPPL304','FPPL214')"
                    oda.Fill(dt2)
                    If dt2.Rows.Count > 0 Then
                        For i As Integer = 0 To dt2.Rows.Count - 1
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetEmpLeaveDtl"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt2.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("dt1", dt2.Rows(i).Item(1).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("dt2", dt2.Rows(i).Item(2).ToString)
                            oda.Fill(lv)
                        Next
                    End If
                    Dim rows_to_remove As New List(Of DataRow)()
                    For Each row1 As DataRow In dt1.Rows
                        For Each row2 As DataRow In lv.Rows
                            If (row1.Item(0).ToString() = row2.Item(0).ToString()) And (row1.Item(2).ToString() = row2.Item(2).ToString()) And (row1.Item(3).ToString() = row2.Item(3).ToString()) Then
                                rows_to_remove.Add(row1)
                            End If
                        Next
                    Next
                    For Each row As DataRow In rows_to_remove
                        dt1.Rows.Remove(row)
                        dt1.AcceptChanges()
                    Next
                    Dim fname As String = ""
                    fname = CreateCSV(dt1, FPath)
                    mailsub = mailsub.Replace("{Previous Month}", prmntName)
                    mailsub = mailsub.Replace("{Current Month}", currmntName)
                    msg = msg.Replace("{Previous Month}", prmntName)
                    msg = msg.Replace("{Current Month}", currmntName)
                    Dim obj As New MailUtill(eid:=eid)
                    ' obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, Attachments:=FPath + fname, BCC:=Bcc)
                    obj.SendMail(ToMail:="nirmal.kumar@myndsol.com,nitish.pandey@myndsol.com", Subject:=mailsub, MailBody:=msg, CC:="nirmal.kumar@myndsol.com", Attachments:=FPath + fname, BCC:="vishal.kumar@myndsol.com")
                End If
                ' End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "UPDATE  mmm_mst_reportscheduler SET LASTSCHEDULEDDATE=GETDATE() WHERE TID=" & ds.Tables("rpt").Rows(d).Item("tid") & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Next
            lblMsg.Text = "Mail sent successfully."
            lblMsg.ForeColor = Drawing.Color.Green
        Catch ex As Exception
            lblMsg.Text = "Exception occured at server. Please contact your system administrator !"
            lblMsg.ForeColor = Drawing.Color.Red

            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "Ferring")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 57)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
End Class
