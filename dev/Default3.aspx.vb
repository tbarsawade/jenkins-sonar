Imports System.Data.SqlClient

Imports System.Data

Imports System.Configuration
Partial Class Default3
    Inherits System.Web.UI.Page

    'Private Property DropDownList1 As Object
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
        Dim conStr As [String] = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da1 As New SqlDataAdapter("select distinct IMIENO from [DMS].[MMM_MST_GPSDATA]", con)
        Dim ds As New DataSet()
        da1.Fill(ds)

        If Not IsPostBack Then

            For Each dr As DataRow In ds.Tables(0).Rows

                DropDownList1.Items.Add(dr(0).ToString())


            Next
        End If
    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)


        'Dim da1 As New SqlDataAdapter("select * from [DMS].[MMM_MST_GPSDATA] where IMIENO='" & DropDownList1.SelectedItem.Value & "'", con)
        Dim da1 As New SqlDataAdapter("select lattitude,longitude,ctime,speed,distancetravel from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and cast (floor(cast (cTime as float)) as datetime)  >= '" + Date1.Text + "' AND cast (floor(cast (ctime as float)) as datetime) <= '" + Date2.Text + "' group by lattitude,longitude) order by ctime  ", con)

      
        Dim ds1 As New DataSet()
        Dim centerUser As String

        da1.Fill(ds1)
        'If ds1.Tables(0).Rows.Count >= 1 Then

        '    centerUser = ds1.Tables(0).Rows(0).ItemArray(3) + "," + ds1.Tables(0).Rows(0).ItemArray(3)

        'End If
        Dim JLineCoOrdinate As String = ""

        '  string JLineCoOrdinate = "new google.maps.LatLng(28.438954300000000000, 77.040254900000040000), new google.maps.LatLng(28.5650, 77.2100),  new google.maps.LatLng(28.673792100000000000, 77.134372100000060000),";

        For Each dr As DataRow In ds1.Tables(0).Rows
            JLineCoOrdinate = JLineCoOrdinate & "new google.maps.LatLng(" & dr("lattitude").ToString() & "," & dr("longitude").ToString() & "),"
        Next

        'Dim JQMAP As String = "<script>var line;function initialize() { var mapOptions = {center: new google.maps.LatLng(28.5650, 77.2100), zoom: 10, mapTypeId: google.maps.MapTypeId.TERRAIN };var map = new google.maps.Map(document.getElementById('map-canvas'),  mapOptions); var lineCoordinates = [ " & JLineCoOrdinate & "]; var lineSymbol = {    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW, scale: 6,   strokeColor: 'black'   };  line = new google.maps.Polyline({  path: lineCoordinates,  icons: [{ icon: lineSymbol, offset: '100%'  }],    map: map }); animateCircle();  }  var Tr; var count = 0;function animateCircle() { Tr = window.setInterval(function () {  count = (count + 1) % 200; animateCircle1(); var icons = line.get('icons');   icons[0].offset = (count / 2) + '%';  line.set('icons', icons);  }, 20); }  function animateCircle1() {if (count == 199) {  window.clearInterval(Tr);     }      }google.maps.event.addDomListener(window, 'load', initialize); function ReSet() { if (count == 199) {  count = 0;    animateCircle();  }  }</script>"
        Dim JQMAP As String = "<script>var line;function initialize() { var mapOptions = {center: new google.maps.LatLng(28.5650, 77.2100), zoom: 10, mapTypeId: google.maps.MapTypeId.TERRAIN };var map = new google.maps.Map(document.getElementById('map-canvas'),  mapOptions); var lineCoordinates = [ " & JLineCoOrdinate & "]; var lineSymbol = {    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW, scale: 6,   strokeColor: 'blue'   };  line = new google.maps.Polyline({  path: lineCoordinates,  icons: [{ icon: lineSymbol, offset: '100%'  }],    map: map }); animateCircle();  }  var Tr; var count = 0;function animateCircle() { Tr = window.setInterval(function () {  count = (count + 1) % 200; animateCircle1(); var icons = line.get('icons');   icons[0].offset = (count / 2) + '%';  line.set('icons', icons);  }, 20); }  function animateCircle1() {if (count == 199) {  window.clearInterval(Tr);     }      }google.maps.event.addDomListener(window, 'load', initialize); function ReSet() { if (count == 199) {  count = 0;    animateCircle();  }  }</script>"
        ' Dim JQMAP As String = "<script>var line;function initialize() { var mapOptions = {center: new google.maps.LatLng(" + centerUser + "), zoom: 10, mapTypeId: google.maps.MapTypeId.TERRAIN };var map = new google.maps.Map(document.getElementById('map-canvas'),  mapOptions); var lineCoordinates = [ " & JLineCoOrdinate & "]; var lineSymbol = {    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW, scale: 6,   strokeColor: 'blue'   };  line = new google.maps.Polyline({  path: lineCoordinates,  icons: [{ icon: lineSymbol, offset: '100%'  }],    map: map }); animateCircle();  }  var Tr; var count = 0;function animateCircle() { Tr = window.setInterval(function () {  count = (count + 1) % 200; animateCircle1(); var icons = line.get('icons');   icons[0].offset = (count / 2) + '%';  line.set('icons', icons);  }, 20); }  function animateCircle1() {if (count == 199) {  window.clearInterval(Tr);     }      }google.maps.event.addDomListener(window, 'load', initialize); function ReSet() { if (count == 199) {  count = 0;    animateCircle();  }  }</script>"


        '   string JQMAP = "<script> var line; function initialize() { var mapOptions = { center: new google.maps.LatLng(28.5650, 77.2100), zoom: 10,    mapTypeId: google.maps.MapTypeId.TERRAIN   }; var map = new google.maps.Map(document.getElementById('map-canvas'),mapOptions); var lineCoordinates = [  " + JLineCoOrdinate + "]; var lineSymbol = {  path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,scale: 6, strokeColor: 'black' };    line = new google.maps.Polyline({ path: lineCoordinates,  icons: [{ icon: lineSymbol,  offset: '100%'   }],  map: map  }); animateCircle(); } var Tr;  var count = 0; function animateCircle() {Tr = window.setInterval(function () { count = (count + 1) % 200; animateCircle1(); var icons = line.get('icons');icons[0].offset = (count / 2) + '%';line.set('icons', icons);}, 20); } function animateCircle1() {  if (count == 199) {    window.clearInterval(Tr);  }} google.maps.event.addDomListener(window, 'load', initialize);  function ReSet() { if (count == 199) { count = 0; animateCircle(); }  }</script>";
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)

        ' Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)

    End Sub
End Class
