<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" EnableEventValidation="true" CodeFile="ReportEditor.aspx.vb" Inherits="ReportEditor" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src="tinymce/tinymce.min.js" type="text/javascript"></script>

<script type="text/javascript">
    tinymce.init({
        selector: "textarea",
        plugins: [
        "advlist autolink lists link  charmap print preview anchor",
        "searchreplace visualblocks code fullscreen",
        "insertdatetime  table contextmenu paste"
    ],
        toolbar: "undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent ",

        //         max_height: 200,
        //        min_height: 160,
        height: 300,
        width: 800



    });

</script>




<style type="text/css">
     .auto-style1 {
            width: 59px;
        }
.accordionContent {
background-color: #D3DEEF;
border-color: -moz-use-text-color #2F4F4F #2F4F4F;
border-right: 1px dashed #2F4F4F;
border-style: none dashed dashed;
border-width: medium 1px 1px;
padding: 10px 5px 5px;
width:20%;

}
.accordionHeaderSelected {
background-color: #5078B3;
border: 1px solid #2F4F4F;
color: white;
cursor: pointer;
font-family: Arial,Sans-Serif;
font-size: 12px;
font-weight: bold;
margin-top: 5px;
padding: 5px;
width:20%;
}
.accordionHeader {
background-color: #2E4D7B;
border: 1px solid #2F4F4F;
color: white;
cursor: pointer;
font-family: Arial,Sans-Serif;
font-size: 12px;
font-weight: bold;
margin-top: 5px;
padding: 5px;
width:20%;
margin-left:0px;
}
.href
{
color:White; 
font-weight:bold;
text-decoration:none;
}
</style>

  <asp:UpdatePanel ID="updPnlGrid" runat="server">
    <Triggers>
    <asp:PostBackTrigger ControlID="imgbtnPrint" />
    <asp:PostBackTrigger ControlID="imgbtnexl" />
     <asp:PostBackTrigger ControlID="btnAccSave" />
   
   
    </Triggers>
   <ContentTemplate> 
   <asp:Panel ID="pnlAcc" runat="server">
  

<div>
<table>
<tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="red" 
                    Width="100%" Font-Size="Small" ><h4>Report Editor</h4></asp:Label>
                   </td></tr>
                   <tr><td style="text-align: center; ">
  <asp:Label ID="lblmsg" runat="server"  ForeColor="red" 
               Font-Size="Small" ></asp:Label>                   
    </td>
        </tr>
<tr style="color: #000000" >
 <td style="text-align:left; width:800px; border:3px double lime; height:30px" >


   <asp:Label ID="lblTempName" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Template Name :">
                 </asp:Label>
&nbsp; &nbsp;
<asp:textbox  ID="txtTemp" runat="server"  Width="150px" CssClass="Inputform" Font-Bold="True"  Font-Size="Small"  ></asp:textbox>
 &nbsp; &nbsp;
 <asp:Label ID="lblEntitiy" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Entity Type :" >
                 </asp:Label>
 &nbsp; &nbsp;
<asp:dropdownlist  ID="ddlDocType" runat="server" CssClass="Inputform" 
         Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" 
         Width="150px" AutoPostBack="True">

</asp:dropdownlist>
</td>




<tr>
<td>
<asp:ImageButton ID="imgbtnPrint" runat="server" ImageUrl="~/images/adobe.jpg"/> &nbsp;
<asp:ImageButton ID="imgbtnexl" runat="server" ImageUrl="~/images/excel.png"/>

</td>
</tr>
    <tr>
    
    <td>
   <asp:UpdatePanel ID="upeditor" runat="server" UpdateMode="Conditional" >
   <ContentTemplate>
   <asp:Panel ID="pnlEditor" runat="server" Width="820px" Height="400px" >
    <asp:TextBox ID="txtEditor" runat="server" TextMode="MultiLine" Width="740px" Height="380px"   ></asp:TextBox>
   </asp:Panel>


  <%-- <asp:Panel ID="pnlScrol" runat="server"  ScrollBars="Auto" Height="500px">
    <asp:TextBox ID="txtEditor"  TextMode ="MultiLine"  runat ="server" Width ="800px" Height ="500px" >
    </asp:TextBox>   
    
  <asp:HtmlEditorExtender ID="HEE_body" runat ="server" DisplaySourceTab ="TRUE" TargetControlID ="txtEditor" EnableSanitization ="false" ></asp:HtmlEditorExtender>
       
       </asp:Panel>--%>




   </ContentTemplate>
   </asp:UpdatePanel>
   
    </td>
    
       <td style="width:300px">
       <asp:UpdatePanel ID="uplAc" runat="server" >
       <ContentTemplate>
       <asp:Panel runat="server" ID="pnlAc">
         <asp:Accordion ID="acc1" runat="server"
        SelectedIndex="0" HeaderCssClass="accordionHeader"
        ContentCssClass="accordionContent" AutoSize="None"
        FadeTransitions="true"
    HeaderSelectedCssClass="accordionHeaderSelected"
    
   
    TransitionDuration="250"
    FramesPerSecond="40"
    RequireOpenedPane="false" Width="825px"  >
         
          </asp:Accordion> 
       </asp:Panel>
       </ContentTemplate>
       
       </asp:UpdatePanel>
     
        </td>
    </tr>

</table>
</div>   

               
 
       <asp:DropDownList ID="ddlap2" runat="server" Width="30px" Visible="false">
           <asp:ListItem>userid</asp:ListItem>
           <asp:ListItem>docid</asp:ListItem>
           <asp:ListItem>fdate</asp:ListItem>
           <asp:ListItem>tdate</asp:ListItem>
           <asp:ListItem>aprstatus</asp:ListItem>
           <asp:ListItem>sla</asp:ListItem>
           <asp:ListItem>remarks</asp:ListItem>
       </asp:DropDownList>

     
    
   
   <asp:Panel ID="pnlAccord" runat="server" >

<div style="margin-left:650px"><br />

    <asp:Button ID="btnAccSave" runat="server" Text="Save" Width="100px" CssClass="btnNew" />
    <asp:DropDownList ID="ddlfld" runat="server" Visible="false">
    </asp:DropDownList>
</div>

   
   </asp:Panel>
   

<asp:Panel ID="panel1" runat="server" >
</asp:Panel>
   </asp:Panel>
 </ContentTemplate>
   </asp:UpdatePanel>

</asp:Content>

