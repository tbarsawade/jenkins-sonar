' NOTE: You can use the "Rename" command on the context menu to change the class name "DMSService" in code, svc and config file together.
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
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
'Imports Microsoft.Office.Interop.Excel
'Imports Microsoft.Office.Interop


Public Class DMSService
    Implements IDMSService
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString


    Function GETEIDDATA(Data As EIDDETAIL) As List(Of EIDRESPONSE) Implements IDMSService.GETEIDDATA
        Dim res As New List(Of EIDRESPONSE)
        Try
            Dim EID As Int32 = Data.EID
            Dim dt As New DataTable
            Dim dc As New DataClass()
            'dt = dc.ExecuteQryDT("select * from mmm_mst_ftpfiletransfer where eid=" & EID)
            dt = dc.ExecuteQryDT("select * from mmm_mst_ftpfiletransfer Order by Eid")
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    Dim response As New EIDRESPONSE
                    response.ftpid = dr("ftpid")
                    response.EID = dr("EID")
                    response.uid = dr("UID")
                    response.gid = dr("GID")
                    response.docType = Convert.ToString(dr("doctype"))
                    response.fup_FieldMapping = Convert.ToString(dr("FUP_FIELDMAPPING"))
                    response.loc_FieldMapping = Convert.ToString(dr("loc_FieldMapping"))
                    response.BarCode = Convert.ToString(dr("barcode"))
                    response.locid = dr("locid")
                    response.ReadMode = Convert.ToString(dr("readmode"))
                    response.HostName = Convert.ToString(dr("hostname"))
                    response.UserName = Convert.ToString(dr("username"))
                    response.Password = Convert.ToString(dr("Password"))
                    response.Port = Convert.ToString(dr("Port"))
                    response.PostFix = Convert.ToString(dr("PostFix"))
                    response.SaveAs = Convert.ToString(dr("SaveAs"))
                    res.Add(response)
                Next
            End If
        Catch ex As Exception

        End Try

        'Return res
        'Dim json As New JavaScriptSerializer()
        'Dim s As String = json.Serialize(res)
        Return res

    End Function



    Public Function Address(EID As Integer, documenttype As String) As String Implements IDMSService.Address
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspRetunAddress", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Trim(EID))
        oda.SelectCommand.Parameters.AddWithValue("doctype", Trim(documenttype))

        Dim obj As New GisMethods()
        Dim retobj As New ReturnAddress()
        Dim lat As String = ""
        Dim latlong As String = ""
        Dim lon As String = ""
        Try
            Dim ds As New DataSet
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.Fill(ds, "data")
            '       Dim iSt As String = oda.SelectCommand.ExecuteScalar()
            Dim Addstr As String = ""
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Addstr = ds.Tables("data").Rows(i).Item("fld1").ToString()
                retobj = obj.GoogleGeoCodeFreeText(Addstr)
                Dim flag As Int16 = 0
                If retobj.Success = True Then
                    'latlong = retobj.Latt & "," & retobj.Longt & "|"
                    flag = 1
                    lat = retobj.Latt
                    lon = retobj.Longt

                End If
                If flag = 1 Then
                    latlong &= retobj.Latt & "," & retobj.Longt & "," & ds.Tables("data").Rows(i).Item("fld13").ToString() & "," & ds.Tables("data").Rows(i).Item("fld12").ToString() & "|"
                End If
            Next
            latlong = Left(latlong, latlong.Length - 1)
            con.Close()
            oda.Dispose()
            con.Dispose()
            If latlong.Length > 2 Then
                Return latlong
            Else
                Return "Not found address"
            End If
        Catch ex As Exception
            con.Close()
            oda.Dispose()
            con.Dispose()
            Return "Not found address"
        End Try
    End Function


    'Added for FTP Integration
    Function Scheduler(ByVal FTid As Integer) As Boolean
        Dim b As Boolean = False
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            Dim da As New SqlDataAdapter("select ScheduleTime from mmm_tallyint_ftpsch where tid=" & FTid, con)
            'Dim da As New SqlDataAdapter("select HH,MM,ScheduleTime from oit_mst_inputfile where tid=44" & FTid, con)
            Dim dt As New DataTable()
            da.Fill(dt)
            Dim i As Integer = 0
            ' Dim date1f As String
            If dt.Rows.Count = 1 Then
                If (dt.Rows(0).Item("ScheduleTime").ToString() <> "") Then
                    'Dim schedule As String = "*|*|*|*|*"
                    Dim schedule As String = dt.Rows(0).Item("ScheduleTime").ToString()
                    Dim str_array As [String]() = schedule.Split("|")
                    Dim stringa As [String] = str_array(0)
                    Dim stringb As [String] = str_array(1)
                    Dim stringc As [String] = str_array(2)
                    Dim stringd As [String] = str_array(3)
                    Dim stringe As [String] = str_array(4)
                    Dim currentTime As System.DateTime = System.DateTime.Now
                    Dim currentdate As String = currentTime.Date.Day
                    Dim str_datte As [String]() = stringb.Split(",")
                    Dim str_hours As [String]() = stringd.Split(",")
                    Dim str_mintus As [String]() = stringe.Split(",")

                    If Trim(stringa) = "*" And Trim(stringb) = "*" And Trim(stringc) = "*" And Trim(stringd) = "*" And Trim(stringe) = "*" Then
                        b = True
                        Return b
                        ' Exit For
                    End If

                    If stringb <> "*" Then
                        For j As Integer = 0 To str_datte.Length - 1
                            Dim A As [String] = str_datte(j)
                            If A.Contains("-") Then
                                Dim o As [String]() = A.Split("-")
                                Dim f As [String] = o(0)
                                Dim g As [String] = o(1)
                                If (f <= currentdate And g >= currentdate) Then
                                    For n As Integer = 0 To str_hours.Length - 1
                                        For m As Integer = 0 To str_mintus.Length - 1
                                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                            If x <= time2 And x >= time1 Then
                                                b = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                            Else
                                If ((currentdate = A)) Then
                                    For n As Integer = 0 To str_hours.Length - 1
                                        For m As Integer = 0 To str_mintus.Length - 1
                                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                            If x <= time2 And x >= time1 Then
                                                b = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                            End If
                        Next
                    ElseIf ((currentdate = stringb) Or (stringb.Trim() = "*") Or (stringd.Trim() <> "") Or (stringe.Trim() <> "")) Then
                        For n As Integer = 0 To str_hours.Length - 1
                            For m As Integer = 0 To str_mintus.Length - 1
                                Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                ' Dim x As DateTime = (Convert.ToDateTime(stringd & ":" & stringe & ":" & "00").ToShortTimeString)
                                If x <= time2 And x >= time1 Then
                                    b = True
                                    Exit For
                                End If
                            Next
                        Next
                    ElseIf ((stringa.Trim() = "*") Or (stringb.Trim() = "*") Or (stringc.Trim() = "*") Or (stringd.Trim() = "*") Or (stringe.Trim() = "*")) Then
                        b = True
                        'Exit For
                    End If
                End If
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
            dt.Dispose()
            Return b
        Catch ex As Exception

            ' Label1.Text = "Exception found in cushman_varroc automation()"
        End Try
    End Function
    'Added for FTP import Field
    Public Function CopyFilefromFtp(ByVal FileNm As String, ByVal FilePath As String, ByVal loginID As String, ByVal Pwd As String, ByVal Valuename As String) As String
        'UpdateErrorLog(FileNm, "txt", "Enter in CopyFilefromFtp", "1", loginID)
        Try

            Dim ImportPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~\ES_Import\")
            Dim fileCount As Integer = 0

            Dim dtCl As New DataTable

            If ftpFileExist(FilePath & FileNm, loginID, Pwd) = True Then
                fileCount = fileCount + 1
                Dim URI As String = FilePath & FileNm
                Dim ftp As Net.FtpWebRequest = CType(FtpWebRequest.Create(URI), FtpWebRequest)
                ftp.Credentials = New Net.NetworkCredential(loginID, Pwd)
                ftp.Method = Net.WebRequestMethods.Ftp.DownloadFile

                ftp.UseBinary = False
                Dim targetFI As New FileInfo(ImportPath + "\" + FileNm)
                'If targetFI.Exists Then
                ' targetFI.Delete()
                'End If

                Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
                    Using responseStream As Stream = response.GetResponseStream
                        Using fs As FileStream = targetFI.OpenWrite
                            Try
                                Dim buffer(2047) As Byte
                                Dim read As Integer = 0
                                Do
                                    read = responseStream.Read(buffer, 0, buffer.Length)
                                    fs.Write(buffer, 0, read)
                                Loop Until read = 0
                                responseStream.Close()
                                fs.Flush()
                                fs.Close()
                                '' write here to delete from ftp - call function
                                If (UCase(Valuename) = "BOTH") Or (UCase(Valuename) = "DELETE") Then
                                    DeleteFileFromServer(URI, loginID, Pwd)
                                End If
                                CopyFilefromFtp = "SUCCESSFUL"
                                Return "SUCCESSFUL"
                                'UpdateErrorLog("TC", "Fatal", "CopyFilefromFtp SUCCESSFUL", "2", "FE")
                                Exit Function
                            Catch ex As Exception
                                'fs.Close()
                                'targetFI.Delete()
                                Return "ERROR"
                                'UpdateErrorLog("TC", "Fatal", "CopyFilefromFtp ERROR", "3", "FE")
                                'Throw
                            End Try
                        End Using
                        responseStream.Close()
                    End Using
                    response.Close()
                    'UpdateErrorLog("TC", "Fatal", "Exit from CopyFilefromFtp", "4", "FE")
                End Using

            Else
                'UpdaeErrorLog("TC", "Fatal", "File FTP Not Found", "5", "FE")
            End If
        Catch ex As Exception
            'UpdateErrorLog("Fatal Error(Exl)", "Fatal", "Exception found in CopyFilefromFtp() func MSG - " & ex.Message.ToString, "1", "FE")
        End Try
    End Function
    Public Sub DeleteFileFromServer(ByVal filename As String, ByVal login As String, ByVal pwd As String)
        'UpdateErrorLog("TC", "DeleteFileFromServer", "Enter in DeleteFileFromServer", "2", "FE")
        Dim request As FtpWebRequest = DirectCast(WebRequest.Create(filename), FtpWebRequest)
        request.Method = WebRequestMethods.Ftp.DeleteFile
        request.Credentials = New NetworkCredential(login, pwd)
        Dim response As FtpWebResponse = DirectCast(request.GetResponse(), FtpWebResponse)
        response.Close()
        'UpdateErrorLog("TC", "DeleteFileFromServer", "Exit from DeleteFileFromServer", "2", "FE")
    End Sub

    Public Function ftpFileExist(ByVal fileName As String, ByVal loginID As String, ByVal Pwd As String) As Boolean
        Dim wc As New WebClient()
        Try
            wc.Credentials = New NetworkCredential(loginID, Pwd)
            Dim fData As Byte() = wc.DownloadData(fileName)
            If fData.Length > -1 Then
                Return True
            Else
                Return False
            End If
            ' Debug here?
        Catch generatedExceptionName As Exception
        End Try
        Return False
    End Function
    Protected Sub GetDataFromXml(ByVal Filename As String, ByVal FilePath As String, ByVal UserName As String, ByVal PWD As String)
        If Right(Filename, 4).ToUpper() = ".XML" Then
            Dim sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/ES_Import/" & Filename))
            Dim FileNames = System.Web.Hosting.HostingEnvironment.MapPath("~/ES_Import/" & Filename)
            Dim SuccessCount As Integer = 0
            Dim FailCount As Integer = 0
            Dim dtData As New DataTable
            dtData.Columns.Add("DocNumber")
            dtData.Columns.Add("Response")
            Dim xmldoc As New XmlDocument()
            Dim fs As FileStream = New FileStream(FileNames, FileMode.Open, FileAccess.Read)
            xmldoc.Load(fs)
            Dim xmlStr As String = xmldoc.InnerXml

            ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

            'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
            'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
            'ServicePointManager.SecurityProtocol = Tls12

            Dim sURL As String
            sURL = "https://Myndsaas.com/BPMCustomWS.svc/Inward"
            Dim request As HttpWebRequest = HttpWebRequest.Create(sURL)
            Dim encoding As New ASCIIEncoding()
            Dim strResult As String = ""


            ' convert xmlstring to byte using ascii encoding
            ' Dim data As Byte() = encoding.GetBytes(sURL)
            'Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(sURL)), HttpWebRequest)
            Dim data As Byte() = encoding.GetBytes(xmlStr)
            Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(sURL)), HttpWebRequest)

            webrequest__1.Method = "POST"
            ' set content type
            webrequest__1.ContentType = "application/x-www-form-urlencoded"
            ' set content length
            webrequest__1.ContentLength = data.Length
            webrequest__1.Timeout = 1000 * 60 * 5
            ' get stream data out of webrequest object
            Dim newStream As Stream = webrequest__1.GetRequestStream()
            newStream.Write(data, 0, data.Length)
            newStream.Close()
            ' declare & read response from service
            Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)
            Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")

            ' read response stream from response object
            Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)

            strResult = loResponseStream.ReadToEnd()
            loResponseStream.Close()
            ' close the response object
            webresponse.Close()


            'Dim request As HttpWebRequest = HttpWebRequest.Create(sURL)
            'request.Method = WebRequestMethods.Http.Post
            'request.ContentLength = xmlStr.Length
            'request.ContentType = "text/xml"
            'request.Timeout = 600 * 1000

            'Dim writer As New StreamWriter(request.GetRequestStream)
            'writer.Write(xmlStr)
            'writer.Close()
            'Dim Response As HttpWebResponse = request.GetResponse()
            ''Response.StatusCode
            'Dim reader As New StreamReader(Response.GetResponseStream())
            'Dim xmlDocReceived As New XmlDocument
            'xmlDocReceived.Load(Response.GetResponseStream())

            'xmlDocReceived.InnerXml
            Dim xdoc As New XmlDocument()
            xdoc.LoadXml(strResult)

            Dim strs As String = System.Web.Hosting.HostingEnvironment.MapPath("~/ES_Import/" & Replace(Filename, ".", "_Result."))
            xdoc.Save(strs)
            'lblMsg.Text = "Run successful"
            CopyfiletoArchive(FilePath, UserName, PWD, strs, Replace(Filename, ".", "_Result."))
        End If
    End Sub
    Private Shared Function ValidateRemoteCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal policyErrors As SslPolicyErrors) As Boolean
        If Convert.ToBoolean(ConfigurationManager.AppSettings("IgnoreSslErrors")) Then
            ' allow any old dodgy certificate...
            Return True
        Else
            Return policyErrors = SslPolicyErrors.None
        End If
    End Function
    Protected Sub CopyfiletoArchive(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String)


        Try
            Dim request As FtpWebRequest = FtpWebRequest.Create(Fadd)
            Dim creds As NetworkCredential = New NetworkCredential(login, pwd)
            request.Credentials = creds

            Dim resp As FtpWebResponse = Nothing
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            request.KeepAlive = True
            Using resp
                resp = request.GetResponse()
                Dim sr As StreamReader = New StreamReader(resp.GetResponseStream(), System.Text.Encoding.ASCII)
                Dim s As String = sr.ReadToEnd()
                ' Dim URI As String = Fadd & "Archive/" & cfoldernm & "/" & filenm
                Dim URI As String = Fadd & filenm
                Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(URI), System.Net.FtpWebRequest)
                clsRequest.Credentials = New System.Net.NetworkCredential(login, pwd)
                clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                ' read in file...
                Dim bFile() As Byte = System.IO.File.ReadAllBytes(readPath)

                ' upload file...
                Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()

                clsStream.Write(bFile, 0, bFile.Length)
                clsStream.Close()
                clsStream.Dispose()
                Console.ReadLine()
            End Using

        Catch ex As Exception

        End Try

    End Sub

    Sub RunEcomplianceScheduler()
        If (DateTime.Now.Hour = 1 And DateTime.Now.Minute <= 10) Then
            Dim objcls = New EcompScheduler()
            'objcls.RunScheduler()
        End If

    End Sub

    '' being called in myndsaas window service (every 10 minutes)
    Public Function Run_FTP_Inward_Integration() As String Implements IDMSService.Run_FTP_Inward_Integration
        '' for consolidated auto mail to myedms users 
        Try
            ' AutoRunLog("Run_FTP_Inward_Integration", "AutoRunSchedulerforDMS", "Calling Function Now", 0)
            AutoRunSchedulerforDMS()
            ' AutoRunLog("Run_FTP_Inward_Integration", "AutoRunSchedulerforDMS", "Calling Function complete", 0)
        Catch ex As Exception
            AutoRunLog("Run_FTP_Inward_Integration", "AutoRunSchedulerforDMS", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        '' for ecompliance account -
        'Try
        '    'RunEcomplianceScheduler()
        'Catch ex As Exception
        '    AutoRunLog("Run_FTP_Inward_Integration", "RunEcomplianceScheduler", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        'End Try

        '' for tally integration
        Try
            'AutoRunLog("Run_FTP_Inward_Integration", "Run_FTP_Inwared_Integration_Tally", "Calling Function Now", 0)
            Run_FTP_Inwared_Integration_Tally()
            ' AutoRunLog("Run_FTP_Inward_Integration", "Run_FTP_Inwared_Integration_Tally", "Calling Function complete", 0)
        Catch ex As Exception
            AutoRunLog("Run_FTP_Inward_Integration", "Run_FTP_Inwared_Integration_Tally", "TC Exception msg - " & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        '' for sending sla expiry alerts to PRA docs - HCL AR  - BY SP on 02-feb-16
        Try
            ' Run_HCL_AR_PRA_SLAexpiry_alerts()  ' daily alerts two times 5 pm and 7 pm 
        Catch ex As Exception
            AutoRunLog("Run_HCL_AR_PRA_SLAexpiry_alerts", "HCL alerts main", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        Try
            '    ' temp disabled on request by puneet on 30 aug (latest updated on 30 aug mid noon - for header issue and added bpm id , address, note
            Hcl_VENDOR_INVOICE_VP_alert_REMINDER()  ' daily alerts 7 am for hcl vendor invoice vp - to approvers 
        Catch ex As Exception
            AutoRunLog("Hcl_VENDOR_INVOICE_VP_alert_REMINDER", "HCL alerts AP", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        Try
            '    ' temp disabled on request by puneet on 30 aug (latest updated on 30 aug mid noon - for header issue and added bpm id , address, note
            '' will be enabled once template is confirmed by HCL team.
            Hcl_VENDOR_INVOICE_VP_Next_alert_REMINDER()  ' daily alerts 8.30 am for hcl vendor invoice vp - to Next approvers 
        Catch ex As Exception
            AutoRunLog("Hcl_VENDOR_INVOICE_VP_Next_alert_REMINDER()", "HCL alerts AP", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try



        Try
            Ticket_Esclation_Mail()
        Catch ex As Exception
            AutoRunLog("Ticket_Auto_SLA_alert_REMINDER", "Ticket Auto SLA Expired Mail", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try
        'Auto SLA Expired for Ticket Mail'
        'Auto_Ticket_Closure for Ticket Mail'
        Try
            Dim objATC As New AutoTicketScheduler()
            objATC.AutoTicketClosure()
        Catch ex As Exception
            AutoRunLog("Ticket_Auto_Ticket_Closure_REMINDER", "Ticket Auto Ticket Closure Mail", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try
        ' Auto_Ticket_Closure for Ticket Mail'


        ' Try
        '     AutoRunLog("Run_Helpdesk_MailRead_Sch", "Before call", "1", 0)
        ' apicall("http://myndsaas.com/DMSService.SVC/Run_Helpdesk_MailRead_Sch")
        '     AutoRunLog("Run_Helpdesk_MailRead_Sch", "After call", "2", 0)
        ' Catch ex As Exception
        '     AutoRunLog("Run_Helpdesk_MailRead_Sch", "Run_Helpdesk_MailRead_Sch", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        ' End Try
        Try
            ' herehere
            Call SendAlertMails()
            Call SendDocumentExpiryMails()
        Catch ex As Exception
            AutoRunLog("SAL_DOC_EXPIRY_Reminders", "SAL DOC EXPIRY Reminders", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        '' below is for calling report scheduler auto mailer with report by vishal 06-dec-18
        Try
            Call ReportSchedulerMail()
        Catch ex As Exception
            AutoRunLog("ReportSchedulerMail", "Report Scheduler Mail", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        ''''FTP Transfer Reports
        Try
            FTPREPORT()
        Catch ex As Exception
            AutoRunLog("FTPReportScheduler", "FTP Report Scheduler", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        '''' Call Rental tool related main fuction balow  - 08-July-20 by Chandni
        Try
            AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "Calling Function Now", 0)
            Dim objRental As New Rentaltool
            objRental.AutoInvoice()
            AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "End Function Now", 0)
        Catch ex As Exception
            AutoRunLog("Run_FTP_Inward_Integration", "AutoInvoice", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        '''' End of Call Rental tool related main fuction


        Return ""
    End Function

    'Auto Closure Ticket Mail'
    Private Sub Ticket_Esclation_Mail()
        Dim objDT As New DataTable()
        Dim objDC As New DataClass()
        objDT = objDC.ExecuteQryDT("select * from mmm_hdmail_schdule where isactive=1")
        For Each dr As DataRow In objDT.Rows
            If TicketScheduler(Convert.ToString(dr("EsclationTime"))) Then
                GetTicketEsclation(Convert.ToString(dr("DocumentType")), Convert.ToInt32(dr("EID")), IIf(IsDBNull(dr("WS")), "OPEN", Convert.ToString(dr("WS"))), IIf(IsDBNull(dr("asla")), 0, dr("asla")), IIf(IsDBNull(dr("bsla")), 0, dr("bsla")), Convert.ToString(dr("mdmailid")), Convert.ToString(dr("mdpwd")), Convert.ToString(dr("hostname")))
            End If
        Next

    End Sub


    Function TicketScheduler(ByVal EsclationTime As String) As Boolean
        Dim b As Boolean = False
        Try
            Dim i As Integer = 0
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            ' Dim date1f As String
            If (EsclationTime.ToString() <> "") Then
                'Dim schedule As String = "*|*|*|*|*"
                Dim schedule As String = EsclationTime.ToString()
                Dim str_array As [String]() = schedule.Split("|")
                Dim stringa As [String] = str_array(0)
                Dim stringb As [String] = str_array(1)
                Dim stringc As [String] = str_array(2)
                Dim stringd As [String] = str_array(3)
                Dim stringe As [String] = str_array(4)
                Dim currentTime As System.DateTime = System.DateTime.Now
                Dim currentdate As String = currentTime.Date.Day
                Dim str_datte As [String]() = stringb.Split(",")
                Dim str_hours As [String]() = stringd.Split(",")
                Dim str_mintus As [String]() = stringe.Split(",")

                If Trim(stringa) = "*" And Trim(stringb) = "*" And Trim(stringc) = "*" And Trim(stringd) = "*" And Trim(stringe) = "*" Then
                    b = True
                    Return b
                    ' Exit For
                End If

                If stringb <> "*" Then
                    For j As Integer = 0 To str_datte.Length - 1
                        Dim A As [String] = str_datte(j)
                        If A.Contains("-") Then
                            Dim o As [String]() = A.Split("-")
                            Dim f As [String] = o(0)
                            Dim g As [String] = o(1)
                            If (f <= currentdate And g >= currentdate) Then
                                For n As Integer = 0 To str_hours.Length - 1
                                    For m As Integer = 0 To str_mintus.Length - 1
                                        Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                        If x <= time2 And x >= time1 Then
                                            b = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If
                        Else
                            If ((currentdate = A)) Then
                                For n As Integer = 0 To str_hours.Length - 1
                                    For m As Integer = 0 To str_mintus.Length - 1
                                        Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                        If x <= time2 And x >= time1 Then
                                            b = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    Next
                ElseIf ((currentdate = stringb) Or (stringb.Trim() = "*") Or (stringd.Trim() <> "") Or (stringe.Trim() <> "")) Then
                    For n As Integer = 0 To str_hours.Length - 1
                        For m As Integer = 0 To str_mintus.Length - 1
                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                            ' Dim x As DateTime = (Convert.ToDateTime(stringd & ":" & stringe & ":" & "00").ToShortTimeString)
                            If x <= time2 And x >= time1 Then
                                b = True
                                Exit For
                            End If
                        Next
                    Next
                ElseIf ((stringa.Trim() = "*") Or (stringb.Trim() = "*") Or (stringc.Trim() = "*") Or (stringd.Trim() = "*") Or (stringe.Trim() = "*")) Then
                    b = True
                    'Exit For
                End If
            End If
            Return b
        Catch ex As Exception
        End Try
    End Function

    Private Sub GetTicketEsclation(ByRef documenttype As String, ByRef eid As Integer, ByRef WS As String, ByRef asla As Integer, ByRef bsla As Integer, ByRef mailFrom As String, ByRef mailPassword As String, ByRef mailHost As String)
        Dim objDC As New DataClass()
        Dim ht As New Hashtable()
        Dim dtUsers As New DataTable()
        ht.Add("@En", documenttype)
        ht.Add("@Ws", WS)
        ht.Add("@Asla", asla)
        ht.Add("@Bsla", bsla)
        ht.Add("@Eid", eid)
        dtUsers = objDC.ExecuteProDT("uspAlertMailSLA_Ticket", ht)
        'Dim cc As String = objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' + isnull(emailid,'') from mmm_mst_user where  eid=" & eid & " and userrole='ADMIN' FOR XML PATH('')) ,1,1,'') as Response")
        Dim cc As String = ""
        For k As Integer = 0 To dtUsers.Rows.Count - 1
            Dim MailTo As New ArrayList()
            Dim MailSubject As String = ""
            Dim MailCC As New ArrayList()
            Dim MailBody As New StringBuilder()
            Dim TempMailBody As New StringBuilder()
            Dim ToUser As String = ""
            If Not IsDBNull(dtUsers.Rows(k).Item("userid")) Then
                Dim dtDtl As New DataTable()
                Dim curUser As Integer = dtUsers.Rows(k).Item("userid").ToString()
                ht.Clear()
                ht.Add("@En", documenttype)
                ht.Add("@Ws", WS)
                ht.Add("@Asla", asla)
                ht.Add("@Bsla", bsla)
                ht.Add("@Eid", eid)
                ht.Add("@curuser", curUser)
                dtDtl = objDC.ExecuteProDT("uspAlertMailSLA_getTicketUserIDList", ht)
                Dim ColList As String = ""
                If dtDtl.Rows.Count > 0 Then

                    Dim uStr As String = "SELECT emailid  from MMM_MST_user where eid=" & eid & " and uid=" & curUser
                    Dim dtU As New DataTable()
                    dtU = objDC.ExecuteQryDT(uStr)
                    ToUser = Convert.ToString(dtU.Rows(0).Item("emailid"))

                    ColList = dtDtl.Rows(0).Item(0).ToString()
                    Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select username from mmm_mst_user where  uid=d.fld14) [Requester Name],fld2[Requestor],fld3[Subject],fld4[Query],fld5[MessageID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld8,0) from mmm_mst_master where tid=isnull(d.fld8,0) and documenttype='Department')) [Category],( select username from mmm_mst_user where  uid=d.fld7) [Assignee],fld10[Agent Comments],fld12[CC],fld13[BCC],fld15[Ticket Attachments],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld16,0) from mmm_mst_master where tid=isnull(d.fld16,0) and documenttype='Organizations')) [Organization],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Sub Department')) [Sub Category],fld18[Ticket Priority],case when isnull(fld19,0)=0 then 2 when isnull(fld19,0)='' then 2 else isnull(fld19,0)/24 end [SLA] from MMM_MST_DOC d  " & " where tid in (" & ColList & ")"
                    Dim dtQuery As New DataTable()
                    dtQuery = objDC.ExecuteQryDT(strQry)
                    If dtQuery.Rows.Count > 0 Then
                        TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""> <font face=""arial, helvetica, sans-serif"" size=""2""> <b style=""text-align: center; line-height: 1.6em;"">Dear Sir/Madam, </b></p><br> The SLA for the following Invoices have <u>Expired</u>. Please log-In to Help Desk Portal to Approve/Reject these Tickets immediately:<br><br></font>")
                        TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                        TempMailBody.Append("<tr style=""background-color:#87CEFA""><td><font face=""arial, helvetica, sans-serif"" size=""2""> Ticket ID </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Organization </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Requester Name </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Subject </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Category </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Sub Category </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Priority </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> SLA in (Days) </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Last Action by </font></td><td> <font face=""arial, helvetica, sans-serif"" size=""2""> Last Action Date </font></td></tr>")
                        For Each dr As DataRow In dtQuery.Rows
                            TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("SYSTICKETID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Organization")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Requester Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Subject")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Category")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Sub Category")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Ticket Priority")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToInt32(dr("SLA")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("LastActionDate")) & "</font></td>")
                        Next
                        TempMailBody.Append("</table>")
                        TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""><br></p><p style=""margin: 0in 0in 0.0001pt;""><br></p><font face=""arial, helvetica, sans-serif"" size=""2"">You can access <b> 'HELP DESK PORTAL' </b> through ""helpDesk"" by log into Help Desk Portal.</font><p style=""margin: 0in 0in 0.0001pt;""></p> <font  face=""arial, helvetica, sans-serif"" size=""2""> <p><b>Regards</b></p><p>Ticket Help Desk</p><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""><div><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""></b></div>This is a System generated mail. Please do not reply to this mail</b><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;"">!</b><br><br></font>")
                    End If
                    SendMail(TempMailBody.ToString(), ToUser, "Ticket SLA Expired Reminder", cc:=cc, mailFrom:=mailFrom, mailPassword:=mailPassword, mailHost:=mailHost, alertType:=eid & "_Agent_Alert_Mail", mailEvent:="Ticket-" & eid & "_" & WS.ToString(), eid:=eid)
                End If
            End If
        Next
    End Sub


    Private Sub GetAdminTicketEsclation(ByRef documenttype As String, ByRef eid As Integer, ByRef WS As String, ByRef asla As Integer, ByRef bsla As Integer, ByRef mailFrom As String, ByRef mailPassword As String, ByRef mailHost As String)
        Dim objDC As New DataClass()
        Dim dtDtl As New DataTable()
        Dim ht As New Hashtable()
        Dim dtUsers As New DataTable()
        Dim TempMailBody As New StringBuilder()
        ht.Add("@En", documenttype)
        ht.Add("@Ws", WS)
        ht.Add("@Asla", asla)
        ht.Add("@Bsla", bsla)
        ht.Add("@Eid", eid)
        dtUsers = objDC.ExecuteProDT("uspAlertMailSLA_Ticket", ht)
        Dim curUser As New ArrayList()
        For Each dr As DataRow In dtUsers.Rows
            curUser.Add(dr(0))
        Next
        If curUser.Count > 0 Then
            ht.Clear()
            ht.Add("@En", documenttype)
            ht.Add("@Ws", WS)
            ht.Add("@Asla", asla)
            ht.Add("@Bsla", bsla)
            ht.Add("@Eid", eid)
            ht.Add("@curuser", String.Join(",", curUser.ToArray()))
            dtDtl = objDC.ExecuteProDT("uspAlertMailSLA_Admin", ht)
            If dtDtl.Rows.Count > 0 Then
                Dim ToUser As String = objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' + isnull(emailid,'') from mmm_mst_user where  eid=" & eid & " and userrole='ADMIN' FOR XML PATH('')) ,1,1,'') as Response")
                Dim cc As String = ""
                Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select username from mmm_mst_user where  uid=d.fld14) [Requester Name],fld2[Requestor],fld3[Subject],fld4[Query],fld5[MessageID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld8,0) from mmm_mst_master where tid=isnull(d.fld8,0) and documenttype='Department')) [Category],( select username from mmm_mst_user where  uid=d.fld7) [Assignee],fld10[Agent Comments],fld12[CC],fld13[BCC],fld15[Ticket Attachments],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld16,0) from mmm_mst_master where tid=isnull(d.fld16,0) and documenttype='Organizations')) [Organization],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Sub Department')) [Sub Category],fld18[Ticket Priority],case when isnull(fld19,0)=0 then 2 when isnull(fld19,0)='' then 2 else isnull(fld19,0)/24 end [SLA] from MMM_MST_DOC d  " & " where tid in (" & dtDtl.Rows(0).Item(0).ToString() & ")"
                Dim dtQuery As New DataTable()
                dtQuery = objDC.ExecuteQryDT(strQry)
                If dtQuery.Rows.Count > 0 Then
                    TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""> <font face=""arial, helvetica, sans-serif"" size=""2""> <b style=""text-align: center; line-height: 1.6em;"">Dear Sir/Madam, </b></p><br> The SLA for the following Invoices have <u>Expired</u>. Please log-In to Help Desk Portal to Approve/Reject these Tickets immediately:<br><br></font>")
                    TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                    TempMailBody.Append("<tr style=""background-color:#87CEFA""><td><font face=""arial, helvetica, sans-serif"" size=""2""> Ticket ID </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Organization </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Requester Name </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Subject </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Category </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Sub Category </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Priority </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> SLA in (Days) </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Last Action by </font></td><td> <font face=""arial, helvetica, sans-serif"" size=""2""> Last Action Date </font></td></tr>")
                    For Each dr As DataRow In dtQuery.Rows
                        TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("SYSTICKETID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Organization")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Requester Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Subject")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Category")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Sub Category")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("Ticket Priority")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToInt32(dr("SLA")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(dr("LastActionDate")) & "</font></td>")
                    Next
                    TempMailBody.Append("</table>")
                    TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""><br></p><p style=""margin: 0in 0in 0.0001pt;""><br></p><font face=""arial, helvetica, sans-serif"" size=""2"">You can access <b> 'HELP DESK PORTAL' </b> through ""helpDesk"" by log into Help Desk Portal.</font><p style=""margin: 0in 0in 0.0001pt;""></p> <font  face=""arial, helvetica, sans-serif"" size=""2""> <p><b>Regards</b></p><p>Ticket Help Desk</p><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""><div><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""></b></div>This is a System generated mail. Please do not reply to this mail</b><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;"">!</b><br><br></font>")
                    SendMail(TempMailBody.ToString(), ToUser, "Ticket SLA Expired Reminder", cc:=cc, mailFrom:=mailFrom, mailPassword:=mailPassword, mailHost:=mailHost, alertType:=eid & "_Admin_Alert_Mail", mailEvent:="Ticket-" & eid & "_" & WS.ToString(), eid:=eid)
                End If
            End If
        End If


    End Sub



    Private Sub Ticket_Admin_Esclation_Mail()
        Dim objDT As New DataTable()
        Dim objDC As New DataClass()
        objDT = objDC.ExecuteQryDT("select * from mmm_hdmail_schdule where isactive=1")
        For Each dr As DataRow In objDT.Rows
            If TicketScheduler(Convert.ToString(dr("Admin_EsclationTime"))) Then
                GetAdminTicketEsclation(Convert.ToString(dr("DocumentType")), Convert.ToInt32(dr("EID")), IIf(IsDBNull(dr("WS")), "OPEN", Convert.ToString(dr("WS"))), IIf(IsDBNull(dr("asla")), 0, dr("asla")), IIf(IsDBNull(dr("bsla")), 0, dr("bsla")), Convert.ToString(dr("mdmailid")), Convert.ToString(dr("mdpwd")), Convert.ToString(dr("hostname")))
            End If
        Next
    End Sub




    Function SendMail(ByRef TemplateBody As String, ByRef MailTo As String, ByRef MailSubject As String, ByRef cc As String, ByRef mailFrom As String, ByRef mailPassword As String, ByRef mailHost As String, ByRef alertType As String, ByRef mailEvent As String, ByRef eid As Integer) As Boolean
        Dim bool As Boolean = True
        If TemplateBody.ToString().Length > 1 Then
            Dim objDC As New DataClass()
            '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
            '    Dim objMail As New MailSend(mailFrom, mailPassword, mailHost)
            Dim bcc As String = "mayank.garg@myndsol.com,sunil.pareek@myndsol.com"
            '    objMail.SendMail(Mto:=MailTo.ToString(), MSubject:=MailSubject, MBody:=TemplateBody.ToString(), cc:=cc, bcc:=bcc)

            Dim obj As New MailUtill(eid:=100)
            obj.SendMail(ToMail:=MailTo, Subject:=MailSubject, MailBody:=TemplateBody.ToString(), CC:=cc, BCC:=bcc)


            'If bool = False Then
            '    Dim finalBody As String = "Error in " & MailSubject.ToString() & " <br/> " & ""
            '    objMail.SendMail(Mto:="sunil.pareek@myndsol.com,mayank.garg@myndsol.com", MSubject:=finalBody.ToString(), MBody:=TemplateBody.ToString(), cc:=cc, bcc:=bcc)
            'End If
            Dim htTran As New Hashtable()
            htTran.Add("@MAILTO", MailTo)
            htTran.Add("@CC", cc)   ' String.Join(",", MailCC.ToArray())
            htTran.Add("@MSG", TemplateBody.ToString())
            htTran.Add("@ALERTTYPE", alertType)
            htTran.Add("@MAILEVENT", mailEvent)
            htTran.Add("@EID", eid)
            objDC.ExecuteProDT("INSERT_MAILLOG", htTran)
            Return bool
        Else
            bool = False
        End If
    End Function


    ''' <remarks> below is for escalation to next approver</remarks>
    Private Sub Hcl_VENDOR_INVOICE_VP_Next_alert_REMINDER()
        Try
            Dim isTime As Boolean = False
            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 830 And cTime <= 840) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim objDC As New DataClass()
                Dim dtUsers As New DataTable()
                Dim dtNextUsers As New DataTable()
                Dim arrWFStatus() = {"APPROVER 1", "APPROVER 2"}
                Dim arrInoviceType() = {"958564", "958565", "1078985"}
                Dim EventName As String = "Vendor Invoice VP"
                Dim bsla As Integer = 54
                Dim asla As Integer = 99999
                Dim eid As Integer = 46
                For i As Integer = 0 To arrWFStatus.Length - 1
                    Try
                        Dim ht As New Hashtable()
                        ht.Add("@En", EventName)
                        ht.Add("@Ws", arrWFStatus(i))
                        ht.Add("@Asla", asla)
                        ht.Add("@Bsla", bsla)
                        ht.Add("@Eid", eid)
                        ht.Add("@FTYPE", "")
                        dtUsers = objDC.ExecuteProDT("uspAlertMailSLA_getAllUsersWithDocID_VIPHCL", ht)
                        'Now Get Distinct UserID's on Next Approver
                        Dim DynamicColumn As String = ""
                        Dim DynamicCurrentColumn As String = ""
                        If i = 0 Then
                            DynamicCurrentColumn = "L1 Approver"
                            DynamicColumn = "L2 Approver"
                        ElseIf i = 1 Then
                            DynamicCurrentColumn = "L2 Approver"
                            DynamicColumn = "L3 Approver"
                            'ElseIf i = 2 Then
                            '    DynamicCurrentColumn = "L3 Approver"
                            '    DynamicColumn = "L4 Approver"
                            'ElseIf i = 3 Then
                            '    DynamicCurrentColumn = "L4 Approver"
                            '    DynamicColumn = "L5 Approver"
                        End If

                        Dim NextUserView As DataView = dtUsers.DefaultView
                        Dim distinctValues As DataTable = NextUserView.ToTable(True, "" & DynamicColumn & "")

                        For Each drUserData As DataRow In distinctValues.Rows
                            Dim ToUser As String = ""
                            Dim filteredRow As DataRow() = dtUsers.Select("[" & DynamicColumn & "] =" & drUserData(0).ToString() & "")
                            Dim FinalDocIDBasedOnUser As New ArrayList()
                            For Each final As DataRow In filteredRow
                                FinalDocIDBasedOnUser.Add(final("DOCID"))
                            Next
                            'Now Get Distinct TIDS based on Next Approver
                            If FinalDocIDBasedOnUser.Count > 0 And drUserData(0).ToString() <> "0" Then
                                Dim MailBody As New StringBuilder()
                                Dim TempMailBody As New StringBuilder()
                                Dim MailTo As New ArrayList()
                                Dim MailCC As New ArrayList()
                                Dim MailSubject As String = ""
                                Dim uStr As String = "SELECT emailid  from MMM_MST_user where eid=46 and uid=" & drUserData(0).ToString()
                                Dim dtU As New DataTable()
                                dtU = objDC.ExecuteQryDT(uStr)
                                ToUser = Convert.ToString(dtU.Rows(0).Item("emailid"))
                                ' Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code],fld180[Payment Terms],fld181[INCOTerms],fld32[Location],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],fld160[WBS Description],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],fld110[sep3forPO],fld107[Sep2forPO],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],fld106[sep1forPO],fld111[sep4forPO],fld95[sep5forPO],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld42,0) from mmm_mst_master where tid=isnull(d.fld42,0) and documenttype='Doc Nature')) [Doc Nature],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [Company Code],fld3[RAO Remarks],fld15[Company Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld136,0) from mmm_mst_master where tid=isnull(d.fld136,0) and documenttype='Profit Center')) [Profit Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld135,0) from mmm_mst_master where tid=isnull(d.fld135,0) and documenttype='Cost Center')) [Cost Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],fld58[Email Of User], (select fld1 from mmm_mst_master where tid = (select ISNULL(NULLIF(d.fld2, 0), 0)  from mmm_mst_master where tid=ISNULL(NULLIF(d.fld2, 0), 0) and documenttype='currency' ))[Currency] ,fld31[Dispatch Remarks],fld34[Physical Doc (Recd)],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld12[SSC Processing Date],fld182[Service Tax Categor],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld124[Low TDS Applicable],fld125[Low TDS Certificate],fld129[Remarks If any],fld130[Invoicing Milestone Description],fld108[Sep1forChklist],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[Clarification Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Remarks by HCL Receipt User],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld52[Processor Cancellation Remarks],fld53[Processor Reconsider Remarks],fld86[Warranty Certificate Attachment],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld75[Technical Compliance Certificate],fld4[Courier Docket No.],fld30[Dispatch Date],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='Rejection Reasons')) [Rejection Reason],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld51[Query Sent To],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld57[Parking Date],fld79[Proof of Electronics delivery],fld36[SSC Processing Remarks],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld13[Other Deduction],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld6,0) from mmm_mst_master where tid=isnull(d.fld6,0) and documenttype='Clarification User')) [User Email],fld67[Open Amount],fld7[Fiscal Year Posted],fld62[HC Rejection Remarks],fld63[Invoice Received Date],fld43[MIGO No],fld64[Dispatch Reconsider Remarks],fld65[QC Reconsider Remarks],( select username from mmm_mst_user where  uid=d.fld50) [Clarification by user],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld185[Monthly Fee],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld163[Supporting Attachment],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld206[PO Deleted],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld208[PO Date] from MMM_MST_DOC d  " & " where tid in (" & String.Join(",", FinalDocIDBasedOnUser.ToArray()) & ") order by [Invoice Type] asc"
                                Dim strQry As String = " SELECT DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], dms.GetUnAssignedAgent(tid) [GETUNASSIGNEDAGENT], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld208[PO Date],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code Description],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld180,0) from mmm_mst_master where tid=isnull(d.fld180,0) and documenttype='Payment Terms Master')) [Payment Term Description],fld181[INCOTerms],(select fld1 from mmm_mst_master where tid in (select fld3 from mmm_mst_master where documenttype='Plant Master' and eid= 46 and tid=d.fld128))[Invoice Submitters Location],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld63,0) from mmm_mst_master where tid=isnull(d.fld63,0) and documenttype='State Master')) [HCL State Billed by Vendor (Business Place)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld62,0) from mmm_mst_master where tid=isnull(d.fld62,0) and documenttype='State Master')) [State – Mat or Services delivered (Place of Supply)],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],fld217[L1 Approver (Dept)],fld218[L2 Approver (Dept)],fld219[L3 Approver (Dept)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],fld160[WBS Description],fld110[sep3forPO],fld214[L1 Approver (WBS)],fld210[WBS Description (Non-PO)],fld215[L2 Approver (WBS)],fld107[Sep2forPO],fld216[L3 Approver (WBS)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],fld42[Vendor GSTN Status],fld43[Vendor GSTIN],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld57,0) from mmm_mst_master where tid=isnull(d.fld57,0) and documenttype='State Master')) [State - from where Vendor Billed],fld58[Email Of User],fld31[Dispatch Remarks],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [HCL Entity Code],fld15[HCL Entity Name],fld34[Physical Doc (Recd)],( select fld5 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='GSTIN Master')) [HCL Entity GSTIN],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld182[Service Tax Categor],(select fld1 from mmm_mst_master where tid = isnull(d.fld2,0) and eid=46 and documenttype='currency') [Currency], (select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld130[Expense or Milestone Desc],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld50[CGST],fld51[SGST],fld52[IGST],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld129[Remarks If any],fld124[Low TDS Applicable],fld108[Sep1forChklist],fld125[Low TDS Certificate],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[VHD Reconsider Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Hcl Entity (As per Vendor Invoice)],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld86[Warranty Certificate Attachment],fld53[Processor Reconsider Remarks],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld30[Dispatch Date],fld4[Courier Docket No.],fld75[Technical Compliance Certificate],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld79[Proof of Electronics delivery],fld36[Is Additional Approval Reqd for Payment],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],fld7[Fiscal Year Posted],fld67[Open Amount],fld64[Submiter Rejection Remarks],fld65[QC Reconsider Remarks],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld66[GSTN Status of Retainer],fld185[Monthly Fee],fld95[GSTIN of Retainer],( select fld5 from mmm_mst_master where  tid=(select isnull(d.fld111,0) from mmm_mst_master where tid=isnull(d.fld111,0) and documenttype='GSTIN Master')) [HCL GSTIN],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld106,0) from mmm_mst_master where tid=isnull(d.fld106,0) and documenttype='State Master')) [State - from where Retainer Billed],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld163[Supporting Attachment],fld206[PO Deleted],fld209[GL Codes],fld213[Internal Order No],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld220[PUR GP Description] from MMM_MST_DOC d  " & " where tid in (" & String.Join(",", FinalDocIDBasedOnUser.ToArray()) & ") order by [Invoice Type] asc"

                                Dim dtQuery As New DataTable()
                                dtQuery = objDC.ExecuteQryDT(strQry)
                                TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""> <font face=""arial, helvetica, sans-serif"" size=""2""> <b style=""text-align: center; line-height: 1.6em;"">Dear " & dtQuery.Rows(0)("" & DynamicColumn & "") & ", </b></p><br> This is to informed you that the SLA of below invoices have expired and are pending for action with respective users (Please Refer Last Column).<br><br>This email is reached to you as you are  <b> Next Approver (" & DynamicColumn & ") </b> for below invoices. Please issue necessary instructions to your team members to Approve/Reject below invoices.</font>")

                                '''' ***** PO Based ****
                                Dim viewPO As DataView = dtQuery.DefaultView
                                viewPO.RowFilter = "[Invoice Type] = 'PO BASED'"
                                If viewPO.ToTable().Rows.Count > 0 Then
                                    TempMailBody.Append("<font face=""arial, helvetica, sans-serif"" size=""3""> <p style=""background-color:#fff;color:#104E8B"" >Invoice Type : <b> PO Based </b></p></font>  ")
                                    TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                                    TempMailBody.Append("<tr style=""background-color:#FF3030""><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" color=""#FFFFFF"" size=""2""> BPM ID </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> PO No. </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Vendor Code </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Vendor Name </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> WBS No. </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Department </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Invoice No. </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Invoice Date </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Invoice Value (with Tax) </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Last Action by </font></td><td> <font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending From </font></td>")
                                    ' TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending With</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Next Approver</font></td></tr>")
                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending With</font></td></tr>")
                                End If
                                For z As Integer = 0 To viewPO.ToTable().Rows.Count - 1
                                    MailTo.Add(Convert.ToString(viewPO.ToTable().Rows(z)("CURRENT USER")))
                                    TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("BPM ID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("PO No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Vendor Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Vendor Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("WBS No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Department")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Invoice No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Invoice Date")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">(" & Convert.ToString(viewPO.ToTable().Rows(z)("Currency")) & ") " & Convert.ToString(viewPO.ToTable().Rows(z)("Total Invoice Amount")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("LastActionDate")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("" & DynamicCurrentColumn.ToString() & "")) & "</font></td></tr>")
                                Next
                                If viewPO.ToTable().Rows.Count > 0 Then
                                    TempMailBody.Append("</table>")
                                End If

                                '''' ***** non po ****
                                Dim viewNonPO As DataView = dtQuery.DefaultView
                                viewNonPO.RowFilter = "[Invoice Type] = 'NON PO BASED'"
                                If viewNonPO.ToTable().Rows.Count > 0 Then
                                    TempMailBody.Append("<font face=""arial, helvetica, sans-serif"" size=""3""> <p style=""background-color:#fff;color:#104E8B"" >Invoice Type : <b> NON PO Based</b></p></font>")
                                    TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                                    TempMailBody.Append("<tr style=""background-color:#FF3030""><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" color=""#FFFFFF"" size=""2""> BPM ID </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" color=""#FFFFFF"" size=""2""> Vendor Code </font><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Vendor Name </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> WBS No. </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Department </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Invoice No. </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Invoice Date </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Invoice Value (with Tax) </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2""> Last Action by </font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending From</font></td>")
                                    'TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending With</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Next Approver</font></td></tr>")
                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending With</font></td></tr>")
                                End If
                                For z As Integer = 0 To viewNonPO.ToTable().Rows.Count - 1
                                    MailTo.Add(Convert.ToString(viewNonPO.ToTable().Rows(z)("CURRENT USER")))
                                    TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("BPM ID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Vendor Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Vendor Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("WBS No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Department")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Invoice No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Invoice Date")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">(" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Currency")) & ") " & Convert.ToString(viewNonPO.ToTable().Rows(z)("Total Invoice Amount")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("LastActionDate")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("" & DynamicCurrentColumn & "")) & "</font></td></tr>")
                                Next
                                If viewNonPO.ToTable().Rows.Count > 0 Then
                                    TempMailBody.Append("</table>")
                                End If

                                '''' ***** retainer Based ****
                                Dim viewRetainer As DataView = dtQuery.DefaultView
                                viewRetainer.RowFilter = "[Invoice Type] = 'RETAINERS'"
                                If viewRetainer.ToTable().Rows.Count > 0 Then
                                    TempMailBody.Append("<font face=""arial, helvetica, sans-serif"" size=""3""> <p style=""background-color:#fff;color:#104E8B"">Invoice Type : <b> Retainers</b></p></font>")
                                    TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                                    TempMailBody.Append("<tr style=""background-color:#FF3030""><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">BPM ID</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Consultant Emp. Code</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Consultant Vendor Code</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Consultant Name</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Contract Expiry</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">For Month</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Working Days</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Amount Claimed</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Mobile Number</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Company Code</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Cost Center</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Department</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Last Action by</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending From</font></td>")
                                    'TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending With</font></td><td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Next Approver</font></td></tr>")
                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" color=""#FFFFFF"" size=""2"">Pending With</font></td></tr>")
                                End If
                                For z As Integer = 0 To viewRetainer.ToTable().Rows.Count - 1
                                    MailTo.Add(Convert.ToString(viewRetainer.ToTable().Rows(z)("CURRENT USER")))
                                    TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("BPM ID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Consultant Emp Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Consultant Vendor Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Consultant Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Contract Expiry")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("For Month")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Working Days")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">(INR)" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Amount Claimed")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Mobile Number")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Company Code Retainer")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Cost Center Retainer")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Department")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("LastActionDate")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("" & DynamicCurrentColumn & "")) & "</font></td></tr>")
                                Next
                                If viewRetainer.ToTable().Rows.Count > 0 Then
                                    TempMailBody.Append("</table>")
                                End If
                                'Uncomment but check before what need over there 
                                'MailCC.Add(objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' +  emailid  from mmm_mst_user where uid in ( select distinct fld120[L2] from mmm_mst_doc where  tid in (" & ColList & "))                  FOR XML PATH('')), 1, 1, '') AS [Output]"))
                                If i = 0 Then
                                    MailSubject = "PEARL- Vendor Invoices Approvals Pending (Level-1)"
                                ElseIf i = 1 Then
                                    MailSubject = "PEARL- Vendor Invoice Approvals Pending (Level-2)"
                                    'ElseIf i = 2 Then
                                    '    MailSubject = "PEARL- Vendor Invoice Approvals Pending (Level-3)"
                                    'Else
                                    '    MailSubject = "PEARL- Vendor Invoice Approvals Pending (Level-4)"
                                End If
                                MailBody.Append(TempMailBody.ToString())

                                Dim finalBody As String = MailBody.ToString()
                                If finalBody.Length > 10 Then
                                    MailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""><br></p><p style=""margin: 0in 0in 0.0001pt;""><br></p><font face=""arial, helvetica, sans-serif"" size=""2""> For more details, You can access <b> 'PEARL Portal' </b> through HCL Intranet Homepage using PEARL Icon. Click  <a href=""https://intranet.hclinsys.com""> Here </a> to Login</font><p style=""margin: 0in 0in 0.0001pt;""></p> <font  face=""arial, helvetica, sans-serif"" size=""2""> <p><b>Regards</b></p><p>Vendor Help Desk<br/>Procurement Department<br/>HCL Infosystems Limited<br/> E - 4,5,6 Sector - XI,<br/>Noida - 201301 U.P. (India)<br/>Email : vendorhelpdesk@hcl.com</b></p><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""><div><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""></b></div>This is a System generated mail. Please do not reply to this mail</b><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;"">!</b><br><br></font>")
                                    finalBody = MailBody.ToString()
                                    Dim htTran As New Hashtable()
                                    htTran.Add("@MAILTO", ToUser)
                                    htTran.Add("@CC", "")   ' String.Join(",", MailCC.ToArray())
                                    htTran.Add("@MSG", finalBody.ToString())
                                    htTran.Add("@ALERTTYPE", "VP_ALERT_Escalation_MAIL")
                                    htTran.Add("@MAILEVENT", EventName & "-" & arrWFStatus(i).ToString())
                                    htTran.Add("@EID", eid)
                                    objDC.ExecuteProDT("INSERT_MAILLOG", htTran)
                                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                    ' sendMail1(ToUser, "", "", MailSubject, finalBody)
                                    Dim obj As New MailUtill(eid:=eid)
                                    obj.SendMail(ToMail:=ToUser, Subject:=MailSubject, MailBody:=finalBody, CC:="", BCC:="")
                                End If
                            Else
                                objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP',' No list found and workflow status is " & arrWFStatus(i).ToString() & " and current userid is:-" & drUserData(0).ToString() & "  ',getdate()," & eid & ")")
                            End If
                        Next
                    Catch ex As Exception
                        objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " ',getdate()," & eid & ")")
                    End Try
                Next
            End If
        Catch ex As Exception
            'objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_Escalation','Try Catch Error','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " ',getdate(),46)")
            Dim finalBody As String = "Error in HCL Reminder mail sending <br/> " & "Error message is - " & Convert.ToString(ex.Message) & "<br/> Inner message is - " & Convert.ToString(ex.InnerException.Message)
            ' sendMail1("sunil.pareek@myndsol.com", "", "", "Error in HCL Reminder mail sending", finalBody)
        End Try
    End Sub



    ''' <remarks> below is for mail to Users (for pendency) </remarks>
    Private Sub Hcl_VENDOR_INVOICE_VP_alert_REMINDER()
        Try
            Dim isTime As Boolean = False
            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 800 And cTime <= 810) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim objDC As New DataClass()
                Dim dtUsers As New DataTable()
                Dim arrWFStatus() = {"APPROVER 1", "APPROVER 2", "APPROVER 3", "APPROVER 4"}
                Dim arrInoviceType() = {"958564", "958565", "1078985"}
                Dim EventName As String = "Vendor Invoice VP"
                Dim bsla As Integer = 2
                Dim asla As Integer = 99999
                Dim eid As Integer = 46
                For i As Integer = 0 To arrWFStatus.Length - 1
                    Try
                        Dim ht As New Hashtable()
                        ht.Add("@En", EventName)
                        ht.Add("@Ws", arrWFStatus(i))
                        ht.Add("@Asla", asla)
                        ht.Add("@Bsla", bsla)
                        ht.Add("@Eid", eid)
                        ht.Add("@FTYPE", "")
                        dtUsers = objDC.ExecuteProDT("uspAlertMailSLA_getAllUsers_VIPHCL", ht)
                        For k As Integer = 0 To dtUsers.Rows.Count - 1
                            Dim MailTo As New ArrayList()
                            Dim MailSubject As String = ""
                            Dim MailCC As New ArrayList()
                            Dim MailBody As New StringBuilder()
                            Dim TempMailBody As New StringBuilder()
                            Dim ToUser As String = ""
                            Dim curUser As Integer
                            Try
                                If Not IsDBNull(dtUsers.Rows(k).Item("userid")) Then
                                    Dim dtDtl As New DataTable()
                                    curUser = dtUsers.Rows(k).Item("userid").ToString()
                                    ht.Clear()
                                    ht.Add("@En", EventName)
                                    ht.Add("@Ws", arrWFStatus(i))
                                    ht.Add("@Asla", asla)
                                    ht.Add("@Bsla", bsla)
                                    ht.Add("@Eid", eid)
                                    ht.Add("@curuser", curUser)
                                    dtDtl = objDC.ExecuteProDT("uspAlertMailSLA_getUserIDList", ht)
                                    Dim ColList As String = ""
                                    If dtDtl.Rows.Count <> 0 Then

                                        Dim uStr As String = "SELECT emailid  from MMM_MST_user where eid=46 and uid=" & curUser
                                        Dim dtU As New DataTable()
                                        dtU = objDC.ExecuteQryDT(uStr)
                                        ToUser = Convert.ToString(dtU.Rows(0).Item("emailid"))

                                        ColList = dtDtl.Rows(0).Item(0).ToString()
                                        If Convert.ToString(ColList) <> String.Empty Then

                                            ' Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code],fld180[Payment Terms],fld181[INCOTerms],fld32[Location],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],fld160[WBS Description],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],fld110[sep3forPO],fld107[Sep2forPO],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],fld106[sep1forPO],fld111[sep4forPO],fld95[sep5forPO],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld42,0) from mmm_mst_master where tid=isnull(d.fld42,0) and documenttype='Doc Nature')) [Doc Nature],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [Company Code],fld3[RAO Remarks],fld15[Company Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld136,0) from mmm_mst_master where tid=isnull(d.fld136,0) and documenttype='Profit Center')) [Profit Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld135,0) from mmm_mst_master where tid=isnull(d.fld135,0) and documenttype='Cost Center')) [Cost Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],fld58[Email Of User], (select fld1 from mmm_mst_master where tid = (select ISNULL(NULLIF(d.fld2, 0), 0)  from mmm_mst_master where tid=ISNULL(NULLIF(d.fld2, 0), 0) and documenttype='currency' ))[Currency] ,fld31[Dispatch Remarks],fld34[Physical Doc (Recd)],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld12[SSC Processing Date],fld182[Service Tax Categor],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld124[Low TDS Applicable],fld125[Low TDS Certificate],fld129[Remarks If any],fld130[Invoicing Milestone Description],fld108[Sep1forChklist],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[Clarification Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Remarks by HCL Receipt User],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld52[Processor Cancellation Remarks],fld53[Processor Reconsider Remarks],fld86[Warranty Certificate Attachment],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld75[Technical Compliance Certificate],fld4[Courier Docket No.],fld30[Dispatch Date],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='Rejection Reasons')) [Rejection Reason],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld51[Query Sent To],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld57[Parking Date],fld79[Proof of Electronics delivery],fld36[SSC Processing Remarks],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld13[Other Deduction],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld6,0) from mmm_mst_master where tid=isnull(d.fld6,0) and documenttype='Clarification User')) [User Email],fld67[Open Amount],fld7[Fiscal Year Posted],fld62[HC Rejection Remarks],fld63[Invoice Received Date],fld43[MIGO No],fld64[Dispatch Reconsider Remarks],fld65[QC Reconsider Remarks],( select username from mmm_mst_user where  uid=d.fld50) [Clarification by user],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld185[Monthly Fee],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld163[Supporting Attachment],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld206[PO Deleted],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld208[PO Date] from MMM_MST_DOC d  " & " where tid in (" & ColList & ") order by [Invoice Type] asc"
                                            Dim strQry As String = "SELECT DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], dms.GetUnAssignedAgent(tid) [GETUNASSIGNEDAGENT], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld208[PO Date],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code Description],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld180,0) from mmm_mst_master where tid=isnull(d.fld180,0) and documenttype='Payment Terms Master')) [Payment Term Description],fld181[INCOTerms],(select fld1 from mmm_mst_master where tid in (select fld3 from mmm_mst_master where documenttype='Plant Master' and eid= 46 and tid=d.fld128))[Invoice Submitters Location],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld63,0) from mmm_mst_master where tid=isnull(d.fld63,0) and documenttype='State Master')) [HCL State Billed by Vendor (Business Place)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld62,0) from mmm_mst_master where tid=isnull(d.fld62,0) and documenttype='State Master')) [State – Mat or Services delivered (Place of Supply)],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],fld217[L1 Approver (Dept)],fld218[L2 Approver (Dept)],fld219[L3 Approver (Dept)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],fld160[WBS Description],fld110[sep3forPO],fld214[L1 Approver (WBS)],fld210[WBS Description (Non-PO)],fld215[L2 Approver (WBS)],fld107[Sep2forPO],fld216[L3 Approver (WBS)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],fld42[Vendor GSTN Status],fld43[Vendor GSTIN],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld57,0) from mmm_mst_master where tid=isnull(d.fld57,0) and documenttype='State Master')) [State - from where Vendor Billed],fld58[Email Of User],fld31[Dispatch Remarks],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [HCL Entity Code],fld15[HCL Entity Name],fld34[Physical Doc (Recd)],( select fld5 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='GSTIN Master')) [HCL Entity GSTIN],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld182[Service Tax Categor],(select fld1 from mmm_mst_master where tid = isnull(d.fld2,0) and eid=46 and documenttype='currency') [Currency], (select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld130[Expense or Milestone Desc],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld50[CGST],fld51[SGST],fld52[IGST],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld129[Remarks If any],fld124[Low TDS Applicable],fld108[Sep1forChklist],fld125[Low TDS Certificate],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[VHD Reconsider Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Hcl Entity (As per Vendor Invoice)],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld86[Warranty Certificate Attachment],fld53[Processor Reconsider Remarks],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld30[Dispatch Date],fld4[Courier Docket No.],fld75[Technical Compliance Certificate],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld79[Proof of Electronics delivery],fld36[Is Additional Approval Reqd for Payment],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],fld7[Fiscal Year Posted],fld67[Open Amount],fld64[Submiter Rejection Remarks],fld65[QC Reconsider Remarks],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld66[GSTN Status of Retainer],fld185[Monthly Fee],fld95[GSTIN of Retainer],( select fld5 from mmm_mst_master where  tid=(select isnull(d.fld111,0) from mmm_mst_master where tid=isnull(d.fld111,0) and documenttype='GSTIN Master')) [HCL GSTIN],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld106,0) from mmm_mst_master where tid=isnull(d.fld106,0) and documenttype='State Master')) [State - from where Retainer Billed],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld163[Supporting Attachment],fld206[PO Deleted],fld209[GL Codes],fld213[Internal Order No],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld220[PUR GP Description] from MMM_MST_DOC d " & " where tid in (" & ColList & ") order by [Invoice Type] asc"
                                            Dim dtQuery As New DataTable()
                                            dtQuery = objDC.ExecuteQryDT(strQry)
                                            TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""> <font face=""arial, helvetica, sans-serif"" size=""2""> <b style=""text-align: center; line-height: 1.6em;"">Dear Sir/Madam, </b></p><br> The SLA for the following Invoices have <u>Expired</u>. Please log-In to HCL PEARL Portal to Approve/Reject these invoices immediately:<br><br></font>")
                                            ' TempMailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""> <font face=""arial, helvetica, sans-serif"" size=""2""> <u><b style=""text-align: center; line-height: 1.6em;"">CC Users:- </b></u><b>  Please note this mail is sent to you because you are immediate next approver of below Invoices.Actual dependency is with users  mention in <b>""TO""</b><br></font></p>")

                                            '''' ***** PO Based ****
                                            Dim viewPO As DataView = dtQuery.DefaultView
                                            viewPO.RowFilter = "[Invoice Type] = 'PO BASED'"
                                            If viewPO.ToTable().Rows.Count > 0 Then
                                                TempMailBody.Append("<font face=""arial, helvetica, sans-serif"" size=""3""> <p style=""background-color:#fff;color:#104E8B"" >Invoice Type : <b> PO Based </b></p></font>  ")
                                                TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                                                TempMailBody.Append("<tr style=""background-color:#87CEFA""><td><font face=""arial, helvetica, sans-serif"" size=""2""> BPM ID </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> PO No. </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Vendor Code </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Vendor Name </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> WBS No. </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Department </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Invoice No. </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Invoice Date </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Invoice Value (with Tax) </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Last Action by </font></td><td> <font face=""arial, helvetica, sans-serif"" size=""2""> Last Action Date </font></td>")
                                                TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">Next Approver</font></td></tr>")
                                            End If
                                            For z As Integer = 0 To viewPO.ToTable().Rows.Count - 1
                                                MailTo.Add(Convert.ToString(viewPO.ToTable().Rows(z)("CURRENT USER")))
                                                TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("BPM ID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("PO No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Vendor Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Vendor Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("WBS No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Department")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Invoice No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("Invoice Date")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">(" & Convert.ToString(viewPO.ToTable().Rows(z)("Currency")) & ") " & Convert.ToString(viewPO.ToTable().Rows(z)("Total Invoice Amount")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("LastActionDate")) & "</font></td>")
                                                If i = 0 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("L2 Approver")) & "</font></td></tr>")
                                                ElseIf i = 1 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("L3 Approver")) & "</font></td></tr>")
                                                ElseIf i = 2 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & "NA" & "</font></td></tr>")
                                                Else
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewPO.ToTable().Rows(z)("L5 Approver")) & "</font></td></tr>")
                                                End If
                                            Next
                                            If viewPO.ToTable().Rows.Count > 0 Then
                                                TempMailBody.Append("</table>")
                                            End If

                                            '''' ***** non po ****
                                            Dim viewNonPO As DataView = dtQuery.DefaultView
                                            viewNonPO.RowFilter = "[Invoice Type] = 'NON PO BASED'"
                                            If viewNonPO.ToTable().Rows.Count > 0 Then
                                                TempMailBody.Append("<font face=""arial, helvetica, sans-serif"" size=""3""> <p style=""background-color:#fff;color:#104E8B"" >Invoice Type : <b> NON PO Based</b></p></font>")
                                                TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                                                TempMailBody.Append("<tr style=""background-color:#87CEFA""><td><font face=""arial, helvetica, sans-serif"" size=""2""> BPM ID </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Vendor Code </font><td><font face=""arial, helvetica, sans-serif"" size=""2""> Vendor Name </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> WBS No. </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Department </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Invoice No. </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Invoice Date </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Invoice Value (with Tax) </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2""> Last Action by </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">  Last Action Date </font></td>")
                                                TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">Next Approver</font></td></tr>")
                                            End If
                                            For z As Integer = 0 To viewNonPO.ToTable().Rows.Count - 1
                                                MailTo.Add(Convert.ToString(viewNonPO.ToTable().Rows(z)("CURRENT USER")))
                                                TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("BPM ID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Vendor Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Vendor Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("WBS No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Department")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Invoice No")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Invoice Date")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">(" & Convert.ToString(viewNonPO.ToTable().Rows(z)("Currency")) & ") " & Convert.ToString(viewNonPO.ToTable().Rows(z)("Total Invoice Amount")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("LastActionDate")) & "</font></td>")
                                                If i = 0 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("L2 Approver")) & "</font></td></tr>")
                                                ElseIf i = 1 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("L3 Approver")) & "</font></td></tr>")
                                                ElseIf i = 2 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("L4 Approver")) & "</font></td></tr>")
                                                Else
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewNonPO.ToTable().Rows(z)("L5 Approver")) & "</font></td></tr>")
                                                End If
                                            Next
                                            If viewNonPO.ToTable().Rows.Count > 0 Then
                                                TempMailBody.Append("</table>")
                                            End If

                                            '''' ***** retainer Based ****
                                            Dim viewRetainer As DataView = dtQuery.DefaultView
                                            viewRetainer.RowFilter = "[Invoice Type] = 'RETAINERS'"
                                            If viewRetainer.ToTable().Rows.Count > 0 Then
                                                TempMailBody.Append("<font face=""arial, helvetica, sans-serif"" size=""3""> <p style=""background-color:#fff;color:#104E8B"">Invoice Type : <b> Retainers</b></p></font>")
                                                TempMailBody.Append("<table width=""100%"" border=""1px"" cellpadding=""3px"" cellspacing=""0px"" border=""1""> ")
                                                TempMailBody.Append("<tr style=""background-color:#87CEFA""><td><font face=""arial, helvetica, sans-serif"" size=""2"">BPM ID</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Consultant Emp. Code</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Consultant Vendor Code</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Consultant Name</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Contract Expiry</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">For Month</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Working Days</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Amount Claimed</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Mobile Number</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Company Code</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Cost Center</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Department</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Last Action by</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">Last Action Date</font></td>")
                                                TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">Next Approver</font></td></tr>")
                                            End If
                                            For z As Integer = 0 To viewRetainer.ToTable().Rows.Count - 1
                                                MailTo.Add(Convert.ToString(viewRetainer.ToTable().Rows(z)("CURRENT USER")))
                                                TempMailBody.Append("<tr><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("BPM ID")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Consultant Emp Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Consultant Vendor Code")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Consultant Name")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Contract Expiry")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("For Month")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Working Days")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">(INR)" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Amount Claimed")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Mobile Number")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Company Code Retainer")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Cost Center Retainer")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("Department")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("LastActionName")) & "</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("LastActionDate")) & "</font></td>")
                                                If i = 0 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("L2 Approver")) & "</font></td></tr>")
                                                ElseIf i = 1 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("L3 Approver")) & "</font></td></tr>")
                                                ElseIf i = 2 Then
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("L4 Approver")) & "</font></td></tr>")
                                                Else
                                                    TempMailBody.Append("<td><font face=""arial, helvetica, sans-serif"" size=""2"">" & Convert.ToString(viewRetainer.ToTable().Rows(z)("L5 Approver")) & "</font></td></tr>")
                                                End If
                                            Next
                                            If viewRetainer.ToTable().Rows.Count > 0 Then
                                                TempMailBody.Append("</table>")
                                            End If


                                            MailCC.Add(objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' +  emailid  from mmm_mst_user where uid in ( select distinct fld120[L2] from mmm_mst_doc where  tid in (" & ColList & "))                  FOR XML PATH('')), 1, 1, '') AS [Output]"))
                                            If i = 0 Then
                                                MailSubject = "Reminder-Invoice Approvals Pending (Level-1)"
                                            ElseIf i = 1 Then
                                                MailSubject = "Reminder-Invoice Approvals Pending (Level-2)"
                                            ElseIf i = 2 Then
                                                MailSubject = "Reminder-Invoice Approvals Pending (Level-3)"
                                            Else
                                                MailSubject = "Reminder-Invoice Approvals Pending (Level-4)"
                                            End If
                                        Else
                                            objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP',' No list found and workflow status is " & arrWFStatus(i).ToString() & " and current userid is:-" & curUser & "  ',getdate()," & eid & ")")
                                        End If
                                    End If
                                End If
                                MailBody.Append(TempMailBody.ToString())
                                Dim finalBody As String = MailBody.ToString()
                                If finalBody.Length > 10 Then
                                    MailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""><br></p><p style=""margin: 0in 0in 0.0001pt;""><br></p><font face=""arial, helvetica, sans-serif"" size=""2"">You can access <b> 'PEARL Portal' </b> through HCL Intranet Homepage.</font><p style=""margin: 0in 0in 0.0001pt;""></p> <font  face=""arial, helvetica, sans-serif"" size=""2""> <p><b>Regards</b></p><p>Vendor Help Desk<br/>Procurement Department<br/>HCL Infosystems Limited<br/> E - 4,5,6 Sector - XI,<br/>Noida - 201301 U.P. (India)<br/>Email : vendorhelpdesk@hcl.com</b></p><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""><div><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""></b></div>This is a System generated mail. Please do not reply to this mail</b><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;"">!</b><br><br></font>")
                                    finalBody = MailBody.ToString()
                                    ' Dim TOs As String = String.Join(",", MailTo.ToArray().Distinct())
                                    Dim htTran As New Hashtable()
                                    htTran.Add("@MAILTO", ToUser)
                                    htTran.Add("@CC", "")   ' String.Join(",", MailCC.ToArray())
                                    htTran.Add("@MSG", finalBody.ToString())
                                    htTran.Add("@ALERTTYPE", "VP_ALERT_MAIL")
                                    htTran.Add("@MAILEVENT", EventName & "-" & arrWFStatus(i).ToString())
                                    htTran.Add("@EID", eid)
                                    objDC.ExecuteProDT("INSERT_MAILLOG", htTran)
                                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                    ' sendMail1(ToUser, "", "", MailSubject, finalBody)
                                    'sendMail1(TOs, "", "mayank.garg@myndsol.com,sunil.pareek@myndsol.com", MailSubject, finalBody)
                                    Dim obj As New MailUtill(eid:=eid)
                                    obj.SendMail(ToMail:=ToUser, Subject:=MailSubject, MailBody:=finalBody, CC:="", BCC:="")
                                End If
                            Catch ex As Exception
                                objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " and current userid is:-" & curUser & "  ',getdate()," & eid & ")")
                            End Try
                        Next
                    Catch ex As Exception
                        objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " ',getdate()," & eid & ")")
                    End Try
                Next
            End If
        Catch ex As Exception
            ' objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','Try Catch Error','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " ',getdate(),46)")
            Dim finalBody As String = "Error in HCL Reminder mail sending <br/> " & "Error message is - " & Convert.ToString(ex.Message) & "<br/> Inner message is - " & Convert.ToString(ex.InnerException.Message)
            ' sendMail1("sunil.pareek@myndsol.com", "", "", "Error in HCL Reminder mail sending", finalBody)
        End Try
    End Sub




    Public Function Run_Helpdesk_MailRead_Sch() As String Implements IDMSService.Run_Helpdesk_MailRead_Sch
        Try
            Dim TS As New TicketScheduler()
            TS.CreateDocumentEntityWise()
            Run_Helpdesk_MailRead_Sch = "Successful"
        Catch ex As Exception
            AutoRunLog("Run_Helpdesk_MailRead_Sch", "CreateDocumentEntityWise", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try
    End Function


    '' disabled by sunil for not in use on mail protocol upgrade
    'Protected Sub Run_HCL_AR_PRA_SLAexpiry_alerts()
    '    Try
    '        '     Call Hcl_PRA_NON_EFT_alert_superVisor()  ' daily alerts two times 5 pm and 7 pm 
    '    Catch ex As Exception
    '        AutoRunLog("Hcl_PRA_NON_EFT_alert_superVisor", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
    '    End Try
    '    Try
    '        '     Call Hcl_PRA_EFT_alert_superVisor() ' daily alerts two times 5 pm and 7 pm 
    '    Catch ex As Exception
    '        AutoRunLog("Hcl_PRA_EFT_alert_superVisor", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
    '    End Try
    '    Try
    '        '     Call Hcl_PRA_NON_EFT_alert_SalesTeam() ' daily alerts two times 5 pm and 7 pm 
    '    Catch ex As Exception
    '        AutoRunLog("Hcl_PRA_NON_EFT_alert_SalesTeam", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
    '    End Try
    '    Try
    '        '    Call Hcl_PRA_EFT_alert_SalesTeam() ' daily alerts two times 5 pm and 7 pm 
    '    Catch ex As Exception
    '        AutoRunLog("Hcl_PRA_EFT_alert_SalesTeam", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
    '    End Try
    '    Try
    '        '    Call Hcl_PRA_NON_EFT_alert_RAO() ' daily alerts two times 5 pm and 7 pm 
    '    Catch ex As Exception
    '        AutoRunLog("Hcl_PRA_NON_EFT_alert_RAO", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
    '    End Try
    'End Sub

    '' disabled by sunil for not in use on mail protocol upgrade
    'Private Sub Hcl_PRA_NON_EFT_alert_SalesTeam()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    If con.State = ConnectionState.Closed Then
    '        con.Open()
    '    End If
    '    Try
    '        Dim isTime As Boolean = False

    '        Dim cTime As Integer
    '        cTime = (Now.Hour * 100) + Now.Minute
    '        If (cTime >= 1701 And cTime <= 1710) Or (cTime >= 1901 And cTime <= 1910) Then
    '            isTime = True
    '        Else
    '            isTime = False
    '        End If
    '        If isTime Then
    '            Dim Qrystr As String
    '            Qrystr = "Select distinct d.OUID [OUID] from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created' and dt.aprstatus is null"

    '            da.SelectCommand.CommandText = Qrystr
    '            Dim DtM As New DataTable
    '            da.Fill(DtM)
    '            If DtM.Rows.Count <> 0 Then
    '                Dim CurrUser As Integer
    '                For i As Integer = 0 To DtM.Rows.Count - 1
    '                    CurrUser = DtM.Rows(i).Item("OUID")
    '                    Qrystr = "Select  'PRA Non EFT' [PRA Type], u.username,u.emailid, d.fld1 [BPM ID], d.fld16 [PRA Date] ,d.fld12 [Customer Name], d.fld45 [Payment Type], d.fld57 [Total Payment Amount], d.fld3 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created'  and dt.aprstatus is null and d.OUID=" & CurrUser
    '                    Dim EmailTo As String = ""
    '                    Dim MailSub As String = "PRA N-EFT Pending for Approval at Sales Manager"

    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtR As New DataTable
    '                    da.Fill(dtR)
    '                    Dim MailBody As String = ""


    '                    Qrystr = "Select emailid from mmm_mst_user where eid=46 and uid=" & CurrUser
    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtU As New DataTable
    '                    da.Fill(dtU)
    '                    EmailTo = dtU.Rows(0).Item("emailid").ToString

    '                    ''''
    '                    MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
    '                    MailBody &= "<p>This is to informed to you that following PRAs are pending for approval of Sales Manager</p> <br>"

    '                    MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
    '                    MailBody &= "<TD>PRA Type</TD>"
    '                    MailBody &= "<TD>BPM ID</TD>"
    '                    MailBody &= "<TD>PRA Date</TD>"
    '                    MailBody &= "<TD>Customer name</TD>"
    '                    MailBody &= "<TD>Payment type</TD>"
    '                    MailBody &= "<TD>Total Payment Amount</TD>"
    '                    MailBody &= "<TD>Invoice Total</TD>"
    '                    MailBody &= "<TD>Received Date</TD>"
    '                    MailBody &= "<TD>Pending Hours</TD>"
    '                    MailBody &= "</Tr>"

    '                    For k As Integer = 0 To dtR.Rows.Count - 1
    '                        EmailTo = dtR.Rows(0).Item("emailid").ToString()
    '                        MailBody &= "<Tr>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("PRA Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
    '                        MailBody &= "</Tr>"
    '                    Next


    '                    MailBody &= "</Table>"
    '                    MailBody &= "<p> Click  <a href=""https://hcl.myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"


    '                    MailBody &= "<p style=""color: Maroon"">Regards</p>"
    '                    MailBody &= "<p style=""color: Maroon"">HCL AR Team</p>"

    '                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
    '                    sendMail1(EmailTo, "", "tanweer.alam@myndsol.com", MailSub, MailBody)

    '                    Dim obj As New MailUtill(eid:=eid)
    '                    obj.SendMail(ToMail:=EmailTo, Subject:=MailSub, MailBody:=MailBody, CC:="", BCC:="")

    '                    ''''
    '                Next
    '            End If
    '            DtM.Dispose()
    '        End If
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    Catch ex As Exception

    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    End Try

    'End Sub

    '' disabled by sunil for not in use on mail protocol upgrade
    'Private Sub Hcl_PRA_EFT_alert_SalesTeam()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    If con.State = ConnectionState.Closed Then
    '        con.Open()
    '    End If
    '    Try
    '        Dim isTime As Boolean = False

    '        Dim cTime As Integer
    '        cTime = (Now.Hour * 100) + Now.Minute
    '        If (cTime >= 1701 And cTime <= 1710) Or (cTime >= 1901 And cTime <= 1910) Then
    '            isTime = True
    '        Else
    '            isTime = False
    '        End If
    '        If isTime Then
    '            Dim Qrystr As String
    '            Qrystr = "Select distinct d.OUID [OUID] from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA EFT' and d.curstatus='Created' and dt.aprstatus is null"

    '            da.SelectCommand.CommandText = Qrystr
    '            Dim DtM As New DataTable
    '            da.Fill(DtM)
    '            If DtM.Rows.Count <> 0 Then
    '                Dim CurrUser As Integer
    '                For i As Integer = 0 To DtM.Rows.Count - 1
    '                    CurrUser = DtM.Rows(i).Item("OUID")
    '                    Qrystr = "Select  'PRA EFT' [PRA Type], u.username,u.emailid, d.fld45 [BPM ID], d.fld16 [PRA Date] ,d.fld15 [Customer Name], d.fld21 [Payment Type], d.fld5 [Total Payment Amount], d.fld34 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA Eft' and d.curstatus='Created'  and dt.aprstatus is null and d.OUID=" & CurrUser
    '                    Dim EmailTo As String = ""
    '                    Dim MailSub As String = "PRA EFT Pending for Approval at Sales Manager"

    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtR As New DataTable
    '                    da.Fill(dtR)
    '                    Dim MailBody As String = ""


    '                    Qrystr = "Select emailid from mmm_mst_user where eid=46 and uid=" & CurrUser
    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtU As New DataTable
    '                    da.Fill(dtU)
    '                    EmailTo = dtU.Rows(0).Item("emailid").ToString


    '                    MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
    '                    MailBody &= "<p>This is to informed to you that following PRAs are pending for approval of Sales Manager</p> <br>"

    '                    MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
    '                    MailBody &= "<TD>PRA Type</TD>"
    '                    MailBody &= "<TD>BPM ID</TD>"
    '                    MailBody &= "<TD>PRA Date</TD>"
    '                    MailBody &= "<TD>Customer name</TD>"
    '                    MailBody &= "<TD>Payment type</TD>"
    '                    MailBody &= "<TD>Total Payment Amount</TD>"
    '                    MailBody &= "<TD>Invoice Total</TD>"
    '                    MailBody &= "<TD>Received Date</TD>"
    '                    MailBody &= "<TD>Pending Hours</TD>"
    '                    MailBody &= "</Tr>"

    '                    For k As Integer = 0 To dtR.Rows.Count - 1
    '                        EmailTo = dtR.Rows(0).Item("emailid").ToString()
    '                        MailBody &= "<Tr>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("PRA Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
    '                        MailBody &= "</Tr>"
    '                    Next


    '                    MailBody &= "</Table>"
    '                    MailBody &= "<p> Click  <a href=""https://hcl.myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"


    '                    MailBody &= "<p style=""color: Maroon"">Regards</p>"
    '                    MailBody &= "<p style=""color: Maroon"">HCL AR Team</p>"

    '                    sendMail1(EmailTo, "", "tanweer.alam@myndsol.com", MailSub, MailBody)
    '                Next
    '            End If
    '            DtM.Dispose()
    '        End If
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    Catch ex As Exception

    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    End Try

    'End Sub

    '' below is for sending mail to user for duplicate file found by file transfer utility run
    Private Sub FileTransferDyn_send_duplicity_inti_mail_toUser(ByVal DupFileName As String, ByVal Uid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Try
            Dim Qrystr As String
            Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created' and dt.aprstatus is null"

            da.SelectCommand.CommandText = Qrystr
            Dim DtM As New DataTable
            da.Fill(DtM)
            If DtM.Rows.Count <> 0 Then
                Dim CurrUser As Integer
                For i As Integer = 0 To DtM.Rows.Count - 1
                    CurrUser = DtM.Rows(i).Item("userID")
                    Qrystr = "Select  username , emailid from mmm_mst_user where uid=" & Uid
                    Dim EmailTo As String = ""
                    Dim UserName As String = ""
                    Dim MailSub As String = "Duplicate File Found During File Transfer!"

                    da.SelectCommand.CommandText = Qrystr
                    Dim dtR As New DataTable
                    da.Fill(dtR)
                    Dim MailBody As String = ""

                    EmailTo = dtR.Rows(0).Item("emailid").ToString()
                    UserName = dtR.Rows(0).Item("username").ToString()

                    MailBody &= "<p style=""color: Maroon""> Dear " & UserName & " </p>"
                    MailBody &= "<p>While transfering Files by MyndSaas Utility, below Duplicate file was found! </br> This File is ignored by utility. </p> <br>"

                    MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
                    MailBody &= "<TD>File Name</TD>"
                    MailBody &= "<TD>" & DupFileName & "</TD>"
                    MailBody &= "</Tr>"

                    MailBody &= "</Table>"

                    MailBody &= "<p style=""color: Maroon"">Regards</p>"
                    MailBody &= "<p style=""color: Maroon"">MyndSasA File Transfer Utility</p>"
                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                    'sendMail1(EmailTo, "", "", MailSub, MailBody)
                    Dim obj As New MailUtill(eid:=46)
                    obj.SendMail(ToMail:=EmailTo, Subject:=MailSub, MailBody:=MailBody, CC:="", BCC:="")
                Next
            End If

            con.Close()
            con.Dispose()
            da.Dispose()
        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub



    'Private Sub Hcl_PRA_NON_EFT_alert_superVisor()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    If con.State = ConnectionState.Closed Then
    '        con.Open()
    '    End If
    '    Try
    '        Dim isTime As Boolean = False

    '        Dim cTime As Integer
    '        cTime = (Now.Hour * 100) + Now.Minute
    '        If (cTime >= 1701 And cTime <= 1710) Or (cTime >= 1901 And cTime <= 1910) Then
    '            isTime = True
    '        Else
    '            isTime = False
    '        End If
    '        If isTime Then
    '            Dim Qrystr As String
    '            Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created' and dt.aprstatus is null"

    '            da.SelectCommand.CommandText = Qrystr
    '            Dim DtM As New DataTable
    '            da.Fill(DtM)
    '            If DtM.Rows.Count <> 0 Then
    '                Dim CurrUser As Integer
    '                For i As Integer = 0 To DtM.Rows.Count - 1
    '                    CurrUser = DtM.Rows(i).Item("userID")
    '                    Qrystr = "Select  'PRA N-EFT' [PRA Type], u.username,u.emailid, d.fld1 [BPM ID], d.fld16 [PRA Date] ,d.fld12 [Customer Name], d.fld45 [Payment Type], d.fld57 [Total Payment Amount], d.fld3 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created'  and dt.aprstatus is null and dt.userid=" & CurrUser
    '                    Dim EmailTo As String = ""
    '                    Dim MailSub As String = "PRA Non Eft Pending for Approval"

    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtR As New DataTable
    '                    da.Fill(dtR)
    '                    Dim MailBody As String = ""

    '                    MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
    '                    MailBody &= "<p>Following PRAs are pending for your approval, please take suitable action at your end</p> <br>"

    '                    MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
    '                    MailBody &= "<TD>PRA Type</TD>"
    '                    MailBody &= "<TD>BPM ID</TD>"
    '                    MailBody &= "<TD>PRA Date</TD>"
    '                    MailBody &= "<TD>Customer name</TD>"
    '                    MailBody &= "<TD>Payment type</TD>"
    '                    MailBody &= "<TD>Total Payment Amount</TD>"
    '                    MailBody &= "<TD>Invoice Total</TD>"
    '                    MailBody &= "<TD>Received Date</TD>"
    '                    MailBody &= "<TD>Pending Hours</TD>"
    '                    MailBody &= "</Tr>"

    '                    For k As Integer = 0 To dtR.Rows.Count - 1
    '                        EmailTo = dtR.Rows(0).Item("emailid").ToString()
    '                        MailBody &= "<Tr>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("PRA Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
    '                        MailBody &= "</Tr>"
    '                    Next


    '                    MailBody &= "</Table>"
    '                    MailBody &= "<p> Click  <a href=""https://hcl.myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"


    '                    MailBody &= "<p style=""color: Maroon"">Regards</p>"
    '                    MailBody &= "<p style=""color: Maroon"">HCL AR Team</p>"

    '                    sendMail1(EmailTo, "", "tanweer.alam@myndsol.com", MailSub, MailBody)
    '                Next
    '            End If
    '        End If
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    Catch ex As Exception

    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    End Try

    'End Sub

    'Private Sub Hcl_PRA_EFT_alert_superVisor()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    If con.State = ConnectionState.Closed Then
    '        con.Open()
    '    End If
    '    Try
    '        Dim isTime As Boolean = False

    '        Dim cTime As Integer
    '        cTime = (Now.Hour * 100) + Now.Minute
    '        If (cTime >= 1701 And cTime <= 1710) Or (cTime >= 1901 And cTime <= 1910) Then
    '            isTime = True
    '        Else
    '            isTime = False
    '        End If
    '        If isTime Then
    '            Dim Qrystr As String
    '            Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA EFT' and d.curstatus='Created' and dt.aprstatus is null"

    '            da.SelectCommand.CommandText = Qrystr
    '            Dim DtM As New DataTable
    '            da.Fill(DtM)
    '            If DtM.Rows.Count <> 0 Then
    '                Dim CurrUser As Integer
    '                For i As Integer = 0 To DtM.Rows.Count - 1
    '                    CurrUser = DtM.Rows(i).Item("userID")
    '                    Qrystr = "Select  'PRA EFT' [PRA Type], u.username,u.emailid, d.fld45 [BPM ID], d.fld16 [PRA Date] ,d.fld15 [Customer Name], d.fld21 [Payment Type], d.fld5 [Total Payment Amount], d.fld34 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA Eft' and d.curstatus='Created'  and dt.aprstatus is null and dt.userid=" & CurrUser
    '                    Dim EmailTo As String = ""
    '                    Dim MailSub As String = "PRA EFT Pending for Approval"

    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtR As New DataTable
    '                    da.Fill(dtR)
    '                    Dim MailBody As String = ""


    '                    MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
    '                    MailBody &= "<p>Following PRAs are pending for your approval, please take suitable action at your end</p> <br>"

    '                    MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
    '                    MailBody &= "<TD>PRA Type</TD>"
    '                    MailBody &= "<TD>BPM ID</TD>"
    '                    MailBody &= "<TD>PRA Date</TD>"
    '                    MailBody &= "<TD>Customer name</TD>"
    '                    MailBody &= "<TD>Payment type</TD>"
    '                    MailBody &= "<TD>Total Payment Amount</TD>"
    '                    MailBody &= "<TD>Invoice Total</TD>"
    '                    MailBody &= "<TD>Received Date</TD>"
    '                    MailBody &= "<TD>Pending Hours</TD>"
    '                    MailBody &= "</Tr>"

    '                    For k As Integer = 0 To dtR.Rows.Count - 1
    '                        EmailTo = dtR.Rows(0).Item("emailid").ToString()
    '                        MailBody &= "<Tr>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("PRA Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
    '                        MailBody &= "</Tr>"
    '                    Next

    '                    MailBody &= "</Table>"
    '                    MailBody &= "<p> Click  <a href=""https://hcl.myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"

    '                    MailBody &= "<p style=""color: Maroon"">Regards</p>"
    '                    MailBody &= "<p style=""color: Maroon"">HCL AR Team</p>"

    '                    sendMail1(EmailTo, "", "tanweer.alam@myndsol.com", MailSub, MailBody)
    '                Next
    '            End If
    '        End If
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    Catch ex As Exception

    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    End Try

    'End Sub

    'Private Sub Hcl_PRA_NON_EFT_alert_RAO()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    If con.State = ConnectionState.Closed Then
    '        con.Open()
    '    End If
    '    Try
    '        Dim isTime As Boolean = False

    '        Dim cTime As Integer
    '        cTime = (Now.Hour * 100) + Now.Minute
    '        If (cTime >= 1701 And cTime <= 1710) Or (cTime >= 1901 And cTime <= 1910) Then
    '            isTime = True
    '        Else
    '            isTime = False
    '        End If
    '        If isTime Then
    '            Dim Qrystr As String
    '            Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Approved' and dt.aprstatus is null"

    '            da.SelectCommand.CommandText = Qrystr
    '            Dim DtM As New DataTable
    '            da.Fill(DtM)
    '            If DtM.Rows.Count <> 0 Then
    '                Dim CurrUser As Integer
    '                For i As Integer = 0 To DtM.Rows.Count - 1
    '                    CurrUser = DtM.Rows(i).Item("userID")
    '                    Qrystr = "Select  'PRA N-EFT' [PRA Type], u.username,u.emailid, d.fld1 [BPM ID], d.fld16 [PRA Date] ,d.fld12 [Customer Name], d.fld45 [Payment Type], d.fld57 [Total Payment Amount], d.fld3 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA' and d.curstatus='Approved'  and dt.aprstatus is null and dt.userid=" & CurrUser
    '                    Dim EmailTo As String = ""
    '                    Dim MailSub As String = "PRA Non Eft Pending for Approval"

    '                    da.SelectCommand.CommandText = Qrystr
    '                    Dim dtR As New DataTable
    '                    da.Fill(dtR)
    '                    Dim MailBody As String = ""

    '                    MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
    '                    MailBody &= "<p>Following PRAs are pending for your approval, please take suitable action at your end</p> <br>"

    '                    MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
    '                    MailBody &= "<TD>PRA Type</TD>"
    '                    MailBody &= "<TD>BPM ID</TD>"
    '                    MailBody &= "<TD>PRA Date</TD>"
    '                    MailBody &= "<TD>Customer name</TD>"
    '                    MailBody &= "<TD>Payment type</TD>"
    '                    MailBody &= "<TD>Total Payment Amount</TD>"
    '                    MailBody &= "<TD>Invoice Total</TD>"
    '                    MailBody &= "<TD>Received Date</TD>"
    '                    MailBody &= "<TD>Pending Hours</TD>"
    '                    MailBody &= "</Tr>"

    '                    For k As Integer = 0 To dtR.Rows.Count - 1
    '                        EmailTo = dtR.Rows(0).Item("emailid").ToString()
    '                        MailBody &= "<Tr>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("PRA Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
    '                        MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
    '                        MailBody &= "</Tr>"
    '                    Next

    '                    MailBody &= "</Table>"
    '                    MailBody &= "<p> Click  <a href=""https://hcl.myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"

    '                    MailBody &= "<p style=""color: Maroon"">Regards</p>"
    '                    MailBody &= "<p style=""color: Maroon"">HCL AR Team</p>"

    '                    sendMail1(EmailTo, "", "tanweer.alam@myndsol.com", MailSub, MailBody)

    '                Next
    '            End If
    '        End If
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    Catch ex As Exception

    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        da.Dispose()
    '    End Try

    'End Sub

    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
            End If
            'vc79aK123AJ&$kL0
            If bcc <> "" Then
                Email.Bcc.Add(bcc)
            End If
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub
            End Try
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub


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
    Public Sub Run_FTP_Inwared_Integration_Tally()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select * from mmm_tallyint_ftpsch where Generatetype='AUTO' order by tid", con)
        Dim dtM As New DataTable()
        da.Fill(dtM)
        Dim fileReadpath As String = System.Web.Hosting.HostingEnvironment.MapPath("~\ES_Import\")
        For i As Integer = 0 To dtM.Rows.Count - 1 Step 1
            Dim Tid As Integer = dtM.Rows(i).Item("tid")
            Dim cid As String = dtM.Rows(i).Item("EID")
            Dim FileNm As String = dtM.Rows(i).Item("filename").ToString
            Dim FilePath As String = dtM.Rows(i).Item("fileaddress").ToString
            Dim LoginID As String = dtM.Rows(i).Item("loginid").ToString
            Dim pwd As String = dtM.Rows(i).Item("pwd").ToString
            If Scheduler(Tid) Then
                Dim RES As String = ""
                Try
                    RES = CopyFilefromFtp(FileNm, FilePath, LoginID, pwd, "DELETE")
                Catch ex As Exception
                    'UpdateErrorLog(FileNm, "txt", "Error in CopyFilefromFtp", Tid, cid)
                End Try
                Dim dtCl As New DataTable
                Dim TotRecCount As Integer = 0
                Dim TotRecCountRun As Integer = 0
                If RES = "SUCCESSFUL" Then
                    If System.IO.File.Exists(fileReadpath & FileNm) Then
                        GetDataFromXml(FileNm, FilePath, LoginID, pwd)
                        Try
                            'dtInput = GetDataFromExcel(FileNm, fileReadpath)
                        Catch ex As Exception
                            'UpdateErrorLog(FileNm, "Tcatch", "Error in GetDataFromExcel", Tid, cid)
                        End Try
                        Dim currentTime As DateTime = System.DateTime.Now
                        Dim lastrunDMY As String = currentTime.Day.ToString.PadLeft(2, "0") & "-" & currentTime.Month.ToString.PadLeft(2, "0") & "-" & currentTime.Year.ToString & "-" & currentTime.Hour.ToString.PadLeft(2, "0") & "-" & currentTime.Minute.ToString.PadLeft(2, "0")
                        'UpdateErrorLog(FileNm, "txt", "Enter in GenerateOutputfromExcelInputHCL", Tid, cid)
                        Try
                            'Call GenerateOutputfromExcelInputHCL(dtInput, Tid, FileNm, cid, lastrunDMY, websrvicecmpny)
                        Catch ex As Exception
                            'UpdateErrorLog(FileNm, "Tcatch", "Error in GenerateOutputfromExcelInputHCL", Tid, cid)
                        End Try
                        da.SelectCommand.CommandText = "Update mmm_tallyint_ftpsch set lastrun=getdate() where tid=" & Tid
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                        Try
                            'SendFTPIntegrationLogMail(cid, FileNm, lastrunDMY)
                        Catch ex As Exception
                            'UpdateErrorLog(FileNm, "Tcatch", "Error in SendFTPIntegrationLogMail", Tid, cid)
                        End Try

                        'UpdateErrorLog(FileNm, "txt", "complete sending mail", Tid, cid)
                        Try
                            ' CopyfiletoArchive(FilePath, LoginID, pwd, fileReadpath, FileNm, lastrunDMY, valuename)
                            'CopyfiletoArchive(FilePath, LoginID, pwd, fileReadpath, FileNm, lastrunDMY, valuename)
                        Catch ex As Exception
                            'UpdateErrorLog(FileNm, "Tcatch", "Error in CopyfiletoArchive_Rename", Tid, cid)
                        End Try
                    Else
                        'SendExceptionErrorMails(Tid, cid, "NOT-RUN", FileNm, "")
                    End If
                Else
                    'SendExceptionErrorMails(Tid, cid, "NOT-RUN", FileNm, "")
                End If
            End If
        Next

        con.Close()
        con.Dispose()
        con = Nothing
        dtM.Dispose()
        dtM = Nothing
        da.Dispose()
        da = Nothing
    End Sub

    Public Function MoveFile(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer, ByVal BarCodeFldmapping As String, ByVal barcodefldval As String) As String Implements IDMSService.MoveFile
        '& EID & "&gid=" & Gid & "&STID=0&docurl=" & foundfile.Name & "&oUID=" & uid & "&filesize=" & sz & "" & "&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
        Dim DocImage As String = ""
        Dim ext As String = Right(docurl, 4)
        If Left(ext, 1) = "." Then
            ext = Right(ext, 3)
        End If

        '' user below line to add datetimestamp to file name by sp on 17_may_14
        '' temp disabled by sunil 20_may_14
        'Dim dtstamp As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond
        'docurl = docurl.Replace("." & ext, dtstamp & "." & ext)

        Dim AcFileName As String = Replace(docurl, "." & ext, "")
        Select Case ext.ToUpper()
            Case "DOC", "DOCX"
                DocImage = "word.png"
            Case "JPG"
                DocImage = "jpeg.png"
            Case "PDF"
                DocImage = "adobe.png"
            Case "XLS", "XLSX"
                DocImage = "excel.png"
            Case Else
                DocImage = "nofileimage.png"
        End Select

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)


        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        '' for duplicity check 
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.CommandText = "uspCheckDuplicateDoc_Tecum"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.AddWithValue("eid", EID)
        da.SelectCommand.Parameters.AddWithValue("Barcode", barcodefldval)
        da.SelectCommand.Parameters.AddWithValue("documenttype", Doctype)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim Result As String = da.SelectCommand.ExecuteScalar() ' will return one of results - NOT EXIST, REJECTED, NOT REJECTED

        If Result = "NOT EXIST" Then
            da.SelectCommand.CommandText = "uspAddFileService_MyndSaas_Tecum"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()

            da.SelectCommand.Parameters.AddWithValue("eid", EID)
            da.SelectCommand.Parameters.AddWithValue("gid", gid)
            da.SelectCommand.Parameters.AddWithValue("fname", AcFileName)
            If stid <> 0 Then
                If stid > 9999 Then
                    da.SelectCommand.Parameters.AddWithValue("stid", stid)
                End If
            End If
            da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
            da.SelectCommand.Parameters.AddWithValue("docimage", DocImage)
            da.SelectCommand.Parameters.AddWithValue("oUID", oUID)
            da.SelectCommand.Parameters.AddWithValue("filesize", filesize)
            '' new added for hcl 
            ''&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
            da.SelectCommand.Parameters.AddWithValue("doctype", Doctype)
            da.SelectCommand.Parameters.AddWithValue("fup_FM", fup_fieldmapping)
            da.SelectCommand.Parameters.AddWithValue("fup_FileName", EID & "/" & docurl)
            da.SelectCommand.Parameters.AddWithValue("LOC_FM", loc_fieldmapping)
            da.SelectCommand.Parameters.AddWithValue("LOCID", location_id)
            da.SelectCommand.Parameters.AddWithValue("BARCODE_FM", BarCodeFldmapping)
            da.SelectCommand.Parameters.AddWithValue("BARCODE_VAL", barcodefldval)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim fileID As Integer = da.SelectCommand.ExecuteScalar()

            '' code for adding autonumber field in document created by scheduler starts  - 23_apr_14
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & EID & " and FormName = '" & Doctype & "' order by displayOrder"
            Dim ds As New DataSet
            da.Fill(ds, "fields")
            Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
            If row.Length > 0 Then
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
                da.SelectCommand.Parameters.AddWithValue("docid", fileID)
                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                Dim an As String = da.SelectCommand.ExecuteScalar()
                ' msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
                da.SelectCommand.Parameters.Clear()
            End If
            '' code for adding autonumber field in document created by scheduler ends here - 23_apr_14
            Dim ob As New DMSUtil()
            ob.CheckWorkFlow(fileID, EID)

        ElseIf Result = "REJECTED" Then
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "select * from mmm_mst_doc where eid=" & EID & " and docurl='" & docurl & "'"
            da.SelectCommand.CommandType = CommandType.Text
            Dim dtR As New DataTable
            da.Fill(dtR)
            Dim SeleDocID As Integer = 0
            If dtR.Rows.Count <> 0 Then
                SeleDocID = dtR.Rows(0).Item("tid").ToString
            End If

            da.SelectCommand.CommandText = "ReCreateDoc_DefaultMovement"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("tid", SeleDocID)
            da.SelectCommand.Parameters.AddWithValue("CUID", dtR.Rows(0).Item("oUID"))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            ' Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
            Dim dm As New DMSUtil()
            Call dm.GetNextUserFromRolematrix(SeleDocID, EID, dtR.Rows(0).Item("oUID"), "", dtR.Rows(0).Item("oUID"))
        ElseIf Result = "NOT REJECTED" Then
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "uspAddtoDuplicateFileLog"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("EID", EID)
            da.SelectCommand.Parameters.AddWithValue("docurl", barcodefldval & "." & ext)
            da.SelectCommand.Parameters.AddWithValue("ouid", oUID)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
        End If

        con.Close()
        da.Dispose()
        con.Dispose()

        If Result = "NOT EXIST" Then
            Return "ADDED SUCCESSFULLY"
        ElseIf Result = "REJECTED" Then
            Return "FILE REFRESHED"
        ElseIf Result = "NOT REJECTED" Then
            Return "DUPLICATE FILE FOUND"
        Else
            Return "SOME UNKNOWN ERROR"
        End If
    End Function


    '' new movefileDfaft for saving rec. in draft table by sunil on 27-Nov-18
    Public Function MoveFileDraft(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer, ByVal BarCodeFldmapping As String, ByVal barcodefldval As String) As String Implements IDMSService.MoveFileDraft
        '& EID & "&gid=" & Gid & "&STID=0&docurl=" & foundfile.Name & "&oUID=" & uid & "&filesize=" & sz & "" & "&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
        Dim DocImage As String = ""
        Dim ext As String = Right(docurl, 4)
        If Left(ext, 1) = "." Then
            ext = Right(ext, 3)
        End If

        '' user below line to add datetimestamp to file name by sp on 17_may_14
        '' temp disabled by sunil 20_may_14
        'Dim dtstamp As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond
        'docurl = docurl.Replace("." & ext, dtstamp & "." & ext)

        Dim AcFileName As String = Replace(docurl, "." & ext, "")
        Select Case ext.ToUpper()
            Case "DOC", "DOCX"
                DocImage = "word.png"
            Case "JPG"
                DocImage = "jpeg.png"
            Case "PDF"
                DocImage = "adobe.png"
            Case "XLS", "XLSX"
                DocImage = "excel.png"
            Case Else
                DocImage = "nofileimage.png"
        End Select

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)


        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        '' for duplicity check 
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.CommandText = "uspCheckDuplicateDoc_FileTrans_To_Draft"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.AddWithValue("eid", EID)
        da.SelectCommand.Parameters.AddWithValue("Barcode", barcodefldval)
        da.SelectCommand.Parameters.AddWithValue("documenttype", Doctype)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim Result As String = da.SelectCommand.ExecuteScalar() ' will return one of results - NOT EXIST, REJECTED, NOT REJECTED

        If Result = "NOT EXIST" Then
            da.SelectCommand.CommandText = "uspAddFileService_MyndSaas_To_Draft"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()

            da.SelectCommand.Parameters.AddWithValue("eid", EID)
            da.SelectCommand.Parameters.AddWithValue("gid", gid)
            da.SelectCommand.Parameters.AddWithValue("fname", AcFileName)
            If stid <> 0 Then
                If stid > 9999 Then
                    da.SelectCommand.Parameters.AddWithValue("stid", stid)
                End If
            End If
            da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
            da.SelectCommand.Parameters.AddWithValue("docimage", DocImage)
            da.SelectCommand.Parameters.AddWithValue("oUID", oUID)
            da.SelectCommand.Parameters.AddWithValue("filesize", filesize)
            '' new added for hcl 
            ''&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
            da.SelectCommand.Parameters.AddWithValue("doctype", Doctype)
            da.SelectCommand.Parameters.AddWithValue("fup_FM", fup_fieldmapping)
            da.SelectCommand.Parameters.AddWithValue("fup_FileName", EID & "/" & docurl)
            da.SelectCommand.Parameters.AddWithValue("LOC_FM", loc_fieldmapping)
            da.SelectCommand.Parameters.AddWithValue("LOCID", location_id)
            da.SelectCommand.Parameters.AddWithValue("BARCODE_FM", BarCodeFldmapping)
            da.SelectCommand.Parameters.AddWithValue("BARCODE_VAL", barcodefldval)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim fileID As Integer = da.SelectCommand.ExecuteScalar()


            ' '' no need below autonumber gen. code because entry in now in draft table. by sunil. dec-18
            ' '' code for adding autonumber field in document created by scheduler starts  - 23_apr_14
            'da.SelectCommand.CommandType = CommandType.Text
            'da.SelectCommand.CommandText = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & EID & " and FormName = '" & Doctype & "' order by displayOrder"
            'Dim ds As New DataSet
            'da.Fill(ds, "fields")
            'Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
            'If row.Length > 0 Then
            '    da.SelectCommand.Parameters.Clear()
            '    da.SelectCommand.CommandText = "usp_GetAutoNoNew"
            '    da.SelectCommand.CommandType = CommandType.StoredProcedure
            '    da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
            '    da.SelectCommand.Parameters.AddWithValue("docid", fileID)
            '    da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
            '    da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
            '    Dim an As String = da.SelectCommand.ExecuteScalar()
            '    ' msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
            '    da.SelectCommand.Parameters.Clear()
            'End If
            ' '' code for adding autonumber field in document created by scheduler ends here - 23_apr_14
            ' '' no need above autonumber gen. code because entry in now in draft table. by sunil. dec-18

            'Dim ob As New DMSUtil()
            'ob.CheckWorkFlow(fileID, EID)

            ' '' no need above autonumber gen. code because entry in now in draft table. by sunil. dec-18

        ElseIf Result = "REJECTED" Then
            'da.SelectCommand.Parameters.Clear()
            'da.SelectCommand.CommandText = "select * from mmm_mst_doc where eid=" & EID & " and docurl='" & docurl & "'"
            'da.SelectCommand.CommandType = CommandType.Text
            'Dim dtR As New DataTable
            'da.Fill(dtR)
            'Dim SeleDocID As Integer = 0
            'If dtR.Rows.Count <> 0 Then
            '    SeleDocID = dtR.Rows(0).Item("tid").ToString
            'End If

            'da.SelectCommand.CommandText = "ReCreateDoc_DefaultMovement"
            'da.SelectCommand.CommandType = CommandType.StoredProcedure
            'da.SelectCommand.Parameters.Clear()
            'da.SelectCommand.Parameters.AddWithValue("tid", SeleDocID)
            'da.SelectCommand.Parameters.AddWithValue("CUID", dtR.Rows(0).Item("oUID"))
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'da.SelectCommand.ExecuteNonQuery()
            '' Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
            'Dim dm As New DMSUtil()
            'Call dm.GetNextUserFromRolematrix(SeleDocID, EID, dtR.Rows(0).Item("oUID"), "", dtR.Rows(0).Item("oUID"))
        ElseIf Result = "NOT REJECTED" Then
            Call FileTransferDyn_send_duplicity_inti_mail_toUser(barcodefldval & "." & ext, oUID)
            'da.SelectCommand.Parameters.Clear()
            'da.SelectCommand.CommandText = "uspAddtoDuplicateFileLog"
            'da.SelectCommand.CommandType = CommandType.StoredProcedure
            'da.SelectCommand.Parameters.AddWithValue("EID", EID)
            'da.SelectCommand.Parameters.AddWithValue("docurl", barcodefldval & "." & ext)
            'da.SelectCommand.Parameters.AddWithValue("ouid", oUID)
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
        End If

        con.Close()
        da.Dispose()
        con.Dispose()

        If Result = "NOT EXIST" Then
            Return "ADDED SUCCESSFULLY"
        ElseIf Result = "REJECTED" Then
            Return "FILE REFRESHED"
        ElseIf Result = "NOT REJECTED" Then
            Return "DUPLICATE FILE FOUND"
        Else
            Return "SOME UNKNOWN ERROR"
        End If
    End Function



    '' prev before duplicity check configurable - by sp on 14-Nov-18
    'Public Function MoveFile(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer, ByVal BarCodeFldmapping As String, ByVal barcodefldval As String) As String Implements IDMSService.MoveFile
    '    '& EID & "&gid=" & Gid & "&STID=0&docurl=" & foundfile.Name & "&oUID=" & uid & "&filesize=" & sz & "" & "&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
    '    Dim DocImage As String = ""
    '    Dim ext As String = Right(docurl, 4)
    '    If Left(ext, 1) = "." Then
    '        ext = Right(ext, 3)
    '    End If

    '    '' user below line to add datetimestamp to file name by sp on 17_may_14
    '    '' temp disabled by sunil 20_may_14
    '    'Dim dtstamp As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond
    '    'docurl = docurl.Replace("." & ext, dtstamp & "." & ext)

    '    Dim AcFileName As String = Replace(docurl, "." & ext, "")
    '    Select Case ext.ToUpper()
    '        Case "DOC", "DOCX"
    '            DocImage = "word.png"
    '        Case "JPG"
    '            DocImage = "jpeg.png"
    '        Case "PDF"
    '            DocImage = "adobe.png"
    '        Case "XLS", "XLSX"
    '            DocImage = "excel.png"
    '        Case Else
    '            DocImage = "nofileimage.png"
    '    End Select

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("", con)


    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If

    '    '' for duplicity check 
    '    '' for duplicity check 
    '    Dim BC As String()
    '    If EID = 7 Then
    '        BC = barcodefldval.Split("_")
    '        barcodefldval = BC(0)
    '    End If
    '    '"rm20170320_0001_2017320907_839"
    '    If EID = 127 Then
    '        BC = barcodefldval.Split("_")
    '        If BC.Length = 4 Then
    '            barcodefldval = BC(0) & "_" & BC(1)
    '        ElseIf BC.Length = 3 Then
    '            barcodefldval = BC(0)
    '        Else
    '            barcodefldval = BC(0)
    '        End If
    '    End If


    '    da.SelectCommand.Parameters.Clear()
    '    da.SelectCommand.CommandText = "uspCheckDuplicateDoc_Tecum"
    '    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '    da.SelectCommand.Parameters.AddWithValue("eid", EID)
    '    da.SelectCommand.Parameters.AddWithValue("Barcode", barcodefldval)
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If

    '    Dim Result As String = da.SelectCommand.ExecuteScalar() ' will return one of results - NOT EXIST, REJECTED, NOT REJECTED

    '    If Result = "NOT EXIST" Then
    '        da.SelectCommand.CommandText = "uspAddFileService_MyndSaas_Tecum"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.Clear()

    '        da.SelectCommand.Parameters.AddWithValue("eid", EID)
    '        da.SelectCommand.Parameters.AddWithValue("gid", gid)
    '        da.SelectCommand.Parameters.AddWithValue("fname", AcFileName)
    '        If stid <> 0 Then
    '            If stid > 9999 Then
    '                da.SelectCommand.Parameters.AddWithValue("stid", stid)
    '            End If
    '        End If
    '        da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
    '        da.SelectCommand.Parameters.AddWithValue("docimage", DocImage)
    '        da.SelectCommand.Parameters.AddWithValue("oUID", oUID)
    '        da.SelectCommand.Parameters.AddWithValue("filesize", filesize)
    '        '' new added for hcl 
    '        ''&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
    '        da.SelectCommand.Parameters.AddWithValue("doctype", Doctype)
    '        da.SelectCommand.Parameters.AddWithValue("fup_FM", fup_fieldmapping)
    '        da.SelectCommand.Parameters.AddWithValue("fup_FileName", EID & "/" & docurl)
    '        da.SelectCommand.Parameters.AddWithValue("LOC_FM", loc_fieldmapping)
    '        da.SelectCommand.Parameters.AddWithValue("LOCID", location_id)
    '        da.SelectCommand.Parameters.AddWithValue("BARCODE_FM", BarCodeFldmapping)
    '        da.SelectCommand.Parameters.AddWithValue("BARCODE_VAL", barcodefldval)

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '        Dim fileID As Integer = da.SelectCommand.ExecuteScalar()

    '        '' code for adding autonumber field in document created by scheduler starts  - 23_apr_14
    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.CommandText = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & EID & " and FormName = '" & Doctype & "' order by displayOrder"
    '        Dim ds As New DataSet
    '        da.Fill(ds, "fields")
    '        Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
    '        If row.Length > 0 Then
    '            da.SelectCommand.Parameters.Clear()
    '            da.SelectCommand.CommandText = "usp_GetAutoNoNew"
    '            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '            da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
    '            da.SelectCommand.Parameters.AddWithValue("docid", fileID)
    '            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
    '            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '            Dim an As String = da.SelectCommand.ExecuteScalar()
    '            ' msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
    '            da.SelectCommand.Parameters.Clear()
    '        End If
    '        '' code for adding autonumber field in document created by scheduler ends here - 23_apr_14
    '        Dim ob As New DMSUtil()
    '        ob.CheckWorkFlow(fileID, EID)

    '    ElseIf Result = "REJECTED" Then
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandText = "select * from mmm_mst_doc where eid=" & EID & " and docurl='" & docurl & "'"
    '        da.SelectCommand.CommandType = CommandType.Text
    '        Dim dtR As New DataTable
    '        da.Fill(dtR)
    '        Dim SeleDocID As Integer = 0
    '        If dtR.Rows.Count <> 0 Then
    '            SeleDocID = dtR.Rows(0).Item("tid").ToString
    '        End If

    '        da.SelectCommand.CommandText = "ReCreateDoc_DefaultMovement"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.Parameters.AddWithValue("tid", SeleDocID)
    '        da.SelectCommand.Parameters.AddWithValue("CUID", dtR.Rows(0).Item("oUID"))
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        da.SelectCommand.ExecuteNonQuery()
    '        ' Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
    '        Dim dm As New DMSUtil()
    '        Call dm.GetNextUserFromRolematrix(SeleDocID, EID, dtR.Rows(0).Item("oUID"), "", dtR.Rows(0).Item("oUID"))
    '    ElseIf Result = "NOT REJECTED" Then
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandText = "uspAddtoDuplicateFileLog"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.AddWithValue("EID", EID)
    '        da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
    '        da.SelectCommand.Parameters.AddWithValue("ouid", oUID)
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
    '    End If

    '    con.Close()
    '    da.Dispose()
    '    con.Dispose()

    '    If Result = "NOT EXIST" Then
    '        Return "ADDED SUCCESSFULLY"
    '    ElseIf Result = "REJECTED" Then
    '        Return "FILE REFRESHED"
    '    ElseIf Result = "NOT REJECTED" Then
    '        Return "DUPLICATE FILE FOUND"
    '    Else
    '        Return "SOME UNKNOWN ERROR"
    '    End If
    'End Function

    '' prev b4 datetime stamp added 
    'Public Function MoveFile(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer) As String Implements IDMSService.MoveFile
    '    '& EID & "&gid=" & Gid & "&STID=0&docurl=" & foundfile.Name & "&oUID=" & uid & "&filesize=" & sz & "" & "&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
    '    Dim DocImage As String = ""
    '    Dim ext As String = Right(docurl, 4)
    '    If Left(ext, 1) = "." Then
    '        ext = Right(ext, 3)
    '    End If

    '    '' user below line to add datetimestamp to file name by sp on 17_may_14
    '    Dim dtstamp As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond
    '    docurl = docurl.Replace("." & ext, dtstamp & "." & ext)

    '    Dim AcFileName As String = Replace(docurl, "." & ext, "")
    '    Select Case ext.ToUpper()
    '        Case "DOC", "DOCX"
    '            DocImage = "word.png"
    '        Case "JPG"
    '            DocImage = "jpeg.png"
    '        Case "PDF"
    '            DocImage = "adobe.png"
    '        Case "XLS", "XLSX"
    '            DocImage = "excel.png"
    '        Case Else
    '            DocImage = "nofileimage.png"
    '    End Select

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

    '    da.SelectCommand.Parameters.Clear()
    '    da.SelectCommand.CommandText = "uspCheckDuplicateDoc"
    '    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '    da.SelectCommand.Parameters.AddWithValue("eid", EID)
    '    da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If

    '    Dim Result As String = da.SelectCommand.ExecuteScalar() ' will return one of results - NOT EXIST, REJECTED, NOT REJECTED

    '    If Result = "NOT EXIST" Then
    '        da.SelectCommand.CommandText = "uspAddFileService_MyndSaas"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.Clear()

    '        da.SelectCommand.Parameters.AddWithValue("eid", EID)
    '        da.SelectCommand.Parameters.AddWithValue("gid", gid)
    '        da.SelectCommand.Parameters.AddWithValue("fname", AcFileName)
    '        If stid <> 0 Then
    '            If stid > 9999 Then
    '                da.SelectCommand.Parameters.AddWithValue("stid", stid)
    '            End If
    '        End If
    '        da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
    '        da.SelectCommand.Parameters.AddWithValue("docimage", DocImage)
    '        da.SelectCommand.Parameters.AddWithValue("oUID", oUID)
    '        da.SelectCommand.Parameters.AddWithValue("filesize", filesize)
    '        '' new added for hcl 
    '        ''&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID
    '        da.SelectCommand.Parameters.AddWithValue("doctype", Doctype)
    '        da.SelectCommand.Parameters.AddWithValue("fup_FM", fup_fieldMapping)
    '        da.SelectCommand.Parameters.AddWithValue("fup_FileName", eid & "/" & docurl)
    '        da.SelectCommand.Parameters.AddWithValue("LOC_FM", loc_fieldMapping)
    '        da.SelectCommand.Parameters.AddWithValue("LOCID", location_id)

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If

    '        Dim fileID As Integer = da.SelectCommand.ExecuteScalar()


    '        '' code for adding autonumber field in document created by scheduler starts  - 23_apr_14
    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.CommandText = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number') and F.EID=" & EID & " and FormName = '" & Doctype & "' order by displayOrder"
    '        Dim ds As New DataSet
    '        da.Fill(ds, "fields")
    '        Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
    '        If row.Length > 0 Then
    '            da.SelectCommand.Parameters.Clear()
    '            da.SelectCommand.CommandText = "usp_GetAutoNoNew"
    '            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '            da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
    '            da.SelectCommand.Parameters.AddWithValue("docid", fileID)
    '            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
    '            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '            Dim an As String = da.SelectCommand.ExecuteScalar()
    '            ' msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
    '            da.SelectCommand.Parameters.Clear()
    '        End If
    '        '' code for adding autonumber field in document created by scheduler ends here - 23_apr_14


    '        Dim ob As New DMSUtil()
    '        ob.CheckWorkFlow(fileID, EID)

    '    ElseIf Result = "REJECTED" Then

    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandText = "select * from mmm_mst_doc where eid=" & EID & " and docurl='" & docurl & "'"
    '        da.SelectCommand.CommandType = CommandType.Text
    '        Dim dtR As New DataTable
    '        da.Fill(dtR)
    '        Dim SeleDocID As Integer = 0
    '        If dtR.Rows.Count <> 0 Then
    '            SeleDocID = dtR.Rows(0).Item("tid").ToString
    '        End If
    '        '' this was previous code 
    '        '' now call approve workflow proc to move to next user 
    '        'da.SelectCommand.Parameters.Clear()
    '        'da.SelectCommand.CommandText = "ApproveWorkFlow"
    '        'da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        'da.SelectCommand.Parameters.AddWithValue("TID", SeleDocID)
    '        'If con.State <> ConnectionState.Open Then
    '        '    con.Open()
    '        'End If

    '        'Dim iSt As Integer = da.SelectCommand.ExecuteScalar()
    '        '' new code added in myndbpm platform
    '        '''' following is for tecumseh company (queueing) 
    '        'CheckandApplyWorkFlowTecum(docid, eid)
    '        da.SelectCommand.CommandText = "ReCreateDoc_DefaultMovement"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.Parameters.AddWithValue("tid", SeleDocID)
    '        da.SelectCommand.Parameters.AddWithValue("CUID", dtR.Rows(0).Item("oUID"))
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        da.SelectCommand.ExecuteNonQuery()
    '        ' Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
    '        Dim dm As New DMSUtil()
    '        Call dm.GetNextUserFromRolematrix(SeleDocID, EID, dtR.Rows(0).Item("oUID"), "", dtR.Rows(0).Item("oUID"))

    '        '' prev code bkup on 31_mar_14
    '        'da.SelectCommand.Parameters.Clear()
    '        'da.SelectCommand.CommandText = "select tid from mmm_mst_doc where eid=" & EID & " and docurl='" & docurl & "'"
    '        'da.SelectCommand.CommandType = CommandType.Text
    '        'Dim dtR As New DataTable
    '        'da.Fill(dtR)
    '        'Dim SeleDocID As Integer = 0
    '        'If dtR.Rows.Count <> 0 Then
    '        '    SeleDocID = dtR.Rows(0).Item("tid").ToString
    '        'End If
    '        ''' now call approve workflow proc to move to next user
    '        'da.SelectCommand.Parameters.Clear()
    '        'da.SelectCommand.CommandText = "ApproveWorkFlow"
    '        'da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        'da.SelectCommand.Parameters.AddWithValue("TID", SeleDocID)
    '        'If con.State <> ConnectionState.Open Then
    '        '    con.Open()
    '        'End If

    '        'Dim iSt As Integer = da.SelectCommand.ExecuteScalar()

    '    ElseIf Result = "NOT REJECTED" Then
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandText = "uspAddtoDuplicateFileLog"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.AddWithValue("EID", EID)
    '        da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
    '        da.SelectCommand.Parameters.AddWithValue("ouid", oUID)
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim fileID As Integer = da.SelectCommand.ExecuteScalar()
    '    End If

    '    con.Close()
    '    da.Dispose()
    '    con.Dispose()
    '    'Dim ob As New DMSUtil()
    '    'ob.InsertAction(oUID, "FILE UPLOADED", AcFileName & " is Uploaded")
    '    If Result = "NOT EXIST" Then
    '        Return "ADDED SUCCESSFULLY"
    '    ElseIf Result = "REJECTED" Then
    '        Return "FILE REFRESHED"
    '    ElseIf Result = "NOT REJECTED" Then
    '        Return "DUPLICATE FILE FOUND"
    '    Else
    '        Return "SOME UNKNOWN ERROR"
    '    End If
    'End Function

    Public Shared Function distance(Lat1 As Double, Long1 As Double, Lat2 As Double, Long2 As Double, unit As Char) As Double

        Dim dDistance As Double = [Double].MinValue
        Dim dLat1InRad As Double = Lat1 * (Math.PI / 180.0)
        Dim dLong1InRad As Double = Long1 * (Math.PI / 180.0)
        Dim dLat2InRad As Double = Lat2 * (Math.PI / 180.0)
        Dim dLong2InRad As Double = Long2 * (Math.PI / 180.0)

        Dim dLongitude As Double = dLong2InRad - dLong1InRad
        Dim dLatitude As Double = dLat2InRad - dLat1InRad

        ' Intermediate result a.
        Dim a As Double = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) + Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) * Math.Pow(Math.Sin(dLongitude / 2.0), 2.0)

        ' Intermediate result c (great circle distance in Radians).
        Dim c As Double = 2.0 * Math.Asin(Math.Sqrt(a))

        ' Distance.
        ' const Double kEarthRadiusMiles = 3956.0;
        Const kEarthRadiusKms As [Double] = 6376.5
        dDistance = kEarthRadiusKms * c

        Return dDistance
    End Function

    Public Function GetSiteInfo(ID As Integer, Eid As Integer) As String Implements IDMSService.GetSiteInfo

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim Info As New StringBuilder()

        Dim query As String = ""

        If Eid = 0 Then
            query = "select m.tid[TID],m.fld10[SiteID],m.fld11[SiteName],"
            query &= "m.fld13[Address],m1.fld1[Site],ltrim(rtrim(left(replace(m.fld21,',','        '),9)))[Lat] ,rtrim(ltrim(right(replace(m.fld21,',','        '),9)))[Long], m.fld12[Group],m.fld11[Site Name],u.UserName[OandM Head],u1.UserName[Maintenance Head], u2.UserName[Opex Manager],u3.UserName[Security Manager],u4.UserName[Zonal Head],u5.UserName[Cluster Manager],m2.fld1[Supervisor], m3.fld1[Technician], m.fld2[No of OPCOs],m.fld19[Anchor OPCO],m.fld15[Cluster] from  mmm_mst_master m with (nolock) inner join mmm_mst_user u with (nolock) on convert(nvarchar,u.uid)=m.fld23 join mmm_mst_user u1 with (nolock) on convert(nvarchar,u1.uid)=m.fld24 join mmm_mst_user u2 with (nolock) on convert(nvarchar,u2.uid)=m.fld25  join mmm_mst_user u3 with (nolock) on convert(nvarchar,u3.uid)=m.fld26  join mmm_mst_user u4 with (nolock) on convert(nvarchar,u4.uid)=m.fld27 join mmm_mst_user u5 with (nolock) on convert(nvarchar,u5.uid)=m.fld28 join mmm_mst_master m2 with (nolock) on convert(nvarchar,m2.tid)=m.fld29 join mmm_mst_master m3 with (nolock) on convert(nvarchar,m3.tid)=m.fld3 join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=54 and m.eid=54 and  u.eid=54 and "
            query &= " u1.eid=54 and u2.eid=54 and m2.eid=54 and m3.eid=54 and m.Tid=" & ID
        ElseIf Eid = 73 Then
            Dim qry = "Select m.tid[TID],m.fld10[SiteID],m.fld11[SiteName],isNull(m.fld13,'')[Address],m1.fld1[Site],ltrim(rtrim(left(replace(m.fld21,',','        '),9)))[Lat] ,"
            qry &= " rtrim(ltrim(right(replace(m.fld21,',','        '),9)))[Long], m.fld12[Group] "
            qry &= " from mmm_mst_master m with (nolock) join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12 where m.Tid=" & ID & " and m.eid=73 and m1.Eid=73"
            Dim da As SqlDataAdapter = New SqlDataAdapter(qry, con)
            Dim dt1 As New DataTable
            da.Fill(dt1)
            Info.Append("SiteID : " + dt1.Rows(0).Item("SiteID").ToString())
            Info.Append(",")
            Info.Append("Site : " + dt1.Rows(0).Item("Site").ToString())
            Info.Append(",")
            Info.Append("Site Name : " + dt1.Rows(0).Item("SiteName").ToString())
            Return Info.ToString()
        Else
            query = "select m.tid[TID],m.fld10[SiteID],m.fld11[SiteName],"
            query &= "m.fld13[Address],m1.fld1[Site],ltrim(rtrim(left(replace(m.fld21,',','        '),9)))[Lat] ,rtrim(ltrim(right(replace(m.fld21,',','        '),9)))[Long], m.fld12[Group],m.fld11[Site Name],u.UserName[OandM Head],u1.UserName[Maintenance Head], u2.UserName[Opex Manager],u3.UserName[Security Manager],u4.UserName[Zonal Head],u5.UserName[Cluster Manager],m2.fld1[Supervisor], m3.fld1[Technician], m.fld2[No of OPCOs],m.fld19[Anchor OPCO],m.fld15[Cluster] from  mmm_mst_master m with (nolock) inner join mmm_mst_user u with (nolock) on convert(nvarchar,u.uid)=m.fld23 join mmm_mst_user u1 with (nolock) on convert(nvarchar,u1.uid)=m.fld24 join mmm_mst_user u2 with (nolock) on convert(nvarchar,u2.uid)=m.fld25  join mmm_mst_user u3 with (nolock) on convert(nvarchar,u3.uid)=m.fld26  join mmm_mst_user u4 with (nolock) on convert(nvarchar,u4.uid)=m.fld27 join mmm_mst_user u5 with (nolock) on convert(nvarchar,u5.uid)=m.fld28 join mmm_mst_master m2 with (nolock) on convert(nvarchar,m2.tid)=m.fld29 join mmm_mst_master m3 with (nolock) on convert(nvarchar,m3.tid)=m.fld3 join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=" & Eid & " and m.eid=" & Eid & " and  u.eid=" & Eid & " and "
            query &= " u1.eid=" & Eid & " and u2.eid=" & Eid & " and m2.eid=" & Eid & " and m3.eid=" & Eid & " and m.Tid=" & ID
        End If


        Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
        Dim dt As New DataTable
        oda.Fill(dt)

        If dt.Rows.Count = 0 Then
            Return ""
        End If

        Info.Append("SiteID : " + dt.Rows(0).Item("SiteID").ToString())
        Info.Append(",")
        Info.Append("Site : " + dt.Rows(0).Item("Site").ToString())
        Info.Append(",")
        Info.Append("Site Name : " + dt.Rows(0).Item("SiteName").ToString())
        Info.Append(",")
        Info.Append("OandM Head: " + dt.Rows(0).Item("OandM Head").ToString())
        Info.Append(",")
        Info.Append("Maintenance Head : " + dt.Rows(0).Item("Maintenance Head").ToString())
        Info.Append(",")
        Info.Append("Opex Manager : " + dt.Rows(0).Item("Opex Manager").ToString())
        Info.Append(",")
        Info.Append("Security Manager : " + dt.Rows(0).Item("Security Manager").ToString())
        Info.Append(",")
        Info.Append("Zonal Head : " + dt.Rows(0).Item("Zonal Head").ToString())
        Info.Append(",")
        Info.Append("Cluster Manager :" + dt.Rows(0).Item("Cluster Manager").ToString())
        Info.Append(",")
        Info.Append("Supervisor : " + dt.Rows(0).Item("Supervisor").ToString())
        Info.Append(",")
        Info.Append("Technician : " + dt.Rows(0).Item("Technician").ToString())
        Info.Append(",")
        Info.Append("No of OPCOs : " + dt.Rows(0).Item("No of OPCOs").ToString())
        Info.Append(",")
        Info.Append("Anchor OPCO : " + dt.Rows(0).Item("Anchor OPCO").ToString())

        Return Info.ToString()

    End Function

    Public Function GetVehicleInfo(IMIENO As String, Eid As Integer) As String Implements IDMSService.GetVehicleInfo

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim Info As New StringBuilder()

        Dim query As String = ""

        If Eid = 0 Then
            query = "select convert(nvarchar,g.tid)[TID],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],CONVERT(VARCHAR(20), ctime, 113)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed"
            query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno "
            query &= "where m2.documenttype='vehicle'  and m2.eid=54 and m2.fld12='" & IMIENO & "' " 'and m2.fld14 in (" & Ids.TrimEnd(CChar(",")) & ")
            query &= " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime)"
            query &= " from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) "
        Else
            query = "select convert(nvarchar,g.tid)[TID],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],CONVERT(VARCHAR(20), ctime, 113)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed"
            query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno "
            query &= "where m2.documenttype='vehicle'  and m2.eid=" & Eid & "  and m2.fld12='" & IMIENO & "' " 'and m2.fld14 in (" & Ids.TrimEnd(CChar(",")) & ")
            query &= " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime)"
            query &= " from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) "
        End If

        Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
        Dim dt As New DataTable
        oda.Fill(dt)

        If dt.Rows.Count = 0 Then
            Return ""
        End If

        Dim TotalHrs As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) / 60
        Dim TotalMints As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) Mod 60

        Dim hr As String
        Dim mm As String

        hr = If(TotalHrs < 10, "0" & TotalHrs.ToString(), TotalHrs.ToString())
        mm = If(TotalMints < 10, "0" & TotalMints.ToString(), TotalMints.ToString())

        Dim dipTime As String = hr & ":" & mm

        Info.Append("IMEINO : " & dt.Rows(0).Item("imieno").ToString())
        Info.Append(",")
        Info.Append("Vehicle Name : " + dt.Rows(0).Item("Site_Name").ToString())
        Info.Append(",")
        Info.Append("Vehicle No : " + dt.Rows(0).Item("vehicleNo").ToString())
        Info.Append(",")
        Info.Append("Speed : " + dt.Rows(0).Item("Speed").ToString() + " Km/h")
        Info.Append(",")
        Info.Append("Ideal Time : " + dipTime.ToString() + "(HH:MM) ")
        Info.Append(",")
        Info.Append("Last Record Time : " + dt.Rows(0).Item("ctime").ToString())
        Info.Append(",")
        Info.Append("Lattitude : " + dt.Rows(0).Item("Lat").ToString())
        Info.Append(",")
        Info.Append("Longitude : " + dt.Rows(0).Item("Long").ToString())

        Return Info.ToString()

    End Function

    Public Function CreateUsers(userName As String, emailID As String, pwd As String, code As String) As String Implements IDMSService.CreateUsers
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUserServiceChange", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("username", Trim(userName))
        oda.SelectCommand.Parameters.AddWithValue("emailid", Trim(emailID))
        oda.SelectCommand.Parameters.AddWithValue("pwd", pwd)
        oda.SelectCommand.Parameters.AddWithValue("Code", code)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Then
            Return "User " & userName & " Created "
        Else
            Return "Users " & userName & " Updated"
        End If
    End Function
    Public Function MyndVAS(ByVal mn As String, ByVal msg As String) As String Implements IDMSService.MyndVas
        If mn.Length > 10 Then
            mn = Right(mn, 10)
        End If
        If mn.ToString = "9311111870" Or mn.ToString = "9810923322" Then
            Dim rnd As String = CheckMyndApp(mn, msg)
            Return rnd
            Exit Function
        End If
        msg = Trim(msg)
        Dim keywd As String = Left(msg, msg.IndexOf(" "))
        msg = msg.Replace(keywd, "").Trim()
        Dim para() As String
        para = msg.Split(",")
        Dim dtv() As String
        dtv = msg.Split(",")

        'Since parameter is recieved.. just make entry in log table  
        Dim tid As Integer = InsertSMSLog(mn, keywd, msg.Replace(keywd, ""))
        Dim eid As Integer = 0
        If para.Length < 1 Or keywd.Length() < 1 Then
            'Invalid keyword message
            SendReply("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.", tid, eid)
            Return "Error"
            Exit Function
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter(" ", con)
        Dim dt As New DataTable()
        Try
            da.SelectCommand.CommandText = "SELECT * from MMM_MST_SMSKEYWORDS where keywordname='" & keywd & "'"
            da.Fill(dt)
            Dim UM As String = ""
            Dim DM As String = ""
            Dim Ddays As Integer
            If dt.Rows.Count > 0 Then
                UM = dt.Rows(0).Item("userMobField").ToString
                DM = dt.Rows(0).Item("DriverMobFld").ToString
                Ddays = Val(dt.Rows(0).Item("DriverRestDays").ToString)
                eid = Val(dt.Rows(0).Item("eid"))
            End If

            'check if keyword is help
            If para(0).ToUpper() = "HELP" Then
                If dt.Rows.Count = 1 Then
                    SendReply("9037", mn, "Usage of " & keywd & ": " & dt.Rows(0).Item("helpingmsg").ToString() & " in exact sequence", tid, eid)
                Else
                    SendReply("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.", tid, eid)
                End If
                da.Dispose()
                con.Close()
                con.Dispose()
                Return "DONE"
                Exit Function
            End If
            If dt.Rows.Count <> 1 Then
                SendReply("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.", tid, eid)
                da.Dispose()
                con.Close()
                con.Dispose()
                Return "ERROR"
                Exit Function
            End If
            'Now lets find out the keyword saved in DataBase
            If para.Count <> Val(dt.Rows(0).Item("ParaCount").ToString()) Then
                SendReply("8759", mn, "This keywords require " & dt.Rows(0).Item("ParaCount").ToString() & " parameters and you supplied " & para.Count & " parameters", tid, eid)
                da.Dispose()
                con.Close()
                con.Dispose()
                Return "ERROR"
                Exit Function
            End If
            Dim uid As Integer
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ttype As String = dt.Rows(0).Item("ttype").ToString
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String
            Dim sdt As String
            Dim edt As String
            Dim ds1 As New DataSet()
            'common check for settings
            eid = dt.Rows(0).Item("EID").ToString
            oda.SelectCommand.CommandText = "select* from mmm_mst_entity where eid=" & eid & " "
            oda.Fill(ds1, "data")
            If ds1.Tables("data").Rows.Count > 0 Then
                Udtype = ds1.Tables("data").Rows(0).Item("uvdtype").ToString
                Ufld = ds1.Tables("data").Rows(0).Item("uvuserfield").ToString
                UVfld = ds1.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                Vdtype = ds1.Tables("data").Rows(0).Item("VIDType").ToString
                Vfld = ds1.Tables("data").Rows(0).Item("vivehiclefield").ToString
                vemei = ds1.Tables("data").Rows(0).Item("viimeifield").ToString
                sdt = ds1.Tables("data").Rows(0).Item("UVStartDateTime").ToString
                edt = ds1.Tables("data").Rows(0).Item("UVEndDateTime").ToString
            End If
            'Now Check Device Last Signal
            If ttype.ToUpper = "DEV SIGNAL" Then
                'oda.SelectCommand.CommandText = "select max(recordtime) from mmm_mst_gpsdata where imieno='" & para(0).ToString & "'"
                oda.SelectCommand.CommandText = "select max(recordtime),(select tripon from mmm_mst_gpsdata where tid=max(g.tid))  from mmm_mst_gpsdata g where imieno='" & para(0).ToString & "' group by imieno"
                Dim dtdevch As New DataTable
                oda.Fill(dtdevch)
                Dim lsig As String = ""
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'lsig = oda.SelectCommand.ExecuteScalar().ToString
                If dtdevch.Rows.Count > 0 Then
                    lsig = dtdevch.Rows(0).Item(0).ToString
                    If dtdevch.Rows(0).Item(1).ToString = "1" Then
                        lsig = lsig & " and Switch On"
                    Else
                        lsig = lsig & " and Switch Off"
                    End If
                End If
                con.Dispose()
                If lsig <> "" Then
                    SendReply("14751", mn, "Dear User, Last Signal of this IMEI is " & lsig.ToString & ". Thanks", tid, eid)
                    Return "Dear User, Last Signal of this IMEI is " & lsig.ToString & ". Thanks"
                    Exit Function
                Else
                    SendReply("14752", mn, "Dear User, IMEI entered by you is invalid, Please retry with valid IMEI.", tid, eid)
                    Return "Dear User, IMEI entered by you is invalid, Please retry with valid IMEI."
                    Exit Function
                End If
            End If

            'Now check Trip entry through Driver
            If ttype.ToUpper = "DTRIP START" Or ttype.ToUpper = "DTRIP END" Or ttype.ToUpper = "DELOG" Or ttype.ToUpper = "NIL TRIP DRIVER" Then
                Dim vid As String
                'Dim uid As String = ""
                oda.SelectCommand.CommandText = "select tid,eid from  mmm_mst_master where " & DM & " like '%" & mn & "%'"
                Dim dtdm As New DataTable
                oda.Fill(dtdm)
                If dtdm.Rows.Count = 1 Then
                    vid = dtdm.Rows(0).Item(0).ToString
                    eid = dtdm.Rows(0).Item(1).ToString
                ElseIf dtdm.Rows.Count > 1 Then
                    SendReply("9802", mn, "Dear User, your request can’t be processed as your mobile number exists in more than one account. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as your mobile number exists in more than one account. Please contact your Admin"
                    Exit Function
                Else
                    SendReply("9544", mn, "Dear user, Mobile Number does not exist in the system.", tid, eid)
                    Return "Mobile Number does not exist in the system."
                    Exit Function
                End If

                'New Change 12th APR 2014
                If Trim(para(0)).ToString.Length = 11 Then
                    para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                ElseIf Trim(para(0)).ToString.Length = 10 Then
                    para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(6, 2) & ":" & para(0).Substring(8, 2)
                End If
                'Old Code
                'oda.SelectCommand.CommandText = "select  " & UVfld & "," & Ufld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & UVfld & " = " & vid & " and curstatus in ('Allotted')"
                'New Change 12th APR 2014
                oda.SelectCommand.CommandText = "select  " & UVfld & "," & Ufld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & UVfld & " = " & vid & " and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, " & sdt & ",3)<= convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21)<=CONVERT(datetime, " & edt & ",3) and CONVERT(datetime," & edt & ",3)>=convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) >= CONVERT(datetime, " & sdt & ",3)"
                Dim dtcnt As New DataTable
                oda.Fill(dtcnt)
                If dtcnt.Rows.Count > 1 Then
                    SendReply("9804", mn, "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin"
                    Exit Function
                ElseIf dtcnt.Rows.Count = 0 Then
                    SendReply("9803", mn, "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin"
                    Exit Function
                Else
                    uid = dtcnt.Rows(0).Item(1).ToString
                End If
                'Old Code
                'oda.SelectCommand.CommandText = " select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & eid & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & UVfld & " = " & vid & " and curstatus in ('Allotted'))"
                'New Change 12th APR 2014
                oda.SelectCommand.CommandText = "select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & eid & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & UVfld & " = " & vid & " and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, " & sdt & ",3)<= convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21)<=CONVERT(datetime, " & edt & ",3) and CONVERT(datetime," & edt & ",3)>=convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) >= CONVERT(datetime, " & sdt & ",3))"
                ds1.Clear()
                para(0) = dtv(0)
                oda.Fill(ds1, "vemei")
                Dim vno As String
                Dim IMEINO As String
                If ds1.Tables("vemei").Rows.Count = 1 Then
                    vno = ds1.Tables("vemei").Rows(0).Item(0).ToString
                    IMEINO = ds1.Tables("vemei").Rows(0).Item(1).ToString
                ElseIf ds1.Tables("vemei").Rows.Count = 0 Then
                    SendReply("9803", mn, "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin"
                    Exit Function
                Else
                    SendReply("9804", mn, "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin"
                    Exit Function
                End If
                oda.SelectCommand.CommandText = "select distinct IMIENO from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "' "
                Dim dt1 As DataTable = New DataTable()
                oda.Fill(dt1)
                If dt1.Rows.Count = 0 Then
                    SendReply("9805", mn, "Dear User, your request can’t be processed as your Vehicle is not mapped with GPS Device in System. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as your Vehicle is not mapped with GPS Device in System. Please contact your Admin"
                    Exit Function
                End If
                Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
                Dim tedate As String = ""
                If ttype.ToUpper = "DELOG" Then
                    'Change both value in date and time
                    If Trim(para(0)).ToString.Length > 11 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 11 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    ElseIf Trim(para(1)).ToString.Length > 11 Or para(1).Contains(":") Or Trim(para(0)).ToString.Length < 11 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If
                    para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                    para(1) = "20" & para(1).Substring(4, 2) & "-" & para(1).Substring(2, 2) & "-" & Left(para(1), 2) & " " & para(1).Substring(7, 2) & ":" & para(1).Substring(9, 2)

                    If IsDate(para(0)) = False Or IsDate(para(1)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If

                    'check date for Driver entry 
                    Dim ddt As Date = Date.Now.AddDays(-Ddays).ToString("yyyy-MM-dd")
                    Dim str As String = Left(para(0), 10)
                    If str.ToString < ddt.ToString Then
                        SendReply("10646", mn, "Hi, You cannot make Trip now, since 2 days have been passed. Please contact your Vehicle User to create the Trip.", tid, eid)
                        Return "Hi, You cannot make Trip now, since 2 days have been passed. Please contact your Vehicle User to create the Trip."
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select isnull(sum(devdist),0) from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and   cTime >= convert(datetime,'" & para(0) & "') AND ctime <= convert(datetime,'" & para(1) & "') group by lattitude,longitude)  "
                    Dim sum As DataTable = New DataTable()
                    oda.Fill(sum)
                    If DateTime.Parse(para(0)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    ElseIf DateTime.Parse(para(1)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    End If
                    If CDate(para(0)) > CDate(para(1)) Then
                        SendReply("9806", mn, "Dear User, your request can’t be processed as trip end datetime should be greater than trip start datetime", tid, eid)
                        Return "Dear User, your request can’t be processed as trip end datetime should be greater than trip start datetime"
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & uid & " and imei_no='" & IMEINO & "' and eid=" & eid & ")"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    tedate = oda.SelectCommand.ExecuteScalar().ToString
                    If tedate = "" Then
                        SendReply("9527", mn, "Please End previous trip before starting new trip.", tid, eid)
                        Return "Please End previous trip before starting new trip."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(1) & "' <= trip_end_Datetime and '" & para(1) & "' >= trip_start_Datetime) "
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "uspInsertSMSELOGBOOKNew"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", para(0))
                    oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", para(1))
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid)
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select lattitude,longitude,ctime,speed,devdist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and  cTime >= convert(datetime,'" & para(0) & "') AND ctime <= convert(datetime,'" & para(1) & "')  group by lattitude,longitude) order by ctime ", con)
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)
                    If dtlatlong.Rows.Count > 0 Then
                        oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString()))
                        oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", Location(dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString(), dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString()))
                    Else
                        SendReply("9807", mn, "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time", tid, eid)
                        Return "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time"
                        Exit Function
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("Total_Distance", sum.Rows(0).ItemArray(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    oda.SelectCommand.Parameters.AddWithValue("isAuth", 0)
                    oda.SelectCommand.Parameters.AddWithValue("entryby", "Driver")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    Return "eLog Successfully"
                    Exit Function

                    'Now check for Nil trip
                ElseIf ttype.ToUpper = "NIL TRIP DRIVER" Then
                    If Trim(para(0)).ToString.Length > 6 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 6 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If

                    para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2)

                    If IsDate(para(0)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & uid & " and imei_no='" & IMEINO & "' and eid=" & eid & ")"
                    Dim dtt As New DataTable()
                    oda.Fill(dtt)

                    If dtt.Rows.Count > 0 Then
                        tedate = dtt.Rows(0).Item("Trip_end_datetime").ToString()
                        If tedate = "" Then
                            SendReply("9527", mn, "Dear user, Please end previous trip before starting new trip", tid, eid)
                            Return "Please End previous trip before starting new trip."
                            con.Dispose()
                            Exit Function
                        End If
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) "
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "uspInsertSMSELOGBOOKTripStrtNil"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid.ToString())
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", para(0) & " 00:00")
                    oda.SelectCommand.Parameters.AddWithValue("Trip_End_DateTime", para(0) & " 23:59")
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    oda.SelectCommand.Parameters.AddWithValue("isAuth", 0)
                    oda.SelectCommand.Parameters.AddWithValue("entryby", "Driver")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    'pending MSG ID
                    SendReply("10694", mn, "Dear User, Your nil trip has been registered for " & para(0) & " successfully.", tid, eid)
                    Return "Nil Trip Created Successfully"
                    Exit Function
                ElseIf ttype.ToUpper = "DTRIP START" Then
                    If Trim(para(0)).ToString.Length > 11 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 10 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If
                    If Trim(para(0)).ToString.Length = 11 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                    ElseIf Trim(para(0)).ToString.Length = 10 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(6, 2) & ":" & para(0).Substring(8, 2)
                    End If
                    If IsDate(para(0)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If

                    If DateTime.Parse(para(0)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    End If

                    'check date for driver trip entry
                    Dim ddt As Date = Date.Now.AddDays(-Ddays).ToString("yyyy-MM-dd")
                    Dim str As String = Left(para(0), 10)
                    If CDate(str.ToString) < CDate(ddt.ToString) Then
                        SendReply("10646", mn, "Hi, You cannot make Trip now, since 2 days have been passed. Please contact your Vehicle User to create the Trip.", tid, eid)
                        Return "Hi, You cannot make Trip now, since 2 days have been passed. Please contact your Vehicle User to create the Trip."
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & uid & " and imei_no='" & IMEINO & "' and eid=" & eid & ")"
                    Dim dtt As New DataTable()
                    oda.Fill(dtt)

                    If dtt.Rows.Count > 0 Then
                        tedate = dtt.Rows(0).Item("Trip_end_datetime").ToString()
                        If tedate = "" Then
                            SendReply("9527", mn, "Dear user, Please end previous trip before starting new trip", tid, eid)
                            Return "Please End previous trip before starting new trip."
                            con.Dispose()
                            Exit Function
                        End If
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) "
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "uspInsertSMSELOGBOOKTripStrtNew"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", para(0))
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid.ToString())
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select top 1 lattitude,longitude,ctime,speed,devdist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and   convert(nvarchar(16),cTime,121)  >= convert(nvarchar,'" & para(0) & "',120) group by lattitude,longitude) order by ctime ", con)
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)
                    Dim tripstartLocation As String = String.Empty
                    If dtlatlong.Rows.Count > 0 Then
                        tripstartLocation = Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", tripstartLocation)
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    oda.SelectCommand.Parameters.AddWithValue("isAuth", 0)
                    oda.SelectCommand.Parameters.AddWithValue("entryby", "Driver")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    SendReply("9538", mn, "Dear User, Your trip started at " & para(0).ToString() & " from " & tripstartLocation & " successfully", tid, eid)
                    Return "Trip Start Successfully"
                    Exit Function
                ElseIf ttype.ToUpper = "DTRIP END" Then
                    If Trim(para(0)).ToString.Length > 11 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 10 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If
                    If Trim(para(0)).ToString.Length = 11 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                    ElseIf Trim(para(0)).ToString.Length = 10 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(6, 2) & ":" & para(0).Substring(8, 2)
                    End If
                    If IsDate(para(0)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If
                    If DateTime.Parse(para(0)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select trip_start_datetime,trip_end_datetime from mmm_mst_elogbook where imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and tid=(select max(tid) from mmm_mst_elogbook where imei_no='" & IMEINO & "' and vehicle_no='" & vno & "')"
                    Dim dttt As New DataTable()
                    oda.Fill(dttt)
                    If dttt.Rows.Count = 0 Then
                        SendReply("9541", mn, "Dear User, You don't have any open trip", tid, eid)
                        Return "Dear User, You don't have any open trip"
                        Exit Function
                    End If
                    If dttt.Rows(0).Item("trip_end_datetime").ToString.Length() > 0 Then
                        SendReply("9542", mn, "Dear User, You already closed your last trip", tid, eid)
                        Return "Dear User, You already closed your last trip"
                        Exit Function
                    End If
                    Dim stdt As String = dttt.Rows(0).Item("trip_start_datetime").ToString

                    If CDate(stdt) > CDate(para(0)) Then
                        SendReply("9540", mn, "Dear User, Trip End Date should be greater than Trip Start Date", tid, eid)
                        Return "Dear User, Trip End Date should be greater than Trip Start Date"
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & stdt & "' <= trip_end_Datetime and '" & stdt & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime)"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()

                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select isnull(sum(devdist),0) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and   cTime >= convert(datetime,'" & stdt & "') AND ctime <= convert(datetime,'" & para(0) & "') "
                    Dim sum As DataTable = New DataTable()
                    oda.Fill(sum)

                    oda.SelectCommand.CommandText = "uspUpdateSMSELOGBOOKTripEnd"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", para(0))
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid.ToString())
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select  top 1 lattitude,longitude,ctime,speed,devdist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and  convert(nvarchar(16),cTime,121)  >= convert(nvarchar,'" & para(0) & "',120) group by lattitude,longitude) order by ctime ", con)
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)

                    Dim tripsendLocation As String = String.Empty

                    If dtlatlong.Rows.Count > 0 Then
                        'oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString()))
                        tripsendLocation = Location(dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString(), dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", tripsendLocation)
                    Else
                        SendReply("9807", mn, "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time", tid, eid)
                        Return "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time"
                        Exit Function
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("Total_Distance", sum.Rows(0).ItemArray(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    SendReply("9539", mn, "Dear User, Your trip end at " & para(0).ToString() & " from " & tripsendLocation & " Trip length : " & sum.Rows(0).ItemArray(0).ToString() & " successfully", tid, eid)
                    Return "Trip End Successfully"
                    Exit Function
                End If
                Return "ERROR"
                Exit Function
            End If

            'Now check Trip entry through user
            oda.SelectCommand.CommandText = "select uid,eid from  mmm_mst_user where " & UM & " like '%" & mn & "%'"
            Dim dt2 As New DataTable
            oda.Fill(dt2)

            If dt2.Rows.Count = 1 Then
                uid = dt2.Rows(0).Item(0).ToString
                eid = dt2.Rows(0).Item(1).ToString
            ElseIf dt2.Rows.Count > 1 Then
                SendReply("9802", mn, "Dear User, your request can’t be processed as your mobile number exists in more than one account. Please contact your Admin", tid, eid)
                Return "Dear User, your request can’t be processed as your mobile number exists in more than one account. Please contact your Admin"
                Exit Function
            Else
                SendReply("9544", mn, "Dear user, Mobile Number does not exist in the system.", tid, eid)
                Return "Mobile Number does not exist in the system."
                Exit Function
            End If

            'New change 12th APR 2014
            If Trim(para(0)).ToString.Length = 11 Then
                para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
            ElseIf Trim(para(0)).ToString.Length = 10 Then
                para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(6, 2) & ":" & para(0).Substring(8, 2)
            End If

            If dt.Rows(0).Item("Ktype").ToString.ToUpper = "STATIC" Then
                'Old Code
                'oda.SelectCommand.CommandText = "select  " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & uid & " and curstatus in ('Allotted')"
                'New Change 12th APR 2014
                oda.SelectCommand.CommandText = "select  " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & uid & " and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, " & sdt & ",3)<= convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21)<=CONVERT(datetime, " & edt & ",3) and CONVERT(datetime," & edt & ",3)>=convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) >= CONVERT(datetime, " & sdt & ",3)"
                Dim dtcnt As New DataTable
                oda.Fill(dtcnt)
                If dtcnt.Rows.Count > 1 Then
                    SendReply("9804", mn, "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin"
                    Exit Function
                ElseIf dtcnt.Rows.Count = 0 Then
                    SendReply("9803", mn, "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin"
                    Exit Function
                End If
                'Old Code
                'oda.SelectCommand.CommandText = " select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & eid & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & uid & " and curstatus in ('Allotted'))"
                'New Change 12th APR 2014
                oda.SelectCommand.CommandText = "select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & eid & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & uid & " and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, " & sdt & ",3)<= convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21)<=CONVERT(datetime, " & edt & ",3) and CONVERT(datetime," & edt & ",3)>=convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) and convert(nvarchar,convert(datetime,'" & para(0).ToString() & "'),21) >= CONVERT(datetime, " & sdt & ",3))"
                ds1.Clear()
                para(0) = dtv(0)
                oda.Fill(ds1, "vemei")
                Dim vno As String
                Dim IMEINO As String
                If ds1.Tables("vemei").Rows.Count = 1 Then
                    vno = ds1.Tables("vemei").Rows(0).Item(0).ToString
                    IMEINO = ds1.Tables("vemei").Rows(0).Item(1).ToString
                ElseIf ds1.Tables("vemei").Rows.Count = 0 Then
                    SendReply("9803", mn, "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as vehicle is not allotted to you. Please contact your Admin"
                    Exit Function
                Else
                    SendReply("9804", mn, "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as more than one vehicle is allotted to you. Please contact your Admin"
                    Exit Function
                End If
                oda.SelectCommand.CommandText = "select distinct IMIENO from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "' "
                Dim dt1 As DataTable = New DataTable()
                oda.Fill(dt1)
                If dt1.Rows.Count = 0 Then
                    SendReply("9805", mn, "Dear User, your request can’t be processed as your Vehicle is not mapped with GPS Device in System. Please contact your Admin", tid, eid)
                    Return "Dear User, your request can’t be processed as your Vehicle is not mapped with GPS Device in System. Please contact your Admin"
                    Exit Function
                End If
                Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
                Dim tedate As String = ""
                If ttype.ToUpper = "ELOG" Then
                    'Change both value in date and time
                    If Trim(para(0)).ToString.Length > 11 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 11 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    ElseIf Trim(para(1)).ToString.Length > 11 Or para(1).Contains(":") Or Trim(para(0)).ToString.Length < 11 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If
                    para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                    para(1) = "20" & para(1).Substring(4, 2) & "-" & para(1).Substring(2, 2) & "-" & Left(para(1), 2) & " " & para(1).Substring(7, 2) & ":" & para(1).Substring(9, 2)

                    If IsDate(para(0)) = False Or IsDate(para(1)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select isnull(sum(devdist),0) from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and   cTime >= convert(datetime,'" & para(0) & "') AND ctime <= convert(datetime,'" & para(1) & "') group by lattitude,longitude)  "
                    Dim sum As DataTable = New DataTable()
                    oda.Fill(sum)
                    If DateTime.Parse(para(0)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    ElseIf DateTime.Parse(para(1)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    End If
                    If CDate(para(0)) > CDate(para(1)) Then
                        SendReply("9806", mn, "Dear User, your request can’t be processed as trip end datetime should be greater than trip start datetime", tid, eid)
                        Return "Dear User, your request can’t be processed as trip end datetime should be greater than trip start datetime"
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & uid & " and imei_no='" & IMEINO & "' and eid=" & eid & ")"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    tedate = oda.SelectCommand.ExecuteScalar().ToString

                    If tedate = "" Then
                        SendReply("9527", mn, "Please End previous trip before starting new trip.", tid, eid)
                        Return "Please End previous trip before starting new trip."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(1) & "' <= trip_end_Datetime and '" & para(1) & "' >= trip_start_Datetime) "
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "uspInsertSMSELOGBOOKNew"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", para(0))
                    oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", para(1))
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid)
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select lattitude,longitude,ctime,speed,devdist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and  cTime >= convert(datetime,'" & para(0) & "') AND ctime <= convert(datetime,'" & para(1) & "')  group by lattitude,longitude) order by ctime ", con)
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)
                    If dtlatlong.Rows.Count > 0 Then
                        oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString()))
                        oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", Location(dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString(), dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString()))
                    Else
                        SendReply("9807", mn, "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time", tid, eid)
                        Return "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time"
                        Exit Function
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("Total_Distance", sum.Rows(0).ItemArray(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    oda.SelectCommand.Parameters.AddWithValue("isAuth", 1)
                    oda.SelectCommand.Parameters.AddWithValue("entryby", "User")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    Return "eLog Successfully"
                    Exit Function

                    'Now check for Nil trip
                ElseIf ttype.ToUpper = "NIL TRIP USER" Then
                    If Trim(para(0)).ToString.Length > 6 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 6 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If

                    para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2)

                    If IsDate(para(0)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & uid & " and imei_no='" & IMEINO & "' and eid=" & eid & ")"

                    Dim dtt As New DataTable()
                    oda.Fill(dtt)

                    If dtt.Rows.Count > 0 Then
                        tedate = dtt.Rows(0).Item("Trip_end_datetime").ToString()
                        If tedate = "" Then
                            SendReply("9527", mn, "Dear user, Please end previous trip before starting new trip", tid, eid)
                            Return "Please End previous trip before starting new trip."
                            con.Dispose()
                            Exit Function
                        End If
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) "
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "uspInsertSMSELOGBOOKTripStrtNil"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid.ToString())
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", para(0) & " 00:00")
                    oda.SelectCommand.Parameters.AddWithValue("Trip_End_DateTime", para(0) & " 23:59")
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    oda.SelectCommand.Parameters.AddWithValue("isAuth", 1)
                    oda.SelectCommand.Parameters.AddWithValue("entryby", "User")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    'pending MSG ID
                    SendReply("10694", mn, "Dear User, Your nil trip has been registered for " & para(0) & " successfully.", tid, eid)
                    Return "Nil Trip Created Successfully"
                    Exit Function
                ElseIf ttype.ToUpper = "TRIP START" Then
                    If Trim(para(0)).ToString.Length > 11 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 10 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If
                    If Trim(para(0)).ToString.Length = 11 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                    ElseIf Trim(para(0)).ToString.Length = 10 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(6, 2) & ":" & para(0).Substring(8, 2)
                    End If
                    If IsDate(para(0)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If
                    If DateTime.Parse(para(0)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & uid & " and imei_no='" & IMEINO & "' and eid=" & eid & ")"

                    Dim dtt As New DataTable()
                    oda.Fill(dtt)

                    If dtt.Rows.Count > 0 Then
                        tedate = dtt.Rows(0).Item("Trip_end_datetime").ToString()
                        If tedate = "" Then
                            SendReply("9527", mn, "Dear user, Please end previous trip before starting new trip", tid, eid)
                            Return "Please End previous trip before starting new trip."
                            con.Dispose()
                            Exit Function
                        End If
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime) "
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "uspInsertSMSELOGBOOKTripStrtNew"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", para(0))
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid.ToString())
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select top 1 lattitude,longitude,ctime,speed,devdist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and   convert(nvarchar(16),cTime,121)  >= convert(nvarchar,'" & para(0) & "',120) group by lattitude,longitude) order by ctime ", con)
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)
                    Dim tripstartLocation As String = String.Empty
                    If dtlatlong.Rows.Count > 0 Then
                        tripstartLocation = Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", tripstartLocation)
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    oda.SelectCommand.Parameters.AddWithValue("isAuth", 1)
                    oda.SelectCommand.Parameters.AddWithValue("entryby", "User")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    SendReply("9538", mn, "Dear User, Your trip started at " & para(0).ToString() & " from " & tripstartLocation & " successfully", tid, eid)
                    Return "Trip Start Successfully"
                    Exit Function
                ElseIf ttype.ToUpper = "TRIP END" Then
                    If Trim(para(0)).ToString.Length > 11 Or para(0).Contains(":") Or Trim(para(0)).ToString.Length < 10 Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid format/data"
                        Exit Function
                    End If
                    If Trim(para(0)).ToString.Length = 11 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(7, 2) & ":" & para(0).Substring(9, 2)
                    ElseIf Trim(para(0)).ToString.Length = 10 Then
                        para(0) = "20" & para(0).Substring(4, 2) & "-" & para(0).Substring(2, 2) & "-" & Left(para(0), 2) & " " & para(0).Substring(6, 2) & ":" & para(0).Substring(8, 2)
                    End If
                    If IsDate(para(0)) = False Then
                        SendReply("9545", mn, "Dear user,Please enter valid Date/Time", tid, eid)
                        Return "Please enter valid Date/Time"
                        Exit Function
                    End If
                    If DateTime.Parse(para(0)) > DateTime.Parse(crrdate) Then
                        SendReply("9409", mn, "Dear user, future date/time is not allowed in log book entry", tid, eid)
                        Return "ERROR"
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select trip_start_datetime,trip_end_datetime from mmm_mst_elogbook where imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and tid=(select max(tid) from mmm_mst_elogbook where imei_no='" & IMEINO & "' and vehicle_no='" & vno & "')"
                    Dim dttt As New DataTable()
                    oda.Fill(dttt)

                    If dttt.Rows.Count = 0 Then
                        SendReply("9541", mn, "Dear User, You don't have any open trip", tid, eid)
                        Return "Dear User, You don't have any open trip"
                        Exit Function
                    End If

                    If dttt.Rows(0).Item("trip_end_datetime").ToString.Length() > 0 Then
                        SendReply("9542", mn, "Dear User, You already closed your last trip", tid, eid)
                        Return "Dear User, You already closed your last trip"
                        Exit Function
                    End If
                    Dim stdt As String = dttt.Rows(0).Item("trip_start_datetime").ToString

                    If CDate(stdt) > CDate(para(0)) Then
                        SendReply("9540", mn, "Dear User, Trip End Date should be greater than Trip Start Date", tid, eid)
                        Return "Dear User, Trip End Date should be greater than Trip Start Date"
                        Exit Function
                    End If
                    oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & stdt & "' <= trip_end_Datetime and '" & stdt & "' >= trip_start_Datetime) or (imei_no='" & IMEINO & "' and vehicle_no='" & vno & "' and  '" & para(0) & "' <= trip_end_Datetime and '" & para(0) & "' >= trip_start_Datetime)"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        SendReply("9525", mn, "Trip already exist at this period.", tid, eid)
                        Return "Trip already exist at this period."
                        con.Dispose()
                        Exit Function
                    End If

                    oda.SelectCommand.CommandText = "select isnull(sum(devdist),0) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and   cTime >= convert(datetime,'" & stdt & "') AND ctime <= convert(datetime,'" & para(0) & "') "
                    Dim sum As DataTable = New DataTable()
                    oda.Fill(sum)

                    oda.SelectCommand.CommandText = "uspUpdateSMSELOGBOOKTripEnd"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", para(0))
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vno)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid.ToString())
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select  top 1 lattitude,longitude,ctime,speed,devdist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "'  and  convert(nvarchar(16),cTime,121)  >= convert(nvarchar,'" & para(0) & "',120) group by lattitude,longitude) order by ctime ", con)
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)

                    Dim tripsendLocation As String = String.Empty

                    If dtlatlong.Rows.Count > 0 Then
                        'oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString()))
                        tripsendLocation = Location(dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString(), dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", tripsendLocation)
                    Else
                        SendReply("9807", mn, "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time", tid, eid)
                        Return "Dear User, your request can’t be processed as GPS data is not uploaded in the System till now. Please retry after some time"
                        Exit Function
                    End If

                    oda.SelectCommand.Parameters.AddWithValue("Total_Distance", sum.Rows(0).ItemArray(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", IMEINO)
                    oda.SelectCommand.Parameters.AddWithValue("uid", uid)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    con.Dispose()
                    SendReply("9539", mn, "Dear User, Your trip end at " & para(0).ToString() & " from " & tripsendLocation & " Trip length : " & sum.Rows(0).ItemArray(0).ToString() & " successfully", tid, eid)
                    Return "Trip End Successfully"
                    Exit Function
                End If
            Else

                'Athorization process
                da.SelectCommand.CommandText = "select count(*) from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Authentication' "
                Dim ds As New DataSet
                Dim cnt As Integer
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                cnt = da.SelectCommand.ExecuteScalar()
                If cnt > 0 Then
                    da.SelectCommand.CommandText = "select * from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Authentication' "
                    da.Fill(ds, "Auth")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    For i As Integer = 0 To ds.Tables("Auth").Rows.Count - 1
                        If ds.Tables("Auth").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                            If ds.Tables("Auth").Rows(i).Item("paratype").ToString.ToUpper = "STATIC" Then
                                da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & " like  '%" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString & "%'"
                                cnt = da.SelectCommand.ExecuteScalar()
                                If cnt = 0 Then
                                    SendReply("10887", mn, "Dear User, Your Authentication is failed", tid, eid)
                                    Return "Dear User, Your Authentication is failed"
                                    con.Dispose()
                                    Exit Function
                                End If
                            Else
                                If ds.Tables("Auth").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                                    da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & " like '%" & mn & "%'"
                                    cnt = da.SelectCommand.ExecuteScalar()
                                    If cnt = 0 Then
                                        SendReply("10887", mn, "Dear User, Your Authentication is failed", tid, eid)
                                        Return "Dear User, Your Authentication is failed"
                                        con.Dispose()
                                        Exit Function
                                    End If
                                Else
                                    da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & " like '%" & para("" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString) & "%'"
                                    cnt = da.SelectCommand.ExecuteScalar()
                                    If cnt = 0 Then
                                        SendReply("10888", mn, "Dear User, Data does not exist in the system", tid, eid)
                                        Return "Dear User, Data does not exist in the system"
                                        con.Dispose()
                                        Exit Function
                                    End If
                                End If
                            End If
                        Else
                            If ds.Tables("Auth").Rows(i).Item("paratype").ToString.ToUpper = "STATIC" Then
                                da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Auth").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & " like '%" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString & "%'"
                                cnt = da.SelectCommand.ExecuteScalar()
                                If cnt = 0 Then
                                    SendReply("10887", mn, "Dear User, Your Authentication is failed", tid, eid)
                                    Return "Dear User, Your Authentication is failed"
                                    con.Dispose()
                                    Exit Function
                                End If
                            Else
                                If ds.Tables("Auth").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                                    da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Auth").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & " like '%" & mn & "%'"
                                    cnt = da.SelectCommand.ExecuteScalar()
                                    If cnt = 0 Then
                                        SendReply("10887", mn, "Dear User, Your Authentication is failed", tid, eid)
                                        Return "Dear User, Your Authentication is failed"
                                        con.Dispose()
                                        Exit Function
                                    End If
                                Else
                                    da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Auth").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & " like '%" & para("" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString) & "%'"
                                    cnt = da.SelectCommand.ExecuteScalar()
                                    If cnt = 0 Then
                                        SendReply("10888", mn, "Dear User, Data does not exist in the system", tid, eid)
                                        Return "Dear User, Data does not exist in the system"
                                        con.Dispose()
                                        Exit Function
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If

                'Existence process
                Dim dtype As String = ""
                da.SelectCommand.CommandText = "select count(*) from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Existence' "
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                cnt = da.SelectCommand.ExecuteScalar()
                If cnt > 0 Then
                    da.SelectCommand.CommandText = "select * from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Existence' "
                    da.Fill(ds, "Exist")
                    For i As Integer = 0 To ds.Tables("Exist").Rows.Count - 1
                        If ds.Tables("Exist").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                            da.SelectCommand.CommandText = "select datatype from mmm_mst_fields  where documenttype='" & ds.Tables("Exist").Rows(i).Item("Documenttype").ToString & "' and  fieldmapping='" & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & "'"
                            dtype = da.SelectCommand.ExecuteScalar().ToString
                            If dtype.ToUpper = "NUMERIC" Then
                                If Not IsNumeric(para("" & ds.Tables("Exist").Rows(i).Item("paravalue").ToString)) Then
                                    SendReply("10918", mn, "Dear User, Please enter valid data", tid, eid)
                                    Return "Dear User, Please enter valid data"
                                    con.Dispose()
                                    Exit Function
                                End If
                            End If
                            If ds.Tables("Exist").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                                da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & " like '%" & mn & "%'"
                            Else
                                da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & " like  '%" & para("" & ds.Tables("Exist").Rows(i).Item("paravalue").ToString) & "%'"
                            End If
                        Else
                            da.SelectCommand.CommandText = "select datatype from mmm_mst_fields  where documenttype='" & ds.Tables("Exist").Rows(i).Item("Documenttype").ToString & "' and  fieldmapping='" & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & "'"
                            dtype = da.SelectCommand.ExecuteScalar().ToString
                            If dtype.ToUpper = "NUMERIC" Then
                                If Not IsNumeric(para("" & ds.Tables("Exist").Rows(i).Item("paravalue").ToString)) Then
                                    SendReply("10918", mn, "Dear User, Please enter valid data", tid, eid)
                                    Return "Dear User, Please enter valid data"
                                    con.Dispose()
                                    Exit Function
                                End If
                            End If
                            If ds.Tables("Exist").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                                da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Exist").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & " like '%" & mn & "%'"
                            Else
                                da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Exist").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & " like  '%" & para("" & ds.Tables("Exist").Rows(i).Item("paravalue").ToString) & "%'"
                            End If
                        End If
                        cnt = da.SelectCommand.ExecuteScalar()
                        If cnt = 1 Then
                            SendReply("10917", mn, "Dear User, Data already exist in the system please remove previous data", tid, eid)
                            Return "Dear User, Data already exist in the system please remove previous data"
                            con.Dispose()
                            Exit Function
                        End If
                    Next
                End If

                'Processing
                da.SelectCommand.CommandText = "select count(*) from mmm_sms_settings where keyword='" & keywd & "' and settingtype in ('Processing','Where') "
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                cnt = da.SelectCommand.ExecuteScalar()
                Dim insfld As String = ""
                Dim insval As String = ""
                Dim Updateval As String = ""
                Dim cnd As String = ""
                If cnt > 0 Then
                    da.SelectCommand.CommandText = "select * from mmm_sms_settings where keyword='" & keywd & "' and settingtype in ('Processing','Where') ORDER BY SETTINGTYPE "
                    da.Fill(ds, "Process")
                    For j As Integer = 0 To ds.Tables("Process").Rows.Count - 1
                        If ds.Tables("Process").Rows(j).Item("ProcType").ToString.ToUpper = "INSERT" Then
                            insfld = insfld & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & ","
                            If ds.Tables("Process").Rows(j).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                                insval = insval & "'" & mn & "'" & ","
                            ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                                insval = insval & "'" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                            Else
                                insval = insval & "'" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "'" & ","
                            End If
                        ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "APPEND" Then
                            If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= " & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & " + '," & mn & "'" & ","
                            ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= " & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & " + '," & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                            Else
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= " & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & " + '," & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "'" & ","
                            End If
                        ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "=" Then
                            If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & mn & "'" & ","
                            ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                            Else
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "'" & ","
                                ' Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & para(j) & "'" & ","
                            End If
                        ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "REMOVE" Then
                            If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & mn & "'" & ","
                            ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                            Else
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= replace(" & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & ",'" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "','')" & ","
                            End If
                        ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "REMOVE ALL" Then
                            If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '',"
                            ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '',"
                            Else
                                Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '',"
                            End If
                        ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "WHERE" Then
                            If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                                cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & mn & "' and "
                            ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                                cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "' and "
                            Else
                                cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "' and "
                                ' cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & para(j) & "' and "
                            End If
                        End If
                    Next

                    If Updateval.ToString <> "" Then
                        Updateval = Left(Updateval, Updateval.Length - 1)
                    End If
                    If cnd.ToString <> "" Then
                        cnd = Left(cnd, cnd.Length - 4)
                    End If

                    For i As Integer = ds.Tables("Process").Rows.Count - 1 To ds.Tables("Process").Rows.Count - 1
                        If ds.Tables("Process").Rows(i).Item("ProcType").ToString.ToUpper = "UPDATE" And ds.Tables("Process").Rows(i).Item("SettingType").ToString.ToUpper = "WHERE" Then
                            If ds.Tables("Process").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                                da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & "  " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " set  " & Updateval & " where " & cnd
                            Else
                                da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & "  " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " set  " & Updateval & " where " & cnd & "  and   documenttype='" & ds.Tables("process").Rows(i).Item("Documenttype").ToString & "'"
                            End If
                        ElseIf ds.Tables("Process").Rows(i).Item("ProcType").ToString.ToUpper = "INSERT" Then
                            If insfld.Length > 1 And insval.Length > 1 Then
                                insfld = Left(insfld, insfld.Length - 1)
                                insval = Left(insval, insval.Length - 1)
                            End If
                            If ds.Tables("Process").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                                da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & " into  " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " (" & insfld & ") values (" & insval & ")"
                            ElseIf ds.Tables("Process").Rows(i).Item("TableName").ToString.ToUpper = "M_MST_MASTER" Then
                                da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & " into " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " ( " & insfld & ",documenttype,eid,createdby,updateddate ) values (" & insval & ",'" & ds.Tables("process").Rows(i).Item("Documenttype").ToString & "'," & eid & "," & uid & ",getdate())"
                            Else
                            End If
                        End If
                    Next
                    Try
                        da.SelectCommand.ExecuteNonQuery()
                        'SendReply("8757", mn, "Dear User, command executed successfully",tid,eid)
                        SendReply("10915", mn, "Dear User, Details have been updated", tid, eid)
                        Return "Dear User, Details have been updated"
                        Exit Function
                    Catch ex As Exception
                        SendReply("10916", mn, "Dear User, Details have not been updated", tid, eid)
                        Return "Dear User, Details have not been updated"
                        con.Dispose()
                        Exit Function
                    End Try
                End If
            End If
        Catch ex As Exception
        Finally
            con.Dispose()
        End Try
        Return "Done"
    End Function

    Public Function CheckMyndApp(ByVal mn As String, ByVal msg As String) As String
        Dim keywd As String = Left(msg, msg.IndexOf(" "))
        msg = Trim(msg)
        Dim Para As String = Right(msg, 3)
        Para = Trim(Para)
        msg = msg.Replace(keywd, "").Trim()
        Dim constr As String = ""

        If keywd.ToUpper = "MYNDSAAS" Then
            constr = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Gtr$698B#"
        ElseIf keywd.ToUpper = "MYNDBPM" Then
            constr = "server=172.17.109.152;initial catalog=PREDMS;uid=DMS;pwd=Ztsu93#u"
        End If
        Dim con As New SqlConnection(constr)
        Try
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            If keywd.ToUpper <> "MYNDSAAS" And keywd.ToUpper <> "MYNDBPM" Then
                'Invalid keyword message
                SendReply1("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.")
                Return "Done"
                Exit Function
            End If
            If Para.ToUpper = "ON" Then
                oda.SelectCommand.CommandText = "update mmm_mst_entity set pvisit=4568 where code='mynd'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                SendReply1("8757", mn, "Dear User, command executed successfully")
                Return "Done"
                Exit Function
            ElseIf Para.ToUpper = "OFF" Then
                Dim rnd = New Random()
                Dim i As Integer = rnd.Next(10000)
                oda.SelectCommand.CommandText = "update mmm_mst_entity set pvisit=" & i & " where code='mynd'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
                SendReply1("8757", mn, "Dear User, command executed successfully")
                Return "Done"
                Exit Function
            Else
                SendReply1("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.")
                Return "Done"
                Exit Function
            End If
        Catch ex As Exception
        Finally
            con.Dispose()
        End Try
        Return "Done"
    End Function

    Private Sub SendReply1(templateID As String, MobileNumber As String, ByVal msg As String)
        Dim msgString As String = "http://smsalertbox.com/api/sms.php?uid=6d796e646270&pin=51f36abe9f80a&sender=MYNDBP&route=5&tempid=" & templateID & "&mobile=" & MobileNumber & "&message=" & msg & "&pushid=1"
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        ' Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSLogUpdate", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May=2014
        Try
            Dim result As String = apicall(msgString)
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Sub

    Private Sub SendReply(templateID As String, MobileNumber As String, ByVal msg As String, tid As Integer, Optional ByVal eid As Integer = 0)
        Dim msgString As String = "http://smsalertbox.com/api/sms.php?uid=6d796e646270&pin=51f36abe9f80a&sender=MYNDBP&route=5&tempid=" & templateID & "&mobile=" & MobileNumber & "&message=" & msg & "&pushid=1"
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSLogUpdate", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May=2014
        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("tid", tid)
            oda.SelectCommand.Parameters.AddWithValue("msg", msg)
            oda.SelectCommand.Parameters.AddWithValue("eid", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            con.Close()
            oda.Dispose()
            con.Dispose()
            Dim result As String = apicall(msgString)
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub

    Public Shared Function InsertSMSLog(ByVal sendorNumer As String, ByVal keyword As String, ByVal msgtext As String) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSLogNew", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May=2014
        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("SendorNumber", sendorNumer)
            oda.SelectCommand.Parameters.AddWithValue("Keyword", keyword)
            oda.SelectCommand.Parameters.AddWithValue("MsgText", msgtext)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()

            Return iSt
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function

    Public Function Location(lat As String, log As String) As String
        'Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(constr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        ''try Catch Block Added by Ajeet Kumar :Date::22 May=2014
        'Try
        '    oda.SelectCommand.CommandText = "select top 1 * from gpsLocation where Lat_start  <=" + lat + " and  lat_end >= " + lat + " and long_start <= " + log + " and long_end >= " + log + " "
        '    Dim locatoinr As DataTable = New DataTable()
        '    oda.Fill(locatoinr)
        '    If locatoinr.Rows.Count > 0 Then
        '        Return locatoinr.Rows(0).Item(1).ToString
        '    Else

        '        Dim url As String = "http://maps.googleapis.com/maps/api/geocode/xml?latlng=" & lat & "," & log & "&sensor=false"
        '        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        '        Dim response As WebResponse = request.GetResponse()
        '        Dim dataStream As Stream = response.GetResponseStream()
        '        Dim sreader As New StreamReader(dataStream)
        '        Dim responsereader As String = sreader.ReadToEnd()
        '        response.Close()
        '        Dim xmldoc As New XmlDocument()
        '        xmldoc.LoadXml(responsereader)

        '        If xmldoc.GetElementsByTagName("status")(0).ChildNodes(0).InnerText = "OK" Then

        '            oda.SelectCommand.CommandText = "gpsinsertlocation"
        '            oda.SelectCommand.CommandType = CommandType.StoredProcedure
        '            oda.SelectCommand.Parameters.AddWithValue("complete_latitude", lat)
        '            oda.SelectCommand.Parameters.AddWithValue("complete_longitude", log)
        '            oda.SelectCommand.Parameters.AddWithValue("Lat_start", Convert.ToDouble(lat.Substring(0, 5)))
        '            oda.SelectCommand.Parameters.AddWithValue("lat_end", Convert.ToDouble(lat.Substring(0, 5)) + 0.01)
        '            oda.SelectCommand.Parameters.AddWithValue("long_start", Convert.ToDouble(log.Substring(0, 5)))
        '            oda.SelectCommand.Parameters.AddWithValue("long_end", Convert.ToDouble(log.Substring(0, 5)) + 0.01)
        '            Dim fulladdress As String = String.Empty
        '            Try
        '                If xmldoc.ChildNodes.Count > 0 Then
        '                    Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
        '                    Dim Cnt As Integer = 0
        '                    Dim nodes As XmlNodeList = xmldoc.SelectNodes(SelNodesTxt)

        '                    Dim other As Int32 = 0
        '                    For Each node As XmlNode In nodes
        '                        For c As Integer = 0 To node.ChildNodes.Count - 1
        '                            If node.ChildNodes(c).Name = "result" Then
        '                                Cnt += 1
        '                                For c2 As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Count - 1
        '                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "address_component" Then
        '                                        For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
        '                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count = 2 And other = 0 Then
        '                                                oda.SelectCommand.Parameters.AddWithValue("other", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerText)
        '                                                other = 1
        '                                            End If
        '                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "type" Then
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_address" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("street_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If

        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "floor" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("floor", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "parking" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("parking", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "post_box" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("post_box", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_town" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("postal_town", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "room" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("room", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "train_station" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("train_station", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "establishment" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("establishment_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_number" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("street_number", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "bus_station" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("stationaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "route" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("rld", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "neighborhood" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("npa", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)  ''neighborhood address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "sublocality" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("sublocalityaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "locality" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("locPaddre", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''locality
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_3" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("admini3address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_2" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("adminpoladdress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_1" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("addressLongName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
        '                                                    oda.SelectCommand.Parameters.AddWithValue("addShortName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "country" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("countryLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
        '                                                    oda.SelectCommand.Parameters.AddWithValue("countryShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_code" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("postalLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
        '                                                    oda.SelectCommand.Parameters.AddWithValue("postalShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "airport" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("airport", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "point_of_interest" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("point_of_interest", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "park" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("park", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "intersection" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("intersection", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "colloquial_area" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("colloquial_area", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "premise" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("premise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "subpremise" Then
        '                                                    oda.SelectCommand.Parameters.AddWithValue("subpremise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
        '                                                End If
        '                                            End If
        '                                        Next
        '                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "formatted_address" Then
        '                                        fulladdress = node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText
        '                                        oda.SelectCommand.Parameters.AddWithValue("location_namer", node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText)
        '                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "geometry" Then
        '                                        For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
        '                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "location" Then
        '                                                For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
        '                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lat" Then
        '                                                        oda.SelectCommand.Parameters.AddWithValue("geometrylat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
        '                                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lng" Then
        '                                                        oda.SelectCommand.Parameters.AddWithValue("geometrylng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
        '                                                    End If
        '                                                Next
        '                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "viewport" Then
        '                                                For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
        '                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
        '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
        '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("vSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("vSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            End If
        '                                                        Next
        '                                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
        '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
        '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("vNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("vNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            End If
        '                                                        Next
        '                                                    End If
        '                                                Next
        '                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "bounds" Then
        '                                                For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
        '                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
        '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
        '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("bSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("bSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            End If
        '                                                        Next
        '                                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
        '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
        '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("bNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
        '                                                                oda.SelectCommand.Parameters.AddWithValue("bNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
        '                                                            End If
        '                                                        Next
        '                                                    End If
        '                                                Next
        '                                            End If
        '                                        Next
        '                                    End If
        '                                Next
        '                            End If
        '                            If Cnt = 1 Then
        '                                Exit For
        '                            End If
        '                        Next
        '                    Next
        '                End If
        '                If con.State <> ConnectionState.Open Then
        '                    con.Open()
        '                End If
        '                oda.SelectCommand.ExecuteNonQuery()
        '                locatoinr.Dispose()

        '            Catch ex As Exception
        '            End Try
        '            Return fulladdress
        '        End If
        '    End If
        '    Return Nothing
        'Catch ex As Exception
        '    Throw
        'Finally
        '    If Not oda Is Nothing Then
        '        con.Dispose()
        '    End If
        '    If Not oda Is Nothing Then
        '        oda.Dispose()
        '    End If
        'End Try
    End Function

    Public Function PushGPSData(imieno As String, lat As Double, longitude As Double, altitude As Double, speed As Double, angle As Integer, cTime As Date, nosat As Integer, IsGPS As Integer, IsGPRS As Integer, distance As Double) As String Implements IDMSService.PushGPSData

        'Dim imeinoexist As String = "353324064221861,353324064233866,353324063422288,353324064824284,353324064897843,353324064822809,353324063417007,353324064233783,353324063410804,353324063429143,353324064212407,353324063394081," &
        '    "353324063727306,353324064530949,353324063420225,353324064538868,353324064155861,353324064827303,353324063422403,353324064917963,353324064212068,353324063421926,353324063421967,353324063734401," &
        '    "353324063409285,353324063408287,353324064233445,353324064150649,353324064535427,353324063775586,353324064825265,353324063424987,353324064897009,353324063399544,353324063956822,353324064220723," &
        '    "353324064151944,353324064917823,353324063734500,353324064232827,353324063950403,353324064493742,353324063465121,353324063423948,353324063776006,353324064841742,353324063775982,353324064823963," &
        '    "353324064154021,353324064234344,353324063954124,353324063733783,353324064210542,353324064217422,353324063734369,353324063400649,353324064156307,353324064852026,353324064537365,353324064152728,353324064156562,353324064822668," &
        '    "353324063465287,353324063732942,353324064157164,353324064212845,"

        'If (imeinoexist.Contains(imieno + ",")) Then

        '    Dim webserviceurl As String = "http://bpm.sequelone.com/DMSService.svc/PushGPSData?imieno=" & imieno & "&lat=" & lat & "&longitude=" & longitude & "&altitude=" & altitude & "&speed=" & speed & "&angle=" & angle & "&cTime=" & cTime & "&nosat=" & nosat & "&IsGPS=" & IsGPS & "&IsGPRS=" & IsGPRS & "&distance=" & distance & ""
        '    Dim request As HttpWebRequest = HttpWebRequest.Create(webserviceurl)
        '    Try
        '        Dim oResponse As HttpWebResponse = request.GetResponse()
        '    Catch ex As Exception
        '        Return "ERROR"
        '    End Try

        '    Return "SUCCESS"
        '    Exit Function
        'End If


        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertGPSDataPhone", con)
        'Try
        '    'Make log of each connection to a file
        '    'Dim file As New System.IO.StreamWriter(HttpContext.Current.Server.MapPath("~/Docs") & "/DeviceLog.txt", True)
        '    'file.WriteLine(Now.Date.Date & "," & imieno & "," & lat & "," & longitude & "," & cTime)
        '    'file.Close()

        '    If lat = 0 Or longitude = 0 Then
        '        Return "SUCCESS"
        '    End If
        '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
        '    oda.SelectCommand.Parameters.AddWithValue("IMIENO", imieno)
        '    oda.SelectCommand.Parameters.AddWithValue("lattitude", lat)
        '    oda.SelectCommand.Parameters.AddWithValue("longitude", longitude)
        '    oda.SelectCommand.Parameters.AddWithValue("altitude", altitude)
        '    oda.SelectCommand.Parameters.AddWithValue("speed", speed)
        '    oda.SelectCommand.Parameters.AddWithValue("angle", angle)
        '    oda.SelectCommand.Parameters.AddWithValue("ctime", cTime)
        '    oda.SelectCommand.Parameters.AddWithValue("noofsat", nosat)
        '    oda.SelectCommand.Parameters.AddWithValue("DevType", "PHONE")
        '    oda.SelectCommand.Parameters.AddWithValue("DevDist", distance)
        '    oda.SelectCommand.Parameters.AddWithValue("IsGPS", IsGPS)
        '    oda.SelectCommand.Parameters.AddWithValue("IsGPRS", IsGPRS)

        '    If con.State <> ConnectionState.Open Then
        '        con.Open()
        '    End If

        '    Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()

        '    If iSt = 0 Then
        '        Return "SUCCESS"
        '    Else
        '        Return "ERROR"
        '    End If

        'Catch ex As Exception
        '    Return ex.Message
        'Finally
        '    If Not con Is Nothing Then
        '        con.Close()
        '        con.Dispose()
        '    End If
        '    If Not oda Is Nothing Then
        '        oda.Dispose()
        '    End If
        'End Try
    End Function
    Public Function InsertGPSTransaction(Apikey As String, UserID As String, SiteID As String, lattitude As String, longitude As String, SubmitTime As String) As String Implements IDMSService.InsertGPSTransaction
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim IsValid As Boolean = True
        Dim DS As New DataSet()
        Dim obj As New MyndBPMWS()
        DS = obj.AuthenticateWSRequest(Apikey)
        Dim cTime As Date
        If DS.Tables(0).Rows.Count > 0 Then
            SubmitTime = HttpUtility.UrlDecode(SubmitTime)
            If (String.IsNullOrEmpty(UserID.Trim)) Then
                Return "UserID required"
            End If
            If (String.IsNullOrEmpty(SiteID.Trim)) Then
                Return "SiteID required"
            End If
            If (String.IsNullOrEmpty(lattitude)) Then
                Return "Lattitude required"
            End If
            If Not (IsNumeric(lattitude)) Then
                Return "Lattitude must be numeric"
            End If
            If (String.IsNullOrEmpty(longitude)) Then
                Return "Longitude required"
            End If
            If Not (IsNumeric(longitude)) Then
                Return "Longitude must be numeric"
            End If
            If (String.IsNullOrEmpty(SubmitTime)) Then
                Return "SubmitTime required"
            End If
            Dim arr = SubmitTime.Split(" ")
            Try
                cTime = GetFDate(arr(0))
            Catch ex As Exception
                Return "Invalid SubmitTime"
            End Try

            If Not (IsDate(cTime)) Then
                Return "Invalid SubmitTime"
            End If
            SubmitTime = cTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) & " " & arr(1)
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertGPSTransaction", con)
            Try
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@IMIENO", UserID)
                oda.SelectCommand.Parameters.AddWithValue("@lattitude", lattitude)
                oda.SelectCommand.Parameters.AddWithValue("@longitude", longitude)
                oda.SelectCommand.Parameters.AddWithValue("@ctime", SubmitTime)
                oda.SelectCommand.Parameters.AddWithValue("@SiteID", SiteID)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
                If iSt > 0 Then
                    Return "SUCCESS"
                Else
                    Return "Something went wrong"
                End If

            Catch ex As Exception
                Return "Something went wrong"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If
            End Try
        Else
            Return "Invalid key supplyed"
        End If

    End Function

    Public Shared Function GetFDate(strDate As String) As Date
        Dim ret As Date
        Try
            'Here we are assuming seperator may be /,\,-,.
            Dim arr = strDate.Split("/", "\", "-", ".")
            Dim Month As String = "0"
            Try
                'Checking if Month Name is string
                If Not IsNumeric(arr(1)) Then
                    'dim Months thisMonth
                    If arr(1).ToUpper = "JAN" Or arr(1).ToUpper = "JANUARY" Then
                        Month = "01"
                    ElseIf arr(1).ToUpper = "FEB" Or arr(1).ToUpper = "FEBRUARY" Then
                        Month = "02"
                    ElseIf arr(1).ToUpper = "MAR" Or arr(1).ToUpper = "MARCH" Then
                        Month = "03"
                    ElseIf arr(1).ToUpper = "APR" Or arr(1).ToUpper = "APRIL" Then
                        Month = "04"
                    ElseIf arr(1).ToUpper = "MAY" Then
                        Month = "05"
                    ElseIf arr(1).ToUpper = "JUNE" Or arr(1).ToUpper = "JUN" Then
                        Month = "06"
                    ElseIf arr(1).ToUpper = "JULY" Or arr(1).ToUpper = "JUL" Then
                        Month = "07"
                    ElseIf arr(1).ToUpper = "AUG" Or arr(1).ToUpper = "AUGUST" Then
                        Month = "08"
                    ElseIf arr(1).ToUpper = "SEPT" Or arr(1).ToUpper = "SEPTEMBER" Then
                        Month = "09"
                    ElseIf arr(1).ToUpper = "OCT" Or arr(1).ToUpper = "OCTOBER" Then
                        Month = "10"
                    ElseIf arr(1).ToUpper = "NOV" Or arr(1).ToUpper = "NOVEMBER" Then
                        Month = "11"
                    ElseIf arr(1).ToUpper = "DEC" Or arr(1).ToUpper = "DECEMBER" Then
                        Month = "12"
                    End If
                Else
                    Month = arr(1)
                End If
            Catch ex As Exception
                Throw
            End Try
            ret = New Date(arr(0), Month, arr(2))
            If Not IsDate(ret) Then
                Throw New Exception()
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    Public Function GetGPSPoint(imieno As String, sTime As Date, eTime As Date) As String Implements IDMSService.GetGPSPoint
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select lattitude,longitude from MMM_MST_GPSDATA WHERE IMIENO='" & imieno & "' and ctime between '" & sTime & "' and '" & eTime & "' order by ctime", con)
        Try

            Dim ds As New DataSet
            oda.Fill(ds, "GpsData")
            Dim retStr As New StringBuilder()
            For i As Integer = 0 To ds.Tables("GpsData").Rows.Count - 1
                retStr.Append(ds.Tables("GpsData").Rows(i).Item(0).ToString() & "," & ds.Tables("GpsData").Rows(i).Item(1).ToString() & ":")
            Next
            Return Left(retStr.ToString(), retStr.ToString().Length - 1)
        Catch ex As Exception
            Return ex.Message
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function

    Public Function GetNearestDevice(EID As Integer, imieno As String, lattitude As Double, longitutde As Double, stype As String) As String Implements IDMSService.GetNearestDevice
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("GetAllActiveDeviceList", con)
        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("eid", EID)
            Dim ds As New DataSet
            oda.Fill(ds, "data")

            Dim retStr As New StringBuilder()

            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim DLat As String = ds.Tables("data").Rows(i).Item("lattitude").ToString()
                Dim DLong As String = ds.Tables("data").Rows(i).Item("longitude").ToString()
                If calculate(lattitude, longitutde, DLat, DLong) <= 50 Then
                    retStr.Append(DLat & "," & DLong & "," & ds.Tables("data").Rows(i).Item("imieno").ToString() & ":")
                End If
            Next

            If ds.Tables("data").Rows.Count < 1 Then
                Return "No Detail Found"
            Else
                Return Left(retStr.ToString(), retStr.ToString().Length - 1)
            End If
        Catch ex As Exception
            Return ex.Message
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Function


    Public Function GetDeviceDetail(EID As Integer, imieno As String, stype As String) As String Implements IDMSService.GetDeviceDetail
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("GetDeviceRateCard", con)
        Try
            ' style variable is not 
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("eid", EID)
            oda.SelectCommand.Parameters.AddWithValue("imieno", imieno)
            Dim ds As New DataSet
            oda.Fill(ds, "data")

            Dim retStr As New StringBuilder()

            Dim VehType, VendorName As String
            VehType = ds.Tables("data").Rows(ds.Tables("data").Rows.Count - 1).Item("Rate Type").ToString()
            VendorName = ds.Tables("data").Rows(ds.Tables("data").Rows.Count - 1).Item("Rate").ToString()

            retStr.Append(VehType & ":" & VendorName & ":")

            For i As Integer = 0 To ds.Tables("data").Rows.Count - 2
                Dim DLat As String = ds.Tables("data").Rows(i).Item("Rate Type").ToString()
                Dim DLong As String = ds.Tables("data").Rows(i).Item("Rate").ToString()
                retStr.Append(DLat & " - " & DLong & ":")
            Next


            If ds.Tables("data").Rows.Count < 1 Then
                Return "No Detail Found"
            Else
                Return Left(retStr.ToString(), retStr.ToString().Length - 1)
            End If
        Catch ex As Exception
            Return ex.Message
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Function

    Public Function BookCab(eid As Integer, imieno As String, ratetype As String, pickup As String, drop As String, pickuptime As String, usermobile As String) As String Implements IDMSService.BookCab

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        'This procedure will return From User Table Vendor Name, Vendor Mobile number and Vendor email Addresss

        Dim oda As SqlDataAdapter = New SqlDataAdapter("GetDeviceOwner", con)
        Try
            'This service will send a mail to vendor and one SMS to vendor about booking
            'This service will also send one SMS to customer about there booking request
            'Style variable is not 

            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("eid", eid)
            oda.SelectCommand.Parameters.AddWithValue("imieno", imieno)
            Dim ds As New DataSet
            oda.Fill(ds, "data")


            Dim VendorName As String = ds.Tables("data").Rows(0).Item("UserName").ToString()
            Dim eMailID As String = ds.Tables("data").Rows(0).Item("emailid").ToString()
            Dim MobileNo As String = ds.Tables("data").Rows(0).Item("mobileno").ToString()

            '''' now send email to vendor for booking confirmation

            Dim mbody As String = "Dear <b>" & VendorName & ",</b><br/><br/> Cab booking request had been submitted to you, Please find details of booking </b><br/> "
            mbody = mbody & "Rate Type  " & ratetype & "<br/> PickUp Address : " & pickup
            mbody = mbody & "Drop Address  " & drop & "<br/> PickUp Time : " & pickuptime
            mbody = mbody & "Customer Contact Number :  " & usermobile & "<br /><br /><br /><br /> Thanks and Regards"


            Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", eMailID, "New Cab Booking Request", mbody)
            Dim mailclient As New System.Net.Mail.SmtpClient()
            'email.cc.add("manish@myndsol.com")
            email.IsBodyHtml = True
            Dim basicauthenticationinfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "smaca")
            mailclient.Host = "mail.myndsol.com"
            mailclient.UseDefaultCredentials = False
            mailclient.Credentials = basicauthenticationinfo
            Try
                mailclient.Send(email)
                SendBookSMS(11100, MobileNo, "Dear " & VendorName & ",Booking Request from " & usermobile & " recieved. Thanks")
            Catch ex As Exception
                Return "Error Sending Request"
            End Try

            If ds.Tables("data").Rows.Count < 1 Then
                Return "Error Sending Request"
            Else
                Return "Booking Request Submitted"
            End If
        Catch ex As Exception
            Return ex.Message
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Function

    Private Sub SendBookSMS(templateID As String, MobileNumber As String, ByVal msg As String)
        Dim msgString As String = "http://smsalertbox.com/api/sms.php?uid=6d796e646270&pin=51f36abe9f80a&sender=MYNDBP&route=5&tempid=" & templateID & "&mobile=" & MobileNumber & "&message=" & msg & "&pushid=1"
        Dim result As String = apicall(msgString)
    End Sub

    Public Function CheckFence(lat As Double, log As Double, redious As Integer, checklat As Single, checklog As Single) As Boolean
        Dim inrcule As Boolean = False
        Dim check As Double
        check = Math.Pow((checklat - lat), 2) + Math.Pow((checklog - log), 2) - Math.Pow(Convert.ToDouble(redious), 2)
        If check < 0 Then
            inrcule = True
        Else
            inrcule = False
        End If
        Return inrcule
    End Function


    Public Shared Function calculate(lat1 As Double, lon1 As Double, lat2 As Double, lon2 As Double) As Double
        Dim R = 6372.8
        ' In kilometers
        Dim dLat = toRadians(lat2 - lat1)
        Dim dLon = toRadians(lon2 - lon1)
        lat1 = toRadians(lat1)
        lat2 = toRadians(lat2)

        Dim a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2)
        Dim c = 2 * Math.Asin(Math.Sqrt(a))
        Return R * 2 * Math.Asin(Math.Sqrt(a))
    End Function

    Public Shared Function toRadians(angle As Double) As Double
        Return Math.PI * angle / 180.0
    End Function

    Public Function apicall(url As String) As String

        ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

        'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
        'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
        'ServicePointManager.SecurityProtocol = Tls12

        Dim httpreq As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Try
            Dim httpres As HttpWebResponse = DirectCast(httpreq.GetResponse(), HttpWebResponse)
            Dim sr As New StreamReader(httpres.GetResponseStream())
            Dim results As String = sr.ReadToEnd()
            sr.Close()
            Return results
        Catch
            Return "0"
        End Try
    End Function

    Public Function GetVehicles() As String Implements IDMSService.GetVehicles

        Dim returnString As New StringBuilder()
        returnString.Clear()

        Dim dtImieNo As New DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=54 and fld12<>'' and fld12<>'0' ", con)
        da.Fill(dtImieNo)

        For i As Integer = 0 To dtImieNo.Rows.Count - 1

            Dim dtveh As New DataTable
            Dim icon As String
            Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] , g.Speed"
            str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
            da.SelectCommand.CommandText = str
            da.SelectCommand.CommandTimeout = 180
            da.Fill(dtveh)
            If dtveh.Rows.Count = 0 Then
                Continue For
            End If

            Dim todayDt As String = (DirectCast(Now, Date)).ToShortDateString()
            'Dim todayDt As String = "01/15/2015"
            returnString.Append(dtveh.Rows(0).Item("VhNo"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("IMIENO"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Site_Name"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Speed"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Lat"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Long"))
            returnString.Append(",")

            If dtveh.Rows(0).Item("IdealTime") > 10 And dtveh.Rows(0).Item("IdealTime") <= 600 And CDate(dtveh.Rows(0).Item("ctime")) = CDate(todayDt) Then
                icon = "#999999"
            ElseIf dtveh.Rows(0).Item("IdealTime") >= 0 And dtveh.Rows(0).Item("IdealTime") < 10 And CDate(dtveh.Rows(0).Item("ctime")) = CDate(todayDt) Then
                icon = "#009900"
            ElseIf (dtveh.Rows(0).Item("IdealTime") > 600 And dtveh.Rows(0).Item("IdealTime") <= 1440) Or (CDate(dtveh.Rows(0).Item("ctime")) = CDate(todayDt) And dtveh.Rows(0).Item("IdealTime") > 1440) Then
                icon = "#FF4DFF"
            Else
                icon = "#FF1919"
            End If
            returnString.Append(icon)
            returnString.Append(":")

        Next

        Return returnString.ToString()

    End Function

    Public Function GetNearestSites(IMIENO As String, Radius As Integer, Eid As Integer) As String Implements IDMSService.GetNearestSites
        Dim returnString As New StringBuilder()
        Dim VehString As New StringBuilder()
        returnString.Clear()

        Dim dtveh As New DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Select top 1 Tid,Lattitude[Lat],Longitude[Long] from mmm_mst_gpsdata where IMIENO='" & IMIENO & "' order by Ctime desc", con)
        oda.Fill(dtveh)

        If dtveh.Rows.Count = 0 Then
            Return ""
        End If
        returnString.Append(dtveh.Rows(0).Item("Lat").ToString())
        returnString.Append(",")
        returnString.Append(dtveh.Rows(0).Item("Long").ToString())
        returnString.Append("=")

        Dim filePath As String = ""

        If Eid = 0 Then
            filePath = HostingEnvironment.MapPath("~/Scripts/CsvJson_54.txt")
        Else
            filePath = HostingEnvironment.MapPath("~/Scripts/CsvJson_" & Eid & ".txt")
        End If

        Dim csvString As String = IO.File.ReadAllText(filePath)
        Dim csvRows = csvString.Split("|")

        For rowIndex As Integer = 0 To csvRows.Length - 1
            Dim rowArray As String() = csvRows(rowIndex).Split("^")
            If rowArray(0).ToString().Trim() = "" Then
                Continue For
            End If

            If IsNumeric(rowArray(4)) And IsNumeric(rowArray(5)) Then
                Dim latt As Double = Convert.ToDouble(rowArray(4))
                Dim longt As Double = Convert.ToDouble(rowArray(5))
                Dim dist As Double = distance(latt, longt, dtveh.Rows(0).Item("Lat"), dtveh.Rows(0).Item("Long"), "K")
                If dist <= Convert.ToDouble(Radius) Then
                    returnString.Append(rowArray(0).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(1).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(2).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(3).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(4).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(5).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(6).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(7).ToString())
                    returnString.Append(":")
                End If
            End If
        Next



        Dim dtImieNo As New DataTable

        Dim vQry = ""

        If Eid = 0 Then
            vQry = "select distinct fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=54 and fld12<>'' and fld12<>'0' "
        Else
            vQry = "select distinct fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=" & Eid & " and fld12<>'' and fld12<>'0' "
        End If

        Dim da As SqlDataAdapter = New SqlDataAdapter(vQry, con)

        da.Fill(dtImieNo)


        For i As Integer = 0 To dtImieNo.Rows.Count - 1

            Dim dtAllveh As New DataTable
            Dim icon As String
            Dim str As String = ""
            If Eid = 0 Then
                str = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] , g.Speed"
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
            Else
                str = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] , g.Speed"
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=" & Eid & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
            End If



            da.SelectCommand.CommandText = str
            da.SelectCommand.CommandTimeout = 180
            da.Fill(dtAllveh)
            If dtAllveh.Rows.Count = 0 Then
                Continue For
            End If

            Dim dist As Double = distance(dtAllveh.Rows(0).Item("Lat"), dtAllveh.Rows(0).Item("Long"), dtveh.Rows(0).Item("Lat"), dtveh.Rows(0).Item("Long"), "K")

            If dist <= Convert.ToDouble(Radius) Then

                Dim todayDt As String = (DirectCast(Now, Date)).ToShortDateString()

                VehString.Append(dtAllveh.Rows(0).Item("VhNo"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("IMIENO"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Site_Name"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Speed"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Lat"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Long"))
                VehString.Append(",")

                If dtAllveh.Rows(0).Item("IdealTime") > 10 And dtAllveh.Rows(0).Item("IdealTime") <= 600 And dtAllveh.Rows(0).Item("ctime") = todayDt Then
                    icon = "#999999"
                ElseIf dtAllveh.Rows(0).Item("IdealTime") >= 0 And dtAllveh.Rows(0).Item("IdealTime") < 10 And dtAllveh.Rows(0).Item("ctime") = todayDt Then
                    icon = "#009900"
                ElseIf (dtAllveh.Rows(0).Item("IdealTime") > 600 And dtAllveh.Rows(0).Item("IdealTime") <= 1440) Or (dtAllveh.Rows(0).Item("ctime") = todayDt And dtAllveh.Rows(0).Item("IdealTime") > 1440) Then
                    icon = "#FF4DFF"
                Else
                    icon = "#FF1919"
                End If
                VehString.Append(icon)
                VehString.Append(":")
            End If

        Next

        Return returnString.ToString() & "=" & VehString.ToString()

    End Function


    Public Function GetCircles(Eid As Integer) As String Implements IDMSService.GetCircles
        Dim returnString As New StringBuilder()
        returnString.Clear()
        Try
            Dim Qry As String = "Select Tid,fld1[Circle] from MMM_mst_master where Eid=" & Eid & " and documenttype='Circle'"
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter(Qry, con)
            da.Fill(dt)
            For i As Integer = 0 To dt.Rows.Count - 1
                returnString.Append(dt.Rows(i).Item("Tid").ToString())
                returnString.Append(",")
                returnString.Append(dt.Rows(i).Item("Circle").ToString())
                returnString.Append(":")
            Next
        Catch ex As Exception
            Return returnString.ToString()
        Finally

        End Try
        Return returnString.ToString()
    End Function
    Public Function GetCircleVehicles(Id As Integer) As String Implements IDMSService.GetCircleVehicles
        Dim VehString As New StringBuilder()
        VehString.Clear()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dtImieNo As New DataTable
            Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=54 and fld12<>'' and fld12<>'0' and fld19='" & Id & "' ", con)
            da.Fill(dtImieNo)

            For i As Integer = 0 To dtImieNo.Rows.Count - 1
                Dim dtAllveh As New DataTable
                Dim icon As String
                Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] , g.Speed"
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
                da.SelectCommand.CommandText = str
                da.SelectCommand.CommandTimeout = 180
                da.Fill(dtAllveh)
                If dtAllveh.Rows.Count = 0 Then
                    Continue For
                End If

                Dim todayDt As String = (DirectCast(Now, Date)).ToShortDateString()

                VehString.Append(dtAllveh.Rows(0).Item("VhNo"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("IMIENO"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Site_Name"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Speed"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Lat"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Long"))
                VehString.Append(",")

                If dtAllveh.Rows(0).Item("IdealTime") > 10 And dtAllveh.Rows(0).Item("IdealTime") <= 600 And dtAllveh.Rows(0).Item("ctime") = todayDt Then
                    icon = "#999999"
                ElseIf dtAllveh.Rows(0).Item("IdealTime") >= 0 And dtAllveh.Rows(0).Item("IdealTime") < 10 And dtAllveh.Rows(0).Item("ctime") = todayDt Then
                    icon = "#009900"
                ElseIf (dtAllveh.Rows(0).Item("IdealTime") > 600 And dtAllveh.Rows(0).Item("IdealTime") <= 1440) Or (dtAllveh.Rows(0).Item("ctime") = todayDt And dtAllveh.Rows(0).Item("IdealTime") > 1440) Then
                    icon = "#FF4DFF"
                Else
                    icon = "#FF1919"
                End If
                VehString.Append(icon)
                VehString.Append(":")

            Next
        Catch ex As Exception
            Return VehString.ToString()
        End Try
        Return VehString.ToString()

    End Function

    Public Function GetSitesFromLatLong(Lat As Double, Longt As Double, radius As Integer) As String Implements IDMSService.GetSitesFromLatLong
        Dim returnString As New StringBuilder()
        returnString.Clear()
        Dim filePath As String = HostingEnvironment.MapPath("~/DOCS/CsvJson.txt")
        Dim csvString As String = IO.File.ReadAllText(filePath)
        Dim csvRows = csvString.Split("|")
        For rowIndex As Integer = 0 To csvRows.Length - 1
            Dim rowArray As String() = csvRows(rowIndex).Split("^")
            If IsNumeric(rowArray(4)) And IsNumeric(rowArray(5)) Then
                Dim SiteLat As Double = Convert.ToDouble(rowArray(4))
                Dim SiteLong As Double = Convert.ToDouble(rowArray(5))
                Dim dist As Double = distance(SiteLat, SiteLong, Lat, Longt, "K")
                If dist <= Convert.ToDouble(radius) Then
                    returnString.Append(rowArray(0).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(1).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(2).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(3).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(4).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(5).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(6).ToString())
                    returnString.Append(",")
                    returnString.Append(rowArray(7).ToString())
                    returnString.Append(":")
                End If
            End If
        Next
        Return returnString.ToString()
    End Function
    Public Function GetUserVehicles(Eid As Integer, Uid As Integer, CircleId As Integer) As String Implements IDMSService.GetUserVehicles
        Dim VehString As New StringBuilder()
        VehString.Clear()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            da.SelectCommand.CommandText = "Select * from mmm_mst_user where uid=" & Uid
            Dim dtUser As New DataTable
            da.Fill(dtUser)
            'If dtUser.Rows(0).Item("userrole") = "SU" Or dtUser.Rows(0).Item("userrole").ToString.ToUpper() = "CORPORATEUSER" Then
            '    da.SelectCommand.CommandText = "Select fld11[Clusters] from MMM_MST_USER with (nolock) where EID=" & Eid
            'Else
            '    da.SelectCommand.CommandText = "Select fld11[Clusters] from MMM_MST_USER with (nolock) where EID=" & Eid & " and uid=" & Uid
            'End If

            da.SelectCommand.CommandText = "Select DocMapping from mmm_mst_forms where eid=" & Eid & " and IsRoleDef=1 and FormName='Cluster'"
            Dim dtDocMapping As New DataTable
            da.Fill(dtDocMapping)

            If dtDocMapping.Rows.Count = 0 Then
                Return ""
            End If

            da.SelectCommand.CommandText = "Select " & dtDocMapping.Rows(0).Item("DocMapping") & "[Clusters],documenttype from MMM_Ref_Role_User where eid=" & Eid & " and Uid=" & Uid & " and rolename='" & dtUser.Rows(0).Item("userrole") & "'  and documenttype like '%Cluster%'"
            Dim dt As New DataTable
            da.Fill(dt)
            'If dt.Rows.Count = 0 Then
            '    Return ""
            'End If

            Dim dtClusters As New DataTable
            If dtUser.Rows(0).Item("userrole") = "SU" Or dtUser.Rows(0).Item("userrole").ToString.ToUpper() = "CORPORATEUSER" Then
                da.SelectCommand.CommandText = "select Tid,fld1[ClusterName] from mmm_mst_master with (nolock) where Eid=" & Eid & " and Documenttype='Cluster' and fld11=" & CircleId
            Else
                da.SelectCommand.CommandText = "select Tid,fld1[ClusterName] from mmm_mst_master with (nolock) where Eid=" & Eid & " and Documenttype='Cluster' and fld11=" & CircleId & " and Tid in(" & dt.Rows(0).Item(0) & ")"
            End If
            da.Fill(dtClusters)
            Dim dtImieNo As New DataTable
            If dtUser.Rows(0).Item("userrole") = "SU" Or dtUser.Rows(0).Item("userrole").ToString.ToUpper() = "CORPORATEUSER" Then
                da.SelectCommand.CommandText = "select fld1[VhNo], fld10[VehicleName], fld16[Cluster], fld12[IMIENO] from mmm_mst_master with (nolock) where documenttype='vehicle' and eid=" & Eid & " and fld12<>'0' and fld12<>''"
            Else
                da.SelectCommand.CommandText = "select fld1[VhNo], fld10[VehicleName], fld16[Cluster], fld12[IMIENO] from mmm_mst_master with (nolock) inner join  dbo.split('" & dt.Rows(0).Item("Clusters").ToString() & "', ',') s on s.items in (select items from dbo.split(fld16, ',')) where documenttype='vehicle' and eid=" & Eid & " and fld12<>'0' and fld12<>''"
            End If
            da.Fill(dtImieNo)
            If dtImieNo.Rows.Count = 0 Then
                Return ""
            End If
            For i As Integer = 0 To dtImieNo.Rows.Count - 1
                Dim dtAllveh As New DataTable
                Dim icon As String
                Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] , g.Speed"
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=" & Eid & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
                da.SelectCommand.CommandText = str
                da.SelectCommand.CommandTimeout = 180
                da.Fill(dtAllveh)
                If dtAllveh.Rows.Count = 0 Then
                    Continue For
                End If

                Dim todayDt As String = (DirectCast(Now, Date)).ToShortDateString()

                VehString.Append(dtAllveh.Rows(0).Item("VhNo"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("IMIENO"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Site_Name"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Speed"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Lat"))
                VehString.Append(",")
                VehString.Append(dtAllveh.Rows(0).Item("Long"))
                VehString.Append(",")

                If dtAllveh.Rows(0).Item("IdealTime") > 10 And dtAllveh.Rows(0).Item("IdealTime") <= 600 And CDate(dtAllveh.Rows(0).Item("ctime")) = CDate(todayDt) Then
                    icon = "#999999"
                ElseIf dtAllveh.Rows(0).Item("IdealTime") >= 0 And dtAllveh.Rows(0).Item("IdealTime") < 10 And CDate(dtAllveh.Rows(0).Item("ctime")) = CDate(todayDt) Then
                    icon = "#009900"
                ElseIf (dtAllveh.Rows(0).Item("IdealTime") > 600 And dtAllveh.Rows(0).Item("IdealTime") <= 1440) Or (CDate(dtAllveh.Rows(0).Item("ctime")) = CDate(todayDt) And dtAllveh.Rows(0).Item("IdealTime") > 1440) Then
                    icon = "#FF4DFF"
                Else
                    icon = "#FF1919"
                End If
                VehString.Append(icon)
                VehString.Append(":")
            Next

        Catch ex As Exception
            Return ""
        End Try
        Return VehString.ToString()
    End Function
    Public Function NearestSite(Lat As Double, Longt As Double, Eid As Integer) As String Implements IDMSService.NearestSite
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim returnString As New StringBuilder()
        Try
            Dim qry As String = "Select fld1[Store Name], fld10[Address], fld11[City], fld12[State], fld13[Pin Code], fld14[Location] "
            qry &= " from MMM_MST_MASTER where Eid=" & Eid & " and Documenttype='Store Master' and fld14 is not null"
            Dim da As New SqlDataAdapter(qry, con)
            Dim dt As New DataTable()
            da.Fill(dt)
            dt.Columns.Add("Distance")
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim rowArray As String() = dt.Rows(i).Item("Location").ToString.Split(",")
                If rowArray(0).ToString().Trim() = "" Then
                    Continue For
                End If
                If IsNumeric(rowArray(0)) And IsNumeric(rowArray(1)) Then
                    Dim Sitelatt As Double = Convert.ToDouble(rowArray(0))
                    Dim SiteLong As Double = Convert.ToDouble(rowArray(1))
                    Dim Dist = distance(Sitelatt, SiteLong, Lat, Longt, "K")
                    dt.Rows(i).Item("Distance") = Dist
                End If
            Next
            Dim view As New DataView(dt)
            view.Sort = "Distance ASC"
            Dim dtr = view.ToTable()
            For i As Integer = 0 To dt.Columns.Count - 2
                returnString.Append(dtr.Columns(i).ColumnName & " : " & dtr.Rows(0).Item(i))
                returnString.Append("^")
            Next
            returnString.Append("|")
            For i As Integer = 0 To dt.Columns.Count - 2
                returnString.Append(dtr.Columns(i).ColumnName & " : " & dtr.Rows(1).Item(i))
                returnString.Append("^")
            Next
            Return returnString.ToString()
        Catch ex As Exception
            Return returnString.ToString()
        End Try
    End Function

    'Public Function GetGPSRawData(Skey As String, Eid As Integer, sDate As String, eDate As String) As String Implements IDMSService.GetGPSRawData
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Try
    '        If Not Skey = "R@GPS" Then
    '            Return "Invalid key"
    '        End If
    '        Dim qry As String = "Select IMIENO, Lattitude, Longitude, Altitude, Speed, convert(varchar,CTime,9)CTime, convert(varchar(128),RecordTime)RecordTime, DevDist[DevDistance],ibuttonID[CardID]"
    '        qry += "from mmm_mst_gpsdata with(nolock) where IMIENO='356307047664379'"
    '        qry += "And convert(datetime,Ctime)>=convert(datetime,'" & sDate & "')"
    '        qry += "and convert(datetime,Ctime)<=convert(datetime,'" & eDate & "')"
    '        Dim da As SqlDataAdapter = New SqlDataAdapter(qry, con)
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        Return GetJson(dt)
    '    Catch ex As Exception
    '        Return ""
    '    Finally

    '    End Try
    '    Return ""
    'End Function

    Public Function GetGPSRawData(Skey As String, Eid As Integer, sDate As String, eDate As String, IMEI As String) As String Implements IDMSService.GetGPSRawData
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            If Not Skey = "R@GPS" Then
                Return "Invalid key"
            End If
            '356307047664379
            Dim qry As String = "Select IMIENO, Lattitude, Longitude, Altitude, Speed, convert(varchar,CTime,9)CTime, convert(varchar(128),RecordTime)RecordTime, DevDist[DevDistance],ibuttonID[CardID]"
            qry += "from mmm_mst_gpsdata with(nolock) where IMIENO='" & IMEI & "'"
            qry += "And convert(datetime,Ctime)>=convert(datetime,'" & sDate & "')"
            qry += "and convert(datetime,Ctime)<=convert(datetime,'" & eDate & "')"
            Dim da As SqlDataAdapter = New SqlDataAdapter(qry, con)
            Dim dt As New DataTable
            da.Fill(dt)
            Return GetJson(dt)
        Catch ex As Exception
            Return ""
        Finally

        End Try
        Return ""
    End Function



    Private Function GetJson(ByVal dt As DataTable) As String
        Dim Jserializer As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim rowsList As New List(Of Dictionary(Of String, Object))()
        Dim row As Dictionary(Of String, Object)
        For Each dr As DataRow In dt.Rows
            row = New Dictionary(Of String, Object)()
            For Each col As DataColumn In dt.Columns
                row.Add(col.ColumnName, dr(col))
            Next
            rowsList.Add(row)
        Next
        Jserializer.MaxJsonLength = 2147483647
        Return Jserializer.Serialize(rowsList)
    End Function

    Public Function GetEmergencyServices(IMEI As String, Latt As Double, Longt As Double) As XElement Implements IDMSService.GetEmergencyServices
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim XDoc As New XDocument()
        Try
            Dim ds As New DataSet()
            Dim Query = "Select Tid, fld1[Emergency_Service_Name], fld10[Service_Type], fld11[Icon], fld12[Emergency_Service_Ph_No], "
            Query += " fld13[Service_Area], fld14[Contact_Person]"
            Query += " from mmm_mst_master where Documenttype='Emergency Services' and IsAuth=1"
            Dim da As New SqlDataAdapter(Query, con)
            da.SelectCommand.CommandText = Query
            da.Fill(ds, "Master")
            Dim newTable As DataTable
            newTable = ds.Tables("Master").Clone()
            newTable.Clear()
            For i As Integer = 0 To ds.Tables("Master").Rows.Count - 1
                Dim AreaLatLong() As String = ds.Tables("Master").Rows(i).Item("Service_Area").ToString().Split(",")
                Dim Dist As Double = distance(Latt, Longt, Convert.ToDouble(AreaLatLong(0)), Convert.ToDouble(AreaLatLong(1)), "m")
                If Dist <= Convert.ToDouble(AreaLatLong(2)) Then
                    newTable.ImportRow(ds.Tables("Master").Rows(i))
                End If
            Next

            Dim dsnew As New DataSet("GetEmergencyServicesResult")
            dsnew.Tables.Add(newTable)
            XDoc = XDocument.Parse(dsnew.GetXml)
            Return XDoc.Root

        Catch ex As Exception

        End Try
        Return XDoc.Root
    End Function

    Public Function GetOffers(IMEI As String, Latt As Double, Longt As Double) As XElement Implements IDMSService.GetOffers
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim XDoc As New XDocument()
        Try
            Dim Query = "Select Tid, fld1[Merchant_Name], fld10[Merchant_Contact_No], fld11[Offer_for_the_Day], fld12[Service_Area] "
            Query += " from mmm_mst_master where Documenttype='Merchant Master' and IsAuth=1"
            Dim da As New SqlDataAdapter(Query, con)
            Dim ds As New DataSet()
            da.Fill(ds, "Master")
            Dim newTable As DataTable
            newTable = ds.Tables("Master").Clone()
            newTable.Clear()
            For i As Integer = 0 To ds.Tables("Master").Rows.Count - 1
                Dim AreaLatLong() As String = ds.Tables("Master").Rows(i).Item("Service_Area").ToString().Split(",")
                Dim Dist As Double = distance(Latt, Longt, Convert.ToDouble(AreaLatLong(0)), Convert.ToDouble(AreaLatLong(1)), "m")
                If Dist <= Convert.ToDouble(AreaLatLong(2)) Then
                    newTable.ImportRow(ds.Tables("Master").Rows(i))
                End If
            Next
            Dim dsnew As New DataSet("GetOffersResult")
            dsnew.Tables.Add(newTable)
            XDoc = XDocument.Parse(dsnew.GetXml)
            Return XDoc.Root
        Catch ex As Exception

        End Try
        Return XDoc.Root
    End Function

    Public Function MerchentPOCLogin(Uid As String, Password As String) As XElement Implements IDMSService.MerchentPOCLogin
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim XDoc As New XDocument()
        Dim xmlStr As New StringBuilder()
        Try
            Dim Query = "Select Tid, fld13, fld14 as [Password] "
            Query += " from mmm_mst_master where Eid=61 and Documenttype='Merchant Master' and IsAuth=1 and fld13='" & Uid & "'"
            Dim da As New SqlDataAdapter(Query, con)
            Dim dt As New DataTable
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                If dt.Rows(0).Item("Password") = Password Then
                    xmlStr.Append("<LoginResult>")
                    xmlStr.Append("<Result>1</Result>")
                    xmlStr.Append("<Data>" & dt.Rows(0).Item("Tid") & "</Data>")
                    xmlStr.Append("</LoginResult>")
                Else
                    xmlStr.Append("<LoginResult>")
                    xmlStr.Append("<Result>0</Result>")
                    xmlStr.Append("<Data></Data>")
                    xmlStr.Append("</LoginResult>")
                End If
            Else
                xmlStr.Append("<LoginResult>")
                xmlStr.Append("<Result>0</Result>")
                xmlStr.Append("<Data></Data>")
                xmlStr.Append("</LoginResult>")
            End If
            XDoc = XDocument.Parse(xmlStr.ToString)
            Return XDoc.Root
        Catch ex As Exception

        End Try
        Return XDoc.Root
    End Function

    Public Function SaveGCM(UserKey As String, IMEI As String, MerchentID As Integer) As String Implements IDMSService.SaveGCM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim Query = "Select * from PocRegisterUser where UserKey='" & UserKey & "'"
            Dim da As New SqlDataAdapter(Query, con)
            Dim dt As New DataTable
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                Query = "Update PocRegisterUser set IMEI='" & IMEI & "', MerchentID='" & MerchentID & "' IsActive=1 where Tid=" & dt.Rows(0).Item("Tid")
            Else
                Query = "insert into PocRegisterUser(UserKey,IMEI,MerchentID,IsActive) values('" & UserKey & "', '" & IMEI & "', '" & MerchentID & "',1)"
            End If

            If Not con.State = ConnectionState.Open Then
                con.Open()
            End If
            Dim cmd As New SqlCommand(Query, con)
            Return cmd.ExecuteNonQuery().ToString()
            con.Close()
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Return 0
    End Function
    Public Function SendGcmNotification(MerchentID As Integer, Radius As Integer, offer As String) As String Implements IDMSService.SendGcmNotification
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim retStr As New StringBuilder()
        Try
            Dim Query = "Select Tid, fld1[Merchant_Name], fld10[Merchant_Contact_No], fld11[Offer_for_the_Day], fld12[Service_Area] "
            Query += " from mmm_mst_master where Documenttype='Merchant Master' and IsAuth=1 and Tid=" & MerchentID
            Dim da As New SqlDataAdapter(Query, con)
            Dim ds As New DataSet()
            da.Fill(ds, "Master")
            Dim newTable As DataTable
            newTable = ds.Tables("Master").Clone()
            newTable.Clear()
            Dim AreaLatLong() As String = ds.Tables("Master").Rows(0).Item("Service_Area").ToString().Split(",")

            Query = "Select * from PocRegisterUser where MerchentID=" & MerchentID
            da.SelectCommand.CommandText = Query
            da.Fill(ds, "Users")

            For i As Integer = 0 To ds.Tables("Users").Rows.Count - 1
                Try
                    Query = "Select top 1 Lattitude, Longitude from mmm_mst_Gpsdata with (nolock) where IMIENO=" & ds.Tables("Users").Rows(i).Item("IMEI") & " order by cTime desc"
                    da.SelectCommand.CommandText = Query
                    Dim dtGps As New DataTable()
                    da.Fill(dtGps)
                    If dtGps.Rows.Count = 0 Then
                        Continue For
                    End If

                    Dim Dist As Double = distance(dtGps.Rows(0).Item("Lattitude"), dtGps.Rows(0).Item("Longitude"), Convert.ToDouble(AreaLatLong(0)), Convert.ToDouble(AreaLatLong(1)), "m")
                    If Dist <= Radius Then
                        Dim obj As New GisMethods()
                        'obj.sendNotification(ds.Tables("Users").Rows(i).Item("UserKey"), ds.Tables("Master").Rows(0).Item("Offer_for_the_Day").ToString)
                        obj.sendNotification(ds.Tables("Users").Rows(i).Item("UserKey"), offer)
                    End If
                    retStr.Append(ds.Tables("Users").Rows(i).Item("IMEI") & ":Sent,")
                Catch ex As Exception
                    retStr.Append(ds.Tables("Users").Rows(i).Item("IMEI") & ":Error,")
                End Try

            Next
            Return retStr.ToString()
        Catch ex As Exception

        End Try
        Return retStr.ToString()
    End Function

    Public Function GetUsers(MerchentID As Integer, Radius As Integer) As XElement Implements IDMSService.GetUsers
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim XDoc As New XDocument()
        Try
            Dim Query = "Select Tid, fld1[Merchant_Name], fld10[Merchant_Contact_No], fld11[Offer_for_the_Day], fld12[Service_Area] "
            Query += " from mmm_mst_master where Documenttype='Merchant Master' and IsAuth=1 and Tid=" & MerchentID
            Dim da As New SqlDataAdapter(Query, con)
            Dim ds As New DataSet()
            da.Fill(ds, "Master")
            Query = "Select IMIENO, Max(Ctime) ctime from tempGPS group by IMIENO"
            da.SelectCommand.CommandText = Query
            da.Fill(ds, "latest")

            Dim AreaLatLong() As String = ds.Tables("Master").Rows(0).Item("Service_Area").ToString().Split(",")

            Dim dt As New DataTable()
            For i As Integer = 0 To ds.Tables("latest").Rows.Count - 1
                Query = "Select IMIENO, Lattitude, Longitude from tempGPS where ctime='" & ds.Tables("latest").Rows(i).Item("ctime") & "'"
                da.SelectCommand.CommandText = Query
                da.Fill(dt)
            Next

            Dim newTable As DataTable
            newTable = dt.Clone()
            newTable.Clear()
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim Dist As Double = distance(dt.Rows(i).Item("Lattitude"), dt.Rows(i).Item("Longitude"), Convert.ToDouble(AreaLatLong(0)), Convert.ToDouble(AreaLatLong(1)), "m")
                If Dist <= Radius Then
                    newTable.ImportRow(dt.Rows(i))
                End If
            Next

            Dim dss As New DataSet("Result")
            dss.Tables.Add(newTable)
            XDoc = XDocument.Parse(dss.GetXml)
            Return XDoc.Root
        Catch ex As Exception

        End Try
        Return XDoc.Root
    End Function

    Public Function googleCloudID(eid As Integer, googleid As String, userid As Integer, userrole As String) As String Implements IDMSService.googleCloudID
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertGoogleCloudId", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("eid", Trim(eid))
            oda.SelectCommand.Parameters.AddWithValue("googleid", Trim(googleid))
            oda.SelectCommand.Parameters.AddWithValue("userid", Trim(userid))
            oda.SelectCommand.Parameters.AddWithValue("userole", Trim(userrole))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Catch ex As Exception
            Return "Not Success"
        Finally
            con.Close()
        End Try
        Return "Success"
        con.Close()
    End Function


    Function ExecuteScheduleDocument() As String Implements IDMSService.ExecuteScheduleDocument
        Try
            Dim objSh As New ScheduleDocument(0, "", "Schedule")
            Dim res = objSh.Execute(0)
            '  Dim res = ""
            Return res
        Catch ex As Exception
            Return "Error in execution."
        End Try
    End Function


    Public Function ClusterwiseData(EID As Integer, UID As Integer, USERROLE As String) As XElement Implements IDMSService.ClusterwiseData
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        Dim oda As SqlDataAdapter = New SqlDataAdapter("ClusterwiseData", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Trim(EID))
        oda.SelectCommand.Parameters.AddWithValue("uid", Trim(UID))
        oda.SelectCommand.Parameters.AddWithValue("userrole", Trim(USERROLE))
        Dim xdoc As New XDocument()
        Dim ds As New DataSet
        oda.SelectCommand.CommandTimeout = 20000
        oda.Fill(ds, "item")
        'Dim newTable As DataTable
        'newTable = ds.Tables("item").Clone()
        'newTable.Clear()
        'For i As Integer = 0 To ds.Tables("item").Rows.Count - 1
        'Next
        'Dim dsnew As New DataSet("IMIENO")
        'dsnew.Tables.Add(newTable)
        xdoc = XDocument.Parse(ds.GetXml)
        Return xdoc.Root
    End Function

    Public Function VehicleDataInfo(IMEINO As String) As XElement Implements IDMSService.VehicleDataInfo
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("select *from (select [Driver Name]DriverName,[driver mobile number]DriverMobileNo,[Registration No.]RegistrationNo,[Vehicle Manufacturer]VehicleManufacturer,[GPS Device IMEI No] GPSDeviceIMEI  from v32vehicle where [GPS Device IMEI No]='" & IMEINO & "') a join (select top 1 lattitude,longitude,IMIENO,speed,cTime from MMM_MST_GPSDATA where IMIENO='" & IMEINO & "' order BY cTime DESC) b on b.IMIENO=a.[GPSDeviceIMEI]", con)

        Dim xdoc As New XDocument()
        Dim ds As New DataSet
        oda.SelectCommand.CommandTimeout = 20000
        oda.Fill(ds, "item")
        xdoc = XDocument.Parse(ds.GetXml)
        Return xdoc.Root
    End Function

    Public Sub AutoRunSchedulerforDMS()
        AutoRunLog("AutoRunSchedulerforDMS", "Func Call Starts", "Starting", "0")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim schdeuledtime As String = ""
        Dim DaysDiff As String = ""
        Dim eid As Integer = 0
        Dim da As New SqlDataAdapter("select case when ismailsend=1 then scheduledmail else null end as schdeuledtime,eid from mmm_mst_entity where ismailsend=1", con)
        Dim dt As New DataTable
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            For m As Integer = 0 To dt.Rows.Count - 1
                schdeuledtime = Convert.ToString(dt.Rows(m)(0))
                eid = dt.Rows(m)(1)
                ' AutoRunLog("AutoRunSchedulerforDMS", "in For Loop", "Entity ", eid)
                Try
                    da.SelectCommand.CommandText = "declare @startdate datetime declare @EndDate datetime =getdate() select @startdate=lastrun from mmm_mst_entity where eid=" & eid & " select DATEDIFF(day,@startdate,@EndDate)  as Days"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    DaysDiff = Convert.ToString(da.SelectCommand.ExecuteScalar())
                    ' AutoRunLog("AutoRunSchedulerforDMS", "in For Loop", "Getting Days difference " & DaysDiff, eid)
                    con.Close()
                Catch ex As Exception
                    Dim err As New Apperrors(EID:=eid, UID:=0, MessHeading:=ex.Message, erroMess:="Issue in getting Scheduled timing")
                End Try
                Try
                    ' AutoRunLog("AutoRunSchedulerforDMS", "in For Loop", "Check Validate timing " & DaysDiff, eid)
                    If schdeuledtime.Length > 1 Then
                        If (IsValidScheduleTime(schdeuledtime)) Then
                            AutoRunLog("AutoRunSchedulerforDMS", "Scheduled Time Validated", "Run According to Entity wise", eid)
                            Dim folderids As String = ""
                            Dim fileids As String = ""
                            da.SelectCommand.CommandText = "declare @TIDS nvarchar(max)=''	SELECT @TIDS= STUFF((SELECT  ',' + convert( varchar,a.tid)           from mmm_mst_doc as a inner join mmm_mst_entity  as b on a.eid=b.eid where  b.eid=" & eid & " and  a.adate between  b.LastRun and getdate() and a.documenttype is null and a.curstatus='UPLOADED'        FOR XML PATH('')), 1, 1, '') 	select @TIDS	"
                            Try
                                If con.State = ConnectionState.Closed Then
                                    con.Open()
                                End If
                                folderids = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                AutoRunLog("AutoRunSchedulerforDMS", "Getting Folders Created IDs", folderids.ToString(), eid)
                                con.Close()
                            Catch ex As Exception
                                Dim err As New Apperrors(EID:=eid, UID:=0, MessHeading:=ex.Message, erroMess:="Issue in getting Multiple FolderIds")
                            End Try

                            notificationMailForMulitpleTids(folderids, eid, "FOLDER", "CREATED", DaysDiff)
                            AutoRunLog("AutoRunSchedulerforDMS", "Folders Created Mail has been sent", "", eid)
                            da.SelectCommand.CommandText = "declare @TIDS nvarchar(max)=''	SELECT @TIDS= STUFF((SELECT  ',' + convert( varchar,a.tid)           from mmm_mst_doc as a inner join mmm_mst_entity  as b on a.eid=b.eid where  b.eid=" & eid & " and  a.adate between  b.LastRun and getdate() and a.documenttype ='FILE' and a.curstatus='UPLOADED'        FOR XML PATH('')), 1, 1, '') 	select @TIDS"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            fileids = Convert.ToString(da.SelectCommand.ExecuteScalar())
                            AutoRunLog("AutoRunSchedulerforDMS", "Getting Files Created IDs", fileids.ToString(), eid)
                            TemplateCallingForMultipleTids(fileids, eid, "FILE", "CREATED", DaysDiff)
                            AutoRunLog("AutoRunSchedulerforDMS", "Files Created Mail has been sent", "", eid)
                            DeletedAction("FOLDER", "DELETED", eid, DaysDiff)
                            AutoRunLog("AutoRunSchedulerforDMS", "DELETED Folder Mail has been sent", "", eid)
                            DeletedAction("FILE", "DELETED", eid, DaysDiff)
                            AutoRunLog("AutoRunSchedulerforDMS", "DELETED Files Mail has been sent", "", eid)
                        End If
                    End If
                Catch ex As Exception
                    AutoRunLog("AutoRunSchedulerforDMS", "Folders Created Mail has been sent", "TC Exception msg at notification -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), eid)
                End Try
                da.SelectCommand.CommandText = "update mmm_mst_entity set lastrun=getdate() where eid=" & eid
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim flag As Boolean = Convert.ToBoolean(da.SelectCommand.ExecuteNonQuery())
                ' AutoRunLog("AutoRunSchedulerforDMS", "All Process have done with status " & flag, "", eid)
                con.Close()
            Next
        End If
        ' AutoRunLog("AutoRunSchedulerforDMS", "Exit from function", "", eid)
    End Sub

    Function IsValidScheduleTime(Time As String) As Boolean
        Dim b As Boolean = False
        Try
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            Dim i As Integer = 0
            ' Dim date1f As String 
            If (Time <> "") Then
                'Dim schedule As String = "*|*|*|*|*"
                Dim schedule As String = Time
                Dim str_array As [String]() = schedule.Split("|")
                Dim stringa As [String] = str_array(0)
                Dim stringb As [String] = str_array(1)
                Dim stringc As [String] = str_array(2)
                Dim stringd As [String] = str_array(3)
                Dim stringe As [String] = str_array(4)
                Dim currentTime As System.DateTime = System.DateTime.Now
                Dim currentdate As String = currentTime.Date.Day
                Dim str_datte As [String]() = stringb.Split(",")
                Dim str_hours As [String]() = stringd.Split(",")
                Dim str_mintus As [String]() = stringe.Split(",")
                If Trim(stringa) = "*" And Trim(stringb) = "*" And Trim(stringc) = "*" And Trim(stringd) = "*" And Trim(stringe) = "*" Then
                    b = True
                    Return b
                    ' Exit For
                End If
                If stringb <> "*" Then
                    For j As Integer = 0 To str_datte.Length - 1
                        Dim A As [String] = str_datte(j)
                        If A.Contains("-") Then
                            Dim o As [String]() = A.Split("-")
                            Dim f As [String] = o(0)
                            Dim g As [String] = o(1)
                            If (f <= currentdate And g >= currentdate) Then
                                For n As Integer = 0 To str_hours.Length - 1
                                    For m As Integer = 0 To str_mintus.Length - 1
                                        Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                        If x <= time2 And x >= time1 Then
                                            b = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If
                        Else
                            If ((currentdate = A)) Then
                                For n As Integer = 0 To str_hours.Length - 1
                                    For m As Integer = 0 To str_mintus.Length - 1
                                        Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                        If x <= time2 And x >= time1 Then
                                            b = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    Next
                ElseIf ((currentdate = stringb) Or (stringb.Trim() = "*") Or (stringd.Trim() <> "") Or (stringe.Trim() <> "")) Then
                    For n As Integer = 0 To str_hours.Length - 1
                        For m As Integer = 0 To str_mintus.Length - 1
                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                            '  Dim x As DateTime = (Convert.ToDateTime(stringd & ":" & stringe & ":" & "00").ToShortTimeString)
                            If x <= time2 And x >= time1 Then
                                b = True
                                Exit For
                            End If
                        Next
                    Next
                ElseIf ((stringa.Trim() = "*") Or (stringb.Trim() = "*") Or (stringc.Trim() = "*") Or (stringd.Trim() = "*") Or (stringe.Trim() = "*")) Then
                    b = True
                    'Exit For
                End If
            End If
            Return b
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Sub TemplateCallingForMultipleTids(ByVal tid As String, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String, ByVal DaysDiff As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim DS As New DataSet
        Try
            Dim STR As String = ""
            Dim MAILTO As String = ""
            Dim MAILID As String = ""
            Dim subject As String = ""
            Dim MSG As String = ""
            Dim cc As String = ""
            Dim Bcc As String = ""
            Dim MainEvent As String = ""
            Dim fn As String = ""
            STR = "SELECT  dms.getFolderName(stid) [Folder Name], fname [File Name], docurl [File URL], adate [Upload Date], filesize [File Size], dms.getUserName(oUID) [File Owner], dms.getProjectName(gid) [PROJECT NAME], DMS.GetAuthorisedUser(1,Stid,gid,Eid) [CONCERN USERS], DMS.GetParent(1,Stid,Eid) [Absolute Path] FROM  MMM_MST_DOC with (nolock)  WHERE TID in(select * from  SplitString('" & tid & "',','))"

            AutoRunLog("AutoRunSchedulerforDMS", "TemplateCallingForMultipleTids", "Preparing multiple ids for files" & STR, eid)
            da.SelectCommand.CommandText = STR
            da.SelectCommand.CommandTimeout = 4200
            da.Fill(DS, "qry")
            For Each dc As DataColumn In DS.Tables("qry").Columns
                fn = "{" & dc.ColumnName.ToString() & "}"
                MSG = MSG.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                subject = subject.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                MAILTO = MAILTO.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                cc = cc.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
            Next
            subject = "New Files created in Myedms portal"
            da.SelectCommand.CommandText = "select emailid from mmm_mst_user  where eid=" & eid & " and MydmsEnableAlertsendmail='True' "
            da.Fill(DS, "USERS")
            For i As Integer = 0 To DS.Tables("USERS").Rows.Count - 1
                AutoRunLog("AutoRunSchedulerforDMS", "TemplateCallingForMultipleTids", "Gettig users from entity to send mail at row " & i, eid)
                Dim dvPrograms As New DataView
                dvPrograms = DS.Tables("qry").DefaultView
                dvPrograms.RowFilter = "[CONCERN USERS] like '%" & Convert.ToString(DS.Tables("USERS").Rows(i)(0)) & "%'"
                If dvPrograms.ToTable().Rows.Count > 0 Then
                    MAILTO = Convert.ToString(DS.Tables("USERS").Rows(i)(0))
                    AutoRunLog("AutoRunSchedulerforDMS", "TemplateCallingForMultipleTids", "Gettig  matched user email id from entity to send mail at row " & i & " and email id is" & Convert.ToString(DS.Tables("USERS").Rows(i)(0)), eid)
                    'MAILTO = "sunil.pareek@myndsol.com"
                    Dim MailTable As New StringBuilder()
                    MailTable.Append("<div style=""color:""#000000"">Dear Member</div>")
                    MailTable.Append("<br/>")
                    MailTable.Append("During last " & DaysDiff & " days, There are files created by your group members in MYEDMS. ")
                    MailTable.Append("<br/>")

                    If dvPrograms.ToTable().Rows.Count > 12 Then

                        Dim filename As String = MyExtensions.AppendTimeStamp("MYEDMS.CSV")
                        Using writer As StreamWriter = New StreamWriter(Path.Combine(HttpRuntime.AppDomainAppPath, "DOCS/" & eid & "/" & filename))
                            Rfc4180Writer.WriteDataTable(dvPrograms.ToTable(), writer, True)
                        End Using
                        MailTable.Append("<div> <a href=""https://myndsaas.com/DOCS/" & eid & "/" & filename & """ ><b>Click Here</b></a> <b>  link to download and view the list of files. </b> </div>")

                    Else
                        MailTable.Append("<div><b> Following is Details of files –</b></div>")
                        MailTable.Append("<br/>")
                        MailTable.Append("<table border=""1"" width=""100%"">")
                        MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")  ' for header only 
                        For l As Integer = 0 To DS.Tables("qry").Columns.Count - 1
                            MailTable.Append("<td>" & DS.Tables("qry").Columns(l).ColumnName & "</td>")
                        Next
                        For k As Integer = 0 To dvPrograms.ToTable().Rows.Count - 1 ' binding the tr tab in table
                            MailTable.Append("</tr><tr>") ' for row records
                            For t As Integer = 0 To dvPrograms.Table().Columns.Count - 1
                                MailTable.Append("<td>" & dvPrograms.ToTable().Rows(k).Item(t).ToString() & " </td>")
                            Next
                            MailTable.Append("</tr>")
                        Next
                        MailTable.Append("</table>")
                    End If
                    MailTable.Append("<br/><br/>")
                    MailTable.Append("<div>")
                    MailTable.Append("If you want to see more information please")

                    MailTable.Append(" <a href=""https://myedms.myndsolution.com/"" ><b>Click Here</b></a>")
                    MailTable.Append("</div>")
                    MailTable.Append("<br/><br/>")
                    MailTable.Append("Regards ")
                    MailTable.Append("<br/><br/>")
                    MailTable.Append("Myedms Team")
                    MSG = MailTable.ToString()
                    Dim mailevent As String = en & "-" & SUBEVENT
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "INSERT_MAILLOG"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                    da.SelectCommand.Parameters.AddWithValue("@CC", cc)
                    da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
                    da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
                    da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
                    da.SelectCommand.Parameters.AddWithValue("@EID", eid)

                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim RES As Integer = da.SelectCommand.ExecuteScalar()
                    If RES > 0 Then
                        If MAILTO.Length > 1 Then
                            Try
                                Dim mail As New MailUtill(eid)
                                mail.SendMail(MAILTO, subject, MSG, "", "", "mayank.garg@myndsol.com;")
                            Catch ex As Exception
                                AutoRunLog("AutoRunSchedulerforDMS", "Files Created Mail getting error against with Email id" & Convert.ToString(DS.Tables("USERS").Rows(i)(0)), "TC Exception msg at TemplateCallingForMultipleTids -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), eid)
                            End Try

                        End If
                    End If

                End If
            Next
        Catch ex As Exception
            AutoRunLog("AutoRunSchedulerforDMS", "Folders Created Mail has been sent", "TC Exception msg at TemplateCallingForMultipleTids -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), eid)
        Finally
            con.Close()
            da.Dispose()
        End Try

    End Sub
    Public Sub notificationMailForMulitpleTids(ByVal uID As String, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String, ByVal DaysDiff As String)
        Try

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim STR As String = ""
            Dim MAILTO As String = ""
            Dim MAILID As String = ""
            Dim subject As String = ""
            Dim MSG As String = ""
            Dim cc As String = ""
            Dim Bcc As String = ""
            Dim MainEvent As String = ""
            Dim fn As String = ""
            'fill Product  
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            STR = "SELECT  FOLDERNAME [FOLDERNAME], dms.getUserName(oUID) [FOLDER OWNER], DMS.GetParent(1,Stid,Eid) [ABSOLUTE PATH], DMS.GetAuthorisedUser(1,Stid,gid,Eid) [CONCERN USERS], dms.getProjectName(gid) [PROJECT NAME], adate [UPLOAD DATE] FROM  MMM_MST_DOC with (nolock)"
            If STR.IndexOf("MMM_MST_USER") <> -1 Then
                STR &= " WHERE UID in (select * from  SplitString('" & uID & "',',') )"  ' uid changed to TID because on deletion and creation of file throw exception 
            Else
                STR &= " WHERE TID in (select * from  SplitString('" & uID & "',',') )"
            End If
            AutoRunLog("AutoRunSchedulerforDMS", "notificationMailForMulitpleTids", STR, eid)
            da.SelectCommand.CommandText = STR
            da.SelectCommand.CommandTimeout = 30000
            da.Fill(ds, "qry")
            If ds.Tables("qry").Rows.Count > 0 Then
                AutoRunLog("AutoRunSchedulerforDMS", "notificationMailForMulitpleTids", "Getting records based on folersids ", eid)
                For Each dc As DataColumn In ds.Tables("qry").Columns
                    fn = "{" & dc.ColumnName.ToString() & "}"
                    MSG = MSG.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    subject = subject.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    MAILTO = MAILTO.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    cc = cc.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                Next
            End If
            subject = "New Folders created in Myedms portal"
            da.SelectCommand.CommandText = "select emailid from mmm_mst_user  where eid=" & eid & " and MydmsEnableAlertsendmail='True' "
            da.Fill(ds, "USERS")
            Try
                For i As Integer = 0 To ds.Tables("USERS").Rows.Count - 1
                    ' AutoRunLog("AutoRunSchedulerforDMS", "notificationMailForMulitpleTids", "in For loop based on Users Emailids  at row  " & i, eid)
                    Dim dvPrograms As New DataView
                    dvPrograms = ds.Tables("qry").DefaultView
                    dvPrograms.RowFilter = "[CONCERN USERS] like '%" & Convert.ToString(ds.Tables("USERS").Rows(i)(0)) & "%'"
                    If dvPrograms.ToTable().Rows.Count > 0 Then
                        ' AutoRunLog("AutoRunSchedulerforDMS", "notificationMailForMulitpleTids", "in For loop based on Users Emailids matched at row  " & i & " with email id " & Convert.ToString(ds.Tables("USERS").Rows(i)(0)), eid)
                        MAILTO = Convert.ToString(ds.Tables("USERS").Rows(i)(0))
                        'MAILTO = "sunil.pareek@myndsol.com"
                        Dim MailTable As New StringBuilder()
                        MailTable.Append("<div style=""color:""#000000"">Dear Member</div>")
                        MailTable.Append("<br/>")
                        MailTable.Append("During last " & DaysDiff & " days, There are folders created by your group members in MYEDMS. ")
                        MailTable.Append("<br/>")

                        If dvPrograms.ToTable().Rows.Count > 12 Then
                            AutoRunLog("AutoRunSchedulerforDMS", "Folders having more than 30 records at row " & i, eid)
                            Dim filename As String = MyExtensions.AppendTimeStamp("MYEDMS.CSV")
                            AutoRunLog("AutoRunSchedulerforDMS", "Folders having more than 30 records at row " & i & " and folder created by " & filename, "", eid)
                            Try
                                Using writer As StreamWriter = New StreamWriter(Path.Combine(HttpRuntime.AppDomainAppPath, "DOCS/" & eid & "/" & filename))
                                    Rfc4180Writer.WriteDataTable(dvPrograms.ToTable(), writer, True)
                                    AutoRunLog("AutoRunSchedulerforDMS", "Folders having more than 30 records at row " & i & " and folder successfully created by " & filename, "", eid)
                                End Using
                            Catch ex As Exception
                                AutoRunLog("AutoRunSchedulerforDMS", "TC Exception msg at while creating file Inside email loop at row " & i & " -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), eid)
                            End Try

                            MailTable.Append("<div>  <a href=""https://myndsaas.com/DOCS/" & eid & "/" & filename & """ ><b>Click Here</b></a><b> to download and view the list of folders. </b></div>")
                        Else

                            AutoRunLog("AutoRunSchedulerforDMS", "Folders having less than 30 records at row " & i, "", eid)
                            MailTable.Append("<div><b> Following is Details of folders –</b></div>")
                            MailTable.Append("<br/>")
                            MailTable.Append("<table border=""1"" width=""100%"">")
                            MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")  ' for header only 
                            For l As Integer = 0 To ds.Tables("qry").Columns.Count - 1
                                MailTable.Append("<td>" & ds.Tables("qry").Columns(l).ColumnName & "</td>")
                            Next
                            For k As Integer = 0 To dvPrograms.ToTable().Rows.Count - 1 ' binding the tr tab in table
                                MailTable.Append("</tr><tr>") ' for row records
                                For t As Integer = 0 To dvPrograms.Table().Columns.Count - 1
                                    MailTable.Append("<td>" & dvPrograms.ToTable().Rows(k).Item(t).ToString() & " </td>")
                                Next
                                MailTable.Append("</tr>")
                            Next
                            MailTable.Append("</table>")
                        End If

                        MailTable.Append("<br/><br/>")
                        MailTable.Append("<div>")
                        MailTable.Append("If you want to see more information please")
                        MailTable.Append(" <a href=""https://myedms.myndsolution.com/"" ><b>Click Here</b></a>")
                        MailTable.Append("</div>")
                        MailTable.Append("<br/><br/>")
                        MailTable.Append("Regards ")
                        MailTable.Append("<br/><br/>")
                        MailTable.Append("Myedms Team")
                        MSG = MailTable.ToString()
                        Dim mailevent As String = en & "-" & SUBEVENT
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "INSERT_MAILLOG"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                        da.SelectCommand.Parameters.AddWithValue("@CC", "mayank.garg@myndsol.com")
                        da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
                        da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
                        da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
                        da.SelectCommand.Parameters.AddWithValue("@EID", eid)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim RES As Integer = da.SelectCommand.ExecuteScalar()
                        AutoRunLog("AutoRunSchedulerforDMS", "Folders having saved wtih id" & RES & "at row " & i, "", eid)
                        If RES > 0 Then
                            If MAILTO.Length > 1 Then
                                Try
                                    Dim mail As New MailUtill(eid)
                                    mail.SendMail(MAILTO, subject, MSG, "", "", "mayank.garg@myndsol.com;")
                                    AutoRunLog("AutoRunSchedulerforDMS", "Folders having mail sent saved wtih id" & RES & "at row " & i, "", eid)
                                Catch ex As Exception
                                    AutoRunLog("AutoRunSchedulerforDMS", "Folders having mail sent getting exception saved wtih id" & RES & "at row " & i, Regex.Replace(ex.Message.ToString, "[""']", String.Empty), eid)
                                End Try

                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                AutoRunLog("AutoRunSchedulerforDMS", "Folders Created Mail has been sent", "TC Exception msg at notification Inside email loop -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), eid)
            Finally
                con.Close()
                da.Dispose()
            End Try

        Catch ex As Exception

        End Try
    End Sub


    Public Sub DeletedAction(ByVal en As String, ByVal SUBEVENT As String, ByVal eid As Integer, ByVal DaysDiff As String)
        Dim MAILTO As String = ""
        Dim MAILID As String = ""
        Dim subject As String = ""
        Dim MSG As String = ""
        Dim cc As String = ""
        Dim Bcc As String = ""
        Dim MainEvent As String = ""
        Dim fn As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("declare @lastrun datetime=getdate() select @lastrun= lastrun from mmm_mst_entity  where eid=" & eid & " select  replace(a.actiondesc,' is Deleted','')  as [FOLDERNAME], b.username as [FOLDER OWNER],a.actiondate as [Deletion Time],b.emailid  as [CONCERN USERS] from mmm_mst_actionlog  as a inner join mmm_mst_user as b on a.uid=b.uid where a.actiontype='" & en & " " & SUBEVENT & "' and a.actiondate between  @lastrun and getdate()", con)
        Dim DS As New DataSet
        da.Fill(DS, "qry")
        If DS.Tables("qry").Rows.Count > 0 Then
            da.SelectCommand.CommandText = "select emailid from mmm_mst_user  where eid=" & eid & " and MydmsEnableAlertsendmail='True' "
            da.Fill(DS, "USERS")
            For i As Integer = 0 To DS.Tables("USERS").Rows.Count - 1
                Dim dvPrograms As New DataView
                dvPrograms = DS.Tables("qry").DefaultView
                dvPrograms.RowFilter = "[CONCERN USERS] like '%" & Convert.ToString(DS.Tables("USERS").Rows(i)(0)) & "%'"
                If dvPrograms.ToTable().Rows.Count > 0 Then
                    MAILTO = Convert.ToString(DS.Tables("USERS").Rows(i)(0))
                    'MAILTO = "sunil.pareek@myndsol.com"
                    subject = SUBEVENT & " " & en & " list"

                    Dim MailTable As New StringBuilder()
                    MailTable.Append("<div style=""color:""#000000"">Dear Member</div>")
                    MailTable.Append("<br/>")
                    MailTable.Append("During last " & DaysDiff & " days, There are " & en & " " & SUBEVENT & " by your group members in MYEDMS. ")
                    MailTable.Append("<br/>")

                    If dvPrograms.ToTable().Rows.Count > 12 Then

                        Dim filename As String = MyExtensions.AppendTimeStamp("MYEDMS.CSV")
                        Using writer As StreamWriter = New StreamWriter(Path.Combine(HttpRuntime.AppDomainAppPath, "DOCS/" & eid & "/" & filename))
                            Rfc4180Writer.WriteDataTable(dvPrograms.ToTable(), writer, True)
                        End Using
                        MailTable.Append("<div >  <a href=""https://myndsaas.com/DOCS/" & eid & "/" & filename & """ ><b>Click Here</b></a> <b> to download and view the list of " & en & ". </b></div>")
                    Else
                        MailTable.Append("<div><b> Following is Details of " & en & " –</b></div>")
                        MailTable.Append("<br/>")
                        MailTable.Append("<table border=""1"" width=""100%"">")

                        MailTable.Append("<br/>")
                        MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")  ' for header only 
                        For l As Integer = 0 To DS.Tables("qry").Columns.Count - 1
                            MailTable.Append("<td>" & DS.Tables("qry").Columns(l).ColumnName & "</td>")
                        Next
                        For k As Integer = 0 To dvPrograms.ToTable().Rows.Count - 1 ' binding the tr tab in table
                            MailTable.Append("</tr><tr>") ' for row records
                            For t As Integer = 0 To dvPrograms.Table().Columns.Count - 1
                                MailTable.Append("<td>" & dvPrograms.ToTable().Rows(k).Item(t).ToString() & " </td>")
                            Next
                            MailTable.Append("</tr>")
                        Next
                        MailTable.Append("</table>")
                    End If

                    MailTable.Append("<br/><br/>")
                    MailTable.Append("<div>")
                    MailTable.Append("If you want to see more information please")
                    MailTable.Append(" <a href=""https://myedms.myndsolution.com/"" ><b>Click Here</b></a>")
                    MailTable.Append("</div>")
                    MailTable.Append("<br/><br/>")
                    MailTable.Append("Regards ")
                    MailTable.Append("<br/><br/>")
                    MailTable.Append("Myedms Team")
                    MSG = MailTable.ToString()
                    Dim mailevent As String = en & "-" & SUBEVENT
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "INSERT_MAILLOG"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                    da.SelectCommand.Parameters.AddWithValue("@CC", cc)
                    da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
                    da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
                    da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
                    da.SelectCommand.Parameters.AddWithValue("@EID", eid)

                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim RES As Integer = da.SelectCommand.ExecuteScalar()
                    If RES > 0 Then
                        If MAILTO.Length > 1 Then
                            Dim mail As New MailUtill(eid)
                            mail.SendMail(MAILTO, subject, MSG, "", "", "mayank.garg@myndsol.com;")
                        End If
                    End If

                End If
            Next
        End If
    End Sub


    Public Class Rfc4180Writer
        Public Shared Sub WriteDataTable(ByVal sourceTable As DataTable, ByVal writer As TextWriter, ByVal includeHeaders As Boolean)
            If (includeHeaders) Then
                Dim headerValues As IEnumerable(Of String) = sourceTable.Columns.OfType(Of DataColumn)().Select(Function(column) QuoteValue(column.ColumnName))
                writer.WriteLine(String.Join(",", headerValues))
            End If

            Dim items As IEnumerable(Of String) = Nothing
            For Each row As DataRow In sourceTable.Rows
                items = row.ItemArray.Select(Function(obj) QuoteValue(obj.ToString()))
                writer.WriteLine(String.Join(",", items))
            Next
            writer.Flush()
        End Sub

        Private Shared Function QuoteValue(ByVal value As String) As String
            Return String.Concat("""", value.Replace("""", """"""), """")
        End Function
    End Class
    Public Class MyExtensions
        Public Shared Function AppendTimeStamp(ByVal fileName As String) As String
            Return String.Concat(Path.GetFileNameWithoutExtension(fileName), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName))
        End Function
    End Class

    Public Class Apperrors
        Dim EID As Integer
        Dim UID As Integer
        Dim MessHeading As String
        Dim erroMess As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

        Public Sub New(EID As Integer, UID As Integer, MessHeading As String, erroMess As String)
            Try
                Using con = New SqlConnection(conStr)
                    Dim qry As String = "insert into mmm_mst_apperror (eid,UID,MessHeading,erroMess,LogTime) values (" & EID & "," & UID & ",'" & MessHeading & "','" & erroMess & "',getdate() )"
                    Using cmd = New SqlCommand(qry, con)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim result As Integer
                        result = Convert.ToInt32(cmd.ExecuteNonQuery())
                        con.Dispose()
                        cmd.Dispose()
                    End Using
                End Using
            Catch ex As Exception
                Throw
            Finally

            End Try


        End Sub

    End Class

    '' new for hcl - appr info ws  -  ' for hcl by sp - Apr_2017
    Function getPearlApprovalPendency() As List(Of Hcl_apprvl_pending) Implements IDMSService.getPearlApprovalPendency
        Dim lstpending As New List(Of Hcl_apprvl_pending)
        Dim ds As New DataSet()
        Dim objM As New Hcl_apprvl_pending()
        Try
            Dim strquery As String = "	select d.fld47 [BPMID], dms.udf_split('MASTER-Invoice Type Master-fld1',d.fld96)[Invoice_Type],(select fld1 from mmm_mst_master with(nolock) where documenttype='Vendor' and convert(nvarchar,tid)=d.fld17)[Vendor_Name],    d.fld10[Invoice_No],d.fld11[Invoice_Date], d.fld20[Invoice_Amount_WO_Tax], datediff(day,fdate,getdate())[Pending_Days],convert(datetime,d.adate) [Creation_Date], (select userid from mmm_mst_user where uid in(select top 1 userid from mmm_doc_dtl with(nolock) where docid=d.tid order by tid desc))[Ecode], (select username from mmm_mst_user where uid in(select top 1 userid from mmm_doc_dtl with(nolock) where docid=d.tid order by tid desc))[Emp_Name], case d.curstatus when 'DISPATCH' THEN 'Pending for Dispatch by Submitter' 	when 'Dispatch' THEN 'Pending for Dispatch by Submitter'	when 'Physical' THEN 'Pending with Vendor Helpdesk'	when 'Approver 1' THEN 'Level-1 Approval Pending'	when 'Approver 2' THEN 'Level-2 Approval Pending'	when 'Approver 3' THEN 'Level-3 Approval Pending'	when 'Approver 4' THEN 'Level-4 Approval Pending'	when 'Approver 5' THEN 'Level-5 Approval Pending'	when 'GR Service Entry' THEN 'GRN SE in SAP Pending'	when 'SAP ID Update' THEN 'IV Pending'	when 'SAP ID Posted' THEN 'IV Pending'	when 'QC' THEN 'IV Done (Pending for QC)'	when 'Archive' THEN 'Processed in Pearl (Archive)'	when 'REJECTED' THEN 'Rejected / Cancelled'	when 'UPLOADED' THEN 'Pending with Submitter' 	else d.curstatus end [Status_In_Pearl], (select username from MMM_MST_USER with(nolock) where uid=d.fld119)[L1_Approver],(select username from MMM_MST_USER with(nolock) where uid=d.fld120)[L2_Approver], (select username from MMM_MST_USER with(nolock) where uid=d.fld161)[L3_Approver],(select username from MMM_MST_USER with(nolock) where uid=d.fld162)[L4_approver], (select username from MMM_MST_USER with(nolock) where uid=d.fld200)[L5_Approver] From mmm_mst_doc d with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL Dt with (nolock) on Dt.tid=d.lasttid  where cast(d.tid as nvarchar(10)) in (SELECT doc.tid   from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid      where  Doc.DocumentType='Vendor Invoice VP' and doc.EID=46 and doc.curstatus IN ('Approver 1','Approver 2','Approver 3','Approver 4','Approver 5') and dtl.aprstatus is null  and userid in ( select userid from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid  where  Doc.DocumentType='Vendor Invoice VP' and doc.EID=46 and doc.curstatus in ('Approver 1','Approver 2','Approver 3','Approver 4','Approver 5') and dtl.aprstatus is null and doc.fld96 in ('958564','958565','1078985')  group by userid ) )"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(strquery, con)
                    da.SelectCommand.CommandType = CommandType.Text
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using

            If ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    objM = New Hcl_apprvl_pending()
                    objM.Bpmid = Convert.ToString(ds.Tables(0).Rows(i).Item("Bpmid"))
                    objM.Invoice_Type = Convert.ToString(ds.Tables(0).Rows(i).Item("Invoice_Type"))
                    objM.Vendor_Name = Convert.ToString(ds.Tables(0).Rows(i).Item("Vendor_Name"))
                    objM.Invoice_No = Convert.ToString(ds.Tables(0).Rows(i).Item("Invoice_No"))
                    objM.Invoice_Amount_WO_Tax = Convert.ToString(ds.Tables(0).Rows(i).Item("Invoice_Amount_WO_Tax"))
                    objM.Pending_Days = Convert.ToString(ds.Tables(0).Rows(i).Item("Pending_Days"))
                    objM.Creation_Date = Convert.ToString(ds.Tables(0).Rows(i).Item("Creation_Date"))
                    objM.L1_Approver = Convert.ToString(ds.Tables(0).Rows(i).Item("L1_Approver"))
                    objM.L2_Approver = Convert.ToString(ds.Tables(0).Rows(i).Item("L2_Approver"))
                    objM.L3_Approver = Convert.ToString(ds.Tables(0).Rows(i).Item("L3_Approver"))
                    objM.L4_Approver = Convert.ToString(ds.Tables(0).Rows(i).Item("L4_Approver"))
                    objM.Ecode = Convert.ToString(ds.Tables(0).Rows(i).Item("Ecode"))
                    objM.Emp_Name = Convert.ToString(ds.Tables(0).Rows(i).Item("Emp_Name"))
                    objM.Status_In_Pearl = Convert.ToString(ds.Tables(0).Rows(i).Item("Status_In_Pearl"))
                    lstpending.Add(objM)
                Next
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstpending
    End Function


    Public Sub TemplateCalling(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String)
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim constr As String = "server=172.16.20.3\SQLDEV;initial catalog=DMS;uid=DMS;pwd=Gtr$698B#"

        Dim con As New SqlConnection(conStr)
        Dim STR As String = ""
        Dim MAILTO As String = ""
        Dim MAILID As String = ""
        Dim subject As String = ""
        Dim MSG As String = ""
        Dim cc As String = ""
        Dim Bcc As String = ""
        Dim MainEvent As String = ""
        Dim fn As String = ""

        Dim da As SqlDataAdapter = New SqlDataAdapter("select documenttype,curstatus from MMM_MST_DOC where tid=" & tid, con)
        Dim DS As New DataSet
        Try
            'Dim WFstatus As String = "NOT REQUIRED"
            'da.Fill(DS, "doctype")
            'If en.Length < 2 Then
            '    en = DS.Tables("doctype").Rows(0).Item("documenttype").ToString()
            'End If
            'If DS.Tables("doctype").Rows.Count <> 0 Then
            '    WFstatus = DS.Tables("doctype").Rows(0).Item("curstatus").ToString()
            'End If
            'If SUBEVENT = "REJECT" Then
            '    WFstatus = "REJECTED"
            'End If

            't.statusname parameter is deleted from the sql string
            da.SelectCommand.CommandText = "select T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' and EID=" & eid & " "
            da.Fill(DS, "TEMP")
            If DS.Tables("TEMP").Rows.Count > 0 Then
                MSG = System.Web.HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = DS.Tables("TEMP").Rows(0).Item("qry").ToString()
            Else
                't.statusname parameter is deleted from the sql string
                da.SelectCommand.CommandText = "select T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND  EID=0 "
                da.Fill(DS, "TEMP")
                If DS.Tables("TEMP").Rows.Count <> 0 Then
                    MSG = System.Web.HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                    subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                    MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                    cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                    Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                    MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                    STR = DS.Tables("TEMP").Rows(0).Item("qry")
                End If
            End If

            If DS.Tables("TEMP").Rows.Count <> 0 Then
                STR &= " WHERE TID=" & tid & ""
                da.SelectCommand.CommandText = STR
                da.Fill(DS, "qry")

                For Each dc As DataColumn In DS.Tables("qry").Columns
                    fn = "{" & dc.ColumnName.ToString() & "}"
                    MSG = MSG.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    subject = subject.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    MAILTO = MAILTO.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    cc = cc.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                Next
                If cc.ToString.ToUpper.Contains("{ADMIN}") Then
                    da.SelectCommand.CommandText = "select distinct (select emailID from MMM_MST_USER with(nolock) where uid in (R.uid)  )[emailid] from MMM_MST_USER U with(nolock) left outer join MMM_Ref_Role_User R with(nolock) on U.EID=R.eid and U.fld15=R.fld4 where U.emailid in ('" & MAILTO & "') and r.rolename='admin' and r.fld4 like '%'+u.fld15+'%' order by emailid "
                    da.SelectCommand.CommandType = CommandType.Text
                    Dim DTADMIN As New DataTable
                    da.Fill(DTADMIN)
                    Dim ccadmin As String = ""
                    For i As Integer = 0 To DTADMIN.Rows.Count - 1
                        ccadmin = ccadmin & DTADMIN.Rows(i).Item("emailid").ToString & ","
                        ccadmin = Left(ccadmin, ccadmin.Length - 1)
                        cc = Replace(cc, "{Admin}", ccadmin)
                    Next
                End If
                Dim mailevent As String = en & "-" & SUBEVENT
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandText = "INSERT_MAILLOG"
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                da.SelectCommand.Parameters.AddWithValue("@CC", cc)
                da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
                da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT MAIL")
                da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
                da.SelectCommand.Parameters.AddWithValue("@EID", eid)

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim RES As Integer = da.SelectCommand.ExecuteScalar()
                If RES > 0 Then
                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                    ' sendMail1(MAILTO, cc, Bcc, subject, MSG)
                    Dim obj As New MailUtill(eid:=eid)
                    obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
                End If
            End If

        Catch ex As Exception
        Finally
            DS.Dispose()
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub

    Private Sub SendAlertMails()
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtAlert As New DataTable()
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim supervisoremail As String = ""
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select * from MMM_MST_Template where action=1"  '2629,2630,2631

            da.Fill(dtAlert)
            If dtAlert.Rows.Count > 0 Then
                For i As Integer = 0 To dtAlert.Rows.Count - 1
                    Dim curEid As Integer = dtAlert.Rows(i).Item("EID").ToString()
                    Dim CurTID As Integer = dtAlert.Rows(i).Item("tid").ToString
                    Dim Bsla As Integer = dtAlert.Rows(i).Item("bsladay").ToString
                    Dim Asla As Integer = dtAlert.Rows(i).Item("Asladay").ToString
                    Dim WFstatus As String = dtAlert.Rows(i).Item("statusname").ToString
                    Dim CurEventNm As String = dtAlert.Rows(i).Item("EventName").ToString
                    Dim SubEventNm As String = dtAlert.Rows(i).Item("SubEvent").ToString
                    Dim supervisorFLD As String = dtAlert.Rows(i).Item("userfield").ToString

                    Dim STR As String = "", MAILTO As String = "", MAILID As String = "", subject As String = "", MSGMain As String = "", cc As String = "", Bcc As String = ""
                    Dim fn As String = "", MSG As String
                    Dim DS As New DataSet

                    MSGMain = System.Web.HttpUtility.HtmlDecode(dtAlert.Rows(i).Item("msgbody").ToString())
                    subject = dtAlert.Rows(i).Item("SUBJECT").ToString()
                    cc = dtAlert.Rows(i).Item("CC").ToString()
                    Bcc = dtAlert.Rows(i).Item("BCC").ToString()
                    STR = dtAlert.Rows(i).Item("qry").ToString()


                    cc = dtAlert.Rows(i).Item("CC").ToString()
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "Sp_RoleToEmailID"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("RoleNames", cc)
                    da.SelectCommand.Parameters.AddWithValue("Eid", curEid)
                    Dim dtcc As New DataTable()
                    da.Fill(dtcc)
                    Dim acc As New ArrayList()

                    If dtcc.Rows.Count > 0 Then
                        For q As Integer = 0 To dtcc.Rows.Count - 1
                            If Convert.ToString(dtcc.Rows(q)(0)) <> "" Then
                                acc.Add(Convert.ToString(dtcc.Rows(q)(0)))
                            End If
                        Next
                        If acc.Count > 0 Then
                            cc = ""
                            cc = cc & String.Join(",", acc.ToArray())
                        End If
                    End If

                    If SchedulerOld(CurTID) = True Then
                        Dim dtUsers As New DataTable

                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If

                        da.SelectCommand.Parameters.Clear()
                        ' da.SelectCommand.CommandText = "select userid from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid inner join MMM_MOVPATH_DOC mvDoc on dtl.pathID=mvDoc.tid where  Doc.DocumentType='" & CurEventNm & "' and doc.EID=" & curEid & " and doc.curstatus='" & WFstatus & "' and dtl.aprstatus is null  and  case when ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) >" & Bsla & ") and ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) <" & Asla & ") then 'TRUE' ELSE 'FALSE' END ='TRUE' group by userid order by userid "
                        da.SelectCommand.CommandText = "uspAlertMailSLA_getAllUsers"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("en", CurEventNm)
                        da.SelectCommand.Parameters.AddWithValue("ws", WFstatus)
                        da.SelectCommand.Parameters.AddWithValue("bsla", Bsla)
                        da.SelectCommand.Parameters.AddWithValue("asla", Asla)
                        da.SelectCommand.Parameters.AddWithValue("eid", curEid)
                        da.Fill(dtUsers)

                        ' Call UpdateLoginDB("Time matched for Template : " & CurEventNm & "-" & WFstatus) -- removed on 12_may

                        For k As Integer = 0 To dtUsers.Rows.Count - 1
                            If Not IsDBNull(dtUsers.Rows(k).Item("userid")) Then
                                Dim curUser As Integer = dtUsers.Rows(k).Item("userid").ToString()
                                Dim dtQry As New DataTable
                                Dim dtDtl As New DataTable
                                Dim dtDocs As New DataTable
                                Dim dtMail As New DataTable
                                Dim StrQry As String = ""
                                MSG = MSGMain

                                da.SelectCommand.Parameters.Clear()
                                'da.SelectCommand.CommandText = "select DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvdoc.SLA*24 [curDt_diff_sla], mvdoc.sla [SLA], (mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24)  [act_hour], DATEADD(day,mvDoc.sla ,fdate) [ExpireDt],  doc.tid, dtl.*  from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid   inner join MMM_MOVPATH_DOC mvDoc on dtl.pathID=mvDoc.tid where  Doc.DocumentType='" & CurEventNm & "' and doc.EID=" & curEid & " and doc.curstatus='" & WFstatus & "' and dtl.aprstatus is null  and  case when ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) >" & Bsla & ") and ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) <" & Asla & ") then 'TRUE' ELSE 'FALSE' END ='TRUE' and userid=" & curUser & " order by docid "
                                da.SelectCommand.CommandText = "uspAlertMailSLA_getUserDocID"
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.AddWithValue("en", CurEventNm)
                                da.SelectCommand.Parameters.AddWithValue("ws", WFstatus)
                                da.SelectCommand.Parameters.AddWithValue("bsla", Bsla)
                                da.SelectCommand.Parameters.AddWithValue("asla", Asla)
                                da.SelectCommand.Parameters.AddWithValue("eid", curEid)
                                da.SelectCommand.Parameters.AddWithValue("curuser", curUser)
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.Fill(dtDocs)

                                ' get docid's separated by comma 
                                da.SelectCommand.Parameters.Clear()
                                'da.SelectCommand.CommandText = "select SUBSTRING( ( SELECT ',' +   convert(nvarchar(20),doc.tid)  from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid   inner join MMM_MOVPATH_DOC mvDoc on dtl.pathID=mvDoc.tid where  Doc.DocumentType='" & CurEventNm & "' and doc.EID=" & curEid & " and doc.curstatus='" & WFstatus & "' and dtl.aprstatus is null  and  case when ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) >" & Bsla & ") and ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) <" & Asla & ") then 'TRUE' ELSE 'FALSE' END ='TRUE' and userid=" & curUser & " order by doc.tid FOR XML PATH('')), 2,5000) AS ColList"
                                da.SelectCommand.CommandText = "uspAlertMailSLA_getUserIDList"
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.AddWithValue("en", CurEventNm)
                                da.SelectCommand.Parameters.AddWithValue("ws", WFstatus)
                                da.SelectCommand.Parameters.AddWithValue("bsla", Bsla)
                                da.SelectCommand.Parameters.AddWithValue("asla", Asla)
                                da.SelectCommand.Parameters.AddWithValue("eid", curEid)
                                da.SelectCommand.Parameters.AddWithValue("curuser", curUser)
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.Fill(dtDtl)

                                Dim ColList As String = ""
                                If dtDtl.Rows.Count <> 0 Then
                                    ColList = dtDtl.Rows(0).Item(0).ToString()
                                End If

                                StrQry = STR & " WHERE TID in (" & ColList & ")"
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.CommandText = StrQry
                                da.Fill(dtQry)

                                'fn = "{@TABLE}"
                                Dim Spos As Integer = MSG.IndexOf("{@TABLE}")
                                Dim Epos As Integer = MSG.IndexOf("{@/TABLE}")

                                Dim TblText As String = MSG.Substring(Spos + 8, (Epos - (Spos + 8)))
                                Dim ArrVars() As String = TblText.Split("|")

                                Dim MailBody As String = "<table width=""100%"" border=""3px"" cellpadding=""2px"" border=""1"" cellspacing=""2px""> <tr style=""background-color:#87CEFA""> "
                                Dim sI As Integer = 0, eI As Integer = 0
                                For m As Integer = 0 To ArrVars.Length - 1
                                    sI = ArrVars(m).IndexOf("{")
                                    eI = ArrVars(m).IndexOf("}")
                                    ArrVars(m) = ArrVars(m).Substring(sI + 1, (eI - sI) - 1)
                                    MailBody &= "<td> <font face=""arial, helvetica, sans-serif"" size=""2"">  " & ArrVars(m).ToString & " </font> </td>"
                                Next
                                MailBody &= " </tr> "

                                For j As Integer = 0 To dtQry.Rows.Count - 1 '''' this is for no. of rows of docs - outer most
                                    MailBody &= "<tr> "
                                    For m As Integer = 0 To ArrVars.Length - 1
                                        'Dim Colnm As String = ArrVars(m).Substring(1, ArrVars(m).Length - 1)
                                        Dim Colnm As String = ArrVars(m)
                                        If dtQry.Columns.Contains(Colnm) Then
                                            MailBody &= "<td> <font face=""arial, helvetica, sans-serif"" size=""2""> " & "" & dtQry.Rows(j).Item(Colnm).ToString() & "</font></td>"
                                        End If
                                    Next
                                    MailBody &= "</tr>"
                                Next
                                MailBody &= "</table>"

                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If

                                Dim UserName As String = ""
                                '' select - get mail id of current user
                                If supervisorFLD.Length > 0 Then
                                    da.SelectCommand.CommandText = "select  emailid," & supervisorFLD & ",username from mmm_mst_user where eid=" & curEid & " and uid=" & curUser
                                    da.Fill(dtMail)
                                    If dtMail.Rows.Count <> 0 Then
                                        MAILTO = dtMail.Rows(0).Item("emailid").ToString
                                        UserName = dtMail.Rows(0).Item("username").ToString
                                        da.SelectCommand.CommandText = "Select emailid from mmm_mst_user where eid=" & curEid & " and uid=" & dtMail.Rows(0).Item(supervisorFLD).ToString() & ""
                                        supervisoremail = da.SelectCommand.ExecuteScalar()
                                        If cc <> "" Then
                                            cc = cc & ";" & supervisoremail
                                        Else
                                            cc = supervisoremail
                                        End If

                                    End If
                                Else
                                    da.SelectCommand.CommandText = "select emailid,username from mmm_mst_user where eid=" & curEid & " and uid=" & curUser
                                    da.Fill(dtMail)
                                    If dtMail.Rows.Count <> 0 Then
                                        MAILTO = dtMail.Rows(0).Item("emailid").ToString
                                        UserName = dtMail.Rows(0).Item("username").ToString
                                    End If
                                End If

                                '' select - get mail id of current user
                                'da.SelectCommand.CommandText = "select emailid from mmm_mst_user where eid=" & curEid & " and uid=" & curUser
                                'da.Fill(dtMail)
                                'If dtMail.Rows.Count <> 0 Then
                                '    MAILTO = dtMail.Rows(0).Item("emailid").ToString
                                'End If

                                '' code here to replace between {@TABLE} and {@/TABLE}
                                Spos = MSG.IndexOf("{@TABLE}")
                                Epos = MSG.IndexOf("{@/TABLE}")
                                Dim repVal As String = MSG.Substring(Spos, ((Epos + 9) - Spos))
                                MSG = MSG.Replace(repVal, MailBody)
                                '' new aded line by sunil 03-aug-18 
                                MSG = MSG.Replace("{CURRENT USERNAME}", UserName)

                                '' send mail here 
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = "INSERT_MAILLOG"
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                da.SelectCommand.Parameters.AddWithValue("@CC", cc)
                                da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
                                da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT MAIL")
                                da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", CurEventNm & "-" & WFstatus)
                                da.SelectCommand.Parameters.AddWithValue("@EID", curEid)

                                Dim RES As Integer = da.SelectCommand.ExecuteScalar()
                                If RES > 0 Then
                                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                    'sendMail1(MAILTO, cc, Bcc, subject, MSG)
                                    Dim obj As New MailUtill(eid:=curEid)
                                    obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
                                End If
                                dtDocs.Dispose()
                                dtQry.Dispose()
                                dtDtl.Dispose()
                                dtMail.Dispose()
                            End If
                        Next
                        dtUsers.Dispose()
                    Else
                    End If
                Next
            End If

            ' Call UpdateLoginDB("Exits from Sendalertmail function (Live)") -- removed on 12_may_14

            'lblmsg.Text = "Action Successful"


        Catch ex As Exception
            Call UpdateLoginDB("Error in Sendalertmail function msg is - " & ex.InnerException.Message)
        Finally
            dtAlert.Dispose()
            con.Close()
            da.Dispose()
        End Try
    End Sub

    '' code for report scheduler templates calling by vishal 
    Sub ReportSchedulerMail()
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/import/")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim alertname As String = ""
        Try
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler with(nolock) where isActive='1' and tid >=491  order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                Dim sendtype As String = ds.Tables("rpt").Rows(d).Item("sendtype").ToString()
                eid = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                alertname = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                    If sendtype.ToString.ToUpper = "EXCEL SHEET" Then
                        Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                        Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                        Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                        Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                        Dim role As String = ds.Tables("rpt").Rows(d).Item("sendto").ToString()
                        Dim reporttype As String = ds.Tables("rpt").Rows(d).Item("reporttype").ToString()
                        Dim dt As String = ds.Tables("rpt").Rows(d).Item("date").ToString()
                        eid = ds.Tables("rpt").Rows(d).Item("EID").ToString()

                        Dim qry As String
                        If role <> "" Then
                            role = Replace(role, ",", "','")
                            oda.SelectCommand.CommandText = " select * from mmm_mst_user where uid in (select distinct uid from mmm_ref_role_user where eid=" & eid & " and rolename in ('" & role & "') and documenttype like '%invoice%') "
                            oda.Fill(ds, "User")
                            'Dim lastschtime As String = Format(Convert.ToDateTime(ds.Tables("rpt").Rows(d).Item("lastscheduleddate").ToString), "yyyy-MM-dd")
                            'Dim curtime As String = Date.Now.ToString("yyyy-MM-dd")
                            'If lastschtime = curtime Then
                            'End If
                            oda.SelectCommand.CommandText = "select FormName,DocMapping from mmm_mst_forms where eid=" & eid & " and DocMapping is not null"
                            oda.Fill(ds, "DocMapping")
                            If ds.Tables("DocMapping").Rows.Count > 0 Then
                                oda.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & eid & " and dropdown like '%" & ds.Tables("DocMapping").Rows(0).Item("FormName") & "%' and documenttype in ('PES','Invoice PO','Invoice Non PO','Invoice on Hold')"
                                oda.Fill(ds, "Fieldmapping")
                            End If

                            For u As Integer = 0 To ds.Tables("User").Rows.Count - 1
                                If Len(MAILTO) < 3 Then
                                    MAILTO = ds.Tables("User").Rows(u).Item("EmailID").ToString
                                End If
                                qry = str.ToUpper
                                qry = qry.ToString.ToUpper
                                oda.SelectCommand.CommandText = "select " & ds.Tables("DocMapping").Rows(0).Item("DocMapping") & " from MMM_Ref_Role_User where eid=" & eid & " and uid=" & ds.Tables("User").Rows(u).Item("uid") & " and rolename in ('" & role & "') and documenttype like '%invoice%'"
                                Dim id As String = ""
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.CommandType = CommandType.Text
                                id = oda.SelectCommand.ExecuteScalar().ToString
                                For dm As Integer = 0 To ds.Tables("Fieldmapping").Rows.Count - 1
                                    If ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE PO'", "Documenttype='Invoice PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE PO')", "Documenttype in ('Invoice PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE NON PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE NON PO'", "Documenttype='Invoice non PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE NON PO')", "Documenttype in ('Invoice non PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE ON HOLD" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE ON HOLD'", "Documenttype='Invoice on Hold' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE ON HOLD')", "Documenttype in ('Invoice on Hold') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "PES" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='PES'", "Documenttype='PES' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('PES')", "Documenttype in ('PES') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    End If
                                Next
                                If reporttype.ToUpper = "MONTHLY" Then
                                    qry = Replace(qry.ToString.ToUpper, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                ElseIf reporttype.ToUpper = "WEEKLY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                ElseIf reporttype.ToUpper = "DAILY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                Else
                                    'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                                End If
                                oda.SelectCommand.CommandText = qry
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandTimeout = 1200
                                Dim Common As New System.Data.DataTable
                                oda.Fill(Common)
                                If Common.Rows.Count > 0 Then
                                    Dim CNT As Integer = 0
                                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                    oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "Vishal.kumar@myndsol.com")
                                    oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                    oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                    oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                    oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If
                                    oda.SelectCommand.ExecuteNonQuery()


                                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                    Dim fname As String = ""
                                    fname = CreateCSVR(Common, FPath, ds.Tables("User").Rows(u).Item("uid"))
                                    Dim obj As New MailUtill(eid:=eid)
                                    obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, Attachments:=FPath + fname, BCC:=Bcc)
                                    'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, FPath + fname)
                                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                    ' sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, FPath + fname)
                                    'lblMsg1.Text = "Report Scheduled...."
                                End If
                                MAILTO = ""
                            Next
                        Else
                            qry = str
                            qry = qry.ToString.ToUpper
                            If reporttype.ToUpper = "MONTHLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                            ElseIf reporttype.ToUpper = "WEEKLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                            ElseIf reporttype.ToUpper = "DAILY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                            Else
                                'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                            End If
                            oda.SelectCommand.CommandText = qry
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandTimeout = 1200
                            Dim Common As New System.Data.DataTable
                            oda.Fill(Common)
                            If Common.Rows.Count > 0 Then
                                Dim CNT As Integer = 0
                                Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.ExecuteNonQuery()

                                Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                Dim fname As String = ""
                                fname = CreateCSVR(Common, FPath, eid)
                                Dim obj As New MailUtill(eid:=eid)
                                obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, Attachments:=FPath + fname, BCC:=Bcc)
                                'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, FPath + fname)
                                '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                '  sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, FPath + fname)
                                'lblMsg1.Text = "Report Scheduled...."
                            End If
                        End If
                    ElseIf sendtype.ToString.ToUpper = "MAILBODY" Then
                        Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                        Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                        Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                        Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                        Dim role As String = ds.Tables("rpt").Rows(d).Item("sendto").ToString()
                        Dim reporttype As String = ds.Tables("rpt").Rows(d).Item("reporttype").ToString()
                        Dim dt As String = ds.Tables("rpt").Rows(d).Item("date").ToString()
                        eid = ds.Tables("rpt").Rows(d).Item("EID").ToString()
                        Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                        Dim qry As String
                        If role <> "" Then
                            role = Replace(role, ",", "','")
                            oda.SelectCommand.CommandText = " select * from mmm_mst_user where uid in (select distinct uid from mmm_ref_role_user where eid=" & eid & " and rolename in ('" & role & "') and documenttype like '%invoice%') "
                            oda.Fill(ds, "User")

                            'Dim lastschtime As String = Format(Convert.ToDateTime(ds.Tables("rpt").Rows(d).Item("lastscheduleddate").ToString), "yyyy-MM-dd")
                            'Dim curtime As String = Date.Now.ToString("yyyy-MM-dd")
                            'If lastschtime = curtime Then
                            '    Exit Sub
                            'End If
                            oda.SelectCommand.CommandText = "select FormName,DocMapping from mmm_mst_forms where eid=" & eid & " and DocMapping is not null"
                            oda.Fill(ds, "DocMapping")
                            If ds.Tables("DocMapping").Rows.Count > 0 Then
                                oda.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & eid & " and dropdown like '%" & ds.Tables("DocMapping").Rows(0).Item("FormName") & "%' and documenttype in ('PES','Invoice PO','Invoice Non PO','Invoice on Hold')"
                                oda.Fill(ds, "Fieldmapping")
                            End If

                            For u As Integer = 0 To ds.Tables("User").Rows.Count - 1
                                If Len(MAILTO) < 3 Then
                                    MAILTO = ds.Tables("User").Rows(u).Item("EmailID").ToString
                                End If
                                qry = str.ToUpper
                                qry = qry.ToString.ToUpper
                                oda.SelectCommand.CommandText = "select " & ds.Tables("DocMapping").Rows(0).Item("DocMapping") & " from MMM_Ref_Role_User where eid=" & eid & " and uid=" & ds.Tables("User").Rows(u).Item("uid") & " and rolename in ('" & role & "') and documenttype like '%invoice%'"
                                Dim id As String = ""
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.CommandType = CommandType.Text
                                id = oda.SelectCommand.ExecuteScalar().ToString
                                For dm As Integer = 0 To ds.Tables("Fieldmapping").Rows.Count - 1
                                    If ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE PO'", "Documenttype='Invoice PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE PO')", "Documenttype in ('Invoice PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE NON PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE NON PO'", "Documenttype='Invoice non PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE NON PO')", "Documenttype in ('Invoice non PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE ON HOLD" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE ON HOLD'", "Documenttype='Invoice on Hold' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE ON HOLD')", "Documenttype in ('Invoice on Hold') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "PES" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='PES'", "Documenttype='PES' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('PES')", "Documenttype in ('PES') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    End If
                                Next
                                If reporttype.ToUpper = "MONTHLY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                ElseIf reporttype.ToUpper = "WEEKLY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                ElseIf reporttype.ToUpper = "DAILY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                Else
                                    'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                                End If
                                oda.SelectCommand.CommandText = qry
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandTimeout = 1200

                                Dim Common As New System.Data.DataTable
                                oda.Fill(Common)
                                If Common.Rows.Count > 0 Then
                                    Dim CNT As Integer = 0
                                    oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                    oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                    oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                    oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                    oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If
                                    oda.SelectCommand.ExecuteNonQuery()

                                    Dim MailTable As New StringBuilder()
                                    MailTable.Append("<table border=""1"" width=""100%"">")
                                    MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True"">  ")

                                    For l As Integer = 0 To Common.Columns.Count - 1
                                        If Common.Columns(l).ColumnName.ToUpper = "CURRENTSTATUS" Then
                                            MailTable.Append("<td > CURRENT STATUS </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT1TO7DAYS" Then
                                            MailTable.Append("<td > COUNT 1-7 DAYS </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT1TO7DAYS" Then
                                            MailTable.Append("<td > INVOICE AMOUNT 1-7 DAYS(LACS) </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT8TO14DAYS" Then
                                            MailTable.Append("<td > COUNT 8-14 DAYS</td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT8TO14DAYS" Then
                                            MailTable.Append("<td > INVOICE AMOUNT 8-14 DAYS(LACS) </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT15DAYS" Then
                                            MailTable.Append("<td > COUNT 15+ DAYS </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT15DAYS" Then
                                            MailTable.Append("<td > INVOICE AMOUNT 15+ DAYS(LACS) </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALBARCODECOUNT" Then
                                            MailTable.Append("<td > TOTAL BAR CODE COUNT </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALINVOICEAMOUNT" Then
                                            MailTable.Append("<td > TOTAL INVOICE AMOUNT(LACS) </td>")
                                        Else
                                            MailTable.Append("<td > " & Common.Columns(l).ColumnName & " </td>")
                                        End If
                                    Next
                                    'For l As Integer = 0 To Common.Columns.Count - 1
                                    '    MailTable.Append("<td >  " & Common.Columns(l).ColumnName & " </td>")
                                    'Next
                                    '#DCDCDC
                                    For k As Integer = 0 To Common.Rows.Count - 1 ' binding the tr tab in table
                                        If (Common.Rows(k).Item(0).ToString.Contains("TOTAL") = True) Then
                                            MailTable.Append("</tr><tr style=""background-color:#DCDCDC"">") ' for row records
                                        Else
                                            MailTable.Append("</tr><tr>") ' for row records
                                        End If
                                        For t As Integer = 0 To Common.Columns.Count - 1
                                            If IsNumeric(Common.Rows(k).Item(t).ToString()) = True Then
                                                MailTable.Append("<td align=""right""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                            Else
                                                MailTable.Append("<td align=""left""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                            End If
                                        Next
                                        MailTable.Append(" </tr>")
                                    Next
                                    MailTable.Append("</table>")

                                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                    Dim strmsgBdy As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                    If strmsgBdy.Contains("@body") Then
                                        strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                        msg = strmsgBdy
                                    Else
                                        msg = MailTable.ToString()
                                    End If
                                    msg = strmsgBdy

                                    'sendMail1(MAILTO, CC, Bcc, mailsub, msg)
                                    Dim obj As New MailUtill(eid:=eid)
                                    obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, BCC:=Bcc)
                                    'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, "")
                                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                    ' sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, "")
                                    'lblMsg1.Text = "Report Scheduled...."
                                End If
                                MAILTO = ""
                            Next
                        Else
                            qry = str
                            qry = qry.ToString.ToUpper
                            If reporttype.ToUpper = "MONTHLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                            ElseIf reporttype.ToUpper = "WEEKLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                            ElseIf reporttype.ToUpper = "DAILY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                            Else
                                'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                            End If
                            oda.SelectCommand.CommandText = qry
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandTimeout = 1200

                            Dim Common As New System.Data.DataTable
                            oda.Fill(Common)
                            If Common.Rows.Count > 0 Then
                                Dim CNT As Integer = 0
                                oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.ExecuteNonQuery()

                                Dim MailTable As New StringBuilder()
                                MailTable.Append("<table border=""1"" width=""100%"">")
                                MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True"">")

                                For l As Integer = 0 To Common.Columns.Count - 1
                                    If Common.Columns(l).ColumnName.ToUpper = "CURRENTSTATUS" Then
                                        MailTable.Append("<td > CURRENT STATUS </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT1TO7DAYS" Then
                                        MailTable.Append("<td > COUNT 1-7 DAYS </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT1TO7DAYS" Then
                                        MailTable.Append("<td > INVOICE AMOUNT 1-7 DAYS(LACS) </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT8TO14DAYS" Then
                                        MailTable.Append("<td > COUNT 8-14 DAYS</td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT8TO14DAYS" Then
                                        MailTable.Append("<td > INVOICE AMOUNT 8-14 DAYS(LACS) </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT15DAYS" Then
                                        MailTable.Append("<td > COUNT 15+ DAYS </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT15DAYS" Then
                                        MailTable.Append("<td > INVOICE AMOUNT 15+ DAYS(LACS) </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALBARCODECOUNT" Then
                                        MailTable.Append("<td > TOTAL BAR CODE COUNT </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALINVOICEAMOUNT" Then
                                        MailTable.Append("<td > TOTAL INVOICE AMOUNT(LACS) </td>")
                                    Else
                                        MailTable.Append("<td > " & Common.Columns(l).ColumnName & " </td>")
                                    End If
                                Next

                                For k As Integer = 0 To Common.Rows.Count - 1 ' binding the tr tab in table
                                    If (Common.Rows(k).Item(0).ToString.Contains("TOTAL") = True) Then
                                        MailTable.Append("</tr><tr style=""background-color:#DCDCDC"">") ' for row records
                                    Else
                                        MailTable.Append("</tr><tr>") ' for row records
                                    End If
                                    For t As Integer = 0 To Common.Columns.Count - 1
                                        If IsNumeric(Common.Rows(k).Item(t).ToString()) = True Then
                                            MailTable.Append("<td align=""right""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                        Else
                                            MailTable.Append("<td align=""left""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                        End If
                                    Next
                                    MailTable.Append(" </tr>")
                                Next
                                MailTable.Append("</table>")

                                Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                Dim strmsgBdy As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                If strmsgBdy.Contains("@body") Then
                                    strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                    msg = strmsgBdy
                                Else
                                    msg = MailTable.ToString()
                                End If
                                msg = strmsgBdy

                                'sendMail1(MAILTO, CC, Bcc, mailsub, msg)
                                Dim obj As New MailUtill(eid:=eid)
                                obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, BCC:=Bcc)
                                'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, "")
                                '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                ' sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, "")
                                'lblMsg1.Text = "Report Scheduled...."
                            End If
                        End If
                    End If
                End If
            Next
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
    End Sub


    'Public Sub sendMailRepSchedule(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String, Optional ByVal backuppath As String = "")
    '    Try
    '        If Left(Mto, 1) = "{" Then
    '            Exit Sub
    '        End If
    '        ' Dim fname1 As String = ""
    '        ' Dim fname2 As String = ""
    '        'bcc = "ravi.sharma@myndsol.com"
    '        'Dim path As String = "https://hfcl.myndsaas.com/MailAttach/"
    '        ' fname2 = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & ".CSV"

    '        '  Dim path As String = "rajat/"


    '        Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
    '        Dim mailClient As New System.Net.Mail.SmtpClient()
    '        Email.IsBodyHtml = True
    '        If cc <> "" Then
    '            Email.CC.Add(cc)
    '            ' Email.Attachments.Add(att)
    '        End If
    '        If bcc <> "" Then
    '            Email.Bcc.Add(bcc)
    '            'Email.Attachments.Add(att)
    '        End If
    '        'If att.ContentType.ToString.Contains(".csv") Then
    '        If backuppath <> "" Then
    '            Dim att As Attachment
    '            att = New Attachment(backuppath)
    '            Email.Attachments.Add(att)
    '        End If

    '        'End If

    '        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "Dn#Ms@538Ti")
    '        mailClient.Host = "mail.myndsol.com"
    '        mailClient.UseDefaultCredentials = False
    '        mailClient.Credentials = basicAuthenticationInfo
    '        Try
    '            mailClient.Send(Email)
    '        Catch ex As Exception
    '            Exit Sub
    '        End Try
    '    Catch ex As Exception
    '        Exit Sub
    '    End Try
    'End Sub

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


    '' code for report scheduler templates calling by vishal 

    Public Sub SendDocumentExpiryMails()
        'Dim constr As String = "server=172.16.20.3\SQLDEV;initial catalog=DMS;uid=DMS;pwd=Gtr$698B#"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtAlert As New DataTable
        Try
            da.SelectCommand.CommandText = "select * from MMM_MST_Template where action=1 and subevent='DOCUMENT EXPIRY'"
            da.Fill(dtAlert)
            If dtAlert.Rows.Count > 0 Then
                For i As Integer = 0 To dtAlert.Rows.Count - 1
                    Dim curEid As Integer = dtAlert.Rows(i).Item("EID").ToString()
                    Dim CurTID As Integer = dtAlert.Rows(i).Item("tid")
                    Dim doctype As String = dtAlert.Rows(i).Item("eventname").ToString
                    Dim Bsla As Integer = dtAlert.Rows(i).Item("bsladay").ToString
                    Dim Asla As Integer = dtAlert.Rows(i).Item("Asladay").ToString
                    Dim WFstatus As String = dtAlert.Rows(i).Item("statusname").ToString
                    Dim subevent As String = dtAlert.Rows(i).Item("subevent").ToString()
                    If SchedulerOld(CurTID) = True Then
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "select tid [DOCID],curstatus," & WFstatus & " [EXDT] from  MMM_MST_DOC where DocumentType='" & doctype & "' and EID=" & curEid
                        Dim dtDoc As New DataTable
                        da.Fill(dtDoc)
                        For j As Integer = 0 To dtDoc.Rows.Count - 1
                            Dim ExDate As DateTime
                            Dim docID As Integer = dtDoc.Rows(j).Item("DOCID").ToString

                            If Not IsDBNull(dtDoc.Rows(j).Item("EXDT")) Then
                                If Len(dtDoc.Rows(j).Item("EXDT")) > 5 Then
                                    Dim ArrDateVals() As String
                                    Dim clubVals As String = ""
                                    Dim A1 As Integer = 0
                                    Dim A2 As Integer = 0
                                    If InStr(dtDoc.Rows(j).Item("EXDT"), "/") > 0 Then
                                        ArrDateVals = Split(dtDoc.Rows(j).Item("EXDT").ToString, "/")
                                        clubVals = Trim(ArrDateVals(1)) & "/" & Trim(ArrDateVals(0)) & "/" & Trim(ArrDateVals(2))
                                    ElseIf InStr(dtDoc.Rows(j).Item("EXDT"), "-") > 0 Then
                                        ArrDateVals = Split(dtDoc.Rows(j).Item("EXDT"), "-")
                                        clubVals = Trim(ArrDateVals(1)) & "/" & Trim(ArrDateVals(0)) & "/" & Trim(ArrDateVals(2))
                                    End If
                                    ExDate = clubVals
                                    A1 = ((24) - ((DateDiff(DateInterval.Hour, Now(), ExDate)) + 24))
                                    A2 = ((24) - ((DateDiff(DateInterval.Hour, Now(), ExDate)) + 24))
                                    If A1 > Bsla And A2 < Asla Then
                                        Call TemplateCalling(docID, curEid, doctype, subevent)
                                    End If
                                End If
                            End If
                        Next
                    End If
                Next
            End If

        Catch ex As Exception
            Call UpdateLoginDB("Fatal Error in sendDocumentexpiry function")
        Finally
            con.Close()
            da.Dispose()
            dtAlert.Dispose()
            con.Dispose()
        End Try
    End Sub
    Public Function SchedulerOld(ByVal Tempid As Integer) As Boolean
        Dim b As Boolean = False
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
        Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
        Dim da As New SqlDataAdapter("select sHH,sMM from MMM_MST_TEMPLATE where tid=" & Tempid, con)
        Dim dt As New DataTable()
        Try
            da.Fill(dt)
            Dim i As Integer = 0
            For i = 0 To dt.Rows.Count - 1
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(i)(0)) & ":" & Trim(dt.Rows(i)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                    Exit For
                End If
            Next

            Return b
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
            dt.Dispose()
        End Try
    End Function

    Public Sub UpdateLoginDB(ByVal rmk As String)
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "insert into schTestLog (remarks) values ('" & rmk & "')"

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            da.SelectCommand.ExecuteNonQuery()
        Catch ex As Exception
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub


    Private Function CreateCSVR(ByVal dt As System.Data.DataTable, ByVal path As String, ByVal uid As String) As String
        Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & uid & ".xls"
        'Dim fname As String = "F014Z1_" & DateTime.Now.ToString("yyyy-MM-dd") & ".CSV"
        Try
            If File.Exists(path & fname) Then
                File.Delete(path & fname)
            End If

            Dim sw As StreamWriter

            sw = New StreamWriter(path & fname, True)
            Dim hw As New HtmlTextWriter(sw)
            sw.Flush()
            Dim iColCount As Integer = dt.Columns.Count
            Dim grd As New GridView
            grd.DataSource = dt
            grd.DataBind()

            For i = 0 To grd.HeaderRow.Cells.Count - 1
                If grd.HeaderRow.Cells(i).Text.Contains("CurrentStatus").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Current Status"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("Count1To7Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Count 1-7 Days"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("InvoiceAmount1To7Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Invoice Amount 1-7 Days(lacs)"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("Count8To14Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Count 8-14 Days"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("InvoiceAmount8To14Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Invoice Amount 8-14 Days(lacs)"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("Count15Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Count 15+ Days"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("InvoiceAmount15Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Invoice Amount 15+ Days(lacs)"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("TotalBarCodeCount").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Total Bar Code Count"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("TotalInvoiceAmount").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Total Invoice Amount(lacs)"
                End If
            Next

            For i As Integer = 0 To grd.Rows.Count - 1
                grd.Rows(i).Attributes.Add("class", "textmode")
                'Apply text style to each Row 
            Next

            'grd.CssClass = style
            grd.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\""0000""; } </style>"
            sw.WriteLine(style)
            sw.Write(sw.NewLine)

            ' Now write all the rows.

            sw.Close()
        Catch ex As Exception

        End Try
        Return fname
        'sw.Close()
    End Function

    '''FTP Transfer Report 23-07-2019
    Sub FTPREPORT()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim FTPType As String = ""
        Try
            'Dim FPath As String = My.Application.Info.DirectoryPath & "/MailAttach/"
            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/")
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler where ftpflag=2 order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                    If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "USER" Then
                        eid = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                        oda.SelectCommand.CommandText = "select * from  MMM_MST_Entity where eid='" & eid & "' "
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.Fill(ds, "ftpsettings")
                        Dim ftpadd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpAddress").ToString()
                        Dim ftpid As String = ds.Tables("ftpsettings").Rows(0).Item("FtpID").ToString()
                        Dim ftppwd As String = ds.Tables("ftpsettings").Rows(0).Item("FtpPwd").ToString()
                        Dim cfolder As String = ""
                        If ds.Tables("rpt").Rows(d).Item("RSFolder").ToString() = "" Then
                            cfolder = ds.Tables("ftpsettings").Rows(0).Item("FtpFolder").ToString()
                        Else
                            cfolder = ds.Tables("rpt").Rows(d).Item("RSFolder").ToString()
                        End If

                        Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                        FTPType = ds.Tables("ftpsettings").Rows(0).Item("FTPType").ToString()
                        Dim filetype As String = ds.Tables("rpt").Rows(d).Item("sendtype").ToString()
                        Dim ReportTid As String = ds.Tables("rpt").Rows(d).Item("Tid").ToString()
                        Dim lastschedule As String = ds.Tables("rpt").Rows(d).Item("LastScheduledDate").ToString
                        lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                        str = Replace(str, "@lastsch", lastschedule)
                        oda.SelectCommand.CommandText = str
                        oda.SelectCommand.CommandType = CommandType.Text
                        Dim FTPR As New DataTable
                        oda.Fill(FTPR)
                        If FTPR.Rows.Count > 0 Then
                            InwardFTPREPORT(ReportTid)
                            Dim CNT As Integer = 0
                            Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                            Rptname = ds.Tables("rpt").Rows(d).Item("reportname").ToString()
                            oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "FTP")
                            oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                            oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                            oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "FTPREPORT")
                            oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", ds.Tables("rpt").Rows(d).Item("reportname").ToString())
                            oda.SelectCommand.Parameters.AddWithValue("@EID", ds.Tables("rpt").Rows(d).Item("eid").ToString())
                            oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            oda.SelectCommand.ExecuteNonQuery()
                            ' Dim fname As String = ""
                            'fname = CreateCSVFTP(FTPR, Rptname)
                            'CopyfiletoInbox(Fadd, "hcl", "sh1nodrm", FPath, fname, "FORHCL")

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

                        End If

                    End If
                End If
            Next
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
    End Sub
    Protected Sub CopyfiletoInbox(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim URI As String = Fadd & "/" & filenm


            'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
            'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
            'ServicePointManager.SecurityProtocol = Tls12

            Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(URI), System.Net.FtpWebRequest)
            'ftp://103.25.172.228/FORHCL
            'ftp://103.25.172.228/FORMYND/
            clsRequest.Credentials = New System.Net.NetworkCredential(login, pwd)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile

            'ftp.SSLConfiguration.EnabledSslProtocols = SslProtocols.Tls12


            ' read in file...
            'Dim file() As Byte = System.IO.File.ReadAllBytes()
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
    End Sub

    Protected Sub CopyfiletoSFTP(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim URI As String = Fadd & "/" & filenm

            Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/MailAttach/" & filenm)
            'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
            'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
            'ServicePointManager.SecurityProtocol = Tls12
            Dim client As SftpClient = New SftpClient(Fadd, login, pwd)
            client.Connect()

            Using stream As Stream = File.OpenRead(readPath & filenm)
                client.UploadFile(stream, cfoldernm & filenm)
            End Using

        Catch ex As Exception
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
    End Sub
    Private Function CreateCSVFTP(ByVal dt As DataTable, ByVal ReportName As String) As String
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

    Private Function CreateXMLFTP(ByVal dt As DataTable, ByVal ReportName As String) As String
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

    Sub InwardFTPREPORT(ByVal Tid As String)
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
    End Sub


    Public Class EIDDETAIL
        '<DataMember(Name:="EID", Order:=1)>
        Public Property EID As Int32
    End Class

    '<DataContractAttribute(Namespace:="", Name:="EIDRESPONSE")>
    Public Class EIDRESPONSE
        '<DataMember(Name:="EID", Order:=1)>
        Public Property EID As Int32
        '<DataMember(Name:="FTPID", Order:=2)>
        Public Property ftpid As Int32
        '<DataMember(Name:="UID", Order:=3)>
        Public Property uid As Int32

        '<DataMember(Name:="GID", Order:=3)>
        Public Property gid As Int32


        '<DataMember(Name:="DOCTYPE", Order:=3)>
        Public Property docType As String

        '<DataMember(Name:="FUP_FieldMapping", Order:=3)>
        Public Property fup_FieldMapping As String
        '<DataMember(Name:="LOC_FIELDMAPPING", Order:=3)>
        Public Property loc_FieldMapping As String
        '<DataMember(Name:="LOCID", Order:=3)>
        Public Property locid As String
        '<DataMember(Name:="ReadMode", Order:=3)>
        Public Property ReadMode As String
        '<DataMember(Name:="HostName", Order:=3)>
        Public Property HostName As String
        '<DataMember(Name:="UserName", Order:=3)>
        Public Property UserName As String
        '<DataMember(Name:="Password", Order:=3)>
        Public Property Password As String
        '<DataMember(Name:="Port", Order:=3)>
        Public Property Port As String
        '<DataMember(Name:="PostFix", Order:=3)>
        Public Property PostFix As String

        '<DataMember(Name:="BarCode", Order:=3)>
        Public Property BarCode As String
        '<DataMember(Name:="BarCode", Order:=3)>
        Public Property SaveAs As String

    End Class


End Class

<DataContractAttribute(Namespace:="", Name:="Res")>
Public Class Hcl_apprvl_pending
    <DataMember(Name:="Bpmid", Order:=1)> _
    Public Property Bpmid As String
    <DataMember(Name:="Invoice_Type", Order:=2)> _
    Public Property Invoice_Type As String
    <DataMember(Name:="Vendor_Name", Order:=3)> _
    Public Property Vendor_Name As String
    <DataMember(Name:="Invoice_No", Order:=4)> _
    Public Property Invoice_No As String
    <DataMember(Name:="Invoice_Amount_WO_Tax", Order:=5)> _
    Public Property Invoice_Amount_WO_Tax As String
    <DataMember(Name:="Pending_Days", Order:=6)> _
    Public Property Pending_Days As String
    <DataMember(Name:="Creation_Date", Order:=7)> _
    Public Property Creation_Date As String
    <DataMember(Name:="L1_Approver", Order:=8)> _
    Public Property L1_Approver As String
    <DataMember(Name:="L2_Approver", Order:=9)> _
    Public Property L2_Approver As String
    <DataMember(Name:="L3_Approver", Order:=10)> _
    Public Property L3_Approver As String
    <DataMember(Name:="L4_Approver", Order:=11)> _
    Public Property L4_Approver As String
    <DataMember(Name:="Ecode", Order:=12)> _
    Public Property Ecode As String
    <DataMember(Name:="Emp_Name", Order:=13)> _
    Public Property Emp_Name As String
    <DataMember(Name:="Status_In_Pearl", Order:=14)> _
    Public Property Status_In_Pearl As String
End Class

