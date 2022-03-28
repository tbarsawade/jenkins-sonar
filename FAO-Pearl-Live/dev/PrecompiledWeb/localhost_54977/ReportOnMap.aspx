<%@ page language="VB" autoeventwireup="false" inherits="ReportOnMap, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBfaExFhABnh7I3-vTHJjA4TWqtYnMccKE&sensor=false"></script>
    <script type="text/javascript">
        $(document).ready(function () { MyndMap(); });
        function MyndMap() {
            $.ajax({
                type: "post",
                url: "ReportOnMap.aspx/ConvertDataTabletoString",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var d = data.d;
                   // alert(d);
                    bindMap(d);
                },
                error: function (data) { alert("Error"); }
            });
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scmge"  runat="server"></asp:ScriptManager>
    <%--<div>
        Latitude:<input type="text" id="txtLat" /><br /><br />
        Latitude:<input type="text" id="txtlong" /><br /><br />
        <asp:HiddenField ID="hdn" runat="server" />
        <input type="button" value="check" onclick="javascript: bindMap();" />
        <br />
        <br />--%>
         <div>
                        <asp:Label ID="Map" runat="server"></asp:Label>
                        <asp:HiddenField ID="hdntid" runat="server" />
                        
                    </div>
       <div id="googleMap" style="height: 500px; position: relative; background-color: rgb(229, 227, 223); overflow: hidden;">
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    initialize();
    google.maps.Polygon.prototype.Contains = function (point) {
        // ray casting alogrithm http://rosettacode.org/wiki/Ray-casting_algorithm
        var crossings = 0,
            path = this.getPath();

        // for each edge
        for (var i = 0; i < path.getLength() ; i++) {
            var a = path.getAt(i),
                j = i + 1;
            if (j >= path.getLength()) {
                j = 0;
            }
            var b = path.getAt(j);
            if (rayCrossesSegment(point, a, b)) {
                crossings++;
            }
        }

        // odd number of crossings?
        return (crossings % 2 == 1);

        function rayCrossesSegment(point, a, b) {
            var px = point.lng(),
                py = point.lat(),
                ax = a.lng(),
                ay = a.lat(),
                bx = b.lng(),
                by = b.lat();
            if (ay > by) {
                ax = b.lng();
                ay = b.lat();
                bx = a.lng();
                by = a.lat();
            }
            // alter longitude to cater for 180 degree crossings
            if (px < 0) { px += 360 };
            if (ax < 0) { ax += 360 };
            if (bx < 0) { bx += 360 };

            if (py == ay || py == by) py += 0.00000001;
            if ((py > by || py < ay) || (px > Math.max(ax, bx))) return false;
            if (px < Math.min(ax, bx)) return true;

            var red = (ax != bx) ? ((by - ay) / (bx - ax)) : Infinity;
            var blue = (ax != px) ? ((py - ay) / (px - ax)) : Infinity;
            return (blue >= red);

        }
    };
    var map;
    var boundaryPolygon;
    function initialize() {

        google.maps.Polygon.prototype.Contains = function (point) {
            // ray casting alogrithm http://rosettacode.org/wiki/Ray-casting_algorithm
            var crossings = 0,
            path = this.getPath();

            // for each edge
            for (var i = 0; i < path.getLength() ; i++) {
                var a = path.getAt(i),
                j = i + 1;
                if (j >= path.getLength()) {
                    j = 0;
                }
                var b = path.getAt(j);
                if (rayCrossesSegment(point, a, b)) {
                    crossings++;
                }
            }

            // odd number of crossings?
            return (crossings % 2 == 1);

            function rayCrossesSegment(point, a, b) {
                var px = point.lng(),
                py = point.lat(),
                ax = a.lng(),
                ay = a.lat(),
                bx = b.lng(),
                by = b.lat();
                if (ay > by) {
                    ax = b.lng();
                    ay = b.lat();
                    bx = a.lng();
                    by = a.lat();
                }
                if (py == ay || py == by) py += 0.00000001;
                if ((py > by || py < ay) || (px > Math.max(ax, bx))) return false;
                if (px < Math.min(ax, bx)) return true;

                var red = (ax != bx) ? ((by - ay) / (bx - ax)) : Infinity;
                var blue = (ax != px) ? ((py - ay) / (px - ax)) : Infinity;
                return (blue >= red);
            }
        };


        var mapProp = {
            center: new google.maps.LatLng(19.2297897, 72.8535233),
            zoom: 10,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
    }

    function bindMap(boundary) {
        //var boundary = '72.8489 19.1754,	72.8553 19.1759,	72.8568 19.1758,	72.8585 19.1753,	72.8588 19.1743,	72.8655 19.1757,	72.8719 19.1757,	72.874 19.1758,	72.8744 19.1726,	72.874 19.1721,	72.8738 19.1711,	72.8724 19.1698,	72.8723 19.1689,	72.8712 19.1683,	72.871 19.1677,	72.8702 19.1675,	72.8697 19.1672,	72.8684 19.1668,	72.8671 19.1659,	72.8668 19.166,	72.8663 19.1657,	72.8652 19.1637,	72.8651 19.1634,	72.8641 19.1627,	72.8637 19.1628,	72.8637 19.1629,	72.863 19.1627,	72.8625 19.1623,	72.8605 19.1622,	72.86 19.1621,	72.8594 19.1626,	72.8584 19.1601,	72.8578 19.161,	72.8567 19.1536,	72.8559 19.1448,	72.8549 19.1418,	72.8538 19.142,	72.8523 19.1431,	72.8501 19.144,	72.8502 19.1496,	72.8494 19.1646,	 400064,	72.8351 19.2007,	72.8353 19.2007,	72.837 19.201,	72.8397 19.2003,	72.842 19.1994,	72.8422 19.1993,	72.8424 19.1993,	72.8425 19.198,	72.8432 19.1981,	72.8433 19.1955,	72.8451 19.1956,	72.8452 19.1954,	72.8466 19.1955,	72.8471 19.1954,	72.8475 19.1944,	72.8492 19.1943,	72.8493 19.1944,	72.8496 19.1944,	72.8502 19.1942,	72.8486 19.1846,	72.849 19.174,	72.8454 19.1735,	72.8436 19.1736,	72.8431 19.1738,	72.8368 19.1738,	72.8297 19.1756,	72.8301 19.179,	72.8298 19.1796,	72.8299 19.1807,	72.8297 19.1816,	72.8295 19.1822,	72.8285 19.1834,	72.8283 19.185,	72.8287 19.1861,	72.8296 19.1876,	72.829 19.1925,	72.8297 19.1948,	72.831 19.1964,	72.8314 19.1979,	72.8321 19.1986,	72.8333 19.1989,	72.8344 19.1994,	 400065,	72.8889 19.1821,	72.8894 19.181,	72.8907 19.1743,	72.8993 19.171,	72.8993 19.1648,	72.9001 19.1578,	72.8959 19.1485,	72.8962 19.1474,	72.8966 19.1435,	72.8953 19.143,	72.8948 19.1429,	72.8943 19.1423,	72.8933 19.1423,	72.8939 19.1408,	72.8939 19.1399,	72.8893 19.1377,	72.8872 19.1341,	72.8891 19.132,	72.8884 19.131,	72.8868 19.1307,	72.8859 19.1298,	72.8846 19.1297,	72.8844 19.1294,	72.8793 19.1298,	72.8759 19.1306,	72.8748 19.1317,	72.8745 19.1345,	72.873 19.1372,	72.871 19.139,	72.8681 19.1398,	72.8642 19.1388,	72.8627 19.139,	72.8636 19.1404,	72.8634 19.1416,	72.8627 19.142,	72.8586 19.1419,	72.8582 19.1415,	72.858 19.1405,	72.8567 19.141,	72.8548 19.1406,	72.856 19.145,	72.8579 19.1609,	72.8587 19.1613,	72.86 19.162,	72.8606 19.1622,	72.8624 19.1623,	72.8637 19.1629,	72.8641 19.1628,	72.8651 19.1634,	72.8651 19.164,	72.8652 19.1643,	72.8661 19.1657,	72.867 19.166,	72.8685 19.1668,	72.8701 19.1675,	72.8709 19.1676,	72.8712 19.1682,	72.872 19.1687,	72.8723 19.1695,	72.8724 19.1698,	72.8736 19.1706,	72.8739 19.1709,	72.874 19.1719,	72.874 19.1721,	72.8741 19.1722,	72.8741 19.1758,	72.8814 19.1763,	 400066,	72.8764 19.2391,	72.874 19.225,	72.8772 19.2159,	72.8761 19.2145,	72.8718 19.2147,	72.8686 19.2145,	72.8636 19.2133,	72.8606 19.213,	72.8571 19.2126,	72.8539 19.2139,	72.8584 19.2376,	72.8588 19.2378,	72.8595 19.2392,	72.8603 19.2391,	72.8632 19.2372,	72.8635 19.2377,	72.8639 19.2397,	72.8645 19.2445,	72.8694 19.2408,	 400067,	72.854 19.2147,	72.8503 19.1959,	72.8502 19.1943,	72.8497 19.1944,	72.8492 19.1943,	72.8471 19.1945,	72.8471 19.1954,	72.8467 19.1955,	72.8451 19.1954,	72.8451 19.1957,	72.8433 19.1955,	72.8432 19.1981,	72.8425 19.198,	72.8424 19.1993,	72.8414 19.1997,	72.837 19.201,	72.8352 19.2007,	72.8336 19.2009,	72.8317 19.2006,	72.8307 19.2006,	72.8281 19.1998,	72.8271 19.2,	72.8236 19.202,	72.8251 19.2043,	72.8251 19.205,	72.8212 19.2068,	72.8185 19.2066,	72.8106 19.2078,	72.8102 19.2082,	72.81 19.2081,	72.8099 19.2083,	72.8101 19.2085,	72.8101 19.2087,	72.8098 19.2087,	72.8091 19.2081,	72.8088 19.2083,	72.8087 19.2084,	72.8087 19.2088,	72.80861 19.20903,	72.80826 19.20937,	72.80816 19.20956,	72.80818 19.2098,	72.80835 19.21058,	72.8083 19.2111,	72.8078 19.2116,	72.8069 19.2121,	72.8082 19.2133,	72.8092 19.2156,	72.8106 19.221,	72.8129 19.2239,	72.8135 19.2241,	72.8148 19.2242,	72.8165 19.2223,	72.8174 19.2219,	72.8186 19.2221,	72.8193 19.2224,	72.8208 19.2248,	72.8224 19.2255,	72.8268 19.2247,	72.829 19.2229,	72.8294 19.2227,	72.8303 19.2234,	72.8311 19.2236,	72.834 19.2237,	72.8344 19.2232,	72.8348 19.2223,	72.8347 19.2218,	72.8347 19.2204,	72.8348 19.22,	72.835 19.2198,	72.8357 19.2189,	72.8365 19.2183,	72.8368 19.2174,	72.8395 19.2176,	72.84 19.2175,	72.8404 19.2159,	72.8408 19.2157,	72.8412 19.2159,	72.8425 19.216,	72.8429 19.2159,	72.8436 19.2158,	72.8442 19.216,	72.8447 19.216,	72.8448 19.214,	72.8479 19.2141,	72.848 19.2147,	72.8483 19.2146,	72.8486 19.2148';
        // debugger;
        var ar = boundary.split("|");
        var arr = ar[1].split("#");
        var Str = "";
        var locations = eval("[" + ar[0] + "]");
        //var locations = [['Product Name: Dream Plan- FAV Option 100 percent<br>Advisor Code: 508283', 19.1754, 72.8489, 'images/human.png'], ];
        for (var i = 0; i < arr.length; i++) {

            if (arr[i].trim() != "") {
                drawPolygon(arr[i]);
                Str = arr[i];
            }
        }
        //for (i = 0; i < locations.length; i++) {
        //   marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map, icon: locations[i][3], size: new google.maps.Size(1, 1) });
        //   google.maps.event.addListener(marker, 'click', (function (marker, i) {
        //        return function () {
        //            infowindow.setContent(locations[i][0]);
        //            infowindow.open(map, marker);
        //        }
        //    })(marker, i));
        //}
        //var map = new google.maps.Map(document.getElementById('googleMap'), { zoom: 5, center: new google.maps.LatLng(19.8761653, 75.3433139), mapTypeId: google.maps.MapTypeId.ROADMAP });
        var infowindow = new google.maps.InfoWindow();
        //var lineCoordinates = [];
        //for (i = 0; i < locations.length; i++) {
        //    lineCoordinates.push(new google.maps.LatLng(0, 0));
        //}
       // var FrPath = new google.maps.Polyline({ path: lineCoordinates, strokeColor: 'None' }); FrPath.setMap(map);
        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map, icon: locations[i][3], size: new google.maps.Size(1, 1) });
            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }
        
    //google.maps.event.addDomListener(window, 'load', initialize);

    function drawPolygon(boundary) {
        //var boundary = '-97.370989999999992 46.992124, -97.370986 46.992287, -97.404366 46.992346, -97.40961 46.992432, -97.409600000000012 46.993206, -97.409765 46.993213999999995, -97.410456 46.993027999999995, -97.410618 46.992433999999996, -97.41815 46.992459, -97.396357 46.977869, -97.39667 46.977866999999996, -97.397527 46.978437, -97.397883 46.978272, -97.397886 46.977869, -97.409083999999993 46.977903, -97.408824 46.962755, -97.393744 46.962813, -97.393713999999989 46.962308, -97.393385 46.96223, -97.393039 46.96236, -97.393025 46.962813999999995, -97.387739 46.962816, -97.387783 46.972404999999995, -97.387827 46.972705999999995, -97.38839 46.97385, -97.38850699999999 46.974388, -97.388545999999991 46.977838999999996, -97.357395 46.977753, -97.358465 46.978674, -97.358299 46.978932, -97.357631 46.979014, -97.357932999999989 46.979532, -97.357866 46.979744, -97.35763399999999 46.979887, -97.356097999999989 46.980121999999994, -97.355798 46.980382999999996, -97.35579899999999 46.980875999999995, -97.356401999999989 46.981840999999996, -97.357171999999991 46.982386999999996, -97.35807299999999 46.982313, -97.358240999999992 46.982431, -97.358108 46.982808, -97.358108 46.983089, -97.35837699999999 46.983208999999995, -97.358944 46.983197, -97.359646 46.982938, -97.360737 46.983602999999995, -97.360731 46.984094999999996, -97.360159 46.984359999999995, -97.360120999999992 46.984784999999995, -97.360186 46.984942, -97.359913999999989 46.9853, -97.359338999999991 46.985768, -97.359367999999989 46.986126, -97.360064 46.986048, -97.360362 46.986134, -97.360756999999992 46.986557999999995, -97.360884 46.987162, -97.360879 46.98761, -97.360575 46.987832999999995, -97.359294999999989 46.98793, -97.358824 46.988082999999996, -97.358820999999992 46.988329, -97.359450999999993 46.988457, -97.359681999999992 46.988613, -97.35974 46.989374, -97.36 46.990001, -97.360295999999991 46.990379999999995, -97.360193 46.990671, -97.360188999999991 46.991006999999996, -97.360585 46.991273, -97.361215 46.99131, -97.361444999999989 46.99151, -97.361351 46.992263, -97.367206 46.992273, -97.367217 46.987584, -97.367831 46.988023, -97.368158999999991 46.988174, -97.369042999999991 46.988375999999995, -97.369925 46.988848999999995, -97.370021 46.989002, -97.369508 46.989149, -97.36962 46.990027999999995, -97.369512 46.990314, -97.369994999999989 46.990925999999995, -97.369884 46.991344, -97.370989999999992 46.992124';
        var boundarydata = new Array();
        var latlongs = boundary.split(",");
        for (var i = 0; i < latlongs.length; i++) {
            latlong = latlongs[i].trim().split(" ");
            boundarydata[i] = new google.maps.LatLng(latlong[1], latlong[0]);
        }

        boundaryPolygon = new google.maps.Polygon({
            path: boundarydata,
            strokeColor: "#0000FF",
            strokeOpacity: 0.9,
            strokeWeight: 1,
            fillColor: 'Red',
            fillOpacity: 0.2

        });
        map.setCenter(boundarydata[0]);
        boundaryPolygon.setMap(map);
    }

    //function initialize()
    //{
    //    var locations = ''
    //    var map = new google.maps.Map(document.getElementById('map'), { zoom:5, center: new google.maps.LatLng(19.8761653, 75.3433139), mapTypeId: google.maps.MapTypeId.ROADMAP });
    //    var infowindow = new google.maps.InfoWindow();
    //    var lineCoordinates = [];
    //    for (i = 0; i < locations.length; i++) {
    //     lineCoordinates.push(new google.maps.LatLng(0, 0));
    //    }
    //    var FrPath = new google.maps.Polyline({ path: lineCoordinates, strokeColor: 'None' }); FrPath.setMap(map);
    //    for (i = 0; i < locations.length; i++) {
    //        marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map, icon: locations[i][3], size: new google.maps.Size(1, 1) });
    //        google.maps.event.addListener(marker, 'click', (function (marker, i) {
    //            return function () {
    //                infowindow.setContent(locations[i][0]);
    //                infowindow.open(map, marker);
    //            }
    //        })(marker, i));
    //    }
    //} google.maps.event.addDomListener(window, 'load', initialize);
</script>