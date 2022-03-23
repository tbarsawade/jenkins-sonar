<%@ Page Title="Work Flow Master" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="WorkFlow.aspx.vb" Inherits="WorkFlow" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>WorkFlow Master</h4></asp:Label>
    </td>
        </tr>

    <tr style="color: #000000">
        <td style="text-align:left;width:100%;border:3px double green ">
       <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%" 
                border="0px">
         <tr> 
            <td style="width: 90px;"> 
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Field Name" Width="99%">
                 </asp:Label>
         </td>

         <td style="width: 170px;">
             <asp:DropDownList ID="ddlField" runat="server" CssClass="Inputform" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Width="99%">
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
                 ImageUrl="~/Images/search.png"  ToolTip="Search"/>
          </td>

         <td style="text-align: right;">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" ToolTip="Add new Work Flow" onclick="Add"/>
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
             CellPadding="2" DataKeyNames="wfid"
                    ForeColor="#333333" Width="100%" 
               AllowSorting="True" AllowPaging="True" DataSourceID="SqlData">
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


  <asp:BoundField DataField="wfName" HeaderText="Workflow Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        

                        <asp:BoundField DataField="wflogic" HeaderText="WorkFlow Logic">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="users" HeaderText="Users">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                             
                          <asp:TemplateField HeaderText="Action" >
                            <ItemTemplate>
                                                                

                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip="Edit record" OnClick="EditHit" AlternateText="Edit" />
                                 
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" ToolTip="Delete record" OnClick="DeleteHit" AlternateText="Delete"/>
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>
           <asp:SqlDataSource ID="SqlData" runat="server" 
               ConnectionString="<%$ ConnectionStrings:conStr %>" 
               SelectCommand="uspGetResultwf" SelectCommandType="StoredProcedure">
               <SelectParameters>
                   <asp:ControlParameter ControlID="ddlField" Name="sField" 
                       PropertyName="SelectedValue" Type="String" />
                   <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"  
                       Type="String" />
                   <asp:SessionParameter DefaultValue="0" Name="eid" SessionField="EID" 
                       Type="Int32" />
               </SelectParameters>
           </asp:SqlDataSource>

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

<asp:Panel ID="pnlPopupEdit" runat="server" Width="800px" style="" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:780px"><h3>New / Update WorkFlow</h3></td>
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
      <td colspan="7" align="left" >
      <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
      </td>
   </tr>

      <tr>
           <td align="left" style="width:110px" >
               <label>WF Name</label></td>
           <td colspan="3" align="left">
               <asp:TextBox ID="txtName" runat="server" CssClass="txtBox" Width="98%"></asp:TextBox>
           </td>

       <td style="width:120px" align="left" ><label>Document Type</label></td> 
       <td colspan="2"  align="left">
        <asp:DropDownList ID="ddlDocumentType" AutoPostBack="true"  CssClass="txtBox" runat="server" Width="98%">            
        </asp:DropDownList>
     </td>

       </tr>

   <tr>
      <td align="center" colspan="7"> 
       <table style="border-spacing:10px;"> 
        <tr> 
          <td colspan="4">
              &nbsp;           
         </td>
         </tr> 
        <tr> 
           <td align="left" style="width:110px">
               <asp:Label ID="Label2" runat="server" text="Select Field"> </asp:Label>
           </td> 
           <td align="left" style="width:80px">
               <asp:Label ID="Label4" runat="server" text="Operator"> </asp:Label>
           </td>
           <td align="left" style="width:120px">
               <asp:Label ID="Label5" runat="server" text="Value"> </asp:Label>
           </td>

           <td align="left" style="width:120px">
               <asp:Label ID="Label3" runat="server" text="Join"> </asp:Label>
          </td>
       
            <tr>
                <td align="left" style="width:170px">
                    <asp:DropDownList ID="ddlType" AutoPostBack="true" runat="server" CssClass="txtBox" Width="98%">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddlOperator" Width="70px" runat="server">
                        <asp:ListItem>  &gt; </asp:ListItem>
                        <asp:ListItem>  &lt; </asp:ListItem>
                        <asp:ListItem>  Like </asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtCondValue" runat="server" CssClass="txtBox" Width="150px"></asp:TextBox>
                </td>
                <td>
                    <asp:DropDownList ID="ddlAndOR" runat="server">
                        <asp:ListItem>  </asp:ListItem>
                        <asp:ListItem> AND </asp:ListItem>
                        <asp:ListItem> OR </asp:ListItem>
                     </asp:DropDownList>
                     <asp:Button ID="btnAddCond" runat="server" CssClass="btnNew" Font-Bold="True" 
                        Font-Size="X-Small" Text="Select" Width="50px" />
                     </td>                    
            </tr>
            <tr>
                <td colspan="6">
                    <asp:TextBox ID="txtLogic" runat="server" CssClass="txtBox" Width="100%"> </asp:TextBox>
                </td>
            </tr>  
       
        </tr>

        <tr> 
          <td colspan="4">
              &nbsp;           
         </td>
         </tr> 
             
       </table>
     </td>
   </tr>

   
   
   <tr>
    <td style="width:110px" align="left">
        <label>WorkFlow User</label>
      </td>
    <td align="left" style="width:170px">
        <asp:DropDownList ID="ddlOwner" CssClass="txtBox" runat="server" Width="98%">
        </asp:DropDownList>
       </td>

    <td style="width:60px" align="left"><label>SLA</b></label></td> 
    <td align="left"> <asp:TextBox ID="txtSLA" 
            runat="server" CssClass="txtBox" Width="90px"></asp:TextBox>        
    </td> 


  
    <td style="width:110px" align="left">
        <label>Next Status</label>  </td>
    <td align="left" style="width:170px">
        <asp:DropDownList ID="ddlWFStatus" CssClass="txtBox" runat="server" Width="98%">
        </asp:DropDownList>
       </td>

    <td style="width:60px" align="left">
      <asp:Button ID="btnActUserSave" runat="server" CssClass="btnNew" Font-Bold="True" 
            Font-Size="X-Small" Text="Add User" Width="70px" />
    </td> 
    


  </tr>
       <tr>
           <td align="left" colspan="7">
               <asp:GridView ID="gvUsers" runat="server" AllowPaging="false" 
                   AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" 
                   DataKeyNames="tid" ForeColor="#333333" Width="100%">
                   <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                   <RowStyle BackColor="#EFF3FB" />
                   <EditRowStyle BackColor="#2461BF" />
                   <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                   <PagerStyle BackColor="CornflowerBlue" ForeColor="White" 
                       HorizontalAlign="Center" />
                   <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                   <AlternatingRowStyle BackColor="White" />
                   <Columns>
                       <asp:TemplateField HeaderText="S.No">
                           <ItemTemplate>
                               <%# CType(Container, GridViewRow).RowIndex + 1%>
                           </ItemTemplate>
                           <ItemStyle Width="50px" />
                       </asp:TemplateField>
                       <asp:BoundField DataField="username" HeaderText="User Name">
                       <HeaderStyle HorizontalAlign="Left" />
                       </asp:BoundField>
                       <asp:BoundField DataField="WfStatus" HeaderText="WF Status">
                       <HeaderStyle HorizontalAlign="Left" />
                       </asp:BoundField>
                       <asp:BoundField DataField="sla" HeaderText="SLA">
                       <HeaderStyle HorizontalAlign="Left" />
                       </asp:BoundField>
                       <asp:TemplateField HeaderText="Action">
                           <ItemTemplate>
                               <asp:ImageButton ID="btnDown" runat="server" AlternateText="Move Down" ToolTip="Move Down"  
                                   Height="16px" ImageUrl="~/images/down.png" OnClick="PositionDown" Width="16px" />
                               &nbsp;
                              <asp:ImageButton ID="btnUp" runat="server" AlternateText="Move Up" ToolTip="Move Up"  
                                   Height="16px" ImageUrl="~/images/up.png" OnClick="PositionUp" Width="16px" />
                               &nbsp;

                               <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" ToolTip="Delete"
                                   Height="16px" ImageUrl="~/images/Cancel.gif" OnClick="DeleteHitUser" Width="16px" />
                           </ItemTemplate>
                           <ItemStyle HorizontalAlign="Center" Width="80px" />
                       </asp:TemplateField>
                   </Columns>
               </asp:GridView>
           </td>
       </tr>


  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Update" 
                            OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
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



<asp:Button id="btnShowPopupDeleteUser" runat="server" style="Display:none" />
<asp:ModalPopupExtender ID="btnDeleteUser_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDeleteUser" PopupControlID="pnlPopupDeleteUser" 
                CancelControlID="btnCloseDeleteUser" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupDeleteUser" runat="server" Width="500px" style="Display:none " BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>User Delete : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDeleteUser" ImageUrl="images/close.png" runat="server"/></td>
</tr>

<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDeleteUser" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<h2> <asp:Label ID="lblMsgDeleteUser" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnActDeleteUser" runat="server" Text="Yes Delete"  Width="90px" 
                    OnClick="DeleteRecordUser" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>

</table> 
</div>
</asp:Panel>



</asp:Content>