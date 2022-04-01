<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="CbInvPOJVReport, App_Web_4ysr50e5" viewStateEncryptionMode="Always" %>

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
        .k-loading-image
        {
            background-image: url('~/images/prg.gif');
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
        function GetReport() {
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            $("#kgrdtl").html('');
            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '"}';

            $.ajax({
                type: "POST",
                url: "CbInvPOJVReport.aspx/GetData",
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
        var initialLoad = true;
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
                requestStart: function () {
                    if (initialLoad)
                        kendo.ui.progress($("#kgrid"), true);
                },
                requestEnd: function () {
                    if (initialLoad)
                        kendo.ui.progress($("#kgrid"), false);
                    initialLoad = false;
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

    </script>
     <div id="divProgress" style="text-align: center; display: none;">
            <div class="loader"></div>
        </div>
    <div class="container-fluid">      
        <div class="form" style="text-align: left">
            <div class="doc_header">
                Invoice PO JV Report
            </div>
            <div class="row mg">
                <div class="col-md-2 col-sm-2">
                    <label>From Date:</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true" CssClass="txtBox"
                        ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender3" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>To Date:</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox runat="server" placeholder="End Date" ReadOnly="true" ID="txtd2" ClientIDMode="Static" CssClass="txtBox"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/search.png" CssClass="sch"
                        ToolTip="Search  " OnClientClick="GetReport(); return false;" />
                </div>
            </div>

            <div class="row mg">
                <div class="col-md-12 col-sm-12">
                      <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                    <div id="kgrid" style="width: 100%">
                    </div>
                </div>
            </div>
            <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                <ProgressTemplate>
                    <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                please wait...
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </div>
</asp:Content>
