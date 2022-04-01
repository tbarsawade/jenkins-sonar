<%@ Page Title="" Language="VB" MasterPageFile="~/PublicMaster.master" AutoEventWireup="false" CodeFile="TestCluster.aspx.vb" Inherits="TestCluster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

     <link href="css/style.css" rel="Stylesheet" type="text/css" />
<link rel="stylesheet" type="text/css" href="/resources/demos/style.css"/>
<link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css"/>

<script src="http://code.jquery.com/jquery-1.9.1.js" type="text/javascript"></script>
<script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
<script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false"></script> 
<script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>
<script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>
<script src="Scripts/NokiaGoogleMap1.js" type="text/javascript"></script>
    <script src="js/markerclusterer.js"></script>
<link href="StyleSheet.css" rel="stylesheet" type="text/css" />



    <script type="text/javascript">

        var myMarkersArray = [];
        var RemovedMarkers = myMarkersArray.slice(0);

        $(document).ready(function () { ShowMap(); });
        function ShowMap() {
            $.ajax({
                type: "post",
                url: "Test.aspx/GetMarkerListJSON",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) { chk(data); },
                error: function (data) { chk(data); }
            });
        }
        function chk(abc) {
            var str = abc.d;
            if (abc.d == null) {

            }
            else {
                var v = str.substring(1, str.lastIndexOf(","));
                var lastChar = str.slice(-1);
                if (lastChar == ',') {
                    str = str.slice(0, -1);
                }
                str = str + ']';

                var obj = JSON.parse(str);

                $.each(obj, function (i, value) {

                    if (value.Site.toUpperCase() == 'HUB') {

                        this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        this['img'] = "http://www.myndsaas.com/images/darkyellow.png";
                        this['Group'] = value.Group;
                        this['SiteID'] = value.SiteID;
                        this['Site_Name'] = value.Site_Name;
                        this['Cluster'] = value.Cluster;
                        this['latitude'] = value.Lat;
                        this['longitude'] = value.Long;
                        // this['IsCluster'] = contains ? true : false;

                        //icon = "http://www.myndsaas.com/images/darkyellow.png";
                        //JLineCoOrdinate = JLineCoOrdinate + "['SiteID : " + value.SiteID + "<br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + " <br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "'," + value.Lat + "," + value.Long + ",'" + icon + "','" + value.Group + "','" + value.SiteID + "','" + value.Site_Name + "'],";
                    }
                    else if (value.Site.toUpperCase() == 'BSC') {

                        this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        this['img'] = "http://www.myndsaas.com/images/lightblue.png";
                        this['Group'] = value.Group;
                        this['SiteID'] = value.SiteID;
                        this['Site_Name'] = value.Site_Name;
                        this['Cluster'] = value.Cluster;
                        this['latitude'] = value.Lat;
                        this['longitude'] = value.Long;
                        // this['IsCluster'] = contains ? true : false;

                        //icon = "http://www.myndsaas.com/images/lightblue.png";
                        //JLineCoOrdinate = JLineCoOrdinate + "['SiteID : " + value.SiteID + "<br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + " <br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "'," + value.Lat + "," + value.Long + ",'" + icon + "','" + value.Group + "','" + value.SiteID + "','" + value.Site_Name + "'],";
                    }
                    else if (value.Site.toUpperCase() == 'STRATEGIC') {

                        this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        this['img'] = "http://www.myndsaas.com/images/blue.png";
                        this['Group'] = value.Group;
                        this['SiteID'] = value.SiteID;
                        this['Site_Name'] = value.Site_Name;
                        this['Cluster'] = value.Cluster;
                        this['latitude'] = value.Lat;
                        this['longitude'] = value.Long;

                        //this['IsCluster'] = contains ? true : false;
                    }
                    else if (value.Site.toUpperCase() == 'NON STRATEGIC') {

                        this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        this['img'] = "http://www.myndsaas.com/images/darkk.png";
                        this['Group'] = value.Group;
                        this['SiteID'] = value.SiteID;
                        this['Site_Name'] = value.Site_Name;
                        this['Cluster'] = value.Cluster;
                        this['latitude'] = value.Lat;
                        this['longitude'] = value.Long;
                        // this['IsCluster'] = contains ? true : false;

                    }

                });

                ReportMaster(obj);
                // clusterDataPoints(obj);

            }


        }


        var mapContainer;
        var map;
        function ReportMaster(str) {

            var test = 0;
            try {
                //var data = JSON.parse(str);
                var data = str;
                $('#dvMap').html('');
                document.getElementById("dvMap").style.display = "block";
                nokia.Settings.set("app_id", "VG3IAYYwc1Y7XaBWEqU9");
                nokia.Settings.set("app_code", "R7W2h1KNHKBqJsJOkRbTiw");
                nokia.Settings.set("serviceMode", "cit");
                (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

                mapContainer = document.getElementById("dvMap");
                var infoBubbles = new nokia.maps.map.component.InfoBubbles();

                map = new nokia.maps.map.Display(mapContainer, {
                    center: [19.8761653, 75.3433139],
                    zoomLevel: 7,

                    components: [infoBubbles,
                                   new nokia.maps.map.component.ZoomBar(),
               new nokia.maps.map.component.Traffic(),
                                new nokia.maps.map.component.Behavior(),
                 new nokia.maps.map.component.TypeSelector()]

                });

                var redMarker;
                var markersContainer = new nokia.maps.map.Container();

                // var Buttonid = '<input type="image" ID="btnEdit1" src="././images/edit.jpg" style="height:26px;Width:75px;" class="click"  AlternateText="Edit" />';
                // var Buttonid2 = '<input type="image" ID="btnDtl1" src="././images/lock.png" style="height:26px;Width:75px;" class="click"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';

                var icon1 = "http://www.myndsaas.com/images/darkyellow.png";


                for (var i = 0; i < data.length - 1; i++) {
                    test += 1;

                    var x = parseFloat(data[i].Lat);
                    var y = parseFloat(data[i].Long);

                    if (i > 6000) {
                        icon1 = "http://www.myndsaas.com/images/blue.png";
                    }


                    if (isNaN(x) == true || isNaN(y) == true)
                    { continue; }

                    var imageMarker = new nokia.maps.map.Marker([x, y], {
                        icon: data[i].img,
                        dragable: true,
                        position: [x, y],
                        anchor: new nokia.maps.util.Point(1, 1),
                        $html: i
                    }),



                    image2Marker = new nokia.maps.map.Marker([x, y]);
                    map.objects.addAll([imageMarker]);
                    map.objects.add(markersContainer);
                    markersContainer.addListener("CLICK", function (evt) {
                        document.body.style.cursor = "default";
                    });
                    imageMarker.html = "<span style='font-weight:bold;'> SiteID : " + data[i].SiteID + "</span><br>Site : " + data[i].Site + "<br>Site Name : " + data[i].Site_Name + "<br>OandM Head: " + data[i].OandM_Head + " <br> Maintenance Head : " + data[i].Maintenance_Head + " <br>Opex Manager : " + data[i].Opex_Manager + "<br>Security Manager : " + data[i].Security_Manager + "<br>Zonal Head : " + data[i].Zonal_Head + "<br>Cluster Manager :" + data[i].Cluster_Manager + "<br>Supervisor : " + data[i].Supervisor + "<br>Technician : " + data[i].Technician + "<br>No of OPCOs : " + data[i].No_of_OPCOs + " <br>Anchor OPCO : " + data[i].Anchor_OPCO + "";
                    var TOUCH = nokia.maps.dom.Page.browser.touch,
                    CLICK = TOUCH ? "tap" : "click";
                    imageMarker.addListener(CLICK, function (evt) {
                        infoBubbles.openBubble(this.html, this.coordinate);
                        ntid = (evt.target.$html);
                    });
                }

                clusterDataPoints(data);
            }
            catch (e) {
                alert(e.message + ', ' + test);
            }
        }


        var markerCluster;
        var addCount;
        var markers = [];

        function clusterDataPoints(data) {

            $('#dvMap').html('');

            addCount = data.length;

            document.getElementById("dvMap").style.display = "block";
            nokia.Settings.set("app_id", "VG3IAYYwc1Y7XaBWEqU9");
            nokia.Settings.set("app_code", "R7W2h1KNHKBqJsJOkRbTiw");
            nokia.Settings.set("serviceMode", "cit");
            (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

            mapContainer = document.getElementById("dvMap");

            map = new nokia.maps.map.Display(mapContainer, {
                "components": [new nokia.maps.map.component.ZoomBar(), new nokia.maps.map.component.Behavior(), new nokia.maps.map.component.TypeSelector()],
                "center": [19.8761653, 75.3433139],
                "zoomLevel": 7
            });

            for (i = 0; i < data.length - 1; i++) {
                addCount--;
                //var infoBubbles = new nokia.maps.map.component.InfoBubbles();


                var x = parseFloat(data[i].Lat);
                var y = parseFloat(data[i].Long);

                if (isNaN(x) == true || isNaN(y) == true)
                { continue; }

                var imageMarker = new nokia.maps.map.Marker([x, y], {
                    icon: data[i].img,
                    dragable: true,
                    position: [x, y],
                    anchor: new nokia.maps.util.Point(1, 1),
                    $html: i
                });

                //var marker = new nokia.maps.map.StandardMarker(randomPointNear(lat, long, spread));
                markers.push(imageMarker);


                //var clusterProvider = new nokia.maps.clustering.ClusterProvider(map, {
                //    eps: 16,
                //    minPts: 1,
                //    dataPoints: data
                //});

                //this.data = data;

                //clusterProvider.cluster();

                if (addCount == 0) {


                }
            }

            //alert('Hello');
            markerCluster = new MarkerClusterer();
            markerCluster.objects.addAll(markers);
            map.addComponent(markerCluster);


        }


        function CreatePolygonNokiaReports(geopoints) {
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

            nokia.Settings.set("app_id", "DemoAppId01082");

            nokia.Settings.set("app_code", "AJKnXv84fjrb0KIHawS0Tg");

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

                        imageMarker.html = "<table width='100%'><tr><td style='width:50%;text-align:left;color:white;'>" + "Product Name :</td>" + '<td style="width:50%;text-align:left;color:white;">' + geopoints[i]["product Name"] + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;'>Owner Name:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Owner First Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Branch:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i].Branch + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Advisor Code:</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Advisor Code"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;'>Advisor Name</td><td style='width:50%;text-align:left;color:white;'>" + geopoints[i]["Advisor Name"] + "</td></tr></table>"

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


        var myVals = new Array();

        function CacheValues() {
            var l = document.getElementById('<%= LstVehicle.ClientID%>');

            for (var i = 0; i < l.options.length; i++) {

                myVals[i] = l.options[i];
            }
            return;
        }

        function focusMarker(sender) {
            var v = $(sender).attr("value");
            for (i = 0; i < myMarkersArray.length; i++) {
                if ((myMarkersArray[i].primaryKey == v)) {

                    google.maps.event.trigger(myMarkersArray[i], 'click')
                    map.setCenter(myMarkersArray[i].getPosition());
                    myMarkersArray[i].setVisible(true);
                    return false;
                }
            }

            //for (i = 0; i < RemovedMarkers.length; i++) {
            //    if ((RemovedMarkers[i].primaryKey == v)) {

            //        google.maps.event.trigger(RemovedMarkers[i], 'click')
            //        map.setCenter(RemovedMarkers[i].getPosition());
            //        RemovedMarkers[i].setVisible(true);
            //        return false;

            //    }
            //}
        }

        function SearchList() {
            // alert('hello');
            var l = document.getElementById('<%= LstVehicle.ClientID%>');
            var tb = document.getElementById('VehSearch');
            var strlb = String;
            if (!String.prototype.startsWith) {
                String.prototype.startsWith = function (str) {
                    return !this.indexOf(str);
                }
            }

            l.options.length = 0;

            if (tb.value == "") {
                for (var i = 0; i < myVals.length; i++) {
                    var colr = myVals[i].style.color;

                    var op = '<option onclick="focusMarker(this);" value=' + myVals[i].value + ' style="color:' + colr + '" >' + myVals[i].text + '</option>';
                    $('#<%= LstVehicle.ClientID%>').append(op);

                }
            }
            else {


                for (var i = 0; i < myVals.length; i++) {

                    if (myVals[i].text.toLowerCase().match(tb.value.toLowerCase())) {

                        var colr = myVals[i].style.color;

                        var op = '<option  onclick="focusMarker(this);" value=' + myVals[i].value + '  style="color:' + colr + '" >' + myVals[i].text + '</option>';
                        $('#<%= LstVehicle.ClientID%>').append(op);

                    }
                    else {
                        // do nothing

                    }
                }
            }
        }


</script>



    

<div>
        
        <input type="hidden" id="hdndata" value="" RefreshTime="" />
        <asp:HiddenField ID="hdnRefTime" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdnCluster" Value="" runat="server" ClientIDMode="Static" />

        <asp:UpdatePanel ID="updPnlGrid" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="Circle" />
                <asp:PostBackTrigger ControlID="chkvtype" />
            </Triggers>
            <ContentTemplate>
                 <table style="width: 100%; height: 670px;">
                    <tr>
                        <td style="width: 13%; height: 670px; vertical-align: top;">
                            <div style="width: 100%; height: 770px;">
                               <%-- <fieldset style="height: 670px">--%>
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">Select Site</legend>
                                    <asp:Panel ID="Panel2" runat="server" Height="80px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="false" OnTextChanged="FilterSite">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                    <br />
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">Select Vehicle Type</legend>
                                    <asp:Panel ID="Panel1" runat="server" Height="100px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="chkvtype" runat="server" AutoPostBack="false" OnTextChanged="FilterSite">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                
                                
                                
                                <br />
                                    <asp:Panel ID="Panel3" runat="server" Height="210px"  Style="display: block" ScrollBars="Auto">
                                        <legend style="color: Black; text-align: Left; font-weight: bold;">Search Vehicle</legend>
                                        <div>
                                            <input type="text" id="VehSearch" placeholder="Type Vechicle name" style="width:95%;" onkeyup="return SearchList();" />
                                        </div>
                                        <div >
                                        <asp:ListBox ID="LstVehicle" onchange="focusMarker(this);" runat="server" class="lstContent" Height="150px" Width="98%" DataTextField="Vehicle">
                                        </asp:ListBox>
                                            
                                        </div>
                                    </asp:Panel>

                                <legend style="color: Black; text-align: Left; font-weight: bold;">Search Sites</legend>
                                        <input type="text" id="txtSearchV" style="width:96%; " placeholder="Type Site Name" onkeyup="return Gmap.SearchSite(this,'#idSite');"  />
                                    <asp:Panel ID="Panel4" runat="server" Height="70px" Width="130px" Style="display: block; border:1px solid #CCCCCC;" ScrollBars="Vertical">
                                        

                                        <table style="width:98%;">
                                            <tbody id="idSite">

                                            </tbody>
                                        </table>

                                        </asp:Panel>
                                
                            </div>
                        </td>
                        <%--</fieldset>--%>
                        <td style="width: 87%; height: 770px;" valign="top" >
                            <div id="dvMap" style="width: 100%; text-align:center;vertical-align:middle ;height: 100%;">
                                <div style="width:100%; height:760px; background-color: rgba(104, 149, 166, 0.3);  ">
                                     <img src="images/MapLoading.gif" height="50%" width="50%" />
                                </div>
                               
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </div>
                        </td>
                    </tr>
                    <tr valign="top" >
                        <td valign="top" >
                        </td>
                        <td valign="top"  >
                         <table cellspacing="0" celpadding="0" width="100%" border="1">
                             <colgroup style="width:10%; "></colgroup>
                             <colgroup style="width:40%"></colgroup>
                             <colgroup style="width:10%; "></colgroup>
                             <colgroup style="width:40%"></colgroup>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image5" runat="server" ImageUrl="~/images/blue.png" />
                                    </td>
                                    <td>
                                        &nbsp; Strategic</td>
                                    <td>
                                        <asp:Image ID="Image9" runat="server" ImageUrl="~/images/truck_red.png" />
                                    </td>
                                    <td>
                                        &nbsp;  
                                        No Warehouse Vehicle Data more than 24 Hrs.
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image6" runat="server" ImageUrl="~/images/darkyellow.png" />
                                    </td>
                                    <td>
                                        &nbsp; Normal Hub</td>
                                     <td>
                                        <asp:Image ID="Image10" runat="server" ImageUrl="~/images/truck_green.png" />
                                    </td>
                                    <td>
                                        &nbsp;Warehouse Vehicle is Moving</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image7" runat="server" ImageUrl="~/images/darkk.png" />
                                    </td>
                                    <td>
                                        &nbsp; Non Strategic</td>
                                    <td>
                                        <asp:Image ID="Image11" runat="server" ImageUrl="~/images/truck_gray.png" />
                                    </td>
                                    <td>
                                        &nbsp;Warehouse Vehicle is Halted 10 minutes to 4 Hours</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image8" runat="server" ImageUrl="~/images/lightblue.png" />
                                    </td>
                                    <td>
                                        &nbsp; BSC</td>
                                    <td>
                                        <asp:Image ID="Image12" runat="server" ImageUrl="~/images/truck_pink.png" />
                                    </td>
                                    <td>
                                        &nbsp;Warehouse Vehicle is Halted more than 4 Hours</td>
                                </tr>
                                <tr>
                                    <td >
                                        <asp:Image ID="img" runat="server" ImageUrl="~/images/car1.png" />
                                    </td>
                                    <td >
                                        &nbsp; No Vehicle Data more than 24 Hrs.</td>
                                    <td >
                                        <asp:Image ID="Image13" runat="server" ImageUrl="~/images/dfred.png" />
                                    </td>
                                    <td >
                                        &nbsp; No Diesel Filing Vehicle Data more than 24 Hrs.</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/car2.png" />
                                    </td>
                                    <td>
                                        &nbsp; Vehicle is Moving</td>
                                    <td>
                                        <asp:Image ID="Image14" runat="server" ImageUrl="~/images/dfgreen.png" />
                                    </td>
                                    <td>
                                        &nbsp;Diesel Filing Vehicle is Moving</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/car4.png" />
                                    </td>
                                    <td>
                                        &nbsp; Vehicle is Halted 10 minutes to 4 Hours</td>
                                    <td>
                                        <asp:Image ID="Image15" runat="server" ImageUrl="~/images/dfgray.png" />
                                    </td>
                                    <td>
                                        &nbsp;Diesel Filing Vehicle is Halted 10 minutes to 4 Hours</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image4" runat="server" ImageUrl="~/images/car5.png" />
                                    </td>
                                    <td>
                                        &nbsp; Vehicle is Halted more than 4 Hours</td>
                                    <td>
                                        <asp:Image ID="Image16" runat="server" ImageUrl="~/images/dfpink.png" />
                                    </td>
                                    <td>
                                        &nbsp;Diesel Filing Vehicle is Halted more than 4 Hours</td>
                                </tr>
                            </table>
                          </td>
                    </tr>
                </table>
                </ContentTemplate>
        </asp:UpdatePanel>
               </div>
   
</asp:Content>

