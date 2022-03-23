<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" CodeFile="DocumentwithchildUploaderDraft.aspx.vb" Inherits="DocumentwithchildUploaderDraft" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .mg {
            margin: 10px 0px;
        }

        .errormsg {
            text-align: center;
            display: block;
            font-family: serif;
            font-size: 16px;
            color: #f90309;
            font-weight: bold;
            line-height: 25px;
        }
    </style>
        <script type="text/javascript" language="javascript">
        function ValidateCSVDownload() {
            var FormName = "";
            var subFormName = "";
            FormName = $("#ContentPlaceHolder1_ddlFormtype").val();
            subFormName = $("#ContentPlaceHolder1_ddlAction").val();
            var IsValid = true;
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (FormName == "0") {
                ErrorMsg = ErrorMsg + "Document Type required.\n";
                IsValid = false;
            }
            if (subFormName == "0") {
                ErrorMsg = ErrorMsg + "Sub Document Type required.\n";
                IsValid = false;
            }
            if (IsValid == false)
                alert(ErrorMsg);
            return IsValid;
        }
        function ValidateForm() {
            var FormName = "";
            var FileName = "";
            var FieldValue = "";
            var OperationType = "";
            FormName = $("#ContentPlaceHolder1_ddlFormtype").val();
            FileName = $("#ContentPlaceHolder1_CSVUploader").val();
            FieldValue = $("#ContentPlaceHolder1_hdnFileType").val();
            OperationType = $("#ContentPlaceHolder1_ddlOperation").val();
            var IsValid = true;
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (FormName == "0") {
                ErrorMsg = ErrorMsg + "Document Type required.\n";
                IsValid = false;
            }
            if (OperationType == "SELECT") {
                ErrorMsg = ErrorMsg + "Operation Type required \n";
                IsValid = false;
            }
            if (FileName == "") {
                ErrorMsg = ErrorMsg + "CSV file required.\n";
                IsValid = false;
            }
            else {
                var fileExtension = "";
                if (FieldValue.toUpperCase() == "XML") {
                    fileExtension = ['xml', 'XML'];
                }
                else if (FieldValue.toUpperCase() == "CSV") {
                    fileExtension = ['csv', 'CSV'];
                }
                else if (FieldValue.toUpperCase() == "BOTH") {
                    fileExtension = ['csv', 'CSV', 'xml', 'XML'];
                }
                else if (FieldValue.toUpperCase() == "ZIPWITHCSV") {
                    fileExtension = ['zip', 'ZIP'];
                }
                //if ($.inArray(FileName.split('.').pop().toLowerCase(), fileExtension) == -1) {
                //    ErrorMsg = ErrorMsg + "A valid " + fileExtension.toString() + " file required.\n";
                //    IsValid = false;
                //}
            }
            //ContentPlaceHolder1_CSVUploader
            if (IsValid == false)
                //Change the value after testing
                alert(ErrorMsg);
            return IsValid;
        }
    </script>
<div class="container-fluid container-fluid-amend">
    <div class="form">
        <div class="doc_header">
            <label>Upload Document In Draft</label>
        </div>
        <asp:UpdatePanel ID="updateDocument" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

                <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>Document Type:</label>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:DropDownList ID="ddlFormtype" runat="server" CssClass="txtBox" OnSelectedIndexChanged="ddlFormtype_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>

                        <asp:HiddenField ID="hdnFileType" runat="server" />
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <asp:ImageButton ID="imgCSVSample" runat="server" ImageUrl="~/images/Helpexp.png" Height="20px" Style="padding-top: 5px;" ToolTip="Download sample CSV." OnClientClick="javascript:return ValidateCSVDownload();" />
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <label>operation:</label>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:DropDownList ID="ddlOperation" runat="server" CssClass="txtBox">
                            <asp:ListItem Selected="True">SELECT</asp:ListItem>
                            <asp:ListItem>CREATE</asp:ListItem>
                            <asp:ListItem>UPDATE</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <label>Select a file:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:FileUpload runat="server" ID="CSVUploader" CssClass="txtBox" ToolTip="Select a CSV file to upload." />
                    </div>

                </div>
                <div class="row mg">
                    <div class="col-md-12 col-sm-12">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btnNew" Text="Upload File" ToolTip="Upload CSV file." OnClientClick="javascript:return ValidateForm();" />
                    </div>
                </div>
                <div class="row mg">
                    <div class="col-md-12 col-sm-12">
                        <asp:Label runat="server" ID="lblMessage" CssClass="errormsg"></asp:Label>
                    </div>
                </div>
                <div class="row mg">
                    <div class="col-md-12 col-sm-12">
                        <asp:GridView ID="gvResult" runat="server" Style="display: none;">
                        </asp:GridView>
                    </div>
                </div>
                <div class="row mg">
                    <div class="col-md-12 col-sm-12">
                        <asp:Panel ID="pnlMessage" runat="server" BorderColor="#888" Visible="false">
                            <div class="output-grid" style="background-color: #888; color: #fff; font-weight: bold; border-radius: 3px; font-family: sans-serif; font-size: 14px; padding: 5px 10px; margin: 0px;">Output Result</div>
                            <div class="row" style="border: 1px solid #ccc; padding: 0px; margin: 0px; line-height: 30px;">
                                <div class="col-md-2 col-sm-2" style="padding: 0px 20px; margin: 0px;">
                                    <label>Total Documents :</label>
                                </div>
                                <div class="col-md-1 col-sm-1" style="padding: 0px; margin: 0px;">
                                    <asp:Label ID="lblTotalDocument" runat="server"></asp:Label>
                                </div>
                                <div class="col-md-3 col-sm-3" style="padding: 0px; margin: 0px;">
                                    <label>Successfull uploaded Document :</label>
                                </div>
                                <div class="col-md-1 col-sm-1" style="padding: 0px; margin: 0px;">
                                    <asp:Label ID="lblSuccessDocument" runat="server"></asp:Label>
                                </div>
                                <div class="col-md-1 col-sm-1" style="padding: 0px; margin: 0px;">
                                    <label>Failed Document :</label>
                                </div>
                                <div class="col-md-1 col-sm-1" style="padding: 0px; margin: 0px;">
                                    <asp:Label ID="lblFailedDocument" runat="server"></asp:Label>
                                </div>
                                <div class="col-md-2 col-sm-2" style="padding: 0px; margin: 0px;">
                                    <label>Export Error Document :</label>
                                </div>
                                <div class="col-md-1 col-sm-1" style="padding: 0px; margin: 6px 0px;">
                                    <asp:ImageButton ID="imgExport" runat="server" ImageUrl="~/images/excel.png" Width="20" Height="20" OnClick="imgExport_Click" />
                                </div>
                                <div class="row mg">
                                    <div class="col-md-12 col-sm-12">
                                        <asp:GridView ID="gvData" SelectedIndex="0" runat="server" Width="100%" AllowPaging="false" AutoGenerateColumns="false" CellPadding="2">
                                            <FooterStyle CssClass="FooterStyle" />
                                            <RowStyle CssClass="RowStyle" />
                                            <EditRowStyle CssClass="EditRowStyle" />
                                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                                            <PagerStyle CssClass="PagerStyle" />
                                            <HeaderStyle CssClass=" HeaderStyle" />
                                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                            <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Doc Number">
                                                    <ItemTemplate>
                                                        <%#Eval("DocNumber")%>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="4%" Height="30px" HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Result">
                                                    <ItemTemplate>
                                                        <%#Eval("Response")%>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="96%" Height="30px" HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="imgCSVSample" />
                <asp:PostBackTrigger ControlID="btnSubmit" />
                <asp:PostBackTrigger ControlID="imgExport" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</div>
</asp:Content>

