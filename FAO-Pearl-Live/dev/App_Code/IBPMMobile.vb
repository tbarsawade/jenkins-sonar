Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IBPMMobile" in both code and config file together.
<ServiceContract()>
Public Interface IBPMMobile

    <OperationContract()>
    <WebGet(UriTemplate:="/logout?UID={UID}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function logout(UID As String) As String
    <OperationContract()>
    <WebGet(UriTemplate:="/GetActiveAudiCycle?FieldID={FieldID}&ECode={ECode}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetActiveAudiCycle(FieldID As String, Ecode As String) As String
    <OperationContract()>
    <WebGet(UriTemplate:="/EmpNomCheck?userid={userid}&cid={cid}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function EmpNomCheck(userid As String, cid As String) As String
    <OperationContract()>
    <WebGet(UriTemplate:="/AuthanticateUser?UserID={UserID}&Password={Password}&ECode={ECode}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function AuthanticateUser(UserID As String, Password As String, ECode As String) As Userdetails
    <OperationContract()>
    <WebGet(UriTemplate:="/GetDataOFForm?Key={Key}&EID={EID}&FormName={FormName}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetDataOFForm(Key As String, EID As String, FormName As String) As List(Of FormData)
    <OperationContract()>
    <WebGet(UriTemplate:="/GetUserMenu?Key={Key}&EID={EID}&UserRole={UserRole}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetUserMenu(Key As String, EID As String, UserRole As String) As List(Of UserMenu)
    <OperationContract()>
    <WebGet(UriTemplate:="/ImportMaster?UserID={UserID}&Password={Password}&ECode={ECode}&DocType={DocType}&Data={Data}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function ImportMaster(UserID As String, Password As String, ECode As String, DocType As String, Data As String) As String
    <OperationContract()>
    <WebGet(UriTemplate:="/SyncData?Key={Key}&UserID={UserID}&EID={EID}&URole={URole}&IMINum={IMINum}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function SyncData(Key As String, UserID As String, EID As String, URole As String, IMINum As String) As List(Of DocData)
    <OperationContract>
    <WebInvoke(Method:="POST", UriTemplate:="/SaveDraft")>
    Function SaveDraft(Data As Stream) As String

    <OperationContract>
    <WebInvoke(Method:="POST", UriTemplate:="/SaveDraftA")>
    Function SaveDraftA(Data As Stream) As String

    <OperationContract>
    <WebInvoke(Method:="POST", UriTemplate:="/UpdateData")>
    Function UpdateData(Data As Stream) As String

    <OperationContract>
   <WebInvoke(UriTemplate:="/Base64DecodedImageName", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, Method:="POST", BodyStyle:=WebMessageBodyStyle.Wrapped)>
    Function Base64DecodedImageName(base64EncodedString As String, filePath As String) As String

    <OperationContract>
    <WebInvoke(Method:="POST", UriTemplate:="/UpdateDataA")>
    Function UpdateDataA(Data As Stream) As String

    <OperationContract>
    <WebInvoke(Method:="POST", UriTemplate:="/SaveData")>
    Function SaveData(Data As Stream) As String

    <OperationContract>
    <WebInvoke(Method:="POST", UriTemplate:="/SaveDataA")>
    Function SaveDataA(Data As Stream) As String

    <OperationContract()>
    <WebGet(UriTemplate:="/DropDownFilter?Key={Key}&EID={EID}&FieldID={FieldID}&DOCID={DOCID}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function DropDownFilter(Key As String, EID As String, FieldID As Integer, DOCID As Integer) As List(Of DropDownData)

    <OperationContract()>
    <WebGet(UriTemplate:="/GetChildItemDefaultValue?Key={Key}&EID={EID}&ChildItemName={ChildItemName}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetChildItemDefaultValue(Key As String, EID As String, ChildItemName As String) As List(Of ChildItemDefaultValue)
    <OperationContract()>
    <WebGet(UriTemplate:="/GetDataOFAllForm?Key={Key}&EID={EID}&URole={URole}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetDataOFAllForm(Key As String, EID As String, URole As String) As List(Of FormData)
    <OperationContract()>
    <WebGet(UriTemplate:="/GetChildItemDefaultValueALL?Key={Key}&EID={EID}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetChildItemDefaultValueALL(Key As String, EID As String) As List(Of ChildItemDefaultValue)
    <OperationContract()>
    <WebInvoke(Method:="POST", UriTemplate:="/PostTest")>
    Function PostTest(Data As Stream) As String
    <OperationContract()>
    <WebGet(UriTemplate:="/GetExternalLookup?Key={Key}&EID={EID}&FieldID={FieldID}&Value={Value}&UID={UID}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function GetExternalLookup(Key As String, EID As String, FieldID As Integer, Value As String, UID As Integer) As List(Of DropDownData)

    <OperationContract()>
    <WebGet(UriTemplate:="/Checkduplicate?BarCode={BarCode}", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)>
    Function Checkduplicate(BarCode As String) As String
End Interface
