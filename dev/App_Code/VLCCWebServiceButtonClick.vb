Imports Microsoft.VisualBasic
Imports System.Data.DataSet
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Web.Script.Serialization

Public Class VLCCWebServiceButtonClick


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
    Public Shared dsVLCCConfigData As New System.Data.DataSet()
    Public Shared ReportQueryResult As New System.Data.DataTable()


    'Dim constr As String = "server=MYNDHOSTDBVIP1;initial catalog=DMS;uid=DMS;pwd=mY#4dmP$juCh"


    Public Sub New()
        dsVLCCConfigData = Nothing
        ReportQueryResult = Nothing
        FinalResponse = String.Empty
        SoapBody = String.Empty
        MethodName = String.Empty
        MethodType = String.Empty
        CompleteSoapRequest = String.Empty

    End Sub

    Public Function EntryPoint(ByVal methodName As String) As String

        FinalResponse = String.Empty
        dsVLCCConfigData = GetVLCCData()
        OutwardSoapWebServiceProcess(dsVLCCConfigData, methodName)
        Return FinalResponse

    End Function

    Private Function GetVLCCData() As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "Select r.tid,r.eid,r.IsSendAttachment,r.RSFolder,r.rerunquery,r.msgbody,r.emailto,r.cc,r.bcc,r.sendtype,r.SendTo,r.FileSeparator,r.PostFixType,r.FileExtension,r.reportname,r.reportSubject,r.LastScheduledDate,r.USERNAME,r.PASSWORD,r.OUT_INTGR_FLD1 URL,r.OUT_INTGR_FLD2 SOAPAction,r.OUT_INTGR_FLD3 MethodName,r.OUT_INTGR_FLD4 Methodurl,r.OUT_INTGR_FLD5 innerMethodurl,r.OUT_INTGR_FLD6 MYNDMethodName,r.OUT_INTGR_FLD10 MethodType from  MMM_MST_ReportScheduler r where eid=209 And FtpFlag=6"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        Return ds
    End Function


    Sub OutwardSoapWebServiceProcess(ByVal ds As System.Data.DataSet, ByVal methodName As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New System.Data.SqlClient.SqlConnection(conStr)
        Dim oda As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter("", con)
        Dim eid As String = ""
        Dim RptSub As String = ""
        Dim MAILTO As String = ""
        Dim msg As String = ""
        Dim CC As String = ""
        Dim Bcc As String = ""

        Try
            Dim table As DataTable = ds.Tables("rpt")
            Dim rows() As DataRow = table.Select("MethodName = '" + methodName + "'")
            If rows(0)("SendTo").ToString.ToUpper = "USER" Then
                eid = rows(0)("Eid").ToString()
                msg = rows(0)("msgbody").ToString()
                MAILTO = rows(0)("emailto").ToString()
                CC = rows(0)("cc").ToString()
                Bcc = rows(0)("bcc").ToString()
                ReportQueryResult = Nothing
                VLCCWebServiceButtonClick.MethodName = methodName
                VLCCWebServiceButtonClick.MethodType = rows(0)("MethodType").ToString()
                VLCCWebServiceButtonClick.URL = rows(0).Item("URL").ToString()
                VLCCWebServiceButtonClick.Rptname = rows(0).Item("reportname").ToString()
                Dim query As String = rows(0)("rerunquery").ToString()
                Dim pValues As String = PrepareValuesInQuotes(VLCCWebServiceButtonClick.PearlIds)
                query = query.Replace("@PearlID", pValues)
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
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | VLCC | " + VLCCWebServiceButtonClick.MethodName)
            oda.SelectCommand.Parameters.AddWithValue("@EID", "209")
            oda.SelectCommand.Parameters.AddWithValue("@RESPONSEMSG", CompleteSoapRequest)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            Dim obj As New MailUtill(EID:=eid)
            If MAILTO.ToString.Trim <> "" Then
                Dim filepath As String = String.Empty
                If ReportQueryResult.Rows.Count > 0 Then
                    filepath = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\") + CreateCSV(ReportQueryResult, "209")
                End If
                obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " | " & VLCCWebServiceButtonClick.MethodName & " | Integration Failed", MailBody:=msg, CC:=CC, Attachments:=filepath, BCC:=Bcc)
            End If
        Finally
            con.Dispose()
        End Try
    End Sub

    Public Shared Function PrepareValuesInQuotes(ByVal s As String) As String
        Dim res As String = ""
        Dim list As List(Of String) = s.Split(",").ToList()
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
            Select Case VLCCWebServiceButtonClick.MethodName
                Case "Vendor_Registration"
                    For Each datarow As DataRow In dt.Rows
                        builder.Append("{")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            Dim value = datarow(colName).ToString()
                            Dim colNo As Integer = dataCol.Ordinal
                            If (datarow("PearlID") IsNot Nothing) And (Not datarow("PearlID").ToString().Equals(String.Empty)) Then
                                If Not datarow("PearlID").ToString().Equals(String.Empty) And datarow("PearlID") IsNot Nothing Then
                                    Code = datarow("PearlID").ToString()
                                End If
                            End If
                            If (colNo = "35") Then
                                builder.Append("""" + colName + """:""" + value + """")
                            Else
                                builder.Append("""" + colName + """:""" + value + """,")
                            End If
                        Next
                        builder.Append("}")
                        SoapBody = builder.ToString()
                        CallWebService(builder, MethodName, MethodType)
                    Next

                Case "Vendor_Modification"
                    For Each datarow As DataRow In dt.Rows
                        builder.Append("{")
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            Dim value = datarow(colName).ToString()
                            Dim colNo As Integer = dataCol.Ordinal
                            If (datarow("RequestID") IsNot Nothing) And (Not datarow("RequestID").ToString().Equals(String.Empty)) Then
                                If Not datarow("RequestID").ToString().Equals(String.Empty) And datarow("RequestID") IsNot Nothing Then
                                    Code = datarow("RequestID").ToString()
                                End If
                            End If
                            If (colNo = "28") Then
                                builder.Append("""" + colName + """:""" + value + """")
                            Else
                                builder.Append("""" + colName + """:""" + value + """,")
                            End If
                        Next
                        builder.Append("}")
                        SoapBody = builder.ToString()
                        CallWebService(builder, MethodName, MethodType)
                    Next
                Case "GRN_Invoice"
                    'For Each datarow As DataRow In dt.Rows
                    builder.Clear()
                    SoapBody = String.Empty
                    CompleteSoapRequest = String.Empty
                    Dim irowsCount As Integer = dt.Rows.Count
                    builder.Append("{")
                    For c As Integer = 0 To irowsCount - 1
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            Dim colNo As Integer = dataCol.Ordinal
                            Dim value = dt.Rows(c).ItemArray(colNo).ToString
                            If (c = 0) Then
                                If colName.Contains("Line_Item_Doc-") Then
                                    Dim LineItem As String = colName.Replace("Line_Item_Doc-", "")

                                    If (colNo = 15) Then

                                        builder.Append("""GRNItem""")
                                        builder.Append(":[{")
                                    End If
                                    If (colNo = 26) Then
                                        builder.Append("""" + LineItem + """:""" + value + """")
                                        builder.Append("}")

                                    Else
                                        builder.Append("""" + LineItem + """:""" + value + """,")
                                    End If
                                Else
                                    builder.Append("""" + colName + """:""" + value + """,")
                                    Code = dt.Rows(c).Item("GRNNO").ToString()
                                End If
                            Else
                                If colName.Contains("Line_Item_Doc-") Then

                                    Dim childitem As String = colName.Replace("Line_Item_Doc-", "")

                                    If (colNo = 15) Then

                                        builder.Append(",{")
                                        builder.Append("""" + childitem + """:""" + value + """,")

                                    ElseIf (colNo = 26) Then
                                        builder.Append("""" + childitem + """:""" + value + """")
                                        builder.Append("}")
                                    Else
                                        builder.Append("""" + childitem + """:""" + value + """,")
                                    End If
                                End If
                            End If
                        Next
                    Next
                    builder.Append("]}")
                    SoapBody = builder.ToString()
                    CallWebService(builder, MethodName, MethodType)
                    'Next

                Case "purchase_Invoice"
                    'For Each datarow As DataRow In dt.Rows
                    builder.Clear()
                    SoapBody = String.Empty
                    CompleteSoapRequest = String.Empty
                    Dim irowsCount As Integer = dt.Rows.Count
                    builder.Append("{")
                    For c As Integer = 0 To irowsCount - 1
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            Dim colNo As Integer = dataCol.Ordinal
                            Dim value = dt.Rows(c).ItemArray(colNo).ToString
                            If (c = 0) Then
                                If colName.Contains("Line_Item_Doc-") Then
                                    Dim LineItem As String = colName.Replace("Line_Item_Doc-", "")
                                    If (colNo = 24) Then
                                        builder.Append("""POInvoiceItem""")
                                        builder.Append(":[{")
                                    End If
                                    If (colNo = 35) Then
                                        builder.Append("""" + LineItem + """:""" + value + """")
                                        builder.Append("}")

                                    Else
                                        builder.Append("""" + LineItem + """:""" + value + """,")
                                    End If
                                Else
                                    builder.Append("""" + colName + """:""" + value + """,")
                                    Code = dt.Rows(c).Item("PONumber").ToString()
                                End If
                            Else
                                If colName.Contains("Line_Item_Doc-") Then

                                    Dim childitem As String = colName.Replace("Line_Item_Doc-", "")
                                    If (colNo = 24) Then
                                        builder.Append(",{")
                                        builder.Append("""" + childitem + """:""" + value + """,")

                                    ElseIf (colNo = 35) Then
                                        builder.Append("""" + childitem + """:""" + value + """")
                                        builder.Append("}")
                                    Else
                                        builder.Append("""" + childitem + """:""" + value + """,")
                                    End If
                                End If
                            End If
                        Next
                    Next
                    builder.Append("]}")
                    SoapBody = builder.ToString()
                    CallWebService(builder, MethodName, MethodType)
                   ' Next

                Case "Invoice_Non_PO"
                    'For Each datarow As DataRow In dt.Rows
                    builder.Clear()
                    SoapBody = String.Empty
                    CompleteSoapRequest = String.Empty
                    Dim irowsCount As Integer = dt.Rows.Count
                    builder.Append("{")
                    For c As Integer = 0 To irowsCount - 1
                        For Each dataCol As DataColumn In dt.Columns
                            Dim colName As String = dataCol.ColumnName
                            Dim colNo As Integer = dataCol.Ordinal
                            Dim value = dt.Rows(c).ItemArray(colNo).ToString
                            If (c = 0) Then
                                If colName.Contains("Line_Item_Doc-") Then
                                    Dim LineItem As String = colName.Replace("Line_Item_Doc-", "")
                                    If (colNo = 25) Then
                                        builder.Append("""NonPOInvoiceItem""")
                                        builder.Append(":[{")
                                    End If
                                    If (colNo = 30) Then
                                        builder.Append("""" + LineItem + """:""" + value + """")
                                        builder.Append("}")

                                    Else
                                        builder.Append("""" + LineItem + """:""" + value + """,")
                                    End If
                                Else
                                    builder.Append("""" + colName + """:""" + value + """,")
                                    Code = dt.Rows(c).Item("VendInvoiceNo").ToString()
                                End If
                            Else
                                If colName.Contains("Line_Item_Doc-") Then

                                    Dim childitem As String = colName.Replace("Line_Item_Doc-", "")
                                    If (colNo = 25) Then
                                        builder.Append(",{")
                                        builder.Append("""" + childitem + """:""" + value + """,")

                                    ElseIf (colNo = 30) Then
                                        builder.Append("""" + childitem + """:""" + value + """")
                                        builder.Append("}")
                                    Else
                                        builder.Append("""" + childitem + """:""" + value + """,")
                                    End If
                                End If
                            End If
                        Next
                    Next
                    builder.Append("]}")
                    SoapBody = builder.ToString()
                    CallWebService(builder, MethodName, MethodType)
                    'Next
                Case Else
            End Select
        End If
        VLCCWebServiceButtonClick.MethodName = String.Empty
        VLCCWebServiceButtonClick.MethodType = String.Empty
    End Function

    Private Sub CallWebService(ByVal stringBuilder As StringBuilder, ByVal methodName As String, ByVal methodType As String)
        Dim soapResultStatus As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New System.Data.SqlClient.SqlConnection(conStr)
        Dim oda As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter("", con)
        Dim responseData As VLCCoutwardkey = New VLCCoutwardkey
        Try
            Dim httpWebRequest = CType(WebRequest.Create(URL), HttpWebRequest)
            httpWebRequest.ContentType = "application/json"
            httpWebRequest.Method = methodType
            Using streamWriter = New StreamWriter(httpWebRequest.GetRequestStream())
                'Dim json As String = "{""RequestID"":""3796574"",""VendorName"":""OM WELLNESS"",""Email"":""anil.kumar@myndsol.com"",""Address1"":""Ad1"",""Address2"":""Ad2"",""Address3"":""Ad3"",""City"":""DDUN"",""VendState"":""Uttarakhand"",""Country"":""India"",""ContPersonaName"":""Anil Kumar"",""ContPersonaEmail"":""neeraj@myndsol.com"",""ContPersonaMobile"":"""",""PANNumber"":""PAN01"",""OrgType"":""Company"",""GSTStatus"":""Unregistered"",""GSTNum"":"""",""MSMEReg"":""1969928"",""MSMERegNumber"":"""",""AcHolderName"":""abc"",""BankAccount"":""098976777777"",""BankIFSC"":""SBIN0002224"",""BankName"":""SBI"",""BankAddress"":""Ad1"",""EInvoiceAppicable"":""1969928"",""TCSApplicable"":""1969928"",""PaymentTerms"":"""",""NewVendBussExperiece"":"""",""PearlID"":""VR1001"",""VendorType"":""Raw Material"",""RequestType"":""New Vendor"",""ExistingVendName"":"""",""ExistingVendCode"":"""",""ResonOfAddition"":"""",""CostImpact"":"""",""DeliveryLocation"":""NEW DELHI""}"
                If stringBuilder.ToString().Contains("&") Or stringBuilder.ToString().Contains("\""") Or stringBuilder.ToString().Contains("'") Then
                    stringBuilder = stringBuilder.Replace("&", "&amp;")
                    stringBuilder = stringBuilder.Replace("'", "&apos;")
                    stringBuilder = stringBuilder.Replace("\""", "&quot;")
                End If
                Dim json As String = stringBuilder.ToString()
                streamWriter.Write(json)
                streamWriter.Flush()
                streamWriter.Close()
            End Using
            Dim httpResponse = CType(httpWebRequest.GetResponse(), HttpWebResponse)
            Using streamReader = New StreamReader(httpResponse.GetResponseStream())
                Dim result As String = streamReader.ReadToEnd()
                responseData = (New JavaScriptSerializer()).Deserialize(Of VLCCoutwardkey)(result)
                If (responseData.Responsecodes = "200") Then
                    soapResultStatus = "Success | " + responseData.Responsecodes + "  | " + responseData.Responsemessage + " | " + Code

                ElseIf (responseData.Responsecodes = "201") Then
                    soapResultStatus = "Duplicate | " + responseData.Responsecodes + "  | " + responseData.Responsemessage + " | " + Code
                Else
                    soapResultStatus = "Failed | " + responseData.Responsecodes + "  | " + responseData.Responsemessage + " | " + Code
                End If
            End Using
            FinalResponse += soapResultStatus + " | Web Service Complete Response:"
            Dim today = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            Call Save_Log("209", "", "VLCC Outward Integration", "", soapResultStatus, "", today.ToString(), today.ToString(), 1, soapResultStatus, stringBuilder.ToString())
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
            'textOfException = textOfException + " SoapBody>> " + SoapBody + " CompleteSoapRequest>> " + soapResultStatus
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", textOfException)
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | VLCC | " + VLCCWebServiceButtonClick.MethodName)
            oda.SelectCommand.Parameters.AddWithValue("@EID", "209")
            oda.SelectCommand.Parameters.AddWithValue("@RESPONSEMSG", stringBuilder.ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Dispose()
        End Try
    End Sub


    Protected Function Save_Log(ByVal CID As String, ByVal Filename As String, ByVal ActionCode As String, ByVal Empcode As String, ByVal Result As String, ByVal Remarks As String, ByVal fileRundate As String, ByVal SuccessfulRun As String, ByVal TotalAttempts As Integer, ByVal TransType As String, ByVal xmlFileNm As String, Optional ByVal Rerunstr As String = "", Optional ByVal ErrMsg As String = "", Optional ByRef LastInsertDate As String = "") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
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
            Dim fname As String = apid & "_" & VLCCWebServiceButtonClick.MethodName + DateTime.Now.ToString("ddMMyyyyHHMMss") & ".CSV"
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

    Public Class VLCCoutwardkey

        Public Property Responsecodes As String
        Public Property Responsemessage As String

    End Class

End Class
