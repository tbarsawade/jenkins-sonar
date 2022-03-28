<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ReportScheduler, App_Web_o3dtvhns" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
            background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
            background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlStype" EventName="SelectedIndexChanged" />
        </Triggers>
        <ContentTemplate>
            <div style="border: solid; color: orange; border-style: double; border-width: 1px;
                height: 20px">
                <asp:ImageButton ID="btnReportSch" ToolTip=" Gps Report Schedular" runat="server"
                    Width="18px" Height="18px" ImageAlign="Right" ImageUrl="~/Images/plus.jpg" />
                <asp:Label ID="lblForm" runat="server" ForeColor="Red" align="center" />
            </div>
            <br />
            <br />
            <asp:GridView ID="gvpending" runat="server" AutoGenerateColumns="False" CellPadding="2"
                DataKeyNames="tid" ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="True">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#EFF3FB" />
                <EditRowStyle BackColor="#2461BF" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                        </ItemTemplate>
                        <ItemStyle Width="50px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="reportname" HeaderText="Report Name">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="reportsubject" HeaderText="Send Subject">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="reporttype" HeaderText="Report Type">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="date" HeaderText="Date">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="HH" HeaderText="Hour">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="MM" HeaderText="Minute">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="msgbody" HeaderText="HtmlBody">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ordering" HeaderText="Ordering">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px"
                                Width="16px" ToolTip="Edit record" OnClick="EditHit" AlternateText="Edit" />
                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px"
                                Width="16px" ToolTip="Delete record" OnClick="DeleteHit" AlternateText="Delete" />
                        </ItemTemplate>
                        <ItemStyle Width="60px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <%--Report Schedular from here--%>
            <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="btnSc_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm"
                TargetControlID="btnShowPopupForm" CancelControlID="btnCloseForm" BackgroundCssClass="modalBackground"
                DropShadow="true">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlPopupForm" runat="server" Width="100%" Height="600px" BackColor="White"
                ScrollBars="Both">
                <div class="box" style="width: 800px">
                    <table cellspacing="0px" cellpadding="0px" width="800px">
                        <tr>
                            <td width="760px">
                                <h3>
                                    Report Scheduler</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <div id="main" style="width: 800px">
                        <div class="form" style="text-align: left">
                            <table cellspacing="5px" cellpadding="0px" width="800px" border="0px">
                                <tr>
                                    <td style="width: 100%" colspan="2">
                                        <h2 style="text-align: center">
                                            &nbsp;
                                        </h2>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 100%" colspan="2">
                                        <h2 style="text-align: center">
                                            Enter basic Report scheduler information</h2>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="middle" style="text-align: right">
                                        <label>
                                            Report Name :
                                        </label>
                                    </td>
                                    <td style="">
                                        <asp:TextBox ID="txtRptname" runat="server" CssClass="txtBox" Width="380px"></asp:TextBox>
                                        <asp:Label ID="lblRptName" runat="server" ForeColor="Red" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="middle" style="text-align: right">
                                        <label>
                                            Report Subject :
                                        </label>
                                    </td>
                                    <td style="">
                                        <asp:TextBox ID="txtSendSub" runat="server" CssClass="txtBox" Width="380px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right">
                                        <label>
                                            Report Type :
                                        </label>
                                    </td>
                                    <td style="">
                                        <asp:DropDownList ID="ddlStype" runat="server" CssClass="txtBox" Width="196px">
                                            <asp:ListItem>Daily</asp:ListItem>
                                            <asp:ListItem>Weekly</asp:ListItem>
                                            <asp:ListItem>Monthly</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtDay" runat="server" CssClass="txtBox" Width="167px"></asp:TextBox>
                                        <ajaxToolkit:BalloonPopupExtender ID="BalloonPopupExtender1" TargetControlID="txtDay"
                                            UseShadow="true" DisplayOnFocus="true" Position="bottomright" BalloonPopupControlID="Panel3"
                                            BalloonStyle="Rectangle" runat="server" BalloonSize="medium" />
                                        <asp:Panel ID="Panel3" runat="server">
                                            For daily remain blank.<br />
                                            OR
                                            <br />
                                            For weekly enter day number(ex: Monday=1)<br />
                                            OR
                                            <br />
                                            For Monthly enter date(ex: 1 to 31)
                                        </asp:Panel>
                                        <asp:Label ID="lblweek" runat="server" ForeColor="Red" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right">
                                        <label>
                                            Scheduler Time :
                                        </label>
                                    </td>
                                    <td style="">
                                        <asp:DropDownList ID="ddlHour" runat="server" CssClass="txtBox" Width="196px">
                                            <asp:ListItem>00</asp:ListItem>
                                            <asp:ListItem>01</asp:ListItem>
                                            <asp:ListItem>02</asp:ListItem>
                                            <asp:ListItem>03</asp:ListItem>
                                            <asp:ListItem>04</asp:ListItem>
                                            <asp:ListItem>05</asp:ListItem>
                                            <asp:ListItem>06</asp:ListItem>
                                            <asp:ListItem>07</asp:ListItem>
                                            <asp:ListItem>08</asp:ListItem>
                                            <asp:ListItem>09</asp:ListItem>
                                            <asp:ListItem>10</asp:ListItem>
                                            <asp:ListItem>11</asp:ListItem>
                                            <asp:ListItem>12</asp:ListItem>
                                            <asp:ListItem>13</asp:ListItem>
                                            <asp:ListItem>14</asp:ListItem>
                                            <asp:ListItem>15</asp:ListItem>
                                            <asp:ListItem>16</asp:ListItem>
                                            <asp:ListItem>17</asp:ListItem>
                                            <asp:ListItem>18</asp:ListItem>
                                            <asp:ListItem>19</asp:ListItem>
                                            <asp:ListItem>20</asp:ListItem>
                                            <asp:ListItem>21</asp:ListItem>
                                            <asp:ListItem>22</asp:ListItem>
                                            <asp:ListItem>23</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtminut" runat="server" CssClass="txtBox" Width="167px"></asp:TextBox>
                                        <ajaxToolkit:BalloonPopupExtender ID="BalloonPopupExtender3" runat="server" BalloonPopupControlID="Panel4"
                                            BalloonSize="small" BalloonStyle="Rectangle" DisplayOnFocus="true" Position="bottomright"
                                            TargetControlID="txtminut" UseShadow="true" />
                                        <asp:Panel ID="Panel4" runat="server">
                                            Enter Minute for scheduling
                                            <br />
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="middle" style="text-align: right">
                                        <label>
                                            Report Order :
                                        </label>
                                    </td>
                                    <td style="">
                                        <asp:TextBox ID="txtOrder" runat="server" CssClass="txtBox" Width="196px" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right">
                                        <label>
                                            Alert Type :
                                        </label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlAlertType" runat="server" CssClass="txtBox" 
                                            Width="196px" AutoPostBack="True">
                                            <asp:ListItem>Mail Alert</asp:ListItem>
                                            <asp:ListItem>Gps Mail Alert</asp:ListItem>
                                            <asp:ListItem>Gps Sms Alert</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                 <asp:Panel runat="server" ID="pnlsms">
                                <tr>
                                    <td style="text-align: right">
                                        <label>
                                            Geofence Document Type :
                                        </label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlGeoDocType" runat="server" CssClass="txtBox" Width="196px"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                        <label>
                                            Mobile Field :
                                        </label>
                                        <asp:DropDownList ID="ddlMobileFields" runat="server" CssClass="txtBox" Width="196px">
                                        </asp:DropDownList>
                                        <%--  <asp:TextBox ID="txtMobile" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>--%>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right">
                                        <label>
                                            SMS Dispay Type :
                                        </label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSmsField" runat="server" CssClass="txtBox" Width="196px">
                                        </asp:DropDownList>
                                        <label>
                                            IMIE No.&nbsp;&nbsp;&nbsp;&nbsp; :&nbsp;
                                        </label>
                                        <asp:DropDownList ID="ddlIMEINo" runat="server" CssClass="txtBox" Width="196px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                </asp:Panel>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:Panel ID="pnlScrol" runat="server" Height="250px">
                                            <asp:TextBox ID="txtBody" runat="server" Height="200px" Style="text-align: left"
                                                TextMode="MultiLine" Width="90%">
                                            </asp:TextBox>
                                            <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" EnableSanitization="false"
                                                TargetControlID="txtBody">
                                            </asp:HtmlEditorExtender>
                                        </asp:Panel>
                                        <tr>
                                            <td align="right" colspan="2" style="margin-left: 40px">
                                                <asp:Button ID="btnSave" runat="server" CssClass="btnNew" Width="100Px" />
                                            </td>
                                        </tr>
                                    </td>
                                </tr>
                            </table>
                            </td> </tr> </td> </tr> </table>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <%--Report Schedular to here--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--Delete confirmation start to here--%>
    <asp:Button ID="btnShowPopupDelete" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupDelete"
        PopupControlID="pnlPopupDelete" CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" BackColor="Aqua" Style="display: none">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>
                            Report Scheduler Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
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
    <%--Delete confirmation end to here--%>
</asp:Content>
