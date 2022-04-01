<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="LastLocation, App_Web_2echgblw" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link rel="stylesheet" type="text/css" href="/Src%20New/Src%20New/resources/demos/style.css" />
        
    <link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css" />    
    <script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>
    <script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
   
     <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js"></script>
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

        function OpenWindow(url)
        {
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
      
        var flag = 0;       
        function ShowMap()
        {
                
                document.getElementById("divdata").style.display = "none";
                $.ajax({
                    type: "post",
                    url: "GPSLastLocation.aspx/DrawMap",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) { sdm(data); },
                    error: function (data) { sdm(data); }
                });
                flag = 1;
        
            return false;
            
        }

        function sdm(msg)
        {
          
            CreateMap(msg.d);
            BuildTable(msg.d);
        }

        function CreateMap(geopoints) 
        {
            var k;

            var temp = 0;

            var marker_icon;

            marker_icon = "images/human.png";

            document.getElementById("divmap").innerHTML = "";

            var GeoPointslength = geopoints.length;

            var x, y;

            for (var ctr = 0; ctr < GeoPointslength; ctr++)
            {
                var markersContainer = new nokia.maps.map.Container();

                x = parseFloat(geopoints[ctr].lattitude);

                y = parseFloat(geopoints[ctr].longitude);

                if (isNaN(x) || isNaN(y))
                {
                    continue;
                }
                else
                {
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
                zoomLevel: 6,

                components: [infoBubbles, new nokia.maps.map.component.ZoomBar(), new nokia.maps.map.component.Traffic(), new nokia.maps.map.component.Behavior(), new nokia.maps.map.component.TypeSelector()]
                //components: [infoBubbles, new nokia.maps.map.component.Behavior()]
            });
            

            for (var i = 0; i < GeoPointslength; i++)
            {
                var markersContainer = new nokia.maps.map.Container();

                x = parseFloat(geopoints[i].lattitude);

                y = parseFloat(geopoints[i].longitude);
                
               var imageMarker = new nokia.maps.map.Marker([x,y],
                {
                  icon: marker_icon,
                  dragable: true,
                  position: [x,y],
                  anchor: new nokia.maps.util.Point(1, 1),
                  $html: 0000
               })

                        map.objects.addAll([imageMarker]);

                        map.objects.add(markersContainer);

                        var Buttonid = '<input type="image" ID="btnEdit12" src="././images/edit.jpg" style="height:26px;Width:50px;" class="click" onClick="editing()"  AlternateText="Edit" onfocus=abc() />';
                        var Buttonid2 = '<input type="image" ID="btnDtl12" src="././images/lock.png" style="height:26px;Width:50px;" class="click" onClick="Locking()" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';

                        imageMarker.html = "<table width='100%'><tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + "Address :</td>" + '<td style="width:50%;text-align:left;color:white;font-size:15px;">' + geopoints[i].Address + '</td></tr>' + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>IMEI No.:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["Device IMEI No."] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>User Name:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["User Name"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>Vehicle No:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["Vehicle No"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>State:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].State + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>City :</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].City + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>Last Movement Date and Time :</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i]["Last Movement Date & Time"] + "</td></tr>" + "<tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>Lattitude:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].lattitude + "</td></tr><tr><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + "Longitude:</td><td style='width:50%;text-align:left;color:white;font-size:15px;'>" + geopoints[i].longitude + "</td></tr></table>"

                        var TOUCH = nokia.maps.dom.Page.browser.touch,

                        CLICK = TOUCH ? "tap" : "click";

                        imageMarker.addListener(CLICK, function (evt) {

                         infoBubbles.openBubble(this.html, this.coordinate);

                         tid = (evt.target.$html);

                        });
                    }          
        }

               var myvar = setInterval(function () {if (flag == 1) { ShowMap();  } }, 60000);
          
              function BuildTable(msg)
               {
                   document.getElementById("tblGVReport").style.display = "block";
                   document.getElementById("tblGVReport").innerHTML = "";
                   var ht = "<table cellspacing='0' cellpadding='3' rules='all' id='tblGVReport' style='border-color:Green;border-width:1px;border-style:None;font-size:Small;width:100%;border-collapse:collapse;'><tr class='gridheaderhome' align='center' style='color:Black;background-color:#D0D0D0;border-color:Green;border-width:1px;border-style:solid;font-weight:bold;height:25px;'><th scope='col' style='border-right:solid 1px Gray;'>State</th><th scope='col' style='border-right:solid 1px Gray;'>City</th><th scope='col' style='border-right:solid 1px Gray;'>Device IMEI No.</th><th scope='col' style='border-right:solid 1px Gray;'>Vehicle No</th><th scope='col' style='border-right:solid 1px Gray;'>User Name</th><th scope='col' style='border-right:solid 1px Gray;'>Last Movement Date &amp; Time</th><th scope='col'>Current Location Address</th></tr>";
                   $("#tblGVReport").html(ht);
                    for (var ctr = 0; ctr < msg.length; ctr++)
                   {
                        $("#tblGVReport").append("<tr class='gridrowhome' align='center' valign='middle' style='color:#333333;background-color:#F7F6F3;border-color:Green;border-width:1px;border-style:solid;height:25px;'><td style='border-right:solid 1px Gray;'>" + msg[ctr].State + "</td><td style='border-right:solid 1px Gray;'>" + msg[ctr].City + "</td><td style='border-right:solid 1px Gray;'>" + msg[ctr]["Device IMEI No."] + "</td><td style='border-right:solid 1px Gray;'>" + msg[ctr]["Vehicle No"] + "</td><td style='border-right:solid 1px Gray;'>" + msg[ctr]["User Name"] + "</td><td style='border-right:solid 1px Gray;'>" + msg[ctr]["Last Movement Date & Time"] + "</td><td style='border-right:solid 1px Gray;'>" + msg[ctr]["Address"] + "</td></tr></table>");
                   }
              }

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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnSearch"  />--%>
            <%-- <asp:PostBackTrigger ControlID="btnExportPDF" />--%>
            <asp:PostBackTrigger ControlID="btnexportxl" />
            <asp:PostBackTrigger ControlID="btnshow" />
        </Triggers>
        <ContentTemplate>
            <br />
            <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>
            <%--<asp:ImageButton ID="btnExportPDF" ToolTip="Export PDF"  runat="server" Width="18px" ImageAlign="Right"  Height="18px" ImageUrl="~/images/export.png"/>&nbsp;&nbsp;--%>
            <asp:ImageButton ID="btnexportxl" ToolTip="Export EXCEL" ImageAlign="Right" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg" />
            &nbsp;
            <div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top: 10px">
                <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">

                    <tr>
                        <td>
                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>

                        <td align="Left" width="30%">
                            <fieldset style="height: 170px">

                                <legend style="color: Black; text-align: Left; font-weight: bold;">
                                    <asp:CheckBox ID="circlecheck" runat="server" AutoPostBack="true" OnCheckedChanged  ="checkuncheckcicle"  />
                                   State
                                </legend>
                                <asp:Panel ID="Panel2" runat="server" Height="150px"
                                    Style="display: block" ScrollBars="Auto">
                                <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="true" OnSelectedIndexChanged ="checkuncheckcicle1" OnCheckedChanged="checkuncheckcicle1" >
                                    </asp:CheckBoxList>
                                    </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="Left" width="30%">
                            <fieldset style="height: 170px">
                                <legend style="color: Black; text-align: Left; font-weight: bold;">
                                    <asp:CheckBox ID="Citycheck" runat="server" AutoPostBack="true" OnCheckedChanged="Citycheckuncheck" />
                                    City</legend>
                                <asp:Panel ID="Panel3" runat="server" Height="150px"
                                    Style="display: block" ScrollBars="Auto">
                                
                                    <asp:CheckBoxList ID="City" runat="server" AutoPostBack="true" OnSelectedIndexChanged="FilterUser" OnCheckedChanged="FilterUser" >
                                    </asp:CheckBoxList>
                                    </asp:Panel>
                            </fieldset>

                        </td>

                        <td align="Left" width="35%">
                            <fieldset style="height: 170px">
                                <legend style="color: Black; text-align: Left; font-weight: bold;">
                                    <asp:CheckBox ID="CheckBox1" OnCheckedChanged="checkuncheck" AutoPostBack="true" runat="server" />User-Vehicle</legend>
                                <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto"
                                    Style="display: block">


                                    <asp:CheckBoxList ID="UsrVeh" runat="server">
                                    </asp:CheckBoxList>
                                    <%--       <asp:CheckBoxList ID="smsreports"   Visible="false"  runat="server">
               
                      </asp:CheckBoxList>--%>
                                </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="center" class="m8b" width="5%">
                            <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px" ImageUrl="~/images/Search.gif"  ToolTip="Search  " /><br />
                            <input type="image" src="images/worldmap.jpg" style="width:50pxd; height:25px;" onclick="ShowMap();"  />

                        </td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="width: 70%">
                            <table cellspacing="0" cellpadding="3" rules="all" id="tblGVReport" style="border-color:Green;border-width:1px;border-style:None;font-size:Small;width:100%;border-collapse:collapse;display:none;">
    <tr class="gridheaderhome" align="center" style="color:Black;background-color:#D0D0D0;border-color:Green;border-width:1px;border-style:solid;font-weight:bold;height:25px;">
	<th scope="col" style="border-right:solid 1px Gray;">State</th><th scope="col" style="border-right:solid 1px Gray;">City</th><th scope="col" style="border-right:solid 1px Gray;">Device IMEI No.</th><th scope="col" style="border-right:solid 1px Gray;">Vehicle No</th><th scope="col" style="border-right:solid 1px Gray;">User Name</th><th scope="col" style="border-right:solid 1px Gray;">Last Movement Date &amp; Time</th><th scope="col">Current Location Address</th></tr></table>
  
                        </td>
                        
                    </tr>
                </table>
            </div>

            <div id="divdata">
                <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">                 
                 <tr>
                       
                        <td>
                            <caption>
                                <br />
                                <asp:UpdatePanel ID="upnl" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pngv" runat="server">
                                            <asp:GridView ID="GVReport" runat="server" AllowPaging="true" AllowSorting="False" AutoGenerateColumns="true" BorderColor="Green" BorderStyle="none" BorderWidth="1px" CellPadding="3" EmptyDataText="Record does not exists." Font-Size="Small" PageSize="15" ShowFooter="false" Width="100%">
                                                <RowStyle BackColor="White" BorderColor="Green" BorderWidth="1px" CssClass="gridrowhome" ForeColor="Black" Height="25px" />
                                                <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                                <HeaderStyle BackColor="#d0d0d0" BorderColor="Green" BorderWidth="1px" CssClass="gridheaderhome" Font-Bold="True" ForeColor="black" Height="25px" HorizontalAlign="Center" />
                                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                                <Columns>
                                                </Columns>
                                                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                                <SortedDescendingHeaderStyle BackColor="#93451F" />
                                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:GridView>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </caption>
                        </td>
                    </tr>
                    
                </table>
            </div>
           
        </ContentTemplate>
    </asp:UpdatePanel>
      <asp:Panel ID="PleaseWaitMessagePanel" runat="server" CssClass="modalPopup" Height="50px" Width="125px">
        Please wait &nbsp&nbsp&nbsp&nbsp&nbsp<asp:ImageButton ID="imgclose" runat="server" ImageUrl="~/images/close.png" />
        <br />
        <img src="images/uploading.gif" alt="Please wait" />
    </asp:Panel>
    <asp:Button ID="HiddenButton" runat="server" CssClass="hidden" Text="Hidden Button"
        ToolTip="Necessary for Modal Popup Extender" />
    <asp:ModalPopupExtender ID="PleaseWaitPopup" BehaviorID="PleaseWaitPopup" runat="server" TargetControlID="HiddenButton" PopupControlID="PleaseWaitMessagePanel" BackgroundCssClass="modalBackground" CancelControlID="imgclose">
    </asp:ModalPopupExtender>
     <div id="divmap" style="margin-top:10px; width:100%; height:500px;"></div> 
 </asp:Content>

