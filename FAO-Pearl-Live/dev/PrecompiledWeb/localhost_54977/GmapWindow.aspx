<%@ page language="VB" autoeventwireup="false" inherits="GmapWindow, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Show Map</title>
    <script src="js/jquery-1.9.1.min.js"></script>
    <script src="js/Gmarkers.js"></script>
        
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
    
        <div style="margin:auto;width:90%; text-align:left; ">
        
            <table style="width:100%; " border="1" cellpadding="4" cellspacing="0"  >
                <col style="width:50px;" />
                <col style="width:150px;" />
                <col style="width:50px;" />
                <col style="width:150px;" />
                <tr id="trVehicle" runat="server">
                    <td style="font-weight:bold;">Vehicle No</td>
                    <td><asp:Label ID="lblVehicleNumber" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;"></td>
                    <td></td>
                </tr>
                <tr id="trspeed" runat="server">
                    <td style="font-weight:bold;">Maximum speed</td>
                    <td><asp:Label ID="lblMaxSpeed" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;">Total Distance</td>
                    <td><asp:Label ID="lblTotalDist" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr id="trStartEndLoc" runat="server">
                    <td style="font-weight:bold;">Start location</td>
                    <td><asp:Label ID="lblStartLocation" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;">End location</td>
                    <td><asp:Label ID="lblEndLocation" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr id="trStartEndDt" runat="server">
                    <td style="font-weight:bold;">Start Date & Time</td>
                    <td><asp:Label ID="lblstartDt" runat="server" Text=""></asp:Label></td>
                    <td style="font-weight:bold;">End Date & Time</td>
                    <td><asp:Label ID="lblendDt" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </div>

        <br />
        
        <div style="border:2px solid;width: 90%; height: 800px; margin:auto;">  
                      
         <div id="map" style="width: 100%; height: 800px;"></div>
  
  </div>
  </asp:Panel>
    </form>
</body>
</html>
