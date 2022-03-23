<%@ page language="VB" autoeventwireup="false" inherits="PhoneReport, App_Web_tj04ay21" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:DropDownList ID="DropDownList1" runat="server">
        </asp:DropDownList> &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" Text="SEE ON MAP" onclick="Button1_Click" />

  <asp:TextBoxWatermarkExtender ID="TextBoxWatermkarkExtender1" runat="server"    TargetControlID="TextBox1"
    WatermarkText="HH:MM">
    </asp:TextBoxWatermarkExtender>
               <asp:TextBox ID="Date1" runat="server"></asp:TextBox>
&nbsp;<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
<asp:CalendarExtender ID="CalendarExtender1"   TargetControlID="Date1"  Format="yyyy/M/d" runat="server">  </asp:CalendarExtender>


    To <asp:TextBox ID="Date2" runat="server"></asp:TextBox>
      <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server"    TargetControlID="TextBox2"
    WatermarkText="HH:MM">
    </asp:TextBoxWatermarkExtender>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <asp:CalendarExtender ID="CalendarExtender2"   TargetControlID="Date2"  Format="yyyy/M/d" runat="server"></asp:CalendarExtender>
       Maximum Speed <asp:Label ID="Label1" runat="server" Text=""></asp:Label>(Km/Hr) &nbsp;&nbsp;&nbsp;&nbsp;
     Total Distance(Haversine one) <asp:Label ID="Label2" runat="server" Text=""></asp:Label>Km&nbsp;&nbsp;&nbsp;&nbsp;

       Total Distance(Haversine two) <asp:Label ID="Label3" runat="server" Text=""></asp:Label>Km&nbsp;&nbsp;&nbsp;&nbsp;
        Total Distance(Haversine Three) <asp:Label ID="Label4" runat="server" Text=""></asp:Label>Km
        <br /><br />
     <div id="map" style="width: 1000px; height: 1000px;"></div>
    </div>
    </form>
</body>
</html>
