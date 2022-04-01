Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports PearlInwardWSModel
Imports Newtonsoft.Json

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class PearlInwardWS


    ' Dim constr As String = "server=MYNDHOSTDBVIP1;initial catalog=DMS;uid=DMS;pwd=mY#4dmP$juCh"

    'Dim constr As String = "server=172.17.159.44;initial catalog=PEARL_TTSL;uid=PEARL_TTSL;pwd=P&aTt5L22"

    <OperationContract>
    <WebInvoke(UriTemplate:="/GenerateToken", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function GenerateToken(ByVal userDetails As APIUserDetails) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Try
            If userDetails.UserId IsNot Nothing And userDetails.UserId <> String.Empty And userDetails.Pwd IsNot Nothing And userDetails.Pwd <> String.Empty And userDetails.Code IsNot Nothing And userDetails.Code <> String.Empty And userDetails.EId IsNot Nothing And userDetails.EId <> String.Empty Then
                Dim dataClass As New DataClass()
                Dim dt As DataTable = dataClass.ExecuteQryDT("SELECT * FROM mmm_mst_entity  where UserID='" & userDetails.UserId & "' COLLATE SQL_Latin1_General_CP1_CS_AS  AND pwd='" & userDetails.Pwd & "' COLLATE SQL_Latin1_General_CP1_CS_AS AND  Code='" & userDetails.Code & "' COLLATE SQL_Latin1_General_CP1_CS_AS AND EID='" & userDetails.EId & "'")
                If dt.Rows.Count > 0 Then
                    Dim TokenExpireDate As Date = Date.Now.AddHours(ConfigurationManager.AppSettings("TokenExpTime"))
                    Dim Token As String = Guid.NewGuid().ToString()
                    Using cmd As New SqlCommand("Usp_UpdateTokenId", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@TokenId", Token)
                        cmd.Parameters.AddWithValue("@TokenExpireDate", TokenExpireDate)
                        cmd.Parameters.AddWithValue("@EId", dt.Rows(0).Item("EID"))


                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        Dim i As Integer = cmd.ExecuteNonQuery()
                        If i > 0 Then
                            responseData.Token = Token
                            responseData.Message = "Token generated successfully."
                        End If
                    End Using
                Else
                    responseData.Message = "Invalid UserId Or Password."
                End If
            Else
                responseData.Message = "Please provide the valid request."
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.GenerateToken", ex.Message)
        Finally
            con.Close()
        End Try
        Return responseData
    End Function
    Private Function ValidateToken(ByVal Token As String, ByVal key As String) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Dim dataClass As New DataClass()
        Dim dt As DataTable = dataClass.ExecuteQryDT("select EID,UserId,TokenId,TokenExpiredDate from mmm_mst_entity where TokenId='" & Token & "' And APIkey='" & key & "'")
        If dt.Rows.Count > 0 Then
            Dim TokenExpireDate = dt.Rows(0).Item("TokenExpiredDate")
            If TokenExpireDate > DateTime.Now Then
                responseData.Message = "success"
            Else
                'If Token Is Expire then Generate a new token and send back to user
                Token = Guid.NewGuid().ToString()
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As New SqlConnection(constr)
                Using cmd As New SqlCommand("Usp_UpdateTokenId", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@TokenId", Token)
                    cmd.Parameters.AddWithValue("@TokenExpireDate", Date.Now.AddHours(ConfigurationManager.AppSettings("TokenExpTime")))
                    cmd.Parameters.AddWithValue("@EId", dt.Rows(0).Item("EID"))
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim i As Integer = cmd.ExecuteNonQuery()
                    If i > 0 Then
                        responseData.Message = "Your token is expired please process with new token"
                        responseData.Token = Token
                    End If
                End Using
            End If
        Else
            responseData.Message = "Invalid Token "
        End If
        Return responseData
    End Function

    <OperationContract>
    <WebInvoke(UriTemplate:="/LKMaster", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function Master(ByVal locationList As Master_Code) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")
                        'For Each item As Master_Code In locationList
                        If locationList.Code IsNot Nothing And locationList.Name IsNot Nothing Then
                            sb.Append("Name::" & locationList.Name.ToString() & "|Code::" & locationList.Code.ToString())

                            'ElseIf item.Name IsNot Nothing And item.Code IsNot Nothing Then
                            '    sb.Append("Name::" & item.Name.ToString() & "|Code::" & item.Code.ToString())
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        'Next
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.UpdateDataA(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData
    End Function

    <OperationContract>
    <WebInvoke(UriTemplate:="/LKLocationMaster", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function LocationMaster(ByVal LocationMasterLIST As Location_Master) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")
                        'For Each item As Location_Master In LocationMasterLIST
                        If LocationMasterLIST.Name IsNot Nothing And LocationMasterLIST.Code IsNot Nothing Then
                            sb.Append("Code::" & LocationMasterLIST.Code.ToString() & "|Name::" & LocationMasterLIST.Name.ToString() & "|Address::" & LocationMasterLIST.Address.ToString() & "|Address 2::" & LocationMasterLIST.Address_2.ToString() & "|City::" & LocationMasterLIST.City.ToString() & "|Post Code::" & LocationMasterLIST.Post_Code.ToString() & "|State Code::" & LocationMasterLIST.State_Code.ToString() & "|T.A.N. No.::" & LocationMasterLIST.TAN_No.ToString() & "|GST Registration No.::" & LocationMasterLIST.GST_Registration_No.ToString())
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        'Next
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.UpdateDataA(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData

    End Function

    <OperationContract>
    <WebInvoke(UriTemplate:="/LKVendorModification", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function VendorModification(ByVal Vendor_ModificationLIST As Vendor_Modification) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")
                        'For Each item As Location_Master In LocationMasterLIST
                        If Vendor_ModificationLIST.Vendor_Code IsNot Nothing And Vendor_ModificationLIST.Vendor_Name IsNot Nothing Then
                            sb.Append("Vendor Code::" & Vendor_ModificationLIST.Vendor_Code.ToString() & "|Vendor Name.::" & Vendor_ModificationLIST.Vendor_Name.ToString() & "|Vendor Email::" & Vendor_ModificationLIST.Vendor_Email.ToString() & "|Contact Person Name::" & Vendor_ModificationLIST.Contact_Person_Name.ToString() & "|Contact Person Mobile::" & Vendor_ModificationLIST.Contact_Person_Mobile.ToString() & "|Contact Person Email::" & Vendor_ModificationLIST.Contact_Person_Email.ToString() & "|Billing Address Line1::" & Vendor_ModificationLIST.Billing_Address_Line1.ToString() & "|Billing Address Line2::" & Vendor_ModificationLIST.Billing_Address_Line2.ToString() & "|Billing Address City::" & Vendor_ModificationLIST.Billing_Address_City.ToString() & "|Billing Address State::" & Vendor_ModificationLIST.Billing_Address_State.ToString() & "|Billing Address PIN Code::" & Vendor_ModificationLIST.Billing_Address_PIN_Code.ToString() & "|Telephone 1::" & Vendor_ModificationLIST.Telephone_1.ToString() & "|Communication Address 1.::" & Vendor_ModificationLIST.Communication_Address_1.ToString() & "|Communication Address 2::" & Vendor_ModificationLIST.Communication_Address_2.ToString() & "|Communication Add City::" & Vendor_ModificationLIST.Communication_Add_City.ToString() & "|Billing Address State::" & Vendor_ModificationLIST.Communication_Add_State.ToString() & "|Communication Add Pincode::" & Vendor_ModificationLIST.Communication_Add_Pincode.ToString() & "|PAN Number::" & Vendor_ModificationLIST.PAN_Number.ToString() & "|Organization Type::" & Vendor_ModificationLIST.Organization_Type.ToString() & "|GST Status::" & Vendor_ModificationLIST.GST_Status.ToString() & "|GSTIN Num::" & Vendor_ModificationLIST.GSTIN_Num.ToString() & "|MSME Registration::" & Vendor_ModificationLIST.MSME_Registration.ToString() & "|MSME Registration Number::" & Vendor_ModificationLIST.MSME_Registration_Number.ToString() & "|Payee Name::" & Vendor_ModificationLIST.Payee_Name.ToString() & "|Bank Name::" & Vendor_ModificationLIST.Bank_Name.ToString() & "|Bank Account No::" & Vendor_ModificationLIST.Bank_Account_No.ToString() & "|IFSC Code::" & Vendor_ModificationLIST.IFSC_Code.ToString() & "|Currency::" & Vendor_ModificationLIST.Currency.ToString() & "|E Invoice Applicable.::" & Vendor_ModificationLIST.E_Invoice_Applicable.ToString() & "|TCS Applicable::" & Vendor_ModificationLIST.TCS_Applicable.ToString() & "|ESI Reg No.::" & Vendor_ModificationLIST.ESI_Reg_No.ToString() & "|ESI Reg Date::" & Vendor_ModificationLIST.ESI_Reg_Date.ToString() & "|P.F.  for Service type::" & Vendor_ModificationLIST.P_F_for_Service_type.ToString() & "|Declaration Received::" & Vendor_ModificationLIST.Declaration_Received.ToString() & "|Vendor Name::" & Vendor_ModificationLIST.Vendor_Name_old.ToString())
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        'Next
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.UpdateDataA(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData

    End Function

    <OperationContract>
    <WebInvoke(UriTemplate:="/VLVendorCodeupdate", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function VendorCodeupdate(ByVal VendorCodeupdateList As VLVendorCodeupdate) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~ACTIONTYPE$$" + headers("ACTIONTYPE") + "~Data$$")
                        If VendorCodeupdateList.VendorCode IsNot Nothing And VendorCodeupdateList.PearlID IsNot Nothing Then
                            sb.Append("Vendor Code::" & VendorCodeupdateList.VendorCode.ToString() & "|PEARL ID::" & VendorCodeupdateList.PearlID.ToString())
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.DocumentApproval(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData
    End Function
    <OperationContract>
    <WebInvoke(UriTemplate:="/VLGRNData", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function GRNData(ByVal VLccGRNDatalist As VLGRNDatalist) As PearlResponseData


        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")

                        If VLccGRNDatalist.GRNNum IsNot Nothing And VLccGRNDatalist.GRNDate IsNot Nothing Then

                            sb.Append("GRN Num::" & VLccGRNDatalist.GRNNum.ToString() & "|GRN Date::" & VLccGRNDatalist.GRNDate.ToString() & "|Vendor Name::" & VLccGRNDatalist.VendorName.ToString() & "|Vendor Code::" & VLccGRNDatalist.VendorCode.ToString() & "|Department::" & VLccGRNDatalist.Department.ToString() & "|Delivery Location::" & VLccGRNDatalist.DeliveryLocation.ToString() & "|Delivery State::" & VLccGRNDatalist.DeliveryState.ToString() & "|Supplier Invoice No::" & VLccGRNDatalist.SupplierInvoiceNo.ToString() & "|Supplier Invoice Date::" & VLccGRNDatalist.SupplierInvoiceDate.ToString() & "|PO Number::" & VLccGRNDatalist.PONumber.ToString() & "|GRN Total Amount::" & VLccGRNDatalist.GRNTotalAmount.ToString() & "|Remarks::" & VLccGRNDatalist.Remarks.ToString() & "|Currency::" & VLccGRNDatalist.Currency.ToString() & "|Sub Unit::" & VLccGRNDatalist.SubUnit.ToString() & "| GRN Item::")

                            If VLccGRNDatalist.GRNItems IsNot Nothing Then

                                For i As Integer = 0 To VLccGRNDatalist.GRNItems.Count - 1

                                    Dim Line_Num As String = VLccGRNDatalist.GRNItems(i).LineNum
                                    Dim Item_Type As String = VLccGRNDatalist.GRNItems(i).ItemType
                                    Dim Item_Code As String = VLccGRNDatalist.GRNItems(i).ItemCode
                                    Dim Item_Name As String = VLccGRNDatalist.GRNItems(i).ItemName
                                    Dim UoM As String = VLccGRNDatalist.GRNItems(i).UoM
                                    Dim Rate As String = VLccGRNDatalist.GRNItems(i).Rate
                                    Dim HSN_Code As String = VLccGRNDatalist.GRNItems(i).HSNCode
                                    Dim GRN_Qty As String = VLccGRNDatalist.GRNItems(i).GRNQty
                                    Dim GST_Rate As String = VLccGRNDatalist.GRNItems(i).GSTRate
                                    Dim GST_Type As String = VLccGRNDatalist.GRNItems(i).GSTType
                                    Dim Line_Amount As String = VLccGRNDatalist.GRNItems(i).LineAmount
                                    Dim Balance_Qty As String = VLccGRNDatalist.GRNItems(i).BalanceQty
                                    Dim PO_Number As String = VLccGRNDatalist.GRNItems(i).PONumber
                                    sb.Append("{}()Item Type<>" & Item_Type.ToString & "()Item Code<>" & Item_Code.ToString() & "()Item Name<>" & Item_Name.ToString() & "()UoM<>" & UoM.ToString() & "()Rate<>" & Rate.ToString() & "()HSN Code<>" & HSN_Code.ToString() & "()GRN Qty<>" & GRN_Qty.ToString() & "()GST Rate<>" & GST_Rate.ToString() & "()GST Type<>" & GST_Type.ToString() & "()Line Amount<>" & Line_Amount.ToString() & "()Balance Qty<>" & Balance_Qty.ToString() & "()Line Num<>" & Line_Num.ToString() & "()PO Number<>" & PO_Number.ToString())

                                Next
                            End If
                        Else
                            responseData.Message = "Please check your properties name."
                        End If

                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        'responseData.Message = myndService.UpdateDataA(ms)
                        responseData.Message = myndService.SaveData(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try

        Return responseData
    End Function


    <OperationContract>
    <WebInvoke(UriTemplate:="/VLPurchaseOrder", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function PurchaseOrder(ByVal VLccPurchaseOrderList As VLPurchaseOrderlist) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")

                        If VLccPurchaseOrderList.PONumber IsNot Nothing And VLccPurchaseOrderList.PODate IsNot Nothing Then
                            sb.Append("PO Number::" & VLccPurchaseOrderList.PONumber.ToString() & "|PO Date::" & VLccPurchaseOrderList.PODate.ToString() & "|Company Name::" & VLccPurchaseOrderList.CompanyName.ToString() & "|Location::" & VLccPurchaseOrderList.Location.ToString() & "|Warehouse::" & VLccPurchaseOrderList.Warehouse.ToString() & "|Department::" & VLccPurchaseOrderList.Department.ToString() & "|Vendor Code::" & VLccPurchaseOrderList.VendorCode.ToString() & "|Vendor Name::" & VLccPurchaseOrderList.VendorName.ToString() & "|Vendors GST::" & VLccPurchaseOrderList.VendorsGST.ToString() & "|Vendors PAN::" & VLccPurchaseOrderList.VendorsPAN.ToString() & "|Payment Terms::" & VLccPurchaseOrderList.PaymentTerms.ToString() & "|Expected Receipt Date::" & VLccPurchaseOrderList.ExpectedReceiptDate.ToString() & "|Currency::" & VLccPurchaseOrderList.Currency.ToString() & "|PO Amount (wo Tax)::" & VLccPurchaseOrderList.POAmountTax.ToString() & "|PO Amount (INR)::" & VLccPurchaseOrderList.POAmountINR.ToString() & "|Sub Unit::" & VLccPurchaseOrderList.SubUnit.ToString() & "| PO Item::")
                            If VLccPurchaseOrderList.POItems IsNot Nothing Then

                                For i As Integer = 0 To VLccPurchaseOrderList.POItems.Count - 1

                                    Dim SL_No As String = VLccPurchaseOrderList.POItems(i).SLNo
                                    Dim Item_Type As String = VLccPurchaseOrderList.POItems(i).ItemType
                                    Dim Item_Code As String = VLccPurchaseOrderList.POItems(i).ItemCode
                                    Dim Item_Name As String = VLccPurchaseOrderList.POItems(i).ItemName
                                    Dim UoM As String = VLccPurchaseOrderList.POItems(i).UoM
                                    Dim Unit_Price As String = VLccPurchaseOrderList.POItems(i).UnitPrice
                                    Dim PO_Qty As String = VLccPurchaseOrderList.POItems(i).POQty
                                    Dim Line_Amount As String = VLccPurchaseOrderList.POItems(i).LineAmount
                                    Dim Amt_in_Reporting_Currency As String = VLccPurchaseOrderList.POItems(i).AmtinReportingCurrency
                                    Dim HSN_SAC_Code As String = VLccPurchaseOrderList.POItems(i).HSNSACCode
                                    Dim GST_Rate As String = VLccPurchaseOrderList.POItems(i).GSTRate
                                    Dim GST_Type As String = VLccPurchaseOrderList.POItems(i).GSTType
                                    Dim Balance_Qty As String = VLccPurchaseOrderList.POItems(i).BalanceQty

                                    sb.Append("{}()Item Type<>" & Item_Type.ToString & "()Item Code<>" & Item_Code.ToString() & "()SL No<>" & SL_No.ToString() & "()GST Type<>" & GST_Type.ToString() & "()PO Qty<>" & PO_Qty.ToString() & "()Item Name<>" & Item_Name.ToString() & "()UoM<>" & UoM.ToString() & "()Unit Price<>" & Unit_Price.ToString() & "()Line Amount<>" & Line_Amount.ToString() & "()Amt in Reporting Currency<>" & Amt_in_Reporting_Currency.ToString() & "()HSN SAC Code<>" & HSN_SAC_Code.ToString() & "()GST Rate<>" & GST_Rate.ToString() & "()Balance Qty<>" & Balance_Qty.ToString())

                                Next
                            End If

                            responseData.Message = "Please check your properties name."
                        End If

                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.UpdateDataA(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try

        Return responseData
    End Function
    <OperationContract>
    <WebInvoke(UriTemplate:="/VLERPVoucherNumber", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function VLERPVoucherNumber(ByVal VLERPVoucherNUmberList As VLERPVoucherNUmber) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    'NPO Accounting App,IPO Accounting App,IG Accounting App
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~ACTIONTYPE$$" + headers("ACTIONTYPE") + "~Data$$")
                        If VLERPVoucherNUmberList.AccountingVoucherNum IsNot Nothing And VLERPVoucherNUmberList.AccountingDate IsNot Nothing Then
                            sb.Append("Accounting Voucher Num::" & VLERPVoucherNUmberList.AccountingVoucherNum.ToString() & "|Accounting Date::" & VLERPVoucherNUmberList.AccountingDate.ToString() & "|Accounting Remarks::" & VLERPVoucherNUmberList.AccountingRemarks.ToString() & "|Pearl Id::" & VLERPVoucherNUmberList.PearlID.ToString() & "|Debit Note Amount::" & VLERPVoucherNUmberList.DebitNoteAmount.ToString() & "|Debit Note Reference No::" & VLERPVoucherNUmberList.DebitNoteReferenceNo.ToString())
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.DocumentApproval(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData
    End Function

    <OperationContract>
    <WebInvoke(UriTemplate:="/VLPaymentDetailUpdate", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function VLPaymentDetailUpdate(ByVal VLPaymentDetailUpdateList As VLPaymentDetailUpdate) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    'IPO Payment Update App,NPO Payment Update App,IG Payment Update App
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~ACTIONTYPE$$" + headers("ActionType") + "~Data$$")
                        If VLPaymentDetailUpdateList.PaymentMode IsNot Nothing And VLPaymentDetailUpdateList.PaymentVoucherreference IsNot Nothing Then
                            sb.Append("Payment Mode::" & VLPaymentDetailUpdateList.PaymentMode.ToString() & "|Payment Voucher reference::" & VLPaymentDetailUpdateList.PaymentVoucherreference.ToString() & "|Payment Date::" & VLPaymentDetailUpdateList.PaymentDate.ToString() & "|Paid Amount::" & VLPaymentDetailUpdateList.PaidAmount.ToString() & "|TDS Deduction::" & VLPaymentDetailUpdateList.TDSDeduction.ToString() & "|Pearl Id::" & VLPaymentDetailUpdateList.PearlID.ToString())
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.DocumentApproval(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData
    End Function

    <OperationContract>
    <WebInvoke(UriTemplate:="/TTSLPurchaseOrder", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function TTSLPurchaseOrder(ByVal PurchaseOrderList As TTSLPurchaseOrder) As PearlResponseData
        Dim responseData As PearlResponseData = New PearlResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                responseData = ValidateToken(headers("Token"), headers("Key"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")
                        If PurchaseOrderList.PONumber IsNot Nothing And PurchaseOrderList.PODate IsNot Nothing Then
                            sb.Append("PO Number::" & PurchaseOrderList.PONumber.ToString() & "|PO Date::" & PurchaseOrderList.PODate.ToString() & "|Vendor Code::" & PurchaseOrderList.VendorCode.ToString() & "|Vendor Name::" & PurchaseOrderList.VendorName.ToString() & "|Vendors GST::" & PurchaseOrderList.VendorsGST.ToString() & "|Vendors PAN::" & PurchaseOrderList.VendorsPAN.ToString() & "|Company Code::" & PurchaseOrderList.CompanyCode.ToString() & "|Category::" & PurchaseOrderList.Category.ToString() & "|PO Type::" & PurchaseOrderList.POType.ToString() & "|Circle::" & PurchaseOrderList.Circle.ToString() & "|Approver::" & PurchaseOrderList.Approver.ToString() & "|Approver Name::" & PurchaseOrderList.ApproverName.ToString() & "|Cost center::" & PurchaseOrderList.CostCentre.ToString() & "|Purchase Organization::" & PurchaseOrderList.PurchaseOrganization.ToString() & "|Department::" & PurchaseOrderList.Department.ToString() & "|Payment Terms::" & PurchaseOrderList.PaymentTerms.ToString() & "|Credit Period::" & PurchaseOrderList.CreditPeriod.ToString() & "|Currency::" & PurchaseOrderList.Currency.ToString() & "|GL Code::" & PurchaseOrderList.GLCode.ToString() & "|PO Amount::" & PurchaseOrderList.POAmount.ToString() & "|SGST::" & PurchaseOrderList.SGST.ToString() & "|CGST::" & PurchaseOrderList.CGST.ToString() & "|IGST::" & PurchaseOrderList.IGST.ToString() & "|UTGST::" & PurchaseOrderList.UTGST.ToString() & "|Total PO Amount::" & PurchaseOrderList.TotalPOAmount.ToString() & "|PO Remarks::" & PurchaseOrderList.PORemarks.ToString())
                            '& "|Dispute if any::" & PurchaseOrderList.Disputeifany.ToString() & "|Dispute Resolution Remarks::" & PurchaseOrderList.DisputeResolutionRemarks.ToString()
                        Else
                            responseData.Message = "Please check your properties name."
                        End If
                        Dim myndService As MyndBPMWS = New MyndBPMWS()
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(sb.ToString())
                        Dim ms As MemoryStream = New MemoryStream(byteArray)
                        responseData.Message = myndService.UpdateDataA(ms)
                        If responseData.Message Is String.Empty Then
                            responseData.Message = "Please check your data"
                        End If
                    Else
                        responseData.Message = "Please provide a valid request"
                    End If
                End If
            Else
                responseData.Message = "Token can not be empty"
            End If
        Catch ex As Exception
            responseData.Message = ex.Message
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
        End Try
        Return responseData
    End Function

    ' To use HTTP GET, add <WebGet()> attribute. (Default ResponseFormat is WebMessageFormat.Json)
    ' To create an operation that returns XML,
    '     add <WebGet(ResponseFormat:=WebMessageFormat.Xml)>,
    '     and include the following line in the operation body:
    '         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml"
    <OperationContract()>
    Public Sub DoWork()
        ' Add your operation implementation here
    End Sub

    ' Add more operations here and mark them with <OperationContract()>

End Class
