function GmapClass1() { }

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


GmapClass1.prototype.SearchSite = function (Sender, body) {

    var txt = $(Sender).val();
    
    $(body).html('');

        if (txt == '')
        {
            $(body).html('');
        }
        else if (txt.length<3)
        {
            return false;
        }
         else
        {
        for (i = 0; i < myMarkersArray.length - 1; i++) {
            if (myMarkersArray[i].SiteName.toUpperCase().match(txt.toUpperCase())) {
                var row = '<tr id="' + myMarkersArray[i].primaryKey + '" onclick="Gmap1.FocusSite(this);"><td>' + myMarkersArray[i].SiteName + '<td/><tr/>';
                $(body).append(row);
            }
        }
        }
        return false;
}

GmapClass1.prototype.FocusSite = function (Sender) {
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

GmapClass1.prototype.GetMarkers = function (ddlsite, ddlVechical) {
    var str = '';
    var CHK = document.getElementById(ddlsite);
    var checkbox = CHK.getElementsByTagName("input");
    //var label = CHK.getElementsByTagName("label");
    for (var i = 0; i < checkbox.length; i++) {
        if (checkbox[i].checked) {

            str += checkbox[i].value + ',';
            
        }
    }

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
        url: "GMapHome1.aspx/GetMarkerListCSV",
        data: '{IDs: "' + str + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Gmap1.DrawMarkers,
        failure: function (response) {
            alert(response.d);
        }
    });

}

GmapClass1.prototype.DrawMarkers = function (result) {

    var str = result.d;
    //var v = str.substring(1, str.lastIndexOf(","));
    //var lastChar = str.slice(-1);
    //if (lastChar == ',') {
    //    str = str.slice(0, -1);
    //}
    //str = str + ']';

    var obj = csv2array(str, '^', '|');
    var l = obj.length;
    var JLineCoOrdinate = '';
    var icon = '';
    
    var cluster = $('#hdnCluster').attr('value')
    var clusters = $('#hdnCluster').attr('value').split(',');

    var contains = false;
    Gmap1.DrawMap(obj);

    $.each(obj, function (i, value) {
           
        if (cluster != 'SU') {
                for (i = 0; i <= clusters.length - 1; i++) {
                    if (clusters[i] == value.Cluster) {
                        contains = true;
                        break;
                    }
                    else {
                        contains = false;
                    }
                }

        }
        else if (cluster.toUpperCase() == 'SU') {
            //for (i = 0; i <= clusters.length - 1; i++) {
                contains = true;
                //break;
            //}
            }


            if (value.Site.toUpperCase() == 'HUB') {

                this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                this['Lat'] = value.Lat;
                this['Long'] = value.Long;
                this['img'] = "http://www.myndsaas.com/images/darkyellow.png";
                this['Group'] = value.Group;
                this['SiteID'] = value.SiteID;
                this['Site_Name'] = value.Site_Name;
                this['Cluster'] = value.Cluster;
                this['IsCluster']=contains ? true : false;

                icon = "http://www.myndsaas.com/images/darkyellow.png";
                JLineCoOrdinate = JLineCoOrdinate + "['SiteID : " + value.SiteID + "<br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + " <br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "'," + value.Lat + "," + value.Long + ",'" + icon + "','" + value.Group + "','" + value.SiteID + "','" + value.Site_Name + "'],";
            }
            else if (value.Site.toUpperCase() == 'BSC') {

                this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                this['Lat'] = value.Lat;
                this['Long'] = value.Long;
                this['img'] = "http://www.myndsaas.com/images/lightblue.png";
                this['Group'] = value.Group;
                this['SiteID'] = value.SiteID;
                this['Site_Name'] = value.Site_Name;
                this['Cluster'] = value.Cluster;
                this['IsCluster']=contains ? true : false;

                icon = "http://www.myndsaas.com/images/lightblue.png";
                JLineCoOrdinate = JLineCoOrdinate + "['SiteID : " + value.SiteID + "<br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + " <br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "'," + value.Lat + "," + value.Long + ",'" + icon + "','" + value.Group + "','" + value.SiteID + "','" + value.Site_Name + "'],";
            }
            else if (value.Site.toUpperCase() == 'STRATEGIC') {

                this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                this['Lat'] = value.Lat;
                this['Long'] = value.Long;
                this['img'] = "http://www.myndsaas.com/images/blue.png";
                this['Group'] = value.Group;
                this['SiteID'] = value.SiteID;
                this['Site_Name'] = value.Site_Name;
                this['Cluster'] = value.Cluster;
                this['IsCluster']=contains ? true : false;

                icon = "http://www.myndsaas.com/images/blue.png";
                JLineCoOrdinate = JLineCoOrdinate + "['SiteID : " + value.SiteID + "<br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + " <br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "'," + value.Lat + "," + value.Long + ",'" + icon + "','" + value.Group + "','" + value.SiteID + "','" + value.Site_Name + "'],";
            }
            else if (value.Site.toUpperCase() == 'NON STRATEGIC') {

                this['Information'] = "<span style='font-weight:bold;'> SiteID : " + value.SiteID + "</span><br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + "<br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "";
                this['Lat'] = value.Lat;
                this['Long'] = value.Long;
                this['img'] = "http://www.myndsaas.com/images/darkk.png";
                this['Group'] = value.Group;
                this['SiteID'] = value.SiteID;
                this['Site_Name'] = value.Site_Name;
                this['Cluster'] = value.Cluster;
                this['IsCluster']=contains ? true : false;

                icon = "http://www.myndsaas.com/images/darkk.png";
                JLineCoOrdinate = JLineCoOrdinate + "['SiteID : " + value.SiteID + "<br>Site : " + value.Site + "<br>Site Name : " + value.Site_Name + "<br>OandM Head: " + value.OandM_Head + " <br> Maintenance Head : " + value.Maintenance_Head + " <br>Opex Manager : " + value.Opex_Manager + "<br>Security Manager : " + value.Security_Manager + "<br>Zonal Head : " + value.Zonal_Head + "<br>Cluster Manager :" + value.Cluster_Manager + "<br>Supervisor : " + value.Supervisor + "<br>Technician : " + value.Technician + " <br>No of OPCOs : " + value.No_of_OPCOs + " <br>Anchor OPCO : " + value.Anchor_OPCO + "'," + value.Lat + "," + value.Long + ",'" + icon + "','" + value.Group + "','" + value.SiteID + "','" + value.Site_Name + "'],";
            }
            else
            {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1
                var yyyy = today.getFullYear();
                mm = mm < 10 ? '0' + mm : mm;
                dd = dd < 10 ? '0' + dd : dd;
                var dateVal  = mm + '/' + dd + '/' + yyyy;

                var val = (value.OandM_Head / 60).toFixed(2);
                var ar = val.split('.');
                var hrs=ar[0];
                var mnts = ar[1];
                if (mnts > 59) {
                    mnts = '0' + mnts;
                    mnts = mnts.substring(0, 2);
                }
                
                

                var dipTime=hrs+':'+mnts;

                //alert(dateVal+'-------'+value.SiteName);

                if (value.OandM_Head > 10 && value.OandM_Head <= 600 && value.SiteName==dateVal) {

                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + " <br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + "(HH:MM) <br>Last Record Time : " + value.SiteName + " <br> Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        if (value.Group == 554655)
                        {
                            this['img'] = 'images/dfgray.png';
                        }
                        else if (value.Group == 583948) {
                            this['img'] = 'images/truck_gray.png';
                        }
                        else if (value.Group == 554654) {
                            this['img'] = 'images/mob_gray.png';
                        }
                        else if (value.Group == 554657) {
                            this['img'] = 'images/sc_gray.png';
                        }
                        else {
                            this['img'] = "http://www.myndsaas.com/images/car4.png";
                        }
                        this['Group'] = value.Group;
                        // this['Cluster'] = value.Cluster;
                        this['IsCluster'] = true;

                        icon = "http://www.myndsaas.com/images/car4.png";
                        JLineCoOrdinate = JLineCoOrdinate + "['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                    }
                else if (value.OandM_Head >= 0 && value.OandM_Head <= 10 && value.SiteName == dateVal) {

                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span><br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + " (HH:MM) <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        if (value.Group == 554655) {
                            this['img'] = 'images/dfgreen.png';
                        }
                        else if (value.Group == 583948) {
                            this['img'] = 'images/truck_green.png';
                        }
                        else if (value.Group == 554654) {
                            this['img'] = 'images/mob_green.png';
                        }
                        else if (value.Group == 554657) {
                            this['img'] = 'images/sc_green.png';
                        }
                        else {
                            this['img'] = "http://www.myndsaas.com/images/car2.png";
                        }
                        
                        this['Group'] = value.Group;
                        //this['Cluster'] = value.Cluster;
                        this['IsCluster'] = true;

                        icon = "http://www.myndsaas.com/images/car2.png";
                        JLineCoOrdinate = JLineCoOrdinate + "['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                    }
                else if ((value.OandM_Head > 600 && value.OandM_Head <= 1440) || (value.SiteName == dateVal && value.OandM_Head > 1440)) {

                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + " (HH:MM) <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        if (value.Group == 554655) {
                            this['img'] = 'images/dfpink.png';
                        }
                        else if (value.Group == 583948) {
                            this['img'] = 'images/truck_pink.png';
                        }
                        else if (value.Group == 554654) {
                            this['img'] = 'images/mob_pink.png';
                        }
                        else if (value.Group == 554657) {
                            this['img'] = 'images/sc_pink.png';
                        }
                        else {
                            this['img'] = "http://www.myndsaas.com/images/car5.png";
                        }
                       
                        this['Group'] = value.Group;
                        //this['Cluster'] = value.Cluster;
                        this['IsCluster'] = true;


                        icon = "http://www.myndsaas.com/images/car5.png";
                        JLineCoOrdinate = JLineCoOrdinate + "['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                    }
                    else {

                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + " (HH:MM) <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                        this['Lat'] = value.Lat;
                        this['Long'] = value.Long;
                        if (value.Group == 554655) {
                            this['img'] = 'images/dfred.png';
                        }
                        else if (value.Group == 583948) {
                            this['img'] = 'images/truck_red.png';
                        }
                        else if (value.Group == 554654) {
                            this['img'] = 'images/mob_red.png';
                        }
                        else if (value.Group == 554657) {
                            this['img'] = 'images/sc_red.png';
                        }
                        else {
                            this['img'] = "http://www.myndsaas.com/images/car1.png";
                        }
                        this['Group'] = value.Group;
                        //this['Cluster'] = value.Cluster;
                        this['IsCluster'] = true;

                        icon = "http://www.myndsaas.com/images/car1.png";
                        JLineCoOrdinate = JLineCoOrdinate + " ['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                    }

        }
        if (i == 10)
        {
            
       }
    });

    
    //Gmap.DrawMap(obj);
}



var map;
GmapClass1.prototype.DrawMap = function (str) {
   
    var cluster = $('#hdnCluster').attr('value')
    var clusters = $('#hdnCluster').attr('value').split(',');
    var contains = false;


    var locations = str
    map = new google.maps.Map(document.getElementById('map'), { zoom: 7, center: new google.maps.LatLng(19.8761653, 75.3433139), mapTypeId: google.maps.MapTypeId.ROADMAP });
    var infowindow = new google.maps.InfoWindow(); //var lineCoordinates = [];
    
    for (i = 0; i < locations.length - 1; i++) {
       
        if (cluster != 'SU') {
            for (i = 0; i <= clusters.length - 1; i++) {
                if (clusters[i] == locations[i][7]) {
                    contains = true;
                    break;
                }
                else {
                    contains = false;
                }
            }

        }
        else if (cluster.toUpperCase() == 'SU') {
            contains = true;
        }

        var Siteicon = '';

        if (locations[i][3].toUpperCase() == 'HUB') {
            Siteicon = "http://www.myndsaas.com/images/darkyellow.png";
        }
        else if (locations[i][3].toUpperCase() == 'BSC') {
            Siteicon = "http://www.myndsaas.com/images/lightblue.png";
        }
        else if (locations[i][3].toUpperCase() == 'STRATEGIC') {
            Siteicon = "http://www.myndsaas.com/images/blue.png";
        }
        else if (locations[i][3].toUpperCase() == 'NON STRATEGIC') {
            Siteicon = "http://www.myndsaas.com/images/darkk.png";
        }



        
        marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][4], locations[i][5]), map: map, icon: Siteicon, group: Siteicon, size: new google.maps.Size(1, 1) });
        marker.primaryKey = locations[i][1];
        marker.SiteName = locations[i][2];
        marker.Tid = locations[i][0];
        myMarkersArray.push(marker);
        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                if (InfoWindowVar != null) {
                    InfoWindowVar.close();
                    infowindow.setContent('Testing...');
                    infowindow.open(map, marker);
                    InfoWindowVar = infowindow;
                }
                else {
                    infowindow.setContent('Testing...');
                    infowindow.open(map, marker);
                    InfoWindowVar = infowindow;
                }
                
            }
        })(marker, i));
    }
   

    
    var refTime = $('#hdnRefTime').attr('value');

    setInterval(Gmap1.GetNewMarker, refTime);

}



GmapClass1.prototype.GetNewMarker = function ()
{
    var str = document.getElementById("hdndata").value;
    //url: "GMapHome.aspx/RefreshVechicals",
    
    $.ajax({
        type: "POST",
        url: "GMapHome.aspx/RefreshVechicals",
        data: '{IDs: "' + str + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Gmap1.GetNewMarkerSucceed,
        failure: function (response) {
            alert(response.d);
        }
    });
}

GmapClass1.prototype.GetNewMarkerSucceed = function (result) {

  

    var str = result.d;
    var obj = JSON.parse(str);
   
 

    var JLineCoOrdinate = '';
    var icon = '';

    $.each(obj, function (i, value) {
      
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1
        var yyyy = today.getFullYear();
        mm = mm < 10 ? '0' + mm : mm;
        dd = dd < 10 ? '0' + dd : dd;
        var dateVal = mm + '/' + dd + '/' + yyyy;

        var val = (value.OandM_Head / 60).toFixed(2);
        var ar = val.split('.');
        var hrs = ar[0];
        var mnts = ar[1]
        if (mnts > 59) {
            mnts = '0' + mnts;
            mnts = mnts.substring(0, 2);
        }

        var dipTime = hrs + ':' + mnts;

        //if (value.SiteID == 356307048927569)
        //{
        //    alert(value.OandM_Head+','+dipTime);
        //}

        

            if (value.Site != '' || value.info != '') {
                if (value.OandM_Head > 10 && value.OandM_Head <= 600 && value.SiteName == dateVal) {
                    if (value.Group == 554655)
                    {
                        icon = 'images/dfgray.png';
                    }
                    else if (value.Group == 583948) {
                        icon = 'images/truck_gray.png';
                    }
                    else if (value.Group == 554654) {
                        icon = 'images/mob_gray.png';
                    }
                    else if (value.Group == 554657) {
                        icon = 'images/sc_gray.png';
                    }
                    else {
                        icon = "http://www.myndsaas.com/images/car4.png";
                    }
                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + " <br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + "(HH:MM) <br>Last Record Time : " + value.SiteName + " <br> Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                    this['img'] = icon;
                    this['Group'] = value.Group;
                    this['IsCluster'] = true;

                    var ele = $("option[value='" + value.SiteID + "']");
                    ele.removeAttr('style');
                    ele.attr('style', 'color:black');
                   

                    //JLineCoOrdinate = JLineCoOrdinate + " ['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time :" + dipTime + " (HH:MM)<br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                }
                else if (value.OandM_Head >= 0 && value.OandM_Head <= 10 && value.SiteName == dateVal) {
                    if (value.Group == 554655)
                    {
                        icon = 'images/dfgreen.png';
                    }
                    else if (value.Group == 583948) {
                        icon = 'images/truck_green.png';
                    }
                    
                    else if (value.Group == 554654) {
                        icon = 'images/mob_green.png';
                    }
                    else if (value.Group == 554657) {
                        icon = 'images/sc_green.png';
                    }
                    else {
                        icon = "http://www.myndsaas.com/images/car2.png";
                    }
                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + " <br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + "(HH:MM) <br>Last Record Time : " + value.SiteName + " <br> Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                    this['img'] = icon;
                    this['Group'] = value.Group;
                    this['IsCluster'] = true;

                    var ele = $("option[value='" + value.SiteID + "']");
                    ele.removeAttr('style');
                    ele.attr('style', 'color:green');
                    
                    //JLineCoOrdinate = JLineCoOrdinate + " ['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + " (HH:MM) <br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                }
                else if ((value.OandM_Head > 600 && value.OandM_Head <= 1440) || (value.SiteName == dateVal && value.OandM_Head > 1440)) {

                    if (value.Group == 554655)
                    {
                        icon = 'images/dfpink.png';
                    }
                    else if (value.Group == 583948) {
                        icon = 'images/truck_pink.png';
                    }
                    else if (value.Group == 554654) {
                        icon = 'images/mob_pink.png';
                    }
                    else if (value.Group == 554657) {
                        icon = 'images/sc_pink.png';
                    }
                    else {
                        icon = "http://www.myndsaas.com/images/car5.png";
                    }
                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + " <br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + "(HH:MM) <br>Last Record Time : " + value.SiteName + " <br> Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                    this['img'] = icon;
                    this['Group'] = value.Group;
                    this['IsCluster'] = true;

                    var ele = $("option[value='" + value.SiteID + "']");
                    ele.removeAttr('style');
                    ele.attr('style', 'color:rgb(184, 106, 132)');
                    
                    //JLineCoOrdinate = JLineCoOrdinate + " ['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + " (HH:MM)<br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                }
                else {
                    if (value.Group == 554655)
                    {
                        icon = 'images/dfred.png';
                    }
                    else if (value.Group == 583948) {
                        icon = 'images/truck_red.png';
                    }
                    else if (value.Group == 554654) {
                        icon = 'images/mob_red.png';
                    }
                    else if (value.Group == 554657) {
                        icon = 'images/sc_red.png';
                    }
                    else {
                        icon = "http://www.myndsaas.com/images/car1.png";
                    }
                    this['Information'] = "<span style='font-weight:bold;'>IMEINO : " + value.SiteID + "</span> <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + " <br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + "(HH:MM) <br>Last Record Time : " + value.SiteName + " <br> Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + "";
                    this['img'] = icon;
                    this['Group'] = value.Group;
                    this['IsCluster'] = true;

                    var ele = $("option[value='" + value.SiteID + "']");
                    ele.removeAttr('style');
                    ele.attr('style', 'color:red');
                    
                    //JLineCoOrdinate = JLineCoOrdinate + " ['IMEINO : " + value.SiteID + " <br>Vehicle Name : " + value.Site_Name + " <br>Vehicle No : " + value.Address + "<br>Speed : " + value.Site_Address + " Km/h <br>Ideal Time : " + dipTime + " (HH:MM)<br>Last Record Time : " + value.SiteName + " <br>Lattitude : " + value.Lat + "<br>Longitude : " + value.Long + " '," + value.Lat + "," + value.Long + ",'" + icon + "'," + "'" + value.Group + "'," + value.SiteID + "],";
                }
            }
        

    });



   
   // Gmap1.RefreshVechical(obj);
}

GmapClass1.prototype.RefreshVechical = function (str) {

    //var locations = eval(str);
    var locations = str;

    //var latlng = new google.maps.LatLng(locations[i][1], locations[i][2]);

   

    for (i = 0; i < myMarkersArray.length - 1; i++) {

        for (j = 0; j < locations.length - 1; j++) {

            if ((myMarkersArray[i].primaryKey == locations[j]["SiteID"])) {

               // var latlng = new google.maps.LatLng(-24.397, 140.644);
                var latlng = new google.maps.LatLng(locations[j]["Lat"], locations[j]["Long"]);

                myMarkersArray[i].setPosition(latlng);



                //myMarkersArray[i].setIcon('http://www.myndsaas.com/images/car1.png');
                myMarkersArray[i].setIcon(locations[j]["img"]);



                var infowindow = new google.maps.InfoWindow({
                    content: locations[j]["Information"]
                });



                var marker = myMarkersArray[i];


                marker.infowindow = new google.maps.InfoWindow({
                    content: locations[j]["Information"]
                });
            
                marker.html = locations[j]["Information"];
                
                google.maps.event.clearListeners(marker, 'click');

                //infowindow.set(marker, null);

                google.maps.event.addListener(marker, 'click', (function (marker, i) {
                    return function () {
                        if (InfoWindowVar != null) {
                            InfoWindowVar.close();
                            infowindow.setContent(marker.html);
                            infowindow.open(map, this);
                            InfoWindowVar = infowindow;
                        }
                        else {
                            infowindow.setContent(marker.html);
                            infowindow.open(map, this);
                            InfoWindowVar = infowindow;
                        }
                    }

                })(marker, i));

            }
        }
    }

    //for (i = 0; i < RemovedMarkers.length - 1; i++) {

    //    for (j = 0; j < locations.length - 1; j++) {
    //        if ((RemovedMarkers[i].primaryKey == locations[j][5])) {
    //            var marker = myMarkersArray[i];
    //            marker.setPosition(new google.maps.LatLng(locations[j][1], locations[j][2]));
    //            google.maps.event.addListener(marker, 'click', (function (marker, i) {
    //                return function () {
    //                    infowindow.setContent(locations[j][0]);
                        
    //                    infowindow.open(map, marker);
    //                }
    //            })(marker, i));
    //        }
    //    }
    //}

}


GmapClass1.prototype.RefreshVechical1 = function (sender, sender1) {
    $('#' + sender1).removeClass('hide');
    $(sender).removeClass('show1');

    $('#' + sender1).addClass('show1');
    $(sender).addClass('hide');

    Gmap1.GetNewMarker();

    setTimeout(function () {
        $('#' + sender1).removeClass('show1');
        $(sender).removeClass('hide');
        $('#' + sender1).addClass('hide');
        $(sender).addClass('show1');
    }, 10000);
  
}


var Gmap1 = new GmapClass1();
