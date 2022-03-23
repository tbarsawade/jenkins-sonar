Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic
Imports System.IO
Imports System.ServiceModel

Public Class PearlInwardWSModel

    Public Class PearlResponseData
        Public Property Message As String
        Public Property Token As String

    End Class

    Public Class APIUserDetails
        Public Property UserId As String
        Public Property Pwd As String
        Public Property Code As String
        Public Property EId As String
    End Class


    'lenskart
    Public Class Location_Master
        Public Property Code As String
        Public Property Name As String
        Public Property Address As String
        Public Property Address_2 As String
        Public Property City As String
        Public Property Post_Code As String
        Public Property State_Code As String
        Public Property TAN_No As String
        Public Property GST_Registration_No As String
    End Class

    Public Class Master_Code

        Public Property Code As String
        Public Property Name As String

    End Class


    Public Class Vendor_Modification
        Public Property Vendor_Code As String
        Public Property Vendor_Name As String
        Public Property Vendor_Name_old As String
        Public Property Vendor_Email As String
        Public Property Contact_Person_Name As String
        Public Property Contact_Person_Mobile As String
        Public Property Contact_Person_Email As String
        Public Property Billing_Address_Line1 As String
        Public Property Billing_Address_Line2 As String
        Public Property Billing_Address_City As String
        Public Property Billing_Address_State As String
        Public Property Billing_Address_PIN_Code As String
        Public Property Telephone_1 As String
        Public Property Communication_Address_1 As String
        Public Property Communication_Address_2 As String
        Public Property Communication_Add_City As String
        Public Property Communication_Add_State As String
        Public Property Communication_Add_Pincode As String
        Public Property PAN_Number As String
        Public Property Organization_Type As String
        Public Property GST_Status As String
        Public Property GSTIN_Num As String
        Public Property MSME_Registration As String
        Public Property MSME_Registration_Number As String
        Public Property Payee_Name As String
        Public Property Bank_Name As String
        Public Property Bank_Account_No As String
        Public Property IFSC_Code As String
        Public Property Currency As String
        Public Property E_Invoice_Applicable As String
        Public Property TCS_Applicable As String
        Public Property ESI_Reg_No As String
        Public Property ESI_Reg_Date As String
        Public Property P_F_for_Service_type As String
        Public Property Declaration_Received As String
    End Class

    'Lenskart

    Public Class VLVendorCodeupdate

        Public Property VendorCode As String
        Public Property PearlID As String

    End Class

    Public Class VLGRNDatalist

        Public Property GRNNum As String
        Public Property GRNDate As String
        Public Property VendorName As String
        Public Property VendorCode As String
        Public Property Department As String
        Public Property DeliveryLocation As String
        Public Property DeliveryState As String
        Public Property SupplierInvoiceNo As String
        Public Property SupplierInvoiceDate As String
        Public Property PONumber As String
        Public Property GRNTotalAmount As String
        Public Property Remarks As String
        Public Property Currency As String
        Public Property SubUnit As String
        Public Property GRNItems As List(Of VLGRN_Line_Items)
        'line


    End Class
    Public Class VLGRN_Line_Items


        Public Property ItemType As String
        Public Property LineNum As String
        Public Property ItemCode As String
        Public Property ItemName As String
        Public Property UoM As String
        Public Property Rate As String
        Public Property HSNCode As String
        Public Property GRNQty As String
        Public Property GSTRate As String
        Public Property GSTType As String
        Public Property LineAmount As String
        Public Property BalanceQty As String
        Public Property PONumber As String

    End Class

    Public Class VLPurchaseOrderlist

        Public Property PONumber As String
        Public Property PODate As String
        Public Property CompanyName As String
        Public Property Location As String
        Public Property Warehouse As String
        Public Property Department As String
        Public Property VendorCode As String
        Public Property VendorName As String
        Public Property VendorsGST As String
        Public Property VendorsPAN As String
        Public Property PaymentTerms As String
        Public Property ExpectedReceiptDate As String
        Public Property Currency As String
        Public Property POAmountTax As String
        Public Property CurrencyExchangeRate As String
        Public Property POAmountINR As String
        Public Property PORemarks As String
        Public Property SubUnit As String
        Public Property POItems As List(Of VLPO_Line_Items)
        'line


    End Class

    Public Class VLPO_Line_Items


        Public Property SLNo As String
        Public Property ItemType As String
        Public Property ItemCode As String
        Public Property ItemName As String
        Public Property UoM As String
        Public Property UnitPrice As String
        Public Property POQty As String
        Public Property LineAmount As String
        Public Property AmtinReportingCurrency As String
        Public Property HSNSACCode As String
        Public Property GSTRate As String
        Public Property GSTType As String
        Public Property BalanceQty As String

    End Class


    Public Class VLERPVoucherNUmber

        Public Property AccountingRemarks As String
        Public Property AccountingVoucherNum As String
        Public Property AccountingDate As String
        Public Property PearlID As String
        Public Property DebitNoteAmount As String
        Public Property DebitNoteReferenceNo As String

    End Class

    Public Class VLPaymentDetailUpdate

        Public Property PaymentMode As String
        Public Property PaymentVoucherreference As String
        Public Property PaymentDate As String
        Public Property PaidAmount As String
        Public Property TDSDeduction As String
        Public Property PearlID As String

    End Class


End Class

