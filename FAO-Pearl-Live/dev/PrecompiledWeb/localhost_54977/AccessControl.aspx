<%@ page title="Access Control" language="VB" masterpagefile="~/USR.master" autoeventwireup="True" inherits="AccessControl, App_Web_sfds111l" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


    <div id="main">
   <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>

  <td colspan="2" style="width:30%;" valign="top" align="left">
<asp:UpdatePanel ID="updMsg" runat="server" UpdateMode="Always" >
    <ContentTemplate>
<h1>Access Control : <asp:Label ID="lblFolderName" runat="server" Text="Folder Name"></asp:Label>
        </h1>  
</ContentTemplate> </asp:UpdatePanel>   
  </td> 
</tr>

<tr style="">
    <td valign="top" align="left">
    <div class="form" style="margin-left:-5px; overflow: auto;width:220px; height:380px;">
<asp:UpdatePanel ID="updTV" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
        <asp:TreeView ID="tv" runat="server" Height="100%" ImageSet="Inbox" 
            Width="100%">
            <ParentNodeStyle Font-Bold="False" />
            <HoverNodeStyle Font-Underline="True" />
            <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" 
                VerticalPadding="0px" />
            <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" 
                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
        </asp:TreeView>
</ContentTemplate> 
</asp:UpdatePanel> 
    </div>     
    </td>
  <td valign="top" align="left">
      <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>

 <asp:UpdatePanel ID="updGrid" runat="server" UpdateMode="Conditional" style="margin-left:-3px; ">
   <ContentTemplate>
   <asp:Panel ID="pnlgrid" runat="server" Height="360px" style="overflow:auto  ;  width:750px;">
  
    <asp:GridView ID="gvData" runat="server"  
             CellPadding="3" Width="100%" AutoGenerateColumns="False" 
           DataKeyNames="TID" BackColor="White" BorderColor="#CCCCCC" 
           BorderStyle="None" BorderWidth="1px">
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                     <AlternatingRowStyle BackColor="#d8dbe5"/>
                    <Columns>
                        <asp:BoundField DataField="grpName" HeaderText="Group Name" />
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

                      <b>
       <asp:Label ID="lblMsgS" runat="server"></asp:Label></b><br />
  
  
   <asp:Button ID="btnSave" runat="server"  CssClass="btnNew" Width ="150px" 
        Text="Save" UseSubmitBehavior="False" Visible="false"        />
    </ContentTemplate> 
  </asp:UpdatePanel> <br/>
  
 
   
</td>    
</tr>
</table> 
</div>

</asp:Content>


