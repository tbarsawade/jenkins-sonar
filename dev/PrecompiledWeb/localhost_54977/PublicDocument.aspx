<%@ page title="" language="VB" masterpagefile="~/PublicMaster.master" autoeventwireup="false" inherits="PublicDocument, App_Web_2echgblw" viewStateEncryptionMode="Always" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%--<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>--%>
<script type="text/javascript" src ="js_child/jquery.min.cache" ></script>
 <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />   
    
<script type="text/javascript">
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

    function pageLoad(sender, args) {

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

     </script> 
 
<asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>


     <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
<td style="width:100%;" valign="top" align="left">
	<div id="main" style="min-height:400px">
<asp:Label ID="lblCaption" runat="server"></asp:Label>
<asp:UpdatePanel ID="UpdatePanel1" runat="server"  UpdateMode ="Conditional"  >
<Triggers>
   <%--  <asp:PostBackTrigger ControlID ="btnimmm"  />--%>
    <%-- <asp:PostBackTrigger ControlID="helpexport" />--%>
     <%--<asp:PostBackTrigger ControlID="btnimport" />--%>
</Triggers>
<ContentTemplate>

<div class="form" style="text-align:left"> 
<table width ="100%">
<tr>
<td>
<asp:Label ID="lblTab" runat ="server" Font-Bold ="true" ForeColor ="Red" Font-Size="Small"  ></asp:Label>
</td>
</tr>
<tr><td>
<asp:Panel ID="pnlFields" runat ="server" Width ="960px" >

</asp:Panel>
<div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Save" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                             CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
 </div>
 </td></tr></table>
</div>
</ContentTemplate>
        </asp:UpdatePanel>
</div>	
</td>    
</tr>
</table> 

<asp:Panel ID="pnlMsg" runat ="server" Visible ="false" >
<asp:Label ID="lbview" runat ="server" Text ="You don't have write to Create Document !" ForeColor ="Red" Font-Bold ="true" ></asp:Label>
</asp:Panel>




<asp:Button id="btnShowPopupForm" runat ="server" style="display:none" />
<asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat ="server" PopupControlID ="pnlPopupForm"  TargetControlID ="btnShowPopupForm" 
 BackgroundCssClass ="modalBackground"  ></asp:ModalPopupExtender>
 <asp:Panel ID="pnlPopupForm" runat ="server" Width ="400px" Height ="100px" BackColor ="White" style="display:none"   >
 <div class ="box">
 <table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td style="width:400px"><h3>Confirmation</h3></td>
</tr>
<tr>
<td>
</td>
</tr>
<tr>
<td >
<asp:UpdatePanel ID="updMsg" runat ="server" UpdateMode ="Conditional" ><ContentTemplate >
<asp:Label ID="lblMsg" runat ="server" Font-Bold ="true" ForeColor ="Red" Font-Size ="Small"  ></asp:Label>
</ContentTemplate></asp:UpdatePanel>
</td>
</tr>
<tr>
<td align ="right" >
<asp:Button ID="btnOk" runat ="server" Text ="OK" CssClass ="btnNew" Width ="80px" />
</td>
</tr>
</table> 
 </div>
 </asp:Panel> 

<asp:Button id="btnReConfirmation" runat ="server" style="display:none" />
<asp:ModalPopupExtender ID="MP_Reconfirm" runat ="server" PopupControlID ="pnlReConfirm"  TargetControlID ="btnReConfirmation" CancelControlID="calncelReconfirm"  BackgroundCssClass ="modalBackground"  ></asp:ModalPopupExtender>
 <asp:Panel ID="pnlReConfirm" runat ="server" Width ="400px" Height ="100px" BackColor ="White" style="display:none"   >
 <div class ="box">
 <table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td style="width:400px"><h3>Re - Confirmation</h3></td>
</tr>
<tr>
<td>
</td>
</tr>
<tr>
<td >
<asp:UpdatePanel ID="updReConfirm" runat ="server" UpdateMode ="Conditional" ><ContentTemplate >
<asp:Label ID="lblReconfirmation" runat ="server" Font-Bold ="true" ForeColor ="Red" Font-Size ="Small"> Please reconfirm details filled in, since this is non editable once submitted </asp:Label>
</ContentTemplate></asp:UpdatePanel>
</td>
</tr>
<tr>
<td align ="right">
<asp:Button ID="btnComrmation" runat ="server" Text ="SAVE" OnClick ="ConfirmedData" CssClass ="btnNew" Width ="80px" /> <asp:Button ID="calncelReconfirm" runat ="server" Text ="CANCEL" OnClick="CancelConfirmedData" CssClass ="btnNew" Width ="80px" />
</td>
</tr>
</table> 
 </div>
 </asp:Panel> 




 <asp:Button id="Btnchild" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
                                TargetControlID="Btnchild" PopupControlID="pnlPopupchild" 
                CancelControlID="btnClose" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupchild" runat="server" Width="800px" Height ="300px" ScrollBars ="Auto"  BackColor="aqua" >
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:780px"><asp:UpdatePanel ID="updChildFormname" runat="server" UpdateMode="Conditional">
 <ContentTemplate>
  <div style="width:760px"><h3><asp:Label ID="lblChildFormName" runat="server" Font-Bold="True"></asp:Label></h3></div>
</ContentTemplate> </asp:UpdatePanel> </td><td style="width:20px;text-align:Right"><asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server"/></td></tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
  <asp:ImageButton ID="btnimmm" align="right"  ToolTip="Import" runat="server" Height="22px" Width="90px" Visible ="false"  ImageUrl="~/Images/upload.png"/>
  <asp:Panel ID ="Pnllable" runat ="server" >
 <asp:Label ID="lblTab1" runat ="server" Font-Bold ="true" ForeColor ="Red" Font-Size="Small"  ></asp:Label>
 <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label></asp:Panel> 
 <asp:Panel ID="pnlFields1" Width="100%" runat="server">
 </asp:Panel>
 <div style="width:100%;text-align:right">
<asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" OnClick ="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                    
 </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>

<asp:Button id="BtnAdd" runat="server" style="display:none"  />
<asp:ModalPopupExtender ID="AddTask_ModalPopUp" runat="server" 
                                TargetControlID="BtnAdd" PopupControlID="pnlAddTask"  CancelControlID ="TaskClose"
                 BackgroundCssClass="modalBackground" DropShadow ="true"  >
</asp:ModalPopupExtender>

<asp:Panel ID="pnlAddTask" runat="server" Width="500px" Height ="330px" ScrollBars ="Auto" style="display:none"   BackColor="white">
<div class="box">
<asp:UpdatePanel ID="UpdPnlAddTask" runat="server"  UpdateMode="Conditional"  >
<ContentTemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Manage Task</h3></td>
<td style="width:20px"><asp:ImageButton ID="TaskClose" ImageUrl="images/close.png" runat="server" /></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="4"><asp:Label ID="Label1" runat="server"  Text="Please select the field"></asp:Label></td>
     </tr> 
     <tr><td align="left"><label>User Name:</label></td>
     <td align ="left" ><asp:DropDownList ID ="ddluser" runat ="server" Width="150px"  CssClass="txtBox"></asp:DropDownList> </td>
     <td align="left" valign ="top" ><label>Due Date:</label></td>
    <td align ="left" ><asp:TextBox ID="txtDue_Date" runat ="server" CssClass ="txtBox"></asp:TextBox>
    <asp:CalendarExtender ID="txtCLNDR" runat="server"  TargetControlID ="txtDue_Date"></asp:CalendarExtender>
    </td></tr>
   <tr>
   <td align ="left" ><label>Remarks:</label>   </td>
   <td align ="left" colspan ="3"><asp:TextBox ID="txtRemarks" TextMode ="MultiLine" Width ="100%" runat ="server" CssClass ="txtBox"></asp:TextBox></td>
   </tr>    
   <tr><td align ="right" colspan ="4" ><asp:Button ID="BTNTask" runat ="server" CssClass ="btnNew" Text ="Add User" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" /></td></tr>
</table>
</div>
</td>
</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
</div>
</asp:Panel>




<%--child item uploader popup--%>
 <style id="cssStyle" type="text/css" media="all">
.CS
{
background-color:white;
color: #99ae46;
border: 1px solid #99ae46;
font: Verdana 10px;
padding: 1px 4px;
font-family: Palatino Linotype, Arial, Helvetica, sans-serif;

}
</style>
   <%--<asp:Button id="btnim" runat="server" style=" display:none;" />
            <asp:ModalPopupExtender ID="modalpopupimport" runat="server" 
                                TargetControlID="btnim" PopupControlID="pnlimport" 
                CancelControlID="ImageButton2" BackgroundCssClass="modalBackground" 
                                DropShadow="true" >
                                </asp:ModalPopupExtender>
<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
<asp:Panel ID="pnlimport" runat="server" Width="400px" Height="100px"    BackColor="white">
<div class="box" > 
<table cellspacing="1px" cellpadding="2px" width="100%" border="0px" >
<tr>

<td><h3> <asp:Label ID="lblpophead" runat="server" Text="Import Csv File"  Font-Bold="True"></asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="ImageButton2" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr >
<td><br /><asp:FileUpload ID="impfile" CssClass="CS"   runat="server" /> &nbsp;
             <asp:ImageButton ID="btnimport" ToolTip="Import" style=" vertical-align:bottom;" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/import.png"/>&nbsp;
              <asp:ImageButton ID="helpexport"  ToolTip="Download Import Sample & Format" runat="server" style=" vertical-align:bottom;" Width="20px" Height="20px" ImageUrl="~/Images/Helpexp.png"/>
             &nbsp;</td>

</tr>
</table>
</div>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>--%>

 <asp:Button id="btnstatus" runat="server"  />
    <asp:ModalPopupExtender ID="modalstatus" runat="server" 
                                TargetControlID="btnstatus" PopupControlID="pnlstatus" 
                CancelControlID="ImageButton1" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlstatus" runat="server" Width="500px" Height="300px"    BackColor="aqua">
<div class="box" Width="500px" > 
<table cellspacing="2px" cellpadding="2px" width="100%" border="1px" >
<tr>
<td><h3> <asp:Label ID="lblstatus" runat="server" text="Data Uploading Status" Font-Bold="True"></asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="ImageButton1" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr style=" border:1px solid black;"><td><asp:Label ID="lblstat" runat="server" Font-Bold="True"></asp:Label></td></tr>
<tr style=" border:1px solid black;"><td><asp:Label ID="Label4" runat="server" Font-Bold="True"></asp:Label></td></tr>
<tr><td><table border="1px" ><tr><td width="217"><span style="color:Black; font-weight:bold;">Row No</span></td><td width="217px"><span style="color:Black; font-weight:bold;">Column Where Error Occured</span></td></tr></table></td></tr>
<tr style=" border:1px solid black;"><td> <div style="overflow:scroll; height:300px"> <asp:Label ID="lblstatus1" runat="server" /></div></td></tr>
</table>
</div>
</asp:Panel>
 </asp:Content>

