Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IBPMMobileOffline" in both code and config file together.
<ServiceContract()>
Public Interface IBPMMobileOffline

    <OperationContract()> _
  <WebGet(UriTemplate:="/SyncData?Key={Key}&UserID={UserID}&EID={EID}&URole={URole}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncData(Key As String, UserID As String, EID As String, URole As String, IMINum As String, ST As String) As String

    <OperationContract()> _
       <WebGet(UriTemplate:="/GetDataOFAllForm?Key={Key}&EID={EID}&URole={URole}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetDataOFAllForm(Key As String, EID As String, URole As String, IMINum As String, ST As String) As List(Of FormData)

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetChildItemDefaultValueALL?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetChildItemDefaultValueALL(Key As String, EID As String, IMINum As String, ST As String) As List(Of ChildItemDefaultValue)

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetUserMenu?Key={Key}&EID={EID}&UserRole={UserRole}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetUserMenu(Key As String, EID As String, UserRole As String, IMINum As String, ST As String) As List(Of UserMenu)

    <OperationContract()> _
    <WebGet(UriTemplate:="/Syncacknowledgement?Key={Key}&EID={EID}&IMINum={IMINum}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function Syncacknowledgement(Key As String, EID As String, IMINum As String) As String

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetFormValidaion?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetFormValidaion(Key As String, EID As String, IMINum As String, ST As String) As List(Of FormValidation)


    <OperationContract()> _
   <WebGet(UriTemplate:="/GetFormDetails?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetFormDetails(Key As String, EID As String, IMINum As String, ST As String) As List(Of FormDetails)

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetPDFTemplet?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetPDFTemplet(Key As String, EID As String, IMINum As String, ST As String) As List(Of PDFTemplet)
    <OperationContract()> _
   <WebGet(UriTemplate:="/ChangePassword?Key={Key}&EID={EID}&UID={UID}&cpwd={cpwd}&npwd={npwd}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function ChangePassword(key As String, ByVal EID As String, ByVal UID As Integer, ByVal cpwd As String, ByVal npwd As String) As String
    <OperationContract()> _
   <WebGet(UriTemplate:="/ForgotPassword?UserName={UserName}&ECODE={ECODE}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function ForgotPassword(UserName As String, ECODE As String) As String
    <OperationContract()> _
   <WebGet(UriTemplate:="/SyncUser?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncUser(Key As String, EID As String, IMINum As String, ST As String) As String
    <OperationContract()> _
  <WebGet(UriTemplate:="/GetTriggers?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetTriggers(Key As String, EID As String, IMINum As String, ST As String) As List(Of Trigger)
    <OperationContract()> _
  <WebGet(UriTemplate:="/SyncDeletedFields?Key={Key}&EID={EID}&IMINum={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncDeletedFields(Key As String, EID As String, IMINum As String, ST As String) As String
End Interface
