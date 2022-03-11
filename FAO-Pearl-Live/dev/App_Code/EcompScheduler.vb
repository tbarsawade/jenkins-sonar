Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading

Public Class EcompScheduler
    


    Sub RunScheduler()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dt As New DataTable
        Dim tran As SqlTransaction = Nothing
        Dim cmd As New SqlCommand()
        
        cmd.Connection = con
        cmd.CommandText = "select count(*)[rows] from mmm_schedulerlog where convert(date,latrundatetime) = convert(date,getdate())"
        con.Open()
        Dim rows As Integer = cmd.ExecuteScalar()
        con.Close()
        If (rows = 0) Then
            RunSchedulerPE()
            RunSchedulerContractor()
        End If



    End Sub


    Sub RunSchedulerPE()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dt As New DataTable
        Dim tran As SqlTransaction = Nothing
        Dim cmd As New SqlCommand()
        Dim ds As New DataSet()

        Dim duplicatecounter As Integer = 0
        cmd.Connection = con
        cmd.CommandText = "insert into mmm_schedulerlog (eid,schedulerName,SuccessCount,FailedCount,LatRundatetime,Remarks) values (98,'EcomplianceScheduler',0,0,getdate(),'Enter in PE Activity')"
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
        Dim SuccessCount As Integer = 0
        Dim FailedCount As Integer = 0
        Dim objerrormail = New MailUtill(98)
        'Try
        ' For :List of all PE and contractor Activities for which act document is to be created 
        Dim da As New SqlDataAdapter("select actsapplicable.tid[ActID1],SiteMaster.fld100 [Site Name],CompanyMaster.fld106 [CompanyCode], (case when actsapplicable.fld8 is not null and actsapplicable.fld8<>0 then actsapplicable.fld8  " &
" when SiteMaster.fld113  is not null and  SiteMaster.fld113 <> 0 then SiteMaster.fld113 when CompanyMaster.fld109 is not null and  CompanyMaster.fld109 <> 0 then CompanyMaster.fld109 else 0 end) [Maker]" &
" ,(case when actsapplicable.fld9 is not null and actsapplicable.fld9 <>0 then actsapplicable.fld9  " &
" when SiteMaster.fld120  is not null and  SiteMaster.fld120 <> 0 then SiteMaster.fld120 when CompanyMaster.fld110 is not null and  CompanyMaster.fld110 <> 0 then CompanyMaster.fld110 else 0 end) Checker " &
" ,actsapplicable.fld1 [SiteID],dms.udf_split('MASTER-Company Master-fld1', actsapplicable.fld5)[Company Name],actsapplicable.fld5 [CompanyID],actsapplicable.fld3 [ActID],actsapplicable.fld4[Act] ,State.fld1 [State Name], State.tid [StateID] " &
 " from mmm_mst_master actsapplicable  inner join  mmm_mst_master SiteMaster on actsapplicable.fld1= SiteMaster.tid inner join mmm_mst_master State on SiteMaster.fld101 = State.tid    inner join  mmm_mst_master CompanyMaster on actsapplicable.fld5= CompanyMaster.tid " &
 " where actsapplicable.eid=98 and actsapplicable.Documenttype='Acts applicable to Site' and dms.udf_split('MASTER-Company Master-fld1', actsapplicable.fld5) not like '%telenor%'; " &
         "select activity.tid,activity.fld112[Activity],activity.fld113[Year], activity.fld108 [Activity_type],activity.fld107, dms.udf_split('MASTER-Act Master-fld1',activity.fld1)[Act],activity.fld1 [ActID],activity.fld101 [DOM],activity.fld102 [RemindDays],activity.fld109 [DOY],activity.fld110 [Month], " &
  " activity.fld111 [DOW], dms.udf_split('MASTER-Frequency Master-fld1',activity.fld100) [Frequency] from mmm_mst_master activity where eid=98 and documenttype='Activity Master'  and fld108='PE' ", con)

        da.Fill(ds)

        dt = ds.Tables(0)

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim dtActivities As New DataTable
            Dim dv As New DataView(ds.Tables(1), " fld107 like '%" & dt.Rows(i).Item("StateID") & "%' and ActID = '" & dt.Rows(i).Item("ActID").ToString() & "'  ", "", DataViewRowState.CurrentRows)
            dtActivities = dv.ToTable()
            '' Commented by pallavi on 24 th  May to reduce db calls 


            '          da = New SqlDataAdapter("select activity.tid,activity.fld112[Activity],activity.fld113[Year], activity.fld108 [Activity_type],dms.udf_split('MASTER-Act Master-fld1',activity.fld1)[Act],activity.fld1 [ActID],activity.fld101 [DOM],activity.fld102 [RemindDays],activity.fld109 [DOY],activity.fld110 [Month], " &
            '" activity.fld111 [DOW], dms.udf_split('MASTER-Frequency Master-fld1',activity.fld100) [Frequency] from mmm_mst_master activity where eid=98 and documenttype='Activity Master' and  fld107 like '%" & dt.Rows(i).Item("StateID") & "%' and fld108='PE' and fld1='" & dt.Rows(i).Item("ActID").ToString() & "'  ", con)

            '          da.Fill(dtActivities)

            '' Commented by pallavi on 24 th  May to reduce db calls 
            For j As Integer = 0 To dtActivities.Rows.Count - 1
                con = New SqlConnection(conStr)

                Try
                    If (CheckScheduler(dtActivities.Rows(j).Item("Frequency").ToString(), dtActivities.Rows(j).Item("DOM").ToString(), dtActivities.Rows(j).Item("DOY").ToString(), dtActivities.Rows(j).Item("Month").ToString(), dtActivities.Rows(j).Item("DOW").ToString(), dtActivities.Rows(j).Item("RemindDays").ToString(), dtActivities.Rows(j).Item("Year").ToString())) Then

                        Dim objdocutil As New DocUtils
                        Dim dt1 As Date = ReturnDate(dtActivities.Rows(j).Item("Frequency").ToString(), dtActivities.Rows(j).Item("DOM").ToString(), dtActivities.Rows(j).Item("DOY").ToString(), dtActivities.Rows(j).Item("Month").ToString(), dtActivities.Rows(j).Item("DOW").ToString(), dtActivities.Rows(j).Item("RemindDays").ToString())

                        Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                        Dim con1 As New SqlConnection(conStr1)
                        Dim cmd1 As New SqlCommand()

                        'cmd1.Connection = con1
                        'cmd1.CommandText = "select count(*) from mmm_mst_doc where fld5='" & dt.Rows(i).Item("CompanyID").ToString() & "' and fld7 ='" & dt.Rows(i).Item("SiteID").ToString() & "' and fld2 ='" & dt.Rows(i).Item("ActID1").ToString() & "' and fld24='" & dtActivities.Rows(j).Item("tid").ToString() & "' and convert(date,convert(datetime,fld43,3)) = convert(date,'" & dt1 & "')"
                        'con1.Open()
                        'Dim rows As Integer = cmd1.ExecuteScalar()
                        'con1.Close()

                        'duplicatecounter += 1
                        'If (rows = 0) Then

                        '    duplicatecounter += 1

                        da = New SqlDataAdapter("INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source,fld25,fld5,fld7,fld2,fld24,fld29,fld30,fld31,fld47,fld6,fld43) " &
                             " VALUES (98,'Act Document','" & dt.Rows(i).Item("Maker").ToString() & "',getdate(),'Scheduler','" & dtActivities.Rows(j).Item("Activity_Type").ToString() & "', " &
                             " '" & dt.Rows(i).Item("CompanyID").ToString() & "','" & dt.Rows(i).Item("SiteID").ToString() & "','" & dt.Rows(i).Item("ActID1").ToString() & "','" & dtActivities.Rows(j).Item("tid").ToString() & "','" & dt.Rows(i).Item("Company Name").ToString() & "'," &
                             " '" & dt.Rows(i).Item("Site Name").ToString() & "','" & dt.Rows(i).Item("Act").ToString() & "','" & dtActivities.Rows(j).Item("Activity").ToString() & "','" & dt.Rows(i).Item("CompanyCode").ToString() & "','" & dt1.Day.ToString() & "/" & dt1.Month.ToString() & "/" & dt1.Year.ToString().Substring(2) & "'); select @@Identity", con)

                        Dim Docid As String = ""
                        con.Open()
                        tran = con.BeginTransaction()

                        'Transactional Query

                        da.SelectCommand.Transaction = tran
                        Docid = da.SelectCommand.ExecuteScalar().ToString()
                        objdocutil.ExecuteCommanModule(98, "Act Document", "Document", Docid, dt.Rows(i).Item("Maker").ToString(), con, tran)
                        objdocutil.ExecuteDocumentModuleModule(98, "Act Document", "Document", Docid, dt.Rows(i).Item("Maker").ToString(), con, tran)
                        objdocutil.ExecuteUserModule(98, "Act Document", "Document", Docid, dt.Rows(i).Item("Maker").ToString(), con, tran)

                        tran.Commit()

                        Try

                            Dim strparams As String = Docid & "||" & Convert.ToString(dt.Rows(i).Item("Maker"))
                            ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, strparams)

                            ' objdocutil.ExecuteOtherUtils(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")))



                        Catch ex As Exception
                            objerrormail.SendMail("pallavi.bhardwaj@myndsol.com", "Error in ecomp scheduler mail shoot!", " Error occured while ExecuteOtherUtils:' " & ex.Message & " '", "ajeet.kumar@myndsol.com")
                            'Please note.Not throwing any exception from this block because it wont impact any BPM Activity
                        End Try
                        SuccessCount += 1

                    End If

                    ' For Contractor Types  Creation of Act Doc 
                    'da = New SqlDataAdapter("INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source,fld25,fld5,fld7,fld2,fld24,fld29,fld30,fld31,fld47,fld6,fld43) " &
                    '     " VALUES (98,'Act Document','" & dt.Rows(i).Item("Maker").ToString() & "',getdate(),'Scheduler','" & dtActivities.Rows(j).Item("Activity_Type").ToString() & "', " &
                    '     " '" & dt.Rows(i).Item("CompanyID").ToString() & "','" & dt.Rows(i).Item("SiteID").ToString() & "','" & dt.Rows(i).Item("ActID1").ToString() & "','" & dtActivities.Rows(j).Item("tid").ToString() & "','" & dt.Rows(i).Item("Company Name").ToString() & "'," &
                    '     " '" & dt.Rows(i).Item("Site Name").ToString() & "','" & dt.Rows(i).Item("Act").ToString() & "','" & dtActivities.Rows(j).Item("Activity").ToString() & "','" & dt.Rows(i).Item("CompanyCode").ToString() & "','" & dt1.Day.ToString() & "/" & dt1.Month.ToString() & "/" & dt1.Year.ToString().Substring(2) & "'); select @@Identity", con)

                    'Dim Docid As String = ""
                    'con.Open()
                    'tran = con.BeginTransaction()

                    ''Transactional Query

                    'da.SelectCommand.Transaction = tran
                    'Docid = da.SelectCommand.ExecuteScalar().ToString()
                    'objdocutil.ExecuteCommanModule(98, "Act Document", "Document", Docid, dt.Rows(i).Item("Maker").ToString(), con, tran)
                    'objdocutil.ExecuteDocumentModuleModule(98, "Act Document", "Document", Docid, dt.Rows(i).Item("Maker").ToString(), con, tran)
                    'objdocutil.ExecuteUserModule(98, "Act Document", "Document", Docid, dt.Rows(i).Item("Maker").ToString(), con, tran)

                    'tran.Commit()
                    'SuccessCount += 1
                    'End If



                Catch ex As Exception
                    objerrormail.SendMail("pallavi.bhardwaj@myndsol.com", "Error in ecomp scheduler Actvites!", " Error occured while loop on activities:' " & ex.Message & " '", "ajeet.kumar@myndsol.com")
                    FailedCount += 1

                    Try
                        tran.Rollback()
                    Catch ex1 As Exception

                    End Try

                Finally

                    If (con.State = ConnectionState.Open) Then
                        con.Close()
                    End If

                End Try
            Next

        Next


        cmd.Connection = con
        cmd.CommandText = "insert into mmm_schedulerlog (eid,schedulerName,SuccessCount,FailedCount,LatRundatetime,Remarks) values (98,'EcomplianceScheduler'," & SuccessCount & "," & FailedCount & ",getdate(),'Exit from PE Activity')"
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
    End Sub
    
    Sub RunSchedulerContractor()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dt As New DataTable
        Dim tran As SqlTransaction = Nothing
        Dim cmd As New SqlCommand()
        cmd.Connection = con
        cmd.CommandText = "insert into mmm_schedulerlog (eid,schedulerName,SuccessCount,FailedCount,LatRundatetime,Remarks) values (98,'EcomplianceScheduler',0,0,getdate(),'Enter in Contractor Activity')"
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
        Dim SuccessCount As Integer = 0
        Dim FailedCount As Integer = 0
        Dim ds As New DataSet()
        ' Dim duplicatecounter As Integer = 0
        Dim objerrormail = New MailUtill(98)
        'Try
        ' For :List of all PE and contractor Activities for which act document is to be created 
        Dim da As New SqlDataAdapter("select actsapplicable.tid[ActID1], SiteMaster.fld100 [Site Name],CompanyMaster.fld106 [CompanyCode], (case when actsapplicable.fld8 is not null and actsapplicable.fld8<>0 then actsapplicable.fld8  " &
" when SiteMaster.fld113  is not null and  SiteMaster.fld113 <> 0 then SiteMaster.fld113 when CompanyMaster.fld109 is not null and  CompanyMaster.fld109 <> 0 then CompanyMaster.fld109 else 0 end) Maker  " &
" ,(case when actsapplicable.fld9 is not null and actsapplicable.fld9 <>0 then actsapplicable.fld9  " &
" when SiteMaster.fld120  is not null and  SiteMaster.fld120 <> 0 then SiteMaster.fld120 when CompanyMaster.fld110 is not null and  CompanyMaster.fld110 <> 0 then CompanyMaster.fld110 else 0 end) Checker " &
" , actsapplicable.fld1 [SiteID],dms.udf_split('MASTER-Company Master-fld1', actsapplicable.fld5)[Company Name], " &
" actsapplicable.fld5 [CompanyID],actsapplicable.fld3 [ActID],actsapplicable.fld4[Act] ,State.fld1 [State Name], State.tid [StateID] , ContractorMas.fld1[Contractor], ContractorMas.tid [ContractorID],SiteContractMas.tid [SiteContMasID] " &
  " from mmm_mst_master actsapplicable     inner join  mmm_mst_master SiteMaster on actsapplicable.fld1= SiteMaster.tid   inner join mmm_mst_master State on SiteMaster.fld101 = State.tid  " &
  " inner join  mmm_mst_master CompanyMaster on actsapplicable.fld5= CompanyMaster.tid " &
  " inner join mmm_mst_master SiteContractMas on SiteContractMas.fld1 = actsapplicable.fld5 and SiteContractMas.fld3 = SiteMaster.tid inner join mmm_mst_master ContractorMas on SiteContractMas.fld4 = ContractorMas.tid " &
  " where actsapplicable.eid=98 and actsapplicable.Documenttype='Acts applicable to Site'  and SiteContractMas.eid =98 and SiteContractMas.Documenttype='Site Contractor MAster';  " &
      "select activity.tid,activity.fld112[Activity],activity.fld113[Year], activity.fld108 [Activity_type],activity.fld107, dms.udf_split('MASTER-Act Master-fld1',activity.fld1)[Act],activity.fld1 [ActID],activity.fld101 [DOM],activity.fld102 [RemindDays],activity.fld109 [DOY],activity.fld110 [Month], " &
  " activity.fld111 [DOW], dms.udf_split('MASTER-Frequency Master-fld1',activity.fld100) [Frequency] from mmm_mst_master activity where eid=98 and documenttype='Activity Master'  and fld108='Contractor' ", con)

        da.Fill(ds)

        dt = ds.Tables(0)

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim dtActivities As New DataTable
            Dim dv As New DataView(ds.Tables(1), " fld107 like '%" & dt.Rows(i).Item("StateID") & "%' and ActID = '" & dt.Rows(i).Item("ActID").ToString() & "'  ", "", DataViewRowState.CurrentRows)
            dtActivities = dv.ToTable()
            ' Dim dtActivities As New DataTable
            '          da = New SqlDataAdapter("select activity.tid,activity.fld112[Activity],activity.fld113[Year], activity.fld108 [Activity_type],dms.udf_split('MASTER-Act Master-fld1',activity.fld1)[Act],activity.fld1 [ActID],activity.fld101 [DOM],activity.fld102 [RemindDays],activity.fld109 [DOY],activity.fld110 [Month], " &
            '" activity.fld111 [DOW], dms.udf_split('MASTER-Frequency Master-fld1',activity.fld100) [Frequency] from mmm_mst_master activity where eid=98 and documenttype='Activity Master' and  fld107 like '%" & dt.Rows(i).Item("StateID") & "%' and fld108='Contractor' and fld1='" & dt.Rows(i).Item("ActID").ToString() & "'  ", con)

            '          da.Fill(dtActivities)

            For j As Integer = 0 To dtActivities.Rows.Count - 1
                con = New SqlConnection(conStr)

                Try
                    If (CheckScheduler(dtActivities.Rows(j).Item("Frequency").ToString(), dtActivities.Rows(j).Item("DOM").ToString(), dtActivities.Rows(j).Item("DOY").ToString(), dtActivities.Rows(j).Item("Month").ToString(), dtActivities.Rows(j).Item("DOW").ToString(), dtActivities.Rows(j).Item("RemindDays").ToString(), dtActivities.Rows(j).Item("Year").ToString())) Then

                        Dim objdocutil As New DocUtils
                        Dim dt1 As Date = ReturnDate(dtActivities.Rows(j).Item("Frequency").ToString(), dtActivities.Rows(j).Item("DOM").ToString(), dtActivities.Rows(j).Item("DOY").ToString(), dtActivities.Rows(j).Item("Month").ToString(), dtActivities.Rows(j).Item("DOW").ToString(), dtActivities.Rows(j).Item("RemindDays").ToString())

                        'Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                        'Dim con1 As New SqlConnection(conStr1)
                        'Dim cmd1 As New SqlCommand()

                        'cmd1.Connection = con1
                        'cmd1.CommandText = "select count(*) from mmm_mst_doc where fld5='" & dt.Rows(i).Item("CompanyID").ToString() & "' and fld7 ='" & dt.Rows(i).Item("SiteID").ToString() & "' and fld2 ='" & dt.Rows(i).Item("ActID1").ToString() & "' and fld19='" & dtActivities.Rows(j).Item("tid").ToString() & "' and convert(date,convert(datetime,fld43,3)) = convert(date,'" & dt1 & "')"
                        'con1.Open()
                        'Dim rows As Integer = cmd1.ExecuteScalar()
                        'con1.Close()
                        'duplicatecounter += 1
                        'If (rows = 0) Then
                        '    duplicatecounter += 1

                        da = New SqlDataAdapter("INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source,fld1,fld25,fld5,fld7,fld2,fld19,fld29,fld30,fld31,fld46,fld44,fld23,fld6,fld43) " &
                             " VALUES (98,'Act Document','" & dt.Rows(i).Item("Maker").ToString() & "',getdate(),'Scheduler','" & dt.Rows(i).Item("SiteContMasID").ToString() & "','" & dtActivities.Rows(j).Item("Activity_Type").ToString() & "', " &
                             " '" & dt.Rows(i).Item("CompanyID").ToString() & "','" & dt.Rows(i).Item("SiteID").ToString() & "','" & dt.Rows(i).Item("ActID1").ToString() & "','" & dtActivities.Rows(j).Item("tid").ToString() & "','" & dt.Rows(i).Item("Company Name").ToString() & "'," &
                             " '" & dt.Rows(i).Item("Site Name").ToString() & "','" & dt.Rows(i).Item("Act").ToString() & "','" & dtActivities.Rows(j).Item("Activity").ToString() & "','" & dt.Rows(i).Item("Contractor").ToString() & "','" & dt.Rows(i).Item("ContractorID").ToString() & "','" & dt.Rows(i).Item("CompanyCode").ToString() & "','" & dt1.Day.ToString() & "/" & dt1.Month.ToString() & "/" & dt1.Year.ToString().Substring(2) & "'); select @@Identity", con)

                        Dim Docid As String = ""
                        con.Open()
                        tran = con.BeginTransaction()

                        'Transactional Query

                        da.SelectCommand.Transaction = tran
                        Docid = da.SelectCommand.ExecuteScalar().ToString()
                        objdocutil.ExecuteCommanModule(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")), con, tran)
                        objdocutil.ExecuteDocumentModuleModule(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")), con, tran)
                        objdocutil.ExecuteUserModule(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")), con, tran)
                        tran.Commit()
                        Try
                            Dim strparams As String = Docid & "||" & Convert.ToString(dt.Rows(i).Item("Maker"))
                            ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, strparams)

                            '  objdocutil.ExecuteOtherUtils(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")))

                        Catch ex As Exception
                            objerrormail.SendMail("pallavi.bhardwaj@myndsol.com", "Error in ecomp scheduler mail shoot!", " Error occured while ExecuteOtherUtils:' " & ex.Message & " '", "ajeet.kumar@myndsol.com")
                            'Please note.Not throwing any exception from this block because it wont impact any BPM Activity
                        End Try

                        SuccessCount += 1
                        'End If

                        ' 'For Contractor Types  Creation of Act Doc 
                        'da = New SqlDataAdapter("INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source,fld1,fld25,fld5,fld7,fld2,fld19,fld29,fld30,fld31,fld46,fld44,fld23,fld6,fld43) " &
                        '     " VALUES (98,'Act Document','" & dt.Rows(i).Item("Maker").ToString() & "',getdate(),'Scheduler','" & dt.Rows(i).Item("SiteContMasID").ToString() & "','" & dtActivities.Rows(j).Item("Activity_Type").ToString() & "', " &
                        '     " '" & dt.Rows(i).Item("CompanyID").ToString() & "','" & dt.Rows(i).Item("SiteID").ToString() & "','" & dt.Rows(i).Item("ActID1").ToString() & "','" & dtActivities.Rows(j).Item("tid").ToString() & "','" & dt.Rows(i).Item("Company Name").ToString() & "'," &
                        '     " '" & dt.Rows(i).Item("Site Name").ToString() & "','" & dt.Rows(i).Item("Act").ToString() & "','" & dtActivities.Rows(j).Item("Activity").ToString() & "','" & dt.Rows(i).Item("Contractor").ToString() & "','" & dt.Rows(i).Item("ContractorID").ToString() & "','" & dt.Rows(i).Item("CompanyCode").ToString() & "','" & dt1.Day.ToString() & "/" & dt1.Month.ToString() & "/" & dt1.Year.ToString().Substring(2) & "'); select @@Identity", con)

                        'Dim Docid As String = ""
                        'con.Open()
                        'tran = con.BeginTransaction()

                        ''Transactional Query

                        'da.SelectCommand.Transaction = tran
                        'Docid = da.SelectCommand.ExecuteScalar().ToString()
                        'objdocutil.ExecuteCommanModule(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")), con, tran)
                        'objdocutil.ExecuteDocumentModuleModule(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")), con, tran)
                        'objdocutil.ExecuteUserModule(98, "Act Document", "Document", Docid, Convert.ToString(dt.Rows(i).Item("Maker")), con, tran)
                        'tran.Commit()
                        'SuccessCount += 1

                    End If

                Catch ex As Exception
                    objerrormail.SendMail("pallavi.bhardwaj@myndsol.com", "Error in ecomp scheduler Actvites!", " Error occured while loop on activities:' " & ex.Message & " '", "ajeet.kumar@myndsol.com")
                    FailedCount += 1
                    Try
                        tran.Rollback()
                    Catch ex1 As Exception

                    End Try

                Finally

                    If (con.State = ConnectionState.Open) Then
                        con.Close()
                    End If

                End Try
            Next

        Next

        cmd.Connection = con
        cmd.CommandText = "insert into mmm_schedulerlog (eid,schedulerName,SuccessCount,FailedCount,LatRundatetime,Remarks) values (98,'EcomplianceScheduler'," & SuccessCount & "," & FailedCount & ",getdate(),'Exit from Contractor Activity')"
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
    End Sub

    Sub ThreadProc(str As Object)
        Dim objdocutil As New DocUtils

        Dim values As String() = str.ToString().Split("||")
        Dim docid As String = values(0)
        Dim UID As String = values(2)
        objdocutil.ExecuteOtherUtils(98, "Act Document", "Document", docid, Convert.ToInt16(UID))
        ' No state object was passed to QueueUserWorkItem, so stateInfo is null.
        ' Dim obj As New testinfo()
        'obj = stateInfo

    End Sub

    Function CheckScheduler(Frequency As String, DOM As String, DOY As String, Month As String, DOW As String, Reminddays As String, Year1 As String) As Boolean

        Select Case Frequency

            Case "Daily"

                Return True

            Case "As Needed"

                Return False

            Case "Monthly"
                Dim date1 As New Date(DateTime.Now.Year, DateTime.Now.Month, DOM)
                date1 = date1.AddDays(-Reminddays)
                If (date1 = DateTime.Now.Date) Then
                    Return True
                Else
                    Return False
                End If

            Case "Weekly"

                Dim date1 As New Date
                Dim datetoday As Integer = DateTime.Now.Day
                If ((weekday(DateTime.Now.DayOfWeek.ToString().Substring(0, 3).ToUpper()) + Reminddays) = weekday(DOW)) Then
                    Return True
                Else : Return False
                End If

            Case "Yearly"

                Dim date1 As New Date(DateTime.Now.Year, GetMonth(Month), DOM)
                date1 = date1.AddDays(-Reminddays)
                If (date1.Day.ToString() = DateTime.Now.Day.ToString() And date1.Month = DateTime.Now.Month.ToString()) Then
                    Return True
                Else
                    Return False
                End If

            Case "Quarterly"

                Dim year As String
                If (GetMonth(Month) = "4" And DateTime.Now.Month < 4) Then
                    year = DateTime.Now.Year - 1
                Else
                    year = DateTime.Now.Year - 1
                End If

                Dim date1 As New Date(year, GetMonth(Month), DOM)
                date1 = date1.AddDays(-Reminddays)

                If (date1 = DateTime.Now.Date) Then
                    Return True
                ElseIf (date1.AddMonths(3) = DateTime.Now.Date) Then
                    Return True
                ElseIf (date1.AddMonths(6) = DateTime.Now.Date) Then
                    Return True
                ElseIf (date1.AddMonths(9) = DateTime.Now.Date) Then
                    Return True
                Else : Return False
                End If

            Case "Bi-annual"

                If ((DateTime.Now.Year - Convert.ToInt16(Year1)) Mod 2 = 0) Then

                    Dim date1 As New Date(DateTime.Now.Year, GetMonth(Month), DOM)
                    date1 = date1.AddDays(-Reminddays)
                    If (date1 = DateTime.Now.Date) Then
                        Return True
                    Else
                        Return False
                    End If

                End If

            Case "Half Yearly"
                Dim date1 As New Date(DateTime.Now.Year, GetMonth(Month), DOM)
                date1 = date1.AddDays(-Reminddays)

                If (date1 = DateTime.Now.Date Or date1.AddMonths(6) = DateTime.Now.Date) Then
                    Return True
                Else : Return False

                End If

        End Select

        Return False

    End Function

    Function ReturnDate(Frequency As String, DOM As String, DOY As String, Month As String, DOW As String, Reminddays As String) As Date

        Select Case Frequency

            Case "Daily"

                Return DateTime.Now.Date

            Case "Monthly"

                Dim date1 As New Date(DateTime.Now.Year, DateTime.Now.Month, DOM)
                Return date1

            Case "Weekly"

                Dim date1 As New Date
                Dim datetoday As Integer = DateTime.Now.Day
                If ((weekday(DateTime.Now.DayOfWeek) + Reminddays) = weekday(DOW)) Then
                    date1 = New Date(DateTime.Now.Year, DateTime.Now.Month, datetoday + Reminddays)
                End If

            Case "Yearly"

                Dim date1 As New Date(DateTime.Now.Year, GetMonth(Month), DOM)
                Return date1

            Case "Quarterly"

                Dim year As String

                If (DOM = "4" And DateTime.Now.Month < 4) Then
                    year = DateTime.Now.Year - 1
                Else
                    year = DateTime.Now.Year - 1
                End If

                Dim date1 As New Date(year, GetMonth(Month), DOM)
                Dim date2 As New Date(year, GetMonth(Month), DOM - Reminddays)
                If (date2 = DateTime.Now.Date) Then
                    Return date1
                ElseIf (date2.AddMonths(3) = DateTime.Now.Date) Then
                    Return date1.AddMonths(3)
                ElseIf (date2.AddMonths(6) = DateTime.Now.Date) Then
                    Return date1.AddMonths(6)
                ElseIf (date2.AddMonths(9) = DateTime.Now.Date) Then
                    Return date1.AddMonths(9)
                Else : Return ""
                End If

            Case "Bi-annual"

                Dim date1 As New Date(DateTime.Now.Year, GetMonth(Month), DOY - Reminddays)
                Return date1

            Case "Half Yearly"

                Dim date1 As New Date(DateTime.Now.Year, DateTime.Now.Month, DOM)
                Return date1



        End Select

        Return ""
    End Function


    'Function CheckScheduler(Frequency As String, DOM As String, DOY As String, Month As String, DOW As String, Reminddays As String) As Boolean

    '    Select Case Frequency

    '        Case "Daily"
    '            Return True

    '        Case "As Needed"
    '            Return False

    '        Case "Monthly"
    '            If (DOM - Reminddays = DateTime.Now.Day.ToString()) Then
    '                Return True
    '            Else
    '                Return False
    '            End If

    '        Case "Weekly"

    '            Dim date1 As New Date
    '            Dim datetoday As Integer = DateTime.Now.Day
    '            If ((weekday(DateTime.Now.DayOfWeek.ToString().Substring(0, 3).ToUpper()) + Reminddays) = weekday(DOW)) Then
    '                Return True
    '            Else : Return False
    '            End If

    '        Case "Yearly"

    '            If (DOM - Reminddays = DateTime.Now.Day.ToString() And GetMonth(Month) = DateTime.Now.Month.ToString()) Then
    '                Return True
    '            Else
    '                Return False
    '            End If

    '        Case "Quarterly"

    '            Dim year As String
    '            If (DOM = "4" And DateTime.Now.Month < 4) Then
    '                year = DateTime.Now.Year - 1
    '            Else
    '                year = DateTime.Now.Year - 1
    '            End If

    '            Dim date1 As New Date(year, GetMonth(Month), DOM - Reminddays)

    '            If (date1 = DateTime.Now.Date) Then
    '                Return True
    '            ElseIf (date1.AddMonths(3) = DateTime.Now.Date) Then
    '                Return True
    '            ElseIf (date1.AddMonths(6) = DateTime.Now.Date) Then
    '                Return True
    '            ElseIf (date1.AddMonths(9) = DateTime.Now.Date) Then
    '                Return True
    '            Else : Return False
    '            End If

    '    End Select

    '    Return False

    'End Function

    'Function ReturnDate(Frequency As String, DOM As String, DOY As String, Month As String, DOW As String, Reminddays As String) As Date

    '    Select Case Frequency

    '        Case "Daily"

    '            Return DateTime.Now.Date

    '        Case "Monthly"

    '            Dim date1 As New Date(DateTime.Now.Year, DateTime.Now.Month, DOM)
    '            Return date1

    '        Case "Weekly"

    '            Dim date1 As New Date
    '            Dim datetoday As Integer = DateTime.Now.Day
    '            If ((weekday(DateTime.Now.DayOfWeek) + Reminddays) = weekday(DOW)) Then
    '                date1 = New Date(DateTime.Now.Year, DateTime.Now.Month, datetoday + Reminddays)
    '            End If

    '        Case "Yearly"

    '            Dim date1 As New Date(DateTime.Now.Year, GetMonth(Month), DOM)
    '            Return date1

    '        Case "Quarterly"

    '            Dim year As String

    '            If (DOM = "4" And DateTime.Now.Month < 4) Then
    '                year = DateTime.Now.Year - 1
    '            Else
    '                year = DateTime.Now.Year - 1
    '            End If

    '            Dim date1 As New Date(year, GetMonth(Month), DOM)
    '            Dim date2 As New Date(year, GetMonth(Month), DOM - Reminddays)
    '            If (date2 = DateTime.Now.Date) Then
    '                Return date1
    '            ElseIf (date2.AddMonths(3) = DateTime.Now.Date) Then
    '                Return date1.AddMonths(3)
    '            ElseIf (date2.AddMonths(6) = DateTime.Now.Date) Then
    '                Return date1.AddMonths(6)
    '            ElseIf (date2.AddMonths(9) = DateTime.Now.Date) Then
    '                Return date1.AddMonths(9)
    '            Else : Return ""
    '            End If

    '    End Select

    '    Return ""

    'End Function

    Function GetMonth(MonthNm As String) As Integer

        Select Case MonthNm
            Case "JAN"
                Return 1
            Case "FEB"
                Return 2
            Case "MAR"
                Return 3
            Case "APR"
                Return 4
            Case "MAY"
                Return 5
            Case "JUN"
                Return 6
            Case "JUL"
                Return 7
            Case "AUG"
                Return 8
            Case "SEP"
                Return 9
            Case "OCT"
                Return 10
            Case "NOV"
                Return 11
            Case "DEC"
                Return 12
            Case Else
                Return 1
        End Select

    End Function

    Function weekday(day As String) As Integer
        Select Case day
            Case "MON"
                Return 1
            Case "TUE"
                Return 2
            Case "WED"
                Return 3
            Case "THU"
                Return 4
            Case "FRI"
                Return 5
            Case "SAT"
                Return 6
            Case "SUN"
                Return 7
        End Select
        Return 0

    End Function



    Function getdaylist() As List(Of Integer)

        Dim days As New List(Of Integer)
        For i As Integer = 1 To DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)
            days.Add(i)
        Next
        Return days

    End Function



End Class
