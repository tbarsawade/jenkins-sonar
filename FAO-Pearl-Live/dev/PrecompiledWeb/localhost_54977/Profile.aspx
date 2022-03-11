<%@ page title="My Profile" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="Profile, App_Web_sfds111l" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
    $(document).ready(function () {
        ("#footer").hide();
    });
    </script>    
    <div class="container-fluid container-fluid-amend">
        <div class="col-md-12">
            <div class="form nav nav-tabs NotificationsTabMain" style="height: 400px; margin: 0px!important;">
                <asp:TabContainer ID="changePassword" runat="server" ActiveTabIndex="0" Height="100%">
                    <asp:TabPanel ID="chpTab" runat="server" HeaderText="Change Password">
                        <HeaderTemplate>
                            <li>Change Password</li>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:Panel ID="pnlPWD" runat="server" DefaultButton="btnSave">
                                 <div class="doc_header">
                                My Profile (Change Password)
                            </div>
                            <asp:UpdatePanel ID="upChangePassword" runat="server">
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btnlogin" />
                                    <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="click" />
                                </Triggers>
                                <ContentTemplate>
                                    <div class="row text-center">
                                        <label style="color: red">
                                            <asp:Label ID="lblMsgSave" runat="server" Text=""></asp:Label>
                                        </label>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2 col-sm-2">
                                            &nbsp;
                                        </div>
                                        <div class="col-md-3 col-sm-3">
                                            <label>Current Password *</label>
                                        </div>
                                        <div class="col-md-4 col-sm-4">
                                            <asp:TextBox ID="txtCP" runat="server" CssClass="txtBox mg-top10"
                                                TextMode="Password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2 col-sm-2">
                                            &nbsp;
                                        </div>
                                        <div class="col-md-3 col-sm-3">
                                            <label>New Password *</label>
                                        </div>
                                        <div class="col-md-4 col-sm-4">
                                            <asp:TextBox ID="txtNP" runat="server" CssClass="txtBox mg-top10" TextMode="Password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2 col-sm-2">
                                            &nbsp;
                                        </div>
                                        <div class="col-md-3 col-sm-3">
                                            <label>Confirm New Password *</label>
                                        </div>
                                        <div class="col-md-4 col-sm-4">
                                            <asp:TextBox ID="txtRP" runat="server" CssClass="txtBox mg-top10" TextMode="Password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12" style="text-align: center;">
                                            <%-- <a id="anGPIN" style="color: blue;" runat="server" href="MPIN.aspx">Generate MPIN</a>--%>
                                            <%--<asp:LinkButton ID="lnkGPTIN"  runat="server" ForeColor="Blue">Generate MPIN</asp:LinkButton>--%>
                                            <asp:Button ID="btnSave" runat="server" ValidationGroup="b" OnClick="btnSave_Click" CssClass="btnNew" Text=" Change Password" />

                                        </div>
                                    </div>
                                    <asp:Panel runat="server" ID="pnlhide" Visible="false">
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <asp:CheckBox ID="chkMnMxPs" runat="server" />
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:Label ID="lblMinMaxChar" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <asp:CheckBox ID="chkPassType" runat="server" />
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:Label ID="lblPassType" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <h2>Change Picture</h2>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <asp:Label ID="lblImage" runat="server" Text=""></asp:Label>
                                            </div>

                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <label>Upload Image*</label>
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:FileUpload ID="flU" runat="server" CssClass="txtBox mg-top10" />
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:Button ID="btnlogin" runat="server" CssClass="btnNew"
                                                    Text="Change Image" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <h2>Change Settings </h2>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <label>OFF : </label>
                                            </div>
                                            <div class="col-md-8 col-sm-8">
                                                <asp:CheckBox ID="Mon" runat="server" Text="Monday" />
                                                <asp:CheckBox ID="Tus" runat="server" Text="Tuesday" />
                                                <asp:CheckBox ID="Wed" runat="server" Text="Wednesday" />
                                                <asp:CheckBox ID="Thus" runat="server" Text="Thursday" />
                                                <asp:CheckBox ID="Fri" runat="server" Text="Friday" />
                                                <asp:CheckBox ID="Sat" runat="server" Text="Saturday" />
                                                <asp:CheckBox ID="Sun" runat="server" Text="Sunday" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                <label>Location Name : </label>
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:DropDownList ID="ddlLocationName" runat="server" CssClass="txtBox mg-top10">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                From :
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:DropDownList ID="ddlFrom" runat="server">
                                                </asp:DropDownList></td>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2 col-sm-2">
                                                To :
                                            </div>
                                            <div class="col-md-4 col-sm-4">
                                                <asp:DropDownList ID="ddlTo" runat="server" CssClass="txtBox mg-top10">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row text-center">
                                            <asp:Button ID="btnSetting" runat="server" CssClass="btnNew"
                                                Text=" Change Setting" />
                                            <asp:Label ID="lblmessage" runat="Server" Text="" ForeColor="Red"></asp:Label>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </asp:Panel>
                           

                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="changePin" runat="server" HeaderText="Change PIN">
                        <HeaderTemplate>
                            <b>Change PIN</b>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:Panel ID="pnlPin" runat="server" DefaultButton="btnSaveMpin">
                                 <div class="doc_header">
                                My Profile (PIN GENERATION)
                            </div>
                            <div class="row text-center">
                                <label style="color: red">
                                    <asp:Label ID="lblMpinMsg" runat="server" Text=""></asp:Label>
                                </label>
                            </div>
                            <div class="row" id="CMPINROW" runat="server">
                                <div class="col-md-2 col-sm-2">
                                    &nbsp;
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>Current PIN *</label>
                                </div>
                                <div class="col-md-4 col-sm-4">
                                    <asp:TextBox ID="txtCMPIN" runat="server" CssClass="txtBox mg-top10"
                                        TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqCURMPIN" ForeColor="Red" runat="server" ControlToValidate="txtCMPIN" Display="None" ErrorMessage="Please enter current PIN" SetFocusOnError="true" ValidationGroup="a"></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="val_reqCURMPIN" Enabled="true" runat="server" TargetControlID="reqCURMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                    <asp:RegularExpressionValidator ID="Reg_txtCMPIN" ForeColor="Red" ValidationGroup="a" Display="None"
                                        ControlToValidate="txtCMPIN" runat="server" ErrorMessage="Enter 4 Numeric Digit PIN"
                                        SetFocusOnError="True" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
                                    <asp:ValidatorCalloutExtender ID="Val_Reg_txtCMPIN" runat="server" TargetControlID="Reg_txtCMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-2 col-sm-2">
                                    &nbsp;
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>New PIN *</label>
                                </div>
                                <div class="col-md-4 col-sm-4">
                                    <asp:TextBox ID="txtNMPIN" runat="server" CssClass="txtBox mg-top10" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqMPIN" Display="None" runat="server" ControlToValidate="txtNMPIN" ErrorMessage="Please enter PIN" Enabled="true" ForeColor="Red" ValidationGroup="a"></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="Val_reqMPIN" Enabled="true" runat="server" TargetControlID="reqMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                    <asp:RegularExpressionValidator ID="Reg_txtNMPIN" ForeColor="Red" ValidationGroup="a" Display="None"
                                        ControlToValidate="txtNMPIN" runat="server" ErrorMessage="Enter 4 Numeric Digit PIN"
                                        SetFocusOnError="True" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
                                    <asp:ValidatorCalloutExtender ID="Val_Reg_txtNMPIN" runat="server" TargetControlID="Reg_txtNMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-2 col-sm-2">
                                    &nbsp;
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>Confirm New PIN *</label>
                                </div>
                                <div class="col-md-4 col-sm-4">
                                    <asp:TextBox ID="txtRMPIN" runat="server" CssClass="txtBox mg-top10" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="req_txtRMPIN" runat="server" Display="None" ControlToValidate="txtRMPIN" ErrorMessage="Please enter confirm MPIN  " Enabled="true" ValidationGroup="a" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="Val_reqCONMPIN" Enabled="true" runat="server" TargetControlID="req_txtRMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                    <asp:CompareValidator ID="cmpMPIN" Display="None" ForeColor="Red" runat="server" ErrorMessage="New PIN and Confirm MPIN should be same" ControlToCompare="txtNMPIN" ControlToValidate="txtRMPIN" ValidationGroup="a"></asp:CompareValidator>
                                    <asp:ValidatorCalloutExtender ID="Val_cmpMPIN" Enabled="true" runat="server" TargetControlID="cmpMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                    <asp:RegularExpressionValidator ID="Reg_txtRMPIN" ForeColor="Red" ValidationGroup="a" Display="None"
                                        ControlToValidate="txtRMPIN" runat="server" ErrorMessage="Enter 4 Numeric Digit PIN"
                                        SetFocusOnError="True" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
                                    <asp:ValidatorCalloutExtender ID="Val_Reg_txtRMPIN" runat="server" TargetControlID="Reg_txtRMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                                </div>
                            </div>
                            <div class="row">
                                &nbsp;
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12" style="text-align: center;">
                                    <asp:Button ID="btnSaveMpin" runat="server" CssClass="btnNew" Text=" Change PIN" OnClick="btnSaveMpin_Click" ValidationGroup="a" />

                                </div>
                            </div>
                            </asp:Panel>
                           
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>

            </div>
        </div>
    </div>
    <div class="clear"></div>
</asp:Content>

