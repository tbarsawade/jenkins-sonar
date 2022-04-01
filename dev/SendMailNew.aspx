<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="false" debug="True"  CodeFile="SendMailNew.aspx.vb" Inherits="SendMailNew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%--  <script src="js/gridviewScroll.min.js"></script>--%>
    <script src='https://code.jquery.com/jquery-latest.min.js' type='text/javascript'> </script>

   
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
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>



  

   <script type="text/javascript">
       $(document).ready(function () {
           $('#txtBcc').show().toggleClass('hidden');;
               BindTemplate();
               BindGrid();
           });
       var asub;
       var bbbc;
       var cmsgbody;
       var Mailqry;
       function BindTemplate() {
           var ddlDocumentType = $("#ddltemplate");
           var strData = "";
           var t = '{ documentType: ""}';
           $.ajax({
               type: "POST",
               url: "SendMailNew.aspx/GetTemplate",
               contentType: "application/json; charset=utf-8",
               data: t,
               dataType: "json",
               success: function (response) {
                   strData = JSON.parse(response.d.Data);
                   /*ddlDocumentType.empty().append($('<option></option>').val("").html("-- Select --"));*/
                   if (strData.length > 0) {
                       $.each(strData, function () {
                           ddlDocumentType.append($('<option></option>').val(this.TID).html(this.TEMPLATE_NAME));
                       });
                       asub = $("#txtSubject").val(strData[0].subject);
                       bbbc = $("#txtBcc").val(strData[0].BCC);
                       cmsgbody = strData[0].msgBody;
                       Mailqry = strData[0].qry;
                       cd = $("#ContentPlaceHolder1_txtBody");
                   }
                   else {
                       //ddlDocumentType.empty().append($('<option></option>').val("").html("-- Select --"));
                       //$("#dvloader").hide();
                   }

               },
               error: function (data) {
                   //Code For hiding preloader
               }
           });
           //$("#ddltemplate").change(function () {
                


           //});
       }
       function BindGrid() {
         
           var url = "SendMailNew.aspx/Getdata";
           //var typepar = '@ViewBag.Type';

           var kgridMaker = "";
           var Columns = "";
           if (kgridMaker != "") {
               $('#kgrid').kendoGrid('destroy').empty();
           }
           kgridMaker = $("#kgrid").kendoGrid({
               dataSource: {
                   dataType: "json",
                   transport: {
                       read: {
                           url: url,
                           dataType: "json",
                           type: "POST",
                           contentType: "application/json; charset=utf-8",
                       },

                   },
                  
                   pageSize: 50,
                   pageable: {
                       pageSize: 50
                   },
                   schema: {
                       model: {
                           fields: {
                               UserName: { type: "string" },
                               Emailid: { type: "string" },
                               UserID: { type: "string" },
                               userRole: { type: "string" },
                           }
                       },
                       data: function (data) {
                           Columns = data.d.Column;
                           var res = $.parseJSON(data.d);
                           return res || [];
                       },


                   },

               },
               //dataBound: onDataBound,
               Columns: Columns,
               pageable:true,
               pageable: { pageSizes: true, refresh: true },
               noRecords: true,
               groupable: false,
               resizable: true,
               height: 500,
               filterable: true,
               toolbar: [{ name: "excel" }],
               excel: {
                   fileName: "Users.xlsx",
                   filterable: true,

                   allPages: true
               },
               sortable: {
                   mode: "multiple"
               },
               pageable: {
                   pageSizes: true,
                   refresh: true
               },
               columns: [

                   { title: "<input id='chkAll' class='checkAllCls' type='checkbox'/>", width: "35px", template: "<input type='checkbox' class='check-box-inner' />", filterable: false },
                   { field: "UserName", title: "UserName", width: 100 },
                   { field: "Emailid", title: "Emailid", width: 100 },
                   { field: "UserID", title: "UserID", width: 100 },
                   { field: "userRole", title: "userRole", width: 150 },

               ]
           });
           if (kgridMaker != "") {

               //$('.aaf').on("click", function () {
               //    alert("Filter");
               //})

               $(".checkAllCls").on("click", function () {
                   var ele = this;
                   var state = $(ele).is(':checked');
                   var grid = $('#kgrid').data('kendoGrid');
                   if (state == true) {
                       $('.check-box-inner').prop('checked', true);
                   }
                   else {
                       $('.check-box-inner').prop('checked', false);
                   }
               });
               
               $('.aaf').on("click", function () {
                   if ($("#ddltemplate").val() == "") {
                       alert("Please select Template");
                       $("#ddltemplate").focus();
                       return true;
                   }
                   if ($("#txtSubject").val() == "") {
                       alert("Please fill subject");
                       $("#txtSubject").focus();
                       return true;
                   }

                   var grid = $("#kgrid").data("kendoGrid");
                   var ds = grid.dataSource.data();
                   var lstdata = [];
                   var arrerror = [];
                  
                   for (var i = 0; i < ds.length; i++) {
                       var row = grid.table.find("tr[data-uid='" + ds[i].uid + "']");
                       var checkbox = $(row).find(".check-box-inner");
                       if (checkbox.is(":checked")) {
                           var idstosend = {UID:0, UserName: "", Emailid: "", UserID: "", userRole: "" };
                           idstosend.UserName = ds[i].UserName;
                           idstosend.Emailid = ds[i].Emailid;
                           idstosend.UserID = ds[i].UserID;
                           idstosend.userRole = ds[i].userRole;
                           idstosend.UID = ds[i].UID;
                           lstdata.push(idstosend);
                       }
                       else {

                           arrerror.push(true);
                       }
                   }

                   if (lstdata.length > 0) {
                       if (arrerror.length > 0) {
                           for (var i = 0; i < arrerror.length; i++) {
                               if (arrerror[i] == false) {
                                   return false;
                               }
                           }
                       }
                       var txtbody = $("#txtBody").val();
                       var txtsub = $("#txtSubject").val();
                       var cbbc = $("#txtBcc").val()
                       var ddltemplate = $("#ddltemplate").val();
                      
                      /*// ClsNextUser: lstdata,*/
                       //var temp = { subj: txtsub, bcc: cbbc,Mailqry: Mailqry};
                       var temp = { ClsNextUser: lstdata, MSG: cmsgbody, subj: txtsub, bcc: cbbc, Mailqry: Mailqry };
                       $.ajax({
                           type: "POST",
                           url: "SENDMAILNEW.ASPX/sendnewmail",
                           contentType: "application/json; charset=utf-8",
                           data:JSON.stringify(temp),
                           dataType: "json",
                           success: function (response) {
                               var msg = response.d;
                               alert(msg);
                               $("#dvloaderSave").hide();
                               window.location = 'SendMailNew.aspx';
                           },
                           error: function (data) {
                               //Code For hiding preloader
                           }
                       });
                   }
                   else {
                       alert("please select at-least one User.");
                   }
               });
           }
       }

       
       
   </script>    
   
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
 <Triggers>
      <%--  <asp:PostBackTrigger ControlID="btnSearch"  />
       <asp:PostBackTrigger ControlID="btnAdvanceSearch" />
       <asp:PostBackTrigger ControlID="btnDynamicSearch" />--%>
        </Triggers>
<ContentTemplate>
 

    <div class="container-fluid">
                <div class="form">

                    <div class="doc_header">
                        Send Bulk Mail
                    </div>



     <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                
                <tr style="color: #000000">
                    <td style="text-align: center;">
                        <asp:Label ID="lblMsg1" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"></asp:Label>
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td class="top_head">
                        <table cellspacing="2px" cellpadding="2px" width="100%">
             
                <tr>
                    <td colspan="2">

                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="form">
                                    <table cellspacing="2px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                              <td align="left" style="width:200px;"><asp:label id="lbltemp" text="Template:" runat="server" CssClass="label" ></asp:label>  </td>
                                            <td align="left" width="400px">
                                                <%--<asp:DropDownList ID="ddltemplate"  runat="server" Width="182px" CssClass="txtBox_pop" AutoPostBack="true">
                                                </asp:DropDownList>--%>
                                                <select id="ddltemplate" class="txtBox">
                                                
                                             </select>
                                            </td>
                                            <td align="left" style="width:150px;"> </td>
                                            <td align="left"style="width:750px;">  </td>
                                        </tr>
                                        <%--<tr>
                                            RepeatColumns="6"
                                                <asp:DropDownList ID="ddlnt" runat="server" Width="182px" Style="display: none;" CssClass="txtBox">
                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                    <asp:ListItem Value="1">No Signal Alert</asp:ListItem>
                                                    <asp:ListItem Value="2">Trip Alert</asp:ListItem>
                                                    <asp:ListItem Value="3">SMS Alert</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>--%>
                                        <tr>
                            <td align="left">&nbsp;&nbsp;&nbsp;</td>
          <td align ="left">
                </td>
                                        </tr>
                                        <tr>
                                            <td align="left"> <asp:label id="lblbcc" Text="BCC:"  runat="server" Visible="false" CssClass="label" ></asp:label></td>
                                            <td align="left" colspan="5">
                                                <%--<asp:TextBox ID="txtBcc" CssClass="txtBox_pop" runat="server" Visible="false" Width="99%"></asp:TextBox>--%>
                                                <input type="text" id="txtBcc" class="txtBox" style="width:100%" />

                                            </td>
                                            <%--<td align="center" valign="top">
                                                <asp:CheckBox ID="chkPublicView" runat="server" Text="Documnet Public View" AutoPostBack="true" /></td>--%>
                                        </tr>

                                        <tr>
                                            <td align="left" ><asp:Label Text="Mail Subject:" ID="lblsubject" runat="server" CssClass="label" ></asp:Label></td>
                                            <td align="left" colspan="5">
                                                <input type="text" id="txtSubject" class="txtBox" style="width:100%" />
                                                <%--<asp:TextBox ID="txtSubject"  CssClass="txtBox_pop" Enabled="false" runat="server" Width="99%"></asp:TextBox></td>--%>
                                             <%--<td><asp:Label ID="lblLnkExpiryDate" runat="server" Text="Link Expiry days" ></asp:Label><asp:TextBox id="txtLnkexpdate" runat="server" Width="35Px" CssClass="txtBox" ></asp:TextBox>  </td>--%>
                                        </tr>
                                         <tr>
                            <td align="left">&nbsp;&nbsp;&nbsp;</td>
          <td align ="left">
                </td>
                                        </tr>
                                        <tr>

                                            <td align="left"> <asp:label ID="lblulist" CssClass="label" runat="server" Text="User&#39;s List:"></asp:label></td>
                                            <td align="left" valign="bottom" colspan="5">
                                                <asp:Panel ID="Panel2" runat="server" Height="600px" Width="100%"  ClientIDMode="Static"   Style="display: block">
                                                 <div class="row">
                                                     <div class="col-md-12 col-sm-12">
                                                         <div align="center" style="width: auto">
                                                            <div style="max-width: 1320px; overflow-x: scroll;">
                                                             <div id="kgrid" style="height: unset;">
                                                              </div>
                                                                <div id="dvgrid"></div>
                                                          </div>
                                                         </div>
                                                </div>
                                                     <div class="col-md-12 col-sm-12">
                                                         <div class="pull-right">
                                                        <a href="javascript:void(0)" id="liApprove" class="submit_btn aaf">Send Mail</a>
                                                        <asp:Button ID="btnActEdit" runat="server" CssClass="submit_btn" OnClick="ALErtHit" Visible="false" Font-Bold="True" Text="Send Mail" Width="98px" />
                                                    </div>
                                                     </div>
                                                </div>
                                                    <asp:CheckBoxList ID="DDLCC" RepeatLayout="table"  runat="server" Width="98%"  CssClass="txtBox_pop"></asp:CheckBoxList>
                                                </asp:Panel>
                                            </td>
                                            
                                        </tr>
                                        <%-- <tr>
                                            <td align="right" valign="bottom" colspan="5">
                                           
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td align="left" colspan="6" style="height: 250px">

                                                <asp:Panel ID="pnlScrol" runat="server" ScrollBars="Auto" Height="250px">
                                                 <asp:TextBox ID="txtBody" TextMode="MultiLine" Visible="false" runat="server" Width="100%" Height="98%" >
                                                    </asp:TextBox>
                                                    <%--<textarea type="text"  id="txtbody1" runat="server"></textarea>--%>
                                                    
                                                    <asp:HtmlEditorExtender ID="HEE_body" runat="server" DisplaySourceTab="TRUE" EnableSanitization="false"  TargetControlID="txtbody" ></asp:HtmlEditorExtender>

                                                </asp:Panel>
                                            </td>
                                        </tr>

                                       
                                </div>
                                </td>
  </tr>
  </table>
</div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
                        
    <asp:Button ID="btnShowPopupDelete" runat="server"  style="display:none;" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete"
        CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" Style="Display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Send Mail : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="97%" Font-Size="Medium" ></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelete" runat="server" Text="Shoot Mail" Width="90px"
                                        OnClick="getallrecords" CssClass="submit_btn" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>

                    </td>
                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left;" valign="top">
                        
                        &nbsp;</td>
                </tr>
            </table> 

  
                      </div>
        </div>
    </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

