<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="BulkDownloader, App_Web_iqn0gzeb" enableeventvalidation="true" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }
        $(document).ready(function () {

        });
        function ShowDialog(url) {
            var $dialog = $('<div></div>')
            .load(url)
            .dialog({
                autoOpen: true,
                title: 'Document Detail',
                width: 700,
                height: 550,
                modal: true
            });
            return false;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=gvPending.ClientID %>').Scrollable({
                ScrollHeight: 300,
                IsInUpdatePanel: true
            });
        });
</script>
    <style type="text/css">
        .gradientBoxesWithOuterShadows
        {
            height: 100%;
            width: 99%;
            padding: 5px;
            background-color: white; /* outer shadows  (note the rgba is red, green, blue, alpha) */
            -webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4);
            -moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5); /* rounded corners */
            -webkit-border-radius: 12px;
            -moz-border-radius: 7px;
            border-radius: 7px; /* gradients */
            background: -webkit-gradient(linear, left top, left bottom, 
color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5));
            background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%);
        }
        .FixedHeader {
            position: absolute;
            font-weight: bold;
        }     
        .myClass
        {
            border: solid 1px red;
        }
        .myClass input
        {
            background-color: green;
        }
        .myClass label
        {
            font-weight: bold;
        }
        
        .txtbox1
        {
            background-image: url(images/bg.png);
            border: 1px solid #6297BC;
        }
        .modalBackground
        {
            height: 100%;
            background-color: gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        .modalpopup
        {
            background-color: #EEEEEE;
            border: 1px groove gray;
            padding: 3px;
            font-family: Verdana;
            }
        .divWaiting
        {
            position: absolute;
            background-color: #FAFAFA;
            z-index: 2147483647 !important;
            opacity: 0.8;
            overflow: hidden;
            text-align: center;
            top: 0;
            left: 0;
            height: 100%;
            width: 100%;
            padding-top: 60%;
        }
        ::-webkit-scrollbar
        {
            width: 12px; /* for vertical scrollbars */
            height: 12px; /* for horizontal scrollbars */
        }
        
        ::-webkit-scrollbar-track
        {
            background: rgba(0, 0, 0, 0.1);
        }
        
        ::-webkit-scrollbar-thumb
        {
            background: rgba(0, 0, 0, 0.5);
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSearch" />
            <asp:PostBackTrigger ControlID="btnAdvanceSearch" />
            <asp:PostBackTrigger ControlID="btnDynamicSearch" />
        </Triggers>
        <ContentTemplate>
            <div class="gradientBoxesWithOuterShadows" style="margin-top: 10px">
                <table>
                    &nbsp;&nbsp;&nbsp;
                    <tr>
                        <td style="text-align: right; width: 150px">
                            <asp:Label ID="lblDocType" runat="server" Font-Bold="True" Text="Document Type"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlDocType" runat="server" AutoPostBack="true" CssClass="txtbox1"
                                Height="20px" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: left; width: 120px">
                            &nbsp;
                            <asp:Label ID="lblSearch" runat="server" Font-Bold="True" Text="Search By DocID"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="txtbox1" Height="20px" Width="150px"></asp:TextBox>
                        </td>
                        &nbsp;&nbsp;&nbsp;
                        <td>
                            <asp:ImageButton ID="btnSearch" runat="server" Height="25px" ImageUrl="~/Images/1.png"
                                ToolTip="Search  " />
                        </td>
                        &nbsp;&nbsp;&nbsp;
                        <td>
                            <asp:ImageButton ID="btnAdvanceSearch" runat="server" Height="25px" ImageUrl="~/Images/2.png"
                                ToolTip="Advance Search  " />
                        </td>
                        &nbsp;&nbsp;&nbsp;
                        <td>
                            <asp:ImageButton ID="btnCreateFolder" runat="server" AlternateText="Create Folder"
                                Height="25px" ImageUrl="~/images/4.png" OnClick="AddFolderHit" ToolTip="Create Folder" />
                        </td>
                    </tr>
                    </tr>
                </table>
                <br />
                <div align="center">
                    <asp:Label ID="lblmsg1" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div align="center">
                    <asp:Label ID="lblmsg" runat="server" ForeColor="Red"></asp:Label>
                </div>
                   <asp:Panel runat="server" ID="pngv" Width="960px" Height="440px" ScrollBars="Both">
                   <div align="center">
                    <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                        <ProgressTemplate>
                            <div id="Layer1" style="position: absolute; z-index: 1; left: 45%; top: 50%;">
                                <asp:Image ID="Image1" runat="server" Height="40px" ImageUrl="~/Images/spinner4-black.gif" />
                                <b style="color: #2C3539">&nbsp;Please wait...</b>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:GridView ID="gvPending" AllowSorting="True" pagesize="100" runat="server"
                        CellPadding="3" Width="100%"  BorderStyle="None" BorderColor="Green"
                        BorderWidth="1px" Font-Size="Small" style="overflow:auto" HeaderStyle-CssClass="FixedHeader">
                        <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green"
                            BorderWidth="1px" ForeColor="Black" />
                        <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                        <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                        <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px"
                            ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                        <Columns>
                            <asp:TemplateField HeaderText="Document Detail">
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="ChkDocid" class="myClass" />&nbsp;&nbsp;&nbsp;
                                    <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                        <%#DataBinder.Eval(Container.DataItem, "tid")%>
                                    </a>
                                </ItemTemplate>
                                <ItemStyle Width="120px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:GridView>
                    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
                        <tr>
                            <td style="width: 34%; text-align: center;">
                                <asp:Label ID="lbltotpending" runat="server" Style="text-align: center; color: #598526;"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                   </asp:Panel>
                
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlFields"
        TargetControlID="btnShowPopupForm" CancelControlID="btnCloseForm" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <%--<div id="main" >--%>
    <asp:Panel ID="pnlFields" runat="server" Width="750px" Height="430px" BackColor="White"
        Style="display: none; overflow: scroll" class="gradientBoxesWithOuterShadows">
        <table cellspacing="0px" cellpadding="0px" width="100%">
            <tr>
                <td width="980px">
                    <h2 align="center">
                        Advance Search</h2>
                </td>
                <td style="width: 20px">
                    <asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                <tr>
                                    <td style="width: 100%">
                                        <asp:Label ID="lblForm" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <div style="margin-left: 600px">
            <asp:ImageButton ID="btnDynamicSearch" runat="server" Height="25px" ImageUrl="~/Images/Asearch.png"
                ToolTip="Search  " />
        </div>
    </asp:Panel>
    <asp:Button ID="btnShowPopupAddFolder" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnAddFolder_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupAddFolder"
        PopupControlID="pnlPopupAddFolder" CancelControlID="btnCloseAddFolder" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupAddFolder" runat="server" Width="550px" Height="270px" BackColor="white"
        BorderColor="Gray" CssClass="modalpopup">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="95%">
                <tr>
                    <td style="width: 480px">
                        <h3>
                            Add Folder/Files</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseAddFolder" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelAddFolder" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table cellspacing="4px" cellpadding="0px" width="550px" Height="170px"  border="0px">
                                    <tr>
                                        <td align="left">
                                            <b style="margin-left:21px">Folder Name :</b>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:TextBox ID="txtFolderName" runat="server" Width="190px" CssClass="txtbox1" Height="20px"></asp:TextBox><br /><br />
                                             <b>&nbsp;&nbsp;&nbsp;&nbsp; File Name(Prefix) :&nbsp;&nbsp;&nbsp;&nbsp;</b>&nbsp; &nbsp;
                                            <asp:TextBox ID="txtFileName" runat="server" Width="190px" CssClass="txtbox1" Height="20px"></asp:TextBox><br /><br />
                                            &nbsp;
                                            <b>&nbsp;&nbsp;&nbsp;File Name(Suffix) :&nbsp;&nbsp;&nbsp;&nbsp;</b>
                                            &nbsp;&nbsp;
                                            <asp:DropDownList ID="ddlFieldName" runat="server" Width="190px" CssClass="txtbox" Height="20px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <caption>
                                        <br /><br />
                                        </td>
                                        </tr>
                                        <tr>
                                        <td>
                                        </td>
                                        </tr>
                                          <tr>
                                        <td>
                                        </td>
                                        </tr>
                                        <tr>
                                        <td align="left">
                                                 <strong >&nbsp;&nbsp;&nbsp;&nbsp; File Type :</strong><b>&nbsp;&nbsp;&nbsp;
                                                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Portable PDF</b>
                                                <asp:CheckBox ID="chkPDF" runat="server" />
                                                &nbsp;&nbsp; <b>Attachments</b><asp:CheckBox ID="chkAttachment" runat="server" />
                                                &nbsp;&nbsp; <b>Cover Sheet</b><asp:CheckBox ID="chkCoverSheet" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                        </tr>
                                        <caption>
                                            <asp:Label ID="lblMsgAddFolder" runat="server" BackColor="White" 
                                                Font-Bold="True" Font-Names="Verdana" Font-Size="X-Small" ForeColor="White" 
                                                Style="text-align: center" Width="100%"></asp:Label>
                                            <br />
                                            <tr>
                                                <td align="center">
                                                    <asp:ImageButton ID="btnDownload" runat="server" AlternateText="Download Files" 
                                                        Height="25px" ImageUrl="~/images/3.png" ToolTip="Download Files" />
                                                    <%--<asp:Button ID="btnDownload" runat="server" Height="25px" Text="Download" ToolTip="Download Document" />--%>
                                                    <br />
                                                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                                        <ProgressTemplate>
                                                            <div ID="Layer1" style="position: absolute; z-index: 1; left: 45%; top: 50%;">
                                                                <asp:Image ID="Image2" runat="server" Height="40px" 
                                                                    ImageUrl="~/Images/spinner4-black.gif" />
                                                                <b style="color: #2C3539">&nbsp;Please wait...</b>
                                                            </div>
                                                        </ProgressTemplate>
                                                    </asp:UpdateProgress>
                                                </td>
                                            </tr>
                                        </caption>
                                    </caption>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
