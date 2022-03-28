<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="PlannedRout, App_Web_0xvfyc51" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
         <col style="padding:5px;" />
         <col style="padding:5px;"/>
        <tr>
            <td>
                <asp:DropDownList ID="ddlRoute" CssClass="ddl" Width="200px" runat="server" onchange="GetLocations();" ClientIDMode="Static">
                </asp:DropDownList>
            </td>
        </tr>
    </table>


    <div id="map" style="width: 100%; height: 400px; background: grey" />


    <script type="text/javascript">

        function addPolylineToMap(map, data) {
            var strip = new H.geo.Strip();

            //strip.pushPoint({ lat: 28.3477, lng: 77.2597 });
            //strip.pushPoint({ lat: 28.5008, lng: 77.1224 });
            //strip.pushPoint({ lat: 28.8567, lng: 77.3508 });
            //strip.pushPoint({ lat: 28.5166, lng: 77.3833 });
            for (var i = 0; i < objArr.length - 2; i++) {
                var x = parseFloat(objArr[i][6]);
                var y = parseFloat(objArr[i][7]);
                strip.pushPoint({ lat: x, lng: y });
            }


            map.addObject(new H.map.Polyline(
              strip, { style: { lineWidth: 4 } }
            ));
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

        function GetLocations() {
            
            //calculateRouteFromAtoB(platform, '');
            //return false;
            var ddlval = $("#<%=ddlRoute.ClientID%> option:selected").val()
            if (ddlval.toUpperCase() == 'SELECT') {
                return false;
            }
            $("#loading-div-background").show();

            $.ajax({
                type: "POST",
                url: "PlannedRout.aspx/GetLocations",
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

        var markers = [];
        var line;
        var bubble = null;
        function GetLocationSucceed(result) {
            $("#loading-div-background").hide();
            var objArr = csv2array(result.d, '^', '|');

            for (var i = 0; i < objArr.length-1; i++) {
             var   x = parseFloat(objArr[i][6]);
             var y = parseFloat(objArr[i][7]);
             var ico;
             if (i == 0) {
                 ico = new H.map.Icon(svgMarkupGreen, { anchor: { x: 8, y: 8 } });
             }
             else if (i == objArr.length - 2) {
                 ico = new H.map.Icon(svgMarkupRed, { anchor: { x: 8, y: 8 } });
             }
             else {
                 ico = new H.map.Icon(svgMarkupBlue, { anchor: { x: 8, y: 8 } });
             }
            
             var marker = new H.map.Marker({ lat: x, lng: y }, { icon: ico });

             var eta = objArr[i][5]
             if (objArr[i][5] != '') {
                 eta = (objArr[i][5]) / 60;
             }
             else {
                 eta = 0;
             }

             var info = 'Sequence : ' + objArr[i][4] + '<br/>ETA : ' + eta + ' Min';
             marker.setData(info);
             marker.addEventListener('tap', function (evt) {
                 
                 if (bubble != null)
                 {
                     bubble.close();
                 }
                  bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                     content: evt.target.getData()
                 });

                 ui.addBubble(bubble);


             }, false);


             map.addObject(marker);
             //markers.push(marker);

            }

           // addPolylineToMap(map, objArr);
            
            //for (var i = 0; i < objArr.length-1; i++) {

            //    var strip = new H.geo.Strip();
            //    var x = parseFloat(objArr[i][6]);
            //    var y = parseFloat(objArr[i][7]);
            //    var x1 = parseFloat(objArr[i+1][6]);
            //    var y1 = parseFloat(objArr[i+1][7]);
            //    strip.pushPoint({ lat: x, lng: y });
            //    strip.pushPoint({ lat: x1, lng: y1 });
            //    map.addObject(new H.map.Polyline(
            //    strip, { style: { lineWidth: 4 } }
            //  ));
            //    i = i + 1;

            //}
            var strip = new H.geo.Strip();

            for (var i = 0; i < objArr.length - 1; i++) {
                var x = parseFloat(objArr[i][6]);
                var y = parseFloat(objArr[i][7]);
                strip.pushPoint({ lat: x, lng: y });
            }

            map.addObject(new H.map.Polyline(
              strip, { style: { lineWidth: 4 } }
            ));
          
        }



        var platform = new H.service.Platform({
            app_id: 'DemoAppId01082013GAL',
            app_code: 'AJKnXv84fjrb0KIHawS0Tg',
            useCIT: true
        });
        var defaultLayers = platform.createDefaultLayers();

        //Step 2: initialize a map - this map is centered over Europe
        var map = new H.Map(document.getElementById('map'),
          defaultLayers.normal.map, {
              center: { lat: 28.502269, lng: 77.085958 },
              zoom: 4
          });

        //Step 3: make the map interactive
        // MapEvents enables the event system
        // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
        var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

        // Create the default UI components
        var ui = H.ui.UI.createDefault(map, defaultLayers);

        // Now use the map as required...
        //addMarkersToMap(map);

        function addMarkersToMap(map) {
            var parisMarker = new H.map.Marker({ lat: 48.8567, lng: 2.3508 });
            map.addObject(parisMarker);

            var romeMarker = new H.map.Marker({ lat: 41.9, lng: 12.5 });
            map.addObject(romeMarker);

            var berlinMarker = new H.map.Marker({ lat: 52.5166, lng: 13.3833 });
            map.addObject(berlinMarker);

            var madridMarker = new H.map.Marker({ lat: 40.4, lng: -3.6833 });
            map.addObject(madridMarker);

            var londonMarker = new H.map.Marker({ lat: 51.5008, lng: -0.1224 });
            map.addObject(londonMarker);
        }

    </script>
</asp:Content>

