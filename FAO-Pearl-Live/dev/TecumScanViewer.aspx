<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/USR.master" CodeFile="TecumScanViewer.aspx.vb"
    Inherits="TecumScanViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>

    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
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
    
    <div style="width: 998px; border: solid 2px #e1e1e1; margin-top: 20px;">

        <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
            <tr style="background-color: #e2e2e2;">
                <td colspan="3"><b>View Barcode Scan</b></td>
            </tr>
            <tr>
                <td colspan="3">&nbsp;</td>
            </tr>
            <tr>
                <td width="10%"></td>
                <td width="80%">
                    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">                       
                       
                        <tr>
                            <td height="20px">

                            
                    <asp:Label runat="server" Text="Type Barcode No." ID="Label1"></asp:Label>
                                 <asp:TextBox ID="txtbarcode" runat="server" CssClass="txtBox"  Width="290px"></asp:TextBox>
                                <asp:Button runat="server" ID="btnSubmit" CssClass="btnNew" Text="View Barcode" ToolTip="Click to View" OnClick="OpenWindow"  />
                </td>
                  
                        </tr>

                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="3" height="20px">
                    <asp:Label runat="server" ID="lblMessage"></asp:Label>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
