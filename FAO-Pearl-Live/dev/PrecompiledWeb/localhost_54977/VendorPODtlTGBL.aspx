<%@ page title="" language="VB" masterpagefile="~/USRFullscreenBPM.master" autoeventwireup="false" inherits="VendorPODtlTGBL, App_Web_15ulzn3z" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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

   
    <div style="width:100%;height:20px; padding-bottom: 25px; padding-left: 10px;margin-top:10px;margin-bottom:10px; font: bold 17px 'verdana'; color: #fff; background-color: red;">
         PO Status Report
    </div>
       <div>
       <div id="kgrid" style="width: 98%">
        </div>
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
            background-color: rgba(0, 0, 0, 0.59);
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }
        .k-grid table tr:hover 
        
        {
            background: rgb(69, 167, 84);
            color: black;

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
        /*.link {
            color: #35945B;
        }

            .link:hover {
                font: 15px;
                color: green;
                background: yellow;
                border: solid 1px #2A4E77;
            }*/
         .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }
        .k-grid td
        {
             text-align:left;
        }

         .k-grid-toolbar
        {
            text-align:right;
        }
    </style>
    <script type="text/javascript">


      function GetReport() {
                  $.ajax({
                type: "POST",
                url: "VendorPODtlTGBL.aspx/GetData",
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
            var command = { command: { text: "PO Detail", click: chartClick }, title: " ", width: "100px" };
            Columns.push(command);
            bindGrid("kgrid", data, Columns, true, true, true, 550);
        }


        // View Details on chart click.
        function chartClick(e) {

            $("#kgridDtl").html('');
            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var Pnum = dataItem.PO;
//            var vtype = e.series.name;
//            $("#spnCircle").text(circlename);
//            $("#spnVtype").text(vtype);

       var str = '{"Pnum":"' + Pnum + '"}';
//            var Name = "Circle: " + circlename;
            $("#kgridDtl").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupdivgrd").kendoWindow({
                width: "950px",
                height: "600px",
                title: "PO Invoices",
                visible: false,
                modal: true
            });

            $("#popupdivgrd").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "VendorPODtlTGBL.aspx/getDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str ,
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
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgridDtl", data, Columns, true, true, true, 550);
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





