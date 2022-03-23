<%@ page title="" language="VB" masterpagefile="~/usrFullScreen.master" autoeventwireup="false" inherits="NewReportMasterEcomp, App_Web_erizob0y" enableeventvalidation="false" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script src="js/ReadControlesEcompbeta.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>

    <style type="text/css">
        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }
        .k-grid table tr:hover 
        
        {
            background: rgb(69, 167, 84);
            color: black;

        }

        [data-ty*="MULTISELECT"] {
            font-size:12px!important;
        }

    </style>
  <%--  <script type="text/javascript" src="js/jquery.min.js">
    </script>--%>
     <script type="text/javascript">
         function RMH(r1, listr2) {
             debugger;
             var checked_checkboxes = $("[id*=" + r1 + "] input:checked");
             var Unchecked_checkboxes = $("[id*=" + r1 + "]");
             var uncheckval = Unchecked_checkboxes.length
             var value = checked_checkboxes.length + 1
             if (uncheckval == value) {
                 $("[id*=" + listr2 + "]").attr("checked", "checked");
             }
             else {
                 $("[id*=" + listr2 + "]").removeAttr("checked");
             }

         }

    </script>
    <script type="text/javascript">
        function r(r1, listr2) {
            debugger;
            if ($('#ContentPlaceHolder1_' + r1 + '').is(':checked')) {
                $('#ContentPlaceHolder1_' + listr2 + ' input[type=checkbox]').each(function () {
                    $(this).prop('checked', true);
                });
            }
            else {

                $('#ContentPlaceHolder1_' + listr2 + ' input[type=checkbox]').each(function ()
                { $(this).prop('checked', false); });
            }

        }
    </script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tabstrip").kendoTabStrip();
            //gridviewScroll();
            // $('#<%=gvReport.ClientID%>').css("width", '100%');
        });

        function gridviewScroll() {
            $('#<%=gvReport.ClientID%>').gridviewScroll({
                width: 960,
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
        /*label {
       font-weight: normal!important;
    padding-left: 7px!important;
    font-family: sans-serif!important;
    font-size:16px!important;
}*/

    </style>
    <style type="text/css">
        #mask {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.59);
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }

        #loader {
            position: absolute;
            left: 50%;
            top: 50%;
            width: 200px;
            height: 70px;
            background: none;
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
        .cls
        {
            width:100%;
        }
    </style>
    <asp:HiddenField ID="hdnView" Value="0" ClientIDMode="Static" runat="server" />
    <asp:UpdatePanel ID="updPnlGrid" runat="server" class="cls">
        <Triggers>
            <asp:PostBackTrigger ControlID="btns" />
            <asp:PostBackTrigger ControlID="btnExport" />
            <asp:PostBackTrigger ControlID="btnexcel" />
            <asp:PostBackTrigger ControlID="Excelexport" />
            <asp:PostBackTrigger ControlID="btnexpo" />
            <asp:PostBackTrigger ControlID="btnViewInExcel" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                border="0px">
                <tr style="color: #000000">
                    <td style="text-align: left;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red" Width="97%"
                            Font-Size="Small"><h4></h4></asp:Label>
                    </td>
                    <td align="right">
                        <asp:ImageButton ID="showReport" runat="server" ToolTip="Show Report" Visible="false" ImageUrl="~/images/search.png"
                            Width="18px" Height="18px" OnClick="show" />&nbsp;
                        <asp:ImageButton ID="btnExport" ToolTip="Export PDF" runat="server" Width="18px"
                            Height="18px" ImageUrl="~/images/export.png" Visible="False" />&nbsp;
                        <asp:ImageButton ID="btnexcel" ToolTip="Export CSV" runat="server" Visible="false" Width="18px" Height="18px"
                            ImageUrl="~/images/csv.png" />&nbsp;
                        <asp:ImageButton ID="Excelexport" ToolTip="Export EXCEL" runat="server" Width="18px"
                            Height="18px" ImageUrl="~/Images/excelexpo.jpg" Visible="False" />
                        &nbsp;
                        <asp:ImageButton ID="btnchart" runat="server" ToolTip="Show Charts" ImageUrl="~/images/chart1.png"
                            Width="18px" Height="18px" Visible="false" />
                        <asp:ImageButton ID="btnchangeView" runat="server" ImageUrl="~/images/worldmap.jpg"
                            CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="50px" Height="17px"
                            OnClientClick="return ShowLocations();" />
                    </td>
                </tr>
            </table>
            <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
                <tr>
                    <td style="width: 100%;" valign="top" align="left">
                        <%--<div id="main" style="min-height:400px">--%>
                        <asp:Label ID="lblCaption" runat="server" ClientIDMode="Static"></asp:Label>
                        <div class="form" style="text-align: left">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblTab" ClientIDMode="Static" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div >
                                            <asp:Panel ID="pnlFields" runat="server" Width="100%" Height="100%" ScrollBars="Vertical">
                                            </asp:Panel>
                                            <asp:Panel ID="PnlControls" runat="server" Width="100%" Height="100%" ScrollBars="Vertical"
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
                                                    Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="getReport(); return false;" />
                                                <%--<asp:Button ID="btnViewInExcel" runat="server" Visible="True" Text="View in Excel"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" />--%>&nbsp;&nbsp;
                                                <asp:LinkButton ID="btnViewInExcel" runat="server" Style="display: none;" CssClass="link"><b>Click to download in excel</b></asp:LinkButton>
                                            </div>

                                        </div>
                                        <br />

                                        <div id="tabstrip" >
                                            <ul>
                                                <li class="k-item k-state-default k-first k-tab-on-top k-state-active">Customers</li>
                                                <li>Sites</li>
                                                <li>Acts</li>
                                                <li>Contractors</li>
                                                <li>Company</li>
                                            </ul>

                                            <div style="min-height:300px;">
                                                <div id="Chart0" style=""></div>
                                            </div>
                                            <div style="min-height:300px;"><div id="Chart1" style=""></div></div>
                                            <div style="min-height:300px;"><div id="Chart2" style=""></div></div>
                                            <div style="min-height:300px;"><div id="Chart3" style=""></div></div>
                                            <div style="min-height:300px;"><div id="Chart4" style=""></div></div>
                                            <div style="min-height:300px;"><div id="Chart5" style=""></div></div>
                                        </div>

                                        <div style="width:100%; max-height: 250px; overflow-x: scroll;">
                                            
                                            <div id="divChart1" style="width: 49%; float: left;">
                                            </div>
                                        </div>


                                        <div style="width: 100%; overflow-x: auto; margin: 0px; padding: 0px;">
                                            <div id="kgrid" style="display: block; overflow-x: auto; width:1486px;">
                                            </div>
                                        </div>

                                        <div id="dvmsg" clientidmode="Static" runat="server" style="color: green; font-weight: bold;">
                                            <%--<asp:Label ID="lblMail" runat="server" Text=""></asp:Label>--%>
                                        </div>

                                        <asp:Panel runat="server" ID="pngv" Width="960px" Height="300px" ScrollBars="None">
                                            <asp:GridView ID="gvReport" runat="server" Visible="false" CellPadding="2" CssClass="GridView" CaptionAlign="Left"
                                                ForeColor="#333333" Width="100%" AllowSorting="false" AllowPaging="true">
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
            <asp:Panel runat="server" ID="pnlchart" Width="100%" Style="display: none;" Height="400px">
                <table border="1px" width="100%">
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
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="mask">
        <div id="loader">
            <img src="images/loading.gif" />
        </div>
    </div>
</asp:Content>
