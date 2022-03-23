<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="NGroupMap, App_Web_gsdfcjye" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src="https://js.api.here.com/v3/3.0/mapsjs-core.js" type="text/javascript" charset="utf-8"></script>
		<script src="https://js.api.here.com/v3/3.0/mapsjs-service.js" type="text/javascript" charset="utf-8"></script>
		<script src="https://js.api.here.com/v3/3.0/mapsjs-mapevents.js" type="text/javascript" charset="utf-8"></script>
		<script src="https://js.api.here.com/v3/3.0/mapsjs-ui.js" type="text/javascript" charset="utf-8"></script>
		
		<link rel="stylesheet" type="text/css" href="https://js.api.here.com/v3/3.0/mapsjs-ui.css" />
		
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

        .hide {
            display:none;
        }
        .gm-style-iw {
           /*font-weight:bold !important;*/
            text-align:left !important;
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
        option
        {
            padding-left: 5px;
            font-size: 14px;
            height:20px;
        }
</style>
     <div id="loading-div-background">
    <div id="loading-div" class="ui-corner-all" >
      <img style="height:34px;margin:4px;" src="images/attch.gif" alt="Loading.."/>
      <h2 style="color:black;font-weight:normal;">Please wait....</h2>
     </div>
    </div>
    <table style="margin-bottom:15px; margin-top:15px;">
        <tr>
            <td><b>Select group name : </b></td>
            <td><asp:DropDownList ID="ddlGroup" CssClass="textbox" Width="200px" runat="server" onchange="GetMarkers();"></asp:DropDownList></td>
            <td>

            </td>
        </tr>
    </table>


    <div id="mapContainer" style="width: 73%; height: 75%;text-align:center;vertical-align:middle; margin:auto; background: gray; border: 2px solid gray; min-height:450px;" />

    <script type="text/javascript">
        
        var hidpi = ('devicePixelRatio' in window && devicePixelRatio > 1);

        var mapContainer = document.getElementById('mapContainer'),

            platform = new H.service.Platform({
                app_id: 'FhrHxdDSWojustuTPwwL',
                app_code: '-DMrq8Tm98ut9TA3-wSnOA',
                useCIT: true,
                useHTTPS: true,
            }),
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

        function GetMarkers() {
           
            
            for (var i = 0; i < a.length; i++) {
                a[i].setVisibility(false);
            }

            var ddltext = $("#<%=ddlGroup.ClientID%> option:selected").text()

            if (ddltext.toUpperCase() == 'SELECT')
            {
                return false;
            }

            $("#loading-div-background").show();

            $.ajax({
                type: "POST",
                url: "NGroupMap.aspx/GetMarkers",
                data: '{GroupName: "' + ddltext + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: GetMarkerSucceed,
                failure: function (response) {
                    $("#loading-div-background").hide();
                    alert(response.d);
                }
            });
        }

        var info = null;
        function GetMarkerSucceed(result) {
            $("#loading-div-background").hide();
            var arr = result.d.split("=");
            var iconStr = arr[1].split(':');

            var HmapIcon = {}
            for (var i = 0; i < iconStr.length - 1; i++) {
                HmapIcon[iconStr[i].replace('.', '_')] = new H.map.Icon("images/MapIcons/" + iconStr[i]);
            }


            var data = csv2array(arr[0], '^', '|');

            for (var i = 0; i < data.length - 1; i++) {
                var x = parseFloat(data[i][2]);
                var y = parseFloat(data[i][3]);
                if (isNaN(x) == true || isNaN(y) == true)
                { continue; }
                var marker = new H.map.Marker(new mapsjs.geo.Point(x, y),
                    {
                        icon: HmapIcon[data[i][4].replace('.', '_')]
                    });
                marker.setData({
                    doc: data[i][1],
                    tid: data[i][0],
                    fldid: data[i][5]
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


                    var tid = evt.target.getData().tid;
                    var doc = evt.target.getData().doc;
                    var fldid = evt.target.getData().fldid;
                    var ddltext = $("#<%=ddlGroup.ClientID%> option:selected").text()

                    $.ajax({
                        type: "post",
                        url: "NGroupMap.aspx/GetInfo",
                        data: "{Tid : " + tid + ", Doc:'" + doc + "', FldId:" + fldid + ", GroupName:'" + ddltext + "'}",
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
                a[i] = marker;
            }

        }

        var a = new Array();

    </script>

</asp:Content>

