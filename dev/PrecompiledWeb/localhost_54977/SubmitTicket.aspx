<%@ page language="VB" autoeventwireup="false" inherits="SubmitTicket, App_Web_01howaz0" viewStateEncryptionMode="Always" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" EnableViewState="false">--%>

<!DOCTYPE html>
<!-- saved from url=(0047)http://virtualmynd.m1xchange.com/website/lead/3 -->
<html>
    <head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<%--	<link rel="icon" href="http://myndsaas.com/favicon.ico" type="image/gif">--%>

<title></title>
        <script src="https://code.jquery.com/jquery-1.11.0.min.js"></script>
<link rel="stylesheet" href="Submit_Request_files/bootstrap.min.css" media="screen" title="no title">
<link href="Submit_Request_files/custom.min.css" rel="stylesheet">
<!--link href="http://virtualmynd.m1xchange.com/assets/css/agency.min.css" rel="stylesheet"-->
       
<script src="Submit_Request_files/jquery.min.js.download"></script>
<script src="Submit_Request_files/jquery.inputmask.bundle.min.js.download"></script>
<script src="Submit_Request_files/moment.min.js.download"></script>
<script src="Submit_Request_files/bootstrap-datetimepicker.min.js.download"></script>



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
                               $("#hdnUploadedFileName").val(dataValues[0]);
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
        function OpenWindow(url) {
            var new_window = window.open(url, "", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }
    </script>
<script>		
   function submit_form(){
        $('#submitbutton').attr('disabled','disabled');
        var menuitemid = $('#menuitemid').val();
		
        var req_name = $('#req_name').val();
        var req_email = $('#req_email').val();
		
        var user_email = $('#user_email').val();
		
        var org_name = $('#org_name').val();
		
        var user_mobile = $('#user_mobile').val();
		
        var industry = $('#industry').val();
		
        var country = $('#country').val();
		
        var state = $('#state').val();
		
        var city = $('#city').val();
		
        var designation = $('#designation').val();
		
        var repeat_email =  $('#repeat_email').val();
		
        var lead_remarks = $('#lead_remarks').val();
		
        if (user_email != repeat_email){
			
            $('#message').css( "color", "red" );
			
            $('#message').html("E-Mail and Repeat E-Mail do not match");
			
            $('#snackbar').html("E-Mail and Repeat E-Mail do not match");
			
            var x = document.getElementById("snackbar");
			
            x.className = "show";
			
            setTimeout(function(){ x.className = x.className.replace("show", ""); }, 3000);
			
            return ;
        }
		 
        $.ajax({  
			
            url:"http://virtualmynd.m1xchange.com/website/register_lead",  
			
            data: {
                menuitemid : menuitemid ,
				
                req_name  : req_name  ,
				
                req_email  : req_email  ,
				
                user_email  : user_email  ,
				
                lead_remarks : lead_remarks ,
				
                org_name    : org_name    ,
				
                user_mobile : user_mobile ,
				
                industry    : industry    ,
				
                country     : country     ,
				
        state       : state       ,
				
        city        : city        ,
				
        designation : designation 
    },  
			
    type: "POST",  
			
        success:function(data){  
				
            if (data == '"OK"'){
					
                $('#message').css( "color", "green" );
					
                $('#snackbar').html("Your request has been submitted ! We will get back to you. Redirecting to website.");
					
                var x = document.getElementById("snackbar");
                x.className = "show";
					
                setTimeout(function()
                { x.className = x.className.replace("show", ""); }, 3000);
					
                setTimeout(function(){
						
                    window.location = "http://virtualmynd.m1xchange.com/" ;
                }, 3000);
			    
            } 
            else {
                $('#message').css( "color", "red" );
					
                $('#message').html(data);
					
                $('#snackbar').html(data);
                var x = document.getElementById("snackbar");
					
                x.className = "show";
                setTimeout(function(){ x.className = x.className.replace("show", ""); }, 3000);
					
                $('#submitbutton').attr('disabled','disabled');
            }
        }  
    }); 
    }

		
    $(document).ready(function(){
		  
        $('#country').on("change", function(e){
            var country = $('#country').val() ;
			
            if (country == 'India') {
				
                var arg ='Select State';
                $('#state > option').each(function(){
                    if($(this).text()==arg) $(this).parent

   ('select').val($(this).val())
                });
                $('#state').removeAttr('disabled'); 
            } else {
		

                var arg ='Outside India';
                $('#state > option').each(function(){
                    if($(this).text()==arg) 

                        $(this).parent('select').val($(this).val())
                });
                $('#state').attr('disabled','disabled'); 
			

            }
        });
		  
        $("#repeat_email").on("contextmenu",function(e){
            return false;
        });
		   

        $('#repeat_email').bind('cut copy paste', function (e) {
            e.preventDefault();
        });		  
    });
	
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
	
</script>

        <script type="text/javascript">
            function OpenWindow(url) {
                var new_window = window.open(url, "", "scrollbars=yes,resizable=yes,width=800,height=480");
                //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
                return false;
            }
    </script>

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
		
  </head>
  
<body>
   


<span style="background-color:red;">
 <div class="container" style="text-align:center" ><!-- container class is used to centered  the body of the browser with some decent width-->

      
<div class="row"><!-- row class is used for grid system in Bootstrap-->
          
<div class="col-md-6 col-md-offset-3"><!--col-md-4 is used to create the no of colums in the grid also use for medimum and large devices-->              
<div class="login-panel panel panel-primary">                  
<div class="panel-heading">                      
<h3 class="panel-title">Submit Request</h3>                  
</div>
                  
<div class="panel-body">    
<div id="snackbar">Some text some message..</div>                      
 <form runat="server">            
<fieldset>                  
      <asp:ToolkitScriptManager ID="scriptmanager" runat="server"></asp:ToolkitScriptManager>
 <div id="message" class="form-group col-xs-12">				   
     <asp:Label ID="lblTab" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
  </div>
    <asp:Panel ID="pnlnewfields" runat="server" CssClass="pnlfields">
                                        
<%--<div class="form-group col-xs-6">
 <div class="controls">
     <asp:DropDownList ID="req_name" runat="server" placeholder="Requester Name" required="" class="form-control">
        <asp:ListItem Text="SELECT ONE" Selected="True" Value="0"></asp:ListItem>                                                    
        </asp:DropDownList>

   </div>          
</div>   
<div class="form-group col-xs-6">  
     <asp:TextBox ID="req_email" runat="server" placeholder="Requester Email *" class="form-control"></asp:TextBox>                                                              
</div>
 
 <div class="form-group col-xs-6">
 <div class="controls">
     <asp:DropDownList ID="department" runat="server" placeholder="Department" required="" class="form-control">
         <asp:ListItem Text="SELECT ONE" Selected="True" Value="0"></asp:ListItem>                                                    
         </asp:DropDownList>
   </div>                            
</div>      
                                 
<div class="form-group col-xs-6">
 <div class="controls">
     <asp:DropDownList ID="Category" runat="server" placeholder="Category" required="" class="form-control">
         <asp:ListItem Text="SELECT ONE" Selected="True" Value="0"></asp:ListItem>                                                    
         </asp:DropDownList>
   </div>                            
</div>   
  
 <div class="form-group col-xs-6">
 <div class="controls">
     <asp:DropDownList ID="SubCategory" runat="server" placeholder="Sub Category" required="" class="form-control">
       <asp:ListItem Text="SELECT ONE" Selected="True" Value="0"></asp:ListItem>                                                    
       </asp:DropDownList>
   </div>                            
</div>         
 
<div class="form-group col-xs-6">
    <asp:TextBox ID="sla" runat="server" placeholder="SLA (Hrs) *" class="form-control"></asp:TextBox>                                
  </div>
                             
<div class="form-group col-xs-6">                                  
     <asp:TextBox ID="subject" runat="server" placeholder="Subject *" class="form-control"></asp:TextBox> 
 </div>  
                              
                              
<%--   <div class="form-group col-xs-4">
			 <select id="state" placeholder="State" disabled="" required="" class="form-control">
					<option value="" disabled="" selected="">Select State</option>
		   		    <option>Andhra Pradesh</option>				  
			  </select>
         </div>--%>

  
 <%-- <div class="form-group col-xs-12">
         <textarea runat="server" class="form-control" id="lead_remarks" rows="6" placeholder="Any information that you want to provide to help us understand Issue" 
             name="query"></textarea>
   </div>--%>



        </asp:Panel>                   
        <div class="form-group col-xs-6">
           <label style="display:block;text-align:left;"  class="control-label"> Attachment *</label>
            </div>
          <div class="form-group col-xs-6">
           <asp:HiddenField ID="hdnUploadedFileName" runat="server" />
                         <%--<asp:HiddenField ID="hdnDOCID" runat="server" />--%>
                                    <span style="float: right">
                                        <asp:FileUpload ID="file_upload" class="multi" runat="server" OnClientClick="selectFile(); return false;" onChange="uploadImage()" />
                                        <%--<asp:Button ID="Button1" runat="server" Text="Upload" Style="display: none;" /><br />--%>
               <br />   </span>             
        </div>        
        

        <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm" TargetControlID="btnShowPopupForm"
        BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupForm" runat="server" Width="400px" Height="80px" BackColor="White" Style="display: none">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" style="width:100%" >
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
                        <asp:Button ID="btnOk" runat="server" Text="Close" OnClick="Reset" CssClass="btn btn-primary" Width="80px" />
                       <%-- <asp:Button ID="btnview" runat="server" Text="View" CssClass="btnNew" OnClick="OpenWindow" Width="80px" />--%>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    
	<div class="form-group col-md-12">
	  <%--<a href="http://myndsaas.myndsolution.com/home.aspx"> <asp:Button ID="btncancel" runat="server"  class="btn btn-warning" Text="Cancel" type="button"/> </a>--%>
        <asp:Button ID="btncancel" runat="server"  class="btn btn-warning" OnClientClick="JavaScript:window.history.back(1); return false;" Text="Cancel" type="button"/>         
       <asp:Button ID="btnsubmit" runat="server" OnClick="btnsubmit_click" OnClientClick="return UpdateCall();"  class="btn btn-primary" Text="Submit"/>									
							</div>

    
                          </fieldset>
   </form>
					<br>
                  </div>
              </div>
          </div>
      </div>
  </div>
<br>
<br>
<br>
<br>
<br>
</span> 

</body></html>

<%--</asp:Content>--%>