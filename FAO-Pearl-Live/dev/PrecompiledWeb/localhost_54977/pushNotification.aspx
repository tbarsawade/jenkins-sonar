<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="pushNotification, App_Web_o3dtvhns" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%--<table>
        <tr>
            <td>
               <asp:Label ID="lblUserRole" runat="server" > Select User Role </asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlUserrole" runat="server" AutoPostBack="true"></asp:DropDownList>
            </td> 
            <td> <asp:CheckBoxList ID="UserName" runat="server" Visible="false" > </asp:CheckBoxList> </td>
        </tr>

    </table>--%>


    <script type="text/javascript" language="javascript">
        function checkAll()
        {
            var chkbox = document.getElementById('<%= chkDocType.ClientID%>');
            var chkboxlist = document.getElementById('<%= chkUserName.ClientID%>');
            var chkboxlistCount = chkboxlist.getElementsByTagName('input');
            if (chkbox.checked == true) {
                for (var i = 0; i < chkboxlistCount.length; i++) {
                    chkboxlistCount[i].checked = true;
                }
            }
            else {
                for (var i = 0; i < chkboxlistCount.length ; i++) {
                    chkboxlistCount[i].checked = false;
                }
            }


        }
   </script>


    <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                            <tr>
                                <td rowspan="2" style="width: 65%;" valign="top">
                                    <asp:Panel ID="pnlFields" Width="100%" runat="server">
                                       <div>
                                        <asp:Label ID="lblUserRole" runat="server" > Select User Role : </asp:Label> 
                                        <asp:DropDownList ID="ddlUserrole" runat="server" AutoPostBack="true"></asp:DropDownList>
                                     </div>
                                        <div style="height:200px;">
                                            <asp:TextBox ID="megBox" runat="server" TextMode="MultiLine" Height="195px" Width="100%"></asp:TextBox>
                                           
                                        </div>
                                    </asp:Panel>
                                </td>
                                <td valign="top">
                                    <div class="form" style="overflow-Y: scroll; width: 95%; height: 200px">
                                        <asp:UpdatePanel ID="updUserName"  UpdateMode="Conditional" runat="server">
                                            <contenttemplate> 
                                        <asp:CheckBox ID="chkDocType" runat="server"  onclick="checkAll()"  Text="User Name" /> 
                                      <asp:CheckBoxList ID="chkUserName" runat="server" CssClass="txtBox" Width="98%" Height="300px">
                                      </asp:CheckBoxList>
                                       </contenttemplate>
                                    </asp:UpdatePanel>
                                    </div>
                                </td>
                            </tr>
                        </table>

    <asp:Button ID="sendMessage" runat="server" Text="Send Message" />
     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter text" ControlToValidate="megBox"></asp:RequiredFieldValidator>
    <asp:Label ID="lblError" runat="server" Text="An error occur at server" Visible="false"></asp:Label>
</asp:Content>

