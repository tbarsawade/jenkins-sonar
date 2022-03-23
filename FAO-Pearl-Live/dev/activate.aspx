<%@ Page Language="VB" AutoEventWireup="false" CodeFile="activate.aspx.vb" Inherits="activate1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Create/Reset Password</title>
    <meta http-equiv="content-type" content="application/xhtml+xml; charset=UTF-8" />
    <meta name="author" content="Manish Kumar - http://www.veasta.com" />
    <meta name="robots" content="index, follow, noarchive" />
    <meta name="googlebot" content="noarchive" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="EXPIRES" content="-1" />
    <link rel="stylesheet" type="text/css" media="screen" href="images/style.css" />
    <script src="Jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <style>
        .BarIndicator_TextBox2_weak {
            color: Red;
            background-color: Red;
        }

        .BarIndicator_TextBox2_average {
            color: Blue;
            background-color: Blue;
        }

        .BarIndicator_TextBox2_good {
            color: Green;
            background-color: Green;
        }

        .BarBorder_TextBox2 {
            border-style: solid;
            border-width: 1px;
            padding: 2px 2px 2px 2px;
            width: 200px;
            vertical-align: middle;
        }
    </style>
     <script type="text/javascript">
         function myTestFunction(msg) {
             //if (confirm(msg + '?')) {
             //    window.location.href = 'Default.aspx';
             //    return true;
             //}
             //else {
             //    window.location.href = 'Default.aspx';
             //    return false;
             //}

             alert(msg);
             window.location.href = 'Default.aspx';
         }

          
         
    </script>
</head>

<body>
     <%--<script type="text/javascript">
         //function myTestFunction(msg) {
         //    if (confirm(msg+'?')) {
         //        return true;
         //    }
         //    else {
         //        return false;
         //    }
         //}

         function myTestFunction(msg) {
             alert(msg);
         }
         //function myTestFunction() {
         //    if (confirm('vivek ?')) {
         //        return true;
         //    }
         //    else {
         //        return false;
         //    }
         //}
    </script>--%>
    <form id="form1" runat="server" autocomplete="off" method="post">

        <div id="wrap" runat="server">
            <!-- wrap starts here -->
            <table width="100%" cellspacing="0px" cellpadding="0px" border="0px">
                <tr>
                    <td style="width: 100%; height: 17px" colspan="3">

                        <table width="100%" cellspacing="0px" cellpadding="0px" border="0px">
                            <tr>
                                <td style="width: 70%;">
                                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                                    </asp:ScriptManager>
                                    <asp:Label ID="lblLogo" runat="server"></asp:Label>

                                </td>
                                <td style="width: 30%; text-align: left" valign="top">&nbsp;</td>

                            </tr>

                            <tr>

                                <td colspan="2" style="width: 100%" valign="top">
                                    <span style="font-family: Verdana; font-size: 20px; font-weight: bold">Password Reset</span>
                                    <br />
                                    <br />
                                </td>
                            </tr>

                        </table>
                    </td>

                </tr>
            </table>


            <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                <tr>
                    <td style="width: 100%; border: 3px double green; padding: 5px;" valign="top">
                        <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">

                            <tr>

                                <td style="width: 100%" colspan="2" align="left">
                                    <div class="form" style="min-height:0px !important;">
                                        <h1>
                                            <%--<asp:Label ID="lblmsg" Text="Please enter new password hereunder for Activation of account\reset existing forgotten one. We also recommend you to take note of the mandatory conditions for password change." runat="server">  </asp:Label>--%>
                                             <asp:Label ID="lblmsg" Text="Setup a new Password or reset the existing one for login. The Password field is case sensitive." runat="server">  </asp:Label>
                                        </h1>
                                        <br />


                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">


                                    <label style="color: red">
                                        <asp:Label ID="lblMsgSave" Font-Size="10pt" runat="server" Text=""></asp:Label>
                                    </label>
                                </td>

                            </tr>



                            <tr>
                                <td style="text-align: right">
                                    <label>*Enter  Password [?] : </label>
                                </td>
                                <td style="">
                                    <asp:TextBox ID="txtNP" ReadOnly="true" autocomplete="off" onfocus="$(this).removeAttr('readonly');" runat="server" CssClass="txtBox" Width="250px" TextMode="Password"></asp:TextBox>

                                    <asp:Label ID="TextBox2_HelpLabel" runat="server" />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right">
                                    <label>*Confirm  Password [?] : </label>
                                </td>
                                <td style="">
                                    <asp:TextBox ID="txtRP" ReadOnly="true" autocomplete="off" onfocus="$(this).removeAttr('readonly');" runat="server" CssClass="txtBox"
                                        Width="250px" TextMode="Password"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td style="text-align: right"></td>
                                <td style="">
                                    <asp:Button ID="btnSave" runat="server" Width="80px" CssClass="btnNew"
                                        Text=" Submit" />
                                </td>
                            </tr>


                            <tr>
                                <td style="text-align: right">&nbsp;</td>
                                <td style="">
                                    <asp:Label Style="color: red;" Font-Size="10pt" ID="lblmes" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right">&nbsp;</td>
                                <td style="">
                                    <a href="https://hfcl.myndsaas.com">
                                        <asp:Label Style="color: blue" ID="lbllink" runat="server" Visible="false"> hfcl.myndsaas.com  </asp:Label>
                                    </a>
                                </td>
                            </tr>

                            <ajaxToolkit:PasswordStrength ID="PasswordStrength2" runat="server" TargetControlID="txtNP"
                                DisplayPosition="RightSide"
                                StrengthIndicatorType="BarIndicator"
                                HelpStatusLabelID="TextBox2_HelpLabel"
                                StrengthStyles="BarIndicator_TextBox2_weak;BarIndicator_TextBox2_average;BarIndicator_TextBox2_good"
                                BarBorderCssClass="BarBorder_TextBox2"
                                TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent" />


                            <tr>
                                <td style="text-align: right">
                                    <asp:CheckBox ID="chkMnMxPs" runat="server" />
                                </td>
                                <td style="text-align: left">
                                    <asp:Label ID="lblMinMaxChar" runat="server"></asp:Label></td>
                            </tr>

                            <tr>
                                <td style="text-align: right">
                                    <asp:CheckBox ID="chkPassType" runat="server" /></td>
                                <td style="text-align: left">
                                    <asp:Label ID="lblPassType" runat="server"></asp:Label></td>
                            </tr>

                            <tr>
                                <td style="text-align: right">&nbsp;</td>
                                <td style="">&nbsp;</td>
                            </tr>

                        </table>
                    </td>
                </tr>
            </table>

        </div>
        <!-- wrap ends here -->
        <br />
        <!-- footer starts here -->
        <%--<div id="footer">--%>
        <div id="deleted">
            <div id="footer-content">


                <%--<a href="#">Privacy Policy</a> - <a href="#">Terms of Use</a> 
<br />
<a href="http://www.myndsol.com">Copyright Notice © 2010-2013 myndsol.com. Designed and Developed by Mynd Integrated Solutions . All rights reserved.</a> --%>
            </div>
        </div>
        <!-- footer ends here -->

    </form>
    <script type="text/javascript">
        function disableautocompletion(id) {
            var passwordControl = document.getElementById(id);
            passwordControl.setAttribute("autocomplete", "off");
        }
    </script>
    
</body>
</html>

