<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="TripReport, App_Web_sifhu5tb" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<script src="http://maps.google.com/maps/api/js?sensor=false" 
          type="text/javascript"></script>
   <script type="text/javascript">

       function OpenWindow(url) {

           var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
           //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
           return false;
       }

    </script>
    <div>
        <asp:UpdatePanel ID="updPnlSearch" runat="server">
            <Triggers>
                <%--  <asp:PostBackTrigger ControlID ="btnCsvexport" />--%>
                <asp:PostBackTrigger ControlID="btnExcelExport" />
                <asp:PostBackTrigger ControlID="btnPdfExport" />
            </Triggers>
            <ContentTemplate>
                <table cellpadding="5px" cellspacing="5px" style="width: 100%; border: solid; color: blue;
                    border-style: double; border-width: 1px">
                    <tr>
                        <td colspan="6" style="text-align: center">
                            <asp:Label ID="lblHeader" runat="server" Text="VEHICLE LOG BOOK (Electronic)" Style="font-weight: bold;
                                color: #800000;"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">
                            <asp:Label ID="lblFMonth" runat="server" Text="Log for the month :" Style="color: Maroon;
                                font-weight: bold"></asp:Label>
                        </td>
                        <td style="width: 260px">
                            <asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="01">January</asp:ListItem>
                                <asp:ListItem Value="02">February</asp:ListItem>
                                <asp:ListItem Value="03">March</asp:ListItem>
                                <asp:ListItem Value="04">April</asp:ListItem>
                                <asp:ListItem Value="05">May</asp:ListItem>
                                <asp:ListItem Value="06">June</asp:ListItem>
                                <asp:ListItem Value="07">July</asp:ListItem>
                                <asp:ListItem Value="08">August</asp:ListItem>
                                <asp:ListItem Value="09">September</asp:ListItem>
                                <asp:ListItem Value="10">October</asp:ListItem>
                                <asp:ListItem Value="11">November</asp:ListItem>
                                <asp:ListItem Value="12">December</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;<asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="True" Width="120px">
<%--                                  <asp:ListItem>--Select Year--</asp:ListItem>--%>
                                  <asp:ListItem>2015</asp:ListItem>
                                  <asp:ListItem>2014</asp:ListItem>
                               <%-- <asp:ListItem>2013</asp:ListItem>--%>
                               </asp:DropDownList>
                        </td>
                        <td style="width: 170px">
                            <asp:Label ID="lblVehicleRegNo" runat="server" Text="Vehicle Registration No." Style="color: Maroon;
                                font-weight: bold"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlVehicleRegNo" runat="server" CssClass="txtBox" Width="170px" />
                        </td>
                        <td>
                            <asp:Button ID="btsSearch" runat="server" Text="Search" CssClass="btnNew" />
                            &nbsp;
                            <asp:ImageButton ID="btnExcelExport" ToolTip="Export EXCEL" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
                            <asp:ImageButton ID="btnPdfExport" ToolTip="Export PDF" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/images/export.png" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:GridView ID="grdTripData" EmptyDataText="Record does not exists." 
                                AllowSorting="False" AllowPaging="True" runat="server" DataKeyNames="tid" 
                                CellPadding="3" Width="100%" BorderStyle="None" BorderColor="Green"
                                BorderWidth="1px" Font-Size="Small" CaptionAlign="Top">
                                <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green"
                                    BorderWidth="1px" ForeColor="Black" />
                                <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px"
                                    ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                <SortedDescendingHeaderStyle BackColor="#93451F" />
                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                <Columns>
                                 <asp:TemplateField HeaderText="Map" ItemStyle-HorizontalAlign="Right"  >
                            <ItemTemplate>
                                  <%-- <a class="detail" style="text-decoration:none;"  href="#" onclick="OpenWindow('http://<%#  HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath  %>/Showmap.aspx?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')"   >
                 <img alt="Show On Map" src="images/earth_search.png"  height="16px" width="16px"/></a>--%>
                              
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="center"/>
                        </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
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
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
