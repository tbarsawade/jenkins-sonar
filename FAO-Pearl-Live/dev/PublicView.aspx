<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile ="~/PublicMaster.master" CodeFile="PublicView.aspx.vb" Inherits="PublicView" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
.gradientBoxesWithOuterShadows { 
height: 100%;
width: 96%; 
padding: 5px;
background-color: white; 

/* outer shadows  (note the rgba is red, green, blue, alpha) */
-webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4); 
-moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);

/* rounded corners */
-webkit-border-radius: 12px;
-moz-border-radius: 7px; 
border-radius: 7px;

/* gradients */
background: -webkit-gradient(linear, left top, left bottom, 
color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5)); 
background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%); 
}

</style>
   <div class="gradientBoxesWithOuterShadows">
   <asp:datalist ID="repeater1" runat ="server" RepeatColumns="1" RepeatDirection="Horizontal"  >
   <ItemTemplate >
   <div  style="text-align:justify; vertical-align:top; height: 100%;width: 96%; padding: 5px;background-color: white; webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4);-moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5); webkit-border-radius: 12px; moz-border-radius: 7px;border-radius: 7px;" >
   <asp:Label ID="lbl" runat ="server" Text='<%#Eval("layoutdata")%>' ></asp:Label>
   </div>
   </ItemTemplate>
   </asp:datalist>
 </div>
 </asp:Content>
