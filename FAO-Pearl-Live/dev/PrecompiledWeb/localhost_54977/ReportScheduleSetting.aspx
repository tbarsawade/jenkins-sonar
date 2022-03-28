<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" validaterequest="false" autoeventwireup="false" inherits="ReportScheduleSetting, App_Web_c5kjwoe4" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <%-- <script src="Jquery/jquery.sumoselect.min.js"></script>
     <link href="css/sumoselect.css" rel="stylesheet">--%>

   <%-- <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.2/jquery.min.js"></script>--%>
    <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
     <script type="text/javascript">
         $(document).ready(function () {
             //$(<%=lstdept.ClientID%>).SumoSelect({ selectAll: true });
         });
        
         function GetIDS() {
             var str = '';
             var CHK = document.getElementById('<%= lstdept.ClientID%>');
            if (CHK != null) {
                var checkbox = CHK.getElementsByTagName("input");
                for (var i = 0; i < CHK.length; i++) {
                    if (CHK[i].selected) {
                        str += CHK[i].value + ',';
                    }
                }
            }
             // var hdn = document.getElementById("hdndata");
             //hdn.value = str;
            return str;
         }
         function SaveUpdateScheduler() {
             $("#ddlVehicleType").html("");
             //$("#ddlVehicleType").append($("<option></option>").val(0).html('--Select Vehicle Type--'));
             var role = GetIDS();
             var str='{ "Role": "' + role + '"}'
             $.ajax({
                 type: "POST",
                 url: "ReportScheduleSetting.aspx/SaveUpdateSchedule",
                 data: str,
                 dataType: "json",
                 contentType: "application/json",
                 success: function (response) {
                     //var res = jQuery.parseJSON(response.d.Data);
                     //$.each(res, function (data, value) {
                     //  $("#ddlVehicleType").append($("<option></option>").val(value.TID).html(value.VEHICLETYPE));
                    
                 }
             })
         }
    </script>
     
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
            <asp:PostBackTrigger ControlID="btnNew" />
        </Triggers>--%>
         <ContentTemplate>
            <div class="container-fluid">
                <div class="form">

                    <div class="doc_header">
                        Report Scheduler
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12" style="text-align: center;">
                            <asp:Label ID="lblMsg1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-1 col-sm-1">
                            <label>Field Name</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox">
                                <asp:ListItem Text="Select One" Selected="True" Value="0"></asp:ListItem>
                                <asp:ListItem Text="ReportName" Value="1"></asp:ListItem>
                                <asp:ListItem Text="ReportSubject" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Value</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox"></asp:TextBox>
                        </div>
                        <%--<div>
                             <asp:listbox runat="server" id="lstdept" selectionmode="Multiple" Width="182px" >
                                                 </asp:listbox>
                        </div>--%>
                        <div class="col-md-1 col-sm-1">
                            <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                ImageUrl="~/Images/search.png" />
                        </div>
                        
                        <div class="col-md-4 col-sm-4">
                            &nbsp;
                        </div>
                        <div class="col-md-1 col-sm-1" style="float: right;">
                            <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" OnClick="Add" />
                        </div>
                                               
                        
                    </div>
                    <br />
                    <div class="col-md-1 col-sm-1" style="float: right;">
                             <asp:Button ID="btnmail" CssClass="btnNew" runat="server" Text="Mail Schedule" />
                        </div>
                     <br />
                      <br />
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <asp:Panel ID="pnlgvData" runat="server" Width="100%" Height="400px" ScrollBars="Both">
                                <asp:GridView ID="gvData" runat="server" Width="100%" AutoGenerateColumns="False" DataKeyNames="tid">
                                    <HeaderStyle CssClass="GridviewScrollHeader" />
                                    <RowStyle CssClass="GridviewScrollItem" />
                                    <PagerStyle CssClass="GridviewScrollPager" />
                                     <Columns>
                                         <asp:TemplateField HeaderText="S.No">
                                            <ItemTemplate>
                                                <%# CType(Container, GridViewRow).RowIndex + 1%>
                                            </ItemTemplate>
                                             </asp:TemplateField>
                                        <asp:BoundField DataField="ReportName" HeaderText="Report Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ReportSubject" HeaderText="Report Subject">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ReportType" HeaderText="Report Type">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Sendtype" HeaderText="Send Type">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Action">
                                             <HeaderStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                               <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit"   AlternateText="Edit" ToolTip="Edit" />
                                                &nbsp;
                                              <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="deleteHit" AlternateText="Delete" ToolTip="Delete"  />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>

                        </div>
                    </div>
                </div>
            </div>
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
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupEdit" runat="server" Width="920px" ScrollBars="Auto" Style="display: block">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr style="background: #1f6b07;">
                    <td style="width: 900px">
                        <h3>New / Update Report Scheduler</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit"
                            CssClass="PopUpCloseBtn" runat="server" AlternateText="." /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        

                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                                
                                <div class="form">

                                    <table cellspacing="2px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="5">
                                                <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" Font-Size="X-Small"
                                                    ForeColor="Red" Width="98%"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" width="120px">Report Name</td>
                                            <td align="left" width="200px">
                                               <asp:DropDownList ID="ddlreportname" runat="server" Width="182px" CssClass="txtBox" AutoPostBack="true">
                                                  </asp:DropDownList>
                                            </td>

                                            <td align="left"> Mail Type  </td>
                                            <td align="left" width="200px">
                                                <asp:DropDownList ID="ddlsendtype" runat="server" Width="182px" CssClass="txtBox">
                                                    <asp:ListItem Text="Select One" Selected="True" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Excel Sheet" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="MailBody" Value="2"></asp:ListItem>
                                                    </asp:DropDownList>
                                            </td>

                                            <%--<td width="250px" align="right" rowspan="5" valign="top">
                                                <span>Variables:(Copy  & Paste to Mail Body)</span><br />
                                                <asp:TextBox ID="txtvar" runat="server" TextMode="MultiLine" Width="250px"></asp:TextBox>
                                            </td>--%>
                                        </tr>

                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="lbldt" runat="server" Text="Role:"></asp:Label><asp:Label ID="Label3" runat="server" Style="display: none;" Text="Alert Type" /></td>
                                            <td align="left">
                                               
                                                <asp:listbox runat="server" id="lstdept" selectionmode="Multiple" Width="182px"  AutoPostBack="true">
                                                 </asp:listbox>
                                               
                                             </td>
                                            <td align="left">Mail To:</td>
                                            <td align="left">
                                               <asp:TextBox ID="txtmailto" runat="server" Width="182Px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                         </tr>

                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="lblcc" Text="CC List" runat="server"></asp:Label>
                                            </td>
                                            <td align="left">
                                               <asp:TextBox ID="txtcc" runat="server" Width="182Px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblbcc" Text="BCC List" runat="server"></asp:Label>
                                            </td>
                                            <td align="left">
                                               <asp:TextBox ID="txtbcc" runat="server" Width="182Px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                          </tr>
                                        
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="ASD" runat="server" Text="Report Type"></asp:Label></td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlreporttype" runat="server" Width="182px"  CssClass="txtBox">
                                                <asp:ListItem Text="Select One" Selected="True" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Daily" Value="1"></asp:ListItem>
                                                     <asp:ListItem Text="As On Date" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Weekly" Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="Monthly" Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                             </td>
                                             <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="Scheduling Date"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtdate" runat="server" Width="182Px" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Stime" runat="server" Text="Scheduling Time"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtHH" runat="server" Width="55Px" CssClass="txtBox"></asp:TextBox><asp:Label ID="lblhh" runat="server" Text="HH :"></asp:Label>
                                                <asp:TextBox ID="txtMM" runat="server" Width="55Px" CssClass="txtBox"></asp:TextBox><asp:Label ID="lblmm" runat="server" Text="MM:"></asp:Label>
                                            </td>
                                        </tr>
                                        
                                        <tr>
                                            <td align="left">Mail Subject:</td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtSubject" CssClass="txtBox"
                                                    runat="server" Width="487px"></asp:TextBox></td>
                                            <%--<td><asp:Label ID="lblLnkExpiryDate" runat="server" Text="Link Expiry days" ></asp:Label><asp:TextBox id="txtLnkexpdate" runat="server" Width="35Px" CssClass="txtBox" ></asp:TextBox>  </td>--%>
                                        </tr>


                                        <tr>
                                            <td align="left" colspan="6" style="height: 250px">

                                                <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="250px">
                                                    <asp:TextBox ID="txtbody" TextMode="MultiLine" runat="server" Width="100%" Height="98%" CssClass="txtbody">
                                                    </asp:TextBox>

                                                    <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" TargetControlID="txtbody" EnableSanitization="false"></asp:HtmlEditorExtender>

                                                </asp:Panel>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="right" valign="bottom" colspan="6">
                                                <asp:Button ID="btnActEdit" runat="server" CssClass="submit_btn" Font-Bold="True" Text="Update" Width="98px" />
                                            </td>
                                        </tr>
                                </div>
                                </td>
  </tr>
  </table>
</div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopupDelete" runat="server" Style="display: none"/>
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete"
        CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" Style="display:none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Report Scheduler  Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="97%" Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelete" runat="server" Text="Yes Delete" Width="90px"
                                        OnClick="DeleteRecord" CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>
 </asp:Content>

