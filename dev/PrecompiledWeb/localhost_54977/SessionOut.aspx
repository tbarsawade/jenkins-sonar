<%@ page language="VB" autoeventwireup="false" inherits="SeesionOut, App_Web_ds2n0wlx" viewStateEncryptionMode="Always" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Session Time Out</title>
    <meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
    <meta name="author" content="Manish Kumar - http://www.veasta.com" />
    <meta name="robots" content="index, follow, noarchive" />
    <meta name="googlebot" content="noarchive" />
    <link href="css/signoutcss.css" rel="stylesheet" />
    <link href="App_Themes/Default/bootstrap.min.css" rel="stylesheet" />
</head>

<body class="session">
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <header>
            <div class="container-fluid headbg">
                <a href="#">
                    <asp:Label ID="lblLogo" runat="server"></asp:Label>
                </a>
            </div>
        </header>
        <section>
            <div class="row">
             <div class="container">
                <div class="row">
                    <div class="col-md-6 col-sm-6">
                        <div class="txt" style="margin-top:100px;">
                            Your session has expired and you have been logged off from your account. Thanks for using 
                        <asp:Label ID="lblmsg" runat="server"></asp:Label>
                            System.
                        </div>
                        <div class="txt">We recommend you to close this window or clear your browser cache for added security. This will ensure that any information that is cached (stored) on your browser is erased and cannot be viewed by others later.</div>

                        <div class="txt">
                            access your account, you need to
                        <asp:LinkButton ID="btndefaultpage" Font-Size="Large" class="login" runat="server">Login again.</asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-md-6 col-sm-6">
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
