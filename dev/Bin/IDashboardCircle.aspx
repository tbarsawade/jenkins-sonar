<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false"
    CodeFile="IDashboardCircle.aspx.vb" Inherits="IDashboardCircle" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
    <style type="text/css">
        #divFilters
        {
            padding: 15px;
            background: #F7F7F7;
            min-height: 60px; /*width:1000px;*/
            box-sizing: border-box;
        }
        .k-grid-toolbar
        {
            text-align: right;
        }
        
        #mask
        {
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
        
        .loader
        {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }
        
        .k-grid-header .k-header
        {
            overflow: visible;
            white-space: normal;
        }
        #tabstrip h2
        {
            font-weight: lighter;
            font-size: 5em;
            line-height: 1;
            padding: 0 0 0 30px;
            margin: 0;
        }
        
        #tabstrip h2 span
        {
            background: none;
            padding-left: 5px;
            font-size: .3em;
            vertical-align: top;
        }
        
        #tabstrip p
        {
            margin: 0;
            padding: 0;
        }
    </style>
    <%--  <div id="example" style="height:300px; overflow-y:scroll;">
    <div class="demo-section k-content wide">
        <div id="chart" style="background: center no-repeat url('../content/shared/styles/world-map.png'); width:1000px;height:300px;  "></div>
    </div>
    </div>--%>
    <script type="text/jscript">

        // VEHICLE MILEAGE DASHBOARD.
        function GetData() {
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/Mileage",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    createChart(res);

                }
            });

        }

        function createChart(result) {

            $("#chart").kendoChart({
                title: {
                    text: "Vehicle Mileage Dashboard"
                },
                legend: {
                    position: "right"
                },
                theme: "Moonlight",
                seriesDefaults: {
                    type: "column",
                    stack: true
                },
                series: result.d.series,
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
                    categories: result.d.cata.categories,
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
                seriesClick: chartClick,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
            });
        }
        // View Details on chart click.
        function chartClick(e) {

            $("#kgridMileage").html('');
            $("#kgridOver").html('');
            var circlename = e.category;
            var vtype = e.series.name;
            $("#spnCircle").text(circlename);
            $("#spnVtype").text(vtype);

            var str = '{"Circle":"' + circlename + '","vtype":"' + vtype + '"}';
            var Name = "Circle: " + circlename;
            $("#kgridMileage").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivgrd").kendoWindow({
                width: "950px",
                height: "600px",
                title: "Vehicles Details",
                visible: false,
                modal: true
            });

            $("#popupdivgrd").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/getMileageDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridMileage").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridMileage").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridMileage", data, Columns, true, true, true, 550);

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


        // VEHICLE OVER SPEED DASHBOARD.

        function OverSpeed() {
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/OverSpeed",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    OverSpeedCom(res);

                }
            });

        }
        function OverSpeedCom(result) {

            $("#ChartSpeed").kendoChart({
                title: {
                    text: "Vehicle Over Speed Dashboard"
                },
                legend: {
                    position: "right"
                },
                theme: "Moonlight",
                seriesDefaults: {
                    type: "column",
                    stack: true
                },
                series: result.d.series,
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
                //  seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                categoryAxis: {
                    categories: result.d.cata.categories,
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
                seriesClick: OverSpeedClick,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
            });
        }

        function OverSpeedClick(e) {
            $("#kgridMileage").html('');
            $("#kgridOver").html('');
            var circlename = e.category;
            var vtype = e.series.name;
            $("#spnCircle").text(circlename);
            $("#spnVtype").text(vtype);

            var str = '{"Circle":"' + circlename + '","vtype":"' + vtype + '"}';
            var Name = "Circle: " + circlename;
            $("#kgridOver").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupOverSpeed").kendoWindow({
                width: "950px",
                height: "600px",
                title: "Vehicles Details",
                visible: false,
                modal: true
            });

            $("#popupOverSpeed").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/getOverSpeedDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridOver").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridOver").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridOver", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }



        // MANPOWER DASHBOARD
        function MANPOWER() {
            $("#chart-loading").html("<div class='loader' style='width:800px;' ></div>");
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/MANPOWER",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    MANPOWERCOM(res);

                }
            });

        }
        function MANPOWERCOM(result) {

            $("#ChartMan").kendoChart({
                title: {
                    text: "Manpower Compliance"
                },
                legend: {
                    position: "right"
                },

                seriesDefaults: {
                    type: "column",
                    stack: true
                },
                theme: "Moonlight",
                series: result.d.series,
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
                    categories: result.d.cata.categories,
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
                seriesClick: MANPOWERClick,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
            });
        }


        // View Details on chart click.
        function MANPOWERClick(e) {

            var circlename = e.category;
            var vtype = e.series.name;
            $("#spnCircle").text(circlename);
            $("#spnVtype").text(vtype);

            var str = '{"Circle":"' + circlename + '","vtype":"' + vtype + '"}';
            var Name = "Circle: " + circlename;
            $("#kgridMan").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupManpowerGrd").kendoWindow({
                width: "950px",
                height: "600px",
                modal: true,
                title: "Vehicles Details",
                visible: false
            });

            $("#popupManpowerGrd").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/getImapDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridMan").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridMan").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridMan", data, Columns, true, true, true, 550);

                    }
                }
            });
        }



        // DTS Vs VTS
        function DVTS() {
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/DVTS",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (res) {
                    DTS(res);

                }
            });

        }
        function DTS(result) {

            $("#ChartDVTS").kendoChart({
                title: {
                    text: "DTS Vs VTS"
                },
                legend: {
                    position: "right"
                },
                seriesDefaults: {
                    type: "column",
                    stack: true
                },
                theme: "Moonlight",
                series: result.d.series,
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
                    categories: result.d.cata.categories,
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
                seriesClick: DTSClick,
                tooltip: {
                    visible: true,
                    format: "{0}",
                    template: "#= series.name #: #= value #"
                }
            });
        }


        // View Details on chart click.
        function DTSClick(e) {

            var circlename = e.category;
            var vtype = e.series.name;
            $("#spnCircle").text(circlename);
            $("#spnVtype").text(vtype);

            var str = '{"Circle":"' + circlename + '","vtype":"' + vtype + '"}';
            var Name = "Circle: " + circlename;
            $("#kgrid").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivgrd").kendoWindow({
                width: "950px",
                height: "600px",
                modal: true,
                title: "Vehicles Details",
                visible: false
            });

            $("#popupdivgrd").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "IDashboardCircle.aspx/getVehicle",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrid").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrid").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgrid", data, Columns, true, true, true, 550);

                        //bindGridDetail($.parseJSON(data.d));
                    }
                }
            });
        }
        $(document).ready(function () {
            $("#tabstrip").kendoTabStrip({
                animation: {
                    open: {
                        effects: "fadeIn"
                    }
                }
            });
        });
        kendo.ui.progress($(".chart-loading"), true);
        $(document).ready(MANPOWER);
        $(document).ready(DVTS);
        $(document).ready(GetData);
        $(document).ready(OverSpeed);
        $(document).bind("kendo:skinChange", GetData);
        $(document).bind("kendo:skinChange", OverSpeed);
        //     $(document).bind("kendo:skinChange", MANPOWER);
        //       $(document).bind("kendo:skinChange", DVTS);

    </script>
    <div id="example" style="height: 1200px; margin-top: 0px">
        <div class="demo-section k-content">
            <div id="tabstrip">
                <ul>
                    <li class="k-state-active">Vehicle</li>
                    <li>Manpower/DTS </li>
                </ul>
                <div>
                    <table style="width: 1000px; height: 100%">
                        <tr style="width: 100%">
                            <td>
                                <div class="demo-section k-content wide">
                                    <div id="chart" style="background: center no-repeat url('../content/shared/styles/world-map.png');
                                        width: 100%; height: 250px;">
                                        <div class="chart-loading">
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr style="width: 100%">
                            <td>
                                <div class="demo-section k-content wide">
                                    <div id="ChartSpeed" style="background: center no-repeat url('../content/shared/styles/world-map.png');
                                        width: 100%; height: 250px;">
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table style="width: 1000px; height: 100%">
                        <tr style="width: 100%">
                            <td>
                                <div class="demo-section k-content wide">
                                    <div id="ChartMan" style="background: center no-repeat url('../content/shared/styles/world-map.png');
                                        width: 100%; height: 250px;">
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr style="width: 100%">
                            <td>
                                <div class="demo-section k-content wide">
                                    <div id="ChartDVTS" style="background: center no-repeat url('../content/shared/styles/world-map.png');
                                        width: 100%; height: 250px;">
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="popupdivgrd" style="display: none;">
        <table>
            <tr>
                <td>
                    Circle:<span id="spnCircle"></span>
                </td>
                <td>
                    Vehicle Type:<span id="spnVtype"></span>
                </td>
                <td>
                    <span id="spndtl"></span>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <div id="kgrid">
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
