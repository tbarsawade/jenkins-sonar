Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
' NOTE: You can use the "Rename" command on the context menu to change the interface name "IDMSNewService" in both code and config file together.
<ServiceContract()>
Public Interface IDMSNewService

    <OperationContract()> _
<WebGet(UriTemplate:="/MissedCall?who={who}&ChannelID={ChannelID}&Circle={Circle}&Operator={Optr}&QualityScore={QualityScore}&DateTime={DateTime}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MissedCall(who As String, ChannelID As String, Circle As String, Optr As String, QualityScore As String, DateTime As String) As String

    <OperationContract()> _
<WebGet(UriTemplate:="/GetReport?uid={uid}&eid={eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function GetReport(UID As Integer, EID As Integer) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/MissedCallDoc?skey={skey}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MissedCallDoc(skey As String) As String

    <OperationContract()> _
    <WebGet(UriTemplate:="/MailAlertGeofenceBased", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Xml)> _
    Function MailAlertGeofenceBased() As String

    <OperationContract()> _
<WebGet(UriTemplate:="/GetSiteData?skey={skey}&eid={eid}&ltlng={ltlng}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetSiteData(Skey As String, Eid As Integer, ltlng As String) As String

End Interface
