<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" validaterequest="false" autoeventwireup="false" inherits="SU_LoginMgmt, App_Web_gn32bfei" viewStateEncryptionMode="Always" %>
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
            SU Login Access Management
        </div>
    <div class="row mg">
        <div class="col-md-12 col-sm-12">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
   <ContentTemplate> 

       <div class="col-md-12 col-sm-12">             
           <asp:Label ID="lblRecord" runat="server"  ForeColor="red" Font-Size="Small" ></asp:Label>                             
      </div>

       <div class="col-md-12 col-sm-12">   
           
           <div class="col-md-6 col-sm-6">                                       
               <br /><br /><br />
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Size="XX-Large"  Text="Current Status of Superuser is - ">
                 </asp:Label> &nbsp;
               <asp:Label ID="LblStatus" runat="server" Font-Bold="True"  Font-Size="XX-Large" ForeColor="Navy"  Text="Status">
                 </asp:Label>
            </div>
                  
         <div class="col-md-6 col-sm-6">           
             <br /><br /><br />
              <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/lock.png" Height="36px" Width="36px" OnClick="DeleteHit" AlternateText="Delete"/>
         </div> 
            
               
         <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
         </div>
     </ProgressTemplate>
      </asp:UpdateProgress>
                </div>

     </ContentTemplate> 
</asp:UpdatePanel> 
            </div>
    </div>


<asp:Button id="btnShowPopupDelete" runat="server" style="Display:none" />
<asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete" 
   CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground" DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" style="Display:none " BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Confirmation</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server"/></td>
</tr>

<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<h2> <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       <div style="width:100%;text-align:right" >
                <asp:Button ID="btnActDelete" runat="server" Text="Yes"  Width="90px" 
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

