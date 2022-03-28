<%@ page language="VB" autoeventwireup="false" inherits="VLCCOutWard, App_Web_nfrpb0kv" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Please Enter PearlID :"></asp:Label>
                <br />
        <br />
                <asp:TextBox ID="TextBox2" Font-Size="Large" runat="server" Width="1244px" Style="margin-bottom: 2%;"></asp:TextBox>
            <br />
             <asp:Button ID="Vendor_Registration" runat="server" Height="33px" Text="Vendor Registration" Width="147px" />
             <asp:Button ID="GRN_Invoice" runat="server" Height="33px" Text="Invoice GRN" Width="147px" />
             <asp:Button ID="Vendor_Modification" runat="server" Height="33px" Text="Vendor Modification" Width="147px" />
              <asp:Button ID="purchase_Invoice" runat="server" Height="33px" Text="Purchase Invoice PO" Width="147px" />
             <asp:Button ID="purchase_Invoice_NON_PO" runat="server" Height="33px" Text="Purchase Invoice NON PO" Width="170px" />
      
             <asp:TextBox ID="TextBox1" Font-Size="Medium" runat="server" Height="59px" Width="1251px" Style="margin-top:2%"></asp:TextBox>
        </div>
    </form>
</body>
</html>
