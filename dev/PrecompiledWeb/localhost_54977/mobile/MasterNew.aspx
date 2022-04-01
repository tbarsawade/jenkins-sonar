<%@ page language="VB" autoeventwireup="false" inherits="mobile_MasterNew, App_Web_dzjktowh" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, usesscr-scalable=yes, minimum-scale=1.0, maximum-scale=1.0" />
    <title></title>
   <link href="styles/style.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="Sc1">
        </asp:ScriptManager>
        <div class="box">
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr style="color: #000000" height="20px">
                    <td style="text-align: left;"></td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updatePanelEdit" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                <asp:Panel ID="pnlFields" Width="100%" runat="server" Style="overflow: scroll;">
                                </asp:Panel>
                                <div style="width: 100%; padding-top: 10px; margin: 0 0 0 10px; text-align: left;">
                                    <asp:Button ID="btnActEdit" runat="server" Text="Save" OnClick="EditRecord" CssClass="btnNew"
                                        Font-Bold="True" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Button ID="Btnchild" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="Btnchild"
            PopupControlID="pnlPopupchild" CancelControlID="btnClose" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlPopupchild" runat="server" Width="700px" Height="420px" Style="display: none"
            BackColor="aqua">
            <div class="box" style="overflow: scroll;">
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="Label3" runat="server" Font-Bold="True"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnClose" ImageUrl="~/images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel1" runat="server">
                                        <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="Panel2" Width="100%" ScrollBars="auto" runat="server">
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right;">
                                        <asp:Button ID="Button1" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                            OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;"
                                            UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel3" runat="server" Width="700px" Height="420px" Style="display: none"
            BackColor="aqua">
            <div class="box" style="overflow: scroll;">
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Pnllable" runat="server">
                                        <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlFields1" Width="100%" ScrollBars="auto" runat="server">
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right;">
                                        <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                            OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;"
                                            UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
