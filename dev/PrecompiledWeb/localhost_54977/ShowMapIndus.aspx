<%@ page language="VB" autoeventwireup="false" inherits="ShowMapIndus, App_Web_01howaz0" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title>Show Map</title>
    <script src="js/jquery-1.9.1.min.js"></script>
    <script src="js/Gmarkers.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>

     <script> 
         var myMarkersArray = [];
         debugger;

         $(document).ready(function () {

             Markers.initialize();
             Markers.GetSites();
           });
         
     </script>


</head>
<body>
    <form id="form1" runat="server">
        
 <asp:Panel ID="nomap" runat="server" Visible="false">
 <b> I have no map for this trip</b>
     <div id="msg">

     </div>
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
        <br />
        
        <div style="border:2px solid;width: 90%; height: 800px; margin:auto;">  
                      
         <div id="map" style="width: 100%; height: 800px;"></div>
  
  </div>
  </asp:Panel>
    </form>
</body>
</html>
