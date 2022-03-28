<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="WSErrLog, App_Web_wzrtint1" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 
<asp:ImageButton ID="btnexportxl"  ToolTip="Export EXCEL" ImageAlign="Right"  runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg"/> &nbsp;
<div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px">
    <table>
        <colgroup style="width:150px;"></colgroup>
         <colgroup style="width:150px;"></colgroup>
         <colgroup style="width:120px;"></colgroup>
         <colgroup style="width:120px;"></colgroup>
        <tr>
            <td>From Date</td> <td>To Date</td> <td>Success/Fail</td> <td></td>
        </tr>
        <tr>
            <td>
<asp:TextBox ID="txtsdate"  runat="server"></asp:TextBox>
                              <ajaxToolkit:CalendarExtender ID="txtsdate_CalendarExtender" runat="server" Enabled="True" TargetControlID="txtsdate">
                              </ajaxToolkit:CalendarExtender>
            </td> 
            <td>
                <asp:TextBox ID="txtedate"   runat="server" AutoCompleteType="Search" ></asp:TextBox>
                              <ajaxToolkit:CalendarExtender ID="txtedate_CalendarExtender" runat="server" Enabled="True" TargetControlID="txtedate">
                              </ajaxToolkit:CalendarExtender>

            </td>
             <td>
                 <asp:DropDownList ID="ddlSuccess" runat="server">
                     <asp:ListItem>-Select-</asp:ListItem>
                     <asp:ListItem Value="1">Success</asp:ListItem>
                     <asp:ListItem Value="0">Fail</asp:ListItem>
                 </asp:DropDownList>
             </td>
             <td>
                 <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px"  ImageUrl="~/images/showbutton.png"  ToolTip="Search  " />
                                  
             </td>
        </tr>

    </table>




  
    </div>
    
    <div>
          <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">               
                           <tr>
                                            <td>
                                                <br />
  <asp:UpdatePanel ID="upnl" runat="server">
  <ContentTemplate>
  <asp:Panel  runat="server" ID="pngv"> 
                      <asp:GridView ID="GVReport" runat="server" AllowPaging="true" 
                          AllowSorting="true" AutoGenerateColumns="true" BorderColor="Green" 
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



                       <asp:UpdateProgress ID="UpdateProgress2" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>
      </asp:Panel>
      </ContentTemplate>
  </asp:UpdatePanel>                                              

                  </td>
              </tr>
          </table>
 </div>

</asp:Content>

