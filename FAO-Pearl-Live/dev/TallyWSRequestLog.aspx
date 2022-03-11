<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="TallyWSRequestLog.aspx.vb" Inherits="TallyWSRequestLog" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  
        <asp:UpdatePanel ID="updPnlGrid" runat="server">
    <Triggers>
     
     <asp:PostBackTrigger ControlID="btnexport" />
    </Triggers>
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
               <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h3>Tally WS Request Log</h3></asp:Label>
                </td></tr>
                <tr>
                <td style="text-align: center; ">
               <asp:Label ID="lblMsgupdate" runat="server" Font-Bold="True" ForeColor="Red" 
                    Font-Size="Small" ></asp:Label>
                </td>

        </tr>

    <tr style="color: #000000">
        <td style="text-align:left;width:100%;border:3px double lime ">
       <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%" 
                border="0px">
         <tr> 
            <td style="width: 5%;"> 
             <asp:Label ID="lblPcompany" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Principal Company"></asp:Label>
            </td>


         <td style="width: 20%;">
             <asp:Label ID="lblres0" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Super Distributor"></asp:Label>
         </td>

         <td width="20%">
             <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Doc Type"></asp:Label>
             </td>

          

         <td width="20%">
                          <asp:Label ID="lblres" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Service Name"></asp:Label>
          
             </td>

         <td style="width: 10%;">
               <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text=" Action From"></asp:Label>
         </td>
             <td style="width:2%;">
                 &nbsp;</td>
             <td style="width: 10%;">
                 <asp:Label ID="lblto" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Action To"></asp:Label>
         </td>
             <td style="text-align: right;" width="5%">&nbsp;<tr>
                <td style="width: 20%;">
                      <asp:DropDownList ID="ddlPcompany" runat="server" Width="99%" 
                          AutoPostBack="True">
                     </asp:DropDownList>
                     </td>
                 <td style="width: 20%;">
                     <asp:DropDownList ID="ddlSuperDistrbtr" runat="server" Width="99%">
                     </asp:DropDownList>
                 </td>
                 <td width="20%">
                     <asp:DropDownList ID="ddlDocType" runat="server" AutoPostBack="True" CssClass="Inputform" Width="90%">
                     </asp:DropDownList>
                 </td>
                 <td width="20%">
                     <asp:DropDownList ID="ddlServiceNm" runat="server" Width="99%">
                     </asp:DropDownList>
                 </td>
                 <td style="width: 10%;" nowrap="nowrap">
                     <asp:TextBox ID="txtFrmDt" runat="server"  Font-Size="Small" Width="80%"></asp:TextBox>
                    <%-- <asp:CalendarExtender TargetControlID="txtFrmDt" runat="server" ID="txtFrmDt_CalendarExtender" Enabled="True" PopupButtonID="ImgbtnFrmDt"></asp:CalendarExtender>
                     <asp:ImageButton ID="ImgbtnFrmDt" runat="server" ImageUrl="~/images/cal.png" Width="20%" />--%>
                 </td>
                 <td style="width:2%;">&nbsp;</td>
                 <td style="width: 10%;" nowrap="nowrap">
                    
                    
              <%--  <input type="text" id="txtToDt">--%>
                       <asp:TextBox ID="txtToDt" runat="server"   Font-Size="Small" Width="80%"></asp:TextBox>
                    <%--   <asp:CalendarExtender ID="CalendarExtender2"   runat="server" Format="yyyy-MM-dd" TargetControlID="txtToDt" PopupPosition="Left" PopupButtonID="ImgbtnToDt">
                     </asp:CalendarExtender>
                      
                       <asp:ImageButton ID="ImgbtnToDt" runat="server" ImageUrl="~/images/cal.png" Width="20%" />--%>
                 </td>
                 <td style="text-align: center;" width="5%">
                     <asp:ImageButton ID="ImgBtnSearch" runat="server" ImageUrl="~/images/search.png" Width="20px" />
                     <caption>
                     &nbsp;
                     </caption>
                 </td>
                 </tr>
             </td>
         
          

         </tr>
      </table>

           <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>

         </ProgressTemplate>
      </asp:UpdateProgress>

        </td>
    </tr>


     <tr style="color: #000000">
     <td  valign="top" align="center">
       <div>  <table width="100%">
           <tr><td align="right" style="padding-right: 5%; height: 20px;">
               <asp:ImageButton ID="btnexport" runat="server" ImageUrl="~/Images/excel.gif" ToolTip="Export" Visible="False" />
          </td></tr>

              <tr>
                  <td>
                      <asp:GridView ID="gvSearch" runat="server" Width="100%" >
                           <RowStyle BackColor="White" CssClass="gridrowhome" HorizontalAlign="Center"   Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                    <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White"  />
                    <PagerStyle ForeColor="green" HorizontalAlign="Center"  />
                    <HeaderStyle Font-Bold="True"    BorderColor="Green"  BorderWidth="1px"  
                            Height="25px" ForeColor="black" 
                        CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center"  />
                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
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
<asp:Button id="btnShowPopupEdit" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
    
     <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.6/jquery.min.js" type="text/javascript"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js" type="text/javascript"></script>
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="Stylesheet" type="text/css" />
<script type="text/javascript" >

    $(document).ready(function () {
        $("[id$=txtToDt]").datepicker({
            showOn: "button",
            buttonImage: "images/cal.png",
            buttonImageOnly: true,
            buttonText: ""
        });


        $("[id$=txtFrmDt]").datepicker({
            showOn: "button",
            buttonImage: "images/cal.png",
            buttonImageOnly: true,
            buttonText: ""

        });
    });
    function pageLoad(sender, args) {
     
        // JS to execute during full and partial postbacks
        $("[id$=txtToDt]").datepicker({
            showOn: "button",
            buttonImage: "images/cal.png",
            buttonImageOnly: true,
            buttonText: ""
        });


        $("[id$=txtFrmDt]").datepicker({
            showOn: "button",
            buttonImage: "images/cal.png",
            buttonImageOnly: true,
            buttonText: ""

        });
    }
   
    
      
   
</script>
</asp:Content>

