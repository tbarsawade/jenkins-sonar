Imports Microsoft.VisualBasic
Imports System.Data.DataSet
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Web.Hosting

Public Class IHCLWebService
    Public Shared UserName As String = String.Empty
    Public Shared Password As String = String.Empty
    Public Shared URL As String = String.Empty
    Public Shared MethodName As String = String.Empty
    Public Shared MethodType As String = String.Empty

    Public Shared TajUrl As String = String.Empty
    Public Shared WsseUrl As String = String.Empty
    Public Shared TypeUrl As String = String.Empty

    Public Shared Reqenv_xmlns_Tag As String = String.Empty ' new by sp

    Public Shared Responsibility As String = String.Empty
    Public Shared RespApplication As String = String.Empty
    Public Shared SecurityGroup As String = String.Empty
    Public Shared NLSLanguage As String = String.Empty

    Public Shared SoapBody As String = String.Empty
    Public Shared CompleteSoapRequest As String = String.Empty

    Public Shared dsIHCLConfigData As New System.Data.DataSet()
    Public Shared PearlIds As String = String.Empty
    Public Shared FinalResponse As String = String.Empty
    Public Shared ReportQueryResult As New System.Data.DataTable()

    Public Shared Ondemand As Boolean = False

    'Dim constr As String = "server=MYNDHOSTDBVIP1;initial catalog=DMS;uid=DMS;pwd=mY#4dmP$juCh"

    Public Sub New()
        dsIHCLConfigData = Nothing
        ReportQueryResult = Nothing
        SoapBody = String.Empty
        MethodName = String.Empty
        MethodType = String.Empty
        CompleteSoapRequest = String.Empty
    End Sub

    Public Function EntryPoint1(ByVal methodName As String) As String
        FinalResponse = String.Empty
        IHCLWebService.MethodName = methodName
        Ondemand = True
        dsIHCLConfigData = GetIHCLData1()
        OutwardSoapWebServiceProcess(dsIHCLConfigData)
        Return FinalResponse
    End Function


    Public Sub EntryPoint()

        IHCLWebService.MethodName = MethodName
        dsIHCLConfigData = GetIHCLData()
        ' SetConfigProperties(dsIHCLConfigData)
        OutwardSoapWebServiceProcess(dsIHCLConfigData)

    End Sub

    Private Function GetIHCLData1() As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "Select r.tid,r.eid,r.IsSendAttachment,r.RSFolder,r.reportquery,r.msgbody,r.emailto,r.cc,r.bcc,r.sendtype,r.SendTo,r.FileSeparator,r.PostFixType,r.FileExtension,r.reportname,r.reportSubject,r.LastScheduledDate,r.USERNAME,r.PASSWORD,r.OUT_INTGR_FLD1 URL,r.OUT_INTGR_FLD2 TAJ,r.OUT_INTGR_FLD3 MethodName,r.OUT_INTGR_FLD4 WSSE,r.OUT_INTGR_FLD5 TYPE,r.OUT_INTGR_FLD6 Responsibility,r.OUT_INTGR_FLD7 RespApplication,r.OUT_INTGR_FLD8 SecurityGroup,r.OUT_INTGR_FLD9 NLSLanguage,r.OUT_INTGR_FLD10 MethodType,r.OUT_INTGR_FLD11 soapReqenv_xmlns_Tag from  MMM_MST_ReportScheduler r where eid=205 And FtpFlag=4 and r.OUT_INTGR_FLD3='" & MethodName & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        Return ds
    End Function

    Private Function GetIHCLData() As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "Select r.tid,r.eid,r.IsSendAttachment,r.RSFolder,r.reportquery,r.msgbody,r.emailto,r.cc,r.bcc,r.sendtype,r.SendTo,r.FileSeparator,r.PostFixType,r.FileExtension,r.reportname,r.reportSubject,r.LastScheduledDate,r.USERNAME,r.PASSWORD,r.OUT_INTGR_FLD1 URL,r.OUT_INTGR_FLD2 TAJ,r.OUT_INTGR_FLD3 MethodName,r.OUT_INTGR_FLD4 WSSE,r.OUT_INTGR_FLD5 TYPE,r.OUT_INTGR_FLD6 Responsibility,r.OUT_INTGR_FLD7 RespApplication,r.OUT_INTGR_FLD8 SecurityGroup,r.OUT_INTGR_FLD9 NLSLanguage,r.OUT_INTGR_FLD10 MethodType,r.OUT_INTGR_FLD11 soapReqenv_xmlns_Tag from  MMM_MST_ReportScheduler r where eid=205 And FtpFlag=4"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        Return ds
    End Function

    Private Sub SetConfigProperties(ds As System.Data.DataSet)
        '' moved from here to loop
        'UserName = ds.Tables("rpt").Rows(0).Item("USERNAME").ToString()
        'Password = ds.Tables("rpt").Rows(0).Item("PASSWORD").ToString()
        'URL = ds.Tables("rpt").Rows(0).Item("URL").ToString()

        'TajUrl = ds.Tables("rpt").Rows(0).Item("TAJ").ToString()
        'WsseUrl = ds.Tables("rpt").Rows(0).Item("WSSE").ToString()
        'TypeUrl = ds.Tables("rpt").Rows(0).Item("TYPE").ToString()

        'Responsibility = ds.Tables("rpt").Rows(0).Item("Responsibility").ToString()
        'RespApplication = ds.Tables("rpt").Rows(0).Item("RespApplication").ToString()
        'SecurityGroup = ds.Tables("rpt").Rows(0).Item("SecurityGroup").ToString()
        'NLSLanguage = ds.Tables("rpt").Rows(0).Item("NLSLanguage").ToString()
    End Sub

    Sub OutwardSoapWebServiceProcess(ByVal ds As System.Data.DataSet)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New System.Data.SqlClient.SqlConnection(conStr)
        Dim oda As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter("", con)
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim RptSub As String = ""
        Dim FTPType As String = ""
        Dim msg As String = ""
        Dim MAILTO As String = ""
        Dim CC As String = ""
        Dim Bcc As String = ""
        Dim Scfname As String = ""
        Dim IsSendAttachment As String = ""
        Dim dmsService As New DMSService()
        Try
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                If (dmsService.ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True) Or (Ondemand = True) Then

                    If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "USER" Then
                        eid = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                        msg = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                        MAILTO = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                        CC = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                        Bcc = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                        Dim scheduleDate As String = Format(Convert.ToDateTime(Now.ToString), "yyyy-MM-dd HH:mm:ss:fff")

                        IHCLWebService.MethodName = ds.Tables("rpt").Rows(d).Item("MethodName").ToString()
                        IHCLWebService.MethodType = ds.Tables("rpt").Rows(d).Item("MethodType").ToString()

                        ' new brough here from global by sp
                        IHCLWebService.UserName = ds.Tables("rpt").Rows(d).Item("USERNAME").ToString()
                        IHCLWebService.Password = ds.Tables("rpt").Rows(d).Item("PASSWORD").ToString()
                        IHCLWebService.URL = ds.Tables("rpt").Rows(d).Item("URL").ToString()

                        IHCLWebService.TajUrl = ds.Tables("rpt").Rows(d).Item("TAJ").ToString()
                        IHCLWebService.WsseUrl = ds.Tables("rpt").Rows(d).Item("WSSE").ToString()
                        IHCLWebService.TypeUrl = ds.Tables("rpt").Rows(d).Item("TYPE").ToString()

                        IHCLWebService.Reqenv_xmlns_Tag = ds.Tables("rpt").Rows(d).Item("soapReqenv_xmlns_Tag").ToString() 'new

                        IHCLWebService.Responsibility = ds.Tables("rpt").Rows(d).Item("Responsibility").ToString()
                        IHCLWebService.RespApplication = ds.Tables("rpt").Rows(d).Item("RespApplication").ToString()
                        IHCLWebService.SecurityGroup = ds.Tables("rpt").Rows(d).Item("SecurityGroup").ToString()
                        IHCLWebService.NLSLanguage = ds.Tables("rpt").Rows(d).Item("NLSLanguage").ToString()

                        Dim query As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                        Rptname = ds.Tables("rpt").Rows(d).Item("reportname").ToString()
                        RptSub = ds.Tables("rpt").Rows(d).Item("reportSubject").ToString()
                        'Dim lastschedule As String = ds.Tables("rpt").Rows(d).Item("LastScheduledDate").ToString
                        'lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                        'str = Replace(str, "@lastsch", lastschedule)

                        Dim methodName As String = IHCLWebService.MethodName
                        Dim methodType As String = IHCLWebService.MethodType
                        oda.SelectCommand.CommandText = query
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandTimeout = 180
                        Dim FTPR As New DataTable

                        oda.Fill(FTPR)
                        ReportQueryResult = FTPR
                        If FTPR.Rows.Count > 0 Then
                            Dim CNT As Integer = FTPR.Rows.Count
                            PostVendorCreationData(FTPR, methodName, methodType)
                            oda.SelectCommand.CommandText = "UPDATE_LASTSCHEDULEDDATE_IHCL"
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("@Sdate", scheduleDate)
                            oda.SelectCommand.Parameters.AddWithValue("@EID", ds.Tables("rpt").Rows(d).Item("eid").ToString())
                            oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            oda.SelectCommand.ExecuteNonQuery()
                        End If
                    End If
                End If
            Next

        Catch ex As Exception

            Dim textOfException As String = ex.ToString()
            If ex.GetType() Is GetType(WebException) Then
                Dim exp As WebException = CType(ex, WebException)
                Dim errResp As WebResponse = exp.Response
                If Not errResp Is Nothing Then
                    Using respStream As Stream = errResp.GetResponseStream()
                        Dim reader As StreamReader = New StreamReader(respStream)
                        textOfException = reader.ReadToEnd()
                    End Using
                End If
            End If
            textOfException = textOfException + " SoapBody>> " + SoapBody + " CompleteSoapRequest>> " + CompleteSoapRequest
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", textOfException)

            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | IHCL | " + IHCLWebService.MethodName)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()

            Dim obj As New MailUtill(EID:=eid)
            If MAILTO.ToString.Trim <> "" Then
                Dim filepath As String = String.Empty
                'obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " Integration Failed", MailBody:=msg, CC:=CC, Attachments:="", BCC:=Bcc)
                If ReportQueryResult.Rows.Count > 0 Then
                    filepath = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\") + CreateCSV(ReportQueryResult, "205")
                End If
                '' below line added new by sp for including actual error message.
                msg &= "<br> Error message is - <br><br>" & textOfException
                obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " | " & IHCLWebService.MethodName & " | Integration Failed", MailBody:=msg, CC:=CC, Attachments:=filepath, BCC:=Bcc)
            End If
        Finally
            con.Dispose()
        End Try

    End Sub





    Public Sub PostVendorCreationData(ByVal dt As DataTable, ByVal methodName As String, ByVal methodType As String)
        Dim builder As StringBuilder = New StringBuilder()
        Dim PearlId As String = String.Empty

        'Create multiple payloads of object in builder
        If dt.Rows.Count > 0 Then

            Select Case methodName
                Case "vendor_creation"
                    For Each datarow As DataRow In dt.Rows
                        builder.Clear()
                        PearlId = String.Empty
                        SoapBody = String.Empty
                        CompleteSoapRequest = String.Empty
                        If Not datarow("P_PEARL_ID").ToString().Equals(String.Empty) And datarow("P_PEARL_ID") IsNot Nothing Then
                            PearlId = datarow("P_PEARL_ID").ToString()
                        End If
                        builder.Append("<" & Reqenv_xmlns_Tag & ":InputParameters>")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            'remove this IF after discussion
                            If datarow(colName) IsNot Nothing And (Not datarow(colName).ToString().Equals(String.Empty)) Then
                                Dim value = datarow(colName).ToString() ' builder.Append("<ven:" + colName + ">" + value + "</ven:" + colName + ">")
                                value = value.Replace("<", "&lt;")
                                value = value.Replace(">", "&gt;")
                                builder.Append("<" & Reqenv_xmlns_Tag & ":" + colName + ">" & value & "</" & Reqenv_xmlns_Tag & ":" & colName & ">")
                            End If
                        Next
                        builder.Append("</" & Reqenv_xmlns_Tag & ":InputParameters>")
                        SoapBody = builder.ToString()
                        CallWebService(builder, methodName, methodType, PearlId)
                    Next
                Case "vendor_modfn"
                    For Each datarow As DataRow In dt.Rows
                        builder.Clear()
                        PearlId = String.Empty
                        SoapBody = String.Empty
                        CompleteSoapRequest = String.Empty
                        If (datarow("P_VENDOR_ID") IsNot Nothing) And (Not datarow("P_VENDOR_ID").ToString().Equals(String.Empty)) And (datarow("P_VENDOR_SITE_ID") IsNot Nothing) And (Not datarow("P_VENDOR_SITE_ID").ToString().Equals(String.Empty)) Then
                            If Not datarow("P_PEARL_ID").ToString().Equals(String.Empty) And datarow("P_PEARL_ID") IsNot Nothing Then
                                PearlId = datarow("P_PEARL_ID").ToString()
                            End If
                            builder.Append("<" & Reqenv_xmlns_Tag & ":InputParameters>")
                            For Each dataCol As DataColumn In dt.Columns
                                Dim colName As String = dataCol.ColumnName
                                If datarow(colName) IsNot Nothing And (Not datarow(colName).Equals(String.Empty)) Then
                                    Dim value = datarow(colName).ToString()  ' builder.Append("<ven:" + colName + ">" + value + "</ven:" + colName + ">")
                                    value = value.Replace("<", "&lt;")
                                    value = value.Replace(">", "&gt;")
                                    builder.Append("<" & Reqenv_xmlns_Tag & ":" + colName + ">" & value & "</" & Reqenv_xmlns_Tag & ":" & colName & ">")
                                End If
                            Next
                            builder.Append("</" & Reqenv_xmlns_Tag & ":InputParameters>")
                            SoapBody = builder.ToString()
                            CallWebService(builder, methodName, methodType, PearlId)
                        End If
                    Next
                Case "vendor_siteextension"
                    For Each datarow As DataRow In dt.Rows
                        builder.Clear()
                        PearlId = String.Empty
                        SoapBody = String.Empty
                        CompleteSoapRequest = String.Empty
                        If (datarow("P_VENDOR_ID") IsNot Nothing) And (Not datarow("P_VENDOR_ID").ToString().Equals(String.Empty)) And (datarow("P_VENDOR_SITE_ID") Is Nothing) Or (datarow("P_VENDOR_SITE_ID").ToString().Equals(String.Empty)) Then
                            If Not datarow("P_PEARL_ID").ToString().Equals(String.Empty) And datarow("P_PEARL_ID") IsNot Nothing Then
                                PearlId = datarow("P_PEARL_ID").ToString()
                            End If
                            builder.Append("<" & Reqenv_xmlns_Tag & ":InputParameters>")
                            For Each dataCol As DataColumn In dt.Columns
                                Dim colName As String = dataCol.ColumnName
                                If datarow(colName) IsNot Nothing And (Not datarow(colName).Equals(String.Empty)) Then
                                    Dim value = datarow(colName).ToString() ' builder.Append("<ven:" + colName + ">" + value + "</ven:" + colName + ">")
                                    value = value.Replace("<", "&lt;")
                                    value = value.Replace(">", "&gt;")
                                    builder.Append("<" & Reqenv_xmlns_Tag & ":" + colName + ">" & value & "</" & Reqenv_xmlns_Tag & ":" & colName & ">")
                                End If
                            Next
                            builder.Append("</" & Reqenv_xmlns_Tag & ":InputParameters>")
                            SoapBody = builder.ToString()
                            CallWebService(builder, methodName, methodType, PearlId)
                        End If
                    Next
                Case "material_rcpt_url"
                    For Each datarow As DataRow In dt.Rows
                        builder.Clear()
                        PearlId = String.Empty
                        SoapBody = String.Empty
                        CompleteSoapRequest = String.Empty
                        If Not datarow("P_INVOICE_ID").ToString().Equals(String.Empty) And datarow("P_INVOICE_ID") IsNot Nothing Then
                            PearlId = datarow("P_INVOICE_ID").ToString()
                        End If
                        builder.Append("<" & Reqenv_xmlns_Tag & ":InputParameters>")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            If datarow(colName) IsNot Nothing And (Not datarow(colName).Equals(String.Empty)) Then
                                Dim value = datarow(colName).ToString()
                                value = value.Replace("<", "&lt;")
                                value = value.Replace(">", "&gt;")
                                builder.Append("<" & Reqenv_xmlns_Tag & ":" + colName + ">" & value & "</" & Reqenv_xmlns_Tag & ":" & colName & ">")
                            End If
                        Next
                        builder.Append("</" & Reqenv_xmlns_Tag & ":InputParameters>")
                        SoapBody = builder.ToString()
                        CallWebService(builder, methodName, methodType, PearlId)
                    Next
                Case "inv_non_ers_crtn"
                    For Each datarow As DataRow In dt.Rows
                        builder.Clear()
                        PearlId = String.Empty
                        SoapBody = String.Empty
                        CompleteSoapRequest = String.Empty
                        If Not datarow("P_PEARL_ID").ToString().Equals(String.Empty) And datarow("P_PEARL_ID") IsNot Nothing Then
                            PearlId = datarow("P_PEARL_ID").ToString()
                        End If
                        builder.Append("<" & Reqenv_xmlns_Tag & ":InputParameters>")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            If datarow(colName) IsNot Nothing And (Not datarow(colName).Equals(String.Empty)) Then
                                Dim value = datarow(colName).ToString()
                                value = value.Replace("<", "&lt;")
                                value = value.Replace(">", "&gt;")
                                builder.Append("<" & Reqenv_xmlns_Tag & ":" + colName + ">" & value & "</" & Reqenv_xmlns_Tag & ":" & colName & ">")
                            End If
                        Next
                        builder.Append("</" & Reqenv_xmlns_Tag & ":InputParameters>")
                        SoapBody = builder.ToString()
                        CallWebService(builder, methodName, methodType, PearlId)
                    Next
                Case Else

            End Select
        End If
        IHCLWebService.MethodName = String.Empty
        IHCLWebService.MethodType = String.Empty
    End Sub

    '------------------------------------------------WSDL CALL LOGIC BEGINS ----------------------------------------------
    Private Sub CallWebService(ByVal stringBuilder As StringBuilder, ByVal methodName As String, ByVal methodType As String, ByVal pearlId As String)
        Dim schedulerMethodName As String = methodName

        If methodName.Equals("vendor_siteextension") Then
            methodName = "vendor_creation"
        End If

        Dim soapEnvelopeXml As XmlDocument = CreateSoapEnvelope(stringBuilder, methodName)
        CompleteSoapRequest = soapEnvelopeXml.InnerXml.ToString()
        Dim webRequest As HttpWebRequest = CreateWebRequest(URL, methodType)
        InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest)
        Dim soapResultStatus As String = String.Empty

        Dim soapResult As String = "Default Assigned Value | Something went wrong, please check logs."

        Dim Result = webRequest.GetResponse()
        Dim oReceiveStream As Stream = Result.GetResponseStream()
        Dim streamReader As StreamReader = New StreamReader(oReceiveStream)
        soapResult = streamReader.ReadToEnd()

        If soapResult.Contains("Successfully Inserted") Or soapResult.Contains("URL Updated") Or soapResult.Contains("Success") Then
            soapResultStatus = "Success | " + methodName + " | " + pearlId
        Else
            soapResultStatus = "Failed | " + methodName + " | " + pearlId
        End If

        Dim today = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        Call Save_Log("205", "", "IHCL Outward Integration", "", soapResultStatus, soapResult, today.ToString(), today.ToString(), 1, soapResultStatus, soapEnvelopeXml.InnerXml)
    End Sub


    Private Shared Function CreateWebRequest(ByVal url As String, ByVal reqType As String) As HttpWebRequest
        Dim webRequest As HttpWebRequest = CType(System.Net.WebRequest.Create(url), HttpWebRequest)
        webRequest.Method = reqType
        webRequest.Headers.Add("Username", UserName)
        webRequest.Headers.Add("Password", Password)
        webRequest.ContentType = "text/xml;charset=""utf-8"""
        webRequest.Accept = "text/xml"
        Return webRequest
    End Function

    Private Shared Function CreateSoapEnvelope(ByVal stringBuilder As StringBuilder, ByVal methodName As String) As XmlDocument
        Dim soapEnvelopeDocument As XmlDocument = New XmlDocument()

        If stringBuilder.ToString().Contains("&") Or stringBuilder.ToString().Contains("\""") Or stringBuilder.ToString().Contains("'") Then
            stringBuilder = stringBuilder.Replace("&", "&amp;")
            stringBuilder = stringBuilder.Replace("'", "&apos;")
            stringBuilder = stringBuilder.Replace("\""", "&quot;")
        End If
        '' to be implemented below 
        'Or stringBuilder.ToString().Contains("<") Or stringBuilder.ToString().Contains(">")
        ' stringBuilder = stringBuilder.Replace("<", "&lt;")
        ' stringBuilder = stringBuilder.Replace(">", "&gt;")

        'Dim Xmlstr As String = "<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:taj=""" & TajUrl & """ xmlns:ven=""" & TajUrl + methodName + "/" & """><soapenv:Header><wsse:Security xmlns:wsse=""" & WsseUrl & """><wsse:UsernameToken><wsse:Username>" & UserName & "</wsse:Username><wsse:Password Type=""" & TypeUrl & """>" & Password & "</wsse:Password></wsse:UsernameToken></wsse:Security><taj:SOAHeader><!--Optional:--><taj:Responsibility>" & Responsibility & "</taj:Responsibility><!--Optional:--><taj:RespApplication>" & RespApplication & "</taj:RespApplication><!--Optional:--><taj:SecurityGroup>" & SecurityGroup & "</taj:SecurityGroup><!--Optional:--><taj:NLSLanguage>" & NLSLanguage & "</taj:NLSLanguage><!--Optional:--><taj:Org_Id>84</taj:Org_Id></taj:SOAHeader></soapenv:Header><soapenv:Body>" & stringBuilder.ToString() & "</soapenv:Body></soapenv:Envelope>"

        Dim Xmlstr As String = "<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:taj=""" & TajUrl &
            """ xmlns:" & Reqenv_xmlns_Tag & "=""" & TajUrl + methodName + "/" & """><soapenv:Header><wsse:Security xmlns:wsse=""" & WsseUrl &
            """><wsse:UsernameToken><wsse:Username>" & UserName & "</wsse:Username><wsse:Password Type=""" & TypeUrl & """>" & Password &
            "</wsse:Password></wsse:UsernameToken></wsse:Security><taj:SOAHeader><!--Optional:--><taj:Responsibility>" & Responsibility &
            "</taj:Responsibility><!--Optional:--><taj:RespApplication>" & RespApplication &
            "</taj:RespApplication><!--Optional:--><taj:SecurityGroup>" & SecurityGroup &
            "</taj:SecurityGroup><!--Optional:--><taj:NLSLanguage>" & NLSLanguage &
            "</taj:NLSLanguage><!--Optional:--><taj:Org_Id>84</taj:Org_Id></taj:SOAHeader></soapenv:Header><soapenv:Body>" &
            stringBuilder.ToString() & "</soapenv:Body></soapenv:Envelope>"

        soapEnvelopeDocument.LoadXml(Xmlstr)
        Return soapEnvelopeDocument
    End Function
    Sub InsertSoapEnvelopeIntoWebRequest(ByVal soapEnvelopeXml As XmlDocument, ByVal webRequest As WebRequest)
        Using stream As Stream = webRequest.GetRequestStream()
            soapEnvelopeXml.Save(stream)
        End Using
    End Sub


    Protected Function Save_Log(ByVal CID As String, ByVal Filename As String, ByVal ActionCode As String, ByVal Empcode As String, ByVal Result As String, ByVal Remarks As String, ByVal fileRundate As String, ByVal SuccessfulRun As String, ByVal TotalAttempts As Integer, ByVal TransType As String, ByVal xmlFileNm As String, Optional ByVal Rerunstr As String = "", Optional ByVal ErrMsg As String = "", Optional ByVal LastInsertDate As String = "") As String
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

        'da.SelectCommand.ExecuteNonQueryAsync()
        da.SelectCommand.ExecuteNonQuery()
        con.Dispose()
        da.Dispose()
        con = Nothing
        da = Nothing
        Save_Log = ""
    End Function


    Private Function CreateCSV(ByVal dt As DataTable, ByVal apid As String) As String
        'Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & ".CSV"
        Try
            ' UpdateErrorLog("Varroc_leave", "Exit from CreateCSV", "WS To FTP", "1", "WTF")
            Dim fname As String = apid & "_" & IHCLWebService.MethodName & ".CSV"

            Dim path As String = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\")
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
                        If dr(i).ToString().Contains(",") Then
                            'Dim val = dr(i).ToString().Replace(",", " comma")
                            Dim val = dr(i).ToString().Replace(dr(i).ToString(), """" + dr(i).ToString() + """")

                            sw.Write(val.ToString)
                        Else
                            sw.Write(dr(i).ToString)
                        End If
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
