<%@ page language="VB" autoeventwireup="false" inherits="Lenskart, App_Web_x1sxprmw" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Please Enter DocumentID :"></asp:Label>
                <br />
        <br />
                <asp:TextBox ID="TextBox2" Font-Size="Large" runat="server" Width="1244px" Style="margin-bottom: 2%;"></asp:TextBox>
            <br />
             <asp:Button ID="Vendor_Master" runat="server" Height="33px" Text="Vendor Master" Width="147px" />
             <asp:Button ID="Purchase_Requistion" runat="server" Height="33px" Text="Purchase Requistion" Width="147px" />
            <asp:Button ID="vendor_Invoice" runat="server" Height="33px" Text="Vendor Invoice"  Visible="false" Width="147px" />
             <asp:TextBox ID="TextBox1" Font-Size="Medium" runat="server" Height="59px" Width="1251px" Style="margin-top:2%"></asp:TextBox>
        </div>
    </form>
</body>
</html>
