<%@ page language="VB" autoeventwireup="false" inherits="QueryRunnerOIT, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title>MyeDMS Login</title>
<meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
<meta name="author" content="Manish Kumar - http://www.veasta.com" />
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
<td style="width:100%">
    <asp:TextBox ID="txtQry" TextMode="MultiLine"   runat="server" Height="100px" 
        Width="95%"></asp:TextBox>

    <asp:Button ID="btnQuery" runat="server" Text="Run Query" />

</td>
</tr>


<tr>
<td style="width:1200px">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlgrid" Width="1200px" ScrollBars="Auto"   >

            </asp:Panel>
            <asp:GridView ID="gvData" runat="server" Width="100%">
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>


</td>
</tr>
</table>

</form> 
</body> 
</html> 

