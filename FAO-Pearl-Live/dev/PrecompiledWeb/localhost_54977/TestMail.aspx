<%@ page language="VB" autoeventwireup="false" inherits="TestMail, App_Web_yp33scrq" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("button").click(function () {
                $("#").hide();
            });
        });
    </script>
</head>
<form id="form1" runat="server">
Name:
<input type="text" id="name" value="10" size="25" maxlength="50" />
Email:
<input type="text" id="email" value="20" size="25" maxlength="50" />
<input type="button" id="submit" value="sign me up!" onclick="FormSubmit();" />

<asp:Button id="Run" runat="server" Text="Run"></asp:Button>

<br />
<p>
    Jquery
</p>
<br />
</form>
</html>
