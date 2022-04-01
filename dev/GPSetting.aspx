<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="GPSetting.aspx.vb" Inherits="GPSetting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
<td style="width:100%;" valign="top" align="left">
	<div id="main">
        
<h1>GPS Settings</h1>
<asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional"  runat="server">
   <ContentTemplate>
<div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">

<tr><td style="width:100%" colspan="2"><br /><h2>User -
    <label>
    Vehicle</label>Mapping</h2><br /></td></tr>

  <%--<tr>
   <td style="text-align:right"> <label> *Select Mapping Document Type&nbsp; : </label></td>
   <td style=""> 
   <asp:DropDownList ID="APPTYPE" runat="server"  AutoPostBack="true"  width="250px"  OnSelectedIndexChanged="FilldropDown"  CssClass="txtBox">

</asp:DropDownList>
     </td>
</tr>--%>
    <tr>
   <td style="text-align:right"> </td>
   <td style=""> 
<asp:Label ID="savedvehiclemapping" ForeColor="Red" runat="server"></asp:Label>
     </td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Select VehicleDocument Type&nbsp; : </label></td>
   <td style=""> 
   <asp:DropDownList ID="ddlDocumentType" runat="server"  AutoPostBack="true"  width="250px"  OnSelectedIndexChanged="FilldropDown"  CssClass="txtBox">
</asp:DropDownList>
     </td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Select Vehicle Status&nbsp; : </label></td>
   <td style="">   <asp:DropDownList ID="ddstatus" runat="server"  Width="250px"  CssClass="txtBox">
</asp:DropDownList>

       
   </td>
</tr>
<tr>
   <td style="text-align:right"> <label> *Select Vehicle Owner&nbsp; : </label></td>
   <td style="">   <asp:DropDownList ID="ddowner" runat="server"  Width="250px"  CssClass="txtBox">
</asp:DropDownList>

       
   </td>
</tr>
<tr>
   <td style="text-align:right"> <label> *Select Vehicle Type &nbsp; : </label></td>
   <td style="">   <asp:DropDownList ID="ddVehicleType" runat="server"  Width="250px"  CssClass="txtBox">
</asp:DropDownList>

       
   </td>
</tr>
<tr>
   <td style="text-align:right"> <label> *select Rate Card Doc&nbsp; : </label></td>
   <td style="">         
   <asp:DropDownList ID="ddRatecarddoc" runat="server" Width="250px"   CssClass="txtBox">
</asp:DropDownList>

 </td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Select Customer Doc&nbsp; : </label></td>
   <td style="">  
      <asp:DropDownList ID="ddlCustomerDoc" runat="server" Width="250px"   CssClass="txtBox">

</asp:DropDownList>


 </td>
</tr>
<tr>
   <td style="text-align:right"> &nbsp;</td>
   <td style=""> &nbsp;</td>
</tr>

<tr>
   <td style="text-align:right"> </td>
   <td style="">             <asp:Button ID="btnUname" runat="server" CssClass="btnNew" 
                Text="Save" />
 </td>
</tr>
<tr>
   <td style="text-align:right"> &nbsp;</td>
   <td style=""> <asp:Label style="color:red" ID="lblUname" runat="server"></asp:Label></td>
</tr>

</table>
</div>

</ContentTemplate>
</asp:UpdatePanel>

</div>	
</td>    
</tr>
</table> 
</asp:Content>