<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="NewMaster, App_Web_dqvq3srr" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <link rel="stylesheet" href="kendu/homekendo.common.min.css" />
    <link rel="stylesheet" href="kendu/homekendo.rtl.min.css" />
    <%--<link rel="stylesheet" href="kendu/homekendo.silver.min.css" />--%>
    <link rel="stylesheet" href="kendu/kendo.uniform.min.css" />

    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="kendu/homekendo.mobile.all.min.css" />

    <%--<script src="kendu/js/jquery.min.js"></script>--%>
    <%--kendo.data.min.js--%>
     <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
     <%--<script src="kendu/homejquery-1.9.1.min.js"></script>--%>
    <script src="kendu/homekendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <%--<script src="kendu/content/shared/js/console.js"></script>
    <%--<script src="kendu/js/jszip.min.js"></script>--%>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>

    <script src="Jquery/jquery-ui-v1.12.1.js"></script>
    <script src="mobile/js/jquery-ui-1.10.3.custom.js"></script>

    <%--<link href="css/ui-lightness/jquery-ui-1.10.2.custom.css" rel="stylesheet" />--%>
      <%--<link href="js_child/jquery-ui[1].css" rel="stylesheet" />--%>

    <link href="css/style.css" rel="stylesheet" />
    <link href="css/Master.css" rel="stylesheet" />
    <style type="text/css">
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
        }

        /*.mycontent {
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 0 5px #a0a0a0;
            margin: 115px auto 20px !important;
            min-height: 430px !important;
            padding: 15px;
            width: 100% !important;
            box-sizing: border-box;
        }*/

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
        .k-MEidt {
            background:url(../images/edit.jpg) no-repeat top left;
            display: inline-block;
            height: 21px;
            width: 20px;
            border-color: transparent;
            background-position: center; 
    }
        .k-MDetials {
            background:url(../images/seeall.png) no-repeat top left;
            display: inline-block;
            height: 21px;
            width: 20px;
            border-color: transparent;
            background-position: center; 
    }
        .k-MLock {
            background:url(../images/lock.png) no-repeat top left;
            display: inline-block;
            height: 21px;
            width: 20px;
            border-color: transparent;
            background-position: center; 
    }
    .verror{
        box-shadow: 0 0px 5px #ff0000;
        border: solid 1px #ff0000 !important;
    }

        /*.k-window-action
{
    visibility: hidden ;
}*/
        .ui-widget-header {
    color: #2e2e2e;
    font-weight: bold;
}
        .k-button
        {
            min-width:0px !important;
        }
        .k-grid {
        font-size: 11px;
    }
    .k-grid td { 
        line-height: 11px;
    }
    .k-grid th { 
        font-size: 13px;
        font-weight:bold;
    }
    .k-window-titlebar
    {
        background-color:#e96125;
    }
    .k-window-title
    {
        font: bold 15px 'verdana';
        color: white;
    }
    </style>
    <%--<div style="text-align: left; color:#ffffff; height:30px; background-color:#e96125; margin-bottom:10px; border-radius:5px; font: bold 17px 'verdana';">
        &nbsp;&nbsp;<span id="spnHeading"></span>
    </div>--%>
    <asp:HiddenField ID="hdnDocumentType" runat="server" />
        <%--<table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
        <tr style="text-align: left; color:#ffffff; height:30px; background-color:#e96125; margin-bottom:10px; border-radius:5px; font: bold 17px 'verdana';">
            <td style="text-align: left; width: 100%; border: 2px solid green; padding:5px;">
                <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"    border="0px">
                    <tr>
                        <td><span id ="spnHeading1" style="font: bold 17px 'verdana';"></span></td>
                        <td style="text-align: right;">&nbsp;
                            <input type="image" id="btnAdd" src="Images/plus.jpg" style="height:20px;width:30px;" title="Add new item"  onclick="javascript: return ADDMaster(); return false;"/>
                            <asp:ImageButton ID="btnchangeView" runat="server" ImageUrl="~/images/Viewmapicon.jpg" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" Height="17px" Visible="false" />
                        </td>
                    </tr>
                    
                </table>
            </td>
        </tr>
    </table>--%>
    <br />
    <div style="width: 100%; display: block; margin-top:-18px;">
        <div class="demo-section k-header">
            <div style="padding: 0px;">
                <div>
                    <div id="dvMaster" style="width: 100%; overflow-x: auto; margin: 0px; padding: 0px;">
                        <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                            <input type="image" src="../images/preloader22.gif" />
                        </div>
                        <div id="kgrdMaster" style="display: block; overflow-x: auto;"></div>
                        <div id="NoRecord1" style="display: none; text-align: center; min-height: 80px;">
                            <span style="color: red; position: relative; top: 30px;">No record found</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

   <div id="window1" style="display:none">
    </div>
    <div id="dvControls" style="display:none;" runat="server">

    </div>
    <input type="hidden" id="hdnMethod" value="" />

    <div id="popLoader" class="ui-dialog-content ui-widget-content" style="width: auto; display:none; min-height: 0px; max-height: none; height: auto; overflow: hidden;">
        <div style="text-align:center; padding:15px;">Please wait...<br>
            <img src="../images/preloader22.gif" alt="Please wait...">
        </div>
    </div>
    <br /><br />

    <div id="dvLockUnloack" Width="500px" style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <%--<h3>Lock / Unlock : Confirmation</h3>--%>
                    </td>
                    <td style="width: 20px">
                    </td>
                </tr>
                <tr> 
                    <td colspan="2">
                        <span id="lblLock" style="font:bold; color:red;"></span>
                        <div style="width: 100%; text-align: right">
                            <input type="button" id="btnLockupdate" value="Yes Lock" style="width:90px; height:12px;" class="btnNew" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <script type="text/javascript">
        function OpenPopLoader() {
            var window = $("#popLoader");
            window.kendoWindow({
                modal: true,
                open: function () {
                    $('#popLoader').show();
                }
            });
            window.data("kendoWindow").center().open();
            $("#popLoader").parent().find(".k-window-action").css("visibility", "hidden");
            //$("#popLoader").parent().find(".k-window-titlebar").css("visibility", "hidden");
            $('#popLoader').parent().find('.k-window-titlebar,.k-window-actions').css('display', 'none');
            $('#popLoader').css('overflow', 'hidden');
            $('#popLoader').css('height', 'auto');
        }
        function ClosePopLoader() {
            $("#popLoader").data("kendoWindow").close();
        }

        //Code By Ajeet Kumar opening partial View
        $(document).ready(function () {
            $("#btnSave").click(function () {
                validate();
            });
        })
        var KgridFilter;
        function validate(e) {
            //e.preventDefault();
            var ObjCtrl = $("[bpminput=true]");
            //removing error class from inputs
            $(ObjCtrl).removeClass("verror");
            //variable declration
            var Key = "";
            var Value = "";
            var DataStr = "";
            var vres = { Msg: "Error(s) in your submission.Please fill all the mandatory fields.\n-----------------------------\n", IsValid: true };
            var type = "";
            $(ObjCtrl).each(function () {
                type = $(this).prop("type");
                //alert(type);
                switch (type) {
                    case "text":
                        {
                            //Creating Data string for posting
                            key = $(this).attr("key");
                            Value = $(this).val();
                            DataStr = DataStr + "|" + key + "::" + Value;
                            if ($(this).attr('mandatory') == "1" && Value.trim() == "") {
                                vres.IsValid = false;
                                vres.Msg += key + " required.\n";
                                $(this).addClass('verror');
                            }
                            break;
                        }
                    case "select-one":
                        {
                            //alert($(this).hasClass('req'));
                            //alert($(this).attr('mandatory'));
                            key = $(this).attr("key");
                            Value = $(this).find('option:selected').text();
                            DataStr = DataStr + "|" + key + "::" + Value;
                            if ($(this).attr('mandatory') == "1" && $(this).val() == "0") {
                                vres.IsValid = false;
                                vres.Msg += key + " required.\n";
                                $(this).addClass('verror');
                            }
                            //DataStr = DataStr + "|" + $(this).attr("key") + "::" + $(this).find('option:selected').text();
                            break;
                        }
                    case "checkbox":
                        {
                            key = $(this).attr("key");
                            var mand = $(this).attr("mandatory");
                            var chktext = "";
                            $('input[name="chkCheckBoxList"]:checked').each(function () {
                                if (chktext == "")
                                    chktext = $(this).val();
                                else
                                    chktext = chktext + "," + $(this).val();
                            });
                            DataStr = DataStr + "|" + key + "::" + chktext;
                            if (chktext == "" && mand == "1") {
                                vres.IsValid = false;
                                vres.Msg += key + " required.\n";
                                $(this).addClass('verror');
                            }
                            break;
                        }
                    //case "AUTOCOMPLETE"
                    default:
                        break;
                }
                //$('#hdn' + tid).val()
                //Each function end here
            });
            if (vres.IsValid) {
                //every thing is fine logig of save
                //var checkstr = confirm("Hi I am going to save it" + DataStr);
                checkstr = true;
                if (checkstr == true) {
                    $("#lblmsg").hide();
                    var documentType = getParameterByName('SC');
                    var SaveMethod = $("#hdnMethod").val();
                    var Data = '{strData:"' + DataStr + '",DocType:"' + documentType + '",SMethd:"' + SaveMethod + '"}';
                    //alert(Data);
                    $.ajax({
                        type: "POST",
                        url: 'NewMaster.aspx/Save',
                        data: Data,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        beforeSend: function (e) {
                            OpenPopLoader();
                        },
                        complete: function (e) {
                            ClosePopLoader();
                        },
                        success: function (response) {
                            if (response.d.indexOf("Your DocID is") >= 0) {
                                alert(response.d);
                                if ($(kGrid).length > 0)
                                    KgridFilter = $("#kgrdMaster").data("kendoGrid").dataSource.filter();
                                GetColumnGrid(documentType);
                                $("#ContentPlaceHolder1_dvControls").data("kendoWindow").close();
                            }
                            else if (response.d == "Record updated successfully.") {
                                alert(response.d);
                                if ($(kGrid).length > 0)
                                    KgridFilter = $("#kgrdMaster").data("kendoGrid").dataSource.filter();

                                GetColumnGrid(documentType);
                                GetColumnGrid(documentType);
                                $("#ContentPlaceHolder1_dvControls").data("kendoWindow").close();
                            }
                            else {
                                var width = $(".k-window-content").width() + 'px';
                                $('.k-window-content').prepend('<label id="lblmsg"><p style="width:' + width + ';color:red;">' + response.d + '</p></label>');
                            }
                        }
                    });
                }
            }
            else {
                alert(vres.Msg);
                //var obj = $('.verror');
                //$(obj).each(function () {
                //    $(this).css('background-color', 'red');
                //});
            }
            //btnSave
        }
        function ADDMaster() {
            var doc = $("#ContentPlaceHolder1_hdnDocumentType").val();
            var Data = '{DocType:"' + doc + '",EID:"",Tid:"0"}';
            //alert(Data);
            $.ajax({
                type: "POST",
                url: 'NewMaster.aspx/CreatedDynamicFields',
                data: Data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend:function(e)
                {
                    OpenPopLoader();
                },
                complete:function(e)
                {
                    ClosePopLoader();
                },
                success: function (response) {
                    $("#hdnMethod").val("0");
                    $("#ContentPlaceHolder1_dvControls").html("");
                    $("#ContentPlaceHolder1_dvControls").html(response.d);

                    //$(".contact main").append("<div><input type=\"button\" id=\"btnSave\" class='MainHomeLink' value=\"Save\" onclick='javascript:validate();' /></div>");
                    //$("#ContentPlaceHolder1_dvControls").append("<div><input type=\"button\" id=\"btnSave\" class='MainHomeLink' value=\"Save\" onclick='javascript:validate();' /></div>");

                    $("input[datatype='Datetime']").datepicker({
                        showOn: "button",
                        buttonImage: "images/cal.png",
                        buttonImageOnly: true,
                        buttonText: "Select date",
                        dateFormat: 'dd/mm/y',
                        //startDate: new Date(),
                        showOn: "both"
                    });
                    $("input[datatype='Datetime']").datepicker("setDate", new Date());
                    openkendowindow(doc);
                }
            });
            return false;
            //Click handler End
        }
        function openkendowindow(doc) {
            var window = $("#ContentPlaceHolder1_dvControls");
            window.kendoWindow({
                title: doc,
                modal: true,
                actions: [
                    "Minimize",
                    "Maximize",
                    "Close"
                ],
                open: function () {
                    $('#ContentPlaceHolder1_dvControls').show();
                }
            });
            window.data("kendoWindow").center().open();
            AddHandler();
        }
        function ApplyKendu(Container) {
            $("#tabstrip").kendoTabStrip({
                animation: {
                    open: {
                        effects: "fadeIn"
                    }
                }
            });
            $(".datepicker").kendoDatePicker({
                format: "dd/MM/yy"
            });
            $(".ChkList").kendoMultiSelect().data("kendoMultiSelect");
            $(".dropdown").kendoDropDownList();
            $(".num").kendoNumericTextBox({ decimals: 2 });
            $('[type="button"]').kendoButton();
            $('[type="file"]').kendoUpload();
        }
        function validationEvent() {
            var ObjCtrl = $("[bpminput='true'].req");
            $(ObjCtrl).each(function () {
                type = $(this).prop("type");
                switch (type) {
                    case "text":
                        {
                            $(this).on('change keyup paste mouseup', function () {
                                if ($(this).val().trim() == "") {
                                    $(this).closest('.k-numerictextbox').addClass('verror');
                                    $(this).addClass('verror');
                                }
                                else {
                                    $(this).closest('.k-numerictextbox').removeClass('verror');
                                    $(this).removeClass('verror');
                                }
                            });
                            break;
                        }
                    case "select-one":
                        {
                            //binding select change event to a dropdown for validation
                            $(this).bind("change", {}, function (e) {
                                if ($(this).data("kendoDropDownList").value() == "0") {
                                    $(this).closest('.k-dropdown').addClass('verror');
                                }
                                else {
                                    $(this).closest('.k-dropdown').removeClass('verror');
                                }
                            });
                            break;
                        }
                    default:
                        break;
                }
                //Each function end here
            });
        }
        function AddHandler() {
            //Getting All the dropdown for which server side handler is configured
            var ObjCtrl = $("[allowfilter=1]");
            $(ObjCtrl).each(function () {
                //alert("Assigning event.");
                $(this).bind("change", {}, DropdownFilter);
                //Each function end here
            });
            var objAutoCtrl = $("[autocontrol=1]")
            $(objAutoCtrl).each(function () {
                getAutoComplete($(this));
            });
        }
        function getAutoComplete(obj) {
            //alert((obj).attr('id'));
            var id = (obj).attr('id');
            var tid = id.replace('fld', '');

            //alert(tid);
            var DocumentType = getParameterByName('SC');
            $("#" + id).kendoAutoComplete({
                dataTextField: "Text",
                dataValueField: "tid",
                filter: "contains",
                minLength: 3,
                select: function (e) {
                    var autoFiltervalue = (obj).attr('autofilter');
                    //alert(autoFiltervalue);
                    var dataItem = this.dataItem(e.item.index());
                    if (autoFiltervalue == "1")
                        AutoFilter(dataItem.tid, id);
                    //alert(dataItem.Text);
                    $('#hdn' + tid).val(dataItem.Text);
                    //alert($('#hdn' + tid).val());
                },
                dataSource: {
                    serverFiltering: true,
                    transport: {
                        read: function (options) {
                            var text = (obj).val();
                            //alert(text);
                            var FilterDocID = obj.attr("filterDocID");
                            //alert(FilterDocID);
                            if (FilterDocID == "") {
                                var d = "{'str':'" + text + "','tid':'" + tid + "','DocType':'" + DocumentType + "'}";
                                var url1 = "NewMaster.aspx/GetAutoCompleteValue";
                            }
                            else {
                                var url1 = "NewMaster.aspx/FilterDataForAutoComplete";
                                var d = '{FieldId:"' + tid + '",DOCID:"' + FilterDocID + '",str:"' + text + '",DocType:"' + DocumentType + '"}';
                            }
                            //var FilterDocID = obj.attr("filterDocID")
                            //alert(FilterDocID);
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: url1,
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
        }
        function AutoFilter(docID, fieldID) {
            //$('#window1').prepend(loader);
            //$('#dvloader').css({ "z-index": "1500", "position": "absolute", "left": "37%" })
            //$('#dvloader').show();
            var Data = '{FieldId:"' + fieldID + '",DOCID:"' + docID + '"}';
            $.ajax({
                type: "POST",
                url: 'NewMaster.aspx/FilterDropDown',
                data: Data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (e) {
                    OpenPopLoader();
                },
                complete: function (e) {
                    ClosePopLoader();
                },
                success: function (response) {
                    var lookupval = "";
                    if (response.d.lookupvalue != "")
                        lookupval = $.parseJSON(response.d.lookupvalue);

                    var ddlData = response.d.ddldata;
                    var objCtrl = "";
                    $.each(lookupval, function (key, val) {
                        $.each(val, function (key, val) {
                            objCtrl = $("#fld" + key);
                            //If control exists then fill that contrlos with their lookup values
                            if (objCtrl.length > 0) {
                                objCtrl.val(val);
                            }
                        });
                        //loookup Each end below
                    });
                    //alert(ddlData.length);
                    if (ddlData.length > 0) {
                        if (ddlData[0].FieldType == "AUTOCOMPLETE") {
                            objCtrl = "#fld" + ddlData[0].FieldID;
                            //objCtrl = $("#fld" + ddlData[0].FieldID);
                            $(objCtrl).attr("filterDocID", DOCID);
                            //getAutoComplete($(objCtrl));
                        }
                        else {
                            //binding Drop Downs Data
                            $.each(ddlData, function (i, Curobj) {
                                var FieldID = Curobj.FieldID;
                                var options = Curobj.Data;
                                objCtrl = $("#fld" + FieldID);
                                if (objCtrl.length > 0) {
                                    objCtrl.html("");
                                    objCtrl.append($("<option>").val("0").html("--Select--"));
                                    var stroptions = "";
                                    $.each(options, function (i, cObj) {
                                        // stroptions += "<option value='" + cObji.tid + "'>" + cObji.Text + "</option>";
                                        objCtrl.append($("<option>").val(cObj.tid).html(cObj.Text));
                                    });
                                }
                                //loookup Each end below
                            });
                        }
                    }
                }
            });
        }

        function DropdownFilter() {
            var ID = "";
            var DOCID = 0;
            ID = $(this).attr("id");
            DOCID = $(this).val();
            //$('#window1').prepend(loader);
            //$('#dvloader').css({ "z-index": "1500", "position": "absolute", "left": "37%" })
            //$('#dvloader').show();
            var Data = '{FieldId:"' + ID + '",DOCID:"' + DOCID + '"}';
            $.ajax({
                type: "POST",
                url: 'NewMaster.aspx/FilterDropDown',
                data: Data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (e) {
                    OpenPopLoader();
                },
                complete: function (e) {
                    ClosePopLoader();
                },
                success: function (response) {
                    var lookupval = "";
                    if (response.d.lookupvalue != "")
                        lookupval = $.parseJSON(response.d.lookupvalue);

                    var ddlData = response.d.ddldata;
                    var objCtrl = "";
                    //$('#dvloader').hide();
                    //binding lookup Data

                    $.each(lookupval, function (key, val) {
                        $.each(val, function (key, val) {
                            objCtrl = $("#fld" + key);
                            //If control exists then fill that contrlos with their lookup values
                            if (objCtrl.length > 0) {
                                objCtrl.val(val);
                            }
                        });
                        //loookup Each end below
                    });
                    //binding Drop Downs Data
                    //if(ddlData)
                    //alert(ddlData.length);
                    if (ddlData.length > 0) {
                        if (ddlData[0].FieldType == "AUTOCOMPLETE") {
                            objCtrl = "#fld" + ddlData[0].FieldID;
                            //objCtrl = $("#fld" + ddlData[0].FieldID);
                            $(objCtrl).attr("filterDocID", DOCID);
                            //getAutoComplete($(objCtrl));
                        }
                        else {
                            //binding Drop Downs Data
                            $.each(ddlData, function (i, Curobj) {
                                var FieldID = Curobj.FieldID;
                                var options = Curobj.Data;
                                objCtrl = $("#fld" + FieldID);
                                if (objCtrl.length > 0) {
                                    objCtrl.html("");
                                    objCtrl.append($("<option>").val("0").html("--Select--"));
                                    var stroptions = "";
                                    $.each(options, function (i, cObj) {
                                        // stroptions += "<option value='" + cObji.tid + "'>" + cObji.Text + "</option>";
                                        objCtrl.append($("<option>").val(cObj.tid).html(cObj.Text));
                                    });
                                }
                                //loookup Each end below
                            });
                        }
                    }
                }
            });

        }
</script>

    <%--Kendo grid Logic and event--%>
    <script type="text/javascript">
        $(document).ready(function () {
            var DocumentType = getParameterByName('SC')
            $("#spnHeading").html(DocumentType);
            $("#spnHeading1").html(DocumentType);
            GetColumnGrid(DocumentType);
        });

        function GetColumnGrid(DocumentType) {
            $("#dvloader").show();
            $("#kgrdMaster").hide();
            var t = '{ documentType: "' + DocumentType + '" }';
            $.ajax({
                type: "POST",
                url: "NewMaster.aspx/GetColumn",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    if ($(kGrid).length > 0) {
                        $('#kgrdMaster').data().kendoGrid.destroy();
                        $('#kgrdMaster').empty();
                    }
                    var strData = response.d.Data;
                    var CommanObj = { 
                        command: [
                            { name: "Details", text: "", title: 'View Details', imageClass: "k-icon k-i-folder-add", click: DetailHandler },
                            { name: "Edit", text: "", title: 'Edit', imageClass: "k-icon k-edit", click: EditHandler },
                            { name: "LockUnlock", text: "", title: 'LockUnlock', imageClass: "k-icon k-i-lock", click: LockHandler },
                        ],
                        title: "Action", width: 120
                    };

                    //var CommanObj = {
                    //    command: [
                    //        { name: "Details", text: "", title: 'View Details', template: "<a href=\"javascript:DetailHandler('#:DocDetID#');\"><span class=\"k-icon k-delete\"></span></a>" },
                    //        { name: "Edit", text: "", title: 'Edit', template: "<span class='k-icon k-edit'></span>", click: EditHandler },
                    //        { name: "LockUnlock", text: "", title: 'LockUnlock', template: "<span class='k-icon k-i-lock'></span>", click: LockHandler },
                    //    ],
                    //    title: "Action", width: 60
                    //};
                    var Columns = response.d.Column;
                    //Columns.push(CommanObj);
                    Columns.splice(0, 0, CommanObj);
                    if (Columns.length > 0) {
                        BindMasterData(DocumentType, Columns);
                        if (KgridFilter != null) {
                            $("#kgrdMaster").data("kendoGrid").refresh();
                            $("#kgrdMaster").data("kendoGrid").dataSource.filter(KgridFilter);
                        }
                        $("#kgrdMaster").show();
                    }
                    else {
                        $("#kgrdMaster").hide();
                        $("#NoRecord1").show();

                    }

                    $("#dvloader,#dvloader1,#dvloader2").hide();
                    $("#kgrdMaster").show();
                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }

        var kGrid;
        function BindMasterData(documentType, Columns) {
            var t = '{ DocumentType: "' + documentType + '" }';
            kGrid = $("#kgrdMaster").kendoGrid({
                toolbar: ['excel'],
                excel: {
                    fileName: "RptMaster.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: Columns,
                dataSource: {
                    toolbar: ['excel'],
                    excel: {
                        fileName: "RptMaster.xlsx",
                        filterable: true,
                        pageable: true,
                        allPages: true
                    },
                    type: "json",
                    transport: {
                        read: {
                            url: 'NewMaster.aspx/GetMasterData',
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            beforeSend: function (e)
                            {
                                //$("#popLoader").data("kendoWindow").close();
                            }
                        },
                        parameterMap: function (data, type) {
                            return JSON.stringify({
                                documentType: documentType,
                                page: data.page,
                                pageSize: data.pageSize,
                                skip: data.skip,
                                take: data.take,
                                sorting: data.sort === undefined ? null : data.sort,
                                filter: data.filter === undefined ? null : data.filter
                            });
                        },

                    },
                    schema: {
                        data: function (data) {
                            return $.parseJSON(data.d.Data) || [];
                        },
                        total: function (data) {
                            //$("#spnNeedToAct").html("(" + data.d.total + ")");
                            //if (data.d.length > 0) {
                            return data.d.total || [];
                            //}
                        }
                    },
                    pageSize: 12,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                },
                columns: Columns,
                sortable: true,
                resizable: true,
                columnResize: function(e) {
                    $("#grid colgroup col").each(function () {
                        if ($(this).width() < 50) {
                            $(this).css("width", "50px");
                        }
                    })
                },
                reorderable: true,
                height: 500,
                width: 1386,
                noRecords: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                scrollable: true,
                filterable: true,
                groupable: true,
                //filterable: {
                //    mode: "row",
                //},
                sortable: {
                    mode: "multiple"
                },
                pageable: {
                    pageSizes: true,
                    refresh: true
                },
            });
            $(".k-grid-Details").attr('title', "Show Details");
            $(".k-button").click(function () {
                $(".k-grid-Details").attr('title', "Show Details");
            });
            $(".k-grid-toolbar").append("<span style='font-size: 18px;'>" + documentType + "</span>");
            $(".k-grid-toolbar").append("<input type='button' id='btnAdd' class='k-button' value='Add' style='float: right;' onclick='javascript: return ADDMaster(); return false;'/>");
        }
        
        var DetailHandler = function DetailHandler(e) {
            //dataItem = $(e.currentTarget).closest("tr");
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            OpenWindow('DocDetailMaster.aspx?DOCID=' + dataItem.DocDetID + '');
            //OpenWindow('DocDetail.aspx?DOCID="dataItem.tid"')
        }

        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }

        var EditHandler = function EditHandler(e) {
            e.preventDefault();
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var tid = dataItem.DocDetID;
            //alert(tid);
            var documentType = getParameterByName('SC');
            var Data = '{DocType:"' + documentType + '",EID:"",Tid:"' + tid + '"}';
            //alert(Data);
            $.ajax({
                type: "POST",
                url: 'NewMaster.aspx/CreatedDynamicFields',
                data: Data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (e) {
                    OpenPopLoader();
                },
                complete: function (e) {
                    ClosePopLoader();
                },
                success: function (response) {
                    $("#hdnMethod").val(tid);
                    $("#ContentPlaceHolder1_dvControls").html("");
                    $("#ContentPlaceHolder1_dvControls").html(response.d);
                    //$("#ContentPlaceHolder1_dvControls").append("<div><input type=\"button\" id=\"btnSave\" class='MainHomeLink' value=\"UPDATE\" onclick='javascript:validate();' /></div>");
                    $("input[datatype='Datetime']").datepicker({
                        showOn: "button",
                        buttonImage: "images/cal.png",
                        buttonImageOnly: true,
                        buttonText: "Select date",
                        dateFormat: 'dd/mm/y',
                        setDate: new Date(),
                        showOn: "both"
                    });
                    openkendowindow(documentType);
                }
            });
            
        }
        var LockHandler = function LockHandler(e) {
            var DocumentType = getParameterByName('SC')
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var grid = $("#kgrdMaster").data("kendoGrid");
            var columns = grid.options.columns;
            var tid = dataItem.DocDetID;
            var col = columns[3].field;
            var status = dataItem.Status;
            if (status == 'ACTIVE') {
                $("#lblLock").html("<b>Please click the option hereunder to confirm if you wish to lock the " + DocumentType + " record - " + dataItem[col] + "</b>");
                $("#btnLockupdate").val("Lock");
            }
            else {
                $("#lblLock").html("<b>Please click the option hereunder to confirm if you wish to Unlock the " + DocumentType + " record - " + dataItem[col] + "</b>");
                $("#btnLockupdate").val("Unlock");
            }
            
            var window = $("#dvLockUnloack");
            window.kendoWindow({
                title: "Lock/Unloack",
                modal: true,
                width: 500,
                resizable: true,
                scrollable: false,
                open: function () {
                    $('#dvLockUnloack').show();
                }
            });
            //window.data("kendoWindow").setOptions({ width: windowWidth });
            window.data("kendoWindow").center().open();

            $("#btnLockupdate").unbind('click');
            $("#btnLockupdate").bind('click', function () {
                var t = '{pid:"' + tid + '", doctype: "' + DocumentType + '" }';
                $.ajax({
                    type: "POST",
                    url: "NewMaster.aspx/LockRecord",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    beforeSend: function (e) {
                        OpenPopLoader();
                    },
                    complete: function (e) {
                        ClosePopLoader();
                    },
                    success: function (response) {
                        var result = response.d;
                        if (result == "0" || result == "1") {
                            alert("Updated  successfully");
                            $("#dvLockUnloack").data("kendoWindow").close();
                            GetColumnGrid(DocumentType);
                        }
                        else {
                            alert("Sorry! Error occured in server. Please try again.");
                        }
                    },
                    error: function (data) {
                        //Code For hiding preloader
                    }
                });
            });
        }
        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }
    </script>
    <script>
        $("form").submit(function (e) {
            e.preventDefault();
        });
    </script>
</asp:Content>