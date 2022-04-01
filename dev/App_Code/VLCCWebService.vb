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

Public Class VLCCWebService

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
    Public Shared eid As String = String.Empty
    Public Shared msg As String = String.Empty
    Public Shared RptSub As String = String.Empty
    Public Shared MAILTO As String = String.Empty
    Public Shared CC As String = String.Empty
    Public Shared Bcc As String = String.Empty

    Public Sub New()
        dsVLCCConfigData = Nothing
        ReportQueryResult = Nothing
        FinalResponse = String.Empty
        SoapBody = String.Empty
        MethodName = String.Empty
        MethodType = String.Empty
        CompleteSoapRequest = String.Empty
    End Sub

    Public Sub EntryPoint()

        dsVLCCConfigData = GetVLCCData()
        OutwardSoapWebServiceProcess(dsVLCCConfigData)


    End Sub

    Private Function GetVLCCData() As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "Select r.tid,r.eid,r.IsSendAttachment,r.RSFolder,r.reportquery,r.msgbody,r.emailto,r.cc,r.bcc,r.sendtype,r.SendTo,r.FileSeparator,r.PostFixType,r.FileExtension,r.reportname,r.reportSubject,r.LastScheduledDate,r.USERNAME,r.PASSWORD,r.OUT_INTGR_FLD1 URL,r.OUT_INTGR_FLD2 SOAPAction,r.OUT_INTGR_FLD3 MethodName,r.OUT_INTGR_FLD4 Methodurl,r.OUT_INTGR_FLD5 innerMethodurl,r.OUT_INTGR_FLD6 MYNDMethodName,r.OUT_INTGR_FLD10 MethodType from  MMM_MST_ReportScheduler r where eid=209 And FtpFlag=6"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "rpt")
        Return ds
    End Function


    Sub OutwardSoapWebServiceProcess(ByVal ds As System.Data.DataSet)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New System.Data.SqlClient.SqlConnection(conStr)
        Dim oda As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter("", con)
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim RptSub As String = ""
        Dim MAILTO As String = ""
        Dim msg As String = ""
        Dim CC As String = ""
        Dim Bcc As String = ""
        Dim dmsService As New DMSService()
        Try
            'Dim table As DataTable = ds.Tables("rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                If dmsService.ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                    If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "USER" Then
                        VLCCWebService.eid = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                        VLCCWebService.msg = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                        VLCCWebService.MAILTO = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                        VLCCWebService.CC = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                        VLCCWebService.Bcc = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                        Dim scheduleDate As String = Format(Convert.ToDateTime(Now.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                        VLCCWebService.MethodName = ds.Tables("rpt").Rows(d).Item("MethodName").ToString()
                        VLCCWebService.MethodType = ds.Tables("rpt").Rows(d).Item("MethodType").ToString()
                        VLCCWebService.UserName = ds.Tables("rpt").Rows(d).Item("USERNAME").ToString()
                        VLCCWebService.Password = ds.Tables("rpt").Rows(d).Item("PASSWORD").ToString()
                        VLCCWebService.URL = ds.Tables("rpt").Rows(d).Item("URL").ToString()
                        VLCCWebService.SOAPAction = ds.Tables("rpt").Rows(d).Item("SOAPAction").ToString()
                        Dim query As String = ds.Tables("rpt").Rows(d)("reportquery").ToString()
                        Rptname = ds.Tables("rpt").Rows(d).Item("reportname").ToString()
                        RptSub = ds.Tables("rpt").Rows(d).Item("reportSubject").ToString()
                        Dim methodName As String = VLCCWebService.MethodName
                        Dim methodType As String = VLCCWebService.MethodType
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
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | VLCC | " + VLCCWebService.MethodName)
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
                    filepath = System.Web.Hosting.HostingEnvironment.MapPath("~\EMailAttachment\") + CreateCSV(ReportQueryResult, "209")
                End If
                msg &= "<br> Error message is - <br><br>" & textOfException
                msg &= "<br> Error message is - <br><br>" & CompleteSoapRequest
                obj.SendMail(ToMail:=MAILTO, Subject:=RptSub & " | " & VLCCWebService.MethodName & " | Integration Failed", MailBody:=msg, CC:=CC, Attachments:=filepath, BCC:=Bcc)
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

    Public Sub PostVendorCreationData(ByVal dt As DataTable, ByVal methodName As String, ByVal methodType As String)

        Dim builder As StringBuilder = New StringBuilder()
        FinalResponse = String.Empty
        If dt.Rows.Count > 0 Then
            SoapBody = String.Empty
            CompleteSoapRequest = String.Empty
            Select Case VLCCWebService.MethodName
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
                        CallWebService(builder, methodName, methodType)
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
                        CallWebService(builder, methodName, methodType)
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
                    CallWebService(builder, methodName, methodType)
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
                    CallWebService(builder, methodName, methodType)
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
                    CallWebService(builder, methodName, methodType)
                    'Next
                Case Else
            End Select
        End If
        VLCCWebService.MethodName = String.Empty
        VLCCWebService.MethodType = String.Empty
    End Sub

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
                If (responseData.Responsecodess = "200") Then
                    soapResultStatus = "Success | " + responseData.Responsecodess + "  | " + responseData.Responsemessagee + " | " + Code

                ElseIf (responseData.Responsecodess = "201") Then
                    soapResultStatus = "Duplicate | " + responseData.Responsecodess + "  | " + responseData.Responsemessagee + " | " + Code
                Else
                    soapResultStatus = "Failed | " + responseData.Responsecodess + "  | " + responseData.Responsemessagee + " | " + Code
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
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", Rptname + " | VLCC | " + VLCCWebService.MethodName)
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
            Dim fname As String = apid & "_" & VLCCWebService.MethodName + DateTime.Now.ToString("ddMMyyyyHHMMss") & ".CSV"
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

        Public Property Responsecodess As String
        Public Property Responsemessagee As String

    End Class
End Class
