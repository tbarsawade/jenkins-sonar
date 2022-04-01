<%@ Page Language="VB" AutoEventWireup="false" CodeFile="LastSignal.aspx.vb" Inherits="LastSignal" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .tbx
        {
            background-image: url('images/form_bg.jpg');
            background-repeat: repeat-x;
            border: 1px solid #d1c7ac;
            color: #333333;
            padding: 3px;
            margin-right: 4px;
            margin-bottom: 8px;
            font-family: tahoma, arial, sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
    </asp:UpdatePanel>
    &nbsp;&nbsp;&nbsp;
    <div style="margin-top: 10%">
        <table width="100%">
        
            <tr>
                <td align="center">
                    <label>
                        Enter IMEI No.</label>
                    <asp:TextBox ID="txtImei" runat="server" Height="36px" Width="254px" CssClass="tbx"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="center">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnShow" runat="server" Text="Show" Width="100px" Height="40px" />
                </td>
            </tr>
            </table>
        <table width="60%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0"
            class="m9">
            <tr>
                <td>
                    <br />
                    <asp:UpdatePanel ID="upnl" runat="server">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pngv">
                                <asp:GridView ID="GVReport" runat="server" AllowSorting="False" AutoGenerateColumns="true"
                                    BorderColor="Green" BorderStyle="none" BorderWidth="1px" CellPadding="3" EmptyDataText="Record does not exists."
                                    Font-Size="Small" PageSize="1" ShowFooter="false" Width="100%">
                                    <RowStyle BackColor="White" BorderColor="Green" BorderWidth="1px" CssClass="gridrowhome"
                                        ForeColor="Black" Height="25px" />
                                    <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#d0d0d0" BorderColor="Green" BorderWidth="1px" CssClass="gridheaderhome"
                                        Font-Bold="True" ForeColor="black" Height="25px" HorizontalAlign="Center" />
                                 
                                    <Columns>
                                    </Columns>
                                   
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:GridView>
                                <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
