<%@ page title="" language="VB" masterpagefile="~/USR.master" validaterequest="false" autoeventwireup="false" inherits="Multipleprerole, App_Web_bv10wntb" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"><h4>Multi Prerole Assignment user</h4></asp:Label>
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td style="text-align: center;">
                        <asp:Label ID="lblMsg1" runat="server" Font-Bold="True" ForeColor="Red"
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

                                <td style="text-align: right;">
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" OnClick="Add" />
                                    &nbsp;
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
                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" CellPadding="2" DataKeyNames="tid"
                            ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="false">
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
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="userid" HeaderText="User Id">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Username" HeaderText="User Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Rolename" HeaderText="Role">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <%-- <asp:BoundField DataField="CurrentRoles" HeaderText="Current Role">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>--%>
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnDeleteUser" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHitUser"
                                            AlternateText="Delete" ToolTip="Delete" />
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>


                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupEdit" runat="server" Width="920px" Height="150px" ScrollBars="Auto" Style="display: block">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 900px">
                        <h3>Add Multiple Role</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="form">

                                    <table cellspacing="2px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="5">
                                                <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" Font-Size="X-Small"
                                                    ForeColor="Red" Width="98%"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="right" width="15%">User Name</td>
                                            <td align="left" width="30%">
                                                <asp:DropDownList ID="ddlusr"  runat="server" Width="98%" CssClass="txtBox" AutoPostBack="true"></asp:DropDownList>
                                            </td>

                                            <td align="right" width="15%">Role Name</td>
                                            <td align="left" width="30%">
                                                <asp:DropDownList ID="ddlrole" runat="server" Width="98%" CssClass="txtBox" AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="bottom" colspan="5">
                                                <asp:Label ID="lblcurrRoles" runat="server" Font-Bold="True" Font-Size="X-Small" 
                                                    ForeColor="Red" Width="70%"></asp:Label>
                                                
                                            </td>
                                            <td align="left" valign="bottom" colspan="5">
                                            <asp:Button ID="btnadd" runat="server" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Text="Add" Width="100px" OnClick="btnadd_Click" />
                                            </td>
                                        </tr>
                                </div>
                                </td>
  </tr>
  </table>
</div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>





</asp:Content>

