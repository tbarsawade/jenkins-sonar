<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="TripReport, App_Web_cjg31vo3" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
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
                 <%-- <asp:PostBackTrigger ControlID ="btsSearch" />--%>
                <asp:PostBackTrigger ControlID="btnExcelExport" />
                            </Triggers>
            <ContentTemplate>
                <table cellpadding="5px" cellspacing="5px" style="width: 100%; border: solid; color: blue;
                    border-style: double; border-width: 1px">
                    <tr>
                        <td colspan="6" style="text-align: center">
                            <asp:Label ID="lblHeader" runat="server" Text="GPS Half Hourly Report" Style="font-weight: bold;
                                color: #800000;"></asp:Label>
                        </td>
                    </tr>
                    <tr>
          <td>
          <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"  ></asp:Label>
          </td>
          </tr>
                    <tr>
                        <td style="width: 150px" align ="center" >
                            <asp:Label ID="lblFMonth" runat="server" Text="Select Date:" Style="color: Maroon;
                                font-weight: bold"></asp:Label>
                        </td>
                                                
                        <td>
                           <asp:TextBox ID="txtsdate"   runat="server"></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="yyyy-MM-dd"   runat="server" />
                        </td>
                        <td>
                            <asp:Button ID="btsSearch" runat="server" Text="Search" CssClass="btnNew" />
                            &nbsp;
                            <asp:ImageButton ID="btnExcelExport" ToolTip="Export EXCEL" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
                                                   </td>
                    </tr>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                    <ProgressTemplate>
                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                            please wait...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                    <tr>
                        <td colspan="6">
                            <asp:GridView ID="GPSReport"  runat="server"     
             CellPadding="2" CssClass="GridView"  CaptionAlign="Left" 
                    ForeColor="#333333" Width="100%" 
               AllowSorting="false" AllowPaging="true"  >
                    <FooterStyle  CssClass="FooterStyle" />
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle CssClass="EmptyDataRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />

                </asp:GridView>
                        </td>
                    </tr>
                </table>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
