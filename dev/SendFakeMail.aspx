<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SendFakeMail.aspx.vb" Inherits="SendGreeting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 96px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table cellspacing="0px" cellpadding="0px" width="800px" border="2px">
        <tr>
            <td class="auto-style1">Subject</td>
            <td>        <asp:TextBox ID="txtSubject" runat="server"  Width="771px"></asp:TextBox></td>
        </tr>

         <tr>
            <td class="auto-style1">Email IDs :</td>
            <td>        
                <asp:TextBox ID="txtMailIDs" runat="server" Height="16px" Width="771px"></asp:TextBox></td>
        </tr>

        <tr>
            <td class="auto-style1">Sendor ID</td>
            <td> <asp:TextBox ID="txtSendorID" runat="server"  Width="771px"></asp:TextBox></td>
        </tr>

         <tr>
            <td class="auto-style1">Password</td>
            <td> <asp:TextBox ID="txtPWD" runat="server"  Width="771px" TextMode="Password"></asp:TextBox></td>
        </tr>

         <tr>
            <td class="auto-style1">BCC</td>
            <td> <asp:TextBox ID="txtBCC" runat="server"  Width="771px"></asp:TextBox></td>
        </tr>

    </table>

        <asp:Button ID="btnSend" runat="server" Text="Send" Width="170px" />
    
    &nbsp;<asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
    
    </div>
    </form>
</body>
</html>
