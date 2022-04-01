<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TestForm.aspx.vb" Inherits="TestForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   <%-- <div>--%>
      <%--  <asp:TextBox ID="txtfid" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtEID" runat="server"></asp:TextBox>
    <asp:Button ID="btnSave" runat="server" Text="Getrecord" OnClick="btnSave_Click" />
    </div>
        <div>
            <asp:GridView ID="gv" runat="server" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                <SortedDescendingHeaderStyle BackColor="#93451F" />

            </asp:GridView>
        </div>--%>
        <asp:Label ID="lvl" runat="server">Enter Valid Email ID</asp:Label>
        <%--<asp:TextBox ID="txttest" TextMode="MultiLine"   runat="server"></asp:TextBox>--%>
        <asp:TextBox ID="txtEmail" Height="20px" Width="200px" Font-Bold="true" runat="server"></asp:TextBox>
        <asp:Label ID="Label2" runat="server">Enter Valid Atchment path</asp:Label>
         <asp:TextBox ID="txtAtchment" Height="20px" Width="200px" Font-Bold="true" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Send mail" />
        <asp:Button ID="Button2" runat="server" Text="Send Attachment mail" />
        <asp:Label ID="Label1" runat="server" Text="Label" Width="400px"></asp:Label>
    </form>
</body>
</html>
