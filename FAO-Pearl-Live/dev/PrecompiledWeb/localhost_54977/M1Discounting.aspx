<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="M1Discounting, App_Web_20pgc3v0" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" />
    <style type="text/css">
        .mg {
            margin: 10px;
        }
        .form-control[disabled], .form-control[readonly], fieldset[disabled] .form-control {
            background-color: rgba(158, 158, 158, 0.45);
        }
    </style>
    <script type="text/javascript">
        var docid;
        $(document).ready(function () {
            var tid = GetParameterValues('tid');
            docid = tid;
            alert('Your document has been successfully saved with docid ' + tid);
            ShowCurrentDocument(tid);
            function ShowCurrentDocument(tid) {
                $.ajax({
                    type: "POST",
                    url: "M1Discounting.aspx/GetDocumentDetail",
                    data: '{docid: "' + tid + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccess,
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
            $(".self").hide();
            function GetParameterValues(param) {
                var url = window.location.href.slice(window.location.href.indexOf("?") + 1).split('&');
                for (var i = 0; i < url.length; i++) {
                    var urlParam = url[i].split('=');
                    if (urlParam[0] == param) {
                        return urlParam[1];
                    }
                }
            }
            function OnSuccess(response) {
                $("#txtInvAmount").val(response.d["TOTALINVOICEAMOUNT"]);
                $("#txtInvoiceDate").val(response.d["INVOICEDATE"]);
                $("#txtVendorCode").val(response.d["VENDORCODE"]);
                var dtDropDownList = $('#ddlDiscountType');
                dtDropDownList.append($("<option></option>").val(0).html("SELECT"));
                $.each(response.d["LISTOFDISCOUNTTYPE"], function () {
                    dtDropDownList.append($("<option></option>").val(this["DISCOUNTTYPEID"]).html(this["DISCOUNTTYPE"]));
                });
            }
            $("#ddlDiscountType").change(function () {
                var index = $("#ddlDiscountType").prop('selectedIndex');

                if (index == 0) {
                    $(".self").hide();
                }
                else if (index == 1) {
                    $(".self").hide();
                }
                else {
                    $(".self").show();
                    $('#ddlDiscountRate').empty();
                    $("#txtIntrestedOn").val(0);
                    BindDiscountRate(tid);
                    NetPaidAmount();
                }
            });
            function BindDiscountRate(param) {
                var myDropDownList = $('#ddlDiscountRate');
                myDropDownList.append($("<option></option>").val(0).html("SELECT").attr("interest", 0).attr("days", 0));
                $.ajax({
                    type: "POST",
                    url: "M1Discounting.aspx/GetDiscountRateMaster",
                    data: '{docid: "' + tid + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $.each(data.d.ListOfDiscountingMaster, function (key, value) {
                            myDropDownList.append($("<option></option>").val(this['MASTERTID']).html(this['NAME']).attr("interest", this["DISCOUNTINGPERCENT"]).attr("days", this["PAYABLEDAYS"]));
                        });
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
            $("#ddlDiscountRate").change(function () {
                $("#txtIntrestedOn").val($("#ddlDiscountRate").find('option:selected').attr('interest'));
                $("#txtPayableDays").val($("#ddlDiscountRate").find('option:selected').attr('days'));
                NetPaidAmount();
            });

            function NetPaidAmount() {
                var InvAmount = Number($('#txtInvAmount').val());
                var ROI = Number($('#txtIntrestedOn').val());
                var Netpaid = ((InvAmount) - (InvAmount * ROI / 100));
                $("#txtNetpaidAmount").val(Netpaid.toFixed(2));
            }
        }
   );
        function AcceptDiscounting(Type) {
            var invoiceAmount = $("#txtInvAmount").val();
            var discountType = $("#ddlDiscountType").val();
            var disocuntRate = $("#ddlDiscountRate").val();
            var interest = $("#txtIntrestedOn").val();
            var netPaidAmount = $("#txtNetpaidAmount").val();
            var payabledays = $("#txtPayableDays").val();
            if (Type == "SAVE") {
                if (discountType == "SELECT") {
                    $("#ddlDiscountType").focus();
                    alert("Please select Discount Type");
                    return;
                }
                if ($("#ddlDiscountType").prop("selectedIndex") == 2 && disocuntRate == 0) {
                    $("#ddlDiscountRate").focus();
                    alert("Please select Discount Rate");
                    return;
                }
            }
            if (invoiceAmount == "") {
                invoiceAmount = 0;
            }
            if (disocuntRate == null) {
                disocuntRate = 0;
            }
            if (interest == "") {
                interest = 0;
            }
            if (netPaidAmount == "") {
                netPaidAmount = 0;
            }
            if (payabledays == "") {
                payabledays = 0;
            }
            

            $.ajax({
                type: "POST",
                url: "M1Discounting.aspx/SaveDiscounting",
                data: '{type: "' + Type + '",invoiceamount:"' + invoiceAmount + '",docid:"' + docid + '",disocuntRate:"' + disocuntRate + '",interest:"' + interest + '",netPaidAmount:"' + netPaidAmount + '",discountType:"' + discountType + '",payabledays:"'+ payabledays +'" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d["RESCODE"] == 404) {
                        alert(response.d["RESMESSAGE"]);
                        return;
                    }
                    else {
                        alert(response.d["RESMESSAGE"]);
                        window.location.href = response.d["RESURL"]

                    }
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
        }

    </script>
    <div class="container fluid">
        <div class="form">

            <div class="doc_header">
                Invoice  Discounting 
            </div>
            <div class="row mg">
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <label>Invoice Amount</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <input type="text" id="txtInvAmount" class="txtBox" readonly="readonly" />
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <label>Invoice Date</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <input type="text" id="txtInvoiceDate" class="txtBox" readonly="readonly" />
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <label>Vendor Code</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <input type="text" id="txtVendorCode" class="txtBox" readonly="readonly" />
                </div>
            </div>

            <div class="row mg">
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <label>Discount Type</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2">
                    <select id="ddlDiscountType" class="txtBox">
                    </select>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <label>Discount Rate</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <select id="ddlDiscountRate" class="txtBox">
                    </select>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <label>Payable Days</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <input type="text" id="txtPayableDays" class="txtBox form-control disabled" value="0" readonly="readonly" />
                </div>
            </div>
            <div class="row mg">
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <label>Interest On</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <input type="text" id="txtIntrestedOn" class="txtBox form-control disabled" readonly="readonly" />
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <label>Net Paid Amount</label>
                </div>
                <div class="col-xs-12 col-md-2 col-sm-2 self">
                    <input type="text" id="txtNetpaidAmount" class="txtBox form-control disabled" readonly="readonly" />
                </div>
            </div>
            <div class="pull-right">
                <button type="button" class="btn btn-primary submit" onclick="AcceptDiscounting('REJECT');" style="height: 30px; width: 100px;" value="Login"><i class="fa fa-times"></i>Reject</button>
                <button type="button" id="btnAccept" class="btn btn-success submit" onclick="AcceptDiscounting('SAVE');" style="height: 30px; width: 100px;" value="Login"><i class="fa fa-check"></i>Accept</button>

            </div>
        </div>

    </div>
</asp:Content>


