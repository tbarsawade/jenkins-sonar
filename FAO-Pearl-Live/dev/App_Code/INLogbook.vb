Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "ILogbook" in both code and config file together.
<ServiceContract()>
Public Interface INLogbook

    <OperationContract()> _
   <WebGet(UriTemplate:="/GetlogBook?sdate={sdate}&tdate={tdate}&UID={UID}&UROLE={UROLE}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetlogBook(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Demo1

    <OperationContract()> _
  <WebGet(UriTemplate:="/GetOverSpeed?sdate={sdate}&tdate={tdate}&UID={UID}&UROLE={UROLE}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetOverSpeed(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Demo1
    '{"sdate":"", "tdate":"","UID":"5897","UROLE":"SU"}
    '?UserID={UserID}&Password={Password}&ECode={ECode}
    <OperationContract()> _
<WebGet(UriTemplate:="/GetUnderSpeed?sdate={sdate}&tdate={tdate}&UID={UID}&UROLE={UROLE}&Eid={Eid}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetUnderSpeed(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Demo1

End Interface
