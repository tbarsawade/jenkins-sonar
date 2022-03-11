<%@ Page Title="User Action Log Report" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="UserLog.aspx.vb" Inherits="UserLog" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
   <ContentTemplate> 
    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ><h4>User Action Log Report</h4></asp:Label>
    </td>
        </tr>

    <tr style="color: #000000">
        <td style="text-align:left;width:100%;border:3px double green ">
       <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%" 
                border="0px">
         <tr> 
            <td style="width: 90px;"> 
                 <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" 
                     Font-Size="Small" ForeColor="Navy" Text="From Date" Width="99%"></asp:Label>
         </td>

         <td style="width: 170px;">

             <asp:TextBox ID="txtFDate" runat="server" Width="150px"></asp:TextBox>
    <asp:CalendarExtender ID="txtFDate_CalendarExtender" runat="server" 
        Enabled="True" Format="dd-MMM-yy" TargetControlID="txtFDate">
    </asp:CalendarExtender>

</td>

         <td style="width: 100px;">
         <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" 
                 Font-Size="Small" ForeColor="Navy" Text="To Date" Width="99%"></asp:Label>
         </td>

         <td style="width: 200px;">

             <asp:TextBox ID="txtLDate" runat="server" Width="150px"></asp:TextBox>
    &nbsp;&nbsp;
    <asp:CalendarExtender ID="txtLDate_CalendarExtender" runat="server" 
        Enabled="True" Format="dd-MMM-yy" TargetControlID="txtLDate">
    </asp:CalendarExtender>



         </td>

         <td style="text-align: right;width:25px">
             <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" 
                 ImageUrl="~/Images/search.png" />
          </td>

         <td style="text-align: right;">

             </td>
         </tr>
           </table>
         <asp:UpdateProgress ID="UpdateProgress1" runat="server">
         <ProgressTemplate>
         <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%">
              <asp:Image ID="Image1" runat="server" height="25px" ImageUrl="~/Images/prg.gif"/> please wait...
</div>
         </ProgressTemplate>
      </asp:UpdateProgress>

        </td>
    </tr>

     <tr style="color: #000000">
     <td style="text-align: left;" valign="top">
       <asp:GridView ID="gvData" runat="server" 
             CellPadding="2"
                    ForeColor="#333333" Width="100%" 
               AllowSorting="True" AllowPaging="True">
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#EFF3FB" />
                    <EditRowStyle BackColor="#2461BF" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>

    </td> 
     </tr>
         </table>
                         </ContentTemplate> 
                </asp:UpdatePanel> 
</asp:Content>


