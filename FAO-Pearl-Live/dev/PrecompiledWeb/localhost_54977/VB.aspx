<%@ page language="VB" autoeventwireup="false" inherits="VB, App_Web_c5kjwoe4" masterpagefile="~/USR.master" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?sensor=false"></script>
     <link href="css/style.css" rel="Stylesheet" type="text/css" />
   <link href="css/ui-lightness/jquery-ui-1.10.2.custom.css" type="text/css" rel="stylesheet" />

<script src="http://code.jquery.com/jquery-1.9.1.js" type="text/javascript"></script>
<script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
<link rel="stylesheet" href="/resources/demos/style.css"/>
    <meta http-equiv="X-UA-Compatible" content="IE=7; IE=EmulateIE9; IE=10" />
		<%--<base href="https://developer.here.com/enterprise/apiexplorer/examples/api-for-js/routing/map-with-route-from-a-to-b.html" />--%>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
    <meta name="description" content="Routing Manager offers the ability to request a route with various modes between two points"/>
		<meta name="keywords" content="routing, services, a to b, route, direction, navigation"/>
		<!-- For scaling content for mobile devices, setting the viewport to the width of the device-->
		<meta name=viewport content="initial-scale=1.0, maximum-scale=1.0, user-scalable=no"/>
		<!-- Styling for example container (NoteContainer & Logger)  -->
		<%--<link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css"/>--%>
		<!-- By default we add ?with=all to load every package available, it's better to change this parameter to your use case. Options ?with=maps|positioning|places|placesdata|directions|datarendering|all -->
		<%--<script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>--%>
		<!-- JavaScript for example container (NoteContainer & Logger)  -->
		<%--<script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>--%>
       <%--<link href="StyleSheet.css" rel="stylesheet" type="text/css" />--%>



        <script src="https://js.api.here.com/v3/3.0/mapsjs-core.js" type="text/javascript" charset="utf-8"></script>
		<script src="https://js.api.here.com/v3/3.0/mapsjs-service.js" type="text/javascript" charset="utf-8"></script>
		<script src="https://js.api.here.com/v3/3.0/mapsjs-mapevents.js" type="text/javascript" charset="utf-8"></script>
		<script src="https://js.api.here.com/v3/3.0/mapsjs-ui.js" type="text/javascript" charset="utf-8"></script>
		<link rel="stylesheet" type="text/css" href="https://js.api.here.com/v3/3.0/mapsjs-ui.css" />
    <link href="css/Default.css" rel="stylesheet" />

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

        .hide {
            display:none;
        }
        .gm-style-iw {
           /*font-weight:bold !important;*/
            text-align:left !important;
        }
    </style>


    <script type="text/javascript">
        var x;
        var ntid;
        var sc;
        var circle;
        var rad = 500;
        var values;
        var MarkerArr = [];
        function Map(value) {
            values = value;
            if (value == "NM") {
                initializeNokiaMap1();
            }
            else if (value == "GM") {
                initialize();
            }
          
        }
        function ChangeRadius() {
          
            sc = document.getElementById("rg").value;
            rad = parseInt(sc);
            var rd = parseFloat(rad);
            rd = (rd / 1000);
            var diss = String(rd);
            
            document.getElementById('radiusvalue').innerText = diss + " Km";
            if (values == "NM") {
                initializeNokiaMap1();
            }
            else if (values == "GM") {
                initialize();
            }
        }

       var InfoWindowVar = null;
        function initialize() {
           
            var latlong;
            $("#dvMap").html("");
                sc = document.getElementById("rg").value;
                rad = parseInt(sc);
                var rad1 = parseFloat(rad);
                rad1 = (rad1 / 1000);
                var dis = String(rad1);
                document.getElementById('radiusvalue').innerText = dis + " Km";

                map = new google.maps.Map(document.getElementById('dvMap'), { zoom: 5, center: new google.maps.LatLng(19.8761653, 75.3433139), mapTypeId: google.maps.MapTypeId.ROADMAP });
                var infowindow = new google.maps.InfoWindow();

                var marker;
                var markers = JSON.parse('<%=ConvertDataTabletoString() %>');
                MarkerArr.length = markers.length;
                if (markers.length > 0)
                {
                    for (i = 0; i < markers.length-1; i++) {
                        latlong = markers[i].GeoPoint.split(",");
                        
                        var marker = new google.maps.Marker({ position: new google.maps.LatLng(latlong[0], latlong[1]), tid: markers[i].tid, map: map,  size: new google.maps.Size(1, 1) });

                        google.maps.event.addListener(marker, 'click', (function (marker, i) {

                            var strInfo = "Loading...";

                            return function () {

                                if (InfoWindowVar != null) {
                                    InfoWindowVar.close();
                                    infowindow.setContent(strInfo);
                                    infowindow.open(map, marker);
                                    InfoWindowVar = infowindow;
                                }
                                else {
                                    infowindow.setContent(strInfo);
                                    infowindow.open(map, marker);
                                    InfoWindowVar = infowindow;
                                }
                                var fldid = document.getElementById("hdnFld").value;
                                var doc = document.getElementById("hdnDoc").value;

                                $.ajax({
                                    type: "post",
                                    url: "VB.aspx/GetInfo",
                                    data: "{Tid : " + marker.tid + ", Doc : '" + doc + "', FldId:" + fldid + "}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (result) {
                                        strInfo = result.d;
                                        if (InfoWindowVar != null) {
                                            InfoWindowVar.close();
                                            infowindow.setContent(strInfo);
                                            infowindow.open(map, marker);
                                            InfoWindowVar = infowindow;
                                        }
                                        else {
                                            infowindow.setContent(strInfo);
                                            infowindow.open(map, marker);
                                            InfoWindowVar = infowindow;
                                        }
                                    },
                                    error: function (result) {
                                        strInfo = result.d;
                                    }
                                });
                            }

                        })(marker, i));

                    }
                }
                else
                {
                    latlong = markers[i].GeoPoint.split(",");
                    var data = markers[i]
                    var mapOptions = {
                        center: new google.maps.LatLng(28.61, 77.23),
                        zoom: 6,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                       // tid: markers[i].tid
                    };
                }
                //var infoWindow = new google.maps.InfoWindow();
                //var map = new google.maps.Map(document.getElementById("dvMap"), mapOptions);
                //var Buttonid = '<asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" onc Height="16px" Width="16px" OnClick="EditHit" AlternateText="Edit" OnClientClick="Googletid()" />'
                //var Buttonid2 = '<asp:ImageButton ID="btnDtl" ImageUrl="~/images/lock.png"  runat="server" Height="16px" Width="16px" OnClick="LockHit"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" OnClientClick="Googletid()" />'
          
                //for (i = 0; i < markers.length; i++) {

                
                //    //data1 = String.concat("Lat :", markers[i].fld15, "", '</br>', "Long:", markers[i].fld16, '</br>', "Area Name:", markers[i].fld1, '</br>', "Spread Name:", markers[i].fld10, '</br>', "Office Address:", markers[i].fld11, '</br>', "Pin Code:", markers[i].fld12, '</br>', "State:", markers[i].fld13, '</br>', "City:", markers[i].fld14)
                //    latlong = markers[i].GeoPoint.split(",");
                //    var myLatlng = new google.maps.LatLng(latlong[0], latlong[1]);
                //    marker = new google.maps.Marker({
                //        position: myLatlng,
                //        map: map,
                //        // title: data.title,
                //        id: markers[i].tid
                //    });
               
                //    // marker.html = "Lat :" + latlong[0] + "" + '</br>' + "Long:" + latlong[1] + "" + '</br>' + "Area Name:" + markers[i].fld1 + "" + '</br>' + "Spread Name:" + markers[i].fld10 + "" + '</br>' + "Office Address:" + markers[i].fld11 + "" + '</br>' + "Pin Code:" + markers[i].fld12 + "" + '</br>' + "State:" + markers[i].fld13 + "" + '</br>' + "City:" + markers[i].fld14 + "" + '</br>' + Buttonid + Buttonid2
                //    google.maps.event.addListener(marker, 'click', function () {
                //        infoWindow.setContent(this.html);
                //        infoWindow.open(map, this);
                //        x = this.id;
                        
                //    });             
               
                //}
                circle = new google.maps.Circle({
                    map: map,
                    clickable: false,
                    draggable: true,
                    // metres
                    radius: rad,
                    fillColor: '#FFE6E6',
                    fillOpacity: .6,
                    strokeColor: '#313131',
                    strokeOpacity: .6,
                    strokeWeight: .8,
                
                
                });
            
            
                circle.bindTo('center', marker, 'position');           
                //for (i = 0; i < markers.length; i++) {
                //   var dist= getDistanceFromLatLonInKm(markers[0].fld15, markers[0].fld16, markers[i + 1].fld15, markers[i + 1].fld16);
                //   if (dist < 500 || dist == 500)
                //   {
                //       //for (var j = 0; j < MarkerArr.length; j++) {
                //           //MarkerArr.push(markers[i + 1].fld1);
                //       if (MarkerArr.contains(markers[i + 1].fld1)) {
                //           MarkerArr.push(markers[i + 1].fld1);
                //       }
                //       else {
                //           alert("hi");
                //       }
                //           }
                //       }
                //       document.getElementById('lblCity').innerHTML = MarkerArr.length;
            
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
        function initializeNokiaMap() {
     
            var latlong;
            
            $("#dvMap").html("");
               
                var imageMarker;
                var markers = JSON.parse('<%=ConvertDataTabletoString() %>');            

            nokia.Settings.set("app_id", "FhrHxdDSWojustuTPwwL");
            nokia.Settings.set("app_code", "-DMrq8Tm98ut9TA3-wSnOA");
                nokia.Settings.set("serviceMode", "cit");
                (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

                var mapContainer = document.getElementById("dvMap");
                var infoBubbles = new nokia.maps.map.component.InfoBubbles();
                var map = new nokia.maps.map.Display(mapContainer, {
                    center: [28.610, 77.23],
                    zoomLevel:6,

                    components: [infoBubbles,
                                   new nokia.maps.map.component.ZoomBar(),
               new nokia.maps.map.component.Traffic(),
                                new nokia.maps.map.component.Behavior(),
                 new nokia.maps.map.component.TypeSelector()]
                });
                var redMarker;
                var markersContainer = new nokia.maps.map.Container();
                var Buttonid = '<span>   <asp:ImageButton ID="btnEdit1" runat="server" ImageUrl="~/images/edit.jpg" onc Height="16px" Width="16px" OnClick="EditHit" AlternateText="Edit" OnClientClick="Nokiatid()" /></span>'
                var Buttonid2 = '<span>   <asp:ImageButton ID="btnDtl1" ImageUrl="~/images/lock.png"  runat="server" Height="16px" Width="16px" OnClick="LockHit"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" OnClientClick="Nokiatid()" /></span>'

                for (var i = 0; i < markers.length; i++)
                {
                    latlong = markers[i].GeoPoint.split(",");
                    var x = parseFloat(latlong[0]);
                    var y = parseFloat(latlong[1]);
                    var imageMarker = new nokia.maps.map.Marker([x ,y], {
                        icon: "images/Nokia2.png",
                        dragable: true,
                        position: [x, y],                     
                        anchor: new nokia.maps.util.Point(1, 1),
                        $html: markers[i].tid
                    }),

                    image2Marker = new nokia.maps.map.Marker([x, y]);
                    map.objects.addAll([imageMarker]);
                    map.objects.add(markersContainer);
                    markersContainer.addListener("mouseout", function (evt) {
                        document.body.style.cursor = "default";
                    });
                    // imageMarker.html = "Lat :"+ markers[i].fld15+ ""+ '</br>'+ "Long:"+ markers[i].fld16 +""+ '</br>'+ "Area Name:"+ markers[i].fld1+ ""+ '</br>'+ "Spread Name:"+ markers[i].fld10+""+ '</br>'+  "Office Address:"+ markers[i].fld11+""+ '</br>'+"Pin Code:"+ markers[i].fld12+""+ '</br>'+  "State:"+ markers[i].fld13+""+ '</br>'+ "City:"+ markers[i].fld14+  Buttonid+ Buttonid2
                    var TOUCH = nokia.maps.dom.Page.browser.touch,
                      CLICK = TOUCH ? "tap" : "click";
                    imageMarker.addListener(CLICK,function (evt) {
                        infoBubbles.openBubble(this.html, this.coordinate);
                        ntid = (evt.target.$html);
                    });
                }
                if (markers.length > 0) {
                    for (var i = 0; i < 1; i++) {
                        latlong = markers[i].GeoPoint.split(",");
                        var map = new nokia.maps.map.Display(mapContainer, {
                            center: [latlong[0], latlong[1]],
                            zoomLevel: 6,

                            components: [infoBubbles,
                                           new nokia.maps.map.component.ZoomBar(),
                       new nokia.maps.map.component.Traffic(),
                                        new nokia.maps.map.component.Behavior(),
                         new nokia.maps.map.component.TypeSelector()]
                        });
                        // Transparent aqua colored circle with a red border
                        map.objects.add(new nokia.maps.map.Circle(
                        // The center or the circle will be on longitude 50.0361, latitude  8.5619
                        new nokia.maps.geo.Coordinate(latlong[0], latlong[1]),
                        // The radius of the circle in meters
                        rad,
                        {

                            pen: {
                                fillColor: '#FFE6E6',
                                fillOpacity: .6,
                                strokeColor: '#313131',
                                strokeOpacity: .6,
                                strokeWeight: .8,
                                lineWidth: 2
                            },
                            brush: {

                                fillColor: '#FFE6E6',
                                fillOpacity: .6,
                                strokeColor: '#313131',
                                strokeOpacity: .6,
                                strokeWeight: .8,
                                draggable: true,
                                fillOpacity: .6
                            }

                        }


                    ));
                    }
                }
                else
                {
                    var map = new nokia.maps.map.Display(mapContainer, {
                        center: [28.610, 77.23],
                        zoomLevel: 6,

                        components: [infoBubbles,
                                       new nokia.maps.map.component.ZoomBar(),
                   new nokia.maps.map.component.Traffic(),
                                    new nokia.maps.map.component.Behavior(),
                     new nokia.maps.map.component.TypeSelector()]
                    });
                    // Transparent aqua colored circle with a red border
                    map.objects.add(new nokia.maps.map.Circle(
                    // The center or the circle will be on longitude 50.0361, latitude  8.5619
                    new nokia.maps.geo.Coordinate(28.610, 77.23),
                    // The radius of the circle in meters
                    rad,
                    {

                        pen: {
                            fillColor: '#FFE6E6',
                            fillOpacity: .6,
                            strokeColor: '#313131',
                            strokeOpacity: .6,
                            strokeWeight: .8,
                            lineWidth: 2
                        },
                        brush: {

                            fillColor: '#FFE6E6',
                            fillOpacity: .6,
                            strokeColor: '#313131',
                            strokeOpacity: .6,
                            strokeWeight: .8,
                            draggable: true,
                            fillOpacity: .6
                        }

                    }


                ));

                }
               
                //for (i = 0; i < markers.length; i++) {
                //    var dist = getDistanceFromLatLonInKm(markers[0].fld15, markers[0].fld16, markers[i + 1].fld15, markers[i + 1].fld16);
                 
                //    if (dist < 500 || dist == 500) {
                //        alert("hello");
                //    }
                //}
           

        }


        var nmap;
        var mapContainer;
        var platform;
        var maptileService;
        var maptypes;
        var hidpi;
        var info;
        function initializeNokiaMap1() {
            var latlong;

            $("#dvMap").html("");

            mapContainer = document.getElementById('dvMap');
            platform = new H.service.Platform({
                app_id: 'FhrHxdDSWojustuTPwwL',
                app_code: '-DMrq8Tm98ut9TA3-wSnOA',
                useCIT: true,
                useHTTPS: true,
            });
            maptileService = platform.getMapTileService({ 'type': 'base' });
            maptypes = platform.createDefaultLayers(hidpi ? 512 : 256, hidpi ? 320 : null);
            map = new H.Map(mapContainer, maptypes.normal.map,
                {
                    center: new H.geo.Point(21.2597, 77.5114),
                    zoom: 4
                }
            );

            new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
            var ui = H.ui.UI.createDefault(map, maptypes);
            window.addEventListener('resize', function () { map.getViewPort().resize(); });

            var imageMarker;
            var markers = JSON.parse('<%=ConvertDataTabletoString() %>');  
            var HmapIcon = { markerImg: new H.map.Icon("images/Nokia2.png") }

            for (var i = 0; i < markers.length; i++) {
                latlong = markers[i].GeoPoint.split(",");
                var x = parseFloat(latlong[0]);
                var y = parseFloat(latlong[1]);
                if (isNaN(x) == true || isNaN(y) == true)
                { continue; }
                var marker = new H.map.Marker(new mapsjs.geo.Point(x, y),
                    {
                        icon: HmapIcon['markerImg']
                    });
                marker.setData({
                    tid: markers[i].tid,
                });

                marker.addEventListener('tap', function (evt) {

                    var strInfo = "Loading...";

                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                        content: strInfo//evt.target.getData()
                    });

                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
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


                    var fldid = document.getElementById("hdnFld").value;
                    var doc = document.getElementById("hdnDoc").value;

                    $.ajax({
                        type: "post",
                        url: "VB.aspx/GetInfo",
                        data: "{Tid : " + evt.target.getData().tid + ", Doc : '" + doc + "', FldId:" + fldid + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            strInfo = result.d;
                            var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });

                            var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
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
                        },
                        error: function (result) {
                            strInfo = result;
                        }
                    });

                }, false);
                marker.setVisibility(true);
                map.addObject(marker);
            }

        }





        function Googletid() {
            var hdnvalue = document.getElementById('<%= hdntid.ClientID%>');
              hdnvalue.value = x;
        }
        function Nokiatid() {
            var hdnvalueNokia = document.getElementById('<%= hdntid.ClientID%>');
            hdnvalueNokia.value = ntid;
         }

    </script>

    <asp:HiddenField ID="hdnDoc" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnFld" runat="server" Value="0" ClientIDMode="Static" />
    <asp:Panel ID="PnlMap" runat="server">
        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
            border="0px">
            <tr>
                <td style="width: 210px;"></td>

                <td style="width: 210px; text-align:left;">
                    The Radius of Circle is : <span id="radiusvalue"></span>
                </td>



                <td style="width: 210px;">
                    <input type="range" id="rg" min="500" max="1500" onchange="ChangeRadius();"  />
                   
                </td>

                <td style="width: 210px;">
                    <div>
                        <asp:Label ID="Map" Text="Choose Map" runat="server"></asp:Label>
                        <asp:DropDownList ID="DropDownList1" runat="server" onchange="javascript:Map(this.value);">
                            <asp:ListItem  Value="NM" Text="Nokia Map" ></asp:ListItem>
                            <asp:ListItem Value="GM" Text ="Google Map" ></asp:ListItem>
                        </asp:DropDownList>
                        <asp:HiddenField ID="hdntid" runat="server" />
                    </div>
                </td>

                <td style="text-align: right; width: 25px">
                    <asp:Button ID="btnsavedata" runat="server" Text="Save" OnClick="btnsavedata_Click" Visible="false"  />
                    <asp:Button ID="btnchangeview" runat="server" Text="ChangeView" CssClass="btnNew" Font-Bold="True" />
                </td>
            </tr>
        </table>
        <div id="dvMap" style="width: 1000px; height: 700px;"></div>        
          <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
            border="0px">
              
            <tr>
                 <th>Markers Details</th>
            </tr>
             <tr>
                <td style="width: 210px;">    <label id="lblCity"></label></td>
                  
                <td style="width: 210px;"></td>



                <td style="width: 210px;"></td>

                <td style="width: 210px;">
                   
                </td>

                <td style="text-align: right; width: 25px">
                   
                </td>
                 </tr>
        </table>
    </asp:Panel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="900px" Style="display: none" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updPanalHeader" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h3>
                                    <asp:Label ID="lblHeaderPopUp" runat="server" Font-Bold="True"></asp:Label></h3>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <%--<Triggers > <asp:PostBackTrigger ControlID ="btnActEdit" /></Triggers>--%>
                            <ContentTemplate>
                                <asp:Panel ID="pnlPoupup" Width="900px" runat="server">
                                    <asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </asp:Panel>
                                <asp:Panel ID="pnlFields" Width="900px" Height="500px" runat="server" Style="overflow: scroll;">
                                </asp:Panel>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActEdit" runat="server" Text="Save"
                                        OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnLock" runat="server" Style="Display: none" />
    <asp:ModalPopupExtender ID="ModalPopup_Lock" runat="server"
        TargetControlID="btnLock" PopupControlID="pnlPopupLock"
        CancelControlID="btnCloseLock" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupLock" runat="server" Width="500px" Style="Display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Lock / Unlock : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseLock" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updLock" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblLock" runat="server" Font-Bold="True" ForeColor="Red"
                                    Width="440px" Font-Size="X-Small"></asp:Label>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnLockupdate" runat="server" Text="Yes Lock" Width="90px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        OnClick="LockRecord" CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                                <asp:Label ID="lblRecord" runat="server" ForeColor="red"
                                    Font-Size="Small"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
