<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="NHome, App_Web_2echgblw" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script src="js/jquery-1.9.1.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://js.api.here.com/v3/3.0/mapsjs-ui.css" />
    <script type="text/javascript" charset="UTF-8" src="https://js.api.here.com/v3/3.0/mapsjs-core.js"></script>
    <script type="text/javascript" charset="UTF-8" src="https://js.api.here.com/v3/3.0/mapsjs-service.js"></script>
    <script type="text/javascript" charset="UTF-8" src="https://js.api.here.com/v3/3.0/mapsjs-ui.js"></script>
    <script type="text/javascript" charset="UTF-8" src="https://js.api.here.com/v3/3.0/mapsjs-mapevents.js"></script>

    <style type="text/css">
        .hide {
            display: none;
        }

        .active {
            background: #808080;
        }

        .btn {
            border: 1px solid #9D9D9D;
            border-bottom: 0px;
            width: 100%;
            /*padding: top right bottom left;*/
            /*padding: 2px 2px 2px 0px;*/
            padding: 2px;
            box-sizing: border-box;
        }

        .img {
            cursor: pointer;
            border: 1px solid #9D9D9D;
            width: 33px;
        }

        .leftToggle {
            background: #fff none repeat scroll 0 0;
            border: 1px solid #9d9d9d;
            box-shadow: 6px 6px 4px #ccc;
            color: #000;
            min-height: 500px;
            position: absolute;
            width: 300px;
            z-index: 100;
            padding: 10px;
        }

        .H_ib {
            color: #fff !important;
            fill: #000 !important;
            font-size: 12px !important;
            width: 250px !important;
            line-height: 20px !important;
            text-align: left !important;
        }

        .H_ib_content {
            margin: 10px;
            color: #000;
        }

        .H_ib_body {
            width: 250px;
            right: 0 !important;
            background: #fff;
            border: 1px solid #808080;
            box-shadow: 5px 5px 5px #888888;
        }
        /*.H_ib_close {
            background:#000;
            height:15px;
        }*/
        .H_ib_close svg.H_icon {
            fill: #000!important;
        }

        .H_ib_tail {
            bottom: -0.6em!important;
        }

        .hide {
            display: none;
        }

        .gm-style-iw {
            /*font-weight:bold !important;*/
            text-align: left !important;
        }

        .chklist {
            max-height: 130px;
            min-height: 130px;
            overflow-y: scroll;
            /*margin-bottom:3px;*/
            border: 0.5px solid rgba(206, 203, 203, 0.96);
            max-width: 150px;
        }

            .chklist label {
                font-weight: normal;
                font-size: 10px;
            }

            .chklist tr:hover {
                background: #89C4FA;
                cursor: pointer;
            }

            .chklist option:hover {
                background: #89C4FA;
                cursor: pointer;
            }

        .chktext {
            margin-top: 2px;
        }

        .ddl {
            border: 1px solid #d2d2d2;
            padding: 3px;
            width: 100%;
        }

        .mul_text {
            border: 1px solid #d2d2d2;
            line-height: 23px;
            width: 99%;
        }

        .mul_div {
            background: #fff none repeat scroll 0 0;
            border: 1px solid #d2d2d2;
            max-width: 45.5%;
            min-height: 50px;
            position: absolute;
            width: 100%;
            box-shadow: 4px 4px 3px #ccc;
        }

        #dvCluster {
            max-height: 150px;
            overflow-y: scroll;
            padding: 3px;
            box-sizing: border-box;
        }

            #dvCluster input {
                cursor: pointer;
            }

        .txt {
            width: 100%;
            border: 1px solid #d2d2d2;
            padding: 3px;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            font-size: 10px;
        }

        .lbl {
            color: #000;
        }

        .input50 {
        }

            .input50 input {
                width: 48%;
                float: left;
            }

        .btn1 {
            background: #000;
            color: #fff;
            border: 1px solid #000;
            float: right;
            margin-bottom: 2px;
            margin-left: 2px;
        }

            .btn1:hover {
                background: #535252;
                cursor: pointer;
            }

        .hide {
            display: none;
        }

        #loading-div {
            width: 300px;
            height: 70px;
            background-color: #fff !important;
            text-align: center;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -150px;
            margin-top: -100px;
            z-index: 2000;
            opacity: 1.0;
            color: black;
        }

        #loading-div-background {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            background: rgba(0, 0, 0, 0.54);
            width: 100%;
            height: 100%;
            z-index: 1000;
        }

        .imgRefresh {
            height: 15px;
        }

            .imgRefresh:hover {
                cursor: pointer;
                background: #89C4FA;
            }
    </style>
    <input type="hidden" id="hdndata" value="" refreshtime="" />
    <asp:HiddenField ID="hdnRefTime" Value="" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnCluster" Value="" runat="server" ClientIDMode="Static" />
    <div class="btn">
        <img src="images/menu.png" class="img tmenu" title="Apply filters" onclick="ToggelLeftBar();" />
    </div>
    <div class="leftToggle">
        <table style="width: 100%">
            <col style="width: 150px;" />
            <col style="width: 150px;" />
            <tr>
                <td>
                    <div class="lbl">
                        <asp:DropDownList ID="ddlCircle" CssClass="ddl" runat="server" onchange="GetCluster(this);">
                            <asp:ListItem Selected="True" Value="0">Select Circle</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </td>
                <td>
                    <div class="lbl">
                        <asp:TextBox ID="txtCluster" ClientIDMode="Static" placeholder="Select Clusters" runat="server" CssClass="mul_text"></asp:TextBox>
                        <div class="mul_div hide">
                            <div id="dvCluster">
                            </div>
                            <div>
                                <input id="btnok" type="button" class="btn1" value="ok" style="bottom: 0; float: left;" />
                                <input id="btnCancel" type="button" class="btn1" value="Cancel" style="bottom: 0; float: left;" />
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="lbl">
                        Site Type
                    </div>
                    <div class="chklist">
                        <asp:CheckBoxList ID="ChkSiteType" runat="server">
                        </asp:CheckBoxList>
                    </div>
                </td>
                <td>
                    <div class="lbl input50">
                        <input type="text" id="txtSearchV" class="txt" placeholder="Site Name" onkeyup="return SearchSite(this,0);" />
                        <input style="float: right !important;" type="text" id="txtSearchId" class="txt" placeholder="SiteID" onkeyup="return SearchSite(this,1);" />
                    </div>
                    <div style="clear: both;"></div>
                    <div class="chklist">
                        <table>
                            <tbody id="trSites">
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="lbl">
                        <table>
                            <col style="width: 90%" />
                            <col style="width: 10%" />
                            <tr>
                                <td>Vehicle Type</td>
                                <td style="text-align: right;">
                                    <img src="images/MarkerIcons/arr.png" class="imgRefresh" onclick="RefreshVehicle();" title="Refresh Vehicles" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="chklist">
                        <asp:CheckBoxList ID="chkvtype" runat="server">
                        </asp:CheckBoxList>
                    </div>
                </td>
                <td>
                    <div class="lbl">
                        <input type="text" id="VehSearch" placeholder="Type Vechicle name" class="txt" onkeyup="return SearchList(this);" />
                    </div>
                    <div class="">
                        <asp:ListBox ID="LstVehicle" onchange="focusMarker(this);" runat="server" class="chklist" DataTextField="Vehicle" Style="width: 100%; border: 1px solid #D0CDCD;"></asp:ListBox>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="lbl">
                        <table>
                            <col style="width: 90%" />
                            <col style="width: 10%" />
                            <tr>
                                <td>Manpower Type</td>
                                <td style="text-align: right;">
                                    <img src="images/MarkerIcons/arr.png" class="imgRefresh" title="Refresh Manpowers" onclick="RefreshManpower();" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="chklist">
                        <asp:CheckBoxList ID="chkManPower" runat="server">
                        </asp:CheckBoxList>
                    </div>
                </td>
                <td>
                    <div class="lbl">
                        <input type="text" id="txtManpower" placeholder="Type Man power name" class="txt" onkeyup="return SearchManPower(this,'#idMan');" />
                    </div>
                    <div class="chklist">
                        <table>
                            <tbody id="tdManpower">
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>

        </table>
    </div>
    <div id="map" class="mapdiv" style="width: 99.8%; height: 530px; border: 1px solid #9D9D9D;">
    </div>

    <div id="loading-div-background">
        <div id="loading-div" class="ui-corner-all">
            <img style="height: 34px; margin: 4px;" src="images/attch.gif" alt="Loading.." />
            <h2 style="color: black; font-weight: normal;">Please wait....</h2>
        </div>
    </div>

    <script type="text/javascript">


        var HmapIcon = {
            HUB: new H.map.Icon("http://www.myndsaas.com/images/darkyellow.png"),
            BSC: new H.map.Icon("http://www.myndsaas.com/images/lightblue.png"),
            STRATEGIC: new H.map.Icon("http://www.myndsaas.com/images/blue.png"),
            NON_STRATEGIC: new H.map.Icon("http://www.myndsaas.com/images/darkk.png"),
            dfgray: new H.map.Icon("http://www.myndsaas.com/images/dfgray.png"),
            truck_gray: new H.map.Icon("http://www.myndsaas.com/images/truck_gray.png"),
            mob_gray: new H.map.Icon("http://www.myndsaas.com/images/mob_gray.png"),
            sc_gray: new H.map.Icon("http://www.myndsaas.com/images/sc_gray.png"),
            car4: new H.map.Icon("http://www.myndsaas.com/images/car4.png"),
            dfgreen: new H.map.Icon("http://www.myndsaas.com/images/dfgreen.png"),
            truck_green: new H.map.Icon("http://www.myndsaas.com/images/truck_green.png"),
            mob_green: new H.map.Icon("http://www.myndsaas.com/images/mob_green.png"),
            sc_green: new H.map.Icon("http://www.myndsaas.com/images/sc_green.png"),
            car2: new H.map.Icon("http://www.myndsaas.com/images/car2.png"),
            dfpink: new H.map.Icon("http://www.myndsaas.com/images/dfpink.png"),
            truck_pink: new H.map.Icon("http://www.myndsaas.com/images/truck_pink.png"),
            mob_pink: new H.map.Icon("http://www.myndsaas.com/images/mob_pink.png"),
            sc_pink: new H.map.Icon("http://www.myndsaas.com/images/sc_pink.png"),
            dfred: new H.map.Icon("http://www.myndsaas.com/images/dfred.png"),
            truck_red: new H.map.Icon("http://www.myndsaas.com/images/truck_red.png"),
            mob_red: new H.map.Icon("http://www.myndsaas.com/images/mob_red.png"),
            sc_red: new H.map.Icon("http://www.myndsaas.com/images/sc_red.png"),
            car1: new H.map.Icon("http://www.myndsaas.com/images/car1.png"),
            walking: new H.map.Icon("http://www.myndsaas.com/images/walking.png"),
            man1: new H.map.Icon("http://www.myndsaas.com/images/man1.png"),
            man2: new H.map.Icon("http://www.myndsaas.com/images/man2.png"),
            man3: new H.map.Icon("http://www.myndsaas.com/images/man3.png"),
            man4: new H.map.Icon("http://www.myndsaas.com/images/man4.png"),
            man5: new H.map.Icon("http://www.myndsaas.com/images/man5.png")
        };

        var arrCircles = [];
        var info = null;
        var platform = new H.service.Platform({
            app_id: 'DemoAppId01082013GAL',
            app_code: 'AJKnXv84fjrb0KIHawS0Tg',
            useCIT: true,
            useHTTPS: true
        });
        var defaultLayers = platform.createDefaultLayers();

        var map = new H.Map(document.getElementById('map'),
          defaultLayers.normal.map, {
              //center: { lat: 28.502269, lng: 77.085958 },
              center: { lat: 21.1254976, lng: 72.1142578 },
              zoom: 4
          });

        var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
        var ui = H.ui.UI.createDefault(map, defaultLayers);

        function ToggelLeftBar() {
            $(".leftToggle").toggle(500);
            $(".tmenu").toggleClass('active');
        }

        $(document).ready(function () { });

        $('#txtCluster').focus(
            function () {
                $('.mul_div').removeClass('hide');
            });
        $('#btnok').click(
            function () {
                $('.mul_div').addClass('hide');
                GetMarkers();
            });
        $('#btnCancel').click(
            function () {
                $('.mul_div').addClass('hide');
            });

        function GetCluster(ele) {
            $("#loading-div-background").show();
            $('#dvCluster').html('');
            var obj = {};
            obj.CircleID = $(ele).val();
            ToggleCircles();
            $.ajax({
                type: "post",
                url: "NHome.aspx/GetCluster",
                data: JSON.stringify(obj),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#loading-div-background").hide();
                    BindCluster(data);
                },
                error: function (data) {
                    $("#loading-div-background").hide();
                }
            });
        }

        function BindCluster(result) {
            var ar = JSON.parse(result.d);
            var html = '<input id="chk_0" value="0" type="checkbox" container="dvCluster" onclick="CheckAll(this);" /> <strong>Select All</strong><br/>';;
            for (var i = 0; i < ar.length; i++) {
                html += '<input id="chk_"' + ar[i].Value + ' value="' + ar[i].Value + '" type="checkbox" />  ' + ar[i].Text + '<br/>';
            }
            $('#dvCluster').html(html);
        }

        function CheckAll(e) {
            var container = $(e).attr('container');
            if ($(e).is(':checked')) {
                $("#" + container).find(":checkbox").each(function (i, ob) {
                    $(ob).prop('checked', true)
                });
            }
            else {
                $("#" + container).find(":checkbox").each(function (i, ob) {
                    $(ob).prop('checked', false)
                });
            }
        }

        $(document).ready(function () {
            CircleCounts();
        });

        function CircleCounts() {
            var obj = {};
            obj.CircleId = 0;
            $.ajax({
                type: "post",
                url: "NHome.aspx/CircleCounts",
                data: JSON.stringify(obj),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#loading-div-background").hide();
                    DrawCircleCounts(data);
                },
                error: function (data) {
                    $("#loading-div-background").hide();
                    chk(data);
                }
            });
        }

        function DrawCircleCounts(result) {
            if (result.d == '') { return false; }
            var arr = JSON.parse(result.d);
            for (var i = 0; i < arr.length; i++) {
                var x = parseFloat(arr[i].Lat);
                var y = parseFloat(arr[i].Long);

                var bearsIcon = HmapIcon['circles'];

                //var strInfo = '<b>Clusters: </b>' + arr[i].Clusters +'<br/>';
                var strInfo = 'Manpowers: ' + arr[i].Manpowers + '<br/>';
                strInfo += 'Sites:     ' + arr[i].Sites + '<br/>';
                strInfo += 'Vehicles:  ' + arr[i].Vehicles + '<br/>';

                var objData = {};
                objData.ID = arr[i].Tid;
                objData.Type = 0;


                var obInfo = {};
                obInfo.Tid = arr[i].Tid;
                obInfo.Marker = addDomMarker(map, x, y, strInfo, objData);
                arrCircles.push(obInfo);
            }
        }

        function addDomMarker(map, Latitude, Longitude, text, data) {
            var outerElement = document.createElement('div'),
                innerElement = document.createElement('div');
            outerElement.style.userSelect = 'none';
            outerElement.style.webkitUserSelect = 'none';
            outerElement.style.msUserSelect = 'none';
            outerElement.style.mozUserSelect = 'none';
            outerElement.style.cursor = 'default';
            innerElement.style.color = '#fff';
            innerElement.style.backgroundColor = '#FF6600';
            innerElement.style.border = '2px solid #333333';
            innerElement.style.font = 'normal 12px arial';
            innerElement.style.lineHeight = '12px'
            innerElement.style.padding = "5px";
            // add negative margin to inner element
            // to move the anchor to center of the div
            innerElement.style.marginTop = '-10px';
            innerElement.style.marginLeft = '-10px';
            outerElement.appendChild(innerElement);
            // Add text to the DOM element
            //innerElement.innerHTML = 'Sites: 10887<br/>Manpowers: 10887<br/>Vehicles: 10887';
            innerElement.innerHTML = text;
            function changeOpacity(evt) {
                evt.target.style.cursor = "pointer";
                evt.target.style.backgroundColor = '#99CC00';
            };

            function changeOpacityToOne(evt) {
                evt.target.style.opacity = 1;
                evt.target.style.backgroundColor = '#FF6600';
            };

            //create dom icon and add/remove opacity listeners
            var domIcon = new H.map.DomIcon(outerElement, {
                // the function is called every time marker enters the viewport
                onAttach: function (clonedElement, domIcon, domMarker) {
                    clonedElement.addEventListener('mouseover', changeOpacity);
                    clonedElement.addEventListener('mouseout', changeOpacityToOne);
                },
                // the function is called every time marker leaves the viewport
                onDetach: function (clonedElement, domIcon, domMarker) {
                    clonedElement.removeEventListener('mouseover', changeOpacity);
                    clonedElement.removeEventListener('mouseout', changeOpacityToOne);
                }
            });

            var bearsMarker = new H.map.DomMarker({ lat: Latitude, lng: Longitude }, {
                icon: domIcon
            });
            bearsMarker.setData(data);
            map.addObject(bearsMarker);
            return bearsMarker;
        }

        function GetMarkers() {
            try {
                $("#loading-div-background").show();
                ClearMap();
                var Clusters = [];
                $("#dvCluster").find("input:checked").each(function (i, ob) {
                    Clusters.push($(ob).val());
                });
                var obj = {};
                obj.CircleId = $('#<%=ddlCircle.ClientID%>').val();
                obj.Clusters = Clusters;
                $.ajax({
                    type: "post",
                    url: "NHome.aspx/GetMarkers",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#loading-div-background").hide();
                        GetMarkerSucceed(data);
                    },
                    error: function (data) {
                        $("#loading-div-background").hide();
                    }
                });
            }
            catch (err) {
                $("#loading-div-background").hide();
            }
            var refTime = $('#hdnRefTime').attr('value');
           // setInterval(RefreshVehicle(), refTime);
        }
        function GetMarkerSucceed(result) {
            var obj = result.d;
            var Sites = csv2array(obj.Sites, '^', '|');
            var ManPowers = csv2array(obj.ManPowers, '^', '|');
            var Vehicles = csv2array(obj.Vehicles, '^', '|');
            var Siteicon = '';
            //Sites
            for (var i = 0; i < Sites.length; i++) {
                if (Sites[i][0] == "")
                { continue; }
                if (Sites[i] == "")
                { continue; }
                if (Sites[i][1].toUpperCase() == '516531') {
                    Siteicon = 'HUB';
                }
                else if (Sites[i][1].toUpperCase() == '516534') {
                    Siteicon = 'BSC';
                }
                else if (Sites[i][1].toUpperCase() == '516532') {
                    Siteicon = 'STRATEGIC';
                }
                else //if (Sites[i][1].toUpperCase() == '516533')
                {
                    Siteicon = 'NON_STRATEGIC';
                }
                var objdata = {};
                objdata.ID = Sites[i][0];
                objdata.Type = Sites[i][1];
                objdata.Flag = 0;
                var x = parseFloat(Sites[i][2]);
                var y = parseFloat(Sites[i][3]);
                AddMarker(Siteicon, x, y, objdata);
            }//Sites

            //Manpowers
            for (var i = 0; i < ManPowers.length; i++) {
                
                if (ManPowers[i][0] == "")
                { continue; }
                if (ManPowers[i][1].toUpperCase() == 'CLUSTER MANAGER') {
                    Siteicon = 'man2';
                }
                else if (ManPowers[i][1].toUpperCase() == 'FSE') {
                    Siteicon = 'man2';
                }
                else if (ManPowers[i][1].toUpperCase() == 'TECHNICAL MANAGER') {
                    Siteicon = 'man2';
                }
                else if (ManPowers[i][1].toUpperCase() == 'TECHNICIAN') {
                    Siteicon = 'man4';
                }
                else if (ManPowers[i][1].toUpperCase() == 'ZONAL HEAD') {
                    Siteicon = 'man5';
                }
                else {
                    Siteicon = 'man5';
                }

                var objdata = {};
                objdata.ID = ManPowers[i][0];
                objdata.Type = ManPowers[i][1];
                objdata.Flag = 1;
                var x = parseFloat(ManPowers[i][2]);
                var y = parseFloat(ManPowers[i][3]);
                AddMarker(Siteicon, x, y, objdata);
            }//Manpowers

            for (var i = 0; i < Vehicles.length; i++) {
                if (Vehicles[i][0] == "")
                { continue; }
                if (Vehicles[i] == "" )
                { continue; }
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1
                var yyyy = today.getFullYear();
                mm = mm < 10 ? '0' + mm : mm;
                dd = dd < 10 ? '0' + dd : dd;
                var dateVal = mm + '/' + dd + '/' + yyyy;

                if (Vehicles[i][3] > 10 && Vehicles[i][3] <= 600 && Vehicles[i][6] == dateVal) {

                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfgray';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_gray';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_gray';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_gray';
                    }
                    else {
                        Siteicon = 'car4';
                    }
                }
                else if (Vehicles[i][3] >= 0 && Vehicles[i][3] <= 10 && Vehicles[i][6] == dateVal) {
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfgreen';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_green';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_green';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_green';
                    }
                    else {
                        Siteicon = 'car2';
                    }
                }
                else if ((Vehicles[i][3] > 600 && Vehicles[i][3] <= 1440) || (Vehicles[i][6] == dateVal && Vehicles[i][3] > 1440)) {
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfpink';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_pink';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_pink';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_pink';
                    }
                    else {
                        Siteicon = 'car2';
                    }
                }
                else {
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfred';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_red';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_red';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_red';
                    }
                    else {
                        Siteicon = 'car1';
                    }


                }
                var objdata = {};
                objdata.ID = Vehicles[i][1];
                objdata.Type = Vehicles[i][7];
                objdata.Flag = 2;
                var x = parseFloat(Vehicles[i][4]);
                var y = parseFloat(Vehicles[i][5]);
                AddMarker(Siteicon, x, y, objdata);
            }//Vehicle

        }

        function AddMarker(icon, Latitude, Longitude, MarkerData) {
            var hicon = HmapIcon[icon];
            var Marker = new H.map.Marker({ lat: Latitude, lng: Longitude }, { icon: hicon });
            Marker.setData(MarkerData);

            Marker.addEventListener('tap', function (evt) {
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
                var objinfo = {};
                var hdn = document.getElementById("hdndata");
                objinfo['ID'] = evt.target.getData().ID;
                //objinfo['Type'] = evt.target.getData().Type;
                objinfo['Flag'] = evt.target.getData().Flag;
                objinfo['Ids'] = GetIDS();

                var str = JSON.stringify(objinfo)
                $.ajax({
                    type: "post",
                    url: "NHome.aspx/GetInfo",
                    data: str,
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

            map.addObject(Marker);
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

        function GetIDS() {
            var str = '';
            var CHK = document.getElementById('<%= ChkSiteType.ClientID%>');
            if (CHK != null) {
                var checkbox = CHK.getElementsByTagName("input");
                for (var i = 0; i < checkbox.length; i++) {
                    if (checkbox[i].checked) {
                        str += checkbox[i].value + ',';
                    }
                }
            }
            var CHK1 = document.getElementById('<%= chkvtype.ClientID%>');
            if (CHK1 != null) {
                var checkbox1 = CHK1.getElementsByTagName("input");
                for (var i = 0; i < checkbox1.length; i++) {
                    if (checkbox1[i].checked) {
                        str += checkbox1[i].value + ',';
                    }
                }
            }
            var hdn = document.getElementById("hdndata");
            hdn.value = str;
            return str;
        }


        function ShowHideMap(sender) {
            var group = $(sender).attr("value");
            var objects = map.getObjects(),
              len = map.getObjects().length,
              i;
            if ($(sender).is(':checked')) {
                for (i = 0; i < len; i += 1) {
                    var groupID = objects[i].getData().Type;
                    if (groupID == group) {
                        objects[i].setVisibility(true);
                    }
                }
            }
            else {
                for (i = 0; i < len; i += 1) {
                    var groupID = objects[i].getData().Type;
                    if (groupID == group) {
                        objects[i].setVisibility(false);
                    }
                }
            }
        }

        function ToggleCircles() {
            if ($('#<%=ddlCircle.ClientID%>').val() == 0) {
            for (var i = 0; i < arrCircles.length; i++) {
                arrCircles[i].Marker.setVisibility(true);
            }
        }
        else {
            for (var i = 0; i < arrCircles.length; i++) {
                arrCircles[i].Marker.setVisibility(false);
            }
        }
    }

    function ClearMap() {
        var objects = map.getObjects(),
          len = map.getObjects().length,
          i;
        for (i = 0; i < len; i += 1) {
            var type = objects[i].getData().Type;
            if (type != '0') {
                objects[i].setVisibility(false);
            }
        }
    }

    function SearchList1() {
        var l = document.getElementById('<%= LstVehicle.ClientID%>');
            var tb = document.getElementById('VehSearch');
            var strlb = String();
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

        function SearchList(ele) {
            $.each($('#<%= LstVehicle.ClientID%>').find('option'), function (index, element) {
                if ($(element).text().toLowerCase().match($(ele).val().toLowerCase())) {
                    $(element).removeClass('hide');
                }
                else {
                    $(element).addClass('hide');
                }
            });
        }

        function SearchSite(Sender, Flag) {
            try {
                $('#trSites').html('')
                var Clusters = [];
                $("#dvCluster").find("input:checked").each(function (i, ob) {
                    Clusters.push($(ob).val());
                });
                var obj = {};
                obj.CircleId = $('#<%=ddlCircle.ClientID%>').val();
                obj.Clusters = Clusters;
                obj.SearchText = $(Sender).val();
                obj.Flag = Flag;
                $.ajax({
                    type: "post",
                    url: "NHome.aspx/SearchSites",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        SearchSiteSucceed(data);
                    },
                    error: function (data) {
                    }
                });
            }
            catch (err) {

            }
        }
        function SearchSiteSucceed(result) {
            if (result.d == "") {
                return false;
            }
            var obj = JSON.parse(result.d);
            var row = '';
            for (var i = 0; i < obj.length; i++) {
                row += '<tr id="' + obj[i].Value + '" value="' + obj[i].Value + '" onclick="FocusMarker(this);"><td>' + (i + 1) + ':' + obj[i].Text + '<td/><tr/>';
            }
            $('#trSites').html(row);
        }

        function SearchManPower(Sender) {
            try {
                $('#tdManpower').html('')
                var Clusters = [];
                $("#dvCluster").find("input:checked").each(function (i, ob) {
                    Clusters.push($(ob).val());
                });
                var obj = {};
                obj.CircleId = $('#<%=ddlCircle.ClientID%>').val();
                obj.Clusters = Clusters;
                obj.SearchText = $(Sender).val();
                $.ajax({
                    type: "post",
                    url: "NHome.aspx/SearchManPower",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        SearchManPowerSucceed(data);
                    },
                    error: function (data) {
                    }
                });
            }
            catch (err) {

            }
        }
        function SearchManPowerSucceed(result) {
            if (result.d == "") {
                return false;
            }
            //var obj = JSON.parse(result.d);
            var arr = csv2array(result.d, '^', '|');
            var row = '';
            for (var i = 0; i < arr.length - 1; i++) {

                row += '<tr id="' + arr[i][0] + '" value="' + arr[i][0] + '" onclick="FocusMarker(this);"><td>' + (i + 1) + ':' + arr[i][1] + '<td/><tr/>';
            }
            $('#tdManpower').html(row);
        }

        function FocusMarker(Sender) {
            var ID = $(Sender).attr('value');
            var objects = map.getObjects(),
             len = map.getObjects().length,
             i;
            for (i = 0; i < len; i += 1) {
                var MarkerID = objects[i].getData().ID;

                if (MarkerID == ID) {

                    objects[i].setVisibility(true);
                    var lat = objects[i].getPosition().lat;
                    var lng = objects[i].getPosition().lng;

                    // map.setCenter({lat: (lat-0.003), lng:(lng+0.0001)});
                    var strInfo = "Loading...";
                    var bubble = new H.ui.InfoBubble(objects[i].getPosition(), {
                        content: strInfo//evt.target.getData()
                    });
                    var bubble = new H.ui.InfoBubble(objects[i].getPosition(), {
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
                    var objinfo = {};
                    var hdn = document.getElementById("hdndata");
                    objinfo['ID'] = objects[i].getData().ID;
                    //objinfo['Type'] = evt.target.getData().Type;
                    objinfo['Flag'] = objects[i].getData().Flag;
                    objinfo['Ids'] = GetIDS();
                    var str = JSON.stringify(objinfo);
                    var marker = objects[i];
                    $.ajax({
                        type: "post",
                        url: "NHome.aspx/GetInfo",
                        data: str,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            strInfo = result.d;
                            var bubble = new H.ui.InfoBubble(marker.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });
                            var bubble = new H.ui.InfoBubble(marker.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });
                            info.setContent(strInfo);
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
                }
            }
        }

        function RefreshVehicle() {
            $("#loading-div-background").show();
            try {

                var obj = {};
                var str = '';
                $("#ContentPlaceHolder1_chkvtype").find("input:checked").each(function (i, ob) {
                    str += $(ob).val() + ',';
                });

                var Clusters = [];
                $("#dvCluster").find("input:checked").each(function (i, ob) {
                    Clusters.push($(ob).val());
                });
                obj.Ids = str;
                obj.CircleId = $('#<%=ddlCircle.ClientID%>').val();
                obj.Clusters = Clusters;

                $.ajax({
                    type: "post",
                    url: "NHome.aspx/RefreshVehicle",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#loading-div-background").hide();
                        RefreshVehicleSucceed(data);
                    },
                    error: function (data) {
                        $("#loading-div-background").hide();
                    }
                });

            }
            catch (err)
            { $("#loading-div-background").hide(); }
        }

        function RefreshVehicleSucceed(result) {
            var Vehicles = csv2array(result.d, "^", "|");

            for (var i = 0; i < Vehicles.length; i++) {
                if (Vehicles[i] == "")
                { continue; }
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1
                var yyyy = today.getFullYear();
                mm = mm < 10 ? '0' + mm : mm;
                dd = dd < 10 ? '0' + dd : dd;
                var dateVal = mm + '/' + dd + '/' + yyyy;
                var Siteicon = '';
                var color = '';
                if (Vehicles[i][3] > 10 && Vehicles[i][3] <= 600 && Vehicles[i][6] == dateVal) {
                    color = "#000";
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfgray';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_gray';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_gray';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_gray';
                    }
                    else {
                        Siteicon = 'car4';
                    }
                }
                else if (Vehicles[i][3] >= 0 && Vehicles[i][3] <= 10 && Vehicles[i][6] == dateVal) {
                    color = "#279E27";
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfgreen';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_green';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_green';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_green';
                    }
                    else {
                        Siteicon = 'car2';
                    }
                }
                else if ((Vehicles[i][3] > 600 && Vehicles[i][3] <= 1440) || (Vehicles[i][6] == dateVal && Vehicles[i][3] > 1440)) {
                    color = "#B86A84";
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfpink';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_pink';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_pink';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_pink';
                    }
                    else {
                        Siteicon = 'car2';
                    }
                }
                else {
                    color = "#FF0000";
                    if (Vehicles[i][7] == 554655) {
                        Siteicon = 'dfred';
                    }
                    else if (Vehicles[i][7] == 583948) {
                        Siteicon = 'truck_red';
                    }
                    else if (Vehicles[i][7] == 554654) {
                        Siteicon = 'mob_red';
                    }
                    else if (Vehicles[i][7] == 554657) {
                        Siteicon = 'sc_red';
                    }
                    else {
                        Siteicon = 'car1';
                    }
                }

                var VID = Vehicles[i][1];

                var x = parseFloat(Vehicles[i][4]);
                var y = parseFloat(Vehicles[i][5]);

                var objects = map.getObjects(),
                len = map.getObjects().length,
                i;
                for (i = 0; i < len; i += 1) {
                    var ID = objects[i].getData().ID;
                    if (VID == ID) {
                        objects[i].setIcon(HmapIcon[Siteicon]);
                        objects[i].setPosition({ lat: x, lng: y });
                        $('[value="' + VID + '"]').css('color', color);
                    }
                }

                AddMarker(Siteicon, x, y, objdata);
            }//Vehicle
        }

        function RefreshManpower() {
            $("#loading-div-background").show();
            try {
                var obj = {};
                var Clusters = [];
                $("#dvCluster").find("input:checked").each(function (i, ob) {
                    Clusters.push($(ob).val());
                });
                obj.CircleID = $('#<%=ddlCircle.ClientID%>').val();
                obj.Clusters = Clusters;

                $.ajax({
                    type: "post",
                    url: "NHome.aspx/RefreshManpower",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#loading-div-background").hide();
                        RefreshManpowerSucceed(data);
                    },
                    error: function (data) {
                        $("#loading-div-background").hide();
                    }
                });

            }
            catch (e) {
                $("#loading-div-background").hide();
            }
        }

        function RefreshManpowerSucceed(result) {
            var ManPowers = csv2array(result.d, "^", "|");
            for (var i = 0; i < ManPowers.length - 1; i++) {
                if (ManPowers[i][0] == "")
                { continue; }
               
               var MID = ManPowers[i][0];
                
                var x = parseFloat(ManPowers[i][2]);
                var y = parseFloat(ManPowers[i][3]);

                var objects = map.getObjects(),
                len = map.getObjects().length,
                i;
                for (i = 0; i < len; i += 1) {
                    var ID = objects[i].getData().ID;
                    if (MID == ID) {
                        objects[i].setPosition({ lat: x, lng: y });
                    }
                }

            }//Manpowers
        }

    </script>
</asp:Content>

