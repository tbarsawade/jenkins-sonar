﻿<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="NewIdeaReport, App_Web_c5kjwoe4" viewStateEncryptionMode="Always" %>

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
        .textbox
        {
            border: 1px solid #848484;
            -webkit-border-radius: 30px;
            -moz-border-radius: 30px;
            border-radius: 30px;
            outline: 0;
            height: 25px;
            width: 275px;
            padding-left: 10px;
            padding-right: 10px;
        }
    </style>
    <script type="text/javascript">
        //        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        //        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
        //var textval = $('#txtDslip').val()

        function GetReport() {


            // var str = '{ "d1": "' + $("#ddlStatus").val() + '"}';
            //  var dslipnum = $('#txtDslip').val();
            $.ajax({
                type: "POST",
                url: "NewIdeaReport.aspx/GetData",
                data: {},
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
            var CommanObj = { command: [{ name: "Details", text: "", click: DetailHandler, imageClass: "k-icon k-i-pencil"}], title: "Details", width: 60 };
            var Columns = result.d.Column;
            Columns.splice(0, 0, CommanObj);
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
          var DetailHandler = function DetailHandler(e) {
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            OpenWindow('DocDetail.aspx?DOCID=' + dataItem.TID + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }

        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }
        $(document).ready(GetReport);

    </script>
    <br />

    <div style="width:100%;height:47px;padding-top:15px;padding-left:10px;font:bold 17px 'verdana';color:#fff;background-image:url(logo/hfclstrip.jpg);background-repeat:no-repeat">New Idea Report</div>
    <%--<div >
        <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0"
            class="m9">
            <tr>
                <td>
                    <asp:Label ID="lblmsg" runat="server" style="width: 100%; padding-top: 5px; color: #E96125; font-family: Calibri;
        font-size: large; font-weight: bold;" Text="New Idea Report"></asp:Label>
                </td>
            </tr>
        </table>
    </div>--%>
    <br />
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
