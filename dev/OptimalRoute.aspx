<%@ Page Title="" Language="VB" MasterPageFile="~/PublicMaster.master" AutoEventWireup="false" CodeFile="OptimalRoute.aspx.vb" Inherits="OptimalRoute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <script src="js/route.js"></script>
    <script src="js/jquery-1.4.1.min.js"></script>

    <link rel="stylesheet" type="text/css"
    href="http://js.api.here.com/v3/3.0/mapsjs-ui.css" />
  <script type="text/javascript" charset="UTF-8"
    src="http://js.api.here.com/v3/3.0/mapsjs-core.js"></script>
  <script type="text/javascript" charset="UTF-8"
    src="http://js.api.here.com/v3/3.0/mapsjs-service.js"></script>
  <script type="text/javascript" charset="UTF-8"
    src="http://js.api.here.com/v3/3.0/mapsjs-mapevents.js"></script>
  <script type="text/javascript"  charset="UTF-8"
    src="http://js.api.here.com/v3/3.0/mapsjs-ui.js"></script>


   <style type="text/css">
       .options {
       height:594px;
        border:1px solid #A6A6A6;
        margin-right:2px;
        padding:3px;
        /*background:rgba(230, 230, 230, 0.80);*/
        background:#124191;
        
       }
       .mapbox {
           height:600px;
            border:1px solid #124191;
            background:#A6A6A6;
   
       }
       .ddl {
           width:100%;
          color:#554e4e;
          vertical-align:middle;
          padding:5px 0;
          
          
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


   </style>
    
    
     <table width="100%">
         <col style="width:20%;" />
         <col style="width:80%;"/>
        <tr>
            <td>
                <div class="options">
                    <table width="100%">
                        <tr>
                            <td>
                               <span class="lbl">Source</span> 

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="text" id="loc1" class="txt"  onblur="GetLatLong(this,true);" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="lbl">Destination</span> 
                                </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="text" id="loc2" class="txt" onblur="GetLatLong(this,false);" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="lbl">Route Type</span> 
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <select id="ddlVeh" class="ddl" >
                                    <option value="car">Car</option>
                                    <option value="truck">Truck</option>
                                    <option value="pedestrian">Pedestrian</option>
                                    <option value="publicTransport">publicTransport</option>
                                </select>
                            </td>
                        </tr>
                        <tr class="chk" style="">
                            <td>
                              <input type="checkbox" id="chkTraffic" /><span class="lbl"> Include Traffic</span>
                            </td>
                        </tr>
                        <tr class="chk">
                            <td>
                                <input type="radio"  id="radioShortest" name="o" value="shortest" tr="" /><span class="lbl"> Shortest</span>
                                <input type="radio"  id="radioFastest" name="o" value="fastest" tr=""/><span class="lbl"> Fastest</span>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <input id="btnCalculate" class="btn" type="button" onclick="DrawRoute();" value="Calculate Route" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <col style="width:50%" />
                        <col  style="width:50%"/>
                        <tr>
                            <td colspan="2"><span class="lbl">Source LatLong:</span> </td>
                        </tr>
                        <tr>
                            <td>
                                <span id="lblslat" class="lbl"></span> 
                            </td>
                            <td>
                                <span id="lblslong" class="lbl"></span> 
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><span class="lbl">Destination LatLong:</span> </td>
                        </tr>
                        <tr>
                            <td>
                                <span id="lbldlat" class="lbl"></span> 
                            </td>
                            <td>
                                <span id="lbldlong" class="lbl"></span> 
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <span id="Span1" class="lbl">Time:</span> 
                            </td>
                            <td>
                                <span id="lblTime" class="lbl"></span> 
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span id="Span3" class="lbl">Distance:</span> 
                            </td>
                            <td>
                                <span id="lblDist" class="lbl"></span> 
                            </td>
                        </tr>

                    </table>
                </div>
            </td>
            <td>
                <div id="mapContainer" class="mapbox"></div>
            </td>
        </tr>
    </table>


   <script  type="text/javascript" charset="UTF-8" >

       var arLocations = [];

       function GetLatLong(sender, isStart) {

           var txt = $(sender).val();

           var geocoder = platform.getGeocodingService(),
                     geocodingParameters = {
                         searchText: txt
                     };

           geocoder.geocode(
                     geocodingParameters,
                     function (result) {
                         var ar = {
                             Lat: result.Response.View[0].Result[0].Location.DisplayPosition.Latitude,
                             Longt: result.Response.View[0].Result[0].Location.DisplayPosition.Longitude
                         }

                         if (isStart) {
                             arLocations[0] = ar;
                         }
                         else {
                             arLocations[1] = ar;
                             //arLocations.push(ar);
                         }
                         //routeData["waypoint" + i - 1] = result.Response.View[0].Result[0].Location.DisplayPosition.Latitude + "," + result.Response.View[0].Result[0].Location.DisplayPosition.Longitude;
                     },
                     function (data) { alert('error'); }
                   );

       }

       var traffic;
       var type;
       var vehicle;

       function DrawRoute() {

           vehicle = $('#ddlVeh').val();

           if ($("#chkTraffic").is(':checked')) {
               traffic = 'enabled';
           }
           else {
               traffic = 'disabled';
           }

           type = $('input[name="o"]:checked').val();


           var routeData = {};

           for (var j = 0; j < arLocations.length; j++) {
               routeData["waypoint" + j] = arLocations[j].Lat + "," + arLocations[j].Longt;
           }
           routeData["mode"] = type + ';' + vehicle + ';traffic:' + traffic + ';motorway:-1';

           //routeData["mode"] = type + ';' + vehicle ;

           routeData["representation"] = 'display';
           routeData["instructionformat"] = 'html';

           //var routeData1 = {
           //    'waypoint0': 28.502269 + "," + 77.085958,
           //    //'waypoint1': 28.51946 + "," + 77.214289,
           //    'waypoint1': 28.623869 + "," + 77.280114,
           //    'mode': 'shortest;car;traffic:enabled;motorway:-1',
           //    'representation': 'display'
           //};
           if (line != null) {
               line.style.lineWidth = 0;
               line.setVisibility(false);
           }
           if (grp != null) {
               grp.setVisibility(false);
           }
           calculateRouteFromAtoB(platform,routeData);

       }


       function calculateRouteFromAtoB(platform,routeParams) {
           var router = platform.getEnterpriseRoutingService(),
             //routeRequestParams = {
             //    mode: 'fastest;car',
             //    representation: 'display',
             //    waypoint0: '28.502269,77.085958', 
             //    waypoint1: '28.623869,77.280114',  
             //    instructionformat: 'html'
             //};
             routeRequestParams = routeParams;
           router.calculateRoute(
             routeRequestParams,
             onSuccess,
             onError
           );
       }

       function onSuccess(result) {
          // var v = result.Response.ManeuverType.TravelTime;
           var route = result.Response.Route[0];
           addRouteShapeToMap(route);
           addManueversToMap(route);
           GetRouteInfo();
       }


       function onError(error) {
           alert('Ooops!');
       }

       var platform = new H.service.Platform({
           app_id: app_id,
           app_code: app_code,
           useCIT: true
       });
       var defaultLayers = platform.createDefaultLayers();


       var map = new H.Map(document.getElementById('mapContainer'),
         defaultLayers.normal.map, {
             center: { lat: 21.0000, lng: 78.0000 },
             zoom: 4
         });

       var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

       var ui = H.ui.UI.createDefault(map, defaultLayers);


       var bubble;


       function openBubble(position, text) {
           if (!bubble) {
               bubble = new H.ui.InfoBubble(
                 position,
                 { content: text });
               ui.addBubble(bubble);
           } else {
               bubble.setPosition(position);
               bubble.setContent(text);
               bubble.open();
           }
       }

       var line=null;
       var grp=null;
       function addRouteShapeToMap(route) {

           var strip = new H.geo.Strip(),
             routeShape = route.Shape,
             polyline;

           routeShape.forEach(function (point) {
               var parts = point.split(',');
               strip.pushLatLngAlt(parts[0], parts[1]);
           });

           polyline = new H.map.Polyline(strip, {
               style: {
                   lineWidth: 4,
                   strokeColor: 'rgba(247, 16, 16, 0.57)'
               }
           });
           polyline.setArrows({ color: "#F00F", width: 1.5, length: 2, frequency: 4 });
           line = polyline;
           
           // Add the polyline to the map
           map.addObject(polyline);
           // And zoom to its bounding rectangle
           map.setViewBounds(polyline.getBounds(), true);
       }

       function addManueversToMap(route) {
           
           var svgMarkup = '<svg width="18" height="18" ' +
             'xmlns="http://www.w3.org/2000/svg">' +
             '<circle cx="6" cy="6" r="6" ' +
               'fill="#1b468d" stroke="white" stroke-width="1"  />' +
             '</svg>',
             dotIcon = new H.map.Icon(svgMarkup, { anchor: { x: 8, y: 8 } }),
             group = new H.map.Group(),
             i,
             j;

           // Add a marker for each maneuver
           for (i = 0; i < route.Leg.length; i += 1) {
               for (j = 0; j < route.Leg[i].Maneuver.length; j += 1) {
                   // Get the next maneuver.
                   maneuver = route.Leg[i].Maneuver[j];
                   // Add a marker to the maneuvers group
                   var marker = new H.map.Marker({
                       lat: maneuver.Position.Latitude,
                       lng: maneuver.Position.Longitude
                   },
                     { icon: dotIcon });
                   marker.instruction = maneuver.Instruction;
                   group.addObject(marker);
               }
           }

           group.addEventListener('tap', function (evt) {
               map.setCenter(evt.target.getPosition());
               openBubble(
                  evt.target.getPosition(), evt.target.instruction);
           }, false);

           // Add the maneuvers group to the map
           map.addObject(group);
           grp = group;
         
       }

       // Now use the map as required...
       //calculateRouteFromAtoB(platform);

       function GetRouteInfo()
       {
           $.ajax({
               type: "POST",
               url: "OptimalRoute.aspx/RouteInfo",
               data: '{lat1: "' + arLocations[0].Lat + '", long1:"' + arLocations[0].Longt + '",lat2: "' + arLocations[1].Lat + '", long2:"' + arLocations[1].Longt + '", Vehicle:"' + vehicle + '", mode:"' + type + '", traffic:"'+traffic+'"}',
               contentType: "application/json; charset=utf-8",
               dataType: "json",
               success: ShowRouteInfo,
               failure: function (response) {
                   alert(response.d);
               }
           });

       }

       function ShowRouteInfo(result)
       {
           var ar = new Array();
               ar= result.d.split('|');

               $('#lblTime').html(ar[1]+'  Min');
               $('#lblDist').html(ar[0]+'  Kms');
               $('#lblslat').html(arLocations[0].Lat+',');
               $('#lblslong').html(arLocations[0].Longt);
               $('#lbldlat').html(arLocations[1].Lat+',');
               $('#lbldlong').html(arLocations[1].Longt);
       }
      

  </script>




</asp:Content>

