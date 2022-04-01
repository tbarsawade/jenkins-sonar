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

<Assembly: DebuggerDisplay("{ToString}", Target:=GetType(Date))>

Partial Class testSP
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
        ' Call CreaHCLUserPwd()
        ' Call StartServiceCode()
        ' Call MoveFile(7, 34, 0, "TestInv_30Nov1.PDF", 192, 434344, "Vendor Invoice", "fld15", "fld14", 1116432, "fld17", "TestInv_30Nov")
        'Try
        '    'Call sendMail1("sunil.pareek@myndsol.com", "", "", "Test mail for Name instead of email ID", "Welcome to World!")
        '    ' Call genHCLpendencyXML()

        '    '   Call DownloadMulitFiles()

        '    Dim name As String = "Sunil Pareek" 'Session("USERNAME")
        '    Dim email As String = "sunil.pareek@myndsol.com" '  Session("EMAIL").ToString()
        '    Dim org As String = ""
        '    Dim HDurl As String = ""
        '    Dim token As String = ""

        '    If email.ToString() = "" Then
        '        ' lblPageName.Text = "Please update your email ID to Use Support Center"
        '        Label1.Text = "Invalid Email ID to Use Support Center, please update in your profile"
        '        Exit Sub
        '    End If
        '    If email.Length > 2 Then
        '        If Not email.Contains("@") Then
        '            'lblPageName.Visible = t
        '            Label1.Text = "Invalid Email ID to Use Support Center, please update in your profile"

        '            Exit Sub
        '        End If
        '    Else
        '        Label1.Text = "Invalid Email ID to Use Support Center, please update in your profile"
        '        Exit Sub
        '    End If


        '    '' get org name and url from database  by sunil on 02/aug/13
        '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        '    Dim con As SqlConnection = New SqlConnection(conStr)
        '    '  Dim sqlq As String = ""
        '    '  sqlq = "SELECT organization,helpdeskurl,helpdeskToken FROM TDMS_MST_COMPANY where CID='" & Session("CID") & "'"
        '    '  Dim da As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
        '    '  Dim dt As New DataTable
        '    ' da.Fill(dt)

        '    ' If dt.Rows.Count > 0 Then
        '    HDurl = "https://agilenthelp.zendesk.com/access/jwt" ' dt.Rows(0).Item("helpdeskurl").ToString
        '    org = "Agilent"  'dt.Rows(0).Item("organization").ToString
        '    token = "M1AATNmOB1Ds34PYBCSlG75N3gTx9JqvFMLDDaf59TOJ3hLi" ' dt.Rows(0).Item("helpdeskToken").ToString
        '    ' End If
        '    ' da.Dispose()
        '    ' dt.Dispose()
        '    con.Dispose()

        '    Dim t As TimeSpan = (DateTime.UtcNow - New DateTime(1970, 1, 1))
        '    Dim timestamp As Integer = CInt(Math.Truncate(t.TotalSeconds))

        '    Dim payload = New Dictionary(Of String, Object)  '() From  {{"iat", timestamp}, {"jti", System.Guid.NewGuid().ToString()}, {"name", name}, {"email", email}}

        '    payload.Add("iat", timestamp)
        '    payload.Add("jti", System.Guid.NewGuid().ToString())
        '    payload.Add("name", name)
        '    payload.Add("email", email)

        '    token = JWT.JsonWebToken.Encode(payload, token, JWT.JwtHashAlgorithm.HS256)
        '    Dim redirectUrl As String = HDurl & "?jwt=" & token
        '    Response.Redirect(redirectUrl)

        'Catch ex As Exception
        '    Label1.Text = "Error in mail sending"
        'End Try
        '' Label1.Text = "Mail sent successfully"

        ''Call createhclusers()
        ' Call CreaHCLUserPwd()

    End Sub

    Private Shared Sub DownloadMulitFiles()
        Dim ftpRequest As FtpWebRequest = DirectCast(WebRequest.Create("ftp://103.25.172.228/Location1/"), FtpWebRequest)
        ftpRequest.Credentials = New NetworkCredential("Ferring", "BNx$724dE")
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory
        Dim response As FtpWebResponse = DirectCast(ftpRequest.GetResponse(), FtpWebResponse)
        Dim streamReader As New StreamReader(response.GetResponseStream())
        Dim directories As New List(Of String)()

        Dim line As String = streamReader.ReadLine()
        While Not String.IsNullOrEmpty(line)
            directories.Add(line)
            line = streamReader.ReadLine()
        End While
        streamReader.Close()


        Using ftpClient As New WebClient()
            ftpClient.Credentials = New System.Net.NetworkCredential("Ferring", "BNx$724dE")

            For i As Integer = 0 To directories.Count - 1
                If directories(i).Contains(".") Then

                    Dim path As String = "ftp://103.25.172.228/Location1/" + directories(i).ToString()
                    Dim trnsfrpth As String = "E:\\MYNDSOLUTION_BPM\" + directories(i).ToString()
                    ftpClient.DownloadFile(path, trnsfrpth)
                End If
            Next
        End Using
    End Sub

    ' ''''' method 1
    'Private Sub DownloadFtpDirectory(url As String, credentials As NetworkCredential, localPath As String)
    '    Dim listRequest As FtpWebRequest = DirectCast(WebRequest.Create(url), FtpWebRequest)
    '    listRequest.UsePassive = True
    '    listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails
    '    listRequest.Credentials = credentials

    '    Dim lines As New List(Of String)()

    '    Using listResponse As FtpWebResponse = DirectCast(listRequest.GetResponse(), FtpWebResponse)
    '        Using listStream As Stream = listResponse.GetResponseStream()
    '            Using listReader As New StreamReader(listStream)
    '                While Not listReader.EndOfStream
    '                    lines.Add(listReader.ReadLine())
    '                End While
    '            End Using
    '        End Using
    '    End Using

    '    For Each line As String In lines
    '    Dim tokens As String() = line.Split(New () {" "C}, 9, StringSplitOptions.RemoveEmptyEntries)
    '        Dim name As String = tokens(8)
    '        Dim permissions As String = tokens(0)

    '        Dim localFilePath As String = Path.Combine(localPath, name)
    '        Dim fileUrl As String = url & name

    '        If permissions(0) = "d"c Then
    '            Directory.CreateDirectory(localFilePath)
    '            DownloadFtpDirectory(fileUrl & Convert.ToString("/"), credentials, localFilePath)
    '        Else
    '            Dim downloadRequest As FtpWebRequest = DirectCast(WebRequest.Create(fileUrl), FtpWebRequest)
    '            downloadRequest.UsePassive = True
    '            downloadRequest.UseBinary = True
    '            downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile
    '            downloadRequest.Credentials = credentials

    '            Using downloadResponse As FtpWebResponse = DirectCast(downloadRequest.GetResponse(), FtpWebResponse)
    '                Using downloadReader As New StreamReader(downloadResponse.GetResponseStream())
    '                    Using writer As New StreamWriter(localFilePath)
    '                        writer.Write(downloadReader.ReadToEnd())
    '                    End Using
    '                End Using
    '            End Using
    '        End If
    '    Next
    'End Sub

    Private Sub genHCLpendencyXML()
        Try
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

                                        ' prev Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code],fld180[Payment Terms],fld181[INCOTerms],fld32[Location],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],fld160[WBS Description],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],fld110[sep3forPO],fld107[Sep2forPO],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],fld106[sep1forPO],fld111[sep4forPO],fld95[sep5forPO],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld42,0) from mmm_mst_master where tid=isnull(d.fld42,0) and documenttype='Doc Nature')) [Doc Nature],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [Company Code],fld3[RAO Remarks],fld15[Company Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld136,0) from mmm_mst_master where tid=isnull(d.fld136,0) and documenttype='Profit Center')) [Profit Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld135,0) from mmm_mst_master where tid=isnull(d.fld135,0) and documenttype='Cost Center')) [Cost Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],fld58[Email Of User], (select fld1 from mmm_mst_master where tid = (select ISNULL(NULLIF(d.fld2, 0), 0)  from mmm_mst_master where tid=ISNULL(NULLIF(d.fld2, 0), 0) and documenttype='currency' ))[Currency] ,fld31[Dispatch Remarks],fld34[Physical Doc (Recd)],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld12[SSC Processing Date],fld182[Service Tax Categor],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld124[Low TDS Applicable],fld125[Low TDS Certificate],fld129[Remarks If any],fld130[Invoicing Milestone Description],fld108[Sep1forChklist],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[Clarification Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Remarks by HCL Receipt User],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld52[Processor Cancellation Remarks],fld53[Processor Reconsider Remarks],fld86[Warranty Certificate Attachment],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld75[Technical Compliance Certificate],fld4[Courier Docket No.],fld30[Dispatch Date],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='Rejection Reasons')) [Rejection Reason],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld51[Query Sent To],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld57[Parking Date],fld79[Proof of Electronics delivery],fld36[SSC Processing Remarks],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld13[Other Deduction],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld6,0) from mmm_mst_master where tid=isnull(d.fld6,0) and documenttype='Clarification User')) [User Email],fld67[Open Amount],fld7[Fiscal Year Posted],fld62[HC Rejection Remarks],fld63[Invoice Received Date],fld43[MIGO No],fld64[Dispatch Reconsider Remarks],fld65[QC Reconsider Remarks],( select username from mmm_mst_user where  uid=d.fld50) [Clarification by user],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld185[Monthly Fee],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld163[Supporting Attachment],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld206[PO Deleted],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld208[PO Date] from MMM_MST_DOC d  " & " where tid in (" & ColList & ") order by [Invoice Type] asc"
                                        Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code],fld180[Payment Terms],fld181[INCOTerms],fld32[Location],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],fld160[WBS Description],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],fld110[sep3forPO],fld107[Sep2forPO],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],fld106[sep1forPO],fld111[sep4forPO],fld95[sep5forPO],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld42,0) from mmm_mst_master where tid=isnull(d.fld42,0) and documenttype='Doc Nature')) [Doc Nature],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [Company Code],fld3[RAO Remarks],fld15[Company Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld136,0) from mmm_mst_master where tid=isnull(d.fld136,0) and documenttype='Profit Center')) [Profit Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld135,0) from mmm_mst_master where tid=isnull(d.fld135,0) and documenttype='Cost Center')) [Cost Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],fld58[Email Of User], (select fld1 from mmm_mst_master where tid = (select ISNULL(NULLIF(d.fld2, 0), 0)  from mmm_mst_master where tid=ISNULL(NULLIF(d.fld2, 0), 0) and documenttype='currency' ))[Currency] ,fld31[Dispatch Remarks],fld34[Physical Doc (Recd)],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld12[SSC Processing Date],fld182[Service Tax Categor],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld124[Low TDS Applicable],fld125[Low TDS Certificate],fld129[Remarks If any],fld130[Invoicing Milestone Description],fld108[Sep1forChklist],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[Clarification Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Remarks by HCL Receipt User],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld52[Processor Cancellation Remarks],fld53[Processor Reconsider Remarks],fld86[Warranty Certificate Attachment],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld75[Technical Compliance Certificate],fld4[Courier Docket No.],fld30[Dispatch Date],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='Rejection Reasons')) [Rejection Reason],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld51[Query Sent To],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld57[Parking Date],fld79[Proof of Electronics delivery],fld36[SSC Processing Remarks],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld13[Other Deduction],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld6,0) from mmm_mst_master where tid=isnull(d.fld6,0) and documenttype='Clarification User')) [User Email],fld67[Open Amount],fld7[Fiscal Year Posted],fld62[HC Rejection Remarks],fld63[Invoice Received Date],fld43[MIGO No],fld64[Dispatch Reconsider Remarks],fld65[QC Reconsider Remarks],( select username from mmm_mst_user where  uid=d.fld50) [Clarification by user],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld185[Monthly Fee],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld163[Supporting Attachment],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld206[PO Deleted],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld208[PO Date] from MMM_MST_DOC d  " & " where tid in (" & ColList & ") order by [Invoice Type] asc"
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
                                sendMail1(ToUser, "", "", MailSubject, finalBody)
                                'sendMail1(TOs, "", "mayank.garg@myndsol.com,sunil.pareek@myndsol.com", MailSubject, finalBody)
                            End If
                        Catch ex As Exception
                            objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " and current userid is:-" & curUser & "  ',getdate()," & eid & ")")
                        End Try
                    Next
                Catch ex As Exception
                    objDC.ExecuteQryDT("insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('Hcl_VENDOR_INVOICE_VP_alert_REMINDER','HCL alerts AP','" & ex.InnerException.ToString() & " and workflow status is " & arrWFStatus(i).ToString() & " ',getdate()," & eid & ")")
                End Try
            Next

        Catch ex As Exception
            Dim finalBody As String = "Error in HCL Reminder mail sending <br/> " & "Error message is - " & Convert.ToString(ex.Message) & "<br/> Inner message is - " & Convert.ToString(ex.InnerException.Message)
            sendMail1("sunil.pareek@myndsol.com", "", "", "Error in HCL Reminder mail sending", finalBody)
        End Try
    End Sub

    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim Email As New System.Net.Mail.MailMessage("Do Not Reply", Mto, MSubject, MBody) '
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
            End If

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


    Private Sub CreaHCLUserPwd()
        Dim constr As String = "server=MYNDHOSTDBVIP;initial catalog=DMS;uid=DMS;pwd=Gtr$698B#"
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtSrc As New DataTable()
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim supervisoremail As String = ""
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.Text
            'original to be used tomorrow - 
            da.SelectCommand.CommandText = "select * From mmm_mst_USER where eid=88 and userrole not in ('su','admin')"
            ' da.SelectCommand.CommandText = "select uid  From mmm_mst_user where eid=46 and uid in(11880,11881)"
            da.Fill(dtSrc)

            Dim obj As New User

            If dtSrc.Rows.Count > 0 Then
                For i As Integer = 0 To dtSrc.Rows.Count - 1


                    Dim pwd As String
                    Dim sKey As Integer
                    Dim Generator As System.Random = New System.Random()
                    sKey = Generator.Next(10000, 99999)
                    pwd = Generator.Next(100000, 999999)
                    Dim encPwd As String = obj.EncryptTripleDES(pwd, sKey)


                    Dim uid As String = dtSrc.Rows(i).Item("UID").ToString()

                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If

                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "update mmm_mst_user set isauth=1, modifydate=getdate(), passtry=0, pwd='" & encPwd & "', skey='" & sKey & "', FLD100='" & pwd & "' where eid=88 and uid='" & uid & "'"
                    da.SelectCommand.CommandType = CommandType.Text
                    Dim RES As String = da.SelectCommand.ExecuteNonQuery()
                Next
            End If
            Label1.Text = "successful"
        Catch ex As Exception
            '   Call UpdateLoginDB("Error in Sendalertmail function msg is - " & ex.InnerException.Message)
            Label1.Text = "Errrrrrrrrrrrrrrrr- " & ex.Message().ToString
        Finally
            con.Close()
            da.Dispose()
        End Try


    End Sub

    'Public Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
    '    Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
    '    Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

    '    ' scramble the key
    '    sKey = ScrambleKey(sKey)
    '    ' Compute the MD5 hash.
    '    DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
    '    ' Set the cipher mode.
    '    DES.Mode = System.Security.Cryptography.CipherMode.ECB
    '    ' Create the decryptor.
    '    Dim DESDecrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateDecryptor()
    '    Dim Buffer As Byte() = Convert.FromBase64String(sOut)
    '    ' Transform and return the string.
    '    Return System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    'End Function
    'Private Function ScrambleKey(ByVal v_strKey As String) As String
    '    Dim sbKey As New System.Text.StringBuilder
    '    Dim intPtr As Integer
    '    For intPtr = 1 To v_strKey.Length
    '        Dim intIn As Integer = v_strKey.Length - intPtr + 1
    '        sbKey.Append(Mid(v_strKey, intIn, 1))
    '    Next
    '    Dim strKey As String = sbKey.ToString
    '    Return sbKey.ToString
    'End Function
    'Public Function EncryptTripleDES(ByVal sIn As String, ByVal sKey As String) As String
    '    Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
    '    Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

    '    ' scramble the key
    '    sKey = ScrambleKey(sKey)
    '    ' Compute the MD5 hash.
    '    DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
    '    ' Set the cipher mode.
    '    DES.Mode = System.Security.Cryptography.CipherMode.ECB
    '    ' Create the encryptor.
    '    Dim DESEncrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateEncryptor()
    '    ' Get a byte array of the string.
    '    Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn)
    '    ' Transform and return the string.
    '    Return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    'End Function
    Private Sub createhclusers()
        ' Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Gtr$698B#"
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtSrc As New DataTable()
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim supervisoremail As String = ""
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select * from mmm_hcl_test_user order by userid,userrole"

            da.Fill(dtSrc)
            If dtSrc.Rows.Count > 0 Then
                For i As Integer = 0 To dtSrc.Rows.Count - 1
                    Dim userid As String = dtSrc.Rows(i).Item("USERID").ToString()
                    Dim username As String = dtSrc.Rows(i).Item("USERNAME").ToString
                    Dim userrole As String = dtSrc.Rows(i).Item("userrole").ToString
                    Dim emailid As String = dtSrc.Rows(i).Item("EMAILID").ToString
                    Dim phone As String = dtSrc.Rows(i).Item("phone").ToString

                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If

                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "TempHclUserCreation_SP"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("USERID", userid)
                    da.SelectCommand.Parameters.AddWithValue("USERNAME", username)
                    da.SelectCommand.Parameters.AddWithValue("USERROLE", userrole)
                    da.SelectCommand.Parameters.AddWithValue("EMAILID", emailid)
                    da.SelectCommand.Parameters.AddWithValue("PHONE", phone)

                    Dim RES As String = da.SelectCommand.ExecuteScalar()
                Next
            End If
            Label1.Text = "successful"
        Catch ex As Exception
            '   Call UpdateLoginDB("Error in Sendalertmail function msg is - " & ex.InnerException.Message)
        Finally
            con.Close()
            da.Dispose()
        End Try
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


    Public Function MoveFile(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer, ByVal BarCodeFldmapping As String, ByVal barcodefldval As String) As String
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
            da.SelectCommand.Parameters.AddWithValue("docurl", docurl)
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


    Protected Sub btnEnc_Click(sender As Object, e As EventArgs) Handles btnEnc.Click
        TxtREs.Text = EncryptTripleDES(txtEnc.Text, "234342343423434234342343")
    End Sub

    Protected Sub btnDec_Click(sender As Object, e As EventArgs) Handles btnDec.Click
        TxtREs.Text = DecryptTripleDES(txtEnc.Text, "234342343423434234342343")
    End Sub

    Public Function EncryptTripleDES(ByVal sIn As String, ByVal sKey As String) As String
        Try
            Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
            ' scramble the key
            ' Compute the MD5 hash.
            DES.Key = UTF8Encoding.UTF8.GetBytes(sKey)
            ' Set the cipher mode.
            DES.Mode = System.Security.Cryptography.CipherMode.ECB
            ' Create the encryptor.
            Dim DESEncrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateEncryptor()
            ' Get a byte array of the string.
            Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn)
            ' Transform and return the string.
            Return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
        Try
            Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
            DES.Key = UTF8Encoding.UTF8.GetBytes(sKey)
            ' Set the cipher mode.
            DES.Mode = System.Security.Cryptography.CipherMode.ECB
            ' Create the decryptor.
            Dim DESDecrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateDecryptor()
            Dim Buffer As Byte() = Convert.FromBase64String(sOut)
            ' Transform and return the string.
            Return System.Text.UTF8Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
        Catch ex As Exception
            Throw New Exception()
        End Try
    End Function

    Protected Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
        'Dim colValue As String = txtVal.Text

        'If IsNumeric(colValue) Then
        '    lblres.Text = "Numeric"
        'Else
        '    lblres.Text = "Not Numeric"
        'End If

        Dim colValue As String = Convert.ToDecimal(txtVal.Text)

        If colValue Then
            lblres.Text = "Numeric"
        Else
            lblres.Text = "Not Numeric"
        End If

    End Sub
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

    'Protected Sub btnAutoInvoice_Click(sender As Object, e As EventArgs) Handles btnAutoInvoice.Click


    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim insStr As String = String.Empty
    '    Dim UpdStr As String = String.Empty
    '    Dim res As Integer = 0
    '    Dim ob1 As New DMSUtil()
    '    Dim ob As New DynamicForm
    '    Dim tran As SqlTransaction = Nothing
    '    Dim da As New SqlDataAdapter
    '    Try

    '        da = New SqlDataAdapter("select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld from   MMM_MST_AutoInvoiceSetting where  IsActiveStatus=1", con)
    '        '  da.SelectCommand.Transaction = tran
    '        Dim ds As New DataSet
    '        Dim dt As New DataTable
    '        da.Fill(ds, "AutoInvSettingData")
    '        Dim StartDateFld As String = String.Empty
    '        Dim EndDateFld As String = String.Empty
    '        Dim FrequencyFld As String = String.Empty
    '        Dim PeriodFld As Int16 = 0
    '        Dim RentalFld As String = String.Empty
    '        Dim RegistrationDateFld As String = String.Empty
    '        Dim SchedulerTidID As String = String.Empty
    '        Dim Fldstr As String = ""
    '        Dim strTFlds As String = String.Empty
    '        Dim strSFlds As String = String.Empty
    '        Dim strHTFlds As String = String.Empty
    '        Dim strHSFlds As String = String.Empty
    '        Dim RentFreePeriodFlds As String = String.Empty
    '        Dim EID As String = String.Empty
    '        If ds.Tables("AutoInvSettingData").Rows.Count <> 0 Then
    '            For i As Integer = 0 To ds.Tables("AutoInvSettingData").Rows.Count - 1

    '                StartDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("StartDateFld"))
    '                EndDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EndDateFld"))
    '                FrequencyFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("FrequencyFld"))
    '                PeriodFld = Convert.ToInt16(ds.Tables("AutoInvSettingData").Rows(i).Item("PeriodFld"))
    '                RentalFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
    '                RegistrationDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RegistrationDateFld"))
    '                RentFreePeriodFlds = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentFreePeriodFld"))
    '                EID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EID"))
    '                SchedulerTidID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid"))
    '                Dim FldsVal As String = String.Empty
    '                Dim SchedulerCheck As Boolean
    '                SchedulerCheck = Scheduler(SchedulerTidID)
    '                ' SchedulerCheck = True
    '                If SchedulerCheck = True Then
    '                    Dim FldsValArr As String()
    '                    Dim value As String = ""

    '                    da.SelectCommand.CommandText = "select  F.Tid,F.TFld,F.SFld,F.SDocType,F.AutoInvTid,F.EID,F.TFldName,F.sFldName,F.Leasetype,F.TDocType  from    MMM_MST_AutoInvoiceSetting C   inner join MMM_MST_AutoInvFieldSetting F on c.Tid=F.AutoInvTid where C.eid=" & EID & " and C.IsActiveStatus=1 and F.AutoInvTid=" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid")) & ""
    '                    'da.SelectCommand.Transaction = tran
    '                    da.Fill(ds, "AutoInvFieldData")
    '                    Dim valueL As String


    '                    Dim names As List(Of String) = (From row In ds.Tables("AutoInvFieldData").AsEnumerable()
    '                                                    Select row.Field(Of String)("TDocType") Distinct).ToList()

    '                    For f As Integer = 0 To names.Count - 1
    '                        Dim dvs As DataView = ds.Tables("AutoInvFieldData").DefaultView
    '                        dvs.RowFilter = "TDocType='" & names(f) & "'"

    '                        Dim filteredTable As New DataTable()
    '                        filteredTable = dvs.ToTable()
    '                        If filteredTable.Rows.Count <> 0 Then
    '                            strTFlds = ""
    '                            strSFlds = ""
    '                            strTFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray)
    '                            strSFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray)
    '                            Dim strHSFldsArr As String()
    '                            strHSFldsArr = strSFlds.Split(",")
    '                            valueL = String.Join(",',',", strHSFldsArr)
    '                            strHSFlds = valueL
    '                        End If
    '                        Dim myDt As DateTime = DateTime.Now

    '                        Dim xmlDate As String = myDt.ToString("dd/MM/yy")
    '                        xmlDate = "30/01/20"
    '                        da.SelectCommand.CommandText = " select  tid,convert(varchar, convert(datetime, " & RegistrationDateFld & ", 3), 103)  as RegistrationDate ," & RentFreePeriodFlds & " as RentFreePeriod from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & " where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and convert(varchar(200),getdate(),3) between " & StartDateFld & " and " & EndDateFld & " and reftid<>''"
    '                        ' da.SelectCommand.CommandText = " select  tid,convert(varchar, convert(datetime, " & RegistrationDateFld & ", 3), 103) as RegistrationDate ," & RentalFld & " as RentalAmount   from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & " where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and '" & xmlDate & "' between " & StartDateFld & " and " & EndDateFld & ""
    '                        da.Fill(ds, "AutoInvDocData")
    '                        Dim SourceDocData As New DataTable
    '                        Dim RDOCIDData As New DataTable
    '                        'RDOCIDData
    '                        If ds.Tables("AutoInvDocData").Rows.Count <> 0 Then
    '                            con.Open()
    '                            tran = con.BeginTransaction()
    '                            For j As Integer = 0 To ds.Tables("AutoInvDocData").Rows.Count - 1


    '                                If SourceDocData.Rows.Count > 0 Then
    '                                    SourceDocData.Clear()
    '                                End If
    '                                da.SelectCommand.CommandText = " select tid as RDOCID,concat(" & strHSFlds & ") as TFldVAl from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & "   where documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "'  and eid=" & EID & " and tid=" & ds.Tables("AutoInvDocData").Rows(j).Item("Tid") & " "
    '                                da.SelectCommand.CommandType = CommandType.Text
    '                                da.SelectCommand.Transaction = tran
    '                                da.Fill(ds, "SourceDocData")
    '                                SourceDocData = ds.Tables("SourceDocData")
    '                                Dim RDOCID As String = ""
    '                                If ds.Tables("SourceDocData").Rows.Count > 0 Then
    '                                    FldsVal = ""
    '                                    FldsVal = SourceDocData.Rows(0).Item("TFldVAl")
    '                                    FldsValArr = FldsVal.Split(",")
    '                                    value = ""
    '                                    value = String.Join("','", FldsValArr)
    '                                    value = "'" + value + "'"
    '                                    RDOCID = Convert.ToString(SourceDocData.Rows(0).Item("RDOCID"))
    '                                End If
    '                                If ds.Tables("SourceDocData").Rows.Count > 0 Then
    '                                    If RDOCIDData.Rows.Count > 0 Then
    '                                        RDOCIDData.Clear()
    '                                    End If
    '                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then
    '                                        da.SelectCommand.CommandText = " select * from MMM_MST_MASTER where RefTid=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "' "
    '                                    Else
    '                                        da.SelectCommand.CommandText = " select * from MMM_MST_DOC where RDOCID=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "'"
    '                                    End If
    '                                    da.SelectCommand.CommandType = CommandType.Text

    '                                    da.SelectCommand.Transaction = tran
    '                                    da.Fill(ds, "RDOCIDData")
    '                                    RDOCIDData = ds.Tables("RDOCIDData")
    '                                    If ds.Tables("RDOCIDData").Rows.Count = 0 Then
    '                                        If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("LastInvoceGenrationDate")) = "" Then
    '                                            'Plus 30 days
    '                                            Dim Monthly As Double = 30
    '                                            Dim Quaterly As Double = 90
    '                                            ' Dim ActualInvoiceDate As Date = Date.ParseExact("18-12-2007", ds.Tables("AutoInvDocData").Rows(j).Item("RegistrationDate"), System.Globalization.CultureInfo.InvariantCulture)
    '                                            Dim myString As String = ds.Tables("AutoInvDocData").Rows(j).Item("RegistrationDate")
    '                                            Dim lastBusinessDay As New DateTime()
    '                                            Dim lastBusinessDayplus5 As New DateTime()
    '                                            Dim arr = myString.Split("/")
    '                                            Dim date1 As New Date(arr(2), arr(1), arr(0))
    '                                            date1 = CDate(date1)
    '                                            Dim RentFreePeriod As Integer
    '                                            Dim date2 As DateTime
    '                                            If Convert.ToString(ds.Tables("AutoInvDocData").Rows(j).Item("RentFreePeriod")) = "" Then
    '                                                RentFreePeriod = 0

    '                                                If FrequencyFld = "Monthly" Then
    '                                                    date2 = CDate(date1).AddDays(+Monthly)
    '                                                ElseIf FrequencyFld = "Quaterly" Then
    '                                                    date2 = CDate(date1).AddDays(+Quaterly)
    '                                                End If
    '                                            Else
    '                                                RentFreePeriod = Convert.ToInt16(ds.Tables("AutoInvDocData").Rows(j).Item("RentFreePeriod"))
    '                                                date2 = CDate(date1).AddDays(+RentFreePeriod)
    '                                            End If

    '                                            lastBusinessDay = Date.Now().ToString("MM/dd/yyyy")
    '                                            lastBusinessDayplus5 = CDate(lastBusinessDay).AddDays(+5)
    '                                            ' lastBusinessDay = "2/05/2020"
    '                                            If date2 >= lastBusinessDay Or date2 <= lastBusinessDayplus5 Then
    '                                                insStr = ""

    '                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

    '                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
    '                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

    '                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),getdate(),getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
    '                                                End If
    '                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
    '                                                da.SelectCommand.CommandType = CommandType.Text
    '                                                da.SelectCommand.Transaction = tran
    '                                                If con.State = ConnectionState.Closed Then
    '                                                    con.Open()
    '                                                End If
    '                                                res = da.SelectCommand.ExecuteScalar()
    '                                                Session("docid") = res
    '                                                da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
    '                                                da.SelectCommand.CommandType = CommandType.Text
    '                                                da.SelectCommand.Transaction = tran
    '                                                da.SelectCommand.ExecuteNonQuery()

    '                                                da.Fill(ds, "fields")
    '                                                'Fieldtype='Auto Number'
    '                                                Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
    '                                                If row.Length > 0 Then
    '                                                    For l As Integer = 0 To row.Length - 1
    '                                                        Select Case row(l).Item("fieldtype").ToString
    '                                                            Case "Auto Number"
    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                                da.SelectCommand.Transaction = tran
    '                                                                da.SelectCommand.CommandText = "usp_GetAutoNoNew"
    '                                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                                                da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("docid", res)
    '                                                                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '                                                                da.SelectCommand.ExecuteScalar()

    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                            Case "New Auto Number"
    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                                da.SelectCommand.Transaction = tran
    '                                                                da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
    '                                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                                                da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("SearchFldid", row(l).Item("dropdown").ToString)
    '                                                                da.SelectCommand.Parameters.AddWithValue("docid", res)
    '                                                                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '                                                                da.SelectCommand.ExecuteScalar()

    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                        End Select
    '                                                    Next
    '                                                End If
    '                                                'calculative fields
    '                                                Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
    '                                                Dim viewdoc As String = Convert.ToString(names(f))
    '                                                viewdoc = viewdoc.Replace(" ", "_")
    '                                                If CalculativeField.Length > 0 Then
    '                                                    For Each CField As DataRow In CalculativeField
    '                                                        Dim formulaeditorr As New formulaEditor
    '                                                        Dim forvalue As String = String.Empty
    '                                                        'Coomented By Komal on 28March2014
    '                                                        forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
    '                                                        ' forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0)
    '                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & res & ""
    '                                                        da.SelectCommand.CommandText = upquery
    '                                                        da.SelectCommand.CommandType = CommandType.Text
    '                                                        da.SelectCommand.ExecuteNonQuery()
    '                                                    Next
    '                                                End If
    '                                                '' insert default fiest movement of document - by sunil

    '                                                da.SelectCommand.CommandText = "InsertDefaultMovement"
    '                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                                da.SelectCommand.Parameters.Clear()
    '                                                da.SelectCommand.Transaction = tran
    '                                                da.SelectCommand.Parameters.AddWithValue("tid", res)
    '                                                da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
    '                                                da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
    '                                                da.SelectCommand.ExecuteNonQuery()
    '                                                'Dim ob1 As New DMSUtil()
    '                                                Dim res12 As String = String.Empty
    '                                                'Dim ob As New DynamicForm
    '                                                res12 = ob1.GetNextUserFromRolematrixT(res, EID, "30200", "", "30200", con, tran)
    '                                                Dim sretMsgArr() As String = res12.Split(":")
    '                                                ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)
    '                                                If sretMsgArr(0) = "ARCHIVE" Then
    '                                                    'Dim Op As New Exportdata()
    '                                                    'Op.PushdataT(res, sretMsgArr(1), EID, con, tran)
    '                                                End If

    '                                                '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
    '                                                If sretMsgArr(0).ToUpper() = "NO SKIP" Then
    '                                                    Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
    '                                                    'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
    '                                                    'this code block is added by ajeet kumar for transaction to be rolled back
    '                                                    ''''tran.Rollback()
    '                                                    ''''Exit Sub
    '                                                Else

    '                                                End If
    '                                                'Execute Trigger
    '                                                Try
    '                                                    Dim FormName As String = Convert.ToString(names(f))
    '                                                    '     Dim EID As Integer = 0
    '                                                    If (res > 0) And (FormName <> "") Then
    '                                                        Trigger.ExecuteTriggerT(FormName, EID, res, con, tran)
    '                                                    End If
    '                                                Catch ex As Exception
    '                                                    Throw
    '                                                End Try


    '                                            End If

    '                                        Else
    '                                            Dim myString As String = ds.Tables("AutoInvSettingData").Rows(i).Item("LastInvoceGenrationDate")
    '                                            Dim lastBusinessDay As New DateTime()
    '                                            Dim lastBusinessDayplus5 As New DateTime()

    '                                            Dim arr = myString.Split("/")
    '                                            Dim date1 As New Date(arr(2), arr(1), arr(0))
    '                                            Dim Monthly As Double = 30
    '                                            Dim Quaterly As Double = 90
    '                                            date1 = CDate(date1)
    '                                            'Dim date2 = CDate(date1).AddDays(+Monthly)
    '                                            Dim date2 As DateTime
    '                                            If FrequencyFld = "Monthly" Then
    '                                                date2 = CDate(date1).AddDays(+Monthly)
    '                                            ElseIf FrequencyFld = "Quaterly" Then
    '                                                date2 = CDate(date1).AddDays(+Quaterly)
    '                                            End If
    '                                            lastBusinessDay = Date.Now().ToString("MM/dd/yyyy")
    '                                            'lastBusinessDay = "3/06/2020"

    '                                            If date2 >= lastBusinessDay Or date2 <= lastBusinessDayplus5 Then
    '                                                insStr = ""
    '                                                If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

    '                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",REFTID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"

    '                                                ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

    '                                                    insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,Ouid," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),getdate(),getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
    '                                                End If

    '                                                da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
    '                                                da.SelectCommand.CommandType = CommandType.Text
    '                                                da.SelectCommand.Transaction = tran
    '                                                If con.State = ConnectionState.Closed Then
    '                                                    con.Open()
    '                                                End If
    '                                                res = da.SelectCommand.ExecuteScalar()
    '                                                Session("docid") = res
    '                                                da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
    '                                                da.SelectCommand.CommandType = CommandType.Text
    '                                                da.SelectCommand.Transaction = tran
    '                                                da.SelectCommand.ExecuteNonQuery()

    '                                                da.Fill(ds, "fields")
    '                                                'Fieldtype='Auto Number'
    '                                                Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
    '                                                If row.Length > 0 Then
    '                                                    For l As Integer = 0 To row.Length - 1
    '                                                        Select Case row(l).Item("fieldtype").ToString
    '                                                            Case "Auto Number"
    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                                da.SelectCommand.Transaction = tran
    '                                                                da.SelectCommand.CommandText = "usp_GetAutoNoNew"
    '                                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                                                da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("docid", res)
    '                                                                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '                                                                da.SelectCommand.ExecuteScalar()

    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                            Case "New Auto Number"
    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                                da.SelectCommand.Transaction = tran
    '                                                                da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
    '                                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                                                da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("SearchFldid", row(l).Item("dropdown").ToString)
    '                                                                da.SelectCommand.Parameters.AddWithValue("docid", res)
    '                                                                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
    '                                                                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
    '                                                                da.SelectCommand.ExecuteScalar()

    '                                                                da.SelectCommand.Parameters.Clear()
    '                                                        End Select
    '                                                    Next
    '                                                End If
    '                                                'calculative fields
    '                                                Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
    '                                                Dim viewdoc As String = Convert.ToString(names(f))
    '                                                viewdoc = viewdoc.Replace(" ", "_")
    '                                                If CalculativeField.Length > 0 Then
    '                                                    For Each CField As DataRow In CalculativeField
    '                                                        Dim formulaeditorr As New formulaEditor
    '                                                        Dim forvalue As String = String.Empty
    '                                                        'Coomented By Komal on 28March2014
    '                                                        forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
    '                                                        'forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0)
    '                                                        Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & res & ""
    '                                                        da.SelectCommand.CommandText = upquery
    '                                                        da.SelectCommand.CommandType = CommandType.Text
    '                                                        da.SelectCommand.ExecuteNonQuery()
    '                                                    Next
    '                                                End If
    '                                                '' insert default fiest movement of document - by sunil
    '                                                Dim res13 As String = String.Empty
    '                                                da.SelectCommand.CommandText = "InsertDefaultMovement"
    '                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                                da.SelectCommand.Parameters.Clear()
    '                                                da.SelectCommand.Transaction = tran
    '                                                da.SelectCommand.Parameters.AddWithValue("tid", res)
    '                                                da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
    '                                                da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
    '                                                da.SelectCommand.ExecuteNonQuery()

    '                                                res13 = ob1.GetNextUserFromRolematrixT(res, EID, "30200", "", "30200", con, tran)
    '                                                Dim sretMsgArr() As String = res13.Split(":")
    '                                                ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)
    '                                                If sretMsgArr(0) = "ARCHIVE" Then
    '                                                    'Dim Op As New Exportdata()
    '                                                    'Op.PushdataT(res, sretMsgArr(1), EID, con, tran)
    '                                                End If

    '                                                '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
    '                                                If sretMsgArr(0).ToUpper() = "NO SKIP" Then
    '                                                    Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
    '                                                    'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
    '                                                    'this code block is added by ajeet kumar for transaction to be rolled back
    '                                                    ''''tran.Rollback()
    '                                                    ''''Exit Sub
    '                                                Else


    '                                                End If
    '                                                'Execute Trigger
    '                                                Try
    '                                                    Dim FormName As String = Convert.ToString(names(f))
    '                                                    '     Dim EID As Integer = 0
    '                                                    If (res > 0) And (FormName <> "") Then
    '                                                        Trigger.ExecuteTriggerT(FormName, EID, res, con, tran)
    '                                                    End If
    '                                                Catch ex As Exception
    '                                                    Throw
    '                                                End Try
    '                                                'Try
    '                                                '    UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
    '                                                '    da.SelectCommand.CommandText = UpdStr.ToString()
    '                                                '    da.SelectCommand.Transaction = tran
    '                                                '    da.SelectCommand.ExecuteNonQuery()
    '                                                '    Dim srerd As String = String.Empty
    '                                                'Catch ex As Exception

    '                                                'End Try
    '                                                'tran.Commit()
    '                                                ''Non transactional Query
    '                                                ''Check Work Flow
    '                                                'ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
    '                                                'da.SelectCommand.CommandType = CommandType.Text
    '                                                'da.SelectCommand.Parameters.Clear()
    '                                                'da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
    '                                                'Dim dt1 As New DataTable
    '                                                'da.Fill(dt1)
    '                                                'If dt1.Rows.Count = 1 Then
    '                                                '    If dt1.Rows(0).Item(0).ToString = "1" Then
    '                                                '        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
    '                                                '    End If
    '                                                'End If
    '                                            End If
    '                                        End If
    '                                    End If

    '                                End If




    '                            Next
    '                        End If

    '                        Try
    '                            UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
    '                            da.SelectCommand.CommandText = UpdStr.ToString()
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.ExecuteNonQuery()
    '                            Dim srerd As String = String.Empty
    '                        Catch ex As Exception

    '                        End Try
    '                        tran.Commit()
    '                        'Dim ob1 As New DMSUtil()
    '                        ' Dim res1 As String = String.Empty
    '                        ' Dim ob As New DynamicForm
    '                        'Non transactional Query
    '                        'Check Work Flow
    '                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
    '                        Dim dt1 As New DataTable
    '                        da.Fill(dt1)
    '                        If dt1.Rows.Count = 1 Then
    '                            If dt1.Rows(0).Item(0).ToString = "1" Then
    '                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
    '                            End If
    '                        End If

    '                    Next

    '                End If

    '            Next

    '        End If
    '    Catch ex As Exception
    '        If Not tran Is Nothing Then
    '            tran.Rollback()
    '        End If
    '        Throw
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not con Is Nothing Then
    '            con.Close()
    '        End If
    '    End Try

    'End Sub

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
    Protected Sub btn_Sales_Click(sender As Object, e As EventArgs) Handles btn_Sales.Click
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
                                                            lblres.Text = "Document Saved  "

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
        AutoInvoice()
    End Sub

    Protected Sub AutoInvoice()

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


            da = New SqlDataAdapter("select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld,RentEsc,CAMEsc,SDmonths,RentEscptage	,CamEscptage from   MMM_MST_AutoInvoiceSetting with(nolock) where  IsActiveStatus=1", con)
            '  da.SelectCommand.Transaction = tran

            Dim dt As New DataTable
            da.Fill(ds, "AutoInvSettingData")
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
                    da.SelectCommand.CommandText = "with cte as(select  ROW_NUMBER() OVER(PARTITION BY fld2,fld11 ORDER BY tid DESC) rn, fld2,  " & LeaseType & "  as leasetype, " & fieldmappingLeaseDocdt & " as [LeaseDocNo], " & fieldmappingINVdt & " as LRentInvGenDate,  tid as tid,convert(varchar, convert(datetime," & RegistrationDateFld & ", 3), 103)  as RegistrationDate ," & RentFreePeriodFlds & " as RentFreePeriod," & FrequencyFld & " as [RentPaymentCycle],fld49 as [RentFreeFitOutStartDate],fld50 as [RentFreeFitOutEndDate]," & RentFreePeriodFlds & " as [RentFreeDays]," & StartDateFld & " as LStartDate , " & EndDateFld & " as LEndDate," & MGAmount & " as LMGAmount," & RentEsc & " As RentEsc," & CAMEsc & " as CAMEsc," & SDmonths & " as SDMonths," & RentEscPtage & " as RentEscPtage," & CAMEscPtage & " as CAMEscPtage,fld57 as [LessorsPropertyShare],fld41 as [RentType],fld76 as [CamPaymentcycle]," & SDFld & " as SDAmt,fld48 as CAMCommDate," & CAMFld & " as CAMAmt,fld78 as AmendmentFlag,fld75 as LTokenAmnt from MMM_MST_MASTER  with(nolock)   where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and  convert(date, getdate(), 3) between   convert(date," & StartDateFld & ", 3) and  convert(date, " & EndDateFld & ", 3)    and reftid<>'' and isauth=1)	select * from cte where rn=1"
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
                    Dim LTokenAmnt As Double = 0


                    If ds.Tables("AutoInvDocData").Rows.Count <> 0 Then

                        For j As Integer = 0 To ds.Tables("AutoInvDocData").Rows.Count - 1

                            Try

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
                                LTokenAmnt = ds.Tables("AutoInvDocData").Rows(j).Item("LTokenAmnt")

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

                                Dim names As New ArrayList
                                Dim TestLeaseType = ds.Tables("AutoInvDocData").Rows(j).Item("leasetype")

                                If TestLeaseType = "1491591" Then 'Rental
                                    names.Add("Rental Invoice")
                                ElseIf TestLeaseType = "1491592" Then 'Security Deposit 
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
                                                        '  If LeaseComDate = Date.Now Then
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
                                                                lblres.Text = "Document Saved "
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

                                                        '  End If

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
                                                                        lblres.Text = "Document Saved"
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
                                                                        lblres.Text = "Document Saved"
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
                                                                        lblres.Text = "Document Saved "
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
                                                                    Else
                                                                        tran.Commit()
                                                                    End If

                                                                End If
                                                            End If

                                                        ElseIf InvGenDt = InvGenDt Then
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
                                                                        lblres.Text = "Document Saved ID "
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
                                                        If LeaseComDate = LeaseComDate Then

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
                                                                LSDAmount = LSDAmt - LTokenAmnt

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
                                                                lblres.Text = "Document Saved ID "
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



                                                        If InvGenDt = InvGenDt Then
                                                            insStr = ""
                                                            If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                            ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                                insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'" & InvGenDt & "',getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
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
                                                                lblres.Text = "Document Saved ID"
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
                                                            Else
                                                                tran.Commit()
                                                            End If



                                                        End If

                                                    End If

                                                End If
                                            Catch ex As Exception

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
                                                        If LCAMComDate = LCAMComDate Then 'date.now

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
                                                                lblres.Text = "Document Saved ID "
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
                                                                            If CDate(Finaldate).Year = Escdt.Year Then
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
                                                                    lblres.Text = "Document Saved ID"
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
                                                                            If CDate(Finaldate).Year = Escdt.Year Then
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
                                                                    lblres.Text = "Document Saved ID"
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
                                                                            If CDate(Finaldate).Year = Escdt.Year Then
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
                                                                    lblres.Text = "Document Saved "
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


                                                        ElseIf InvGenDt = InvGenDt Then ' Date.Now Then

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
                                                                            If CDate(Finaldate).Year = Escdt.Year Then
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
                                                                    lblres.Text = "Document Saved"
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
                                                If Not tran Is Nothing Then
                                                    tran.Rollback()
                                                End If
                                            End Try
                                        End If
                                    End If
                                Next


                            Catch ex As Exception
                                If Not tran Is Nothing Then
                                    tran.Rollback()
                                End If
                            End Try
                        Next
                    End If

                Next
            End If

        Catch ex As Exception
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
                    ElseIf days = DaysStayed Then
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
End Class
