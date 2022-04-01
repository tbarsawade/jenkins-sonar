<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="MIS_DashBord_Config, App_Web_2echgblw" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .scrollingControlContainer {
            overflow-x: hidden;
            overflow-y: scroll;
        }

        .scrollingCheckBoxList {
            border: 1px #808080 solid;
            margin: 10px 10px 10px 10px;
            height: 300px;
            margin-left: -2px
        }

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
        function ValidateCheckBoxList() {

            var listItems = document.getElementById('<%=chkbxFilterStatus.ClientID%>').getElementsByTagName("input");
            var txtalias = document.getElementById('<%=txtalias.ClientID%>').value
            <%--var txtorder = document.getElementById('<%=txtorderid.ClientID%>').value--%>

            var itemcount = listItems.length;
            var iCount = 0;
            var isItemSelected = false;
            for (iCount = 0; iCount < itemcount; iCount++) {
                if (listItems[iCount].checked) {
                    isItemSelected = true;
                    break;
                }
            }
            if (!isItemSelected) {
                alert("Please select an Item.");
            }
            if (txtalias == '') {
                alert('Please Enter Alias Name!!')
            }
            //if (txtorder == '') {
            //    alert('Please Enter Order Number!!')
            //}


            else {
                return true;
            }
            return false;
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
                        MIS DashBord Configuration
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
                                <asp:ListItem Text="DBName">DBName</asp:ListItem>
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
                                ToolTip="Add User" OnClick="Add" />
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
                                <div class="col-md-12 col-sm-12">
                                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                        <ProgressTemplate>
                                            <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                please wait...
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <asp:GridView ID="gvDashboardData" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                        DataKeyNames="Tid" ForeColor="#333333" Width="100%" AllowSorting="True" PageSize="10">
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
                                            <asp:BoundField DataField="DBName" HeaderText="Dash Board Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ChartType" HeaderText="Chart Type">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Createdon" HeaderText="Created On">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Roles" HeaderText="Role Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Isactive" HeaderText="Is Active ">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Dord" HeaderText="Display Order" ControlStyle-Width="15px">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.png" Height="16px"
                                                        Width="16px" OnClick="EditHit" ToolTip="Edit DashBoard Details" AlternateText="Edit" />
                                                    &nbsp;
                                                    <asp:ImageButton ID="btnAddChildFields" runat="server" ImageUrl="~/images/ChildItem.jpg" Height="16px" Width="16px" ToolTip="Add Chart Fields" AlternateText="Edit" OnClick="child" />
                                                    
                                                    &nbsp;
                                                    <asp:ImageButton ID="btnimgdetail" runat="server" ImageUrl="~/images/additemdetailm.gif" Height="16px" Width="16px" ToolTip="Add Detail Fields" AlternateText="Add Detail Item Fields" OnClick="Detail" />
                                                   

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
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="1000px" BackColor="#FFFFFF" Height="500px" Style="overflow: scroll">
        <div>
            <div class="container-fluid">
                <div class="doc_header">
                    Add/Update DashBoard Configuration
                    <div style="float: right;">
                        <asp:ImageButton ID="btnCloseEdit" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png" />
                    </div>
                </div>
            </div>
            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div class="alert alert-danger" id="popAlert" runat="server">
                                <strong>Alert!</strong>
                                <asp:Label ID="lblMsgEdit" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th scope="col">
                                    <label>DB Name</label></th>
                                <td>
                                    <asp:HiddenField ID="hdnTID" runat="server" />
                                    <asp:DropDownList ID="txtDBName" runat="server" CssClass="txtBox" Width="195px">
                                        <asp:ListItem>Select</asp:ListItem>
                                        <asp:ListItem>Expense Breakup</asp:ListItem>
                                        <asp:ListItem>Invoice LifeCycle</asp:ListItem>
                                        <asp:ListItem>Supplier Spend Breakup</asp:ListItem>
                                        <asp:ListItem>Invoice Distribution</asp:ListItem>
                                        <asp:ListItem>Open Invoice Ageing</asp:ListItem>
                                        <asp:ListItem>SLA Performance</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>
                                    <label>Chart Type</label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="ddlcharttype" runat="server"
                                        CssClass="txtBox">
                                        <asp:ListItem Value="Pie">Pie</asp:ListItem>
                                        <asp:ListItem Value="Stack">Stack</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th scope="col">
                                    <label style="margin-top: -310px">Allowed Role</label>
                                </th>
                                <td>
                                    <asp:Panel ID="checkBoxPanel" runat="server"
                                        CssClass="scrollingControlContainer scrollingCheckBoxList">
                                        <asp:CheckBox ID="chkAllowroll" onclick="check(this);" runat="server" Text="Select All" CssClass="chkmar" />
                                        <asp:CheckBoxList ID="CheckBoxList1" runat="server"
                                            CssClass="txtBox" Height="50px">
                                        </asp:CheckBoxList>
                                        <asp:HiddenField ID="hdnallowedroll" runat="server" />
                                    </asp:Panel>
                                </td>
                                <th scope="col">
                                    <label style="margin-top: -310px">IsActive</label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="ddlisactive" runat="server" CssClass="txtBox">
                                        <asp:ListItem Value="1">True</asp:ListItem>
                                        <asp:ListItem Value="0">False</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" align="center">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                        Width="100px"
                                        ValidationGroup="a" OnClientClick="return validate();" OnClick="btnSubmit_Click" />
                                </td>
                            </tr>
                        </thead>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <asp:Button ID="btnShowPopupAdd" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupAddchilditem" runat="server" TargetControlID="btnShowPopupAdd"
        PopupControlID="Panel1" CancelControlID="imgcloseedit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="Panel1" runat="server" Width="1000px" BackColor="#FFFFFF" Height="500px" Style="overflow: scroll">
        <div>
            <div class="container-fluid">
                <div class="doc_header">
                    Add/Update DashBoard ChildItem
                    <div style="float: right;">
                        <asp:ImageButton ID="imgcloseedit" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png" />
                    </div>
                </div>
            </div>
            <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panel2" runat="server">
                        <div class="row">
                            <div class="col-md-12 col-sm-12">
                                <div class="alert alert-danger alert-dismissible" id="Div1" runat="server" role="alert">
                                    <strong>Alert!</strong>
                                    <asp:Label ID="Label1" runat="server"></asp:Label>
                                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="table-responsive">
                            <table class="table">
                                <thead>
                                    <tbody>
                                        <tr id="trtid" runat="server">
                                            <td>
                                                <label style="visibility: hidden">Tid</label>

                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtchildTid" runat="server" Text="0" CssClass="txtBox">
                                                </asp:TextBox>
                                                <asp:HiddenField ID="hdnfield" runat="server" />
                                            </td>
                                        </tr>
                                        <tr runat="server">
                                            <td>
                                                <label>DB Name</label>

                                            </td>
                                            <td>
                                                <asp:TextBox ID="ddldbname" runat="server" CssClass="txtBox">
                                                    
                                                </asp:TextBox>
                                                <asp:HiddenField ID="HiddenField1" runat="server" />
                                            </td>
                                            <td colspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Document Type</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddldoctype" runat="server" CssClass="txtBox" OnSelectedIndexChanged="ddldoctype_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                            <td>
                                                <label>Type</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddltype" runat="server" CssClass="txtBox">
                                                    <asp:ListItem>Department</asp:ListItem>
                                                    <asp:ListItem>Expence Nature</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:HiddenField ID="hdnrefid" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Catagory Field</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlcatfield" runat="server" CssClass="txtBox" OnSelectedIndexChanged="ddlcatfield_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <label>Value Field</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlvaluefield" runat="server" CssClass="txtBox"></asp:DropDownList>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Recv Date field</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlrecvdatefield" runat="server" CssClass="txtBox" OnSelectedIndexChanged="ddlrecvdatefield_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <label>Invoice Date Field</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlinvdatefield" runat="server" CssClass="txtBox"></asp:DropDownList>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" align="right">
                                                <asp:Button ID="addgridvalue" runat="server" Text="Add" CssClass="btnNew" OnClick="subchildbind" />
                                            </td>
                                        </tr>
                                        <tr>

                                            <td colspan="6">
                                                <asp:GridView ID="gridchilditemdata" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                                    DataKeyNames="RefTid" ForeColor="#333333" Width="100%" AllowSorting="True" PageSize="10">
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
                                                        <asp:BoundField DataField="RefTid" HeaderText="Ref TID">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="DocumentType" HeaderText="Document Type">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Type" HeaderText="Type">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="CatField" HeaderText="Catagory">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="ValueField" HeaderText="Value Field">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="RecdateField" HeaderText="RecieveDate Field">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="InvDateField" HeaderText="InvoiceDate Field">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="EditshowchildItem" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip="Show Child Fields" AlternateText="Show Child Item Fields" OnClick="EditshowchildItem_Click" />
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
                                            </td>
                                        </tr>
                                        <tr id="trwf" runat="server">
                                            <td colspan="8">
                                                <div class="table-responsive">
                                                    <table class="table">
                                                        <tr>
                                                            <td>
                                                                <asp:HiddenField ID="hdnWorkFlow" runat="server" />
                                                                <label>WorkFlow</label>
                                                            </td>
                                                            <td style="overflow-y: scroll;">
                                                                <asp:CheckBoxList ID="chkbxFilterStatus" runat="server" CssClass="txtBox">
                                                                </asp:CheckBoxList>
                                                            </td>
                                                            <td>
                                                                <label>Alias Name</label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtalias" runat="server" CssClass="txtBox" Width="165px"></asp:TextBox>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label>Order</label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtorderid" runat="server" CssClass="txtBox" Width="75px"></asp:TextBox>
                                                            </td>
                                                            <td colspan="4" align="right">
                                                                <asp:Button ID="btnselect" runat="server" Text="Add" CssClass="btnNew" OnClientClick="return ValidateCheckBoxList()" OnClick="Insert" />
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="8" align="center">
                                                                <asp:GridView ID="Griddatediff" runat="server"
                                                                    AutoGenerateColumns="false" ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="true" PageSize="10" EmptyDataText="No records has been added." DataKeyNames="Tid">
                                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                    <RowStyle BackColor="#EFF3FB" />
                                                                    <EditRowStyle BackColor="#2461BF" />
                                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                                                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                                                    <AlternatingRowStyle BackColor="White" />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="S.No" ItemStyle-Width="50">
                                                                            <ItemTemplate>
                                                                                <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="Tid" HeaderText="Tid" ItemStyle-Width="50">
                                                                            <HeaderStyle HorizontalAlign="Left" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="DocumentType" HeaderText="DocumentType" ItemStyle-Width="120">
                                                                            <HeaderStyle HorizontalAlign="Left" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="WorkFlow" HeaderText="WorkFlow" ItemStyle-Width="170">
                                                                            <HeaderStyle HorizontalAlign="Left" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="alias" HeaderText="alias" ItemStyle-Width="120">
                                                                            <HeaderStyle HorizontalAlign="Left" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="orderid" HeaderText="orderid" ItemStyle-Width="50">
                                                                            <HeaderStyle HorizontalAlign="Left" />
                                                                        </asp:BoundField>
                                                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="50">
                                                                            <ItemTemplate>
                                                                                <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/images/Delete.gif" Height="16px" Width="16px" ToolTip="Delete Record" AlternateText="Delete Record" OnClick="imgDelete_Click" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />

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
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        
                                        <tr>
                                            <td colspan="4">
                                                <div style="width: 100%; text-align: center">
                                                    <asp:Button ID="btnchild" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                                        Width="100px"
                                                        ValidationGroup="a" OnClientClick="return validate();" OnClick="btnchild_Click" />
                                                </div>
                                            </td>
                                        </tr>

                                    </tbody>
                                </thead>
                            </table>
                        </div>
                        <br />
                    </asp:Panel>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <asp:Button ID="btnshowchildItem" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtendershowchilditem" runat="server" TargetControlID="btnshowchildItem"
        PopupControlID="pnlchildshow" CancelControlID="imgclosechildshow" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlchildshow" runat="server" Width="1000px" BackColor="#FFFFFF" Height="500px" Style="overflow: scroll">
        <div>
            <div class="container-fluid">
                <div class="doc_header">
                    Update DashBoard ChildItem
                    <div style="float: right;">
                        <asp:ImageButton ID="imgclosechildshow" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png" />
                    </div>
                </div>
            </div>
            <asp:UpdatePanel ID="updtpnlchildshow" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div class="alert alert-danger" id="Div2" runat="server">
                                <strong>Alert!</strong>
                                <asp:Label ID="Label2" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                <ProgressTemplate>
                                    <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                        <asp:Image ID="imgprogress" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                        please wait...
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>

                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <asp:Button ID="btndetailAdd" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtenderDetailfield" runat="server" TargetControlID="btndetailAdd"
        PopupControlID="pnladddetailsitem" CancelControlID="ImageButton1" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnladddetailsitem" runat="server" Width="1000px" BackColor="#FFFFFF" Height="500px" Style="overflow: scroll">
        <div>
            <div class="container-fluid">
                <div class="doc_header">
                    Add Detail Fields
                    <div style="float: right;">
                        <asp:ImageButton ID="ImageButton1" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png" />
                    </div>
                </div>
            </div>
            <asp:UpdatePanel ID="updtpnladddetailsfields" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12 col-sm-12">
                            <div class="alert alert-danger" id="Div3" runat="server">
                                <strong>Alert!</strong>
                                <asp:Label ID="lbldetailmsg" runat="server"></asp:Label>
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="table-responsive">
                        <table class="table">
                            <thead>
                                <tbody>
                                    <tr id="tr1" runat="server">
                                        <td>
                                            <label>Display Name</label>
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hdndetail" runat="server" />
                                            <asp:DropDownList ID="ddlDisplayName" runat="server" CssClass="txtBox" Width="170px">
                                                <asp:ListItem Value="">Select</asp:ListItem>
                                                <asp:ListItem>Bar Code</asp:ListItem>
                                                <asp:ListItem>Invoice No</asp:ListItem>
                                                <asp:ListItem>Invoice Date</asp:ListItem>
                                                <asp:ListItem>Vendor Name</asp:ListItem>
                                                <asp:ListItem>Vendor Code</asp:ListItem>
                                                <asp:ListItem>Currency</asp:ListItem>
                                                <asp:ListItem>Department</asp:ListItem>
                                                <asp:ListItem>Total Amount</asp:ListItem>
                                                <asp:ListItem>Current Status</asp:ListItem>
                                                <asp:ListItem>Last Action Date</asp:ListItem>
                                                <asp:ListItem>DocID</asp:ListItem>

                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <label>Document Type</label></td>
                                        <td>
                                            <asp:DropDownList ID="documentTypeddl" runat="server" CssClass="txtBox" OnSelectedIndexChanged="documentTypeddl_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Fields</label></td>
                                        <td>
                                            <asp:DropDownList ID="fieldsdropdown" runat="server" CssClass="txtBox" Width="170px"></asp:DropDownList></td>
                                        <td>
                                            <label>Type</label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="Dropdowntype" runat="server" CssClass="txtBox">
                                                <asp:ListItem>Department</asp:ListItem>
                                                <asp:ListItem>Expence Nature</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" colspan="4">
                                            <asp:Button ID="btnadddetail" runat="server" Text="Add" CssClass="btnNew" OnClick="InsertDetail" />
                                        </td>

                                    </tr>
                                    <tr>
                                        <td colspan="8" align="center">
                                            <asp:GridView ID="detailgriditem" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                DataKeyNames="RefTid" ForeColor="#333333" Width="100%" AllowSorting="True" PageSize="10">
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
                                    <asp:BoundField DataField="RefTid" HeaderText="Ref TID">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DisplayName" HeaderText="Display Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DocumentType" HeaderText="Document Type">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Fields" HeaderText="Fields">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="EditshowdetailItem" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px" ToolTip="Show Detail Fields" AlternateText="Show Detail Item Fields" OnClick="EditshowdetailItem_Click" />
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
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <div style="width: 100%; text-align: center">
                                                <asp:Button ID="btnsavedetail" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                                    Width="100px"
                                                    ValidationGroup="a" OnClick="btnsavedetail_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </thead>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        </asp:Panel>
</asp:Content>

