<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="ReallocationConfiguration, App_Web_gn32bfei" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="form">
         <div class="doc_header">
            Re-Allocation Configuration
        </div>
       <div class="row mg">
        <div class="col-md-12 col-sm-12">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <Triggers>
        </Triggers>
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <%--<tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"><h3>Reallocation Configuration</h3></asp:Label>
                    </td>
                </tr>--%>
                <tr>
                    <td style="text-align: center;">
                        <asp:Label ID="lblMsgupdate" runat="server" Font-Bold="True" ForeColor="Red"
                            Font-Size="Small"></asp:Label>
                    </td>

                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double lime">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0">
                            <tr>


                                <td style="text-align: right;">
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Plus.jpg" OnClick="Add" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;
          
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
                        <div style="overflow: scroll; height: 400px;">
                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                                CellPadding="2" DataKeyNames="Tid" DataSourceID="SqlData"
                                ForeColor="#333333" Width="100%"
                                AllowSorting="False" AllowPaging="False">
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <RowStyle BackColor="#EFF3FB" />
                                <EditRowStyle BackColor="#2461BF" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="S.No">
                                        <ItemTemplate>
                                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>


                                    <asp:BoundField DataField="doctype" HeaderText="Document Type">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="status" HeaderText="Status">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Role" HeaderText="Role">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="displayname" HeaderText="Role Assignment Field">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Remarks" HeaderText="Remarks">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>


                                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/editrole1.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit WS" AlternateText="Edit" />
                                            &nbsp;
                                   <asp:ImageButton ID="btnDtl" ImageUrl="~/images/closered.png" runat="server" Height="16px" Width="16px" ToolTip="Delete Role" OnClick="DeleteHit" AlternateText="Delete" />
                                        </ItemTemplate>
                                        <ItemStyle Width="90px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SqlData" runat="server"
                                ConnectionString="<%$ ConnectionStrings:conStr %>"
                                SelectCommand="uspgetresult_Reallocation" SelectCommandType="StoredProcedure">
                                <SelectParameters>
                                    <asp:SessionParameter Name="eid" SessionField="EID" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>


                        </div>

                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    
     </div>
    </div>


    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupEdit" runat="server" Width="700px" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>Create / Update Reallocation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                    <tr>
                                        <td colspan="2" align="left">
                                            <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>


                                    <tr>
                                        <td style="width: 125px" align="left"><b>Document Type </b></td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddldoctype" AutoPostBack="true" runat="server" CssClass="txtbox" Width="98%">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 125px" align="left"><b>Status </b></td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlstatus" AutoPostBack="false" runat="server" CssClass="txtbox" Width="98%">
                                            </asp:DropDownList></td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Role</b></td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlrole" AutoPostBack="true" runat="server" CssClass="txtbox" Width="98%">
                                            </asp:DropDownList></td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left"><b>Role Assignment Field</b></td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlcfield"  runat="server" CssClass="txtbox" Width="98%">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 125px" align="left"><b>Allowed Roles</b>                  
                                        </td>
                                        <td align="left">
                                            <div style="max-height:100px;height:auto !important;height:100px;overflow:scroll;width:250px">
                                                 <asp:CheckBoxList ID="chkRoles" runat="server"></asp:CheckBoxList>
                                            </div>
                                           
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 125px" align="left"><b>Remarks </b></td>
                                        <td align="left">
                                            <asp:TextBox ID="txtRemarks" runat="server" Width="98%"></asp:TextBox></td>
                                    </tr>
                                 

                                </table>
                                <div style="width: 100%; text-align: right">
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


    <asp:Button ID="btnShowPopupDelFolder" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder"
        CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 480px">
                        <h3>Delete Service</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelFolder"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td colspan="2" align="left">
                                            <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete this Service" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>
                                </table>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete"
                                        OnClick="DelFile" CssClass="btnNew" Font-Bold="True"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        </div>
</asp:Content>
