<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/usrFullScreenBPM.master" CodeFile="DocumentUploader.aspx.vb"
    Inherits="DocumentUploader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
            var formSource = "";
            FormName = $("#ContentPlaceHolder1_ddlFormtype").val();
            subFormName = $("#ContentPlaceHolder1_ddlAction").val();
            formSource = $("#ContentPlaceHolder1_ddlSource").val();
            var IsValid = true;
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (formSource == "0") {
                ErrorMsg = ErrorMsg + "Document Source required.\n";
                IsValid = false;
            }
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
            var formSource = "";
            var FieldValue = "";
            FormName = $("#ContentPlaceHolder1_ddlFormtype").val();
            FileName = $("#ContentPlaceHolder1_CSVUploader").val();
            formSource = $("#ContentPlaceHolder1_ddlSource").val();
            FieldValue = $("#ContentPlaceHolder1_hdnFileType").val();
            var IsValid = true;
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (formSource == "0") {
                ErrorMsg = ErrorMsg + "Document Source required.\n";
                IsValid = false;
            }
            if (FormName == "0") {
                ErrorMsg = ErrorMsg + "Document Type required.\n";
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

                if ($.inArray(FileName.split('.').pop().toLowerCase(), fileExtension) == -1) {
                    ErrorMsg = ErrorMsg + "A valid " + fileExtension.toString() + " file required.\n";
                    IsValid = false;
                }
            }
            //ContentPlaceHolder1_CSVUploader
            if (IsValid == false)
                alert(ErrorMsg);
            return IsValid;
        }
    </script>
<div class="container-fluid container-fluid-amend">
    <div class="form">
        <div class="doc_header">
            Upload Document
        </div>
        <asp:UpdatePanel ID="updateDocument" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>Document Source:</label>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:DropDownList ID="ddlSource" runat="server" AutoPostBack="true" CssClass="txtBox">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                            <asp:ListItem Value="1">Document</asp:ListItem>
                            <asp:ListItem Value="2">Master</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:DropDownList ID="ddlOperation" runat="server" CssClass="txtBox" Visible="false">
                            <asp:ListItem>CREATE</asp:ListItem>
                            <asp:ListItem>EDIT</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Document Type:</label>
                    </div>
                    <div class="col-md-4 col-sm-4">
                        <asp:DropDownList ID="ddlFormtype" runat="server" CssClass="txtBox" Width="200px" OnSelectedIndexChanged="ddlFormtype_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:ImageButton ID="imgCSVSample" runat="server" ImageUrl="~/images/Helpexp.png" Height="20px" Style="padding-top: 5px;" ToolTip="Download sample CSV." OnClientClick="javascript:return ValidateCSVDownload();" />
                        <asp:HiddenField ID="hdnFileType" runat="server" />
                        <asp:label ID="lblSeperator" Text=""  Font-Size="Small" Font-Bold="true" ForeColor="red" runat="server" > </asp:label>
                    </div>

                </div>
                <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>Select a file:</label>
                    </div>
                    <div class="col-md-4 col-sm-4">
                        <asp:FileUpload runat="server" ID="CSVUploader" CssClass="txtBox" ToolTip="Select a CSV file to upload." />
                    </div>
                    <div class="col-md-6 col-sm-6" style="text-align: center;">
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

                        <asp:Panel ID="pnlMessage" runat="server" BorderColor="#888" Visible="false">
                            <div class="output-grid" style="background-color: #888; color: #fff; font-weight: bold; border-radius: 3px; font-family: sans-serif; font-size: 14px; padding: 5px 10px; margin: 0px;">Output Result</div>
                            <div id="divData" style="border: 0; margin-top: 20px; overflow-x: scroll;">
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
                                            <asp:GridView ID="gvData" Width="100%" SelectedIndex="0" runat="server" AllowPaging="false" OnRowCreated="gv_OnRowCreated" AutoGenerateColumns="false" CellPadding="2">
                                                <FooterStyle CssClass="FooterStyle" />
                                                <RowStyle CssClass="RowStyle" />
                                                <EditRowStyle CssClass="EditRowStyle" />
                                                <SelectedRowStyle CssClass="SelectedRowStyle" />
                                                <PagerStyle CssClass="PagerStyle" />
                                                <HeaderStyle CssClass=" HeaderStyle" />
                                                <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No.">
                                                        <ItemTemplate>
                                                            <%#Container.DataItemIndex + 1 %>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="4%" Height="30px" HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Result">
                                                        <ItemTemplate>
                                                            <%#Eval("Service_response") %>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="96%" Height="30px" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
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
