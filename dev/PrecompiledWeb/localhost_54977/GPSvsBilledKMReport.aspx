<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="GPSvsBilledKMReport, App_Web_4ysr50e5" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
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
                            <asp:Label ID="lblHeader" runat="server" Text="GPS vs Billed KM Report" Style="font-weight: bold;
                                color:#FF0000;"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">
                            <asp:Label ID="lblFMonth" runat="server" Text="Log for the month :" Style="color: Maroon;
                                font-weight: bold"></asp:Label>
                        </td>
                        <td style="width: 170px">
                            <asp:DropDownList ID="ddlMonth" runat="server">
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
                            &nbsp;<asp:DropDownList ID="ddlYear" runat="server">
                               
                            </asp:DropDownList>
                        </td>
                        <td style="width: 170px" align="center">
                            <asp:Label ID="lblCircle" runat="server" Text="Circle :" Style="color: Maroon;
                                font-weight: bold"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" 
                      style=" display:block ">
                      
       
                      <asp:CheckBoxList ID="CheckListCircle"  DataValueField="tid"   runat="server">
               
                      </asp:CheckBoxList>
                  </asp:Panel>
	
                        </td>
                        <panel>
                        <td align="center" ><asp:Button ID="btsSearch" runat="server" Text="Search" CssClass="btnNew"  />
                            &nbsp;
                        <asp:ImageButton ID="btnExcelExport" ToolTip="Export EXCEL" runat="server" Width="18px"  
                                Height="18px"  ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
                            <asp:ImageButton ID="btnPdfExport" ToolTip="Export PDF" runat="server"  Width="18px" 
                                Height="18px" ImageUrl="~/images/export.png" />
                        </td>
                            </panel>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:GridView ID="grdgpsbilleddata" EmptyDataText="Record does not exists." 
                               AllowPaging="True" runat="server" 
                                CellPadding="3" Width="100%" BorderStyle="None" BorderColor="Green"
                                BorderWidth="1px" Font-Size="Small" CaptionAlign="Top" UseAccessibleHeader="False">
                                <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green"
                                    BorderWidth="1px" ForeColor="Black" />
                                <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px"
                                    ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Left" />
                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                <Columns>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                     <td colspan="5">
                            <asp:Label ID="lblNotAuthorised" runat="server" Text="You are not Authorised to Access any Data" Style="color: Maroon;
                                font-weight: bold" Visible="false"></asp:Label>
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
