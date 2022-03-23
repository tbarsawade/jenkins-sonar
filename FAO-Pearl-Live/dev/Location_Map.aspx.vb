Imports System.Net.HttpWebRequest
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Partial Class Location_Map
    Inherits System.Web.UI.Page
    Protected WayPoint As New List(Of String)
    Protected DistandTime As New List(Of String)
    Protected RouteWay As String
    Protected Mode As String
    Protected Traffic As String
    Dim Long1 As String
    Dim Latt1 As String
    Dim Long2 As String
    Dim Latt2 As String
    Dim Long3 As String
    Dim Latt4 As String
    Dim Dist As Double


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DropDownList1.Items.Add("fastestNow")
            DropDownList1.Items.Add("fastest")
            DropDownList1.Items.Add("directDrive")
            DropDownList1.Items.Add("scenic")
            DropDownList2.Items.Add("enabled")
            DropDownList2.Items.Add("disabled")
            ' AutoComplete.GetCountries(TextBox1.Text)
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
    <System.Web.Script.Services.ScriptMethod> _
      <System.Web.Services.WebMethod> _
    Public Shared Function GetStartLocation(prefixText As String) As List(Of String)
        Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("dbconnection").ToString())
        con.Open()
        Dim cmd As New SqlCommand("select Trip_Start_Location from MMM_MST_ELOGBOOK where Trip_Start_Location like @Name+'%'", con)
        cmd.Parameters.AddWithValue("@Name", prefixText)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        Dim CountryNames As New List(Of String)()
        For i As Integer = 0 To dt.Rows.Count - 1
            CountryNames.Add(dt.Rows(i)(0).ToString())
        Next

        Return CountryNames
    End Function
    <System.Web.Script.Services.ScriptMethod> _
      <System.Web.Services.WebMethod> _
    Public Shared Function GetDestLocation(prefixText As String) As List(Of String)
        Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("dbconnection").ToString())
        con.Open()
        Dim cmd As New SqlCommand("select Trip_End_Location from MMM_MST_ELOGBOOK where Trip_Start_Location like @Name+'%'", con)
        cmd.Parameters.AddWithValue("@Name", prefixText)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        Dim CountryNames As New List(Of String)()
        For i As Integer = 0 To dt.Rows.Count - 1
            CountryNames.Add(dt.Rows(i)(0).ToString())
        Next
        Return CountryNames
    End Function
    Protected Sub Location1()
        If Not ClientScript.IsStartupScriptRegistered("alert") Then
            Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&gen=3&searchtext=" + TextBox1.Text + ""
            'Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&waypoint0=geo!" + Long1 + "," + Latt1 + " &waypoint1=geo!" + Long2 + "," + Latt2 + "&mode=shortest;car;traffic:default;tollroad:-1"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim sreader As New StreamReader(dataStream)
            Dim responsereader As String = sreader.ReadToEnd()
            response.Close()
            Dim xmldoc As New XmlDocument()
            Dim Lat1 As String = ""
            Dim Lat2 As String = ""
            xmldoc.LoadXml(responsereader)
            'namespaces.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/")
            'Dim xPathString = "/SOAP-ENV:Envelope/SOAP-ENV:Body/SOAP-ENV:Fault/faultstring"
            If xmldoc.ChildNodes.Count > 0 Then
                Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
                Dim Cnt As Integer = 0
                'Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Waypoint/MappedPosition")
                Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Location/DisplayPosition")
                For Each node As XmlNode In nodes
                    For c As Integer = 0 To node.ChildNodes.Count - 1
                        If node.ChildNodes.Item(c).Name = "Latitude" Then
                            WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                        End If
                        If node.ChildNodes.Item(c).Name = "Longitude" Then
                            WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                        End If
                    Next
                Next
                'Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
                'jqueryInclude.Attributes.Add("type", "text/javascript")
                'Page.Header.Controls.Add(jqueryInclude)
                'nokia.Settings.set("app_id", "DemoAppId01082013GAL")
            End If
            'Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + "," + WayPoint(2) + "," + WayPoint(3) + ");", True)
            'Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + ");", True)
        End If
    End Sub
    Protected Sub Location2()
        If Not ClientScript.IsStartupScriptRegistered("alert") Then
            Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&gen=3&searchtext=" + TextBox2.Text + ""
            'Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&waypoint0=geo!" + Long1 + "," + Latt1 + " &waypoint1=geo!" + Long2 + "," + Latt2 + "&mode=shortest;car;traffic:default;tollroad:-1"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim sreader As New StreamReader(dataStream)
            Dim responsereader As String = sreader.ReadToEnd()
            response.Close()
            Dim xmldoc As New XmlDocument()
            Dim Lat1 As String = ""
            Dim Lat2 As String = ""
            xmldoc.LoadXml(responsereader)
            'namespaces.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/")
            'Dim xPathString = "/SOAP-ENV:Envelope/SOAP-ENV:Body/SOAP-ENV:Fault/faultstring"
            If xmldoc.ChildNodes.Count > 0 Then
                Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
                Dim Cnt As Integer = 0
                'Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Waypoint/MappedPosition")
                Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Location/DisplayPosition")
                For Each node As XmlNode In nodes
                    For c As Integer = 0 To node.ChildNodes.Count - 1
                        If node.ChildNodes.Item(c).Name = "Latitude" Then
                            WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                        End If
                        If node.ChildNodes.Item(c).Name = "Longitude" Then
                            WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                        End If
                    Next

                Next
                'Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
                'jqueryInclude.Attributes.Add("type", "text/javascript")
                'Page.Header.Controls.Add(jqueryInclude)
                'nokia.Settings.set("app_id", "DemoAppId01082013GAL")
            End If
            'Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + "," + WayPoint(2) + "," + WayPoint(3) + ");", True)

        End If
    End Sub
    Protected Sub Location3()
        If Not ClientScript.IsStartupScriptRegistered("alert") Then
            Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&gen=3&searchtext=" + TextBox3.Text + ""
            'Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&waypoint0=geo!" + Long1 + "," + Latt1 + " &waypoint1=geo!" + Long2 + "," + Latt2 + "&mode=shortest;car;traffic:default;tollroad:-1"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim sreader As New StreamReader(dataStream)
            Dim responsereader As String = sreader.ReadToEnd()
            response.Close()
            Dim xmldoc As New XmlDocument()
            Dim Lat1 As String = ""
            Dim Lat2 As String = ""
            xmldoc.LoadXml(responsereader)
            'namespaces.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/")
            'Dim xPathString = "/SOAP-ENV:Envelope/SOAP-ENV:Body/SOAP-ENV:Fault/faultstring"
            If xmldoc.ChildNodes.Count > 0 Then
                Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
                Dim Cnt As Integer = 0
                'Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Waypoint/MappedPosition")
                Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Location/DisplayPosition")
                For Each node As XmlNode In nodes
                    For c As Integer = 0 To node.ChildNodes.Count - 1
                        If node.ChildNodes.Item(c).Name = "Latitude" Then
                            WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                        End If
                        If node.ChildNodes.Item(c).Name = "Longitude" Then
                            WayPoint.Add(node.ChildNodes.Item(c).InnerText)
                        End If
                    Next
                Next
                'Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
                'jqueryInclude.Attributes.Add("type", "text/javascript")
                'Page.Header.Controls.Add(jqueryInclude)
                'nokia.Settings.set("app_id", "DemoAppId01082013GAL")
            End If
            'Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + "," + WayPoint(2) + "," + WayPoint(3) + ");", True)

        End If
    End Sub

    Protected Sub btnnewRoute_Click(sender As Object, e As EventArgs) Handles btnnewRoute.Click
        PanelInstruction.Visible = False
        pnldynamically.Visible = True

    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sb As New StringBuilder()
        If TextBox1.Text <> "" And TextBox2.Text <> "" Then
            Dim _Speed As Integer = 30
            'Dim Time As Decimal
            Call Location1()
            Call Location2()
            Call Location3()
            RouteWay = DropDownList1.SelectedItem.Text
            Mode = DropDownList3.SelectedItem.Text
            Traffic= DropDownList2.SelectedItem.Text
            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + "," + WayPoint(2) + "," + WayPoint(3) + "," + WayPoint(4) + "," + WayPoint(5) + ",'" + RouteWay + "','" + Mode + "','" + Traffic + "');", True)
            'Time = DistandTime(1)
            'Label12.Text = Time
        End If
        pnldynamically.Visible = False
        ' Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&gen=3&searchtext=" + TextBox2.Text + ""
        Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&routeattributes=shape&maneuverattributes=direction,shape&jsonAttributes=1&waypoint0=" + WayPoint(0) + "," + WayPoint(1) + " &waypoint1=" + WayPoint(2) + "," + WayPoint(3) + "&waypoint1=" + WayPoint(4) + "," + WayPoint(5) + "&language=en-US&mode0=" + DropDownList1.SelectedItem.Text + ";" + DropDownList3.SelectedItem.Text + ";traffic:" + DropDownList2.SelectedItem.Text + ""
        'Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&routeattributes=shape&maneuverattributes=direction,shape&jsonAttributes=1&waypoint0=28.5003567,77.0855179%20&waypoint1=28.494142,%2077.080883&waypoint2=28.4396782,76.9918365&language=en-US&mode0=fastest;car;traffic:enabled"
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
            Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Maneuver")
            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If node.ChildNodes.Item(c).Name = "Instruction" Then
                        'DistandTime.Add(node.ChildNodes.Item(c).InnerText)
                        sb.Append(node.ChildNodes.Item(c).InnerText)
                        sb.Append(Environment.NewLine)
                        sb.Append(Environment.NewLine)
                    End If
                Next
            Next
            'Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
            'jqueryInclude.Attributes.Add("type", "text/javascript")
            'Page.Header.Controls.Add(jqueryInclude)
            'nokia.Settings.set("app_id", "DemoAppId01082013GAL")
        End If
        Label3.Text = WayPoint(0)      'Source Lat
        Label4.Text = WayPoint(1)       'Source Long
        Label5.Text = WayPoint(2)       'Destination Lat
        Label6.Text = WayPoint(3)       'Destination Long
        Label20.Text = WayPoint(4)       'Third Location Lat
        Label21.Text = WayPoint(5)      'Third Location Long
        distance(WayPoint(0), WayPoint(1), WayPoint(2), WayPoint(3), WayPoint(4), WayPoint(5), "K")
        txtroute.Text = sb.ToString()
        PanelInstruction.Visible = True
    End Sub
    'Public Function distance(lat1 As String, lon1 As String, lat2 As String, lon2 As String, ByVal unit As Char) As Double
    '    Dim theta As Double = lon1 - lon2
    '    Dim dist As Double = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta))
    '    dist = Math.Acos(dist)
    '    dist = rad2deg(dist)
    '    dist = dist * 60 * 1.1515
    '    If unit = "K" Then
    '        dist = dist * 1.609344
    '    ElseIf unit = "N" Then
    '        dist = dist * 0.8684
    '    End If
    '    Return dist
    'End Function

    'Private Function deg2rad(ByVal deg As Double) As Double
    '    Return (deg * Math.PI / 180.0)
    'End Function

    'Private Function rad2deg(ByVal rad As Double) As Double
    '    Return rad / Math.PI * 180.0
    'End Function
    Public Sub distance(lat1 As String, lon1 As String, lat2 As String, lon2 As String, lat3 As String, lon3 As String, ByVal unit As Char)

        ' Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&gen=3&searchtext=" + TextBox2.Text + ""
        Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&routeattributes=shape&maneuverattributes=direction,shape&jsonAttributes=1&waypoint0=" + WayPoint(0) + "," + WayPoint(1) + " &waypoint1=" + WayPoint(2) + "," + WayPoint(3) + "&waypoint1=" + WayPoint(4) + "," + WayPoint(5) + "&language=en-US&mode0=" + DropDownList1.SelectedItem.Text + ";" + DropDownList3.SelectedItem.Text + ";traffic:" + DropDownList2.SelectedItem.Text + ""
        'Dim url As String = "https://route.st.nlp.nokia.com/routing/6.2/calculateroute.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&routeattributes=shape&maneuverattributes=direction,shape&jsonAttributes=1&waypoint0=28.5003567,77.0855179%20&waypoint1=28.494142,%2077.080883&waypoint2=28.4396782,76.9918365&language=en-US&mode0=fastest;car;traffic:enabled"
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
            Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Summary")
            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If node.ChildNodes.Item(c).Name = "Distance" Then
                        'DistandTime.Add(node.ChildNodes.Item(c).InnerText)
                        Label11.Text = (node.ChildNodes.Item(c).InnerText) / 1000
                    End If
                    If DropDownList1.SelectedItem.Text = "disabled" Then
                        If node.ChildNodes.Item(c).Name = "BaseTime" Then
                            'DistandTime.Add(node.ChildNodes.Item(c).InnerText)
                            Label12.Text = (node.ChildNodes.Item(c).InnerText) / 3600
                        End If
                    Else
                        If node.ChildNodes.Item(c).Name = "TrafficTime" Then
                            'DistandTime.Add(node.ChildNodes.Item(c).InnerText)
                            Dim t As TimeSpan = TimeSpan.FromHours(node.ChildNodes.Item(c).InnerText / 3600)
                            Label12.Text = String.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds)
                        End If
                    End If
                   
                Next
            Next
            'Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
            'jqueryInclude.Attributes.Add("type", "text/javascript")
            'Page.Header.Controls.Add(jqueryInclude)
            'nokia.Settings.set("app_id", "DemoAppId01082013GAL")
        End If
        'Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "fnSample(" + WayPoint(0) + "," + WayPoint(1) + "," + WayPoint(2) + "," + WayPoint(3) + ");", True)
        'Dist = WayPoint(4)

    End Sub

    Protected Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        '  AutoComplete.GetCountries(TextBox1.Text)
    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList1.SelectedIndexChanged
        If DropDownList1.SelectedItem.Value = "fastestNow" Then
            DropDownList2.Items.Clear()
            DropDownList2.Items.Add("enabled")
        ElseIf DropDownList1.SelectedItem.Value = "directDrive" Then
            DropDownList2.Items.Clear()
            DropDownList2.Items.Add("disabled")
        Else
            DropDownList2.Items.Clear()
            DropDownList2.Items.Add("enabled")
            DropDownList2.Items.Add("disabled")
        End If
    End Sub
   
    Protected Sub DropDownList3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList3.SelectedIndexChanged
        If DropDownList3.SelectedItem.Value = "car" Then
            DropDownList1.Items.Clear()
            DropDownList2.Items.Clear()
            DropDownList1.Items.Add("fastestNow")
            DropDownList1.Items.Add("fastest")
            DropDownList1.Items.Add("directDrive")
            DropDownList1.Items.Add("scenic")
            If DropDownList1.SelectedItem.Value = "fastestNow" Then
                DropDownList2.Items.Add("enabled")
            Else
                DropDownList2.Items.Add("enabled")
                DropDownList2.Items.Add("disabled")
            End If
        ElseIf DropDownList3.SelectedItem.Value = "pedestrian" Then
            DropDownList1.Items.Clear()
            DropDownList2.Items.Clear()
            DropDownList1.Items.Add("scenic")
            DropDownList1.Items.Add("shortest")
            DropDownList1.Items.Add("fastest")
            DropDownList2.Items.Add("enabled")
            DropDownList2.Items.Add("disabled")
        Else
            DropDownList1.Items.Clear()
            DropDownList2.Items.Clear()
            DropDownList1.Items.Add("fastestNow")
            DropDownList1.Items.Add("fastest")
            DropDownList1.Items.Add("directDrive")
            DropDownList1.Items.Add("scenic")
            If DropDownList1.SelectedItem.Value = "fastestNow" Then
                DropDownList2.Items.Add("enabled")
            Else
                DropDownList2.Items.Add("enabled")
                DropDownList2.Items.Add("disabled")
            End If
        End If
    End Sub
End Class
