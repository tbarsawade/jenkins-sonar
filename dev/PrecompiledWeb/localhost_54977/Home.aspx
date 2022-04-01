<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="Home, App_Web_iv00bt22" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link rel="stylesheet" href="kendu/homekendo.common.min.css" />
    <link rel="stylesheet" href="kendu/homekendo.rtl.min.css" />
    <%--<link rel="stylesheet" href="kendu/homekendo.silver.min.css" />--%>
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="kendu/homekendo.mobile.all.min.css" />

    <%--kendo.data.min.js--%>
    <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <%--<script src="kendu/homejquery-1.9.1.min.js"></script>--%>
    <script src="kendu/homekendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <%--<script src="kendu/content/shared/js/console.js"></script>
    <%--<script src="kendu/js/jszip.min.js"></script>--%>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>
    <link href="css/style.css" rel="stylesheet" />
    <script type="text/javascript">
        //Dated 18-November-2015 By Ajeet
        //used for refressing only needtoact grid on child close
        var new_window;
        var filtersNeedToAct, filterMyReq, filterHistory, filterDraft, url; //= $("#kgrd").data("kendoGrid").dataSource.filter();
        function OpenWindow(url) {
            msieversion(url);
            return false;
        }
        function msieversion(url) {
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
            {
                new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
                //alert(parseInt(ua.substring(msie + 5, ua.indexOf(".", msie))));
                //var a = document.createElement("a");
                //if (a.click) {
                //    //alert(navigator.userAgent);

                //    a.setAttribute("href", new_window);
                //    a.style.display = "none";
                //    document.body.appendChild(a);
                //    a.click();
                //}
            }
            else  // If another browser, return 0
            {
                new_window = window.open(url, "_blank", "", true);
            }
            return false;
        }
        //var win = window.open("Child.aspx", "thePopUp", "");
        function childClose() {
            //if (new_window.closed) {
            if ($(kGrid).length > 0)
                filtersNeedToAct = $("#kgrd").data("kendoGrid").dataSource.filter();
            if ($(kgrdMyReq).length > 0)
                filterMyReq = $("#kgrdMyReq").data("kendoGrid").dataSource.filter();
            if ($(kgrdHistory).length > 0)
                filterHistory = $("#kgrdHistory").data("kendoGrid").dataSource.filter();
            if ($(kgrdDraft).length > 0)
                filterDraft = $("#kgrdDraft").data("kendoGrid").dataSource.filter();

            var DocumentType = $("#ContentPlaceHolder1_ddldocType").val();
            GetColumnGrid(DocumentType);
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
    </script>
    <style type="text/css">
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
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
            /*z-index:999;  -webkit-z-index:999;*/
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
                font-weight: bold;
                font-size: 12px;
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



        .k-window div.k-window-content {
            overflow: hidden;
        }

        .k-grid-toolbar a {
            float: right;
        }
    </style>
    <script type="text/javascript">
        function myFun() {
            var url = window.location.href;
            var data = [];
            data = url.split("/");
            window.location = window.location.href.replace(data[data.length - 1], 'Thome.aspx');
            return false;
        }
      
    </script>
    <div class="container-fluid container-fluid-amend">
        <table  cellspacing="0px" cellpadding="1px" class="HomeContent">
            <tr>
               <%-- '<td style="width: 100%; display: block; float: left; padding-bottom: 2px;">--%>
                <td style="padding-bottom: 2px;">
                    <table style="width: 100%;">
                        <tr>

                            <td style="padding: 0px 12px 0px;">
                                <label for="lblDocType" style="font-size: 12px !important;">Document Type :</label>
                                <asp:DropDownList ID="ddldocType" runat="server" Style="padding: 3px; font-size: 11px; line-height: 16px;">
                                    <asp:ListItem Text="Act Document">Act Document</asp:ListItem>
                                </asp:DropDownList>
                                <%--@Html.DropDownList("ddldocType")
                                @Html.Hidden("hdnddldocType")--%>
                            </td>

                            <td class="right-button">
                                <div id="dvbtnloader" style="display: block; text-align: center; height: 35px; padding-top: 10px;">
                                    <input type="image" src="../images/loading12.gif" />
                                </div>
                                <div id="dvWidgetsBtn" style="margin: 0px 0px -4px;">
                                    <input type='button' class='MainHomeLink' id='btnTicket' runat="server" visible="true" onclick='myFun(); return false;' />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div class="container-fluid container-fluid-amend">    
    <div style="display: block;" class="HomeContent ">
        <div class="demo-section k-header">
            <div id="tabstrip">
                <ul>
                    <li class="k-state-active" style="margin-right: 5px;">
                        <b>Need To Act <span id="spnNeedToAct">(0)</span></b>
                    </li>
                    <li style="margin-right: 5px;">
                        <b>My Request <span id="spnMyReq">(0)</span></b>
                    </li>
                    <li>
                        <b>History <span id="spnHistory">(0)</span></b>
                    </li>
                    <li>
                        <b>Draft <span id="spnDraft">(0)</span></b>
                    </li>
                </ul>
                <div style="padding: 0px;">
                    <div>
                        <div id="dvneedToAct" style="width: 100%; overflow-x: auto; margin: 0px; padding: 0px;">
                            <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kgrd" style="display: block; overflow-x: auto;"></div>
                            <div id="NoRecord1" style="display: none; text-align: center; min-height: 80px;">
                                <span style="color: red; position: relative; top: 30px;">No record found</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="padding: 0px!important;">
                    <div>
                        <div>
                            <div id="dvloader1" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kgrdMyReq" style="display: block; overflow-x: auto;"></div>
                            <div id="NoRecord2" style="display: none; text-align: center; min-height: 80px;">
                                <span style="color: red; position: relative; top: 30px;">No record found</span>
                            </div>
                        </div>
                    </div>

                </div>
                <div style="padding: 0px!important;">
                    <div>
                        <div id="example">
                            <div id="dvloader2" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kgrdHistory" style="display: block; overflow-x: auto;"></div>
                            <div id="NoRecord3" style="display: none; text-align: center; min-height: 80px;">
                                <span style="color: red; position: relative; top: 30px;">No record found</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="padding: 0px!important;">
                    <div>
                        <div id="dvDraft">
                            <div id="dvloader3" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                            </div>
                            <div id="kgrdDraft" style="display: block; overflow-x: auto;"></div>
                            <div id="NoRecord4" style="display: none; text-align: center; min-height: 80px;">
                                <span style="color: red; position: relative; top: 30px;">No record found</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
        </div>
    <div class="container-fluid container-fluid-amend">    </div>
    <div class="mar_botm" id="wdgtwrap" style="display: none;">
        <div id="dv1" class="box divCustom" style="display: none;"></div>
        <div id="dv2" class="box divCustom" style="display: none;"></div>
        <div id="dv3" class="box divCustom" style="display: none;"></div>
        <div id="dv4" class="box divCustom" style="display: none;"></div>
        <div id="dv5" class="box divCustom" style="display: none;"></div>
        <div id="dv6" class="box divCustom" style="display: none;"></div>
        <div id="dv7" class="box divCustom" style="display: none;"></div>
        <div id="dv8" class="box divCustom" style="display: none;"></div>
        <div id="dv9" class="box divCustom" style="display: none;"></div>
        <input type="hidden" id="hdnwidgetTID" />
    </div>
    <div id="dvPopuploader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
        <input type="image" src="../images/preloader22.gif" />
    </div>
    <div class="mar_botm" id="wdgtwrap2" style="display: none;">
        <div id="KgridFirstLevelWdgt"></div>
    </div>

    <div class="mar_botm" id="wdgtwrap3" style="display: none;">
        <div id="KgridSecondLevelWdgt"></div>
    </div>

    <%-- new added by sunil oct-18--%>

     <asp:Button ID="btnim" runat="server" Style="display: none;" />
    <asp:ModalPopupExtender ID="modalpopuppassexp" runat="server" PopupControlID="PnlPassExp" TargetControlID="btnim"
        CancelControlID="btnNo" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>

    <asp:UpdatePanel ID="UpdatePanelPassexp" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="PnlPassExp" runat="server" CssClass="modalPopup">
                <div class="header">
                    <asp:Label runat="server" ID="lblPassexpmsg"></asp:Label>
                </div>
                <div class="body">
                    We recommend you to change your password
                </div>
                <div class="footer" align="right">
                    <asp:Button ID="btnYes" runat="server" Text="Change Password" CssClass="btnNew"  Width="140px" />&nbsp;
        <asp:Button ID="btnNo" runat="server" Text="Skip" CssClass="btnNew"  Width="60px" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- new added by sunil oct-18--%>
    <asp:Button ID="btnAlert" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_Alert" runat="server" PopupControlID="pnlAlert" TargetControlID="btnAlert" BackgroundCssClass="modalBackground" CancelControlID="btnAlertClose">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlAlert" runat="server" Width="850px" Height="600px"  BackColor="White" ScrollBars="Auto" Style="display: none">
        <div class="box">
            <table cellspacing="20px"  cellpadding="20px" width="100%">
                <tr>
                  <td style="width:5px;">
                      </td> 
                <td style="width: 840px;">
                          <asp:UpdatePanel ID="updAlert" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                  

                 <table cellspacing="0px"  cellpadding="0px" width="98%">
                <tr>
                    <td>
                        <h3>Merger Alert Message </h3>
                    </td>
                    <td align="right">
                        <asp:ImageButton ID="btnAlertClose" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                
                <tr>
                      <td style="text-align:left">
                                <asp:Label ID="lblAlertMes" runat="server" textalign="left" ></asp:Label>&nbsp;
                                <%--<asp:Label ID="Label1" runat="server" textalign="left" Font-Bold="true" Font-Size="Medium" ></asp:Label>&nbsp;--%>
                         
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnCloseAlertMess" runat="server" Text="OK" OnClick="HideAlert" CssClass="MainHomeLink" Width="80px" />
                    </td>
                </tr>
            </table>
                       </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
       
        </div>
    </asp:Panel>

        <%-- new added by Pinki|19 Jun 2020|for Paytm Client--%>
    <asp:Button ID="btnTCAlert" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="TC_Alert" runat="server" PopupControlID="pnlTCAlert" TargetControlID="btnTCAlert" BackgroundCssClass="modalBackground" >
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlTCAlert" runat="server" Width="850px" Height="600px"  BackColor="White" ScrollBars="Auto" Style="display: none">
        <div class="box">
            <table cellspacing="20px"  cellpadding="20px" width="100%">
                <tr>
                  <td style="width:5px;">
                      </td> 
                <td style="width: 840px;">
                          <asp:UpdatePanel ID="updTCAlert" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                  

                 <table cellspacing="0px"  cellpadding="0px" width="98%">
                <tr>
                    <td>
                        <h3>Terms and Conditions</h3>
                    </td>
                </tr>
                
                <tr>
                      <td style="text-align:left">
                                <asp:Label ID="lblTCAlertMes" runat="server" textalign="left" ></asp:Label>&nbsp;
                                <%--<asp:Label ID="Label1" runat="server" textalign="left" Font-Bold="true" Font-Size="Medium" ></asp:Label>&nbsp;--%>
                         
                    </td>
                </tr>
             
                <tr>
                    <td><asp:CheckBox ID="chkTermAndCondition" Text="I Agree to the Terrms & Conditions" runat="server" AutoPostBack="true"  OnCheckedChanged="chkTCChanged" />
                           &nbsp;&nbsp;&nbsp;&nbsp;
                             <asp:Button ID="btnCloseTCAlertMess" runat="server" Text="Accept" OnClick="TCHideAlert" CssClass="MainHomeLink" Width="80px" />
                       </td>
                    <%--<td align="right">
                    
                    </td>--%>
                </tr>
                <tr>
                    <td align="left"><asp:Label ID="lblWarningMsg" runat="server" style="color:red" ></asp:Label></td>
                </tr>
            </table>
                       </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
       
        </div>
    </asp:Panel>
    <script type="text/javascript">

        //   $("#kgrd").data("kendoGrid").refresh();

        $(document).ready(function () {
            //ldrdv example
            $("#tabstrip").kendoTabStrip();
            $("#btnSearch").kendoButton();
            $("#btnSearchMyReq").kendoButton();
            $("#btnSearchHistory").kendoButton();
            $("#ContentPlaceHolder1_ddldocType").kendoDropDownList();
            var DocumentType = $("#ContentPlaceHolder1_ddldocType").val();
            GetColumnGrid(DocumentType);
            GetDraftColumnGrid(DocumentType);
        });

        $("#ContentPlaceHolder1_ddldocType").change(function () {
            var DocumentType = $("#ContentPlaceHolder1_ddldocType").val();
            GetColumnGrid(DocumentType);
            GetDraftColumnGrid(DocumentType);
        });
        function GetColumnGrid(DocumentType) {
            $("#dvloader,#dvloader1,#dvloader2").show();
            $("#kgrd,#kgrdMyReq,#kgrdHistory").hide();
            var t = '{ documentType: "' + DocumentType + '" ,type:"WITHOUTDRAFT"}';
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetColumn",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    if ($(kGrid).length > 0) {
                        $('#kgrd').data().kendoGrid.destroy();
                        $('#kgrd').empty();
                    }
                    if ($(kgrdMyReq).length > 0) {
                      $('#kgrdMyReq').data().kendoGrid.destroy();
                        $('#kgrdMyReq').empty();
                    }
                    if ($(kgrdHistory).length > 0) {
                        $('#kgrdHistory').data().kendoGrid.destroy();
                        $('#kgrdHistory').empty();
                    }
                    var strData = response.d.Data;
                    var CommanObj = {
                        command: [{ name: "Details", text: "View Details", title: 'View Details', click: DetailHandler }], title: "Action", width: 110
                    }
                    var Columns = response.d.Column;
                    Columns.splice(0, 0, CommanObj);
                    if (Columns.length > 0) {
                        BindNeedToAct(DocumentType, Columns);
                        GetGridMyReq(DocumentType, Columns);
                        GetGridHistory(DocumentType, Columns);
                        $(".k-grid-Details").attr('title', "Show Details");
                        if (filtersNeedToAct != null) {
                            $("#kgrd").data("kendoGrid").refresh();
                            $("#kgrd").data("kendoGrid").dataSource.filter(filtersNeedToAct);
                        }
                        if (filterMyReq != null) {
                            $("#kgrdMyReq").data("kendoGrid").refresh();
                            $("#kgrdMyReq").data("kendoGrid").dataSource.filter(filterMyReq);
                            //alert('Hi');
                        }
                        if (filterHistory != null) {
                            $("#kgrdHistory").data("kendoGrid").refresh();
                            $("#kgrdHistory").data("kendoGrid").dataSource.filter(filterHistory);
                            //alert('Hi');
                        }
                        if (filterDraft != null) {
                            $("#kgrdDraft").data("kendoGrid").refresh();
                            $("#kgrdDraft").data("kendoGrid").dataSource.filter(filterDraft);
                        }
                        $("#dvloader,#dvloader1,#dvloader2,#dvloader3").hide();
                        $("#kgrd,#kgrdMyReq,#kgrdHistory,#kgrdDraft").show();
                    }
                    else {
                        $("#kgrd").hide();
                        $("#NoRecord1").show();
                    }
                    $("#dvloader,#dvloader1,#dvloader2,#dvloader3").hide();
                    $("#kgrd,#kgrdMyReq,#kgrdHistory,#kgrdDraft").show();
                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }

        function GetDraftColumnGrid(DocumentType) {
            $("#dvloader3").show();
            $("#kgrdDraft").hide();
            var t = '{ documentType: "' + DocumentType + '" ,type:"DRAFT"}';
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetColumn",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    if ($(kgrdDraft).length > 0) {
                        $('#kgrdDraft').data().kendoGrid.destroy();
                        $('#kgrdDraft').empty();
                    }
                    var strData = response.d.Data;
                    var CommanObj = {
                        command: [
                                  //{ name: "Print", text: "Print", title: 'Print Draft', click: PrintHandler },
                                  { name: "Edit", text: "Edit", title: 'Edit Draft', click: DraftEditHandler },
                                  { name: "Delete", text: "Delete", title: 'Delete Draft', click: DraftDeleteHandler }
                        ], title: "Action", width: 150
                    }
                    //var DraftCommanObj = {
                    //    command: [{ name: "Print", text: "Print", title: 'Print Draft', click: PrintHandler },
                    //              { name: "Edit", text: "Edit", title: 'Edit Draft', click: PrintHandler },
                    //              { name: "Delete", text: "Delete", title: 'Delete Draft', click: PrintHandler }
                    //    ], title: "Action", width: 100
                    //}
                    //var DraftColumns = response.d.Column;
                    //DraftColumns.splice(0, 0, DraftCommanObj);

                    var Columns = response.d.Column;
                    Columns.splice(0, 0, CommanObj);
                    if (Columns.length > 0) {
                        GetGridDraft(DocumentType, Columns);
                        $(".k-grid-Details").attr('title', "Show Details");
                        if (filterDraft != null) {
                            $("#kgrdDraft").data("kendoGrid").refresh();
                            $("#kgrdDraft").data("kendoGrid").dataSource.filter(filterDraft);
                        }
                        $("#dvloader3").hide();
                        $("#kgrdDraft").show();
                    }
                    else {
                        $("#kgrdDraft").hide();
                        $("#NoRecord4").show();
                    }
                    $("#dvloader3").hide();
                    $("#kgrdDraft").show();
                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }

        var kGrid = "";
        function BindNeedToAct(documentType, Columns) {
            var t = '{ DocumentType: "' + documentType + '" }';
            kGrid = $("#kgrd").kendoGrid({
                toolbar: ['excel'],
                excel: {
                    fileName: "RptNeedToAct.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: Columns,
                dataSource: {
                    toolbar: ['excel'],
                    excel: {
                        fileName: "RptNeedToAct.xlsx",
                        filterable: true,
                        pageable: true,
                        allPages: true
                    },
                    type: "json",
                    transport: {
                        read: {
                            url: 'Home.aspx/GetNeedToAct',
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8"
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
                            $("#spnNeedToAct").html("(" + data.d.total + ")");
                            //if (data.d.length > 0) {
                            return data.d.total || [];
                            //}
                        }
                    },
                    pageSize: 20,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                },
                columns: Columns,
                sortable: true,
                resizable: true,
                height: 420,
                width: 1386,
                noRecords: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                scrollable: true,

                filterable: {
                    mode: "row",
                    //extra: false,
                    operators: {
                        string: {
                            //enabled: true,
                            contains: "Contains",
                        },
                    }

                },
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
        }
        var kgrdMyReq = "";
        function GetGridMyReq(documentType, Columns) {
            //$("#dvloader").show();
            //$("#kgrd").hide();
            var t = '{ DocumentType: "' + documentType + '" }';
            kgrdMyReq = $("#kgrdMyReq").kendoGrid({
                toolbar: ['excel'],
                excel: {
                    fileName: "RptMyReq.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: Columns,
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: 'Home.aspx/GetDataMyRequestGrid',
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8"
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
                            //}
                        },
                        total: function (data) {
                            $("#spnMyReq").html("(" + data.d.total + ")");
                            //if (data.d.length > 0) {
                            return data.d.total || [];
                            //}
                        }
                    },
                    columns: Columns,
                    pageSize: 20,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,

                },
                columns: Columns,
                //reorderable: true,
                sortable: true,
                resizable: true,
                height: 420,
                width: 1386,
                noRecords: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                scrollable: true,
                filterable: {
                    mode: "row",
                    operators: {
                        string: {
                            //enabled: true,
                            contains: "Contains",
                        },
                    }
                },
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
        }
        var kgrdHistory = "";
        function GetGridHistory(documentType, Columns) {
            $(".k-loading").hide();
            //$("#dvloader").show();
            //$("#kgrd").hide();
            var t = '{ DocumentType: "' + documentType + '" }';
            kgrdHistory = $("#kgrdHistory").kendoGrid({
                toolbar: ['excel'],
                excel: {
                    fileName: "RptHistory.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: Columns,
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: 'Home.aspx/GetDataHistory',
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8"
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
                            $("#spnHistory").html("(" + data.d.total + ")");
                            return data.d.total || [];
                        }
                    },
                    columns: Columns,
                    pageSize: 20,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,

                },
                columns: Columns,
                //reorderable: true,
                sortable: true,
                resizable: true,
                height: 420,
                width: 1386,
                noRecords: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                scrollable: true,
                filterable: {
                    mode: "row",
                    operators: {
                        string: {
                            //enabled: true,
                            contains: "Contains",
                        },
                    }
                },
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
        }
        var kgrdDraft = "";
        function GetGridDraft(documentType, Columns) {
            $(".k-loading").hide();
            var t = '{ DocumentType: "' + documentType + '" }';
            kgrdDraft = $("#kgrdDraft").kendoGrid({
                toolbar: ['excel'],
                excel: {
                    fileName: "RptDraft.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: Columns,
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: 'Home.aspx/GetDraftAct',
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8"
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
                            $("#spnDraft").html("(" + data.d.total + ")");
                            return data.d.total || [];
                        }
                    },
                    columns: Columns,
                    pageSize: 20,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,

                },
                columns: Columns,
                //reorderable: true,
                sortable: true,
                resizable: true,
                height: 420,
                width: 1386,
                noRecords: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                scrollable: true,
                filterable: {
                    mode: "row",
                    operators: {
                        string: {
                            //enabled: true,
                            contains: "Contains",
                        },
                    }
                },

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

        }
        var DetailHandler = function DetailHandler(e) {
            var EID = '<%= Session("EID")%>';
           
            var Docversion = '<%=Session("Docversion")%>';
            debugger
            try { debugger
                if (Docversion == "New") {
                    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                    if ($.isNumeric(dataItem.DocDetID)) {
                        OpenWindow('NewDocDetail.aspx?DOCID=' + dataItem.DocDetID + '');
                    }
                }
                else {
                    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                    if ($.isNumeric(dataItem.DocDetID)) {
                        OpenWindow('DocDetail.aspx?DOCID=' + dataItem.DocDetID + '');
                    }
                }
            }
            catch (ex) {
                var tr = $(e.currentTarget).closest("tr");
                var docid = $(e.currentTarget).closest("tr")[0].cells[1].innerText;
                if ($.isNumeric(docid)) {
                    if (Docversion == "New") {
                        OpenWindow('NewDocDetail.aspx?DOCID=' + docid + '');
                    }
                    else {
                        OpenWindow('DocDetail.aspx?DOCID=' + docid + '');
                    }                   
                }
            }
            //OpenWindow('NewDocDetail.aspx?DOCID="dataItem.tid"')
        }
        var PrintHandler = function PrintHandler(e) {
            try {
                dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                $.ajax({
                    type: "POST",
                    url: "Home.aspx/DraftPrint",
                    data: '{docid: "' + dataItem.DocDetID + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert(response.d);
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
            catch (ex) {
                var tr = $(e.currentTarget).closest("tr");
                var docid = $(e.currentTarget).closest("tr")[0].cells[1].innerText;
                if (Docversion == "New") {
                OpenWindow('NewDocDetail.aspx?DOCID=' + docid + '');

                }
                else {
                OpenWindow('DocDetail.aspx?DOCID=' + docid + '');

                }

            }
            //OpenWindow('NewDocDetail.aspx?DOCID="dataItem.tid"')
        }
        var DraftEditHandler = function DraftEditHandler(e) {
            try {
                dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                $.ajax({
                    type: "POST",
                    url: "Home.aspx/DraftEdit",
                    data: '{docid: "' + dataItem.DocDetID + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        window.location.href = response.d;
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
            catch (ex) {
                var tr = $(e.currentTarget).closest("tr");
                var docid = $(e.currentTarget).closest("tr")[0].cells[1].innerText;
                if (Docversion == "New") {
                    OpenWindow('NewDocDetail.aspx?DOCID=' + docid + '');
                }
                else {
                    OpenWindow('DocDetail.aspx?DOCID=' + docid + '');

                }
            }
            //OpenWindow('NewDocDetail.aspx?DOCID="dataItem.tid"')
        }
        var DraftDeleteHandler = function DraftDeleteHandler(e) {
            try {
                dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                var result = confirm("Are you sure to delete this DraftID:  " + dataItem.DocDetID);
                if (result == true) {
                    $.ajax({
                        type: "POST",
                        url: "Home.aspx/DraftDelete",
                        data: '{docid: "' + dataItem.DocDetID + '" }',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d == "SUCCESS") {
                                alert('Deleted SuccessFully!!!!!!!!');
                                GetDraftColumnGrid($("#ContentPlaceHolder1_ddldocType").val());
                            };
                        },
                        failure: function (response) {
                            alert(response.d);
                        }
                    });
                }
            }
            catch (ex) {
                var tr = $(e.currentTarget).closest("tr");
                var docid = $(e.currentTarget).closest("tr")[0].cells[1].innerText;
                alert(docid);
                if (Docversion == "New") {
                    OpenWindow('NewDocDetail.aspx?DOCID=' + docid + '');
                }
                else {
                    OpenWindow('DocDetail.aspx?DOCID=' + docid + '');

                }

            }
        }
        function OpenWindow(url) {
            msieversion(url);
            return false;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetWidgets",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    if (res != "") {
                        var data = $.parseJSON(res);
                        var dbtype = ""
                        var WNature = "";
                        $.each(data, function (i) {
                            dbtype = this.Widgettype;
                            WNature = this.WidgetNature;
                            $('#dvWidgetsBtn').append("<input type='button' class='MainHomeLink' id='btn" + (i + 1) + "'  title='" + this.Tooltip + "'  value='" + this.DBName + "' onclick='javascript:openWidgetPopup(" + this.Tid + ") '/>&nbsp;&nbsp;");
                            //$('#btnloader').removeClass('loader');
                            //$("#btn" + (i + 1)).kendoButton();
                            //Each function end here
                        });
                    }
                    else {
                        //$('#btnloader').removeClass('loader');
                    }
                    $('#dvbtnloader').hide();
                },
                error: function (data) {
                    //alert("Hi Error occured while calling!!!");
                }
                //Ajax call end here
            });
            //ready function end here
        });

        function openWidgetPopup(tid) {
            var t = '{tid: "' + tid + '" }';
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetWidgets1",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    if (res != "") {
                        var data = $.parseJSON(res);
                        var dbtype = ""
                        var WNature = "";
                        $.each(data, function (i) {
                            dbtype = this.Widgettype;
                            WNature = this.WidgetNature;
                            $("#dv" + (i + 1)).html("<div class='loader'></div>").removeClass("divCustom").addClass("divCustom1");
                            //$("#wdgtwrap").show();
                            switch (dbtype) {

                                //<div class="loader1"></div>
                                case "Usefull Links":
                                    {
                                        bindUseFulLink(this.DBName, "dv" + (i + 1));
                                    }
                                    break;
                                case "New":
                                    {
                                        switch (WNature) {
                                            case "Grid":
                                                {
                                                    bindCustomWidget(this.Tid, "dv" + (i + 1), this.DBName, (i + 1));
                                                }
                                                break;
                                            case "Chart":
                                                {
                                                    GetPiChartWidget(this.Tid, "dv" + (i + 1), this.DBName, (i + 1));
                                                    //alert("Hi I am going to bind chart now.");
                                                }
                                                break;
                                        }
                                    }
                            }
                            //Each function end here
                        });
                    }
                },
                error: function (data) {
                    //alert("Hi Error occured while calling!!!");
                }
                //Ajax call end here
            });
        }

        function bindCustomWidget(Obj, Divid, DbName, index) {
            var t = '{tid: "' + Obj + '" }';
            $("#hdnwidgetTID").val(Obj);
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetCustomWidget",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response;
                    $("#" + Divid).html("").show();
                    //$("#" + Divid).append("<h3>" + DbName + "</h3>");
                    $("#" + Divid).append("<div id='kgrd" + index + "'  style='overflow-x:auto;'></div>");
                    var result = res.d;
                    var data1 = "";
                    if (result.Data != "")
                        data1 = $.parseJSON(result.Data);
                    var Column = result.Column;
                    var windowWidth = result.WindowWidth;
                    if (result.ShowAction == "yes") {
                        var CommanObj = { command: [{ name: "Details", Title: "Details", text: "Details", click: WidgetDetailHandler }], title: "Action", width: 60 }
                        Column.push(CommanObj);
                    }
                    if (data1 != "") {                        //alert(data1.length);
                        var datalen = data1.length;
                        kgridHeight = 600;
                        if (datalen < 20) {
                            if (datalen < 5)
                                kgridHeight = datalen * 50;
                            else
                                kgridHeight = datalen * 30;
                        }
                        bindGrid1(data1, Column, "kgrd" + index, DbName, kgridHeight);
                    }
                    else {
                        $("#kgrd" + index).append("<div id='norecord" + index + "' style='text-align:center; color:red; height: 88px;'><span style='color:red;  position: relative; top: 30px;'> No record found</span></div>")
                    }
                    //$("#wdgtwrap").data("kendoWindow").destroy();
                    var window = $("#wdgtwrap");
                    window.kendoWindow({
                        title: DbName,
                        modal: true,
                        width: windowWidth,
                        resizable: true,
                        scrollable: false,
                        open: function () {
                            $('#wdgtwrap').show();
                        }
                    });
                    window.data("kendoWindow").setOptions({ width: windowWidth });
                    window.data("kendoWindow").center().open();
                    $("#wdgtwrap_wnd_title").html(DbName);
                    event.preventDefault();
                },
                error: function (data) {
                    alert("Hi error has occured");
                    //Will write code later
                }
                //Ajax call end here
            });

        }
        var kgrdbind;
        function bindGrid1(Data1, Column, grid, title, kgriHeight) {

            $("#" + grid).kendoGrid({
                dataSource: {
                    data: Data1
                },
                scrollable: true,
                resizable: true,
                reorderable: true,
                sortable: true,
                filterable: true,
                height: kgriHeight,
                columns: Column
            });
            //resizeGrid();
        }

        //1st level Wedget details handler===========================================

        var WidgetDetailHandler = function WidgetDetailHandler(e) {
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            //var gridDataArray = $('#kgrd1').data('kendoGrid')._data;
            //var columnDataVector = [];
            //for (var index = 0; index < gridDataArray.length; index++) {
            //    columnDataVector.append(gridDataArray[index]);
            //};


            var grid = $("#kgrd1").data("kendoGrid");
            var columns = grid.options.columns;
            var columnRow = "";
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].title != "Action") {
                    var f = columns[i].field;
                    columnRow += columns[i].field + "::" + dataItem[f] + "|";
                }
            }
            columnRow += "key_Company::" + dataItem.Key_Company + "|key_Act::" + dataItem.Key_Act;
            var tid = $("#hdnwidgetTID").val();
            var t = '{data: "' + columnRow + '", tid: "' + tid + '" }';
            $.ajax({
                type: "POST",
                url: "/Home.aspx/GetFirstlevelWidget",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: t,
                success: function (response) {
                    var res = response;
                    //$("#" + Divid).html("").show();
                    //$("#" + Divid).append("<h3>" + DbName + "</h3>");
                    //$("#" + Divid).append("<div id='kgrd" + index + "'></div>");
                    var result = res.d;
                    var data1 = "";
                    if (result.Data != "")
                        data1 = $.parseJSON(result.Data);
                    var Column = result.Column;
                    var windowWidth = result.WindowWidth;
                    if (result.ShowAction == "yes") {
                        var CommanObj = { command: [{ name: "Details", Title: "Details", text: "Details", click: SeclevelWidgetDetailHandler }], title: "Action", width: 60 }
                        Column.push(CommanObj);
                    }
                    if (data1 != "") {
                        //bindGrid1(data1, Column, "kgrd" + index);
                        $("#KgridFirstLevelWdgt").kendoGrid({
                            dataSource: {
                                data: data1
                            },
                            scrollable: true,
                            resizable: true,
                            reorderable: true,
                            sortable: true,
                            filterable: true,
                            columns: Column
                        });
                    }

                    else {
                        $("#KgridFirstLevelWdgt" + index).append("<div id='norecord" + index + "' style='text-align:center; color:red; height: 88px;'><span style='color:red;  position: relative; top: 30px;'> No record found</span></div>")
                    }
                    var window = $("#wdgtwrap2");
                    window.kendoWindow({
                        title: 'Details',
                        modal: true,
                        width: windowWidth,
                        //height: 200,
                        open: function () {
                            $('#wdgtwrap2').show();
                        }
                    });
                    window.data("kendoWindow").setOptions({ width: windowWidth });
                    window.data("kendoWindow").center().open();
                },
                error: function (data) {
                    //Will write code later
                }
                //Ajax call end here
            });

        }

        //=================================================================


        //2nd level Wedget details handler===========================================

        var SeclevelWidgetDetailHandler = function SeclevelWidgetDetailHandler(e) {
            dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            var grid = $("#KgridFirstLevelWdgt").data("kendoGrid");
            var columns = grid.options.columns;
            var columnRow = "";
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].title != "Action") {
                    var f = columns[i].field;
                    columnRow += columns[i].field + "::" + dataItem[f] + "|";
                }
            }
            columnRow += "key_Company::" + dataItem.Key_Company + "|key_Act::" + dataItem.Key_Act;
            var tid = $("#hdnwidgetTID").val();
            var t = '{data: "' + columnRow + '", tid: "' + tid + '" }';
            $.ajax({
                type: "POST",
                url: "/Home.aspx/GetSecondlevelWidget",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: t,
                success: function (response) {
                    var res = response;
                    //$("#" + Divid).html("").show();
                    //$("#" + Divid).append("<h3>" + DbName + "</h3>");
                    //$("#" + Divid).append("<div id='kgrd" + index + "'></div>");
                    var result = res.d;
                    var data1 = "";
                    if (result.Data != "")
                        data1 = $.parseJSON(result.Data);
                    var Column = result.Column;
                    if (result.ShowAction == "yes") {
                        var CommanObj = { command: [{ name: "Details", Title: "Details", text: "Details" }], title: "Action", width: 60 }
                        Column.push(CommanObj);
                    }
                    if (data1 != "") {
                        //bindGrid1(data1, Column, "kgrd" + index);
                        $("#KgridSecondLevelWdgt").kendoGrid({
                            dataSource: {
                                data: data1
                            },
                            scrollable: true,
                            resizable: true,
                            reorderable: true,
                            sortable: true,
                            filterable: true,
                            columns: Column
                        });
                    }

                    else {
                        $("#kgrd" + index).append("<div id='norecord" + index + "' style='text-align:center; color:red; height: 88px;'><span style='color:red;  position: relative; top: 30px;'> No record found</span></div>")
                    }
                },
                error: function (data) {
                    //Will write code later
                }
                //Ajax call end here
            });
            var window = $("#wdgtwrap3");
            window.kendoWindow({
                title: 'Details',
                modal: true,
                width: 500,
                //height: 200,
                open: function () {
                    $('#wdgtwrap3').show();
                }
            });
            window.data("kendoWindow").center().open();
        }

        //=================================================================

        var kgrdbind;

        function bindUseFulLink(DbName, Divid) {
            $.ajax({
                type: "POST",
                url: "Home.aspx/GetUsefullLink",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //var Docdata = $.parseJSON(res);
                    //alert(res.DocList);
                    $("#" + Divid).html("").show();
                    //$("#" + Divid).append("<h3>" + DbName + "</h3>");
                    //$("#dv2").append("<h3>" + DbName + "</h3>");
                    var Docdata = {}, SsoList = {};
                    if (res.DocList != "")
                        Docdata = $.parseJSON(res.DocList);
                    if (res.SsoList != "")
                        SsoList = $.parseJSON(res.SsoList);
                    var dbtype = ""
                    $.each(SsoList, function (i) {
                        $("#" + Divid).append("<span>Please click on Help Desk to create support ticket or email us at support@myndhrohd.zendesk.com<br/><br/>Helpdesk No: 0124-4724693<br/></span>");
                        $("#" + Divid).append("<a class='SSOLink' style='cursor:pointer;' id='ssllnk" + i + "'>" + this.DisplayName + "</a>");
                        $("#ssllnk" + i).bind('click', Showlinkredirecr);
                        //Each function end here
                    });
                    var Str = "<ul class='doclink'>"
                    $.each(Docdata, function () {
                        Str = Str + "<li><a href='" + this.url + "'>" + this.displayName + "</a></li>"
                        //Each function end here
                    });
                    Str = Str + "</ul>"
                    $("#" + Divid).append(Str);

                    var window = $("#wdgtwrap");
                    window.kendoWindow({
                        title: 'Details',
                        modal: true,
                        width: 500,
                        //height: 200,
                        open: function () {
                            $('#wdgtwrap').show();
                        }
                    });
                    window.data("kendoWindow").setOptions({ width: 500 });
                    window.data("kendoWindow").center().open();
                    $("#wdgtwrap_wnd_title").html(DbName);
                    event.preventDefault();
                },
                error: function (data) {
                    //Will write code later
                }
                //Ajax call end here
            });
        }



        function GetPiChartWidget(Obj, Divid, DbName, index) {
            //var t = '{data: "' + columnRow + '", tid: "' + tid + '" }';
            var t = '{tid: "' + Obj + '" }';
            //var t = '{tid: '+Obj+'}';
            $.ajax({
                type: "POST",
                url: "/Home.aspx/GetPiChartWidget",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    $("#" + Divid).html("").show().removeClass("pos1");
                    //$("#" + Divid).html("").show().removeClass("pos1").resizable().draggable();;
                    // $("#" + Divid).html("").removeClass("pos")
                    //$("#" + Divid).append("<h3>" + DbName + "</h3>");
                    $("#" + Divid).append("<div  id='kpi" + index + "' style='height:200px;position:absolute;top:0;left:0;bottom:0;margin:0 auto;'></div>");
                    var chartID = "kpi" + index;
                    var d1 = res.data;
                    var windowWidth = "";
                    if (d1.length > 0) {
                        windowWidth = res.WindowWidth;

                        createChart(d1, "test", chartID);
                    }
                    else {
                        $("#" + chartID + index).append("<div id='norecord" + index + "' style='text-align:center; color:red; height: 88px;'><span style='color:red;  position: relative; top: 30px;'> No record found</span></div>")
                    }
                    //alert(windowWidth);
                    var window = $("#wdgtwrap");
                    window.kendoWindow({
                        title: 'Details',
                        modal: true,
                        width: windowWidth,
                        //height: 200,
                        open: function () {
                            $('#wdgtwrap').show();
                        }
                    });
                    window.data("kendoWindow").setOptions({ width: windowWidth });
                    window.data("kendoWindow").center().open();
                    $("#wdgtwrap_wnd_title").html(DbName);
                    event.preventDefault();
                },
                error: function (data) {
                    //Will write code later
                }
                //Ajax call end here
            });
        }
        function createChart(data1, Name, chartID) {
            $("#" + chartID).kendoChart({
                title: {
                    position: "bottom"
                },
                legend: {
                    visible: true
                },
                chartArea: {
                    background: ""
                },
                seriesDefaults: {
                    labels: {
                        visible: false,
                        background: "transparent",
                        template: "#= category #"
                    }
                },
                series: [{
                    type: "pie",
                    startAngle: 150,
                    data: data1
                }],
                tooltip: {
                    visible: true,
                    template: "${ category } - ${ value }"
                }
            });

        }
    </script>
    <script>
        $("form").submit(function (e) {
            e.preventDefault();
        });
    </script>
</asp:Content>


