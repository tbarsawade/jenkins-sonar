Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IBPMMobileSyncAPI" in both code and config file together.
<ServiceContract()>
Public Interface IBPMMobileSyncAPI

    <OperationContract()> _
   <WebGet(UriTemplate:="/AuthanticateUser?UserID={UserID}&Password={Password}&ECode={ECode}&IMINumber={IMINumber}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function AuthanticateUser(UserID As String, Password As String, ECode As String, IMINumber As String) As Userdetails

    <OperationContract()> _
  <WebGet(UriTemplate:="/AuthanticateUserMultiRole?UserID={UserID}&Password={Password}&ECode={ECode}&IMINumber={IMINumber}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function AuthanticateUserMultiRole(UserID As String, Password As String, ECode As String, IMINumber As String) As Userdetails

    
   <OperationContract()> _
   <WebGet(UriTemplate:="/SyncData?Key={Key}&UID={UID}&EID={EID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncData(Key As String, UID As String, EID As String, URole As String, IMINumber As String, ST As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/SyncCRMDATA?Key={Key}&UID={UID}&EID={EID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncCRMDATA(Key As String, UID As String, EID As String, URole As String, IMINumber As String, ST As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/SyncDataA?Key={Key}&UID={UID}&EID={EID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncDataA(Key As String, UID As String, EID As String, URole As String, IMINumber As String, ST As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/SyncChild?Key={Key}&UID={UID}&EID={EID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncChild(Key As String, UID As String, EID As String, URole As String, IMINumber As String, ST As String) As String


    <OperationContract()> _
   <WebGet(UriTemplate:="/SyncDocDetails?Key={Key}&UID={UID}&EID={EID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncDocDetails(Key As String, UID As String, EID As String, URole As String, IMINumber As String, ST As String) As XElement

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetDataOFAllFormForApproval?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&URole={URole}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetDataOFAllFormForApproval(Key As String, EID As String, UID As Integer, URole As String, IMINumber As String, ST As String) As List(Of FormData)

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetDataOFAllForm?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&URole={URole}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetDataOFAllForm(Key As String, EID As String, UID As Integer, URole As String, IMINumber As String, ST As String) As List(Of FormData)

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetChildItemDefaultValueALL?Key={Key}&EID={EID}&UID={UID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetChildItemDefaultValueALL(Key As String, EID As String, UID As Integer, URole As String, IMINumber As String, ST As String) As List(Of ChildItemDefaultValue)

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetUserMenu?Key={Key}&EID={EID}&UserRole={UserRole}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetUserMenu(Key As String, EID As String, UserRole As String, UID As Integer, IMINumber As String, ST As String) As List(Of UserMenu)

    <OperationContract()> _
    <WebGet(UriTemplate:="/Syncacknowledgement?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function Syncacknowledgement(Key As String, EID As String, UID As Integer, IMINumber As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetFormValidaion?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetFormValidaion(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As List(Of FormValidation)


    <OperationContract()> _
    <WebGet(UriTemplate:="/GetFormDetails?Key={Key}&EID={EID}&UID={UID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetFormDetails(Key As String, EID As String, UID As Integer, URole As String, IMINumber As String, ST As String) As List(Of FormDetails)

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetPDFTemplet?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetPDFTemplet(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As List(Of PDFTemplet)

    <OperationContract()> _
    <WebGet(UriTemplate:="/ChangePassword?Key={Key}&EID={EID}&UID={UID}&cpwd={cpwd}&npwd={npwd}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function ChangePassword(key As String, ByVal EID As String, ByVal UID As Integer, ByVal cpwd As String, ByVal npwd As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/ForgotPassword?UserName={UserName}&ECODE={ECODE}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function ForgotPassword(UserName As String, ECODE As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/SyncAuthMatrix?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncAuthMatrix(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/SyncUser?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncUser(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetTriggers?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetTriggers(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As List(Of Trigger)

    <OperationContract()> _
    <WebGet(UriTemplate:="/SyncDeletedFields?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SyncDeletedFields(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As String

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetDashBoard?Key={Key}&EID={EID}&UID={UID}&URole={URole}&IMINumber={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetDashBoard(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of DashBoard)

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetChildCalandar?Key={Key}&EID={EID}&UID={UID}&URole={URole}&IMINumber={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetChildCalandar(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of ChildCalandar)

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetCalandar?Key={Key}&EID={EID}&UID={UID}&URole={URole}&IMINumber={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetCalandar(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of Calandar)

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetDocCalandar?Key={Key}&EID={EID}&UID={UID}&URole={URole}&IMINumber={IMINum}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetDocCalandar(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of DocCalandar)

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetWorkFlowSetting?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetWorkFlowSetting(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As XElement

    <OperationContract()> _
  <WebGet(UriTemplate:="/GetEntityInfo?Key={Key}&EID={EID}&UID={UID}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetEntityInfo(Key As String, EID As String, UID As Integer, IMINumber As String, ST As String) As XElement
    <OperationContract()> _
 <WebGet(UriTemplate:="/GetLastMobileSync?Key={Key}&UID={UID}&EID={EID}&URole={URole}&IMINumber={IMINumber}&ST={ST}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetLastMobileSync(Key As String, UID As String, EID As String, URole As String, IMINumber As String, ST As String) As XElement
    <WebGet(UriTemplate:="/GetCreatedDocumentByDocumenttype?Eid={Eid}&DocType={DocType}&UID={UID}&TimeStamp={TimeStamp}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetCreatedDocumentByDocumenttype(Eid As Integer, DocType As String, UID As Integer, TimeStamp As String) As arr3

    <WebGet(UriTemplate:="/GetCreatedDocumentByDocumenttype1?Eid={Eid}&DocType={DocType}&UID={UID}&TimeStamp={TimeStamp}&Filter={Filter}&AuthKey={AuthKey}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetCreatedDocumentByDocumenttype1(Eid As Integer, DocType As String, UID As Integer, TimeStamp As String, Filter As String, AuthKey As String) As arr3

    <WebGet(UriTemplate:="/PHRO_EmployeeRegistration?IMINumber={IMINumber}&Mobile_Number={Mobile_Number}&Emp_Code={Emp_Code}&Comp_Code={Comp_Code}&AdminAppr_Status={AdminAppr_Status}&Email_ID={Email_ID}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function PHRO_EmployeeRegistration(IMINumber As String, Mobile_Number As String, Emp_Code As String, Comp_Code As String, AdminAppr_Status As String, Email_ID As String) As String

    <WebGet(UriTemplate:="/PHRO_EmployeeVerifiedOTP?IMINumber={IMINumber}&Emp_Code={Emp_Code}&Comp_Code={Comp_Code}&Email_ID={Email_ID}&OTP={OTP}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function PHRO_EmployeeVerifiedOTP(IMINumber As String, Emp_Code As String, Comp_Code As String, Email_ID As String, OTP As String) As String

    <WebGet(UriTemplate:="/GetAssetss?Eid={Eid}&UID={UID}&TimeStamp={TimeStamp}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetAssetss(Eid As Integer, UID As Integer, TimeStamp As String) As arr4

End Interface
