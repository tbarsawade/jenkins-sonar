<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="EtaTracking, App_Web_o3dtvhns" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <meta name="viewport" content="initial-scale=1.0, width=device-width" />
    <script src="js/jquery-1.9.1.min.js"></script>
  <link rel="stylesheet" type="text/css"
    href="https://js.api.here.com/v3/3.0/mapsjs-ui.css" />
  <script type="text/javascript" charset="UTF-8"
    src="https://js.api.here.com/v3/3.0/mapsjs-core.js"></script>
  <script type="text/javascript" charset="UTF-8"
    src="https://js.api.here.com/v3/3.0/mapsjs-service.js"></script>
  <script type="text/javascript" charset="UTF-8"
    src="https://js.api.here.com/v3/3.0/mapsjs-ui.js"></script>
  <script type="text/javascript" charset="UTF-8"
    src="https://js.api.here.com/v3/3.0/mapsjs-mapevents.js"></script>

     <style type="text/css">
       .options {
       height:544px;
        border:1px solid #A6A6A6;
        margin-right:2px;
        padding:3px;
        /*background:rgba(230, 230, 230, 0.80);*/
        background:#124191;
       }
       .mapbox {
           /*height:398px;*/
            background:#A6A6A6;
            border: 3px solid #e0dede;
            height: 540px;
            width: 99.5%;
       }
       
         .leftbar {
            border-radius:3px;
            padding:10px;
             color:#000;
             background:rgba(226, 226, 226, 0.40);
             margin-bottom:10px;
             margin-top:5px;
             min-height:525px;
         }


       .ddl {
           width:100%;
          color:#124191;
          vertical-align:middle;
          padding:3px 0;
       }

        .ddl option {
            color:#124191;
          vertical-align:middle;
          padding:3px 4px;
        }

       .chk {
        margin-top:5px;
        display:block;
       }

       .txt {
           width:100%;
           height:25px;
          color:#554e4e;
       }
       .txt:focus {
        
    }
       .lbl {
           color: #fff;
           font-weight:bold;
       }
       .btn {
           width:101%;
           height:30px;
           color:#124191;
           background:#fff;
           margin-top:10px;
           border-radius:3px;
           cursor:pointer;
           /*border:1px solid #fff;*/
       }
       .btn:hover {
           
       }

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

        #DivLoc{
            padding:7px;
        max-height:304px;
        min-height:304px;
        border: 2px solid #fff;
        color:white;
        overflow :scroll;
        }

        #loading-div
        {
             width: 300px;
             height: 70px;
             background-color: #fff !important;
             text-align:center;
             position:absolute;
             left: 50%;
             top: 50%;
             margin-left:-150px;
             margin-top: -100px;
             z-index:2000;
             opacity:1.0;
             color:black;
         }

        #loading-div-background 
        {
            display:none;
            position:fixed;
            top:0;
            left:0;
            background:rgba(0, 0, 0, 0.54);
            width:100%;
            height:100%;
            z-index:1000;
         }
         fieldset {
             border-radius:2px;
             background:#fff;
             padding:2px;
             margin-top:5px;
             margin-bottom:5px;
         }
         legend {
             width:67px;
             height:18px;
             border-radius:3px;
             border:1px solid #808080;
             padding:2px;
             vertical-align:middle;
             cursor:pointer;
             background:#fff;
         }
             legend:hover {
                 background:#99CEFE;
             }
         #divDest {
             padding:2px; height:350px; overflow-y:scroll;
         }
         fieldset tr {
             
         }
             fieldset tr:hover {
                 
             }
             fieldset tr td:nth-child(2) {
                 padding:3px;
             }
             fieldset tr td:nth-child(2):hover {
                 cursor:pointer;
                 background:#99CEFE !important;
             }

             fieldset tr:nth-child(odd) {
                 background:#F3F3F3;
             }

         .tr {
             background:#99CEFE !important;
         }
         .lgend {
             background:#99CEFE !important;
         }
         .btn {
            border:1px solid #000;
            /*padding:3px;*/
            cursor:pointer;
            color:#fff;
            background:#000;
            height:20px;
            width:110px;
        }
         .btn1 {
            border:1px solid #000;
            cursor:pointer;
            color:#fff;
            background:#000;
            height:20px;
        }
           .btn1:hover, .btn:hover{
                color:#000;
                background:#E2E2E2;
            }
         .hide {
             display:none;
         }
   </style>

    <table style="width:100%;" >
        <col style="width:350px; padding:3px; vertical-align:top; height:100%;" />
        <col style="padding:3px;" />
        <tr style="width:100%;">
            <td style="padding:3px">
                <div class="leftbar">
                     <div style="margin-bottom:5px;">
                        <input type="button" id="btn" class="btn" value="Refresh Vehicle"  onclick="RefreshVechical(this);" />
                        <input type="button" id="btn1" class="btn hide" value="Shortest Route"  onclick="GetShortestPAth();"/>
                    </div>
                    <asp:DropDownList ID="ddlVehicle" CssClass="ddl" Width="324px" runat="server" onchange="GetLocations();" ClientIDMode="Static">
                    </asp:DropDownList>
                    <asp:Label ID="lblMsg" runat="server" Text="" ClientIDMode="Static"></asp:Label><br />
                   
                    <div id="divDest">
                        
                    </div>
                    <div style="padding:5px;">
                        Planned Route
                        <div style="height:7px; background:rgba(70, 193, 21, 0.78); margin-bottom:5px;"></div>
                        Current Best Route
                        <div style="height:7px; background:rgba(247, 16, 16, 0.57); margin-bottom:5px;"></div>
                        Current Shortest Route
                        <div style="height:7px; background:rgba(37, 101, 232, 0.78);"></div>
                    </div>
                </div>
            </td>
            <td>
                <div id="map" class="mapbox"></div>
            </td>
        </tr>
    </table>
   
    <div id="loading-div-background">
    <div id="loading-div" class="ui-corner-all" >
      <img style="height:34px;margin:4px;" src="images/attch.gif" alt="Loading.."/>
      <h2 style="color:black;font-weight:normal;">Please wait....</h2>
     </div>
    </div>
    
    <script  type="text/javascript" charset="UTF-8" >

        var platform = new H.service.Platform({
            app_id: 'DemoAppId01082013GAL',
            app_code: 'AJKnXv84fjrb0KIHawS0Tg',
            useCIT: true,
            useHTTPS: true
        });
        var defaultLayers = platform.createDefaultLayers();

        var map = new H.Map(document.getElementById('map'),
          defaultLayers.normal.map, {
              center: { lat: 28.502269, lng: 77.085958 },
              zoom: 4
          });

        var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
        var ui = H.ui.UI.createDefault(map, defaultLayers);

        //addMarkersToMap(map);

        var PlanRoute=null;
        var CurrentRoute = null;
        var ShortestRoute = null;
        var markers = new Array();
        var info = null;
        var vehCurrPosition = {}
        var obEta = {};
        var vehMarker = null;
        var CurMarker = null;
        var wayPoints = new Array();

        var HmapIcon = {
            HUB: new H.map.Icon("http://www.myndsaas.com/images/darkyellow.png"),
            BSC: new H.map.Icon("http://www.myndsaas.com/images/lightblue.png"),
            car: new H.map.Icon("http://www.myndsaas.com/images/car2.png")
        };

        function GetLocations() {
            ClearMap();
            try {

                if (vehMarker != null) {
                    vehMarker.setVisibility(false);
                }
                if (CurMarker != null) {
                    CurMarker.setVisibility(false);
                }
                PlanRoute = null;

                $('#btn1').addClass('hide');

                var ddlval = $("#<%=ddlVehicle.ClientID%> option:selected").val()
                if (ddlval == '0') {
                    $('#divDest').html('');
                    return false;
                }

                $("#loading-div-background").show();
                $.ajax({
                    type: "POST",
                    url: "EtaTracking.aspx/GetLocations",
                    data: '{Tid: ' + ddlval + ' }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: GetLocationSucceed,
                    failure: function (response) {
                        $("#loading-div-background").hide();
                        alert(response.d);
                    }
                });
            }
            catch (err) {
                $("#loading-div-background").hide();
            }
        }


        function GetLocationSucceed(result) {

            $("#loading-div-background").hide();
            try {
                AddVehicleToMap(result.d.VehLocation);
                if (result.d.Result.length == 0)
                { $('#divDest').html(''); return false; }
                var obj = result.d.Result;

                var str = '';

                for (var j = 0; j < obj.length; j++)
                {
                    var template = '<fieldset><legend docid="' + obj[j].DocId + '" VehType="' + obj[j].VehType + '" onclick="DrawPlanRoute(this);" >' + obj[j].Plan + '</legend><table cellpadding="0" cellspacing="0" style="width:100%"><col style="width:22px;" /><col style="width:50%;" /><col />';
                    var obj1 = obj[j].DocDetail;
                    for (var i = 0; i < obj1.length; i++)
                    {
                        template += '<tr id="' + obj1[i].Tid + '" destID="' + obj1[i].DestinationId + '" LatLong="' + obj1[i].LetLong + '" VehType="' + obj[j].VehType + '" Hault="' + obj1[i].Hault + '">';
                        template += '<td>'+'<input id="chk_" type="checkbox" />'+'</td>'
                        template += '<td destID="' + obj1[i].DestinationId + '" LatLong="' + obj1[i].LetLong + '" VehType="' + obj[j].VehType + '" Hault="' + obj1[i].Hault + '" onclick="GetCurrentRoute(this);">' + obj1[i].Destination + '</td><td>' + obj1[i].PlannedDate + '</td></tr>';
                    }
                    template += '</table>';
                    template += '<input id="btnGetMultipleEta" type="button" onclick="GetEtaViaWaypoints(this);" class="btn1" value="Get ETA via waypoints" /></fieldset>';
                    str += template;
                }

                $('#divDest').html(str);
            }
            catch(err)
            {
            }
        }
        
        var locations = new Array();
        function DrawPlanRoute(ele) {
            locations = [];
            markers = [];
            ClearMap();
            $("#loading-div-background").show();
            $('#btn1').addClass('hide');
            var tbl = $(ele).next();
            var routeData = {};
            routeData["waypoint0"] = vehCurrPosition.Lat + "," + vehCurrPosition.Lng;
            $(tbl).find('tr').each(function (i, e) {
                var ar = $(this).attr('latlong').split(',');
                routeData["waypoint" + (i+1)] = ar[0] + "," + ar[1];
                var latLong = {};
                latLong.Lat = ar[0];
                latLong.Lng = ar[1];
                //latLong.fence = ar[2];
                latLong.Tid = $(this).attr('id');
                locations.push(latLong);
            });
            
            routeData["mode"] = 'shortest;' + $(ele).attr('vehtype') + ';traffic:enabled;motorway:-1';
            routeData["representation"] = 'display';
            routeData["instructionformat"] = 'html';
            addMarkersToMap(locations)
            calculateRouteFromAtoB(platform, routeData);
            
        }

        function calculateRouteFromAtoB(platform, routeParams) {
            var router = platform.getEnterpriseRoutingService(),
              routeRequestParams = routeParams;
            router.calculateRoute(
              routeRequestParams,
              onSuccess,
              onError
            );
        }

        function onSuccess(result) {
            $("#loading-div-background").hide();
            var route = result.Response.Route[0];
            addRouteShapeToMap(route,1);
        }
        function onError(error) {
            $("#loading-div-background").hide();
            alert('Ooops!');
        }
       
        function addMarkersToMap(locations) {
            for (var i = 0; i < locations.length; i++)
            {
                var Marker = new H.map.Marker({ lat: locations[i].Lat, lng: locations[i].Lng });
                Marker.setData({ Tid: locations.Tid });
                map.addObject(Marker);
                markers.push(Marker);
            }
        }

        function addRouteShapeToMap(route,flag) {

            var strip = new H.geo.Strip(),
              routeShape = route.Shape,
              polyline;

            routeShape.forEach(function (point) {
                var parts = point.split(',');
                strip.pushLatLngAlt(parts[0], parts[1]);
            });
            //rgba(37, 101, 232, 0.78)
            var stroke = '';
            if (flag == 1) {
                stroke = 'rgba(70, 193, 21, 0.78)';
            } else if (flag == 0 || flag == 3) {
                stroke = 'rgba(247, 16, 16, 0.57)';
            }
            else {
                stroke = 'rgba(37, 101, 232, 0.78)';
            }

            polyline = new H.map.Polyline(strip, {
                style: {
                    lineWidth: 6,
                    strokeColor: stroke
                }
            });
            polyline.setArrows({ color: "#F00F", width: 1.5, length: 2, frequency: 4 });
            if (flag == 1) {
                PlanRoute = polyline;
            }
            else if (flag == 0) {
                CurrentRoute = polyline;
            }
            else {
                ShortestRoute = polyline;
            }

            // Add the polyline to the map
            map.addObject(polyline);
            // And zoom to its bounding rectangle
            map.setViewBounds(polyline.getBounds(), true);
            if (flag == 0)
            {
                GetETA();
            }
            else if (flag == 2) {
                GetETA1();
            }
            else if (flag == 3) {
                GetETA2();
            }
            else if(flag==4)
            { GetETA3(); }
        }

        function ClearMap()
        {
            if (PlanRoute != null) {
                PlanRoute.style.lineWidth = 0;
                PlanRoute.setVisibility(false);
            }
            if (info != null) {
                info.close();
            }
            if (CurrentRoute != null) {
                CurrentRoute.style.lineWidth = 0;
                CurrentRoute.setVisibility(false);
            }
            if (ShortestRoute != null) {
                ShortestRoute.style.lineWidth = 0;
                ShortestRoute.setVisibility(false);
            }
            for (var i = 0; i < markers.length; i++) {
                markers[i].setVisibility(false);
            }
            
        }

        function AddVehicleToMap(LatLong)
        {
            var bearsIcon = HmapIcon['car'];
            var arr = LatLong.split(',');
            vehCurrPosition.Lat = arr[0];
            vehCurrPosition.Lng = arr[1];
            var Marker = new H.map.Marker({ lat: arr[0], lng: arr[1] }, { icon: bearsIcon });
            Marker.setData(0);
            map.addObject(Marker);
            vehMarker = Marker;
        }

        function GetCurrentRoute(ele)
        {
            if (CurrentRoute != null) {
                CurrentRoute.style.lineWidth = 0;
                CurrentRoute.setVisibility(false);
            }
            if (ShortestRoute != null) {
                ShortestRoute.style.lineWidth = 0;
                ShortestRoute.setVisibility(false);
            }
            if (PlanRoute == null) {
                return false;
            }

            $("#loading-div-background").show();

            var SiteLatLong = $(ele).attr("latlong");
            var VehType = $(ele).attr("VehType");
            var arr = SiteLatLong.split(',');


            $('#btn1').attr('latlong', SiteLatLong);
            $('#btn1').attr('VehType', VehType);
            $('#btn1').removeClass('hide');
            $('#btn1').attr('onclick', 'GetShortestPAth();');

            obEta.Source = SiteLatLong;
            obEta.Destination = vehCurrPosition.Lat + "," + vehCurrPosition.Lng;
            obEta.VehType = VehType;
            obEta.Tid = $(ele).attr("id");
            obEta.mode = 'fastest';
            obEta.traffic = 'enabled';
            var routeData = {};
           
            routeData["waypoint0"] = vehCurrPosition.Lat + "," + vehCurrPosition.Lng;
            routeData["waypoint1"] = arr[0] + "," + arr[1];
            routeData["mode"] = 'fastest;' + VehType + ';traffic:enabled;motorway:-1';
            routeData["representation"] = 'display';
            routeData["instructionformat"] = 'html';

            var router = platform.getEnterpriseRoutingService(),
             routeRequestParams = routeData;
            router.calculateRoute(
              routeRequestParams,
              onSuccess1,
              onError
            );
        }
        function onSuccess1(result) {
            // var v = result.Response.ManeuverType.TravelTime;
            $("#loading-div-background").hide();
            var route = result.Response.Route[0];
            addRouteShapeToMap(route, 0);
        }

        
        function GetETA()
        {
            try {
                $("#loading-div-background").show();
                var str = JSON.stringify(obEta);
                $.ajax({
                    type: "POST",
                    url: "EtaTracking.aspx/GetETA",
                    data: str,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        GetETASucceed(result,0);
                    },
                    failure: function (response) {
                        $("#loading-div-background").hide();
                        alert('Error');
                    }
                });

            }
            catch(err)
            { $("#loading-div-background").hide(); }
        }

        function GetETASucceed(result, flag)
        {
            $("#loading-div-background").hide();
            var strInfo = '<b>Distance</b>: ' + result.d.Distance + '<br/><b>ETA</b>: ' + result.d.Time;

            var str = obEta.Source.split(',');

            var x = parseFloat(str[0]);
            var y = parseFloat(str[1]);

            var bubble = new H.ui.InfoBubble({ lat: x, lng: y }, {
                content: strInfo//evt.target.getData()
            });

            var bubble = new H.ui.InfoBubble({ lat: x, lng: y }, {
                content: strInfo//evt.target.getData()
            });

            if (info == null) {
                ui.addBubble(bubble);
                info = bubble;
            }
            else {
                info.close();
                ui.addBubble(bubble);
                info = bubble;
            }
            if (CurMarker != null) {
                CurMarker.setVisibility(false);
            }

            if (flag == 0) {
                var arr = obEta.Source.split(',');
                var Marker = new H.map.Marker({ lat: arr[0], lng: arr[1] });
                Marker.setData(0);
                map.addObject(Marker);
                CurMarker = Marker;
            }
        }

        function RefreshVechical()
        {
            try {
                $("#loading-div-background").show();

                var ddlval = $("#<%=ddlVehicle.ClientID%> option:selected").val()
                if (ddlval == '0') {
                    $("#loading-div-background").hide();
                    return false;
                }

                var ob = {};
                ob.Tid = ddlval;
                var str = JSON.stringify(ob);

                $.ajax({
                    type: "POST",
                    url: "EtaTracking.aspx/RefreshVehicle",
                    data: str,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        $("#loading-div-background").hide();
                        if (vehMarker != null) {
                            var arr = result.d.split(',');

                            var x = parseFloat(arr[0]);
                            var y = parseFloat(arr[1]);
                            vehMarker.setPosition({ lat: x, lng: y })
                        }
                    },
                    failure: function (response) {
                        $("#loading-div-background").hide();
                        alert('Error');
                    }
                });

            }
            catch (err)
            { $("#loading-div-background").hide(); }
        }

        function GetShortestPAth()
        {
            $("#loading-div-background").show();
            try{
            if (ShortestRoute != null) {
                ShortestRoute.style.lineWidth = 0;
                ShortestRoute.setVisibility(false);
            }
            var arr = obEta.Source.split(',');
            obEta.mode = 'shortest';
            obEta.traffic = 'disabled';
            var routeData = {};

            routeData["waypoint0"] = vehCurrPosition.Lat + "," + vehCurrPosition.Lng;
            routeData["waypoint1"] = arr[0] + "," + arr[1];
            routeData["mode"] = 'shortest;' + obEta.VehType + ';traffic:disabled;motorway:-1';
            routeData["representation"] = 'display';
            routeData["instructionformat"] = 'html';

            var router = platform.getEnterpriseRoutingService(),
             routeRequestParams = routeData;
            router.calculateRoute(
              routeRequestParams,
              onSuccess2,
              onError
            );
            }
            catch (err) { $("#loading-div-background").hide(); }
        }

        function onSuccess2(result) {
            // var v = result.Response.ManeuverType.TravelTime;
            $("#loading-div-background").hide();
            var route = result.Response.Route[0];
            addRouteShapeToMap(route, 2);
        }

        function GetETA1()
        {
            try {
                $("#loading-div-background").show();
                obEta.mode = 'shortest';
                obEta.traffic = 'disabled';
                var str = JSON.stringify(obEta);

                $.ajax({
                    type: "POST",
                    url: "EtaTracking.aspx/GetETA",
                    data: str,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        GetETASucceed(result,1);
                    },
                    failure: function (response) {
                        $("#loading-div-background").hide();
                        alert('Error');
                    }
                });

            }
            catch (err)
            { $("#loading-div-background").hide(); }
        }

        function GetEtaViaWaypoints(ele)
        {
            $("#loading-div-background").show();
            try {
                if (CurrentRoute != null) {
                    CurrentRoute.style.lineWidth = 0;
                    CurrentRoute.setVisibility(false);
                }
                if (ShortestRoute != null) {
                    ShortestRoute.style.lineWidth = 0;
                    ShortestRoute.setVisibility(false);
                }
                if (PlanRoute == null) {
                    $("#loading-div-background").hide();
                    return false;
                }

                $('#btn1').attr('onclick', 'GetShortestPAth1();');

            var tbl = $(ele).prev('table');
            var routeData = {};
            wayPoints = [];
            routeData["waypoint0"] = vehCurrPosition.Lat + "," + vehCurrPosition.Lng;
            var counter = 1;
            var vehType = '';
            $(tbl).find('tr').each(function (i, e) {
                vehType = $(e).attr('vehtype');
             var chk=$(e).children("td:first").find('input[type="checkbox"]');
             if (chk.prop("checked") == true) {
                 var ar = $(e).attr('latlong').split(',');
                 routeData["waypoint" + counter] = ar[0] + "," + ar[1];
                 var obj = {};
                 obj.Lat = ar[0];
                 obj.Lng = ar[1];
                 obj.Tid = $(e).attr('id');
                 obj.hault = $(e).attr('hault');
                 obj.VehType = vehType;
                 wayPoints.push(obj);
                 counter += 1;
             }
            });
            
            routeData["mode"] = 'fastest;' + vehType + ';traffic:enabled;motorway:-1';
            routeData["representation"] = 'display';
            routeData["instructionformat"] = 'html';

            $('#btn1').attr('latlong', "");
            $('#btn1').attr('VehType', vehType);
            $('#btn1').removeClass('hide');

            var router = platform.getEnterpriseRoutingService(),
            routeRequestParams = routeData;
            router.calculateRoute(
              routeRequestParams,
              onSuccess3,
              onError
            );

            }
            catch (err) {
                $("#loading-div-background").hide();
            }
        }
        function onSuccess3(result) {
            $("#loading-div-background").hide();
            var route = result.Response.Route[0];
            addRouteShapeToMap(route, 3);
        }

        function GetETA2() {
            try {
                $("#loading-div-background").show();
                var obj = {};
                var arrpoints = new Array();
                var arrHaults = new Array();
                arrpoints.push(vehCurrPosition.Lat + "," + vehCurrPosition.Lng);
                for (var i = 0; i < wayPoints.length; i++) {
                    arrpoints.push(wayPoints[i].Lat + ',' + wayPoints[i].Lng);
                    arrHaults.push(wayPoints[i].hault);
                }

                obEta.Source = wayPoints[wayPoints.length - 1].Lat + ',' + wayPoints[wayPoints.length - 1].Lng;

                obj.wayPointsArr = arrpoints;
                obj.Haults = arrHaults;
                obj.VehType = wayPoints[0].VehType;
                obj.mode = 'fastest';
                obj.traffic = 'enabled';
                var str = JSON.stringify(obj);

                $.ajax({
                    type: "POST",
                    url: "EtaTracking.aspx/GetEtaViaWaypoints",
                    data: str,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        GetETASucceed(result, 1);
                    },
                    failure: function (response) {
                        $("#loading-div-background").hide();
                        alert('Error');
                    }
                });

            }
            catch (err)
            { $("#loading-div-background").hide(); }
        }

        function GetShortestPAth1() {

            $("#loading-div-background").show();
            try {
                if (CurrentRoute != null) {
                    CurrentRoute.style.lineWidth = 0;
                    CurrentRoute.setVisibility(false);
                }
                if (ShortestRoute != null) {
                    ShortestRoute.style.lineWidth = 0;
                    ShortestRoute.setVisibility(false);
                }
                if (PlanRoute == null) {
                    $("#loading-div-background").hide();
                    return false;
                }

                var routeData = {};
                routeData["waypoint0"] = vehCurrPosition.Lat + "," + vehCurrPosition.Lng;
                var counter = 1;
                var vehType = wayPoints[0].VehType;
              
                for (var i = 0; i < wayPoints.length; i++) {
                    routeData["waypoint" + (i + 1)] = wayPoints[i].Lat + "," + wayPoints[i].Lng;
                }


                routeData["mode"] = 'shortest;' + vehType + ';traffic:disabled;motorway:-1';
                routeData["representation"] = 'display';
                routeData["instructionformat"] = 'html';

                $('#btn1').attr('latlong', "");
                $('#btn1').attr('VehType', vehType);
                $('#btn1').removeClass('hide');
                
                var router = platform.getEnterpriseRoutingService(),
                routeRequestParams = routeData;
                router.calculateRoute(
                  routeRequestParams,
                  onSuccess4,
                  onError
                );

            }
            catch (err) {
                $("#loading-div-background").hide();
            }
        }
        function onSuccess4(result) {
            $("#loading-div-background").hide();
            var route = result.Response.Route[0];
            addRouteShapeToMap(route, 4);
        }

        function GetETA3() {
            try {
                $("#loading-div-background").show();
                var obj = {};
                var arrpoints = new Array();
                var arrHaults = new Array();
                arrpoints.push(vehCurrPosition.Lat + "," + vehCurrPosition.Lng);
                for (var i = 0; i < wayPoints.length; i++) {
                    arrpoints.push(wayPoints[i].Lat + ',' + wayPoints[i].Lng);
                    arrHaults.push(wayPoints[i].hault);
                }

                obEta.Source = wayPoints[wayPoints.length - 1].Lat + ',' + wayPoints[wayPoints.length - 1].Lng;

                obj.wayPointsArr = arrpoints;
                obj.Haults = arrHaults;
                obj.VehType = wayPoints[0].VehType;
                obj.mode = 'shortest';
                obj.traffic = 'disabled';
                var str = JSON.stringify(obj);

                $.ajax({
                    type: "POST",
                    url: "EtaTracking.aspx/GetEtaViaWaypoints",
                    data: str,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        GetETASucceed(result, 1);
                    },
                    failure: function (response) {
                        $("#loading-div-background").hide();
                        alert('Error');
                    }
                });

            }
            catch (err)
            { $("#loading-div-background").hide(); }
        }
       
  </script>


</asp:Content>

