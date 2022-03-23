<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DocDetail.aspx.vb" EnableEventValidation="false"
    Inherits="DocDetail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Document Detail</title>
   <%-- <link href="css/style.css" rel="Stylesheet" type="text/css" />
    <link href="css/ui-lightness/jquery-ui-1.10.2.custom.css" type="text/css" rel="stylesheet" />
    <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/jquery.slidePanel.min.js"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <link href="css/DateTimePicker.css" rel="stylesheet" />
    <script src="js/DateTimePicker.js"></script>
    <script src="js/Utils.js"></script>
    <style type="text/css">--%>
     <meta http-equiv="X-UA-Compatible" content="IE=10,chrome=1" />
    
  <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/css/select2.min.css" rel="stylesheet" />
    <link href="css/style.css" rel="Stylesheet" type="text/css" />
   <link href="css/ui-lightness/jquery-ui-1.10.2.custom.css" type="text/css" rel="stylesheet" />
    <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <link href="css/TabStyleSheet.css" rel="stylesheet" type="text/css" />
     <script type="text/javascript" src="scripts/jquery.slidePanel.min.js"></script>
    <script src="js/Utils.js" type="text/javascript"></script>
    <script src="js/TicketUtils.js" type="text/javascript"></script>
    <link href="css/DateTimePicker.css" rel="stylesheet" />
    <script src="js/DateTimePicker.js" type="text/javascript"></script>
    <script src="js/Canara/CanaraScript.js" type="text/javascript"></script>
    <script src="js/CanaraHSBC/CanaraHSBCScript.js" type="text/javascript"></script>
    <script src="js/FreeCharge/FreeChargeScript.js" type="text/javascript"></script>
    
    
    <style type="text/css">
        /*.classEdit {
            min-width: 1000px;
            min-height: 630px;
        } */
        #tabPending0 {
            min-height: 300px;
            overflow-x: scroll;
            overflow-y: scroll;
            max-width: 1165px;
            
        }
        .select2-dropdown { z-index:999999!important;}
    </style>

     <script type="text/javascript">
         //$(document).ready(function () {
         //    $(".DCB").click(function () {
         //        var txtfileupload = $(this).attr("attrfileupid");
         //        var lblidf = $(this).attr("attrlblid");
         //        var arrhdnid = $(this).attr("attrhdnid");
         //       // var data = $(this).prev().find("#" + hdnid).html();
         //        //alert(lblidf + txtfileupload + arrhdnid);
         //        $("#ContentPlaceHolder1_" + txtfileupload).val('');
         //        $("#" + lblidf).text("");
         //        $("#" + arrhdnid).val('');
         //        return;
         //    });
         //});
         function ClearFields(MainfileId, fileuploadid, hdnfldid) {
             debugger
             var A = $("#ContentPlaceHolder1_" + MainfileId).val('');
             var V = $("#" + fileuploadid).text("");
             var C = $("#" + hdnfldid).val('');
         }
    </script>

    <script type="text/javascript">
        function CalWithCalendar() {
            var dates = new Date();
            var currentDate = dates.getDate() + '/' + (dates.getMonth() + 1) + '/' + dates.getFullYear().toString().substring(2, 4);
            var dueDate = $("#txtDueDate").val();
            var paymentDate = $("#txtPaymentDate").val();
            if ($.datepicker.parseDate('dd/mm/yy', paymentDate) < $.datepicker.parseDate('dd/mm/yy', currentDate)) {
                $("#txtPaymentDate").val('');
                alert('Payment date should be future date');
            }
            var NodaysDiff = Math.round(($.datepicker.parseDate('dd/mm/yy', dueDate) - $.datepicker.parseDate('dd/mm/yy', paymentDate)) / (1000 * 60 * 60 * 24));
            if ($.datepicker.parseDate('dd/mm/yy', paymentDate) > $.datepicker.parseDate('dd/mm/yy', dueDate)) {
                $("#txtPaymentDate").val('');
                alert('Payment Date (' + paymentDate + ') is greater than Due Date (' + dueDate + ')');
            }
            if (NodaysDiff < 12) {
                if ($("#ddlDiscountingType").val() == "M1" || $("#ddlDiscountingType").val() == "M2") {
                    $("#txtPaymentDate").val('');
                    alert('Payment Date should be greater than 11 days to process in M1');
                }
            }
        }


        $(document).ready(function () {
            $($("form")[0]).on("keypress", function (e) {
                var keycode = (e.keyCode ? e.keyCode : e.which);
                if (keycode == '13') {
                    if ($(e.target).prop("type") === "textarea") {
                        return;
                    }
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            });
        });



        function netpaidamount() {
            debugger;
            var roi = Number($("#txtROI").val());
            var nod = Number($("#txtNOD").val());
            var amount = Number($("#txtPaidAmount").val());
            var netpaidAmount = 0;
            var marginmoney = Number($("#txtMarginMoney").val());
            if (marginmoney == "") {
                $("#txtMarginMoney").val('0')
                marginmoney = 0;
                netpaidAmount = ((amount) - (amount * roi / 100));
            }
            else {
                //(netpaidAmount) - (netpaidAmount * marginmoney / 100)
                var partialAmount = Number(amount - (amount * marginmoney / 100));
                netpaidAmount = (partialAmount - (partialAmount * roi / 100));
            }
            $("#txtnetPaidAmount").val(netpaidAmount.toFixed(2));
        }
        function discountratelist() {
            if ($("#ddlDiscounting").val() == "SELECT") {
                $("#txtROI").val('0');
                $("#txtNOD").val('0');
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "DocDetail.aspx/DiscountRateList",
                    data: '{ratecardID: "' + $("#ddlDiscounting").val() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $("#txtROI").val(response.d["ROI"]);
                        $("#txtNOD").val(response.d["Days"]);
                        var roi = Number($("#txtROI").val());
                        var nod = Number($("#txtNOD").val());
                        var amount = Number($("#txtPaidAmount").val());
                        var netpaidAmount = ((amount) - (amount * roi / 100));
                        var marginmoney = Number($("#txtMarginMoney").val());
                        if (marginmoney == "") {
                            $("#txtMarginMoney").val('0')
                            marginmoney = 0;
                            netpaidAmount = ((netpaidAmount) - (netpaidAmount * marginmoney / 100));
                        }
                        else {
                            netpaidAmount = ((netpaidAmount) - (netpaidAmount * marginmoney / 100));
                        }
                        $("#txtnetPaidAmount").val(netpaidAmount.toFixed(2));
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
        }
        function ValidateDate() {
            var dueDate = $("#txtDueDate").val();
            var paymentDate = $("#txtPaymentDate").val();
            var nod = Number($("#txtNOD").val());
            var NodaysDiff = Math.round(($.datepicker.parseDate('dd/mm/yy', dueDate) - $.datepicker.parseDate('dd/mm/yy', paymentDate)) / (1000 * 60 * 60 * 24));
            if (nod < NodaysDiff) {
                alert('Payment Date is not falling in Selected Discount Slab!');
                return false;
            }
            else {
                return true;
            }
        }

        function AcceptDiscounting(type) {
            var discountingtypeData = $("#ddlDiscountingType").val();
            var discountingratelistData = $("#ddlDiscounting").val();
            var roiData = $("#txtROI").val();
            var daysData = $("#txtNOD").val();
            var marginmoneyData = $("#txtMarginMoney").val();
            var netpaidamountData = $("#txtnetPaidAmount").val();
            var invoiceAmount = $("#txtPaidAmount").val();
            var varPaymentDate = $("#txtPaymentDate").val();
            var varDueDate = $("#txtDueDate").val();
            if (type == "ACCEPT") {
                if (discountingtypeData == "SELECT") {
                    alert("Please select Discount Type");
                    $("#ddlDiscountingType").focus();
                    return;
                }
                else {
                    if (discountingratelistData == "SELECT") {
                        $("#ddlDiscounting").focus();
                        alert("Please select One of Slabs from List");
                        return;
                    }
                    if (marginmoneyData == "") {
                        $("#txtMarginMoney").focus();
                        alert("Please enter margin money amount");
                        return;
                    }
                    if (varPaymentDate == "") {
                        $("#txtPaymentDate").focus();
                        alert("Please enter payment data from calendar");
                        return;
                    }
                    if (varDueDate == "") {
                        $("#txtDueDate").focus();
                        alert("Please contact Admin due date is invalid or blank");
                        return;
                    }

                    //'Add condition for check validation on date'
                    if (!ValidateDate()) {
                        return;
                    };

                }

                //if (discountingtypeData == "SELF")

                $.ajax({
                    type: "POST",
                    url: "DocDetail.aspx/SaveDiscounting",
                    data: '{type: "' + type + '",ratecardid:"' + discountingratelistData + '",marginmoney:"' + marginmoneyData + '",netPaidAmount:"' + netpaidamountData + '",discountingtype:"' + discountingtypeData + '",docid:"' + $("#hdnDocID").val() + '",paymentDate:"' + varPaymentDate + '",dueDate:"' + varDueDate + '",partialPaidAmount:"' + invoiceAmount + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d["resCode"] == "200") {
                            $("#btnCloseDiscounting").click();
                            alert(response.d["resString"]);
                            window.location.reload();
                        }
                        else {
                            alert(response.d["resString"]);
                        }

                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });

            }
            else {

                //if (DeleteConfirm()) {

                $.ajax({
                    type: "POST",
                    url: "DocDetail.aspx/SaveDiscounting",
                    data: '{type: "' + type + '",ratecardid:"' + discountingratelistData + '",marginmoney:"' + marginmoneyData + '",netPaidAmount:"' + netpaidamountData + '",discountingtype:"' + discountingtypeData + '",docid:"' + $("#hdnDocID").val() + '",paymentDate:"' + varPaymentDate + '",dueDate:"' + varDueDate + '",partialPaidAmount:"' + invoiceAmount + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d["resCode"] == "200") {
                            $("#btnCloseDiscounting").click();
                            alert(response.d["resString"]);
                            window.location.reload();
                        }
                        else {
                            alert(response.d["resString"]);
                        }

                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
                //}
            }
        }

        function discounttype() {
            if ($("#ddlDiscountingType").val() == "SELF") {
                $(".self").css('visibility', 'visible');

                //$(".self").show();
            }
            else {
                //$(".self").css('visibility', 'hidden');
                $("#ddlDiscounting").prop('selectedIndex', 0);
                //$("#txtROI").val('0');
                //$("#txtNOD").val('0');
                //$("#txtMarginMoney").val('0');
                //$("#txtnetPaidAmount").val('0');

            }
        }
    </script>
     <script type="text/javascript">

         function performCalc(docid, EidPage) {

             var randomnumber = Math.floor((Math.random() * 100) + 1);
             
             if (EidPage.includes(".msg")) {
                 window.open('viewpdf.aspx?docid=' + docid + '&EidPage=' + EidPage + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');
             }
             else {
                 window.open('DOCS/' + EidPage + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');
             }
         }
     </script>

     <script type="text/javascript">

         function performCalcChild(PageUrl) {

             var randomnumber = Math.floor((Math.random() * 100) + 1);
             window.open('DOCS/' + PageUrl + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');

             //tblChildItem.Append("<td align=""left"" class=""bootstraptd""><input type=""button"" value=""View Attachment"" onclick=""Javascript: return window.open('DOCS/" & dtItem.Rows(iRow - 1).Item(iColumn).ToString() & "', 'CustomPopUp', 'width=600, height=600, menubar=no, resizable=yes');"" /></td>")

         }
    </script>

    <script>
        function performCalcNEwDocdetail(docid) {


            var randomnumber = Math.floor((Math.random() * 100) + 1);
            window.open('DocDetail.aspx?docid=' + docid + '', "_blank", 'PopUp' + randomnumber + ',scrollbars=1,menubar=0,resizable=1,width=850,height=500');
        }
    </script>

    <%--07- Jan-2017 added--%>

    <%--07- Jan-2017 added--%>
    <style type="text/css">
        .bootstraptd {
            padding: 1px !important;
        }

        .nav > li > a:hover {
            background: #fafafa !important;
        }
    </style>

    <%--07- Jan-2017 added--%>
    <script type="text/javascript">
        function ace_itemSelected(sender, e) {

            var test = e.get_value();
            var g1 = sender.get_element()
            var t1 = g1.id;
            var hdnID = t1.replace("fld", "HDN");
            document.getElementById(hdnID).value = test;
            //            alert(document.getElementById(hdnID).value);
        }

        function onDataShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 1000001;
            // sender._popupBehavior._element.style.left = "54px"; //set positions according to your requriment.
            // sender._popupBehavior._element.style.top = "50px"; //set top postion accorind to you requirement.

            //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
        }

        function ValidateSMS() {
            //txtMeaasge txtMob
            var Message = "", MobNum = "";
            Message = $("#txtMeaasge").val();
            MobNum = $("#txtMob").val();
            var obj = ""
            var Isvalid = true;
            var ErrorMessage = "Error(s) in your submission.\n-----------------------------------\n";
            if (Message.trim() == "") {
                ErrorMessage = ErrorMessage + "Message required.\n";
                Isvalid = false;
                obj = $("#txtMeaasge");
            }
            if (MobNum.trim() == "") {
                ErrorMessage = ErrorMessage + "Mobile number required.\n";
                Isvalid = false;
                if (obj != "") {
                    obj = $("#txtMob");
                }
            }
            else {
                if (MobNum.trim().length < 10) {
                    ErrorMessage = ErrorMessage + "A valid mobile number required.\n";
                    Isvalid = false;
                    if (obj != "") {
                        obj = $("#txtMob");
                    }
                }
            }
            if (Isvalid == false) {
                obj.focus();
                alert(ErrorMessage);
            }
            return Isvalid;
        }
        function numbersonly(e) {
            var unicode = e.charCode ? e.charCode : e.keyCode
            if (unicode != 8) { //if the key isn't the backspace key (which we should allow)
                if (unicode < 48 || unicode > 57) //if not a number
                    if (unicode != 46)
                        return false //disable key press
            }
        }
        function ValidateCRMMail() {
            //txtBody txtCC txtToMail txtSMTP txtPassword ddlFromMail
            var MailBoby = "", CC = "", ToMail = "", SMTP = "", Password = "", FromMail = "";
            MailBoby = $("#txtBody").val();
            ToMail = $("#txtToMail").val();
            Password = $("#txtPassword").val();
            FromMail = $("#ddlFromMail").val();
            SMTP = $("#txtSMTP").val();
            var obj = ""
            var Isvalid = true;
            var ErrorMessage = "Error(s) in your submission.\n-----------------------------------\n";
            if (FromMail != "0") {
                if (Password.trim() == "") {
                    Isvalid = false;
                    ErrorMessage = ErrorMessage + "Password required\n";
                    if (obj != "") {
                        obj = $("#txtPassword");
                    }
                }
                if (Password.trim() == "") {
                    ErrorMessage = ErrorMessage + "SMTP Serve required\n";
                    Isvalid = false;
                    if (obj != "") {
                        obj = $("#txtSMTP");
                    }
                }
            }
            if (MailBoby.trim() == "") {
                ErrorMessage = ErrorMessage + "Mail Body required\n";
                Isvalid = false;
                if (obj != "") {
                    obj = $("#txtBody");
                }
            }
            if (ToMail.trim() == "") {
                ErrorMessage = ErrorMessage + "ToMail Body required\n";
                Isvalid = false;
                if (obj != "") {
                    obj = $("#txtToMail");
                }
            }
            if (Isvalid == false) {
                alert(ErrorMessage);
                // obj.focus();
            }
            return Isvalid;
        }
        function TomailChange(fromMail) {
            //txtPassword txtSMTP
            if (fromMail == "0") {
                $("#txtPassword").attr("readonly", true).val("");
                $("#txtSMTP").attr("readonly", true).val("");
            }
            else {
                $("#txtPassword").attr("readonly", false);
                $("#txtSMTP").attr("readonly", false);
            }
        }

        $(document).ready(function () {
            $("#dtBox").DateTimePicker();
            var objgrid = $("table.invisible");
            objgrid.each(function () {
                var parent = $(this).parent().parent().parent();
                parent.hide();
                var ancher = $(parent).attr("id");
                var li = $('a[href="#' + ancher + '"]').parent();//$("a").find("[href='#" + ancher + "']").parent();
                li.hide();
            });

            var objgrid1 = $("table.mGrid");
            objgrid1.each(function () {
                debugger
                var parent = $(this).parent().parent().parent();
                parent.show();
                var ancher = $(parent).attr("id");
                var li = $('a[href="#' + ancher + '"]').parent();//$("a").find("[href='#" + ancher + "']").parent();
                li.show();
            });

        });
        var selTab;
        $(function () {

            var tabs = $("#tabs").tabs({
                show: function () {

                    selTab = $('#tabs').tabs('option', 'selected');
                }
            });

        });;
        var docselTab;
        $(function () {
            var tabs = $("#doctabs").tabs({
                show: function () {

                    docselTab = $('#doctabs').tabs('option', 'selected');
                }
            });

        });;

        $(function () {
            $(".btnDyn").button()

        });


        function bindDateTime() {
            //alert("This is partial page load");
            $("input[data-field='date'], input[data-field='time'], input[data-field='datetime']").each(function () {
                $(this).unbind("click");
                //                 alert("This is ID of my concern element." + $(this).attr("id"));
            });
            $("#dtBox").DateTimePicker();

            //$("#dtBoxC").DateTimePicker();
            //            alert("hv done with ashignment");
        }





        //function pageLoad(sender, args) {
        //    // alert("Hi This is page load");
        //    if (args.get_isPartialLoad()) {
        //        $("#tabs").tabs({
        //            show: function () {
        //                //get the selected tab index on partial postback  
        //                selTab = $('#tabs').tabs('option', 'selected');
        //            }, selected: selTab
        //        });

        //        $(function () {
        //            $(".btnDyn").button()
        //        });
        //    }

        //};


        $(document).ready(function () {
            $("#loading-div-background").css({ opacity: "0.8", display: "none" });

            //$("#loading-div-background").css({  });
        });
    </script>
    <script type='text/javascript'>
        function pageLoad(sender, args) { // this gets fired when the UpdatePanel.Update() completes
            $("select").not('.invisible').select2({
            });

            $("input.txtNumber,input.txtHeaderNumber").keydown(function (e) {
                $("#lblDetail1").text("");
                var key = e.which || e.keyCode;
                var ctrl = e.ctrlKey ? e.ctrlKey : ((key === 17) ? true : false);

                if (($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 109, 189, 109, 189]) !== -1 ||
                    (e.keyCode == 65 && e.ctrlKey === true) || (key == 86 && ctrl) || (key == 67 && ctrl) ||
                    (e.keyCode >= 35 && e.keyCode <= 39) || (e.keyCode >= 96 && e.keyCode <= 105))) {
                    var number = this.value.split('.');
                    if (number.length > 1 && (key == 190 || key == 110)) {
                        $("#lblDetail1").text("Number field can accept only one decimal point.");
                        return false;
                    }
                    return;
                }

                var charValue = String.fromCharCode(e.keyCode)
                    , valid = /^[0-9\.]/.test(charValue);

                if (!valid) {
                    var columnName = "";
                    if ($(this).hasClass('txtHeaderNumber')) {
                        columnName = $(this).closest('td').prev('td').find('span').text()
                        $("#lblDetail1").text(columnName + " accept only numeric digit.");
                    }
                    else {
                        var col = $(this).parent('td').index();
                        var row = $(this).parent('td').closest('tr').index();
                        columnName = $(this).closest('table').find("th:eq(" + col + ")").text();
                        $("#lblDetail1").text(columnName + " at line no " + row + " accept only numeric digit.");
                    }

                    e.preventDefault();
                }

            }).blur(function (e) {
                this.value = this.value.replace(/[^-?0-9\.]/g, '');
                if (this.value == ".") { this.value = 0 }
            });

            if (args.get_isPartialLoad()) {
                $("div#pnlApprove").attr('style', 'width: 1065px !important');
                $("div#pnlApprove div.form").attr('style', 'min-height: 0px !important');
                $("#tabs").tabs({
                    show: function () {
                        //get the selected tab index on partial postback  
                        selTab = $('#tabs').tabs('option', 'selected');
                    }, selected: selTab
                });

                $(function () {
                    $(".btnDyn").button()
                });

                var objInv = $(".invisible");
                objInv.each(function () {
                    var tr = $(this).parent().parent();
                    var inv = $(".invisible", tr);

                    //var img= inv.find("img")
                    var invImg = $(inv).parent("td");
                    var alltd = tr.find("td");
                    if (invImg.length == alltd.length) {
                        tr.addClass("invisible");
                        //img.addClass("invisible");
                    }
                    else {
                        tr.removeClass("invisible");
                    }
                    //onjInv Each function Ende Here
                });
                var objgrid = $("table.invisible");
                objgrid.each(function () {
                    var parent = $(this).parent().parent().parent();
                    parent.hide();
                    var ancher = $(parent).attr("id");
                    var li = $('a[href="#' + ancher + '"]').parent();//$("a").find("[href='#" + ancher + "']").parent();
                    li.hide();
                });
                var objgrid1 = $("table.mGrid");
                objgrid1.each(function () {
                    var parent = $(this).parent().parent().parent();
                    parent.show();
                    var ancher = $(parent).attr("id");
                    var li = $('a[href="#' + ancher + '"]').parent();//$("a").find("[href='#" + ancher + "']").parent();
                    li.show();
                });



            }

            //     ReBindMyStuff();
            //     window.alert('partial postback');

            //' for Hide all content'
            var isExpand = true;
            $("#t1 .row-fluid").each(function () {
                $(this).find(".portlet-title .tools a").each(function () {
                    if (isExpand == true) { $(this).removeClass("expand"); $(this).addClass("collapse"); } else { $(this).removeClass("collapse"); $(this).addClass("expand"); isExpand = false; }
                });
                //    ' for Hide all content'
                $(this).find(".portlet-body").each(function () {
                    if (isExpand == true) { $(this).css("display", "block"); } else { $(this).css("display", "none"); isExpand = false; }
                });
                isExpand = false;
            });



            $(function () {
                $(".btnDyn").button()
            });
            $(document).ready(function () {
                $("#tabs").tabs();
            });
            $(function () {
                $("#tab").tabs();
            });
            $(function () {
                $("#tabss").tabs();
            });
        }
        //Wiriten By Ajeet Kumar: Dated :18-Nov-2015
        //It is used to invock method of parent page:
        window.onunload = function () {
            var win = window.opener;
            if (!win.closed) {
                win.childClose();
            }
        };


        function GetParameterValues(param) {
            var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < url.length; i++) {
                var urlparam = url[i].split('=');
                if (urlparam[0] == param) {
                    return urlparam[1];
                }
            }
        }
        var formtabSelDiv;
        var formTabIndex;
        var count = 1;
        $(document).ready(function () {
            // We Will hide all div initially then conditonal based show divs
            $(".maindivTabs").each(function (index) {
                if (formtabSelDiv == undefined) {
                    $("#" + $(this).attr('id')).show();
                    formtabSelDiv = $(this).attr('id');
                    $("#hdnCurrentMainTab").val(index + 1);
                }
                else {
                    $("#" + $(this).attr('id')).hide();
                }
            });
            var CurrentSelectedIndex = $("#hdnCurrentMainTab.ClientID").val();
            $(".formtab").each(function (indexes) {
                if (indexes + 1 == CurrentSelectedIndex) {
                    $(this).addClass('done');
                }
                else {
                    $(this).removeClass('done');
                }
               <%-- alert('hii' + $("#" + '<%= hdnCurrentMainTab.ClientID %>').val());
                alert(indexes);
                if (formtabSelDiv == undefined) {
                    $(this).addClass('done');
                    formTabIndex = 1;
                }
                else {
                    $(this).removeClass('done');
                }--%>
                //count++;
            });


        });

        function checkAll(GID) {
            //var obj = $("#ContentPlaceHolder1_gvData_ctl00").attr('checked');
            //alert(GID);
            //var GridIDCHK = "#ContentPlaceHolder1_GRD" + GID + "_ctl00";
            var GridIDCHK = "#GRD" + GID + "_ctl00";

            var obj = $(GridIDCHK);
            // var GridID = "#ContentPlaceHolder1_GRD" + GID;
            var GridID = "#GRD" + GID;
            //alert("Hi I Am being fired");
            //alert("Object Is" + obj);
            var str = $(obj).prop('checked');
            //alert("Property IS " + str);
            // alert(str);
            if (str == true) {
                $(GridID).find(':checkbox').each(function () {
                    $(this).prop('checked', "checked");
                });
            }
            else {
                $(GridID).find(':checkbox').each(function () {
                    $(this).removeAttr('checked');
                });
            }
        }


    </script>


    <%--for tab (doc detail fields panel) show and hide functionality --%>
    <script type="text/javascript">
        $(document).ready(function () {
            PopulateTabObjects();
        });
        var tabobjects = "";
        var arrtabobjects = [];
        function beforeAsyncPostBack() {
            PopulateTabObjects();
        }
        function beforeAsyncPostBack() {
            PopulateTabObjects();
        }

        function afterAsyncPostBack() {
            for (var i = 0; i < arrtabobjects.length; i++) {
                var aobj = $("#" + arrtabobjects[i].element);
                //is(b)
                if (aobj.length > 0) {
                    var parent = $(aobj).parent().parent().next();
                    if (arrtabobjects[i].cssclass == "collapse") {
                        aobj.addClass("collapse");
                        $(parent).show();
                    }
                    else {
                        aobj.addClass("expand");
                        $(parent).hide();
                    }
                }
            }
        }


        function appl_init() {
            var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
            pgRegMgr.add_beginRequest(BeginHandler);
            pgRegMgr.add_endRequest(EndHandler);

        }

        Sys.Application.add_init(appl_init);
        function BeginHandler() {
            beforeAsyncPostBack();
        }

        function EndHandler() {
            afterAsyncPostBack();

        }

        function PopulateTabObjects() {
            $("a.expand").each(function (index) {
                tabobjects = { cssclass: "expand", element: $(this).attr("id") };
                arrtabobjects.push(tabobjects);
            });
            //collapse
            $("a.collapse").each(function (index) {
                tabobjects = { cssclass: "collapse", element: $(this).attr("id") };
                arrtabobjects.push(tabobjects);
            });
        }
    </script>
    <%--for tab (doc detail fields panel) show and hide functionality --%>

    <style type="text/css" media="print">
        body, html, #wrapper {
            font-size: 8px;
            font-family: @Adobe Fan Heiti Std B;
            width: 100%;
            border-collapse: collapse;
            border: 0px;
            border-spacing: 0px;
            float: left;
            height: 200%;
            margin-bottom: 0px;
            margin-left: 0px;
            margin-right: 0px;
            margin-top: 0px;
        }

        .err {
            color: red;
        }
    </style>
    <script type="text/javascript">
        function OpenWindow(url) {
            var new_window = window.open(url, "new", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }
        $(document).ready(function () {
            //    $('#panel1').slidePanel({
            //	triggerName: '#trigger1',
            //	position: 'fixed',
            //	triggerTopPos: '65px',
            //	panelTopPos: '95px'
            //});

            //$('.detail').each(function() {
            //    var $link = $(this);
            //    var $dialog = $('<div></div>')
            //		.load($link.attr('href'))
            //		.dialog({
            //		    autoOpen: false,
            //		    title: 'Document Detail',
            //		    width: 700,
            //		    height: 550,
            //		    modal: true
            //		});

            //    $link.click(function() {
            //        $dialog.dialog('open');
            //        return false;
            //    });
            //});
        });

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

        function DeleteConfirm() {
            var x;
            if (confirm("Are you sure you want to delete?") == true) {
                return true;
            } else {
                return false;
            }

        }


    </script>
    <script>
        $(function () {
            $("#tabs").tabs();
        });
        $(function () {
            $("#tabss").tabs();
        });

        $(function () {
            $("#tab").tabs();
        });
    </script>
    <%--Add content for Next level design --%>
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet"
        type="text/css" />
    <link href="assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet"
        type="text/css" />
    <link href="css/docstyle.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            //alert('yes');		
            //$(".t").unbind().bind("click", function () {		
            //    alert('yes full tab');		
            //    $(".fulltab").each(function () {		
            //        alert('yes full tab l');		
            //        //$(this).css("background-color", "#3a87ad");		
            //        $(this).hide();		
            //    });		
            //    var id = $(this).attr("id");		
            //    alert(id);		
            //    $("#t" + id.substring(2, id.length)).show();		
            //    //$("#li" + id.substring(2, id.length)).css("background-color", "#23769c");		
            //});		
        });
    </script>
    <%--For child Item--%>
    <script type="text/javascript">
        $(function () {
            $(".c").bind("click", function () {
                var i = 0;
                $(".Childfulltab").each(function () {
                    $(this).hide();
                });
                var id = $(this).attr("id");
                $("#C" + id.substring(2, id.length)).show();
            });
        });
        function setCSS(obj) {
            $(".fulltab").each(function () {
                $(this).hide();
            });
            var id = $(obj).attr("id");
            $("#t" + id.substring(2, id.length)).show();
        }
    </script>
    <%--Add content for Next level design --%>
    <script src="assets/scripts/app.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // initiate layout and plugins		
            App.init();
            UIGeneral.init();
        });
    </script>
    <style type="text/css">
        .modal {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }

        .docloading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 99999;
        }

         .autocomplete_completionListElement {    
    width:500px!important;
    min-width:500px;
    margin : 0px!important;
    background-color : inherit;
    color : windowtext;
    border : buttonshadow;
    border-width : 1px;
    border-style : solid;
    overflow :auto;
    height : 200px;
    text-align : left; 
}
  
    </style>

    <style type="text/css" media="all">
        .CS {
            background-color: white;
            color: #99ae46;
            border: 1px solid #99ae46;
            font: Verdana 10px;
            padding: 1px 4px;
            font-family: Palatino Linotype, Arial, Helvetica, sans-serif;
        }

        #loading-div {
            width: 300px;
            height: 50px;
            background-color: #fff !important;
            text-align: center;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -150px;
            margin-top: -100px;
            z-index: 2000;
            opacity: 1.0;
            color: black;
        }

        #loading-div-background {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            background: black;
            width: 100%;
            height: 100%;
            z-index: 1000;
        }
    </style>
    
     <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/js/select2.min.js"></script>
</head>
    
<body class="page-header-fixed">
    <form id="form1" runat="server">
        <div id="loading-div-background" style="display: none; position: fixed; top: 0; left: 0; background: black; width: 100%; height: 100%; z-index: 1000000;">
            <div id="loading-div" class="ui-corner-all" style="width: 300px; height: 64px; background-color: #fff !important; text-align: center; position: absolute; left: 50%; top: 50%; margin-left: -150px; margin-top: -100px; z-index: 2000; opacity: 1.0; color: black;">
                <img style="height: 30px; margin-top: 5px;" src="images/attch.gif" alt="Loading.." />
                <h2 style="color: black; font-weight: normal;">Uploading. Please wait....</h2>
            </div>
        </div>
        <ajaxToolkit:ToolkitScriptManager ID="tscript" runat="server"></ajaxToolkit:ToolkitScriptManager>
        <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrint1" />
                <asp:PostBackTrigger ControlID="btnPrint2" />
                <asp:PostBackTrigger ControlID="btnPrint3" />
                <asp:PostBackTrigger ControlID="btnPrintWord" />
                 <asp:PostBackTrigger ControlID="btnPrintExcel" />
            </Triggers>
            <ContentTemplate>
                <div class="box" style="text-align: center; width: 100%;">
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                 Please wait..! its processing
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </div>
                <%--Add content for next level design--%>
                <!-- BEGIN HEADER -->
                <div class="header navbar navbar-inverse navbar-fixed-top">
                    <!-- BEGIN TOP NAVIGATION BAR -->
                    <div class="top-inner">
                        <div class="row">
                            <div class="container-fluid container-fluid-amend">
                            <!-- BEGIN LOGO -->
                            <div class="col-md-3 col-sm-3">
                                <a class="brand" href="index.html">
                                    <img id="imglogo" runat="server" src="images/logo.png" alt="logo" />
                                </a>
                            </div>
                            <!-- END LOGO -->
                            <div class="col-md-7 col-sm-7">
                                <!-- BEGIN TOP MIDDLE TAB -->
                                <div class="top-btn">
                                    <ul class="tab-btn">
                                        <li class="">
                                            <asp:Button ID="btnDocApprove" ToolTip="Approve" OnClick="ShowApprove" OnClientClick="ShowProgress();" runat="server" /></li>
                                        <li class="">
                                            <asp:Button ID="btnDocReject" ToolTip="Reconsider" OnClick="ShowReconsider" OnClientClick="ShowProgress();" runat="server" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnRejectDoc" ToolTip="Reject" OnClick="ShowPermanentReject" OnClientClick="ShowProgress();" runat="server" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnDocEdit" ToolTip="Edit" runat="server" OnClientClick="ShowProgress();" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnAmendment" ToolTip="Amendment" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnRecall" ToolTip="Recall" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnCancel" ToolTip="Cancel" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnSplit" ToolTip="Split/Copy" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnCopy" ToolTip="Copy" runat="server" Visible="false" OnClientClick="ShowProgress();" />
                                        </li>
                                        <li class="">
                                            <asp:Button ID="btnEPD" ToolTip="Early Payment Discounting" OnClick="EarlyPaymentDiscounting" runat="server" Visible="false" />
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <!-- END TOP MIDDLE TAB -->
                            <div class="col-md-2 col-sm-2 pull-right" style="padding: 10px;">
                                <!-- BEGIN TOP NAVIGATION MENU -->
                                <div class="pull-right" style="padding: 10px;">
                                    <asp:ImageButton ID="btnCloseWindow" OnClientClick="javaScript:window.close(); return false;" ToolTip="Close" runat="server" ImageUrl="~/images/remove-icon-small.png" />
                                </div>
                                <div class="pull-right" style="padding: 10px;">
                                    <asp:ImageButton ID="btnPrint1" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/print.png" />
                                </div>
                                <div class="pull-right" style="padding: 10px;">
                                    <asp:ImageButton ID="btnPrint2" OnClick="PrintDoc" ToolTip="Print" runat="server" ImageUrl="~/images/print.png" CssClass="print" />
                                </div>
                                <div class="pull-right" style="padding: 10px;">
                                   <asp:ImageButton ID="btnPrint3" class="printPage" ToolTip="Print" runat="server" ImageUrl="~/images/print.png" />
                                </div>
                                <div class="pull-right" style="padding: 10px;">
                                    <asp:ImageButton ID="btnPrintWord" OnClick="PrintWord" ToolTip="Print" Visible="false" runat="server" ImageUrl="~/images/word.png" />
                                </div>

                                <div class="pull-right" style="padding: 10px;">
                                    <asp:ImageButton ID="btnPrintExcel" OnClick="PrintExcel" ToolTip="Print" Visible="false" runat="server" ImageUrl="~/images/excelexpo.jpg" />
                                </div>
                                <!-- END TOP NAVIGATION MENU -->
                            </div>
                        </div>
                        </div>
                    </div>
                </div>
                <!-- END HEADER -->
                <!-- BEGIN CONTAINER -->
                <!-- END HEADER -->
                <!-- BEGIN CONTAINER -->
                <div class="page-container row-fluid">
                    <!-- END SAMPLE PORTLET CONFIGURATION MODAL FORM-->
                    <div class="span12">
                        <!-- BEGIN PAGE HEADER-->
                        <div class="tabbed-area">
                            <ul class="tabs">
                                <li id="li1" class="t" onclick="javascript:setCSS(this);"><a href="javascript:void(0);">CURRENT </a></li>
                                <li id="li2" runat="server" class="group t" onclick="javascript:setCSS(this);"><a
                                    href="javascript:void(0);">HISTORY </a></li>
                                 <li id="li3" runat="server" class="group t" onclick="javascript:setCSS(this);"><a
                                    href="javascript:void(0);">
                                    <asp:Label ID="lblCRM" runat="server"></asp:Label>
                                </a></li>
                            </ul>
                        </div>
                        <!-- END PAGE HEADER-->
                    </div>
                    <div class="container-fluid">
                        <!-- BEGIN PAGE -->
                        <div id="staticContent" runat="server">
                        </div>
                        <div id="t1" runat="server" class="page-content fulltab">
                            <!-- BEGIN PAGE CONTENT-->
                        </div>
                        <!-- BEGIN fixed tab history-->
                        <div id="t2" runat="server" class="portlet box blue fulltab" style="display: none; margin-bottom: 30px;">
                            <div class="portlet-title">
                                <div class="caption">
                                    History Detail
                                </div>
                            </div>
                            <div class="portlet-body" id="divHistory" runat="server">
                            </div>
                        </div>
                        <!-- BEGIN fixed tab history-->
                    </div>
                    <!-- BEGIN fixed table4 for Movement detail-->
                    <div class="container-fluid">
                        <div class="portlet box blue">
                            <div class="portlet-title">
                                <div class="caption">
                                    Movement Detail
                                </div>
                            </div>
                            <div class="portlet-body" id="movement" runat="server">
                            </div>
                        </div>
                    </div>
                   
                    <!-- BEGIN PAGE -->
                    <!-- BEGIN fixed table4 for future Movement detail-->
                    <div class="container-fluid">
                        <div class="portlet box blue">
                            <div class="portlet-title">
                                <div class="caption">
                                    Future Movements
                                </div>
                            </div>
                            <div class="portlet-body" id="futureMovement" runat="server">
                            </div>
                        </div>
                    </div>
                    <%--Add content for next level design--%>
                    <div class="container-fluid container-fluid-amend">
                         <table style="width: 100%; cellspacing: 0px; cellpadding: 0px; empty-cells: show">
                        <tr>
                            <td style="width: 80%">
                                <div id="tabss">
                                    <%--<ul>
                                    <li>
                                        <asp:Label ID="lblpending" runat="server"><a href="#tabPending">CURRENT</a></asp:Label>
                                    </li>
                                    <li>
                                        <asp:Label ID="lblaction" runat="server">
                                            <asp:Label ID="LBLHistroy" runat="server"></asp:Label>
                                        </asp:Label>
                                    </li>
                                    <li>
                                        <asp:Label ID="Label5" runat="server">
                                            <asp:Label ID="lblCRM" runat="server"></asp:Label>
                                        </asp:Label>
                                    </li>
                                </ul>
                                <div id="tabPending" style="min-height: 300px;">
                                    <asp:UpdatePanel ID="updPnlGrid" runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <asp:Label ID="lblDetail" runat="server" Text="Folder Name"></asp:Label>
                                            </div>
                                            <div>
                                                <h3>
                                                    Movement Detail</h3>
                                                <asp:GridView ID="gvMovDetail" runat="server" AutoGenerateColumns="False" DataKeyNames="tid"
                                                    Width="100%" PageSize="20">
                                                    <HeaderStyle CssClass="GridHeader" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="S.No">
                                                            <ItemTemplate>
                                                                <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="20px" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="UserName" HeaderText="User Name">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="STATUS" HeaderText="STAGE">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="fdate" HeaderText="In Date">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="tdate" HeaderText="Out Date">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="ptat" HeaderText="SLA">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="atat" HeaderText="A. SLA">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="remarks" HeaderText="Remarks">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                    <SortedDescendingHeaderStyle BackColor="#002876" />
                                                </asp:GridView>
                                            </div>
                                            <div>
                                                <h3>
                                                    Future Movements</h3>
                                                <asp:GridView ID="gvFutureMov" runat="server" AutoGenerateColumns="False" DataKeyNames="tid"
                                                    Width="100%" PageSize="20">
                                                    <HeaderStyle CssClass="GridHeader" HorizontalAlign="Left" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="S.No">
                                                            <ItemTemplate>
                                                                <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="20px" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="username" HeaderText="User/Role to Action">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="STATUS" HeaderText="STAGE">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SLA" HeaderText="SLA">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                    <SortedDescendingHeaderStyle BackColor="#002876" />
                                                </asp:GridView>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>--%>
                                    <%--      <div id="tabMy" runat="server" style="min-height: 300px;">
                                        <asp:UpdatePanel ID="updPNLMyUpload" runat="server" style="overflow: auto">
                                            <ContentTemplate>
                                                <br />
                                                <asp:Panel ID="pnlhis" runat="server" Width="800px">
                                                    <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" DataKeyNames="tid" Width="100%" PageSize="20">
                                                        <HeaderStyle CssClass="GridHeader" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="S.No">
                                                                <ItemTemplate>
                                                                    <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>--%>
                                    <div id="divCRM" runat="server">
                                        <asp:UpdatePanel ID="UpCRMGrid" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <%--<div>
                                                    <asp:Label ID="lblCRMHeading" runat="server" Text="CRM Details"></asp:Label>
                                                </div>--%>
                                                <div>
                                                    <h3 style="background-color: #FAFAFA">
                                                        <asp:Label ID="lblCRMHeader" runat="server"></asp:Label>
                                                    </h3>
                                                    <div style="width: auto; height: auto; border: 5px solid #bed5cd; overflow-x: scroll; overflow-y: scroll; white-space: nowrap;">
                                                        <asp:GridView ID="GVCRM" DataKeyNames="tid" runat="server" AutoGenerateColumns="True"
                                                            OnRowCreated="gv_OnRowCreated" Width="100%">
                                                            <HeaderStyle CssClass="GridHeader" />
                                                            <EmptyDataTemplate>
                                                                <div style="border: none; text-align: center; width: 100%; height: 100%;">
                                                                    No activity found.
                                                                </div>
                                                            </EmptyDataTemplate>
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="View Conversation">
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="btnViewConver" runat="server" CssClass="btnNew" Text="View" OnClick="GetConversation" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                                <div>
                                                    <table width="100%" cellspacing="0px" cellpadding="0px">
                                                        <tr style="height: 40px;">
                                                            <td colspan="3"></td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 80%"></td>
                                                            <td align="right">
                                                                <asp:Button ID="btnAddAction" runat="server" Text="Add Action" CssClass="btnNew" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:Button ID="btnSendMail" runat="server" Visible="false" Text="Send Mail" CssClass="btnNew" />
                                                            </td>
                                                            <%--<td align="right">
                                                                <asp:Button ID="btnSendMessage" Visible="false" runat="server" Text="Send Message" CssClass="btnNew" />
                                                            </td>--%>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                    </div>
                    <div class="container-fluid container-fluid-amend">
                    <div class="row" id="footer">
                        <div class="col-md-12" id="footer-content">
                            <div class="ftr"> Copyright © Mynd Integrated Solutions Pvt. Ltd. All rights reserved.</div>
                        </div>
                    </div>
                </div>
                    <asp:Button ID="btnShowPopUpApprove" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnApprove_ModalPopupExtender" runat="server" TargetControlID="btnShowPopUpApprove"
                        PopupControlID="pnlPopupApprove" CancelControlID="btnCloseApprove" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <div>
                        <%--<asp:Panel ID="pnlPopupApprove" runat="server" Style="display: none; height: 300px; overflow: scroll; width: auto;"--%>
                        <asp:Panel ID="pnlPopupApprove" runat="server" Style="display: none; height: 300px;width:1095px; overflow: scroll;" BackColor="white">                        
                            
                            <div class="box">
                                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                    <tr>
                                        <td>
                                            <h3>
                                                <asp:Label ID="lblAppdoc" runat="server"></asp:Label>
                                                Document</h3>
                                        </td>
                                        <td style="width: 50px;">
                                            <asp:ImageButton ID="btnCloseApprove" ImageUrl="images/close.png" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: left; width: 100%">
                                            <asp:UpdatePanel ID="updatePanelApprove" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblTabApprove" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Panel ID="pnlApprove"  runat="server">
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblMsgRule2" runat="server" Text=""></asp:Label>
                                                                <div style="clear: both;">
                                                                </div>

                                                                <div style="text-align: right;">
                                                                    <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClientClick="this.disabled=true;ShowProgress();"
                                                                        UseSubmitBehavior="false" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small"
                                                                        Width="100px" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </asp:Panel>
                    </div>

                    <%--Added functionality for early payment discounting--%>
                    <asp:Button ID="btnShowDiscounting" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnShowDiscounting_ModelPopupExtender" runat="server" TargetControlID="btnShowDiscounting"
                        PopupControlID="pnlPopupDiscounting" CancelControlID="btnCloseDiscounting" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <asp:Panel ID="pnlPopupDiscounting" runat="server" Width="999px" Height="350px" Style="display: none; overflow: auto" BackColor="White">
                        <div class="doc_header">
                            Discounting Details
                                 <div class="pull-right">
                                     <asp:ImageButton ID="btnCloseDiscounting" ImageUrl="images/close.png" runat="server" />
                                 </div>

                        </div>
                        <div class="row" style="margin: 10px 0px;">
                            <div class="col-md-12 col-sm-12  col-xs-12">
                                <div class="col-md-3 col-sm-3  col-xs-3">
                                    <label>Invoice Amount</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3">
                                    <asp:TextBox ID="txtPaidAmount" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3">
                                    <label>Discounting Type</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3">
                                    <asp:DropDownList ID="ddlDiscountingType" runat="server" CssClass="txtBox" onchange="javascript:discounttype();">
                                    </asp:DropDownList>
                                </div>

                            </div>
                        </div>
                        <div class="row" style="margin: 5px 0px;">

                            <div class="col-md-12 col-sm-12  col-xs-12">
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Discounting Rate List</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:DropDownList ID="ddlDiscounting" runat="server" CssClass="txtBox" onchange="javascript:discountratelist();">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Rate Of Interest</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:TextBox ID="txtROI" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div class="row" style="margin: 5px 0px;">
                            <div class="col-md-12 col-sm-12  col-xs-12">
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Days</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:TextBox ID="txtNOD" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Discount Margin (%)</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:TextBox ID="txtMarginMoney" runat="server" CssClass="txtBox" onkeyup="javascript:netpaidamount();"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div class="row" style="margin: 5px 0px;">
                            <div class="col-md-12 col-sm-12  col-xs-12">
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Payment Date</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:TextBox ID="txtPaymentDate" runat="server" CssClass="txtBox disable_past_dates" ReadOnly="true" Width="81%" onchange="return CalWithCalendar();"></asp:TextBox>
                                    <img id="ContentPlaceHolder1_imgtxtPaymentDate" src="images/cal.png" />
                                    <asp:CalendarExtender ID="cal_txtPaymentDate" runat="server" PopupButtonID="ContentPlaceHolder1_imgtxtPaymentDate" TargetControlID="txtPaymentDate" Enabled="true" Format="dd/MM/yy"></asp:CalendarExtender>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Due Date</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:TextBox ID="txtDueDate" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div class="row" style="margin: 5px 0px;">
                            <div class="col-md-12 col-sm-12  col-xs-12">
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <label>Net Paid Amount</label>
                                </div>
                                <div class="col-md-3 col-sm-3  col-xs-3 self">
                                    <asp:TextBox ID="txtnetPaidAmount" runat="server" CssClass="txtBox" ReadOnly="true"></asp:TextBox>
                                    <asp:HiddenField ID="hdnDocID" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="row pull-right" style="margin: 5px 0px;">
                            <div class="col-md-12 col-sm-12  col-xs-12">
                                <button type="button" class="btn btn-primary submit" onclick="AcceptDiscounting('REJECT');" style="height: 30px; width: 100px;" value="Login"><i class="fa fa-times"></i>Reject</button>
                                <button type="button" id="btnAccept" class="btn btn-success submit" onclick="AcceptDiscounting('ACCEPT');" style="height: 30px; width: 100px;" value="Login"><i class="fa fa-check"></i>Accept</button>
                            </div>
                        </div>
                        <%--  </div>--%>
                    </asp:Panel>
                    <%--Added functionality for early payment discounting--%>



                    <asp:Button ID="btnShowPopupReject" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnReject_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupReject"
                        PopupControlID="pnlPopupReject" CancelControlID="btnCloseReject" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <asp:Panel ID="pnlPopupReject" runat="server" Width="700px" Height="400px" Style="display: none; overflow: auto"
                        BackColor="aqua">
                        <div class="box">
                            <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                                <tr>
                                    <td>
                                        <h3>
                                            <asp:Label ID="lblrejdoc" runat="server"></asp:Label>
                                            Document</h3>
                                    </td>
                                    <td style="width: 31px">
                                        <asp:ImageButton ID="btnCloseReject" ImageUrl="images/close.png" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: left">
                                        <asp:UpdatePanel ID="updatePanelReject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Label ID="lblTabRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                <asp:HiddenField ID="hdnIsConditionalReconsider" runat="server" />
                                                <div id="DIVConditionalReconsider" runat="server" visible="false" class="form" style="min-height:50px !important; width:95%;">
                                                    <table width="100%" cellspacing="5px" border="0" cellpadding="0px">
                                                        <tr>
                                                            <td style="width:208px; text-align:left;" ><span id="lbl29242" style="font-weight: bold;">Reconsider to Stage:*</span></td>
                                                            <td >
                                                                <asp:DropDownList ID="ddlReconsiderStage" CssClass="txtBox" runat="server" Visible="true"></asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <asp:Panel ID="pnlFieldsRej" Width="100%" runat="server">
                                                </asp:Panel>
                                                <div style="width: 100%; text-align: right">
                                                    <asp:Label ID="lblRuleMsg3" runat="server" Text=""></asp:Label>
                                                    <asp:Button ID="btnReject" OnClientClick="this.disabled=true;ShowProgress();" UseSubmitBehavior="false"
                                                        runat="server" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small" OnClick="editBtnReject"
                                                        Text="Reject" Width="100px" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>
                    <asp:Button ID="btnPerRejectpopup" runat="server" Style="display: none" />
                    <asp:ModalPopupExtender ID="btnPerRejectModalpopup" runat="server" TargetControlID="btnPerRejectpopup"
                        PopupControlID="pnlPerReject" CancelControlID="btnClosePerReject" BackgroundCssClass="modalBackground"
                        DropShadow="true">
                    </asp:ModalPopupExtender>
                    <asp:Panel ID="pnlPerReject" runat="server" Width="700px" Height="400px" Style="display: none; overflow: auto"
                        BackColor="aqua">
                        <div class="box">
                            <table cellspacing="2px" cellpadding="2px" width="100%">
                                <tr>
                                    <td>
                                        <h3>Permanent
                                            <asp:Label ID="lblPREJ" runat="server"></asp:Label>
                                            Document</h3>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:ImageButton ID="btnClosePerReject" ImageUrl="images/close.png" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: left">
                                        <asp:UpdatePanel ID="updPerReject" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Label ID="lblPerRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                <asp:Panel ID="PanelPerReject" Width="100%" runat="server">
                                                </asp:Panel>
                                                <div style="width: 100%; text-align: right">
                                                    <asp:Label ID="lblMsgRule1" runat="server" Text=""></asp:Label>
                                                    <asp:Button ID="btnPerReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                                        runat="server" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small" OnClick="editBtnPerReject"
                                                        Text="Reject" Width="100px" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <%--HTML FOR CRM by Ajeet--%>
        <asp:Button ID="btnCRMH" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="MDLCRMAC" runat="server" TargetControlID="btnCRMH" PopupControlID="pnlCRMAction"
            CancelControlID="IMGCRMClose" BackgroundCssClass="modalBackground" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlCRMAction" runat="server" Style="display: none; overflow: auto"
            BackColor="aqua">
            <div class="box" style="width: 100%;">
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td>
                            <h3>Add Activity</h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="IMGCRMClose" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: left; width: 100%">
                            <asp:UpdatePanel ID="UpCRMACtion" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="lblCRMAction" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    <asp:Panel ID="pnlCRMFields" Width="100%" runat="server">
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="btnCRMAction" runat="server" Text="Add" OnClientClick="this.disabled=true;"
                                            UseSubmitBehavior="false" CssClass="ststusButton" Font-Bold="True" Font-Size="X-Small"
                                            Width="100px" OnClick="btnCRMApprove" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <asp:Button ID="btnPSendSMS" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="mdpPopUpSMS" runat="server" TargetControlID="btnPSendSMS"
            PopupControlID="pnlSendSmS" CancelControlID="imgSmsClose" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlSendSmS" runat="server" BorderColor="#666666" ForeColor="Black"
            Style="display: none;" BackColor="Silver">
            <div>
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td>
                            <h3 style="border: solid 2px #f6f6f6;">Send Message</h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="imgSmsClose" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: left; width: 100%">
                            <asp:UpdatePanel ID="upSendMessage" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                        <tr style="height: 20px;">
                                            <td colspan="2">
                                                <asp:Label ID="lblSMSMessage" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Mobile No.<span style="color: red;">*</span>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMob" onkeypress="return numbersonly(event);" runat="server" MaxLength="10"
                                                    Width="200"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td>Message.<span style="color: red;">*</span>
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" Columns="10" TextMode="MultiLine" Rows="5" ID="txtMeaasge"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">
                                                <asp:Button ID="btnSendSMS" runat="server" Text="Send" CssClass="ststusButton" OnClientClick="javascript:return ValidateSMS();" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Button ID="hdnVConv" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="mdlCRMViewConver" runat="server" TargetControlID="hdnVConv"
            PopupControlID="pncCRMViewConver" CancelControlID="IMgCloseCRM" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pncCRMViewConver" runat="server" BorderColor="#666666" Width="600"
            ForeColor="Black" Style="display: none;" BackColor="Silver">
            <div>
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td width="95%">
                            <h3 style="border: solid 2px #f6f6f6;">View Conversation</h3>
                        </td>
                        <td style="width: 5%">
                            <asp:ImageButton ID="IMgCloseCRM" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: left; width: 100%">
                            <asp:UpdatePanel ID="upCRMConver" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                        <tr style="height: 20px;">
                                            <td colspan="4">
                                                <asp:Label ID="CRMMessage" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="15%">
                                                <b>Mobile No.</b>
                                            </td>
                                            <td width="35%">
                                                <asp:Label runat="server" ID="lblMobNum"></asp:Label>
                                            </td>
                                            <td width="10%">
                                                <b>To Mail.</b>
                                            </td>
                                            <td width="40%">
                                                <asp:Label runat="server" ID="lblTomail"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <b>Conversation:</b>
                                                <div style="border: solid 1px #4cff00; overflow: scroll; width: 100%; height: 200px; overflow: auto;">
                                                    <asp:Label ID="lblConver" runat="server"></asp:Label>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Button ID="hdnbtnsendMail" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="mdlSendMail" runat="server" TargetControlID="hdnbtnsendMail"
            PopupControlID="pnlSendMail" CancelControlID="imgClosebtnClose" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlSendMail" runat="server" Width="850" Height="550" BorderColor="#666666"
            Style="display: none;" ForeColor="Black" BackColor="Silver">
            <div>
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td style="width: 95%;">
                            <h3 style="border: solid 2px #f6f6f6;">Send Mail</h3>
                        </td>
                        <td style="width: 5%">
                            <asp:ImageButton ID="imgClosebtnClose" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: left; width: 100%">
                            <asp:UpdatePanel ID="UpSendMail" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="2px" cellpadding="2px" width="100%">
                                        <tr style="height: 20px;">
                                            <td colspan="4">
                                                <asp:Label ID="lblsendMSG" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="10%">From Email.<span style="color: red;">*</span>
                                            </td>
                                            <td width="40%">
                                                <asp:DropDownList runat="server" onchange="javascript:TomailChange(this.value);"
                                                    Width="250" ID="ddlFromMail" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                            <td width="10%">Password.
                                            </td>
                                            <td width="50%">
                                                <asp:TextBox ID="txtPassword" TextMode="Password" Width="250" ReadOnly="true" CssClass="txtBox"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 10px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <%--<tr>
                                                    <td>SMTP Server.</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSMTP" ReadOnly="true" runat="server" CssClass="txtBox" Width="400" Height="30"></asp:TextBox>
                                                    </td>
                                                </tr>--%>
                                        <tr style="height: 10px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td>To.<span style="color: red;">*</span>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtToMail" placeholder="Enter emails seperated by ','" Width="250"
                                                    runat="server" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td>CC.
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCC" runat="server" placeholder="Enter emails seperated by ','"
                                                    Width="250" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td>Bcc.
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCRMBCC" placeholder="Enter emails seperated by ','" Width="250"
                                                    runat="server" CssClass="txtBox"></asp:TextBox>
                                            </td>
                                            <td>Subject.
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCRMSub" runat="server" placeholder="Enter subject" Width="250"
                                                    CssClass="txtBox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td>Body.<span style="color: red;">*</span>
                                            </td>
                                            <td align="left" style="height: 250px;" colspan="3">
                                                <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="250px" Width="650"
                                                    BackColor="White">
                                                    <asp:TextBox ID="txtBody" Visible="false" TextMode="MultiLine" Text=" " runat="server"
                                                        Width="100%" Height="98%" BackColor="White">
                                                    </asp:TextBox>
                                                    <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" TargetControlID="txtbody"
                                                        EnableSanitization="false">
                                                    </asp:HtmlEditorExtender>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr style="height: 10px;">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="4">
                                                <asp:Button ID="btnSendmailHit" OnClick="btnSendmailHit_Click" OnClientClick="javascript:return ValidateCRMMail();"
                                                    runat="server" Text="Send" CssClass="ststusButton" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <%--CRM Code END HERE--%>
        <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="ModalEditPopup" runat="server" TargetControlID="btnShowPopupEdit"
            PopupControlID="pnlEdit" CancelControlID="ImageButton1" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlEdit" Height="630" Width="1300" runat="server" ScrollBars="auto" BackColor="aqua">
            <div class="box">

                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;"
                    border="1px">
                    <tr>
                        <td style="width: 940px">
                            <h3>
                                <asp:Label ID="lblstatus" runat="server" Text="Document Editing" Font-Bold="True"></asp:Label></h3>
                        </td>
                        <td style="width: 60px">
                            <asp:ImageButton ID="ImageButton1" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div>
                                        <asp:Label ID="lblDetail1" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                    <asp:Panel ID="pnlFields" runat="server" Width="1200">
                                    </asp:Panel>
                                    <div id="dtBox" style="z-index: 9999999999;">
                                         <asp:HiddenField ID="hdnCurrentMainTab" runat="server" />
                                    </div>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="btnActEdit" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                            Font-Size="X-Small" OnClick="EditRecord" Width="100px" />
                                    </div>
                                    <%--<asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                        <ProgressTemplate>
                                            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                                                <asp:Image ID="Image2" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                please wait...
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>--%>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <%--for document child item edit--%>
        <asp:Button ID="Btnchild" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="Btnchild"
            PopupControlID="pnlPopupchild" CancelControlID="btnClose" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlPopupchild" runat="server" Width="800px" Height="300px" ScrollBars="Auto"
            BackColor="aqua">
            <div class="box">
                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Pnllable" runat="server">
                                        <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlFields1" Width="100%" runat="server">
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                            OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;"
                                            UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Button ID="btnReCallpopUp" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="MP_Recall" runat="server" TargetControlID="btnReCallpopUp"
            PopupControlID="pnlRecall" CancelControlID="btnCloseRecall" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlRecall" runat="server" Width="800px" Height="300px" ScrollBars="Auto"
            BackColor="aqua">
            <div class="box">
                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="lblAmendment" runat="server" Font-Bold="True" Text="DOCUMENT RECALL"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnCloseRecall" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="updRecall" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel2" runat="server">
                                        <asp:Label ID="lblRecallMess" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="Panel3" Width="100%" runat="server">
                                        <asp:Label ID="Label4" runat="server" Text="Recall Remarks" Font-Bold="True"></asp:Label>
                                        &nbsp;&nbsp
                                    <asp:TextBox ID="txtRemarkRecall" runat="server" CssClass="txtBox" Width="500px"></asp:TextBox>
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="btnRecallSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                            Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                        <asp:Button ID="btnCancelReject" runat="server" Text="Cancel" CssClass="btnNew" Font-Bold="True"
                                            Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <%--for cancel popup --%>
        <asp:Button ID="btnCancelPopup" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="MP_CancelPopup" runat="server" TargetControlID="btnCancelPopup"
            PopupControlID="pnlCancel" CancelControlID="btnClosecancel" BackgroundCssClass="modalBackground"
            DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlCancel" runat="server" Width="800px" Height="300px" ScrollBars="Auto"
            BackColor="aqua">
            <div class="box">
                <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                    <tr>
                        <td>
                            <h3>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="DOCUMENT CANCEL"></asp:Label></h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnClosecancel" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="UpdDocCancel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel4" runat="server">
                                        <asp:Label ID="lblDocCancelMsg" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlDocCancel" Width="100%" runat="server">
                                    </asp:Panel>
                                    <div style="width: 100%; text-align: right">
                                        <asp:Button ID="btnCancelSave" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                            Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                        <asp:Button ID="btnCancelCan" runat="server" Text="Cancel" CssClass="btnNew" Font-Bold="True"
                                            Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm"
            TargetControlID="btnShowPopupForm" BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlPopupForm" runat="server" Width="400px" Height="100px" BackColor="White"
            Style="display: none">
            <div class="box">
                <table cellspacing="0px" cellpadding="0px" width="100%">
                    <tr>
                        <td style="width: 400px">
                            <h3>Confirmation</h3>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="updMsg" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="lblMsg" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Button ID="btnOk" runat="server" Text="OK" OnClick="Reset" CssClass="btnNew"
                                Width="80px" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <%--For Show history popup--%>
        <asp:Button ID="btnShowPopupHistory" runat="server" Style="display: none" />
        <asp:ModalPopupExtender ID="btnShowPopupHistory_ModalPopupExtender" runat="server"
            PopupControlID="pnlShowPopupHistory" TargetControlID="btnShowPopupHistory" CancelControlID="btnCloseShowPopupHistory"
            BackgroundCssClass="modalBackground" DropShadow="true">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlShowPopupHistory" runat="server" Width="1000px" Height="300px"
            BackColor="White" Style="">
            <div class="box">
                <table cellspacing="1px" cellpadding="1px" width="100%">
                    <tr>
                        <td width="980px">
                            <h3>Transaction History
                            </h3>
                        </td>
                        <td style="width: 20px">
                            <asp:ImageButton ID="btnCloseShowPopupHistory" ImageUrl="images/close.png" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:UpdatePanel ID="UpdatePanel_ShowPopupHistory" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlgridhistory" runat="server" ScrollBars="Both" Width="100%">
                                        <asp:GridView ID="grdShowPopupHistory" DataKeyNames="TID" Width="100%" runat="server"
                                            BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px"
                                            CellPadding="3" CellSpacing="2" AutoGenerateColumns="true">
                                            <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                                            <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                                            <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                                            <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                                            <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                            <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                            <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                            <SortedDescendingHeaderStyle BackColor="#93451F" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <div class="docloading" align="center">
            Its processing.<br />
            <br />
            <img src="images/loader.gif" alt="" />
        </div>
    </form>
    <script type="text/javascript">
        function hideprogressbar() {
            $(".docloading").hide();
        }
        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".docloading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 200);
        }
    </script>

    <script type="text/javascript">
        $(".printPage").click(function () {
            $("#t1 .row-fluid").each(function () {
                $(this).find(".portlet-title .tools a").each(function () {
                    $(this).removeClass("expand"); $(this).addClass("collapse");
                });
                //    ' for Hide all content'
                $(this).find(".portlet-body").each(function () {
                    $(this).css("display", "block");
                });
                //isExpand = false;
            });
            window.print();
        })
    </script>

</body>
</html>
