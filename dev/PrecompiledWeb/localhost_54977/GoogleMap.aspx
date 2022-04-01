<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="GoogleMap, App_Web_fdh01zus" viewStateEncryptionMode="Always" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src="http://maps.google.com/maps/api/js?sensor=false" type="text/javascript"></script>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:760px">
      <h3> <asp:Label ID="lblLoginName" runat="server" Text="Login User"></asp:Label>    &nbsp;&nbsp;&nbsp;
          <asp:DropDownList ID="DropDownList1" runat="server">
          </asp:DropDownList>
          <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" TargetControlID="TextBox1"
                    WatermarkText="HH:MM" runat="server">
          </ajaxToolkit:TextBoxWatermarkExtender>
      &nbsp; &nbsp;
      <asp:TextBox runat="server" ID="Date1" Width="100px"  /> &nbsp;
          <asp:TextBox ID="TextBox1" runat="server" Width="100px"></asp:TextBox>   &nbsp;
          <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="Date1" Format="yyyy-MM-dd" >
          </ajaxToolkit:CalendarExtender>
      </h3>
    <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
</td>
<td>
             <asp:Button ID="btnAll" runat="server" CssClass="txtBox"  Text="All Team" Width="106px" />
             &nbsp;
             <asp:Button ID="btnJustMe" runat="server" CssClass="txtBox"  Text="Just Me" Width="106px" />
</td> 
</tr>
    <tr>
<td colspan="2" style="text-align:center">
          <div id="map" style="width: 980px; height: 500px;"></div>
</td>
</tr>
</table> 
</asp:Content>

