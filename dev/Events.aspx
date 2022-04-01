<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="Events.aspx.vb" Inherits="Events" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
<td style="width:100%;" valign="top" align="left">
	<div id="main" style="min-height:400px">
<h1><asp:Label ID="lblCaption" runat="server"></asp:Label></h1>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
<div class="form" style="text-align:left"> 

Your field will appear here

</div>
</ContentTemplate>
</asp:UpdatePanel>
</div>	
</td>    
</tr>
</table> 
</asp:Content>


