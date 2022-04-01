Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
'Imports Tasks
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Delegate
Imports System.Web.Script.Serialization
Imports System.Security.Authentication

Imports MailBee
Imports MailBee.Mime
Imports MailBee.ImapMail
Imports MailBee.Pop3Mail

Partial Class MyEpayWStest
    Inherits System.Web.UI.Page
    'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
    
        End If
    End Sub
    'Add Theme Code
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try

    End Sub
    Public Sub RunWebservice()

        Dim xmlDocSend As New XmlDocument
        xmlDocSend.LoadXml(txtInputXml.Text)
        Dim xmlStr As String = xmlDocSend.InnerXml

        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf ValidateRemoteCertificate)

        Dim sURL As String

        '    sURL = "http://localhost:26656/MYNDSAAS.com LIVE for Dev/BPMCustomWS.svc/INWARD"
        ' sURL = "http://myndsaas.com/BPMCustomWS.svc/MASTERSYNC"
        sURL = txtUrl.Text

        ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

        'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
        'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
        'ServicePointManager.SecurityProtocol = Tls12

        '' temp remove after testing
        Dim request As HttpWebRequest = HttpWebRequest.Create(sURL)
        Dim encoding As New ASCIIEncoding()
        Dim strResult As String = ""


        ' convert xmlstring to byte using ascii encoding
        ' Dim data As Byte() = encoding.GetBytes(sURL)
        'Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(sURL)), HttpWebRequest)
        Dim data As Byte() = encoding.GetBytes(xmlStr)
        Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(sURL)), HttpWebRequest)

        webrequest__1.Method = "POST"
        ' set content type
        webrequest__1.ContentType = "application/x-www-form-urlencoded"
        ' set content length
        webrequest__1.ContentLength = data.Length
        webrequest__1.Timeout = 1000 * 60 * 5
        ' get stream data out of webrequest object
        Dim newStream As Stream = webrequest__1.GetRequestStream()
        newStream.Write(data, 0, data.Length)
        newStream.Close()
        ' declare & read response from service
        Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)
        Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")

        ' read response stream from response object
        Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)

        strResult = loResponseStream.ReadToEnd()
        loResponseStream.Close()
        ' close the response object
        webresponse.Close()

        txtOutputXml.Text = strResult  'xmlDocReceived.InnerXml
        lblMsg.Text = "Run successful"
        ' Response.Close()
        'da.Dispose()
        'dtUrl.Dispose()
        'con.Dispose()
    End Sub

    
    Private Shared Function ValidateRemoteCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal policyErrors As SslPolicyErrors) As Boolean
        If Convert.ToBoolean(ConfigurationManager.AppSettings("IgnoreSslErrors")) Then
            ' allow any old dodgy certificate...
            Return True
        Else
            Return policyErrors = SslPolicyErrors.None
        End If
    End Function


    Protected Sub btnClear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClear.Click
        txtInputXml.Text = ""
        txtOutputXml.Text = ""
        txtUrl.Text = ""
        lblMsg.Text = ""
        txtInputXml.Focus()
    End Sub


    Protected Sub btnShowGrid_Click(sender As Object, e As EventArgs) Handles btnShowGrid.Click
        RunWebservice()
    End Sub
End Class
