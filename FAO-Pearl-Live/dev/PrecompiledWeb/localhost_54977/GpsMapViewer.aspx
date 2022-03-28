<%@ page language="VB" autoeventwireup="false" inherits="GpsMapViewer, App_Web_nfrpb0kv" viewStateEncryptionMode="Always" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Show Map</title>
</head>
<body>
    <form id="form1" runat="server">
 <asp:Panel ID="nomap" runat="server" Visible="false">
 <b> No Data found</b>
 </asp:Panel>   
 <asp:Label ID="msg" runat="server"  ForeColor="Red"></asp:Label>
    <asp:Panel ID="Tripmap" runat="server"  Visible="false">
        <br /><br />
        <table align="center">
        <tr>
        <td >
        <div style="border:2px solid;width: 1100px; height: 800px; border-color:Teal" >         
             <div id="map" style="width: 1100px; height: 800px;"></div>
         </div>
        </td>
        </tr>
        </table>
        
  
  
  </asp:Panel>
    </form>
</body>
</html>
