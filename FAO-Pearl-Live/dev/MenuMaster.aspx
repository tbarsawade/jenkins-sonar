<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" CodeFile="MenuMaster.aspx.vb" Inherits="MM" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
        </Triggers>
        <ContentTemplate>
            <asp:UpdateProgress ID="updateProgress" runat="server">
                <ProgressTemplate>
                    <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 30%">
                        <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                        please wait...
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <link rel="Stylesheet" href="http://ajax.microsoft.com/ajax/beta/0911/extended/tabs/tabs.css" />
             <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
            <script src="http://ajax.microsoft.com/ajax/beta/0911/Start.debug.js" type="text/javascript"></script>
            <script src="http://ajax.microsoft.com/ajax/beta/0911/extended/ExtendedControls.debug.js" type="text/javascript"></script>
            <script type="text/javascript">

                function SelectCheckboxes(headerChk, columnIndex) {

                    var IsChecked = headerChk.checked;
                    var tbl = document.getElementById("<%=Gvrole.ClientID %>");
                    for (i = 1; i < tbl.rows.length; i++) {
                        var curTd = tbl.rows[i].cells[columnIndex];
                        var item = curTd.getElementsByTagName('input');
                        for (j = 0; j < item.length; j++) {
                            if (item[j].type == "checkbox") {
                                if (item[j].checked != IsChecked) {
                                    item[j].click();
                                }
                            }

                        }
                    }
                }

            </script>
            <style type="text/css">
                .fancy-green .ajax__tab_header {
                    background: url(images/green_bg_Tab.gif) repeat-x;
                    cursor: pointer;
                }

                .fancy-green .ajax__tab_hover .ajax__tab_outer, .fancy-green .ajax__tab_active .ajax__tab_outer {
                    background: url(images/green_left_Tab.gif) no-repeat left top;
                }

                .fancy-green .ajax__tab_hover .ajax__tab_inner, .fancy-green .ajax__tab_active .ajax__tab_inner {
                    background: url(images/green_right_Tab.gif) no-repeat right top;
                }

                .fancy .ajax__tab_header {
                    font-size: 13px;
                    font-weight: bold;
                    color: #000;
                    font-family: sans-serif;
                }

                    .fancy .ajax__tab_active .ajax__tab_outer, .fancy .ajax__tab_header .ajax__tab_outer, .fancy .ajax__tab_hover .ajax__tab_outer {
                        height: 46px;
                    }

                    .fancy .ajax__tab_active .ajax__tab_inner, .fancy .ajax__tab_header .ajax__tab_inner, .fancy .ajax__tab_hover .ajax__tab_inner {
                        height: 46px;
                        margin-left: 16px; /* offset the width of the left image */
                    }

                    .fancy .ajax__tab_active .ajax__tab_tab, .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_header .ajax__tab_tab {
                        margin: 16px 16px 0px 0px;
                    }

                .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_active .ajax__tab_tab {
                    color: #fff;
                }

                .fancy .ajax__tab_body {
                    font-family: Arial;
                    font-size: 10pt;
                    border-top: 0;
                    border: 1px solid #999999;
                    padding: 8px;
                    background-color: #ffffff;
                }

                .CS {
                    background-color: white;
                    color: #99ae46;
                    border: 1px solid #99ae46;
                    font: Verdana 10px;
                    padding: 1px 4px;
                    font-family: Palatino Linotype, Arial, Helvetica, sans-serif;
                }

                .GVFixedHeader {
                    font-weight: bold;
                    background-color: Green;
                    background: url(images/gridheadmenu.png) repeat-x;
                    margin-right: 2px;
                    position: relative;
                    top: expression(this.parentNode.parentNode.parentNode.scrollTop-1);
                }

                .gradientBoxesWithOuterShadows {
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

                .style2 {
                    width: 30%;
                }

                .dyn {
                    text-align: left;
                    z-index: 1000;
                }

                dynamicMenu {
                    z-index: 100;
                    position: absolute;
                }
            </style>
            <div class="container-fluid">
                <div class="form">
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <asp:Label ID="lblmsg" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <div class="col-md-8 col-sm-8">

                                    <ajax:TabContainer ID="TabContainer1" runat="server" CssClass="fancy fancy-green" ActiveTabIndex="2">
                                        <ajax:TabPanel ID="tbpnluser" runat="server">
                                            <HeaderTemplate>
                                                Add Root Menu
                                            </HeaderTemplate>
                                            <ContentTemplate>
                                                <asp:Panel ID="UserReg" runat="server">

                                                    <br />
                                                    <table width="100%" border="1" style="border-style: solid;">
                                                        <tr>
                                                            <td align="right">
                                                                <span>Menu Type</span>

                                                            </td>
                                                            <td align="left">

                                                                <asp:Label ID="lblmenu" Text="New" runat="server" Font-Bold="True" ForeColor="Red"
                                                                    Width="97%" Font-Size="Small"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" class="style3" style="width: 100px;">
                                                                <asp:Label ID="lblChild" runat="server" Text="Menu Name" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" class="style4">
                                                                <asp:TextBox ID="txtchild" runat="server" Width="200px"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="REValphaOnly" ForeColor="Red" runat="server" ErrorMessage="* Please enter only alphanumeric."
                                                                    ControlToValidate="txtchild" ValidationExpression="^[a-zA-Z0-9 ]+$"></asp:RegularExpressionValidator>

                                                            </td>
                                                        </tr>


                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="lbldord" runat="server" Text="Display Order" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 200px;">
                                                                <asp:TextBox ID="txtdord" runat="server" Width="200px"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ForeColor="Red" runat="server" ErrorMessage="* Please enter only numeric."
                                                                    ControlToValidate="txtdord" ValidationExpression="^[0-9]+$"></asp:RegularExpressionValidator>

                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="Label15" runat="server" Text="Is Mobile" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 200px;">
                                                                <asp:CheckBox ID="chkparent" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" style="width: 100px;">
                                                                <asp:Label ID="lblImage" runat="server" Text="Icon image" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 220px;">
                                                                <asp:FileUpload ID="uploadimage" runat="server" CssClass="CS" Width="200px" />
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" style="width: 100px;">
                                                                <asp:Label ID="lblpicon" runat="server" Text="Icon image" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 220px;">
                                                                <asp:Image ID="imgicon" runat="server" />
                                                                &nbsp;
                                                            </td>


                                                        </tr>
                                                        <tr>
                                                            <td align="right" colspan="2">
                                                                <br />
                                                                <br />


                                                                <asp:Button ID="btnsave" runat="server" Text="Save" CssClass="btnNew" />
                                                            </td>

                                                        </tr>


                                                    </table>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </ajax:TabPanel>
                                        <ajax:TabPanel ID="tbpnlusrdetails" runat="server">
                                            <HeaderTemplate>
                                                Add Submenu
                                            </HeaderTemplate>
                                            <ContentTemplate>
                                                <asp:Panel ID="Panel1" runat="server">
                                                    <table width="100%" border="1" style="border-style: solid;">
                                                        <tr>
                                                            <td align="right">
                                                                <span>Select Parent Menu</span>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlpar" Width="200px" runat="server"></asp:DropDownList>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="Label9" Width="100px" runat="server" Text="Page Link Type"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 200px;">
                                                                <asp:DropDownList ID="ddlplt1" OnSelectedIndexChanged="ddlplt1_SelectedIndexChanged" AutoPostBack="True" runat="server" CssClass="textbox"
                                                                    Width="200px">
                                                                    <asp:ListItem>Select</asp:ListItem>
                                                                    <asp:ListItem>Static</asp:ListItem>
                                                                    <asp:ListItem>Master</asp:ListItem>
                                                                    <asp:ListItem>Document</asp:ListItem>
                                                                    <asp:ListItem>Report</asp:ListItem>
                                                                    <asp:ListItem>Calendar</asp:ListItem>
                                                                    <asp:ListItem>Master Calendar</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>


                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="Label10" runat="server" Text="Page Link" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 200px;">
                                                                <asp:DropDownList ID="ddlpl1" AutoPostBack="True" runat="server" CssClass="textbox" Width="200px">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" class="style3" style="width: 100px;">
                                                                <asp:Label ID="Label8" runat="server" Text="Menu Name" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" class="style4">
                                                                <asp:TextBox ID="txtchild1" runat="server" Width="200px"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ForeColor="Red" runat="server" ErrorMessage="* Please enter only alphanumeric."
                                                                    ControlToValidate="txtchild1" ValidationExpression="^[a-zA-Z0-9 ]+$"></asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="Label11" runat="server" Text="Display Order" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 200px;">
                                                                <asp:TextBox ID="txtdord1" runat="server" Width="200px"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ForeColor="Red" runat="server" ErrorMessage="* Please enter only numeric."
                                                                    ControlToValidate="txtdord1" ValidationExpression="^[0-9]+$"></asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="Label1" runat="server" Text="Is Mobile" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 200px;">
                                                                <asp:CheckBox ID="chkmobile" runat="server" />
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right" style="width: 100px;">
                                                                <asp:Label ID="Label12" runat="server" Text="Icon image" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 220px;">
                                                                <asp:FileUpload ID="fp2" runat="server" CssClass="CS" Width="200px" />
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" style="width: 100px;">
                                                                <asp:Label ID="Label13" runat="server" Text="Icon image" Width="100px"></asp:Label>
                                                            </td>
                                                            <td align="left" style="width: 220px;">
                                                                <asp:Image ID="img2" runat="server" />
                                                                &nbsp;
                                                            </td>


                                                        </tr>
                                                        <tr>
                                                            <td align="right" colspan="2">
                                                                <asp:Button ID="btnsavesubmenu" runat="server" Text="Save" CssClass="btnNew" />
                                                            </td>

                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </ajax:TabPanel>
                                        <ajax:TabPanel ID="tbpnljobdetails" runat="server">
                                            <HeaderTemplate>
                                                Edit / Delete Menu
                                            </HeaderTemplate>
                                            <ContentTemplate>
                                                <asp:Panel ID="Panel2" runat="server">
                                                    <div id="item" style="text-align: left; overflow: scroll;">
                                                        <table width="100%" border="1" style="border-style: solid;">

                                                            <tr>
                                                                <td>

                                                                    <asp:Menu ID="MenuPreview" runat="server"
                                                                        Orientation="Horizontal" Style="z-index: 9999999;"
                                                                        ForeColor="Black" Height="35px" Width="600px" RenderingMode="Table" BackColor="#F7F6F3"
                                                                        DynamicHorizontalOffset="2" Font-Bold="False" Font-Names="Verdana" Font-Size="Small"
                                                                        StaticSubMenuIndent="10px">
                                                                        <DynamicHoverStyle BackColor="#D8EFCA" BorderColor="#FF6600" BorderStyle="Ridge"
                                                                            BorderWidth="1px" ForeColor="#598527" />
                                                                        <DynamicMenuItemStyle BackColor="#DDDBF8" BorderColor="Silver" BorderStyle="Solid"
                                                                            BorderWidth="2px" HorizontalPadding="5px" VerticalPadding="2px" />
                                                                        <DynamicMenuStyle BorderColor="#DDDBF8" BackColor="#F7F6F3" CssClass="dyn" />
                                                                        <DynamicSelectedStyle BackColor="#5D7B9D" />
                                                                        <StaticHoverStyle BorderColor="#FF954F" ForeColor="#598527" />
                                                                        <StaticMenuItemStyle BackColor="#DDDBF8" BorderColor="#666666" HorizontalPadding="5px"
                                                                            VerticalPadding="2px" />
                                                                        <StaticMenuStyle BorderColor="#83BCD4" />
                                                                        <StaticSelectedStyle ForeColor="#DDDBF8" BackColor="#5D7B9D" BorderColor="#FF6600"
                                                                            BorderStyle="Solid" BorderWidth="2px" />
                                                                        <StaticItemTemplate>
                                                                            <%# Eval("Text") %>
                                                                        </StaticItemTemplate>
                                                                    </asp:Menu>

                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table width="100%" border="1" style="border-style: solid;">
                                                            <tr>
                                                                <td align="left">
                                                                    <span>Parent Menu</span>

                                                                </td>
                                                                <td align="left">
                                                                    <table border="1">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lbl3" runat="server" Font-Bold="True" ForeColor="Red"
                                                                                    Width="97%" Font-Size="Small"></asp:Label></td>
                                                                            <td>
                                                                                <asp:Label ID="lblchngp" runat="server" ForeColor="Navy" Visible="False" Text="Change Parent"></asp:Label></td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlchngp" Visible="False" runat="server" AutoPostBack="True"></asp:DropDownList></td>

                                                                        </tr>

                                                                    </table>

                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="left">
                                                                    <asp:Label ID="Label3" Width="100px" runat="server" Text="Page Link Type"></asp:Label>
                                                                </td>
                                                                <td align="left" style="width: 200px;">
                                                                    <asp:DropDownList ID="ddlpt2" AutoPostBack="True" runat="server" CssClass="textbox"
                                                                        Width="200px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>


                                                            <tr>
                                                                <td align="left">
                                                                    <asp:Label ID="Label4" runat="server" Text="Page Link" Width="100px"></asp:Label>
                                                                </td>
                                                                <td align="left" style="width: 200px;">
                                                                    <asp:DropDownList ID="ddlpl2" runat="server" AutoPostBack="True" CssClass="textbox" Width="200px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" class="style3" style="width: 100px;">
                                                                    <asp:Label ID="Label2" runat="server" Text="Menu Name" Width="100px"></asp:Label>
                                                                </td>
                                                                <td align="left" class="style4">
                                                                    <asp:TextBox ID="txtchild2" runat="server" Width="200px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ForeColor="Red" runat="server" ErrorMessage="* Please enter only alphanumeric."
                                                                        ControlToValidate="txtchild2" ValidationExpression="^[a-zA-Z0-9 ]+$"></asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:Label ID="Label5" runat="server" Text="Display Order" Width="100px"></asp:Label>
                                                                </td>
                                                                <td align="left" style="width: 200px;">
                                                                    <asp:TextBox ID="txtdord2" runat="server" Width="200px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ForeColor="Red" ErrorMessage="* Please enter only numeric."
                                                                        ControlToValidate="txtdord2" ValidationExpression="^[0-9]+$"></asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="left">
                                                                    <asp:Label ID="Label14" runat="server" Text="Is Mobile" Width="100px"></asp:Label>
                                                                </td>
                                                                <td align="left" style="width: 200px;">
                                                                    <asp:CheckBox ID="CheckBox1" runat="server" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" style="width: 100px;">
                                                                    <asp:Label ID="Label6" runat="server" Text="Icon image" Width="100px"></asp:Label>
                                                                </td>
                                                                <td align="left" style="width: 220px;">
                                                                    <asp:UpdatePanel ID="updpanlFile" runat="server" UpdateMode="Conditional">
                                                                        <ContentTemplate>
                                                                            <asp:FileUpload ID="fp3" runat="server" CssClass="CS" Width="200px" />
                                                                            <asp:Button ID="btnupdate" runat="server" Text="Update" CssClass="btnNew" />
                                                                        </ContentTemplate>
                                                                        <Triggers>
                                                                            <asp:PostBackTrigger ControlID="btnupdate" />
                                                                        </Triggers>
                                                                    </asp:UpdatePanel>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" style="width: 100px;">
                                                                    <asp:Label ID="Label7" runat="server" Text="Icon image" Width="100px"></asp:Label>
                                                                </td>
                                                                <td align="left" style="width: 220px;">
                                                                    <asp:Image ID="img3" runat="server" />
                                                                    &nbsp;
                                                                </td>


                                                            </tr>
                                                            <tr>
                                                                <td align="right" colspan="2">
                                                                    <fieldset>

                                                                        <asp:Button ID="btndelete" runat="server" Visible="False" OnClientClick="return confirm('Are you sure you want to delete record?');" CssClass="btnNew" Text="Delete" />

                                                                    </fieldset>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </ajax:TabPanel>
                                    </ajax:TabContainer>

                                </div>
                                <div class="col-md-4 col-sm-4">
                                    <div id="scroll" style="text-align: left; margin-top: 20px;  height: 500px; overflow: auto;">
                                        <asp:GridView ID="Gvrole" runat="server" AutoGenerateColumns="False" GridLines="Both"
                                            BackColor="White" BorderColor="#3366CC" BorderStyle="Double" BorderWidth="1px"
                                            CellPadding="4">
                                            <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                                            <HeaderStyle Height="38px" CssClass="GVFixedHeader" BorderColor="white" BorderStyle="Double"
                                                HorizontalAlign="Center" BorderWidth="2px" Font-Bold="True" ForeColor="Black" />
                                            <PagerStyle BackColor="#99CCCC" ForeColor="#a2c445" HorizontalAlign="Left" />
                                            <RowStyle BackColor="White" ForeColor="#3c5201" />
                                            <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                            <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                            <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                            <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                            <SortedDescendingHeaderStyle BackColor="#002876" />
                                            <Columns>
                                                <asp:BoundField DataField="rolename" HeaderText="Roles" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-Width="80px" />
                                                <asp:TemplateField HeaderText="IsView" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAllV" onclick="SelectCheckboxes(this, 1)" runat="server" Text="IsView" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chbview" runat="server" value="1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IsCreate" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAllC" runat="server" onclick="SelectCheckboxes(this, 2)" Text="IsCreate" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chbcreate" runat="server" value="2" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IsEdit" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAllE" runat="server" onclick="SelectCheckboxes(this,3)" Text="IsEdit" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chbedit" runat="server" value="4" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IsDelete/IsLock" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAllD" runat="server" onclick="SelectCheckboxes(this, 4)" Text="IsDelete/IsLock" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chbdelete" runat="server" value="8" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

