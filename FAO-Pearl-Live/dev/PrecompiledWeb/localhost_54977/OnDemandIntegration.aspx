<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="OnDemandIntegration, App_Web_20pgc3v0" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src='http://code.jquery.com/jquery-latest.min.js' type='text/javascript'> </script>

    <link rel="stylesheet" href="kendu/homekendo.common.min.css" />
    <link rel="stylesheet" href="kendu/homekendo.rtl.min.css" />
    <%--<link rel="stylesheet" href="kendu/homekendo.silver.min.css" />--%>
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="kendu/homekendo.mobile.all.min.css" />

    <%--kendo.data.min.js--%>
    <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <%--<script src="kendu/homejquery-1.9.1.min.js"></script>--%>

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
    <style>
         .k-grid-toolbar a {
            float: right;
        }
        .error {
            border: 1px solid red !important;
        }
        .loader {
            left: 50%;
            top: 50%;
            position: absolute;
            z-index: 101;
            opacity : 0.5;
            background-repeat : no-repeat;
            background-position : center;
            width: 32px;
            height: 32px;
            margin-left: -16px;
            margin-top: -16px;
           
        }
    </style>
    <div class="form" style="text-align: left">
        <div class="doc_header">
            Reports Integration
        </div>
       
                <br />  
                <div class="row mg">
                    <div class="col-md-1 col-sm-1">
                        </div>
                    <div class="col-md-1 col-sm-1">
                        <label>Report Name:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <select id="ddlReportName" class="txtBox">
                            <option value="0">-Select-</option>
                        </select>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Integration Type:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <select id="ddlSchType" class="txtBox">
                            <option value="0">-Select-</option>
                            <option value="1">On Demand</option>
                            <option value="2">Re Run</option>
                        </select>
                    </div>
                     <%--<div class="col-md-2 col-sm-2" style="float:right;">
                        <input type="button" id="btnsearch" value="Run" class="btnNew" />
                    </div>--%>
                     
                </div>
        <br />
            <div class="row mg" id="dvcal">
                   <div class="col-md-1 col-sm-1">
                        </div>
                <div class="col-md-1 col-sm-1">
                    <label>From Date:</label>
                </div>
                <div class="col-md-3 col-sm-3">
                   <%-- <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true" CssClass="txtBox"
                        ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender3" runat="server" Format="yyyy-MM-dd HH:MM" TargetControlID="txtd1">
                    </asp:CalendarExtender>--%>
                    <input id="txtd1" title="datetimepicker" class="text-box" style="width: 100%;" />
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>To Date:</label>
                </div>
                <div class="col-md-3 col-sm-3">
                  <%--  <asp:TextBox runat="server" placeholder="End Date" ReadOnly="true" ID="txtd2" ClientIDMode="Static" CssClass="txtBox"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="yyyy-MM-dd HH:MM" TargetControlID="txtd2">
                    </asp:CalendarExtender>--%>
                    <input id="txtd2" title="datetimepicker" class="text-box"  style="width: 100%;" />
                </div>
                
                 
            </div>
        <br />
           <div class="row">
                <div class="col-md-1 col-sm-1">
                    </div>
                <div class="col-md-11 col-sm-11">
                     <label id="lblMsg"></label>
                    </div>
               </div>
                   
        <br />
      
        <div class="row mg" id="dvbtn">
            <div class="col-md-7 col-sm-7">

            </div>
            <div class="col-md-1 col-sm-1">
                     <input type="button" id="btnsearch" class="btnNew" />
                </div>
            <div class="col-md-2 col-sm-2">
                     <input type="button" id="btnsave" value="Run Integration" class="btnNew" />
                </div>
           <div class="col-md-2 col-sm-2">

            </div>
        </div>
        <br />
                <br />
        <div class="row">
            <div class="col-md-1 col-sm-1">
                 <br /><br />
                       
                
            </div>
            <div class="col-md-10 col-sm-10" id="dvkgd">
                <div id="kgrid"></div>
            </div>
            <div class="col-md-1 col-sm-1">
                <%-- <br /><br />
                        <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                        </div>
                --%>
                
                
            </div>
        </div><br />
        <div id="dvloader" style="display: none;" class="loader">
            <input type="image" src="../images/preloader22.gif" />
        </div>
        <div id="dvloaderSave" style="display: none;" class="loader">
            <input type="image" src="../images/preloader22.gif" />
        </div>
    <script type="text/javascript">
        $(document).ready(function () {
            // create DateTimePicker from input HTML element
            $("#txtd1,#txtd2").kendoDateTimePicker({
                value: new Date(),
                format: "yyyy-MM-dd HH:mm",
                dateInput: true
            });
        });
        $(document).ready(function () {
            $("#dvcal,#dvbtn,#btnsave").hide();
            BindReportName();
            $("#ddlSchType").change(function () {
                if ($("#ddlReportName").val() == "0") {
                    alert("Please select report name");
                    return false;
                }
                //$("#dvloader").show();
                if ($("#ddlSchType").val() == "0") {
                    alert("Please select integration type");
                    return false;
                }
                if ($("#ddlSchType").val() == "1") {
                    $("#dvcal").hide();
                    $("#dvbtn").show();
                    $("#dvkgd,#btnsave").hide();
                    if (Kgrid) {
                        $('#kgrid').kendoGrid('destroy').empty();
                    }
                    $("#btnsearch").prop("value", "Run");
                }
                if ($("#ddlSchType").val() == "2") {
                    $("#dvcal,#dvbtn,#dvkgd").show();
                    $("#btnsearch").prop("value", "Search");
                }
            });
            $("#ddlReportName").change(function () {
                GetLastUpdateDate();
            });
            $("#btnsearch").click(function () {
               // alert($("#txtd1").val());
                if ($("#ddlSchType").val() == "0") {
                    alert("Please select integration type");
                    return false;
                }
                if ($("#ddlSchType").val() == "1") {
                   // Run on demand
                    ExecuteOnDemand();
                }
                if ($("#ddlSchType").val() == "2") {
                    if ($("#txtd1").val() == "" || $("#txtd2").val() == "") {
                        alert("Please select date range");
                        return false;
                    }
                    else {
                        BindGrid();
                    }
                }
               
            });
            function GetLastUpdateDate() {
                var t = '{ TID: "' + $("#ddlReportName").val() + '"}'
                $.ajax({
                    type: "POST",
                    url: "OnDemandIntegration.aspx/GetLastUpdateDate",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        $("#lblMsg").text("Last Scheduled: " + response.d.Message);
                    },
                    error: function (response) {
                        alert("Unexpected Error Occured");
                    }
                });
            };
        });
        var Kgrid = "";
        function BindReportName() {
            $("#dvloader").show();
            var ddlReportName = $("#ddlReportName");
            var t = '{ reportname: ""}';
            $.ajax({
                type: "POST",
                url: "OnDemandIntegration.aspx/GetReportName",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var strData = JSON.parse(response.d.Data);
                    ddlReportName.empty().append($('<option></option>').val("0").html("-- Select --"));
                    if (strData.length > 0) {
                        $.each(strData, function () {
                            ddlReportName.append($('<option></option>').val(this.TID).html(this.ReportSubject));
                        });
                        $("#dvloader").hide();
                    }
                    else {
                        ddlReportName.empty().append($('<option></option>').val("0").html("-- Select --"));
                        $("#dvloader").hide();
                    }

                },
                error: function (data) {
                    //Code For hiding preloader
                     $("#dvloader").hide();
                }
            });
        }
        var Kgrid = "";
        function BindGrid() {
             $("#dvloader").show();
            var ddlReportName = $("#ddlReportName").val();
            if (ddlReportName != "") {
                var t = '{ TID: "' + ddlReportName + '",sdate: "' + $("#txtd1").val() + '",edate: "' + $("#txtd2").val() + '"}';
             //   alert(t);
                $.ajax({
                    type: "POST",
                    url: "OnDemandIntegration.aspx/getresult",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (result) {
                        if (result.d.Data != "") {
                            var strData = JSON.parse(result.d.Data);
                            if (strData.length > 0) {
                                //if (Kgrid) {
                                //    $('#kgrid').kendoGrid('destroy').empty();
                                //}
                                
                                $("#btnsave").show();
                                var Columns = result.d.Column;
                               // { selectable: true, width: "50px" },
                                var CommanObj = { title: "<input id='chkAll' class='checkAllCls' type='checkbox'/>", width: "35px", template: "<input type='checkbox' class='check-box-inner' />", filterable: false };
                                Columns.splice(0, 0, CommanObj);

                                var data = JSON.parse(result.d.Data);
                                bindGrid("kgrid", data, Columns, true, true, true, 550);
                                 $("#dvloader").hide();

                            }
                            else {
                                 $("#dvloader").hide();
                            }
                        }
                        else {
                            alert("No data found.");
                            $("#dvloader").hide();
                            $("#btnsave").hide();
                        }
                    },
                    error: function (data) {
                        //Code For hiding preloader
                         $("#dvloader").hide();
                    }
                });

            }
        }
        function onChange(arg) {
            var ids = this.selectedKeyNames();
            $("#hdnTID").val(this.selectedKeyNames().join(", "));
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
                //dataBound: onDataBound,
                onChange: onChange,
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
            if (gridDiv != "") {
                $(".checkAllCls").on("click", function () {
                    var ele = this;
                    var state = $(ele).is(':checked');
                    var grid = $('#kgrid').data('kendoGrid');
                    if (state == true) {
                        $('.check-box-inner').prop('checked', true);
                    }
                    else {
                        $('.check-box-inner').prop('checked', false);
                    }
                });

                $("#btnsave").unbind().bind("click", function () {
                    $("#dvloaderSave").show();
                    var grid = $("#kgrid").data("kendoGrid");
                    var ds = grid.dataSource.data();
                    var lstData = [];
                    var arrError = [];
                    for (var i = 0; i < ds.length; i++) {
                        var row = grid.table.find("tr[data-uid='" + ds[i].uid + "']");
                        var checkbox = $(row).find(".check-box-inner");
                        var DelayReason = $(row).find(".DelayReason").val();
                        if (checkbox.is(":checked")) {
                            var idsToSend = { DocID: 0};
                            if (DelayReason == "0") {
                                $(row).find(".DelayReason").addClass('error');
                                arrError.push(false);
                            }
                            else {
                                $(row).find(".DelayReason").removeClass('error');
                                arrError.push(true);
                            }
                            idsToSend.DocID = ds[i].DocID;
                            lstData.push(idsToSend);
                        }
                        else {
                            $(row).find(".DelayReason").removeClass('error');
                            arrError.push(true);
                        }
                    }
                    if (lstData.length > 0) {
                        if (arrError.length > 0) {
                            for (var i = 0; i < arrError.length; i++) {
                                if (arrError[i] == false) {
                                    $("#dvloaderSave").hide();
                                    return false;
                                }
                            }
                        }
                        var strdocs = "";
                        $.each(lstData, function () {
                            if (strdocs == "")
                                strdocs = this.DocID;
                            else
                                strdocs += "," + this.DocID;
                        });

                        var ReportName = $("#ddlReportName").val();
                        var t = '{ TID: "' + ReportName + '",DOCID: "' + strdocs + '",sdate: "' + $("#txtd1").val() + '",edate: "' + $("#txtd2").val() + '"}';
                        $.ajax({
                            type: "POST",
                            url: "OnDemandIntegration.aspx/ExecuteRerunIntegration",
                            contentType: "application/json; charset=utf-8",
                            data: t,
                            dataType: "json",
                            success: function (response) {
                                //var msg = response.d;
                                //alert(msg);
                                alert("Executed Successfuly");
                                $("#dvloaderSave").hide();
                                BindGrid();
                            },
                            error: function (data) {
                                //Code For hiding preloader
                            }
                        });
                        }
                    else {
                        alert("Please select at-least one document to proceed.");
                        $("#dvloader").hide();
                        $("#dvloaderSave").hide();
                    }
                });
            }

        }

        // Run Ob demand integration
        function ExecuteOnDemand() {
            $("#dvloaderSave").show();
            var t = '{ TID: "' + $("#ddlReportName").val() + '"}'
            $.ajax({
                type: "POST",
                url: "OnDemandIntegration.aspx/ExecuteOnDemand",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (result) {
                   // var strData = JSON.parse(response.d.Message);
                    $("#dvloaderSave").hide();
                    alert("Executed Successfuly");
                   
                },
                error: function (result) {
                    $("#dvloaderSave").hide();
                    alert("Unexpected Error Occured");
                    
                }
            });
        }
    </script>
</asp:Content>

