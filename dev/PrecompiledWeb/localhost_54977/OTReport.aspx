<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="OTReport, App_Web_4somiesn" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <%--<script src="kendu/js/jquery.min.js"></script>--%>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <style type="text/css">
        #divFilters
        {
            padding: 15px;
            background: #F7F7F7;
            min-height: 60px; /*width:1000px;*/
            box-sizing: border-box;
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
        
        #loader
        {
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
        
        .k-grid-header .k-header
        {
            overflow: visible;
            white-space: normal;
        }
        .textbox
        {
            border: 1px solid #848484;
            -webkit-border-radius: 30px;
            -moz-border-radius: 30px;
            border-radius: 30px;
            outline: 0;
            height: 25px;
            width: 275px;
            padding-left: 10px;
            padding-right: 10px;
        }
        .k-multiselect-wrap
        {
            overflow: auto;
            max-height: 65px;
        }
    </style>
    <script type="text/javascript">
        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
        //var textval = $('#txtDslip').val()

        function GetReport() {
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            var Bunit = $("#ddlBunit").val()
            $("#kgrdtl").html('');
            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '", "Bunit": "' + Bunit + '"}';
            //var str = '{"sdate": "' + t1 + '"}';

            $.ajax({
                type: "POST",
                url: "OTReport.aspx/GetData",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }

        function Export() {
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            var Bunit = $("#ddlBunit").val()
            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '", "Bunit": "' + Bunit + '"}';
            //var str = '{"sdate": "' + t1 + '"}';

            $.ajax({
                type: "POST",
                url: "OTReport.aspx/GetExport",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindExl,
                failure: function (response) {
                    alert(response.d);
                },
                error: function (response) {
                    alert(response.d);
                }
            });
        }

        function BindExl(result) {
            var url = result.d;
            window.location = url;
        }

        function BindReport(result) {
            if (result.d.Success == false) {
                $("#kgrid").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                return;
            }
            var Columns = result.d.Column;
            var data = JSON.parse(result.d.Data);
            bindGrid("kgrid", data, Columns, true, true, true, 550);
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


        function GetBusinessUnit() {
            var Bunit = $("#ddlBunit").val()
            var str = '{"Bunit": "' + Bunit + '"}';
            $.ajax({
                type: "POST",
                url: "OTReport.aspx/GetBusinessUnit",
                contentType: "application/json; charset=utf-8",
                data: str,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //dvsqEditLoader
                    var data = $.parseJSON(res);
                    $("#ddlBunit").kendoMultiSelect({
                        dataTextField: "BU",
                        dataValueField: "TID",
                        dataSource: data,
                        autoClose: false
                    }).data("kendoMultiSelect");
                },
                error: function (data) {
                }
            });
        }


        function SelectALLCircle() {
            var multiSelect = $("#ddlBunit").data("kendoMultiSelect");
            var selectedValues = "";
            var strComma = "";
            for (var i = 0; i < multiSelect.dataSource.data().length; i++) {
                var item = multiSelect.dataSource.data()[i];
                selectedValues += strComma + item.TID;
                strComma = ",";
            }
            multiSelect.value(selectedValues.split(","));
        }



        $(document).ready(function () {
            GetBusinessUnit();
            $("#select").bind("click", {}, SelectALLCircle);
        });
    </script>
    <div style="width: 100%; height: 47px; padding-top: 15px; padding-left: 10px; font: bold 17px 'verdana';
        color: #fff; background-image: url(logo/hfclstrip.jpg); background-repeat: no-repeat">
        Payout OT Report</div>
    <br />
    <div style="height: 50px;">
        <table style="width: 100%; border: 1">
            <tr>
                <td width="80px">
                    From Date:
                </td>
                <td width="120px">
                    <asp:TextBox ID="txtd1" runat="server" Width="120px" ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="txtd1" Format="yyyy-MM-dd"
                        runat="server">
                    </asp:CalendarExtender>
                </td>
                <td width="80px">
                    To Date:
                </td>
                <td width="120px">
                    <asp:TextBox ID="txtd2" runat="server" Width="120px" ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtd2" Format="yyyy-MM-dd"
                        runat="server">
                    </asp:CalendarExtender>
                </td>
                <td width="80px">
                    Business Unit:
                </td>
                <td width="150px" style="height: 20px">
                    <select id="ddlBunit" data-placeholder="Select  Circle" style="width: 150px;">
                    </select>
                </td>
                <td width="80px">
                    <input type="button" class="k-button" id="select" value="ALL" />
                </td>
                <td width="80px">
                    <input type="button" id="btnPlane" value="Search" class="btn-default" onclick="GetReport();"
                        style="width: 80px" />
                </td>
                <td width="80px">
                    <input type="button" id="btnExport" value="Export" class="btn-default" onclick="Export();"
                        style="width: 80px" />
                    <%-- <asp:Button ID="btnExp" runat="server" Text="Export" class="btn-default" Style="width: 80px" />--%>
                </td>
            </tr>
        </table>
    </div>
    <br /><br />
    <div>
        <div id="kgrid" style="width: 999px">
        </div>
    </div>
    <div id="mask">
        <div id="loader">
            <img src="images/loading.gif" />
        </div>
    </div>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/images/loading.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
