<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="IHCLInvoiceStatusReport, App_Web_dqvq3srr" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

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

     <script src="Jquery/jquery.sumoselect.min.js"></script>
   <link href="css/sumoselect.css" rel="stylesheet">
    
    <Link href="bootstrap-4/css/bootstrap.min.css"
    <Link href="bootstrap-4/css/bootstrap-multiselect.css"
    <script src="bootstrap-4/js/bootstrap.min.js"></script>
    <script src="bootstrap-4/js/bootstrap-multiselect.js"></script>


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

        function GetVendors() {
            $.ajax({
                type: "POST",
                url: "IHCLInvoiceStatusReport.aspx/GetVendors",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    var ddlvendor = $("[id*=ddlvendor]");
                    ddlvendor.removeAttr("disabled");
                    ddlvendor.empty().append('<option selected="selected" value="0">All</option>');
                    $.each(r.d, function () {
                        ddlvendor.append($("<option></option>").val(this['Value']).html(this['Text']));
                    });
                }
            });
        }
        
       

        


        function getReport() {
            //var fname = $("#ContentPlaceHolder1_ddlfields option:selected").text();
            var dept = $('#ContentPlaceHolder1_lstdept').val();
            var filedval = $('#ContentPlaceHolder1_ddlvendor').val();
            var Sdate = $('#ContentPlaceHolder1_txtsdate').val();
            var Edate = $('#ContentPlaceHolder1_txtedate').val();
            var sts = $('#ContentPlaceHolder1_ddlstatus').val();
              //var fval = $("#ContentPlaceHolder1_txtval").text();
            var fval = $("#ContentPlaceHolder1_txtval").val()

            if (dept == '') {
                alert("Please Select Department");
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
            //var str1 = JSON.stringify(obj);
                var dept = GetIDS();
                if (dept == '') {
                    alert("Please Select Department");
                    return false;
                }
            //var dept = '';
            var str = '{ "sdate": "' + Sdate + '", "edate": "' + Edate + '", "vendor": "' + filedval + '", "status": "' + sts + '", "dept": "'+ dept +'"}';
            $.ajax({
                type: "POST",
                url: "IHCLInvoiceStatusReport.aspx/GetData",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    BindGrid(data);
                    GetVendors();
                },
                failure: function (response) {
                    $("#lblTab").text('Unknown error. Please contact your system administrator.');
                }
            });
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

        function BindGrid(result) {

            if (!result.d.Success) {
                $("#lblTab").text(result.d.Message);
                return false;
            }
            else {
                $("#lblTab").text('');
            }
            $("#lblCaption").text(result.d.Count + ' records found.');
            var Columns = result.d.Column;
            var data = JSON.parse(result.d.Data);
            //  var CommanObj = { command: { text: "View Map", click: DetailHandler} }
            var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil" }], title: "Action" }
            // var CommanObj = { command: [{ text: "Details", click: DetailHandler}] }
            // Columns.push(CommanObj);
            bindGrid("kgrid", data, Columns, true, true, true, 550);
            // var data = grid.data("kgrid").dataSource.view();
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
                dataBound: onDataBound,
                groupable: true,
                sortable: true,
                filterable: true,
                resizable: true,
                height: height,
                //toolbar: ['excel'],
                //excel: {
                //    fileName: "Report.xlsx",
                //    filterable: true,
                //    pageable: true,
                //    allPages: true
                //}

            });

        }
        //function bindGrid(gridDiv, Data1, columns, pageable, filterable, sortable, height) {

        //    var g = $("#" + gridDiv).data("kendoGrid");

        //    if (g != null || g != undefined) {
        //        g.destroy();
        //        $("#" + gridDiv).html('');
        //    }

        //    gridDiv = $("#" + gridDiv).kendoGrid({


        //        dataSource: {
        //            pageSize: 20,
        //            data: Data1
        //        },
        //        scrollable: {
        //            virtual: true
        //        },

        //        columns: columns,
        //        pageable: true,
        //        pageSize: 20,

        //        scrollable: true,
        //        reorderable: true,
        //        columnMenu: true,
        //        dataBound: onDataBound,

        //        groupable: true,
        //        sortable: true,
        //        filterable: true,
        //        resizable: true,
        //        height: height,
        //        //toolbar: ["excel"],
        //        //excel: {
        //        //    fileName: "Report.xlsx",
        //        //    filterable: true,
        //        //    pageable: true,
        //        //    allPages: true
        //        //}

        //    });

        //}

        var Rc = new ReadControles();

        function DetailHandler(evt) {
            //dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var docid = evt.data.DOCID;
            //  window.open('DocDetail.aspx?DOCID=' + docid + '');

            window.open('DocDetail.aspx?DOCID=' + docid + '', '_blank');

            //OpenWindow('DocDetail.aspx?DOCID=' + docid + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }

        function onDataBound(e) {
            var grid = $("#kgrid").data("kendoGrid");
            var gridData = grid.dataSource.view();
            for (var i = 0; i < gridData.length; i++) {
                var DOCID = gridData[i].DOCID;
                grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
            }
        }

        function GetIDS() {
            var str = '';
            var CHK = document.getElementById('<%= lstdept.ClientID%>');
            if (CHK != null) {
                //var checkbox = CHK.getElementsByTagName("input");
                //var chr=CHK.outerHTML
                for (var i = 0; i < CHK.length; i++) {
                    //alert(CHK[i].value);
                    if (CHK[i].selected) {
                        str += CHK[i].value + ',';
                    }
                }
               
            }
                   
            //var hdn = document.getElementById("hdndata");
            //hdn.value = str;
            return str;
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
     <script type="text/javascript">
         $(document).ready(function () {
             $(<%=lstdept.ClientID%>).SumoSelect({ selectAll: true });
         });
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
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <Triggers>
                                 <%--<asp:PostBackTrigger ControlID="btnSearch" />--%>
                                <asp:PostBackTrigger ControlID="btnViewInExcel" />
                                 <asp:PostBackTrigger ControlID="lstdept" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="form" style="text-align: left">
                                    <div class="doc_header">
                                        &nbsp;Invoice Status Report
                                    </div>
                                    <div class="row mg">
                                        <div class="col-md-2 col-sm-2">
                                           <label>Department Name</label>
                                        </div>
                                        <%--<div class="col-md-2 col-sm-2">
                                            <asp:DropDownList ID="ddlDocType"  runat="server" CssClass="txtBox">
                                            </asp:DropDownList>
                                        </div>--%>
                                        <div class="col-md-2 col-sm-2">
                                        <asp:listbox runat="server" id="lstdept" selectionmode="Multiple" >
                                        </asp:listbox> </div>
                                       <%-- <div class="col-md-1 col-sm-1">
                                           <label>Filter Action </label>
                                        </div>
                                        <div class="col-md-2 col-sm-2">
                                            <asp:DropDownList ID="ddlfields" CssClass="txtBox" runat="server" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>--%>
                                        <div class="col-md-2 col-sm-2 ">
                                                    <asp:Label ID="Label1"
                                                        runat="server" Font-Bold="True" Text="From Date"></asp:Label>
                                                </div>
                                                <div class="col-md-2 col-sm-2 ">
                                                    <asp:TextBox ID="txtsdate" CssClass="txtBox" runat="server" ></asp:TextBox>
                                                    <asp:CalendarExtender ID="calendersdate" TargetControlID="txtsdate" Format="yyyy-MM-dd"
                                                        runat="server" />
                                                </div>
                                                <div class="col-md-1 col-sm-1 ">
                                                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Text=" To Date"></asp:Label>
                                                </div>
                                                <div class="col-md-2 col-sm-2 ">
                                                    <asp:TextBox ID="txtedate" runat="server" CssClass="txtBox"></asp:TextBox>
                                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd"
                                                        runat="server" />
                                                </div>
                                        
                                   </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12">
                                            <asp:Panel ID="pnl1" runat="server">
                                                <div class="col-md-2 col-sm-2">
                                           <label>Vendor Name</label>
                                        </div>
                                                <div class="col-md-2 col-sm-2">
                                            <asp:DropDownList ID="ddlvendor" CssClass="txtBox" runat="server" >
                                            </asp:DropDownList>
                                        </div>
                                                <div class="col-md-2 col-sm-2">
                                           <label>Status</label>
                                        </div>
                                                <div class="col-md-2 col-sm-2">
                                            <asp:DropDownList ID="ddlstatus" CssClass="txtBox" runat="server" >
                                                 <asp:ListItem Value="0">All</asp:ListItem>
                                                 <asp:ListItem Value="1">Paid</asp:ListItem>
                                                 <asp:ListItem Value="2">Unpaid</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                                
                                                <div class="col-md-4 col-sm-4">
                                            <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/search.png" CssClass="sch"  
                                                ToolTip="Search  " OnClientClick="getReport(); return false;" /> &nbsp;
                                            <%--OnClientClick="getReport(); return false;"--%>
                                            <asp:LinkButton ID="btnViewInExcel" runat="server" CssClass="link" ><b >Click to download in excel</b></asp:LinkButton>
                                           </div>
                                                <br />
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <br>
                                    </br>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12">
                                            <div align="center" style="width: auto">
                                                <div style="max-width: 1350px; overflow-x: scroll;">
                                                    <div id="kgrid" style="height: 550px;">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%-- <table>
                                            <caption>
                                                &nbsp;&nbsp;--%>
                                    <%--   <tr>
                            <td>&nbsp;&nbsp;
                                <asp:Label ID="lblDocType" runat="server" Font-Bold="True" Text="Document Type"></asp:Label>

                            </td>
                            <td></td>
                            <td style="text-align: left; width: 120px">
                                <table style="width: 470px">
                                    <tr>
                                        <td>
                                         
                                        </td>
                                        <td>
                                         
                                        </td>
                                        <td></td>
                                        <td>
                                            
                                        </td>
                                     
                                    </tr>
                                </table>
                            </td>
                            <caption>
                                &nbsp;&nbsp; &nbsp;&nbsp;
                                <caption>
                                    &nbsp;&nbsp;
                                    <caption>
                                        &nbsp;&nbsp;
                                    </caption>
                                </caption>
                            </caption>
                        </tr>--%>
                                    <%--  <tr colspan="3" align="left">
                                                    <td style="text-align: Left" colspan="3">
                                                     
                                                    </td>
                                                </tr>
                                            </caption>
                                        </table>
                                        <br />
                                        <br />--%>
                                    <div align="center">
                                        <asp:Label ID="lblmsg" runat="server" ForeColor="Red"></asp:Label>
                                        <asp:Label ID="lblTab" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </td>
            </tr>
        </table>
        <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlFields"
            TargetControlID="btnShowPopupForm" CancelControlID="btnCloseForm" BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
        <%--<div id="main" >--%>
        <asp:Panel ID="pnlFields" runat="server" Width="750px" Height="430px" BackColor="White"
            Style="display: none;">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td width="980px">
                        <h2 align="center">Advance Search</h2>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td style="width: 100%">
                                            <asp:Label ID="lblForm" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            <div style="margin-left: 600px">
                <asp:ImageButton ID="btnDynamicSearch" runat="server" Height="25px" ImageUrl="~/Images/Asearch.png"
                    ToolTip="Search  " />
            </div>

        </asp:Panel>
        <table cellspacing="0px" cellpadding="0px" width="100%">
            <tr>
                <td></td>
            </tr>

        </table>

    </div>
    <div id="mask">
        <div id="loader" align="center">
            <img src="images/loading.gif" />
        </div>
    </div>
</asp:Content>
