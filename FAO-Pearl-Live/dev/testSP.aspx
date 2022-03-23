<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" CodeFile="testSP.aspx.vb" Inherits="testSP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Button ID="Button1" runat="server"  Visible="false" Text="Button" />

    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    <asp:Label ID="lblEnc" runat="server" Visible="false" Text="String"></asp:Label><asp:TextBox ID="txtEnc" Visible="false" runat="server"></asp:TextBox>
    <asp:Label ID="lblDec" runat="server" Visible="false" Text="Result"></asp:Label><asp:TextBox ID="TxtREs" Visible="false" runat="server"></asp:TextBox><asp:Button ID="btnEnc" runat="server" Text="Encrypt"  Visible="false"/>
    <asp:Button ID="btnDec" runat="server" Text="Decrypt" Visible="false" />
    <br />
    <asp:Label ID="Label2" runat="server" Text="Any Value"></asp:Label><asp:TextBox  Visible="false" ID="txtVal" runat="server"></asp:TextBox>
    <asp:Button ID="btnCheck" runat="server" Visible="false" Text="check" />
    <br />
    <br />
    <asp:Button ID="btnAutoInvoice" runat="server" Text="Auto Invoice" Style="height: 26px" Visible="false" />
    <br />
     
        <table>

            <tr>
                <td></td>
                <td></td>
                <asp:Button ID="btnAutoInvManual" runat="server" Text="Auto Invoice Manual Generate Invoice" style="margin-left: 323px; margin-top: 34px;" Width="250px"  />
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td></td>
                <td></td>
                <asp:Button ID="btn_Sales" runat="server" Text="Sales Form AutoGenerate Rental Invoice" style="margin-left: 323px; margin-top: 34px;" Width="250px"  />
                <td></td>
                <td></td>
            </tr><tr>
                <td></td>
                <td></td>
               <asp:Label ID="lblres" ForeColor="White" runat="server" Text="" style="color:white;margin-left: 323px; margin-top: 34px;" Width="250px" ></asp:Label>
                <td></td>
                <td></td> 
                 </tr>
        </table>

     
    <br />
    
</asp:Content>

