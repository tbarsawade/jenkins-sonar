<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" enableeventvalidation="false" inherits="RoleMaster, App_Web_sifhu5tb" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
   
    <style type="text/css">
        .mg {
            margin: 10px 0px;
        }

    </style>

    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnimport" />
            <asp:PostBackTrigger ControlID="btnexport" />
        </Triggers>
        <ContentTemplate>
            <div class="form" style="text-align: left">
                <div class="doc_header">
                    <asp:Label ID="lblMsg" runat="server" Text="Role Master"></asp:Label>
                </div>
                <div class="row mg">
                    <div class="col-md-12 col-sm-12" style="text-align: center;">
                        <asp:Label ID="lblMsgupdate" runat="server" Font-Bold="True" ForeColor="Red"
                            Font-Size="Small"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-1 col-sm-1 mg">
                        <label>Field Name</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-1 col-sm-1 mg">
                        <label>Values</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox"></asp:TextBox>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <div class="pull-left" style="border: 1px solid #ccc; padding: 4px 4px;">
                            <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/search.png" />
                        </div>
                        <div class="pull-left" style="margin: 0px 0px 0px 25px; border: 1px solid #ccc; padding: 4px 0px 4px 3px;">
                            <asp:FileUpload ID="FileUpload" Font-Bold="true" ForeColor="Navy" runat="server" />
                        </div>
                        <div class="pull-right" style="border: 1px solid #ccc; padding: 4px 4px;">
                            <asp:ImageButton ID="btnimport" ToolTip="Import" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/up.png" />
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <div class="pull-right" style="border: 1px solid #ccc; padding: 3px 12px;">
                            <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/add_group.png" OnClick="Add" />
                            &nbsp;<asp:ImageButton ID="btnexport" runat="server" ToolTip="Export" ImageUrl="~/Images/excel.gif" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12 mg">
                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                            CellPadding="2" DataKeyNames="roleid" DataSourceID="SqlData"
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

                                <asp:BoundField DataField="RoleName" HeaderText="Role">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="RoleDescription" HeaderText="Role Description">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="RoleType" HeaderText="Role Type">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Access Type" HeaderText="Access Type">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="MainHome_DefaultDcoument" HeaderText="MainHome Default Dcoument">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                   <asp:BoundField DataField="mappedroleassignment" HeaderText="Mapped Role Assignment">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="AllowedDelegate" HeaderText="AllowedDelegate">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                  <asp:BoundField DataField="CreatedBy" HeaderText="Created By">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                   <asp:BoundField DataField="ADate" HeaderText="Create Date">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                 

                                   <asp:BoundField DataField="ModifyDate" HeaderText="Modify Date">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                 
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>


                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/editrole1.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit Role" AlternateText="Edit" />
                                        &nbsp;
                                   <asp:ImageButton ID="btnDtl" ImageUrl="~/images/closered.png" runat="server" Height="16px" Width="16px" ToolTip="Delete Role" OnClick="DeleteHit" AlternateText="Delete" />
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlData" runat="server"
                            ConnectionString="<%$ ConnectionStrings:conStr %>"
                            SelectCommand="uspGetResultrole" SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlField" Name="sField"
                                    PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"
                                    Type="String" />
                                <asp:SessionParameter Name="eid" SessionField="EID" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>

            <%--     <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">--%>
            <%--     <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"><h3>Role Master</h3></asp:Label>
                    </td>
                </tr>--%>
            <%--  <tr>
                    <td style="text-align: center;"></td>

                </tr>--%>

            <%--    <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double lime">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="width: 90px;">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Field Name" Width="99%"> </asp:Label>
                                </td>

                                <td style="width: 170px;"></td>

                                <td style="width: 50px;">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Value" Width="99%"></asp:Label>
                                </td>

                                <td style="width: 200px;"></td>

                                <td style="text-align: right; width: 25px"></td>
                                <td>&nbsp;
            

                                <td style="text-align: right;">


                                    <caption>
                                        &nbsp;
                                    </caption>

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
                </tr>--%>


            <%--  <tr style="color: #000000">
                    <td style="text-align: left;" valign="top"></td>
                </tr>--%>
            <%--  </table>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupEdit" runat="server" Width="600px" BackColor="aqua">
        <div class="form">
            <div class="doc_header">
                New / Update Role
                <div class="pull-right">
                    <asp:ImageButton ID="btnCloseEdit"
                        ImageUrl="images/close.png" runat="server" />
                </div>
            </div>
            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="row mg">
                        <div class="col-md-12 col-sm-12 mg" style="text-align: center;">
                            <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="true" ForeColor="Red" Width="100%"></asp:Label>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>User Role*</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:TextBox ID="txtrole" runat="server" CssClass="txtBox"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Role Description</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:TextBox ID="txtdesc" runat="server" CssClass="txtBox"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Role Type</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:DropDownList ID="ddlrtype" runat="server" CssClass="txtBox" onchange="YourChangeFun(this);">
                                    <asp:ListItem>Pre Type</asp:ListItem>
                                    <asp:ListItem>Post Type</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Access Type</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:DropDownList ID="ddlaccesstype" runat="server" CssClass="txtBox">
                                    <asp:ListItem Value="0">Web</asp:ListItem>
                                    <asp:ListItem Value="1">Tab</asp:ListItem>
                                    <asp:ListItem Value="2">Both</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>MainHome Default Document</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:DropDownList ID="ddldefdoc" runat="server" CssClass="txtBox">
                                </asp:DropDownList>
                            </div>
                        </div>
<div class="row">
<div class="col-md-3 col-sm-3">
                        <label>Allowed delegation</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
<asp:CheckBox ID="chkVacation1" runat="server" Checked="true"/>
                    </div>
                              </div>
                        
                        <div class="row" id="dvHideShow">
                            <div class="col-md-3 col-sm-3">
                                <label>Mapped Role Assignment</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:ListBox ID="lstmappedroleassignment" CssClass="txtBox ddlmappedroleassignment" style="min-height:100px" runat="server" SelectionMode="Multiple">
                                    
                                </asp:ListBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12" style="text-align: center;">
                                <asp:Button ID="btnActEdit" runat="server" Text="Update"
                                    OnClick="EditRecord" CssClass="btnNew" />
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <%--   <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3></h3>
                    </td>
                    <td style="width: 20px"></td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
            </table>
        </div>--%>
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
                        <h3>Delete Role</h3>
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
                                            <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete" Font-Bold="True" ForeColor="Red"
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
    <script type="text/javascript">
        function multiselectddl() {
            if ($("select[id*='ddlrtype']").find('option:selected').val() == "Post Type") {
                $("#dvHideShow").css("display", "block");
            }
            else {
                $("#dvHideShow").css("display", "none");
            }
        }
        function YourChangeFun(ddl) {
            if ($(ddl).find('option:selected').val() == "Post Type") {
                $("#dvHideShow").css("display", "block");
            }
            else {
                $("#dvHideShow").css("display", "none");
            }
        }
        $(function () {
            $("#dvHideShow").css("display", "none");
        })
    </script>
</asp:Content>


