<%@ page language="VB" autoeventwireup="false" inherits="SignOut, App_Web_ds2n0wlx" viewStateEncryptionMode="Always" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>MyeDMS Login</title>
    <meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
    <meta name="robots" content="index, follow, noarchive" />
    <meta name="googlebot" content="noarchive" />
    <link href="css/signoutcss.css" rel="stylesheet" />
    <link href="App_Themes/Default/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="signout">
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
            <div class="container">
                <div class="row">
                    <div class="col-md-6 col-sm-6">
                        <div class="txt">
                            You have successfully logged off from your account. Thanks for using
                        <asp:Label ID="lblmsg" runat="server"></asp:Label>
                            System.
                        </div>
                        <div class="txt">We recommend you to close this window or clear your browser cache for added security. This will ensure that any information that is cached (stored) on your browser is erased and cannot be viewed by others later.</div>

                        <div class="txt">
                            To access your account again, you need to
                        <asp:LinkButton ID="btndefaultpage" Font-Size="Large" class="login" runat="server">Login again.</asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-md-6 col-sm-6">
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6 col-sm-6">
                        <h3>Logout Summary </h3>
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>Logged In Time</th>
                                    <th>Logged Off Time</th>
                                    <th>Duration</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td> <asp:Label ID="lblIntime" runat="server" Text="Label"></asp:Label></td>
                                    <td><asp:Label ID="lblOutTime" runat="server" Text="Label"></asp:Label></td>
                                    <td><asp:Label ID="lblDuration" runat="server" Text="Label"></asp:Label></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="col-md-6 col-sm-6"></div>
                </div>
            </div>
        </section>


        <%-- <div id="wrap">

            <table width="100%" cellspacing="0px" cellpadding="0px" border="0px">
                <tr>
                    <td style="width: 100%; height: 17px" colspan="3">
                        <table width="100%" cellspacing="0px" cellpadding="0px" border="0px">
                            <tr>
                                <td style="width: 70%;"></td>
                                <td style="width: 30%; text-align: left" valign="top">&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>

                    <td colspan="3" style="width: 100%" valign="top">
                        <span style="font-family: Verdana; font-size: 20px; font-weight: bold">Logged Off</span>
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>

                    <td style="width: 100%; border: 3px double green; padding: 5px; min-height: 220px;" valign="top">
                        <div class="form">
                            <h1>
                                <asp:Label ID="lblmsg" runat="server">  </asp:Label>
                            </h1>
                            <br />
                            <br />
                            <h5>We recommend you to close this window or clear your browser cache for added security. This will ensure that any information that is cached (stored) on your browser is erased and cannot be viewed by others later.</h5>
                            <br />
                            <h5>If you wish to access your account,  you need to
                               
                            </h5>
                        </div>
                    </td>

                </tr>
                <tr>

                    <td style="width: 100%; border: 3px double green; padding: 0px;" valign="top">
                        <div class="form">
                            Logout Summary
    <table width="90%" cellspacing="1px" cellpadding="1px" border="1px">
        <tr>
            <td style="width: 35%" align="center">
                <h5>Logged In Time</h5>
            </td>

            <td style="width: 35%" align="center">
                <h5>Logged Off Time</h5>
            </td>

            <td align="center">
                <h5>Duration</h5>
            </td>

        </tr>

        <tr>
            <td align="center">
                <h5>
                    <asp:Label ID="lblIntime" runat="server" Text="Label"></asp:Label>
                </h5>
            </td>

            <td align="center">
                <h5>
                    <asp:Label ID="lblOutTime" runat="server" Text="Label"></asp:Label>
                </h5>
            </td>

            <td align="center">
                <h5>
                    <asp:Label ID="lblDuration" runat="server" Text="Label"></asp:Label>
                </h5>
            </td>

        </tr>
    </table>


                        </div>

                    </td>
                </tr>
            </table>

        </div>--%>
        <!-- wrap ends here -->

        <!-- footer starts here -->
        <%--   <div id="footer">
            <div id="footer-content">
                <br />
                <a href="http://www.myndsol.com">Copyright &copy;
                    <asp:Label runat="server" ID="lblYear"></asp:Label>, Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a>
            </div>
        </div>--%>
        <!-- footer ends here -->

    </form>
    <footer>
        <div class="container-fluid ftrbg">
            <div class="container">
                Copyright © <span id="lblYear" runat="server"></span> Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.
            </div>
        </div>
        <footer>
            <%-- <div class="container copyrt" style="color:#333; font-family:sans-serif; font-size:14px;text-align:center;">
        <a href="http://www.myndsol.com">Copyright © <span id="lblYear">2013-2017</span> Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a>
    </div>--%>
</body>

</html>

