<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="DeviceLocation, App_Web_erizob0y" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false" type="text/javascript"></script>
    <script src="Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="Scripts/NokiaGoogleMap.js" type="text/javascript"></script>
    <script type="text/javascript">
        var myMarkersArray = [];
        

        function toggleDiv(divId) {
            $("#" + divId).toggle();
        }
        function HideDiv(divId) {
            $("#" + divId).Hide();
        }
    </script>
    <style type="text/css">
    .gradientBoxesWithOuterShadows {
            height: 100%;
            width: 99%;
            padding: 5px;
            background-color: white;
            /* outer shadows  (note the rgba is red, green, blue, alpha) */
            -webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4);
            -moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);
            /* rounded corners */
            -webkit-border-radius: 12px;
            -moz-border-radius: 7px;
            border-radius: 7px;
            /* gradients */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5));
            background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%);
            background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
            background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
        }

        .style2 {
            width: 30%;
        }
    </style>
    <style type="text/css">
        .CompletionListCssClass {
            font-size: medium;
            border: 1px solid gray;
            background-color: white;
            width: auto;
            float: left;
            z-index: 100;
            position: initial;
            margin-left: -30px;
            overflow: scroll;
            height: 500px;
        }
    </style>
    <style type="text/css">
        /*Modal Popup*/
        .modalBackground {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }

        .modalPopup {
            background-color: White;
            border-width: 3px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            text-align: center;
        }

        .hidden {
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function pageLoad(sender, args) {
            var sm = Sys.WebForms.PageRequestManager.getInstance();
            if (!sm.get_isInAsyncPostBack()) {
                sm.add_beginRequest(onBeginRequest);
                sm.add_endRequest(onRequestDone);
            }
        }
        function onBeginRequest(sender, args) {
            var send = args.get_postBackElement().id;
            if (displayWait(send) == "yes") {
                $find(
                'PleaseWaitPopup').show();
            }
        }
        function onRequestDone() {
            $find(
            'PleaseWaitPopup').hide();

        }
        function displayWait(send) {
            return ("yes");
        }
    </script>

    <script type="text/javascript">
        var dt1, dt2, imeino;
        function getValues() {

            var imeino = $("#ContentPlaceHolder1_txtIMIENo").val();
            document.getElementById("ContentPlaceHolder1_hdimieno").value = imeino;

            var date1 = $("#ContentPlaceHolder1_Date1").val().split("/");
            var dt1 = date1[0] + "-" + date1[1] + "-" + date1[2];
            var time1 = $("#ContentPlaceHolder1_TextBox1").val();
            dt1 = dt1 + " " + time1;

            document.getElementById("ContentPlaceHolder1_hddate1").value = dt1;

            var date2 = $("#ContentPlaceHolder1_Date2").val().split("/");
            var dt2 = date2[0] + "-" + date2[1] + "-" + date2[2]
            var time2 = $("#ContentPlaceHolder1_TextBox2").val();
            dt2 = dt2 + " " + time2;

            document.getElementById("ContentPlaceHolder1_hddate2").value = dt2;

        }

        function MovingMap() {

            var im = document.getElementById("ContentPlaceHolder1_hdimieno").value;
            var d1 = document.getElementById("ContentPlaceHolder1_hddate1").value;
            var d2 = document.getElementById("ContentPlaceHolder1_hddate2").value;
            var vehcleName = document.getElementById("ContentPlaceHolder1_hdvehcleName").value;
            
            getLatLongs(im, d1, d2, vehcleName);
            
            var val = $('#<%= hdnui.ClientID%>').attr('value');
            if (val == 54) {
                GetSiteList();
            }
        }

       

    </script>

     <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>

    <div>
        <div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top: 10px; border:solid 1px #e2e2e2;">
            <asp:UpdatePanel ID="upnl" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="Button1" />
                </Triggers>
                <ContentTemplate>
                    <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">
                        <tr>
                            <td>
                                <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
                                <asp:HiddenField ID="hdveh" runat="server" />
                                <asp:HiddenField ID="hdcity" runat="server" />
                                <asp:HiddenField ID="hdcir" runat="server" />
                                <asp:HiddenField ID="HiddenField1" runat="server" />

                            </td>
                        </tr>
                        <tr>

                            <td align="Left" width="30%">
                                <fieldset style="height: 170px">

                                    <legend style="color: Black; text-align: Left; font-weight: bold;">
                                        <asp:CheckBox ID="circlecheck" runat="server" AutoPostBack="true" OnCheckedChanged="checkuncheckcicle" />
                                        <asp:Label ID="lblCircle" runat="server" Text="Label"></asp:Label>
                                    </legend>
                                    <asp:Panel ID="Panel2" runat="server" Height="130px"
                                        Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="true">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                </fieldset>
                            </td>
                            <td align="Left" width="30%">
                                <fieldset style="height: 170px">
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">
                                        <asp:CheckBox ID="Citycheck" runat="server" AutoPostBack="true" OnCheckedChanged="Citycheckuncheck" />
                                       <asp:Label ID="lblcity" runat="server" Text="Label"></asp:Label></legend>
                                    <asp:Panel ID="Panel3" runat="server" Height="150px"
                                        Style="display: block" ScrollBars="Auto">

                                        <asp:CheckBoxList ID="City" runat="server" AutoPostBack="true">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                </fieldset>

                            </td>

                            <td align="Left" width="35%">
                                <fieldset style="height: 170px">
                                    <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto"
                                        Style="display: block">
                                        <asp:CheckBoxList ID="UsrVeh" runat="server" AutoPostBack="True">
                                        </asp:CheckBoxList>

                                    </asp:Panel>
                                </fieldset>
                            </td>
                            <td align="center" class="m8b" width="5%"></td>
                        </tr>
                        <tr>
                            <td colspan="4" style="width: 100%">
                                <table cellspacing="0" cellpadding="3" rules="all" id="tblGVReport" style="border-color: Green; border-width: 1px; border-style: None; font-size: Small; width: 100%; border-collapse: collapse; display: none;">
                                    <tr class="gridheaderhome" align="center" style="color: Black; background-color: #D0D0D0; border-color: Green; border-width: 1px; border-style: solid; font-weight: bold; height: 25px;">
                                        <th scope="col" style="border-right: solid 1px Gray; width: 10%;">State</th>
                                        <th scope="col" style="border-right: solid 1px Gray; width: 15%;">City</th>
                                        <th scope="col" style="border-right: solid 1px Gray; width: 15%;">Device IMEI No.</th>
                                        <th scope="col" style="border-right: solid 1px Gray; width: 15%;">Vehicle No</th>
                                        <th scope="col" style="border-right: solid 1px Gray; width: 15%;">User Name</th>
                                        <th scope="col" style="border-right: solid 1px Gray; width: 15%;">Last Movement Date &amp; Time</th>
                                        <th scope="col" style="border-right: solid 1px Gray; width: 15%;">Current Location Address</th>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="width: 100%; text-align: center;">
                                <asp:Label ID="lblerror" runat="server" ForeColor="Red"></asp:Label></td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <br />
        <b>Enter Time and Date Details :</b>
        &nbsp;&nbsp;<span id="spnwait"></span>
        <asp:TextBox ID="txtIMIENo" runat="server" Style="display: none;"></asp:TextBox>
        <asp:AutoCompleteExtender ID="auto1" runat="server" CompletionInterval="1" TargetControlID="txtIMIENo" CompletionListCssClass="CompletionListCssClass" EnableCaching="true" MinimumPrefixLength="2" CompletionSetCount="10000000" ServiceMethod="AutoIMIENO"></asp:AutoCompleteExtender>
        <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="TextBox1" WatermarkText="HH:MM"></asp:TextBoxWatermarkExtender>
        <asp:TextBox ID="Date1" runat="server"></asp:TextBox>
        &nbsp;<asp:TextBox ID="TextBox1" runat="server" Width="100px" Text="00:00"></asp:TextBox>
        <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="Date1" Format="yyyy/M/d" runat="server"></asp:CalendarExtender>
        To
            <asp:TextBox ID="Date2" runat="server"></asp:TextBox>
        <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="TextBox2" WatermarkText="HH:MM">
        </asp:TextBoxWatermarkExtender>
        <asp:TextBox ID="TextBox2" runat="server" Width="100px" Text="23:59"></asp:TextBox>
        <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="Date2" Format="yyyy/M/d" runat="server"></asp:CalendarExtender>
        &nbsp;&nbsp;
            <asp:Button ID="Button1" runat="server" Text="Show" Style="background-color: green; border: 0px; color: white; border-radius: 6px; padding:5px;" OnClientClick="getValues();" />&nbsp;
        <input type="button" value="Play" onclick="Play()" style="background-color: green; padding:5px; border: 0px; color: white; border-radius: 6px" />&nbsp;
        <input type="button" value="Stop" onclick="StopMarker()" style="background-color: green; padding:5px; border: 0px; color: white; border-radius: 6px" />&nbsp;
        <br /> 
        <br />
            Maximum Speed
            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>(Km/Hr) &nbsp;&nbsp;
            Total Distance
            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>Km&nbsp;&nbsp;
            <br />
        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
        <br />
        <div id="map" style="width: 1000px; height: 1000px;border:solid 1px #e2e2e2;">
        </div>
        <asp:HiddenField ID="hdimieno" runat="server" />
        <asp:HiddenField ID="hdvehcleName" runat="server" />
        <asp:HiddenField ID="hddate1" runat="server" />
        <asp:HiddenField ID="hddate2" runat="server" />
    </div>
    <asp:Panel ID="PleaseWaitMessagePanel" runat="server" CssClass="modalPopup" Height="50px" Width="125px">
        Please wait &nbsp&nbsp&nbsp&nbsp&nbsp<asp:ImageButton ID="imgclose" runat="server" ImageUrl="~/images/close.png" />
        <br />
        <img src="images/uploading.gif" alt="Please wait" />
    </asp:Panel>
    <asp:Button ID="HiddenButton" runat="server" CssClass="hidden" Text="Hidden Button"
        ToolTip="Necessary for Modal Popup Extender" />
    <asp:ModalPopupExtender ID="PleaseWaitPopup" BehaviorID="PleaseWaitPopup" runat="server" TargetControlID="HiddenButton" PopupControlID="PleaseWaitMessagePanel" BackgroundCssClass="modalBackground" CancelControlID="imgclose">
    </asp:ModalPopupExtender>
    <input type="hidden" id="hdnmove" value="0" />

   <asp:HiddenField ID="hdnui" runat="server" />

</asp:Content>

