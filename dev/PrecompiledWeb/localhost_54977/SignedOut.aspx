<%@ page language="VB" autoeventwireup="false" inherits="SignedOut, App_Web_ds2n0wlx" viewStateEncryptionMode="Always" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Logged IN Another Machine/Browser</title>
    <meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
    <meta name="robots" content="index, follow, noarchive" />
    <meta name="googlebot" content="noarchive" />
    <link href="css/signoutcss.css" rel="stylesheet" />
    <link href="App_Themes/Default/bootstrap.min.css" rel="stylesheet" />
</head>

<body style="background-color:#398DE4;">
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <header>
            <div class="container-fluid headbg">
               <%-- <a href="#">
                    <asp:Label ID="lblLogo" runat="server"></asp:Label>
                </a>--%>
            </div>
        </header>
        <section>
            <div class="row" style= text-align:center;color:#fff; " >
             <div class="container">
                <div class="row">
                    <div class="col-md-12 col-sm-12">
                        <div class="txt" style="margin-top:100px; text-align:center;color:#fff;font-size:large;">
                           <b>Your User ID is logged in from another system/browser, hence you have been logged out. </b>
                               </div>
                        <div class="txt" style= text-align:center;color:#fff;font-size:large; " >
                            Access your account, you need to
                        <asp:LinkButton ID="btndefaultpage" Font-Size="Larger" class="login" runat="server">Login again.</asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-md-12 col-sm-12">
                    </div>
                </div>
            </div>
            </div>
        </section>
        <div class="container-fluid ftrbg" style="background-color:#398DE4;">
            <div class="container">
                <%--<a href="http://www.myndsol.com" style="color: #fff;text-decoration: none;">--%>Copyright © <span id="lblYear" runat="server"></span>Mynd Solutions Pvt. Ltd. All rights reserved.<%--</a>--%>
            </div>
        </div>

    </form>


    <%-- <div class="container copyrt" style="color:#333; font-family:sans-serif; font-size:14px;text-align:center;">
        <a href="http://www.myndsol.com">Copyright © <span id="lblYear">2013-2017</span> Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a>
    </div>--%>
</body>
<%--   <div id="wrap"><!-- wrap starts here -->
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
    <span style="font-family:Verdana;font-size:20px;font-weight:bold">Session Time Out</span>
<br />
<br />
</td>
</tr>
<tr>
 
<td style="width:100%;border:3px double green;padding:5px;min-height:220px;" valign="top">
<div class="form">
   
    <h1><asp:Label ID="lblMsg" runat="server"></asp:Label></h1><br />
    
    <br />
<h5>We recommend you to close this window or clear your browser cache for added security. This will ensure that any information that is cached (stored) on your browser is erased and cannot be viewed by others later.</h5>
<br />
<h5>If you wish to access your account, you need to 
    <asp:LinkButton ID="btndefaultpage" Font-Size ="Large" runat="server">Login again</asp:LinkButton> </h5>
</div>
</td>    
    </tr>
</table> 	
	
</div>--%>
<!-- wrap ends here -->

<!-- footer starts here -->
<%--	<div id="footer">
		<div id="footer-content">
<br />
<a href="http://www.myndsol.com">Copyright &copy; <asp:Label runat="server" ID="lblYear"></asp:Label> Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a> 
		</div>	
	</div>--%>
<!-- footer ends here -->
<%-- </form>--%>
<%--</body>--%>
</html>
