<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" EnableEventValidation="false" CodeFile="SMSTemplate.aspx.vb" Inherits="SMSkeywordsetting" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
      <ContentTemplate> 
   
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg1" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>SMS Tamplate</h4></asp:Label>
    </td>
        </tr>
        <tr>
        <td style="text-align:left;"> <asp:Label ID="lblerr" runat="server"  style="text-align:left;" ForeColor="Red" ></asp:Label> </td>
        </tr>
    <tr style="color: #000000"> 
           <td style="text-align:right;width:100%;border:3px double green ">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" onclick="Add" ToolTip="ADD Form"/>
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
       <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
             CellPadding="2" DataKeyNames="tid"
                    CssClass="GridView"
               AllowSorting="True" PageSize ="20" AllowPaging="True">
                    <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                    <Columns>

  <asp:TemplateField HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="50px" />
</asp:TemplateField>
  <asp:BoundField DataField="tname" HeaderText="Template Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="ttype" HeaderText="Template Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                     
                        <asp:BoundField DataField="ttext" HeaderText="Template Text">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                                             
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                               
                                  <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" ToolTip ="Edit Template" AlternateText="Edit Template" />
                                 &nbsp;
                                                                
                                <%--<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHit" ToolTip ="Delete Form" AlternateText="Delete"/>--%>
                                                             <asp:ImageButton ID="btndelete" runat="server" OnClick="deletehit" ImageUrl="~/images/closered.png" Height="16px" Width="16px"  ToolTip ="Delete Template" AlternateText="Delete Template" />
                            </ItemTemplate>
                            <ItemStyle Width="220px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>
    </td> 
     </tr>
         </table>
      </ContentTemplate> 
     </asp:UpdatePanel> 


<asp:Button id="btnShowPopupForm" runat ="server" style="display:none" />
<asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat ="server" PopupControlID ="pnlPopupForm"  TargetControlID ="btnShowPopupForm" 
 CancelControlID ="btnCloseForm" BackgroundCssClass ="modalBackground" DropShadow ="true" ></asp:ModalPopupExtender>

 <asp:Panel ID="pnlPopupForm" runat ="server" Width ="500px" Height ="180px" BackColor ="White"  style="" >
 <div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td width="680px"><h3>Add SMS Template</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan ="2">
 <div id="main">
 <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<div class="form" style="text-align:left"> 

<table cellspacing="5px"  style="width:94%" cellpadding="0px" border="0px">
<tr><td colspan="2">   <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label> 
</td></tr>

<tr>
<td valign="middle" style="text-align:right;"><label title="" > *Template Type: 
  </label></td>
<td>
    <asp:DropDownList ID="ddlttype" runat="server" Width="230px">
        <asp:ListItem>Authentication</asp:ListItem>
        <asp:ListItem>Existence</asp:ListItem>
        <asp:ListItem>Processing</asp:ListItem>
        <asp:ListItem>Parameterized</asp:ListItem>
        <asp:ListItem>Success</asp:ListItem>
    </asp:DropDownList>
    </td></tr>
<tr>
   <td valign="middle" style="text-align:right;"><label title="" > *Template Name: 
  </label></td>
   <td > 
       <asp:TextBox ID="txttname" runat="server" CssClass="txtBox" 
          Width="220px" ></asp:TextBox> 
     </td>
     </tr>
     <tr>
     <td valign="middle" style="text-align:right; "><label title="" > *Template Text: 
  </label></td>
   <td > 
       <asp:TextBox ID="txttText" runat="server" CssClass="txtBox" TextMode="MultiLine"  
           Width="220px"></asp:TextBox> 
     </td>
</tr>

<tr>
   <td style="width: 174px"></td><td>
<asp:Button ID="btnsave" runat="server" OnClick="EditRecord" CssClass="btnNew" 
                Text="Save" Width="100Px" />
</td>

</tr>





</table>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</div>
</td></tr>
</table>
</div> 
 </asp:Panel>

<asp:Button id="btnShowPopupDeleteField" runat="server" style="Display:none" />
<asp:ModalPopupExtender ID="Modelpopup1" runat="server" 
                                TargetControlID="btnShowPopupDeleteField" PopupControlID="pnlPopupDeleteField" 
                CancelControlID="btnCloseDeleteField" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupDeleteField" runat="server" Width="500px" style="Display:none " BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Field Delete : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDeleteField" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updDeleteField" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<h2> <asp:Label ID="lblmessagedelete" runat="server" Font-Bold="True" Text="Are you sure want to Delete" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnDeleteField" runat="server" Text="Yes Delete"  Width="90px"  CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>

</table> 
</div>
</asp:Panel>

</asp:Content>


