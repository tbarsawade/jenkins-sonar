<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="WebServiceLogOutward, App_Web_gsdfcjye" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
    <Triggers>
     
     <asp:PostBackTrigger ControlID="btnexport" />
    </Triggers>
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
               <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h3>Role Master</h3></asp:Label>
                </td></tr>
                <tr>
                <td style="text-align: center; ">
               <asp:Label ID="lblMsgupdate" runat="server" Font-Bold="True" ForeColor="Red" 
                    Font-Size="Small" ></asp:Label>
                </td>

        </tr>

    <tr style="color: #000000">
        <td style="text-align:left;width:100%;border:3px double lime ">
       <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%" 
                border="0px">
         <tr> 
            <td style="width: 90px;"> 
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Doc Type" Width="99%"> </asp:Label>
         </td>

         <td style="width: 220px;">
             <asp:DropDownList ID="ddlField" runat="server" CssClass="Inputform"  Width="99%">
             </asp:DropDownList>
         </td>

         <td style="width: 50px;">
         <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="From" Width="99%"></asp:Label>
         </td>

         <td style="width: 120px;">
               <asp:TextBox ID="txtValue" runat="server" CssClass="Inputform" Font-Bold="True" 
               Font-Size="Small"  Width="99%"></asp:TextBox>
              <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtValue" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
         </td>
             <td style="width:40px;">
                 <asp:Label ID="lblto"  Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" runat="server" Text="To"></asp:Label>
             </td>
             <td style="width: 120px;">
               <asp:TextBox ID="txtto" runat="server" CssClass="Inputform" Font-Bold="True" 
               Font-Size="Small"  Width="99%"></asp:TextBox>
              <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="txtto" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
         </td>
             <td style="width:60px;">
                 <asp:Label ID="lblres" runat="server"  Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Status"></asp:Label>
             </td>
             <td style="width:120px;"><asp:DropDownList Width="99%" runat="server" ID="ddlsta" >
                 <asp:ListItem Value="0">Select All</asp:ListItem>
                 <asp:ListItem Value="1">Success</asp:ListItem>
                 <asp:ListItem Value="2">Failed</asp:ListItem>
                 </asp:DropDownList></td>
         
          

         <td style="text-align: right;">
                          &nbsp;<asp:ImageButton ID="btnexport" runat="server" ToolTip="Export"  ImageUrl="~/Images/excel.gif" /> 
                       
                              <caption>
                                  &nbsp;
             </caption>
          
             </td>

         </tr>
      </table>

           <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>

         </ProgressTemplate>
      </asp:UpdateProgress>

        </td>
    </tr>


     <tr style="color: #000000">
     <td style="text-align: left;" valign="top">
         &nbsp;</td> 
     </tr>
         </table>
                         </ContentTemplate> 
                </asp:UpdatePanel> 
<asp:Button id="btnShowPopupEdit" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>



</asp:Content>
