<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="SendMail, App_Web_ik1k4di5" viewStateEncryptionMode="Always" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <table cellpadding="0" cellspacing="0" style="text-align: left" width="100%" border="0">
          <tr style="color: #000000">
            <td style="text-align: left; ">
<h4>Welcome Mail Sender</h4>
    </td>
        </tr>
                      <tr style="color: #000000">
            <td style="text-align: left; ">

                <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="Red" 
                    Width="97%" Font-Size="Small" ></asp:Label>

</td> </tr> 
            <tr>
                <td style="width:100%;text-align:left">
                    Enter Employee UserIDs - Separated by Commas (,)

                </td>
                </tr>
            <tr>
                <td style="width:99%;text-align:left">
                    <asp:TextBox ID="txtUserID" TextMode="MultiLine" Width="99%"   runat="server"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td style="width:99%;text-align:left">
          
                    BCC List             </td>
            </tr>
            

            <tr>
                <td style="width:99%;text-align:left">
                    <asp:TextBox ID="txtCCList" Width="99%"   runat="server"></asp:TextBox>
                </td>
            </tr>
            
            <tr>
                <td style="width:99%;text-align:left">
    <asp:Button ID="Button1" runat="server" Text="Send Mail" />
               </td>
            </tr>
</table> 

    
</asp:Content>

