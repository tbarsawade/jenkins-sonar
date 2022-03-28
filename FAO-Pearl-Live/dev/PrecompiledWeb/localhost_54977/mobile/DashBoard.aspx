<%@ page language="VB" autoeventwireup="false" inherits="mobile_DashBoard, App_Web_towehstj" viewStateEncryptionMode="Always" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, usesscr-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
    <title>::Dash Board::</title>
    <link rel="stylesheet" type="text/css" href="styles/Site.css" />
    <link rel="stylesheet" type="text/css" href="styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <table width="100%" border="0" cellspacing="2px" cellpadding="2px">
                <tr>
                    <td style="width: 33%;height:100px; text-align:center;background-color:#598526;">
                        <a href="../mobile/Pending.aspx" style="text-decoration:none;">Need To Act(<asp:Label runat="server" ID="lblNeedToAct" ></asp:Label>)</a>
                    </td> 
                    <td style="width: 33%;height:100px; text-align:center;background-color:#598526;">
                        <a href="../mobile/MyRequest.aspx" style="text-decoration:none;">My Request(<asp:Label runat="server" ID="lblRequest" ></asp:Label>)</a>
                    </td> 
                    
                    <td style="width: 33%;height:100px; text-align:center;background-color:#598526;">
                        <a href="../mobile/DocumentHistroy.aspx" style="text-decoration:none;">Histroy(<asp:Label runat="server" ID="lblHistroy" ></asp:Label>)</a>
                    </td> 
                </tr> 
            </table>
        </div>
    </form>
</body>
</html>
