<%@ page title="" language="VB" masterpagefile="~/usr.master" autoeventwireup="false" validaterequest="false" inherits="MyEpayWStest, App_Web_ws3kahym" viewStateEncryptionMode="Always" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <table width="100%">
<tr>
<td align="center">

<asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
   <Triggers>
   <%--  <asp:PostBackTrigger ControlID ="btnExportInput" />
     <asp:PostBackTrigger ControlID ="btnExportOutput" />--%>
     <asp:PostBackTrigger ControlID ="btnshowGrid" />

   </Triggers> 
   <ContentTemplate> 
    <table width="100%">

<tr>
<td colspan="2">
<asp:Label ID="lblMsg" runat="server"  Text="" Font-Size="x-Small" ForeColor="red" > 
    </asp:Label>
</td>
</tr>
   <tr>
     <td colspan="2" valign="middle">
       <asp:Label ID="Label2" runat="server" BorderStyle="None" CssClass="Inputform" ForeColor="Navy" Font-Bold="True">Select Type :</asp:Label> &nbsp;
      &nbsp;&nbsp;                            
         <asp:TextBox ID="txtUrl" Width="600px" runat="server"> </asp:TextBox>
        <asp:Button ID="btnShowGrid" runat="server" Width="60px" Text="Generate" />   
        &nbsp;
         <asp:Button ID="btnClear" runat="server" Text="Clear" Width="60px" />
     </td>
   </tr>

  <tr>
  <td colspan="2">
       <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="50">
          <ProgressTemplate>         
             <asp:Label ID="lbwait" runat ="server" Text="Loading Please Wait...." Font-Size ="Large" Font-Names ="Verdana"></asp:Label>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/prg1.gif" alt="" />
          </ProgressTemplate>
       </asp:UpdateProgress>
  </td>
  </tr>

  <tr>
    <td width="50%" style="text-align:center;">
        <asp:Label ID="Label6" runat="server" BorderStyle="None" CssClass="Inputform" ForeColor="Navy" Font-Bold="True">Input Sring</asp:Label>
    </td>
    <td width="50%" style="text-align:center; vertical-align:middle;">        
      <asp:Label ID="Label1" runat="server" BorderStyle="None" CssClass="Inputform" ForeColor="Navy" Font-Bold="True">Result</asp:Label>
    
    </td>
</tr> 

  <tr>
  <td colspan="2">
      
       <asp:Panel  ID="Panel1" runat="server" ForeColor="#160C16"  BorderColor="#00CC00" Width="100%" Wrap="False" Visible ="TRUE" >
          <table width="100%">
           <tr>
            <td width="50%">
             <asp:Panel  ID="Panel2" runat="server" ForeColor="#160C16"  BorderColor="#00CC00" Width="100%" Wrap="False" Visible ="TRUE" >        
              <asp:TextBox ID="txtInputXml" runat="server"  TextMode="MultiLine" Rows="10" Wrap="true" Width="100%" Height="400px"></asp:TextBox>   
              </asp:Panel> 
            </td>
            <td width="50%">
             <asp:Panel  ID="Panel3" runat="server" ForeColor="#160C16"  BorderColor="#00CC00" Width="100%" Wrap="False" Visible ="TRUE" >        
              <asp:TextBox ID="txtOutputXml" runat="server" TextMode="MultiLine"   Rows="10" Wrap="true" Width="100%" Height="400px"></asp:TextBox>   
              </asp:Panel> 
            </td>
           </tr>
          </table>
           
           
       </asp:Panel> 
  </td>
  </tr>
  
<tr>
    <td colspan="2" width="100%" style="text-align: center;">
        &nbsp;</td>
</tr> 

</table> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table>

</asp:Content>



