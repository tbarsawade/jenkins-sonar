<%@ page title="" language="VB" autoeventwireup="false" masterpagefile="~/usrFullScreenBPM.master" inherits="PearlDashboard, App_Web_gsdfcjye" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="css/StyleSheet.css" rel="stylesheet" type="stylesheet" />
    <link href="css/style.css" rel="stylesheet" type="stylesheet" />
    <link href="css/font-awesome.min.css" rel="stylesheet" />
    <link href="css/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Montserrat" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>

    <style>
        body {
            /*font-family: Metric;*/
            font-family: 'Helvetica', sans-serif;
        }

        .wrapper_container {
            width: 100%;
        }

        .nopadding {
        }

            .nopadding > [class*="col-"] {
                padding-left: 0px;
                padding-right: 0px;
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
            border:1px solid #E96125;
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
            padding: 10px 0;
            color: #fff;
            display: block;
            z-index: 10;
            /*background: rgba(0,0,0,0.1);*/
            background: #E96125;
            text-decoration: none;
            width: 100%;
            /*background-color:gray;*/
            border-radius: 10px 10px 0px 0px;
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
            /*display: none;*/
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

        .container-fluid {
            padding-right: 15px;
            padding-left: 15px;
            margin-right: auto;
            margin-left: auto;
        }
        .main_menu {
            margin: 8px 0px 3px;
        }
      
    </style>


    <script type="text/javascript">
        // PR PO BREAKUP
        function GetDataPOBreakup() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/POBreakup",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChart(res);

                }
            });

        }
        function createChart(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdGrp").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },
                chartArea: {
                    background: ""
                },
                legend: {
                    visible: false,
                    position: "bottom",
                    template: "#= category #"
                },
                seriesDefaults: {
                    labels: {
                        template: "#= category # \n (#= value #)",
                        align: "circle",
                       // position: "insideEnd",
                        visible: true,
                        position: "right",                       
                       // background: "transparent",
                        color: "#1F6B08"
                    }
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{
                    type: "donut",
                    field: "value",
                    categoryField: "category"
                }],
                seriesColors: ['#9de219', '#004d38'],
                seriesClick: dtlPRPOBREAKUP,
                tooltip: {
                    visible: true,
                    template: "#= category #: #= value #"
                    // template: "Circle:  #= category # <br/>Total Count: #= value#/" + ttl + "-(#= kendo.format('{0:P}', percentage)#)"
                }
            });
        }
        function dtlPRPOBREAKUP(e) {
            $("#kgridPRPOBRKUP").html('');
            var Type = e.category;
            $("#kgridPRPOBRKUP").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivPRPOBRKUP").kendoWindow({
                width: "950px",
                height: "580px",
                title: "PR PO BREAKUP",
                visible: false,
                modal: true
            });
            var str = '{"Type":"' + Type + '"}';
            $("#popupdivPRPOBRKUP").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getdtlPRPOBREAKUP",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridPRPOBRKUP").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridPRPOBRKUP").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridPRPOBRKUP", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        
        // TOP CATAGORIES
        function GetDataTopCategory() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataTopCategory",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartTopCategory(res);

                }
            });

        }
        function createChartTopCategory(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdGrpTopCat").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },
                chartArea: {
                    background: ""
                },
                legend: {
                    visible: false,
                    position: "bottom"
                },
                seriesDefaults: {
                    labels: {
                        visible: true,
                        background: "transparent",
                        color: "white",
                        format: "N0",
                        template: "#= category #: #= value #"
                    },
                    dynamicSlope: false,
                    dynamicHeight: false
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{
                    
                    type: "funnel",
                    field: "value",
                    categoryField: "category"
                   
                }],
                seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                //seriesClick: dtlTOPCat,
                tooltip: {
                    visible: true,
                    template: "#= category #: #= value #"
                }
            });
        }
       
                
        // TOP SUPPLIERS

        function GetDataTopSupplier() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataTopSupplier",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartTopSupplier(res);

                }
            });

        }
        function createChartTopSupplier(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdGrpTopSup").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },
                chartArea: {
                    background: ""
                },
                legend: {
                    visible: false,
                    position: "bottom"
                },
                seriesDefaults: {
                    labels: {
                        visible: true,
                        background: "transparent",
                        color: "white",
                        format: "N0",
                        template: "#= category #: #= value #"
                    },
                    dynamicSlope: false,
                    dynamicHeight: false
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{

                    type: "funnel",
                    field: "value",
                    categoryField: "category"

                }],
                seriesColors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
                seriesClick: dtlTOPSupplier,
                tooltip: {
                    visible: true,
                    template: "#= category #: #= value #"
                }
            });
        }
        function dtlTOPSupplier(e) {
            $("#kgridTopSupplier").html('');
            var Vname = e.category;
            $("#kgridTopSupplier").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivTopSupplier").kendoWindow({
                width: "950px",
                height: "580px",
                title: "TOP SUPPLIERS",
                visible: false,
                modal: true
            });
            var str = '{"Vname":"' + Vname + '"}';
            $("#popupdivTopSupplier").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getTopSupplierdtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridTopSupplier").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridTopSupplier").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridTopSupplier", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        // TOTAL PR TO PO CLOSURE

        function GetDataPRPOClosure() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataPRPOClosure",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartPRPOClosure(res);

                }
            });

        }
        function createChartPRPOClosure(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdPRPOClouser").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },
                chartArea: {
                    background: ""
                },
                legend: {
                    visible: true
                },
                seriesDefaults: {
                    labels: {
                        visible: true,
                        font: "11px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        padding: 3,
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        template: " #= value # days"

                    },
                    dynamicSlope: false,
                    dynamicHeight: false
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{

                    type: "column",
                    field: "value",
                    categoryField: "category"

                }],

                seriesColors: ['rgb(51, 102, 204)'],
                seriesClick: dtlTotalPRTOPOCLOSURE,
                tooltip: {
                    visible: true,
                    template: "#= category #"
                }
                              
            });
        }
        function dtlTotalPRTOPOCLOSURE(e) {
            $("#kgrdPRTOPOCLOSURE").html('');
            var Month = e.category;
             $("#kgrdPRTOPOCLOSURE").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivPRTOPOCLOSURE").kendoWindow({
                width: "950px",
                height: "580px",
                title: "PR TO PO CLOSURE",
                visible: false,
                modal: true
            });
            var str = '{"Month":"' + Month + '"}';
            $("#popupdivPRTOPOCLOSURE").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getPRTOPOCLOSUREDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdPRTOPOCLOSURE").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdPRTOPOCLOSURE").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgrdPRTOPOCLOSURE", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        
        // TOTAL INVOICE PAYMENT CYCLE

        function GetDataInvPaymentCycle() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataInvPaymentCycle",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartInvPaymentCycle(res);

                }
            });

        }
        function createChartInvPaymentCycle(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdInvPaymentCycle").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },
                chartArea: {
                    background: ""
                },
                legend: {
                    visible: true
                },
                seriesDefaults: {
                    labels: {
                        visible: true,
                        font: "11px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        padding: 3,
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        template: " #= value # days"
                    },
                    dynamicSlope: false,
                    dynamicHeight: false
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{

                    type: "column",
                    field: "value",
                    categoryField: "category"

                }],
                seriesColors: ['#e83e8c'],
                seriesClick: dtlTotalInvoicePayemnt,
                tooltip: {
                    visible: true,
                    template: "#= category #"
                }
            });
        }
        function dtlTotalInvoicePayemnt(e) {
            $("#kgridInvPaymentCycle").html('');
            var Month = e.category;
            $("#kgridInvPaymentCycle").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivInvPaymentCycle").kendoWindow({
                width: "950px",
                height: "580px",
                title: "INVOICE PAYMENT CYCLE",
                visible: false,
                modal: true
            });
            var str = '{"Month":"' + Month + '"}';
            $("#popupdivInvPaymentCycle").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getInvoicePayemntDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridInvPaymentCycle").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridInvPaymentCycle").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridInvPaymentCycle", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        
        // TOTAL INVOICE

        function GetDataTotalInvoice() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataTotalInvoice",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartTotalInvoice(res);

                }
            });

        }
        function createChartTotalInvoice(data1) {
            
            $("#kgrdGrpTotalInvoice").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },

                legend: {
                    visible: true,
                    position: "bottom"

                },
                seriesDefaults: {
                    type: "bar",
                    stack: true,
                        labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        template: " #=  value #",
                        color:"white"
                    },



                            },
                series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                    },
                    line: {
                        visible: false
                    },

                    axisCrossingValue: 0
                },

                seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                categoryAxis: {
                    categories: data1.d.categoryAxis,
                    line: {
                        visible: false
                    },
                    labels: {
                        rotation: 0,
                        padding: { top: 0 }
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
                seriesClick: dtlTotalInvoice,
                    tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
              });
        }
        function dtlTotalInvoice(e) {
            $("#kgrdTotalInv").html('');
            var Month = e.category;
            var Status = e.series.name;
            $("#kgrdTotalInv").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivTotalInv").kendoWindow({
                width: "950px",
                height: "580px",
                title: "INOVICES",
                visible: false,
                modal: true
            });
            var str = '{"Month":"' + Month + '","Status":"' + Status + '"}';
            $("#popupdivTotalInv").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getTotalInvoice",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdTotalInv").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdTotalInv").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgrdTotalInv", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        
        // TOTAL PURCHASE REQUEST

        function GetDataTotalPurchaseRequest() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataTotalPurchaseRequest",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartTotalPurchaseRequest(res);

                }
            });

        }
        function createChartTotalPurchaseRequest(data1) {

            $("#kgrdGrpTotalPurchaseRequest").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },

                legend: {
                    visible: true,
                    position: "bottom"

                },
                seriesDefaults: {
                    type: "bar",
                    stack: true,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        template: " #=  value #",
                        color: "white"
                    },



                },
                series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                    },
                    line: {
                        visible: false
                    },

                    axisCrossingValue: 0
                },

                seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                categoryAxis: {
                    categories: data1.d.categoryAxis,
                    line: {
                        visible: false
                    },
                    labels: {
                        rotation: 0,
                        padding: { top: 0 }
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
                seriesClick: dtlTotalPurchaseRequest,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
            });
        }
        function dtlTotalPurchaseRequest(e) {
            $("#kgrdTotalPR").html('');
            var Month = e.category;
            var Status = e.series.name;
            $("#kgrdTotalPR").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivTotalPR").kendoWindow({
                width: "950px",
                height: "580px",
                title: "PURCHASE REQUEST",
                visible: false,
                modal: true
            });
            var str = '{"Month":"' + Month + '","Status":"' + Status + '"}';
            $("#popupdivTotalPR").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getTotalPRDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdTotalPR").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdTotalPR").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgrdTotalPR", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }

        // TPTAL PURCHASE ORDER

        function GetDataTotalPurchaseOrder() {
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/GetDataTotalPurchaseOrder",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChartTotalPurchaseOrder(res);

                }
            });

        }
        function createChartTotalPurchaseOrder(data1) {

            $("#kgrpTotalPurchaseOrder").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },

                legend: {
                    visible: true,
                    position: "bottom"

                },
                seriesDefaults: {
                    type: "bar",
                    stack: true,
                    labels: {
                        visible: true,
                        font: "12px Segoe UI, Arial ",
                        margin: { top: 0, left: 0 },
                        border: { color: "black", width: 0 },
                        background: "transparent",
                        format: "N0",
                        position: "center",
                        template: " #=  value #",
                        color: "white"
                    },



                },
                series: data1.d.series,
                valueAxis: {
                    labels: {
                        format: "{0}"
                    },
                    line: {
                        visible: false
                    },

                    axisCrossingValue: 0
                },

                seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                categoryAxis: {
                    categories: data1.d.categoryAxis,
                    line: {
                        visible: false
                    },
                    labels: {
                        rotation: 0,
                        padding: { top: 0 }
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
                seriesClick: dtlTotalPurchaseOrder,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
            });
        }
        function dtlTotalPurchaseOrder(e) {
            $("#kgrdTotalPO").html('');
            var Month = e.category;
            var Status = e.series.name;
            $("#kgrdTotalPO").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivTotalPO").kendoWindow({
                width: "950px",
                height: "580px",
                title: "PURCHASE ORDER",
                visible: false,
                modal: true
            });
            var str = '{"Month":"' + Month + '","Status":"' + Status + '"}';
            $("#popupdivTotalPO").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "PearlDashboard.aspx/getTotalPODtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdTotalPO").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdTotalPO").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgrdTotalPO", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }


        var gridDiv;
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
                //dataBound: onDataBound,
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


        $(document).ready(function () {
            GetDataPOBreakup();
            GetDataTopCategory();
            GetDataTopSupplier();
            GetDataPRPOClosure();
            GetDataInvPaymentCycle();
            GetDataTotalInvoice();
            GetDataTotalPurchaseRequest();
            GetDataTotalPurchaseOrder();

            $("#monthpicker").kendoDatePicker({
                // defines the start view
                start: "year",

                // defines when the calendar should return date
                depth: "year",

                // display month and year in the input
                format: "MMMM yyyy",

                // specifies that DateInput is used for masking the input element
                dateInput: true
            });

        });

    </script>


    

    <%--  <input id="monthpicker" value="February 2018" title="monthpicker" style="width: 20%" />--%>
    
     <div class="inner_div">
         <div style="background-color:white; text-align:left; font-family:sans-serif; font-size:14px; padding-top:8px;padding-left:5px;"> 
            <b>OPERATIONAL DASHBOARD FOR THE MONTH OF: FEBRUARY</b>&nbsp; 
            <span><input id="monthpicker" value="February 2018" title="monthpicker" style="width:10px; height:4px; margin-top:-8px;" /></span>
             </div>
<%--         <div style="background-color: white; text-align: left; font-family: sans-serif; font-size: 16px; padding-top: 2px; padding-left: 5PX">
             <table>
                 <tr>
                     <td style="width:1020px">

                         <b>OPERATIONAL DASHBOARD FOR THE MONTH OF: FEBRUARY</b>

                     </td>
                     <td>
                         <input id="monthpicker" value="February 2018" title="monthpicker" style="width: 100%; height:10px" />
                       
                     </td>
                     <td><asp:ImageButton ID="btnimmm" ToolTip="Import" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/import.png"/></td>
                 </tr>
             </table>

         </div>--%>

         <div class="container-fluid" style="background-color: white;">
            <div class="row nopadding">
                <div class="col-md-3">
                    <div class="small-box bg-purples">
                        <a href="#" class="small-box-footer">PR to PO Closure Cycle</a>
                        <div class="inner">
                            <div id="kgrdPRPOClouser" style="height: 190px; width: 300px;">
                            </div>

                            <div class="cl"></div>
                        </div>
                    </div>
                </div>


                 <div class="col-md-3">
                    <div class="small-box bg-purples">
                        <a href="#" class="small-box-footer">Purchase Request(Count)</a>
                         <div class="inner">
                            <div id="kgrdGrpTotalPurchaseRequest" style="height: 190px; width: 300px;">
                            </div>
                              <%-- <div style="text-align:center"><asp:Label runat="server" Text="2018-200"></asp:Label></div>--%>
                            <div class="cl"></div>
                        </div>


                    </div>
                </div>
                                

                <div class="col-md-3">
                    <div class="small-box .bg-purples">
                        <a href="#" class="small-box-footer">PO-Non Po Breakup</a>
                        <div class="inner">
                            <div id="kgrdGrp" class="donut-chart" style="height: 215px; width: 380px; margin-top:-20px; margin-left:-30px;">
                            </div>
                              <div class="inner-content"></div>


                            <div class="cl"></div>
                        </div>


                    </div>
                </div>



                <div class="col-md-3">
                    <div class="small-box .bg-purples">
                        <a href="#" class="small-box-footer">Purchase Order (Count)</a>
                        <div class="inner">
                            <div id="kgrpTotalPurchaseOrder" style="height: 200px; width: 300px;">
                            </div>
                            <div class="cl"></div>
                        </div>


                    </div>
                </div>


                <div class="col-md-3">
                    <div class="small-box bg-purples">
                        <a href="#" class="small-box-footer">Invoice (Count)</a>
                         <div class="inner">
                            <div id="kgrdGrpTotalInvoice" style="height: 190px; width: 300px;">
                            </div>
                              <%-- <div style="text-align:center"><asp:Label runat="server" Text="2018-200"></asp:Label></div>--%>
                            <div class="cl"></div>
                        </div>


                    </div>
                </div>

                <div class="col-md-3">
                    <div class="small-box bg-purples ">
                        <a href="#" class="small-box-footer">Invoice Payment Cycle</a>
                        <div class="inner">
                            <div class="inner">
                            <div id="kgrdInvPaymentCycle" style="height: 190px; width: 300px;">
                              
                            </div>
                              
                                  
                        </div>
                            <div class="cl"></div>
                        </div>
                    </div>
                </div>
                

                <div class="col-md-3">
                    <div class="small-box bg-purples">
                        <a href="#" class="small-box-footer">Spend by category (&#8377;)</a>
                        <div class="inner">
                            <div id="kgrdGrpTopCat" style="height: 190px; width: 300px;">
                            </div>

                            <div class="cl"></div>
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="small-box bg-purples">
                        <a href="#" class="small-box-footer">Spend by supplier (&#8377;)</a>
                      <div class="inner">
                            <div id="kgrdGrpTopSup" style="height: 190px; width: 300px;">
                            </div>

                            <div class="cl"></div>
                        </div>
                    </div>
                </div>
            </div>
            <!--row-->
            <div class="cl"></div>
        </div>
    </div>
        
     <div id="popupdivPRTOPOCLOSURE" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgrdPRTOPOCLOSURE" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    
     <div id="popupdivTotalPR" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgrdTotalPR" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

     <div id="popupdivTotalPO" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgrdTotalPO" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

     <div id="popupdivTotalInv" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgrdTotalInv" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

     <div id="popupdivInvPaymentCycle" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgridInvPaymentCycle" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div id="popupdivTopSupplier" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgridTopSupplier" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

     <div id="popupdivPRPOBRKUP" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgridPRPOBRKUP" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>


</asp:Content>
