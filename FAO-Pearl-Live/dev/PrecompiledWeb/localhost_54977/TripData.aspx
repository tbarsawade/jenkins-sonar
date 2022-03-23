<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" enableeventvalidation="false" inherits="TripData, App_Web_bv10wntb" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">

        function OpenWindow(url) {

            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }

    </script>
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <ContentTemplate>
            <%-- <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" WatermarkText="HH:MM" TargetControlID="STime1" runat="server">
       </asp:TextBoxWatermarkExtender>
       <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2"  WatermarkText="HH:MM" TargetControlID="STime2" runat="server">
       </asp:TextBoxWatermarkExtender>--%>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%"
                border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="red" Width="97%"
                            Font-Size="Small"><h4>Trip Entry </h4></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;">
                        <asp:Label ID="lblRecord" runat="server" ForeColor="red" Font-Size="Small"></asp:Label>
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double green">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="width: 133px;">
                                    <asp:DropDownList ID="DropDownList1" runat="server" Width="130px">
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 300px">
                                    Trip Start &nbsp;
                                    <asp:TextBox ID="SDate1" Width="180px" onkeypress="return false;" runat="server"></asp:TextBox>
                                    <%--<asp:Image ID="clr" runat="server" ImageUrl="~/Images/cal.png"  />--%>
                                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="SDate1" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
                                    <%--&nbsp; <asp:TextBox ID="STime1" Width="50px" Text="00:00" runat="server"></asp:TextBox> --%>
                                    &nbsp; &nbsp; To
                                </td>
                                <td style="width: 250px">
                                    &nbsp; End Trip&nbsp;
                                    <asp:TextBox ID="SDate2" runat="server" onkeypress="return false;" Width="180px"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="SDate2" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
                                    <%-- &nbsp; <asp:TextBox ID="STime2" Width="50px" Text="23:59"  runat="server" />--%>
                                </td>
                                <td>
                                    <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/search.png"
                                        ToolTip="Search Location " OnClick="Search" />
                                </td>
                                <td style="text-align: right;">
                                    <asp:ImageButton ID="ImageButton3" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/lockunlock.png"
                                        ToolTip="Lock Unlock Trip" OnClick="LockUnlock" />
                                    &nbsp;
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg"
                                        ToolTip="Create Trip Entry" OnClick="popupshow" />
                                    &nbsp;
                                    <asp:ImageButton ID="btnnil" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/nil.png"
                                        ToolTip="Nil Trip Entry" OnClick="NilShow" />
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
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td style="text-align: left;" valign="top">
                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" CellPadding="2"
                            DataKeyNames="Tid" PageSize="20" CssClass="GridView" AllowSorting="True" AllowPaging="True">
                            <FooterStyle CssClass="FooterStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <EditRowStyle CssClass="EditRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass=" HeaderStyle" />
                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                            <Columns>
                                <asp:TemplateField HeaderText="S.No">
                                    <ItemTemplate>
                                        <%# CType(Container, GridViewRow).RowIndex + 1%>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="UserName" HeaderText="User Name" />
                                <asp:BoundField DataField="vehicle_no" HeaderText="Vehicle No ">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <%--<asp:BoundField DataField="IMEI_NO"  HeaderText="IMEI NO" />--%>
                                <asp:BoundField DataField="Trip_Start_DateTime" HeaderText="Trip Start DateTime"
                                    DataFormatString="{0:yyyy-mm-dd HH:MM}" />
                                <asp:BoundField DataField="Trip_end_DateTime" HeaderText="Trip End DateTime" DataFormatString="{0:yyyy-mm-dd HH:MM}" />
                                <asp:BoundField DataField="Trip_Start_Location" HeaderText="Trip Start Location">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Trip_End_Location" HeaderText="Trip End Location " />
                                <asp:BoundField DataField="Total_Distance" HeaderText="Total Distance (Km)" />
                                <asp:BoundField DataField="Triptype" HeaderText="Trip Type" />
                                <asp:BoundField DataField="Islock" HeaderText="Unlock Lock" />
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                                <asp:TemplateField HeaderText="check">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkHeader" runat="server" OnCheckedChanged="checkchange" AutoPostBack="true" />&nbsp;
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="check" runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle CssClass="" HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action" HeaderStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnapprove" runat="server" ImageUrl="~/images/av.png" Height="16px"
                                            Width="16px" ToolTip="approve trip" OnClick="approve" AlternateText="Approve" />
                                        <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('http://<%#  HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath  %>/Showmap.aspx?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')"
                                            title="Show On Map">
                                            <img alt="Show On Map" src="images/map.jpg" height="16px" width="16px" /></a>
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px"
                                            Width="16px" ToolTip="Edit trip" OnClick="EditHit" AlternateText="Edit" />
                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px"
                                            Width="16px" ToolTip="Delete trip" OnClick="DeleteHit" AlternateText="Delete" />
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupEdit"
        PopupControlID="pnlPopupEdit" CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="450px" Style="" BackColor="aqua">
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
                                    <%-- <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" WatermarkText="HH:MM" TargetControlID="Time1" runat="server">
       </asp:TextBoxWatermarkExtender>
       <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender4"  WatermarkText="HH:MM" TargetControlID="Time2" runat="server">
       </asp:TextBoxWatermarkExtender>--%>
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
                                            <asp:Panel ID="pnlmb" runat="server">
                                                <td align="left" style="width: 124px">
                                                    <label>
                                                        Entry Option</label>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="Triptype" runat="server" OnSelectedIndexChanged="typechange"
                                                        AutoPostBack="true">
                                                        <asp:ListItem Value="0">Automatic</asp:ListItem>
                                                        <%--<asp:ListItem Value="1">Manual</asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </td>
                                            </asp:Panel>
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
                                                <asp:TextBox ID="Date1" runat="server" Width="70" OnTextChanged="Add" AutoPostBack="true" ></asp:TextBox>
                                                &nbsp;
                                                <asp:DropDownList ID="ddlshh" runat="server" Width="60px">
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlsmm" runat="server" Width="60px">
                                                </asp:DropDownList>
                                                &nbsp;
                                                <%--&nbsp;YYYY-MM-DD(2013-08-15) &nbsp;24 Hour(23:59)--%>
                                                <asp:ImageButton ID="imgbtnloc" runat="server" ImageUrl="~/Images/Location.png" ToolTip="See Start Location" />
                                                <%-- <asp:Button ID="Check1" runat="server" Text="Check" ToolTip="Check Location" />--%>
                                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
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
                                                <asp:ImageButton ID="imgbtnloc1" runat="server" ImageUrl="~/Images/Location.png"
                                                    ToolTip="See End Location" />
                                                <%-- <asp:Button ID="Check2" runat="server" Text="Check" ToolTip="Check Location" />--%>
                                                <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
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
                                        </tr>
                                        <%-- <tr>
           <td align="left" style="width:110px">
            <asp:Label runat="server" ID="VehicalNoforManual" Visible="false" Font-Bold="true">Vehicle No*</asp:Label>   </td>
           <td colspan="3" align="left">
               <asp:TextBox ID="VehicalManual" runat="server"  Visible="false"></asp:TextBox> 
           </td>
       </tr>--%>
                                        <%--  <asp:Label ID="rr11" runat="server"></asp:Label>--%>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <asp:Label ID="StartLocation" runat="server" Visible="false" Font-Bold="true">Start Location*</asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="Sloc" runat="server" Visible="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <asp:Label ID="EndLocation" runat="server" Visible="false" Font-Bold="true">End Location*</asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="Eloc" runat="server" Visible="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <asp:Label ID="Tdistance" runat="server" Visible="false" Font-Bold="true">Total Distance*</asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="todistance" runat="server" Visible="false"></asp:TextBox>
                                                <asp:Label ID="km" runat="server" Visible="false">Kilometer</asp:Label>
                                            </td>
                                        </tr>
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
    <asp:Button ID="btnShowPopupLockUnlock" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="lockunlock_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupLockUnlock"
        PopupControlID="pnlPopuplockunlock" CancelControlID="btnCloseLockUnlock" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopuplockunlock" runat="server" Width="500px" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>
                            Lock /Unlock: Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseLockUnlock" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanellockunlock" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsglockUnlock" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="97%" Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActlockunlock" runat="server" Text="" Width="90px" OnClick="islock"
                                        CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnShowPopupDelete" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupDelete"
        PopupControlID="pnlPopupDelete" CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" Style="display: none"
        BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>
                            Formula Delete : Confirmation</h3>
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
    <asp:Button ID="btntrargetapprove" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnapprove_modelpopup" TargetControlID="btntrargetapprove"
        runat="server" PopupControlID="pnlapprove" CancelControlID="ImageButton1" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlapprove" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>
                            Formula Approved : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton1" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <br />
                                <h2>
                                    <asp:Label ID="lblMsgapprove" runat="server" Font-Bold="True" ForeColor="Red" Width="97%"
                                        Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnapprove" runat="server" Text="Approved" Width="90px" OnClick="Approvetrip"
                                        CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="Button2" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnniltripmodelpopup" runat="server" TargetControlID="Button2"
        PopupControlID="pnlniltrip" CancelControlID="ImageButton2" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlniltrip" runat="server" Width="450px" Style="" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>
                            Nil Trip New / Update
                        </h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton2" ImageUrl="images/close.png" runat="server" ToolTip="Close" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="upniltrip" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:CalendarExtender ID="CalendarExtender5" TargetControlID="nilstartdate" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
                                    <asp:CalendarExtender ID="CalendarExtender6" TargetControlID="nilenddate" Format="yyyy-MM-dd"
                                        runat="server">
                                    </asp:CalendarExtender>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="3" align="left">
                                                <asp:Label ID="lblniltrip" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 124px">
                                                <label>
                                                    Start Date*</label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="nilstartdate" runat="server" Width="118px" OnTextChanged="NilTrip" AutoPostBack="true" ></asp:TextBox>00:00
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 120px">
                                                <label>
                                                    END Date*</label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="nilenddate" runat="server" Height="16px" Width="117px"></asp:TextBox>23:59
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <asp:Label ID="Label6" runat="server" Font-Bold="true">Vehicle No </asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="niltripVehicle" Enabled="false" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 110px" align="left">
                                                <asp:Label ID="Label7" runat="server" Font-Bold="true"> IMEI NO</asp:Label>
                                            </td>
                                            <td colspan="2" align="left">
                                                <asp:TextBox ID="niltripIMEI" runat="server" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        </tr>
                                        <tr>
                                            <td colspan="3" align="right">
                                                <asp:Button ID="btbniltripsave" runat="server" CssClass="btnNew" Font-Bold="True"
                                                    OnClick="niltripsave" Font-Size="X-Small" Text="Save" Width="70px" />
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
</asp:Content>
