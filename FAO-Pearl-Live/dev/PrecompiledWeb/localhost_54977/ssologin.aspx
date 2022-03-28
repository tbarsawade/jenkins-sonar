<%@ page language="VB" autoeventwireup="false" inherits="ssologin, App_Web_nfrpb0kv" viewStateEncryptionMode="Always" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
    </div>
        <asp:Panel ID="pnlsso" runat="server" Visible="false">
            <div style="color:red;text-align:center;">
                <h1>Invalid token</h1>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
