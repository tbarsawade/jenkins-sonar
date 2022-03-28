<%@ page language="VB" autoeventwireup="false" inherits="DeviceLastSignal, App_Web_jrsibs13" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .link
        {
            color: #35945B;
        }
        .link:hover
        {
            font: 15px;
            color: green;
            background: yellow;
            border: solid 1px #2A4E77;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="updPnlGrid" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnViewInExcel" />
            </Triggers>
        </asp:UpdatePanel>
        &nbsp;&nbsp;&nbsp;
        <table width="100%">
            <tr>
                <td align="center">
                    <asp:LinkButton ID="btnViewInExcel" runat="server" CssClass="link"> <b>Click to download last signal report</b></asp:LinkButton>
                </td>
            </tr>
        </table>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
