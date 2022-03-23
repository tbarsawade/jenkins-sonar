<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="HclPoForBuyers, App_Web_gsdfcjye" viewStateEncryptionMode="Always" %>

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

        function GetVtype() {
            $.ajax({
                type: "POST",
                url: "HclPoForBuyers.aspx/GetPGtype",
                contentType: "application/json; charset=utf-8",
                data: {},
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //dvsqEditLoader
                    var data = $.parseJSON(res);
                    AllVType=  $("#ddlPurchaseGrp").kendoMultiSelect({
                        dataTextField: "fld1",
                        dataValueField: "tid",
                        dataSource: data,
                    }).data("kendoMultiSelect");
                    //$("#sallVtype").bind("click", { ID: "ddlVType",key:"vtype" }, SelectALL);
                },
                error: function (data) {
                }
            });
        }
        




        function GetReport() {
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            var t3 = $("#txtPoNum").val();
            var t4 = $("#txtVname").val();
            var t5 = $("#ddlPurchaseGrp").val();
            
            $("#kgrid").html('');

            var str = '{"sdate": "' + t1 + '", "tdate": "' + t2 + '", "PoNum": "' + t3 + '", "Vname": "' + t4 + '", "PrGrp": "' + t5 + '"}';


            $.ajax({
                type: "POST",
                url: "HclPoForBuyers.aspx/GetData",
                data: str,
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

         
          $(document).ready(function () {
          $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
            GetVtype();
              });

    </script>
    <div style="width: 100%; height: 47px; padding-top: 15px; padding-left: 10px; font: bold 17px 'verdana';
        color: #fff; background-image: url(logo/hfclstrip.jpg); background-repeat: no-repeat">
        Status of PO Based Invoices
    </div>
    <br />
    <table style="width: 100%; border: 1; padding: 10px;">
        <tr>
            <td>
                <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true" Width="200px"
                    ClientIDMode="Static" Height="20px"></asp:TextBox>
                <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                </asp:CalendarExtender>
            </td>
            <td>
                <asp:TextBox runat="server" placeholder="End Date" ReadOnly="true" ID="txtd2" Width="200px"
                    ClientIDMode="Static" Height="20px"></asp:TextBox>
                <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                </asp:CalendarExtender>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtPoNum" placeholder="PO Number" Width="200px" ClientIDMode="Static"
                    Height="20px"></asp:TextBox>
            </td>
        </tr>
        <tr style="height: 8px">
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 300px">
                <select id="ddlPurchaseGrp" data-placeholder="Purchase Group" style="width: 200px;">
                </select>
                <%--<input type="button" class="k-button" id="select" value="ALL" />--%>
                <%--<asp:TextBox runat="server" ID="txtPurchase" placeholder="Purchase Group" Width="200px"
                    ClientIDMode="Static" Height="20px"></asp:TextBox>--%>
            </td>
            <td>
                <asp:TextBox runat="server" placeholder="Vendor Name" ID="txtVname" Width="200px"
                    ClientIDMode="Static"></asp:TextBox>
            </td>
            <td>
                <input type="button" id="Button1" value="Search" class="btn-default" onclick="GetReport();"
                    style="width: 80px" />
            </td>
        </tr>
    </table>
    <br />
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
                <%--      <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/images/attch.gif" />--%>
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
