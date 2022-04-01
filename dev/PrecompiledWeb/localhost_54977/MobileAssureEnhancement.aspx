<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="MobileAssureEnhancement, App_Web_zha44vbj" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">

        function OpenWindow(url) {
            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");           
            return false;
        }
    $(document).ready(function ()
    {
         
        });

        function ShowDialog(url) {
            // do some thing with currObj data

            var $dialog = $('<div></div>')
                .load(url)
                .dialog({
                    autoOpen: true,
                    title: 'Document Detail',
                    width: 700,
                    height: 550,
                    modal: true
                });
            return false;
        }
    </script>
    <style type="text/css">
        .gradientBoxesWithOuterShadows {
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
        }

        .mg {
            margin: 10px 0px;
        }

        .errormsg {
            text-align: center;
            margin: auto;
            font-weight: bold;
            color: #333;
            font-size: 24px;
            font-family: sans-serif;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSearch" />      
            <asp:PostBackTrigger ControlID="btnexportxl" />
        </Triggers>
        <ContentTemplate>
        <div class="container-fluid container-fluid-amend">
            <div class="form" style="text-align: left">
                <div class="doc_header">
                   Auto Reconciliation Report   
                </div>
                <div class="row mg">
                    <div class="col-md-2 col-sm-2 mg">
                        <label>Audit Cycle</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlDocType" CssClass="txtBox" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2 mg">
                        <label>Location</label>
                    </div>
                    <div class="col-md-2 col-sm-2">
                       
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="txtBox"></asp:TextBox>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:ImageButton ID="btnSearch" runat="server" Height="25px" ImageUrl="~/Images/search1.png" ToolTip="Search  " />
                        <asp:ImageButton ID="btnexportxl" ToolTip="Export EXCEL" ImageAlign="Right" runat="server" Width="18px" Height="18px" ImageUrl="~/Images/excelexpo.jpg" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12">
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <div style="text-align: center;">
                            <asp:Label ID="lblmsg" runat="server" ForeColor="Red" CssClass="errormsg"></asp:Label>
                        </div>

                        <asp:GridView ID="gvPending" AllowSorting="true" ShowFooter="false" AllowPaging="true" runat="server" AutoGenerateColumns="true"
                            CellPadding="3" Width="99%"
                            PageSize="15" BorderStyle="none" BorderColor="Green"
                            BorderWidth="1px" Font-Size="Small">
                            <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                            <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                            <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                            <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px"
                                Height="25px" ForeColor="black"
                                CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                            <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                           <%-- <Columns>
                                <asp:TemplateField HeaderText="Document Detail">
                                    <ItemTemplate>
                                        <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                            <%#DataBinder.Eval(Container.DataItem, "tid")%> </a>
                                    </ItemTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>--%>
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
       </div>         
        </ContentTemplate>
    </asp:UpdatePanel>
   
   
     
</asp:Content>


