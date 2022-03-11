<%@ Page Language="VB" AutoEventWireup="false" CodeFile="IHCLService.aspx.vb" Inherits="IHCLService" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #TextArea1 {
            height: 185px;
            width: 583px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Label ID="Label3" runat="server" Font-Size="Larger" Font-Bold="True" ForeColor="#9900cc" Text="Push record by entering PEARL ID | IHCL Web Service Outward Integration"></asp:Label>

                <br />
        <br />

                <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Please Enter Comma Separated PEARL_IDs(Ex# PER0001,PER002,PER003) :"></asp:Label>
         &nbsp;
        <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="Red" Text=""></asp:Label>
                <br />
        <br />
                <asp:TextBox ID="TextBox2" Font-Size="Large" runat="server" Height="27px" Width="1244px"></asp:TextBox>




        <br />
        <br />




        <asp:Button ID="Button1" runat="server" Height="33px" Text="Vendor Creation" Width="147px" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" Visible="true" Height="33px" Text="Vendor Modification" Width="146px" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button3" runat="server" Visible="true" Height="33px" Text="Vendor Site Extension" Width="146px" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button4" runat="server" Visible="true" Height="33px" Text="Mat Invoice URL" Width="146px" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button5" runat="server" Visible="true" Height="33px" Text="Invoice Service/Non-PO " Width="146px" />
        
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Records Fetched:"></asp:Label>
        <br />
        <br />
        <br />
        <asp:GridView ID="GridView1" Visible="true" runat="server" Height="145px" Width="477px">
        </asp:GridView>
        <br />
        <br />
        <br />



        <asp:Label ID="Label1" runat="server" Text="Respone: " Font-Bold="True"></asp:Label>
        :<br />
        <br />
        <br />
        <asp:TextBox ID="TextBox1" Font-Size="Medium" runat="server" Height="59px" Width="1251px"></asp:TextBox>
        <br />
        &nbsp;</form>
    
</body>
</html>
