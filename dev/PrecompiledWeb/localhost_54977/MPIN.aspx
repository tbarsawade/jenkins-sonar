<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="MPIN, App_Web_i1zxh3rv" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-fluid">
        <div class="form" style="height:550px;">
            <div class="doc_header">
                My Profile (MPIN GENERATION)
            </div>
            <div class="row text-center">
                <label style="color: red">
                    <asp:Label ID="lblMsgSave" runat="server" Text=""></asp:Label>
                </label>
            </div>
            <div class="row" id="CMPINROW" runat="server">
                <div class="col-md-3 col-sm-3">
                    &nbsp;
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>Current MPIN *</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox ID="txtCMPIN" runat="server" CssClass="txtBox"
                        TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqCURMPIN" ForeColor="Red" runat="server" ControlToValidate="txtCMPIN" Display="None" ErrorMessage="Please enter current MPIN" SetFocusOnError="true" ValidationGroup="a"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="val_reqCURMPIN" Enabled="true" runat="server" TargetControlID="reqCURMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                    <asp:RegularExpressionValidator ID="Reg_txtCMPIN" ForeColor="Red" ValidationGroup="a" Display="None"
                        ControlToValidate="txtCMPIN" runat="server" ErrorMessage="Enter Valid MPIN."
                        SetFocusOnError="True" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
                    <asp:ValidatorCalloutExtender ID="Val_Reg_txtCMPIN" runat="server" TargetControlID="Reg_txtCMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                </div>
            </div>
            <div class="row">
                <div class="col-md-3 col-sm-3">
                    &nbsp;
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>New MPIN *</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox ID="txtNMPIN" runat="server" CssClass="txtBox" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqMPIN" Display="None" runat="server" ControlToValidate="txtNMPIN" ErrorMessage="Please enter MPIN" Enabled="true" ForeColor="Red" ValidationGroup="a"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="Val_reqMPIN" Enabled="true" runat="server" TargetControlID="reqMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                    <asp:RegularExpressionValidator ID="Reg_txtNMPIN" ForeColor="Red" ValidationGroup="a" Display="None"
                        ControlToValidate="txtNMPIN" runat="server" ErrorMessage="Enter Valid MPIN."
                        SetFocusOnError="True" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
                    <asp:ValidatorCalloutExtender ID="Val_Reg_txtNMPIN" runat="server" TargetControlID="Reg_txtNMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                </div>
            </div>
            <div class="row">
                <div class="col-md-3 col-sm-3">
                    &nbsp;
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>Confirm New MPIN *</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox ID="txtRMPIN" runat="server" CssClass="txtBox" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="req_txtRMPIN" runat="server" Display="None" ControlToValidate="txtRMPIN" ErrorMessage="Please enter confirm MPIN  " Enabled="true" ValidationGroup="a" ForeColor="Red"></asp:RequiredFieldValidator>
                    <asp:ValidatorCalloutExtender ID="Val_reqCONMPIN" Enabled="true" runat="server" TargetControlID="req_txtRMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                    <asp:CompareValidator ID="cmpMPIN" Display="None" ForeColor="Red" runat="server" ErrorMessage="New MPIN and Confirm MPIN should be same" ControlToCompare="txtNMPIN" ControlToValidate="txtRMPIN" ValidationGroup="a"></asp:CompareValidator>
                    <asp:ValidatorCalloutExtender ID="Val_cmpMPIN" Enabled="true" runat="server" TargetControlID="cmpMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                    <asp:RegularExpressionValidator ID="Reg_txtRMPIN" ForeColor="Red" ValidationGroup="a" Display="None"
                        ControlToValidate="txtRMPIN" runat="server" ErrorMessage="Enter Valid MPIN."
                        SetFocusOnError="True" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
                    <asp:ValidatorCalloutExtender ID="Val_Reg_txtRMPIN" runat="server"  TargetControlID="Reg_txtRMPIN" PopupPosition="Right"></asp:ValidatorCalloutExtender>
                </div>
            </div>
            <div class="row">
                &nbsp;
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12" style="text-align: center;">
                    <asp:Button ID="btnSave" runat="server" CssClass="btnNew" Text=" Change MPIN" OnClick="btnSave_Click" ValidationGroup="a" />

                </div>
            </div>
        </div>
</asp:Content>

