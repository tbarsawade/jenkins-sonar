<%@ page title="Manage Account" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ManageAccount, App_Web_4ysr50e5" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <ContentTemplate>

            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"></asp:Label>
                    </td>
                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double lime">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="width: 90px;">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Field Name" Width="99%">
                                    </asp:Label>
                                </td>

                                <td style="width: 170px;">
                                    <asp:DropDownList ID="ddlField" runat="server" CssClass="Inputform" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Width="99%">
                                    </asp:DropDownList>
                                </td>

                                <td style="width: 50px;">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Value" Width="99%"></asp:Label>
                                </td>

                                <td style="width: 200px;">
                                    <asp:TextBox ID="txtValue" runat="server" CssClass="Inputform" Font-Bold="True"
                                        Font-Size="Small" Width="99%"></asp:TextBox>
                                </td>

                                <td style="text-align: right; width: 25px">
                                    <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                        ImageUrl="~/Images/search.png" />
                                </td>

                                <td style="text-align: right;">&nbsp;
                                </td>

                            </tr>
                        </table>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>

                    </td>
                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left;" valign="top">
                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                            CellPadding="2" DataKeyNames="eid"
                            ForeColor="#333333" Width="100%"
                            AllowSorting="True" DataSourceID="SqlData"
                            PageSize="20">
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#EFF3FB" />
                            <EditRowStyle BackColor="#2461BF" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>

                                <asp:TemplateField HeaderText="S.No">
                                    <ItemTemplate>
                                        <%# CType(Container, GridViewRow).RowIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Code" HeaderText="Acc. Code">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="name" HeaderText="Acc. Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>


                                <asp:BoundField DataField="UserName" HeaderText="Super User">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="emailid" HeaderText="User ID">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>


                                <asp:BoundField DataField="ipaddress" HeaderText="IP Address">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="status" HeaderText="Status">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnLock" ImageUrl="~/images/lock.png" runat="server" Height="16px" Width="16px" OnClick="LockHit" ToolTip="Lock / Unlock Customer" AlternateText="Lock / Unlock" />
                                        &nbsp;
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" AlternateText="Edit" />
                                        &nbsp;
                                <asp:ImageButton ID="btnTicket" runat="server" ImageUrl="~/images/TicketSupport.png" Height="16px" Width="16px" OnClick="AddTicket" AlternateText="Add Ticket Configuration" ToolTip="Add Ticket Configuration" />
                                    </ItemTemplate>
                                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlData" runat="server"
                            ConnectionString="<%$ ConnectionStrings:conStr %>"
                            SelectCommand="uspGetResultAccount" SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlField" Name="sField"
                                    PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"
                                    Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--Configuration ticket support--%>
    <asp:Button ID="btnAddTicketSetting" runat="server" Style="display: none;" />
    <asp:ModalPopupExtender ID="btnTicket_ModalPopupExtender" runat="server" TargetControlID="btnAddTicketSetting" PopupControlID="pnlPopupTicket" CancelControlID="btnCloseTicket" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupTicket" runat="server" Width="600px" Style="Display: none" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>Add Ticket Account Configuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseTicket"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:UpdatePanel ID="UpdatePanelTicket" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td colspan="2" align="left">
                                            <asp:Label ID="lblMsgTicket" runat="server" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="padding: .5%;"><b>User EmailID</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtUserEmailID" runat="server" CssClass="txtBox"></asp:TextBox>
                                        </td>
                                        <td align="left"><b>Password</b></td>
                                        <td align="right" style="padding-right: .5%;">
                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="txtBox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="padding: .5%;"><b>Port Number</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPortNumber" runat="server" CssClass="txtBox"></asp:TextBox>
                                        </td>
                                        <td align="left"><b>Host Name</b></td>
                                        <td align="right" style="padding-right: .5%;">
                                            <asp:TextBox ID="txtHostName" runat="server" CssClass="txtBox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="padding: .5%;"><b>Is Allow Create User</b></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkIsAllowCreateUser" runat="server" />
                                        </td>
                                        <td align="left"><b>SSL </b></td>
                                        <td align="left" style="padding-right: .5%;">
                                            <asp:CheckBox ID="chkSSL" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="padding: .5%;"><b>Is Active</b></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkIsActive" runat="server" />
                                        </td>
                                        <td align="left"><b>RoleMatrix From Organization</b></td>
                                        <td align="left" style="padding-right: .5%;">
                                            <asp:CheckBox ID="chkRoleMatrix" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <asp:Button ID="btnSave" runat="server" Text="Create HelpDesk Account" CssClass="btnNew" OnClick="btnSave_Click" />
                                        </td>
                                    </tr>
                                   <tr>
                                        <td colspan="4">&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />

    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupEdit" runat="server" Width="600px" Style="Display: none" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>Edit Account</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td colspan="2" align="left">
                                            <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Account Code</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtAccCode" runat="server" Enabled="false" Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="left" style="width: 125px">
                                            <b>Account Name</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtAccName" runat="server" Width="98%"></asp:TextBox>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Server IP</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtServerIP" runat="server" Width="98%"></asp:TextBox></td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Server User ID</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtServeruserID" runat="server" Width="98%"></asp:TextBox></td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Server Pwd</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtServerPWD" runat="server" Width="98%"></asp:TextBox></td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Super User Name</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtSUName" runat="server" Width="98%"></asp:TextBox>
                                        </td>
                                    </tr>


                                    <tr>
                                        <td style="width: 125px" align="left"><b>Super User ID</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtSUID" runat="server" Width="98%"></asp:TextBox>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Super User PWD</b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtSUPwd" runat="server" Width="98%"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 125px" align="left"><b>
                                            <asp:Label ID="lblAppKeyName" runat="server" Width="98%" Visible="false" Text="Your API Key"></asp:Label>
                                        </b></td>
                                        <td align="left">
                                            <asp:Label ID="lblApiKey" runat="server" Width="98%" Visible="false"></asp:Label>
                                        </td>
                                    </tr>



                                </table>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnGenApiKey" runat="server" Text="Generate Key" OnClick="GenerateKey"
                                        CssClass="btnNew" Font-Bold="True" Visible="false"
                                        Font-Size="X-Small" Width="100px" />
                                    <asp:Button ID="btnActEdit" runat="server" Text="Update"
                                        OnClick="EditRecord" CssClass="btnNew" Font-Bold="True"
                                        Font-Size="X-Small" Width="100px" />

                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopuplock" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnlock_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopuplock" PopupControlID="pnlPopuplock"
        CancelControlID="btnCloselock" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupLock" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Entity : Lock / Unlock</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseLock" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelLock" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsgLock" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="97%" Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActLock" runat="server" Text="Yes Proceed" Width="90px"
                                        OnClick="LockUser" CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

</asp:Content>

