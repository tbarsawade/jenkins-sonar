﻿function TqmapClass() { }

var InfoWindowVar = null;


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

function FilterArr(element,index, arr) {
    return element.primaryKey >= 10;
}

TqmapClass.prototype.SearchSite = function (Sender, body,IsSiteId) {

    var txt = $(Sender).val();
    
    $(body).html('');

        if (txt == '')
        {
            $(body).html('');
        }
        if (IsSiteId && txt.length < 5)
        {
            return false;
        }
         if (txt.length < 3)
        {
            return false;
        }
         else
        {
            for (i = 0; i < myMarkersArray.length - 1; i++) {
                if (myMarkersArray[i].IsVehical=='0')
                {
                    continue;
                }
                if (IsSiteId) {
                    if (myMarkersArray[i].primaryKey.toUpperCase().match(txt.toUpperCase())) {
                        var row = '<tr id="' + myMarkersArray[i].primaryKey + '" onclick="Tqmap.FocusSite(this);"><td>' + myMarkersArray[i].primaryKey + '<td/><tr/>';
                        $(body).append(row);
                    }
                }
                else {
                    if (myMarkersArray[i].SiteName.toUpperCase().match(txt.toUpperCase())) {
                        var row = '<tr id="' + myMarkersArray[i].primaryKey + '" onclick="Tqmap.FocusSite(this);"><td>' + myMarkersArray[i].SiteName + '<td/><tr/>';
                        $(body).append(row);
                    }
                }
            }

        }
        return false;
}

TqmapClass.prototype.FocusSite = function (Sender) {
    var key = $(Sender).attr('id');
    for (i = 0; i < myMarkersArray.length; i++) {
        if ((myMarkersArray[i].primaryKey == key)) {
            google.maps.event.trigger(myMarkersArray[i], 'click')
            map.setCenter(myMarkersArray[i].getPosition());
            myMarkersArray[i].setVisible(true);
            return false;
        }
    }
}

TqmapClass.prototype.GetMarkers = function (ddlVechical) {
    var str = '';
    //var CHK = document.getElementById(ddlsite);
    //var checkbox = CHK.getElementsByTagName("input");
   
    var CHK1 = document.getElementById(ddlVechical);
    var checkbox1 = CHK1.getElementsByTagName("input");
    for (var i = 0; i < checkbox1.length; i++) {
        if (checkbox1[i].checked) {
            str += checkbox1[i].value + ',';
        }
    }

    var data = {
        IDs : str
    };
   
    var hdn = document.getElementById("hdndata");
    hdn.value = str;

    $.ajax({
        type: "POST",
        url: "TqMapHome.aspx/GetMarkerList",
        data: '{IDs: "' + str + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Tqmap.DrawMarkers,
        failure: function (response) {
            alert(response.d);
        }
    });

}

TqmapClass.prototype.DrawMarkers = function (result) {

    var str = result.d;
    
    var obj = csv2array(str, '^', '|');
    var l = obj.length;
    var JLineCoOrdinate = '';
    var icon = '';
    
    var cluster = $('#hdnCluster').attr('value')
    var clusters = $('#hdnCluster').attr('value').split(',');

    var contains = false;
    Tqmap.DrawMap(obj);
   
}



var map;
TqmapClass.prototype.DrawMap = function (str) {
   
    var cluster = $('#hdnCluster').attr('value')
    var clusters = $('#hdnCluster').attr('value').split(',');
    var contains = false;


    var locations = str
    map = new google.maps.Map(document.getElementById('map'), { zoom: 6, center: new google.maps.LatLng(28.4401823, 77.1023073), mapTypeId: google.maps.MapTypeId.ROADMAP });
    var infowindow = new google.maps.InfoWindow(); //var lineCoordinates = [];
    
    for (i = 0; i < locations.length; i++) {
       
        if (cluster != 'SU' && cluster.toUpperCase() != 'CORPORATEUSER') {
            for (j = 0; j <= clusters.length - 1; j++) {
                if (clusters[j] == locations[i][7]) {
                    contains = true;
                    break;
                }
                else {
                    contains = false;
                }
            }

        }
        else if (cluster.toUpperCase() == 'SU' || cluster.toUpperCase() == 'CORPORATEUSER') {
            contains = true;
        }

        if (locations[i][8] == '0') {
            contains = true;
        }
        contains = true;

        if (!contains)
        { continue; }

        var Siteicon = '';



        if (locations[i][8] != '0') {
            Siteicon = "images/Greendot.png";
        }
        else {
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1
            var yyyy = today.getFullYear();
            mm = mm < 10 ? '0' + mm : mm;
            dd = dd < 10 ? '0' + dd : dd;
            var dateVal = mm + '/' + dd + '/' + yyyy;

            if(locations[i][3] > 10 && locations[i][3] <= 600 && locations[i][6] == dateVal) {

                if (locations[i][7] == 554655) {
                    Siteicon = 'images/dfgray.png';
                }
                else if (locations[i][7] == 583948) {
                    Siteicon = 'images/truck_gray.png';
                }
                else if (locations[i][7] == 554654) {
                    Siteicon = 'images/mob_gray.png';
                }
                else if (locations[i][7] == 554657) {
                    Siteicon = 'images/sc_gray.png';
                }
                else if (locations[i][7] == 0) {
                    Siteicon = 'images/walking.png';
                }
                else {
                    Siteicon = "http://www.myndsaas.com/images/car4.png";
                }

                
            }
            else if (locations[i][3] >= 0 && locations[i][3] <= 10 && locations[i][6] == dateVal) {

                if (locations[i][7] == 554655) {
                    Siteicon = 'images/dfgreen.png';
                }
                else if (locations[i][7] == 583948) {
                    Siteicon = 'images/truck_green.png';
                }
                else if (locations[i][7] == 554654) {
                    Siteicon = 'images/mob_green.png';
                }
                else if (locations[i][7] == 554657) {
                    Siteicon = 'images/sc_green.png';
                }
                else if (locations[i][7] == 0) {
                    Siteicon = 'images/walking.png';
                }
                else {
                    Siteicon = "http://www.myndsaas.com/images/car2.png";
                }


            }
            else if ((locations[i][3] > 600 && locations[i][3] <= 1440) || (locations[i][6] == dateVal && locations[i][3] > 1440)) {
                if (locations[i][7] == 554655) {
                    Siteicon = 'images/dfpink.png';
                }
                else if (locations[i][7] == 583948) {
                    Siteicon = 'images/truck_pink.png';
                }
                else if (locations[i][7] == 554654) {
                    Siteicon = 'images/mob_pink.png';
                }
                else if (locations[i][7] == 554657) {
                    Siteicon = 'images/sc_pink.png';
                }
                else if (locations[i][7] == 0) {
                    Siteicon = 'images/walking.png';
                }
                else {
                    Siteicon = "http://www.myndsaas.com/images/car2.png";
                }
            }
            else {
                if (locations[i][7] == 554655) {
                    Siteicon = 'images/dfred.png';
                }
                else if (locations[i][7] == 583948) {
                    Siteicon = 'images/truck_red.png';
                }
                else if (locations[i][7] == 554654) {
                    Siteicon = 'images/mob_red.png';
                }
                else if (locations[i][7] == 554657) {
                    Siteicon = 'images/sc_red.png';
                }
                else if (locations[i][7] == 0) {
                    Siteicon = 'images/walking.png';
                }
                else {

                    Siteicon = "http://www.myndsaas.com/images/car1.png";
                }
            }

        }
        var grp;
        var tid;
        var lat;
        var long;
        var pk;
        if (locations[i][8] == '0') {
            grp = locations[i][7];
            tid = locations[i][1];
            lat = locations[i][4];
            long = locations[i][5];
            pk = locations[i][1];
        }
        else {
            grp = locations[i][6];
            tid = locations[i][0];
            lat = locations[i][1];
            long = locations[i][2];
            pk = locations[i][3];
        }
        
        marker = new google.maps.Marker({ position: new google.maps.LatLng(lat, long), map: map, icon: Siteicon, group: grp, size: new google.maps.Size(1, 1) });
        marker.primaryKey = pk;//locations[i][1];
        marker.SiteName = locations[i][2];
        marker.Tid = tid;//locations[i][0];
        marker.IsVehical = locations[i][8];
        
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

                $.ajax({
                    type: "post",
                    url: "TqMapHome.aspx/GetMarkerInfo",
                    data: "{Id : " + marker.Tid + ", IsVehical : " + str + ", Ids:'" + hdn.value + "'}",
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
    }
   

    var refTime = $('#hdnRefTime').attr('value');

    setInterval(Tqmap.GetNewMarker, refTime);

}



TqmapClass.prototype.GetNewMarker = function ()
{
    var str = document.getElementById("hdndata").value;
    //url: "TqMapHome.aspx/RefreshVechicals",
    
    $.ajax({
        type: "POST",
        url: "TqMapHome.aspx/RefreshVechicals",
        data: '{IDs: "' + str + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Tqmap.GetNewMarkerSucceed,
        failure: function (response) {
            alert(response.d);
        }
    });
}

TqmapClass.prototype.GetNewMarkerSucceed = function (result) {

    var str = result.d;
     var obj = csv2array(str, '^', '|');
   
    var JLineCoOrdinate = '';
    var icon = '';

    Tqmap.RefreshVechical(obj);

    $('#' + sendr1).removeClass('show1');
    $(sendr).removeClass('hide');
    $('#' + sendr1).addClass('hide');
    $(sendr).addClass('show1');
}

TqmapClass.prototype.RefreshVechical = function (str) {

    var locations = str;
    var Siteicon;
    for (i = 0; i <= myMarkersArray.length - 1; i++) {

        for (j = 0; j <= locations.length - 1; j++) {

            if ((myMarkersArray[i].primaryKey == locations[j][1]))
            {

                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1
                var yyyy = today.getFullYear();
                mm = mm < 10 ? '0' + mm : mm;
                dd = dd < 10 ? '0' + dd : dd;
                var dateVal = mm + '/' + dd + '/' + yyyy;

                if (locations[j][3] > 10 && locations[j][3] <= 600 && locations[j][6] == dateVal) {

                    if (locations[j][7] == 554655) {
                        Siteicon = 'images/dfgray.png';
                    }
                    else if (locations[j][7] == 583948) {
                        Siteicon = 'images/truck_gray.png';
                    }
                    else if (locations[j][7] == 554654) {
                        Siteicon = 'images/mob_gray.png';
                    }
                    else if (locations[j][7] == 554657) {
                        Siteicon = 'images/sc_gray.png';
                    }
                    else if (locations[j][7] == 0) {
                        Siteicon = 'images/walking.png';
                    }
                    else {
                        Siteicon = "http://www.myndsaas.com/images/car4.png";
                    }


                }
                else if (locations[j][3] >= 0 && locations[j][3] <= 10 && locations[j][6] == dateVal) {

                    if (locations[j][7] == 554655) {
                        Siteicon = 'images/dfgreen.png';
                    }
                    else if (locations[j][7] == 583948) {
                        Siteicon = 'images/truck_green.png';
                    }
                    else if (locations[j][7] == 554654) {
                        Siteicon = 'images/mob_green.png';
                    }
                    else if (locations[j][7] == 554657) {
                        Siteicon = 'images/sc_green.png';
                    }
                    else if (locations[j][7] == 0) {
                        Siteicon = 'images/walking.png';
                    }
                    else {
                        Siteicon = "http://www.myndsaas.com/images/car2.png";
                    }


                }
                else if ((locations[j][3] > 600 && locations[j][3] <= 1440) || (locations[j][6] == dateVal && locations[j][3] > 1440)) {
                    if (locations[j][7] == 554655) {
                        Siteicon = 'images/dfpink.png';
                    }
                    else if (locations[j][7] == 583948) {
                        Siteicon = 'images/truck_pink.png';
                    }
                    else if (locations[j][7] == 554654) {
                        Siteicon = 'images/mob_pink.png';
                    }
                    else if (locations[j][7] == 554657) {
                        Siteicon = 'images/sc_pink.png';
                    }
                    else if (locations[j][7] == 0) {
                        Siteicon = 'images/walking.png';
                    }
                    else {
                        Siteicon = "http://www.myndsaas.com/images/car2.png";
                    }
                }
                else {
                    if (locations[j][7] == 554655) {
                        Siteicon = 'images/dfred.png';
                    }
                    else if (locations[j][7] == 583948) {
                        Siteicon = 'images/truck_red.png';
                    }
                    else if (locations[j][7] == 554654) {
                        Siteicon = 'images/mob_red.png';
                    }
                    else if (locations[j][7] == 554657) {
                        Siteicon = 'images/sc_red.png';
                    }
                    else if (locations[j][7] == 0) {
                        Siteicon = 'images/walking.png';
                    }
                    else {

                        Siteicon = "http://www.myndsaas.com/images/car1.png";
                    }
                }
                
                myMarkersArray[i].setIcon(Siteicon);
                position = new google.maps.LatLng(parseFloat(locations[j][4]), parseFloat(locations[j][5]));
                myMarkersArray[i].setPosition(position);

                $(sendr).removeClass('hide');
                $(sendr).addClass('show1');
                $('#img1').removeClass('show1');
                $('#img1').addClass('hide');
            }
            
        }
    }
    
    $(sendr).removeClass('hide');
    $(sendr).addClass('show1');
    $('#' + sendr1).removeClass('show1');
    $('#' + sendr1).addClass('hide');
    
}

var sendr;
var sendr1;
TqmapClass.prototype.RefreshVechical1 = function (sender, sender1) {
    sendr = sender;
    sender1 = sender1;
    $('#' + sender1).removeClass('hide');
    $(sender).removeClass('show1');

    $('#' + sender1).addClass('show1');
    $(sender).addClass('hide');

    Tqmap.GetNewMarker();

    //setTimeout(function () {
    //    $('#' + sender1).removeClass('show1');
    //    $(sender).removeClass('hide');
    //    $('#' + sender1).addClass('hide');
    //    $(sender).addClass('show1');
    //}, 10000);
  
}


var Tqmap = new TqmapClass();