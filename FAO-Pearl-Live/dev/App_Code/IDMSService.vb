Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports DMSService

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IDMSService" in both code and config file together.
<ServiceContract()>
Public Interface IDMSService

    <OperationContract>
  <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="/GETEIDDATA")>
    Function GETEIDDATA(Data As EIDDETAIL) As List(Of EIDRESPONSE)

    <OperationContract()> _
<WebGet(UriTemplate:="/Address?EID={EID}&Doctype={Doctype}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function Address(ByVal EID As Integer, ByVal doctype As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/MoveFile?EID={EID}&gid={gid}&stid={stid}&docurl={docurl}&oUID={oUID}&filesize={filesize}&Doctype={Doctype}&fup_fieldmapping={fup_fieldmapping}&loc_fieldmapping={loc_fieldmapping}&location_id={location_id}&barcodefldmapping={barcodefldmapping}&barcodefldval={barcodefldval}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MoveFile(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer, ByVal barcodefldmapping As String, ByVal barcodefldval As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/MoveFileDraft?EID={EID}&gid={gid}&stid={stid}&docurl={docurl}&oUID={oUID}&filesize={filesize}&Doctype={Doctype}&fup_fieldmapping={fup_fieldmapping}&loc_fieldmapping={loc_fieldmapping}&location_id={location_id}&barcodefldmapping={barcodefldmapping}&barcodefldval={barcodefldval}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MoveFileDraft(ByVal EID As Integer, ByVal gid As Integer, ByVal stid As Integer, ByVal docurl As String, ByVal oUID As Integer, ByVal filesize As Integer, ByVal Doctype As String, ByVal fup_fieldmapping As String, ByVal loc_fieldmapping As String, ByVal location_id As Integer, ByVal barcodefldmapping As String, ByVal barcodefldval As String) As String

    <OperationContract()> _
     <WebGet(UriTemplate:="/Run_FTP_Inward_Integration", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function Run_FTP_Inward_Integration() As String

    <OperationContract()> _
     <WebGet(UriTemplate:="/Run_Helpdesk_MailRead_Sch", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function Run_Helpdesk_MailRead_Sch() As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/ClusterwiseData?EID={EID}&UID={UID}&USERROLE={USERROLE}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function ClusterwiseData(ByVal EID As Integer, ByVal UID As Integer, ByVal USERROLE As String) As XElement

    <OperationContract()> _
    <WebGet(UriTemplate:="/CreateUsers?username={userName}&emailid={emailID}&pwd={pwd}&code={code}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function CreateUsers(userName As String, emailID As String, pwd As String, code As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/MyndVas?mn={mn}&msg={msg}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MyndVas(ByVal mn As String, ByVal msg As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/PushGPSData?imieno={imieno}&lat={lat}&longitude={longitude}&altitude={altitude}&speed={speed}&angle={angle}&cTime={cTime}&nosat={nosat}&IsGPS={IsGPS}&IsGPRS={IsGPRS}&distance={distance}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function PushGPSData(imieno As String, lat As Double, longitude As Double, altitude As Double, speed As Double, angle As Integer, cTime As Date, nosat As Integer, IsGPS As Integer, IsGPRS As Integer, distance As Double) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetGPSPoint?imieno={imieno}&sTime={sTime}&eTime={eTime}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetGPSPoint(imieno As String, sTime As Date, eTime As Date) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetNearestDevice?eid={eid}&imieno={imieno}&lattitudet={lattitude}&longitude={longitude}&stype={stype}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetNearestDevice(eid As Integer, imieno As String, lattitude As Double, longitude As Double, stype As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetDeviceDetail?eid={eid}&imieno={imieno}&stype={stype}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetDeviceDetail(eid As Integer, imieno As String, stype As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/BookCab?eid={eid}&imieno={imieno}&ratetype={ratetype}&pickup={pickup}&drop={drop}&pickuptime={pickuptime}&susermobile={usermobile}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function BookCab(eid As Integer, imieno As String, ratetype As String, pickup As String, drop As String, pickuptime As String, usermobile As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetNearestSites?IMIENO={IMIENO}&radius={radius}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetNearestSites(IMIENO As String, radius As Integer, Eid As Integer) As String

    <OperationContract()> _
     <WebGet(UriTemplate:="/GetSiteInfo?ID={ID}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetSiteInfo(ID As Integer, Eid As Integer) As String


    <OperationContract()> _
    <WebGet(UriTemplate:="/GetVehicleInfo?IMIENO={IMIENO}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetVehicleInfo(IMIENO As String, Eid As Integer) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetVehicles", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetVehicles() As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetCircles?Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetCircles(Eid As Integer) As String

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetCircleVehicles?Id={Id}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetCircleVehicles(Id As Integer) As String

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetSitesFromLatLong?Lat={Lat}&Longt={Longt}&radius={radius}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetSitesFromLatLong(Lat As Double, Longt As Double, radius As Integer) As String

    <OperationContract()> _
      <WebGet(UriTemplate:="/GetUserVehicles?Eid={Eid}&Uid={Uid}&CircleId={CircleId}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetUserVehicles(Eid As Integer, Uid As Integer, CircleId As Integer) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/NearestSite?Lat={Lat}&Longt={Longt}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function NearestSite(Lat As Double, Longt As Double, Eid As Integer) As String

    '<OperationContract()> _
    '<WebGet(UriTemplate:="/GetGPSRawData?Skey={Skey}&Eid={Eid}&sDate={sDate}&eDate={eDate}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    'Function GetGPSRawData(Skey As String, Eid As Integer, sDate As String, eDate As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/GetGPSRawData?Skey={Skey}&Eid={Eid}&sDate={sDate}&eDate={eDate}&IMEI={IMEI}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetGPSRawData(Skey As String, Eid As Integer, sDate As String, eDate As String, IMEI As String) As String


    <OperationContract()> _
<WebGet(UriTemplate:="/GetEmergencyServices?IMEI={IMEI}&Latt={Latt}&Longt={Longt}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetEmergencyServices(IMEI As String, Latt As Double, Longt As Double) As XElement

    <OperationContract()> _
<WebGet(UriTemplate:="/GetOffers?IMEI={IMEI}&Latt={Latt}&Longt={Longt}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetOffers(IMEI As String, Latt As Double, Longt As Double) As XElement


    <OperationContract()> _
<WebGet(UriTemplate:="/MerchentPOCLogin?Uid={Uid}&Password={Password}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MerchentPOCLogin(Uid As String, Password As String) As XElement

    <OperationContract()> _
<WebGet(UriTemplate:="/SaveGCM?UserKey={UserKey}&IMEI={IMEI}&MerchentID={MerchentID}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SaveGCM(UserKey As String, IMEI As String, MerchentID As Integer) As String

    <OperationContract()> _
<WebGet(UriTemplate:="/SendGcmNotification?MerchentID={MerchentID}&Radius={Radius}&offer={offer}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function SendGcmNotification(MerchentID As Integer, Radius As Integer, offer As String) As String

    <OperationContract()> _
 <WebGet(UriTemplate:="/GetUsers?MerchentID={MerchentID}&Radius={Radius}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetUsers(MerchentID As Integer, Radius As Integer) As XElement

    <OperationContract()> _
  <WebGet(UriTemplate:="/googleCloudID?eid={eid}&googleid={googleid}&userid={userid}&userrole={userrole}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function googleCloudID(ByVal eid As Integer, ByVal googleid As String, ByVal userid As Integer, ByVal userrole As String) As String

    <OperationContract()> _
        <WebGet(UriTemplate:="/ExecuteScheduleDocument", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function ExecuteScheduleDocument() As String

    <OperationContract()> _
       <WebGet(UriTemplate:="/InsertGPSTransaction?Apikey={Apikey}&UserID={UserID}&SiteID={SiteID}&lattitude={lattitude}&longitude={longitude}&SubmitTime={SubmitTime}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function InsertGPSTransaction(Apikey As String, UserID As String, SiteID As String, lattitude As String, longitude As String, SubmitTime As String) As String

    <OperationContract()> _
<WebGet(UriTemplate:="/VehicleDataInfo?IMEINO={IMEINO}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function VehicleDataInfo(ByVal IMEINO As String) As XElement

    ' for hcl by sp - Apr_2017
    <OperationContract()> _
   <WebGet(UriTemplate:="/getPearlApprovalPendency", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function getPearlApprovalPendency() As List(Of Hcl_apprvl_pending)

End Interface
