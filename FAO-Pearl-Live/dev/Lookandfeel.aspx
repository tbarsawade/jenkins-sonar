<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="Lookandfeel.aspx.vb" Inherits="companylogo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table>
<tr><td style="width:100%" colspan="2" align="center" ><br /><h2>Company Logo</h2><br /></td></tr>
<tr>
   <td style="text-align:center" colspan="2" >     <asp:Label ID="lblImage" runat="server" ForeColor="Red" Text=""></asp:Label></td>
</tr>
<tr>
   <td style="text-align:right"> <label> *Upload Logo [?] : </label></td>
   <td style=""> 
       <asp:FileUpload ID="flU" runat="server" CssClass="txtBox" Width="300px" /> </td> 
   </tr>
 <tr>
 <td> 
     <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>  </td>
        <td align="left">

            <asp:Button ID="btnlogo" runat="server" CssClass="btnNew" 
                Text="Change Logo" />
        
            <br />
        </td>
    </tr>

<tr><td style="width:100%" colspan="2" align="center" ><br /><h2>Company Header</h2><br /></td></tr>
<tr>
   <td style="text-align:center" colspan="2" >     <asp:Label ID="lblHdr" runat="server" Text=""></asp:Label></td>
</tr>
<tr>
   <td style="text-align:right"> <label> *Upload Header [?] : </label></td>
   <td style=""> 
       <asp:FileUpload ID="updHeader" runat="server" CssClass="txtBox" Width="300px" /> </td> 
   </tr>
 <tr>
 <td> 
     <asp:Label ID="lblheaderMsg" runat="server" ForeColor="Red" Text=""></asp:Label>  </td>
        <td align="left">

            <asp:Button ID="btnHdr" runat="server" CssClass="btnNew" 
                Text="Change Header" />
        
            <br />
        </td>
    </tr>

<tr><td style="width:100%" colspan="2" align="center" ><br /><h2>Company Header Strip</h2><br /></td></tr>
<tr>
   <td style="text-align:center" colspan="2" >     <asp:Label ID="lblHdrStrp" runat="server" Text=""></asp:Label></td>
</tr>
<tr>
   <td style="text-align:right"> <label> *Upload Header Strip [?] : </label></td>
   <td style=""> 
       <asp:FileUpload ID="updHstrip" runat="server" CssClass="txtBox" Width="300px" /> </td> 
   </tr>
 <tr>
 <td> 
     <asp:Label ID="lblstrp" runat="server" ForeColor="Red" Text=""></asp:Label>  </td>
        <td align="left">

            <asp:Button ID="btnHdrStrp" runat="server" CssClass="btnNew" 
                Text="Change Header Strip" />
        
            <br />
        </td>
    </tr>
    </table>
</asp:Content>

