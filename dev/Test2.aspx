<%@ Page Title="" Language="VB" MasterPageFile="~/PublicMaster.master" AutoEventWireup="false" CodeFile="Test2.aspx.vb" Inherits="Test2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

  <meta name="viewport" content="initial-scale=1.0, width=device-width" />
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
                                        <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="false" >
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                    <br />
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">Select Vehicle Type</legend>
                                    <asp:Panel ID="Panel1" runat="server" Height="100px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="chkvtype" runat="server" AutoPostBack="false" >
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
                                <asp:Button ID="Button1" runat="server" Visible="false" Text="Button" />
                            </div>
                        </td>
                        <%--</fieldset>--%>
                        <td style="width: 87%; height: 770px;" valign="top" >
                             <div id="map" style="width: 100%; height: 100%;text-align:center;vertical-align:middle; margin:auto; background: gray; border: 2px solid gray;" />
                            
                                
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

    <script  type="text/javascript" charset="UTF-8" >
       
        var myMarkersArray = [];
        var RemovedMarkers = myMarkersArray.slice(0);

        $(document).ready(function () { ShowMap(); });

        function ShowMap() {
            $.ajax({
                type: "post",
                url: "Test2.aspx/GetMarkerListJSON2",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) { chk(data); },
                error: function (data) {
                    chk(data);
                }
            });
        }
        function chk(abc) {
            var str = abc.d;
            if (abc.d == null) {

            }
            else {

                var obj = csv2array(str, '^', '|');

                var l = obj.length;
                ReportMaster(obj);

            }


        }

        var info;
        var mapContainer;
        var map;

        function ReportMaster(str) {

            var test = 0;
            try {
                var data = str;
                //var data = JSON.parse(str);
                function addMarkerToGroup(group, coordinate, html) {
                    var marker = new H.map.Marker(coordinate);
                    // add custom data to the marker
                    marker.setData(html);
                    group.addObject(marker);
                }

                function addInfoBubble() {

                    var Siteicon = '';
                    for (var i = 0; i < data.length - 1; i++) {
                        test += 1;

                        if (data[i][3].toUpperCase() == 'HUB') {
                            Siteicon = "http://www.myndsaas.com/images/darkyellow.png";
                        }
                        else if (data[i][3].toUpperCase() == 'BSC') {
                            Siteicon = "http://www.myndsaas.com/images/lightblue.png";
                        }
                        else if (data[i][3].toUpperCase() == 'STRATEGIC') {
                            Siteicon = "http://www.myndsaas.com/images/blue.png";
                        }
                        else if (data[i][3].toUpperCase() == 'NON STRATEGIC') {
                            Siteicon = "http://www.myndsaas.com/images/darkk.png";
                        }


                        var x = parseFloat(data[i][4]);
                        var y = parseFloat(data[i][5]);


                        if (isNaN(x) == true || isNaN(y) == true)
                        { continue; }
                        var bearsIcon = new H.map.Icon(Siteicon);

                        var bearsMarker = new H.map.Marker({ lat: x, lng: y },
                            { icon: bearsIcon, Tid: data[i][0] }, { group: data[i][6] }, { Tid: data[i][0] }, { Cluster: data[i][7] });
                        // bearsMarker.setData('hi');
                        map.addObject(bearsMarker);
                        var group = new H.map.Group();

                        map.addObject(group);

                        // add 'tap' event listener, that opens info bubble, to the group
                        group.addEventListener('tap', function (evt) {
                            // event target is the marker itself, group is a parent event target
                            // for all objects that it contains
                            var str = "This is test..";

                            $.ajax({
                                type: "post",
                                url: "Test2.aspx/GetMarkerInfo",
                                data: "{Id : " + evt.target.b + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (result) {
                                    str = result.d;
                                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                                        // read custom data
                                        content: str//evt.target.getData()
                                    });

                                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {

                                        content: str//evt.target.getData()

                                    });


                                    // show info bubble
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
                                    str = result;
                                }
                            });

                            //var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {

                            //    content: str//evt.target.getData()

                            //});


                            //// show info bubble
                            //if (info == null) {
                            //    ui.addBubble(bubble);
                            //    info = bubble;
                            //}
                            //else {
                            //    info.close();
                            //    ui.addBubble(bubble);
                            //    info = bubble;
                            //}



                        }, false);

                        bearsMarker.setData('<div style="width: 100%;background: black;font-size:small; " />' + data[i].Information + '</div>');
                        bearsMarker.b = data[i][0];
                        group.addObject(bearsMarker);
                        myMarkersArray.push(bearsMarker);
                    }



                }




                var platform = new H.service.Platform({
                    app_id: 'VG3IAYYwc1Y7XaBWEqU9',
                    app_code: 'R7W2h1KNHKBqJsJOkRbTiw',
                        //app_id: 'DemoAppId01082013GAL',
                        //app_code: 'AJKnXv84fjrb0KIHawS0Tg',
                    useCIT: true
                });
                var defaultLayers = platform.createDefaultLayers();


                var map = new H.Map(document.getElementById('map'),
                  defaultLayers.normal.map, {
                      center: { lat: 19.8761653, lng: 75.3433139 },
                      zoom: 7
                  });


                var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));


                var ui = H.ui.UI.createDefault(map, defaultLayers);



                addInfoBubble();

            }
            catch (e) {
                alert(e.message + ', ' + test);
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

  </script>


   

</asp:Content>

