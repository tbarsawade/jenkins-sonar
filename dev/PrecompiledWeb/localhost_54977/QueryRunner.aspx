<%@ page language="VB" autoeventwireup="false" inherits="QueryRunner, App_Web_54qm34zl" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title>Query runner</title>
<meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
<meta name="robots" content="index, follow, noarchive" />
<meta name="googlebot" content="noarchive" />
<link rel="stylesheet" type="text/css" media="screen" href="images/style.css" />
</head>
<body>
<form id="form1" runat="server">
   <asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>

<table width="100%">
    <tr>
        <td>
            <asp:Label runat="server" ID="label1" Text="Type Query Here">

            </asp:Label>
        </td>
    </tr> 
<tr>
<td align="left" style="width:100%">

    <asp:TextBox ID="txtQry" TextMode="MultiLine" Font-Size="Medium" Text="Type_here"  runat="server" Height="100px" 
        Width="95%"></asp:TextBox>

    <asp:Button ID="btnQuery" runat="server"  Text="Run Query" />

</td>
</tr>


<tr>
<td style="width:1000px">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlQry" width="1300px" Wrap="true"   ScrollBars="Both">
            <asp:GridView ID="gvData" runat="server">
            </asp:GridView>
                </asp:Panel> 
        </ContentTemplate>
    </asp:UpdatePanel>


</td>
</tr>
</table>

</form> 
</body> 
</html> 

