<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Masters.aspx.vb" Inherits="mobile_Master" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, usesscr-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
    <title></title>
   <link href="styles/style.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="sp1">
    </asp:ScriptManager>
    <div class="box">
        <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
            <tr style="color: #000000" height="20px">
                <td style="text-align: left;">
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
                            <td style="width: 90px;">
                                <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small"
                                    ForeColor="black" Text="Field Name" Width="99%">
                                </asp:Label>
                            </td>
                            <td style="width: 170px;">
                                <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox" Font-Bold="True"
                                    Font-Names="Verdana" Font-Size="Small" Width="99%">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 50px;">
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small"
                                    ForeColor="Black" Text="Value" Width="99%"></asp:Label>
                            </td>
                            <td style="width: 200px;">
                                <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox" Font-Bold="True" Font-Size="Small"
                                    Width="99%"></asp:TextBox>
                            </td>
                            <td style="text-align: right; width: 25px">
                                <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/search.png" />
                            </td>
                            <td style="text-align: right;">
                                &nbsp;
                                <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg"
                                    OnClick="Add" />
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
            <tr>
                <td style="height:20px"></td>
            </tr>
            <tr style="color: #000000">
                <td style="text-align: left" valign="top" width="100%">
                    <div style="overflow: auto; width: 100%">
                        <asp:GridView ID="gvData" runat="server" CssClass="GridView" CellPadding="2" DataKeyNames="tid"
                              AllowPaging="True" PageSize="15" Width="100%">
                            <FooterStyle CssClass="FooterStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <EditRowStyle CssClass="EmptyDataRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass="HeaderStyle" />
                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                            <Columns>
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <a class="detail" style="text-decoration: none;" href="../mobile/MasterHistory.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>">
                                            <img alt="Detail" src="../images/seeall.png" />
                                        </a>&nbsp;
                                        <a class="detail" style="text-decoration: none;" href="../mobile/MasterEdit.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>&SC=<%=DocumentType %>">
                                            <img alt="Edit" src="../images/edit.jpg" />
                                        </a>&nbsp;
                                    </ItemTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
