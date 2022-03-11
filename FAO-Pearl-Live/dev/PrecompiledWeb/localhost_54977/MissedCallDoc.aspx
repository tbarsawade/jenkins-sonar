<%@ page language="VB" autoeventwireup="false" inherits="DeviceLocation, App_Web_tqo4ibz3" viewStateEncryptionMode="Always" %>
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

                             
        &nbsp;&nbsp;
        

 
        <asp:Button ID="Button1" runat="server" Text="Create Doc" onclick="Button1_Click" /> &nbsp;     
       
        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
        <br />
     <div id="map" style="width: 1000px; height: 1000px;"></div>
    </div>
    </form>
</body>
</html>
