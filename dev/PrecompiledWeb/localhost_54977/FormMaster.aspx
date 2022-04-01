<%@ page title="Form Master" language="VB" masterpagefile="~/usrFullScreenBPM.master" enableviewstate="true" autoeventwireup="false" inherits="FormMaster, App_Web_cjg31vo3" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>




<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" language="javascript">
        function Validatetrigger() {

            var tText = $("#ContentPlaceHolder1_txtTrgText").val().trim();
            var ret = true;
            if (tText == "") {
                alert("Trigger text required.");
                $("#ContentPlaceHolder1_txtTrgText").focus();
                ret = false;
            }
            return ret;
        }


    </script>
  
      <%-- <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
<%--     <script type="text/javascript" src="js/jquery.min.js">
    </script>--%>
<script type="text/javascript">
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    function IsNumeric(e) {
        var keyCode = e.which ? e.which : e.keyCode
        var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
        document.getElementById("error").style.display = ret ? "none" : "inline";
        return ret;
    }
    $(document).ready(function () {
        $(".test").change(function () {
            var values = '';
            debugger;
            $('#<%=chklFUL.ClientID %> input[type=checkbox]:checked').each(function () {
                if (values.length == 0) {
                    values = $('label[for=' + this.id + ']').html();
                }
                else {
                    values += "," + $('label[for=' + this.id + ']').html();
                }
            });
            alert(values);
            return false;
        });
    });
 </script>
  
    <script  type="text/javascript">
        function GetSelectedValue1() {
            debugger;
            var chkBox = document.getElementById("<%=chklFUL.ClientID%>");
            var checkbox = chkBox.getElementsByTagName("input");
            var objTextBox = document.getElementById("<%=txtDPFU.ClientID%>");
            var counter = 0;
            objTextBox.value = "";
            for (var i = 0; i < checkbox.length; i++) {
                if (checkbox[i].checked) {
                    var chkBoxText = checkbox[i].parentNode.getElementsByTagName('label');
                    if (objTextBox.value == "") {
                        objTextBox.value = "{" + chkBoxText[0].innerHTML + "}";
                    }
                    else {
                        objTextBox.value = objTextBox.value + "," + "{" + chkBoxText[0].innerHTML + "}";
                    }
                }
            }

        }

    </script>
     <script src="js/Utils.js" type="text/javascript"></script>
  <script type="text/javascript">
      function myFunction(id) {
          var ids = id.val()
          alert(ids)
      }
      //Function to allow only numbers to textbox

      function validate(key) {
          //getting key code of pressed key
          var keycode = (key.which) ? key.which : key.keyCode;
          var phn = document.getElementById('txtSyncTally');
          //comparing pressed keycodes
          if (!(keycode == 8 || keycode == 46) && (keycode < 48 || keycode > 57)) {
              return false;
          }
          else {
              //Condition to check textbox contains ten numbers or not
              if (phn.value.length < 10) {
                  return true;
              }
              else {
                  return false;
              }
          }
      }
</script>
    <style type="text/css" >
        .fancy-green .ajax__tab_header {
            background: url(images/green_bg_Tab.gif) repeat-x;
            cursor: pointer;
        }

        .fancy-green .ajax__tab_hover .ajax__tab_outer, .fancy-green .ajax__tab_active .ajax__tab_outer {
            background: url(images/green_left_Tab.gif) no-repeat left top;
        }

        .fancy-green .ajax__tab_hover .ajax__tab_inner, .fancy-green .ajax__tab_active .ajax__tab_inner {
            background: url(images/green_right_Tab.gif) no-repeat right top;
        }

        .fancy .ajax__tab_header {
            font-size: 13px;
            font-weight: bold;
            color: #000;
            font-family: sans-serif;
        }

            .fancy .ajax__tab_active .ajax__tab_outer, .fancy .ajax__tab_header .ajax__tab_outer, .fancy .ajax__tab_hover .ajax__tab_outer {
                height: 46px;
            }

            .fancy .ajax__tab_active .ajax__tab_inner, .fancy .ajax__tab_header .ajax__tab_inner, .fancy .ajax__tab_hover .ajax__tab_inner {
                height: 46px;
                margin-left: 16px; /* offset the width of the left image */
            }

            .fancy .ajax__tab_active .ajax__tab_tab, .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_header .ajax__tab_tab {
                margin: 16px 16px 0px 0px;
            }

        .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_active .ajax__tab_tab {
            color: #fff;
        }

        .fancy .ajax__tab_body {
            font-family: Arial;
            font-size: 10pt;
            border-top: 0;
            border: 1px solid #999999;
            padding: 8px;
            background-color: #ffffff;
        }

        .hidden {
            display: none;
        }
    </style>

     <div class="form">
         <div class="doc_header">
             Dynamic Screen Designer
        </div>
         <div class="row mg">
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <contenttemplate> 

  
        <%--<div class="doc_header"> Form Designer </div>--%>
        
            <div class="col-md-12 col-sm-12">
                             <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small"></asp:Label>
                </div>

            <div class="col-md-12 col-sm-12">
                <div class="col-md-2 col-sm-2">
                              <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Field Name" Width="99%">
                                    </asp:Label>
                    </div>
                <div class="col-md-2 col-sm-2">
                        <asp:DropDownList ID="ddlField" runat="server" CssClass="Inputform" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Width="99%">  </asp:DropDownList>
                    </div>
                <div class="col-md-2 col-sm-2">
                    <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Value" Width="99%"></asp:Label>
                    </div>
                <div class="col-md-2 col-sm-2">
                               <asp:TextBox ID="txtValue" runat="server" CssClass="Inputform" Font-Bold="True"
                                        Font-Size="Small" Width="99%"></asp:TextBox> 
                </div>
                <div class="col-md-2 col-sm-2">
                    <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                        ImageUrl="~/Images/search.png" />
                    </div>
                <div class="col-md-2 col-sm-2">
                    <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/images/Configuration.jpg" Height="16px" Width="16px" ToolTip="Automatic Document Configuration" PostBackUrl="~/AutoDocConfig.aspx"  AlternateText="Automatic Document Creation"/>
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" onclick="Add" ToolTip="ADD Form"/>
                                    &nbsp;

                     <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1000454545454; left: 50%;top : 30%;">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>

                    </div>
             </div>

            <div class="col-md-12 col-sm-12">    
                      <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
             CellPadding="2" DataKeyNames="Formid"
                    CssClass="GridView"
               AllowSorting="True" PageSize ="20" AllowPaging="True">
                    <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                    <Columns>

  <asp:TemplateField HeaderText="S.No" >    
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="50px" />
</asp:TemplateField>

  <asp:BoundField DataField="FormName" HeaderText="Form Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="formCaption" HeaderText="Caption">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                     
                        <asp:BoundField DataField="formType" HeaderText="Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="formsource" HeaderText="Form Source">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="LayoutType" HeaderText="Layout Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="DocumentType" HeaderText="Document Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="docnature" HeaderText="Doc Nature">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="isActive" HeaderText="Status">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                             
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnPreview" runat="server" ImageUrl="~/images/search.png" Height="16px" Width="16px" OnClick="PreviewHit" ToolTip ="Preview Form" AlternateText="Preview" />
                                 &nbsp;
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" ToolTip ="Edit Form Detail" AlternateText="Edit" />
                                 &nbsp;
                                 <asp:ImageButton ID="btnAddFields" runat="server" ImageUrl="~/images/addfields.jpg" Height="16px" Width="16px" OnClick="AddFields" ToolTip ="Add Fields" AlternateText="Add Fields" />
                                   &nbsp;
                                 <asp:ImageButton ID="btnFormDesg" runat="server" ImageUrl="~/images/edit.png" Height="16px" Width="16px" OnClick="ScreenDesg" ToolTip ="Design Screen" AlternateText="Design Screen" />
                                  &nbsp;
                                  <asp:ImageButton ID="btnApplyFields" runat="server" ImageUrl="~/images/process.png" Height="16px" Width="16px" OnClick="ApplyFieldsHit" ToolTip ="Apply Fields" AlternateText="Apply Custom Fields" />
                                   &nbsp; 
                                <asp:ImageButton ID="btnexpMap" runat="server" ImageUrl="~/images/account.png" Height="16px" Width="16px" OnClick="ExportMapping" ToolTip ="Export Mapping" AlternateText="Mapping Fields"/>
                                <%--<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHit" ToolTip ="Delete Form" AlternateText="Delete"/>--%>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/edit.gif" Height="16px" Width="16px" OnClick="TriggerModal" ToolTip ="Add Trigger" AlternateText="Add Trigger"/>
                                <asp:ImageButton ID="btnLock" runat="server" ImageUrl="~/images/lock.PNG" Height="16px" Width="16px" OnClick="LockForm" ToolTip ="LOCK/UNLOCK Form" AlternateText="Lock"/>&nbsp;
                                 <asp:ImageButton ID="btnvalidation" runat="server" ImageUrl="~/images/validation1.png" Height="16px" Width="16px" OnClick="ValidateHit" ToolTip ="Apply Validation" AlternateText="Apply Form Validation" />
                               <%-- <asp:ImageButton ID="btnisinline" runat="server" Visible="false"  ImageUrl="~/images/inlineedit.png" Height="16px" Width="16px"  OnClick="isinlineediting" ToolTip ="Inline Editing Configuration" AlternateText="Inline Editing Configuration" /> --%>
                                <%--<asp:ImageButton ID="btnukey" runat="server" Visible="True"  ImageUrl="~/images/uniquekey.png" Height="16px" Width="16px" onclick="Hitukey"  ToolTip ="Unique Key Configuration" AlternateText="Unique Key Configuration" /> --%>
                                    &nbsp;
                                <asp:ImageButton ID="btnisinline" runat="server" Visible="false"  ImageUrl="~/images/inlineedit.png" Height="16px" Width="16px" OnClick="isinlineediting" ToolTip ="Inline Editing Configuration" AlternateText="Inline Editing Configuration" />
                                   &nbsp;
                                <asp:ImageButton ID="btnukey" runat="server" ImageUrl="~/images/uniquekey.png" Height="16px" Width="16px" OnClick="Hitukey" ToolTip ="UniqueKey & Sorting Configuration" AlternateText="UniqueKey & Sorting Configuration" />
                                <input type="image" src="images/DocRelations.png"   onclick='javascript: return OpenRelationDiv("<%#Eval("FormName")%>    ");' alt="Relation" title="Add Update Relation" />
                                <asp:ImageButton ID="btnBalM" runat="server" ImageUrl="~/images/1414775815_41.png" Height="16px" Width="16px" OnClick="HituBalance" ToolTip ="Balance Maintenance Configuration" AlternateText="Balance Maintenance Configuration" />
                                <asp:ImageButton ID="btnxml" runat="server" Visible="True"  ImageUrl="~/images/asterisk_orange.png" Height="16px" Width="16px" OnClick="clickxmlio" ToolTip ="XML Inward Outward Configuration" AlternateText="XML Inward Outward Configuration" />
                                
                                <asp:ImageButton ID="btnDetailRule" runat="server" Visible="True"  ImageUrl="~/images/tools.png" Height="16px" Width="16px" OnClick="btnDetailRuleEngine" ToolTip ="Detail form rule engine" AlternateText="Detail rule engine" />
                                <%--Add Tally config--%>
                                <asp:ImageButton ID="btnMasterConfig" runat="server" Visible="true" ImageUrl="~/images/master_mapping.png" Height="16px" Width="16px" OnClick="btnMasterConfiguraiton" ToolTip ="Master Form Configuration" AlternateText="Master Form Configuration" />
                                <%--<asp:ImageButton ID="btnAutoConfig" runat="server" Visible="false" ImageUrl="~/images/Configuration.jpg" Height="16px" Width="16px" ToolTip="Automatic   Document Creation" OnClick="btnAutoConfig_click" AlternateText="Automatic Document Creation"/>--%>
                                   &nbsp;
                                <asp:ImageButton ID="btnshowexisting" runat="server" Visible="true" ImageUrl="~/images/ShowExisting.png" Height="16px" Width="16px" OnClick="btnshowexisting_Click" ToolTip ="Show Existing Values Configuration" AlternateText="Show Existing Values Configuration" />
                                &nbsp;
                                   <asp:ImageButton ID="btndocdtlsorting" runat="server" Visible="true" ImageUrl="~/images/docDtlsorting.png" Height="16px" Width="16px" OnClick="btndocdtlsorting_Click" ToolTip ="Show Sorting Values on Doc Detail" AlternateText="Show Sorting Values on Doc Detail" />
                                  &nbsp;
                                   <asp:ImageButton ID="imgButtonTab" runat="server" ImageAlign="Left" Visible="true" ImageUrl="~/images/tab.png" Height="16px" Width="26px" OnClick="imgButtonTab_Click"  ToolTip ="Configure Tab Functionality for Document" AlternateText="Configure Tab Functionality for Document" />
                                  &nbsp;
                                <asp:ImageButton ID="imgAccordian" runat="server" ImageAlign="Left" Visible="true" ImageUrl="~/images/accordian.png" Height="16px" Width="26px" OnClick="imgAccordian_Click"  ToolTip ="Configure accordian functionality for doc detail" AlternateText="Configure accordian functionality for doc detail" />
                                 &nbsp;
                                <asp:ImageButton ID="img_InputThruMail" runat="server" ImageAlign="Left" Visible="true" ImageUrl="~/images/InputThruMail.png" Height="16px" Width="26px" OnClick="imgInputAction_Click"  ToolTip ="Input thru Mail Configuration" AlternateText="Mail Input"  />
                                &nbsp;
                                <asp:ImageButton ID="img_LMSetting" runat="server" ImageAlign="Left" Visible="true" ImageUrl="~/images/leaseMastConfig.png" Height="16px" Width="26px" OnClick="img_LMSetting_Click" ToolTip="Master Child Configuration" AlternateText="Master Child Configuration" />
                                 &nbsp;
                                <asp:ImageButton ID="img_AutoInvsetting" runat="server" ImageAlign="Left" Visible="true" ImageUrl="~/images/leaseMastConfig.png" Height="16px" Width="26px" OnClick="img_AutoInvSetting_Click" ToolTip="Auto Invoice Configuration" AlternateText="Auto invoice Configuration" />
                            </ItemTemplate>
                            <ItemStyle Width="220px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>
         </div>    


      </contenttemplate>
    </asp:UpdatePanel>
  </div>
   <div class="row mg">    



    <asp:Button id="btnShowPopupForm" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlPopupForm" TargetControlID="btnShowPopupForm"
        CancelControlID="btnCloseForm" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupForm" runat="server" Width="1000px" Height="300px" BackColor="White" style="">
<div class="box">
     <table cellspacing="0px" cellpadding="0px" width="100%">
         <tr>
             <td width="980px">
                 <h3>New Form</h3>
             </td>
             <td style="width: 20px">
                 <asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server" />
             </td>
         </tr>
         <tr>
             <td colspan="2">
                 <div id="main">
                     <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                         <contenttemplate>
<div class="form" style="text-align:left"> 
    
<table cellspacing="5px" cellpadding="0px" width="100%" border="0">
<tr><td colspan ="2" align ="right"  >
<asp:Label ID="lblnk" runat ="server" ></asp:Label>
</td></tr>
<tr><td style="width:100%" colspan="2">   <asp:Label ID="lblForm" runat="server" Text=""></asp:Label> 
</td></tr>

<tr><td style="width:100%" colspan="2"><h2>Enter basic form information</h2></td></tr>

<tr>
   <td valign="middle" style="text-align:left;"><label title="This name will appear in the menu through which user will be able to open this form for input" > *Form Name <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtFormName" runat="server" CssClass="txtBox" 
           Width="196px" onkeyup="document.getElementById('ContentPlaceHolder1_txtFormCaption').value=this.value;  document.getElementById('ContentPlaceHolder1_txtFormDesc').value=this.value;" ></asp:TextBox> 
     </td>
</tr>
<tr>
   <td valign="middle" style="text-align:left"><label title="Form Caption will appear in the header of the form" > *Form Caption <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtFormCaption" runat="server" CssClass="txtBox" 
           Width="400px"></asp:TextBox> 
     </td>
</tr>

<tr>
   <td valign="middle" style="text-align:left"><label title="Form Description will describe about form and it will appear below to caption also in the title of the Form" > *Form Description <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtFormDesc" runat="server" CssClass="txtBox" 
           Width="400px"></asp:TextBox> 
     </td>
</tr>

<tr>
   <td style="text-align:left"> <label title="Form type decide default functionality to attach with the form.You have three options here (DOCUMENT / EVENT / MASTER. If Document is selected then this input form will have option to add workflows, Calender, Movements etc" > *Form Type <img src="Images/Help.png" alt="" /> : </label></td>
   <td style=""> 
  
   <asp:DropDownList ID="ddlFormType" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="True">
           <asp:ListItem>MASTER</asp:ListItem>
           <asp:ListItem>DOCUMENT</asp:ListItem>
          </asp:DropDownList>
   </td>
</tr>

<tr style="vertical-align:top;">
    <td style="text-align:left"> <label title="Form source decide form trigger action,it can be event driven or menu driven.If Event driven is selected then it will come in that event. No menu will be provided for its data entry" > *Form Source <img src="Images/Help.png" alt="" /> : </label></td>
   <td>
      <%-- <table cellpadding="0" cellspacing="0" border="1">
           <tr>             
               <td>--%>
     <div style="width:100%">
       <div style="width:500px;float:left;">
                    <asp:DropDownList ID="ddlFormSource" runat="server" CssClass="txtBox" 
           Width="150px" AutoPostBack="True">
           <asp:ListItem> </asp:ListItem>
           <asp:ListItem>MENU DRIVEN</asp:ListItem>
           <asp:ListItem>ACTION DRIVEN</asp:ListItem>
           <asp:ListItem>DETAIL FORM </asp:ListItem>
           <%--<asp:ListItem>Publicview Driven </asp:ListItem>
           <asp:ListItem>PublicEntry Driven </asp:ListItem>--%>
         </asp:DropDownList>
              <%-- </td>
               <td>--%>
                    <asp:CheckBox ID="chkIsDef" runat="server" Text="Is Default Value" Visible="false" AutoPostBack="true" />
                   <asp:DropDownList ID="ddlEvent" runat ="server" Width="150px" CssClass ="txtBox" AutoPostBack="True" ToolTip="Main Event" Visible="False" ></asp:DropDownList>
              <%-- </td>
               <td>--%>
                    <asp:CheckBox ID="chkdefblnk" runat="server" Text="Default Blank Rows" Visible="false"  AutoPostBack="true" />
                     <label title ="No. of Rows"></label>
       <asp:TextBox ID="txtdefblnk" runat="server" Visible="false" Width="6%" ></asp:TextBox>
                     <asp:CheckBox ID="Chkisinline" runat="server" Text="Is-InLine-Editing" Visible="false"  AutoPostBack="true" />  
              <%-- </td>
               <td rowspan="2">--%>
           </div>
            <div style="float:right; vertical-align:top">
                   <asp:Panel ID="pnlchild" runat="server"  Height="60px" ScrollBars="Both" Visible="false">
                       <span style="vertical-align:top;font-weight:bold;">Select Child Item</span>
                   <asp:CheckBoxList ID="chkChild" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                       </asp:Panel>
                </div>
         </div>
       <div id="ad" runat="server" visible="false">
           <div>
               <%--<div style="text-align:right">

               </div>--%>
               <div style="float:left;">
                   <asp:DropDownList ID="ddlSBE" runat ="server" Width="150px" CssClass ="txtBox" AutoPostBack="True" ToolTip="Sub Event" Visible="False" ></asp:DropDownList>
                   <asp:DropDownList ID="ddlStatus" runat ="server" Width="150px" CssClass ="txtBox" ToolTip="WF Status" Visible="False" ></asp:DropDownList>
                    <asp:DropDownList ID="ddlDocNature" runat ="server" Width="150px" CssClass ="txtBox" ToolTip="Document Nature" Visible="False" >
                <asp:ListItem>SELECT ONE</asp:ListItem>
                <asp:ListItem>CREATE</asp:ListItem>
                <asp:ListItem>MODIFY</asp:ListItem>
                <asp:ListItem>CANCEL</asp:ListItem>
         </asp:DropDownList>
               </div>
           </div>
       </div>
               <%--</td>--%>
       </td>
           </tr>
           <tr id="ads" runat="server" visible="false">
              <td style="text-align:right">

              </td>
               <td>
                   
              <%-- </td>
               <td>--%>
                    
              <%-- </td>
               <td>--%>
                   
              <%-- </td>
              
           </tr>
       </table>--%>
   </td>   
</tr>
<tr>
   <td style="text-align:left"> <label title="Layout decide form controls will appear in one column or two column" > *Form Layout <img src="Images/Help.png" alt="" /> : </label></td>
   <td style=""> 
      <%--<table cellpadding="0" cellspacing="0">
          <tr>
              <td>--%>
                  <asp:DropDownList ID="ddlLayout" runat="server" CssClass="txtBox" 
           Width="150px">
           <asp:ListItem>SINGLE COLUMN</asp:ListItem>
           <asp:ListItem>DOUBLE COLUMN</asp:ListItem>
           <asp:ListItem>TRIPLE COLUMN</asp:ListItem>
           <asp:ListItem>CUSTOM</asp:ListItem>
       </asp:DropDownList>
             <%-- </td>
              <td>--%>
                  <%-- <asp:DropDownCheckBoxes ID="chkChild" runat="server" 
                    AddJQueryReference="True" UseButtons="True" UseSelectAllNode="True">
                    <Style SelectBoxWidth="200" DropDownBoxBoxWidth="200" DropDownBoxBoxHeight="130" />
                    <Texts SelectBoxCaption="Select Country" />
                </asp:DropDownCheckBoxes>--%>
            <%--  </td>
              <td style="width:100%;">--%>
                  <%--  <div style="float:right;width:100%; padding-left:0px; vertical-align:top; text-align:right;">
       <asp:Panel ID="pnlchild" runat="server" Width="100%" Height="40px" ScrollBars="Both" Visible="false"> --%>
          <%-- <asp:ListBox ID="chkChild" runat="server" SelectionMode="Multiple"></asp:ListBox>--%>
          <%-- <asp:CheckBoxList ID="chkChild" runat="server" CssClass="checkboxlist"></asp:CheckBoxList>--%>
     <%--  </asp:Panel>
           </div>--%>
             <%-- </td>
          </tr>
      </table>--%>
         
          
     
          
   </td>
</tr>
<%--<tr><td valign="middle" style="text-align:right"> <label> Enable Draft </label> </td> <td><asp:TextBox ID="txtEnblDft" runat="server" CssClass="txtBox"></asp:TextBox>  </td> </tr>

<tr><td valign="middle" style="text-align:right"> <label> Enable CRM </label> </td> <td><asp:TextBox ID="txtEnblCRM" runat="server" Enabled="false" CssClass="txtBox"></asp:TextBox>  </td> </tr>


<tr><td valign="middle" style="text-align:right"> <label> Allowed File Size (MB) </label> </td> <td><asp:TextBox ID="txtAllowedUploadSize" runat="server" CssClass="txtBox"></asp:TextBox>  </td> </tr>
<tr><td valign="middle" style="text-align:right"> <label> Show Child Uploader </label> </td> <td><asp:CheckBox ID="chkshowCUploder" runat="server"/>  </td> </tr>--%>
    <tr>
     <td style="text-align:left"> <label title="Enables Draft mode Saving For Documents." > Enable Draft <img src="Images/Help.png" alt="" /> : </label></td>
     <td><asp:TextBox ID="txtEnblDft" runat="server" CssClass="txtBox"></asp:TextBox>  </td> </tr> 
 <tr>
     <td style="text-align:left"> <label title="Enables CRM feature for Action Screen/Form." > Enable CRM <img src="Images/Help.png" alt="" /> : </label></td>
     <td><asp:TextBox ID="txtEnblCRM" runat="server" Enabled="false" CssClass="txtBox"></asp:TextBox>  </td> </tr> 
<tr>
     <td style="text-align:left"> <label title="Specify Any Value in MB to Restrict Total File Size in a Form(0 for Unlimited Size)." > Allowed File Size (MB) <img src="Images/Help.png" alt="" /> : </label></td>
     <td><asp:TextBox ID="txtAllowedUploadSize" runat="server" CssClass="txtBox"></asp:TextBox>  </td> </tr>
<tr>
     <td style="text-align:left"> <label title="." > Auto Save Interval <img src="Images/Help.png" alt="" /> : </label></td>
     <td><asp:TextBox ID="txtAutosaveinterval" runat="server" CssClass="txtBox"></asp:TextBox>  </td> </tr>
<tr>
    <td style="text-align:left"> <label title="Enables Item Upload threw CSV File For Child Item Only." > Enable Uploader <img src="Images/Help.png" alt="" /> : </label></td>
    <td >
       <table cellpadding="0" cellspacing="0">
           <tr >
       <td><asp:CheckBox ID="chkshowCUploder" OnCheckedChanged="chkshowCUploder_CheckedChanged" AutoPostBack="true" runat="server"/> 
              
                   &nbsp; &nbsp;
                <div id="uploader" runat="server" visible="false" style="float:right; vertical-align:top;">
                      <asp:CheckBox ID="chkCSV" runat="server" Text="CSV" />
                           <asp:CheckBox ID="chkXML" runat="server" Text="XML" />
                   <div style="float:right;">
                    <asp:CheckBoxList ID="chkfldsep" runat="server" RepeatColumns="1" RepeatDirection="Vertical" >
                        <asp:ListItem Value="COMMA" Selected="True">COMMA</asp:ListItem>
                        <asp:ListItem Value="PIPE">PIPE</asp:ListItem>
                    </asp:CheckBoxList>
                       </div>
                </div>
                      
</td>
                                              </tr> </table> </td>

</tr>
<tr>
    <td style="text-align:left"> <label title="Enables Master Forms referancess Unique Value to Save in User Master(Export Mapping)." > Enable UserCreation <img src="Images/Help.png" alt="" /> : </label></td>
    <td><asp:CheckBox ID="chkcreation" runat="server" Checked="false"/>  </td> </tr>
     <asp:PlaceHolder runat="server" ID="test">
         <tr>

    <td style="text-align:left"> <label title="Enables Item Upload threw CSV File For Child Item Only." > Enable Sync Tally <img src="Images/Help.png" alt="" /> : </label></td>
    <td><asp:CheckBox ID="CheckBox2"   runat="server"/>
        <asp:TextBox ID="txtSyncTally" CssClass="txtBox" onkeypress="return validate(event)" runat="server"></asp:TextBox>
    </td>
       
       
</asp:PlaceHolder>

<tr>

      <td style="text-align:left"> <label title="Allow Multiuse" >   Allow Multiuse <img src="Images/Help.png" alt="" /> : </label></td>
           <%--  </td style="text-align:right">
             <caption>
                 <label title="Allow Multiuse">
                 Allow Multiuse
                 <img src="Images/Help.png" alt="" />
                 :
                 </label>--%>
                 <td>
                     <asp:CheckBox ID="Chkmultiuse" runat="server" Checked="false" />
                 </td>
             </caption>
         </tr>
    <%--Add Tally Synch by mayank --%>
    <tr id="TallyRegistration" runat="server" visible="false">
         <td style="text-align:left"> <label title="Tally Registration" >Enable Tally Registration <img src="Images/Help.png" alt="" /> : </label></td>
        <td>
            <asp:CheckBox ID="chkEnableTallyRegistration" runat="server" Checked="false" />
        </td>
    </tr>
    <%--Add Tally Synch by mayank --%>   
    <tr id="EnabledAddRow" runat="server" visible="false">
        <td style="text-align:left"> <label title="Enabled Add Row" >Enable Add Rows <img src="Images/Help.png" alt="" /> : </label></td>
        <td>
            <asp:CheckBox ID="chkEnableAddrow" runat="server" Checked="false" />
        </td>
    </tr>
    <%--Add Show Existing--%>
    <tr id="ShowExisting" runat="server" visible="false">
        <td style="text-align:left"> <label title="Enable Show Eisting Value" >Enable Show Existing Value<img src="Images/Help.png" alt="" /> : </label></td>
        <td><asp:TextBox ID="txtESEV" runat="server" CssClass="txtBox"></asp:TextBox></td>
    </tr>
    <%--Add Show Existing--%>
     <tr>
         <td colspan="2" style="text-align:left">
             <asp:CheckBox ID="chkEnblWS" runat="server" Text="Enable Web Service" />
             <asp:CheckBox ID="chkCalendar" runat="server" Text="Calendar" />
             <asp:CheckBox ID="chkWF" runat="server" Text="WorkFlow" />
             <asp:CheckBox ID="chkHistory" runat="server" Text="History" />
             <asp:CheckBox ID="chkrole" runat="server" Text="Role Definition" />
             <asp:CheckBox ID="ChKPV" runat="server" Text="PUBLIC VIEW" />
             <asp:CheckBox ID="ChKPE" runat="server" Text="PUBLIC ENTRY" />
         </td>
    </tr>
    
    <tr>
        <td></td>
        <td align="left">
            <asp:Button ID="btnlogin" runat="server" CssClass="btnNew" Text="Save" Width="100Px" />
        </td>
    </tr>
    </td>
     </table>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</div>
</td></tr>
</table>
</div> 
 </asp:Panel>

    <asp:ModalPopupExtender id="mdladdtrg" runat="server" PopupControlID="pnltrigger" TargetControlID="btnAdTrigger" CancelControlID="imgtrclose" BackgroundCssClass="modalbackground" dropshadow="true">
    </asp:ModalPopupExtender>
    <asp:Button id="btnAdTrigger" runat="server" style="Display: none" />
    <asp:ModalPopupExtender id="MdlBalance" runat="server" PopupControlID="pnlBalancemaintanance" TargetControlID="Button6" CancelControlID="ImageButton4" BackgroundCssClass="modalbackground" dropshadow="true">
    </asp:ModalPopupExtender>
    <asp:Button id="Button6" runat="server" style="Display: none" />

    <asp:ModalPopupExtender ID="mdlTrgDel" runat="server" TargetControlID="btnhdnDelTrg" PopupControlID="pnlDeltrg" CancelControlID="ImageButton4" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Button id="btnhdnDelTrg" runat="server" style="Display: none" />
    <asp:Panel ID="pnlDeltrg" runat="server" Width="500px" style="Display: none; z-index: 999999;" BackColor="Aqua">
        <div class="box" style="border: 1px solid #e1e1e1;">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Trigger Delete : Confirmation </h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="imClosedelTrg" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="upDelTrg" runat="server" UpdateMode="Conditional">
                            <contenttemplate> 
                            <h2> <asp:Label ID="lblTrgDel" runat="server" Font-Bold="True" ForeColor="Red" Width="97%" Font-Size="X-Small" ></asp:Label></h2>
                            <div style="width:100%;text-align:right; padding:0 5px 10px 0;" >
                                <asp:Button ID="btntrgdel" runat="server" Text="Yes Delete"  Width="90px" OnClick="DeleteTrigger" CssClass="btnNew" Font-Size="X-Small" />
                            </div> 
                        </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlTrigger" runat="server" Width="800px" style="Display: none" BackColor="Aqua">
        <div class="box" style="border: 2px solid #e1e1e1; text-align: center;">


            <table cellspacing="0px" cellpadding="0px" width="100%">


                <tr>
                    <td>
                        <h3>
                            <asp:Label ID="Fnameontrigger" runat="server"></asp:Label>
                            Add/Update Trigger</h3>

                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="imgtrclose" ImageUrl="images/close.png" OnClick="hidetrigger" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <h2>
                            <asp:Label ID="lblTrigger" runat="server" Font-Bold="True"></asp:Label></h2>
                        <div style="width: 100%; text-align: right">
                            <asp:UpdatePanel ID="Uptrigger" runat="server" UpdateMode="Conditional">
                                <contenttemplate> 
                            <table cellspacing="0px" cellpadding="0px" width="100%">
                                <tr style="height:25px;">
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td width="30%">Trigger Text:</td>
                                    <td width="70%">
                                        <asp:TextBox runat="server" ID="txtTrgText" TextMode="MultiLine" Columns="20" Rows="10" ></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                     <asp:CheckBox ID="chkTrgEdit" runat="server" Text="On Edit" /> &nbsp;&nbsp;&nbsp; <asp:CheckBox ID="chkTrgOnCreate" runat="server" Text="On Create" /> 
                                        <asp:Button ID="btnAddTrigger" OnClientClick="javascript:return Validatetrigger();" OnClick="AddTrigger"  runat="server" Text="Add Trigger" />
                                        <asp:Button ID="btnEditTrigger" OnClientClick="javascript:return Validatetrigger();" OnClick="UpdateTrigger"  runat="server" Text="Update Trigger" Visible="false"  />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center" style="width:100%;">
                                             <asp:Panel ID="pnltrgGrd" runat="server" Height="200px" ScrollBars="Auto" Width="100%">
                                                <asp:GridView ID="gvTrg" runat="server" AllowPaging="false" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" DataKeyNames="tid" >
                                                    <FooterStyle CssClass="FooterStyle"/>
                                                    <RowStyle  CssClass="RowStyle"/>
                                                    <EditRowStyle  CssClass="EditRowStyle" />
                                                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                                                    <PagerStyle  CssClass="PagerStyle" />
                                                    <HeaderStyle  CssClass=" HeaderStyle" />
                                                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="S.No">
                                                            <ItemTemplate>
                                                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%"  HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="BaseDocType" HeaderText="Base Doc Type">
                                                            <HeaderStyle HorizontalAlign="Left" Width="15%" />
                                                            <ItemStyle HorizontalAlign="Left" Width="15%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="DocType" HeaderText="DOC Type">
                                                            <HeaderStyle HorizontalAlign="Left" Width="15%" />
                                                            <ItemStyle HorizontalAlign="Left" Width="15%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="TriggerText" HeaderText="Trigger Text">
                                                            <HeaderStyle HorizontalAlign="Left" Width="50%" />
                                                            <ItemStyle HorizontalAlign="Left" Width="50%" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action"> 
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnTrgEdit" OnClick="BtnTgrHit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip ="Edit Trigger" AlternateText="Edit" />
                                                                <asp:ImageButton ID="btnTrgdel" runat="server" OnClick="DeleteHitTrigger" AlternateText="Delete Trigger" Height="16px" ImageUrl="~/images/Cancel.gif" ToolTip="Delete Trigger" Width="16px" /> &nbsp;
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Left" Width="15%" />
                                                            <ItemStyle HorizontalAlign="Left" Width="15%" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                             </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        </ContentTemplate> 
               </asp:UpdatePanel> 
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlBalancemaintanance" runat="server" Width="800px" style="Display: none" BackColor="Aqua">
        <div class="box" style="border: 2px solid #e1e1e1; text-align: center; overflow: auto;">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 98%">
                        <h3>Add Update Balance Maintanance</h3>
                    </td>
                    <td style="width: 2%">
                        <asp:ImageButton ID="ImageButton4" ImageUrl="images/close.png" OnClick="hideBAlModal" runat="server" />
                    </td>
                </tr>
                <tr style="height: 10px;">
                    <td colspan="2"></td>
                </tr>
                 <tr>
                    <td style="width: 100%" colspan="2">
                        <asp:UpdatePanel ID="UPB" runat="server" UpdateMode="Conditional">
                            <contenttemplate>
                             <table style="width:100%">
                       <tr>
                          <%-- <td>Balance Maintenance Mode</td>
                           <td>
                               <asp:DropDownList ID="ddlBMode" runat="server" Width="200" CssClass="form-control">
                                   <asp:ListItem Value="0">--Select--</asp:ListItem>
                                   <asp:ListItem Value="Yearly">Yearly</asp:ListItem>
                                   <asp:ListItem Value="Half Yearly">Half Yearly</asp:ListItem>
                                   <asp:ListItem Value="Quaterly">Quaterly</asp:ListItem>
                                   <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                               </asp:DropDownList>
                           </td>--%>
                           <%--Chages by Mayank--%>
                            <td >
                              Relation Type
                          </td>
                           <td>
                               <asp:DropDownList ID="ddlRelation_Type" runat="server" Width="200" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRelation_Type_SelectedIndexChanged">
                                   <asp:ListItem>Main</asp:ListItem>
                                   <asp:ListItem>Child</asp:ListItem>
                               </asp:DropDownList>
                           </td>
                            <td>
                               
                            </td>
                          <td colspan="2">
                              <table id="chld" runat="server" cellpadding="0" cellspacing="1" visible="false" width="100%">
                                  <tr>
                                      <td style="width:36%;">
                                          Child
                                      </td>
                                      <td>
                                          <asp:DropDownList ID="ddlChild" runat="server" Width="200" CssClass="form-control" OnSelectedIndexChanged="ddlChild_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                      </td>
                                  </tr>
                              </table>
                          </td>
                          
                         
                       </tr>
                            <tr>
                                 <td>
                               Relation Doc Name
                           </td>
                           <td>
                               <asp:DropDownList ID="ddlRel_doc_type"  runat="server" Width="200" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRel_doc_type_SelectedIndexChanged">

                               </asp:DropDownList>
                           </td>
                                <td>

                                </td>
                                      <td>
                                          Source 1
                                      </td>
                                      <td>
                                          <asp:DropDownList ID="ddlSource" runat="server" Width="200" CssClass="form-control"></asp:DropDownList>
                                      </td>
                                  </tr>
                       <tr>
                            <td>
                                          Source 2
                                      </td>
                                      <td> 
                                          <asp:DropDownList ID="ddlSTarget" runat="server" Width="200"  CssClass="form-control"></asp:DropDownList>
                                          
                                      </td>
                            <td></td>
                           <td>
                               Item Number
                           </td>
                           <td>
                               <asp:DropDownList Width="200" ID="ddlItemN" runat="server" CssClass="form-control">
                                   <asp:ListItem Value="0">--Select--</asp:ListItem>
                               </asp:DropDownList>
                           </td>
                       </tr>
                                 <tr>
                                      <td>Effective Amount Field</td>
                           <td>
                               <asp:DropDownList ID="ddlEAmountField" runat="server" CssClass="form-control" Width="200">
                                   <asp:ListItem Value="0">--Select--</asp:ListItem>
                               </asp:DropDownList>
                           </td>
                                     
                                <td></td>
                                     <td>
                               Effective Date Field
                             <asp:HiddenField ID="hdnFromName" runat="server" />
                           </td>
                           <td>
                               <asp:DropDownList ID="ddlEDate" runat="server" CssClass="form-control" Width="200" >
                                   <asp:ListItem Value="0">--Select--</asp:ListItem>
                               </asp:DropDownList>
                           </td>
                                        
                                    
                                 </tr>
                                 <tr>
                                     <td>
                                         Action 
                                     </td>
                                     <td>
                                         <asp:DropDownList ID="ddlAction" Width="200" runat="server" CssClass="form-control">
                                             <asp:ListItem Value="0">--Select--</asp:ListItem>
                                             <asp:ListItem>Add</asp:ListItem>
                                             <asp:ListItem>Subtract</asp:ListItem>
                                         </asp:DropDownList>
                                     </td>
                                     <td>

                                     </td>
                                     <td colspan="2"></td>

                                 </tr>

                        <tr>
                           <td colspan="5" style="height:20px;">
                           </td>
                       </tr>
                       <tr>
                           <td colspan="2">
                               <asp:Label runat="server" Font-Bold="true" ID="lblBalInfo"></asp:Label>
                           </td>
                           <td colspan="3" style="text-align:right; padding-right:10px;">
                               <asp:Button ID="btnBalSubmit" runat="server" Text="Submit" OnClientClick="javascript:return ValidateBal();" CssClass="myButton2" />
                           </td>
                       </tr>
                       <tr>
                           <td colspan="5" style="height:10px;">
                           </td>
                       </tr>
                   </table>
                       </contenttemplate>
                        </asp:UpdatePanel>

                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button id="btnShowPopupEdit" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="900px" Height="500px" ScrollBars="Auto">
<div class="box">
    <table cellspacing="2px" cellpadding="2px" width="100%">
        <tr>


            <td style="width: 880px; height: 31px;">
                <div style="width: auto; height: auto; vertical-align: top">
                    <asp:UpdatePanel ID="updatefname" runat="server" UpdateMode="Conditional">
                        <contenttemplate><h3> <asp:Label ID="Fname" runat="server"> </asp:Label>&nbsp;NEW/UPDATE FIELDS</h3></contenttemplate>
                    </asp:UpdatePanel>
                </div>
            </td>

            <td style="width: 20px; height: 31px;">
                <asp:ImageButton ID="btnCloseEdit"
                    ImageUrl="images/close.png" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align: left">

                <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                    <contenttemplate> 
<div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
<tr>
    <td  colspan="2">
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
    </td>

</tr>
<tr>
      <td colspan ="2" > <label> *Field Type : </label>
       <asp:DropDownList ID="ddlType" runat="server" CssClass="txtBox" 
           Width="150px" AutoPostBack="True">
           <asp:ListItem>Please Select</asp:ListItem>
           <asp:ListItem>Auto Number</asp:ListItem>
             <asp:ListItem>New Auto Number</asp:ListItem>
           <asp:ListItem>Text Box</asp:ListItem>
           <asp:ListItem>Text Area</asp:ListItem>
           <asp:ListItem>Drop Down</asp:ListItem>
           <asp:ListItem>List Box</asp:ListItem>
           <asp:ListItem>Check Box</asp:ListItem>
           <asp:ListItem>CheckBox List</asp:ListItem>
           <asp:ListItem>File Uploader</asp:ListItem>
           <asp:ListItem>Lookup</asp:ListItem>
           <asp:ListItem>LT Lookup</asp:ListItem>
            <asp:ListItem>Calculative Field</asp:ListItem>
            <asp:ListItem >Child Item</asp:ListItem>
            <asp:ListItem >Child Item Total</asp:ListItem>
           <%--Added By Mayank--%>
           <%--  <asp:ListItem >Child Item Specific Text</asp:ListItem>--%>
             <asp:ListItem >Self Reference</asp:ListItem>
             <asp:ListItem >Parent Field</asp:ListItem>
             <asp:ListItem>Parent Value</asp:ListItem>
             <asp:ListItem >Formula Field</asp:ListItem>
            <asp:ListItem>Signature</asp:ListItem>
             <asp:ListItem>Geo Point</asp:ListItem>
            <asp:ListItem>Geo Fence</asp:ListItem>
            <asp:ListItem >Child Item MAX</asp:ListItem>
            <asp:ListItem>LookupDDL</asp:ListItem>
           <asp:ListItem>Catchment</asp:ListItem>
           <asp:ListItem>Advance Formula</asp:ListItem>
           <asp:ListItem>Multi LookUP</asp:ListItem>
           <%--add mulit tookup ddl--%>
           <asp:ListItem>Multi LookupDDL</asp:ListItem>
           <asp:ListItem>Separator</asp:ListItem>
           <asp:ListItem>Variance</asp:ListItem>
       </asp:DropDownList>
       &nbsp;&nbsp;
     </td>
    

</tr>
<tr>
              <asp:HiddenField ID="hdnDPName" runat="server" />
            <td align="left">
                <asp:Panel ID="PnlGrid" runat="server" Height="200px" ScrollBars="Auto"
                    Width="100%">
                    <asp:GridView ID="gvUsers" runat="server" AllowPaging="false"
                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        DataKeyNames="Fieldid">
                        <footerstyle cssclass="FooterStyle" />
                        <rowstyle cssclass="RowStyle" />
                        <editrowstyle cssclass="EditRowStyle" />
                        <selectedrowstyle cssclass="SelectedRowStyle" />
                        <pagerstyle cssclass="PagerStyle" />
                        <headerstyle cssclass=" HeaderStyle" />
                        <alternatingrowstyle cssclass="AlternatingRowStyle" />
                        <columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                 <asp:BoundField DataField="Displayname" HeaderText="Field Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="FieldType" HeaderText="FieldType">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="IsActive" HeaderText="Status">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="isrequired" HeaderText="Required">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="datatype" HeaderText="DataType">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="minlen" HeaderText="Min.Length">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="maxlen" HeaderText="Max.Length">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>
                         <asp:ImageButton ID="btnDown" runat="server" AlternateText="Move Down" 
                             Height="16px" ImageUrl="~/images/down.png" OnClick="PositionDown" 
                             ToolTip="Move Down" Width="16px" />
                         &nbsp;
                         <asp:ImageButton ID="btnUp" runat="server" AlternateText="Move Up" 
                             Height="16px" ImageUrl="~/images/up.png" OnClick="PositionUp" ToolTip="Move Up" 
                             Width="16px" />
                         &nbsp;
                         <asp:ImageButton ID="btnEditField" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHitField" ToolTip ="Edit fields" AlternateText="Edit" />
                     
                         <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/access.png" Height="16px" Width="16px" OnClick="CommonPropertiesofField" ToolTip ="Common Properties Fields" AlternateText="Edit" />
                         
                           <asp:ImageButton ID="UPDField" runat="server" AlternateText="Add Extra properties" Height="16px" ImageUrl="~/images/save.png" OnClick="KickFieldHit" 
                             ToolTip="Set Kicking Field" Width="16px" /> &nbsp;
                            <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitUser" 
                             ToolTip="Delete" Width="16px" />
                          <asp:ImageButton ID="btnCST" runat="server" AlternateText="Child Specific Text" ToolTip="Child Specific Text" Height="16px" Width="16px" ImageUrl="~/images/img_CST.jpg" OnClick="CSTClick" />
                         <asp:ImageButton ID="imageBtnUserConnectivity" runat="server" AlternateText="Mapping with Master" ToolTip="Mapping with Master" Height="16px" Width="16px" ImageUrl="~/images/usrconnectivity.png" OnClick="MasterMapping" />
                         </ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" Width="140px" />
                 </asp:TemplateField>
             </columns>
                    </asp:GridView>
                </asp:Panel>
            </td>
            <tr>
                <td>&nbsp;</td>
                <td align="left">
                    <div style="width: 100%; text-align: right">
                        <asp:Button ID="btnActEdit" runat="server" CssClass="btnNew" Font-Bold="True"
                            Font-Size="X-Small" OnClick="EditRecord" Text="Update" Visible="false"
                            Width="100px" />
                    </div>
                    <br />
                </td>
            </tr>
        </tr>
    </table>
</div>
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
    <%--Added functionality for docdetail Accordian --%>
    <asp:Button id="btnAccordianEdit" runat="server" style="display: none" />
     <asp:ModalPopupExtender ID="btnAccordianEdit_ModalPopupExtende" runat="server"
        TargetControlID="btnAccordianEdit" PopupControlID="pnlPopupAccordianEdit"
        CancelControlID="btnAccordianClose" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>

      <asp:Panel ID="pnlPopupAccordianEdit" runat="server" Width="960px" Height="700px" ScrollBars="Auto">
          <div class="box">
                <table cellspacing="2px" cellpadding="2px" width="100%">
                     <tr>
                         <td style="width: 880px; height: 31px;">
                              <div style="width: auto; height: auto; vertical-align: top">
                                   <asp:UpdatePanel ID="UpdatePanelAccordianHeader" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <contenttemplate>
                                          <h3> Document Detail Panel Configuration for - <asp:Label ID="labAccordianPanel" runat="server"> </asp:Label> </h3>
                                    </contenttemplate>
                                </asp:UpdatePanel>
                             
                             </div>
                      </td>
                          <td style="width: 20px; height: 31px;">
                                 <asp:ImageButton ID="btnAccordianClose" ImageUrl="images/close.png" runat="server" />       
                          </td>
                    </tr>
                    <tr>
                         <td colspan="2" style="text-align: left">
                             <asp:UpdatePanel ID="UpdatePanelAccordian" runat="server" UpdateMode="Conditional">
                                 <contenttemplate>
                                 <div class="form">
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0">
                                         <tr>
                                               <td align="left" style="width:14%">
                                                 <label>Panel Name</label></td>
                                                  <td align="left" style="width:28%" >
                                                      <asp:Label ID="Label27" runat="server" Visible="false"></asp:Label>
                                                         <asp:TextBox ID="txtAcc_PanelName" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
                                                      <asp:HiddenField ID="hdnAccordian_PanelID" runat="server" />
                                                   </td>
                                                <td align="left" style="width:14%">
                                                 <label>Display Order</label></td>
                                                  <td align="left" style="width:28%" >
                                                         <asp:TextBox ID="txtAcc_DisplayOrder" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
                                                  </td>
                                            </tr>
                                              <tr>
                                                <td align="left" style="width:14%">
                                                 <label>Configured Fields</label></td>
                                                    <td align="left" colspan="3">
                                                <asp:Panel ID="pnlAcc_ConfiguredDetails" runat="server" Width="100%" ScrollBars="Both" Height="250px" BorderStyle="Solid">
                                                    <asp:CheckBoxList ID="chkAccFields" runat="server" Height="100%" RepeatColumns="3"  Width="100%"></asp:CheckBoxList>
                                                </asp:Panel>
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                               <td align="left" style="width:14%">
                                                 <label>Is Active</label></td>
                                                  <td align="left" style="width:28%" >
                                                    <asp:CheckBox ID="chkIsActive" runat="server" Width="50%" CssClass="txtBox" />
                                                         <%--<asp:TextBox ID="TextBox2" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>--%>
                                                   </td>
                                                <td align="left" style="width:14%">
                                                 <label>Is HeadInclude</label></td>
                                                  <td align="left" style="width:28%" >
                                                       <asp:CheckBox ID="chkIsHeadInclude" runat="server" Width="50%" CssClass="txtBox" />
                                                  </td>
                                            </tr>
                                        <tr>
                                             <td align="left" style="width:14%">
                                                 <label>Restricted Roles <br> (Comma Delimited)</label></td>
                                            <td colspan="3">
                                               <asp:TextBox ID="txtResRoles" runat="server"  CssClass="txtBox" Width="70%"></asp:TextBox>
                                            </td>
                                        </tr>
                                           <tr>
                                            <td colspan="4" style="text-align:center;">
                                                <asp:Button ID="btnDocPanelConfiguration" runat="server" Text="Save Doc Panel Configuration" CssClass="btnNew" OnClick="btnDocPanelConfiguration_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                    <asp:GridView ID="grdDocDetailPanel" runat="server" AllowPaging="false"
                                                     AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" Width="100%"
                        DataKeyNames="panel_id">
                        <footerstyle cssclass="FooterStyle" />
                        <rowstyle cssclass="RowStyle" />
                        <editrowstyle cssclass="EditRowStyle" />
                        <selectedrowstyle cssclass="SelectedRowStyle" />
                        <pagerstyle cssclass="PagerStyle" />
                        <headerstyle cssclass=" HeaderStyle" />
                        <alternatingrowstyle cssclass="AlternatingRowStyle" />
                        <columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                 <asp:BoundField DataField="panel_id" HeaderText="Panel ID">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="Panel_Name" HeaderText="Panel Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="AccessDisplayName" HeaderText="Access Display Fields">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="AccessFieldId" HeaderText="Access Fields">
                 <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>
                </asp:BoundField>
                 <asp:BoundField DataField="documenttype">
                 <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="Panel_displayOrder" HeaderText="Display Order">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:TemplateField HeaderText="Active">
                  <ItemTemplate>
                     <%#IIf(Boolean.Parse(Eval("IsActive").ToString()), "Yes", "No")%>
                  </ItemTemplate>
                  </asp:TemplateField>
                 <asp:BoundField DataField="IsActive" HeaderText="Is Active">
                 <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>
                 </asp:BoundField>
                 <asp:BoundField DataField="IsHeadInclude" HeaderText="Is Head Include">
                  <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>
                 </asp:BoundField>
                  <asp:BoundField DataField="RestrictedRoles">
                 <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                  <asp:TemplateField HeaderText="Active">
                  <ItemTemplate>
                     <%#IIf(Boolean.Parse(Eval("IsHeadInclude").ToString()), "Yes", "No")%>
                  </ItemTemplate>
                  </asp:TemplateField>
                 <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>
                         <asp:ImageButton ID="btnEditDocDetail" runat="server" AlternateText="Edit" 
                             Height="16px" ImageUrl="~/images/edit.png" OnClick="btnEditDocDetail_Click" 
                             ToolTip="Edit Panel Configuration" Width="16px" />
                           &nbsp;
                         <asp:ImageButton ID="btnDeleteTab" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Delete.png" ToolTip="Delete tab panel and remove fields from panel" Width="16px" OnClick="btnDeleteAccordian_Click" OnClientClick=" return confirm('Are you sure you want to delete this configuration !!');" />
                         </ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" Width="140px" />
                 </asp:TemplateField>
             </columns>
                    </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                 </div>
                              </contenttemplate>
                              </asp:UpdatePanel>
                         </td>
                    </tr>
                </table>
          </div>
      </asp:Panel>

    <%--Added functionality for docdetail Accordian --%>
    <%--Added functionality for Tab document configuration--%>
    <asp:Button id="btnTabPopupEdit" runat="server" style="display: none" />
     <asp:ModalPopupExtender ID="btnTabEdit_ModalPopupExtende" runat="server"
        TargetControlID="btnTabPopupEdit" PopupControlID="pnlPopupTabEdit"
        CancelControlID="btnTabClose" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
      <asp:Panel ID="pnlPopupTabEdit" runat="server" Width="900px" Height="500px" ScrollBars="Auto">
          <div class="box">
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                         <td style="width: 880px; height: 31px;">
                              <div style="width: auto; height: auto; vertical-align: top">
                                   <asp:UpdatePanel ID="UpdatePanelTabHeader" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <contenttemplate>
                                          <h3> <asp:Label ID="labTabPanel" runat="server"> </asp:Label></h3>
                                    </contenttemplate>
                                </asp:UpdatePanel>
                             
                             </div>
                      </td>
                          <td style="width: 20px; height: 31px;">
                                 <asp:ImageButton ID="btnTabClose" ImageUrl="images/close.png" runat="server" />       
                          </td>
                    </tr>
                    <tr>
                            <td colspan="2" style="text-align: left">
                                 <asp:UpdatePanel ID="UpdatePanelTab" runat="server" UpdateMode="Conditional">
                                 <contenttemplate>
                                 <div class="form">
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0">
                                            <tr>
                                               <td align="left" style="width:14%">
                                                 <label>Panel Name</label></td>
                                                  <td align="left" style="width:28%" >
                                                      <asp:Label ID="lblPanelID" runat="server" Visible="false"></asp:Label>
                                                         <asp:TextBox ID="txtTabpanelName" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
                                                   </td>
                                                <td align="left" style="width:14%">
                                                 <label>Display Order</label></td>
                                                  <td align="left" style="width:28%" >
                                                         <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
                                                  </td>
                                            </tr>
                                        <tr>
                                            <td align="left" style="width:14%">
                                                 <label>Configured Fields</label></td>
                                            <td align="left" colspan="3">
                                                <asp:Panel ID="pnlConfiguredFields" runat="server" Width="100%" ScrollBars="Both" Height="120px" BorderStyle="Solid">
                                                    <asp:CheckBoxList ID="chkFields" runat="server" Height="100%" Width="100%"></asp:CheckBoxList>
                                                </asp:Panel>
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="text-align:center;">
                                                <asp:Button ID="btnSubmitConfigure" runat="server" Text="Save Panel Configuration" CssClass="btnNew" OnClick="btnSubmitConfigure_Click" />
                                            </td>
                                        </tr>
                                          <tr>
                                            <td colspan="4">
                                                &nbsp;
                                            </td>
                                        </tr>
                                            <tr>
                                                <td colspan="4">
                                                      <asp:GridView ID="grdTabConfugureData" runat="server" AllowPaging="false"
                                                     AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" Width="100%"
                        DataKeyNames="docPanelID">
                        <footerstyle cssclass="FooterStyle" />
                        <rowstyle cssclass="RowStyle" />
                        <editrowstyle cssclass="EditRowStyle" />
                        <selectedrowstyle cssclass="SelectedRowStyle" />
                        <pagerstyle cssclass="PagerStyle" />
                        <headerstyle cssclass=" HeaderStyle" />
                        <alternatingrowstyle cssclass="AlternatingRowStyle" />
                        <columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                 <asp:BoundField DataField="docPanelID" HeaderText="Panel ID">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="PanelName" HeaderText="Panel Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="Displayname" HeaderText="Display Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                  <asp:BoundField DataField="flds">
                      <HeaderStyle CssClass="hidden" />
                       <ItemStyle CssClass="hidden"/>
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="documenttype">
                 <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="displayorder" HeaderText="Display Order">
                <%-- <HeaderStyle CssClass="hidden" />
                 <ItemStyle CssClass="hidden"/>--%>
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                         <asp:ImageButton ID="btnDown" runat="server" AlternateText="Edit" 
                             Height="16px" ImageUrl="~/images/edit.png" OnClick="imgEdit_Click" 
                             ToolTip="Edit Panel Configuration" Width="16px" />
                           &nbsp;
                         <asp:ImageButton ID="btnDeleteTab" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Delete.png" ToolTip="Delete tab panel and remove fields from panel" Width="16px" OnClick="btnDeleteTab_Click" OnClientClick=" return confirm('Are you sure you want to delete this configuration !!');" />
                         </ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" Width="140px" />
                 </asp:TemplateField>
             </columns>
                    </asp:GridView>
                                                 
                                                </td>
                                            </tr>
                                            </table>
                                     </div>
                                 </contenttemplate> 
                                 </asp:UpdatePanel>
                            </td>
                    </tr>
                </table>
          </div>
      </asp:Panel>

    <%--Added functionality for Tab document configuration--%>


    <asp:Button id="btnSortingPopupEdit" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnSortingEdit_ModalPopupExtende" runat="server"
        TargetControlID="btnSortingPopupEdit" PopupControlID="pnlPopupSortingEdit"
        CancelControlID="btnSortingCloseEdit" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupSortingEdit" runat="server" Width="900px" Height="500px" ScrollBars="Auto">
<div class="box">
    <table cellspacing="2px" cellpadding="2px" width="100%">
        <tr>


            <td style="width: 880px; height: 31px;">
                <div style="width: auto; height: auto; vertical-align: top">
                    <asp:UpdatePanel ID="UpdatePanelSorting1" runat="server" UpdateMode="Conditional">
                        <contenttemplate><h3> <asp:Label ID="Label24" runat="server"> </asp:Label>&nbsp;Set Doc Detail Ordering</h3></contenttemplate>
                    </asp:UpdatePanel>
                </div>
            </td>

            <td style="width: 20px; height: 31px;">
                <asp:ImageButton ID="btnSortingCloseEdit"
                    ImageUrl="images/close.png" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align: left">

                <asp:UpdatePanel ID="UpdatepanelSorting" runat="server" UpdateMode="Conditional">
                    <contenttemplate> 
<div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
<tr>
    <td  colspan="2">
    <asp:Label ID="Label25" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
    </td>
    </tr>

        </tr>
        <tr>
              <asp:HiddenField ID="HiddenField1" runat="server" />
            <td align="left">
                <asp:Panel ID="Panel8" runat="server" Height="200px" ScrollBars="Auto"
                    Width="100%">
                    <asp:GridView ID="GridViewSorting" runat="server" AllowPaging="false"
                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        DataKeyNames="Fieldid">
                        <footerstyle cssclass="FooterStyle" />
                        <rowstyle cssclass="RowStyle" />
                        <editrowstyle cssclass="EditRowStyle" />
                        <selectedrowstyle cssclass="SelectedRowStyle" />
                        <pagerstyle cssclass="PagerStyle" />
                        <headerstyle cssclass=" HeaderStyle" />
                        <alternatingrowstyle cssclass="AlternatingRowStyle" />
                        <columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                 <asp:BoundField DataField="Displayname" HeaderText="Field Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="FieldType" HeaderText="FieldType">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="IsActive" HeaderText="Status">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="isrequired" HeaderText="Required">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="datatype" HeaderText="DataType">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="minlen" HeaderText="Min.Length">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="maxlen" HeaderText="Max.Length">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>
                         <asp:ImageButton ID="btnDown" runat="server" AlternateText="Move Down" 
                             Height="16px" ImageUrl="~/images/down.png" OnClick="SearchingPositionDown" 
                             ToolTip="Move Down" Width="16px" />
                         &nbsp;
                         <asp:ImageButton ID="btnUp" runat="server" AlternateText="Move Up" 
                             Height="16px" ImageUrl="~/images/up.png" OnClick="SearchingPositionUp" ToolTip="Move Up" 
                             Width="16px" />
                         &nbsp;
                      <%--   <asp:ImageButton ID="btnEditField" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHitField" ToolTip ="Edit fields" AlternateText="Edit" />
                     
                         <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/access.png" Height="16px" Width="16px" OnClick="CommonPropertiesofField" ToolTip ="Common Properties Fields" AlternateText="Edit" />
                         
                           <asp:ImageButton ID="UPDField" runat="server" AlternateText="Add Extra properties" Height="16px" ImageUrl="~/images/save.png" OnClick="KickFieldHit" 
                             ToolTip="Set Kicking Field" Width="16px" /> &nbsp;
                            <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitUser" 
                             ToolTip="Delete" Width="16px" />
                          <asp:ImageButton ID="btnCST" runat="server" AlternateText="Child Specific Text" ToolTip="Child Specific Text" Height="16px" Width="16px" ImageUrl="~/images/img_CST.jpg" OnClick="CSTClick" />--%>
                         </ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" Width="140px" />
                 </asp:TemplateField>
             </columns>
                    </asp:GridView>
                </asp:Panel>
            </td>
            <tr>
                <td>&nbsp;</td>
                <td align="left">
                    <div style="width: 100%; text-align: right">
                        <asp:Button ID="Button5" runat="server" CssClass="btnNew" Font-Bold="True"
                            Font-Size="X-Small" OnClick="EditRecord" Text="Update" Visible="false"
                            Width="100px" />
                    </div>
                    <br />
                </td>
            </tr>
        </tr>
    </table>
</div>
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>

    <asp:Button id="btnShowPopupF" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnF_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupF" PopupControlID="PnlPopupF"
        CancelControlID="btnCloseF" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="PnlPopupF" runat="server" Width="700px" ScrollBars="Auto" style="" BackColor="aqua">
        <div class="box">
             <asp:UpdatePanel ID="updatePanel2" runat="server" UpdateMode="Conditional">
                            <contenttemplate>  
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 680px">
                        <h3>New / Update Field</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseF" ImageUrl="images/close.png" CausesValidation="true" runat="server" OnClick="btnCloseF_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left;" valign="top">   
                                           
                                                 <table width="100%">
<tr>
<td align ="left" >
<asp:Label ID="lbMsgField" runat ="server" Font-Size ="X-Small" Font-Bold ="true" ForeColor ="Red" ></asp:Label>
</td>
</tr>
<tr>
     <td> 
   <div style="height:200px; overflow:scroll" >
       
     
 <asp:GridView ID="GrdAction" runat="server" AllowPaging="false" 
             AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" 
             DataKeyNames="Fieldid" >
               <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
             <Columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                 <asp:BoundField DataField="Displayname" HeaderText="Field Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="FieldType" HeaderText="FieldType">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="IsActive" HeaderText="Status">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="isrequired" HeaderText="Required">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="datatype" HeaderText="DataType">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="minlen" HeaderText="Min.Length">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="maxlen" HeaderText="Max.Length">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>
                         <asp:ImageButton ID="btnDown" runat="server" AlternateText="Move Down" 
                             Height="16px" ImageUrl="~/images/down.png" OnClick="PositionDownAction" 
                             ToolTip="Move Down" Width="16px" />
                         &nbsp;
                         <asp:ImageButton ID="btnUp" runat="server" AlternateText="Move Up" 
                             Height="16px" ImageUrl="~/images/up.png" OnClick="PositionUpAction" ToolTip="Move Up" 
                             Width="16px" />
                         &nbsp;
                         <asp:ImageButton ID="btnEditField" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHitField" ToolTip ="Edit fields" AlternateText="Edit" />
                         
                         <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/access.png" Height="16px" Width="16px" OnClick="CommonPropertiesofField" ToolTip ="Edit fields" AlternateText="Edit" />
                        
                           <asp:ImageButton ID="UPDField" runat="server" AlternateText="Add Extra properties" Height="16px" ImageUrl="~/images/save.png" OnClick="KickFieldHit" ToolTip="Set Kicking Field" Width="16px" /> &nbsp;
                            <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitUser" 
                             ToolTip="Delete" Width="16px" />
                         </ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" Width="140px" />
                 </asp:TemplateField>
             </Columns>
         </asp:GridView>
          
   </div>
     </td>
</tr>
<tr>
<td>
     
<%--     added by balmiki--%>

    


                 <asp:Panel ID="pnlGrid1" runat ="server">
                      <div id="tabPending00" runat="server" visible="false" >
       <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
             CellPadding="4" DataKeyNames="FieldID" 
               AllowSorting="True"  BackColor="White"   >
                     <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                    <Columns>
                   <asp:TemplateField >
                                 <ItemTemplate>
                                      <asp:CheckBox ID="Check" runat="server"   />
                                 </ItemTemplate>
                            <ItemStyle Width="20px" />
                   </asp:TemplateField>
                    
         
   <asp:BoundField DataField="Displayname" HeaderText="Field Name">
                       <HeaderStyle HorizontalAlign="Left" />
                       </asp:BoundField>
                       <asp:BoundField DataField="FieldType" HeaderText="FieldType">
                       <HeaderStyle HorizontalAlign="Left" />
                       </asp:BoundField>
                        <asp:BoundField DataField="IsActive" HeaderText="Status">
                       <HeaderStyle HorizontalAlign="Left" />
                       </asp:BoundField>
                       <asp:TemplateField HeaderText="Editable">
                                 <ItemTemplate>
                                      <asp:CheckBox ID="ChkEdit" runat="server"   />
                                 </ItemTemplate>
                            <ItemStyle Width="40px" />
                      </asp:TemplateField>
                    </Columns>
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                   <AlternatingRowStyle BackColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                </asp:GridView>
                           </div>
                </asp:Panel> 
         
    
                </td>
                </tr>
                <tr>
                <td align ="right" >
                <asp:Button runat ="server" ID="btnAdd" CssClass ="btnNew" Font-Size="X-Small" OnClick ="ADDField" Text ="ADD Fields" />
                </td>
                </tr>
                </table>       
                               
                    </td>
                </tr>
            </table>
                     </contenttemplate>
                        </asp:UpdatePanel>                     
        </div>
    </asp:Panel>


    <asp:Button id="btnShowPopupPreview" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnPreview_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupPreview" PopupControlID="pnlPopupPreview"
        CancelControlID="btnClosePreview" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupPreview" runat="server" Width="700px" Height="500px" ScrollBars="Auto" style="display: none" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 680px">
                        <h3>Form Preview</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnClosePreview" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left">

                        <asp:UpdatePanel ID="updatePanelPreview" runat="server" UpdateMode="Conditional">
                            <contenttemplate> 
   <asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
     <asp:Panel ID="pnlFields" Width="100%" runat="server">
            </asp:Panel>
   </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button id="btnScrnDEsg" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnScrn_ModalPopupExtender" runat="server"
        TargetControlID="btnScrnDEsg" PopupControlID="pnlPopupScrn"
        CancelControlID="btnCloseFormDesign" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupScrn" ScrollBars="Auto" runat="server" Width="800px" Height="450px" style="display: none" BackColor="White">
<div class="box">
    <asp:UpdatePanel ID="updatePanel3" runat="server" UpdateMode="Conditional">
        <contenttemplate> 
 <table cellspacing="2px" cellpadding="2px" width="100%" >
<tr>
<td style="width:780px"><h3>Screen Design</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseFormDesign" 
        ImageUrl="images/close.png" runat="server" ToolTip="Close"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="7" align="left" >
    <asp:Label ID="lblmes" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
    <tr>
           <td align="left" style="width:14%">
               <label>Table Width</label></td>
           <td align="left" style="width:14%" >
               <asp:TextBox ID="txtWidth" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
           </td>
          <td align="left" style="width:14%">
               <label>Fixed Row</label></td>
           <td align="left" style="width:14%">
               <asp:TextBox ID="txtFixRow" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
               </td>
                <td align="left" style="width:14%">
               <label>Fixed Column</label></td>
           <td align="left" style="width:14%">
               <asp:TextBox ID="txtFixCol" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
           </td>
         <td align="left"style="width:14%">
          <asp:Button ID="txtSubmit" runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Submit" Width="70px"  />
         </td>

           </tr>
        <tr>
           <td align="left" style="width:110px">
               <label>Edit Row No.</label></td>
           <td align="left">
               <asp:TextBox ID="txtEditRowNo" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
           </td>



           <td align="left" style="width:16px">
               <label>Logic</label></td>
           <td align="left">
               <asp:TextBox ID="txtEditRowLogic" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
           </td>
          <td align="left">
          <asp:Button ID="btnUpdate" runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Update" Width="70px" />
          </td>


       </tr>
      <tr>
           <td align="left" style="width:110px">
               <label>Insert at Row</label></td>
           <td align="left">
               <asp:TextBox ID="txtRowInsert" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>
           </td>

           <td align="left" style="width:16px">
               <label>Logic</label></td>
           <td align="left">
               <asp:TextBox ID="txtInsertLogic" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>

              </td>
              <td align="left">
          <asp:Button ID="btnInsertRow" runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Insert" Width="70px" />
           </td>


       </tr>
        <tr>
   

           <td align="left" style="width: 16px">
               <label>Delete Row</label></td>
           <td align="left">
               <asp:TextBox ID="txtDelRow" runat="server" CssClass="txtBox" Width="50%"></asp:TextBox>

            </td><td align="left">
          <asp:Button ID="btnDelRow" runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Delete" Width="70px" />
           </td>

           
       </tr>
       <tr>
       <td colspan="7" align="left" style="width:780px">
        <asp:Label ID="lblTablePreview" runat="server" Width ="100%" Text="Label"></asp:Label>
        </td>
       </tr>

       
       <tr>
       <td colspan="7" align="center">

        <asp:TextBox ID="txtTableSource" runat="server" TextMode="MultiLine" Height="200px" Width="100%"></asp:TextBox>
       
       </td>
       
       </tr>
       <tr>
       <td align="right" colspan ="6">
       <asp:Button ID="btnTxt"  runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Save" Width="70px"  />
       </td>
       <td align ="right" >
       <asp:Button ID="btnFinalSubmit"  runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Final Submit" Width="70px"  />
       </td>
       </tr>
          
  </table>
</div>
</td>
</tr>
</table>
</div>
          
   </ContentTemplate> 
   </asp:UpdatePanel> 
   </div>
   </asp:Panel>
    <asp:Button id="Button1" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender_ApplyFields" runat="server"
        TargetControlID="Button1" PopupControlID="pnlCustomLayout"
        CancelControlID="ImageButton2" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlCustomLayout" runat="server" Width="700px" Height="500px" ScrollBars="Auto" style="display: none" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 680px">
                        <h3>Apply Fields</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton2" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left">
                        <asp:UpdatePanel ID="updatePanel4" runat="server" UpdateMode="Conditional">
                            <contenttemplate> 
   <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
     <asp:Panel ID="pnlLayout" Width="100%" runat="server">
     </asp:Panel>
      <div style="width:100%;text-align:right">
  <asp:Button ID="btnApplyFields" runat ="server" CssClass="btnNew" OnClick ="SaveFields" Text="Apply Fields" />
</div>     
   </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button id="btnAutonumber" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPop_autoNumber" runat="server"
        TargetControlID="btnAutonumber" PopupControlID="pnlAutoNumber"
        CancelControlID="txtboxcancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlAutoNumber" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updAutonumber" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="AutonumberF" runat="server"></asp:Label> Apply Fields Auto Number</h3></td>
<td style="width:20px"><asp:ImageButton ID="autocancel" ImageUrl="images/close.png" runat="server" OnClick="autoNumberClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">

    <tr>
     <td align="left"> <asp:Label ID="lblautonumber" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtAutoNumbrDisplay" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     </tr><tr>
     
       <td align="left"> <label>Prefix Text box :</label>  <asp:TextBox ID="txtPrefix" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat ="server"    TargetControlID ="txtPrefix" WatermarkCssClass ="water" WatermarkText ="Prefix text "></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
           <td align="left"> <label>Series Number :</label> 
             <asp:TextBox ID="txtSeriesNumber" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
                   <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat ="server"    TargetControlID ="txtSeriesNumber" WatermarkCssClass ="water"    WatermarkText =" Enter Number Series Number"></asp:TextBoxWatermarkExtender> </td>   </tr>
<tr>
<td align="left"><label> Mandatory Field </label></td> <td align="left"> <asp:CheckBox ID="autoChkMan" runat="server" Text="Field is Mandatory" /> <asp:CheckBox ID="autoChkActive" runat="server" Text="Active" /> <%--<asp:CheckBox ID="CheckBox3" runat="server" Text="Editable" /> <asp:CheckBox ID="CheckBox4" runat="server" Text="Work Flow" />--%></td>
<td><asp:Button ID="BtnAutoSeriesSave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%--Add Tally Synch by mayank --%>
    <asp:Button id="btnnewAutonumber" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPop_newautoNumber" runat="server"
        TargetControlID="btnnewAutonumber" PopupControlID="pnlnewAutoNumber"
        CancelControlID="txtboxnewcancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlnewAutoNumber" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updnewAutonumber" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="NewAutonumberF" runat="server"></asp:Label> Apply Fields New Auto Number</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtboxnewcancel" ImageUrl="images/close.png" runat="server" OnClick="autonewNumberClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">

    <tr>
     <td align="left" > <asp:Label ID="lblnewautonumber" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td > </td></tr>
     <tr><td align="left" >   <label> * Display Name :</label>&nbsp;&nbsp;<asp:TextBox ID="txtdisplynewauto" runat="server" CssClass="txtBox"></asp:TextBox> </td>
               <%--<td align="left"> <label>Prefix Text box :</label>  <asp:TextBox ID="txtprefixnew" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender19" runat ="server"    TargetControlID ="txtprefixnew" WatermarkCssClass ="water" WatermarkText ="Prefix text "></asp:TextBoxWatermarkExtender></td>--%>
     <%--    <td align="left"><label>   Form :</label>&nbsp;&nbsp;<asp:DropDownList ID="ddlform" runat="server" AutoPostBack="true"></asp:DropDownList></td>--%>
     </tr>
        <tr>
        <%--   <td align="left"> <label>   &nbsp;&nbsp; Fields :</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:DropDownList ID="ddlfields" runat="server" AutoPostBack="true" Width="50%"></asp:DropDownList></td>--%>
         <td align="left"><label> &nbsp;Field For Prefix :</label>&nbsp;<asp:DropDownList ID="ddlfieldss" runat="server" AutoPostBack="true" Width="130px"></asp:DropDownList></td> 
            <td align="left"><label>Filed Name : </label>&nbsp;<asp:DropDownList ID="ddlAutoFieldName" runat="server" Width="130px" ></asp:DropDownList></td>
          <td align="left"> <label>Series Start From :</label> 
             <asp:TextBox ID="txtnewSeriesNumber" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
            <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender18" runat ="server"    TargetControlID ="txtnewSeriesNumber" WatermarkCssClass ="water"    WatermarkText =" Enter Number Series Number"></asp:TextBoxWatermarkExtender> </td>   
        </tr>
        
 <tr>
     <td align="left"><label> Mandatory Field </label></td> <td align="left"> <asp:CheckBox ID="chkmannewauto" runat="server" Text="Field is Mandatory" /> <asp:CheckBox ID="chkactnewauto" runat="server" Text="Active" /></td>
     <td><asp:Button ID="BtnnewAutoSave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <%--Addded By Mayank for Show Existing Values--%>
     <asp:Button id="btnShowExistingValues" runat="server" style="display: none" />
     <asp:ModalPopupExtender ID="MP_ShowWixistingValues" runat="server"
        TargetControlID="btnShowExistingValues" PopupControlID="pnl_ShowexistingVal"
        CancelControlID="cancel_ShowExistingValue" BackgroundCssClass="modalBackground"
        DropShadow="true">
        </asp:ModalPopupExtender>
      <asp:Panel ID="pnl_ShowexistingVal" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
              <asp:UpdatePanel ID="up_showexisting" runat="server" UpdateMode="Conditional">
                <contenttemplate> 
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                             <td style="width:880px"><h3><asp:Label ID="Label19" runat="server"></asp:Label> Apply Show Existing Configuration </h3></td>
                             <td style="width:20px"><asp:ImageButton ID="cancel_ShowExistingValue" ImageUrl="images/close.png" runat="server" OnClick="CloseShowExistingValues" /> </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                 <div class="form">
                                 <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                     <tr style="vertical-align:top;">
                                         <asp:Label ID="lblFormname" runat="server" Visible="false"></asp:Label>
                                        <td align="left" >   <label style="width:220px"> *Checked Condition & show Existing Values :</label> </td>
                                         <td><asp:CheckBoxList ID="chkexistingval" runat="server"></asp:CheckBoxList></td>
                                            <td align="left" >   <label style="width:220px"> *Checked Show Existing Values :</label> </td>
                                         <td><asp:CheckBoxList ID="chkexistingtravalEX" runat="server"></asp:CheckBoxList></td>
                                     </tr>
                                     <tr>
                                         <td colspan="4">
                                             &nbsp;
                                         </td>
                                     </tr>
                                     <tr>
                                         <td colspan="4">
                                             <asp:Button ID="btnSaveExistingval" runat="server" Text="SAVE" CssClass="btnNew" OnClick="btnSaveExistingval_Click" /> 
                                         </td>
                                     </tr>
                                     </table>
                                     </div>
                            </td>
                        </tr>
                        </table>
                    </contenttemplate>
                    </asp:UpdatePanel>
            </div>
          </asp:Panel>
    <%--Addded By Mayank for Show Existing Values--%>
    <%--Added By Mayank--%>
    
      <asp:Button id="btnMaster_Config" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="MP_MasterConfig" runat="server"
        TargetControlID="btnMaster_Config" PopupControlID="pnl_MasterConfig"
        CancelControlID="cancel_MasterConfig" BackgroundCssClass="modalBackground"
        DropShadow="true">
        </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_MasterConfig" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpMasterConfig" runat="server" UpdateMode="Conditional">
                <contenttemplate> 
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                             <td style="width:880px"><h3><asp:Label ID="Label15" runat="server"></asp:Label> Apply Master Tally Configuration </h3></td>
                             <td style="width:20px"><asp:ImageButton ID="cancel_MasterConfig" ImageUrl="images/close.png" runat="server" OnClick="CloseMasterConfig" /> </td>
                        </tr>
                        <tr>
                        <td colspan="2">
                            <div class="form">
                                 <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                         <td align="left" > <asp:Label ID="Label16" runat="server"  ForeColor="Red"></asp:Label></td>
                                         </tr> <tr><td > </td></tr>
                            <tr><td align="left" >   <label style="width:220px"> *Sync Eanbled Forms :</label><asp:DropDownList ID="ddlEnabledMaster" runat="server" Width="150px" CssClass="txtBox" AutoPostBack="true" OnSelectedIndexChanged="ddlEnabledMaster_SelectedIndexChanged"></asp:DropDownList> </td><td><label style="width:220px"> *Tally Registration Forms :</label><asp:DropDownList ID="ddltallyRegisteredForms" Width="150px" runat="server" CssClass="txtBox" AutoPostBack="true" OnSelectedIndexChanged="ddltallyRegisteredForms_SelectedIndexChanged"></asp:DropDownList></td></tr>
                                <tr><td align="left" > <label style="width:220px">*Sync Filter Operation :</label> <asp:DropDownList ID="ddlFillOperation" runat="server" Width="150px" AutoPostBack="true" CssClass="txtBox"  OnSelectedIndexChanged="ddlFilOperation_SelectedIndexChanged"><asp:ListItem Selected="True">SELECT</asp:ListItem><asp:ListItem Value="=">EQUAL</asp:ListItem><asp:ListItem Value="CONTAINS">CONTAINS</asp:ListItem></asp:DropDownList> </td> <td id="synfileter" runat="server" visible="false" ><label style="width:220px"> *Sync ALL Filter Fields :</label><asp:DropDownList ID="ddlFilterOperation" runat="server" Width="150px" AutoPostBack="true" CssClass="txtBox" OnSelectedIndexChanged="ddlFilterOperation_SelectedIndexChanged"></asp:DropDownList> </td>
                                                          </tr>
                                     <tr>
                                         <td align="left"><label style="width:220px"> *Sync Filter Fields :</label><asp:DropDownList ID="ddlSyncMaster" runat="server" Width="150px" AutoPostBack="true" CssClass="txtBox" OnSelectedIndexChanged="ddlSyncMaster_SelectedIndexChanged"></asp:DropDownList> </td>
                                        <td ><label style="width:220px"> *Tally Registration Filter Fields :</label><asp:DropDownList ID="ddlTallyRegisteredFileds" Width="150px" Enabled="false" runat="server" CssClass="txtBox"></asp:DropDownList></td>
                                        
                                     </tr>
                                     <tr>
                                        <td align="left" >
                                              <label style="width:220px"> *Is Active :</label><asp:DropDownList ID="ddlIsActive" runat="server" Width="150px" CssClass="txtBox">
                                                  <asp:ListItem>SELECT</asp:ListItem>
                                                  <asp:ListItem Value="1">Yes</asp:ListItem>
                                                  <asp:ListItem Value="0">No</asp:ListItem>
                                                                          </asp:DropDownList>
                                         </td>
                                         <td id="tagetdependentfld" runat="server" visible="false">
                                               <label style="width:220px"> *Target Dependent Fields :</label><asp:DropDownList ID="ddlTargetDependentFld" runat="server" Width="150px" CssClass="txtBox">
                                                                          </asp:DropDownList>
                                         </td>
                                     </tr>
                                     <tr>
                                         <td colspan="2">&nbsp;</td>
                                     </tr>
                                      <tr>
            <td align="left" colspan="2">
                <asp:Panel ID="Pnlmasterconfig" runat="server" Height="100px" ScrollBars="Auto"
                    Width="100%">
                    <asp:GridView ID="grdmasterconfig" runat="server" AllowPaging="false"
                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        DataKeyNames="tid" Width ="100%">
                        <footerstyle cssclass="FooterStyle" />
                        <rowstyle cssclass="RowStyle" />
                        <editrowstyle cssclass="EditRowStyle" />
                        <selectedrowstyle cssclass="SelectedRowStyle" />
                        <pagerstyle cssclass="PagerStyle" />
                        <headerstyle cssclass=" HeaderStyle" />
                        <alternatingrowstyle cssclass="AlternatingRowStyle" />
                        <columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                 <asp:BoundField DataField="sourceform" HeaderText="Source Form Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="sourcefields" HeaderText="Source Fields">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                             <asp:BoundField DataField="Operands" HeaderText="Operands">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>    
                  <asp:BoundField DataField="targetForm" HeaderText="Target Form Name">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                     <asp:BoundField DataField="TargetDepFields" HeaderText="Target Dep Field">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>       
                 <asp:BoundField DataField="targetfields" HeaderText="Target Fields">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>
                 <asp:BoundField DataField="IsActive" HeaderText="Status">
                 <HeaderStyle HorizontalAlign="Left" />
                 </asp:BoundField>    
                 <asp:TemplateField HeaderText="Action" HeaderStyle-Width="30px" ItemStyle-Width="30px">
                     <ItemTemplate>
                         
                        <asp:ImageButton ID="btnDeletemasterconfig" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="btnDeletemasterconfig_Click" 
                             ToolTip="Delete" OnClientClick=" return confirm('Are you sure you want to delete this configuration !!');" Width="16px" />
</ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" />
                 </asp:TemplateField>
             </columns>
                    </asp:GridView>
                </asp:Panel>
  </td>
            <tr>
                                <td>
                                    <asp:Button ID="btnMaster_Configuration" runat="Server" CssClass="btnNew" OnClick="btnMaster_Configuration_Click" Text="SAVE" Width="80px" />
                                </td>

                             </table>
                          </div>
                        </table>
                    </contenttemplate>
                </asp:UpdatePanel>
            </div>
        </asp:Panel>
    
    <%--added for auto configuration --%>
    <asp:Button ID="btnAutoConfig" runat="server" Style="display:none" />
    <asp:ModalPopupExtender ID="MP_AutoConfig" runat="server" TargetControlID="btnAutoConfig" PopupControlID="pnlAutoConfig"
        CancelControlID="cancel_AutoConfig" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

      <asp:Panel ID="pnlAutoConfig" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Up_pnlAutoConfig" runat="server" UpdateMode="Conditional">
                <contenttemplate> 
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                             <td style="width:880px"><h3><asp:Label ID="Label17" runat="server"></asp:Label> Apply For Auto Configuration </h3></td>
                             <td style="width:20px"><asp:ImageButton ID="cancel_AutoConfig" ImageUrl="images/close.png" runat="server" OnClick="CloseAutoConfig" /> </td>
                        </tr>
                        <tr>
                        <td colspan="2">
                            <div class="form">
                                 <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                     <tr>
                                       
                                    <td align="left" colspan="4" > <asp:Label ID="Label18" runat="server"  ForeColor="Red"></asp:Label></td>
                                        
                                     </tr>
                                     <tr>
                                       
                                             <td align="left" >   <label style="width:220px"> *Source Doc :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlSourceForm" runat="server"></asp:DropDownList></td>
                                         <td align="left"><label style="width:220px"> *Source Type :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlFormSourceType" runat="server"></asp:DropDownList> </td>
                                        
                                     </tr>
                                     <tr>
                                         <td align="left" >   <label style="width:220px"> *Target Doc :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlTargetDoc" runat="server"></asp:DropDownList></td>
                                         <td align="left"><label style="width:220px"> *Target Type :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlTargetType" runat="server"><asp:ListItem>DOCUMENT</asp:ListItem></asp:DropDownList> </td>
                                        
                                     </tr>
                                     <tr>
                                         <td align="left" >   <label style="width:220px"> *No of Rows :</label><asp:TextBox CssClass="txtBox" ID="txtNoRows" runat="server"></asp:TextBox></td>
                                         <td align="left"><label style="width:220px"> *Create Event :</label><asp:DropDownList Width="150" CssClass="txtBox" ID="ddlCreateEvent" runat="server"><asp:ListItem  Selected="True">SELECT</asp:ListItem><asp:ListItem>Schedule</asp:ListItem><asp:ListItem>Save</asp:ListItem><asp:ListItem>Approved</asp:ListItem></asp:DropDownList> </td>
                                     </tr>
                                      <tr>
                                         <td align="left" >   <label style="width:220px"> *Work Flow Status :</label><asp:DropDownList CssClass="txtBox" ID="ddlWFStatus"  Width="150" runat="server"></asp:DropDownList></td>
                                         <td align="left"><label style="width:220px"> *Creator :</label><asp:DropDownList Width="150" CssClass="txtBox" AutoPostBack="true" ID="ddlCreator" runat="server" OnSelectedIndexChanged="ddlCreator_SelectedIndexChanged"><asp:ListItem  Selected="True">SELECT</asp:ListItem><asp:ListItem>OUID</asp:ListItem><asp:ListItem>Role</asp:ListItem><asp:ListItem>Filed Value</asp:ListItem></asp:DropDownList> </td>
                                     </tr>
                                     <tr>
                                         <td align="left" >   <label style="width:220px"> *Creator Value :</label><asp:DropDownList CssClass="txtBox" ID="ddlCreatorValue"  Width="150" runat="server"></asp:DropDownList></td>
                                     </tr>
                                     </table>
                                </div>
                            </td>
                            </tr>
                        </table>
                    </contenttemplate>
                </asp:UpdatePanel>
            </div>
          </asp:Panel>
     <asp:Button id="btnDetailRule" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="MP_DetailRule" runat="server"
        TargetControlID="btnDetailRule" PopupControlID="panel_DetailRule"
        CancelControlID="cancel_detailRule" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="panel_DetailRule" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="upd_detailRule" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="lblDetailRuleHead" runat="server"></asp:Label> Apply Rules </h3></td>
<td style="width:20px"><asp:ImageButton ID="cancel_detailRule" ImageUrl="images/close.png" runat="server" OnClick="CloseCRule" /> </td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left" > <asp:Label ID="lblDetailMess" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td > </td></tr>
     <tr><td align="left" >   <label> * Detail Form Fields :</label><asp:TextBox ID="txtDetailRuleFields" runat="server" CssClass="txtBox" TextMode="MultiLine"></asp:TextBox> </td>
     <td align="left"><label> Static Rule :</label>&nbsp;<asp:TextBox ID="txtDetailRuleCalC" runat="server" CssClass="txtBox" TextMode="MultiLine"></asp:TextBox> </td>
     </tr>
    <tr> <td><asp:Button ID="btnDetailRuleSave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="DetailRuleSave"/></td> </tr>

</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%--Add LT Lookup--%>
     <asp:Button id="ddlLTLOOKUPSelect" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModlPopupLTLOOKUP" runat="server"
        TargetControlID="ddlLTLOOKUPSelect" PopupControlID="pnlLTLOOKUPSelect"
        CancelControlID="LTLOOKUPcancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlLTLOOKUPSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updLTLOOKUP" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="Label20" runat="server"></asp:Label> Apply Fields LAST TRANSACTION LOOKUP</h3></td>
<td style="width:20px"><asp:ImageButton ID="LTLOOKUPcancel" ImageUrl="images/close.png" runat="server"  OnClick="LTLOOKUPClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
    <tr>
     <td align="left"> <asp:Label ID="Label21" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtLTLOOKUPDN" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtLTLOOKUPDS" runat="server" CssClass="txtBox"></asp:TextBox><label> Source Fields :</label><asp:DropDownList ID="ddlLLSF" Width="150px" OnSelectedIndexChanged="ddlLLSF_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="txtBox"></asp:DropDownList> </td> 
     </tr>
        <tr>
            <td align="left"><label> * Target Doc   :</label><asp:DropDownList ID="ddlLLTS" Width="150px" runat="server" CssClass="txtBox" OnSelectedIndexChanged="ddlLLTS_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList></td>
            <td align="left"><table><tr> <td><label> Selection Criteria :</label><asp:DropDownList ID="ddlLLSC" runat="server" Enabled="false"><asp:ListItem Value="M">MAIN</asp:ListItem><asp:ListItem Value="C">CHILD</asp:ListItem></asp:DropDownList><label>TO</label><asp:DropDownList ID="ddlLLTFS" runat="server" OnSelectedIndexChanged="ddlLLTFS_SelectedIndexChanged" AutoPostBack="true"><asp:ListItem Value="M">MAIN</asp:ListItem><asp:ListItem Value="C">CHILD</asp:ListItem></asp:DropDownList></td><td><table id="OtherSource" runat="server" cellpadding="0" cellspacing="0" style="vertical-align:top;" visible="false"><tr style="vertical-align:top;"><td><label style="text-align:left;">Target Child Doc :</label><asp:DropDownList ID="ddlLLCTD" runat="server" CssClass="txtBox" Width="150px" OnSelectedIndexChanged="ddlLLCTD_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList></td></tr></table><table id="TDF" runat="server" visible="false" cellpadding="0" cellspacing="0" style="vertical-align:top;"><tr><td><label style="text-align:left;">Target Fields :</label><asp:DropDownList ID="ddlLLTFLDS" OnSelectedIndexChanged="ddlLLTFLDS_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="txtBox" Width="150px"></asp:DropDownList> </td></tr></table></td></tr> </table> </td>
       </tr>
        <tr>            
            <td align="left" id="TCF" runat="server" visible="false"><label> * Target Child Fields :</label><asp:DropDownList ID="ddlLLTCRF" runat="server" CssClass="txtBox" Width="150px" OnSelectedIndexChanged="ddlLLTCRF_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList></td>
            <td align="left"><label> * Mapping Fields :</label><asp:DropDownList ID="ddlLLMF" runat="server" CssClass="txtBox" Width="150px"></asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="ChkLLM" runat="server" Text="Mandatory" /> <asp:CheckBox ID="ChkLLA" runat="server" Text="Active" /> <asp:CheckBox ID="ChkLLE" runat="server" Text="Editable" /> <asp:CheckBox ID="ChkLLW" runat="server" Text="Work Flow" /> <asp:CheckBox ID="ChkLLU" runat="server" Text="Unique" /> <asp:CheckBox ID="ChkLLD" runat="server" Text="Description" /> </td>
             <td><asp:Button ID="btnSaveLL" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
                    </tr>
                </table>
            </td>
            
            
        </tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <%--Added Seperator--%>
    <asp:Button id="ddlSeperatorBoxSelect" runat="server" style="display: none" />
      <asp:ModalPopupExtender ID="ModlPopupSeperator" runat="server"
        TargetControlID="ddlSeperatorBoxSelect" PopupControlID="pnlSeperatorBoxSelect"
        CancelControlID="Seperatorboxcancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
     <asp:Panel ID="pnlSeperatorBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updSeperator" runat="server" UpdateMode="Conditional">
                <contenttemplate> 
                <table cellspacing="2px" cellpadding="2px" width="100%">
                    <tr>
                        <td style="width:880px"><h3><asp:Label ID="Label22" runat="server"></asp:Label> Apply Fields Separator</h3></td>
                        <td style="width:20px"><asp:ImageButton ID="Seperatorboxcancel" ImageUrl="images/close.png" runat="server" OnClick="txtSepClose"/></td>
                    </tr>
                    <tr>
                    <td colspan="2">
                    <div class="form">
                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                    <tr>
                        <td align="left"> <asp:Label ID="lblSeparator" runat="server"  ForeColor="Red"></asp:Label></td>
                    </tr>
                    <tr>
                        <td> 

                        </td>

                    </tr>
                    <tr>
                    <td align="left">
                    <label> * Display Name :</label><asp:TextBox ID="txtSepDP" runat="server" CssClass="txtBox"></asp:TextBox> </td>
                     <td align="left">
                      <label> Border Type :</label><asp:DropDownList ID="ddlBorderType" runat="server" CssClass="txtBox"><asp:ListItem>Select</asp:ListItem><asp:ListItem Value="border">Border All</asp:ListItem> <asp:ListItem Value="border-top">Border Top</asp:ListItem><asp:ListItem Value="border-bottom">Border Bottom</asp:ListItem></asp:DropDownList><label> Value Type :</label><asp:DropDownList ID="ddlSEPVT" runat="server" CssClass="txtBox"><asp:ListItem Value="dotted">Dotted</asp:ListItem><asp:ListItem Value="solid">Solid</asp:ListItem></asp:DropDownList> </td> 
                    </tr>
                        <tr>
                            <td colspan="2" align="left">
                                <label> * Select Color  :</label> <asp:ColorPickerExtender ID="colorpicker" runat="server" Enabled="true" TargetControlID="txtColorPicker"></asp:ColorPickerExtender>
                            <asp:TextBox ID="txtColorPicker" runat="server"></asp:TextBox>
                            </td>
                           
                        </tr>
                     <tr>
                       
                        <td colspan="2"><asp:Button ID="btnSeparator" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
                        </tr>
                 </table>
</div>
</td>
                </table>
                </contenttemplate>
                </asp:UpdatePanel>
            </div>
         </asp:Panel>

    <%--add mulit tookup ddl--%>
    <asp:Button id="ddlTxtBoxSelect" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModlPopupTxtBox" runat="server"
        TargetControlID="ddlTxtBoxSelect" PopupControlID="pnlTxtBoxSelect"
        CancelControlID="txtboxcancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlTxtBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updtxtBox" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="Textboxf" runat="server"></asp:Label> Apply Fields TEXTBOX</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtboxcancel" ImageUrl="images/close.png" runat="server" OnClick="txtBoxClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lbltxtbox" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtDisplayName" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtDescription" runat="server" CssClass="txtBox"></asp:TextBox><label> Default Value :</label><asp:TextBox ID="txtDefVal" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr><tr>
     
     <td  align="left" ><label>* Data Types :  </label> &nbsp;<asp:DropDownList ID="ddltextBoxItem" runat="server" AutoPostBack="true">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     <asp:ListItem>New Datetime</asp:ListItem>
     <asp:ListItem>Time</asp:ListItem>
        <asp:ListItem>FY Start</asp:ListItem>
         <asp:ListItem>FY End</asp:ListItem>
         <asp:ListItem>Scheduler</asp:ListItem>
     </asp:DropDownList>  </td>

            <td align="left" ><label id="lblcasetxt" runat="server" visible="false">*Case of Text:  </label> &nbsp;
     <asp:DropDownList ID="ddltextcase" runat="server" AutoPostBack="true" Visible="false">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Uper Case</asp:ListItem>
     <asp:ListItem>Lower Case</asp:ListItem>
     <asp:ListItem>Proper Case</asp:ListItem>
         </asp:DropDownList> </td>     
        </tr>
        <tr><td align="left" colspan="4"> <label>Field Length :</label>  <asp:TextBox ID="txtMinlength" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" runat ="server"    TargetControlID ="txtMinlength" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
             <asp:TextBox ID="txtMaxL" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
             <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender4" runat ="server"    TargetControlID ="txtMaxL" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>  </tr>
<tr>
<td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="chkFldMandotry" runat="server" Text="Mandatory" /> <asp:CheckBox ID="chkActv" runat="server" Text="Active" /> <asp:CheckBox ID="chkEditable" runat="server" Text="Editable" /> <asp:CheckBox ID="chkTextBoxWF" runat="server" Text="Work Flow" /> <asp:CheckBox ID="Chkunique" runat="server" Text="Unique" /> <asp:CheckBox ID="ChkDesc" runat="server" Text="Description" /> </td>
<td><asp:Button ID="btnTxtBoxSave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%# CType(Container, GridViewRow).RowIndex + 1%>

      <asp:Button id="btnVarianceSelect" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mod_Vairance" runat="server"
        TargetControlID="btnVarianceSelect" PopupControlID="pnlVarianceSelect"
        CancelControlID="VaianceCancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlVarianceSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpdateVariance" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="lblVarianceHeader" runat="server"></asp:Label> Apply Fields Variance</h3></td>
<td style="width:20px"><asp:ImageButton ID="VaianceCancel" ImageUrl="images/close.png" runat="server" OnClick="VarianceClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblMsgVariance" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtVariance_DisplayName" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtVariance_Description" runat="server" CssClass="txtBox"></asp:TextBox>&nbsp;<label> Select Fields :</label><asp:DropDownList ID="ddlSelectFileds" runat="server" CssClass="txtBox" Width="150px"></asp:DropDownList></td>
     <tr>
         <td align="left">
             <label>
             Field Properties
             </label>
         </td>
         <td align="left">
             <asp:CheckBox ID="ChkVarMan" runat="server" Text="Mandatory" />
             <asp:CheckBox ID="ChkVarAct" runat="server" Text="Active" />
             <asp:CheckBox ID="ChkVarEdit" runat="server" Text="Editable" />
             <asp:CheckBox ID="ChkVarWf" runat="server" Text="Work Flow" />
             <asp:CheckBox ID="ChkVarUQ" runat="server" Text="Unique" />
         </td>
         <td>
             <asp:Button ID="btnVarianceSave" runat="Server" CssClass="btnNew" OnClick="EditRecord" Text="SAVE" Width="80px" />
         </td>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%# CType(Container, GridViewRow).RowIndex + 1%>
    <asp:Button ID="buttonAF" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupadvformula" runat="server"
        TargetControlID="buttonAF" PopupControlID="paneladvanceformula"
        CancelControlID="imgadfor" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="paneladvanceformula" runat="server" Width="1000px" Height="450px" Style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updpaneladvanceformula" runat="server" UpdateMode="Conditional">
                <contenttemplate>

                    <table cellspacing="2px" cellpadding="2px" width="100%">

                        <tr>
                            <td style="width: 880px">
                                
                                 <script type="text/javascript">

                                     function storeCaret(textEl) { textEl.caretPos = document.selection.createRange().duplicate(); }
                                     function insertAtCaret(textEl, text) { if (textEl.createTextRange && textEl.caretPos) { textEl.caretPos.text = text; } else { alert('no carat'); } }

                                     function insertCode(t) {
                                         var subject = document.getElementById('<%= txtcontionadvform.ClientID%>');
                                         if (document.selection) {
                                             insertAtCaret(document.getElementById('<%= txtcontionadvform.ClientID%>'), '{' + t + '}');
                                         }
                                         else if (subject.selectionStart || subject.selectionStart == '0') {
                                             var str = subject.value;
                                             var a = subject.selectionStart, b = subject.selectionEnd;
                                             subject.value = str.substring(0, a) + arguments[0] + (arguments[1] ? str.substring(a, b) + arguments[1] : "") + str.substring(b, str.length);
                                             return;
                                         }
                                     };

                                     function setCaret(t) {
                                         // var inp = document.getElementById('selectList');
                                         insertCode('{' + t + '}');
                                     }



                                     function setCaretcccccccccc(t) {
                                         // var textarea = document.getElementById('<%= txtcontionadvform.ClientID%>');
                                         var e = document.getElementById('<%= tvadvform.ClientID%>');
                                         var text = e.options[e.selectedIndex].value;
                                         var textarea = document.getElementById('txtcontionadvform'),
                                             tempStr1 = textarea.value.substring(0, globalCursorPos),
                                             tempStr2 = textarea.value.substring(globalCursorPos);
                                         textarea.value = tempStr1 + t + tempStr2;
                                         document.getElementById("Line").innerHTML = '<strong>Start</strong> ' + tempStr1;
                                         document.getElementById("Column").innerHTML = '<strong>End</strong> ' + tempStr2;
                                     }
                                     function updatePosition(t) {
                                         globalCursorPos = getCursorPos(t);
                                     }
                                     var globalCursorPos;
                                     function getCursorPos(textElement) {
                                         //save off the current value to restore it later,
                                         var sOldText = textElement.value;


                                         //create a range object and save off it's text
                                         var objRange = document.selection.createRange();
                                         var sOldRange = objRange.text;


                                         //set this string to a small string that will not normally be encountered
                                         var sWeirdString = '#%~';


                                         //insert the weirdstring where the cursor is at
                                         objRange.text = sOldRange + sWeirdString; objRange.moveStart('character', (0 - sOldRange.length - sWeirdString.length));


                                         //save off the new string with the weirdstring in it
                                         var sNewText = textElement.value;


                                         //set the actual text value back to how it was
                                         objRange.text = sOldRange;


                                         //look through the new string we saved off and find the location of
                                         //the weirdstring that was inserted and return that value
                                         for (i = 0; i <= sNewText.length; i++) {
                                             var sTemp = sNewText.substring(i, i + sWeirdString.length);
                                             if (sTemp == sWeirdString) {
                                                 var cursorPos = (i - sOldRange.length);
                                                 return cursorPos;
                                             }
                                         }
                                     }

                                 </script>
                                <h3>
                                    <asp:Label ID="Label13" runat="server"></asp:Label>
                                    Apply Advance Formula</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="imgadfor" ImageUrl="images/close.png" runat="server" OnClick="advanceformulaclose" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form" style="overflow:scroll; width:990px;">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0" >
                                        <tr>
                                            <td align="left" colspan="3">
                                                <asp:Label ID="lblmsgadvanceformula" runat="server" ForeColor="Red"></asp:Label></td>
                                        </tr>
                                       
                                        <tr>
                                            <td  style="width:150px;" >
                                                <label>* Display Name :</label><br/><asp:TextBox ID="txtdisplaynameadvform" runat="server" CssClass="txtBox"></asp:TextBox>
                                                
                                            </td>
                                            <td  style="width:150px;" >
                                                  <label>Description :</label><br /><asp:TextBox ID="txtdescadvform" runat="server" CssClass="txtBox"></asp:TextBox>
                                                                                                    
                                                </td>
                                            <td style="width:150px;"> <label>Is Active :</label><br /><asp:CheckBox ID="chkadvformactive" runat="server"  /></td>
                                           <td  style="width:150px;" >
                                               <label>Form Source :</label><br />
                                                <asp:DropDownList ID="ddladvfoftype" CssClass="txtbox" Height="25px" runat="server" Width="150px" AutoPostBack="true">
                                                    <asp:ListItem Value="0">SELECT</asp:ListItem>
                                                    <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                    <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                    <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                                    <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                                </asp:DropDownList>
                                                
                                           </td>
                                            <td style="width:150px;">
                                                 <label>Document Type :</label><br />
                                                <asp:DropDownList ID="ddladvfodoctype" AutoPostBack="true" Height="25px" CssClass="txtbox" runat="server" Width="150px">
                                                </asp:DropDownList>
                                            </td>

                                           
                                        </tr>
                                        <tr>
                                            
                                            <td colspan="4">

                                                <asp:Button ID="btnpop" runat="server" Text="Add More Document" Visible="True" CssClass="btnNew" OnClick="AddRelation" />

                                            </td>

                                        </tr>
                                        <tr>

                                            <td style="width: 350px; height: 200px;" colspan="2">
                                              
                                                    <div style="overflow: scroll; float:left; margin:10px; width:480px;height:200px;"  >
                                                        <asp:TreeView ID="tvadvform" EnableClientScript="true" runat="server" Height="100%" ImageSet="Inbox"
                                                            Width="100%">
                                                            <ParentNodeStyle Font-Bold="False" />
                                                            <HoverNodeStyle Font-Underline="True" />
                                                            <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                                VerticalPadding="0px" />
                                                            <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                        </asp:TreeView>
                                                         <asp:TreeView ID="tvsource" EnableClientScript="true" runat="server" Height="100%" ImageSet="Inbox"
                                                             Width="100%">
                                                             <ParentNodeStyle Font-Bold="False" />
                                                             <HoverNodeStyle Font-Underline="True" />
                                                             <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                                 VerticalPadding="0px" />
                                                             <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                                 HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                            </asp:TreeView>
                                                            <asp:Panel ID="pnllsd" runat="server">
                                                            </asp:Panel>
                                                    </div>
                                                

                                                   
                                            
                                            </td>
                                           <td style="width: 300px; height: 200px;" colspan="3" >
                                                <div style="float:left; margin:10px; margin-left:0px; width:450px;height:200px;"  >
                                                        <asp:TextBox Height="200px" ID="txtcontionadvform" onclick="storeCaret(this);"
                                                            onselect="storeCaret(this);" onkeyup="storeCaret(this);" runat="server" Style="border: 1px dashed  #54c618;" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                                    </div>

                                           </td>

                                        </tr>
                                        <tr>
                                             <td style="width:600px;" colspan="4" >
                                                 <style type="text/css" >
                                                     .myPanelClass {
                                                         max-height: 400px;
                                                         width: 100%;
                                                         overflow: auto;
                                                     }
                                                 </style>
                                                                        <asp:UpdatePanel runat="server" ID="updgrid" UpdateMode="Always" >
                                                                                                                                                        <ContentTemplate>
                                                                        <asp:Panel runat="server" ID="pblm"  CssClass="myPanelClass"   >
                                                                         
                                                                            <asp:GridView ID="gvmap" OnRowDataBound = "gvmapOnRowDataBound" OnRowDeleting="gvmapOnRowDeleting" Width="98%"  runat="server" AutoGenerateColumns="true">
                                                                                <Columns>
                                                                                    <%--<asp:TemplateField HeaderText="Action">
                                                                                        <ItemTemplate>
                                                                                            <asp:ImageButton ID="btnDelete"  ImageUrl="~/images/closered.png" CommandName="Delete"  runat="server" Height="16px" Width="16px" ToolTip="Delete Role" AlternateText="Delete" />
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                                                                                    </asp:TemplateField>--%>

                                                                                    
                                                                                    <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                             
                                                                        </asp:Panel>
                                                                                </ContentTemplate>
                                                                            </asp:UpdatePanel>
                                                                    </td>
                                        </tr>
                                        <tr>

                                            <td>
                                                <asp:Button ID="btnsaveadvformula" runat="Server" CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord" /></td>
                                        </tr>
                                    </table>
                               </div>
                            </td>
                        </tr>
                    </table>
                </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:Button ID="btnformularel" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupformularelation" runat="server"
        TargetControlID="btnformularel" PopupControlID="panelformularelation"
        CancelControlID="btncloseformrel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="panelformularelation" runat="server" Width="600px" BackColor="aqua">
        <div class="box" style="height: 200px; overflow: scroll;">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>New / Update Role</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btncloseformrel"
                            ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="UpdatePanelFormulaRelation" runat="server" UpdateMode="Conditional">
                            <contenttemplate>

                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <asp:Label ID="lblmsgFrelation" runat="server" Font-Bold="True" ForeColor="Red"
                                                        Width="100%" Font-Size="X-Small"></asp:Label>

                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="width: 125px" align="left">Source Document<b>  </b></td>
                                                <td align="left">
                                                    <asp:Label ID="lblsd" runat="server"></asp:Label></td>

                                                <td style="width: 150px" align="left">Source Doc Fields</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlsdf" runat="server" Width="99%"></asp:DropDownList>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td style="width: 150px" align="left">
                                                    <asp:Label ID="lblsformsource" Text="Target Doc Type" runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList ID="ddlsourcetype" AutoPostBack="true" Width="99%" runat="server">
                                                        <asp:ListItem Value="0">SELECT</asp:ListItem>
                                                        <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                        <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                        <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                                        <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                                    </asp:DropDownList></td>
                                                <td>Target Document Name</td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList Width="99%" AutoPostBack="true" ID="ddlsourcedoc" runat="server">
                                                        <asp:ListItem></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 150px" align="left">
                                                    <asp:Label ID="lbldss" runat="server" Text="Target Fields"></asp:Label>
                                                </td>
                                                <td style="width: 150px" align="left">
                                                    <asp:DropDownList ID="ddltf" Width="99%" runat="server"></asp:DropDownList>
                                                </td>
                                                <td style="width: 150px" align="left"> <asp:Label ID="lblsortsource" runat="server" Text="Sorting Fields"></asp:Label></td>
                                                <td style="width: 150px" align="left">
                                                     <asp:DropDownList Width="99%"  ID="ddlsortingfields" runat="server">
                                                        
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                           


                                        </table>
                                        <div style="width: 100%; text-align: right">
                                            <asp:Button ID="btnsavefrelation" runat="server" Text="Update"
                                                OnClick="EditRecordFormulaRelation" CssClass="btnNew" Font-Bold="True"
                                                Font-Size="X-Small" Width="100px" />
                                        </div>
                                    </contenttemplate>
                        </asp:UpdatePanel>

                    </td>
               </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnfreldel" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupfreldel" runat="server"
        TargetControlID="btnfreldel" PopupControlID="panelfreldel"
        CancelControlID="btncloseFreldel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="panelfreldel" runat="server" Width="500px" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 480px">
                        <h3>Delete Rule</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btncloseFreldel"
                            ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatepanelFRelDel" runat="server" UpdateMode="Conditional">
                            <contenttemplate>
                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <asp:Label ID="Label14" runat="server" Text="Are you sure want to delete" Font-Bold="True" ForeColor="Red"
                                                        Width="100%" Font-Size="X-Small"></asp:Label>

                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: right">
                                            <asp:Button ID="btnFrelDell" runat="server"  Text="Yes Delete"
                                                CssClass="btnNew" Font-Bold="True"
                                                Font-Size="X-Small" Width="100px" />
                                        </div>
                                    </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    
    <%--     added by balmiki--%>
    <asp:Button id="ddlTxtBoxArea" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopUpTxtArea" runat="server"
        TargetControlID="ddlTxtBoxArea" PopupControlID="pnlTxtBoxArea"
        CancelControlID="txtAreacancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlTxtBoxArea" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updtxtArea" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="textareaf" runat="server"></asp:Label> Apply Fields TEXT AREA</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtAreacancel" ImageUrl="images/close.png" runat="server" OnClick="txtBoxAreaClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lbltxtArea" runat="server"   ForeColor="Red"></asp:Label></td>
     </tr><tr><td> </td></tr>
     <tr><td align="left"><label>Display Name</label><asp:TextBox ID="txtDispalyNameArea" runat="server" CssClass="txtBox"></asp:TextBox> </td><td><label> Default Value :</label><asp:TextBox ID="txtAreaDefVal" runat="server" CssClass="txtBox"></asp:TextBox></td></tr>
     <tr>
     <td align="left"><label> Data Types </label> &nbsp; <asp:DropDownList ID="ddlDataTypesArea" runat="server">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList> </td>
    <td align="left" > <label>Field Length </label>  <asp:TextBox ID="txtareaMinLength" runat="server" CssClass="txtBox" Width="146px" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender5" runat ="server"    TargetControlID ="txtareaMinLength" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           
             <asp:TextBox ID="txtareaMaxLength" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
                   <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender6" runat ="server"    TargetControlID ="txtareaMaxLength" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>   </tr>

<tr>     
 <td align="left"><label> Mandatory Field </label>
</td><td align="left"><asp:CheckBox ID="chktxtAreaMandatory" runat="server" Text="Field is Mandatory" /><asp:CheckBox ID="chktxtAreaActive" runat="server" Text="Active" /><asp:CheckBox ID="chktxtAreaEditable" runat="server" Text="Editable" /> <asp:CheckBox ID="chkTxtAreaWF" runat="server" Text="Work Flow" />  </td>
<td> <asp:Button ID="btnTxtArea" runat="Server"  CssClass="btnNew"  Width="60px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>

</table>


</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    
    <asp:Button id="btnDropDown" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopUpDropDown" runat="server" TargetControlID="btnDropDown" PopupControlID="pnlDropDown"
        BackgroundCssClass="modalBackground" CancelControlID="txtdropdowncancel" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlDropDown" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updDropdownSelected" runat="server" UpdateMode="Conditional">
                <contenttemplate> 
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3> <asp:Label ID="dd" runat ="server" > </asp:Label> Apply Fields DropDown </h3></td>
<td style="width:20px"><asp:ImageButton ID="txtdropdowncancel" ImageUrl="images/close.png" OnClick ="Close" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table width="100%" border="0px">
        <tr>
     <td align="left" colspan="3"> <asp:Label ID="lbldropdown" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr><tr><td> </td></tr>

     <tr><td align="right"> <label>* Display Name  : </label></td> <td align="left" colspan="3"><asp:TextBox  ID="ddlDispalyName" runat="server"  CssClass="txtBox"></asp:TextBox> 
     &nbsp;&nbsp;<label>* Field Type    :  </label><asp:DropDownList ID="ddlDropdownList" runat="server" Width="150px" AutoPostBack="true">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>FIX VALUED</asp:ListItem>
     <asp:ListItem>MASTER VALUED</asp:ListItem>
     <asp:ListItem>SESSION VALUED</asp:ListItem>
     <asp:ListItem>CHILD VALUED</asp:ListItem>
     </asp:DropDownList> <asp:CheckBox ID="chkAutoCom" runat="server" Text="Auto Complete" Visible="false" />
      </td></tr><tr>
    <td align="right"> <asp:label ID="lblDdlvalue" runat="server" ><b>  Value : </b></asp:label></td> <td  align="left" colspan="4"> <asp:TextBox ID="txtdropDownValued" runat="server" CssClass="txtBox"  Width="400px">
    </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender7" runat ="server"  TargetControlID ="txtdropDownValued" WatermarkCssClass ="water"    WatermarkText ="Enter comma saperated Value"></asp:TextBoxWatermarkExtender>
    </td></tr> 
        <tr>
    <td align="right"> <asp:label ID="lbldftvalfix" runat="server" Visible ="false" ><b>  Default Value : </b></asp:label></td>
   <td  align="left" colspan="4"> <asp:TextBox ID="txtdftvalfix" runat="server" CssClass="txtBox"  Width="400px" Visible ="false">
    </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender21" runat ="server"  TargetControlID ="txtdftvalfix" WatermarkCssClass ="water"    WatermarkText ="Enter Only One Value From List"></asp:TextBoxWatermarkExtender>
    </td></tr>           
    <tr><td align="right"><asp:label ID="lblDdlSelectMaster" runat="server" ><b> Select Master : </b></asp:label></td><td align="left" colspan="3">
    <asp:DropDownList ID="ddlDropDownMasteValSelect" runat="server"  Width="150px" AutoPostBack="true" >
    </asp:DropDownList>
    <asp:label ID="lblSelectField" runat="server" Text="* Select Field : " ></asp:label>
    <asp:DropDownList ID="ddlDropDownFieldSelect"  runat="server"  Width="150px" AutoPostBack="True">
     </asp:DropDownList></td> <td align ="right" style ="font-weight:bold" ></td> </tr>
   <tr><td align="right">
       <asp:label ID="lbldfaultvalfld" runat="server" Visible ="false"><b> Select Default Value : </b></asp:label></td>
    <td align="left" colspan="3"> <asp:DropDownList ID="ddldfultvalfld"  runat="server"  Width="150px" AutoPostBack="True" Visible ="false">
     </asp:DropDownList>
       
       <asp:label ID="lblfilteron" runat="server" Visible ="false" ><b>&nbsp;&nbsp;Fiter On: : </b></asp:label>
           &nbsp;&nbsp;    <asp:DropDownList ID="ddlFLT" runat ="server" Visible ="false"  Width ="150px" ></asp:DropDownList>

         </td> <td align ="right" style ="font-weight:bold" ></td> 
   </tr>
      
      <tr><td align="right"><asp:label ID="lblFldsource" runat="server" Text=" Init. Filter Source:" ></asp:label></td><td align="left" colspan="3" >
      <asp:DropDownList ID="ddlFltrSource"  runat="server"  Width="150px"></asp:DropDownList>
      <asp:label ID="lblFltrDestination" runat="server" Text="Init Filter From:"></asp:label><asp:DropDownList ID="ddlFltrMapping" runat="server"  Width="150px"></asp:DropDownList></td>

      </tr>
        <tr>
            <td align="right"><asp:label ID="Label23" runat="server" Text=" Session Field Value:" ></asp:label></td>
            <td align="left" colspan="3">
                <asp:DropDownList ID="ddlSessionFieldVal" OnSelectedIndexChanged="ddlSessionFieldVal_SelectedIndexChanged" AutoPostBack="true" runat="server" Width="150px"></asp:DropDownList>
                <asp:Label ID="lbl" runat="server" Text="Dependent Master"></asp:Label><asp:DropDownList ID="ddldependent" runat="server"></asp:DropDownList>
            </td>
          
        </tr>
<tr>     
 <td align="right"><label> Mandatory Field :  </label>
</td><td align="left" colspan="4"><asp:CheckBox ID="chkDdlMan" runat="server" Text="Field is Mandatory" /><asp:CheckBox ID="chkDlActive" runat="server" Text="Active" /><asp:CheckBox ID="chkDdlEditable" runat="server" Text="Editable" /><asp:CheckBox ID="chkDdlWF" runat="server" Text="Work Flow" /> </td>
<td> <asp:Button ID="btnddlSelect" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
                <triggers>
<asp:AsyncPostBackTrigger ControlID="txtdropdowncancel" />
</triggers>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    
    <asp:Button id="btnListBoxSelected" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupListBox" runat="server"
        TargetControlID="btnListBoxSelected" PopupControlID="pnlLstBox"
        CancelControlID="ListBoxcancel" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlLstBox" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">


            <asp:UpdatePanel ID="updLstBox" runat="server" UpdateMode="Conditional">
                <contenttemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="ListBoxF" runat="server"></asp:Label> Apply Fields ListBox</h3></td>
<td style="width:20px"><asp:ImageButton ID="ListBoxcancel" ImageUrl="images/close.png" runat="server" OnClick="btnlistCalncel"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="2px" cellpadding="2px" width="100%">
    <tr>
    <td align="left"> <asp:Label ID="lblListBox" runat="server"   ForeColor="Red"></asp:Label></td>
     </tr><tr><td> </td></tr>

     <tr><td align="right"> <label>* Display Name  : </label></td><td align="left" colspan="3"> <asp:TextBox  ID="txtListBoxDisplayName" runat="server" CssClass="txtBox" ></asp:TextBox>
     &nbsp;&nbsp;<label>* Field Type :  </label><asp:DropDownList ID="ddlListBoxValued" runat="server" AutoPostBack="true" Width="150px">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>FIX VALUED</asp:ListItem>
     <asp:ListItem>MASTER VALUED</asp:ListItem>
     <asp:ListItem>SESSION VALUED</asp:ListItem>
     </asp:DropDownList> </td></tr><tr>
    <td align="right"> <asp:label ID="lblListBoxSelect" runat="server" ><b> Value : </b> </asp:label></td><td align="left" colspan="3">  <asp:TextBox ID="txtListBoxValue" runat="server" CssClass="txtBox" Width="400px" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender8" runat ="server"    TargetControlID ="txtListBoxValue" WatermarkCssClass ="water"    WatermarkText ="Enter comma saperated Value"></asp:TextBoxWatermarkExtender>
    </td></tr>          

     <tr><td align="right"><asp:label ID="lblListSelectMaster" runat="server" ><b> Select Master :</b></asp:label></td><td align="left" colspan="3"><asp:DropDownList ID="ddlLstMasterSelect" runat="server"  Width="150px" AutoPostBack="true" >
        </asp:DropDownList>
     <asp:DropDownList ID="ddlLstBxField"  runat="server" Width="150px">
         </asp:DropDownList></td> </tr>
     
<tr>     
 <td align="right"><label> Mandatory Field : </label>
</td><td align="left" colspan="4"><asp:CheckBox ID="chklstBoxMan" runat="server" Text="Field is Mandatory" /><asp:CheckBox ID="chkLstBoxActive" runat="server" Text="Active" /><asp:CheckBox ID="chkLstBoxEditable" runat="server" Text="Editable" /><asp:CheckBox ID="chkLstBoxWF" runat="server" Text="Work Flow" /> </td>
<td> <asp:Button ID="btnChkLstBoxSave" runat="Server" CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>
   
</table>

</div>
</td>
</tr>
</table>
</contenttemplate>
                <triggers>
<asp:AsyncPostBackTrigger ControlID="ListBoxcancel" />
</triggers>

            </asp:UpdatePanel>

        </div>
    </asp:Panel>


    <asp:Button id="btnChkBoxLst" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupChkListBoxSelect" runat="server"
        TargetControlID="btnChkBoxLst" PopupControlID="pnlChkListBox"
        CancelControlID="txtChkLstBoxCancl" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlChkListBox" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">


            <asp:UpdatePanel ID="updChk" runat="server" UpdateMode="Conditional">
                <contenttemplate>

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="CheckBoxF" runat="server"></asp:Label> Apply Fields CheckBox List</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtChkLstBoxCancl" ImageUrl="images/close.png" runat="server" OnClick="chkClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="2px" cellpadding="2px" width="100%">
   <tr>
     <td align="left" colspan="3"> <asp:Label ID="lblChkBoxlst" runat="server"   ForeColor="Red"></asp:Label></td>
     </tr><tr><td> </td></tr>

     <tr><td align="right"> <label> Display Name  : </label></td><td align="left" colspan="3"><asp:TextBox  ID="txtChkBoxList" runat="server" CssClass="txtBox" ></asp:TextBox>
      &nbsp;&nbsp;<label> Field Type :  </label><asp:DropDownList ID="ddlChkListBox" runat="server" AutoPostBack="true" Width="150px">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>FIX VALUED</asp:ListItem>
     <asp:ListItem>MASTER VALUED</asp:ListItem>
     </asp:DropDownList> </td></tr><tr>
    <td align="right"> <asp:label id="lblChlVal" runat="server"><b> Value :  </b></asp:label></td><td align="left" colspan="3"> <asp:TextBox ID="txtChkListBox" runat="server" CssClass="txtBox" Width="500px" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender9" runat ="server"    TargetControlID ="txtChkListBox" WatermarkCssClass ="water"    WatermarkText ="Enter comma saperated Value"></asp:TextBoxWatermarkExtender>
    </td></tr>          

    <tr><td align="right"><asp:label ID="lblChkMastr" runat="server" ><b>Select Master :</b> </asp:label></td><td align="left" colspan="3"><asp:DropDownList ID="ddlChkMasterSelect" runat="server"  Width="150px" AutoPostBack="true" >
        </asp:DropDownList>
     <asp:DropDownList ID="ddlChkfieldVal"  runat="server" Width="150px">
         </asp:DropDownList></td> </tr>

<tr>     
 <td align="right"><label> Mandatory Field :</label>
</td><td align="left" colspan="4">><asp:CheckBox ID="chklstMandatory" runat="server" Text="Field is Mandatory" /><asp:CheckBox ID="ChklistBoxActive" runat="server" Text="Active" /><asp:CheckBox ID="chklistBoxEditable" runat="server" Text="Editable" /> <asp:CheckBox ID="chkListBoxWF" runat="server" Text="Work Flow" /></td>
<td> <asp:Button ID="btnchkBoxLstSave" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>
   
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
                <triggers>
<asp:AsyncPostBackTrigger ControlID="txtChkLstBoxCancl" />
</triggers>

            </asp:UpdatePanel>

        </div>
    </asp:Panel>

    <asp:Button id="btnFileUploader" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupFileUoloader" runat="server"
        TargetControlID="btnFileUploader" PopupControlID="pnlFileUploader"
        CancelControlID="fileuploaderCancel" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlFileUploader" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">

            <asp:UpdatePanel ID="updFU" runat="server" UpdateMode="Conditional">
                <contenttemplate>


<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3><asp:Label ID="FileUploaderF" runat="server"></asp:Label> Apply Fields File Uploader</h3></td>
<td style="width:20px"><asp:ImageButton ID="fileuploaderCancel" ImageUrl="images/close.png" runat="server" OnClick="btnFUclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">

    <tr>
     <td align="left"> <asp:Label ID="lblfileuploader" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name : </label><asp:TextBox ID="txtFUDiaplayName" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     </tr>
     <tr>
     <td  align="left"><label>* Data Types : </label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlFUDataTypes" runat="server" Width="150px" AutoPostBack="true" OnSelectedIndexChanged="ddlFUDataTypes_SelectedIndexChanged" >
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Image</asp:ListItem>
     <asp:ListItem>File</asp:ListItem>
    <%-- <asp:ListItem>Datetime</asp:ListItem>--%>
     </asp:DropDownList> </td>
     
       <td align="left"> <label>Field Length : </label>  <asp:TextBox ID="txtFUMinL" runat="server" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender12" runat ="server"    TargetControlID ="txtFUMinL" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
             <asp:TextBox ID="txtFUMaxL" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender13" runat ="server"    TargetControlID ="txtFUMaxL" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>   </tr>
  <tr><td> </td><td align="left"> <label>Upload Type : </label><asp:TextBox ID="txtFupload" runat="server" Width="300px" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="txtWaterexMarkFLU" runat ="server"    TargetControlID ="txtFupload" WatermarkCssClass ="water"    WatermarkText ="Enter extension of file  ex:- .txt,.cvc,.xlsx "></asp:TextBoxWatermarkExtender></tr>
<tr id="ImgType" runat="server"  visible="true" valign="top">
     <td align="left" colspan="3"> <asp:Panel ID="pnlFPFields" runat="server" ScrollBars="Auto" Height="100px" BorderStyle="Double"><table cellpadding="0" cellspacing="0" width="100%"><tr style="vertical-align:top;"><td align="left"><label style="vertical-align:top;">Selection Fields : </label></td><td><asp:CheckBoxList ID="chklFUL" onclick="GetSelectedValue1();" runat="server"></asp:CheckBoxList></td><td align="right" abbr="Display Name values"><asp:TextBox ID="txtDPFU" runat="server" TextMode="MultiLine"  Height="90px"></asp:TextBox></td></tr></table> </asp:Panel></td>
</tr>
<tr>
<td> </td> <td align="left"> <asp:CheckBox ID="chkIsEinvoiceAttachment" runat="server" Text="Is EinvoiceAttachment" /> <asp:CheckBox ID="chkFUMandaotry" runat="server" Text="Field is Mandatory" /> <asp:CheckBox ID="chkFUActive" runat="server" Text="Active" /> <asp:CheckBox ID="chkFUEditable" runat="server" Text="Editable" /> <asp:CheckBox ID="chkFUWF" runat="server" Text="Work Flow" /></td>
<td> <asp:Button ID="btnFUSave" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>

</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>

        </div>
    </asp:Panel>

    <%--Added By Mayank--%>

    <asp:Button id="btnLookUpFld" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupLookUp" runat="server"
        TargetControlID="btnLookUpFld" PopupControlID="pnlLookUp"
        CancelControlID="lookupClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlLookUp" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">

            <asp:UpdatePanel ID="updatepanelLookUp" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="LookupF" runat="server"></asp:Label> Apply Fields Look up</h3></td>
<td style="width:20px"><asp:ImageButton ID="lookupClose" ImageUrl="images/close.png" runat="server" OnClick="btnLookUpClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblLookUP" runat="server"  Text="Please select the field" ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label>* Display Name : </label><asp:TextBox ID="txtLookUP" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label>* Data Types  : </label>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlLUdatatypes" runat="server" >
      <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList> <label> Description :</label><asp:TextBox ID="txtlukupdesc" runat="server" CssClass="txtBox"></asp:TextBox></td>
     </tr>
     <tr>
     <td  align="left"><label>* Select Form : </label>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlLookUpMasterField" runat="server"  Width="150px" AutoPostBack="true">
         
     </asp:DropDownList> </td>
     <td  align="left"><label>* Select Field: </label><asp:DropDownList ID="ddlLookUpField" runat="server" Width="150px" AutoPostBack="true">
     
     </asp:DropDownList> </td></tr>
        <tr><td align="left" colspan="2" id="lukup" runat="server" visible="false"> <label>Field Length :</label>  <asp:TextBox ID="txtlukupmin" runat="server" CssClass="txtBox"  > </asp:TextBox>
           &nbsp;&nbsp;     <asp:TextBox ID="txtlukupmax" runat="server" CssClass="txtBox" Width="146px" ></asp:TextBox>
             </td></tr>
               <tr><td align="left" colspan="2" id="Td15" runat="server" visible="true"> <label>Show Text :</label> 
           &nbsp;&nbsp;     
                   <asp:CheckBox ID="chkShowText" runat="server" />
             </td></tr>
     <tr>
<td align="left"><label> Mandatory Field  : </label></td> <td align="left"> <asp:CheckBox ID="chkLUMan" runat="server" Text="Field is Mandatory" /> <asp:CheckBox ID="chkLUAct" runat="server" Text="Active" /> <asp:CheckBox ID="chkLUEdi" runat="server" Text="Editable" /> <asp:CheckBox ID="chkLUWF" runat="server" Text="Work Flow" /></td>
<td> <asp:Button ID="btnlookUpSave" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>

</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
                <triggers>
<asp:AsyncPostBackTrigger ControlID="lookupClose" />
</triggers>

            </asp:UpdatePanel>

        </div>
    </asp:Panel>

    <%# CType(Container, GridViewRow).RowIndex + 1%>

    <asp:Button id="btnDDlLookUpFld" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupDDlLookup" runat="server"
        TargetControlID="btnDDlLookUpFld" PopupControlID="pnlDDlLookUp"
        CancelControlID="DDllookupClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlDDlLookUp" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">

            <asp:UpdatePanel ID="updatepanelDDlLookUp" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="DDlLookupF" runat="server"></asp:Label> Apply Fields Look Up Dropdown</h3></td>
<td style="width:20px"><asp:ImageButton ID="DDllookupClose" ImageUrl="images/close.png" runat="server" OnClick="btnDDlLookUpClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
    <tr>
     <td align="left"> <asp:Label ID="lblDDlLookUP" runat="server"  Text="Please select the field" ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label>* Display Name : </label><asp:TextBox ID="txtDDlLookUP" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label>* Data Types  : </label>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlDDlLUdatatypes" runat="server" >
      <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList> <label> Description :</label><asp:TextBox ID="txtDDllukupdesc" runat="server" CssClass="txtBox"></asp:TextBox></td>
     </tr>
     <tr>
     <td  align="left"><label>* Select Form : </label>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlDDlLookUpMasterField" runat="server"  Width="150px" AutoPostBack="true">
         
     </asp:DropDownList> </td>
     <td  align="left"><label>* Select Field: </label><asp:DropDownList ID="ddlDDlLookUpField" runat="server" Width="150px" AutoPostBack="true">
     
     </asp:DropDownList> </td></tr>
        <tr><td align="left" colspan="2" id="DDllukup" runat="server" visible="false"> <label>Field Length :</label>  <asp:TextBox ID="txtDDllukupmin" runat="server" CssClass="txtBox"  > </asp:TextBox>
           &nbsp;&nbsp;     <asp:TextBox ID="txtDDllukupmax" runat="server" CssClass="txtBox" Width="146px" ></asp:TextBox>
             </td></tr>
     <tr>
<td align="left"><label> Mandatory Field  : </label></td> <td align="left"> <asp:CheckBox ID="chkDDlLUMan" runat="server" Text="Field is Mandatory" /> <asp:CheckBox ID="chkDDlLUAct" runat="server" Text="Active" /> <asp:CheckBox ID="chkDDlLUEdi" runat="server" Text="Editable" /> <asp:CheckBox ID="chkDDlLUWF" runat="server" Text="Work Flow" /></td>
<td> <asp:Button ID="btnDDllookUpSave" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>

</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
                <triggers>
<asp:AsyncPostBackTrigger ControlID="DDllookupClose" />
</triggers>

            </asp:UpdatePanel>

        </div>
    </asp:Panel>

    <%--  <asp:ListItem >Child Item Specific Text</asp:ListItem>--%>
    <%# CType(Container, GridViewRow).RowIndex + 1%>

    <asp:Button id="BtnCF" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="CFModalPopup" runat="server"
        TargetControlID="BtnCF" PopupControlID="pnlCF1" CancelControlID="Clfclose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlCF1" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: none" BackColor="white">
<div class="box">
    <asp:UpdatePanel ID="upd_CF" runat="server" UpdateMode="Conditional">
        <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="CalculativeF" runat="server"></asp:Label> Apply Fields Calculative</h3></td>
<td style="width:20px"><asp:ImageButton ID="Clfclose" ImageUrl="images/close.png" runat="server" OnClick="btnCFclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="4"> <asp:Label ID="CFlbl" runat="server" ></asp:Label></td>
     </tr> 
     <tr><td align="left" style="width:100px"><label>Display Name:</label></td>
     <td align ="left"  colspan ="3" ><asp:TextBox ID="CF_txtdisplayname" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr>
     <tr><td align ="left"><label> Default Value :</label></td><td align ="left"  colspan ="3"><asp:TextBox ID="txtCalDefVal" runat="server" CssClass="txtBox"></asp:TextBox> </tr></td>
     <tr><td align="left" valign ="top" ><label>Fields:</label></td>
     <td align ="left" style="width:300px" ><asp:TextBox ID="CF_TxtFields" Width="100%" runat="server" CssClass="txtBox" TextMode ="MultiLine" ></asp:TextBox> </td>
      <td align="left" valign ="top"  style="width:130px"  ><label>Formula Editor:</label></td>
     <td align ="left" style="width:300px" ><asp:TextBox ID="CF_txtFE" runat="server" Width="100%" CssClass="txtBox" TextMode ="MultiLine" ></asp:TextBox> </td>
     </tr>
<tr>
<td align="left"><label> Mandatory Field:</label></td> 
 <td align="left" colspan ="2"> 
 <asp:CheckBox ID="CF_chkM" runat="server" Text="Field is Mandatory" Checked ="true"  /> 
 <asp:CheckBox ID="CF_chkActive" runat="server" Text="Active" Checked ="true" /> 
 <asp:CheckBox ID="CF_chkEdit" runat="server" Text="Editable" Checked ="true" /> 
 <asp:CheckBox ID="CF_chkWF" runat="server" Text="Work Flow" /></td>
<td><asp:Button ID="CF_Save" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick="EditRecord" Width="80px"/></td>
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


    <%# CType(Container, GridViewRow).RowIndex + 1%>

    <asp:Button id="BtnDF" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="DFModalPopup" runat="server" TargetControlID="BtnDF" PopupControlID="pnlDF" CancelControlID="DFClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlDF" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpdPnlDF" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="ChildItemF" runat="server"></asp:Label> Apply Fields Child Item</h3></td>
<td style="width:20px"><asp:ImageButton ID="DFClose" ImageUrl="images/close.png" runat="server" OnClick="btnDFclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
    <td align="left" colspan ="4"> <asp:Label ID="Label2" runat="server" ></asp:Label></td>
    </tr> 
    <tr><td align="left"><label>Display Name:</label></td>
    <td align ="left" colspan ="3" ><asp:TextBox ID="TextBox1" runat="server" CssClass="txtBox"></asp:TextBox> </td>
    </tr>
   
    <tr><td align="left" valign ="top" ><label>Detail Form:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlDF" runat ="server" Width="150px" 
            CssClass="txtBox" AutoPostBack="True"></asp:DropDownList></td>
    <td align ="left"><asp:CheckBox ID="ChkRef" runat="server" Text="Referral Key"  AutoPostBack ="true" OnCheckedChanged ="ReferralCheck" /> </td>
    <td align ="left" ><asp:DropDownList ID ="ddlref" runat ="server" Width="150px" CssClass="txtBox" Enabled ="false" ></asp:DropDownList></td>

    <tr>
    <td align="left" valign ="top" ><label>Main Form Field:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlMFF" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
    <td align="left" valign ="top" ><label>Child Form Field:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlCFF" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
    </tr>
    </tr>
<tr>
<td align="left"><label> Mandatory Field:</label></td> 
 <td align="left" colspan ="2"> 
 <asp:CheckBox ID="DFChkM" runat="server" Text="Field is Mandatory" Checked ="true"  /> 
 <asp:CheckBox ID="DFchkA" runat="server" Text="Active" Checked ="true" /> 
 <asp:CheckBox ID="DFchkE" runat="server" Text="Editable" Checked ="true" /> 
 <asp:CheckBox ID="DFchkw" runat="server" Text="Work Flow" /></td>
<td><asp:Button ID="btnDF1" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick="EditRecord" Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>


    <%--     added by balmiki--%>
    <asp:Button id="btnBlockForm" runat="server" style="Display: none" />
    <asp:ModalPopupExtender ID="MPblockForm" runat="server"
        TargetControlID="btnBlockForm" PopupControlID="pnlPopupblockForm"
        CancelControlID="btnCloseBlockform" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupblockForm" runat="server" Width="500px" style="Display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Field Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseBlockform" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updBlockform" runat="server" UpdateMode="Conditional">
                            <contenttemplate> 
<h2> <asp:Label ID="lblblockform" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
      <div style="width:100%;text-align:right" >
                <asp:Button ID="btnblocksave" runat="server" Text="Block"  Width="90px" OnClick="BlockFormRecord" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>
    
    <asp:Button id="btnShowPopupDeleteField" runat="server" style="Display: none" />
    <asp:ModalPopupExtender ID="ModalPoUPDeleteField" runat="server"
        TargetControlID="btnShowPopupDeleteField" PopupControlID="pnlPopupDeleteField"
        CancelControlID="btnCloseDeleteField" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDeleteField" runat="server" Width="500px" style="Display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Field Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDeleteField" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updDeleteField" runat="server" UpdateMode="Conditional">
                            <contenttemplate> 
<h2> <asp:Label ID="lblmessagedelete" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnDeleteField" runat="server" Text="Yes Delete"  Width="90px" OnClick="DeleteFieldRecord" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>

     <asp:Button id="BtnCHT" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupCHT" runat="server"
        TargetControlID="BtnCHT" PopupControlID="pnlCHT" CancelControlID="CHTCLOSE"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlCHT" runat="server" Width="600px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpDCHT" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3><asp:Label ID="ChilditemtotalF" runat="server"></asp:Label> Apply Fields Child Item Total</h3></td>
<td style="width:20px"><asp:ImageButton ID="CHTCLOSE" ImageUrl="images/close.png" runat="server" OnClick="btnCHTclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="3"> <asp:Label ID="Label3" runat="server" ></asp:Label></td>
     </tr> 
     <tr><td align="left"><label>Display Name:</label></td>
     <td align ="left" colspan ="2" ><asp:TextBox ID="TXTCHT" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     </tr>
     <tr><td align="left" valign ="top" ><label>Detail Form:</label></td>
    <td align ="left" ><asp:DropDownList ID ="DDLCHF" runat ="server" Width="150px" AutoPostBack ="true"  CssClass="txtBox"></asp:DropDownList></td>
    <td align ="left" ><asp:DropDownList ID ="DDLCHFF" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
     </tr>
<tr>
  <td align="left" colspan ="2"> 
 <asp:CheckBox ID="CHT_ChKM" runat="server" Text="Field is Mandatory" Checked ="true"  /> 
 <asp:CheckBox ID="CHT_ChKA" runat="server" Text="Active" Checked ="true" /> 
 <asp:CheckBox ID="CHT_ChKE" runat="server" Text="Editable" Checked ="true" /> 
 <asp:CheckBox ID="CHT_ChKWF" runat="server" Text="Work Flow" /></td>
<td><asp:Button ID="BTNCHTSAVE" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick="EditRecord" Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%--<asp:CheckBox ID="CheckBox3" runat="server" Text="Editable" /> <asp:CheckBox ID="CheckBox4" runat="server" Text="Work Flow" />--%>
      <asp:Button id="btnchldST" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mod_CHLDST" runat="server"
        TargetControlID="btnchldST" PopupControlID="pnlCHLDST" CancelControlID="CHLDST"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlCHLDST" runat="server" Width="600px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="upCHLDST" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3> <asp:Label ID="lblCHLDST" runat="server"></asp:Label> Apply Setting for Specific Child Item Text</h3></td>
<td style="width:20px"><asp:ImageButton ID="CHLDST" ImageUrl="images/close.png" runat="server" OnClick="CHLDSTClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="3"> <asp:Label ID="lblmsgCHLDST" runat="server" ></asp:Label></td>
     </tr> 
     <tr><td align="left"><label style="display:none;">Display Name:</label></td>
     <td align ="left"><asp:DropDownList ID="ddlchldvalue" runat="server" Width="150px" CssClass="txtBox"></asp:DropDownList><asp:TextBox ID="txtCHLDSTD" Visible="false" runat="server" CssClass="txtBox"></asp:TextBox></td><td align="left" style="margin-left:0px"></td>
     </tr>
     <tr><td align="left" valign ="top" ><label>Detail Form:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlCHLDSTC" runat ="server" Width="150px" AutoPostBack ="true"  CssClass="txtBox"></asp:DropDownList></td>
    <td align ="left" ><asp:DropDownList ID ="ddlCHLDSTCommanChld" runat ="server" Width="150px" CssClass="txtBox"  AutoPostBack ="true" ></asp:DropDownList></td>
         <td align ="left"><asp:DropDownList ID ="ddlCHLDCommanVal" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
         <asp:HiddenField ID="hdnDDNVALUE" runat="server" />
     </tr>
<tr>
  <td align="left" colspan ="2"> 
 <asp:CheckBox ID="chkCHLDSTFM" runat="server" Text="Field is Mandatory" Checked ="true" Visible="false"  /> 
 <asp:CheckBox ID="chkCHLDSTA" runat="server" Text="Active" Checked ="true" Visible="false" /> 
 <asp:CheckBox ID="chkCHLDSTE" runat="server" Text="Editable" Checked ="true" Visible="false" /> 
 <asp:CheckBox ID="chkCHLDWF" runat="server" Text="Work Flow" Visible="false" /></td>
     <asp:HiddenField ID="hdnTempval" runat="server" />
<td><asp:Button ID="btnCHLDSTSAVE" runat="Server"  CssClass="btnNew" Text="UPDATE" OnClick="ChldEditRecord" Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <%--Add Tally Synch by mayank --%><%--<td align="left"> <label>Prefix Text box :</label>  <asp:TextBox ID="txtprefixnew" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender19" runat ="server"    TargetControlID ="txtprefixnew" WatermarkCssClass ="water" WatermarkText ="Prefix text "></asp:TextBoxWatermarkExtender></td>--%>
    <%--Changes for USER Master Values--%>    
    <asp:ModalPopupExtender ID="modUserMapping" runat="server"
        TargetControlID="btnUserMapping" PopupControlID="pnlUserMapping" CancelControlID="imgBtnUserMappingClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
      <asp:Button id="btnUserMapping" runat="server" style="display: none" />
      <asp:Panel ID="pnlUserMapping" runat="server" Width="600px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
                   <asp:UpdatePanel ID="upUserMapping" runat="server" UpdateMode="Conditional">
                            <contenttemplate>
                               <table cellspacing="2px" cellpadding="2px" width="100%">
                                 <tr>
                                        <td style="width:580px"><h3> <asp:Label ID="Label26" runat="server"></asp:Label> Apply Setting for Master Mapping fields</h3></td>
                                        <td style="width:20px"><asp:ImageButton ID="imgBtnUserMappingClose" ImageUrl="images/close.png" runat="server" OnClick="UserMappingClose"/></td>
                                 </tr>
                                 <tr>
                                        <td colspan="2">
                                        <div class="form">
                                            <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                                <tr>
                                                    <td>
                                                            <asp:Label ID="lblErrorUserMapping" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <asp:Label ID="lbltid" runat="server" Visible="false"></asp:Label>
                                                      <td align="left"><asp:Label ID="lblTargetDocumentType" Font-Bold="true" runat="server"></asp:Label> <label > Fields:</label></td>
                                                    <td align ="left" ><asp:DropDownList ID ="ddlTargetDocumentFields" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
                                                    <td align="left"><asp:Label ID="lblDocumentType" Font-Bold="true" runat="server"></asp:Label> <label > Fields:</label></td>
                                                     <td align ="left"><asp:DropDownList ID ="ddlDocumentFields" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
                                                </tr>
                                                 <tr>
                                                    <td colspan="4">
                                                        <asp:Button ID="btnAddFiedsForUserMapping" runat="Server"  CssClass="btnNew" Text="ADD" OnClick="btnAddFiedsForUserMapping_Click"    Width="80px"/>
                                                    </td>
                                                </tr>
                                                 <tr valign="top">
                                                      <td align="left" colspan="1"><label >Mapping Text:</label></td>
                                                     <td align="left" colspan="3"><asp:TextBox ID="txtMappingDisplayText" TextMode="MultiLine" Height="40" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                                </tr>
                                                <%-- <tr valign="top">
                                                      <td align="left" colspan="1"><label >Mapping Value:</label></td>
                                                     <td align="left" colspan="3"><asp:TextBox ID="txtMappingDisplayValue" TextMode="MultiLine" Height="40" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                                </tr>--%>
                                               
                                                <tr>
                                                     <td colspan="4"><asp:Button ID="btnSaveUserMapping" runat="Server"  CssClass="btnNew" Text="UPDATE" Width="80px" OnClick="btnSaveUserMapping_Click" /></td>
                                                </tr>
                                            </table>
                                        </div>
                                        </td>
                                </tr>
                             </table>
                            </contenttemplate>
            </asp:UpdatePanel>
            </div>
          </asp:Panel>

    <asp:Button id="Btnchildmax" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mdl_childmax" runat="server" TargetControlID="Btnchildmax" PopupControlID="pnlchildmax" CancelControlID="childmaxCLOSE"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="PNLCHILDMAX" runat="server" Width="600px" Height="330px" ScrollBars="Auto" style="" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Upchildmax" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3><asp:Label ID="childmaxlbl" runat="server"></asp:Label> Apply Fields Child Item Max</h3></td>
<td style="width:20px"><asp:ImageButton ID="childmaxCLOSE" ImageUrl="images/close.png" runat="server" OnClick="btnChildmaxclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form"> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="3"> <asp:Label ID="Label10" runat="server" ></asp:Label></td>
     </tr> 
     <tr><td align="left"><label>Display Name:</label></td>
     <td align ="left" colspan ="2" ><asp:TextBox ID="TXTChilddisplay" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     </tr>
     <tr><td align="left" valign ="top" ><label>Detail Form:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlchildF" runat ="server" Width="150px" AutoPostBack ="true"  CssClass="txtBox"></asp:DropDownList></td>
    <td align ="left" ><asp:DropDownList ID ="ddlchildHFF" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
     </tr>
<tr>
  <td align="left" colspan ="2"> 
 <asp:CheckBox ID="Childmax_ChKM" runat="server" Text="Field is Mandatory" Checked ="true"  /> 
 <asp:CheckBox ID="Childmax_ChKA" runat="server" Text="Active" Checked ="true" /> 
 <asp:CheckBox ID="Childmax_ChKE" runat="server" Text="Editable" Checked ="true" /> 
 <asp:CheckBox ID="Childmax_ChKWF" runat="server" Text="Work Flow" /></td>
<td><asp:Button ID="BTNChildmaxSAVE" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick="EditRecord" Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>  
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:Button id="BtnSRF" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="SRF_ModalPopup" runat="server"
        TargetControlID="BtnSRF" PopupControlID="pnlSRF" CancelControlID="SRFClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlSRF" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
       <div class="box">
            <asp:UpdatePanel ID="UpdPnlSF" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3>Apply Self Reference Fields</h3></td>
<td style="width:20px"><asp:ImageButton ID="SRFClose" ImageUrl="images/close.png" runat="server" OnClick="btnSRFclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="4"> <asp:Label ID="Label4" runat="server"  Text=""></asp:Label></td>
     </tr> 
     <tr><td align="left"><label>Display Name:</label></td>
     <td align ="left" colspan ="3" ><asp:TextBox ID="SRF_txtdisplay" runat="server" CssClass="txtBox"></asp:TextBox></td>
     </tr>
  <tr><td align="left"><label>Form Name:</label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlSF" runat="server" Width="150px"  
             CssClass="txtBox" AutoPostBack="True"></asp:dropdownlist></td>
     <td align="left"><label>Referencing Field:</label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlRF" runat="server" Width="150px"  CssClass="txtBox"></asp:dropdownlist></td>
     </tr>
<tr>
<td align="left"><label> Mandatory Field:</label></td> 
 <td align="left" colspan ="2"> 
 <asp:CheckBox ID="SRF_ChkM" runat="server" Text="Field is Mandatory" Checked ="true"  /> 
 <asp:CheckBox ID="SRF_ChkA" runat="server" Text="Active" Checked ="true" /> 
 <asp:CheckBox ID="SRF_ChkE" runat="server" Text="Editable" Checked ="true" /> 
 <asp:CheckBox ID="SRF_ChkWF" runat="server" Text="Work Flow" /></td>
<td><asp:Button ID="BtnSRFSave" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick="EditRecord" Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:Button id="BTNPR" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="PR_ModalPopup" runat="server"
        TargetControlID="BtnPR" PopupControlID="pnlPR" CancelControlID="PRClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPR" runat="server" Width="600px" Height="200px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpdPNLPR" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3><asp:Label ID="ParentFieldsF" runat="server"></asp:Label> Apply Parent Fields</h3></td>
<td style="width:20px"><asp:ImageButton ID="PRClose" ImageUrl="images/close.png" runat="server" OnClick="btnPRclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
        <tr>
     <td align="left" colspan ="4"> <asp:Label ID="Label5" runat="server"  Text=""></asp:Label></td>
     </tr> 
     <tr><td align="left"><label>Display Name:</label></td>
     <td align ="left" ><asp:TextBox ID="prTxtName" runat="server" CssClass="txtBox"></asp:TextBox></td>
     
  <td align="left"><label>Parent Field:</label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlPR" runat="server" Width="120px"  CssClass="txtBox" ></asp:dropdownlist></td  
   </tr>
<tr>
<td align="left"><label> Mandatory Field:</label></td> 
 <td align="left" colspan ="2"> 
 <asp:CheckBox ID="PR_ChkM" runat="server" Text="Mandatory" Checked ="true"  /> 
 <asp:CheckBox ID="PR_ChkA" runat="server" Text="Active" Checked ="true" /> 
 <asp:CheckBox ID="PR_ChkE" runat="server" Text="Editable" Checked ="true" /> 
 <asp:CheckBox ID="PR_ChkWF" runat="server" Text="Work Flow" /></td>
<td><asp:Button ID="BtnPR_Save" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick="EditRecord" Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <%--Added By Mayank--%>
    <asp:Button id="btnEexMaP" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="MP_ExpMap" runat="server"
        TargetControlID="btnEexMaP" PopupControlID="pnlExMp" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlExMp" runat="server" Width="600px" Height="200px" ScrollBars="Auto" BackColor="white">
<div class="box">
    <asp:UpdatePanel ID="updExMp" runat="server" UpdateMode="Conditional">
        <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3><asp:Label runat="server" ID="Exportmappingss"></asp:Label> Apply Export Mapping</h3></td>
<td style="width:20px"><asp:ImageButton ID="exMPClose" ImageUrl="images/close.png" runat="server" onclick="btncloseexportmapping" /></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="4"> <asp:Label ID="lblMesExMp" runat="server"  Text=""></asp:Label></td>
     </tr> 
       <tr>  
  <td align="left"><label>Master Form </label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddltarget" runat="server" Width="120px"  
             CssClass="txtBox" AutoPostBack="True" ></asp:dropdownlist></td
  
   </tr>
   <tr>

   <td align ="left" ><label>Source Fields</label></td></td>
   <td align ="left" ><asp:dropdownlist ID="ddlSourceFields" runat="server" Width="120px"  CssClass="txtBox" ></asp:dropdownlist></td>
   <td align="left"><label>Target Fields</label></td> <td align ="left"  ><asp:dropdownlist ID="ddlTargetField" runat="server" Width="120px"  CssClass="txtBox" ></asp:dropdownlist></td> 
   </tr>
<tr>
<td colspan ="4" align ="right" ><asp:Button ID="BtnExport" runat="Server"  CssClass="btnNew" Text="SAVE" OnClick ="Savemapping"  Width="80px"/></td>
</tr>
<tr>
<td colspan ="4" width="100%">
<asp:GridView ID="GrdFM" runat="server" AutoGenerateColumns="False" 
             CellPadding="4" DataKeyNames="TID" 
               AllowSorting="True" Width ="100%"  BackColor="White"   >
                     <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                    <Columns>
                    <asp:BoundField DataField="SDisplayname" HeaderText="Source Field">
                       <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle  HorizontalAlign="Left"/>
                       </asp:BoundField>
                       <asp:BoundField DataField="TDisplayname" HeaderText="Target Feild">
                       <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle  HorizontalAlign="Left"/>
                       </asp:BoundField>
                       <asp:TemplateField HeaderText ="Action" >
                       <ItemTemplate >
                       <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteMapping" ToolTip ="Delete Mapping" AlternateText="Delete"/>
                       </ItemTemplate>
                       </asp:TemplateField>
                    </Columns>
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                   <AlternatingRowStyle BackColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                </asp:GridView>
</td>
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

    <%-- <asp:ListItem>Datetime</asp:ListItem>--%><%# CType(Container, GridViewRow).RowIndex + 1%><%--  <asp:ListItem >Child Item Specific Text</asp:ListItem>--%>
    <asp:Button id="BtnKCKF" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="KCKF_ModalPopup" runat="server"
        TargetControlID="BtnKCKF" PopupControlID="pnlKCKF" CancelControlID="KCKFClose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlKCKF" runat="server" Width="600px" Height="150px" ScrollBars="Auto" style="display: none" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpdPNLKCKF" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3><asp:Label  ID="KickingF" runat="server"></asp:Label> Apply Kicking Fields</h3></td>
<td style="width:20px"><asp:ImageButton ID="KCKFClose" ImageUrl="images/close.png" runat="server" OnClick="btnKCKFClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
<table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left" colspan ="4"> <asp:Label ID="LBKCK" runat="server"  ></asp:Label></td>
     </tr> 
   <tr>
    <td align="left" valign ="top" ><label>DocumentType:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlDoctype" runat ="server" Width="150px" AutoPostBack ="true"  CssClass="txtBox"></asp:DropDownList></td>
    <td align="left" valign ="top" ><label>Field:</label></td>
    <td align ="left" ><asp:DropDownList ID ="ddlKFields" runat ="server" Width="150px" CssClass="txtBox"></asp:DropDownList></td>
   </tr>
<tr>
<td align="left" valign ="top" ><label>Update Logic:</label></td>
    <td align ="left" colspan ="2" ><asp:DropDownList ID ="ddlLogic" runat ="server" Width="150px" CssClass="txtBox">
    <asp:ListItem Value ="">SELECT LOGIC</asp:ListItem>
    <asp:ListItem Value ="+">Add</asp:ListItem>
    <asp:ListItem Value ="-">Subtract</asp:ListItem>
    <asp:ListItem Value ="*">Multiplication</asp:ListItem>
    <asp:ListItem Value ="R" >Replace</asp:ListItem>
    </asp:DropDownList>
    </td>
<td align ="left" ><asp:Button ID="BtnKCKFSave" runat="Server"  CssClass="btnNew" Text="Submit"  Width="80px"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%# CType(Container, GridViewRow).RowIndex + 1%>
    <asp:Button id="btnfieldBlock" runat="server" style="Display: none" />
    <asp:ModalPopupExtender ID="MPFieldBlock" runat="server"
        TargetControlID="btnfieldBlock" PopupControlID="pnlPopupblockField"
        CancelControlID="btnCloseBlockfield" BackgroundCssClass="modalBackground"
        DropShadow="true">
   </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupblockField" runat="server" Width="500px" style="Display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Field LOCK : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseBlockfield" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdLockField" runat="server" UpdateMode="Conditional">
                            <contenttemplate> 
<h2> <asp:Label ID="lblLockField" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="BtnLockfield" runat="server" Text="lock"  Width="90px" OnClick="LockfieldRecord" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>

    <%# CType(Container, GridViewRow).RowIndex + 1%>
    <asp:Button id="btnShowPopupinconfig" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtenderinconfig" runat="server" PopupControlID="pnlPopupinconfig" TargetControlID="btnShowPopupinconfig"
        CancelControlID="btnCloseuKEY" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupinconfig" runat="server" Width="1000px" Height="300px" BackColor="White" style="">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td width="980px">
                        <h3>Inline Mapping & Filtering Configuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseinconfig" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="Div2">
                            <asp:UpdatePanel ID="Updpanelinconfig" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
        <div class="form" style="text-align:left ; "> 
    
            <div>
                <table><tr>
        <td colspan="4">
            <asp:Label ID="Lblinlineeditmsg" runat ="server"  ForeColor="Red" ></asp:Label>
        </td>
    </tr><tr>
            <td align="left" style="width:100px"><asp:Label ID="lblttype" runat="server"  Text="Type" Font-Size="Smaller"  Font-Bold="True" ></asp:Label></span></td>
            <td  align="left" style="width:150px"><asp:DropDownList ID="ddlintype" Width="150px" AutoPostBack="True"  runat="server"   >
                <asp:ListItem Value="0">SELECT</asp:ListItem>
                <asp:ListItem Value="1">MASTER</asp:ListItem>
                <asp:ListItem Value="2">DOCUMENT</asp:ListItem>
                <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                

                </asp:DropDownList></td>

            <td align="left" style="width:120px"><asp:Label ID="lblsdoc" runat="server"  Text="Source Document" Font-Size="Smaller"  Font-Bold="True" ></asp:Label> </td>
            <td align="left" style="width:200px"><asp:DropDownList ID="ddlsdoc" Width="200px" AutoPostBack="True"  runat="server" ></asp:DropDownList></td>
        <td align="left" style="width:120px"><asp:Label ID="lblInLineEdit" runat="server"  Text="Is In Line Editing" Font-Size="Smaller"  Font-Bold="True" ></asp:Label></td>
            <td align="left" style="width:200px" colspan="2"><asp:DropDownList ID="ddlInlineEdit" Width="200px" AutoPostBack="True"  runat="server" OnSelectedIndexChanged="ddlInlineEdit_SelectedIndexChanged" >
                <asp:ListItem >SELECT</asp:ListItem>
                <asp:ListItem Value="1">ACTIVE</asp:ListItem>
                <asp:ListItem Value="0">IN ACTIVE</asp:ListItem>

                                                             </asp:DropDownList><br /> </td>

            </tr></table>
            </div>
            <br />

<asp:TabContainer ID="TabContainer2" Width="100%"  runat="server" CssClass="fancy fancy-green" ActiveTabIndex="1">
                    <asp:TabPanel ID="TabPanel1"  runat="server"><HeaderTemplate>
Mapping Fields
</HeaderTemplate>
<ContentTemplate>
    
            <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical"><div class="form" style="text-align:left"><table border="0" width="100%"><caption><br /><tr><td align="left" style="width:110px"><asp:Label ID="lblcurdoc" runat="server" Font-Bold="True" Font-Size="Smaller" Text="Current Doc Fields"></asp:Label></td>
    <td align="left" style="width:150px"><asp:DropDownList ID="ddlcurdoc" runat="server" Width="150px"></asp:DropDownList></td><td align="left" style="width:100px"><asp:Label ID="lblsdocf" runat="server" Font-Bold="True" Font-Size="Smaller" Text="Source Doc Fields"></asp:Label></td><td align="left" colspan="2" style="width:200px"><asp:DropDownList ID="ddlsdocf" runat="server" Width="200px"></asp:DropDownList><br /></td></tr><tr><td align="right" colspan="5"><asp:Button ID="btnsaves" runat="server" CssClass="btnNew" OnClick="saveinlinemappingconfig" Text="Save" Width="80px" /></td></tr><tr><td colspan="5" width="100%"><asp:GridView ID="gvinline" runat="server" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" CellPadding="4" DataKeyNames="TID" Width="100%"><FooterStyle CssClass="FooterStyle" /><RowStyle BackColor="#EFF3FB" CssClass="RowStyle" /><EditRowStyle CssClass="EditRowStyle" /><SelectedRowStyle CssClass="SelectedRowStyle" /><PagerStyle CssClass="PagerStyle" /><HeaderStyle BackColor="CornflowerBlue" CssClass=" HeaderStyle" Font-Bold="True" ForeColor="White" /><AlternatingRowStyle BackColor="White" CssClass="AlternatingRowStyle" /><Columns><asp:TemplateField HeaderText="S.No"><ItemTemplate></ItemTemplate><ItemStyle Width="50px" /></asp:TemplateField><asp:BoundField DataField="Type" HeaderText="Type"><HeaderStyle HorizontalAlign="Left" /><ItemStyle HorizontalAlign="Left" /></asp:BoundField><asp:BoundField DataField="Source Document" HeaderText="Source Document"><HeaderStyle HorizontalAlign="Left" /><ItemStyle HorizontalAlign="Left" /></asp:BoundField><asp:BoundField DataField="FormName" HeaderText="FormName"><HeaderStyle HorizontalAlign="Left" /><ItemStyle HorizontalAlign="Left" /></asp:BoundField><asp:BoundField DataField="item" HeaderText="Mapping Fields"><HeaderStyle HorizontalAlign="Left" /><ItemStyle HorizontalAlign="Left" /></asp:BoundField><asp:TemplateField HeaderText="Action"><ItemTemplate><asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitinline" ToolTip="Delete Inline Configuration" Width="16px" /></ItemTemplate></asp:TemplateField></Columns><sortedascendingcellstyle backcolor="#EDF6F6" /><sortedascendingheaderstyle backcolor="#0D4AC4" /><sorteddescendingcellstyle backcolor="#D6DFDF" /><sorteddescendingheaderstyle backcolor="#002876" /></asp:GridView></td></tr></caption></table></div></asp:Panel>

        
</ContentTemplate>
</asp:TabPanel>
                    <asp:TabPanel ID="TabPanel2" runat="server"><HeaderTemplate>
Filter
</HeaderTemplate>
<ContentTemplate>
<asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical">
    <br />
    <table border="0" cellpadding="0px" cellspacing="4px" width="100%">
        <tr><td align="left" style="width:200px;"><asp:Label ID="lblftype" runat="server" Font-Bold="True" Font-Size="Smaller" Text="Filter Type"></asp:Label>



</td><td align="left" style="width:150px"><asp:DropDownList ID="ddlsdtype" runat="server" AutoPostBack="True"><asp:ListItem Value="0">SELECT</asp:ListItem>
<asp:ListItem Value="1">STATIC</asp:ListItem>
<asp:ListItem Value="2">DYNAMIC</asp:ListItem>
</asp:DropDownList>



</td>
    <td align="left" style="width:200px;">
        Match Type
    </td>
         <td><asp:DropDownList ID="ddlMatchType" runat="server"><asp:ListItem>VALUE</asp:ListItem>
<asp:ListItem>TEXT</asp:ListItem>
</asp:DropDownList>




         </td>                                                                                                                            </tr><tr id="lastrow" runat="server"><td id="Td1" runat="server" align="left" style="width:200px"><asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Size="Smaller" Text="Current Doc Fields"></asp:Label>



</td>
<td id="Td2" runat="server" align="left" style="width:150px"><asp:DropDownList ID="ddlcdocfilter" runat="server" Width="150px"></asp:DropDownList>



</td>
<td id="Td3" runat="server" align="left" style="width:200px"><asp:Label ID="Label9" runat="server" Font-Bold="True" Font-Size="Smaller" Text="Operator" Width="100px"></asp:Label>



<asp:DropDownList ID="ddlopfields" runat="server"><asp:ListItem>SELECT</asp:ListItem>
<asp:ListItem>=</asp:ListItem>
<asp:ListItem>&gt;</asp:ListItem>
<asp:ListItem>&lt;</asp:ListItem>
<asp:ListItem>&gt;=</asp:ListItem>
<asp:ListItem>&lt;=</asp:ListItem>
</asp:DropDownList>



</td>
<td id="Td4" runat="server" align="left"><asp:DropDownList ID="ddlsdocfieldss" runat="server" Visible="False"></asp:DropDownList>



</td>
<td id="Td5" runat="server" align="left"><asp:TextBox ID="txtsdocvalue" runat="server" Visible="False"></asp:TextBox>



</td>
</tr>



<tr><td align="right" colspan="5"><asp:Button ID="btnsavefields" runat="server" CssClass="btnNew" OnClick="saveinlinefilterconfig" Text="Save" />



</td></tr><tr><td colspan="5" width="100%"><asp:GridView ID="gvinlinefilter" runat="server" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" CellPadding="4" DataKeyNames="TID" Width="100%">
<AlternatingRowStyle BackColor="White" CssClass="AlternatingRowStyle" />
<Columns>
<asp:TemplateField HeaderText="S.No"><ItemTemplate>
<%# CType(Container, GridViewRow).RowIndex + 1%>
</ItemTemplate>

<ItemStyle Width="50px" />
</asp:TemplateField>
<asp:BoundField DataField="Type" HeaderText="Type">
<HeaderStyle HorizontalAlign="Left" />

<ItemStyle HorizontalAlign="Left" />
</asp:BoundField>
<asp:BoundField DataField="Source Document" HeaderText="Source Document">
<HeaderStyle HorizontalAlign="Left" />

<ItemStyle HorizontalAlign="Left" />
</asp:BoundField>
<asp:BoundField DataField="FormName" HeaderText="FormName">
<HeaderStyle HorizontalAlign="Left" />

<ItemStyle HorizontalAlign="Left" />
</asp:BoundField>
<asp:BoundField DataField="Inline Filter" HeaderText="Filter Fields">
<HeaderStyle HorizontalAlign="Left" />

<ItemStyle HorizontalAlign="Left" />
</asp:BoundField>
<asp:TemplateField HeaderText="Action"><ItemTemplate>
<asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete" Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitinlinefilter" ToolTip="Delete Inline Configuration" Width="16px" />
</ItemTemplate>
</asp:TemplateField>
</Columns>

<EditRowStyle CssClass="EditRowStyle" />

<FooterStyle CssClass="FooterStyle" />

<HeaderStyle BackColor="CornflowerBlue" CssClass=" HeaderStyle" Font-Bold="True" ForeColor="White" />

<PagerStyle CssClass="PagerStyle" />

<RowStyle BackColor="#EFF3FB" CssClass="RowStyle" />

<SelectedRowStyle CssClass="SelectedRowStyle" />

<sortedascendingcellstyle backcolor="#EDF6F6" />

<sortedascendingheaderstyle backcolor="#0D4AC4" />

<sorteddescendingcellstyle backcolor="#D6DFDF" />

<sorteddescendingheaderstyle backcolor="#002876" />
</asp:GridView>



</td></tr></table></asp:Panel>



</ContentTemplate>
</asp:TabPanel>
                </asp:TabContainer>



</div>




</contenttemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopupDelFolder" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder"
        CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 480px">
                        <h3>Delete Inline Editing Confiuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelFolder"
                            ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
                            <contenttemplate>
                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                    <tr>
                                        <td colspan="2" align="center" >
                                            <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete !!!!" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>
                                </table>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete"
                                        OnClick="DelFile" CssClass="btnNew" Font-Bold="True"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="Button2" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender2" runat="server"
        TargetControlID="Button2" PopupControlID="pnldeletefilter"
        CancelControlID="ImageButton3" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnldeletefilter" runat="server" Width="500px" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 480px">
                        <h3>Delete Inline Editing Confiuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton3"
                            ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanel5" runat="server" UpdateMode="Conditional">
                            <contenttemplate>
                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                    <tr>
                                        <td colspan="2" align="center" >
                                            <asp:Label ID="Label7" runat="server" Text="Are you sure want to delete !!!!" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>
                                </table>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="Button3" runat="server" Text="Yes Delete"
                                        OnClick="DelFilefilter" CssClass="btnNew" Font-Bold="True"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </contenttemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
       
    <asp:Button id="btnShowPopupuKEY" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtenderuKEY" runat="server" PopupControlID="pnlPopupuKEY" TargetControlID="btnShowPopupuKEY"
        CancelControlID="btnCloseuKEY" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupuKEY" runat="server" Width="1000px" Height="300px" BackColor="White" style="">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td width="980px">
                        <h3>Unique Key & Sorting Configuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseuKEY" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="Div1">
                            <asp:UpdatePanel ID="Updpanelukey" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
         <%--<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.js" type="text/javascript"></script>--%>
            <%--<script src="http://ajax.microsoft.com/ajax/beta/0911/Start.debug.js" type="text/javascript"></script>--%>
            <script src="http://ajax.microsoft.com/ajax/beta/0911/extended/ExtendedControls.debug.js" type="text/javascript"></script>

                                    <%--Change By Mayank  tab is not showing--%>
                                    <style type="text/css">
                                      
                                    </style>

           <%-- <style type="text/css">
                .fancy-green .ajax__tab_hader {
                    kground url(ima i cursor: pointer;
                }

                .fancy-gr en .ajax__tab_ho e ancy-green ajax__tab_active .ajax__tab_ouer {
                    t_Tab .gif) n -repeat left top;
                }

                .fancy-gr en .ajax__tab_ho e ancy-green ajax__tab_active .ajax__tab_innr {
                    r _Tab.g f) no-repeat righ;
                }

                _header {
                    : 13px weight: bold # font family: sans-seri;
                }

                .fancy .ajax__ta _activ .ajax__tab_oute, .fancy .ajax__ a .fancy .ajax _ height: 46px;
                }

                .fancy .ajax__ta _activ .ajax__tab_inne, .fancy .ajax__ a .fancy .ajax er {
                    height: 46px;
                    h of t e left image */;
                }

                .fancy .aja __tab_ ctive .ajax__tab_ ab, .fancy .aj x, .fanc .ajx__tb_hader margin 16px 16px 0px 0 x;
                }

                .fancy .aj x _tab, fancy colo :
                }

                body {
                    y: Arial;
                    size: 1 pt;
                    lid #999999;
                    p b ff;
                }

                {
                    or: whi e;
                    9ae46;
                    na 10px;
                    pdding: 1px 4px;
                    y Arial, Helvet c;
                }

                eader {
                    ;
                    background-color: Green;
                    kground: url( mage peat-x;
                    : 2p;
                    position: relative;
                    t tNode.parentNode.scrollTop-1);
                    terSha ows height: 100% width: 99%;
                    padding: 5px;
                    background-color: w (note the rgba is red gren, lue, alha *
   it-box-shadow: 0 x  0x  1px gba(0, 0 0, 0.4;
                    
                                       -mo 6px rgba(23, 69, 88, . );  /*
                                       webk
                    ;
                  -moz-border-            border-radius: 7px; /* gadiets *
                  backgound:  -ebkit-gradient(inear, eft top, left botom, color
                    r-stop(15%, white), color-stop(100%, D7E9F));
                 back
                    adient(top, white 0%, white 55%,  #D54F3 130);
                    
                  background -ms-linear-radient(top, #e1ffff 0%,#dffff  0%,#e1fff  100%,#c8efb  100%,#c8efb  100%,#be4f8  100%,#be4f8  100%,#be4f8  10 %,#bee4f8 1
                    f5 100%,#e1 fff 100%,#e6f8fd  10%); /* IE10+ *
                    
                                      background: inear-gradiet(to bottom, #e1ffff 0%,#dffff  0%,#e1fff  100%,#c8efb  100%,#c8efb  100%,#be4f8  100%,#be4f8  100%,#be4f8  10 %,#bee4f8
                    d8f5 10 %,#e1ffff 100%,#e6f8fd 100%); /* W3C */
                   filter: progid:DXImageTransform.Microsoft.grdi nt( startCo
                e

                ,Gradie t
                    
               %;
              .dyn {  x

                                        ynamicMen  {
     n

                               position: lute;
                };;;;;;;;
            </style>--%>

<div class="form" style=" text-align: left ; "> 
    


<asp:TabContainer ID="TabContainer1" Width="100%"  runat="server" CssClass="fancy fancy-green" ActiveTabIndex="0">
                    <asp:TabPanel ID="tbpnluser"  runat="server"><HeaderTemplate>Unique Key</HeaderTemplate><ContentTemplate><asp:Panel ID="UserReg" runat="server" ScrollBars="Vertical"><br /><table cellspacing="4px" cellpadding="0px" width="100%" border="0"><tr><td colspan="2" align="left"><asp:Label ID="lblmsgukey" runat="server" Font-Bold="True" ForeColor="Red"
                                                            Width="100%" Font-Size="X-Small"></asp:Label></td></tr><tr><td style="width: 125px" align="left"><b>Form Name </b></td><td align="left"><asp:Label ID="lblfn" runat="server"></asp:Label></td></tr><tr><td style="width: 125px" align="left"><b>Unique Keys</b></td><td style="width: 300px" align="left"><asp:TextBox ID="txtuk" Width="500px" runat="server" ReadOnly="true"></asp:TextBox></td></tr><tr><td style="width: 125px" align="left"><b>Excluding Characters</b></td><td><asp:TextBox ID="txtexchars" Width="100px" runat="server" ></asp:TextBox></td></tr><tr align="left"><td style="width: 125px" align="left"><b>Keys</b></td><td><div style="width: 100%; height: 150px; overflow: scroll;"><asp:CheckBoxList ID="chkflds" runat="server" AutoPostBack="true"></asp:CheckBoxList></div></td></tr><tr><td colspan="2" align="right" ><asp:Button ID="btnActEditukey" OnClick="editrecordukey" runat="server" Text="Update"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" /></td></tr></table></asp:Panel></ContentTemplate></asp:TabPanel>
                    <asp:TabPanel ID="tbpnlusrdetails" runat="server"><HeaderTemplate>Sorting</HeaderTemplate><ContentTemplate><asp:Panel ID="pnlsorting" runat="server" ScrollBars="Vertical"><br /><table cellspacing="4px" cellpadding="0px" width="100%" border="0"><tr><td colspan="2" align="left"><asp:Label ID="lblmsgSorting" runat="server" Font-Bold="True" ForeColor="Red"
                                                            Width="100%" Font-Size="X-Small"></asp:Label></td></tr><tr><td style="width: 125px" align="left"><b>Form Name </b></td><td align="left"><asp:Label ID="lblfnSorting" runat="server"></asp:Label></td></tr><tr><td style="width: 125px" align="left"><b>Sorting Fields</b></td><td style="width: 300px" align="left"><asp:TextBox ID="txtSorting" Width="500px" runat="server" ReadOnly="true"></asp:TextBox></td></tr><tr align="left"><td style="width: 125px" align="left"><b>Fields</b></td><td><div style="width: 100%; height: 150px; overflow: scroll;"><asp:CheckBoxList ID="chkfldsorting" runat="server" AutoPostBack="true"></asp:CheckBoxList></div></td></tr><tr><td colspan="2" align="right" ><asp:Button ID="btnActEditSorting" OnClick="EditRecordsortingfields" runat="server" Text="Update"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" /></td></tr></table></asp:Panel></ContentTemplate></asp:TabPanel>
<%--Add Primary key field by Mayank --%>
    <asp:TabPanel ID="tbpnlprimarykeydetails" runat="server">
        <HeaderTemplate>
            Primary Key
        </HeaderTemplate>
        <ContentTemplate>
            <asp:Panel ID="pnlPrimaryKey" runat="server" ScrollBars="Vertical"><br /> <table cellspacing="4" cellpadding="0" width="100%" border="0"><tr><td colspan="2" align="left"><asp:Label ID="lblmsgPrimaryKey" runat="server" Font-Bold="True" ForeColor="Red"
                                                            Width="100%" Font-Size="X-Small"></asp:Label></td></tr><tr><td style="width: 125px" align="left"><b>Form Name </b></td><td align="left"><asp:Label ID="lblfnPrimary" runat="server"></asp:Label></td></tr><tr><td style="width: 125px" align="left"><b>Primary Fields</b></td><td style="width: 300px" align="left"><asp:TextBox ID="txtPrimaryKey" Width="500px" runat="server" ReadOnly="true"></asp:TextBox></td></tr><tr align="left"><td style="width: 125px" align="left"><b>Fields</b></td><td><div style="width: 100%; height: 150px; overflow: scroll;"><asp:CheckBox ID="chkDocID" OnCheckedChanged="chkDocID_CheckedChanged" AutoPostBack="true" Text="&nbsp;DocID" runat="server" /><asp:CheckBoxList ID="chkfldPrimarykey" runat="server" AutoPostBack="true"></asp:CheckBoxList></div></td></tr><tr><td colspan="2" align="right" ><asp:Button ID="btnActEditPrimarykey" OnClick="EditRecordsPrimarykeyfields" runat="server" Text="Update"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" /></td></tr> </table></asp:Panel>
        </ContentTemplate>
    </asp:TabPanel>                
</asp:TabContainer>



</div>


</contenttemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <%--Add Tally Config--%>
    <%# CType(Container, GridViewRow).RowIndex + 1%>
    <asp:Button ID="btnxmlio" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupxmlinwardoutward" runat="server" PopupControlID="pnlPopupxmlinwardoutward" TargetControlID="btnxmlio"
        CancelControlID="btnclosexmlio" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupxmlinwardoutward" runat="server" Width="1000px" Height="300px" BackColor="White" Style="">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td width="980px">
                        <h3>XML InWard & OutWard Configuration </h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnclosexmlio" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="Div3">
                            <asp:UpdatePanel ID="updatepanelxmlinwardoutward" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
                                    <%--<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.js" type="text/javascript"></script>--%>
                                    <%--<script src="http://ajax.microsoft.com/ajax/beta/0911/Start.debug.js" type="text/javascript"></script>--%>
                                    <script src="http://ajax.microsoft.com/ajax/beta/0911/extended/ExtendedControls.debug.js" type="text/javascript"></script>

                                   

                                    <div class="form" style="text-align: left;">



                                        <asp:TabContainer ID="TabContainer3"  Width="100%" runat="server" CssClass="fancy fancy-green" ActiveTabIndex="0">
                                            <asp:TabPanel ID="TabPanel3" runat="server">
                                                <HeaderTemplate>
XML InWard 
</HeaderTemplate>
<ContentTemplate>
                                                    <asp:Panel ID="Panel5" runat="server" ScrollBars="Vertical">
                                                          <asp:label ID="lblheaderxmltagdoc" runat="server"></asp:label>












                                                        <br />
                                                         <br />
                                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                                           
                                                            
                                                            <tr>
                                                                <td colspan="4" align="left">
                                                                    <asp:Label ID="lblxmlinwardmsg" runat="server" Font-Bold="True" ForeColor="Red"
                                                                        Width="100%" Font-Size="X-Small"></asp:Label>














</td>

                                                            </tr>
                                                            <tr style="line-height:20px;" runat="server" id="rowxmlhead"><td align="left" style="width:150px;" runat="server">
                                                                    <b>DocType Tags </b>
                                                                </td>
<td align="left" style="width:150px;" runat="server">
                                                                    <asp:TextBox ID="txtxmlinward" runat="server"></asp:TextBox>













                                                                </td>
<td style="width: 125px" align="left" runat="server">
                                                                    <b>EntityCode Tags </b>
                                                                </td>
<td align="left" runat="server">
                                                                    <asp:TextBox ID="txtxmlentitycode" runat="server"></asp:TextBox>












                                                                    <br />
                                                                </td>
<td style="width: 125px" align="left" runat="server">
                                                                    <b>TallyCancelXMLTag </b>
                                                                    </td>
<td runat="server">
                                                                    <asp:TextBox ID="txtTallyCancelXMLTag" runat="server"></asp:TextBox>











                                                                </td>
</tr>












<tr align="left">
                                                                
                                                                <td align="left" colspan="1" class="auto-style2"><b>Fields</b></td>
                                                                <td colspan="3" style="width:300px;">
                                                                    <div style="width: 100%; height: 150px; width:465px; overflow: scroll;">
                                                                        <asp:GridView ID="gvdata_xmlinward" Width="508px" runat="server"
                                                                            AutoGenerateColumns="False">
<AlternatingRowStyle BackColor="#FFD4BA" />
<Columns>
<asp:TemplateField HeaderText="Ischeck"><ItemTemplate>
                                                                                        <asp:CheckBox ID="chkxmlinward" runat="server" />
                                                                                    
</ItemTemplate>

<HeaderStyle Width="50px" />

<ItemStyle Width="50px" />
</asp:TemplateField>
<asp:BoundField DataField="Displayname" HeaderText="Display Name">
<HeaderStyle Width="250px" />

<ItemStyle Width="150px" />
</asp:BoundField>
<asp:TemplateField HeaderText="InWard XmlTag"><ItemTemplate>
                                                                                        <asp:TextBox ID="txtxmlinwardtags" runat="server" CssClass="txtBox" Width="150px" />
                                                                                    
</ItemTemplate>

<ItemStyle Width="150px" />
</asp:TemplateField>
</Columns>

<FooterStyle BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" />

<HeaderStyle BackColor="#DCDBDB" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" Font-Size="15px" Height="30px" />

<PagerStyle BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" />

<RowStyle Height="20px" Font-Size="13px" BorderColor="#CCCCCC" BorderStyle="Solid"
                                                                                BorderWidth="1px" />
</asp:GridView>











                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="4" align="right">
                                                                    <asp:Button ID="btnxmlinward" runat="server" Text="Update"
                                                                        CssClass="btnNew" Font-Bold="True"
                                                                        Font-Size="X-Small" Width="100px" />












</td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>











</ContentTemplate>
</asp:TabPanel>
                                            <asp:TabPanel ID="TabPanel4" runat="server">
                                                <HeaderTemplate>
XML OutWard 
</HeaderTemplate>
<ContentTemplate>
                                                    <asp:Panel ID="Panel4" runat="server" ScrollBars="Vertical">
                                                          <asp:label ID="lblxmloutwardhead" runat="server"></asp:label>
                                                        <br />
                                                         <br />
                                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                                           
                                                            
                                                            <tr>
                                                                <td colspan="4" align="left">
                                                                    <asp:Label ID="lblxmloutwardmsg" runat="server" Font-Bold="True" ForeColor="Red"
                                                                        Width="100%" Font-Size="X-Small"></asp:Label></td>

                                                            </tr>
                                                            <tr style="line-height:20px;" runat="server" id="rowxmloutward">
                                                                <td id="Td6" align="left" style="width:150px;" runat="server">
                                                                    <b>DocType Tags </b>
                                                                </td>
                                                                <td id="Td7" align="left" style="width:150px;" runat="server">
                                                                    <asp:TextBox ID="txtxmloutwarddoctype" runat="server"></asp:TextBox>

                                                                </td>
                                                                <td id="Td8" style="width: 125px" align="left" runat="server">
                                                                    <b>EntityCode Tags </b>
                                                                </td>
                                                                <td id="Td9" align="left" runat="server">
                                                                    <asp:TextBox ID="txtxmloutwardentitycode" runat="server"></asp:TextBox>
                                                                    <br />
                                                                </td>
                                                                <td id="Td10" align="left" style="width:150px;" runat="server">
                                                                    <b>RowFilter XML Tags </b>
                                                                </td>
                                                                <td id="Td11" align="left" style="width:150px;" runat="server">
                                                                    <asp:TextBox ID="txtrowfilterxmltag" runat="server"></asp:TextBox>

                                                                </td>
                                                       <td id="Td12" align="left" style="width:150px;" runat="server">
                                                                    <b>RowFilter BPM Field </b>
                                                                </td>
                                                                <td id="Td13" align="left" style="width:150px;" runat="server">
                                                                    <asp:TextBox ID="txtrowfilterbpmfield" runat="server"></asp:TextBox>

                                                                </td>
                                                                </tr>
                                                            <tr runat="server" id="rowchild" visible="False" >
                                                                <td runat="server">
                                                                        <b>Child Master Field </b>
                                                                </td>
                                                                <td runat="server">
                                                                    <asp:DropDownList ID="ddlchildmasterfield" runat="server" CssClass="txtBox" ></asp:DropDownList>
                                                                                                                                    </td>
                                                            </tr>
                                                          
                                                            <tr align="left">
                                                                
                                                                <td align="left" colspan="1" class="auto-style2"><b>Fields</b></td>
                                                                <td colspan="7" style="width:300px;">
                                                                    <div style="width: 100%; height: 150px; overflow: scroll;">
                                                                        <asp:GridView ID="gvxmloutward" Width="508px" runat="server"
                                                                            AutoGenerateColumns="False">
                                                                            <AlternatingRowStyle BackColor="#FFD4BA" />
                                                                            <FooterStyle BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" />
                                                                            <PagerStyle BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" />
                                                                            <HeaderStyle Height="30px" BackColor="#DCDBDB" Font-Size="15px" BorderColor="#CCCCCC"
                                                                                BorderStyle="Solid" BorderWidth="1px" />
                                                                            <RowStyle Height="20px" Font-Size="13px" BorderColor="#CCCCCC" BorderStyle="Solid"
                                                                                BorderWidth="1px" />
                                                                            <Columns>
                                                                                <asp:TemplateField HeaderText="Ischeck">
                                                                                                                                                                        <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkxmloutward" runat="server" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="50px" />
                                                                                    <ItemStyle Width="50px" />
                                                                                </asp:TemplateField>
                                                                                <asp:BoundField DataField="Displayname" HeaderText="Display Name">


                                                                                    <HeaderStyle Width="250px" />
                                                                                    <ItemStyle Width="150px" />
                                                                                </asp:BoundField>


                                                                                <asp:TemplateField HeaderText="OutWard XmlTag">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="txtxmlOutwardtags" CssClass="txtBox"   runat="server" Width="150px" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="150px" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="RowFilter BPM">
                                                                                                                                                                        <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkxmloutwardRowFilterBPM" runat="server" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="50px" />
                                                                                    <ItemStyle Width="50px" />
                                                                                </asp:TemplateField>
                                                                            </Columns>

                                                                        </asp:GridView>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="8" align="right">
                                                                    <asp:Button ID="btnxmloutwardsave" runat="server" Text="Update"
                                                                        CssClass="btnNew" Font-Bold="True"
                                                                        Font-Size="X-Small" Width="100px" /></td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                
</ContentTemplate>
</asp:TabPanel>

                                            <asp:TabPanel ID="TabPanel5" runat="server">
                                                <HeaderTemplate>
                                                    XML Registration
                                                
</HeaderTemplate>
                                                










<ContentTemplate>
                                                    <asp:Panel ID="Panel6" runat="server" ScrollBars="Vertical">
                                                        <asp:Label ID="lbldispxmlregfield" ForeColor="Red"  runat="server"></asp:Label>
                                                        <br />
                                                        <br />
                                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0">

                                                            <tr>
                                                                <td><b>TallY XML Registration Field</b></td>
                                                                <td><asp:TextBox ID="txttallyxmlregfield" runat="server" ></asp:TextBox></td>
                                                            </tr>

                                                            <tr>
                                                                <td colspan="8" align="right">
                                                                    <asp:Button ID="btnsavexmltallyregfield" runat="server" Text="Update"
                                                                        CssClass="btnNew" Font-Bold="True"
                                                                        Font-Size="X-Small" Width="100px" /></td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>

                                                
</ContentTemplate>
                                            










</asp:TabPanel>
                                        </asp:TabContainer>



                                    </div>


                                </contenttemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <%--Variance Field Added By Mayank--%>
    <asp:Button id="btnvld" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server"
        TargetControlID="btnvld" PopupControlID="pnlvalidation" CancelControlID="btnclose"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlvalidation" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="uplvalidation" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="validationFname" runat="server"></asp:Label> Validation Fields</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnclose" ImageUrl="images/close.png" runat="server" OnClick="btnvclose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
        <tr>
     <td align="left" colspan ="4"> <asp:Label ID="lblvalmsg" runat="server"  Text="" ForeColor="Red"></asp:Label></td>
     </tr> 
     
  <tr><td align="left"><asp:Label ID="llbvtype" runat="server"  Text="Validation Type:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left"  >
         <asp:dropdownlist ID="ddlvtype" runat="server" Width="150px" CssClass="txtBox" AutoPostBack="True">
             <asp:ListItem>Select One</asp:ListItem>
             <asp:ListItem>Static</asp:ListItem>
              <asp:ListItem>Field</asp:ListItem>
               <asp:ListItem>Mandatory</asp:ListItem>
                <asp:ListItem>Dynamic</asp:ListItem>
         <asp:ListItem>DuplicacyCheck</asp:ListItem>
             </asp:dropdownlist></td>
     <td align="left"><asp:Label ID="llbFName" runat="server"  Text="Field Name:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlfldname" runat="server" Width="150px"  CssClass="txtBox"></asp:dropdownlist></td>
     </tr>
     <div id="divopr" runat="server">
     <tr><td align="left"><asp:Label ID="lbloperator" runat="server"  Text="Operator:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddloperator" runat="server" Width="150px"  
             CssClass="txtBox" >
             <asp:ListItem>=</asp:ListItem>
             <asp:ListItem>></asp:ListItem>
                <asp:ListItem><</asp:ListItem>
                 <asp:ListItem><=</asp:ListItem>
                 <asp:ListItem>>=</asp:ListItem>
             </asp:dropdownlist><asp:dropdownlist ID="ddlfldopr" runat="server" Width="150px"  
             CssClass="txtBox" Visible="false"  ></asp:dropdownlist></td>
     <td align="left"><asp:Label ID="lblval" runat="server"  Text="Value:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlvalue" runat="server" Width="150px" 
     CssClass="txtBox" Visible="false" ></asp:dropdownlist><asp:TextBox ID="txtval" runat="server" CssClass="txtBox"></asp:TextBox>
     </td>
     </tr></div>
     <div id="dynamic" runat="server" visible="false">
     <tr><td align="left"><asp:Label ID="llbdtype" runat="server"  Text="Doc Type:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlDtype" runat="server" Width="150px" AutoPostBack="true"   
             CssClass="txtBox">
      </asp:dropdownlist>
     <td align="left"><asp:Label ID="llbtrfield" runat="server"  Text="Target Field:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left"  ><asp:dropdownlist ID="ddlTfield" runat="server" Width="150px" 
     CssClass="txtBox" ></asp:dropdownlist>
      <asp:dropdownlist ID="ddlopr" runat="server" Width="60px" 
     CssClass="txtBox" >
             <asp:ListItem>=</asp:ListItem>
             <asp:ListItem>></asp:ListItem>
             <asp:ListItem><</asp:ListItem>
             <asp:ListItem><=</asp:ListItem>
             <asp:ListItem>>=</asp:ListItem>
     </asp:dropdownlist>
    <asp:dropdownlist ID="ddlvalfield" runat="server" Width="150px" 
     CssClass="txtBox" ></asp:dropdownlist>
      <asp:Button id="btnAddfld1" runat="server" Text="Add" CssClass="btnNew" />
     </td>
     </tr>
     </div>
      <tr>
          <td align ="left" >Work Status:</td>
           <td colspan="2" align="center" style="text-align:center" >
               <div style="overflow:scroll;height :100px; width:150px; text-align:left" >
                   <asp:CheckBoxList ID="chkWFStatus" runat ="server" RepeatDirection ="Vertical" ></asp:CheckBoxList>
               </div>
           </td>
     <td  align="center" style="text-align:center" >
     <asp:TextBox runat="server" TextMode="MultiLine" CssClass="txtBox" 
    Height="60px" Width="230px" ID="txtDispFilter" Visible="false"></asp:TextBox>
     </td>
      </tr>
    <tr><td align="left"><asp:Label ID="llberrmsg" runat="server"  Text="Error Message:" Font-Size="Smaller"  Font-Bold="true" ></asp:Label></td>
     <td align ="left" ><asp:TextBox ID="txterrmsg" runat="server" CssClass="txtBox" Width="140px"></asp:TextBox>
     </td>
    
     <td align ="left" >&nbsp;</td>
     </tr>
    <tr>
    <td align="left"><label>Doc Nature</label></td>
    <td align="left">
    <asp:DropDownList ID="docnature" runat="server">
     <asp:ListItem>CREATE</asp:ListItem>
     <asp:ListItem>MODIFY</asp:ListItem>
     <asp:ListItem>BOTH</asp:ListItem>
    
     <%--<asp:ListItem>Amendment</asp:ListItem>--%>
     <%--<asp:ListItem>BOTH</asp:ListItem> --%>
     </asp:DropDownList></td>
         <td align ="left" ><asp:Button ID="btnvsave" runat="Server"  CssClass="btnNew" Text="Submit"  Width="80px"/></td>
    </tr>
    
 
 </table>
</div>
</td>
</tr>
<tr>
<td>
<asp:Panel ID="Panel1" runat="server"  ScrollBars="Auto" Width="100%">
    <asp:GridView ID="grdvalidation" runat="server" AllowSorting="True"   AutoGenerateColumns="False" CellPadding="2" DataKeyNames="tid"  ForeColor="#333333" Width="100%">
                            <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                     <asp:TemplateField HeaderText="S.No">
                                     <ItemTemplate>
                                     <%# Container.DataItemIndex + 1%>
                                     </ItemTemplate>
                                     <ItemStyle Width="50px" /></asp:TemplateField>
                                     <asp:BoundField DataField="doctype" HeaderText="Document Type">
                                     <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="Valtype" HeaderText="Validation Type">
                                     <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="fldid" HeaderText="Field ID">
                                     <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="Operator" HeaderText="Operator">
                                     <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="value" HeaderText="Value">
                                     <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField DataField="Err_msg" HeaderText="Error Msg">
                                     <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:TemplateField HeaderText="Action">
                                     <ItemTemplate>
                                     <%--<asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Detail"   Height="16px" ImageUrl="~/images/edit.jpg"    ToolTip="Edit Detail" Width="16px" />--%>
                                     <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete"  Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHit" OnClientClick = "Confirm()"  ToolTip="Delete" Width="16px" />
                                            
                                                     </ItemTemplate>
                                                     <ItemStyle HorizontalAlign="Center" Width="80px" />
                                                     </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#2461BF" />
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="CornflowerBlue" ForeColor="White"   HorizontalAlign="Center" />
                            <RowStyle BackColor="#EFF3FB" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>
</asp:Panel>
</td>
</tr>
</table>
</contenttemplate>

            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <%--Advance Formula --%>
    <asp:Button id="btnComProp" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="MP_CommonProp" runat="server"
        TargetControlID="btnComProp" PopupControlID="pnlComProp" CancelControlID="btnCancelComProp"
        BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlComProp" runat="server" Width="600px" Height="500px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updCommonProp" runat="server" UpdateMode="Conditional">
                <contenttemplate>
                 <table cellspacing="2px" cellpadding="2px" width="100%">
                 <tr><td><asp:Label ID="lblMessComProp" runat="server"  ForeColor="Red"></asp:Label> </td></tr>
                    <tr>
                        <td style="width:580px"><h3>Properties of Field <asp:Label ID="lblControl" runat="server" ></asp:Label> </h3></td>
                        <td style="width:20px"><asp:ImageButton ID="btnCancelComProp" ImageUrl="images/close.png" runat="server" OnClick="closeProperty"/></td>
                    </tr>
                     <tr>
                        <td colspan="2">
                            <div class="form">
                                 <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td style="text-align:left;" colspan ="2"> <asp:Label ID="Label6" runat="server"  ></asp:Label></td>
                                     </tr> 
                                    <tr>
                                        <td width="320px">
                                            <label for="ContentPlaceHolder1_chkCMan"> MANDATORY </label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCMan" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkAActive"> ACTIVE </label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chkAActive" runat="server" />
                                         </td>
                                    </tr>
                                    <tr>
                                         <td><label for="ContentPlaceHolder1_chkCEdit"> EDITABLE </label></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCEdit" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkCWork"> WORK FLOW </label></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCWork" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkCUni"> UNIQUE </label></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCUni" runat="server" />
                                        </td>
                                     </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkShowAmen"> SHOW ON AMENDMENT </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkShowAmen" runat="server" />
                                        </td>
                                    </tr> 
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkSearch"> SEARCH </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkSearch" runat="server"/> 
                                        </td>
                                    </tr> 
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkEditonAmendment"> EDIT ON AMENDMENT </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkEditonAmendment" runat="server"/> 
                                        </td>
                                    </tr> 
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkImieNo"> IMIE No. </label></td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkImieNo" runat="server"/> 
                                         </td>
                                     </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkPhoneNo">  PHONE No. </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkPhoneNo" runat="server"/> 
                                        </td>
                                     </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_chkIsmail">  MAIL </label></td>
                                     <td align="left" style="width:50%">
                                        <asp:CheckBox ID="chkIsmail" runat="server"/>   
                                      </td> </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_chkIsmail">  WorkFlow Status </label></td>
                                        <td> <div style="width: 80%; height: 60px; overflow: scroll;">
                                             
                                             <asp:TextBox ID="txtmail" runat="server" Text="Please Select" Visible="false"></asp:TextBox>
                                             <asp:CheckBoxList ID="chkddlismail" runat="server" AutoPostBack="true">
                                             </asp:CheckBoxList>
                                         </div>
                                      </td>
                                 </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkInLinEdt">  IN LINE EDITING </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkInLinEdt" runat="server"/> 
                                        </td>
                                     </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkDocDetail">  SHOW ON DOC DETAIL </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkDocDetail" runat="server"/> 
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkinvisible">  Invisible </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkinvisible" runat="server" Checked="false" /> 
                                        </td>
                                     </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_chksplttable">  SPLITTABLE </label></td>
                                           <td align="left" colspan="3">
                                         <asp:CheckBox ID="chksplttable" runat="server" AutoPostBack="true" />
                                         <asp:DropDownList ID="ddlSpltOprtr" runat="server" Visible="false">                                         
                                             <asp:ListItem Text="Please Select"></asp:ListItem>
                                             <asp:ListItem Text="="></asp:ListItem>
                                             <asp:ListItem Text="<"></asp:ListItem>
                                             <asp:ListItem Text=">"></asp:ListItem>
                                             <asp:ListItem Text="<="></asp:ListItem>
                                             <asp:ListItem Text=">="></asp:ListItem>
                                             <asp:ListItem Text="<>"></asp:ListItem>
                                         </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlspltTable" runat="server" Visible="false">                                         
                                         </asp:DropDownList>
                                         </td>                                         
                                    </tr>

                                    <tr>
                                       <td><label for="ContentPlaceHolder1_chkDocSplit">  SHOW ON SPLIT </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkDocSplit" runat="server"/> 
                                        </td>
                                       
                                    </tr>

                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkIssupervisor">  IS SUPERVISOR </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkIsSupervisor" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                      <tr>
                                        <td><label for="ContentPlaceHolder1_chkenableEdit">  EnableEdit </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkenableEdit" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                      <tr>
                                        <td><label for="ContentPlaceHolder1_chkpriority">  Priority Decider </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkpriority" runat="server"/> Urgent, High, Top, Critical
                                        </td> 
<%--                                          <td><label></label></td>    --%>
                                    </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_chkenableEdit">  CRM Reminder </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkcrmreminder" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                       <tr>
                                        <td><label for="ContentPlaceHolder1_deshbord"> Show On Deshboard </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkdeshboard" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_ChkShwCRM"> Show On CRM </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="ChkShwCRM" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                    <tr>
                                        <td><label for="ContentPlaceHolder1_chkEditCrm"> EDIT On CRM </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkEditCrm" runat="server"/> 
                                        </td>                                        
                                    </tr> 
                                      <tr>
                                        <td><label for="ContentPlaceHolder1_chkEditistotal">  Is Total </label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkeditistotal" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_chkEditistotal">Allow Edit On Edit</label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkalloweditonedit" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                     <tr>
                                        <td><label for="ContentPlaceHolder1_chkEditextrnallukup">External Lookup For Mobile</label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chkextrnllukupformobile" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                         <tr>
                                        <td><label for="ContentPlaceHolder1_chkEditextrnallukup">Show In Role Assignment</label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chk_assignment" runat="server"/> 
                                        </td>                                        
                                    </tr>
                                     <%--Added By mayank for Is Card No Property 11-Feb-2015 --%>
                                     <tr>
                                         <td><label for="ContentPlaceHolder1_chkEditCardNo">Is Card No</label></td>
                                        <td align="left">
                                        <asp:CheckBox ID="chk_CardNO" runat="server"/> 
                                        </td>                                        
                                     </tr>
                                     <tr id="fillonedit" runat="server" visible="false">
                                         <td> <label for="ContentPlaceHolder1_fillwithexistingonEdit">Fill Dropdown Existing on Edit</label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chk_fillwithexistingonEdit" runat="server" />
                                         </td>
                                     </tr>
                                     <tr id="IsGeoFenceFilter" runat="server" visible="false">
                                         <td> <label for="ContentPlaceHolder1_IsGeoFenceFilter">Is Geo Fence Filter</label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chk_IsGeoFenceFilter" runat="server" />
                                             <asp:DropDownList ID="ddlGeoFenceMap" runat="server"></asp:DropDownList>
                                         </td>
                                     </tr>
                                      <tr id="Tr1" runat="server" >
                                         <td> <label for="ContentPlaceHolder1_ShowOnReallocation">Show On Reallocation</label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chk_Reallocation" runat="server" />
                                         </td>
                                     </tr>
                                      <%-- Start for GST PAN IFSC--%>
                                      <tr id="Tr2" runat="server" >
                                         <td> <label for="ContentPlaceHolder1_GST">GST</label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chk_GST" runat="server" />
                                             <asp:TextBox ID="txtGSTExcep" runat="server" placeholder="Enter Exceptional GST Values" Width="250px"></asp:TextBox>
                                         </td>
                                     </tr>
                                      <tr id="Tr3" runat="server" >
                                         <td> <label for="ContentPlaceHolder1_PAN">PAN</label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chk_PAN" runat="server"/>
                                             <asp:TextBox ID="txtPANExcep" runat="server" placeholder="Enter Exceptional PAN Values" Width="250px"></asp:TextBox>
                                         </td>
                                      </tr>
                                      <tr id="Tr4" runat="server" >
                                         <td> <label for="ContentPlaceHolder1_IFSC">IFSC</label></td>
                                         <td align="left">
                                             <asp:CheckBox ID="chk_IFSC" runat="server" />
                                             <asp:TextBox ID="txtIFSCExcep" runat="server" placeholder="Enter Exceptional IFSC Values" Width="250px"></asp:TextBox>
                                         </td>
                                      </tr>
                                     <%-- End for GST PAN IFSC--%>
                                     <tr>
                                         <td>
                                         <label for="ContentPlaceHolder1_ReportName">Report Name</label>
                                         </td>
                                         <td align="left">
                                             <asp:TextBox ID="txtReportName" runat="server" ></asp:TextBox>
                                         </td>
                                     </tr>
                                      <tr>
                                         <td>
                                         <label for="txtAllowDecimalDigit">Allow Decimal Digit</label>
                                         </td>
                                         <td align="left">
                                             <asp:TextBox ID="txtAllowDecimalDigit"  runat="server" value="0" maxlength="1" onkeypress="return IsNumeric(event);" ondrop="return false;" onpaste="return false;"></asp:TextBox>
                                             <span id="error" style="color: Red; display: none">* Input digits (0 - 9)</span>
   
                                         </td>
                                     </tr>
                                     <tr>
                                        <td>
                                        <label for="ddlEinvoiceList">Map QR Code Fields</label>
                                        </td>
                                        <td align="left">
                                              <asp:DropDownList ID="ddlEinvoiceFields" runat="server" Width="150px" Height="21">
                                            <asp:ListItem Value="">Please Select</asp:ListItem>
                                                 <asp:ListItem Value="SellerGstin">Seller GSTIN</asp:ListItem>
                                                <asp:ListItem Value="BuyerGstin">Buyer GSTIN</asp:ListItem>
                                                <asp:ListItem Value="IRN">IRN (Inv Ref No)</asp:ListItem>
                                                <asp:ListItem Value="IRNDate">IRN Date</asp:ListItem>
                                                 <asp:ListItem Value="DocType">DOC Type</asp:ListItem>
                                                 <asp:ListItem Value="DocNo">DOC No</asp:ListItem>
                                                <asp:ListItem Value="DocDate">DOC Date</asp:ListItem>
                                                <asp:ListItem Value="ItemCount">Item Count</asp:ListItem>
                                                <asp:ListItem Value="MainHsnCode">Main HSN Code</asp:ListItem>                                           
                                         </asp:DropDownList>
                                        </td>
                                    </tr>
                                     <tr id="RoundOffHead" runat="server" visible="false">
                                         <td>
                                              <label for="ContentPlaceHolder1_ReportName">Round Off Value</label>
                                         </td>
                                          <td align="left">
                                            <asp:CheckBox ID="IsRoundOff" runat="server" AutoPostBack="true" OnCheckedChanged="IsRoundOff_CheckedChanged" />
                                              <asp:TextBox ID="txtRoundOffVal" runat="server" Visible="false"></asp:TextBox>
                                         </td>
                                     </tr>
                                     <tr id="UserEanbled" runat="server" visible="false">
                                           <td>
                                              <label for="ContentPlaceHolder1_ReportName"> Enabled Usable Session</label>
                                         </td>
                                          <td align="left">
                                              <asp:CheckBox ID="IsEUS" runat="server" />
                                          </td>
                                     </tr>
                                     <%--Added By mayank for Is Card No Property 11-Feb-2015 --%>
                                     <tr> <td>
                                        <asp:Button ID="btnCommonproperties" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="SaveCommonProp"/>
                                        </td></tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <asp:Button id="btnFormula" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="MP_FormulaText" runat="server"
        TargetControlID="btnFormula" PopupControlID="pnlFormulaText"
        CancelControlID="closeFormulatext" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlFormulaText" runat="server" Width="900px" Height="330px" ScrollBars="Auto" style="display: " BackColor="white">
        <div class="box">

            <asp:UpdatePanel ID="UP_formulatxt" runat="server" UpdateMode="Conditional">
                <contenttemplate>
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:880px"><h3><asp:Label ID="FormulaF" runat="server"></asp:Label> Apply Formula Text Field</h3></td>
<td style="width:20px"><asp:ImageButton ID="closeFormulatext" ImageUrl="images/close.png" runat="server" OnClick="closeformula"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblFromula" runat="server"  Text="Please select the field" ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label>* Display Name : </label><asp:TextBox ID="txtFormulaName" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <%--<td  align="left"><label>* Select Document : </label>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlFormulaDoc" runat="server"  AutoPostBack="true" Width="150px">
     </asp:DropDownList> </td>--%>
    <td align="left"><label>* Data Types  : </label>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddldatatypeformula" runat="server" >
      <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList></td>
     </tr>

     <tr>
     <td align="left"><label>* Formula variable : </label><asp:TextBox ID="txtformulaVar" runat="server" CssClass="txtBox" TextMode="MultiLine"></asp:TextBox> </td>
     <td align="left"><table cellpadding="0" cellspacing="0"><tr><td align="left"><label>* Formula : </label><asp:TextBox ID="txtFormulaText" Height="50%" runat="server" CssClass="txtBox" TextMode="MultiLine"></asp:TextBox> </td></tr>
         <tr>
             <td align="left"><label>* Formula For Mob : </label><asp:TextBox ID="txtFormulaTextMob" Height="50%" runat="server" CssClass="txtBox" TextMode="MultiLine"></asp:TextBox> </td>
         </tr>
                      </table> 
     
     </tr><tr>
<td align="left"><asp:CheckBox ID="chkformulaActive" runat="server" Text="Is Active" /> <asp:CheckBox ID="chkformulaRunable" runat="server" Text="Runable" /></td>
<td> <asp:Button ID="btnFormulaSave" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>

</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%--<asp:Button id="btnDropDown" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="ModalPopUpDropDown" runat="server" 
                                TargetControlID="btnDropDown" PopupControlID="pnlDropDown" 
                  BackgroundCssClass="modalBackground"  CancelControlID="txtdropdowncancel"
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlDropDown" runat="server" Width="900px" Height ="330px" ScrollBars ="Auto"    BackColor="white">
<div class="box">

<asp:UpdatePanel ID="updDropdownSelected" runat="server"  UpdateMode="Conditional"  >
<ContentTemplate> 

<table cellspacing="2px" cellpadding="2px" width="100%">

<tr>
<td style="width:880px"><h3>Apply Fields</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtdropdowncancel" ImageUrl="images/close.png" OnClick ="Close" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">
    <table width="100%" border="0px">
        <tr>
     <td align="left" colspan="3"> <asp:Label ID="lbldropdown" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr><tr><td> </td></tr>

     <tr><td align="right"> <label>* Display Name  : </label></td> <td align="left" colspan="3"><asp:TextBox  ID="ddlDispalyName" runat="server"  CssClass="txtBox"></asp:TextBox> 
     &nbsp;&nbsp;<label>* Field Type    :  </label><asp:DropDownList ID="ddlDropdownList" runat="server" Width="150px" AutoPostBack="true">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>FIX VALUED</asp:ListItem>
     <asp:ListItem>MASTER VALUED</asp:ListItem>
     <asp:ListItem>SESSION VALUED</asp:ListItem>
     <asp:ListItem>CHILD</asp:ListItem>
     </asp:DropDownList> </td></tr><tr>
    <td align="right"> <asp:label ID="lblDdlvalue" runat="server" ><b>  Value : </b></asp:label></td> <td  align="left" colspan="4"> <asp:TextBox ID="txtdropDownValued" runat="server" CssClass="txtBox"  Width="400px">
    </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender7" runat ="server"    TargetControlID ="txtdropDownValued" WatermarkCssClass ="water"    WatermarkText ="Enter comma saperated Value"></asp:TextBoxWatermarkExtender>
    </td></tr>          
    
    <tr><td align="right"><asp:label ID="lblDdlSelectMaster" runat="server" ><b> Select Master : </b></asp:label></td><td align="left" colspan="3"><asp:DropDownList ID="ddlDropDownMasteValSelect" runat="server"  Width="150px" AutoPostBack="true" >
    </asp:DropDownList>
     <asp:DropDownList ID="ddlDropDownFieldSelect"  runat="server"  Width="150px" AutoPostBack="True">
     </asp:DropDownList></td> </tr>
      <tr>
          <td align ="right" style ="font-weight:bold" ><asp:Label ID="lblfilteron" runat ="server" Text="Fiter On:" Visible ="false"></asp:Label></td>
          <td colspan ="2" align ="left" >
              <asp:DropDownList ID="ddlFLT" runat ="server" Visible ="false"  Width ="200px" ></asp:DropDownList>
          </td>
      </tr>
<tr>     
 <td align="right"><label> Mandatory Field :  </label>
</td><td align="left" colspan="4"><asp:CheckBox ID="chkDdlMan" runat="server" Text="Field is Mandatory" /><asp:CheckBox ID="chkDlActive" runat="server" Text="Active" /><asp:CheckBox ID="chkDdlEditable" runat="server" Text="Editable" /><asp:CheckBox ID="chkDdlWF" runat="server" Text="Work Flow" /> </td>
<td> <asp:Button ID="btnddlSelect" runat="Server"  CssClass="btnNew"  Width="80px" Text="SAVE" OnClick="EditRecord"/> </td>
</tr>

</table>


</div>
</td>
</tr>
</table>
</ContentTemplate>


<Triggers>
<asp:AsyncPostBackTrigger ControlID="txtdropdowncancel" />
</Triggers>




</asp:UpdatePanel>

</div>
</asp:Panel>
    --%>
    <asp:Button id="btnsignature" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mdlpopup_signature" runat="server" TargetControlID="btnsignature" PopupControlID="pnlSignBoxSelect"
        CancelControlID="txtsigncancel" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlSignBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="UpSignature" runat="server" UpdateMode="Conditional">
                <contenttemplate>  
<table cellspacing="2px" cellpadding="2px" width="100%"> 
<tr>
<td style="width:880px"><h3><asp:Label ID="Signature" runat="server"></asp:Label>Apply Fields SIGNATURE</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtsigncancel" ImageUrl="images/close.png" runat="server" OnClick="txtSignClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="box">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblsign" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtdisplysign" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtdescsign" runat="server" CssClass="txtBox"></asp:TextBox><label> Default Value :</label><asp:TextBox ID="txtDefValsign" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr><tr>
     
     <td  align="left" ><label>* Data Types :  </label> &nbsp;<asp:DropDownList ID="ddldatatypesign" runat="server" >
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList> </td>
     
       <td align="left"> <label>Field Length :</label>  <asp:TextBox ID="txtfieldsignmin" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender10" runat ="server"    TargetControlID ="txtfieldsignmin" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
             <asp:TextBox ID="txtfieldsignmax" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
             <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender11" runat ="server"    TargetControlID ="txtfieldsignmax" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>   </tr>
<tr>
<td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="chkfieldsignman" runat="server" Text="Mandatory" /> <asp:CheckBox ID="chkfieldsignact" runat="server" Text="Active" /> <asp:CheckBox ID="chkfieldsignedit" runat="server" Text="Editable" /> <asp:CheckBox ID="chkfieldsignwf" runat="server" Text="Work Flow" /> <asp:CheckBox ID="chkfieldsignuniq" runat="server" Text="Unique" /> <asp:CheckBox ID="chkfieldsigndes" runat="server" Text="Description" /> </td>
<td><asp:Button ID="btnsignsave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

    <%--For drop Down--%><%--<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.js" type="text/javascript"></script>--%>
    <asp:Button id="btngeo" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mdl_geo" runat="server" TargetControlID="btngeo" PopupControlID="pnlgeoBoxSelect"
        CancelControlID="txtgeocancel" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlgeoBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Upgeo" runat="server" UpdateMode="Conditional">
                <contenttemplate>  
<table cellspacing="2px" cellpadding="2px" width="100%"> 
<tr>
<td style="width:880px"><h3><asp:Label ID="Geo" runat="server"></asp:Label>Apply Fields GEO POINT</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtgeocancel" ImageUrl="images/close.png" runat="server" OnClick="txtGeoClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="box">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblgeo" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtgeodisplay" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtgeodes" runat="server" CssClass="txtBox"></asp:TextBox><label> Default Value :</label><asp:TextBox ID="txtdefgeo" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr><tr>
     
     <td  align="left" ><label>* Data Types :  </label> &nbsp;<asp:DropDownList ID="ddltypegeo" runat="server" >
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList> </td>

   
     
       <td align="left"> <label>Field Length :</label>  <asp:TextBox ID="txtfgeomin" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender14" runat ="server"    TargetControlID ="txtfgeomin" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
             <asp:TextBox ID="txtfgeomax" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
             <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender15" runat ="server"    TargetControlID ="txtfgeomax" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>   </tr>

        <tr valign="top">  <td  align="left" ><label>* Geo Point Type :  </label> &nbsp;<asp:DropDownList ID="ddlgeo" runat="server" AutoPostBack="true">
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Auto</asp:ListItem>
     <asp:ListItem>Manual</asp:ListItem> 
     </asp:DropDownList> </td>
      <td align="left">
          <table cellpadding="0" cellspacing="0">
              <tr>
                  <td>
                         <table cellpadding="0" cellspacing="0" border="1"> 
               <tr id="autochk" runat="server"  >
                   <td align="left"><label>Geo Point Source :</label> 
       <asp:Panel ID="pnllist" runat="server" ScrollBars="Auto" Height="200px">
           <asp:CheckBoxList ID="chkautolist" runat="server" RepeatColumns="2" RepeatDirection="Vertical"></asp:CheckBoxList></asp:Panel> 
                  </td>

               </tr></table>
                  </td>
                  <td style="padding-left:2px;">
                       <table cellpadding="0" cellspacing="0" border="1">
               <tr>
                  <td align="left"><label>Fields For MAP:</label><asp:Panel ID="pnlmaplist" runat="server" Height="200px" ScrollBars="Auto"><asp:CheckBoxList ID="ChkMapList" runat="server" RepeatColumns="2" RepeatDirection="Vertical"></asp:CheckBoxList></asp:Panel></td>
               </tr>
           </table>
                  </td>
              </tr>
          </table>
        
          
                                                                                                                                                                                                                                     </td></tr>
       
 <tr>
<td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="chkgeoman" runat="server" Text="Mandatory" /> <asp:CheckBox ID="chkgeoact" runat="server" Text="Active" /> <asp:CheckBox ID="chkgeoedit" runat="server" Text="Editable" /> <asp:CheckBox ID="chkgeoWF" runat="server" Text="Work Flow" /> <asp:CheckBox ID="chkgeouni" runat="server" Text="Unique" /> <asp:CheckBox ID="chkgeodes" runat="server" Text="Description" /> </td>
<td><asp:Button ID="btngeosave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

     <%--<script src="http://ajax.microsoft.com/ajax/beta/0911/Start.debug.js" type="text/javascript"></script>--%>
    <asp:Button ID="btnchkbox" runat="server" Style="display:none;" />
    <asp:ModalPopupExtender ID="mdl_chkbox" runat="server" TargetControlID="btnchkbox" PopupControlID="pnlCheckBoxSelect"
        CancelControlID="Imgchkbox" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
        <asp:Panel ID="pnlCheckBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Upchkbox" runat="server" UpdateMode="Conditional">
                <contenttemplate>  
<table cellspacing="2px" cellpadding="2px" width="100%"> 
<tr>
<td style="width:880px"><h3><asp:Label ID="lblChkBox" runat="server"></asp:Label>Apply Fields Check Box</h3></td>
<td style="width:20px"><asp:ImageButton ID="Imgchkbox" ImageUrl="images/close.png" runat="server" OnClick="imgChkBoxClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="box">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
    <tr>
     <td align="left"> <asp:Label ID="lblChkboxmsg" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td colspan="2">&nbsp; </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtchkboxDN" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtchkboxDS" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr>
        <tr>
            <td align="left" colspan="2"><label>* Display Text  :</label><asp:TextBox ID="txtchkboxDT" runat="server" Width="80%" CssClass="txtBox"></asp:TextBox></td>
        </tr>
        <tr>
            <td align="left">
                <label>
                Field Properties
                </label>
            </td>
            <td align="left">
                <asp:CheckBox ID="ChkboxMan" runat="server" Text="Mandatory" />
                <asp:CheckBox ID="ChkboxAct" runat="server" Text="Active" />
                <asp:CheckBox ID="ChkboxEdit" runat="server" Text="Editable" />
                <asp:CheckBox ID="chkboxwf" runat="server" Text="Work Flow" />
                <asp:CheckBox ID="chkboxun" runat="server" Text="Unique" />
                <asp:CheckBox ID="chkboxdes" runat="server" Text="Description" />
            </td>
            <td>
                <asp:Button ID="btn_chkbox" runat="Server" CssClass="btnNew" OnClick="EditRecord" Text="SAVE" Width="80px" />
            </td>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <%--Change By Mayank  tab is not showing--%><%-- <style type="text/css">
                .fancy-green .ajax__tab_hader {
                    kground url(ima i cursor: pointer;
                }

                .fancy-gr en .ajax__tab_ho e ancy-green ajax__tab_active .ajax__tab_ouer {
                    t_Tab .gif) n -repeat left top;
                }

                .fancy-gr en .ajax__tab_ho e ancy-green ajax__tab_active .ajax__tab_innr {
                    r _Tab.g f) no-repeat righ;
                }

                _header {
                    : 13px weight: bold # font family: sans-seri;
                }

                .fancy .ajax__ta _activ .ajax__tab_oute, .fancy .ajax__ a .fancy .ajax _ height: 46px;
                }

                .fancy .ajax__ta _activ .ajax__tab_inne, .fancy .ajax__ a .fancy .ajax er {
                    height: 46px;
                    h of t e left image */;
                }

                .fancy .aja __tab_ ctive .ajax__tab_ ab, .fancy .aj x, .fanc .ajx__tb_hader margin 16px 16px 0px 0 x;
                }

                .fancy .aj x _tab, fancy colo :
                }

                body {
                    y: Arial;
                    size: 1 pt;
                    lid #999999;
                    p b ff;
                }

                {
                    or: whi e;
                    9ae46;
                    na 10px;
                    pdding: 1px 4px;
                    y Arial, Helvet c;
                }

                eader {
                    ;
                    background-color: Green;
                    kground: url( mage peat-x;
                    : 2p;
                    position: relative;
                    t tNode.parentNode.scrollTop-1);
                    terSha ows height: 100% width: 99%;
                    padding: 5px;
                    background-color: w (note the rgba is red gren, lue, alha *
   it-box-shadow: 0 x  0x  1px gba(0, 0 0, 0.4;
                    
                                       -mo 6px rgba(23, 69, 88, . );  /*
                                       webk
                    ;
                  -moz-border-            border-radius: 7px; /* gadiets *
                  backgound:  -ebkit-gradient(inear, eft top, left botom, color
                    r-stop(15%, white), color-stop(100%, D7E9F));
                 back
                    adient(top, white 0%, white 55%,  #D54F3 130);
                    
                  background -ms-linear-radient(top, #e1ffff 0%,#dffff  0%,#e1fff  100%,#c8efb  100%,#c8efb  100%,#be4f8  100%,#be4f8  100%,#be4f8  10 %,#bee4f8 1
                    f5 100%,#e1 fff 100%,#e6f8fd  10%); /* IE10+ *
                    
                                      background: inear-gradiet(to bottom, #e1ffff 0%,#dffff  0%,#e1fff  100%,#c8efb  100%,#c8efb  100%,#be4f8  100%,#be4f8  100%,#be4f8  10 %,#bee4f8
                    d8f5 10 %,#e1ffff 100%,#e6f8fd 100%); /* W3C */
                   filter: progid:DXImageTransform.Microsoft.grdi nt( startCo
                e

                ,Gradie t
                    
               %;
              .dyn {  x

                                        ynamicMen  {
     n

                               position: lute;
                };;;;;;;;
            </style>--%><%--Add Primary key field by Mayank --%><%-- fOR LookupDDl  --%>
    <asp:Button ID="btnml" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalpopupMultiLookUP" runat="server"
        TargetControlID="btnml" PopupControlID="pnlMultiLookUp"
        CancelControlID="MultilookupClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlMultiLookUp" runat="server" Width="900px" Height="330px" ScrollBars="Auto" Style="" BackColor="white">
        <div class="box">

            <asp:UpdatePanel ID="UpdpanelMultiLookUp" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 880px">
                                <h3>
                                    <asp:Label ID="lblformcaptionML" runat="server"></asp:Label>
                                    Apply Multi Look up</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="MultilookupClose" ImageUrl="images/close.png" runat="server" OnClick="btnMultiLookUpClose" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                        <tr>
                                            <td align="left" colspan="6">
                                                <asp:Label ID="lbldispmsgML" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width:120px;" align="left">
                                                <label>* Display Name : </label>
                                                
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <asp:TextBox ID="txtdpML" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                            <td style="width: 140px;" align="left">
                                                <label>* Data Types  : </label>
                                                &nbsp;&nbsp;&nbsp;
                                                </td>
                                            <td style="width: 150px;" align="left">
                                                <asp:DropDownList ID="ddlftypeML" runat="server">
                                                    <asp:ListItem>SELECT ONE</asp:ListItem>
                                                    <asp:ListItem>Text</asp:ListItem>
                                                    <asp:ListItem>Numeric</asp:ListItem>
                                                    <asp:ListItem>Datetime</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <label>Description :</label></td>
                                            <td style="width: 150px;" align="left">
                                                <asp:TextBox ID="txtdescML" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 120px;" align="left">
                                                <label>* Form Type : </label>
                                                </td>
                                            <td style="width: 150px;" align="left">
                                                &nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlFtypeMultiL" runat="server" Width="150px" AutoPostBack="true">
                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                    <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                    <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                    <asp:ListItem Value="3">USER</asp:ListItem>

                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <label>* Composite Form : </label></td>
                                            <td style="width: 150px;" align="left">
                                                &nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlcompositeForm" runat="server" Width="150px" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                              <%--<td style="width: 150px;" align="left">
                                                <label>* Field Type : </label></td>
                                            <td>
                                                <asp:DropDownList ID="ddlFieldType" runat="server" Width="150px" AutoPostBack="true">
                                                    <asp:ListItem>SELECT</asp:ListItem>
                                                    <asp:ListItem>DROPDOWN</asp:ListItem>
                                                    <asp:ListItem>LOOKUPDDL</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>--%>
                                           <td style="width: 150px;" align="left">
                                                <label>* Select Field: </label>
                                                
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <asp:DropDownList ID="ddlfieldsMLC" runat="server" Width="150px" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        
                                        <tr>
                                            <td colspan="6">
                                                <div style="overflow-x: hidden;overflow-y: scroll;max-height:150px; ">
                                                <asp:CheckBoxList ID="chklistMultiLookup" TextAlign="Right" runat="server"></asp:CheckBoxList>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="6" id="lengthML" runat="server" visible="false">
                                                <label>Field Length :</label>
                                                <asp:TextBox ID="txtFieldLengthminML" runat="server" CssClass="txtBox"> </asp:TextBox>
                                                &nbsp;&nbsp;    
                                                <asp:TextBox ID="txtFieldLengthmaxML" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label>Mandatory Field  : </label>
                                            </td>
                                            <td align="left" colspan="5">
                                                <asp:CheckBox ID="chkFieldMandML" runat="server" Text="Field is Mandatory" />
                                                <asp:CheckBox ID="chkFieldActdML" runat="server" Text="Active" />
                                                <asp:CheckBox ID="chkFieldEditdML" runat="server" Text="Editable" />
                                                <asp:CheckBox ID="chkFieldWFdML" runat="server" Text="Work Flow" /></td>
                                         
                                        </tr>
                                        <tr>
                                            <td colspan="6">
                                                <asp:Button ID="BtnSaveML" runat="Server" CssClass="btnNew" Width="80px" Text="SAVE" OnClick="EditRecord" />
                                                                                    </td></tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="lookupClose" />
                </Triggers>

            </asp:UpdatePanel>

        </div>
    </asp:Panel>

    <%--for LookupDDl field--%>
     <asp:Button ID="Btnmlddl" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="Mod_mlddl" runat="server"
        TargetControlID="Btnmlddl" PopupControlID="pnlMultiLookUpddl"
        CancelControlID="MultilookupDDLClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlMultiLookUpddl" runat="server" Width="900px" Height="330px" ScrollBars="Auto" Style="" BackColor="white">
        <div class="box">

            <asp:UpdatePanel ID="UpMultiLookUpddl" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 880px">
                                <h3>
                                    <asp:Label ID="Lbl_MultiLookUpddl" runat="server"></asp:Label>
                                    Apply Multi LookupDDL</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="MultilookupDDLClose" ImageUrl="images/close.png" runat="server" OnClick="btnMultiLookUpDDLClose" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="form">
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                        <tr>
                                            <td align="left" colspan="6">
                                                <asp:Label ID="msg_mlddl" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="width:120px;" align="left">
                                                <label>* Display Name : </label>
                                                
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <asp:TextBox ID="txtmlddldn" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                            <td style="width: 140px;" align="left">
                                                <label>* Data Types  : </label>
                                                &nbsp;&nbsp;&nbsp;
                                                </td>
                                            <td style="width: 150px;" align="left">
                                                <asp:DropDownList ID="Ddlmlddldt" runat="server">
                                                    <asp:ListItem>SELECT ONE</asp:ListItem>
                                                    <asp:ListItem>Text</asp:ListItem>
                                                    <asp:ListItem>Numeric</asp:ListItem>
                                                    <asp:ListItem>Datetime</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <label>Description :</label></td>
                                            <td style="width: 150px;" align="left">
                                                <asp:TextBox ID="txtmlddlDs" runat="server" CssClass="txtBox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 120px;" align="left">
                                                <label>* Form Type : </label>
                                                </td>
                                            <td style="width: 150px;" align="left">
                                                &nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlmlddlft" runat="server" Width="150px" AutoPostBack="true">
                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                    <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                    <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                    <asp:ListItem Value="3">USER</asp:ListItem>

                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <label>* Composite Form : </label></td>
                                            <td style="width: 150px;" align="left">
                                                &nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlmlddlcf" runat="server" Width="150px" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                             <%--<td style="width: 150px;" align="left">
                                                <label>* Field Type : </label>
                                                
                                            </td> 
                                            <td>
                                                <asp:DropDownList ID="ddlMLDDLType" runat="server" Width="150px" AutoPostBack="true">
                                                    <asp:ListItem>SELECT</asp:ListItem>
                                                    <asp:ListItem>DROPDOWN</asp:ListItem>
                                                    <asp:ListItem>LOOKUPDDL</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>--%>
                                             <td style="width: 150px;" align="left">
                                                <label>* Select Field: </label>
                                                
                                            </td>
                                            <td style="width: 150px;" align="left">
                                                <asp:DropDownList ID="ddlmlddlf" runat="server" Width="150px" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            
                                        </tr>
                                       
                                        <tr>
                                            <td colspan="6">
                                                <div style="overflow-x: hidden;overflow-y: scroll;max-height:150px; ">
                                                <asp:CheckBoxList ID="chkmlddl" runat="server"></asp:CheckBoxList>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="6" id="Td14" runat="server" visible="false">
                                                <label>Field Length :</label>
                                                <asp:TextBox ID="txtmlddlfl" runat="server" CssClass="txtBox"> </asp:TextBox>
                                                &nbsp;&nbsp;    
                                                <asp:TextBox ID="txtmlddlfml" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <label>Mandatory Field  : </label>
                                            </td>
                                            <td align="left" colspan="5">
                                                <asp:CheckBox ID="chkmlddlMF" runat="server" Text="Field is Mandatory" />
                                                <asp:CheckBox ID="chkmlddlACT" runat="server" Text="Active" />
                                                <asp:CheckBox ID="chkmlddlE" runat="server" Text="Editable" />
                                                <asp:CheckBox ID="chkmlddlWF" runat="server" Text="Work Flow" /></td>
                                         
                                        </tr>
                                        <tr>
                                            <td colspan="6">
                                                <asp:Button ID="btn_mlddlSU" runat="Server" CssClass="btnNew" Width="80px" Text="SAVE" OnClick="EditRecord" />
                                                                                    </td></tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="MultilookupDDLClose" />
                </Triggers>

            </asp:UpdatePanel>

        </div>
    </asp:Panel>
    <%--<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.js" type="text/javascript"></script>--%>    <%--<script src="http://ajax.microsoft.com/ajax/beta/0911/Start.debug.js" type="text/javascript"></script>--%><%--For Block form --%><%--<asp:ListItem>Amendment</asp:ListItem>--%>
    <asp:Button id="btncatchment" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mdl_catchment" runat="server" TargetControlID="btncatchment" PopupControlID="pnlcatchmentBoxSelect"
        CancelControlID="txtcatchmentcancel" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlcatchmentBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Upcatchment" runat="server" UpdateMode="Conditional">
                <contenttemplate>  
<table cellspacing="2px" cellpadding="2px" width="100%"> 
<tr>
<td style="width:880px"><h3><asp:Label ID="Catchment" runat="server"></asp:Label>Apply Fields Catchment</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtcatchmentcancel" ImageUrl="images/close.png" runat="server" OnClick="txtcatchmentClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="box">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblcatchment" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtcatchmentdisplay" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtcatchmentdes" runat="server" CssClass="txtBox"></asp:TextBox><label> Default Value :</label><asp:TextBox ID="txtdefcatchment" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr><tr>
     
     <td  align="left" ><label>* Data Types :  </label> &nbsp;<asp:DropDownList ID="ddltypecatchment" runat="server" >
      <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem>
     <asp:ListItem>Numeric</asp:ListItem>
     <asp:ListItem>Datetime</asp:ListItem>
     </asp:DropDownList> </td> 
       <td align="left"> <label>Field Length :</label>  <asp:TextBox ID="txtfcatchmentmin" runat="server" CssClass="txtBox" > </asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender19" runat ="server"    TargetControlID ="txtfcatchmentmin" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
             <asp:TextBox ID="txtfcatchmentmax" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
             <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender20" runat ="server"    TargetControlID ="txtfcatchmentmax" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>   </tr>

        <tr><td align="left" ><label>* Geo Point Field :</label> &nbsp;<asp:DropDownList ID="ddlcatchmentgeopointfield" runat="server" AutoPostBack="true"></asp:DropDownList></td>
            <td align="left" ><label>* Geo Display Name :</label> &nbsp;<asp:DropDownList ID="ddlcatchmentdisplayfield" runat="server" AutoPostBack="true"></asp:DropDownList></td>
              </tr>
          <tr><td align="left" ><label>* Geo Fence Field :</label> &nbsp;<asp:DropDownList ID="ddlcatchmentgeofencefield" runat="server" AutoPostBack="true"></asp:DropDownList></td>
        </tr>
<tr>
<td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="chkcatchmentman" runat="server" Text="Mandatory" /> <asp:CheckBox ID="chkcatchmentact" runat="server" Text="Active" /> <asp:CheckBox ID="chkcatchmentedit" runat="server" Text="Editable" /> <asp:CheckBox ID="chkcatchmentWF" runat="server" Text="Work Flow" /> <asp:CheckBox ID="chkcatchmentuni" runat="server" Text="Unique" /> <asp:CheckBox ID="chkcatchmentdes" runat="server" Text="Description" /> </td>
<td><asp:Button ID="btncatchmentsave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
   
    <asp:Button id="btngeofence" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="mdl_geofence" runat="server" TargetControlID="btngeofence" PopupControlID="pnlgeofeBoxSelect"
        CancelControlID="txtgeofecancel" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlgeofeBoxSelect" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="Upgeofence" runat="server" UpdateMode="Conditional">
                <contenttemplate>  
<table cellspacing="2px" cellpadding="2px" width="100%"> 
<tr>
<td style="width:880px"><h3><asp:Label ID="GeoFence" runat="server"></asp:Label>Apply Fields GEO FENCE</h3></td>
<td style="width:20px"><asp:ImageButton ID="txtgeofecancel" ImageUrl="images/close.png" runat="server" OnClick="txtGeofenceClose"/></td>
</tr>
<tr>
<td colspan="2">
<div class="box">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblfencegeo" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> <tr><td> </td></tr>
     <tr>
         <td align="left"><label>* GeoFence Type </label></td>
         <td align="left">
             <asp:DropDownList ID="ddlgeotype" runat="server"  >
                 <asp:ListItem Value="0">Select</asp:ListItem>
                 <asp:ListItem Value="1">Polygon</asp:ListItem>
                 <asp:ListItem Value="2">Circular</asp:ListItem>

             </asp:DropDownList>
         </td>
     </tr>
        
        <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtdisplaynamefence" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     <td align="left"><label> Description :</label><asp:TextBox ID="txtdesnamefence" runat="server" CssClass="txtBox"></asp:TextBox><label> Default Value :</label><asp:TextBox ID="txtdefaultfence" runat="server" CssClass="txtBox"></asp:TextBox> </td> 
     </tr><tr>
     
     <td  align="left" ><label>* Data Types :  </label> &nbsp;<asp:DropDownList ID="ddldatatypegeofence" runat="server" >
     <asp:ListItem>SELECT ONE</asp:ListItem>
     <asp:ListItem>Text</asp:ListItem> 
     </asp:DropDownList> </td> 
     
       <td align="left"> <label>Field Length :</label>  <asp:TextBox ID="txtminfence" runat="server" CssClass="txtBox" > </asp:TextBox>
           <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender16" runat ="server"    TargetControlID ="txtminfence" WatermarkCssClass ="water"    WatermarkText ="Minimum Length"></asp:TextBoxWatermarkExtender>
           &nbsp;&nbsp;
             <asp:TextBox ID="txtmaxfence" runat="server" CssClass="txtBox" Width="146px"></asp:TextBox>
             <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender17" runat ="server"    TargetControlID ="txtmaxfence" WatermarkCssClass ="water"    WatermarkText ="Maximum Length"></asp:TextBoxWatermarkExtender> </td>   </tr>

 <tr>
<td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="chkmanfence" runat="server" Text="Mandatory" /> <asp:CheckBox ID="chkactfence" runat="server" Text="Active" /> <asp:CheckBox ID="chkeditfence" runat="server" Text="Editable" /> <asp:CheckBox ID="chkflowfence" runat="server" Text="Work Flow" /> <asp:CheckBox ID="chkunifence" runat="server" Text="Unique" /> <asp:CheckBox ID="chkdescfence" runat="server" Text="Description" /> </td>
<td><asp:Button ID="btngeofencesave" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord"/></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
      <asp:Button id="btn_ParentVal" runat="server" style="display: none" />
    <asp:ModalPopupExtender ID="modalpopup_ParentVal" runat="server" TargetControlID="btn_ParentVal" PopupControlID="pnl_ParentVal"
        CancelControlID="BtnParentVal_Close" BackgroundCssClass="modalBackground" DropShadow="true">
   </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_ParentVal" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="upd_ParentVal" runat="server" UpdateMode="Conditional">
                <contenttemplate>  
<table cellspacing="2px" cellpadding="2px" width="100%"> 
<tr>
<td style="width:880px"><h3><asp:Label ID="lbl_ParentVal" runat="server"></asp:Label>Parent Value</h3></td>
<td style="width:20px"><asp:ImageButton ID="BtnParentVal_Close" ImageUrl="images/close.png" runat="server" OnClick="btn_ParentVal_Cancle"/></td>
</tr>
<tr>
<td colspan="2">
<div class="box">
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
    <tr>
     <td align="left"> <asp:Label ID="lblParentVal" runat="server"  ForeColor="Red"></asp:Label></td>
     </tr> 
        
        <tr><td align="left"><label> * Display Name :</label><asp:TextBox ID="txtParentVal_DisplayNm" runat="server" CssClass="txtBox"></asp:TextBox> </td>
     </tr><tr>
     
     <td  align="left" ><label>* Select Parent:  </label> &nbsp;<asp:DropDownList ID="ddlParentVal_Parentdoc" runat="server" AutoPostBack="True" >
     </asp:DropDownList> </td> 
     
       <td align="left"> <label>* Select Field :</label>  
          
           <asp:DropDownList ID="ddlParentVal_ParentFields" runat="server">
           </asp:DropDownList>
            </td> 

          </tr>

 <tr>
<td align="left"><label> Field Properties </label></td> <td align="left"> <asp:CheckBox ID="chkPV_Man" runat="server" Text="Mandatory" /> 
     <asp:CheckBox ID="chkPV_Active" runat="server" Text="Active" /> <asp:CheckBox ID="chkPV_Editable" runat="server" Text="Editable" />
      <asp:CheckBox ID="chkPV_WrkFlow" runat="server" Text="Work Flow" /> <asp:CheckBox ID="chkPV_Unique" runat="server" Text="Unique" />
      <asp:CheckBox ID="chkPV_Desc" runat="server" Text="Description" /> </td>
<td><asp:Button ID="btnParentVal_Save" runat="Server"  CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord" /></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
</contenttemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>



         <%--  chandni--%>

       <asp:Button ID="Btn_InputAction" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="modalpopup_InputAction" runat="server" TargetControlID="Btn_InputAction" PopupControlID="pnl_InputAction"
                CancelControlID="BtnInputAction_Close" BackgroundCssClass="modalBackground" DropShadow="true">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnl_InputAction" runat="server" Width="900px" Height="330px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="upd_InputAction" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div>
                        <table cellspacing="2px" cellpadding="2px" width="100%">
                            <tr>
                                <td style="width: 880px">
                                    <h3>
                                        <asp:Label ID="Label28" runat="server"></asp:Label>Input Action Value</h3>
                                </td>
                                <td style="width: 20px">

                                    <asp:ImageButton ID="BtnInputAction_Close" ImageUrl="images/close.png" runat="server" OnClick="InputActionClose" /></td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <div style="margin-top: 10px;  margin-bottom: 10px;  margin-right: 50px;  margin-left: 50px;" class="box">
                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                            <tr>
                                                <td align="left">
                                                    <asp:Label ID="lblActionVal" runat="server" ForeColor="Red"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <label>* IsActive:  </label>
                                                </td>
                                                <td  align="left"><asp:DropDownList ID="ddlActionVal_IsActive" runat="server" Width="300px" CssClass="txtBox">
                                                    <asp:ListItem Selected="True" Value="0" Text="InActive"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                                </asp:DropDownList>
                                                </td>

                                                

                                            </tr>
                                            <tr>

                                                <td align="left">
                                                    <label>* Caption :</label>
                                                </td>
                                                <td align="left" colspan="2">
                                                    <asp:TextBox ID="txtActionVal_Caption"  Width="300px" runat="server"  CssClass="txtBox">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                 <td align="left">
                                                    <label>* Input Field Value :</label>
                                                </td>
                                                <td align="left" colspan="2">
                                                    <asp:TextBox ID="txtActionVal_InputFieldVal" runat="server" CssClass="txtBox">
                                                    </asp:TextBox></td>
                                            </tr>
                                            <tr>
                                               
                                                <td align="left">
                                                    <label>* Select Target Field :</label></td>
                                                <td  align="left"><asp:DropDownList ID="chklActionVal_TargetFieldVal" runat="server" Width="300px" CssClass="txtBox"></asp:DropDownList>

                                                </td>
                                            </tr>
                                            <tr>

                                                <td align="left">
                                                    <label>* Select Work Flow :</label></td>
                                                <td  align="left"><asp:CheckBoxList ID="chkl_Workflow" runat="server" RepeatColumns="2" RepeatDirection="Vertical"></asp:CheckBoxList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <asp:label id="lbl_inputAction" runat="server" ></asp:label>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Button ID="btnActionVal_Save" runat="Server" CssClass="btnNew" Text="SAVE" Width="80px" OnClick="EditRecord_ActionVal" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

          <%--  chandni--%>
       <asp:Button ID="Btn_LMSetting" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="mod_LMSetting" runat="server" TargetControlID="Btn_LMSetting" PopupControlID="pnl_LMSetting"
        CancelControlID="Btn_LMSetting_close" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_LMSetting" runat="server" Width="900px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="up_pnl_LMSetting" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div>
                        <table cellspacing="2px" cellpadding="2px" width="100%">
                            <tr>
                                <td style="width: 880px">
                                    <h3>
                                        <asp:Label ID="Label29" runat="server"></asp:Label>Master And Child Confriguration</h3>
                                </td>
                                <td style="width: 20px">

                                    <asp:ImageButton ID="Btn_LMSetting_close" ImageUrl="images/close.png" runat="server" OnClick="LMSettingClose" /></td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <div style="margin-top: 0px; margin-bottom: 0px; margin-right: 50px; margin-left: 50px;">
                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                            <tr>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                            </tr>

                                            <tr>
                                                <td align="center">
                                                    <label>Base Document:  </label>
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lbl_BaseDoc" Text=""></asp:Label></td>
                                                
                                            </tr>
                                            
                                            <tr>
                                                <td align="center">
                                                    <label>Source Document:  </label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="DDL_AddNewSDoc" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                                <td align="center">
                                                    &nbsp;</td>
                                                <td></td>

                                            </tr>
                                            <tr  align="left">
                                                <td align="center">
                                                    <label>*Source Doc Type: </label>
                                                </td>
                                                <td style="width: 565px">
                                                    <asp:DropDownList ID="ddl_DocType" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="True">
                                                        <asp:ListItem Selected="True" Text="SELECT" Value="0"></asp:ListItem>
                                                        <asp:ListItem Text="Main" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="Detail Form" Value="2"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td align="center">
                                                    <asp:label id="lbl_fields" runat="server" visible="false" text="*Main Doc Fields:"></asp:label>
                                                    <asp:DropDownList ID="ddl_ChildDocType" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="True" Visible="false">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddl_FielMapping" runat="server" CssClass="txtBox" Width="200px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td align="center">
                                                    <label>*Target Doc: </label>
                                                </td>
                                                <td style="width: 565px">
                                                    <asp:DropDownList ID="ddl_TargetDocType" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                                <td align="center">
                                                    <label>*Target Doc Fields: </label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddl_TargetDocFieldMapping" runat="server" CssClass="txtBox" Width="200px">
                                                    </asp:DropDownList></td>
                                            </tr>
                                            
                                            <tr align="left">
                                                <td align="center">
                                                    <label>*Work Flow Status: </label>
                                                </td>
                                                <td style="width: 565px">
                                                    <asp:DropDownList ID="ddl_wfstatus" runat="server" CssClass="txtBox" Width="200px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td  align="center">
                                                    <label>*IsActive:  </label>
                                                </td>
                                                <td align="left" style="width: 565px">
                                                    <asp:DropDownList ID="ddl_Isactive" runat="server" Width="200px" CssClass="txtBox">
                                                        <asp:ListItem Selected="True" Value="0" Text="No"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td colspan="4" style="height: 10px"></td>
                                            </tr>
                                            <tr align="left">
                                                <td></td>
                                                <td colspan="2">
                                                    <asp:Label ID="lbl_Msg" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Button ID="btn_Add" runat="Server" CssClass="btnNew" Text="Add LM Field mapping" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td colspan="4" style="height: 10px"></td>
                                            </tr>
                                            <tr align="center">
                                                <td colspan="4">
                                                    <div style="height: 100px; overflow-y: scroll;">
                                                        <asp:GridView ID="gv_FieldMapping" AutoGenerateColumns="false" runat="server" Width="100%" CellPadding="2" OnRowDataBound="OnRowDataBound" OnPageIndexChanging="OnPageIndexChanging" EmptyDataText="No records has been added." OnRowDeleting=" OnRowDeleting" CssClass="GridView" AllowSorting="True" HeaderStyle-BackColor="#3366ff">
                                                            <Columns>
                                                                <asp:BoundField DataField="RowNumber" HeaderText="S.No" />
                                                                <asp:BoundField DataField="TFld" HeaderText="TargetFld" >
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="TFldName" HeaderText="TargetFldName">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SFld" HeaderText="SourceFld" >
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SFldName" HeaderText="SourceFldName">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Ordering" HeaderText="Ordering">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="DType" HeaderText="DocumentType">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="childdoctype" HeaderText="BaseDocType">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                 <asp:BoundField DataField="SourceDocType" HeaderText="SourceDocType">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:CommandField  ShowDeleteButton="True" ButtonType="Button" />
                                                            </Columns>
                                                            <HeaderStyle BackColor="#3366FF" />
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="height: 10px"></td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="4">
                                                    <asp:Button ID="btn_Save" runat="Server" CssClass="btnNew" Text="SAVE" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="height: 10px"></td>
                                            </tr>
                                        </table>

                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

        <%--  chandni--%>
           <asp:Button ID="Btn_AutoInv" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="Mod_AutoInvSettings" runat="server" TargetControlID="Btn_AutoInv" PopupControlID="pnl_AutoInvSetting"
        CancelControlID="Btn_AutoInv_close" BackgroundCssClass="modalBackground" DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_AutoInvSetting" runat="server" Width="900px" ScrollBars="Auto" BackColor="white">
        <div class="box">
            <asp:UpdatePanel ID="updpnl_Btn_AutoInv" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div style="margin-top: 0px; margin-bottom: 0px; margin-right: 50px; margin-left: 50px;">
                        <table cellspacing="2px" cellpadding="2px" width="100%">
                            <tr>
                                <td style="width: 880px">
                                    <h3>
                                        <asp:Label ID="lbl_AutoInv" runat="server"></asp:Label>Auto Invoice Master Configuration</h3>
                                </td>
                                <td style="width: 20px">
                                    <asp:ImageButton ID="Btn_AutoInv_close" ImageUrl="images/close.png" runat="server" OnClick="AutoInvSettingClose" /></td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <div style="margin-top: 0px; margin-bottom: 0px; margin-right: 10px; margin-left: 10px;">
                                        <table>
                                            <tr>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                            </tr>

                                            <tr>

                                                <td>
                                                    <label>*Lease Type: </label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddl_AutoInvLType" runat="server" class="form-control" Width="200px" >
                                                    </asp:DropDownList>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>

                                            <tr>
                                                <td colspan="4">
                                                    <div id="div_AutoInvLT_Inv" style="margin-top: 0px; margin-bottom: 0px; margin-right: 5px; margin-left: 5px;">
                                                        <table cellspacing="2px" cellpadding="2px" width="100%">
                                                            <tr>
                                                                <td style="width: 25%"></td>
                                                                <td style="width: 25%"></td>
                                                                <td style="width: 25%"></td>
                                                                <td style="width: 25%"></td>
                                                            </tr>
                                                            <tr align="left">
                                                                <td>Start Date</td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_AISrcSDt" runat="server" class="form-control" Width="200px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>End Date</td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_AISrcEDt" runat="server" class="form-control" Width="200px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr align="left">
                                                                <td>Payment Term Field</td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_FrequencyFld" runat="server" class="form-control" Width="200px">
                                                                       
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>Rent Free Period</td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_RentPeriodFld" class="form-control" Width="200px" runat="server"></asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr align="left">
                                                                <td>Rental Amount Field</td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddl_AISrcRentalFld" runat="server" class="form-control" Width="200px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>SD AMount</td>
                                                                <td> 
                                                                    <asp:DropDownList ID="ddl_AutoInv_SDCDate" runat="server" class="form-control" Width="200px">
                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr  align="left">
                                                <td>Cam Amount</td>
                                                <td>
                                                    <asp:DropDownList ID="DDL_CAMAmnt" runat="server" class="form-control" Width="200px">
                                                    </asp:DropDownList></td>
                                               
                                               <td>Security Period (Month)</td>
                                                <td>
                                                    <asp:DropDownList ID="DDl_SDMonths" runat="server" class="form-control" Width="200px">
                                                    </asp:DropDownList></td>

                                            </tr>
                                                            <tr align="left">
                                                                <td>Rent Esclation clause</td>
                                                                <td>
                                                                    <asp:DropDownList ID="DDl_rentEsc" runat="server" class="form-control" Width="200px">
                                                                    </asp:DropDownList></td>
                                                                <td>Rent Esclation(%)</td>
                                                                <td>
                                                                    <asp:DropDownList ID="DDL_RentEscPtage" runat="server" class="form-control" Width="200px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                </tr>
                                                </td>
                                            </tr>
                                            <tr  align="left">
                                                <td>Cam Esclation Clause</td>
                                                <td>
                                                    <asp:DropDownList ID="DDl_CamEsc" runat="server" class="form-control" Width="200px">
                                                    </asp:DropDownList></td>
                                                <td>Cam Esclation(%)</td>
                                                <td>
                                                    <asp:DropDownList ID="DDL_CAMEscPtage" runat="server" class="form-control" Width="200px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                             
                                           
                                            <tr>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                                <td style="width: 25%"></td>
                                            </tr>
                                        </table>
                                    </div>

                                </td>
                            </tr>
                            <tr align="left">
                                <td align="left">
                                    <label>*Source Document:  </label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lbl_AutoBaseDoc" Text=""></asp:Label></td>
                                <td align="left">
                                    <label>*Source Doc Fields: </label>
                                </td>

                                <td align="left">
                                    <asp:DropDownList ID="ddl_AutoInvSrcDoc_flds" runat="server" class="form-control" Width="200px">
                                    </asp:DropDownList>
                                </td>

                            </tr>
                            <tr align="left">
                                <td align="left" style="height: 20px">
                                    <label>*Target Doc: </label>
                                </td>
                                <td style="height: 20px">
                                    <asp:DropDownList ID="ddl_AutoInvTgtDoc" runat="server" class="form-control" Width="200px" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                                <td align="left" style="height: 20px">
                                    <label>*Target Doc Fields: </label>
                                </td>
                                <td align="left" style="height: 20px">
                                    <asp:DropDownList ID="ddl_AutoInvTgtDoc_flds" runat="server" class="form-control" Width="200px">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr align="left">

                                <td align="left">
                                    <label>*IsActive:  </label>
                                </td>
                                <td align="left" style="width: 565px">
                                    <asp:DropDownList ID="ddl_ActiveStatus" runat="server" Width="200px" class="form-control">
                                        <asp:ListItem Selected="True" Value="SELECT" Text="SELECT"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td align="left"></td>
                                <td style="width: 565px"></td>
                            </tr>
                            <tr align="left">
                                <td colspan="4" style="height: 10px">&nbsp;</td>
                            </tr>
                            <tr align="left">
                                <td colspan="2">
                                    <asp:Label ID="lbl_AutoInvMsg" ForeColor="Red" runat="server"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Button ID="btn_AutoInvAdd" runat="Server" CssClass="btnNew" ToolTip="Add Auto Inv Field mapping" Text="Add Auto Inv Field mapping" />
                                </td>
                                <td align="right">
                                    <asp:Button ID="btn_headerUpdate" runat="Server" CssClass="btnNew" ToolTip="Header Configuration Update" Text="Header Configuration Update" /></td>
                            </tr>
                            <tr align="left">
                                <td colspan="4" style="height: 10px"></td>
                            </tr>
                            <tr align="left">
                                <td colspan="4">
                                    <div style="height: 100px; overflow-y: scroll; margin-top: 0px; margin-bottom: 0px; margin-right: 5px; margin-left: 5px;">

                                        <asp:GridView ID="gv_AutoInv" AutoGenerateColumns="false" runat="server" Width="100%" CellPadding="2" OnRowDataBound="OnRowDataBound" EmptyDataText="No records has been added." class="form-control" OnRowDeleting=" OnRowDeletingAutoInv" CssClass="GridView" AllowSorting="True" HeaderStyle-BackColor="#3366ff">
                                            <Columns>
                                                <asp:BoundField DataField="RowNumber" HeaderText="S.No" />
                                                <asp:BoundField DataField="TFld" HeaderText="TargetFld">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TFldName" HeaderText="TargetFldName">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="SFld" HeaderText="SourceFld">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="SFldName" HeaderText="SourceFldName">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Sdoctype" HeaderText="SourceDocType">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Tdoctype" HeaderText="TargetDocType">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <%-- <asp:BoundField DataField="LeaseType" HeaderText="LeaseType">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>--%>
                                                <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />

                                            </Columns>

                                            <HeaderStyle BackColor="#3366FF" />
                                        </asp:GridView>

                                    </div>


                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 10px"></td>
                            </tr>
                            <tr>
                                <td align="right" colspan="4">
                                    <asp:Button ID="btn_AutoInvSave" Width="100px" runat="Server" CssClass="btnNew" Text="SAVE" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 10px"></td>
                            </tr>
                        </table>

                    </div>
                    </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddl_AutoInvLType" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </asp:Panel>

  <%--  chandni--%>





   </div>

    <div id="dialog" title="Dynamic Relation Setting" style="display: none;">
        <div class="wrap" style="width: 100%;">
            <div id="dvloader" style="display: block; text-align: center;">
                <input type="image" src="images/preloader22.gif" />
            </div>
            <div id="dvRMessage" style="display: none; text-align: center; font: bold 18px;">
            </div>
            <div id="dvrContent" style="display: none;">
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <label class="label" for="txtRName">Name</label></td>
                        <td>
                            <input type="text" id="txtRName" placeholder="Relation Name" class="form-control" />
                        </td>
                        <td>
                            <label class="label" for="ddlRelationtype">Relation Type</label></td>
                        <td>
                            <select id="ddlRelationtype" class="form-control">
                                <option value="0">---SELECT---</option>
                                <option value="ONE TO MANY">ONE TO MANY</option>
                                <option value="MANY TO MANY">MANY TO MANY</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label class="label" for="ddlSDOC1">Source Doc1</label></td>
                        <td>
                            <select id="ddlSDOC1" class="form-control">
                            </select>
                        </td>
                        <td>
                            <label class="label" for="ddlSource2">Source Doc2</label></td>
                        <td>
                            <select id="ddlSource2" class="form-control">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5"></td>
                    </tr>
                    <tr>
                        <td>
                            <label class="label" for="ddlRQ">Quantity</label></td>
                        <td>
                            <select id="ddlRQ" class="form-control">
                                <option value="0">--select--</option>
                            </select>
                        </td>
                        <td>
                            <label class="label" for="ddlSource2">Balance Mode</label></td>
                        <td>
                            <select id="ddlBMode" class="form-control">
                                   <option value="0">---select---</option>
                                   <option value="YEARLY">YEARLY</option>
                                   <option value="HALF YEARLY">HALF YEARLYy</option>
                                   <option value="QUATERLY">QUATERLY</option>
                                   <option value="MONTHLY">MONTHLY</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5"></td>
                    </tr>
                    <tr>
                        <td>
                            <label class="label" for="spnTDocType">Target Doc Type</label>
                        </td>
                        <td style="text-align: left">
                           <span style="text-align: left; font: bold;" id="spnTDocType"></span>
                        </td>
                        <td>
                            <label class="label" for="chkISActive">Active</label></td>
                        <td style="text-align: left">
                            <input type="checkbox" id="chkISActive" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5"></td>
                    </tr>
                    <tr>
                        <td id="showEx">
                            <label class="label" for="chkShowExted">Show Extend</label></td>
                        <td style="text-align: left" id="showEx1">
                            <input type="checkbox" id="chkShowExted" />
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="5"></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align: right;">
                            <input type="image" src="images/preloader.gif" id="RPreLoader" style="display: none;" />
                            <input type="button" id="btnRSubmit" value="Submit" class="myButton2" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <input type="hidden" id="hdnRID" value="0" />
                        </td>
                    </tr>
                </table>
            </div>

        </div>
    </div>
     <%--  <%# Container.DataItemIndex + 1%>--%>
    <script type="text/javascript">
        function bindrelDoc(res, Quantity) {
            var Docdata = $.parseJSON(res);
            var qdata = $.parseJSON(Quantity);
            //alert(Docdata);
            var optionhtml1 = '<option value="0">' + "---SELECT-----" + '</option>';
            var ddlDoc1 = $("#ddlSDOC1");
            var ddlDoc2 = $("#ddlSource2");
            var ddlDoc3 = $("#ddlRQ");
            $(ddlDoc1).html("");
            $(ddlDoc2).html("");
            $(ddlDoc3).html("");
            $(ddlDoc1).append(optionhtml1);
            $(ddlDoc2).append(optionhtml1);
            $(ddlDoc3).append(optionhtml1)
            $.each(Docdata, function (i) {
                var optionhtml = '<option value="' + Docdata[i].Dropdown + '">' + Docdata[i].Dropdown + '</option>';
                $(ddlDoc1).append(optionhtml);
                $(ddlDoc2).append(optionhtml);
                //Each function end here
            });
            $.each(qdata, function (i) {
                var optionhtml = '<option value="' + qdata[i].FieldMapping + '">' + qdata[i].DisplayName + '</option>';
                $(ddlDoc3).append(optionhtml);
                //Each function end here
            });
        }
        //ddlRQ,ddlBMode
        $("#ddlRelationtype,#ddlTDocType,#ddlSDOC1,#ddlRQ,#ddlBMode").change(function () {
            var val = $(this).val();
            if (val == "0")
                $(this).addClass('Error1');
            else
                $(this).removeClass('Error1');
        });
        $("#txtRName").change(function () {
            var val = $.trim($(this).val());
            if (val == "")
                $(this).addClass('Error1');
            else
                $(this).removeClass('Error1');
        });
        $("#txtRName").keyup(function () {
            var val = $.trim($(this).val());
            if (val == "")
                $(this).addClass('Error1');
            else
                $(this).removeClass('Error1');
        });
        $("#ddlRelationtype").change(function () {
            var val = $(this).val();
            if (val == "ONE TO MANY") {
                $("#showEx1").hide();
                $("#showEx").hide();
                $("#chkShowExted").attr('checked', false)
            }
            else {
                $("#showEx1").show();
                $("#showEx").show();
            }
        });
        var obj = new Array();;
        $("#btnRSubmit").click(function () {
            $("#btnRSubmit").hide();
            $("#RPreLoader").show();
            var Name, Type, T_DOCType, T_DOC, S_DOC1, S_DOC2, ShowExtend = "0", TID = "0", IsActive = "0";
            //txtRName ddlRelationtype ddlTDocType ddlSDOC1 ddlSource2 chkShowExted chkISActive
            Name = $("#txtRName").val();
            TID = $("#hdnRID").val();
            Type = $("#ddlRelationtype").val();
            T_DOC = $("#spnTDocType").html()
            S_DOC1 = $("#ddlSDOC1").val();
            S_DOC2 = $("#ddlSource2").val();
            var RQ = $("#ddlRQ").val();
            var BMODE = $("#ddlBMode").val();
            // //ddlRQ ddlBMode
            var ShowExtend1 = $("#chkShowExted").prop('checked');
            var IsActive1 = $("#chkISActive").prop('checked');
            if (ShowExtend1)
                ShowExtend = '1';
            if (IsActive1)
                IsActive = '1';
            var errMSG = "Error(s) in your submission.\n------------------------------------\n";
            var ISValid = true;
            $.each(obj, function () {
                $(this).removeClass('Error1');
            });
            obj = new Array();
            if ($.trim(Name) == "") {
                errMSG = errMSG + "Name required.\n";
                ISValid = false;
                obj.push($("#txtRName"));
            }
            if (Type == "0") {
                errMSG = errMSG + "Type required.\n";
                ISValid = false;
                obj.push($("#ddlRelationtype"));
            }
            if (S_DOC1 == "0") {
                errMSG = errMSG + "Source Doc1 required.\n";
                ISValid = false;
                obj.push($("#ddlSDOC1"));
            }

            if (RQ == "0") {
                errMSG = errMSG + "Quantity required.\n";
                ISValid = false;
                obj.push($("#ddlRQ"));
            }
            if (BMODE == "0") {
                errMSG = errMSG + "Balance Mode required.\n";
                ISValid = false;
                obj.push($("#ddlBMode"));
            }
            //ddlRQ ddlBMode
            if (ISValid == false) {
                $.each(obj, function () {
                    $(this).addClass('Error1');
                });
                alert(errMSG);
                $(this).show();
                $("#RPreLoader").hide();
                //return false;
            }
            else {
                //Name, Type, T_DOCType, T_DOC, S_DOC1, S_DOC2, ShowExtend = "0", TID = "0", IsActive = "0"; RQ BMODE                                           RQ BMODE

                //var t = '{"Name":"' + Name + '","Type":"' + Type + '"' + ',"T_DOCType":"' + T_DOCType + '",' + '"T_DOC":"' + T_DOC + '",' + '"S_DOC1":"' + S_DOC1 + '",'+'"S_DOC2":"'+S_DOC2 +'",'+ '"ShowExtend":"' + ShowExtend + '",' + '"TID":"' + TID + '",' + '"IsActive":"' + IsActive + '"}';
                var t = '{"Name":"' + Name + '","Type":"' + Type + '"' + ',' + '"T_DOC":"' + T_DOC + '",' + '"S_DOC1":"' + S_DOC1 + '",' + '"S_DOC2":"' + S_DOC2 + '",' + '"ShowExtend":"' + ShowExtend + '",' + '"TID":"' + TID + '",' + '"IsActive":"' + IsActive + '","RQ":"' + RQ + '","BMODE":"' + BMODE + '"}';
                // alert(t);
                //alert("Hi All Set going to save it");
                $.ajax({
                    type: "POST",
                    url: "FormMaster.aspx/AddUpdateRelation",
                    contentType: "application/json; charset=utf-8",
                    data: t,
                    dataType: "json",
                    success: function (response) {
                        //dvRMessage dvrContent dvloader
                        var res = response.d;
                        if (res == "1") {
                            $("#dvRMessage").html("Relation Updated Successfully.");
                            $("#dvRMessage").show();
                            $("#dvrContent").hide();
                            setTimeout("closeDialog()", 1000);
                        }
                        else if (res == "2") {
                            $("#dvRMessage").html("Relation Updated Successfully.");
                            $("#dvRMessage").show();
                            $("#dvrContent").hide();
                            setTimeout("closeDialog()", 1000);
                        }
                        else if (res == "0") {
                            alert("Please try again later error occured!!!");
                            $("#btnRSubmit").show();
                            $("#RPreLoader").hide();
                        }
                        else if (res == "-1") {
                            alert("Sorry!!! Your login session has expired!!");
                            window.location = "SessionOut.aspx";
                        }
                        $(this).show();
                        $("#RPreLoader").hide();
                        //$("#dialog").dialog("close");
                        //alert("Hi This is response " + res);
                    },
                    error: function (data) {
                        alert("Please try again later error occured!!!");
                        $("#btnRSubmit").show();
                        $("#RPreLoader").hide();
                    }
                    //Ajax call end here 
                });
                return false;
            }

        });
        //Ready Function End Here
        function OpenRelationDiv(FormName) {
            //dvRMessage dvrContent dvloader //ddlRQ ddlBMode
            $("#dvRMessage").html("");
            $("#dvRMessage").hide();
            $("#dvrContent").hide();
            $("#dvloader").show();
            $("#spnTDocType").html(FormName);
            $("#btnRSubmit").show();
            $("#RPreLoader").hide();
            $("#txtRName,#ddlRelationtype,#ddlSDOC1,#ddlSource2,#chkShowExted,#chkISActive,#ddlRQ,#ddlBMode").removeClass('Error1');
            var t = '{"TDocType":"' + FormName + '"}';
            $("#dialog").dialog({
                closeOnEscape: false,
                draggable: true,
                modal: true,
                width: 750,
                dialogClass: "main2"
            });
            $.ajax({
                type: "POST",
                url: "FormMaster.aspx/GetRelation",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    var arr = res.split("|");
                    var data1 = $.parseJSON(arr[0]);
                    bindrelDoc(arr[1], arr[2]);
                    //alert(Docdata);
                    if (data1.length > 0) {
                        $.each(data1, function (i) {
                            $("#txtRName").val(data1[i].Name);
                            $("#ddlRelationtype").val(data1[i].Type);
                            //$("#ddlTDocType").val(data1[i].T_DOCType); //ddlRQ ddlBMode
                            $("#spnTDocType").html(data1[i].T_DOC);
                            $("#ddlSDOC1").val(data1[i].S_DOC1);
                            $("#ddlSource2").val(data1[i].S_DOC2);
                            $("#ddlRQ").val(data1[i].Quantity);
                            $("#ddlBMode").val(data1[i].Mode);
                            var IsActive = data1[i].IsActive;
                            var Extend = data1[i].ShowExtend;
                            if (IsActive == "1")
                                $("#chkISActive").attr('checked', true);
                            else
                                $("#chkISActive").attr('checked', false);
                            if (IsActive == "1")
                                $("#chkShowExted").attr('checked', true);
                            else
                                $("#chkShowExted").attr('checked', false);

                            var Tid = data1[i].Tid;
                            $("#hdnRID").val(Tid);
                        });
                    }
                    else {
                        if (res == "-1") {
                            alert("Your login session has expired.");
                            window.location = "SessionOut.aspx";
                        }
                        else if (res == "0") {
                            alert("Error Occured at server!!! Try again later");
                        }
                        else {
                            $("#txtRName").val("");
                            $("#ddlRelationtype").val("0");
                            $("#ddlTDocType").val("0");
                            $("#ddlSDOC1").val("0");
                            $("#ddlSource2").val(0);
                            $("#chkShowExted").attr('checked', false)
                            $("#chkISActive").attr('checked', false)
                            $("#hdnRID").val("0");
                        }
                    }
                    $("#dvRMessage").hide();
                    $("#dvrContent").show();
                    $("#dvloader").hide();

                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
            return false;
        }
        function closeDialog(ret) {

            $("#dialog").dialog("close");
            //setTimeout("closeDialog()", 100);
        }
        //Change By Mayank
        function ValidateBal() {
            debugger;
            var ret = true;
            var Msg = "Error(s) in your submission.\n-----------------------\n"

            //if($("#ContentPlaceHolder1_ddlBMode").val()=="0")
            //{
            //    ret=false;
            //    Msg=Msg + "Balance Maintenance Mode Required.\n";
            //}

            if ($("#ContentPlaceHolder1_ddlEDate").val() == "0") {
                ret = false;
                Msg = Msg + "Effective Date Field required .\n";
            }
            if ($("#ContentPlaceHolder1_ddlEAmountField").val() == "0") {
                ret = false;
                Msg = Msg + "Effective Amount Field rrequired.\n";
            }
            if ($("#ContentPlaceHolder1_ddlRel_doc_type").val() == "0") {
                ret = false;
                Msg = Msg + "Relation Doc Name rrequired.\n";
            }
            if ($("#ContentPlaceHolder1_ddlAction").val() == "0") {
                ret = false;
                Msg = Msg + "Action Name rrequired.\n";
            }
            if ($("#ContentPlaceHolder1_ddlSource").val() == null) {
                ret = false;
                Msg = Msg + "Source rrequired.\n";
            }
            if ($("#ContentPlaceHolder1_ddlSTarget").val() == null) {
                ret = false;
                Msg = Msg + "Target rrequired.\n";
            }
            if (ret == false)
                alert(Msg);
            return ret;
        }
    </script>
       <script type="text/javascript">
           var selTab;
           $(function () {
               var tabs = $("#tabs").tabs({
                   show: function () {

                       //get the selected tab index  
                       selTab = $('#tabs').tabs('option', 'selected');
                       alert(selTab)
                   }
               });

           });;
           $(function () {
               $(".btnDyn").button()
           });

           function pageLoad(sender, args) {

               if (args.get_isPartialLoad()) {
                   $("#tabs").tabs({
                       show: function () {

                           //get the selected tab index on partial postback  
                           selTab = $('#tabs').tabs('option', 'selected');
                           alert(selTab)
                       }, selected: selTab
                   });

                   $(function () {
                       $(".btnDyn").button()
                   });
               }

           };

     </script>
    <script type='text/javascript'>
        function pageLoad() { // this gets fired when the UpdatePanel.Update() completes
            //     ReBindMyStuff();
            //     window.alert('partial postback');
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

        window.onunload = function () {
            window.opener.location.reload();
        };

</script>
         <script  type="text/javascript">
             function ToggleVisible(ddl) {
                 var div1 = document.getElementById('div_AutoInvLT_Inv');
                 var div2 = document.getElementById('div_AutoInvLT_SD');
                 var value = ddl.options[ddl.selectedIndex].value;
                 debugger;
                 if (value == "Invoice") {
                     div2.style.display = "none";
                     div1.style.display = "block";
                 }
                 else if (value == "Security Deposite") {
                     div2.style.display = "block";
                     div1.style.display = "none";
                 }
             }

         </script>
    <script type="text/javascript">
        $(function () {
            $("#tabs").tabs();
        });
                </script>
     <script type="text/javascript">
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
     
   
    </div>
</asp:Content>

