<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="HolidayMaster.aspx.vb" Inherits="Location" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdatePanel ID="updPnlGrid" runat="server">
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>Holiday  Master</h4></asp:Label> </td> </tr>

                    <tr><td style="text-align: center; ">
  <asp:Label ID="lblRecord" runat="server"  ForeColor="red" 
               Font-Size="Small" ></asp:Label></td></tr>




            <tr style="color: #000000">
        <td style="text-align:left;width:100%;border:3px double lime ">
       <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%" 
                border="0px">
         <tr> 
            <td style="width: 140px;"> 
                 <asp:Label ID="lblLocatioName" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Holiday Field" Width="100%">
                 </asp:Label>
         </td>

         <td style="width: 170px;">
             <asp:DropDownList ID="ddlHolidayName" runat="server" CssClass="Inputform" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Width="99%">
             </asp:DropDownList>
         </td>
         <td style="width: 50px;">
         <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Value" Width="99%"></asp:Label>
         </td>

         <td style="width: 200px;">
               <asp:TextBox ID="txtValue" runat="server" CssClass="Inputform" Font-Bold="True" 
               Font-Size="Small"  Width="99%"></asp:TextBox>
         </td>
         

         <td style="text-align: right;width:25px">
             <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" 
                 ImageUrl="~/Images/search.png"  ToolTip="Search Location " OnClick="Search"/>
          </td>

          
          
         <td style="text-align: right;">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px"   Height="20px" ImageUrl="~/Images/plus.jpg" ToolTip="Add Location" onclick="Add"/>
             &nbsp;
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
     <td style="text-align: left;" valign="top">
       <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
             CellPadding="2" DataKeyNames="HoliID"
                    ForeColor="#333333" Width="100%" 
               AllowSorting="True" AllowPaging="True">
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                    <EditRowStyle BackColor="#2461BF" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>

  <asp:TemplateField HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="50px" />
</asp:TemplateField>


  <asp:BoundField DataField="holidayname" HeaderText="Holiday Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        

                        <asp:BoundField DataField="locationname" HeaderText="Holiday Location" />

                        <asp:BoundField DataField="HoliDate" HeaderText="Holiday Date">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                             
                          <asp:BoundField DataField="HoliDayDesc" HeaderText="Discription" />
                                             
                          <asp:TemplateField HeaderText="Action" >
                            <ItemTemplate>
                                                                

                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip="Edit record" OnClick="EditHit" AlternateText="Edit" />
                                 
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" ToolTip="Delete record" OnClick="DeleteHit" AlternateText="Delete"/>
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>

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


<asp:Panel ID="pnlPopupEdit" runat="server" Width="600px" style="" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3>New / Update Holiday</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server" ToolTip="Close"/></td>
</tr>
<tr>
<td colspan="2">
<div class="form">

<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 

   <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="4" align="left" >
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>

        <tr>
           <td align="left" style="width:110px">
               <label>Holiday Name</label></td>
           <td colspan="3" align="left">
               <asp:TextBox ID="txtHoliName" runat="server" CssClass="txtBox" Width="98%"></asp:TextBox>
           </td>
       </tr>
   <tr>
    <td style="width:110px" align="left"><label>Holiday Location</label></td> 
    <td style="width:300px" align="left">
        <asp:DropDownList ID="ddlHoliLocation" CssClass="txtBox" runat="server" Width="98%">
           
        </asp:DropDownList>

       </td>
    

   </tr>


   <tr>
    <td style="width:110px" align="left">
        <label>Holiday Date</label></td>
    

    
    <td style="width: 170px;">
                 <asp:TextBox ID="txtHoliDate" runat="server" Width="170px"></asp:TextBox>
    <asp:CalendarExtender ID="txtHoliDate_CalendarExtender" runat="server" 
        Enabled="True" Format="dd-MMM-yyyy" TargetControlID="txtHoliDate">
    </asp:CalendarExtender>
     </td>     

     <tr>
           <td align="left" style="width:110px">
               <label>Discription</label></td>
           <td colspan="3" align="left">
               <asp:TextBox ID="txtHoliDesc" runat="server" CssClass="txtBox" Width="98%"></asp:TextBox>
           </td>
       </tr>


<tr><td colspan="2" align="right"><asp:Button ID="btnActUserSave" runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Save" Width="70px" />
</td></tr>
  </tr>
       
  </table>
          
   </ContentTemplate> 
   </asp:UpdatePanel> 

</div>
</td>
</tr>
</table> 
</div>
</asp:Panel>

<asp:Button id="btnShowPopupDelete" runat="server" style="Display:none" />
<asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete" 
                CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" style="Display:none " BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Work Flow Delete : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<h2> <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnActDelete" runat="server" Text="Yes Delete"  Width="90px" 
                    OnClick="DeleteRecord" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>

</table> 
</div>
</asp:Panel>




</asp:Content>

