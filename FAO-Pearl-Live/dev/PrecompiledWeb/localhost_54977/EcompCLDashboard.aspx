<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="EcompCLDashboard, App_Web_ju5zdh34" viewStateEncryptionMode="Always" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <%--<script type="text/javascript">
        $(document).ready(function () {
            $(".mycontent").addClass("contentsamita");
            $(".mycontent").removeClass("mycontent");
        });
        
    </script>--%>
       <link href="css/dashboardstyle.css" rel="stylesheet" />
     
    <link href="css/dashboardstyle.css" rel="stylesheet" />
         <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
     <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
   <style type="text/css">
      
        .k-grid-toolbar
        {
            text-align:right;
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

       .loader {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }
       
     
    </style>
    
    <style type="text/css">
         /* Always set the map height explicitly to define the size of the div
        * element that contains the map. */
         #map {
             height: 100%;
         }
    </style>
    
    <style>
         /* Always set the map height explicitly to define the size of the div
        * element that contains the map. */
         #map {
             height: 100%;
         }
         .section { height:244px;padding: 50px 0px;}
#sub-container { width:200px; height:auto; margin:auto; }
.sub-box {width:200px; height:40px; margin:auto; }
.sub-allcredit {margin:5px 0px;}
.strap {border-bottom:10px solid green;margin:3px 0px 0px;
}
.sub-icon { font-family:Arial, Helvetica, sans-serif; text-align:left; font-size:13px; color:#717171; float:left;}
.sub-number {width:40px; height:22px;background:green;float:right; text-align:center;color:#fff;}

    </style>

    <div id="wrapper">
         
        
        
       
        <div class="controllers_dashboard"> 
           <label>From Date :</label>        
      <asp:TextBox ID="fromdtpicker" runat="server" ></asp:TextBox>     
            
        </div>

        <div class="controllers_dashboard"> 
           <label>To Date :</label>             
             <asp:TextBox ID="todtpicker" runat="server"></asp:TextBox>
        </div>

        <div class="controllers_dashboard">
            <asp:HiddenField ID="hdnddlCompany" Value="0" runat="server" />
           <label id="lblcomp" runat="server" >Company :</label>                    
              <asp:DropDownList ID="ddlCompany" runat="server"></asp:DropDownList>
        </div>
           <div class="btn">          
                <input ID="Text1" value="Submit" style="background:#f47a53;border:1px solid #f47a53; width:100px;padding:5px;border-radius:3px;color:#fff;text-align:center;cursor:pointer;font-weight:bold;" onclick="btnclick()" />
           </div>
        <div class="clear"></div>

      <div class="mainlayout_part1">
            <h2 class="headicon">Need To Act</h2>
            <div class="box">
                <div class="icon"><img src="images/act.png"></div>
                <div class="num" >
                  <a id="lblneedtoact" href="javascript:void(0);" style="color: #fff; text-decoration:none;" ">0</a>   </div>
                <div class="clear"></div>
                <h4>Need To Act</h4>
            </div>
            <div class="box" style="background:#60bcff;">
                <div class="icon"><img src="images/req.png"></div>
                <div class="num"  ><a id="lblMyRequest" href="javascript:void(0);" style="color: #fff; text-decoration:none;" ">0</a></div>
                <div class="clear"></div>
                <h4>My Request</h4>
            </div>
            <div class="box" style="background:#4a366d;">
                <div class="icon"><img src="images/hist.png"></div>
                <div class="num" ><a id="lblHistory" href="javascript:void(0);" style="color: #fff; text-decoration:none;" ">0</a></div>
                <div class="clear"></div>
                <h4>History</h4>
            </div>
            <div class="box" style="background:#f47a53;">
                <div class="icon"><img src="images/due.png"></div>
                <div class="num" id="lblDocumentDue" style="cursor:pointer;" onclick="lblClick('Due')">3</div>
                <div class="clear"></div>
                <h4>Document Due</h4>
            </div>
            <div class="box" style="background:#13a08b;">
                <div class="icon"><img src="images/days.png"></div>
                <div class="num" id="lblDuein5" style="cursor:pointer;" onclick="lblClick('Duein5')">50</div>
                <div class="clear"></div>
                <h4>Due in 5Days</h4>
            </div>
            <div class="box" style="background:rgb(179, 30, 30);">
                <div class="icon"><img src="images/expired.png"></div>
                <div class="num" id="lblExpiredSLA" style="cursor:pointer;" onclick="lblClick('ExpiredSLA')">25</div>
                <div class="clear"></div>
                <h4>Expired SLA</h4>
            </div>
        </div>
      <%--  <div class="mainlayout_part1 layout_er"  id="chartwrapper" >
            <h2 class="headicon_comp">Company Wise</h2>
                       
                <div id="dvpiechart" style="width:50%;"></div>
          
        </div>--%>
        <div class="mainlayout_part1 layout_er" id="chartwrapper">
            <h2 class="headicon_comp">Company Wise</h2>
               <div id="ldrCompany" class="loader"></div>
            <div id="dvPNoData" class="gap" style="margin:auto;text-align:center;">
                    No Data found
                </div>
            <div id="sitewisewrapper" class="gap">
            Performed<br />
            <div id="dvper" style="width: 100%;"></div>
           Not Performed<br />
            <div id="dvpafterdue" style="width: 100%;"></div>
            In Process<br />
            <div id="dvinProcess" style="width: 100%;"></div>
           
            </div>
        </div>

        <div class="clear"></div>

        <div class="mainlayout_part1">
          <h2 class="headicon_act">Act Wise</h2>
             <div id="ldrActwise" class="loader"></div>
            <div class="actwise_container" id="dvActwise"></div>
        </div>
        
        <div class="mainlayout_part1 layout_er">
          <h2 class="headicon_site">Site Map View</h2>
                 <div id="ldrSitewise" class="loader"></div>
          <div class="actwise_container" style="height:400px;">
              <div id="map"></div>
          </div>  
        </div>
    </div>
    <div id="dvpopupgrid" style="display:none;background: center no-repeat url('kendu/content/shared/styles/world-map.png');text-align:center;"  >
           <div id="ldrdvGrid" class="loader"></div>
           <div id="dvgrid"></div>
         </div>
    <div id="dvPopupCGrid" style="display:none; text-align:left;">
           <div id="kgrd"></div>
    </div>

     <div id="dvpopupgridhomedet" style="display:none;background: center no-repeat url('kendu/content/shared/styles/world-map.png');text-align:center;"  >
           <div id="ldrdvGridhomedet" class="loader"></div>
           <div id="dvGridHomedet"></div>
         </div>
     <script type="text/javascript">

         $(document).ready(function () {
             $("#<%=fromdtpicker.ClientID%>").kendoDatePicker({

                 start: "year",


                 depth: "year",


                 format: "MMMM yyyy",

             });
             $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker").value(new Date("October 1, 2016"));
             $("#<%=todtpicker.ClientID%>").kendoDatePicker({
                 // defines the start view
                 start: "year",
                 // defines when the calendar should return date
                 depth: "year",
                 // display month and year in the input
                 format: "MMMM yyyy"
             });
             $('#<%=todtpicker.ClientID%>').data("kendoDatePicker").value(new Date("November 30, 2016"));


           
             BindHomeDet();
             BindPieChart();
             BindChart();
             
             //  Bindcompddl();
             // $("#fromdtpicker").datepicker();
             //{ dateFormat: 'dd/mm/yy' }
             //$("#fromdtpicker").kendoDatePicker({

             //    start: "year",


             //    depth: "year",


             //    format: "MMMM yyyy",

             //});
             //$('#fromdtpicker').data("kendoDatePicker").value(new Date(2016,06));
             //$("#todtpicker").kendoDatePicker({
             //    // defines the start view
             //    start: "year",
             //    // defines when the calendar should return date
             //    depth: "year",
             //    // display month and year in the input
             //    format: "MMMM yyyy"
             //});
             //$('#todtpicker').data("kendoDatePicker").value(new Date());



         });
        </script>
    <script type="text/javascript">
        function lblClick(type) {

            //$("#dvpopupgrid").kendoWindow({
            //    width: "900px",
            //    height: "700px",
            //    modal: true,
            //    title: "Document Details",
            //    visible: false
            //});
            //$("#dvpopupgrid").data("kendoWindow").center().open();
            //BindGridHomeDet(type);
        }
                
        function btnclick()
        {
            BindHomeDet();
            BindPieChart();
            BindChart();
            bindSite();
        }
        function BindHomeDet() {

            $('#<%=fromdtpicker.ClientID%>')
            $('#<%=todtpicker.ClientID%>')
            var Sdatepicker =   $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker");
            var Tdatepicker = $('#<%=todtpicker.ClientID%>').data("kendoDatePicker");
            var SMonth =  kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear =  kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear =  kendo.toString(Tdatepicker.value(), 'yyyy');
            // var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            var Compid = $("#ContentPlaceHolder1_ddlCompany").val();
            if (Compid == undefined)
                Compid = $("#ContentPlaceHolder1_hdnddlCompany").val();
            // var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var str = '{"CompanyID":"' + Compid + '","Uid":"' + UID + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var winwidth = $(window).width() - 100;
            $.ajax({
                type: "POST",
                url: "EcompCLDashboard.aspx/GetHomeDet",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    var Data = JSON.parse(res.d);
                    $("#lblneedtoact").html(Data[0]["Need_To_Act"]);
                    $("#lblMyRequest").html(Data[0]["My_Requests"]);
                    $("#lblHistory").html(Data[0]["History"]);
                    $("#lblDocumentDue").html(Data[0]["Due"]);
                    $("#lblDuein5").html(Data[0]["Duein5"]);
                    $("#lblExpiredSLA").html(Data[0]["Expired_SLA"]);
                },

                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrdv" + Compid).hide();
                },
                failure: function (response) {
                    alert("failure in chart bind");
                    $("#ldrdv" + Compid).hide();

                }
            });

        }

        function BindChart() {
            $('#<%=fromdtpicker.ClientID%>')
            $('#<%=todtpicker.ClientID%>')
            var Sdatepicker = $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker");
            var Tdatepicker = $('#<%=todtpicker.ClientID%>').data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            var Compid = $("#ContentPlaceHolder1_ddlCompany").val();
            if (Compid == undefined)
                Compid = $("#ContentPlaceHolder1_hdnddlCompany").val();
            var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var winwidth = $(window).width() - 100;
            var strid = "#dvActwise";
            $.ajax({
                type: "POST",
                url: "EcompCLDashboard.aspx/GetActwiseChart",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {

                    var Data = res.d;
                    var Series = Data.series1;
                    var Category = Data.categoryAxis;
                    var width = 200 + 100 * Category.length;
                    if (width < winwidth)
                    { width = winwidth; }
                    $(strid).empty();
                    if (Category.length == 0) {


                        if ($(strid).data("kendoChart") != undefined) {
                            $(strid).data("kendoChart").destroy();
                        }

                    }
                    else {

                        if ($(strid).data("kendoChart") != undefined) {
                            $(strid).data("kendoChart").destroy();
                            $(strid).empty();
                        }
                        $(strid).kendoChart({
                            title: {

                            },
                            chartArea: {

                                height: 400
                            },
                            legend: {
                                position: "bottom",
                                visible: true,
                            },
                            seriesDefaults: {
                                type: "column",
                                stack: true

                            },
                            majorGridLines: {
                                visible: false,
                            },
                            series: Series,
                            categoryAxis: {
                                categories: Category,
                                labels: {
                                    rotation: -90,
                                    template: "#=value.split(':,')[0]#",
                                }
                            },

                            valueAxis: {

                                labels: {
                                    format: "{0}"
                                },
                                line: {
                                    visible: false
                                }
                            },
                            tooltip: {
                                visible: true,
                                template: "#= series.name # Percentage: #=kendo.format('{0:n2}', (value/category.split(':,')[2]) *100 ) #% <br/> #= series.name # Count : #= value#  "
                            },
                            seriesClick: function seriesclick(e) {
                                $("#ldrdvGrid").show();
                                $("#dvGrid").empty();
                                BindGrid(1118586, e.category, e.series.name);
                                $("#dvpopupgrid").kendoWindow({
                                    width: "900px",
                                    height: "700px",
                                    modal: true,
                                    title: "Document Details",
                                    visible: false
                                });
                                $("#dvpopupgrid").data("kendoWindow").center().open();
                            },
                        });
                    }
                },
                complete: function comp() {

                    $("#ldrActwise").hide();
                },
                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrActwise").show();
                },
                failure: function (response) {
                    alert("failure in chart bind");
                    $("#ldrActwise").show();

                }
            });

        }

        var PerProgress = "";
        var PerafterDueProgress = "";
        var InprocessProgress = "";

        function BindPieChart() {
            $('#<%=fromdtpicker.ClientID%>')
            $('#<%=todtpicker.ClientID%>')
            var Sdatepicker = $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker");
            var Tdatepicker = $('#<%=todtpicker.ClientID%>').data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            var Compid = $("#ContentPlaceHolder1_ddlCompany").val();
            if (Compid == undefined)
                Compid = $("#ContentPlaceHolder1_hdnddlCompany").val();
            var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            var width = $("#chartwrapper").width() - 50;
            $.ajax({
                type: "POST",
                url: "EcompCLDashboard.aspx/GetPieChart",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    //dvper dvpafterdue dvinProcess dvidelayed
                    var data = res.d;
                    if (data == "") {
                        $("#sitewisewrapper").hide();
                        return;
                    }
                    else {
                        
                        $("#dvPNoData").hide();
                        $("#sitewisewrapper").show();
                    }
                    if (InprocessProgress == "") {
                        PerProgress = $("#dvper").kendoProgressBar({
                            max: 100
                        }).data("kendoProgressBar");

                        PerafterDueProgress = $("#dvpafterdue").kendoProgressBar({
                            type: "value",
                            max: 100
                        }).data("kendoProgressBar");

                        InprocessProgress = $("#dvinProcess").kendoProgressBar({
                            type: "value",
                            max: 100
                        }).data("kendoProgressBar");
                    }
                    PerProgress.progressWrapper.css({
                        "background-image": "none",
                        "border-image": "none"
                    });
                    PerProgress.value(data[0].value);
                    PerafterDueProgress.value(data[1].value);
                    InprocessProgress.value(data[2].value);
                    PerafterDueProgress.progressWrapper.css({
                        "background-image": "none",
                        "border-image": "none"
                    });
                    InprocessProgress.progressWrapper.css({
                        "background-image": "none",
                        "border-image": "none"
                    });

                    PerProgress.progressWrapper.css({
                        "background-color": "green",
                        "border-color": "green"
                    });


                    PerafterDueProgress.progressWrapper.css({
                        "background-color": "red",
                        "border-color": "red"
                    });

                    PerProgress.progressStatus.text(data[0].value + "% (Count :" + data[0].count + ")");
                    PerafterDueProgress.progressStatus.text(data[1].value + "% (Count :" + data[1].count + ")");
                    InprocessProgress.progressStatus.text(data[2].value + "% (Count :" + data[2].count + ")");



                    InprocessProgress.progressWrapper.css({
                        "background-color": "#FF9900",
                        "border-color": "#FF9900"
                    });
                },
                complete: function comp() {
                    //$("#sitewisewrapper").show();
                    $("#ldrCompany").hide();
                },
                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrCompany").hide();
                },
                failure: function (response) {
                    alert("failure in chart bind");
                    $("#ldrCompany").hide();

                }
            });

        }


        function BindGridHomeDet(Type) {
             $('#<%=fromdtpicker.ClientID%>')
            $('#<%=todtpicker.ClientID%>')
            var Sdatepicker =   $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker");
            var Tdatepicker = $('#<%=todtpicker.ClientID%>').data("kendoDatePicker");
            var SMonth =  kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear =  kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear =  kendo.toString(Tdatepicker.value(), 'yyyy');
            // $("#ldrdvSite").hide();
            //CompanyID As String, SubType As String, uid As String
            var UID = '<%= Session("UID").ToString()%>';
            var Compid = $("#ContentPlaceHolder1_ddlCompany").val();
            if (Compid == undefined)
                Compid = $("#ContentPlaceHolder1_hdnddlCompany").val();
            var str = '{"CompanyID":"' + Compid + '","SubType":"' + Type + '","uid":"' + UID + '"}';
            $.ajax({
                type: "POST",
                url: "EcompCLDashboard.aspx/GetGridHomeDet",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    var grid = $("#dvGridHomedet").data("kendoGrid");
                    // detach events
                    if (grid != undefined) {
                        grid.destroy(); // destroy the Grid
                        $("#dvGridHomedet").empty();
                    }
                    if (data.d.length == 0) {

                        alert("No Record Found!");
                        return;

                    }
                    var StrData = $.parseJSON(data.d.data);
                    if (StrData.length == 0) {
                        alert("No Record Found!");
                        return;

                    }
                    var columns = data.d.columns;


                    $("#dvGridHomedet").kendoGrid({
                        dataSource: {
                            data: StrData,
                        }
                    ,
                        height: 600,
                        scrollable: true,
                        resizable: true,
                        reorderable: true,
                        groupable: true,
                        sortable: true,
                        filterable: true,
                        toolbar: [{ name: "excel" }],
                        excel: {
                            fileName: "Report_Company.xlsx",
                            filterable: true
                        },
                        columns: columns
                    });
                    $("#ldrdvGridhomedet").hide();
                },
                complete: function comp() {

                    $("#ldrdvGridhomedet").hide();
                },
                error: function (data) {
                    alert(data.error);
                    $("#ldrdvGridhomedet").hide();
                }

            });
        }

        function BindGrid(CompanyID, ActID, Status) {

            $('#<%=fromdtpicker.ClientID%>')
            $('#<%=todtpicker.ClientID%>')
            var Sdatepicker = $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker");
            var Tdatepicker = $('#<%=todtpicker.ClientID%>').data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            // $("#ldrdvSite").hide();
            var actid1 = ActID.split(":,")[1];
            var Compid = $("#ContentPlaceHolder1_ddlCompany").val();
            if (Compid == undefined)
                Compid = $("#ContentPlaceHolder1_hdnddlCompany").val();
            var str = '{"CompanyID":"' + Compid + '","ActID":"' + actid1 + '","Status":"' + Status + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            $.ajax({
                type: "POST",
                url: "EcompCLDashboard.aspx/GetActclickGrid",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    var grid = $("#dvgrid").data("kendoGrid");
                    // detach events
                    if (grid != undefined) {
                        grid.destroy(); // destroy the Grid
                        $("#dvgrid").empty();
                    }
                    if (data.d.length == 0) {
                        alert("No Record Found!");
                        return;

                    }
                    var StrData = $.parseJSON(data.d.data);
                    var columns = data.d.columns;


                    $("#dvgrid").kendoGrid({
                        dataSource: {
                            data: StrData,
                        }
                    ,
                        height: 600,
                        scrollable: true,
                        resizable: true,
                        reorderable: true,
                        groupable: true,
                        sortable: true,
                        filterable: true,
                        toolbar: [{ name: "excel" }],
                        excel: {
                            fileName: "Report_Company.xlsx",
                            filterable: true
                        },
                        columns: columns
                    });
                    $("#ldrdvGrid").hide();
                },
                complete: function comp()
                { $("#ldrdvGrid").hide(); },
                error: function (data) {
                    alert(data.error);
                    $("#ldrdvGrid").hide();
                }

            });
        }

    </script>
    <script type="text/javascript">
        function GetData(type) {
            //alert("type");
            GetColumnGrid("Act Document",type);
            $("#dvPopupCGrid").kendoWindow({
                width: "1300px",
                height: "450px",
                modal: true,
                title: type,
                visible: false
            });
            $("#dvPopupCGrid").data("kendoWindow").center().open();
            $("#dvPopupCGrid_wnd_title").html(type);
        }

        function GetColumnGrid(DocumentType,type) {
            $("#dvloader,#dvloader1,#dvloader2").show();
            $("#kgrd,#kgrdMyReq,#kgrdHistory").hide();
            var t = '{ documentType: "' + DocumentType + '" }';
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetColumn",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    if ($(kGrid).length > 0) {
                        //$('#kgrd').data().kendoGrid.destroy();
                        $('#kgrd').empty();
                   }
                    //if ($(kgrdMyReq).length > 0) {
                    //    $('#kgrdMyReq').data().kendoGrid.destroy();
                    //    $('#kgrdMyReq').empty();
                    //}
                    //if ($(kgrdHistory).length > 0) {
                    //    $('#kgrdHistory').data().kendoGrid.destroy();
                    //    $('#kgrdHistory').empty();
                    //}
                    var strData = response.d.Data;
                    var CommanObj = { command: [{ name: "Details", text: "View Details", title: 'View Details', click: DetailHandler }], title: "Action", width: 120 }
                    var Columns = response.d.Column;
                    Columns.splice(0, 0, CommanObj);
                    if (Columns.length > 0) {
                        BindGridData(DocumentType, Columns, type);

                        //if (filtersNeedToAct != null) {
                        //    $("#kgrd").data("kendoGrid").refresh();
                        //    $("#kgrd").data("kendoGrid").dataSource.filter(filtersNeedToAct);
                        //}
                        //if (filterMyReq != null) {
                        //    $("#kgrdMyReq").data("kendoGrid").refresh();
                        //    $("#kgrdMyReq").data("kendoGrid").dataSource.filter(filterMyReq);
                        //    //alert('Hi');
                        //}
                        //if (filterHistory != null) {
                        //    $("#kgrdHistory").data("kendoGrid").refresh();
                        //    $("#kgrdHistory").data("kendoGrid").dataSource.filter(filterHistory);
                        //    //alert('Hi');
                        //}
                        $("#dvloader,#dvloader1,#dvloader2").hide();
                        $("#kgrd,#kgrdMyReq,#kgrdHistory").show();
                    }
                    else {
                        $("#kgrd").hide();
                        $("#NoRecord1").show();

                    }

                    $("#dvloader,#dvloader1,#dvloader2").hide();
                    $("#kgrd,#kgrdMyReq,#kgrdHistory").show();
                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }
        var kGrid;
        function BindGridData(documentType, Columns,type) {
            //alert(type);
            var t = '{ DocumentType: "' + documentType + '" }';
            var url1 = "", fileName1 = "";
            if (type == "Need To Act") {
                url1 = 'Home.aspx/GetNeedToAct';
                fileName1 = "RptNeedToAct.xlsx";
            }
            else if (type == "My Request") {
                url1 = 'Home.aspx/GetDataMyRequestGrid';
                fileName1 = "RptMyRequest.xlsx";
            }
            else if (type == "History") {
                url1 = 'Home.aspx/GetDataHistory';
                fileName1 = "RptHistory.xlsx";
            }
            
            kGrid = $("#kgrd").kendoGrid({
                toolbar: ['excel'],
                excel: {
                    fileName: fileName1,
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: Columns,
                dataSource: {
                    toolbar: ['excel'],
                    excel: {
                        fileName: fileName1,
                        filterable: true,
                        pageable: true,
                        allPages: true
                    },
                    type: "json",
                    transport: {
                        read: {
                            url: url1,
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8"
                        },
                        parameterMap: function (data, type) {
                            return JSON.stringify({
                                documentType: documentType,
                                page: data.page,
                                pageSize: data.pageSize,
                                skip: data.skip,
                                take: data.take,
                                sorting: data.sort === undefined ? null : data.sort,
                                filter: data.filter === undefined ? null : data.filter
                            });
                        },

                    },
                    schema: {
                        data: function (data) {
                            return $.parseJSON(data.d.Data) || [];
                        },
                        total: function (data) {
                            $("#spnNeedToAct").html("(" + data.d.total + ")");
                            //if (data.d.length > 0) {
                            return data.d.total || [];
                            //}
                        }
                    },
                    pageSize: 20,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                },
                columns: Columns,
                sortable: true,
                resizable: true,
                height: 400,
                width: 1386,
                noRecords: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                scrollable: true,
                filterable:true,
                //filterable: {
                //    mode: "row",
                //},
                sortable: {
                    mode: "multiple"
                },
                pageable: {
                    pageSizes: true,
                    refresh: true
                },
            });
            $(".k-grid-Details").attr('title', "Show Details");
            $(".k-button").click(function () {
                $(".k-grid-Details").attr('title', "Show Details");
            });
        }
        var DetailHandler = function DetailHandler(e) {
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            OpenWindow('DocDetail.aspx?DOCID=' + dataItem.DocDetID + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }
    </script>


    <script type="text/javascript">
        var sites = [];
        var markers = [];
        var map;
        var contentString = "";
        function drop() {
            var v1 = sites;
            clearMarkers();
            for (var i = 0; i < sites.length; i++) {
                addMarkerWithTimeout(sites[i], i * 200);
            }
        }
        function initMap() {
            //20.5937° N, 78.9629°
            var Country = { lat: 20.5937, lng: 78.9629 };
            map = new google.maps.Map(document.getElementById('map'), {
                zoom: 4,
                center: Country
                //center: { lat: 52.520, lng: 13.410 }
            });
            //drop();
            bindSite();
        }

        function addMarkerWithTimeout(obj, timeout) {
            var position = { lat: obj.lat, lng: obj.lng };
            window.setTimeout(function ()
            {
                //{"SiteID":"1113227","Site":"Bengaluru","Delayed":0,"Performed":3,"Performed After Due Date":0,"latlong":"12.940726, 77.695473"}
                var contentString = '<div class="section">' +
                    '<div> Site Name ' + obj.Site + '</div>'+
                '<div id="sub-container">' +
                '<div class="sub-box">' +
                '<div class="sub-allcredit"> ' +
                '<div class="sub-icon">Performed </div>' +
                '<div class="sub-number" style="background:green;"> ' + obj.Performed + '</div>' +
                '<div class="clear"></div>' +
                '<div class="strap" style="background:green;"></div>' +
                '</div>' +
                '</div>' +
                '<div class="sub-box">' +
                '<div class="sub-allcredit"> ' +
                '<div class="sub-icon">Not Performed</div>' +
                '<div class="sub-number" style="background:red;">' + obj["Not Performed"] + '</div>' +
                '<div class="clear"></div>' +
                '<div class="strap" style="border-bottom:10px solid red;"></div>' +
                '</div>' +
                '</div>' +
                '<div class="sub-box">' +
                '<div class="sub-allcredit"> ' +
                '<div class="sub-icon">In Process</div>' +
                '<div class="sub-number" style="background:#FF9900;">' + obj.InProcess + '</div>' +
                ' <div class="clear"></div>' +
                '<div class="strap" style="border-bottom:10px solid #FF9900;"></div>' +
                '</div>' +
                                       
                
                
                
                '</div>' +
                '</div>' +
            '</div>';

                //var contentString = "<div>Site Name : " + obj.Site + "<br/>Performed:" + obj.Performed + "<br/>Delayed: " + obj.Delayed + "<br/>Performed After Due Date: " + obj["Performed After Due Date"] + "<div>";
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });
                var marker = new google.maps.Marker({
                    position: position,
                    map: map,
                    animation: google.maps.Animation.DROP,
                    title: obj.Site

                });
                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });
                //infowindow.open(map, marker);
                markers.push(marker);
            }, timeout);
        }

        function clearMarkers() {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];
        }
        function bindSite() {

            $('#<%=fromdtpicker.ClientID%>')
            $('#<%=todtpicker.ClientID%>')
            var Sdatepicker = $('#<%=fromdtpicker.ClientID%>').data("kendoDatePicker");
            var Tdatepicker = $('#<%=todtpicker.ClientID%>').data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            var Compid = $("#ContentPlaceHolder1_ddlCompany").val();
            if (Compid == undefined)
                Compid = $("#ContentPlaceHolder1_hdnddlCompany").val();
            var str = '{"CompanyID":"' + Compid + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            $.ajax({
                type: "POST",
                url: "EcompCLDashboard.aspx/GetSites",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (response) {
                    var res = response.d;
                    if (res != "") {
                        var Site = $.parseJSON(res);

                        $.each(Site, function (i) {

                            var obj = { lat: 0, lng: 0 };
                            if (this.latlong != null) {
                                var arr = this.latlong.split(',');
                                obj.lat = parseFloat(arr[0].trim());
                                obj.lng = parseFloat(arr[1].trim());
                            }

                            obj.Site = this.Site;

                            obj.Performed = this.Performed;
                            obj.InProcess = this.InProcess;
                            obj["Not Performed"] = this["Not Performed"];
                            //latlong
                            //{"SiteID":"1113227","Site":"Bengaluru","Delayed":0,"Performed":3,"Performed After Due Date":0,"latlong":"12.940726, 77.695473"}
                            sites.push(obj);
                        });
                        drop();
                    }
                    $("#ldrSitewise").hide();
                },
                complete: function comp() {
                    $("#ldrSitewise").hide();
            },
            error: function (err) {
               
                $("#ldrSitewise").hide();
            },
            failure: function (response) {
                alert("failure in chart bind");
                $("#ldrSitewise").hide();

            }
                //Ajax call end here 
            });
        }
    </script>
    
    <script async defer 
            src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBqCmulejs-VROzzpz_cJsMl7tEDbdIZrw&callback=initMap">
    </script>
    </a>
</asp:Content>

