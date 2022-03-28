<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="DashboardEcomp, App_Web_iqn0gzeb" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
    <br />
    <br />
    <div style="width:100%;text-align:center;font-size:x-large;"><b> ECOMPLIANCE DASHBOARD</b></div>


    <br /><br />
  <div >
       <%--<div style="width:100%;height:50px;">--%>
      
             <div id="lblslide" style="vertical-align:middle;"><img id="imgside" title="Click for filetrs"  src="images/arwiconright.png" width="30px" height="40px" /> </div>
       <div id="dvfilter"   style="display:none;text-align:center;margin-left:31px;margin-right:5%;margin-top:-50px;border-style:inset;border-radius:2px;border-width:2px;background-color:#FFE4CA;box-shadow: 3px 3px 3px grey;">
           
           <table style="width: 100%;">
               <tr>
                   <td width="15%" align="left" style="padding-left: 20px">&nbsp;</td>
                   <td width="30%" align="center">&nbsp;</td>
                   <td width="15%" align="center">&nbsp;</td>
                   <td width="35%" align="center">&nbsp;</td>
                   <td width="5%">&nbsp;</td>
               </tr>
                <tr>
                   <td width="15%" style="padding-left: 20px;" align="left">
                       <b>From (Month-Year) :</b></td>
                   <td width="30%" valign="top" style="height: 24px" align="left">
                       <input id="fromdtpicker"  style="width: 50%" /></td>
                   <td width="15%" valign="top" style="height: 24px">  <b>From (Month-Year) :</b> </td>
                   <td width="35%" valign="top" style="height: 24px" align="left"> <input id="todtpicker"  style="width: 50%" />
                       <input type="button" id ="btnshowRpt" value="Show Report" class="k-button"/>
                   </td>
                   <td width="5%" style="height: 24px"></td>
               </tr>
              
               
               <tr>
                   <td width="15%" align="left" style="padding-left: 20px">&nbsp;</td>
                   <td valign="top" colspan="3" style="width: 55%">
                       &nbsp;</td>
                   <td width="5%">&nbsp;</td>
               </tr>
                <tr>
                   <td width="15%" align="left" style="padding-left: 20px"><b>Company : </b></td>
                   <td valign="top" colspan="3" style="width: 55%">
                       <select id="ddlCompany" style="width:100%;" name="D1">
                       </select>

                   </td>
                   <td width="5%">&nbsp;</td>
               </tr>
              
               
               <tr>
                   <td width="15%" style="padding-left: 20px;" align="left">&nbsp;</td>
                   <td width="30%" valign="top" style="height: 24px" align="left">
                       &nbsp;</td>
                   <td width="15%" valign="top" style="height: 24px">  &nbsp;</td>
                   <td width="35%" valign="top" style="height: 24px" align="left"> &nbsp;</td>
                   <td width="5%" style="height: 24px">&nbsp;</td>
               </tr>
               
           </table>
                    
       </div>
     <%-- </div>--%>
      <br />
      
    <div id="dvChartsComp" style="margin:10px;" >
        
      
    </div>
    <div id="dvpopupSite" style="display:none;background: center no-repeat url('kendu/content/shared/styles/world-map.png');text-align:center;"  >
        <div id="ldrdvSite" class="loader"></div>
        <div id="dvchatSite" ></div>
    </div>
         <div id="dvpopupgrid" style="display:none;background: center no-repeat url('kendu/content/shared/styles/world-map.png');text-align:center;"  >
           <div id="ldrdvGrid" class="loader"></div>
           <div id="dvgrid"></div>
         </div>
      </div>


    <script type="text/javascript">
        $(document).ready(function () {
            Bindcompddl();
           // $("#fromdtpicker").datepicker();
            //{ dateFormat: 'dd/mm/yy' }
            $("#fromdtpicker").kendoDatePicker({
            
                start: "year",

           
                depth: "year",

           
                format: "MMMM yyyy",
                
            });
            $('#fromdtpicker').data("kendoDatePicker").value(new Date(2016,06));
            $("#todtpicker").kendoDatePicker({
                // defines the start view
                start: "year",
                // defines when the calendar should return date
                depth: "year",
                // display month and year in the input
                format: "MMMM yyyy"
            });
            $('#todtpicker').data("kendoDatePicker").value(new Date());

            
        });
        $("#lblslide").click(function ()
        {
            if ($("#imgside").attr('src') == 'images/arwiconright.png')
            { $("#imgside").attr('src','images/arwiconleft.png');}
            else
            { $("#imgside").attr('src', 'images/arwiconright.png'); }

            $("#dvfilter").toggle("slide");
           
        });
        
       
        $("#btnshowRpt").click(function () {
            var data = $("#ddlCompany").data("kendoMultiSelect").dataSource._data;
            $.each(data, function (i, item) {
                var strid = "#dvchartcmp" + item.tid;
                $(strid).css("height", '');
                $("#ldrdv" + item.tid).show();
                BindChart(item.tid, item.CompanyName, "", "");
            });
        });
         
        function Bindcompddl()
        {
           
            var Tdatepicker = $("#datepicker").data("kendoDatePicker");
            $("#ddlCompany").kendoMultiSelect({
                dataTextField: "CompanyName",
                dataValueField: "tid",
                dataSource: {
                    serverFiltering: true,
                    transport: {
                        read: function (options) {

                            var Site = '';
                            var UID = '<%= Session("UID").ToString()%>';
                            var URole = '<%= Session("USERROLE").ToString()%>';
                            
                            var d = "{'str':'" + Site + "','uid':'" + UID + "', 'urole':'" + URole + "'}";
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: "EcomplianceReport.aspx/GetCompany",
                                global: false,
                                data: d,
                                dataType: "json",
                                async: true,
                                success: function (msg) {
                                    var Data = $.parseJSON(msg.d);
                                    options.success(Data);
                                    $.each(Data, function (i, item) {
                                        var winwidth = $(window).width()-100;
                                        var str = " <div id='dvchartswrapper" + item.tid + "' style='width:" + winwidth + "px;box-shadow: 3px 3px 3px grey;border-style:inset;border-color:black;margin:10px;'> <div class='loader' id='ldrdv" + item.tid + "'></div><div id='headercomp" + item.tid + "' style='width:100%;text-align:center;'></div> <div style='width:" + winwidth + "px;overflow:auto; ' > <div id='dvchartcmp" + item.tid + "'  ></div> </div></div>";
                                        $("#dvChartsComp").append(str);
                                    });
                                },
                                complete: function (data) {
                                    var data = $("#ddlCompany").data("kendoMultiSelect").dataSource;
                                    
                                    $.each(data._data, function (i, item) {
                                        BindChart(item.tid, item.CompanyName,"", "");
                                    });
                                    

                                }
                            });
                        }
                    }
                },
                 dataBound: function dbound(e) {
                     var data = e.sender.dataSource._data;
                     var ds = e.sender.dataSource;
                     var values = [];
                     $.each(data, function (i, item) {
                         values.push(item.tid);
                     });
                     this.value(values);
                     this._savedOld = this.value();
                    
                 },
                
                 select: function select(e) {
                     var dataItem = this.dataItem(e.item.index());
                     $("#ldrdv" + dataItem.tid).show();
                     BindChart(dataItem.tid,dataItem.CompanyName, "", "");

                 },
                 change: function (e) {
                     var previous = this._savedOld;
                     var current = this.value();
                     var diff = [];
                     if (previous) {
                         diff = $(previous).not(current).get();
                         if (diff.length > 0) {
                             var strid = "#dvchartcmp" + diff[0];
                             if ($(strid).data("kendoChart") != undefined) {
                                 $(strid).data("kendoChart").destroy();
                                
                             }
                             $(strid).empty();
                             $(strid).height(0);
                             $("#headercomp" + diff[0]).html('');
                             $("#dvchartswrapper" + diff[0]).css("border-style", '');
                         }
                     }
                     this._savedOld = current.slice(0);
                 },
                 close: function close(e)
                 {
                     var data = e.sender.dataSource._data;
                     var ds = e.sender.dataSource;
                     var values = [];
                     $.each(data, function (i, item) {
                       
                     });

                 }
            });
            return true;
        }
        
        function BindChart(Compid, CompNm, Month, Year) {
            var Sdatepicker = $("#fromdtpicker").data("kendoDatePicker");
            var Tdatepicker = $("#todtpicker").data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            var str = '{"CompanyID":"' + Compid + '","SMonth":"'+SMonth+'","SYear":"'+SYear+'","TMonth":"'+TMonth+'","TYear":"'+TYear+'"}';
          
            var winwidth = $(window).width() - 100;
            $.ajax({
                type: "POST",
                url: "DashboardEcomp.aspx/GetCompChart",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                  
                    var Data = res.d;
                    var Series = Data.series1;
                    if (Series.length == 0) {
                        
                    }
                    var Category = Data.categoryAxis;
                    var width = 200 + 100 * Category.length;
                    if (width < winwidth)
                    { width = winwidth; }
                    $(strid).empty();
                    if (Category.length == 0)
                    {
                        //$("#headercomp" + Compid).html('');
                       // $("#dvchartswrapper" + Compid).css("border-style", '');
                        $("#ldrdv" + Compid).hide();
                        var strid = "#dvchartcmp" + Compid;
                        if ($(strid).data("kendoChart") != undefined) {
                            $(strid).data("kendoChart").destroy();
                           
                           
                        }

                        $("#dvchartswrapper" + Compid).css("border-style", 'inset');
                        $("#headercomp" + Compid).html("<h1 style='text-shadow: 2px 2px 4px ;'>For: " + CompNm + "</h1>");
                        $(strid).html('<label><b> No Record Found ! </b> </label>');
                    }
                    else{
                        var strid = "#dvchartcmp" + Compid;
                        $("#dvchartswrapper" + Compid).css("border-style", 'inset');
                        $("#headercomp" + Compid).html("<h1 style='text-shadow: 2px 2px 4px ;'>For: " + CompNm + "</h1>");
                        if ($(strid).data("kendoChart") != undefined)
                        {
                        $(strid).data("kendoChart").destroy();
                        $(strid).empty();
                        }
                    $(strid).kendoChart({
                        title: {
                            
                        },
                        chartArea: {
                            width: width,
                            height: 400
                        },
                        legend: {
                            position: "top",
                            visible: true,
                        },
                        seriesDefaults: {
                            type: "column"
                        },
                        series: Series,
                        categoryAxis: {
                            categories: Category,
                           
                            labels: {
                                rotation: -90,
                                template: "#= value.split(':,')[0] #",
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
                            //format: "{0}%",
                            template: "#= series.name #: #= value # %"
                        },
                                            
                        seriesClick: function seriesclick(e)
                        {
                            $("#ldrdvSite").show();
                            $("#dvchatSite").empty();
                            BindchartSite(Compid, e.category,e.series.name);
                            $("#dvpopupSite").kendoWindow({
                                width: "900px",
                                height: "600px",
                                modal: true,
                                title: "Vehicles Details",
                                visible: false
                            });
                            $("#dvpopupSite").data("kendoWindow").center().open();
                        },
                       
                    });
                    $("#ldrdv"+ Compid).hide();
                    
                           }     
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

        function BindchartSite(Compid,siteid,status)
        {
            var Sdatepicker = $("#fromdtpicker").data("kendoDatePicker");
            var Tdatepicker = $("#todtpicker").data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            var siteid1 = siteid.split(":,")[1];
            var str = '{"CompanyID":"' + Compid + '","SiteID":"' + siteid1 + '","Status":"' + status + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            
       
            $.ajax({
                type: "POST",
                url: "DashboardEcomp.aspx/GetSiteChart",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {

                    var Data = res.d;
                    var Series = Data.series1;
                    if (Series.length == 0) {

                    }
                    var Category = Data.categoryAxis;

                    var width = 200 + 100 * Category.length;
                   
                    if (Category.length == 0) {
                        
                     
                    }
                    else {
                        
                        $("#dvchatSite").kendoChart({
                            title: {
                                text:"Chart Act wise"
                            },
                            
                            legend: {
                                position: "top",
                                visible: true,
                            },
                            chartArea: {
                                width: 780,
                                height: 500
                            },
                            seriesDefaults: {
                                type: "column"
                            },
                            series: Series,
                            categoryAxis: {
                                categories: Category,
                              
                                labels: {
                                    template: "#=value.split(':,')[0]#",
                                    rotation: -90
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
                                //format: "{0}%",
                                template: "#= series.name #: #= value #"
                            },

                            seriesClick: function seriesclick(e) {
                                $("#ldrdvGrid").show();
                                $("#dvGrid").empty();
                                BindGrid(Compid, siteid1, e.category, e.series.name);
                                $("#dvpopupgrid").kendoWindow({
                                    width: "900px",
                                    height: "700px",
                                    modal: true,
                                    title: "Document Deatails",
                                    visible: false
                                });
                                $("#dvpopupgrid").data("kendoWindow").center().open();
                            },

                        });
                    }
                },
                complete:function comp()
                { $("#ldrdvSite").hide(); },
                error: function (err) {
                    alert("error in chartbind");
                    $("#ldrdvSite").hide();
                },
                failure: function (response) {
                    alert("failure in chart bind");

                }
            });
           // $("#ldrdvSite").hide();
        }


        function BindGrid(CompanyID, SiteID,ActID,Status)
        {
            var Sdatepicker = $("#fromdtpicker").data("kendoDatePicker");
            var Tdatepicker = $("#todtpicker").data("kendoDatePicker");
            var SMonth = kendo.toString(Sdatepicker.value(), 'MM');
            var TMonth = kendo.toString(Tdatepicker.value(), 'MM');
            var SYear = kendo.toString(Sdatepicker.value(), 'yyyy');
            var TYear = kendo.toString(Tdatepicker.value(), 'yyyy');
            $("#ldrdvSite").hide();
            var actid1 = ActID.split(":,")[1];
            var str = '{"CompanyID":"' + CompanyID + '","SiteID":"' + SiteID + '","ActID":"' + actid1 + '","Status":"' + Status + '","SMonth":"' + SMonth + '","SYear":"' + SYear + '","TMonth":"' + TMonth + '","TYear":"' + TYear + '"}';
            $.ajax({
                type: "POST",
                url: "DashboardEcomp.aspx/GetdocGrid",
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

</asp:Content>

