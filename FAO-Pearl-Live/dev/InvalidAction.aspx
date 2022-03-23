<%@ Page Title="" Language="VB" MasterPageFile="~/USRfullscreenbpm.master" AutoEventWireup="false" CodeFile="InvalidAction.aspx.vb" Inherits="InvalidAction" %>

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
            Page Not Authorized
        </div>
    <div class="row mg">
        <div class="col-md-12 col-sm-12">

            <div id="wrap"><!-- wrap starts here -->
	<table width="100%" cellspacing = "0px" cellpadding="0px" border="0px">
    <tr>
    <td style="width:100%;height:17px" colspan="3" >
    <table width="100%" cellspacing = "0px" cellpadding="0px" border="0px">
    <tr>
    <td style="width:70%;">
        <asp:Label ID="lblLogo" runat="server"></asp:Label>
    </td>
    <td style="width:30%;text-align:left" valign="top">
        &nbsp;
    </td>
   </tr>
    </table>
    </td>
  </tr>

<tr>
 
<td style="width:100%;border:3px double green;padding:5px;min-height:220px;" valign="top">
<div class="form">

    <h1><asp:Label ID="lblMsg" runat="server"></asp:Label></h1><br />
    
    <br />

<h5> 
    <%--<asp:LinkButton ID="btndefaultpage" runat="server">Click Here to go back to Home Page </asp:LinkButton>--%>

</h5>
</div>
</td>    
    </tr>
</table> 	
	
</div>

    </div>
    </div>
    </div>

   
<!-- wrap ends here -->	
<br />
<!-- footer starts here -->	
	<div id="footer">
		<div id="footer-content">
<br />
<%--<a href="http://www.myndsol.com">Copyright &copy; 2021, Mynd Integrated Solutions  Pvt. Ltd. All rights reserved.</a> --%>
		</div>	
	</div>
<!-- footer ends here -->	
</asp:Content>
