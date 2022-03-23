<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="PreRoleDataFilter, App_Web_erizob0y" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%"
                border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="red" Width="97%"
                            Font-Size="Small"><h4>Pre Role Data Filter </h4></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;">
                        <asp:Label ID="lblRecord" runat="server" ForeColor="red" Font-Size="Small"></asp:Label>
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double green">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="text-align: right;">
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" ToolTip="Add Status" OnClick="Add" />
                                </td>
                            </tr>
                        </table>
                        <table width="100%" cellspacing="0px" cellpadding="0px">
                            <tr>
                                <td style="width: 80%">
                                    <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" CellPadding="2"
                                        DataKeyNames="TID" ForeColor="#333333" Width="100%" AllowSorting="True">
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
                                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="RoleName" HeaderText="Role Name">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Documenttype" HeaderText="Documenttype">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                             </asp:BoundField>
                                             <asp:BoundField DataField="DisplayName" HeaderText="DisplayName">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                             </asp:BoundField>
                                              <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <%--<asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px"
                                                        Width="16px" ToolTip="Edit record" OnClick="EditHit" AlternateText="Edit" />--%>
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px"
                                                        Width="16px" ToolTip="Delete record" OnClick="DeleteHit" AlternateText="Delete" />
                                                </ItemTemplate>
                                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
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
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupEdit"
        PopupControlID="pnlPopupEdit" CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="500px" Style="" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>
                           Pre Role Assignment</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit" ImageUrl="images/close.png" runat="server" ToolTip="Close" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                    Role Name</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlUserRoleName" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                    Documenttype</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlDocType" runat="server" Width="85%" CssClass="Inputform" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                   Document Field Mapping</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlDocFldMapping" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnActUserSave" runat="server" CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Text="Save" Width="70px" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnConfigStatus" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_ConfigStatus" runat="server" TargetControlID="btnConfigStatus"
        PopupControlID="pnlConfigStatus" CancelControlID="btnCloseConfigstatus" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlConfigStatus" runat="server" Width="500px" Style="" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>
                            Configure First / Default status</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseConfigstatus" ImageUrl="images/close.png" runat="server"
                            ToolTip="Close" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="updConfigStatus" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblConfigStatus" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px; height: 25px;">
                                                <label>
                                                    Document Type</label>
                                            </td>
                                            <td align="left" style="height: 25px">
                                                <asp:DropDownList ID="ddlConfigstatus" runat="server" Width="85%" CssClass="Inputform"
                                                    AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                    Status Name</label>
                                            </td>
                                            <td align="left">
                                                <%-- <asp:TextBox ID="txtConfigstatus" runat="server"  CssClass="txtBox" Width="85%" Text="UPLOADED"></asp:TextBox>--%>
                                                <asp:DropDownList ID="ddlstatusNam" runat="server" Width="85%" CssClass="Inputform"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Text="Select"></asp:ListItem>
                                                    <asp:ListItem Text="UPLOADED"></asp:ListItem>
                                                    <asp:ListItem Text="ARCHIVE"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Display order</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtConfDord" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <%--<tr>
    <td style="width:110px" align="left"><label>Approve Caption</label></td> 
    <td align="left">
        <asp:TextBox ID="TextBox3" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
     </td>
   </tr>--%>
                                        <%--<tr>
    <td style="width:110px" align="left"><label>Reject Caption</label></td> 
    <td align="left">
        <asp:TextBox ID="TextBox4" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
     </td>
   </tr>--%>
                                        <%--<tr>
    <td style="width:110px" align="left"><label>Reconsider Caption</label></td> 
    <td align="left">
        <asp:TextBox ID="TextBox5" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
     </td>
   </tr>--%>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Amendment
                                                </label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtConfAmendment" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Recall
                                                </label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtConfRecall" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Cancel</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtConfCancel" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Manage by other role</label>
                                            </td>
                                            <td align="left">
                                                <asp:CheckBox ID="chkConfManByOtherRole" runat="server" AutoPostBack="true" />
                                                <asp:Label ID="lblConfManByother" runat="server" Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px; height: 25px;">
                                                <label>
                                                    Role Name</label>
                                            </td>
                                            <td align="left" style="height: 25px">
                                                <asp:DropDownList ID="ddlConfRoleName" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Allow Skip</label>
                                            </td>
                                            <td align="left">
                                                <asp:CheckBox ID="chkCofAllowSkp" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Allow Split</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtConfAllSplt" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
                                                <%--<asp:CheckBox ID="chkConfSplt" runat="server"/> --%>
                                            </td>
                                        </tr>
                                         <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Document Edit</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtConfDocEdit" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>
                                                <%--<asp:CheckBox ID="chkConfSplt" runat="server"/> --%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <%--<td style="width:110px" align="left"><label> Activate Doc.</label></td>--%>
                                            <td align="left">
                                                <%--<asp:CheckBox ID="CheckBox1" runat="server"/> --%>
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnConfigStatusSave" runat="server" CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Text="Save" Width="70px" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnShowPopupDelete" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupDelete"
        PopupControlID="pnlPopupDelete" CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" Style="display: none"
        BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>
                            Work Flow Edit/Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red" Width="97%"
                                        Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelete" runat="server" Text="Yes Delete" Width="90px" OnClick="DeleteRecord"
                                        CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnshowRejectStatus" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender_RejectStatus" runat="server" TargetControlID="btnshowRejectStatus"
        PopupControlID="pnlpopRejectstatus" CancelControlID="btnCloseRejEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlpopRejectstatus" runat="server" Width="500px" Style="" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>
                            Workflow Reconsider Status</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseRejEdit" ImageUrl="images/close.png" runat="server"
                            ToolTip="Close" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="updatePanel_rejectstatus" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblRejectStatus" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px; height: 25px;">
                                                <label>
                                                    Document Type</label>
                                            </td>
                                            <td align="left" style="height: 25px">
                                                <%--<asp:DropDownList ID="DropDownList1"  runat="server" Width="85%" CssClass="Inputform"></asp:DropDownList>--%>
                                                <asp:TextBox ID="txtRejDocumenttype" runat="server" CssClass="txtBox" Width="85%"
                                                    Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                    Status Name</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtRejStatusname" runat="server" CssClass="txtBox" Width="85%" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Reconsider Status</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlRejStatus" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                                <%--<asp:TextBox ID="TextBox4" runat="server" CssClass="txtBox" Width="85%"></asp:TextBox>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right">
                                                <asp:Button ID="btnRejStatus" runat="server" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small"
                                                    Text="Save" Width="70px" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <%--cur/new docs status--%>
    <asp:Button ID="btnshowCurrStatus" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_CurNewStatus" runat="server" TargetControlID="btnshowCurrStatus"
        PopupControlID="pnlCurStatus" CancelControlID="btncloseCurStatus" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlCurStatus" runat="server" Width="500px" Style="" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>
                            Current/New Document Status</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btncloseCurStatus" ImageUrl="images/close.png" runat="server"
                            ToolTip="Close" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="updCurStatus" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblCurStatus" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px; height: 25px;">
                                                <label>
                                                    Document Type</label>
                                            </td>
                                            <td align="left" style="height: 25px">
                                                <asp:TextBox ID="txtCurDocType" runat="server" CssClass="txtBox" Width="85%" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                    Status Name</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtCurStatus" runat="server" CssClass="txtBox" Width="85%" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    Current Document Status</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlCurstatus" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <label>
                                                    NEW Document Status</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlNewStatus" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right">
                                                <asp:Button ID="btnCurNewStatus" runat="server" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small"
                                                    Text="Save" Width="70px" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
