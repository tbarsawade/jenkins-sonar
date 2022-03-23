<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="ActivityLog, App_Web_gsdfcjye" viewStateEncryptionMode="Always" %>

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
    </style>
    <script type="text/javascript">
        //        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        //        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
        //var textval = $('#txtDslip').val()

        function GetReport() {
            var str = '{"sdate":"' + $('#txtd1').val() + '","tdate":"' + $('#txtd2').val() + '"}';
            //  var dslipnum = $('#txtDslip').val();
            $.ajax({
                type: "POST",
                url: "ActivityLog.aspx/GetLog",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }

        // new by sp for last login time report on 07-Feb-22

        function GetLastLoginReport() {
            var str = '{"sdate":"' + $('#txtd1').val() + '","tdate":"' + $('#txtd2').val() + '"}';
            //  var dslipnum = $('#txtDslip').val();
            $.ajax({
                type: "POST",
                url: "ActivityLog.aspx/GetLastLoginReport",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }

        // up to here last login time report


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
                    fileName: "ActivityLog.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }

    </script>

    <div class="form">
        <div class="doc_header">
            User's Activity Logs Report
        </div>

    <div class="row mg">
        <div class="col-md-12 col-sm-12">

    <%--<div style="width: 100%; height: 47px; padding-top: 15px; padding-left: 10px; font: bold 17px 'verdana';
        color: #fff; background-image: url(logo/hfclstrip.jpg); background-repeat: no-repeat">
        Activity Log Report</div>--%>
    <br />
    <div id="divFilters">
        <table style="width: 100%;">
            <tr>
                <td style="width:300px">
                    Start Date &nbsp;
                    <asp:TextBox runat="server" ID="txtd1" ClientIDMode="Static" placeholder="Start Date"
                        ReadOnly="true"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender2" ClientIDMode="Static" runat="server"
                        Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </td>
               <td style="width:300px">
                    End Date &nbsp;
                    <asp:TextBox runat="server" ClientIDMode="Static" placeholder="End Date" ReadOnly="true"
                        ID="txtd2"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </td>
                <td>
                    <asp:Button ID="btnSearch" ClientIDMode="Static" class="k-button" role="button" aria-disabled="false"
                        flag="0" runat="server" OnClientClick="GetReport(); return false;" Text="Get Report" />
                </td>
                <td>
                    <asp:Button ID="btnShowLastLogtime" ClientIDMode="Static" class="k-button" role="button" aria-disabled="false"
                        flag="0" runat="server" OnClientClick="GetLastLoginReport(); return false;" Text="Last Login Report" />
                </td>
            </tr>
        </table>
    </div>
    <div id="mask" style="height: 10px;">
        <div id="loader" style="height: 50px;">
        </div>
    </div>
    <div>
        <div id="kgrid" style="width:100%">
        </div>
    </div>
</div>
    </div>
</div> 
</asp:Content>
