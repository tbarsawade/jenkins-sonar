<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" validaterequest="false" autoeventwireup="false" inherits="authMetrix, App_Web_sfds111l" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style type="text/css">
    .water
    {
         font-family: Tahoma, Arial, sans-serif;
         font-style:italic;
         color:gray;
    }
  </style>

<div class="form">
    <div class="doc_header">
            Dynamic Authority Matrix
        </div>
    <div class="row mg">
        <div class="col-md-12 col-sm-12">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
   <ContentTemplate> 

       <div class="col-md-12 col-sm-12">             
           <asp:Label ID="lblRecord" runat="server"  ForeColor="red" Font-Size="Small" ></asp:Label>                             
      </div>

       <div class="col-md-12 col-sm-12">   
           <div class="col-md-1 col-sm-1">           
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Text="Document Type">
                 </asp:Label>
            </div>
           <div class="col-md-1 col-sm-1">              
             <asp:DropDownList ID="ddlDocumentType" runat="server" CssClass ="txtBox"                   
                 AutoPostBack="True">
             </asp:DropDownList>
                 </div>
         <div class="col-md-1 col-sm-1">           
         <asp:Label ID="Label5" runat="server" Font-Bold="True" Text="Doc Nature"></asp:Label>
               </div> 
           <div class="col-md-1 col-sm-1">           
              <asp:DropDownList ID="ddlSeleDocNature" runat ="server" CssClass ="txtBox">
    <asp:ListItem Text="SELECT" Value ="SELECT"> </asp:ListItem>
    <asp:ListItem Text="CREATE" Value ="CREATE"> </asp:ListItem>
    <asp:ListItem Text="MODIFY" Value ="MODIFY"> </asp:ListItem>
    <asp:ListItem Text="CANCEL" Value ="CANCEL"> </asp:ListItem>
  </asp:DropDownList>
               </div>
           <div class="col-md-1 col-sm-1">           
         <asp:Label ID="Label4" runat="server" Text="Select Type"></asp:Label>
           </div>
           <div class="col-md-1 col-sm-1">           
              <asp:DropDownList ID="ddlSelectType" AutoPostBack="true" runat ="server" CssClass ="txtBox" >
    <asp:ListItem Text="SELECT" Value ="SELECT"> </asp:ListItem>
    <asp:ListItem Text="ROLE" Value ="ROLE"> </asp:ListItem>
    <asp:ListItem Text="USER" Value ="USER"> </asp:ListItem>
  <asp:ListItem Text="NEWROLE" Value ="NEWROLE"> </asp:ListItem>
  </asp:DropDownList>
               </div>
           <div class="col-md-1 col-sm-1" >          
         <asp:Label ID="Label1" runat="server" Font-Bold="True"  Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Select User" Width="99%"></asp:Label>
               </div>
           <div class="col-md-1 col-sm-1">           
             <asp:DropDownList ID="ddlSelectRole" runat="server" CssClass ="txtBox" >
             </asp:DropDownList>
               </div>
           <div class="col-md-1 col-sm-1">           
         <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Status" ></asp:Label>
               </div>        
           <div class="col-md-1 col-sm-1">           
             <asp:DropDownList ID="ddlStatus" runat="server" CssClass ="txtBox" >
             </asp:DropDownList>
               </div>       
           <div class="col-md-2 col-sm-2">           
             <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" 
                 ImageUrl="~/Images/search.png"  ToolTip="Search"/>
               <asp:ImageButton ID="btnAdd" runat="server" Width="20px" Height="20px" 
                 ImageUrl="~/Images/plus.jpg"  ToolTip="Add New Record" Enabled ="false"  OnClick ="ADD"/>
               </div>        
               
         <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
         </div>
     </ProgressTemplate>
      </asp:UpdateProgress>
                </div>

    <%--<tr style="color: #000000">--%>
       <div class="col-md-12 col-sm-12">   

     <%--<td style="text-align: left;" valign="top">--%>
    <asp:GridView ID="gvData" runat="server" 
             CellPadding="2" DataKeyNames="tid"
                    ForeColor="#333333" Width="100%" 
               AllowSorting="True" AllowPaging="false" >
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
 <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" AlternateText="Edit" />
                                 &nbsp;
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHit" AlternateText="Delete"/>
                               <asp:ImageButton ID="btnDown" runat="server" AlternateText="Move Down" ToolTip="Move Down"  
                                   Height="16px" ImageUrl="~/images/down.png" OnClick="PositionDown" Width="16px" />
                               &nbsp;
                              <asp:ImageButton ID="btnUp" runat="server" AlternateText="Move Up" ToolTip="Move Up"  
                                   Height="16px" ImageUrl="~/images/up.png" OnClick="PositionUp" Width="16px" />
                               &nbsp;
                            </ItemTemplate>
                           <ItemStyle Width="120px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                       </Columns>
                </asp:GridView>
<%--    <asp:SqlDataSource ID="SqlData" runat="server" 
               ConnectionString="<%$ ConnectionStrings:conStr %>" 
               SelectCommand="uspGetGridByMasterType1New" SelectCommandType="StoredProcedure">
               <SelectParameters>
                   <asp:ControlParameter ControlID="ddlField" Name="sField" 
                       PropertyName="SelectedValue" Type="String" />
                   <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"  
                       Type="String" />
                   <asp:SessionParameter DefaultValue="0" Name="eid" SessionField="EID" 
                       Type="Int32" />
                   <asp:QueryStringParameter DefaultValue="" Name="documentType" 
                       QueryStringField="SC" Type="String" />
               </SelectParameters>
           </asp:SqlDataSource>
         <asp:GridView ID="gvexport" style="display:none;" runat="server" CssClass="GridView" 
             CellPadding="2" DataKeyNames="tid"
                    ForeColor="#333333" Width="100%"  
               AllowSorting="False" AllowPaging="False" DataSourceID="SqlData">
                    <FooterStyle  CssClass="FooterStyle" />
                    <RowStyle CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EmptyDataRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle"/>
                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                    <Columns>
                    </Columns>
                </asp:GridView>
       --%>
       <asp:Button id="btnstatus" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="modalstatus" runat="server" 
                                TargetControlID="btnstatus" PopupControlID="pnlstatus" 
                CancelControlID="ImageButton1" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlstatus" runat="server" Width="500px" Height="300px"  style="display:none;"   BackColor="aqua">
<div class="box" > 
<table cellspacing="2px" cellpadding="2px" width="100%" border="1px" >
<tr>
<td><h3> <asp:Label ID="lblstatus" runat="server" text="Data Uploading Status" Font-Bold="True"></asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="ImageButton1" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr style=" border:1px solid black;"><td><asp:Label ID="lblstat" runat="server" Font-Bold="True"></asp:Label></td></tr>
<tr style=" border:1px solid black;"><td><asp:Label ID="Label3" runat="server" Font-Bold="True"></asp:Label></td></tr>
<tr><td><table border="1px" ><tr><td width="217"><span style="color:Black; font-weight:bold;">Row No</span></td><td width="217px"><span style="color:Black; font-weight:bold;">Column Where Error Occured</span></td></tr></table></td></tr>
<tr style=" border:1px solid black;"><td> <div style="overflow:scroll; height:300px"> <asp:Label ID="lblstatus1" runat="server" /></div></td></tr>
</table>
</div>
</asp:Panel>
          
    <%--</td> 
     </tr>--%>
       </div>
    <%--   </table>--%>
     </ContentTemplate> 
</asp:UpdatePanel> 
            </div>
    </div>

<asp:Button id="btnShowPopupEdit" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupEdit" runat="server" Width="900px"  Height ="500px" style="" BackColor="aqua" ScrollBars="Auto">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:870px"><h3> Add / Edit Auth Matrix</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">

<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
 <asp:Panel ID ="pnlPoupup" runat ="server" Width="600px" >
 <asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label></asp:Panel> 
 <asp:Panel ID="pnluser" runat ="server" Width ="100%" >
 <table width ="100%">

 <tr>
  <td colspan="6"> 
   <table width="100%">

   <tr> 
  <td align ="right"><label>Select Doc Nature:</label></td>
  <td align ="left"><asp:DropDownList ID="ddlDocNature" AutoPostBack="true" runat ="server" CssClass ="txtBox" Width ="170px">
    <asp:ListItem Text="SELECT" Value ="SELECT"> </asp:ListItem>
    <asp:ListItem Text="CREATE" Value ="CREATE"> </asp:ListItem>
    <asp:ListItem Text="MODIFY" Value ="MODIFY"> </asp:ListItem>
    <asp:ListItem Text="CANCEL" Value ="CANCEL"> </asp:ListItem>
  </asp:DropDownList> </td>  
   </tr> 

   <tr> 
  <td align ="right"><label>Select Type:</label></td>
  <td align ="left"><asp:DropDownList ID="ddlType" AutoPostBack="true" runat ="server" CssClass ="txtBox" Width ="170px">
    <asp:ListItem Text="SELECT" Value ="SELECT"> </asp:ListItem>
    <asp:ListItem Text="ROLE" Value ="ROLE"> </asp:ListItem>
    <asp:ListItem Text="USER" Value ="USER"> </asp:ListItem>
    <asp:ListItem Text="NEWROLE" Value ="NEWROLE"> </asp:ListItem>
  </asp:DropDownList> </td>  
   </tr> 
  </table>

  </td>
 </tr>

  <tr>
  <td align ="right"><label>Select Role/User:</label></td>
  <td align ="left" ><asp:DropDownList ID="ddlRole" runat ="server" AutoPostBack="true" CssClass ="txtBox" Width ="170px"  ></asp:DropDownList> </td>  
 
  <td align ="right"><asp:Label ID="lblfldName" runat="server" Text="Select Field Name:" Font-Bold="true"></asp:Label> </td>
  <td align ="left" ><asp:DropDownList ID="ddlFieldName" runat ="server" CssClass ="txtBox" Width ="170px"  ></asp:DropDownList> </td>  

  <td align ="right"> <asp:Label ID="lblLevel" runat="server" Text="Level:" Font-Bold="true"></asp:Label> </td>
  <td align="left"> <asp:TextBox ID="txtLevel" runat="server" CssClass="txtBox" Width="66px"></asp:TextBox> </td>
 </tr>

 <tr>
 <td align ="right"><label>Approve Status:</label></td>
 <td align ="left" > <asp:DropDownList ID="ddlstatus1" AutoPostBack="true" runat ="server"  CssClass ="txtBox" Width ="170px" ></asp:DropDownList></td>
 <td align ="right"> <label>ORDERING:</label></td>
 <td align="left"><asp:TextBox ID="txtOrdering" runat="server" CssClass="txtBox" Width="66px"></asp:TextBox> </td>
 <td align ="right"> <label>SLA:</label></td>
 <td align="left"> <asp:TextBox ID="txtsla" runat="server" CssClass="txtBox" Width="66px"></asp:TextBox> </td>

 </tr>

 </table>

 </asp:Panel>

 <asp:Panel ID="pnlFields" Width="100%" runat="server">
 </asp:Panel>

 <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Update"  
                             CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
 </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 

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
<td><h3>Auth Matrix Delete : Confirmation</h3></td>
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

        </div>

  <%--<asp:Button id="btnim" runat="server" style=" display:none;" />
    <asp:ModalPopupExtender ID="modalpopupimport" runat="server" 
                        TargetControlID="btnim" PopupControlID="pnlimport" 
        CancelControlID="ImageButton2" BackgroundCssClass="modalBackground" 
                        DropShadow="true" >
    </asp:ModalPopupExtender>

<asp:Panel ID="pnlimport" runat="server" Width="400px" Height="100px" BackColor="white">
<div class="box" > 
<table cellspacing="1px" cellpadding="2px" width="100%" border="0px" >
<tr>
<td><h3> <asp:Label ID="lblpophead" runat="server" Text="Import Csv File"  Font-Bold="True"></asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="ImageButton2" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>

<tr >
<td colspan="2"><br /><asp:FileUpload ID="impfile" CssClass="CS"   runat="server" /> &nbsp;
             <asp:ImageButton ID="btnimport" ToolTip="Import" style=" vertical-align:bottom;" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/import.png"/>&nbsp;
              <asp:ImageButton ID="helpexport"  ToolTip="Download Import Sample & Format" runat="server" style=" vertical-align:bottom;" Width="20px" Height="20px" ImageUrl="~/Images/Helpexp.png"/>
             &nbsp;</td>

</tr>
</table>
</div>
</asp:Panel>--%>


</asp:Content>

