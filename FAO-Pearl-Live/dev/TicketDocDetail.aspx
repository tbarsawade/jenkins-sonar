<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketDocDetail.aspx.vb" Inherits="TicketDocDetail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        /*.pnlfields {
            height: 210px;
            overflow-x: hidden;
            overflow-y: scroll;
        }*/

        .mainform-scroll {
            height: 560px;
            overflow-x: hidden;
            overflow-y: scroll;
        }
    </style>

    <script type="text/javascript">
        function OnChangeCheckbox(checkbox) {
            Javascript: return window.open('' + checkbox + '', 'CustomPopUp', 'width=600, height=600, menubar=no, resizable=yes');
            return false;
        }
    </script>


    <meta charset="utf-8" />
    <title>Ticket Details</title>
    <meta http-equiv="X-UA-Compatible" content="IE=10,chrome=1" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta content="" name="description" />
    <meta content="" name="author" />
    <!-- BEGIN GLOBAL MANDATORY STYLES -->
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style-metro.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style-responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/themes/default.css" rel="stylesheet" type="text/css" id="style_color" />
    <link href="assets/plugins/uniform/css/uniform.default.css" rel="stylesheet" type="text/css" />
    <!-- END GLOBAL MANDATORY STYLES -->
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link href="assets/plugins/bootstrap-tag/css/bootstrap-tag.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/fancybox/source/jquery.fancybox.css" rel="stylesheet" />
    <link href="assets/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.css" rel="stylesheet" type="text/css" />
    <!-- BEGIN:File Upload Plugin CSS files-->
    <%-- <link href="assets/plugins/jquery-file-upload/css/jquery.fileupload-ui.css" rel="stylesheet" type="text/css">--%>
    <!-- END:File Upload Plugin CSS files-->
    <%-- <link href="assets/css/pages/inbox.css" rel="stylesheet" type="text/css" />--%>
    <!-- END PAGE LEVEL STYLES -->
    <link rel="shortcut icon" href="favicon.ico" />



    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <link rel="Stylesheet" href="css/fonts/font-awesome.min.css" />
    <link rel="Stylesheet" href="css/fonts/bootstrap.min.css" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <style type="text/css">
        .wrapContent {
            width: 650px;
            height: auto;
            margin: 0 auto;
        }

        .main-contend {
            width: auto;
            height: auto;
            border: 1px solid #ccc;
            display: inline-block;
            margin: 0px;
            padding: 12px;
        }

            .main-contend .btn-group {
                border: 1px solid #ccc;
                padding: 7px;
                display: inline-block;
                text-align: center;
            }

        .btn-group #crossbtn {
            display: block;
            font-size: 17px;
            line-height: 16px;
            width: 100px;
            padding: 3px;
            border: 1px solid #ccc;
            cursor: pointer;
            overflow: visible;
            background-color: #CCCCCC;
        }

            .btn-group #crossbtn a {
                font-size: 17px;
                line-height: 16px;
                text-decoration: none;
                color: #333333;
            }

        .icon {
            font-size: 18px;
            margin-left: 15px;
            position: relative;
            top: 1px;
            color: #333333;
        }
    </style>
    <script type="text/javascript">


        var dataValues = [];
        function uploadImage() {


            var fileUpload = $("#file_upload").get(0);
            var files = fileUpload.files;

            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            $.ajax({
                url: "TicketFileUploaderHandler.ashx",
                type: "POST",
                data: data,
                contentType: false,
                processData: false,

                success: function (e) {
                    debugger;
                    dataValues.push($.parseJSON(e).result);
                    if (dataValues.length > 0) {
                        $("#lblMessage").html('');
                        for (var i = 0; i < dataValues.length; i++) {
                            var path = dataValues[i].split("|");
                            $("#hdnSpan").val(i);
                            $("#lblMessage").append("<span id=spn" + $("#hdnSpan").val() +
                           "><a href='#'   style='color:red; border:solid; border-color:gray; font-size:10px;font-family:Lucida Console;'" +
                           " onclick=\"return DeleteFile('" + path[0].trim() + "','spn" + $("#hdnSpan").val() + "');\">x</a>" + path[1].toString() + " Saved Successfully<br></span>");
                        }
                        $("#hdnUploadedFileName").val('');
                        $("#hdnUploadedFileName").val(dataValues.join(","));
                        alert($("#hdnUploadedFileName").val());
                    }
                },
                error: function (err) {
                    alert(err.statusText)
                }
            });


            //$('#btnUpload').click();

            //var data = UtilJs.UploadFile(this, 'dnf28049', 'dnf28049');
            //alert(data);
            //var file = $("#file_upload")[0].files[0];
            //var name = file.name;
            //var size = file.size;
            //var type = file.type;
            //if (file.name.length < 1) {
            //    return false;;
            //}
            //else if (file.size > 10000000) {
            //    alert('Failed to upload image<span class=\"errorText\"><br/>Selected file is greater than 10MB.</span>');
            //    return false;;
            //}
            //alert(name + size + type);
            //var formData = new FormData();
            //formData.append("name", name);
            //formData.append("size", size);
            //formData.append("type", type);
            //formData.append("file", file);
            //$.ajax({
            //    type: "POST",
            //    url: 'TicketDocDetail.aspx/UploadFile',
            //    data: formData,
            //    cache: false,
            //    contentType: false,
            //    processData: false,
            //    success: function (data) {
            //        data = data.d;
            //        //if (data == -2) {
            //        //    toastr.error('Error', 'Failed to upload image<span class=\"errorText\"><br/>You do not have the required permissions to upload images.</span>');
            //        //} else if (data == -3) {
            //        //    toastr.error('Error', 'Failed to upload image<span class=\"errorText\"><br/>There was a fatal error. Please contact support.</span>');
            //        //} else {
            //        //    toastr.success('Success', 'Image Id <b>' + data + '</b> was saved successfully.');
            //        //}
            //    },
            //    error: function (err) {
            //        toastr.error('Error', 'Failed to upload image<span class=\"errorText\"><br/>There was a fatal error. Please contact support.</span>');
            //    }
            //});

        }
        function selectFile() {
            $('#file_upload').click();
        }
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

        .loading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
    </style>
    <script type="text/javascript">
        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 200);
        }
    </script>
    <script type="text/javascript">

        function DeleteFile(classname, spanID) {
            if (confirm("are you sure you want to delete this file " + classname)) {
                var t = '{deletePathfilename:"DOCS/TEMP/' + classname + '"}';
                $.ajax({
                    type: "POST",
                    url: "TicketDocDetail.aspx/DeleteFile",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        debugger;
                        if (response.d == 'TRUE') {
                            var temp = [];
                            if (dataValues.length > 0) {
                                $("#lblMessage").html('');
                                for (var i = 0; i < dataValues.length; i++) {
                                    var path = dataValues[i].split("|");
                                    if (path[0].toString() == classname) {
                                        temp.push(dataValues[i]);
                                        //dataValues.splice($.inArray(dataValues[i], dataValues), 1);
                                    }
                                    else {
                                        $("#hdnSpan").val(i);
                                        $("#lblMessage").append("<span id=spn" + $("#hdnSpan").val() +
                                       "><a href='#'   style='color:red; border:solid; border-color:gray; font-size:10px;font-family:Lucida Console;'" +
                                       " onclick=\"return DeleteFile('" + path[0].trim() + "','spn" + $("#hdnSpan").val() + "');\">x</a>" + path[1].toString() + " Saved Successfully<br></span>");
                                    }
                                }
                                for (var j = 0; j < temp.length; j++) {
                                    dataValues.splice($.inArray(temp[j], dataValues), 1);
                                }
                                $("#hdnUploadedFileName").val('');
                                $("#hdnUploadedFileName").val(dataValues.join(","));
                            }
                        }
                    },
                    error: function (data) {
                    }
                });
            }
        }
    </script>
</head>
<body class="page-header-fixed" data-spy="scroll" style="overflow: hidden;">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="scr" runat="server"></asp:ScriptManager>
        <div class="loading" align="center">
            Loading. Please wait.<br />
            <br />
            <img src="images/loader.gif" alt="" />
        </div>
        <!-- BEGIN HEADER -->
        <div class="header navbar navbar-inverse navbar-fixed-top">
            <!-- BEGIN TOP NAVIGATION BAR -->
            <div class="navbar-inner">
                <div class="container-fluid">
                    <!-- BEGIN LOGO -->
                    <a class="brand" href="index.html">
                        <img id="imageSource" runat="server" src="assets/img/logo.png" alt="logo" />
                    </a>

                    <!-- END LOGO -->
                    <!-- BEGIN RESPONSIVE MENU TOGGLER -->
                    <a href="javascript:;" class="btn-navbar collapsed" data-toggle="collapse" data-target=".nav-collapse">
                        <img src="assets/img/menu-toggler.png" alt="" />
                    </a>
                    <!-- END RESPONSIVE MENU TOGGLER -->
                    <!-- BEGIN TOP NAVIGATION MENU -->
                    <%--  <ul class="nav pull-right">
                        <!-- BEGIN NOTIFICATION DROPDOWN -->
                        <li class="dropdown" id="header_notification_bar">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                <i class="icon-warning-sign"></i>
                                <span class="badge">6</span>
                            </a>
                            <ul class="dropdown-menu extended notification">
                                <li>
                                    <p>You have 14 new notifications</p>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="label label-success"><i class="icon-plus"></i></span>
                                        New user registered. 
								<span class="time">Just now</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="label label-important"><i class="icon-bolt"></i></span>
                                        Server #12 overloaded. 
								<span class="time">15 mins</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="label label-warning"><i class="icon-bell"></i></span>
                                        Server #2 not respoding.
								<span class="time">22 mins</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="label label-info"><i class="icon-bullhorn"></i></span>
                                        Application error.
								<span class="time">40 mins</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="label label-important"><i class="icon-bolt"></i></span>
                                        Database overloaded 68%. 
								<span class="time">2 hrs</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="label label-important"><i class="icon-bolt"></i></span>
                                        2 user IP blocked.
								<span class="time">5 hrs</span>
                                    </a>
                                </li>
                                <li class="external">
                                    <a href="#">See all notifications <i class="m-icon-swapright"></i></a>
                                </li>
                            </ul>
                        </li>
                        <!-- END NOTIFICATION DROPDOWN -->
                        <!-- BEGIN INBOX DROPDOWN -->
                        <li class="dropdown" id="header_inbox_bar">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                <i class="icon-envelope"></i>
                                <span class="badge">5</span>
                            </a>
                            <ul class="dropdown-menu extended inbox">
                                <li>
                                    <p>You have 12 new messages</p>
                                </li>
                                <li>
                                    <a href="inbox.html?a=view">
                                        <span class="photo">
                                            <img src="./assets/img/avatar2.jpg" alt="" /></span>
                                        <span class="subject">
                                            <span class="from">Lisa Wong</span>
                                            <span class="time">Just Now</span>
                                        </span>
                                        <span class="message">Vivamus sed auctor nibh congue nibh. auctor nibh
								auctor nibh...
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="inbox.html?a=view">
                                        <span class="photo">
                                            <img src="./assets/img/avatar3.jpg" alt="" /></span>
                                        <span class="subject">
                                            <span class="from">Richard Doe</span>
                                            <span class="time">16 mins</span>
                                        </span>
                                        <span class="message">Vivamus sed congue nibh auctor nibh congue nibh. auctor nibh
								auctor nibh...
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="inbox.html?a=view">
                                        <span class="photo">
                                            <img src="./assets/img/avatar1.jpg" alt="" /></span>
                                        <span class="subject">
                                            <span class="from">Bob Nilson</span>
                                            <span class="time">2 hrs</span>
                                        </span>
                                        <span class="message">Vivamus sed nibh auctor nibh congue nibh. auctor nibh
								auctor nibh...
                                        </span>
                                    </a>
                                </li>
                                <li class="external">
                                    <a href="inbox.html">See all messages <i class="m-icon-swapright"></i></a>
                                </li>
                            </ul>
                        </li>
                        <!-- END INBOX DROPDOWN -->
                        <!-- BEGIN TODO DROPDOWN -->
                        <li class="dropdown" id="header_task_bar">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                <i class="icon-tasks"></i>
                                <span class="badge">5</span>
                            </a>
                            <ul class="dropdown-menu extended tasks">
                                <li>
                                    <p>You have 12 pending tasks</p>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="task">
                                            <span class="desc">New release v1.2</span>
                                            <span class="percent">30%</span>
                                        </span>
                                        <span class="progress progress-success ">
                                            <span style="width: 30%;" class="bar"></span>
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="task">
                                            <span class="desc">Application deployment</span>
                                            <span class="percent">65%</span>
                                        </span>
                                        <span class="progress progress-danger progress-striped active">
                                            <span style="width: 65%;" class="bar"></span>
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="task">
                                            <span class="desc">Mobile app release</span>
                                            <span class="percent">98%</span>
                                        </span>
                                        <span class="progress progress-success">
                                            <span style="width: 98%;" class="bar"></span>
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="task">
                                            <span class="desc">Database migration</span>
                                            <span class="percent">10%</span>
                                        </span>
                                        <span class="progress progress-warning progress-striped">
                                            <span style="width: 10%;" class="bar"></span>
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="task">
                                            <span class="desc">Web server upgrade</span>
                                            <span class="percent">58%</span>
                                        </span>
                                        <span class="progress progress-info">
                                            <span style="width: 58%;" class="bar"></span>
                                        </span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <span class="task">
                                            <span class="desc">Mobile development</span>
                                            <span class="percent">85%</span>
                                        </span>
                                        <span class="progress progress-success">
                                            <span style="width: 85%;" class="bar"></span>
                                        </span>
                                    </a>
                                </li>
                                <li class="external">
                                    <a href="#">See all tasks <i class="m-icon-swapright"></i></a>
                                </li>
                            </ul>
                        </li>
                        <!-- END TODO DROPDOWN -->
                        <!-- BEGIN USER LOGIN DROPDOWN -->
                        <li class="dropdown user">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                <img alt="" src="assets/img/avatar1_small.jpg" />
                                <span class="username">Bob Nilson</span>
                                <i class="icon-angle-down"></i>
                            </a>
                            <ul class="dropdown-menu">
                                <li><a href="extra_profile.html"><i class="icon-user"></i>My Profile</a></li>
                                <li><a href="page_calendar.html"><i class="icon-calendar"></i>My Calendar</a></li>
                                <li><a href="inbox.html"><i class="icon-envelope"></i>My Inbox(3)</a></li>
                                <li><a href="#"><i class="icon-tasks"></i>My Tasks</a></li>
                                <li class="divider"></li>
                                <li><a href="extra_lock.html"><i class="icon-lock"></i>Lock Screen</a></li>
                                <li><a href="login.html"><i class="icon-key"></i>Log Out</a></li>
                            </ul>
                        </li>
                        <!-- END USER LOGIN DROPDOWN -->
                    </ul>--%>
                    <!-- END TOP NAVIGATION MENU -->
                </div>
            </div>
            <!-- END TOP NAVIGATION BAR -->
        </div>
        <!-- END HEADER -->
        <!-- BEGIN CONTAINER -->
        <div class="page-container row-fluid">

            <!-- BEGIN SIDEBAR -->


            <div class="span3">
                <div class="portlet box gray" style="width: 109%;">
                    <div class="portlet-title">
                        <div class="caption"><i class="icon-reorder"></i></div>
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                            <%--  <a href="javascript:;" class="reload"></a>--%>
                            <%-- <a href="javascript:;" class="remove"></a>--%>
                        </div>
                    </div>
                    <div class="portlet-body form mainform-scroll">
                        <!-- BEGIN FORM-->
                        <%--     <form action="#" class="form-horizontal">--%>
                        <%--<p>
                            <a class="btn bigicn-only green" href="#" title="Back to Home  Page">
                                <i class="m-icon-big-swapleft m-icon-white"></i>
                            </a>
                        </p>--%>
                        <%-- <p>
                            <a class="btn icn-only green" href="#" title="Back to Home  Page">
                                <i class="m-icon-swapleft m-icon-white"></i>
                            </a>
                        </p>--%>
                        <div class="control-group" id="organization" runat="server">
                            <label class="control-label">Organization*</label>
                            <div class="controls">

                                <select class="span12 m-wrap" id="ddlOrganization" runat="server">
                                </select>
                                <asp:HiddenField ID="hdnOrganization" runat="server" />
                            </div>
                        </div>
                        <div class="control-group" id="ActualAssignee">
                            <label class="control-label">Assignee*</label>
                            <asp:HiddenField ID ="hdnChangeAssignee" runat="server" />
                            <span id="Takeit" runat="server" style="float: right;">
                                <asp:LinkButton ID="lnktakeit" runat="server" OnClick="lnktakeit_Click" Text="Take It"></asp:LinkButton>
                            </span>
                            <span id="SetActualAssignee" runat="server" style="float: right;">
                                <asp:LinkButton ID="lnkSetActualAssignee" OnClientClick="return ToggleChangeAssignee();" runat="server" Text="Actual Assignee"></asp:LinkButton>
                            </span>
                            <span id="ChangeAssigne" runat="server" style="float: right;">
                                <asp:LinkButton ID="lnkchageAssignee" OnClientClick="return ChangeAssignee();" runat="server" Text="Change Assignee"></asp:LinkButton>
                            </span>
                            <div class="controls">
                                <select class="span12 m-wrap" id="ddlAssignee" runat="server">
                                </select>
                            </div>
                        </div>
                        <div class="control-group" id="NewAssignee">
                            <label class="control-label">New Assignee*</label>
                            <div class="controls">
                                <select class="span12 m-wrap" id="ddlNewAssignee" runat="server">
                                </select>
                            </div>
                            <%-- <span id="TicketAssigned" runat="server" style="float: right;">
                                <asp:LinkButton ID="lnkAssigned" runat="server" OnClick="lnkAssigned_Click" Text="Assigned"></asp:LinkButton>
                            </span>--%>
                            <asp:HiddenField ID="hdnNewAssignee" runat="server" />
                        </div>
                        <div class="control-group" style="display: none;">
                            <label class="control-label">Status*</label>
                            <div class="controls">
                                <select class="span12 m-wrap" id="ddlStatus" runat="server">
                                </select>
                            </div>
                        </div>
                        <div class="control-group">
                            <label class="control-label">CC*</label>
                            <div class="controls">
                                <asp:TextBox ID="txtCC" runat="server" class="span12 m-wrap"></asp:TextBox>
                            </div>
                        </div>
                        <asp:HiddenField ID="hdnStatus" runat="server" />
                      <%--  <div class="control-group">
                            <label class="control-label">Priority*</label>
                            <div class="controls">
                                <select class="span12 m-wrap" id="ddlPriority" runat="server">
                                </select>

                            </div>
                        </div>--%>
                        <%--<div class="control-group">
                            <label class="control-label">Tags*</label>
                            <div class="controls">
                                <input type="text" runat="server" placeholder="Please Enter Tags to search" id="txtTags" class="span12 m-wrap" />
                            </div>
                        </div>--%>
                        <%-- <div class="control-group">--%>

                        <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlFields1" CssClass="controls">
                                             <label class="control-label">Company Name*</label>
                                <div class="controls">
                                    <input type="text" placeholder="Please Enter Company Name" id="Text1" class="span12 m-wrap" />
                                </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>--%>
                        <%-- </div>--%>
                        <asp:Panel ID="pnlnewfields" runat="server" CssClass="pnlfields">
                        </asp:Panel>


                        <%-- <a class="btn icn-only green" href="#">
                            <asp:Button ID="btnUpdate" runat="server" CssClass="btn icn-only green" />--%>

                        <%--<div class="control-group">
                                <label class="control-label">Employee Code*</label>
                                <div class="controls">
                                    <input type="text" placeholder="Please Enter Employee Code" id="Text2" class="span12 m-wrap" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Type*</label>
                                <div class="controls">
                                    <select class="span12 m-wrap" id="ddlType">
                                    </select>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                </div>
                            </div>--%>
                        <%--</form>--%>
                    </div>

                </div>
            </div>
            <div class="span9">
                <div class="portlet box gray">
                    <div class="portlet-title">
                        <div class="caption"><i class="icon-reorder"></i>Ticket Conversation for Ticket #<span id="docid"></span> (STATUS- <span id="status" runat="server"></span>)</div>
                        <asp:HiddenField ID="hdnPreStatus" runat="server" />
                        <div class="tools">
                            <a href="javascript:;" class="collapse"></a>
                            <%-- <a href="javascript:;" class="reload"></a>--%>

                            <%-- <a href="javascript:;" class="remove"></a>--%>
                        </div>

                        <a href="thome.aspx" style="color: white;" title="Back to Home"><i class="icon-home" style="float: right; margin-top: .5%; padding-right: 1.0% !important;"></i></a>

                    </div>
                    <div class="portlet-body form mainform-scroll ">
                        <div class="control-group">
                            <div class="controls">
                                <div class="span9">
                                    <div class="input-icon left">
                                        <i class="icon-envelope"></i>
                                        <asp:TextBox runat="server" ID="txtSubject" class="span12 m-wrap"></asp:TextBox>

                                        <asp:Panel ID="pnlDownloads" runat="server"></asp:Panel>
                                    </div>
                                </div>
                                <div class="span3">
                                    <asp:HiddenField ID="hdnSpan" runat="server" />
                                    <asp:HiddenField ID="hdnAction" runat="server" />
                                    <asp:HiddenField ID="hdnAllowEnableStatusToEndUser" runat="server" />
                                    <div class="btn-group" id="useraction" runat="server">
                                        <a class="btn green ddlAction" href="#" data-toggle="dropdown">
                                            <i class="icon-user"></i>Action									
                                            <i class="icon-angle-down"></i>
                                        </a>
                                        <ul class="dropdown-menu">
                                            <li><a href="#Open" data-toggle="tooltip" data-placement="bottom" title="Help Desk staff is working on the ticket">Open</a></li>
                                            <li><a href="#Pending" data-toggle="tooltip" data-placement="bottom" title="Help Desk staff is waiting for the requester reply">Pending</a></li>
                                            <li><a href="#Close" data-toggle="tooltip" data-placement="bottom" title="The Ticket has been Solved">Solved</a></li>
                                        </ul>

                                        <%--OnClientClick="return AddFileName();"--%>
                                        <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_click" OnClientClick="return UpdateCall();" class="btn blue" Text="Submit" />
                                        <asp:Button ID="btnTest" runat="server" OnClick="btnTest_Click" class="btn blue" Text="GO!" Visible="false" />

                                        <i class="m-icon-swapright m-icon-white"></i>
                                        <%-- <button class="btn blue" id="btnUpdate" onserverclick="btnUpdate_click" runat="server">
                                            <i class="m-icon-swapright m-icon-white"></i>
                                        </button>--%>
                                    </div>
                                    <div class="btn-group" id="noaction" runat="server">
                                        <asp:Label ID="btnFollowup" runat="server" CssClass="btn blue" Text="Please Generate Fresh Ticket"></asp:Label>
                                        <%--<asp:Button ID="btnFollowup" runat="server" OnClick="btnFollowup_Click" OnClientClick="Followup();" UseSubmitBehavior="false"  Enabled="false"  class="btn blue" Text="Please Generate Fresh Ticket" />--%>
                                    </div>

                                </div>

                                <p class="help-block">
                                    <asp:HiddenField ID="hdnUploadedFileName" runat="server" />
                                    <span class="muted" id="mailDate" runat="server"></span><span class="muted" id="DisplayName" runat="server"></span><span class="muted" id="TO" runat="server"></span>
                                    <%--  <asp:Label runat="server" ID="txtmsg" class="span12 m-wrap"></asp:Label>--%>
                                    <asp:HiddenField ID="hdnDOCID" runat="server" />
                                    <span style="float: right">

                                        <%--  <asp:UpdatePanel ID="uppnl" runat="server">
                                            <ContentTemplate>--%>
                                        <asp:FileUpload ID="file_upload" class="multi" runat="server" onChange="uploadImage()" Style="display: none;" />
                                        <asp:Button ID="btnUpload" runat="server" Text="Upload"
                                            Style="display: none;" /><br />
                                        <%--OnClick="btnUpload_Click"--%>
                                        <%--   </ContentTemplate>
                                            <Triggers>
                                                <asp:PostBackTrigger ControlID="btnUpload" />
                                            </Triggers>
                                        </asp:UpdatePanel>--%>

                                       

                                    </span>
                                </p>
                                <div class="demo-section k-content">
                                    <input name="files" id="files" type="file" style="display: none;" />
                                    <%--  <p style="padding-top: 1em; text-align: right">
                                        <input type="submit" value="Submit" class="k-button k-primary" />
                                    </p>--%>
                                </div>
                                <div class="btn-toolbar">
                                </div>
                                <%--<input class="span10 m-wrap" type="text" placeholder="Email Subject" runat="server" id="txtSubject" />--%>
                            </div>
                        </div>
                        <hr style="margin-top: 0px;" />
                        <div class="control-group">
                            <div class="controls ">
                                <asp:HiddenField ID="hdndivhtml" runat="server" />
                                <label class="control-label">Reply</label>
                                <div class="controls" style="height: 200px; overflow-y: auto; overflow-x: hidden; overflow: hidden;">
                                    <%-- <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="150px">--%>
                                    <asp:TextBox ID="txtBody" TextMode="MultiLine" runat="server" Width="100%" Height="98%" CssClass="span8 m-wrap">
                                    </asp:TextBox>
                                    <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="false" TargetControlID="txtbody" EnableSanitization="false"></asp:HtmlEditorExtender>
                                    <%--  </asp:Panel>--%>
                                    <asp:HiddenField ID="hdnAssignee" runat="server" />
                                </div>
                            </div>
                            <div style="border: 1px solid #ccc; border-radius: 5px; width: 100%; padding: 5px 5px; max-height: 100px; overflow-y: auto; margin-top: -0.5%; border-bottom-right-radius: 8px ! important; border-bottom-left-radius: 8px ! important;">
                                <asp:LinkButton ID="LinkButton1" runat="server" Text="Attachment" OnClientClick="selectFile(); return false;"></asp:LinkButton>
                                <span style="margin-left: 10px; float: right; height: 40px; overflow-y: auto; width: 30%;">
                                    <asp:Label ID="lblMessage" runat="server" />
                                </span>
                            </div>
                        </div>
                        <hr />
                        <div class="control-group">
                            <div class="controls">
                                <%-- <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" Height="210px" Enabled="false">--%>
                                <div class="controls" id="divHistory" runat="server" style="height: auto; overflow-y: auto; overflow-x: hidden; overflow: hidden; border: 1px solid #ccc; border-radius: 5px !important; padding: 5px !important; border-bottom-right-radius: 8px ! important; border-bottom-left-radius: 8px ! important;">
                                    <%-- <asp:TextBox ID="txtHistory" TextMode="MultiLine" ReadOnly="true" runat="server" Width="100%" Height="98%" CssClass="span12 m-wrap">
                                    </asp:TextBox>
                                    <asp:HtmlEditorExtender ID="HEE_History" runat="server" DisplaySourceTab="false" TargetControlID="txtHistory" EnableSanitization="false">
                                    </asp:HtmlEditorExtender>--%>
                                </div>
                                <%--  </asp:Panel>--%>
                            </div>
                        </div>
                    </div>

                </div>

            </div>
        </div>
        <!-- END SIDEBAR -->
        <!-- BEGIN PAGE -->

        <!-- END PAGE -->


        <!-- END CONTAINER -->
        <!-- BEGIN FOOTER -->
        <div class="footer">
            <div class="footer-inner">
                2016 &copy; Mynd Solution
            </div>
            <div class="footer-tools">
                <span class="go-top">
                    <i class="icon-angle-up"></i>
                </span>
            </div>
        </div>
        <!-- END FOOTER -->
        <!-- BEGIN JAVASCRIPTS(Load javascripts at bottom, this will reduce page load time) -->
        <!-- BEGIN CORE PLUGINS -->
        <script src="assets/plugins/jquery-1.10.1.min.js" type="text/javascript"></script>
        <script src="assets/plugins/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
        <!-- IMPORTANT! Load jquery-ui-1.10.1.custom.min.js before bootstrap.min.js to fix bootstrap tooltip conflict with jquery ui tooltip -->
        <script src="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js" type="text/javascript"></script>
        <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
        <!--[if lt IE 9]>
	<script src="assets/plugins/excanvas.min.js"></script>
	<script src="assets/plugins/respond.min.js"></script>  
	<![endif]-->
        <script src="assets/plugins/breakpoints/breakpoints.min.js" type="text/javascript"></script>
        <script src="assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js" type="text/javascript"></script>
        <script src="assets/plugins/jquery.blockui.min.js" type="text/javascript"></script>
        <script src="assets/plugins/jquery.cookie.min.js" type="text/javascript"></script>
        <script src="assets/plugins/uniform/jquery.uniform.min.js" type="text/javascript"></script>
        <!-- END CORE PLUGINS -->
        <!-- BEGIN PAGE LEVEL PLUGINS -->
        <%-- <script src="assets/plugins/bootstrap-tag/js/bootstrap-tag.js" type="text/javascript"></script>
        <script src="assets/plugins/fancybox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
        <script src="assets/plugins/bootstrap-wysihtml5/wysihtml5-0.3.0.js" type="text/javascript"></script>
        <script src="assets/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.js" type="text/javascript"></script>--%>
        <!-- BEGIN:File Upload Plugin JS files-->
        <%--   <script src="assets/plugins/jquery-file-upload/js/vendor/jquery.ui.widget.js"></script>--%>
        <!-- The Templates plugin is included to render the upload/download listings -->
        <%--   <script src="assets/plugins/jquery-file-upload/js/vendor/tmpl.min.js"></script>--%>
        <!-- The Load Image plugin is included for the preview images and image resizing functionality -->
        <%-- <script src="assets/plugins/jquery-file-upload/js/vendor/load-image.min.js"></script>--%>
        <!-- The Canvas to Blob plugin is included for image resizing functionality -->
        <%-- <script src="assets/plugins/jquery-file-upload/js/vendor/canvas-to-blob.min.js"></script>--%>
        <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
        <%--  <script src="assets/plugins/jquery-file-upload/js/jquery.iframe-transport.js"></script>--%>
        <!-- The basic File Upload plugin -->
        <%-- <script src="assets/plugins/jquery-file-upload/js/jquery.fileupload.js"></script>--%>
        <!-- The File Upload file processing plugin -->
        <%--  <script src="assets/plugins/jquery-file-upload/js/jquery.fileupload-fp.js"></script>--%>
        <!-- The File Upload user interface plugin -->
        <%--    <script src="assets/plugins/jquery-file-upload/js/jquery.fileupload-ui.js"></script>--%>
        <!-- The XDomainRequest Transport is included for cross-domain file deletion for IE8+ -->
        <!--[if gte IE 8]><script src="assets/plugins/jquery-file-upload/js/cors/jquery.xdr-transport.js"></script><![endif]-->
        <!-- END:File Upload Plugin JS files-->
        <!-- END PAGE LEVEL PLUGINS -->
        <script src="assets/scripts/app.js"></script>
        <%--   <script src="assets/scripts/inbox.js"></script>--%>
        <script>
            jQuery(document).ready(function () {
                // initiate layout and plugins
                App.init();
            });
        </script>
        <!-- END JAVASCRIPTS -->
    </form>
</body>
</html>
<script type="text/javascript">

    //var dataValues = [];
    $(document).ready(function () {
        $.noConflict();
        if ($("#hdndivhtml").val() != "") {
            $("div.demo-section").html($("#hdndivhtml").val());
        }
        //$("#ChangeAssigne").hide();
        //$("#NewAssignee").hide();
        $("#SetActualAssignee").hide();
        GetOrganization();
        GetAssognee();
        $("#ddlCC").kendoDropDownList();
        $("#ddlPriority").append($("<option>-</option><option>LOW</option><option>NORMAL</option><option>HIGH</option><option>URGENT</option>"));
        $("#ddlPriority").kendoDropDownList();
        $("#ddlStatus").append($("<option>PENDING</option><option>Solved</option><option>OPEN</option><option>SUSPENDED</option><option>NEW</option>"));
        $("#ddlStatus").kendoDropDownList();
        $("#useraction ul>li>a").on("click", function (event) {
            event.preventDefault();
            $("#hdnAction").val($(this).text());
            $("#useraction a>span").remove();
            $("#useraction a>i:eq(1)").after(' <span>' + $(this).text() + '</span>');
        });
        if (Number($("input[id*='hdnAllowEnableStatusToEndUser']").val()) == 1) {
            $(".ddlAction").addClass('disabled');
        }
        else {
            $(".ddlAction").removeClass('disabled');
        }

    });
    function AddFileName() {
        if (dataValues.length > 0) {
            $("#hdnUploadedFileName").val(dataValues.join(","))
        }
        $("#hdndivhtml").val($("div.demo-section").html());
        return true;
    }

    function GetOrganization() {
        var docid = $("#hdnDOCID").val()
        $.ajax({
            type: "POST",
            url: "TicketDocDetail.aspx/GetOrganization",
            contentType: "application/json;charset=utf-8",
            data: '{ DocID: ' + docid + ' }',
            dataType: "json",
            success: function (response) {
                var res = response.d;
                var data = $.parseJSON(res.ds);
                if (res.StatusData == "SUCCESS") {
                    $("#organization").show();
                    var orgKedo = $("#ddlOrganization").kendoDropDownList({
                        index: 0,
                        dataTextField: "organization",
                        dataValueField: "tid",
                        change: onChangeOrg,
                        dataSource: data,
                        optionLabel: "Select",
                    });
                    if (res.SelectedValForOrgainzations != "") {
                        $("#ddlOrganization").data('kendoDropDownList').value(res.SelectedValForOrgainzations);
                        $("#hdnOrganization").val($("#ddlOrganization").val());
                        //hide organization because value will be selected and do not change the organizations
                        $("#organization").hide();
                        ShowHideNewAssignee();
                    }
                    if ($("#hdnOrganization").val() != "") {
                        $("#ddlOrganization").data('kendoDropDownList').value($("#hdnOrganization").val());
                    }
                    function onChangeOrg() {
                        var dropdownlist = $("#ddlOrganization").val();
                        $("#hdnOrganization").val(dropdownlist);
                        ChangeAssignee();
                        //if ($("#SetActualAssignee").css("display") == "block") {
                        //    ChangeAssignee();
                        //}
                    }
                }
                else {
                    if ($("#hdnChangeAssignee").val() == "CHANGE ASSIGNEE") {
                        $("#ChangeAssigne").show();
                    }
                    else {
                        $("#ChangeAssigne").hide();
                        ShowHideNewAssignee();
                    }
                    $("#organization").hide();
                   
                }
            },
            error: function (data) {
                alert('Error');
            }
        });

    }
    function GetAssognee() {
        var docid = $("#hdnDOCID").val()
        $("#docid").html(docid);
        $.ajax({
            type: "POST",
            url: "TicketDocDetail.aspx/GetAssignee",
            contentType: "application/json; charset=utf-8",
            data: '{ DocID: ' + docid + ' }',
            dataType: "json",
            success: function (response) {
                var res = response.d;
                //dvsqEditLoader
                var data = $.parseJSON(res.ds);

                $("#ddlAssignee").kendoDropDownList({
                    dataTextField: "username",
                    dataValueField: "uid_Id",
                    dataSource: data,
                });
                //$("#ddlAssignee").data('kendoDropDownList').value(parseInt(res.dropdownlist));
                $("#ddlStatus").data('kendoDropDownList').value(res.StatusData);
                $("#status").html(res.StatusData);
                $("#hdnPreStatus").val(res.StatusData);
                if (Number($("input[id*='hdnAllowEnableStatusToEndUser']").val()) == 1) {
                    $("#hdnAction").val(res.StatusData);
                    $("#useraction a>span").remove();
                    $("#useraction a>i:eq(1)").after(' <span>' + res.StatusData + '</span>');
                }
             

            },
            error: function (data) {
            }
        });

    }
    function GetNewAssignee() {
        if ($("#hdnOrganization").val() == "") {
            if ($("#hdnChangeAssignee").val() == "CHANGE ASSIGNEE") {
                $("#hdnOrganization").val("0");
                alert($("#hdnOrganization").val());
            }
        }
        $.ajax({
            type: "POST",
            url: "TicketDocDetail.aspx/GetNewAssignee",
            contentType: "application/json; charset=utf-8",
            data: '{ OrgID: ' + $("#hdnOrganization").val() + ',CurrentAssignee:' + $("#ddlAssignee").val() + ' }',
            dataType: "json",
            success: function (response) {
                var res = response.d;
                //dvsqEditLoader
                var data = $.parseJSON(res.ds);

                $("#ddlNewAssignee").kendoDropDownList({
                    dataTextField: "username",
                    dataValueField: "Userid",
                    dataSource: data,
                    optionLabel: "Select",
                    change: onChangeNewAssignee,
                });
            },
            error: function (data) {
            }
        });
        function onChangeNewAssignee() {
            var dropdownlistForAssignee = $("#ddlNewAssignee").val();
            if (dropdownlistForAssignee == "") dropdownlistForAssignee = 0;
            $("#hdnNewAssignee").val(dropdownlistForAssignee);
        }
    }
    function UpdateCall() {
        if ($("#organization").css("display") == "block") {
            if ($("#ddlOrganization").val() == "") {
                alert('Please select organization');
                return false;
            }
        }
        if ($("#NewAssignee").css("display") == "block") {
            if ($("#hdnNewAssignee").val() == "0" || $("#hdnNewAssignee").val() == "") {
                alert('Please select New Assignee or click on Change Assigne if you do not want to change!');
                return false;
            }
        }
        if ($("#SetActualAssignee").css("display") == "block") {
            $("#hdnNewAssignee").val($("#ddlNewAssignee").val());
            alert($("#hdnNewAssignee").val());
        }
        else {
            if ($("#NewAssignee").css("display") == "block") {

            }
            else {
                $("#hdnNewAssignee").val(0);
            }
            //alert('hii');
            //$("#hdnNewAssignee").val(0);
        }
        $("#hdnAssignee").val($("#ddlAssignee").val());
        ShowProgress();
    }
    function Followup() {
        $("#hdnStatus").val('OPEN');
    }
    function ShowHideNewAssignee() {
        if ($("#NewAssignee").css("display") == "block") {
            $("#NewAssignee").hide();
        }
        else {
            $("#NewAssignee").show();
        }
    }
</script>
<%--<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('a').tooltip();
        document.getElementById("txtHistory$HtmlEditorExtenderBehavior_ExtenderButtonContainer").style.display = "none";
    });

</script>--%>
<script type="text/javascript">
    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
</script>
<script type="text/javascript">
    function ChangeAssignee() {
        if ($("#hdnOrganization").val() == "" && $("#hdnChangeAssignee").val() != "CHANGE ASSIGNEE") {
            alert('Please select organization');
            return false;
        }
        else {
            ShowHideNewAssignee();
            //$("#NewAssignee").show();
            GetNewAssignee();
            //$("#ChangeAssigne").hide();
            //$("#SetActualAssignee").show();
            return false;
        }
    }
    function ToggleChangeAssignee() {
        $("#SetActualAssignee").hide();
        $("#ChangeAssigne").show();
        $("#NewAssignee").hide();
        return false;
    }
</script>
