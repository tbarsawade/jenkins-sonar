Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Partial Class GpsMapViewer
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        ' If Not IsPostBack Then
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'Dim IMIENO As String = Request.QueryString("IMIE").ToString()


        'Dim tid As String = Request.QueryString("tid").ToString()

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        Dim sDate As String = String.Empty
        Dim eDate As String = String.Empty
        'Dim tid As String = Request.QueryString("tid").ToString()
        Dim IMIENO As String = Request.QueryString("IMIE").ToString()
        sDate = DateTime.Parse(Request.QueryString("Start").ToString()).ToString("yyyy-MM-dd HH:mm:ss ")
        eDate = DateTime.Parse(Request.QueryString("End").ToString()).ToString("yyyy-MM-dd HH:mm:ss ")
        Dim apikey As String = String.Empty
        apikey = ds.Tables("data").Rows(0).Item("APIkey").ToString
        Tripmap.Visible = True
        'oda.SelectCommand.CommandText = "select * from mmm_mst_gpsdata where IMIENO=" & IMIENO & " and  cTime >= '" + sdate & " 00:00 '  AND ctime <= '" & edate & "23:59' "
        '    oda.Fill(ds, "gpsdata")
        Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" & sDate & "' AND ctime <= '" & eDate & "'  group by lattitude,longitude) order by ctime "
            oda.SelectCommand.CommandText = r
            oda.Fill(ds, "table1")
        oda.SelectCommand.CommandText = "select sum(DevDist)[DevDist],max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" & sDate & "'  AND ctime <= '" & eDate & "'  group by lattitude,longitude)  "
        oda.Fill(ds, "speedtable")

        'TotalDistance.Text = ds.Tables("speedtable").Rows(0).Item("DevDist").ToString
        Dim centerUser As String = String.Empty
        Dim JLineCoOrdinate As String = ""
        If ds.Tables("table1").Rows.Count = 0 Then
            msg.Text = "I have no data for this input"
            Exit Sub
        End If

        'For Each dr As DataRow In ds.Tables("table1").Rows
        '    JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & "],"
        'Next
        Dim icon As String = ""

        ' for image on map
        oda.SelectCommand.CommandText = "select  * from   mmm_mst_gpsdata with (nolock) where imieno='" & IMIENO & "' and ctime>='" & sDate & "'  and ctime<='" & eDate & "' order by ctime"
        oda.SelectCommand.CommandTimeout = 180
        oda.Fill(ds, "tbl")
        Dim devStatus As String = "START"
        Dim newStatus As String = "START"
        Dim FrDate As Date
        Dim ToDate As Date
        Dim dist As Double
        Dim finFDate As Date
        icon = ""
        If ds.Tables("tbl").Rows.Count > 0 Then
            FrDate = ds.Tables("tbl").Rows(0).Item("ctime").ToString()
            finFDate = FrDate.ToString()
            ToDate = ds.Tables("tbl").Rows(0).Item("ctime").ToString()
        End If
        ToDate = ds.Tables("tbl").Rows(0).Item("ctime").ToString()

        Select Case DateDiff(DateInterval.Second, FrDate, ToDate)
            Case 160 To 1000
                If Val(ds.Tables("tbl").Rows(0).Item("speed").ToString()) <> 0 And Val(ds.Tables("tbl").Rows(0).Item("devdist").ToString()) <> 0 And Val(ds.Tables("tbl").Rows(0).Item("igstatus").ToString()) = 0 Then
                    'newStatus = "IGNITION OFF AND MOVING"
                    icon = " images/blue.png"
                Else
                    'newStatus = "IGNITION OFF AND IDLE"
                    icon = " images/RedDot.png"
                End If
            Case 0 To 120
                If Val(ds.Tables("tbl").Rows(0).Item("speed").ToString()) = 0 And Val(ds.Tables("tbl").Rows(0).Item("devdist").ToString()) = 0 And Val(ds.Tables("tbl").Rows(0).Item("igstatus").ToString()) = 1 Then
                    'newStatus = "IGNITION ON AND IDLE"
                    icon = " images/YellowDot.png"
                ElseIf Val(ds.Tables("tbl").Rows(0).Item("speed").ToString()) <> 0 And Val(ds.Tables("tbl").Rows(0).Item("devdist").ToString()) <> 0 And Val(ds.Tables("tbl").Rows(0).Item("igstatus").ToString()) = 0 Then
                    'newStatus = "IGNITION OFF AND MOVING"
                    icon = " images/blue.png"
                ElseIf Val(ds.Tables("tbl").Rows(0).Item("speed").ToString()) <> 0 And Val(ds.Tables("tbl").Rows(0).Item("devdist").ToString()) <> 0 And Val(ds.Tables("tbl").Rows(0).Item("igstatus").ToString()) = 1 Then
                    'newStatus = "IGNITION ON AND MOVING"
                    icon = " images/Greendot.png"
                Else
                    'newStatus = "IGNITION OFF AND IDLE"
                    icon = " images/RedDot.png"
                End If
            Case Else
                'newStatus = "IGNITION OFF AND IDLE"
                icon = " images/RedDot.png"
        End Select


        For i As Integer = 1 To ds.Tables("tbl").Rows.Count - 1
            JLineCoOrdinate = JLineCoOrdinate & "['Speed " & ds.Tables("tbl").Rows(i).Item("speed").ToString() & " Km / Hr <br>RecordTime " + ds.Tables("tbl").Rows(i).Item("recordTime").ToString() + "<br>Ctime " + ds.Tables("tbl").Rows(i).Item("ctime").ToString() + " '," & ds.Tables("tbl").Rows(i).Item("lattitude").ToString() & "," & ds.Tables("tbl").Rows(i).Item("longitude").ToString() & ",'" + icon + "'],"
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
        Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:'" + icon + "'  , size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
        ' End If
        'Else
        '    nomap.Visible = True
        'End If

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
End Class
