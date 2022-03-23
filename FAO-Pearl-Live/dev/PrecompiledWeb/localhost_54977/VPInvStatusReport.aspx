<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="VPInvStatusReport, App_Web_erizob0y" viewStateEncryptionMode="Always" %>

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
        #divFilters {
            padding: 15px;
            background: #F7F7F7;
            min-height: 60px; /*width:1000px;*/
            box-sizing: border-box;
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


        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }

        .textbox {
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
        .mg {
        margin:10px 0px;
        }
    </style>
    <script type="text/javascript">

        function GetVtype() {
            $.ajax({
                type: "POST",
                url: "VPInvStatusReport.aspx/GetPGtype",
                contentType: "application/json; charset=utf-8",
                data: {},
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //dvsqEditLoader
                    var data = $.parseJSON(res);
                    AllVType = $("#ddlPurchaseGrp").kendoMultiSelect({
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
                url: "VPInvStatusReport.aspx/GetData",
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
    <div class="container-fluid">
        <div class="form">
            <div class="doc_header">
                Vendor Invoice Status Report
            </div>
            <div class="row mg">
                <div class="col-md-1 col-sm-1">
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true"
                        ClientIDMode="Static" CssClass="txtBox"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-1 col-sm-1">
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:TextBox runat="server" placeholder="End Date" ReadOnly="true" ID="txtd2"
                        ClientIDMode="Static" CssClass="txtBox"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:TextBox runat="server" ID="txtPoNum" CssClass="txtBox" placeholder="PO Number" ClientIDMode="Static"></asp:TextBox>
                </div>
                <div class="col-md-1 col-sm-1">
                </div>

            </div>
            <div class="row mg">
                <div class="col-md-1 col-sm-1">
                </div>
                <div class="col-md-3 col-sm-3">
                    <select id="ddlPurchaseGrp" data-placeholder="Purchase Group">
                    </select>
                </div>
                <div class="col-md-1 col-sm-1">
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:TextBox runat="server" placeholder="Vendor Name" ID="txtVname" class="txtBox"
                        ClientIDMode="Static"></asp:TextBox>
                </div>
                <div class="col-md-1 col-sm-1">
                </div>
                <div class="col-md-3 col-sm-3">
                    <input type="button" id="Button1" value="Search" class="btnNew" onclick="GetReport();" />
                </div>
            </div>
            <div class="row mg">
                <div class="col-md-12 col-sm-12">
                    <div id="kgrid" style="width: 100%">
                    </div>
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
        </div>
    </div>



</asp:Content>
