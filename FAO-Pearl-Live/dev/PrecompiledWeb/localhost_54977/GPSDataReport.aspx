<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="GPSDataReport, App_Web_0gl03q5k" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js">
</script>--%>
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js"></script>
    <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function toggleDiv(divId) {
        $("#" + divId).toggle();
    }
    function HideDiv(divId) {
        $("#" + divId).Hide();
    }
</script>
<script src="http://maps.google.com/maps/api/js?sensor=false" 
          type="text/javascript"></script>
   <script type="text/javascript">

       function OpenWindow(url) {
           var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
           //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
           return false;
       }

    </script>
<style type="text/css">
.gradientBoxesWithOuterShadows { 
height: 100%;
width: 99%; 
padding: 5px;
background-color: white; 

/* outer shadows  (note the rgba is red, green, blue, alpha) */
-webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4); 
-moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);

/* rounded corners */
-webkit-border-radius: 12px;
-moz-border-radius: 7px; 
border-radius: 7px;

/* gradients */
background: -webkit-gradient(linear, left top, left bottom, 
color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5)); 
background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%); 
background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
}

    .style2
    {
        width: 30%;
    }

</style>
   
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
 <Triggers>
        <%--<asp:PostBackTrigger ControlID="btnSearch"  />--%>
       <asp:PostBackTrigger ControlID="btnExportPDF" />
       <asp:PostBackTrigger ControlID="btnexportxl" />
     <asp:PostBackTrigger ControlID ="btnshow" />
        </Triggers>
<ContentTemplate>
<br />
   <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>
<asp:ImageButton ID="btnExportPDF" ToolTip="Export PDF"  runat="server" Width="18px" ImageAlign="Right"  Height="18px" ImageUrl="~/images/export.png"/>&nbsp;&nbsp;
<asp:ImageButton ID="btnexportxl"  ToolTip="Export EXCEL" ImageAlign="Right"  runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg"/> &nbsp;
<div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px">
  <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9"> 
     
          <tr>
          <td>
          <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"  ></asp:Label>
          </td>
          </tr>
          <tr>
                            
                            <td  align="left" class="m8b" width="25%">
                          
                           <fieldset>

  <legend style=" color:Black; text-align:center; font-weight:bold;">Report Type</legend>
                            <br />
                            <br />
                 <asp:Panel ID="Panel2" runat="server" Height="100px"
                      style=" display:block ">
                  <asp:DropDownList ID="ddlrtype" Width="150px"  runat="server">
                      <asp:ListItem Value="0">Select</asp:ListItem>
                     <asp:ListItem Value="3">Mileage Report</asp:ListItem>
                     </asp:DropDownList> 
                  <br />
                  <br />
                  <asp:DropDownList ID="ddldhm" Width="150px" runat="server">
                      <asp:ListItem Value="0">Select</asp:ListItem>
                      <asp:ListItem Value="1">Daily</asp:ListItem>
                     <%-- <asp:ListItem Value="2">Half Hourly</asp:ListItem>--%>
                       </asp:DropDownList>
               </asp:Panel>
                  </fieldset>
              </td>
            
              <td align="left" class="style2">
                  <table   >
                      <tr>
                          <td >
                                         
                          <fieldset style="width:326px; height:170px;">
                          <legend style=" color:Black; text-align:center;font-weight:bold;">Date Range</legend>
                         
                          <table>
                          <tr>

                          <td >
                          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtsdate" AutoPostBack="false" onkeypress="return false;" runat="server"></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="yyyy-MM-dd"    runat="server" /> <br/><br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtedate"  runat="server" AutoCompleteType="Search" onkeypress="return false;"  ></asp:TextBox>
                                 <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd" runat="server" /> <br /><br />
                                 </td>
                                 <td >
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="TxtStime" Width="50px" Text="00:00"  runat="server"></asp:TextBox><br /><br />
                                                    
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtetime" Text="23:59" Width="50px" runat="server"></asp:TextBox>
                             <%-- <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
    TargetControlID="txtetime"
    WatermarkText="HH:MM"
    WatermarkCssClass="watermarked" />--%><br /><br />
                              </td>
                              </tr>
                              
                                  <caption>
                                      <br />
                                      <br />
                              </caption>
                             
                              </table>
                                </fieldset>
                          </td>
                      </tr>
                  </table>
              </td>
                            <td align="Left" width="35%">
                            <fieldset style="height:170px">
                        <legend style=" color:Black; text-align:left;font-weight:bold; "><asp:CheckBox ID="CheckBox1"  OnCheckedChanged="checkuncheck" AutoPostBack="true"  runat="server" /><asp:Label ID="lblvehn" runat="server" Text=""></asp:Label></legend>
                <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" 
                      style=" display:block ">
                      
       
                      <asp:CheckBoxList ID="UsrVeh"    runat="server">
               
                      </asp:CheckBoxList>
                      <%--       <asp:CheckBoxList ID="smsreports"   Visible="false"  runat="server">
               
                      </asp:CheckBoxList>--%>
                  </asp:Panel>
                  </fieldset>
              </td>
               <td align="center" class="m8b" width="5%">
                             <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px"  ImageUrl="~/images/showbutton.png"  ToolTip="Search  " />
              </td>
               </td>
          </tr>
            <tr>
          <td colspan="3" style=" width:70%"></td>
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



                      <asp:GridView ID="GVReport1" runat="server" AllowPaging="true" 
                          AllowSorting="False" AutoGenerateColumns="true" BorderColor="Green" 
                          BorderStyle="none" BorderWidth="1px" CellPadding="3" DataKeyNames="tid" 
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
                                 <Columns>
                                 <asp:TemplateField HeaderText="Map" ItemStyle-HorizontalAlign="Right"  >
                            <ItemTemplate>
                    <%--<a class="detail" style="text-decoration:none;"  href="#" onclick="OpenWindow('http://<%# HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath%>/<%#DataBinder.Eval(Container.DataItem, "uri")%>?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')"   >--%>
                                <%--<a class="detail" style="text-decoration:none;"  href="#" onclick="OpenWindow('http://<%# DataBinder.Eval(Container.DataItem, "uri")%>?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')"   >
                 <img alt="Show On Map" src="images/earth_search.png"  height="16px" width="16px"/></a>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                                </Columns>
                         
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
</ContentTemplate>
</asp:UpdatePanel>
 </asp:Content>
