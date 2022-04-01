Imports System.Data
Imports System.Data.SqlClient

'Imports Microsoft.Office.Interop.Excel
'Imports Microsoft.Office.Interop


Public Class Rentaltool
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Protected Sub AutoRunLog(ByVal FunctionName As String, ByVal Activity As String, Optional MsgErrror As String = "", Optional eid As Integer = 0)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim cmd As New SqlCommand("", con)
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('" & FunctionName & "','" & Activity & "','" & MsgErrror & "',getdate()," & eid & ")"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd.ExecuteNonQuery()
            con.Dispose()
        Catch ex As Exception
            con.Dispose()
        End Try

    End Sub
    Public Sub AutoInvoice()
        AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "Calling AutoInvoice Function Now 1", 181)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim insStr As String = String.Empty
        Dim UpdStr As String = String.Empty
        Dim res As Integer = 0
        Dim ob1 As New DMSUtil()
        Dim ob As New DynamicForm
        Dim tran As SqlTransaction = Nothing
        Dim da As New SqlDataAdapter
        Try
            Dim ds As New DataSet
            da = New SqlDataAdapter("select convert(varchar(100),LastScheduledDate,103) as LastScheduledDate, * from  MMM_MST_ReportScheduler with(nolock) where isActive='1' and eid=181 order by hh,mm,ordering", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "rpt")
            If ds.Tables("rpt").Rows.Count > 0 Then

                For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1

                    If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then


                        If CDate(ds.Tables("rpt").Rows(d).Item("LastScheduledDate")) <> Date.Now Then
                            da = New SqlDataAdapter("select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld,RentEsc,CAMEsc,SDmonths,RentEscptage	,CamEscptage from   MMM_MST_AutoInvoiceSetting with(nolock) where  IsActiveStatus=1", con)

                            da.Fill(ds, "AutoInvSettingData")
                            Dim str As String = ""

                            Dim dt As New DataTable
                            Dim StartDateFld As String = String.Empty
                            Dim EndDateFld As String = String.Empty
                            Dim FrequencyFld As String = String.Empty
                            Dim PeriodFld As Int16 = 0
                            Dim RentalFld As String = String.Empty
                            Dim CAMFld As String = String.Empty
                            Dim SDFld As String = String.Empty
                            Dim RegistrationDateFld As String = String.Empty
                            Dim SchedulerTidID As String = String.Empty
                            Dim RentEsc As String = String.Empty
                            Dim CAMEsc As String = String.Empty
                            Dim SDmonths As String = String.Empty
                            Dim RentEscPtage As String = String.Empty
                            Dim CAMEscPtage As String = String.Empty
                            Dim Fldstr As String = ""
                            Dim strTFlds As String = String.Empty
                            Dim strSFlds As String = String.Empty
                            Dim strHTFlds As String = String.Empty
                            Dim strHSFlds As String = String.Empty
                            Dim strHSFldsArr As String()
                            Dim RentFreePeriodFlds As String = String.Empty
                            Dim EID As String = String.Empty
                            Dim LeaseType As String = String.Empty
                            Dim MGAmount As String = String.Empty

                            If ds.Tables("AutoInvSettingData").Rows.Count <> 0 Then
                                For i As Integer = 0 To ds.Tables("AutoInvSettingData").Rows.Count - 1


                                    StartDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("StartDateFld"))
                                    EndDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EndDateFld"))
                                    FrequencyFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("FrequencyFld"))
                                    RentalFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                                    CAMFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMFld"))
                                    SDFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDFld"))
                                    RegistrationDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RegistrationDateFld"))
                                    RentFreePeriodFlds = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentFreePeriodFld"))
                                    EID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EID"))
                                    SchedulerTidID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid"))
                                    LeaseType = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("LeaseType"))
                                    MGAmount = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                                    RentEsc = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentEsc"))
                                    CAMEsc = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMEsc"))
                                    RentEscPtage = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentEscPtage"))
                                    CAMEscPtage = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMEscPtage"))
                                    SDmonths = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDMonths"))

                                    Dim FldsVal As String = String.Empty
                                    Dim SchedulerCheck As Boolean
                                    '  SchedulerCheck = Scheduler(SchedulerTidID)
                                    SchedulerCheck = True

                                    Dim FldsValArr As String()
                                    Dim value As String = ""

                                    da.SelectCommand.CommandText = "select  F.Tid,F.TFld,F.SFld,F.SDocType,F.AutoInvTid,F.EID,F.TFldName,F.sFldName,F.Leasetype,F.TDocType  from    MMM_MST_AutoInvoiceSetting C inner join MMM_MST_AutoInvFieldSetting F on c.Tid=F.AutoInvTid where C.eid=" & EID & " and C.IsActiveStatus=1 and F.AutoInvTid=" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid")) & ""
                                    'da.SelectCommand.Transaction = tran
                                    da.Fill(ds, "AutoInvFieldData")
                                    Dim valueL As String

                                    Dim AutoInvDocData As New DataTable

                                    Dim fieldmapping As String = ""
                                    Dim fieldmappingINVdt As String = ""
                                    Dim fieldmappingLeaseDocdt As String = ""
                                    If AutoInvDocData.Rows.Count <> 0 Then
                                        ds.Tables("AutoInvDocData").Clear()
                                    End If


                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and Displayname='Rent Invoice Generation Date' and datatype='Datetime'"
                                    Dim dtDtF As New DataTable
                                    da.Fill(dtDtF)
                                    If dtDtF.Rows.Count > 0 Then
                                        fieldmappingINVdt = dtDtF.Rows(0).Item("fieldmapping")
                                    End If
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and Displayname='Lease Doc No' and datatype='Text'"
                                    Dim LDocdtDtF As New DataTable
                                    da.Fill(LDocdtDtF)
                                    If LDocdtDtF.Rows.Count > 0 Then
                                        fieldmappingLeaseDocdt = LDocdtDtF.Rows(0).Item("fieldmapping")
                                    End If


                                    'Rental/Sd/CAM Inv Gen data
                                    da.SelectCommand.CommandText = "with cte as(select  ROW_NUMBER() OVER(PARTITION BY fld2,fld11 ORDER BY tid DESC) rn, fld2,  " & LeaseType & "  as leasetype, " & fieldmappingLeaseDocdt & " as [LeaseDocNo], " & fieldmappingINVdt & " as LRentInvGenDate,  tid as tid,convert(varchar, convert(datetime," & RegistrationDateFld & ", 3), 103)  as RegistrationDate ," & RentFreePeriodFlds & " as RentFreePeriod," & FrequencyFld & " as [RentPaymentCycle],fld49 as [RentFreeFitOutStartDate],fld50 as [RentFreeFitOutEndDate]," & RentFreePeriodFlds & " as [RentFreeDays]," & StartDateFld & " as LStartDate , " & EndDateFld & " as LEndDate," & MGAmount & " as LMGAmount," & RentEsc & " As RentEsc," & CAMEsc & " as CAMEsc," & SDmonths & " as SDMonths," & RentEscPtage & " as RentEscPtage," & CAMEscPtage & " as CAMEscPtage,fld57 as [LessorsPropertyShare],fld41 as [RentType],fld76 as [CamPaymentcycle]," & SDFld & " as SDAmt,fld48 as CAMCommDate," & CAMFld & " as CAMAmt,fld78 as AmendmentFlag,fld75 as LTokenAmnt from MMM_MST_MASTER  with(nolock)   where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and  convert(date, getdate(), 3) between   convert(date," & StartDateFld & ", 3) and  convert(date, " & EndDateFld & ", 3)    and reftid<>'' and isauth=1 )	select * from cte where rn=1"
                                    da.Fill(ds, "AutoInvDocData")
                                    AutoInvDocData = ds.Tables("AutoInvDocData")

                                    Dim SourceDocData As New DataTable
                                    Dim RDOCIDData As New DataTable
                                    Dim FieldData As New DataTable

                                    Dim LDocNo As String = String.Empty
                                    Dim LStartDate As String = String.Empty
                                    Dim LENDDate As String = String.Empty
                                    Dim LFitOutStartDate As String = String.Empty
                                    Dim LFitOutEndDate As String = String.Empty
                                    Dim LeaseComDate As String = String.Empty
                                    Dim CAMComDate As String = String.Empty
                                    Dim LRentPayment As String = String.Empty
                                    Dim LRentFreedays As String = String.Empty
                                    Dim LRentInvGenDate As String = String.Empty
                                    Dim LRentESC As String = String.Empty
                                    Dim LCAMEsc As String = String.Empty
                                    Dim LRentESCPtage As String = String.Empty
                                    Dim LCAMEscPtage As String = String.Empty
                                    Dim LSDMonths As String = String.Empty
                                    Dim LPropershare As String = String.Empty
                                    Dim LRenttype As String = String.Empty
                                    Dim LCAMRentCycletype As String = String.Empty
                                    Dim LAmendmentFlag As String = String.Empty
                                    Dim LMGAmt As Double = 0
                                    Dim LSDAmt As Double = 0
                                    Dim LCAMAmt As Double = 0
                                    Dim LTokenAmt As Double = 0


                                    If ds.Tables("AutoInvDocData").Rows.Count <> 0 Then

                                        For j As Integer = 0 To ds.Tables("AutoInvDocData").Rows.Count - 1

                                            LDocNo = ds.Tables("AutoInvDocData").Rows(j).Item("LeaseDocNo")
                                            LStartDate = ds.Tables("AutoInvDocData").Rows(j).Item("LStartDate")
                                            LENDDate = ds.Tables("AutoInvDocData").Rows(j).Item("LEndDate")
                                            LFitOutStartDate = ds.Tables("AutoInvDocData").Rows(j).Item("RentFreeFitOutStartDate")
                                            LFitOutEndDate = ds.Tables("AutoInvDocData").Rows(j).Item("RentFreeFitOutEndDate")
                                            LRentPayment = ds.Tables("AutoInvDocData").Rows(j).Item("RentPaymentCycle")
                                            LRentInvGenDate = ds.Tables("AutoInvDocData").Rows(j).Item("LRentInvGenDate")
                                            LMGAmt = ds.Tables("AutoInvDocData").Rows(j).Item("LMGAmount")
                                            LSDAmt = ds.Tables("AutoInvDocData").Rows(j).Item("SDAmt")
                                            LRentESC = ds.Tables("AutoInvDocData").Rows(j).Item("RentESC")
                                            LCAMEsc = ds.Tables("AutoInvDocData").Rows(j).Item("CAMESC")
                                            LRentESCPtage = ds.Tables("AutoInvDocData").Rows(j).Item("RentESCPtage")
                                            LCAMEscPtage = ds.Tables("AutoInvDocData").Rows(j).Item("CAMESCPtage")
                                            LSDMonths = ds.Tables("AutoInvDocData").Rows(j).Item("SdMonths")
                                            LPropershare = ds.Tables("AutoInvDocData").Rows(j).Item("LessorsPropertyShare")
                                            LRenttype = ds.Tables("AutoInvDocData").Rows(j).Item("Renttype")
                                            LCAMRentCycletype = ds.Tables("AutoInvDocData").Rows(j).Item("CamPaymentcycle")
                                            CAMComDate = ds.Tables("AutoInvDocData").Rows(j).Item("CAMCommDate")
                                            LCAMAmt = ds.Tables("AutoInvDocData").Rows(j).Item("CAMAmt")
                                            LAmendmentFlag = ds.Tables("AutoInvDocData").Rows(j).Item("AmendmentFlag")
                                            LTokenAmt = ds.Tables("AutoInvDocData").Rows(j).Item("LTokenAmt")


                                            Dim Larr = LFitOutStartDate.Split("/")
                                            Dim Ldate1 As New Date(Larr(2), Larr(1), Larr(0))
                                            Ldate1 = CDate(Ldate1)
                                            Dim Larr1 = LFitOutEndDate.Split("/")
                                            Dim Ldate2 As New Date(Larr1(2), Larr1(1), Larr1(0))
                                            Ldate2 = CDate(Ldate2)
                                            Dim Ldt1 As DateTime = Convert.ToDateTime(Ldate1.ToString("MM/dd/yy"))

                                            Dim Ldt2 As DateTime = Convert.ToDateTime(Ldate2.ToString("MM/dd/yy"))

                                            ''count total day between selected your date

                                            Dim ts As TimeSpan = Ldt2.Subtract(Ldt1)
                                            If Convert.ToInt32(ts.Days) >= 0 Then
                                                LRentFreedays = Convert.ToInt32(ts.Days)
                                            Else
                                                LRentFreedays = "0"
                                            End If


                                            'Lease Start Date Format
                                            Dim LStartDate1 As New DateTime()
                                            Dim LSarr = LStartDate.Split("/")
                                            Dim LSdate1 As New Date(LSarr(2), LSarr(1), LSarr(0))
                                            LSdate1 = CDate(LSdate1)
                                            Dim LSdt1 As DateTime = Convert.ToDateTime(LSdate1.ToString("MM/dd/yy"))

                                            'Lease End date Format
                                            Dim LEndDate1 As New DateTime()
                                            Dim LEarr = LENDDate.Split("/")
                                            Dim LEdate1 As New Date(LEarr(2), LEarr(1), LEarr(0))
                                            LEdate1 = CDate(LEdate1)
                                            Dim LEdt1 As DateTime = Convert.ToDateTime(LEdate1.ToString("MM/dd/yy"))

                                            Dim AddDayInRentFreedays As Int32 = 0
                                            AddDayInRentFreedays = Convert.ToInt64(LRentFreedays) + 1
                                            LeaseComDate = LSdt1.AddDays(+AddDayInRentFreedays)
                                            Dim LComDate As String = String.Empty
                                            Dim LCAMComDate As String = String.Empty
                                            'LComDate = LeaseComDate.ToString()
                                            Dim StrARR As String() = LeaseComDate.Split("/")
                                            LComDate = StrARR(1) + "/" + StrARR(0) + "/" + StrARR(2)

                                            LCAMComDate = StrARR(1) + "/" + StrARR(0) + "/" + StrARR(2)

                                            'Calculating escalation Date
                                            Dim LRIGDTarr = LComDate.Split("/")
                                            Dim LRIGDTdate1 As New Date(LRIGDTarr(2), LRIGDTarr(1), LRIGDTarr(0))
                                            LRIGDTdate1 = CDate(LRIGDTdate1)
                                            Dim LRIGDTdt As Date
                                            Dim LRIGDTdt1 As Date
                                            Dim LRInvGDTdt1 As DateTime = Convert.ToDateTime(LRIGDTdate1.ToString("MM/dd/yy"))
                                            Dim LRInvGDTdt2 As Date = Date.Today
                                            LRInvGDTdt2 = Convert.ToDateTime(LRInvGDTdt2.ToString("MM/dd/yy"))

                                            Dim yearInTheFuture As Date
                                            Dim TFyearInTheFuture As Boolean = False
                                            Dim RentyearInTheFuture As String = ""

                                            If Date.TryParse(LRInvGDTdt1, LRIGDTdt) And Date.TryParse(LRInvGDTdt1, LRIGDTdt1) Then


                                                Dim startDtStr As String = String.Empty
                                                Dim startDtyear As Int16 = 0

                                                startDtStr = Convert.ToString(LRIGDTdt1.Month) + "/" + Convert.ToString(LRIGDTdt1.Day)
                                                startDtyear = LRIGDTdt1.Year
                                                Dim RentEscdtStr As String = String.Empty
                                                Dim RentEscyrdtStr As Int16 = 0

                                                If LRentESC = 1491593 Then 'Annual 
                                                    'calculating the RentEsclation 

                                                    RentEscdtStr = Convert.ToString(LRIGDTdt.Month) + "/" + Convert.ToString(LRIGDTdt.Day)
                                                    yearInTheFuture = LRIGDTdt.AddYears(1)
                                                    RentEscyrdtStr = CDate(yearInTheFuture).Year
                                                ElseIf LRentESC = 1491594 Then 'Bi-Annual

                                                    RentEscdtStr = Convert.ToString(LRIGDTdt.Month) + "/" + Convert.ToString(LRIGDTdt.Day)
                                                    yearInTheFuture = LRIGDTdt.AddYears(2)
                                                    RentEscyrdtStr = CDate(yearInTheFuture).Year
                                                ElseIf LRentESC = 1491595 Then 'Tri-Annual

                                                    RentEscdtStr = Convert.ToString(LRIGDTdt.Month) + "/" + Convert.ToString(LRIGDTdt.Day)
                                                    yearInTheFuture = LRIGDTdt.AddYears(3)
                                                    RentEscyrdtStr = CDate(yearInTheFuture).Year
                                                End If

                                                If RentEscdtStr = startDtStr And startDtyear <> RentEscyrdtStr Then

                                                    RentyearInTheFuture = yearInTheFuture
                                                    Dim Arr As String() = RentyearInTheFuture.Split("/")
                                                    RentyearInTheFuture = ""
                                                    RentyearInTheFuture = Arr(1) + "/" + Arr(0) + "/" + Arr(2)
                                                    TFyearInTheFuture = True

                                                End If
                                            End If

                                            AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "Calling AutoInvoice Function Now 1", 181)
                                            Try


                                                da.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                da.SelectCommand.Parameters.Clear()
                                                da.SelectCommand.Parameters.AddWithValue("@MAILTO", "Chandni.devi@myndsol.com")
                                                da.SelectCommand.Parameters.AddWithValue("@CC", "")
                                                da.SelectCommand.Parameters.AddWithValue("@MSG", "In Function")
                                                da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                                da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                                                da.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                da.SelectCommand.ExecuteNonQuery()


                                                Dim names As New ArrayList
                                                Dim TestLeaseType = ds.Tables("AutoInvDocData").Rows(j).Item("leasetype")

                                                If TestLeaseType = "1491591" Then 'Rental
                                                    names.Add("Rental Invoice")
                                                ElseIf TestLeaseType = "1491592" Then 'Security Deposit 1491592
                                                    names.Add("Security Deposit")
                                                ElseIf TestLeaseType = "1554309" Then 'Rental And SD
                                                    names.Add("Rental Invoice")
                                                    names.Add("Security Deposit")
                                                ElseIf TestLeaseType = "1554310" Then 'CAM 
                                                    names.Add("CAM")
                                                ElseIf TestLeaseType = "1570943" Then 'ALL
                                                    names.Add("Rental Invoice")
                                                    names.Add("Security Deposit")
                                                    names.Add("CAM")
                                                End If


                                                'Non transactional Query

                                                For f As Integer = 0 To names.Count - 1
                                                    If names(f).ToUpper() = ("Rental Invoice").ToUpper() Then
                                                        If names(f).ToUpper() = ("Rental Invoice").ToUpper() Then

                                                            Dim dvs As DataView = ds.Tables("AutoInvFieldData").DefaultView
                                                            dvs.RowFilter = "TDocType='" & names(f) & "'"

                                                            Dim filteredTable As New DataTable()
                                                            filteredTable = dvs.ToTable()
                                                            If filteredTable.Rows.Count <> 0 Then
                                                                strTFlds = ""
                                                                strSFlds = ""
                                                                strTFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                                                strSFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                                                strHSFldsArr = Nothing
                                                                strHSFldsArr = strSFlds.Split(",")
                                                                valueL = String.Join(",',',", strHSFldsArr)
                                                                strHSFlds = valueL
                                                            End If
                                                            If SourceDocData.Rows.Count > 0 Then
                                                                SourceDocData.Clear()
                                                            End If
                                                            If FieldData.Rows.Count > 0 Then
                                                                FieldData.Clear()
                                                            End If
                                                            da.SelectCommand.CommandText = " select fld2 as [LeaseDocNo],fld11 as [Name of Lessor],fld7 as [Rental Amount],tid as RDOCID,concat(" & strHSFlds & ") as TFldVAl from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & "   where documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "'  and eid=" & EID & " and tid=" & ds.Tables("AutoInvDocData").Rows(j).Item("Tid") & " "
                                                            da.SelectCommand.CommandType = CommandType.Text

                                                            da.Fill(ds, "SourceDocData")

                                                            SourceDocData = ds.Tables("SourceDocData")
                                                            Dim RDOCID As String = ""
                                                            Dim LeaseDocNo As String = ""
                                                            If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                                                FldsVal = ""
                                                                FldsVal = ds.Tables("SourceDocData").Rows(0).Item("TFldVAl")
                                                                FldsValArr = FldsVal.Split(",")
                                                                value = ""
                                                                value = String.Join("','", FldsValArr)
                                                                value = "'" + value + "'"
                                                                RDOCID = Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("RDOCID"))
                                                                LeaseDocNo = Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("LeaseDocNo"))
                                                            End If
                                                            Try
                                                                If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                                                    If RDOCIDData.Rows.Count > 0 Then
                                                                        RDOCIDData.Clear()
                                                                    End If
                                                                    'Name of Lessor
                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then
                                                                        da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate();  select * from MMM_MST_MASTER where RefTid=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "'  --and adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE) "
                                                                    Else
                                                                        da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate(); select * from MMM_MST_DOC where fld11='" & Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("Name of Lessor")) & "'  and  fld2='" & LDocNo & "' and documenttype='" & Convert.ToString(names(f)) & "' --and  adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE);"
                                                                    End If
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.Fill(ds, "RDOCIDData")
                                                                    RDOCIDData = ds.Tables("RDOCIDData")
                                                                    If FieldData.Rows.Count > 0 Then
                                                                        FieldData.Clear()
                                                                    End If
                                                                    'Collecting the field data
                                                                    da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.Fill(ds, "fields")
                                                                    FieldData = ds.Tables("fields")

                                                                    If ds.Tables("RDOCIDData").Rows.Count = 0 Then
                                                                        'check Inv Gen date  Date.Now
                                                                        If CDate(LeaseComDate) = Date.Now Then
                                                                            If LRenttype = 1554651 Or LRenttype = 1554653 Then 'Fixed or 'Fixed and Revenue Sharing 
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If
                                                                                tran = con.BeginTransaction()
                                                                                insStr = ""
                                                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & LeaseComDate & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                End If
                                                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Transaction = tran
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If
                                                                                res = da.SelectCommand.ExecuteScalar()


                                                                                ' End If

                                                                                If res <> 0 Then
                                                                                    'Calculeting the Actual lease start date
                                                                                    Dim date2 As DateTime = Convert.ToDateTime(LSdate1.ToString("MM/dd/yy"))
                                                                                    Dim LESCDate As Date = LeaseComDate

                                                                                    Dim FinalInvDate As String = String.Empty
                                                                                    Dim MGAmtDT As Double
                                                                                    Dim ParseDate As String = String.Empty
                                                                                    If LRentFreedays = "0" Then
                                                                                        ParseDate = date2
                                                                                    Else
                                                                                        ParseDate = LeaseComDate
                                                                                    End If
                                                                                    Dim LMGAmount As Double = 0
                                                                                    LMGAmount = LMGAmt

                                                                                    Dim ResultStr = ParseDateFn(LRentPayment, ParseDate, LMGAmount)
                                                                                    Dim PDFnResultStr = ResultStr.Split("=")
                                                                                    FinalInvDate = PDFnResultStr(0)
                                                                                    MGAmtDT = PDFnResultStr(1)

                                                                                    Dim Finaldate As String = String.Empty
                                                                                    Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                    Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                    Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                                    Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                    Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)

                                                                                    Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric'")
                                                                                    If rowMGAmt.Length > 0 Then
                                                                                        For Each CField As DataRow In rowMGAmt

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                        Next
                                                                                    End If

                                                                                    Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                                                                    If rowRCD.Length > 0 Then
                                                                                        For Each CField As DataRow In rowRCD

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()

                                                                                        Next
                                                                                    End If

                                                                                    Dim LESCDatestr As String = LESCDate.ToString("dd/MM/yy")
                                                                                    Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In rowECD

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LESCDatestr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()


                                                                                        Next
                                                                                    End If

                                                                                    'Lease Period Start	

                                                                                    Dim LSPARR As String() = ParseDate.Split("/")
                                                                                    ParseDate = LSPARR(0) + "/" + LSPARR(1) + "/" + LSPARR(2)
                                                                                    ParseDate = CDate(ParseDate).ToString("dd/MM/yy")

                                                                                    Dim SprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period Start' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In SprowECD

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & ParseDate & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()


                                                                                        Next
                                                                                    End If

                                                                                    'Lease Period End	
                                                                                    Dim LPEdate As Date = New Date(CDate(FinalInvDate).Year, CDate(FinalInvDate).Month, 1).AddDays(-1)
                                                                                    Dim LPEdatestr As String = LPEdate.ToString("dd/MM/yy")
                                                                                    Dim EprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period End' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In EprowECD

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LPEdatestr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()


                                                                                        Next
                                                                                    End If
                                                                                    ' Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)
                                                                                    Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                    ''INSERT INTO HISTORY 
                                                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                    Try
                                                                                        UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                        da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                        Dim srerd As String = String.Empty
                                                                                    Catch ex As Exception

                                                                                    End Try
                                                                                    tran.Commit()

                                                                                    'Check Work Flow
                                                                                    ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Parameters.Clear()
                                                                                    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                    Dim dt1 As New DataTable
                                                                                    da.Fill(dt1)
                                                                                    If dt1.Rows.Count = 1 Then
                                                                                        If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                            ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                        End If
                                                                                    End If

                                                                                End If


                                                                            End If

                                                                        End If

                                                                    ElseIf ds.Tables("RDOCIDData").Rows.Count > 0 And Convert.ToString(names(f)) = "Rental Invoice" Then

                                                                        If con.State = ConnectionState.Closed Then
                                                                            con.Open()
                                                                        End If
                                                                        Dim Finaldate As String = String.Empty
                                                                        tran = con.BeginTransaction()
                                                                        Dim FinalInvDate As String = String.Empty
                                                                        Dim AlreadyEistData As New DataTable
                                                                        Dim MGAmtDT As Double = 0
                                                                        Dim Fieldmap As String = String.Empty
                                                                        Dim EscFieldmap As String = String.Empty
                                                                        If Convert.ToString(names(f)) = "Rental Invoice" Then
                                                                            Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                                                            Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                                            If rowRCD.Length > 0 Then
                                                                                For Each CField As DataRow In rowRCD
                                                                                    Fieldmap = CField.Item("FieldMapping").ToString()
                                                                                Next
                                                                            End If
                                                                            If rowECD.Length > 0 Then
                                                                                For Each CField As DataRow In rowECD
                                                                                    EscFieldmap = CField.Item("FieldMapping").ToString()
                                                                                Next
                                                                            End If
                                                                            da.SelectCommand.CommandText = "Select top 1 " & Fieldmap & "  as InvCreationDt, " & EscFieldmap & " as EscDate from mmm_mst_doc with (nolock) where fld2='" & LDocNo & "' and eid=" & EID & "  and Documenttype='" & Convert.ToString(names(f)) & "' and RDOCID=" & ds.Tables("AutoInvDocData").Rows(j).Item("tid") & " order by tid desc "


                                                                        End If
                                                                        da.SelectCommand.Transaction = tran
                                                                        If AlreadyEistData.Rows.Count > 0 Then
                                                                            AlreadyEistData.Clear()
                                                                        End If
                                                                        da.Fill(AlreadyEistData)
                                                                        'Calculating Escalation date  from previous Escalation date
                                                                        Dim ESCDtarr = AlreadyEistData.Rows(0).Item("EscDate").ToString().Split("/")
                                                                        Dim ESCDTdate1 As New Date(ESCDtarr(2), ESCDtarr(1), ESCDtarr(0))
                                                                        ESCDTdate1 = CDate(ESCDTdate1)
                                                                        Dim ESCGenDt As DateTime = Convert.ToDateTime(ESCDTdate1.ToString("MM/dd/yy"))
                                                                        Dim EscalationDate As String = getEscdate(LRentESC, AlreadyEistData.Rows(0).Item("EscDate").ToString())

                                                                        'Dim ESCFinaldateARR As String() = EscalationDate.Split("/")
                                                                        'EscalationDate = ESCFinaldateARR(1) + "/" + ESCFinaldateARR(0) + "/" + ESCFinaldateARR(2)
                                                                        Dim ESCdt As Date = Convert.ToDateTime(EscalationDate)
                                                                        Dim ESCInvGenDate As String = ESCdt.ToString("MM/dd/yy")
                                                                        Dim ESCInvGenDateStr As String() = ESCInvGenDate.Split("/")
                                                                        Dim ESCLInvGenDateStr As String = ESCInvGenDateStr(1) + "/" + ESCInvGenDateStr(0) + "/" + ESCInvGenDateStr(2)
                                                                        'Calculating the Actual lease start date
                                                                        Dim InvCreationDtarr = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString().Split("/")
                                                                        Dim InvCreationLRIGDTdate1 As New Date(InvCreationDtarr(2), InvCreationDtarr(1), InvCreationDtarr(0))
                                                                        InvCreationLRIGDTdate1 = CDate(InvCreationLRIGDTdate1)
                                                                        Dim InvGenDt As DateTime = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("MM/dd/yy"))

                                                                        Dim InvGenDate As String = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("dd/MM/yy"))
                                                                        Dim InvGenDateStr As String() = InvGenDate.Split("/")
                                                                        Dim LInvGenDateStr As String = InvGenDateStr(1) + "/" + InvGenDateStr(0) + "/" + InvGenDateStr(2)
                                                                        'Dim LComDate As DateTime = Convert.ToDateTime(LeaseComDate)

                                                                        Dim LMGAmount As Double = 0
                                                                        LMGAmount = LMGAmt
                                                                        'Date.Now
                                                                        Dim ResultStr = ParseDateFn(LRentPayment, InvGenDt, LMGAmount)
                                                                        Dim PDFnResultStr = ResultStr.Split("=")
                                                                        FinalInvDate = PDFnResultStr(0)
                                                                        MGAmtDT = PDFnResultStr(1)

                                                                        insStr = ""
                                                                        ' RentyearInTheFuture
                                                                        'InvGenDt
                                                                        'date.Now() 

                                                                        Dim Result As String() = ParseEscDateFn(LRentPayment, ESCGenDt, InvGenDt, LMGAmount).Split("=")

                                                                        Dim TotalDays As String = ""
                                                                        Dim BAMnt As Double = 0
                                                                        Dim ADays As Int16 = 0
                                                                        Dim BDays As Int16 = 0
                                                                        Dim NInvdtdt As String = ""

                                                                        NInvdtdt = Result(0)

                                                                        BDays = Result(1)
                                                                        ADays = Result(2)
                                                                        BAMnt = Result(3)
                                                                        TotalDays = Result(4)


                                                                        If ((CDate(LEdt1) >= InvGenDt) And (CDate(NInvdtdt) > CDate(LEdt1))) Then

                                                                            If names(f).ToUpper() = ("Rental Invoice").ToUpper() Then
                                                                                insStr = ""

                                                                                Dim LeaseEndDate As String = LEdt1
                                                                                Dim FinaldateARR As String() = LeaseEndDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = CDate(FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)).ToString("dd/MM/yy")


                                                                                If LRenttype = 1554651 Or LRenttype = 1554653 Then 'Fixed or 'Fixed and Revenue Sharing 


                                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    End If
                                                                                    da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Transaction = tran
                                                                                    If con.State = ConnectionState.Closed Then
                                                                                        con.Open()
                                                                                    End If
                                                                                    res = da.SelectCommand.ExecuteScalar()

                                                                                    If res <> 0 Then


                                                                                        Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime' and Documenttype='rental Invoice'")
                                                                                        If rowRCDF.Length > 0 Then
                                                                                            For Each CField As DataRow In rowRCDF

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Next
                                                                                        End If

                                                                                        Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In rowECD
                                                                                                If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & ESCLInvGenDateStr & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                Else
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("EscDate").ToString() & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                End If


                                                                                            Next
                                                                                        End If
                                                                                        Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric' and Documenttype='rental Invoice'")
                                                                                        If rowMGAmt.Length > 0 Then
                                                                                            For Each CField As DataRow In rowMGAmt
                                                                                                MGAmtDT = BAMnt
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()
                                                                                            Next
                                                                                        End If
                                                                                        'Lease Period Start	
                                                                                        'InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")
                                                                                        Dim LSPARR As String() = InvGenDate.Split("/")
                                                                                        InvGenDate = LSPARR(1) + "/" + LSPARR(0) + "/" + LSPARR(2)
                                                                                        InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")

                                                                                        Dim SprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period Start' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In SprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & InvGenDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If

                                                                                        'Lease Period End	
                                                                                        Dim LPEdate As Date = New Date(CDate(FinalInvDate).Year, CDate(FinalInvDate).Month, 1).AddDays(-1)
                                                                                        Dim LPEdatestr As String = LPEdate.ToString("dd/MM/yy")
                                                                                        Dim EprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period End' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In EprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LPEdatestr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If
                                                                                        Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)
                                                                                        ''INSERT INTO HISTORY 
                                                                                        ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                        Try
                                                                                            UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                            da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                            Dim srerd As String = String.Empty
                                                                                        Catch ex As Exception

                                                                                        End Try
                                                                                        tran.Commit()

                                                                                        'Check Work Flow
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.Parameters.Clear()
                                                                                        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                        Dim dt1 As New DataTable
                                                                                        da.Fill(dt1)
                                                                                        If dt1.Rows.Count = 1 Then
                                                                                            If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                            End If
                                                                                        End If
                                                                                    Else
                                                                                        tran.Commit()
                                                                                    End If


                                                                                End If

                                                                            End If

                                                                        ElseIf ((CDate(ESCGenDt) > InvGenDt) And (CDate(NInvdtdt) > CDate(ESCGenDt))) Then

                                                                            If names(f).ToUpper() = ("Rental Invoice").ToUpper() Then
                                                                                'If con.State = ConnectionState.Closed Then
                                                                                '    con.Open()
                                                                                'End If
                                                                                Finaldate = ""
                                                                                Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)
                                                                                'tran = con.BeginTransaction()
                                                                                If LRenttype = 1554651 Or LRenttype = 1554653 Then 'Fixed or 'Fixed and Revenue Sharing 



                                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    End If
                                                                                    da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Transaction = tran
                                                                                    If con.State = ConnectionState.Closed Then
                                                                                        con.Open()
                                                                                    End If
                                                                                    res = da.SelectCommand.ExecuteScalar()

                                                                                    If res <> 0 Then
                                                                                        ' Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)

                                                                                        Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime' and Documenttype='rental Invoice'")
                                                                                        If rowRCDF.Length > 0 Then
                                                                                            For Each CField As DataRow In rowRCDF

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Next
                                                                                        End If

                                                                                        Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In rowECD
                                                                                                If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & ESCLInvGenDateStr & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                Else
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("EscDate").ToString() & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                End If
                                                                                            Next
                                                                                        End If

                                                                                        'Lease Period Start	
                                                                                        'InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")
                                                                                        Dim LSPARR As String() = InvGenDate.Split("/")
                                                                                        InvGenDate = LSPARR(1) + "/" + LSPARR(0) + "/" + LSPARR(2)
                                                                                        InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")

                                                                                        Dim SprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period Start' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In SprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & InvGenDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If

                                                                                        'Lease Period End	
                                                                                        Dim LPEdate As Date = New Date(CDate(FinalInvDate).Year, CDate(FinalInvDate).Month, 1).AddDays(-1)
                                                                                        Dim LPEdatestr As String = LPEdate.ToString("dd/MM/yy")
                                                                                        Dim EprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period End' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In EprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LPEdatestr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If
                                                                                        Dim AmountSum As Double = 0
                                                                                        'Calculating Rent Before  Esclation

                                                                                        Dim LessorsMgAmount As Double = 0
                                                                                        Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric' and Documenttype='rental Invoice'")
                                                                                        If rowMGAmt.Length > 0 Then
                                                                                            For Each CField As DataRow In rowMGAmt

                                                                                                LessorsMgAmount = (BAMnt * LPropershare) / 100
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsMgAmount & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()
                                                                                            Next
                                                                                        End If
                                                                                        Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                        ''INSERT INTO HISTORY 
                                                                                        ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                        Try
                                                                                            UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                            da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                            Dim srerd As String = String.Empty
                                                                                        Catch ex As Exception

                                                                                        End Try

                                                                                        tran.Commit()

                                                                                        'Non transactional Query
                                                                                        'Check Work Flow
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.Parameters.Clear()
                                                                                        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                        Dim dt1 As New DataTable
                                                                                        da.Fill(dt1)
                                                                                        If dt1.Rows.Count = 1 Then
                                                                                            If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                            End If
                                                                                        End If
                                                                                    End If


                                                                                    res = 0
                                                                                    If con.State = ConnectionState.Closed Then
                                                                                        con.Open()
                                                                                    End If
                                                                                    Finaldate = ""
                                                                                    tran = con.BeginTransaction()
                                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDate & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    End If
                                                                                    da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Transaction = tran
                                                                                    If con.State = ConnectionState.Closed Then
                                                                                        con.Open()
                                                                                    End If
                                                                                    res = da.SelectCommand.ExecuteScalar()

                                                                                    If res <> 0 Then
                                                                                        ' Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)

                                                                                        Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime' and Documenttype='rental Invoice'")
                                                                                        If rowRCDF.Length > 0 Then
                                                                                            For Each CField As DataRow In rowRCDF

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Next
                                                                                        End If

                                                                                        Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In rowECD
                                                                                                If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & ESCLInvGenDateStr & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                Else
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("EscDate").ToString() & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                End If


                                                                                            Next
                                                                                        End If
                                                                                        'Lease Period Start	
                                                                                        InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")
                                                                                        Dim SprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period Start' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In SprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & InvGenDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If

                                                                                        'Lease Period End	
                                                                                        Dim EprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period End' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In EprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If
                                                                                        Dim AmountSum As Double = 0
                                                                                        'Calculating Rent After  Esclation
                                                                                        Dim MGAmtDtable As New DataTable()
                                                                                        Dim Amount As Double = 0

                                                                                        AmountSum = (LMGAmount * LRentESCPtage) / 100
                                                                                        AmountSum = LMGAmount + AmountSum
                                                                                        If LRentPayment = "1554654" Then '"Monthly"
                                                                                            AmountSum = AmountSum
                                                                                        ElseIf LRentPayment = "1554655" Then '"Quarterly"
                                                                                            AmountSum = AmountSum * 3

                                                                                        ElseIf LRentPayment = "1554656" Then 'Half Yearly
                                                                                            AmountSum = AmountSum * 6

                                                                                        End If

                                                                                        Dim LessorsMgAmount As Double = 0
                                                                                        Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric' and Documenttype='rental Invoice'")
                                                                                        If rowMGAmt.Length > 0 Then
                                                                                            For Each CField As DataRow In rowMGAmt

                                                                                                Amount = AmountSum / TotalDays
                                                                                                Amount = Amount * ADays
                                                                                                LessorsMgAmount = (Amount * LPropershare) / 100
                                                                                                Dim BdecimalVar As Decimal
                                                                                                BdecimalVar = Decimal.Round(LessorsMgAmount, 2, MidpointRounding.AwayFromZero)
                                                                                                BdecimalVar = Math.Round(BdecimalVar, 2)
                                                                                                LessorsMgAmount = BdecimalVar
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsMgAmount & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()
                                                                                                AmountSum = 0
                                                                                                AmountSum = (LMGAmount * LRentESCPtage) / 100
                                                                                                AmountSum = LMGAmount + AmountSum
                                                                                                LessorsMgAmount = 0
                                                                                                LessorsMgAmount = (AmountSum * LPropershare) / 100
                                                                                            Next
                                                                                        End If


                                                                                        Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)
                                                                                        'RentyearInTheFuture

                                                                                        Call RentEsclationLeaseMasterUpdate(EID, ds.Tables("AutoInvDocData").Rows(j).Item("TID"), LMGAmount, LessorsMgAmount, Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")), con, tran)
                                                                                        ''INSERT INTO HISTORY 
                                                                                        ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                        Try
                                                                                            UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                            da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                            Dim srerd As String = String.Empty
                                                                                        Catch ex As Exception

                                                                                        End Try

                                                                                        tran.Commit()

                                                                                        'Check Work Flow
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.Parameters.Clear()
                                                                                        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                        Dim dt1 As New DataTable
                                                                                        da.Fill(dt1)
                                                                                        If dt1.Rows.Count = 1 Then
                                                                                            If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                            End If
                                                                                        End If
                                                                                    Else
                                                                                        tran.Commit()
                                                                                    End If

                                                                                End If
                                                                            End If

                                                                        ElseIf CDate(InvGenDt) = Date.Now Then
                                                                            If names(f).ToUpper() = ("Rental Invoice").ToUpper() Then
                                                                                'Dim Finaldate As String = String.Empty
                                                                                Finaldate = ""
                                                                                Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)

                                                                                If LRenttype = 1554651 Or LRenttype = 1554653 Then 'Fixed or 'Fixed and Revenue Sharing 



                                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                    End If
                                                                                    da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Transaction = tran
                                                                                    If con.State = ConnectionState.Closed Then
                                                                                        con.Open()
                                                                                    End If
                                                                                    res = da.SelectCommand.ExecuteScalar()

                                                                                    If res <> 0 Then


                                                                                        Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime' and Documenttype='rental Invoice'")
                                                                                        If rowRCDF.Length > 0 Then
                                                                                            For Each CField As DataRow In rowRCDF

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Next
                                                                                        End If

                                                                                        Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In rowECD
                                                                                                If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & ESCLInvGenDateStr & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                Else
                                                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("EscDate").ToString() & "'  where tid =" & res & ""
                                                                                                    da.SelectCommand.CommandText = upquery
                                                                                                    da.SelectCommand.Transaction = tran
                                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                                    da.SelectCommand.ExecuteNonQuery()

                                                                                                End If


                                                                                            Next
                                                                                        End If
                                                                                        'Lease Period Start	
                                                                                        'InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")
                                                                                        Dim LSPARR As String() = InvGenDate.Split("/")
                                                                                        InvGenDate = LSPARR(1) + "/" + LSPARR(0) + "/" + LSPARR(2)
                                                                                        InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")

                                                                                        Dim SprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period Start' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In SprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & InvGenDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If

                                                                                        'Lease Period End	
                                                                                        Dim LPEdate As Date = New Date(CDate(FinalInvDate).Year, CDate(FinalInvDate).Month, 1).AddDays(-1)
                                                                                        Dim LPEdatestr As String = LPEdate.ToString("dd/MM/yy")
                                                                                        Dim EprowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Period End' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                                                        If rowECD.Length > 0 Then
                                                                                            For Each CField As DataRow In EprowECD

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LPEdatestr & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()


                                                                                            Next
                                                                                        End If
                                                                                        Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric' and Documenttype='rental Invoice'")
                                                                                        If rowMGAmt.Length > 0 Then
                                                                                            For Each CField As DataRow In rowMGAmt

                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()
                                                                                            Next
                                                                                        End If

                                                                                        Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)
                                                                                        ''INSERT INTO HISTORY 
                                                                                        ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                        Try
                                                                                            UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                            da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                            Dim srerd As String = String.Empty
                                                                                        Catch ex As Exception

                                                                                        End Try
                                                                                        tran.Commit()

                                                                                        'Check Work Flow
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.Parameters.Clear()
                                                                                        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                        Dim dt1 As New DataTable
                                                                                        da.Fill(dt1)
                                                                                        If dt1.Rows.Count = 1 Then
                                                                                            If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                            End If
                                                                                        End If
                                                                                    Else
                                                                                        tran.Commit()
                                                                                    End If
                                                                                End If
                                                                            End If

                                                                        Else
                                                                            tran.Commit()
                                                                        End If


                                                                    End If


                                                                End If
                                                            Catch ex As Exception
                                                                AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "Rental AutoInvoice Function Exception:" + ex.Message.ToString(), 181)

                                                            End Try
                                                        End If
                                                    ElseIf names(f).ToUpper() = ("Security deposit").ToUpper() Then
                                                        If names(f).ToUpper() = ("Security Deposit").ToUpper() Then

                                                            Dim dvs As DataView = ds.Tables("AutoInvFieldData").DefaultView
                                                            dvs.RowFilter = "TDocType='" & names(f) & "'"

                                                            Dim filteredTable As New DataTable()
                                                            filteredTable = dvs.ToTable()
                                                            If filteredTable.Rows.Count <> 0 Then
                                                                strTFlds = ""
                                                                strSFlds = ""
                                                                strTFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                                                strSFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                                                strHSFldsArr = Nothing
                                                                strHSFldsArr = strSFlds.Split(",")
                                                                valueL = String.Join(",',',", strHSFldsArr)
                                                                strHSFlds = valueL
                                                            End If
                                                            If SourceDocData.Rows.Count > 0 Then
                                                                SourceDocData.Clear()
                                                            End If
                                                            If FieldData.Rows.Count > 0 Then
                                                                FieldData.Clear()
                                                            End If
                                                            da.SelectCommand.CommandText = " select tid as RDOCID,concat(" & strHSFlds & ") as TFldVAl from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & "   where documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "'  and eid=" & EID & " and tid=" & ds.Tables("AutoInvDocData").Rows(j).Item("Tid") & " "
                                                            da.SelectCommand.CommandType = CommandType.Text

                                                            da.Fill(ds, "SourceDocData")

                                                            SourceDocData = ds.Tables("SourceDocData")
                                                            Dim RDOCID As String = ""
                                                            If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                                                FldsVal = ""
                                                                FldsVal = ds.Tables("SourceDocData").Rows(0).Item("TFldVAl")
                                                                FldsValArr = FldsVal.Split(",")
                                                                value = ""
                                                                value = String.Join("','", FldsValArr)
                                                                value = "'" + value + "'"
                                                                RDOCID = Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("RDOCID"))
                                                            End If
                                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.Fill(ds, "fields")
                                                            FieldData = ds.Tables("fields")
                                                            Try
                                                                If ds.Tables("SourceDocData").Rows.Count > 0 Then


                                                                    If RDOCIDData.Rows.Count > 0 Then
                                                                        RDOCIDData.Clear()
                                                                    End If
                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then
                                                                        da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate();  select * from MMM_MST_MASTER where RefTid=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "' --and adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE) "
                                                                    Else
                                                                        da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate(); select * from MMM_MST_DOC where RDOCID=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "' --and  adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE);"
                                                                    End If
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.Fill(ds, "RDOCIDData")
                                                                    RDOCIDData = ds.Tables("RDOCIDData")
                                                                    If ds.Tables("RDOCIDData").Rows.Count = 0 Then
                                                                        If CDate(LeaseComDate) = Date.Now Then

                                                                            If con.State = ConnectionState.Closed Then
                                                                                con.Open()
                                                                            End If
                                                                            tran = con.BeginTransaction()
                                                                            insStr = ""

                                                                            If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                            ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & LeaseComDate & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                            End If
                                                                            da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                            da.SelectCommand.Transaction = tran
                                                                            If con.State = ConnectionState.Closed Then
                                                                                con.Open()
                                                                            End If
                                                                            res = da.SelectCommand.ExecuteScalar()


                                                                            If res <> 0 Then

                                                                                'Calculeting the Actual lease start date
                                                                                Dim SDInvDate As DateTime = Convert.ToDateTime(LSdate1.ToString("MM/dd/yy"))
                                                                                'Dim LComDate As DateTime = Convert.ToDateTime(LeaseComDate)

                                                                                Dim FinalInvDate As String = String.Empty
                                                                                Dim MGAmtDT As Double = 0
                                                                                Dim ParseDate As String = String.Empty
                                                                                If LRentFreedays = "0" Then
                                                                                    ParseDate = SDInvDate
                                                                                Else
                                                                                    ParseDate = LeaseComDate
                                                                                End If
                                                                                Dim LSDAmount As Double = 0
                                                                                LSDAmount = LSDAmt - LTokenAmt

                                                                                Dim LMGAmount As Double = 0

                                                                                LMGAmount = LMGAmt

                                                                                Dim ResultStr = ParseDateFn(LRentPayment, ParseDate, LMGAmount)
                                                                                Dim PDFnResultStr = ResultStr.Split("=")
                                                                                FinalInvDate = PDFnResultStr(0)
                                                                                MGAmtDT = PDFnResultStr(1)
                                                                                'calculate nest sd inv date

                                                                                Dim SDyearInTheFuture As Date = Convert.ToDateTime(ParseDate).AddYears(1)
                                                                                Dim SDyearInTheFuture1 As New Date(SDyearInTheFuture.Year, SDyearInTheFuture.Month, SDyearInTheFuture.Day)

                                                                                Dim SDFinalStr As String = SDyearInTheFuture1.ToString("dd/MM/yy")

                                                                                'Creation date as dynamic field date data update
                                                                                Dim DateTimeField() As DataRow = ds.Tables("fields").Select("DisplayName='Next SD Invoice date' and Datatype='Datetime'")
                                                                                If DateTimeField.Length > 0 Then
                                                                                    For Each CField As DataRow In DateTimeField

                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & SDFinalStr & "'  where tid =" & res & " and  DocumentType='Security Deposit' "
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                    Next

                                                                                End If
                                                                                ''Rental Amount as dynamic field date data update
                                                                                'Dim RAmtField() As DataRow = ds.Tables("fields").Select("DisplayName='Lease Rental amount' and Datatype='Numeric'")
                                                                                'If RAmtField.Length > 0 Then
                                                                                '    For Each CField As DataRow In RAmtField

                                                                                '        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where tid =" & res & " and  DocumentType='Security Deposit' "
                                                                                '        da.SelectCommand.CommandText = upquery
                                                                                '        da.SelectCommand.CommandType = CommandType.Text
                                                                                '        da.SelectCommand.ExecuteNonQuery()
                                                                                '    Next

                                                                                'End If
                                                                                'SD Amount as dynamic field date data update
                                                                                Dim SDAmtField() As DataRow = ds.Tables("fields").Select("DisplayName='SD Adjustment Amount' and Datatype='Numeric'")
                                                                                If SDAmtField.Length > 0 Then
                                                                                    For Each CField As DataRow In SDAmtField

                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LSDAmount & "'  where tid =" & res & " and  DocumentType='Security Deposit' "
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                    Next

                                                                                End If
                                                                                'Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)
                                                                                Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                Try
                                                                                    UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                    da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.ExecuteNonQuery()
                                                                                    Dim srerd As String = String.Empty
                                                                                Catch ex As Exception

                                                                                End Try
                                                                                tran.Commit()

                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Parameters.Clear()
                                                                                da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                Dim dt1 As New DataTable
                                                                                da.Fill(dt1)
                                                                                If dt1.Rows.Count = 1 Then
                                                                                    If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                    End If
                                                                                End If
                                                                            Else
                                                                                tran.Commit()
                                                                            End If


                                                                        End If
                                                                    ElseIf ds.Tables("RDOCIDData").Rows.Count > 0 And (Convert.ToString(names(f)) = "Security Deposit") Then

                                                                        Dim Finaldate As String = String.Empty
                                                                        If con.State = ConnectionState.Closed Then
                                                                            con.Open()
                                                                        End If
                                                                        tran = con.BeginTransaction()
                                                                        Dim FinalInvDate As String = String.Empty
                                                                        Dim AlreadyEistData As New DataTable
                                                                        Dim MGAmtDT As Double = 0
                                                                        Dim Fieldmap As String = String.Empty
                                                                        If Convert.ToString(names(f)) = "Security Deposit" Then
                                                                            Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next SD Invoice date' and Datatype='Datetime'")

                                                                            If rowRCD.Length > 0 Then
                                                                                For Each CField As DataRow In rowRCD
                                                                                    Fieldmap = CField.Item("FieldMapping").ToString()

                                                                                Next
                                                                            End If
                                                                            da.SelectCommand.CommandText = "Select top 1 " & Fieldmap & "  as InvCreationDt from mmm_mst_doc with (nolock) where fld19='" & LDocNo & "' and eid=" & EID & "  and Documenttype='" & Convert.ToString(names(f)) & "' and RDOCID=" & ds.Tables("AutoInvDocData").Rows(j).Item("tid") & " order by tid desc "
                                                                            da.SelectCommand.Transaction = tran
                                                                        End If

                                                                        If AlreadyEistData.Rows.Count > 0 Then
                                                                            AlreadyEistData.Clear()
                                                                        End If
                                                                        da.Fill(AlreadyEistData)
                                                                        'Calculeting the Actual lease start date
                                                                        Dim InvCreationDt As String = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString()

                                                                        Dim SDTFyearInTheFuture As Boolean = False
                                                                        Dim SDyearInTheFuture As String = ""
                                                                        Dim yearInTheFuture1 As Date
                                                                        yearInTheFuture1 = Date.Now
                                                                        Dim SDEscDAte As String = getEscdate(LRentESC, InvCreationDt)

                                                                        Dim FinalDateArr As String() = SDEscDAte.Split("/")
                                                                        Finaldate = CDate(SDEscDAte).ToString("dd/MM/yy")
                                                                        '      Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                        'Calculating the Actual lease start date
                                                                        Dim InvCreationDtarr = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString().Split("/")
                                                                        Dim InvCreationLRIGDTdate1 As New Date(InvCreationDtarr(2), InvCreationDtarr(1), InvCreationDtarr(0))
                                                                        InvCreationLRIGDTdate1 = CDate(InvCreationLRIGDTdate1)
                                                                        Dim InvGenDt As DateTime = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("MM/dd/yy"))



                                                                        If CDate(InvCreationDt) = Date.Now Then
                                                                            insStr = ""
                                                                            If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                            ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),getdate(),getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                            End If
                                                                            da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                            da.SelectCommand.Transaction = tran
                                                                            If con.State = ConnectionState.Closed Then
                                                                                con.Open()
                                                                            End If
                                                                            res = da.SelectCommand.ExecuteScalar()


                                                                            If res <> 0 Then
                                                                                'Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)

                                                                                'Creation date as dynamic field date data update
                                                                                Dim DateTimeField() As DataRow = ds.Tables("fields").Select("DisplayName='Next SD Invoice date'  and Documenttype='Security Deposit'")
                                                                                If DateTimeField.Length > 0 Then
                                                                                    For Each CField As DataRow In DateTimeField
                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & Finaldate & "'  where tid =" & res & " and  DocumentType='Security Deposit'"
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                    Next
                                                                                End If


                                                                                Dim LSDAmount As Double = 0
                                                                                LSDAmount = LSDAmt

                                                                                Dim LMGAmount As Double = 0

                                                                                LMGAmount = LMGAmt

                                                                                Dim ResultStr = ParseDateFn(LRentPayment, SDEscDAte, LMGAmount)
                                                                                Dim PDFnResultStr = ResultStr.Split("=")
                                                                                FinalInvDate = PDFnResultStr(0)
                                                                                MGAmtDT = PDFnResultStr(1)

                                                                                'Rental Amount as dynamic field date data update
                                                                                'Dim RAmtField() As DataRow = ds.Tables("fields").Select("DisplayName='Lease Rental amount' and Datatype='Numeric'")
                                                                                'If RAmtField.Length > 0 Then
                                                                                '    For Each CField As DataRow In RAmtField

                                                                                '        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where tid =" & res & " and  DocumentType='Security Deposit' "
                                                                                '        da.SelectCommand.CommandText = upquery
                                                                                '        da.SelectCommand.CommandType = CommandType.Text
                                                                                '        da.SelectCommand.ExecuteNonQuery()
                                                                                '    Next

                                                                                'End If
                                                                                Dim MGAmtDtable As New DataTable()
                                                                                Dim Amount As Double = 0
                                                                                Dim AmountSum As Double = 0
                                                                                Dim TMGAmountFld As String = ""
                                                                                Dim LMGAmountFld As String = ""
                                                                                Dim rowTMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='SD Invoice amount'  and Datatype='Numeric' and Documenttype='Security Deposit'")
                                                                                If rowTMGAmt.Length > 0 Then
                                                                                    For Each CField As DataRow In rowTMGAmt
                                                                                        'Calculating Rent Esclation

                                                                                        Dim SelectStr As String = "Select " & CField.Item("FieldMapping").ToString & " as TMGAmt from  " & CField.Item("DBTableName").ToString & "  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                        da.SelectCommand.CommandText = SelectStr
                                                                                        da.SelectCommand.Transaction = tran
                                                                                        If MGAmtDtable.Rows.Count > 0 Then
                                                                                            MGAmtDtable.Clear()
                                                                                        End If
                                                                                        da.Fill(MGAmtDtable)
                                                                                        If MGAmtDtable.Rows.Count > 0 Then
                                                                                            Amount = Convert.ToDouble(MGAmtDtable.Rows(0).Item("TMGAmt"))
                                                                                            AmountSum = (Amount * LRentESCPtage) / 100
                                                                                            Amount = Amount + AmountSum
                                                                                        End If
                                                                                        TMGAmountFld = CField.Item("FieldMapping").ToString
                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & Amount & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.Transaction = tran
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                    Next
                                                                                End If

                                                                                Dim LessorsMgAmount As Double = 0
                                                                                Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='SD Invoice amount'  and Datatype='Numeric' and Documenttype='Security Deposit'")
                                                                                If rowMGAmt.Length > 0 Then
                                                                                    For Each CField As DataRow In rowMGAmt

                                                                                        LMGAmountFld = CField.Item("FieldMapping").ToString
                                                                                        LessorsMgAmount = (Amount * LPropershare) / 100
                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsMgAmount & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.Transaction = tran
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                    Next
                                                                                End If

                                                                                'RentyearInTheFuture

                                                                                Call SDEsclationLeaseMasterUpdate(TMGAmountFld, LMGAmountFld, EID, ds.Tables("AutoInvDocData").Rows(j).Item("TID"), Amount, LessorsMgAmount, Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")), con, tran)
                                                                                ''INSERT INTO HISTORY 
                                                                                ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)
                                                                                Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                Try
                                                                                    UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                    da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.ExecuteNonQuery()
                                                                                    Dim srerd As String = String.Empty
                                                                                Catch ex As Exception

                                                                                End Try
                                                                                tran.Commit()

                                                                                'Non transactional Query
                                                                                'Check Work Flow
                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Parameters.Clear()
                                                                                da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                Dim dt1 As New DataTable
                                                                                da.Fill(dt1)
                                                                                If dt1.Rows.Count = 1 Then
                                                                                    If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                    End If
                                                                                End If
                                                                            Else
                                                                                tran.Commit()
                                                                            End If



                                                                        End If

                                                                    End If

                                                                End If
                                                            Catch ex As Exception
                                                                AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "Security Deposit AutoInvoice Function Exception:" + ex.Message.ToString(), 181)
                                                            End Try
                                                        End If
                                                    ElseIf names(f).ToUpper() = ("CAM").ToUpper() Then
                                                        If names(f).ToUpper() = ("CAM").ToUpper() Then

                                                            Dim dvs As DataView = ds.Tables("AutoInvFieldData").DefaultView
                                                            dvs.RowFilter = "TDocType='" & names(f) & "'"

                                                            Dim filteredTable As New DataTable()
                                                            filteredTable = dvs.ToTable()
                                                            If filteredTable.Rows.Count <> 0 Then
                                                                strTFlds = ""
                                                                strSFlds = ""
                                                                strTFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                                                strSFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                                                strHSFldsArr = Nothing
                                                                strHSFldsArr = strSFlds.Split(",")
                                                                valueL = String.Join(",',',", strHSFldsArr)
                                                                strHSFlds = valueL
                                                            End If
                                                            If SourceDocData.Rows.Count > 0 Then
                                                                SourceDocData.Clear()
                                                            End If
                                                            If FieldData.Rows.Count > 0 Then
                                                                FieldData.Clear()
                                                            End If
                                                            da.SelectCommand.CommandText = " select tid as RDOCID,fld11 as [Name of Lessor],concat(" & strHSFlds & ") as TFldVAl from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & "   where documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "'  and eid=" & EID & " and tid=" & ds.Tables("AutoInvDocData").Rows(j).Item("Tid") & " "
                                                            da.SelectCommand.CommandType = CommandType.Text

                                                            da.Fill(ds, "SourceDocData")

                                                            SourceDocData = ds.Tables("SourceDocData")
                                                            Dim RDOCID As String = ""
                                                            If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                                                FldsVal = ""
                                                                FldsVal = ds.Tables("SourceDocData").Rows(0).Item("TFldVAl")
                                                                FldsValArr = FldsVal.Split(",")
                                                                value = ""
                                                                value = String.Join("','", FldsValArr)
                                                                value = "'" + value + "'"
                                                                RDOCID = Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("RDOCID"))
                                                            End If
                                                            Try


                                                                If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                                                    If RDOCIDData.Rows.Count > 0 Then
                                                                        RDOCIDData.Clear()
                                                                    End If
                                                                    'Name of Lessor
                                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then
                                                                        da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate();  select * from MMM_MST_MASTER where RefTid=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "'  --and adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE) "
                                                                    Else
                                                                        da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate(); select * from MMM_MST_DOC where fld6='" & Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("Name of Lessor")) & "'  and  fld13='" & LDocNo & "' and documenttype='" & Convert.ToString(names(f)) & "' --and  adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE);"
                                                                    End If
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.Fill(ds, "RDOCIDData")
                                                                    RDOCIDData = ds.Tables("RDOCIDData")
                                                                    If FieldData.Rows.Count > 0 Then
                                                                        FieldData.Clear()
                                                                    End If
                                                                    'Collecting the field data
                                                                    da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.Fill(ds, "fields")
                                                                    FieldData = ds.Tables("fields")

                                                                    If ds.Tables("RDOCIDData").Rows.Count = 0 Then
                                                                        'check Inv Gen date
                                                                        If CDate(LCAMComDate) = Date.Now Then 'date.now

                                                                            If con.State = ConnectionState.Closed Then
                                                                                con.Open()
                                                                            End If
                                                                            tran = con.BeginTransaction()
                                                                            insStr = ""
                                                                            If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                            ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & LeaseComDate & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                            End If
                                                                            da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                            da.SelectCommand.Transaction = tran
                                                                            If con.State = ConnectionState.Closed Then
                                                                                con.Open()
                                                                            End If
                                                                            res = da.SelectCommand.ExecuteScalar()

                                                                            If res <> 0 Then

                                                                                'Calculeting the Actual lease start date
                                                                                Dim date2 As DateTime = Convert.ToDateTime(LSdate1.ToString("MM/dd/yy"))
                                                                                Dim LESCDate As Date = LeaseComDate

                                                                                Dim FinalInvDate As String = String.Empty
                                                                                Dim MGAmtDT As Double
                                                                                Dim ParseDate As String = String.Empty
                                                                                If LRentFreedays = "0" Then
                                                                                    ParseDate = date2
                                                                                Else
                                                                                    ParseDate = LeaseComDate
                                                                                End If
                                                                                Dim LCAMAmount As Double = 0
                                                                                LCAMAmount = LCAMAmt

                                                                                Dim ResultStr = ParseDateFn(LCAMRentCycletype, ParseDate, LCAMAmount)
                                                                                Dim PDFnResultStr = ResultStr.Split("=")
                                                                                FinalInvDate = PDFnResultStr(0)
                                                                                MGAmtDT = PDFnResultStr(1)

                                                                                Dim Finaldate As String = String.Empty
                                                                                Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)


                                                                                MGAmtDT = (MGAmtDT * LPropershare) / 100

                                                                                'Dim Finaldate As String = String.Empty
                                                                                'Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                'Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)

                                                                                Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Amount'  and Datatype='Numeric'")
                                                                                If rowMGAmt.Length > 0 Then
                                                                                    For Each CField As DataRow In rowMGAmt

                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.Transaction = tran
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                    Next
                                                                                End If

                                                                                Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next CAM Invoice date' and Datatype='Datetime'")
                                                                                If rowRCD.Length > 0 Then
                                                                                    For Each CField As DataRow In rowRCD

                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.Transaction = tran
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()

                                                                                    Next
                                                                                End If
                                                                                Dim LESCDatestr As String = LESCDate.ToString("dd/MM/yy")
                                                                                'Dim LESCDatestr1() As String = LESCDate.ToString("dd/MM/yy").Split("/")
                                                                                'Dim LESCDatestr As String = LESCDatestr1(1) + "/" + LESCDatestr1(0) + "/" + LESCDatestr1(2)
                                                                                Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Escalation date' and Datatype='Datetime'")
                                                                                If rowECD.Length > 0 Then
                                                                                    For Each CField As DataRow In rowECD

                                                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LESCDatestr & "'  where tid =" & res & ""
                                                                                        da.SelectCommand.CommandText = upquery
                                                                                        da.SelectCommand.Transaction = tran
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()

                                                                                    Next
                                                                                End If
                                                                                'Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)
                                                                                Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                ''INSERT INTO HISTORY 
                                                                                ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                Try
                                                                                    UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                    da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.ExecuteNonQuery()
                                                                                    Dim srerd As String = String.Empty
                                                                                Catch ex As Exception

                                                                                End Try
                                                                                tran.Commit()

                                                                                'Check Work Flow
                                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Parameters.Clear()
                                                                                da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                Dim dt1 As New DataTable
                                                                                da.Fill(dt1)
                                                                                If dt1.Rows.Count = 1 Then
                                                                                    If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                    End If
                                                                                End If
                                                                            Else
                                                                                tran.Commit()
                                                                            End If

                                                                        End If

                                                                    ElseIf ds.Tables("RDOCIDData").Rows.Count > 0 And Convert.ToString(names(f)) = "CAM" Then

                                                                        If con.State = ConnectionState.Closed Then
                                                                            con.Open()
                                                                        End If
                                                                        Dim Finaldate As String = String.Empty
                                                                        tran = con.BeginTransaction()
                                                                        Dim FinalInvDate As String = String.Empty
                                                                        Dim AlreadyEistData As New DataTable
                                                                        Dim MGAmtDT As Double = 0
                                                                        Dim Fieldmap As String = String.Empty
                                                                        Dim ESCFieldmap As String = String.Empty
                                                                        If Convert.ToString(names(f)) = "CAM" Then
                                                                            Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next CAM Invoice date' and Datatype='Datetime'")
                                                                            Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Cam Escalation date' and Datatype='Datetime'")
                                                                            If rowRCD.Length > 0 Then
                                                                                For Each CField As DataRow In rowRCD
                                                                                    Fieldmap = CField.Item("FieldMapping").ToString()
                                                                                Next
                                                                            End If
                                                                            If rowECD.Length > 0 Then
                                                                                For Each CField As DataRow In rowECD
                                                                                    ESCFieldmap = CField.Item("FieldMapping").ToString()
                                                                                Next
                                                                            End If
                                                                            da.SelectCommand.CommandText = "Select top 1 " & Fieldmap & "  as InvCreationDt," & ESCFieldmap & " as CAmEscDate from mmm_mst_doc with (nolock) where RDOCID=" & RDOCID & " and eid=" & EID & "  and Documenttype='" & Convert.ToString(names(f)) & "' and RDOCID=" & ds.Tables("AutoInvDocData").Rows(j).Item("tid") & " order by tid desc "


                                                                        End If
                                                                        da.SelectCommand.Transaction = tran
                                                                        If AlreadyEistData.Rows.Count > 0 Then
                                                                            AlreadyEistData.Clear()
                                                                        End If
                                                                        da.Fill(AlreadyEistData)

                                                                        'Calculating Escalation date  from previous Escalation date
                                                                        Dim ESCDtarr = AlreadyEistData.Rows(0).Item("CAmEscDate").ToString().Split("/")
                                                                        Dim ESCDTdate1 As New Date(ESCDtarr(2), ESCDtarr(1), ESCDtarr(0))
                                                                        ESCDTdate1 = CDate(ESCDTdate1)
                                                                        Dim ESCGenDt As DateTime = Convert.ToDateTime(ESCDTdate1.ToString("MM/dd/yy"))
                                                                        Dim EscalationDate As String = getEscdate(LRentESC, AlreadyEistData.Rows(0).Item("CAmEscDate").ToString())

                                                                        'Dim ESCFinaldateARR As String() = EscalationDate.Split("/")
                                                                        'EscalationDate = ESCFinaldateARR(1) + "/" + ESCFinaldateARR(0) + "/" + ESCFinaldateARR(2)
                                                                        Dim ESCdt As Date = Convert.ToDateTime(EscalationDate)
                                                                        Dim ESCInvGenDate As String = ESCdt.ToString("MM/dd/yy")
                                                                        Dim ESCInvGenDateStr As String() = ESCInvGenDate.Split("/")
                                                                        Dim ESCLInvGenDateStr As String = ESCInvGenDateStr(1) + "/" + ESCInvGenDateStr(0) + "/" + ESCInvGenDateStr(2)

                                                                        'Calculating the Actual lease start date
                                                                        Dim InvCreationDtarr = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString().Split("/")
                                                                        Dim InvCreationLRIGDTdate1 As New Date(InvCreationDtarr(2), InvCreationDtarr(1), InvCreationDtarr(0))
                                                                        InvCreationLRIGDTdate1 = CDate(InvCreationLRIGDTdate1)
                                                                        Dim InvGenDt As DateTime = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("MM/dd/yy"))


                                                                        Dim InvGenDate As String = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("dd/MM/yy"))
                                                                        Dim InvGenDateStr As String() = InvGenDate.Split("/")
                                                                        Dim LInvGenDateStr As String = InvGenDateStr(1) + "/" + InvGenDateStr(0) + "/" + InvGenDateStr(2)

                                                                        Dim LCAMAmount As Double = 0
                                                                        LCAMAmount = LCAMAmt


                                                                        'Date.Now
                                                                        Dim ResultStr = ParseDateFn(LCAMRentCycletype, InvGenDt, LCAMAmount)
                                                                        Dim PDFnResultStr = ResultStr.Split("=")
                                                                        FinalInvDate = PDFnResultStr(0)
                                                                        MGAmtDT = PDFnResultStr(1)
                                                                        'MGAmtDT = (MGAmtDT * LPropershare) / 100

                                                                        insStr = ""
                                                                        ' RentyearInTheFuture
                                                                        'InvGenDt
                                                                        'date.Now() 

                                                                        Dim Result As String() = ParseEscDateFn(LCAMRentCycletype, ESCGenDt, InvGenDt, LCAMAmount).Split("=")

                                                                        Dim TotalDays As String = ""
                                                                        Dim BAMnt As Double = 0
                                                                        Dim ADays As Int16 = 0
                                                                        Dim BDays As Int16 = 0
                                                                        Dim NInvdtdt As String = ""

                                                                        NInvdtdt = Result(0)

                                                                        BDays = Result(1)
                                                                        ADays = Result(2)
                                                                        BAMnt = Result(3)
                                                                        TotalDays = Result(4)

                                                                        If ((CDate(LEdt1) >= InvGenDt) And (CDate(NInvdtdt) > CDate(LEdt1))) Then
                                                                            If names(f).ToUpper() = ("CAM").ToUpper() Then
                                                                                insStr = ""

                                                                                Dim LeaseEndDate As String = LEdt1
                                                                                Dim FinaldateARR As String() = LeaseEndDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = CDate(FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)).ToString("dd/MM/yy")



                                                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                End If
                                                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Transaction = tran
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If
                                                                                res = da.SelectCommand.ExecuteScalar()

                                                                                If res <> 0 Then


                                                                                    Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='Next CAM Invoice date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowRCDF.Length > 0 Then
                                                                                        For Each CField As DataRow In rowRCDF

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()

                                                                                        Next
                                                                                    End If
                                                                                    Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Escalation Date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In rowECD
                                                                                            If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & EscalationDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Else
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("CAmEscDate").ToString() & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            End If


                                                                                        Next
                                                                                    End If
                                                                                    Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Amount'  and Datatype='Numeric' and Documenttype='CAM'")
                                                                                    If rowMGAmt.Length > 0 Then
                                                                                        For Each CField As DataRow In rowMGAmt
                                                                                            MGAmtDT = BAMnt
                                                                                            MGAmtDT = (MGAmtDT * LPropershare) / 100
                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                        Next
                                                                                    End If

                                                                                    Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)
                                                                                    ''INSERT INTO HISTORY 
                                                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                    Try
                                                                                        UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                        da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                        Dim srerd As String = String.Empty
                                                                                    Catch ex As Exception

                                                                                    End Try
                                                                                    tran.Commit()

                                                                                    'Non transactional Query
                                                                                    'Check Work Flow
                                                                                    ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Parameters.Clear()
                                                                                    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                    Dim dt1 As New DataTable
                                                                                    da.Fill(dt1)
                                                                                    If dt1.Rows.Count = 1 Then
                                                                                        If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                            ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                        End If
                                                                                    End If

                                                                                End If


                                                                            End If

                                                                        ElseIf ((CDate(ESCGenDt) > InvGenDt) And (CDate(NInvdtdt) > CDate(ESCGenDt))) Then
                                                                            If names(f).ToUpper() = ("CAM").ToUpper() Then
                                                                                'If LRenttype = 1554651 Then 'Fixed
                                                                                'If con.State = ConnectionState.Closed Then
                                                                                '    con.Open()
                                                                                'End If
                                                                                Finaldate = ""
                                                                                Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)
                                                                                'tran = con.BeginTransaction()
                                                                                'tran = con.BeginTransaction()
                                                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"

                                                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                End If
                                                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Transaction = tran
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If
                                                                                res = da.SelectCommand.ExecuteScalar()

                                                                                If res <> 0 Then

                                                                                    Dim AmountSum As Double = 0

                                                                                    'Calculating Rent Before  Esclation

                                                                                    Dim LessorsCAMAmount As Double = 0
                                                                                    Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Amount'  and Datatype='Numeric' and Documenttype='CAM'")
                                                                                    If rowMGAmt.Length > 0 Then
                                                                                        For Each CField As DataRow In rowMGAmt

                                                                                            LessorsCAMAmount = (BAMnt * LPropershare) / 100
                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsCAMAmount & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                        Next
                                                                                    End If


                                                                                    Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='NEXT CAM Invoice date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowRCDF.Length > 0 Then
                                                                                        For Each CField As DataRow In rowRCDF

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()

                                                                                        Next
                                                                                    End If
                                                                                    Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Escalation Date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In rowECD
                                                                                            If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & EscalationDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Else
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("CAmEscDate").ToString() & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            End If


                                                                                        Next
                                                                                    End If

                                                                                    Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                    ''INSERT INTO HISTORY 
                                                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                    Try
                                                                                        UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                        da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                        Dim srerd As String = String.Empty
                                                                                    Catch ex As Exception

                                                                                    End Try

                                                                                    tran.Commit()

                                                                                    'Check Work Flow
                                                                                    ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Parameters.Clear()
                                                                                    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                    Dim dt1 As New DataTable
                                                                                    da.Fill(dt1)
                                                                                    If dt1.Rows.Count = 1 Then
                                                                                        If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                            ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                        End If
                                                                                    End If
                                                                                End If
                                                                                res = 0
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If

                                                                                tran = con.BeginTransaction()
                                                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                End If
                                                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Transaction = tran
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If
                                                                                res = da.SelectCommand.ExecuteScalar()

                                                                                If res <> 0 Then
                                                                                    ' Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)

                                                                                    Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='Next CAM Invoice date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowRCDF.Length > 0 Then
                                                                                        For Each CField As DataRow In rowRCDF

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()

                                                                                        Next
                                                                                    End If
                                                                                    Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Escalation Date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In rowECD
                                                                                            If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & EscalationDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Else
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("CAmEscDate").ToString() & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            End If


                                                                                        Next
                                                                                    End If

                                                                                    Dim AmountSum As Double = 0
                                                                                    'Calculating Rent After  Esclation
                                                                                    Dim MGAmtDtable As New DataTable()
                                                                                    Dim Amount As Double = 0

                                                                                    AmountSum = (LCAMAmount * LCAMEscPtage) / 100
                                                                                    AmountSum = LCAMAmount + AmountSum
                                                                                    If LCAMRentCycletype = "1554654" Then '"Monthly"
                                                                                        AmountSum = AmountSum
                                                                                    ElseIf LCAMRentCycletype = "1554655" Then '"Quarterly"
                                                                                        AmountSum = AmountSum * 3

                                                                                    ElseIf LCAMRentCycletype = "1554656" Then 'Half Yearly
                                                                                        AmountSum = AmountSum * 6

                                                                                    End If

                                                                                    Dim LessorsCAMAmount As Double = 0
                                                                                    Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Amount'  and Datatype='Numeric' and Documenttype='CAM'")
                                                                                    If rowMGAmt.Length > 0 Then
                                                                                        For Each CField As DataRow In rowMGAmt

                                                                                            Amount = AmountSum / TotalDays
                                                                                            Amount = Amount * ADays
                                                                                            LessorsCAMAmount = (Amount * LPropershare) / 100
                                                                                            Dim BdecimalVar As Decimal
                                                                                            BdecimalVar = Decimal.Round(LessorsCAMAmount, 2, MidpointRounding.AwayFromZero)
                                                                                            BdecimalVar = Math.Round(BdecimalVar, 2)
                                                                                            LessorsCAMAmount = BdecimalVar
                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsCAMAmount & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                            AmountSum = 0
                                                                                            AmountSum = (LCAMAmount * LCAMEscPtage) / 100
                                                                                            AmountSum = LCAMAmount + AmountSum
                                                                                            LessorsCAMAmount = 0
                                                                                            LessorsCAMAmount = (AmountSum * LPropershare) / 100
                                                                                        Next
                                                                                    End If


                                                                                    Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                                                    ''INSERT INTO HISTORY 
                                                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)
                                                                                    Call CAMEsclationLeaseMasterUpdate(EID, ds.Tables("AutoInvDocData").Rows(j).Item("TID"), LessorsCAMAmount, Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")), con, tran)
                                                                                    Try
                                                                                        UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                        da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                        Dim srerd As String = String.Empty
                                                                                    Catch ex As Exception

                                                                                    End Try

                                                                                    tran.Commit()

                                                                                    'Check Work Flow
                                                                                    ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Parameters.Clear()
                                                                                    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                    Dim dt1 As New DataTable
                                                                                    da.Fill(dt1)
                                                                                    If dt1.Rows.Count = 1 Then
                                                                                        If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                            ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                        End If
                                                                                    End If

                                                                                End If

                                                                            End If


                                                                        ElseIf CDate(InvGenDt) = Date.Now Then ' Date.Now Then

                                                                            If names(f).ToUpper() = ("CAM").ToUpper() Then


                                                                                'If LRenttype = 1554651 Then 'Fixed
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If

                                                                                insStr = ""

                                                                                Finaldate = ""
                                                                                Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                                                                Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                                                                Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                                                                Dim FinalDateStr As String() = Finaldate.Split("/")
                                                                                Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)

                                                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,gatdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CreatedOn,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                                                End If
                                                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                da.SelectCommand.Transaction = tran
                                                                                If con.State = ConnectionState.Closed Then
                                                                                    con.Open()
                                                                                End If
                                                                                res = da.SelectCommand.ExecuteScalar()

                                                                                If res <> 0 Then

                                                                                    Dim rowRCDF As DataRow() = ds.Tables("fields").Select("DisplayName='NEXT CAM Invoice date' and Datatype='Datetime' and Documenttype='CAM'")
                                                                                    If rowRCDF.Length > 0 Then
                                                                                        For Each CField As DataRow In rowRCDF

                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LFinalDateStr & "'  where tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()

                                                                                        Next
                                                                                    End If
                                                                                    Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Amount'  and Datatype='Numeric' and Documenttype='CAM'")
                                                                                    If rowMGAmt.Length > 0 Then
                                                                                        For Each CField As DataRow In rowMGAmt
                                                                                            MGAmtDT = (MGAmtDT * LPropershare) / 100
                                                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & MGAmtDT & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                                            da.SelectCommand.CommandText = upquery
                                                                                            da.SelectCommand.Transaction = tran
                                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                                            da.SelectCommand.ExecuteNonQuery()
                                                                                        Next
                                                                                    End If

                                                                                    Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Escalation Date' and Datatype='Datetime' and Documenttype='Cam'")
                                                                                    If rowECD.Length > 0 Then
                                                                                        For Each CField As DataRow In rowECD
                                                                                            If CDate(Finaldate).Year = ESCdt.Year Then
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & EscalationDate & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            Else
                                                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("CAmEscDate").ToString() & "'  where tid =" & res & ""
                                                                                                da.SelectCommand.CommandText = upquery
                                                                                                da.SelectCommand.Transaction = tran
                                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                                da.SelectCommand.ExecuteNonQuery()

                                                                                            End If


                                                                                        Next
                                                                                    End If
                                                                                    Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)
                                                                                    ''INSERT INTO HISTORY 
                                                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                                                    Try
                                                                                        UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                                                        da.SelectCommand.CommandText = UpdStr.ToString()
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        da.SelectCommand.ExecuteNonQuery()
                                                                                        Dim srerd As String = String.Empty
                                                                                    Catch ex As Exception

                                                                                    End Try
                                                                                    tran.Commit()
                                                                                    'Dim ob1 As New DMSUtil()
                                                                                    ' Dim res1 As String = String.Empty
                                                                                    ' Dim ob As New DynamicForm
                                                                                    'Non transactional Query
                                                                                    'Check Work Flow
                                                                                    ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                                    da.SelectCommand.Parameters.Clear()
                                                                                    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                                                    Dim dt1 As New DataTable
                                                                                    da.Fill(dt1)
                                                                                    If dt1.Rows.Count = 1 Then
                                                                                        If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                                            ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                                                        End If
                                                                                    End If

                                                                                End If

                                                                            End If
                                                                        Else
                                                                            tran.Commit()
                                                                        End If

                                                                    End If

                                                                End If
                                                            Catch ex As Exception
                                                                AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "CAM AutoInvoice Function Exception:" + ex.Message.ToString(), 181)

                                                            End Try
                                                        End If
                                                    End If
                                                Next

                                            Catch ex As Exception
                                                AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "AutoInvoice Function Exception:" + ex.Message.ToString(), 181)
                                                If Not tran Is Nothing Then
                                                    tran.Rollback()
                                                End If
                                            End Try
                                        Next
                                    End If

                                Next
                            End If
                        End If


                    End If
                Next
            End If
            AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "Calling AutoInvoice Function ENd Now 1", 181)
        Catch ex As Exception
            AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "AutoInvoice Function Exception:" + ex.Message.ToString(), 181)
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
            End If
        End Try

    End Sub

    Public Sub RentEsclationLeaseMasterUpdate(ByVal EID As String, ByVal MasterTid As Int64, ByVal MGAmount As Double, ByVal LMGAmount As Double, ByVal Documenttype As String, con As SqlConnection, tran As SqlTransaction)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim ds As New DataSet()
        Dim FieldData As New DataTable
        If FieldData.Rows.Count > 0 Then
            FieldData.Clear()
        End If
        da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(Documenttype) & "' order by displayOrder", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.Transaction = tran

        da.Fill(ds, "fields")
        FieldData = ds.Tables("fields")
        Dim MGAmountfld As String = ""
        Dim LMGAmountfld As String = ""
        Try
            If MasterTid <> 0 Then
                ''INSERT INTO HISTORY 
                Dim ob As New DynamicForm
                ob.HistoryT(EID, MasterTid, "30200", Documenttype, "MMM_MST_Master", "ADD", con, tran)
                Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Total MG Amount'  and Datatype='Numeric' ")
                If rowMGAmt.Length > 0 Then
                    For Each CField As DataRow In rowMGAmt
                        MGAmountfld = CField.Item("FieldMapping").ToString
                    Next
                End If
                Dim rowLMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric' ")
                If rowLMGAmt.Length > 0 Then
                    For Each CField As DataRow In rowLMGAmt
                        LMGAmountfld = CField.Item("FieldMapping").ToString
                    Next
                End If
                da.SelectCommand.CommandText = "Update MMM_MST_MASTER set " & MGAmountfld & "='" & MGAmount & "'," & LMGAmountfld & "='" & LMGAmount & "' where tid = " & MasterTid & ""
                da.SelectCommand.Transaction = tran
                da.SelectCommand.ExecuteNonQuery()
            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub SDEsclationLeaseMasterUpdate(ByVal MGAmountfld As String, ByVal LMGAmountfld As String, ByVal EID As String, ByVal MasterTid As Int64, ByVal MGAmount As Int64, ByVal LMGAmount As Int64, ByVal Documenttype As String, con As SqlConnection, tran As SqlTransaction)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim ds As New DataSet()
        Dim FieldData As New DataTable
        If FieldData.Rows.Count > 0 Then
            FieldData.Clear()
        End If
        da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(Documenttype) & "' order by displayOrder", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.Transaction = tran

        da.Fill(ds, "fields")
        FieldData = ds.Tables("fields")
        MGAmountfld = ""
        LMGAmountfld = ""
        Try
            If MasterTid <> 0 Then
                ''INSERT INTO HISTORY 
                Dim ob As New DynamicForm
                ob.HistoryT(EID, MasterTid, "30200", Documenttype, "MMM_MST_Master", "ADD", con, tran)
                Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Security Deposit (Lease)'  and Datatype='Numeric' ")
                If rowMGAmt.Length > 0 Then
                    For Each CField As DataRow In rowMGAmt
                        MGAmountfld = CField.Item("FieldMapping").ToString
                    Next
                End If
                Dim rowLMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='Lessors Security Deposit'  and Datatype='Numeric' ")
                If rowLMGAmt.Length > 0 Then
                    For Each CField As DataRow In rowLMGAmt
                        LMGAmountfld = CField.Item("FieldMapping").ToString
                    Next
                End If
                da.SelectCommand.CommandText = "Update MMM_MST_MASTER set " & MGAmountfld & "='" & MGAmount & "'," & LMGAmountfld & "='" & LMGAmount & "' where tid = " & MasterTid & ""
                da.SelectCommand.Transaction = tran
                da.SelectCommand.ExecuteNonQuery()
            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub CAMEsclationLeaseMasterUpdate(ByVal EID As String, ByVal MasterTid As Int64, ByVal MGAmount As Int64, ByVal Documenttype As String, con As SqlConnection, tran As SqlTransaction)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim ds As New DataSet()
        Dim FieldData As New DataTable
        If FieldData.Rows.Count > 0 Then
            FieldData.Clear()
        End If
        da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(Documenttype) & "' order by displayOrder", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.Transaction = tran

        da.Fill(ds, "fields")
        FieldData = ds.Tables("fields")
        Dim MGAmountfld As String = ""
        Try
            If MasterTid <> 0 Then
                ''INSERT INTO HISTORY 
                Dim ob As New DynamicForm
                ob.HistoryT(EID, MasterTid, "30200", Documenttype, "MMM_MST_Master", "ADD", con, tran)
                Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='CAM Amount'  and Datatype='Numeric' ")
                If rowMGAmt.Length > 0 Then
                    For Each CField As DataRow In rowMGAmt
                        MGAmountfld = CField.Item("FieldMapping").ToString
                    Next
                End If

                da.SelectCommand.CommandText = "Update MMM_MST_MASTER set " & MGAmountfld & "='" & MGAmount & "'  where tid = " & MasterTid & ""
                da.SelectCommand.Transaction = tran
                da.SelectCommand.ExecuteNonQuery()
            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub Recalculate_CalfieldsonSave(ByVal EID As Integer, ByVal docid As Integer, con As SqlConnection, tran As SqlTransaction)
        '''''''''For recalculation of calculative fields, if any ''''''''''''''''
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.Transaction = tran
        Dim dsscal As New DataSet
        Dim dt5 As New DataTable
        Dim dt6 As New DataTable

        Dim cal_mpng As String = ""
        Dim cal_text As String = ""
        Dim fldmpng As String = ""
        Dim stringf As String = ""
        oda.SelectCommand.CommandText = "Select documenttype from MMM_MST_DOC where tid = " & docid & ""
        oda.Fill(dsscal, "caldoc")
        Dim Dtype As String = ""
        Dtype = Convert.ToString(oda.SelectCommand.ExecuteScalar())

        oda.SelectCommand.CommandText = "Select cal_text,fieldmapping,isrecalculative from MMM_MST_FIELDS with (nolock) where documenttype ='" & Dtype & "' and fieldtype='Calculative Field' and eid='" & EID & "' and isactive=1"
        oda.Fill(dt5)
        If dt5.Rows.Count > 0 Then
            For n As Integer = 0 To dt5.Rows.Count - 1
                'Add Code if not required again calculation
                If dt5.Rows(n).Item("isrecalculative") = True Then
                    Dim orignlfinalstr As String = ""


                    cal_text = dt5.Rows(n).Item("cal_text")
                    cal_mpng = dt5.Rows(n).Item("fieldmapping")
                    dt6.Rows.Clear()
                    oda.SelectCommand.CommandText = "Select displayname,fieldmapping,isactive from MMM_MST_FIELDS with(nolock) where documenttype = '" & dsscal.Tables("caldoc").Rows(0).Item("documenttype").ToString() & "' and eid = " & EID & " "
                    oda.Fill(dt6)
                    stringf = cal_text
                    For m As Integer = 0 To dt6.Rows.Count - 1
                        '  If cal_text.Contains("{" & dt6.Rows(m).Item("displayname").ToString() & "}") Then
                        If cal_text.Trim().Contains("{" & dt6.Rows(m).Item("displayname").ToString().Trim() & "}") Then
                            fldmpng = dt6.Rows(m).Item("fieldmapping").ToString().Trim()
                            stringf = stringf.Replace("{" & dt6.Rows(m).Item("displayname").ToString().Trim() & "}", "{" & fldmpng.Trim() & "}")
                        End If
                    Next
                    stringf = stringf.Replace("{", "")
                    stringf = stringf.Replace("}", "")
                    Dim st As String() = Split(stringf, ",")
                    For k As Int16 = 0 To st.Length - 1
                        Dim dt7 As New DataTable
                        Dim finalstrr As String = ""
                        Dim s As String() = Split(st(k), "=")
                        If s(0).ToString.Length > 2 Then
                            Dim resultfldd As String = s(0)
                            orignlfinalstr = s(1)
                            If Right(orignlfinalstr, 1) = "," Then
                                orignlfinalstr = Left(orignlfinalstr, orignlfinalstr.Length - 1)
                            End If
                            Dim intval As String = ""
                            Dim spltstr() As String = orignlfinalstr.Split("(", ")", "+", "-", "*", "/")
                            For i As Integer = 0 To spltstr.Length - 1
                                If spltstr(i).Contains("fld") Then
                                    finalstrr = finalstrr & spltstr(i) & ","
                                Else
                                    intval = intval & spltstr(i) & ","
                                End If
                            Next
                            If Right(finalstrr, 1) = "," Then
                                finalstrr = Left(finalstrr, finalstrr.Length - 1)
                            End If
                            Dim value As String = ""
                            Dim Numericvalue As String = ""
                            Dim opr As String = ""
                            oda.SelectCommand.CommandText = "Select " & finalstrr & " from MMM_MST_DOC  where tid = " & docid & ""
                            oda.Fill(dt7)
                            Dim str As String = ""
                            Dim Temporignlfinalstr As String = orignlfinalstr
                            For h As Integer = 0 To dt7.Columns.Count - 1
                                For Each c As Char In orignlfinalstr
                                    str &= c
                                    If c = "(" Or c = ")" Then
                                        If orignlfinalstr.ToString.Contains("(") Or orignlfinalstr.ToString.Contains(")") Then
                                            value = value & c
                                            str = ""
                                        End If
                                    ElseIf str.Trim = dt7.Columns(h).ColumnName.ToString Then
                                        value &= IIf(String.IsNullOrEmpty(dt7.Rows(0).Item(dt7.Columns(h).ColumnName.ToString)), "0", dt7.Rows(0).Item(dt7.Columns(h).ColumnName.ToString))
                                        'Exit For
                                        'orignlfinalstr = orignlfinalstr.Replace(dt7.Columns(h).ColumnName.ToString, IIf(IsDBNull(dt7.Rows(0).Item(dt7.Columns(h).ColumnName)), "0", dt7.Rows(0).Item(dt7.Columns(h).ColumnName).ToString()))
                                    ElseIf c = "+" Or c = "-" Or c = "*" Or c = "/" Or c = "%" Then
                                        If String.IsNullOrEmpty(value) Then
                                            value = value & Numericvalue & c
                                            Numericvalue = ""
                                        Else
                                            value = value & c
                                        End If

                                        'orignlfinalstr = Left(orignlfinalstr, orignlfinalstr.Length - 1)
                                        Dim fld As String = str & c
                                        ' orignlfinalstr = Replace(orignlfinalstr, "(" & str.Trim, "")
                                        If orignlfinalstr.ToString.Contains("(") Or orignlfinalstr.ToString.Contains(")") Then
                                            orignlfinalstr = Replace(orignlfinalstr, "(" & str.Trim, "")
                                        Else
                                            orignlfinalstr = Replace(orignlfinalstr, str.Trim, "")
                                        End If
                                        str = ""
                                        'orignlfinalstr = Right(orignlfinalstr, orignlfinalstr.Length - 1)
                                        If h < dt7.Columns.Count - 1 Then
                                            Exit For
                                        End If
                                    Else
                                        If Temporignlfinalstr.Length <> orignlfinalstr.Length Then
                                            If IsNumeric(str) Then
                                                value = value & c
                                            End If
                                            'ElseIf c = "(" Or c = ")" Then
                                            '    value = value & c
                                        Else
                                            If IsNumeric(str) Then
                                                Numericvalue = Numericvalue & c
                                            End If
                                        End If
                                    End If
                                Next
                                'orignlfinalstr = orignlfinalstr.Replace(dt7.Columns(h).ColumnName.ToString, "")
                                'orignlfinalstr = Right(orignlfinalstr, orignlfinalstr.Length - 1)
                            Next
                            If Val(orignlfinalstr.Trim) <> 0 Then
                                value = value & Val(orignlfinalstr.Trim)  '' removed on 01_jan_15
                            End If
                            Dim res = New DataTable().Compute(value, 0).ToString()
                            Dim decimalVar As Decimal
                            decimalVar = Decimal.Round(res, 2, MidpointRounding.AwayFromZero)
                            decimalVar = Math.Round(decimalVar, 2)
                            'Dim res = New DataTable().Compute(orignlfinalstr, 0).ToString()


                            oda.SelectCommand.CommandText = "Update MMM_MST_DOC set " & resultfldd & "='" & decimalVar.ToString() & "' where tid = " & docid & ""
                            oda.SelectCommand.ExecuteNonQuery()
                            dt7.Rows.Clear()
                            dt7.Dispose()
                        End If
                    Next
                End If
            Next

        End If
    End Sub

    Public Function getEscdate(ByVal LRentESC As String, ByVal PEscdate As String) As String
        Dim LRIGDTarr = PEscdate.Split("/")
        Dim LRIGDTdate1 As New Date(LRIGDTarr(2), LRIGDTarr(1), LRIGDTarr(0))
        LRIGDTdate1 = CDate(LRIGDTdate1)
        Dim LRIGDTdt As Date
        Dim LRIGDTdt1 As Date
        Dim LRInvGDTdt1 As DateTime = Convert.ToDateTime(LRIGDTdate1.ToString("MM/dd/yy"))
        Dim LRInvGDTdt2 As Date = Date.Today
        LRInvGDTdt2 = Convert.ToDateTime(LRInvGDTdt2.ToString("MM/dd/yy"))

        Dim yearInTheFuture As Date
        Dim TFyearInTheFuture As Boolean = False
        Dim RentyearInTheFuture As String = ""

        If Date.TryParse(LRInvGDTdt1, LRIGDTdt) And Date.TryParse(LRInvGDTdt1, LRIGDTdt1) Then
            Dim endDt As New Date(LRIGDTdt.Year, LRIGDTdt.Month, LRIGDTdt.Day)
            Dim startDt As New Date(LRIGDTdt1.Year, LRIGDTdt1.Month, LRIGDTdt1.Day)
            Dim startDtStr As String = String.Empty
            Dim startDtyear As Int16 = 0

            startDtStr = Convert.ToString(LRIGDTdt1.Month) + "/" + Convert.ToString(LRIGDTdt1.Day)
            startDtyear = LRIGDTdt1.Year
            Dim RentEscdtStr As String = String.Empty
            Dim RentEscyrdtStr As Int16 = 0

            If LRentESC = 1491593 Then 'Annual 
                'calculating the RentEsclation 
                yearInTheFuture = LRIGDTdt.AddYears(1)
                RentEscdtStr = Convert.ToString(LRIGDTdt.Month) + "/" + Convert.ToString(LRIGDTdt.Day)
                RentEscyrdtStr = yearInTheFuture.Year
            ElseIf LRentESC = 1491594 Then 'Bi-Annual
                yearInTheFuture = LRIGDTdt.AddYears(2)
                RentEscdtStr = Convert.ToString(LRIGDTdt.Month) + "/" + Convert.ToString(LRIGDTdt.Day)
                RentEscyrdtStr = yearInTheFuture.Year
            ElseIf LRentESC = 1491595 Then 'Tri-Annual
                yearInTheFuture = LRIGDTdt.AddYears(3)
                RentEscdtStr = Convert.ToString(LRIGDTdt.Month) + "/" + Convert.ToString(LRIGDTdt.Day)
                RentEscyrdtStr = yearInTheFuture.Year
            End If

            If startDt = endDt Then
                Dim LRentPaymenttest As String = String.Empty
            End If
            If RentEscdtStr = startDtStr And startDtyear <> RentEscyrdtStr Then
                RentyearInTheFuture = yearInTheFuture
                TFyearInTheFuture = True
            End If
            Return RentyearInTheFuture
        End If

    End Function
    Public Function ParseEscDateFn(ByVal LRentPayment As String, ByVal date1 As Date, ByVal date2 As Date, ByVal MGAmount As Double) As String

        'date2 as InvGenDt
        'Date1 as EscDt
        Dim Result As String = String.Empty
        Dim FinalInvDate As Date
        Dim MGAmtDT As Double = 0
        Dim AMGAmtDT As Double = 0
        Dim BMGAmtDT As Double = 0
        Dim dat As Date
        Dim dat1 As Date
        Dim dat2 As Date
        Dim totaldayscnt As Int64 = 0
        Dim ESCFinalInvDate As String = ""
        Dim AESCFinalInvDate As String = ""
        Dim AESCTotalDays As String = ""
        Dim BESCTotalDays As String = ""
        Dim BESCdss As String = ""
        If Date.TryParse(date2, dat) And Date.TryParse(date1, dat2) Then

            If LRentPayment = "1554654" Then '"Monthly"
                If Date.TryParse(date2, dat) Then
                    Dim startDt As New Date(dat.Year, dat.Month, 1)
                    Dim endDt As New Date(dat.Year, dat.Month, Date.DaysInMonth(dat.Year, dat.Month))
                    Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                    Dim Days As Int32 = endDt.Subtract(date2).Days + 1
                    Dim nexMonth = startDt.AddMonths(1)
                    Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)


                    FinalInvDate = StartDate


                End If

            ElseIf LRentPayment = "1554655" Then 'Quaterly
                Dim dt As Date
                If Date.TryParse(date2, dat) Then
                    Dim newDate As Date = date2.AddMonths(2)
                    If Date.TryParse(newDate, dt) Then
                        Dim startDt As New Date(dat.Year, dat.Month, 1)
                        Dim endDt As New Date(dt.Year, dt.Month, Date.DaysInMonth(dt.Year, dt.Month))
                        Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                        Dim Days As Int32 = endDt.Subtract(date2).Days + 1
                        Dim nexMonth = endDt.AddMonths(1)
                        Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)
                        FinalInvDate = StartDate

                    End If

                End If

            ElseIf LRentPayment = "1554656" Then 'Half Yearly
                Dim dt As Date
                If Date.TryParse(date2, dat) Then
                    Dim newDate As Date = date2.AddMonths(5)
                    If Date.TryParse(newDate, dt) Then
                        Dim startDt As New Date(dat.Year, dat.Month, 1)
                        Dim endDt As New Date(dt.Year, dt.Month, Date.DaysInMonth(dt.Year, dt.Month))
                        Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                        Dim Days As Int32 = endDt.Subtract(date2).Days + 1
                        Dim nexMonth = endDt.AddMonths(1)
                        Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)
                        FinalInvDate = StartDate

                    End If

                End If
            End If


            'calculating After days from escdate 
            Dim BESCstartDt As New Date(dat2.Year, dat2.Month, dat2.Day)
            Dim ESCendDt As New Date(dat2.Year, dat2.Month + 1, 1)
            Dim ESClastDay As Date = New Date(dat2.Year, dat2.Month, 1)
            Dim ESCdiff2 As Int64 = (ESCendDt - BESCstartDt).TotalDays.ToString()
            Dim ESCdifft2 As String = ESClastDay.AddMonths(1).AddDays(-1)
            If Date.TryParse(ESCdifft2, dat2) Then

                Dim dt As Date = CDate(FinalInvDate)
                ' lastDay = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                Dim endDate As New Date(dt.Year, dt.Month, 1)
                endDate = endDate.AddDays(-1)
                Dim tss As TimeSpan
                If endDate > BESCstartDt Then
                    tss = (endDate.Subtract(BESCstartDt))
                Else
                    tss = BESCstartDt.Subtract(endDate)
                End If

                If Convert.ToInt32(tss.Days) >= 0 Then
                    AESCTotalDays = tss.Days + 1
                End If
            End If
            'calculating Before days from escdate 
            If Date.TryParse(date1, dat2) Then
                Dim AESCstartDt As New Date(dat.Year, dat.Month, 1)
                Dim AESCendDt As New Date(dat2.Year, dat2.Month, dat2.Day)
                Dim AESCdiff2 As Int64 = (AESCendDt - AESCstartDt).TotalDays.ToString()

                Dim endDate As New Date(dat2.Year, dat2.Month, dat2.Day)
                ' Dim tss As TimeSpan = endDate.Subtract(AESCstartDt)
                Dim tss As TimeSpan
                If endDate > AESCstartDt Then
                    tss = endDate.Subtract(AESCstartDt)
                Else
                    tss = AESCstartDt.Subtract(endDate)
                End If
                If Convert.ToInt32(tss.Days) >= 0 Then
                    BESCTotalDays = tss.Days
                End If
            End If
            ' calculating Before ESC Rent Amount
            'AMGAmtDT
            'BMGAmtDT
            Dim FinalstartDt As New Date(dat.Year, dat.Month, 1)
            totaldayscnt = (FinalInvDate - FinalstartDt).TotalDays.ToString()

            If LRentPayment = "1554654" Then '"Monthly"
                MGAmount = MGAmount
            ElseIf LRentPayment = "1554655" Then '"Quarterly"
                MGAmount = MGAmount * 3

            ElseIf LRentPayment = "1554656" Then 'Half Yearly
                MGAmount = MGAmount * 6

            End If
            If BESCTotalDays > 0 Then
                MGAmtDT = MGAmount
                MGAmtDT = MGAmtDT / totaldayscnt
                BMGAmtDT = MGAmtDT * BESCTotalDays

            End If

        End If
        Dim BdecimalVar As Decimal
        BdecimalVar = Decimal.Round(BMGAmtDT, 2, MidpointRounding.AwayFromZero)
        BdecimalVar = Math.Round(BdecimalVar, 2)

        Result = Convert.ToString(FinalInvDate) + "=" + Convert.ToString(BESCTotalDays) + "=" + Convert.ToString(AESCTotalDays) + "=" + Convert.ToString(BdecimalVar) + "=" + Convert.ToString(totaldayscnt)

        Return Result

    End Function


    Public Function ParseDateFn(ByVal LRentPayment As String, ByVal date2 As Date, ByVal MGAmount As Double) As String


        Dim Result As String = String.Empty
        Dim FinalInvDate As String = ""
        Dim MGAmtDT As Double = 0
        Dim dat As Date

        If Date.TryParse(date2, dat) Then

            If LRentPayment = "1554654" Then '"Monthly"
                If Date.TryParse(date2, dat) Then
                    Dim startDt As New Date(dat.Year, dat.Month, 1)
                    Dim endDt As New Date(dat.Year, dat.Month, Date.DaysInMonth(dat.Year, dat.Month))
                    Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                    Dim Days As Int32 = endDt.Subtract(date2).Days + 1
                    Dim nexMonth = startDt.AddMonths(1)
                    Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)


                    FinalInvDate = StartDate
                    ' calculating rent Amount
                    If Days < DaysStayed Then
                        MGAmtDT = MGAmount
                        MGAmtDT = MGAmtDT / DaysStayed
                        MGAmtDT = MGAmtDT * Days
                    ElseIf Days = DaysStayed Then
                        MGAmtDT = MGAmount
                    End If

                End If

            ElseIf LRentPayment = "1554655" Then 'Quaterly
                Dim dt As Date
                If Date.TryParse(date2, dat) Then
                    Dim newDate As Date = date2.AddMonths(2)
                    If Date.TryParse(newDate, dt) Then
                        Dim startDt As New Date(dat.Year, dat.Month, 1)
                        Dim endDt As New Date(dt.Year, dt.Month, Date.DaysInMonth(dt.Year, dt.Month))
                        Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                        Dim Days As Int32 = endDt.Subtract(date2).Days + 1
                        Dim nexMonth = endDt.AddMonths(1)
                        Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)
                        FinalInvDate = StartDate
                        ' calculating rent Amount
                        If Days < DaysStayed Then
                            MGAmtDT = MGAmount
                            MGAmtDT = MGAmtDT / DaysStayed
                            MGAmtDT = MGAmtDT * Days
                        ElseIf Days = DaysStayed Then
                            MGAmtDT = MGAmount
                        End If
                    End If

                End If

            ElseIf LRentPayment = "1554656" Then 'Half Yearly
                Dim dt As Date
                If Date.TryParse(date2, dat) Then
                    Dim newDate As Date = date2.AddMonths(5)
                    If Date.TryParse(newDate, dt) Then
                        Dim startDt As New Date(dat.Year, dat.Month, 1)
                        Dim endDt As New Date(dt.Year, dt.Month, Date.DaysInMonth(dt.Year, dt.Month))
                        Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                        Dim Days As Int32 = endDt.Subtract(date2).Days + 1
                        Dim nexMonth = endDt.AddMonths(1)
                        Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)
                        FinalInvDate = StartDate
                        ' calculating rent Amount
                        If Days < DaysStayed Then
                            MGAmtDT = MGAmount
                            MGAmtDT = MGAmtDT / DaysStayed
                            MGAmtDT = MGAmtDT * Days
                        ElseIf Days = DaysStayed Then
                            MGAmtDT = MGAmount
                        End If
                    End If

                End If
            End If


        End If

        Dim decimalVar As Decimal
        decimalVar = Decimal.Round(MGAmtDT, 2, MidpointRounding.AwayFromZero)
        decimalVar = Math.Round(decimalVar, 2)
        Result = Convert.ToString(FinalInvDate) + "=" + Convert.ToString(decimalVar)

        Return Result

    End Function
    Public Function EsclationFn(ByVal LRentPayment As Int64, ByVal date2 As Date, ByVal MGAmount As Int64) As String

        Dim Result As String = String.Empty
        Dim FinalInvDate As String = ""
        Dim MGAmtDT As Double = 0
        Dim dat As Date
        Dim dat1 As Date

        If Date.TryParse(date2, dat) Then

            If LRentPayment = 1491593 Then 'Annual
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                dtss = DateAdd("M", 1, startDt)
                If Date.TryParse(dtss, DTS) Then
                    If DTS.Year <> dat.Year Then

                        Dim endDt As New Date(DTS.Year, DTS.Month, 1)
                        Dim lastDay As DateTime = New DateTime(DTS.Year, DTS.Month, DTS.Day)
                        Dim difft2 As DateTime = lastDay.AddMonths(0).AddDays(-1)
                        Dim diff2 As Int64 = (difft2 - startDt).TotalDays.ToString()
                        If Date.TryParse(difft2, DTS) Then
                            Dim endDate As New Date(DTS.Year, DTS.Month, DTS.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(DTS.Year, DTS.Month + 1, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If diff2 < dss Then
                            MGAmtDT = MGAmount
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * diff2
                        ElseIf diff2 = dss Then
                            MGAmtDT = MGAmount
                        End If
                    End If
                End If


            ElseIf LRentPayment = "Quaterly" Then
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                dtss = DateAdd("M", 3, startDt)
                If Date.TryParse(dtss, DTS) Then
                    If DTS.Year = dat.Year Then
                        Dim endDt As New Date(dat.Year, dat.Month + 3, 1)
                        Dim lastDay As DateTime = New DateTime(dat.Year, dat.Month + 2, 1)
                        Dim diff2 As Int64 = (endDt - startDt).TotalDays.ToString()
                        Dim difft2 As String = lastDay.AddMonths(1).AddDays(-1)
                        If Date.TryParse(difft2, dat1) Then
                            Dim endDate As New Date(dat1.Year, dat1.Month, dat1.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(dat.Year, dat.Month + 3, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If (diff2 - 1) < dss Then
                            MGAmtDT = MGAmount * 3
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * (diff2 - 1)
                        ElseIf (diff2 - 1) = dss Then
                            MGAmtDT = MGAmount * 3
                        End If
                    Else
                        Dim endDt As New Date(DTS.Year, DTS.Month, 1)
                        Dim lastDay As DateTime = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                        Dim difft2 As DateTime = lastDay.AddMonths(1).AddDays(-1)
                        Dim diff2 As Int64 = (difft2 - startDt).TotalDays.ToString()
                        If Date.TryParse(difft2, DTS) Then
                            Dim endDate As New Date(DTS.Year, DTS.Month, DTS.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(DTS.Year, DTS.Month + 1, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If diff2 < dss Then
                            MGAmtDT = MGAmount * 3
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * diff2
                        ElseIf diff2 = dss Then
                            MGAmtDT = MGAmount * 3
                        End If
                    End If
                End If

            ElseIf LRentPayment = "Half Yearly" Then
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                dtss = DateAdd("M", 6, startDt)
                If Date.TryParse(dtss, DTS) Then
                    If DTS.Year = dat.Year Then
                        Dim endDt As New Date(dat.Year, dat.Month + 6, 1)
                        Dim lastDay As DateTime = New DateTime(dat.Year, dat.Month + 5, 1)
                        Dim diff2 As Int64 = (endDt - startDt).TotalDays.ToString()
                        Dim difft2 As String = lastDay.AddMonths(1).AddDays(-1)
                        If Date.TryParse(difft2, dat1) Then
                            Dim endDate As New Date(dat1.Year, dat1.Month, dat1.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(dat.Year, dat.Month + 6, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If (diff2 - 1) < dss Then
                            MGAmtDT = MGAmount * 6
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * (diff2 - 1)
                        ElseIf (diff2 - 1) = dss Then
                            MGAmtDT = MGAmount * 6
                        End If
                    Else
                        Dim endDt As New Date(DTS.Year, DTS.Month, 1)
                        Dim lastDay As DateTime = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                        Dim difft2 As DateTime = lastDay.AddMonths(1).AddDays(-1)
                        Dim diff2 As Int64 = (difft2 - startDt).TotalDays.ToString()
                        If Date.TryParse(difft2, DTS) Then
                            Dim endDate As New Date(DTS.Year, DTS.Month, DTS.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(DTS.Year, DTS.Month + 1, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If diff2 < dss Then
                            MGAmtDT = MGAmount * 6
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * diff2
                        ElseIf diff2 = dss Then
                            MGAmtDT = MGAmount * 6
                        End If
                    End If
                End If
            End If


        End If

        Result = FinalInvDate + "=" + MGAmtDT

        Return Result
    End Function
    Public Sub CommonFunctionality(ByVal Documenttype As String, ByVal EID As Integer, ByVal Res As Integer, ByVal fields As DataTable, con As SqlConnection, tran As SqlTransaction)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran

        If Res <> 0 Then


            'Fieldtype='Auto Number'
            Dim row As DataRow() = fields.Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
            If row.Length > 0 Then
                For l As Integer = 0 To row.Length - 1
                    Select Case row(l).Item("fieldtype").ToString
                        Case "Auto Number"
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.Transaction = tran
                            da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
                            da.SelectCommand.Parameters.AddWithValue("docid", Res)
                            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
                            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                            da.SelectCommand.ExecuteScalar()
                            da.SelectCommand.Parameters.Clear()
                        Case "New Auto Number"
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.Transaction = tran
                            da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
                            da.SelectCommand.Parameters.AddWithValue("SearchFldid", row(l).Item("dropdown").ToString)
                            da.SelectCommand.Parameters.AddWithValue("docid", Res)
                            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
                            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                            da.SelectCommand.ExecuteScalar()
                            da.SelectCommand.Parameters.Clear()
                    End Select
                Next
            End If


            ' here is recalculate for main form   28 april 2020
            Call Recalculate_CalfieldsonSave(EID, Res, con, tran) 'fOR cALCULATIV fIELD   ' changes by balli  brings from above to down 

            'calculative fields
            Dim CalculativeField() As DataRow = fields.Select("Fieldtype='Formula Field'")
            Dim viewdoc As String = Convert.ToString(Documenttype)
            viewdoc = viewdoc.Replace(" ", "_")
            If CalculativeField.Length > 0 Then
                For Each CField As DataRow In CalculativeField
                    Dim formulaeditorr As New formulaEditor
                    Dim forvalue As String = String.Empty
                    'Coomented By Komal on 28March2014
                    forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), Res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
                    ' forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0)
                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & Res & ""
                    da.SelectCommand.CommandText = upquery
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.ExecuteNonQuery()
                Next


            End If

            Dim ob1 As New DMSUtil()
            ' Dim res1 As String = String.Empty
            Dim ob As New DynamicForm

            '' insert default first movement of document - by sunil
            da.SelectCommand.CommandText = "InsertDefaultMovement"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Transaction = tran
            da.SelectCommand.Parameters.AddWithValue("tid", Res)
            da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
            da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
            da.SelectCommand.ExecuteNonQuery()

            Dim res12 As String = String.Empty

            res12 = ob1.GetNextUserFromRolematrixT(Res, EID, "30200", "", "30200", con, tran)
            Dim sretMsgArr() As String = res12.Split(":")
            ob.HistoryT(EID, Res, "30200", Convert.ToString(Documenttype), "MMM_MST_DOC", "ADD", con, tran)
            If sretMsgArr(0) = "ARCHIVE" Then
                'Dim Op As New Exportdata()
                'Op.PushdataT(res, sretMsgArr(1), EID, con, tran)
            End If

            '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
            If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
                'this code block is added by ajeet kumar for transaction to be rolled back
                ''''tran.Rollback()
                ''''Exit Sub
            Else

            End If
            'Execute Trigger
            Try
                Dim FormName As String = Convert.ToString(Documenttype)
                '     Dim EID As Integer = 0
                If (Res > 0) And (FormName <> "") Then
                    Trigger.ExecuteTriggerT(FormName, EID, Res, con, tran)
                End If
            Catch ex As Exception
                Throw
            End Try


        End If
    End Sub
    Public Function ReportScheduler(ByVal tid As Integer) As Boolean
        Dim b As Boolean = False

        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
        Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
        Dim da As New SqlDataAdapter("select HH,MM,reporttype,TID,date from MMM_MST_ReportScheduler where tid=" & tid & " ", con)
        Dim dt As New System.Data.DataTable()
        da.Fill(dt)
        Dim SchType As String = dt.Rows(0).Item("reporttype").ToString()
        If ((UCase(SchType) = "DAILY") Or (UCase(SchType) = "AS ON DATE")) Then
            Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
            If x <= time2 And x >= time1 Then
                b = True
            End If
        End If
        If UCase(SchType) = "WEEKLY" Then
            Dim dayName As String = DateTime.Now.ToString("dddd")
            Dim currentweek As String = dt.Rows(0).Item("Date").ToString()
            If currentweek = 1 Then
                currentweek = "Monday"
            ElseIf currentweek = 2 Then
                currentweek = "Tuesday"
            ElseIf currentweek = 3 Then
                currentweek = "Wednesday"
            ElseIf currentweek = 4 Then
                currentweek = "Thursday"
            ElseIf currentweek = 5 Then
                currentweek = "Friday"
            ElseIf currentweek = 6 Then
                currentweek = "Saturday"
            ElseIf currentweek = 7 Then
                currentweek = "Sunday"
            End If
            If currentweek = dayName Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        If UCase(SchType) = "MONTHLY" Then
            Dim currentDate As DateTime = DateTime.Now
            Dim dateofMonth As Integer = currentDate.Day
            Dim dateMail As Integer = dt.Rows(0).Item("Date").ToString()
            If dateofMonth = dateMail Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        con.Close()
        con.Dispose()
        da.Dispose()
        dt.Dispose()
        Return b
    End Function

    'MG Sales invoice

    Public Sub MGSalesInv()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim insStr As String = String.Empty
        Dim UpdStr As String = String.Empty
        Dim res As Integer = 0
        Dim ob1 As New DMSUtil()
        Dim ob As New DynamicForm
        Dim tran As SqlTransaction = Nothing
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        Dim dtSFields As New DataTable
        Dim dtRFields As New DataTable
        Try
            'Sales Invoive Documenttype=
            Dim Documenttype As String = String.Empty
            'Rental Lease fields
            Dim LRentPayment As String = String.Empty
            Dim LRentFreedays As String = String.Empty
            Dim LRentInvGenDate As String = String.Empty
            Dim LRentInvAmt As Double = 0
            'sales Form Fields 
            Dim Lease_Doc_No As String = String.Empty
            Dim Store_Code As String = String.Empty
            Dim Rent_Period_From As String = String.Empty
            Dim Rent_Period_Till As String = String.Empty
            Dim Rent_Invoice_No As String = String.Empty
            Dim Department As String = String.Empty
            Dim Store_Name As String = String.Empty
            Dim SDOCNO As String = String.Empty
            Dim SStartDate As String = String.Empty
            Dim SEndDate As String = String.Empty
            Dim EID As Int16 = 181
            Dim SInvDate As String = String.Empty
            Dim SValue As Double = 0
            Dim STotalSale As Double = 0
            Dim SVarience As Double = 0
            Dim LTotalMGAmnt As Double = 0
            Dim SReveSharingptage As Int16 = 0
            Dim FieldData As New DataTable
            Dim Fieldstr As String = ""
            Fieldstr = "select concat(FieldMapping+' [',displayName+']') as Fields from MMM_MST_FIELDS with(nolock) where eid=181 and documenttype='Sales Form'"
            da = New SqlDataAdapter(Fieldstr, con)
            da.Fill(dtSFields)
            Dim strSFlds As String = ""
            strSFlds += String.Join(",", (From row In dtSFields.Rows Select CType(row.Item("Fields"), String)).ToArray)

            Dim FieldRstr As String = ""
            FieldRstr = "select concat(FieldMapping+' [',displayName+']') as Fields from MMM_MST_FIELDS with(nolock) where eid=181 and documenttype='Rental Invoice'"
            da = New SqlDataAdapter(FieldRstr, con)
            da.Fill(dtRFields)
            Dim strRFlds As String = ""
            strRFlds += String.Join(",", (From row In dtRFields.Rows Select CType(row.Item("Fields"), String)).ToArray)
            Documenttype = "MG Sales Invoice"
            da = New SqlDataAdapter("DECLARE @DATE DATETIME,@LDocNo as nvarchar(200) ;SET @DATE= getdate(); select Tid, dms.udf_split('DOCUMENT-MOU Lease Document-fld50',fld1) [LeaseDocNo]," & strSFlds & " from MMM_MST_DOC with(Nolock)  where   documenttype='Sales Form' and  adate between @DATE-DAY(@DATE) and EOMONTH(@DATE);", con)

            da.Fill(ds, "SalesInvDAta")

            Dim RentalInvData As New DataTable
            Dim RDOCIDData As New DataTable
            Dim MGDOCIDData As New DataTable

            If ds.Tables("SalesInvDAta").Rows.Count <> 0 Then
                For i As Integer = 0 To ds.Tables("SalesInvDAta").Rows.Count - 1


                    If ds.Tables("SalesInvDAta").Rows(i).Item("Rent type") = "Fixed and Revenue Sharing" Then

                        If RentalInvData.Rows.Count > 0 Then
                            RentalInvData.Clear()
                        End If
                        da.SelectCommand.CommandText = "Select tid,dms.udf_split('MASTER-Rent Payment Model-fld1',fld37)  as LRentPayment,fld20 as LeaseType, convert(varchar,CreatedOn,3) as CreatedOn," & strRFlds & "  from mmm_mst_doc with (nolock) where  eid=" & EID & "  and Documenttype='rental invoice' and fld2='" & ds.Tables("SalesInvDAta").Rows(i).Item("LeaseDocNo") & "' and fld20='" & ds.Tables("SalesInvDAta").Rows(i).Item("lease type") & "'   order by tid ASC"
                        da.SelectCommand.CommandType = CommandType.Text
                        da.Fill(ds, "RentalInvData")
                        RentalInvData = ds.Tables("RentalInvData")


                        If ds.Tables("RentalInvData").Rows.Count <> 0 Then


                            For j As Integer = 0 To ds.Tables("RentalInvData").Rows.Count - 1

                                If MGDOCIDData.Rows.Count > 0 Then
                                    MGDOCIDData.Clear()
                                End If
                                da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate(); select * from MMM_MST_DOC  with (nolock) where fld16='" & Convert.ToString(ds.Tables("RentalInvData").Rows(j).Item("Rental Invoice No")) & "' and documenttype='" & Convert.ToString(Documenttype) & "'  and  adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE);"

                                da.SelectCommand.CommandType = CommandType.Text
                                da.Fill(ds, "MGDOCIDData")
                                MGDOCIDData = ds.Tables("MGDOCIDData")
                                If ds.Tables("MGDOCIDData").Rows.Count = 0 Then
                                    If ds.Tables("RentalInvData").Rows(j).Item("LeaseType") = "1491591" Or ds.Tables("RentalInvData").Rows(j).Item("LeaseType") = "1554309" Or ds.Tables("RentalInvData").Rows(j).Item("LeaseType") = "1570941" Or ds.Tables("RentalInvData").Rows(j).Item("LeaseType") = "1570943" Then

                                        If RDOCIDData.Rows.Count > 0 Then
                                            RDOCIDData.Clear()
                                        End If

                                        da.SelectCommand.CommandText = "select * from MMM_MST_DOC  with (nolock) where RDOCID=" & ds.Tables("RentalInvData").Rows(j).Item("Tid") & " and documenttype='" & Convert.ToString(Documenttype) & "';"
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.Fill(ds, "RDOCIDData")
                                        RDOCIDData = ds.Tables("RDOCIDData")
                                        If ds.Tables("RDOCIDData").Rows.Count = 0 Then

                                            SStartDate = ds.Tables("SalesInvDAta").Rows(i).Item("Sales Period From")
                                            SEndDate = ds.Tables("SalesInvDAta").Rows(i).Item("Sales Period Till")
                                            SValue = ds.Tables("SalesInvDAta").Rows(i).Item("Revenue Share Amount")
                                            STotalSale = ds.Tables("SalesInvDAta").Rows(i).Item("Total Sales")
                                            Store_Code = ds.Tables("SalesInvDAta").Rows(i).Item("Store Code")
                                            SDOCNO = ds.Tables("SalesInvDAta").Rows(i).Item("Sales Form No")
                                            Store_Name = ds.Tables("SalesInvDAta").Rows(i).Item("Store Name")
                                            SReveSharingptage = ds.Tables("SalesInvDAta").Rows(i).Item("Revenue sharing Percentage")
                                            SVarience = 0
                                            Lease_Doc_No = ds.Tables("RentalInvData").Rows(j).Item("Lease Doc No")
                                            LRentPayment = ds.Tables("RentalInvData").Rows(j).Item("LRentPayment")
                                            LRentInvGenDate = ds.Tables("RentalInvData").Rows(j).Item("Next Invoice Creation Date")
                                            Dim RentInvCreationDt As String = String.Empty
                                            Dim LSharing As Double = 0
                                            RentInvCreationDt = ds.Tables("RentalInvData").Rows(j).Item("CreatedOn")
                                            If IsDBNull(ds.Tables("RentalInvData").Rows(j).Item("Lessors MG Amount")) = True Then
                                                LRentInvAmt = 0
                                            Else
                                                LRentInvAmt = Convert.ToDouble(ds.Tables("RentalInvData").Rows(j).Item("Lessors MG Amount"))
                                            End If

                                            LSharing = ds.Tables("RentalInvData").Rows(j).Item("Revenue Sharing Percentage")
                                            Rent_Period_From = SStartDate 'ds.Tables("RentalInvData").Rows(j).Item("Lease Start Date")
                                            Rent_Period_Till = SEndDate 'ds.Tables("RentalInvData").Rows(j).Item("Lease End Date")
                                            Department = ds.Tables("RentalInvData").Rows(j).Item("Department")
                                            Rent_Invoice_No = ds.Tables("RentalInvData").Rows(j).Item("Rental Invoice No")
                                            If IsDBNull(ds.Tables("RentalInvData").Rows(j).Item("Total MG Amount")) = True Then
                                                LTotalMGAmnt = 0
                                            Else
                                                LTotalMGAmnt = Convert.ToDouble(ds.Tables("RentalInvData").Rows(j).Item("Total MG Amount"))
                                            End If


                                            'calculating dates

                                            Dim Larr = SStartDate.Split("/")
                                            Dim Ldate1 As New Date(Larr(2), Larr(1), Larr(0))
                                            Ldate1 = CDate(Ldate1)
                                            Dim Larr1 = SEndDate.Split("/")
                                            Dim Ldate2 As New Date(Larr1(2), Larr1(1), Larr1(0))
                                            Ldate2 = CDate(Ldate2)
                                            Dim SDateS As Date
                                            Dim SDateE As Date
                                            Dim Ldt1 As DateTime = Convert.ToDateTime(Ldate1.ToString("MM/dd/yy"))
                                            Dim Ldt2 As DateTime = Convert.ToDateTime(Ldate2.ToString("MM/dd/yy"))


                                            Dim FinalInvDate As String = String.Empty

                                            Dim m As Integer
                                            'calculating Sales Invoice gen date

                                            Dim LRIGDTarr = RentInvCreationDt.Split("/") 'Creation date
                                            Dim LRIGDTdate1 As New Date(LRIGDTarr(2), LRIGDTarr(1), LRIGDTarr(0))
                                            LRIGDTdate1 = CDate(LRIGDTdate1)
                                            Dim LRInvGDTdt1 As DateTime = Convert.ToDateTime(LRIGDTdate1.ToString("MM/dd/yy"))
                                            Dim dat As Date

                                            Dim LRIGDTarrE = LRentInvGenDate.Split("/") ' Lent next gen date
                                            Dim LRIGDTdate1E As New Date(LRIGDTarrE(2), LRIGDTarrE(1), LRIGDTarrE(0))
                                            LRIGDTdate1E = CDate(LRIGDTdate1E)
                                            Dim LRInvGDTdt1E As DateTime = Convert.ToDateTime(LRIGDTdate1E.ToString("MM/dd/yy"))

                                            Dim LRIGDTdtE As Date
                                            Dim Result As Boolean = False
                                            If Date.TryParse(LRInvGDTdt1, dat) And Date.TryParse(LRInvGDTdt1E, LRIGDTdtE) And Date.TryParse(Ldt1, SDateS) And Date.TryParse(Ldt2, SDateE) Then


                                                If LRentPayment = "Monthly" Then
                                                    Dim sDayLast As String = ""
                                                    Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                                                    Dim endDt As New Date(LRIGDTdtE.Year, LRIGDTdtE.Month, 1)
                                                    Dim lastDay As Date = New Date(LRIGDTdtE.Year, LRIGDTdtE.Month, 1)

                                                    Dim oTimeSpan As New System.TimeSpan(1, 0, 0, 0)
                                                    Dim oDate As Date = endDt.Subtract(oTimeSpan)


                                                    Dim SstartDt As New Date(SDateS.Year, SDateS.Month, SDateS.Day)
                                                    Dim SendDt As New Date(SDateE.Year, SDateE.Month, SDateE.Day)

                                                    If SstartDt = startDt And SendDt = oDate Then
                                                        Result = True
                                                    Else
                                                        Result = False
                                                    End If

                                                    ' calculating months
                                                    m = DateDiff(DateInterval.Month, startDt, endDt) + 1


                                                ElseIf LRentPayment = "Quaterly" Then
                                                    Dim sDayLast As String = ""
                                                    Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                                                    Dim endDt As New Date(LRIGDTdtE.Year, LRIGDTdtE.Month, 1)
                                                    Dim lastDay As Date = New Date(LRIGDTdtE.Year, LRIGDTdtE.Month, 1)

                                                    Dim oTimeSpan As New System.TimeSpan(1, 0, 0, 0)
                                                    Dim oDate As Date = endDt.Subtract(oTimeSpan)


                                                    Dim SstartDt As New Date(SDateS.Year, SDateS.Month, SDateS.Day)
                                                    Dim SendDt As New Date(SDateE.Year, SDateE.Month, SDateE.Day)

                                                    If SstartDt = startDt And SendDt = oDate Then
                                                        Result = True
                                                    Else
                                                        Result = False
                                                    End If

                                                    ' calculating months
                                                    m = DateDiff(DateInterval.Month, startDt, endDt) + 1


                                                ElseIf LRentPayment = "Half Yearly" Then
                                                    Dim sDayLast As String = ""
                                                    Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                                                    Dim endDt As New Date(LRIGDTdtE.Year, LRIGDTdtE.Month, 1)
                                                    Dim lastDay As Date = New Date(LRIGDTdtE.Year, LRIGDTdtE.Month, 1)

                                                    Dim oTimeSpan As New System.TimeSpan(1, 0, 0, 0)
                                                    Dim oDate As Date = endDt.Subtract(oTimeSpan)


                                                    Dim SstartDt As New Date(SDateS.Year, SDateS.Month, SDateS.Day)
                                                    Dim SendDt As New Date(SDateE.Year, SDateE.Month, SDateE.Day)

                                                    If SstartDt = startDt And SendDt = oDate Then
                                                        Result = True
                                                    Else
                                                        Result = False
                                                    End If

                                                    ' calculating months
                                                    m = DateDiff(DateInterval.Month, startDt, endDt) + 1


                                                End If


                                            End If


                                            If Result = True Then
                                                If SValue > LTotalMGAmnt Then
                                                    'calculating Varience


                                                    SVarience = ((SValue - LTotalMGAmnt) * LSharing) / 100

                                                    Dim decimalVar As Decimal
                                                    decimalVar = Decimal.Round(SVarience, 2, MidpointRounding.AwayFromZero)
                                                    decimalVar = Math.Round(decimalVar, 2)

                                                    If SVarience <> 0 Then
                                                        If con.State = ConnectionState.Closed Then
                                                            con.Open()
                                                        End If
                                                        tran = con.BeginTransaction()
                                                        If FieldData.Rows.Count > 0 Then
                                                            FieldData.Clear()
                                                        End If
                                                        insStr = "insert into MMM_MST_DOC (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID,RDOCID,fld4,fld9,fld10,fld12,fld13,fld14,fld15,fld16,fld18,fld19,fld20,fld21,fld22,fld23,fld24,fld25) values ('" & Documenttype & "','" & EID & "',1,getdate(),getdate(),getdate(),'30200','" & ds.Tables("SalesInvDAta").Rows(i).Item("Tid") & "','" & Store_Code & "','" & Rent_Period_From & "','" & Rent_Period_Till & "','" & LTotalMGAmnt & "','" & SValue & "','" & decimalVar & "','" & Lease_Doc_No & "','" & Rent_Invoice_No & "','" & Department & "','" & Store_Name & "','" & SReveSharingptage & "','" & ds.Tables("RentalInvData").Rows(j).Item("Name of Lessor") & "','" & SDOCNO & "','" & STotalSale & "','" & LRentInvAmt & "','" & LSharing & "');"

                                                        da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                        da.SelectCommand.CommandType = CommandType.Text
                                                        da.SelectCommand.Transaction = tran
                                                        If con.State = ConnectionState.Closed Then
                                                            con.Open()
                                                        End If
                                                        res = da.SelectCommand.ExecuteScalar()


                                                        If res <> 0 Then
                                                            'Collecting the field data



                                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Documenttype & "' order by displayOrder", con)
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.SelectCommand.Transaction = tran
                                                            da.Fill(ds, "fields")
                                                            FieldData = ds.Tables("fields")

                                                            'Fieldtype='Auto Number'
                                                            Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
                                                            If row.Length > 0 Then
                                                                For l As Integer = 0 To row.Length - 1
                                                                    Select Case row(l).Item("fieldtype").ToString
                                                                        Case "Auto Number"
                                                                            da.SelectCommand.Parameters.Clear()
                                                                            da.SelectCommand.Transaction = tran
                                                                            da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                                                                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                                            da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
                                                                            da.SelectCommand.Parameters.AddWithValue("docid", res)
                                                                            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
                                                                            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                                                                            da.SelectCommand.ExecuteScalar()
                                                                            da.SelectCommand.Parameters.Clear()
                                                                        Case "New Auto Number"
                                                                            da.SelectCommand.Parameters.Clear()
                                                                            da.SelectCommand.Transaction = tran
                                                                            da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                                                                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                                            da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
                                                                            da.SelectCommand.Parameters.AddWithValue("SearchFldid", row(l).Item("dropdown").ToString)
                                                                            da.SelectCommand.Parameters.AddWithValue("docid", res)
                                                                            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
                                                                            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                                                                            da.SelectCommand.ExecuteScalar()
                                                                            da.SelectCommand.Parameters.Clear()
                                                                    End Select
                                                                Next
                                                            End If


                                                            ' here is recalculate for main form   28 april 2020
                                                            Call Recalculate_CalfieldsonSave(EID, res, con, tran) 'fOR cALCULATIV fIELD   ' changes by balli  brings from above to down 

                                                            'calculative fields
                                                            Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
                                                            Dim viewdoc As String = Convert.ToString(Documenttype)
                                                            viewdoc = viewdoc.Replace(" ", "_")
                                                            If CalculativeField.Length > 0 Then
                                                                For Each CField As DataRow In CalculativeField
                                                                    Dim formulaeditorr As New formulaEditor
                                                                    Dim forvalue As String = String.Empty
                                                                    'Coomented By Komal on 28March2014
                                                                    forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
                                                                    ' forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0)
                                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & res & ""
                                                                    da.SelectCommand.CommandText = upquery
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.SelectCommand.ExecuteNonQuery()
                                                                Next


                                                            End If


                                                            '' insert default first movement of document - by sunil
                                                            da.SelectCommand.CommandText = "InsertDefaultMovement"
                                                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                            da.SelectCommand.Parameters.Clear()
                                                            da.SelectCommand.Transaction = tran
                                                            da.SelectCommand.Parameters.AddWithValue("tid", res)
                                                            da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
                                                            da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                                                            da.SelectCommand.ExecuteNonQuery()

                                                            Dim res12 As String = String.Empty

                                                            res12 = ob1.GetNextUserFromRolematrixT(res, EID, "30200", "", "30200", con, tran)
                                                            Dim sretMsgArr() As String = res12.Split(":")
                                                            ob.HistoryT(EID, res, "30200", Convert.ToString(Documenttype), "MMM_MST_DOC", "ADD", con, tran)
                                                            If sretMsgArr(0) = "ARCHIVE" Then
                                                                'Dim Op As New Exportdata()
                                                                'Op.PushdataT(res, sretMsgArr(1), EID, con, tran)
                                                            End If

                                                            '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
                                                            If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                                                                Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                                                                'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
                                                                'this code block is added by ajeet kumar for transaction to be rolled back
                                                                ''''tran.Rollback()
                                                                ''''Exit Sub
                                                            Else

                                                            End If
                                                            'Execute Trigger
                                                            Try
                                                                Dim FormName As String = Convert.ToString(Documenttype)
                                                                '     Dim EID As Integer = 0
                                                                If (res > 0) And (FormName <> "") Then
                                                                    Trigger.ExecuteTriggerT(FormName, EID, res, con, tran)
                                                                End If
                                                            Catch ex As Exception
                                                                Throw
                                                            End Try

                                                            tran.Commit()

                                                            'Non transactional Query
                                                            'Check Work Flow
                                                            ob1.TemplateCalling(res, EID, Convert.ToString(Documenttype), "CREATED")
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.SelectCommand.Parameters.Clear()
                                                            da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                            Dim dt1 As New DataTable
                                                            da.Fill(dt1)
                                                            If dt1.Rows.Count = 1 Then
                                                                If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                    ob1.TemplateCalling(res, EID, Convert.ToString(Documenttype), "APPROVE")
                                                                End If
                                                            End If
                                                        End If


                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Next


                        End If
                    End If

                Next
            End If

        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
            End If
        End Try

    End Sub

    Public Sub CheckLMSettingsEdit(ByVal eid As Integer, ByVal Uid As Integer, ByVal DOCType As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction)
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con1 As SqlConnection = New SqlConnection(conStr)
        Dim ob As New DynamicForm
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran

        Try

            Dim con1 As SqlConnection = New SqlConnection(conStr)
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

            Dim ds As New DataSet
            Dim dt As New DataTable
            da1.Fill(ds, "LMSettingData")

            Dim Fldstr As String = ""
            If ds.Tables("LMSettingData").Rows.Count <> 0 Then


                Dim Uptstr As String = String.Empty
                Dim res As String = ""
                Dim strTFlds As String = String.Empty
                Dim strSFlds As String = String.Empty
                Dim strHTFlds As String = String.Empty
                Dim strHSFlds As String = String.Empty
                Dim ChildDocT As String = String.Empty
                Dim TDocType As String = String.Empty
                Dim TFldmapping As String = String.Empty
                Dim Sfldmapping As String = String.Empty


                Dim dtdata As New DataTable
                Dim dtdataHeader As New DataTable
                If ds.Tables("LMSettingData").Rows.Count > 0 Then
                    dtdata = ds.Tables("LMSettingData").[Select]("DocType = 'Line'").CopyToDataTable()
                    dtdataHeader = ds.Tables("LMSettingData").[Select]("DocType = 'Header'").CopyToDataTable()
                    If dtdataHeader.Rows.Count > 0 Then

                        strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strHSFldsArr As String()
                        strHSFldsArr = strHSFlds.Split(",")
                        Dim value As String = String.Join(",',',", strHSFldsArr)
                        strHSFlds = "d." + value
                    End If
                    If dtdata.Rows.Count > 0 Then
                        TDocType = dtdata.Rows(0).Item("TDocType")
                        ChildDocT = dtdata.Rows(0).Item("MChildDocType")
                        strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strSFldsArr As String()
                        strSFldsArr = strSFlds.Split(",")
                        Dim valueL As String = String.Join(",',',", strSFldsArr)
                        strSFlds = "i." + valueL
                        'For i As Integer = 0 To dtdata.Rows.Count - 1
                        da.SelectCommand.CommandText = "select df.FieldMapping as DFld,mf.FieldMapping as MFld from MMM_MST_FIELDS mf inner join MMM_MST_FIELDS df on mf.displayName=df.displayName where mf.eid=" & eid & " and mf.documenttype='" & TDocType & "' and  df.eid=" & eid & " and df.documenttype='" & DOCType & "'  and  df.FieldType='Auto Number'; "
                        da.Fill(ds, "LMDocDataFlds")
                        ' Next
                        If ds.Tables("LMDocDataFlds").Rows.Count > 0 Then
                            TFldmapping = ds.Tables("LMDocDataFlds").Rows(0).Item("MFld")
                            Sfldmapping = "d." + ds.Tables("LMDocDataFlds").Rows(0).Item("DFld")
                        End If


                        da.SelectCommand.CommandText = " select i.tid, concat(" & strSFlds & ") as SrcLineFldVAl,concat(" & strHSFlds & ") as SrcHeaderFldVAl," & Sfldmapping & " as Sfldmapping from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & DOCType & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.docid=" & DocID & " order by i.tid asc "
                        da.Fill(ds, "LMDocData")

                        'da.SelectCommand.CommandText = "Select TID from mmm_mst_master  where documenttype='" & TDocType & "' and eid=" & eid & "  and " & TFldmapping & "='" & ds.Tables("LMDocData").Rows(0).Item("Sfldmapping") & "' order by tid asc"
                        'da.Fill(ds, "LMMasterData")

                    End If

                    If ds.Tables("LMDocData").Rows.Count > 0 Then

                        For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1


                            Dim SrcLineFldsArr As String() = strTFlds.Split(",")

                            Dim strLineFldvalarr As String() = ds.Tables("LMDocData").Rows(j).Item("SrcLineFldVAl").ToString().Split(",")

                            Dim strRes As String = ""
                            For str1 As Integer = 0 To SrcLineFldsArr.Length - 1
                                For str As Integer = str1 To strLineFldvalarr.Length - 1
                                    strRes += "," + SrcLineFldsArr(str1) + "='" + strLineFldvalarr(str) + "'"

                                    Exit For
                                Next
                            Next

                            Dim SrcHeaderFldVAl As String = String.Empty
                            SrcHeaderFldVAl = ds.Tables("LMDocData").Rows(j).Item("SrcHeaderFldVAl")


                            Dim SfldmappingVal As String = String.Empty
                            SfldmappingVal = ds.Tables("LMDocData").Rows(j).Item("Sfldmapping")

                            Uptstr = "Update mmm_mst_master set UPDATEDDATE=getdate()" & strRes & " where eid=" & eid & " and " & TFldmapping & "='" & SfldmappingVal & "' and  documenttype='" & TDocType & "' and reftid=" & ds.Tables("LMDocData").Rows(j).Item("tid") & ";"
                            da.SelectCommand.CommandText = Uptstr.ToString()
                            res = da.SelectCommand.ExecuteScalar()

                            ob.HistoryT(eid, res, Uid, TDocType, "MMM_MST_MASTER", "EDIT", con, tran)



                        Next


                    End If

                End If

            Else

                da.Dispose()
                da.Dispose()

            End If

        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
        End Try

    End Sub

    Public Function CheckLMSettings(ByVal eid As Integer, ByVal DOCType As String, ByVal UID As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con1 As SqlConnection = New SqlConnection(conStr)
        Dim ob As New DynamicForm
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran
        'Chandni
        Try

            Dim con1 As SqlConnection = New SqlConnection(conStr)
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,lmfm.BaseDocName,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

            Dim ds As New DataSet
            Dim dt As New DataTable
            da1.Fill(ds, "LMSettingData")

            Dim Fldstr As String = ""
            If ds.Tables("LMSettingData").Rows.Count <> 0 Then


                Dim insStr As String = String.Empty
                Dim UpdateStr As String = String.Empty
                Dim res As String = ""
                Dim Upres As String = ""
                Dim MasterRes As String = ""
                Dim ChildDocT As String = String.Empty
                Dim TDocType As String = String.Empty

                Dim dtdata As New DataTable
                Dim dtdataHeader As New DataTable
                Dim dtdataSource As New DataTable
                Dim InsertQryStr As String = String.Empty
                Dim UpdateQryStr As String = String.Empty
                If ds.Tables("LMSettingData").Rows.Count > 0 Then

                    Dim SDocnames As List(Of String) = (From row In ds.Tables("LMSettingData").AsEnumerable()
                                                        Select row.Field(Of String)("BaseDocName") Distinct).ToList()
                    Dim strTFlds As String = String.Empty
                    Dim strHTFlds As String = String.Empty
                    For f As Integer = 0 To SDocnames.Count - 1
                        If dtdataSource.Rows.Count > 0 Then
                            dtdataSource = Nothing
                        End If
                        Dim dvs As DataView = ds.Tables("LMSettingData").DefaultView
                        dvs.RowFilter = "BaseDocName='" & SDocnames(f) & "'"
                        dtdataSource = dvs.ToTable()
                        If dtdataSource.Rows.Count > 0 Then
                            ChildDocT = ""
                            TDocType = ""
                            If SDocnames(f) = DOCType Then

                                Dim strSFlds As String = String.Empty
                                If dtdataHeader.Rows.Count > 0 Then
                                    dtdataHeader = Nothing
                                End If
                                If dtdata.Rows.Count > 0 Then
                                    dtdata = Nothing
                                End If
                                Dim strHSFlds As String = String.Empty
                                dtdata = dtdataSource.[Select]("DocType ='Line'").CopyToDataTable()
                                dtdataHeader = dtdataSource.[Select]("DocType ='Header'").CopyToDataTable()
                                strHTFlds = ""
                                If dtdataHeader.Rows.Count > 0 Then
                                    strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strHSFldsArr As String()
                                    strHSFldsArr = strHSFlds.Split(",")
                                    Dim value As String = String.Join(",',',", strHSFldsArr)
                                    strHSFlds = "d." + value
                                End If

                                If dtdata.Rows.Count > 0 Then

                                    strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strSFldsArr As String()
                                    strSFldsArr = strSFlds.Split(",")
                                    Dim valueL As String = String.Join(",',',", strSFldsArr)
                                    strSFlds = "i." + valueL + ",',',"

                                End If
                                TDocType = dtdata.Rows(0).Item("TDocType")
                                ChildDocT = dtdata.Rows(0).Item("ChildDocType")
                                Dim BaseDocName As String = dtdata.Rows(0).Item("BaseDocName")

                                InsertQryStr = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl  from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & BaseDocName & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.docid=" & DocID & "; "
                                da.SelectCommand.CommandText = InsertQryStr
                                da.Fill(ds, "LMDocData")
                                If ds.Tables("LMDocData").Rows.Count > 0 Then


                                    Dim ColumnStr As String = ""
                                    Dim str As String = ","
                                    Dim mtr As String = "|"
                                    'For Each datarow As DataRow In ds.Tables("LMDocData").Rows
                                    '    For Each datacol As DataColumn In ds.Tables("LMDocData").Columns
                                    '        datarow(datacol) = Replace(datarow(datacol).ToString, str, mtr)
                                    '    Next
                                    'Next

                                    For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1

                                        Dim LMFldsVal As String = String.Empty
                                        LMFldsVal = ds.Tables("LMDocData").Rows(j).Item("TFldVAl")
                                        Dim LMFldsValArr As String()
                                        LMFldsValArr = LMFldsVal.Split(",")
                                        Dim value As String = String.Join("','", LMFldsValArr)
                                        value = "'" + value + "'"
                                        'value = Replace(value.ToString, "|", ",")

                                        insStr = "insert into mmm_mst_master (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & "," & strHTFlds & ",refTid) values ('" & TDocType & "'," & eid & ",1,getdate(),'" & UID & "',getdate(),getdate()," & value & "," & ds.Tables("LMDocData").Rows(j).Item("tid") & " );"

                                        da.SelectCommand.CommandText = insStr.ToString() + ";Select @@identity"
                                        da.SelectCommand.Transaction = tran
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If
                                        res = da.SelectCommand.ExecuteScalar()
                                        MasterRes = MasterRes + res + ","

                                        ob.HistoryT(eid, res, UID, TDocType, "MMM_MST_MASTER", "ADD", con, tran)
                                        'Execute Trigger
                                        Try
                                            Dim FormName As String = TDocType
                                            '     Dim EID As Integer = 0
                                            If (res > 0) And (FormName <> "") Then
                                                ' Trigger.ExecuteTriggerT(FormName, eid, res, con, tran)
                                            End If
                                        Catch ex As Exception
                                            Throw
                                        End Try

                                    Next
                                    '    End If
                                End If
                            ElseIf SDocnames(f) <> DOCType Then
                                If dtdataHeader.Rows.Count > 0 Then
                                    dtdataHeader = Nothing
                                End If
                                If dtdata.Rows.Count > 0 Then
                                    dtdata = Nothing
                                End If

                                Dim strSFlds As String = String.Empty
                                Dim strHSFlds As String = String.Empty
                                strHTFlds = ""
                                strTFlds = ""
                                dtdata = dtdataSource.[Select]("DocType ='Line'").CopyToDataTable()
                                dtdataHeader = dtdataSource.[Select]("DocType ='Header'").CopyToDataTable()

                                If dtdataHeader.Rows.Count > 0 Then

                                    strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strHSFldsArr As String()
                                    strHSFldsArr = strHSFlds.Split(",")
                                    Dim value As String = String.Join(",',',", strHSFldsArr)
                                    strHSFlds = "d." + value
                                End If

                                If dtdata.Rows.Count > 0 Then

                                    strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strSFldsArr As String()
                                    strSFldsArr = strSFlds.Split(",")
                                    Dim valueL As String = String.Join(",',',", strSFldsArr)
                                    strSFlds = "i." + valueL + ",',',"

                                End If
                                TDocType = dtdata.Rows(0).Item("TDocType")
                                ChildDocT = dtdata.Rows(0).Item("ChildDocType")
                                Dim BaseDocName As String = dtdata.Rows(0).Item("BaseDocName")

                                UpdateQryStr = ""
                                Dim ValueinsStr As String = ""
                                ValueinsStr = "select p.FieldMapping as pFieldMapping, p.FieldType as pFieldType, p.Dropdown as pDropdown,c.FieldMapping as cFieldMapping from mmm_mst_fields p inner join mmm_mst_fields c on p.displayName=c.displayName where p.eid=" & eid & " and p.Documenttype='" & DOCType & "' and c.Documenttype='" & ChildDocT & "' ;"
                                da.SelectCommand.CommandText = ValueinsStr
                                da.Fill(ds, "ValueinsStr")

                                Dim MasterDAta As String = ""
                                MasterDAta = "select * from mmm_mst_master  where  eid=" & eid & " and  Documenttype='" & TDocType & "' and tid in (" & MasterRes.TrimEnd(",") & ") ;"
                                da.SelectCommand.CommandText = MasterDAta
                                da.Fill(ds, "MasterDAta")



                                If ds.Tables("MasterDAta").Rows.Count > 0 Then

                                    For j As Integer = 0 To ds.Tables("MasterDAta").Rows.Count - 1
                                        Dim FldinsStr As String = ""
                                        Dim fldtype As String = ""
                                        If ds.Tables("ValueinsStr").Rows(0).Item("pFieldType") = "Drop Down" Then
                                            fldtype = "dms.udf_split('" & ds.Tables("ValueinsStr").Rows(0).Item("pDropdown").ToString() & "'," + ds.Tables("ValueinsStr").Rows(0).Item("pFieldMapping").ToString() + ")"
                                        Else
                                            fldtype = ds.Tables("ValueinsStr").Rows(0).Item("pFieldMapping").ToString()
                                        End If
                                        FldinsStr = "select " & fldtype & " as fldval from mmm_mst_doc   where  eid=" & eid & " and  Documenttype='" & DOCType & "' and tid=" & DocID & " ;"
                                        da.SelectCommand.CommandText = FldinsStr
                                        da.Fill(ds, "FldinsStr")
                                        If ds.Tables("FldinsStr").Rows.Count > 0 Then
                                            UpdateQryStr = ""
                                            UpdateQryStr = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl  from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & BaseDocName & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i." & ds.Tables("ValueinsStr").Rows(0).Item("cFieldMapping").ToString() & "='" & ds.Tables("FldinsStr").Rows(0).Item("fldval").ToString() & "'; "
                                            da.SelectCommand.CommandText = UpdateQryStr
                                            da.Fill(ds, "UpdateLMDocData")

                                            If ds.Tables("UpdateLMDocData").Rows.Count > 0 Then
                                                'Fields
                                                Dim FieldStr As String = strTFlds + "," + strHTFlds
                                                Dim FieldArr As String() = FieldStr.Split(",")
                                                Dim Field As String = String.Join("=,", FieldArr)
                                                Field = Field + "="
                                                'Value
                                                Dim LMFldsVal As String = String.Empty
                                                LMFldsVal = ds.Tables("UpdateLMDocData").Rows(j).Item("TFldVAl")
                                                Dim LMFldsValArr As String()
                                                LMFldsValArr = LMFldsVal.Split(",")
                                                Dim valuestr As String = String.Join("','", LMFldsValArr)
                                                valuestr = "'" + valuestr + "'"
                                                Dim ValueArr As String() = valuestr.Split(",")

                                                Dim joinarr = Field.Split(",")
                                                Dim Value As String = String.Join(",", ValueArr)
                                                Dim strValue As String = ""
                                                For k As Integer = 0 To joinarr.Length - 1
                                                    For l As Integer = k To ValueArr.Length - 1
                                                        strValue = strValue + joinarr(k) + ValueArr(l) + ","
                                                        Exit For
                                                    Next
                                                    Dim strValu1e As String = ""
                                                Next
                                                UpdateStr = ""
                                                UpdateStr = "Update mmm_mst_master set UPDATEDDATE=getdate(),LASTUPDATE=getdate()," & strValue.TrimEnd(",") & " where tid=" & ds.Tables("MasterDAta").Rows(j).Item("Tid") & " and eid=" & eid & " and documenttype='" & TDocType & "' ;"

                                                da.SelectCommand.CommandText = UpdateStr.ToString()
                                                da.SelectCommand.Transaction = tran
                                                If con.State = ConnectionState.Closed Then
                                                    con.Open()
                                                End If
                                                Upres = da.SelectCommand.ExecuteScalar()



                                            End If

                                        End If

                                        'calculating backdated data for Lease Amendment Document
                                        If DOCType = "Lease Amendment Document" Then

                                            Dim FieldData As New DataTable

                                            If FieldData.Rows.Count > 0 Then
                                                FieldData.Clear()
                                            End If
                                            'Collecting the field data
                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = '" & Convert.ToString(DOCType) & "' order by displayOrder", con)
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.SelectCommand.Transaction = tran
                                            da.Fill(ds, "fields")
                                            FieldData = ds.Tables("fields")

                                            Dim AmendmentSdt As Date
                                            Dim LeaseSdt As Date
                                            Dim AmendmentSdate As String = String.Empty
                                            Dim LeaseSdate As String = String.Empty
                                            Dim LeaseSDocNo As String = String.Empty
                                            Dim rowln As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Doc no' and Datatype='text' and Documenttype='" & DOCType & "'")
                                            If rowln.Length > 0 Then
                                                For Each CField As DataRow In rowln
                                                    LeaseSDocNo = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            Dim rowASdt As DataRow() = ds.Tables("fields").Select("DisplayName='Amendment Start Date' and Datatype='Datetime' and Documenttype='" & DOCType & "'")
                                            If rowASdt.Length > 0 Then
                                                For Each CField As DataRow In rowASdt
                                                    AmendmentSdate = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            Dim rowLSdt As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Start Date' and Datatype='Datetime' and Documenttype='" & DOCType & "'")
                                            If rowLSdt.Length > 0 Then
                                                For Each CField As DataRow In rowLSdt
                                                    LeaseSdate = CField.Item("FieldMapping").ToString
                                                Next
                                            End If


                                            Dim ChildItemdocType As String = String.Empty
                                            Dim rowLSCI As DataRow() = ds.Tables("fields").Select("FieldType='Child Item' and Documenttype='" & DOCType & "'")
                                            If rowLSCI.Length > 0 Then
                                                For Each CField As DataRow In rowLSCI
                                                    ChildItemdocType = CField.Item("DropDown").ToString
                                                Next
                                            End If

                                            'Collecting the field data
                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = '" & Convert.ToString(ChildItemdocType) & "' order by displayOrder", con)
                                            da.SelectCommand.Transaction = tran
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.Fill(ds, "Childfields")

                                            Dim LALessorsMgAmt As Double = 0
                                            Dim LLessorsMgAmt As Double = 0
                                            Dim LALessorsMgAmtstr As String = ""
                                            Dim LLessorsMgAmtstr As String = ""
                                            Dim rowLASChildItem As DataRow() = ds.Tables("Childfields").Select("DisplayName='Lessors MG Amount Share' and Datatype='Numeric' and Documenttype='" & ChildItemdocType & "'")
                                            If rowLASChildItem.Length > 0 Then
                                                For Each CField As DataRow In rowLASChildItem
                                                    LLessorsMgAmtstr = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            Dim rowLSChildItem As DataRow() = ds.Tables("Childfields").Select("DisplayName='Lessors MG Amount Share.' and Datatype='Numeric' and Documenttype='" & ChildItemdocType & "'")
                                            If rowLSChildItem.Length > 0 Then
                                                For Each CField As DataRow In rowLSChildItem
                                                    LALessorsMgAmtstr = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            da = New SqlDataAdapter("Select dms.udf_split('DOCUMENT-Mou Lease Document-fld50',d." & LeaseSDocNo & ") as LeaseSDocNo, d." & AmendmentSdate & " as AmendmentSdate,d." & LeaseSdate & " as LeaseSdate,i." & LALessorsMgAmtstr & " as LALessorsMgAmt,i." & LLessorsMgAmtstr & " as LLessorsMgAmt from  mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.docid where d.documenttype='" & DOCType & "' and d.eid=" & eid & " and d.tid=" & DocID & "", con)
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.SelectCommand.Transaction = tran
                                            da.Fill(ds, "LeaseAmendmentData")


                                            If ds.Tables("LeaseAmendmentData").Rows.Count > 0 Then
                                                'Lease No
                                                Dim LDocNo As String = ds.Tables("LeaseAmendmentData").Rows(0).Item("LeaseSDocNo").ToString()
                                                'Calculeting the Amendment lease start date
                                                Dim LADtarr = ds.Tables("LeaseAmendmentData").Rows(0).Item("AmendmentSdate").ToString().Split("/")
                                                Dim LAdate1 As New Date(LADtarr(2), LADtarr(1), LADtarr(0))
                                                LAdate1 = CDate(LAdate1)
                                                AmendmentSdt = Convert.ToDateTime(LAdate1.ToString("MM/dd/yy"))
                                                'Calculeting the Actual lease start date
                                                Dim LDtarr = ds.Tables("LeaseAmendmentData").Rows(0).Item("LeaseSdate").ToString().Split("/")
                                                Dim Ldate1 As New Date(LDtarr(2), LDtarr(1), LDtarr(0))
                                                Ldate1 = CDate(Ldate1)
                                                LeaseSdt = Convert.ToDateTime(Ldate1.ToString("MM/dd/yy"))

                                                Dim TotalMGAmt As Double = 0
                                                'checking backdated data of Lease amendment
                                                If AmendmentSdt > LeaseSdt And AmendmentSdt < Date.Now Then
                                                    Dim AlreadyEistData As New DataTable
                                                    Dim MGAmtDT As Double = 0
                                                    Dim Fieldmap As String = String.Empty
                                                    Dim EscFieldmap As String = String.Empty
                                                    Dim RentCycFieldmap As String = String.Empty
                                                    'Collecting the field data
                                                    da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = 'Rental Invoice' order by displayOrder", con)
                                                    da.SelectCommand.Transaction = tran
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.Fill(ds, "Rentalfields")

                                                    Dim rowRCD As DataRow() = ds.Tables("Rentalfields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                                    Dim rowECD As DataRow() = ds.Tables("Rentalfields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                    Dim rowRCCD As DataRow() = ds.Tables("Rentalfields").Select("DisplayName='Rent Payment Cycle' ")
                                                    If rowRCD.Length > 0 Then
                                                        For Each CField As DataRow In rowRCD
                                                            Fieldmap = CField.Item("FieldMapping").ToString()
                                                        Next
                                                    End If
                                                    If rowECD.Length > 0 Then
                                                        For Each CField As DataRow In rowECD
                                                            EscFieldmap = CField.Item("FieldMapping").ToString()
                                                        Next
                                                    End If
                                                    If rowRCCD.Length > 0 Then
                                                        For Each CField As DataRow In rowRCCD
                                                            RentCycFieldmap = CField.Item("FieldMapping").ToString()
                                                        Next
                                                    End If
                                                    da.SelectCommand.CommandText = "Select top 1  convert(varchar,CreatedOn,103)   as InvCreationDt, " & EscFieldmap & " as EscDate," & RentCycFieldmap & " as RentCyc from mmm_mst_doc with (nolock) where fld2='" & LDocNo & "' and eid=" & eid & "  and Documenttype='rental invoice' order by tid desc "
                                                    da.Fill(AlreadyEistData)

                                                    Dim FinalInvDate As String = ""
                                                    Dim LRentPayment As String = ""
                                                    Dim InvCreationDtarr = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString().Split("/")
                                                    Dim InvCreationLRIGDTdate1 As New Date(InvCreationDtarr(2), InvCreationDtarr(1), InvCreationDtarr(0))
                                                    InvCreationLRIGDTdate1 = CDate(InvCreationLRIGDTdate1)
                                                    Dim LastInvGenDt As Date = InvCreationLRIGDTdate1


                                                    LRentPayment = AlreadyEistData.Rows(0).Item("RentCyc").ToString()

                                                    Dim ResultStr = ParseDateFn(LRentPayment, LastInvGenDt, LLessorsMgAmt)
                                                    Dim PDFnResultStr = ResultStr.Split("=")
                                                    FinalInvDate = PDFnResultStr(0)
                                                    MGAmtDT = PDFnResultStr(1)
                                                    Dim LPEdate As Date = New Date(CDate(FinalInvDate).Year, CDate(FinalInvDate).Month, 1).AddDays(-1)
                                                    Dim LPEdatestr As String = LPEdate.ToString("dd/MM/yy")
                                                    Dim LAdiff = DateDiff(DateInterval.Month, AmendmentSdt, LPEdate) + 1 'DateDiff(DateInterval.Month, startDt, endDt)

                                                    If (LAdiff > 0) Then

                                                        LALessorsMgAmt = LAdiff * Convert.ToDouble(ds.Tables("LeaseAmendmentData").Rows(0).Item("LALessorsMgAmt"))
                                                        LLessorsMgAmt = LAdiff * Convert.ToDouble(ds.Tables("LeaseAmendmentData").Rows(0).Item("LLessorsMgAmt"))

                                                        TotalMGAmt = LALessorsMgAmt - LLessorsMgAmt


                                                        Dim decimalVar As Decimal
                                                        decimalVar = Decimal.Round(TotalMGAmt, 2, MidpointRounding.AwayFromZero)
                                                        decimalVar = Math.Round(decimalVar, 2)
                                                        TotalMGAmt = 0
                                                        TotalMGAmt = decimalVar


                                                        Try
                                                            Dim SourceDocData As New DataTable
                                                            If SourceDocData.Rows.Count > 0 Then
                                                                SourceDocData.Clear()
                                                            End If
                                                            Dim strHSFldsInv As String = String.Empty
                                                            Dim strSFldsInv As String = String.Empty
                                                            Dim strTFldsINV As String = String.Empty
                                                            Dim strHSFldsArrInv As String()
                                                            Dim valueLInv As String
                                                            Dim RDOCID As String = ""
                                                            Dim value As String = ""
                                                            da.SelectCommand.CommandText = "select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld,RentEsc,CAMEsc,SDmonths,RentEscptage	,CamEscptage from   MMM_MST_AutoInvoiceSetting with(nolock) where  IsActiveStatus=1 and Sdoctype='Lease Master'"

                                                            da.Fill(ds, "AutoInvSettingData")
                                                            Dim AutoInvFieldData As New DataTable
                                                            If ds.Tables("AutoInvSettingData").Rows.Count <> 0 Then
                                                                For i As Integer = 0 To ds.Tables("AutoInvSettingData").Rows.Count - 1

                                                                    If AutoInvFieldData.Rows.Count > 0 Then
                                                                        AutoInvFieldData.Clear()
                                                                    End If

                                                                    da.SelectCommand.CommandText = "select  F.Tid,F.TFld,F.SFld,F.SDocType,F.AutoInvTid,F.EID,F.TFldName,F.sFldName,F.Leasetype,F.TDocType  from    MMM_MST_AutoInvoiceSetting C inner join MMM_MST_AutoInvFieldSetting F on c.Tid=F.AutoInvTid where C.eid=" & eid & " and C.IsActiveStatus=1 and F.AutoInvTid=" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid")) & ""
                                                                    ' da.SelectCommand.Transaction = tran
                                                                    da.Fill(ds, "AutoInvFieldData")
                                                                    AutoInvFieldData = ds.Tables("AutoInvFieldData")
                                                                    If AutoInvFieldData.Rows.Count <> 0 Then

                                                                        Dim namesInv As List(Of String) = (From row In AutoInvFieldData.AsEnumerable()
                                                                                                           Select row.Field(Of String)("TDocType") Distinct).ToList()

                                                                        For Inv As Integer = 0 To namesInv.Count - 1
                                                                            If namesInv(Inv) = "Rental Invoice" Then

                                                                                Dim dvsInv As DataView = AutoInvFieldData.DefaultView
                                                                                dvsInv.RowFilter = "TDocType='" & namesInv(Inv) & "'"

                                                                                Dim filteredTable As New DataTable()
                                                                                filteredTable = dvsInv.ToTable()
                                                                                If filteredTable.Rows.Count <> 0 Then
                                                                                    strTFldsINV = ""
                                                                                    strSFldsInv = ""
                                                                                    strTFldsINV += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray().Distinct())
                                                                                    strSFldsInv += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray().Distinct())
                                                                                    strHSFldsArrInv = Nothing
                                                                                    strHSFldsArrInv = strSFldsInv.Split(",")
                                                                                    valueLInv = String.Join(",',',", strHSFldsArrInv)
                                                                                    strHSFldsInv = valueLInv
                                                                                End If

                                                                                If FieldData.Rows.Count > 0 Then
                                                                                    FieldData.Clear()
                                                                                End If
                                                                                da.SelectCommand.CommandText = " select fld2 as [LeaseDocNo],fld7 as [Rental Amount],tid as RDOCID,concat(" & strHSFldsInv & ") as TFldVAl from MMM_MST_MASTER   where documenttype='Lease Master'  and eid=" & eid & " and tid=" & ds.Tables("MasterDAta").Rows(j).Item("Tid") & " "
                                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                                ' da1.SelectCommand.Transaction = tran
                                                                                da.Fill(ds, "SourceDocData")


                                                                                SourceDocData = ds.Tables("SourceDocData")
                                                                                Dim FldsValInv As String = ""

                                                                                Dim FldsValArrInv As String() = Nothing
                                                                                If SourceDocData.Rows.Count > 0 Then

                                                                                    Dim LeaseDocNo As String = ""
                                                                                    If SourceDocData.Rows.Count > 0 Then
                                                                                        FldsValInv = ""
                                                                                        FldsValInv = SourceDocData.Rows(0).Item("TFldVAl")
                                                                                        FldsValArrInv = FldsValInv.Split(",")
                                                                                        value = ""
                                                                                        value = String.Join("','", FldsValArrInv)
                                                                                        value = "'" + value + "'"
                                                                                        RDOCID = Convert.ToString(SourceDocData.Rows(0).Item("RDOCID"))
                                                                                        LeaseDocNo = Convert.ToString(SourceDocData.Rows(0).Item("LeaseDocNo"))
                                                                                    End If
                                                                                    If SourceDocData.Rows.Count > 0 Then

                                                                                        If FieldData.Rows.Count > 0 Then
                                                                                            FieldData.Clear()
                                                                                        End If
                                                                                        ' Collecting the field data
                                                                                        da.SelectCommand.CommandText = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = '" & Convert.ToString(namesInv(Inv)) & "' order by displayOrder"
                                                                                        da.SelectCommand.CommandType = CommandType.Text
                                                                                        'da1.SelectCommand.Transaction = tran
                                                                                        da.Fill(ds, "fields")
                                                                                        FieldData = ds.Tables("fields")
                                                                                    End If

                                                                                End If
                                                                            End If
                                                                        Next
                                                                    End If

                                                                Next
                                                            End If


                                                            Call AutoInvoiceAmendLease(AmendmentSdt:=AmendmentSdt, MasterTid:=ds.Tables("MasterDAta").Rows(j).Item("Tid"), Doctype:="Lease Master", LessorsMGAmt:=TotalMGAmt, value:=value, RDOCID:=RDOCID, FieldData:=FieldData, strTFlds:=strTFldsINV, con:=con, tran:=tran)
                                                            strTFldsINV = ""



                                                        Catch ex As Exception
                                                            AutoRunLog("Lease Amendment", "AutoInvoiceAmendLease", "AutoInvoice Function Exception:" + ex.Message.ToString(), 181)
                                                            tran.Rollback()
                                                            Exit Function
                                                        End Try
                                                    End If




                                                End If

                                            End If


                                        End If


                                    Next
                                End If
                            End If

                        End If
                    Next

                Else

                    da.Dispose()
                    da.Dispose()
                    Return "fail"
                    Return "NA"
                End If



            End If
        Catch ex As Exception
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try
        Return "Success"
    End Function

    Protected Sub AutoInvoiceAmendLease(ByVal AmendmentSdt As Date, ByVal MasterTid As String, ByVal Doctype As String, ByVal LessorsMGAmt As Double, ByVal RDOCID As String, ByVal value As String, ByVal strTFlds As String, FieldData As DataTable, ByVal con As SqlConnection, ByVal tran As SqlTransaction)


        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim insStr As String = String.Empty
        Dim UpdStr As String = String.Empty
        Dim res As Integer = 0
        Dim con1 As SqlConnection = New SqlConnection(conStr)
        con1.Open()
        Dim trans1 As SqlTransaction = Nothing
        trans1 = con1.BeginTransaction()
        Dim ob1 As New DMSUtil()
        Dim ob As New DynamicForm

        Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        da.SelectCommand.Transaction = trans1
        'Chandni
        Try

            da.SelectCommand.CommandText = "select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld,RentEsc,CAMEsc,SDmonths,RentEscptage	,CamEscptage from   MMM_MST_AutoInvoiceSetting with(nolock) where  IsActiveStatus=1 and Sdoctype='" & Doctype & "'"
            '  da.SelectCommand.Transaction = tran
            Dim ds As New DataSet
            Dim dt As New DataTable
            da.Fill(ds, "AutoInvSettingData")
            Dim StartDateFld As String = String.Empty
            Dim EndDateFld As String = String.Empty
            Dim FrequencyFld As String = String.Empty
            Dim PeriodFld As Int16 = 0
            Dim RentalFld As String = String.Empty
            Dim RegistrationDateFld As String = String.Empty
            Dim SchedulerTidID As String = String.Empty
            Dim RentEsc As String = String.Empty
            Dim CAMEsc As String = String.Empty
            Dim SDmonths As String = String.Empty
            Dim RentEscPtage As String = String.Empty
            Dim CAMEscPtage As String = String.Empty
            Dim Fldstr As String = ""

            Dim strSFlds As String = String.Empty
            Dim strHTFlds As String = String.Empty
            Dim strHSFlds As String = String.Empty
            Dim RentFreePeriodFlds As String = String.Empty
            Dim EID As String = String.Empty
            Dim LeaseType As String = String.Empty
            Dim MGAmount As String = String.Empty
            If ds.Tables("AutoInvSettingData").Rows.Count <> 0 Then
                For i As Integer = 0 To ds.Tables("AutoInvSettingData").Rows.Count - 1


                    StartDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("StartDateFld"))
                    EndDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EndDateFld"))
                    FrequencyFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("FrequencyFld"))
                    RentalFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                    RegistrationDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RegistrationDateFld"))
                    RentFreePeriodFlds = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentFreePeriodFld"))
                    EID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EID"))
                    SchedulerTidID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid"))
                    LeaseType = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("LeaseType"))
                    MGAmount = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                    RentEsc = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentEsc"))
                    CAMEsc = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMEsc"))
                    RentEscPtage = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentEscPtage"))
                    CAMEscPtage = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMEscPtage"))
                    SDmonths = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDMonths"))

                    Dim FldsVal As String = String.Empty
                    Dim SchedulerCheck As Boolean
                    '  SchedulerCheck = Scheduler(SchedulerTidID)
                    SchedulerCheck = True


                    da.SelectCommand.CommandText = "select  F.Tid,F.TFld,F.SFld,F.SDocType,F.AutoInvTid,F.EID,F.TFldName,F.sFldName,F.Leasetype,F.TDocType  from    MMM_MST_AutoInvoiceSetting C inner join MMM_MST_AutoInvFieldSetting F on c.Tid=F.AutoInvTid where C.eid=" & EID & " and C.IsActiveStatus=1 and F.AutoInvTid=" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid")) & ""
                    ' da.SelectCommand.Transaction = tran
                    da.Fill(ds, "AutoInvFieldData")

                    Dim AutoInvDocData As New DataTable

                    Dim fieldmapping As String = ""
                    Dim fieldmappingINVdt As String = ""
                    Dim fieldmappingLeaseDocdt As String = ""
                    If AutoInvDocData.Rows.Count <> 0 Then
                        ds.Tables("AutoInvDocData").Clear()
                    End If


                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and Displayname='Rent Invoice Generation Date' and datatype='Datetime'"
                    Dim dtDtF As New DataTable
                    ' da.SelectCommand.Transaction = tran
                    da.Fill(dtDtF)
                    If dtDtF.Rows.Count > 0 Then
                        fieldmappingINVdt = dtDtF.Rows(0).Item("fieldmapping")
                    End If
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and Displayname='Lease Doc No' and datatype='Text'"
                    Dim LDocdtDtF As New DataTable
                    ' da.SelectCommand.Transaction = tran
                    da.Fill(LDocdtDtF)
                    If LDocdtDtF.Rows.Count > 0 Then
                        fieldmappingLeaseDocdt = LDocdtDtF.Rows(0).Item("fieldmapping")
                    End If


                    'Rental Inv Gen data
                    da.SelectCommand.CommandText = "select   " & LeaseType & "  as leasetype, " & fieldmappingLeaseDocdt & " as [LeaseDocNo], " & fieldmappingINVdt & " as LRentInvGenDate,  tid as tid,convert(varchar, convert(datetime," & RegistrationDateFld & ", 3), 103)  as RegistrationDate ," & RentFreePeriodFlds & " as RentFreePeriod," & FrequencyFld & " as [RentPaymentCycle],fld49 as [RentFreeFitOutStartDate],fld50 as [RentFreeFitOutEndDate]," & RentFreePeriodFlds & " as [RentFreeDays]," & StartDateFld & " as LStartDate , " & EndDateFld & " as LEndDate," & MGAmount & " as LMGAmount," & RentEsc & " As RentEsc," & CAMEsc & " as CAMEsc," & SDmonths & " as SDMonths," & RentEscPtage & " as RentEscPtage," & CAMEscPtage & " as CAMEscPtage,fld57 as [LessorsPropertyShare],fld41 as [RentType],fld76 as [CamPaymentcycle],fld10 as SDAmt,fld48 as CAMCommDate,fld63 as CAMAmt,'YES' as AmendmentFlag from MMM_MST_MASTER  with(nolock)   where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and  convert(date, getdate(), 3) between   convert(date," & StartDateFld & ", 3) and  convert(date, " & EndDateFld & ", 3)    and reftid<>''   and tid in (" & MasterTid & ")"
                    'da.SelectCommand.Transaction = tran

                    da.Fill(ds, "AutoInvDocData")
                    AutoInvDocData = ds.Tables("AutoInvDocData")
                    'Dim SourceDocData As New DataTable
                    Dim RDOCIDData As New DataTable

                    Dim LDocNo As String = String.Empty
                    Dim LStartDate As String = String.Empty
                    Dim LENDDate As String = String.Empty
                    Dim LFitOutStartDate As String = String.Empty
                    Dim LFitOutEndDate As String = String.Empty
                    Dim LeaseComDate As String = String.Empty
                    Dim CAMComDate As String = String.Empty
                    Dim LRentPayment As String = String.Empty
                    Dim LRentFreedays As String = String.Empty
                    Dim LRentInvGenDate As String = String.Empty
                    Dim LRentESC As String = String.Empty
                    Dim LCAMEsc As String = String.Empty
                    Dim LRentESCPtage As String = String.Empty
                    Dim LCAMEscPtage As String = String.Empty
                    Dim LSDMonths As String = String.Empty
                    Dim LPropershare As String = String.Empty
                    Dim LRenttype As String = String.Empty
                    Dim LCAMRentCycletype As String = String.Empty
                    Dim LAmendmentFlag As String = String.Empty
                    Dim LMGAmt As Double = 0
                    Dim LSDAmt As Double = 0
                    Dim LCAMAmt As Double = 0


                    If ds.Tables("AutoInvDocData").Rows.Count <> 0 Then

                        'For j As Integer = 0 To ds.Tables("AutoInvDocData").Rows.Count - 1

                        LDocNo = ds.Tables("AutoInvDocData").Rows(0).Item("LeaseDocNo")
                        LStartDate = ds.Tables("AutoInvDocData").Rows(0).Item("LStartDate")
                        LENDDate = ds.Tables("AutoInvDocData").Rows(0).Item("LEndDate")
                        LFitOutStartDate = ds.Tables("AutoInvDocData").Rows(0).Item("RentFreeFitOutStartDate")
                        LFitOutEndDate = ds.Tables("AutoInvDocData").Rows(0).Item("RentFreeFitOutEndDate")
                        LRentPayment = ds.Tables("AutoInvDocData").Rows(0).Item("RentPaymentCycle")
                        LRentInvGenDate = ds.Tables("AutoInvDocData").Rows(0).Item("LRentInvGenDate")
                        LMGAmt = ds.Tables("AutoInvDocData").Rows(0).Item("LMGAmount")
                        LSDAmt = ds.Tables("AutoInvDocData").Rows(0).Item("SDAmt")
                        LRentESC = ds.Tables("AutoInvDocData").Rows(0).Item("RentESC")
                        LCAMEsc = ds.Tables("AutoInvDocData").Rows(0).Item("CAMESC")
                        LRentESCPtage = ds.Tables("AutoInvDocData").Rows(0).Item("RentESCPtage")
                        LCAMEscPtage = ds.Tables("AutoInvDocData").Rows(0).Item("CAMESCPtage")
                        LSDMonths = ds.Tables("AutoInvDocData").Rows(0).Item("SdMonths")
                        LPropershare = ds.Tables("AutoInvDocData").Rows(0).Item("LessorsPropertyShare")
                        LRenttype = ds.Tables("AutoInvDocData").Rows(0).Item("Renttype")
                        LCAMRentCycletype = ds.Tables("AutoInvDocData").Rows(0).Item("CamPaymentcycle")
                        CAMComDate = ds.Tables("AutoInvDocData").Rows(0).Item("CAMCommDate")
                        LCAMAmt = ds.Tables("AutoInvDocData").Rows(0).Item("CAMAmt")
                        LAmendmentFlag = ds.Tables("AutoInvDocData").Rows(0).Item("AmendmentFlag")



                        If ds.Tables("AutoInvDocData").Rows(0).Item("leasetype") = "1491591" Or ds.Tables("AutoInvDocData").Rows(0).Item("leasetype") = "1570943" Or ds.Tables("AutoInvDocData").Rows(0).Item("leasetype") = "1554309" Or ds.Tables("AutoInvDocData").Rows(0).Item("leasetype") = "1570941" Then 'Lease Type Master
                            Dim names As List(Of String) = (From row In ds.Tables("AutoInvFieldData").AsEnumerable()
                                                            Select row.Field(Of String)("TDocType") Distinct).ToList()

                            For f As Integer = 0 To names.Count - 1
                                If names(f) = "Rental Invoice" Then


                                    Dim Finaldate As String = String.Empty


                                    ' If ds.Tables("RDOCIDData").Rows.Count > 0 Then

                                    If LRenttype = 1554651 Or LRenttype = 1554653 Then 'Fixed And Revenue Sharing  Then 'Fixed

                                        Dim FinalInvDate As String = String.Empty
                                        Dim AlreadyEistData As New DataTable
                                        Dim MGAmtDT As Double = 0
                                        Dim Fieldmap As String = String.Empty
                                        Dim EscFieldmap As String = String.Empty
                                        If Convert.ToString(names(f)) = "Rental Invoice" Then
                                            Dim rowRCD As DataRow() = FieldData.Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                            Dim rowECD As DataRow() = FieldData.Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                            If rowRCD.Length > 0 Then
                                                For Each CField As DataRow In rowRCD
                                                    Fieldmap = CField.Item("FieldMapping").ToString()
                                                Next
                                            End If
                                            If rowECD.Length > 0 Then
                                                For Each CField As DataRow In rowECD
                                                    EscFieldmap = CField.Item("FieldMapping").ToString()
                                                Next
                                            End If
                                            da.SelectCommand.CommandText = "Select top 1 " & Fieldmap & "  as InvCreationDt, " & EscFieldmap & " as EscDate from mmm_mst_doc with (nolock) where fld2='" & LDocNo & "' and eid=" & EID & "  and Documenttype='" & Convert.ToString(names(f)) & "' order by tid desc "


                                        End If

                                        If AlreadyEistData.Rows.Count > 0 Then
                                            AlreadyEistData.Clear()
                                        End If
                                        da.SelectCommand.Transaction = trans1
                                        da.Fill(AlreadyEistData)


                                        Dim InvCreationDtarr = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString().Split("/")
                                        Dim InvCreationLRIGDTdate1 As New Date(InvCreationDtarr(2), InvCreationDtarr(1), InvCreationDtarr(0))
                                        InvCreationLRIGDTdate1 = CDate(InvCreationLRIGDTdate1)
                                        Dim InvGenDt As DateTime = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("MM/dd/yy"))

                                        Dim InvGenDate As String = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("dd/MM/yy"))
                                        Dim ResultStr = ParseDateFn(LRentPayment, InvGenDt, LessorsMGAmt)
                                        Dim PDFnResultStr = ResultStr.Split("=")
                                        FinalInvDate = PDFnResultStr(0)
                                        MGAmtDT = PDFnResultStr(1)

                                        insStr = ""
                                        If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                            insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & RDOCID & "' );"
                                        ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                            insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),getdate(),getdate(),'30200'," & value & ",'" & RDOCID & "' );"
                                        End If
                                        da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.Transaction = trans1
                                        If con1.State = ConnectionState.Closed Then
                                            con1.Open()
                                        End If
                                        res = da.SelectCommand.ExecuteScalar()

                                        Finaldate = ""
                                        Dim FinaldateARR As String() = FinalInvDate.Split("/")
                                        Finaldate = FinaldateARR(1) + "/" + FinaldateARR(0) + "/" + FinaldateARR(2)
                                        Finaldate = CDate(Finaldate).ToString("dd/MM/yy")
                                        Dim FinalDateStr As String() = Finaldate.Split("/")
                                        Dim LFinalDateStr As String = FinalDateStr(1) + "/" + FinalDateStr(0) + "/" + FinalDateStr(2)

                                        ' End If

                                        If res <> 0 Then

                                            Dim rowMGAmt As DataRow() = FieldData.Select("DisplayName='Lessors MG Amount'  and Datatype='Numeric'")
                                            If rowMGAmt.Length > 0 Then
                                                For Each CField As DataRow In rowMGAmt

                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsMGAmt & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                    da.SelectCommand.CommandText = upquery
                                                    da.SelectCommand.Transaction = trans1
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.ExecuteNonQuery()
                                                Next
                                            End If

                                            Dim rowRCD As DataRow() = FieldData.Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                            If rowRCD.Length > 0 Then
                                                For Each CField As DataRow In rowRCD

                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("InvCreationDt").ToString() & "'  where tid =" & res & ""
                                                    da.SelectCommand.CommandText = upquery
                                                    da.SelectCommand.Transaction = trans1
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.ExecuteNonQuery()

                                                Next
                                            End If
                                            Dim rowECD As DataRow() = FieldData.Select("DisplayName='Escalation Date' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                            If rowECD.Length > 0 Then
                                                For Each CField As DataRow In rowECD

                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("EscDate").ToString() & "'  where tid =" & res & ""
                                                    da.SelectCommand.CommandText = upquery
                                                    da.SelectCommand.Transaction = trans1
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.ExecuteNonQuery()


                                                Next
                                            End If
                                            'Lease Period Start	
                                            'InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")
                                            Dim LSPARR As String() = Convert.ToString(AmendmentSdt).Split("/")
                                            InvGenDate = LSPARR(1) + "/" + LSPARR(0) + "/" + LSPARR(2)
                                            InvGenDate = CDate(InvGenDate).ToString("dd/MM/yy")

                                            Dim SprowECD As DataRow() = FieldData.Select("DisplayName='Lease Period Start' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                            If rowECD.Length > 0 Then
                                                For Each CField As DataRow In SprowECD

                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & InvGenDate & "'  where tid =" & res & ""
                                                    da.SelectCommand.CommandText = upquery
                                                    da.SelectCommand.Transaction = trans1
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.ExecuteNonQuery()


                                                Next
                                            End If

                                            'Lease Period End	
                                            Dim LPEdate As Date = New Date(CDate(FinalInvDate).Year, CDate(FinalInvDate).Month, 1).AddDays(-1)
                                            Dim LPEdatestr As String = LPEdate.ToString("dd/MM/yy")
                                            Dim EprowECD As DataRow() = FieldData.Select("DisplayName='Lease Period End' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                            If rowECD.Length > 0 Then
                                                For Each CField As DataRow In EprowECD

                                                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LPEdatestr & "'  where tid =" & res & ""
                                                    da.SelectCommand.CommandText = upquery
                                                    da.SelectCommand.Transaction = trans1
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.ExecuteNonQuery()


                                                Next
                                            End If
                                            ' Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)
                                            Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con1, tran:=trans1)

                                            ''INSERT INTO HISTORY 
                                            ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con1, trans1)

                                            Try
                                                UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                da.SelectCommand.CommandText = UpdStr.ToString()
                                                da.SelectCommand.CommandType = CommandType.Text
                                                da.SelectCommand.ExecuteNonQuery()
                                                Dim srerd As String = String.Empty
                                            Catch ex As Exception

                                            End Try
                                            trans1.Commit()

                                            'Check Work Flow
                                            ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.SelectCommand.Parameters.Clear()
                                            da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                            Dim dt1 As New DataTable
                                            da.Fill(dt1)
                                            If dt1.Rows.Count = 1 Then
                                                If dt1.Rows(0).Item(0).ToString = "1" Then
                                                    ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                End If
                                            End If

                                        End If

                                    End If

                                    'End If

                                End If
                            Next

                        End If

                        '  Next
                    End If

                Next
            End If
        Catch ex As Exception
            AutoRunLog("Lease Amendment", "AutoInvoiceAmendLease", "AutoInvoice Function Exception:" + ex.Message.ToString(), 181)
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
            If Not trans1 Is Nothing Then
                trans1.Rollback()
            End If
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try

    End Sub

    Public Function CheckLMSettingsChildInsertion(ByVal eid As Integer, ByVal DOCType As String, ByVal UID As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con1 As SqlConnection = New SqlConnection(conStr)
        Dim ob As New DynamicForm
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran

        Try

            Dim con1 As SqlConnection = New SqlConnection(conStr)
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

            Dim ds As New DataSet
            Dim dt As New DataTable
            da1.Fill(ds, "LMSettingData")

            Dim Fldstr As String = ""
            If ds.Tables("LMSettingData").Rows.Count <> 0 Then


                Dim insStr As String = String.Empty
                Dim res As String = ""
                Dim strTFlds As String = String.Empty
                Dim strSFlds As String = String.Empty
                Dim strHTFlds As String = String.Empty
                Dim strHSFlds As String = String.Empty
                Dim ChildDocT As String = String.Empty
                Dim TDocType As String = String.Empty

                Dim dtdata As New DataTable
                Dim dtdataHeader As New DataTable
                If ds.Tables("LMSettingData").Rows.Count > 0 Then
                    dtdata = ds.Tables("LMSettingData").[Select]("DocType = 'Line'").CopyToDataTable()
                    dtdataHeader = ds.Tables("LMSettingData").[Select]("DocType = 'Header'").CopyToDataTable()

                    If dtdataHeader.Rows.Count > 0 Then

                        strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strHSFldsArr As String()
                        strHSFldsArr = strHSFlds.Split(",")
                        Dim value As String = String.Join(",',',", strHSFldsArr)
                        strHSFlds = "d." + value
                    End If
                    If dtdata.Rows.Count > 0 Then

                        strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strSFldsArr As String()
                        strSFldsArr = strSFlds.Split(",")
                        Dim valueL As String = String.Join(",',',", strSFldsArr)
                        strSFlds = "i." + valueL + ",',',"
                        'For i As Integer = 0 To dtdata.Rows.Count - 1

                        TDocType = dtdata.Rows(0).Item("TDocType")
                        ChildDocT = dtdata.Rows(0).Item("MChildDocType")
                        da.SelectCommand.CommandText = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & DOCType & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.Tid=" & DocID & " "
                        da.Fill(ds, "LMDocData")

                        ' Next

                    End If

                    If ds.Tables("LMDocData").Rows.Count > 0 Then

                        For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1
                            Dim LMFldsVal As String = String.Empty
                            LMFldsVal = ds.Tables("LMDocData").Rows(j).Item("TFldVAl")
                            Dim LMFldsValArr As String()
                            LMFldsValArr = LMFldsVal.Split(",")
                            Dim value As String = String.Join("','", LMFldsValArr)
                            value = "'" + value + "'"


                            insStr = "insert into mmm_mst_master (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & "," & strHTFlds & ",refTid) values ('" & TDocType & "'," & eid & ",1,getdate(),'" & UID & "',getdate(),getdate()," & value & "," & ds.Tables("LMDocData").Rows(j).Item("tid") & " );"
                            da.SelectCommand.CommandText = insStr.ToString()
                            res = da.SelectCommand.ExecuteScalar()

                            ob.HistoryT(eid, res, UID, TDocType, "MMM_MST_MASTER", "ADD", con, tran)
                            'Execute Trigger
                            Try
                                Dim FormName As String = TDocType
                                '     Dim EID As Integer = 0
                                If (res > 0) And (FormName <> "") Then
                                    Trigger.ExecuteTriggerT(FormName, eid, res, con, tran)
                                End If
                            Catch ex As Exception
                                Throw
                            End Try
                        Next

                    End If

                End If
            Else

                da.Dispose()
                da.Dispose()
                Return "fail"
                Return "NA"
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
        End Try
        Return "Success"
    End Function

End Class
