<%@ Page Title="" Language="VB" AutoEventWireup="false" MasterPageFile="~/usrFullScreenBPM.master"
    CodeFile="MISDashboardNew.aspx.vb" Inherits="MISDashboardNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%-- <link href="css/StyleSheet.css" rel="stylesheet" type="stylesheet" />
    <link href="css/style.css" rel="stylesheet" type="stylesheet" />
    <link href="css/font-awesome.min.css" rel="stylesheet" />--%>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <!-- stylesheets -->
	<link href="MISDashboardCSS/font-roboto.css" rel="stylesheet">
	<link href="MISDashboardCSS/bootstrap4-alpha3.min.css" rel="stylesheet">
	<link href="MISDashboardCSS/font-awesome.min.css" rel="stylesheet">
	<link href="MISDashboardCSS/style.css" rel="stylesheet">
   
       <%--<script src="MISDashboardCSS/loading-bar.js"></script>
    <link href="MISDashboardCSS/loading-bar.css" rel="stylesheet">--%>
    <style>
        body {
            /*font-family: Metric;*/
            font-family: 'Helvetica', sans-serif;
        }
        .container-fluid {padding-left:4px!important; padding-right:4px!important;}
        .wrapper_container {
            width: 100%;
        }

        .nopadding {
        }

            .nopadding > [class*="col-"] {
                padding-left: 0px;
                padding-right: 0px;
            }
        .caret {
            display:none;
        }
        .dropdown-menu {
            position: absolute;
            top: 100%;
            left: 0;
            z-index: 1000;
            display: none;
            float: left;
            min-width: 160px;
            padding: 5px 0;
            margin: 2px 0 0;
            font-size: 14px;
            list-style: none;
            background-color: #2771A1;
            border: 1px solid #ccc;
            border: 1px solid rgba(0,0,0,0.15);
            border-radius: 4px;
            -webkit-box-shadow: 0 6px 12px rgba(0,0,0,0.175);
            box-shadow: 0 6px 12px rgba(0,0,0,0.175);
            background-clip: padding-box;
            color: #fff;
        }
        .small-box {
            /*width: 24%;
            float: left;
            position: relative;
            margin-right: 12px;*/
            font-size: 15px;
            min-height: 265px;
            margin: 5px;
            border-radius: 10px;
            border: solid 1px;
            border-color: black;
        }

        .text_formon > .lefttxt {
            float: left;
        }

        .text_formon > .righttxt {
            float: right;
            text-align: right;
        }

        .text_formon label {
            display: block;
            margin-bottom: 0px;
            color: white;
            font-size: 17px;
            text-align: center;
        }

            .text_formon label.count {
                font-size: 37px;
                text-align: center;
            }

            .text_formon label.Mcount {
                font-size: 60px;
            }

            .text_formon label.Scount {
                font-size: 80px;
            }

        .text_topfive {
            display: block;
            margin-bottom: 0px;
            color: white;
            font-size: 14px;
            text-align: left;
        }

        .bg-blues {
            background-color: #3b5998;
        }

        .bg-skyblue {
            background-color: #00aced;
        }

        .bg-greens {
            background-color: #73ae20;
        }

        .bg-yellows {
            background-color: #f39c12;
        }

        .bg-browns {
            background-color: #b84d45;
        }

        .bg-oranges {
            background-color: #f39c12;
        }

        .bg-purples {
            background-color: white;
        }

        .bg-greys {
            background-color: #333;
        }

        .small-box > .inner {
            padding: 10px;
        }

        .lefttxt label {
            font-weight: bold;
            margin: 0 0 4px 0;
            white-space: nowrap;
            color: white;
            text-align: left;
        }

        .diff_day {
            float: right;
            position: relative;
            top: -26px;
            text-align: right;
        }

            .diff_day label {
                float: left;
                display: block;
            }

            .diff_day .arrow {
                float: left;
                width: 20px;
                height: 19px;
                background-color: #fff;
                border-radius: 50%;
                padding: 0px;
                margin-left: 5px;
            }

            .diff_day i.fa-caret-up {
                color: #5acc1d;
            }

        .small-box p {
            font-size: 15px;
            margin: 0px 0 5px 0;
        }

        .small-box > .small-box-footer {
            bottom: 0;
            text-align: center;
            padding: 0px;
            color: black;
            display: block;
            z-index: inherit;
            background: rgba(0,0,0,0.1);
            text-decoration: none;
            width: 100%;
            /*background-color:gray;*/
            border-radius: 0px 0px 0px 0px;
        }

        .small-box > a {
            text-transform: uppercase;
            font-weight: bold;
            color: black;
        }

        /*.small-box > .small-box-footer {
            color: #fff;
            background: rgba(0,0,0,0.15);
        }*/

        .cl {
            clear: both;
        }

        #footer {
            display: none;
        }

        span.day {
            padding-left: 5px;
            font-size: 18px;
        }

        span.day-small {
            padding-left: 5px;
            font-size: 12px;
            float: left;
            color: #fff;
            padding-top: 5px;
        }

        .inner_div {
            padding: 0px 15px;
        }

        .text_formon > .righttxt label.day_wrp, .text_formon > .righttxt label.day_wrp span {
            float: left;
        }

        .pending_txt {
            float: left;
        }

        .year_dayscenter label {
            font-size: 32px;
        }

            .year_dayscenter label + label {
                font-size: 24px !important;
            }

        .donut-wrapper {
            position: relative;
            width: 280px;
            height: 280px;
            background-color: #3f3f3f;
        }

        /* The width and height of the chart must be equal to the width and height of the .donut-wrapper in order to be horizontally and vertically centered  */
        .donut-chart {
            width: 280px;
            height: 280px;
        }
        .loading img{ margin:auto;position:absolute;top:30%;left:45%; }
         #loading-div-background {
            display: none;
            flex-align:center;
            position:fixed;
            background: rgba(0, 0, 0, 0.54);
            width: 100%;
            height: 100%;
            z-index: 100;
        }

        .inner-content {
            position: absolute;
            top: 50%;
            left: 50%;
            width: 100px;
            height: 100px;
            margin-top: -40px;
            margin-left: -40px;
            font-size: 14px;
            line-height: 100px;
            vertical-align: middle;
            text-align: center;
            color: black;
        }

        .k-grid tr td, .k-pivot-layout .k-grid tr td {
            border-bottom-width: 1px !important;
            border-style: solid !important;
            vertical-align: middle !important;
            text-align: left;
        }

        .k-grid {
            font-size: 12px;
            font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
        }

            .k-grid td {
                line-height: 12px;
                vertical-align: middle;
                border-style: solid !important;
                border-width: 0 0 0 1px !important;
            }

            .k-grid th {
                font-size: 13px;
                font-weight: bold;
            }

        .k-link {
            padding: 0px 0px;
        }

       /* .k-widget k-datepicker k-header k-input {
            width:30%;
        }*/

        .container-fluid {
            padding-right: 15px;
            padding-left: 15px;
            margin-right: auto;
            margin-left: auto;
        }
        .main_menu {
          margin: 10px 11px 3px!important; 
        }
        .fixwidth > span  {
            width: 30%!important;
        }
        
         /*.ui-datepicker
      {
         display:none
      }*/
      
    </style>
    <script type="text/javascript">

        // Invoice Distribution
        function GetDataInvoiceDist(e) {
            //$("#loading-div-background").show();
            var type = $("#myList3 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            $('#AllExp').click(function () {
                txt = 'All';
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"'+ txt +'"}';
                     
            $("#datepicker2").kendoDatePicker({ dateFormat: 'dd-mm-yy' });
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/InvoiceDist",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    createChart(res);
                    //$("#loading-div-background").hide();

                }
            });

        }
        function createChart(data1) {
            //var tt1 = "Chaeck data";
            var dt = $.parseJSON(data1.d);
            $("#kgrdGrp").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    field: "DaysType",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },

                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                valueAxis: {
                    labels: {
                        format: "{0}",
                        rotation: -45
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",

                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category"
                },



                seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlInvoiceDist,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });
        }
        function GetDataInvoiceDist1(e) {
            //$("#loading-div-background").show();
            var type = $("#myList3 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = 'All';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"' + txt + '"}';
            $("#popupdivAllDetailChart_wnd_title").html('Invoice Distribution All Details Chart');
            $("#Button3").attr("onclick", "OpenInvoiceDistAllDetails()");
            $("#kgrdAllDetailChart").width(700).height(530);
            $("#kgrdAllDetailChart").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetailChart").kendoWindow({
                width: "850px",
                height: "550px",
                title: "Invoice Distribution All Details Chart",
                visible: false,
                modal: true
            });
            $("#popupdivAllDetailChart").data("kendoWindow").center().open();
            $("#datepicker2").kendoDatePicker({ dateFormat: 'dd-mm-yy' });
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/InvoiceDist",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    createChart1(res);
                    //$("#loading-div-background").hide();

                }
            });

        }
        function createChart1(data1) {
            var dt = $.parseJSON(data1.d);
            //ObjInvToolTip = data1.d.countseries;

            $("#kgrdAllDetailChart").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },
                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                //seriescount: data1.d.countseries,
                //series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",

                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category"
                },


                seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlInvoiceDist,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });               
                
        }
        function dtlInvoiceDist(e) {
           // $("#popupdivprg").show();
            $("#kgrdDetail").html('');
            $("#popupdivDetail_wnd_title").html('Invoice Distribution Details');
            var Doc = e.category;
            var name = e.series.name;
            var type = $("#myList3 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            //,"Sdate":"' + dt1 + '","Edate":"' + dt2 + '"
            $("#kgrdDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Invoice Distribution Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Doc":"' + Doc + '","Name":"' + name + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getInvoiceDistDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdDetail").html("");
                   // $("#popupdivprg").hide();
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid("kgrdDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
      
        // TOTAL ExpenseBreakup
        function GetDataExpenseBreakup(e) {
            //$("#loading-div-background").show();
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"'+ txt +'"}';
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataExpenseBreakup",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //$("#loading-div-background").hide();
                    createChartExpenseBreakup(res);

                }
            });

        }
        function createChartExpenseBreakup(data1) {
            //var tt1 = "Chaeck data";
            var dt = $.parseJSON(data1.d);
            $("#kgrdExpenseBreakup").kendoChart({
                //title: {
                //    text: ""
                //},
                //legend: {
                //    position: "bottom",
                //    labels: {
                //        font: "bold italic 10px Segoe UI, Arial;"
                //    }
                //},
                seriesDefaults: {
                        labels: {
                        visible: true,
                        format: "{0}%",
                        background: "transparent"
                    }
                },
                //series: [{
                //    type: "pie",
                //    data: [{
                //        category: "Recruitment Cost",
                //        value: 35
                //    }, {
                //        category: "Operational Cost",
                //        value: 25
                //    }, {
                //        category: "Printing Stationary",
                //        value: 20
                //    }, {
                //        category: "Others",
                //        value: 10
                //    }]
                //}],
                //seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //tooltip: {
                //    visible: true,
                //    template: "#= category # - #= kendo.format('{0:P}',percentage) #"
                //}
                title: {
                    position: "bottom",
                    text: "Rupees (Million)",
                    font: "10px"
                },
                legend: {
                    position: "bottom",
                    padding: 1,
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;"
                    },
                    
                },
                //seriesDefaults: {
                //        labels: {
                //        visible: true,
                //        format: "{0}%",
                //        background: "transparent"
                //        }
                   
                //},
                seriesDefaults: {
                    width: 30,
                    gap: 10,
                    labels: {
                        visible: true,
                        aglin: "left",
                        font: "10px Segoe UI, Arial ",
                        margin: { top: .5, left: 0 },
                        padding: 1,
                        rotation: "auto",
                        position: "auto",
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        template: "Mn ₹ #= value #"

                    },
                   
                    
                },
                
                dataSource: {
                    data: dt,
                   
                },
                //group: {field: "value", aggregates: [{
                //    field: "value", aggregate: "sum" * 100
                //}]},
                
                
                series: [{

                    type: "pie",
                    field: "value",
                   // datafield:"count",
                   // field:"count",
                    categoryField: "category"
                    //aggregate: "sum"
                    //explodeField: "count"
                    

                }],
               
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
               // seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlExpenseBreakup,
                tooltip: {
                    visible: true,
                    format: "{0}%",
                    template: "#= category # - Mn ₹ #= value # , (#=templateFormat(percentage) #%)"
                }
                              
            });
        }
        function templateFormat(value) {
            var percentage = Math.round(value * 100);
            //var minutes = Math.floor(value % 60);
            return percentage;
         }
        function dtlExpenseBreakup(e) {
           // $("#loading-div-background").show();
            $("#kgrdDetail").html('');
            $("#popupdivDetail_wnd_title").html('Expense Breakup Details');
            var Doc = e.category;
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            //,"Sdate":"' + dt1 + '","Edate":"' + dt2 + '"
            $("#kgrdDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Expense Breakup Details",
                visible: false,               
                modal: true
            });
            var str = '{"Type":"' + type + '","Doc":"' + Doc + '","Dtf":"'+ dt +'","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getExpenseBreakupDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdDetail").html("");
                   // $("#loading-div-background").hide();
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid("kgrdDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function GetDataExpenseBreakup1(e) {
            //$("#loading-div-background").show();
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            txt = 'All';
           
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"' + txt + '"}';
            $("#popupdivAllDetailChart_wnd_title").html('Expense Breakup All Details Chart');
            $("#Button3").attr("onclick", "OpenExpensAllDetails()");
            $("#kgrdAllDetailChart").width(700).height(530);
            $("#kgrdAllDetailChart").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetailChart").kendoWindow({
                width: "850px",
                height: "550px",
                title: "Expense Breakup All Details Chart",
                visible: false,
                modal: true
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetailChart").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataExpenseBreakup",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //$("#loading-div-background").hide();
                    createChartExpenseBreakup1(res);

                }
            });

        }
        function createChartExpenseBreakup1(data1) {
            //var tt1 = "Chaeck data";
            var dt = $.parseJSON(data1.d);
            $("#kgrdAllDetailChart").kendoChart({
                //title: {
                //    text: ""
                //},
                //legend: {
                //    position: "bottom",
                //    labels: {
                //        font: "bold italic 10px Segoe UI, Arial;"
                //    }
                //},
                //seriesDefaults: {
                //        labels: {
                //        visible: true,
                //        format: "{0}%",
                //        background: "transparent"
                //    }
                //},
                //series: [{
                //    type: "pie",
                //    data: [{
                //        category: "Recruitment Cost",
                //        value: 35
                //    }, {
                //        category: "Operational Cost",
                //        value: 25
                //    }, {
                //        category: "Printing Stationary",
                //        value: 20
                //    }, {
                //        category: "Others",
                //        value: 10
                //    }]
                //}],
                //seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //tooltip: {
                //    visible: true,
                //    template: "#= category # - #= kendo.format('{0:P}',percentage) #"
                //}
                title: {
                    position: "bottom",
                    text: "Rupees (Million)",
                    font: "10px"
                },
                legend: {
                    position: "bottom",
                    padding: 1,
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;"
                    },

                },
                //seriesDefaults: {
                //        labels: {
                //        visible: true,
                //        format: "{0}%",
                //        background: "transparent"
                //        }

                //},
                seriesDefaults: {
                    width: 30,
                    gap: 10,
                    labels: {
                        visible: true,
                        aglin: "left",
                        font: "10px Segoe UI, Arial ",
                        margin: { top: .5, left: 0 },
                        padding: 1,
                        rotation: "auto",
                        position: "auto",
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        template: "Mn ₹ #= value #"

                    },


                },

                dataSource: {
                    data: dt,

                },
                //group: {field: "value", aggregates: [{
                //    field: "value", aggregate: "sum" * 100
                //}]},


                series: [{

                    type: "pie",
                    field: "value",
                    // datafield:"count",
                    // field:"count",
                    categoryField: "category"
                    //aggregate: "sum"
                    //explodeField: "count"


                }],

                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                // seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlExpenseBreakup,
                tooltip: {
                    visible: true,
                    format: "{0}%",
                    template: "#= category # - Mn ₹ #= value # , (#=templateFormat(percentage) #%)"
                }

            });
        }
        
        // Supplier Spend Break UP
        function GetDataSuppSpendBreakup() {
            //$("#loading-div-background").show();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            var str = '{"Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"'+ txt +'"}';
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataSuppSpendBreakup",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //$("#loading-div-background").hide();
                    createChartSuppSpendBreakup(res);

                }
            });

        }
        function createChartSuppSpendBreakup(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdSuppSpendBreakup").kendoChart({
            //    title: {
            //        text: ""
            //    },
            //    legend: {
            //        position: "bottom",
            //        labels: {
            //            font: "bold italic 10px Segoe UI, Arial;"
            //        }
            //    },
            //    seriesDefaults: {
            //        labels: {
            //            visible: true,
            //            format: "{0}%",
            //            background: "transparent"
            //        }
            //    },
            //    series: [{
            //        type: "pie",
            //        data: [{
            //            category: "ABC Limited",
            //            value: 18
            //        }, {
            //            category: "KPMG",
            //            value: 25
            //        }, {
            //            category: "Maruti",
            //            value: 28
            //        }, {
            //            category: "JK Paper",
            //            value: 12
            //        }, {
            //            category: "Others",
            //            value: 16
            //        }
            //        ]
            //    }],
            //    seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
            //    tooltip: {
            //        visible: true,
            //        template: "#= category # - #= kendo.format('{0:P}', percentage) #"
            //    }
                //});
                title: {
                    position: "right",
                    text: "Rupees (Million)",
                    font: "10px"
                },
                legend: {
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;"
                    }
                },
                //seriesDefaults: {
                //    labels: {
                //        visible: true,
                //        format: "{0}%",
                //        background: "transparent"
                //    }
                   
                //},
                seriesDefaults: {
                    width: 30,
                    gap: 10,
                    labels: {
                        visible: true,
                        aglin: "left",
                        font: "10px Segoe UI, Arial ",
                        margin: { top: .5, left: 0 },
                        padding: 1,
                        rotation: "auto",
                        position: "auto",
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        template: "Mn ₹ #= value #"

                    },
                    
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{

                    type: "pie",
                    field: "value",
                    categoryField: "category"

                }],

                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                // seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlSupplierBreakup,
                tooltip: {
                    visible: true,
                    template: "#= category # - Mn ₹ #= value # (#=templateFormat(percentage) #%)"
                }

            });
        }
        function dtlSupplierBreakup(e) {
            //$("#loading-div-background").show();
            $("#kgrdDetail").html('');
            $("#popupdivDetail_wnd_title").html('Supplier Spend Breakup');
            var Doc = e.category;
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            //,"Sdate":"' + dt1 + '","Edate":"' + dt2 + '"
            $("#kgrdDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Supplier Spend Breakup",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Doc":"' + Doc + '","Dtf":"'+ dt +'","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getSupplierBreakupDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdDetail").html("");
                    //$("#loading-div-background").hide();
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                       // var Columns = result.d.Column;
                        //var data = JSON.parse(result.d.Data);
                        //  var CommanObj = { command: { text: "View Map", click: DetailHandler} }
                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid("kgrdDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function GetDataSuppSpendBreakup1() {
            //$("#loading-div-background").show();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = 'All';
            var str = '{"Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"'+ txt +'"}';
            $("#popupdivAllDetailChart_wnd_title").html('Supplier Spend Breakup All Details Chart');

            $("#kgrdAllDetailChart").width(850).height(530);
            $("#Button3").attr("onclick", "OpenSupplierBreakupAllDetails()");
            $("#kgrdAllDetailChart").html('');
            $("#kgrdAllDetailChart").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetailChart").kendoWindow({
                width: "850px",
                height: "570px",
                title: "Supplier All Details Chart",
                visible: false,
                modal: true
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetailChart").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataSuppSpendBreakup",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                   // $("#loading-div-background").hide();
                    createChartSuppSpendBreakup1(res);

                }
            });

        }
        function createChartSuppSpendBreakup1(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdAllDetailChart").kendoChart({
               
                title: {
                    position: "right",
                    text: "Rupees (Million)",
                    font: "10px"
                },
                legend: {
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;"
                    }
                },
                //seriesDefaults: {
                //    labels: {
                //        visible: true,
                //        format: "{0}%",
                //        background: "transparent"
                //    }

                //},
                seriesDefaults: {
                    width: 30,
                    gap: 10,
                    labels: {
                        visible: true,
                        aglin: "left",
                        font: "10px Segoe UI, Arial ",
                        margin: { top: .5, left: 0 },
                        padding: 1,
                        rotation: "auto",
                        position: "auto",
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        template: "Mn ₹ #= value #"

                    },

                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{

                    type: "pie",
                    field: "value",
                    categoryField: "category"

                }],

                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                // seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlSupplierBreakup,
                tooltip: {
                    visible: true,
                    template: "#= category # - Mn ₹ #= value # (#=templateFormat(percentage) #%)"
                }

            });
        }
        
        // SLA  Performance
        function GetDataSLAPerformance() {
            //$("#loading-div-background").show();
            var type = $("#select5 option:selected").text();
            var sts = $("#Select6 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            $('#SLA').click(function () {
                txt = 'All';
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Status":"' + sts + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"'+ txt +'"}';
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataSLAPerformance",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //$("#loading-div-background").hide();
                    createChartSLAPerformance(res);

                }
            });

        }
        function createChartSLAPerformance(data1) {
            var dt = $.parseJSON(data1.d);
            $("#kgrdGrpSLAPerformance").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    field: "DaysType",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },

              dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                valueAxis: {
                    labels: {
                        format: "{0}",
                        rotation: -45
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",

                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category",
                    gridPosition: "start"
                },


                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],

                seriesClick: dtlSLA,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });
        }
        function dtlSLA(e) {
            //$("#loading-div-background").show();
            $("#kgrdDetail").html('');
            $("#popupdivDetail_wnd_title").html('SLA Performance Details');
            var Doc = e.category;
            var name = e.series.name;
            var type = $("#select5 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var sts = $("#Select6 option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            //,"Sdate":"' + dt1 + '","Edate":"' + dt2 + '"
            $("#kgrdDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "SLA Performance Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Doc":"' + Doc + '","Name": "' + name + '","Status":"'+ sts +'","Dtf":"'+ dt +'","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getSLAPerformanceDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdDetail").html("");
                   // $("#loading-div-background").hide();
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid("kgrdDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function GetDataSLAPerformance1() {
            //$("#loading-div-background").show();
            var type = $("#select5 option:selected").text();
            var sts = $("#Select6 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            $('#SLA').click(function () {
                txt = 'All';
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Status":"' + sts + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"' + txt + '"}';
            $("#popupdivAllDetailChart_wnd_title").html('SLA Performance All Details Chart');

            $("#kgrdAllDetailChart").width(700).height(530);
            $("#Button3").attr("onclick", "OpenSLAPerformanceAllDetails()");
            $("#kgrdAllDetailChart").html('');
            $("#kgrdAllDetailChart").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetailChart").kendoWindow({
                width: "850px",
                height: "570px",
                title: "Supplier All Details Chart",
                visible: false,
                modal: true
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetailChart").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataSLAPerformance",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //$("#loading-div-background").hide();
                    createChartSLAPerformance1(res);

                }
            });

        }
        function createChartSLAPerformance1(data1) {
            var dt = $.parseJSON(data1.d);
            $("#kgrdAllDetailChart").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },

                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                //seriescount: data1.d.countseries,
                //series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",

                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category"
                },


                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],

                seriesClick: dtlSLA,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });
        }
        
        // TOTAL Invoice LifeCycle
        var ObjInvToolTip;
        function GetDataInvoiceLifeCycle() {
            $("#loading-div-background").show();
            var dt = $("#periodF option:selected").text();
            var type = $("#myList option:selected").text();
            var invdt = $("#myList1 option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = '';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Invdt": "' + invdt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"'+ txt +'"}';
            $.ajax({
                type: "POST",
                //url: "MISDashboardNew.aspx/GetDataInvoiceLifeCycle",
                url: "MISDashboardNew.aspx/GetDataInvoiceLifeCycle",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    $("#loading-div-background").hide();
                    createChartInvoiceLifeCycle(res);
                    //createChartInvoiceLifeCycle1(res);
                }
            });

        }
        function createChartInvoiceLifeCycle(data1) {
            var dt = $.parseJSON(data1.d);
            //ObjInvToolTip = data1.d.countseries;
             $("#kgrdGrpInvoiceLifeCycle").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },

               

                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType",
                    
                    },
                   
                },
               
                valueAxis: {
                    labels: {
                        format: "{0}",
                        rotation: -45
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",
                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category"
                },


                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],

                seriesClick: dtlInvoiceLifeCycle,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                },              

            });
                    

        }
        function GetDataInvoiceLifeCycle1() {
            //$("#loading-div-background").show();
            var dt = $("#periodF option:selected").text();
            var type = $("#myList option:selected").text();
            var invdt = $("#myList1 option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            var txt = 'All';
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Invdt": "' + invdt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '","All":"' + txt + '"}';
            $("#popupdivAllDetailChart_wnd_title").html('Invoice LifeCycle All Details Chart');
            $("#Button3").attr("onclick", "OpenInvoiceLifeCycleAllDetails()");
            $("#kgrdAllDetailChart").width(700).height(530);
            $("#kgrdAllDetailChart").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetailChart").kendoWindow({
                width: "850px",
                height: "550px",
                title: "",
                visible: false,
                modal: true
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetailChart").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                //url: "MISDashboardNew.aspx/GetDataInvoiceLifeCycle",
                url: "MISDashboardNew.aspx/GetDataInvoiceLifeCycle",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //createChartInvoiceLifeCycle(res);
                   // $("#loading-div-background").hide();
                    createChartInvoiceLifeCycle1(res);
                }
            });

        }
        function createChartInvoiceLifeCycle1(data1) {
            var dt = $.parseJSON(data1.d);
            //ObjInvToolTip = data1.d.countseries;

            $("#kgrdAllDetailChart").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },
                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                //seriescount: data1.d.countseries,
                //series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                       
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                                visible: true,
                                font: "12px Segoe UI, Arial ",
                                margin: { top: 0, left: 0 },
                                border: { color: "black", width: 0 },
                                background: "transparent",
                                format: "N0",
                                position: "center",
                               // template: " #=  value #",

                                color: "white"
                            },
                }],
                categoryAxis: {
                    field: "category"
                },


                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
               
                seriesClick: dtlInvoiceLifeCycle,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });
        }
        function dtlInvoiceLifeCycle(e) {
            // $("#loading-div-background").show();
            $("#kgrdDetail").html('');
            $("#popupdivDetail_wnd_title").html('Invoice Lifecycle Details');
            var Doc = e.category;
            var name = e.series.name;
            var type = $("#myList option:selected").text();
            var dt = $("#periodF option:selected").text();
            var invdt = $("#myList1 option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            //,"Sdate":"' + dt1 + '","Edate":"' + dt2 + '"
            $("#kgrdDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Invoice Lifecycle Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Doc":"' + Doc + '","Name": "' + name + '","Dtf":"' + dt + '","Invdt": "' + invdt + '" ,"Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getInvoiceLifeCycleDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdDetail").html("");
                   // $("#loading-div-background").hide();
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid("kgrdDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }

        // Total Invoice Ageing
        function GetDataInvoiceAgeing() {
            //$("#loading-div-background").show();
            var dt = $("#periodF option:selected").text();
            var type = $("#Select3 option:selected").text();
            var invdt = $("#Select4 option:selected").text();
            var txt = '';
            $('#InvAge').click(function () {
                txt = 'All';
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Invdt": "' + invdt + '","All":"'+ txt +'"}';
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataInvoiceAgeing",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                   createChartInvoiceAgeing(res);
                   // $("#loading-div-background").hide();
                }
            });

        }
        function createChartInvoiceAgeing(data1) {
            var dt = $.parseJSON(data1.d);
            $("#kgrpInvoiceAgeing").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },


                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                //seriescount: data1.d.countseries,
                //series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}",
                        rotation: -45
                    },
                    line: {
                        visible: false
                    },

                    //axisCrossingValue: 5
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",

                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category",
                    //gridPosition: "start",
                    rotation: -45
                    //labels: {
                    //    template: function (e) {
                    //        return e.category.split("+ ").join("+\n");
                    //    }
                    //}
                },


                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],

                seriesClick: dtlOpenInvoiceAgeing,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });
        }
        function dtlOpenInvoiceAgeing(e) {
            //  $("#popupdivprg").show();
            //CheckSession();
            $("#kgrdDetail").html('');
            $("#popupdivDetail_wnd_title").html('Open Invoice Ageing Details');
            var Doc = e.category;
            var name = e.series.name;
            var type = $("#Select3 option:selected").text();
            var invdt = $("#Select4 option:selected").text();
            var dt = $("#periodF option:selected").text();
            $("#kgrdDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Open Invoice Ageing Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Doc":"' + Doc + '","Name": "' + name + '","Invdt":"'+ invdt +'","Dtf":"'+ dt +'"}';
            $("#popupdivDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getOpenInvoiceAgeingDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    //$("#popupdivprg").hide();
                    $("#kgrdDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid("kgrdDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function GetDataInvoiceAgeing1() {
            $("#loading-div-background").show();
            var dt = $("#periodF option:selected").text();
            var type = $("#Select3 option:selected").text();
            var invdt = $("#Select4 option:selected").text();
            var txt = '';
            $('#InvAge').click(function () {
                txt = 'All';
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Invdt": "' + invdt + '","All":"' + txt + '"}';
            $("#popupdivAllDetailChart_wnd_title").html('Invoice Ageing All Details Chart');
            $("#Button3").attr("onclick", "OpenInvoiceAgeingAllDetails()");
            $("#kgrdAllDetailChart").width(700).height(530);
            $("#kgrdAllDetailChart").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetailChart").kendoWindow({
                width: "850px",
                height: "550px",
                title: "",
                visible: false,
                modal: true
            });
            //var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetailChart").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetDataInvoiceAgeing",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    //$("#loading-div-background").hide();
                    createChartInvoiceAgeing1(res);
                    $("#loading-div-background").hide();

                }
            });

        }
        function createChartInvoiceAgeing1(data1) {
            var dt = $.parseJSON(data1.d);
            $("#kgrdAllDetailChart").kendoChart({
                title: {
                    position: "right",
                    text: "Invoice Count",
                    font: "10px"
                },

                legend: {
                    visible: true,
                    position: "bottom",
                    labels: {
                        font: "bold italic 10px Segoe UI, Arial;",

                    }


                },

                dataSource: {
                    data: dt,
                    group: {
                        field: "DaysType"
                    }
                },

                //seriescount: data1.d.countseries,
                //series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                    },
                    line: {
                        visible: false
                    },

                   // axisCrossingValue: 10
                },
                transitions: false,
                series: [{
                    type: "bar",
                    stack: "true",
                    field: "Count",
                    //stack: true,
                    width: 60,
                    gap: .5,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        // template: " #=  value #",

                        color: "white"
                    },
                }],
                categoryAxis: {
                    field: "category"
                },


                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                seriesColors: ["#f3ac32", "#b8b8b8", "#bb6e36", "#cd1533", "#d43851", "#cd1533", "#d43851", "#e47f8f", "#eba1ad", "#9C27B0", "#1B5E20", "#00ACC1"],
                //seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],

                seriesClick: dtlOpenInvoiceAgeing,

                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#=  category #, Count - #=  value # , Mn ₹  #=   dataItem.Amount # "
                }
            });
        }

        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }

        function ShowDialog(url) {
            // do some thing with currObj data

            var $dialog = $('<div></div>')
            .load(url)
            .dialog({
                autoOpen: true,
                title: 'Document Detail',
                width: 700,
                height: 550,
                modal: true
            });
            return false;
        }

        function DetailHandler(evt) {
            //dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var docid = evt.data.DOCID;
            //  window.open('DocDetail.aspx?DOCID=' + docid + '');

            window.open('DocDetail.aspx?DOCID=' + docid + '', '_blank');

            //OpenWindow('DocDetail.aspx?DOCID=' + docid + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }
        
        function bindGrid(gridDiv, Data1, columns, pageable, filterable, sortable, height) {

            $("#" + gridDiv).html('');
            var g = $("#" + gridDiv).data("kendoGrid");
            if (g != null || g != undefined) {
                g.destroy();
                $("#" + gridDiv).html('');
            }
            gridDiv = $("#" + gridDiv).kendoGrid({
                dataSource: {
                    pageSize: 20,
                    data: Data1
                },
                scrollable: {
                    virtual: true
                },

                columns: columns,
                pageable: true,
                pageSize: 20,
                scrollable: true,
                reorderable: true,
                columnMenu: true,
                dataBound: onDataBound,
                groupable: true,
                sortable: true,
                filterable: true,
                resizable: true,
                height: height,
                toolbar: ['excel'],
                excel: {
                    fileName: "Report.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }

        function bindGrid1(gridDiv, Data1, columns, pageable, filterable, sortable, height) {

            $("#" + gridDiv).html('');
            var g = $("#" + gridDiv).data("kendoGrid");
            if (g != null || g != undefined) {
                g.destroy();
                $("#" + gridDiv).html('');
            }
            gridDiv = $("#" + gridDiv).kendoGrid({
                dataSource: {
                    pageSize: 20,
                    data: Data1
                },
                scrollable: {
                    virtual: true
                },

                columns: columns,
                pageable: true,
                pageSize: 20,
                scrollable: true,
                reorderable: true,
                columnMenu: true,
                dataBound: onDataBound1,
                groupable: true,
                sortable: true,
                filterable: true,
                resizable: true,
                height: height,
                toolbar: ['excel'],
                excel: {
                    fileName: "Report.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }
                
        function GetQueryString(param) {
            var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < url.length; i++) {
                var urlparam = url[i].split('=');
                if (urlparam[0] == param) {
                    return urlparam[1];
                }
            }
        }
        function onDataBound(e) {
            var grid = $("#kgrdDetail").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var DOCID = gridData[i].DocID;
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
            }
        }
        function onDataBound1(e) {
            var grid = $("#kgrdAllDetail").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var DOCID = gridData[i].DocID;
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
            }
        }
        function OpenExpensAllDetails(e) {
            //debugger;
            $("#kgrdAllDetail").html('');
            $("#popupdivAllDetail_wnd_title").html('Expense Breakup All Details');
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            $("#kgrdAllDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Expense Breakup All Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getExpenseBreakupAllDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdAllDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdAllDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid1("kgrdAllDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function OpenSupplierBreakupAllDetails(e) {
            //debugger;
            $("#kgrdAllDetail").html('');
            $("#popupdivAllDetail_wnd_title").html('Supplier Breakup All Details');
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            $("#kgrdAllDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Supplier Breakup All Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getSupplierBreakupAllDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdAllDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdAllDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid1("kgrdAllDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function OpenInvoiceDistAllDetails(e) {
            //debugger;
            $("#kgrdAllDetail").html('');
            $("#popupdivAllDetail_wnd_title").html('Invoice Distribution All Details');
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            $("#kgrdAllDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Invoice Distribution All Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getInvoiceDistAllDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdAllDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdAllDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid1("kgrdAllDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function OpenInvoiceLifeCycleAllDetails(e) {
            //debugger;
            $("#kgrdAllDetail").html('');
            $("#popupdivAllDetail_wnd_title").html('Invoice Life Cycle All Details');
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            $("#kgrdAllDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Invoice Life Cycle All Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getInvoiceLifeCycleAllDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdAllDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdAllDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid1("kgrdAllDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function OpenInvoiceAgeingAllDetails(e) {
            //debugger;
            $("#kgrdAllDetail").html('');
            $("#popupdivAllDetail_wnd_title").html('Invoice Ageing All Details');
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            $("#kgrdAllDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "Invoice Ageing All Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getOpenInvoiceAgeingAllDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdAllDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdAllDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid1("kgrdAllDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function OpenSLAPerformanceAllDetails(e) {
            //debugger;
            $("#kgrdAllDetail").html('');
            $("#popupdivAllDetail_wnd_title").html('SLA Performance All Details');
            var type = $("#select2 option:selected").text();
            var dt = $("#periodF option:selected").text();
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            $("#kgrdAllDetail").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivAllDetail").kendoWindow({
                width: "1150px",
                height: "550px",
                title: "SLA Performance All Details",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + type + '","Dtf":"' + dt + '","Sdate":"' + dt1 + '","Edate":"' + dt2 + '"}';
            $("#popupdivAllDetail").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/getSLAPerformanceAllDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdAllDetail").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdAllDetail").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);

                        var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
                        bindGrid1("kgrdAllDetail", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        function DateFilter() {
            $("#loading-div-background").show();
            var dttext = $("#periodF option:selected").text();
            if (dttext == 'Other Date') {
                $("#divdt").show();
                $("#datepicker1").kendoDatePicker();
                var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');
                $("#datepicker1").data("kendoDatePicker").value(todayDate);
                $("#datepicker2").kendoDatePicker();
                $("#datepicker2").data("kendoDatePicker").value(todayDate);
               
            }
            else {
               // $("#datepicker1").hide();
                //$("#datepicker2").hide();
                $("#divdt").hide();
                GetDataInvoiceDist();
                GetDataExpenseBreakup();
                GetDataSuppSpendBreakup();
                GetDataSLAPerformance();
                GetDataInvoiceLifeCycle();
                // GetDataInvoiceAgeing();
                //$("#loading-div-background").hide();
            }
        }
        
        function callfunction() {
            var dt1 = $("#datepicker1").val();
            var dt2 = $("#datepicker2").val();
            //debugger;
            if (dt1 == '' || dt2 == '') {
                alert("Please select Start/End Date. ");
            }
            else {
                GetDataInvoiceDist();
                GetDataExpenseBreakup();
                GetDataSuppSpendBreakup();
                GetDataSLAPerformance();
                GetDataInvoiceLifeCycle();
            }
        }
        function LoadSLADept() {
            $.ajax({
                    type: "POST",
                    url: "MISDashboardNew.aspx/GetSLADepartments",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#select5").get(0).options.length = 0;
                        $("#select5").get(0).options[0] = new Option("All Departments", "-1");
                        var dt = $.parseJSON(data.d);
                        for (var i = 0; i < dt.length; i++) {
                            var opt = new Option(dt[i].Display);
                            $("#select5").append(opt);
                           
                        }
                    },
                    error: function () {
                        alert("Failed to load Department");
                    }
                });
        }
        function LoadAgeingDept() {
            $.ajax({
                type: "POST",
                url: "MISDashboardNew.aspx/GetAgeingDepartments",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#Select3").get(0).options.length = 0;
                    $("#Select3").get(0).options[0] = new Option("All Departments", "-1");
                    var dt = $.parseJSON(data.d);
                    for (var i = 0; i < dt.length; i++) {
                        var opt = new Option(dt[i].Display);
                        $("#Select3").append(opt);
                    }
                },
                error: function () {
                    alert("Failed to load Department");
                }
            });
        }
        $(document).ready(function () {
            $("#loading-div-background").show();
            GetDataInvoiceAgeing();
            DateFilter();
            LoadSLADept();
            LoadAgeingDept();
            //$("#loading-div-background").hide();
            $("#monthpicker").kendoDatePicker({
                // defines the start view
                start: "year",

                // defines when the calendar should return date
                depth: "year",

                // display month and year in the input
                format: "dd-mm-yyyy",

                // specifies that DateInput is used for masking the input element
                dateInput: true
            });

        });

    </script>
   
       <html>
           <%--//style="margin-top:10px;"--%>
<body>

    <div class="container-fluid">
			<div class="pull-right" style="margin-top:10px;" >
					<select id = "periodF" onchange="DateFilter()" style="font-weight: bold;"  >
						<option value = "1">Current FY</option>
                       
                        <option value = "2">Current Month</option>
                                  
						<option value = "3">Current Quarter</option>
						<option value = "4">Last Quarter</option>               
						 <option value = "5">Last Month</option>  
						<option value = "6">Last FY</option> 
                        <option value = "7">Other Date</option>               
						<%--<option value = "7">Other Period</option> --%>              
					</select>					
					<%--<select id = "periodV">
						<option value = "1">Month</option>
						<option value = "1">Quarter</option>
						<option value = "1">FY</option>
					</select>					
					<select id = "Select1">
						<option value = "1">Dec 2018</option>
						<option value = "1">Jan 2019</option>
						<option value = "1">Feb 2019</option>
					</select>	--%>	
               <%-- <input type="text" id="monthpicker"	/>		--%>
                
                
                   
			
       <span id="divdt" class="fixwidth">
            <input id="datepicker1" title="StartDate" style="width:30%" /> 
                <input id="datepicker2"   title="EndDate" />
                <button type="button" id="show" onclick="callfunction()"><b>Show</b></button>
     </span>
           </div>
        <div class="pull-right" style="margin-top:40px;">
         
        </div>

        <%-- <div class="box-col">
            <button class='export-img k-button'>Export as Image</button>
        </div>		--%>
		
      </div>
      
         

	<div>
		<div class="container-fluid">

           

    </div>			<!-- 1st row -->
			<div class="col-md-12 m-b-2">
				 
                <!-- Progress Bar-->
    <div id="loading-div-background" class="loading">
        <div id="loading-div">
            <img style="height: 40px; margin: 4px;" src="images/prg.gif" alt="Loading.." />
            <%--<h6 style="color: black; font-weight: normal;">Please wait....</h6>--%>
        </div>
    </div>   
				<div class="col-lg-4" id="Expense">
					<div class="card card-block">
						<div class="card-title row">
							<div class="col-md-8">
                                <%--<a href="#" onclick="OpenAllDetails()">--%>
								<div class=""><b>Expense Breakup</b> <img src="Images/DashboardIcon.png" id="AllExp" alt="All" title="Click here for all details " onclick="GetDataExpenseBreakup1()"/></div>
                                    <%--<button type="button" id="Button1" onclick="GetDataExpenseBreakup1()" style="font:icon"><b>All</b></button>--%>
                                						                 
								<%--<div class="row" style="font-size:9px"><b>Invoices submitted top 5 during the period</b></div>--%>
							</div>
							<div class="col-md-4">
										<div class="select-one" >
													<select id = "select2" onchange="GetDataExpenseBreakup()">
													<option value = "1">Department</option>
                                                   <%-- <option value = "2">Expense Nature</option> --%>  
													</select>
										</div>
							</div>
                            <div class="col-md-12">
								<div class="" style="font-size:10px"><b>Invoices submitted top 5 during the period</b></div>
							</div>
						</div>
                     <div id="kgrdExpenseBreakup" style="height: 310px; width: 340px;"></div>
					</div>
				</div>

				<div class="col-md-4" id="InvLC">
					<div class="card card-block">
						<div class="card-title row">
								<div class="col-md-8">
									<div class=""><b>Invoice Lifecycle</b>
                                        <img src="Images/DashboardIcon.png" id="Invlf" alt="All" title="Click here for all details "  onclick="GetDataInvoiceLifeCycle1()" />
									</div>
                                    <div class="" style="font-size:10px"><b>Invoices paid top 5 during the period</b></div>
								</div>
								<div class="col-md-4">
									<div class="select-one " >
										<select id = "myList" onchange="GetDataInvoiceLifeCycle()">
											<option value = "1">Department</option>
											<%--<option value = "2">Expense Nature</option>  --%>             
										</select>
									</div>
                                    <br />
									<div class="select-one " >
										<select id = "myList1" onchange="GetDataInvoiceLifeCycle()">
											<option value = "1">By Receipt Date</option>  
                                            <option value = "2">By Invoice Date</option>
										</select>
									</div>
								</div>
                            <%--<div class="col-md-12">
									
								</div>--%>
						</div>
						 <div id="kgrdGrpInvoiceLifeCycle" style="height: 310px; width: 380px;"></div>
					</div>
				</div>

				<div class="col-md-4" id="InvAgeing">
					<div class="card card-block">
						<div class="card-title row" >
								<div class="col-md-8">
									<div class=""><b>Open Invoice Ageing</b>
                                         <img src="Images/DashboardIcon.png" id="InvAge" title="Click here for all details "  alt="All" onclick="GetDataInvoiceAgeing1()" />
									</div>
                                    <div class="" style="font-size:10px"><b>Unpaid invoices as on date</b></div>
								</div>
								<div class="col-md-4">
									<div class="select-one " >
										<select id = "Select3" style="width:100%;text-align:left" onchange="GetDataInvoiceAgeing()">
											<option value = "1">Department</option>
											<%--<option value = "2">Expense Nature</option>  --%>             
										</select>
									</div>
                                    <br />
									<div class="select-one " >
										<select id = "Select4" onchange="GetDataInvoiceAgeing()">
											<option value = "1">By Receipt Date</option>   
                                            <option value = "2">By Invoice Date</option>
											            
										</select>
									</div>
								</div>
                           <%-- <div class="col-md-12">
									
								</div>--%>
                            
						</div>
						 <div id="kgrpInvoiceAgeing" style="height: 310px; width: 380px;"></div>
					</div>
                    
				</div>

                <div class="col-lg-4" id="supp1">
					<div class="card card-block">
						<div class="card-title row">
							<div class="col-md-8">
								<div class=""><b>Supplier Spend Breakup </b><img src="Images/DashboardIcon.png" id="supp" title="Click here for all details "  alt="All" onclick="GetDataSuppSpendBreakup1()" /> </div>
							</div>
                           <div class="col-md-12">
								<div class="" style="font-size:10px"><b>Spend on Top 5 suppliers during the period</b></div>
							</div>
						</div>
						 <div id="kgrdSuppSpendBreakup" style="height: 350px; width: 320px;"></div>
                        
					</div>
                    
				</div>
				<div class="col-md-4" id="SLA1" >
					<div class="card card-block">
						<div class="card-title row">
							<div class="col-md-8">
								<div class=""><b>SLA Performance</b>
                                     <img src="Images/DashboardIcon.png" alt="All" onclick="GetDataSLAPerformance1()" id="SLA" title="Click here for all details "  />
								</div>
                                <div class="" style="font-size:10px"><b>SLA Tracking of the invoices during the period</b></div>
							</div>
                            <div class="col-md-4">
										<div class="select-one " >
													<select id = "select5" onchange="GetDataSLAPerformance()" style="width:100%;text-align:left">
													<option value = "1">Department</option>
													</select>
										</div>
                                <br />
									<div class="select-one " >
										<select id = "Select6" onchange="GetDataSLAPerformance()">
											<option value = "1">All </option>
											<option value = "2">Paid</option>
                                            <option value = "2">Unpaid</option>                
										</select>
									</div>
                                 
							</div>
                           <%-- <div class="col-md-12">
								
							</div>--%>
						</div>
						<div id="kgrdGrpSLAPerformance" style="height: 310px; width: 380px;"></div>
					</div>
                   
				</div>
				<div class="col-lg-4" id="InvDist">
					<div class="card card-block">
						<div class="card-title row">
								 
									<div class="col-md-8">
										<div class=""><b>Invoice Distribution</b>
                                             <img src="Images/DashboardIcon.png" alt="All" onclick="GetDataInvoiceDist1()" id="InvDistall" title="Click here for all details "  />
										</div>
                                       	
									</div>
                            
									<div class="col-md-4">
										<div class="select-one">
											<select id = "myList3" onchange="GetDataInvoiceDist()">
												<option value = "1">Department</option>
												<%--<option value = "2" disabled="disabled">Expense Nature</option>--%>               
											</select>
										</div>
									</div>                           
								 	
                                    <div class="col-md-12">
                                       <div class="" style="font-size:10px"><b>Top 5 distribution of Invoices during the period - Against PO or Without PO</b></div>	
									</div>				
						</div>
                       		<div id="kgrdGrp" style="height: 310px; width: 380px;"></div>				
					</div>
				</div>
			</div>
    		</div>

    <%--<div id="popupdivprg" class="loading">
        <div id="Div2">
            <img style="height: 40px; margin: 4px;" src="images/prg.gif" alt="Loading.." />
        </div>
    </div>--%>  
	    <div id="popupdivDetail" title="Details" style="display: none;">
            
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgrdDetail" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="popupdivAllDetail" title="All Details" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgrdAllDetail" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

     <div id="popupdivAllDetailChart" title="All Details" style="display: none;">
        <table style="width: 100%;">
            <tr style="width:10px;height:5px;">
                <td >
                    <button type="button" id="Button3" >Show All Data</button> 
                </td>
            </tr>
            <tr>
                <td colspan="3">
                     <div id="kgrdAllDetailChart">
                    </div>
                </td>
            </tr>
        </table>
    </div>
       
    </body> 
           </html> 
  </asp:Content>
