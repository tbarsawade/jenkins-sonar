<%@ page language="VB" autoeventwireup="false" inherits="VehicleDetails, App_Web_nfrpb0kv" viewStateEncryptionMode="Always" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vehicle ElogBook Report</title>
  </head>
<body>
    <form id="form1" runat="server">
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
    </style>
    &nbsp;<div class="gradientBoxesWithOuterShadows" style="margin-top: 10px">
        <table align="center">
            &nbsp;&nbsp;
            <tr>
                <td style="text-align: center; width: 300px;">
                    <asp:Label ID="lblDocType" runat="server" Font-Bold="True" Text="Vehicle Log Book Report"
                        Style="color: #660033;"></asp:Label>
                </td>
                &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
            </tr>
        </table>
        <br />
        <div align="Right">
            <asp:Label ID="lblmsg" runat="server" ForeColor="Red" Style="color: #FF3300"></asp:Label>
            <asp:ImageButton ID="btnExcelExport" ToolTip="Export EXCEL" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
        </div>
        <div align="center">
            <asp:GridView ID="gvData" AllowSorting="False" ShowFooter="false" runat="server"
                AutoGenerateColumns="true" CellPadding="3" Width="100%" PageSize="15" BorderStyle="none"
                BorderColor="Green" BorderWidth="1px" Font-Size="Small">
                <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green"
                    BorderWidth="1px" ForeColor="Black" HorizontalAlign="Center"  />
                <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px"
                    ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
