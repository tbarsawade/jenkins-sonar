<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="THome, App_Web_fdh01zus" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <script type="text/javascript">

        var new_window;
        function OpenWindow(url) {
            new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }
        //var win = window.open("Child.aspx", "thePopUp", "");
        function childClose() {
            //if (new_window.closed) {

            var DocumentType = $("#ContentPlaceHolder1_ddldocType").val();
            BindNeedToAct(DocumentType);
            GetGridMyReq(DocumentType);
            GetGridHistory(DocumentType);
            //$("#ContentPlaceHolder1_btnpendinggrdcl").click();

            //}
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
    </script>
    <style type="text/css">
        .loader {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .loader1 {
            background-image: url("images/loading.gif");
            background-repeat: no-repeat;
            background-position: center center;
            position: relative;
            top: calc(50% - 16px);
            display: block;
            height: 50px;
        }

        .tdhover {
            background: #c6def5;
        }

            .tdhover:hover {
                color: #3585f3;
                text-underline-position: below;
            }

        .right-button {
            text-align: right;
        }

            .right-button .btn-default {
                background-color: #E96125;
                color: #fff;
                border: none;
                font-weight: bold;
                font-size: 12px;
            }

                .right-button .btn-default:hover, .btn-default:focus, .btn-default:active, .btn-default.active {
                    border: none;
                }

        #wrap {
            width: 98%;
            max-width: 98%;
            padding: 0px !important;
            margin: 0 auto;
            box-sizing: border-box;
            -moz-box-sizing: border-box;
            -webkit-box-sizing: border-box;
        }

        .k-grid table tr:hover {
            background: #E96125;
            color: black;
        }

        .k-window div.k-window-content {
            overflow: hidden;
        }

        .k-grid .k-state-selected {
            background: #E96125;
        }

        .yellow {
            background-color: red;
        }

        a.clickable {
            cursor: pointer;
            cursor: hand;
        }

        /*headers*/

        .k-grid th.k-header, .k-grid-header {
            font-weight: bold;
        }

            .k-grid th.k-header, .k-grid th.k-header .k-link {
                color: #E96125;
            }

        .Cursor {
            cursor: pointer;
            cursor: hand;
        }
    </style>
    <div style="width: 1333px; margin: 0px 0px 0px 15px; display: block;">
        <div class="demo-section Thomedemo-section k-header">
            <table width="100%">
                <tr>
                    <td style="width: 45%">
                        <asp:ImageButton runat="server" ImageUrl="images/Tktchart.png" ID="mapbtn" CssClass="imgButton"
                            alt="Summary Chart" OnClientClick="GetMap()" />
                        <%--      <input type="image" id="mapbtn1" src="images/Tktchart.png" alt="Summary Chart" onclick="return GetMap()" />--%>
                    </td>
                    <td style="width: 55%">
                        <p id="p1" style="font-size: 21px;">
                            Status Wise Ticket Chart
                        </p>
                        <%--   <asp:Label ID="Label1" runat="server" Text="Status Wise Ticket Chart" Font-Bold="true"
                    Font-Size="Large"></asp:Label>--%>
                    </td>
                </tr>
            </table>
        </div>
        <div style="width: 100%;">
            <div class="demo-section k-header">
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <div id="dvloader1" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kSuspendedStatus" style="display: block; overflow-x: auto; width: 300px">
                            </div>
                            <%-- <a href="#" class="clickable">--%>
                            <div id="kgrdDStatus" style="display: block; overflow-x: auto; width: 300px">
                            </div>
                            <%--</a>--%>
                            <div id="NoRecord2" style="display: none; text-align: center; min-height: 80px;">
                                <span style="color: red; position: relative; top: 30px;">No Data found</span>
                            </div>
                        </td>
                        <td>
                            <div id="example" class="graphchart">
                                <div id="dvloader2" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                    <input type="image" src="../images/preloader22.gif" />
                                </div>
                                <div id="kgrdDtl" style="display: block; width: 1000px;  height: 500px">
                                </div>
                                <div id="NoRecord3" style="display: none; text-align: center; height: 500px; width: 1000px; border: 4px solid #ccc; background-color: white;">
                                    <%--<span style="color: #E96125; position: relative; top: 30px;">No Data found</span>--%>
                                    <img src="images/no-record-found.png" class="img-responsive" alt="No record found." style="padding: 50px 30px 50px 100px" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="dvloader" style="position: absolute; display: none; z-index: 1; left: 50%; top: 50%; z-index: 10001">
            <input type="image" id="Imageprog" src="images/prg.gif" style="height: 25px;" />
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            GetSuspendedStatus()
            $("#kSuspendedStatus tbody .k-state-selected").css({
                "background-image": "none",
                "background-color": "blue"
            });

            GetDStatus()
            $("#kgrdDStatus tbody .k-state-selected").css({
                "background-image": "none",
                "background-color": "blue"
            });

        });

        function GetSuspendedStatus() {
            $.ajax({
                type: "POST",
                url: "THome.aspx/GetSuspendedStatus",
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindSuspendedStatus,
                error: function (err) { alert('err'); },
                falure: function (response) {
                }
            });

        }
        function GetDStatus() {
            $.ajax({
                type: "POST",
                url: "THome.aspx/GetDStatus",
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }
        function BindSuspendedStatus(result) {
            if (result.d.Success == false) {
                if (result.d.Message != "INVISIBLE") {
                    $("#kSuspendedStatus").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                }
                else {
                    $("#kSuspendedStatus").hide();
                }
                return;
            }
            var Columns = result.d.Column;
            var data = JSON.parse(result.d.Data);
            bindSuspendedGrid("kSuspendedStatus", data, Columns, true, true, true, 50);
        }
        function BindReport(result) {

            if (result.d.Success == false) {
                $("#kgrdDStatus").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                return;
            }
            var Columns = result.d.Column;
            var data = JSON.parse(result.d.Data);
            bindGrid("kgrdDStatus", data, Columns, true, true, true, 500);

        }

        var susgridDiv;
        function bindSuspendedGrid(susgridDiv, SuspendedData1, Suspendedcolumns, Suspendedpageable, Suspendedfilterable, Suspendedsortable, Suspendedheight) {
            $("#" + susgridDiv).html('');
            var g = $("#" + susgridDiv).data("kendoGrid");
            if (g != null || g != undefined) {
                g.destroy();
                $("#" + susgridDiv).html('');
            }
            susgridDiv = $("#" + susgridDiv).kendoGrid({
                dataSource: {
                    pageSize: 20,
                    data: SuspendedData1
                },
                scrollable: {
                    virtual: true
                },

                columns: Suspendedcolumns,
                width: 100,
                scrollable: true,
                reorderable: true,
                columnMenu: true,
                //detailInit: detailInit,
                dataBound: onSuspendedDataBound,
                sortable: true,
                allowCopy: true,
                resizable: true,
                height: Suspendedheight
            });
        }

        function onSuspendedDataBound(e) {
            var grid = $("#kSuspendedStatus").data("kendoGrid");
            $('#kSuspendedStatus').find('td').addClass("Cursor");
            $('#kSuspendedStatus').find('P').parent('td').removeClass("Cursor");
            $(grid.tbody).on("click", "tr", function (e) {
                var row = $(grid).closest("tr");
                $(row).addClass("yellow");
                var rowIdx = $("tr", grid.tbody).index(row);
                var colIdx = $("td", row).index(grid);
                dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
                document.getElementById("p1").innerHTML = dataItem.Suspended_Tickets;
                Binddetailgrid(dataItem);
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
                    group: [{
                        field: "Organizations"
                    }],
                    pageSize: 20,
                    data: Data1
                },
                scrollable: {
                    virtual: true
                },

                columns: [{ field: "Views" }],
                width: 100,
                scrollable: true,
                reorderable: true,
                columnMenu: true,
                //detailInit: detailInit,
                dataBound: onDataBound,
                sortable: true,
                allowCopy: true,
                resizable: true,
                height: height
            });

        }
        function detailInit(e) {
            alert(1);
        }
        function onDataBound(e) {
            var grid = $("#kgrdDStatus").data("kendoGrid");
            $('#kgrdDStatus').find('td').addClass("Cursor");
            $('#kgrdDStatus').find('P').parent('td').removeClass("Cursor");
            $(grid.tbody).on("click", "tr:Not('.k-grouping-row')", function (e) {
                var row = $(grid).closest("tr");
                $(row).addClass("yellow");
                var rowIdx = $("tr", grid.tbody).index(row);
                var colIdx = $("td", row).index(grid);
                dataItem = grid.dataItem($(e.currentTarget).closest("tr"));
                document.getElementById("p1").innerHTML = dataItem.Views;
                Binddetailgrid(dataItem);
            });
        }
        function Binddetailgrid(result) {
            $("#NoRecord3").hide();
            $("#dvloader").show();
            var str = '{ "reportname": "' + result.Views + '","organizationName":"' + result.Organizations + '"}';
            $.ajax({
                type: "POST",
                url: "THome.aspx/GetDetail",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindDtlReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }
        function BindDtlReport(result) {
            var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Details", width: 60 };

            if (result.d.Success == false) {
                $("#dvloader").hide();
                var Columns = result.d.Column;
                Columns.splice(0, 0, CommanObj);
                var data = result.d.Data;
                bindGridDesc("kgrdDtl", data, Columns, true, true, true, 550);
                $("#kgrdDtl").hide();
                $("#NoRecord3").show();
                return;
            }
            $("#NoRecord3").hide();
            $("#kgrdDtl").show();
            var Columns = result.d.Column;
            Columns.splice(0, 0, CommanObj);
            var data = JSON.parse(result.d.Data);
            bindGridDesc("kgrdDtl", data, Columns, true, true, true, 550);
            $("#dvloader").hide();
        }

        var gridDiv;
        function bindGridDesc(gridDiv, Data1, columns, pageable, filterable, sortable, height) {

            $("#" + gridDiv).html('');

            var g = $("#" + gridDiv).data("kendoGrid");

            if (g != null || g != undefined) {
                g.destroy();
                $("#" + gridDiv).html('');
            }

            gridDiv = $("#" + gridDiv).kendoGrid({
                dataSource: {
                    pageSize: 15,
                    data: Data1
                },
                scrollable: {
                    virtual: true
                },

                columns: columns,
                scrollable: true,
                reorderable: true,
                columnMenu: true,
                //dataBound: onDataBound,
                groupable: true,
                sortable: true,
                noRecords: true,
                sortable: true,
                allowCopy: true,
                resizable: true,
                pageable: true,
                filterable: true,
                pageSize: 15,
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

        var DetailHandler = function DetailHandler(e) {
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            OpenWindow('TicketDocDetail.aspx?DOCID=' + dataItem.TID + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }
        function OpenWindow(url) {
            window.location.replace(url);
            //    window.open(url, 'Google', 'dialogWidth: 400px; dialogHeight: 300px; align: center; resizable: no; scroll: no; status: no;toolbar: no; width: 100px; height: 100px;help: no');
            //   window.open(url, '_blank');
            //  var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            // return false;
        }



        function GetData() {
            $.ajax({
                type: "POST",
                url: "Thome.aspx/GetJSON",
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
            var chartdata = JSON.parse(data1.d);
            var IsValid = false;
            for (var i = 0; i < chartdata.length - 1; i++) {
                if (chartdata[i]["value"] != 0) {
                    $("#NoRecord3").hide();
                    IsValid = true;
                }
            }
            if (IsValid) {
                $("#kgrdDtl").show();
                $("#NoRecord3").hide();
                $("#kgrdDtl").kendoChart({
                    title: {
                        position: "bottom"
                        //text: "DOC Count"
                    },
                    legend: {
                        visible: false
                    },
                    seriesDefaults: {
                        labels: {
                            template: "#= category #:(#= kendo.format('{0}', value)#)",
                            visible: true,
                            background: "transparent"
                        }
                    },
                    dataSource: {
                        data: JSON.parse(data1.d)
                    },
                    series: [{
                        type: "pie",
                        field: "value",
                        categoryField: "category"
                    }],
                    seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                    tooltip: {
                        visible: true
                        // template: "Circle:  #= category # <br/>Total Count: #= value#/" + ttl + "-(#= kendo.format('{0:P}', percentage)#)"
                    },
                    height: 1200
                });
            }
            else {
                $("#kgrdDtl").hide();
                $("#NoRecord3").show();
            }
        }
        $(document).ready(GetData);
        function GetMap() {
            $(document).ready()
        }

    </script>
</asp:Content>
