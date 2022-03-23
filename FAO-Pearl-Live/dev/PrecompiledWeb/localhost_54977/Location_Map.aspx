<%@ page language="VB" autoeventwireup="false" inherits="Location_Map, App_Web_1rjiof5j" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit"
         TagPrefix="ajax" %>
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
<title>Welcome to Nokia Maps</title>
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
		<link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css"/>
		<!-- By default we add ?with=all to load every package available, it's better to change this parameter to your use case. Options ?with=maps|positioning|places|placesdata|directions|datarendering|all -->
		<script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>
		<!-- JavaScript for example container (NoteContainer & Logger)  -->
		<script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>
       <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
		<style type="text/css">
			html {
				overflow:hidden;
			}
	.styled-button-10
	 {
	background:#5CCD00;
	background:-moz-linear-gradient(top,#5CCD00 0%,#4AA400 100%);
	background:-webkit-gradient(linear,left top,left bottom,color-stop(0%,#5CCD00),color-stop(100%,#4AA400));
	background:-webkit-linear-gradient(top,#5CCD00 0%,#4AA400 100%);
	background:-o-linear-gradient(top,#5CCD00 0%,#4AA400 100%);
	background:-ms-linear-gradient(top,#5CCD00 0%,#4AA400 100%);
	background:linear-gradient(top,#5CCD00 0%,#4AA400 100%);
	filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#5CCD00',endColorstr='#4AA400',GradientType=0);
	padding:10px 15px;
	color:#fff;
	font-family:'Helvetica Neue',sans-serif;
	font-size:16px;
	border-radius:5px;
	-moz-border-radius:5px;
	-webkit-border-radius:5px;
	border:1px solid #459A00
 }
			
			body {
				margin: 0;
				padding: 0;
				overflow: hidden;
				width: 100%;
				height: 100%;
				position: absolute;
                background-color:#F0F0F0;
			}
			
			#mapContainer {
				width: 75%;
				height: 80%;
				position: fixed;
                left :281px;
               
            }
            #Panel {
position:fixed;
top:30px;
vertical-align :top;
}.AutoExtender
        {
            font-family: Verdana, Helvetica, sans-serif;
            font-size: .8em;
            font-weight: normal;
            border: solid 1px #006699;
            line-height: 20px;
            background-color: White;
            
            Overflow:Auto;
        }
        .AutoExtenderList
        {
            border-bottom: dotted 1px #006699;
            cursor: pointer;
            color: Maroon;
        }
        .AutoExtenderHighlight
        {
            color: White;
            background-color: #006699;
            cursor: pointer;
        }
        #divwidth
        {
          width: 150px !important;    

        }
        #divwidth div
       {
        width: 150px !important;   
       }
 
		    .Inputform
            {}
 
		    .txtBox {}
 
		</style>    
		
 
     <script type="text/javascript">
         function fnSample(t1, t2, t3, t4,t5,t6, Route, Mode,Traffic) {
             nokia.Settings.set("app_id", "DemoAppId01082013GAL");
             nokia.Settings.set("app_code", "AJKnXv84fjrb0KIHawS0Tg");
             // Use staging environment (remove the line for production environment)
             nokia.Settings.set("serviceMode", "cit");
             // Enable https
             (document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");
             /* We create a UI notecontainer for example description
 * NoteContainer is a UI helper function and not part of the Nokia Maps API
 * See exampleHelpers.js for implementation details 
 */

             // Get the DOM node to which we will append the map
             var mapContainer = document.getElementById("mapContainer");
             var infoBubbles = new nokia.maps.map.component.InfoBubbles();
             var map = new nokia.maps.map.Display(mapContainer, {
                 // Initial center and zoom level of the map
                 center: [52.51676875, 13.39201495],
                 zoomLevel: 18,
                 // We add the behavior component to allow panning / zooming of the map
                 components: [
                                infoBubbles,
          // ZoomBar provides a UI to zoom the map in & out
                            new nokia.maps.map.component.ZoomBar(),
          // We add the behavior component to allow panning / zooming of the map
          // Creates a toggle button to show/hide traffic information on the map
		new nokia.maps.map.component.Traffic(),
                         new nokia.maps.map.component.Behavior(),
          // Creates UI to easily switch between street map satellite and terrain mapview modes
                      new nokia.maps.map.component.TypeSelector()
                 ]
             });

// Enable traffic
map.set("baseMapType", nokia.maps.map.Display.TRAFFIC);
                 router = new nokia.maps.routing.Manager(); // create a route manager;


             // The function onRouteCalculated  will be called when a route was calculated
             var onRouteCalculated = function (observedRouter, key, value) {
                 if (value == "finished") {
                     var routes = observedRouter.getRoutes();

                     //create the default map representation of a route
                     var mapRoute = new nokia.maps.routing.component.RouteResultSet(routes[0]).container;
                     map.objects.add(mapRoute);

                     //Zoom to the bounding box of the route
                     map.zoomTo(mapRoute.getBoundingBox(), false, "default");
                 } else if (value == "failed") {
                     alert("The routing request failed.");
                 }
             };
             
             /* We create on observer on router's "state" property so the above created
              * onRouteCalculated we be called once the route is calculated
              */
             router.addObserver("state", onRouteCalculated);

             // Create waypoints
             var waypoints = new nokia.maps.routing.WaypointParameterList();
             waypoints.addCoordinate(new nokia.maps.geo.Coordinate(t1, t2));
             waypoints.addCoordinate(new nokia.maps.geo.Coordinate(t3, t4));
             waypoints.addCoordinate(new nokia.maps.geo.Coordinate(t5, t6));
             /* Properties such as type, transportModes, options, trafficMode can be
              * specified as second parameter in performing the routing request.
              * 
              * See for the mode options the "nokia.maps.routing.Mode" section in the developer's guide
              */


            

             var modes = [{
                 type: Route,
                 transportModes: [Mode],
                 options: "avoidTollroad",
                 trafficMode: Traffic
             }];

             // Trigger route calculation after the map emmits the "displayready" event
             map.addListener("displayready", function () {
                 router.calculateRoute(waypoints, modes);
             }, false);
             var positionLogger = new Logger({
                 id: "positionLogger",
                 parent: document.getElementById("mapContainer"),
                 title: "Clicked position log"
             });

             /* We would like to add event listener on mouse click or finger tap so we check
              * nokia.maps.dom.Page.browser.touch which indicates whether the used browser has a touch interface.
              */
             var TOUCH = nokia.maps.dom.Page.browser.touch,
                 CLICK = TOUCH ? "tap" : "click";

             /* Attach an event listener to map display
              * push info bubble with coordinate information to map
              */
             map.addListener(CLICK, function (evt) {
                 var coord = map.pixelToGeo(evt.displayX, evt.displayY);
                 /* We create an infobubble using infoBubbles.openBubble.
                  * 
                  * openBubble(content, coordinate, onUserClose, hideCloseButton) takes for parameters 
                  * 		- content: to be shown in the info bubble;
                  * 		 	it can be an HTML string or an instance of nokia.maps.search.Location
                  * 		- coordinate: An object containing the geographic coordinates 
                  * 			of the location, where the bubble's anchor is to be placed
                  * 		- onUserClose: A callback method which is called when bubble is closed
                  * 		- hideCloseButton: Hides close button if set to true.
                  */
                 infoBubbles.openBubble("Clicked at " + coord, coord);

                 // Clear the logger
                 positionLogger.clear();

                 // We now print the latitude & longitude to the logger
                 positionLogger.log(
                     "Clicked at position: <br />latitude: " +
                     coord.latitude + "<br /> longitude: " + coord.longitude);
             });


             /* We create a UI notecontainer for example description and controls
 * NoteContainer is a UI helper function and not part of the Nokia Maps API
 * See exampleHelpers.js for implementation details 
 */
             var noteContainer = new NoteContainer({
                 id: "movingUi",
                 parent: document.getElementById("uiContainer"),
                 title: "Panning the map",
                 content:
		'<input id="moveTo" role="button" type="button" value="move to" visible="false"/><br />' +
		'<p>Move map by pan()</p>' +
		'<input id="panLeft" role="button" type="button" value="pan left"/><br />' +
		'<input id="panRight" role="button" type="button" value="pan right"/><br />' +
		'<input id="panUp" role="button"  type="button" value="pan up"/><br />' +
		'<input id="panDown" role="button" type="button" value="pan down"/><br />' +
		'<p id="centerCoord"></p>' +
		'<p id="zoomlevel"></p>'
             });


             // Binding of DOM elements to several variables so we can install event handlers.
             var moveToUiElt = document.getElementById("moveTo"),
               centerUiElt = document.getElementById("centerCoord"),
                 zoomlevelUiElt = document.getElementById("zoomlevel"),
                 leftUiElt = document.getElementById("panLeft"),
                 rightUiElt = document.getElementById("panRight"),
                 upUiElt = document.getElementById("panUp"),
                 downUiElt = document.getElementById("panDown"),
                 // Geo coordinate of the Big Ben in London, United Kingdom
                 coord = new nokia.maps.geo.Coordinate(t1, -t2);
             /* We create a UI notecontainer for example description
              * NoteContainer is a UI helper function and not part of the Nokia Maps API
              * See exampleHelpers.js for implementation details 
              */

             // Move map using map's set() by changing its center.
             moveToUiElt.onclick = function () {
                 /* map.set(x, y) takes two arguments;
                  * 		- x: The key needs to be set.
                  * 		- y: The new value to be set
                  *
                  * Example to set map center to [53, 13]
                  *	 latitude: 53, longitude: 13
                  * There are two other ways to specify a Coordinate:
                  * map.set("center", {lat: 53, lng: 13});
                  * map.set("center", new nokia.maps.geo.Coordinate(53,13));
                  */
                 map.set("center", coord);
             };

             // Move map with map's pan()
             leftUiElt.onclick = function () {
                 /* Pans the map by the delta defined by start and end point.
                  *
                  * pan(startX, startY, endX, endY, animation) takes four arguments:
                  * 		- startX: The x-position of the pixel relative to the top-left 
                  * 				corner of the current view from where to start pan.
                  * 		- startY: The y-position of the pixel relative to the top-left 
                  * 				corner of the current view from where to start pan.
                  * 		- endX: The x-position of the pixel relative to the top-left 
                  * 				corner of the current view to where to pan.
                  * 		- endY: The y-position of the pixel relative to the top-left 
                  * 				corner of the current view to where to pan.
                  * 		- [animation]: Optional argument. 
                  *  			The animation to be used while modifying 
                  * 				the view, must be a value from the animation list.
                  * 				The list can be found in map.animation.
                  */
                 map.pan(0, 0, -30, 0, "default");
             };
             rightUiElt.onclick = function () {
                 map.pan(0, 0, 30, 0, "default");
             };
             upUiElt.onclick = function () {
                 map.pan(0, 0, 0, -30, "default");
             };
             downUiElt.onclick = function () {
                 map.pan(0, 0, 0, 30, "default");
             };

         }
    </script>
</head>
<body>
     <asp:Panel ID="PanelMain" runat="server" ScrollBars="Auto">
               
    <div>
    <form id="form1" runat="server">

    <div id="Fields" aria-posinset="" style="text-align:left" >
   
           <br />
         <div align="center">
          <asp:Label ID="Label14" runat="server" Text="Nokia Test Map" Font-Bold="true" 
                 style="font-size:medium; color: #006666; text-decoration: underline;"></asp:Label>
         </div>
         
            <br />
  <div id="mapContainer" style="border:solid" >
          </div>
  
        <div id="uiContainer"></div>

            <br />
               &nbsp;&nbsp;
      <asp:Panel ID="pnldynamically" runat="server" Width="300px">
         
               <asp:Label ID="Label1" runat="server" Text="Source Location :" style="font-weight:bold; font-size:12px;" ></asp:Label><br />
        
       <%-- <ajax:ToolkitScriptManager ID="ScriptManager1" runat="server"/>    --%>
        &nbsp;&nbsp; <asp:TextBox ID="TextBox1" runat="server" CssClass="txtBox" Width="240px" Height="16px" Wrap="true"></asp:TextBox><br />
       <%-- <ajax:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" TargetControlID="TextBox1"  MinimumPrefixLength="1" EnableCaching="true"  CompletionInterval="1000" ServiceMethod="GetStartLocation" CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList" CompletionListHighlightedItemCssClass="AutoExtenderHighlight" ShowOnlyCurrentWordInCompletionListItem="true" >
</ajax:AutoCompleteExtender>--%>
        
               &nbsp;<asp:Label ID="Label2" runat="server" style="font-weight:bold; font-size:12px;" Text="Destination Location:"></asp:Label>
               <br />
               &nbsp;&nbsp;
               <asp:TextBox ID="TextBox2" runat="server" CssClass="txtBox" Height="17px" Width="240px"></asp:TextBox>
               <%--<ajax:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" TargetControlID="TextBox2" MinimumPrefixLength="1" EnableCaching="true" CompletionSetCount="1" CompletionInterval="1000" ServiceMethod="GetDestLocation" CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList" CompletionListHighlightedItemCssClass="AutoExtenderHighlight" ShowOnlyCurrentWordInCompletionListItem="true">
</ajax:AutoCompleteExtender>--%>
               <br />
               &nbsp;<asp:Label ID="Label18" runat="server" Text="Third Location:" style="font-weight:bold; font-size:12px;"></asp:Label><br />   
        
               &nbsp; <asp:TextBox ID="TextBox3" runat="server" CssClass="txtBox"  Width="240px" Height="17px"></asp:TextBox>
             <%--<ajax:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" TargetControlID="TextBox2" MinimumPrefixLength="1" EnableCaching="true" CompletionSetCount="1" CompletionInterval="1000" ServiceMethod="GetDestLocation" CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList" CompletionListHighlightedItemCssClass="AutoExtenderHighlight" ShowOnlyCurrentWordInCompletionListItem="true">
</ajax:AutoCompleteExtender>--%>
               &nbsp;&nbsp;&nbsp;<br />&nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label13" runat="server" Text="Advanced Options" style="font-weight:bold; font-size:12px;"></asp:Label>
           <br />
               &nbsp;<asp:Label ID="Label17" runat="server" Text="Mode" style="font-weight:bold; font-size:12px; direction: ltr; display: inline-block;" ></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:DropDownList ID="DropDownList3" runat="server" CssClass="txtBox" Width="160px" Height="28px" AutoPostBack="true">
                <asp:ListItem Selected="True">car</asp:ListItem>
                <asp:ListItem>pedestrian</asp:ListItem>
                <asp:ListItem>truck</asp:ListItem>
            </asp:DropDownList>
               <asp:Label ID="Label15" runat="server" style="font-weight:bold; font-size:12px; direction:ltr; display:inline-block;" Text="Route Options"></asp:Label>
               &nbsp;&nbsp;&nbsp;
               <asp:DropDownList ID="DropDownList1" runat="server" CssClass="txtBox" Height="28px" Width="160px" AutoPostBack="true">
                                 </asp:DropDownList>
          
               <br />
               <asp:Label ID="Label16" runat="server" style="font-weight:bold; font-size:12px;direction:ltr; display:inline-block;" Text="Traffic"></asp:Label>
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
               <asp:DropDownList ID="DropDownList2" runat="server" CssClass="txtBox" Height="28px" Width="160px">
                 
               </asp:DropDownList>
            <br />
               &nbsp; &nbsp;&nbsp;
               <asp:Button ID="Button1" runat="server" CssClass="styled-button-10" Height="42px" Text="Show" />
            <br />

            <br />
       
        
          </asp:Panel>
            </div>
        <div id="Panel">
            <asp:Panel ID="Panel1" runat="server">
            </asp:Panel>

        </div>
        <div>
             <asp:Panel ID="PanelInstruction" runat="server" Width="250px" Visible="false">
                 <asp:Label ID="Label9" runat="server" Text="Source Lat & Long" style="font-weight:bold; font-size:12px;"></asp:Label>
:<br /> 
                 <asp:Label ID="Label3" runat="server" Text="" ></asp:Label>
                 
                 <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                 <br />
                 <asp:Label ID="Label10" runat="server" Text="Destination Lat &amp; Long" style="font-weight:bold; font-size:12px;"></asp:Label>
                 <br />
                 <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                 <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                 <br />
                 <asp:Label ID="Label19" runat="server" Text="Third Location Lat &amp; Long" style="font-weight:bold; font-size:12px;"></asp:Label><br />
                 <asp:Label ID="Label20" runat="server" Text=""></asp:Label>
                 <asp:Label ID="Label21" runat="server" Text=""></asp:Label>
                 <br />
                 <asp:Label ID="Label7" runat="server" Text="Distance :" style="font-weight:bold; font-size:12px;"></asp:Label>
                 <asp:Label ID="Label11" runat="server" Text="Distance"></asp:Label>
                 <br />
            <asp:Label ID="Label8" runat="server" Text="Time Taken" style="font-weight:bold; font-size:12px;"></asp:Label>
            <asp:Label ID="Label12" runat="server" Text="Time"></asp:Label>
            <br />
                  <asp:Label ID="Label22" runat="server"  Text="Path" style="font-weight:bold; font-size:12px; text-align:left"></asp:Label>
                 <br />
                 <asp:TextBox ID="txtroute" runat="server" BorderStyle="None" TextMode="MultiLine" Height="300px"  Width="250px"></asp:TextBox>
                  <asp:Button ID="btnnewRoute" runat="server" Text="New Route" CssClass="styled-button-10" 
               Height="42px"  />
            </asp:Panel>
            </div>
        
    </form>
        </div>

         </asp:Panel>
</body>
</html>
