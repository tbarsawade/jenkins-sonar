<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="MenuControl.aspx.vb" Inherits="MenuControl" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<style type="text/css" >
.btndynamic
{
text-decoration: none; 
text-align: center;
font-weight: bold;
font-size : 12px; 
background:#2278e2;  
color:white; 
height: 24px;
}


.btndynamic:hover 
{ 
	text-decoration: none; 
	background: white;
	color:Blue ;  
	}

</style>
    <div id="main">
   <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>

  <td colspan="2" style="width:30%;" valign="top" align="left">

<h1>Menu Control : <asp:Label ID="lblFolderName" runat="server" Text=""></asp:Label>
        </h1>  
 
  </td> 
</tr>

<tr style="">
    
  <td valign="top" align="left">
      <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>

 <asp:UpdatePanel ID="updGrid" runat="server" UpdateMode="Conditional" >
   <ContentTemplate>
   <asp:Panel ID="pnlgrid" runat="server" Height="350px" style="overflow:auto  ;  width:1000px;">
  
    <asp:GridView ID="gvData" runat="server"  
             CellPadding="3" Width="100%" AutoGenerateColumns="true" 
           BackColor="White" BorderColor="#CCCCCC" 
           BorderStyle="None" BorderWidth="1px">
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                     <AlternatingRowStyle BackColor="#d8dbe5"/>
               <Columns>
               <asp:TemplateField>
               <ItemTemplate>
               <asp:Button runat="server"  Text="Dynamic Menu" ID="btndm" CssClass="btndynamic"  Visible="false"  OnClick="showpopup" />
               </ItemTemplate>
               </asp:TemplateField>
               </Columns>
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <PagerStyle ForeColor="#000066" HorizontalAlign="Left" BackColor="White" />
                    <HeaderStyle   BackColor="#006699" Font-Bold="True"   ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                </asp:GridView>
                              
                      
              </asp:Panel>
              <asp:Button ID="Button1" runat="server"  CssClass="btnNew" Width ="150px" 
        Text="Save" UseSubmitBehavior="False" Visible="True"        />
                      <b>
       <asp:Label ID="lblMsgS" runat="server"></asp:Label></b><br />
 
   
    </ContentTemplate> 
  </asp:UpdatePanel> <br/>
  
 
   
</td>    
</tr>
</table> 
</div>
 <asp:Button id="btnShowPopupEdit" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupEdit" runat="server" Width="800px" Height="300px"    BackColor="white">
<div class="box" style="overflow:scroll; height:300px">
<table cellspacing="2px" cellpadding="2px" width="98%">
<tr>
<td style="width:550px"><h3>Menu Access</h3></td>
<td style="width:40px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">

<asp:UpdatePanel ID="updatePanelEdit" runat="server"   UpdateMode="Conditional">
 <ContentTemplate> 

   <table cellspacing="2px" cellpadding="0px" width="99%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
   <tr>
   <td>
   <asp:GridView ID="GridView1" runat="server"  
             CellPadding="3" Width="100%" AutoGenerateColumns="true" 
           BackColor="White" BorderColor="#CCCCCC" 
           BorderStyle="None" BorderWidth="1px">
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                     <AlternatingRowStyle BackColor="#d8dbe5"/>
               
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <PagerStyle ForeColor="#000066" HorizontalAlign="Left" BackColor="White" />
                    <HeaderStyle   BackColor="#006699" Font-Bold="True"   ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                </asp:GridView>
   </td>
   </tr>

  
  </table>
          <div style="width:99%;text-align:right; ">
                    <asp:Button ID="btnActEdit" runat="server" Text="SAVE" 
                            CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 

</td>
</tr>
</table> 
</div>
</asp:Panel>

</asp:Content>


