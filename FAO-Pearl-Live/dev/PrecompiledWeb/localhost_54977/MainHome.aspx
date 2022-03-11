<%@ page title=":: Welcome ::" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="MainHome, App_Web_ik1k4di5" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <%--<script src="kendu/js/jquery.min.js"></script>--%>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>
    <link href="css/style.css" rel="stylesheet" />
    <script type="text/javascript">
        //Dated 18-November-2015 By Ajeet
        //used for refressing only needtoact grid on child close
        var new_window;
        function OpenWindow(url) {
            var DOCID = url.split("=")
            GetCURDOCUserRole(DOCID[1], $("#ddlUserRole").val());
            new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            return false;
        }
        function GetCURDOCUserRole(DOCID, CURUSERROLE) {
            var t = '{"DOCID":"' + DOCID + '","CURUSERROLE":"' + CURUSERROLE + '"}';
            $.ajax({
                type: "POST",
                url: "MAINHome.aspx/GetCurDocUserRole",
                data: t,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $("#ddlUserRole").val(response.d);
                    if (response.d != CURUSERROLE) {
                        $("#ddlUserRole").change();
                    }
                    //if (res != "") {
                    //    var data = $.parseJSON(res);
                    //    alert(data);
                    //}
                    //if (res != "") {
                    //    var data = $.parseJSON(res);

                    //    var dbtype = ""
                    //    $.each(data, function (i) {
                    //        dbtype = this.Widgettype;
                    //        $("#dv" + (i + 1)).html("<div class='loader'></div>").removeClass("divCustom").addClass("divCustom1");

                    //        switch (dbtype) {

                    //            case "Usefull Links":
                    //                {
                    //                    bindUseFulLink(this.DBName, "dv" + (i + 1));
                    //                }
                    //                break;
                    //            case "New":
                    //                {
                    //                    bindCustomWidget(this.Tid, "dv" + (i + 1), this.DBName, (i + 1));
                    //                }
                    //        }

                    //    });
                    //}
                },

                error: function (data) {
                    //alert("Hi Error occured while calling!!!");
                }
                //Ajax call end here 
            });
        }
        //var win = window.open("Child.aspx", "thePopUp", "");
        function childClose() {
            // if (new_window.closed) {
            $("#ContentPlaceHolder1_btnpendinggrdcl").click();
            // }
        }
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
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
        }

        .modalPopup {
            background-color: #FFFFFF;
            width: 350px;
            border: 3px solid #0DA9D0;
            border-radius: 12px;
            padding: 0;
        }

            .modalPopup .header {
                background-color: red;
                height: 30px;
                color: White;
                line-height: 30px;
                text-align: center;
                font-weight: bold;
                border-top-left-radius: 6px;
                border-top-right-radius: 6px;
            }

            .modalPopup .body {
                min-height: 50px;
                line-height: 30px;
                text-align: center;
                font-weight: bold;
            }

            .modalPopup .footer {
                padding: 6px;
            }

            .modalPopup .yes, .modalPopup .no {
                height: 23px;
                color: White;
                line-height: 23px;
                text-align: center;
                font-weight: bold;
                cursor: pointer;
                border-radius: 4px;
            }

            .modalPopup .yes {
                background-color: #598526;
                border: 1px solid #5C5C5C;
            }

            .modalPopup .no {
                background-color: #598526;
                border: 1px solid #5C5C5C;
            }

        .menu-bar {
            width: 95%;
            margin: 4px 0px 0px 0px;
            padding: 1px 1px 1px 1px;
            height: auto;
            line-height: inherit;
            border-radius: 10px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            box-shadow: 2px 2px 3px #666666;
            -webkit-box-shadow: 2px 2px 3px #666666;
            -moz-box-shadow: 2px 2px 3px #666666;
            background: #8B8B8B;
            background: linear-gradient(top, #ccc7c5, #7A7A7A);
            background: -ms-linear-gradient(top, #ccc7c5, #7A7A7A);
            background: -webkit-gradient(linear, left top, left bottom, from(#ccc7c5), to(#7A7A7A));
            background: -moz-linear-gradient(top, #ccc7c5, #7A7A7A);
            border: solid 1px #6D6D6D;
            position: relative;
            /*z-index:999;
  -webkit-z-index:999;*/
        }

            .menu-bar li {
                margin: 0px 0px 1px 0px;
                padding: 0px 1px 0px 1px;
                float: left;
                position: relative;
                list-style: none;
            }

            .menu-bar a {
                text-decoration: none;
                padding: 1px 2px 1px 2px;
                margin-bottom: 1px;
                border-radius: 1px;
                -webkit-border-radius: 1px;
                -moz-border-radius: 1px;
                font-weight: bold;
                font-family: arial;
                font-style: normal;
                font-size: 12px;
                color: #d0e41f;
                text-shadow: 1px 1px 1px #000000;
                display: block;
                margin: 0;
                margin-bottom: 1px;
                border-radius: 4px;
                -webkit-border-radius: 4px;
                -moz-border-radius: 4px;
                text-shadow: 2px 2px 3px #000000;
            }

            .menu-bar li ul li a {
                margin: 0;
            }

            .menu-bar .active a, .menu-bar li:hover > a {
                background: #0399D4;
                background: linear-gradient(top, #EB2F2F, #960000);
                background: -ms-linear-gradient(top, #EB2F2F, #960000);
                background: -webkit-gradient(linear, left top, left bottom, from(#EB2F2F), to(#960000));
                background: -moz-linear-gradient(top, #EB2F2F, #960000);
                color: #F2F2F2;
                -webkit-box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                -moz-box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                text-shadow: 2px 2px 3px #FFA799;
            }

            .menu-bar ul li:hover a, .menu-bar li:hover li a {
                background: none;
                border: none;
                color: #666;
                -box-shadow: none;
                -webkit-box-shadow: none;
                -moz-box-shadow: none;
            }

            .menu-bar ul a:hover {
                background: #0399D4 !important;
                background: linear-gradient(top, #EB2F2F, #960000) !important;
                background: -ms-linear-gradient(top, #EB2F2F, #960000) !important;
                background: -webkit-gradient(linear, left top, left bottom, from(#EB2F2F), to(#960000)) !important;
                background: -moz-linear-gradient(top, #EB2F2F, #960000) !important;
                color: #FFFFFF !important;
                border-radius: 0;
                -webkit-border-radius: 0;
                -moz-border-radius: 0;
                text-shadow: 2px 2px 3px #FFA799;
            }

            .menu-bar ul {
                background: #DDDDDD;
                background: linear-gradient(top, #FFFFFF, #CFCFCF);
                background: -ms-linear-gradient(top, #FFFFFF, #CFCFCF);
                background: -webkit-gradient(linear, left top, left bottom, from(#FFFFFF), to(#CFCFCF));
                background: -moz-linear-gradient(top, #FFFFFF, #CFCFCF);
                display: none;
                margin: 0;
                padding: 0;
                width: 0px;
                position: absolute;
                /*top: 10px;*/
                left: 0;
                border: solid 1px #B4B4B4;
                border-radius: 10px;
                -webkit-border-radius: 10px;
                -moz-border-radius: 10px;
                -webkit-box-shadow: 2px 2px 3px #222222;
                -moz-box-shadow: 2px 2px 3px #222222;
                box-shadow: 2px 2px 3px #222222;
            }

            .menu-bar li:hover > ul {
                display: block;
            }

            .menu-bar ul li {
                float: none;
                margin: 0;
                padding: 0;
            }

        .menu-bar {
            display: inline-block;
        }

        html[xmlns] .menu-bar {
            display: block;
        }

        * html .menu-bar {
            height: 1%;
        }

        .doclink li {
            list-style: none;
            text-decoration: none;
            color: #000;
            padding: 6px 5px;
            border-bottom: 1px solid rgba(0, 0, 0, .2);
        }

            .doclink li a {
                text-decoration: none;
            }

                .doclink li a:hover {
                    text-decoration: none;
                }

            .doclink li:hover {
                background: #598526;
                color: #fff!important;
            }


        .mar_botm {
            margin-bottom: 10px;
            border: 1px solid green;
            text-align: left;
        }

        /*.mar_botm div {
            border: 1px solid green; overflow: auto; width: 100%; text-align: left; padding-left: 0px;display:none; 
        }*/
        .divCustom {
            border: 1px solid green;
            overflow: auto;
            width: 100%;
            text-align: left;
            padding-left: 0px;
            display: none;
        }

        .divCustom1 {
            border: 1px solid green;
            overflow: auto;
            width: 100%;
            text-align: left;
            padding-left: 0px;
        }

        .loader {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .loader1 {
            background-image: url("images/loading.gif");
            background-repeat: no-repeat;
            background-position: center center;
            position: relative;
            top: calc(50% - 16px);
            display: block;
            height: 50px;
        }

        .tdhover {
            background: #c6def5;
        }

            .tdhover:hover {
                color: #3585f3;
                text-underline-position: below;
            }
    </style>
    <table width="100%" cellspacing="0px" cellpadding="0px">
        <tr valign="top">
            <td style="overflow: scroll">
                <div>
                    <asp:Label ID="Label1" CssClass="txtBox left" Text="Document Type" runat="server"></asp:Label>
                    <asp:UpdatePanel ID="Update" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddldynamic" Width="200px" ToolTip="Select Document Type to view all documents" CssClass="txtBox sel" runat="server" AutoPostBack="true"></asp:DropDownList><br />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <br />
                </div>


                <asp:Panel ID="dash" runat="server">
                    <div id="tabs" style="width: 700px">

                        <ul>
                            <li><a href="#tabPending">Need to Act                                       
                                                <asp:Label ID="lblpending" runat="server" EnableViewState="false"></asp:Label>
                            </a></li>

                            <li><a href="#tabMy">My Request
                            <asp:Label ID="lblaction" runat="server" EnableViewState="false"></asp:Label></a></li>
                            <li><a href="#tabAppr">History
                            <asp:Label ID="lbluploading" runat="server" EnableViewState="false"></asp:Label></a></li>
                            <li>
                                <asp:Label ID="lblDraft" runat="server" EnableViewState="false"></asp:Label></li>
                        </ul>

                        <div id="tabPending" style="min-height: 300px;">
                            <asp:UpdatePanel ID="updPnlGrid" runat="server">
                                <ContentTemplate>
                                    <asp:Button ID="btnRefresh" runat="server" OnClick="RefreshPanel" Style="display: none" Text="Refresh" Width="100px" />
                                    <div>
                                        <asp:UpdatePanel ID="upddropdown" runat="server">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="ddlPendinggrdHdr" runat="server" CssClass="txtBox sel" Width="150px" Height="25px">
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlPendinggrdVal" Visible="false" runat="server" CssClass="txtBox" Width="150px" Height="25px"
                                                    AutoPostBack="True" />
                                                <asp:TextBox ID="txtPendinggrdval" CssClass="txtBox" runat="server"></asp:TextBox>
                                                <asp:Button ID="btnpendinggrdcl" runat="server" CssClass="btnNew" Text="Search" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                            <ProgressTemplate>
                                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%; z-index: 10001">
                                                    <asp:Image ID="Imageprog" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                    please wait...
                                                </div>
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </div>
                                    <asp:Panel ID="pnlfixed" runat="server" ScrollBars="Horizontal">
                                        <asp:GridView ID="gvPending" DataKeyNames="SYSTEM ID" runat="server" AutoGenerateColumns="false" EmptyDataText="Record does not exists." ShowFooter="false" AllowPaging="true" CellPadding="3" Width="800px"
                                            PageSize="10" BorderStyle="none" BorderColor="Green"
                                            BorderWidth="1px" Font-Size="Small" AllowSorting="true" OnSorting="gvPending_Sorting">
                                            <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                                            <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle HorizontalAlign="Center" Font-Bold="true" Font-Size="24px" CssClass="pager_no" />
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
                                        </asp:GridView>

                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                        <div id="tabMy" style="min-height: 300px;">

                            <asp:UpdatePanel ID="updPNLMyUpload" runat="server">
                                <ContentTemplate>
                                    <div>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="ddlMyReqHdr" runat="server" CssClass="txtBox" Width="150px" Height="25px">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtmyreqval" runat="server" CssClass="txtBox"></asp:TextBox>
                                                <asp:Button ID="btnmyreqcl" runat="server" CssClass="btnNew" Text="Search" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                            <ProgressTemplate>
                                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                    <asp:Image ID="Image41" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                    please wait...
                                                </div>
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </div>
                                    <asp:GridView ID="gvMyUpload" runat="server" EmptyDataText="Record does not exists." AllowPaging="true" ShowFooter="false" AutoGenerateColumns="true"
                                        CellPadding="3" DataKeyNames="SYSTEM ID" Width="700px" CssClass="gridviewhome"
                                        PageSize="10" BorderStyle="none" BorderColor="Green"
                                        BorderWidth="1px" Font-Size="Small">
                                        <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                                        <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                        <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px"
                                            Height="25px" ForeColor="black"
                                            CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#594B9C" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#33276A" />
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                    </asp:GridView>
                                    <table cellspacing="0px" cellpadding="0px" width="100%" border="0">
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                        <div id="tabAppr" style="min-height: 300px;">
                            <asp:UpdatePanel ID="updAction" runat="server">
                                <ContentTemplate>
                                    <div>
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="ddlGrdHdr" runat="server" CssClass="txtBox" Width="150px" Height="25px">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtgrdval" CssClass="txtBox" runat="server"></asp:TextBox>
                                                <asp:Button ID="btngrdcl" runat="server" CssClass="btnNew" Text="Search" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdateProgress ID="UpdateProgress3" runat="server">
                                            <ProgressTemplate>
                                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                    <asp:Image ID="Image3" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                    please wait...
                                                </div>
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </div>
                                    <asp:GridView ID="gvAction" runat="server" EmptyDataText="Record does not exists." AllowPaging="true"
                                        ShowFooter="false" AutoGenerateColumns="true"
                                        CellPadding="3" DataKeyNames="SYSTEM ID" Width="700px"
                                        PageSize="10" BorderStyle="None" BorderColor="Green"
                                        BorderWidth="1px" Font-Size="Small">

                                        <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                        <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                        <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px" ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />
                                        <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                        <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                        <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                        <SortedDescendingHeaderStyle BackColor="#93451F" />
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                    </asp:GridView>
                                    <table cellspacing="0px" cellpadding="0px" width="100%" border="0">
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="tabDraf" runat="server" style="min-height: 300px; display: none;">
                            <asp:UpdatePanel ID="UpdpnlDraft" runat="server">
                                <ContentTemplate>

                                    <div>
                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                            <ContentTemplate>

                                                <asp:DropDownList ID="ddlDraftHdr" runat="server" CssClass="txtBox" Width="150px" Height="25px">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtDraftVal" CssClass="txtBox" runat="server">
                                                </asp:TextBox>
                                                <asp:Button ID="btndraftval" runat="server" CssClass="btnNew" Text="Search" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdateProgress ID="UpdateProgress4" runat="server">
                                            <ProgressTemplate>
                                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                    please wait...
                                                </div>
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </div>

                                    <asp:GridView ID="gvDraft" runat="server" EmptyDataText="Record does not exists." AllowPaging="true" ShowFooter="false" AutoGenerateColumns="True" CellPadding="3" DataKeyNames="SYSTEM ID" Width="700px" PageSize="10" BorderStyle="None" BorderColor="Green" BorderWidth="1px" Font-Size="Small">
                                        <RowStyle BackColor="White" CssClass="gridrowhome" Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                        <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle ForeColor="green" HorizontalAlign="Center" />
                                        <HeaderStyle Font-Bold="True" BorderColor="Green" BorderWidth="1px" Height="25px" ForeColor="black" CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center" />

                                        <Columns>



                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>


                                                    <asp:ImageButton ID="btnprint" runat="server" ImageUrl="~/images/Print64.png" Height="16px" Width="16px" OnClick="printHit" ToolTip="Print" AlternateText="Print" />
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" OnClick="EditHit" ToolTip="eidt" AlternateText="edit Hit" />
                                                    &nbsp;
                                <asp:ImageButton ID="btnDiscard" runat="server" ImageUrl="~/images/closered.png" Height="16px" Width="16px" OnClick="DocDiscardHit" ToolTip="discard" AlternateText="discard" />
                                                </ItemTemplate>
                                                <ItemStyle Width="120px" HorizontalAlign="Center" />
                                            </asp:TemplateField>

                                        </Columns>

                                        <SortedAscendingCellStyle BackColor="#FFF1D4" />
                                        <SortedAscendingHeaderStyle BackColor="#B95C30" />
                                        <SortedDescendingCellStyle BackColor="#F1E5CE" />
                                        <SortedDescendingHeaderStyle BackColor="#93451F" />
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                    </asp:GridView>
                                    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </asp:Panel>


                <script>
                    $(function () {
                        $("#tabs").tabs();
                    });
                </script>
            </td>
            <td style="width: 30%; padding-left: 5px" valign="top">
                <div class="mar_botm" id="wdgtwrap" style="display: none;">
                    <div id="dv1" class="box divCustom"></div>
                    <div id="dv2" class="box divCustom"></div>
                    <div id="dv3" class="box divCustom"></div>
                    <div id="dv4" class="box divCustom"></div>
                    <div id="dv5" class="box divCustom"></div>
                    <div id="dv6" class="box divCustom"></div>
                    <div id="dv7" class="box divCustom"></div>
                    <div id="dv8" class="box divCustom"></div>
                    <div id="dv9" class="box divCustom"></div>
                </div>
            </td>
        </tr>
    </table>

    <div class="Form">
    </div>
    <asp:Button ID="btnShowCalendar" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btncalendar_modalPopupExtender" runat="server"
        TargetControlID="btnShowCalendar" PopupControlID="pnlPopupCalendar"
        CancelControlID="btnCloseCalendar" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupCalendar" runat="server" Width="500px" Height="550px" Style="display: none;" BackColor="SILVER">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 480px">
                        <h3>Calendar Detail</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseCalendar"
                            ImageUrl="images/close.png" runat="server" ToolTip="Close" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updTaskPanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="form">
                                    <asp:GridView ID="grdTask" runat="server" AutoGenerateColumns="False"
                                        CellPadding="2" Width="100%"
                                        BackColor="LightGoldenrodYellow" BorderColor="Tan"
                                        BorderWidth="1px" ForeColor="Black" GridLines="None">
                                        <FooterStyle BackColor="Tan" />
                                        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                                        <PagerStyle ForeColor="DarkSlateBlue" HorizontalAlign="Center"
                                            BackColor="PaleGoldenrod" />
                                        <HeaderStyle Font-Bold="True"
                                            CssClass="GridHeader" BackColor="Tan" />
                                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                                        <Columns>
                                            <asp:BoundField DataField="time" HeaderText="Time">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="task" HeaderText="Event">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                        </Columns>
                                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                    </asp:GridView>
                                </div>
                                <br />


                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnim" runat="server" Style="display: none;" />
    <asp:ModalPopupExtender ID="modalpopuppassexp" runat="server" PopupControlID="PnlPassExp" TargetControlID="btnim"
        CancelControlID="btnNo" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>

    <asp:UpdatePanel ID="UpdatePanelPassexp" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="PnlPassExp" runat="server" CssClass="modalPopup">
                <div class="header">
                    <asp:Label runat="server" ID="lblPassexpmsg"></asp:Label>
                </div>
                <div class="body">
                    We recommend you to change your password
                </div>
                <div class="footer" align="right">
                    <asp:Button ID="btnYes" runat="server" Text="Change Password" CssClass="yes" Width="140px" />&nbsp;
        <asp:Button ID="btnNo" runat="server" Text="Skip" CssClass="no" Width="60px" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>


    <asp:Button ID="btnAlert" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_Alert" runat="server" PopupControlID="pnlAlert" TargetControlID="btnAlert" BackgroundCssClass="modalBackground" CancelControlID="btnAlertClose">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlAlert" runat="server" Width="450px" Height="100px" BackColor="White" Style="display: none">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td style="width: 400px">
                        <h3>Message </h3>
                    </td>
                    <td align="right">
                        <asp:ImageButton ID="btnAlertClose" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updAlert" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblAlertMes" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>&nbsp;
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnCloseAlertMess" runat="server" Text="OK" OnClick="HideAlert" CssClass="btnNew" Width="80px" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>




    <a id="anchorId" runat="server" onclick="return true" onserverclick="RefreshPanel" style="display: none">this will be clicked</a>
    <asp:Button ID="btnShowconfirm" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnConfirm" runat="server" PopupControlID="pnlconfirm" TargetControlID="btnShowconfirm" BackgroundCssClass="modalBackground" CancelControlID="btnClose">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlconfirm" runat="server" Width="400px" Height="100px" BackColor="White" Style="display: none">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td style="width: 400px">
                        <h3>Confirmation  </h3>
                    </td>
                    <td align="right">
                        <asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updMsg" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblMsg" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>&nbsp;
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnOk" runat="server" Text="OK" OnClick="ConfirmDelete" CssClass="btnNew" Width="80px" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


    <asp:Button ID="btnAppHid" runat="server" Text="Button" Style="display: none;" />
    <asp:ModalPopupExtender ID="ModalApprove" runat="server" PopupControlID="pnlPopupApprove1" TargetControlID="btnAppHid"
        CancelControlID="btnCloseApprove" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>

    <asp:UpdatePanel ID="UpdatePnl4" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlPopupApprove1" runat="server" Width="820px" Height="500px" Style="Display: none; background: #fff; overflow-y: scroll;" BackColor="#fff">
                <div class="box">
                    <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                        <tr>
                            <td>
                                <h3>
                                    <asp:Label ID="lblApp" runat="server"></asp:Label>
                                    Document</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="btnCloseApprove" ImageUrl="images/close.png" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: left; width: 100%">
                                <asp:UpdateProgress ID="UpdateProgress5" runat="server">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%; z-index: 10001">
                                            <asp:Image ID="Image11" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <asp:UpdatePanel ID="updatePanelApprove" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="lblTabApprove" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                        <asp:Panel ID="pnlApprove" Width="100%" runat="server" ScrollBars="Vertical" Style="min-height: 400px !important">
                                        </asp:Panel>
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td style="text-align: right;">
                                                    <div style="width: 100%; text-align: right;">
                                                        <asp:Label ID="lblMsgRule2" runat="server" Text=""></asp:Label>
                                                        <asp:Button ID="btnApprove1" runat="server" Text="Approve" OnClientClick="this.disabled=true;" OnClick="editBtnApprove" UseSubmitBehavior="false"
                                                            CssClass="ststusButton" Font-Bold="True"
                                                            Font-Size="X-Small" Width="100px" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:ModalPopupExtender ID="btnReject_ModalPopupExtender" runat="server"
        TargetControlID="btnRejModel" PopupControlID="pnlPopupReject"
        CancelControlID="btnCloseReject" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Button ID="btnRejModel" runat="server" Text="Button" Style="display: none;" />
    <asp:Panel ID="pnlPopupReject" runat="server" Width="820px" Height="450px" Style="Display: none; overflow-y: scroll; background: #fff" BackColor="#fff">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" style="width: 100%; empty-cells: show;">
                <tr>
                    <td>
                        <h3>
                            <asp:Label ID="lblRej" runat="server"></asp:Label>
                            Document</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseReject"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left">
                        <asp:UpdatePanel ID="updatePanelReject" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:Label ID="lblTabRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                <asp:Panel ID="pnlFieldsRej" Width="100%" runat="server" ScrollBars="Vertical" Style="min-height: 300px">
                                </asp:Panel>
                                <div style="width: 100%; text-align: right">
                                    <asp:Label ID="lblRuleMsg3" runat="server"></asp:Label>
                                    <asp:Button ID="btnReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" CssClass="ststusButton" Font-Bold="True"
                                        Font-Size="X-Small" OnClick="editBtnReject" Text="Reject" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


    <asp:Button ID="btnPerRejectpopup" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnPerRejectModalpopup" runat="server"
        TargetControlID="btnPerRejectpopup" PopupControlID="pnlPerReject"
        CancelControlID="btnClosePerReject" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPerReject" runat="server" Width="820px" Height="450px" Style="Display: none; background: #fff; overflow-y: scroll;" BackColor="#fff">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <h3>Permanent
                            <asp:Label ID="lblPRej" runat="server"></asp:Label>
                            Document</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnClosePerReject"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left">
                        <asp:UpdatePanel ID="updPerReject" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:Label ID="lblPerRej" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                <asp:Panel ID="PanelPerReject" Width="100%" runat="server" ScrollBars="Vertical" Style="min-height: 400px">
                                </asp:Panel>


                                <div style="width: 100%; text-align: right">
                                    <asp:Label ID="lblMsgRule1" runat="server"></asp:Label>
                                    <asp:Button ID="btnPerReject" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" CssClass="ststusButton" Font-Bold="True"
                                        Font-Size="X-Small" OnClick="editBtnPerReject" Text="Reject" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


    <script type="text/javascript">
        $(document).ready(function () {
            //alert("document ready function fired.");
            $.ajax({
                type: "POST",
                url: "MainHome.aspx/GetWidgets",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    if (res != "") {
                        var data = $.parseJSON(res);
                        var dbtype = ""
                        var WNature = "";
                        $.each(data, function (i) {
                            dbtype = this.Widgettype;
                            WNature = this.WidgetNature;
                            $("#dv" + (i + 1)).html("<div class='loader'></div>").removeClass("divCustom").addClass("divCustom1");
                            $("#wdgtwrap").show();
                            switch (dbtype) {

                                //<div class="loader1"></div>
                                case "Usefull Links":
                                    {
                                        bindUseFulLink(this.DBName, "dv" + (i + 1));
                                    }
                                    break;
                                case "New":
                                    {
                                        switch (WNature) {
                                            case "Grid":
                                                {
                                                    bindCustomWidget(this.Tid, "dv" + (i + 1), this.DBName, (i + 1));
                                                }
                                                break;
                                            case "Chart":
                                                {
                                                    GetPiChartWidget(this.Tid, "dv" + (i + 1), this.DBName, (i + 1));
                                                    //alert("Hi I am going to bind chart now.");
                                                }
                                                break;
                                        }
                                    }
                            }
                            //Each function end here
                        });
                    }
                },
                error: function (data) {
                    //alert("Hi Error occured while calling!!!");
                }
                //Ajax call end here 
            });
            //ready function end here
        });
        function bindUseFulLink(DbName, Divid) {
            $.ajax({
                type: "POST",
                url: "MainHome.aspx/GetUsefullLink",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //var Docdata = $.parseJSON(res);
                    //alert(res.DocList);
                    $("#" + Divid).html("").show();
                    $("#" + Divid).append("<h3>" + DbName + "</h3>");
                    //$("#dv2").append("<h3>" + DbName + "</h3>");
                    var Docdata = {}, SsoList = {};
                    if (res.DocList != "")
                        Docdata = $.parseJSON(res.DocList);
                    if (res.SsoList != "")
                        SsoList = $.parseJSON(res.SsoList);
                    var dbtype = ""
                    $.each(SsoList, function (i) {
                        $("#" + Divid).append("<span>Please click on Help Desk to create support ticket or email us at support@myndhrohd.zendesk.com<br/><br/>Helpdesk No: 0124-4724693<br/></span>");
                        $("#" + Divid).append("<a class='SSOLink' style='cursor:pointer;' id='ssllnk" + i + "'>" + this.DisplayName + "</a>");
                        $("#ssllnk" + i).bind('click', Showlinkredirecr);
                        //Each function end here
                    });
                    var Str = "<ul class='doclink'>"
                    $.each(Docdata, function () {
                        Str = Str + "<li><a href='" + this.url + "'>" + this.displayName + "</a></li>"
                        //Each function end here
                    });
                    Str = Str + "</ul>"
                    $("#" + Divid).append(Str);
                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
        }
        function GetPiChartWidget(Obj, Divid, DbName, index) {
            var t = '{"tid":"' + Obj + '"}';
            $.ajax({
                type: "POST",
                url: "MainHome.aspx/GetPiChartWidget",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    $("#" + Divid).html("").show().removeClass("pos1");
                    //$("#" + Divid).html("").show().removeClass("pos1").resizable().draggable();;
                    // $("#" + Divid).html("").removeClass("pos")
                    $("#" + Divid).append("<h3>" + DbName + "</h3>");
                    $("#" + Divid).append("<div  id='kpi" + index + "' style='height:200px;position:absolute;top:0;left:0;bottom:0;margin:0 auto;'></div>");
                    var chartID = "kpi" + index;
                    var d1 = res.data;
                    createChart(d1, "test", chartID);
                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
        }
        function createChart(data1, Name, chartID) {
            $("#" + chartID).kendoChart({
                title: {
                    position: "bottom"
                },
                legend: {
                    visible: true
                },
                chartArea: {
                    background: ""
                },
                seriesDefaults: {
                    labels: {
                        visible: false,
                        background: "transparent",
                        template: "#= category #"
                    }
                },
                series: [{
                    type: "pie",
                    startAngle: 150,
                    data: data1
                }],
                tooltip: {
                    visible: true,
                    template: "${ category } - ${ value }"
                }
            });
        }
        function bindCustomWidget(Obj, Divid, DbName, index) {
            var t = '{"tid":"' + Obj + '"}';
            $.ajax({
                type: "POST",
                url: "MainHome.aspx/GetCustomWidget",
                contentType: "application/json; charset=utf-8",
                data: t,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    $("#" + Divid).html("").show();
                    $("#" + Divid).append("<h3>" + DbName + "</h3>");
                    $("#" + Divid).append("<div id='kgrd" + index + "'></div>");
                    var data1 = $.parseJSON(res.Data);
                    var Column = res.Column
                    bindGrid(data1, Column, "kgrd" + index);
                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
        }

        function bindGrid(Data1, Column, grid) {
            $("#" + grid).kendoGrid({
                dataSource: {
                    data: Data1
                },
                scrollable: true,
                resizable: true,
                reorderable: true,
                sortable: true,
                filterable: true
            });
        }

        function Showlinkredirecr() {
            //'//SholinkURL
            $.ajax({
                type: "POST",
                url: "MainHome.aspx/SholinkURL",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    window.open(res);
                },
                error: function (data) {
                    //Will write code later 
                }
                //Ajax call end here 
            });
        }
    </script>

</asp:Content>
