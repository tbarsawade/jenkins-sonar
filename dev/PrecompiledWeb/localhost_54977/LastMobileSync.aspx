<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="LastMobileSync, App_Web_4somiesn" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"><h4>Last Mobile Sync Configuration</h4></asp:Label>
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double lime">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="width: 90px;">
                                    <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="From Name" Width="99%">
                                    </asp:Label>
                                </td>
                                <td style="width: 200px;">
                                    <asp:TextBox ID="txtValue" runat="server" CssClass="Inputform" Font-Bold="True"
                                        Font-Size="Small" Width="99%"></asp:TextBox>
                                </td>

                                <td style="text-align: right; width: 25px">
                                    <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                        ImageUrl="~/Images/search.png" />
                                </td>

                                <td style="text-align: right;">
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" OnClick="Add" ToolTip="ADD Form" />
                                    &nbsp;
                                </td>

                            </tr>

                        </table>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1000454545454; left: 50%; top: 30%;">
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
                            CellPadding="2" DataKeyNames="SyncID"
                            CssClass="GridView"
                            AllowSorting="True" PageSize="20" AllowPaging="True">
                            <FooterStyle CssClass="FooterStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <EditRowStyle CssClass="EditRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass=" HeaderStyle" />
                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                            <Columns>

                                <asp:TemplateField HeaderText="S.No">
                                    <ItemTemplate>
                                        <%# CType(Container, GridViewRow).RowIndex + 1%>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>

                                <asp:BoundField DataField="Documenttype" HeaderText="Form Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Rolename" HeaderText="Role Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="isactive" HeaderText="Active">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit Form Detail" AlternateText="Edit" />
                                    </ItemTemplate>
                                    <ItemStyle Width="220px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm" TargetControlID="btnShowPopupForm"
        CancelControlID="btnCloseForm" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupForm" runat="server" Width="1000px" Height="300px" BackColor="White" Style="">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td width="980px">
                        <h3>New Last Mobile Sync Configuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="main">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="form" style="text-align: left">

                                        <table cellspacing="5px" cellpadding="0px" width="100%" border="0">
                                            <tr>
                                                <td colspan="2" align="right">
                                                    <asp:Label ID="lblnk" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100%" colspan="2">
                                                    <asp:Label ID="lblForm" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="width: 100%" colspan="2">
                                                    <h2>Enter basic form information</h2>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td valign="middle" style="text-align: right;">
                                                    <label title="This name will appear in the menu through which user will be able to open this form for input">
                                                        *Form Name
                                                        <img src="Images/Help.png" alt="" />
                                                        : 
                                                    </label>
                                                </td>
                                                <td style="">
                                                    <asp:DropDownList ID="ddlDocType" runat="server" CssClass="txtBox" Width="204px" OnSelectedIndexChanged="ddlDocType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="middle" style="text-align: right">
                                                    <label title="Form Caption will appear in the header of the form">
                                                        *Based Condition
                                                        <img src="Images/Help.png" alt="" />
                                                        : 
                                                    </label>
                                                </td>
                                                <td style="">
                                                    <asp:DropDownList ID="ddlBasedCon" runat="server" CssClass="txtBox" Width="204px"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="middle" style="text-align: right">
                                                    <label title="Form Caption will appear in the header of the form">
                                                        *Role Name
                                                        <img src="Images/Help.png" alt="" />
                                                        : 
                                                    </label>
                                                </td>
                                                <td style="">
                                                    <asp:DropDownList ID="ddlRoleName" runat="server" CssClass="txtBox" Width="204px"></asp:DropDownList>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td valign="middle" style="text-align: right">
                                                    <label title="Form Description will describe about form and it will appear below to caption also in the title of the Form">
                                                        *No Of Days
                                                        <img src="Images/Help.png" alt="" />
                                                        : 
                                                    </label>
                                                </td>
                                                <td style="">
                                                    <asp:TextBox ID="txtNoofdays" runat="server" CssClass="txtBox"
                                                        Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="middle" style="text-align: right">
                                                    <label title="Form Description will describe about form and it will appear below to caption also in the title of the Form">
                                                        *Is Acive
                                                        <img src="Images/Help.png" alt="" />
                                                        : 
                                                    </label>
                                                </td>
                                                <td style="">
                                                    <asp:CheckBox ID="chkisActive" runat="server" />
                                                </td>

                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td align="left">
                                                    <asp:Button ID="btnlogin" runat="server" CssClass="btnNew" Text="Save" Width="100Px" />
                                                </td>
                                            </tr>
                                            </td>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>

