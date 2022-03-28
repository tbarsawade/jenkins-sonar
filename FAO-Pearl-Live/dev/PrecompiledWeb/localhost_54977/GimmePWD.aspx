<%@ page language="VB" autoeventwireup="false" inherits="_GimmePWD, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Gimme Pwd</title>
</head>
<body> 
    <form id="form1" runat="server">
    <div style="text-align:center;"  >
        <table  style="width: 100%;text-align:center;">
            <tr>
                <td style="text-align: center">
                 <asp:Panel runat="server" ID="pnlAll" HorizontalAlign="Center"  > 

                    <table border="0" cellpadding="5" cellspacing="0" style="width: 300px; text-align: center">
                        <tr>
                            <td colspan="2" style="background-color: #4169e1">
                                <span style="font-size: 12pt; color: #ffffff; font-family: Verdana"><strong>Give Me
                                    Password</strong></span></td>
                        </tr>
                        <tr>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <span style="font-size: 10pt; font-family: Verdana">Password</span></td>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <asp:TextBox ID="txtPass" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <span style="font-size: 10pt; font-family: Verdana">Key</span></td>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <asp:TextBox ID="txtKey" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="width: 400px; background-color: #f0f8ff">
                                <asp:Button ID="btnGive" runat="server" Text="Give Me Password" /></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="width: 400px; background-color: #f0f8ff">
                                <asp:TextBox ID="txtResult" runat="server" Width="256px"></asp:TextBox></td>
                        </tr>
                    </table>
                    <br />
                    <table border="0" cellpadding="5" cellspacing="0" style="width: 300px; text-align: center">
                        <tr>
                            <td colspan="2" style="background-color: #4169e1">
                                <span style="font-size: 12pt; color: #ffffff; font-family: Verdana"><strong>Give Me
                                    Encrypted Password</strong></span></td>
                        </tr>
                        <tr>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <span style="font-size: 10pt; font-family: Verdana">Password</span></td>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <span style="font-size: 10pt; font-family: Verdana">Key</span></td>
                            <td style="width: 400px; background-color: #f0f8ff">
                                <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="width: 400px; background-color: #f0f8ff">
                                <asp:Button ID="Button1" runat="server" Text="Give Me Password" /></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="width: 400px; background-color: #f0f8ff">
                                <asp:TextBox ID="TextBox3" runat="server" Width="256px"></asp:TextBox></td>
                        </tr>
                    </table>
                    </asp:Panel>

                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
