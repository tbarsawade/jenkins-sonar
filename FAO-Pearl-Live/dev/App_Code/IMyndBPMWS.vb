Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IImportMaster" in both code and config file together.
<ServiceContract()>
Public Interface IMyndBPMWS
    <OperationContract> _
       <WebInvoke(Method:="POST", UriTemplate:="/SaveData")> _
    Function SaveData(Data As Stream) As String
    <OperationContract> _
       <WebInvoke(Method:="POST", UriTemplate:="/UpdateData")> _
    Function UpdateData(Data As Stream) As String

    <OperationContract> _
       <WebInvoke(Method:="POST", UriTemplate:="/UpdateDataA")> _
    Function UpdateDataA(Data As Stream) As String

    <OperationContract> _
       <WebInvoke(Method:="POST", UriTemplate:="/DocumentApproval")> _
    Function DocumentApproval(Data As Stream) As String

    <OperationContract> _
      <WebInvoke(Method:="POST", UriTemplate:="/DeleteDOCList")> _
    Function DeleteDOCList(Data As Stream) As String
    <OperationContract> _
     <WebInvoke(UriTemplate:="/SetTransaction", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")> _
    Function SetTransaction(Data As Stream) As Grofers_Res

    <OperationContract> _
    <WebGet(UriTemplate:="/GeneratePDF?EmpCode={EmpCode}&Month={Month}&Year={Year}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GeneratePDF(EmpCode As String, Month As String, Year As String) As String

    <OperationContract> _
         <WebInvoke(UriTemplate:="/SsoAuth", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")> _
    Function SsoAuth(Data As Stream) As Hcl_Res

    <OperationContract>
    <WebInvoke(UriTemplate:="/Base64DecodedImageName", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST", BodyStyle:=WebMessageBodyStyle.Wrapped)>
    Function Base64DecodedImageName(base64EncodedString As String, filePath As String) As String

    <OperationContract>
    <WebInvoke(UriTemplate:="/Base64DecodedImageNameNew", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST", BodyStyle:=WebMessageBodyStyle.Wrapped)>
    Function Base64DecodedImageNameNew(base64EncodedString As String, filePath As String, Optional ByVal ext As String = "jpg") As String

    <OperationContract>
    <WebInvoke(UriTemplate:="/OcrInvoiceValues", RequestFormat:=WebMessageFormat.Xml, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Function OcrInvoiceValues(Data As Stream) As XElement

    <OperationContract>
    <WebInvoke(UriTemplate:="/RegisterationWithM1", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Function RegisterationWithM1(Data As M1Registration) As Grofers_Res

    <OperationContract>
  <WebInvoke(UriTemplate:="/M1_Disocunt_Detail_Update", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST")>
    Function M1_Disocunt_Detail_Update(Data As M1Discounting) As Grofers_Res


    'UpdateData
End Interface
