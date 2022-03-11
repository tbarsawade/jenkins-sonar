Imports System.Data
Imports System.Data.SqlClient
Partial Class Default3NOKIAGOOGLE
    Inherits System.Web.UI.Page
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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da1 As New SqlDataAdapter("select distinct IMIENO from [DMS].[MMM_MST_GPSDATA]", con)
        Dim ds As New DataSet()
        da1.Fill(ds)

        If Not IsPostBack Then

            For Each dr As DataRow In ds.Tables(0).Rows

                DropDownList1.Items.Add(dr(0).ToString())


            Next
        End If

        con.Dispose()
        da1.Dispose()
        ds.Dispose()
    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim ds As New DataSet()

        Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude) order by ctime "
        oda.SelectCommand.CommandText = r
        oda.Fill(ds, "table1")
        oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude) "
        oda.Fill(ds, "speedtable")


        oda.SelectCommand.CommandText = "select sum(DistAnceTravel) as DistAnceTravel ,sum(DevDist) as DevDist ,sum(DistAnother) as DistAnother from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude) "
        oda.Fill(ds, "table3")

        Label1.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
        Label2.Text = ds.Tables("table3").Rows(0).Item("DistAnceTravel").ToString

        Dim centerUser As String = String.Empty
        Dim JLineCoOrdinate As String = ""
        Dim icon As String = ""
        If ds.Tables("table1").Rows.Count = 0 Then
            Label1.Text = "I have no co-ordinate"
            Exit Sub
        Else
            Label1.Text = ""
        End If
        Dim JLinepr As String = ""

        For i As Integer = 0 To ds.Tables("table1").Rows.Count - 1

            JLineCoOrdinate = JLineCoOrdinate & "['Speed " & ds.Tables("table1").Rows(i).Item("speed").ToString() & " Km / Hr <br>RecordTime " + ds.Tables("table1").Rows(i).Item("recordTime").ToString() + "<br>Ctime " + ds.Tables("table1").Rows(i).Item("ctime").ToString() + " '," & ds.Tables("table1").Rows(i).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(i).Item("longitude").ToString() & ",],"



            JLinepr = JLinepr + ds.Tables("table1").Rows(i).Item("lattitude").ToString() + "," + ds.Tables("table1").Rows(i).Item("longitude").ToString() + ","
        Next

        JLinepr = JLinepr.Remove(JLinepr.Length - 1)

        JLineCoOrdinate = JLineCoOrdinate.Remove(JLineCoOrdinate.Length - 1)
        Dim cLocation As Integer = ds.Tables("table1").Rows.Count / 2
        Dim linkr As HtmlGenericControl = New HtmlGenericControl("script")

        If MAPTYPE.SelectedItem.Text = "NOKIA" Then
            linkr.Attributes.Add("type", "text/javascript")
            linkr.Attributes.Add("src", "http://js.cit.api.here.com/se/2.5.3/jsl.js?with=all")
            Page.Header.Controls.Add(linkr)
            Dim JQMAP As String = "<script>nokia.Settings.set('app_id', 'DemoAppId01082013GAL'); nokia.Settings.set('app_code', 'AJKnXv84fjrb0KIHawS0Tg'); nokia.Settings.set('serviceMode', 'cit');  var r1 = [ " + JLineCoOrdinate + " ];  var map = new nokia.maps.map.Display(document.getElementById('mapshowr'), {components: [new nokia.maps.map.component.Behavior(),new nokia.maps.map.component.ZoomBar(),new nokia.maps.map.component.Overview(),new nokia.maps.map.component.TypeSelector(),new nokia.maps.map.component.ScaleBar()],zoomLevel: 12,center: [" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "]});   var infoBubbles = new nokia.maps.map.component.InfoBubbles();  map.components.add(infoBubbles); var TOUCH = nokia.maps.dom.Page.browser.touch,CLICK = TOUCH ? 'tap' : 'click';container = new nokia.maps.map.Container();  container.addListener(CLICK, function (evt) { infoBubbles.openBubble(evt.target.html, evt.target.coordinate); }, false);  var marker, i; for (i = 0; i < r1.length; i++) { marker = new nokia.maps.map.StandardMarker([r1[i][1], r1[i][2]], { html: r1[i][0] });     map.objects.add(container);  container.objects.add(marker);  } var routerr = [" + JLinepr + " ]; var route = new nokia.maps.map.Polyline(new nokia.maps.geo.Strip( routerr  ),  { color: '#00F6', width: 4 }),	sceneContainer = new nokia.maps.map.Container([route]);map.addListener('displayready', function () {  map.objects.add(sceneContainer); map.zoomTo(sceneContainer.getBoundingBox()); }); </script>"

            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)

        ElseIf MAPTYPE.SelectedItem.Text = "GOOGLE" Then
            linkr.Attributes.Add("type", "text/javascript")
            linkr.Attributes.Add("src", "http://maps.google.com/maps/api/js?sensor=false")
            Page.Header.Controls.Add(linkr)
            Dim JQMAP As String = " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('mapshowr'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'car.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
        End If

        oda.Dispose()
        ds.Dispose()
        con.Dispose()
        'Dim inii As String
    End Sub
End Class
