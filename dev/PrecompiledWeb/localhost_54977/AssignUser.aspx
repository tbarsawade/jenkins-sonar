<%@ page title="Assign User" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="AssignUser, App_Web_2echgblw" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; "> <h3>Assign Users To Project <asp:Label ID="lblMsg" runat="server" Font-Bold="True"></asp:Label></h3></td>
           <td style="text-align: Right; ">
          <%-- <asp:ImageButton ID="links" runat="server" Width="20px" Height="20px"  ImageUrl="~/images/links.jpg" OnClick="usefullLinks"/>--%>
           </td>
           </tr>
           
    <tr style="color: #000000">
        <td style="text-align:left;width:100%;">
<div class="formSer">
 <table cellspacing="0px" cellpadding="0%" width="100%" border="0px">


   <tr style="color: #000000">
     <td colspan="3" style="text-align: left;" valign="top">
            <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>

 <asp:UpdatePanel ID="updGrid" runat="server"  UpdateMode="Conditional"  style="margin-left:5px; overflow: auto;width:960px" >
   <ContentTemplate> 
    <asp:GridView ID="gvData" runat="server" 
             CellPadding="3" Width="100%" AutoGenerateColumns="False" 
           DataKeyNames="UID" BackColor="#DEBA84" BorderColor="#DEBA84" 
           BorderStyle="None" BorderWidth="1px" CellSpacing="2">
                    <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                    <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                    <Columns>
                        <asp:BoundField DataField="grpName" HeaderText="Group Name" />
                    </Columns>
                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                    <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#FFF1D4" />
                    <SortedAscendingHeaderStyle BackColor="#B95C30" />
                    <SortedDescendingCellStyle BackColor="#F1E5CE" />
                    <SortedDescendingHeaderStyle BackColor="#93451F" />
                </asp:GridView>
<br />
   <asp:Button ID="btnSave" runat="server" CssClass="btnNew" Width ="150px" 
        Text="Save" UseSubmitBehavior="False" />
                       <br /> <b>
       <asp:Label ID="lblMsgS" runat="server"></asp:Label></b>
    </ContentTemplate> 
  </asp:UpdatePanel> 

    </td> 
     </tr>

</table> 
</div>
        </td>
    </tr>
    </table>


<%--
   <asp:ModalPopupExtender ID="link_ModelPopControl" runat="server" 
                                TargetControlID="Links" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>


<asp:Panel ID="pnlPopupEdit" runat="server" Width="500px" style="" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3>Update Usefull link</h3></td>
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
       <asp:TextBox ID="txtDName" runat="server" CssClass="txtBox" 
           Width="290px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *URL : </label></td>
   <td style=""> 
       <asp:TextBox ID="txtUrl" runat="server" CssClass="txtBox" 
           Width="290px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Display Order : </label></td>
   <td style=""> 
       <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="txtBox" 
           Width="290px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> Script To Run : </label></td>
   <td style=""> 
       <asp:TextBox ID="txtScriptToRun" runat="server" CssClass="txtBox" 
           Width="290px"></asp:TextBox></td>
</tr>





 <tr>
 <td> 
     &nbsp;</td>
        <td align="left">

          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Update" 
                            OnClick="UpdateRecord" CssClass="btnNew" Font-Bold="True" 
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


--%>

</asp:Content>

