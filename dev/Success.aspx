<%@ Page Title="Congratulations User : Account created successfully" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="Success.aspx.vb" Inherits="Success" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">

    <tr>
  
<td style="width:750px;" valign="top" align="center">
<br />
  
<br />
<span style="font"></span>

<h1><img src="images/Congratulation.png" alt="successfull" /> Congratulations <asp:Label ID="lblMsg" runat="server" Text="Label"></asp:Label> ! You have successfully registered on 
    eDMS</h1>
<h3>Please open your inbox and click verification link to activate your Account.</h3>

<br />
<br />

If your have validated your account click <a href="Default.aspx">here</a> to login
             						
</td>    
  
    </tr>
</table> 



</asp:Content>

