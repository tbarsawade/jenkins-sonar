<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ExtendRelation, App_Web_dqvq3srr" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="js/gridviewScroll.min.js"></script>
    <link href="css/GridviewScroll.css" rel="stylesheet" />

    <div style="width: 100%; border: solid 2px #e1e1e1; margin-top: 20px;">

        <table style="text-align: left";" width="100%" cellspacing="0px">
            <tr style="background-color: #e2e2e2; height: 30px;">
                <td colspan="3" style="border-bottom:1px solid #e1e1e1; padding:0 5px; background:#e1e1e1;"><b>Extend Relation</b></td>
            </tr>
            <tr>
                
                <td style="width:100%;">
                    <asp:UpdatePanel runat="server" ID="Up1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel runat="server" Visible="true" ID="pnlData">
                                <asp:UpdatePanel ID="upSearch" runat="server">
                                    <ContentTemplate>
                                        <div id="dvSearch">
                                            <table>
                                                <tr>
                                                    <td colspan="6"  style=" height:20px;"></td>
                                                </tr>
                                                <tr>
                                                    <td style="width:190px;">&nbsp;</td>
                                                    <td>
                                                        <label class="label" for="ContentPlaceHolder1_ddlFieldName">Filter on:</label></td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlFieldName" runat="server" CssClass="form-control" AutoPostBack="true" Width="200px">
                                                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <label class="label" for="ContentPlaceHolder1_ddlFormValue">Value:</label></td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlFormValue" runat="server" CssClass="form-control" Width="200px">
                                                            <asp:ListItem Value="0" Text="--Select--">
                                                        
                                                            </asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>&nbsp;</td>
                                                    <td>
                                                        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="myButton2" OnClientClick="javascript:return DocumentFilter();" />
                                                    </td>
                                                    <td style="width: 100px;">&nbsp;</td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <br />
                                <br />
                                <asp:UpdatePanel ID="upData" runat="server">
                                    <ContentTemplate>
                                        <span><b>Total Number Of Record(s) </b></span>
                                        <asp:Label runat="server" ID="lblDataCount"></asp:Label>
                                        <div id="divData" style="width: 99%; border: 0; margin-top: 20px;" class="dvGrid">
                                            <table>
                                                <tr>
                                                    <td style ="width:100%;">
                                                        <asp:GridView ID="gvData" runat="server" DataKeyNames="DOCID" Width="998px" AllowPaging="false"   AutoGenerateColumns="true" CellPadding="2">
                                                            <FooterStyle CssClass="FooterStyle" />
                                                            <RowStyle CssClass="RowStyle" />
                                                            <EditRowStyle CssClass="EditRowStyle" />
                                                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                                                            <PagerStyle CssClass="PagerStyle" />
                                                            <HeaderStyle CssClass=" HeaderStyle" />
                                                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                                            <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="SELECT">
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox runat="server"  onclick="javascript:checkAll();" Text="All" ID="ckhHeader" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox runat="server" ID="chkBox" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <table style="text-align: left";" width="100%">
                                    <tr style="height: 40px;">
                                        <td colspan="3"></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right" colspan="2">
                                            <asp:Button ID="btnExtend" runat="server" Text="Extend Relation" OnClientClick="javascript:return Approve();" CssClass="myButton2" />
                                        </td>
                                        <td style="width: 10px;"></td>
                                    </tr>
                                    <tr style="height: 20px;">
                                        <td colspan="3"></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </td>
            </tr>
        </table>

    </div>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <script type="text/javascript">
        $(document).ready(function () {
            gridviewScroll();
           $('#<%=gvData.ClientID%>').css("width",'100%');
        });

        function gridviewScroll() {
            $('#<%=gvData.ClientID%>').gridviewScroll({
                width: 1000,
                height: 600,
                arrowsize: 30,
                varrowtopimg: "Images/arrowvt.png",
                varrowbottomimg: "Images/arrowvb.png",
                harrowleftimg: "Images/arrowhl.png",
                harrowrightimg: "Images/arrowhr.png"
            });
        }

        function pageLoad(sender, args) {
            if (args.get_isPartialLoad()) {
                gridviewScroll();
            }
        }
</script>

    <script type="text/javascript">
        function checkAll() {
            //var obj = $("#ContentPlaceHolder1_gvData_ctl00").attr('checked');
            var obj = $("#ContentPlaceHolder1_gvData_ckhHeader_Copy");
            var obj2 = $('#<%=gvData.ClientID%>');
            var str = $(obj).prop('checked');
            if (str == true) {
                $("#ContentPlaceHolder1_gvDataWrapper").find(':checkbox').each(function () {
                    $(this).prop('checked', "checked");
                });
            }
            else {
                $("#ContentPlaceHolder1_gvDataWrapper").find(':checkbox').each(function () {
                    $(this).removeAttr('checked');
                });
            }
        }
        function Approve() {
            var ret = false;
            var Count = 0;
            var obj = $("#ContentPlaceHolder1_gvData_ckhHeader");
            var chkAll = $(obj).prop('checked');
            $("#ContentPlaceHolder1_gvData").find(':checkbox').each(function () {
                var IsChecked = $(this).prop('checked');
                if (IsChecked == true) {
                    Count = Count + 1;
                }
            });
            if (chkAll == true) {
                Count = Count - 1;
            }
            if (Count > 0) {
                var Msg = "You are going to extend " + Count + " relation(s).Click on, ''Ok'' to proceed, ''Cancel'' to deny.";
                if (confirm(Msg)) {
                    ret = true;
                }
                else { ret = false; }
            }
            else {
                alert("Please select a document to extend.");
            }
            return ret;
        }
    </script>
</asp:Content>

