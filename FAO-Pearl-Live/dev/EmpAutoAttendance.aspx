<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false"
    CodeFile="EmpAutoAttendance.aspx.vb" Inherits="EmpAutoAttendance" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .gradientBoxesWithOuterShadows
        {
            height: 100%;
            width: 1000px;
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
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSearch" />
        </Triggers>
        <ContentTemplate>
            <br />
            <div style="margin-left: 10px">
                <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0"
                    class="m9">
                    <tr>
                        <td align="left">
                            <fieldset style="width: 300px; height: 150px;">
                                <legend style="color: Black; font-size: small;">Report Type</legend>
                                <br />
                                <asp:Panel ID="Panel2" runat="server" Height="100px" Style="display: block">
                                    <asp:DropDownList ID="ddlRtype" CssClass="txtBox" runat="server" Width="160px" AutoPostBack="True">
                                        <asp:ListItem Selected="True">Select Report</asp:ListItem>
                                        <asp:ListItem>Send Attendance</asp:ListItem>
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                    <asp:DropDownList ID="ddlcyc" CssClass="txtBox" runat="server" Width="160px" AutoPostBack="True">
                                        <asp:ListItem Selected="True">Select Cycle</asp:ListItem>
                                        <asp:ListItem>26 TO 25</asp:ListItem>
                                        <asp:ListItem>1 TO 31</asp:ListItem>
                                        <asp:ListItem>21 TO 20</asp:ListItem>
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="left">
                            <table>
                                <tr>
                                    <td>
                                        <fieldset style="width: 300px; height: 150px;">
                                            <legend style="color: Black; font-size: small;">Date Range</legend>
                                            <table>
                                                <tr>
                                                    <td><br />
                                                        <asp:Label ID="lblMonth" runat="server" Font-Bold="True" Text="Month: "></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlMonth" CssClass="txtBox" runat="server"
                                                            Width="110px">
                                                            <asp:ListItem Selected="True">JAN</asp:ListItem>
                                                            <asp:ListItem>FEB</asp:ListItem>
                                                            <asp:ListItem>MAR</asp:ListItem>
                                                            <asp:ListItem>APR</asp:ListItem>
                                                            <asp:ListItem>MAY</asp:ListItem>
                                                            <asp:ListItem>JUN</asp:ListItem>
                                                            <asp:ListItem>JUL</asp:ListItem>
                                                            <asp:ListItem>AUG</asp:ListItem>
                                                            <asp:ListItem>SEP</asp:ListItem>
                                                            <asp:ListItem>OCT</asp:ListItem>
                                                            <asp:ListItem>NOV</asp:ListItem>
                                                            <asp:ListItem>DEC</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <br />
                                                        <br />
                                                        <asp:Label ID="lblYear" runat="server" Font-Bold="True" Text="Year: "></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:DropDownList ID="ddlYear" CssClass="txtBox" runat="server" Width="110px">
                                                            <asp:ListItem Selected="True">2015</asp:ListItem>
                                                            <asp:ListItem>2016</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <br />
                                                        <br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="Left" width="35%">
                            &nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="btnNew" Height="25px" Text="Send"
                                ToolTip="Send" Width="70px" />
                        </td>
                        <td align="center" class="m8b" width="5%">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <div>
                                <br />
                                <br />
                                <asp:Label ID="lblMsg" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Medium"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <br />
            <asp:Panel runat="server" ID="pnlgrd" ScrollBars="Auto" Visible="true">
                <div align="center" style="width: 1000px">
                    <asp:GridView ID="gvPending" AllowSorting="true" ShowFooter="false" AllowPaging="true"
                        runat="server" AutoGenerateColumns="true" CellPadding="3" Width="800px" PageSize="15"
                        BorderStyle="none" BorderColor="Green" BorderWidth="1px" Font-Size="Small">
                        <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green"
                            BorderWidth="1px" ForeColor="Black" />
                        <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                        <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                        <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px"
                            ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
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
</asp:Content>
