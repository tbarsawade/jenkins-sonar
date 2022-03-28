<%@ page language="VB" autoeventwireup="false" inherits="NewDocDetail, App_Web_nfrpb0kv" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <link rel="shortcut icon" href="images/favicon.png" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>Doc Detail Page</title>
    <!-- Bootstrap -->
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery-ui.css" rel="stylesheetcol" type="text/css" />
    <%--<link href="css/style1.css" rel="stylesheet" type="text/css" />--%>

    <%--<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />--%>
    
  <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.0/js/bootstrap.min.js"></script>
    <%--<script type="text/javascript" src="js/jquery.min.js"></script>--%>
    <script  src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="js/enscroll-0.6.2.min.js"></script>
     <script src="assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <script src="js_child/jquery-ui.js"></script>
     <link href="css/DateTimePicker.css" rel="stylesheet" />
    <script src="js/DateTimePicker.js"></script>
    <script src="js/Canara/CanaraScript.js" type="text/javascript"></script>
    <script src="js/CanaraHSBC/CanaraHSBCScript.js" type="text/javascript"></script>
    <script src="js/FreeCharge/FreeChargeScript.js" type="text/javascript"></script>
    <script src="js/TicketUtils.js" type="text/javascript"></script>
    <script src="js/Utils.js" type="text/javascript"></script>
   <%-- <link rel="stylesheet" href="http://code.jquery.com/ui/1.11.0/themes/smoothness/jquery-ui.css">
    <script src="http://code.jquery.com/jquery-1.10.2.js"></script>
   <script src="http://code.jquery.com/ui/1.11.0/jquery-ui.js"></script>--%>
    <%-- Added By Manvendra Singh --%>
    <script src="js_child/jquery-1.10.2.js"></script>
    <script src="js_child/jquery-ui1.js"></script>
    <link rel="stylesheet"   href="js_child/fontsgoogleaspiscss.css" />
    <link rel="stylesheet" href="js_child/jqry-ui.css" />
      <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/js/select2.min.js"></script>
    <style>
		/* for create magnify glass */
		.large{
			width: 175px;
			height: 175px;
			position: absolute;
			border-radius: 100%;
			box-shadow: 0 0 0 7px rgba(255, 255, 255, 0.85), 
			0 0 7px 7px rgba(0, 0, 0, 0.25), 
			inset 0 0 40px 2px rgba(0, 0, 0, 0.25);
			display: none;
		}
		.small{
			display: block;
		}
    </style>
<script type="text/javascript">
    $(document).ready(function () {
        var sub_width = 0;
        var sub_height = 0;
        $(".large").css("background", "url('/" + $("#iframesrc").val() + "') no-repeat");
        $(".panel-body").mousemove(function (e) {
            //$("#btnZoomIn").click(function (e) {
            if (!sub_width && !sub_height) {
                var image_object = new Image();
                image_object.src = $("#iframesrc").val();
                sub_width = image_object.width;
                sub_height = image_object.height;
            }
            else {
                var magnify_position = $(this).offset();
                var mx = e.pageX - magnify_position.left;
                var my = e.pageY - magnify_position.top;
                if (mx < $(this).width() && my < $(this).height() && mx > 0 && my > 0) {
                    $(".large").fadeIn(50);
                }
                else {
                    $(".large").fadeOut(100);
                }
                if ($(".large").is(":visible")) {
                    var rx = Math.round(mx / $(".small").width() * sub_width - $(".large").width() / 2) * -1;
                    var ry = Math.round(my / $(".small").height() * sub_height - $(".large").height() / 2) * -1;
                    var bgp = rx + "px " + ry + "px";
                    var px = mx - $(".large").width() / 2;
                    var py = my - $(".large").height() / 2;
                    $(".large").css({ left: px, top: py, backgroundPosition: bgp });
                }
            }
        })
    })
    function ShowPopup(title, body) {
        $("#MyPopup .modal-title").html(title);
        $("#MyPopup .modal-body").html(body);
        $("#MyPopup").modal("show");
    }

    // below new by sunil on 27oct21 for prevendint postback on enter key hit
    $(document).ready(function () {
        $($("form")[0]).on("keypress", function (e) {
            var keycode = (e.keyCode ? e.keyCode : e.which);
            if (keycode == '13') {
                if ($(e.target).prop("type") === "textarea") {
                    return;
                }
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        });
    });

</script>
    <script type="text/javascript">
        function printIframe(id) {
            var iframe = document.frames ? document.frames[id] : document.getElementById(id);
            var ifWin = iframe.contentWindow || iframe;
            iframe.focus();
            ifWin.printPage();
            return false;
        }
    </script>
    <script type="text/javascript">
        $(function () {
            var id = '';
            $("[id*='img']").click(function () {
                id = (this.id);
            })


            $("[id*='img']").click(function () {
                //$.fn.datepicker.defaults.format = "mm/dd/yyyy";
                id = (this.id);
                var fid = id.replace("img", "fld");
                
                $("[id*='" + fid + "']").datepicker({
                    showmonth: true,
                    autoSize: true,
                    showAnim: 'slideDown',
                    duration: 'fast',
                    dateFormat:'dd/mm/y'
                    
                    
                });
               
                $(this).datepicker('show');
                
                
            });
        });
    </script>


    <style type="text/css">
        .mg-10 {
            margin-top: 10px;
        }
        .mg-btm {
            margin-bottom: 0px !important;
        }
        .panel-body{
            padding:9px !important;
        }
                .H5{
            margin: 0px;
            padding: 1px;
            line-height: 2.2;
            color: #fff;
        }

        .autocomplete_completionListElement {    
    width:500px!important;
    min-width:500px;
    margin : 0px!important;
    background-color : inherit;
    color : windowtext;
    border : buttonshadow;
    border-width : 1px;
    border-style : solid;
    overflow :auto;
    height : 200px;
    text-align : left; 
}

    </style>

    <style type="text/css">
        #scrollToTop, #scrollToBottom {
            cursor: pointer;
            background-color: #0090CB;
            display: inline-block;
            height: 17px;
            width: 18px;
            color: #fff;
            font-size: 13pt;
            text-align: center;
            text-decoration: none;
            line-height: 18px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#scrollToBottom').bind("click", function () {
                $('html, body').animate({ scrollTop: $(document).height() }, 1200);
                return false;
            });
            $('#scrollToTop').bind("click", function () {
                $('html, body').animate({ scrollTop: 0 }, 1200);
                return false;
            });
        });
    </script>
    <script>
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
    </script>
    <script type="text/javascript">

        function performCalc(docid, EidPage) {
            var randomnumber = Math.floor((Math.random() * 100) + 1);
            if (EidPage.includes(".msg")) {
                window.open('viewpdf.aspx?docid=' + docid + '&EidPage=' + EidPage + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');
            }
            else {
                window.open('DOCS/' + EidPage + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');
            }
        }
    </script>
    <script type="text/javascript">

        function DateCalender(Imgid, txtboxid) {
            $("#Imgid").click(function () {

                $("#txtboxid").datepicker();

            });
        }
    </script>
       <script type="text/javascript">
           function ClearFields(MainfileId, fileuploadid, hdnfldid) {
               var A = $("#" + MainfileId).val('');

               

               var V = $("#" + fileuploadid).text("");

               var C = $("#" + hdnfldid).val('');
           }
    </script>


    <script type="text/javascript">
        function performCalcNEwDocdetail(docid) {
            var randomnumber = Math.floor((Math.random() * 100) + 1);
            window.open('NewDocDetail.aspx?docid=' + docid + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="scriptmanager" runat="server"></ajaxToolkit:ToolkitScriptManager>
        <div class="WebPageMaxWidth bg-color">
            <header>
                <div class="container-fluid container-fluid-amend">
                    <div class="TopMasterHeader">
                        <div class="row">
                        <div class="col-md-7">
                            <div class="col-md-4">
                            <span class="Toplogo-img">
                                <img id="imglogo" runat="server" class="img-responsive" src="images/mynd-logo.png" alt="Pearl" />
                            </span>
                           </div>
                            <div class="col-md-8 mg-10">
                                <div id="usrnameandrole" runat="server"></div>
                                </div>
                        </div>
                        <div class="col-md-5">
                            <div class="col-md-9">
                                 <div class="btn-group pull-right">
                                <asp:Button class="btn btn-primary" ID="btnDocApprove" ToolTip="Approve" OnClick="ShowApprove" OnClientClick="ShowProgress();" runat="server" />
                                <asp:Button class="btn btn-primary" ID="btnDocReject" ToolTip="Reconsider" OnClick="ShowReconsider" OnClientClick="ShowProgress();" runat="server" />
                                <asp:Button class="btn btn-primary" ID="btnRejectDoc" ToolTip="Reject" OnClick="ShowPermanentReject" OnClientClick="ShowProgress();" runat="server" />
                                <asp:Button class="btn btn-primary" ID="btnDocEdit" ToolTip="Edit" runat="server" OnClientClick="ShowProgress();" />
                                <asp:Button class="btn btn-primary" ID="btnAmendment" ToolTip="Amendment" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                <asp:Button class="btn btn-primary" ID="btnRecall" ToolTip="Recall" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                <asp:Button class="btn btn-primary" ID="btnCancel" ToolTip="Cancel" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                <asp:Button class="btn btn-primary" ID="btnSplit" ToolTip="Split/Copy" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                <asp:Button class="btn btn-primary" ID="btnCopy" ToolTip="Copy" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                <asp:Button class="btn btn-primary" ID="btnEPD" ToolTip="Early Payment Discounting" OnClick="EarlyPaymentDiscounting" runat="server" Visible="false" />
                            </div>
                            </div>
                            <div class="col-md-3 mg-10">
                            <div class="btn-group pull-right">
                                <asp:ImageButton ID="btnPrint1" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/print1.png" />
                                <asp:ImageButton ID="btnPrint2" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/print1.png" CssClass="print" />
                                <asp:ImageButton ID="btnPrint3" class="printPage" ToolTip="Print" runat="server" ImageUrl="~/images/print1.png" />
                                <asp:ImageButton ID="btnPrintWord" OnClick="PrintWord" ToolTip="Print" Visible="false" runat="server" ImageUrl="~/images/word.png" />
                                 <asp:ImageButton ID="btnPrintExcel" OnClick="PrintExcel" ToolTip="Print" Visible="false" runat="server" ImageUrl="~/images/excelexpo.jpg" />
                                <asp:ImageButton ID="btnCloseWindow" OnClientClick="javaScript:window.close(); return false;" ToolTip="Close" runat="server" ImageUrl="~/images/cross.png" />
                            </div>

                            <%--<div align="right">--%>
                            <%--</div>--%>

                            <!-- END TOP NAVIGATION MENU -->
                        </div>
                        </div>
                        </div>
                        <%-- <span class="font-icon"><i class="fa fa-print"></i></span>
                <span class="font-icon"><i class="fa fa-times-circle-o"></i></span>--%>
                    </div>
                </div>
            </header>
   
            <div class="clear20"></div>
            <%--<ajaxToolkit:ToolkitScriptManager ID="tscript" runat="server"></ajaxToolkit:ToolkitScriptManager>--%>

            <section>
                <div class="container-fluid container-fluid-amend">
                    <div class="panel panel-default panel-amend">
                        <div class="panel-body pd-0">
                            <div class="row">
                                <div class="col-md-12" style="padding-left: 0;">
                                    <div class="pull-right">
                                        <a href="javascript:;" id="scrollToBottom" data-togle="tooltip" title="Scroll Down">&#x25BC;</a>
                                    </div>
                                    <div class="table-responsive" id="ulProgressBar" runat="server">
                                        <%--<ul class="progress-tracker progress-tracker--text progress-tracker--center trip-track" >
                                            <li class="progress-step is-complete">
                                                <span class="progress-marker"></span>
                                                <span class="progress-text">
                                                    <span class="author">Abhi Sharma</span>
                                                    <span class="date">11/15/2014- 11/15/2014</span>
                                                    <span class="subauthor">Approved </span>
                                                </span>
                                            </li>

                                            <li class="progress-step is-complete">
                                                <span class="progress-marker"></span>
                                                <span class="progress-text">
                                                    <span class="author">Abhi Sharma</span>
                                                    <span class="date">11/15/2014- 11/15/2014</span>
                                                    <span class="subauthor">Approved </span>
                                                </span>
                                            </li>

                                            <li class="progress-step is-complete">
                                                <span class="progress-marker"></span>
                                                <span class="progress-text">
                                                    <span class="author">Abhi Sharma</span>
                                                    <span class="date">11/15/2014- 11/15/2014</span>
                                                    <span class="subauthor">Approved </span>
                                                </span>
                                            </li>

                                            <li class="progress-step is-active">
                                                <span class="progress-marker"></span>
                                                <span class="progress-text">
                                                    <span class="author">Abhi Sharma</span>
                                                    <span class="date">11/15/2014- 11/15/2014</span>
                                                    <span class="subauthor">Pending</span>
                                                </span>
                                            </li>

                                            <li class="progress-step">
                                                <span class="progress-marker"></span>
                                                <span class="progress-text">
                                                    <span class="author">Abhi Sharma</span>
                                                    <span class="date">11/15/2014- 11/15/2014</span>
                                                    <span class="subauthor">Shift Completed</span>
                                                </span>
                                            </li>
                                        </ul>--%>
                                    </div>


                                </div>
                                <!-- time line --->
                                <%-- <div class="col-md-1 viewall">
                                    <a data-toggle="modal" href="#viewall" class="">View All</a>
                                </div>--%>
                                <!--end timeline  -->
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-7 col-sm-7 col-xs-12 pd-right">
                            <div class="PanelContentBox1" id="popdiv" runat="server" style="display: none;">
                                <div class="panel panel-default panel-amend">

                                    <%-- <div class="panel-body">--%>
                                    <asp:Panel ID="pnlPerReject" runat="server" Width="100%" Visible="false">
                                        <div class="panel-heading">
                                            <div class="pull-left">
                                                <h2 class="mg-top">Permanent 
                                                    <asp:Label ID="lblPREJ" runat="server"></asp:Label>
                                                    </h2>
                                            </div>
                                            <%--<div class="pull-right">
                                                <asp:ImageButton ID="btnClosePerReject" OnClick="btnClosePerReject_Click" ImageUrl="images/close.png" runat="server" />
                                            </div>--%>
                                            <div class="clear"></div>
                                        </div>
                                        <asp:UpdatePanel ID="updPerReject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="padding">
                                                    <div class="form-group row">
                                                        <div class="col-md-12 col-sm-12">
                                                            <asp:Label ID="lblPerRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <asp:Panel ID="PanelPerReject" Width="100%" runat="server">
                                                    </asp:Panel>
                                                    <div class="form-group row">
                                                        <div class="col-md-12 col-sm-12">
                                                            <asp:Label ID="lblMsgRule1" runat="server" Text=""></asp:Label>
                                                        </div>

                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <div class="pull-right">
                                                            <asp:Button ID="btnPerReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                                                runat="server" CssClass="btn btn-primary" OnClick="editBtnPerReject" Text="Reject" />
                                                        </div>
                                                            </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                    </asp:Panel>
                                       
                                    <asp:Panel ID="pnlPopupApprove" runat="server" Width="100%" Visible="false" ScrollBars="None">
                                        <div class="panel-heading">
                                            <div class="pull-left">

                                                <h2 class="mg-top5">
                                                    <asp:Label ID="lblAppdoc" runat="server"></asp:Label>
                                                    </h2>
                                            </div>
                                            <div class="pull-right">
                                                <%--<i class="fa fa-times-circle-o"> --%>
                                                <%--<asp:ImageButton ID="btnCloseApprove" OnClick="btnCloseApprove_Click" ImageUrl="images/close.png" runat="server" />--%>

                                                <%--</i>--%>
                                            </div>
                                            <div class="clear"></div>
                                        </div>
                                        <asp:UpdatePanel ID="updatePanelApprove" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div>
                                                    <div class="form-group row">
                                                        <div class="col-md-12 col-sm-12">
                                                            <asp:Label ID="lblTabApprove" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                        </div>
                                                    </div>

                                                    <asp:Panel ID="pnlApprove" Width="100%" runat="server">
                                                    </asp:Panel>

                                                    <div class="form-group row">
                                                        <div class="col-md-12 col-sm-12">
                                                            <asp:Label ID="lblMsgRule2" runat="server" Text=""></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-12" style="text-align: right">
                                                             <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClientClick="this.disabled=true;"
                                                                UseSubmitBehavior="false" CssClass="btn btn-primary" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                    </asp:Panel>

                                    <asp:Panel ID="pnlEdit" Height="630" Visible="false" runat="server">
                                        <div class="panel-heading" style="background-color:#428bca">
                                            <div class="pull-left">

                                                <h2 class="mg-top" style="color:#fff;">
                                                    <asp:Label ID="lblstatus" runat="server" Text="Document Editing" Font-Bold="True"></asp:Label>
                                                </h2>
                                            </div>
                                            <%--<div class="pull-right">
                                                <%--<i class="fa fa-times-circle-o"> --%>
                                                <%--<asp:ImageButton ID="ImageButton1" ImageUrl="images/close.png" runat="server" />--%>

                                                <%--</i>--%>
                                            <%--</div>--%>
                                            <div class="clear"></div>
                                        </div>
                                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="padding">
                                                    <div class="form-group row">
                                                        <div class="col-md-12 col-sm-12">
                                                            <asp:Label ID="lblDetail1" runat="server" ForeColor="Red"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <asp:Panel ID="pnlFields" runat="server">
                                                    </asp:Panel>
                                                       <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="pull-right">
                                                        <%--<asp:Button ID="btnActEdit" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="EditRecord" />--%>
                                                        <asp:Button ID="btnActEdit" runat="server" Text="Save" OnClientClick="this.disabled=true;ShowProgress();" UseSubmitBehavior="false" CssClass="btn btn-primary" OnClick="btnActEdit_Click"/>
                                                            </div>
                                                    </div>
                                                </div>
                                                </div>
                                                <div id="dtBox" style="z-index: 9999999999;">
                                                    
                                                </div>
                                              
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:Panel>
                                    <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional">
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnPrint1" />
                                            <asp:PostBackTrigger ControlID="btnPrint2" />
                                            <asp:PostBackTrigger ControlID="btnPrint3" />
                                            <asp:PostBackTrigger ControlID="btnPrintWord" />
                                             <asp:PostBackTrigger ControlID="btnPrintExcel" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:Panel ID="pnlShowPopupHistory" runat="server" Height="300px"
                                                BackColor="White" Style="" Visible="false">
                                                <div class="box">
                                                    <table cellspacing="1px" cellpadding="1px" width="100%">
                                                        <tr>
                                                            <td width="980px">
                                                                <h3>Transaction History
                                                                </h3>
                                                            </td>
                                                            <td style="width: 20px">
                                                                <asp:ImageButton ID="btnCloseShowPopupHistory" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center">
                                                                <asp:UpdatePanel ID="UpdatePanel_ShowPopupHistory" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Panel ID="pnlgridhistory" runat="server" ScrollBars="Both" Width="100%">
                                                                            <asp:GridView ID="grdShowPopupHistory" DataKeyNames="TID" Width="100%" runat="server"
                                                                                BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px"
                                                                                CellPadding="3" CellSpacing="2" AutoGenerateColumns="true">
                                                                                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                                                                                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                                                                                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                                                                                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                                                                                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                                                                                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                                                                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                                                                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                                                                <SortedDescendingHeaderStyle BackColor="#93451F" />
                                                                            </asp:GridView>
                                                                        </asp:Panel>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="pncCRMViewConver" runat="server" BorderColor="#666666" Visible="false"
                                                ForeColor="Black" Style="display: none;" BackColor="Silver">
                                                <div>
                                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                                        <tr>
                                                            <td width="95%">
                                                                <h3 style="border: solid 2px #f6f6f6;">View Conversation</h3>
                                                            </td>
                                                            <td style="width: 5%">
                                                                <asp:ImageButton ID="IMgCloseCRM" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="text-align: left; width: 100%">
                                                                <asp:UpdatePanel ID="upCRMConver" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <table cellspacing="2px" cellpadding="2px" width="100%">
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="4">
                                                                                    <asp:Label ID="CRMMessage" ForeColor="Red" runat="server"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td width="15%">
                                                                                    <b>Mobile No.</b>
                                                                                </td>
                                                                                <td width="35%">
                                                                                    <asp:Label runat="server" ID="lblMobNum"></asp:Label>
                                                                                </td>
                                                                                <td width="10%">
                                                                                    <b>To Mail.</b>
                                                                                </td>
                                                                                <td width="40%">
                                                                                    <asp:Label runat="server" ID="lblTomail"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan="4">
                                                                                    <b>Conversation:</b>
                                                                                    <div style="border: solid 1px #4cff00; overflow: scroll; width: 100%; height: 200px; overflow: auto;">
                                                                                        <asp:Label ID="lblConver" runat="server"></asp:Label>
                                                                                    </div>
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
                                            <asp:Panel ID="pnlSendSmS" runat="server" BorderColor="#666666" ForeColor="Black"
                                                Style="display: none;" BackColor="Silver">
                                                <div>
                                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                                        <tr>
                                                            <td>
                                                                <h3 style="border: solid 2px #f6f6f6;">Send Message</h3>
                                                            </td>
                                                            <td style="width: 20px">
                                                                <asp:ImageButton ID="imgSmsClose" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="text-align: left; width: 100%">
                                                                <asp:UpdatePanel ID="upSendMessage" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <table cellspacing="2px" cellpadding="2px" width="100%">
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="2">
                                                                                    <asp:Label ID="lblSMSMessage" ForeColor="Red" runat="server"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Mobile No.<span style="color: red;">*</span>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtMob" onkeypress="return numbersonly(event);" runat="server" MaxLength="10"
                                                                                        Width="200"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="2"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Message.<span style="color: red;">*</span>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox runat="server" Columns="10" TextMode="MultiLine" Rows="5" ID="txtMeaasge"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="2"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td></td>
                                                                                <td align="right">
                                                                                    <asp:Button ID="btnSendSMS" runat="server" Text="Send" CssClass="ststusButton" OnClientClick="javascript:return ValidateSMS();" />
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
                                            <asp:Panel ID="pnlRecall" runat="server" Visible="false" Height="95px" ScrollBars="Auto"
                                                BackColor="aqua">
                                                <div class="box">
                                                    <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                                        <tr>
                                                            <td>
                                                                <h3>
                                                                    <asp:Label ID="lblAmendment" runat="server" Font-Bold="True" Text="DOCUMENT RECALL"></asp:Label></h3>
                                                            </td>
                                                            <td style="width: 20px">
                                                                <asp:ImageButton ID="btnCloseRecall" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:UpdatePanel ID="updRecall" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Panel ID="Panel2" runat="server">
                                                                            <asp:Label ID="lblRecallMess" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                        </asp:Panel>
                                                                        <asp:Panel ID="Panel3" Width="100%" runat="server">
                                                                            <asp:Label ID="Label4" runat="server" Text="Recall Remarks" Font-Bold="True"></asp:Label>
                                                                            &nbsp;&nbsp
                                                                            <asp:TextBox ID="txtRemarkRecall" runat="server" CssClass="txtBox" Width="500px"></asp:TextBox>
                                                                        </asp:Panel>
                                                                        <div style="width: 100%; text-align: right">
                                                                            <asp:Button ID="btnRecallSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                                                                Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                                                            <asp:Button ID="btnCancelReject" runat="server" Text="Cancel" CssClass="btnNew" Font-Bold="True"
                                                                                Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlSendMail" runat="server" Width="850" Height="550" BorderColor="#666666"
                                                Style="display: none;" ForeColor="Black" BackColor="Silver">
                                                <div>
                                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                                        <tr>
                                                            <td style="width: 95%;">
                                                                <h3 style="border: solid 2px #f6f6f6;">Send Mail</h3>
                                                            </td>
                                                            <td style="width: 5%">
                                                                <asp:ImageButton ID="imgClosebtnClose" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="text-align: left; width: 100%">
                                                                <asp:UpdatePanel ID="UpSendMail" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <table cellspacing="2px" cellpadding="2px" width="100%">
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="4">
                                                                                    <asp:Label ID="lblsendMSG" ForeColor="Red" runat="server"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td width="10%">From Email.<span style="color: red;">*</span>
                                                                                </td>
                                                                                <td width="40%">
                                                                                    <asp:DropDownList runat="server" onchange="javascript:TomailChange(this.value);"
                                                                                        Width="250" ID="ddlFromMail" CssClass="txtBox">
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                                <td width="10%">Password.
                                                                                </td>
                                                                                <td width="50%">
                                                                                    <asp:TextBox ID="txtPassword" TextMode="Password" Width="250" ReadOnly="true" CssClass="txtBox"
                                                                                        runat="server"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 10px;">
                                                                                <td colspan="4"></td>
                                                                            </tr>
                                                                            <%--<tr>
                                                    <td>SMTP Server.</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSMTP" ReadOnly="true" runat="server" CssClass="txtBox" Width="400" Height="30"></asp:TextBox>
                                                    </td>
                                                </tr>--%>
                                                                            <tr style="height: 10px;">
                                                                                <td colspan="4"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>To.<span style="color: red;">*</span>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtToMail" placeholder="Enter emails seperated by ','" Width="250"
                                                                                        runat="server" CssClass="txtBox"></asp:TextBox>
                                                                                </td>
                                                                                <td>CC.
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtCC" runat="server" placeholder="Enter emails seperated by ','"
                                                                                        Width="250" CssClass="txtBox"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="4"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Bcc.
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtCRMBCC" placeholder="Enter emails seperated by ','" Width="250"
                                                                                        runat="server" CssClass="txtBox"></asp:TextBox>
                                                                                </td>
                                                                                <td>Subject.
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtCRMSub" runat="server" placeholder="Enter subject" Width="250"
                                                                                        CssClass="txtBox"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 20px;">
                                                                                <td colspan="4"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Body.<span style="color: red;">*</span>
                                                                                </td>
                                                                                <td align="left" style="height: 250px;" colspan="3">
                                                                                    <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="250px" Width="650"
                                                                                        BackColor="White">
                                                                                        <asp:TextBox ID="txtBody" Visible="false" TextMode="MultiLine" Text=" " runat="server"
                                                                                            Width="100%" Height="98%" BackColor="White">
                                                                                        </asp:TextBox>
                                                                                        <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" TargetControlID="txtbody"
                                                                                            EnableSanitization="false">
                                                                                        </asp:HtmlEditorExtender>
                                                                                    </asp:Panel>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 10px;">
                                                                                <td colspan="4"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right" colspan="4">
                                                                                    <asp:Button ID="btnSendmailHit" OnClick="btnSendmailHit_Click" OnClientClick="javascript:return ValidateCRMMail();"
                                                                                        runat="server" Text="Send" CssClass="ststusButton" />
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
                                            <asp:Panel ID="pnlPopupDiscounting" runat="server" Width="999px" Height="350px" Style="display: none; overflow: auto" BackColor="White">
                                                <div class="doc_header">
                                                    Discounting Details
                                 <div class="pull-right">
                                     <asp:ImageButton ID="btnCloseDiscounting" ImageUrl="images/close.png" runat="server" />
                                 </div>

                                                </div>
                                                <div class="row" style="margin: 10px 0px;">
                                                    <div class="col-md-12 col-sm-12  col-xs-12">
                                                        <div class="col-md-3 col-sm-3  col-xs-3">
                                                            <label>Invoice Amount</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3">
                                                            <asp:TextBox ID="txtPaidAmount" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3">
                                                            <label>Discounting Type</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3">
                                                            <asp:DropDownList ID="ddlDiscountingType" runat="server" CssClass="txtBox" onchange="javascript:discounttype();">
                                                            </asp:DropDownList>
                                                        </div>

                                                    </div>
                                                </div>
                                                <div class="row" style="margin: 5px 0px;">

                                                    <div class="col-md-12 col-sm-12  col-xs-12">
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Discounting Rate List</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:DropDownList ID="ddlDiscounting" runat="server" CssClass="txtBox" onchange="javascript:discountratelist();">
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Rate Of Interest</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:TextBox ID="txtROI" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                                        </div>

                                                    </div>
                                                </div>
                                                <div class="row" style="margin: 5px 0px;">
                                                    <div class="col-md-12 col-sm-12  col-xs-12">
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Days</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:TextBox ID="txtNOD" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Discount Margin (%)</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:TextBox ID="txtMarginMoney" runat="server" CssClass="txtBox" onkeyup="javascript:netpaidamount();"></asp:TextBox>
                                                        </div>

                                                    </div>
                                                </div>
                                                <div class="row" style="margin: 5px 0px;">
                                                    <div class="col-md-12 col-sm-12  col-xs-12">
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Payment Date</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:TextBox ID="txtPaymentDate" runat="server" CssClass="txtBox disable_past_dates" ReadOnly="true" Width="81%" onchange="return CalWithCalendar();"></asp:TextBox>
                                                            <img id="ContentPlaceHolder1_imgtxtPaymentDate" src="images/cal.png" />
                                                            <asp:CalendarExtender ID="cal_txtPaymentDate" runat="server" PopupButtonID="ContentPlaceHolder1_imgtxtPaymentDate" TargetControlID="txtPaymentDate" Enabled="true" Format="dd/MM/yy"></asp:CalendarExtender>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Due Date</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:TextBox ID="txtDueDate" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                                        </div>

                                                    </div>
                                                </div>
                                                <div class="row" style="margin: 5px 0px;">
                                                    <div class="col-md-12 col-sm-12  col-xs-12">
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <label>Net Paid Amount</label>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3  col-xs-3 self">
                                                            <asp:TextBox ID="txtnetPaidAmount" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                                            <asp:HiddenField ID="hdnDocID" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row pull-right" style="margin: 5px 0px;">
                                                    <div class="col-md-12 col-sm-12  col-xs-12">
                                                        <button type="button" class="btn btn-primary submit" onclick="AcceptDiscounting('REJECT');" style="height: 30px; width: 100px;" value="Login"><i class="fa fa-times"></i>Reject</button>
                                                        <button type="button" id="btnAccept" class="btn btn-success submit" onclick="AcceptDiscounting('ACCEPT');" style="height: 30px; width: 100px;" value="Login"><i class="fa fa-check"></i>Accept</button>
                                                    </div>
                                                </div>
                                                <%--  </div>--%>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlPopupForm" runat="server" Width="400px" Height="100px" BackColor="White"
                                                Style="display: none">
                                                <div class="box">
                                                    <table cellspacing="0px" cellpadding="0px" width="100%">
                                                        <tr>
                                                            <td style="width: 400px">
                                                                <h3>Confirmation</h3>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:UpdatePanel ID="updMsg" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Label ID="lblMsg" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Button ID="btnOk" runat="server" Text="OK" OnClick="Reset" CssClass="btnNew"
                                                                    Width="80px" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlCancel" runat="server" Visible="false" Height="300px" ScrollBars="Auto"
                                                BackColor="aqua">
                                                <div class="box">
                                                    <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                                        <tr>
                                                            <td>
                                                                <h3>
                                                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="DOCUMENT CANCEL"></asp:Label></h3>
                                                            </td>
                                                            <td style="width: 20px">
                                                                <asp:ImageButton ID="btnClosecancel" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:UpdatePanel ID="UpdDocCancel" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Panel ID="Panel4" runat="server">
                                                                            <asp:Label ID="lblDocCancelMsg" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                        </asp:Panel>
                                                                        <asp:Panel ID="pnlDocCancel" Width="100%" runat="server">
                                                                        </asp:Panel>
                                                                        <div style="width: 100%; text-align: right">
                                                                            <asp:Button ID="btnCancelSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                                                                Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                                                            <asp:Button ID="btnCancelCan" runat="server" Text="Cancel" CssClass="btnNew" Font-Bold="True"
                                                                                Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                             
                                         
                                            <asp:Panel ID="pnlPopupchild" runat="server" Visible="false" Height="300px" ScrollBars="Auto"
                                                BackColor="aqua">
                                                <div class="box">
                                                    <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                                        <tr>
                                                            <td>
                                                                <h3>
                                                                    <asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></h3>
                                                            </td>
                                                            <td style="width: 20px">
                                                                <asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Panel ID="Pnllable" runat="server">
                                                                            <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                        </asp:Panel>
                                                                        <asp:Panel ID="pnlFields1" Width="100%" runat="server">
                                                                        </asp:Panel>
                                                                        <div style="width: 100%; text-align: right">
                                                                            <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                                                                OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;"
                                                                                UseSubmitBehavior="false" />
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>


                                            <asp:Panel ID="pnlCRMAction" runat="server" Style="display: none; overflow: auto"
                                                BackColor="aqua">
                                                <div class="box" style="width: 100%;">
                                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                                        <tr>
                                                            <td>
                                                                <h3>Add Activity</h3>
                                                            </td>
                                                            <td style="width: 20px">
                                                                <asp:ImageButton ID="IMGCRMClose" ImageUrl="images/close.png" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="text-align: left; width: 100%">
                                                                <asp:UpdatePanel ID="UpCRMACtion" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Label ID="lblCRMAction" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                        <asp:Panel ID="pnlCRMFields" Width="100%" runat="server">
                                                                        </asp:Panel>
                                                                        <div style="width: 100%; text-align: right">
                                                                            <asp:Button ID="btnCRMAction" runat="server" Text="Add" OnClientClick="this.disabled=true;"
                                                                                UseSubmitBehavior="false" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small"
                                                                                Width="100px" OnClick="btnCRMApprove" />
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <asp:Panel ID="pnlPopupReject" runat="server" Height="300px" Visible="false" ScrollBars="Auto">
                                               
                                                <div class="panel-heading" style="background-color:#428bca">
                                            <div class="pull-left">

                                                <h2 class="mg-top" style="color:#fff;">
                                                    <asp:Label ID="lblrejDoc" runat="server"></asp:Label>
                                                    </h2>
                                            </div>
                                            <div class="pull-right">
                                                <%--<i class="fa fa-times-circle-o"> --%>
                                                <%--<asp:ImageButton ID="btnCloseApprove" OnClick="btnCloseApprove_Click" ImageUrl="images/close.png" runat="server" />--%>

                                                <%--</i>--%>
                                            </div>
                                            <div class="clear"></div>
                                        </div>
                                                                <asp:UpdatePanel ID="updatePanelReject" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate>
                                                                        <asp:Label ID="lblTabRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                                        <asp:HiddenField ID="hdnIsConditionalReconsider" runat="server" />
                                                                        <div id="DIVConditionalReconsider" runat="server" class="form" visible="false" style="min-height: 50px !important; width: 95%;">
                                                                           
                                                                                        <asp:DropDownList ID="ddlReconsiderStage" CssClass="txtBox" runat="server" Visible="true"></asp:DropDownList>
                                                                                  
                                                                        </div>
                                                                        <asp:Panel ID="pnlFieldsRej" Width="100%" runat="server">
                                                                        </asp:Panel>
                                                                        <div style="width: 100%; text-align: right">
                                                                            <asp:Label ID="lblRuleMsg3" runat="server" Text=""></asp:Label>
                                                                            <asp:Button ID="btnReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                                                                runat="server" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small" OnClick="editBtnReject"
                                                                                Text="Reject" Width="100px" />
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            
                                            </asp:Panel>
                                    <%--</div>--%>
                                </div>
                            </div>
                            <div class="PanelContentBox1">
                                <div class="panel panel-default panel-amend">
                                    <div class="panel-body">

                                        <ul class="nav nav-tabs NotificationsTabMain">
                                            <li id="li1" class="t" onclick="javascript:setCSS(this);">
                                                <a href="javascript:void(0);" style="color:#fff"><b>CURRENT</b></a>
                                            </li>
                                            <li id="li2" runat="server" class="group t" onclick="javascript:setCSS(this);"><a
                                                href="javascript:void(0);" style="color:#fff"><b>HISTORY</b></a></li>
                                            <li id="li3" runat="server" class="group t" onclick="javascript:setCSS(this);"><a
                                                href="javascript:void(0);">
                                                <asp:Label ID="lblCRM" runat="server"></asp:Label>
                                            </a></li>
                                             <li id="li4" runat="server" class="group t" >
                                                 <div class="row-fluid">
                                                     <div class="span12">
                                                         <div class="portlet box blue">
                                                             <div class="portlet-title">
                                                                 <div id="lbldetailofdoc" runat="server" class="caption"></div>
                                                             </div>
                                                         </div>
                                                     </div>
                                                </div>
                                            </li>
                                         </ul>
                                        <div class="clear20"></div>
                                        <div class="tab-content">
                                            <%--  <div id="NotificationsTab1" class="tab-pane fade in active">
                                                <div class="NotificationsBoxMain">
                                                    <div class="row">
                                                        <div class="databox">
                                                            <div class="vendor-bg">
                                                                <h4>Invoice PO - <small>[System Document ID: 2188806] LoggedIn User: [DH_Adm_HR]| Role :[Approver]</small> </h4>
                                                            </div>
                                                            <div class="table-responsive">
                                                                <div class="col-md-12 col-sm-12 col-xs-12 pd-0">
                                                                    <div class="panel-default mg-0">
                                                                        <div class="panel-heading v-response-panelbg">
                                                                            <h4 class="panel-title">
                                                                                <a data-toggle="collapse" href="#load" class="rfpheading" aria-expanded="true">PO & Vendor Details 
                                                    <i class="dot"></i>
                                                                                </a>
                                                                            </h4>
                                                                        </div>
                                                                        <div id="load" class="panel-collapse collapse in" aria-expanded="true" style="">
                                                                            <div class="panel-body pd-0">
                                                                                <div class="TableOuterDiv table-responsive">
                                                                                    <table class="table CustomTable">
                                                                                        <tbody class="horizontalscroll">
                                                                                            <tr>
                                                                                                <th>Created By</th>
                                                                                                <td>Test Requester</td>
                                                                                                <th>Creation Date</th>
                                                                                                <td>24/10/18 – 1</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Current Status</th>
                                                                                                <td>GRN / SRN</td>
                                                                                                <th>Invoice ID</th>
                                                                                                <td>POINV10242</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <td>PO_10039</td>
                                                                                                <th>PO Amount (Excl. Tax)</th>
                                                                                                <td>800</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Balance PO Amount</th>
                                                                                                <td>100</td>
                                                                                                <th>Payment Term Description</th>
                                                                                                <td>45 DAYS </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Valid From</th>
                                                                                                <td>14/05/18</td>
                                                                                                <th>Valid To</th>
                                                                                                <td>30/06/18</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Vendor Name</th>
                                                                                                <td>RENT WORK INDIA PVT.LTD.</td>
                                                                                                <th>Vendor Code</th>
                                                                                                <td>TestV </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor PAN</th>
                                                                                                <td>AAACA0120J</td>
                                                                                                <th>Vendor GSTN Status</th>
                                                                                                <td>Registered</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor GSTIN</th>
                                                                                                <td>09AAAA01201ZV</td>
                                                                                                <th>Place of Supply/Service</th>
                                                                                                <td>Gurugram</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Company Name</th>
                                                                                                <td>Travel Corporation</td>
                                                                                                <th>Item Sub Category</th>
                                                                                                <td>Printing Stationary</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>PO Accepted</th>
                                                                                                <td>YES</td>
                                                                                                <th></th>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Business Unit Name</th>
                                                                                                <td>CUSTOMER SERVICE SUPPORT</td>
                                                                                                <th>Business Unit Code</th>
                                                                                                <td>css</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Department</th>
                                                                                                <td></td>
                                                                                                <th>Early Payment Required</th>
                                                                                                <td>No</td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="panel-default mg-0">
                                                                        <div class="panel-heading v-response-panelbg">
                                                                            <h4 class="panel-title">
                                                                                <a data-toggle="collapse" href="#escort" class="rfpheading">Invoice Details
                                                    <i class="dot"></i>
                                                                                </a>
                                                                            </h4>
                                                                        </div>
                                                                        <div id="escort" class="panel-collapse collapse" aria-expanded="true" style="">
                                                                            <div class="panel-body pd-0">
                                                                                <div class="TableOuterDiv table-responsive">
                                                                                    <table class="table CustomTable table-bordered">
                                                                                        <thead></thead>
                                                                                        <tbody class="horizontalscroll">
                                                                                            <tr>
                                                                                                <th>Created By</th>
                                                                                                <td>Test Requester</td>
                                                                                                <th>Creation Date</th>
                                                                                                <td>24/10/18 – 1</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Current Status</th>
                                                                                                <td>GRN / SRN</td>
                                                                                                <th>Invoice ID</th>
                                                                                                <td>POINV10242</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <td>PO_10039</td>
                                                                                                <th>PO Amount (Excl. Tax)</th>
                                                                                                <td>
                                                                                                    <button type="button" class="btn btn-default BtnSm">View Attachment</button>
                                                                                                </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Balance PO Amount</th>
                                                                                                <td>100</td>
                                                                                                <th>Payment Term Description</th>
                                                                                                <td>45 DAYS </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Valid From</th>
                                                                                                <td>14/05/18</td>
                                                                                                <th>Valid To</th>
                                                                                                <td>30/06/18</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Vendor Name</th>
                                                                                                <td>RENT WORK INDIA PVT.LTD.</td>
                                                                                                <th>Vendor Code</th>
                                                                                                <td>TestV </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor PAN</th>
                                                                                                <td>AAACA0120J</td>
                                                                                                <th>Vendor GSTN Status</th>
                                                                                                <td>Registered</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor GSTIN</th>
                                                                                                <td>09AAAA01201ZV</td>
                                                                                                <th>Place of Supply/Service</th>
                                                                                                <td>Gurugram</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Company Name</th>
                                                                                                <td>Travel Corporation</td>
                                                                                                <th>Item Sub Category</th>
                                                                                                <td>Printing Stationary</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>PO Accepted</th>
                                                                                                <td>YES</td>
                                                                                                <th></th>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Business Unit Name</th>
                                                                                                <td>CUSTOMER SERVICE SUPPORT</td>
                                                                                                <th>Business Unit Code</th>
                                                                                                <td>css</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Department</th>
                                                                                                <td></td>
                                                                                                <th>Early Payment Required</th>
                                                                                                <td>No</td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="panel-default mg-0">
                                                                        <div class="panel-heading v-response-panelbg">
                                                                            <h4 class="panel-title">
                                                                                <a data-toggle="collapse" href="#discount" class="rfpheading">Early Payment Discouting
                                                    <i class="dot"></i>
                                                                                </a>
                                                                            </h4>
                                                                        </div>

                                                                        <div id="discount" class="panel-collapse collapse" aria-expanded="">
                                                                            <div class="panel-body pd-0">
                                                                                <div class="TableOuterDiv table-responsive">
                                                                                    <table class="table CustomTable table-bordered">
                                                                                        <thead></thead>
                                                                                        <tbody class="horizontalscroll">
                                                                                            <tr>
                                                                                                <th>Created By</th>
                                                                                                <td>Test Requester</td>
                                                                                                <th>Creation Date</th>
                                                                                                <td>24/10/18 – 1</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Current Status</th>
                                                                                                <td>GRN / SRN</td>
                                                                                                <th>Invoice ID</th>
                                                                                                <td>POINV10242</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <td>PO_10039</td>
                                                                                                <th>PO Amount (Excl. Tax)</th>
                                                                                                <td>800</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Balance PO Amount</th>
                                                                                                <td>100</td>
                                                                                                <th>Payment Term Description</th>
                                                                                                <td>45 DAYS </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Valid From</th>
                                                                                                <td>14/05/18</td>
                                                                                                <th>Valid To</th>
                                                                                                <td>30/06/18</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Vendor Name</th>
                                                                                                <td>RENT WORK INDIA PVT.LTD.</td>
                                                                                                <th>Vendor Code</th>
                                                                                                <td>TestV </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor PAN</th>
                                                                                                <td>AAACA0120J</td>
                                                                                                <th>Vendor GSTN Status</th>
                                                                                                <td>Registered</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor GSTIN</th>
                                                                                                <td>09AAAA01201ZV</td>
                                                                                                <th>Place of Supply/Service</th>
                                                                                                <td>Gurugram</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Company Name</th>
                                                                                                <td>Travel Corporation</td>
                                                                                                <th>Item Sub Category</th>
                                                                                                <td>Printing Stationary</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>PO Accepted</th>
                                                                                                <td>YES</td>
                                                                                                <th></th>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Business Unit Name</th>
                                                                                                <td>CUSTOMER SERVICE SUPPORT</td>
                                                                                                <th>Business Unit Code</th>
                                                                                                <td>css</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Department</th>
                                                                                                <td></td>
                                                                                                <th>Early Payment Required</th>
                                                                                                <td>No</td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="panel-default mg-0">
                                                                        <div class="panel-heading v-response-panelbg">
                                                                            <h4 class="panel-title">
                                                                                <a data-toggle="collapse" href="#remarks" class="rfpheading">Approver Remarks
                                                    <i class="dot"></i>
                                                                                </a>
                                                                            </h4>
                                                                        </div>
                                                                        <div id="remarks" class="panel-collapse collapse" aria-expanded="">
                                                                            <div class="panel-body pd-0">
                                                                                <div class="TableOuterDiv table-responsive">
                                                                                    <table class="table CustomTable table-bordered">
                                                                                        <thead></thead>
                                                                                        <tbody class="horizontalscroll">
                                                                                            <tr>
                                                                                                <th>Created By</th>
                                                                                                <td>Test Requester</td>
                                                                                                <th>Creation Date</th>
                                                                                                <td>24/10/18 – 1</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Current Status</th>
                                                                                                <td>GRN / SRN</td>
                                                                                                <th>Invoice ID</th>
                                                                                                <td>POINV10242</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <td>PO_10039</td>
                                                                                                <th>PO Amount (Excl. Tax)</th>
                                                                                                <td>800</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Balance PO Amount</th>
                                                                                                <td>100</td>
                                                                                                <th>Payment Term Description</th>
                                                                                                <td>45 DAYS </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Valid From</th>
                                                                                                <td>14/05/18</td>
                                                                                                <th>Valid To</th>
                                                                                                <td>30/06/18</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Vendor Name</th>
                                                                                                <td>RENT WORK INDIA PVT.LTD.</td>
                                                                                                <th>Vendor Code</th>
                                                                                                <td>TestV </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor PAN</th>
                                                                                                <td>AAACA0120J</td>
                                                                                                <th>Vendor GSTN Status</th>
                                                                                                <td>Registered</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor GSTIN</th>
                                                                                                <td>09AAAA01201ZV</td>
                                                                                                <th>Place of Supply/Service</th>
                                                                                                <td>Gurugram</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Company Name</th>
                                                                                                <td>Travel Corporation</td>
                                                                                                <th>Item Sub Category</th>
                                                                                                <td>Printing Stationary</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>PO Accepted</th>
                                                                                                <td>YES</td>
                                                                                                <th></th>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Business Unit Name</th>
                                                                                                <td>CUSTOMER SERVICE SUPPORT</td>
                                                                                                <th>Business Unit Code</th>
                                                                                                <td>css</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Department</th>
                                                                                                <td></td>
                                                                                                <th>Early Payment Required</th>
                                                                                                <td>No</td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="panel-default mg-0">
                                                                        <div class="panel-heading v-response-panelbg">
                                                                            <h4 class="panel-title">
                                                                                <a data-toggle="collapse" href="#movement" class="rfpheading">Movement Details
                                                    <i class="dot"></i>
                                                                                </a>
                                                                            </h4>
                                                                        </div>
                                                                        <div id="movement1" class="panel-collapse collapse" aria-expanded="">
                                                                            <div class="panel-body pd-0">
                                                                                <div class="TableOuterDiv table-responsive">
                                                                                    <table class="table CustomTable table-bordered">
                                                                                        <thead></thead>
                                                                                        <tbody class="horizontalscroll">
                                                                                            <tr>
                                                                                                <th>Created By</th>
                                                                                                <td>Test Requester</td>
                                                                                                <th>Creation Date</th>
                                                                                                <td>24/10/18 – 1</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Current Status</th>
                                                                                                <td>GRN / SRN</td>
                                                                                                <th>Invoice ID</th>
                                                                                                <td>POINV10242</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <td>PO_10039</td>
                                                                                                <th>PO Amount (Excl. Tax)</th>
                                                                                                <td>800</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Balance PO Amount</th>
                                                                                                <td>100</td>
                                                                                                <th>Payment Term Description</th>
                                                                                                <td>45 DAYS </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Valid From</th>
                                                                                                <td>14/05/18</td>
                                                                                                <th>Valid To</th>
                                                                                                <td>30/06/18</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Vendor Name</th>
                                                                                                <td>RENT WORK INDIA PVT.LTD.</td>
                                                                                                <th>Vendor Code</th>
                                                                                                <td>TestV </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor PAN</th>
                                                                                                <td>AAACA0120J</td>
                                                                                                <th>Vendor GSTN Status</th>
                                                                                                <td>Registered</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor GSTIN</th>
                                                                                                <td>09AAAA01201ZV</td>
                                                                                                <th>Place of Supply/Service</th>
                                                                                                <td>Gurugram</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Company Name</th>
                                                                                                <td>Travel Corporation</td>
                                                                                                <th>Item Sub Category</th>
                                                                                                <td>Printing Stationary</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>PO Accepted</th>
                                                                                                <td>YES</td>
                                                                                                <th></th>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Business Unit Name</th>
                                                                                                <td>CUSTOMER SERVICE SUPPORT</td>
                                                                                                <th>Business Unit Code</th>
                                                                                                <td>css</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Department</th>
                                                                                                <td></td>
                                                                                                <th>Early Payment Required</th>
                                                                                                <td>No</td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>--%>

                                            <%--<div id="NotificationsTab2" class="tab-pane fade in">
                                                <div class="NotificationsBoxMain">
                                                    <div class="row">
                                                        <div class="databox">
                                                            <div class="vendor-bg">
                                                                <h4>Invoice Non PO - <small>[System Document ID: 2188806] LoggedIn User: [DH_Adm_HR]| Role :[Approver]</small> </h4>
                                                            </div>

                                                            <div class="table-responsive">
                                                                <div class="col-md-12 col-sm-12 col-xs-12 pd-0">
                                                                    <div class="panel-default mg-0">
                                                                        <div class="panel-heading v-response-panelbg">
                                                                            <h4 class="panel-title">
                                                                                <a data-toggle="collapse" href="#dummy" class="rfpheading" aria-expanded="true">PO & Vendor Details 
                                                        <i class="dot"></i>
                                                                                </a>
                                                                            </h4>
                                                                        </div>
                                                                        <div id="dummy" class="panel-collapse collapse in" aria-expanded="true" style="">
                                                                            <div class="panel-body pd-0">
                                                                                <div class="TableOuterDiv table-responsive">
                                                                                    <table class="table CustomTable">
                                                                                        <tbody class="horizontalscroll">
                                                                                            <tr>
                                                                                                <th>Created By</th>
                                                                                                <td>Test Requester</td>
                                                                                                <th>Creation Date</th>
                                                                                                <td>24/10/18 – 1</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Current Status</th>
                                                                                                <td>GRN / SRN</td>
                                                                                                <th>Invoice ID</th>
                                                                                                <td>POINV10242</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <td>PO_10039</td>
                                                                                                <th>PO Amount (Excl. Tax)</th>
                                                                                                <td>800</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Balance PO Amount</th>
                                                                                                <td>100</td>
                                                                                                <th>Payment Term Description</th>
                                                                                                <td>45 DAYS </td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Valid From</th>
                                                                                                <td>14/05/18</td>
                                                                                                <th>Valid To</th>
                                                                                                <td>30/06/18</td>
                                                                                            </tr>

                                                                                            <tr>
                                                                                                <th>Vendor Name</th>
                                                                                                <td>RENT WORK INDIA PVT.LTD.</td>
                                                                                                <th>Vendor Code</th>
                                                                                                <td>TestV </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor PAN</th>
                                                                                                <td>AAACA0120J</td>
                                                                                                <th>Vendor GSTN Status</th>
                                                                                                <td>Registered</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Vendor GSTIN</th>
                                                                                                <td>09AAAA01201ZV</td>
                                                                                                <th>Place of Supply/Service</th>
                                                                                                <td>Gurugram</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Company Name</th>
                                                                                                <td>Travel Corporation</td>
                                                                                                <th>Item Sub Category</th>
                                                                                                <td>Printing Stationary</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>PO Accepted</th>
                                                                                                <td>YES</td>
                                                                                                <th></th>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Business Unit Name</th>
                                                                                                <td>CUSTOMER SERVICE SUPPORT</td>
                                                                                                <th>Business Unit Code</th>
                                                                                                <td>css</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th>Department</th>
                                                                                                <td></td>
                                                                                                <th>Early Payment Required</th>
                                                                                                <td>No</td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>--%>
                                        </div>
                                        <table style="width: 100%; cellspacing: 0px; cellpadding: 0px; empty-cells: show">
                                            <tr>
                                                <td style="width: 80%">
                                                    <div id="tabss" style="padding:0px !important;">
                                                        <div id="divCRM" runat="server">
                                                            <asp:UpdatePanel ID="UpCRMGrid" runat="server" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <%--<div>
                                                    <asp:Label ID="lblCRMHeading" runat="server" Text="CRM Details"></asp:Label>
                                                </div>--%>
                                                                    <div>
                                                                        <h3 style="background-color: #FAFAFA">
                                                                            <asp:Label ID="lblCRMHeader" runat="server"></asp:Label>
                                                                        </h3>
                                                                        <div style="width: auto; height: auto; border: 5px solid #bed5cd; overflow-x: scroll; overflow-y: scroll; white-space: nowrap;">
                                                                            <asp:GridView ID="GVCRM" DataKeyNames="tid" runat="server" AutoGenerateColumns="True"
                                                                                OnRowCreated="gv_OnRowCreated" Width="100%">
                                                                                <HeaderStyle CssClass="GridHeader" />
                                                                                <EmptyDataTemplate>
                                                                                    <div style="border: none; text-align: center; width: 100%; height: 100%;">
                                                                                        No activity found.
                                                                                    </div>
                                                                                </EmptyDataTemplate>
                                                                                <Columns>
                                                                                    <asp:TemplateField HeaderText="View Conversation">
                                                                                        <ItemTemplate>
                                                                                            <asp:Button ID="btnViewConver" runat="server" CssClass="btnNew" Text="View" OnClick="GetConversation" />
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="120px" HorizontalAlign="Center" />
                                                                                    </asp:TemplateField>
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </div>
                                                                    </div>
                                                                    <div>
                                                                        <table width="100%" cellspacing="0px" cellpadding="0px">
                                                                            <tr style="height: 40px;">
                                                                                <td colspan="3"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="width: 80%"></td>
                                                                                <td align="right">
                                                                                    <asp:Button ID="btnAddAction" runat="server" Text="Add Action" CssClass="btnNew" />
                                                                                </td>
                                                                                <td align="right">
                                                                                    <asp:Button ID="btnSendMail" runat="server" Visible="false" Text="Send Mail" CssClass="btnNew" />
                                                                                </td>
                                                                                <%--<td align="right">
                                                                <asp:Button ID="btnSendMessage" Visible="false" runat="server" Text="Send Message" CssClass="btnNew" />
                                                            </td>--%>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <div id="staticContent" runat="server">
                                        </div>
                                        <div id="t1" runat="server" class="page-content fulltab">
                                            <!-- BEGIN PAGE CONTENT-->
                                        </div>
                                        <!-- BEGIN fixed tab history-->
                                        <div id="t2" runat="server" class="portlet box blue fulltab" style="display: none; margin-bottom: 30px;">
                                            <div class="portlet-title">
                                                <div class="caption">
                                                    History Detail
                                                </div>
                                            </div>
                                            <div class="portlet-body" id="divHistory" runat="server">
                                            </div>
                                        </div>
                                        <div class="portlet box blue">
                                            <div class="portlet-title">
                                                <div class="caption">
                                                    Movement Detail 
                                                </div>
                                                <div class="tools"><a href="javascript:;" data-toggle="collapse" data-target="#movement" class="expand"></a></div>

                                            </div>
                                            <div class="portlet-body collapse" id="movement" runat="server">
                                            </div>
                                        </div>
                                        <div class="portlet box blue">
                                            <div class="portlet-title">
                                                <div class="caption">
                                                    Future Movements
                                                </div>
                                                <div class="tools"><a href="javascript:;" data-toggle="collapse" data-target="#futureMovement" class="expand"></a></div>
                                            </div>
                                            <div class="portlet-body collapse" id="futureMovement" runat="server">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-5 col-sm-5 col-xs-12 pd-left">
                            <div class="PanelContentBox1">
                                <div class="panel panel-default panel-amend">
                                    <div class="panel-heading" style="background-color:#428bca">
                                        <h5 class="H5">Attachment <%-- <asp:ImageButton ID="btnSearch" CssClass=" pull-right" runat="server" Width="20px" Height="20px" ToolTip="Print"
                                         ImageUrl="~/Images/imagesPrint.jpg" OnClientClick="printIframe(ifreadPDF);" Visible="false"/>
                                            &nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:ImageButton ID="ImageButton2" CssClass="pull-right" runat="server" Width="20px" Height="20px" ToolTip="Download"
                                                ImageUrl="~/Images/dwnldp.png" style="margin-top: 4px;" OnClick="ImageButton2_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                                        </h5>
                                    </div>

                                    <div class="clear20"></div>
                                    <div class="panel-body">
                                        <div class="large"></div>
                                        <iframe id="ifreadPDF" runat="server" class="small" width="100%" height="800px;"></iframe>
                                        <%--<img id="ifreadPDF" src="images/attachment.jpg" class="img-responsive" runat="server" />--%>
                                    </div>
                                </div>
                            </div>
                            <div class="clear20"></div>
                           
                        </div>
                    </div>
                </div>
                <div class="container-fluid container-fluid-amend">
                    <div class="row" id="footer">
                        <div class="col-md-12" id="footer-content">
                            <div class="ftr"> Copyright © Mynd Integrated Solutions Pvt. Ltd. All rights reserved.</div>
                        </div>
                        <div class="clear10"></div>
                    </div>
                </div>
            </section>
        </div>
        <asp:HiddenField ID="iframesrc" runat="server" />
        <div class="docloading" align="center" >
            Loading. Please wait.<br />
            <br />
            <img src="images/loader.gif" alt="" />
        </div>
    </form>
    <script type="text/javascript">
        function CalWithCalendar() {
            var dates = new Date();
            var currentDate = dates.getDate() + '/' + (dates.getMonth() + 1) + '/' + dates.getFullYear().toString().substring(2, 4);
            var dueDate = $("#txtDueDate").val();
            var paymentDate = $("#txtPaymentDate").val();
            if ($.datepicker.parseDate('dd/mm/yy', paymentDate) < $.datepicker.parseDate('dd/mm/yy', currentDate)) {
                $("#txtPaymentDate").val('');
                alert('Payment date should be future date');
            }
            var NodaysDiff = Math.round(($.datepicker.parseDate('dd/mm/yy', dueDate) - $.datepicker.parseDate('dd/mm/yy', paymentDate)) / (1000 * 60 * 60 * 24));
            if ($.datepicker.parseDate('dd/mm/yy', paymentDate) > $.datepicker.parseDate('dd/mm/yy', dueDate)) {
                $("#txtPaymentDate").val('');
                alert('Payment Date (' + paymentDate + ') is greater than Due Date (' + dueDate + ')');
            }
            if (NodaysDiff < 12) {
                if ($("#ddlDiscountingType").val() == "M1" || $("#ddlDiscountingType").val() == "M2") {
                    $("#txtPaymentDate").val('');
                    alert('Payment Date should be greater than 11 days to process in M1');
                }
            }
        }

        function netpaidamount() {
            debugger;
            var roi = Number($("#txtROI").val());
            var nod = Number($("#txtNOD").val());
            var amount = Number($("#txtPaidAmount").val());
            var netpaidAmount = 0;
            var marginmoney = Number($("#txtMarginMoney").val());
            if (marginmoney == "") {
                $("#txtMarginMoney").val('0')
                marginmoney = 0;
                netpaidAmount = ((amount) - (amount * roi / 100));
            }
            else {
                //(netpaidAmount) - (netpaidAmount * marginmoney / 100)
                var partialAmount = Number(amount - (amount * marginmoney / 100));
                netpaidAmount = (partialAmount - (partialAmount * roi / 100));
            }
            $("#txtnetPaidAmount").val(netpaidAmount.toFixed(2));
        }
        function discountratelist() {
            if ($("#ddlDiscounting").val() == "SELECT") {
                $("#txtROI").val('0');
                $("#txtNOD").val('0');
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "DocDetail.aspx/DiscountRateList",
                    data: '{ratecardID: "' + $("#ddlDiscounting").val() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $("#txtROI").val(response.d["ROI"]);
                        $("#txtNOD").val(response.d["Days"]);
                        var roi = Number($("#txtROI").val());
                        var nod = Number($("#txtNOD").val());
                        var amount = Number($("#txtPaidAmount").val());
                        var netpaidAmount = ((amount) - (amount * roi / 100));
                        var marginmoney = Number($("#txtMarginMoney").val());
                        if (marginmoney == "") {
                            $("#txtMarginMoney").val('0')
                            marginmoney = 0;
                            netpaidAmount = ((netpaidAmount) - (netpaidAmount * marginmoney / 100));
                        }
                        else {
                            netpaidAmount = ((netpaidAmount) - (netpaidAmount * marginmoney / 100));
                        }
                        $("#txtnetPaidAmount").val(netpaidAmount.toFixed(2));
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
        }
        function ValidateDate() {
            var dueDate = $("#txtDueDate").val();
            var paymentDate = $("#txtPaymentDate").val();
            var nod = Number($("#txtNOD").val());
            var NodaysDiff = Math.round(($.datepicker.parseDate('dd/mm/yy', dueDate) - $.datepicker.parseDate('dd/mm/yy', paymentDate)) / (1000 * 60 * 60 * 24));
            if (nod < NodaysDiff) {
                alert('Payment Date is not falling in Selected Discount Slab!');
                return false;
            }
            else {
                return true;
            }
        }

        function AcceptDiscounting(type) {
            var discountingtypeData = $("#ddlDiscountingType").val();
            var discountingratelistData = $("#ddlDiscounting").val();
            var roiData = $("#txtROI").val();
            var daysData = $("#txtNOD").val();
            var marginmoneyData = $("#txtMarginMoney").val();
            var netpaidamountData = $("#txtnetPaidAmount").val();
            var invoiceAmount = $("#txtPaidAmount").val();
            var varPaymentDate = $("#txtPaymentDate").val();
            var varDueDate = $("#txtDueDate").val();
            if (type == "ACCEPT") {
                if (discountingtypeData == "SELECT") {
                    alert("Please select Discount Type");
                    $("#ddlDiscountingType").focus();
                    return;
                }
                else {
                    if (discountingratelistData == "SELECT") {
                        $("#ddlDiscounting").focus();
                        alert("Please select One of Slabs from List");
                        return;
                    }
                    if (marginmoneyData == "") {
                        $("#txtMarginMoney").focus();
                        alert("Please enter margin money amount");
                        return;
                    }
                    if (varPaymentDate == "") {
                        $("#txtPaymentDate").focus();
                        alert("Please enter payment data from calendar");
                        return;
                    }
                    if (varDueDate == "") {
                        $("#txtDueDate").focus();
                        alert("Please contact Admin due date is invalid or blank");
                        return;
                    }

                    //'Add condition for check validation on date'
                    if (!ValidateDate()) {
                        return;
                    };

                }

                //if (discountingtypeData == "SELF")

                $.ajax({
                    type: "POST",
                    url: "NewDocDetail.aspx/SaveDiscounting",
                    data: '{type: "' + type + '",ratecardid:"' + discountingratelistData + '",marginmoney:"' + marginmoneyData + '",netPaidAmount:"' + netpaidamountData + '",discountingtype:"' + discountingtypeData + '",docid:"' + $("#hdnDocID").val() + '",paymentDate:"' + varPaymentDate + '",dueDate:"' + varDueDate + '",partialPaidAmount:"' + invoiceAmount + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d["resCode"] == "200") {
                            $("#btnCloseDiscounting").click();
                            alert(response.d["resString"]);
                            window.location.reload();
                        }
                        else {
                            alert(response.d["resString"]);
                        }

                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });

            }
            else {

                //if (DeleteConfirm()) {

                $.ajax({
                    type: "POST",
                    url: "NewDocDetail.aspx/SaveDiscounting",
                    data: '{type: "' + type + '",ratecardid:"' + discountingratelistData + '",marginmoney:"' + marginmoneyData + '",netPaidAmount:"' + netpaidamountData + '",discountingtype:"' + discountingtypeData + '",docid:"' + $("#hdnDocID").val() + '",paymentDate:"' + varPaymentDate + '",dueDate:"' + varDueDate + '",partialPaidAmount:"' + invoiceAmount + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d["resCode"] == "200") {
                            $("#btnCloseDiscounting").click();
                            alert(response.d["resString"]);
                            window.location.reload();
                        }
                        else {
                            alert(response.d["resString"]);
                        }

                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
                //}
            }
        }

        function discounttype() {
            if ($("#ddlDiscountingType").val() == "SELF") {
                $(".self").css('visibility', 'visible');

                //$(".self").show();
            }
            else {
                //$(".self").css('visibility', 'hidden');
                $("#ddlDiscounting").prop('selectedIndex', 0);
                //$("#txtROI").val('0');
                //$("#txtNOD").val('0');
                //$("#txtMarginMoney").val('0');
                //$("#txtnetPaidAmount").val('0');

            }
        }
    </script>

    <%--07- Jan-2017 added--%>

    <%--07- Jan-2017 added--%>


    <%--07- Jan-2017 added--%>
    <script type="text/javascript">
        function ace_itemSelected(sender, e) {

            var test = e.get_value();
            var g1 = sender.get_element()
            var t1 = g1.id;
            var hdnID = t1.replace("fld", "HDN");
            document.getElementById(hdnID).value = test;
            //            alert(document.getElementById(hdnID).value);
        }

        function onDataShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 1000001;
            // sender._popupBehavior._element.style.left = "54px"; //set positions according to your requriment.
            // sender._popupBehavior._element.style.top = "50px"; //set top postion accorind to you requirement.

            //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
        }

        function ValidateSMS() {
            //txtMeaasge txtMob
            var Message = "", MobNum = "";
            Message = $("#txtMeaasge").val();
            MobNum = $("#txtMob").val();
            var obj = ""
            var Isvalid = true;
            var ErrorMessage = "Error(s) in your submission.\n-----------------------------------\n";
            if (Message.trim() == "") {
                ErrorMessage = ErrorMessage + "Message required.\n";
                Isvalid = false;
                obj = $("#txtMeaasge");
            }
            if (MobNum.trim() == "") {
                ErrorMessage = ErrorMessage + "Mobile number required.\n";
                Isvalid = false;
                if (obj != "") {
                    obj = $("#txtMob");
                }
            }
            else {
                if (MobNum.trim().length < 10) {
                    ErrorMessage = ErrorMessage + "A valid mobile number required.\n";
                    Isvalid = false;
                    if (obj != "") {
                        obj = $("#txtMob");
                    }
                }
            }
            if (Isvalid == false) {
                obj.focus();
                alert(ErrorMessage);
            }
            return Isvalid;
        }
        function numbersonly(e) {
            var unicode = e.charCode ? e.charCode : e.keyCode
            if (unicode != 8) { //if the key isn't the backspace key (which we should allow)
                if (unicode < 48 || unicode > 57) //if not a number
                    if (unicode != 46)
                        return false //disable key press
            }
        }
        function ValidateCRMMail() {
            //txtBody txtCC txtToMail txtSMTP txtPassword ddlFromMail
            var MailBoby = "", CC = "", ToMail = "", SMTP = "", Password = "", FromMail = "";
            MailBoby = $("#txtBody").val();
            ToMail = $("#txtToMail").val();
            Password = $("#txtPassword").val();
            FromMail = $("#ddlFromMail").val();
            SMTP = $("#txtSMTP").val();
            var obj = ""
            var Isvalid = true;
            var ErrorMessage = "Error(s) in your submission.\n-----------------------------------\n";
            if (FromMail != "0") {
                if (Password.trim() == "") {
                    Isvalid = false;
                    ErrorMessage = ErrorMessage + "Password required\n";
                    if (obj != "") {
                        obj = $("#txtPassword");
                    }
                }
                if (Password.trim() == "") {
                    ErrorMessage = ErrorMessage + "SMTP Serve required\n";
                    Isvalid = false;
                    if (obj != "") {
                        obj = $("#txtSMTP");
                    }
                }
            }
            if (MailBoby.trim() == "") {
                ErrorMessage = ErrorMessage + "Mail Body required\n";
                Isvalid = false;
                if (obj != "") {
                    obj = $("#txtBody");
                }
            }
            if (ToMail.trim() == "") {
                ErrorMessage = ErrorMessage + "ToMail Body required\n";
                Isvalid = false;
                if (obj != "") {
                    obj = $("#txtToMail");
                }
            }
            if (Isvalid == false) {
                alert(ErrorMessage);
                // obj.focus();
            }
            return Isvalid;
        }
        function TomailChange(fromMail) {
            //txtPassword txtSMTP
            if (fromMail == "0") {
                $("#txtPassword").attr("readonly", true).val("");
                $("#txtSMTP").attr("readonly", true).val("");
            }
            else {
                $("#txtPassword").attr("readonly", false);
                $("#txtSMTP").attr("readonly", false);
            }
        }

        var selTab;
        $(function () {
            var tabs = $("#tabs").tabs({
                show: function () {

                    //get the selected tab index  
                    selTab = $('#tabs').tabs('option', 'selected');

                }
            });

        });;
        $(function () {
            $(".btnDyn").button()
        });

        function bindDateTime() {
            //alert("This is partial page load");
            $("input[data-field='date'], input[data-field='time'], input[data-field='datetime']").each(function () {
                $(this).unbind("click");
                //                 alert("This is ID of my concern element." + $(this).attr("id"));
            });
            $("#dtBox").DateTimePicker();
            //$("#dtBoxC").DateTimePicker();
            //            alert("hv done with ashignment");
        }
        //function pageLoad(sender, args) {
        //    // alert("Hi This is page load");
        //    if (args.get_isPartialLoad()) {
        //        $("#tabs").tabs({
        //            show: function () {
        //                //get the selected tab index on partial postback  
        //                selTab = $('#tabs').tabs('option', 'selected');
        //            }, selected: selTab
        //        });

        //        $(function () {
        //            $(".btnDyn").button()
        //        });
        //    }

        //};


        $(document).ready(function () {
            //$("#btnDocApprove").click();
            $("#loading-div-background").css({ opacity: "0.8", display: "none" });
            //$("#loading-div-background").css({  });

            $("#li1").css({ backgroundColor: "#5bc0de", color: "#fff"});
            $("#li2").css({ backgroundColor: "#428bca", color: "#fff" });

        });

        $("#li2").click(function () {
            $("#li2").css({ backgroundColor: "#5bc0de", color: "#fff" });
            $("#li1").css({ backgroundColor: "#428bca;", color: "#fff" });

        })
        $("#li1").click(function () {
            $("#li1").css({ backgroundColor: "#5bc0de", color: "#fff" });
            $("#li2").css({ backgroundColor: "#428bca;", color: "#fff" });

        })
    </script>


    <%--  <script type="text/javascript">
        $(function () {
            $(".btn-primary").click(function () {
                $('.btn-primary').removeClass('btn-primary');
                $(this).addClass('btn-Basic');
               // return false;
            });
        });
    </script>--%>


    <script type='text/javascript'>
        function pageLoad(sender, args) { // this gets fired when the UpdatePanel.Update() completes
            $("select").not('.invisible').select2({
            });
            var iFramescr = ""; debugger
            // src = $(this).attr("classvalue");
            iFramescr = $("#iframesrc").val();
            //iFramescr=$(this).attr("classvalue");
            if (iFramescr != "") {
                $('#ifreadPDF').attr('src', iFramescr);
            }
            if (args.get_isPartialLoad()) {
                var iFramescr = ""; debugger
                // src = $(this).attr("classvalue");
                iFramescr = $("#iframesrc").val();
                //iFramescr=$(this).attr("classvalue");
                if (iFramescr != "") {
                    $('#ifreadPDF').attr('src', iFramescr);
                }

                $("div#pnlApprove div.form").attr('style', 'min-height: 0px !important');
                $("#tabs").tabs({
                    show: function () {
                        //get the selected tab index on partial postback  
                        selTab = $('#tabs').tabs('option', 'selected');
                    }, selected: selTab
                });

                $(function () {
                    $(".btnDyn").button()
                });

                $(function () {
                    $('.btn-primary').removeClass('btn-primary');
                    $(this).addClass('btn-Basic');
                });
                //Uncomment after user just becuase when click on submit button div is hide why so
                //var objInv = $(".invisible");
                //objInv.each(function () {
                //    var tr = $(this).parent().parent();
                //    var inv = $(".invisible", tr);

                //    //var img= inv.find("img")
                //    var invImg = $(inv).parent("td");
                //    var alltd = tr.find("td");
                //    if (invImg.length == alltd.length) {
                //        tr.addClass("invisible");
                //        //img.addClass("invisible");
                //    }
                //    else {
                //        tr.removeClass("invisible");
                //    }
                //    //onjInv Each function Ende Here
                //});


            }

            //     ReBindMyStuff();
            //     window.alert('partial postback');

            //' for Hide all content'
            var isExpand = true; debugger
            $("#t1 .row-fluid").each(function () {
                $(this).find(".portlet-title .tools a").each(function () {
                    if (isExpand == true) {
                        $(this).removeClass("collapse"); $(this).addClass("expand");
                    }
                    else {
                        $(this).removeClass("collapse"); $(this).addClass("expand");
                        isExpand = false;
                    }
                });
                //    ' for Hide all content'
                $(this).find(".portlet-body").each(function () {
                    //Changed By Manvendra 19/06/
                    if (isExpand == true) {
                        $(this).css("display", "none");
                    }
                    else {
                        $(this).css("display", "none");
                        isExpand = false;
                    }
                });
                isExpand = false;
            });




            $(function () {
                $(".btnDyn").button()
            });
            $(document).ready(function () {
                $("#tabs").tabs();
            });
            $(function () {
                $("#tab").tabs();
            });
            $(function () {
                $("#tabss").tabs();
            });

        }
        //Wiriten By Ajeet Kumar: Dated :18-Nov-2015
        //It is used to invock method of parent page:
        window.onunload = function () {
            var win = window.opener;
            if (!win.closed) {
                win.childClose();
            }
        };


        function GetParameterValues(param) {
            var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < url.length; i++) {
                var urlparam = url[i].split('=');
                if (urlparam[0] == param) {
                    return urlparam[1];
                }
            }
        }


        function checkAll(GID) {
            //var obj = $("#ContentPlaceHolder1_gvData_ctl00").attr('checked');
            //alert(GID);
            //var GridIDCHK = "#ContentPlaceHolder1_GRD" + GID + "_ctl00";
            var GridIDCHK = "#GRD" + GID + "_ctl00";

            var obj = $(GridIDCHK);
            //  var GridID = "#ContentPlaceHolder1_GRD" + GID;
            var GridID = "#GRD" + GID;
            //alert("Hi I Am being fired");
            //alert("Object Is" + obj);
            var str = $(obj).prop('checked');
            //alert("Property IS " + str);
            // alert(str);
            if (str == true) {
                $(GridID).find(':checkbox').each(function () {
                    $(this).prop('checked', "checked");
                });
            }
            else {
                $(GridID).find(':checkbox').each(function () {
                    $(this).removeAttr('checked');
                });
            }
        }




    </script>

    <%--for tab (doc detail fields panel) show and hide functionality --%>
    <script type="text/javascript">
        $(document).ready(function () {
            PopulateTabObjects();
        });
        var tabobjects = "";
        var arrtabobjects = [];
        function beforeAsyncPostBack() {
            PopulateTabObjects();
        }
        function beforeAsyncPostBack() {
            PopulateTabObjects();
        }

        function afterAsyncPostBack() {
            for (var i = 0; i < arrtabobjects.length; i++) {
                var aobj = $("#" + arrtabobjects[i].element);
                //is(b)
                if (aobj.length > 0) {
                    var parent = $(aobj).parent().parent().next();
                    if (arrtabobjects[i].cssclass == "collapse") {
                        aobj.addClass("collapse");
                        $(parent).show();
                    }
                    else {
                        aobj.addClass("expand");
                        $(parent).hide();
                    }
                }
            }
        }

        Sys.Application.add_init(appl_init);

        function appl_init() {
            var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
            pgRegMgr.add_beginRequest(BeginHandler);
            pgRegMgr.add_endRequest(EndHandler);
        }


        function BeginHandler() {
            beforeAsyncPostBack();
        }

        function EndHandler() {
            afterAsyncPostBack();
        }

        function PopulateTabObjects() {
            $("a.expand").each(function (index) {
                tabobjects = { cssclass: "expand", element: $(this).attr("id") };
                arrtabobjects.push(tabobjects);
            });
            //collapse
            $("a.collapse").each(function (index) {
                tabobjects = { cssclass: "collapse", element: $(this).attr("id") };
                arrtabobjects.push(tabobjects);
            });
        }
    </script>

    <script type="text/javascript">
        function OpenWindow(url) {
            var new_window = window.open(url, "new", "scrollbars=yes,resizable=yes,width=800,height=480");
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

        function DeleteConfirm() {
            var x;
            if (confirm("Are you sure you want to delete?") == true) {
                return true;
            } else {
                return false;
            }

        }


    </script>
    <script>
        $(function () {
            $("#tabs").tabs();
        });
        $(function () {
            $("#tabss").tabs();
        });

        $(function () {
            $("#tab").tabs();
        });
    </script>
    <style type="text/css">
        .modal {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }

        .docloading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 99999;
        }
    </style>
    <script type="text/javascript">
        function hideprogressbar() {
            $(".docloading").hide();
        }
        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".docloading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 200);
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            //alert('yes');		
            //$(".t").unbind().bind("click", function () {		
            //    alert('yes full tab');		
            //    $(".fulltab").each(function () {		
            //        alert('yes full tab l');		
            //        //$(this).css("background-color", "#3a87ad");		
            //        $(this).hide();		
            //    });		
            //    var id = $(this).attr("id");		
            //    alert(id);		
            //    $("#t" + id.substring(2, id.length)).show();		
            //    //$("#li" + id.substring(2, id.length)).css("background-color", "#23769c");		
            //});		
        });
    </script>
    <%--For child Item--%>
    <script type="text/javascript">
        $(function () {
            $(".c").bind("click", function () {
                var i = 0;
                $(".Childfulltab").each(function () {
                    $(this).hide();
                });
                var id = $(this).attr("id");
                $("#C" + id.substring(2, id.length)).show();
            });
        });
        function setCSS(obj) {
            $(".fulltab").each(function () {
                $(this).hide();
            });
            var id = $(obj).attr("id");
            $("#t" + id.substring(2, id.length)).show();
        }
    </script>
    <%--Add content for Next level design --%>
    <script src="assets/scripts/app.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // initiate layout and plugins		
            App.init();
            UIGeneral.init();
           
                

        });
    </script>
    <script type="text/javascript">
        var src = "";
        $(".readpdf").click(function () {
            //alert("ready to read pdf" + $(this).attr("classvalue"));
            src = $(this).attr("classvalue");
            $('#ifreadPDF').attr('src', src);
            $("#iframesrc").val(src);
        });

    </script>

    <script type="text/javascript">
        $('.scrollbox3').enscroll({
            showOnHover: false,
            verticalTrackClass: 'track3',
            verticalHandleClass: 'handle3'
        });
    </script>

</body>

</html>

