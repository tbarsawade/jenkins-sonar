﻿<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="Mailtest.aspx.vb" Inherits="Mailtest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    

     <table width="100%">
<tr>
<td align="center">

<asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Always" >
   <Triggers>
       <asp:PostBackTrigger ControlID ="btnshowGrid" />

   </Triggers> 
   <ContentTemplate> 

        <table width="100%">

<tr>
<td >




    </td>
    </tr>
    </table>


    <asp:Button ID="btnShowGrid" runat="server" Width="90px" Text="Generate" />
   
<asp:Label ID="lblMsg" runat="server"  Text="" Font-Size="x-Small" ForeColor="red" >   

</asp:Label>



       </ContentTemplate> 
    </asp:UpdatePanel>
    </td>
    </tr>

    </table>
    

</asp:Content>

