Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports LenskartInwardWSModel
Imports Newtonsoft.Json

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class LenskartInward

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    <OperationContract>
    <WebInvoke(UriTemplate:="/Master", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function Master(ByVal locationList As Master_Code) As ResponseData
        Dim responseData As ResponseData = New ResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty Then
                responseData = ValidateToken(headers("Token"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")
                        'For Each item As Master_Code In locationList
                        If locationList.code IsNot Nothing And locationList.name IsNot Nothing Then
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

    Private Function ValidateToken(ByVal Token As String) As ResponseData
        Dim responseData As ResponseData = New ResponseData
        Dim dataClass As New DataClass()
        Dim dt As DataTable = dataClass.ExecuteQryDT("select EID,UserId,TokenId,TokenExpiredDate from mmm_mst_entity where TokenId='" & Token & "'")
        If dt.Rows.Count > 0 Then
            Dim TokenExpireDate = dt.Rows(0).Item("TokenExpiredDate")
            If TokenExpireDate > DateTime.Now Then
                responseData.Message = "success"
            Else
                'If Token Is Expire then Generate a new token and send back to user
                Token = Guid.NewGuid().ToString()
                Dim con As New SqlConnection(conStr)
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
    <WebInvoke(UriTemplate:="/GenerateToken", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function GenerateToken(ByVal userDetails As APIUserDetails) As ResponseData
        Dim responseData As ResponseData = New ResponseData
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


    <OperationContract>
    <WebInvoke(UriTemplate:="/LocationMaster", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function LocationMaster(ByVal LocationMasterLIST As Location_Master) As ResponseData
        Dim responseData As ResponseData = New ResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty Then
                responseData = ValidateToken(headers("Token"))
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
    <WebInvoke(UriTemplate:="/VendorModification", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Public Function VendorModification(ByVal Vendor_ModificationLIST As Vendor_Modification) As ResponseData
        Dim responseData As ResponseData = New ResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty Then
                responseData = ValidateToken(headers("Token"))
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

End Class


