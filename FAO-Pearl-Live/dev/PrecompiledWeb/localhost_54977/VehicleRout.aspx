<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="VehicleRout, App_Web_15ulzn3z" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src="js/jquery-1.9.1.min.js"></script>
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
       height:544px;
        border:1px solid #A6A6A6;
        margin-right:2px;
        padding:3px;
        /*background:rgba(230, 230, 230, 0.80);*/
        background:#124191;
        
       }
       .mapbox {
           /*height:398px;*/
           height:548px;
            border:1px solid #124191;
            background:#A6A6A6;
       }
       .mapbox1 {
           height:150px;
            background:#A6A6A6;
             border-left: 1px solid #124191;
         border-bottom: 1px solid #124191;
          border-right: 1px solid #124191;
          display:none;
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

   </style>

    <div id="loading-div-background">
    <div id="loading-div" class="ui-corner-all" >
      <img style="height:34px;margin:4px;" src="images/attch.gif" alt="Loading.."/>
      <h2 style="color:black;font-weight:normal;">Calculating route....</h2>
     </div>
    </div>


    <table width="100%">
         <col style="width:20%;" />
         <col style="width:80%;"/>
        <tr>
            <td>
                <div class="options">
                    <table width="100%">
                        <tr>
                            <td>
                               <span class="lbl">Select Vehicle</span> 

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlVehicle" CssClass="ddl" Width="100%" runat="server" onchange="GetLocations();" ClientIDMode="Static">
                                    
                                </asp:DropDownList>
                            </td>
                        </tr>
                       
                        <tr>
                            <td>
                                
                            </td>
                        </tr>
                        
                        <tr class="chk" style="display:none">
                            <td>
                              <input type="checkbox" id="chkTraffic" /><span class="lbl"> Include Traffic</span>
                            </td>
                        </tr>
                        <tr class="chk" style="display:none">
                            <td>
                                <input type="radio"  id="radioShortest" name="o" value="shortest" tr="" /><span class="lbl"> Shortest</span>
                                <input type="radio"  id="radioFastest" name="o" value="fastest" tr=""/><span class="lbl"> Fastest</span>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        
                        <tr>
                            <td>
                                
                            </td>
                        </tr>
                        <tr>
                            <td>
                                
                            </td>
                        </tr>

                    </table>
                    <table width="100%">
                        <col style="width:50%" />
                        <col  style="width:50%"/>
                        
                       
                        <tr>
                            <td colspan="2">
                                 <span class="lbl"></span> 
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div id="DivLoc1">
                                   
                                </div>
                            </td>
                        </tr>

                    </table>
                </div>
            </td>
            <td>
                <div id="map" class="mapbox"></div>
                <div id="divRouts" class="mapbox1">
                    <table width="100%">
                        <col style="width:100px" />
                        <col  style="width:100px"/>
                        <col  style=""/>
                        <tr>
                            <th></th><th></th><th></th><th></th>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>

    <script  type="text/javascript" charset="UTF-8" >

        function calculateRouteFromAtoB(platform, routeParams) {
            var router = platform.getEnterpriseRoutingService(),
             //routeRequestParams = {
             //    mode: 'fastest;car',
             //    representation: 'display',
             //    waypoint0: '28.502269,77.085958', 
             //    waypoint1: '28.623869,77.280114',  
             //    instructionformat: 'html',
             //    app_id: 'DemoAppId01082013GAL',
             //    app_code: 'AJKnXv84fjrb0KIHawS0Tg'
             //};
            
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
            addRouteShapeToMap(route);
           // addManueversToMap(route);
            AddMarkers();
        }

       
        function onError(error) {
            alert('Ooops!');
        }

        var platform = new H.service.Platform({
            //app_id: 'DemoAppId01082013GAL',
            //app_code: 'AJKnXv84fjrb0KIHawS0Tg',
            app_id: 'DemoAppId01082013GAL',
            app_code: 'AJKnXv84fjrb0KIHawS0Tg',
            useCIT: true
        });

        var defaultLayers = platform.createDefaultLayers();

        var map = new H.Map(document.getElementById('map'),
          defaultLayers.normal.map, {
              center: { lat: 28.502269, lng: 77.085958 },
              zoom: 13
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
                    strokeColor: 'rgba(0, 128, 255, 0.7)'
                }
            });
            // Add the polyline to the map
            map.addObject(polyline);
            routeLine = polyline;
            // And zoom to its bounding rectangle
            map.setViewBounds(polyline.getBounds(), true);
            
        }
        var routeLine;
        var markers = [];
        
        function AddMarkers()
        {
            var distPath = objMarkers['DistancePath'];
            var svgMarkupGreen = '<svg width="22" height="22" ' +
              'xmlns="http://www.w3.org/2000/svg">' +
              '<circle cx="8" cy="8" r="8" ' +
                'fill="#009900" stroke="black" stroke-width="1"  />' +
              '</svg>';

            var svgMarkupRed = '<svg width="22" height="22" ' +
             'xmlns="http://www.w3.org/2000/svg">' +
             '<circle cx="8" cy="8" r="8" ' +
               'fill="#E62E00" stroke="black" stroke-width="1"  />' +
             '</svg>';

            var svgMarkupBlue = '<svg width="22" height="22" ' +
             'xmlns="http://www.w3.org/2000/svg">' +
             '<circle cx="8" cy="8" r="8" ' +
               'fill="#3399FF" stroke="black" stroke-width="1"  />' +
             '</svg>';

            //var ico = new H.map.Icon(svgMarkup, { anchor: { x: 8, y: 8 } });
            group = new H.map.Group();
            var ico;
            for (var i = 0; i < distPath.length; ++i) {
                if (i == 0)
                {
                    ico = new H.map.Icon(svgMarkupGreen, { anchor: { x: 8, y: 8 } });
                }
                else if (i == distPath.length - 1) {
                    ico = new H.map.Icon(svgMarkupRed, { anchor: { x: 8, y: 8 } });
                }
                else {
                    ico = new H.map.Icon(svgMarkupBlue, { anchor: { x: 8, y: 8 } });
                }

                var marker = new H.map.Marker({
                    lat: distPath[i].AdditionalData.Latt,
                    lng: distPath[i].AdditionalData.Longt
                }, { icon: ico });

                var info = 'SiteName : ' + distPath[i].AdditionalData.SiteName + '<br/>Address : ' + distPath[i].AdditionalData.Address;
                marker.instruction = info;
               // group.addObject(marker);
                marker.addEventListener('tap', function (evt) {
                    map.setCenter(evt.target.getPosition());
                    openBubble(
                       evt.target.getPosition(), evt.target.instruction);
                }, false);
                marker.setZIndex(100);
                map.addObject(marker);
                markers.push(marker);
            }

            //group.addEventListener('tap', function (evt) {
            //    map.setCenter(evt.target.getPosition());
            //    openBubble(
            //       evt.target.getPosition(), evt.target.instruction);
            //}, false);

            //map.addObject(group);
        }

        function addManueversToMap(route) {
            var svgMarkup = '<svg width="18" height="18" ' +
              'xmlns="http://www.w3.org/2000/svg">' +
              '<circle cx="8" cy="8" r="8" ' +
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
                    Maneuver = route.Leg[i].Maneuver[j];
                    // Add a marker to the maneuvers group
                    var marker = new H.map.Marker({
                        lat: Maneuver.Position.Latitude,
                        lng: Maneuver.Position.Longitude
                    },
                      { icon: dotIcon });
                    marker.instruction = Maneuver.Instruction;
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
        }

        // Now use the map as required...
        //calculateRouteFromAtoB(platform);


        function GetLocations() {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setVisibility(false);
            }
            if (routeLine != undefined || routeLine != null)
            {
                routeLine.setVisibility(false);
            }
            //calculateRouteFromAtoB(platform, '');
            //return false;
            var ddlval = $("#<%=ddlVehicle.ClientID%> option:selected").val()
            if (ddlval.toUpperCase() == 'SELECT') {
                return false;
            }
            $("#loading-div-background").show();

            $.ajax({
                type: "POST",
                url: "VehicleRout.aspx/GetLocations",
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
        var line = null;
        var grp = null;
        var objMarkers = null;
        function GetLocationSucceed(result) {
            var obj = JSON.parse(result.d);
            objMarkers = obj;
            var distPath = obj['DistancePath'];
            var routeData = {};
            for (var i = 0; i < distPath.length; ++i) {
                routeData["waypoint" + i] = distPath[i].AdditionalData.Latt + ',' + distPath[i].AdditionalData.Longt
                //routeRequestParams['destination' + (i - 1)] = arr[2];
            }
            routeData["mode"] = 'fastest;car;traffic:enabled;motorway:-1';
            routeData["representation"] = 'display';
            routeData["instructionformat"] = 'html';

            routeData["app_id"] = 'DemoAppId01082013GAL';
            routeData["app_code"] = 'AJKnXv84fjrb0KIHawS0Tg';

            if (line != null) {
                line.style.lineWidth = 0;
                line.setVisibility(false);
            }
            if (grp != null) {
                grp.setVisibility(false);
            }

            calculateRouteFromAtoB(platform, routeData);
        }

  </script>

</asp:Content>

