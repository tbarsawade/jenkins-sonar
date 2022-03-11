
Imports System.Data
Imports System.IO
Imports System.Net
Imports System.Web.Script.Serialization
Imports System.Security.Authentication

Partial Class VendorPortal
    Inherits System.Web.UI.Page

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim test As String = "ajeet""kumer"
        Dim docid As Int32 = 1691830
        Dim documenttype As String = "Vendor Invoice"
        Dim eid As Int32 = 124
        Dim objDT As New DataTable()
        Dim objRowDT As New DataTable()
        Dim objDC As New DataClass()
        Dim sb As New StringBuilder()
        sb.Append("param={")
        objDT = objDC.ExecuteQryDT("select * from mmm_mst_fields where documenttype='" & documenttype & "' and eid=" & eid & " and (M1Outward is not null and M1Outward<>'')")
        Dim dynQry As New StringBuilder()
        dynQry.Append("Select ")
        Dim arlist As New ArrayList()
        For Each dr As DataRow In objDT.Rows
            Select Case dr("DropDownType").ToString().ToUpper
                Case "MASTER VALUED"
                    arlist.Add("dms.udf_split('" & dr("dropdown") & "'," & dr("fieldmapping") & ") as [" & dr("M1Outward") & "]")
                Case Else
                    arlist.Add("isnull(" & dr("fieldmapping") & ",'') as [" & dr("M1Outward") & "]")
            End Select
        Next
        If arlist.Count > 0 Then
            Dim qry As String = dynQry.ToString() & (String.Join(", ", arlist.ToArray())).ToString() & " from mmm_mst_doc where tid=" & docid
            objDT = objDC.ExecuteQryDT(qry)
            GetString(objDT, sb)
            WSPost(sb.ToString())
            'Dim Str As String = "param={""Payment_Date_of_Buyer"": ""03/10/2017"",""Underlying_Commodity_Description"": ""Some Services"",""Buyer_Name"": ""mynd solution"",""PV_Sending_date"":""03/10/2017"",""Invoices"": [{""Invoice_Doc_No"": ""57124"",""PV_Unique_Ref_No"": ""D8403833"",""Invoice_NO"": ""INV34343"",""Invoice_Amount"": ""50000"",""GRN_SRN_Number"":""8699"",""GRN_SRN_Date"": ""03/10/2017"",""Invoice_Date"": ""03/10/2017""},{""Invoice_Doc_No"": ""7497"",""PV_Unique_Ref_No"": ""PV_INV_69"",""Invoice_NO"": ""INV90670"",""Invoice_Amount"": ""79999"",""GRN_SRN_Number"":""8462"",""GRN_SRN_Date"": ""03/10/2017"",""Invoice_Date"": ""03/10/2017""}],""Supplier_Name"": ""flipkart"",""Payment_Due_Date_for_supplier"": ""03/10/2017"",""Supplier_Code"": ""FK1009"",""PV_Factoring_Unit_No"": ""9823423"",""Underlying_Commodity_Type"": ""Services""}"
            Dim Str As String = "param={""Payment_Date_of_Buyer"":""03/10/2017"",""Underlying_Commodity_Description"":""AMC Others"",""Buyer_Name"":""ABC Infosystems Ltd."",""PV_Sending_date"":"""",""GRN_SRN_Date"":"""",""Invoices"":[{""Invoice_Doc_No"":"""",""PV_Unique_Ref_No"":""D8403833"",""Invoice_NO"":""123456789"",""Invoice_Amount"":""0"",""GRN_SRN_Number"":"""",""Invoice_Date"":""24/07/17"" }],""Supplier_Name"":""flipkart"",""Payment_Due_Date_for_supplier"":"""",""Supplier_Code"":""FK1009"",""PV_Factoring_Unit_No"":"""",""Underlying_Commodity_Type"":""MATERIAL""}"
            WSPost(Str.ToString())
        End If
    End Sub

    Public Function GetString(objDt As DataTable, ByRef str As StringBuilder) As String
        Dim inputs() As String = {"Payment_Date_of_Buyer", "Underlying_Commodity_Description", "Buyer_Name", "PV_Sending_date", "Invoices", "Invoice_Doc_No", "PV_Unique_Ref_No", "Invoice_NO", "Invoice_Amount", "GRN_SRN_Number", "GRN_SRN_Date", "Invoice_Date", "Supplier_Name", "Payment_Due_Date_for_supplier", "Supplier_Code", "PV_Factoring_Unit_No", "Underlying_Commodity_Type"}
        Dim lstOfString As List(Of String) = New List(Of String)(inputs)
        Dim arbuilder As New ArrayList()
        Dim chbuilder As New ArrayList()
        Dim invoice As String = ""
        Dim IsInvoiceInvoke As Boolean = False
        For Each strdata As String In lstOfString
            If (strdata.ToString() = "Invoices" Or strdata.ToString() = "Invoice_Doc_No" Or strdata.ToString() = "PV_Unique_Ref_No" Or strdata.ToString() = "Invoice_NO" Or strdata.ToString() = "Invoice_Amount" Or strdata.ToString() = "GRN_SRN_Number" Or strdata.ToString() = "Invoice_Date") Then
                If strdata.ToString() = "Invoices" Then
                    invoice = """Invoices""" & ":" & "[{"
                    IsInvoiceInvoke = False
                ElseIf strdata.ToString() = "Invoice_Date" Then
                    If (Not objDt.Columns.Contains(strdata)) Then
                        chbuilder.Add("""" & strdata.ToString() & "" & ":" & """""")
                    Else
                        chbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & Convert.ToString(objDt.Rows(0)(strdata)) & """")
                    End If
                    IsInvoiceInvoke = True
                    invoice &= String.Join(",", chbuilder.ToArray()) & " }]"
                Else
                    If (Not objDt.Columns.Contains(strdata)) Then
                        chbuilder.Add("""" & strdata.ToString() & ("""" & ":" & """"""))
                    Else
                        chbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & Convert.ToString(objDt.Rows(0)(strdata)) & """")
                    End If
                    IsInvoiceInvoke = False
                End If
            Else
                If IsInvoiceInvoke = True Then
                    arbuilder.Add(invoice.ToString())
                    IsInvoiceInvoke = False
                End If
                If (Not objDt.Columns.Contains(strdata)) Then
                    arbuilder.Add("""" & strdata.ToString() & """" & ":" & """""")
                Else
                    arbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & Convert.ToString(objDt.Rows(0)(strdata)) & """")
                End If
            End If
        Next
        str.Append(String.Join(",", arbuilder.ToArray()))
        str.Append("}")
        Return str.ToString()
    End Function
    Public Function WSPost(Str As String) As String

        Dim ret As String = ""

        Try
            'url = "http://www.myndsaas.com/MyndBPMWS.svc/SaveData";


            Dim url As String = "http://103.25.172.132:8092/PVService/createReq"

            ' declare ascii encoding
            Dim encoding As New ASCIIEncoding()

            Dim strResult As String = String.Empty

            ' sample xml sent to Service & this data is sent in POST
            Dim postData As String = Str

            ' convert xmlstring to byte using ascii encoding
            Dim data As Byte() = encoding.GetBytes(postData)
            ' declare httpwebrequet wrt url defined above

            ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

            'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
            'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
            'ServicePointManager.SecurityProtocol = Tls12

            Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            ' set method as post
            webrequest__1.Method = "POST"
            ' set content type
            webrequest__1.ContentType = "application/x-www-form-urlencoded"
            ' set content length
            webrequest__1.ContentLength = data.Length
            ' get stream data out of webrequest object
            Dim newStream As Stream = webrequest__1.GetRequestStream()
            newStream.Write(data, 0, data.Length)
            newStream.Close()
            ' declare & read response from service
            Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)

            ' set utf8 encoding
            Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
            ' read response stream from response object
            Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)
            ' read string from stream data
            strResult = loResponseStream.ReadToEnd()
            ' close the stream object
            loResponseStream.Close()
            ' close the response object
            webresponse.Close()
            ' below steps remove unwanted data from response string
            ret = strResult
        Catch ex As Exception
            Return ex.ToString()
        End Try
        Return ret
    End Function

End Class
