<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="UserwiseAnalysisReport_VMS, App_Web_sifhu5tb" viewStateEncryptionMode="Always" %>
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
         $("#<%=txtsdate.ClientID%>").change(function () {
             var txtsdate = new Date($("#<%=txtsdate.ClientID%>").val());

                       var tdate = new Date(txtsdate.getFullYear(), txtsdate.getMonth() + 1, 0);

                       $("#<%=txtedate.ClientID%>").val(tdate.format("yyyy-MM-dd"));

                   });

     });
         function pageLoad(sender, args) {
             if ($("#<%=txtsdate.ClientID%>").val() != "") {
             var txtsdate = new Date($("#<%=txtsdate.ClientID%>").val());

             var tdate = new Date(txtsdate.getFullYear(), txtsdate.getMonth() + 1, 0);

             $("#<%=txtedate.ClientID%>").val(tdate.format("yyyy-MM-dd"));
         }

     }
     function BindGrid() {
         $("#DivRpt").html("");
         $("#Layer1").show();

         var obj = {};
         
         obj.Startdate = new Date($("#<%=txtsdate.ClientID%>").val()).format("yyyy-MM-dd");
         obj.Enddate = new Date($("#<%=txtedate.ClientID%>").val()).format("yyyy-MM-dd");
         obj.UID = <%= Session("UID")%>
         obj.Userrole = "<%= Convert.ToString(Session("USERROLE"))%>";
         $.ajax({
             type: "post",
             url: "UserwiseAnalysisReport_VMS.aspx/GetReportData",
             contentType: "application/json; charset=utf-8",
             dataType: "json",
             data: JSON.stringify(obj),
             success: function (msg) {
                 if (msg.d.ErrMessage != "") {
                     $('#<%=lblmsg.ClientID%>').html(msg.d.ErrMessage);
                         //$("#lblmsg").text =
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
                                                 VRF_No:{type:"string"},
                                                 Approving_Authority: { type: "string" },
                                                 Vehicle_No: { type: "string" },
                                                 Date_of_Allotment: { type: "string" },
                                                 //Committed_Kms: { type: "string" },
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
                                                  //templet: '<a href="VehicleDetails.aspx?vehicle No=#Vehicle_No#&Start Date=#Start_Date#&End Date=#End_Date#>#Vehicle_No#</a>'
                                              }, {
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
                                              }
                                              //, {
                                              //    field: "Committed_Kms",
                                              //    title: "Committed Kms"
                                              //}
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

                                              //{ command: { text: "View Details", click: showDetails } }
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

                          <td width="13%" align="right" >
                              &nbsp;</td>

                          <td width="10%" align="right" class="k-label" >
                             From Date
                            
                                 </td>

                          <td width="20%" >
                              <asp:TextBox 
                                  ID="txtsdate" runat="server" CssClass="k-textbox"  ></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="yyyy-MM-dd"    runat="server" /> 
                            
                                 </td>
                              <td width="10%" align="right" class="k-label">To Date
                                  </td>
                              <td width="20%">  
                              <asp:TextBox ID="txtedate"  runat="server" Enabled="False" CssClass="k-textbox"     ></asp:TextBox>
                                 <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd" runat="server" /> </td>
                              <td width="10%">  
                                                                         
                                       <asp:ImageButton ID="btnshow" runat="server" Height="25px" CssClass="k-button-bare" ImageUrl="~/images/showbutton.png" ToolTip="Search"
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


