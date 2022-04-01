<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ConsolidatedLastSignal, App_Web_zha44vbj" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 
<asp:ImageButton ID="btnexportxl"  ToolTip="Export EXCEL" ImageAlign="Right"  runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg"/> &nbsp;
<div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px">
  <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9"> 
     
          <tr>
                            
              <td align="left" class="style2">
              <table>
                          <tr>

                          <td >

                          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;From Date&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtsdate" text="yyyy-mm-dd"  runat="server"></asp:TextBox>
                          <asp:TextBox ID="TxtStime" Width="50px"  Text="00:00" runat="server"></asp:TextBox><ajaxToolkit:TextBoxWatermarkExtender ID="TBWE2" runat="server"
    TargetControlID="TxtStime"
    WatermarkText="HH:MM"
    WatermarkCssClass="watermarked" /> 
                              <br/><br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;To Date&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtedate"  text="yyyy-mm-dd"  runat="server" AutoCompleteType="Search" ></asp:TextBox>
                             <asp:TextBox ID="txtetime" Width="50px" Text="23:59" runat="server"></asp:TextBox>
                              <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
    TargetControlID="txtetime"
    WatermarkText="HH:MM"
    WatermarkCssClass="watermarked" /> <br /><br />
                                 </td>
                                 <td >
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
                                   <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px"  ImageUrl="~/images/showbutton.png"  ToolTip="Search  " />
                                     <br />
                                                    
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br /><br />
                              </td>
                              </tr>
                              
                                  </table>
                  &nbsp;</td>
             
          </tr>
            <tr>
          <td style=" width:70%"></td>
          <td  style=" width:30%; text-align:right;">
           
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

