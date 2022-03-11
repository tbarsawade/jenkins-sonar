<%@ page language="VB" autoeventwireup="false" inherits="MailApproval, App_Web_1rjiof5j" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Document Detail</title>
    <link href="css/style.css" rel="Stylesheet" type="text/css" />
    <link href="css/ui-lightness/jquery-ui-1.10.2.custom.css" type="text/css" rel="stylesheet" />
    <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/jquery.slidePanel.min.js"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <link href="css/DateTimePicker.css" rel="stylesheet" />
    <script src="js/DateTimePicker.js"></script>
    <script src="js/Utils.js"></script>

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
        function pageLoad(sender, args) {
            // alert("Hi This is page load");
            if (args.get_isPartialLoad()) {
                $("#tabs").tabs({
                    show: function () {
                        //get the selected tab index on partial postback  
                        selTab = $('#tabs').tabs('option', 'selected');
                    }, selected: selTab
                });

                $(function () {
                    $(".btnDyn").button()
                });
            }

        };


        $(document).ready(function () {
            $("#loading-div-background").css({ opacity: "0.8", display: "none" });
            //$("#loading-div-background").css({  });
        });
    </script>
    <script type='text/javascript'>
        function pageLoad() { // this gets fired when the UpdatePanel.Update() completes
            //     ReBindMyStuff();
            //     window.alert('partial postback');
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
    <style type="text/css" media="print">
        body, html, #wrapper {
            font-size: 8px;
            font-family: @Adobe Fan Heiti Std B;
            width: 100%;
            border-collapse: collapse;
            border: 0px;
            border-spacing: 0px;
            float: left;
            height: 200%;
            margin-bottom: 0px;
            margin-left: 0px;
            margin-right: 0px;
            margin-top: 0px;
        }

        .err {
            color: red;
        }
    </style>

    <script type="text/javascript">
        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }
        $(document).ready(function () {
            //    $('#panel1').slidePanel({
            //	triggerName: '#trigger1',
            //	position: 'fixed',
            //	triggerTopPos: '65px',
            //	panelTopPos: '95px'
            //});

            //$('.detail').each(function() {
            //    var $link = $(this);
            //    var $dialog = $('<div></div>')
            //		.load($link.attr('href'))
            //		.dialog({
            //		    autoOpen: false,
            //		    title: 'Document Detail',
            //		    width: 700,
            //		    height: 550,
            //		    modal: true
            //		});

            //    $link.click(function() {
            //        $dialog.dialog('open');
            //        return false;
            //    });
            //});
        });

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


</head>

<body>
    <form id="form1" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="tscript" runat="server"></ajaxToolkit:ToolkitScriptManager>
              <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrint1" />
                <asp:PostBackTrigger ControlID="btnPrint2" />
                <asp:PostBackTrigger ControlID="btnPrint3" />
                <asp:PostBackTrigger ControlID="btnPrintWord" />
            </Triggers>
            <ContentTemplate>

                <div class="box" style="text-align: center; width: 100%;">

                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                please wait...
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>


                    <table cellspacing="2px" cellpadding="2px" border="1" style="empty-cells: show; width: 100%">
                        <tr>
                            <td style="text-align: left; width: 70%;">
                                <asp:Label ID="lblLogo" runat="server"></asp:Label>
                                <asp:Button ID="btnDocApprove" CssClass="ststusButton" ToolTip="Approve" OnClick="ShowApprove" runat="server" />
                                <asp:Button ID="btnDocReject" CssClass="ststusButton" ToolTip="Reconsider" OnClick="ShowReconsider" runat="server" />
                                <asp:Button ID="btnRejectDoc" CssClass="ststusButton" ToolTip="Reject" OnClick="ShowPermanentReject" runat="server" />
                                <%--above button code should be modified later (btnRejectDoc)--%>
                                <asp:Button ID="btnDocEdit" CssClass="ststusButton" ToolTip="Edit" runat="server" />
                                <asp:Button ID="btnAmendment" CssClass="ststusButton" ToolTip="Amendment" runat="server" Visible="false" />
                                <asp:Button ID="btnRecall" CssClass="ststusButton" ToolTip="Recall" runat="server" Visible="false" />
                                <asp:Button ID="btnCancel" CssClass="ststusButton" ToolTip="Cancel" runat="server" Visible="false" />
                                <asp:Button ID="btnSplit" CssClass="ststusButton" ToolTip="Split/Copy" runat="server" Visible="false" />
                                <asp:Button ID="btnCopy" CssClass="ststusButton" ToolTip="Copy" runat="server" Visible="false" />
                                <%-- <asp:Button ID="btnclosewindow" CssClass="ststusButton" Text="Close Window" OnClientClick="javaScript:window.close(); return false;" Width="120px" ToolTip ="Close Window" runat="server" Visible="True"/>--%>
                                <%--<asp:Button ID="Button3" CssClass="ststusButton" ToolTip ="Cancel" runat="server" CloseDocDtl Visible="false"/>--%>
                                <asp:Label ID="lblMAILMSG" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Medium" Font-Underline="true"></asp:Label>
                            </td>

                            <td style="text-align: right; width: 30%;">

                                <asp:ImageButton ID="btnPrint1" CssClass="docbutton" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/dprint.png" />
                                <asp:ImageButton ID="btnPrint2" CssClass="docbutton" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/dprint.png" />
                                <asp:ImageButton ID="btnPrint3" CssClass="docbutton" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/dprint.png" />
                                <asp:ImageButton ID="btnPrintWord" CssClass="docbutton" OnClick="PrintWord" ToolTip="Print" Width="50px" Visible="false" Height="36px" runat="server" ImageUrl="~/images/msWord.jpg" />
                                <asp:ImageButton ID="btnCloseWindow" CssClass="docbutton" OnClientClick="javaScript:window.close(); return false;" ToolTip="Close" runat="server" ImageUrl="~/images/CloseDocDtl.png" />

                            </td>
                        </tr>
                    </table>


                    <table style="width: 100%; cellspacing: 0px; cellpadding: 0px; empty-cells: show">
                        <tr>
                            <td style="width: 80%">
                                <div id="tabss">
                                    <ul>
                                        <li>
                                            <asp:Label ID="lblpending" runat="server"><a href="#tabPending">CURRENT</a></asp:Label>
                                        </li>
                                        <li>
                                            <asp:Label ID="lblaction" runat="server">
                                                <%--<a href="#tabMy">HISTORY</a>--%>
                                                <asp:Label ID="LBLHistroy" runat="server"></asp:Label>
                                            </asp:Label>
                                        </li>
                                        <li>
                                            <asp:Label ID="Label5" runat="server">
                                                <asp:Label ID="lblCRM" runat="server"></asp:Label>
                                            </asp:Label>
                                        </li>
                                    </ul>

                                    <div id="tabPending" style="min-height: 300px;">
                                        <asp:UpdatePanel ID="updPnlGrid" runat="server">
                                            <ContentTemplate>
                                                <div>
                                                    <%--<asp:Panel ID="Panel1" runat="server" Width="800px" ScrollBars="auto" >--%>
                                                    <asp:Label ID="lblDetail" runat="server" Text="Folder Name"></asp:Label>
                                                    <%--</asp:Panel> --%>
                                                </div>
                                                <div>
                                                    <h3 style="background-color: #FAFAFA">Movement Detail</h3>
                                                    <asp:GridView ID="gvMovDetail" runat="server" AutoGenerateColumns="False" DataKeyNames="tid" Width="100%"
                                                        PageSize="20">
                                                        <HeaderStyle
                                                            CssClass="GridHeader" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="S.No">
                                                                <ItemTemplate>
                                                                    <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="UserName" HeaderText="User Name">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="STATUS" HeaderText="STAGE">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="fdate" HeaderText="In Date">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="tdate" HeaderText="Out Date">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="ptat" HeaderText="SLA">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="atat" HeaderText="A. SLA">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="remarks" HeaderText="Remarks">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                                    </asp:GridView>


                                                </div>
                                                <div>
                                                    <h3 style="background-color: #FAFAFA">Future Movements</h3>
                                                    <asp:GridView ID="gvFutureMov" runat="server" AutoGenerateColumns="False" DataKeyNames="tid" Width="100%"
                                                        PageSize="20">
                                                        <HeaderStyle
                                                            CssClass="GridHeader" HorizontalAlign="Left" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="S.No">
                                                                <ItemTemplate>
                                                                    <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="username" HeaderText="User/Role to Action">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="STATUS" HeaderText="STAGE">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="SLA" HeaderText="SLA">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                                    </asp:GridView>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div id="tabMy" runat="server" style="min-height: 300px;">
                                        <asp:UpdatePanel ID="updPNLMyUpload" runat="server" style="overflow: auto">
                                            <ContentTemplate>
                                                <br />
                                                <asp:Panel ID="pnlhis" runat="server" Width="800px">
                                                    <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" DataKeyNames="tid" Width="100%" PageSize="20">
                                                        <HeaderStyle CssClass="GridHeader" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="S.No">
                                                                <ItemTemplate>
                                                                    <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
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
                                                        <asp:GridView ID="GVCRM" DataKeyNames="tid" runat="server" AutoGenerateColumns="True" OnRowCreated="gv_OnRowCreated" Width="100%">
                                                            <HeaderStyle CssClass="GridHeader" />
                                                            <EmptyDataTemplate>
                                                                <div style="border: none; text-align: center; width: 100%; height: 100%;">No activity found.</div>
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
                        </tr>

                    </table>
                    <asp:Button ID="btnShowPopUpApprove" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnApprove_ModalPopupExtender" runat="server"
                        TargetControlID="btnShowPopUpApprove" PopupControlID="pnlPopupApprove"
                        CancelControlID="btnCloseApprove" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <div>
                        <asp:Panel ID="pnlPopupApprove" runat="server" Width="700px" Style="Display: none; height: 400px; overflow: scroll" BackColor="white">

                            <div class="box">
                                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                    <tr>
                                        <td>
                                            <h3>
                                                <asp:Label ID="lblAppdoc" runat="server"></asp:Label>
                                                Document</h3>
                                        </td>
                                        <td style="width: 20px">
                                            <asp:ImageButton ID="btnCloseApprove" ImageUrl="images/close.png" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: left; width: 100%">
                                            <asp:UpdatePanel ID="updatePanelApprove" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Label ID="lblTabApprove" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                    <asp:Panel ID="pnlApprove" Width="100%" runat="server">
                                                    </asp:Panel>
                                                    <div style="clear: both;"></div>

                                                    <div style="width: 100%; text-align: right;">
                                                        <asp:Label ID="lblMsgRule2" runat="server" Text=""></asp:Label>
                                                        <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                                            CssClass="ststusButton" Font-Bold="True"
                                                            Font-Size="X-Small" Width="100px" />
                                                    </div>

                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                            </div>


                        </asp:Panel>

                    </div>


                    <asp:Button ID="btnShowPopupReject" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnReject_ModalPopupExtender" runat="server"
                        TargetControlID="btnShowPopupReject" PopupControlID="pnlPopupReject"
                        CancelControlID="btnCloseReject" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <asp:Panel ID="pnlPopupReject" runat="server" Width="700px" Height="400px" Style="Display: none; overflow: auto" BackColor="aqua">

                        <div class="box">
                            <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                <tr>
                                    <td>
                                        <h3>
                                            <asp:Label ID="lblrejdoc" runat="server"></asp:Label>
                                            Document</h3>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:ImageButton ID="btnCloseReject"
                                            ImageUrl="images/close.png" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: left">
                                        <asp:UpdatePanel ID="updatePanelReject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>

                                                <asp:Label ID="lblTabRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                <asp:Panel ID="pnlFieldsRej" Width="100%" runat="server">
                                                </asp:Panel>


                                                <div style="width: 100%; text-align: right">
                                                    <asp:Label ID="lblRuleMsg3" runat="server" Text=""></asp:Label>
                                                    <asp:Button ID="btnReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" CssClass="ststusButton" Font-Bold="True"
                                                        Font-Size="X-Small" OnClick="editBtnReject" Text="Reject" Width="100px" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>



                    <asp:Button ID="btnPerRejectpopup" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnPerRejectModalpopup" runat="server"
                        TargetControlID="btnPerRejectpopup" PopupControlID="pnlPerReject"
                        CancelControlID="btnClosePerReject" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <asp:Panel ID="pnlPerReject" runat="server" Width="700px" Height="400px" Style="Display: none; overflow: auto" BackColor="aqua">
                        <div class="box">
                            <table cellspacing="2px" cellpadding="2px" width="100%">
                                <tr>
                                    <td>
                                        <h3>Permanent
                                            <asp:Label ID="lblPREJ" runat="server"></asp:Label>
                                            Document</h3>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:ImageButton ID="btnClosePerReject"
                                            ImageUrl="images/close.png" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: left">
                                        <asp:UpdatePanel ID="updPerReject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>

                                                <asp:Label ID="lblPerRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                <asp:Panel ID="PanelPerReject" Width="100%" runat="server">
                                                </asp:Panel>


                                                <div style="width: 100%; text-align: right">
                                                    <asp:Label ID="lblMsgRule1" runat="server" Text=""></asp:Label>
                                                    <asp:Button ID="btnPerReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" CssClass="ststusButton" Font-Bold="True"
                                                        Font-Size="X-Small" OnClick="editBtnPerReject" Text="Reject" Width="100px" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>

                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
        <%--HTML FOR CRM by Ajeet--%>
        <asp:Button ID="btnCRMH" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="MDLCRMAC" runat="server"
            TargetControlID="btnCRMH" PopupControlID="pnlCRMAction"
            CancelControlID="IMGCRMClose" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlCRMAction" runat="server" Style="Display: none; overflow: auto" BackColor="aqua">
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
                                        <asp:Button ID="btnCRMAction" runat="server" Text="Add" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClick="btnCRMApprove" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Button ID="btnPSendSMS" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="mdpPopUpSMS" runat="server"
            TargetControlID="btnPSendSMS" PopupControlID="pnlSendSmS" CancelControlID="imgSmsClose" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlSendSmS" runat="server" BorderColor="#666666" ForeColor="Black" Style="display: none;" BackColor="Silver">
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
                                            <td>Mobile No.<span style="color: red;">*</span></td>
                                            <td>
                                                <asp:TextBox ID="txtMob" onkeypress="return numbersonly(event);" runat="server" MaxLength="10" Width="200"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td>Message.<span style="color: red;">*</span></td>
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
        <asp:Button ID="hdnVConv" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="mdlCRMViewConver" runat="server"
            TargetControlID="hdnVConv" PopupControlID="pncCRMViewConver" CancelControlID="IMgCloseCRM" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pncCRMViewConver" runat="server" BorderColor="#666666" Width="600" ForeColor="Black" Style="display: none;" BackColor="Silver">
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
                                            <td width="15%"><b>Mobile No.</b></td>
                                            <td width="35%">
                                                <asp:Label runat="server" ID="lblMobNum"></asp:Label>
                                            </td>
                                            <td width="10%"><b>To Mail.</b>
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
        <asp:Button ID="hdnbtnsendMail" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="mdlSendMail" runat="server"
            TargetControlID="hdnbtnsendMail" PopupControlID="pnlSendMail" CancelControlID="imgClosebtnClose" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlSendMail" runat="server" Width="850" Height="550" BorderColor="#666666" Style="display: none;" ForeColor="Black" BackColor="Silver">
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
                                            <td width="10%">From Email.<span style="color: red;">*</span></td>
                                            <td width="40%">
                                                <asp:DropDownList runat="server" onchange="javascript:TomailChange(this.value);" Width="250" ID="ddlFromMail" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                            <td width="10%">Password.</td>
                                            <td width="50%">
                                                <asp:TextBox ID="txtPassword" TextMode="Password" Width="250" ReadOnly="true" CssClass="txtBox" runat="server"></asp:TextBox>
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
                                            <td>To.<span style="color: red;">*</span></td>
                                            <td>
                                                <asp:TextBox ID="txtToMail" placeholder="Enter emails seperated by ','" Width="250" runat="server" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td>CC.</td>
                                            <td>
                                                <asp:TextBox ID="txtCC" runat="server" placeholder="Enter emails seperated by ','" Width="250" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td>Bcc.</td>
                                            <td>
                                                <asp:TextBox ID="txtCRMBCC" placeholder="Enter emails seperated by ','" Width="250" runat="server" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td>Subject.</td>
                                            <td>
                                                <asp:TextBox ID="txtCRMSub" runat="server" placeholder="Enter subject" Width="250" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td>Body.<span style="color: red;">*</span></td>
                                            <td align="left" style="height: 250px;" colspan="3">
                                                <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="250px" Width="650" BackColor="White">
                                                    <asp:TextBox ID="txtBody" Visible="false" TextMode="MultiLine" Text=" " runat="server" Width="100%" Height="98%" BackColor="White">
                                                    </asp:TextBox>
                                                    <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" TargetControlID="txtbody" EnableSanitization="false"></asp:HtmlEditorExtender>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr style="height: 10px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="4">
                                                <asp:Button ID="btnSendmailHit" OnClick="btnSendmailHit_Click" OnClientClick="javascript:return ValidateCRMMail();" runat="server" Text="Send" CssClass="ststusButton" />
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
        <%--CRM Code END HERE--%>
        <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="ModalEditPopup" runat="server"
            TargetControlID="btnShowPopupEdit" PopupControlID="pnlEdit"
            CancelControlID="ImageButton1" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>



        <asp:Panel ID="pnlEdit" runat="server" Width="1000px" Height="400px" ScrollBars="auto" BackColor="aqua">


            <div class="box">

                <div id="loading-div-background" style="display: none; position: fixed; top: 0; left: 0; background: black; width: 100%; height: 100%; z-index: 1000;">
                    <div id="loading-div" class="ui-corner-all" style="width: 300px; height: 64px; background-color: #fff !important; text-align: center; position: absolute; left: 50%; top: 50%; margin-left: -150px; margin-top: -100px; z-index: 2000; opacity: 1.0; color: black;">
                        <img style="height: 30px; margin-top: 5px;" src="images/attch.gif" alt="Loading.." />
                        <h2 style="color: black; font-weight: normal;">Uploading. Please wait....</h2>
                    </div>
                </div>

                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;" border="1px">
                    <tr>
                        <td style="width: 940px">
                            <h3>
                                <asp:Label ID="lblstatus" runat="server" Text="Document Editing" Font-Bold="True"></asp:Label></h3>
                        </td>
                        <td style="width: 60px">
                            <asp:ImageButton ID="ImageButton1"
                                ImageUrl="images/close.png" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div>
                                        <asp:Label ID="lblDetail1" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                    <asp:Panel ID="pnlFields" runat="server" Width="1000px">
                                    </asp:Panel>
                                    <div id="dtBox" style="z-index: 9999999999;"></div>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="btnActEdit" runat="server" Text="Save"
                                            CssClass="btnNew" Font-Bold="True"
                                            Font-Size="X-Small" OnClick="EditRecord" Width="100px" />
                                    </div>
                                    <%--<asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                        <ProgressTemplate>
                                            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                                                <asp:Image ID="Image2" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                please wait...
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>--%>
                                </ContentTemplate>

                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <%--for document child item edit--%>

        <asp:Button ID="Btnchild" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server"
            TargetControlID="Btnchild" PopupControlID="pnlPopupchild"
            CancelControlID="btnClose" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>

        <asp:Panel ID="pnlPopupchild" runat="server" Width="800px" Height="300px" ScrollBars="Auto" BackColor="aqua">
            <div class="box">
                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnClose"
                                ImageUrl="images/close.png" runat="server" /></td>
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
                                        <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>


        <asp:Button ID="btnReCallpopUp" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="MP_Recall" runat="server"
            TargetControlID="btnReCallpopUp" PopupControlID="pnlRecall"
            CancelControlID="btnCloseRecall" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>

        <asp:Panel ID="pnlRecall" runat="server" Width="800px" Height="300px" ScrollBars="Auto" BackColor="aqua">
            <div class="box">
                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="lblAmendment" runat="server" Font-Bold="True" Text="DOCUMENT RECALL"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnCloseRecall"
                                ImageUrl="images/close.png" runat="server" /></td>
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
     <asp:TextBox ID="txtRemarkRecall" runat="server" CssClass="txtBox"
         Width="500px"></asp:TextBox>
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="btnRecallSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                        <asp:Button ID="btnCancelReject" runat="server" Text="Cancel" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>


        <%--for cancel popup --%>

        <asp:Button ID="btnCancelPopup" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="MP_CancelPopup" runat="server"
            TargetControlID="btnCancelPopup" PopupControlID="pnlCancel"
            CancelControlID="btnClosecancel" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>

        <asp:Panel ID="pnlCancel" runat="server" Width="800px" Height="300px" ScrollBars="Auto" BackColor="aqua">
            <div class="box">
                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="DOCUMENT CANCEL"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnClosecancel"
                                ImageUrl="images/close.png" runat="server" /></td>
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
                                        <asp:Button ID="btnCancelSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                        <asp:Button ID="btnCancelCan" runat="server" Text="Cancel" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>


        <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm" TargetControlID="btnShowPopupForm"
            BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlPopupForm" runat="server" Width="400px" Height="100px" BackColor="White" Style="display: none">
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
                            <asp:Button ID="btnOk" runat="server" Text="OK" OnClick="Reset" CssClass="btnNew" Width="80px" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>



        <%--for Amendment/Modify popup --%>

        <%--<asp:Button id="btnAmendPopup" runat="server" style="display:none" />
 <asp:ModalPopupExtender ID="MP_AmendPopup" runat="server" 
                                TargetControlID="btnAmendPopup" PopupControlID="pnlAmendment" 
                CancelControlID="btnAmendClose" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlAmendment" runat="server" Width="1000px" Height ="400px" ScrollBars ="Auto"  BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td><h3> <asp:Label ID="Label5" runat="server" Font-Bold="True" Text="DOCUMENT AMENDMENT" ></asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="btnAmendClose" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updDocAmendment" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
 <asp:Panel ID ="Panel5" runat ="server" >
 <asp:Label ID="lblmsgAmendment" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label></asp:Panel> 
 <asp:Panel ID="pnlDocAmendment" Width="100%" runat="server">
 </asp:Panel>
 <div style="width:100%;text-align:right">
 <asp:Button ID="btnSaveAmendment" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
 </div> 

    </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
        --%>

        <%--for document child item edit on amendment mode--%>

        <%--<asp:Button id="btnChildAmend" runat="server" style="display:none" />
 <asp:ModalPopupExtender ID="MP_ChildAmend" runat="server" 
                                TargetControlID="BtnchildAmend" PopupControlID="pnlPopupChildAmend" 
                CancelControlID="btnCloseChildAmend" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupChildAmend" runat="server" Width="800px" Height ="300px" ScrollBars ="Auto"  BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td><h3> <asp:Label ID="Label6" runat="server" Font-Bold="True"></asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseChildAmend" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updChildAmendment" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
 <asp:Panel ID ="Panel6" runat ="server" >
 <asp:Label ID="Label7" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label></asp:Panel> 
 <asp:Panel ID="pnlChildAmendment" Width="100%" runat="server">
 </asp:Panel>
 <div style="width:100%;text-align:right">
 <asp:Button ID="Button3" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" OnClick ="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
         
 </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>



        --%>
    </form>
</body>
</html>
