Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.IO
Imports System.Web.Services
Imports System.Net
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Web.Hosting
Imports System.Random
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Net.Mail
Imports System.Threading
Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Diagnostics
Imports System.Web.Script.Serialization
Imports System.Security.Authentication
Imports Renci.SshNet
Imports Npgsql
Partial Class MonthJVIntegration
    Inherits System.Web.UI.Page

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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd1.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub


    <WebMethod()>
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        Dim eid As Integer = HttpContext.Current.Session("eid")
        Dim GLN_Date As String = ""
        Dim Total As Integer = 1

        Try

            SAVEFILEREPORTTO_STAGING(sdate.ToString, eid, Total)
            grid.Message = "Sucessfully Run !"
            grid.Success = False
            Return grid

        Catch Ex As Exception
            grid.Success = False
            grid.Message = "Please contact system admin."
            grid.Count = 0

        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        Return grid
    End Function
    Protected Shared Function SAVEFILEREPORTTO_STAGING(GL_date As String, eid As String, MonthJV As Integer) As Boolean

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim builder As SqlConnectionStringBuilder = New SqlConnectionStringBuilder(conStr)
        Dim ds As New DataSet()
        Dim tid As String = ""
        Dim Rptname As String = ""
        Dim tableAction = ""
        Dim tableKey = ""
        Dim FTPType As String = ""
        Dim alertname As String = ""

        Try
            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
            oda.SelectCommand.CommandText = "Select * from MMM_MST_ReportScheduler where EID=194  and Tid=615 "
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            Try

                For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                    Try
                        If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = False Then
                            Try
                                If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "USER" Then
                                    Dim DBlastrun As DateTime = Convert.ToDateTime(ds.Tables("rpt").Rows(d).Item("LastScheduledDate").ToString())

                                    eid = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                                    alertname = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                    tid = ds.Tables("rpt").Rows(d).Item("tid").ToString()
                                    con = New SqlConnection(conStr)
                                    oda = New SqlDataAdapter("", con)
                                    oda.SelectCommand.CommandText = String.Empty
                                    oda.SelectCommand.CommandText = "select * from  MMM_MST_Entity where eid='" & eid & "' "
                                    oda.SelectCommand.CommandType = CommandType.Text
                                    oda.Fill(ds, "ftpsettings")
                                    Dim ftpadd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpAddress").ToString()
                                    Dim ftpid As String = ds.Tables("ftpsettings").Rows(0).Item("FtpID").ToString()
                                    Dim ftppwd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpPwd").ToString()
                                    Dim cid As String = ds.Tables("ftpsettings").Rows(0).Item("Code").ToString()
                                    Dim dbadd As String = ds.Tables("ftpsettings").Rows(0).Item("DBserverIP").ToString()
                                    Dim dbcatalog As String = ds.Tables("ftpsettings").Rows(0).Item("DBcatelogue").ToString()
                                    Dim dbuserid As String = ds.Tables("ftpsettings").Rows(0).Item("DBuserid").ToString()
                                    Dim dbpwd As String = ds.Tables("ftpsettings").Rows(0).Item("DBpwd").ToString()
                                    Dim dbport As String = ds.Tables("ftpsettings").Rows(0).Item("Port").ToString()
                                    Dim dbtableschema As String = ds.Tables("ftpsettings").Rows(0).Item("DBTableSchema").ToString()
                                    Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                                    FTPType = ds.Tables("ftpsettings").Rows(0).Item("FTPType").ToString()
                                    Dim filetype As String = ds.Tables("rpt").Rows(d).Item("sendtype").ToString()
                                    Dim ReportTid As String = ds.Tables("rpt").Rows(d).Item("Tid").ToString()
                                    Dim lastschedule As String = ds.Tables("rpt").Rows(d).Item("LastScheduledDate").ToString
                                    Dim reportName As String = ds.Tables("rpt").Rows(d).Item("ReportName").ToString
                                    lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                                    str = Replace(str, "@lastsch", lastschedule)
                                    str = Replace(str, "---" + reportName, "")

                                    oda.SelectCommand.CommandText = "GetMonthJVApollo"
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@GL_Date", GL_date)
                                    Dim FTPR As New DataTable
                                    oda.Fill(FTPR)

                                    Try
                                        If FTPR.Rows.Count > 0 Then
                                            Dim sqlconn As New SqlConnection
                                            Dim sqlquery As New SqlCommand
                                            tableAction = ds.Tables("rpt").Rows(d).Item("DBTableAction").ToString()
                                            tableKey = ds.Tables("rpt").Rows(d).Item("DBTablePKey").ToString()
                                            Dim tablename As String = ds.Tables("rpt").Rows(d).Item("DBtableName").ToString() 'abctable'

                                            Dim MyCon As NpgsqlConnection = New NpgsqlConnection("Server=" & dbadd & ";User Id=" & dbuserid & ";" & "Password=" & dbpwd & ";Database=" & dbcatalog & ";")

                                            MyCon.Open()
                                            If MyCon.State = ConnectionState.Open Then

                                                Dim objConn As New NpgsqlConnection
                                                Dim objCmd As New NpgsqlCommand
                                                Dim dtAdapter As New NpgsqlDataAdapter
                                                Dim dsss As New DataSet

                                                Dim STAGING_Tbl As New DataTable
                                                STAGING_Tbl = FTPR
                                                Dim Result As String = ""
                                                Dim row_number As Integer = 0


                                                For Each row2 As DataRow In STAGING_Tbl.Rows
                                                    Dim col As String = String.Empty
                                                    Dim Value As String = String.Empty
                                                    Dim insert As String = ""

                                                    For Each column As DataColumn In STAGING_Tbl.Columns
                                                        Console.Write(column.ColumnName)
                                                        insert = ""
                                                        col = col + column.ColumnName + ","
                                                        Value = Value + "'" + row2(column.ColumnName).ToString + "'" + ","
                                                    Next

                                                    Value = Value.Remove(0, Value.Split(",")(0).Length)
                                                    Value = "nextval('vendor_master_seq')" + Value
                                                    insert = "insert into  " + dbcatalog.Trim + "." + dbtableschema.Trim + "." + tablename + "(" + col.Trim().Substring(0, col.Length - 1) + ") values (" + Value.Trim().Substring(0, Value.Length - 1) + ")"
                                                    Dim LastRunDate As String = ""

                                                    Dim currentTime As DateTime = System.DateTime.Now

                                                    LastRunDate = currentTime.Day.ToString.PadLeft(2, "0") & "-" & currentTime.Month.ToString.PadLeft(2, "0") & "-" & currentTime.Year.ToString & "-" & currentTime.Hour.ToString.PadLeft(2, "0") & "-" & currentTime.Minute.ToString.PadLeft(2, "0")

                                                    Try
                                                        objCmd = New NpgsqlCommand(insert, MyCon)
                                                        Result = objCmd.ExecuteNonQuery()
                                                        insert = ""
                                                        col = ""
                                                        Value = ""

                                                        If Result > 0 Then

                                                            Dim query As String = "update MMM_MST_ReportScheduler set LastScheduledDate =getdate() where tid=" + tid + ""
                                                            Using command As SqlCommand = New SqlCommand(query, con)
                                                                con.Open()
                                                                command.ExecuteNonQuery()
                                                                con.Close()
                                                            End Using
                                                            Call Save_Log(cid, reportName, reportName, eid, Result, "", Now(), Now(), 1, "SUCCESS", "", "", "", LastRunDate)
                                                        End If

                                                    Catch ex As SqlException
                                                        Call Save_Log(cid, reportName, reportName, eid, "Failled", "", Now(), Now(), 1, "ERROR", "", "", ex.Message, LastRunDate)
                                                        Call SendStagingIntegrationLogMail(eid, tid, LastRunDate)

                                                    Catch ex As Exception
                                                        Call Save_Log(cid, reportName, reportName, eid, "Failled", "", Now(), Now(), 1, "ERROR", "", "", ex.Message, LastRunDate)
                                                        Call SendStagingIntegrationLogMail(eid, tid, LastRunDate)

                                                    End Try

                                                Next
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim sta As New StackTrace(ex, True)
                                        Dim fr = sta.GetFrame(sta.FrameCount - 1)
                                        Dim frline As String = fr.GetFileLineNumber.ToString()
                                        Dim methodname = fr.GetMethod.ToString()
                                        oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & ": LineNo-" & frline & "MethodName" & methodname)
                                        oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", alertname)
                                        oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        oda.SelectCommand.ExecuteNonQuery()
                                    Finally
                                        con.Close()
                                        con.Dispose()
                                        oda.Dispose()
                                    End Try

                                End If
                            Catch ex As Exception
                                Dim sta As New StackTrace(ex, True)
                                Dim fr = sta.GetFrame(sta.FrameCount - 1)
                                Dim frline As String = fr.GetFileLineNumber.ToString()
                                Dim methodname = fr.GetMethod.ToString()
                                oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & ": LineNo-" & frline & "MethodName" & methodname)
                                oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", alertname)
                                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.ExecuteNonQuery()
                            Finally

                                con.Close()
                                con.Dispose()
                                oda.Dispose()
                            End Try
                        End If
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
                        oda.SelectCommand.ExecuteNonQuery()
                    Finally
                        con.Close()
                        con.Dispose()
                        oda.Dispose()
                    End Try
                Next
            Catch exx As Exception
                Dim sta As New StackTrace(exx, True)
                Dim fr = sta.GetFrame(sta.FrameCount - 1)
                Dim frline As String = fr.GetFileLineNumber.ToString()
                Dim methodname = fr.GetMethod.ToString()
                oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", exx.ToString() & ": LineNo-" & frline & "MethodName" & methodname)
                oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", alertname)
                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Finally
                con.Close()
                con.Dispose()
                oda.Dispose()
            End Try
        Catch ex As Exception
            Dim sta As New StackTrace(ex, True)
            Dim fr = sta.GetFrame(sta.FrameCount - 1)
            Dim frline As String = fr.GetFileLineNumber.ToString()
            Dim methodname = fr.GetMethod.ToString()
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & ": LineNo-" & frline & "MethodName" & methodname)
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", alertname)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()

        End Try

    End Function

    Protected Shared Function ReportScheduler(ByVal tid As Integer) As Boolean
        Dim b As Boolean = False

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
        Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()

        Dim da As New SqlDataAdapter("select HH,MM,reporttype,TID,date,isnull(scheduletime,'')[scheduletime] from MMM_MST_ReportScheduler where tid=" & tid & " ", con)
        Dim dt As New System.Data.DataTable()
        da.Fill(dt)
        Dim SchType As String = dt.Rows(0).Item("reporttype").ToString()
        Dim schedultime As String = dt.Rows(0).Item("scheduletime").ToString()

        If UCase(SchType) = "MONTHLY" Then
            Dim currentDate As DateTime = DateTime.Now
            Dim dateofMonth As Integer = currentDate.Day
            Dim dateMail As Integer = dt.Rows(0).Item("Date").ToString()

        End If
        con.Close()
        con.Dispose()
        da.Dispose()
        dt.Dispose()
        Return b
    End Function

    Protected Shared Function Save_Log(ByVal CID As String, ByVal Filename As String, ByVal ActionCode As String, ByVal Empcode As String, ByVal Result As String, ByVal Remarks As String, ByVal fileRundate As String, ByVal SuccessfulRun As String, ByVal TotalAttempts As Integer, ByVal TransType As String, ByVal xmlFileNm As String, Optional ByVal Rerunstr As String = "", Optional ByVal ErrMsg As String = "", Optional ByRef LastInsertDate As String = "") As String

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        LastInsertDate = fileRundate
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "insert into Staging_Outwardintegration_Result (EntryDate,cid, ReportName, actionCode, empcode, result, remarks, fileRundate, SuccessfulRun, TotAttempts, transType, XmlFileName, ReRunRemarks, errMsg) values('" & fileRundate & "','" & CID & "','" & Filename & "', '" & ActionCode & "', '" & Empcode & "', '" & Result & "', '" & Remarks & "','" & fileRundate & "','" & SuccessfulRun & "','" & TotalAttempts & "','" & TransType & "','" & xmlFileNm & "','" & Rerunstr & "','" & ErrMsg & "')"
        da.SelectCommand.Parameters.Clear()
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        da.SelectCommand.ExecuteNonQuery()
        con.Dispose()
        da.Dispose()
        con = Nothing
        da = Nothing
        Save_Log = ""
    End Function

    Protected Shared Function SendStagingIntegrationLogMail(ByVal CurCid As String, ByVal tid As Integer, ByVal LastRunDMY As String) As String

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            If con.State <> ConnectionState.Open Then con.Open()
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select * from MMM_MST_ENTITY where EID='" & CurCid & "'"
            Dim dtcomp As New DataTable
            da.Fill(dtcomp)

            If dtcomp.Rows.Count > 0 Then
                Dim path1 As String = HostingEnvironment.MapPath("~/Import/")
                Dim path2 As String = HostingEnvironment.MapPath("~/ES_Import/")
                Dim Mailto, MailSub, vmsg, MailCC, MailBCC, SendType As String
                Dim Attchfilename As String = ""
                Dim FADD As String = ""
                Dim login As String = ""
                Dim pwd As String = ""
                Dim Filename As String = ""
                Dim FilenameError As String = ""
                Dim FileSource As String = ""
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = "select * from MMM_MST_ReportScheduler where tid=" & tid & " and FTPFlag=3"
                Dim dtM As New DataTable()
                If con.State = ConnectionState.Closed Then con.Open()
                da.Fill(dtM)

                If dtM.Rows.Count > 0 Then
                    Dim isFileNameStatic As String = dtM.Rows(0).Item("ReportName")
                    Dim fileext As String = ".txt"
                    Dim FileNameSuffix As String = ""
                    Dim FileSuffixDateFormat As String = ""
                    Dim ReduceDays As Integer = 0
                    Dim CurrDate As DateTime = System.DateTime.Now
                    Dim DMYstr As String = ""

                    Filename = dtM.Rows(0).Item("ReportName").ToString()
                    FileSuffixDateFormat = Convert.ToString(dtM.Rows(0).Item("LastScheduledDate"))
                    If FileSuffixDateFormat = "DDMMYYYY" Then
                        DMYstr = (CurrDate.Day - ReduceDays).ToString.PadLeft(2, "0") & CurrDate.Month.ToString.PadLeft(2, "0") & CurrDate.Year.ToString
                    ElseIf FileSuffixDateFormat = "MMDDYYYY" Then
                        DMYstr = CurrDate.Month.ToString.PadLeft(2, "0") & (CurrDate.Day - ReduceDays).ToString.PadLeft(2, "0") & CurrDate.Year.ToString
                    Else
                        DMYstr = CurrDate.Year.ToString & CurrDate.Month.ToString.PadLeft(2, "0") & (CurrDate.Day - ReduceDays).ToString.PadLeft(2, "0")
                    End If
                    Filename = dtM.Rows(0).Item("ReportName").ToString() & FileNameSuffix & DMYstr & "." & fileext

                    Mailto = Convert.ToString(dtM.Rows(0).Item("emailto"))
                    MailCC = Convert.ToString(dtM.Rows(0).Item("CC"))
                    MailBCC = Convert.ToString(dtM.Rows(0).Item("bcc"))
                    SendType = Convert.ToString(dtM.Rows(0).Item("SendTo"))

                    FileSource = Convert.ToString(dtcomp.Rows(0).Item("FTPType"))
                    Dim Sendattch As String = ""
                    If FileSource = "STAGING DB" Then
                        Sendattch = System.Web.Hosting.HostingEnvironment.MapPath("~/ES_Import/" & Filename)
                    End If
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandType = CommandType.Text
                    If SendType = "ALL" Then
                        da.SelectCommand.CommandText = "select replace(filerunDate,'_',' To ') [Batch Detail], actioncode [Serial No.], CONVERT(NVARCHAR(30),entrydate,113) [Created On], empcode [Unique Key],result [Response], Remarks [Result], transtype [Trans-Status], ErrMsg  from Staging_Outwardintegration_Result where filerunDate='" & LastRunDMY & "'  order by tid"
                    Else
                        da.SelectCommand.CommandText = "select replace(filerunDate,'_',' To ') [Batch Detail], actioncode [Serial No.], CONVERT(NVARCHAR(30),entrydate,113) [Created On], empcode [Unique Key],result [Response], Remarks [Result], transtype [Trans-Status], ErrMsg  from Staging_Outwardintegration_Result where filerunDate='" & LastRunDMY & "' and  TransType='ERROR' order by tid"
                    End If

                    Dim dtErr As New DataTable
                    da.Fill(dtErr)
                    Dim ErrMsg As String = dtErr.Rows(0).Item("ErrMsg")

                    If dtErr.Rows.Count > 0 Then
                        Dim resfilename As String = CreateCSV(dtErr, Left(Filename, Len(Filename) - 4) & "_" & CurCid & "_Result_")

                        MailSub = "Result of OutWard Integration File Run"
                        vmsg = "<b><p style=""background-color:#fff;color:#104E8B"" > <font face=""arial, helvetica, sans-serif"" size=""2"">   Dear All </font> </p></b>"
                        vmsg &= "<p>  <font face=""arial, helvetica, sans-serif"" size=""2""> OutWard Integrated file run for date : <b> " & LastRunDMY & " </b>"
                        vmsg &= "<br>Please review and take suitable action for the records where Trans-Status is 'ERROR' as the same could not be updated in PEARL Portal.<br /> Error:- " & ErrMsg & "</font></p>"
                        vmsg += "<table border=1>"
                        vmsg += "<tr><td><font face=""arial, helvetica, sans-serif"" size=""2""> Download Integration Result File <a href=""https://oit.myndsolution.com/Import/" & resfilename & """>" & "Click Here" & "</a></font></td></tr>"
                        vmsg &= "</table>"
                        vmsg &= "<br><b>" & " <p style=""background-color:#fff;color:#104E8B"" > <font face=""arial, helvetica, sans-serif"" size=""2"">Regards"
                        vmsg &= "<br>Mynd Integrated Solutions Team</font></p></b>"

                        Try
                            Dim objMail As New MailUtill(EID:=CurCid)
                            objMail.SendMail(ToMail:=Mailto, Subject:=MailSub, MailBody:=vmsg, CC:=MailCC, BCC:=MailBCC)
                        Catch ex As Exception
                        End Try
                        dtErr.Dispose()
                        dtErr = Nothing
                    End If
                End If
            End If
            con.Close()
            con.Dispose()
            con = Nothing
            da.Dispose()
            da = Nothing
            SendStagingIntegrationLogMail = ""
        Catch ex As Exception
            Dim txtmsg As String = "Try catch error - " & ex.Message.ToString
        End Try
    End Function

    Protected Shared Function CreateCSV(ByVal dt As DataTable, ByVal apid As String) As String
        'Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & ".CSV"
        Try
            ' UpdateErrorLog("Varroc_leave", "Exit from CreateCSV", "WS To FTP", "1", "WTF")
            Dim fname As String = apid & ".CSV"

            Dim path As String = System.Web.Hosting.HostingEnvironment.MapPath("~\Import\")
            Dim targetFI As New FileInfo(path + "\" + fname)
            If targetFI.Exists Then
                targetFI.Delete()
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
            'UpdateErrorLog("Varroc_leave", "Exit from  CreateCSV", "WS To FTP", "1", "WTF")
        Catch ex As Exception
            Throw New Exception(ex.Message)
            ' UpdateErrorLog("Varroc_leave", "Error in  CreateCSV", "WS To FTP", "1", "WTF")
        End Try
    End Function

End Class
