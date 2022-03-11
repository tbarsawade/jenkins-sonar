<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="DeviceRegistration, App_Web_tuuwcp04" viewStateEncryptionMode="Always" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">

   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
               <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h3>Device Registration</h3></asp:Label>
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
           <%-- <td style="width: 120px;"> 
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="IMIEI Number" Width="99%"> </asp:Label>
         </td>--%>

          <td style="width: 90px;">
                                    <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Field Name" Width="99%">
                                    </asp:Label>
                                </td>
           <td style="width: 170px;">
             <asp:DropDownList ID="ddluserlist" runat="server" CssClass="Inputform" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Width="99%">
                   <asp:ListItem>USERNAME</asp:ListItem>
                 <asp:ListItem>IMEI NUMBER</asp:ListItem>
                   <asp:ListItem>USERID</asp:ListItem>
                  

             </asp:DropDownList>
         </td>

         <td style="width: 200px;">
               <asp:TextBox ID="txtValue" runat="server" CssClass="Inputform" Font-Bold="True" 
               Font-Size="Small"  Width="99%"></asp:TextBox>
         </td>

             
           
         <td style="text-align: right;width:25px">
             <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" 
                 ImageUrl="~/Images/search.png" />

          </td>
          <td>
              &nbsp;
             </td>

         <td style="text-align: right;">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" onclick="Add"/>
                          &nbsp;<caption>
                              <asp:Label ID="lblRecord" runat="server"  ForeColor="red" 
               Font-Size="Small" ></asp:Label>
             </caption>
          
             </td>

         </tr>
      </table>

           <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50% ; z-index:-999999">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>

         </ProgressTemplate>
      </asp:UpdateProgress>

        </td>
    </tr>


     <tr style="color: #000000">
     <td style="text-align: left;" valign="top">
       <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
             CellPadding="2" DataKeyNames="TID"  
                    ForeColor="#333333" Width="100%" 
               AllowSorting="False" AllowPaging="False" >
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                    <EditRowStyle BackColor="#2461BF" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
<asp:TemplateField HeaderStyle-HorizontalAlign="Left"  HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
    <HeaderStyle HorizontalAlign="Left" />
</asp:TemplateField>

                        <asp:BoundField DataField="iminumber" HeaderText="IMEI Number">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                                         
                        <asp:BoundField DataField="User" HeaderText="User">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>     

                         <asp:BoundField DataField="Userid" HeaderText="UserId">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>  

                        <asp:BoundField DataField="Status" HeaderText="Status" >
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>     
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                            
                                 
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/editrole1.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit Role"  AlternateText="Edit" />
                                &nbsp;
                               <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/lock.png"  runat="server" Height="16px" Width="16px" OnClick="LockHit" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />
                                 <%--<asp:ImageButton ID="img_user" runat="server" CommandName="Select" ImageUrl='<%# Eval("status")%>' Width="20px" Height="20px" />--%>
        
        
                                &nbsp;
                                   <asp:ImageButton ID="btnDtl" ImageUrl="~/images/closered.png"  runat="server" Height="16px" Width="16px"  ToolTip="Delete Role" OnClick="DeleteHit" AlternateText="Delete" />
                            </ItemTemplate>
                            <ItemStyle Width="90px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>
                 
       
      

       
    </td> 
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

<asp:Panel ID="pnlPopupEdit" runat="server" Width="600px"  BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3>New / Update Device Registration</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">

<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 

   <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>

 <tr>
    <td style="width:125px" align="left"><b>IMIEI No.* </b></td>
    <td align="left"><asp:TextBox ID="txtimi" runat="server" Width="98%"></asp:TextBox>
        <asp:RegularExpressionValidator runat="server" id="rexNumber" controltovalidate="txtimi" validationexpression="^[0-9]{15}$" ForeColor="Red"  errormessage="* Please enter a 15 digit number!" />

    </td>
   </tr>

   <tr>
    <td style="width:125px" align="left"><b>User Name * </b></td>
    <td align="left">  <asp:DropDownList ID="ddluser" runat="server"  CssClass="txtbox"  Width="98%">
                         </asp:DropDownList></td>
   </tr>
       
  
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Update" 
                            OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 

</td>
</tr>
</table> 
</div>
</asp:Panel>


<asp:Button id="btnShowPopupDelFolder" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder" 
                CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Delete !!!!</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelFolder" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete" 
                            OnClick="DelFile" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
    <asp:Button id="btnLock" runat="server" style="Display:none" />
<asp:ModalPopupExtender ID="ModalPopup_Lock" runat="server" 
                                TargetControlID="btnLock" PopupControlID="pnlPopupLock" 
                CancelControlID="btnCloseLock" BackgroundCssClass="modalBackground" 
                                >
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupLock" runat="server" Width="500px" style="Display:none" BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Active / InActive : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseLock" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updLock" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<asp:Label ID="lblLock" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="440px" Font-Size="X-Small" ></asp:Label>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnLockupdate" runat="server" Text="Yes Lock"  Width="90px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                    OnClick="LockRecord" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
</asp:Content>
