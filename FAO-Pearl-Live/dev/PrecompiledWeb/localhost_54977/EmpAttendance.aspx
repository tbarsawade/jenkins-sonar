<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="EmpAttendance, App_Web_s1ukpvof" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">

        function OpenWindow(url) {

            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }
        $(document).ready(function () {
          
        });

        function ShowDialog(url) {
            // do some thing with currObj data

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
            <asp:PostBackTrigger ControlID="btnexportxl" />
            <asp:PostBackTrigger ControlID="btnExport" />
        </Triggers>
        <ContentTemplate>
            <div class="gradientBoxesWithOuterShadows" style="margin-left: 10px">
                <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0"
                    class="m9">
                    <tr>
                        <td colspan="4" align="right">
                            <asp:ImageButton ID="btnexportxl" runat="server" Height="18px" ImageUrl="~/Images/excelexpo.jpg"
                                ToolTip="Export EXCEL" Width="18px" />
                            <asp:ImageButton ID="btnExport" runat="server" Height="18px" ImageUrl="~/images/export.png"
                                ToolTip="Export PDF" Width="18px" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="m8b" width="25%">
                            <fieldset>
                                <legend style="color: Black; text-align: center; font-weight: bold;">Report Type</legend>
                                <br />
                                <br />
                                <asp:Panel ID="Panel2" runat="server" Height="100px" Style="display: block">
                                    <asp:DropDownList ID="ddlRtype" CssClass="txtBox" runat="server" Width="160px" AutoPostBack="True">
                                        <asp:ListItem Selected="True">Select Report</asp:ListItem>
                                       <%-- <asp:ListItem>Details</asp:ListItem>--%>
                                        <asp:ListItem>Attendance</asp:ListItem>
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="left" class="style2">
                            <table>
                                <tr>
                                    <td>
                                        <fieldset style="width: 326px; height: 150px;">
                                            <legend style="color: Black; text-align: center; font-weight: bold;">Date Range</legend>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <br />
                                                        <br />
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
                                                        <asp:Label ID="lblType" runat="server" Font-Bold="True" Text="Part: "></asp:Label>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlPeriod" CssClass="txtBox" runat="server"
                                                            Width="110px">
                                                            <asp:ListItem>First</asp:ListItem>
                                                              <asp:ListItem>Second</asp:ListItem>
                                                               
                                                        
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="Left" width="35%">
                            <fieldset style="height: 140px">
                                <legend style="color: Black; text-align: center; font-weight: bold;">Job Code</legend>
                                <asp:Panel ID="PnlLoc" runat="server" Height="150px" ScrollBars="Auto" Style="display: block">
                                    <br />
                                    <br />
                                    &nbsp;
                                    <asp:DropDownList ID="ddlLocation" CssClass="txtBox" runat="server" Width="200px">
                                    </asp:DropDownList>
                                    <asp:CheckBoxList ID="chkLoc" runat="server">
                                    </asp:CheckBoxList>
                                </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="center" class="m8b" width="5%">
                            <asp:Button ID="btnSearch" runat="server" CssClass="btnNew" Height="25px" Text="Search"
                                ToolTip="Search" Width="70px" />
                        </td>
                    </tr>
                </table>
                <br />
                
                <br />
                <asp:Panel runat="server" ID="pnlgrd" ScrollBars="Auto">
                    <div align="center" style="width: 1000px">
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
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
