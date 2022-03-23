<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="VehicleHaltReport, App_Web_ik1k4di5" viewStateEncryptionMode="Always" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
     <link href="css/GridviewScroll.css" rel="stylesheet" />
       <script src="js/gridviewScroll.min.js"></script>
   
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
        
  
 
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <style type="text/css">
          #mask
        {
            background-color: #F6F6F6;
            z-index: 10000;
            height: 100%;
            width: 800px;
        }
    #loader
        {
            width: 200px;
            height: 200px;
            position: absolute;
            left: 50%;
            top: 50%;
            background-image: url("images/loading.gif");
            background-repeat: no-repeat;
            background-position: center;
            margin: -100px 0 0 -100px;
        }
        .auto-style1
        {
            height: 25px;
        }
        </style>  
    
    
  
      
    <div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px" >
       <asp:UpdatePanel ID="UpdatePanel1" runat="server" > <Triggers>
        <asp:PostBackTrigger ControlID ="btnExcelExport" />
     <asp:PostBackTrigger ControlID ="btnshow" />
        </Triggers><ContentTemplate>
                      
                <%--  <div id="mask" style="height: 10px;">
            <div id="loader" style="height: 50px;">
            </div>
        </div>--%>
         
        <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9" >
            <tr >
                <td >
                    <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"  ></asp:Label>
                </td>
            </tr>
            <tr>
               <td align="left" >
                    <table    >
                        <tr >
                            <td  >
                                <fieldset style="width:326px; height:170px;" >
                                    <legend style=" color:Black; text-align:center;font-weight:bold;">Date Range</legend>
                                    <table >
                                        <tr>
                                            <td><%--&nbsp;&nbsp;From Date:&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtsdate" AutoPostBack="false" onkeypress="return false;" runat="server"></asp:TextBox><asp:ImageButton ID="IBtnFromDt" runat="server" ImageUrl="~/images/cal.png" />
                                                <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" PopupButtonID="IBtnFromDt" Format="yyyy-MM-dd"    runat="server" />
                                                <br />
                                                <br/>&nbsp;&nbsp;To Date:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtedate"  runat="server" AutoCompleteType="Search" onkeypress="return false;"  ></asp:TextBox>
                                                <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" PopupButtonID="IBtnToDt" Format="yyyy-MM-dd" runat="server" />
                                                <asp:ImageButton ID="IBtnToDt" runat="server" ImageUrl="~/images/cal.png" />--%>
                                                </td>
                                        </tr>
                                        <caption >
                                            <br  />
                                            <br  />
                                        </caption>
                                    </table>
                                    <table style="width:100%;">
                                        <tr>
                                            <td width="30%" valign="baseline" class="auto-style1" style="padding-left: 10px">From Date</td>
                                            <td width="70%" nowrap="nowrap" valign="baseline" class="auto-style1"><asp:TextBox ID="txtsdate" AutoPostBack="false" onkeypress="return false;" runat="server"></asp:TextBox>
                                                <asp:ImageButton ID="IBtnFromDt" runat="server" ImageUrl="~/images/cal.png" />
                                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" PopupButtonID="IBtnFromDt" Format="yyyy-MM-dd"    runat="server" />
                                            </td>
                                            
                                        </tr>
                                        <tr>
                                            <td width="30%" valign="baseline" style="padding-left: 10px">&nbsp;</td>
                                            <td width="70%" valign="baseline">&nbsp;</td>
                                            
                                        </tr>
                                        <tr>
                                            <td width="30%" valign="baseline" class="auto-style1" style="padding-left: 10px">To Date</td>
                                            <td width="50%" nowrap="nowrap" valign="baseline" class="auto-style1"><asp:TextBox ID="txtedate"  runat="server" AutoPostBack="false" onkeypress="return false;"  ></asp:TextBox>
                                                   <asp:ImageButton ID="IBtnToDt" runat="server" ImageUrl="~/images/cal.png" />
                                                <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" PopupButtonID="IBtnToDt" Format="yyyy-MM-dd" runat="server" />
                                            </td>
                                           
                                        </tr>
                                    </table>
                                </fieldset> </td>
                        </tr>
                    </table>
                </td>
                <td align="left" width="35%" >
                    <fieldset style="height:170px" >
                        <legend style=" color:Black; text-align:left;font-weight:bold; " >
                            <asp:CheckBox ID="CheckBox1"  OnCheckedChanged="checkuncheck" AutoPostBack="true"  runat="server" />
                            <asp:Label ID="lblvehn" runat="server" Text=""></asp:Label>
                        </legend>
                        <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" 
                      style=" display:block ">
                            <asp:CheckBoxList ID="UsrVeh"    runat="server">
                            </asp:CheckBoxList>
                           
                        </asp:Panel>
                    </fieldset> </td>
                <td align="center"  width="5%" style="padding-left: 10px" >
                    <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px"  ImageUrl="~/images/showbutton.png" 
                        OnClientClick="javascript:return MyGrid(); return false;"  ToolTip="Search  " />
                </td> 
              
            </tr>
            <tr >
                <td colspan="3" style=" width:70%" ></td>
                <td  style=" width:30%; text-align:right;" > 
                    <asp:ImageButton ID="btnExcelExport" runat="server" Height="18px" ImageAlign="Right" Visible="false" 
                        ImageUrl="~/Images/excelexpo.jpg" ToolTip="Export EXCEL" Width="18px"  OnClientClick="javascript: Export('grid'); return false;"  />
                    
           </td>
            </tr>
        </table>
          
        <div id="grid"> 
        </div>
           </ContentTemplate></asp:UpdatePanel>
    </div>
         
    <script language="javascript" type ="text/javascript" >


        $(document).ready()
        {
            $("#mask").hide();
            $("[id$='btnExcelExport']").hide()
        }
        function pageLoad(sender, args) {
            $("#mask").hide();

        }

        function MyGrid() {
            $("#mask").show();

            var t1 = $("#ContentPlaceHolder1_txtsdate").val();
            var t2 = $("#ContentPlaceHolder1_txtedate").val();

            var timeDiff = Math.abs(new Date(t1).getTime() - new Date().getTime());
            var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));


            var selectedValues = [];
            $("[id*=UsrVeh] input:checked").each(function () {
                selectedValues.push($(this).val());
            });


            if (t1 == "")
            { $("[id$='lblmsg']").html("Please select From Date!."); }
            else if (t2 == "")
            { $("[id$='lblmsg']").html("Please select From Date!."); }
            else if ((new Date(t1).getTime() > new Date().getTime()) || (new Date(t2).getTime() > new Date().getTime())) {
                $("[id$='lblmsg']").html("Future dates are not allowed!.");
            }
            else if (new Date(t1).getTime() > new Date(t2).getTime()) {
                $("[id$='lblmsg']").html("From Date cannot be greater than To Date!");
            }
            else if (diffDays > 31)
            { $("[id$='lblmsg']").html("Report for maximum 31 days can only be retrived."); }

            else if (selectedValues.length == 0)
            { $("[id$='lblmsg']").html("Please select atleast one vehicle."); }
            else {
                $("[id$='lblmsg']").html("");
                $("[id$='btnExcelExport']").show()
                var eid = '<%= Session("EID").ToString()%>';
                
                var t = '{"Sdate":"' + t1 + '", "EDate":"' + t2 + '", "Vehicles":"' + selectedValues.toString() + '","EID":"' + eid + '"}';
                $.ajax({
                    type: "post",
                    url: "VehicleHaltReport.aspx/FillGrid",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: t,
                    success: function (data) {
                        var d = $.parseJSON(data.d);
                        bindGrid(d);
                    },

                    error: function (data) {
                        alert(data.error);
                        $("#mask").hide();
                    }
                });
            }
            $("#mask").hide();
            $("[id$='btnExcelExport']").show();
    return false;
}
function bindGrid(Data1) {
    try {
        $("#grid").kendoGrid({

            dataSource: {
                dataType: "json",
                data: Data1,
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
            toolbar: ["excel"],
            excel: {
                fileName: "VehicleHaltReport.xlsx",
                filterable: true,
                pageable: true,
                allPages: true
            }
                    ,
            columns: [
                {
                    field: "FromDate",
                    title: "From Date"
                },
                 {
                     field: "ToDate",
                     title: "To Date"

                 },
                 {
                     field: "Vehicle_No",
                     title: "Vehicle No"
                 },
                 {
                     field: "SiteName",
                     title: "Site Name"

                 },
                 
                 {
                     field: "StartTime",
                     title: "Start Time",

                 }
                         , {
                             field: "EndTime",
                             title: "End Time",

                         },
                          
                        {
                            field: "HaltDuration",
                            title: "Halt Duration"

                        },
            ]

        });
    }
    catch (error)
    { }
    $("#mask").hide();

}

function Export(divid) {


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
    data = '<table>' + data + '</table>';
    data = escape(data);

    $('body').prepend("<form method='post' action='exportPage.aspx' style='top:-3333333333px;' id='tempForm'><input  name='data' value='" + data + "' ></form>");

    $("#tempForm").submit();


    //$('#tempForm').submit();
    $("#tempForm").remove();

    return false;
};
   </script>
</asp:Content>
