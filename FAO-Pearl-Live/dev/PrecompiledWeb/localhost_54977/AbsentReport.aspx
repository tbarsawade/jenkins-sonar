<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="AbsentReport, App_Web_kxub2lm0" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <style type="text/css">
        .headdiv {
            width:100%;
            padding:30px;
        }
    </style>
    <div style="min-height:450px;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server">
    <div class="headdiv">
        <table>
            <colgroup>
                <col style="width:200px;" />
                <col style="width:250px;"/>
                <tr>
                    <td>
                        <asp:Button ID="Button1" runat="server" CssClass="ststusButton" Text="Absent Report" Visible="True"  />
                    </td>
                    <td>
                        <asp:Button ID="Button2" runat="server" CssClass="ststusButton" Text="Absent consolidated report" Width="230px" />
                    </td>
                     <td>
                        <asp:Button ID="Button3" runat="server" CssClass="ststusButton" Text="Test Absent consolidated report" Width="240px" />
                    </td>
                </tr>
            </colgroup>
        </table>
       </div>

    <div  class="headdiv">
        <div style="width:60%; border:1px solid #e0e0e0; padding:20px;">
            <asp:Label ID="lblMsg" runat="server" Font-Bold="true" Text=""></asp:Label>
        </div>
    </div>
    
        </asp:Panel>
         </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    </div>
</asp:Content>

