<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="MGRentProjectionReport, App_Web_qpjniz5y" viewStateEncryptionMode="Always" %>


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
        function pageLoad(sender, args) {
            if (args.get_isPartialLoad()) {
                gridviewScroll();
            }
        }
    </script>
    <script type="text/javascript">
        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }


        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });

        function GetReport() {
            var t1 = '';
            var t2 = '';
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

            var str = '{"SYear": "' + firstYear + '","EYear": "' + SecondYear + '"}';
            //var str = '{"sdate": "' + t1 + '"}';

            $.ajax({
                type: "POST",
                url: "MGRentProjectionReport.aspx/GetDataStore",
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

            var str = '{"SYear": "' + firstYear + '"}';

            $.ajax({
                type: "POST",
                url: "MGRentProjectionReport.aspx/GetExport",
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
            bindGrid("kgrid", data, Columns, true, true, true, 450);
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
                    pageSize: 10,
                    data: Data1
                },
                scrollable: {
                    virtual: true
                },
                columns: columns,
                pageable: true,
                pageSize: 10,

                scrollable: true,
                reorderable: true,
                columnMenu: true,
                groupable: true,
                sortable: true,
                filterable: true,
                resizable: true,
                height: height, 
                excel: {
                    fileName: "MGRentProjectionReport.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }

    </script>

    <style type="text/css">
        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }

        #kgrid tbody tr:hover {
            background: #ef671a;
            color: White;
            font-weight: bold;
            cursor: pointer;
        }

        .k-grid-toolbar a {
            float: right;
        }

        .k-grouping-header {
            float: left;
            margin: auto;
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

        .gradientBoxesWithOuterShadows {
            height: 100%;
            width: 99%;
            padding: 5px;
            background-color: white; /* outer shadows  (note the rgba is red, green, blue, alpha) */
        }

        .link {
            color: #35945B;
        }

            .link:hover {
                font: 15px;
                color: green;
                background: yellow;
                border: solid() 1px #2A4E77;
            }

        .loadexcel {
            margin: 3px 0px 0px;
            border: 1px solid #333;
            padding: 5px 5px;
            border-radius: 5px;
        }

        .sch {
            border: 1px solid #333;
            padding: 3px 3px;
            border-radius: 5px;
            margin: 6px 0px 0px;
        }

        .mg {
            margin: 10px 0px;
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
                                    MG Rent Projection Report 
                                </div>

                                <div class="row mg">
                                    <div style="text-align: center;">
                                        <table style="width: 100%; height: 100%; text-align: center;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblYear" runat="server" Font-Bold="true" Font-Size="Small" Text="UPTo year :"></asp:Label>
                                                    &nbsp;&nbsp;    
                                        <asp:DropDownList ID="ddlFYear" Font-Size="Small" ForeColor="#333333" class="txtbox" runat="server">
                                        </asp:DropDownList></td>
                                            </tr>
                                        </table>

                                    </div>
                                    <br />
                                    <div style="width: 100%; height: 100%; text-align: center; bottom: auto;">

                                        <input type="button" id="btnPlane" value="Search" class="btnNew" style="height: 26px" onclick="GetReport();" />
                                        &nbsp;&nbsp;
                            <input type="button" id="btnExport1" value="Export" class="btnNew" style="height: 26px" onclick="Export();" />
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
                                    <asp:Label ID="lblmsg" runat="server" ForeColor="Red"></asp:Label>
                                    <asp:Label ID="lblTab" runat="server" ForeColor="Red"></asp:Label>
                                </div>


                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
         </div>
    <div id="mask">
        <div id="loader" align="center">
            <img src="images/loading.gif" />
        </div>
    </div>
</asp:Content>

