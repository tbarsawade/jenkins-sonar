Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Drawing

Partial Class ShowMapIndus
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ' If Not IsPostBack Then
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Udtype As String = String.Empty
        Dim Ufld As String = String.Empty
        Dim UVfld As String = String.Empty
        Dim Vdtype As String = String.Empty
        Dim Vfld As String = String.Empty
        Dim vemei As String = String.Empty
        Dim apikey As String = String.Empty

        'Dim IMIENO As String = Request.QueryString("IMIE").ToString()
        Dim sDate As String = ""
        Dim eDate As String = ""
        'Dim tid As String = Request.QueryString("tid").ToString()

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
            Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
            UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
            Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
            Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
            vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
            apikey = ds.Tables("data").Rows(0).Item("APIkey").ToString
        End If

        Dim tid As String = Request.QueryString("tid").ToString()
        Dim IMIENO As String = String.Empty
        Dim vehicle As String = String.Empty

        'Dim sDate As String = String.Empty
        'Dim eDate As String = String.Empty
        'Dim Trip_Start_Location As String = String.Empty
        'Dim Trip_End_Location As String = String.Empty
        'Dim Total_Distance As String = String.Empty
        Dim uid As String = String.Empty

        If Request.QueryString("type").ToString().ToUpper <> "MANUAL" Then
            Tripmap.Visible = True

            oda.SelectCommand.CommandText = "select * from mmm_mst_gpsdata where tid=" & tid & " "
            oda.Fill(ds, "gpsdata")
            If ds.Tables("gpsdata").Rows.Count > 0 Then
                IMIENO = ds.Tables("gpsdata").Rows(0).Item("IMIENO").ToString
                'vehicle = ds.Tables("elogbook").Rows(0).Item("vehicle_no").ToString
                'Trip_Start_Location = ds.Tables("elogbook").Rows(0).Item("Trip_Start_Location").ToString
                'Trip_End_Location = ds.Tables("elogbook").Rows(0).Item("Trip_End_Location").ToString
                'Total_Distance = ds.Tables("elogbook").Rows(0).Item("Total_Distance").ToString
                'uid = ds.Tables("elogbook").Rows(0).Item("uid").ToString
                sDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString).ToString("yyyy-MM-dd ")

                eDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString()).ToString("yyyy-MM-dd ")
            End If
            'VehicleNo.Text = vehicle
            'TripStartLocation.Text = Trip_Start_Location
            'TripEndLocation.Text = Trip_End_Location
            'TripStartDateTime.Text = sDate
            'TripEndDateTime.Text = eDate
            'TotalDistance.Text = Total_Distance
            'oda.SelectCommand.CommandText = "select * from mmm_mst_user where uid=" & uid & " "
            'oda.Fill(ds, "Username")
            'User.Text = ds.Tables("Username").Rows(0).Item("UserName")

            Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "00:00' AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude) order by ctime "
            oda.SelectCommand.CommandText = r
            oda.Fill(ds, "table1")

            oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & " 00:00 '  AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude)  "
            oda.Fill(ds, "speedtable")
            MaximumSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
            Dim centerUser As String = String.Empty
            Dim JLineCoOrdinate As String = ""

            If ds.Tables("table1").Rows.Count = 0 Then
                msg.Text = "I have no data for this trip"
                Exit Sub
            End If

            Dim drPrev As DataRow
            Dim c = 0
            For Each dr As DataRow In ds.Tables("table1").Rows
                Dim icon As String
                If c = 0 Then
                    icon = "images/start1.png"
                ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                    icon = "images/end1.png"
                Else
                    'Dim lat1 As Double = Convert.ToDouble(drPrev("lattitude"))
                    'Dim long1 As Double = Convert.ToDouble(drPrev("longitude"))
                    'Dim lat2 As Double = Convert.ToDouble(dr("lattitude"))
                    'Dim long2 As Double = Convert.ToDouble(dr("longitude"))

                    'Dim dir = GetAngle(lat1, long1, lat2, long2)

                    'If dir = 0 Then
                    '    icon = "images/dir0.png"
                    'ElseIf dir > 0 And dir <= 45 Then
                    '    icon = "images/dir45.png"
                    'ElseIf dir > 45 And dir <= 90 Then
                    '    icon = "images/dir90.png"
                    'ElseIf dir > 90 And dir <= 135 Then
                    '    icon = "images/dir135.png"
                    'ElseIf dir > 135 And dir <= 180 Then
                    '    icon = "images/dir180.png"
                    'ElseIf dir > 180 And dir <= 225 Then
                    '    icon = "images/dir225.png"
                    'ElseIf dir > 225 And dir <= 270 Then
                    '    icon = "images/dir270.png"
                    'ElseIf dir > 270 And dir < 360 Then
                    '    icon = "images/dir315.png"
                    'Else
                    '    icon = "images/Greendot.png"
                    'End If

                    If Convert.ToDouble(dr("speed")) <= 0 Then
                        icon = "images/reddot.png"
                    Else
                        icon = "images/Greendot.png"
                    End If
                    

                End If

                JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ", '" & icon & "'],"
                drPrev = dr
                c += 1
            Next

            JLineCoOrdinate = JLineCoOrdinate.Remove(JLineCoOrdinate.Length - 1)

            Dim url As String
            Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")

            If apikey = "" Then
                jqueryInclude.Attributes.Add("type", "text/javascript")
                jqueryInclude.Attributes.Add("src", "http://maps.google.com/maps/api/js?sensor=false")
                url = ""
                Page.Header.Controls.Add(jqueryInclude)
            Else
                jqueryInclude.Attributes.Add("type", "text/javascript")
                jqueryInclude.Attributes.Add("src", "http://www.google.com/jsapi?key=" + apikey + "")
                Page.Header.Controls.Add(jqueryInclude)
                url = "<script type='text/javascript'>google.load('maps', '3.7', { 'other_params': 'sensor=true' }); </script>"
            End If
            'to get central location divide it by two
            Dim cLocation As Integer = ds.Tables("table1").Rows.Count / 2
            Dim clat = ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString()
            Dim clong = ds.Tables("table1").Rows(cLocation).Item("longitude").ToString()

            Dim JQMAP As String = " " + url + " <script> var eid=" & Session("EID") & ";  var locations=[ " + JLineCoOrdinate + " ]; ; var map;  var lineCoordinates = []; var clat=" & clat & ";  var clong=" & clong & "; Markers.initialize(); </script>"
            ' Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/Greendot.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
            ' End If
        Else
            nomap.Visible = True
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

    Public Shared strPathJson As String = HttpContext.Current.Server.MapPath("DOCS/Jasontext.json")

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerListJSON() As String
        Dim strList As String = ""
        Dim d As String = IO.File.ReadAllText(strPathJson)
        Return d
    End Function


    Public Shared Function RadToDeg(radians As Double) As Double
        Return radians * (180 / Math.PI)
    End Function

    Public Shared Function DegToRad(degrees As Double) As Double
        Return degrees * (Math.PI / 180)
    End Function

    

    Public Shared Function ConvertToBearing(deg As Double) As Double
        Return (deg + 360) Mod 360
    End Function

    Private Function GetAngle(lat1 As Double, long1 As Double, lat2 As Double, long2 As Double) As Double
        Dim degreesToRadians = Math.PI / 180
        Dim degreesPerRadian = 180.0 / Math.PI
        lat1 = ShowMapIndus.DegToRad(lat1)
        long1 = ShowMapIndus.DegToRad(long1)
        lat2 = ShowMapIndus.DegToRad(lat2)
        long2 = ShowMapIndus.DegToRad(long2)
        Dim ra = Math.PI / 180
        Dim deg = 180 / Math.PI


        Dim x = lat2 - lat1
        Dim y = long2 - long1
        Dim f = 0
        If x >= 0 And y >= 0 Then
            y = y * ra
            x = x * ra
            f = 90 - Math.Atan(y / x) * deg
        ElseIf x >= 0 And y <= 0 Then
            y = y * ra
            x = x * ra
            f = 90 + Math.Abs(Math.Atan(y / x) * deg)
        ElseIf x <= 0 And y <= 0 Then
            y = y * ra
            x = x * ra
            f = 270 - Math.Atan(y / x) * deg
        ElseIf x <= 0 And y >= 0 Then
            y = y * ra
            x = x * ra
            f = 270 + Math.Abs(Math.Atan(y / x) * deg)
        End If
        Return f
    End Function


End Class
