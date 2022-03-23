<%@ Page Title="Report Builder" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" EnableEventValidation="false"  CodeFile="ReportBuilder.aspx.vb" Inherits="ReportBuilder" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   

    <asp:UpdatePanel ID="updPnlGrid" runat="server"  >
  
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>Report Designer</h4></asp:Label>
    </td>
              </tr>
       <tr>
               <td style="text-align:right;width:100%;border:3px double lime ">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" onclick="Add"/>
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
             CellPadding="2" DataKeyNames="reportid"
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

  <asp:BoundField DataField="ReportName" HeaderText="Report Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="reportDescription" HeaderText="Description">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                     
                        <asp:BoundField DataField="MainEntity" HeaderText="Main Entity">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="SubEntity" HeaderText="Sub Entity">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                            
                          <asp:BoundField DataField="pageheader" HeaderText="Page Header" />
                        <asp:BoundField DataField="pagefooter" HeaderText="Page Footer" />
                                            
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                              <%--  <asp:ImageButton ID="btnPreview" runat="server" ImageUrl="~/images/search.png" Height="16px" Width="16px" OnClick="PreviewHit" ToolTip ="Preview Form" AlternateText="Preview" />
                                 &nbsp;--%>
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/editreport.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip ="Edit Report Detail" AlternateText="Edit" />
                                 &nbsp;
                                 <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/addfields.png" Height="16px" Width="16px" OnClick="UpdateScreen"  AlternateText="Update Screen" ToolTip="Build Query" />
                                   &nbsp;
                                  <%-- <asp:ImageButton ID="btnAddFilter" runat="server" ImageUrl="~/images/filter.png" Height="16px" Width="16px" OnClick="AddFilter"  AlternateText="Add Filter" ToolTip="Add Filter" />--%>
                                   &nbsp;
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/closered.png" Height="16px" Width="16px" OnClick="DeleteHit" ToolTip ="Delete Report" AlternateText="Delete"/>
                            </ItemTemplate>
                            <ItemStyle Width="130px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>
    </td> 
     </tr>
         </table>
                 </ContentTemplate> 
             </asp:UpdatePanel> 


<asp:Button id="btnShowPopupForm" runat ="server" style="display:none" />
<asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat ="server" PopupControlID ="pnlPopupForm"  TargetControlID ="btnShowPopupForm" 
 CancelControlID ="btnCloseForm" BackgroundCssClass ="modalBackground"    ></asp:ModalPopupExtender>

 <asp:Panel ID="pnlPopupForm" runat ="server" Width ="750px" Height ="300px" BackColor ="White"   >
 <div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td width="980px"><h3>New Report</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr><td colspan ="2">
 <div id="main" >
 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
<div class="form" style="text-align:left"> 

<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
<tr><td style="width:100%" colspan="2">   <asp:Label ID="lblForm" runat="server" Text=""></asp:Label> 
</td></tr>

<tr><td style="width:100%" colspan="2"><h2>Enter Basic Report Information</h2></td></tr>

<tr>
   <td valign="middle" style="text-align:right"><label title="Report Name" > *Report Name <img src="Images/Help.png" alt="Enter Your Report Name" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtReportName" runat="server" CssClass="txtBox" 
         Width="498px"></asp:TextBox> 
       </td>
       <tr>
       
       <td valign="middle" style="text-align:right">
  </label></td>
  <td>
  
    
        <label title="Sub Entity" > Report Type : </label>
  
        <asp:DropDownList ID="ddlrptType" runat="server" AutoPostBack="true" 
                    CssClass="txtBox" Enabled="True" Width="185px">
                  
                    <asp:ListItem>Menu Driven </asp:ListItem>
                    <asp:ListItem>Action Driven</asp:ListItem>
                </asp:DropDownList> 

  
  </td>

       </tr>
       
     

</tr>


<tr>
   <td valign="middle" style="text-align:right"><label title="Report Description will describe about Report" > *Report Description  <img src="Images/Help.png" alt="Enter Your Report Description" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtDesc" runat="server" CssClass="txtBox" 
           Width="502px" TextMode="MultiLine" Height="50px"></asp:TextBox> 
     </td>
</tr>

<tr>
   <td valign="middle" style="text-align:right"><label title="Main Entity" > *Main Entity <img src="Images/Help.png" alt="Main Entity" /> : 
  </label></td>
   <td style=""> 
       <asp:DropDownList ID="ddlMainEntity" runat="server" AutoPostBack="true" 
                    CssClass="txtBox" Enabled="True" Width="180px">
                    <%--<asp:ListItem>--Select Main Entity--</asp:ListItem>--%>
                    <asp:ListItem>DOCUMENT</asp:ListItem>
                    <asp:ListItem>MASTER</asp:ListItem>
                </asp:DropDownList> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                  <label title="Sub Entity" > *Sub Entity <img src="Images/Help.png" alt="Sub Entity" /> : </label>
  
                <asp:DropDownList ID="ddlSubEntity" runat="server"  AutoPostBack="true"
                    CssClass="txtBox" Enabled="True" Width="176px">
                  
                </asp:DropDownList> 
     </td>
     
</tr>

<tr>
   <td style="text-align:right"> <label title="Page Header Of Report" > *Page Header <img src="Images/Help.png" alt="Page Header" /> : </label></td>

   <td style=""> 



   <asp:TextBox ID="txtPageHeader" runat="server" CssClass="txtBox" 
           Width="502px" TextMode="MultiLine" Height="50px"></asp:TextBox> 
      
   </td>


</tr>
<tr>
   <td valign="middle" style="text-align:right"><label title="Page Footer" > *Page Footer <img src="Images/Help.png" alt="Page Footer" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtPageFooter" runat="server" CssClass="txtBox" 
           Width="504px" TextMode="MultiLine" Height="50px" ></asp:TextBox> 
     </td>
</tr>


 <tr>
 <td> 
   </td>
        <td align="left">
            <asp:Button ID="btnsavereport" runat="server" CssClass="btnNew" 
                Text="Save And Create Report" Width="200px" /> &nbsp;
               
     <asp:Label ID="lblReportSave" runat="server" ForeColor="Red"></asp:Label>
            </td>
    </tr>

</td></tr>
</table>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</div>
</td></tr>
</table>
</div> 
 </asp:Panel>

 <%# CType(Container, GridViewRow).RowIndex + 1%>

 <asp:Button id="btnRptFields" runat="server" style="display:none"  />
<asp:ModalPopupExtender ID="btnDeleteReport" runat="server" 
                                TargetControlID="btnRptFields" PopupControlID="pnlReportDelete" 
                CancelControlID="btnCloseDelete1" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlReportDelete" runat="server" Width="500px"  BackColor="Aqua" style="display:none"   >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Report Delete : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelete1" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updateDeleteRptFiles" runat="server" UpdateMode="always">
   <ContentTemplate> 
<h2> <asp:Label ID="lblconf" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnDltFld" runat="server" Text="Yes Delete"  Width="90px" 
                     CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>



<%--  <asp:ImageButton ID="btnPreview" runat="server" ImageUrl="~/images/search.png" Height="16px" Width="16px" OnClick="PreviewHit" ToolTip ="Preview Form" AlternateText="Preview" />
                                 &nbsp;--%>




 
<asp:Button id="btnShowPopupDelete" runat="server" style="Display:none "  />
<asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete" 
                CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" style="Display:none;" BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Delete : Confirmation</h3></td>
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




<asp:Button id="btnShowPopupF" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="btnF_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupF" PopupControlID="PnlPopupF" 
                CancelControlID="btnCloseF" BackgroundCssClass="modalBackground" 
                               >
</asp:ModalPopupExtender>
<asp:Panel ID="PnlPopupF" runat="server" Width="600px"  style="display:none;" BackColor="aqua">
<div class="box" style="overflow:scroll;">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3>New / Update Field</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseF" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan ="2" style="text-align: left;" valign="top">
<asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
 <table width="100%">
 <tr>
 <td align ="left" >
 <asp:Label ID="lbMsgField" runat ="server" Font-Size ="X-Small" Font-Bold ="true" ForeColor ="Red" ></asp:Label>
 </td>
 </tr>
 <tr>

 <td>
     </td>
                </tr>
                <tr>
                <td align ="right" >
                
                </td>
                </tr>
                </table>
                </ContentTemplate> 
                </asp:UpdatePanel> 
            </td>
</tr>
</table> 
</div>
</asp:Panel>
<asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" PopupControlID ="pnlview" TargetControlID="btnpop" CancelControlID="btnclose"  BackgroundCssClass="modalBackground"></asp:ModalPopupExtender>
           <asp:Button ID="btnpop" runat="server" style="display:none"  />
          
           
<asp:Panel ID="pnlview" runat="server" Width="600px"  BorderColor="orange"  BorderWidth="1"   >
<div class="box" >
<table cellspacing="2px" cellpadding="2px" width="100%" >
<tr>
<td style="width:580px"><h3>Create your Query </h3></td>
<td style="width:20px"><asp:ImageButton ID="btnClose" ImageUrl="images/close.png" 
        runat="server" style="width: 16px"/>
        </td>
        </tr>
<tr>
<td colspan="2" >
<asp:updatepanel id="updatePanelEdit" runat="server" UpdateMode="Conditional" >
<ContentTemplate>
<table cellpadding ="1px" cellspacing ="1px"  width="100%" >
<tr>
<td width="100%" align="left"  >
    
        <table cellspacing="5px" cellpadding="0px" width="100%" border="0px" style="margin-top:0px">
        <tr>
        <td  colspan="2" style="margin-top:5px">
        <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
        </td>
        </tr>
        <tr>
        <td  style="text-align:left; ">
        
         <asp:Label ID="lblFname" runat="server" Text="*Build Query :" ></asp:Label>
                                
                       </td>
                       <td >
                       <div style="overflow:scroll;">
                           <asp:TextBox ID="txtquery" runat="server" CssClass="txtBox" TextMode="MultiLine" Height="200px"  Width="460px"></asp:TextBox>
                           </div>
            </td>
                       </tr>
                       <caption>
                           &nbsp;&nbsp;&nbsp;
                          
            </caption>
            </td>
            </tr>
       <tr>
              <td>
              </td>
                      <td>
                          <asp:Button ID="btnActSave" runat="server" CssClass="btnNew" Font-Bold="True" 
                              Font-Size="X-Small" Text="Save" Width="70px" />
                      </td>
                      </caption>
    </tr>
                             </table>
         
       
              
</td>
</tr>
</table>
</ContentTemplate> 
 </asp:updatepanel> 
</td>
</tr></table>
</div>
    </asp:Panel> 
</asp:Content>


