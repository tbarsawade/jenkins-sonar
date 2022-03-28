<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="HomeMap, App_Web_20pgc3v0" viewStateEncryptionMode="Always" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <title></title>
    <link href="css/style.css" rel="Stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="/Src%20New/Src%20New/resources/demos/style.css" />
    <link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css" />
    <script src="http://code.jquery.com/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
     <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false"></script>
    <script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>
    <script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>
    <script src="Scripts/NokiaGoogleMap.js" type="text/javascript"></script>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        var btypes = "";

        var stvalue = "";
        var arg = new Array();

        function ViewMap() {
            stvalue = "";
            $("#ContentPlaceHolder1_Panel2").find(':checkbox').each(function () {
                var t1 = $(this).prop('checked');
                if (t1 == true) {
                    if (stvalue == "")
                        stvalue = $(this).val();
                    else
                        stvalue = stvalue + "," + $(this).val();
                }
            });
            var eid = '<%= Session("EID")%>';
            GetMonitor(eid, stvalue);
        }

        $(document).ajaxStart(function () { $("#wait").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#wait").css("display", "none"); });

        function GetMonitor(eid, argbranch) {
            $.ajax({
                type: "post",
                url: "HomeMap.aspx/ConvertDataTabletoString",
                data: JSON.stringify({ 'eid': eid, 'doctype': argbranch }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) { sdm(data); },
                error: function (data) { sdm(data); }
            });
        }

        function sdm(sd) {

            if (sd.d[0] == "") {
                $("#dvMap").html(" ");
            }
            else {
                // CreatePolygonNokias(sd.d);
                initialize_enquirys(sd.d);
            }
        }
        function CreatePolygonNokias(geopoints) {

            var k;

            var temp = 0;


            var arrLat = new Array();

            var arrLong = new Array();

            document.getElementById("dvMap").innerHTML = "";

            var GeoPointslength = geopoints.length;

            var argLatitude = new Array();

            var argLongitude = new Array();

            var x, y;

            for (var ctr = 0; ctr < GeoPointslength; ctr++) {
                var ar = geopoints[ctr].Geopoint.split(",");

                x = parseFloat(ar[0]);

                y = parseFloat(ar[1]);

                if (isNaN(x) || isNaN(y)) {
                    continue;
                }
                else {
                    break;
                }
            }

            nokia.Settings.set("app_id", "VG3IAYYwc1Y7XaBWEqU9");

            nokia.Settings.set("app_code", "R7W2h1KNHKBqJsJOkRbTiw");

            nokia.Settings.set("serviceMode", "cit");

            (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

            var mapContainer = document.getElementById("dvMap");

            var infoBubbles = new nokia.maps.map.component.InfoBubbles();

            var map = new nokia.maps.map.Display(mapContainer, {

                center: [x, y],
                zoomLevel: 4,
                components: [infoBubbles, new nokia.maps.map.component.Behavior()]
            });


            var vlat = new Array();

            var vlon = new Array();

            var marker_icon;

            var count = 0;

            for (var i = 0; i < GeoPointslength; i++) {
                temp = 0;
                var arms = geopoints[i].Geopoint.split(",");
                var gpoint = geopoints[i].Geopoint;

                if (geopoints[i].Site == "Hub") {
                    marker_icon = "images/blue.png";
                }
                else if (geopoints[i].Site == "Strategic") {
                    marker_icon = "images/Reddot.png";
                }
                else {
                    marker_icon = "images/Greendot.png";
                }

                var markersContainer = new nokia.maps.map.Container();

                var y1 = parseFloat(arms[0]);

                var y2 = parseFloat(arms[1]);

                if (isNaN(y1) || isNaN(y2)) {
                    continue;
                }
                else {
                    var imageMarker = new nokia.maps.map.Marker([y1, y2],
                    {
                        icon: marker_icon,
                        dragable: true,
                        position: [y1, y2],
                        anchor: new nokia.maps.util.Point(1, 1),
                        $html: 0000
                    })

                    map.objects.addAll([imageMarker]);

                    map.objects.add(markersContainer);

                    imageMarker.html = "<table width='100%'><tr><td style='width:50%;text-align:left;color:white;font-size:16px;'>Site ID :</td><td style='width:50%;text-align:left;color:white;font-size:16px;'>" + geopoints[i].SiteID + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;font-size:16px;'>Site Name:</td><td style='width:50%;text-align:left;color:white;font-size:16px;'>" + geopoints[i].SiteName + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;font-size:16px;'>Site Address:</td><td style='width:50%;text-align:left;color:white;font-size:16px;'>" + geopoints[i].Address + '</td></tr></table>';

                    var TOUCH = nokia.maps.dom.Page.browser.touch,

                    CLICK = TOUCH ? "tap" : "click";

                    imageMarker.addListener(CLICK, function (evt) {

                        infoBubbles.openBubble(this.html, this.coordinate);

                        tid = (evt.target.$html);

                    });

                }

            }

        }


        function initialize_enquirys(latlongs) {
           
            var latlong;
            var marker_icon;
            document.getElementById("dvMap").innerHTML = "";
            var marker;
            var markers = latlongs;
            MarkerArr.length = markers.length;

            if (markers.length > 0) {
                for (i = 0; i < 1; i++) {
                    latlong = markers[i].Geopoint.split(",");
                    var mapOptions = {
                        center: new google.maps.LatLng(latlong[0], latlong[1]),
                        zoom: 5,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    };
                }
            }
            else {
                var mapOptions = {
                    center: new google.maps.LatLng(28.5808792, 77.2263718),
                    zoom: 5,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
            }

            var infoWindow = new google.maps.InfoWindow();

            var map = new google.maps.Map(document.getElementById("dvMap"), mapOptions);

            for (i = 0; i < markers.length; i++)
            {
                if (markers[i].Site == "Hub") {
                    marker_icon = "images/blue.png";
                }
                else if (markers[i].Site == "Strategic") {
                    marker_icon = "images/Reddot.png";
                }
                else {
                    marker_icon = "images/Greendot.png";
                }

                latlong = markers[i].Geopoint.split(",");
                var myLatlng = new google.maps.LatLng(latlong[0], latlong[1]);
                marker = new google.maps.Marker({
                    icon: marker_icon,
                    position: myLatlng,
                    map: map,
                    draggable: true,
                    // title: data.title,
                    id: markers[i].tid
                });

                marker.html = "Address :" + markers[i].Address + "" + '</br>' + "Site :" + markers[i].Site + "" + '</br>' + "Site ID:" + markers[i].SiteID + "" + '</br>' + "Site Name:" + markers[i].SiteName + "" + '</br>';
                google.maps.event.addListener(marker, 'click', function () {
                    infoWindow.setContent(this.html);
                    infoWindow.open(map, this);
                    x = this.id;

                });
                var modes = [{
                    type: Route,
                    transportModes: [Mode],
                    options: "avoidTollroad",
                    trafficMode: Traffic
                }];


            }



        }
    </script>
<table style="width: 100%; height: 670px;">
            <tr>
                <td style="width: 20%; height: 670px; vertical-align:top;">
                    <div style="width:100%; height:670px;">
                     <asp:Panel ID="Panel2" runat="server" Height="670px" 
                                        Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="Circle" runat="server">
                                        </asp:CheckBoxList>
                                    </asp:Panel></div></td>
                <td style="width: 80%; height: 670px;"><div style="width:100%;">
                <div id="dvMap" style="width: 100%;  height: 670px; border: solid 5px #e2e2e2;">
                  <img src="images/Nokiamap.jpg" height="680px" width="820px" />
                </div>
                </div></td>
            </tr>
        </table>
         <div id="wait" style="display: none; width: 69px; height: 50px; position: absolute; top: 50%; left: 50%; padding: 2px;">
            <img src="images/uploading.gif" width="64" height="64" /><br />
            Loading..
        </div>
</asp:Content>

