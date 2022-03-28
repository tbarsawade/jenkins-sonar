<%@ page language="VB" autoeventwireup="false" inherits="MasterMap, App_Web_0gl03q5k" masterpagefile="~/USR.master" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="css/style.css" rel="Stylesheet" type="text/css" />
<link rel="stylesheet" type="text/css" href="/resources/demos/style.css"/>
<link rel="stylesheet" type="text/css" href="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.css"/>

<script src="http://code.jquery.com/jquery-1.9.1.js" type="text/javascript"></script>
<script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
<script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false"></script> 
<script type="text/javascript" charset="UTF-8" src="https://js.cit.api.here.com/ee/2.5.3/jsl.js?with=all"></script>
<script type="text/javascript" charset="UTF-8" src="https://developer.here.com/enterprise/apiexplorer/examples/templates/js/exampleHelpers.js"></script>
<script src="Scripts/NokiaGoogleMap.js" type="text/javascript"></script>
<link href="StyleSheet.css" rel="stylesheet" type="text/css" />

<style type="text/css"> 
/*Modal Popup*/ 
.modalBackground 
{
background-color: Gray; 
filter: alpha(opacity=70); 
opacity: 0.7; 
}
.modalPopup 
{
background-color: White; 
border-width: 3px; 
border-style: solid; 
border-color: Gray; 
padding: 3px; 
text-align: center; 
}
.hidden 
{
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
        window.close();
    }
    function displayWait(send) {
        return ("yes");
    }
</script>




<script type="text/javascript">    
    $(document).ready(function ()
    {
       var dt = GetParameterValues('DocName'); GetData('<%=Session("EID")%>', dt);
    });
   
 function GetParameterValues(param)
   {
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] == param) {
                return urlparam[1];
            }
        }
 }

</script>

 
<asp:Panel ID="PnlMap" runat="server">
<table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
            border="0px">
            <tr>
                <td style="width: 210px;"> 
                   </td>
                <td style="width: 210px; text-align:left;">
                </td>
                <td style="width: 210px;">                                    
                </td>
                <td style="width: 210px;">
                    <div>
                        
                        <asp:HiddenField ID="hdntid" runat="server" />
                        
                    </div>
                </td>
                <td style="text-align: right; width: 25px">  
                    <%--<asp:Button ID="btnsavedata" runat="server" Text="Save" OnClick="btnsavedata_Click" />--%>
                </td>
            </tr>            
        </table>
        <div id="dvMap" style="width: 1000px; height: 700px;"></div>  
         <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
            border="0px">
              
            <tr>
                 <th></th>
            </tr>
             <tr>
                <td style="width: 210px;">    <label id="lblCity"></label></td>
                  
                <td style="width: 210px;">
                   

                </td>



                <td style="width: 210px;"></td>

                <td style="width: 210px;">
                   
                </td>

                <td style="text-align: right; width: 25px">
                   
                </td>
                 </tr>
        </table>
    </asp:Panel>    
<asp:Button ID="btnEdit" runat="server" style="display:none;" OnClick="EditHit" />
<asp:Button ID="btnLockhit" runat="server" style="display:none;" OnClick="LockHit" />
<asp:UpdatePanel ID="upsa" runat="server">
<ContentTemplate>          
<asp:HiddenField ID="hdfc" runat="server" />
<asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
<asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
   <asp:Panel ID="pnlPopupEdit" runat="server" Width="900px" Style="display: none" BackColor="aqua">
    <div class="popup">
        <div class="content">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updPanalHeader" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h3>
                                    <asp:Label ID="lblHeaderPopUp" runat="server" Font-Bold="True"></asp:Label></h3>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <%--<Triggers > <asp:PostBackTrigger ControlID ="btnActEdit" /></Triggers>--%>
                            <ContentTemplate>
                                <asp:Panel ID="pnlPoupup" Width="900px" runat="server">
                                    <asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </asp:Panel>
                              
                                <div style="width: 100%; text-align: right">
                                 <asp:Button ID="btnActEdit" runat="server" Text="Save" OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Button ID="btnLock" runat="server" Style="Display: none" />
    <asp:ModalPopupExtender ID="ModalPopup_Lock" runat="server"
        TargetControlID="btnLock" PopupControlID="pnlPopupLock"
        CancelControlID="btnCloseLock" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupLock" runat="server" Width="500px" Style="Display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Lock / Unlock : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseLock" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updLock" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblLock" runat="server" Font-Bold="True" ForeColor="Red"
                                    Width="440px" Font-Size="X-Small"></asp:Label>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnLockupdate" runat="server" Text="Yes Lock" Width="90px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        OnClick="LockRecord" CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                                <asp:Label ID="lblRecord" runat="server" ForeColor="red"
                                    Font-Size="Small"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        </ContentTemplate>            
       </asp:UpdatePanel> 
<asp:Button ID="HiddenButton" runat="server" CssClass="hidden" Text="Hidden Button" 
ToolTip="Necessary for Modal Popup Extender" /> 
<asp:ModalPopupExtender ID="PleaseWaitPopup" BehaviorID="PleaseWaitPopup"   runat="server" TargetControlID="HiddenButton" PopupControlID="PleaseWaitMessagePanel" BackgroundCssClass="modalBackground" CancelControlID="imgclose"> 
</asp:ModalPopupExtender>
</asp:Content>
