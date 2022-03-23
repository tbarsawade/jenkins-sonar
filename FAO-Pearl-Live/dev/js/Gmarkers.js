function Gmarkers() { }


var InfoWindowVar = null;

Gmarkers.prototype.GetDistance = function (lat1, lon1, lat2, lon2) {

    var degreesToRadians = Math.PI / 180;
    var earthRadius = 6371; // approximation in kilometers assuming earth to be spherical

    // CONVERT LATITUDE AND LONGITUDE VALUES TO RADIANS
    var previousRadianLat = lat1 * degreesToRadians;
    var previousRadianLong = lon1 * degreesToRadians;
    var currentRadianLat = lat2 * degreesToRadians;
    var currentRadianLong = lon2 * degreesToRadians;

    // CALCULATE RADIAN DELTA BETWEEN THE TWO POSITIONS
    var latitudeRadianDelta = currentRadianLat - previousRadianLat;
    var longitudeRadianDelta = currentRadianLong - previousRadianLong;

    var expr1 = (Math.sin(latitudeRadianDelta / 2) * Math.sin(latitudeRadianDelta / 2)) +
                (Math.cos(previousRadianLat) * Math.cos(currentRadianLat) * Math.sin(longitudeRadianDelta / 2) * Math.sin(longitudeRadianDelta / 2));
    var expr2 = 2 * Math.atan2(Math.sqrt(expr1), Math.sqrt(1 - expr1));
    var distanceValue = earthRadius * expr2;

    return distanceValue * 1000; //Meters

}


Gmarkers.prototype.GetSiteList = function () {
  
    $.ajax({
        type: "POST",
        url: "GmapWindow.aspx/GetMarkerListJSON",
        data: '{ }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Markers.CachMarkers,
        failure: function (response) {
            alert(response.d);
        }
    });

}

function FilterArray(element)
{
    var l = lineCoordinates.lengthl-1;
    return element.Lat >= lineCoordinates[0].k + 0.01 && element.Long >= lineCoordinates[0].B + 0.01 && element.Lat <= lineCoordinates[l].k + 0.01 && element.Long <= lineCoordinates[l].B + 0.01;
}


Gmarkers.prototype.CachMarkers = function (result)
{
    //$('#divload').html('<img src="images/load.gif" / style="width:100%; height:50px;">');

    var str = result.d;
    var obj = csv2array(str, '^', '|');

    //var v = str.substring(1, str.lastIndexOf(","));
    //var lastChar = str.slice(-1);
    //if (lastChar == ',') {
    //    str = str.slice(0, -1);
    //}
    //str = str + ']';
    //var obj = JSON.parse(str);
    //var l = obj.length;
    
    var infowindow = new google.maps.InfoWindow();
    var icon;

    //obj = $.grep(obj, function (element, i) {
    //    var l = lineCoordinates.length-1;
    //    //return ((element.Lat >= lineCoordinates[0].k + 0.01 && element.Long >= lineCoordinates[0].B + 0.01) || (element.Lat <= lineCoordinates[0].k - 0.01 && element.Long <= lineCoordinates[0].B - 0.01))
    //    //&& ((element.Lat >= lineCoordinates[l].k + 0.01 && element.Long >= lineCoordinates[l].B + 0.01) || (element.Lat <= lineCoordinates[l].k - 0.01 && element.Long <= lineCoordinates[l].B - 0.01));
    //    return ((element.Lat >= lineCoordinates[0].k - 5 && element.Long >= lineCoordinates[0].B - 5) || (element.Lat <= lineCoordinates[0].k + 5 && element.Long <= lineCoordinates[0].B + 5))
    //       && ((element.Lat >= lineCoordinates[l].k - 5 && element.Long >= lineCoordinates[l].B - 5) || (element.Lat <= lineCoordinates[l].k + 5 && element.Long <= lineCoordinates[l].B + 5));
    //})
   
    var l1 = obj.length;

    for (i = 0; i < obj.length ; i++) {
        for (j = 0; j < lineCoordinates.length; j++) {
            var latLong = lineCoordinates[j];
            var Dist= Markers.GetDistance(obj[i][4], obj[i][5],  latLong.k,latLong.D);
            if (Dist <= 1000 && Dist >= 0)
            {
                if (obj[i][3].toUpperCase() == 'HUB') {
                    icon = "http://www.myndsaas.com/images/darkyellow.png";
                }
                else if (obj[i][3].toUpperCase() == 'BSC') {
                    icon = "http://www.myndsaas.com/images/lightblue.png";
                }
                else if (obj[i][3].toUpperCase() == 'STRATEGIC') {
                    icon = "http://www.myndsaas.com/images/blue.png";
                }
                else if (obj[i][3].toUpperCase() == 'NON STRATEGIC') {
                    icon = "http://www.myndsaas.com/images/darkk.png";
                }
                
                marker = new google.maps.Marker({ position: new google.maps.LatLng(obj[i][4], obj[i][5]), map: map, icon: icon, group: obj[i][6], size: new google.maps.Size(1, 1) });
                marker.primaryKey = obj[i][1];
                marker.SiteName = obj[i][2];
                marker.Tid = obj[i][0];
                marker.IsVehical = undefined;
                //marker.html = "<span style='font-weight:bold;'> SiteID : " + obj[i].SiteID + "</span><br>Site : " + obj[i].Site + "<br>Site Name : " + obj[i].Site_Name + "<br>OandM Head: " + obj[i].OandM_Head + " <br> Maintenance Head : " + obj[i].Maintenance_Head + " <br>Opex Manager : " + obj[i].Opex_Manager + "<br>Security Manager : " + obj[i].Security_Manager + "<br>Zonal Head : " + obj[i].Zonal_Head + "<br>Cluster Manager :" + obj[i].Cluster_Manager + "<br>Supervisor : " + obj[i].Supervisor + "<br>Technician : " + obj[i].Technician + "<br>No of OPCOs : " + obj[i].No_of_OPCOs + " <br>Anchor OPCO : " + obj[i].Anchor_OPCO + "<br>Distance : " + (Dist / 1000).toFixed(3) + " Km";

                myMarkersArray.push(marker);
                google.maps.event.addListener(marker, 'click', (function (marker, i) {

                    var strInfo = "Loading...";

                    return function () {
                        if (InfoWindowVar != null) {
                            InfoWindowVar.close();
                            infowindow.setContent(strInfo);
                            infowindow.open(map, marker);
                            InfoWindowVar = infowindow;
                        }
                        else {
                            infowindow.setContent(strInfo);
                            infowindow.open(map, marker);
                            InfoWindowVar = infowindow;
                        }
                        var str = marker.IsVehical == undefined ? '1' : '0';
                        var hdn = document.getElementById("hdndata");
                        var hdnval = '';
                        if (hdn !=null)
                        {
                            hdnval = hdn.value;
                        }

                        $.ajax({
                            type: "post",
                            url: "GMapHome.aspx/GetMarkerInfo",
                            data: "{Id : " + marker.Tid + ", IsVehical : " + str + ", Ids:'" + hdnval + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (result) {
                                strInfo = result.d;
                                if (InfoWindowVar != null) {
                                    InfoWindowVar.close();
                                    infowindow.setContent(strInfo);
                                    infowindow.open(map, marker);
                                    InfoWindowVar = infowindow;
                                }
                                else {
                                    infowindow.setContent(strInfo);
                                    infowindow.open(map, marker);
                                    InfoWindowVar = infowindow;
                                }
                            },
                            error: function (result) {
                                strInfo = result.d;
                            }
                        });
                    }

                })(marker, i));
                break;
            }
        }
        
    }
   // $('#divload').html('');
   
}

Gmarkers.prototype.initialize = function ()
{
    map = new google.maps.Map(document.getElementById('map'), { zoom: 12, center: new google.maps.LatLng(clat, clong), mapTypeId: google.maps.MapTypeId.ROADMAP });
    var infowindow = new google.maps.InfoWindow();
    // debugger;

    var ar = new Array();

    for (i = 0; i < locations.length; i++) {
        lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2]));
        if (i > 0) {
            ar.push(new google.maps.LatLng(locations[i - 1][1], locations[i - 1][2]));
            ar.push(new google.maps.LatLng(locations[i][1], locations[i][2]));
            var FrPath = new google.maps.Polyline({ path: ar, strokeColor: 'rgba(1,1,1,0.3)' });
            FrPath.setMap(map);
        }
    }

    //for (i = 0; i < locations.length-1; i++) {
    //    lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2]));
    //}
   // var FrPath = new google.maps.Polyline({ path: lineCoordinates, strokeColor: 'Black' });
   //FrPath.setMap(map);
    var c = 0;
    var icon;
    for (i = 0; i < locations.length; i++) {
       // icon = 'images/Greendot.png';
        icon = locations[i][3];
        //if (i == 0) {
        //    icon = 'images/car1.png';
        //}
        //else {
        //    var p1 = new google.maps.LatLng(locations[i - 1][1], locations[i - 1][2]);
        //    var p2 = new google.maps.LatLng(locations[i][1], locations[i][2]);
           
            
        //    var dir = Markers.bearing(p1, p2);
        //    //Markers.bearing(p1, p2);
        //    if (dir == 0) { icon = 'images/dir0.png'; }
        //    if (dir > 0 && dir <= 45) { icon = 'images/dir45.png'; }
        //    else if (dir > 45 && dir <= 90) { icon = 'images/dir90.png'; }
        //    else if (dir > 90 && dir <= 135) { icon = 'images/dir135.png'; }
        //    else if (dir > 135 && dir <= 180) { icon = 'images/dir180.png'; }
        //    else if (dir > 180 && dir <= 225) { icon = 'images/dir225.png'; }
        //    else if (dir > 225 && dir <= 270) { icon = 'images/dir270.png'; }
        //    else if (dir > 270 && dir < 360) { icon = 'images/dir315.png'; }
        //    else if(dir < 0 || dir >= 360 || dir == null)
        //    {
        //        alert("missing angle");
        //    }
        //}

        marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map, icon: icon, size: new google.maps.Size(1, 1) });
        c = c + 1;
        google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]); infowindow.open(map, marker); } })(marker, i));

        //$('#msg').html(c + '_' + locations.length);
    }
   // Markers.GetSites();

}

//function GetSites()
Gmarkers.prototype.GetSites = function () 
{
   if (eid == 54) {
        Markers.GetSiteList();
    }
}

Gmarkers.prototype.bearing = function (from, to) {

    var degreesToRadians = Math.PI / 180;
    var degreesPerRadian = 180.0 / Math.PI;
    var lat1 = from.k * degreesToRadians;
    var lon1 = from.B * degreesToRadians;
    var lat2 = to.k * degreesToRadians;
    var lon2 = to.B * degreesToRadians;
    var e = Math, ra = e.PI / 180;
    var deg = 180 / e.PI;
    var x = lat2 - lat1;
    var y = lon2 - lon1;
    var f = 0;

    if (x >= 0 && y >= 0) {
        y = y * ra; x = x * ra;
        f = 90 - e.atan(y / x) * deg;

    } else if (x >= 0 && y <= 0) {
        y = y * ra; x = x * ra;
        f = 90 + e.abs(e.atan(y / x) * deg);

    } else if (x <= 0 && y <= 0) {
        y = y * ra; x = x * ra;
        f = 270 - e.atan(y / x) * deg;

    } else if (x <= 0 && y >= 0) {
        y = y * ra; x = x * ra;
        f = 270 + e.abs(e.atan(y / x) * deg);
    }
    return f;

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

var Markers = new Gmarkers();