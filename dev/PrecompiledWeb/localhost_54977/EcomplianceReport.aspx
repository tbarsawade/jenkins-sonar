<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="EcomplianceReport, App_Web_2echgblw" viewStateEncryptionMode="Always" %>

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
            background: #e96125;
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
        /**/
        .k-grid-toolbar a {
            float: right;
        }

        .k-tabstrip-wrapper {
            width: 100%;
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

        .k-grid table tr:hover 
        {
            background: rgb(69, 167, 84);
            color: black;
        }
    </style>
    <script type="text/javascript">

        //function OpenWindow(url) {
        //    var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
        //    return false;
        //}


    </script>
    <div style="text-align: left; color:#ffffff; height:30px; background-color:#e96125; margin-bottom:10px; border-radius:5px;">
        <h1 style="line-height:28px; padding-left:5px;"><b>Compliance ScoreCard</b></h1>
    </div>
    <div style="width: 100%; display: block;">
    
    <div class="basic-grey" style="border: 1px solid #dddddd; border-radius: 5px;">
        <asp:HiddenField ID="hdnUID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnurole" runat="server" Value="" />
        <asp:HiddenField ID="hdnEid" runat="server" Value="" />
        <asp:HiddenField ID="hdnMap" runat="server" Value="" />
        <table style="width: 100%; padding: 15px 15px;">

            <tr style="height: 20px;">
                <td colspan="5"></td>
            </tr>
            <tr>
                 <th>&nbsp;</th>
                <th>Select Company</th>
                <th>Select Site</th>
                <th>Select Month</th>
                <th>Select Year</th>
                <th></th>
            </tr>
            <tr style="height:12px;">
                <td colspan="6"></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <input type="text" id="ddlCompany" style="width:200px;" class="k-text" placeholder="Please type company name" />
                    <input type="hidden" value="0" id="hdnCompID" />

                    <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <asp:DropDownList ID="ddlCompany" Width="150" ClientIDMode="Static" AutoPostBack="true" Style="height: 34px; width:320px;" runat="server">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlCompany" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>--%>
                </td>
                <td>
                    <select id="ddlSite" style="height: 34px; width:150px;">
                        <option value="0">All</option>
                    </select>

                    <%--<asp:DropDownList ID="ddlSite" Width="150" Style="height: 34px;" runat="server">
                        <asp:ListItem Value="0">All</asp:ListItem>
                    </asp:DropDownList>--%>

                    <%--<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlSite" Width="150" ClientIDMode="Static" Style="height: 34px;" runat="server">
                                <asp:ListItem Value="0">All</asp:ListItem>
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlSite" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>--%>
                </td>
                <td>
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
                <td>
                    <asp:DropDownList ID="ddlYear" Width="150" ClientIDMode="Static" Style="height: 34px;" runat="server">
                    </asp:DropDownList>
                </td>
                <td>
                    <%--<asp:Button ID="btnSearch1" runat="server" Text="Search" OnClientClick="javascript:return Bindreport();"
                        CssClass="button" />--%>
                    <input id="btnSearch" class="button" type="button" value="Search" onclick="BindreportEcomp();" />
                </td>
            </tr>
            <tr style="height: 20px;">
                <td colspan="5"></td>
            </tr>
        </table>
    </div>
</div>
    <div>
        <div id="mask">
            <div id="loader">
                <img src="images/preloader22.gif" alt="" />
            </div>
        </div>
    </div>
    <br />
    <br />
    <div id="tabstrip">
        <ul>
            <li class="k-item k-state-default k-first k-tab-on-top k-state-active" id="liCompany">Company</li>

            <li id="liContractors">Contractors</li>

        </ul>
        <div style="width: 96%; overflow-x: scroll;">
            <div id="grid">
            </div>
        </div>
        <div style="width: 96%; overflow-x: scroll;">
            <div id="gridcontarctor">
            </div>
        </div>

    </div>
    <div id="details">
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
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $(document).ready(function () {
                $("#ddlCompany").kendoAutoComplete({
                    dataTextField: "CompanyName",
                    dataValueField: "tid",
                    filter: "contains",
                    minLength: 3,
                    select: function (e) {
                        var dataItem = this.dataItem(e.item.index());
                        $('#hdnCompID').val(dataItem.tid);
                        bindSite(dataItem.tid);
                    },
                    dataSource: {
                        serverFiltering: true,
                        transport: {
                            read: function (options) {
                                var Site = $("#ddlCompany").val(); 
                                var UID = '<%= Session("UID").ToString()%>';
                                var URole = '<%= Session("USERROLE").ToString()%>';

                                var d = "{'str':'" + Site + "','uid':'"+UID+"', 'urole':'"+ URole +"'}"; 
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

                                    }
                                });
                            }
                        }
                    }
                });

            });
        });
        function bindSite(tid) {
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            var t = '{ CompID: "' + tid + '" ,uid:"'+UID+'", urole:"'+ URole +'"}';
            $("#ddlSite").empty().append($("<option></option>").val("0").html("All"))
            $.ajax({
                type: "POST",
                url: "EcomplianceReport.aspx/GetSite",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    var dsc = JSON.parse(res);
                    if (dsc.length > 0) {
                        //$("#ContentPlaceHolder1_ddlSite").append($("<option></option>").val("0").html("--Select--"));
                        $.each(dsc, function () {
                            var CompID = this.tid;
                            var CompName = this.fld100;
                            $("#ddlSite").append($("<option></option>").val(CompID).html(CompName));
                        });
                        //$("#ContentPlaceHolder1_ddlSite").kendoDropDownList();
                    }
                    else {
                        $("#ddlSite").empty().append($("<option></option>").val("0").html("All"));
                        //$("#ContentPlaceHolder1_ddlSite").empty();
                        //$("#ContentPlaceHolder1_ddlSite").kendoDropDownList();
                    }
                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
        }
    </script>
    <script type="text/javascript">
        function BindreportEcomp() {
            MyGrid();
            MyGridcontractor();
            //var grid = $("#grid").data("kendoGrid");
            //grid.bind("dataBound", collapseAll);
            return false;
        }


        $(document).ajaxStart(
            function () {
                $("#grid,#gridcontarctor").html('');
                $("#mask").css("display", "block");
                $("#loader").css("display", "block");
            }
            );
        $(document).ajaxComplete(function () {
            $("#mask").css("display", "none"); $("#loader").css("display", "none");
        });

        $(document).ready(function () {

            $("#tabstrip").kendoTabStrip();

        });



        function MyGrid() {
            var Comp = $("#hdnCompID").val();
            //var compnm = $("#ddlCompany option:selected").text();
            var Site = $("#ddlSite").val();
            var Month = $("#ddlMonth").val();
            var Year = $("#ddlYear").val();
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            var value = $("#ddlCompany").data("kendoAutoComplete").value();
            if (Comp == 0 || value =='') {
                alert("Please select Company");
                return;
            }
            //else if (Site == 0) {
            //    alert("Please select Site");
            //    return;
            //}
                //else if (compnm.indexOf("Telenor") > -1)
                //{
                //   

                //}

            else {

                $("#liCompany").show();
                // $("#mask").show();
                var str = '{"Company":"' + Comp + '","Site":"' + Site + '","Month":"' + Month + '","year":"' + Year + '","UID":"'+UID+'","Urole":"'+URole+'"}';
                $.ajax({
                    type: "POST",
                    url: "EcomplianceReport.aspx/GetJSON",
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
                        command = { command: { text: 'View Details', click: showDetails } };
                        columns.push(command);
                        if (StrData.length == 0)
                        {
                            $("#liCompany").hide();
                            $("#dvcomp").hide();
                            var tabToActivate = $("#liContractors");
                            $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
                        }
                        else
                        {

                            $("#liCompany").show();
                            $("#dvcomp").show();
                            var tabToActivate = $("#liCompany");
                            $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
                        }
                        $("#grid").kendoGrid({
                            dataSource: {
                                data: StrData,
                                //pageSize: 100,
                                group: [
                                      { field: "Company", aggregates: [{ field: "Company", aggregate: "count" }, { field: "Percentage", aggregate: "average" }] },
                                    { field: "Site", aggregates: [{ field: "Site", aggregate: "count" }, { field: "Percentage", aggregate: "average" }] },
                                    { field: "Act", aggregates: [{ field: "Act", aggregate: "count" }, { field: "Percentage", aggregate: "average" }] }

                                ],
                                aggregate: [{ field: "Act", aggregate: "count" },
                                              { field: "Site", aggregate: "average" }
                                              , { field: "Company", aggregate: "average" }
                                ]
                            }
                        ,
                            height: 700,
                            scrollable: true,
                            resizable: true,
                            reorderable: true,
                            groupable: false,
                            filterable: true,
                            sortable: true,
                            toolbar: [{ name: "excel" }, { template: kendo.template($("#template").html()) },
                                { template: kendo.template($("#template1").html()) }
                            ],

                            // buttons:"Expand All",

                            excel: {
                                fileName: "Report_Company.xlsx",
                                filterable: true,
                                pageable: true,
                                allPages: true
                            },
                            ExpandAll: {},

                            //pageable: {
                            //    buttonCount: 5
                            //},
                            columns: columns
                        });
                        // $("#mask").hide();

                        collapseAll();
                    },
                    error: function (data) {
                        alert(data.error);
                        // $("#mask").hide();
                    }

                });
                wnd = $("#details")
                        .kendoWindow({
                            title: "Document Details",
                            modal: true,
                            visible: false,
                            resizable: false,
                            width: 500
                        }).data("kendoWindow");

                detailsTemplate = kendo.template($("#popup_editor").html());


            }
        }

        function MyGridcontractor() {
            var Comp = $("#hdnCompID").val();
            var Site = $("#ddlSite").val();
            var Month = $("#ddlMonth").val();
            var Year = $("#ddlYear").val();
            var value = $("#ddlCompany").data("kendoAutoComplete").value();
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            if (Comp == 0 || value == '') {
                //alert("Please select Company");
                return;
            }
            
            else {
                //  $("#mask").show();
                var str = '{"Company":"' + Comp + '","Site":"' + Site + '","Month":"' + Month + '","year":"' + Year + '","UID":"' + UID + '","Urole":"' + URole + '"}';
                $.ajax({
                    type: "POST",
                    url: "EcomplianceReport.aspx/GetJSONcontractor",
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
                        command = { command: { text: 'View Details', click: showDetails } };
                        columns.push(command);
                        $("#gridcontarctor").kendoGrid({
                            dataSource: {
                                data: StrData,
                              
                                group: [
                                      { field: "Company", aggregates: [{ field: "Company", aggregate: "count" }, { field: "Percentage", aggregate: "average" }] },
                                    { field: "Site", aggregates: [{ field: "Site", aggregate: "count" }, { field: "Percentage", aggregate: "average" }] },
                                     { field: "Contractor", aggregates: [{ field: "Percentage", aggregate: "average" }] },
                                    { field: "Act", aggregates: [{ field: "Percentage", aggregate: "average" }] }


                                ],
                                aggregate: [{ field: "Act", aggregate: "count" },
                                              { field: "Site", aggregate: "average" }
                                              , { field: "Company", aggregate: "average" },
                                              { field: "Contractor", aggregate: "average" }
                                ]
                            }
                        ,
                            height: 700,
                            scrollable: true,
                            resizable: true,
                            reorderable: true,
                            groupable: false,
                            sortable: true,
                            filterable: true,
                            toolbar: [{ name: "excel", text: "Export Contractor" }, { template: kendo.template($("#expandallcont").html()) },
                                { template: kendo.template($("#collapseallcont").html()) }

                            ],

                            // buttons:"Expand All",

                            excel: {
                                fileName: "Report_Contractor.xlsx",
                                filterable: true,
                                pageable: true,
                                allPages: true
                            },


                            //pageable: {
                            //    buttonCount: 5
                            //},
                            columns: columns
                        });
                        //  $("#mask").hide();

                        collapseAllcont();
                    },
                    error: function (data) {
                        alert(data.error);
                        // $("#mask").hide();
                    }

                });
                wnd = $("#details")
                .kendoWindow({
                    title: "Document Details",
                    modal: true,
                    visible: false,
                    resizable: false,
                    width: 900
                }).data("kendoWindow");

                detailsTemplate = kendo.template($("#popup_editor").html());

            }
        }
        function showDetails(e) {
            e.preventDefault();

            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            // alert(dataItem.Tid);
            wnd.content(detailsTemplate(dataItem));
            wnd.center().open();
        }


        function UPload1Handler1(FileUpload) {

            if (FileUpload != '') {
                window.open('../../docs/' + FileUpload + '');
            }
            else { alert('File does not exist.'); }
        }


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
    <script id="popup_editor" type="text/x-kendo-template"> 
              
         <div id="details-container">
        <table style="width:100%;">
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Company:</b></td>
                  <td align="left" width="70%"><h2>#= Company #</h2></td>
              </tr>
              <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Site:</b></td>
                  <td align="left" width="70%"><h2>#= Site #</h2></td>
              </tr>
        <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Act:</b></td>
                  <td align="left" width="70%"><h2>#= Act #</h2></td>
              </tr>
        <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Activity:</b></td>
                  <td align="left" width="70%"><h2>#= Activity #</h2></td>
              </tr>
              <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
     
              <tr >
                  <td align="left" style="padding-left: 20px" width="30%">
        </td>
                  <td align="left"  width="70%">
        #if(File1 === '')
        {# #}
        else{#  <a class="k-button" title="Click to download attachment" onclick="return UPload1Handler1('#=File1#')"  >#=File1D#</a> #}#
         #if(File2 === '')
        {##}
        else{#  <a class="k-button" title="Click to download attachment" onclick="return UPload1Handler1('#=File2#')">#=File2D#</a> #}#
         #if(File3 === '')
        {##}
        else{#   <a class="k-button"  title="Click to download attachment" onclick="return UPload1Handler1('#=File3#')">#=File3D#</a> #}#
         #if(File4 === '')
        {##}
        else{#     <a class="k-button" title="Click to download attachment"  onclick="return UPload1Handler1('#=File4#')">#=File4D#</a> #}#
         #if(File5 === '')
        {##}
        else{#  <a class="k-button" title="Click to download attachment"  onclick="return UPload1Handler1('#=File5#')">#=File5D#</a> #}
        #
      
       
        
     
        

        </td>
              </tr> 
        
          </table>
                 
        
       
        
      
                    
                </div> 
     
    </script>
    <style type="text/css">
        #details-container {
            padding: 10px;
        }

            #details-container h2 {
                margin: 0;
                color: black;
            }
    </style>
</asp:Content>

