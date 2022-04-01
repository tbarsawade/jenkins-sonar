<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="testFormula, App_Web_tkfk5fna" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table width="100%">
        
        <tr>
            <td>
            <asp:label ID="lblHelp" Text="Syntax Help"   runat="server" Font-Size="X-Small" Font-Bold="true" Width="100%" Wrap="true"></asp:label>
            </td>
        </tr>

        <tr>
            <td>
            <asp:label ID="label2" Text="Type here Expression" Font-Size="Small" ForeColor="RED" Font-Bold ="true"  runat="server" Width="100%" Wrap="true"></asp:label>
            </td>
        </tr>
        <tr>
            <td>
            <asp:TextBox ID="txtInput" runat="server" Height="50px" Rows="2" TextMode="MultiLine" Width="100%" Wrap="true"></asp:TextBox>

            </td>
        </tr>
        <tr>
             <td align="center">
                 <asp:Button ID="btnTestPRAalertmails" runat="server" Text=" Test mails" />
                 </td>
        </tr>
        <tr>
            <td>
            <asp:label ID="label1" Font-Size="Small" Font-Bold ="true"  ForeColor="RED" Text="Result"  runat="server" Width="100%" Wrap="true"></asp:label>
            </td>
        </tr>
        <tr>
            <td>
            <asp:TextBox ID="txtOutput" runat="server" Height="125px" Rows="5" TextMode="MultiLine" Width="100%" Wrap="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    
    
</asp:Content>

