<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="PlanVsActualTimeBased, App_Web_cjg31vo3" viewStateEncryptionMode="Always" %>

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
<%--<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js">--%>
   
<script type="text/javascript">

    function toggleDiv(divId) {
        $("#" + divId).toggle();
    }
    function HideDiv(divId) {
        $("#" + divId).Hide();
    }



    function BindGrid() {
        $("#DivRpt").html("");
        $("#Layer1").show();

        var t1 = $("#ContentPlaceHolder1_txtsdate").val();
        var t2 = $("#ContentPlaceHolder1_txtedate").val();
        var eid = $("#ContentPlaceHolder1_hdnEid").val();
        var ddloption = $('#<%=ddlReportType.ClientID%>').val();
        var FuncUrl = "PlanVsActualTimeBased.aspx/GetReportData"
        if (ddloption == "Time Based") {
            var FuncUrl = "PlanVsActualTimeBased.aspx/GetReportData"
            var t = GetControlData();
            $.ajax({
                type: "post",
                url: FuncUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{"str":' + JSON.stringify(t) + '}',
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

                                            Name: { type: "string" },
                                            Device_IMEI_No: { type: "string" },
                                            Planned_Site_Name: { type: "string" },
                                            Planned_DateTime: { type: "string" },
                                            Visit_Sequence: { type: "number" },
                                            Status: { type: "string" },

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
                                             field: "Name",
                                             title: "Name"

                                         }, {
                                             field: "Device_IMEI_No",
                                             title: "Device IMEI_No"
                                             //templet: '<a href="VehicleDetails.aspx?vehicle No=#Vehicle_No#&Start Date=#Start_Date#&End Date=#End_Date#>#Vehicle_No#</a>'
                                         }, {
                                             field: "Planned_Site_Name",
                                             title: "Planned Site Name",

                                         }
                                         , {
                                             field: "Planned_DateTime",
                                             title: "Planned DateTime",

                                         }
                                         , {
                                             field: "Visit_Sequence",
                                             title: "Visit Sequence"
                                         }
                                         , {
                                             field: "Status",
                                             title: "Status"
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
                alert("Error");
                return false;
            }

        });
        return false;
    }
    else {
            var FuncUrl = "PlanVsActualTimeBased.aspx/GetReportDataSequence"
        var t = GetControlData();
        $.ajax({
            type: "post",
            url: FuncUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{"str":' + JSON.stringify(t) + '}',
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

                                                Name: { type: "string" },
                                                Device_IMEI_No: { type: "string" },
                                                Planned_Date: { type: "string" },
                                                Planned_Site_Name: { type: "string" },
                                                Planned_Sequence: { type: "string" },
                                                Actual_Sequence: { type: "string" },
                                                Actual_Visit_Date: { type: "string" },
                                                Actual_Visit_Time: { type: "string" },




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
                                                 field: "Name",
                                                 title: "Name"

                                             }, {
                                                 field: "Device_IMEI_No",
                                                 title: "Device IMEI_No"
                                                 //templet: '<a href="VehicleDetails.aspx?vehicle No=#Vehicle_No#&Start Date=#Start_Date#&End Date=#End_Date#>#Vehicle_No#</a>'
                                             },
                                             {
                                                 field: "Planned_Date",
                                                 title: "Planned Date",

                                             },
                                             {
                                                 field: "Planned_Site_Name",
                                                 title: "Planned Site Name",

                                             }
                                             , {
                                                 field: "Planned_Sequence",
                                                 title: "Planned Sequence",

                                             },
                                                {
                                                    field: "Actual_Sequence",
                                                    title: "Actual Sequence",

                                                }

                                             , {
                                                 field: "Actual_Visit_Date",
                                                 title: "Actual Visit Date"
                                             }
                                             ,
                                             {
                                                 field: "Actual_Visit_Time",
                                                 title: "Actual Visit Time"
                                             }
                                              ,
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
                    alert("Error");
                    return false;
                }

            });

            return false;
        }
    }


    function GetControlData() {
        var controls = $('[IsSearch]');
        var result = {};
        var Key = "";
        $.each(controls, function (i, e) {
            var type = $(e).attr('data-ty');
            Key = $(e).attr('fld');
            switch (type) {
                case "DATETIME":
                    result[Key] = $(e).val();
                    break;
                case 'NUMERIC':
                    result[Key] = $(e).val();
                    break;
                case 'MULTISELECT':
                    var vals = [];
                    $('#' + $(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
                        vals.push($(this).attr('value'));
                    });
                    result[Key] = vals;
                    break;
                case 'SINGLESELECT':
                    $('#' + $(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
                        result[Key] = $(this).attr('value');
                    });
                    break;
                default:
                    break;
            }
        });
        return result;
    }


    function Export(divid) {

        var grid = $("#" + divid).html();

        //  var data1 = $(divid + " #" + id).html();
        var data1 = $("#" + divid).find('tbody').html();
        var header = $("#" + divid).find('thead').html();

        $("#" + divid + " table thead tr").each(function () {
            //$(this).find("th:first").remove();
            $(this).find("th:last").remove();

        });

        $("#" + divid + " table tbody tr").each(function () {
            // $(this).find("td:first").remove();
            $(this).find("td:last").remove();
        });

        $("#" + divid + " table thead tr").css('background-color', '#CCCCB2');

        var data = $("#" + divid).find('tbody').html();
        $("#" + divid).find('tbody').html(data1);
        // $("#" + id).html(data1);
        data = header + data;
        data = escape(data);

        $('body').prepend("<form method='post' action='exportPage.aspx' style='top:-3333333333px;' id='tempForm'><input type='hidden' name='data' value='" + data + "' ><input type='hidden' name='filename' value='PlanVsActualTimeBased' ><input type='hidden' name='ExportType' value='Excel' ></form>");
        $('#tempForm').submit();
        $("tempForm").remove();
        return false;
    };
</script>
<script src="http://maps.google.com/maps/api/js?sensor=false" 
          type="text/javascript"></script>
   <script type="text/javascript">

       function OpenWindow(url) {

           var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
           //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
           return false;
       }

    </script>

  
<style type="text/css">
.gradientBoxesWithOuterShadows { 
height: 100%;
width: 99%; 
padding: 5px;
background-color: white; 

/* outer shadows  (note the rgba is red, green, blue, alpha) */
-webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4); 
-moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);

/* rounded corners */
-webkit-border-radius: 12px;
-moz-border-radius: 7px; 
border-radius: 7px;

/* gradients */
background: -webkit-gradient(linear, left top, left bottom, 
color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5)); 
background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%); 
background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
}

    .style2
    {
        width: 30%;
    }

</style>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
 <Triggers>
        <%--<asp:PostBackTrigger ControlID="btnSearch"  />--%>
       <asp:PostBackTrigger ControlID="btnExportPDF" />
       <asp:PostBackTrigger ControlID="btnexportxl" />
     <%--<asp:PostBackTrigger ControlID ="btnshow" />--%>
        </Triggers>
<ContentTemplate>
<br />
   <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>
<asp:ImageButton ID="btnExportPDF" ToolTip="Export PDF"  runat="server" Width="18px" ImageAlign="Right"  Height="18px" ImageUrl="~/images/export.png"/>&nbsp;&nbsp;
<asp:ImageButton ID="btnexportxl"  ToolTip="Export EXCEL" ImageAlign="Right"  runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg"
    /> &nbsp;
<div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px">
  <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9"> 
     
          <tr>
          <td>
          <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"  ></asp:Label>
          </td>
          </tr>
          <tr>
                            
              <td align="left" class="style2">
                  <table   >
                      <tr>
                          <td >
                                         
                          <fieldset style="width:326px; height:170px;">
                          <legend style=" color:Black; text-align:center;font-weight:bold;">Date Range</legend>
                         
                          <table>
                          <tr>

                          <td >
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;From Date&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox 
                                  ID="txtsdate" runat="server" IsSearch="1" fld="SDate" data-ty="DATETIME" ></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="yyyy-MM-dd HH:MM"    runat="server" /> <br/><br />
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;To Date&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                              <asp:TextBox ID="txtedate"  runat="server" AutoCompleteType="Search" IsSearch="1" fld="TDate" data-ty="DATETIME" 
                              ></asp:TextBox>
                                 <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd HH:MM" runat="server" /> <br /><br />
                                 </td>
                              </tr>
                                <tr>

                          <td >
                              
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Report Type&nbsp; 
                              <asp:DropDownList ID="ddlReportType" runat="server">
                                  <asp:ListItem>Time Based</asp:ListItem>
                                  <asp:ListItem>Sequence Based</asp:ListItem>
                              </asp:DropDownList>
                                 <asp:CalendarExtender  ID="CalendarExtender3" TargetControlID="txtedate" Format="yyyy-MM-dd HH:MM" runat="server" /> <br /><br />
                                 </td>
                              </tr>
                              
                                  <caption>
                                      <br />
                                      <br />
                              </caption>
                             
                              </table>
                                </fieldset>
                          </td>
                      </tr>
                  </table>
              </td>
                            <td align="Left" width="35%">
                            <fieldset style="height:170px">
                        <legend style=" color:Black; text-align:left;font-weight:bold; ">
                            <asp:CheckBox ID="CheckBox1"  OnCheckedChanged="checkuncheck" AutoPostBack="true"  runat="server" />
                        <asp:Label runat="server" id="lbltxt"></asp:Label>
                        </legend>
                <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" 
                      style=" display:block ">
                      
       
                      <asp:CheckBoxList ID="UsrVeh"    runat="server" IsSearch="1" fld="Vehicles" data-ty="MULTISELECT"  >
               
                      </asp:CheckBoxList>
                      <%--       <asp:CheckBoxList ID="smsreports"   Visible="false"  runat="server">
               
                      </asp:CheckBoxList>--%>
                  </asp:Panel>
                  </fieldset>
              </td>
             
               <td align="center" class="m8b" width="10%">
                                                                         
                                       <asp:ImageButton ID="btnshow" runat="server" Height="25px" CssClass="button" ImageUrl="~/images/showbutton.png" ToolTip="Search"
                                            Width="50px" OnClientClick="javascript:return BindGrid();"/>
                
              </td>
            
          </tr>
            <tr>
          <td colspan="3" style=" width:70%"></td>
          <td  style=" width:30%; text-align:right;">
           
           </td>
          </tr>
          </table>
    </div>
    <div id ="DivRpt"></div>
       <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%;display:none;">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
    <div>
          <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">               
                           <tr>
                                            <td>
                                                <br />
                                                

                  </td>
              </tr>
          </table>
 </div>
</ContentTemplate>
</asp:UpdatePanel>
 </asp:Content>


