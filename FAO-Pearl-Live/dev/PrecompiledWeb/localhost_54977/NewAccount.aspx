<%@ page title="New Account" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="NewAccount, App_Web_o3dtvhns" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
        <tr>
            <td style="width: 100%;" valign="top" align="left">
                <div id="main">
                    <h1>
                        New Account Sign Up</h1>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="form">
                                <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            <br />
                                            <h2>
                                                Enter Company Information</h2>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *App Type [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:DropDownList ID="ddlAppType" runat="server" CssClass="txtBox" Width="208px" >
                                                <asp:ListItem Value="0">---Select---</asp:ListItem>
                                                <asp:ListItem Value="CAB">CAB</asp:ListItem>
                                                <asp:ListItem Value="FIELD FORCE">FIELD FORCE</asp:ListItem>
                                                <asp:ListItem Value="VMS">VMS</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Account Code [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtAcCode" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Account Name [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtAcName" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Default Folder [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:RadioButton ID="rdDef" GroupName="default" runat="server" Checked="true" Text="Create Default Folder" />&nbsp;&nbsp;&nbsp;
                                            <asp:RadioButton ID="rdDefNo" GroupName="default" runat="server" Text="Don't Create Default Folder" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            <br />
                                            <h2>
                                                Setup File Server</h2>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *File System [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:RadioButton ID="rdLocal" GroupName="file" runat="server" Checked="true" Text="Local File System" />&nbsp;&nbsp;&nbsp;
                                            <asp:RadioButton ID="rdServer" GroupName="file" runat="server" Text="FTP Server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Server IP Address [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtIPAddress" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Server User ID [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtServerUserID" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Server Password [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtServerPWD" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            <br />
                                            <h2>
                                                Enter your Email Address & Create a Password</h2>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Super user Name [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtUserName" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Email [?] :
                                            </label>
                                        </td>
                                        <td style="" valign="middle">
                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                            <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender6" runat="server" Enabled="True"
                                                WatermarkCssClass="waterClass" WatermarkText="This Email will be used as UserID"
                                                TargetControlID="txtEmail">
                                            </asp:TextBoxWatermarkExtender>
                                            <%-- &nbsp;<asp:Label ID="lblEmailChk" runat="server"></asp:Label>--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Create Password [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtPWD" runat="server" CssClass="txtBox" Width="200px" TextMode="Password"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Re-Type Password [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtRePwd" runat="server" CssClass="txtBox" Width="200px" TextMode="Password"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnlogin" runat="server" CssClass="btnNew" Text=" Create My Account " />
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
