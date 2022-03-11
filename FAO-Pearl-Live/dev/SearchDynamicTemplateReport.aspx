<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" CodeFile="SearchDynamicTemplateReport.aspx.vb" Inherits="SearchDynamicTemplateReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="js/gridviewScroll.min.js"></script>
    
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
        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
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

        function ReadControles() { }
        ReadControles.prototype.getData = function () {
            var controls = $('[IsSearch]');
            var result = {};
            var Key = "";
            $.each(controls, function (i, e) {
                var type = $(e).attr('data-ty');
                Key = $(e).attr('fld');
                switch (type) {
                    case "DATETIME":
                        result[Key] = $(e).val();
                        break;
                    case 'NUMERIC':
                        result[Key] = $(e).val();
                        break;
                    case 'DDL':
                        result[Key] = $(e).val();
                        break;
                    case 'MULTISELECT':
                        var vals = [];
                        $('#' + $(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
                            vals.push($(this).attr('value'));
                        });
                        result[Key] = vals;
                        break;
                    case 'SINGLESELECT':
                        $('#' + $(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
                            result[Key] = $(this).attr('value');
                        });
                        break;
                    default:
                        break;
                }
            });

            result.DocFromDate = $('#ContentPlaceHolder1_Frflddate').val();
            result.DocToDate = $('#ContentPlaceHolder1_Toflddate').val();
            result.IsView = $('#hdnView').attr('Value');

            return result;
        }
        function getReport() {     
            var Sdate = $('#ContentPlaceHolder1_txtsdate').val();
            var Edate = $('#ContentPlaceHolder1_txtedate').val();
            var ReportName = $('#ContentPlaceHolder1_txtreportName').val();
            if (ReportName == '') {
                alert("Please Enter Report Name")
                $('#ContentPlaceHolder1_txtreportName').focus();
                return false;
            }
                if (Sdate == '') {
                    alert("Please Select From Date");
                    return false;
                }
                else if (Edate == '') {
                    alert("Please Select To Date");
                    return false;

                }
            var obj = Rc.getData();
            var str1 = JSON.stringify(obj);
            
            $.ajax({
                type: "POST",
                url: "SearchDynamicTemplateReport.aspx/GetData",
                data: '{ "json": ' + str1 + ' }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindGrid,
                failure: function (response) {
                    $("#lblTab").text('Unknown error. Please contact your system administrator.');
                }
            });
        }

        function BindGrid(result) {

            if (!result.d.Success) {
                $("#ContentPlaceHolder1_lblTab").text('No Data Found..!');
                //$("#lblTab").text(result.d.Message);
                return false;
            }
            else {
                $("#ContentPlaceHolder1_lblTab").text('');
            }
            $("#lblCaption").text(result.d.Count + ' records found.');
            var Columns = result.d.Column;
            var cnt = result.d.Column.Count;
            var data = JSON.parse(result.d.Data);
            if (Columns.indexOf('UID') > 0 ) {

            }
            else {
                var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
            }
            bindGrid("kgrid", data, Columns, true, true, true, 550);

        }
        function DetailHandler(evt) {
            //dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var docid = evt.data.DOCID;
            //  window.open('DocDetail.aspx?DOCID=' + docid + '');

            window.open('DocDetail.aspx?DOCID=' + docid + '', '_blank');

            //OpenWindow('DocDetail.aspx?DOCID=' + docid + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }
        function GetQueryString(param) {
            var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < url.length; i++) {
                var urlparam = url[i].split('=');
                if (urlparam[0] == param) {
                    return urlparam[1];
                }
            }
        }
        function bindGrid(gridDiv, Data1, columns, pageable, filterable, sortable, height) {

            var g = $("#" + gridDiv).data("kendoGrid");

            if (g != null || g != undefined) {
                g.destroy();
                $("#" + gridDiv).html('');
            }

            if (columns.indexOf('UID') > 0) {
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
                    toolbar: ["excel"],
                    excel: {
                        fileName: "Report.xlsx",
                        filterable: true,
                        pageable: true,
                        allPages: true
                    }

                });
            }
            else {
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

                    dataBound: onDataBound,

                    groupable: true,
                    sortable: true,
                    filterable: true,
                    resizable: true,
                    height: height,
                    toolbar: ["excel"],
                    excel: {
                        fileName: "Report.xlsx",
                        filterable: true,
                        pageable: true,
                        allPages: true
                    }

                });
            }
            

        }
        var Rc = new ReadControles();
        function onDataBound(e) {
            var grid = $("#kgrid").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var DOCID = gridData[i].DocID;
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
            }
        }
    </script>
    <script type="text/javascript">
        function ShowLocations() {
            document.getElementById("ContentPlaceHolder1_pngv").style.display = "none";
            document.getElementById("fmap").style.display = "block";
            return false;
        }
        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
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
    <div class="container-fluid container-fluid-amend">
        <table cellspacing="0px" cellpadding="0px" width="100%" border="0">
            <tr>
                <td style="width: 100%;" valign="top" align="left">
                    <div id="main" style="min-height: 400px">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnViewInExcel" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="form" style="text-align: left">
                                    <div class="doc_header">
                                        Dynamic Template Search Report
                                    </div>
                                    <div class="row mg">
                                        <div class="col-md-1 col-sm-2">
                                            <label>Report Description</label>
                                        </div>
                                        <div class="col-md-2 col-sm-2">
                                            <asp:DropDownList ID="ddlDocType" runat="server" AutoPostBack="true" CssClass="txtBox" OnSelectedIndexChanged="ddlDocType_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                         <div class="col-md-1 col-sm-1">
                                           <label> Action </label>
                                        </div>
                                        <div class="col-md-2 col-sm-2">
                                            <asp:DropDownList ID="ddlfields" CssClass="txtBox" runat="server" AutoPostBack="true" Width="144px">
                                            </asp:DropDownList>
                                        </div>
                                        <asp:Panel ID="pnl1" runat="server">
                                                <div class="col-md-1 col-sm-1 ">
                                                    <asp:Label ID="Label1"
                                                        runat="server" Font-Bold="True" Text="From Date"></asp:Label>
                                                </div>
                                                <div class="col-md-1 col-sm-2 ">
                                                    <asp:TextBox ID="txtsdate" CssClass="txtBox" runat="server" Width="120"></asp:TextBox>
                                                    <asp:CalendarExtender ID="calendersdate" TargetControlID="txtsdate" Format="yyyy-MM-dd"
                                                        runat="server" />
                                                </div>
                                                <div class="col-md-1 col-sm-2 " style="margin-left:25px">
                                                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Text=" To Date"></asp:Label>
                                                </div>
                                                <div class="col-md-1 col-sm-1 ">
                                                    <asp:TextBox ID="txtedate" runat="server" CssClass="txtBox" Width="120"></asp:TextBox>
                                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd"
                                                        runat="server" />
                                                </div>

                                               <div class="col-md-1 col-sm-1" style="margin-left:25px">
                                            <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/search.png" CssClass="sch"
                                                ToolTip="Search" OnClientClick="getReport(); return false;" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                              <asp:ImageButton ID="btnViewInExcel" runat="server" ImageUrl="~/images/excel.png" CssClass="sch"
                                                ToolTip="Export To Excel" OnClick="btnViewInExcel_Click"/>
                                        </div>

                                                <br />
                                            </asp:Panel>
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

