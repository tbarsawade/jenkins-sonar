<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ftpSetting, App_Web_jkdm3bq3" viewStateEncryptionMode="Always" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
<td style="width:100%;" valign="top" align="left">
	<div id="main">
<h1>Update File Server Settings</h1>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
<div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">

<tr><td style="width:100%" colspan="2"><br /><h2>Update File Server</h2><br /></td></tr>

<tr>
   <td style="text-align:right"> <label> *Server IP Address [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtIPAddress" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Server User ID [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtServerUserID" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Server Password [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtServerPWD" runat="server" CssClass="txtBox" Width="412px"></asp:TextBox></td>
</tr>






 <tr>
 <td> 
     <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>  </td>
        <td align="left">

            <asp:Button ID="btnlogin" runat="server" CssClass="btnNew" 
                Text=" Update Settings " />
        
            <br />
        </td>
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

