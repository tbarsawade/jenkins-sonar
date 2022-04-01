<%@ page title="" language="VB" masterpagefile="~/USRFullscreenBPM.master" autoeventwireup="false" inherits="VendorPODtl, App_Web_ws3kahym" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>

    <div class="form">
        <div class="doc_header">
            <asp:Label ID="lblCaption" runat="server" Text="PO Status Report"></asp:Label>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 mg" style="text-align: center;">
                <div id="kgrid" style="width:100%;">
                </div>
            </div>
        </div>
    </div>
    </div>
    <%-- <div style="width: 100%; height: 20px; padding-bottom: 25px; padding-left: 10px; margin-top: 10px; margin-bottom: 10px; font: bold 17px 'verdana'; color: #fff; background-color: red;">
        PO Status Report
    </div>--%>
    <div>
    </div>
    <div>
        <div id="mask">
            <div id="loader">
                <img src="images/preloader22.gif" alt="" />
            </div>
        </div>
    </div>
    <div id="popupdivgrd" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td colspan="3">
                    <div id="kgridDtl" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <style type="text/css">
        .water {
            font-family: Tahoma, Arial, sans-serif;
            font-style: italic;
            color: gray;
        }


        .style2 {
            width: 100%;
            height: 73px;
        }

        .loader {
            background: url("images/preloader22.gif") no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }
    </style>
    <style type="text/css">
        #mask {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #35945B;
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }

        .k-grid table tr:hover {
            background: #2367a6;
            color: white;
            
        }


        #loader {
            position: absolute;
            left: 50%;
            top: 50%;
            width: 200px;
            height: 70px;
            background: none;
            margin: -100px 0 0 -100px;
            display: none;
            padding-top: 25px;
        }
        .link {
            color: #2367a6;
        }

            .link:hover {
                font: 15px;
                color: #2367a6;
                background: yellow;
                border: solid 1px #2A4E77;
            }
        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }

        .k-grid td {
            text-align: left;
        }

        .k-grid-toolbar {
            text-align: right;
        }
    </style>
    <script type="text/javascript">


        function GetReport() {
            $.ajax({
                type: "POST",
                url: "VendorPODtl.aspx/GetData",
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }

        function BindReport(result) {

            if (result.d.Success == false) {
                $("#kgrid").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                return;
            }
            var Columns = result.d.Column;
            var data = JSON.parse(result.d.Data);
            var command = { command: { text: "View Invoices", click: chartClick }, title: " ", width: "110px",align:"left" };
            var command1 = { command: { text: "View PO Details", click: DetailHandler }, title: " ", width: "130px" };
            Columns.push(command);
            Columns.push(command1);
            bindGrid("kgrid", data, Columns, true, true, true, 550);
        }

        function onDataBound(e) {
            var grid = $("#kgrid").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var DOCID = gridData[i].DocID;
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
            }
        }
        function onDataBound1(e) {
            var grid = $("#kgridDtl").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var DOCID = gridData[i].DocID;
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
            }
        }

        function DetailHandler(evt) {
             var EID = '<%= Session("EID")%>';
            var Docversion = '<%=Session("Docversion")%>';
            //dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var docid = evt.data.DOCID;
            //  window.open('DocDetail.aspx?DOCID=' + docid + '');
            var type = evt.data.commandName;
            var name = evt.target;
            var evnt = name.className;
            if (docid !== undefined) {
                if ((evnt == 'k-button k-button-icontext k-grid-ViewPODetails') && (docid != '')) {
                    if (Docversion == "New") {
                        window.open('NewDocDetail.aspx?DOCID=' + docid + '', '_blank');
                    }
                    else {
                        window.open('DocDetail.aspx?DOCID=' + docid + '', '_blank');
                    }
                }
                else if ((evnt == 'k-button k-button-icontext k-grid-ViewInvoiceDetails') && (docid != ''))
               if (Docversion == "New") {
                        window.open('NewDocDetail.aspx?DOCID=' + docid + '', '_blank');
                    }
                    else {
                        window.open('DocDetail.aspx?DOCID=' + docid + '', '_blank');
                    }
           }
            //OpenWindow('DocDetail.aspx?DOCID=' + docid + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }


        // View Details on chart click.
        function chartClick(e) {

            $("#kgridDtl").html('');
            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var tr = $("#kgrid").find("[data-uid='" + dataItem.uid + "']");
            var cell = $("td:nth-child(1)", tr);
            var Pnum = cell[0].textContent;
            //            var vtype = e.series.name;
            //            $("#spnCircle").text(circlename);
            //            $("#spnVtype").text(vtype);

            var str = '{"Pnum":"' + Pnum + '"}';
            //            var Name = "Circle: " + circlename;
            $("#kgridDtl").html("<div class='loader' style='width:1200px;' ></div>");
            $("#popupdivgrd").kendoWindow({
                width: "1200px",
                height: "580px",
                title: "PO Invoices",
                visible: false,
                modal: true
            });

            $("#popupdivgrd").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "VendorPODtl.aspx/getDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgridDtl").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgridDtl").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var command1 = { command: { text: "View Invoice Details", click: DetailHandler }, title: " ", width: "150px" };
                        Columns.push(command1);
                        var data = $.parseJSON(result.d.Data);
                        bindGrid1("kgridDtl", data, Columns, true, true, true, 550);
                    }
                }
            });
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
                dataBound: onDataBound,
                columnMenu: true,
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
                    data: Data1,
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

        $(document).ajaxStart(
            function () {
                $("#mask").css("display", "block");
                $("#loader").css("display", "block");
            }
            );
        $(document).ajaxComplete(function () {
            $("#mask").css("display", "none"); $("#loader").css("display", "none");
        });

        $(document).ready(function () {
            GetReport();
        });


    </script>

</asp:Content>





