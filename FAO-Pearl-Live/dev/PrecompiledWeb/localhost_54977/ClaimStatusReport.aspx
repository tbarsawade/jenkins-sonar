<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="claimstatusreport, App_Web_o3dtvhns" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
    <link href="css/style.css" rel="stylesheet" />
    <script type="text/javascript">

        var new_window;
        function OpenWindow(url) {
            new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }
        //var win = window.open("Child.aspx", "thePopUp", "");
        function childClose() {
            //if (new_window.closed) {

            var DocumentType = $("#ContentPlaceHolder1_ddldocType").val();
            BindNeedToAct(DocumentType);
            GetGridMyReq(DocumentType);
            GetGridHistory(DocumentType);
            //$("#ContentPlaceHolder1_btnpendinggrdcl").click();

            //}
        }
        function ShowDialog(url) {
            // do some thing with currObj data

            var $dialog = $('<div></div>')
               .load(url)
               .dialog({
                   autoOpen: true,
                   title: 'Document Detail',
                   width: 700,
                   height: 550,
                   modal: true
               });
            return false;
        }
    </script>
  <style type="text/css">
        #divFilters {
            padding: 15px;
            background: #F7F7F7;
            min-height: 60px;
            /*width:1000px;*/
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

        
    </style>
    <div style="width: 100%; height: 47px; padding-top: 15px; padding-left: 10px; font: bold 17px 'verdana';
        color: #fff; background-image: url(logo/hfclstrip.jpg); background-repeat: no-repeat">
        Status Wise Claim Report</div>
    <br />
    <table style="width: 100%; border: 1">
        <tr>
            <td>
                From Date:&nbsp;
                <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true" Width="200px"
                    ClientIDMode="Static" Height="20px"></asp:TextBox>
                <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                </asp:CalendarExtender>
                &nbsp;&nbsp; To Date:&nbsp;
                <asp:TextBox runat="server" placeholder="End Date" ReadOnly="true" ID="txtd2" Width="200px"
                    ClientIDMode="Static" Height="20px"></asp:TextBox>
                <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                </asp:CalendarExtender>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <input type="button" id="btnPlane" value="Search" class="btn-default" onclick="GetData();" style="width:80px" />
            </td>
        </tr>
    </table>
    <br />
    <br />
    <div style="width: 100%;">
        <div class="demo-section k-header" style="width: 100%">
            <table style="width: 100%;">
                <tr>
                    <td width="600px">
                        <div id="example">
                            <div id="dvloader2" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kgrdChart" style="display: block; width: 900px; height:400px">
                            </div>
                            <div id="NoRecord3" style="display: none; text-align: center; min-height: 80px;">
                                <span style="color: #E96125; position: relative; top: 30px;">No Data found</span>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="popupDtl" style="display: none;">
        <table style="width: 100%;">
            <tr>
                <td>
                    <div id="kgrdtl" style="width: 100%;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        function GetData() {
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            var str = '{"sdate": "' + t1 + '", "tdate": "' + t2 + '"}';
            $.ajax({
                type: "POST",
                url: "claimstatusreport.aspx/GetJSON",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (res) {
                    createChart(res);

                }
            });

        }

        function createChart(data1) {
            //var tt1 = "Chaeck data";
            $("#kgrdChart").kendoChart({
                title: {
                    position: "bottom"
                    //text: "DOC Count"
                },
                legend: {
                    visible: false
                },
                seriesDefaults: {
                    labels: {
                        template: "#= category #:(#= kendo.format('{0}', value)#)",
                        visible: true,
                        background: "transparent"
                    }
                },
                dataSource: {
                    data: JSON.parse(data1.d)
                },
                series: [{
                    type: "pie",
                    field: "Count",
                    categoryField: "Status"
                }],
                seriesColors: ["#FF5733", "#FFC300", "#BF00FF", "#8000FF", "#2E64FE", "#9C27B0", "#1B5E20", "#00ACC1", "#FFEB3B", "#1A237E", "#880E4F", "#FF0000"],
                tooltip: {
                    visible: true
                    // template: "Circle:  #= category # <br/>Total Count: #= value#/" + ttl + "-(#= kendo.format('{0:P}', percentage)#)"
                },
                seriesClick: chartClick,
                height: 1200
            });
        }

       // View Details on chart click.
        function chartClick(e) {
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            $("#kgrdtl").html('');
           // var sts = e.dataItem.Status;
            //var sts1 = e.dataItem.category;
            var sts = e.category;
            //var sts = e.Status;
            var str = '{"sdate": "' + t1 + '", "tdate": "' + t2 + '", "status": "' + sts + '"}';
            $("#kgrdtl").html("<div class='loader' style='width:800px;' ></div>");
            $("#popupDtl").kendoWindow({
                width: "950px",
                height: "600px",
                title: "Status Wise Claim Report",
                visible: false,
                modal: true
            });

            $("#popupDtl").data("kendoWindow").center().open();
            $.ajax({
                type: "POST",
                url: "claimstatusreport.aspx/getDtl",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (result) {
                    $("#kgrdtl").html("");
                    if (result.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {

                        if (result.d.Success == false) {
                            $("#kgrdtl").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                            return;
                        }
                        var Columns = result.d.Column;
                        var data = $.parseJSON(result.d.Data);
                        bindGrid("kgrdtl", data, Columns, true, true, true, 550);

                    }
                }
            });
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
                   data: Data1
                },
                scrollable: {
                    virtual: true
                },

                columns: columns,
                width: 100,
                scrollable: true,
                reorderable: true,
                columnMenu: true,
                sortable: true,
                allowCopy: true,
                resizable: true,
                height: height,
                filterable: true,
                toolbar: ['excel'],
                excel: {
                    fileName: "Report.xlsx",
                    filterable: true,
                    pageable: true
                   }

            });

        }
        
        
    </script>
</asp:Content>
