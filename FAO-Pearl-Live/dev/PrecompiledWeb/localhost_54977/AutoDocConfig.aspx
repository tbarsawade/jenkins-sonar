<%@ page language="VB" autoeventwireup="false" inherits="AutoDocConfig, App_Web_tuuwcp04" masterpagefile="~/usrFullScreenBPM.master" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>




<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script src="Jquery/jquery-3.3.1.min.js" type="text/javascript"></script>


    <script type="text/javascript">

        function ShowAvailability() {

            $.ajax({

                type: "POST",

                url: "AutoDocConfig.aspx/CheckUserName",

                data: '{userName: "' + $("#<%=txtConfigName.ClientID%>")[0].value + '" }',

                contentType: "application/json; charset=utf-8",

                dataType: "json",

                success: OnSuccess,

                failure: function (response) {

                    alert(response);

                }

            });

        }

        function OnSuccess(response) {

            var mesg = $("#mesg")[0];



            switch (response.d) {

                case "true":

                    mesg.style.color = "green";

                    mesg.innerHTML = "Available";

                    break;

                case "false":

                    mesg.style.color = "red";

                    mesg.innerHTML = "Not Available";

                    break;

                case "error":

                    mesg.style.color = "red";

                    mesg.innerHTML = "Error occured";

                    break;

            }

        }

        function OnChange(txt) {
            debugger;
            $("#mesg")[0].innerHTML = "";
            ShowAvailability();
        }
    </script>
    <div>
        <asp:HiddenField ID="hdnAction" runat="server" />
        <asp:UpdatePanel ID="updateautocofig" runat="server">
            <ContentTemplate>
                <table border="0px" cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%">
                    <tr style="color: #000000">
                        <td style="text-align: left; border: 3px double lime; padding-left: 1%;">
                            <div>
                                <div style="float: left;">
                                    <asp:Label ID="lblMsg" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="Red"><h4>Auto Document Configuration</h4></asp:Label>
                                </div>
                                <div style="text-align: center; vertical-align: top; float: right;">
                                    <asp:ImageButton ID="btnGpBack" runat="server" Height="30px" ImageUrl="~/Images/GoBack.png" ToolTip="Go Back to Form Master" PostBackUrl="~/FormMaster.aspx" />
                                    <asp:ImageButton ID="btnNew" runat="server" Width="30px" Height="30px" ImageUrl="~/Images/plus.jpg" OnClick="btnAutoConfig_click" ToolTip="ADD Automatic Configuration" />
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;
                        </td>
                    </tr>
                    <tr style="color: #000000">
                        <td style="text-align: left; border: 3px double blue; padding-left: 1%;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="llbFName" runat="server" Text="Field Name  : " Font-Bold="true"></asp:Label></td>
                                    <td>
                                        <asp:DropDownList ID="ddlfldname" runat="server" CssClass="txtBox" AutoPostBack="true">
                                            <asp:ListItem>Configuration Name</asp:ListItem>
                                            <asp:ListItem>Source DOC</asp:ListItem>
                                            <asp:ListItem>Target DOC</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width: 50px;">
                                        <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Value" Width="99%"></asp:Label>
                                    </td>

                                    <td style="width: 200px;">
                                        <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox" Font-Bold="True"
                                            Font-Size="Small" Width="99%"></asp:TextBox>
                                    </td>

                                    <td style="text-align: right; width: 25px">
                                        <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                            ImageUrl="~/Images/search.png" OnClick="btnSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <div id="Layer1" style="position: absolute; z-index: 1000000009; left: 50%; top: 50%">
                                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                please wait...
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <tr style="color: #000000">
                        <td style="text-align: left;" valign="top">
                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                                CellPadding="2" DataKeyNames="TID"
                                CssClass="GridView"
                                AllowSorting="True" PageSize="20" AllowPaging="True">
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
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="configname" HeaderText="Configuration Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Sourcedoc" HeaderText="Source Doc">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="targetdoc" HeaderText="Target Doc">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="noofrecords" HeaderText="No of records">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="WFstatus" HeaderText="Work Flow Status">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>

                                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip="Edit Form Detail" AlternateText="Edit" OnClick="EditHit" />
                                            &nbsp;
                                 <asp:ImageButton ID="btnAddFields" runat="server" ImageUrl="~/images/addfields.jpg" Height="16px" Width="16px" ToolTip="Add Fields" AlternateText="Add Config Details" OnClick="btnAddFields_Click" />
                                            &nbsp;   
                                 <asp:ImageButton ID="btnAddChildFields" runat="server" ImageUrl="~/images/ChildItem.jpg" Height="16px" Width="16px" ToolTip="Add Child Fields" AlternateText="Add Child Item Fields" OnClick="btnAddChildFields_Click" />
                                        </ItemTemplate>
                                        <ItemStyle Width="220px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    <asp:Button ID="btnAutoConfig" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_AutoConfig" runat="server" TargetControlID="btnAutoConfig" PopupControlID="pnlAutoConfig"
        CancelControlID="cancel_AutoConfig" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlAutoConfig" runat="server" Width="900px" Height="330px" ScrollBars="Auto" Style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Up_pnlAutoConfig" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 880px">
                                <h3>
                                    <asp:Label ID="Label17" runat="server"></asp:Label>
                                    Apply For Auto Configuration </h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="cancel_AutoConfig" ImageUrl="images/close.png" runat="server" OnClick="CloseAutoConfig" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>

                                            <td align="left" colspan="4">
                                                <asp:Label ID="lblmsgred" runat="server" ForeColor="Red"></asp:Label></td>

                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 220px">

                                                <label>*Configuration Name:</label>
                                            </td>
                                            <td>
                                                <asp:TextBox Width="140" CssClass="txtBox" ID="txtConfigName" onblur="OnChange(this)" runat="server"></asp:TextBox>
                                                <span id="mesg"></span>
                                            </td>
                                            <td align="left" style="width: 220px">
                                                <label>No of Document :</label></td>
                                            <td>
                                                <asp:TextBox CssClass="txtBox" ID="txtNoRows" runat="server" Width="135px   "></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" style="width: 220px">
                                                <label style="width: 220px">*Source Doc :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSourceForm" runat="server" OnSelectedIndexChanged="ddlSourceForm_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                            <td align="left" style="width: 220px;">
                                                <label>*Source Type :</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlFormSourceType" runat="server" Enabled="false">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>

                                            <td align="left" style="width: 220px">
                                                <label>*Creator :</label>
                                                <asp:CheckBox ID="isSecCreator" runat="server" Text="Is Secondary" OnCheckedChanged="isSecCreator_CheckedChanged" AutoPostBack="true" />
                                            </td>
                                            <td colspan="3" id="Primary" runat="server">
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList Width="150" CssClass="txtBox" AutoPostBack="true" ID="ddlCreator" runat="server" OnSelectedIndexChanged="ddlCreator_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True">SELECT</asp:ListItem>
                                                                <asp:ListItem>Role</asp:ListItem>
                                                                <asp:ListItem>Filed Value</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td align="left" style="width: 220px">
                                                            <label>*Creator Value :</label>

                                                        </td>

                                                        <td>
                                                            <asp:DropDownList CssClass="txtBox" ID="ddlCreatorValue" Width="150" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                            <td colspan="3" id="Secondary" runat="server" visible="false">
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList Width="150" CssClass="txtBox" AutoPostBack="true" ID="ddlSecSourceCreator" runat="server" OnSelectedIndexChanged="ddlSecSourceCreator_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True">SELECT</asp:ListItem>
                                                                <asp:ListItem>Role</asp:ListItem>
                                                                <asp:ListItem>Filed Value</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td align="left" style="width: 220px">
                                                            <label>*Sec Creator Value :</label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList CssClass="txtBox" ID="ddlSecSourceCreatorValue" Width="150" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 220px">
                                                <label>Condition Data Fields :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlConditionDataFields" OnSelectedIndexChanged="ddlConditionDataFields_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                                            </td>

                                            <td align="left" style="width: 220px">
                                                <label>Condition Data values :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlConditionValues" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr style="vertical-align: top;">
                                            <td align="left" style="width: 220px">
                                                <label>Primary Condition Data :</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlANDOR" runat="server" Width="135" CssClass="txtBox">
                                                    <asp:ListItem Selected="True">AND</asp:ListItem>
                                                    <asp:ListItem>OR</asp:ListItem>
                                                </asp:DropDownList><asp:ImageButton ID="imgAddCondition" runat="server" AlternateText="Add" ImageUrl="~/images/0000000000019.gif" Width="15" Height="15" OnClick="imgAddCondition_Click" />
                                            </td>
                                            <td colspan="2">
                                                <div style="width: 100%; vertical-align: top;">
                                                    <div style="width: 98%">
                                                        <asp:Panel ID="pnlformula" runat="server" Width="96%" Height="40" ScrollBars="Auto">
                                                            <asp:TextBox ID="txtShowAddConditionArea" runat="server" CssClass="txtBox" TextMode="MultiLine" Height="40px" Width="95%"></asp:TextBox>
                                                            <asp:TextBox ID="txtAddConditionArea" runat="server" CssClass="txtBox" TextMode="MultiLine" Height="40px" Width="95%"></asp:TextBox>
                                                        </asp:Panel>
                                                    </div>
                                                    <div style="float: left;">
                                                        <asp:ImageButton ID="ImgReferesh" runat="server" AlternateText="Referesh" ToolTip="Referesh" ImageUrl="~/images/refresh.png" Width="15" Height="15" OnClick="ImgReferesh_Click" />
                                                    </div>
                                                </div>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <div style="height: 20px; width: 100%; background-color: #1F6B08; font-weight: bold; color: white; text-align: left;">
                                                    Seconday Field Configuration 
                                                </div>
                                            </td>
                                        </tr>

                                        <tr id="Secondarydoc" runat="server" visible="true">
                                            <td align="left" style="width: 220px">
                                                <label>Secondary Source Doc :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSecondarySourceForm" OnSelectedIndexChanged="ddlSecondarySourceForm_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                                            </td>
                                            <td align="left" style="width: 220px">
                                                <label>Secondary Source Type :</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSecondarySourceType" runat="server" Enabled="false">
                                                    <asp:ListItem>DOCUMENT</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="commanfilter" runat="server" visible="false" valign="top">
                                            <td align="left" colspan="4">
                                                <table width="100%;">
                                                    <tr>
                                                        <td colspan="4">
                                                            <div style="width: 100%; vertical-align: top;">
                                                                <div style="width: 20%; float: left;">
                                                                    <label style="width: 220px">Filter Fields:</label>
                                                                </div>
                                                                <div style="text-align: left; float: right; vertical-align: top;">
                                                                    <asp:Panel BorderWidth="1" BorderColor="#D0D0D0" ID="Panel1" Width="100%" Height="100px" runat="server" ScrollBars="Auto">
                                                                        <asp:CheckBoxList RepeatColumns="2" RepeatDirection="Vertical" ID="chkCommanFields" runat="server"></asp:CheckBoxList>
                                                                    </asp:Panel>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>

                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" style="width: 220px">
                                                <label>Sec Condition Data Fields :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSConditionDataFields" OnSelectedIndexChanged="ddlSConditionDataFields_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                                            </td>
                                            <td align="left" style="width: 220px">
                                                <label>Sec Condition Data values :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSConditionValues" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr style="vertical-align: top;">
                                            <td align="left" style="width: 220px">
                                                <label>Secondary Condition Data :</label>

                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlSANDOR" runat="server" Width="135" CssClass="txtBox">
                                                    <asp:ListItem Selected="True">AND</asp:ListItem>
                                                    <asp:ListItem>OR</asp:ListItem>
                                                </asp:DropDownList><asp:ImageButton ID="imgSAddCondition" runat="server" AlternateText="Add" ImageUrl="~/images/0000000000019.gif" Width="15" Height="15" OnClick="imgSAddCondition_Click" />
                                            </td>
                                            <td colspan="2">
                                                <div style="width: 100%; vertical-align: top;">
                                                    <div style="width: 98%">
                                                        <asp:Panel ID="pnlSformula" runat="server" Width="96%" Height="40" ScrollBars="Auto">
                                                            <asp:TextBox ID="txtSShowAddConditionArea" runat="server" CssClass="txtBox" TextMode="MultiLine" Height="40px" Width="95%"></asp:TextBox>
                                                            <asp:TextBox ID="txtSAddConditionArea" runat="server" CssClass="txtBox" TextMode="MultiLine" Height="40px" Width="95%"></asp:TextBox>
                                                        </asp:Panel>
                                                    </div>
                                                    <div style="float: left;">
                                                        <asp:ImageButton ID="ImgSReferesh" runat="server" AlternateText="Referesh" ToolTip="Referesh" ImageUrl="~/images/refresh.png" Width="15" Height="15" OnClick="ImgSReferesh_Click" />
                                                    </div>
                                                </div>
                                            </td>

                                        </tr>
                                        <tr>

                                            <td colspan="4">
                                                <div style="height: 20px; width: 100%; background-color: #1F6B08; font-weight: bold; color: white; text-align: left;">
                                                    Target Field Configuration 
                                                </div>
                                            </td>


                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 220px">
                                                <label>*Target Doc :</label></td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlTargetDoc" OnSelectedIndexChanged="ddlTargetDoc_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                                            </td>
                                            <td align="left" style="width: 220px">
                                                <label>*Target Type :</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlTargetType" runat="server" Enabled="false">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>

                                            <td align="left" style="width: 220px">
                                                <label>*Create Event :</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList Width="150" CssClass="txtBox" ID="ddlCreateEvent" OnSelectedIndexChanged="ddlCreateEvent_SelectedIndexChanged" AutoPostBack="true" runat="server">
                                                    <asp:ListItem Selected="True">SELECT</asp:ListItem>
                                                    <asp:ListItem>Schedule</asp:ListItem>
                                                    <asp:ListItem>Save</asp:ListItem>
                                                    <asp:ListItem>Approval</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" style="width: 220px">
                                                <label>Work Flow Status :</label></td>
                                            <td>
                                                <asp:DropDownList CssClass="txtBox" ID="ddlWFStatus" Width="150" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr id="schtime" runat="server" visible="false">
                                            <td align="left" colspan="4">
                                                <div style="width: 100%;">
                                                    <div style="width: 25%; float: left;">
                                                        <label>Schedule Time :</label>
                                                    </div>
                                                    <div style="float: right; width: 72%;">
                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                            <tr>
                                                                <td style="width: 20%;"><b>MM</b>
                                                                </td>
                                                                <td style="width: 20%;"><b>DD</b>
                                                                </td>
                                                                <td style="width: 20%;"><b>WW</b>
                                                                </td>
                                                                <td style="width: 20%;"><b>HH</b>
                                                                </td>
                                                                <td style="width: 20%;"><b>MN</b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 20%;">
                                                                    <asp:TextBox ID="txtMM" runat="server" CssClass="txtBox" Width="70%"></asp:TextBox>
                                                                    <asp:FilteredTextBoxExtender ID="fil_txtMM" runat="server" Enabled="true" TargetControlID="txtMM" FilterMode="ValidChars" ValidChars="0123456789-*"></asp:FilteredTextBoxExtender>

                                                                </td>
                                                                <td style="width: 20%;">
                                                                    <asp:TextBox ID="txtDD" runat="server" CssClass="txtBox" Width="70%"></asp:TextBox>
                                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" Enabled="true" TargetControlID="txtDD" FilterMode="ValidChars" ValidChars="0123456789-*"></asp:FilteredTextBoxExtender>
                                                                </td>
                                                                <td style="width: 20%;">
                                                                    <asp:TextBox ID="txtWW" runat="server" CssClass="txtBox" Width="70%"></asp:TextBox>
                                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" Enabled="true" TargetControlID="txtWW" FilterMode="ValidChars" ValidChars="0123456789-*"></asp:FilteredTextBoxExtender>
                                                                </td>
                                                                <td style="width: 20%;">
                                                                    <asp:TextBox ID="txtHH" runat="server" CssClass="txtBox" Width="70%"></asp:TextBox>
                                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" Enabled="true" TargetControlID="txtHH" FilterMode="ValidChars" ValidChars="0123456789-*"></asp:FilteredTextBoxExtender>
                                                                </td>
                                                                <td style="width: 20%;">
                                                                    <asp:TextBox ID="txtMN" runat="server" CssClass="txtBox" Width="70%"></asp:TextBox>
                                                                    <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" Enabled="true" TargetControlID="txtMN" FilterMode="ValidChars" ValidChars="0123456789-*"></asp:FilteredTextBoxExtender>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <div style="clear: both;"></div>
                                                </div>

                                                <asp:TextBox ID="txtschtime" runat="server" CssClass="txtBox" Visible="false"></asp:TextBox>
                                                <%-- <td style="">--%>

                                                <%-- </td>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="4">
                                                <asp:CheckBox ID="chkisactive" runat="server" Text="Active" Checked="true" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <asp:Button ID="btnSave" runat="server" CssClass="btnNew" OnClick="btnSave_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:Button ID="btnAutoConfigdtl" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_AutoConfigdtl" runat="server" TargetControlID="btnAutoConfigdtl" PopupControlID="pnlAutoConfigdtl"
        CancelControlID="cancel_AutoConfigdtl" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlAutoConfigdtl" runat="server" Width="900px" Height="330px" ScrollBars="Auto" Style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Up_pnlAutoConfigdtl" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 880px">
                                <h3>
                                    <asp:Label ID="Label1" runat="server"></asp:Label>
                                    Apply For Auto Configuration Details </h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="cancel_AutoConfigdtl" ImageUrl="images/close.png" runat="server" OnClick="CloseAutoConfigdtl" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>

                                            <td align="left" colspan="2">
                                                <asp:Label ID="lblmsgreddtl" runat="server" ForeColor="Red"></asp:Label></td>

                                        </tr>

                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>


    <asp:Button ID="btnAutoConfigChilddtlMapping" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_AutoConfigChilddtlMapping" runat="server" TargetControlID="btnAutoConfigChilddtlMapping" PopupControlID="pnlAutoConfigdtlMappingChild"
        CancelControlID="cancel_AutoConfigdtlMappingChild" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlAutoConfigdtlMappingChild" runat="server" Width="900px" Height="330px" ScrollBars="Auto" Style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UP_AutoConfigChilddtlMappingChild" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 880px">
                                <h3>
                                    <asp:Label ID="Label3" runat="server"></asp:Label>
                                    Apply For Auto Configuration Child Item Details </h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="cancel_AutoConfigdtlMappingChild" ImageUrl="images/close.png" runat="server" OnClick="CloseAutoConfigdtlMappingChild" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>

                                            <td align="left" colspan="2">
                                                <asp:Label ID="lblChildRedMsg" runat="server" ForeColor="Red"></asp:Label></td>

                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label style="width: 220px">*Target Child Document :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlTargetChildDoc" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTargetChildDoc_SelectedIndexChanged">
                                                </asp:DropDownList></td>

                                            <td>
                                                <label style="width: 220px">*Source Child Document :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSourceChildDoc" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSourceChildDoc_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label style="width: 220px">*Mapping Type :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlChildMappingType" runat="server" OnSelectedIndexChanged="ddlChildMappingType_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Selected="True">SELECT</asp:ListItem>
                                                    <asp:ListItem>FIX</asp:ListItem>
                                                    <asp:ListItem>COPY</asp:ListItem>
                                                    <asp:ListItem>FORMULA</asp:ListItem>
                                                </asp:DropDownList></td>
                                            <%-- <td align="left">
                                                <label style="width: 220px">*Mapping Fromula :</label><asp:TextBox Width="150" CssClass="txtBox" ID="txtMappingFormula" runat="server">      </asp:TextBox>
                                            </td>--%>
                                            <td>
                                                <label style="width: 220px">Remarks :</label><asp:TextBox Width="150" CssClass="txtBox" ID="txtChildRemarks" runat="server">
                                                </asp:TextBox>
                                                <td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label style="width: 220px">*Target Mapping :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlChildTargetMapping" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <label id="lblChildMappingType" runat="server" style="width: 220px"></label>
                                                <asp:DropDownList Width="150" Visible="false" CssClass="txtBox" ID="ddlChildSourceMapping" runat="server"></asp:DropDownList>
                                                <asp:TextBox Width="150" CssClass="txtBox" ID="txtChildFixMapping" Visible="false" runat="server">      </asp:TextBox>
                                                <asp:TextBox Width="150" CssClass="txtBox" ID="txtChildFormulaMapping" Visible="false" runat="server">      </asp:TextBox>
                                                <asp:ImageButton ID="btnChildAddFieldsMapping" runat="server" ImageUrl="~/images/addfields.jpg" Height="16px" Width="16px" ToolTip="Add Fields" AlternateText="Add Config Details" OnClick="btnAddFieldsMapping_Click" />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td colspan="2">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button ID="btnSaveDtlChild" Text="SAVE" runat="server" CssClass="btnNew" OnClick="btnSaveDtlChild_Click" />
                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td align="left">
                                                <label>Add Fields Mapping</label>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:Panel ID="Panel2" runat="server" Height="200px" ScrollBars="Auto"
                                                    Width="100%">
                                                    <asp:GridView ID="GridView1" runat="server" AllowPaging="false"
                                                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                                        DataKeyNames="TID" Width="100%">
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
                                                                <ItemStyle Width="50px" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="ChildTargetdoc" HeaderText="Target Document">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="ChildSourcedoc" HeaderText="Source Document">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="MappingType" HeaderText="Mapping Type">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Remarks" HeaderText="Remarks">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Action">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitUserChild"
                                                                        ToolTip="Delete" Width="16px" OnClientClick="return confirm('Are you sure to delte this record ?')" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" Width="140px" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td colspan="2">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button ID="Button4" runat="server" CssClass="btnNew" OnClick="btnSaveDtl_Click" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:Button ID="btnAutoConfigdtlMapping" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_AutoConfigdtlMapping" runat="server" TargetControlID="btnAutoConfigdtlMapping" PopupControlID="pnlAutoConfigdtlMapping"
        CancelControlID="cancel_AutoConfigdtlMapping" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlAutoConfigdtlMapping" runat="server" Width="900px" Height="330px" ScrollBars="Auto" Style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Up_pnlAutoConfigdtlMapping" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 880px">
                                <h3>
                                    <asp:Label ID="Label2" runat="server"></asp:Label>
                                    Apply For Auto Configuration Details </h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="cancel_AutoConfigdtlMapping" ImageUrl="images/close.png" runat="server" OnClick="CloseAutoConfigdtlMapping" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>

                                            <td align="left" colspan="2">
                                                <asp:Label ID="lblmsgredmapping" runat="server" ForeColor="Red"></asp:Label></td>

                                        </tr>
                                        <tr>

                                            <td align="left">
                                                <label style="width: 220px">*Target Document :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlMainTarget" runat="server"></asp:DropDownList>
                                            </td>
                                            <td>
                                                <label style="width: 220px">*Primary/Secondary Document :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlMainSource" runat="server" OnSelectedIndexChanged="ddlMainSource_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label style="width: 220px">*Mapping Type :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlMappingType" runat="server" OnSelectedIndexChanged="ddlMappingType_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Selected="True">SELECT</asp:ListItem>
                                                    <asp:ListItem>FIX</asp:ListItem>
                                                    <asp:ListItem>COPY</asp:ListItem>
                                                    <asp:ListItem>FORMULA</asp:ListItem>
                                                </asp:DropDownList></td>
                                            <%-- <td align="left">
                                                <label style="width: 220px">*Mapping Fromula :</label><asp:TextBox Width="150" CssClass="txtBox" ID="txtMappingFormula" runat="server">      </asp:TextBox>
                                            </td>--%>
                                            <td>
                                                <label style="width: 220px">*Remarks :</label><asp:TextBox Width="150" CssClass="txtBox" ID="txtRemarks" runat="server">
                                                </asp:TextBox>
                                                <td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label style="width: 220px">*Target Mapping :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddltargetfldMapping" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <label id="lblMapping" runat="server" style="width: 220px"></label>
                                                <asp:DropDownList Width="150" Visible="false" CssClass="txtBox" ID="ddlSourceFldMapping" runat="server"></asp:DropDownList>
                                                <asp:TextBox Width="150" CssClass="txtBox" ID="txtMappingFormula" Visible="false" runat="server">      </asp:TextBox>
                                                <asp:TextBox Width="150" CssClass="txtBox" ID="txtFIX" Visible="false" runat="server">      </asp:TextBox>
                                                <asp:ImageButton ID="btnAddFieldsMapping" runat="server" ImageUrl="~/images/addfields.jpg" Height="16px" Width="16px" ToolTip="Add Fields" AlternateText="Add Config Details" OnClick="btnAddFieldsMapping_Click" />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td colspan="2">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button ID="btnSaveDtl" runat="server" CssClass="btnNew" OnClick="btnSaveDtl_Click" />
                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td align="left">
                                                <label>Add Fields Mapping</label>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <asp:Panel ID="PnlGrid" runat="server" Height="200px" ScrollBars="Auto"
                                                    Width="100%">
                                                    <asp:GridView ID="gvUsers" runat="server" AllowPaging="false"
                                                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                                        DataKeyNames="TID" Width="100%">
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
                                                                <ItemStyle Width="50px" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="TargetFieldName" HeaderText="Field Name">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Mtype" HeaderText="Mapping Type">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Value" HeaderStyle-Width="100px">
                                                                <ItemStyle Width="100px" />
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSfldMapping" runat="server" Text='<%  #Eval("SfldMapping")%>' ToolTip='<% #Eval("description")%>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <%--<asp:BoundField DataField="SfldMapping" HeaderText="Values" ToolTip='<%# Eval("SfldMapping") %>'>
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>--%>
                                                            <asp:BoundField DataField="Remarks" HeaderText="Remarks">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Action">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitUser"
                                                                        ToolTip="Delete" Width="16px" OnClientClick="return confirm('Are you sure to delte this record ?')" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" Width="140px" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td colspan="2">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button ID="Button1" runat="server" Visible="false" CssClass="btnNew" OnClick="btnSaveDtl_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>




    <asp:Button ID="buttonAF" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupadvformula" runat="server"
        TargetControlID="buttonAF" PopupControlID="paneladvanceformula"
        CancelControlID="imgadfor" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="paneladvanceformula" runat="server" Width="1000px" Height="450px" Style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updpaneladvanceformula" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <table cellspacing="2px" cellpadding="2px" width="100%">

                        <tr>
                            <td style="width: 880px">

                                <script type="text/javascript">

                                    function storeCaret(textEl) { textEl.caretPos = document.selection.createRange().duplicate(); }
                                    function insertAtCaret(textEl, text) { if (textEl.createTextRange && textEl.caretPos) { textEl.caretPos.text = text; } else { alert('no carat'); } }

                                    function insertCode(t) {
                                        var subject = document.getElementById('<%= txtcontionadvform.ClientID%>');
                                        if (document.selection) {
                                            insertAtCaret(document.getElementById('<%= txtcontionadvform.ClientID%>'), '{' + t + '}');
                                        }
                                        else if (subject.selectionStart || subject.selectionStart == '0') {
                                            var str = subject.value;
                                            var a = subject.selectionStart, b = subject.selectionEnd;
                                            subject.value = str.substring(0, a) + arguments[0] + (arguments[1] ? str.substring(a, b) + arguments[1] : "") + str.substring(b, str.length);
                                            return;
                                        }
                                    };

                                    function setCaret(t) {
                                        // var inp = document.getElementById('selectList');
                                        insertCode('{' + t + '}');
                                    }



                                    function setCaretcccccccccc(t) {
                                        // var textarea = document.getElementById('<%= txtcontionadvform.ClientID%>');
                                        var e = document.getElementById('<%= tvadvform.ClientID%>');
                                        var text = e.options[e.selectedIndex].value;
                                        var textarea = document.getElementById('txtcontionadvform'),
                                            tempStr1 = textarea.value.substring(0, globalCursorPos),
                                            tempStr2 = textarea.value.substring(globalCursorPos);
                                        textarea.value = tempStr1 + t + tempStr2;
                                        document.getElementById("Line").innerHTML = '<strong>Start</strong> ' + tempStr1;
                                        document.getElementById("Column").innerHTML = '<strong>End</strong> ' + tempStr2;
                                    }
                                    function updatePosition(t) {
                                        globalCursorPos = getCursorPos(t);
                                    }
                                    var globalCursorPos;
                                    function getCursorPos(textElement) {
                                        //save off the current value to restore it later,
                                        var sOldText = textElement.value;


                                        //create a range object and save off it's text
                                        var objRange = document.selection.createRange();
                                        var sOldRange = objRange.text;


                                        //set this string to a small string that will not normally be encountered
                                        var sWeirdString = '#%~';


                                        //insert the weirdstring where the cursor is at
                                        objRange.text = sOldRange + sWeirdString; objRange.moveStart('character', (0 - sOldRange.length - sWeirdString.length));


                                        //save off the new string with the weirdstring in it
                                        var sNewText = textElement.value;


                                        //set the actual text value back to how it was
                                        objRange.text = sOldRange;


                                        //look through the new string we saved off and find the location of
                                        //the weirdstring that was inserted and return that value
                                        for (i = 0; i <= sNewText.length; i++) {
                                            var sTemp = sNewText.substring(i, i + sWeirdString.length);
                                            if (sTemp == sWeirdString) {
                                                var cursorPos = (i - sOldRange.length);
                                                return cursorPos;
                                            }
                                        }
                                    }

                                </script>
                                <h3>
                                    <asp:Label ID="Label13" runat="server"></asp:Label>
                                    Apply Advance Formula</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="imgadfor" ImageUrl="images/close.png" runat="server" OnClick="advanceformulaclose" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form" style="overflow: scroll; width: 990px;">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                        <tr>
                                            <td align="left" colspan="3">
                                                <asp:Label ID="lblmsgadvanceformula" runat="server" ForeColor="Red"></asp:Label></td>
                                        </tr>

                                        <tr>
                                            <td style="width: 150px; display: none;">
                                                <label>* Display Name :</label><br />
                                                <asp:TextBox ID="txtdisplaynameadvform" runat="server" CssClass="txtBox"></asp:TextBox>

                                            </td>
                                            <td style="width: 150px; display: none;">
                                                <label>Description :</label><br />
                                                <asp:TextBox ID="txtdescadvform" runat="server" CssClass="txtBox"></asp:TextBox>

                                            </td>
                                            <td style="width: 150px; display: none;">
                                                <label>Is Active :</label><br />
                                                <asp:CheckBox ID="chkadvformactive" runat="server" /></td>
                                            <td style="width: 150px;">
                                                <label>Form Source :</label><br />
                                                <asp:DropDownList ID="ddladvfoftype" CssClass="txtbox" Height="25px" runat="server" Width="150px" AutoPostBack="true">
                                                    <asp:ListItem Value="0">SELECT</asp:ListItem>
                                                    <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                    <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                    <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                                    <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                                </asp:DropDownList>

                                            </td>
                                            <td style="width: 150px;">
                                                <label>Document Type :</label><br />
                                                <asp:DropDownList ID="ddladvfodoctype" AutoPostBack="true" Height="25px" CssClass="txtbox" runat="server" Width="150px">
                                                </asp:DropDownList>
                                            </td>


                                        </tr>
                                        <tr>

                                            <td colspan="4">

                                                <asp:Button ID="btnpop" runat="server" Text="Add More Document" Visible="True" CssClass="btnNew" OnClick="AddRelation" />

                                            </td>

                                        </tr>
                                        <tr>

                                            <td style="width: 350px; height: 200px;" colspan="2">

                                                <div style="overflow: scroll; float: left; margin: 10px; width: 480px; height: 200px;">
                                                    <asp:TreeView ID="tvadvform" EnableClientScript="true" runat="server" Height="100%" ImageSet="Inbox"
                                                        Width="100%">
                                                        <ParentNodeStyle Font-Bold="False" />
                                                        <HoverNodeStyle Font-Underline="True" />
                                                        <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                            VerticalPadding="0px" />
                                                        <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                            HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                    </asp:TreeView>
                                                    <asp:TreeView ID="tvsource" EnableClientScript="true" runat="server" Height="100%" ImageSet="Inbox"
                                                        Width="100%">
                                                        <ParentNodeStyle Font-Bold="False" />
                                                        <HoverNodeStyle Font-Underline="True" />
                                                        <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                            VerticalPadding="0px" />
                                                        <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                            HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                    </asp:TreeView>
                                                    <asp:Panel ID="pnllsd" runat="server">
                                                    </asp:Panel>
                                                </div>




                                            </td>
                                            <td style="width: 300px; height: 200px;" colspan="3">
                                                <div style="float: left; margin: 10px; margin-left: 0px; width: 450px; height: 200px;">
                                                    <asp:TextBox Height="200px" ID="txtcontionadvform" onclick="storeCaret(this);"
                                                        onselect="storeCaret(this);" onkeyup="storeCaret(this);" runat="server" Style="border: 1px dashed  #54c618;" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                                </div>

                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="width: 600px;" colspan="4">
                                                <style type="text/css">
                                                    .myPanelClass {
                                                        max-height: 400px;
                                                        width: 100%;
                                                        overflow: auto;
                                                    }
                                                </style>
                                                <asp:UpdatePanel runat="server" ID="updgrid" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <asp:Panel runat="server" ID="pblm" CssClass="myPanelClass">

                                                            <asp:GridView ID="gvmap" OnRowDataBound="gvmapOnRowDataBound" OnRowDeleting="gvmapOnRowDeleting" Width="98%" runat="server" AutoGenerateColumns="true">
                                                                <Columns>
                                                                    <%--<asp:TemplateField HeaderText="Action">
                                                                                        <ItemTemplate>
                                                                                            <asp:ImageButton ID="btnDelete"  ImageUrl="~/images/closered.png" CommandName="Delete"  runat="server" Height="16px" Width="16px" ToolTip="Delete Role" AlternateText="Delete" />
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                                                                                    </asp:TemplateField>--%>


                                                                    <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />
                                                                </Columns>
                                                            </asp:GridView>

                                                        </asp:Panel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <tr>

                                            <td>
                                                <asp:Button ID="btnsaveadvformula" runat="Server" CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord" /></td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>


    <asp:Button ID="btnformularel" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupformularelation" runat="server"
        TargetControlID="btnformularel" PopupControlID="panelformularelation"
        CancelControlID="btncloseformrel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="panelformularelation" runat="server" Width="600px" BackColor="aqua">
        <div class="box" style="height: 200px; overflow: scroll;">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>New / Update Role</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btncloseformrel"
                            ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="UpdatePanelFormulaRelation" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td colspan="2" align="left">
                                            <asp:Label ID="lblmsgFrelation" runat="server" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="width: 125px" align="left">Source Document<b>  </b></td>
                                        <td align="left">
                                            <asp:Label ID="lblsd" runat="server"></asp:Label></td>

                                        <td style="width: 150px" align="left">Source Doc Fields</td>
                                        <td>
                                            <asp:DropDownList ID="ddlsdf" runat="server" Width="99%"></asp:DropDownList>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td style="width: 150px" align="left">
                                            <asp:Label ID="lblsformsource" Text="Target Doc Type" runat="server"></asp:Label>
                                        </td>
                                        <td style="width: 150px;">
                                            <asp:DropDownList ID="ddlsourcetype" AutoPostBack="true" Width="99%" runat="server">
                                                <asp:ListItem Value="0">SELECT</asp:ListItem>
                                                <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                                <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                            </asp:DropDownList></td>
                                        <td>Target Document Name</td>
                                        <td style="width: 150px;">
                                            <asp:DropDownList Width="99%" AutoPostBack="true" ID="ddlsourcedoc" runat="server">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 150px" align="left">
                                            <asp:Label ID="lbldss" runat="server" Text="Target Fields"></asp:Label>
                                        </td>
                                        <td style="width: 150px" align="left">
                                            <asp:DropDownList ID="ddltf" Width="99%" runat="server"></asp:DropDownList>
                                        </td>
                                        <td style="width: 150px" align="left">
                                            <asp:Label ID="lblsortsource" runat="server" Text="Sorting Fields"></asp:Label></td>
                                        <td style="width: 150px" align="left">
                                            <asp:DropDownList Width="99%" ID="ddlsortingfields" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>



                                </table>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnsavefrelation" runat="server" Text="Update"
                                        OnClick="EditRecordFormulaRelation" CssClass="btnNew" Font-Bold="True"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>








