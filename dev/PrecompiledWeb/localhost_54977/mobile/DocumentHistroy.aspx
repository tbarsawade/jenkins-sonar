<%@ page language="VB" autoeventwireup="false" inherits="mobile_DocumentHistroy, App_Web_dzjktowh" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, usesscr-scalable=yes, minimum-scale=1.0, maximum-scale=1.0" />
    <title></title>
   <link href="styles/style.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="Sc1">
        </asp:ScriptManager>
        <div class="box">
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr>
                    <td style="width: 100%">
                        <div id="tabAppr" style="min-height: 300px;">

                            <asp:DropDownList ID="ddlGrdHdr" runat="server" CssClass="txtBox" Height="25px"
                                AutoPostBack="True">
                            </asp:DropDownList>
                            <asp:DropDownList ID="ddlGrdVal" runat="server" CssClass="txtBox"  Height="25px"
                                AutoPostBack="True">
                            </asp:DropDownList>
                            <asp:UpdatePanel ID="updAction" runat="server">
                                <ContentTemplate>

                                    <br />
                                    <asp:GridView ID="gvAction" runat="server" EmptyDataText="Record does not exists." AllowPaging="true"
                                        ShowFooter="false" AutoGenerateColumns="False" AllowSorting="true"
                                        CellPadding="3" DataKeyNames="tid" Width="100%"
                                        PageSize="10" BorderStyle="None" BorderColor="Green"
                                        BorderWidth="1px" Font-Size="Small">

                                        <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                        <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                        <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px"
                                            Height="25px" ForeColor="black"
                                            CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="System Doc. ID">
                                                <ItemTemplate>
                                                    <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                                        <%#DataBinder.Eval(Container.DataItem, "tid")%> </a>
                                                </ItemTemplate>
                                                <ItemStyle Width="100px" HorizontalAlign="left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Applied On" SortExpression="adate" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <a class="detail" href="#" style="text-decoration: none;" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                                        <%#DataBinder.Eval(Container.DataItem, "adate", "{0:dd-MMM-yyyy}")%> </a>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="left" Width="100px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Subject" SortExpression="Documenttype" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <a class="detail" href="#" style="text-decoration: none;" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                                        <%#DataBinder.Eval(Container.DataItem,"Documenttype") %> </a>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:TemplateField>
                                           <%-- <asp:TemplateField HeaderText="Status" SortExpression="curstatus" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <a class="detail" href="#" style="color: #004388; text-decoration: none;" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')"><%#DataBinder.Eval(Container.DataItem,"curstatus") %></a>

                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Created By" SortExpression="username" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <a class="detail" href="#" style="text-decoration: none;" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')"><%#DataBinder.Eval(Container.DataItem,"username") %></a>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="To" SortExpression="apusername" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <a class="detail" href="#" style="text-decoration: none;" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')"><%#DataBinder.Eval(Container.DataItem,"apusername") %></a>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Pending From" SortExpression="fdate" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <a class="detail" href="#" style="text-decoration: none;" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')"><%#DataBinder.Eval(Container.DataItem,"fdate") %></a>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                                            </asp:TemplateField>--%>
                                        </Columns>
                                        <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                        <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                        <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                        <SortedDescendingHeaderStyle BackColor="#93451F" />
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                    </asp:GridView>
                                    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td style="width: 33%; text-align: left;">
                                                <asp:LinkButton ID="lnkprev" runat="server" CommandName="Previous" OnCommand="ChangePage" Text="Previous" Style="text-align: left;"></asp:LinkButton>&nbsp;
                                            </td>
                                            <td style="width: 34%; text-align: center;">
                                                <asp:Label ID="lbltotalpages" runat="server" Style="text-align: center; color: #598526;"></asp:Label>
                                            </td>
                                            <td style="width: 33%; text-align: right;">
                                                <asp:LinkButton ID="lnknext" runat="server" Text="Next" CommandName="Next" OnCommand="ChangePage" Style="text-align: right;"></asp:LinkButton>
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
    </form>
</body>
</html>
