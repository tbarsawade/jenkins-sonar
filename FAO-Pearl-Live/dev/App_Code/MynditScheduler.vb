Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient



Public Class MynditScheduler
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Sub AlertProjection()

        Dim dt As New DataTable()
        Try

            If (DateTime.Now.DayOfWeek = DayOfWeek.Wednesday Or DateTime.Now.DayOfWeek = DayOfWeek.Friday) Then
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter("select UserName, Emailid  from mmm_mst_user  where eid =100 and  Userrole <> 'SU' and Userrole<> 'MANAGEMENT' and uid <> 8974 and uid not in (select distinct fld3 from mmm_mst_doc where eid =100 and documenttype ='task document' and fld6 = datepart(week, getdate())+1) order by username", con)
                        da.SelectCommand.CommandType = CommandType.Text
                        da.Fill(dt)
                        For i As Integer = 0 To dt.Rows.Count - 1
                            Dim objcls = New MailUtill(100)
                            Dim strbody As String = "  <br /> Dear " & dt.Rows(i).Item("UserName").ToString() & ",<br /> <br /> Please fill your task projections for the next week.<br />  <br /> <br /> Thanks <br/> IT Helpdesk"

                            objcls.SendMail(dt.Rows(i).Item("Emailid").ToString(), "Tasks Alert!!", strbody, "", "", "")
                        Next

                    End Using
                End Using
            End If

        Catch ex As Exception
            Throw
        End Try

    End Sub


    Sub AlertActualHrsPendingTasks()

        Dim dt As New DataTable()
        Try
            If (DateTime.Now.DayOfWeek = DayOfWeek.Wednesday Or DateTime.Now.DayOfWeek = DayOfWeek.Saturday) Then
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter("select UserName, Emailid  from mmm_mst_user  where eid =100 and  Userrole <> 'SU' and Userrole<> 'MANAGEMENT'  and uid <> 8974  and uid not in (select distinct fld3 from mmm_mst_doc where eid =100 and documenttype ='task document' and fld6 = datepart(week, getdate()) and fld13 is not null) order by username ", con)
                        da.SelectCommand.CommandType = CommandType.Text
                        da.Fill(dt)

                        For i As Integer = 0 To dt.Rows.Count - 1
                            Dim objcls = New MailUtill(100)
                            Dim strbody As String = "  <br /> Dear " & dt.Rows(i).Item("UserName").ToString() & ",<br /> <br /> Please Submit your tasks for the week.<br />  <br /> <br />Thanks <br/> IT Helpdesk"
                            objcls.SendMail(dt.Rows(i).Item("Emailid").ToString(), "Tasks Alert!!", strbody, "", "", "")
                        Next

                    End Using
                End Using
            End If

        Catch ex As Exception
            Throw
        End Try

    End Sub

    Sub ConsolidatedTasksSheet()

        Dim dt As New DataTable()
        Dim dt2 As New DataTable()
        Dim objcls = New MailUtill(100)
        Dim strbody As String = ""

        Try
            If (DateTime.Now.DayOfWeek = DayOfWeek.Friday Or DateTime.Now.DayOfWeek = DayOfWeek.Saturday) Then
                strbody &= "  <br /> Dear Sir ,<br />  <br /> "
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter("select UserName, Emailid  from mmm_mst_user  where eid =100 and  Userrole <> 'SU' and Userrole<> 'MANAGEMENT'  and uid <> 8974  and uid not in (select distinct fld3 from mmm_mst_doc where eid =100 and documenttype ='task document' and fld6 = datepart(week, getdate())+1) order by username ", con)
                        da.SelectCommand.CommandType = CommandType.Text
                        da.Fill(dt)
                        da.SelectCommand.CommandText = "select UserName, Emailid  from mmm_mst_user  where eid =100 and  Userrole <> 'SU' and Userrole<> 'MANAGEMENT'  and uid <> 8974  and uid not in (select distinct fld3 from mmm_mst_doc where eid =100 and documenttype ='task document' and fld6 = datepart(week, getdate()) and fld13 is not null) order by username"
                        da.Fill(dt2)

                        If (dt.Rows.Count > 0) Then

                            strbody &= " Following Users have not submitted Projected task hours for the next week : <br />"

                            For i As Integer = 0 To dt.Rows.Count - 1

                                strbody &= (i + 1).ToString() & ") " & dt.Rows(i).Item("UserName").ToString() & "  <br /> "

                            Next
                            strbody &= "<br />"
                        End If

                        If (dt2.Rows.Count > 0) Then

                            strbody &= " Following Users have not submitted actual task hours for the week : <br />"

                            For i As Integer = 0 To dt2.Rows.Count - 1

                                strbody &= (i + 1).ToString() & ") " & dt2.Rows(i).Item("UserName").ToString() & "  <br /> "

                            Next
                            strbody &= "<br />"
                        End If

                        strbody &= "<br/> Thanks <br/> IT Helpdesk"

                        If (dt.Rows.Count > 0 Or dt2.Rows.Count > 0) Then
                            objcls.SendMail("manish@myndsol.com", "Tasks Alert!!", strbody, "garima.paliwal@myndsol.com", "", "")
                        End If

                    End Using
                End Using
            End If

        Catch ex As Exception
            Throw
        End Try

    End Sub

    '   Sub PendingTasksMail()

    '       Dim dt As New DataTable()
    '       Dim objcls = New MailUtill(100)
    '       Dim strbody As String = ""
    '       Try
    '           If (DateTime.Now.DayOfWeek = DayOfWeek.Wednesday Or DateTime.Now.DayOfWeek = DayOfWeek.Saturday) Then
    '               Using con = New SqlConnection(conStr)
    '                   Using da = New SqlDataAdapter("select dms.udf_split('MASTER-Project Master-fld1',fld1)[Project Name],dms.udf_split('MASTER-Module-fld2',fld11)[Module],fld2[Task],dms.udf_split('STATIC-USER-UserName',fld3)[Assign To], " &
    '" fld6[Planned week],fld7[Actual Week],fld4[Planned Hrs],fld13 [Actual hrs], curstatus [Status]  from mmm_mst_doc where eid =100 and Documenttype ='task document' and curstatus <> 'Archive' and fld6= datepart(week,getdate())-3 ", con)
    '                       da.SelectCommand.CommandType = CommandType.Text
    '                       da.Fill(dt)

    '                       If (dt.Rows.Count > 0) Then

    '                           strbody &= "  <br /> Dear Sir ,<br /> <br /> Following tasks have been pending for 2 weeks: <br />  <br /> "
    '                           strbody &= " <table style='width:100%;'> <tr><td> <b> Project Name </b></td><td> <b>Module</b></td> <td> <b>Task</b></td><td><b>Assign To</b></td> " &
    '                    " <td><b>Planned week</b></td><td><b>Actual Week</b></td> <td><b>Planned Hrs</b></td> <td><b>Actual Hrs</b></td>   <td><b>Status</b></td> </tr>"

    '                           For i As Integer = 0 To dt.Rows.Count - 1

    '                               strbody &= " <tr> <td>" & dt.Rows(i).Item("Project Name").ToString() & "</td> <td>" & dt.Rows(i).Item("Module").ToString() & "</td><td>" & dt.Rows(i).Item("Task").ToString() & " </td>" &
    '                 "  <td>" & dt.Rows(i).Item("Assign To").ToString() & "</td><td> " & dt.Rows(i).Item("Planned week").ToString() & "</td> <td>" & dt.Rows(i).Item("Actual Week").ToString() & "</td> <td>" & dt.Rows(i).Item("Planned Hrs").ToString() & "</td> " &
    '                  " <td>" & dt.Rows(i).Item("Actual hrs").ToString() & "</td> <td>" & dt.Rows(i).Item("Status").ToString() & "</td> </tr>"

    '                           Next

    '                           objcls.SendMail("manish@myndsol.com", "Tasks Alert!!", strbody, "garima.paliwal@myndsol.com", "", "")

    '                       End If
    '                   End Using
    '               End Using
    '           End If

    '       Catch ex As Exception
    '           Throw
    '       End Try
    '   End Sub













End Class
