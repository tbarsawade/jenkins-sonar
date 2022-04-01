Imports Microsoft.VisualBasic

Public Class InvoiceDetail
    Public Sub New(ByVal obj As Newtonsoft.Json.Linq.JObject)
        Dim docDate As String = obj("DocDt").ToString()
        Me.SellerGstin = obj("SellerGstin").ToString()
        Me.BuyerGstin = obj("BuyerGstin").ToString()
        Me.DocNo = obj("DocNo").ToString()
        Me.DocType = obj("DocTyp").ToString()
        Me.DocDate = DateTime.ParseExact(docDate, "dd/MM/yyyy", Nothing)
        Me.TotalInvoiceValue = Convert.ToDecimal(obj("TotInvVal"))
        Me.ItemCount = Convert.ToInt32(obj("ItemCnt"))
        Me.MainHsnCode = obj("MainHsnCode").ToString()
        Me.IRN = obj("Irn").ToString()
        Me.IRNDate = Convert.ToDateTime(obj("IrnDt"))
    End Sub

    Public Property SellerGstin As String
    Public Property BuyerGstin As String
    Public Property DocNo As String
    Public Property DocType As String
    Public Property DocDate As DateTime
    Public Property TotalInvoiceValue As Decimal
    Public Property ItemCount As Integer
    Public Property MainHsnCode As String
    Public Property IRN As String
    Public Property IRNDate As DateTime


End Class
Public Class EInvoiceSignatureParameters
    Public Property alg As String
    Public Property kid As String
    Public Property typ As String
    Public Property x5t As String
End Class
