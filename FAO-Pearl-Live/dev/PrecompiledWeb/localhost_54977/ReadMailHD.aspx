<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ReadMailHD, App_Web_13sbvfgr" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <table width="100%">
        <tr>
            <td align="center">

                <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Always">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnshowGrid" />

                    </Triggers>
                    <ContentTemplate>

                        <table width="100%">

                            <tr>
                                <td>


                                    <br />
                                    <br />
                                    <br />
                                    <asp:Panel ID="pnlMail" runat="server" ScrollBars="Auto" Width="90%">
                                          <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="false" Width="100%">
                                        <Columns>
                                            <asp:BoundField HeaderText="MessageId" DataField="MessageId" />
                                            <asp:BoundField HeaderText="From" DataField="From" />
                                            <asp:HyperLinkField HeaderText="Subject" DataNavigateUrlFields="MessageNumber" DataNavigateUrlFormatString="~/ShowMessage.aspx?MessageNumber={0}" DataTextField="Subject" />
                                            <asp:BoundField HeaderText="Date" DataField="DateSent" />
                                            <asp:BoundField HeaderText="Message Content" DataField="MessageContent" />
                                            <asp:BoundField HeaderText="Is Attachment" DataField="IsAttachment" />
                                            <asp:BoundField HeaderText="File Name" DataField="FileName" />


                                        </Columns>
                                    </asp:GridView>
                                    </asp:Panel>
                                  
                                    <br />
                                    <br />

                                </td>
                                <tr>
                                    <td>

                                        <asp:Label runat="server" ID="lblDelID" Text="Type ID Here"></asp:Label>
                                        <asp:TextBox runat="server" ID="txtDelID" Width="100px"> </asp:TextBox>

                                    </td>
                                </tr>
                            </tr>
                        </table>


                        <asp:Button ID="btnShowGrid" runat="server" Width="90px" Text="Show List" />
                        <asp:Button ID="btnDelMail" runat="server" Width="90px" Text="Del Mail" />

                        <asp:Label ID="lblMsg" runat="server" Text="" Font-Size="x-Small" ForeColor="red">   
                            <asp:Button ID="btnWebService" runat="server" Text="Web Service Call" OnClick="btnWebService_Click" />

                        </asp:Label>



                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>

    </table>


</asp:Content>

