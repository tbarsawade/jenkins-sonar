<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="LastLocation, App_Web_iqn0gzeb" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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

      
<script type="text/javascript">
    function toggleDiv(divId) {
        $("#" + divId).toggle();
    }
    function HideDiv(divId) {
        $("#" + divId).Hide();
    }
</script>
<script src="http://maps.google.com/maps/api/js?sensor=false" 
          type="text/javascript"></script>
   <script type="text/javascript">

       function OpenWindow(url) {
           var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
           return false;

       }
       function Bindreport() {
           MyGrid();
           $("#mask").show();

           return false;
       }
       function ReadControls() {
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
                       result[Key] = $('#' + $(e).attr('id') + ' option:selected').text();
                       break;
                   default:
                       break;
               }
           });


           return result;
       }

       function MyGrid() {

         
           var ControlValArr = ReadControls();
           var ChkBoxValscir = ControlValArr["Circle"];
           var ChkBoxValscit = ControlValArr["City"];
           var ChkBoxValsIMEI = ControlValArr["IMEI"];
           var uid = '<%= Session("UID")%>';
           var uRole = '<%= Session("USERROLE")%>';
           var eid = '<%= Session("EID")%>';
           var IMEI = ""; var cir = ""; var cit = "";
           for (var i = 0 ; i < ChkBoxValsIMEI.length; i++) {
               IMEI = IMEI + "'" + ChkBoxValsIMEI[i] + "',";
           }
           for (var i = 0 ; i < ChkBoxValscir.length; i++) {
               cir = cir + "'" + ChkBoxValscir[i] + "',";
           }
           for (var i = 0 ; i < ChkBoxValscit.length; i++) {
               cit = cit + "'" + ChkBoxValscit[i] + "',";
           }
           var t = {};
           t["cir"] = cir;
           t["cit"] = cit;
           t["Imieno"] = IMEI;
           t["EID"] = eid;
           t["uRole"] = uRole;
           t["uid"] = uid;
           var str = JSON.stringify(t);
          
           $.ajax({
               type: "post",
               url: "GPSLastLocation.aspx/ShowDynamic1",
               contentType: "application/json; charset=utf-8",
               dataType: "json",
               data: str,
               success: function (data) {
                   bindGrid(data);
               },

               error: function (data) {
                   alert(data.error);
                   $("#mask").hide();
               }

           });
       }


       function bindGrid(Data1) {
           var grid = $("#divGrid").data("kendoGrid");
           // detach events
           if (grid != undefined)
               grid.destroy();

           var d = Data1.d;
           var d2 = JSON.parse(d["GridData"]);
           var col = d["Columns"];
         
           $("#divGrid").kendoGrid({
               dataSource: {
                   dataType: "json",
                   data: d2,
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
                   fileName: "VehiclePosition.xlsx",
                   filterable: true,
                   pageable: true,
                   allPages: true
               }  ,
              columns: col
           });
           $("#mask").hide();
       }


       function ShowPop(item) {
           var grid = $("#divGrid").data("kendoGrid");
           var dataItem = grid.dataItem($(item).closest("tr"));
           var TID = dataItem.TID;
           var TripType = dataItem.TripType;
           var sTime = $("#ContentPlaceHolder1_TxtStime").val();
           var ETime = $("#ContentPlaceHolder1_txtetime").val();
           var url = $("#hdnURI").val() + "?tid=" + TID + "&type=" + TripType + "&flag=3&stime=" + sTime + "&etime=" + ETime;
           OpenWindow(url);

           return false;
       }
    </script>

<style type="text/css">
    .GridCommandClass
    {
 background-image: url('images/earth_search.png');
    background-repeat: no-repeat;

    }
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnSearch"  />--%>
            <%-- <asp:PostBackTrigger ControlID="btnExportPDF" />--%>
            <asp:PostBackTrigger ControlID="btnexportxl" />
            <asp:PostBackTrigger ControlID="btnshow" />
        </Triggers>
        <ContentTemplate>
            <br />
            <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>
            <%--<asp:ImageButton ID="btnExportPDF" ToolTip="Export PDF"  runat="server" Width="18px" ImageAlign="Right"  Height="18px" ImageUrl="~/images/export.png"/>&nbsp;&nbsp;--%>
            <asp:ImageButton ID="btnexportxl" ToolTip="Export EXCEL" ImageAlign="Right" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg" />
            &nbsp;
            <div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top: 10px">
                <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">

                    <tr>
                        <td>
                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>

                        <td align="Left" width="30%">
                            <fieldset style="height: 170px">

                                <legend style="color: Black; text-align: Left; font-weight: bold;">
                                    <asp:CheckBox ID="circlecheck" runat="server" AutoPostBack="true" OnCheckedChanged  ="checkuncheckcicle"  />
                                   State
                                </legend>
                                <asp:Panel ID="Panel2" runat="server" Height="150px"
                                    Style="display: block" ScrollBars="Auto">
                                <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="true" OnSelectedIndexChanged ="checkuncheckcicle1"
                                     OnCheckedChanged="checkuncheckcicle1" IsSearch="1" data-ty="MULTISELECT" fld="Circle">
                                    </asp:CheckBoxList>
                                    </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="Left" width="30%">
                            <fieldset style="height: 170px">
                                <legend style="color: Black; text-align: Left; font-weight: bold;">
                                    <asp:CheckBox ID="Citycheck" runat="server" AutoPostBack="true" OnCheckedChanged="Citycheckuncheck" />
                                    City</legend>
                                <asp:Panel ID="Panel3" runat="server" Height="150px"
                                    Style="display: block" ScrollBars="Auto">
                                
                                    <asp:CheckBoxList ID="City" runat="server" AutoPostBack="true" OnSelectedIndexChanged="FilterUser" OnCheckedChanged="FilterUser" 
                                        IsSearch="1" data-ty="MULTISELECT" fld="City">
                                    </asp:CheckBoxList>
                                    </asp:Panel>
                            </fieldset>

                        </td>

                        <td align="Left" width="35%">
                            <fieldset style="height: 170px">
                                <legend style="color: Black; text-align: Left; font-weight: bold;">
                                    <asp:CheckBox ID="CheckBox1" OnCheckedChanged="checkuncheck" AutoPostBack="true" runat="server" />User-Vehicle</legend>
                                <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto"
                                    Style="display: block">


                                    <asp:CheckBoxList ID="UsrVeh" runat="server" IsSearch="1" data-ty="MULTISELECT" fld="IMEI">
                                    </asp:CheckBoxList>
                                    <%--       <asp:CheckBoxList ID="smsreports"   Visible="false"  runat="server">
               
                      </asp:CheckBoxList>--%>
                                </asp:Panel>
                            </fieldset>
                        </td>
                        <td align="center" class="m8b" width="5%">
                            <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px" OnClientClick="javascript:return Bindreport();" ImageUrl="~/images/showbutton.png" ToolTip="Search  " />
                        </td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="width: 70%"></td>
                        <td style="width: 30%; text-align: right;"></td>
                    </tr>
                     <tr>
                        <td colspan="4">
                            <div id="divGrid">
                            </div>
                         </td>
                    </tr>
                </table>
            </div>

            <div>
                <table width="100%" align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">
                    <tr>
                        <td>
                            <br />
                            <asp:UpdatePanel ID="upnl" runat="server">
                                <ContentTemplate>
                                    <asp:Panel runat="server" ID="pngv">
                                        <asp:GridView ID="GVReport" runat="server" AllowPaging="true"
                                            AllowSorting="False" AutoGenerateColumns="true" BorderColor="Green"
                                            BorderStyle="none" BorderWidth="1px" CellPadding="3"
                                            EmptyDataText="Record does not exists." Font-Size="Small" PageSize="15"
                                            ShowFooter="false" Width="100%">
                                            <RowStyle BackColor="White" BorderColor="Green" BorderWidth="1px"
                                                CssClass="gridrowhome" ForeColor="Black" Height="25px" />
                                            <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                            <HeaderStyle BackColor="#d0d0d0" BorderColor="Green" BorderWidth="1px"
                                                CssClass="gridheaderhome" Font-Bold="True" ForeColor="black" Height="25px"
                                                HorizontalAlign="Center" />
                                            <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                            <Columns>
                                            </Columns>
                                            <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                            <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                            <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                            <SortedDescendingHeaderStyle BackColor="#93451F" />
                                            <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                            <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                            <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                            <SortedDescendingHeaderStyle BackColor="#002876" />
                                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center"
                                                VerticalAlign="Middle" />
                                            </asp:GridView>
                                           <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                            <ProgressTemplate>
                                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                    please wait...
                                                </div>
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                          </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

