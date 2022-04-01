
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports System.Security.Authentication

Partial Class M1InvoiceBidding
    Inherits System.Web.UI.Page


    Private Sub M1InvoiceBidding_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim obj As New Object()

        obj = "{'Buyer_Name': 'GLOBUS SPIRITS','Supplier_Name': 'TIRUPATI AGRO','Supplier_Code': 'BUYER00000047','Invoices': [{'PV_Unique_Ref_No': '66127','Invoice_NO': 'TIR002','Invoice_Doc_No': 'GLOBUS102','Invoice_Date': '2017-03-24'},{'PV_Unique_Ref_No': '66127','Invoice_NO': 'TIR002','Invoice_Doc_No': 'GLOBUS102','Invoice_Date': '2017-03-24'}],'Financer_Amount': '1485.23','Buyer_Amount': '17485.23','Supplier_Payment_Date': '2017-03-22','PV_Factoring_Unit_No': 'PV82760290320175051','Financer_Name': 'Yahoo','Financer_Payment_Date': '2017-03-22','charge_Type': 'A','charge_Amount': '258.46','Interest_Rate': '12','Treds_Tracking_No': '145','Treds_Sending_DateTime': '2017-03-22'}"
        Dim objNewResponse As New Object()

        MakeRequest(requestUrl:="http://161.202.19.217:8888/PVAPIS/createResponse", JSONRequest:=obj, JSONmethod:="POST", JSONContentType:="application/json; charset=utf-8")
    End Sub

    Public Class objGeneric
        Public Property Buyer_Name As String
        Public Property Supplier_Name As String
        Public Property Supplier_Code As String
        Public Property Invoices As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String
        'Public Property Buyer_Name As String

    End Class





    Public Shared Function MakeRequest(requestUrl As String, JSONRequest As Object, JSONmethod As String, JSONContentType As String) As Object

        Try

            ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

            'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
            'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
            'ServicePointManager.SecurityProtocol = Tls12

            Dim request As HttpWebRequest = TryCast(WebRequest.Create(requestUrl), HttpWebRequest)
            request.ContentType = JSONContentType
            request.Method = JSONmethod
            Dim sb As String = JsonConvert.SerializeObject(JSONRequest)
            Dim bt As [Byte]() = Encoding.UTF8.GetBytes(sb)
            Dim st As Stream = request.GetRequestStream()
            st.Write(bt, 0, bt.Length)
            st.Close()
            Using response As HttpWebResponse = TryCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode <> HttpStatusCode.OK Then
                    Throw New Exception([String].Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription))
                End If
                Dim stream1 As Stream = response.GetResponseStream()
                Dim sr As New StreamReader(stream1)
                Dim strsb As String = sr.ReadToEnd()
                Dim objResponse As Object = JsonConvert.DeserializeObject(strsb)
                Return objResponse
            End Using
        Catch e As Exception
            Console.WriteLine(e.Message)
            Return Nothing
        End Try
    End Function
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        'Try
        '    If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
        '        Page.Theme = Convert.ToString(Session("CTheme"))
        '    Else
        '        Page.Theme = "Default"
        '    End If
        'Catch ex As Exception
        'End Try

    End Sub
End Class
