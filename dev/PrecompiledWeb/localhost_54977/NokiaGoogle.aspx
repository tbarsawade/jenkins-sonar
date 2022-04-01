<%@ page language="VB" autoeventwireup="false" inherits="Default3NOKIAGOOGLE, App_Web_ds2n0wlx" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">


    <title></title>

</head>
<body>
    <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    <div>
    <asp:DropDownList ID="DropDownList1" runat="server">
        </asp:DropDownList> 
       
         <asp:Button ID="Button1" runat="server" Text="SEE ON MAP" onclick="Button1_Click" />
               <asp:TextBox ID="Date1" runat="server"></asp:TextBox>
&nbsp;<asp:TextBox ID="TextBox1" runat="server" Text="00:00"></asp:TextBox>
<asp:CalendarExtender ID="CalendarExtender1"   TargetControlID="Date1"  Format="yyyy/M/d" runat="server">  </asp:CalendarExtender>


    To <asp:TextBox ID="Date2" runat="server"></asp:TextBox>

    <asp:TextBox ID="TextBox2" runat="server" Text="23:59"></asp:TextBox>
    <asp:CalendarExtender ID="CalendarExtender2"   TargetControlID="Date2"  Format="yyyy/M/d" runat="server"></asp:CalendarExtender>
         Maximum Speed <asp:Label ID="Label1" runat="server" Text=""></asp:Label>(Km/Hr) &nbsp;&nbsp;&nbsp;&nbsp;
     Total Distance <asp:Label ID="Label2" runat="server" Text=""></asp:Label>Km&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:DropDownList ID="MAPTYPE" runat="server">
        <asp:ListItem>NOKIA</asp:ListItem>
        <asp:ListItem>GOOGLE</asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <div id="mapshowr" style="width:600px; height:400px;"></div>
 
    </form>
</body>
</html>
