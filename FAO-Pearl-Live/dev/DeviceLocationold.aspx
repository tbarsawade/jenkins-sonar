<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DeviceLocationold.aspx.vb" Inherits="DeviceLocationold" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

                       IMIE No :

                       <asp:TextBox ID="txtIMIENo" runat="server"></asp:TextBox>
        
        &nbsp;&nbsp;
        

  <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"    TargetControlID="TextBox1"
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
      &nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" Text="SEE ON MAP" onclick="Button1_Click" />      
         Maximum Speed <asp:Label ID="Label1" runat="server" Text=""></asp:Label>(Km/Hr) &nbsp;&nbsp;&nbsp;&nbsp;
       Total Distance <asp:Label ID="Label3" runat="server" Text=""></asp:Label>Km&nbsp;&nbsp;&nbsp;&nbsp;
     
        <br />
     <div id="map" style="width: 1000px; height: 1000px;"></div>
    </div>
    </form>
</body>
</html>
