<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="PearlOutwardIntErrorResultview, App_Web_whgqqjhx" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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

    <div id="divProgress" style="text-align: center; display: none;">
            <div class="loader"></div>
        </div>
    <div class="container-fluid">      
        <div class="form" style="text-align: left">
            <div class="doc_header">
              Pearl Outward Integration Result View
            </div>
            <br /> 
            <div class="row mg">
                <div class="col-md-2 col-sm-2">
                    <label>From Date</label>
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:TextBox runat="server" ID="txtd1" placeholder="From Date" ReadOnly="true" CssClass="txtBox"
                    ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>To date</label>
                </div>
                <div class="col-md-3 col-sm-3">
                    <asp:TextBox runat="server" placeholder="To date" ReadOnly="true" ID="txtd2" ClientIDMode="Static" CssClass="txtBox"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </div>
            </div>
            <br />
            <div class="row mg">
                <div class="col-md-2 col-sm-2">
                </div>
                <div class="col-md-3 col-sm-3">
                    
                </div>
                <div class="col-md-2 col-sm-2">
                    
                </div>
                <div class="col-md-3 col-sm-3" style="text-align:right;">
                    <input type="button" id="btnsearch" value="Search" class="btnNew" />
                </div>
            </div>
            <br />
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
    <script type="text/javascript">
        $(document).ready(function () {
            //BindReportName();
            $("#btnsearch").click(function () {
                GetReport();
            });
        });
        //function BindReportName() {
        //    var ddlReportName = $("#ddlReportName");
        //    var t = '{ documentType: ""}';
        //    $.ajax({
        //        type: "POST",
        //        url: "AmbitReport.aspx/GetReportName",
        //        contentType: "application/json; charset=utf-8",
        //        data: t,
        //        dataType: "json",
        //        success: function (response) {
        //            var strData = JSON.parse(response.d.Data);
        //            ddlReportName.empty().append($('<option></option>').val("").html("-- Select --"));
        //            if (strData.length > 0) {
        //                $.each(strData, function () {
        //                    ddlReportName.append($('<option></option>').val(this.Reportid).html(this.ReportName));
        //                });

        //            }
        //            else {
        //                ddlReportName.empty().append($('<option></option>').val("").html("-- Select --"));
        //                //$("#dvloader").hide();
        //            }

        //        },
        //        error: function (data) {
        //            //Code For hiding preloader
        //        }
        //    });
        //}

        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
        function GetReport() {
            $("#dvloader").show();
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            $("#kgrdtl").html("");
            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '"}';
            $.ajax({
                type: "POST",
                url: "PearlOutwardIntErrorResultview.aspx/GetDataresult",
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
                $("#dvloader").hide();
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
            $("#dvloader").hide();
        }

    </script>
</asp:Content>

