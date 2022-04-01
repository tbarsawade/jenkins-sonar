<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="Userwise_AnalysisRpt2, App_Web_ws3kahym" viewStateEncryptionMode="Always" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
   
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
      <script src="http://cdn.kendostatic.com/2014.3.1029/js/jszip.min.js"></script>
 <script >
     $(document).ready(function () {



     });
     function pageLoad(sender, args) {

         $("#<%=txtsdate.ClientID%>").change(function () {
             var SMonth = new Date($("#<%=txtsdate.ClientID%>").val()).getMonth() + 1;
             if (SMonth > new Date().getMonth() + 1) {
                 alert("Future date is not allowed!");
                 $('#<%=txtsdate.ClientID%>').val("");
             }
             else if (new Date().getMonth() + 1 - SMonth > 3) {
                 alert("Please select month from last 3 previous months only!");
                 $('#<%=txtsdate.ClientID%>').val("");
                       }



         });

               $("#<%=txtedate.ClientID%>").change(function () {
             var SMonth = new Date($("#<%=txtsdate.ClientID%>").val()).getMonth() + 1;
             var EMonth = new Date($("#<%=txtedate.ClientID%>").val()).getMonth() + 1;
             if (EMonth > new Date().getMonth() + 1) {
                 alert("Future date is not allowed!");
                 $('#<%=txtedate.ClientID%>').val("");
                    $('#<%=lblmsg.ClientID%>').html("Please select ");
             }
             else if (new Date().getMonth() + 1 - SMonth > 3) {
                 alert("Please select month from last 3 previous months only!");
                 $('#<%=txtedate.ClientID%>').val("");
             }
             else if (SMonth > EMonth) {
                 alert("Endate cannot be greater than Fromdate! ");
                 $('#<%=txtedate.ClientID%>').val("");
             }


             //  $("#<%=txtedate.ClientID%>").val(tdate.format("yyyy-MM-dd"));

         });
     }




     function BindGrid() {
         if ($("#<%=ddlUsers.ClientID%>").val() != "Select User") {
             if ($("#<%=txtsdate.ClientID%>").val() != "") {
                 if ($("#<%=txtedate.ClientID%>").val() != "") {
                     $("#DivRpt").html("");
                     $("#Layer1").show();

                     var obj = {};
                     obj.Startdate = new Date($("#<%=txtsdate.ClientID%>").val()).format("yyyy-MM-dd");
                     var txttdate = new Date($("#<%=txtedate.ClientID%>").val());
                     obj.Enddate = new Date(txttdate.getFullYear(), txttdate.getMonth() + 1, 0).format("yyyy-MM-dd");
                     obj.UID = <%= Session("UID")%>
         obj.Userrole = "<%= Convert.ToString(Session("USERROLE"))%>";
                     obj.UserID = $("#<%=ddlUsers.ClientID%>").val();
                     $.ajax({
                         type: "post",
                         url: "Userwise_AnalysisRpt2.aspx/GetReportData",
                         contentType: "application/json; charset=utf-8",
                         dataType: "json",
                         data: JSON.stringify(obj),
                         success: function (msg) {
                             if (msg.d.ErrMessage != "") {
                                 $('#<%=lblmsg.ClientID%>').html(msg.d.ErrMessage);
                             }

                             else {
                                 $('#<%=lblmsg.ClientID%>').html('');
                                 if (msg.d.Success = true) {
                                     var t2 = JSON.parse(msg.d.Data);

                                     $("#DivRpt").kendoGrid({

                                         dataSource: {
                                             data: t2,
                                             schema: {
                                                 model: {
                                                     fields: {

                                                         Name_of_User: { type: "string" },
                                                         User_Department: { type: "string" },
                                                         VRF_No: { type: "string" },
                                                         Approving_Authority: { type: "string" },
                                                         Vehicle_No: { type: "string" },
                                                         Date_of_Allotment: { type: "string" },
                                                         Bill_Period: { type: "string" },
                                                         Monthly_Rate: { type: "string" },
                                                         Kms_Run: { type: "string" },
                                                         Actual_Cost_in_Rs: { type: "string" },
                                                         Kms_Run_as_per_GPS: { type: "string" },

                                                     }
                                                 }
                                             },
                                             pageSize: 20
                                         },
                                         height: 430,
                                         scrollable: true,
                                         resizable: true,
                                         reorderable: true,
                                         groupable: true,
                                         sortable: true,
                                         filterable: true,
                                         pageable: true,
                                         columns: [
                                                      {
                                                          field: "Name_of_User",
                                                          title: "Name of User"

                                                      }, {
                                                          field: "User_Department",
                                                          title: "Department"

                                                      }
                                                      , {
                                                          field: "VRF_No",
                                                          title: "VRF No.",

                                                      }, {
                                                          field: "Approving_Authority",
                                                          title: "Approving Authority",

                                                      }
                                                      , {
                                                          field: "Vehicle_No",
                                                          title: "Vehicle No",

                                                      }
                                                      , {
                                                          field: "Date_of_Allotment",
                                                          title: "Date of Allotment"
                                                      }, {
                                                          field: "Bill_Period",
                                                          title: "Bill Period"
                                                      }
                                                      ,
                                                      {
                                                          field: "Monthly_Rate",
                                                          title: "Monthly Rate"
                                                      },
                                                      {
                                                          field: "Kms_Run",
                                                          title: "Kms Run"
                                                      },
                                                      {
                                                          field: "Actual_Cost_in_Rs",
                                                          title: "Actual Cost in Rs."
                                                      },
                                                      {
                                                          field: "Kms_Run_as_per_GPS",
                                                          title: "Kms Run as per GPS"
                                                      },

                                         ]
                                     });
                                 }
                                 else { $("#lblmsg").text = "No Record found!" }
                             }



                             $("#Layer1").hide();

                             return false;
                         },
                         error: function (data) {
                             $("#Layer1").hide();
                             alert("Error");

                             return false;
                         }

                     });
                     return false;
                 }
                 else {
                     alert("Please enter 'To Month'!");
                     return false;
                 }
             }
             else { alert("Please enter 'From Month'!"); return false; }
         }
         else { alert("Please select a 'User'!"); return false; }

     }
     function onCalendarShown() {

         var cal = $find("calendar1");
         //Setting the default mode to month
         cal._switchMode("months", true);

         //Iterate every month Item and attach click event to it
         if (cal._monthsBody) {
             for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                 var row = cal._monthsBody.rows[i];
                 for (var j = 0; j < row.cells.length; j++) {
                     Sys.UI.DomEvent.addHandler(row.cells[j].firstChild, "click", call);
                 }
             }
         }
     }

     function onCalendarHidden() {
         var cal = $find("calendar1");
         //Iterate every month Item and remove click event from it
         if (cal._monthsBody) {
             for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                 var row = cal._monthsBody.rows[i];
                 for (var j = 0; j < row.cells.length; j++) {
                     Sys.UI.DomEvent.removeHandler(row.cells[j].firstChild, "click", call);
                 }
             }
         }

     }
     function call(eventElement) {
         var target = eventElement.target;
         switch (target.mode) {
             case "month":
                 var cal = $find("calendar1");
                 cal._visibleDate = target.date;
                 cal.set_selectedDate(target.date);
                 cal._switchMonth(target.date);
                 cal._blur.post(true);
                 cal.raiseDateSelectionChanged();
                 break;
         }
     }
     function onCalendarShown2() {

         var cal = $find("calendar2");
         //Setting the default mode to month
         cal._switchMode("months", true);

         //Iterate every month Item and attach click event to it
         if (cal._monthsBody) {
             for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                 var row = cal._monthsBody.rows[i];
                 for (var j = 0; j < row.cells.length; j++) {
                     Sys.UI.DomEvent.addHandler(row.cells[j].firstChild, "click", call2);
                 }
             }
         }
     }

     function onCalendarHidden2() {
         var cal = $find("calendar2");
         //Iterate every month Item and remove click event from it
         if (cal._monthsBody) {
             for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                 var row = cal._monthsBody.rows[i];
                 for (var j = 0; j < row.cells.length; j++) {
                     Sys.UI.DomEvent.removeHandler(row.cells[j].firstChild, "click", call2);
                 }
             }
         }

     }
     function call2(eventElement) {
         var target = eventElement.target;
         switch (target.mode) {
             case "month":
                 var cal = $find("calendar2");
                 cal._visibleDate = target.date;
                 cal.set_selectedDate(target.date);
                 cal._switchMonth(target.date);
                 cal._blur.post(true);
                 cal.raiseDateSelectionChanged();
                 break;
         }
     }

 </script>
  
   
                            <div style="text-align:center"> <h1 style="color:black"> VMS Data Analysis Report </h1></div>             
                          <fieldset style="width:100%;text-align:left">
                          <legend style=" color:Black; text-align:left;font-weight:bold;">Date Range</legend>
                         
                          <table width="100%">
                          <tr>

                          <td width="13%" align="right" >
                              &nbsp;</td>

                          <td width="10%" align="right" >
                              &nbsp;</td>

                          <td width="20%" >
                              &nbsp;</td>
                              <td width="10%">&nbsp;</td>
                              <td width="20%">  
                                  &nbsp;</td>
                              <td width="10%">  
                                  &nbsp;</td>
                              <td width="12%">  
                                  <asp:ImageButton ID="btnExportPDF" runat="server" Height="18px" ImageAlign="Right" ImageUrl="~/images/export.png" ToolTip="Export PDF" Width="18px" />
                                  &nbsp;&nbsp;
                                  <asp:ImageButton ID="btnexportxl" runat="server" Height="18px" ImageAlign="Right" ImageUrl="~/Images/excelexpo.jpg" ToolTip="Export EXCEL" Width="18px" />
                              </td>
                              </tr>
                                
                              
                                
                             
                          <tr>

                          <td width="13%" align="right" style="height: 20px" >
                              </td>

                          <td width="10%" align="right" class="k-label" style="height: 20px" >
                              Select User</td>

                          <td width="20%" style="height: 20px" >
                              <asp:DropDownList ID="ddlUsers" runat="server" CssClass="k-dropdown-wrap k-state-border-up" Width="150px">
                              </asp:DropDownList>
                            
                                 </td>
                              <td width="10%" align="right" class="k-label" style="height: 20px"></td>
                              <td width="20%" style="height: 20px">  
                                  </td>
                              <td width="10%" style="height: 20px">  
                                                                         
                                       </td>
                              <td width="12%" style="height: 20px">  
                                  </td>
                              </tr>
                                
                              
                                
                             
                          <tr >

                          <td width="13%" align="right" >
                              &nbsp;</td>

                          <td width="10%" align="right" class="k-label" >
                              From Month </td>

                          <td width="20%" >
                              <asp:TextBox 
                                  ID="txtsdate" runat="server" CssClass="k-textbox" Width="150px"  ></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="MMM,yyyy"    runat="server" OnClientHidden="onCalendarHidden"  OnClientShown="onCalendarShown" BehaviorID="calendar1" DefaultView="Months" /> 
                            
                                 </td>
                              <td width="10%" align="right" class="k-label">To Month
                                  </td>
                              <td width="20%">  
                              <asp:TextBox ID="txtedate"  runat="server"  CssClass="k-textbox" Width="150px"     ></asp:TextBox>
                                 <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" Format="MMM,yyyy" runat="server"  OnClientHidden="onCalendarHidden2"  OnClientShown="onCalendarShown2" BehaviorID="calendar2"  DefaultView="Months" /> </td>
                              <td width="10%">  
                                                                         
                                       <asp:ImageButton ID="btnshow" runat="server" Height="25px" CssClass="button" ImageUrl="~/images/showbutton.png" ToolTip="Search"
                                            Width="50px" OnClientClick="javascript:return BindGrid();"/>
                
                              </td>
                              <td width="12%">  
                                  &nbsp;</td>
                              </tr>
                                
                              
                                
                             
                          <tr>

                          <td width="13%" align="right" >
                              &nbsp;</td>

                          <td width="10%" align="right" >
                              &nbsp;</td>

                          <td width="20%" >
          <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"  ></asp:Label>
                              </td>
                              <td width="10%">&nbsp;</td>
                              <td width="20%">  
                                  &nbsp;</td>
                              <td width="10%">  
                                  &nbsp;</td>
                              <td width="12%">  
                                  &nbsp;</td>
                              </tr>
                                
                              
                                
                             
                              </table>
                                </fieldset><br />
    <div id="DivRpt"> </div>
    <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%;display:none;">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>

                        

</asp:Content>

