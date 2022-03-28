<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="OnMap, App_Web_kxub2lm0" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <script src="http://maps.google.com/maps/api/js?sensor=false" 
          type="text/javascript"></script>

    <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="SEEMAP">
    </asp:DropDownList>
               Maximum Speed <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>(Km/Hr) &nbsp;&nbsp;&nbsp;&nbsp;
     Total Distance(Haversine one) <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>Km&nbsp;&nbsp;&nbsp;&nbsp;

       Total Distance(Haversine two) <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>Km&nbsp;&nbsp;&nbsp;&nbsp;
        Total Distance(Haversine Three) <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>Km
        <br /><br />
           <div id="map" style="width: 1000px; height: 1000px;"></div>
</asp:Content>

