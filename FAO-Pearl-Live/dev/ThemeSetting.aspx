<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="ThemeSetting.aspx.vb" Inherits="ThemeSetting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        /* styles unrelated to zoom */
        * {
            border: 0;
            margin: 0;
            padding: 0;
        }

        p {
            position: absolute;
            top: 3px;
            right: 28px;
            color: #555;
            font: bold 13px/1 sans-serif;
        }

        /* these styles are for the demo, but are not required for the plugin */
        .zoom {
            display: inline-block;
            position: relative;
        }

            /* magnifying glass icon */
            .zoom:after {
                content: '';
                display: block;
                width: 33px;
                height: 33px;
                position: absolute;
                top: 0;
                right: 0;
                background: url(icon.png);
            }

            .zoom img {
                display: block;
            }

                .zoom img::selection {
                    background-color: transparent;
                }

        #ex2 img:hover {
            cursor: url(grab.cur), default;
        }

        #ex2 img:active {
            cursor: url(grabbed.cur), default;
        }
    </style>
    <script src='http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js' type="text/javascript"></script>
    <script src="Jquery/jquery.zoom.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#ex1').zoom();
            $('#ex2').zoom();
            $('#ex3').zoom();
        });
    </script>
    <script type="text/javascript">
        $("ContentPlaceHolder1_imgLogin").click(function () {
            alert('Hii');
        });
    </script>

    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr style="color: #000000">
            <td>
                <asp:UpdatePanel ID="updtheme" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="grdTheme" CssClass="GridView" runat="server" AutoGenerateColumns="false" CellPadding="2">
                            <FooterStyle CssClass="FooterStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <EditRowStyle CssClass="EmptyDataRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass="HeaderStyle" />
                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        Theme Name
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblThemeName" runat="server" Text='<%#Eval("ThemeName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        Action
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <center>
                                            <asp:ImageButton ID="imgApply" CssClass="img-circle" ToolTip="Click Apply To Theme" runat="server" Width="30px" ImageUrl="~/images/dapprove.png" Height="30px" OnClick="imgApply_click" />
                                            <asp:ImageButton ID="imgPreview" CssClass="img-circle" ToolTip="Click Preview To Theme" runat="server" Width="30px" ImageUrl="~/Images/Preview.jpg" Height="30px" OnClick="imgPreview_click" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        Is Current Theme
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <center>
                                            <asp:Label ID="lblICT" runat="server" Text='<%#Eval("IsCT")%>'></asp:Label>

                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
                    TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
                    BackgroundCssClass="modalBackground"
                    CancelControlID="btnCloseEdit"
                    DropShadow="true">
                </asp:ModalPopupExtender>

                <asp:Panel ID="pnlPopupEdit" runat="server" Width="1050px" BackColor="aqua" Style="display: none;">
                    <div class="box" style="overflow: scroll;">

                        <table cellspacing="2px" cellpadding="2px" width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:UpdatePanel ID="updPanalHeader" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <h3>
                                                <asp:Label ID="lblHeaderPopUp" runat="server" Font-Bold="True"></asp:Label></h3>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td style="width: 20px">
                                    <asp:ImageButton ID="btnCloseEdit" ImageUrl="images/close.png" runat="server" CausesValidation="false" />

                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:UpdatePanel ID="updateimage" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <table cellpadding="2" cellspacing="2" border="1" width="98%">
                                                <tr>
                                                    <td style="border-width: thick;">
                                                        <span class='zoom' id='ex1'>
                                                            <img id="imgLogin" runat="server" width="340" height="350" />
                                                        </span>

                                                        <asp:Label ID="lblLogin" runat="server"></asp:Label>
                                                    </td>
                                                    <td style="border-width: thick;">
                                                        <span class='zoom' id='ex2'>
                                                            <img id="imgHome" runat="server" width="340" height="350" />

                                                        </span>

                                                        <asp:Label ID="lblHome" runat="server"></asp:Label>
                                                    </td>
                                                    <td style="border-width: thick;">
                                                        <span class='zoom' id='ex3'>
                                                            <img id="imgControl" runat="server" width="340" height="350" />

                                                        </span>
                                                        <asp:Label ID="lblControl" runat="server"></asp:Label>
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
            </td>

        </tr>
    </table>
    <%--<asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        BackgroundCssClass="modalBackground"
        CancelControlID="btnCloseEdit"
        DropShadow="true">
    </asp:ModalPopupExtender>--%>
    <%-- <div id="dialog" style="display: none">--%>

    <%--  </div>--%>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
</asp:Content>

