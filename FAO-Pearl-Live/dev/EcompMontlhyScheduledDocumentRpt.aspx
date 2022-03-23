<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreen.master" AutoEventWireup="false" CodeFile="EcompMontlhyScheduledDocumentRpt.aspx.vb" Inherits="EcompMontlhyScheduledDocumentRpt" %>
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
        .basic-grey .basic-grey input[type="email"], .basic-grey textarea, .basic-grey select {
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

        .basic-grey .button {
            border: none;
            padding: 9px 25px;
            color: #FFF;
            box-shadow: 1px 1px 5px #B6B6B6;
            border-radius: 3px;
            text-shadow: 1px 1px 1px #9E3F3F;
            cursor: pointer;
        }

        #mask {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.59);
            z-index: 10000;
            height: 50%;
            display: none;
            opacity: 0.9;
            text-align: center;
        }

        #loader {
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

        .loader {
            background: url("images/preloader22.gif") no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .k-grid-toolbar a {
            float: right;
        }

        .k-grid td {
            text-align: left;
        }

        .k-tabstrip-wrapper {
            width: 1000px;
        }

        input#exp {
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
            input#exp:hover, input#exp:focus {
                border: 2px solid #dad9d8;
            }

            #wrap {
            width:98%;
            max-width:98%;
            padding:0px!important;
            margin:0 auto;
            box-sizing:border-box;
            -moz-box-sizing:border-box;
            -webkit-box-sizing:border-box;
        }
        .content {
            width:100%!important;
            max-width:100%;
            box-sizing:border-box;
        }
        .k-tabstrip-wrapper
        {
            width:100%;
        }
    </style>

    <%--<div style="text-align: left; width: 1000px; margin-bottom: 10px; margin-top: 10px;" class="btnNew">
        <b>Document Summarized Report</b>
        <br />
    </div>--%>
    <div style="text-align:left;color:black; height: 33px;" class="btnNew">
        <h1 style="padding-left: 7px; margin:0px;"><b>Monthly Scheduled Dates</b> </h1></div>

    <div class="basic-grey" style="width: 100%; margin-top: 16px; border: 1px solid #dddddd; border-radius: 5px;">
        <asp:HiddenField ID="hdnUID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnurole" runat="server" Value="" />
        <asp:HiddenField ID="hdnEid" runat="server" Value="" />
        <asp:HiddenField ID="hdnMap" runat="server" Value="" />
        <table style="width: 100%; padding: 15px 15px;">
            <tr style="height: 20px;">
                <td colspan="3"></td>
            </tr>
            <tr>
               
                <th style="text-align: center" class="auto-style1">Select Month</th>
                <th style="text-align: center" class="auto-style1">Select Year</th>
                <th class="auto-style1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;</th>
            </tr>
            <tr>
                <td align="center">
                    <asp:DropDownList ID="ddlMonth" Width="150" ClientIDMode="Static" Style="height: 34px;" runat="server">
                        <asp:ListItem Value="1">January</asp:ListItem>
                        <asp:ListItem Value="2">February</asp:ListItem>
                        <asp:ListItem Value="3">March</asp:ListItem>
                        <asp:ListItem Value="4">April</asp:ListItem>
                        <asp:ListItem Value="5">May</asp:ListItem>
                        <asp:ListItem Value="6">June</asp:ListItem>
                        <asp:ListItem Value="7">July</asp:ListItem>
                        <asp:ListItem Value="8">August</asp:ListItem>
                        <asp:ListItem Value="9">September</asp:ListItem>
                        <asp:ListItem Value="10">October</asp:ListItem>
                        <asp:ListItem Value="11">November</asp:ListItem>
                        <asp:ListItem Value="12">December</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="center">
                    <asp:DropDownList ID="ddlYear" Width="150" ClientIDMode="Static" Style="height: 34px;" runat="server">
                    </asp:DropDownList>
                </td>
                <td>
                    <%--<asp:Button ID="btnSearch1" runat="server" Text="Search" OnClientClick="javascript:return Bindreport();"
                        CssClass="button" />--%>
                    <input id="btnSearch" class="btnNew" type="button" value="Search" onclick="BindreportEcomp();" />
                </td>
            </tr>
            <tr style="height: 20px;">
                <td colspan="3">&nbsp;&nbsp;&nbsp; &nbsp;</td>
            </tr>
        </table>

    </div>

    <div style="width: 100%;">
        <div id="mask" >
            <div id="loader">
                <img src="images/preloader22.gif" alt="" />
            </div>
        </div>
    </div>

    <br />
    <br />
    <div id="tabstrip" style="width: 100%;">

        <ul>
            <li class="k-item k-state-default k-first k-tab-on-top k-state-active">Company</li>

            <li>Contractor</li>

        </ul>

        <div style="width: 98%; overflow-x: scroll; max-height: 800px; ">
            <div id="grid" style="width: 100%;">
            </div>
        </div>

        <div style="width: 98%; overflow-x: scroll;">
            <div id="gridcontarctor" style="width: 100%;">
            </div>
        </div>

        <div id="popupdivCreated" style="display: none; text-align: center;">
            <div class='loader' id="ldrdvcreated"></div>
            <table style="width: 100%;">
                <tr>
                    <td style="text-align: left; width: 33%;"><span id="spnCompany1"></span></td>
                    <td style="text-align: center; width: 34%;"><span id="spnSite1"></span></td>
                    <td style="text-align: right; width: 33%; margin-right: 5px;"><span id="spnAct1"></span></td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 10px;"></td>
                </tr>
                <tr>
                    <td style="width: 100%;" colspan="3">
                        <div id="gvCreatedDocDet" style="width: 100%;"></div>
                    </td>
                </tr>
            </table>
        </div>

        <div id="popupdivToBeCreated" style="display: none; text-align: center;">
            <div class='loader' id="ldrdvtobecreated"></div>
            <table style="width: 100%;">
                <tr>
                    <td style="text-align: left; width: 33%;"><span id="spnCompany2"></span></td>
                    <td style="text-align: center; width: 34%;"><span id="spnSite2"></span></td>
                    <td style="text-align: right; width: 33%; margin-right: 5px;"><span id="spnAct2"></span></td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 10px;"></td>
                </tr>
                <tr>
                    <td style="width: 100%;" colspan="3">
                        <div id="gvToBeCreatedDocDet" style="width: 100%;"></div>
                    </td>
                </tr>
            </table>
        </div>
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

        .link {
            color: #35945B;
        }

            .link:hover {
                font: 15px;
                color: green;
                background: yellow;
                border: solid 1px #2A4E77;
            }

        .auto-style1 {
            height: 16px;
        }
    </style>
    <script type="text/javascript">
        function BindreportEcomp() {
          
          
                $("#grid,#gridcontarctor").html('');

                MyGrid();
                MyGridcontractor();
          

            return false;
        }

        var counter = 0;

      
        $(document).ajaxSend(

            function () {
                counter++;
                if (counter) {
                    $("#mask").css("display", "block");
                    $("#loader").css("display", "block");
                }
            }
            );

        $(document).ajaxComplete(function () {
            counter--;
            if (!counter) {
                $("#mask").css("display", "none"); $("#loader").css("display", "none");
            }
        });

        $(document).ready(function () {

           $("#tabstrip").kendoTabStrip();

        });



        function MyGrid() {
          //  $("#mask").show();

          //  $("#loader").show();
            var Month = $("#ddlMonth").val();
            var Year = $("#ddlYear").val();
          

           
                var str = '{"Month":"' + Month + '","year":"' + Year + '"}';
                $.ajax({
                    type: "POST",
                    url: "EcompMontlhyScheduledDocumentRpt.aspx/GetJSON",
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
                        var columns = data.d.columns;


                        $("#grid").kendoGrid({
                            dataSource: {
                                data: StrData,
                                group: [{
                                    field: "Due_Date",
                                    type: "date",
                                    format: "{0:yyyy/MM/dd }",
                                    aggregates: [
                                      { field: "To_be_created", aggregate: "sum" }, { field: "Created", aggregate: "sum" }
                                    ]
                                },{
                                    field: "Company_Name", aggregates: [
                                      { field: "To_be_created", aggregate: "sum" }, { field: "Created", aggregate: "sum" }
                                    ]
                                }, {
                                    field: "Act", aggregates: [{ field: "To_be_created", aggregate: "sum" },
                                        { field: "Created", aggregate: "sum" }
                                    ]
                                },
                                {
                                    field: "Activity", aggregates: [{ field: "To_be_created", aggregate: "sum" },
                                        { field: "Created", aggregate: "sum" }
                                    ]
                                }],

                                aggregate: [{ field: "To_be_created", aggregate: "sum" },
                                    { field: "Created", aggregate: "sum" },
                                ]

                            }
                        ,
                            height: 700,
                            scrollable: true,
                            resizable: true,
                            reorderable: true,
                            groupable: true,
                            sortable: true,
                            filterable: true,
                            toolbar: [{ name: "excel" }, { template: kendo.template($("#template").html()) },
                            { template: kendo.template($("#template1").html()) }
                            ],



                            excel: {
                                fileName: "Report_Company.xlsx",
                                filterable: true

                            },



                            columns: [{
                                field: "Due_Date",
                                title: "Due Date",
                                type: "date",
                                format: "{0:dd/MM/yyyy }",
                                groupHeaderTemplate: "#= kendo.toString(value,'dd/MM/yyyy') #  :  (Total Scheduled on Date : #= aggregates.To_be_created.sum# )  (Total Created : #=aggregates.Created.sum#  )"

                            },{
                                field: "Company_Name",
                                title: "Company Name",
                                groupHeaderTemplate: "#= value#  :  (Total Scheduled on Company : #= aggregates.To_be_created.sum# )  (Total Created : #=aggregates.Created.sum#  )"

                            },

                                {
                                    field: "Site_Name",
                                    title: "Site Name",

                                }
                                ,
                                {
                                    field: "Act",
                                    title: "Act",
                                    groupHeaderTemplate: "Act: #= value# :  ( Total Scheduled on Act : #= aggregates.To_be_created.sum# )   (Created : #=aggregates.Created.sum# ) "

                                },
                                {
                                    field: "Activity",
                                    title: "Activity",
                                    groupHeaderTemplate: "Activity: #= value# :  ( Total Scheduled on Activity : #= aggregates.To_be_created.sum# )   (Created : #=aggregates.Created.sum# ) "

                                },

                                {
                                    field: "To_be_created",
                                    title: "Total Scheduled",
                                  //  template: '<span title="Click here to get details" style="text-decoration:underline;cursor:pointer;" onclick="ShowDetailsToBeCreated(\'#=CompanyID#\',\'#=SiteID#\',\'#=ActID1#\',\'Scheduled\',\'#=Company_Name#\',\'#=Site_Name#\',\'#=Act#\');return false;" >#=To_be_created#</span>'

                                },
                                {
                                    field: "Created",
                                    title: "Created Till Date",
                                   // template: '<span title="Click here to get details" style="text-decoration:underline;cursor:pointer;" onclick="ShowDetails(\'#=CompanyID#\',\'#=SiteID#\',\'#=ActID1#\',\'Scheduled\',\'#=Company_Name#\',\'#=Site_Name#\',\'#=Act#\');return false;" >#=Created#</span>'

                                }
                            ]
                        });
                        collapseAll();
                    },
                    error: function (data) {
                        alert(data.error);
                      //  $("#mask").hide(); $("#loader").hide();
                    }

                });

              //  $("#mask").hide(); $("#loader").hide();

        }

        function MyGridcontractor()
        {

            //$("#mask").show(); $("#loader").show();

                var Month = $("#ddlMonth").val();
                var Year = $("#ddlYear").val();



                var str = '{"Month":"' + Month + '","year":"' + Year + '"}';
                $.ajax({
                    type: "POST",
                    url: "EcompMontlhyScheduledDocumentRpt.aspx/GetJSONcontractor",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: str,
                    success: function (data) {
                        var grid = $("#gridcontarctor").data("kendoGrid");
                        // detach events
                        if (grid != undefined) {
                            grid.destroy(); // destroy the Grid
                            $("#gridcontarctor").empty();
                        }
                        if (data.d.length == 0) {
                            alert("No Record Found!");
                            return;

                        }
                        var StrData = $.parseJSON(data.d.data);
                        var columns = data.d.columns;


                        $("#gridcontarctor").kendoGrid({
                            dataSource: {
                                data: StrData,
                                group: [{
                                    field: "Due_Date", aggregates: [
                                      { field: "To_be_created", aggregate: "sum" }, { field: "Created", aggregate: "sum" }
                                    ]
                                },{
                                    field: "Contractor", aggregates: [{ field: "To_be_created", aggregate: "sum" },
                                        { field: "Created", aggregate: "sum" }
                                    ]
                                },{
                                    field: "Company_Name", aggregates: [
                                      { field: "To_be_created", aggregate: "sum" }, { field: "Created", aggregate: "sum" }
                                    ]
                                }, {
                                    field: "Act", aggregates: [{ field: "To_be_created", aggregate: "sum" },
                                        { field: "Created", aggregate: "sum" }
                                    ]
                                }, {
                                    field: "Activity", aggregates: [{ field: "To_be_created", aggregate: "sum" },
                                        { field: "Created", aggregate: "sum" }
                                    ]
                                },
                                ],

                                aggregate: [{ field: "To_be_created", aggregate: "sum" },
                                    { field: "Created", aggregate: "sum" },
                                ]

                            }
                        ,
                            height: 700,
                            scrollable: true,
                            resizable: true,
                            reorderable: true,
                            groupable: true,
                            sortable: true,
                            filterable: true, 
                                toolbar: [{ name: "excel" }, { template: kendo.template($("#expandallcont").html()) },
                            { template: kendo.template($("#collapseallcont").html()) }
                            ],



                            excel: {
                                fileName: "Report_Company.xlsx",
                                filterable: true

                            },



                            columns: [{
                                field: "Due_Date",
                                title: "Due Date",
                                type: "date",
                                format: "{0:dd/MM/yyyy }",
                                groupHeaderTemplate: "#= kendo.toString(value,'dd/MM/yyyy') #   :  (Total Scheduled on Date : #= aggregates.To_be_created.sum# )  (Total Created : #=aggregates.Created.sum#  )"

                            }, {
                                field: "Company_Name",
                                title: "Company Name",
                                groupHeaderTemplate: "#= value#  :  (Total Scheduled on Company : #= aggregates.To_be_created.sum# )  (Total Created : #=aggregates.Created.sum#  )"

                            },

                                {
                                    field: "Site_Name",
                                    title: "Site Name",

                                }
                                ,
                                {
                                    field: "Act",
                                    title: "Act",
                                    groupHeaderTemplate: "Act: #= value# :  ( Total Scheduled on Act : #= aggregates.To_be_created.sum# )   (Created : #=aggregates.Created.sum# ) "

                                },
                                {
                                    field: "Activity",
                                    title: "Activity",
                                    groupHeaderTemplate: "Activity: #= value# :  ( Total Scheduled on Activity : #= aggregates.To_be_created.sum# )   (Created : #=aggregates.Created.sum# ) "

                                },  {
                                    field: "Contractor",
                                    title: "Contractor",
                                    groupHeaderTemplate: "#= value# :  ( Total Scheduled on Contractor : #= aggregates.To_be_created.sum# )   (Created : #=aggregates.Created.sum# ) "

                                },
                                
                                {
                                    field: "To_be_created",
                                    title: "Total Scheduled",
                                  //  template: '<span title="Click here to get details" style="text-decoration:underline;cursor:pointer;" onclick="ShowDetailsToBeCreated(\'#=CompanyID#\',\'#=SiteID#\',\'#=ActID1#\',\'Scheduled\',\'#=Company_Name#\',\'#=Site_Name#\',\'#=Act#\');return false;" >#=To_be_created#</span>'

                                },
                                {
                                    field: "Created",
                                    title: "Created Till Date",
                                   // template: '<span title="Click here to get details" style="text-decoration:underline;cursor:pointer;" onclick="ShowDetails(\'#=CompanyID#\',\'#=SiteID#\',\'#=ActID1#\',\'Scheduled\',\'#=Company_Name#\',\'#=Site_Name#\',\'#=Act#\');return false;" >#=Created#</span>'

                                }
                            ]
                        });
                        collapseAllcont();
                    },
                    error: function (data) {
                        alert(data.error);
                       // $("#mask").hide(); $("#loader").hide();
                    }

                });


                //$("#mask").hide(); $("#loader").hide();
            
        }
        //function ShowDetails(CompanyID, SiteID, ActID, Type, CompanyNm, SiteNm, Act) {
        //    $("#ldrdvcreated").show();

        //    var Month = $("#ddlMonth").val();
        //    var Year = $("#ddlYear").val();
        //    $("#spnCompany1").html("<b> Company :  " + CompanyNm + "  </b>");
        //    $("#spnSite1").html("<b>  Site :  " + SiteNm + "  </b>");
        //    $("#spnAct1").html("<b> Act :  " + Act + "  </b>");
        //    $("#popupdivCreated").kendoWindow({
        //        width: "1150px",
        //        height: "500px",
        //        modal: true,
        //        title: "Created Document Details:",
        //        visible: false
        //    })
        //    $("#popupdivCreated").data("kendoWindow").center().open();
        //    CreatedDocsFillGrid(CompanyID, SiteID, ActID, Type, Month, Year, CompanyNm, SiteNm, Act);

        //    $("#ldrdvcreated").hide();
        //    return false;
        //}

        //function ShowDetailsToBeCreated(CompanyID, SiteID, ActID, Type, CompanyNm, SiteNm, Act) {
        //    $("#ldrdvtobecreated").show();
        //    var Month = $("#ddlMonth").val();
        //    var Year = $("#ddlYear").val();

        //    $("#spnCompany2").html("<b> Company :  " + CompanyNm + "  </b>");
        //    $("#spnSite2").html("<b>  Site :  " + SiteNm + "  </b>");
        //    $("#spnAct2").html("<b> Act :  " + Act + "  </b>");

        //    //  $("#ldrdv3").show();
        //    //dvTeam spnProjectTeam gvTeamDtlpopupdivToBeCreated
        //    $("#popupdivToBeCreated").kendoWindow({
        //        width: "1150px",
        //        height: "500px",
        //        modal: true,
        //        title: "Total Scheduled Document Details:",
        //        visible: false
        //    });
        //    // $("#gvModule").html("<div class='loader' style='width:100%;' ></div>");

        //    //spnProject
        //    // $("#spnProjectTeam").html(pname);
        //    $("#popupdivToBeCreated").data("kendoWindow").center().open();
        //    ToBeCreatedDocsFillGrid(CompanyID, SiteID, ActID, Type, Month, Year, CompanyNm, SiteNm, Act);

        //    $("#ldrdvtobecreated").hide();
        //    return false;
        //}

        //function CreatedDocsFillGrid(CompanyID, SiteID, ActID, Type, Month, Year, CompanyNm, SiteNm, Act) {
        //    var str = '{"CompanyID":"' + CompanyID + '","SiteID":"' + SiteID + '","ActID":"' + ActID + '","Type":"' + Type + '","Month":"' + Month + '","Year":"' + Year + '"}';
        //    ////dvTeam spnProjectTeam gvTeamDtl
        //    $.ajax({
        //        type: "POST",
        //        url: "EcomplianceDetailedRpt.aspx/GetJSONCreated",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        data: str,
        //        success: function (data) {
        //            $("#gvCreatedDocDet").html("");
        //            var grid1 = $("#gvCreatedDocDet").data("kendoGrid");
        //            if (grid1 != undefined) {
        //                grid1.destroy();
        //                // destroy the Grid

        //            }

        //            if (data.d.length == 0) {
        //                alert("No Record Found!");
        //                return;
        //            }
        //            var StrData = $.parseJSON(data.d.data);
        //            var columns = data.d.columns;
        //            $("#gvCreatedDocDet").kendoGrid({
        //                dataSource: {
        //                    data: StrData
        //                },
        //                height: 450,
        //                scrollable: true,
        //                resizable: true,
        //                sortable: true,
        //                reorderable: true,
        //                groupable: false,
        //                filterable: true,
        //                toolbar: [{ name: "excel", text: "Export to Excel" }],
        //                excel: {
        //                    fileName: "CreatedDocsDet.xlsx",
        //                    filterable: true
        //                },

        //                columns:
        //                    [
        //                    {

        //                        field: "Activity",
        //                        title: "Activity",


        //                    }
        //                ,
        //                {
        //                    field: "DueDate",
        //                    title: "Due Date",

        //                }
        //                        ,
        //                {
        //                    field: "FirstAlertDate",
        //                    title: "First Alert Date",

        //                }, {
        //                    field: "Type",
        //                    title: "Type",

        //                },
        //                {
        //                    field: "Contractor",
        //                    title: "Contractor",

        //                }
        //               ,
        //                {
        //                    field: "In_Bucket_OF",
        //                    title: "In Bucket OF",

        //                }, {
        //                    field: "PendingDays",
        //                    title: "Pending Days",

        //                }, {
        //                    field: "Status",
        //                    title: "Status",

        //                }, {
        //                    field: "Source",
        //                    title: "Source",

        //                }

        //                    ]

        //            });

        //        },
        //        error: function (data) {
        //            alert(data.error);

        //        }

        //    });
        //}
        //function ToBeCreatedDocsFillGrid(CompanyID, SiteID, ActID, Type, Month, Year, CompanyNm, SiteNm, Act) {
        //    var str = '{"CompanyID":"' + CompanyID + '","SiteID":"' + SiteID + '","ActID":"' + ActID + '","Type":"' + Type + '","Month":"' + Month + '","Year":"' + Year + '"}';
        //    ////dvTeam spnProjectTeam gvTeamDtl
        //    $.ajax({
        //        type: "POST",
        //        url: "EcomplianceDetailedRpt.aspx/GetJSONToBeCreated",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        data: str,
        //        success: function (data) {
        //            $("#gvToBeCreatedDocDet").html("");
        //            var grid1 = $("#gvToBeCreatedDocDet").data("kendoGrid");
        //            if (grid1 != undefined) {
        //                grid1.destroy();

        //            }

        //            if (data.d.length == 0) {
        //                alert("No Record Found!");
        //                return;
        //            }
        //            var StrData = $.parseJSON(data.d.data);
        //            var columns = data.d.columns;
        //            $("#gvToBeCreatedDocDet").kendoGrid({
        //                dataSource: {
        //                    data: StrData
        //                },
        //                height: 450,
        //                scrollable: true,
        //                resizable: true,
        //                sortable: true,
        //                reorderable: true,
        //                groupable: false,
        //                filterable: true,
        //                toolbar: [{ name: "excel", text: "Export to Excel" }],
        //                excel: {
        //                    fileName: "ToBeCreatedDocsDet.xlsx",
        //                    filterable: true
        //                },

        //                columns:
        //                    [
        //                    {

        //                        field: "Activity",
        //                        title: "Activity",


        //                    }
        //                ,
        //                {
        //                    field: "Due_Date",
        //                    title: "Due Date",

        //                }
        //                        ,
        //                {
        //                    field: "First_Alert_Date",
        //                    title: "First Alert Date",

        //                }, {
        //                    field: "Type",
        //                    title: "Type",

        //                },
        //                {
        //                    field: "Contractor",
        //                    title: "Contractor",

        //                }

        //                    ]

        //            });

        //        },
        //        error: function (data) {
        //            alert(data.error);

        //        }

        //    });

        //}
        function collapseAll() {

            var grid = $("#grid").data("kendoGrid");
            grid.tbody.find("tr.k-grouping-row").each(function (index) {
                grid.collapseGroup(this);
            });
            grid.pageable = false;



            return false;
        }

        function expandAll() {
            var grid = $("#grid").data("kendoGrid");


            grid.tbody.find("tr.k-grouping-row").each(function (index) {
                grid.expandGroup(this);

            });
            grid.pageable = true;
            grid.dataSource.pageSize(100);
            // grid.data("kendoGrid").refresh();
            return false;
        }

        function collapseAllcont() {
            var grid = $("#gridcontarctor").data("kendoGrid");
            grid.tbody.find("tr.k-grouping-row").each(function (index) {
                grid.collapseGroup(this);
            });
            grid.pageable = false;
            return false;
        }
        function expandAllcont() {
            var grid = $("#gridcontarctor").data("kendoGrid");
            grid.tbody.find("tr.k-grouping-row").each(function (index) {
                grid.expandGroup(this);
            });
            grid.pageable = true;
            return false;
        }
    </script>
    <script id="template" type="text/x-kendo-template">
    <a class="k-button" href="\#" onclick="return expandAll()">Expand All</a>
    </script>
    <script id="template1" type="text/x-kendo-template">
    <a class="k-button" href="\#" onclick="return collapseAll()">Collapse All</a>
    </script>
    <script id="expandallcont" type="text/x-kendo-template">
    <a class="k-button" href="\#" onclick="return expandAllcont()">Expand All</a>
    </script>
    <script id="collapseallcont" type="text/x-kendo-template">
    <a class="k-button" href="\#" onclick="return collapseAllcont()">Collapse All</a>
    </script>

</asp:Content>


