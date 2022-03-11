Imports Microsoft.VisualBasic
Imports System.Data.DataSet
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Web.Hosting

Public Class Lenskartwebservicebuttononclick

    Public Shared UserName As String = String.Empty
    Public Shared Password As String = String.Empty
    Public Shared URL As String = String.Empty
    Public Shared METHOD As String = String.Empty
    Public Shared CompleteSoapRequest As String = String.Empty
    Public Shared MethodType As String = String.Empty
    Public Shared MethodName As String = String.Empty
    Public Shared SoapBody As String = String.Empty
    Public Shared FinalResponse As String = String.Empty
    Public Shared SOAPAction As String = String.Empty
    Public Shared Rptname As String = String.Empty
    Public Shared PearlIds As String = String.Empty
    Public Shared Code As String = String.Empty
    Public Shared Companyurl As String = String.Empty
    Public Shared dsLenskartConfigData As New System.Data.DataSet()
    Public Shared ReportQueryResult As New System.Data.DataTable()
    Public Shared PRNO As String = String.Empty
    Public Shared eid As String = String.Empty
    Public Shared msg As String = String.Empty
    Public Shared RptSub As String = String.Empty
    Public Shared MAILTO As String = String.Empty
    Public Shared CC As String = String.Empty
    Public Shared Bcc As String = String.Empty


    Dim constr As String = "server=MYNDHOSTDBVIP1;initial catalog=DMS;uid=DMS;pwd=mY#4dmP$juCh"

    Public Sub New()
        dsLenskartConfigData = Nothing
        ReportQueryResult = Nothing
        FinalResponse = String.Empty
        SoapBody = String.Empty
        MethodName = String.Empty
        MethodType = String.Empty
        CompleteSoapRequest = String.Empty
        dsLenskartConfigData = GetLenskartData()

    End Sub

    Private Function GetLenskartData() As DataSet

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "Select r.tid,r.eid,r.IsSendAttachment,r.RSFolder,r.ReRunQuery,r.msgbody,r.emailto,r.cc,r.bcc,r.sendtype,r.SendTo,r.FileSeparator,r.PostFixType,r.FileExtension,r.reportname,r.reportSubject,r.LastScheduledDate,r.USERNAME,r.PASSWORD,r.OUT_INTGR_FLD1 URL,r.OUT_INTGR_FLD2 SOAPAction,r.OUT_INTGR_FLD3 MethodName,r.OUT_INTGR_FLD4 Methodurl,r.OUT_INTGR_FLD5 innerMethodurl,r.OUT_INTGR_FLD6 MYNDMethodName,r.OUT_INTGR_FLD10 MethodType from  MMM_MST_ReportScheduler r where eid=185 And FtpFlag=5"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        Return ds
    End Function


    Public Function EntryPoint(ByVal methodName As String) As String

        FinalResponse = String.Empty
        OutwardSoapWebServiceProcess(dsLenskartConfigData, methodName)
        Return FinalResponse

    End Function

    Sub OutwardSoapWebServiceProcess(ByVal ds As System.Data.DataSet, ByVal methodName As String)
        Dim con As New System.Data.SqlClient.SqlConnection(constr)
        Dim oda As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter("", con)
        'Dim eid As String = ""
        'Dim RptSub As String = ""
        'Dim MAILTO As String = ""
        'Dim msg As String = ""
        'Dim CC As String = ""
        'Dim Bcc As String = ""

        Try
            Dim table As DataTable = ds.Tables("rpt")

            Dim rows() As DataRow = table.Select("MethodName = '" + methodName + "'")
            If rows(0)("sendto").ToString.ToUpper = "USER" Then
                Lenskartwebservicebuttononclick.eid = rows(0)("Eid").ToString()
                Lenskartwebservicebuttononclick.msg = rows(0)("msgbody").ToString()
                Lenskartwebservicebuttononclick.MAILTO = rows(0)("emailto").ToString()
                Lenskartwebservicebuttononclick.CC = rows(0)("cc").ToString()
                Lenskartwebservicebuttononclick.Bcc = rows(0)("bcc").ToString()
                Lenskartwebservicebuttononclick.RptSub = rows(0)("reportSubject").ToString()
                ReportQueryResult = Nothing
                Lenskartwebservicebuttononclick.MethodName = methodName
                Lenskartwebservicebuttononclick.MethodType = rows(0)("MethodType").ToString()
                Lenskartwebservicebuttononclick.UserName = rows(0).Item("USERNAME").ToString()
                Lenskartwebservicebuttononclick.Password = rows(0).Item("PASSWORD").ToString()
                Lenskartwebservicebuttononclick.URL = rows(0).Item("URL").ToString()
                Lenskartwebservicebuttononclick.SOAPAction = rows(0).Item("SOAPAction").ToString()
                Lenskartwebservicebuttononclick.Rptname = rows(0).Item("reportname").ToString()
                Dim query As String = rows(0)("ReRunQuery").ToString()
                'Dim pValues As String = LenskartWebService.docid
                'Dim pValues As String = PrepareValuesInQuotes(IHCLWebServiceRunOnButtonClick.docid)
                'query = query.Replace("@tid", docid)
                Dim pValues As String = PrepareValuesInQuotes(Lenskartwebservicebuttononclick.PearlIds)
                query = query.Replace("@tid", pValues)
                oda.SelectCommand.CommandText = query
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandTimeout = 180
                Dim FTPR As New DataTable
                oda.Fill(FTPR)
                ReportQueryResult = FTPR
                If FTPR.Rows.Count > 0 Then
                    Dim CNT As Integer = FTPR.Rows.Count
                    PostVendorCreationData(FTPR)
                End If
            End If
        Catch ex As Exception

            FinalResponse += "Something went wrong, please check logs."
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
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | LENSKAERT | " + Lenskartwebservicebuttononclick.MethodName)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            oda.SelectCommand.Parameters.AddWithValue("@RESPONSEMSG", CompleteSoapRequest)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            Dim obj As New MailUtill(EID:=eid)
            If MAILTO.ToString.Trim <> "" Then
                Dim filepath As String = String.Empty
                If ReportQueryResult.Rows.Count > 0 Then

                    filepath = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\") + CreateCSV(ReportQueryResult, "185")
                End If
                msg &= "<br> Error message is - <br><br>" & textOfException
                msg &= "<br> Error message is - <br><br>" & CompleteSoapRequest
                obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " | " & Lenskartwebservicebuttononclick.MethodName & " | Integration Failed", MailBody:=msg, CC:=CC, Attachments:=filepath, BCC:=Bcc)
            End If
        Finally
            con.Dispose()
        End Try
    End Sub

    Public Shared Function PrepareValuesInQuotes(ByVal s As String) As String
        Dim res As String = ""
        Dim list As List(Of String) = s.Split(",").ToList() 'ToList(Of String)()

        For Each item In list
            res += "'" & item & "',"
        Next
        res = res.TrimEnd(",")
        Return res
    End Function

    Public Function PostVendorCreationData(ByVal dt As DataTable)

        Dim builder As StringBuilder = New StringBuilder()
        FinalResponse = String.Empty
        If dt.Rows.Count > 0 Then
            SoapBody = String.Empty
            CompleteSoapRequest = String.Empty
            Select Case Lenskartwebservicebuttononclick.MethodName
                Case "Vendor_Master"
                    For Each datarow As DataRow In dt.Rows
                        builder.Append("<CreateVendoMyndXML xmlns=""urn:microsoft-dynamics-schemas/codeunit/LenskartMynd""><createVendoMyndXML><InboundVendors xmlns=""urn:microsoft-dynamics-nav/xmlports/x50220"">")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            Lenskartwebservicebuttononclick.Companyurl = datarow("Companyurl").ToString()
                            'If datarow(colName) IsNot Nothing And (Not datarow(colName).ToString().Equals(String.Empty)) Then
                            If Not datarow(colName).ToString().Equals(datarow("Companyurl").ToString()) Then
                                Dim value = datarow(colName).ToString()
                                If (datarow("VendorNo.") IsNot Nothing) And (Not datarow("VendorNo.").ToString().Equals(String.Empty)) Then
                                    If Not datarow("VendorNo.").ToString().Equals(String.Empty) And datarow("VendorNo.") IsNot Nothing Then
                                        Code = datarow("VendorNo.").ToString()
                                    End If
                                End If
                                builder.Append("<" + colName + ">" + value + "</" + colName + ">")
                            End If
                        Next
                        builder.Append("</InboundVendors></createVendoMyndXML></CreateVendoMyndXML>")
                        SoapBody = builder.ToString()
                        CallWebService(builder, MethodName, MethodType)
                    Next
                Case "vendor_Invoice"
                    For Each datarow As DataRow In dt.Rows
                        builder.Append("<PurchInvMyndXML xmlns=""urn:microsoft-dynamics-schemas/codeunit/LenskartMynd""><pIMyndXML><PiMynd xmlns=""urn:microsoft-dynamics-nav/xmlports/x50220"">")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            If datarow(colName) IsNot Nothing And (Not datarow(colName).ToString().Equals(String.Empty)) Then
                                Dim value = datarow(colName).ToString()
                                builder.Append("<" + colName + ">" + value + "</" + colName + ">")
                            End If
                        Next
                        builder.Append("</PiMynd></pIMyndXML></PurchInvMyndXML>")
                        'SoapBody = builder.ToString()
                        'CallWebService(builder, MethodName, MethodType)
                    Next
                Case "Purchase_Requistion"
                    For Each datarow As DataRow In dt.Rows
                        builder.Clear()
                        SoapBody = String.Empty
                        CompleteSoapRequest = String.Empty
                        Dim irowsCount As Integer = dt.Rows.Count
                        builder.Append("<PurchQuoteHeaderLineMyndXML xmlns=""urn:microsoft-dynamics-schemas/codeunit/LenskartMynd""><purchQuoteHeaderLineMyndXML><InboundPurchaseHeader xmlns=""urn:microsoft-dynamics-nav/xmlports/x50221"">")
                        For c As Integer = 0 To irowsCount - 1
                            For Each dataCol As DataColumn In dt.Columns
                                Dim colName As String = dataCol.ColumnName
                                Lenskartwebservicebuttononclick.Companyurl = datarow("Companyurl").ToString()
                                Dim colNo As Integer = dataCol.Ordinal
                                Dim value = dt.Rows(c).ItemArray(colNo).ToString
                                If Not datarow(colName).ToString().Equals(datarow("Companyurl").ToString()) Then
                                    If (c = 0) Then
                                        If colName.Contains("Line_Item_Doc-") Then
                                            Dim LineItem As String = colName.Replace("Line_Item_Doc-", "")
                                            builder.Append("<" + LineItem + ">" + value + "</" + LineItem + ">")
                                            If (colNo = 44) Then
                                                builder.Append("</InboundPurchaseLine>")
                                            End If
                                        Else
                                            builder.Append("<" + colName + ">" + value + "</" + colName + ">")
                                            If (colNo = 23) Then
                                                builder.Append("</InboundPurchaseHeader><InboundPurchaseLine xmlns = ""urn:microsoft-dynamics-nav/xmlports/x50221"">")
                                            End If
                                            PRNO = datarow("No.").ToString()
                                        End If
                                    Else
                                        If colName.Contains("Line_Item_Doc-") Then
                                            If (colNo = 24) Then
                                                builder.Append("<InboundPurchaseLine xmlns = ""urn:microsoft-dynamics-nav/xmlports/x50221"" >")
                                            End If
                                            Dim childitem As String = colName.Replace("Line_Item_Doc-", "")
                                            builder.Append("<" + childitem + ">" + value + "</" + childitem + ">")
                                            If (colNo = 44) Then
                                                builder.Append("</InboundPurchaseLine>")
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        Next

                        builder.Append("</purchQuoteHeaderLineMyndXML></PurchQuoteHeaderLineMyndXML>")
                        SoapBody = builder.ToString()
                        CallWebService(builder, MethodName, MethodType)
                    Next

                Case Else
            End Select
            'SoapBody = builder.ToString()
            'CallWebService(builder, MethodName, MethodType)
        End If
        Lenskartwebservicebuttononclick.MethodName = String.Empty
        Lenskartwebservicebuttononclick.MethodType = String.Empty
    End Function

    Private Sub CallWebService(ByVal stringBuilder As StringBuilder, ByVal methodName As String, ByVal methodType As String)

        Dim con As New System.Data.SqlClient.SqlConnection(constr)
        Dim oda As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter("", con)
        Dim soapEnvelopeXml As XmlDocument = CreateSoapEnvelope(stringBuilder, methodName)
        CompleteSoapRequest = soapEnvelopeXml.InnerXml.ToString()
        URL = Lenskartwebservicebuttononclick.Companyurl
        Dim webRequest As HttpWebRequest = CreateWebRequest(URL, methodType)
        InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest)
        Dim soapResultStatus As String = String.Empty
        Dim soapResult As String = "Default Assigned Value."
        Try
            Dim Result = webRequest.GetResponse()
            Dim oReceiveStream As Stream = Result.GetResponseStream()
            Dim streamReader As StreamReader = New StreamReader(oReceiveStream)
            soapResult = streamReader.ReadToEnd()
            If soapResult.Contains("true") Then
                If methodName = "Vendor_Master" Then
                    soapResultStatus = "Success | " + methodName + " | " + Code

                Else
                    soapResultStatus = "Success | " + methodName + " | " + PRNO
                End If
                If methodName = "Vendor_Master" Then
                    'preeti' 
                    'Dim responseData As Responsekey1 = New Responsekey1
                    'Dim ApIKEY = ConfigurationManager.AppSettings("Lenskartkey")
                    'Dim doctype = "VRgggg Vendor Code Creation App"
                    ''Dim inputString As String = "Key$$ORGSNDIFGAO030644BHQ~DOCTYPE$$VR Vendor Code Creation App~ACTIONTYPE$$APPROVAL~Data$$Vendor Code::" & Code & "|Creation Status::true"
                    'Dim inputString As String = "Key$$" & ApIKEY & "~DOCTYPE$$" & doctype & "~ACTIONTYPE$$APPROVAL~Data$$Vendor Code::" & Code & "|Creation Status::true"
                    'Dim myndService As MyndBPMWS = New MyndBPMWS()
                    'Dim byteArray As Byte() = Encoding.ASCII.GetBytes(inputString.ToString())
                    'Dim ms As MemoryStream = New MemoryStream(byteArray)
                    'responseData.Message = myndService.DocumentApproval(ms)
                    'preeti' 

                End If
            Else
                If methodName = "Vendor_Master" Then
                    soapResultStatus = "Failed | " + methodName + " | " + Code
                Else
                    soapResultStatus = "Failed | " + methodName + " | " + PRNO
                End If
            End If

                FinalResponse += soapResultStatus + " | Web Service Complete Response:" + soapResult
            Dim today = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            Call Save_Log("185", "", "LensKart Outward Integration", "", soapResultStatus, soapResult, today.ToString(), today.ToString(), 1, soapResultStatus, soapEnvelopeXml.InnerXml)
        Catch ex As Exception
            FinalResponse += "Something went wrong, please check logs."
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
            'textOfException = textOfException + " SoapBody>> " + SoapBody + " CompleteSoapRequest>> " + CompleteSoapRequest
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", textOfException)
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | LENSKAERT | " + Lenskartwebservicebuttononclick.MethodName)
            oda.SelectCommand.Parameters.AddWithValue("@EID", "185")
            oda.SelectCommand.Parameters.AddWithValue("@RESPONSEMSG", soapEnvelopeXml.InnerXml.ToString())

            '    If con.State <> ConnectionState.Open Then
            '        con.Open()
            '    End If
            '    oda.SelectCommand.ExecuteNonQuery()
            'Finally
            '    con.Dispose()
            'End Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            Dim obj As New MailUtill(EID:=eid)
            If MAILTO.ToString.Trim <> "" Then
                Dim filepath As String = String.Empty
                If ReportQueryResult.Rows.Count > 0 Then
                    filepath = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\") + CreateCSV(ReportQueryResult, "185")
                End If
                msg &= "<br> Error message is - <br><br>" & soapEnvelopeXml.InnerXml.ToString()
                msg &= "<br> Error message is - <br><br>" & textOfException
                obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " | " & Lenskartwebservicebuttononclick.MethodName & " | Integration Failed", MailBody:=msg, CC:=CC, Attachments:=filepath, BCC:=Bcc)
            End If
        Finally
            con.Dispose()
        End Try

    End Sub

    Private Shared Function CreateWebRequest(ByVal URL As String, ByVal reqType As String) As HttpWebRequest

        Dim webRequest As HttpWebRequest = CType(System.Net.WebRequest.Create(URL), HttpWebRequest)
        Dim credential = New NetworkCredential(UserName, Password)
        Dim credentialCache = New CredentialCache()
        credentialCache.Add(New Uri(URL), "NTLM", credential)
        webRequest.Credentials = credentialCache
        webRequest.Method = reqType
        webRequest.KeepAlive = True
        webRequest.Headers.Add("SOAPAction", SOAPAction)
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
        Dim Xmlstr As String = "<Soap:Envelope xmlns:Soap=""http://schemas.xmlsoap.org/soap/envelope/""><Soap:Body>" & stringBuilder.ToString() & "</Soap:Body></Soap:Envelope>"
        'Dim Xmlstr As String = "<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Header><AUTHHEADER xmlns=""http://tempuri.org/""><USERNAME>harpreets</USERNAME><PASSWORD>Presha0203</PASSWORD></AUTHHEADER></soap:Header><soap:Body><PurchInvMyndXML xmlns=""urn:microsoft-dynamics-schemas/codeunit/LenskartMynd""><pIMyndXML ><PiMynd xmlns=""urn:microsoft-dynamics-nav/xmlports/x50223""><DocType>Invoice</DocType><DocNo>179905600</DocNo><DocPostingDate>2021-07-14</DocPostingDate><SourceDocNo>PO\21-22\000661</SourceDocNo><SourceDocLine>123</SourceDocLine><Type>Item</Type><ItemNo>TEST007</ItemNo><InvoicedQty>20</InvoicedQty><InvoiceAmount>100</InvoiceAmount><UnitPriceEsc.Taxes>5</UnitPriceEsc.Taxes><VendorIncoiceNo>12345</VendorIncoiceNo><ItemDescription>Test</ItemDescription><VendorNo.>Test</VendorNo.></PiMynd></pIMyndXML></PurchInvMyndXML></soap:Body></soap:Envelope>"
        soapEnvelopeDocument.LoadXml(Xmlstr)
        Return soapEnvelopeDocument
    End Function



    Sub InsertSoapEnvelopeIntoWebRequest(ByVal soapEnvelopeXml As XmlDocument, ByVal webRequest As WebRequest)

        Using stream As Stream = webRequest.GetRequestStream()
            Try
                soapEnvelopeXml.Save(stream)
            Catch ex As Exception
            End Try
        End Using
    End Sub


    Protected Function Save_Log(ByVal CID As String, ByVal Filename As String, ByVal ActionCode As String, ByVal Empcode As String, ByVal Result As String, ByVal Remarks As String, ByVal fileRundate As String, ByVal SuccessfulRun As String, ByVal TotalAttempts As Integer, ByVal TransType As String, ByVal xmlFileNm As String, Optional ByVal Rerunstr As String = "", Optional ByVal ErrMsg As String = "", Optional ByRef LastInsertDate As String = "") As String
        Dim con As New SqlConnection(constr)
        Dim da As New SqlDataAdapter("", con)
        LastInsertDate = fileRundate
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "insert into Staging_Outwardintegration_Result (EntryDate,cid,ReportName, actionCode, empcode, result, remarks, fileRundate, SuccessfulRun, TotAttempts, transType, XmlFileName, ReRunRemarks, errMsg) values('" & fileRundate & "','" & CID & "','" & Filename & "', '" & ActionCode & "', '" & Empcode & "', '" & Result & "', '" & Remarks & "','" & fileRundate & "','" & SuccessfulRun & "','" & TotalAttempts & "','" & TransType & "','" & xmlFileNm & "','" & Rerunstr & "','" & ErrMsg & "')"
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


    Private Function CreateCSV(ByVal dt As DataTable, ByVal apid As String) As String
        'Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & ".CSV"
        Try
            ' UpdateErrorLog("Varroc_leave", "Exit from CreateCSV", "WS To FTP", "1", "WTF")
            Dim fname As String = apid & "_" & Lenskartwebservicebuttononclick.MethodName + DateTime.Now.ToString("ddMMyyyyHHMMss") & ".CSV"
            Dim path As String = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\")
            Dim targetFI As New FileInfo(path + "\" + fname)
            If targetFI.Exists = False Then
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

        Catch ex As Exception
            Throw New Exception(ex.Message)

        End Try
    End Function
End Class

Public Class Responsekey1
    Public Property Message As String
    Public Property Token As String
End Class








