Imports System.ServiceModel
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Data
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "ILogbook" in both code and config file together.
<ServiceContract()>
Public Interface ILogbook
    ' added optional parameter for user type report by Pallavi on 24 feb 15
    <OperationContract()> _
   <WebGet(UriTemplate:="/GetlogBook?sdate={sdate}&tdate={tdate}&UID={UID}&UROLE={UROLE}&IsUserRpt={IsUserRpt}", Bodystyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json)> _
    Function GetlogBook(sdate As String, tdate As String, UID As Integer, UROLE As String, Optional ByVal IsUserRpt As Boolean = False) As Demo
    '{"sdate":"", "tdate":"","UID":"5897","UROLE":"SU"}
    '?UserID={UserID}&Password={Password}&ECode={ECode}

 

End Interface
