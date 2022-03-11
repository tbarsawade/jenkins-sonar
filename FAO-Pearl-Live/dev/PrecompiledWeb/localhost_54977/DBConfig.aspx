<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="DBConfig, App_Web_s1ukpvof" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">


        function CheckAll() {
            var intIndex = 0;
            var list = document.getElementById("<%=chkroles.ClientID%>");
            var rowCount = list.getElementsByTagName("input");
            for (i = 0; i < rowCount.length ; i++) {
                if (document.getElementById("<%=cbAll.ClientID%>").checked == true) {
                    if (rowCount[i].type == "checkbox") {
                        rowCount[i].checked = true;

                    }
                }
                else {
                    if (rowCount[i].type == "checkbox") {
                        rowCount[i].checked = false;

                    }
                }
            }
        }
        function UnCheckAll() {
            var intIndex = 0;
            var flag = 0;
            var list = document.getElementById("<%=chkroles.ClientID%>");
             var rowCount = list.getElementsByTagName("input");
             for (i = 0; i < rowCount.length; i++) {

                 if (document.getElementById("<%=chkroles.ClientID()%>" + "_" + i).checked == true) {
                    flag = 1;
                }
                else {
                    flag = 0;
                    break;
                }

            }
            if (flag == 0)
                document.getElementById("<%=cbAll.ClientID%>").checked = false;
        else
            document.getElementById("<%=cbAll.ClientID%>").checked = true;

    }




    </script>
    <style type="text/css">
       
    </style>
    <asp:UpdatePanel ID="updPnlGrid" runat="server">



        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsgg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="97%" Font-Size="Small"><h3>DashBoard Configuration</h3></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td style="text-align: center;">
                        <asp:Label ID="lbldfp" runat="server" CssClass="label_pop" Text="Default Page"></asp:Label>
                        &nbsp;
                        <asp:DropDownList ID="ddldefaultpage" Width="140px" runat="server" CssClass="txtBox_pop">
                            <asp:ListItem Value="0">Home</asp:ListItem>
                            <asp:ListItem Value="1">DashBoard</asp:ListItem>
                        </asp:DropDownList>

                        &nbsp;
                    <asp:Button ID="btnsetdefaultpage" CssClass="button_example" runat="server" Text="Set" />
                    </td>


                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double lime">
                        <table cellpadding="0" cellspacing="3" style="text-align: left" width="100%"
                            border="0">
                            <tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="lblmsg" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="100%" Font-Size="X-Small"></asp:Label>
                                </td>


                                <td style="text-align: right;">
                                    <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Plus.jpg" OnClick="Add" />

                                </td>

                            </tr>

                        </table>

                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>

                            </ProgressTemplate>
                        </asp:UpdateProgress>

                    </td>
                </tr>


                <tr style="color: #000000">
                    <td style="text-align: left;" valign="top">
                        <div style="height: 300px;">
                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                                CellPadding="2" DataKeyNames="Tid" OnSelectedIndexChanging="gvData_SelectedIndexChanging"
                                ForeColor="#333333" Width="100%" OnRowDataBound="gvData_RowDataBound"
                                AllowSorting="False" AllowPaging="False">
                                <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                                <SelectedRowStyle BackColor="White" Font-Bold="True" ForeColor="black" />
                                <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px"
                                    Height="25px" ForeColor="black"
                                    CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />

                                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                <SortedDescendingHeaderStyle BackColor="#93451F" />
                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                <Columns>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="S.No">
                                        <ItemTemplate>
                                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DBNAME" HeaderText="Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="widgetType" HeaderText="Widget Type">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="widgetnature" HeaderText="Widget Nature">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Roles" HeaderText="Roles">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="status" HeaderText="status">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <%--<asp:ImageButton ID="img_status" runat="server" CommandName="Select"
            ImageUrl='<%# Eval("Status")%>' Width="16px" Height="16px" />--%>
                                            <asp:LinkButton ID="img_status" runat="server" CommandName="Select" Text='<%# Eval("Status")%>' CommandArgument='<%# Eval("Status")%>'></asp:LinkButton>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDown" runat="server" AlternateText="Move Down"
                                                Height="16px" ImageUrl="~/images/down.png" OnClick="PositionDown"
                                                ToolTip="Move Down" Width="16px" />
                                            &nbsp;
                         <asp:ImageButton ID="btnUp" runat="server" AlternateText="Move Up"
                             Height="16px" ImageUrl="~/images/up.png" OnClick="PositionUp" ToolTip="Move Up"
                             Width="16px" />
                                            &nbsp;
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>


                                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/editrole1.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit DB Configuration" AlternateText="Edit" />
                                            &nbsp;
                                   <asp:ImageButton ID="btnDtl" ImageUrl="~/images/closered.png" runat="server" Height="16px" Width="16px" ToolTip="Delete Role" OnClick="DeleteHit" AlternateText="Delete" />
                                        </ItemTemplate>
                                        <ItemStyle Width="90px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>


                        </div>

                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button ID="btnShowPopupEdit" runat="server" PopupDragHandleControlID="pnlPopupEdit" Style="display: none" />


    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" PopupDragHandleControlID="pnlPopupEdit"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
   
        <asp:Panel ID="pnlPopupEdit" runat="server"  Width="800px">
            <div class="box">
                <table cellspacing="0px" cellpadding="10px" width="100%">
                    <tr>
                        <td style="width: 780px">
                            <h3>Create / Update DashBoard Configuration</h3>
                        </td>
                        <td style="width: 20px; background: #1f6b08;">
                            <asp:ImageButton ID="btnCloseEdit"
                                ImageUrl="images/closewhite.png" runat="server" Width="16px" /></td>
                    </tr>
                    <tr>
                        <td colspan="2">

                            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

                                    <table cellspacing="0px" cellpadding="0px" width="100%" style="padding: 5px 10px;" border="0">
                                        <tr>
                                            <td colspan="4" align="left">
                                                <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"
                                                    Width="100%" Font-Size="X-Small"></asp:Label>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td  style="text-align: left; width: 10%;">
                                                <asp:Label ID="lblName" CssClass="label_pop" runat="server" Width="99%" Text="Name*"></asp:Label>
                                            </td>
                                            <td style="text-align: left; width: 30%;">
                                                <asp:TextBox ID="txtname" runat="server" Width="95%" placeholder="DashBoard Name" CssClass="txtBox_pop"></asp:TextBox>
                                            </td>
                                            <td style="text-align: center; width: 10%;">
                                                <asp:Label ID="lbldbtype" CssClass="label_pop" runat="server" Width="99%" Text="DashBoard Type*"></asp:Label>
                                            </td>
                                            <td style="text-align: left; width: 30%;">
                                                <asp:DropDownList ID="ddldbtype" runat="server" AutoPostBack="true" Width="99%" CssClass="txtBox_pop">
                                                    <asp:ListItem Value="MainHome">Home</asp:ListItem>
                                                    <asp:ListItem Value="DashBoard">DashBoard</asp:ListItem>

                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>

                                            <td style="text-align: left; width: 10%;">
                                                <asp:Label ID="lblwtype" CssClass="label_pop" runat="server" Width="99%" Text="WidgetType*"></asp:Label>
                                            </td>
                                            <td style="text-align: left; width: 20%;">
                                                <asp:DropDownList ID="ddlwtype" runat="server" Width="99%" AutoPostBack="true" CssClass="txtBox_pop">
                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                    <asp:ListItem Value="Usefull Links">Usefull Links</asp:ListItem>
                                                    <asp:ListItem Value="Facebook Likes">Facebook Likes</asp:ListItem>
                                                    <asp:ListItem Value="Twitter">Twitter</asp:ListItem>
                                                    <asp:ListItem Value="New">New</asp:ListItem>

                                                </asp:DropDownList>
                                                <td style="text-align: center; width: 10%; padding-left: 55px;">
                                                    <asp:Label ID="lbldis" CssClass="label_pop" runat="server"  Text="Display On*"></asp:Label>
                                                </td>
                                                <td style="text-align: left; width: 20%;">
                                                    <asp:RadioButtonList ID="rdbtn" CssClass="txtBox_pop" Width="99%" RepeatColumns ="2" runat="server">
                                                        <asp:ListItem Value="0" >Mobile</asp:ListItem>
                                                        <asp:ListItem  Value="1" Selected="True" >Web</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="text-align: left; width: 10%; ">
                                                <asp:Label CssClass="label_pop" ID="lblwidth" runat="server" Visible="false" Width="99%" Text="Width*"></asp:Label>
                                            </td>
                                            <td style="text-align: left; width: 20%;">
                                                <asp:TextBox ID="txtwidth" runat="server" placeholder="Width" Visible="false" Width="95%" CssClass="txtBox_pop"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr runat="server" id="row1" visible="false">
                                            <td style="text-align: left; width: 10%;">
                                                <asp:Label ID="lblQuery" CssClass="label_pop" runat="server" Width="99%" Text="Query*"></asp:Label>
                                            </td>

                                            <td colspan="3" style="width: 60%; text-align: left;">

                                                <asp:TabContainer ID="QueryContainer" Width="100%" runat="server" CssClass="MyTabStyle" ActiveTabIndex="0">
                                                    <asp:TabPanel ID="tabroot" runat="server"
                                                        HeaderText="Root Query"
                                                        ScrollBars="Auto"
                                                        OnDemandMode="Once">

                                                        <ContentTemplate>
                                                            <asp:Panel ID="pnlroot" runat="server">
                                                                <table style="cellspacing: 5px; cellpadding: 0px; width: 100%; border: 0px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txtroot" placeholder="Write your Query Here ..." runat="server" Width="98%" TextMode="MultiLine"></asp:TextBox>
                                                                        </td>

                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </ContentTemplate>

                                                    </asp:TabPanel>
                                                    <asp:TabPanel runat="server" ID="tabfirstlevel">
                                                        <HeaderTemplate>First Level Query</HeaderTemplate>
                                                        <ContentTemplate>
                                                            <asp:Panel ID="pnlfirst" runat="server">
                                                                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">

                                                                    <td>
                                                                        <asp:TextBox ID="txtfirst" runat="server" placeholder="Write your first level query Here ..." Width="99%" TextMode="MultiLine"></asp:TextBox>
                                                                    </td>
                                                                </table>


                                                            </asp:Panel>
                                                        </ContentTemplate>
                                                    </asp:TabPanel>


                                                    <asp:TabPanel runat="server" ID="tabsecondlevel">
                                                        <HeaderTemplate>Second Level Query</HeaderTemplate>
                                                        <ContentTemplate>
                                                            <asp:Panel ID="pnlSecond" runat="server">
                                                                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txtsecond" runat="server" placeholder="Write your second level query Here ..." Width="99%" TextMode="MultiLine"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>


                                                            </asp:Panel>
                                                        </ContentTemplate>
                                                    </asp:TabPanel>
                                                </asp:TabContainer>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 10%; text-align: left;">
                                                <asp:Label ID="lblroles" CssClass="label_pop" runat="server" Width="99%" Text="Roles"></asp:Label>
                                            </td>
                                            <td colspan="3" style="width: 60%; text-align: left;">
                                                <div style="overflow: auto; height: 100px; width: 99%; margin: 10px 0; border: 1px solid #b9b8b8;">
                                                    <asp:CheckBox ID="cbAll" runat="server" Text="Select All" onclick="CheckAll();" /><br />
                                                    <asp:CheckBoxList ID="chkroles" runat="server" Height="100px" Width="99%"></asp:CheckBoxList>
                                                </div>

                                            </td>
                                        </tr>
                                        <tr runat="server" id="row3" visible="false">
                                            <td style="width: 13%; text-align: left;">
                                                <asp:Label CssClass="label_pop" ID="lblwnature" runat="server" Text="Widget Nature*"></asp:Label>
                                            </td>
                                            <td style="width: 20%; text-align: left;">
                                                <asp:DropDownList ID="ddlwnature" runat="server" Width="99%"  CssClass="txtBox_pop">
                                                    <asp:ListItem Value="Chart">Chart</asp:ListItem>
                                                    <asp:ListItem Value="Grid">Grid</asp:ListItem>

                                                </asp:DropDownList>

                                            </td>
                                            <td style="width: 20%; text-align: center;">
                                                <asp:Label CssClass="label_pop" ID="lblcellpos" runat="server" Visible="false" Width="99%" Text="Cell Position*"></asp:Label>
                                            </td>
                                            <td style="width: 20%; text-align: left;">
                                                <asp:TextBox ID="txtcellposition" runat="server" Visible="false" placeholder="Cell Position ..." CssClass="txtBox_pop" Width="99%"></asp:TextBox>

                                            </td>
                                        </tr>

                                    </table>
                                    <div style="width: 98%; text-align: right">
                                        <asp:Button ID="btnActEdit" runat="server" Text="Update"
                                            OnClick="EditRecord" CssClass="button_example" Font-Bold="True"
                                            Width="100px" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    

    <asp:Button ID="btnShowPopupDelFolder" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder"
        CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 480px">
                        <h3>Delete DashBoard Configuration</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelFolder"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                    <tr>
                                        <td colspan="2" align="left">
                                            <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete this Configuration!!" Font-Bold="True" ForeColor="Red"
                                                Width="100%" Font-Size="X-Small"></asp:Label>

                                        </td>
                                    </tr>
                                </table>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete"
                                        OnClick="DelFile" CssClass="button_example" Font-Bold="True"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

</asp:Content>

