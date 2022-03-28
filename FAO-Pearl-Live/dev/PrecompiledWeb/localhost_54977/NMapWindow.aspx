<%@ page language="VB" autoeventwireup="false" inherits="NMapWindow, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Show Map</title>
      <script src="js/Default.js"></script>
    <link href="css/Default.css" rel="stylesheet" />
    <script src="js/jquery-1.9.1.min.js"></script>

    <style type="text/css">
        .H_ib {
            color: #fff !important;
            fill: #000 !important;
            font-size: 12px !important;
            width:250px !important;
            line-height: 20px !important;
            text-align:left !important;
            
        }
        .H_ib_content {
            margin: 10px;
            color:#000;
        }

        .H_ib_body {
            width:250px;
            right:0 !important;
            background:#fff;
            border:1px solid #808080;
            box-shadow: 5px 5px 5px #888888;
        }
        /*.H_ib_close {
            background:#000;
            height:15px;
        }*/
        .H_ib_close svg.H_icon {
        fill:#000!important;
        }
        .H_ib_tail {
            bottom: -0.6em!important;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
          <div style="width:99.9%; height:auto; border:1px solid #808080;">
              <asp:Panel ID="nomap" runat="server" Visible="false">
                <%-- <b> I have no map for this trip</b>--%>
                     <div id="msg">
                     </div>
                 </asp:Panel>
              <asp:Label ID="msg" runat="server"  ForeColor="Red"></asp:Label>
              <asp:Panel ID="Tripmap" runat="server"  Visible="false">
                     <div style="margin:auto;width:100%; text-align:left">
        
            <table style="width:100%;" border="1" cellpadding="4" cellspacing="0" >
                <col style="width:50px;" />
                <col style="width:150px;" />
                <col style="width:50px;" />
                <col style="width:150px;" />
                <tr id="trVehicle" runat="server">
                    <td style="font-weight:bold;">Vehicle No</td>
                    <td><asp:Label ID="lblVehicleNumber" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;"></td>
                    <td></td>
                </tr>
                <tr id="trspeed" runat="server">
                    <td style="font-weight:bold;">Maximum speed</td>
                    <td><asp:Label ID="lblMaxSpeed" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;">Total Distance</td>
                    <td><asp:Label ID="lblTotalDist" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr id="trStartEndLoc" runat="server">
                    <td style="font-weight:bold;">Start location</td>
                    <td><asp:Label ID="lblStartLocation" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;">End location</td>
                    <td><asp:Label ID="lblEndLocation" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr id="trStartEndDt" runat="server">
                    <td style="font-weight:bold;">Start Date & Time</td>
                    <td><asp:Label ID="lblstartDt" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;">End Date & Time</td>
                    <td><asp:Label ID="lblendDt" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </div>

              </asp:Panel>
          </div>
        <div id="mapContainer" ></div>

    </form>
</body>

    <script type="text/javascript" charset="UTF-8">
        var myMarkersArray = [];

        var infoBubble = null;

        var GlobelInfoWindow = null;

        var HmapIcon = {
            start: new H.map.Icon("http://www.myndsaas.com/images/start1.png"),
            middle: new H.map.Icon("http://www.myndsaas.com/images/greendot.png"),
            red: new H.map.Icon("http://www.myndsaas.com/images/reddot.png"),
            End: new H.map.Icon("http://www.myndsaas.com/images/end1.png"),
            HUB: new H.map.Icon("http://www.myndsaas.com/images/darkyellow.png"),
            BSC: new H.map.Icon("http://www.myndsaas.com/images/lightblue.png"),
            STRATEGIC: new H.map.Icon("http://www.myndsaas.com/images/blue.png"),
            NON_STRATEGIC: new H.map.Icon("http://www.myndsaas.com/images/darkk.png")
        }

        function addObjectsToMap(map) {

            var ar = new Array();
            var strip = new H.geo.Strip();

            for (i = 0; i < locations.length; i++) {
                var _data = {
                    Lat: locations[i][1],
                    Long: locations[i][2]
                }
                lineCoordinates.push(_data);
                if (i > 0) {

                    strip.pushPoint({
                        lat: locations[i - 1][1],
                        lng: locations[i - 1][2]
                    });
                    strip.pushPoint({
                        lat: locations[i][1],
                        lng: locations[i][2]
                    });

                }
            }

            var polyline = new H.map.Polyline(strip, {
                style: {
                    lineWidth: 6
                }
            });
            map.addObject(polyline);

            //var marker = new H.map.Marker({
            //    lat: 47.329,
            //    lng: 5.045
            //});
            //marker.addEventListener('pointerenter', onPointerEnter);
            //marker.addEventListener('pointerleave', onPointerLeave);
            //map.addObject(marker);
            var c = 0;
            var icon;





            for (i = 0; i < locations.length; i++) {

                icon = HmapIcon[locations[i][3]];// locations[i][3] == "0" ? HmapIcon.red : HmapIcon.middle;

                var marker = new H.map.Marker({
                    lat: locations[i][1],
                    lng: locations[i][2]
                }, { icon: icon });

                marker.setData(locations[i][0]);
                //marker.addEventListener('tap', onClick);
                marker.addEventListener('pointerenter', onPointerEnter);
                marker.addEventListener('pointerleave', onPointerLeave);
                //var strInfo=locations[i][0];

                marker.addEventListener('tap', function (evt) {
                    var point = map.screenToGeo(evt.pointers[0].viewportX, evt.pointers[0].viewportY);
                    infoBubble = new H.ui.InfoBubble(point, { content: evt.target.getData() });
                    //ui.addBubble(infoBubble);
                    if (GlobelInfoWindow == null) {
                        ui.addBubble(infoBubble);
                        GlobelInfoWindow = infoBubble;
                    }
                    else {
                        GlobelInfoWindow.close();
                        ui.addBubble(infoBubble);
                        GlobelInfoWindow = infoBubble;
                    }
                });


                map.addObject(marker);
            }
            if (eid == 54) {
                GetSiteList();
            }
        }

        function onClick(e) {

            var point = map.screenToGeo(e.pointers[0].viewportX, e.pointers[0].viewportY);
            infoBubble = new H.ui.InfoBubble(point, {});


            if (GlobelInfoWindow == null) {
                ui.addBubble(infoBubble);
                GlobelInfoWindow = infoBubble;
            }
            else {
                GlobelInfoWindow.close();
                ui.addBubble(infoBubble);
                GlobelInfoWindow = infoBubble;
            }

        }
        function onPointerEnter(e) {
            document.getElementById('mapContainer').style.cursor = 'pointer';
            //var point = map.screenToGeo(e.pointers[0].viewportX, e.pointers[0].viewportY);
            //infoBubble = new H.ui.InfoBubble(point, { content: 'I am being hovered!' });
            //ui.addBubble(infoBubble);
        }

        function onPointerLeave(e) {
            document.getElementById('mapContainer').style.cursor = 'default';
            //ui.removeBubble(infoBubble);
        }




        var platform = new H.service.Platform({
            app_id: app_id,
            app_code: app_code,
            //app_id: 'VG3IAYYwc1Y7XaBWEqU9',
            //app_code: 'R7W2h1KNHKBqJsJOkRbTiw',
            useCIT: true
        });


        var hidpi = ("devicePixelRatio" in window && devicePixelRatio > 1);
        var defaultLayers = platform.createDefaultLayers(hidpi ? 512 : 256,
            hidpi ? 320 : null);

        // initialize a map
        var map = new H.Map(document.getElementById('mapContainer'),
            defaultLayers.normal.map, {
                center: {
                    //lat: 19.8761653, lng: 75.3433139
                    lat: clat, lng: clong
                },
                zoom: 13,
                pixelRatio: hidpi ? 2 : 1
            });


        var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

        // Enable the default UI
        var ui = H.ui.UI.createDefault(map, defaultLayers);

        // setup the Streetlevel imagery
        platform.configure(H.map.render.panorama.RenderEngine);

        // Now use the map as required...
        addObjectsToMap(map);

        //if (eid == 54) {
        //    GetSiteList();
        //}

        function GetSiteList() {
            $.ajax({
                type: "POST",
                url: "GpsNMapReport.aspx/GetMarkerListJSON",
                data: '{ }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: CachMarkers,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }


        function CachMarkers(result) {
            var obj = csv2array(result.d, '^', '|');

            //obj = $.grep(obj, function (element, i) {
            //    var l = lineCoordinates.length - 1;
            //    //return ((element.Lat >= lineCoordinates[0].k + 0.01 && element.Long >= lineCoordinates[0].B + 0.01) || (element.Lat <= lineCoordinates[0].k - 0.01 && element.Long <= lineCoordinates[0].B - 0.01))
            //    //&& ((element.Lat >= lineCoordinates[l].k + 0.01 && element.Long >= lineCoordinates[l].B + 0.01) || (element.Lat <= lineCoordinates[l].k - 0.01 && element.Long <= lineCoordinates[l].B - 0.01));
            //    return ((element[4] >= lineCoordinates[0].Lat - 5 && element[5] >= lineCoordinates[0].Long - 5) || (element[4] <= lineCoordinates[0].Lat + 5 && element[5] <= lineCoordinates[0].Long + 5))
            //       && ((element[4] >= lineCoordinates[l].Lat - 5 && element[5] >= lineCoordinates[l].Long - 5) || (element[4] <= lineCoordinates[l].Lat + 5 && element[5] <= lineCoordinates[l].Long + 5));
            //})


            var Siteicon;
            for (i = 0; i < obj.length ; i++) {
                for (j = 0; j < lineCoordinates.length; j++) {
                    //var latLong ={
                    //    Lat: lineCoordinates[j].Lat,
                    //    Long: lineCoordinates[j].Long
                    //};
                    var Dist = GetDistance(obj[i][4], obj[i][5], lineCoordinates[j].Lat, lineCoordinates[j].Long);

                    if (Dist <= 1000 && Dist >= 0) {

                        if (obj[i][3].toUpperCase() == 'HUB') {
                            Siteicon = 'HUB';
                        }
                        else if (obj[i][3].toUpperCase() == 'BSC') {
                            Siteicon = 'BSC';
                        }
                        else if (obj[i][3].toUpperCase() == 'STRATEGIC') {
                            Siteicon = 'STRATEGIC';
                        }
                        else if (obj[i][3].toUpperCase() == 'NON STRATEGIC') {
                            Siteicon = 'NON_STRATEGIC';
                        }

                        var bearsIcon = HmapIcon[Siteicon];
                        var marker = new H.map.Marker({
                            lat: obj[i][4],
                            lng: obj[i][5]
                        }, { icon: bearsIcon });

                        marker.addEventListener('pointerenter', onPointerEnter);
                        marker.addEventListener('pointerleave', onPointerLeave);


                        var grp;
                        var tid;
                        if (obj[i][8] == '0') {
                            grp = obj[i][7];
                            tid = obj[i][1];
                        }
                        else {
                            grp = obj[i][6];
                            tid = obj[i][0];
                        }

                        marker.b = obj[i][0];

                        //marker.cg = grp + '^' + tid + '^' + obj[i][8] + '^' + obj[i][2] + '^' + obj[i][1];

                        marker.setData({
                            info: '',
                            group: grp,
                            tid: tid,
                            IsVeh: undefined,
                            SiteName: obj[i][2],
                            SiteID: obj[i][1]
                        });

                        marker.addEventListener('tap', function (evt) {

                            var strInfo = "Loading...";


                            var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });

                            var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });

                            if (GlobelInfoWindow == null) {
                                ui.addBubble(bubble);
                                GlobelInfoWindow = bubble;
                            }
                            else {
                                GlobelInfoWindow.close();
                                ui.addBubble(bubble);
                                GlobelInfoWindow = bubble;
                            }


                            //var ar = evt.target.cg.split('^');
                           // var str = ar[2] != 0 ? '1' : '0';
                            var hdn = document.getElementById("hdndata");
                           // var tid = ar[2] != 0 ? evt.target.b : ar[1];

                            var str = evt.target.getData().IsVeh != 0 ? '1' : '0';
                            var tid = evt.target.getData().IsVeh != 0 ? evt.target.b : evt.target.getData().tid;

                            $.ajax({
                                type: "post",
                                url: "GpsNMapReport.aspx/GetMarkerInfo",
                                data: "{Id : " + tid + ", IsVehical : " + str + ", Ids:'" + "" + "'}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (result) {
                                    strInfo = result.d;
                                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                                        content: result.d
                                    });

                                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                                        content: result.d
                                    });

                                    if (GlobelInfoWindow == null) {
                                        ui.addBubble(bubble);
                                        GlobelInfoWindow = bubble;
                                    }
                                    else {
                                        GlobelInfoWindow.close();
                                        ui.addBubble(bubble);
                                        GlobelInfoWindow = bubble;
                                    }
                                    var loc = evt.target.getPosition();
                                    map.setCenter(loc);
                                },
                                error: function (result) {
                                    alert('Server error');
                                }
                            });

                        }, false);

                        map.addObject(marker);

                    }

                }
            }

        }

        function csv2array(csvString, ColDelimeter, RowDelimeter) {
            var csvArray = [];
            var csvRows = csvString.split(RowDelimeter);

            for (var rowIndex = 0; rowIndex < csvRows.length; ++rowIndex) {
                var rowArray = csvRows[rowIndex].split(ColDelimeter);

                var rowObject = csvArray[rowIndex] = {};

                for (var propIndex = 0; propIndex < rowArray.length; ++propIndex) {

                    rowObject[propIndex] = rowArray[propIndex];

                }
                csvArray[rowIndex] = rowObject;
            }

            return csvArray;
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

  </script>

</html>
