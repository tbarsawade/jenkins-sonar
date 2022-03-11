<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="PHRODetails, App_Web_sfds111l" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <%--<script src="kendu/js/jquery.min.js"></script>--%>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>
    <link href="css/style.css" rel="stylesheet" />
    <script type="text/javascript">
        var new_window;
        var filtersNeedToAct, filterMyReq, filterHistory; //= $("#kgrd").data("kendoGrid").dataSource.filter();
        
        function childClose() {
            //if (new_window.closed) {
            if ($(kGrid).length > 0)
                filtersNeedToAct = $("#kgrd").data("kendoGrid").dataSource.filter();
            if ($(kgrdMyReq).length > 0)
                filterMyReq = $("#kgrdMyReq").data("kendoGrid").dataSource.filter();
            if ($(kgrdHistory).length > 0)
                filterHistory = $("#kgrdHistory").data("kendoGrid").dataSource.filter();
            
            var DocumentType = $("#ContentPlaceHolder1_ddldocType").val();
            BindNeedToAct(DocumentType);
            GetGridMyReq(DocumentType);
            GetGridHistory(DocumentType);
            //$("#ContentPlaceHolder1_btnpendinggrdcl").click();

            //}
           
          //  alert("hi");
        }
    </script>
    <style type="text/css">
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
        }

        .mycontent {
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 0 5px #a0a0a0;
            margin: 132px auto 20px !important;
            min-height: 430px !important;
            padding: 15px;
            width: 100% !important;
            box-sizing: border-box;
        }

        .modalPopup {
            background-color: #FFFFFF;
            width: 350px;
            border: 3px solid #0DA9D0;
            border-radius: 12px;
            padding: 0;
        }

            .modalPopup .header {
                background-color: red;
                height: 30px;
                color: White;
                line-height: 30px;
                text-align: center;
                font-weight: bold;
                border-top-left-radius: 6px;
                border-top-right-radius: 6px;
            }

            .modalPopup .body {
                min-height: 50px;
                line-height: 30px;
                text-align: center;
                font-weight: bold;
            }

            .modalPopup .footer {
                padding: 6px;
            }

            .modalPopup .yes, .modalPopup .no {
                height: 23px;
                color: White;
                line-height: 23px;
                text-align: center;
                font-weight: bold;
                cursor: pointer;
                border-radius: 4px;
            }

            .modalPopup .yes {
                background-color: #598526;
                border: 1px solid #5C5C5C;
            }

            .modalPopup .no {
                background-color: #598526;
                border: 1px solid #5C5C5C;
            }

        .menu-bar {
            width: 95%;
            margin: 4px 0px 0px 0px;
            padding: 1px 1px 1px 1px;
            height: auto;
            line-height: inherit;
            border-radius: 10px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            box-shadow: 2px 2px 3px #666666;
            -webkit-box-shadow: 2px 2px 3px #666666;
            -moz-box-shadow: 2px 2px 3px #666666;
            background: #8B8B8B;
            background: linear-gradient(top, #ccc7c5, #7A7A7A);
            background: -ms-linear-gradient(top, #ccc7c5, #7A7A7A);
            background: -webkit-gradient(linear, left top, left bottom, from(#ccc7c5), to(#7A7A7A));
            background: -moz-linear-gradient(top, #ccc7c5, #7A7A7A);
            border: solid 1px #6D6D6D;
            position: relative;
            /*z-index:999;
  -webkit-z-index:999;*/
        }

            .menu-bar li {
                margin: 0px 0px 1px 0px;
                padding: 0px 1px 0px 1px;
                float: left;
                position: relative;
                list-style: none;
            }

            .menu-bar a {
                text-decoration: none;
                padding: 1px 2px 1px 2px;
                margin-bottom: 1px;
                border-radius: 1px;
                -webkit-border-radius: 1px;
                -moz-border-radius: 1px;
                font-weight: bold;
                font-family: arial;
                font-style: normal;
                font-size: 12px;
                color: #d0e41f;
                text-shadow: 1px 1px 1px #000000;
                display: block;
                margin: 0;
                margin-bottom: 1px;
                border-radius: 4px;
                -webkit-border-radius: 4px;
                -moz-border-radius: 4px;
                text-shadow: 2px 2px 3px #000000;
            }

            .menu-bar li ul li a {
                margin: 0;
            }

            .menu-bar .active a, .menu-bar li:hover > a {
                background: #0399D4;
                background: linear-gradient(top, #EB2F2F, #960000);
                background: -ms-linear-gradient(top, #EB2F2F, #960000);
                background: -webkit-gradient(linear, left top, left bottom, from(#EB2F2F), to(#960000));
                background: -moz-linear-gradient(top, #EB2F2F, #960000);
                color: #F2F2F2;
                -webkit-box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                -moz-box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                text-shadow: 2px 2px 3px #FFA799;
            }

            .menu-bar ul li:hover a, .menu-bar li:hover li a {
                background: none;
                border: none;
                color: #666;
                -box-shadow: none;
                -webkit-box-shadow: none;
                -moz-box-shadow: none;
            }

            .menu-bar ul a:hover {
                background: #0399D4 !important;
                background: linear-gradient(top, #EB2F2F, #960000) !important;
                background: -ms-linear-gradient(top, #EB2F2F, #960000) !important;
                background: -webkit-gradient(linear, left top, left bottom, from(#EB2F2F), to(#960000)) !important;
                background: -moz-linear-gradient(top, #EB2F2F, #960000) !important;
                color: #FFFFFF !important;
                border-radius: 0;
                -webkit-border-radius: 0;
                -moz-border-radius: 0;
                text-shadow: 2px 2px 3px #FFA799;
            }

            .menu-bar ul {
                background: #DDDDDD;
                background: linear-gradient(top, #FFFFFF, #CFCFCF);
                background: -ms-linear-gradient(top, #FFFFFF, #CFCFCF);
                background: -webkit-gradient(linear, left top, left bottom, from(#FFFFFF), to(#CFCFCF));
                background: -moz-linear-gradient(top, #FFFFFF, #CFCFCF);
                display: none;
                margin: 0;
                padding: 0;
                width: 0px;
                position: absolute;
                /*top: 10px;*/
                left: 0;
                border: solid 1px #B4B4B4;
                border-radius: 10px;
                -webkit-border-radius: 10px;
                -moz-border-radius: 10px;
                -webkit-box-shadow: 2px 2px 3px #222222;
                -moz-box-shadow: 2px 2px 3px #222222;
                box-shadow: 2px 2px 3px #222222;
            }

            .menu-bar li:hover > ul {
                display: block;
            }

            .menu-bar ul li {
                float: none;
                margin: 0;
                padding: 0;
            }

        .menu-bar {
            display: inline-block;
        }

        html[xmlns] .menu-bar {
            display: block;
        }

        * html .menu-bar {
            height: 1%;
        }

        .doclink li {
            list-style: none;
            text-decoration: none;
            color: #000;
            padding: 6px 5px;
            border-bottom: 1px solid rgba(0, 0, 0, .2);
        }

            .doclink li a {
                text-decoration: none;
            }

                .doclink li a:hover {
                    text-decoration: none;
                }

            .doclink li:hover {
                background: #598526;
                color: #fff !important;
            }


        .mar_botm {
            margin-bottom: 10px;
            border: 1px solid green;
            text-align: left;
        }

        /*.mar_botm div {
            border: 1px solid green; overflow: auto; width: 100%; text-align: left; padding-left: 0px;display:none; 
        }*/
        .divCustom {
            border: 1px solid green;
            overflow: auto;
            width: 100%;
            text-align: left;
            padding-left: 0px;
            display: none;
        }

        .divCustom1 {
            border: 1px solid green;
            overflow: auto;
            width: 100%;
            text-align: left;
            padding-left: 0px;
        }

        .loader {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .loader1 {
            background-image: url("images/loading.gif");
            background-repeat: no-repeat;
            background-position: center center;
            position: relative;
            top: calc(50% - 16px);
            display: block;
            height: 50px;
        }

        .tdhover {
            background: #c6def5;
        }

            .tdhover:hover {
                color: #3585f3;
                text-underline-position: below;
            }

        .right-button {
            text-align: right;
        }

            .right-button .btn-default {
                background-color: #E96125;
                color: #fff;
                border: none;
                font-weight:bold;
                font-size:12px;
            }

                .right-button .btn-default:hover, .btn-default:focus, .btn-default:active, .btn-default.active {
                    border: none;
                }

        #wrap {
            width: 98%;
            max-width: 98%;
            padding: 0px !important;
            margin: 0 auto;
            box-sizing: border-box;
            -moz-box-sizing: border-box;
            -webkit-box-sizing: border-box;
        }
        .k-grid table tr:hover 
        {
            background: rgb(69, 167, 84);
            color: black;
        }
        .k-window  div.k-window-content
        {
            overflow: hidden;
        }
        .k-grid-toolbar a {
            float: right;
        }

    </style>
    <%--<table width="100%" cellspacing="0px" cellpadding="0px">
        <tr valign="top">
            <td style="width: 100%; display: block; float: left;">
                <table style="width: 100%">
                    <tr valign="top">

                        <td>
                        </td>

                        <td class="right-button">
                            <div id="dvbtnloader" style="display: block; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/loading12.gif" />
                            </div>
                            <div id="dvWidgetsBtn">
                                <input type="button" class="MainHomeLink" id="btnUdate" value="Update" onclick="javascript: openPopup() " />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />--%>
    <div style="text-align: left; color:#ffffff; height:30px; background-color:#e96125; margin-bottom:20px; border-radius:5px;">
        <h1 style="line-height:28px; padding-left:5px;"><b>Pocket HRO Details</b></h1>
    </div>
    <div style="width: 100%; display: block;">
        <div class="demo-section k-header">
                <div style="padding: 0px;">
                    <div>
                        <div id="dvneedToAct" style="width: 100%; overflow-x: auto; margin: 0px; padding: 0px;">
                            <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kgrd" style="display: block; overflow-x: auto;"></div>
                            <div id="NoRecord1" style="display: none; background-color:#FAFAFA; text-align: center; min-height: 80px;">
                                <span style="color: red; position: relative; top: 30px;">No record found</span>
                            </div>
                        </div>
                    </div>
                </div>
        </div>
    </div>
    <div class="mar_botm" id="wdgtwrap" style="display: none;">
        <div id="dv1" style="display: block;">
            <table width="100%" align="center" cellspacing="0" cellpadding="0" border="0">
                <tr height="50">
                    <td width="20%">
                        Status
                    </td>
                    <td width="5%"></td>
                    <td width="75%">
                        <select id="ddlSelect">
                            <option value="">Select</option>
                            <option value="Aprroved">Aprroved</option>
                            <option value="Reject">Reject</option>
                        </select>
                    </td>
                </tr>
                <tr height="50">
                    <td>
                        Comment
                    </td>
                    <td></td>
                    <td>
                        <textarea id="txtComment"></textarea>
                    </td>
                </tr>
                <tr height="50">
                    <td class="right-button" style="text-align:center;" colspan="3">
                        <div id="dvbtnloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/loading12.gif" />
                        </div>
                        <div id="dvUpdate">
                            <input type="button" class='MainHomeLink' id="btnUpdate" value="Update" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <script type="text/javascript">
       
     //   $("#kgrd").data("kendoGrid").refresh();
       

        $(document).ready(function () {
            $("#ddlSelect").kendoDropDownList();
            BindPHRODetails();
        });

        function BindPHRODetails() {
            $("#dvloader").show();
            $("#kgrd").hide();
            //var t = '{ DocumentType: "' + documentType + '" }';
            $.ajax({
                type: "POST",
                url: "PHRODetails.aspx/GetPHRODetails",
                contentType: "application/json; charset=utf-8",
                data: null,
                dataType: "json",
                success: function (response) {
                 
                    if ($(kGrid).length > 0) {
                        //$('#kgrd').data().kendoGrid.destroy();
                        $('#kgrd').empty();
                    }
                    var strData = response.d.Data;
                    var data1 = $.parseJSON(strData);
                    var CommanObj = { command: [{ name: "Details", text: "Update",title: 'View Event', click: DetailHandler }], title: "Action", width: 80 }
                    var Columns = response.d.Column;
                    Columns.push(CommanObj);
                    //Columns.splice(0, 0, CommanObj);
                    $("#spnNeedToAct").html("(" + response.d.Count + ")");
                    //Columns.push(CommanObj);
                    if (data1.length > 0) {
                        $("#kgrd").show();
                        $("#NoRecord1").hide();
                        bindGrid(data1, Columns);
                        if (filtersNeedToAct != null) {
                            $("#kgrd").data("kendoGrid").refresh();
                            $("#kgrd").data("kendoGrid").dataSource.filter(filtersNeedToAct);
                            //alert('Hi');
                        }
                    }
                    else {
                        $("#kgrd").hide();
                        $("#NoRecord1").show();

                    }

                    $("#dvloader").hide();
                    $("#kgrd").show();
                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
           
        }

        var kGrid
        function bindGrid(Data1, columns) {
            kGrid = $("#kgrd").kendoGrid({
                dataSource: {
                    pageSize: 10,
                    data: Data1,
                    columns: columns,
                    //serverPaging: true,
                    filterable: {
                        cell: {
                            showOperators: false
                        }
                    }
                    
                },
                scrollable: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                filterable: {
                    mode: "row"
                },
                reorderable: true,
                sortable: true,
                resizable: true,
                height: 400,
                width: 1386,
                columns: columns
            });
            $(".k-grid-Details").attr('title', "Update");
            $(".k-button").click(function () {
                $(".k-grid-Details").attr('title', "Update");
            });
            
            //$(".k-grid-Details").kendoTooltip({ content: "Show Details" });
        }

        var DetailHandler = function DetailHandler(e) {
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            $("#ddlSelect").val(dataItem.AdminAppr_Status);
            $("#txtComment").val(dataItem.Comment)
            $("#ddlSelect").kendoDropDownList();
            var window = $("#wdgtwrap");
            window.kendoWindow({
                title: "APPR Status",
                modal: true,
                width: 500,
                resizable: true,
                scrollable: false,
                open: function () {
                    $('#wdgtwrap').show();
                }
            });
            window.data("kendoWindow").setOptions({ width: 500 });
            window.data("kendoWindow").center().open();
            $("#wdgtwrap_wnd_title").html("APPR Status");
            $("#btnUpdate").unbind('click');
            $("#btnUpdate").bind('click', function () {
                $("#dvUpdate").hide();
                $("#dvbtnloader").show();
                var Tid = dataItem.Tid;
                var Status = $("#ddlSelect").val();
                var comment = $("#txtComment").val();
                if (Status == "") {
                    alert("Please select status")
                    $("#dvUpdate").show();
                    $("#dvbtnloader").hide();
                }
                else {
                    var t = '{ Tid: "' + Tid + '",Status:"' + Status + '",Comment:"' + comment + '"}';
                    $.ajax({
                        type: "POST",
                        url: "PHRODetails.aspx/SetApprStatus",
                        contentType: "application/json; charset=utf-8",
                        data: t,
                        dataType: "json",
                        success: function (response) {
                            var strData = response.d.result;
                            if (strData != "fail") {
                                alert("Appr status updated successfully.")
                                $("#wdgtwrap").data("kendoWindow").close();
                                BindPHRODetails();
                                $("#dvUpdate").show();
                                $("#dvbtnloader").hide();
                            }
                            else {
                                alert("Sorry! Erorr occured in server. Please try again.")
                                $("#dvUpdate").show();
                                $("#dvbtnloader").hide();
                            }
                            e.preventDefault();
                        },
                        error: function (data) {
                            //Code For hiding preloader
                        }
                    });
                }
            });
            e.preventDefault();
        }

    </script>
    <script>
        $("form").submit(function (e) {
            e.preventDefault();
        });
    </script>
</asp:Content>

