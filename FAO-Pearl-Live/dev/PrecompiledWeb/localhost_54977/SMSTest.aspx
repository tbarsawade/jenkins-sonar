<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="SMSTest, App_Web_sfds111l" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <br />
    <br />
    Mobile Number
    <asp:TextBox ID="txtNo" runat="server">9810923322</asp:TextBox>
&nbsp;Message with keyword :<asp:TextBox ID="txtMSG" runat="server"></asp:TextBox>
&nbsp;<asp:Button ID="btnCheck" runat="server" Text="Test"/>
    

    <asp:Button ID="Button15" runat="server" Text="DynamicTrip from Swicth" />
    

    <br />
    <asp:TextBox ID="txtResult" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="lblFMonth" runat="server" Text="Select Date for Trip:" Style="color: Maroon;
                                font-weight: bold"></asp:Label>
                       
                                                
                      
                           <asp:TextBox ID="txtsdate"   runat="server" Width="176px"></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="yyyy-MM-dd"   runat="server" />
                       
    <asp:Button ID="Button1" runat="server" Text="TripFromSwitch" />
    &nbsp;
    <asp:Button ID="Button2" runat="server" Text="Location Update" />&nbsp;
     <asp:Button ID="Button3" runat="server" Text="KM Update" />
    <asp:Button ID="Button4" runat="server" Text="Check Query" />
    <asp:Button ID="Button5" runat="server" Text="Update Site and Location" />
    <asp:Button ID="Button6" runat="server" Text="Nighttrip Create" />
    <asp:Button ID="Button7" runat="server" Text="Check Attendance" Width="133px" />
    <asp:Button ID="Button12" runat="server" Text="Check Attendance Consolidated" />
    <asp:Button ID="Button8" runat="server" Text="SMSAlert" />
    <asp:Button ID="Button9" runat="server" Text="XML Generate" />
    <asp:Button ID="Button10" runat="server" Text="Get Vehicles" />
    <asp:Button ID="Button11" runat="server" Text="SMS APM" />
    <br />
     <asp:Button ID="Button13" runat="server" Text="Trip for Indus" />
     <asp:Button ID="Button14" runat="server" Text="Location Update for Indus" />



    <asp:Label ID="lblmsg" runat="server" Text="" Style="color: Maroon;
                                font-weight: bold"></asp:Label>

    <asp:GridView ID="GridView1" runat="server"></asp:GridView>

    <br />

</asp:Content>

