<%@ page title="Field master" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="FieldMaster, App_Web_ws3kahym" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    F<asp:UpdatePanel ID="updPnlGrid" runat="server">
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>Form Desginer</h4></asp:Label>
    </td>
        </tr>

    <tr style="color: #000000">
        <td style="text-align:right;width:100%;border:3px double green ">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" onclick="Add"/>
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
             CellPadding="2" DataKeyNames="Fieldid"
                    ForeColor="#333333" Width="100%" 
               AllowSorting="True" AllowPaging="True">
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                    <EditRowStyle BackColor="#2461BF" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>

  <asp:TemplateField HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="50px" />
</asp:TemplateField>

  <asp:BoundField DataField="displayName" HeaderText="Display Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                     
                        <asp:BoundField DataField="fieldtype" HeaderText="Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="displayorder" HeaderText="Order">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="dropdown" HeaderText="Drop Down Values">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>


                        <asp:BoundField DataField="isrequired" HeaderText="Mandatory">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="isActive" HeaderText="Status">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                             
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" AlternateText="Edit" />
                                 &nbsp;
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHit" AlternateText="Delete"/>
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center"/>
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

<asp:Panel ID="pnlPopupEdit" runat="server" Width="500px" style="" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3>New / Update Field</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2" style="text-align:left">

<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 

 <div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">

<tr><td style="width:100%" colspan="2">
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
    </td></tr>
<tr>
   <td style="text-align:right"> <label> *Display Name : </label></td>
   <td style=""> 
       <asp:TextBox ID="txtFName" runat="server" CssClass="txtBox" 
           Width="290px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Field Type : </label></td>
   <td style=""> 
       <asp:DropDownList ID="ddlType" runat="server" CssClass="txtBox" 
           Width="200px" AutoPostBack="True">
           <asp:ListItem>Text Box</asp:ListItem>
           <asp:ListItem>Text Area</asp:ListItem>
           <asp:ListItem>Drop Down</asp:ListItem>
       </asp:DropDownList>
    </td>
</tr>

    <tr>
        <td colspan="2" style="text-align:right">
            <asp:TextBox ID="txtDDLValues" runat="server" CssClass="txtBox" Visible="False" 
                Width="430px"></asp:TextBox>
        </td>
    </tr>

<tr>
   <td style="text-align:right"> <label> Mandatry Field : </label></td>
   <td style=""> 
       <asp:CheckBox ID="chkM" runat="server" CssClass="txtBox" 
           Text=" Field Is Mandatry" Checked="True" />
       &nbsp;&nbsp;
       <asp:CheckBox ID="chkActive" runat="server" CssClass="txtBox" Text=" Active" 
           Checked="True" />
    </td>
</tr>

 <tr>
 <td> 
     &nbsp;</td>
        <td align="left">

          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Update" 
                            OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
        
            <br />
        </td>
    </tr>

</table>
</div>

   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>

<asp:Button id="btnShowPopupDelete" runat="server" style="Display:none" />

<asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete" 
                CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" style="Display:none " BackColor="Aqua" >

<div class="box">

<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Field Delete : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
        <h2> <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnActDelete" runat="server" Text="Yes Delete"  Width="90px" 
                    OnClick="DeleteRecord" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
</asp:Content>

