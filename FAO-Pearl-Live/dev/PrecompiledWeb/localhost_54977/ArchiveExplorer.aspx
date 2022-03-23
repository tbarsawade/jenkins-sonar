<%@ page title="Archive Explorer Explorer" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ArchiveExplorer, App_Web_ju5zdh34" viewStateEncryptionMode="Always" %>
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

        <script type="text/javascript">

            function OpenWindow(url) {

                var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
                //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
                return false;
            }
            $(document).ready(function () {
                //    $('#panel1').slidePanel({
                //	triggerName: '#trigger1',
                //	position: 'fixed',
                //	triggerTopPos: '65px',
                //	panelTopPos: '95px'
                //});

                //$('.detail').each(function() {
                //    var $link = $(this);
                //    var $dialog = $('<div></div>')
                //		.load($link.attr('href'))
                //		.dialog({
                //		    autoOpen: false,
                //		    title: 'Document Detail',
                //		    width: 700,
                //		    height: 550,
                //		    modal: true
                //		});

                //    $link.click(function() {
                //        $dialog.dialog('open');
                //        return false;
                //    });
                //});
            });

            function ShowDialog(url) {
                // do some thing with currObj data

                var $dialog = $('<div></div>')
            .load(url)
            .dialog({
                autoOpen: true,
                title: 'Document Detail',
                width: 700,
                height: 550,
                modal: true
            });
                return false;
            }
</script>


<div id="main">
   <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
  <td style="width:230px;" valign="top" align="left">
    <h1>Archive Explorer</h1>
</td> 
  <td style="" valign="top" align="left">
<asp:UpdatePanel ID="UpdMsg" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
      <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
 <tr>
    <td style="width:600px;" valign="middle" align="left">
    <h1>
       <asp:Label ID="lblFolderName" runat="server" Text="Folder Name"></asp:Label>
   </h1>
    </td>  
  </tr>
 </table>
 </ContentTemplate> 
</asp:UpdatePanel> 
</td> 
    </tr>
    <tr>
    <td  valign="top" align="left">
 <div class="form" style="margin-left:-5px; overflow: auto;width:220px">
<asp:UpdatePanel ID="updTV" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
        <asp:TreeView ID="tv" runat="server" Height="100%" ImageSet="Inbox" 
            Width="100%">
            <ParentNodeStyle Font-Bold="False" />
            <HoverNodeStyle Font-Underline="True" />
            <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" 
                VerticalPadding="0px" />
            <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" 
                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
        </asp:TreeView>
</ContentTemplate> 
</asp:UpdatePanel> 
    </div>    
 
    </td>
<td style="padding-top : 5px " valign="top" align="left">
<asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional"  >
   <ContentTemplate> 
       <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td width="60%" style="text-align: left; ">
               <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="100%" Font-Size="Small" ></asp:Label>
                    &nbsp;&nbsp;</td>
                    <td width="40%" align ="right" >
                    <asp:TextBox ID="txtDocid" runat ="server" Width ="120px" CssClass="txtBox" Font-Size ="X-Small" ></asp:TextBox>
                    <asp:TextBoxWatermarkExtender ID="tbWE" runat ="server" TargetControlID ="txtDocid" WatermarkText ="Enter Docid or File Name" WatermarkCssClass ="water" ></asp:TextBoxWatermarkExtender>
                    &nbsp;
                    <asp:ImageButton ID="btnSearch" ImageUrl="~/images/search.png"  runat="server" Height="16px" Width="16px"  OnClick ="SearchbyDocid"  ToolTip="Search By Docid"/></td></tr>
                    <tr>
                    <td colspan ="2">
         <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>
      </td>
    </tr>
    <tr>
     <td colspan ="2" style="text-align: left;padding:0px;" valign="top">
              <asp:GridView ID="gvData" runat="server" CellPadding="4" DataKeyNames="TID" Width="100%" AllowSorting="True" OnPageIndexChanging="gridView_PageIndexChanging" OnSorting="gridView_Sorting" 
             PageSize="20" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" AllowPaging="True" AutoGenerateColumns="false">
                    <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                    <RowStyle BackColor="White" ForeColor="#003399" />
                    <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                    <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="center"  />
                    <HeaderStyle Font-Bold="True" ForeColor="#CCCCFF" CssClass="GridHeader" BackColor="#003399" />
                    <Columns>
                       <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                               <asp:ImageButton ID="btnView" ImageUrl="~/images/view.png"  runat="server" Height="16px" Width="16px" OnClick="ShowHit"  ToolTip="View / Download File"/>
                                 &nbsp;
                                 <a class="detail" style=" text-decoration:none;" href="#"  onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "TID")%>')" >
                                <asp:ImageButton ID="BTNDETAIL" runat="server"  ImageUrl="~/images/show.png" Height="16px" Width="16px" ToolTip ="Take Action" AlternateText="Action" />
                                 &nbsp;
                              <%--  <asp:ImageButton ID="BTNAPR" runat="server"    ImageUrl="~/images/av.png" Height="16px" Width="16px" OnClick="ShowApprove" ToolTip ="Approve Document" AlternateText="Approve Document" />
                                 &nbsp;
                                 <asp:ImageButton ID="BTNREJ"   runat="server" ImageUrl="~/images/closered.png" Height="16px" Width="16px" OnClick="ShowReject" ToolTip ="Reconsider Document" AlternateText="Reconsider Document" />--%>
                            &nbsp;
                            </ItemTemplate>
                            <ItemStyle Width="120px" HorizontalAlign="right"/>
                        </asp:TemplateField>

                        <asp:BoundField DataField="TID" HeaderText="DocID">
                           <HeaderStyle HorizontalAlign="center" />
                        </asp:BoundField>

                        <asp:BoundField DataField="aDate" HeaderText="Creation Date">
                            <HeaderStyle HorizontalAlign="center" />
                        </asp:BoundField>
                     
                        <asp:BoundField DataField="Username" HeaderText="Created By">
                            <HeaderStyle HorizontalAlign="center" />
                        </asp:BoundField>


                        <asp:BoundField DataField="documenttype" HeaderText="Document type">
                            <HeaderStyle HorizontalAlign="center" />
                        </asp:BoundField>
                    </Columns>
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>
     </td> 
     </tr>
         </table>
                </ContentTemplate> 
                </asp:UpdatePanel> 

</td>    
</tr>
</table> 
</div>




  <asp:Button id="btnShowPopupEdit" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit" 
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupEdit" runat="server" Width="950px" Height ="500px" ScrollBars ="Auto"  style="display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:930px"><h3>Document Detail</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseEdit" 
        ImageUrl="images/close.png" runat="server" ToolTip="Close"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
 <ContentTemplate>

<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td colspan ="2">
       <asp:Label ID="lblDetail" runat="server" Text="Folder Name"></asp:Label>
</td>
</tr>

<tr>
<td valign ="top" width="70%" >
<asp:GridView ID="gvMovDetail" runat="server" AutoGenerateColumns="False" 
             CellPadding="2"  Caption="Document Movement" DataKeyNames="tid" Width="100%" 
                             
             PageSize="20" ForeColor="Black" GridLines="None" 
              BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px">
                    <FooterStyle BackColor="Tan" />
                    <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                    <PagerStyle ForeColor="DarkSlateBlue" HorizontalAlign="left" 
                        BackColor="PaleGoldenrod"  />
                    <HeaderStyle Font-Bold="True" 
                        CssClass="GridHeader" BackColor="Tan" />
                    <AlternatingRowStyle BackColor="PaleGoldenrod" />
                    <Columns>
   <asp:TemplateField HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="20px" />
</asp:TemplateField>

  <asp:BoundField DataField="UserName" HeaderText="UserName">
                           <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>


  <asp:BoundField DataField="fdate" HeaderText="In Date">
                           <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>

  <asp:BoundField DataField="tdate" HeaderText="Out Date">
                           <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>

  <asp:BoundField DataField="ptat" HeaderText="SLA">
                           <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>

  <asp:BoundField DataField="atat" HeaderText="A. SLA">
                           <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
  
  <asp:BoundField DataField="remarks" HeaderText="Remarks">
                           <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                      </Columns>
                    <SortedAscendingCellStyle BackColor="#FAFAE7" />
                    <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                    <SortedDescendingCellStyle BackColor="#E1DB9C" />
                    <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>
</td>

<td valign="top" width="30%" >
        <asp:GridView ID="gvFutureMov" runat="server" AutoGenerateColumns="False" 
             CellPadding="2" Caption="Future Movement" DataKeyNames="tid" Width="100%" 
                             
             PageSize="20" BackColor="LightGoldenrodYellow" BorderColor="Tan" 
                        BorderWidth="1px" ForeColor="Black" GridLines="None">
                    <FooterStyle BackColor="Tan" />
                    <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                    <PagerStyle ForeColor="DarkSlateBlue" HorizontalAlign="Center" 
                        BackColor="PaleGoldenrod"  />
                    <HeaderStyle Font-Bold="True" 
                        CssClass="GridHeader" HorizontalAlign ="Left" BackColor="Tan" />
                    <AlternatingRowStyle BackColor="PaleGoldenrod" />
                    <Columns>
                        <asp:TemplateField HeaderText="S.No">
                            <ItemTemplate>
                                <%# CType(Container, GridViewRow).RowIndex + 1%>
                            </ItemTemplate>
                            <ItemStyle Width="20px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="username" HeaderText="User Name">
                        <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SLA" HeaderText="SLA">
                        <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                    <SortedAscendingCellStyle BackColor="#FAFAE7" />
                    <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                    <SortedDescendingCellStyle BackColor="#E1DB9C" />
                    <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>

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






<asp:Button id="btnShowPopupApprove" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="btnApprove_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupApprove" PopupControlID="pnlPopupApprove" 
                CancelControlID="btnCloseApprove" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupApprove" runat="server" Width="600px" Height ="400px" ScrollBars ="Auto"  style="Display:none " BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td><h3> <asp:Label ID="lblHeaderPopUp" runat="server" Font-Bold="True">Document Approval</asp:Label></h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseApprove" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2" style="text-align:left">
<asp:UpdatePanel ID="updatePanelApprove" runat="server" UpdateMode="Conditional">
 <ContentTemplate>
<asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
 <asp:Panel ID="pnlFields" Width="100%" runat="server">
 </asp:Panel>
             <div style="width:100%;text-align:right">
                    <asp:Button ID="btnApprove" runat="server" Text="Approve" 
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

<asp:Button id="btnShowPopupReject" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="btnReject_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupReject" PopupControlID="pnlPopupReject" 
                CancelControlID="btnCloseReject" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupReject" runat="server" Width="600px" Height ="400px" ScrollBars ="Auto"  style="Display:none" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td><h3>Reject Document</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseReject" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2" style="text-align:left">
<asp:UpdatePanel ID="updatePanelReject" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 

<asp:Label ID="lblTabRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
 <asp:Panel ID="pnlFieldsRej" Width="100%" runat="server">
 </asp:Panel>
  
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnReject" runat="server" CssClass="btnNew" Font-Bold="True" 
                        Font-Size="X-Small" Text="Reject" 
                        Width="100px" />
           </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>
</asp:Content>

