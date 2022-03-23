<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="WSInward, App_Web_bv10wntb" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
        </Triggers>

        <ContentTemplate>
            <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
            <script src="http://ajax.microsoft.com/ajax/beta/0911/Start.debug.js" type="text/javascript"></script>
            <script src="http://ajax.microsoft.com/ajax/beta/0911/extended/ExtendedControls.debug.js" type="text/javascript"></script>

            <style type="text/css">
                .fancy-green .ajax__tab_header
                {
                    background: url(images/green_bg_Tab.gif) repeat-x;
                    cursor: pointer;
                }

                .fancy-green .ajax__tab_hover .ajax__tab_outer, .fancy-green .ajax__tab_active .ajax__tab_outer
                {
                    background: url(images/green_left_Tab.gif) no-repeat left top;
                }

                .fancy-green .ajax__tab_hover .ajax__tab_inner, .fancy-green .ajax__tab_active .ajax__tab_inner
                {
                    background: url(images/green_right_Tab.gif) no-repeat right top;
                }

                .fancy .ajax__tab_header
                {
                    font-size: 13px;
                    font-weight: bold;
                    color: #000;
                    font-family: sans-serif;
                }

                    .fancy .ajax__tab_active .ajax__tab_outer, .fancy .ajax__tab_header .ajax__tab_outer, .fancy .ajax__tab_hover .ajax__tab_outer
                    {
                        height: 46px;
                    }

                    .fancy .ajax__tab_active .ajax__tab_inner, .fancy .ajax__tab_header .ajax__tab_inner, .fancy .ajax__tab_hover .ajax__tab_inner
                    {
                        height: 46px;
                        margin-left: 16px; /* offset the width of the left image */
                    }

                    .fancy .ajax__tab_active .ajax__tab_tab, .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_header .ajax__tab_tab
                    {
                        margin: 16px 16px 0px 0px;
                    }

                .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_active .ajax__tab_tab
                {
                    color: #fff;
                }

                .fancy .ajax__tab_body
                {
                    font-family: Arial;
                    font-size: 10pt;
                    border-top: 0;
                    border: 1px solid #999999;
                    padding: 8px;
                    background-color: #ffffff;
                }

                .CS
                {
                    background-color: white;
                    color: #99ae46;
                    border: 1px solid #99ae46;
                    font: Verdana 10px;
                    padding: 1px 4px;
                    font-family: Palatino Linotype, Arial, Helvetica, sans-serif;
                }

                .GVFixedHeader
                {
                    font-weight: bold;
                    background-color: Green;
                    background: url(images/gridheadmenu.png) repeat-x;
                    margin-right: 2px;
                    position: relative;
                    top: expression(this.parentNode.parentNode.parentNode.scrollTop-1);
                }

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
                    background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5));
                    background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%);
                    background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
                    background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
                    filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
                }

                .style2
                {
                    width: 30%;
                }

                .dyn
                {
                    text-align: left;
                    z-index: 1000;
                }

                dynamicMenu
                {
                    z-index: 100;
                    position: absolute;
                }
            </style>
            <div>
                <asp:Label ID="lblss" runat="server"></asp:Label>
            </div>
            <div style="width: 1000px; height: 400px;">
                <ajax:TabContainer ID="TabContainer1" Width="100%" runat="server" CssClass="fancy fancy-green" ActiveTabIndex="1">
                    <ajax:TabPanel ID="tbpnluser" runat="server">
                        <HeaderTemplate>
                            Add Inward
                        </HeaderTemplate>


                        <ContentTemplate>
                            <asp:Panel ID="UserReg" runat="server" ScrollBars="Vertical">
                                <br />
                                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="True" ForeColor="Red"
                                                Font-Size="Small"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <br />
                                        <td style="width: 50%;" align="center">
                                            <asp:Label ID="lbldtype" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" runat="server" Text="Document Type"></asp:Label></td>
                                        <td style="width: 50%;" align="left">
                                            <br />
                                            <asp:DropDownList ID="ddldtype" CssClass="textbox" Width="300px" runat="server" AutoPostBack="true"></asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="lblur" runat="server" Text="URI" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lbluri" Font-Bold="true" Text="http://www.myndsaas.com/MyndBPMWS.svc/SaveData" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="lblf" runat="server" Visible="false" Text="Format" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lblformat" Visible="false" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="lble" Visible="false" runat="server" Text="Example" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lblexample" Visible="false" runat="server"></asp:Label></td>
                                    </tr>
                                </table>
                            </asp:Panel>

                        </ContentTemplate>

                                            </ajax:TabPanel>
                    <ajax:TabPanel ID="tbpnlusrdetails" runat="server">
                        <HeaderTemplate>
                            Edit Inward
                        </HeaderTemplate>

 <ContentTemplate>
                            <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical">
                                <br />
                                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red"
                                                Font-Size="Small"></asp:Label></td>
                                    </tr>
                                    <caption>
                                        <br />
                                        <tr>
                                            <td align="center" style="width: 50%;">
                                                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Document Type"></asp:Label>
                                            </td>
                                            <td align="left" style="width: 50%;">
                                                <br />
                                                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" CssClass="textbox" Width="300px">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </caption>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label3" runat="server" Text="URI" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="Label4" Font-Bold="True" Text="http://www.myndsaas.com/MyndBPMWS.svc/UpdateData" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label5" runat="server" Visible="False" Text="Format" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="Label6" Visible="False" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label7" Visible="False" runat="server" Text="Example" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="Label8" Visible="False" runat="server"></asp:Label></td>
                                    </tr>
                                </table>
                            </asp:Panel>















                        </ContentTemplate>















                    </ajax:TabPanel>

                    <ajax:TabPanel ID="TabPanel1" runat="server">
                        <HeaderTemplate>
                            Action Driven
                        </HeaderTemplate>

 <ContentTemplate>
                            <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical">
                                <br />
                                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lblactmsg" runat="server" Font-Bold="True" ForeColor="Red"
                                                Font-Size="Small"></asp:Label></td>
                                    </tr>
                                    <caption>
                                        <br />
                                        <tr>
                                            <td align="center" style="width: 50%;">
                                                <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Document Type"></asp:Label>
                                            </td>
                                            <td align="left" style="width: 50%;">
                                                <br />
                                                <asp:DropDownList ID="ddlactdr" runat="server" AutoPostBack="True" CssClass="textbox" Width="300px">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </caption>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label11" runat="server" Text="URI" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="Label12" Font-Bold="True" Text="http://myndsolution.com/MyndBPMWS.svc/DocumentApproval" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label13" runat="server" Visible="False" Text="Format" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lblacrf" Visible="False" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label15" Visible="False" runat="server" Text="Example" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lblcte" Visible="False" runat="server"></asp:Label></td>
                                    </tr>
                                </table>
                            </asp:Panel>

                             </ContentTemplate>
                                            </ajax:TabPanel>
                    <ajax:TabPanel ID="TabpanelChild" runat="server">
                        <HeaderTemplate>
                            Child Form
                        </HeaderTemplate>

 <ContentTemplate>
                            <asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical">
                                <br />
                                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lblchildform" Font-Names="Verdana" runat="server" Font-Bold="True" ForeColor="Red"
                                                Font-Size="Small"></asp:Label></td>
                                    </tr>
                                    <caption>
                                        <br />
                                        <tr>
                                            <td align="center" style="width: 50%;">
                                                <asp:Label ID="Label14" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Document Type"></asp:Label>
                                            </td>
                                            <td align="left" style="width: 50%;">
                                                <br />
                                                <asp:DropDownList ID="ddlchild" runat="server" AutoPostBack="True" CssClass="textbox" Width="300px">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </caption>
                                  
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label18" runat="server" Visible="False" Text="Format" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lblchildFormat" Font-Names="Verdana" Visible="False" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <asp:Label ID="Label20" Visible="False" runat="server" Text="Example" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy"></asp:Label></td>
                                        <td>
                                            <br />
                                            <br />
                                            <asp:Label ID="lblchildexample" Visible="False" runat="server"></asp:Label></td>
                                    </tr>
                                </table>
                            </asp:Panel>

                             </ContentTemplate>
                                            </ajax:TabPanel>
                </ajax:TabContainer>

               <%-- <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
                <ajax:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
                    TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
                    CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajax:ModalPopupExtender>

                <asp:Panel ID="pnlPopupEdit" runat="server" Width="700px" BackColor="aqua">
                    <div class="box">
                        <table cellspacing="2px" cellpadding="2px" width="100%">
                            <tr>
                                <td style="width: 580px">
                                    <h3>Update WSInward</h3>
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
                                                    <td style="width: 125px" align="left"><b>Form Name </b></td>
                                                    <td align="left">
                                                        <asp:Label ID="lblfn" runat="server"></asp:Label>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td style="width: 125px" align="left"><b>Unique Keys</b></td>

                                                    <td style="width: 300px" align="left">
                                                        <asp:TextBox ID="txtuk" Width="500px" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </td>

                                                </tr>
                                                <tr align="left">
                                                    <td style="width: 125px" align="left"><b>Keys</b></td>
                                                    <td>
                                                        <div style="width: 100%; height: 200px; overflow: scroll;">
                                                            <asp:CheckBoxList ID="chkflds" runat="server" AutoPostBack="true">
                                                            </asp:CheckBoxList>
                                                        </div>
                                                    </td>

                                                </tr>

                                            </table>
                                            <div style="width: 100%; text-align: right">
                                                <asp:Button ID="btnActEdit" OnClick="editrecord" runat="server" Text="Update"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>--%>





                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                    <ProgressTemplate>
                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 25%; right: 543px;">
                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                            please wait...
                        </div>

                    </ProgressTemplate>
                </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>



</asp:Content>

