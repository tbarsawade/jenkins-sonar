 <%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="NMapReportTest, App_Web_sifhu5tb" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
        .maindiv {
            padding:15px;
             color:#000;
             background:rgba(226, 226, 226, 0.40);
             font-family:Calibri;
             width:1000px;
             min-height:740px;
        }
        .leftbar {
            width:220px;
            border:1px solid rgba(206, 203, 203, 0.96);
            margin-right:2px;
            min-height:740px;
            float:left;
        }
        .right {
            width:774px;
            border:1px solid rgba(206, 203, 203, 0.96);
            float:left;
             height:740px;
        }
         .right1 {
            width:774px;
            float:left;
           min-height:250px;
        }
        .chklist {
        max-height:150px;
        overflow-y:scroll;
        /*margin-bottom:3px;*/
        border:0.5px solid rgba(206, 203, 203, 0.96);
        }
        .chktext {
            margin-top:2px;
        }
        .ddl {
            width:219px;
            height:24px;
        }
        .txt {
            width:218px;
            height:24px;
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
            width:260px;
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

        #divsearch label {
            cursor:pointer;
        }
            #divsearch label:hover {
                background:#3399FF;
            }

        .ref {
            float:right;
            margin-right:3px;
            cursor:pointer;
        }

          .idSite {
              max-height:110px;
              overflow-y:scroll;
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
               
.lstContent option:hover
{
    background-color:#DEEFFF;
    cursor:pointer;
    color:blue;
}

        #idSite tr:hover {
             background-color:#DEEFFF;
    cursor:pointer;
    color:blue;
    
        }

         #idMan tr:hover {
             background-color:#DEEFFF;
    cursor:pointer;
    color:blue;
    
        }

        .hide {
            display:none;
        }
        .show {
            display:block;
        }
        .show1 {
            display:block;
            width:40px;
            height:25px;
        }

    </style>

    <input type="hidden" id="hdndata" value="" RefreshTime="" />
        <asp:HiddenField ID="hdnRefTime" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdnCluster" Value="" runat="server" ClientIDMode="Static" />

    <div class="maindiv">
        <div class="leftbar">
            <asp:Panel ID="pnlChkList" runat="server">
                 <div>
                     Select Site Type
                </div>
                <div class="chklist">
                    <asp:CheckBoxList ID="ChkSiteType" runat="server">
                    </asp:CheckBoxList>
                </div>
                <div>
                     Select Vehicle Type
                </div>
                <div class="chklist">
                     
                    <asp:CheckBoxList ID="chkvtype" runat="server">
                    </asp:CheckBoxList>
                     
                </div>
                <div>
                     Select Manpower Type
                </div>
                <div class="chklist">
                     
                    <asp:CheckBoxList ID="chkManPower" runat="server">
                    </asp:CheckBoxList>
                     
                </div>

                 <div>
                      <input type="text" id="VehSearch" placeholder="Type Vehicle name" class="txt" onkeyup="return SearchList();" />
                 </div>
                      <div class="">
                          <asp:ListBox ID="LstVehicle" onchange="focusMarker(this);" runat="server" class="lstContent" DataTextField="Vehicle" style="width:100%;min-height: 100px; border:1px solid #D0CDCD;">
                           </asp:ListBox>
                      </div>
                <div>
                      <input type="text" id="txtManpower" placeholder="Type Man power name" class="txt" onkeyup="return SearchManPower(this,'#idMan');" />
                 </div>
                      <div class="idSite">
                          <table style="width:100%;">
                                            <tbody id="idMan">

                                            </tbody>
                         </table>
                      </div>

                <div>
                      <input type="text" id="txtSearchV" class="txt" placeholder="Type Site Name" onkeyup="return SearchSite(this,'#idSite',false);"  />
                      <input type="text" id="txtSearchId" class="txt" placeholder="Type SiteID" onkeyup="return SearchSite(this,'#idSite',true);"  />
                 </div>
                
                      <div class="idSite">
                         <table style="width:100%;">
                                            <tbody id="idSite">

                                            </tbody>
                         </table>
                      </div>

            </asp:Panel>
            <asp:Panel ID="pnlSearch" runat="server">

            </asp:Panel>
        </div>
        
        <div id="map" class="right" >

        </div>
           
    </div>
    
    <table cellspacing="0" celpadding="0" width="100%" border="1" style=" vertical-align:middle;">
                             <colgroup style="width:10%;"></colgroup>
                             <colgroup style="width:40%; "></colgroup>
                             <colgroup style="width:10%;  "></colgroup>
                             <colgroup style="width:40%; "></colgroup>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image5" runat="server" ImageUrl="~/images/blue.png" />
                                    </td>
                                    <td>
                                        &nbsp; Strategic</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="width:30px; height:15px; background:red; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                       
                                       &nbsp;Vehicle data not received for more than 24 Hrs.
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image6" runat="server" ImageUrl="~/images/darkyellow.png" />
                                    </td>
                                    <td>
                                        &nbsp; Normal Hub</td>
                                     <td style="text-align:center; vertical-align:middle;">
                                        <span style="width:30px; height:15px; background:green; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp;Vehicle is Moving</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image7" runat="server" ImageUrl="~/images/darkk.png" />
                                    </td>
                                    <td>
                                        &nbsp; Non Strategic</td>
                                    <td style="text-align:center; vertical-align:middle;">

                                        <span style="width:30px; height:15px; background:gray; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp;Vehicle is Halted from 10 minutes to 4 Hours</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image8" runat="server" ImageUrl="~/images/lightblue.png" />
                                    </td>
                                    <td>
                                        &nbsp; BSC</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="width:30px; height:15px; background:#DB94FF; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp;Vehicle is Halted more than 4 Hours</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image11" runat="server" ImageUrl="~/images/car4.png" />
                                    </td>
                                    <td >
                                        &nbsp; CM, ZH and Admin Vehicle </td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="padding:5px;">
                                            <asp:Image ID="Image9" runat="server"  ImageUrl="~/images/man2.png" />
                                        </span>
                                    </td>
                                    <td >
                                        &nbsp; FSE, Technical Manager, Cluster Manager</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="padding:5px;">
                                            <asp:Image ID="Image2" runat="server"  ImageUrl="~/images/truck_gray.png" />
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp; Warehouse</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="padding:5px;">
                                            <asp:Image ID="Image12" runat="server"  ImageUrl="~/images/man4.png" />
                                        </span></td>
                                    <td>
                                        Technician</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image15" runat="server" ImageUrl="~/images/dfgray.png" />
                                    </td>
                                    <td>
                                        &nbsp; Diesel Filing </td>
                                   <td style="text-align:center; vertical-align:middle;">
                                        <span style="padding:5px;">
                                            <asp:Image ID="Image13" runat="server"  ImageUrl="~/images/man5.png" />
                                        </span></td>
                                    <td>
                                        Zonal Head</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/mob_gray.png" />
                                    </td>
                                    <td>
                                        &nbsp; Mobile DG</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                             <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image4" runat="server" ImageUrl="~/images/sc_gray.png" />
                                    </td>
                                    <td>
                                        &nbsp; SMS</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                            </table>

    <div id="loading-div-background">
    <div id="loading-div" class="ui-corner-all" >
      <img style="height:34px;margin:4px;" src="images/attch.gif" alt="Loading.."/>
      <h2 style="color:black;font-weight:normal;">Please wait....</h2>
     </div>
    </div>

    <script  type="text/javascript" charset="UTF-8" >

        var myMarkersArray = [];
        var RemovedMarkers = myMarkersArray.slice(0);
        var defaultLayers;
        var ui;

         $(document).ready(function () { ShowMap('<%= ChkSiteType.ClientID%>', '<%= chkvtype.ClientID%>'); CacheValues(); });

         function ShowMap(ddlsite, ddlVechical) {
             $("#loading-div-background").show();
             var str = '';
             var CHK = document.getElementById(ddlsite);

             if (CHK != null) {
                 var checkbox = CHK.getElementsByTagName("input");
                 for (var i = 0; i < checkbox.length; i++) {
                     if (checkbox[i].checked) {
                         str += checkbox[i].value + ',';
                     }
                 }
             }


             var CHK1 = document.getElementById(ddlVechical);

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

             $.ajax({
                 type: "post",
                 url: "NMapReportTest.aspx/GetMarkerListJSON2",
                 data: '{IDs: "' + str + '" }',
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (data) {
                     $("#loading-div-background").hide();
                     chk(data);
                 },
                 error: function (data) {
                     $("#loading-div-background").hide();
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
                 // ReportMaster(obj);
                 DrawMapMarkers(obj);

             }
         }

         var info;
         var mapContainer;
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
             man5: new H.map.Icon("http://www.myndsaas.com/images/man5.png"),
             circles: new H.map.Icon("http://www.myndsaas.com/images/circles.png"),
         };
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
                         if (data[i] == "")
                         { continue; }
                         if (data[i][3].toUpperCase() == 'HUB') {
                             //Siteicon = "http://www.myndsaas.com/images/darkyellow.png";
                             Siteicon = 'HUB';
                         }
                         else if (data[i][3].toUpperCase() == 'BSC') {
                             //Siteicon = "http://www.myndsaas.com/images/lightblue.png";
                             Siteicon = 'BSC';
                         }
                         else if (data[i][3].toUpperCase() == 'STRATEGIC') {
                             //Siteicon = "http://www.myndsaas.com/images/blue.png";
                             Siteicon = 'STRATEGIC';
                         }
                         else if (data[i][3].toUpperCase() == 'NON STRATEGIC') {
                             //Siteicon = "http://www.myndsaas.com/images/darkk.png";
                             Siteicon = 'NON_STRATEGIC';
                         }
                         else {
                             var today = new Date();
                             var dd = today.getDate();
                             var mm = today.getMonth() + 1
                             var yyyy = today.getFullYear();
                             mm = mm < 10 ? '0' + mm : mm;
                             dd = dd < 10 ? '0' + dd : dd;
                             var dateVal = mm + '/' + dd + '/' + yyyy;

                             if (data[i][3] > 10 && data[i][3] <= 600 && data[i][6] == dateVal) {

                                 if (data[i][7] == 554655) {
                                     //Siteicon = 'images/dfgray.png';
                                     Siteicon = 'dfgray';
                                 }
                                 else if (data[i][7] == 583948) {
                                     //Siteicon = 'images/truck_gray.png';
                                     Siteicon = 'truck_gray';
                                 }
                                 else if (data[i][7] == 554654) {
                                     //Siteicon = 'images/mob_gray.png';
                                     Siteicon = 'mob_gray';
                                 }
                                 else if (data[i][7] == 554657) {
                                     //Siteicon = 'images/sc_gray.png';
                                     Siteicon = 'sc_gray';
                                 }
                                 else {
                                     //Siteicon = "http://www.myndsaas.com/images/car4.png";
                                     Siteicon = 'car4';
                                 }


                             }
                             else if (data[i][3] >= 0 && data[i][3] <= 10 && data[i][6] == dateVal) {

                                 if (data[i][7] == 554655) {
                                     //Siteicon = 'images/dfgreen.png';
                                     Siteicon = 'dfgreen';
                                 }
                                 else if (data[i][7] == 583948) {
                                     //Siteicon = 'images/truck_green.png';
                                     Siteicon = 'truck_green';
                                 }
                                 else if (data[i][7] == 554654) {
                                     //Siteicon = 'images/mob_green.png';
                                     Siteicon = 'mob_green';
                                 }
                                 else if (data[i][7] == 554657) {
                                     //Siteicon = 'images/sc_green.png';
                                     Siteicon = 'sc_green';
                                 }
                                 else {
                                     //Siteicon = "http://www.myndsaas.com/images/car2.png";
                                     Siteicon = 'car2';
                                 }


                             }
                             else if ((data[i][3] > 600 && data[i][3] <= 1440) || (data[i][6] == dateVal && data[i][3] > 1440)) {
                                 if (data[i][7] == 554655) {
                                     //Siteicon = 'images/dfpink.png';
                                     Siteicon = 'dfpink';
                                 }
                                 else if (data[i][7] == 583948) {
                                     //Siteicon = 'images/truck_pink.png';
                                     Siteicon = 'truck_pink';
                                 }
                                 else if (data[i][7] == 554654) {
                                     //Siteicon = 'images/mob_pink.png';
                                     Siteicon = 'mob_pink';
                                 }
                                 else if (data[i][7] == 554657) {
                                     //Siteicon = 'images/sc_pink.png';
                                     Siteicon = 'sc_pink';
                                 }
                                 else {
                                     //Siteicon = "http://www.myndsaas.com/images/car2.png";
                                     Siteicon = 'car2';
                                 }
                             }
                             else {
                                 if (data[i][7] == 554655) {
                                     //Siteicon = 'images/dfred.png';
                                     Siteicon = 'dfred';
                                 }
                                 else if (data[i][7] == 583948) {
                                     //Siteicon = 'images/truck_red.png';
                                     Siteicon = 'truck_red';
                                 }
                                 else if (data[i][7] == 554654) {
                                     //Siteicon = 'images/mob_red.png';
                                     Siteicon = 'mob_red';
                                 }
                                 else if (data[i][7] == 554657) {
                                     //Siteicon = 'images/sc_red.png';
                                     Siteicon = 'sc_red';
                                 }
                                 else {
                                     //Siteicon = "http://www.myndsaas.com/images/car1.png";
                                     Siteicon = 'car1';
                                 }
                             }

                         }
                         var grp;
                         var tid;
                         if (data[i][8] == '0') {
                             grp = data[i][7];
                             tid = data[i][1];
                         }
                         else {
                             grp = data[i][6];
                             tid = data[i][0];
                         }


                         var x = parseFloat(data[i][4]);
                         var y = parseFloat(data[i][5]);


                         if (isNaN(x) == true || isNaN(y) == true)
                         { continue; }
                         //var bearsIcon = new H.map.Icon(Siteicon);

                         var bearsIcon = HmapIcon[Siteicon];

                         var bearsMarker = new H.map.Marker({ lat: x, lng: y },
                             { icon: bearsIcon, Tid: data[i][0] }, { group: data[i][6] }, { Tid: data[i][0] }, { Cluster: data[i][7] });

                         map.addObject(bearsMarker);
                         var group = new H.map.Group();

                         map.addObject(group);


                         group.addEventListener('tap', function (evt) {

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

                             //var ar = evt.target.cg.split('^');

                             //var str = ar[2] != 0 ? '1' : '0';

                             var str = evt.target.getData().IsVeh != 0 ? '1' : '0';

                             var hdn = document.getElementById("hdndata");

                             //var tid = ar[2] != 0 ? evt.target.b : ar[1];

                             var tid = evt.target.getData().IsVeh != 0 ? evt.target.b : evt.target.getData().tid;

                             $.ajax({
                                 type: "post",
                                 url: "NMapReportTest.aspx/GetMarkerInfo",
                                 data: "{Id : " + tid + ", IsVehical : " + str + ", Ids:'" + hdn.value + "'}",
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

                         // bearsMarker.setData('<div style="width: 100%;background: black;font-size:small; " />' + data[i].Information + '</div>');
                         bearsMarker.b = data[i][0];
                         bearsMarker.cg = grp + '^' + tid + '^' + data[i][8] + '^' + data[i][2] + '^' + data[i][1];

                         bearsMarker.setData({
                             info: '<div style="width: 100%;background: black;font-size:small; " />' + data[i].Information + '</div>',
                             group: grp,
                             tid: tid,
                             IsVeh: data[i][8],
                             SiteName: data[i][2],
                             SiteID: data[i][1]
                         });

                         group.addObject(bearsMarker);
                         myMarkersArray.push(bearsMarker);
                     }

                 }


                 //var platform = new H.service.Platform({
                 //    //app_id: 'VG3IAYYwc1Y7XaBWEqU9',
                 //    //app_code: 'R7W2h1KNHKBqJsJOkRbTiw',
                 //    app_id: 'FhrHxdDSWojustuTPwwL',
                 //    app_code: '-DMrq8Tm98ut9TA3-wSnOA',
                 //    useCIT: true
                 //});
                 //defaultLayers = platform.createDefaultLayers();


                 //map = new H.Map(document.getElementById('map'),
                 // defaultLayers.normal.map, {
                 //     center: { lat: 19.8761653, lng: 75.3433139 },
                 //     zoom: 7
                 // });

                 //var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
                 //ui = H.ui.UI.createDefault(map, defaultLayers);

                 addInfoBubble();

             }
             catch (e) {
                 alert(e.message + ', ' + test);
             }

             var refTime = $('#hdnRefTime').attr('value');

             setInterval(GetNewMarker, refTime);

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

         var myVals = new Array();

         function CacheValues() {
             var l = document.getElementById('<%= LstVehicle.ClientID%>');

            for (var i = 0; i < l.options.length; i++) {

                myVals[i] = l.options[i];
            }
            return;
         }

        function SearchList() {
            // alert('hello');
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

        function focusMarker(sender) {
            var v = $(sender).val();
            for (i = 0; i < myMarkersArray.length; i++) {

                var HmapMarker = myMarkersArray[i];

                // var ar = HmapMarker.cg.split('^');

                if ((HmapMarker.getData().tid == v)) {

                    var strInfo = "Loading...";
                    var loc = HmapMarker.getPosition();
                    map.setCenter(loc);

                    var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
                        content: strInfo//evt.target.getData()
                    });

                    var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
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

                    var str = HmapMarker.getData().IsVeh != 0 ? '1' : '0';
                    var hdn = document.getElementById("hdndata");

                    var tid = HmapMarker.getData().IsVeh != 0 ? HmapMarker.b : HmapMarker.getData().tid;

                    $.ajax({
                        type: "post",
                        url: "NMapReportTest.aspx/GetMarkerInfo",
                        data: "{Id : " + tid + ", IsVehical : " + str + ", Ids:'" + hdn.value + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            strInfo = result.d;
                            var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });

                            var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
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

                    return false;
                }
            }
        }

        function SearchManPower(Sender, body) {
            $(body).html('');
            var txt = $(Sender).val();
            if (txt == '') {
                $(body).html('');
                return false;
            }
            for (i = 0; i < myMarkersArray.length - 1; i++) {
                var HmapMarker = myMarkersArray[i];
                if (HmapMarker.getData().IsVeh != 2) {
                    continue;
                }
                if (HmapMarker.getData().SiteID.toUpperCase().match(txt.toUpperCase())) {
                    var row = '<tr id="' + HmapMarker.getData().tid + '" onclick="FocusManPower(this);"><td>' + HmapMarker.getData().SiteID + '<td/><tr/>';
                    $(body).append(row);
                }
            }

        }

        function FocusManPower(Sender)
        {
            var key = $(Sender).attr('id');

            for (i = 0; i < myMarkersArray.length; i++) {

                var HmapMarker = myMarkersArray[i];
                // var ar = HmapMarker.cg.split('^');


                if (HmapMarker.getData().IsVeh != 2) {
                    continue;
                }

                if ((HmapMarker.getData().tid == key)) {

                    var strInfo = "Loading...";

                    var loc = HmapMarker.getPosition();
                    map.setCenter(loc);
                    var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
                        content: strInfo//evt.target.getData()
                    });
                    var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
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

                    var str = '1';
                    var hdn = document.getElementById("hdndata");

                    var tid = HmapMarker.b;

                    $.ajax({
                        type: "post",
                        url: "NMapReportTest.aspx/GetMarkerInfo",
                        data: "{Id : " + tid + ", IsVehical : 2, Ids:'" + hdn.value + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            strInfo = result.d;
                            var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });

                            var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
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
                            info.close();
                        }
                    });


                    return false;
                }
            }
        }

        function FocusSite(Sender) {


            var key = $(Sender).attr('id');

            for (i = 0; i < myMarkersArray.length; i++) {

                var HmapMarker = myMarkersArray[i];
               // var ar = HmapMarker.cg.split('^');


                if (HmapMarker.getData().IsVeh == 0) {
                    continue;
                }

                if ((HmapMarker.getData().SiteID == key)) {

                    var strInfo = "Loading...";

                    var loc = HmapMarker.getPosition();
                    map.setCenter(loc);
                    var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
                        content: strInfo//evt.target.getData()
                    });
                    var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
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

                    var str = '1';
                    var hdn = document.getElementById("hdndata");

                    var tid = HmapMarker.b;

                    $.ajax({
                        type: "post",
                        url: "NMapReportTest.aspx/GetMarkerInfo",
                        data: "{Id : " + tid + ", IsVehical : " + str + ", Ids:'" + hdn.value + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            strInfo = result.d;
                            var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
                                content: strInfo//evt.target.getData()
                            });

                            var bubble = new H.ui.InfoBubble(HmapMarker.getPosition(), {
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
                            info.close();
                        }
                    });


                    return false;
                }
            }
        }



        function SearchSite(Sender, body, IsSiteId) {
            var txt = $(Sender).val();

            $(body).html('');

            if (txt == '') {
                $(body).html('');
            }
            if (IsSiteId && txt.length < 5) {
                return false;
            }
            if (txt.length < 3) {
                return false;
            }
            else {
                for (i = 0; i < myMarkersArray.length - 1; i++) {


                    var HmapMarker = myMarkersArray[i];
                    // var ar = HmapMarker.cg.split('^');


                    if (HmapMarker.getData().IsVeh == 0) {
                        continue;
                    }
                    if (IsSiteId) {

                        if (HmapMarker.getData().SiteID.toUpperCase().match(txt.toUpperCase())) {
                            var row = '<tr id="' + HmapMarker.getData().SiteID + '" onclick="FocusSite(this);"><td>' + HmapMarker.getData().SiteID + '<td/><tr/>';
                            $(body).append(row);
                        }
                    }
                    else {
                        if (HmapMarker.getData().SiteName.toUpperCase().match(txt.toUpperCase())) {
                            var row = '<tr id="' + HmapMarker.getData().SiteID + '" onclick="FocusSite(this);"><td>' + HmapMarker.getData().SiteName + '<td/><tr/>';
                            $(body).append(row);
                        }
                    }
                }

            }
            return false;
        }

        function ShowHideMap(sender) {
            var group = $(sender).attr("value");

            if ($(sender).is(':checked')) {

                for (i = 0; i < RemovedMarkers.length; i++) {

                    var HmapMarker = RemovedMarkers[i];
                    // var ar = HmapMarker.cg.split('^');

                    //if (ar[0] == group) {
                    //    HmapMarker.setVisibility(true);
                    //    myMarkersArray.push(RemovedMarkers[i])
                    //    RemovedMarkers.splice(i, 1);
                    //    i = -1;
                    //}
                    if (HmapMarker.getData().group == group) {
                        HmapMarker.setVisibility(true);
                        myMarkersArray.push(RemovedMarkers[i])
                        RemovedMarkers.splice(i, 1);
                        i = -1;
                    }
                }

            } else {
                for (i = 0; i < myMarkersArray.length; i++) {

                    var HmapMarker = myMarkersArray[i];
                    //var ar = HmapMarker.cg.split('^');

                    //if ((ar[0] == group)) {
                    //    RemovedMarkers.push(myMarkersArray[i])
                    //    HmapMarker.setVisibility(false);
                    //    myMarkersArray.splice(i, 1);
                    //    i = -1;
                    //}

                    if ((HmapMarker.getData().group == group)) {
                        RemovedMarkers.push(myMarkersArray[i])
                        HmapMarker.setVisibility(false);
                        myMarkersArray.splice(i, 1);
                        i = -1;
                    }
                }

            }


        }

        function GetNewMarker() {
            var str = document.getElementById("hdndata").value;
            $.ajax({
                type: "POST",
                url: "NMapReportTest.aspx/RefreshVechicals",
                data: '{IDs: "' + str + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: GetNewMarkerSucceed,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }

        function GetNewMarkerSucceed(result) {
            var str = result.d;
            var obj = csv2array(str, '^', '|');

            RefreshVechical(obj);

            $('#img1').removeClass('show1');
            $('#img2').removeClass('hide');
            $('#img1').addClass('hide');
            $('#img2').addClass('show1');
        }

        function RefreshVechical(str) {
            var locations = str;
            var Siteicon;

            for (i = 0; i < myMarkersArray.length - 1; i++) {

                var HmapMarker = myMarkersArray[i];
                //var ar = HmapMarker.cg.split('^');

                //if (ar[2] != 0) {
                //    continue;
                //}
                if (HmapMarker.getData().IsVeh != 0) {
                    continue;
                }

                for (j = 0; j < locations.length - 1; j++) {

                    if (HmapMarker.getData().tid == locations[j][1]) {

                        var today = new Date();
                        var dd = today.getDate();
                        var mm = today.getMonth() + 1
                        var yyyy = today.getFullYear();
                        mm = mm < 10 ? '0' + mm : mm;
                        dd = dd < 10 ? '0' + dd : dd;
                        var dateVal = mm + '/' + dd + '/' + yyyy;

                        if (locations[j][3] > 10 && locations[j][3] <= 600 && locations[j][6] == dateVal) {

                            if (locations[j][7] == 554655) {

                                Siteicon = 'dfgray';
                            }
                            else if (locations[j][7] == 583948) {

                                Siteicon = 'truck_gray';
                            }
                            else if (locations[j][7] == 554654) {

                                Siteicon = 'mob_gray';
                            }
                            else if (locations[j][7] == 554657) {

                                Siteicon = 'sc_gray';
                            }
                            else {

                                Siteicon = 'car4';
                            }

                        }
                        else if (locations[j][3] >= 0 && locations[j][3] <= 10 && locations[j][6] == dateVal) {

                            if (locations[j][7] == 554655) {

                                Siteicon = 'dfgreen';
                            }
                            else if (locations[j][7] == 583948) {

                                Siteicon = 'truck_green';
                            }
                            else if (locations[j][7] == 554654) {

                                Siteicon = 'mob_green';
                            }
                            else if (locations[j][7] == 554657) {

                                Siteicon = 'sc_green';
                            }
                            else {

                                Siteicon = 'car2';
                            }
                        }
                        else if ((locations[j][3] > 600 && locations[j][3] <= 1440) || (locations[j][6] == dateVal && locations[j][3] > 1440)) {

                            if (locations[j][7] == 554655) {

                                Siteicon = 'dfpink';
                            }
                            else if (locations[j][7] == 583948) {

                                Siteicon = 'truck_pink';
                            }
                            else if (locations[j][7] == 554654) {

                                Siteicon = 'mob_pink';
                            }
                            else if (locations[j][7] == 554657) {

                                Siteicon = 'sc_pink';
                            }
                            else {

                                Siteicon = 'car5';
                            }
                        }
                        else {
                            if (locations[j][7] == 554655) {

                                Siteicon = 'dfred';
                            }
                            else if (locations[j][7] == 583948) {

                                Siteicon = 'truck_red';
                            }
                            else if (locations[j][7] == 554654) {

                                Siteicon = 'mob_red';
                            }
                            else if (locations[j][7] == 554657) {

                                Siteicon = 'sc_red';
                            }
                            else {

                                Siteicon = 'car1';
                            }

                        }

                        var x = parseFloat(locations[j][4]);
                        var y = parseFloat(locations[j][5]);

                        HmapMarker.setPosition({ lat: x, lng: y })

                        HmapMarker.setIcon(HmapIcon[Siteicon]);

                        $('#img2').removeClass('hide');
                        $('#img2').addClass('show1');
                        $('#img1').removeClass('show1');
                        $('#img1').addClass('hide');
                    }

                }
            }

            $('#img2').removeClass('hide');
            $('#img2').addClass('show1');
            $('#img1').removeClass('show1');
            $('#img1').addClass('hide');

        }

        var sendr;
        var sendr1;
        function RefreshVechical1(sender, sender1) {
            sendr = sender;
            sender1 = sender1;
            $('#' + sender1).removeClass('hide');
            $(sender).removeClass('show1');

            $('#' + sender1).addClass('show1');
            $(sender).addClass('hide');

            GetNewMarker();
        }


        function DrawMapMarkers(data) {

            // Check whether the environment should use hi-res maps
            var hidpi = ('devicePixelRatio' in window && devicePixelRatio > 1);

            var mapContainer = document.getElementById('map'),

                platform = new H.service.Platform({
                    app_id: 'DemoAppId01082013GAL',
                    app_code: 'AJKnXv84fjrb0KIHawS0Tg',
                    useCIT: true,
                    useHTTPS: true,
                }),
                maptileService = platform.getMapTileService({ 'type': 'base' });
            maptypes = platform.createDefaultLayers(hidpi ? 512 : 256, hidpi ? 320 : null);
            map = new H.Map(mapContainer, maptypes.normal.map,
				{
				    center: new H.geo.Point(21.2597, 77.5114),
				    zoom: 6
				}
			);

            // add behavior control
            new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

            // Enable the default UI
            ui = H.ui.UI.createDefault(map, maptypes);

            // setup the Streetlevel imagery
            //platform.configure(H.map.render.panorama.RenderEngine);


            window.addEventListener('resize', function () { map.getViewPort().resize(); });


            var svg = '<svg xmlns="http://www.w3.org/2000/svg" width="28px" height="36px">' +
                      '<path d="M 19 31 C 19 32.7 16.3 34 13 34 C 9.7 34 7 32.7 7 31 C 7 29.3 9.7 28 13 28 C 16.3 28 19' +
                      ' 29.3 19 31 Z" fill="#000" fill-opacity=".2"/>' +
                      '<path d="M 13 0 C 9.5 0 6.3 1.3 3.8 3.8 C 1.4 7.8 0 9.4 0 12.8 C 0 16.3 1.4 19.5 3.8 21.9 L 13 31 L 22.2' +
                      ' 21.9 C 24.6 19.5 25.9 16.3 25.9 12.8 C 25.9 9.4 24.6 6.1 22.1 3.8 C 19.7 1.3 16.5 0 13 0 Z" fill="#fff"/>' +
                      '<path d="M 13 2.2 C 6 2.2 2.3 7.2 2.1 12.8 C 2.1 16.1 3.1 18.4 5.2 20.5 L 13 28.2 L 20.8 20.5 C' +
                      ' 22.9 18.4 23.8 16.2 23.8 12.8 C 23.6 7.07 20 2.2 13 2.2 Z" fill="__FILLCOLOR__"/>' +
                      '<text font-size="12" font-weight="bold" fill="#fff" font-family="Nimbus Sans L,sans-serif" x="10" y="19">__NO__</text>' +
                      '</svg>';

            var colors = [
						new H.map.Icon(
						            svg.replace(/__NO__/g, "")
									.replace(/__FILLCOLOR__/g, "#85FFFF")),
						new H.map.Icon(svg.replace(/__NO__/g, "")
									.replace(/__FILLCOLOR__/g, "#FFBF5F")),
						new H.map.Icon(svg.replace(/__NO__/g, "")
									.replace(/__FILLCOLOR__/g, "#CC0099")),
						new H.map.Icon(svg.replace(/__NO__/g, "")
									.replace(/__FILLCOLOR__/g, "#0000FF")),
						new H.map.Icon(svg.replace(/__NO__/g, "")
									.replace(/__FILLCOLOR__/g, "#1A3380"))
            ];

            var Siteicon = '';
            var svgIndex = 0;
            var a = new Array();
            var b = new Array();

            for (var i = 0; i < data.length; i++) {

                if (data[i][0] == "")
                { continue; }

                if (data[i][3].toUpperCase() == 'HUB') {

                    Siteicon = 'HUB';
                    svgIndex = 1;
                }
                else if (data[i][3].toUpperCase() == 'BSC') {

                    Siteicon = 'BSC';
                    svgIndex = 0;
                }
                else if (data[i][3].toUpperCase() == 'STRATEGIC') {

                    Siteicon = 'STRATEGIC';
                    svgIndex = 3;
                }
                else if (data[i][3].toUpperCase() == 'NON STRATEGIC') {

                    Siteicon = 'NON_STRATEGIC';
                    svgIndex = 2;
                }
                else {

                    if (data[i][3] == 2) {
                        if (data[i][2] == 'Cluster Manager') {
                            Siteicon = 'man2';
                        }
                        else if (data[i][2] == 'FSE') {
                            Siteicon = 'man2';
                        }
                        else if (data[i][2] == 'Technical Manager') {
                            Siteicon = 'man2';
                        }
                        else if (data[i][2] == 'Technician') {
                            Siteicon = 'man4';
                        }
                        else if (data[i][2] == 'Zonal Head') {
                            Siteicon = 'man5';
                        }
                        else {
                            Siteicon = 'man5';
                        }
                    }
                    else {


                        svgIndex = -1;
                        var today = new Date();
                        var dd = today.getDate();
                        var mm = today.getMonth() + 1
                        var yyyy = today.getFullYear();
                        mm = mm < 10 ? '0' + mm : mm;
                        dd = dd < 10 ? '0' + dd : dd;
                        var dateVal = mm + '/' + dd + '/' + yyyy;

                        if (data[i][3] > 10 && data[i][3] <= 600 && data[i][6] == dateVal) {

                            if (data[i][7] == 554655) {
                                //Siteicon = 'images/dfgray.png';
                                Siteicon = 'dfgray';
                            }
                            else if (data[i][7] == 583948) {
                                //Siteicon = 'images/truck_gray.png';
                                Siteicon = 'truck_gray';
                            }
                            else if (data[i][7] == 554654) {
                                //Siteicon = 'images/mob_gray.png';
                                Siteicon = 'mob_gray';
                            }
                            else if (data[i][7] == 554657) {
                                //Siteicon = 'images/sc_gray.png';
                                Siteicon = 'sc_gray';
                            }
                            else {
                                //Siteicon = "http://www.myndsaas.com/images/car4.png";
                                Siteicon = 'car4';
                            }


                        }
                        else if (data[i][3] >= 0 && data[i][3] <= 10 && data[i][6] == dateVal) {

                            if (data[i][7] == 554655) {
                                //Siteicon = 'images/dfgreen.png';
                                Siteicon = 'dfgreen';
                            }
                            else if (data[i][7] == 583948) {
                                //Siteicon = 'images/truck_green.png';
                                Siteicon = 'truck_green';
                            }
                            else if (data[i][7] == 554654) {
                                //Siteicon = 'images/mob_green.png';
                                Siteicon = 'mob_green';
                            }
                            else if (data[i][7] == 554657) {
                                //Siteicon = 'images/sc_green.png';
                                Siteicon = 'sc_green';
                            }
                            else {
                                //Siteicon = "http://www.myndsaas.com/images/car2.png";
                                Siteicon = 'car2';
                            }


                        }
                        else if ((data[i][3] > 600 && data[i][3] <= 1440) || (data[i][6] == dateVal && data[i][3] > 1440)) {
                            if (data[i][7] == 554655) {
                                //Siteicon = 'images/dfpink.png';
                                Siteicon = 'dfpink';
                            }
                            else if (data[i][7] == 583948) {
                                //Siteicon = 'images/truck_pink.png';
                                Siteicon = 'truck_pink';
                            }
                            else if (data[i][7] == 554654) {
                                //Siteicon = 'images/mob_pink.png';
                                Siteicon = 'mob_pink';
                            }
                            else if (data[i][7] == 554657) {
                                //Siteicon = 'images/sc_pink.png';
                                Siteicon = 'sc_pink';
                            }
                            else {
                                //Siteicon = "http://www.myndsaas.com/images/car2.png";
                                Siteicon = 'car2';
                            }
                        }
                        else {
                            if (data[i][7] == 554655) {
                                //Siteicon = 'images/dfred.png';
                                Siteicon = 'dfred';
                            }
                            else if (data[i][7] == 583948) {
                                //Siteicon = 'images/truck_red.png';
                                Siteicon = 'truck_red';
                            }
                            else if (data[i][7] == 554654) {
                                //Siteicon = 'images/mob_red.png';
                                Siteicon = 'mob_red';
                            }
                            else if (data[i][7] == 554657) {
                                //Siteicon = 'images/sc_red.png';
                                Siteicon = 'sc_red';
                            }
                            else {
                                //Siteicon = "http://www.myndsaas.com/images/car1.png";
                                Siteicon = 'car1';
                            }
                        }

                    }
                }
                var grp;
                var tid;
                if (data[i][8] == '0') {
                    grp = data[i][7];
                    tid = data[i][1];
                }
                else if (data[i][3] == 2)
                {
                    grp = data[i][2];
                    tid = data[i][0];
                }
                else {
                    grp = data[i][6];
                    tid = data[i][0];
                }

                var x = parseFloat(data[i][4]);
                var y = parseFloat(data[i][5]);


                if (isNaN(x) == true || isNaN(y) == true)
                { continue; }


                var marker;

                //if (svgIndex == -1) {
                //    marker = new H.map.Marker(new mapsjs.geo.Point(x, y),
                //     {
                //         icon: HmapIcon[Siteicon]
                //     });
                //}
                //else {
                //    marker=new H.map.Marker(new mapsjs.geo.Point(x, y),
                //         {
                //             icon:colors[svgIndex]
                //             //icon: HmapIcon[Siteicon]
                //         });
                //}

                marker = new H.map.Marker(new mapsjs.geo.Point(x, y),
                 {
                     icon: HmapIcon[Siteicon]
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


                    var str = evt.target.getData().IsVeh != 0 ? '1' : '0';
                    if (evt.target.getData().IsVeh != 0 && evt.target.getData().IsVeh != 2) {
                        str = "1";
                    }
                    else {
                        str = evt.target.getData().IsVeh;
                    }

                    var hdn = document.getElementById("hdndata");

                    var tid = evt.target.getData().IsVeh != 0 ? evt.target.b : evt.target.getData().tid;

                    $.ajax({
                        type: "post",
                        url: "NMapReportTest.aspx/GetMarkerInfo",
                        data: "{Id : " + tid + ", IsVehical : " + str + ", Ids:'" + hdn.value + "'}",
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

                marker.b = data[i][0];
                //marker.cg = grp + '^' + tid + '^' + data[i][8] + '^' + data[i][2] + '^' + data[i][1];
                var isVeh;
                if (data[i][3] == 2) {
                    isVeh = 2;
                }
                else {
                    isVeh = data[i][8];
                }
               
                marker.setData({
                    info: '<div style="width: 100%;background: black;font-size:small; " />' + data[i].Information + '</div>',
                    group: grp,
                    tid: tid,
                    IsVeh: isVeh,
                    SiteName: data[i][2],
                    SiteID: data[i][1]
                });
                //  a[i] = marker;
                myMarkersArray.push(marker);
                map.addObject(marker);
            }
            //map.addObjects(a);
        }

      </script>

</asp:Content>

