<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="RuleEngineConfiguration, App_Web_dqvq3srr" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .gradientBoxesWithOuterShadows
        {
            height: 100%;
            width: 99%;
            padding: 5px;
            background-color: white;
            /* outer shadows  (note the rgba is red, green, blue, alpha) */
            -webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4);
            -moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);
            /* rounded corners */
            -webkit-border-radius: 12px;
            -moz-border-radius: 7px;
            border-radius: 7px;
            /* gradients */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5));
            background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%);
            background: -ms-linear-gradient(top, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* IE10+ */
            background: linear-gradient(to bottom, #e1ffff 0%,#fdffff 0%,#e1ffff 100%,#c8eefb 100%,#c8eefb 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#bee4f8 100%,#b1d8f5 100%,#e1ffff 100%,#e6f8fd 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e1ffff', endColorstr='#e6f8fd',GradientType=0 ); /* IE6-9 */
        }

        .style2
        {
            width: 30%;
        }
    </style>
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
         

        </Triggers>
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">

                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="100%" Font-Size="Small"></asp:Label>

                    </td>
                </tr>

                <tr>
                    <td style="text-align: center;">


                        <asp:Label ID="lblRecord" runat="server" ForeColor="red"
                            Font-Size="Small"></asp:Label>
                    </td>
                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double green">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="width: 90px;">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="black" Text="Field Name" Width="99%">
                                    </asp:Label>
                                </td>

                                <td style="width: 170px;">
                                    <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" Width="99%">
                                    </asp:DropDownList>
                                </td>

                                <td style="width: 50px;">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Text="Value" Width="99%"></asp:Label>
                                </td>

                                <td style="width: 200px;">
                                    <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox" Font-Bold="True"
                                        Font-Size="Small" Width="99%"></asp:TextBox>
                                </td>

                                <td style="text-align: right; width: 25px">
                                    <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                        ImageUrl="~/Images/search.png" />
                                </td>

                                <td style="text-align: right;">&nbsp;
             <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg" OnClick="Add" />



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
                    <td style="text-align: left" valign="top">
                        <div style="overflow: auto; width: 1000px">



                            <asp:GridView ID="gvData" runat="server" CssClass="GridView"
                                CellPadding="2" DataKeyNames="Ruleid" DataSourceID="SqlData"
                                AllowSorting="False" AllowPaging="True" AutoGenerateColumns="false" PageSize="15">
                                <FooterStyle CssClass="FooterStyle" />
                                <RowStyle CssClass="RowStyle" />
                                <EditRowStyle CssClass="EmptyDataRowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle" />
                                <PagerStyle CssClass="PagerStyle" />
                                <HeaderStyle CssClass="HeaderStyle" />
                                <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                <Columns>

                                 
<asp:TemplateField HeaderStyle-HorizontalAlign="Left"  HeaderText="S.No" >
   <ItemTemplate>    
       <%# CType(Container, GridViewRow).RowIndex + 1%>
   </ItemTemplate>
    <HeaderStyle HorizontalAlign="Left" />
</asp:TemplateField>

                        <asp:BoundField DataField="RUlename" HeaderText="Rule Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                                               <asp:BoundField DataField="formsource" HeaderText="Form Source">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="documenttype" HeaderText="Document Type">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                                          <asp:BoundField DataField="docnature" HeaderText="Doc Nature">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                                          <asp:BoundField DataField="whentorun" HeaderText="whentoRun">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                                

                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <%--<asp:ImageButton ID="img_status" runat="server" CommandName="Select"
            ImageUrl='<%# Eval("Status")%>' Width="16px" Height="16px" />--%>
                                            <asp:LinkButton ID="img_status" runat="server" Text='<%# Eval("Status")%>' OnClick="Active_Clicked"></asp:LinkButton>
                                           

                                        </ItemTemplate>
                                    </asp:TemplateField>
                          
                                                            <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit"  AlternateText="Edit" />
                                            &nbsp;
                                 <asp:ImageButton ID="btnDelete" ImageUrl="~/images/closered.png"  runat="server" Height="16px" Width="16px" ToolTip="Delete Role" OnClick="deletehit"  AlternateText="Delete" />

                                        </ItemTemplate>
                                        <ItemStyle Width="120px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SqlData" runat="server"
                                ConnectionString="<%$ ConnectionStrings:conStr %>"
                                SelectCommand="uspGetResultrules" SelectCommandType="StoredProcedure">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlField" Name="sField"
                                        PropertyName="SelectedValue" Type="String" />
                                    <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"
                                        Type="String" />
                                    <asp:SessionParameter Name="eid" SessionField="EID" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <%--<asp:SqlDataSource ID="SqlData" runat="server" 
               ConnectionString="<%$ ConnectionStrings:conStr %>" 
               SelectCommand="uspGetResultrules" SelectCommandType="StoredProcedure">
               <SelectParameters>
                   <asp:ControlParameter ControlID="ddlField" Name="sField" 
                       PropertyName="SelectedValue" Type="String" />
                   <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"  
                       Type="String" />
                   <asp:SessionParameter Name="eid" SessionField="EID" Type="String"  />
               </SelectParameters>
           </asp:SqlDataSource>--%>
                        </div>
                    </td>
                </tr>
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>
            <asp:Button id="btnShowPopupDelFolder" runat="server" style="display:none" />
    <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server" 
                                TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder" 
                CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupDelFolder" runat="server" Width="500px" style="display:none;" BackColor="aqua">
<div class="box">
<table cellspacing="2px" cellpadding="2px" width="100%">
<tr>
<td style="width:480px"><h3>Delete Rule</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseDelFolder" 
        ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
 <ContentTemplate> 
    <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
   <tr>
    <td colspan="2" align="left" >
    <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete" Font-Bold="True" ForeColor="Red" 
            Width="100%" Font-Size="X-Small" ></asp:Label>
    
    </td>
   </tr>
  </table>
          <div style="width:100%;text-align:right">
                    <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete" 
                            OnClick="DelFile" CssClass="btnNew" Font-Bold="True" 
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
