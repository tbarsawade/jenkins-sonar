Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class OCRFaildInvoice
    Inherits System.Web.Services.WebService
    Dim objDC As New DataClass
    Dim _FileName As String = ""
    Dim _CancellatonReason As String = ""
    <WebMethod()>
    Public Function OCRFaildInvoice(ByVal FILENAME As String, ByVal CANCELLATIONNREASON As String) As XmlDocument
        Dim xmlNewDocRead As New XmlDocument()
        Dim errorMsg As New StringBuilder()
        errorMsg.Append("<RESULT>")
        _FileName = FILENAME
        _CancellatonReason = CANCELLATIONNREASON
        OCRFormValidaton(errorMsg, xmlNewDocRead)
        Return xmlNewDocRead
    End Function

    Private Function OCRFormValidaton(ByRef errorMsg As StringBuilder, ByRef xmlNewDocRead As XmlDocument) As String
        Dim result As String = ""
        Dim ht As New Hashtable()
        Dim objDT As New DataTable()
        Try
            ht.Add("@FileName", _FileName)
            ht.Add("@CancellationRemarks", _CancellatonReason)
            errorMsg.Append(Convert.ToString(objDC.ExecuteProDT("OCRCancelled", ht).Rows(0)(0)))
            errorMsg.Append("</RESULT>")
            xmlNewDocRead.LoadXml(errorMsg.ToString())
            Return result
        Catch ex As Exception
            errorMsg.Append("<MESSAGE> There were some Error please contact admin.</MESSAGE>")
            errorMsg.Append("</RESULT>")
            xmlNewDocRead.LoadXml(errorMsg.ToString())
            Return result
        End Try
    End Function

End Class