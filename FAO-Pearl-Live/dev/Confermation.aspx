<%@ Page Language="VB" AutoEventWireup="false" CodeFile="confermation.aspx.vb" Inherits="confermation" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title>Session Time Out</title>
<meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
<meta name="author" content="Manish Kumar - http://www.veasta.com" />
<meta name="robots" content="index, follow, noarchive" />
<meta name="googlebot" content="noarchive" />
<link rel="stylesheet" type="text/css" media="screen" href="images/style.css" />
</head>

<body>
<form id="form1" runat="server">
   <div id="wrap"><!-- wrap starts here -->
	<table width="100%" cellspacing = "0px" cellpadding="0px" border="0px">
    <tr>
    <td style="width:100%;height:17px" colspan="3" >
    <table width="100%" cellspacing = "0px" cellpadding="0px" border="0px">
    <tr>
    <td style="width:70%;">
        <asp:Label ID="lblLogo" runat="server"></asp:Label>
    </td>
    <td style="width:30%;text-align:left" valign="top">
        &nbsp;
    </td>
   </tr>
    </table>
    </td>
  </tr>
 <tr>
  
<td colspan="3" style="width:100%" valign="top">
    <span style="font-family:Verdana;font-size:20px;font-weight:bold"><asp:Label ID="lblMess" runat="server" Text="Thank You , Your Data Has been saved" ></asp:Label></span>
<br />
<br />
</td>
</tr>
<tr>
 
<td style="width:100%;border:3px double green;padding:5px;min-height:220px;" valign="top">
<div class="form">
   <%-- <h1>Your session has expired and you have been logged off from your Mynd SaaS account. Thanks for using our Business Process Management Application.</h1><br />--%>
    
    <br />
<h5>We recommend you to close this window or clear your browser cache for added security. This will ensure that any information that is cached (stored) on your browser is erased and cannot be viewed by others later.</h5>
<br />
<%--<h5>If you wish to access your account, you need to 
    <asp:LinkButton ID="btndefaultpage" runat="server">Login again</asp:LinkButton> </h5>--%>
</div>
</td>    
    </tr>
</table> 	
	
</div>
<!-- wrap ends here -->	
<br />
<!-- footer starts here -->	
	<div id="footer">
		<div id="footer-content">
<br />
<a href="http://www.myndsol.com">Copyright &copy; 2010-2013, Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a> 
		</div>	
	</div>
<!-- footer ends here -->	
 </form>
</body>
</html>
