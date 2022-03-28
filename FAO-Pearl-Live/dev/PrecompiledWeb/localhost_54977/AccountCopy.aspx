<%@ page title="AccountCopy" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="NewAccount, App_Web_0gl03q5k" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
        <tr>
            <td style="width: 100%;" valign="top" align="left">
                <div id="main">
                    <h1>Account Creation Through Cloning</h1>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <%--   <Triggers>
                            <asp:PostBackTrigger ControlID="btnSave" />
                        </Triggers>--%>
                        <ContentTemplate>
                            <div class="form">
                                <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            
                                            <h2>Enter Company Information</h2>
                                            
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            <asp:Label ID="lblMsg" runat="server" ForeColor="red" Font-Bold="True" Font-Size="Small" Text="" Visible="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Select Account to be Cloned :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:DropDownList ID="ddlentityname" runat="server" CssClass="txtBox" Width="208px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <%--          <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Item to Copy:
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:DropDownList ID="ddlitemcopy" runat="server" CssClass="txtBox" Width="208px">
                                                <asp:ListItem>Select</asp:ListItem>
                                                <asp:ListItem>Form</asp:ListItem>
                                                <asp:ListItem>Fill</asp:ListItem>
                                                <asp:ListItem>Validation</asp:ListItem>
                                                <asp:ListItem>Trigger</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>--%>
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
                                        <td style="text-align: right"></td>
                                        <td style="">&nbsp;&nbsp;
                                        </td>
                                    </tr>

                                    </tr>
                                    <tr>
                                        <td style="width: 100%;" colspan="2">
                                            <br />
                                            <h2>Enter your Email Address &amp; Create a Password</h2>
                                            <br />
                                        </td>

                                        <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Super User Email ID[?] :
                                            </label>
                                        </td>
                                        <td style="" valign="middle">
                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                            <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender6" runat="server" Enabled="True"
                                                WatermarkCssClass="waterClass" WatermarkText="Type your Email ID here"
                                                TargetControlID="txtEmail">
                                            </asp:TextBoxWatermarkExtender>
                                        </td>
                                    </tr>
                                        <tr>
                                            <td style="text-align: right">
                                                <label>
                                                    *Super User Login ID [?] :
                                                </label>
                                            </td>
                                            <td style="">
                                                <asp:TextBox ID="txtUserName" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <%--  <tr>
                                            <td style="text-align: right">
                                                <label>
                                                *Email [?] :
                                                </label>
                                            </td>
                                            <td style="" valign="middle">
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox>
                                                <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender6" runat="server" Enabled="True" TargetControlID="txtEmail" WatermarkCssClass="waterClass" WatermarkText="This Email will be used as UserID">
                                                </asp:TextBoxWatermarkExtender>
                                                 &nbsp;<asp:Label ID="lblEmailChk" runat="server"></asp:Label> </td>
                                        </tr> --%>
                                        <tr>
                                            <td style="text-align: right; height: 29px;">
                                                <label>
                                                    *Default Password [?] :
                                                </label>
                                            </td>
                                            <td style="height: 29px">
                                                <asp:TextBox ID="txtPWD" runat="server" CssClass="txtBox" Text="1@million"  Enabled="false"  Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <%--<tr>
                                            <td style="text-align: right">
                                                <label>
                                                    *Re-Type Password [?] :
                                                </label>
                                            </td>
                                            <td style="">
                                                <asp:TextBox ID="txtRePwd" runat="server" CssClass="txtBox" TextMode="Password" Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td>
                                                &nbsp;</td>
                                            <td align="left">
                                                <asp:Button ID="btnSave" runat="server" CssClass="btnNew" Text=" Create My Account " />
                                                <%--<asp:Button ID="btnfield" runat="server" CssClass="btnNew" Text="Upadte Field" />--%>
                                                <%-- <asp:Button ID="btnvalidation" runat="server" CssClass="btnNew" Text="Upadte Validation Form" />--%>
                                                <asp:Label ID="lblmsgg" runat="server" Visible="false"></asp:Label>
                                                <br />
                                            </td>
                                        </tr>
                                    </tr>
                                </table>
                            </div>
                            <div>
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="100">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
