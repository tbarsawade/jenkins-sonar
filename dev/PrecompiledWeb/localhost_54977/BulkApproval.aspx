<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" enableeventvalidation="false" inherits="BulkApproval, App_Web_qpjniz5y" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
     <style>
        .form { min-height: 0px !important; }
        .btnNew { margin: 0px 2px 10px !important;}
    </style>
    <script type="text/javascript" language="javascript">
        function ValidateForm() {
            var Doc = "0";
            var WF = "0";
            Doc = $("#ContentPlaceHolder1_ddlDocumentType").val();
            WF = $("#ContentPlaceHolder1_ddlWF").val();
            //DocName = $("#ContentPlaceHolder1_ddlAppDocType").val();
            var IsValid = true;
            var Error = "Error(s) in your submission.\n----------------------------------------\n"
            if (Doc == "0") {
                IsValid = false;
                Error = Error + "Document Type required.\n"
            }
            if (WF == "0") {
                IsValid = false;
                Error = Error + "Work Flow Status required.\n"
            }
            if (IsValid == false) {
                alert(Error);
            }
            return IsValid;
        }
        function DocumentFilter() {
            var Field = "0", VALUE = "0";
            Field = $("#ContentPlaceHolder1_ddlFieldName").val();
            VALUE = $("#ContentPlaceHolder1_ddlFormValue").val();
            var IsValid = true;
            var Error = "Error(s) in your submission.\n----------------------------------------\n"
            if (Field == "0") {
                IsValid = false;
                Error = Error + "Filter on required.\n"
            }
            if (VALUE == "0") {
                IsValid = false;
                Error = Error + "Value required.\n"
            }
            if (IsValid == false) {
                alert(Error);
            }
            return IsValid;
        }
        function checkAll() {
            //var obj = $("#ContentPlaceHolder1_gvData_ctl00").attr('checked');
            var obj = $("#ContentPlaceHolder1_gvData_ctl00");
            var str = $(obj).prop('checked');
            if (str == true) {
                $("#ContentPlaceHolder1_gvData").find(':checkbox').each(function () {
                    $(this).prop('checked', "checked");
                });
            }
            else {
                $("#ContentPlaceHolder1_gvData").find(':checkbox').each(function () {
                    $(this).removeAttr('checked');
                });
            }
        }
        function Approve() {
            var ret = false;
            var Count = 0;
            var obj = $("#ContentPlaceHolder1_gvData_ctl00");
            var chkAll = $(obj).prop('checked');
            $("#ContentPlaceHolder1_gvData").find(':checkbox').each(function () {
                var IsChecked = $(this).prop('checked');
                if (IsChecked == true) {
                    Count = Count + 1;
                }
            });
            if (chkAll == true) {
                Count = Count - 1;
            }
            if (Count > 0) {
                var Msg = "You are going to approve " + Count + " document(s).Click on, ''Ok'' to proceed, ''Cancel'' to deny.";
                if (confirm(Msg)) {
                    $("#ContentPlaceHolder1_btnApprove").hide();
                    $("#imgLoader").show();
                    ret = true;
                }
                else { ret = false; }
            }
            else {
                alert("Please select a document to approve.");
            }
            return ret;
        }
        function Copy() {
            var ret = false;
            var Count = 0;
            var obj = $("#ContentPlaceHolder1_gvData_ctl00");
            var chkAll = $(obj).prop('checked');
            $("#ContentPlaceHolder1_gvData").find(':checkbox').each(function () {
                var IsChecked = $(this).prop('checked');
                if (IsChecked == true) {
                    Count = Count + 1;
                }
            });
            if (chkAll == true) {
                Count = Count - 1;
            }
            if (Count > 0) {
                ret = true;
            }
            else {
                alert("Please select a document to copy entered data.");
                ret = false;
            }
            return ret;
        }
        function Upolad() {
            var FileName = "";
            FileName = $("#ContentPlaceHolder1_FlUploader").val();
            var IsValid = true;
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (FileName == "") {
                ErrorMsg = ErrorMsg + "CSV file required.\n";
                IsValid = false;
            }
            else {
                var fileExtension = ['csv', 'CSV'];
                if ($.inArray(FileName.split('.').pop().toLowerCase(), fileExtension) == -1) {
                    ErrorMsg = ErrorMsg + "Only CSV file allowed.\n";
                    IsValid = false;
                }
            }
            //ContentPlaceHolder1_CSVUploader
            if (IsValid == false)
                alert(ErrorMsg);
            return IsValid;
        }
    </script>
   
    <script type="text/javascript">
        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }
        function ShowDialog(url) {
            // do some thing with currObj data

            var $dialog = $('<div></div>')
               .load(url)
               .dialog({
                   autoOpen: true,
                   title: 'Document Detail',
                   width: 700,
                   height: 550,
                   modal: true
               });
            return false;
        }
    </script>
    <div class="container-fluid container-fluid-amend">
        <div class="form">
            <div class="doc_header">
                Bulk Approval
            </div>

            <div class="row mg-top10">
                <div class="col-md-12  col-sm-12">
                    <div class="col-md-2 col-sm-2">
                        <label>Document Type:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlDocumentType" runat="server" AutoPostBack="true" CssClass="txtBox">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Work Flow Status:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlWF" runat="server" CssClass="txtBox" AutoPostBack="true">
                            <asp:ListItem Value="0">--Select Document First--</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btnNew mg-bottom" OnClientClick="javascript:return ValidateForm();" />
                    </div>
                </div>
            </div>
         
            <div class="row mg-top10">
                <div class="col-md-12  col-sm-12">
                    <div class="col-md-2 col-sm-2">
                        <label>Filter on:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlFieldName" runat="server" CssClass="txtBox" AutoPostBack="true">
                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>value:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlFormValue" runat="server" CssClass="txtBox">
                            <asp:ListItem Value="0" Text="--Select--">
                            </asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2"
                        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btnNew mg-bottom" OnClientClick="javascript:return DocumentFilter();" />
                    </div>
                </div>
            </div>

            <div class="row mg-top10" id="pnlUploader" runat="server" visible="false">
                <div class="col-md-12 col-sm-12">
                    <asp:Label ID="lblUpMsg" runat="server"></asp:Label>
                </div>
                <div class="col-md-12 col-sm-12">
                    <div class="col-md-2 col-sm-2">
                        <label>Select a File:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:FileUpload runat="server" ID="FlUploader" />
                    </div>
                    <div class="col-md-5 col-sm-5">
                        <asp:Button ID="btnSample" runat="server" CssClass="btnNew" Text="Download Sample" />
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btnNew" OnClientClick="javascript:return Upolad();" />
                    </div>
                </div>
            </div>

            <div class="row mg-top10">
                <div class="col-md-12 col-sm-12" >
                          <asp:Panel ID="pnlfieldTop" runat="server" Visible="false">
                        <div id="Div1" style="border-top: solid 2px #e1e1e1;">
                            <asp:UpdatePanel ID="UpDynamicFields" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="form" style="text-align: left">
                                        <table width="100%">
                                            <tr style="background-color: #e2e2e2; height: 30px; padding:5px 10px; display:block;">
                                                <td><b>Enter data for Bulk Approval with same value</b></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Panel ID="pnlFields" runat="server" Width="960px">
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right;">
                                                    <asp:Button ID="btnCopy" runat="server" Text="Copy To Selected Rows" OnClientClick="javascript:return Copy();" CssClass="btnNew mg-bottom" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </asp:Panel>
                </div>
            </div>

            <div class="row mg-top10">
                <div class="col-md-12 col-sm-12">
                    &nbsp;
                </div>
            </div>
            <div class="doc_header">
                <asp:Label ID="Label2" runat="server"></asp:Label>
                <asp:Label runat="server" ID="lblDataCount"></asp:Label>
            </div>
            <div class="row" id="divgvData" runat="server" visible="false">
                <div  class="col-md-12 col-sm-12">
                    <asp:UpdatePanel runat="server" ID="Up1">
                          <ContentTemplate>
                            <asp:Panel ID="pnlgvData" runat="server" Visible="true"  ScrollBars="Both" Width="1345px" Height="275px">
                                <asp:GridView ID="gvData" runat="server" Width="998px" DataKeyNames="DOCID" AllowPaging="false" OnRowDataBound="gvData_RowDataBind" OnRowCreated="gv_OnRowCreated" AutoGenerateColumns="true" CellPadding="2">
                                    <FooterStyle CssClass="FooterStyle" />
                                    <RowStyle CssClass="RowStyle" />
                                    <EditRowStyle CssClass="EditRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass=" HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                    <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                </asp:GridView>
                            </asp:Panel>
                        </ContentTemplate>
                       
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExport" />
                            <asp:PostBackTrigger ControlID="btnSample" />
                            <asp:PostBackTrigger ControlID="btnSubmit" />
                            <asp:PostBackTrigger ControlID="btnUpload" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="row" id="dvControl" runat="server" visible="false">
                <div class="col-md-10  col-sm-10">
                    &nbsp;
                </div>
                <div class="col-md-2 col-sm-2">
                    <div class="clear10"></div>
                    <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btnNew" />
                    <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClientClick="javascript:return Approve();" CssClass="btnNew" />
                    <img src="images/preloader22.gif" alt="Progress Bar" id="imgLoader" style="display: none;" />
                </div>
            </div>
            <div class="row mg-top10">
                <div class="col-md-12 col-sm-12">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <div id="main">
        <table cellpadding="0px" cellspacing="0px" width="100%" border="0">
            <tr>
                <td>
                    <asp:UpdatePanel ID="upData" runat="server">
                        <ContentTemplate>
                            <%--          <span><b>Total Number Of Record(s) </b></span>--%>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

