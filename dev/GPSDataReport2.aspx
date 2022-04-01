<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="GPSDataReport2.aspx.vb" Inherits="GPSDataReport2" %>
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
       function ReadControls (){
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
         
           var t1 = $("#ContentPlaceHolder1_txtsdate").val();
           var t2 = $("#ContentPlaceHolder1_txtedate").val();
           var sTime = $("#ContentPlaceHolder1_TxtStime").val();
           var ETime = $("#ContentPlaceHolder1_txtetime").val();
           var ControlValArr = ReadControls();
           var ChkBoxVals = ControlValArr["IMEI"];
           var ReportType = ControlValArr["ReportType"];
           var SubType = ControlValArr["SubType"];
           var uid = '<%= Session("UID")%>';
           var uRole = '<%= Session("USERROLE")%>';
           var eid = '<%= Session("EID")%>' ;
           var IMEI = "";
           for (var i = 0 ; i < ChkBoxVals.length; i++)
           {
               IMEI = IMEI+ "'" + ChkBoxVals[i] +"',";
           }
           var t = {};
           t["ReportType"] = ReportType;
           t["SubType"] = SubType;
           t["sDate"] = t1;
           t["tDate"] =  t2 ;
           t["Imieno"] = IMEI;
           t["STime"] = sTime;
           t["ETime"] = ETime ;
           t["EID"] = eid;
           var str = JSON.stringify(t);
           var testdata = '{"ReportType":"' + ReportType + '", "SubType":"' + SubType + '", "sDate":"' + t1 + '", "tDate":"' + t2 + '","Imieno":"' + IMEI + '","sTime":"' + sTime + '","ETime":"' + ETime + '", "Eid":' + eid + '}'

           $.ajax({
               type: "post",
               url: "GPSDataReport2.aspx/ShowDynamic1",
               contentType: "application/json; charset=utf-8",
               dataType: "json",
               data:str,
               success: function (data) {
                   bindGrid(data);
               },

               error: function (data) {
                   alert(data.error);
                   $("#mask").hide();
               }

           });
       }

       
       function bindGrid(Data1)
       {
           var grid = $("#divGrid").data("kendoGrid");
           // detach events
           if (grid != undefined)
               grid.destroy();

           var d = Data1.d;
           var d2 = JSON.parse(d["GridData"]);
           var col = d["Columns"];
           var command = "";
           var URI = d["URI"];
          
         //  $("#hdnURI").attr('value',URI);
           if (d["IsDetailsOn"] == true)
           {
               command = { command: { text: " ", template: "<input type='image' src='images/earth_search.png' width='30px' onclick='ShowPop(this);return false;'  title='Show Map'/>" } };
              
           }
           col.push(command);
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
                   fileName: "MileageReport.xlsx",
                   filterable: true,
                   pageable: true,
                   allPages: true
               }
                    ,              
               //pageable: {
               //    buttonCount: 5
               //},
               columns: col
           });
           $("#mask").hide();
       }

     
       function ShowPop( item)
       {
           var grid = $("#divGrid").data("kendoGrid");
           var dataItem = grid.dataItem($(item).closest("tr"));
           var TID = dataItem.TID;
           var TripType = dataItem.TripType;
           var sTime = $("#ContentPlaceHolder1_TxtStime").val();
           var ETime = $("#ContentPlaceHolder1_txtetime").val();
           var url = $("#hdnURI").val()+ "?tid=" + TID + "&type=" + TripType + "&flag=3&stime=" + sTime + "&etime=" + ETime;
           OpenWindow(url);
          
           return false;
       }
    </script>

<style type="text/css">
      .divloader
        {
           position: absolute;
background-color: #FAFAFA;
z-index: 2147483647 !important;
opacity: 0.8;
overflow: hidden;
text-align: center; top: 0; left: 0;
height: 100%;
width: 100%;
padding-top:20%;
          
     
        }
   .AjaxLoaderInner
{
    position: absolute;
    top: 45%;
    left: 45%;
    font-size: 11px;
    font-family: Verdana, Arial, Helvetica, sans-serif;
    font-weight: bold;
    color: Black;
}
.AjaxLoaderOuter
{
    position: fixed;
    vertical-align: middle;
    text-align: center;
    z-index: 1000;
    top: 0px;
    left: 0px;
    background-color: Gray;
    filter: alpha(opacity=70);
    opacity: 0.7;
    margin: 0px 0px 0px 0px;
    width: 100%;
    height: 100%;
    min-height: 100%;
    min-width: 100%;    
}
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
   
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
 <Triggers>
        <%--<asp:PostBackTrigger ControlID="btnSearch"  />--%>
       <asp:PostBackTrigger ControlID="btnExportPDF" />
       <asp:PostBackTrigger ControlID="btnexportxl" />
     <asp:PostBackTrigger ControlID ="btnshow" />
        </Triggers>
<ContentTemplate>
<br />
   <a href="javascript:toggleDiv('div1');" style="background-color: #ccc; padding: 5px 10px;">Show / Hide</a>
<asp:ImageButton ID="btnExportPDF" ToolTip="Export PDF"  runat="server" Width="18px" ImageAlign="Right"  Height="18px" ImageUrl="~/images/export.png"/>&nbsp;&nbsp;
<asp:ImageButton ID="btnexportxl"  ToolTip="Export EXCEL" ImageAlign="Right"  runat="server"  Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg"/> &nbsp;
<div class="gradientBoxesWithOuterShadows" id="div1" style="margin-top:10px">
    <asp:UpdateProgress ID="upd" runat="server">
    <ProgressTemplate>
        <div class="AjaxLoaderOuter">
            <div class="AjaxLoaderInner" id="LoadingImg">
                <p>
                    Loading Please wait</p>
                <asp:Image ID="imgloading" runat="server" ImageUrl="~/images/loading.gif" /><br />
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
  <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9"> 
     
          <tr>
          <td>
          <asp:Label ID="lblmsg" runat="server" Font-Bold="true" ForeColor="Red"  ></asp:Label>
          </td>
          </tr>
          <tr>
                            
                            <td  align="left" class="m8b" width="25%">
                          
                           <fieldset>

  <legend style=" color:Black; text-align:center; font-weight:bold;">Report Type</legend>
                            <br />
                            <br />
                 <asp:Panel ID="Panel2" runat="server" Height="100px"
                      style=" display:block ">
                  <asp:DropDownList ID="ddlrtype" Width="150px"  runat="server" IsSearch="1" data-ty="SINGLESELECT" fld="ReportType">
                      <asp:ListItem Value="0">Select</asp:ListItem>
                     <asp:ListItem Value="MileageReport">Mileage Report</asp:ListItem>
                     </asp:DropDownList> 
                  <br />
                  <br />
                  <asp:DropDownList ID="ddldhm" Width="150px" runat="server" IsSearch="1" data-ty="SINGLESELECT" fld="SubType">
                      <asp:ListItem Value="0">Select</asp:ListItem>
                      <asp:ListItem Value="1">Daily</asp:ListItem>
                     <%-- <asp:ListItem Value="2">Half Hourly</asp:ListItem>--%>
                       </asp:DropDownList>
               </asp:Panel>
                  </fieldset>
              </td>
            
              <td align="left" class="style2">
                  <table   >
                      <tr>
                          <td >
                                         
                          <fieldset style="width:326px; height:170px;">
                          <legend style=" color:Black; text-align:center;font-weight:bold;">Date Range</legend>
                         
                          <table>
                          <tr>

                          <td >
                          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtsdate" AutoPostBack="false" onkeypress="return false;" runat="server"></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate" Format="yyyy-MM-dd"    runat="server" /> <br/><br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtedate"  runat="server" AutoCompleteType="Search" onkeypress="return false;"  ></asp:TextBox>
                                 <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd" runat="server" /> <br /><br />
                                 </td>
                                 <td >
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="TxtStime" Width="50px" Text="00:00"  runat="server"></asp:TextBox><br /><br />
                                                    
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtetime" Text="23:59" Width="50px" runat="server"></asp:TextBox>
                             <%-- <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
    TargetControlID="txtetime"
    WatermarkText="HH:MM"
    WatermarkCssClass="watermarked" />--%><br /><br />
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
                        <legend style=" color:Black; text-align:left;font-weight:bold; "><asp:CheckBox ID="CheckBox1"  OnCheckedChanged="checkuncheck" AutoPostBack="true"  runat="server" /><asp:Label ID="lblvehn" runat="server" Text=""></asp:Label></legend>
                <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" 
                      style=" display:block ">
                      
       
                      <asp:CheckBoxList ID="UsrVeh"    runat="server" IsSearch="1" data-ty="MULTISELECT" fld="IMEI">
               
                      </asp:CheckBoxList>
                      <%--       <asp:CheckBoxList ID="smsreports"   Visible="false"  runat="server">
               
                      </asp:CheckBoxList>--%>
                  </asp:Panel>
                  </fieldset>
              </td>
               <td align="center" class="m8b" width="5%">
                             <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px" OnClientClick="javascript:return Bindreport();"
                                  ImageUrl="~/images/showbutton.png"  ToolTip="Search  " />
              </td>
               </td>
          </tr>
            <tr>
          <td colspan="3" style=" width:70%"> <asp:HiddenField ID="hdnURI" ClientIDMode="Static"  runat="server" Value="" /></td>
          <td  style=" width:30%; text-align:right;">
           
           </td>
          </tr>
        <tr>
          <td colspan="4"><div style="width: 100%; border: solid 1px #e1e1e1; overflow-x: scroll;"><div id="divGrid" > </div> </div> </td>
          </tr>
          </table>
    </div>
    
    <div>
          <table width="100%"  align="center" border="0" cellpadding="3" cellspacing="0" bordercolor="#E0E0E0" class="m9">               
                           <tr>
                                            <td>
                                                <br />
  <asp:UpdatePanel ID="upnl" runat="server">
  <ContentTemplate>
  <asp:Panel  runat="server" ID="pngv"> 
                      <asp:GridView ID="GVReport" runat="server" AllowPaging="true" 
                          AllowSorting="False" AutoGenerateColumns="true" BorderColor="Green" 
                          BorderStyle="none" BorderWidth="1px" CellPadding="3" 
                          EmptyDataText="Record does not exists." Font-Size="Small" PageSize="15" 
                          ShowFooter="false" Width="100%" Visible="False">
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



                      <asp:GridView ID="GVReport1" runat="server" AllowPaging="true" 
                          AllowSorting="False" AutoGenerateColumns="true" BorderColor="Green" 
                          BorderStyle="none" BorderWidth="1px" CellPadding="3" DataKeyNames="tid" 
                          EmptyDataText="Record does not exists." Font-Size="Small" PageSize="15" 
                          ShowFooter="false" Width="100%" Visible="False">
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
                                 <Columns>
                                 <asp:TemplateField HeaderText="Map" ItemStyle-HorizontalAlign="Right"  >
                            <ItemTemplate>
                    <%--<a class="detail" style="text-decoration:none;"  href="#" onclick="OpenWindow('http://<%# HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath%>/<%#DataBinder.Eval(Container.DataItem, "uri")%>?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')"   >--%>
                                <%--<a class="detail" style="text-decoration:none;"  href="#" onclick="OpenWindow('http://<%# DataBinder.Eval(Container.DataItem, "uri")%>?tid=<%#DataBinder.Eval(Container.DataItem, "tid")%>&type=<%#DataBinder.Eval(Container.DataItem, "TripType")%>')"   >
                 <img alt="Show On Map" src="images/earth_search.png"  height="16px" width="16px"/></a>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                                </Columns>
                         
                      </asp:GridView>
                       <asp:UpdateProgress ID="UpdateProgress2" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
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

