<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="ReallocationRightsNew, App_Web_mgyyj1iz" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src='http://code.jquery.com/jquery-latest.min.js' type='text/javascript'> </script>

    <link rel="stylesheet" href="kendu/homekendo.common.min.css" />
    <link rel="stylesheet" href="kendu/homekendo.rtl.min.css" />
    <%--<link rel="stylesheet" href="kendu/homekendo.silver.min.css" />--%>
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="kendu/homekendo.mobile.all.min.css" />

    <%--kendo.data.min.js--%>
    <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <%--<script src="kendu/homejquery-1.9.1.min.js"></script>--%>

    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>

    <%--<script src="kendu/content/shared/js/console.js"></script>
    <%--<script src="kendu/js/jszip.min.js"></script>--%>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>

    <%--<script type="text/javascript">
        function SelectAllCheckboxes1(chk) {
            $('#<%=gvData.ClientID%>').find("input:checkbox").each(function () {
                if (this != chk) { this.checked = chk.checked; }
            });
        }
    </script>--%>
    <style>
         .k-grid-toolbar a {
            float: right;
        }
        .error {
            border: 1px solid red !important;
        }
    </style>
    <div class="form" style="text-align: left">
        <div class="doc_header">
            Document Reallocation
        </div>
                <br />  
                <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>Document Type</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <select id="ddldoctype" class="txtBox">
                            <option value="">-Select-</option>
                        </select>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Status</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <select id="ddlstatus" class="txtBox">
                            <option value="">-Select-</option>
                        </select>
                    </div>
                </div><br />

                <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>Filter Target User</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <select id="ddltu" class="txtBox">
                            <option value="">-Select-</option>
                        </select>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Reallocation Remarks</label>
                    </div>
                    <div class="col-md-3 col-sm-3" >
                        
                         <input type="text" id="TXTReamrks" class="txtBox" maxlength="100" />
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <input type="button" id="btnsearch" value="Search" class="btnNew" />
                        
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-sm-12">
                        <br /><br />
                        <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                        </div>
                      
                        <div id="dvSelectTargetUser" class="col-md-3 col-sm-3" style="float:left; display:none; width:70%;">
                        <input type="button" id="btnsearch1" value="Copy Target User To Selected Rows" class="btnNew" style="float:right;"/>
                    </div>
                         <%-- 'preeti'--%>
                         <div id="dvSelectTargetUser1" class="col-md-3 col-sm-3" style="display:none;">
                        <input type="button" id="btnREMARKS" value="Copy Reallocation Remarks To Selected Rows" class="btnNew" style="float:right;"/>
                    </div>
                    <%--    'preeti'--%>
                        <div class="clear"></div>
                        <div id="ReallocationKGrid"></div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12" style="text-align: center;">
                        <div id="dvloaderSave" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                <input type="image" src="../images/preloader22.gif" />
                        </div>

                        <input type="button" id="btnsave" value="Re-Allocate" class="btnNew" style="display:none;" />
                        <%--<asp:Button ID="btnsave" runat="server" Text="Re-Allocate" Width="150px" Visible="false" CssClass="btnNew" />--%>
                    </div>
                </div>
            </div>
    <script type="text/javascript">


        $(document).ready(function () {
            BindDocumentType();
            $("#ddldoctype").change(function () {
                $("#dvloader").show();
                BindStatus();
            });
            $("#btnsearch").click(function () {
                BindGrid();
            });
            $("#ddlstatus").change(function () {
                $("#dvloader").show();
                //BindCurrentUser();
                BindTargetUsers();
                Remarksfield();
            });
            //$("#ddlcu").change(function () {
            //    $("#dvloader").show();
            //    BindTargetUsers();
            //});
            //BindTargetUsers
            $("#btnsearch1").click(function () {
                var TargetUser = $("#ddltu").val();
                if (TargetUser == "") {
                    alert("Please select target user");
                    return;
                }
                var grid = $("#ReallocationKGrid").data("kendoGrid");
                var ds = grid.dataSource.data();
                for (var i = 0; i < ds.length; i++) {
                    var row = grid.table.find("tr[data-uid='" + ds[i].uid + "']");
                    var checkbox = $(row).find(".check-box-inner");
                    var DelayReason = $(row).find(".DelayReason").val();
                    if (checkbox.is(":checked")) {
                        $(row).find(".DelayReason").val(TargetUser);
                    }
                    else {
                        $(row).find(".DelayReason").val("0");
                    }
                }
            });

           /* preeti start*/
            $("#btnREMARKS").click(function () {
                var Remarks = $("#TXTReamrks").val();
                if (Remarks == "") {
                    alert("Please enter Remarks");
                    return;
                }
                var n = $("input:checked").length;
                if (n == "0") {

                    alert("First Select Lines");
                    return;
                }
                var grid = $("#ReallocationKGrid").data("kendoGrid");
                var ds = grid.dataSource.data();
               
                for (var i = 0; i < ds.length; i++) {
                    var row = grid.table.find("tr[data-uid='" + ds[i].uid + "']");
                    var checkbox = $(row).find(".check-box-inner");
                   
                    var DelayReason = $(row).find(".DelayReason1").val();

                  
                    if (checkbox.is(":checked")) {
                        $(row).find(".DelayReason1").val(Remarks);
                    }
                    else {
                       
                       
                        $(row).find(".DelayReason1").val("");
                       /* $(row = grid.table.find("tr[data-uid='" + ds[i].Remarks + "']");*/
                        /* Right  $(row).find(".DelayReason1").val("'" + ds[i].Remarks + "'");*/

                    }
                }
            });
            /*preeti end */
        });
        var Kgrid = "";
        function BindGrid() {
            $("#dvSelectTargetUser").hide();
            $("#dvSelectTargetUser1").hide();
            $("#dvloader").show();
            $("#ddldoctype,#ddlstatus").removeClass("error");
            var ddlStatus = $("#ddlstatus").val() == null ? "" : $("#ddlstatus").val();
            var ddlDocumentType = $("#ddldoctype").val();
            var ddlcu = $("#ddlcu").val();
            var remarks = $("#TXTReamrks").val();
            var ddltu = $("#ddltu").val() == "" ? 0 : $("#ddltu").val();
            if (ddlDocumentType != "" && ddlStatus != "") {
                var curText = "";//$("#ddlcu option:selected").text() == "-- Select --" ? "" : $("#ddlcu option:selected").text();
                var t = '{ documentType: "' + ddlDocumentType + '",status:"' + ddlStatus + '",currUser:"' + curText + '" }';
                $.ajax({
                    type: "POST",
                    url: "ReallocationRightsNew.aspx/getresult",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        if (response.d.Data != "") {
                            var strData = JSON.parse(response.d.Data);
                            var columns = response.d.Column;
                            var CommanObj = { title: "<input id='chkAll' class='checkAllCls' type='checkbox'/>", width: "35px", template: "<input type='checkbox' class='check-box-inner' />", filterable: false };
                            var ddlobj = { field: "NextUser", title: "Target User", width: 150, template: "<select id='ddlNextUser' class='dropDownTemplate DelayReason' style='width:300px;'></select>", filterable: false };
                            var ddlobj1 = { field: "Remarks", title: "Reallocation Remarks", width: 150, template: "<input id='ddlTXTtextbox'  class='TextTemplate DelayReason1' style='width:100px;'></input>", filterable: false };
                            //$("#ddlTXTtextbox").val(this.Remarks)
                          

                            columns.splice(0, 0, CommanObj);
                            //columns.splice(0, 1, ddlobj);
                            columns.push(ddlobj, ddlobj1);
                            //if (strData.length > 0) {
                            if (Kgrid) {
                                $('#ReallocationKGrid').kendoGrid('destroy').empty();
                            }
                            //var data = $.parseJSON(response.d.Data);
                            Kgrid = $("#ReallocationKGrid").kendoGrid({
                                dataSource: {
                                    data: strData,
                                    pageSize: 10,
                                    pageable: {
                                        pageSize: 10
                                    }

                                },
                                columns: columns,
                                pageable: { pageSizes: true },
                                resizable: true,
                                filterable: true,
                                noRecords: true,
                                columnMenu: true,
                                scrollable: false,
                                sortable: {
                                    mode: "multiple"
                                },
                                toolbar: ['excel'],
                                excel: {
                                    fileName: "ReallocationRights.xlsx",
                                    filterable: true,
                                    pageable: true,
                                    allPages: true
                                },
                                //persistSelection: true,
                                sortable: true,
                                change: onChange,
                                dataBound: function (e) {
                                    var grid = $("#ReallocationKGrid").data("kendoGrid");
                                    var gridData = grid.dataSource.view();
                                    if (gridData.length > 0) {
                                        for (var i = 0; i < gridData.length; i++) {
                                            var currentUid = gridData[i].uid;
                                            var Remarks = gridData[i].Remarks;
                                           
                                            var docid = gridData[i].DocID;
                                            var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                                            var ddlStatus = $("#ddlstatus").val();
                                            var ddlDocumentType = $("#ddldoctype").val();
                                          
                                            var t = '{docid:' + docid + ', documentType: "' + ddlDocumentType + '",status:"' + ddlStatus + '"}';
                                            $.ajax({
                                                type: "POST",
                                                url: "ReallocationRightsNew.aspx/getNextUser",
                                                contentType: "application/json; charset=utf-8",
                                                data: t,
                                                dataType: "json",
                                                async: false,
                                                success: function (response) {
                                                    var strData = JSON.parse(response.d.Data);
                                                    var ddlNextUser = $(currentRow).find(".dropDownTemplate");

                                                    ddlNextUser.empty().append($('<option></option>').val("0").html("-- Select --"));
                                                    if (strData.length > 0) {
                                                        $.each(strData, function () {
                                                            ddlNextUser.append($('<option></option>').val(this.uid).html(this.username));
                                                           

                                                        });
                                                        $(ddlNextUser).val(ddltu);
                                                         /* preeti work2  $(ddlTXTtextbox).val(remarks); /*

                                                       
                                                       /* $("#ddlTXTtextbox").val(this.Remarks);*/
                                                        /*alert($("#ddlTXTtextbox").val(this.Remarks));*/
                                                        //ddltu
                                                    }
                                                    else {
                                                        ddlNextUser.empty().append($('<option></option>').val("0").html("-- Select --"));
                                                    }
                                                    $("#dvloader").hide();
                                                    $("#btnsave").show();
                                                    $("#dvSelectTargetUser").show();
                                                    $("#dvSelectTargetUser1").show();
                                                },
                                                error: function (data) {
                                                    //Code For hiding preloader
                                                }
                                            });
                                        }
                                    }
                                    else {
                                        $("#dvloader").hide();
                                        $("#btnsave").hide();
                                    }
                                }

                            });

                            if (Kgrid != "") {
                                $(".checkAllCls").on("click", function () {
                                    var ele = this;
                                    var state = $(ele).is(':checked');
                                    var grid = $('#ReallocationKGrid').data('kendoGrid');
                                    if (state == true) {
                                        $('.check-box-inner').prop('checked', true);
                                    }
                                    else {
                                        $('.check-box-inner').prop('checked', false);
                                       
                                    }
                                });

                                $("#btnsave").unbind().bind("click", function () {
                                    $("#dvloaderSave").show();
                                    var grid = $("#ReallocationKGrid").data("kendoGrid");
                                    var ds = grid.dataSource.data();
                                    var lstData = [];
                                    var arrError = [];
                                    for (var i = 0; i < ds.length; i++) {
                                        var row = grid.table.find("tr[data-uid='" + ds[i].uid + "']");
                                        var checkbox = $(row).find(".check-box-inner");
                                        var DelayReason = $(row).find(".DelayReason").val();
                                        var DelayReason1=$(row).find(".DelayReason1").val();
                                        if (checkbox.is(":checked")) {
                                            var idsToSend = { DocID: 0, NextUser: 0, fdate: "", ptat: "0", ordering: "0" };
                                            if (DelayReason == "0") {
                                                $(row).find(".DelayReason").addClass('error');
                                                arrError.push(false);
                                            }
                                            else {
                                                $(row).find(".DelayReason").removeClass('error');
                                                arrError.push(true);
                                            }
                                            idsToSend.DocID = ds[i].DocID;
                                            idsToSend.NextUser = DelayReason;
                                            idsToSend.Remarks = DelayReason1;
                                            idsToSend.fdate = ds[i].fdate;
                                            idsToSend.ptat = ds[i].ptat;
                                            idsToSend.ordering = ds[i].ordering;
                                            lstData.push(idsToSend);
                                        }
                                        else {
                                            $(row).find(".DelayReason").removeClass('error');
                                            arrError.push(true);
                                        }
                                    }
                                    if (lstData.length > 0) {
                                        if (arrError.length > 0) {
                                            for (var i = 0; i < arrError.length; i++) {
                                                if (arrError[i] == false) {
                                                    $("#dvloaderSave").hide();
                                                    return false;
                                                }
                                            }
                                        }

                                        //var t = '{ClsNextUser:"' + lstData + '"}';
                                        var ddlStatus = $("#ddlstatus").val();
                                        var t = { ClsNextUser: lstData, Status: ddlStatus };
                                        $.ajax({
                                            type: "POST",
                                            url: "ReallocationRightsNew.aspx/SaveNextUser",
                                            contentType: "application/json; charset=utf-8",
                                            data: JSON.stringify(t),
                                            dataType: "json",
                                            success: function (response) {
                                                var msg = response.d;
                                                alert(msg);
                                                $("#dvloaderSave").hide();
                                                window.location = 'ReallocationRightsNew.aspx';
                                            },
                                            error: function (data) {
                                                //Code For hiding preloader
                                            }
                                        });
                                    }
                                    else {
                                        alert("Please select at-least one document to proceed.");
                                        $("#dvloader").hide();
                                        $("#dvloaderSave").hide();
                                    }
                                });
                            }
                            //}
                            //else {

                            //}
                        }
                        else {
                            alert("No data found.");
                            $("#dvloader").hide();
                            $("#btnsave").hide();
                        }
                    },
                    error: function (data) {
                        //Code For hiding preloader
                    }
                });

            }
            else {


                if ($("#ddldoctype").val() == "" && $("#ddlstatus").val() == "") {
                    $("#ddldoctype,#ddlstatus").addClass("error");
                }
                else if ($("#ddldoctype").val() == "")
                    $("#ddldoctype").addClass("error");
                else if ($("#ddlstatus").val() == "")
                    $("#ddlstatus").addClass("error");

                $("#dvloader").hide();
                return false;
            }
        }

        function Remarksfield() {
            var ddlDocumentType = $("#ddldoctype").val();
            var ddlStatus = $("#ddlstatus").val();
            if (ddlDocumentType != "") {
                var t = '{ documentType: "' + ddlDocumentType + '",}';
            }
            if (ddlDocumentType != "" && ddlStatus != "") {
                var t = '{ documentType: "' + ddlDocumentType + '",status:"' + ddlStatus + '"}';
                $.ajax({
                    type: "POST",
                    url: "ReallocationRightsNew.aspx/Remarksdata",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        if (response.d.Data != "") {
                            var strData = JSON.parse(response.d.Data);
                            if (strData.length > 0) {
                                $.each(strData, function () {
                                    $("#TXTReamrks").val(this.Remarks);
                                });

                            }
                         
                            
                        }
                        else {
                           
                        }
                    },
                    error: function (data) {
                        //Code For hiding preloader
                    }
                
                });
            }
            else {
               
            }
        }
        function onChange(arg) {
            var ids = this.selectedKeyNames();
            $("#hdnTID").val(this.selectedKeyNames().join(", "));
        }
        function BindDocumentType() {
            var ddlDocumentType = $("#ddldoctype");
            var t = '{ documentType: ""}';
            $.ajax({
                type: "POST",
                url: "ReallocationRightsNew.aspx/GetDocumentType",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var strData = JSON.parse(response.d.Data);
                    ddlDocumentType.empty().append($('<option></option>').val("").html("-- Select --"));
                    if (strData.length > 0) {
                        $.each(strData, function () {
                            ddlDocumentType.append($('<option></option>').val(this.doctype).html(this.doctype));
                        });

                    }
                    else {
                        ddlDocumentType.empty().append($('<option></option>').val("").html("-- Select --"));
                        //$("#dvloader").hide();
                    }

                },
                error: function (data) {
                    //Code For hiding preloader
                }
            });
        }

        function BindStatus() {
            var ddlStatus = $("#ddlstatus");
            var ddlDocumentType = $("#ddldoctype").val();
            if (ddlDocumentType != "") {
                var t = '{ documentType: "' + ddlDocumentType + '"}';
                $.ajax({
                    type: "POST",
                    url: "ReallocationRightsNew.aspx/GetStatus",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        var strData = JSON.parse(response.d.Data);
                        ddlStatus.empty().append($('<option></option>').val("").html("-- Select --"));
                        if (strData.length > 0) {
                            $.each(strData, function () {
                                ddlStatus.append($('<option></option>').val(this.Status).html(this.Status));
                            });
                            $("#dvloader").hide();
                        }
                        else {
                            ddlStatus.empty().append($('<option></option>').val("").html("-- Select --"));
                            $("#dvloader").hide();
                        }

                    },
                    error: function (data) {
                        //Code For hiding preloader
                    }
                });
            }
            else {
                ddlStatus.empty().append($('<option></option>').val("").html("First select a document type..."));
                //$("#ddlcu").empty().append($('<option></option>').val("").html("First select a document type..."));
                $("#ddltu").empty().append($('<option></option>').val("").html("First select a document type..."));
                $("#dvloader").hide();
            }
        }

        function BindTargetUsers() {
            var ddltu = $("#ddltu");
            var ddlStatus = $("#ddlstatus").val();
            var ddlDocumentType = $("#ddldoctype").val();

            //var ddlcu = $("#ddlcu").val();
            if (ddlDocumentType != "" && ddlStatus != "") {
                var t = '{ documentType: "' + ddlDocumentType + '",status:"' + ddlStatus + '"}';
                $.ajax({
                    type: "POST",
                    url: "ReallocationRightsNew.aspx/GetTargetUsers",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        if (response.d.Data != "") {
                            var strData = JSON.parse(response.d.Data);
                            ddltu.empty().append($('<option></option>').val("").html("-- Select --"));
                            if (strData.length > 0) {
                                $.each(strData, function () {
                                    ddltu.append($('<option></option>').val(this.uid).html(this.username));
                                });
                                $("#dvloader").hide();
                            }
                            else {
                                ddltu.empty().append($('<option></option>').val("").html("-- Select --"));
                                $("#dvloader").hide();
                            }
                        }
                        else {
                            ddltu.empty().append($('<option></option>').val("").html("-- Select --"));
                            $("#dvloader").hide();
                        }
                    },
                    error: function (data) {
                        //Code For hiding preloader
                    }
                });
            }
            else {
                ddltu.empty().append($('<option></option>').val("").html("First select a document type..."));
                $("#ddltu").empty().append($('<option></option>').val("").html("First select a current user..."));
                $("#dvloader").hide();
            }
        }

        function BindCurrentUser() {
            var ddlcu = $("#ddlcu");
            var ddlStatus = $("#ddlstatus").val();
            var ddlDocumentType = $("#ddldoctype").val();
            if (ddlDocumentType != "" && ddlStatus != "") {
                var t = '{ documentType: "' + ddlDocumentType + '",status:"' + ddlStatus + '"}';
                $.ajax({
                    type: "POST",
                    url: "ReallocationRightsNew.aspx/GetCurrentUsers",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        var strData = JSON.parse(response.d.Data);
                        ddlcu.empty().append($('<option></option>').val("").html("-- Select --"));
                        if (strData.length > 0) {
                            $.each(strData, function () {
                                ddlcu.append($('<option></option>').val(this.UID).html(this.CurrentUser));
                            });
                            
                            $("#dvloader").hide();
                        }
                        else {
                            ddlcu.empty().append($('<option></option>').val("").html("-- Select --"));
                            $("#dvloader").hide();
                        }

                    },
                    error: function (data) {
                        //Code For hiding preloader
                    }
                });
            }
            else {
                $("#ddltu").empty().append($('<option></option>').val("").html("First select a status..."));
                $("#ddlcu").empty().append($('<option></option>').val("").html("First select a status..."));
                $("#dvloader").hide();
            }
        }

    </script>
</asp:Content>

