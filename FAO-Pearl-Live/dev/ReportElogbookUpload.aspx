<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false"
    EnableEventValidation="false" CodeFile="ReportElogbookUpload.aspx.vb" Inherits="ElogbookUpload" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="http://maps.google.com/maps/api/js?sensor=false" type="text/javascript"></script>
    <script type="text/javascript">

        function OpenWindow(url) {

            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }

    </script>
    <style type="text/css">
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
            border: 1px groove gray;
            padding: 3px;
            font-family: Verdana;
        }
    </style>
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
                        <td style="width: 200px">
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
                            &nbsp;<asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="True">
                                <asp:ListItem>2015</asp:ListItem>
                                <asp:ListItem>2014</asp:ListItem>
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
                        <td style="text-align: center;" colspan="5">
                            <asp:Label ID="lblRecord" runat="server" ForeColor="red" Font-Size="Small"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:GridView ID="grdTripData" EmptyDataText="Record does not exists." AllowPaging="True"
                                runat="server" DataKeyNames="tid" CellPadding="3" Width="100%" BorderStyle="None"
                                BorderColor="Green" BorderWidth="1px" Font-Size="Small" CaptionAlign="Top" AutoGenerateColumns="False">
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
                                    <asp:TemplateField HeaderText="Map" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('http://<%#  HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath  %>/Showmap1.aspx?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')">
                                                <img alt="Show On Map" src="images/earth_search.png" height="16px" width="16px" /></a>
                                        </ItemTemplate>
                                        <ItemStyle Width="60px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px"
                                                Width="16px" ToolTip="Edit trip" OnClick="EditHit" AlternateText="Edit" />
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px"
                                                Width="16px" ToolTip="Delete trip" OnClick="DeleteHit" AlternateText="Delete" />
                                        </ItemTemplate>
                                        <ItemStyle Width="60px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="User Name" HeaderText="User Name" />
                                    <asp:BoundField DataField="Employee Code" HeaderText="Employee Code" />
                                    <asp:BoundField DataField="Department" HeaderText="Department" />
                                    <asp:BoundField DataField="Loc Start Point" HeaderText="Location Start Point" />
                                    <asp:BoundField DataField="Loc End Point" HeaderText="Location End Point" />
                                    <asp:BoundField DataField="Meter Start" HeaderText="Meter Start reading" />
                                    <asp:BoundField DataField="Meter End" HeaderText="Meter End reading" />
                                    <asp:BoundField DataField="Kms" HeaderText="Total Kms." />
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date & Time" />
                                    <asp:BoundField DataField="EndDate" HeaderText="End Date & Time" />
                                    <asp:BoundField DataField="Total Hrs." HeaderText="Total Hrs." />
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
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupEdit"
        PopupControlID="pnlPopupEdit" CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="450px">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>
                            New / Update
                        </h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit" ImageUrl="images/close.png" runat="server" ToolTip="Close" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="Date1" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
                                    <asp:CalendarExtender ID="CalendarExtender4" TargetControlID="Date2" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
                                    <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                        <ProgressTemplate>
                                            <div id="Layer1" style="position: absolute; z-index: 1; left: 45%; top: 50%;">
                                                <asp:Image ID="Image2" runat="server" Height="40px" ImageUrl="~/Images/spinner4-black.gif" />
                                                <b style="color: #2C3539">&nbsp;Please wait...</b>
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td colspan="2" align="left" style="padding-left: 100px">
                                                HH &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; MM
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 124px">
                                                <label>
                                                    Start Date & Time*</label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="Date1" runat="server" Width="70" AutoPostBack="true"></asp:TextBox>
                                                &nbsp;
                                                <asp:DropDownList ID="ddlshh" runat="server" Width="60px">
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlsmm" runat="server" Width="60px">
                                                </asp:DropDownList>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 120px">
                                                <label>
                                                    END Date & Time*</label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="Date2" runat="server" Height="16px" Width="70"></asp:TextBox>
                                                &nbsp;
                                                <asp:DropDownList ID="ddlehh" runat="server" Width="60px">
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlemm" runat="server" Width="60px">
                                                </asp:DropDownList>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <asp:Label ID="Vehicle" runat="server" Font-Bold="true">Vehicle No </asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="VehicleNo" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <asp:Label ID="IMELabel" runat="server" Font-Bold="true"> IMEI NO</asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="IMEINO" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        </tr> </tr>
                                        <tr>
                                            <td colspan="3" align="right">
                                                <asp:Button ID="btnActUserSave" runat="server" CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Text="Save" Width="70px" />
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
    </asp:Panel>
    <asp:Button ID="btnShowPopupDelete" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupDelete"
        PopupControlID="pnlPopupDelete" CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>
                            Trip Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <br />
                                <h2>
                                    <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red" Width="97%"
                                        Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelete" runat="server" Text="Yes Delete" Width="90px" OnClick="DeleteRecord"
                                        CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
