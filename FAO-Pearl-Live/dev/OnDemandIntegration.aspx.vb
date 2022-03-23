Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Renci.SshNet
Partial Class OnDemandIntegration
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'binddldoctype()
            'xrow.Visible = False
        End If
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

    <System.Web.Services.WebMethod()>
    Public Shared Function GetReportName(reportname As String) As kGridIntegration
        Dim jsonData As String = ""
        Dim ret As New kGridIntegration()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim ds As New DataSet
            strQuery1 = "Select TID,ReportSubject + ' (' + reportname + ')'[ReportSubject] from mmm_mst_reportscheduler where  ftpflag in(1,2) and eid=" & HttpContext.Current.Session("EID") & ""
            ds = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetLastUpdateDate(TID As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim ds As New DataSet
            strQuery1 = "Select TID,lastscheduleddate from mmm_mst_reportscheduler where  ftpflag in(1,2) and eid=" & HttpContext.Current.Session("EID") & " and tid='" & TID & "'"
            ds = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            grid.Message = ds.Tables(0).Rows(0).Item("lastscheduleddate").ToString()
            grid.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return grid
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getresult(TID As Integer, sdate As String, edate As String) As DGrid
        Dim grid As New DGrid()
        'If sdate = "null" Or edate = "null" Then
        '    grid.Message = "Please select date first..!"
        '    Return grid
        '    grid.Success = False
        'End If
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select rerunview from mmm_mst_reportscheduler where eid=" & HttpContext.Current.Session("EID") & " and tid=" & TID & "", con)
            Dim dss As New DataSet
            da.Fill(dss, "qryxt")
            If dss.Tables("qryxt").Rows.Count > 0 Then
                Dim objdc As New DataClass()
                Dim qrystr As String = dss.Tables("qryxt").Rows(0).Item(0).ToString()
                qrystr = Replace(qrystr, "@Frdate", sdate)
                qrystr = Replace(qrystr, "@Todate", edate)
                da.SelectCommand.CommandText = qrystr
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dss, "qrydata")
                'jsonData = JsonConvert.SerializeObject(dss.Tables("qrydata"))
                'ret.Data = jsonData
                Dim strError = ""
                grid = DynamicGrid.GridData(dss.Tables("qrydata"), strError)
                If dss.Tables("qrydata").Rows.Count = 0 Then
                    grid.Message = "No data found...!"
                    grid.Success = False
                End If
            End If
        Catch ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0
        End Try
        Return grid
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function ExecuteOnDemand(TID As String) As DGrid
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim FTPType As String = ""
        If (TID = 600) Then
            CreateXMLForAmbitJV_OnDemand()
            Exit Function
        End If
        Try
            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler where ftpflag=2 and tid='" & TID & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            If (ds.Tables("rpt").Rows.Count > 0) Then
                If ds.Tables("rpt").Rows(0).Item("sendto").ToString.ToUpper = "USER" Then
                    eid = ds.Tables("rpt").Rows(0).Item("eid").ToString()
                    oda.SelectCommand.CommandText = "select * from  MMM_MST_Entity where eid='" & eid & "' "
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.Fill(ds, "ftpsettings")
                    Dim ftpadd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpAddress").ToString()
                    Dim ftpid As String = ds.Tables("ftpsettings").Rows(0).Item("FtpID").ToString()
                    Dim ftppwd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpPwd").ToString()
                    Dim cfolder As String = ""
                    If ds.Tables("rpt").Rows(0).Item("RSFolder").ToString() = "" Then
                        cfolder = ds.Tables("ftpsettings").Rows(0).Item("FtpFolder").ToString()
                    Else
                        cfolder = ds.Tables("rpt").Rows(0).Item("RSFolder").ToString()
                    End If

                    Dim str As String = ds.Tables("rpt").Rows(0).Item("reportquery").ToString()
                    FTPType = ds.Tables("ftpsettings").Rows(0).Item("FTPType").ToString()
                    Dim filetype As String = ds.Tables("rpt").Rows(0).Item("sendtype").ToString()
                    Dim ReportTid As String = ds.Tables("rpt").Rows(0).Item("Tid").ToString()
                    Dim FileSeparator As String = ds.Tables("rpt").Rows(0).Item("FileSeparator").ToString()
                    Dim PostFixType As String = ds.Tables("rpt").Rows(0).Item("PostFixType").ToString()
                    Dim FileExtension As String = ds.Tables("rpt").Rows(0).Item("FileExtension").ToString()
                    Dim lastschedule As String = ds.Tables("rpt").Rows(0).Item("LastScheduledDate").ToString
                    lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                    str = Replace(str, "@lastsch", lastschedule)
                    oda.SelectCommand.CommandText = str
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim FTPR As New DataTable
                    oda.Fill(FTPR)
                    If FTPR.Rows.Count > 0 Then
                        InwardFTPREPORT(ReportTid)
                        Dim CNT As Integer = 0
                        Dim msg As String = ds.Tables("rpt").Rows(0).Item("msgbody").ToString()
                        Rptname = ds.Tables("rpt").Rows(0).Item("reportname").ToString()
                        oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "FTP")
                        oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                        oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                        oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "FTPREPORT")
                        oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", ds.Tables("rpt").Rows(0).Item("reportname").ToString())
                        oda.SelectCommand.Parameters.AddWithValue("@EID", ds.Tables("rpt").Rows(0).Item("eid").ToString())
                        oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(0).Item("tid").ToString)
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        oda.SelectCommand.ExecuteNonQuery()
                        Dim fname As String = ""
                        If filetype.ToString.ToString = "XML" Then
                            fname = CreateXMLFTP(FTPR, Rptname)
                            If FTPType.ToUpper = "SFTP" Then
                                CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            Else
                                CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            End If
                        End If
                        If filetype.ToString.ToString = "EXCEL" Then
                            fname = CreateCSVFTP(FTPR, Rptname)
                            If FTPType.ToUpper = "SFTP" Then
                                CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            Else
                                CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            End If

                        End If

                        If filetype.ToString.ToString = "TEXT" Then
                            fname = CreateTextFileFTP(FTPR, Rptname, FileExtension, PostFixType, FileSeparator)

                            If File.Exists(FPath & fname) = True Then
                            Else
                                fname = CreateTextFileFTP(FTPR, Rptname, FileExtension, PostFixType, FileSeparator)
                            End If

                            If FTPType.ToUpper = "SFTP" Then
                                CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            Else
                                CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            End If

                        End If


                    End If

                End If
            End If
            grid.Message = "Executed Successfully"
            grid.Success = True
        Catch ex As Exception
            grid.Message = "Error Occured"
            grid.Success = False
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"

            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Dispose()
        End Try

    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ExecuteRerunIntegration(TID As String, DOCID As String, sdate As String, edate As String) As DGrid
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim FTPType As String = ""
        If (TID = 600) Then
            CreateXMLForAmbitJV_Rerun(DOCID)
            Exit Function
        End If

        Try
            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler where ftpflag=2 and tid='" & TID & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            If (ds.Tables("rpt").Rows.Count > 0) Then
                If ds.Tables("rpt").Rows(0).Item("sendto").ToString.ToUpper = "USER" Then
                    eid = ds.Tables("rpt").Rows(0).Item("eid").ToString()
                    oda.SelectCommand.CommandText = "select * from  MMM_MST_Entity where eid='" & eid & "' "
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.Fill(ds, "ftpsettings")
                    Dim ftpadd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpAddress").ToString()
                    Dim ftpid As String = ds.Tables("ftpsettings").Rows(0).Item("FtpID").ToString()
                    Dim ftppwd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpPwd").ToString()
                    Dim cfolder As String = ""

                    If ds.Tables("rpt").Rows(0).Item("RSFolder").ToString() = "" Then
                        cfolder = ds.Tables("ftpsettings").Rows(0).Item("FtpFolder").ToString()
                    Else
                        cfolder = ds.Tables("rpt").Rows(0).Item("RSFolder").ToString()
                    End If

                    Dim str As String = ds.Tables("rpt").Rows(0).Item("rerunquery").ToString()
                    FTPType = ds.Tables("ftpsettings").Rows(0).Item("FTPType").ToString()
                    Dim filetype As String = ds.Tables("rpt").Rows(0).Item("sendtype").ToString()
                    Dim ReportTid As String = ds.Tables("rpt").Rows(0).Item("Tid").ToString()
                    Dim FileSeparator As String = ds.Tables("rpt").Rows(0).Item("FileSeparator").ToString()
                    Dim PostFixType As String = ds.Tables("rpt").Rows(0).Item("PostFixType").ToString()
                    Dim FileExtension As String = ds.Tables("rpt").Rows(0).Item("FileExtension").ToString()
                    Dim lastschedule As String = ds.Tables("rpt").Rows(0).Item("LastScheduledDate").ToString
                    lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                    str = Replace(str, "@lastsch", lastschedule)
                    'for daterange rerun
                    'str = Replace(str, "@Frdate", sdate)
                    'str = Replace(str, "@Todate", edate)
                    str = Replace(str, "@DOCID", DOCID)
                    oda.SelectCommand.CommandText = str
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim FTPR As New DataTable
                    oda.Fill(FTPR)
                    If FTPR.Rows.Count > 0 Then
                        InwardFTPREPORT(ReportTid)
                        Dim CNT As Integer = 0
                        Dim msg As String = ds.Tables("rpt").Rows(0).Item("msgbody").ToString()
                        Rptname = ds.Tables("rpt").Rows(0).Item("reportname").ToString()
                        oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "FTP")
                        oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                        oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                        oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "FTPREPORT")
                        oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", ds.Tables("rpt").Rows(0).Item("reportname").ToString())
                        oda.SelectCommand.Parameters.AddWithValue("@EID", ds.Tables("rpt").Rows(0).Item("eid").ToString())
                        oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(0).Item("tid").ToString)
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        oda.SelectCommand.ExecuteNonQuery()
                        Dim fname As String = ""
                        If filetype.ToString.ToString = "XML" Then
                            fname = CreateXMLFTP(FTPR, Rptname)
                            If FTPType.ToUpper = "SFTP" Then
                                CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            Else
                                CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            End If
                        End If
                        If filetype.ToString.ToString = "EXCEL" Then
                            fname = CreateCSVFTP(FTPR, Rptname)
                            If FTPType.ToUpper = "SFTP" Then
                                CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            Else
                                '  CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                                CopyFileToFTPS(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            End If

                        End If
                        If filetype.ToString.ToString = "TEXT" Then
                            fname = CreateTextFileFTP(FTPR, Rptname, FileExtension, PostFixType, FileSeparator)

                            If File.Exists(FPath & fname) = True Then
                            Else
                                fname = CreateTextFileFTP(FTPR, Rptname, FileExtension, PostFixType, FileSeparator)
                            End If

                            If FTPType.ToUpper = "SFTP" Then
                                CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            Else
                                CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, fname, cfolder)
                            End If

                        End If

                    End If

                End If
            End If
            grid.Message = "Executed Successfully"
            grid.Success = True
        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Dispose()
        End Try

    End Function
    Public Shared Function CreateXMLForAmbitJV_Rerun(DOCID As String)
        Dim sp As System.Text.StringBuilder = New System.Text.StringBuilder()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim RptSub As String = ""
        Dim FTPType As String = ""
        Dim MAILTO As String = ""
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
        Dim strxml As String = ""
        oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler with(nolock) where isactive=1 and ftpflag=1 and Tid=600 "
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        ' If ReportScheduler(ds.Tables("rpt").Rows(0).Item("tid")) = True Then
        If ds.Tables("rpt").Rows.Count > 0 Then
            eid = ds.Tables("rpt").Rows(0).Item("eid").ToString()
            RptSub = ds.Tables("rpt").Rows(0).Item("reportsubject").ToString()
            Rptname = ds.Tables("rpt").Rows(0).Item("reportname").ToString()
            MAILTO = ds.Tables("rpt").Rows(0).Item("emailto").ToString()
            Dim ReportqryStr = ds.Tables("rpt").Rows(0).Item("rerunquery").ToString()
            Dim ftpadd As String = ds.Tables("rpt").Rows(0).Item("FtpAddress").ToString()
            Dim ftpid As String = ds.Tables("rpt").Rows(0).Item("FtpID").ToString()
            Dim ftppwd As String = ds.Tables("rpt").Rows(0).Item("FtpPwd").ToString()
            Dim cfolder As String = ds.Tables("rpt").Rows(0).Item("FtpFolder").ToString()
            Dim msg = ds.Tables("rpt").Rows(0).Item("msgbody").ToString()
            Dim cc = ds.Tables("rpt").Rows(0).Item("cc").ToString()
            FTPType = ds.Tables("rpt").Rows(0).Item("FTPType").ToString()
            Dim lastschedule As String = ds.Tables("rpt").Rows(0).Item("LastScheduledDate").ToString
            ReportqryStr = Replace(ReportqryStr, "@DOCID", DOCID)
            Dim fname As String = ""
            oda.SelectCommand.CommandText = ReportqryStr
            oda.SelectCommand.CommandType = CommandType.Text
            Dim FTPR As New DataTable
            oda.Fill(FTPR)
            Dim distinctDT As DataTable = FTPR.DefaultView.ToTable(True, "BusinessUnit")

            For j As Integer = 0 To distinctDT.Rows.Count - 1
                '    For i As Integer = 0 To FTPR.Rows.Count - 1
                strxml += "<?xml version=""1.0"" encoding=""UTF-8""?>"
                strxml += "<SSC>"
                strxml += "<User><Name>PK1</Name></User>"
                strxml += "<SunSystemsContext><BusinessUnit>SMS</BusinessUnit><BudgetCode>A</BudgetCode></SunSystemsContext>"
                strxml += "<MethodContext>"
                'start for loop for LedgerPostingParameters
                ' For i As Integer = 0 To FTPR.Rows.Count - 1
                strxml += "<LedgerPostingParameters>"
                strxml += "<Description>Journal Number</Description>"
                strxml += "<JournalType>MGENV</JournalType>"
                strxml += "<PostingType>2</PostingType>"
                strxml += "<ReportingAccount>99999999</ReportingAccount>"
                strxml += "<SuspenseAccount>99999999</SuspenseAccount>"
                strxml += "<TransactionAmountAccount>99999999</TransactionAmountAccount>"
                strxml += "<CalculateDebitCreditMarker>Y</CalculateDebitCreditMarker>"
                strxml += "<ReportErrorsOnly>Y</ReportErrorsOnly>"
                strxml += "<SuppressSubstitutedMessages>Y</SuppressSubstitutedMessages>"
                strxml += "<CreateMultipleJournals>Y</CreateMultipleJournals>"
                strxml += "</LedgerPostingParameters>"
                ' Next
                'end for loop for LedgerPostingParameters
                strxml += "</MethodContext>"
                strxml += "<Payload>"
                strxml += "<Ledger>"
                'start for loop for LINE
                Dim dv As New DataView(FTPR)
                dv.RowFilter = "BusinessUnit ='" & distinctDT.Rows(j).Item(0) & "'"
                fname = distinctDT.Rows(j).Item(0).ToString & Rptname & Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & ".xml"
                Dim Bufiltertbl As DataTable = dv.ToTable()
                For i As Integer = 0 To Bufiltertbl.Rows.Count - 1

                    strxml += "<Line>"
                    strxml += "<AccountCode>" & Bufiltertbl.Rows(i).Item("AccountCode") & "</AccountCode>"
                    strxml += "<AccountingPeriod>" & Bufiltertbl.Rows(i).Item("AccountingPeriod") & "</AccountingPeriod>"
                    strxml += "<TransactionDate>" & Bufiltertbl.Rows(i).Item("TransactionDate") & "</TransactionDate>"
                    'strxml += "<JournalNumber>1</JournalNumber>"
                    strxml += "<JournalNumber>" & Bufiltertbl.Rows(i).Item("JournalNum") & "</JournalNumber>"
                    strxml += "<JournalLineNumber>0</JournalLineNumber>"
                    strxml += "<JournalType>" & Bufiltertbl.Rows(i).Item("JournalType") & "</JournalType>"
                    strxml += "<TransactionReference>" & Bufiltertbl.Rows(i).Item("TransactionReference") & "</TransactionReference>"
                    strxml += "<Description>" & Bufiltertbl.Rows(i).Item("GeneralDescription13") & "</Description>"
                    'strxml += "<Description>" & FTPR.Rows(i).Item("Description") & "</Description>"
                    strxml += "<BaseAmount>" & Bufiltertbl.Rows(i).Item("BaseAmount") & "</BaseAmount>"
                    strxml += "<DebitCredit>" & Bufiltertbl.Rows(i).Item("DebitCredit") & "</DebitCredit>"
                    strxml += "<CurrencyCode>" & Bufiltertbl.Rows(i).Item("CurrencyCode") & "</CurrencyCode>"
                    strxml += "<TransactionAmount>" & Bufiltertbl.Rows(i).Item("TransactionAmount") & "</TransactionAmount>"
                    strxml += "<TransactionAmountDecimalPlaces>" & Bufiltertbl.Rows(i).Item("TransactionAmountDecimalPlaces") & "</TransactionAmountDecimalPlaces>"
                    strxml += "<Base2ReportingAmount>" & Bufiltertbl.Rows(i).Item("Base2ReportingAmount") & "</Base2ReportingAmount>"
                    strxml += "<MemoAmount>" & Bufiltertbl.Rows(i).Item("MemoAmount") & "</MemoAmount>"
                    strxml += "<EntryDate>" & Bufiltertbl.Rows(i).Item("TransactionDate") & "</EntryDate>"
                    strxml += "<PermanemtPostingDate>" & Bufiltertbl.Rows(i).Item("TransactionDate") & "</PermanemtPostingDate>"
                    strxml += "<AnalysisCode1>" & Bufiltertbl.Rows(i).Item("AnalysisCode1") & "</AnalysisCode1>"
                    strxml += "<AnalysisCode2>" & Bufiltertbl.Rows(i).Item("AnalysisCode2") & "</AnalysisCode2>"
                    strxml += "<AnalysisCode3>" & Bufiltertbl.Rows(i).Item("AnalysisCode3") & "</AnalysisCode3>"
                    strxml += "<AnalysisCode4>" & Bufiltertbl.Rows(i).Item("AnalysisCode4") & "</AnalysisCode4>"
                    strxml += "<AnalysisCode5>" & Bufiltertbl.Rows(i).Item("AnalysisCode5") & "</AnalysisCode5>"
                    strxml += "<AnalysisCode6>" & Bufiltertbl.Rows(i).Item("AnalysisCode6") & "</AnalysisCode6>"
                    strxml += "<AnalysisCode7>" & Bufiltertbl.Rows(i).Item("AnalysisCode7") & "</AnalysisCode7>"
                    strxml += "<AnalysisCode8>" & Bufiltertbl.Rows(i).Item("AnalysisCode8") & "</AnalysisCode8>"
                    strxml += "<AnalysisCode9/>"
                    strxml += "<AnalysisCode10/>"

                    strxml += "<GeneralDescription5>" & Bufiltertbl.Rows(i).Item("GeneralDescription5") & "</GeneralDescription5>"
                    strxml += "<GeneralDescription6>" & Bufiltertbl.Rows(i).Item("GeneralDescription6") & "</GeneralDescription6>"
                    strxml += "<GeneralDescription7>" & Bufiltertbl.Rows(i).Item("GeneralDescription7") & "</GeneralDescription7>"
                    strxml += "<GeneralDescription8>" & Bufiltertbl.Rows(i).Item("GeneralDescription8") & "</GeneralDescription8>"
                    strxml += "<GeneralDescription9>" & Bufiltertbl.Rows(i).Item("GeneralDescription9") & "</GeneralDescription9>"
                    strxml += "<GeneralDescription10>" & Bufiltertbl.Rows(i).Item("GeneralDescription10") & "</GeneralDescription10>"
                    strxml += "<GeneralDescription11>" & Bufiltertbl.Rows(i).Item("GeneralDescription11") & "</GeneralDescription11>"
                    strxml += "<GeneralDescription12>" & Bufiltertbl.Rows(i).Item("GeneralDescription12") & "</GeneralDescription12>"
                    strxml += "<GeneralDescription13>" & Bufiltertbl.Rows(i).Item("PearlID") & "</GeneralDescription13>"
                    strxml += "<GeneralDescription14>" & Bufiltertbl.Rows(i).Item("GeneralDescription14") & "</GeneralDescription14>"


                    'strxml += "<DetailLad/>"
                    strxml += "<Extension/>"
                    strxml += "<PrincipalCodes />"
                    strxml += "<StandardText/>"
                    strxml += "<LedgerNote/>"
                    strxml += "<DetailLad>"
                    strxml += "<GeneralDescription15>" & Bufiltertbl.Rows(i).Item("InvoiceType") & "</GeneralDescription15>"
                    'strxml += "<Description>" & FTPR.Rows(i).Item("PearlID") & "</Description>"
                    'strxml += "<TransactionReference>" & FTPR.Rows(i).Item("InvoiceNumber") & "</TransactionReference>"
                    strxml += "<GeneralDate3>" & Bufiltertbl.Rows(i).Item("InvoiceDate") & "</GeneralDate3>"
                    strxml += "</DetailLad>"
                    strxml += "</Line>"

                Next
                'end code for LINE
                strxml += "</Ledger>"
                strxml += "</Payload>"
                strxml += "<SunSystemsContext>"
                strxml += "<BusinessUnit>SMS</BusinessUnit>"
                strxml += "<BudgetCode>A</BudgetCode>"
                strxml += "</SunSystemsContext>"
                strxml += "<SunSystemsContext>"
                strxml += "<BusinessUnit>SMS</BusinessUnit>"
                strxml += "<BudgetCode>A</BudgetCode>"
                strxml += "</SunSystemsContext>"
                strxml += "</SSC>"
                '  Next
                strxml = strxml.Replace("&", "&amp;")
                strxml = strxml.Replace("'", "&apos;")
                strxml = strxml.Replace("\""", "&quot;")
                Dim targetFI As New FileInfo(FPath + "\" + fname)
                If targetFI.Exists Then
                    targetFI.Delete()
                End If
                Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
                sw.Flush()
                sw.WriteLine(strxml)
                sw.Close()
                CopyfiletoInboxAmbit(ftpadd, ftpid, ftppwd, FPath, fname, cfolder, eid, Rptname, MAILTO, RptSub, msg, cc)
                strxml = ""
            Next


            Try

                oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "FTP")
                oda.SelectCommand.Parameters.AddWithValue("@CC", Rptname)
                oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                oda.SelectCommand.Parameters.AddWithValue("@Sdate", lastschedule)
                oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "FTPREPORT")
                oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", Rptname)
                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(0).Item("tid").ToString)
                oda.SelectCommand.Parameters.AddWithValue("@Dcnt", 0)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                Throw ex
            Finally
                oda.Dispose()
                con.Close()
                con.Dispose()
            End Try
        End If
    End Function
    Public Shared Function CreateXMLForAmbitJV_OnDemand()
        Dim sp As System.Text.StringBuilder = New System.Text.StringBuilder()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim RptSub As String = ""
        Dim FTPType As String = ""
        Dim MAILTO As String = ""
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
        Dim strxml As String = ""
        oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler with(nolock) where isactive=1 and ftpflag=1 and Tid=600 "
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        ' If ReportScheduler(ds.Tables("rpt").Rows(0).Item("tid")) = True Then
        If ds.Tables("rpt").Rows.Count > 0 Then
            eid = ds.Tables("rpt").Rows(0).Item("eid").ToString()
            RptSub = ds.Tables("rpt").Rows(0).Item("reportsubject").ToString()
            Rptname = ds.Tables("rpt").Rows(0).Item("reportname").ToString()
            MAILTO = ds.Tables("rpt").Rows(0).Item("emailto").ToString()
            Dim ReportqryStr = ds.Tables("rpt").Rows(0).Item("reportquery").ToString()
            Dim ftpadd As String = ds.Tables("rpt").Rows(0).Item("FtpAddress").ToString()
            Dim ftpid As String = ds.Tables("rpt").Rows(0).Item("FtpID").ToString()
            Dim ftppwd As String = ds.Tables("rpt").Rows(0).Item("FtpPwd").ToString()
            Dim cfolder As String = ds.Tables("rpt").Rows(0).Item("FtpFolder").ToString()
            Dim msg = ds.Tables("rpt").Rows(0).Item("msgbody").ToString()
            Dim cc = ds.Tables("rpt").Rows(0).Item("cc").ToString()
            FTPType = ds.Tables("rpt").Rows(0).Item("FTPType").ToString()
            Dim lastschedule As String = ds.Tables("rpt").Rows(0).Item("LastScheduledDate").ToString
            Dim fname As String = ""
            oda.SelectCommand.CommandText = ReportqryStr
            oda.SelectCommand.CommandType = CommandType.Text
            Dim FTPR As New DataTable
            oda.Fill(FTPR)
            Dim distinctDT As DataTable = FTPR.DefaultView.ToTable(True, "BusinessUnit")

            For j As Integer = 0 To distinctDT.Rows.Count - 1
                '    For i As Integer = 0 To FTPR.Rows.Count - 1
                strxml += "<?xml version=""1.0"" encoding=""UTF-8""?>"
                strxml += "<SSC>"
                strxml += "<User><Name>PK1</Name></User>"
                strxml += "<SunSystemsContext><BusinessUnit>SMS</BusinessUnit><BudgetCode>A</BudgetCode></SunSystemsContext>"
                strxml += "<MethodContext>"
                'start for loop for LedgerPostingParameters
                ' For i As Integer = 0 To FTPR.Rows.Count - 1
                strxml += "<LedgerPostingParameters>"
                strxml += "<Description>Journal Number</Description>"
                strxml += "<JournalType>MGENV</JournalType>"
                strxml += "<PostingType>2</PostingType>"
                strxml += "<ReportingAccount>99999999</ReportingAccount>"
                strxml += "<SuspenseAccount>99999999</SuspenseAccount>"
                strxml += "<TransactionAmountAccount>99999999</TransactionAmountAccount>"
                strxml += "<CalculateDebitCreditMarker>Y</CalculateDebitCreditMarker>"
                strxml += "<ReportErrorsOnly>Y</ReportErrorsOnly>"
                strxml += "<SuppressSubstitutedMessages>Y</SuppressSubstitutedMessages>"
                strxml += "<CreateMultipleJournals>Y</CreateMultipleJournals>"
                strxml += "</LedgerPostingParameters>"
                ' Next
                'end for loop for LedgerPostingParameters
                strxml += "</MethodContext>"
                strxml += "<Payload>"
                strxml += "<Ledger>"
                'start for loop for LINE
                Dim dv As New DataView(FTPR)
                dv.RowFilter = "BusinessUnit ='" & distinctDT.Rows(j).Item(0) & "'"
                fname = distinctDT.Rows(j).Item(0).ToString & Rptname & Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & ".xml"
                Dim Bufiltertbl As DataTable = dv.ToTable()
                For i As Integer = 0 To Bufiltertbl.Rows.Count - 1

                    strxml += "<Line>"
                    strxml += "<AccountCode>" & Bufiltertbl.Rows(i).Item("AccountCode") & "</AccountCode>"
                    strxml += "<AccountingPeriod>" & Bufiltertbl.Rows(i).Item("AccountingPeriod") & "</AccountingPeriod>"
                    strxml += "<TransactionDate>" & Bufiltertbl.Rows(i).Item("TransactionDate") & "</TransactionDate>"
                    'strxml += "<JournalNumber>1</JournalNumber>"
                    strxml += "<JournalNumber>" & Bufiltertbl.Rows(i).Item("JournalNum") & "</JournalNumber>"
                    strxml += "<JournalLineNumber>0</JournalLineNumber>"
                    strxml += "<JournalType>" & Bufiltertbl.Rows(i).Item("JournalType") & "</JournalType>"
                    strxml += "<TransactionReference>" & Bufiltertbl.Rows(i).Item("TransactionReference") & "</TransactionReference>"
                    strxml += "<Description>" & Bufiltertbl.Rows(i).Item("GeneralDescription13") & "</Description>"
                    'strxml += "<Description>" & FTPR.Rows(i).Item("Description") & "</Description>"
                    strxml += "<BaseAmount>" & Bufiltertbl.Rows(i).Item("BaseAmount") & "</BaseAmount>"
                    strxml += "<DebitCredit>" & Bufiltertbl.Rows(i).Item("DebitCredit") & "</DebitCredit>"
                    strxml += "<CurrencyCode>" & Bufiltertbl.Rows(i).Item("CurrencyCode") & "</CurrencyCode>"
                    strxml += "<TransactionAmount>" & Bufiltertbl.Rows(i).Item("TransactionAmount") & "</TransactionAmount>"
                    strxml += "<TransactionAmountDecimalPlaces>" & Bufiltertbl.Rows(i).Item("TransactionAmountDecimalPlaces") & "</TransactionAmountDecimalPlaces>"
                    strxml += "<Base2ReportingAmount>" & Bufiltertbl.Rows(i).Item("Base2ReportingAmount") & "</Base2ReportingAmount>"
                    strxml += "<MemoAmount>" & Bufiltertbl.Rows(i).Item("MemoAmount") & "</MemoAmount>"
                    strxml += "<EntryDate>" & Bufiltertbl.Rows(i).Item("TransactionDate") & "</EntryDate>"
                    strxml += "<PermanemtPostingDate>" & Bufiltertbl.Rows(i).Item("TransactionDate") & "</PermanemtPostingDate>"
                    strxml += "<AnalysisCode1>" & Bufiltertbl.Rows(i).Item("AnalysisCode1") & "</AnalysisCode1>"
                    strxml += "<AnalysisCode2>" & Bufiltertbl.Rows(i).Item("AnalysisCode2") & "</AnalysisCode2>"
                    strxml += "<AnalysisCode3>" & Bufiltertbl.Rows(i).Item("AnalysisCode3") & "</AnalysisCode3>"
                    strxml += "<AnalysisCode4>" & Bufiltertbl.Rows(i).Item("AnalysisCode4") & "</AnalysisCode4>"
                    strxml += "<AnalysisCode5>" & Bufiltertbl.Rows(i).Item("AnalysisCode5") & "</AnalysisCode5>"
                    strxml += "<AnalysisCode6>" & Bufiltertbl.Rows(i).Item("AnalysisCode6") & "</AnalysisCode6>"
                    strxml += "<AnalysisCode7>" & Bufiltertbl.Rows(i).Item("AnalysisCode7") & "</AnalysisCode7>"
                    strxml += "<AnalysisCode8>" & Bufiltertbl.Rows(i).Item("AnalysisCode8") & "</AnalysisCode8>"
                    strxml += "<AnalysisCode9/>"
                    strxml += "<AnalysisCode10/>"

                    strxml += "<GeneralDescription5>" & Bufiltertbl.Rows(i).Item("GeneralDescription5") & "</GeneralDescription5>"
                    strxml += "<GeneralDescription6>" & Bufiltertbl.Rows(i).Item("GeneralDescription6") & "</GeneralDescription6>"
                    strxml += "<GeneralDescription7>" & Bufiltertbl.Rows(i).Item("GeneralDescription7") & "</GeneralDescription7>"
                    strxml += "<GeneralDescription8>" & Bufiltertbl.Rows(i).Item("GeneralDescription8") & "</GeneralDescription8>"
                    strxml += "<GeneralDescription9>" & Bufiltertbl.Rows(i).Item("GeneralDescription9") & "</GeneralDescription9>"
                    strxml += "<GeneralDescription10>" & Bufiltertbl.Rows(i).Item("GeneralDescription10") & "</GeneralDescription10>"
                    strxml += "<GeneralDescription11>" & Bufiltertbl.Rows(i).Item("GeneralDescription11") & "</GeneralDescription11>"
                    strxml += "<GeneralDescription12>" & Bufiltertbl.Rows(i).Item("GeneralDescription12") & "</GeneralDescription12>"
                    strxml += "<GeneralDescription13>" & Bufiltertbl.Rows(i).Item("PearlID") & "</GeneralDescription13>"
                    strxml += "<GeneralDescription14>" & Bufiltertbl.Rows(i).Item("GeneralDescription14") & "</GeneralDescription14>"


                    'strxml += "<DetailLad/>"
                    strxml += "<Extension/>"
                    strxml += "<PrincipalCodes />"
                    strxml += "<StandardText/>"
                    strxml += "<LedgerNote/>"
                    strxml += "<DetailLad>"
                    strxml += "<GeneralDescription15>" & Bufiltertbl.Rows(i).Item("InvoiceType") & "</GeneralDescription15>"
                    'strxml += "<Description>" & FTPR.Rows(i).Item("PearlID") & "</Description>"
                    'strxml += "<TransactionReference>" & FTPR.Rows(i).Item("InvoiceNumber") & "</TransactionReference>"
                    strxml += "<GeneralDate3>" & Bufiltertbl.Rows(i).Item("InvoiceDate") & "</GeneralDate3>"
                    strxml += "</DetailLad>"
                    strxml += "</Line>"

                Next
                'end code for LINE
                strxml += "</Ledger>"
                strxml += "</Payload>"
                strxml += "<SunSystemsContext>"
                strxml += "<BusinessUnit>SMS</BusinessUnit>"
                strxml += "<BudgetCode>A</BudgetCode>"
                strxml += "</SunSystemsContext>"
                strxml += "<SunSystemsContext>"
                strxml += "<BusinessUnit>SMS</BusinessUnit>"
                strxml += "<BudgetCode>A</BudgetCode>"
                strxml += "</SunSystemsContext>"
                strxml += "</SSC>"
                '  Next
                strxml = strxml.Replace("&", "&amp;")
                strxml = strxml.Replace("'", "&apos;")
                strxml = strxml.Replace("\""", "&quot;")
                Dim targetFI As New FileInfo(FPath + "\" + fname)
                If targetFI.Exists Then
                    targetFI.Delete()
                End If
                Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
                sw.Flush()
                sw.WriteLine(strxml)
                sw.Close()
                CopyfiletoInboxAmbit(ftpadd, ftpid, ftppwd, FPath, fname, cfolder, eid, Rptname, MAILTO, RptSub, msg, cc)
                strxml = ""
            Next


            Try

                oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "FTP")
                oda.SelectCommand.Parameters.AddWithValue("@CC", Rptname)
                oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                oda.SelectCommand.Parameters.AddWithValue("@Sdate", lastschedule)
                oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "FTPREPORT")
                oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", Rptname)
                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(0).Item("tid").ToString)
                oda.SelectCommand.Parameters.AddWithValue("@Dcnt", 0)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                Throw ex
            Finally
                oda.Dispose()
                con.Close()
                con.Dispose()
            End Try
        End If
    End Function
    Public Shared Function CopyfiletoInboxAmbit(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String, ByVal eid As String, ByVal RptName As String, ByVal MAILTO As String, ByVal RptSub As String, ByVal msg As String, ByVal CC As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim URI As String = Fadd & "/" & filenm

            Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(URI), System.Net.FtpWebRequest)

            ''''ftp://103.25.172.228/FORMYND/

            clsRequest.Credentials = New System.Net.NetworkCredential(login, pwd)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile

            ' read in file...
            'Dim file() As Byte = System.IO.File.ReadAllBytes()
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(readPath & filenm)
            ' upload file...
            Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()
            clsStream.Write(bFile, 0, bFile.Length)
            clsStream.Close()
            clsStream.Dispose()
            clsRequest.Abort()

            ''Retry if file is not tranfered to FTP 
            Dim Rtrn As Boolean = CheckIfFileExistsOnServer(URI, login, pwd)
            If Rtrn = False Then
                Dim clsRequest1 As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(URI), System.Net.FtpWebRequest)
                clsRequest1.Credentials = New System.Net.NetworkCredential(login, pwd)
                clsRequest1.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                Dim bFile1() As Byte = System.IO.File.ReadAllBytes(readPath & filenm)
                Dim clsStream1 As System.IO.Stream = clsRequest1.GetRequestStream()
                clsStream1.Write(bFile, 0, bFile.Length)
                clsStream1.Close()
                clsStream1.Dispose()
                clsRequest1.Abort()
            Else
                oda.SelectCommand.CommandText = "INSERT_FILELOG"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                oda.SelectCommand.Parameters.AddWithValue("@FNAME", filenm)
                oda.SelectCommand.Parameters.AddWithValue("@ReportName", RptName)
                oda.SelectCommand.Parameters.AddWithValue("@FileType", "FTP")
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            End If

        Catch ex As Exception
            Dim obj As New MailUtill(EID:=eid)
            If MAILTO.ToString.Trim <> "" Then
                obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " with filename (" & filenm & ") Integration Failed", MailBody:=msg, CC:=CC, Attachments:="", BCC:="")
            End If
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FTPREPORT " & Fadd)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function
    'New


    Public Shared Function CheckIfFileExistsOnServer(ByVal URI As String, ByVal login As String, ByVal pwd As String) As Boolean

        Dim request = CType(WebRequest.Create(URI), FtpWebRequest)
        request.Credentials = New NetworkCredential(login, pwd)
        request.Method = WebRequestMethods.Ftp.GetFileSize

        Try
            Dim response As FtpWebResponse = CType(request.GetResponse(), FtpWebResponse)
            Return True
        Catch ex As WebException
            Dim response As FtpWebResponse = CType(ex.Response, FtpWebResponse)
            If response.StatusCode = FtpStatusCode.ActionNotTakenFileUnavailable Then Return False
        End Try

        Return False
    End Function
    Public Shared Function CreateTextFileFTP(ByVal dt As DataTable, ByVal ReportName As String, Optional ByVal FileExtension As String = "", Optional ByVal PostFixType As String = "", Optional ByVal FileSeparator As String = "") As String
        Dim month As String = Now.Month.ToString()
        Dim day As String = Now.Day.ToString()
        Dim milisec As String = Now.Millisecond.ToString()
        If Len(milisec) > 2 Then
            milisec = milisec.Remove(milisec.Length - 1)
        End If
        Dim fname As String = ""
        fname = ReportName & "_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".txt"
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
        If File.Exists(FPath & fname) Then
            File.Delete(fname)
        End If
        Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
        sw.Flush()
        Dim iColCount As Integer = dt.Columns.Count
        If FileSeparator = "" Then
            For i As Integer = 0 To iColCount - 1
                sw.Write(dt.Columns(i))

                If (i < iColCount - 1) Then
                    sw.Write("|")
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
                        sw.Write("|")
                    End If
                Next
                sw.Write(sw.NewLine)
            Next
        ElseIf FileSeparator.ToUpper = "PIPE" Then
            For i As Integer = 0 To iColCount - 1
                sw.Write(dt.Columns(i))

                If (i < iColCount - 1) Then
                    sw.Write("|")
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
                        sw.Write("|")
                    End If
                Next
                sw.Write(sw.NewLine)
            Next
        ElseIf FileSeparator.ToUpper = "TAB" Then
            For i As Integer = 0 To iColCount - 1
                sw.Write(dt.Columns(i))

                If (i < iColCount - 1) Then
                    sw.Write(vbTab)
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
                        sw.Write(vbTab)
                    End If
                Next
                sw.Write(sw.NewLine)
            Next
        ElseIf FileSeparator.ToUpper = "COMMA" Then
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
        End If
        sw.Close()
        Return fname
    End Function
    Public Shared Function CopyfiletoInbox(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try

            Dim URI As String = Fadd & "/" & filenm
            Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(URI), System.Net.FtpWebRequest)
            'ftp://103.25.172.228/FORHCL
            'ftp://103.25.172.228/FORMYND/
            clsRequest.Credentials = New System.Net.NetworkCredential(login, pwd)

            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            'clsRequest.EnableSsl = True
            'clsRequest.UsePassive = False
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(readPath & filenm)
            ' upload file...
            Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()
            clsStream.Write(bFile, 0, bFile.Length)
            clsStream.Close()
            clsStream.Dispose()
        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FTPREPORT " & Fadd)
            oda.SelectCommand.Parameters.AddWithValue("@EID", 46)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function

    'New

    Public Shared Function CopyFileToFTPS(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String) As Integer
        Dim ftpServerIP As String = "ambitftp.ambit.co" 'IP address
        Dim ftpUserID As String = "APLPearl"
        Dim ftpPassword As String = "Pearl321"
        Dim fileInfo As FileInfo = New FileInfo(filenm)
        Dim uri As String = "ftp://" & ftpServerIP & "/" & fileInfo.Name
        Dim reqFTP As FtpWebRequest

        Try
            reqFTP = CType(FtpWebRequest.Create(New Uri("ftp://" & ftpServerIP & "/" & fileInfo.Name)), FtpWebRequest)
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            reqFTP.KeepAlive = False
            reqFTP.EnableSsl = True
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile
            reqFTP.UseBinary = True
        Catch
            Return 0
        End Try

        reqFTP.ContentLength = fileInfo.Length
        Dim buffLength As Integer = 2048
        Dim buff As Byte() = New Byte(buffLength - 1) {}
        Dim contentLen As Integer
        Dim fs As FileStream = fileInfo.OpenRead()

        Try
            Dim strm As Stream = reqFTP.GetRequestStream()
            contentLen = fs.Read(buff, 0, buffLength)

            While contentLen <> 0
                strm.Write(buff, 0, contentLen)
                contentLen = fs.Read(buff, 0, buffLength)
            End While

            strm.Close()
            fs.Close()
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            response.Close()
            Return 0
        Catch
            fs.Close()
            fs.Dispose()
            Return 1
        End Try
    End Function
    Public Shared Function CopyfiletoSFTP(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim URI As String = Fadd & "/" & filenm
            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/" & filenm)
            Dim client As SftpClient = New SftpClient(Fadd, login, pwd)
            client.Connect()
            Using stream As Stream = File.OpenRead(readPath & filenm)
                client.UploadFile(stream, cfoldernm & filenm)
            End Using
            Return "Success"
        Catch ex As Exception
            Return "Fail"
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "SFTPREPORT " & Fadd)
            oda.SelectCommand.Parameters.AddWithValue("@EID", 46)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            If con.State <> ConnectionState.Closed Then
                con.Close()
            End If
        End Try
    End Function
    Public Shared Function CreateCSVFTP(ByVal dt As DataTable, ByVal ReportName As String) As String
        Dim month As String = Now.Month.ToString()
        Dim day As String = Now.Day.ToString()
        Dim milisec As String = Now.Millisecond.ToString()

        If Len(milisec) > 2 Then
            milisec = milisec.Remove(milisec.Length - 1)
        End If

        Dim fname As String = ""
        If ReportName = "Parking Report" Then
            fname = "veninvpark_msg_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".txt"
        ElseIf ReportName = "Customer Collection" Then
            fname = "custcoll_e_msg_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".txt"
        ElseIf ReportName = "Customer Collection Non Eft" Then
            fname = "Custcoll_n_msg_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".txt"
        ElseIf ReportName = "PCMS_KR Report" Then
            fname = "PCMS_KR_" & day.PadLeft(2, "0") & Now.Month.ToString.PadLeft(2, "0") & Now.Year.ToString & ".txt"
        ElseIf ReportName = "PCMS_SA Report" Then
            fname = "PCMS_SA_" & day.PadLeft(2, "0") & Now.Month.ToString.PadLeft(2, "0") & Now.Year.ToString & ".txt"
        Else
            fname = ReportName & "_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".CSV"
        End If

        'Dim FPath As String = My.Application.Info.DirectoryPath & "/MailAttach/"
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
        If File.Exists(FPath & fname) Then
            File.Delete(fname)
        End If
        Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
        Dim hw As New HtmlTextWriter(sw)
        If fname.Contains("xls") = True Then
            Dim iColCount As Integer = dt.Columns.Count
            Dim grd As New GridView
            grd.DataSource = dt
            grd.DataBind()
            For i As Integer = 0 To grd.Rows.Count - 1
                grd.Rows(i).Attributes.Add("class", "textmode")
                'Apply text style to each Row 
            Next
            'grd.CssClass = style
            grd.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\""0000""; } </style>"
            sw.WriteLine(style)
            sw.Write(sw.NewLine)
            sw.Close()
        Else
            sw.Flush()
            Dim iColCount As Integer = dt.Columns.Count
            If ReportName = "PCMS_KR Report" Or ReportName = "PCMS_SA Report" Then
                For i As Integer = 0 To iColCount - 1
                    sw.Write(dt.Columns(i))

                    If (i < iColCount - 1) Then
                        sw.Write(vbTab)
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
                            sw.Write(vbTab)
                        End If
                    Next
                    sw.Write(sw.NewLine)
                Next
            Else
                For i As Integer = 0 To iColCount - 1
                    sw.Write(dt.Columns(i))

                    If (i < iColCount - 1) Then
                        sw.Write("|")
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
                            sw.Write("|")
                        End If
                    Next
                    sw.Write(sw.NewLine)
                Next

            End If

            'First we will write the headers.


            sw.Close()
        End If
        Return fname
    End Function

    Public Shared Function CreateXMLFTP(ByVal dt As DataTable, ByVal ReportName As String) As String
        Dim month As String = Now.Month.ToString()
        Dim day As String = Now.Day.ToString()
        Dim milisec As String = Now.Millisecond.ToString()

        If Len(milisec) > 2 Then
            milisec = milisec.Remove(milisec.Length - 1)
        End If

        Dim fname As String = ""
        If ReportName = "Parking Report" Then
            fname = "veninvpark_msg_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".xml"
        ElseIf ReportName = "Customer Collection" Then
            fname = "custcoll_e_msg_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".xml"
        ElseIf ReportName = "Customer Collection Non Eft" Then
            fname = "Custcoll_n_msg_" & Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".xml"
        Else
            fname = Now.Year & month.PadLeft(2, "0") & day.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & milisec.ToString.PadLeft(2, "0") & ".xml"
        End If
        'Dim FPath As String = My.Application.Info.DirectoryPath & "/MailAttach/"
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
        If File.Exists(FPath & fname) Then
            File.Delete(fname)
        End If
        dt.TableName = "FTP"
        Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
        dt.WriteXml(sw)
        Return fname
    End Function
    Public Shared Function InwardFTPREPORT(ByVal Tid As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Try

            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
            Dim DocPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/DOCS/")
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler where isSendAttachment=1 and Tid='" & Tid & "' "
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            '  For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
            If ds.Tables("rpt").Rows.Count > 0 Then
                '       If ReportScheduler(ds.Tables("rpt").Rows(0).Item("tid")) = True Then
                ' If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "USER" Then
                Dim eid As String = ds.Tables("rpt").Rows(0).Item("eid").ToString()

                Dim ftpadd As String = ds.Tables("rpt").Rows(0).Item("FtpAddress").ToString()
                Dim ftpid As String = ds.Tables("rpt").Rows(0).Item("FtpID").ToString()
                Dim ftppwd As String = ds.Tables("rpt").Rows(0).Item("FtpPwd").ToString()
                Dim cfolder As String = ds.Tables("rpt").Rows(0).Item("FtpFolder").ToString()
                Dim ftpType As String = ds.Tables("rpt").Rows(0).Item("FTPType").ToString()
                Dim DocType As String = ds.Tables("rpt").Rows(0).Item("m_Documenttype").ToString()
                Dim Fieldstosend As String = ds.Tables("rpt").Rows(0).Item("m_Fieldstosend").ToString()
                Dim AttachRenamefield As String = ds.Tables("rpt").Rows(0).Item("AttachRenamefield").ToString()
                Dim str As String = ds.Tables("rpt").Rows(0).Item("m_Query").ToString()
                Dim filetype As String = ds.Tables("rpt").Rows(0).Item("sendtype").ToString()
                Dim lastschedule As String = ds.Tables("rpt").Rows(0).Item("LastScheduledDate").ToString
                ' lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                'str = Replace(str, "@lastsch", lastschedule)
                oda.SelectCommand.CommandText = str
                oda.SelectCommand.CommandType = CommandType.Text
                Dim FTPR As New DataTable
                oda.Fill(FTPR)
                For i As Integer = 0 To FTPR.Rows.Count - 1
                    Dim FileName As String = ""
                    Dim copyFile As String = ""
                    Dim FtpFile As String = ""
                    Dim RNameFileName As String = ""
                    Dim Docid As String = FTPR.Rows(i).Item("DOCID").ToString()

                    Dim stringSpiltArray As String()
                    If Not Fieldstosend Is Nothing And Fieldstosend.Length <> 0 Then
                        stringSpiltArray = Fieldstosend.Split(",")
                    End If
                    If (stringSpiltArray IsNot Nothing AndAlso stringSpiltArray.Count > 0) Then
                        For Each strField As String In stringSpiltArray
                            Dim count As Integer = 0
                            oda.SelectCommand.CommandText = "select " & strField & " [Fieldstosend]," & AttachRenamefield & " [AttRefield] from  MMM_MST_DOC where tid='" & Docid & "' and DocumentType= '" & DocType & "'"
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.Fill(ds, "ftpRegisters")
                            FileName = ds.Tables("ftpRegisters").Rows(0).Item(0).ToString()
                            RNameFileName = ds.Tables("ftpRegisters").Rows(0).Item(1).ToString()
                            ds.Tables("ftpRegisters").Clear()
                            If Not String.IsNullOrEmpty(FileName) Then
                                Dim strArr() As String
                                strArr = FileName.Split("/")
                                FileName = DocPath & eid & "\" & strArr(1)

                                'Dim name As String = Path.GetFileName(FileName)
                                'Dim bytes As Byte() = System.IO.File.ReadAllBytes(FileName)

                                Dim extension As String = Path.GetExtension(FileName)
                                If (count = 0) Then
                                    copyFile = FPath & RNameFileName & "" & extension
                                    FtpFile = RNameFileName & extension
                                Else
                                    copyFile = FPath & RNameFileName & "_" & count & extension
                                    FtpFile = RNameFileName & "_" & count & extension
                                End If
                                If System.IO.File.Exists(copyFile) Then System.IO.File.Delete(copyFile)

                                If System.IO.File.Exists(FileName) Then
                                    System.IO.File.Copy(FileName, copyFile)
                                End If
                                count = count + 1

                                If (ftpType.ToUpper = "SFTP") Then
                                    CopyfiletoSFTP(ftpadd, ftpid, ftppwd, FPath, FtpFile, cfolder)
                                Else
                                    CopyfiletoInbox(ftpadd, ftpid, ftppwd, FPath, FtpFile, cfolder)
                                End If

                                System.IO.File.Delete(copyFile)
                            End If
                        Next
                    End If

                Next

                ' End If
                '      End If
            End If

            '  Next
        Catch ex As Exception
            ' Throw

            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FTPREPORT")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 46)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()

        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Function

End Class
Public Class kGridIntegration
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnIntegration)
End Class
Public Class kColumnIntegration
    Public Sub New()

    End Sub
End Class