<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="IHCLTATADB, App_Web_l4hlb3yz" viewStateEncryptionMode="Always" %>
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
                <div class="col-md-1 col-sm-1">
                    <label>Cluster</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <select id="ddlCluster" class="txtBox">
                        <option value="">-Select-</option>
                    </select>
                </div>
                 <div class="col-md-1 col-sm-1">
                    <label>Payment Type</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <select id="ddlPaymentttype" class="txtBox">
                        <option value="">-Select-</option>
                        <option value="INVOICE MATERIAL">INVOICE MATERIAL</option>
                        <option value="Service Invoice">SERVICE INVOICE</option>
                        <option value="Non PO Invoice">NON PO INVOICE</option>
                    </select>
                </div>
                <div class="col-md-1 col-sm-1">
                    <label>Value</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <select id="ddlvalue" class="txtBox">
                        <option value="">-Select-</option>
                        <option value="Count of Vendor">Count of Vendor</option>
                        <option value="Count of Invoice">Count of Invoice</option>
                        <option value="Value of Invoice">Value of Invoice</option>
                        <option value="TAT Days ">TAT Days</option>

                    </select>
                </div>
                        
                </div>
            <br />
              <div class="row mg">
                <div class="col-md-1 col-sm-1">
                    <label id="lblFrom">From Date</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox runat="server" ID="txtd1" placeholder="From Date" CssClass="txtBox"
                    ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </div>
                <div class="col-md-1 col-sm-1">
                    <label id="lblTo">To date</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox runat="server" placeholder="To date" ID="txtd2" ClientIDMode="Static" CssClass="txtBox"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </div>
                   <div class="col-md-1 col-sm-1">
                    
                </div>
                <div class="col-md-2 col-sm-2">
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
    <script>
        $(document).ready(function () {
            BindClusterName();


            $("#btnsearch").click(function () {
                GetReport();
            });
        });


        function BindClusterName() {
            var ddlCluster = $("#ddlCluster");
            var t = '{ documentType: ""}';
            $.ajax({
                type: "POST",
                url: "IHCLTATADB.aspx/GetReportName",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var strData = JSON.parse(response.d.Data);
                   // ddlCluster.empty().append($('<option></option>').val("").html("-Select-"));
                    ddlCluster.empty().append($('<option></option>').val("ALL").html("ALL"));

                    if (strData.length > 0) {
                        $.each(strData, function () {
                            ddlCluster.append($('<option></option>').val(this.Tid).html(this.ClusterName));
                        });

                    }
                    else {
                        ddlCluster.empty().append($('<option></option>').val("").html("-Select-"));
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

            //var reportid = $("#ddlReportName").val();
            //if (reportid == "") {
            //    alert("Please select report name.");
            //    return false;
            //}
            $("#dvloader").show();
            var cluster = $("#ddlCluster").val();
            var paymenttype = $("#ddlPaymentttype").val();
            if (paymenttype == "") {
                alert("Please select payment type .");
                $("#ddlPaymentttype").focus();
                return false;
            }
            var valueof = $("#ddlvalue").val();
            if (valueof == "") {
                alert("Please select value .");
                $("#ddlvalue").focus();
                return false;
            }
            var t1 = $("#txtd1").val();
            if (t1 == "") {
                alert("Please select from date .");
                $("#txtd1").focus();
                return false;
            }
            var t2 = $("#txtd2").val();
            if (t2 == "") {
                alert("Please select to date .");
                $("#txtd2").focus();
                return false;
            }
            /*var paymenttype = $("#ddlPaymentttype").val();*/
            
            $("#kgrdtl").html("");
            var str = '{"cluster": "' + cluster + '", "valueof": "' + valueof + '", "sdate": "' + t1 + '", "edate": "' + t2 + '", "paymenttype": "' + paymenttype + '"}';
            $.ajax({
                type: "POST",
                url: "IHCLTATADB.aspx/GetData",
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

