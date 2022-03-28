<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="EPD_SUP_RC, App_Web_0gl03q5k" viewStateEncryptionMode="Always" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-fluid container-fluid-amend">
        <div class="form">
            <div class="doc_header">
                Early Payment Discounting Supplier Rate Card Master           
            </div>
            <%-- <div class="row">
                <div class="col-md-2 col-sm-2">
                    <label>Vendor Name</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:DropDownList ID="ddlVendorName" runat="server" CssClass="txtBox"></asp:DropDownList>
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>Rate Card Description</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox ID="txtRateCardDescription" runat="server" CssClass="txtBox"></asp:TextBox>
                </div>
                <div class="col-md-2 col-sm-2">
                    <label>Rate Of Interest</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox ID="txtROI" runat="server" CssClass="txtBox"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                  <div class="col-md-2 col-sm-2">
                    <label>Number Of Days</label>
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:TextBox ID="txtNOD" runat="server" CssClass="txtBox"></asp:TextBox>
                </div>
            </div>--%>
            <div class="row">
                <div class="col-md-2 col-sm-2">
                    <label>Select File</label>
                </div>
                <div class="col-md-4 col-sm-4">
                    <asp:FileUpload ID="fuMaster" runat="server" CssClass="txtBox" />
                </div>
                <div class="col-md-1 col-sm-1">
                    <asp:ImageButton ID="imgCSVSample" runat="server" ImageUrl="~/images/Helpexp.png" Height="20px" Style="padding-top: 5px;" ToolTip="Download sample CSV." OnClick="imgCSVSample_Click" />
                </div>
            </div>
            <div class="row" style="float: right;">
                <div class="col-md-12 col-sm-12 col-lg-12 col-xs-12">
                    <asp:Button ID="btnDownload" class="btn btn-primary submit" Style="height: 30px; width: 250px;" Text="Download Vendor Rate Card Master" OnClick="btnDownload_Click" runat="server" />
                    <asp:Button ID="btnClear" class="btn btn-success submit" Style="height: 30px; width: 100px;" Text="Clear" runat="server" />
                    <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary submit" ToolTip="Upload CSV file." OnClick="btnSubmit_Click" OnClientClick="javascript:return ValidateForm();" Style="height: 30px; width: 100px;" Text="Save" />
                 </div>
            </div>
            <div class="row">
                <asp:Label ID ="errorMsg" runat="server"></asp:Label>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function ValidateForm() {
            var FileName = "";
            FileName = $("#ContentPlaceHolder1_fuMaster").val();
            var IsValid = true;
            var fileExtension = "";
            fileExtension = ['csv', 'CSV'];
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (FileName == "") {
                ErrorMsg = ErrorMsg + 'File Required';
                IsValid = false
            }
            if ($.inArray(FileName.split('.').pop().toLowerCase(), fileExtension) == -1) {
                ErrorMsg = ErrorMsg + "A valid " + fileExtension.toString() + " file required.\n";
                IsValid = false;
            }
            if (IsValid == false) {
                alert(ErrorMsg);
                return IsValid;
            }
        }
    </script>
</asp:Content>

