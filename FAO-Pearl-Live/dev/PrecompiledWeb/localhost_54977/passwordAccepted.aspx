﻿<%@ page language="VB" autoeventwireup="false" inherits="passwordAccepted, App_Web_apcyhbvt" viewStateEncryptionMode="Always" %>
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
    <span style="font-family:Verdana;font-size:20px;font-weight:bold">Password Accepted</span>
<br />
<br />
</td>
</tr>
<tr>
 
<td style="width:100%;border:3px double green;padding:5px;min-height:220px;" valign="top">
<div class="form">
    <h1><asp:Label ID="lblMsg" runat="server"></asp:Label> 
   </h1><br />
    <br />
<h5>We recommend you to close this window or clear your browser cache for added security. This will ensure that any information that is cached (stored) on your browser is erased and cannot be viewed by others later.</h5>
<br />
<h5>If you wish to access your account, you need to 
    <asp:LinkButton ID="btndefaultpage" runat="server" ForeColor="Red" Font-Underline="true" Font-Size="Large" >Click Here to Login</asp:LinkButton> 

    
    </h5>
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
<%--<a href="http://www.myndsol.com">Copyright Notice © 2010-2012 myndsol.com. Developed and Designed by Mynd Integrated Solutions . All rights reserved.</a> --%>
		</div>	
	</div>
<!-- footer ends here -->	
 </form>
</body>
</html>

