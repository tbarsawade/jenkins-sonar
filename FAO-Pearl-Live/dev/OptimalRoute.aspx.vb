Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.Web.Services


Partial Class OptimalRoute
    Inherits System.Web.UI.Page

    <WebMethod()> _
   <Script.Services.ScriptMethod()> _
    Public Shared Function RouteInfo(lat1 As String, long1 As String, lat2 As String, long2 As String, Vehicle As String, mode As String, traffic As String) As String
        Dim WayPoint As New List(Of String)
        ' Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=As1AGESV4Qio_HDgNw9U&app_code=N4_fgYJCwnzTPt2MGmRS8A&gen=3&searchtext=" + TextBox2.Text + ""
        Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=As1AGESV4Qio_HDgNw9U&app_code=N4_fgYJCwnzTPt2MGmRS8A&routeattributes=shape&maneuverattributes=direction,shape&jsonAttributes=1&waypoint0=" + lat1 + "," + long1 + " &waypoint1=" + lat2 + "," + long2 + "&language=en-US&mode0=" + mode + ";" + Vehicle + ";traffic:" + traffic + ""
        'Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&waypoint0=geo!" + Long1 + "," + Latt1 + " &waypoint1=geo!" + Long2 + "," + Latt2 + "&mode=shortest;car;traffic:default;tollroad:-1"
        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Dim response As WebResponse = request.GetResponse()
        Dim dataStream As Stream = response.GetResponseStream()
        Dim sreader As New StreamReader(dataStream)
        Dim responsereader As String = sreader.ReadToEnd()
        response.Close()
        Dim xmldoc As New XmlDocument()
        xmldoc.LoadXml(responsereader)
        'namespaces.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/")
        'Dim xPathString = "/SOAP-ENV:Envelope/SOAP-ENV:Body/SOAP-ENV:Fault/faultstring"
        If xmldoc.ChildNodes.Count > 0 Then
            Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
            Dim Cnt As Integer = 0
            'Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Waypoint/MappedPosition")
            Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Leg")
            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If node.ChildNodes.Item(c).Name = "Length" Then
                        WayPoint.Insert(0, node.ChildNodes.Item(c).InnerText)
                        ' WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                    End If
                    If node.ChildNodes.Item(c).Name = "TravelTime" Then
                        WayPoint.Insert(1, node.ChildNodes.Item(c).InnerText)
                        'WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                    End If
                Next
            Next
            'Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
            'jqueryInclude.Attributes.Add("type", "text/javascript")
            'Page.Header.Controls.Add(jqueryInclude)
            'nokia.Settings.set("app_id", "DemoAppId01082013GAL")
        End If
        'Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + "," + WayPoint(2) + "," + WayPoint(3) + ");", True)

        Dim dist As Double = Math.Round(Convert.ToDouble(WayPoint(0) / 1000), 2) '/ 1000
        Dim treveltime As Double = Math.Round(Convert.ToDouble(WayPoint(1) / 60), 2) '/ 60

        Return dist.ToString() & "|" & treveltime.ToString()

    End Function

End Class
