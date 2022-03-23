Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IBPMMobile" in both code and config file together.
<ServiceContract()>
Public Interface IBPMTallyInt

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/INWARD")> _
    Function INWARD(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/OUTWARD")> _
    Function OUTWARD(Data As Stream) As XElement


    <OperationContract> _
   <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/REGISTER")> _
    Function REGISTER(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/CONFIRMATION")> _
    Function CONFIRMATION(Data As Stream) As XElement

    <OperationContract> _
    <WebInvoke(Method:="POST", requestformat:=WebMessageFormat.Xml, responseformat:=WebMessageFormat.Xml, UriTemplate:="/UPDATIONFLAG")> _
    Function UPDATIONFLAG(Data As Stream) As XElement

End Interface
