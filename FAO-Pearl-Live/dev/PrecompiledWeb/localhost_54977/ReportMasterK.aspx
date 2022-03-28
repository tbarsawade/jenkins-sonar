<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="ReportMasterK, App_Web_sifhu5tb" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     
    <script src="js/ReadControlesK.js"></script>
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
    </style>

    

    

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
        .water
        {
            font-family: Tahoma, Arial, sans-serif;
            font-style: italic;
            color: gray;
        }
        
        </style>
    <style type="text/css">
        #mask
        {
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
        #loader
        {
            position: absolute;
            left: 50%;
            top: 50%;
            width:200px;
            height:70px;
            background:none;
            margin: -100px 0 0 -100px;
            display: none;
            padding-top: 25px;
        }
        .link
        {
            color: #35945B;
        }
        .link:hover
        {
            font: 15px;
            color: green;
            background: yellow;
            border: solid 1px #2A4E77;
        }
    </style>
    <asp:HiddenField ID="hdnView" Value="0" ClientIDMode="Static" runat="server" />
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
        <Triggers>
           
           
           
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
                        &nbsp; &nbsp; &nbsp; &nbsp;
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
                                        <div>
                                            <asp:Panel ID="pnlFields" runat="server" Width="960px" Height="100%" ScrollBars="Vertical">
                                            </asp:Panel>
                                            <asp:Panel ID="PnlControls" runat="server" Width="960px" Height="100%" ScrollBars="Vertical"
                                                Visible="false">
                                                <table>
                                                    <tr>
                                                        <td style="width: 110px" align="left">
                                                            <asp:Label ID="lblsdate" runat="server" Font-Bold="true" Font-Size="Small" Text="From Date :"></asp:Label>
                                                        </td>
                                                        <td style="width: 90px">
                                                            <asp:TextBox ID="txtsdate" runat="server" ></asp:TextBox>
                                                            <asp:CalendarExtender ID="calendersdate" TargetControlID="txtsdate" Format="yyyy-MM-dd"
                                                                runat="server" />
                                                        </td>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <td style="width: 110px" align="center">
                                                            <asp:Label ID="lbledate" runat="server" Font-Bold="true" Font-Size="Small" Text="To Date :"></asp:Label>
                                                        </td>
                                                        <td style="width: 90px" align="center">
                                                            <asp:TextBox ID="txtedate" runat="server" ></asp:TextBox>
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
                                                    Font-Bold="True" Font-Size="X-Small" Width="100px" OnClientClick="getReport(); return false;"/>
                                                <%--<asp:Button ID="btnViewInExcel" runat="server" Visible="True" Text="View in Excel"
                                                    CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Width="100px" />--%>&nbsp;&nbsp;
                                                <asp:LinkButton ID="btnViewInExcel" runat="server" CssClass="link"><b>Click to download in excel</b></asp:LinkButton>
                                            </div>
                                            
                                        </div>
                                        <br />
                                        
                                        <div style="max-width:1000px; overflow-x:scroll;">
                                        <div id="kgrid" >

                                        </div>
                                        </div>

                                        <div id="dvmsg" clientidmode="Static" runat="server" style="color:green; font-weight:bold;">
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
            
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="mask">
        <div id="loader">
            <img src="images/loading.gif" />
        </div>
    </div>
</asp:Content>
