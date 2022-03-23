<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="Documents, App_Web_4pl2ohtp" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" EnableViewState="false">


   <%-- <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.0/jquery.min.js" type="text/jscript"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" type="text/jscript"></script>
    <link href='https://fonts.googleapis.com/css?family=Raleway:400,600,700' rel='stylesheet' type='text/css' />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" />--%>

     <link rel="stylesheet" href="./Submit Request_files/bootstrap.min.css" media="screen" title="no title">
   <link href="./Submit Request_files/custom.min.css" rel="stylesheet">
   <!--link href="http://virtualmynd.m1xchange.com/assets/css/agency.min.css" rel="stylesheet"-->

    <meta http-equiv="X-UA-Compatible" content="IE=10,chrome=1" />
    <%--<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>--%>
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <link href="css/TabStyleSheet.css" rel="stylesheet" type="text/css" />
    <script src="js/Utils.js" type="text/javascript"></script>
    <script src="js/TicketUtils.js" type="text/javascript"></script>
    <link href="css/DateTimePicker.css" rel="stylesheet" />
    <script src="js/DateTimePicker.js" type="text/javascript"></script>
    <script src="js/Canara/CanaraScript.js" type="text/javascript"></script>
    <script src="js/CanaraHSBC/CanaraHSBCScript.js" type="text/javascript"></script>
    <script src="js/FreeCharge/FreeChargeScript.js" type="text/javascript"></script>
    <script type="text/javascript"></script>


<script src="./Submit Request_files/jquery.min.js.download"></script>
<script src="./Submit Request_files/jquery.inputmask.bundle.min.js.download"></script>
<script src="./Submit Request_files/moment.min.js.download"></script>
<script src="./Submit Request_files/bootstrap-datetimepicker.min.js.download"></script>


    
<style>
#snackbar {
    visibility: hidden;
    min-width: 250px;
    margin-left: -125px;
    background-color: #333;
    color: #fff;
    text-align: center;
    border-radius: 2px;
    padding: 16px;
    position: fixed;
    z-index: 1;
    left: 50%;
    bottom: 100px;
    font-size: 16px;
}

#snackbar.show {
    visibility: 

visible;
    -webkit-animation: fadein 0.5s, fadeout 0.5s 2.5s;
    animation: fadein 0.5s, fadeout 0.5s 2.5s;
}

@-webkit-keyframes fadein {
    from {bottom: 0; opacity: 

0;} 
    to {bottom: 100px; opacity: 1;}
}

@keyframes fadein {
    from {bottom: 0; opacity: 0;}
    to {bottom: 100px; opacity: 1;}
}

@-webkit-keyframes fadeout {
    from 

{bottom: 100px; opacity: 1;} 
    to {bottom: 0; opacity: 0;}
}

@keyframes fadeout {
    from {bottom: 100px; opacity: 1;}
    to {bottom: 0; opacity: 0;}
}
</style>



        function OpenWindow(url) {
            var new_window = window.open(url, "", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }
    </script>
    <style type="text/css">
        .table > thead > tr > th, .table > tbody > tr > th, .table > tfoot > tr > th, .table > thead > tr > td, .table > tbody > tr > td, .table > tfoot > tr > td {
        padding: 2px 8px;
        }
        .msgbody {
        float:left;
        }

    </style>
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
                    if (selTab != undefined) {
                        //    //var data1 = $('#tabs').find("div[id^='tabPending" + selTab + "']").html();                       
                        //    //alert(data1);
                        $('#tabs div[id=tabPending' + selTab + ']').find("input[id ^= 'ContentPlaceHolder1_bntCalFromGrid_']").click();
                        //    //alert(data2);
                        //    //data1.find("input[id^='ContentPlaceHolder1_bntCalFromGrid_']");
                    }
                    selTab = $('#tabs').tabs('option', 'selected');
                }
            });

        });;


        var docselTab;
        $(function () {
            var tabs = $("#doctabs").tabs({
                show: function () {
                    if (docselTab != undefined) {
                        //    //var data1 = $('#tabs').find("div[id^='tabPending" + selTab + "']").html();                       
                        //    //alert(data1);
                        $('#doctabs div[id=doctabPending' + docselTab + ']').find("input[id ^= 'ContentPlaceHolder1_bntCalFromGrid_']").click();
                        //    //alert(data2);
                        //    //data1.find("input[id^='ContentPlaceHolder1_bntCalFromGrid_']");
                    }
                    docselTab = $('#doctabs').tabs('option', 'selected');
                }
            });

        });;

        $(function () {
            $(".btnDyn").button()
        });
        var formtabSelDiv;
        var formTabIndex;
        var count = 1;
        $(document).ready(function () {
            // We Will hide all div initially then conditonal based show divs

            $(".maindivTabs").each(function (index) {
                if (formtabSelDiv == undefined) {
                    $("#" + $(this).attr('id')).show();
                    formtabSelDiv = $(this).attr('id');
                    $("#" + '<%= hdnCurrentMainTab.ClientID %>').val(index + 1);
                }
                else {
                    $("#" + $(this).attr('id')).hide();
                }
            });
            var CurrentSelectedIndex = $("#" + '<%= hdnCurrentMainTab.ClientID %>').val();
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

        function myFunction(id) {
            $(".maindivTabs").each(function (index) {
                if ($(this).attr('id') == id) {
                    $("#" + $(this).attr('id')).show();
                    $("#" + '<%= hdnCurrentMainTab.ClientID %>').val(index + 1);
                }
                else {
                    $("#" + $(this).attr('id')).hide();
                }

            });
            var GetSelectedIndex = $("#" + '<%= hdnCurrentMainTab.ClientID %>').val();
            $(".formtab").each(function (indexes) {
                if (indexes + 1 == GetSelectedIndex) {
                    $(this).addClass('done');
                }
                else {
                    $(this).removeClass('done');
                }
            });
        }
        function checkAll(GID) {
            //var obj = $("#ContentPlaceHolder1_gvData_ctl00").attr('checked');
            var GridIDCHK = "#ContentPlaceHolder1_GRD" + GID + "_ctl00";
            var obj = $(GridIDCHK);
            var GridID = "#ContentPlaceHolder1_GRD" + GID;
            //alert("Hi I Am being fired");
            //alert("Object Is" + obj);
            var str = $(obj).prop('checked');
            //alert("Property IS " + str);

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


        function pageLoad(sender, args) {
            var SelectedIndex = $("#" + '<%= hdnCurrentMainTab.ClientID %>').val();
            if (SelectedIndex != "") {
                $(".maindivTabs").each(function (index) {
                    if (index + 1 == SelectedIndex) {
                        $("#" + $(this).attr('id')).show();
                    }
                    else {
                        $("#" + $(this).attr('id')).hide();
                    }
                });
            }
            $(".formtab").each(function (indexes) {
                if (indexes + 1 == SelectedIndex) {
                    $(this).addClass('done');
                }
                else {
                    $(this).removeClass('done');
                }
            });
            if (args.get_isPartialLoad()) {
                //Code by Ajeet For Datetime picker
                //alert("This is partial page load");
                $("input[data-field='date'], input[data-field='time'], input[data-field='datetime']").each(function () {
                    $(this).unbind("click");
                    //alert("This is ID of my concern element." + $(this).attr("id"));
                });



                //// $("#dtBox").DateTimePicker();
                ////$("#dtBoxC").DateTimePicker();
                //alert("Yes we have done with assignment.");
                //$("#tabs").tabs({
                //    show: function () {
                //        //if (selTab != undefined) {
                //        //    $('#tabs div[id=tabPending' + selTab + ']').find("input[id ^= 'ContentPlaceHolder1_bntCalFromGrid_']").click();
                //        //}
                //        //get the selected tab index on partial postback  
                //        selTab = $('#tabs').tabs('option', 'selected');
                //    }, selected: selTab
                //});

                $("#tabs").tabs({
                    show: function () {

                        //get the selected tab index on partial postback  
                        var oldselTab = selTab
                        selTab = $('#tabs').tabs('option', 'selected');
                        if (oldselTab != undefined && oldselTab != selTab) {
                            $('#tabs div[id=tabPending' + oldselTab + ']').find("input[id ^= 'ContentPlaceHolder1_bntCalFromGrid_']").click();
                        }
                    }, selected: selTab
                });

                $("#doctabs").tabs({
                    show: function () {

                        //get the selected tab index on partial postback  
                        var docoldselTab = docselTab
                        docselTab = $('#doctabs').tabs('option', 'selected');
                        if (docoldselTab != undefined && docoldselTab != docselTab) {
                            $('#doctabs div[id=doctabPending' + docoldselTab + ']').find("input[id ^= 'ContentPlaceHolder1_bntCalFromGrid_']").click();
                        }
                    }, selected: docselTab
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




            // HCL Vendor Invoice VP===================== 
            $("#ContentPlaceHolder1_fld27281").bind("change", function () {

                ValidateWorkingDayForHCL();

            });
            $("#ContentPlaceHolder1_fld27347").bind("change", function () {

                ValidateWorkingDayForHCL();

            });
            //============================================= 


            $(document).ready(function () {
                var documentCanaraController = new DocumentCanaraController();
            });

            $(document).ready(function () {
                var documentCanaraHSBCController = new DocumentCanaraHSBCController();
            });















            // end of pageload()
        };










        //// HCL Vendor Invoice VP=====================
        function ValidateWorkingDayForHCL() {
            //alert("hi");
            var EmpCode = $("#ContentPlaceHolder1_HDN27200").val();
            var Month = $("#ContentPlaceHolder1_fld27281").val();
            var WorkingDays = $("#ContentPlaceHolder1_fld27347").val();
            var monthText = $("#ContentPlaceHolder1_fld27281 option:selected").text();
            $("#ContentPlaceHolder1_fld27347,#ContentPlaceHolder1_fld27281").css('background-color', '');
            if (EmpCode != "") {
                if (Month != "0") {
                    var currentYear = (new Date).getFullYear();
                    var currentMonth = (new Date).getMonth() + 1;
                    var arrMonth = monthText.split("-")
                    var m = GetMonth(arrMonth[0]);
                    //alert(currentYear + "|" + currentMonth + "|" + monthText);
                    if ((m <= currentMonth || (arrMonth[1] < currentYear)) && arrMonth[1] <= currentYear) {
                        var t = '{ EmpCode:"' + EmpCode + '",Month:"' + Month + '" }';
                        $.ajax({
                            type: "POST",
                            url: "Documents.aspx/CheckWorkingDayForHCLVendorInvVP",
                            contentType: "application/json; charset=utf-8",
                            data: t,
                            dataType: "json",
                            success: function (response) {
                                var result = response.d;
                                if (result != "") {
                                    var dsc = JSON.parse(result);
                                    if (dsc.length > 0) {
                                        alert("Invoice for selected Month is already Submitted!");
                                        $("#ContentPlaceHolder1_fld27342").attr("checked", true);
                                        $("#ContentPlaceHolder1_fld27281").css('background-color', 'red');
                                        if (WorkingDays > 15) {
                                            alert("Only Supplimentary Invoice can be submitted for Duplicate Invoice and Working Days must be less than 15.");
                                            $("#ContentPlaceHolder1_fld27347").css('background-color', 'red');
                                            $("#ContentPlaceHolder1_fld27281").val("0");
                                        }
                                    }
                                    else
                                        $("#ContentPlaceHolder1_fld27342").attr("checked", false);
                                }
                                if (result > 0) {
                                    alert("Invoice Already submitted for selected period");
                                    $(txtdate).css('background-color', 'red');
                                }
                                else {

                                    $(txtdate).css('background-color', '');
                                }
                            },
                            error: function (data) {
                                //Code For hiding preloader
                            }
                        });
                    }
                    else {
                        alert("Invoice for Future Month is Not Allowed!");
                        $("#ContentPlaceHolder1_fld27281").val("0");
                    }
                }
            }
            else {
                alert("Please select consultant emp code.");
                $("#ContentPlaceHolder1_fld27200").focus();
            }
        }
        //====================================================
        // for hcl - vendor invoice vp -  future month not allowed in retainer type invoce 
        function GetMonth(month) {
            var m = 0;
            if (month == ("January").toUpperCase())
                m = 1;
            else if (month == ("February").toUpperCase())
                m = 2;
            else if (month == ("March").toUpperCase())
                m = 3;
            else if (month == ("April").toUpperCase())
                m = 4;
            else if (month == ("May").toUpperCase())
                m = 5;
            else if (month == ("June").toUpperCase())
                m = 6;
            else if (month == ("July").toUpperCase())
                m = 7;
            else if (month == ("August").toUpperCase())
                m = 8;
            else if (month == ("September").toUpperCase())
                m = 9;
            else if (month == ("October").toUpperCase())
                m = 10;
            else if (month == ("November").toUpperCase())
                m = 11;
            else if (month == ("December").toUpperCase())
                m = 12;
            return m;
        }
        //============================================================================



















        function ValidateForm() {
            var FileName = "";
            FileName = $("#ContentPlaceHolder1_FUPChilditem").val();
            var IsValid = true;
            //alert('Here is File Name' + FileName);
            var ErrorMsg = "Error(s) in your submission.\n--------------------------------------\n";
            if (FileName == "") {
                ErrorMsg = ErrorMsg + "CSV file required.\n";
                IsValid = false;
            }
            else {
                var fileExtension = ['csv', 'CSV'];
                if ($.inArray(FileName.split('.').pop().toLowerCase(), fileExtension) == -1) {
                    ErrorMsg = ErrorMsg + "A valid CSV file required.\n";
                    IsValid = false;
                }
            }
            //ContentPlaceHolder1_CSVUploader
            if (IsValid == false)
                alert(ErrorMsg);
            return IsValid;
        }

        $(document).ready(function () {
            UtilJs.PreservFile();
        });


    </script>


    <script type="text/javascript">
        var nav = window.Event ? true : false;
        if (nav) {
            window.captureEvents(Event.KEYDOWN);
            window.onkeydown = NetscapeEventHandler_KeyDown;
        } else {
            document.onkeydown = MicrosoftEventHandler_KeyDown;
        }

        function NetscapeEventHandler_KeyDown(e) {
            if (e.which == 13 && (e.target.type == "undefined" || e.target.type == null)) {
                return true;
            }
            if (e.which == 13 && e.target.type != 'textarea' && e.target.type != 'submit') {
                return false;
            }

            return true;
        }

        function MicrosoftEventHandler_KeyDown() {
            if (event.keyCode == 13 && (event.srcElement.type == "undefined" || event.srcElement.type == null)) {
                return true;
            }
            if (event.keyCode == 13 && event.srcElement.type != 'textarea' &&
            event.srcElement.type != 'submit')
                return false;

            return true;
        }
    </script>

    <div class="row">
 	<div class="col-md-4"><img src="./Submit Request_files/m1-logo1.png" alt=""></div>
        <div class="ms-navbar-brand col-md-4"><center>Welcome to Mynd Solutions</center></div>
   </div>

<%--<span style="background-color:red;">--%>
 <div class="container" style="text-align:center" ><!-- container class is used to centered  the body of the browser with some decent width-->

      
<div class="row"><!-- row class is used for grid system in Bootstrap-->
          
<div class="col-md-6 col-md-offset-3"><!--col-md-4 is used to create the no of colums in the grid also use for medimum and large devices-->
              
<div class="login-panel panel panel-primary">
                  
<div class="panel-heading">
                      
<h3 class="panel-title">Submit Request</h3>
                  
</div>
                  
<div class="panel-body">
<div id="snackbar">Some text some message..</div>




    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: fixed; z-index: 9999999; left: 40%; top: 50%">
                <asp:Image ID="Image1" Width="80px" Height="80px" runat="server" ImageUrl="~/Images/prg.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <%--For Show history popup--%>
    <asp:Button ID="btnShowPopupHistory" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnShowPopupHistory_ModalPopupExtender" runat="server" PopupControlID="pnlShowPopupHistory" TargetControlID="btnShowPopupHistory"
        CancelControlID="btnCloseShowPopupHistory" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlShowPopupHistory" runat="server" Width="1000px" Height="300px" BackColor="White" Style="">
        <div class="box">
            <table cellspacing="1px" cellpadding="1px" width="100%">
                <tr>
                    <td width="980px">
                        <asp:HiddenField ID="hdnCurrentMainTab" runat="server" />
                        <h3>Trnsactional History </h3>
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
                                    <asp:GridView ID="grdShowPopupHistory" DataKeyNames="TID" Width="100%" runat="server" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" AutoGenerateColumns="true">
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
    <%--For Show history popup--%>
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0">
        <tr>
            <td style="width: 100%;" valign="top" align="left">
                <div class="container-fluid">
                    <div id="main" style="min-height: 400px">

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <%--  <asp:PostBackTrigger ControlID ="btnimmm"  />--%>
                                <%-- <asp:PostBackTrigger ControlID="helpexport" />--%>
                                <%--<asp:PostBackTrigger ControlID="btnimport" />--%>
                            </Triggers>
                            <ContentTemplate>

                                <div class="form" style="text-align: left">
                                    <asp:Label ID="lblCaption" runat="server"></asp:Label>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTab" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%--Added for Shwo Existing values START HERE --%>
                                                <div style="border: solid skyblue; text-align: right; padding-right: 1%;" id="ShowExist" runat="server" visible="false">
                                                    <asp:Button ID="btnShowExist" runat="server" CssClass="btnNew" OnClick="btnShowExist_Click" />
                                                </div>
                                                <%--Added for Shwo Existing values START HERE --%>

                                                <asp:Panel ID="pnlFields" runat="server" Width="100%" CssClass="content_form" >

                                                    <div id="dtBox">
                                                    </div>
                                                </asp:Panel>
                                                <div style="width: 100%; text-align: right">

                                                    <asp:Button ID="btnDraft" runat="server" Text="DRAFT" Visible="false" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClick="DraftDoc" />
                                                    <asp:Button ID="Button1" runat="server" Text="Calculate" Visible="false" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClick="CalculateFormulaP" />
                                                    <asp:Button ID="btnActEdit" runat="server" Text="Save" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                                        CssClass="btnNew" Font-Bold="True"
                                                        Font-Size="X-Small" Width="100px" TabIndex="5" />
                                                </div>
                                            </td>
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
    <asp:Panel ID="pnlMsg" runat="server" Visible="false">
        <asp:Label ID="lbview" runat="server" Text="You don't have write to Create Document !" ForeColor="Red" Font-Bold="true"></asp:Label>
    </asp:Panel>
    <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm" TargetControlID="btnShowPopupForm"
        BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupForm" runat="server" Width="400px" Height="100px" BackColor="White" Style="display: none">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td style="width: 400px">
                      <asp:UpdatePanel ID="UpdatePaneHeader" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                               <h3 id="panelHeaderConfimation" runat="server"></h3>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updMsg" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblMsg" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>&nbsp;
                                <asp:Label ID="lblxmlmsg" runat="server" Font-Bold="true" ForeColor="navy" Font-Size="Small"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnOk" runat="server" Text="Close" OnClick="Reset" CssClass="btnNew" Width="80px" />
                        <asp:Button ID="btnview" runat="server" Text="View" CssClass="btnNew" OnClick="OpenWindow" Width="80px" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="Btnchild" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server"
        TargetControlID="Btnchild" PopupControlID="pnlPopupchild"
        CancelControlID="btnClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupchild" runat="server" Width="800px" Height="300px" ScrollBars="Auto" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 780px">
                        <asp:UpdatePanel ID="updChildFormname" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="width: 760px">
                                    <h3>
                                        <asp:Label ID="lblChildFormName" runat="server" Font-Bold="True"></asp:Label></h3>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 20px; text-align: Right">
                        <asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ImageButton ID="btnimmm" align="right" ToolTip="Import" runat="server" Height="22px" Width="90px" Visible="false" ImageUrl="~/Images/upload.png" />
                                <asp:Panel ID="Pnllable" runat="server">
                                    <asp:Label ID="lblTab1" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                    <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </asp:Panel>
                                <asp:Panel ID="pnlFields1" Width="100%" runat="server">
                                </asp:Panel>
                                <div id="dtBoxC"></div>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnCalculate" runat="server" Text="Calculate" CssClass="btnNew" Visible="false" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClick="CalculateFormulaC" />
                                    <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <%--Added Pop for Show Existing Values--%>
    <asp:Button ID="btnshow" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_show" runat="server"
        TargetControlID="btnshow" PopupControlID="pnlShow"
        CancelControlID="btnShowClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlShow" runat="server" Width="800px" Height="300px" ScrollBars="Auto" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 780px">
                        <asp:UpdatePanel ID="UP_Show" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="width: 760px">
                                    <h3>
                                        <asp:Label ID="lblShow" runat="server" Font-Bold="True"></asp:Label></h3>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 20px; text-align: Right">
                        <asp:ImageButton ID="btnShowClose" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UP_Show1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="pnltrgGrd" runat="server" Height="263px" ScrollBars="Auto" Width="100%">
                                    <div style="padding-left: 2%; padding-right: 2%; width: 760px;">
                                        <asp:GridView ID="grdShow" runat="server" ssClass="GridView"
                                            CellPadding="2" DataKeyNames="DOCID">
                                            <FooterStyle CssClass="FooterStyle" />
                                            <RowStyle CssClass="RowStyle" />
                                            <EditRowStyle CssClass="EditRowStyle" />
                                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                                            <PagerStyle CssClass="PagerStyle" />
                                            <HeaderStyle CssClass=" HeaderStyle" />
                                            <AlternatingRowStyle CssClass="AlternatingRowStyle" Width="100%" />
                                        </asp:GridView>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <%--Added Pop for Show Existing Values--%>
    <asp:Button ID="BtnAdd" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="AddTask_ModalPopUp" runat="server"
        TargetControlID="BtnAdd" PopupControlID="pnlAddTask" CancelControlID="TaskClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlAddTask" runat="server" Width="500px" Height="330px" ScrollBars="Auto" Style="display: none" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpdPnlAddTask" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 480px">
                                <h3>Manage Task</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="TaskClose" ImageUrl="images/close.png" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td align="left" colspan="4">
                                                <asp:Label ID="Label1" runat="server" Text="Please select the field"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label>User Name:</label></td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddluser" runat="server" Width="150px" CssClass="txtBox"></asp:DropDownList>
                                            </td>
                                            <td align="left" valign="top">
                                                <label>Due Date:</label></td>
                                            <td align="left">
                                                <asp:TextBox ID="txtDue_Date" runat="server" CssClass="txtBox"></asp:TextBox>
                                                <asp:CalendarExtender ID="txtCLNDR" runat="server" TargetControlID="txtDue_Date"></asp:CalendarExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label>Remarks:</label>
                                            </td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtRemarks" TextMode="MultiLine" Width="100%" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="4">
                                                <asp:Button ID="BTNTask" runat="server" CssClass="btnNew" Text="Add User" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" /></td>
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

    <asp:Button ID="hdnbtnChldUpload" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mdlChilditemUpload" runat="server"
        TargetControlID="hdnbtnChldUpload" PopupControlID="pnlChildUpload" CancelControlID="ImgChldUpClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlChildUpload" runat="server" Width="500px" ScrollBars="Auto" BackColor="white" Style="display: none;">
        <div style="border: 2px solid #e1e1e1">
            <asp:UpdatePanel ID="UpChildItemUploader" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="upSample" />
                    <asp:PostBackTrigger ControlID="btnUpload" />
                </Triggers>
                <ContentTemplate>
                    <table style="width: 100%;" cellpadding="2px" cellspacing="2px">
                        <tr style="border: 2px solid #e2e2e2; background-color: #e3e3e3; height: 25px;">
                            <td width="80%">
                                <b>
                                    <asp:Label ID="lblChldUpCaption" runat="server"></asp:Label></b>
                            </td>
                            <td width="20%">
                                <asp:ImageButton ID="ImgChldUpClose" ImageUrl="images/close.png" runat="server" OnClick="childClose" />
                            </td>
                        </tr>
                        <tr height="10px">
                            <td colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td width="100%">
                                <table width="100%">
                                    <tr>
                                        <td colspan="3"></td>
                                    </tr>
                                    <tr>
                                        <td>Select a File:
                                        </td>
                                        <td>
                                            <asp:FileUpload runat="server" ID="FUPChilditem" CssClass="txtBox" ToolTip="Select a file to upload.Please select .CSV File only. " />
                                            <asp:Button ID="upSample" runat="server" CssClass="btnDyn" Text="Download Sample" ToolTip="Download formated .CSV file." OnClick="helpexport_Click" />
                                        </td>
                                    </tr>
                                    <tr height="10px">
                                        <td colspan="3">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btnDyn" OnClientClick="javascript:return ValidateForm();" OnClick="btnUpload_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
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

    <div id="loading-div-background">
        <div id="loading-div" class="ui-corner-all">
            <img style="height: 20px; width: 20px; margin-top: 5px;" src="../images/attch.gif" alt="Uploading... Please wait" />
            <h2 style="color: black; font-weight: normal;">Please wait....</h2>
        </div>
    </div>

    <asp:Button ID="btndraftpopup" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="popupDraftmsg" runat="server" PopupControlID="pnldraftpopup" TargetControlID="btndraftpopup"
        BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnldraftpopup" runat="server" Width="400px" Height="100px" BackColor="White" Style="display: none">
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
                        <asp:UpdatePanel ID="upddraftmsg" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lbldraftmsgpop" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>&nbsp;
                                <%--<asp:Label ID="Label4" runat="server" Font-Bold="true" ForeColor="navy" Font-Size="Small"></asp:Label>--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnokdoc" runat="server" Text="Close" OnClick="Reset" CssClass="btnNew" Width="80px" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

</asp:Content>
