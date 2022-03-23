Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IBPMMobile" in both code and config file together.
<ServiceContract()>
Public Interface IBPMCustomWS
    
    '<OperationContract> _
    '<WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/InwardPAL")> _
    'Function InwardPAL(Data As Stream) As String

    '<OperationContract> _
    '<WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/OutwardPAL")> _
    'Function OutwardPAL(Data As Stream) As ENVELOPE

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARD")> _
    Function INWARD(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARDBULK")> _
    Function INWARDBULK(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/OUTWARD")> _
    Function OUTWARD(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/OUTWARDWM")> _
    Function OUTWARDWM(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARDCANCEL")> _
    Function INWARDCANCEL(Data As Stream) As XElement

    <OperationContract> _
   <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/REGISTER")> _
    Function REGISTER(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/CONFIRMATION")> _
    Function CONFIRMATION(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/UPDATIONFLAG")> _
    Function UPDATIONFLAG(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARDEDIT")> _
    Function INWARDEDIT(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/MASTERSYNC")> _
    Function MASTERSYNC(Data As Stream) As XElement

	 <OperationContract> _
       <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/OUTWARDBULK")> _
    Function OUTWARDBULK(Data As Stream) As XElement

    <OperationContract> _
       <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARDBULKMASTER")> _
    Function INWARDBULKMASTER(Data As Stream) As XElement

    <OperationContract> _
       <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARDBULKALL")> _
    Function INWARDBULKALL(Data As Stream) As XElement


    '<OperationContract>
    '   <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="/GETEIDDATA")>
    'Function GETEIDDATA(Data As EIDDETAIL) As List(Of EIDRESPONSE)

End Interface
