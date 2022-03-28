<%@ page title="File Explorer" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="Explorer, App_Web_iqn0gzeb" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script language="javascript" type="text/javascript">
       function showbutt() {
           $("#hideShow").css("display", "block");
       }

</script>
<style type="text/css" >
.GridPager span { color:Black; text-align:center; font-size:18px;}

</style>

<div id="main">
   <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
  <td style="width:230px;" valign="top" align="left">
<asp:UpdatePanel ID="UpdMsgTV" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
  <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
 <tr>
    <td style="width:130px;" valign="middle" align="left">
    <h1>File Explorer</h1>
</td>  

 <td style="padding-right:15px" valign="middle" align="right">
    <asp:ImageButton ID="btnCreateFolder" runat="server" 
         ImageUrl="~/images/AddFolder.png" onclick="AddFolderHit" 
         AlternateText="Create Folder" ToolTip="Create Folder" />
    <asp:ImageButton ID="btnRenameFolder" runat="server" 
         ImageUrl="~/images/Folderrename.png" onclick="FolderRenameHit" 
         AlternateText="Rename Folder" ToolTip="Rename Folder" />
    <asp:ImageButton ID="btnDeleteFolder" runat="server" 
         ImageUrl="~/images/DelFolder.png" onclick="DelFolderHit" 
         AlternateText="Delete Folder" ToolTip="Delete Folder" />

</td>
  </tr>
 </table>

</ContentTemplate> 
</asp:UpdatePanel> 

</td> 
  <td style="" valign="top" align="left">
<asp:UpdatePanel ID="UpdMsg" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>

  <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
 <tr>
    <td style="width:70%;" valign="middle" align="left">
    <h1>
       <asp:Label ID="lblFolderName" runat="server" Text="Folder Name"></asp:Label>
        </h1>
</td>  

 <td style="" valign="middle" align="right">
    <asp:ImageButton ID="btnUploadFile" runat="server" onclick="UploadFileHit"
         ImageUrl="~/images/Addfile.png" AlternateText="Upload File" 
         ToolTip="Upload Files In Selected Folder" />

    <asp:ImageButton ID="btnMoveFiles" runat="server" 
         ImageUrl="~/images/movefile.png" onclick="MovFileHit" 
         AlternateText="Move File(s)" ToolTip="Move Files to Folder" />
</td>
  </tr>
 </table>

</ContentTemplate> 
</asp:UpdatePanel> 

</td> 
    </tr>
       <tr>
    <td  valign="top" align="left">
 <div class="form" style="margin-left:-5px; overflow:auto ;width:220px; height:380px;">
<asp:UpdatePanel ID="updTV" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
        <asp:TreeView ID="tv"  runat="server"  Height="100%" ImageSet="Inbox" 
            Width="100%">
          
            <ParentNodeStyle Font-Bold="False" />
            
            <HoverNodeStyle Font-Underline="True" />
            <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" 
                VerticalPadding="0px" />
            <NodeStyle Font-Names="Verdana" Font-Size="8pt"  ForeColor="Black" 
                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
        </asp:TreeView>
</ContentTemplate> 
</asp:UpdatePanel> 
    </div>    
 
    </td>
<td style="padding-top : 5px " valign="top" align="left">



    <table cellpadding="0px" cellspacing="0px" style="text-align: left; height:380px;"  width="100%"  border="0px">
          <tr style="color: #000000">
            <td style="text-align: left;" valign="top" >
               <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ></asp:Label>
                    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional"  >
   <ContentTemplate> 
       <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
             CellPadding="3" DataKeyNames="tid" Width="100%" 
               AllowSorting="True" OnPageIndexChanging="gridView_PageIndexChanging" OnSorting="gridView_Sorting" 
             PageSize="16" AllowPaging="True" BackColor="White" BorderColor="#CCCCCC" 
           BorderStyle="None" BorderWidth="1px">
                    <FooterStyle BackColor="#007DBB" CssClass="GridHeader" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="White" ForeColor="#000066" CssClass="GridPager"  
                        HorizontalAlign="Left"   />
                    <HeaderStyle BackColor="#007DBB" Font-Bold="True" ForeColor="White"  
                        CssClass="GridHeader"  />
                        <AlternatingRowStyle BackColor="#d8dbe5"/>
                   
                    <Columns>
                   <asp:TemplateField>
                                 <ItemTemplate>
                                      <asp:CheckBox ID="Check" runat="server" />
                                 </ItemTemplate>
                            <ItemStyle Width="20px" />
                   </asp:TemplateField>
  <asp:TemplateField >
   <ItemTemplate>    
       <img src="images/<%# DataBinder.Eval(Container.DataItem, "DocImage")%>" alt="filetype" /> 
   </ItemTemplate>
      <ItemStyle Width="20px" />
</asp:TemplateField>


  <asp:BoundField DataField="FName" SortExpression="FName"  HeaderText="File Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>


  <asp:BoundField DataField="UserName" SortExpression="UserName"  HeaderText="Uploaded By">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

  <asp:BoundField DataField="adate" SortExpression="adate"  HeaderText="Upload Date">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

  <asp:BoundField DataField="filesize" SortExpression="filesize"  HeaderText="Size">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        
                          <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                             <asp:ImageButton ID="btnView" ImageUrl="~/images/show.png"  runat="server" Height="16px" Width="16px" OnClick="ShowHit"  ToolTip="View / Download File"/>
                                 &nbsp;
                                <asp:ImageButton ID="btnRename" runat="server" Visible="false"   ImageUrl="~/images/filerename.png" Height="16px" Width="16px" ToolTip="Rename File" OnClick="renameFileHit"   />
                                 &nbsp;
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/close.png" Height="16px" Width="16px" ToolTip="Delete File" OnClick="DelFileHit"   />
                            </ItemTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="left"/>
                        </asp:TemplateField>

                      </Columns>
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>
                </ContentTemplate> 
                </asp:UpdatePanel> 
         <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>
      </td>
    </tr>
         
         </table>
                         

</td>    
</tr>
</table> 
</div>





<asp:Button id="btnShowPopupRenameFile" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnRenameFile_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupRenameFile" PopupControlID="pnlPopupRenameFile" 
                CancelControlID="btnCloseRenameFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupRenameFile" runat="server" Width="500px" style="Display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Rename File</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseRenameFile" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelRenameFile" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgRenameFile" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>

        <tr>
           <td align="left" style="width:125px">
               <b>Folder Name</b></td>
           <td align="left">
               <asp:TextBox ID="txtFileReName" runat="server" Width="98%"></asp:TextBox>
           </td>
       </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActRenameFile" runat="server" Text="Rename" 
                            OnClick="RenameFile" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>




<asp:Button id="btnShowPopupRenameFolder" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnRenameFolder_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupRenameFolder" PopupControlID="pnlPopupRenameFolder" 
                CancelControlID="btnCloseRenameFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupRenameFolder" runat="server" Width="500px" style="Display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Rename Folder</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseRenameFolder" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelRenameFolder" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgRenameFolder" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    </td>
   </tr>

        <tr>
           <td align="left" style="width:125px">
               <b>New File Name</b></td>
           <td align="left">
               <asp:TextBox ID="txtFolderReName" runat="server" Width="98%"></asp:TextBox>
           </td>
       </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActRenameFolder" runat="server" Text="Rename" 
                            OnClick="RenameFolder" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>










<asp:Button id="btnShowPopupAddFolder" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnAddFolder_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupAddFolder" PopupControlID="pnlPopupAddFolder" 
                CancelControlID="btnCloseAddFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupAddFolder" runat="server" Width="500px" style="Display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Add Folder</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseAddFolder" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelAddFolder" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgAddFolder" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>

        <tr>
           <td align="left" style="width:125px">
               <b>Folder Name</b></td>
           <td align="left">
               <asp:TextBox ID="txtFolderName" runat="server" Width="98%"></asp:TextBox>
           </td>
       </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActAddFolder" runat="server" Text="Create" 
                            OnClick="AddFolder" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>





<asp:Button id="btnShowPopupAddFile" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnAddFile_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupAddFile" PopupControlID="pnlPopupAddFile" 
                CancelControlID="btnCloseAddFile" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupAddFile" runat="server" Width="700px" style="display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td><h3>Upload File</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseAddFile" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2" style="text-align:left">
  <div class="form">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
        <tr>
        <td>

<asp:UpdatePanel ID="updatePanelAddFile" runat="server" UpdateMode="Conditional"  >
 <ContentTemplate> 
    <asp:Label ID="lblMsgAddFile" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
 <br />

     <div id="fileupload"  style="width:100%;text-align:left;">
       <asp:Label runat="server" ID="myThrobber" Style="display: none;"><img align="absmiddle" alt="" src="images/uploading.gif"/></asp:Label>
             <ajaxToolkit:AjaxFileUpload ID="AFlu" runat="server" Padding-Bottom="4"
            Padding-Left="2" Padding-Right="1" Padding-Top="4" ThrobberID="myThrobber" 
            OnUploadComplete="fileOnUploadComplete" OnClientUploadComplete="showbutt"  MaximumNumberOfFiles="10"
            />
         <%--   AllowedFileTypes="jpg,jpeg,png,doc,pdf,xls" --%>
      </div> 

   <asp:Panel ID="pnlFields" Width="100%" runat="server">
            </asp:Panel>
 <br />
           <div id="hideShow"  style="width:100%;text-align:right;display:none ">
                    <asp:Button ID="btnConfirm" runat="server" Text="Save" 
                             CssClass="btnNew" Font-Bold="True" OnClick="ValidateData" 
                            Font-Size="X-Small" Width="100px" />

                    </div>
   </ContentTemplate> 
   </asp:UpdatePanel> 
         
        </td>
        </tr>
  </table>
  </div>  
</td>
</tr>
</table> 
</div>
</asp:Panel>





<%--For Adding Folder
--%>

<asp:Button id="btnShowPopupDelFolder" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder" 
                CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" style="Display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Delete Folder</h3></td>
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
    <asp:Label ID="lblMsgDelFolder" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete" 
                            OnClick="DelFolder" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>









<%--For Moving Files
--%>

<asp:Button id="btnShowPopupMovFile" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnMovFile_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupMovFile" PopupControlID="pnlPopupMovFile" 
                CancelControlID="btnCloseMovFile" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupMovFile" runat="server" Width="500px" Height="500px" style="display:none">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Move File</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseMovFile" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelMovFile" runat="server" UpdateMode="Conditional" style="overflow:auto; height:500px;">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgMovFile" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>

   <tr>
   <td>
   

        <asp:TreeView ID="tvMove" runat="server" Height="100%" ImageSet="Inbox" 
            Width="100%">
            <ParentNodeStyle Font-Bold="False" />
            <HoverNodeStyle Font-Underline="True" />
            <SelectedNodeStyle Font-Underline="False" HorizontalPadding="0px" 
                VerticalPadding="0px" BackColor="#66FF66" Font-Bold="True" />
            <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" 
                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
        </asp:TreeView>
   
   
   
   </td>
   </tr>

  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActMovFile" runat="server" Text="Yes Move" 
                            OnClick="MovFile" CssClass="btnNew" Font-Bold="True" 
                            Font-Size="X-Small" Width="100px" />
                    </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>





<%--For Deleting File
--%>

<asp:Button id="btnShowPopupDelFile" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnDelFile_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelFile" PopupControlID="pnlPopupDelFile" 
                CancelControlID="btnCloseDelFile" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupDelFile" runat="server" Width="500px" style="Display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Delete File</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelFile" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelFile" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgDelFile" runat="server" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActDelFile" runat="server" Text="Yes Delete" 
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

