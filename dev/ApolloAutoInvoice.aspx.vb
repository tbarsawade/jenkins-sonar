Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Management
Imports System.Xml
Imports System.Management.Instrumentation
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Delegate
Imports JWT.JsonWebToken
Imports System.Diagnostics
Imports System.Globalization

<Assembly: DebuggerDisplay("{ToString}", Target:=GetType(Date))>

Partial Class ApolloAutoInvoice
    Inherits System.Web.UI.Page

    Public Sub New()

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

    Public Sub notificationMail()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = Nothing
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
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
            Dim obj As New MailUtill(180)
            'fill Product  
            'Reset Password functionality user can reset only once if he clicked over there 

            MSG = "This is test mail for checking - sending mail from paytm email id"
            subject = "Test mail"
            MAILTO = "sunil.pareek@myndsol.com"
            'MAILTO = "ardee.school@lbf.co.in,mgr.ic@lbf.co.in"


            cc = "sunil.pareek@myndsol.com,varun.s@myndsol.com,saurabh.agarwal@myndsol.com"
            'Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()

            'MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
            ' STR = ds.Tables("TEMP").Rows(0).Item("QRY")



            obj.SendMail1(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc, EID:=180, DocType:="Purchase Requisition")
            Label1.Text = "MAil send successfull"
        Catch ex As Exception
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Dispose()
            End If
        End Try
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Call notificationMail()
    End Sub

    Private Sub StartServiceCode()
        'Try
        'If Not Directory.Exists("E:\MYNDSOLUTION") Then
        '    Directory.CreateDirectory("E:\MYNDSOLUTION")
        'End If
        Dim sz As Integer = 78979
        'Dim pathstr As String = System.Reflection.Assembly.GetExecutingAssembly().Location
        'pathstr = Path.GetDirectoryName(pathstr)
        ' ''Read Values from Xml File
        'Dim xmlDocRead As New XmlDocument

        ' '' Uncomment on fly
        'xmlDocRead.Load(pathstr + "/" + "TecumSehConfig.xml")

        ''for testing 
        'xmlDocRead.Load("E:\" & "BPM_FT_Config_TecumSeh.xml")
        'Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name & "/" & xmlDocRead.DocumentElement.FirstChild.Name
        Dim Cnt As Integer = 0
        Dim Gid As String = "34"
        Dim uid As String = "192"
        Dim EID As String = "7"
        Dim loc_fieldMapping As String = "fld14"
        Dim fup_fieldMapping As String = "fld15"
        Dim LocationID As String = "1116432"
        Dim doctype As String = "Vendor Invoice"
        Dim rn As New Random  ' new by sunil for random number 31_may_14


        ' Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
        'For Each node As XmlNode In nodes
        '    For c As Integer = 0 To node.ChildNodes.Count - 1
        '        If UCase(node.ChildNodes.Item(c).Name) = "EID" Then
        '            EID = node.ChildNodes.Item(c).InnerXml
        '        ElseIf UCase(node.ChildNodes.Item(c).Name) = "UID" Then
        '            uid = node.ChildNodes.Item(c).InnerXml
        '        ElseIf UCase(node.ChildNodes.Item(c).Name) = "GID" Then
        '            Gid = node.ChildNodes.Item(c).InnerXml
        '        ElseIf UCase(node.ChildNodes.Item(c).Name) = "DOCTYPE" Then
        '            doctype = node.ChildNodes.Item(c).InnerXml
        '        ElseIf UCase(node.ChildNodes.Item(c).Name) = "FUP_FIELDMAPPING" Then
        '            fup_fieldMapping = node.ChildNodes.Item(c).InnerXml
        '        ElseIf UCase(node.ChildNodes.Item(c).Name) = "LOC_FIELDMAPPING" Then
        '            loc_fieldMapping = node.ChildNodes.Item(c).InnerXml
        '        ElseIf UCase(node.ChildNodes.Item(c).Name) = "LOC_ID" Then
        '            LocationID = node.ChildNodes.Item(c).InnerXml
        '        End If
        '    Next

        For Each foundfile As FileInfo In New DirectoryInfo("E:\MYNDSOLUTION_BPM\").GetFiles
            ''Uncomment on fly
            Dim DOCURL As String = foundfile.Name
            Dim exten As String = foundfile.Extension
            Dim Nexten As String = foundfile.Extension
            Dim BarCode As String = foundfile.Name

            BarCode = BarCode.Replace(Nexten, "")
            '' user below line to add datetimestamp to file name by sp on 17_may_14
            Dim n = rn.Next(101, 9999)
            Dim dtstamp As String = "_" & Now.Year & Now.Month & Now.Day & Now.Millisecond & "_" & n
            DOCURL = DOCURL.Replace(Nexten, dtstamp & Nexten)

            '' renaming file 
            System.IO.File.Move("E:\MYNDSOLUTION_BPM\" & foundfile.Name, "E:\MYNDSOLUTION_BPM\" & DOCURL)
            Try
                ' Dim strname As String = "ftp://ftp.myndsolution.com/" & EID & "/" & DOCURL
                Dim strname As String = "ftp://ftp.myndsolution.com/test/" & DOCURL
                Dim request As System.Net.FtpWebRequest = DirectCast(System.Net.FtpWebRequest.Create(strname), System.Net.FtpWebRequest)
                '   request.Credentials = New System.Net.NetworkCredential("ftp_myndsaas", "h6dXtr3#")  
                request.Credentials = New System.Net.NetworkCredential("ftp_myndbpm", "N$bsT73#")
                sz = foundfile.Length
                request.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                Dim file1() As Byte = System.IO.File.ReadAllBytes("E:\MYNDSOLUTION_BPM\" & DOCURL)
                Dim strz As System.IO.Stream = request.GetRequestStream()
                strz.Write(file1, 0, file1.Length)
                strz.Close()
                strz.Dispose()
            Catch ex As Exception
                Call Error_log("Error while copying on ftp - (" & DOCURL & ") - " & ex.Message.ToString())
                Continue For
            End Try

            Try
                ''WebService Call
                'fld17
                'Dim request1 As HttpWebRequest = DirectCast(HttpWebRequest.Create("http://myndsaas.com/DMSservice.svc/MoveFile?EID=" & EID & "&gid=" & Gid & "&STID=0&docurl=" & DOCURL & "&oUID=" & uid & "&filesize=" & sz & "" & "&Doctype=" & doctype & "&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID), HttpWebRequest)
                ' latest Dim request1 As HttpWebRequest = DirectCast(HttpWebRequest.Create("http://myndsaas.com/DMSservice.svc/MoveFile?EID=" & EID & "&gid=" & Gid & "&STID=0&docurl=" & DOCURL & "&oUID=" & uid & "&filesize=" & sz & "" & "&Doctype=" & doctype & "&fup_fieldmapping=" & fup_fieldMapping & "&loc_fieldmapping=" & loc_fieldMapping & "&location_id=" & LocationID & "&BarCodeFldmapping=fld17&barcodefldval=" & BarCode), HttpWebRequest)
                Dim request1 As HttpWebRequest = DirectCast(HttpWebRequest.Create("https://myndbpm.com/DMSservice.svc/MoveFile?EID=7&gid=" & Gid & "&STID=0&docurl=" & DOCURL & "&oUID=" & uid & "&filesize=" & sz & ""), HttpWebRequest)
                request1.Method = "GET"
                request1.UseDefaultCredentials = False
                request1.PreAuthenticate = True
                request1.Timeout = 60 * 2000
                Dim response As HttpWebResponse = DirectCast(request1.GetResponse(), HttpWebResponse)
                Dim stat As String = response.StatusCode.ToString()
                response.Close()

                '  File.Delete(foundfile.FullName)
                '  File.Delete("E:\MYNDSOLUTION\" & DOCURL)

            Catch ex As Exception
                Call Error_log("Error while calling WS - (" & DOCURL & ") - " & ex.InnerException.Message.ToString() & " / " & ex.Message.ToString())
                Continue For
            End Try
            ''WebService Call
            Try
                'File.Delete(foundfile.FullName)
                File.Delete("E:\MYNDSOLUTION_BPM\" & DOCURL)
            Catch ex As Exception
                Call Error_log("Error while Deleting -  (" & DOCURL & ") - " & ex.Message.ToString())
                Continue For
            End Try
        Next

        ''WebService Call
        Label1.Text = "Successful"
        '  Next
    End Sub

    'Private Sub SendAlertMails()
    '    ' Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
    '    Dim con As SqlConnection = New SqlConnection(constr)
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
    '    Dim dtAlert As New DataTable()
    '    Try
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim supervisoremail As String = ""
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.CommandText = "select * from MMM_MST_Template where action=1 and subevent='SLA EXPIRED' and tid=294"

    '        da.Fill(dtAlert)
    '        If dtAlert.Rows.Count > 0 Then
    '            For i As Integer = 0 To dtAlert.Rows.Count - 1
    '                Dim curEid As Integer = dtAlert.Rows(i).Item("EID").ToString()
    '                Dim CurTID As Integer = dtAlert.Rows(i).Item("tid").ToString
    '                Dim Bsla As Integer = dtAlert.Rows(i).Item("bsladay").ToString
    '                Dim Asla As Integer = dtAlert.Rows(i).Item("Asladay").ToString
    '                Dim WFstatus As String = dtAlert.Rows(i).Item("statusname").ToString
    '                Dim CurEventNm As String = dtAlert.Rows(i).Item("EventName").ToString
    '                Dim SubEventNm As String = dtAlert.Rows(i).Item("SubEvent").ToString
    '                Dim supervisorFLD As String = dtAlert.Rows(i).Item("userfield").ToString

    '                Dim STR As String = "", MAILTO As String = "", MAILID As String = "", subject As String = "", MSGMain As String = "", cc As String = "", Bcc As String = ""
    '                Dim fn As String = "", MSG As String
    '                Dim DS As New DataSet

    '                MSGMain = System.Web.HttpUtility.HtmlDecode(dtAlert.Rows(i).Item("msgbody").ToString())
    '                subject = dtAlert.Rows(i).Item("SUBJECT").ToString()
    '                cc = dtAlert.Rows(i).Item("CC").ToString()
    '                Bcc = dtAlert.Rows(i).Item("BCC").ToString()
    '                STR = dtAlert.Rows(i).Item("qry").ToString()

    '                If "1" = "1" Then  'Scheduler(CurTID) = True Then
    '                    Dim dtUsers As New DataTable

    '                    If con.State <> ConnectionState.Open Then
    '                        con.Open()
    '                    End If

    '                    da.SelectCommand.Parameters.Clear()
    '                    ' da.SelectCommand.CommandText = "select userid from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid inner join MMM_MOVPATH_DOC mvDoc on dtl.pathID=mvDoc.tid where  Doc.DocumentType='" & CurEventNm & "' and doc.EID=" & curEid & " and doc.curstatus='" & WFstatus & "' and dtl.aprstatus is null  and  case when ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) >" & Bsla & ") and ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) <" & Asla & ") then 'TRUE' ELSE 'FALSE' END ='TRUE' group by userid order by userid "
    '                    da.SelectCommand.CommandText = "uspAlertMailSLA_getAllUsers"
    '                    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                    da.SelectCommand.Parameters.AddWithValue("en", CurEventNm)
    '                    da.SelectCommand.Parameters.AddWithValue("ws", WFstatus)
    '                    da.SelectCommand.Parameters.AddWithValue("bsla", Bsla)
    '                    da.SelectCommand.Parameters.AddWithValue("asla", Asla)
    '                    da.SelectCommand.Parameters.AddWithValue("eid", curEid)
    '                    da.Fill(dtUsers)

    '                    ' Call UpdateLoginDB("Time matched for Template : " & CurEventNm & "-" & WFstatus) -- removed on 12_may

    '                    For k As Integer = 0 To dtUsers.Rows.Count - 1
    '                        If Not IsDBNull(dtUsers.Rows(k).Item("userid")) Then
    '                            Dim curUser As Integer = dtUsers.Rows(k).Item("userid").ToString()
    '                            Dim dtQry As New DataTable
    '                            Dim dtDtl As New DataTable
    '                            Dim dtDocs As New DataTable
    '                            Dim dtMail As New DataTable
    '                            Dim StrQry As String = ""
    '                            MSG = MSGMain

    '                            da.SelectCommand.Parameters.Clear()
    '                            'da.SelectCommand.CommandText = "select DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvdoc.SLA*24 [curDt_diff_sla], mvdoc.sla [SLA], (mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24)  [act_hour], DATEADD(day,mvDoc.sla ,fdate) [ExpireDt],  doc.tid, dtl.*  from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid   inner join MMM_MOVPATH_DOC mvDoc on dtl.pathID=mvDoc.tid where  Doc.DocumentType='" & CurEventNm & "' and doc.EID=" & curEid & " and doc.curstatus='" & WFstatus & "' and dtl.aprstatus is null  and  case when ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) >" & Bsla & ") and ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) <" & Asla & ") then 'TRUE' ELSE 'FALSE' END ='TRUE' and userid=" & curUser & " order by docid "
    '                            da.SelectCommand.CommandText = "uspAlertMailSLA_getUserDocID"
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.SelectCommand.Parameters.AddWithValue("en", CurEventNm)
    '                            da.SelectCommand.Parameters.AddWithValue("ws", WFstatus)
    '                            da.SelectCommand.Parameters.AddWithValue("bsla", Bsla)
    '                            da.SelectCommand.Parameters.AddWithValue("asla", Asla)
    '                            da.SelectCommand.Parameters.AddWithValue("eid", curEid)
    '                            da.SelectCommand.Parameters.AddWithValue("curuser", curUser)
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.Fill(dtDocs)

    '                            ' get docid's separated by comma 
    '                            da.SelectCommand.Parameters.Clear()
    '                            'da.SelectCommand.CommandText = "select SUBSTRING( ( SELECT ',' +   convert(nvarchar(20),doc.tid)  from MMM_MST_DOC doc inner join MMM_DOC_DTL dtl on doc.tid=dtl.docid   inner join MMM_MOVPATH_DOC mvDoc on dtl.pathID=mvDoc.tid where  Doc.DocumentType='" & CurEventNm & "' and doc.EID=" & curEid & " and doc.curstatus='" & WFstatus & "' and dtl.aprstatus is null  and  case when ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) >" & Bsla & ") and ((mvDoc.sla*24)-(DATEDIFF(hh,getdate(),DATEADD(day,mvDoc.sla ,fdate)) + mvDoc.sla*24) <" & Asla & ") then 'TRUE' ELSE 'FALSE' END ='TRUE' and userid=" & curUser & " order by doc.tid FOR XML PATH('')), 2,5000) AS ColList"
    '                            da.SelectCommand.CommandText = "uspAlertMailSLA_getUserIDList"
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.SelectCommand.Parameters.AddWithValue("en", CurEventNm)
    '                            da.SelectCommand.Parameters.AddWithValue("ws", WFstatus)
    '                            da.SelectCommand.Parameters.AddWithValue("bsla", Bsla)
    '                            da.SelectCommand.Parameters.AddWithValue("asla", Asla)
    '                            da.SelectCommand.Parameters.AddWithValue("eid", curEid)
    '                            da.SelectCommand.Parameters.AddWithValue("curuser", curUser)
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.Fill(dtDtl)

    '                            Dim ColList As String = ""
    '                            If dtDtl.Rows.Count <> 0 Then
    '                                ColList = dtDtl.Rows(0).Item(0).ToString()
    '                            End If

    '                            StrQry = STR & " WHERE TID in (" & ColList & ")"
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.CommandText = StrQry
    '                            da.Fill(dtQry)

    '                            'fn = "{@TABLE}"
    '                            Dim Spos As Integer = MSG.IndexOf("{@TABLE}")
    '                            Dim Epos As Integer = MSG.IndexOf("{@/TABLE}")

    '                            Dim TblText As String = MSG.Substring(Spos + 8, (Epos - (Spos + 8)))
    '                            Dim ArrVars() As String = TblText.Split("|")

    '                            Dim MailBody As String = "<table width=""100%"" border=""3px"" cellpadding=""2px"" cellspacing=""2px""> <tr style=""background-color:Azure""> "
    '                            Dim sI As Integer = 0, eI As Integer = 0
    '                            For m As Integer = 0 To ArrVars.Length - 1
    '                                sI = ArrVars(m).IndexOf("{")
    '                                eI = ArrVars(m).IndexOf("}")
    '                                ArrVars(m) = ArrVars(m).Substring(sI + 1, (eI - sI) - 1)
    '                                MailBody &= "<td>  " & ArrVars(m).ToString & " </td>"
    '                            Next
    '                            MailBody &= " </tr> "

    '                            For j As Integer = 0 To dtQry.Rows.Count - 1 '''' this is for no. of rows of docs - outer most
    '                                MailBody &= "<tr>"
    '                                For m As Integer = 0 To ArrVars.Length - 1
    '                                    'Dim Colnm As String = ArrVars(m).Substring(1, ArrVars(m).Length - 1)
    '                                    Dim Colnm As String = ArrVars(m)
    '                                    If dtQry.Columns.Contains(Colnm) Then
    '                                        MailBody &= "<td>" & "" & dtQry.Rows(j).Item(Colnm).ToString() & "</td>"
    '                                    End If
    '                                Next
    '                                MailBody &= "</tr>"
    '                            Next
    '                            MailBody &= "</table>"

    '                            If con.State <> ConnectionState.Open Then
    '                                con.Open()
    '                            End If

    '                            '' select - get mail id of current user
    '                            If supervisorFLD.Length > 0 And supervisorFLD.ToUpper().Contains("FLD") Then
    '                                da.SelectCommand.CommandText = "select emailid," & supervisorFLD & " from mmm_mst_user where eid=" & curEid & " and uid=" & curUser
    '                                da.Fill(dtMail)
    '                                If dtMail.Rows.Count <> 0 Then
    '                                    MAILTO = dtMail.Rows(0).Item("emailid").ToString
    '                                    da.SelectCommand.CommandText = "Select emailid from mmm_mst_user where eid=" & curEid & " and uid=" & dtMail.Rows(0).Item(supervisorFLD).ToString() & ""
    '                                    supervisoremail = da.SelectCommand.ExecuteScalar()
    '                                    If cc <> "" Then
    '                                        cc = cc & ";" & supervisoremail
    '                                    Else
    '                                        cc = supervisoremail
    '                                    End If

    '                                End If
    '                            Else
    '                                da.SelectCommand.CommandText = "select emailid from mmm_mst_user where eid=" & curEid & " and uid=" & curUser
    '                                da.Fill(dtMail)
    '                                If dtMail.Rows.Count <> 0 Then
    '                                    MAILTO = dtMail.Rows(0).Item("emailid").ToString
    '                                End If
    '                                If cc <> "" Then
    '                                    cc = cc & "," & "sunil.pareek@myndsol.com"
    '                                Else
    '                                    cc = "sunil.pareek@myndsol.com"
    '                                End If
    '                            End If

    '                            '' select - get mail id of current user
    '                            'da.SelectCommand.CommandText = "select emailid from mmm_mst_user where eid=" & curEid & " and uid=" & curUser
    '                            'da.Fill(dtMail)
    '                            'If dtMail.Rows.Count <> 0 Then
    '                            '    MAILTO = dtMail.Rows(0).Item("emailid").ToString
    '                            'End If

    '                            '' code here to replace between {@TABLE} and {@/TABLE}
    '                            Spos = MSG.IndexOf("{@TABLE}")
    '                            Epos = MSG.IndexOf("{@/TABLE}")
    '                            Dim repVal As String = MSG.Substring(Spos, ((Epos + 9) - Spos))
    '                            MSG = MSG.Replace(repVal, MailBody)

    '                            '' send mail here 
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = "INSERT_MAILLOG"
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
    '                            da.SelectCommand.Parameters.AddWithValue("@CC", cc)
    '                            da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
    '                            da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT MAIL")
    '                            da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", CurEventNm & "-" & WFstatus)
    '                            da.SelectCommand.Parameters.AddWithValue("@EID", curEid)

    '                            Dim RES As Integer = da.SelectCommand.ExecuteScalar()
    '                            If RES > 0 Then
    '                                sendMail1(MAILTO, cc, Bcc, subject, MSG)
    '                            End If
    '                            dtDocs.Dispose()
    '                            dtQry.Dispose()
    '                            dtDtl.Dispose()
    '                            dtMail.Dispose()
    '                        End If
    '                    Next
    '                    dtUsers.Dispose()
    '                Else
    '                End If
    '            Next
    '        End If

    '        ' Call UpdateLoginDB("Exits from Sendalertmail function (Live)") -- removed on 12_may_14

    '        'lblmsg.Text = "Action Successful"


    '    Catch ex As Exception
    '        '   Call UpdateLoginDB("Error in Sendalertmail function msg is - " & ex.InnerException.Message)
    '    Finally
    '        dtAlert.Dispose()
    '        con.Close()
    '        da.Dispose()
    '    End Try
    'End Sub

    Protected Sub btnAutoInvoice_Click(sender As Object, e As EventArgs) Handles btnAutoInvoice.Click


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

            da = New SqlDataAdapter("select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld from   MMM_MST_AutoInvoiceSetting where  IsActiveStatus=1", con)
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
            Dim Fldstr As String = ""
            Dim strTFlds As String = String.Empty
            Dim strSFlds As String = String.Empty
            Dim strHTFlds As String = String.Empty
            Dim strHSFlds As String = String.Empty
            Dim RentFreePeriodFlds As String = String.Empty
            Dim EID As String = String.Empty
            If ds.Tables("AutoInvSettingData").Rows.Count <> 0 Then
                For i As Integer = 0 To ds.Tables("AutoInvSettingData").Rows.Count - 1

                    StartDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("StartDateFld"))
                    EndDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EndDateFld"))
                    FrequencyFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("FrequencyFld"))
                    PeriodFld = Convert.ToInt16(ds.Tables("AutoInvSettingData").Rows(i).Item("PeriodFld"))
                    RentalFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                    RegistrationDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RegistrationDateFld"))
                    RentFreePeriodFlds = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentFreePeriodFld"))
                    EID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EID"))
                    SchedulerTidID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid"))
                    Dim FldsVal As String = String.Empty
                    Dim SchedulerCheck As Boolean
                    SchedulerCheck = Scheduler(SchedulerTidID)
                    ' SchedulerCheck = True
                    If SchedulerCheck = True Then
                        Dim FldsValArr As String()
                        Dim value As String = ""

                        da.SelectCommand.CommandText = "select  F.Tid,F.TFld,F.SFld,F.SDocType,F.AutoInvTid,F.EID,F.TFldName,F.sFldName,F.Leasetype,F.TDocType  from    MMM_MST_AutoInvoiceSetting C   inner join MMM_MST_AutoInvFieldSetting F on c.Tid=F.AutoInvTid where C.eid=" & EID & " and C.IsActiveStatus=1 and F.AutoInvTid=" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid")) & ""
                        'da.SelectCommand.Transaction = tran
                        da.Fill(ds, "AutoInvFieldData")
                        Dim valueL As String


                        Dim names As List(Of String) = (From row In ds.Tables("AutoInvFieldData").AsEnumerable()
                                                        Select row.Field(Of String)("TDocType") Distinct).ToList()

                        For f As Integer = 0 To names.Count - 1
                            Dim dvs As DataView = ds.Tables("AutoInvFieldData").DefaultView
                            dvs.RowFilter = "TDocType='" & names(f) & "'"

                            Dim filteredTable As New DataTable()
                            filteredTable = dvs.ToTable()
                            If filteredTable.Rows.Count <> 0 Then
                                strTFlds = ""
                                strSFlds = ""
                                strTFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                strSFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                Dim strHSFldsArr As String()
                                strHSFldsArr = strSFlds.Split(",")
                                valueL = String.Join(",',',", strHSFldsArr)
                                strHSFlds = valueL
                            End If
                            Dim myDt As DateTime = DateTime.Now

                            Dim xmlDate As String = myDt.ToString("dd/MM/yy")
                            xmlDate = "30/01/20"
                            da.SelectCommand.CommandText = " select  tid,convert(varchar, convert(datetime, " & RegistrationDateFld & ", 3), 103)  as RegistrationDate ," & RentFreePeriodFlds & " as RentFreePeriod from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & " where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and convert(varchar(200),getdate(),3) between " & StartDateFld & " and " & EndDateFld & " and reftid<>''"
                            ' da.SelectCommand.CommandText = " select  tid,convert(varchar, convert(datetime, " & RegistrationDateFld & ", 3), 103) as RegistrationDate ," & RentalFld & " as RentalAmount   from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & " where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and '" & xmlDate & "' between " & StartDateFld & " and " & EndDateFld & ""
                            da.Fill(ds, "AutoInvDocData")
                            Dim SourceDocData As New DataTable
                            Dim RDOCIDData As New DataTable
                            'RDOCIDData
                            If ds.Tables("AutoInvDocData").Rows.Count <> 0 Then
                                con.Open()
                                tran = con.BeginTransaction()
                                For j As Integer = 0 To ds.Tables("AutoInvDocData").Rows.Count - 1

                                    If SourceDocData.Rows.Count > 0 Then
                                        SourceDocData.Clear()
                                    End If
                                    da.SelectCommand.CommandText = " select tid as RDOCID,concat(" & strHSFlds & ") as TFldVAl from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & "   where documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "'  and eid=" & EID & " and tid=" & ds.Tables("AutoInvDocData").Rows(j).Item("Tid") & " "
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Transaction = tran
                                    da.Fill(ds, "SourceDocData")
                                    SourceDocData = ds.Tables("SourceDocData")
                                    Dim RDOCID As String = ""
                                    If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                        FldsVal = ""
                                        FldsVal = SourceDocData.Rows(0).Item("TFldVAl")
                                        FldsValArr = FldsVal.Split(",")
                                        value = ""
                                        value = String.Join("','", FldsValArr)
                                        value = "'" + value + "'"
                                        RDOCID = Convert.ToString(SourceDocData.Rows(0).Item("RDOCID"))
                                    End If
                                    If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                        If RDOCIDData.Rows.Count > 0 Then
                                            RDOCIDData.Clear()
                                        End If
                                        If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then
                                            da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate();  select * from MMM_MST_MASTER where RefTid=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "' and adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE) "
                                        Else
                                            da.SelectCommand.CommandText = "DECLARE @DATE DATETIME ;SET @DATE= getdate(); select * from MMM_MST_DOC where RDOCID=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "' and  adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE);"
                                        End If
                                        'DECLARE @DATE DATETIME;SET @DATE=convert(varchar(100),'2020-03-20 12:13:36.480',103) ; select RDOCID,* from MMM_MST_DOC where  documenttype='Rental Invoice'  and adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE) and RDOCID=
                                        da.SelectCommand.CommandType = CommandType.Text

                                        da.SelectCommand.Transaction = tran
                                        da.Fill(ds, "RDOCIDData")
                                        RDOCIDData = ds.Tables("RDOCIDData")
                                        If ds.Tables("RDOCIDData").Rows.Count = 0 Then
                                            If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("LastInvoceGenrationDate")) = "" Then
                                                'Plus 30 days or 90 days
                                                Dim Monthly As Double = 30
                                                Dim Quaterly As Double = 90

                                                Dim myString As String = ds.Tables("AutoInvDocData").Rows(j).Item("RegistrationDate")
                                                Dim lastBusinessDay As New DateTime()
                                                Dim lastBusinessDayplus5 As New DateTime()
                                                Dim arr = myString.Split("/")
                                                Dim date1 As New Date(arr(2), arr(1), arr(0))
                                                date1 = CDate(date1)
                                                Dim RentFreePeriod As Integer
                                                Dim date2 As DateTime
                                                If Convert.ToString(ds.Tables("AutoInvDocData").Rows(j).Item("RentFreePeriod")) = "" Then
                                                    RentFreePeriod = 0
                                                    If FrequencyFld = "Monthly" Then
                                                        date2 = CDate(date1).AddDays(+Monthly)
                                                    ElseIf FrequencyFld = "Quaterly" Then
                                                        date2 = CDate(date1).AddDays(+Quaterly)
                                                    End If
                                                Else
                                                    RentFreePeriod = Convert.ToInt16(ds.Tables("AutoInvDocData").Rows(j).Item("RentFreePeriod"))
                                                    date2 = CDate(date1).AddDays(+RentFreePeriod)
                                                End If

                                                lastBusinessDay = Date.Now().ToString("MM/dd/yyyy")
                                                lastBusinessDayplus5 = CDate(lastBusinessDay).AddDays(+5)
                                                ' lastBusinessDay = "2/05/2020"
                                                If date2 >= lastBusinessDay Or date2 <= lastBusinessDayplus5 Then
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
                                                    da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.Transaction = tran
                                                    da.SelectCommand.ExecuteNonQuery()

                                                    da.Fill(ds, "fields")
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
                                                    'calculative fields
                                                    Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
                                                    Dim viewdoc As String = Convert.ToString(names(f))
                                                    viewdoc = viewdoc.Replace(" ", "_")
                                                    If CalculativeField.Length > 0 Then
                                                        For Each CField As DataRow In CalculativeField
                                                            Dim formulaeditorr As New formulaEditor
                                                            Dim forvalue As String = String.Empty
                                                            'Commented By Komal on 28March2014
                                                            forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & res & ""
                                                            da.SelectCommand.CommandText = upquery
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.SelectCommand.ExecuteNonQuery()
                                                        Next
                                                    End If

                                                    '' insert default First movement of document - by sunil

                                                    da.SelectCommand.CommandText = "InsertDefaultMovement"
                                                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                    da.SelectCommand.Parameters.Clear()
                                                    da.SelectCommand.Transaction = tran
                                                    da.SelectCommand.Parameters.AddWithValue("tid", res)
                                                    da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
                                                    da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                                                    da.SelectCommand.ExecuteNonQuery()
                                                    'Dim ob1 As New DMSUtil()
                                                    Dim res12 As String = String.Empty
                                                    'Dim ob As New DynamicForm
                                                    res12 = ob1.GetNextUserFromRolematrixT(res, EID, "30200", "", "30200", con, tran)
                                                    Dim sretMsgArr() As String = res12.Split(":")
                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)
                                                    If sretMsgArr(0) = "ARCHIVE" Then
                                                        'Dim Op As New Exportdata()
                                                        'Op.PushdataT(res, sretMsgArr(1), EID, con, tran)
                                                    End If

                                                    ' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
                                                    If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                                                        Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                                                        'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
                                                        'this code block is added by ajeet kumar for transaction to be rolled back
                                                        ''tran.Rollback()
                                                        ''Exit Sub
                                                    Else

                                                    End If
                                                    'Execute Trigger
                                                    Try
                                                        Dim FormName As String = Convert.ToString(names(f))
                                                        '     Dim EID As Integer = 0
                                                        If (res > 0) And (FormName <> "") Then
                                                            Trigger.ExecuteTriggerT(FormName, EID, res, con, tran)
                                                        End If
                                                    Catch ex As Exception
                                                        Throw
                                                    End Try


                                                End If

                                            Else
                                                Dim myString As String = ds.Tables("AutoInvSettingData").Rows(i).Item("LastInvoceGenrationDate")
                                                Dim lastBusinessDay As New DateTime()
                                                Dim lastBusinessDayplus5 As New DateTime()

                                                Dim arr = myString.Split("/")
                                                Dim date1 As New Date(arr(2), arr(1), arr(0))
                                                Dim Monthly As Double = 30
                                                Dim Quaterly As Double = 90
                                                date1 = CDate(date1)
                                                'Dim date2 = CDate(date1).AddDays(+Monthly)
                                                Dim date2 As DateTime
                                                If FrequencyFld = "Monthly" Then
                                                    date2 = CDate(date1).AddDays(+Monthly)
                                                ElseIf FrequencyFld = "Quaterly" Then
                                                    date2 = CDate(date1).AddDays(+Quaterly)
                                                End If
                                                lastBusinessDay = Date.Now().ToString("MM/dd/yyyy")
                                                'lastBusinessDay = "3/06/2020"

                                                If date2 >= lastBusinessDay Or date2 <= lastBusinessDayplus5 Then
                                                    insStr = ""
                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",REFTID,isauth) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "','1' );"

                                                    ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,Ouid," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),getdate(),getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' )"
                                                    End If

                                                    da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.Transaction = tran
                                                    If con.State = ConnectionState.Closed Then
                                                        con.Open()
                                                    End If
                                                    res = da.SelectCommand.ExecuteScalar()
                                                    Session("docid") = res
                                                    da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.Transaction = tran
                                                    da.SelectCommand.ExecuteNonQuery()

                                                    da.Fill(ds, "fields")
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
                                                    'calculative fields
                                                    Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
                                                    Dim viewdoc As String = Convert.ToString(names(f))
                                                    viewdoc = viewdoc.Replace(" ", "_")
                                                    If CalculativeField.Length > 0 Then
                                                        For Each CField As DataRow In CalculativeField
                                                            Dim formulaeditorr As New formulaEditor
                                                            Dim forvalue As String = String.Empty
                                                            'Coomented By Komal on 28March2014
                                                            forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
                                                            'forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0)
                                                            Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & res & ""
                                                            da.SelectCommand.CommandText = upquery
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.SelectCommand.ExecuteNonQuery()
                                                        Next
                                                    End If
                                                    '' insert default fiest movement of document - by sunil
                                                    Dim res13 As String = String.Empty
                                                    da.SelectCommand.CommandText = "InsertDefaultMovement"
                                                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                    da.SelectCommand.Parameters.Clear()
                                                    da.SelectCommand.Transaction = tran
                                                    da.SelectCommand.Parameters.AddWithValue("tid", res)
                                                    da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
                                                    da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                                                    da.SelectCommand.ExecuteNonQuery()

                                                    res13 = ob1.GetNextUserFromRolematrixT(res, EID, "30200", "", "30200", con, tran)
                                                    Dim sretMsgArr() As String = res13.Split(":")
                                                    ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)
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
                                                        Dim FormName As String = Convert.ToString(names(f))
                                                        '     Dim EID As Integer = 0
                                                        If (res > 0) And (FormName <> "") Then
                                                            Trigger.ExecuteTriggerT(FormName, EID, res, con, tran)
                                                        End If
                                                    Catch ex As Exception
                                                        Throw
                                                    End Try
                                                End If
                                            End If
                                        End If

                                    End If

                                Next
                            End If

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

                        Next

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

    Function Scheduler(ByVal FTid As Integer) As Boolean
        Dim b As Boolean = False
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            Dim da As New SqlDataAdapter("select ScheduleTime from MMM_MST_AutoInvoiceSetting where tid=" & FTid, con)
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
        Return b
    End Function

    'btn_Sales
    Protected Sub btnAutoInvManual_Click(sender As Object, e As EventArgs) Handles btnAutoInvManual.Click
        Dim Givenstr As Date = "07/16/2020"
        Dim InvGenDt As Date = "07/01/2020"
        Dim dt As Date
        Dim dat As Date
        If Date.TryParse(Givenstr, dt) Then
            Dim newDate As Date = Givenstr.AddMonths(2)
            If Date.TryParse(newDate, dat) Then
                Dim startDt As New Date(dt.Year, dt.Month, 1)
                Dim endDt As New Date(dat.Year, dat.Month, Date.DaysInMonth(dat.Year, dat.Month))
                Dim DaysStayed As Int32 = endDt.Subtract(startDt).Days + 1
                Dim Days As Int32 = endDt.Subtract(Givenstr).Days + 1
                Dim nexMonth = endDt.AddMonths(1)
                Dim StartDate = New Date(nexMonth.Year, nexMonth.Month, 1)
            End If

        End If
        'Dim Result As String() = ParseEscDateFn("1554655", Givenstr, InvGenDt, 30000).Split("=")

        'Dim objRental As New Rentaltool
        'objRental.AutoInvoice()
        LeaseTypeRental()
    End Sub

    Protected Sub LeaseTypeRental()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim insStr As String = String.Empty
        Dim UpdStr As String = String.Empty
        Dim res As Integer = 0
        Dim ob1 As New DMSUtil()
        Dim ob As New DynamicForm
        Dim LRentFreedays As String = String.Empty
        Dim LRentFreedays123 As String = String.Empty
        Dim cLRentFreedays123 As String = String.Empty

        Dim LeaseComDate As String = String.Empty
        Dim LeaseComDate123 As String = String.Empty

        Dim da As New SqlDataAdapter
        Dim dtype As String = String.Empty
        Dim myDate = DateTime.Now
        Dim currdate = myDate.ToString("dd/MM/yy")


        Dim today = DateTime.Today
        Dim month1 = New DateTime(today.Year, today.Month, 1)

        Dim first = month1.AddMonths(-1)
        Dim last = month1.AddDays(-1)
        Dim first3 = month1.AddMonths(-3)
        Dim first6 = month1.AddMonths(-6)

        Dim nextinvdate As String = String.Empty
        Dim startOfMonth = New DateTime(myDate.Year, myDate.Month, 1)
        Dim endOfMonth = startOfMonth.AddMonths(1).AddDays(-1)
        Dim one1date = startOfMonth.ToString("dd/MM/yy")
        Dim one30date = endOfMonth.ToString("dd/MM/yy")
        'DateTime.Now.Date.AddMonths(3)
        Dim ds As New DataSet
        Dim AutoInvDocData As New DataTable
        'Rental/Sd/CAM Inv Gen data
        da = New SqlDataAdapter("select ldoc.tid as 'Dociditem', ldoc.fld17 as 'Lease Doc No','Rent' as 'Lease Type','Rent' as 'InvoiceType',dms.udf_split('MASTER-Vendor Master Test-fld3',mditem.fld1) as 'Landlord Selection',dms.udf_split('MASTER-Operating Unit-fld1',ldoc.fld25) as 'Operating Unit',mditem.fld6 as 'sharedPer',mditem.fld6 as 'vpropshare','Fixed' as 'share mode', 'Monthly' as 'Payment Term',ldoc.fld6  as 'slabEffectivefrom',ldoc.fld7  as 'slabEffectiveto',mditem.fld7 as 'MgAmt',ldoc.fld4 as 'Rate',mditem.FLD5 as 'REVPERCENTAGE',mditem.FLD9 AS 'REVTOBESHARED',  ldoc.fld9 as 'Rent Invoice Generation Date',ldoc.fld9 as 'Rent Start Date','Rent' as 'SlabType', mditem.documenttype as 'childDocumentName','XYZ' as 'Name of Lessor',mditem.fld19 as 'GSTIN NO',ldoc.fld84 'Revenue Sharing Percentage',mditem.fld17 as 'GST Percentage',FORMAT (getdate(), 'dd/MM/yy') as 'Escalation Date',ldoc.fld6 as 'Lease Start Date', ldoc.fld7 as 'Lease End Date',ldoc.fld6 as 'Fit Out Start Date',ldoc.fld9 as 'Fit Out End Date',mditem.fld7 as 'Total MG Amount','' as 'Rent Payment Cycle',ldoc.fld6 as 'Lease Period Start',ldoc.fld7 as 'Lease Period End' from mmm_mst_doc ldoc left join MMM_MST_DOC_ITEM mditem on ldoc.tid=mditem.DOCID  where ldoc.DocumentType='Lease Agreement' and mditem.documenttype='LA Item'  and ldoc.isauth=1 and ldoc.eid=194", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.Fill(ds, "AutoInvDocData")
        Dim dvdistinctLease = New DataView(ds.Tables(0))
        Dim dtdistLease = dvdistinctLease.ToTable(True, "Dociditem")
        Try
            For Each row123 As DataRow In dtdistLease.Rows
                Dim dvleasdate = New DataView(ds.Tables(0))
                dvleasdate.RowFilter = "Dociditem=" & row123("Dociditem")
                Dim Leasdateas = dvleasdate.ToTable()
                'Now find distinct Vendor
                Dim dvvendor = New DataView(ds.Tables(0))
                dvvendor.RowFilter = "Dociditem=" & row123("Dociditem") & " And childDocumentName='LA Item'"
                Dim dtvendor = dvvendor.ToTable()
                Dim dtslabitem = New DataView(ds.Tables(0))
                dtslabitem.RowFilter = "Dociditem=" & row123("Dociditem") & " And childDocumentName='LA Item'"
                Dim newslab = dtslabitem.ToTable(True, "InvoiceType")

                ''count total day between selected your date

                'Lease Start Date Format
                Dim LStartDate123 As New DateTime()
                Dim LSarr123 = Leasdateas.Rows(0).Item("Lease Start Date").Split("/")
                Dim LSdate123 As New Date(LSarr123(2), LSarr123(1), LSarr123(0))
                LSdate123 = CDate(LSdate123)
                Dim LSdt123 As DateTime = Convert.ToDateTime(LSdate123.ToString("MM/dd/yy"), CultureInfo.InvariantCulture)

                'Lease End date Format
                Dim LEndDate123 As New DateTime()
                Dim LEarr123 = Leasdateas.Rows(0).Item("Lease End Date").Split("/")
                Dim LEdate123 As New Date(LEarr123(2), LEarr123(1), LEarr123(0))

                LEdate123 = CDate(LEdate123)
                Dim LEdt123 As DateTime = Convert.ToDateTime(LEdate123.ToString("MM/dd/yy"), CultureInfo.InvariantCulture)
                Dim Newenddate As String = String.Empty
                Newenddate = LEdt123
                Dim LeaseERR As String() = Newenddate.Split("/")
                Dim AddDayInRentFreedays123 As Int32 = 0
                ' AddDayInRentFreedays123 = Convert.ToInt64(LRentFreedays123) + 1
                LeaseComDate123 = LSdt123.AddDays(+AddDayInRentFreedays123)
                Dim LComDate As String = String.Empty
                Dim LCAMComDate As String = String.Empty
                'LComDate = LeaseComDate.ToString()
                Dim StrARR As String() = LeaseComDate123.Split("/")
                LComDate = StrARR(1) + "/" + StrARR(0) + "/" + StrARR(2)
                ' LeaseComDate123 = StrARR(1) + "/" + StrARR(0) + "/" + StrARR(2)
                LeaseComDate123 = StrARR(0) + "/" + StrARR(1) + "/" + StrARR(2)
                For Each drvendor As DataRow In dtvendor.Rows
                    'nextinvdate = LeaseComDate123
                    nextinvdate = StrARR(0) + "/" + StrARR(1) + "/" + StrARR(2)

                    Dim monthyear As New DataTable

                    'Dim dat As Date = Leasdateas.Rows(0).Item("Lease start Date")

                    Dim dat As Date = New Date(Convert.ToInt32(StrARR(2)), Convert.ToInt32(StrARR(0)), Convert.ToInt32(StrARR(1)))
                    Dim startDate As New Date(dat.Year, dat.Month, 1)
                    Dim endDate As Date = New Date(Convert.ToInt32(LeaseERR(2)), Convert.ToInt32(LeaseERR(0)), Convert.ToInt32(LeaseERR(0)))
                    'Dim endDate As New Date(dat.Year, dat.Month, Date.DaysInMonth(dat.Year, dat.Month))
                    'Month and year
                    Dim fromdate As Date = Date.ParseExact(LeaseComDate123, "m/d/yyyy", CultureInfo.InvariantCulture)

                    Dim LeaseAmount = 0.0
                    Dim GstAmount = 0.0
                    Dim Totalamountwithtax = 0.0
                    Dim Col As String = String.Empty
                    Dim value As String = String.Empty
                    Dim MainStringData = String.Empty
                    Dim rntprst As String = String.Empty
                    Dim rntprend As String = String.Empty
                    For Each slabitem As DataRow In newslab.Rows
                        If (slabitem("InvoiceType") = "Rent") Then
                            Dim ds1rent As New DataSet
                            Dim AutoInvDocData1rent As New DataTable
                            da = New SqlDataAdapter("select ldoc.tid as 'Dociditem', ldoc.fld17 as 'Lease Doc No','Rent' as 'Lease Type',dms.udf_split('MASTER-Vendor Master Test-fld3',mditem.fld1) as 'Landlord Selection',dms.udf_split('MASTER-Operating Unit-fld1',ldoc.fld25) as 'Operating Unit',mditem.fld6 as 'sharedPer',mditem.fld6 as 'vpropshare','Fixed' as 'share mode', 'Monthly' as 'Payment Term',ldoc.fld6  as 'slabEffectivefrom',ldoc.fld7  as 'slabEffectiveto',mditem.fld7 as 'MgAmt',ldoc.fld4 as 'Rate',mditem.FLD5 as 'REVPERCENTAGE',mditem.FLD9 AS 'REVTOBESHARED',  ldoc.fld9 as 'Rent Invoice Generation Date',ldoc.fld9 as 'Rent Start Date','Rent' as 'SlabType', mditem.documenttype as 'childDocumentName','XYZ' as 'Name of Lessor',mditem.fld19 as 'GSTIN NO',ldoc.fld84 'Revenue Sharing Percentage',mditem.fld17 as 'GST Percentage',FORMAT (getdate(), 'dd/MM/yy') as 'Escalation Date',ldoc.fld6 as 'Lease Start Date', ldoc.fld7 as 'Lease End Date',ldoc.fld6 as 'Fit Out Start Date',ldoc.fld9 as 'Fit Out End Date',mditem.fld7 as 'Total MG Amount','' as 'Rent Payment Cycle',ldoc.fld6 as 'Lease Period Start',ldoc.fld7 as 'Lease Period End' from mmm_mst_doc ldoc left join MMM_MST_DOC_ITEM mditem on ldoc.tid=mditem.DOCID  where ldoc.DocumentType='Lease Agreement' and mditem.documenttype='LA Item'  and ldoc.isauth=1 and ldoc.eid=194 and ldoc.tid=" & drvendor("Dociditem") & "", con)
                            da.SelectCommand.CommandType = CommandType.Text
                            da.Fill(ds1rent, "AutoInvDocData1rent")
                            Dim dvdistinctLease1rent = New DataView(ds1rent.Tables(0))
                            Dim dtdistLease1rent = dvdistinctLease1rent.ToTable(True, "Dociditem")
                            Try
                                For Each row1231rent As DataRow In dtdistLease1rent.Rows
                                    Dim dvleasdate1rent = New DataView(ds1rent.Tables(0))
                                    dvleasdate1rent.RowFilter = "Dociditem=" & row123("Dociditem")
                                    Dim Leasdateas1rent = dvleasdate1rent.ToTable()
                                    'Now find distinct Vendor
                                    Dim dvvendor1rent = New DataView(ds1rent.Tables(0))
                                    dvvendor1rent.RowFilter = "Dociditem=" & row123("Dociditem") & " And childDocumentName='LA Item'"
                                    Dim dtvendor1rent = dvvendor1rent.ToTable()
                                    Dim Alldatafromleaeserent = New DataView(ds1rent.Tables(0))

                                    ''count total day between selected your date


                                    'Lease Start Date Format
                                    Dim cLStartDate123 As New DateTime()
                                    Dim cLSarr123 = Leasdateas1rent.Rows(0).Item("Lease Start Date").Split("/")
                                    Dim cLSdate123 As New Date(cLSarr123(2), cLSarr123(1), cLSarr123(0))
                                    LSdate123 = CDate(cLSdate123)
                                    Dim cLSdt123 As DateTime = Convert.ToDateTime(LSdate123.ToString("MM/dd/yy"), CultureInfo.InvariantCulture)

                                    'Lease End date Format
                                    Dim cLEndDate123 As New DateTime()
                                    Dim cLEarr123 = Leasdateas1rent.Rows(0).Item("Lease End Date").Split("/")
                                    Dim cLEdate123 As New Date(cLEarr123(2), cLEarr123(1), cLEarr123(0))

                                    cLEdate123 = CDate(cLEdate123)
                                    Dim cLEdt123 As DateTime = Convert.ToDateTime(LEdate123.ToString("MM/dd/yy"), CultureInfo.InvariantCulture)
                                    Dim cNewenddate As String = String.Empty
                                    cNewenddate = cLEdt123
                                    Dim cLeaseERR As String() = cNewenddate.Split("/")
                                    Dim cAddDayInRentFreedays123 As Int32 = 0
                                    'cAddDayInRentFreedays123 = Convert.ToInt64(cLRentFreedays123)
                                    LeaseComDate123 = LSdt123.AddDays(+cAddDayInRentFreedays123)
                                    Dim cLComDate As String = String.Empty
                                    Dim cLCAMComDate As String = String.Empty
                                    'LComDate = LeaseComDate.ToString()
                                    Dim cStrARR As String() = LeaseComDate123.Split("/")
                                    LComDate = cStrARR(1) + "/" + cStrARR(0) + "/" + cStrARR(2)
                                    ' LeaseComDate123 = StrARR(1) + "/" + StrARR(0) + "/" + StrARR(2)
                                    LeaseComDate123 = cStrARR(0) + "/" + cStrARR(1) + "/" + cStrARR(2)
                                    For Each cdrvendorrent As DataRow In dtvendor1rent.Rows
                                        'nextinvdate = LeaseComDate123
                                        nextinvdate = cStrARR(0) + "/" + cStrARR(1) + "/" + cStrARR(2)
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        Dim cmonthyear As New DataTable
                                        'Month and year
                                        Dim cfromdate As Date = Date.ParseExact(LeaseComDate123, "M/d/yyyy", CultureInfo.InvariantCulture)
                                        If (Leasdateas1rent.Rows(0).Item("Payment Term") = "Monthly") Then
                                            Dim creformatted As String = cfromdate.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
                                            da.SelectCommand.Parameters.Clear()
                                            da.SelectCommand.CommandText = "YEARMONTH"
                                            da.SelectCommand.Parameters.AddWithValue("@start", creformatted)
                                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                                            da.Fill(cmonthyear)
                                        End If
                                        Dim Count As Int32 = 0
                                        For Each YearAndMonthrent As DataRow In cmonthyear.Rows
                                            Dim DTAUTOINV As New DataTable
                                            Dim revnueshare As New DataTable
                                            Dim Month = YearAndMonthrent("Month")
                                            Dim Year = YearAndMonthrent("Year")
                                            Dim slabmonth = YearAndMonthrent("slabMonth")
                                            Dim startDtslabrent As New Date(Year, slabmonth, 1)

                                            Dim endDt123slabrent As New Date(Year, slabmonth, Date.DaysInMonth(Year, slabmonth))
                                            Dim abcd As New Date(endDt123slabrent.Year, endDt123slabrent.Month, endDt123slabrent.Day)
                                            Dim slabst As New Date(Year, slabmonth, 1)
                                            Dim Slb As DataTable = New DataTable
                                            Dim endslab As Date
                                            Dim endslabcheck As Date

                                            If (Count = 0) Then
                                                da.SelectCommand.Parameters.Clear()
                                                da.SelectCommand.CommandText = "usp_ApolloLeaseInvoiceForRent"
                                                da.SelectCommand.Parameters.AddWithValue("@Docid", cdrvendorrent("Dociditem"))
                                                da.SelectCommand.Parameters.AddWithValue("@EID", 194)
                                                da.SelectCommand.Parameters.AddWithValue("@filterdate1", cfromdate.ToString("dd/MM/yy"))
                                                da.SelectCommand.Parameters.AddWithValue("@filterdate2", endDt123slabrent.ToString("dd/MM/yy"))
                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                da.Fill(DTAUTOINV)
                                                Alldatafromleaeserent.RowFilter = "Dociditem=" & cdrvendorrent("Dociditem") & " And childDocumentName='LA Item'"
                                                Slb = Alldatafromleaeserent.ToTable()
                                                Dim maindate = Slb.Rows(0).Item("slabEffectiveto").Split("/")
                                                Dim endslab12 As New Date(maindate(2), maindate(1), maindate(0))
                                                endslab12 = CDate(endslab12)
                                                Dim slabeffenddate As DateTime = Convert.ToDateTime(endslab12.ToString("MM/dd/yy"), CultureInfo.InvariantCulture)
                                                endslabcheck = slabeffenddate
                                                Count = Count + 1
                                            Else
                                                da.SelectCommand.Parameters.Clear()
                                                da.SelectCommand.CommandText = "usp_ApolloLeaseInvoiceForRent"
                                                da.SelectCommand.Parameters.AddWithValue("@Docid", cdrvendorrent("Dociditem"))
                                                da.SelectCommand.Parameters.AddWithValue("@EID", 194)
                                                da.SelectCommand.Parameters.AddWithValue("@filterdate1", startDtslabrent.ToString("dd/MM/yy"))
                                                Dim result As Integer = abcd.CompareTo(endslabcheck)
                                                If result > 0 Then
                                                    'da.SelectCommand.Parameters.AddWithValue("@filterdate2", endslabcheck.ToString("dd/MM/yy"))
                                                    da.SelectCommand.Parameters.AddWithValue("@filterdate2", endDt123slabrent.ToString("dd/MM/yy"))
                                                Else
                                                    da.SelectCommand.Parameters.AddWithValue("@filterdate2", endDt123slabrent.ToString("dd/MM/yy"))

                                                End If
                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                da.Fill(DTAUTOINV)
                                                Count = Count + 1
                                            End If
                                            Dim cdtslabrent = DTAUTOINV
                                            Dim strstringData As String = String.Empty
                                            If (cdtslabrent.Rows.Count > 0) Then
                                                MainStringData = ds.Tables("AutoInvDocData").ToString()
                                                For Each Column As DataColumn In dtvendor1rent.Columns
                                                    Col = Column.ColumnName
                                                    value = cdrvendorrent(Column.ColumnName).ToString
                                                    strstringData = strstringData + Col + "::" + value + "|"
                                                Next
                                                Dim cdat As Date
                                                LeaseAmount = (cdtslabrent.Rows(0).Item("MgAmt") * cdrvendorrent("vpropshare")) / 100
                                                Dim FinalInvDate As String = ""
                                                Dim Days As Int32 = 0
                                                Dim DaysStayed As Int32 = 0
                                                If (cdtslabrent.Rows(0).Item("Payment Term") = "Monthly") Then
                                                    If Date.TryParse(nextinvdate, cdat) Then
                                                        Dim startDt As New Date(cdat.Year, cdat.Month, 1)

                                                        Dim endDt As New Date(cdat.Year, cdat.Month, Date.DaysInMonth(cdat.Year, cdat.Month))
                                                        Dim res542 As Integer = endDt.CompareTo(endslabcheck)
                                                        Dim slbmonths As New Date(endslabcheck.Year, endslabcheck.Month, 1)
                                                        If res542 > 0 Then
                                                            Days = endslabcheck.Subtract(slbmonths).Days + 1
                                                            DaysStayed = endDt.Subtract(startDt).Days + 1
                                                        Else
                                                            Days = endDt.Subtract(nextinvdate).Days + 1
                                                            DaysStayed = endDt.Subtract(startDt).Days + 1
                                                        End If

                                                        Dim nexMonth = startDt.AddMonths(1)
                                                        Dim cStartDate = New Date(nexMonth.Year, nexMonth.Month, 1)
                                                        nextinvdate = nexMonth
                                                        rntprst = startDt.ToString("dd/MM/yy")
                                                        rntprend = endDt.ToString("dd/MM/yy")
                                                        FinalInvDate = startDate
                                                        ' calculating rent Amount
                                                        If Count = 1 Then
                                                            LeaseAmount = LeaseAmount
                                                            LeaseAmount = LeaseAmount / DaysStayed
                                                            LeaseAmount = LeaseAmount * Days
                                                            LeaseAmount = Decimal.Round(LeaseAmount, 2, MidpointRounding.AwayFromZero)
                                                            LeaseAmount = Math.Round(LeaseAmount, 2)
                                                        Else
                                                            LeaseAmount = LeaseAmount
                                                            LeaseAmount = Decimal.Round(LeaseAmount, 2, MidpointRounding.AwayFromZero)
                                                            LeaseAmount = Math.Round(LeaseAmount, 2)
                                                        End If

                                                    End If

                                                End If


                                            End If
                                            If Count = 1 Then
                                                'cLRentFreedays123
                                                rntprst = Leasdateas1rent.Rows(0).Item("Rent Start Date")
                                                ' rntprst = CDate(rntprst).AddDays(+cLRentFreedays123).ToString("dd/MM/yy")
                                                'strstringData = strstringData & "Invoice Month" & "::" & Month & "|" & "Invoice Year" & "::" & Year & "|" & "Next Invoice Creation Date" & "::" & nextinvdate & "|" & "Rent Period Start" & "::" & rntprst & "|" & "Rent Period End" & "::" & rntprend & "|" & "Rent Amount" & "::" & LeaseAmount & "|" & "GST Amount" & "::" & GstAmount & "|" & "Total Rent (PM)" & "::" & Totalamountwithtax & "|" & "Invoice Type" & "::" & slabitem("SALBTYPE")
                                                strstringData = strstringData & "Rent Period Start" & "::" & rntprst & "|" & "Rent Period End" & "::" & rntprend & "|" & "Rent Amount" & "::" & LeaseAmount & "|" & "Total Rent (PM)" & "::" & Totalamountwithtax
                                            Else
                                                'strstringData = strstringData & "Invoice Month" & "::" & Month & "|" & "Invoice Year" & "::" & Year & "|" & "Next Invoice Creation Date" & "::" & nextinvdate & "|" & "Rent Period Start" & "::" & rntprst & "|" & "Rent Period End" & "::" & rntprend & "|" & "Rent Amount" & "::" & LeaseAmount & "|" & "GST Amount" & "::" & GstAmount & "|" & "Total Rent (PM)" & "::" & Totalamountwithtax & "|" & "Invoice Type" & "::" & slabitem("SALBTYPE")
                                                strstringData = strstringData & "Rent Period Start" & "::" & rntprst & "|" & "Rent Period End" & "::" & rntprend & "|" & "Rent Amount" & "::" & LeaseAmount & "|" & "Total Rent (PM)" & "::" & Totalamountwithtax
                                            End If

                                            If (LeaseAmount > 0) Then
                                                Dim Result123 = CommanUtil.ValidateParameterByDocumentType(Session("EID"), "Lease Invoice", Session("UID"), strstringData)
                                                CommanUtil.SaveServicerequest(New StringBuilder(strstringData), "Lease Invoice", "SaveData", Result123)
                                            End If

                                        Next
                                    Next
                                Next
                            Catch ex As Exception
                                Continue For
                            End Try
                        End If
                    Next
                Next
            Next

        Catch ex As Exception
            ex.ToString()
        End Try

    End Sub

    Public Sub Error_log(ByVal err As String)
        'If Not Directory.Exists("E:\MyeDMSError_log") Then
        '    Directory.CreateDirectory("E:\MyeDMSError_log")
        'End If
        'Dim file As File
        'Dim fname As String = "FTP" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond
        'fname = "E:\MyeDMSError_log\" & fname & ".txt"
        'file.Create(fname).Dispose()
        'Dim objWriter As New System.IO.StreamWriter(fname, True)
        'objWriter.WriteLine(err)
        'objWriter.Close()
        'objWriter.Dispose()
    End Sub

End Class
