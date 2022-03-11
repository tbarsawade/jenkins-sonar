<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" enableeventvalidation="false" inherits="ElogbookUpload, App_Web_15ulzn3z" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="100%" Height="100%" Style="" BackColor="aqua">
        <div class="box" style="height: 700px">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <asp:Label ID="lblTxt" runat="server" Text="Elogbook Uploader" Font-Size="medium"></asp:Label>
                    </td>
                </tr>
            </table>
            <br /><br />
              <table cellspacing="2px" cellpadding="2px" width="100%">
            <tr>
            <td align="center" style="border-color:Red">
          
                                <table cellspacing="1px" cellpadding="2px" width="100%" border="1px" align="center">
                                    
                                    <tr align="center">
                                        <td align="center">
                                            <br />
                                            <asp:FileUpload ID="impfile" CssClass="CS" runat="server" />
                                            &nbsp;
                                            <asp:ImageButton ID="btnUpload" ToolTip="Import" Style="vertical-align: bottom;"
                                                runat="server" Width="20px" Height="20px" ImageUrl="~/Images/import.png" />&nbsp;
                                            <asp:ImageButton ID="helpexport" ToolTip="Download Import Sample & Format" runat="server"
                                                Style="vertical-align: bottom;" Width="20px" Height="20px" ImageUrl="~/Images/Helpexp.png" />
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                           
                       
            </td>
            </tr>
           <tr>
           <td>
             <asp:Label ID="lblMsg" runat ="server" Font-Bold ="true" ForeColor ="Red" Font-Size ="Small"  ></asp:Label>
           </td>
         
           </tr>
            <tr align="center">
             <td>
                <div>
                    <asp:Panel runat="server" ID="pngv" Width="980px" Height="600px" ScrollBars="Both">
                   <asp:GridView ID="gvData" runat="server" AllowPaging="true" 
                          AllowSorting="False" AutoGenerateColumns="true" BorderColor="Green" 
                          BorderStyle="none" BorderWidth="1px" CellPadding="3" 
                          EmptyDataText="Record does not exists." Font-Size="Small" PageSize="15" 
                          ShowFooter="false" Width="100%">
                          <RowStyle BackColor="White" BorderColor="Green" BorderWidth="1px" 
                              CssClass="gridrowhome" ForeColor="Black" Height="25px" />
                          <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                          <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                          <HeaderStyle BackColor="#d0d0d0" BorderColor="Green" BorderWidth="1px" 
                              CssClass="gridheaderhome" Font-Bold="True" ForeColor="black" Height="25px" 
                              HorizontalAlign="Center" />
                          <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                          <Columns>
                          </Columns>
                          <SortedAscendingCellStyle BackColor="#FFF1D4" />
                          <SortedAscendingHeaderStyle BackColor="#B95C30" />
                          <SortedDescendingCellStyle BackColor="#F1E5CE" />
                          <SortedDescendingHeaderStyle BackColor="#93451F" />
                          <SortedAscendingCellStyle BackColor="#EDF6F6" />
                          <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                          <SortedDescendingCellStyle BackColor="#D6DFDF" />
                          <SortedDescendingHeaderStyle BackColor="#002876" />
                          <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" 
                           VerticalAlign="Middle" />                                                       
                      </asp:GridView>
                        </asp:Panel> 
                    </div>
            </td>
                    
            
            </tr>
            
            </table>
            
                        
                  
        </div>
    </asp:Panel>
</asp:Content>
