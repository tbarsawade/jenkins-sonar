<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="ReallocationRights, App_Web_gsdfcjye" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src='http://code.jquery.com/jquery-latest.min.js' type='text/javascript'> </script>
    <script type="text/javascript">
        function SelectAllCheckboxes1(chk) {
            $('#<%=gvData.ClientID%>').find("input:checkbox").each(function () {
                if (this != chk) { this.checked = chk.checked; }
            });
        }
    </script>

    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <Triggers>
        </Triggers>
        <ContentTemplate>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                <ProgressTemplate>
                    <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                        <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                        please wait...
                    </div>

                </ProgressTemplate>
            </asp:UpdateProgress>
            <div class="container-fluid container-fluid-amend">
                <div class="form" style="text-align: left">
                <div class="doc_header">
                    Document Reallocation
                </div>
                <div class="row mg">
                    <div class="col-md-12 col-sm-12" style="text-align: center;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                        <asp:Label ID="lblMsgupdate" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                    </div>
                </div>
                <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>Document Type</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddldoctype" AutoPostBack="true" runat="server" CssClass="txtBox">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Status</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlstatus" runat="server" CssClass="txtBox">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-1 col-sm-1 mg-top10">
                        <asp:ImageButton ID="btnsearch" runat="server" Width="20px" Height="20px" OnClick="getresult" ImageUrl="~/Images/search-icon.png" />
                    </div>
                </div>
                <div class="row" id="xrow" runat="server" visible="false">
                    <div class="col-md-2 col-sm-2">
                        <label>Filter Current User</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlcu" runat="server" AutoPostBack="True" CssClass="txtBox">
                        </asp:DropDownList></td>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Filter Target User </label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddltu" AutoPostBack="false" runat="server" CssClass="txtBox">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-1 col-sm-1">

                        <asp:ImageButton ID="imgfilter" runat="server" Width="20px" Height="20px" OnClick="getresulfilter" ImageUrl="~/Images/Search.png" />
                    </div>
                </div>
                <div class="row" id="trdocid" runat="server" visible="false">
                    <div class="col-md-2 col-sm-2">
                        <label>Filter by DOCID</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:TextBox ID="txtDocid" runat="server" CssClass="txtBox">
                        </asp:TextBox>
                        <asp:TextBoxWatermarkExtender ID="txtdocidExtnd" runat="server" WatermarkText="Please enter ',' saparated Doc id " TargetControlID="txtDocid"></asp:TextBoxWatermarkExtender>
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <asp:ImageButton ID="imgDocid" runat="server" Width="20px" Height="20px" OnClick="getresultByDocid" ImageUrl="~/Images/Search.png" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12">
                        <asp:GridView ID="gvData" EmptyDataText="No Records Found" runat="server" AutoGenerateColumns="False"
                            CellPadding="2" DataKeyNames="Docid"
                            ForeColor="#333333" Width="100%"
                            AllowSorting="False" AllowPaging="False">
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#EFF3FB" />
                            <EditRowStyle BackColor="#2461BF" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="CornflowerBlue" Width="150px" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="S.No">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkheader" onclick="javascript:SelectAllCheckboxes1(this);" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chk" runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>


                                <asp:BoundField DataField="DocID" HeaderText="DocID">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Current User" HeaderText="Current User">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Target User" ItemStyle-Width="150px" ItemStyle-Height="20px">

                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddluser" Width="98%" runat="server" CssClass="txtBox"></asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12" style="text-align: center;">
                        <asp:Button ID="btnsave" runat="server" Text="Re-Allocate" Width="150px" Visible="false" CssClass="btnNew" />
                    </div>
                </div>
            </div>
            </div>

            <%--   <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                <tr style="color: #000000">
                    <td style="text-align: left;"></td>
                </tr>
                <tr>
                    <td style="text-align: center;"></td>

                </tr>

                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double lime">
                        <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0">
                            <tr>
                                <td style="width: 125px" align="left"><b></b></td>
                                <td align="left" style="width: 250px"></td>
                                <td style="width: 85px" align="left"><b></b></td>
                                <td align="left" style="width: 150px;"></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td style="width: 125px" align="left"><b></b></td>
                                <td align="left" style="width: 250px;">
                                    <td style="width: 125px" align="left"><b></b></td>
                                    <td align="left" style="width: 150px;"></td>
                                    <td></td>
                            </tr>
                            <tr>
                                <td style="width: 125px" align="left"><b></b>
                                </td>
                                <td align="left" style="width: 250px;"></td>
                                <td colspan="2"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td style="text-align: left;" valign="top">
                        <div style="overflow: scroll; height: 400px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <div>
                            

                        </div>

                    </td>
                </tr>
            </table>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
