' NOTE: You can use the "Rename" command on the context menu to change the class name "Lenskart" in code, svc and config file together.
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.ServiceModel.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class PearlWS
    Implements IPearlWS

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Function GenerateToken(userDetails As APIUserDetails) As String Implements IPearlWS.GenerateToken
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
        Return JsonConvert.SerializeObject(responseData)
    End Function

    Public Function CreateUpdateTransaction(Data As Stream) As String Implements IPearlWS.CreateUpdateTransaction
        Dim responseData As ResponseData = New ResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty Then
                responseData = ValidateToken(headers("Token"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim reader As New StreamReader(Data)
                        Dim strData As String = reader.ReadToEnd()
                        Dim jsonObject = JsonConvert.DeserializeObject(strData)
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~Data$$")
                        Dim jsonArray As JArray = jsonObject.Item("Data")
                        'pay load expect to our existing web service 
                        'Dim inputString As String = "Key$$BHJRIGSAKFG036622GIC~DOCTYPE$$Payment Details Doc~Data$$Payment Voucher Ref::70076|Journal Line Number::Non PO|Paid Amount::39788.000|PEARL ID::Inv1018|Bank Payment Reference::50002107|Other Deduction::1526.00|Payment Date::20/05/2021"
                        For Each item As JObject In jsonArray.Children(Of JObject)()
                            For Each element As JProperty In item.Properties()
                                If element.First.ToArray().Count() > 1 Then
                                    sb.Append(element.Name & "{}()")
                                    For Each child In element.First.ToArray()
                                        For Each Ichild As JProperty In child
                                            sb.Append(Ichild.Name & "<>" + Ichild.Value.ToString() & "()")
                                        Next
                                    Next
                                Else
                                    sb.Append(element.Name & "::" + element.Value.ToString() & "|")
                                End If
                            Next
                        Next
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
        Return JsonConvert.SerializeObject(responseData)
    End Function

    Public Function DocumentApproval(Data As Stream) As String Implements IPearlWS.DocumentApproval
        Dim responseData As ResponseData = New ResponseData
        Try
            Dim IncomingRequest As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest
            Dim headers As WebHeaderCollection = IncomingRequest.Headers
            If headers("Token") IsNot Nothing And headers("Token") <> String.Empty Then
                responseData = ValidateToken(headers("Token"))
                If responseData.Message = "success" Then
                    If headers("DocType") IsNot Nothing And headers("DocType") <> String.Empty And headers("Key") IsNot Nothing And headers("Key") <> String.Empty Then
                        Dim reader As New StreamReader(Data)
                        Dim strData As String = reader.ReadToEnd()
                        Dim jsonObject = JsonConvert.DeserializeObject(strData)
                        Dim sb As StringBuilder = New StringBuilder
                        sb.Append("Key$$" + headers("Key") + "~DocType$$" + headers("DocType") + "~ActionType$$" + headers("ActionType") + "~Data$$")
                        Dim jsonArray As JArray = jsonObject.Item("Data")
                        'pay load expect to our existing web service 
                        'Dim inputString As String = "Key$$BHJRIGSAKFG036622GIC~DOCTYPE$$Payment Details Doc~Data$$Payment Voucher Ref::70076|Journal Line Number::Non PO|Paid Amount::39788.000|PEARL ID::Inv1018|Bank Payment Reference::50002107|Other Deduction::1526.00|Payment Date::20/05/2021"
                        For Each item As JObject In jsonArray.Children(Of JObject)()
                            For Each element As JProperty In item.Properties()
                                If element.First.ToArray().Count() > 1 Then
                                    sb.Append(element.Name & "{}()")
                                    For Each child In element.First.ToArray()
                                        For Each Ichild As JProperty In child
                                            sb.Append(Ichild.Name & "<>" + Ichild.Value.ToString() & "()")
                                        Next
                                    Next
                                Else
                                    sb.Append(element.Name & "::" + element.Value.ToString() & "|")
                                End If
                            Next
                        Next
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
            ErrorLog.sendMail("MyndBPMWS.DocumentApproval", ex.Message)
        End Try
        Return JsonConvert.SerializeObject(responseData)
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

End Class


