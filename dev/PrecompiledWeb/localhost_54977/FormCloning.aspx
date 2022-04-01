<%@ page title="AccountCopy" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="FormCloning, App_Web_4ysr50e5" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
        <tr>
            <td style="width: 100%;" valign="top" align="left">
                <div id="main">
                    <h1>Account Creation Through Cloning</h1>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <%--   <Triggers>
                            <asp:PostBackTrigger ControlID="btnSave" />
                        </Triggers>--%>
                        <ContentTemplate>
                            <div class="form">
                                <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            
                                            <h2>Enter Form Information</h2>
                                            
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100%" colspan="2">
                                            <asp:Label ID="lblMsg" runat="server" ForeColor="red" Font-Bold="True" Font-Size="Small" Text="" Visible="True"></asp:Label>
                                            <asp:Label ID="lblmsgg" runat="server" Font-Bold="True" ForeColor="#003399"></asp:Label>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Select Form Type :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:DropDownList ID="ddlFormType" runat="server" CssClass="txtBox" Width="208px" AutoPostBack="True">
                                                <asp:ListItem>Select</asp:ListItem>
                                                <asp:ListItem>Documents</asp:ListItem>
                                                <asp:ListItem>Masters</asp:ListItem>
                                                <asp:ListItem>Action Forms</asp:ListItem>
                                                <asp:ListItem>Detail Forms</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *Select Form to be Cloned :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:DropDownList ID="ddlOldForm" runat="server" CssClass="txtBox" Width="208px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                                *New Form Name [?] :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtNewForm" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    
                                    
                                   
                                    <tr>
                                        <td style="text-align: right">
                                            <label>
                                            * Suffix for Child/Action Forms :
                                            </label>
                                        </td>
                                        <td style="">
                                            <asp:TextBox ID="txtNewFormSuffix" runat="server" CssClass="txtBox" Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    
                                    
                                   
                                    <tr>
                                        <td style="text-align: right; height: 23px;"></td>
                                        <td style="height: 23px">&nbsp;&nbsp;
                                        </td>
                                    </tr>

                                    </tr>
                                    <tr>
                                                                              
                                        <td>&nbsp;</td>
                                        <td align="left">
                                            <asp:Button ID="btnSave" runat="server" CssClass="btnNew" Text="Start Cloning" />
                                            <%--<asp:Button ID="btnfield" runat="server" CssClass="btnNew" Text="Upadte Field" />--%><%-- <asp:Button ID="btnvalidation" runat="server" CssClass="btnNew" Text="Upadte Validation Form" />--%>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td align="left">
                                            &nbsp;</td>
                                    </tr>
                                </table>
                            </div>
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
    <div>
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="100">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif"   />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </div>
</asp:Content>
