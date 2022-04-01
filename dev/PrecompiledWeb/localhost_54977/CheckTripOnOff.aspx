<%@ page title="" language="VB" autoeventwireup="false" inherits="CheckTripOnOff, App_Web_54qm34zl" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
 
<asp:ImageButton ID="btnexportxl"  ToolTip="Export EXCEL" ImageAlign="Right"  runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg"/> &nbsp;
<div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px">
  <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9"> 
     
          <tr>
                            
              <td align="left" class="style2">
              <table>
                          <tr>

                          <td  colspan="2" WIDTH="300PX">

                              <br />
                                 </td>
                                 <td align="center" ><b>
                          ENTER IMEI NO : </b>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtIMEI"  runat="server" Width="150PX" Height="18PX" BorderColor="#9999FF" BorderStyle="Outset"></asp:TextBox>&nbsp;
                                   <asp:ImageButton ID="btnshow" runat="server" Height="32px" Width="60px"  ImageUrl="~/images/showbutton.png"  ToolTip="Search  " />
                                     <br />
                                                    
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br /><br />
                              </td>
                              </tr>
                              
                                  </table>
                  &nbsp;</td>
             
          </tr>
            <tr>
          <td style=" width:70%">
              <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="Red" ></asp:Label>
                </td>
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
  
  <asp:Panel  runat="server" ID="pngv"> 
                      <asp:GridView ID="GVReport" runat="server" AllowPaging="False" 
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
                                           

                  </td>
              </tr>
          </table>
 </div>

</div> 
        </form> 
        </body> 
    </html> 

