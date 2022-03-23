<%@ Page Title="Password Policy" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="PasswordPolicy.aspx.vb" Inherits="PasswordPolicy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
    <tr>
<td style="width:100%;" valign="top" align="left">
	<div id="main">
<h1>Update Password Policy</h1>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
<div class="form">
<table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
<tr>
   <td style="text-align:right"> <label> *Minimum Character [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtMinCHar" runat="server" CssClass="txtBox" 
           Width="100px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Maximum CHaracter [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtMaxChar" runat="server" CssClass="txtBox" 
           Width="100px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Password Type [?] : </label></td>
   <td style=""> 
       <asp:DropDownList ID="ddlPasswordType" runat="server" CssClass="txtBox" Width="350">
           <asp:ListItem>ANY CHARACTER</asp:ListItem>
           <asp:ListItem>ALPHA NUMERIC</asp:ListItem>
           <asp:ListItem>ALPHA NUMERIC WITH CAPS LETTER</asp:ListItem>
           <asp:ListItem>ALPHA NUMERIC WITH SPECIAL CHARACTER</asp:ListItem>
       </asp:DropDownList>
    </td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Password Expiry Days [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtPassExpDays" runat="server" CssClass="txtBox" 
           Width="100px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Password Expiry Message Days [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtPassExpMsgDays" runat="server" CssClass="txtBox" Width="100px"></asp:TextBox></td>
</tr>


<tr>
   <td style="text-align:right"> <label> *AutoUnlock Hours [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtAutoUnlockHour" runat="server" CssClass="txtBox" Width="100px"></asp:TextBox></td>
</tr>

<tr>
   <td style="text-align:right"> <label> *Min Password Attemp [?] : </label></td>
   <td style=""> <asp:TextBox ID="txtPasswordAttempt" runat="server" CssClass="txtBox" Width="100px"></asp:TextBox></td>
</tr>

 <tr>
 <td> 
     <asp:Label ID="lblMsg" runat="server" Text="" ForeColor="Red" ></asp:Label>  </td>
        <td align="left">

            <asp:Button ID="btnlogin" runat="server" CssClass="btnNew" 
                Text=" Update Password Policy " />
        
            <br />
        </td>
    </tr>


<tr><td style="text-align:Center"  colspan="4"  ><br /><h2 style="text-align:Center">Change User Account </h2><br /></td></tr>
<tr>
   <td style="text-align:Center" colspan="2"> <asp:RadioButton ID="btnradioemail" runat="server"  Text="EmailId" GroupName="Account"/>
<asp:RadioButton ID="btnradioothers" runat="server"  Text="Others" GroupName="Account"  AutoPostBack="true"/>   
<asp:DropDownList ID="ddlothers" runat="server" ></asp:DropDownList>

   </td>
</tr>

<tr>
   <td style="text-align:Left"><asp:Label ID="lblMessacc" runat="server"  ForeColor="Red"></asp:Label>  </td>

   <td style="">             <asp:Button ID="btnUname" runat="server" CssClass="btnNew" 
                Text=" Change User account" />
 </td>
</tr>

<tr><td style="text-align:Center"  colspan="4"  ><br /><h2 style="text-align:Center">GPS Account Saving</h2><br /></td></tr>
<tr>
   <td style="text-align:Center" colspan="2">
       <asp:CheckBox ID="chkActivate" runat="server" Text="Activate GPS" />
    <%--<asp:RadioButton ID="RadioButton1" runat="server"  Text="EmailId" GroupName="Account"/>
    <asp:RadioButton ID="RadioButton2" runat="server"  Text="Others" GroupName="Account"  AutoPostBack="true"/>   
    <asp:DropDownList ID="DropDownList1" runat="server" ></asp:DropDownList>--%>
       </td>
</tr>

<tr>
   <td style="text-align:Left"><asp:Label ID="lblGpsAct" runat="server"  ForeColor="Red"></asp:Label>  </td>

   <td style="">             <asp:Button ID="btnGpsActivated" runat="server" CssClass="btnNew" 
                Text="Activate GPS " />
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
