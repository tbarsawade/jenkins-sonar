<%@ page language="VB" autoeventwireup="false" inherits="GPSDATA, App_Web_x1sxprmw" viewStateEncryptionMode="Always" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .Inputform {}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
   </asp:ScriptManager>
    <div>

    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
          <tr style="color: #000000">
            <td style="text-align: left; ">
                         <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="red" 
                    Width="97%" Font-Size="Small" ><h4>GPS DATA</h4></asp:Label>
                   </td></tr>
                    <tr><td style="text-align: center; ">
  <asp:Label ID="lblRecord" runat="server"  ForeColor="red" 
               Font-Size="Small" ></asp:Label>                   
    </td>
        </tr>

    <tr style="color: #000000">
        <td style="text-align:left;width:100%;border:3px double green ">
       <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%" 
                border="0px">
         <tr> 
            <td colspan="3"> 
                 <asp:Label ID="lblImieno" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Select IMIENO" >
                 </asp:Label>
                 <asp:DropDownList ID="ddlImieno" runat="server" CssClass="Inputform" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Width="204px"  AutoPostBack="true">
             </asp:DropDownList>
         &nbsp;Record Count :
                 <asp:TextBox ID="txtCount" runat="server" Text ="50" Width="47px"></asp:TextBox>
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
       <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="true" 
             CellPadding="2" DataKeyNames="TID" 
                    CssClass="GridView" 
               AllowSorting="True">  
                     <FooterStyle CssClass="FooterStyle"/>
                    <RowStyle  CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EditRowStyle" />
                    <SelectedRowStyle  CssClass="SelectedRowStyle" />
                    <PagerStyle  CssClass="PagerStyle" />
                    <HeaderStyle  CssClass=" HeaderStyle" />
                    <AlternatingRowStyle CssClass="AlternatingRowStyle"/>
                    <Columns>

  <asp:TemplateField HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
      <ItemStyle Width="50px" HorizontalAlign="Center"  />
</asp:TemplateField>


                      <%--  <asp:BoundField DataField="locationName" HeaderText="Location Name">
                           <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        

                        <asp:BoundField DataField="ZoneName"  HeaderText="Time Zone" />

                        <asp:BoundField DataField="timeFormat" HeaderText="Time Format">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Center"  />
                        </asp:BoundField>
                                             
                          <asp:TemplateField HeaderText="Action" >
                            <ItemTemplate>
                                                                

                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip="Edit record" OnClick="EditHit" AlternateText="Edit" />
                                 
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" ToolTip="Delete record" OnClick="DeleteHit" AlternateText="Delete"/>
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center"/>
                        </asp:TemplateField>--%>
                      </Columns>
                </asp:GridView>

    </td> 
     </tr>
         </table>
    
    </div>
    </form>
</body>
</html>
