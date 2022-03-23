<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ShowMessage.aspx.vb" Inherits="ShowMessage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">
    <div>
        From: <asp:Label ID="lblFrom" runat="server" Text="" />
        <br />
        Subject: <asp:Label ID="lblSubject" runat="server" Text="" />
        <br />
        Body: <asp:Label ID="lblBody" runat="server" Text="" />
    </div>
    </form>
</body>
</html>
