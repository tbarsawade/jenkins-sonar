<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false"
    CodeFile="OverSpeedreport.aspx.vb" Inherits="logbookreport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--<link href="mobile/css/ui-lightness/jquery-ui-1.10.3.custom.css" rel="stylesheet" />
    <script src="mobile/js/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="mobile/js/jquery-ui-1.10.3.custom.js" type="text/javascript"></script>--%>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <style>
        .basic-grey input[type="text"], .basic-grey input[type="email"], .basic-grey textarea, .basic-grey select
        {
            border: 1px solid #DADADA;
            color: #888;
            height: 26px;
            margin-right: 6px;
            margin-top: 2px;
            outline: 0 none;
            padding: 3px 3px 3px 5px;
            width: 200px;
            font-size: 12px;
            line-height: 15px;
            box-shadow: inset 0px 1px 4px #ECECEC;
            -moz-box-shadow: inset 0px 1px 4px #ECECEC;
            -webkit-box-shadow: inset 0px 1px 4px #ECECEC;
        }
        
        .basic-grey .button
        {
            background: darkgreen;
            border: none;
            padding: 9px 25px;
            color: #FFF;
            box-shadow: 1px 1px 5px #B6B6B6;
            border-radius: 3px;
            text-shadow: 1px 1px 1px #9E3F3F;
            cursor: pointer;
        }
        
        #mask
        {
            background-color: #F6F6F6;
            z-index: 10000;
            height: 100%;
            width: 800px;
        }
        
        #loader
        {
            width: 200px;
            height: 200px;
            position: absolute;
            left: 50%;
            top: 50%;
            background-image: url("images/loading.gif");
            background-repeat: no-repeat;
            background-position: center;
            margin: -100px 0 0 -100px;
        }
        
        
        
        input#exp
        {
            padding: 4px 20px; /*give the background a gradient*/
            background: #ffae00; /*fallback for browsers that don't support gradients*/
            background: -webkit-linear-gradient(top, #ffae00, #d67600);
            background: -moz-linear-gradient(top, #ffae00, #d67600);
            background: -o-linear-gradient(top, #ffae00, #d67600);
            background: linear-gradient(top, #ffae00, #d67600);
            border: 2px outset #dad9d8; /*style the text*/
            font-family: Andika, Arial, sans-serif; /*Andkia is available at http://www.google.com/webfonts/specimen/Andika*/
            font-size: 1.1em;
            letter-spacing: 0.05em;
            text-transform: uppercase;
            color: #fff;
            text-shadow: 0px 1px 10px #000; /*add to small curve to the corners of the button*/
            -webkit-border-radius: 15px;
            -moz-border-radius: 15px;
            border-radius: 15px; /*give the button a drop shadow*/
            -webkit-box-shadow: rgba(0, 0, 0, .55) 0 1px 6px;
            -moz-box-shadow: rgba(0, 0, 0, .55) 0 1px 6px;
            box-shadow: rgba(0, 0, 0, .55) 0 1px 6px;
        }
        /****NOW STYLE THE BUTTON'S HOVER STATE***/
        input#exp:hover, input#exp:focus
        {
            border: 2px solid #dad9d8;
        }
    </style>
    <script type="text/javascript">

        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }
        function ShowDialog(url) {
            var $dialog = $('<div></div>')
               .load(url)
               .dialog({
                   autoOpen: true,
                   title: 'Vehicle Detail',
                   width: 700,
                   height: 550,
                   modal: true
               });
            return false;
        }
    </script>
    <div class="basic-grey" style="width: 100%; margin-top20px; border: 1px solid #dddddd;
        border-radius: 5px; margin-top: 20px;">
        <asp:UpdatePanel ID="upd" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnSearch" />
            </Triggers>
            <ContentTemplate>             
            <asp:HiddenField ID="hdnUID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnurole" runat="server" Value="" />
                 <asp:HiddenField ID="hdnEid" runat="server" Value="0" />
                <asp:HiddenField ID="hdnstdt" runat="server" />
                <asp:HiddenField ID="hdneddt" runat="server" />
        <table style="width: 100%; padding: 15px 125px;">
            <tr>
                <td>
                    <span>Start Date</span>
                    <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                    </asp:CalendarExtender>
                </td>
                <td>
                    <span>End Date</span>
                    <asp:TextBox runat="server" placeholder="End Date" ReadOnly="true" ID="txtd2"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd2">
                    </asp:CalendarExtender>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClientClick="javascript:return Bindreport();"
                        CssClass="button" />
                </td>
            </tr>
        </table>
                </ContentTemplate>
        </asp:UpdatePanel> 
    </div>
    <%--<div id="mask" style="height: 10px;">--%>
            <div id="loader" style="height: 50px;">
            </div>
    <%--</div>--%>
    <asp:ImageButton ID="btnExcelExport" ToolTip="Export EXCEL" runat="server" Width="18px"
        Height="18px" ImageAlign="Right" ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
    <br />
    <div style="width: 100%; border: solid 1px #e1e1e1; overflow-x: scroll;">
        <div id="grid">
        </div>
    </div>
 
    <%--<div id="gvModal" style="border: solid 1px #e1e1e1; verflow-x: scroll;">
        <input type="submit" id="exp" style="margin-left: 1000px; width: 50px; font: bold;" value="Export"
            onclick="javascript:Export('gvModal');return false;"  />
        <div id="grddetails">
          
        </div>
    </div>--%>
 
    <script type="text/javascript">
        function Bindreport() {
            $("#mask").show();
            MyGrid();
            //$("#mask").show();
            MyChart();
            return false;
        }
        //function createChart(data1, Name, chartID) {
        //    $("#" + chartID).kendoChart({
        //        title: {
        //            position: "bottom",
        //            text: Name
        //        },
        //        legend: {
        //            visible: true
        //        },
        //        chartArea: {
        //            background: ""
        //        },
        //        seriesDefaults: {
        //            labels: {
        //                visible: false,
        //                background: "transparent",
        //                template: "#= category #"
        //            }
        //        },
        //        series: [{
        //            type: "pie",
        //            startAngle: 150,
        //            data: data1
        //        }],
        //        tooltip: {
        //            visible: true,
        //            template: "${ category } - ${ value }"
        //        }
        //    });
        //}
        $(document).ready(function () {
            MyGrid();
            $("#mask").show();
            MyChart();
        });


        //        for exprt

        function Export(divid) {


            //  var data1 = $(divid + " #" + id).html();
            var data1 = $("#" + divid).find('tbody').html();
            var header = $("#" + divid).find('thead').html();

            $("#" + divid + " table thead tr").each(function () {
                //$(this).find("th:first").remove();
                $(this).find("th:last").remove();

            });

            $("#" + divid + " table tbody tr").each(function () {
                // $(this).find("td:first").remove();
                $(this).find("td:last").remove();
            });

            $("#" + divid + " table thead tr").css('background-color', '#CCCCB2');

            var data = $("#" + divid).find('tbody').html();
            $("#" + divid).find('tbody').html(data1);
            // $("#" + id).html(data1);
            data = header + data;
            data = escape(data);
            
            $('body').prepend("<form method='post' action='exportPage.aspx' style='top:-3333333333px;' id='tempForm'><input type='hidden' name='data' value='" + data + "' ></form>");
            $('#tempForm').submit();
            $("tempForm").remove();
            return false;
        };




        // end

        function MyChart() {
            var t1 = $("#ContentPlaceHolder1_txtd1").val();
            var t2 = $("#ContentPlaceHolder1_txtd2").val();
            var t = '{"d1":"' + t1 + '", "d2":"' + t2 + '"}';
            $.ajax({
                type: "post",
                url: "OverSpeedreport.aspx/GetJSON",
                data: t,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var d = data.d;
                    var d1 = d.data1;
                    var d2 = d.data2;
                    var d3 = d.data3;

                    //createChart(d1, "Vehicle Count", "chart");
                    //createChart(d2, "Site Visited Count", "chart1");
                    //createChart(d3, "Total Km Run", "chart2");

                },
                error: function (data) {

                    alert("Error");
                }

            });
        }
        function MyGrid() {
            //var data = { 'd1': '" + Salutation + "', 'd2': '" + Name + "' };
            var t1 = $("#ContentPlaceHolder1_txtd1").val();
            var t2 = $("#ContentPlaceHolder1_txtd2").val();
            var uid = $("#ContentPlaceHolder1_hdnUID").val();
            var uRole = $("#ContentPlaceHolder1_hdnurole").val();
            var eid = $("#ContentPlaceHolder1_hdnEid").val();
            var t = '{"sdate":"' + t1 + '", "tdate":"' + t2 + ',"}';
            var t11 = '&sdate=' + t1 + '&tdate=' + t2 + '&' + 'UID=' + uid + '&UROLE=' + uRole+'&Eid='+eid;
            $.ajax({
                type: "GET",
                url: "NightLogbook.svc/GetOverSpeed",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: t11,
                success: function (msg) {
                    $("#mask").hide();
                    var t2 = msg.GetOverSpeedResult;
                    var data1 = $.parseJSON(t2.data);
                    bindGrid(data1);
                },
                error: function (data) {
                    alert("Error");
                    $("#mask").hide();

                }

            });
        }

        //function MyDetailGrid(vno, sDate, tdate) {
        //    var t = '{"vehno":"' + vno + '", "start":"' + sDate + '","enddate":"' + tdate + '"}';
        //    $.ajax({
        //        type: "post",
        //        url: "Nightlogbookreport.aspx/FillUserDetails",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        data: t,
        //        success: function (msg) {
        //            var t2 = msg.d;
        //            var data1 = $.parseJSON(t2);
        //            bindGridDetail(data1);

        //        },
        //        error: function (data) {
        //            alert("Error");
        //            $("#mask").hide();

        //        }

        //    });
        //}

//        function ShowMap(e) {
////            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
////            //var Tid = dataItem.Tid;
////            var Start_Date = kendo.toString(new Date(dataItem.Start_Date), "d");
////            var End_Date = kendo.toString(new Date(dataItem.End_Date), "d");
////            var t1 = "ShowMapIndusNew.aspx?tid=" + Tid;
//            //OpenWindow(t1);



//            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
//            var Vehicle_No = dataItem.Vehicle_No;
//            var Start_DateTime = kendo.toString(new Date(Date.parse(dataItem.Start_DateTime)), "yyyy-MM-dd HH:mm:ss");
//            var End_Datetime = kendo.toString(new Date(Date.parse(dataItem.End_DateTime)), "yyyy-MM-dd HH:mm:ss");
//            var t1 = "ShowMapIndusNew.aspx?vehicle No=" + Vehicle_No + "&Start Date=" + Start_DateTime + "&End Date=" + End_Datetime;
//            OpenWindow(t1);



//            return false;
//        }

    </script>
    <%--<script type="text/javascript">
        function bindGridDetail(Data1) {
            $("#grddetails").kendoGrid({
                dataSource: {
                    data: Data1,
                    schema: {
                        model: {
                            fields: {
                                Circle: { type: "string" },
                                Vehicle_Name: { type: "string" },
                                Vehicle_No: { type: "string" },
                                Start_DateTime: { type: "date" },
                                End_DateTime: { type: "date" },
                                Distance_Travelled: { type: "number" },
                                                                                            
                            }
                        }
                    }
                    // pageSize: 20
                },
                //height: 430,
                scrollable: true,
                resizable: true,
                reorderable: true,
                // groupable: true,
                sortable: true,
                filterable: true,

                //custome : true,
                //pageable: true,
                columns: [{
                    field: "Circle",
                    title: "Circle"
                },

                             {
                                 field: "Vehicle_Name",
                                 title: "Vehicle Name"

                             }, {
                                 field: "Vehicle_No",
                                 title: "Vehicle No"
                             },  {
                                 field: "Start_DateTime",
                                 title: "Start DateTime",
                               format  : "{0:dd-MMM-yyyy hh:mm:ss tt}",
                                parseFormats: ["yyyy-MM-dd'T'HH:mm:ss.zz"]
                             }
                             , {
                                 field: "End_DateTime",
                                 title: "End DateTime",
                                 format  : "{0:dd-MMM-yyyy hh:mm:ss tt}",
                               parseFormats: ["yyyy-MM-dd'T'HH:mm:ss.zz"]
                             },
                              
                             { command: { text: "View Map", click: ShowMap} }
                    ]
            });
            //jQuery.noConflict();
            $("#mask").hide();



            var accessWindow = $("#gvModal").kendoWindow({
                actions: ["Close", "Minimize", "Maximize", "Pin", ], /*from Vlad's answer*/
                draggable: true,
                modal: true,
                resizable: true,
                collapsible: true,
                title: "Vehicle Details",
                visible: false /*don't show it yet*/
            }).data("kendoWindow").center().open();
        }
    </script>--%>
    <script>
        function bindGrid(Data1) {
            //debugger;
            $("#grid").kendoGrid({
                dataSource: {
                    data: Data1,
                    schema: {
                        model: {
                            fields: {
                                Circle: { type: "string" },
                                Vehicle_Name: { type: "string" },
                                Vehicle_No: { type: "string" },
                                Date: { type: "date" },
                                Over_Speed_Instance: { type: "number" },
                                Maximum_Speed: { type: "number" }
                            }
                        }
                    },
                    pageSize: 20
                },
                height: 430,
                scrollable: true,
                resizable: true,
                reorderable: true,
                groupable: true,
                sortable: true,
                filterable: true,
                pageable: true,
                columns: [{
                    field: "Circle",
                    title: "Circle"
                },

                             {
                                 field: "Vehicle_Name",
                                 title: "Vehicle Name"

                             }, {
                                 field: "Vehicle_No",
                                 title: "Vehicle No"
                                 //templet: '<a href="VehicleDetails.aspx?vehicle No=#Vehicle_No#&Start Date=#Start_Date#&End Date=#End_Date#>#Vehicle_No#</a>'
                             }, {
                                 field: "Date",
                                 title: "Date",
                                 format: "{0:dd-MM-yyyy}"
                             }
                             ,
                               {
                                   field: "Over_Speed_Instance",
                                   title: "Over Speed Instance"
                               },
                               {
                                   field: "Maximum_Speed",
                                   title: "Maximum Speed"
                               },
                             //{ command: { text: "View Details", click: showDetails} }
                ]
            });
            $("#mask").hide();
        }


        //function showDetails(e) {

        //    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //    var V_no = dataItem.Vehicle_No;
        //    var Start_Date = kendo.toString(new Date(dataItem.Start_Date), "d");
        //    var End_Date = kendo.toString(new Date(dataItem.End_Date), "d");
        //    //var t1 = "VehicleDetails.aspx?vehicle No=" + V_no + "&Start Date=" + Start_Date + "&End Date=" + End_Date;
        //    MyDetailGrid(V_no, Start_Date, End_Date);

        //    return false;
        //}

    </script>
</asp:Content>
