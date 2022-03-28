<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="NewRoleAssignment, App_Web_whgqqjhx" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
                            <label>Select User</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddluser" runat="server" CssClass="txtBox" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Role</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:DropDownList ID="ddluserrole" runat="server" CssClass="txtBox" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                      <%-- <div class="col-md-1 col-sm-1">
                        <label>Select a file:</label>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:FileUpload runat="server" ID="CSVUploader" CssClass="txtBox" ToolTip="Select a CSV file to upload." />
                    </div>--%>
                        <div class="col-md-1 col-sm-1" id="lblDtype" runat="server">
                            <label>Doc Type</label>
                        </div>
                        <div class="col-md-2 col-sm-2" id="ddddtype" runat="server">
                            <asp:DropDownList ID="ddlSeq" runat="server" CssClass="txtBox" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <%--<asp:ImageButton ID="btnSearch" runat="server" ToolTip="Search" CssClass="sch"
                                ImageUrl="~/Images/search.png" />     --%>                       
                          
                        </div>
                        <div class="col-md-2 col-sm-2">
                             <asp:ImageButton ID="btnexport" ToolTip="Export" runat="server" Width="18px"  Height="18px" ImageUrl="~/Images/excelexpo.jpg" />&nbsp;
                              <asp:ImageButton ID="btndelete" ToolTip="Delete" Visible="false" runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/Delete.Gif" />&nbsp;
                            <asp:ImageButton ID="btnimmm" ToolTip="Import" runat="server" Width="18px"  Height="18px" ImageUrl="~/Images/import.png" />&nbsp;
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="col-md-12 col-sm-12">
                <asp:UpdatePanel ID="updControls" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="col-md-12 col-sm-12" id="PnlRights" runat="server">
                            <asp:Panel ID="pnlFields" Width="100%" runat="server">
                            </asp:Panel>
                        </div>
                        <div id="DocType" runat="server">
                            <div class="form" style="overflow-y: scroll; width: 0%; display:none; height: 0px">
                                <asp:CheckBox ID="chkDocType" onclick="check(this);" Visible="false" runat="server" />
                               <%-- <label>Document Type</label>--%>
                                <asp:CheckBoxList ID="chkDocumentType"  runat="server" Visible="true" CssClass="txtBox" Height="0px">
                                </asp:CheckBoxList>
                            </div>
                            <asp:CheckBox ID="chkCreateRight" Visible="false" Text=" Can Create" runat="server" />&nbsp;<asp:CheckBox ID="chkEditRight"  Visible="false" Text=" Can Edit" runat="server" />
                            &nbsp;<asp:CheckBox ID="chkViewRight" Text=" Can View"  Visible="false" runat="server" />
                            &nbsp;&nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            </div>
     <asp:Button ID="btnim" runat="server" Style="display: none;" />
    <asp:ModalPopupExtender ID="modalpopupimport" runat="server"
        TargetControlID="btnim" PopupControlID="pnlimport"
        CancelControlID="ImageButton2" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
        
    <asp:Panel ID="pnlimport" runat="server" Width="400px" Height="200px" Style="display: none;" BackColor="white">
        <div class="box">
            <table cellspacing="1px" cellpadding="2px" width="99%" border="0px">
                <tr>
                    <td>
                       <h3>
                         <asp:Label ID="lblpophead" runat="server" Text="Import Csv File" Font-Bold="True"></asp:Label></h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton2"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>

                <tr>
                    <td colspan="2">
                        <br />
                        <asp:FileUpload ID="impfile" CssClass="CS" runat="server" />
                        &nbsp;
                        <asp:CheckBoxList ID="chkboxforup"  runat="server" Visible="false" CssClass="txtBox" Height="0px">
                         </asp:CheckBoxList>
             <asp:ImageButton ID="btnimport" ToolTip="Import" Style="vertical-align: bottom;" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/import.png" />&nbsp;
              <asp:ImageButton ID="helpexport" ToolTip="Download Import Sample & Format" runat="server" Style="vertical-align: bottom;" Width="20px" Height="20px" ImageUrl="~/Images/Helpexp.png" />
                        &nbsp;</td>

                </tr>
            </table>
        </div>
    </asp:Panel>
    </div>
     
</asp:Content>

