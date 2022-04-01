<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="WSOutward, App_Web_qpjniz5y" viewStateEncryptionMode="Always" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
    <Triggers>
  
    </Triggers>
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
               <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h3>WebService Outward</h3></asp:Label>
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
            <%--<td style="width: 90px;"> 
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Field Name" Width="99%"> </asp:Label>
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
         </td>--%>

        <%-- <td style="text-align: right;width:25px">
             <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" 
                 ImageUrl="~/Images/search.png" />

          </td>
          <td>
              &nbsp;
             </td>--%>

         <td style="text-align: right;">
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Plus.jpg" onclick="Add"/>
                          &nbsp;<%--<asp:ImageButton ID="btnexport" runat="server" Visible="false"  ToolTip="Export"  ImageUrl="~/Images/excel.gif" />--%><caption>
                                  &nbsp;
             </caption>
          
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
         <div style=" overflow:scroll; height:400px; width:1000px">
       <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
             CellPadding="2" DataKeyNames="Tid" DataSourceID="SqlData"
                    ForeColor="#333333" Width="100%" 
               AllowSorting="False" AllowPaging="False" >
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                    <EditRowStyle BackColor="#2461BF" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
<asp:TemplateField HeaderStyle-HorizontalAlign="Left"  HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
    <HeaderStyle HorizontalAlign="Left" />
</asp:TemplateField>
                        <asp:BoundField DataField="wstype" HeaderText="WS Type"  >
                            <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                        <asp:BoundField DataField="Type" HeaderText="Method Type">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:BoundField DataField="doctype" HeaderText="Document Type">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                               <asp:BoundField DataField="URI" HeaderText="URI">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="ParaSeprator" HeaderText="Parameter Seperator">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="dateformat" HeaderText="Date Format">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="ParaList" HeaderText="Parameter List">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                            
                                 
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/editrole1.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit WS"  AlternateText="Edit" />
                                &nbsp;
                                   <asp:ImageButton ID="btnDtl" ImageUrl="~/images/closered.png"  runat="server" Height="16px" Width="16px"  ToolTip="Delete Role" OnClick="DeleteHit" AlternateText="Delete" />
                            </ItemTemplate>
                            <ItemStyle Width="90px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                      </Columns>
                </asp:GridView>
                 <asp:SqlDataSource ID="SqlData" runat="server" 
               ConnectionString="<%$ ConnectionStrings:conStr %>" 
               SelectCommand="uspGetResultWSOutward" SelectCommandType="StoredProcedure">
               <SelectParameters>
                                  <asp:SessionParameter Name="eid" SessionField="EID" Type="String"  />
               </SelectParameters>
           </asp:SqlDataSource>
       
      
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

<asp:Panel ID="pnlPopupEdit" runat="server" Width="700px"  BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:580px"><h3>Create / Update Service</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">

<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 

   <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
         <tr> <td style="width:125px" align="left"><b>WebService Type </b></td>
           
               <td align="left"><asp:DropDownlist ID="ddlwstype" runat="server" Width="98%">
       <asp:ListItem Value="0">Create</asp:ListItem>
                   <asp:ListItem Value="1">Edit</asp:ListItem>
                     </asp:DropDownlist></td>
       
           <td style="width:180px;height:250px;" rowspan="8">
           <span style="font-size:xx-small;">Copy  & Paste to Parameter List</span>
                <asp:TextBox ID="txtdisplaylist" TextMode="MultiLine" ReadOnly="true"  runat="server" Height="300px" Width="180px"></asp:TextBox>
            </td>
       </tr>
   <tr >
    <td style="width:125px" align="left"><b>Method Type </b></td>
    <td align="left"><asp:DropDownlist ID="ddltype" runat="server" Width="98%">
       
                     </asp:DropDownlist></td>
       
       

   </tr>
<tr>
    <td style="width:125px" align="left"><b>Document Type </b></td>
    <td align="left">
        <asp:DropDownList ID="ddldoctype" AutoPostBack="true"  runat="server" CssClass="txtbox"  Width="98%">
                         </asp:DropDownList>
    </td>
   </tr>
   <tr>
    <td style="width:125px" align="left"><b>URL </b></td>
    <td align="left"><asp:TextBox ID="txtURI" runat="server" Width="98%"></asp:TextBox></td>
   </tr>
     
        <tr>
    <td style="width:125px" align="left"><b>Parameter Seprator </b></td>
    <td align="left"><asp:TextBox ID="txtParameterSeprator" runat="server" Width="98%"></asp:TextBox></td>
   </tr>
       <tr>
    <td style="width:125px" align="left"><b>Date Format </b></td>
    <td align="left"><asp:TextBox ID="txtdatef" runat="server" Width="98%"></asp:TextBox></td>
   </tr>
     
       <tr>
    <td style="width:125px" align="left"><b>Parameter List </b></td>
    <td align="left"><asp:TextBox ID="txtParameterList" TextMode="MultiLine"  runat="server" Width="98%"></asp:TextBox></td>
   </tr>
  
       <tr>
    <td style="width:125px" align="left"><b>Remarks </b></td>
    <td align="left"><asp:TextBox ID="txtRemarks" runat="server" Width="98%"></asp:TextBox></td>
   </tr>
  

  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActEdit" runat="server" Text="Update" 
                            OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 

</td>
</tr>
</table> 
</div>
</asp:Panel>


<asp:Button id="btnShowPopupDelFolder" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder" 
                CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Delete Service</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelFolder" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete this Service" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete" 
                            OnClick="DelFile" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
</asp:Content>
