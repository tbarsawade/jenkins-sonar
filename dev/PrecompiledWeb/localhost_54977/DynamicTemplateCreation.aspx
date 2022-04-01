<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="DynamicTemplateCreation, App_Web_fdh01zus" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<style type="text/css">
        .btnNew1 {
            height: 30px;
            min-width: 40px;
            box-shadow: 2px 2px 2px 1px #888888;
            margin: 5px 2px;
            color: #fff;
            background-color: #428bca;
            border-color: #357ebd;
            display: inline-block;
            padding: 6px 12px;
            margin-bottom: 0;
            font-size: 13px !important;
            font-weight: normal;
            line-height: 1.428571429;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            cursor: pointer;
            background-image: none;
            border: 1px solid transparent;
            border-radius: 4px;
            -webkit-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            -o-user-select: none;
            margin-top: -1px;
        }
         
    </style>
    <style type="text/css">
        .mydatagrid {
            width: 80%;
            border: solid 2px black;
            min-width: 80%;
        }

        .header {
            background-color: #646464;
            font-family: Arial;
            color: White;
            border: none 0px transparent;
            height: 25px;
            text-align: center;
            font-size: 10px;
        }

        .rows {
            background-color: #fff;
            font-family: Arial;
            font-size: 10px;
            color: #000;
            min-height: 25px;
            text-align: left;
            border: none 0px transparent;
        }

            .rows:hover {
                background-color: #ff8000;
                font-family: Arial;
                color: #fff;
                text-align: left;
            }

        .selectedrow {
            background-color: #ff8000;
            font-family: Arial;
            color: #fff;
            font-weight: bold;
            text-align: left;
        }

        .mydatagrid a /** FOR THE PAGING ICONS  **/ {
            background-color: Transparent;
            padding: 5px 5px 5px 5px;
            color: #fff;
            text-decoration: none;
            font-weight: bold;
        }

            .mydatagrid a:hover /** FOR THE PAGING ICONS  HOVER STYLES**/ {
                background-color: #000;
                color: #fff;
            }

        .mydatagrid span /** FOR THE PAGING ICONS CURRENT PAGE INDICATOR **/ {
            background-color: #c9c9c9;
            color: #000;
            padding: 5px 5px 5px 5px;
        }

        .pager {
            background-color: #646464;
            font-family: Arial;
            color: White;
            height: 30px;
            text-align: left;
        }

        .mydatagrid td {
            padding: 3px;
        }

        .mydatagrid th {
            padding: 3px;
        }
    </style>
    <style type="text/css">
        .btnNewgridview {
            text-decoration: none;
            text-align: center;
            font-weight: bold;
            font-size: 12px;
            background: url(../images/pix.png) repeat-x;
            color: #000;
            height: 24px;
            background-color: #f00;
        }


            .btnNewgridview:hover {
                text-decoration: none;
                background: url(../images/nav_hover.gif) repeat-x;
            }
    </style>

    <style type="text/css">
        .chkmar {
            margin-left: -0px;
        }

        .fancy-green .ajax__tab_header {
            background: url(images/green_bg_Tab.gif) repeat-x;
            cursor: pointer;
        }

        .fancy-green .ajax__tab_hover .ajax__tab_outer, .fancy-green .ajax__tab_active .ajax__tab_outer {
            background: url(images/green_left_Tab.gif) no-repeat left top;
        }

        .fancy-green .ajax__tab_hover .ajax__tab_inner, .fancy-green .ajax__tab_active .ajax__tab_inner {
            background: url(images/green_right_Tab.gif) no-repeat right top;
        }

        .fancy .ajax__tab_header {
            font-size: 13px;
            font-weight: bold;
            color: #000;
            font-family: sans-serif;
        }

            .fancy .ajax__tab_active .ajax__tab_outer, .fancy .ajax__tab_header .ajax__tab_outer, .fancy .ajax__tab_hover .ajax__tab_outer {
                height: 46px;
            }

            .fancy .ajax__tab_active .ajax__tab_inner, .fancy .ajax__tab_header .ajax__tab_inner, .fancy .ajax__tab_hover .ajax__tab_inner {
                height: 46px;
                margin-left: 16px; /* offset the width of the left image */
            }

            .fancy .ajax__tab_active .ajax__tab_tab, .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_header .ajax__tab_tab {
                margin: 16px 16px 0px 0px;
            }

        .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_active .ajax__tab_tab {
            color: #fff;
        }

        .fancy .ajax__tab_body {
            font-family: Arial;
            font-size: 10pt;
            border-top: 0;
            border: 1px solid #999999;
            padding: 8px;
            background-color: #ffffff;
        }
    </style>
    <style type="text/css">
        .mg20 {
            margin-top: 20px;
            height: 327px;
        }
    </style>
    <style>
        .checkbox23 {
            display: block;
            min-height: 20px;
            padding-left: 20px;
            margin-top: 10px;
            text-align: left;
            margin-bottom: 10px;
            vertical-align: middle;
        }
    </style>
    <script type="text/javascript">
        function check(checkbox) {

            var cbl = document.getElementById('<%=CheckBoxList1.ClientID%>').getElementsByTagName("input");
            for (i = 0; i < cbl.length; i++) cbl[i].checked = checkbox.checked;
        }
    </script>
    <script type="text/javascript">
        function validate() {
            debugger
            var txtReptName = document.getElementById('<%=txtReptName.ClientID%>');
            var txtdescription = document.getElementById('<%=txtdescription.ClientID%>');

            if (txtReptName.value == "") {
                txtReptName.focus();
                alert('Please Enter Report Name !!');
                return;
            }
            if (txtdescription.value == "") {
                txtdescription.focus();
                alert('Please Enter Report Description!!');
                return;
            }
            if (ddldocumenttype.value == 'Select') {
                ddldocumenttype.focus();
                alert('Please Select Document Type !!');
                return;
            }

        }
    </script>
      
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnActEdit" />--%>
            <asp:PostBackTrigger ControlID="btnexport" />
        </Triggers>
        <ContentTemplate>
            <div class="container-fluid">
                <div class="form">
                    <div class="doc_header">
                        Dynamic Template Creation
                    </div>
                    <div class="col-md-12 col-sm-12">
                        &nbsp;
                    </div>
                    <div class="row">
                        <div class="col-md-1 col-sm-1">
                            <label>Field Name</label>
                        </div>
                        <div class="col-md-3 col-sm-3">
                            <asp:DropDownList ID="ddlField" runat="server" CssClass="txtBox">
                                <asp:ListItem Text="Document Type">Document Type</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <label>Value</label>

                        </div>
                        <div class="col-md-3 col-sm-3">
                            <asp:TextBox ID="txtValue" runat="server" CssClass="txtBox"></asp:TextBox>
                        </div>
                        <div class="col-md-1 col-sm-1">
                            <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px" ToolTip="Search"
                                ImageUrl="~/Images/search.png" OnClick="btnSearch_Click" />
                        </div>
                        <div class="col-md-3 col-sm-3" style="text-align: right;">
                            <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg"
                                ToolTip="Add" OnClick="Add" />
                            &nbsp;<asp:ImageButton ID="btnexport" runat="server" ToolTip="Export" ImageUrl="~/Images/excel.gif" />
                        </div>
                        <div class="col-md-12 col-sm-12">
                            &nbsp;
                        </div>

                        <div class="col-md-12 col-sm-12">
                            <div class="alert alert-success fade in" id="msgUpdateDiv" runat="server" visible="false">
                                <a href="#" class="close" data-dismiss="alert">&times;</a>
                                <strong>Success!</strong>
                                <asp:Label ID="lblMsgupdate" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12" style="overflow: auto;">
                                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                        <ProgressTemplate>
                                            <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                please wait...
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <asp:GridView ID="gvrptData" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                        DataKeyNames="Tid" ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="true" PageSize="10" PagerSettings-PageButtonCount="10" OnPageIndexChanging="gvrptData_PageIndexChanging">
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <RowStyle BackColor="#EFF3FB" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                        <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="S.No">
                                                <ItemTemplate>
                                                    <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Tid" HeaderText="TID">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Temp_Name" HeaderText="Template Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="DocumentType" HeaderText="Document Type">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <%-- <asp:BoundField DataField="Docfields" HeaderText="Doc Fields">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>--%>
                                            <asp:BoundField DataField="Computedfield" HeaderText="Computed Field">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <%-- <asp:BoundField DataField="SortBy" HeaderText="Sort By">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>--%>
                                            <%-- <asp:BoundField DataField="Filterfield1" HeaderText="Filter Field1">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="FilterfieldsData1" HeaderText="Filter Fields Data1">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>--%>

                                            <%-- <asp:BoundField DataField="Filterfield2" HeaderText="Filter Field2">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="FilterfieldData2" HeaderText="Filter Field Data2">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>--%>
                                            <asp:BoundField DataField="FilterStatus" HeaderText="Filter Status">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="AllowedRoles" HeaderText="Allowed Roles">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btnDtl" ImageUrl="~/images/lock.png" runat="server" Height="16px"
                                                        Width="16px" OnClick="LockHit" ToolTip="Lock / Unlock User" AlternateText="Lock / Unlock" />
                                                    &nbsp;
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.png" Height="16px"
                                            Width="16px" OnClick="EditHit" ToolTip="Edit FTP Details" AlternateText="Edit" />
                                                    &nbsp;
                                                </ItemTemplate>
                                                <ItemStyle Width="90px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                        <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                        <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                        <SortedDescendingHeaderStyle BackColor="#002876" />
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupEdit"
        PopupControlID="pnlPopupEdit" CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="1150px" BackColor="#FFFFFF" Height="550px"
        Style="overflow: scroll">
        <div>
            <div class="container-fluid">
                <div class="doc_header">
                    Add/Update Dynamic Template Creation
                        <div style="float: right;">
                            <asp:ImageButton ID="btnCloseEdit" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png" OnClientClick="document.location.reload(true)" />
                        </div>
                </div>
                <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TabContainer ID="TabContainer1" runat="server" CssClass="fancy fancy-green" ActiveTabIndex="0">
                            <asp:TabPanel ID="tbpnluser" runat="server">
                                <HeaderTemplate>
                                    Report Name
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <asp:Panel ID="UserReg" runat="server">
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12">
                                                <div class="alert alert-danger" id="popAlert" runat="server">
                                                    <strong>Alert!</strong>
                                                    <asp:Label ID="lblMsgEdit" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Report Name<span style="color: red">*</span></label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:TextBox ID="txtReptName" runat="server" CssClass="txtBox"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Description</label><span style="color: red">*</span>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:TextBox ID="txtdescription" runat="server" CssClass="txtBox"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row" style="visibility: hidden">
                                            <div class="col-md-12 col-sm-12 mg20">
                                                <div class="col-md-2 col-sm-2" style="visibility: hidden">
                                                    <label>Template ID</label>
                                                </div>
                                                <div class="col-md-2 col-sm-2" style="visibility: hidden">
                                                    <asp:TextBox ID="txtTempID" runat="server" ReadOnly="true" Text="0" CssClass="txtBox"></asp:TextBox>
                                                    <asp:HiddenField ID="hdnTID" runat="server" />
                                                </div>
                                            </div>

                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="tbpnlusrdetails" runat="server">
                                <HeaderTemplate>
                                    Document Type
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <table border="1">
                                        <tr>
                                            <th colspan="3">
                                                <label style="width: 150px;">Document Type<span style="color: red">*</span></label>
                                                <asp:DropDownList ID="ddlDocumenttype" AutoPostBack="true" runat="server" CssClass="txtBox" Width="200px" OnSelectedIndexChanged="ddlDocumenttype_SelectedIndexChanged"></asp:DropDownList>
                                            </th>
                                            <th colspan="3">
                                                <label style="width: 150px;">Child Item<span style="color: red">*</span></label>
                                                <asp:DropDownList ID="ddlchilditem" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="ddlchilditem_SelectedIndexChanged"></asp:DropDownList>
                                            </th>
                                          
                                        </tr>
                                        <tr>
                                            <th>
                                                <lable>Fields</lable>
                                            </th>
                                            <th></th>
                                            <th>
                                                <lable>Selected Fields</lable>
                                            </th>
                                         <th style="width:150px"></th>
                                        <th></th>
                                        <th>Child ItemList</th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:ListBox ID="Listbox1" runat="server" SelectionMode="Multiple" Width="180" Height="280"></asp:ListBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="Up" runat="server" Text="▲" CssClass="btn-info" Width="60" Style="margin-left: 20px" OnClick="Up_Click" />
                                                <asp:Button ID="downArrow" runat="server" Text="▼" Width="60" CssClass="btn-info" Style="margin-left: 20px" OnClick="downArrow_Click" /><br />
                                                <br />
                                                <asp:Button ID="right" runat="server" Text="▶" CssClass="btn-primary" Width="60" Style="margin-left: 20px" OnClick="right_Click" />
                                                <asp:Button ID="leftClick" runat="server" Text="◀" Width="60" CssClass="btn-primary" Style="margin-left: 20px" OnClick="leftClick_Click" />
                                            </td>
                                            <td>
                                                <asp:ListBox ID="Listbox2" runat="server" SelectionMode="Multiple" Width="180" Height="280"></asp:ListBox>
                                            </td>
                                            <td></td>
                                            <td>
                                            <%--<asp:Button ID="Button1" runat="server" Text="▲" CssClass="btn-info" Width="60" Style="margin-left: 20px"/>
                                            <asp:Button ID="Button2" runat="server" Text="▼" Width="60" CssClass="btn-info" Style="margin-left: 20px"/><br />
                                            <br />--%>
                                            <asp:Button ID="btnrightchild" runat="server" Text="▶" CssClass="btn-primary" Width="60" Style="margin-left: 20px" OnClick="btnrightchild_Click"/>
                                            <asp:Button ID="btnleftchild" runat="server" Text="◀" Width="60" CssClass="btn-primary" Style="margin-left: 20px" OnClick="btnleftchild_Click"/>
                                        </td>
                                            <td>
                                                <asp:ListBox ID="ddlchildlist" runat="server"  SelectionMode="Multiple" Width="180" Height="280"></asp:ListBox>
                                            </td>
                                    </table>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="tbpnljobdetails" runat="server">
                                <HeaderTemplate>
                                    Stages Date Fields
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-2 col-sm-2">
                                                <label>Stages In Date</label>
                                            </div>
                                            <div class="col-md-2 col-sm-2" style="overflow-y: scroll; width: 30%; height: 305px; margin-top: -2px;">
                                                <asp:CheckBoxList ID="chkSID" runat="server"
                                                    CssClass="txtBox" Height="300px">
                                                </asp:CheckBoxList>
                                            </div>
                                            <div class="col-md-2 col-sm-2">
                                                <label>Stages Out Date</label>
                                            </div>
                                            <div class="col-md-2 col-sm-2" style="overflow-y: scroll; width: 30%; height: 305px; margin-top: -2px;">
                                                <asp:CheckBoxList ID="chkbxcomputedfeilds" runat="server"
                                                    CssClass="txtBox" Height="300px">
                                                </asp:CheckBoxList>
                                            </div>
                                            <div class="col-md-2 col-sm-2" style="width: 14%">
                                                <label>Sort By</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:DropDownList ID="ddlSortBy" runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="tabpnlfilter" runat="server">
                                <HeaderTemplate>
                                    Filter Data
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Select Date Filter Fields</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3" style="overflow-y: scroll; height:150px; width: 20%;">
                                                <asp:CheckBoxList ID="chkdatefltr" runat="server" 
                                                    CssClass="txtBox">
                                                </asp:CheckBoxList>
                                                <asp:HiddenField ID="HiddenField2" runat="server" />
                                                <%--<asp:DropDownList ID="ddlfilterfieldData1"  runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>--%>
                                            </div>
                                        </div>
                                        <div style="height:10px">
                                            <table>
                                                <tr>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Fix Valued Fields</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:DropDownList ID="ddlfixval" runat="server" AutoPostBack="true" CssClass="txtBox" Width="200px" OnSelectedIndexChanged="ddlfixval_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Fix Valued Data</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3" style="overflow-y: scroll; height:200px; width: 30%; margin-left: 180px; margin-top: -25px;">
                                                <asp:CheckBoxList ID="chkfixval" runat="server" 
                                                    CssClass="txtBox" >
                                                </asp:CheckBoxList>
                                                <asp:HiddenField ID="HiddenField1" runat="server" />

                                                <%--<asp:DropDownList ID="ddlfilterfieldData1"  runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>--%>
                                            </div>
                                        </div>
                                        <div style="height:10px"></div>
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Master Valued Fields1</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:DropDownList ID="ddlFilterfeild1" runat="server" AutoPostBack="true" CssClass="txtBox" Width="200px" OnSelectedIndexChanged="ddlFilterfeild1_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Master Valued Data1</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3" style="overflow-y: scroll; height:200px; width: 30%; margin-left: 180px; margin-top: -25px;">
                                                <asp:CheckBoxList ID="ckbxfilterfield" runat="server" 
                                                    CssClass="txtBox" >
                                                </asp:CheckBoxList>
                                                <asp:HiddenField ID="hdnfilterfielddata1" runat="server" />

                                                <%--<asp:DropDownList ID="ddlfilterfieldData1"  runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>--%>
                                            </div>
                                        </div>
                                        <div style="height:10px"></div>
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Master Valued Fields2</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:DropDownList ID="ddlfilterfield2" runat="server" CssClass="txtBox" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="ddlfilterfield2_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Master Valued Data2</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3" style="overflow-y: scroll; width: 30%; height: px; margin-left: 180px; margin-top: -15px;">
                                                <asp:CheckBoxList ID="chbxfilterfielddata2" runat="server" CssClass="txtBox" Height="200px">
                                                </asp:CheckBoxList>
                                                <%--<asp:DropDownList ID="ddlfilterfieldData2" runat="server" CssClass="txtBox" Width="200px"></asp:DropDownList>--%>
                                            </div>
                                        </div>                                                                            
                                        
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="filterfeild2" runat="server">
                                <HeaderTemplate>
                                    Static Fields
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Last Work Flow Stage</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:CheckBox ID="chkLWFS" runat="server" CssClass="txtBox" />
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Last User Name</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:CheckBox ID="chkLUN" runat="server" CssClass="txtBox" />
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Last Action Date</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:CheckBox ID="chkLAD" runat="server" CssClass="txtBox" />
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Creation date</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:CheckBox ID="chkCD" runat="server" CssClass="txtBox" />
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Current User Name</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <asp:CheckBox ID="chkCUN" runat="server" CssClass="txtBox" />
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="TabPanel1" runat="server">
                                <HeaderTemplate>
                                    Stages
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 mg20">
                                            <div class="col-md-3 col-sm-3">
                                                <label>Filter Stages</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3" style="overflow-y: scroll; width: 30%; height: 305px; margin-top: -2px;">
                                                <asp:CheckBoxList ID="chkbxFilterStatus" runat="server" CssClass="txtBox">
                                                </asp:CheckBoxList>

                                            </div>
                                            <div class="col-md-3 col-sm-3">
                                                <label>Allowed Role</label>
                                            </div>
                                            <div class="col-md-3 col-sm-3" style="overflow-y: scroll; width: 30%; height: 305px; margin-left: 127px; margin-top: -35px;">
                                                <asp:CheckBox ID="chkDocType" onclick="check(this);" runat="server" Text="Select All" CssClass="chkmar" />
                                                <asp:CheckBoxList ID="CheckBoxList1" runat="server"
                                                    CssClass="txtBox">
                                                </asp:CheckBoxList>
                                                <asp:HiddenField ID="hdnallowedroll" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel ID="tblpnl" runat="server">
                                <HeaderTemplate>
                                    Date Difference
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <table class="table">
                                            <thead>
                                                <tbody>
                                                    <tr id="trtid" runat="server">
                                                        <td>
                                                            <label>Date Field1</label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddldatetypefield1" runat="server" CssClass="txtBox" Width="165px" OnSelectedIndexChanged="ddldatetypefield1_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <label>Date Field2 </label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddldatetypefield2" runat="server" CssClass="txtBox" Width="165px"></asp:DropDownList>

                                                        </td>
                                                        <td>
                                                            <label>Display Name</label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtdisplayname" runat="server" CssClass="txtBox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8" align="right">
                                                            <asp:HiddenField ID ="btndatediffhdn" runat="server" />
                                                            <asp:Button ID="btnselect" runat="server" Text="Add" CssClass="btnNew pull right" Font-Bold="True"
                                                                Width="100px" ValidationGroup="a" OnClick="Insert" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8" align="center">
                                                            <asp:GridView ID="Griddatediff" runat="server"
                                                                AutoGenerateColumns="false" ForeColor="#333333" Width="100%" AllowSorting="True" PageSize="10" EmptyDataText="No records has been added." DataKeyNames="Tid">
                                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                <RowStyle BackColor="#EFF3FB" />
                                                                <EditRowStyle BackColor="#2461BF" />
                                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                                                <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                                                <AlternatingRowStyle BackColor="White" />
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="S.No">
                                                                        <ItemTemplate>
                                                                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="Tid" HeaderText="Tid" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Documenttype" HeaderText="DocumentType" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="First_Field" HeaderText="First_Field" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Second_field" HeaderText="Second_Field" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="DisplayName" HeaderText="Display_Name" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                     <asp:BoundField DataField="Type" HeaderText="TypeFirstField" Visible="false" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="TypeSecond_field" HeaderText="TypeSecondField" Visible="false" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>

                                                                    <asp:TemplateField HeaderText="Action">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/images/Delete.gif" Height="16px" Width="16px" ToolTip="Delete Record" AlternateText="Delete Record" OnClick="imgDelete_Click" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </thead>
                                        </table>
                                    </div>
                                    <div class="row" style="margin-top: 30px">
                                        <div style="width: 100%; text-align: center">
                                            <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                                Width="100px"
                                                ValidationGroup="a" OnClientClick="return validate();" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <%--<asp:TabPanel ID="tabstatusdiff" runat="server">
                                <HeaderTemplate>
                                    Stages Date Difference
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <table class="table">
                                            <thead>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <label>Stage1</label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlstatusdiff1" runat="server" AutoPostBack="true" CssClass="txtBox" OnSelectedIndexChanged="ddlstatusdiff1_SelectedIndexChanged" Width="200px"></asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <label>Stage2</label></td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlstatusdiff2" runat="server" CssClass="txtBox" Width="180px"></asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <label>Display Name</label></td>
                                                        <td>
                                                            <asp:TextBox ID="txtdisplaynamestatusdiff" runat="server" CssClass="txtBox" Width="180px"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8" align="right">
                                                            <asp:Button ID="btnstatusdiff" runat="server" Text="Add" CssClass="btnNew" Font-Bold="True" Width="100px" ValidationGroup="a" OnClick="InsertStatus" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8" align="center">
                                                            <asp:GridView ID="gvrstatusdiff" runat="server" AutoGenerateColumns="false" ForeColor="#333333" DataKeyNames="Tid"
                                                                Width="100%" AllowSorting="True" PageSize="10" EmptyDataText="No records has been added.">
                                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                <RowStyle BackColor="#EFF3FB" />
                                                                <EditRowStyle BackColor="#2461BF" />
                                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                                                <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                                                <AlternatingRowStyle BackColor="White" />
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="S.No">
                                                                        <ItemTemplate>
                                                                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="Tid" HeaderText="Tid" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Documenttype" HeaderText="DocumentType" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="First_Field" HeaderText="First_Field" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Second_field" HeaderText="Second_Field" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="DisplayName" HeaderText="DisplayName" ItemStyle-Width="120">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:TemplateField HeaderText="Action">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="Deleteimg" runat="server" ImageUrl="~/images/Delete.gif" Height="16px" Width="16px" ToolTip="Delete Record" AlternateText="Delete Record" OnClick="Deleteimg_Click" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                                                <SortedAscendingCellStyle BackColor="#EDF6F6" />
                                                                <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                                                                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                                                                <SortedDescendingHeaderStyle BackColor="#002876" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </thead>
                                        </table>
                                    </div>
                                    
                                </ContentTemplate>
                            </asp:TabPanel>--%>
                        </asp:TabContainer>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>

    
</asp:Content>

