<%@ Page Title="" Language="VB" MasterPageFile="~/PublicMaster.master" AutoEventWireup="false" CodeFile="GeoCode.aspx.vb" Inherits="GeoCode" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script src="http://js.api.here.com/v3/3.0/mapsjs-core.js" type="text/javascript" charset="utf-8"></script>
		<script src="http://js.api.here.com/v3/3.0/mapsjs-service.js" type="text/javascript" charset="utf-8"></script>
		<script src="http://js.api.here.com/v3/3.0/mapsjs-mapevents.js" type="text/javascript" charset="utf-8"></script>
		<script src="http://js.api.here.com/v3/3.0/mapsjs-ui.js" type="text/javascript" charset="utf-8"></script>
		<%--<script src="http://js.api.here.com/v3/3.0/mapsjs-pano.js" type="text/javascript" charset="utf-8"></script>--%>
		<link rel="stylesheet" type="text/css" href="http://js.api.here.com/v3/3.0/mapsjs-ui.css" />
		<%--<link rel="stylesheet" type="text/css" href="http://tcs.navteq.com/http-proxy2/Examples/src/css/defaults.css" />--%>
    <link href="css/Default.css" rel="stylesheet" />
    <script src="js/jquery-1.9.1.min.js"></script>


     <div style="width:100%; height:40px; background:#e1e1e1; padding:15px;">
       <table>
           <tr>
               <td>Uplaoa Excel : </td>
               <td>
                   <asp:FileUpload ID="FileUpload1" runat="server" />
               </td>
               <td>
                   <asp:Button ID="btnGeoCode" runat="server" Text="Upload" />
                  
                   <input type="button" value="Show on map" onclick="PlotMarkers();"
               </td>
           </tr>
       </table>

       <asp:HiddenField ID="hdnloc" ClientIDMode="Static" Value="0" runat="server" />

   </div>
    <div id="mapContainer" style="width:76% !important;"/>
        <script type="text/javascript">

            var hidpi = ('devicePixelRatio' in window && devicePixelRatio > 1);

            var mapContainer = document.getElementById('mapContainer'),

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
				    zoom: 4
				}
			);

            // add behavior control
            new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

            // Enable the default UI
            var ui = H.ui.UI.createDefault(map, maptypes);

            // setup the Streetlevel imagery
            // platform.configure(H.map.render.panorama.RenderEngine);


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
                                        svg.replace(/__NO__/g, "1")
                                        .replace(/__FILLCOLOR__/g, "#FF0000")),
                            new H.map.Icon(svg.replace(/__NO__/g, "2")
                                        .replace(/__FILLCOLOR__/g, "#FF0000")),
                            new H.map.Icon(svg.replace(/__NO__/g, "3")
                                        .replace(/__FILLCOLOR__/g, "#00FF00")),
                            new H.map.Icon(svg.replace(/__NO__/g, "4")
                                        .replace(/__FILLCOLOR__/g, "#0000FF")),
                            new H.map.Icon(svg.replace(/__NO__/g, "5")
                                        .replace(/__FILLCOLOR__/g, "#F0F000"))
            ];


            //var coordinates = [new mapsjs.geo.Point(12.9244404000, 80.1534195000), new mapsjs.geo.Point(13.0025000000, 80.2561200000), new mapsjs.geo.Point(13.0025000000, 80.2561200000), new mapsjs.geo.Point(23.0307903000, 72.4653778000), new mapsjs.geo.Point(12.9240303000, 77.6528778000), new mapsjs.geo.Point(28.6642494000, 77.2080078000), new mapsjs.geo.Point(28.5634003000, 77.2653275000), new mapsjs.geo.Point(12.8403101000, 77.6774902000), new mapsjs.geo.Point(12.3256998000, 76.6320496000), new mapsjs.geo.Point(19.2271404000, 73.1130829000), new mapsjs.geo.Point(22.7254391000, 75.8909073000), new mapsjs.geo.Point(15.1958399000, 76.6763000000), new mapsjs.geo.Point(17.7229200000, 83.3174800000), new mapsjs.geo.Point(36.8367691000, -77.7738190000), new mapsjs.geo.Point(22.2488403000, 84.8124084000), new mapsjs.geo.Point(30.3095100000, 78.0489100000), new mapsjs.geo.Point(22.3920097000, 82.7466507000), new mapsjs.geo.Point(22.2612305000, 84.8742599000), new mapsjs.geo.Point(25.5932598000, 85.1532516000), new mapsjs.geo.Point(11.9347200000, 79.8123600000), new mapsjs.geo.Point(26.4594000000, 83.5853500000), new mapsjs.geo.Point(28.6312695000, 77.3716125000), new mapsjs.geo.Point(30.3451900000, 76.3831100000), new mapsjs.geo.Point(24.3456700000, 81.5940600000), new mapsjs.geo.Point(23.0450096000, 72.5208282000), new mapsjs.geo.Point(26.9021700000, 75.7884600000), new mapsjs.geo.Point(17.7328396000, 83.3178711000), new mapsjs.geo.Point(22.7323500000, 71.6279100000), new mapsjs.geo.Point(15.3463300000, 75.1480000000), new mapsjs.geo.Point(15.4585600000, 73.8066788000), new mapsjs.geo.Point(15.4833698000, 73.8093109000), new mapsjs.geo.Point(37.3402214000, -88.1012726000), new mapsjs.geo.Point(19.6485300000, 83.2671600000), new mapsjs.geo.Point(20.3514996000, 72.9318771000), new mapsjs.geo.Point(12.9675903000, 77.6079483000), new mapsjs.geo.Point(19.1203594000, 72.9994965000), new mapsjs.geo.Point(28.5956993000, 77.3368988000), new mapsjs.geo.Point(19.9044304000, 75.3465424000), new mapsjs.geo.Point(24.6132107000, 73.6963196000), new mapsjs.geo.Point(22.5308895000, 88.3588333000), new mapsjs.geo.Point(28.6070194000, 77.3441315000), new mapsjs.geo.Point(13.0728903000, 80.2456665000), new mapsjs.geo.Point(18.5914001000, 73.7620392000), new mapsjs.geo.Point(30.7034206000, 76.7894516000), new mapsjs.geo.Point(12.9762497000, 77.6968002000), new mapsjs.geo.Point(21.2771301000, 81.6732788000), new mapsjs.geo.Point(18.4907207000, 73.8538208000), new mapsjs.geo.Point(13.0262604000, 80.2680664000), new mapsjs.geo.Point(17.4800491000, 78.3562927000), new mapsjs.geo.Point(19.0635796000, 72.8252182000), new mapsjs.geo.Point(22.7350502000, 75.8957367000), new mapsjs.geo.Point(8.5122404000, 76.9239120000), new mapsjs.geo.Point(28.5627594000, 77.1619034000), new mapsjs.geo.Point(22.5631199000, 88.3219299000), new mapsjs.geo.Point(13.0523596000, 80.1871796000), new mapsjs.geo.Point(13.0788403000, 80.2471313000), new mapsjs.geo.Point(12.9063301000, 77.5875168000), new mapsjs.geo.Point(23.1562099000, 77.4128265000), new mapsjs.geo.Point(19.1321106000, 72.9233017000), new mapsjs.geo.Point(19.0799198000, 72.9985123000), new mapsjs.geo.Point(12.9502497000, 80.1990738000), new mapsjs.geo.Point(27.1844006000, 73.7371597000), new mapsjs.geo.Point(28.4149500000, 76.9922600000), new mapsjs.geo.Point(22.3296108000, 70.7688980000), new mapsjs.geo.Point(13.0310802000, 80.2157974000), new mapsjs.geo.Point(1.2901700000, 103.8519974000), new mapsjs.geo.Point(28.4944496000, 77.0898972000), new mapsjs.geo.Point(12.9009199000, 77.6010513000), new mapsjs.geo.Point(19.1415100000, 72.8245621000), new mapsjs.geo.Point(12.9408398000, 77.6026230000), new mapsjs.geo.Point(12.9947100000, 80.2574310000), new mapsjs.geo.Point(28.7050304000, 77.2738266000), new mapsjs.geo.Point(22.3124905000, 73.1644669000), new mapsjs.geo.Point(28.4507103000, 77.3137283000), new mapsjs.geo.Point(21.1383209000, 79.0593872000), new mapsjs.geo.Point(20.2880192000, 85.8126907000), new mapsjs.geo.Point(13.0321198000, 77.6499786000), new mapsjs.geo.Point(28.5020905000, 77.0288696000), new mapsjs.geo.Point(23.2769909000, 77.4599380000), new mapsjs.geo.Point(28.4312191000, 77.1048203000), new mapsjs.geo.Point(19.1562805000, 72.9375076000), new mapsjs.geo.Point(28.4800301000, 77.0601883000), new mapsjs.geo.Point(28.5752296000, 77.1588211000), new mapsjs.geo.Point(28.4944496000, 77.0898972000), new mapsjs.geo.Point(17.5135307000, 78.4799576000), new mapsjs.geo.Point(18.4998398000, 73.8034897000), new mapsjs.geo.Point(22.4717007000, 88.3564835000), new mapsjs.geo.Point(13.0646200000, 80.1862411000), new mapsjs.geo.Point(12.9222298000, 77.6028519000), new mapsjs.geo.Point(13.0593500000, 80.2373428000), new mapsjs.geo.Point(13.1445799000, 80.2551498000), new mapsjs.geo.Point(22.7008896000, 75.8704681000), new mapsjs.geo.Point(28.4647598000, 77.0218582000), new mapsjs.geo.Point(12.9003696000, 77.5706711000), new mapsjs.geo.Point(19.1091995000, 72.9200211000), new mapsjs.geo.Point(19.0374699000, 72.9248505000), new mapsjs.geo.Point(23.1634903000, 77.4389038000), new mapsjs.geo.Point(28.5943699000, 77.3057938000), new mapsjs.geo.Point(13.0621996000, 77.5859375000), new mapsjs.geo.Point(28.6334095000, 77.3504868000), new mapsjs.geo.Point(12.9922304000, 77.7161407000), new mapsjs.geo.Point(23.0867996000, 72.5448990000), new mapsjs.geo.Point(18.4982605000, 73.9411316000), new mapsjs.geo.Point(13.0379896000, 77.5930710000), new mapsjs.geo.Point(22.5882893000, 88.3597794000), new mapsjs.geo.Point(28.4199104000, 77.0600967000), new mapsjs.geo.Point(15.4092102000, 73.9921036000), new mapsjs.geo.Point(23.2125092000, 77.4271011000), new mapsjs.geo.Point(13.0608797000, 80.1573105000), new mapsjs.geo.Point(12.9179296000, 77.6161804000), new mapsjs.geo.Point(21.2472800000, 81.6412200000), new mapsjs.geo.Point(18.6206398000, 73.9482803000), new mapsjs.geo.Point(19.1150093000, 72.9085693000), new mapsjs.geo.Point(13.1942997000, 80.0715866000), new mapsjs.geo.Point(17.4551506000, 78.4830780000), new mapsjs.geo.Point(17.3645306000, 78.5428009000), new mapsjs.geo.Point(23.0793591000, 72.5013885000), new mapsjs.geo.Point(12.9391003000, 77.6233215000), new mapsjs.geo.Point(20.4575500000, 85.9220276000), new mapsjs.geo.Point(12.9954596000, 77.5452576000), new mapsjs.geo.Point(20.3248901000, 85.8113098000), new mapsjs.geo.Point(13.0024996000, 80.2561188000), new mapsjs.geo.Point(19.0771408000, 72.8659592000), new mapsjs.geo.Point(18.9955502000, 72.8390274000), new mapsjs.geo.Point(12.9662304000, 77.5215607000), new mapsjs.geo.Point(13.0047998000, 77.5577164000), new mapsjs.geo.Point(12.9354095000, 80.2347183000), new mapsjs.geo.Point(13.0024996000, 80.2561188000), new mapsjs.geo.Point(28.6362896000, 77.0923920000), new mapsjs.geo.Point(13.0434399000, 77.5757217000), new mapsjs.geo.Point(17.4222393000, 78.3371887000), new mapsjs.geo.Point(18.4708805000, 73.8601532000), new mapsjs.geo.Point(12.9379702000, 80.1327515000), new mapsjs.geo.Point(28.3964005000, 77.2745285000), new mapsjs.geo.Point(17.3814602000, 78.4289474000), new mapsjs.geo.Point(20.3785706000, 72.9260864000), new mapsjs.geo.Point(18.9873009000, 72.8243027000), new mapsjs.geo.Point(28.6662407000, 77.0798492000), new mapsjs.geo.Point(26.8432007000, 80.9150925000), new mapsjs.geo.Point(12.9752502000, 80.1903763000), new mapsjs.geo.Point(28.6963005000, 77.2810898000), new mapsjs.geo.Point(17.4671993000, 78.4809036000), new mapsjs.geo.Point(17.4245396000, 78.3929977000), new mapsjs.geo.Point(26.8756504000, 80.9581223000), new mapsjs.geo.Point(12.9357004000, 77.6283875000), new mapsjs.geo.Point(18.9183693000, 72.8270264000), new mapsjs.geo.Point(22.5673504000, 88.4916000000), new mapsjs.geo.Point(28.5350304000, 77.2935562000), new mapsjs.geo.Point(18.9141808000, 72.8234024000), new mapsjs.geo.Point(28.6324196000, 77.1389694000), new mapsjs.geo.Point(28.5897808000, 77.0399170000), new mapsjs.geo.Point(12.9951096000, 77.5714798000), new mapsjs.geo.Point(63.3062515000, 10.3481798000), new mapsjs.geo.Point(12.9000597000, 77.5851517000), new mapsjs.geo.Point(26.1377300000, 91.6236900000), new mapsjs.geo.Point(28.5731506000, 77.1599884000), new mapsjs.geo.Point(17.3671303000, 78.4883499000), new mapsjs.geo.Point(27.2414207000, 78.0096283000), new mapsjs.geo.Point(17.4330807000, 78.4377594000), new mapsjs.geo.Point(12.9812298000, 77.6935196000), new mapsjs.geo.Point(28.5636101000, 77.1941681000), new mapsjs.geo.Point(28.6370792000, 77.1811066000), new mapsjs.geo.Point(22.4773598000, 75.9468765000), new mapsjs.geo.Point(30.7346802000, 76.7423782000), new mapsjs.geo.Point(28.5784893000, 77.3239975000), new mapsjs.geo.Point(8.5312204000, 76.9106522000), new mapsjs.geo.Point(12.3089500000, 76.6532600000), new mapsjs.geo.Point(16.8422394000, 81.5267868000), new mapsjs.geo.Point(20.3890209000, 72.8975220000), new mapsjs.geo.Point(10.9941700000, 76.9663100000), new mapsjs.geo.Point(15.4625196000, 73.8478394000), new mapsjs.geo.Point(15.3858995000, 75.0826035000), new mapsjs.geo.Point(28.4456596000, 77.0667267000), new mapsjs.geo.Point(15.5894604000, 73.8095169000), new mapsjs.geo.Point(28.6460705000, 77.1391296000), new mapsjs.geo.Point(12.9615803000, 80.1895065000), new mapsjs.geo.Point(13.0937595000, 80.2885132000), new mapsjs.geo.Point(15.1451200000, 76.9281800000), new mapsjs.geo.Point(17.3446007000, 78.5453033000), new mapsjs.geo.Point(13.0025101000, 77.7528000000), new mapsjs.geo.Point(13.1086798000, 80.2422791000), new mapsjs.geo.Point(13.1303196000, 80.2533493000), new mapsjs.geo.Point(12.9653702000, 80.1684875000), new mapsjs.geo.Point(12.9587097000, 80.1407700000), new mapsjs.geo.Point(13.0008001000, 80.2039871000), new mapsjs.geo.Point(12.3089504000, 76.6532593000), new mapsjs.geo.Point(27.3247795000, 95.8282928000), new mapsjs.geo.Point(18.4998398000, 73.8034897000), new mapsjs.geo.Point(18.4856205000, 73.9457932000), new mapsjs.geo.Point(22.6707191000, 88.4537430000), new mapsjs.geo.Point(30.3286705000, 78.0028610000), new mapsjs.geo.Point(18.5080605000, 73.9182892000), new mapsjs.geo.Point(23.0536594000, 72.5469894000), new mapsjs.geo.Point(15.3463300000, 75.1480000000), new mapsjs.geo.Point(28.8892899000, 77.6718063000), new mapsjs.geo.Point(27.1899500000, 78.0011500000), new mapsjs.geo.Point(26.8220100000, 92.6853409000), new mapsjs.geo.Point(12.9928398000, 77.7098236000), new mapsjs.geo.Point(26.9106998000, 75.7858734000), new mapsjs.geo.Point(36.8367691000, -77.7738190000), new mapsjs.geo.Point(23.0057201000, 72.5960007000), new mapsjs.geo.Point(28.2338505000, 79.5245209000), new mapsjs.geo.Point(28.2338505000, 79.5245209000), new mapsjs.geo.Point(18.4606495000, 73.8163910000), new mapsjs.geo.Point(22.5000706000, 88.3116531000), new mapsjs.geo.Point(15.3463300000, 75.1480000000), new mapsjs.geo.Point(27.8864400000, 78.0742600000), new mapsjs.geo.Point(29.2026100000, 79.5266100000), new mapsjs.geo.Point(15.3463300000, 75.1480000000), new mapsjs.geo.Point(23.1977596000, 77.4142380000), new mapsjs.geo.Point(18.5914001000, 73.7620392000), new mapsjs.geo.Point(13.0597401000, 80.2013626000), new mapsjs.geo.Point(13.0509596000, 80.1639175000), new mapsjs.geo.Point(22.4908791000, 88.3280334000), new mapsjs.geo.Point(19.1621304000, 72.9896088000), new mapsjs.geo.Point(23.2547207000, 68.8385391000), new mapsjs.geo.Point(26.8903103000, 75.8097076000), new mapsjs.geo.Point(25.6218204000, 85.1152420000), new mapsjs.geo.Point(19.1555195000, 72.9981308000), new mapsjs.geo.Point(17.4450703000, 78.4516602000), new mapsjs.geo.Point(17.7181702000, 83.2466431000), new mapsjs.geo.Point(8.5340796000, 76.8979187000), new mapsjs.geo.Point(22.3080807000, 73.1572113000), new mapsjs.geo.Point(19.1106606000, 72.9254608000), new mapsjs.geo.Point(13.0650997000, 77.6403198000), new mapsjs.geo.Point(19.2172909000, 72.9876022000), new mapsjs.geo.Point(28.4128799000, 77.3318405000), new mapsjs.geo.Point(17.5044804000, 78.3839798000), new mapsjs.geo.Point(30.3814106000, -86.4000778000), new mapsjs.geo.Point(28.4750195000, 77.1321487000), new mapsjs.geo.Point(19.1886501000, 72.9969025000), new mapsjs.geo.Point(21.2472800000, 81.6412200000), new mapsjs.geo.Point(18.5650005000, 73.9200211000), new mapsjs.geo.Point(13.0291100000, 77.6821518000), new mapsjs.geo.Point(28.5008392000, 77.0315628000), new mapsjs.geo.Point(28.6719303000, 77.4600220000), new mapsjs.geo.Point(19.0506592000, 73.0756073000), new mapsjs.geo.Point(17.4685497000, 78.4561920000), new mapsjs.geo.Point(18.5020294000, 73.8989868000), new mapsjs.geo.Point(13.0593500000, 80.2373428000), new mapsjs.geo.Point(28.9951191000, 77.7057190000), new mapsjs.geo.Point(11.0793695000, 76.9495773000), new mapsjs.geo.Point(30.7075405000, 76.8414688000), new mapsjs.geo.Point(27.1997795000, 78.0176697000), new mapsjs.geo.Point(23.0623207000, 72.5431519000), new mapsjs.geo.Point(12.9459000000, 77.6267200000), new mapsjs.geo.Point(27.1844006000, 73.7371597000), new mapsjs.geo.Point(9.9310598000, 76.2954788000), new mapsjs.geo.Point(22.5095901000, 88.3696136000), new mapsjs.geo.Point(26.9009094000, 80.9357529000), new mapsjs.geo.Point(26.8900108000, 75.8247070000), new mapsjs.geo.Point(23.3672695000, 80.0345001000), new mapsjs.geo.Point(13.0327597000, 80.2591019000), new mapsjs.geo.Point(28.5366898000, 77.2122498000), new mapsjs.geo.Point(30.9182091000, 75.8216324000), new mapsjs.geo.Point(17.4587402000, 78.4431381000), new mapsjs.geo.Point(17.3912907000, 78.4985504000), new mapsjs.geo.Point(28.5824509000, 77.3130417000), new mapsjs.geo.Point(28.6719303000, 77.4600220000), new mapsjs.geo.Point(26.8198395000, 87.2703400000), new mapsjs.geo.Point(19.1694298000, 72.8528214000), new mapsjs.geo.Point(30.3095093000, 78.0489120000), new mapsjs.geo.Point(13.0749397000, 80.2250900000), new mapsjs.geo.Point(12.9149399000, 77.4816895000), new mapsjs.geo.Point(11.1018200000, 77.3488464000), new mapsjs.geo.Point(17.4637508000, 78.5511780000), new mapsjs.geo.Point(19.1415100000, 72.8245621000), new mapsjs.geo.Point(20.3039608000, 85.8051834000), new mapsjs.geo.Point(19.0636101000, 73.0050888000), new mapsjs.geo.Point(18.5432205000, 73.9086914000), new mapsjs.geo.Point(23.0667000000, 72.5555115000), new mapsjs.geo.Point(13.1303196000, 80.2533493000), new mapsjs.geo.Point(19.0463009000, 72.8783875000), new mapsjs.geo.Point(28.5869503000, 77.1960678000), new mapsjs.geo.Point(28.5393295000, 77.2877197000), new mapsjs.geo.Point(22.3386993000, 71.8222580000), new mapsjs.geo.Point(13.0980196000, 80.2004471000), new mapsjs.geo.Point(28.5755005000, 77.1573105000), new mapsjs.geo.Point(22.5095901000, 88.3696136000), new mapsjs.geo.Point(28.0348301000, 83.6025391000), new mapsjs.geo.Point(12.9590597000, 77.7181320000), new mapsjs.geo.Point(28.5137901000, 77.4062881000), new mapsjs.geo.Point(12.9229097000, 77.6201172000), new mapsjs.geo.Point(28.4212894000, 77.3249130000), new mapsjs.geo.Point(13.0005302000, 77.6233292000), new mapsjs.geo.Point(19.0897694000, 72.9085617000), new mapsjs.geo.Point(28.6035004000, 77.2386932000), new mapsjs.geo.Point(21.7700700000, 72.1458500000), new mapsjs.geo.Point(28.6921997000, 77.4539566000), new mapsjs.geo.Point(26.8901806000, 75.8196869000), new mapsjs.geo.Point(20.3360500000, 82.0265274000), new mapsjs.geo.Point(26.6956600000, 88.4411400000), new mapsjs.geo.Point(19.1274796000, 72.8298569000), new mapsjs.geo.Point(13.0305901000, 80.2077103000), new mapsjs.geo.Point(18.9151592000, 72.8262177000), new mapsjs.geo.Point(19.1792698000, 72.8166122000), new mapsjs.geo.Point(12.9063301000, 77.5875168000), new mapsjs.geo.Point(28.5674896000, 77.2735825000), new mapsjs.geo.Point(28.6781101000, 77.2813492000), new mapsjs.geo.Point(17.5049992000, 78.3832932000), new mapsjs.geo.Point(17.5324192000, 78.4044800000), new mapsjs.geo.Point(28.6940994000, 77.2176590000), new mapsjs.geo.Point(13.0578604000, 77.5973282000), new mapsjs.geo.Point(17.4910603000, 78.5712967000), new mapsjs.geo.Point(12.9219704000, 77.6449890000), new mapsjs.geo.Point(28.3935909000, 77.3152466000), new mapsjs.geo.Point(28.5739193000, 77.2273178000), new mapsjs.geo.Point(12.9731398000, 77.6094284000), new mapsjs.geo.Point(12.9624596000, 77.5077286000), new mapsjs.geo.Point(27.1764698000, 77.9153519000), new mapsjs.geo.Point(13.0230799000, 77.6898727000), new mapsjs.geo.Point(18.5914001000, 73.7620392000), new mapsjs.geo.Point(12.9660900000, 77.6082600000), new mapsjs.geo.Point(12.8653097000, 77.6587372000), new mapsjs.geo.Point(17.4501991000, 78.5393982000), new mapsjs.geo.Point(28.4763393000, 77.0479584000), new mapsjs.geo.Point(28.5232506000, 77.1681290000), new mapsjs.geo.Point(12.9467001000, 77.7068405000), new mapsjs.geo.Point(19.0994492000, 72.9997787000), new mapsjs.geo.Point(21.7295399000, 73.0121994000), new mapsjs.geo.Point(12.8989697000, 77.6244431000), new mapsjs.geo.Point(12.9991598000, 77.6092834000), new mapsjs.geo.Point(12.9090099000, 77.6040497000), new mapsjs.geo.Point(28.6267109000, 77.1478729000), new mapsjs.geo.Point(28.6712303000, 77.1778564000), new mapsjs.geo.Point(12.9196796000, 77.5108032000), new mapsjs.geo.Point(28.6280804000, 77.1981506000), new mapsjs.geo.Point(28.5568295000, 77.2871323000), new mapsjs.geo.Point(22.6067696000, 88.2740631000), new mapsjs.geo.Point(18.9162693000, 72.8300018000), new mapsjs.geo.Point(28.2422009000, 77.0706024000), new mapsjs.geo.Point(12.9799900000, 80.1876526000), new mapsjs.geo.Point(26.8871593000, 80.9266586000), new mapsjs.geo.Point(22.6067696000, 88.2740631000), new mapsjs.geo.Point(28.5961304000, 77.2948074000), new mapsjs.geo.Point(19.0165997000, 73.0926208000), new mapsjs.geo.Point(13.0787001000, 77.5384521000), new mapsjs.geo.Point(28.5372696000, 77.2649078000), new mapsjs.geo.Point(22.4765301000, 88.3823395000), new mapsjs.geo.Point(22.7162304000, 75.8651199000), new mapsjs.geo.Point(27.4844093000, 94.9231110000), new mapsjs.geo.Point(12.9226704000, 80.1416321000), new mapsjs.geo.Point(13.0552397000, 80.2233582000), new mapsjs.geo.Point(26.8150291000, 80.9036331000), new mapsjs.geo.Point(16.2952200000, 80.4426900000), new mapsjs.geo.Point(12.9644003000, 80.2115173000), new mapsjs.geo.Point(12.8982601000, 80.1100922000), new mapsjs.geo.Point(21.2472800000, 81.6412200000), new mapsjs.geo.Point(13.0976801000, 80.2325134000), new mapsjs.geo.Point(19.0861206000, 73.0254898000), new mapsjs.geo.Point(15.4092102000, 73.9921036000), new mapsjs.geo.Point(19.0243607000, 73.0437698000), new mapsjs.geo.Point(15.3463300000, 75.1480000000), new mapsjs.geo.Point(23.0802097000, 72.6183090000), new mapsjs.geo.Point(16.5226593000, 80.6767120000), new mapsjs.geo.Point(15.1958399000, 76.6763000000), new mapsjs.geo.Point(32.6879300000, 75.9791800000), new mapsjs.geo.Point(13.0519505000, 80.2214890000), new mapsjs.geo.Point(12.9942300000, 77.6878800000), new mapsjs.geo.Point(12.3089500000, 76.6532600000), new mapsjs.geo.Point(22.8229694000, 75.9159012000), new mapsjs.geo.Point(30.3095100000, 78.0489100000), new mapsjs.geo.Point(28.1802807000, 77.6950302000), new mapsjs.geo.Point(23.0737896000, 72.5319672000), new mapsjs.geo.Point(28.5064700000, 77.2739400000), new mapsjs.geo.Point(22.2971700000, 70.8015400000), new mapsjs.geo.Point(25.5986900000, 85.1640701000), new mapsjs.geo.Point(16.2032900000, 77.3448200000), new mapsjs.geo.Point(12.9736900000, 77.6231995000), new mapsjs.geo.Point(61.5328789000, 5.1949801000), new mapsjs.geo.Point(23.4199700000, 76.2807700000), new mapsjs.geo.Point(18.4871502000, 73.9369965000), new mapsjs.geo.Point(37.9105200000, -82.5505800000), new mapsjs.geo.Point(22.8382700000, 69.7285300000), new mapsjs.geo.Point(22.2971700000, 70.8015400000), new mapsjs.geo.Point(28.5876598000, 77.2518997000), new mapsjs.geo.Point(26.4635296000, 87.2885666000), new mapsjs.geo.Point(28.6312695000, 77.3716125000), new mapsjs.geo.Point(19.6885700000, 85.1776500000)];

            //var a = new Array(coordinates.length);
            //for (var i = 0; i < a.length; i++) {
            //    a[i] = new H.map.Marker(coordinates[i],
            //    {
            //        icon: colors[Math.floor((Math.random() * 4))]
            //    });
            //}
            //map.addObjects(a);

            //$(document).ready(function () {
            //    PlotMarkers(str1);
            //});


            function PlotMarkers() {
                var str1 = $('#hdnloc').attr('value');
                var coordinates = str1.split(':');
                var a = new Array(coordinates.length);
                for (var i = 0; i < a.length; i++) {
                    var v = coordinates[i];
                    var loc = v.split(',');
                    var x = parseFloat(loc[0]);
                    var y = parseFloat(loc[1]);

                    if (v == '' || v == undefined)
                    { continue; }

                    if (v[0] == '')
                    { continue; }
                    a[i] = new H.map.Marker(new mapsjs.geo.Point(x, y),
                    {
                        icon: colors[Math.floor((Math.random() * 4))]
                    });
                }
                map.addObjects(a);
                return false;
            }


    </script>

</asp:Content>

