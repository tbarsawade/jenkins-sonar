<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" validaterequest="false" autoeventwireup="false" inherits="TEMPLATEMASTER, App_Web_o3dtvhns" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.2/jquery.min.js"></script>
    <%-- <script src="js/jquery-ui.min.js"></script>
    <script src="js/gridviewScroll.min.js"></script>--%>
    <%--  <link href="css/GridviewScroll.css" rel="stylesheet" />--%>
    <%-- <script type="text/javascript">
        $(document).ready(function () {
            gridviewScroll();
        });

        function gridviewScroll() {
            $('#<%=gvData.ClientID%>').gridviewScroll({
                width: 997,
                height: 380,
                arrowsize: 30,
                varrowtopimg: "Images/arrowvt.png",
                varrowbottomimg: "Images/arrowvb.png",
                harrowleftimg: "Images/arrowhl.png",
                harrowrightimg: "Images/arrowhr.png"
            });
        }
        function pageLoad(sender, args) {
            if (args.get_isPartialLoad()) {
                gridviewScroll();
            }
        }
    </script>--%>
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">

        <ContentTemplate>
            <div class="container-fluid">
                <div class="form">

                    <div class="doc_header">
                        Template Master
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12" style="text-align: center;">
                            <asp:Label ID="lblMsg1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-1 col-sm-1">
                            <label>Field Name</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Value</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox"></asp:TextBox>
                        </div>
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
                                        <asp:BoundField DataField="template_name" HeaderText="Template Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Subject" HeaderText="Subject">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Action" HeaderText="Type">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="EVENTNAME" HeaderText="Document">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SUBEVENT" HeaderText="Event">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="STATUSNAME" HeaderText="WF STATUS">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="btnExtraFld" runat="server" ImageUrl="~/images/add_group.png" Height="16px" Width="16px" OnClick="addExrtafld" AlternateText="Edit" ToolTip="Add Extra Fields" />
                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" AlternateText="Edit" ToolTip="Edit" />
                                                &nbsp;
            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="deleteHit" AlternateText="Delete" ToolTip="Delete" />
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

    <asp:Panel ID="pnlPopupEdit" runat="server" Width="980px" ScrollBars="auto" Style="display: block">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr style="background: #1f6b07;">
                    <td style="width: 900px">
                        <h3>New / Update Template</h3>
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
                                            <td align="left" width="120px">Template Name</td>
                                            <td align="left" width="200px">
                                                <asp:TextBox ID="txtName" CssClass="txtBox" runat="server" Width="175px"></asp:TextBox>
                                            </td>

                                            <td align="left">Mail/Alert:  </td>
                                            <td align="left" width="200px">
                                                <asp:DropDownList ID="ddlAction" runat="server" Width="182px" CssClass="txtBox" AutoPostBack="true">
                                                    <asp:ListItem Text="SELECT ONE" Selected="True" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="ALERT" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="MAIL" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="GPS ALERT" Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="SMS ALERT" Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>

                                            <td width="250px" align="right" rowspan="5" valign="top">
                                                <span>Variables:(Copy  & Paste to Mail Body)</span><br />
                                                <asp:TextBox ID="txtvar" runat="server" TextMode="MultiLine" Width="250px"></asp:TextBox>
                                                <span>Child Variables:(Copy  & Paste to Mail Body)</span><br />
                                                <asp:TextBox ID="txtvar_chil1" runat="server" TextMode="MultiLine" Width="250px"></asp:TextBox>
                                                <asp:TextBox ID="txtvar_chil2" runat="server" TextMode="MultiLine" Width="250px"></asp:TextBox>
                                            </td>
                                            
                                        </tr>

                                        <tr>

                                            <td align="left">
                                                <asp:Label ID="lbldt" runat="server" Text="Document Type:"></asp:Label><asp:Label ID="Label3" runat="server" Style="display: none;" Text="Alert Type" /></td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlEvent" runat="server" Width="182px" CssClass="txtBox"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlnt" runat="server" Width="182px" Style="display: none;" CssClass="txtBox">
                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                    <asp:ListItem Value="1">No Signal Alert</asp:ListItem>
                                                    <asp:ListItem Value="2">Trip Alert</asp:ListItem>
                                                    <asp:ListItem Value="3">SMS Alert</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>


                                            <td align="left">Event:</td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlSBE" runat="server" Width="182px" CssClass="txtBox" AutoPostBack="True"></asp:DropDownList>
                                            </td>


                                        </tr>

                                        <tr>

                                            <%--     <td align="left">Event:</td>
  <td align ="left">
     <asp:DropDownList ID="ddlSBE" runat ="server" Width="182px" CssClass ="txtBox" AutoPostBack="True" ></asp:DropDownList>     
   </td>--%>

                                            <td align="left">
                                                <asp:Label ID="lblst" Text="Select Status" runat="server"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlWS" runat="server" Width="182px" AutoPostBack="true" CssClass="txtBox"></asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="BSD" runat="server" Text="Before SLA Hours"></asp:Label>
                                                <asp:Label ID="lblatype" Visible="false" runat="server" Text="Approval Type"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtBDays" runat="server" Width="182Px" CssClass="txtBox"></asp:TextBox>
                                                <asp:DropDownList ID="ddlatype" runat="server" Width="182px" Visible="false" CssClass="txtBox_pop" AutoPostBack="true">
                                                    <asp:ListItem Text="MANUAL" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="AUTO" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="lbldocnature" Visible="false" runat="server" Text="Document Nature"></asp:Label>
                                                <asp:Label ID="lblufield" Visible="false" runat="server" Text="User Field"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddldocnature" runat="server" Width="182px" Visible="false" CssClass="txtBox" AutoPostBack="true">
                                                    <asp:ListItem Text="CREATE" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="MODIFY" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="CANCEL" Value="2"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlufield" runat="server" Width="182px" Visible="false" CssClass="txtBox" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>


                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="ASD" runat="server" Text="After SLA Hours"></asp:Label></td>
                                            <td align="left">
                                                <asp:TextBox ID="txtAdays" runat="server" Width="182Px" CssClass="txtBox"></asp:TextBox></td>
                                            <td align="left">
                                                <asp:Label ID="Stime" runat="server" Text="Sheduling Time"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtHH" runat="server" Width="35Px" CssClass="txtBox"></asp:TextBox><asp:Label ID="lblhh" runat="server" Text="HH :"></asp:Label>
                                                <asp:TextBox ID="txtMM" runat="server" Width="35Px" CssClass="txtBox"></asp:TextBox><asp:Label ID="lblmm" runat="server" Text="MM:"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">Send To: </td>
                                            <td align="left">
                                                <asp:Panel ID="Panel1" runat="server" Height="50px" ScrollBars="Auto" Style="display: block; margin-bottom: 10px;">
                                                    <asp:CheckBoxList ID="ddlMailto" runat="server" Width="180px" CssClass="txtBox"></asp:CheckBoxList>
                                                </asp:Panel>
                                            </td>
                                            <td align="left">CC List:</td>
                                            <td align="left">
                                                <asp:Panel ID="Panel2" runat="server" Height="50px" ScrollBars="Auto" Style="display: block">
                                                    <asp:CheckBoxList ID="DDLCC" runat="server" Width="180px" CssClass="txtBox"></asp:CheckBoxList>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr id="methodselectid" runat="server" >
                                            <td align="left">Select field for attachements : </td>
                                            <td align="left">
                                                <asp:Panel ID="Panel4" runat="server" Height="50px" ScrollBars="Auto" Style="display: block; margin-bottom: 10px;">
                                                    <asp:CheckBoxList ID="methodckblist" runat="server" Width="180px" CssClass="txtBox"></asp:CheckBoxList>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr >
                                            <td align="left">BCC :</td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtBcc" CssClass="txtBox" runat="server" Width="487px"></asp:TextBox>
                                            </td>
                                        <%--<td align="center" valign="top">
                                                <asp:CheckBox ID="chkPublicView" runat="server" Text="Documnet Public View" AutoPostBack="true" />
                                            </td>--%>
                                        </tr>
                                         <tr>
                                            <td align="left">Condition:</td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="Conditiontxt" CssClass="txtBox"
                                                    runat="server" Width="487px"></asp:TextBox></td>
                                            <%--<td><asp:Label ID="lblLnkExpiryDate" runat="server" Text="Link Expiry days" ></asp:Label><asp:TextBox id="txtLnkexpdate" runat="server" Width="35Px" CssClass="txtBox" ></asp:TextBox>  </td>--%>
                                        </tr>
                                        <tr>
                                            <td align="left">Mail Subject:</td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtSubject" CssClass="txtBox"
                                                    runat="server" Width="487px"></asp:TextBox></td>
                                            <%--<td><asp:Label ID="lblLnkExpiryDate" runat="server" Text="Link Expiry days" ></asp:Label><asp:TextBox id="txtLnkexpdate" runat="server" Width="35Px" CssClass="txtBox" ></asp:TextBox>  </td>--%>
                                        </tr>

                                        
                                        <tr>
                                            <td align="left" colspan="4" style="height: auto">

                                                <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="150px">
                                                    <asp:TextBox ID="txtBody" TextMode="MultiLine" runat="server" Width="100%" Height="98%" CssClass="txtBox">
                                                    </asp:TextBox>

                                                    <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" TargetControlID="txtbody" EnableSanitization="false"></asp:HtmlEditorExtender>

                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                            </td>
                                            <td align="left">
                                                <asp:Panel ID="Panel3" runat="server" Height="50px" ScrollBars="Auto" Style="display: block; margin-bottom: 10px;">
                                                    <asp:CheckBoxList ID="CheckBoxList1" runat="server" CssClass="txtBox" Width="180px">
                                                    </asp:CheckBoxList>
                                                </asp:Panel>
                                            </td>
                                            <tr>
                                                <td align="right" colspan="5" valign="bottom">
                                                    <asp:Button ID="btnActEdit" runat="server" CssClass="submit_btn" Font-Bold="True" Text="Update" Width="98px" />
                                                </td>
                                            </tr>
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

    <asp:Button ID="btnShowPopupDelete" runat="server" Visible="false" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete"
        CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Template  Delete : Confirmation</h3>
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


    <asp:Button ID="btnaddExtrafld" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_ExtraField" runat="server"
        TargetControlID="btnaddExtrafld" PopupControlID="pnlPopupextraField"
        CancelControlID="btnCloseextraField" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupextraField" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="mod_pop">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr style="background: #1f6b07;">
                    <td>
                        <h3>Template  Add Extra Field
                            <asp:Label ID="lblTemheadr" runat="server"></asp:Label>
                        </h3>
                    </td>
                    <td style="width: 33px">
                        <asp:ImageButton ID="btnCloseextraField" ImageUrl="images/close2.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updExtraField" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblmessExtraField" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="97%" Font-Size="X-Small"></asp:Label></h2>
                                <table style="width: 100%; padding: 0 66px; display: block;">
                                    <tr>
                                        <td>
                                            <label class="label">Public Document Type </label>
                                        </td>
                                        <td style="width: 200px">
                                            <asp:DropDownList ID="ddlPvdoctype" runat="server" CssClass="txtBox_pop" Width="200px" AutoPostBack="true"></asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label class="label">Caption Text</label></td>
                                        <td style="width: 200px">
                                            <asp:TextBox ID="txtPvCaption" runat="server" CssClass="txtBox_pop" Width="190px"></asp:TextBox><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label class="label">Mode</label></td>
                                        <td style="width: 192px">
                                            <asp:DropDownList ID="ddlPvMode" runat="server" CssClass="txtBox_pop" Width="200px">
                                                <asp:ListItem>PLEASE SELECT</asp:ListItem>
                                                <asp:ListItem>NEW</asp:ListItem>
                                                <asp:ListItem>EDIT</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label class="label">Control Name</label></td>
                                        <td style="width: 200px">
                                            <asp:DropDownList ID="ddlControlName" runat="server" CssClass="txtBox_pop" Width="200px">
                                                <asp:ListItem>PLEASE SELECT</asp:ListItem>
                                                <asp:ListItem>{DOCUMENT PUBLIC VIEW LINK}</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <label class="label">Relationship</label></td>
                                        <td style="width: 200px">
                                            <asp:DropDownList ID="ddlPvrealtioship" runat="server" CssClass="txtBox_pop" Width="200px">
                                                <asp:ListItem>PLEASE SELECT</asp:ListItem>
                                                <asp:ListItem>DOCID</asp:ListItem>
                                                <asp:ListItem>DOC REFFERENCES</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <label class="label">Link Expiry</label></td>
                                        <td style="width: 200px">
                                            <asp:DropDownList ID="ddlLnkExpDate" runat="server" CssClass="txtBox_pop" Width="200px" AutoPostBack="true">
                                                <asp:ListItem Text="PLEASE SELECT" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="STATIC" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="DYNAMIC" Value="2"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEXDay" runat="server" Text="Expire after Days" Visible="false"></asp:Label></td>
                                        <td>
                                            <asp:TextBox ID="txtExpDays" runat="server" CssClass="txtBox_pop" Width="200px" Visible="false"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblHr" runat="server" Text="Hour" Visible="false"></asp:Label><asp:Label ID="lblfieldtype" runat="server" Text="Select Field" Visible="false"></asp:Label></td>
                                        <td style="width: 200px">
                                            <asp:TextBox ID="txtExpLinkHH" runat="server" CssClass="txtBox" Visible="false"></asp:TextBox><asp:DropDownList ID="ddlExPvDocField" runat="server" CssClass="txtBox" Visible="false" Width="200px"></asp:DropDownList></td>
                                    </tr>
                                    <%--<tr><td><asp:Label ID="lblExAftrDate" runat="server" Text="Days" visible="false"> </asp:Label></td><td><asp:TextBox ID="txtExaftrDate" runat="server" Visible="false" ></asp:TextBox></td></tr>--%>
                                </table>
                                <div style="width: 86%; text-align: right">
                                    <asp:Button ID="AddExtraField" runat="server" Text="ADD" Width="90px" OnClick="AddExtTemplate" CssClass="submit_btn" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>


</asp:Content>

