<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="LastLocation, App_Web_0xvfyc51" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
     <link href="css/GridviewScroll.css" rel="stylesheet" />
       <script src="js/gridviewScroll.min.js"></script>
   
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
        
  
 
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>

    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false" type="text/javascript"></script>
    <script type="text/javascript">

        function toggleDiv(divId) {
            $("#" + divId).toggle();
        }
        function HideDiv(divId) {
            $("#" + divId).Hide();
        }
    </script>
    <script src="http://maps.google.com/maps/api/js?sensor=false"
        type="text/javascript"></script>
    <script type="text/javascript">

        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }

      
    </script>
    <style type="text/css">
    .gradientBoxesWithOuterShadows {
            height: 100%;
            width: 99%;
            padding: 5px;
            background-color: white;
            /* outer shadows  (note the rgba is red, green, blue, alpha) */
            -webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4);
            -moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);
            /* rounded corners */
            -webkit-border-radius: 12px;
            -moz-border-radius: 7px;
            border-radius: 7px;
            /* gradients */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5));
            background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%);
            background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
            background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
        }

        .style2 {
            width: 30%;
        }
    </style>
    <script type="text/javascript">
        var imeinumber = 0;
        var cirl = 0;
        var cty = 0;
    
     function GetUsers123(a, b)
     {
         if (b.checked == true)
         {
            imeinumber = imeinumber +","+ a;
         }
     }
   
     var eid

     function ShowMap()
     {
           eid= '<%= Session("EID")%>';
            var cir = document.getElementById("ContentPlaceHolder1_hdcir").value;
            var city = document.getElementById("ContentPlaceHolder1_hdcity").value;
            var uid = '<%= Session("UID")%>';
            var uRole = '<%= Session("USERROLE")%>';
       
            if (String(document.getElementById("ContentPlaceHolder1_hdimieno").value).length>1) 
            {
                imeinumber = document.getElementById("ContentPlaceHolder1_hdimieno").value;
            }
                      
            document.getElementById("divdata").style.display = "none";
            $.ajax({
                type: "post",
                url: "GPSLastLocation1.aspx/DrawMap",
                data: "{'eid':'" + eid + "','cir':'" + cir + "','cit':'" + city + "','Imieno':'" + String(imeinumber).replace("", "") + "','uid':'" + uid + "','uRole':'" + uRole + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    sdm(data);
                },
                error: function (data)
                { sdm(data); }
            });
            flag = 1;
            return false;
        }

        function sdm(msg)
        {
            Mapping(msg.d["rows"]);
            //BuildTable(msg.d);
            bindGrid(msg)
         
        }
        function bindGrid(Data1) {
            var grid = $("#divGrid").data("kendoGrid");
            // detach events
            if (grid != undefined)
                grid.destroy();

            var d = Data1.d;
            var d2 = JSON.parse(d["GridData"]);
            var col = d["Columns"];

            $("#divGrid").kendoGrid({
                dataSource: {
                    dataType: "json",
                    data: d2,
                    pageSize: 20
                },

                height: 430,
                scrollable: true,
                resizable: true,
                reorderable: true,
                groupable: true,
                sortable: true,
                filterable: true,
                pageable: true,
                toolbar: ["excel"],
                excel: {
                    fileName: "VehicleMapRpt.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: col
            });
            $("#mask").hide();
        }
      
        function CreateMap(geopoints) {
            var k;

            var temp = 0;

            var marker_icon;

            marker_icon = "images/car1.png";

            document.getElementById("divmap").innerHTML = "";

            var GeoPointslength = geopoints.length;

            var x, y;

            for (var ctr = 0; ctr < GeoPointslength; ctr++) {
                var markersContainer = new nokia.maps.map.Container();

                x = parseFloat(geopoints[ctr].lattitude);

                y = parseFloat(geopoints[ctr].longitude);

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

            var mapContainer = document.getElementById("divmap");

            var infoBubbles = new nokia.maps.map.component.InfoBubbles();

            var map = new nokia.maps.map.Display(mapContainer, {

                center: [x, y],
                zoomLevel: 7,

                components: [infoBubbles, new nokia.maps.map.component.ZoomBar(), new nokia.maps.map.component.Traffic(), new nokia.maps.map.component.Behavior(), new nokia.maps.map.component.TypeSelector()]
                //components: [infoBubbles, new nokia.maps.map.component.Behavior()]
            });


            for (var i = 0; i < GeoPointslength; i++) {
                var markersContainer = new nokia.maps.map.Container();

                x = parseFloat(geopoints[i].lattitude);

                y = parseFloat(geopoints[i].longitude);

                var imageMarker = new nokia.maps.map.Marker([x, y],
                 {
                     icon: marker_icon,
                     dragable: true,
                     position: [x, y],
                     anchor: new nokia.maps.util.Point(1, 1),
                     $html: 0000
                 })

                map.objects.addAll([imageMarker]);

                map.objects.add(markersContainer);

                var Buttonid = '<input type="image" ID="btnEdit12" src="././images/edit.jpg" style="height:26px;Width:50px;" class="click" onClick="editing()"  AlternateText="Edit" onfocus=abc() />';
                var Buttonid2 = '<input type="image" ID="btnDtl12" src="././images/lock.png" style="height:26px;Width:50px;" class="click" onClick="Locking()" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';

                imageMarker.html = "<table width='230px'><tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + "Address :</td>" + '<td style="width:50%;text-align:left;color:white;font-size:15px;">' + geopoints[i].Address + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>IMEI No.:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["Device IMEI No."] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>User Name:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["User Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>Vehicle No:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["Vehicle No"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>State:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].State + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>City :</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].City + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>Last Movement Date and Time :</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["Last Movement Date & Time"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>Lattitude:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].lattitude + "</td></tr><tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + "Longitude:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].longitude + "</td></tr></table>"

                var TOUCH = nokia.maps.dom.Page.browser.touch,

                CLICK = TOUCH ? "tap" : "click";

                imageMarker.addListener(CLICK, function (evt) {

                    infoBubbles.openBubble(this.html, this.coordinate);

                    tid = (evt.target.$html);

                });
            }
        }

        var myvar = setInterval(function () { ShowMap(); }, 600000);

        function BuildTable(msg)
        {
       
            document.getElementById("tblGVReport").style.display = "block";
            document.getElementById("tblGVReport").innerHTML = "";
            if (eid == 32)
            {
                var ht = "<table cellspacing='0' cellpadding='3' rules='all' id='tblGVReport' style='border-color:Green;border-width:1px;border-style:None;font-size:Small;width:100%;border-collapse:collapse;'><tr class='gridheaderhome' align='center' style='color:Black;background-color:#D0D0D0;border-color:Green;border-width:1px;border-style:solid;font-weight:bold;height:25px;'><th scope='col' style='border-right:solid 1px Gray;width:10%;'>State</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>City</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Device IMEI No.</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Vehicle No</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>User Name</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Last Movement Date &amp; Time</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Current Location Address</th></tr>";
                $("#tblGVReport").html(ht);
                for (var ctr = 0; ctr < msg.length; ctr++) {
                    $("#tblGVReport").append("<tr class='gridrowhome' align='center' valign='middle' style='color:#333333;background-color:#F7F6F3;border-color:Green;border-width:1px;border-style:solid;height:25px;'><td style='border-right:solid 1px Gray;width:10%;'>" + msg[ctr].State + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr].City + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Device IMEI No."] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Vehicle No"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["User Name"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Last Movement Date & Time"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Address"] + "</td></tr></table>");
                }
            }
            else {
                var ht = "<table cellspacing='0' cellpadding='3' rules='all' id='tblGVReport' style='border-color:Green;border-width:1px;border-style:None;font-size:Small;width:100%;border-collapse:collapse;'><tr class='gridheaderhome' align='center' style='color:Black;background-color:#D0D0D0;border-color:Green;border-width:1px;border-style:solid;font-weight:bold;height:25px;'><th scope='col' style='border-right:solid 1px Gray;width:10%;'>Circle</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Vehicle Name</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Vehicle No</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>User Name</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Last Movement Date &amp; Time</th><th scope='col' style='border-right:solid 1px Gray;width:15%;'>Current Location Address</th></tr>";
                $("#tblGVReport").html(ht);
                for (var ctr = 0; ctr < msg.length; ctr++)
                {
                    $("#tblGVReport").append("<tr class='gridrowhome' align='center' valign='middle' style='color:#333333;background-color:#F7F6F3;border-color:Green;border-width:1px;border-style:solid;height:25px;'><td style='border-right:solid 1px Gray;width:10%;'>" + msg[ctr].Circle + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Vehicle Name"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Vehicle No"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["User Name"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Last Movement Date & Time"] + "</td><td style='border-right:solid 1px Gray;width:15%;'>" + msg[ctr]["Address"] + "</td></tr></table>");
                }
            }
            
        }

        $(document).ajaxStart(function () { $("#wait").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#wait").css("display", "none"); });

    </script>

    <style type="text/css">
        /*Modal Popup*/
        .modalBackground {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }

        .modalPopup {
            background-color: White;
            border-width: 3px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            text-align: center;
        }

        .hidden {
            display: none;
        }

    </style>

    <script type="text/javascript" language="javascript">

        function pageLoad(sender, args) {
            var sm = Sys.WebForms.PageRequestManager.getInstance();
            if (!sm.get_isInAsyncPostBack()) {
                sm.add_beginRequest(onBeginRequest);
                sm.add_endRequest(onRequestDone);
            }
        }
        function onBeginRequest(sender, args) {
            var send = args.get_postBackElement().id;
            if (displayWait(send) == "yes") {
                $find(
                'PleaseWaitPopup').show();
            }
        }
        function onRequestDone() {
            $find(
            'PleaseWaitPopup').hide();

        }
        function displayWait(send) {
            return ("yes");
        }
        
    </script>

    <script type="text/javascript">
        function Mapping(geopoints)
        {          
            document.getElementById("divmap").innerHTML = "";

            if (geopoints.length > 0)
            {
                for (i = 0; i < 1; i++)
                {
                        var mapOptions = {
                        center: new google.maps.LatLng(parseFloat(geopoints[i].lattitude),parseFloat(geopoints[i].longitude)),
                        zoom: 7,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    };
                }
            }

            else {
                var mapOptions = {
                    center: new google.maps.LatLng(28.5808792, 77.2263718),
                    zoom: 7,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
            }

            var infoWindow = new google.maps.InfoWindow();

            var map = new google.maps.Map(document.getElementById("divmap"), mapOptions);
            var Buttonid = '<input type="image" ID="btnEdit" src="././images/edit.jpg" class="click" style="height:"56px; Width:106px;" onClick="editing()" AlternateText="Edit" /></span>';
            var Buttonid2 = '<input type="image" ID="btnLockhit" src="././images/lock.png" style="height:22px; Width:75px;" onClick="Locking()"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" /></span>';

            for (i = 0; i < geopoints.length; i++)
            {
                var myLatlng = new google.maps.LatLng(geopoints[i].lattitude, geopoints[i].longitude);
                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    draggable: true,
                    // title: data.title,
                    id: i
                });
                //debugger;
                if (eid == 32) {
                    marker.html = "<table width='230px'><tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + "Address :</td>" + '<td style="width:50%;text-align:left;color:Black;font-size:15px;">' + geopoints[i].Address + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>IMEI No.:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Device IMEI No."] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>User Name:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["User Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Vehicle No:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Vehicle No"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>State:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i].State + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>City :</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i].City + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Date and Time :</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Last Movement Date & Time"] + "</td></tr></table>"
                }
                else   {
                    marker.html = "<table width='230px'><tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + "Location :</td>" + '<td style="width:50%;text-align:left;color:Black;font-size:15px;">' + geopoints[i].Address + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>IMEI No.:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Device IMEINo"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Phone Name:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Phone Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Date and Time :</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Last Movement Date & Time"] + "</td></tr></table>"
                  }      
                //else  {
                //    marker.html = "<table width='230px'><tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + "Location :</td>" + '<td style="width:50%;text-align:left;color:Black;font-size:15px;">' + geopoints[i].Address + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>IMEI No.:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Device IMEI No."] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Vehicle Name:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Vehicle Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Vehicle No:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Vehicle No"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Circle:</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i].Circle + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>CLuster :</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i].Cluster + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:Black;font-size:15px;'>Date and Time :</td><td style='width:50%;text-align:left;color:Black;font-size:15px;'>" + geopoints[i]["Last Movement Date & Time"] + "</td></tr></table>"
                //}
                google.maps.event.addListener(marker, 'click', function () {
                    infoWindow.setContent(this.html);
                    infoWindow.open(map, this);
                    x = this.id;
                });
              
            }
        }
    </script>

    <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>

    <asp:ImageButton ID="btnexportxl" ToolTip="Export EXCEL" ImageAlign="Right" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg" />
    &nbsp;
            <div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top: 10px">
                <asp:UpdatePanel ID="upnl" runat="server">
                    <ContentTemplate>
                        <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0"  class="m9">
                            <tr>
                                <td>
                                    <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
                                    <asp:HiddenField ID="hdveh" runat="server" />
                                    <asp:HiddenField ID="hdcity" runat="server" />
                                    <asp:HiddenField ID="hdcir" runat="server" />
                                    <asp:HiddenField ID="hdimieno" runat="server" />
                                    
                                </td>
                            </tr>
                            <tr>

                                <td align="Left" width="30%">
                                    <fieldset style="height: 170px">
                                        <legend style="color: Black; text-align: Left; font-weight: bold;">
                                            <asp:CheckBox ID="circlecheck" runat="server" AutoPostBack="true" OnCheckedChanged="checkuncheckcicle" />
                                            <asp:Label ID="lblcircle" runat="server" Text="Label"></asp:Label>
                                        </legend>
                                        <asp:Panel ID="Panel2" runat="server" Height="150px"
                                            Style="display: block" ScrollBars="Auto">
                                            <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="true" >
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                    </fieldset>
                                </td>
                                <td align="Left" width="30%">
                                    <fieldset style="height: 170px">
                                        <legend style="color: Black; text-align: Left; font-weight: bold;">
                                            <asp:CheckBox ID="Citycheck" runat="server" AutoPostBack="true" OnCheckedChanged="Citycheckuncheck" />
                                            <asp:Label ID="lblcity" runat="server" Text="Label"></asp:Label>
                                           </legend>
                                        <asp:Panel ID="Panel3" runat="server" Height="150px"
                                            Style="display: block" ScrollBars="Auto">

                                            <asp:CheckBoxList ID="City" runat="server" AutoPostBack="true">
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                    </fieldset>

                                </td>

                                <td align="Left" width="35%">
                                    <fieldset style="height: 170px">
                                        <legend style="color: Black; text-align: Left; font-weight: bold;">
                                            <asp:CheckBox ID="CheckBox1"  AutoPostBack="true" runat="server" /><asp:Label ID="lblveh" runat="server" Text="Label"></asp:Label></legend>
                                        <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto"
                                            Style="display: block">
                                            <asp:CheckBoxList ID="UsrVeh" runat="server">
                                            </asp:CheckBoxList>
                                            
                                        </asp:Panel>
                                    </fieldset>
                                </td>
                                <td align="center" class="m8b" width="5%">
                                    <input type="image" src="images/worldmap.jpg" style="width: 50pxd; height: 25px;" onclick=" return ShowMap();" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="width: 100%">
                                     <div id="divGrid">
                            </div>
        <%-- <table cellspacing="0" cellpadding="3" rules="all" id="tblGVReport" style="border-color: Green; border-width: 1px; border-style: None; font-size: Small; width: 100%; border-collapse: collapse; display:none;">
        <tr class="gridheaderhome" align="center" style="color: Black; background-color: #D0D0D0; border-color: Green; border-width: 1px; border-style: solid; font-weight: bold; height: 25px;">
            <th scope="col" style="border-right: solid 1px Gray; width:10%;">State</th>
            <th scope="col" style="border-right: solid 1px Gray; width:15%;">City</th>
            <th scope="col" style="border-right: solid 1px Gray; width:15%;">Device IMEI No.</th>
            <th scope="col" style="border-right: solid 1px Gray; width:15%;">Vehicle No</th>
            <th scope="col" style="border-right: solid 1px Gray; width:15%;">User Name</th>
            <th scope="col" style="border-right: solid 1px Gray;width:15%;">Last Movement Date &amp; Time</th>
            <th scope="col" style="border-right: solid 1px Gray;width:15%;">Current Location Address</th>
        </tr></table>--%>
                                    </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
    <div id="divdata"> 

    </div>
    <asp:Panel ID="PleaseWaitMessagePanel" runat="server" CssClass="modalPopup" Height="50px" Width="125px">
        Please wait &nbsp&nbsp&nbsp&nbsp&nbsp<asp:ImageButton ID="imgclose" runat="server" ImageUrl="~/images/close.png" />
        <br />
        <img src="images/uploading.gif" alt="Please wait" />
    </asp:Panel>
    <asp:Button ID="HiddenButton" runat="server" CssClass="hidden" Text="Hidden Button"
        ToolTip="Necessary for Modal Popup Extender" />
    <asp:ModalPopupExtender ID="PleaseWaitPopup" BehaviorID="PleaseWaitPopup" runat="server" TargetControlID="HiddenButton" PopupControlID="PleaseWaitMessagePanel" BackgroundCssClass="modalBackground" CancelControlID="imgclose">
    </asp:ModalPopupExtender>
    <div id="divmap" style="margin-top: 10px; width: 100%; height: 500px;"></div>
    <div id="wait" style="display:none;width:69px;height:50px;border:1px solid black;position:absolute;top:50%;left:50%;padding:2px;"><img src="images/uploading.gif" width="64" height="64" /><br/>Loading..</div>
</asp:Content>

