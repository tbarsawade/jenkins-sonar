<%@ page language="VB" autoeventwireup="false" inherits="DeviceCheck, App_Web_nfrpb0kv" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title>MyndSaas Login</title>
<meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
<meta name="author" content="Manish Kumar - http://www.myndsol.com" />
<meta http-equiv="X-UA-Compatible" content="IE-9" />
<meta name="robots" content="index, follow, noarchive" />
<meta name="googlebot" content="noarchive" />
<link rel="stylesheet" type="text/css" media="screen" href="css/style.css" />
    <style type="text/css">
        .style1
        {
            font-size: 10px;
        }
    </style>
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

        &nbsp;</td>
        &nbsp;
    </tr>
    </table>
    </td>
  </tr>
 <tr>
  
<td colspan="3" style="width:100%;padding-top:4px" valign="top">
      <h1><asp:Label ID="lblmsgName" runat="server"></asp:Label></h1>

    <%--<h1>Welcome To Mynd SaaS: Business Process Management Application</h1>--%>

<br />
<br />
 <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">

    <tr>
  
<td style="width:750px;" valign="top" align="left">
	<div id="main">
               <asp:Label ID="lblHeaderLogo" runat="server"></asp:Label>
	           </div>	
</td>    


<td style="width:240px;border:3px double green;padding:5px;min-height:220px" valign="top">

    
<div class="form">
<asp:UpdatePanel ID="updTV" runat="server" UpdateMode="always" >
    <ContentTemplate>

<table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
<tr>
<td style="width:100%;font-family: 'Trebuchet MS', Tahoma, Verdana, Sans-serif;font-weight: Bold;font-size: 14px;text-transform: uppercase;padding:5px;margin:0px;	border-bottom:3px double lime;text-align : center;">Device Check</td></tr>

<tr><td style="width:100%"><label>IMEI NO</label></td></tr>

<tr><td style="width:100%"> 
    <asp:TextBox ID="txtUserID" runat="server" Width="93%"></asp:TextBox>
    </td></tr>
<tr>
<td align="right" style="width:100%;padding:10px 1px 0px 20px;height:45px">
   <asp:Button ID="btnlogin" runat="server" CssClass="btnNew" Text=" &nbsp; Test &nbsp;" />
    <br />
       <asp:Button ID="btnDelete" runat="server" CssClass="btnNew" Text="Delete Test Data, Disconnect Device First" Visible="False" />
    
     <br />
  </td>
</tr>

    <asp:Label ID="lblError" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="#333300"></asp:Label>

</table>


    </ContentTemplate> 
</asp:UpdatePanel> 

  
  <table align="center"> 
  <tr> 
   <td align="center">
  <span  class="style1"></span> 
   </td>
  </tr>
  </table>

</div>
  
 

</td>    
  
    </tr>
   </table> 
  </td>   
      
 </tr>

</table> 	
	
</div><!-- wrap ends here -->	
<br />

<!-- footer starts here -->	
	<div id="footer">
		<div id="footer-content">

 
<br />
<a href="http://www.myndsol.com">Copyright &copy; 2010-2013, Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a></div>	
	</div>
<!-- footer ends here -->	
 </form>
</body>
</html>