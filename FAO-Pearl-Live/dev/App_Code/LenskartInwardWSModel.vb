Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic
Imports System.IO
Imports System.ServiceModel



Public Class LenskartInwardWSModel

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

    Public Class ResponseData
        Public Property Message As String
        Public Property Token As String

    End Class

    Public Class APIUserDetails
        Public Property UserId As String
        Public Property Pwd As String
        Public Property Code As String
        Public Property EId As String
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
End Class
