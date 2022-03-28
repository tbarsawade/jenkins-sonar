<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="SDReport, App_Web_c5kjwoe4" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="js/gridviewScroll.min.js"></script>
    <%--  <link href="css/GridviewScroll.css" rel="stylesheet" />--%>
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
    <script type="text/javascript">
     <%--   $(document).ready(function () {

           <%-- gridviewScroll();
            $('#<%=gvReport.ClientID%>').css("width", '100%');

        });--%>

       <%-- function gridviewScroll() {
            $('.form').addClass('form pnlAutoHeight');
            $('#<%=gvReport.ClientID%>').gridviewScroll({
                width: 1320,
                height: 400,
                arrowsize: 30,
                varrowtopimg: "Images/arrowvt.png",
                varrowbottomimg: "Images/arrowvb.png",
                harrowleftimg: "Images/arrowhl.png",
                harrowrightimg: "Images/arrowhr.png"
            });

        }--%>

        function pageLoad(sender, args) {
            if (args.get_isPartialLoad()) {
                gridviewScroll();
            }
        }
    </script>
    <script type="text/javascript">

        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
    </script>
    <script type="text/javascript">
        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
        //var textval = $('#txtDslip').val()

        function GetReport() {
            var t1 = $("#txtsdate").val();
            var t2 = $("#txtedate").val();
            if (t1 == undefined) { t1 = '' }
            if (t2 == undefined) { t2 = '' }
            var ddlval = $("#<%=ddlFYear.ClientID%> option:selected").val()
            if (ddlval.toUpperCase() == 'SELECT') {
                ddlval = "";
                // return false;
            } else {
                var Years = ddlval.split("-");
                var firstYear = Years[0]
                var SecondYear = Years[1]
            }

            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '","SYear": "' + firstYear + '","EYear": "' + SecondYear + '"}';
            //var str = '{"sdate": "' + t1 + '"}';

            $.ajax({
                type: "POST",
                url: "SDReport.aspx/GetDataStore",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }

        function Export() {
            var t1 = $("#txtsdate").val();
            var t2 = $("#txtedate").val();
            if (t1 == undefined) { t1 = '' }
            if (t2 == undefined) { t2 = '' }
            var ddlval = $("#<%=ddlFYear.ClientID%> option:selected").val()
            if (ddlval.toUpperCase() == 'SELECT') {
                ddlval = "";
                // return false;
            } else {
                var Years = ddlval.split("-");
                var firstYear = Years[0]
                var SecondYear = Years[1]
            }

            var str = '{"sdate": "' + t1 + '", "edate": "' + t2 + '","SYear": "' + firstYear + '","EYear": "' + SecondYear + '"}';

            $.ajax({
                type: "POST",
                url: "SDReport.aspx/GetExport",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindExl,
                failure: function (response) {
                    alert(response.d);
                },
                error: function (response) {
                    alert(response.d);
                }
            });
        }

        function BindExl(result) {
            var url = result.d;
            window.location = url;
        }

        function BindReport(result) {
            if (result.d.Success == false) {
                $("#kgrid").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                return;
            }
            var Columns = result.d.Column;
            var data = JSON.parse(result.d.Data);
            bindGrid("kgrid", data, Columns, true, true, true, 550);
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
                    pageSize: 20,
                    data: Data1
                },
                scrollable: {
                    virtual: true
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
                //toolbar: ['excel'],
                toolbar: false,
                excel: {
                    fileName: "Report.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }


        function GetBusinessUnit() {
            var Bunit = $("#ddlBunit").val()
            var str = '{"Bunit": "' + Bunit + '"}';
            $.ajax({
                type: "POST",
                url: "OTReport.aspx/GetBusinessUnit",
                contentType: "application/json; charset=utf-8",
                data: str,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //dvsqEditLoader
                    var data = $.parseJSON(res);
                    $("#ddlBunit").kendoMultiSelect({
                        dataTextField: "BU",
                        dataValueField: "TID",
                        dataSource: data,
                        autoClose: false
                    }).data("kendoMultiSelect");
                },
                error: function (data) {
                }
            });
        }


        function SelectALLCircle() {
            var multiSelect = $("#ddlBunit").data("kendoMultiSelect");
            var selectedValues = "";
            var strComma = "";
            for (var i = 0; i < multiSelect.dataSource.data().length; i++) {
                var item = multiSelect.dataSource.data()[i];
                selectedValues += strComma + item.TID;
                strComma = ",";
            }
            multiSelect.value(selectedValues.split(","));
        }



        $(document).ready(function () {
            //  GetBusinessUnit();
            //  $("#select").bind("click", {}, SelectALLCircle);
        });
    </script>
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
            background-color: #F6F6F6;
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }

        #loader {
            position: absolute;
            left: 50%;
            top: 50%;
            background-image: url("images/uploading.gif");
            background-repeat: no-repeat;
            background-position: center;
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

        .mg {
            margin: 10px 0px;
        }

        .pnlAutoHeight {
            min-height: 0px !important;
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-size: 12px;
        }
    </style>
    <div class="container-fluid">
        <table cellspacing="0px" cellpadding="0px" width="100%" border="0">
            <tr>
                <td style="width: 100%;" valign="top" align="left">
                    <div id="main" style="min-height: 400px">
                        <asp:UpdatePanel ID="updPnlGrid" runat="server">

                            <ContentTemplate>


                                <div class="form" style="text-align: left">
                                    <div class="doc_header">

                                        <asp:Label ID="lblMsg" runat="server"> SD Ind Ad Report</asp:Label>
                                    </div>

                                    <div class="col-md-11 col-sm-11 mg">
                                        <asp:Label ID="lblCaption" runat="server"></asp:Label>
                                    </div>
                                    <div class="col-md-1 col-sm-1 mg">
                                        <div class="pull-right">
                                            <%--   <asp:ImageButton ID="showReport" runat="server" ToolTip="Show Report" Visible="false" ImageUrl="~/images/search.png"
                                Width="18px" Height="18px" OnClick="show" />&nbsp;
                            <asp:ImageButton ID="btnExport" ToolTip="Export PDF" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/images/export.png" />&nbsp;
                            <asp:ImageButton ID="btnexcel" ToolTip="Export CSV" runat="server" Visible="false" Width="18px" Height="18px"
                                ImageUrl="~/images/csv.png" />&nbsp;
                            <asp:ImageButton ID="Excelexport" ToolTip="Export EXCEL" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/Images/excelexpo.jpg" />--%>
                                        </div>
                                    </div>
                                    <div class="row mg" id="divvivek">
                                        <div class="col-md-12 col-sm-12">

                                            <asp:Panel ID="pnlFields" CssClass="pnlAutoHeight" runat="server" Width="100%" ScrollBars="Vertical">
                                            </asp:Panel>
                                            <asp:Panel ID="PnlControls" runat="server" Height="100%" ScrollBars="Vertical">
                                                <table>
                                                    <tr>
                                                        <td style="width: 110px" align="left">
                                                            <asp:Label ID="lblsdate" runat="server" Visible="false" Font-Bold="true" Font-Size="Small" Text="From Date :"></asp:Label>
                                                        </td>
                                                        <td style="width: 90px; display: none">
                                                            <asp:TextBox ID="txtsdate" runat="server" ClientIDMode="Static"></asp:TextBox>
                                                            <asp:CalendarExtender ID="calendersdate" TargetControlID="txtsdate" Format="yyyy-MM-dd"
                                                                runat="server" />
                                                        </td>

                                                        <td style="width: 110px" align="center">
                                                            <asp:Label ID="lbledate" Visible="false" runat="server" Font-Bold="true" Font-Size="Small" Text="To Date :"></asp:Label>
                                                        </td>
                                                        <td style="width: 90px; display: none" align="center">
                                                            <asp:TextBox ID="txtedate" runat="server" ClientIDMode="Static"></asp:TextBox>
                                                            <asp:CalendarExtender ID="calenderedate" TargetControlID="txtedate" Format="yyyy-MM-dd"
                                                                runat="server" />
                                                        </td>

                                                        <td style="width: 257px;" align="center"></td>
                                                        <td style="width: 110px" align="center">
                                                            <asp:Label ID="lblYear" runat="server" Font-Bold="true" Font-Size="Small" Text="UPTo year :"></asp:Label>
                                                        </td>
                                                        <td style="width: 110px;" align="center">
                                                            <asp:DropDownList ID="ddlFYear" CssClass="ddl" Width="100%" runat="server" ClientIDMode="Static">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td style="width: 110px" align="center">
                                                            <%--   <asp:Label ID="lblUserName" runat="server" Font-Bold="true" Font-Size="Small" Text="UserName :"></asp:Label>--%>
                                                        </td>
                                                        <td style="width: 110px" align="center">
                                                            <asp:CheckBoxList ID="CheckListuserName" DataValueField="tid" runat="server">
                                                            </asp:CheckBoxList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <br />
                                            <div style="width: 100%; height: 100%; text-align: center; bottom: auto;">
                                                <%-- <asp:Button ID="btnActEdit" runat="server" Visible="True" Text="Search" CssClass="btnNew"  onclick="GetReport();"
                                    Font-Bold="True" Font-Size="X-Small" Width="100px" />--%>
                                                <input type="button" id="btnPlane" value="Search" class="btnNew" style="height: 26px" onclick="GetReport();"
                                                    style="width: 80px" />
                                                <%--<asp:Button ID="btnViewInExcel" runat="server" Visible="True" Text="View in Excel"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" />--%>&nbsp;&nbsp;
                                               <%-- <asp:LinkButton ID="btnViewInExcel" runat="server" CssClass="link"><b>Click to download in excel</b></asp:LinkButton>--%>
                                                <input type="button" id="btnExport1" value="Export" class="btnNew" style="height: 26px" onclick="Export();"
                                                    style="width: 80px" />
                                            </div>

                                        </div>

                                        
                                    </div>
                                    <div class="row">
                                            <div class="col-md-12 col-sm-12">
                                                <div align="center" style="width: auto">
                                                    <div style="max-width: 1320px; overflow-x: scroll;">
                                                        <div id="kgrid" style="height: unset;">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    <div align="center">
                                        <asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label>
                                        <asp:Label ID="lblTab" runat="server" ForeColor="Red"></asp:Label>
                                    </div>


                                    <%--Graph Properties--%>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="mask">
        <div id="loader">
        </div>
    </div>
</asp:Content>
