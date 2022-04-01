<%@ page language="VB" autoeventwireup="false" inherits="Report_Map, App_Web_01howaz0" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<head>

<link href="css/style.css" rel="Stylesheet" type="text/css" />
<link rel="stylesheet" type="text/css" href="/resources/demos/style.css"/>
<link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css"/>

<script src="http://code.jquery.com/jquery-1.9.1.js" type="text/javascript"></script>
<script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
<script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false"></script> 
<script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>
<script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>
<script src="Scripts/NokiaGoogleMap.js" type="text/javascript"></script>
<link href="StyleSheet.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
   $(document).ready(function () { ShowMap(); });
    function ShowMap() {
        $.ajax({
            type: "post",
            url: "Report_Map.aspx/ConvertDataTabletoString",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) { chk(data); },
            error: function (data) { chk(data); }
        });
    }
    function chk(abc)
    {
        debugger;
        if (abc.d == null)
        {
           
        }
        else
        {
            if (abc.d[0].hasOwnProperty("GeoFence"))
            {
                CreatePolygonNokiaReport(abc.d);
            }            
           else
            {
                ReportMaster(abc.d);
            }
        }
        
        
    }
  
    function ReportMaster(data) {
        try {
            document.getElementById("dvMap").style.display = "block";
            nokia.Settings.set("app_id", "VG3IAYYwc1Y7XaBWEqU9");
            nokia.Settings.set("app_code", "R7W2h1KNHKBqJsJOkRbTiw");
            nokia.Settings.set("serviceMode", "cit");
            (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

            var mapContainer = document.getElementById("dvMap");
            var infoBubbles = new nokia.maps.map.component.InfoBubbles();

            var map = new nokia.maps.map.Display(mapContainer, {
                center: [28.61, 77.23],
                zoomLevel: 5,

                components: [infoBubbles,
                               new nokia.maps.map.component.ZoomBar(),
           new nokia.maps.map.component.Traffic(),
                            new nokia.maps.map.component.Behavior(),
             new nokia.maps.map.component.TypeSelector()]

            });

            var redMarker;
            var markersContainer = new nokia.maps.map.Container();

            var Buttonid = '<input type="image" ID="btnEdit1" src="././images/edit.jpg" style="height:26px;Width:75px;" class="click"  AlternateText="Edit" />';
            var Buttonid2 = '<input type="image" ID="btnDtl1" src="././images/lock.png" style="height:26px;Width:75px;" class="click"  ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />';


            for (var i = 0; i < data.length; i++) {

                if (data[i].GeoPoint == null) {
                    continue;
                }
                latlong = data[i].GeoPoint.split(",");
                if (latlong.length > 1) {
                    var x = parseFloat(latlong[0]);
                    var y = parseFloat(latlong[1]);
                }
                else {
                    continue;
                }
                var imageMarker = new nokia.maps.map.Marker([x, y], {
                    icon: "images/Nokia2.png",
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
                imageMarker.html = "Latitude:" + latlong[0] + "" + '</br>' + "Longitude:" + latlong[1] + "" +  '<br/>';
                var TOUCH = nokia.maps.dom.Page.browser.touch,
                CLICK = TOUCH ? "tap" : "mouseover";
                imageMarker.addListener(CLICK, function (evt) {
                    infoBubbles.openBubble(this.html, this.coordinate);
                    ntid = (evt.target.$html);
                });
            }

        }
        catch (e) {
            alert(e.message);
        }
    }


</script>

<style type="text/css"> 
/*Modal Popup*/ 
.modalBackground 
{
background-color: Gray; 
filter: alpha(opacity=70); 
opacity: 0.7; 
}
.modalPopup 
{
background-color: White; 
border-width: 3px; 
border-style: solid; 
border-color: Gray; 
padding: 3px; 
text-align: center; 
}
.hidden 
{
display: none; 
}
</style> 

</head>
<body>
    <form id="form1" runat="server">
<asp:ScriptManager ID="scmge"  runat="server"></asp:ScriptManager>
<asp:Panel ID="PnlMap" runat="server">
<table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
            border="0px">
            <tr>
                <td style="width: 210px;"> 
                   </td>
                <td style="width: 210px; text-align:left;">
                </td>
                <td style="width: 210px;">                                    
                </td>
                <td style="width: 210px;">
                    <div>
                        <asp:Label ID="Map" runat="server"></asp:Label>
                        <asp:HiddenField ID="hdntid" runat="server" />
                        
                    </div>
                </td>
                <td style="text-align: right; width: 25px">  
                    <%--<asp:Button ID="btnsavedata" runat="server" Text="Save" OnClick="btnsavedata_Click" />--%>
                </td>
            </tr>            
        </table>
        <div id="dvMap" style="width: 1000px; height: 700px;"></div>  
         <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
            border="0px">
              
             <tr>
                <td style="width: 210px;">    <label id="lblCity"></label></td>
                  
                <td style="width: 210px;">
                   

                </td>



                <td style="width: 210px;"></td>

                <td style="width: 210px;">
                   
                </td>

                <td style="text-align: right; width: 25px">
                   
                </td>
                 </tr>
        </table>
    </asp:Panel>    
<asp:UpdatePanel ID="upsa" runat="server">
<ContentTemplate>          
        </ContentTemplate>            
       </asp:UpdatePanel> 
        </form>
</body>