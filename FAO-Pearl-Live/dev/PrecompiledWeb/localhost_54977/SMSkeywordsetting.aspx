<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" enableeventvalidation="false" inherits="SMSkeywordsetting, App_Web_tuuwcp04" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
      <ContentTemplate> 
       <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>SMS Keyword Setting</h4></asp:Label>
    </td>
        </tr>
        <tr>
        <td style="text-align:left;"> <asp:Label ID="lblerr" runat="server"  style="text-align:left;" ForeColor="Red" ></asp:Label> </td>
        </tr>
    <tr style="color: #000000">   
        <td style="text-align:right;width:100%;border:3px double green ">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" onclick="Add" ToolTip="ADD Form"/>
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
             CellPadding="2" DataKeyNames="tid"
                    CssClass="GridView"
               AllowSorting="True" PageSize ="20" AllowPaging="True">
                    <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                    <Columns>
  <asp:TemplateField HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="50px" />
</asp:TemplateField>

  <asp:BoundField DataField="keywordname" HeaderText="Keyword">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="kwdesc" HeaderText="Description">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="paracount" HeaderText="Parameter Count">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>                                         
                        <asp:BoundField DataField="helpingmsg" HeaderText="Helping message">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="Ktype" HeaderText="Keyword Type">
                         <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="ttype" HeaderText="Trip Type">
                         <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="isactive" HeaderText="IsActive">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                                             
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                               
                                  <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" ToolTip ="Edit Form Detail" AlternateText="Edit" />
                                 &nbsp;
                                 <asp:ImageButton ID="btnAddFields" runat="server" ImageUrl="~/images/addfields.jpg" Height="16px" Width="16px" OnClick="AddFields" ToolTip ="Add Fields" AlternateText="Add Fields" />
                                   &nbsp;
                                
                                <%--<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHit" ToolTip ="Delete Form" AlternateText="Delete"/>--%>
                                                             <asp:ImageButton ID="btndelete" runat="server" OnClick="deletehit" ImageUrl="~/images/closered.png" Height="16px" Width="16px"  ToolTip ="Apply Validation" AlternateText="Delete" />
                            </ItemTemplate>
                            <ItemStyle Width="220px" HorizontalAlign="Center"/>
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
 CancelControlID ="btnCloseForm" BackgroundCssClass ="modalBackground" DropShadow ="true" ></asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupForm" runat ="server" Width ="600px"  BackColor ="White" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td width="580px"><h3>Add New Keyword</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan ="2">
<div id="main">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"  >
    <ContentTemplate>
<div class="form" style="text-align:left"> 

<table cellspacing="5px" cellpadding="0px" width="580px" style="height:160px;" border="0px">
<tr><td style="width:155px">   <asp:Label ID="lblForm" runat="server" Text=""></asp:Label> 
</td></tr>


<tr>
   <td valign="middle" style="text-align:right; width:155px;"><label title="Enter SMS Keyword here" > *Keyword <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtkeyword" runat="server" CssClass="txtBox" 
           Width="100%" ></asp:TextBox> 
     </td>

<tr>
   <td valign="middle" style="text-align:right; width:155px;"><label title="Enter SMS Keyword Description here" > *Description <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtkwdesc" runat="server" CssClass="txtBox" 
           Width="100%"></asp:TextBox> 
     </td>
</tr>
    <tr>
   <td valign="middle" style="text-align:right; width:155px;"><label title="Enter SMS Helping Message here" > *Helping Message <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txthmsg" runat="server" CssClass="txtBox" 
           Width="100%"></asp:TextBox> 
     </td>
</tr>
<tr>
   <td valign="middle" style="text-align:right; width:155px;"><label title="Enter Parameter count" > *Parameter Count <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
       <asp:TextBox ID="txtparacnt" runat="server" CssClass="txtBox" 
           Width="100%"></asp:TextBox> 
     </td>
</tr>
<%--<tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Authentication Error!" > 
       *Authenication Error <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style="">
       <asp:DropDownList ID="ddlauth" runat="server" Width="100%" CssClass="txtBox"  ></asp:DropDownList>
     </td>
</tr>
<tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Existence Error!" > 
       *Existence Error <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
   <asp:DropDownList ID="ddlexist" runat="server" Width="100%" CssClass="txtBox"  ></asp:DropDownList>
       </td>
</tr>
<tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Processing Error!" > 
       *Processing Error <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
   <asp:DropDownList ID="ddlproc" runat="server" Width="100%" CssClass="txtBox"  ></asp:DropDownList>
   
     </td>
</tr>
<tr>
   <td valign="middle" style="text-align:right; width:175px;"><label title="Parameterized Error!" > 
       *Parameterized Error <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
   <asp:DropDownList ID="ddlpara" runat="server" Width="100%" CssClass="txtBox"  ></asp:DropDownList>
   
      </td>
</tr>
<tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Success Message!" > 
       *Success Message <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style=""> 
    <asp:DropDownList ID="ddlsucc" runat="server" Width="100%" CssClass="txtBox"  ></asp:DropDownList>
  
         </td>
</tr>--%>
    <asp:Panel ID="pnlumobno" runat="server" >
 <tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Select Static Type" > 
     *User MobileNo<img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style="">
       <asp:DropDownList ID="ddlumobno" runat="server" Width="100%" CssClass="txtBox"  >
       </asp:DropDownList>
     </td>
</tr></asp:Panel>
     <asp:Panel ID="pnldmobno" runat="server" >
 <tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Select Static Type" > 
     *Driver MobileNo<img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style="">
       <asp:DropDownList ID="ddldmobno" runat="server" Width="100%" CssClass="txtBox"  >
       </asp:DropDownList>
     </td>
</tr></asp:Panel>
     <asp:Panel ID="pnlddays" runat="server" >
 <tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Select Static Type" > 
     *Driver Restrict Days<img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style="">
      <asp:TextBox ID="txtddays" runat="server" CssClass="txtBox" 
           Width="100%"></asp:TextBox> 
     </td>
</tr></asp:Panel>
    <tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Select Keyword Type" > 
       *Keyword Type <img src="Images/Help.png" alt="" /> : 
  </label></td>
   <td style="">
       <asp:DropDownList ID="ddlKtype" runat="server" Width="100%" CssClass="txtBox" AutoPostBack="true"   >
           <asp:ListItem>Select Type</asp:ListItem>
           <asp:ListItem>Dynamic</asp:ListItem>
           <asp:ListItem>Static</asp:ListItem>
       </asp:DropDownList>
     </td>
</tr>
        <asp:Panel ID="pnltst" runat="server" Visible="false"  >
 <tr>
   <td valign="middle" style="text-align:right; width:165px;"><label title="Select Static Type" > 
     *Static Type<img src="Images/Help.png" alt="" /> : 
  </label>
          </td>
   <td style="">
       <asp:DropDownList ID="ddltst" runat="server" Width="100%" CssClass="txtBox"  >
           <asp:ListItem>eLog</asp:ListItem>
           <asp:ListItem>Trip Start</asp:ListItem>
           <asp:ListItem>Trip End</asp:ListItem>
           <asp:ListItem>DeLog</asp:ListItem>
           <asp:ListItem>DTrip Start</asp:ListItem>
           <asp:ListItem>DTrip End</asp:ListItem>
           <asp:ListItem>Nil Trip Driver</asp:ListItem>
            <asp:ListItem>Nil Trip User</asp:ListItem>
       </asp:DropDownList>
     </td>
</tr></asp:Panel>
    

<tr><td colspan="1" style="width: 155px; height: 25px;"  >


</td>
<td style="text-align:left; height: 25px;">
<asp:CheckBox ID="chkactive" Checked="True"   runat="server" Text="IsActive" />
</td>
</tr>
<tr>
<td style="width: 155px"> 
   </td>
        <td align="left">
            <asp:Button ID="btnsavekw" runat="server"  CssClass="btnNew" 
                Text="Save" Width="100Px" />
                
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


 


<asp:Button id="btnShowPopupEdit" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                               >
</asp:ModalPopupExtender><br /><br /><br /><br /><br /><br /><br /><br /><br />
<asp:Panel ID="pnlPopupEdit" runat="server" Width="860px" Height ="300px"  >
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:840px"><h3>New / Update Field</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2" style="text-align:left">

<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
<div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%"   style="border-color:red; border-width:thin; ">
<tr><td  colspan="6">
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
    </td></tr>
<tr>     
      
      <td style="text-align:right">
        <asp:Label ID="lblst" runat="server" Text="Setting Type: "></asp:Label></td>
    <td style="width:90px">
       <asp:DropDownList ID="ddlstype" runat="server" Width="140PX"  
            AutoPostBack="True">
           <asp:ListItem Value="1">Authentication</asp:ListItem>
           <asp:ListItem Value="2">Existence</asp:ListItem>
           <asp:ListItem Value="3">Processing</asp:ListItem>
           <asp:ListItem Value="4">Where</asp:ListItem>
       </asp:DropDownList>
      </td>
     <td style="text-align:right">
         <asp:Label ID="lblptype" runat="server" Text="Process Type: "></asp:Label></td>
    <td style="width:90px">
                            <asp:DropDownList ID="ddlprc" runat="server" Width="140PX" >
                                <asp:ListItem>INSERT</asp:ListItem>
                                <asp:ListItem>UPDATE</asp:ListItem>
                            </asp:DropDownList>
    </td>
    <td style="text-align:right">
<asp:Label ID="lbldty" runat="server" Text="Document Type: "></asp:Label>
</td>
<td style="width:180px"><asp:DropDownList runat="server" AutoPostBack="true"   ID="ddldtype" Width="180PX"></asp:DropDownList></td>
 </tr>
 <tr>
<td style="text-align:right"><asp:Label ID="lblfn" runat="server" Text="Field Name: "></asp:Label></td>
<td><asp:DropDownList runat="server"  ID="ddlfn" Width="140PX"></asp:DropDownList></td>
    <td style="text-align:right">
<asp:Label ID="lblctype" runat="server" Text="Change Type: "></asp:Label></td>
     <td>
        <asp:DropDownList ID="ddlopr" runat="server" Width="140PX">
            <asp:ListItem Value="=">=</asp:ListItem>
            <asp:ListItem Value="Append">Append</asp:ListItem>
             <asp:ListItem Value="Remove">Remove</asp:ListItem>
             <asp:ListItem Value="Remove All">Remove All</asp:ListItem>
         </asp:DropDownList>
</td >
    <td style="text-align:right"><asp:Label ID="lblpara" Text="Para Value: " runat="server" ></asp:Label></td>
<td style="width:180px">
    <asp:DropDownList ID="ddlparavalue" Width="90PX" runat="server"></asp:DropDownList>
    <asp:TextBox ID="txtRole" runat="server" Width="80px"></asp:TextBox>
</td>
</tr>

          <tr>
            <td align="center" colspan="6">
                <div style="width:100%;text-align:center ">
                    <asp:Button ID="btnsavefield" runat="server" CssClass="btnNew" Font-Bold="True" 
                        Font-Size="X-Small"  Text="Save"  
                        Width="100px" />
                </div>
                <br />
            </td>
     </tr> 
          
  
 <tr>
 <td align="left" colspan="6"> 
     <asp:Panel ID="PnlGrid" runat="server" Height="100%" 
         Width="770px">
         <div style="overflow:scroll; width:770px; height:200px; ">
         <asp:GridView ID="gvField" runat="server" AllowPaging="false" 
             AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" 
             DataKeyNames="TID" >
               <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
             <Columns>
                 <asp:TemplateField HeaderText="S.No">
                     <ItemTemplate>
                         <%# CType(Container, GridViewRow).RowIndex + 1%>
                     </ItemTemplate>
                     <ItemStyle Width="50px" />
                 </asp:TemplateField>
                <asp:BoundField DataField="settingtype" HeaderText="Setting Type">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="documenttype" HeaderText="Documenttype">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Field" HeaderText="Field">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="tablename" HeaderText="Table">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField> <asp:BoundField DataField="keyword" HeaderText="Keyword">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField> <asp:BoundField DataField="errmsg" HeaderText="Error Msg">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="paravalue" HeaderText="Para Value">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ProcType" HeaderText="Proccess Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CType" HeaderText="Change Type">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                 <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>
                        <asp:ImageButton ID="btnEditField" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHitField" ToolTip ="Edit fields" AlternateText="Edit" />
                         &nbsp;
                         <asp:ImageButton ID="btnDeleteUser" runat="server" AlternateText="Delete" OnClick="del" Height="16px" ImageUrl="~/images/closered.png"  
                             ToolTip="Delete" Width="16px" />
                            <%-- <asp:ImageButton ID="btnLockUser" runat="server" AlternateText="Lock" Height="16px" ImageUrl="~/images/Lock.PNG" OnClick="LockHitField" 
                             ToolTip="LOCK/UNLOCK" Width="16px" />--%>
                          </ItemTemplate>
                     <ItemStyle HorizontalAlign="Center" Width="140px" />
                 </asp:TemplateField>
             </Columns>
         </asp:GridView>
         </div>
     </asp:Panel>
     </td>    
    </tr>
</table>
</div>
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>


<asp:Button id="btnShowPopupDeleteField" runat="server" style="Display:none" />
<asp:ModalPopupExtender ID="Modelpopup1" runat="server" 
                                TargetControlID="btnShowPopupDeleteField" PopupControlID="pnlPopupDeleteField" 
                CancelControlID="btnCloseDeleteField" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupDeleteField" runat="server" Width="500px" style="Display:none " BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Field Delete : Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDeleteField" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updDeleteField" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<h2> <asp:Label ID="lblmessagedelete" runat="server" Font-Bold="True" Text="Are you sure want to Delete" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnDeleteField" runat="server" Text="Yes Delete"  Width="90px"  CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>

</table> 
</div>
</asp:Panel>

</asp:Content>


