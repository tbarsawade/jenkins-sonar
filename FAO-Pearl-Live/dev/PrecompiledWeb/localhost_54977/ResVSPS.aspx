<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="ResVSPS, App_Web_sfds111l" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />

    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <%--<script src="kendu/js/jquery.min.js"></script>--%>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>
    <link href="css/style.css" rel="stylesheet" />
    <script type="text/javascript" src="kendu/js/jszip.min.js"></script>
    <style type="text/css">
        .basic-grey .basic-grey input[type="email"], .basic-grey textarea, .basic-grey select
        {
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

        .basic-grey .button
        {
            background: darkgreen;
            border: none;
            padding: 9px 25px;
            color: #FFF;
            box-shadow: 1px 1px 5px #B6B6B6;
            border-radius: 3px;
            text-shadow: 1px 1px 1px #9E3F3F;
            cursor: pointer;
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
            height: 50%;
            display: none;
            opacity: 0.9;           
            text-align:center ;
        }

        #loader
        {
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

        .k-grid-toolbar a
        {
            float: right;
        }
        .k-tabstrip-wrapper
        {
            width:1000px;
        }
            .k-grid table tr:hover 
        
        {
            background: rgb(69, 167, 84);
            color: black;

        }
        input#exp
        {
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
            input#exp:hover, input#exp:focus
            {
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
    </style>
    <div class="btnNew" style="text-align:left;color:black; height: 33px;">
        <h1 style="padding-top: 5px;padding-bottom : 5px; padding-left: 7px; margin:0px;" ><b>Res VS PS</b></h1></div>
    <div class="basic-grey" style="width:100%; margin-top: 16px; border: 1px solid #dddddd; border-radius: 5px;">

        <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr style="height:30px;">
                    <td colspan="6">
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; width: 150px">
                        <asp:Label ID="lblDocType" runat="server" Font-Bold="True" Text="From Date"></asp:Label>
                    </td>
                    <td width="20px"></td>
                    <td>
                        <input type="text" id="txtFDate" style="width: 120px" />
                    </td>

                    <td style="text-align: right; width: 120px">&nbsp;
                        <asp:Label ID="lblSearch" runat="server" Font-Bold="True" Text="ToDate"></asp:Label>
                    </td>
                    <td width="20px"></td>
                    <td>
                        <input type="text" id="txtToDate" style="width: 120px" />
                    </td>
                    <td width="20px" style="padding-right: 74px;">
                        <input type="button"  id="btnSearch" value="Search" />
                        <input type="hidden" value="0" id="hdnSiteID" />
                        <div id="dvbuttonloader" style="position: absolute; display: none; z-index: 1; left: 50%; top: 50%; z-index: 10001">
                            <input type="image" src="http://localhost:4836/../images/loading12.gif" style="height: 25px;" />
                    </td>
                </tr>
                <tr style="height:30px;">
                    <td colspan="6">
                    </td>
                </tr>
            </table>
    </div>
    <div id="dvloader" style="position: absolute; display: none; z-index: 1; left: 50%; top: 50%; z-index: 10001">
        <input type="image" id="Imageprog" src="http://localhost:4836/../images/prg.gif" style="height: 25px;" />
    </div>
    <div id="kgrd" style="margin-top: 15px;"></div>
    <div id="dvNoRecord" style="display: none; text-align: center; width: 100%; min-height: 80px; border: solid 1px #DDD;">
        <span style="color: red; position: relative; top: 30px;">No record found</span>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            //$("#wrap").css("width", "100%");
            //var div = $("div .content");
            //div.addClass("mycontent");
            //div.removeClass("content");
            var p = $("#txtFDate,#txtToDate").kendoDatePicker({
                value: new Date(),
                //dates: disabledDaysBefore,
                format: "yyyy/MM/dd", //"dd MMMM yyyy",
                //change: onChangeVehicle,
                //min: new Date(),

            });
            $("#txtFDate,#txtToDate").attr("readonly", "readonly");
            $("#btnSearch").kendoButton()
            $("#btnSearch").kendoButton({ click: GetData });
        });
        var kGrid
        function GetData() {
            var FromDate = $.trim($('#txtFDate').val());
            var ToDate = $.trim($('#txtToDate').val());
            var UID = '<%= Session("UID").ToString()%>';
            var URole = '<%= Session("USERROLE").ToString()%>';
            if (FromDate != '' && ToDate != '') {
                //$("#dvResVsPS").show();
                $("#dvloader").show();
                //var dataItem = this.dataItem(e.item.index());
                if ($(kGrid).length > 0) {
                    $('#kgrd').empty();
                    kGrid.data().kendoGrid.destroy();
                }
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ResVSPS.aspx/getData",
                    global: false,
                    data: '{FromDate: "' + FromDate + '",ToDate: "' + ToDate + '" ,uid:"'+UID+'",urole:"'+URole+'"}',
                    dataType: "json",
                    async: true,
                    success: function (data) {
                        //  var ds = JSON.parse(data.d);

                        var ds = $.parseJSON(data.d.data);
                        var columns = data.d.columns;
                        var aggregate = data.d.aggregate;
                        kGrid = $("#kgrd").kendoGrid({
                            toolbar: ['excel'],
                            excel: {
                                fileName: "Report.xlsx",
                                filterable: true,
                                pageable: true,
                                allPages: true
                            },
                            dataSource: {
                                pageSize: 20,
                                data: ds,
                                aggregate: aggregate
                            },

                            columns: columns,
                            pageable: true,
                            filterable: true,
                            sortable: true,
                            resizable: true
                        });
                        $("#dvloader").hide();
                        $("#kgrd").show();
                        $("#dvNoRecord").hide();
                        if (ds.length == 0) {
                            $("#kgrd").hide();
                            $("#dvNoRecord").show();
                            $("#dvloader").hide();
                        }
                    }
                });

            }
            else {
                $("#dvResVsPS").hide();
                $("#dvloader").hide();
            }
        }
    </script>
</asp:Content>

