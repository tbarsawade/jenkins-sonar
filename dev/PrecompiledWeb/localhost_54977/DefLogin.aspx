<%@ page language="VB" autoeventwireup="false" inherits="_DefLogin, App_Web_01howaz0" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>
<!-- saved from url=(0049)http://virtualmynd.m1xchange.com/login/login_view -->

<html><head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">	
    <link rel="icon" href="http://myndsaas.com/favicon.ico" type="image/gif">
    

 <%--   <meta http-equiv="X-UA-Compatible"  charset="utf-8" name="viewport" content="width=device-width, initial-scale=1"  />--%>
    
       <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
   


    <link href='https://fonts.googleapis.com/css?family=Raleway:400,600,700' rel='stylesheet' type='text/css' />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" />
    <link href="kendu/content/integration/bootstrap-integration/css/bootstrap.min.css" rel="stylesheet" />
    <script src="Jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <link href="css/mstyle.css" rel="stylesheet" />
  


    
    <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
    <link href="Scripts/jquery-ui.css" rel="stylesheet" type="text/css" />
    

    
<title>Mynd HR Admin Helpdesk </title>
		
<%--<link href="./Submit Request_files/bootstrap.min.css" rel="stylesheet">		
<link href="./Submit Request_files/custom.min.css" rel="stylesheet">  --%>

  <style type="text/css">
        .loader {
            border: 16px solid #f3f3f3;
            border-radius: 50%;
            border-top: 16px solid #3498db;
            width: 100px;
            height: 100px;
            -webkit-animation: spin 2s linear infinite; /* Safari */
            animation: spin 2s linear infinite;
            margin: 40px auto;
            
        }

        /* Safari */
        @-webkit-keyframes spin {
            0% {
                -webkit-transform: rotate(0deg);
            }

            100% {
                -webkit-transform: rotate(360deg);
            }
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>

</head>  
<body>    



<div class="row"> 		
<div class="col-md-4">
 <img src="./images/mynd-logo.png" alt="">        
    </div>
<div class="ms-navbar-brand col-md-4"><center><h3>Welcome to Mynd Solutions</h3></center></div>
   </div>
    
<div class="container">   
<div class="row">        
<div class="col-md-4 col-md-offset-4">            
<div class="login-panel panel panel-primary">                
<div class="panel-heading">
 <h3 class="panel-title">Login</h3>
                </div>
              


                
<div class="panel-body">  

<form id="form1" autocomplete="off">  <%--<form role="form" method="post">--%>                        
    <div id="divProgress" style="text-align:center;">
            <div class="loader"></div>
       </div>
 <fieldset>                            

 <div class="form-group">								
 <div class="input-group">		
   <input class="form-control" onfocus="$(this).removeAttr('readonly');" onkeypress="EnterEvent(event);" id="txtUserID" placeholder="Username">   								          
    <label for="txtUserID" class="input-group-addon glyphicon glyphicon-user"></label>	
  </div>
  </div>
                         
   <div class="form-group">								
   <div class="input-group">	
   <input type="password" class="form-control" readonly="true"  autocomplete="off" onfocus="$(this).removeAttr('readonly');" onkeypress="EnterEvent(event);" placeholder="Password" id="txtPwd" />      
 	<label for="txtPwd" class="input-group-addon glyphicon glyphicon-lock"></label>								
   </div>  </div>

    <div class="form-group">								
 <div class="input-group">		
   <input class="form-control" onfocus="$(this).removeAttr('readonly');" onkeypress="EnterEvent(event);"  placeholder="Customer Code" id="txtEntityName" />   								          
    <label id="igagEntity" for="txtEntityName" class="input-group-addon glyphicon glyphicon-user"></label>	     
  </div>
       <span class="help-block" id="lbltopmsg"></span> 
  </div>

   <div class="ln_solid"></div>
  <div class="form-group"> 
   <div class="col-md-12 col-sm-6 col-xs-12 col-md-offset-2">									          
       <button type="button" id="btnlogin"  style="height: 30px; width: 80px;"  class="btn btn-success submit">Login</button>
      <%-- <button type="button" class="btn btn-primary submit" onclick="ClearAllFields();" type="reset">Reset</button>--%>
       <button type="button" class="btn btn-primary submit" onclick="ClearAllFields();" style="height: 30px; width: 80px;" value="Login">Reset</button>
         <a id="Forgotpswd"  href="#" onclick="popup();">Forgot Password</a> 									 
     </div>
  </div>


    <div id="modal_dialog" style="display: none">
           <div class="col-md-4 col-sm-4" style="color:#333;margin-top:18px;"> <label class="">User Id :</label></div>
           <div class="col-md-8 col-sm-8"><input type="text" class="form-control form-group-box" readonly="true" autocomplete="off" onfocus="$(this).removeAttr('readonly');" onkeypress="ForgotEnterEvent(event);" placeholder="UserID" id="txtEmailName" />
            </div>
               <div id="lblcustcode" class="col-md-4 col-sm-4" style="color:#333;margin-top:25px; ""> <label class="">Customer Code :</label></div>
              <div id="divcustcode" class="col-md-8 col-sm-8" style="margin-top:20px;"><input type="text" class="form-control form-group-box" readonly="true" autocomplete="off" onfocus="$(this).removeAttr('readonly');" onkeypress="ForgotEnterEvent(event);" placeholder="Customer Code" id="txtFEntityName" />
            </div>
             <span class="help-block" id="lblMsgAddFolder"></span>
            
            <div class="form-group drop pull-right">
                   <button type="button" id="btnSubmit" class="btn btn-success submit" style="height: 30px; width: 100px;" value="Login">Submit</button>
                    <button type="button" class="btn btn-primary submit" onclick="ClearAllFields();" style="height: 30px; width: 100px;" value="Login">Reset</button>
            </div>
           </div>
         <div id="success_modal_dialog" style="display: none">
              <div class="topheading-list2">Email sent to <span id="spnUserId"></span>  shortly, containing a link that enables you to reset your password .</div>
             <div class="topheading-list1">If you don't see an email from us within couple of minutes, it may be because: </div>
            
            <ul class="differentimage">
                 <li class="one"><a href="#">The email is in your junk/spam folder</a></li>
                 <li class="two"><a href="#">You do not have an account with us</a></li>
                 <li class="three"><a href="#">Our system does not allow you to reset your Password</a></li>
            </ul>
         </div>
      

 
     </fieldset>
   </form> 
    </div> </div> </div></div> </div>   <div>

        <footer>
            <div class="container-fluid">
                <div class="row" style="background: #f7f7f7;">
                    <div class="col-sm-12 col-md-12">
                        <div class="ftr">
                            <strong>Copyright © 2018, Mynd Solutions Pvt. Ltd. All rights reserved.</strong>

                        </div>
                    </div>
                </div>
            </div>
        </footer>

   <br><br><br><br><br><br><br><br><br><br><br><br><br><br>   <br><br><br><br><br><br><br><br><br><br>
  </div>
    </body></html>

<script type="text/javascript">
    $(document).ready(function () {
        $("#divProgress").hide();
        $("#txtUserID").focus();
        GetRequest();
        $("#btnSubmit").unbind("click").bind("click", function (event) {
            var IsPopValidation = PopupValidation();
            if (IsPopValidation == false) {
                event.preventDefault();
                $("#divProgress").dialog("close");
            }
            else {
                progressPopup();
                var userid = $("#txtEmailName").val();
                var customercode = $("#txtFEntityName").val();
                $.ajax({
                    type: "POST",
                    url: "Deflogin.aspx/ForgotPassword",
                    data: '{userid:"' + userid + '",customercode:"' + customercode + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccessForgotPassword,
                    failure: function (response) {
                        alert(response.d);
                    }
                });
                function OnSuccessForgotPassword(response) {
                    var objData = JSON.parse(response.d);
                    $.each(objData, function (key, value) {
                        if (key == "SUCCESS") {
                            $("#spnUserId").text('' + value + '');
                            $("#modal_dialog").dialog("close");
                            $("#divProgress").dialog("close");
                            successpopup();

                        }
                        else {
                            $("#lblMsgAddFolder").text('' + value + '')
                            $("#modal_dialog").dialog("open");
                            $("#success_modal_dialog").dialog("close");
                            $("#divProgress").dialog("close");
                        }
                    });
                }
            }
        });

        $("#btnlogin").unbind("click").bind("click", function (event) {
            progressPopup();
            var IsValidation = FormValidation();
            if (IsValidation == false) {
                event.preventDefault();
                $("#divProgress").dialog("close");
            }
            else {
                var username = $("#txtUserID").val();
                var password = $("#txtPwd").val();
                var entityname = $("#txtEntityName").val();
                $.ajax({
                    type: "POST",
                    url: "Deflogin.aspx/ValidateUser",
                    data: '{username:"' + username + '",password:"' + password + '",entityname:"' + entityname + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccessValidateUser,
                    failure: function (response) {
                        alert(response.d);
                    }

                });
                function OnSuccessValidateUser(response) {
                    var objData = JSON.parse(response.d);
                    var count = 0
                    $.each(objData, function (key, value) {
                        if (count == 0) {
                            if (key == 1 && value != "Please SET the default page for login  or contact to the super user") {
                                window.location.href = value;
                                $("#divProgress").dialog("close");
                            }
                            else {
                                $("#lbltopmsg").text('' + value + '');
                                $("#divProgress").dialog("close");
                            }
                            count += 1;
                        }
                    });
                }
            }
        });
    });
    function GetRequest() {

        $.ajax({
            type: "POST",
            url: "Deflogin.aspx/GetRequest",
            data: '{url: "' + window.location.href + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnGetRequestSuccess,
            failure: function (response) {
                alert(response.d);
            }
        });
    }
    function OnGetRequestSuccess(response) {
        var obj = JSON.parse(response.d);
        //$("#lblLogo").attr("src", "logo/" + obj[2]);
        //$("#lblHeaderLogo").attr("src", "logo/" + obj[3]);
        //$("#lblmsgName").text('' + obj[1] + '');
        if (obj["EntityName"] == 'VISIBLE') {
            $("#txtEntityName").show();
            $("#txtFEntityName").show();
            $("#divcustcode").show()
            $("#lblcustcode").show()
        }
        else {
            $("#txtEntityName").hide();
            $("#txtEntityName").val(obj["EntityCode"]);
            $("#txtFEntityName").hide();
            $("#txtFEntityName").val(obj["EntityCode"]);
            $("#divcustcode").hide()
            $("#lblcustcode").hide()
            $("#igagEntity").hide();
        }
    }
    function EnterEvent(e) {
        var key = e.which;
        if (key == 13)  // the enter key code
            $("#btnlogin").click();
        return false;
    }
    function ForgotEnterEvent(e) {
        var key = e.which;
        if (key == 13)  // the enter key code
            $("#btnSubmit").click();
        return false;
    }
    function PopupValidation() {
        if ($("#txtEmailName").val() == '') {
            $("#lblMsgAddFolder").text("Kindly Enter User Id");
            $("#txtEmailName").focus();
            return false;
        }
        else if ($("#txtFEntityName").is(":visible")) {
            if ($("#txtFEntityName").val() == '') {
                $("#lblMsgAddFolder").text("Kindly Enter Customer Code");
                $("#txtFEntityName").focus();
                return false;
            }
        }

    }

    function FormValidation() {
        $("#txtFEntityName").show();
        if ($("#txtUserID").val() == '') {
            $("#lbltopmsg").text("Kindly Enter UserName ");
            $("#txtUserID").focus();
            return false;
        }
        else if ($("#txtPwd").val() == '') {
            $("#lbltopmsg").text("Kindly Enter Password ");
            $("#txtPwd").focus();
            IsValidation = false;
            return IsValidation;
        }
        else if ($("#txtEntityName").is(":visible")) {
            $("#txtFEntityName").hide();
            if ($("#txtEntityName").val() == '') {
                $("#lbltopmsg").text("Kindly Fill Customer Code ");
                $("#txtEntityName").focus();
                return false;
            }
        }
    }
    function ClearAllFields() {
        $("input:text").val("");
        $("input:password").val("");
        $("#txtUserID").focus();
    }


    function popup() {
        $("#txtEmailName").val("");
        $("#lblMsgAddFolder").text("");
        $("#modal_dialog").dialog({
            title: "Forgot Password",
            width: 500,
            buttons: {
                Close: function () {
                    $(this).dialog('close');
                }
            },
            modal: true
        });
    }

    function successpopup() {
        $("#success_modal_dialog").dialog({
            title: "Forgot Password Status",
            width: 550,
            buttons: {
                Close: function () {
                    $(this).dialog('close');
                }
            },
            modal: true
        });
    }
    function progressPopup() {
        $("#divProgress").dialog({
            width: 250,
            height: 200,
            modal: true
        });
    }

</script>
