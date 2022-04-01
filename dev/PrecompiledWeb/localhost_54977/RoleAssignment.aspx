<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="UserMaster, App_Web_fdh01zus" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script type="text/javascript">
        function r2(r1, listr2) {

            if ($('#' + r1 + '').is(':checked')) {
                $('#' + listr2 + ' input[type=checkbox]').each(function () {
                    $(this).prop('checked', true);
                });
            }
            else {

                $('#' + listr2 + ' input[type=checkbox]').each(function ()
                { $(this).prop('checked', false); });
            }

        }

    </script>

    <script type="text/javascript" src="js/jquery.min.js">
    </script>

    <script type="text/javascript">
        function RMH(r1, listr2) {
            var checked_checkboxes = $("[id*=" + r1 + "] input:checked");
            var Unchecked_checkboxes = $("[id*=" + r1 + "]");
            var uncheckval = Unchecked_checkboxes.length
            var value = checked_checkboxes.length + 1
            if (uncheckval == value) {
                $("[id*=" + listr2 + "]").attr("checked", "checked");
            }
            else {
                $("[id*=" + listr2 + "]").removeAttr("checked");
            }

        }

    </script>
    <script type="text/javascript">
        function r(r1, listr2) {

            if ($('#ContentPlaceHolder1_' + r1 + '').is(':checked')) {
                $('#ContentPlaceHolder1_' + listr2 + ' input[type=checkbox]').each(function () {
                    $(this).prop('checked', true);
                });
            }
            else {

                $('#ContentPlaceHolder1_' + listr2 + ' input[type=checkbox]').each(function ()
                { $(this).prop('checked', false); });
            }
        }
    </script>
    <script type="text/javascript">
        function check(checkbox) {

            var cbl = document.getElementById('<%=chkDocumentType.ClientID %>').getElementsByTagName("input");
            for (i = 0; i < cbl.length; i++) cbl[i].checked = checkbox.checked;
        }
    </script>


    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <div class="form">
        <div class="doc_header">
            Role Assignment
        </div>
        <div class="row mg">
            <div class="col-md-12 col-sm-12">
                <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                    Font-Size="Small"></asp:Label>
            </div>
            <div class="col-md-12 col-sm-12">
                <asp:UpdatePanel ID="updMsg" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:Label ID="lblMsgupdate" runat="server" Font-Bold="True" ForeColor="Red"
                            Font-Size="Small"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="col-md-12 col-sm-12">
                <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnexport" />
                        <asp:PostBackTrigger ControlID="btndelete" />

                        <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="click" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="col-md-1 col-sm-1">
                            <label>Role</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddluserrole" runat="server" CssClass="txtBox" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Select User</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddluser" runat="server" CssClass="txtBox" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Doc Type</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddlSeq" runat="server" CssClass="txtBox" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3 col-sm-3">
                            <asp:ImageButton ID="btnSearch" runat="server" ToolTip="Search" CssClass="sch"
                                ImageUrl="~/Images/search.png" />
                            <asp:ImageButton ID="btnexport" ToolTip="Export" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
                             <asp:ImageButton ID="btndelete" ToolTip="Delete" Visible="false" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/redx.Png" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="col-md-12 col-sm-12">
                <asp:UpdatePanel ID="updControls" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="col-md-8 col-sm-8">
                            <asp:Panel ID="pnlFields" Width="100%" runat="server">
                            </asp:Panel>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="form" style="overflow-y: scroll; width: 95%; height: 200px">

                                <asp:CheckBox ID="chkDocType" onclick="check(this);" runat="server" />
                                <label>Document Type</label>
                                <asp:CheckBoxList ID="chkDocumentType" runat="server" CssClass="txtBox" Height="300px">
                                </asp:CheckBoxList>
                            </div>
                            <asp:CheckBox ID="chkCreateRight" Text=" Can Create" runat="server" />&nbsp;<asp:CheckBox ID="chkEditRight" Text=" Can Edit" runat="server" />
                            &nbsp;<asp:CheckBox ID="chkViewRight" Text=" Can View" runat="server" />
                            &nbsp;&nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

   <%-- <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">--%>
        <%-- <tr style="color: #000000">
            <td style="text-align: left;"></td>
        </tr>
        <tr>
            <td style="text-align: center;"></td>
        </tr>--%>

        <%-- <tr style="color: #000000">
            <td style="text-align: left; width: 99%; border: 3px double green">


                <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                    border="0">
                    <tr>
                        <td style="width: 60px;">
                           
                        </td>

                        <td style="width: 150px;"></td>

                        <td style="width: 90px;">
                           
                        </td>

                        <td style="width: 220px;"></td>

                        <td style="width: 100px;">
                           
                        </td>

                        <td style="width: 300px;"></td>

                        <td style="text-align: right;"></td>
                        <td></td>
                    </tr>
                </table>
            </td>
        </tr>--%>


      <%--  <tr style="color: #000000">
            <td style="text-align: left;" valign="top">

                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                    <tr>
                        <td rowspan="2" style="width: 65%;" valign="top"></td>
                        <td valign="top"></td>
                    </tr>
                    <tr>
                        <td>&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>



            </td>
        </tr>--%>
  <%--  </table>--%>


</asp:Content>
