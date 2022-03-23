var x;
var ntid;
var sc;
var circle;
var rad = 1000;
var values;
var flag = 0;
var MarkerArr = [];
var Route = "fastest";
var Mode = "car";
var Traffic = "enabled";
var latlongs;
var input;
var count = 0;
var myvar;
var latlone;
var zooming = 15;
var msg;
var btype;
var vpp = new Array();
var interval;
var playgpoint;

var lineCoordinates = [];

function getLatLongs(imieno, d1, d2, vehcleName)
{
    DeviceLocation(imieno, d1, d2, vehcleName);
}


function DeviceLocation(imieno, d1, d2, vehcleName)
{
    
    $.ajax({
        type: "post",
        url: "DeviceLocation1.aspx/GetLocations",
        data: "{'imeino':'" + imieno + "','date1':'" + d1 + "','date2':'" + d2 + "','vname':'" + vehcleName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) { sdk(data); },
        error: function (data) { sdk(data); }
    });
}



function sdk(asd)
{
    
    latlone = asd.d;

    var markers = latlone;

      var mapOptions = {
          center: new google.maps.LatLng(markers[0].Lat, markers[0].Longit),
          zoom: 16,
          mapTypeId: google.maps.MapTypeId.ROADMAP
      };
      var contentString = '<div id="content" style="margin-left:15px; margin-top:3px;overflow:hidden;">' + '<div id="bodyContent">' + '<img src="images/Car1.png" style="width:172px;height:100px;" alt="WebStreet.in"/>' + '<br><font style="color:darkblue;font:11px tahoma;margin-left:5px;"> Your Trusted IT Solutions Provider</font>' + '<br><br><div style="font:13px verdana;color:darkgreen; margin-left:5px;">Plot 9A, Street 11<br>Zakir Nagar West' + '<br>New Delhi - 110025<br>India<br><br>+91-9760599879<br>+ 91-9718441975<br>info@cherisys.com<br><br>' + '<a href="/KB/aspnet/ContactForm.aspx" style="color:#00303f;font:bold 12px verdana;" title="click to contact"> Contact Form</a></div>' + '</div>' + '</div>';
      var infoWindow = new google.maps.InfoWindow({
          content: contentString,
          width: 200,
          height: 250
      });

    map = new google.maps.Map(document.getElementById("map"), mapOptions);

    for (i = 0; i < markers.length; i++) {
        lineCoordinates.push(new google.maps.LatLng(markers[i].Lat, markers[i].Longit));
    }


    marker = new google.maps.Marker({ position: new google.maps.LatLng(markers[0]['Lat'], markers[0]['Longit']), map: map, icon: 'images/start1.png', size: new google.maps.Size(1, 1) });

    marker.html = "<table style='width:100%;'><tr><td style='width:50%;text-align:left;'><b> Speed :<b></td><td style='width:50%;text-align:left;'>" + markers[0].Speeds + " Km/hr </td></tr><tr><td style='width:50%;text-align:left;'><b>Vehicle NO. :</b></td><td style='width:50%;text-align:left;'>" + markers[0].VehiName + "</td></tr><tr><td style='width:50%;text-align:left;'><b>Ctime :</b></td><td style='width:50%;text-align:left;'>" + markers[0].Ctimeing + "</td></tr><tr><td style='width:50%;text-align:left;'><b>IMEINO :</b></td><td style='width:50%;text-align:left;'>" + markers[0].IMEINUMBER + "</td></tr></table>";
    (function (marker, data) {
        var lastCenter = map.getCenter();
        google.maps.event.addListener(map, 'click', function () {
            lastCenter = map.getCenter();
        });
        var infoWindow = new google.maps.InfoWindow();
        google.maps.event.addListener(marker, "click", function (e) {
            if (InfoWindowVar != null) {
                //infoWindow.setContent("<table style='width:100%; border:solid 2px #e2e2e2;height:60px;'><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b> Speed :<b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].Speeds + "Km/hr </td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3; text-align:center;'><b>Vehicle NO. :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].VehiName + "</td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b>Ctime :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].Ctimeing + "</td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b>IMEINO :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].IMEINUMBER + "</td></tr></table>");
                InfoWindowVar.close();
                infoWindow.setContent(marker.html);
                infoWindow.open(map, marker);
                InfoWindowVar = infoWindow;
            }
            else {
                infoWindow.setContent(marker.html);
                infoWindow.open(map, marker);
                InfoWindowVar = infoWindow;
            }
        });


    })(marker, markers[0]);

    marker1 = new google.maps.Marker({ position: new google.maps.LatLng(markers[markers.length - 1]['Lat'], markers[markers.length - 1]['Longit']), map: map, icon: 'images/end1.png', size: new google.maps.Size(1, 1) });
    marker1.html = "<table style='width:100%;'><tr><td style='width:50%;text-align:left;'><b> Speed :<b></td><td style='width:50%;text-align:left;'>" + markers[markers.length - 1].Speeds + " Km/hr </td></tr><tr><td style='width:50%;text-align:left;'><b>Vehicle NO. :</b></td><td style='width:50%;text-align:left;'>" + markers[markers.length - 1].VehiName + "</td></tr><tr><td style='width:50%;text-align:left;'><b>Ctime :</b></td><td style='width:50%;text-align:left;'>" + markers[markers.length - 1].Ctimeing + "</td></tr><tr><td style='width:50%;text-align:left;'><b>IMEINO :</b></td><td style='width:50%;text-align:left;'>" + markers[markers.length - 1].IMEINUMBER + "</td></tr></table>";
    (function (marker1, data) {
        var lastCenter = map.getCenter();
        google.maps.event.addListener(map, 'click', function () {
            lastCenter = map.getCenter();
        });
        var infoWindow = new google.maps.InfoWindow();
        google.maps.event.addListener(marker1, "click", function (e) {
            if (InfoWindowVar != null) {
                //infoWindow.setContent("<table style='width:100%; border:solid 2px #e2e2e2;height:60px;'><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b> Speed :<b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].Speeds + "Km/hr </td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3; text-align:center;'><b>Vehicle NO. :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].VehiName + "</td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b>Ctime :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].Ctimeing + "</td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b>IMEINO :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].IMEINUMBER + "</td></tr></table>");
                InfoWindowVar.close();
                infoWindow.setContent(marker1.html);
                infoWindow.open(map, marker1);
                InfoWindowVar = infoWindow;
            }
            else {
                infoWindow.setContent(marker1.html);
                infoWindow.open(map, marker1);
                InfoWindowVar = infoWindow;
            }
        });


    })(marker1, markers[markers.length-1]);





    InitializePath(markers);
    //GmapMoveMent(latlone);
}



function InitializeDeviceLocations(msg1)
{
    debugger;
    var marker;

    msg = msg1;
    
    var map

    if (msg != null)
    {
        try {

             map = new google.maps.Map(document.getElementById('map'), { zoom: zooming, center: new google.maps.LatLng(msg[0].Lat, msg[0].Longit), mapTypeId: google.maps.MapTypeId.ROADMAP });

    
            var infowindow = new google.maps.InfoWindow();

            var lineCoordinates = [];

            lineCoordinates.push(new google.maps.LatLng(msg[0].Lat, msg[0].Longit));

            var FrPath = new google.maps.Polyline({ path: lineCoordinates, strokeColor: 'black' });

            FrPath.setMap(map);

                   
            for (var ctr = 0; ctr < msg.length; ctr++)
            {
                
               marker = new google.maps.Marker({ position: new google.maps.LatLng(msg[ctr].Lat, msg[ctr].Longit), map: map, icon: "images/Nokia2.png", size: new google.maps.Size(1, 1) });
               google.maps.event.addListener(marker, 'click', (function (marker, ctr) { return function () { infowindow.setContent(latlone[ctr].Lat); infowindow.open(map, marker); } })(marker, ctr));
               marker.setVisible(false);
            }

            google.maps.event.addDomListener(window, 'load', initialize);

        }
        catch (e)
        {
            alert('No Record found and  '+e.message);
        }
    }

    var m = window.setInterval(function () { d(map); }, 4000);       
}



function d(map)
{

    for (var ctr = 0; ctr < 110; ctr++)
    {
        marker = new google.maps.Marker({ position: new google.maps.LatLng(msg[ctr].Lat, msg[ctr].Longit), map: map, icon: "images/Nokia2.png", size: new google.maps.Size(1, 1) });
        google.maps.event.addListener(marker, 'click', (function (marker, ctr) { return function () { infowindow.setContent(latlone[ctr].Lat); infowindow.open(map, marker); } })(marker, ctr));
        marker.setVisible(true);
    }
   
}



function GetData(eid,branchtype)
{
    
       $.ajax({
        type: "post",
        url: "VB.aspx/ConvertDataTabletoString",
        data: "{'eid':" + eid + ",'doctype':'" + branchtype + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) { chk(data,branchtype); },
        error: function (data) { chk(data,branchtype); }
    });
}



function chk(abc, branchtype)
{ 
  CreatePolygonNokia(abc.d,branchtype);
}



function GetMonitor(eid, argbranch) {

    $.ajax({
        type: "post",
        url: "Monitor.aspx/ConvertDataTabletoStringMonitor",
        data: JSON.stringify({'eid':eid, 'doctype': argbranch}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) { sdm(data); },
        error: function (data) { sdm(data); }
    });
}



function sdm(sd)
{
   
    if (sd.d[0] == "")
    {
        $("#dvMap").html(" ");
    }
    else 
    {       
       CreatePolygonNokia(sd.d);     
    }
 }


    function CallGoogleMap(input, MasterType) {
        latlongs = input;
        initialize(MasterType);
    }


    function initialize(MasterType) {
        var latlong;
        document.getElementById("dvMap").innerHTML = "";
        var marker;
        var markers = latlongs;
        var marker_icon;

        MarkerArr.length = markers.length;
        if (String(MasterType).toUpperCase() == String("Branch").toUpperCase()) {
            marker_icon = "images/Home.png";
        }
        else if (String(MasterType).toLocaleUpperCase() == String("Enquiry Master").toUpperCase()) {
            marker_icon = "images/human.png";
        }
        else {
            marker_icon = "images/nokia2.png";
        }
        if (markers.length > 0) {
            for (i = 0; i < 1; i++) {
                latlong = markers[i].GeoPoint.split(",");
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
        var Buttonid = '<input type="image" ID="btnEdit" src="././images/edit.jpg" class="click" style="height:"56px; Width:106px;" onClick="editing()" AlternateText="Edit" /></span>';
        var Buttonid2 = '<input type="image" ID="btnLockhit" src="././images/lock.png" style="height:22px; Width:75px;" onClick="Locking()"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" /></span>';

        for (i = 0; i < markers.length; i++) {
            latlong = markers[i].GeoPoint.split(",");
            var myLatlng = new google.maps.LatLng(latlong[0], latlong[1]);
            marker = new google.maps.Marker({
                position: myLatlng,
                map: map,
                draggable: true,
                // title: data.title,
                id: markers[i].tid,
                icon: marker_icon
            });

            marker.html = "Lat :" + latlong[0] + "" + '</br>' + "Long:" + latlong[1] + "" + '</br>' + "Area Name:" + markers[i].Area + "" + '</br>' + "" + '</br>' + "Office Address:" + markers[i].Address + "" + '</br>' + "Pin Code:" + markers[i].PinCode + "" + '</br>' + '</br>' + Buttonid + Buttonid2 + '</br>' + Googletid(markers[i].tid);
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

        if (MasterType == "Branch") {
            for (i = 0; i < markers.length; i++) {

                latlong = markers[i].GeoFence.split(",");
                if (latlong.length > 0) {
                    var k = 0;


                    for (var j = 0; j < latlong.length; j = j + 2) {
                        k = j + 1;

                        var myLatlng = new google.maps.LatLng(latlong[j], latlong[k]);

                        var modes = [{
                            type: Route,
                            transportModes: [Mode],
                            options: "avoidTollroad",
                            trafficMode: Traffic
                        }];

                    }
                }
            }

            //******************************This Code Draws Polygon******************
            
            var arrLat = new Array();
            var arrLong = new Array();


            for (var i = 0; i < markers.length; i++) {
                var k = 1;
                var temp = 0;
                var arm = markers[i].GeoFence.split(",");

                for (var j = 0; j < arm.length; j = j + 2) {
                    arrLat[temp] = arm[j];
                    arrLong[temp] = arm[k];
                    k = k + 2;
                    var x = parseFloat(arrLat[temp]);
                    var y = parseFloat(arrLong[temp]);
                    vpp[temp] = new google.maps.LatLng(x, y);
                    temp++;
                }

                // Define the LatLng coordinates for the polygon's path.
                var triangleCoords = [vpp];

                // Construct the polygon.
                bermudaTriangle = new google.maps.Polygon({
                    paths: triangleCoords,
                    strokeColor: '#000',
                    strokeOpacity: 0.8,
                    strokeWeight: 2,
                    fillColor: '#C22A',
                    fillOpacity: 0.35
                });
                //  bermudaTriangle.bindTo('center', marker, 'position');
                bermudaTriangle.setMap(map);
                google.maps.event.addDomListener(window, 'load', initialize);
            }
        }

    }


    function initializeNokiaMap() {
        flag = 1;
        var latlong;
        imageMarker;
        latlongs = String(latlongs).replace("\\", "\\\\");
        markers1 = JSON.parse(latlongs);
        document.getElementById("dvMap").innerHTML = "";

        nokia.Settings.set("app_id", "VG3IAYYwc1Y7XaBWEqU9");
        nokia.Settings.set("app_code", "R7W2h1KNHKBqJsJOkRbTiw");
        nokia.Settings.set("serviceMode", "cit");
        (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

        var mapContainer = document.getElementById("dvMap");
        var infoBubbles = new nokia.maps.map.component.InfoBubbles();

        var map = new nokia.maps.map.Display(mapContainer, {
            center: [28.610, 77.23],
            zoomLevel: 5,

            components: [infoBubbles,
                           new nokia.maps.map.component.ZoomBar(),
       new nokia.maps.map.component.Traffic(),
                        new nokia.maps.map.component.Behavior(),
         new nokia.maps.map.component.TypeSelector()]

        });
        map.objects.add(new nokia.maps.map.Circle(
           // The center or the circle will be on longitude 50.0361, latitude  8.5619
           new nokia.maps.geo.Coordinate(28.61, 77.23),
           // The radius of the circle in meters
           rad,
           {

               pen: {
                   fillColor: '#FFE6E6',
                   fillOpacity: .6,
                   strokeColor: '#ffffff',
                   strokeOpacity: .6,
                   strokeWeight: .8,
                   radius: rad,
                   lineWidth: 2
               },
               brush: {

                   fillColor: '#FFE6E6',
                   fillOpacity: .6,
                   radius: rad,
                   strokeColor: '#ffffff',
                   strokeOpacity: .6,
                   strokeWeight: .8,
                   draggable: true,
                   fillOpacity: .6
               }

           }


       ));
        var redMarker;
        var markersContainer = new nokia.maps.map.Container();

        var Buttonid = '<input type="image" ID="btnEdit1" src="././images/edit.jpg" style="height:26px;Width:75px;" class="click" onClick="editing()"  AlternateText="Edit" />';
        var Buttonid2 = '<input type="image" ID="btnDtl1" src="././images/lock.png" style="height:26px;Width:75px;" class="click" onClick="Locking()" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';


        for (var i = 0; i < markers1.length; i++) {
            latlong = markers1[i].GeoPoint.split(",");
            var x = parseFloat(latlong[0]);
            var y = parseFloat(latlong[1]);
            var imageMarker = new nokia.maps.map.Marker([x, y], {
                icon: "images/Nokia2.png",
                dragable: true,
                position: [x, y],
                anchor: new nokia.maps.util.Point(1, 1),
                $html: markers1[i].tid
            }),

            image2Marker = new nokia.maps.map.Marker([x, y]);
            map.objects.addAll([imageMarker]);
            map.objects.add(markersContainer);
            markersContainer.addListener("mouseout", function (evt) {
                document.body.style.cursor = "default";
            });
            imageMarker.html = "Lat :" + latlong[0] + "" + '</br>' + "Long:" + latlong[1] + "" + '</br>' + "Area Name:" + markers1[i].Area + "" + '</br>' + "" + '</br>' + "Office Address:" + markers1[i].Address + "" + '</br>' + "Pin Code:" + markers1[i].CityPinCode + "" + '</br>' + '</br>' + Buttonid + Buttonid2 + '</br>' + Nokiatid(markers1[i].tid);
            var TOUCH = nokia.maps.dom.Page.browser.touch,
              CLICK = TOUCH ? "tap" : "click";
            imageMarker.addListener(CLICK, function (evt) {
                infoBubbles.openBubble(this.html, this.coordinate);
                ntid = (evt.target.$html);
            });
        }
    }


    function updateRadius(circle, rad) {
        circle.setRadius(rad);
    }


    function getDistanceFromLatLonInKm(lat1, lon1, lat2, lon2) {
        var R = 6371; // Radius of the earth in km
        var dLat = deg2rad(lat2 - lat1);  // deg2rad below
        var dLon = deg2rad(lon2 - lon1);
        var a =
          Math.sin(dLat / 2) * Math.sin(dLat / 2) +
          Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
          Math.sin(dLon / 2) * Math.sin(dLon / 2)
        ;
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }



    function deg2rad(deg) {
        return deg * (Math.PI / 180)
    }


    function Googletid(tid) {
        var hdnvalue = document.getElementById('ContentPlaceHolder1_hdntid');
        hdnvalue.value = tid;
        return " ";

    }


    function Nokiatid(tid) {
        var hdnvalueNokia = document.getElementById('ContentPlaceHolder1_hdntid');
        hdnvalueNokia.value = tid;
        return "  ";
    }


    function editing() {

        var btnedits = document.getElementById("ContentPlaceHolder1_btnEdit");
        setTimeout(1000);
        btnedits.click();

    }


    function Locking() {

        var btnlocks = document.getElementById("ContentPlaceHolder1_btnLockhit");
        btnlocks.click();
    }


    function initialize_enquiry() {

        var latlong;
        document.getElementById("dvMap").innerHTML = "";
        var marker;
        var markers = latlongs;
        MarkerArr.length = markers.length;

        if (markers.length > 0) {
            for (i = 0; i < 1; i++) {
                latlong = markers[i].GeoPoint.split(",");
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
        var Buttonid = '<input type="image" ID="btnEdit" src="././images/edit.jpg" class="click" style="height:"56px; Width:106px;" onClick="editing()" AlternateText="Edit" /></span>';
        var Buttonid2 = '<input type="image" ID="btnLockhit" src="././images/lock.png" style="height:22px; Width:75px;" onClick="Locking()"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" /></span>';

        for (i = 0; i < markers.length; i++) {

            latlong = markers[i].GeoPoint.split(",");
            var myLatlng = new google.maps.LatLng(latlong[0], latlong[1]);
            marker = new google.maps.Marker({
                position: myLatlng,
                map: map,
                draggable: true,
                // title: data.title,
                id: markers[i].tid
            });

            marker.html = "Lat :" + latlong[0] + "" + '</br>' + "Long:" + latlong[1] + "" + '</br>' + "Area Name:" + markers[i].Area + "" + '</br>' + "" + '</br>' + "Office Address:" + markers[i].Address + "" + '</br>' + "Pin Code:" + markers[i].PinCode + "" + '</br>' + '</br>' + Buttonid + Buttonid2 + '</br>' + Googletid(markers[i].tid);
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


    function CreatePolygonNokia(geopoints)
    {
        
        var k;

        var temp = 0;


        var arrLat = new Array();

        var arrLong = new Array();

            document.getElementById("dvMap").innerHTML = "";

            var GeoPointslength = geopoints.length;

            var argLatitude = new Array();

            var argLongitude = new Array();

            var x, y;

            for (var ctr = 0; ctr < GeoPointslength; ctr++)
            {
                var ar = geopoints[ctr].GeoPoint.split(",");

                x = parseFloat(ar[0]);

                y = parseFloat(ar[1]);

                if (isNaN(x) || isNaN(y))
                {
                    continue;
                }
                else
                {
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

                center: [x,y],
                zoomLevel: 10,

                //components: [infoBubbles, new nokia.maps.map.component.ZoomBar(), new nokia.maps.map.component.Traffic(), new nokia.maps.map.component.Behavior(), new nokia.maps.map.component.TypeSelector()]
                components: [infoBubbles, new nokia.maps.map.component.Behavior()]
            });


            var vlat = new Array();

            var vlon = new Array();

            var marker_icon;

            var count = 0;

            for (var i = 0; i < GeoPointslength; i++)
            {
                vpp = new Array();
                temp = 0;
                var arms = geopoints[i].GeoPoint.split(",");

                var fence = geopoints[i].GeoFence;
                var gpoint = geopoints[i].GeoPoint;

                

                if ((fence != null || fence != undefined || fence != undefined!="") && (gpoint || null && gpoint || undefined || gpoint !=""))
                {
                   marker_icon = "images/human.png";                    
                }
                else
                {
                    marker_icon = "images/home.png";
                }


                for (var j = 0; j < 1; j++) {

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
                        var Buttonid = '<input type="image" ID="btnEdit12" src="././images/edit.jpg" style="height:26px;Width:25px;" class="click" onClick="editing()"  AlternateText="Edit" onfocus=abc(' + geopoints[i].tid + ') />';
                        var Buttonid2 = '<input type="image" ID="btnDtl12" src="././images/lock.png" style="height:26px;Width:25px;" class="click" onClick="Locking()" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';

                        imageMarker.html = "<table width='100%'><tr><td style='width:50%;text-align:left;color:white;font-size:16px;'>Lat :</td><td style='width:50%;text-align:left;color:white;font-size:16px;'>" + y1 + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;font-size:16px;'>Long:</td><td style='width:50%;text-align:left;color:white;font-size:16px;'>" + y2 + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;font-size:16px;'>Office Address:</td><td style='width:50%;text-align:left;color:white;font-size:16px;'>" + geopoints[i].Address + '</td></tr></table>' + Buttonid + Buttonid2 + '</br>' + Nokiatid(geopoints[i].tid);

                        var TOUCH = nokia.maps.dom.Page.browser.touch,

                        CLICK = TOUCH ? "tap" : "click";

                        imageMarker.addListener(CLICK, function (evt) {

                            infoBubbles.openBubble(this.html, this.coordinate);

                            tid = (evt.target.$html);

                        });

                    }

                    if (geopoints[i].GeoFence == null) { continue; }
                    else
                    {

                        var arm = geopoints[i].GeoFence.split(",");

                        if (arm == null)
                        {
                            continue;
                        }
                        else
                        {
                            try
                            {
                             

                                for (var j = 0; j < arm.length; j = j + 2)
                                {
                                    
                                    arrLat[temp] = arm[j];

                                    k = j + 1;

                                    arrLong[temp] = arm[k];

                                    x = parseFloat(arrLat[temp]);

                                    y = parseFloat(arrLong[temp]);

                                    if (isNaN(x) || isNaN(y))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                       
                                        vpp[temp] = new nokia.maps.geo.Coordinate(x, y);

                                        temp++;

                                    }

                                    
                                }
                             
                            }
                            catch(e)
                            {
                                alert(e.message);
                            }
                           

                        }
                    }
                }
               
                
                if (vpp !="")
                {
                  
                    var TOUCH = nokia.maps.dom.Page.browser.touch, CLICK = TOUCH ? "tap" : "click";

                    map.objects.add(new nokia.maps.map.Polygon(vpp, { pen: { lineWidth: 0 } }));
                  

                    map.set("zoomLevel", 17);

                    vpp = null;

                   
                }
                
            }           
       $find('PleaseWaitPopup').hide();
    }


    function abc(tid) {

        document.getElementById('ContentPlaceHolder1_hdntid').value = tid;

    }

    function MovementInMap(msg) {
        var markers = msg;

        var mapOptions = {
            center: new google.maps.LatLng(markers[0].Lat, markers[0].Longit),
            zoom: 16,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        var infoWindow = new google.maps.InfoWindow();
        var map = new google.maps.Map(document.getElementById("map"), mapOptions);
        var i = 0;
        var interval = setInterval(function () {
            var data = markers[i]
            var myLatlng = new google.maps.LatLng(data.Lat, data.Longit);
            var marker = new google.maps.Marker({
                position: myLatlng,
                map: map,
                title: data.title,
                animation: google.maps.Animation.DROP
            });
            (function (marker, data) {
                google.maps.event.addListener(marker, "mouseover", function (e) {
                    infoWindow.setContent(data.description);
                    infoWindow.open(map, marker);
                });
            })(marker, data);
            i++;
            if (i == markers.length) {
                clearInterval(interval);
            }
        }, 1000);
    }

    var map
    var i = 0;
    var j = i + 1;
    var timer;
    function GmapMoveMent(msg) {
      
        //playgpoin = msg;
        playgpoin = latlone;
       
        try
        {
            var markers = msg;

          //  var mapOptions = {
          //      center: new google.maps.LatLng(markers[0].Lat, markers[0].Longit),
          //      zoom: 16,
          //      mapTypeId: google.maps.MapTypeId.ROADMAP
          //  };
          //  var contentString = '<div id="content" style="margin-left:15px; margin-top:3px;overflow:hidden;">' + '<div id="bodyContent">' + '<img src="images/Car1.png" style="width:172px;height:100px;" alt="WebStreet.in"/>' + '<br><font style="color:darkblue;font:11px tahoma;margin-left:5px;"> Your Trusted IT Solutions Provider</font>' + '<br><br><div style="font:13px verdana;color:darkgreen; margin-left:5px;">Plot 9A, Street 11<br>Zakir Nagar West' + '<br>New Delhi - 110025<br>India<br><br>+91-9760599879<br>+ 91-9718441975<br>info@cherisys.com<br><br>' + '<a href="/KB/aspnet/ContactForm.aspx" style="color:#00303f;font:bold 12px verdana;" title="click to contact"> Contact Form</a></div>' + '</div>' + '</div>';
          //  var infoWindow = new google.maps.InfoWindow({
          //      content: contentString,
          //      width: 200,
          //      height: 250
          //  });

          //map = new google.maps.Map(document.getElementById("map"), mapOptions);

       
          //for (i = 0; i < markers.length; i++) {
          //    lineCoordinates.push(new google.maps.LatLng(markers[i].Lat, markers[i].Longit));
          //}
         



             interval = setInterval(function () {
                DrawLine(markers)
            }, 200);

             timer = 1;
        }
        catch (e)
        {

        }
    }

    function StopMarker()
    {
        if (timer == 0)
        { return; }
        else
        {
            clearInterval(interval);
            timer = 0;
        }
          
    }

    var startPlay = true;

    function Play()
    {
        if (startPlay) {
            GmapMoveMent(latlone);
        }
        else {

            startPlay = false;

            if (timer == 1)
            { return; }
            else
            {
                interval = setInterval(function () {
                    DrawLine(playgpoin)
                }, 200);
                timer = 1;
            }
        }
    }

//**********************************************************************************************************************************************************************************

    function CreatePolygonNokiaReport(geopoints)
    {
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
            var ar = geopoints[ctr].GeoPoint.split(",");

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
            zoomLevel: 6,

            components: [infoBubbles, new nokia.maps.map.component.ZoomBar(), new nokia.maps.map.component.Traffic(), new nokia.maps.map.component.Behavior(), new nokia.maps.map.component.TypeSelector()]
            //components: [infoBubbles, new nokia.maps.map.component.Behavior()]
        });


        var vlat = new Array();

        var vlon = new Array();

        var marker_icon;

        var count = 0;

        for (var i = 0; i < GeoPointslength; i++) {
            vpp = new Array();
            temp = 0;
            var arms = geopoints[i].GeoPoint.split(",");

            var fence = geopoints[i].GeoFence;
            var gpoint = geopoints[i].GeoPoint;



            if ((fence != null || fence != undefined || fence != undefined != "") && (gpoint || null && gpoint || undefined || gpoint != "")) {
                marker_icon = "images/human.png";
            }
            else {
                marker_icon = "images/home.png";
            }


            for (var j = 0; j < 1; j++) {

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
                    var Buttonid = '<input type="image" ID="btnEdit12" src="././images/edit.jpg" style="height:26px;Width:50px;" class="click" onClick="editing()"  AlternateText="Edit" onfocus=abc(' + geopoints[i].tid + ') />';
                    var Buttonid2 = '<input type="image" ID="btnDtl12" src="././images/lock.png" style="height:26px;Width:50px;" class="click" onClick="Locking()" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';

                    imageMarker.html = "<table width='100%'><tr><td style='width:50%;text-align:left;color:white;'>" + "Latitude :</td>" + '<td style="width:50%;text-align:left;color:white;">' + y1 + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;'>Longitude:</td><td style='width:50%;text-align:left;color:white;'>" + y2 + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Branch:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i].Branch + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'><Customer Name:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Customer Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Creation Date:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Creation Date"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Enquiry Date:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Enquiry Date"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Enquiry No.:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Enquiry No."] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Booking Officer:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Booking Officer"] + "</td></tr><tr><td style='width:50%;text-align:left;color:white;'>" + "Grand Total:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Grand Total Size in Cft"] + "</td></tr><tr><td style='width:50%;text-align:left;color:white;'>" + "Status:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Status"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Survey Date:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Survey Date"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Total Goods:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Total Declared Value of Goods"] + "</td></tr></table>"

                    var TOUCH = nokia.maps.dom.Page.browser.touch,
                    
                    CLICK = TOUCH ? "tap" : "click";

                    imageMarker.addListener(CLICK, function (evt) {

                        infoBubbles.openBubble(this.html, this.coordinate);

                        tid = (evt.target.$html);

                    });

                }

                if (geopoints[i].GeoFence == null) { continue; }
                else
                {

                    var arm = geopoints[i].GeoFence.split(",");

                    if (arm == null) {
                        continue;
                    }
                    else {
                        try {


                            for (var j = 0; j < arm.length; j = j + 2) {

                                arrLat[temp] = arm[j];

                                k = j + 1;

                                arrLong[temp] = arm[k];

                                x = parseFloat(arrLat[temp]);

                                y = parseFloat(arrLong[temp]);

                                if (isNaN(x) || isNaN(y)) {
                                    continue;
                                }
                                else {

                                    vpp[temp] = new nokia.maps.geo.Coordinate(x, y);

                                    temp++;

                                }


                            }

                        }
                        catch (e) {
                            alert(e.message);
                        }


                    }
                }
            }


            if (vpp != "") {

                var TOUCH = nokia.maps.dom.Page.browser.touch, CLICK = TOUCH ? "tap" : "click";

                map.objects.add(new nokia.maps.map.Polygon(vpp, { pen: { lineWidth: 0 } }));


                map.set("zoomLevel", 6);

                vpp = null;


            }

        }
        
    }




/*
Prashant Start
*/

    var InfoWindowVar = null;

    function DrawLine(markers) {

        i =parseInt($('#hdnmove').attr('value'));
        j = i + 1;

        var data = markers[i]

        var imgs = '';
        if (i == 191)
        {
            var vf = '';
        }
        if (i == 0)
        {
            imgs = "images/start1.png";
        }
        else if (i == markers.length - 2) {
            imgs = "images/end1.png";
        }
        else {
            imgs = "images/Nokia2.png";
        }
        var myLatlng = new google.maps.LatLng(markers[i].Lat, markers[i].Longit);
        var infowindow = new google.maps.InfoWindow();
        var marker = new google.maps.Marker({
            icon: imgs,
            position: myLatlng,
            map: map,
            animation: google.maps.Animation.draggable
        });

        marker.polyline = new google.maps.Polyline({
            path: [new google.maps.LatLng(markers[i].Lat, markers[i].Longit), new google.maps.LatLng(markers[j].Lat, markers[j].Longit)],
            strokeColor: "blue",
            strokeOpacity: 0.5,
            strokeWeight: 4,
            geodesic: true,
            map: map,
            polylineID: i
        });
        marker.html = "<table style='width:100%;'><tr><td style='width:50%;text-align:left;'><b> Speed :<b></td><td style='width:50%;text-align:left;'>" + markers[i].Speeds + " Km/hr </td></tr><tr><td style='width:50%;text-align:left;'><b>Vehicle NO. :</b></td><td style='width:50%;text-align:left;'>" + markers[i].VehiName + "</td></tr><tr><td style='width:50%;text-align:left;'><b>Ctime :</b></td><td style='width:50%;text-align:left;'>" + markers[i].Ctimeing + "</td></tr><tr><td style='width:50%;text-align:left;'><b>IMEINO :</b></td><td style='width:50%;text-align:left;'>" + markers[i].IMEINUMBER + "</td></tr></table>";
        (function (marker, data) {
            var lastCenter = map.getCenter();
            google.maps.event.addListener(map, 'click', function () {
                lastCenter = map.getCenter();
            });
            var infoWindow = new google.maps.InfoWindow();
            google.maps.event.addListener(marker, "click", function (e) {
                if (InfoWindowVar != null) {
                    //infoWindow.setContent("<table style='width:100%; border:solid 2px #e2e2e2;height:60px;'><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b> Speed :<b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].Speeds + "Km/hr </td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3; text-align:center;'><b>Vehicle NO. :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].VehiName + "</td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b>Ctime :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].Ctimeing + "</td></tr><tr><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'><b>IMEINO :</b></td><td style='width:50%;border:solid 2px #e3e3e3;text-align:center;'>" + markers[i].IMEINUMBER + "</td></tr></table>");
                    InfoWindowVar.close();
                    infoWindow.setContent(marker.html);
                    infoWindow.open(map, marker);
                    InfoWindowVar = infoWindow;
                }
                else {
                    infoWindow.setContent(marker.html);
                    infoWindow.open(map, marker);
                    InfoWindowVar = infoWindow;
                }
            });


        })(marker, data);
        var myLatlng1 = new google.maps.LatLng(markers[i].Lat, markers[i].Longit);
        map.setCenter(myLatlng1)
        i++;
        j++;

        $('#hdnmove').attr('value',i)

        if (j == markers.length) {
            clearInterval(interval);
        }
    }


    function GetSiteList() {

        $.ajax({
            type: "POST",
            url: "DeviceLocation1.aspx/GetMarkerListJSON",
            data: '{ }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: CachMarkers,
            failure: function (response) {
                alert(response.d);
            }
        });

    }

    function CachMarkers(result)
    {
        var str = result.d;
        var v = str.substring(1, str.lastIndexOf(","));
        var lastChar = str.slice(-1);
        if (lastChar == ',') {
            str = str.slice(0, -1);
        }
        str = str + ']';
        var obj = JSON.parse(str);
        var l = obj.length;




        var infowindow = new google.maps.InfoWindow();
        var icon;

        obj = $.grep(obj, function (element, i) {
           // var l1 = lineCoordinates.length - 1;
            //return ((element.Lat >= lineCoordinates[0].k + 0.01 && element.Long >= lineCoordinates[0].B + 0.01) || (element.Lat <= lineCoordinates[0].k - 0.01 && element.Long <= lineCoordinates[0].B - 0.01))
            //&& ((element.Lat >= lineCoordinates[l].k + 0.01 && element.Long >= lineCoordinates[l].B + 0.01) || (element.Lat <= lineCoordinates[l].k - 0.01 && element.Long <= lineCoordinates[l].B - 0.01));
            return ((element.Lat >= lineCoordinates[0].k - 5 && element.Long >= lineCoordinates[0].B - 5) || (element.Lat <= lineCoordinates[0].k + 5 && element.Long <= lineCoordinates[0].B + 5))
               && ((element.Lat >= lineCoordinates[lineCoordinates.length - 1].k - 5 && element.Long >= lineCoordinates[lineCoordinates.length - 1].B - 5) || (element.Lat <= lineCoordinates[lineCoordinates.length - 1].k + 5 && element.Long <= lineCoordinates[lineCoordinates.length - 1].B + 5));
        })

        var ll = obj.length;

        for (i = 0; i < obj.length - 1; i++) {

            for (j = 0; j < lineCoordinates.length; j++) {

                var latLong = lineCoordinates[j];
                var Dist = GetDistance(obj[i]['Lat'], obj[i]['Long'], latLong.k, latLong.B);

                if (Dist <= 1000 && Dist >= 0) {
                    if (obj[i].Site.toUpperCase() == 'HUB') {
                        icon = "http://www.myndsaas.com/images/darkyellow.png";
                    }
                    else if (obj[i].Site.toUpperCase() == 'BSC') {
                        icon = "http://www.myndsaas.com/images/lightblue.png";
                    }
                    else if (obj[i].Site.toUpperCase() == 'STRATEGIC') {
                        icon = "http://www.myndsaas.com/images/blue.png";
                    }
                    else if (obj[i].Site.toUpperCase() == 'NON STRATEGIC') {
                        icon = "http://www.myndsaas.com/images/darkk.png";
                    }

                    marker = new google.maps.Marker({ position: new google.maps.LatLng(obj[i]['Lat'], obj[i]['Long']), map: map, icon: icon, group: obj[i]['Group'], size: new google.maps.Size(1, 1) });
                    marker.primaryKey = obj[i]['SiteID'];
                    marker.SiteName = obj[i]['Site_Name'];

                    marker.html = "<span style='font-weight:bold;'> SiteID : " + obj[i].SiteID + "</span><br>Site : " + obj[i].Site + "<br>Site Name : " + obj[i].Site_Name + "<br>OandM Head: " + obj[i].OandM_Head + " <br> Maintenance Head : " + obj[i].Maintenance_Head + " <br>Opex Manager : " + obj[i].Opex_Manager + "<br>Security Manager : " + obj[i].Security_Manager + "<br>Zonal Head : " + obj[i].Zonal_Head + "<br>Cluster Manager :" + obj[i].Cluster_Manager + "<br>Supervisor : " + obj[i].Supervisor + "<br>Technician : " + obj[i].Technician + "<br>No of OPCOs : " + obj[i].No_of_OPCOs + " <br>Anchor OPCO : " + obj[i].Anchor_OPCO + "";

                    myMarkersArray.push(marker);
                    google.maps.event.addListener(marker, 'click', (function (marker, i) {
                        return function () {
                            if (InfoWindowVar != null) {
                                InfoWindowVar.close();
                                infowindow.setContent(marker.html);
                                infowindow.open(map, marker);
                                InfoWindowVar = infowindow;
                            }
                            else {
                                infowindow.setContent(marker.html);
                                infowindow.open(map, marker);
                                InfoWindowVar = infowindow;
                            }

                        }
                    })(marker, i));

                    break;

                }

            }




        }



    }
   
    function GetDistance(lat1, lon1, lat2, lon2) {

        var degreesToRadians = Math.PI / 180;
        var earthRadius = 6371; // approximation in kilometers assuming earth to be spherical

        // CONVERT LATITUDE AND LONGITUDE VALUES TO RADIANS
        var previousRadianLat = lat1 * degreesToRadians;
        var previousRadianLong = lon1 * degreesToRadians;
        var currentRadianLat = lat2 * degreesToRadians;
        var currentRadianLong = lon2 * degreesToRadians;

        // CALCULATE RADIAN DELTA BETWEEN THE TWO POSITIONS
        var latitudeRadianDelta = currentRadianLat - previousRadianLat;
        var longitudeRadianDelta = currentRadianLong - previousRadianLong;

        var expr1 = (Math.sin(latitudeRadianDelta / 2) * Math.sin(latitudeRadianDelta / 2)) +
                    (Math.cos(previousRadianLat) * Math.cos(currentRadianLat) * Math.sin(longitudeRadianDelta / 2) * Math.sin(longitudeRadianDelta / 2));
        var expr2 = 2 * Math.atan2(Math.sqrt(expr1), Math.sqrt(1 - expr1));
        var distanceValue = earthRadius * expr2;

        return distanceValue * 1000; //Meters

    }


    function InitializePath(markers)
    {
        var locations = markers;
        //map = new google.maps.Map(document.getElementById('map'), { zoom: 12, center: new google.maps.LatLng(locations[0].Lat, locations[0].Longit), mapTypeId: google.maps.MapTypeId.ROADMAP });
        var infowindow = new google.maps.InfoWindow();
        
        var FrPath = new google.maps.Polyline({ path: lineCoordinates, strokeColor: 'rgba(0, 0, 0, 0.5)' });
        FrPath.setMap(map);

        return;
    }


/*
Prashant End
*/


   

