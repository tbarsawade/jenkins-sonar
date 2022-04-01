<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PdfDownloader.aspx.vb" Inherits="PdfDownloader" MasterPageFile="~/PublicMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="width:100%; height:600px;text-align:center">
        <table style="width:1000px; height:600px;text-align:center; background-color:burlywood">

            <tr style="width:1000px">

                <td>
                    <asp:Label runat="server" ID="lblDocid" Text="Please Enter Document ID: "></asp:Label> 
                    
                     <asp:TextBox runat="server" ID="txtDocid" Width="180px" Height="20px" ></asp:TextBox>
              

                    <asp:Button runat="server" ID="btnSave" Text="Download" Width="80px" Height="25px" />&nbsp;&nbsp;
                    <asp:Button runat="server" ID="btnSendEmail" Text="Send Mail" Width="80px" Height="25px" />
                </td>
            </tr>

        </table>
        </div>
      
</asp:Content>


