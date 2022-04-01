Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Web

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IPearlWS" in both code and config file together.
<ServiceContract()>
Public Interface IPearlWS

    <OperationContract>
    <WebInvoke(UriTemplate:="/GenerateToken", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Function GenerateToken(userDetails As APIUserDetails) As String

    <OperationContract>
    <WebInvoke(UriTemplate:="/CreateUpdateTransaction", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Function CreateUpdateTransaction(Data As Stream) As String


    <OperationContract>
    <WebInvoke(UriTemplate:="/DocumentApproval", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Function DocumentApproval(Data As Stream) As String



End Interface
