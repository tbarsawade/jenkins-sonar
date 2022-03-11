<%@ page language="VB" autoeventwireup="false" masterpagefile="~/USR.master" inherits="GMapTest, App_Web_dqvq3srr" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="jquery/jquery-1.3.2.min.js"></script>
    
    <script type="text/javascript" charset="UTF-8" src="http://maps.gstatic.com/cat_js/maps-api-v3/api/js/18/0/%7Bcommon,util,infowindow%7D.js"></script>
<script type="text/javascript" charset="UTF-8" src="http://maps.gstatic.com/cat_js/maps-api-v3/api/js/18/0/%7Bmap%7D.js"></script>
<script type="text/javascript" charset="UTF-8" src="http://maps.gstatic.com/cat_js/maps-api-v3/api/js/18/0/%7Bmarker%7D.js"></script>
<script type="text/javascript" charset="UTF-8" src="http://maps.gstatic.com/cat_js/maps-api-v3/api/js/18/0/%7Bonion%7D.js"></script>
<script type="text/javascript" charset="UTF-8" src="http://maps.gstatic.com/cat_js/maps-api-v3/api/js/18/0/%7Bcontrols%7D.js"></script>
<script type="text/javascript" charset="UTF-8" src="http://maps.gstatic.com/cat_js/maps-api-v3/api/js/18/0/%7Bstats%7D.js"></script>
    <style type="text/css">
        .hide {
            display:none;
        }

.lstContent option:hover
{
    background-color:#DEEFFF;
    cursor:pointer;
    color:blue;
}

    </style>
    <script type="text/javascript">
        var myMarkersArray = [];
        var RemovedMarkers = myMarkersArray.slice(0);

        function ShowHideMap(sender)
        {
            var group = $(sender).attr("value");

            if ($(sender).is(':checked')) {
              
                for (i = 0; i < RemovedMarkers.length; i++)
                {
                    if (RemovedMarkers[i].group == group) {
                        myMarkersArray.push(RemovedMarkers[i])
                        
                        RemovedMarkers[i].setVisible(true);
                        RemovedMarkers.splice(i, 1);
                        i=-1;
                    }
                }

            } else {
                
               
                for (i = 0; i < myMarkersArray.length; i++)
                {
                    if((myMarkersArray[i].group==group))
                    {
                        RemovedMarkers.push(myMarkersArray[i])
                        myMarkersArray[i].setVisible(false);
                        myMarkersArray.splice(i, 1);
                        
                        i=-1;
                    }
                }
                
            }


        }

        function focusMarker(sender)
        {
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

    </script>

    <script type="text/javascript">

        $(document).ready(function () {
            CacheValues();
            var map;
            //var locations=  $('#').append(op);
            //var locations = $('#<%= hdndata.ClientID()%>').attr("value");

            function initialize() {
                 map = new google.maps.Map(document.getElementById('map'), { zoom: 7, center: new google.maps.LatLng(19.8761653, 75.3433139), mapTypeId: google.maps.MapTypeId.ROADMAP });
                var infowindow = new google.maps.InfoWindow(); var lineCoordinates = [];
                for (i = 0; i < locations.length; i++) {
                    lineCoordinates.push(new google.maps.LatLng(0, 0));
                }
                var FrPath = new google.maps.Polyline({ path: lineCoordinates, strokeColor: 'None' });
                FrPath.setMap(map);
                debugger;
                for (i = 0; i < locations.length; i++) {
                    marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map, icon: locations[i][3], group: locations[i][4], size: new google.maps.Size(1, 1) });
                    marker.primaryKey = locations[i][5];
                    myMarkersArray.push(marker);
                    google.maps.event.addListener(marker, 'click', (function (marker, i) {
                        return function () {
                            infowindow.setContent(locations[i][0]); infowindow.open(map, marker);
                        }
                    })(marker, i));
                }
            } google.maps.event.addDomListener(window, 'load', initialize);
        });


       
       
    </script>


    <script type="text/javascript">

        var myVals = new Array();
        
        function CacheValues()
        {
            var l = document.getElementById('<%= LstVehicle.ClientID%>');
 
            for (var i=0; i < l.options.length; i++)
            {
                
                myVals[i] = l.options[i];
            }
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
                    var op = '<option onclick="focusMarker(this);" value=' + myVals[i].value + '>' + myVals[i].text + '</option>';
                    $('#<%= LstVehicle.ClientID%>').append(op);

                }
            }
            else {


                for (var i = 0; i < myVals.length; i++) {

                    if (myVals[i].text.toLowerCase().match(tb.value.toLowerCase()))
                    {
                        
                        var op = '<option  onclick="focusMarker(this);" value=' + myVals[i].value + '>' + myVals[i].text + '</option>';
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
        

        <input type="hidden" id="hdndata" runat="server" value="" />

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
                                    <asp:Panel ID="Panel2" runat="server" Height="100px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="false" OnTextChanged="FilterSite">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                    <br />
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">Select Vehicle Type</legend>
                                    <asp:Panel ID="Panel1" runat="server" Height="200px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="chkvtype" runat="server" AutoPostBack="false" OnTextChanged="FilterSite">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                    <asp:Panel ID="Panel3" runat="server" Height="320px" Style="display: block" ScrollBars="Auto">
                                        <div>
                                            <input type="text" id="VehSearch" style="width:98%;" onkeyup="return SearchList();" />
                                        </div>
                                        <div >
                                        <asp:ListBox ID="LstVehicle" onchange="focusMarker(this);" runat="server" class="lstContent" Height="150px" Width="98%" DataTextField="Vehicle">
                                        </asp:ListBox>
                                        </div>
                                    </asp:Panel>
                            </div>
                        </td>
                        <%--</fieldset>--%>
                        <td style="width: 87%; height: 770px;" valign="top" >
                            <div id="map" style="width: 100%; height: 100%;">
                                <img src="images/Nokiamap.jpg" height="100%" width="100%" />
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
                         <table width="100%" border="1">
                                <tr>
                                    <td>
                                        <asp:Image ID="Image5" runat="server" ImageUrl="~/images/blue.png" />
                                    </td>
                                    <td>
                                        &nbsp; Strategic</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image6" runat="server" ImageUrl="~/images/darkyellow.png" />
                                    </td>
                                    <td>
                                        &nbsp; Normal Hub</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image7" runat="server" ImageUrl="~/images/darkk.png" />
                                    </td>
                                    <td>
                                        &nbsp; Non Strategic</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image8" runat="server" ImageUrl="~/images/lightblue.png" />
                                    </td>
                                    <td>
                                        &nbsp; BSC</td>
                                </tr>
                                <tr>
                                    <td width="20%">
                                        <asp:Image ID="img" runat="server" ImageUrl="~/images/car1.png" />
                                    </td>
                                    <td width="80%">
                                        &nbsp; No Vehicle Data more than 24 Hrs.</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/car2.png" />
                                    </td>
                                    <td>
                                        &nbsp; Vehicle is Moving</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/car4.png" />
                                    </td>
                                    <td>
                                        &nbsp; Vehicle is Halted 10 minutes to 4 Hours</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image4" runat="server" ImageUrl="~/images/car5.png" />
                                    </td>
                                    <td>
                                        &nbsp; Vehicle is Halted more than 4 Hours</td>
                                </tr>
                            </table>
                          </td>
                    </tr>
                </table>
                </ContentTemplate>
        </asp:UpdatePanel>
               </div>

    
  

</asp:Content>
