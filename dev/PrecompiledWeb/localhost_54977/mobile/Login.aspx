<%@ page language="VB" autoeventwireup="false" inherits="Login, App_Web_dzjktowh" viewStateEncryptionMode="Always" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, usesscr-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="box">
        <table width="100%" border="0" cellspacing="2px" cellpadding="2px">
            <tr>
                <td style="text-align: center; width: 100%">
                    <asp:Image ID="imgComp" runat="server" ImageUrl="images/logo.gif" Height="40px" Width="280px" />
                </td>
            </tr>
            <tr>
                <td style="text-align: center; width: 100%">
                    <asp:Image ID="Image1" runat="server" ImageUrl="images/user-login.png" Height="120px"
                        Width="100px" />
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 100%">
                    User ID
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 100%">
                    <asp:TextBox ID="txtName" runat="server" TabIndex="1"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 100%">
                    Password
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 100%">
                    <asp:TextBox ID="txtPwd" runat="server" TabIndex="2"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 100%">
                    Entity Code
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 100%">
                    <asp:TextBox ID="txtEntity" runat="server" TabIndex="3"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td style="text-align:left; width: 100%">
                    <asp:Button ID="btnLogin" runat="server" class="btnLogin" TabIndex="6" Text="Login" />
                </td>
            </tr>
            <tr>
                <td style="text-align:left; width: 100%">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" style="color:Red;" runat="server" Font-Size="X-Small"  ErrorMessage="User ID Required." ControlToValidate="txtName"></asp:RequiredFieldValidator><br />
                <asp:RequiredFieldValidator ID="rqfldPwd" style="color:Red;" runat="server" Font-Size="X-Small" ErrorMessage="Password Required." ControlToValidate="txtPwd"></asp:RequiredFieldValidator><br />
                <asp:RequiredFieldValidator ID="reqFLDEntity" style="color:Red;" runat="server" Font-Size="X-Small" ErrorMessage="Entity Code Required." ControlToValidate="txtEntity"></asp:RequiredFieldValidator><br />
                </td>
            </tr>
             <tr>
                <td style="text-align:left; width: 100%">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
