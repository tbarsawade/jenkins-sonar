<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="userdashboard, App_Web_fdh01zus" viewStateEncryptionMode="Always" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <%--<script type="text/javascript">
        $(document).ready(function () {
            $(".mycontent").addClass("contentsamita");
            $(".mycontent").removeClass("mycontent");
        });
        
    </script>--%>
       <link href="css/dashboardstyle.css" rel="stylesheet" />
     
    <link href="css/dashboardstyle.css" rel="stylesheet" />
         <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
     <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
   <style type="text/css">
      
        .k-grid-toolbar
        {
            text-align:right;
        }

        #mask {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.59);
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }

       .loader {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }
       
     
    </style>
    
    <style type="text/css">
         /* Always set the map height explicitly to define the size of the div
        * element that contains the map. */
         #map {
             height: 100%;
         }
    </style>
    
    <style>
         /* Always set the map height explicitly to define the size of the div
        * element that contains the map. */
         #map {
             height: 100%;
         }
         .section { height:244px;padding: 50px 0px;}
#sub-container { width:200px; height:auto; margin:auto; }
.sub-box {width:200px; height:40px; margin:auto; }
.sub-allcredit {margin:5px 0px;}
.strap {border-bottom:10px solid green;margin:3px 0px 0px;
}
.sub-icon { font-family:Arial, Helvetica, sans-serif; text-align:left; font-size:13px; color:#717171; float:left;}
.sub-number {width:40px; height:22px;background:green;float:right; text-align:center;color:#fff;}

    </style>

    <div id="wrapper">
        <div> </div>

      <div class="mainlayout_part1">
            <h2 class="headicon">Need To Act</h2>
            <div class="box">
                <div class="icon"><img src="images/act.png"></div>
                <div class="num" id="lblneedtoact">100</div>
                <div class="clear"></div>
                <h4>Need To Act</h4>
            </div>
            <div class="box" style="background:#60bcff;">
                <div class="icon"><img src="images/req.png"></div>
                <div class="num" id="lblMyRequest">50</div>
                <div class="clear"></div>
                <h4>My Request</h4>
            </div>
            <div class="box" style="background:#4a366d;">
                <div class="icon"><img src="images/hist.png"></div>
                <div class="num" id="lblHistory">200</div>
                <div class="clear"></div>
                <h4>History</h4>
            </div>
            <div class="box" style="background:#f47a53;">
                <div class="icon"><img src="images/due.png"></div>
                <div class="num" id="lblDocumentDue">3</div>
                <div class="clear"></div>
                <h4>Document Due</h4>
            </div>
            <div class="box" style="background:#13a08b;">
                <div class="icon"><img src="images/days.png"></div>
                <div class="num" id="lblDuein5">50</div>
                <div class="clear"></div>
                <h4>Due in 5Days</h4>
            </div>
            <div class="box" style="background:rgb(179, 30, 30);">
                <div class="icon"><img src="images/expired.png"></div>
                <div class="num" id="lblExpiredSLA">25</div>
                <div class="clear"></div>
                <h4>Expired SLA</h4>
            </div>
        </div>
      <%--  <div class="mainlayout_part1 layout_er"  id="chartwrapper" >
            <h2 class="headicon_comp">Company Wise</h2>
                       
                <div id="dvpiechart" style="width:50%;"></div>
          
        </div>--%>
        <div class="mainlayout_part1 layout_er" id="chartwrapper">
            <h2 class="headicon_comp">Company Wise</h2>
            <div class="gap">
            Performed<br />
            <div id="dvper" style="width: 100%;"></div>
            Performed after due date<br />
            <div id="dvpafterdue" style="width: 100%;"></div>
            In Process<br />
            <div id="dvinProcess" style="width: 100%;"></div>
            Delayed<br />
            <div id="dvidelayed" style="width: 100%;"></div>
            </div>
        </div>

        <div class="clear"></div>

        <div class="mainlayout_part1">
          <h2 class="headicon_act">Act Wise</h2>
            <div class="actwise_container" id="dvActwise"></div>
        </div>
        
        <div class="mainlayout_part1 layout_er">
          <h2 class="headicon_site">Map View</h2>
          <div class="actwise_container" style="height:400px;">
              <div id="map"></div>
          </div>  
        </div>
    </div>
    <div id="dvpopupgrid" style="display:none;background: center no-repeat url('kendu/content/shared/styles/world-map.png');text-align:center;"  >
           <div id="ldrdvGrid" class="loader"></div>
           <div id="dvgrid"></div>
         </div>

     <script type="text/javascript">
          $(document).ready(function () {
            

              $('.anchor').on("click", function () {
                  alert("anchorclicked");
              })
              BindHomeDet();
              BindPieChart();
              BindChart();

              //  Bindcompddl();
              // $("#fromdtpicker").datepicker();
              //{ dateFormat: 'dd/mm/yy' }
              //$("#fromdtpicker").kendoDatePicker({

              //    start: "year",


              //    depth: "year",


              //    format: "MMMM yyyy",

              //});
              //$('#fromdtpicker').data("kendoDatePicker").value(new Date(2016,06));
              //$("#todtpicker").kendoDatePicker({
              //    // defines the start view
              //    start: "year",
              //    // defines when the calendar should return date
              //    depth: "year",
              //    // display month and year in the input
              //    format: "MMMM yyyy"
              //});
              //$('#todtpicker').data("kendoDatePicker").value(new Date());



          });
        </script>
    <script type="text/javascript">
        function BindHomeDet() {
            var Sdatepicker = ''; // $("#fromdtpicker").data("kendoDatePicker");
            var Tdatepicker = '';//$("#todtpicker").data("kendoDatePicker");
            var SMonth = '';// kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = '';//kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = '';// kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = '';//  kendo.toString(Tdatepicker.value(), 'yyyy');
            // var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            // var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var str = '{"CompanyID":"' + 1118586 + '","Uid":"' + UID + '","SMonth":"' + 10 + '","SYear":"' + 2016 + '","TMonth":"' + 11 + '","TYear":"' + 2016 + '"}';
            var winwidth = $(window).width() - 100;
            $.ajax({
                type: "POST",
                url: "userdashboard.aspx/GetHomeDet",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    var Data = JSON.parse(res.d);
                    $("#lblneedtoact").html(Data[0]["Need_To_Act"]);
                    $("#lblMyRequest").html(Data[0]["My_Requests"]);
                    $("#lblHistory").html(Data[0]["History"]);
                    $("#lblDocumentDue").html(Data[0]["Due"]);
                    $("#lblDuein5").html(Data[0]["Duein5"]);
                    $("#lblExpiredSLA").html(Data[0]["Expired_SLA"]);
                },

                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrdv" + Compid).hide();
                },
                failure: function (response) {
                    alert("failure in chart bind");
                    $("#ldrdv" + Compid).hide();

                }
            });

        }

        function BindChart() {
         
            var str = '{"CompanyID":"' + 1118586 + '","SMonth":"' + 10 + '","SYear":"' + 2016 + '","TMonth":"' + 11 + '","TYear":"' + 2016 + '"}';
            var winwidth = $(window).width() - 100;
            var strid = "#dvActwise";
            $.ajax({
                type: "POST",
                url: "userdashboard.aspx/GetActwiseChart",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {

                    var Data = res.d;
                    var Series = Data.series1;
                    if (Series.length == 0) {

                    }
                    var Category = Data.categoryAxis;
                    var width = 200 + 100 * Category.length;
                    if (width < winwidth)
                    { width = winwidth; }
                    $(strid).empty();
                    if (Category.length == 0) {


                        if ($(strid).data("kendoChart") != undefined) {
                            $(strid).data("kendoChart").destroy();
                        }

                    }
                    else {

                        if ($(strid).data("kendoChart") != undefined) {
                            $(strid).data("kendoChart").destroy();
                            $(strid).empty();
                        }
                        $(strid).kendoChart({
                            title: {

                            },
                            chartArea: {
                               
                               height:400
                            },
                            legend: {
                                position: "bottom",
                                visible: true,
                            },
                            seriesDefaults: {
                                type: "column",
                                stack: true
                               
                            },
                            majorGridLines: {
                                visible: false,
                            },
                            series: Series,
                            categoryAxis: {
                                categories: Category,

                                labels: {
                                    rotation: -90,
                                    template: "#=value.split(':,')[0]#",
                                }
                            },
                          
                            valueAxis: {

                                labels: {
                                    format: "{0}"
                                },
                                line: {
                                    visible: false
                                }
                            },

                            tooltip: {
                                visible: true,
                                //format: "{0}%",
                                template: "#= series.name # Percentage: #=kendo.format('{0:n2}', (value/category.split(':,')[2]) *100 ) #% <br/> #= series.name # Count : #= value#  "
                            },
                            labels:{},
                            seriesClick: function seriesclick(e) {
                            
                                $("#ldrdvGrid").show();
                                $("#dvGrid").empty();
                                BindGrid(1118586, e.category, e.series.name);
                                $("#dvpopupgrid").kendoWindow({
                                    width: "900px",
                                    height: "700px",
                                    modal: true,
                                    title: "Document Details",
                                    visible: false
                                });
                                $("#dvpopupgrid").data("kendoWindow").center().open();
                            },
                        });
                     }
                },

                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrdv" + Compid).hide();
                },
                failure: function (response) {
                    alert("failure in chart bind");
                    $("#ldrdv" + Compid).hide();

                }
            });

        }



        function BindPieChart() {
            var str = '{"CompanyID":"' + 1118586 + '","SMonth":"' + 10 + '","SYear":"' + 2016 + '","TMonth":"' + 11 + '","TYear":"' + 2016 + '"}';
            var width = $("#chartwrapper").width() - 50;
            $.ajax({
                type: "POST",
                url: "userdashboard.aspx/GetPieChart",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    //dvper dvpafterdue dvinProcess dvidelayed
                    var data = res.d;
                    var PerProgress = $("#dvper").kendoProgressBar({
                        max: 100,
                        value: data[0].value,
                        text: '50%'
                    }).data("kendoProgressBar");

                    var PerafterDueProgress = $("#dvpafterdue").kendoProgressBar({
                        type: "value",
                        max: 100,
                        value: data[1].value,
                        text: '50%'
                    }).data("kendoProgressBar");

                    var InprocessProgress = $("#dvinProcess").kendoProgressBar({
                        type: "value",
                        max: 100,
                        value: data[2].value,
                        text: '50%'
                    }).data("kendoProgressBar");

                    var DelayedProgress = $("#dvidelayed").kendoProgressBar({
                        type: "value",
                        max: 100,
                        value: data[3].value,
                        text: '50%'
                    }).data("kendoProgressBar");
                    PerProgress.progressWrapper.css({
                        "background-color": "green",
                        "border-color": "green"
                    });
                    PerProgress.progressStatus.text(data[0].value + "%");

                    PerafterDueProgress.progressWrapper.css({
                        "background-color": "red",
                        "border-color": "red"
                    });

                    PerafterDueProgress.progressStatus.text(data[1].value + "%");

                    InprocessProgress.progressStatus.text(data[2].value + "%");

                    DelayedProgress.progressStatus.text(data[3].value + "%");

                    InprocessProgress.progressWrapper.css({
                        "background-color": "#FF9900",
                        "border-color": "#FF9900"
                    });

                    DelayedProgress.progressWrapper.css({
                        "background-color": "#124ba0",
                        "border-color": "#124ba0"
                    });


                },

                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrdv" + Compid).hide();
                },
                failure: function (response) {
                    alert("failure in chart bind");
                    $("#ldrdv" + Compid).hide();

                }
            });

        }




        function BindGrid(CompanyID, ActID, Status) {
            //var Sdatepicker = $("#fromdtpicker").data("kendoDatePicker");
            //var Tdatepicker = $("#todtpicker").data("kendoDatePicker");
            //var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            //var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            //var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            //var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
           // $("#ldrdvSite").hide();
            var actid1 = ActID.split(":,")[1];
            var str = '{"CompanyID":"' + CompanyID + '","ActID":"' + actid1 + '","Status":"' + Status + '","SMonth":"' + 10 + '","SYear":"' + 2016 + '","TMonth":"' + 11 + '","TYear":"' + 2016 + '"}';
            $.ajax({
                type: "POST",
                url: "userdashboard.aspx/GetActclickGrid",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    var grid = $("#dvgrid").data("kendoGrid");
                    // detach events
                    if (grid != undefined) {
                        grid.destroy(); // destroy the Grid
                        $("#dvgrid").empty();
                    }
                    if (data.d.length == 0) {
                        alert("No Record Found!");
                        return;

                    }
                    var StrData = $.parseJSON(data.d.data);
                    var columns = data.d.columns;


                    $("#dvgrid").kendoGrid({
                        dataSource: {
                            data: StrData,
                        }
                    ,
                        height: 600,
                        scrollable: true,
                        resizable: true,
                        reorderable: true,
                        groupable: true,
                        sortable: true,
                        filterable: true,
                        toolbar: [{ name: "excel" }],
                        excel: {
                            fileName: "Report_Company.xlsx",
                            filterable: true
                        },
                        columns: columns
                    });
                    $("#ldrdvGrid").hide();
                },
                complete: function comp()
                { $("#ldrdvGrid").hide(); },
                error: function (data) {
                    alert(data.error);
                    $("#ldrdvGrid").hide();
                }

            });
        }

    </script>
    <script type="text/javascript">
        var sites = [];
        var markers = [];
        var map;
        var contentString = "";
        function drop() {
            var v1 = sites;
            clearMarkers();
            for (var i = 0; i < sites.length; i++) {
                addMarkerWithTimeout(sites[i], i * 200);
            }
        }
        function initMap() {
            //20.5937° N, 78.9629°
            var Country = { lat: 20.5937, lng: 78.9629 };
            map = new google.maps.Map(document.getElementById('map'), {
                zoom: 4,
                center: Country
                //center: { lat: 52.520, lng: 13.410 }
            });
            //drop();
            bindSite();
        }

        function addMarkerWithTimeout(obj, timeout) {
            var position = { lat: obj.lat, lng: obj.lng };
            window.setTimeout(function () {
                //{"SiteID":"1113227","Site":"Bengaluru","Delayed":0,"Performed":3,"Performed After Due Date":0,"latlong":"12.940726, 77.695473"}
                var contentString = '<div class="section">' +
                '<div id="sub-container">' +
                '<div class="sub-box">' +
                '<div class="sub-allcredit"> ' +
                '<div class="sub-icon">Performed </div>' +
                '<div class="sub-number" style="background:green;"> ' +obj.Performed + '</div>' +
                '<div class="clear"></div>' +
                '<div class="strap" style="background:green;"></div>' +
                '</div>' +
                '</div>' +
                '<div class="sub-box">' +
                '<div class="sub-allcredit"> ' +
                '<div class="sub-icon">Performed After Due Date</div>' +
                '<div class="sub-number" style="background:red;">' + obj["Performed After Due Date"] + '</div>' +
                '<div class="clear"></div>' +
                '<div class="strap" style="border-bottom:10px solid red;"></div>' +
                '</div>' +
                '</div>' +
                '<div class="sub-box">' +
                '<div class="sub-allcredit"> ' +
                '<div class="sub-icon">In Process</div>' +
                '<div class="sub-number" style="background:#FF9900;">' + obj.InProcess + '</div>' +
                ' <div class="clear"></div>' +
                '<div class="strap" style="border-bottom:10px solid #FF9900;"></div>' +
                '</div>' +
                ' <div class="sub-box"> ' +
                      '  <div class="sub-allcredit"> ' +
                       '     <div class="sub-icon">Delayed</div>' +
                       '     <div class="sub-number" style="background:#124ba0;">' + obj.Delayed + '</div>' +
                        '    <div class="clear"></div>'+
                '    <div class="strap" style="border-bottom:8px solid #124ba0;"></div>' +
                        '</div> '+
                    '</div>'+
                '</div>' +
                '</div>' +
            '</div>';

                //var contentString = "<div>Site Name : " + obj.Site + "<br/>Performed:" + obj.Performed + "<br/>Delayed: " + obj.Delayed + "<br/>Performed After Due Date: " + obj["Performed After Due Date"] + "<div>";
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });
                var marker = new google.maps.Marker({
                    position: position,
                    map: map,
                    animation: google.maps.Animation.DROP,
                    title: obj.Site

                });
                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });
                //infowindow.open(map, marker);
                markers.push(marker);
            }, timeout);
        }

        function clearMarkers() {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];
        }
        function bindSite() {
            $.ajax({
                type: "POST",
                url: "Userdashboard.aspx/GetSites",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    
                   var  Site = $.parseJSON(res);
                   $.each(Site, function (i) {
                       var arr = this.latlong.split(',');
                       var obj = { lat: 0, lng: 0 };
                       obj.lat = parseFloat(arr[0].trim());
                       obj.lng = parseFloat(arr[1].trim());
                       obj.Site = this.Site;
                       obj.Delayed = this.Delayed;
                       obj.Performed = this.Performed;
                       obj.InProcess = this.InProcess;
                       obj["Performed After Due Date"] = this["Performed After Due Date"];
                       //latlong
                       //{"SiteID":"1113227","Site":"Bengaluru","Delayed":0,"Performed":3,"Performed After Due Date":0,"latlong":"12.940726, 77.695473"}
                        sites.push(obj);
                   });
                   drop();
                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
        }
    </script>
    
    <script async defer 
            src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBqCmulejs-VROzzpz_cJsMl7tEDbdIZrw&callback=initMap">
    </script>
</asp:Content>

