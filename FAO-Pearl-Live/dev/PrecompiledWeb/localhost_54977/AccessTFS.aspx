<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="AccessTFS, App_Web_iqn0gzeb" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   
        
        <div style="border:  1px solid red; width:100%"><br /><br /><br />
        <table>
            <tr style="text-align:center">
                <td  style="width:350px">

                </td>
                <td>
                       &nbsp;<asp:Label ID="lblEmpCode" runat="server" Text="Enter Employee Code :"></asp:Label>
                </td>
                <td>
                    &nbsp;
                    <asp:TextBox ID="txtEmpcode" Width="180px" runat="server" >
                    </asp:TextBox>

                </td>
                 <td> &nbsp;
                   <asp:Button ID="btnSerach" runat="server" Text="Search" />

                </td>

            </tr>

        </table>
 <br /><br /><br />
        <table>
            <tr>
                <td>

                </td>
                <td>
                     <asp:GridView ID="grdData" runat="server" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" Height="247px" style="margin-left: 0px" Width="589px" >
            <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
            <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
            <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
            <RowStyle BackColor="White" ForeColor="#003399" />
            <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
            <SortedAscendingCellStyle BackColor="#EDF6F6" />
            <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
            <SortedDescendingCellStyle BackColor="#D6DFDF" />
            <SortedDescendingHeaderStyle BackColor="#002876" />
        </asp:GridView>
                </td>
            </tr>

        </table>
       <br /><br />
    </div>
    
</asp:Content>

