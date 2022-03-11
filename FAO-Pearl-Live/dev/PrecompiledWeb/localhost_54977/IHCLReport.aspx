<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="IHCLReport, App_Web_13sbvfgr" viewStateEncryptionMode="Always" %>
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
              IHCL Report
            </div>
            <br /> 
            <div class="row mg">
                <div class="col-md-2 col-sm-2">
                    <label>Report Name</label>
                </div>
                <div class="col-md-3 col-sm-3">
                   <%-- <select id="ddlReportName" class="txtBox" runat="server" clientidmode="Static">
                        <option value="">-Select-</option>
                    </select>--%>
                    <asp:DropDownList id="ddlReportName" class="txtBox" runat="server" clientidmode="Static"></asp:DropDownList>
                </div>
                 <div class="col-md-2 col-sm-2 category">
                    <label>Operating Unit</label>
                </div>
                <div class="col-md-3 col-sm-3 category">
                    <%--<select id="ddlHotelCategory" class="txtBox" runat="server" clientidmode="Static">
                        <option value="">-Select-</option>
                    </select>--%>
                    <asp:DropDownList id="ddlHotelCategory" class="txtBox" runat="server" clientidmode="Static"></asp:DropDownList>
                </div>
            </div>
            <br />
          <div class="row mg">
                <div class="col-md-2 col-sm-2 category">
                    <label>Vendor Name</label>
                </div>
                <div class="col-md-3 col-sm-3 category">
                    <asp:TextBox runat="server" ID="txtVendorName" placeholder="Vendor Name"  CssClass="txtBox" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="col-md-2 col-sm-2 opcategory userrpt">
                    <label>From Date</label>
                </div>
                <div class="col-md-3 col-sm-3 opcategory userrpt">
                    <asp:TextBox runat="server" ID="txtd1" placeholder="From Date"  CssClass="txtBox opcategory"
                    ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender3" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-2 col-sm-2 opcategory userrpt">
                    <label>To Date</label>
                </div>
                <div class="col-md-3 col-sm-3 userrpt">
                    <asp:TextBox runat="server" placeholder="To date"  ID="txtd2" ClientIDMode="Static" CssClass="txtBox opcategory"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </div>
                 <div class="col-md-1 col-sm-1" style="text-align:left;">
                    <input type="button" id="btnsearch" value="Search" class="btnNew" />
                </div>
              <div class="col-md-1 col-sm-1 category userrpt" style="text-align:left;">
                <%--    <button id="btnExport" class="btnNew">Export</button>--%>
                  <asp:Button runat="server" ID="btnExport" Text="Direct Export"  class="btnNew"/>
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
            //BindOperatingUnit();
            $(".category").hide();
            $("#btnsearch").click(function () {
                GetReport();
            });
           
        });
        $(function () {
            $("#ddlReportName").change(function () {
                var reportname = $('option:selected', this).val();
                if (reportname == '2133') {
                    $(".category").show();
                    $(".opcategory").hide();
                  
                }
               
                else {
                    $(".category").hide();
                    $(".opcategory").show();
                
                }
            });
        });
        function BindReportName() {
            var ddlReportName = $("#ddlReportName");
            var t = '{ documentType: ""}';
            $.ajax({
                type: "POST",
                url: "IHCLReport.aspx/GetReportName",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var strData = JSON.parse(response.d.Data);
                    ddlReportName.empty().append($('<option></option>').val("").html("-- Select --"));
                    if (strData.length > 0) {
                        $.each(strData, function () {
                            ddlReportName.append($('<option></option>').val(this.Reportid).html(this.ReportName));
                        });

                    }
                    else {
                        ddlReportName.empty().append($('<option></option>').val("").html("-- Select --"));
                        //$("#dvloader").hide();
                    }

                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }

        function BindOperatingUnit() {
            var ddlHotelCategory = $("#ddlHotelCategory");
            $.ajax({
                type: "POST",
                url: "IHCLReport.aspx/GetOperatingUnit",
                contentType: "application/json; charset=utf-8",
                data: {},
                dataType: "json",
                success: function (response) {
                    var strData = JSON.parse(response.d.Data);
                    ddlHotelCategory.empty().append($('<option></option>').val("").html("-- Select --"));
                    if (strData.length > 0) {
                        $.each(strData, function () {
                            ddlHotelCategory.append($('<option></option>').val(this.tid).html(this.Category));
                        });

                    }
                    else {
                        ddlHotelCategory.empty().append($('<option></option>').val("").html("-- Select --"));
                        //$("#dvloader").hide();
                    }

                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }
        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });

        function GetReport() {
            
            var reportid = $("#ddlReportName").val();
            var hotelcat = $("#ddlHotelCategory").val();
            if (reportid == "") {
                alert("Please select report name.");
                return false;
            }
            if (reportid == 2133 && hotelcat=='') {
                alert('Please select hotel category');
                return false;
            }
            $("#dvloader").show();
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            var catrgory = $.trim($("#ddlHotelCategory").val());
            var vendorName = $("#txtVendorName").val(); 
            $("#kgrdtl").html("");
            //var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '", "Reportid": "' + reportid + '", "VendorCode": "' + VendorCode + '"}';
            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '", "Reportid": "' + reportid + '","catrgory": "' + catrgory + '","vendorName": "' + vendorName + '"}';
            $.ajax({
                type: "POST",
                url: "IHCLReport.aspx/GetData",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) {
                    alert('Please Use Direct Export !!!');
                    $("#dvloader").hide();
                },
                failure: function (response) {
                    $("#dvloader").hide();
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

        //function exportReport() {

        //    var reportid = $("#ddlReportName").val();
        //    var hotelcat = $("#ddlHotelCategory").val();
        //    if (reportid == "") {
        //        alert("Please select report name.");
        //        return false;
        //    }
        //    if (reportid == 2133 && hotelcat == '') {
        //        alert('Please select hotel category');
        //        return false;
        //    }
           
        //    var t1 = $("#txtd1").val();
        //    var t2 = $("#txtd2").val();
        //    var catrgory = $.trim($("#ddlHotelCategory").val());
        //    var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '", "Reportid": "' + reportid + '","catrgory": "' + catrgory + '"}';
        //    $.ajax({
        //        type: "POST",
        //        url: "IHCLReport.aspx/GetExport",
        //        data: str,
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: BindExl,
        //        error: function (err) { alert('Something Went Wrong!!!'); },
        //        failure: function (response) {

        //        }
        //    });

        //}
        //function BindExl(result) {
        //    var url = result.d;
        //    alert(url);
        //    window.location = url;
        //}
    </script>
</asp:Content>

