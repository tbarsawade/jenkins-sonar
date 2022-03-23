<%@ page title="" language="VB" masterpagefile="~/PublicMaster.master" autoeventwireup="false" enableeventvalidation="false" inherits="VDoorStatusReport, App_Web_zdxdm40d" viewStateEncryptionMode="Always" %>
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
        .FixedHeader {
            position: absolute;
            font-weight: bold;
        }     
        .myClass
        {
            border: solid 1px red;
        }
        .myClass input
        {
            background-color: green;
        }
        .myClass label
        {
            font-weight: bold;
        }
        
        .txtbox1
        {
            background-image: url(images/bg.png);
            border: 1px solid #6297BC;
        }
        .modalBackground
        {
            height: 100%;
            background-color: gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        .modalpopup
        {
            background-color: #EEEEEE;
            border: 1px solid gray;
            width: 250;
            padding: 3px;
            font-family: Verdana;
            border-style: groove;
        }
        .divWaiting
        {
            position: absolute;
            background-color: #FAFAFA;
            z-index: 2147483647 !important;
            opacity: 0.8;
            overflow: hidden;
            text-align: center;
            top: 0;
            left: 0;
            height: 100%;
            width: 100%;
            padding-top: 60%;
        }
        ::-webkit-scrollbar
        {
            width: 12px; /* for vertical scrollbars */
            height: 12px; /* for horizontal scrollbars */
        }
        
        ::-webkit-scrollbar-track
        {
            background: rgba(0, 0, 0, 0.1);
        }
        
        ::-webkit-scrollbar-thumb
        {
            background: rgba(0, 0, 0, 0.5);
        }
    </style>
   <div  style="margin-top: 10px; border:10px; border-color:Green; height:540px" >
      
        <asp:ImageButton ID="btnexportxl" ToolTip="Export EXCEL" ImageAlign="Right" runat="server"
            Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg" />
        &nbsp;
        <div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top: 10px">
         <table cellpadding="5px" cellspacing="5px" style="width: 100%; border: solid; color: blue;
                    border-style: double; border-width: 1px">
                    <tr>
                        <td colspan="6" style="text-align: center">
                            <asp:Label ID="lblHeader" runat="server" Text="Vehicle Door Status Report" Style="font-weight: bold;
                                color: #800000; font-size: small;"></asp:Label>
                        </td>
                    </tr>
                    </table>
                    <br />
            <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0"
                class="m9">
                <tr>
                    <td align="left" class="style2">
                        <table style="width: 900px">
                            <tr>
                                <td align="left" width="80%">
                                    <b><span style="color: #000000">ENTER IMEI NO :</span> </b>&nbsp;<asp:TextBox ID="txtIMEI" runat="server" Width="150PX"
                                        Height="18PX" BorderColor="#9999FF" BorderStyle="Outset"></asp:TextBox>&nbsp;
                                    <asp:Label ID="Label2" runat="server" Text="Start Date :" Style="color: #000000;
                                        font-weight: 700;"></asp:Label>
                                    &nbsp;<asp:TextBox ID="txtfrom" runat="server" Width="150PX" Height="18PX" BorderColor="#9999FF"
                                        BorderStyle="Outset"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd HH':'mm'"
                                        TargetControlID="txtfrom">
                                    </asp:CalendarExtender>
                                    &nbsp;<asp:Label ID="Label1" runat="server" Text="End Date :" Style="color: #000000;
                                        font-weight: 700;"></asp:Label>
                                    &nbsp;<asp:TextBox ID="txtTo" runat="server" Width="150PX" Height="18PX" BorderColor="#9999FF"
                                        BorderStyle="Outset"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy-MM-dd 23':'59'"
                                        TargetControlID="txtTo">
                                    </asp:CalendarExtender>
                                &nbsp;
                                <asp:Button ID="btnshow" runat="server" Text="Search" Font-Bold="True" 
                                        ForeColor="#336699" BorderColor="#669999" BorderStyle="Groove" Width="80" Height="30px" />

                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%">
                        <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
            <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0"
                class="m9">
                <tr>
                    <td>
                        <br />
                        <asp:Panel runat="server" ID="pngv">
                            <asp:GridView ID="GVReport1" runat="server" 
                                AutoGenerateColumns="True" BorderColor="Green" BorderStyle="none" BorderWidth="1px" 
                                EmptyDataText="Record does not exists." Font-Size="Small" PageSize="15"
                                ShowFooter="false" Width="100%">
                                <RowStyle BackColor="White" BorderColor="Green" BorderWidth="1px" CssClass="gridrowhome"
                                    ForeColor="Black" Height="25px" />
                                <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#d0d0d0" BorderColor="Green" BorderWidth="1px" CssClass="gridheaderhome"
                                    Font-Bold="True" ForeColor="black" Height="25px" HorizontalAlign="Center" />
                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                <SortedDescendingHeaderStyle BackColor="#93451F" />
                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
                               <%-- <Columns>
                                    <asp:BoundField DataField="IMEINO" HeaderText="IMEINO" />
                            <asp:BoundField DataField="FromDate" HeaderText="FromDate" />
                            <asp:BoundField DataField="ToDate" HeaderText="ToDate" />
                            <asp:BoundField DataField="Duration" 
                                HeaderText="Duration" />
                            <asp:BoundField DataField="Status" 
                                HeaderText="Status" />
                            </Columns>--%>
                           </asp:GridView>
                            <asp:GridView ID="GVReport2" runat="server"></asp:GridView>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            
        </div>
    </div>
   
</asp:Content>
