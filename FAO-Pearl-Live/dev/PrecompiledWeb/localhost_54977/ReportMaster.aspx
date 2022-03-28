<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="ReportMaster, App_Web_sifhu5tb" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="js/gridviewScroll.min.js"></script>
    <link href="css/GridviewScroll.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {

            gridviewScroll();
            $('#<%=gvReport.ClientID%>').css("width", '100%');

        });

        function gridviewScroll() {
            $('.form').addClass('form pnlAutoHeight');
            $('#<%=gvReport.ClientID%>').gridviewScroll({
                width: 1320,
                height: 400,
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
        function ShowLocations() {
            document.getElementById("ContentPlaceHolder1_pngv").style.display = "none";
            document.getElementById("fmap").style.display = "block";
            return false;
        }
        $(document).ajaxStart(function () { $("#mask").css("display", "block"); $("#loader").css("display", "block"); });
        $(document).ajaxComplete(function () { $("#mask").css("display", "none"); $("#loader").css("display", "none"); });
    </script>
    <style type="text/css">
        .water {
            font-family: Tahoma, Arial, sans-serif;
            font-style: italic;
            color: gray;
        }

        .style2 {
            width: 100%;
            height: 73px;
        }
    </style>
    <style type="text/css">
        #mask {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #F6F6F6;
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }

        #loader {
            position: absolute;
            left: 50%;
            top: 50%;
            background-image: url("images/uploading.gif");
            background-repeat: no-repeat;
            background-position: center;
            margin: -100px 0 0 -100px;
            display: none;
            padding-top: 25px;
        }

        .link {
            color: #35945B;
        }

            .link:hover {
                font: 15px;
                color: green;
                background: yellow;
                border: solid 1px #2A4E77;
            }

        .mg {
            margin: 10px 0px;
        }

        .pnlAutoHeight {
            min-height: 0px !important;
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-size: 12px;
        }
    </style>
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btns" />
            <asp:PostBackTrigger ControlID="btnExport" />
            <asp:PostBackTrigger ControlID="btnexcel" />
            <asp:PostBackTrigger ControlID="Excelexport" />
            <asp:PostBackTrigger ControlID="btnexpo" />
            <asp:PostBackTrigger ControlID="btnViewInExcel" />
        </Triggers>
        <ContentTemplate>
            <div class="container-fluid">


                <div class="form">
                    <div class="doc_header">

                        <asp:Label ID="lblMsg" runat="server"></asp:Label>
                    </div>

                    <div class="col-md-11 col-sm-11 mg">
                        <asp:Label ID="lblCaption" runat="server"></asp:Label>
                    </div>
                    <div class="col-md-1 col-sm-1 mg">
                        <div class="pull-right">
                            <asp:ImageButton ID="showReport" runat="server" ToolTip="Show Report" Visible="false" ImageUrl="~/images/search.png"
                                Width="18px" Height="18px" OnClick="show" />&nbsp;
                            <asp:ImageButton ID="btnExport" ToolTip="Export PDF" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/images/export.png" />&nbsp;
                            <asp:ImageButton ID="btnexcel" ToolTip="Export CSV" runat="server" Visible="false" Width="18px" Height="18px"
                                ImageUrl="~/images/csv.png" />&nbsp;
                            <asp:ImageButton ID="Excelexport" ToolTip="Export EXCEL" runat="server" Width="18px"
                                Height="18px" ImageUrl="~/Images/excelexpo.jpg" />
                            &nbsp;
                            <asp:ImageButton ID="btnchart" runat="server" ToolTip="Show Charts" ImageUrl="~/images/chart1.png"
                                Width="18px" Height="18px" Visible="false" />
                            <asp:ImageButton ID="btnchangeView" runat="server" ImageUrl="~/images/worldmap.jpg"
                                CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="50px" Height="17px"
                                OnClientClick="return ShowLocations();" />
                        </div>
                    </div>
                    <div class="row mg">
                        <div class="col-md-12 col-sm-12">

                            <asp:Panel ID="pnlFields" CssClass="pnlAutoHeight" runat="server" Width="100%" ScrollBars="Vertical">
                            </asp:Panel>
                            <asp:Panel ID="PnlControls" runat="server" Height="100%" ScrollBars="Vertical"
                                Visible="false">
                                <table>
                                    <tr>
                                        <td style="width: 110px" align="left">
                                            <asp:Label ID="lblsdate" runat="server" Font-Bold="true" Font-Size="Small" Text="From Date :"></asp:Label>
                                        </td>
                                        <td style="width: 90px">
                                            <asp:TextBox ID="txtsdate" runat="server"></asp:TextBox>
                                            <asp:CalendarExtender ID="calendersdate" TargetControlID="txtsdate" Format="yyyy-MM-dd"
                                                runat="server" />
                                        </td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <td style="width: 110px" align="center">
                                                            <asp:Label ID="lbledate" runat="server" Font-Bold="true" Font-Size="Small" Text="To Date :"></asp:Label>
                                                        </td>
                                        <td style="width: 90px" align="center">
                                            <asp:TextBox ID="txtedate" runat="server"></asp:TextBox>
                                            <asp:CalendarExtender ID="calenderedate" TargetControlID="txtedate" Format="yyyy-MM-dd"
                                                runat="server" />
                                        </td>
                                        <td style="width: 110px" align="center">
                                            <asp:Label ID="lblLocation" runat="server" Font-Bold="true" Font-Size="Small" Text="Location :"></asp:Label>
                                        </td>
                                        <td style="width: 110px" align="center">
                                            <asp:CheckBoxList ID="CheckListLocation" DataValueField="tid" runat="server">
                                            </asp:CheckBoxList>
                                        </td>
                                        <td style="width: 110px" align="center">
                                            <asp:Label ID="lblUserName" runat="server" Font-Bold="true" Font-Size="Small" Text="UserName :"></asp:Label>
                                        </td>
                                        <td style="width: 110px" align="center">
                                            <asp:CheckBoxList ID="CheckListuserName" DataValueField="tid" runat="server">
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <div style="width: 100%; height: 100%; text-align: center; bottom: auto;">
                                <asp:Button ID="btnActEdit" runat="server" Visible="True" Text="Search" CssClass="btnNew"
                                    Font-Bold="True" Font-Size="X-Small" Width="100px" />
                                <%--<asp:Button ID="btnViewInExcel" runat="server" Visible="True" Text="View in Excel"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" />--%>&nbsp;&nbsp;
                                                <asp:LinkButton ID="btnViewInExcel" runat="server" CssClass="link"><b>Click to download in excel</b></asp:LinkButton>
                            </div>

                        </div>

                        <div class="col-md-12 col-sm-12 mg">
                            <asp:Panel runat="server" ID="pngv" Height="300px" Width="100%" ScrollBars="None" Style="overflow: scroll;">
                                <asp:GridView ID="gvReport" runat="server" CellPadding="2" CssClass="GridView" CaptionAlign="Left"
                                    ForeColor="#333333" Width="100%" AllowSorting="false" AllowPaging="true" PageSize="20">
                                    <FooterStyle CssClass="FooterStyle" />
                                    <RowStyle CssClass="RowStyle" />
                                    <EditRowStyle CssClass="EmptyDataRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                </asp:GridView>

                                <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </asp:Panel>
                        </div>
                    </div>

                    <table cellspacing="0px" cellpadding="0px" width="100%" border="0">
                        <tr>
                            <td style="width: 100%;" valign="top" align="left">


                                <div class="form" style="text-align: left">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTab" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>

                                                <br />

                                                <div id="dvmsg" clientidmode="Static" runat="server" style="color: green; font-weight: bold;">
                                                </div>

                                            </td>
                                        </tr>
                                    </table>
                                </div>

                            </td>
                        </tr>
                    </table>
                    <iframe id="fmap" name="fband" src="ReportOnMap.aspx" width="100%" height="500px"
                        scrolling="auto" frameborder="0" style="display: none;"></iframe>
                    <%--Graph Properties--%>
                    <asp:Panel runat="server" ID="pnlchart" Style="display: none;" Height="400px">
                        <table border="1" width="100%">
                            <tr style="height: 22px;">
                                <td style="width: 100px;" align="center">
                                    <asp:Label ID="lbl1" runat="server" Text="Select X"></asp:Label>
                                </td>
                                <td style="width: 150px;" align="center">
                                    <asp:DropDownList ID="ddlx" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 100px;" align="center">
                                    <asp:Label ID="lbl2" runat="server" Text="Select Y"></asp:Label>
                                </td>
                                <td style="width: 150px;" align="center">
                                    <asp:DropDownList ID="ddly" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 120px;" align="center">
                                    <asp:Label ID="lbl3" runat="server" Text="Chart Type"></asp:Label>
                                </td>
                                <td style="width: 100px;" align="center">
                                    <asp:DropDownList ID="ddlct" runat="server">
                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                        <asp:ListItem Value="1">Column</asp:ListItem>
                                        <asp:ListItem Value="2">Pie</asp:ListItem>
                                        <asp:ListItem Value="3">Line</asp:ListItem>
                                        <asp:ListItem Value="4">Area</asp:ListItem>
                                        <asp:ListItem Value="5">Pyramid</asp:ListItem>
                                        <asp:ListItem Value="6">Radar</asp:ListItem>
                                        <asp:ListItem Value="7">Bubble</asp:ListItem>
                                        <asp:ListItem Value="8">BoxPlot</asp:ListItem>
                                        <asp:ListItem Value="9">Candlestick</asp:ListItem>
                                        <asp:ListItem Value="10">ErrorBar</asp:ListItem>
                                        <asp:ListItem Value="11">Funnel</asp:ListItem>
                                        <asp:ListItem Value="12">Kagi</asp:ListItem>
                                        <asp:ListItem Value="13">Point</asp:ListItem>
                                        <asp:ListItem Value="14">Polar</asp:ListItem>
                                        <asp:ListItem Value="15">RangeColumn</asp:ListItem>
                                        <asp:ListItem Value="16">ThreeLineBreak</asp:ListItem>
                                        <asp:ListItem Value="17">Spline</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <td style="width: 100px;" align="center">
                                    <asp:ImageButton ID="btns" runat="server" ImageUrl="~/images/chart-search-icon.png"
                                        ToolTip="Search Charts" Width="19px" Height="19px" />
                                    &nbsp;<asp:ImageButton ID="btnexpo" runat="server" ToolTip="Export Chart" ImageUrl="~/images/downloadc.png"
                                        Width="19px" Height="19px" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="7">
                                    <asp:Chart ID="ch" runat="server" Width="1000px" Height="300px" BorderDashStyle="Solid"
                                        BackSecondaryColor="White" BackGradientStyle="TopBottom" BorderWidth="2px" BackColor="211, 223, 240"
                                        BorderColor="#1A3B69">
                                        <Titles>
                                            <asp:Title ShadowOffset="3" Name="Items" />
                                        </Titles>
                                        <Legends>
                                            <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="false" Enabled="false"
                                                Name="Default" LegendStyle="Row" />
                                        </Legends>
                                        <Series>
                                            <asp:Series Name="Series1" IsValueShownAsLabel="false" ChartArea="ChartArea1" BorderWidth="1">
                                            </asp:Series>
                                        </Series>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid"
                                                BackSecondaryColor="White" BackColor="Yellow" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                                <Area3DStyle Enable3D="true" LightStyle="Realistic" Rotation="15" Perspective="0"
                                                    Inclination="15" IsRightAngleAxes="False" WallWidth="10" IsClustered="False"></Area3DStyle>
                                                <AxisY LineColor="64, 64, 64, 64">
                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                                </AxisY>
                                                <AxisX LineColor="64, 64, 64, 64">
                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                                </AxisX>
                                            </asp:ChartArea>
                                        </ChartAreas>
                                    </asp:Chart>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="mask">
        <div id="loader">
        </div>
    </div>
</asp:Content>
