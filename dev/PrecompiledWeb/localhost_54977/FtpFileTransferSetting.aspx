<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="FtpFileTransferSetting, App_Web_4somiesn" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="https://code.jquery.com/jquery-1.11.0.min.js"></script>
    <style type="text/css">
        .mg20 {
            margin-top: 20px;
        }
    </style>
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnActEdit" />--%>
            <asp:PostBackTrigger ControlID="btnexport" />
        </Triggers>
        <ContentTemplate>
            <div class="container-fluid">
                <div class="form">
                    <div class="doc_header">
                        FTP File Transfer Setting from Local File
                    </div>
                    <div class="col-md-12 col-sm-12">
                        &nbsp;
                    </div>
                    <div class="row">
                        <div class="col-md-1 col-sm-1">
                            <label>Field Name</label>
                        </div>
                        <div class="col-md-3 col-sm-3">
                            <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox">
                                <asp:ListItem Text="EID">EID</asp:ListItem>
                                <asp:ListItem Text="Document Type">Document Type</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Value</label>
                        </div>
                        <div class="col-md-3 col-sm-3">
                            <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox"></asp:TextBox>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" ToolTip="Search"
                                ImageUrl="~/Images/search.png" />
                        </div>
                        <div class="col-md-3 col-sm-3" style="text-align: right;">
                            <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg"
                                ToolTip="Add User" OnClick="Add" />
                            &nbsp;<asp:ImageButton ID="btnexport" runat="server" ToolTip="Export" ImageUrl="~/Images/excel.gif" />
                        </div>
                    </div>

                    <div class="col-md-12 col-sm-12">
                        &nbsp;
                    </div>
                    <div class="col-md-12 col-sm-12">
                        <div class="alert alert-success fade in" id="msgUpdateDiv" runat="server" visible="false">
                            <a href="#" class="close" data-dismiss="alert">&times;</a>
                            <strong>Success!</strong>
                            <asp:Label ID="lblMsgupdate" runat="server"></asp:Label>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                <ProgressTemplate>
                                    <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                        <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                        please wait...
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                DataKeyNames="ftpid" ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="True"
                                DataSourceID="SqlData">
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
                                    <asp:BoundField DataField="EID" HeaderText="EID">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="FirstName" HeaderText="User Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="gid" HeaderText="GID">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DocType" HeaderText="Document Type">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="fup_displayname" HeaderText="File Upload Mapping">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="loc_fieldMapping" HeaderText="Location Field Mapping">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="bar_displayname" HeaderText="Bar Code Mapping">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ReadMode" HeaderText="Read Type">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="HostName" HeaderText="Host Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UserName" HeaderText="FTP User Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Password" HeaderText="FTP Password">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Port" HeaderText="FTP Port">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="isDuplicate" HeaderText="Is Duplicate Checked">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <%-- <asp:BoundField DataField="Prefix" HeaderText="FTP Prefix" >
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>--%>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDtl" ImageUrl="~/images/lock.png" runat="server" Height="16px"
                                                Width="16px" OnClick="LockHit" ToolTip="Lock / Unlock User" AlternateText="Lock / Unlock" />
                                            &nbsp;
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.png" Height="16px"
                                            Width="16px" OnClick="EditHit" ToolTip="Edit FTP Details" AlternateText="Edit" />
                                            &nbsp;
                                       <%-- <asp:ImageButton ID="btnRole" runat="server" ImageUrl="~/images/add_group.png" Height="16px"
                                            Width="16px" OnClick="AssignRole" ToolTip="Pre Role Assignment" AlternateText="Pre Role Assignment" />--%>
                                        </ItemTemplate>
                                        <ItemStyle Width="90px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                <SortedDescendingHeaderStyle BackColor="#002876" />
                            </asp:GridView>
                            <asp:SqlDataSource ID="SqlData" runat="server" ConnectionString="<%$ ConnectionStrings:conStr %>"
                                SelectCommand="uspGetFTPDetails" SelectCommandType="StoredProcedure">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlField" Name="sField" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"
                                        Type="String" />
                                    <asp:SessionParameter Name="eid" SessionField="EID" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupEdit"
        PopupControlID="pnlPopupEdit" CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="1000px" BackColor="#FFFFFF" Height="500px"
        Style="overflow: scroll">
        <div class="box">
            <div class="container-fluid">
                <div class="doc_header">
                    Add/Update Ftp File Transfer Setting
                    <div style="float: right;">
                        <asp:ImageButton ID="btnCloseEdit" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png" />
                    </div>

                </div>
                <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <div class="alert alert-danger" id="popAlert" runat="server">
                                    <strong>Alert!</strong>
                                    <asp:Label ID="lblMsgEdit" runat="server"></asp:Label>
                                </div>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>FTP ID</label>

                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:TextBox ID="txtFtpID" runat="server" ReadOnly="true" Text="0" CssClass="txtBox"></asp:TextBox>
                                    <asp:HiddenField ID="hdnFtpID" runat="server" />
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>User Id</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlUserList" runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>
                                </div>

                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>Document Type</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlDocumenttype" AutoPostBack="true" OnSelectedIndexChanged="ddlDocumenttype_SelectedIndexChanged" runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>FUP Mapping</label>

                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlFupMapping" runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>
                                </div>

                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>Location Field</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlLocation" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>Locations</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlLocationMaster" runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>
                                </div>


                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>File Name</label>

                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlBarCode" runat="server" CssClass="txtBox" Width="200px">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3 col-sm-3">
                                    <label>Read From</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:DropDownList ID="ddlReadMode" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="ddlReadMode_SelectedIndexChanged">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="LOCAL">Local</asp:ListItem>
                                        <asp:ListItem Value="FTP">FTP</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>Remove Dublicate Files</label>

                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:CheckBox ID="isDublicityChecked" runat="server" Checked="false" CssClass="txtBox" Width="200px" />
                                </div>
                                <div class="col-md-3 col-sm-3"></div>
                                <div class="col-md-3 col-sm-3"></div>

                            </div>
                        </div>
                        <div class="row" id="FTPMode" runat="server" visible="false">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>Host Name</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:TextBox ID="txtHostName" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <label>User Name</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:TextBox ID="txtFTPUserName" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div class="row" id="FTPMode2" runat="server" visible="false">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>Password</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:TextBox ID="txtFTPPassword" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                </div>

                                <div class="col-md-3 col-sm-3">
                                    <label>Port</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:TextBox ID="txtPort" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div class="row" id="FTPMode3" runat="server" visible="false">
                            <div class="col-md-12 col-sm-12 mg20">
                                <div class="col-md-3 col-sm-3">
                                    <label>Prefix URL</label>
                                </div>
                                <div class="col-md-3 col-sm-3">
                                    <asp:TextBox ID="txtPrefix" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            &nbsp;
                        </div>
                        <div class="row">
                            <div style="width: 100%; text-align: center">
                                <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                    OnClick="btnSubmit_Click" Width="100px" OnClientClick="this.disabled=true;"
                                    UseSubmitBehavior="false" ValidationGroup="a" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        function UpdateTime() {
            alert('Hii');
        }
    </script>
</asp:Content>


