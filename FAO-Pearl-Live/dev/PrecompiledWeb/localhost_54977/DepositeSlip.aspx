<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="DepositeSlip, App_Web_o3dtvhns" viewStateEncryptionMode="Always" %>

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
        #divFilters
        {
            padding: 15px;
            background: #F7F7F7;
            min-height: 60px; /*width:1000px;*/
            box-sizing: border-box;
        }
        
        #mask
        {
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
        
        #loader
        {
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
        
        .k-grid-header .k-header
        {
            overflow: visible;
            white-space: normal;
        }
        .textbox { 
    border: 1px solid #848484; 
    -webkit-border-radius: 30px; 
    -moz-border-radius: 30px; 
    border-radius: 30px; 
    outline:0; 
    height:25px; 
    width: 275px; 
    padding-left:10px; 
    padding-right:10px; 
  } 
    </style>
    <script type="text/javascript">
//        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        //        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
        //var textval = $('#txtDslip').val()
       
        function GetReport() {
            var str = '{ "dslipnum": "' + $("#ContentPlaceHolder1_txtDslip").val() + '" }';
            //  var dslipnum = $('#txtDslip').val();
            $.ajax({
                type: "POST",
                url: "DepositeSlip.aspx/GetDSlip",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

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
                toolbar: ['excel'],
                excel: {
                    fileName: "Report.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }

    </script>



 





    <div id="divFilters">
        <table style="width: 100%;">
            <col style="width: 30%" />
            <col style="width: 30%" />
            <col style="width: 15%" />
            <col style="" />
            <tr>
                <th>
                    &nbsp;
                </th>
                <th>
                    &nbsp;
                </th>
                <th>
                </th>
                <th>
                </th>
            </tr>
            <tr>
                <td style="text-align: right">
                    <b>Deoposite Slip Number : &nbsp;</b>
                </td>
                <td>
                <%-- <input class="textbox"type="text"  ID="txtDslip"/> --%>
                    <asp:TextBox ID="txtDslip" Style="width: 80%;" runat="server" 
                        BorderColor="#339933" BorderStyle="Outset"></asp:TextBox>
                </td>
                <td style="width:260px">
                    <input id="btnReport"  class="btnNew" type="button" value="Search" onclick="GetReport()" />

                   
                    <%--        <asp:Button ID="btnSearch"  runat="server" OnClientClick="GetReport();" Text="Get Report" />--%>
                             <asp:Button ID="btnDownload"  runat="server"  Text="Download Slip" CssClass="btnNew" />
                </td>
                <td style="vertical-align: top; color: red; font-weight: bold; width:60px">
                    <asp:Label ID="lblReport" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
        </table>
    </div>
    <div>
        <div id="kgrid" style="width: 999px">
        </div>
    </div>
    <div id="mask">
        <div id="loader">
            <img src="images/loading.gif" />
        </div>
    </div>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
          <%--      <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/images/attch.gif" />--%>
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
