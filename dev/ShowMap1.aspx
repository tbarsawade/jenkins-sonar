<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ShowMap1.aspx.vb" Inherits="ShowMap" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<%--<meta http-equiv="X-UA-Compatible" content="IE=edge" />--%>

    <title>Show Map</title>
</head>
<body>
    <form id="form1" runat="server">
 <asp:Panel ID="nomap" runat="server" Visible="false">
 <b> I have no map for this trip</b>
 </asp:Panel>   
 <asp:Label ID="msg" runat="server"  ForeColor="Red"></asp:Label>
    <asp:Panel ID="Tripmap" runat="server"  Visible="false">
    <div  style="margin-left:auto;margin-right:auto;width:1000px; text-align:center" >
  <asp:Label ID="User" Font-Bold="true" runat="server" Text=""></asp:Label>&nbsp;
       <b>Total Distance</b> <asp:Label ID="VehicleNo" runat="server" Text=""></asp:Label>&nbsp;
                      <b> Maximum Speed</b> <asp:Label ID="MaximumSpeed" runat="server" Text=""></asp:Label>(Km/Hr) &nbsp;
     <b>Total Distance</b> <asp:Label ID="TotalDistance" runat="server" Text=""></asp:Label>Km&nbsp;
     <br />
     <b>Trip Start Location</b> <asp:Label ID="TripStartLocation" runat="server" Text=""></asp:Label>&nbsp;
     <b>Trip Start Date & Time</b> <asp:Label ID="TripStartDateTime" runat="server" Text=""></asp:Label>&nbsp;
      
    <br />
       <b>Trip End Location</b> <asp:Label ID="TripEndLocation" runat="server" Text=""></asp:Label>&nbsp;
    
      <b>Trip End Date & Time</b> <asp:Label ID="TripEndDateTime" runat="server" Text=""></asp:Label>&nbsp;
    </div>
        <br /><br />
        <div style="border:2px solid;width: 1000px; height: 1000px;">              <div id="map" style="width: 1000px; height: 1000px;"></div>
  
  </div>
  </asp:Panel>
    </form>
</body>
</html>
