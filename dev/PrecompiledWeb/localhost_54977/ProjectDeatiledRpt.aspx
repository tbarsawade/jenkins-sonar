<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="ProjectDeatiledRpt, App_Web_ws3kahym" viewStateEncryptionMode="Always" %>

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

    <div>
        <div id="mask">
            <div id="loader">
                <img src="images/preloader22.gif" alt="" />
            </div>
        </div>
    </div>
     
    <div style="width: 100%;height:20px; padding-bottom: 25px; padding-left: 10px;margin-top:10px;margin-bottom:10px; font: bold 17px 'verdana'; color: #fff; background-color: red;">
         Project Status Report
    </div>
    <div id="MainGridheader" class="basic-grey" style="width: 1000px; border: 1px solid #dddddd; border-radius: 5px;">
    </div>
   
    <div id="grid"></div>
    <div id="popupdiv" style="display:none;" >
         <div class='loader' id="ldrdv"></div>
        <table  style="width:100%;">
               <tr style="height:10px;"><td colspan="5"></td></tr>
            <tr>
                <td style="text-align:left;"><b>Project Name:</b> <span id="spnProject"></span></td>
                <td style="text-align:left;"><span id="spnSPOC"></span></td>
                <td style="text-align:left;"><span id="spnOwner"></span></td>
                <td style="text-align:left;"><span id="spnStatus"></span></td>
                <td style="text-align:left;"><span id="spnEDC"></span></td>
            </tr>
                  <tr style="height:10px;"><td colspan="5"></td></tr>
            <tr>
                <td style="width:100%;" colspan="5">
                    <div id="gvModule" style="width:100%;"></div>
                </td>
            </tr>
        </table>
    </div>

    <div id="popupdiv2" style="display:none;">
         <div class='loader' id="ldrdv2"></div>
        <table style="width:100%;" >
            <tr>
                <td style="text-align:left;"><b>Project Name:</b> <span id="spnProject1"></span></td>
                <td style="text-align:right;"><b>Module Name:</b> <span id="spnModule"></span></td>
            </tr>
            <tr style="height:10px;"><td colspan="2"></td></tr>
            <tr>
                <td style="width:100%;" colspan="2">
                   <div id="gvTeam" style="width:100%;"></div>
                </td>
            </tr>
        </table>
    </div>

    <div id="dvTeam" style="display:none;">
          <div class='loader' id="ldrdv3"></div>
        <table style="width:100%;" >
            <tr>
                <td style="text-align:left;"><b>Project Name:</b><span id="spnProjectTeam"></span></td>
            </tr>
                  <tr style="height:10px;"><td ></td></tr>
            <tr>
                <td style="width:100%;">
                   <div id="gvTeamDtl" style="width:100%;"></div>
                </td>
            </tr>
        </table>
    </div>
    <style type="text/css">
        .water {
            font-family: Tahoma, Arial, sans-serif;
            font-style: italic;
            color: gray;
        }


        .style2 {
            width: 100%;

            height: 73px;
        }
        .loader {
            background: url("images/preloader22.gif") no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }
    </style>
    <style type="text/css">
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
        .k-grid table tr:hover 
        
        {
            background: rgb(69, 167, 84);
            color: black;

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
        /*.link {
            color: #35945B;
        }

            .link:hover {
                font: 15px;
                color: green;
                background: yellow;
                border: solid 1px #2A4E77;
            }*/
         .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }
        .k-grid td
        {
             text-align:left;
        }

         .k-grid-toolbar
        {
            text-align:right;
        }
    </style>
    <script type="text/javascript">
        $(document).ajaxStart(
            function () {
                $("#mask").css("display", "block");
                $("#loader").css("display", "block");
            }
            );
        $(document).ajaxComplete(function () {
            $("#mask").css("display", "none"); $("#loader").css("display", "none");
        });

        $(document).ready(function () {
            ProjectGrid();
        });

        var PrjName = "";

        function ShowModuleDetails(ProjectNm, pname,SPOC,Owner,Status,EDC) {
            PrjName = pname;
            $("#ldrdv").show();
            $("#popupdiv").kendoWindow({
                width: "900px",
                height: "500px",
                modal: true,
                title: "Module Wise Detailed Report",
                visible: false
            });
            $("#gvModule").html("<div class='loader' style='width:100%;' ></div>");
            $("#popupdiv").data("kendoWindow").center().open();
            //spnProject
            $("#spnProject").html("<b> "+pname +"</b>");
            $("#spnSPOC").html("<b> SPOC Name : " + SPOC +"</b>");
            $("#spnOwner").html(" <b> Project Owner : " + Owner + "</b>");
            $("#spnStatus").html(" <b> Status :" + Status + "</b>");
            $("#spnEDC").html("<b> EDC : " + EDC + "</b>");

            MyGridDetail(ProjectNm);
            $("#ldrdv").hide();
            return false;
        }
        function ShowTeamDetails(ProjectNm, pname) {
            PrjName = pname;
            $("#ldrdv3").show();
            //dvTeam spnProjectTeam gvTeamDtl
            $("#dvTeam").kendoWindow({
                width: "900px",
                height: "500px",
                modal: true,
                title: "Team Detailed Report",
                visible: false
            });
            $("#gvTeamDtl").html("<div class='loader' style='width:100%;' ></div>");
            $("#dvTeam").data("kendoWindow").center().open();
            //spnProject
            $("#spnProjectTeam").html("<b>" + pname + "</b>");
            
            MyPTeamDetail(ProjectNm);
            $("#ldrdv3").hide();
            return false;
        }

        function MyPTeamDetail(ProjectNm) {
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
          
            var str = '{"ProjectID":"' + ProjectNm + '","uid":"' + UID + '","urole":"' + URole + '"}';
            ////dvTeam spnProjectTeam gvTeamDtl
            $.ajax({
                type: "POST",
                url: "ProjectDeatiledRpt.aspx/GetJSONTeamP",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    $("#gvTeamDtl").html("");
                    var grid = $("#gvTeamDtl").data("kendoGrid");
                    if (grid != undefined) {
                        grid.destroy();
                        // destroy the Grid
                        // $("#popupdiv").empty();
                    }

                    if (data.d.length == 0)
                    {
                        alert("No Record Found!");
                        return;
                    }
                    var StrData = $.parseJSON(data.d.data);
                    var columns = data.d.columns;
                    $("#gvTeamDtl").kendoGrid({
                        dataSource: {
                            data: StrData,
                            pageSize: 100,
                            aggregate: [{ field: "Total", aggregate: "sum" },
                                { field: "ManHours", aggregate: "sum" },
                                
                              { field: "Completed", aggregate: "average" }
                              , { field: "Remaining", aggregate: "average" }

                            ]
                        },
                        height: 450,
                        scrollable: true,
                        resizable: true,sortable: true,
                        reorderable: true,
                        groupable: false,
                        filterable: true,
                        toolbar: [{ name: "excel", text: "Export to Excel" }],
                        excel: {
                            fileName: "Team wise Report.xlsx",
                            filterable: true,
                            pageable: true,
                            allPages: true
                        },
                        pageable: {
                            buttonCount: 5
                        },
                        columns:
                            [
                            {
                            field: "TeamMemberName",
                            title: "Team Member Name",
                            template: '<span style="float:left">#= TeamMemberName #</span>',
                            footerTemplate: " Grand Total :",
                                width:250,
                            }
                        ,
                        {
                            field: "Total",
                            title: "Total Hours",
                            footerTemplate: " #=sum#",
                            width: 100

                        }
                        , {
                            field: "ManHours",
                            title: "Man Days",
                            footerTemplate: " #=sum#",
                            width: 90

                        },
                        {
                            field: "Completed",
                            title: "Completed(%)",
                            type: "number",
                            format: "{0:n2}",
                            width: 120
                         },
                        {
                            field: "Remaining",
                            title: "Remaining(%)",
                            type: "number",
                            format: "{0:n2}",
                            width: 120
                          
                        }
                        ]

                    }) ;

                },
                error: function (data) {
                    alert(data.error);

                }

            });
        }

        var MName = "";
        function showDetailsTeam(ProjectNm, ModuleID, MName) {
            $("#ldrdv2").show();
            $("#popupdiv2").kendoWindow({
                width: "1150px",
                height: "500px",
                modal: true,
                title: "Task Wise Detailed Report",
                visible: false
            });
            MName = MName;
            $("#popupdiv2").data("kendoWindow").center().open();
            $("#gvTeam").html("<div class='loader' style='width:100%;' ></div>");
            MyGridDetailTeam(ProjectNm, ModuleID);
            $("#spnProject1").html("<b>" +PrjName + "</b>");
            $("#spnModule").html("<b>"+MName + "</b>");
            $("#ldrdv2").hide();
                       
            return false;
        }

        function ProjectGrid() {
            $("#MainGridheader").html("");
            $("#grid").html("");
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            var str = '{"FirstGrid":"' + true + '","uid":"'+UID+'","urole":"'+URole+'"}';
            $.ajax({
                type: "POST",
                url: "ProjectDeatiledRpt.aspx/GetJSON",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    var grid = $("#grid").data("kendoGrid");
                    // detach events
                    if (grid != undefined) {
                        grid.destroy(); // destroy the Grid
                        $("#grid").empty();
                    }
                    if (data.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    var StrData = $.parseJSON(data.d.data);
                    $("#grid").kendoGrid({
                        dataSource: {
                            data: StrData,
                            group: [{ field: "ProjectStatus" }],
                            aggregate: [{ field: "Total", aggregate: "sum" },
                               { field: "ManHours", aggregate: "sum" },]
                            //sort: { field: "ordby" },
                          
                        }
                    , height: 500,
                        scrollable: true,
                        resizable: true,sortable: true,
                        reorderable: true,
                        groupable: false,
                        filterable: true,
                        toolbar: [{ name: "excel" }],
                        excel: {
                            fileName: "Project_Detailed_Rpt.xlsx",
                            filterable: true,
                            // pageable: true,
                            allPages: true
                        },
                        columns: [{
                            field: "ProjectType",
                            title: "Function",
                           width:80

                        }, {
                            field: "ProjectName",
                            title: "Project Name",                          
                            template: '<span title="Click here to get details" style="float:left;text-decoration:underline;cursor:pointer;" onclick="ShowModuleDetails(\'#=ProjectID#\',\'#=ProjectName#\',\'#=SPOCName#\',\'#=ProjectOwner#\',\'#=Status#\',\'#=EDC#\');return false;">#=ProjectName#</span>',
                            groupHeaderTemplate: "Grouped By Name: #= value #",
                            width:230
                  
                            

                        },
                                
                                {
                                    field: "Total",
                                    title: "Total Hours",
                                    footerTemplate: " #=sum#",
                                    width:100

                                },
                                {
                                    field: "ManHours",
                                    title: "Man Days",
                                    footerTemplate: " #=sum#",
                                    width: 90

                                }, {
                                    field: "EDC",
                                    title: "Go Live",
                                    width: 80
                                }
                                ,
                                {
                                    field: "Completed",
                                    title: "Completed(%)",
                                    type: "number",
                                    format: "{0:n2}"
                                    , width: 120

                                },
                                {
                                    field: "Remaining",
                                    title: "Remaining(%)",
                                    type: "number",
                                    format: "{0:n2}"
                                    , width: 120

                                },
                                {
                                    field: "TeamMembers",
                                    title: "Team Members",
                                    template: '<span title="Click here to get details" style="text-decoration:underline;cursor:pointer;" onclick="ShowTeamDetails(\'#=ProjectID#\',\'#=ProjectName#\');return false;">#=TeamMembers#</span>'
                                    , width: 120
                                }
                        ]
                    });
                },
                error: function (data) {
                    alert(data.error);

                }

            });
        }
        function MyGridDetail(ProjectNm) {
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            var str = '{"ProjectID":"' + ProjectNm + '","uid":"' + UID + '","urole":"' + URole + '"}';
            $.ajax({
                type: "POST",
                url: "ProjectDeatiledRpt.aspx/GetJSONModule",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    $("#gvModule").html("");
                    var grid = $("#gvModule").data("kendoGrid");
                    if (grid != undefined) {
                        grid.destroy(); // destroy the Grid
                     
                    }
                    if (data.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    var StrData = $.parseJSON(data.d.data);
                    var columns = data.d.columns;
                    $("#gvModule").kendoGrid({
                        dataSource: {
                            data: StrData,
                            pageSize: 100,
                            aggregate: [{ field: "Total", aggregate: "sum" },
                                { field: "ManHours", aggregate: "sum" },
                                { field: "Completed", aggregate: "average" }
                                , { field: "Remaining", aggregate: "average" }
                                             
                            ]
                        },
                        height: 450,
                        scrollable: true,
                        resizable: true,sortable: true,
                        reorderable: true,
                        groupable: false,
                        filterable: true,
                        toolbar: [{ name: "excel", text: "Export to Excel" }],
                        excel: {
                            fileName: "Module wise Report.xlsx",
                            filterable: true,
                            pageable: true,
                            allPages: true
                        },
                        pageable: {
                            buttonCount: 5
                        },
                        columns: [
                            {
                            field: "ModuleName",
                            title: "Module Name",
                            template: '<span title="Click here to get details" style="float:left;text-decoration:underline;cursor:pointer;" onclick="showDetailsTeam(\'#=ProjectId#\',\'#=ModuleId#\',\'#=ModuleName#\');return false;">#=ModuleName#</span>',
                            footerTemplate: " Grand Total:",
                            width: 300,
                        },

                                {
                                    field: "Total",
                                    title: "Total Hours",
                                    footerTemplate: " #=sum#",
                                    width:100
                                }
                                , {
                                    field: "ManHours",
                                    title: "Man Days",
                                    footerTemplate: " #=sum#",
                                    width: 90

                                },
                        {
                            field: "Completed",
                            title: "Completed(%)",
                            type: "number",
                            format: "{0:n2}",
                            width:120
                            

                        },
                        {
                            field: "Remaining",
                            title: "Remaining(%)",
                            type: "number",
                            format: "{0:n2}",
                            width: 120

                            
                        }
                        ]
                    });

                },
                error: function (data) {
                    alert(data.error);

                }

            });
        }

        function MyGridDetailTeam(ProjectNm, ModuleId) {
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
           
            var str = '{"ProjectID":"' + ProjectNm + '","Moduleid":"' + ModuleId + '","uid":"' + UID + '","urole":"' + URole + '"}';
            $.ajax({
                type: "POST",
                url: "ProjectDeatiledRpt.aspx/GetJSONTeam",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: str,
                success: function (data) {
                    $("#gvTeam").html("");
                    var grid = $("#gvTeam").data("kendoGrid");
                    // detach events
                    if (grid != undefined) {
                        grid.destroy(); // destroy the Grid
                        $("#gvTeam").empty();
                    }
                    if (data.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    var StrData = $.parseJSON(data.d.data);
                    var columns = data.d.columns;
                    $("#gvTeam").kendoGrid({
                        dataSource: {
                            data: StrData,
                            pageSize: 100, aggregate: [{ field: "Total", aggregate: "sum" }, { field: "ManHours", aggregate: "sum" },
                                { field: "Completed", aggregate: "average" }
                                , { field: "Remaining", aggregate: "average" }

                            ]
                        }
                    ,
                    height: 450,
                    scrollable: true,
                    resizable: true,sortable: true,
                    reorderable: true,
                    groupable: false,
                    filterable: true,
                    toolbar: [{ name: "excel", text: "Export to Excel" }],
                    excel: {
                        fileName: "Task wise Report.xlsx",
                        filterable: true,
                        pageable: true,
                        allPages: true
                    },
                        pageable: {
                            buttonCount: 5
                        }, 
                        columns: [{
                            field: "Task",
                            title: "Task",
                            template: '<span style="float:left">#= Task #</span>',
                            width:350,
                            footerTemplate: " Grand Total :"

                        },{
                            field: "UserName",
                            title: "User Name",
                            width: 120
                          
                        }, {
                            field: "EDC",
                            title: "EDC",
                            width: 100
                          
                        }, {
                            field: "ADC",
                            title: "ADC",
                            width: 100
                            
                        },
                                {
                                    field: "Total",
                                    title: "Total Hours",
                                    footerTemplate: " #=sum#",
                                    width:100
                                }
                                , {
                                    field: "ManHours",
                                    title: "Man Days",
                                    footerTemplate: " #=sum#",
                                    width: 90

                                },
                 {
                     field: "Completed",
                     title: "Completed(%)",
                     type: "number",
                     format: "{0:n2}",
                     width: 120
                    
                 },
                    {
                        field: "Remaining",
                        title: "Remaining(%)",
                        type: "number",
                        format: "{0:n2}",
                        width: 120
                        
                    }
                        ]
                    });

                },
                error: function (data) {
                    alert(data.error);

                }

            });
        }





    </script>

</asp:Content>





